using fastwriter.Data;
using fastwriter.Models;
using fastwriter.Services;
using fastwriter.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace fastwriter.Controllers;

[ApiController]
[Route("api/scores")]
[Authorize]
[EnableRateLimiting("scores")]
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
    /// İstemciden gelen yazılan metni, sunucudaki test oturumunun referans metniyle karşılaştırır.
    /// Süre istemciden değil, oturumun sunucudaki başlangıç zamanından hesaplanır.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save([FromBody] ScoreSubmissionDto dto)
    {
        if (dto.SessionId == Guid.Empty)
        {
            return BadRequest(new { message = "Geçersiz test oturumu." });
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

        var userId = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Skor kaydetmek için giriş yapmalısınız." });
        }

        var session = await _db.TestSessions
            .AsNoTracking()
            .Include(s => s.TypingText)
            .FirstOrDefaultAsync(s => s.Id == dto.SessionId);

        if (session is null || session.UserId != userId)
        {
            return NotFound(new { message = "Geçerli bir test oturumu bulunamadı. Testi yeniden başlatın." });
        }

        if (session.CompletedAtUtc is not null)
        {
            return Conflict(new { message = "Bu test oturumu zaten kullanıldı. Testi yeniden başlatın." });
        }

        var now = DateTime.UtcNow;
        var elapsed = now - session.StartedAtUtc;

        if (elapsed > TestSessionRules.Lifetime)
        {
            return BadRequest(new { message = "Test oturumunun süresi doldu. Testi yeniden başlatın." });
        }

        if (elapsed.TotalSeconds < TestSessionRules.MinimumDurationSeconds)
        {
            return BadRequest(new { message = "Test süresi çok kısa. En az 3 saniye yazmalısınız." });
        }

        if (elapsed.TotalSeconds > ExamContent.DurationSeconds + TestSessionRules.MaximumGraceSeconds)
        {
            return BadRequest(new { message = "Test süresi aşıldı. Testi yeniden başlatın." });
        }

        var text = session.TypingText;

        if (text is null || !text.IsActive)
        {
            return NotFound(new { message = "Sınav metni bulunamadı." });
        }

        // 180 saniyeye ulaşan denemelerde ağ gecikmesi için küçük bir tolerans tanınır.
        var durationSeconds = elapsed.TotalSeconds >= ExamContent.DurationSeconds - TestSessionRules.FullExamToleranceSeconds
            ? ExamContent.DurationSeconds
            : (int)Math.Floor(elapsed.TotalSeconds);

        var target = ExamContent.Build(text.Content);
        var metrics = TypingMetrics.Calculate(target, typedText, durationSeconds);

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
            DurationSeconds = durationSeconds,
            CreatedAt = now
        };

        await using var transaction = await _db.Database.BeginTransactionAsync();

        // Oturumu koşullu olarak tek kullanımlık işaretle; aynı oturumla ikinci skor kaydını engelle.
        var markedUsed = await _db.TestSessions
            .Where(s => s.Id == session.Id
                && s.UserId == userId
                && s.CompletedAtUtc == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.CompletedAtUtc, (DateTime?)now));

        if (markedUsed == 0)
        {
            await transaction.RollbackAsync();
            return Conflict(new { message = "Bu test oturumu zaten kullanıldı. Testi yeniden başlatın." });
        }

        _db.Scores.Add(score);
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

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
