namespace fastwriter.ViewModels;

public class LeaderboardEntryViewModel
{
    public int Rank { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public int NetWpm { get; set; }

    public int RawWpm { get; set; }

    public double Accuracy { get; set; }

    public int CorrectWords { get; set; }

    public int DurationSeconds { get; set; }

    public int AttemptCount { get; set; }

    public DateTime AchievedAt { get; set; }
}

public class LeaderboardViewModel
{
    public string Period { get; set; } = "all";

    public List<LeaderboardEntryViewModel> Entries { get; set; } = new();

    public LeaderboardEntryViewModel? CurrentUserEntry { get; set; }

    public string? CurrentUserId { get; set; }
}
