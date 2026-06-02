namespace ReaperChallenges.Configuration;

public sealed class ChallengeDefinition
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Type { get; set; } = "player_kill";
    public int Target { get; set; } = 1;
    public ushort RewardItemId { get; set; }
    public byte RewardAmount { get; set; } = 1;
    public string RewardMessage { get; set; } = "";
}
