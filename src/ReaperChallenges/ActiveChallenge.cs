using ReaperChallenges.Configuration;

namespace ReaperChallenges;

public sealed class ActiveChallenge
{
    public ActiveChallenge(ChallengeDefinition definition, string period, string periodKey)
    {
        Definition = definition;
        Period = period;
        PeriodKey = periodKey;
    }

    public ChallengeDefinition Definition { get; }
    public string Period { get; }
    public string PeriodKey { get; }
}
