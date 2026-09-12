namespace I_HAVE_GAME.Models
{
    public class QuizQuestion
    {
        public int Id { get; set; }
        public required string QuestionText { get; set; }
        public required string CorrectAnswer { get; set; }
        public required string Choice1 { get; set; }
        public required string Choice2 { get; set; }
        public required string Choice3 { get; set; }
        public required string Choice4 { get; set; }
        public int? CreatedByAdminId { get; set; }
    }
}
