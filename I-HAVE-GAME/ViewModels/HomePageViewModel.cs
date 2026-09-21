using I_HAVE_GAME.Models;

namespace I_HAVE_GAME.ViewModels;

public class HomePageViewModel
{
    public Game? SpotlightGame { get; init; }
    public IReadOnlyList<Game> TopGames { get; init; } = [];
    public IReadOnlyList<Game> NewGames { get; init; } = [];
    public IReadOnlyDictionary<string, IReadOnlyList<Game>> GenreShelves { get; init; } = new Dictionary<string, IReadOnlyList<Game>>();
    public int WishlistCount { get; init; }
    public int BacklogCount { get; init; }
    public int PlayedCount { get; init; }
}
