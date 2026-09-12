namespace I_HAVE_GAME.ViewModels
{
    public class QuizViewModel
    {
        public int Id { get; set; }
        public int QuestionNumber { get; set; }
        public int TotalQuestions { get; set; } = 5;
        public string QuestionText { get; set; } = string.Empty;
        public string Choice1 { get; set; } = string.Empty;
        public string Choice2 { get; set; } = string.Empty;
        public string Choice3 { get; set; } = string.Empty;
        public string Choice4 { get; set; } = string.Empty;
        public string? SelectedAnswer { get; set; }
    }
}
