using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;

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
            UnturnedChat.Say(caller, "ReaperChallenges is not loaded.");
            return;
        }

        if (command.Length == 0)
        {
            UnturnedChat.Say(caller, plugin.Prefix(Syntax));
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
                UnturnedChat.Say(caller, plugin.Prefix(Syntax));
                return;
        }

        plugin.Store.Save();
        UnturnedChat.Say(caller, plugin.Prefix("Challenge progress reset."));
    }
}
