namespace fastwriter.Models;

public class Score
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    public int TypingTextId { get; set; }

    public TypingText? TypingText { get; set; }

    /// <summary>Doğru karakter sayısına göre hesaplanan net kelime/dakika.</summary>
    public int NetWpm { get; set; }

    /// <summary>Yazılan tüm karakterlere göre hesaplanan ham kelime/dakika.</summary>
    public int RawWpm { get; set; }

    /// <summary>Doğru karakter / toplam yazılan karakter (yüzde).</summary>
    public double Accuracy { get; set; }

    public int CorrectChars { get; set; }

    public int IncorrectChars { get; set; }

    public int TotalChars { get; set; }

    public int CorrectWords { get; set; }

    public int DurationSeconds { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
