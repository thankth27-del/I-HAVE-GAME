namespace I_HAVE_GAME.Models
{
    public class GameLibraryItem
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required int GameId { get; set; }
        public string? GameSlug { get; set; }
        public required string GameName { get; set; }
        public string? GameImageUrl { get; set; }
        public required string Status { get; set; }
        public int? Rating { get; set; }
        public string? Review { get; set; }
        public DateTime AddedAt { get; set; }

        // Foreign key and navigation property
        public User User { get; set; } = null!;
    }
}
