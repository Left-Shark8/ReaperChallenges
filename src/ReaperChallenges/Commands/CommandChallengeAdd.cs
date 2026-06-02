using System;
using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace ReaperChallenges.Commands;

public sealed class CommandChallengeAdd : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Both;
    public string Name => "challengeadd";
    public string Help => "Adds progress to a player's active challenges of a type.";
    public string Syntax => "/challengeadd <player> <type> <amount>";
    public List<string> Aliases => new();
    public List<string> Permissions => new() { "reaperchallenges.admin" };

    public void Execute(IRocketPlayer caller, string[] command)
    {
        var plugin = Plugin.Instance;
        if (plugin?.Store == null)
        {
            return;
        }

        if (command.Length < 3 || !int.TryParse(command[2], out var amount) || amount <= 0)
        {
            plugin.Say(caller, Syntax);
            return;
        }

        var player = UnturnedPlayer.FromName(command[0]);
        if (player == null)
        {
            plugin.Say(caller, "That player must be online.");
            return;
        }

        plugin.AddProgress(player.CSteamID.m_SteamID.ToString(), player.DisplayName, command[1], amount, true);
        plugin.Store.Save();
        plugin.Say(caller, $"Added {amount} {command[1]} progress to {player.DisplayName}.");
    }
}
