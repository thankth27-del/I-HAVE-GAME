namespace I_HAVE_GAME.Models
{
    public class Game
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? Genres { get; set; }        // comma-separated genres
        public string? Platforms { get; set; }     // comma-separated platforms
        public string? Tags { get; set; }          // comma-separated tags/keywords
        public string? ImageUrl { get; set; }
        public double? Rating { get; set; }
        public decimal? Price { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
