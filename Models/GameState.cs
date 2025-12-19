using System.Collections.Generic;

namespace GuildMaster.Models
{
    public class GameState
    {
        // Player basics
        public string PlayerName { get; set; }
        public int CurrentRoom { get; set; }
        public List<string> PlayerInventory { get; set; }
        public HashSet<string> TakenItems { get; set; }
        public HashSet<string> ExaminedItems { get; set; }
        public int SaveVersion { get; set; } = 2;  // Track save format version

        // Player stats
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public int Gold { get; set; }
        public int AttackDamage { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; }
        public string EquippedWeapon { get; set; }  // Keep for backward compatibility
        public string EquippedWeaponName { get; set; }
        public string EquippedArmorName { get; set; }
        public string EquippedHelmName { get; set; }
        public string EquippedRingName { get; set; }
        public string PlayerClass { get; set; }
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public int ExperienceToNextLevel { get; set; } = 100;
        public bool AutoCombatEnabled { get; set; } = false;
        public bool TutorialsEnabled { get; set; } = true;
        public bool GoreEnabled { get; set; } = false;

        // Time
        public int CurrentDay { get; set; }
        public float CurrentHour { get; set; }

        // NPCs
        public Dictionary<string, string> NPCDialogueStates { get; set; }
        public Dictionary<int, List<string>> RemovedNPCs { get; set; }  // Room ID -> List of removed NPC names

        // Guild
        public List<SavedRecruit> Recruits { get; set; }
        public List<string> ActivePartyNames { get; set; }

        // Quests
        public List<SavedQuest> ActiveQuests { get; set; }

        // Priority Message System
        public HashSet<string> ShownMessages { get; set; }
        public GameState()
        {
            PlayerInventory = new List<string>();
            TakenItems = new HashSet<string>();
            ExaminedItems = new HashSet<string>();
            NPCDialogueStates = new Dictionary<string, string>();
            RemovedNPCs = new Dictionary<int, List<string>>();
            Recruits = new List<SavedRecruit>();
            ActivePartyNames = new List<string>();
            ActiveQuests = new List<SavedQuest>();
            ShownMessages = new HashSet<string>();
        }
    }

    public class SavedRecruit
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public int RecruitedDay { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public int AttackDamage { get; set; }
        public int Defense { get; set; }
        public int Speed { get; set; }
        public bool IsOnQuest { get; set; }
        public bool IsResting { get; set; }
        public float RestUntil { get; set; }
        public int RestUntilDay { get; set; }
        public string EquippedWeapon { get; set; }  // Keep for backward compatibility
        public string EquippedWeaponName { get; set; }
        public string EquippedArmorName { get; set; }
        public string EquippedHelmName { get; set; }
        public string EquippedRingName { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int ExperienceToNextLevel { get; set; }
    }

    public class SavedQuest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Difficulty { get; set; }
        public string AssignedRecruitName { get; set; }
        public int StartDay { get; set; }
        public float StartTime { get; set; }
        public float Duration { get; set; }
        public int MinGold { get; set; }
        public int MaxGold { get; set; }
        public int BaseSuccessChance { get; set; }
        public int BaseExperienceReward { get; set; }
        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }
        public bool WasSuccessful { get; set; }
        public Dictionary<string, int> ItemRewards { get; set; }
        public string PotentialRecruit { get; set; }
    }
}