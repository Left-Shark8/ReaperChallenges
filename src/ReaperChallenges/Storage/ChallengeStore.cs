using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ReaperChallenges.Storage;

public sealed class ChallengeStore
{
    private readonly string filePath;
    private readonly XmlSerializer serializer = new(typeof(ChallengeDatabase));
    private ChallengeDatabase database = new();

    public ChallengeStore(string pluginDirectory)
    {
        filePath = Path.Combine(pluginDirectory, "challenge-progress.xml");
    }

    public void Load()
    {
        if (!File.Exists(filePath))
        {
            database = new ChallengeDatabase();
            Save();
            return;
        }

        using var stream = File.OpenRead(filePath);
        database = serializer.Deserialize(stream) as ChallengeDatabase ?? new ChallengeDatabase();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");
        using var stream = File.Create(filePath);
        serializer.Serialize(stream, database);
    }

    public void RememberPlayer(string steamId, string name)
    {
        if (string.IsNullOrWhiteSpace(steamId))
        {
            return;
        }

        var player = GetOrCreatePlayer(steamId);
        if (!string.IsNullOrWhiteSpace(name))
        {
            player.LastKnownName = name;
        }
    }

    public ChallengeProgress GetOrCreateProgress(string steamId, string challengeId, string periodKey)
    {
        var player = GetOrCreatePlayer(steamId);
        var progress = player.Challenges.FirstOrDefault(item => item.ChallengeId == challengeId && item.PeriodKey == periodKey);
        if (progress != null)
        {
            return progress;
        }

        progress = new ChallengeProgress
        {
            ChallengeId = challengeId,
            PeriodKey = periodKey,
        };
        player.Challenges.Add(progress);
        return progress;
    }

    public IReadOnlyList<ChallengeProgress> GetProgress(string steamId)
    {
        return database.Players.FirstOrDefault(player => player.SteamId == steamId)?.Challenges.ToList()
            ?? new List<ChallengeProgress>();
    }

    public void ResetPeriod(string periodPrefix)
    {
        foreach (var player in database.Players)
        {
            player.Challenges.RemoveAll(progress => progress.PeriodKey.StartsWith(periodPrefix, StringComparison.OrdinalIgnoreCase));
        }
    }

    public void ResetAll()
    {
        database = new ChallengeDatabase();
    }

    private PlayerChallengeProgress GetOrCreatePlayer(string steamId)
    {
        var player = database.Players.FirstOrDefault(item => item.SteamId == steamId);
        if (player != null)
        {
            return player;
        }

        player = new PlayerChallengeProgress { SteamId = steamId };
        database.Players.Add(player);
        return player;
    }
}
