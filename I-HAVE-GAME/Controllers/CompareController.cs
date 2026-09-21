using I_HAVE_GAME.Services;
using I_HAVE_GAME.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace I_HAVE_GAME.Controllers
{
    /// <summary>
    /// Controller for comparing games
    /// Allows users to select 2-3 games and compare them side-by-side
    /// </summary>
    // ฟีเจอร์นี้ใช้ RAWG ซึ่งโปรเจกต์เปลี่ยนมาใช้คลังเกมภายในแล้ว
    [NonController]
    public class CompareController : Controller
    {
        private readonly IRawgService _rawgService;
        private readonly ILogger<CompareController> _logger;

        public CompareController(IRawgService rawgService, ILogger<CompareController> logger)
        {
            _rawgService = rawgService;
            _logger = logger;
        }

        /// <summary>
        /// Main compare page - search for games
        /// </summary>
        public async Task<IActionResult> Index(string? search = null, CancellationToken cancellationToken = default)
        {
            var model = new CompareGamesViewModel();

            if (!_rawgService.IsConfigured())
            {
                model.Message = "The compare feature is currently unavailable. Please contact support.";
                return View(model);
            }

            // If search term provided, fetch games
            if (!string.IsNullOrWhiteSpace(search))
            {
                try
                {
                    var response = await _rawgService.SearchGamesAsync(
                        genre: null,
                        platforms: null,
                        search: search,
                        ordering: "-rating",
                        pageSize: 20,
                        page: 1,
                        cancellationToken: cancellationToken);

                    if (response?.Results != null && response.Results.Count > 0)
                    {
                        model.SearchResults = response.Results
                            .Take(20)
                            .Select(g => new GameSearchResultViewModel
                            {
                                GameId = g.Id,
                                Name = g.Name,
                                BackgroundImage = g.BackgroundImage,
                                Rating = g.Rating,
                                RatingsCount = g.RatingsCount
                            })
                            .ToList();

                        if (model.SearchResults.Count == 0)
                        {
                            model.Message = "No games found matching your search.";
                        }
                    }
                    else
                    {
                        model.Message = "No games found matching your search.";
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Failed to search games from RAWG API");
                    model.Message = "Unable to search games. Please try again later.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error searching games");
                    model.Message = "An unexpected error occurred while searching.";
                }
            }

            return View(model);
        }

        /// <summary>
        /// Compare action - displays comparison of selected games
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Compare(CompareGamesRequest request, CancellationToken cancellationToken)
        {
            var model = new CompareGamesViewModel();

            if (request?.SelectedGameIds == null || request.SelectedGameIds.Count == 0)
            {
                model.Message = "Please select at least 2 games to compare.";
                return View("Index", model);
            }

            if (request.SelectedGameIds.Count > 3)
            {
                model.Message = "You can compare a maximum of 3 games.";
                return View("Index", model);
            }

            if (request.SelectedGameIds.Count < 2)
            {
                model.Message = "Please select at least 2 games to compare.";
                return View("Index", model);
            }

            try
            {
                // Remove duplicates and fetch games
                var uniqueIds = request.SelectedGameIds.Distinct().ToList();
                model.Games = new List<CompareGameViewModel>();

                foreach (var gameId in uniqueIds)
                {
                    try
                    {
                        var game = await _rawgService.GetGameAsync(gameId, cancellationToken: cancellationToken);
                        if (game != null)
                        {
                            model.Games.Add(MapToCompareGameViewModel(game));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to fetch game {GameId}", gameId);
                    }
                }

                if (model.Games.Count == 0)
                {
                    model.Message = "Could not load the selected games. Please try again.";
                    return View("Index", model);
                }

                if (model.Games.Count < 2)
                {
                    model.Message = "Not enough games available for comparison. Please select different games.";
                    return View("Index", model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in compare action");
                model.Message = "An unexpected error occurred. Please try again.";
                return View("Index", model);
            }

            return View("CompareResult", model);
        }

        /// <summary>
        /// Map RAWG game data to compare view model
        /// </summary>
        private CompareGameViewModel MapToCompareGameViewModel(Models.Rawg.RawgGame game)
        {
            return new CompareGameViewModel
            {
                GameId = game.Id,
                Name = game.Name,
                BackgroundImage = game.BackgroundImage,
                Rating = game.Rating,
                RatingsCount = game.RatingsCount,
                Released = game.Released,
                Genres = game.Genres?.Select(g => g.Name).ToList() ?? new List<string>(),
                Platforms = game.Platforms?
                    .Select(p => p.Platform?.Name)
                    .Where(p => p != null)
                    .Cast<string>()
                    .Distinct()
                    .ToList() ?? new List<string>(),
                Description = game.DescriptionRaw ?? string.Empty
            };
        }
    }
}
