using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using I_HAVE_GAME.Data;
using I_HAVE_GAME.Models;
using I_HAVE_GAME.ViewModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace I_HAVE_GAME.Controllers
{
    // เกมทายชื่อเกมถูกปลดออกแล้ว โดยคำถามถูกนำไปใช้กับ Game Buddy แทน
    [NonController]
    public class QuizController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<QuizController> _logger;

        private const int QUESTIONS_PER_ROUND = 5;
        private const string QUIZ_SESSION_KEY = "QuizSession";
        private const string QUIZ_IDS_KEY = "QuestionIds";
        private const string QUIZ_INDEX_KEY = "CurrentIndex";
        private const string QUIZ_SCORE_KEY = "CurrentScore";
        private const string QUIZ_RECORDED_KEY = "AttemptRecorded";

        public QuizController(AppDbContext dbContext, ILogger<QuizController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Get current user's ID from authenticated claims
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim?.Value, out var userId))
            {
                return userId;
            }

            _logger.LogWarning("Unable to extract UserId from claims for Quiz");
            throw new UnauthorizedAccessException("User ID not found");
        }

        /// <summary>
        /// GET: /Quiz/Index - Show quiz intro page
        /// </summary>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// GET: /Quiz/Play - Start a new quiz or continue current round
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Play()
        {
            try
            {
                // Check if there's an active quiz session
                var sessionData = HttpContext.Session.GetString(QUIZ_SESSION_KEY);

                if (string.IsNullOrEmpty(sessionData))
                {
                    // Start a new quiz - fetch 5 random questions
                    var totalQuestions = await _dbContext.QuizQuestions.CountAsync();

                    if (totalQuestions < QUESTIONS_PER_ROUND)
                    {
                        ViewBag.ErrorMessage = $"Not enough questions available. Found {totalQuestions} questions, need at least {QUESTIONS_PER_ROUND}.";
                        return View("Error", new ErrorViewModel { RequestId = "QuizInitError" });
                    }

                    var newQuestionIds = await _dbContext.QuizQuestions
                        .Select(q => q.Id)
                        .ToListAsync();

                    // Randomize the question selection using Fisher-Yates shuffle
                    var random = new Random();
                    for (int i = newQuestionIds.Count - 1; i > 0; i--)
                    {
                        int randomIndex = random.Next(i + 1);
                        (newQuestionIds[i], newQuestionIds[randomIndex]) = (newQuestionIds[randomIndex], newQuestionIds[i]);
                    }

                    newQuestionIds = newQuestionIds.Take(QUESTIONS_PER_ROUND).ToList();

                    // Store in session
                    HttpContext.Session.SetString(QUIZ_IDS_KEY, string.Join(",", newQuestionIds));
                    HttpContext.Session.SetInt32(QUIZ_INDEX_KEY, 0);
                    HttpContext.Session.SetInt32(QUIZ_SCORE_KEY, 0);
                    HttpContext.Session.SetString(QUIZ_RECORDED_KEY, "false");
                    HttpContext.Session.SetString(QUIZ_SESSION_KEY, "active");

                    _logger.LogInformation("New quiz session started for user");
                }

                // Get current question index
                var currentIndex = HttpContext.Session.GetInt32(QUIZ_INDEX_KEY) ?? 0;
                var questionIdsString = HttpContext.Session.GetString(QUIZ_IDS_KEY);

                if (string.IsNullOrEmpty(questionIdsString))
                {
                    return RedirectToAction("Index");
                }

                var questionIds = questionIdsString.Split(',').Select(int.Parse).ToList();

                // If all questions answered, go to result
                if (currentIndex >= QUESTIONS_PER_ROUND)
                {
                    return RedirectToAction("Result");
                }

                // Fetch current question
                var currentQuestionId = questionIds[currentIndex];
                var question = await _dbContext.QuizQuestions.FirstOrDefaultAsync(q => q.Id == currentQuestionId);

                if (question == null)
                {
                    _logger.LogError("Question with ID {QuestionId} not found", currentQuestionId);
                    return RedirectToAction("Index");
                }

                var viewModel = new QuizViewModel
                {
                    Id = question.Id,
                    QuestionNumber = currentIndex + 1,
                    TotalQuestions = QUESTIONS_PER_ROUND,
                    QuestionText = question.QuestionText,
                    Choice1 = question.Choice1,
                    Choice2 = question.Choice2,
                    Choice3 = question.Choice3,
                    Choice4 = question.Choice4
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Quiz Play action");
                ViewBag.ErrorMessage = "An error occurred while loading the quiz. Please try again.";
                return View("Error", new ErrorViewModel { RequestId = "QuizPlayError" });
            }
        }

        /// <summary>
        /// POST: /Quiz/Answer - Submit answer for current question
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer([FromForm] int questionId, [FromForm] string? answer)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(answer))
                {
                    ModelState.AddModelError("", "Please select an answer");
                    return RedirectToAction("Play");
                }

                // Verify question exists and get correct answer
                var question = await _dbContext.QuizQuestions.FirstOrDefaultAsync(q => q.Id == questionId);
                if (question == null)
                {
                    _logger.LogWarning("Question with ID {QuestionId} not found", questionId);
                    return RedirectToAction("Play");
                }

                // Check if answer is correct
                if (question.CorrectAnswer == answer)
                {
                    // Increment score
                    var currentScore = HttpContext.Session.GetInt32(QUIZ_SCORE_KEY) ?? 0;
                    HttpContext.Session.SetInt32(QUIZ_SCORE_KEY, currentScore + 1);

                    _logger.LogInformation("Correct answer for question {QuestionId}", questionId);
                }
                else
                {
                    _logger.LogInformation("Incorrect answer for question {QuestionId}. Expected: {Expected}, Got: {Got}", 
                        questionId, question.CorrectAnswer, answer);
                }

                // Move to next question
                var currentIndex = HttpContext.Session.GetInt32(QUIZ_INDEX_KEY) ?? 0;
                HttpContext.Session.SetInt32(QUIZ_INDEX_KEY, currentIndex + 1);

                // Check if all questions answered
                if (currentIndex + 1 >= QUESTIONS_PER_ROUND)
                {
                    return RedirectToAction("Result");
                }

                return RedirectToAction("Play");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting quiz answer");
                return RedirectToAction("Play");
            }
        }

        /// <summary>
        /// GET: /Quiz/Result - Display final score and save attempt
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Result()
        {
            try
            {
                var currentScore = HttpContext.Session.GetInt32(QUIZ_SCORE_KEY) ?? 0;
                var attemptRecordedString = HttpContext.Session.GetString(QUIZ_RECORDED_KEY) ?? "false";
                var attemptRecorded = bool.Parse(attemptRecordedString);

                var viewModel = new QuizResultViewModel
                {
                    Score = currentScore,
                    TotalQuestions = QUESTIONS_PER_ROUND,
                    PlayedAt = DateTime.UtcNow,
                    AttemptRecorded = attemptRecorded
                };

                // Save quiz attempt only once
                if (!attemptRecorded)
                {
                    var userId = GetCurrentUserId();

                    var attempt = new QuizAttempt
                    {
                        UserId = userId,
                        Score = currentScore,
                        PlayedAt = DateTime.UtcNow
                    };

                    _dbContext.QuizAttempts.Add(attempt);
                    await _dbContext.SaveChangesAsync();

                    HttpContext.Session.SetString(QUIZ_RECORDED_KEY, "true");
                    viewModel.AttemptRecorded = true;

                    _logger.LogInformation("Quiz attempt recorded for user {UserId} with score {Score}", userId, currentScore);
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error displaying quiz result");
                ViewBag.ErrorMessage = "An error occurred while saving your result. Please try again.";
                return View("Error", new ErrorViewModel { RequestId = "QuizResultError" });
            }
        }

        /// <summary>
        /// POST: /Quiz/Reset - Clear session and start new round
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reset()
        {
            // Clear quiz session
            HttpContext.Session.Remove(QUIZ_SESSION_KEY);
            HttpContext.Session.Remove(QUIZ_IDS_KEY);
            HttpContext.Session.Remove(QUIZ_INDEX_KEY);
            HttpContext.Session.Remove(QUIZ_SCORE_KEY);
            HttpContext.Session.Remove(QUIZ_RECORDED_KEY);

            _logger.LogInformation("Quiz session cleared for new round");

            return RedirectToAction("Play");
        }

        /// <summary>
        /// GET: /Quiz/History - Display user's quiz attempt history
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> History()
        {
            try
            {
                var userId = GetCurrentUserId();

                var attempts = await _dbContext.QuizAttempts
                    .Where(qa => qa.UserId == userId)
                    .OrderByDescending(qa => qa.PlayedAt)
                    .AsNoTracking()
                    .ToListAsync();

                var viewModel = attempts.Select(a => new QuizAttemptHistoryViewModel
                {
                    Id = a.Id,
                    Score = a.Score,
                    TotalQuestions = QUESTIONS_PER_ROUND,
                    PlayedAt = a.PlayedAt
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading quiz history");
                ViewBag.ErrorMessage = "An error occurred while loading your quiz history.";
                return View("Error", new ErrorViewModel { RequestId = "QuizHistoryError" });
            }
        }
    }
}
