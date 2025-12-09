// Data/QuestData.cs
using System.Collections.Generic;
using GuildMaster.Models;

namespace GuildMaster.Data
{
    public static class QuestData
    {
        public static List<Quest> GetAvailableQuests()
        {
            return new List<Quest>
            {
                // Easy Quests (4-8 hours)
                new Quest
                {
                    Id = "patrol_roads",
                    Name = "Patrol the Roads",
                    Description = "Scout the roads near the guild for bandit activity",
                    Duration = 4.0f,
                    Difficulty = "Easy",
                    BaseSuccessChance = 80,
                    MinGold = 5,
                    MaxGold = 10,
                    ItemRewards = new Dictionary<string, int> { {"potion", 40} },
                    BaseExperienceReward = 50
                },
                new Quest
                {
                    Id = "deliver_message",
                    Name = "Message Delivery",
                    Description = "Deliver a message to the nearby farm",
                    Duration = 6.0f,
                    Difficulty = "Easy",
                    BaseSuccessChance = 90,
                    MinGold = 3,
                    MaxGold = 7,
                    ItemRewards = new Dictionary<string, int>(),
                    BaseExperienceReward = 40
                },
                
                // Medium Quests (8-16 hours)
                new Quest
                {
                    Id = "clear_wolves",
                    Name = "Wolf Hunting",
                    Description = "Clear wolves from the forest paths",
                    Duration = 12.0f,
                    Difficulty = "Medium",
                    BaseSuccessChance = 65,
                    MinGold = 15,
                    MaxGold = 25,
                    ItemRewards = new Dictionary<string, int>
                    {
                        {"potion", 60},
                        {"iron sword", 20}
                    },
                    BaseExperienceReward = 100
                },
                new Quest
                {
                    Id = "escort_merchant",
                    Name = "Merchant Escort",
                    Description = "Guard a merchant traveling to Belum",
                    Duration = 10.0f,
                    Difficulty = "Medium",
                    BaseSuccessChance = 70,
                    MinGold = 20,
                    MaxGold = 30,
                    ItemRewards = new Dictionary<string, int> { {"battle axe", 15} },
                    PotentialRecruit = "Grax",  // A potential new recruit
                    BaseExperienceReward = 100
                },
                
                // Hard Quests (16-24 hours)
                new Quest
                {
                    Id = "bandit_camp",
                    Name = "Raid Bandit Camp",
                    Description = "Assault a known bandit hideout in the mountains",
                    Duration = 20.0f,
                    Difficulty = "Hard",
                    BaseSuccessChance = 50,
                    MinGold = 40,
                    MaxGold = 60,
                    ItemRewards = new Dictionary<string, int>
                    {
                        {"potion", 80},
                        {"iron sword", 40},
                        {"battle axe", 30}
                    },
                    PotentialRecruit = "Marcus",  // Another potential recruit
                    BaseExperienceReward = 200
                },
                new Quest
                {
                    Id = "ancient_ruins",
                    Name = "Explore Ancient Ruins",
                    Description = "Investigate mysterious ruins deep in the forest",
                    Duration = 24.0f,
                    Difficulty = "Hard",
                    BaseSuccessChance = 45,
                    MinGold = 50,
                    MaxGold = 80,
                    ItemRewards = new Dictionary<string, int>
                    {
                        {"potion", 70},
                        {"amulet", 25}  // Rare item
                    },
                    BaseExperienceReward = 200
                }
            };
        }
    }
}