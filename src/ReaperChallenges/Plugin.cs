using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ReaperChallenges.Configuration;
using ReaperChallenges.Storage;
using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace ReaperChallenges;

public sealed class Plugin : RocketPlugin<PluginConfiguration>
{
    public static Plugin? Instance { get; private set; }

    public ChallengeStore? Store { get; private set; }

    protected override void Load()
    {
        Instance = this;
        NormalizeConfiguration();
        Configuration.Save();
        Store = new ChallengeStore(Directory);
        Store.Load();

        U.Events.OnPlayerConnected += OnPlayerConnected;
        UnturnedPlayerEvents.OnPlayerDeath += OnPlayerDeath;
        UnturnedPlayerEvents.OnPlayerUpdateStat += OnPlayerUpdateStat;
        StartCoroutine(SaveLoop());

        Rocket.Core.Logging.Logger.Log($"{Name} loaded.");
    }

    protected override void Unload()
    {
        StopAllCoroutines();
        UnturnedPlayerEvents.OnPlayerUpdateStat -= OnPlayerUpdateStat;
        UnturnedPlayerEvents.OnPlayerDeath -= OnPlayerDeath;
        U.Events.OnPlayerConnected -= OnPlayerConnected;
        Store?.Save();
        Store = null;
        Instance = null;

        Rocket.Core.Logging.Logger.Log($"{Name} unloaded.");
    }

    public string Prefix(string message)
    {
        return $"{Configuration.Instance.ChatPrefix} {message}";
    }

    public Color ChatColor
    {
        get
        {
            var configuration = Configuration.Instance;
            return new Color(configuration.ChatColorRed / 255f, configuration.ChatColorGreen / 255f, configuration.ChatColorBlue / 255f);
        }
    }

    public void Say(IRocketPlayer player, string message)
    {
        UnturnedChat.Say(player, Prefix(message), ChatColor);
    }

    public void Say(string message)
    {
        UnturnedChat.Say(Prefix(message), ChatColor);
    }

    public IReadOnlyList<ActiveChallenge> GetActiveChallenges(string? period = null)
    {
        var configuration = Configuration.Instance;
        var dailyKey = GetDailyPeriodKey(DateTime.UtcNow);
        var weeklyKey = GetWeeklyPeriodKey(DateTime.UtcNow);
        var challenges = new List<ActiveChallenge>();

        if (period == null || period.Trim().Length == 0 || period.Equals("daily", StringComparison.OrdinalIgnoreCase))
        {
            challenges.AddRange((configuration.DailyChallenges ?? new List<ChallengeDefinition>()).Select(definition => new ActiveChallenge(definition, "daily", dailyKey)));
        }

        if (period == null || period.Trim().Length == 0 || period.Equals("weekly", StringComparison.OrdinalIgnoreCase))
        {
            challenges.AddRange((configuration.WeeklyChallenges ?? new List<ChallengeDefinition>()).Select(definition => new ActiveChallenge(definition, "weekly", weeklyKey)));
        }

        return challenges;
    }

    public IReadOnlyList<ActiveChallenge> GetActiveChallengesForPlayer(string steamId, string? period = null)
    {
        var configuration = Configuration.Instance;
        var challenges = new List<ActiveChallenge>();

        if (period == null || period.Trim().Length == 0 || period.Equals("daily", StringComparison.OrdinalIgnoreCase))
        {
            challenges.AddRange(PickChallenges(
                steamId,
                GetDailyPeriodKey(DateTime.UtcNow),
                "daily",
                configuration.DailyChallenges ?? new List<ChallengeDefinition>(),
                configuration.DailyChallengeCount));
        }

        if (period == null || period.Trim().Length == 0 || period.Equals("weekly", StringComparison.OrdinalIgnoreCase))
        {
            challenges.AddRange(PickChallenges(
                steamId,
                GetWeeklyPeriodKey(DateTime.UtcNow),
                "weekly",
                configuration.WeeklyChallenges ?? new List<ChallengeDefinition>(),
                configuration.WeeklyChallengeCount));
        }

        return challenges;
    }

    public ActiveChallenge? FindActiveChallenge(string steamId, string id)
    {
        return GetActiveChallengesForPlayer(steamId)
            .FirstOrDefault(challenge => challenge.Definition.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }

    public ActiveChallenge? FindActiveChallenge(string steamId, string period, string selector)
    {
        var challenges = GetActiveChallengesForPlayer(steamId, period).ToList();
        if (int.TryParse(selector, out var index) && index >= 1 && index <= challenges.Count)
        {
            return challenges[index - 1];
        }

        return challenges.FirstOrDefault(challenge => challenge.Definition.Id.Equals(selector, StringComparison.OrdinalIgnoreCase));
    }

    public void AddProgress(string steamId, string playerName, string challengeType, int amount, bool announce)
    {
        if (Store == null || amount <= 0)
        {
            return;
        }

        Store.RememberPlayer(steamId, playerName);
        var normalizedType = NormalizeChallengeType(challengeType);
        var matching = GetActiveChallengesForPlayer(steamId)
            .Where(challenge => NormalizeChallengeType(challenge.Definition.Type) == normalizedType)
            .ToList();

        foreach (var challenge in matching)
        {
            var definition = challenge.Definition;
            var progress = Store.GetOrCreateProgress(steamId, definition.Id, challenge.PeriodKey);
            if (progress.RewardClaimed || progress.Progress >= definition.Target)
            {
                continue;
            }

            progress.Progress = Math.Min(definition.Target, progress.Progress + amount);
            if (progress.Progress >= definition.Target)
            {
                var player = UnturnedPlayer.FromCSteamID(new CSteamID(ulong.Parse(steamId)));
                if (player != null)
                {
                    ClaimReward(player, challenge, announce);
                }
            }
        }
    }

    public bool ClaimReward(UnturnedPlayer player, ActiveChallenge challenge, bool announce)
    {
        if (Store == null)
        {
            return false;
        }

        var definition = challenge.Definition;
        var progress = Store.GetOrCreateProgress(player.CSteamID.m_SteamID.ToString(), definition.Id, challenge.PeriodKey);
        if (progress.Progress < definition.Target || progress.RewardClaimed)
        {
            return false;
        }

        progress.RewardClaimed = true;
        var message = string.IsNullOrWhiteSpace(definition.RewardMessage)
            ? $"{definition.Name} complete. Reward claimed!"
            : definition.RewardMessage;

        if (Configuration.Instance.GiveRewardItems && definition.RewardItemId > 0 && definition.RewardAmount > 0)
        {
            player.GiveItem(definition.RewardItemId, definition.RewardAmount);
        }

        if (announce && Configuration.Instance.AnnounceCompletedChallenges)
        {
            Say($"{player.DisplayName} completed {definition.Name}.");
        }

        Say(player, message);
        Store.Save();
        return true;
    }

    public void SaveCleanConfiguration()
    {
        NormalizeConfiguration();
        Configuration.Save();
    }

    private void OnPlayerConnected(UnturnedPlayer player)
    {
        Store?.RememberPlayer(player.CSteamID.m_SteamID.ToString(), player.DisplayName);
    }

    private void OnPlayerDeath(UnturnedPlayer victim, EDeathCause cause, ELimb limb, CSteamID murderer)
    {
        if (murderer == CSteamID.Nil || murderer == victim.CSteamID)
        {
            return;
        }

        var killer = UnturnedPlayer.FromCSteamID(murderer);
        if (killer == null)
        {
            return;
        }

        AddProgress(killer.CSteamID.m_SteamID.ToString(), killer.DisplayName, "player_kill", 1, true);
        if (limb == ELimb.SKULL)
        {
            AddProgress(killer.CSteamID.m_SteamID.ToString(), killer.DisplayName, "player_headshot_kill", 1, true);
        }
    }

    private void OnPlayerUpdateStat(UnturnedPlayer player, EPlayerStat stat)
    {
        var steamId = player.CSteamID.m_SteamID.ToString();
        switch (stat)
        {
            case EPlayerStat.KILLS_ZOMBIES_NORMAL:
            case EPlayerStat.KILLS_ZOMBIES_MEGA:
                AddProgress(steamId, player.DisplayName, "zombie_kill", 1, true);
                break;
            case EPlayerStat.KILLS_ANIMALS:
                AddProgress(steamId, player.DisplayName, "animal_kill", 1, true);
                break;
            case EPlayerStat.FOUND_CRAFTS:
                AddProgress(steamId, player.DisplayName, "craft_item", 1, true);
                break;
            case EPlayerStat.FOUND_RESOURCES:
                AddProgress(steamId, player.DisplayName, "chop_tree", 1, true);
                AddProgress(steamId, player.DisplayName, "find_resource", 1, true);
                break;
            case EPlayerStat.FOUND_PLANTS:
                AddProgress(steamId, player.DisplayName, "harvest_crop", 1, true);
                break;
            case EPlayerStat.FOUND_ITEMS:
                AddProgress(steamId, player.DisplayName, "find_item", 1, true);
                break;
            case EPlayerStat.FOUND_EXPERIENCE:
                AddProgress(steamId, player.DisplayName, "find_experience", 1, true);
                break;
            case EPlayerStat.FOUND_FISHES:
                AddProgress(steamId, player.DisplayName, "catch_fish", 1, true);
                break;
            case EPlayerStat.HEADSHOTS:
                AddProgress(steamId, player.DisplayName, "headshot", 1, true);
                break;
            case EPlayerStat.TRAVEL_FOOT:
                AddProgress(steamId, player.DisplayName, "travel_foot", 1, true);
                break;
            case EPlayerStat.TRAVEL_VEHICLE:
                AddProgress(steamId, player.DisplayName, "travel_vehicle", 1, true);
                break;
            case EPlayerStat.FOUND_BUILDABLES:
                AddProgress(steamId, player.DisplayName, "find_buildable", 1, true);
                break;
            case EPlayerStat.FOUND_THROWABLES:
                AddProgress(steamId, player.DisplayName, "find_throwable", 1, true);
                break;
        }
    }

    private IEnumerator SaveLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Math.Max(30, Configuration.Instance.SaveIntervalSeconds));
            Store?.Save();
        }
    }

    private void NormalizeConfiguration()
    {
        var configuration = Configuration.Instance;
        if (configuration.DailyChallengeCount <= 0)
        {
            configuration.DailyChallengeCount = 3;
        }

        if (configuration.WeeklyChallengeCount <= 0)
        {
            configuration.WeeklyChallengeCount = 3;
        }

        configuration.ChatColorRed = ClampColor(configuration.ChatColorRed);
        configuration.ChatColorGreen = ClampColor(configuration.ChatColorGreen);
        configuration.ChatColorBlue = ClampColor(configuration.ChatColorBlue);

        configuration.DailyChallenges ??= new List<ChallengeDefinition>();
        configuration.WeeklyChallenges ??= new List<ChallengeDefinition>();

        var defaults = new PluginConfiguration();
        defaults.LoadDefaults();
        MergeMissingDefinitions(configuration.DailyChallenges, defaults.DailyChallenges);
        MergeMissingDefinitions(configuration.WeeklyChallenges, defaults.WeeklyChallenges);

        NormalizeDefinitions(configuration.DailyChallenges, "daily");
        NormalizeDefinitions(configuration.WeeklyChallenges, "weekly");
    }

    private static void MergeMissingDefinitions(List<ChallengeDefinition> target, IEnumerable<ChallengeDefinition> defaults)
    {
        foreach (var definition in defaults)
        {
            if (target.Any(existing => existing.Id.Equals(definition.Id, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            target.Add(definition);
        }
    }

    private static void NormalizeDefinitions(List<ChallengeDefinition> definitions, string prefix)
    {
        for (var i = 0; i < definitions.Count; i++)
        {
            var definition = definitions[i];
            if (string.IsNullOrWhiteSpace(definition.Id))
            {
                definition.Id = $"{prefix}_{i + 1}";
            }

            definition.Id = definition.Id.Trim();
            definition.Name = string.IsNullOrWhiteSpace(definition.Name) ? definition.Id : definition.Name.Trim();
            definition.Type = NormalizeChallengeType(definition.Type);
            definition.Target = Math.Max(1, definition.Target);
            definition.RewardAmount = definition.RewardAmount == 0 ? (byte)1 : definition.RewardAmount;
        }
    }

    private static string NormalizeChallengeType(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "player_kill"
            : value.Trim().ToLowerInvariant().Replace("-", "_").Replace(" ", "_");
    }

    private static byte ClampColor(byte value)
    {
        return value;
    }

    private static IReadOnlyList<ActiveChallenge> PickChallenges(string steamId, string periodKey, string period, List<ChallengeDefinition> definitions, int count)
    {
        return definitions
            .Where(definition => !string.IsNullOrWhiteSpace(definition.Id))
            .OrderBy(definition => StableHash($"{steamId}:{periodKey}:{definition.Id}"))
            .Take(Math.Max(1, count))
            .Select(definition => new ActiveChallenge(definition, period, periodKey))
            .ToList();
    }

    private static uint StableHash(string value)
    {
        unchecked
        {
            uint hash = 2166136261;
            foreach (var character in value)
            {
                hash ^= character;
                hash *= 16777619;
            }

            return hash;
        }
    }

    private static string GetDailyPeriodKey(DateTime nowUtc)
    {
        return $"D:{nowUtc:yyyyMMdd}";
    }

    private static string GetWeeklyPeriodKey(DateTime nowUtc)
    {
        var calendar = CultureInfo.InvariantCulture.Calendar;
        var week = calendar.GetWeekOfYear(nowUtc, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        return $"W:{nowUtc.Year:0000}-{week:00}";
    }
}
