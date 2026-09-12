using I_HAVE_GAME.Models.Rawg;

namespace I_HAVE_GAME.Services
{
    /// <summary>
    /// Service interface for RAWG API interactions
    /// </summary>
    public interface IRawgService
    {
        /// <summary>
        /// Search for games with optional filters
        /// </summary>
        Task<RawgGameListResponse?> SearchGamesAsync(
            string? genre = null,
            string? platforms = null,
            string? search = null,
            string? ordering = null,
            int pageSize = 20,
            int page = 1,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Search games with release date filtering
        /// </summary>
        Task<RawgGameListResponse?> SearchGamesAsync(
            string? genre,
            string? platforms,
            string? released,
            string? search,
            string? ordering,
            int pageSize,
            int page,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all genres
        /// </summary>
        Task<List<RawgGenre>?> GetGenresAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all platforms
        /// </summary>
        Task<List<RawgPlatform>?> GetPlatformsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a single game by ID
        /// </summary>
        Task<RawgGame?> GetGameAsync(int gameId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if API is properly configured
        /// </summary>
        bool IsConfigured();
    }
}
