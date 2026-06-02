using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;
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
            UnturnedChat.Say(caller, "ReaperChallenges is not loaded.");
            return;
        }

        if (command.Length == 0)
        {
            UnturnedChat.Say(caller, plugin.Prefix(Syntax));
            return;
        }

        var claim = command[0].Equals("claim", System.StringComparison.OrdinalIgnoreCase);
        var id = claim && command.Length > 1 ? command[1] : command[0];
        var challenge = plugin.FindActiveChallenge(caller.Id, id);
        if (challenge == null)
        {
            UnturnedChat.Say(caller, plugin.Prefix("That challenge is not active right now."));
            return;
        }

        var progress = plugin.Store.GetOrCreateProgress(caller.Id, challenge.Definition.Id, challenge.PeriodKey);
        if (!claim)
        {
            var claimed = progress.RewardClaimed ? "Reward claimed." : progress.Progress >= challenge.Definition.Target ? "Ready to claim." : "Not complete yet.";
            UnturnedChat.Say(caller, plugin.Prefix($"{challenge.Definition.Name}: {challenge.Definition.Description} Progress {progress.Progress}/{challenge.Definition.Target}. {claimed}"));
            return;
        }

        if (progress.Progress < challenge.Definition.Target)
        {
            UnturnedChat.Say(caller, plugin.Prefix($"Not complete yet: {progress.Progress}/{challenge.Definition.Target}."));
            return;
        }

        if (progress.RewardClaimed)
        {
            UnturnedChat.Say(caller, plugin.Prefix("You already claimed that reward."));
            return;
        }

        plugin.ClaimReward((UnturnedPlayer)caller, challenge, false);
    }
}
