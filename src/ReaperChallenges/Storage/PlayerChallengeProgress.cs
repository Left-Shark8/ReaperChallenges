using System.Collections.Generic;

namespace ReaperChallenges.Storage;

public sealed class PlayerChallengeProgress
{
    public string SteamId { get; set; } = "";
    public string LastKnownName { get; set; } = "";
    public List<ChallengeProgress> Challenges { get; set; } = new();
}
