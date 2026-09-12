using System.ComponentModel.DataAnnotations;

namespace I_HAVE_GAME.ViewModels
{
    public class OnboardingViewModel
    {
        [Required(ErrorMessage = "Nickname is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nickname must be between 2 and 50 characters.")]
        public string Nickname { get; set; } = string.Empty;

        public string? MainDevice { get; set; }

        public static readonly string[] DeviceOptions = new[]
        {
            "PC",
            "PlayStation",
            "Xbox",
            "Nintendo Switch",
            "Mobile",
            "Other",
            "Prefer not to specify"
        };
    }
}
