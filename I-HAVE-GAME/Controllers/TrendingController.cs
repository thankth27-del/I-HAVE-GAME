using I_HAVE_GAME.Services;
using I_HAVE_GAME.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace I_HAVE_GAME.Controllers
{
    /// <summary>
    /// Controller for trending/popular games
    /// Shows weekly trending games from RAWG API
    /// </summary>
    [Authorize]
    public class TrendingController : Controller
    {
        private readonly IRawgService _rawgService;
        private readonly ILogger<TrendingController> _logger;

        public TrendingController(IRawgService rawgService, ILogger<TrendingController> logger)
        {
            _rawgService = rawgService;
            _logger = logger;
        }

        /// <summary>
        /// Display trending games
        /// Gets popular games ordered by rating from the current week
        /// </summary>
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = new TrendingGamesViewModel();

            try
            {
                // Calculate date range for the current week (approximately)
                // RAWG date format: "2024-01-01,2024-01-07"
                var today = DateTime.UtcNow.Date;
                var weekStart = today.AddDays(-(int)today.DayOfWeek);
                var weekEnd = weekStart.AddDays(6);
                var dateRange = $"{weekStart:yyyy-MM-dd},{weekEnd:yyyy-MM-dd}";

                // Fetch trending games ordered by rating and popularity
                var response = await _rawgService.SearchGamesAsync(
                    genre: null,
                    platforms: null,
                    released: dateRange,
                    search: null,
                    ordering: "-rating",  // Order by highest rating first
                    pageSize: 12,
                    page: 1,
                    cancellationToken: cancellationToken);

                if (response?.Results == null || response.Results.Count == 0)
                {
                    model.Message = "No trending games found for this week. Try again later!";
                    return View(model);
                }

                // Map RAWG games to view model
                var seenGameIds = new HashSet<int>();
                foreach (var game in response.Results)
                {
                    // Exclude duplicates
                    if (seenGameIds.Contains(game.Id))
                        continue;

                    seenGameIds.Add(game.Id);

                    var trendingGame = MapToTrendingGame(game);
                    model.TrendingGames.Add(trendingGame);
                }

                if (model.TrendingGames.Count == 0)
                {
                    model.Message = "No trending games available at this moment.";
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch trending games from RAWG API");
                model.Message = "Unable to load trending games. Please try again later.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error loading trending games");
                model.Message = "An unexpected error occurred while loading trending games.";
            }

            return View(model);
        }

        /// <summary>
        /// Map RAWG game data to trending game view model
        /// </summary>
        private TrendingGameViewModel MapToTrendingGame(Models.Rawg.RawgGame game)
        {
            return new TrendingGameViewModel
            {
                GameId = game.Id,
                Name = game.Name,
                BackgroundImage = game.BackgroundImage,
                Rating = game.Rating,
                RatingsCount = game.RatingsCount,
                Genres = game.Genres?.Select(g => g.Name).ToList() ?? new List<string>(),
                Platforms = game.Platforms?.Select(p => p.Platform?.Name).Where(p => p != null)
                    .Cast<string>().Distinct().ToList() ?? new List<string>(),
                Released = game.Released
            };
        }
    }
}
