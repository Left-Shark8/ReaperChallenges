using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace ReaperChallenges.Commands;

public sealed class CommandChallenge : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "challenge";
    public string Help => "Shows or claims one challenge.";
    public string Syntax => "/challenge <id> | /challenge claim <id>";
    public List<string> Aliases => new();
    public List<string> Permissions => new() { "reaperchallenges.challenge" };

    public void Execute(IRocketPlayer caller, string[] command)
    {
        var plugin = Plugin.Instance;
        if (plugin?.Store == null)
        {
            return;
        }

        if (command.Length == 0)
        {
            plugin.Say(caller, "Use /cdaily or /cweekly to view and claim challenges.");
            return;
        }

        var claim = command[0].Equals("claim", System.StringComparison.OrdinalIgnoreCase);
        var id = claim && command.Length > 1 ? command[1] : command[0];
        var challenge = plugin.FindActiveChallenge(caller.Id, id);
        if (challenge == null)
        {
            plugin.Say(caller, "That challenge is not active right now.");
            return;
        }

        var progress = plugin.Store.GetOrCreateProgress(caller.Id, challenge.Definition.Id, challenge.PeriodKey);
        if (!claim)
        {
            var claimed = progress.RewardClaimed ? "Reward claimed." : progress.Progress >= challenge.Definition.Target ? "Ready to claim." : "Not complete yet.";
            plugin.Say(caller, $"{challenge.Definition.Name} [{progress.Progress}/{challenge.Definition.Target}]");
            plugin.Say(caller, $"{challenge.Definition.Description} {claimed}");
            return;
        }

        if (progress.Progress < challenge.Definition.Target)
        {
            plugin.Say(caller, $"Not complete yet: {progress.Progress}/{challenge.Definition.Target}.");
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
