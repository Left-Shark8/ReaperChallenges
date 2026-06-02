using System.Collections.Generic;
using Rocket.API;

namespace ReaperChallenges.Commands;

public sealed class CommandCWeekly : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "cweekly";
    public string Help => "Shows or claims your weekly challenges.";
    public string Syntax => "/cweekly | /cweekly claim <number|id>";
    public List<string> Aliases => new() { "weeklychallenges" };
    public List<string> Permissions => new() { "reaperchallenges.challenges" };

    public void Execute(IRocketPlayer caller, string[] command)
    {
        var plugin = Plugin.Instance;
        if (plugin?.Store == null)
        {
            return;
        }

        if (command.Length >= 2 && command[0].Equals("claim", System.StringComparison.OrdinalIgnoreCase))
        {
            ChallengeCommandUtility.Claim(caller, "weekly", command[1]);
            return;
        }

        ChallengeCommandUtility.ShowPeriod(caller, "weekly", Name);
    }
}
