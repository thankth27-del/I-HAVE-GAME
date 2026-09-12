namespace I_HAVE_GAME.ViewModels
{
    public class QuizResultViewModel
    {
        public int Score { get; set; }
        public int TotalQuestions { get; set; } = 5;
        public DateTime PlayedAt { get; set; }
        public bool AttemptRecorded { get; set; }
    }
}
