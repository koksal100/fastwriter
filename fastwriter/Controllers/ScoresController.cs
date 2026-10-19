using fastwriter.Data;
using fastwriter.Models;
using fastwriter.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fastwriter.Controllers;

[Authorize]
public class ScoresController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ScoresController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var scores = await _db.Scores
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .Take(100)
            .ToListAsync();

        var allScores = await _db.Scores
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Select(s => new { s.NetWpm, s.Accuracy })
            .ToListAsync();

        var model = new ScoreHistoryViewModel
        {
            Scores = scores,
            BestNetWpm = allScores.Count > 0 ? allScores.Max(s => s.NetWpm) : 0,
            AverageNetWpm = allScores.Count > 0 ? (int)Math.Round(allScores.Average(s => s.NetWpm)) : 0,
            AverageAccuracy = allScores.Count > 0 ? Math.Round(allScores.Average(s => s.Accuracy), 2) : 0,
            TotalTests = allScores.Count
        };

        return View(model);
    }
}
