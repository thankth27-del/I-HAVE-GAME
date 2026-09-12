using I_HAVE_GAME.Services;
using I_HAVE_GAME.ViewModels;
using I_HAVE_GAME.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace I_HAVE_GAME.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly IRawgService _rawgService;
        private readonly ILogger<SearchController> _logger;

        public SearchController(IRawgService rawgService, ILogger<SearchController> logger)
        {
            _rawgService = rawgService;
            _logger = logger;
        }

        /// <summary>
        /// Main search page - shows the wizard
        /// </summary>
        public async Task<IActionResult> Index()
        {
            if (!_rawgService.IsConfigured())
            {
                return View("Error", new ErrorViewModel 
                { 
                    Message = "The game search feature is currently unavailable. Please contact support.",
                    Details = "RAWG API is not configured."
                });
            }

            var model = new GameSearchViewModel { CurrentStep = 1 };
            return View(model);
        }

        /// <summary>
        /// Step 1: Genre selection
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Step1(GameSearchRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.SelectedGenre))
            {
                ViewData["Error"] = "Please select a genre to continue.";
                var model = new GameSearchViewModel { CurrentStep = 1 };
                return View("Index", model);
            }

            var viewModel = new GameSearchViewModel
            {
                CurrentStep = 2,
                SelectedGenre = request.SelectedGenre
            };

            return View("Index", viewModel);
        }

        /// <summary>
        /// Step 2: Device/Platform selection
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Step2(GameSearchRequest request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.SelectedDevice))
            {
                ViewData["Error"] = "Please select a device to continue.";
                var model = new GameSearchViewModel
                {
                    CurrentStep = 2,
                    SelectedGenre = request.SelectedGenre
                };
                return View("Index", model);
            }

            var viewModel = new GameSearchViewModel
            {
                CurrentStep = 3,
                SelectedGenre = request.SelectedGenre,
                SelectedDevice = request.SelectedDevice
            };

            return View("Index", viewModel);
        }

        /// <summary>
        /// Step 3: Play Mode selection
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Step3(GameSearchRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Invalid input. Please try again.";
                var model = new GameSearchViewModel
                {
                    CurrentStep = 3,
                    SelectedGenre = request.SelectedGenre,
                    SelectedDevice = request.SelectedDevice
                };
                return View("Index", model);
            }

            var viewModel = new GameSearchViewModel
            {
                CurrentStep = 4,
                SelectedGenre = request.SelectedGenre,
                SelectedDevice = request.SelectedDevice,
                SelectedPlayMode = request.SelectedPlayMode
            };

            return View("Index", viewModel);
        }

        /// <summary>
        /// Step 4: Budget selection
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Step4(GameSearchRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Invalid input. Please try again.";
                var model = new GameSearchViewModel
                {
                    CurrentStep = 4,
                    SelectedGenre = request.SelectedGenre,
                    SelectedDevice = request.SelectedDevice,
                    SelectedPlayMode = request.SelectedPlayMode
                };
                return View("Index", model);
            }

            var viewModel = new GameSearchViewModel
            {
                CurrentStep = 5,
                SelectedGenre = request.SelectedGenre,
                SelectedDevice = request.SelectedDevice,
                SelectedPlayMode = request.SelectedPlayMode,
                SelectedBudget = request.SelectedBudget
            };

            return View("Index", viewModel);
        }

        /// <summary>
        /// Step 5: Era/Release Date selection
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Step5(GameSearchRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Invalid input. Please try again.";
                var model = new GameSearchViewModel
                {
                    CurrentStep = 5,
                    SelectedGenre = request.SelectedGenre,
                    SelectedDevice = request.SelectedDevice,
                    SelectedPlayMode = request.SelectedPlayMode,
                    SelectedBudget = request.SelectedBudget
                };
                return View("Index", model);
            }

            var viewModel = new GameSearchViewModel
            {
                CurrentStep = 5,
                SelectedGenre = request.SelectedGenre,
                SelectedDevice = request.SelectedDevice,
                SelectedPlayMode = request.SelectedPlayMode,
                SelectedBudget = request.SelectedBudget,
                SelectedEra = request.SelectedEra
            };

            return View("Index", viewModel);
        }

        /// <summary>
        /// Back button - go to previous step
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GoBack(GameSearchRequest request)
        {
            if (request.CurrentStep <= 1)
            {
                return RedirectToAction("Index");
            }

            var previousStep = request.CurrentStep - 1;
            var viewModel = new GameSearchViewModel
            {
                CurrentStep = previousStep,
                SelectedGenre = request.SelectedGenre,
                SelectedDevice = request.SelectedDevice,
                SelectedPlayMode = request.SelectedPlayMode,
                SelectedBudget = request.SelectedBudget,
                SelectedEra = request.SelectedEra
            };

            return View("Index", viewModel);
        }

        /// <summary>
        /// Search for games based on wizard selections
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(GameSearchRequest request, CancellationToken cancellationToken)
        {
            // Validate selections
            if (string.IsNullOrWhiteSpace(request.SelectedGenre) ||
                string.IsNullOrWhiteSpace(request.SelectedDevice) ||
                string.IsNullOrWhiteSpace(request.SelectedEra))
            {
                var errorModel = new GameSearchViewModel
                {
                    CurrentStep = 5,
                    SelectedGenre = request.SelectedGenre,
                    SelectedDevice = request.SelectedDevice,
                    SelectedPlayMode = request.SelectedPlayMode,
                    SelectedBudget = request.SelectedBudget,
                    SelectedEra = request.SelectedEra,
                    ErrorMessage = "Please complete all steps before searching."
                };
                return View("Index", errorModel);
            }

            var viewModel = new GameSearchViewModel
            {
                CurrentStep = 5,
                SelectedGenre = request.SelectedGenre,
                SelectedDevice = request.SelectedDevice,
                SelectedPlayMode = request.SelectedPlayMode,
                SelectedBudget = request.SelectedBudget,
                SelectedEra = request.SelectedEra,
                CurrentPage = Math.Max(request.CurrentPage, 0),
                PageSize = request.PageSize,
                IsLoading = true
            };

            try
            {
                // Build RAWG query parameters from selections
                var genre = request.SelectedGenre == "" ? null : request.SelectedGenre;
                var platforms = request.SelectedDevice == "" ? null : request.SelectedDevice;
                var dateRange = request.SelectedEra == "" ? null : request.SelectedEra;

                // Call RAWG service
                // RAWG uses 1-based page numbers
                var page = viewModel.CurrentPage + 1;
                var result = await _rawgService.SearchGamesAsync(
                    genre: genre,
                    platforms: platforms,
                    released: dateRange,
                    search: null,
                    ordering: "-rating",
                    pageSize: viewModel.PageSize,
                    page: page,
                    cancellationToken: cancellationToken
                );

                if (result == null)
                {
                    viewModel.ErrorMessage = "Unable to search for games. The RAWG API is currently unavailable. Please try again later.";
                    viewModel.IsLoading = false;
                    return View("Index", viewModel);
                }

                // Map RAWG results to view models
                viewModel.Results = MapRawgGamesToResultViewModels(result.Results);
                viewModel.TotalResults = result.Count;
                viewModel.IsLoading = false;

                if (viewModel.Results.Count == 0)
                {
                    viewModel.WarningMessage = "No games found matching your criteria. Try adjusting your selections.";
                }

                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during game search");
                viewModel.ErrorMessage = "An unexpected error occurred during the search. Please try again.";
                viewModel.IsLoading = false;
                return View("Index", viewModel);
            }
        }

        /// <summary>
        /// Map RAWG game models to result view models
        /// </summary>
        private List<GameResultViewModel> MapRawgGamesToResultViewModels(List<Models.Rawg.RawgGame> rawgGames)
        {
            return rawgGames.Select(g => new GameResultViewModel
            {
                Id = g.Id,
                Slug = g.Slug,
                Name = g.Name,
                BackgroundImage = g.BackgroundImage,
                ReleasedDate = g.Released,
                Rating = g.Rating,
                RatingsCount = g.RatingsCount,
                Genres = g.Genres?.Select(gen => gen.Name).ToList() ?? new List<string>(),
                Platforms = g.Platforms?.Where(p => p.Platform != null).Select(p => p.Platform!.Name).ToList() ?? new List<string>(),
                Stores = g.Stores?.Where(s => s.Store != null).Select(s => s.Store!.Name).ToList() ?? new List<string>(),
                Description = g.DescriptionRaw
            }).ToList();
        }
    }
}
