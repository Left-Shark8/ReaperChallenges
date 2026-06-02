using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Chat;

namespace ReaperChallenges.Commands;

public sealed class CommandChallengesReload : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "challengesreload";
    public string Help => "Saves normalized challenge configuration.";
    public string Syntax => "/challengesreload";
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

        plugin.SaveCleanConfiguration();
        plugin.Store.Save();
        UnturnedChat.Say(caller, plugin.Prefix("Challenge configuration saved."));
    }
}
