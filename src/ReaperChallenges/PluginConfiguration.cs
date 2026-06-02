using System.Collections.Generic;
using ReaperChallenges.Configuration;
using Rocket.API;

namespace ReaperChallenges;

public sealed class PluginConfiguration : IRocketPluginConfiguration
{
    public string ChatPrefix { get; set; } = "[Challenges]";
    public bool AnnounceCompletedChallenges { get; set; } = true;
    public bool GiveRewardItems { get; set; } = true;
    public int DailyChallengeCount { get; set; } = 3;
    public int WeeklyChallengeCount { get; set; } = 3;
    public int SaveIntervalSeconds { get; set; } = 120;
    public List<ChallengeDefinition> DailyChallenges { get; set; } = new();
    public List<ChallengeDefinition> WeeklyChallenges { get; set; } = new();

    public void LoadDefaults()
    {
        ChatPrefix = "[Challenges]";
        AnnounceCompletedChallenges = true;
        GiveRewardItems = true;
        DailyChallengeCount = 3;
        WeeklyChallengeCount = 3;
        SaveIntervalSeconds = 120;
        DailyChallenges = new List<ChallengeDefinition>
        {
            Create("daily_zombie_kills", "Zombie Slayer", "Kill 100 zombies.", "zombie_kill", 100),
            Create("daily_player_kills", "Player Hunter", "Kill 5 players.", "player_kill", 5),
            Create("daily_craft_items", "Craftsperson", "Craft 15 items.", "craft_item", 15),
            Create("daily_chop_trees", "Lumber Run", "Chop down 15 trees.", "chop_tree", 15),
            Create("daily_headshot_player_kills", "Clean Shots", "Kill 2 players with headshots.", "player_headshot_kill", 2),
            Create("daily_animal_kills", "Hunter", "Kill 3 animals.", "animal_kill", 3),
            Create("daily_harvest_crops", "Farm Hand", "Harvest 15 crops.", "harvest_crop", 15),
            Create("daily_loot_items", "Scavenger", "Find 50 items.", "find_item", 50),
            Create("daily_find_resources", "Resource Runner", "Find 25 resources.", "find_resource", 25),
            Create("daily_find_experience", "XP Sweep", "Find experience 10 times.", "find_experience", 10),
            Create("daily_catch_fish", "Gone Fishing", "Catch 5 fish.", "catch_fish", 5),
            Create("daily_headshots", "Head Clicker", "Land 25 headshots.", "headshot", 25),
            Create("daily_travel_foot", "Trail Runner", "Travel on foot 2,500 times.", "travel_foot", 2500),
            Create("daily_travel_vehicle", "Road Trip", "Travel in a vehicle 5,000 times.", "travel_vehicle", 5000),
            Create("daily_find_buildables", "Builder's Eye", "Find 15 buildables.", "find_buildable", 15),
            Create("daily_find_throwables", "Throwable Hunter", "Find 10 throwables.", "find_throwable", 10),
        };
        WeeklyChallenges = new List<ChallengeDefinition>
        {
            Create("weekly_zombie_kills", "Zombie Exterminator", "Kill 1000 zombies.", "zombie_kill", 1000),
            Create("weekly_player_kills", "Weekly Player Hunter", "Kill 20 players.", "player_kill", 20),
            Create("weekly_craft_items", "Workshop Grind", "Craft 100 items.", "craft_item", 100),
            Create("weekly_chop_trees", "Deforestation", "Chop down 100 trees.", "chop_tree", 100),
            Create("weekly_headshot_player_kills", "Sharpshooter", "Kill 25 players with headshots.", "player_headshot_kill", 25),
            Create("weekly_animal_kills", "Big Game Hunter", "Kill 25 animals.", "animal_kill", 25),
            Create("weekly_harvest_crops", "Harvest Week", "Harvest 100 crops.", "harvest_crop", 100),
            Create("weekly_loot_items", "Loot Marathon", "Find 350 items.", "find_item", 350),
            Create("weekly_find_resources", "Resource Stockpile", "Find 200 resources.", "find_resource", 200),
            Create("weekly_find_experience", "XP Week", "Find experience 75 times.", "find_experience", 75),
            Create("weekly_catch_fish", "Fishing Trip", "Catch 35 fish.", "catch_fish", 35),
            Create("weekly_headshots", "Precision Week", "Land 200 headshots.", "headshot", 200),
            Create("weekly_travel_foot", "Long Walk", "Travel on foot 20,000 times.", "travel_foot", 20000),
            Create("weekly_travel_vehicle", "High Mileage", "Travel in a vehicle 40,000 times.", "travel_vehicle", 40000),
            Create("weekly_find_buildables", "Build Supply Run", "Find 100 buildables.", "find_buildable", 100),
            Create("weekly_find_throwables", "Explosive Collector", "Find 75 throwables.", "find_throwable", 75),
        };
    }

    private static ChallengeDefinition Create(string id, string name, string description, string type, int target)
    {
        return new ChallengeDefinition
        {
            Id = id,
            Name = name,
            Description = description,
            Type = type,
            Target = target,
            RewardItemId = 0,
            RewardAmount = 1,
            RewardMessage = $"{name} complete.",
        };
    }
}
