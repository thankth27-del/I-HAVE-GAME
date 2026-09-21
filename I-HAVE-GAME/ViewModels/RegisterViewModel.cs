using System.ComponentModel.DataAnnotations;

namespace I_HAVE_GAME.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อผู้ใช้")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "ชื่อผู้ใช้ต้องมี 3–50 ตัวอักษร")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกอีเมล")]
        [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "รหัสผ่านต้องมีอย่างน้อย 6 ตัวอักษร")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณายืนยันรหัสผ่าน")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "รหัสผ่านทั้งสองช่องไม่ตรงกัน")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
