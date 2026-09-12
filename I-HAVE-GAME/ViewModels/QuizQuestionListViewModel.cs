namespace I_HAVE_GAME.ViewModels
{
    public class QuizQuestionListViewModel
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public int ChoicesCount { get; set; } = 4;
        public int? CreatedByAdminId { get; set; }
    }
}
