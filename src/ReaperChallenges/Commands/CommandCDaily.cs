using System.Collections.Generic;
using Rocket.API;

namespace ReaperChallenges.Commands;

public sealed class CommandCDaily : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "cdaily";
    public string Help => "Shows or claims your daily challenges.";
    public string Syntax => "/cdaily | /cdaily claim <number|id>";
    public List<string> Aliases => new() { "dailychallenges" };
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
            ChallengeCommandUtility.Claim(caller, "daily", command[1]);
            return;
        }

        ChallengeCommandUtility.ShowPeriod(caller, "daily", Name);
    }
}
