using System.Diagnostics;
using fastwriter.Data;
using fastwriter.Models;
using fastwriter.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fastwriter.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Rastgele bir sınav metni döndürür. Metin, 3 dakikalık sınav için yeterli
    /// uzunluğa getirilir ve skor doğrulamasında da aynı içerik kullanılır.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTestText()
    {
        var texts = await _db.TypingTexts
            .AsNoTracking()
            .Where(t => t.IsActive)
            .ToListAsync();

        if (texts.Count == 0)
        {
            return NotFound(new { message = "Sınav metni bulunamadı." });
        }

        var text = texts[Random.Shared.Next(texts.Count)];

        return Json(new
        {
            id = text.Id,
            title = text.Title,
            category = text.Category,
            durationSeconds = ExamContent.DurationSeconds,
            text = ExamContent.Build(text.Content)
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
