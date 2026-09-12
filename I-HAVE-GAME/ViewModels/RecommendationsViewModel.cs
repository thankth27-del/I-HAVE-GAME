namespace I_HAVE_GAME.ViewModels
{
    /// <summary>
    /// View model for displaying game recommendations
    /// </summary>
    public class RecommendationsViewModel
    {
        /// <summary>
        /// List of recommended games
        /// </summary>
        public List<RecommendedGameViewModel> Recommendations { get; set; } = new();

        /// <summary>
        /// Status message (empty state or info message)
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// View model for an individual recommended game
    /// </summary>
    public class RecommendedGameViewModel
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
        /// Release date as string
        /// </summary>
        public string? Released { get; set; }

        /// <summary>
        /// Formatted genres for display (comma-separated)
        /// </summary>
        public string GenresDisplay => string.Join(", ", Genres);

        /// <summary>
        /// Formatted platforms for display (comma-separated)
        /// </summary>
        public string PlatformsDisplay => string.Join(", ", Platforms);
    }
}
