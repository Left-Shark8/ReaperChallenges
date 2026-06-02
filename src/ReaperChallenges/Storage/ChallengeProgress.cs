namespace ReaperChallenges.Storage;

public sealed class ChallengeProgress
{
    public string ChallengeId { get; set; } = "";
    public string PeriodKey { get; set; } = "";
    public int Progress { get; set; }
    public bool RewardClaimed { get; set; }
}
