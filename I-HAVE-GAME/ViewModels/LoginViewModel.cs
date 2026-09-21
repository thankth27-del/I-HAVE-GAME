using System.ComponentModel.DataAnnotations;

namespace I_HAVE_GAME.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อผู้ใช้หรืออีเมล")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
