using System.Collections.Generic;

namespace ReaperChallenges.Storage;

public sealed class ChallengeDatabase
{
    public List<PlayerChallengeProgress> Players { get; set; } = new();
}
