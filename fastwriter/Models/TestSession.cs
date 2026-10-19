namespace fastwriter.Models;

/// <summary>
/// Sunucu taraflı süre doğrulaması için açılan tek kullanımlık test oturumu.
/// </summary>
public class TestSession
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public int TypingTextId { get; set; }

    public TypingText? TypingText { get; set; }

    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAtUtc { get; set; }
}
