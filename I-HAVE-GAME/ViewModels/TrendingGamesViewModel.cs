namespace I_HAVE_GAME.ViewModels
{
    /// <summary>
    /// View model for displaying trending games
    /// </summary>
    public class TrendingGamesViewModel
    {
        /// <summary>
        /// List of trending games
        /// </summary>
        public List<TrendingGameViewModel> TrendingGames { get; set; } = new();

        /// <summary>
        /// Status message (empty state or error message)
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// View model for an individual trending game
    /// </summary>
    public class TrendingGameViewModel
    {
        /// <summary>
        /// RAWG game ID
        /// </summary>
        public int GameId { get; set; }

        /// <summary>
        /// Game name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Game background image URL
        /// </summary>
        public string? BackgroundImage { get; set; }

        /// <summary>
        /// RAWG rating (0-5 scale)
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// Number of ratings this game has received
        /// </summary>
        public int RatingsCount { get; set; }

        /// <summary>
        /// List of genre names
        /// </summary>
        public List<string> Genres { get; set; } = new();

        /// <summary>
        /// List of platform names
        /// </summary>
        public List<string> Platforms { get; set; } = new();

        /// <summary>
        /// Game release date
        /// </summary>
        public string? Released { get; set; }

        /// <summary>
        /// Formatted genres display string (comma-separated)
        /// </summary>
        public string GenresDisplay => string.Join(", ", Genres.Take(3));

        /// <summary>
        /// Formatted platforms display string (comma-separated)
        /// </summary>
        public string PlatformsDisplay => string.Join(", ", Platforms.Take(3));
    }
}
