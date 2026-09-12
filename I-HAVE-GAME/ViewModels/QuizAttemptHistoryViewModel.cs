namespace I_HAVE_GAME.ViewModels
{
    public class QuizAttemptHistoryViewModel
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; } = 5;
        public DateTime PlayedAt { get; set; }
    }
}
