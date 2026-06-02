using System.Linq;
using Rocket.API;
using Rocket.Unturned.Player;

namespace ReaperChallenges.Commands;

internal static class ChallengeCommandUtility
{
    public static void ShowPeriod(IRocketPlayer caller, string period, string commandName)
    {
        var plugin = Plugin.Instance;
        if (plugin?.Store == null)
        {
            return;
        }

        var challenges = plugin.GetActiveChallengesForPlayer(caller.Id, period).ToList();
        if (challenges.Count == 0)
        {
            plugin.Say(caller, $"No {period} challenges are configured.");
            return;
        }

        plugin.Say(caller, $"{period.ToUpperInvariant()} CHALLENGES");
        for (var i = 0; i < challenges.Count; i++)
        {
            var challenge = challenges[i];
            var progress = plugin.Store.GetOrCreateProgress(caller.Id, challenge.Definition.Id, challenge.PeriodKey);
            var status = progress.RewardClaimed ? "CLAIMED" : progress.Progress >= challenge.Definition.Target ? "READY" : "ACTIVE";
            plugin.Say(caller, $"{i + 1}. {challenge.Definition.Name} [{progress.Progress}/{challenge.Definition.Target}] {status}");
            plugin.Say(caller, $"   {challenge.Definition.Description} | claim: /{commandName} claim {i + 1}");
        }
    }

    public static void Claim(IRocketPlayer caller, string period, string selector)
    {
        var plugin = Plugin.Instance;
        if (plugin?.Store == null)
        {
            return;
        }

        var challenge = plugin.FindActiveChallenge(caller.Id, period, selector);
        if (challenge == null)
        {
            plugin.Say(caller, $"That {period} challenge is not active. Use /c{period} to see your list.");
            return;
        }

        var progress = plugin.Store.GetOrCreateProgress(caller.Id, challenge.Definition.Id, challenge.PeriodKey);
        if (progress.Progress < challenge.Definition.Target)
        {
            plugin.Say(caller, $"{challenge.Definition.Name} is not complete yet: {progress.Progress}/{challenge.Definition.Target}.");
            return;
        }

        if (progress.RewardClaimed)
        {
            plugin.Say(caller, "You already claimed that reward.");
            return;
        }

        plugin.ClaimReward((UnturnedPlayer)caller, challenge, false);
    }
}
