namespace I_HAVE_GAME.ViewModels;

public class GameCalendarViewModel
{
    public required DateTime Month { get; init; }
    public IReadOnlyList<GameCalendarEventViewModel> Events { get; init; } = [];
}

public class GameCalendarEventViewModel
{
    public required DateTime Date { get; init; }
    public required string Kind { get; init; }
    public required string Title { get; init; }
    public required string GameTitle { get; init; }
    public required string GameSlug { get; init; }
}
