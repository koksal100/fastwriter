using fastwriter.Models;

namespace fastwriter.ViewModels;

public class ScoreSubmissionDto
{
    /// <summary>Sunucunun test başlarken ürettiği tek kullanımlık oturum kimliği.</summary>
    public Guid SessionId { get; set; }

    public string TypedText { get; set; } = string.Empty;
}

public class ScoreHistoryViewModel
{
    public List<Score> Scores { get; set; } = new();

    public int BestNetWpm { get; set; }

    public int AverageNetWpm { get; set; }

    public double AverageAccuracy { get; set; }

    public int TotalTests { get; set; }
}
