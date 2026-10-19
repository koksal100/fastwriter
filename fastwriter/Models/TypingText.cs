namespace fastwriter.Models;

public class TypingText
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Category { get; set; } = "Hukuk";

    public int Difficulty { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Score> Scores { get; set; } = new List<Score>();
}
