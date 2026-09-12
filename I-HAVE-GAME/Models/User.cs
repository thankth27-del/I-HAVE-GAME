namespace I_HAVE_GAME.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public string? Nickname { get; set; }
        public string? MainDevice { get; set; }
        public required string Role { get; set; }
        public required DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<GameLibraryItem> GameLibraryItems { get; set; } = new List<GameLibraryItem>();
        public ICollection<SearchHistory> SearchHistories { get; set; } = new List<SearchHistory>();
        public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    }
}
