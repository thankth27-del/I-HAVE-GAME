using System.ComponentModel.DataAnnotations;

namespace I_HAVE_GAME.ViewModels
{
    public class QuizQuestionFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Question text is required")]
        [StringLength(500, ErrorMessage = "Question text cannot exceed 500 characters")]
        public string QuestionText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Correct answer is required")]
        [StringLength(200, ErrorMessage = "Correct answer cannot exceed 200 characters")]
        public string CorrectAnswer { get; set; } = string.Empty;

        [Required(ErrorMessage = "Choice 1 is required")]
        [StringLength(200, ErrorMessage = "Choice cannot exceed 200 characters")]
        [Display(Name = "Option 1")]
        public string Choice1 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Choice 2 is required")]
        [StringLength(200, ErrorMessage = "Choice cannot exceed 200 characters")]
        [Display(Name = "Option 2")]
        public string Choice2 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Choice 3 is required")]
        [StringLength(200, ErrorMessage = "Choice cannot exceed 200 characters")]
        [Display(Name = "Option 3")]
        public string Choice3 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Choice 4 is required")]
        [StringLength(200, ErrorMessage = "Choice cannot exceed 200 characters")]
        [Display(Name = "Option 4")]
        public string Choice4 { get; set; } = string.Empty;

        public bool IsEdit => Id > 0;
        public string FormTitle => IsEdit ? "Edit Quiz Question" : "Create New Quiz Question";
    }
}
