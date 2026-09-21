using I_HAVE_GAME.Data;
using I_HAVE_GAME.Models;
using I_HAVE_GAME.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace I_HAVE_GAME.Controllers;

[Authorize]
public class SearchController : Controller
{
    private readonly AppDbContext _dbContext;
    public SearchController(AppDbContext dbContext) => _dbContext = dbContext;

    [HttpGet("Matcher")]
    public IActionResult Index() => View(new GameSearchViewModel());

    [HttpPost("Matcher")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Choose(GameSearchRequest request)
    {
        request.SelectedGenre = Request.Form[nameof(request.SelectedGenre)].LastOrDefault();
        request.SelectedDevice = Request.Form[nameof(request.SelectedDevice)].LastOrDefault();
        request.SelectedPlayMode = Request.Form[nameof(request.SelectedPlayMode)].LastOrDefault();
        request.SelectedBudget = Request.Form[nameof(request.SelectedBudget)].LastOrDefault();
        request.SelectedEra = Request.Form[nameof(request.SelectedEra)].LastOrDefault();
        var step = Math.Clamp(request.CurrentStep, 1, 5);
        if (request.Action == "previous") step--;
        else if (request.Action == "next")
        {
            if (string.IsNullOrWhiteSpace(ChoiceForStep(request, step)))
            {
                var invalidModel = ToViewModel(request, step);
                invalidModel.ErrorMessage = "กรุณาเลือกคำตอบก่อนดำเนินการต่อ";
                return View("Index", invalidModel);
            }
            step++;
        }

        if (request.Action != "complete") return View("Index", ToViewModel(request, Math.Clamp(step, 1, 5)));

        var model = ToViewModel(request, 5);
        var games = await _dbContext.Games.AsNoTracking().ToListAsync();
        var ranked = games
            .Select(game => new { Game = game, Score = Score(game, request) })
            .Where(item => string.IsNullOrWhiteSpace(request.SelectedGenre) || request.SelectedGenre == "any" || Contains(item.Game.Genres, request.SelectedGenre))
            .OrderByDescending(item => item.Score)
            .ThenByDescending(item => item.Game.Rating ?? 0)
            .Take(20)
            .Select(item => Map(item.Game))
            .ToList();

        model.Results = ranked;
        model.TotalResults = ranked.Count;
        if (ranked.Count == 0) model.WarningMessage = "ยังไม่มีเกมในคลังที่ตรงกับคำตอบนี้ ลองเลือกแนวเกมอื่น หรือให้ผู้ดูแลเพิ่มเกมใหม่";
        await SaveHistory(request, ranked.Count);
        return View("Index", model);
    }

    private static GameSearchViewModel ToViewModel(GameSearchRequest request, int step) => new()
    {
        CurrentStep = step, SelectedGenre = request.SelectedGenre, SelectedDevice = request.SelectedDevice,
        SelectedPlayMode = request.SelectedPlayMode, SelectedBudget = request.SelectedBudget, SelectedEra = request.SelectedEra
    };

    private static string? ChoiceForStep(GameSearchRequest request, int step) => step switch
    {
        1 => request.SelectedGenre, 2 => request.SelectedDevice, 3 => request.SelectedPlayMode,
        4 => request.SelectedBudget, 5 => request.SelectedEra, _ => null
    };

    private static int Score(Game game, GameSearchRequest request)
    {
        var score = 0;
        if (Contains(game.Genres, request.SelectedGenre)) score += 5;
        if (Contains(game.Platforms, request.SelectedDevice)) score += 3;
        if (request.SelectedPlayMode == "solo" && !Contains(game.Tags, "multi") && !Contains(game.Tags, "co-op")) score += 2;
        if (request.SelectedPlayMode == "multiplayer" && (Contains(game.Tags, "multi") || Contains(game.Tags, "co-op") || Contains(game.Genres, "multiplayer"))) score += 2;
        if (request.SelectedBudget == "free" && (!game.Price.HasValue || game.Price == 0)) score += 2;
        if (request.SelectedBudget == "under-20" && game.Price is <= 20) score += 2;
        if (request.SelectedBudget == "premium" && game.Price is > 20) score += 2;
        if (request.SelectedEra == "2020+" && game.ReleaseDate?.Year >= 2020) score++;
        if (request.SelectedEra == "before-2020" && game.ReleaseDate?.Year < 2020) score++;
        return score;
    }

    private static bool Contains(string? source, string? value) => !string.IsNullOrWhiteSpace(value) && value != "any" && !string.IsNullOrWhiteSpace(source) && source.Contains(value, StringComparison.OrdinalIgnoreCase);
    private static GameResultViewModel Map(Game game) => new() { Id = game.Id, Slug = game.Slug ?? string.Empty, Name = game.Title, ReleasedDate = game.ReleaseDate?.ToString("d MMM yyyy"), Rating = (decimal)(game.Rating ?? 0), Genres = Split(game.Genres), Platforms = Split(game.Platforms), Description = game.Description };
    private static List<string> Split(string? text) => string.IsNullOrWhiteSpace(text) ? [] : text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();

    private async Task SaveHistory(GameSearchRequest request, int count)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)) return;
        _dbContext.SearchHistories.Add(new SearchHistory { UserId = userId, Genre = request.SelectedGenre, Device = request.SelectedDevice, PlayMode = request.SelectedPlayMode, Budget = request.SelectedBudget, Era = request.SelectedEra, ResultCount = count, SearchedAt = DateTime.UtcNow });
        await _dbContext.SaveChangesAsync();
    }
}
