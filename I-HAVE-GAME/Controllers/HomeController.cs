using I_HAVE_GAME.Models;
using I_HAVE_GAME.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using I_HAVE_GAME.ViewModels;

namespace I_HAVE_GAME.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var allGames = await _dbContext.Games
                .OrderByDescending(game => game.Rating)
                .ThenBy(game => game.Title)
                .AsNoTracking()
                .ToListAsync();

            var model = new HomePageViewModel
            {
                SpotlightGame = allGames.FirstOrDefault(),
                TopGames = allGames.Take(6).ToList(),
                NewGames = allGames.OrderByDescending(game => game.ReleaseDate).Take(6).ToList(),
                GenreShelves = new Dictionary<string, IReadOnlyList<Game>>
                {
                    ["เกมแอ็กชันที่ห้ามพลาด"] = allGames.Where(game => (game.Genres ?? string.Empty).Contains("Action")).Take(6).ToList(),
                    ["เกมเล่นเพลิน ราคาสบายกระเป๋า"] = allGames.Where(game => game.Price is null || game.Price <= 25).OrderByDescending(game => game.Rating).Take(6).ToList()
                }
            };

            if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            {
                var statuses = await _dbContext.GameLibraryItems.Where(item => item.UserId == userId)
                    .GroupBy(item => item.Status).Select(group => new { Status = group.Key, Count = group.Count() }).ToListAsync();
                model = new HomePageViewModel
                {
                    SpotlightGame = model.SpotlightGame, TopGames = model.TopGames, NewGames = model.NewGames, GenreShelves = model.GenreShelves,
                    WishlistCount = statuses.FirstOrDefault(item => item.Status == "Wishlist")?.Count ?? 0,
                    BacklogCount = statuses.FirstOrDefault(item => item.Status == "Backlog")?.Count ?? 0,
                    PlayedCount = statuses.FirstOrDefault(item => item.Status == "Played")?.Count ?? 0
                };
            }
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Surprise()
        {
            var count = await _dbContext.Games.CountAsync();
            if (count == 0) return RedirectToAction("Index", "Catalog", new { notice = "ยังไม่มีเกมในคลัง" });
            var game = await _dbContext.Games.OrderBy(game => game.Id).Skip(Random.Shared.Next(count)).FirstAsync();
            return RedirectToAction("Details", "Catalog", new { slug = game.Slug });
        }

        [AllowAnonymous]
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
}
