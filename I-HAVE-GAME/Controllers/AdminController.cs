using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using I_HAVE_GAME.Data;
using I_HAVE_GAME.Models;
using I_HAVE_GAME.ViewModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace I_HAVE_GAME.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AdminController> _logger;

        public AdminController(AppDbContext dbContext, ILogger<AdminController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // GET: /Admin/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var adminUsername = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
            _logger.LogInformation("Admin {AdminUsername} accessed admin dashboard.", adminUsername);

            ViewData["AdminUsername"] = adminUsername;
            ViewData["GameCount"] = await _dbContext.Games.CountAsync();
            ViewData["QuizCount"] = await _dbContext.QuizQuestions.CountAsync();
            return View();
        }

        #region Quiz Question Management

        /// <summary>
        /// GET: /Admin/QuizQuestionList - List all quiz questions
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> QuizQuestionList()
        {
            try
            {
                var questions = await _dbContext.QuizQuestions
                    .OrderBy(q => q.Id)
                    .AsNoTracking()
                    .ToListAsync();

                var viewModel = questions.Select(q => new QuizQuestionListViewModel
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    CorrectAnswer = q.CorrectAnswer,
                    ChoicesCount = 4,
                    CreatedByAdminId = q.CreatedByAdminId
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading quiz questions list");
                TempData["ErrorMessage"] = "Error loading quiz questions.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// GET: /Admin/CreateQuestion - Show create question form
        /// </summary>
        [HttpGet]
        public IActionResult CreateQuestion()
        {
            return View("QuizQuestionForm", new QuizQuestionFormViewModel());
        }

        /// <summary>
        /// POST: /Admin/CreateQuestion - Save new question
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestion(QuizQuestionFormViewModel model)
        {
            try
            {
                // Validate that correct answer is one of the choices
                var choices = new[] { model.Choice1, model.Choice2, model.Choice3, model.Choice4 };
                if (!choices.Contains(model.CorrectAnswer))
                {
                    ModelState.AddModelError(nameof(model.CorrectAnswer), 
                        "Correct answer must be one of the provided choices");
                }

                if (!ModelState.IsValid)
                {
                    return View("QuizQuestionForm", model);
                }

                var adminId = GetCurrentUserId();

                var question = new QuizQuestion
                {
                    QuestionText = model.QuestionText,
                    CorrectAnswer = model.CorrectAnswer,
                    Choice1 = model.Choice1,
                    Choice2 = model.Choice2,
                    Choice3 = model.Choice3,
                    Choice4 = model.Choice4,
                    CreatedByAdminId = adminId
                };

                _dbContext.QuizQuestions.Add(question);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Quiz question created by admin {AdminId}", adminId);
                TempData["SuccessMessage"] = "Quiz question created successfully!";

                return RedirectToAction("QuizQuestionList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating quiz question");
                TempData["ErrorMessage"] = "Error creating quiz question.";
                return View("QuizQuestionForm", model);
            }
        }

        /// <summary>
        /// GET: /Admin/EditQuestion/{id} - Show edit question form
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditQuestion(int id)
        {
            try
            {
                var question = await _dbContext.QuizQuestions.FirstOrDefaultAsync(q => q.Id == id);
                if (question == null)
                {
                    TempData["ErrorMessage"] = "Quiz question not found.";
                    return RedirectToAction("QuizQuestionList");
                }

                var model = new QuizQuestionFormViewModel
                {
                    Id = question.Id,
                    QuestionText = question.QuestionText,
                    CorrectAnswer = question.CorrectAnswer,
                    Choice1 = question.Choice1,
                    Choice2 = question.Choice2,
                    Choice3 = question.Choice3,
                    Choice4 = question.Choice4
                };

                return View("QuizQuestionForm", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading quiz question for edit");
                TempData["ErrorMessage"] = "Error loading quiz question.";
                return RedirectToAction("QuizQuestionList");
            }
        }

        /// <summary>
        /// POST: /Admin/EditQuestion - Update question
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion(QuizQuestionFormViewModel model)
        {
            try
            {
                var question = await _dbContext.QuizQuestions.FirstOrDefaultAsync(q => q.Id == model.Id);
                if (question == null)
                {
                    TempData["ErrorMessage"] = "Quiz question not found.";
                    return RedirectToAction("QuizQuestionList");
                }

                // Validate that correct answer is one of the choices
                var choices = new[] { model.Choice1, model.Choice2, model.Choice3, model.Choice4 };
                if (!choices.Contains(model.CorrectAnswer))
                {
                    ModelState.AddModelError(nameof(model.CorrectAnswer),
                        "Correct answer must be one of the provided choices");
                }

                if (!ModelState.IsValid)
                {
                    return View("QuizQuestionForm", model);
                }

                question.QuestionText = model.QuestionText;
                question.CorrectAnswer = model.CorrectAnswer;
                question.Choice1 = model.Choice1;
                question.Choice2 = model.Choice2;
                question.Choice3 = model.Choice3;
                question.Choice4 = model.Choice4;

                _dbContext.QuizQuestions.Update(question);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Quiz question {QuestionId} updated by admin", model.Id);
                TempData["SuccessMessage"] = "Quiz question updated successfully!";

                return RedirectToAction("QuizQuestionList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating quiz question");
                TempData["ErrorMessage"] = "Error updating quiz question.";
                return View("QuizQuestionForm", model);
            }
        }

        /// <summary>
        /// POST: /Admin/DeleteQuestion - Delete question
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            try
            {
                var question = await _dbContext.QuizQuestions.FirstOrDefaultAsync(q => q.Id == id);
                if (question == null)
                {
                    TempData["ErrorMessage"] = "Quiz question not found.";
                    return RedirectToAction("QuizQuestionList");
                }

                _dbContext.QuizQuestions.Remove(question);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Quiz question {QuestionId} deleted by admin", id);
                TempData["SuccessMessage"] = "Quiz question deleted successfully!";

                return RedirectToAction("QuizQuestionList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting quiz question");
                TempData["ErrorMessage"] = "Error deleting quiz question.";
                return RedirectToAction("QuizQuestionList");
            }
        }

        #endregion

        #region Game Management

        [HttpGet]
        public async Task<IActionResult> GameList()
        {
            return View(await _dbContext.Games.OrderByDescending(game => game.AddedAt).AsNoTracking().ToListAsync());
        }

        [HttpGet]
        public IActionResult CreateGame() => View("GameForm", new GameFormViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGame(GameFormViewModel model)
        {
            if (!ModelState.IsValid) return View("GameForm", model);
            var slug = CreateSlug(model.Slug, model.Title);
            if (await _dbContext.Games.AnyAsync(game => game.Slug == slug))
            {
                ModelState.AddModelError(nameof(model.Slug), "ลิงก์ชื่อนี้ถูกใช้แล้ว กรุณาเปลี่ยนชื่อเกมหรือ slug");
                return View("GameForm", model);
            }
            var game = MapGame(model, new Game { Title = model.Title, Slug = slug });
            game.ImageUrl = await SaveCoverImage(model.CoverImage, model.ImageUrl);
            _dbContext.Games.Add(game);
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "เพิ่มเกมเรียบร้อยแล้ว";
            return RedirectToAction(nameof(GameList));
        }

        [HttpGet]
        public async Task<IActionResult> EditGame(int id)
        {
            var game = await _dbContext.Games.FindAsync(id);
            if (game is null) return NotFound();
            return View("GameForm", new GameFormViewModel { Id = game.Id, Title = game.Title, Slug = game.Slug, Description = game.Description, Genres = game.Genres, Platforms = game.Platforms, Tags = game.Tags, ImageUrl = game.ImageUrl, Rating = game.Rating, Price = game.Price, ReleaseDate = game.ReleaseDate, LastUpdatedAt = game.LastUpdatedAt });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGame(GameFormViewModel model)
        {
            if (!ModelState.IsValid) return View("GameForm", model);
            var game = await _dbContext.Games.FindAsync(model.Id);
            if (game is null) return NotFound();
            var slug = CreateSlug(model.Slug, model.Title);
            if (await _dbContext.Games.AnyAsync(item => item.Id != model.Id && item.Slug == slug))
            {
                ModelState.AddModelError(nameof(model.Slug), "ลิงก์ชื่อนี้ถูกใช้แล้ว");
                return View("GameForm", model);
            }
            model.Slug = slug;
            MapGame(model, game);
            game.ImageUrl = await SaveCoverImage(model.CoverImage, model.ImageUrl);
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "บันทึกการแก้ไขเกมแล้ว";
            return RedirectToAction(nameof(GameList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _dbContext.Games.FindAsync(id);
            if (game is null) return NotFound();
            _dbContext.Games.Remove(game);
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "ลบเกมแล้ว";
            return RedirectToAction(nameof(GameList));
        }

        private static Game MapGame(GameFormViewModel model, Game game)
        {
            game.Title = model.Title.Trim(); game.Slug = CreateSlug(model.Slug, model.Title); game.Description = model.Description?.Trim(); game.Genres = model.Genres?.Trim(); game.Platforms = model.Platforms?.Trim(); game.Tags = model.Tags?.Trim(); game.Rating = model.Rating; game.Price = model.Price; game.ReleaseDate = model.ReleaseDate; game.LastUpdatedAt = model.LastUpdatedAt;
            return game;
        }

        private static string CreateSlug(string? requestedSlug, string title)
        {
            var source = string.IsNullOrWhiteSpace(requestedSlug) ? title : requestedSlug;
            var slug = System.Text.RegularExpressions.Regex.Replace(source.Trim().ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');
            return string.IsNullOrWhiteSpace(slug) ? $"game-{Guid.NewGuid():N}" : slug;
        }

        private async Task<string?> SaveCoverImage(IFormFile? coverImage, string? imageUrl)
        {
            if (coverImage is null || coverImage.Length == 0) return string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(coverImage.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension) || coverImage.Length > 5 * 1024 * 1024) throw new InvalidOperationException("รูปปกต้องเป็น JPG, PNG หรือ WebP และขนาดไม่เกิน 5 MB");
            var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "games");
            Directory.CreateDirectory(directory);
            var fileName = $"{Guid.NewGuid():N}{extension}";
            await using var stream = System.IO.File.Create(Path.Combine(directory, fileName));
            await coverImage.CopyToAsync(stream);
            return $"/uploads/games/{fileName}";
        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> ManageUpdates(int id)
        {
            var game = await _dbContext.Games.FindAsync(id);
            if (game is null) return NotFound();
            return View(new GameUpdateListViewModel { Game = game, Updates = await _dbContext.GameUpdates.Where(update => update.GameId == id).OrderByDescending(update => update.PublishedAt).AsNoTracking().ToListAsync() });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUpdate(int gameId, string title, string? description, DateTime? publishedAt)
        {
            var game = await _dbContext.Games.FindAsync(gameId);
            if (game is null) return NotFound();
            if (string.IsNullOrWhiteSpace(title)) { TempData["ErrorMessage"] = "กรุณากรอกหัวข้ออัปเดต"; return RedirectToAction(nameof(ManageUpdates), new { id = gameId }); }
            var date = publishedAt ?? DateTime.UtcNow;
            _dbContext.GameUpdates.Add(new GameUpdate { GameId = gameId, Title = title.Trim(), Description = description?.Trim(), PublishedAt = date });
            game.LastUpdatedAt = date;
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "เพิ่มรายการอัปเดตแล้ว";
            return RedirectToAction(nameof(ManageUpdates), new { id = gameId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUpdate(int id, int gameId)
        {
            var update = await _dbContext.GameUpdates.FirstOrDefaultAsync(item => item.Id == id && item.GameId == gameId);
            if (update is not null) { _dbContext.GameUpdates.Remove(update); await _dbContext.SaveChangesAsync(); TempData["SuccessMessage"] = "ลบรายการอัปเดตแล้ว"; }
            return RedirectToAction(nameof(ManageUpdates), new { id = gameId });
        }

        /// <summary>
        /// Get current admin's ID from authenticated claims
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim?.Value, out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Admin ID not found");
        }
    }
}
