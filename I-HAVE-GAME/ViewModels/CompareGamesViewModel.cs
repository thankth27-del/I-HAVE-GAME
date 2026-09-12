namespace I_HAVE_GAME.ViewModels
{
    /// <summary>
    /// Request model for compare games
    /// </summary>
    public class CompareGamesRequest
    {
        /// <summary>
        /// Selected game IDs to compare
        /// </summary>
        public List<int> SelectedGameIds { get; set; } = new();
    }

    /// <summary>
    /// View model for displaying game comparison
    /// </summary>
    public class CompareGamesViewModel
    {
        /// <summary>
        /// Games to compare
        /// </summary>
        public List<CompareGameViewModel> Games { get; set; } = new();

        /// <summary>
        /// Search results for game selection
        /// </summary>
        public List<GameSearchResultViewModel> SearchResults { get; set; } = new();

        /// <summary>
        /// Status message (empty state, error message, or info)
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// View model for an individual game in comparison
    /// </summary>
    public class CompareGameViewModel
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
        /// Game release date
        /// </summary>
        public string? Released { get; set; }

        /// <summary>
        /// List of genre names
        /// </summary>
        public List<string> Genres { get; set; } = new();

        /// <summary>
        /// List of platform names
        /// </summary>
        public List<string> Platforms { get; set; } = new();

        /// <summary>
        /// Game description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Formatted genres display string (comma-separated)
        /// </summary>
        public string GenresDisplay => string.Join(", ", Genres);

        /// <summary>
        /// Formatted platforms display string (comma-separated)
        /// </summary>
        public string PlatformsDisplay => string.Join(", ", Platforms);

        /// <summary>
        /// Truncated description (first 200 characters)
        /// </summary>
        public string DescriptionTruncated => Description.Length > 200
            ? Description.Substring(0, 200) + "..."
            : Description;
    }

    /// <summary>
    /// View model for game search results
    /// </summary>
    public class GameSearchResultViewModel
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
    }
}
