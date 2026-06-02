# ReaperChallenges

RocketMod Unturned plugin that adds configurable daily and weekly player challenges.

## What It Does

- Gives each player 3 daily and 3 weekly challenges from configurable pools.
- Rotates progress by UTC day and UTC week.
- Shuffles daily challenges every day and weekly challenges every 7 days.
- Automatically tracks zombie kills, player kills, player headshot kills, crafting, tree/resource gathering, animal kills, crop harvesting, looting, fishing, headshots, travel, buildables, and throwables.
- Stores progress in `challenge-progress.xml` so progress survives restarts.
- Gives optional item rewards when a challenge is completed and claimed.
- Supports manual/admin progress for custom challenge types.

## Install

Drop the plugin DLL into your server's Rocket plugins folder:

```text
Rocket/
  Plugins/
    ReaperChallenges.dll
```

Restart the server once so Rocket creates the config.

## Commands

```text
/challenges [daily|weekly]
/challenge <id>
/challenge claim <id>
/challengeadd <player> <type> <amount>
/challengesreset <daily|weekly|all>
/challengesreload
```

## Permissions

```text
reaperchallenges.challenges
reaperchallenges.challenge
reaperchallenges.admin
```

## Config

Default daily and weekly challenge pools are generated on first load. Players receive 3 from each pool:

```xml
<DailyChallengeCount>3</DailyChallengeCount>
<WeeklyChallengeCount>3</WeeklyChallengeCount>
```

Daily pool:

```text
Kill 100 Zombies
Kill 5 Players
Craft 15 Items
Chop Down 15 Trees
Kill 2 Players with Headshot
Kill 3 Animals
Harvest 15 Crops
Find 50 Items
Find 25 Resources
Find Experience 10 Times
Catch 5 Fish
Land 25 Headshots
Travel on Foot 2,500 Times
Travel in a Vehicle 5,000 Times
Find 15 Buildables
Find 10 Throwables
```

Weekly pool:

```text
Kill 1000 Zombies
Kill 20 Players
Craft 100 Items
Chop down 100 Trees
Kill 25 Players with Headshot
Kill 25 Animals
Harvest 100 Crops
Find 350 Items
Find 200 Resources
Find Experience 75 Times
Catch 35 Fish
Land 200 Headshots
Travel on Foot 20,000 Times
Travel in a Vehicle 40,000 Times
Find 100 Buildables
Find 75 Throwables
```

Automatic challenge types:

```text
zombie_kill
player_kill
craft_item
chop_tree
player_headshot_kill
animal_kill
harvest_crop
find_item
find_resource
find_experience
catch_fish
headshot
travel_foot
travel_vehicle
find_buildable
find_throwable
```

Set `RewardItemId` to `0` if a challenge should only send a completion message.

`chop_tree` is driven by Unturned's `FOUND_RESOURCES` stat update, which is the closest Rocket-exposed signal for chopped resource nodes.
`find_resource` uses that same stat, so you can keep both in the pool or remove one from the config if they feel too similar in live play.

You can still define custom challenge types and advance them with:

```text
/challengeadd <player> <type> <amount>
```
