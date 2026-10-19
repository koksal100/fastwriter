namespace fastwriter.Services;

public record TypingMetricsResult(
    int NetWpm,
    int RawWpm,
    double Accuracy,
    int CorrectChars,
    int IncorrectChars,
    int TotalChars,
    int CorrectWords);

/// <summary>
/// Karakter bazlı yazma metriklerini hesaplar. İstemci tarafındaki hesaplamayla
/// aynı formülleri kullanır; skor sunucuda bu sınıfla yeniden hesaplanır.
/// </summary>
public static class TypingMetrics
{
    public static TypingMetricsResult Calculate(string target, string typed, int durationSeconds)
    {
        target ??= string.Empty;
        typed ??= string.Empty;

        durationSeconds = Math.Clamp(durationSeconds, 1, 3600);

        var compareLength = Math.Min(target.Length, typed.Length);
        var correctChars = 0;

        for (var i = 0; i < compareLength; i++)
        {
            if (target[i] == typed[i])
            {
                correctChars++;
            }
        }

        var totalChars = typed.Length;
        var incorrectChars = totalChars - correctChars;
        var minutes = durationSeconds / 60.0;

        var rawWpm = (int)Math.Round(totalChars / 5.0 / minutes);
        var netWpm = (int)Math.Round(correctChars / 5.0 / minutes);
        var accuracy = totalChars > 0
            ? Math.Round(correctChars * 100.0 / totalChars, 2)
            : 0;

        return new TypingMetricsResult(
            NetWpm: Math.Max(0, netWpm),
            RawWpm: Math.Max(0, rawWpm),
            Accuracy: accuracy,
            CorrectChars: correctChars,
            IncorrectChars: incorrectChars,
            TotalChars: totalChars,
            CorrectWords: CountCorrectWords(target, typed));
    }

    private static int CountCorrectWords(string target, string typed)
    {
        var targetWords = target.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var typedWords = typed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var length = Math.Min(targetWords.Length, typedWords.Length);
        var correct = 0;

        for (var i = 0; i < length; i++)
        {
            if (string.Equals(targetWords[i], typedWords[i], StringComparison.Ordinal))
            {
                correct++;
            }
        }

        return correct;
    }
}
