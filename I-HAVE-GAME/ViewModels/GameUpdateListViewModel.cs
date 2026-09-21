using I_HAVE_GAME.Models;

namespace I_HAVE_GAME.ViewModels;

public class GameUpdateListViewModel
{
    public required Game Game { get; init; }
    public IReadOnlyList<GameUpdate> Updates { get; init; } = [];
}
