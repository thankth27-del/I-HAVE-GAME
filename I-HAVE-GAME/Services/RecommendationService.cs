using I_HAVE_GAME.Data;
using I_HAVE_GAME.Models;
using I_HAVE_GAME.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace I_HAVE_GAME.Services
{
    /// <summary>
    /// Service for generating game recommendations based on user's library preferences
    /// </summary>
    public class RecommendationService
    {
        private readonly AppDbContext _dbContext;
        private readonly IRawgService _rawgService;
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(
            AppDbContext dbContext,
            IRawgService rawgService,
            ILogger<RecommendationService> logger)
        {
            _dbContext = dbContext;
            _rawgService = rawgService;
            _logger = logger;
        }

        /// <summary>
        /// Get recommended games for a user based on their library preferences
        /// </summary>
        public async Task<RecommendationsViewModel> GetRecommendationsAsync(
            int userId,
            int pageSize = 12,
            CancellationToken cancellationToken = default)
        {
            var viewModel = new RecommendationsViewModel();

            try
            {
                // Get user's played games
                var playedGames = await _dbContext.GameLibraryItems
                    .Where(g => g.UserId == userId && g.Status == "Played")
                    .OrderByDescending(g => g.Rating)
                    .ToListAsync(cancellationToken);

                if (playedGames.Count == 0)
                {
                    viewModel.Message = "You haven't marked any games as played yet. Add games to your library and rate them to get personalized recommendations!";
                    _logger.LogInformation("User {UserId} has no played games", userId);
                    return viewModel;
                }

                // Get user's library game IDs to exclude them
                var userLibraryGameIds = await _dbContext.GameLibraryItems
                    .Where(g => g.UserId == userId)
                    .Select(g => g.GameId)
                    .ToListAsync(cancellationToken);

                // Extract preferred genres from high-rated games (rating >= 4)
                var preferredGenres = await ExtractPreferredGenresAsync(playedGames, userId, cancellationToken);

                if (preferredGenres.Count == 0)
                {
                    viewModel.Message = "We couldn't identify your preferred genres. Try rating more played games!";
                    return viewModel;
                }

                _logger.LogInformation(
                    "User {UserId} has {GenreCount} preferred genres",
                    userId,
                    preferredGenres.Count);

                // Fetch recommendations by querying each genre
                var recommendations = new List<RecommendedGameViewModel>();

                foreach (var genreSlug in preferredGenres)
                {
                    if (recommendations.Count >= pageSize)
                        break;

                    var gameResponse = await _rawgService.SearchGamesAsync(
                        genre: genreSlug,
                        ordering: "-rating",
                        pageSize: pageSize,
                        page: 1,
                        cancellationToken: cancellationToken);

                    if (gameResponse?.Results == null || gameResponse.Results.Count == 0)
                    {
                        _logger.LogWarning("No games found for genre {Genre}", genreSlug);
                        continue;
                    }

                    // Filter out games already in user's library
                    var newGames = gameResponse.Results
                        .Where(g => !userLibraryGameIds.Contains(g.Id) &&
                                   g.Rating > 0 &&
                                   !string.IsNullOrWhiteSpace(g.BackgroundImage))
                        .Take(pageSize - recommendations.Count)
                        .Select(MapToRecommendedGame)
                        .ToList();

                    recommendations.AddRange(newGames);
                }

                // If we don't have enough recommendations, fetch top-rated games overall
                if (recommendations.Count < pageSize)
                {
                    var topRatedResponse = await _rawgService.SearchGamesAsync(
                        ordering: "-rating",
                        pageSize: pageSize,
                        page: 1,
                        cancellationToken: cancellationToken);

                    if (topRatedResponse?.Results != null)
                    {
                        var topRated = topRatedResponse.Results
                            .Where(g => !userLibraryGameIds.Contains(g.Id) &&
                                       !recommendations.Any(r => r.GameId == g.Id) &&
                                       g.Rating > 0 &&
                                       !string.IsNullOrWhiteSpace(g.BackgroundImage))
                            .Take(pageSize - recommendations.Count)
                            .Select(MapToRecommendedGame)
                            .ToList();

                        recommendations.AddRange(topRated);
                    }
                }

                // Sort by rating descending
                viewModel.Recommendations = recommendations.OrderByDescending(r => r.Rating).ToList();

                if (viewModel.Recommendations.Count == 0)
                {
                    viewModel.Message = "We couldn't find any new games to recommend. Try exploring the search!";
                }
                else
                {
                    viewModel.Message = $"Found {viewModel.Recommendations.Count} recommendations based on your preferences!";
                }

                _logger.LogInformation(
                    "Generated {Count} recommendations for user {UserId}",
                    viewModel.Recommendations.Count,
                    userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating recommendations for user {UserId}", userId);
                viewModel.Message = "An error occurred while generating recommendations. Please try again later.";
                viewModel.Recommendations = new List<RecommendedGameViewModel>();
            }

            return viewModel;
        }

        /// <summary>
        /// Extract preferred genres from user's highly-rated played games
        /// </summary>
        private async Task<List<string>> ExtractPreferredGenresAsync(
            List<GameLibraryItem> playedGames,
            int userId,
            CancellationToken cancellationToken)
        {
            var genreFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var highlyRatedGames = playedGames.Where(g => (g.Rating ?? 0) >= 4).ToList();

            // If no games with rating >= 4, consider games with rating >= 3
            if (highlyRatedGames.Count == 0)
            {
                highlyRatedGames = playedGames.Where(g => (g.Rating ?? 0) >= 3).ToList();
            }

            if (highlyRatedGames.Count == 0)
            {
                // Use all played games if no rated games exist
                highlyRatedGames = playedGames;
            }

            // Infer genres from game titles as fallback approach
            // In a production system, you'd fetch each game's details from RAWG
            foreach (var game in highlyRatedGames.OrderByDescending(g => g.Rating))
            {
                var inferredGenres = InferGenresFromTitle(game.GameName);
                int weight = (game.Rating ?? 2) >= 4 ? 2 : 1;

                foreach (var genre in inferredGenres)
                {
                    if (genreFrequency.ContainsKey(genre))
                        genreFrequency[genre] += weight;
                    else
                        genreFrequency[genre] = weight;
                }
            }

            // Return top 3 genres by frequency
            var topGenres = genreFrequency
                .OrderByDescending(g => g.Value)
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            return topGenres;
        }

        /// <summary>
        /// Simple genre inference from game title (common game keywords)
        /// </summary>
        private List<string> InferGenresFromTitle(string gameName)
        {
            var genres = new List<string>();
            var lowerName = gameName.ToLowerInvariant();

            // Map common keywords to RAWG genre slugs
            var genrePatterns = new Dictionary<string, string[]>
            {
                { "action", new[] { "action", "combat", "shooter", "fight" } },
                { "adventure", new[] { "adventure", "fantasy", "quest", "exploration" } },
                { "rpg", new[] { "rpg", "role-play", "character", "level" } },
                { "strategy", new[] { "strategy", "tactics", "turn", "board" } },
                { "sports", new[] { "sports", "racing", "fifa", "football", "baseball", "soccer" } },
                { "puzzle", new[] { "puzzle", "match", "block" } },
                { "simulation", new[] { "simulation", "sim", "tycoon", "manager" } },
                { "indie", new[] { "indie" } }
            };

            foreach (var (genreSlug, patterns) in genrePatterns)
            {
                if (patterns.Any(p => lowerName.Contains(p)))
                {
                    genres.Add(genreSlug);
                }
            }

            // Fallback to action if no genre identified
            if (genres.Count == 0)
                genres.Add("action");

            return genres;
        }

        /// <summary>
        /// Map RAWG game to recommendation view model
        /// </summary>
        private RecommendedGameViewModel MapToRecommendedGame(Models.Rawg.RawgGame game)
        {
            var platformNames = game.Platforms?
                .Where(p => p.Platform != null)
                .Select(p => p.Platform!.Name)
                .Distinct()
                .ToList() ?? new List<string>();

            var genreNames = game.Genres?
                .Select(g => g.Name)
                .ToList() ?? new List<string>();

            return new RecommendedGameViewModel
            {
                GameId = game.Id,
                Name = game.Name,
                BackgroundImage = game.BackgroundImage,
                Rating = game.Rating,
                RatingsCount = game.RatingsCount,
                Genres = genreNames,
                Platforms = platformNames,
                Released = game.Released
            };
        }
    }
}
