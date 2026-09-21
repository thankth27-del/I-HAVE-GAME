using I_HAVE_GAME.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace I_HAVE_GAME.Controllers;

[Authorize]
public class PetGameController : Controller
{
    private readonly AppDbContext _dbContext;

    public PetGameController(AppDbContext dbContext) => _dbContext = dbContext;

    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> Question()
    {
        var ids = await _dbContext.QuizQuestions.Select(question => question.Id).ToListAsync();
        if (ids.Count == 0) return NotFound(new { message = "ยังไม่มีคำถามในระบบ" });
        var question = await _dbContext.QuizQuestions.FindAsync(ids[Random.Shared.Next(ids.Count)]);
        return Json(new { id = question!.Id, text = question.QuestionText, choices = new[] { question.Choice1, question.Choice2, question.Choice3, question.Choice4 }.OrderBy(_ => Random.Shared.Next()).ToArray() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Answer(int questionId, string answer)
    {
        var question = await _dbContext.QuizQuestions.AsNoTracking().FirstOrDefaultAsync(item => item.Id == questionId);
        if (question is null) return NotFound();
        return Json(new { correct = string.Equals(question.CorrectAnswer, answer, StringComparison.Ordinal), message = string.Equals(question.CorrectAnswer, answer, StringComparison.Ordinal) ? "ตอบถูก! บัดดี้ดีใจมาก" : "ยังไม่ถูก ลองข้อใหม่อีกครั้งนะ" });
    }
}
