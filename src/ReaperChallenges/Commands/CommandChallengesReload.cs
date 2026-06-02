using System.Collections.Generic;
using Rocket.API;

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
            return;
        }

        plugin.SaveCleanConfiguration();
        plugin.Store.Save();
        plugin.Say(caller, "Challenge configuration saved.");
    }
}
