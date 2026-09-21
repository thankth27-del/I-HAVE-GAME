using System.ComponentModel.DataAnnotations;

namespace I_HAVE_GAME.ViewModels;

public class GameFormViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "กรุณากรอกชื่อเกม")]
    [StringLength(160)] public string Title { get; set; } = string.Empty;
    [StringLength(160)] public string? Slug { get; set; }
    [StringLength(1500)] public string? Description { get; set; }
    [StringLength(300)] public string? Genres { get; set; }
    [StringLength(500)] public string? Platforms { get; set; }
    [StringLength(500)] public string? Tags { get; set; }
    [Url(ErrorMessage = "URL รูปภาพไม่ถูกต้อง")] public string? ImageUrl { get; set; }
    [Display(Name = "อัปโหลดรูปปก")] public IFormFile? CoverImage { get; set; }
    [Range(0, 5, ErrorMessage = "คะแนนต้องอยู่ระหว่าง 0 ถึง 5")] public double? Rating { get; set; }
    [Range(0, 99999, ErrorMessage = "ราคาไม่ถูกต้อง")] public decimal? Price { get; set; }
    [DataType(DataType.Date)] public DateTime? ReleaseDate { get; set; }
    [DataType(DataType.Date)] public DateTime? LastUpdatedAt { get; set; }
    public bool IsEdit => Id > 0;
}
