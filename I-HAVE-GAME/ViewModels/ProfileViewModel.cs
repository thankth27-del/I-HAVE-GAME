namespace I_HAVE_GAME.ViewModels
{
    /// <summary>
    /// View model for user profile and dashboard
    /// </summary>
    public class ProfileViewModel
    {
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's nickname (optional)
        /// </summary>
        public string Nickname { get; set; } = string.Empty;

        /// <summary>
        /// User's main device (optional)
        /// </summary>
        public string MainDevice { get; set; } = string.Empty;

        /// <summary>
        /// Account creation date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Total count of played games
        /// </summary>
        public int TotalPlayedCount { get; set; }

        /// <summary>
        /// Total count of wishlist games
        /// </summary>
        public int TotalWishlistCount { get; set; }

        /// <summary>
        /// Total count of backlog games
        /// </summary>
        public int TotalBacklogCount { get; set; }

        /// <summary>
        /// Total search count
        /// </summary>
        public int TotalSearchCount { get; set; }

        /// <summary>
        /// Average rating given to played games
        /// </summary>
        public decimal AverageRating { get; set; }

        /// <summary>
        /// Favorite genres based on played games
        /// </summary>
        public List<GenreCountViewModel> FavoriteGenres { get; set; } = new();

        /// <summary>
        /// Search activity data for chart
        /// </summary>
        public List<SearchActivityViewModel> SearchActivityData { get; set; } = new();

        /// <summary>
        /// Genre proportion data for chart
        /// </summary>
        public List<GenreProportionViewModel> GenreProportionData { get; set; } = new();

        /// <summary>
        /// Total library size (all statuses)
        /// </summary>
        public int TotalLibrarySize => TotalPlayedCount + TotalWishlistCount + TotalBacklogCount;

        /// <summary>
        /// Account age in days
        /// </summary>
        public int AccountAgeDays => (DateTime.UtcNow - CreatedAt).Days;

        /// <summary>
        /// Member since formatted string
        /// </summary>
        public string MemberSince => CreatedAt.ToString("MMMM d, yyyy");
    }

    /// <summary>
    /// View model for genre count
    /// </summary>
    public class GenreCountViewModel
    {
        /// <summary>
        /// Genre name
        /// </summary>
        public string Genre { get; set; } = string.Empty;

        /// <summary>
        /// Count of games in this genre
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// View model for search activity
    /// </summary>
    public class SearchActivityViewModel
    {
        /// <summary>
        /// Date (formatted as string for chart)
        /// </summary>
        public string Date { get; set; } = string.Empty;

        /// <summary>
        /// Number of searches on this date
        /// </summary>
        public int SearchCount { get; set; }
    }

    /// <summary>
    /// View model for genre proportion
    /// </summary>
    public class GenreProportionViewModel
    {
        /// <summary>
        /// Genre name
        /// </summary>
        public string Genre { get; set; } = string.Empty;

        /// <summary>
        /// Count of games
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Percentage out of total
        /// </summary>
        public decimal Percentage { get; set; }
    }
}
