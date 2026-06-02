using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;

namespace ReaperChallenges.Commands;

public sealed class CommandChallenges : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "challenges";
    public string Help => "Shows your daily and weekly challenges.";
    public string Syntax => "/challenges [daily|weekly]";
    public List<string> Aliases => new() { "chal" };
    public List<string> Permissions => new() { "reaperchallenges.challenges" };

    public void Execute(IRocketPlayer caller, string[] command)
    {
        var plugin = Plugin.Instance;
        if (plugin?.Store == null)
        {
            UnturnedChat.Say(caller, "ReaperChallenges is not loaded.");
            return;
        }

        string? period = command.Length > 0 ? command[0] : null;
        if (period != null && !period.Equals("daily", StringComparison.OrdinalIgnoreCase) && !period.Equals("weekly", StringComparison.OrdinalIgnoreCase))
        {
            UnturnedChat.Say(caller, plugin.Prefix("Use /challenges daily or /challenges weekly."));
            return;
        }

        var challenges = plugin.GetActiveChallengesForPlayer(caller.Id, period).ToList();
        if (challenges.Count == 0)
        {
            UnturnedChat.Say(caller, plugin.Prefix("No challenges are configured."));
            return;
        }

        foreach (var challenge in challenges)
        {
            var progress = plugin.Store.GetOrCreateProgress(caller.Id, challenge.Definition.Id, challenge.PeriodKey);
            var claimed = progress.RewardClaimed ? "claimed" : progress.Progress >= challenge.Definition.Target ? "ready" : "in progress";
            UnturnedChat.Say(caller, plugin.Prefix($"{challenge.Period}: {challenge.Definition.Name} ({challenge.Definition.Id}) - {progress.Progress}/{challenge.Definition.Target} {claimed}"));
        }
    }
}
