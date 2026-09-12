namespace I_HAVE_GAME.Models
{
    public class QuizAttempt
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required int Score { get; set; }
        public required DateTime PlayedAt { get; set; }

        // Foreign key and navigation property
        public User User { get; set; } = null!;
    }
}
