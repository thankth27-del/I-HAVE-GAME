using I_HAVE_GAME.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using I_HAVE_GAME.Models;
using System.Security.Claims;

namespace I_HAVE_GAME.Controllers;

public class CatalogController : Controller
{
    private readonly AppDbContext _dbContext;

    public CatalogController(AppDbContext dbContext) => _dbContext = dbContext;

    [AllowAnonymous]
    public async Task<IActionResult> Index(string? query, string? genre, string? platform, string? sort, string? notice)
    {
        var games = _dbContext.Games.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            games = games.Where(game => game.Title.Contains(term) ||
                                        (game.Genres ?? string.Empty).Contains(term) ||
                                        (game.Tags ?? string.Empty).Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(genre))
            games = games.Where(game => (game.Genres ?? string.Empty).Contains(genre));
        if (!string.IsNullOrWhiteSpace(platform))
            games = games.Where(game => (game.Platforms ?? string.Empty).Contains(platform));

        ViewBag.Query = query;
        ViewBag.SelectedGenre = genre;
        ViewBag.SelectedPlatform = platform;
        ViewBag.Sort = sort;
        ViewBag.Notice = notice;
        var gameList = await games.ToListAsync();
        ViewBag.Genres = gameList.SelectMany(game => (game.Genres ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(value => value.Trim()).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct().OrderBy(value => value).ToList();
        ViewBag.Platforms = gameList.SelectMany(game => (game.Platforms ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(value => value.Trim()).Where(value => !string.IsNullOrWhiteSpace(value)).Distinct().OrderBy(value => value).ToList();

        gameList = sort switch
        {
            "title" => gameList.OrderBy(game => game.Title).ToList(),
            "newest" => gameList.OrderByDescending(game => game.ReleaseDate).ThenBy(game => game.Title).ToList(),
            "price-low" => gameList.OrderBy(game => game.Price ?? decimal.MaxValue).ToList(),
            "price-high" => gameList.OrderByDescending(game => game.Price ?? decimal.MinValue).ToList(),
            _ => gameList.OrderByDescending(game => game.Rating).ThenBy(game => game.Title).ToList()
        };
        return View(gameList);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return NotFound();
        var game = await _dbContext.Games.AsNoTracking().FirstOrDefaultAsync(game => game.Slug == slug);
        return game is null ? NotFound() : View(game);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToLibrary(int gameId, string status = "Wishlist")
    {
        if (status is not ("Wishlist" or "Backlog" or "Played")) status = "Wishlist";
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId)) return Challenge();

        var game = await _dbContext.Games.FindAsync(gameId);
        if (game is null) return NotFound();

        var exists = await _dbContext.GameLibraryItems.AnyAsync(item => item.UserId == userId && item.GameId == gameId);
        if (!exists)
        {
            _dbContext.GameLibraryItems.Add(new GameLibraryItem
            {
                UserId = userId,
                GameId = game.Id,
                GameSlug = game.Slug,
                GameName = game.Title,
                GameImageUrl = game.ImageUrl,
                Genre = game.Genres,
                Status = status,
                AddedAt = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();
            TempData["CatalogMessage"] = $"เพิ่ม {game.Title} ไปยัง {status} แล้ว";
        }
        else
        {
            TempData["CatalogMessage"] = "เกมนี้อยู่ในคลังของคุณแล้ว";
        }
        return RedirectToAction(nameof(Details), new { slug = game.Slug });
    }
}
