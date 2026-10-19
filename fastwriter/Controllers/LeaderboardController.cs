using System.Security.Claims;
using fastwriter.Data;
using fastwriter.Services;
using fastwriter.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fastwriter.Controllers;

public class LeaderboardController : Controller
{
    private const int MaxDisplayedEntries = 100;

    private readonly ApplicationDbContext _db;

    public LeaderboardController(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Tüm kullanıcıların en iyi 3 dakikalık denemelerini sıralar.
    /// Her kullanıcı yalnızca bir kez listelenir; eşit WPM'de doğruluk öne geçer.
    /// </summary>
    public async Task<IActionResult> Index(string period = "all")
    {
        period = (period ?? "all").ToLowerInvariant();

        if (period is not ("all" or "week" or "today"))
        {
            period = "all";
        }

        var now = DateTime.UtcNow;

        var query = _db.Scores
            .AsNoTracking()
            .Where(s => s.DurationSeconds >= ExamContent.DurationSeconds);

        if (period == "week")
        {
            query = query.Where(s => s.CreatedAt >= now.AddDays(-7));
        }
        else if (period == "today")
        {
            // Saat dilimi karışıklığını önlemek için son 24 saat kullanılır.
            query = query.Where(s => s.CreatedAt >= now.AddHours(-24));
        }

        // Kullanıcı başına en iyi skor.
        var bestScores = await query
            .GroupBy(s => s.UserId)
            .Select(g => g
                .OrderByDescending(s => s.NetWpm)
                .ThenByDescending(s => s.Accuracy)
                .ThenByDescending(s => s.CreatedAt)
                .First())
            .ToListAsync();

        var userIds = bestScores.Select(s => s.UserId).ToList();

        var displayNames = await _db.Users
            .AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.DisplayName })
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName);

        var attemptCounts = await query
            .Where(s => userIds.Contains(s.UserId))
            .GroupBy(s => s.UserId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.UserId, x => x.Count);

        var allEntries = bestScores
            .Select(score => new LeaderboardEntryViewModel
            {
                UserId = score.UserId,
                DisplayName = displayNames.TryGetValue(score.UserId, out var name) && !string.IsNullOrWhiteSpace(name)
                    ? name
                    : "Anonim",
                NetWpm = score.NetWpm,
                RawWpm = score.RawWpm,
                Accuracy = score.Accuracy,
                CorrectWords = score.CorrectWords,
                DurationSeconds = score.DurationSeconds,
                AttemptCount = attemptCounts.TryGetValue(score.UserId, out var count) ? count : 0,
                AchievedAt = score.CreatedAt
            })
            .OrderByDescending(e => e.NetWpm)
            .ThenByDescending(e => e.Accuracy)
            .ThenByDescending(e => e.AchievedAt)
            .ToList();

        for (var i = 0; i < allEntries.Count; i++)
        {
            allEntries[i].Rank = i + 1;
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var model = new LeaderboardViewModel
        {
            Period = period,
            CurrentUserId = currentUserId,
            CurrentUserEntry = currentUserId is null
                ? null
                : allEntries.FirstOrDefault(e => e.UserId == currentUserId),
            Entries = allEntries.Take(MaxDisplayedEntries).ToList()
        };

        return View(model);
    }
}
