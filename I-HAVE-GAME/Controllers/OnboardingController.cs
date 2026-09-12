using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using I_HAVE_GAME.Data;
using I_HAVE_GAME.ViewModels;
using System.Security.Claims;

namespace I_HAVE_GAME.Controllers
{
    [Authorize]
    public class OnboardingController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<OnboardingController> _logger;

        public OnboardingController(AppDbContext dbContext, ILogger<OnboardingController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // GET: /Onboarding/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _dbContext.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // If user has already completed onboarding, redirect to home
            if (!string.IsNullOrEmpty(user.Nickname))
            {
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new OnboardingViewModel
            {
                Nickname = user.Nickname ?? string.Empty,
                MainDevice = user.MainDevice
            };

            return View(viewModel);
        }

        // POST: /Onboarding/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OnboardingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _dbContext.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Update user info
            user.Nickname = model.Nickname;
            user.MainDevice = model.MainDevice;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("User {Username} completed onboarding.", user.Username);

            // Redirect to home
            return RedirectToAction("Index", "Home");
        }
    }
}
