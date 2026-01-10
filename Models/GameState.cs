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
        public bool RoomNumbersEnabled { get; set; } = false;
        public bool DebugLogsEnabled { get; set; } = false;
        public bool WarRoomEnabled { get; set; } = false;  // Dev/test flag for War Room access

        // Dialogue tracking for repeat conversations and topic acknowledgment
        public HashSet<string> MetNPCs { get; set; }  // Track which NPCs the player has met
        public Dictionary<string, HashSet<string>> VisitedDialogueNodes { get; set; }  // Track visited dialogue nodes per NPC

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
        public List<string> CompletedQuestIds { get; set; }
        public Dictionary<string, bool> QuestFlags { get; set; }  // Quest state flags
        public HashSet<string> AlliedFactions { get; set; }  // Allied factions
        public HashSet<string> UnlockedRegions { get; set; }  // Unlocked regions

        // Events
        public HashSet<string> TriggeredEventIds { get; set; }  // Track which one-time events have triggered

        // Priority Message System
        public HashSet<string> ShownMessages { get; set; }

        // Puzzle system
        public Dictionary<string, PuzzleState> PuzzleStates { get; set; }  // Track puzzle states
        public HashSet<string> ExaminedObjects { get; set; }  // Track which room objects have been examined

        // Timer system for timed events (e.g., letter translation)
        public Dictionary<string, GameTimer> ActiveTimers { get; set; }  // Track active timers

        // Milestone tracking for dynamic content
        public int TotalRecruitsEver { get; set; } = 0;
        public HashSet<string> CompletedMilestones { get; set; }
        public Dictionary<int, string> RoomStateOverrides { get; set; }  // RoomId -> State

        // War Room (Act III)
        public WarRoomState? WarRoomState { get; set; }

        // Room respawn tracking
        public Dictionary<int, RoomRespawnData> RoomRespawnStates { get; set; }

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
            CompletedQuestIds = new List<string>();
            QuestFlags = new Dictionary<string, bool>();
            AlliedFactions = new HashSet<string>();
            UnlockedRegions = new HashSet<string>();
            TriggeredEventIds = new HashSet<string>();
            ShownMessages = new HashSet<string>();
            CompletedMilestones = new HashSet<string>();
            RoomStateOverrides = new Dictionary<int, string>();
            MetNPCs = new HashSet<string>();
            VisitedDialogueNodes = new Dictionary<string, HashSet<string>>();
            PuzzleStates = new Dictionary<string, PuzzleState>();
            ExaminedObjects = new HashSet<string>();
            ActiveTimers = new Dictionary<string, GameTimer>();
            RoomRespawnStates = new Dictionary<int, RoomRespawnData>();
        }
    }

    public class RoomRespawnData
    {
        public int LastClearedDay { get; set; }
        public float LastClearedHour { get; set; }
    }

    public class GameTimer
    {
        public string TimerId { get; set; }
        public int StartDay { get; set; }
        public float StartHour { get; set; }
        public float DurationHours { get; set; }

        public bool IsComplete(int currentDay, float currentHour)
        {
            // Calculate total hours elapsed
            float totalHoursElapsed = ((currentDay - StartDay) * 24f) + (currentHour - StartHour);
            return totalHoursElapsed >= DurationHours;
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