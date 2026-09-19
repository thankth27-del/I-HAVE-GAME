using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using I_HAVE_GAME.Data;
using I_HAVE_GAME.Models;

namespace I_HAVE_GAME.Pages
{
    public class SearchModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SearchModel> _logger;

        public SearchModel(AppDbContext db, ILogger<SearchModel> logger)
        {
            _db = db;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string? q { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? genre { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? platform { get; set; }

        [BindProperty(SupportsGet = true)]
        public double? minRating { get; set; }

        public List<Game> Results { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q) && string.IsNullOrWhiteSpace(genre) && string.IsNullOrWhiteSpace(platform) && minRating == null)
                    return;

                var query = _db.Games.AsNoTracking().AsQueryable();

                if (!string.IsNullOrWhiteSpace(q))
                {
                    var terms = q.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var t in terms)
                    {
                        var like = $"%{t}%";
                        query = query.Where(g =>
                            EF.Functions.Like(g.Title, like) ||
                            EF.Functions.Like(g.Description, like) ||
                            EF.Functions.Like(g.Genres, like) ||
                            EF.Functions.Like(g.Platforms, like) ||
                            EF.Functions.Like(g.Tags, like));
                    }
                }

                if (!string.IsNullOrWhiteSpace(genre))
                    query = query.Where(g => EF.Functions.Like(g.Genres ?? string.Empty, $"%{genre}%"));
                if (!string.IsNullOrWhiteSpace(platform))
                    query = query.Where(g => EF.Functions.Like(g.Platforms ?? string.Empty, $"%{platform}%"));
                if (minRating != null)
                    query = query.Where(g => (g.Rating ?? 0) >= minRating);

                Results = await query.OrderByDescending(g => g.Rating).Take(200).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during search");
            }
        }
    }
}
