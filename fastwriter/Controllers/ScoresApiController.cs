using fastwriter.Data;
using fastwriter.Models;
using fastwriter.Services;
using fastwriter.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fastwriter.Controllers;

[ApiController]
[Route("api/scores")]
[Authorize]
public class ScoresApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ScoresApiController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    /// <summary>
    /// İstemciden gelen yazılan metni, veritabanındaki referans metinle karşılaştırarak
    /// metrikleri sunucuda yeniden hesaplar ve skoru kaydeder.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save([FromBody] ScoreSubmissionDto dto)
    {
        if (dto.TextId <= 0)
        {
            return BadRequest(new { message = "Geçersiz sınav metni." });
        }

        var typedText = (dto.TypedText ?? string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\n", " ");

        if (typedText.Length == 0)
        {
            return BadRequest(new { message = "Yazılan metin boş olamaz." });
        }

        if (typedText.Length > 10_000)
        {
            return BadRequest(new { message = "Yazılan metin izin verilen uzunluğu aşıyor." });
        }

        if (dto.DurationSeconds < 3 || dto.DurationSeconds > ExamContent.DurationSeconds + 5)
        {
            return BadRequest(new { message = "Geçersiz test süresi." });
        }

        var text = await _db.TypingTexts
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == dto.TextId && t.IsActive);

        if (text is null)
        {
            return NotFound(new { message = "Sınav metni bulunamadı." });
        }

        var userId = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Skor kaydetmek için giriş yapmalısınız." });
        }

        var target = ExamContent.Build(text.Content);
        var metrics = TypingMetrics.Calculate(target, typedText, dto.DurationSeconds);

        var score = new Score
        {
            UserId = userId,
            TypingTextId = text.Id,
            NetWpm = metrics.NetWpm,
            RawWpm = metrics.RawWpm,
            Accuracy = metrics.Accuracy,
            CorrectChars = metrics.CorrectChars,
            IncorrectChars = metrics.IncorrectChars,
            TotalChars = metrics.TotalChars,
            CorrectWords = metrics.CorrectWords,
            DurationSeconds = dto.DurationSeconds,
            CreatedAt = DateTime.UtcNow
        };

        _db.Scores.Add(score);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            id = score.Id,
            netWpm = score.NetWpm,
            rawWpm = score.RawWpm,
            accuracy = score.Accuracy,
            correctChars = score.CorrectChars,
            incorrectChars = score.IncorrectChars,
            totalChars = score.TotalChars,
            correctWords = score.CorrectWords,
            durationSeconds = score.DurationSeconds
        });
    }
}
