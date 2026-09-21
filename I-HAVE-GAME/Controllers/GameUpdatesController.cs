using I_HAVE_GAME.Data;
using I_HAVE_GAME.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace I_HAVE_GAME.Controllers;

public class GameUpdatesController : Controller
{
    private readonly AppDbContext _dbContext;
    public GameUpdatesController(AppDbContext dbContext) => _dbContext = dbContext;

    [AllowAnonymous]
    public async Task<IActionResult> Index(string slug)
    {
        var game = await _dbContext.Games.AsNoTracking().FirstOrDefaultAsync(item => item.Slug == slug);
        if (game is null) return NotFound();
        var updates = await _dbContext.GameUpdates.Where(item => item.GameId == game.Id).OrderByDescending(item => item.PublishedAt).AsNoTracking().ToListAsync();
        return View(new GameUpdateListViewModel { Game = game, Updates = updates });
    }

    [AllowAnonymous]
    public async Task<IActionResult> Calendar(int? year, int? month)
    {
        var now = DateTime.Today;
        var calendarMonth = new DateTime(year ?? now.Year, month ?? now.Month, 1);
        var start = calendarMonth;
        var end = calendarMonth.AddMonths(1);
        var games = await _dbContext.Games.AsNoTracking().ToListAsync();
        var gameById = games.ToDictionary(game => game.Id);
        var events = games.Where(game => game.ReleaseDate >= start && game.ReleaseDate < end)
            .Select(game => new GameCalendarEventViewModel { Date = game.ReleaseDate!.Value, Kind = "release", Title = "เกมออกใหม่", GameTitle = game.Title, GameSlug = game.Slug ?? string.Empty }).ToList();
        var updates = await _dbContext.GameUpdates.AsNoTracking().Where(update => update.PublishedAt >= start && update.PublishedAt < end).ToListAsync();
        events.AddRange(updates.Where(update => gameById.ContainsKey(update.GameId) && update.Title != "วางจำหน่ายแล้ว")
            .Select(update => new GameCalendarEventViewModel { Date = update.PublishedAt, Kind = "update", Title = update.Title, GameTitle = gameById[update.GameId].Title, GameSlug = gameById[update.GameId].Slug ?? string.Empty }));
        return View(new GameCalendarViewModel { Month = calendarMonth, Events = events.OrderBy(item => item.Date).ToList() });
    }
}
