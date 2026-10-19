using fastwriter.Models;

namespace fastwriter.ViewModels;

public class ScoreSubmissionDto
{
    public int TextId { get; set; }

    public string TypedText { get; set; } = string.Empty;

    public int DurationSeconds { get; set; }
}

public class ScoreHistoryViewModel
{
    public List<Score> Scores { get; set; } = new();

    public int BestNetWpm { get; set; }

    public int AverageNetWpm { get; set; }

    public double AverageAccuracy { get; set; }

    public int TotalTests { get; set; }
}
