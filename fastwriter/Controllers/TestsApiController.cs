using System.Security.Claims;
using fastwriter.Data;
using fastwriter.Models;
using fastwriter.Services;
using fastwriter.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace fastwriter.Controllers;

[ApiController]
[Route("api/tests")]
[Authorize]
public class TestsApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public TestsApiController(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// İlk tuşa basıldığında çağrılır. Sunucu saatini başlatır ve tek kullanımlık oturum döndürür.
    /// </summary>
    [HttpPost("start")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("test-start")]
    public async Task<IActionResult> Start([FromBody] StartTestDto? dto)
    {
        if (dto is null || dto.TextId <= 0)
        {
            return BadRequest(new { message = "Geçersiz sınav metni." });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Test oturumu başlatmak için giriş yapmalısınız." });
        }

        var text = await _db.TypingTexts
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == dto.TextId && t.IsActive);

        if (text is null)
        {
            return BadRequest(new { message = "Sınav metni bulunamadı. Sayfayı yenileyin." });
        }

        var now = DateTime.UtcNow;

        // Kullanıcının süresi dolmuş, kullanılmamış oturumlarını temizle.
        await _db.TestSessions
            .Where(s => s.UserId == userId
                && s.CompletedAtUtc == null
                && s.StartedAtUtc < now - TestSessionRules.Lifetime)
            .ExecuteDeleteAsync();

        var session = new TestSession
        {
            UserId = userId,
            TypingTextId = text.Id,
            StartedAtUtc = now
        };

        _db.TestSessions.Add(session);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            sessionId = session.Id,
            textId = text.Id,
            startedAt = session.StartedAtUtc
        });
    }
}
