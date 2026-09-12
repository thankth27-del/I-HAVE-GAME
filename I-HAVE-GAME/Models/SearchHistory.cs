namespace I_HAVE_GAME.Models
{
    public class SearchHistory
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public string? Genre { get; set; }
        public string? Device { get; set; }
        public string? PlayMode { get; set; }
        public string? Budget { get; set; }
        public string? Era { get; set; }
        public required int ResultCount { get; set; }
        public DateTime SearchedAt { get; set; }

        // Foreign key and navigation property
        public User User { get; set; } = null!;
    }
}
