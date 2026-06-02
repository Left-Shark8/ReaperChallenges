using System.Collections.Generic;
using Rocket.API;

namespace ReaperChallenges.Commands;

public sealed class CommandChallengesReset : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "challengesreset";
    public string Help => "Resets challenge progress.";
    public string Syntax => "/challengesreset <daily|weekly|all>";
    public List<string> Aliases => new();
    public List<string> Permissions => new() { "reaperchallenges.admin" };

    public void Execute(IRocketPlayer caller, string[] command)
    {
        var plugin = Plugin.Instance;
        if (plugin?.Store == null)
        {
            return;
        }

        if (command.Length == 0)
        {
            plugin.Say(caller, Syntax);
            return;
        }

        switch (command[0].ToLowerInvariant())
        {
            case "daily":
                plugin.Store.ResetPeriod("D:");
                break;
            case "weekly":
                plugin.Store.ResetPeriod("W:");
                break;
            case "all":
                plugin.Store.ResetAll();
                break;
            default:
                plugin.Say(caller, Syntax);
                return;
        }

        plugin.Store.Save();
        plugin.Say(caller, "Challenge progress reset.");
    }
}
