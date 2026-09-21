namespace I_HAVE_GAME.Models;

public class GameUpdate
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
}
