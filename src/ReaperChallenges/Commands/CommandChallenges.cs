using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;

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
            return;
        }

        string? period = command.Length > 0 ? command[0] : null;
        if (period != null && !period.Equals("daily", StringComparison.OrdinalIgnoreCase) && !period.Equals("weekly", StringComparison.OrdinalIgnoreCase))
        {
            plugin.Say(caller, "Use /cdaily or /cweekly for cleaner challenge lists.");
            return;
        }

        if (period == null)
        {
            plugin.Say(caller, "Use /cdaily for daily challenges.");
            plugin.Say(caller, "Use /cweekly for weekly challenges.");
            return;
        }

        ChallengeCommandUtility.ShowPeriod(caller, period, period.Equals("daily", StringComparison.OrdinalIgnoreCase) ? "cdaily" : "cweekly");
    }
}
