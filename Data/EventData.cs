using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;

namespace GuildMaster.Data
{
    /// <summary>
    /// Defines an event that can be triggered when entering a room under specific conditions
    /// </summary>
    public class EventData
    {
        public string EventId { get; set; }
        public int TriggerRoomId { get; set; }
        public List<EventCondition> Conditions { get; set; } = new();
        public int Priority { get; set; } = 0; // Higher priority events trigger first
        public bool IsOneTime { get; set; } = true; // Can event trigger multiple times?
        public string DialogueTreeId { get; set; }
        public List<EventAction> Actions { get; set; } = new();

        public EventData(string eventId, int triggerRoomId)
        {
            EventId = eventId;
            TriggerRoomId = triggerRoomId;
        }
    }

    /// <summary>
    /// Defines a condition that must be met for an event to trigger
    /// </summary>
    public class EventCondition
    {
        public ConditionType Type { get; set; }
        public string TargetId { get; set; } // Quest flag ID, item ID, etc.
        public bool RequiredState { get; set; } = true; // For flags: true/false
        public int MinValue { get; set; } = 0; // For numeric conditions like MinRecruitCount

        public EventCondition(ConditionType type, string targetId = "", bool requiredState = true, int minValue = 0)
        {
            Type = type;
            TargetId = targetId;
            RequiredState = requiredState;
            MinValue = minValue;
        }

        /// <summary>
        /// Evaluates this condition against the game state
        /// </summary>
        public bool Evaluate(GameContext context, HashSet<string> triggeredEvents)
        {
            var player = context.Player;

            switch (Type)
            {
                case ConditionType.FirstVisit:
                    // Event hasn't been triggered yet
                    return !triggeredEvents.Contains(TargetId);

                case ConditionType.FirstVisitAfterFlag:
                    // Quest flag is set AND event hasn't been triggered yet
                    return player.QuestFlags.ContainsKey(TargetId) &&
                           player.QuestFlags[TargetId] == RequiredState &&
                           !triggeredEvents.Contains(TargetId);

                case ConditionType.QuestFlagSet:
                    // Specific quest flag must match required state
                    if (!player.QuestFlags.ContainsKey(TargetId))
                        return !RequiredState; // If flag doesn't exist, it's "false"
                    return player.QuestFlags[TargetId] == RequiredState;

                case ConditionType.HasItem:
                    // Player must have specific item
                    return player.Inventory.Contains(TargetId) == RequiredState;

                case ConditionType.QuestCompleted:
                    // Specific quest must be completed
                    return player.CompletedQuestIds.Contains(TargetId) == RequiredState;

                case ConditionType.QuestActive:
                    // Specific quest must be active
                    bool hasActiveQuest = player.ActiveQuests.Any(q => q.Id == TargetId);
                    return hasActiveQuest == RequiredState;

                case ConditionType.MinRecruitCount:
                    // Minimum number of recruits
                    return player.Recruits.Count >= MinValue;

                case ConditionType.MinGold:
                    // Minimum gold amount
                    return player.Gold >= MinValue;

                case ConditionType.MinLevel:
                    // Minimum player level
                    return player.Level >= MinValue;

                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Types of conditions that can trigger events
    /// </summary>
    public enum ConditionType
    {
        FirstVisit,          // First time entering this room (ignores quest flags)
        FirstVisitAfterFlag, // First visit AFTER a specific quest flag is set
        QuestFlagSet,        // Specific quest flag must be true/false
        HasItem,             // Player must have specific item
        QuestCompleted,      // Specific quest must be completed
        QuestActive,         // Specific quest must be active
        MinRecruitCount,     // Minimum number of recruits (for guild progression compatibility)
        MinGold,             // Minimum gold amount
        MinLevel             // Minimum player level
    }

    /// <summary>
    /// Defines an action to execute when an event triggers
    /// </summary>
    public class EventAction
    {
        public ActionType Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();

        public EventAction(ActionType type)
        {
            Type = type;
        }

        /// <summary>
        /// Executes this action against the game state
        /// </summary>
        public void Execute(GameContext context)
        {
            var player = context.Player;

            switch (Type)
            {
                case ActionType.SetQuestFlag:
                    if (Parameters.ContainsKey("flagId") && Parameters.ContainsKey("value"))
                    {
                        string flagId = Parameters["flagId"].ToString();
                        bool value = bool.Parse(Parameters["value"].ToString());
                        player.QuestFlags[flagId] = value;
                    }
                    break;

                case ActionType.GrantItem:
                    if (Parameters.ContainsKey("itemId"))
                    {
                        string itemId = Parameters["itemId"].ToString();
                        if (!player.Inventory.Contains(itemId))
                        {
                            player.Inventory.Add(itemId);
                        }
                    }
                    break;

                case ActionType.RemoveItem:
                    if (Parameters.ContainsKey("itemId"))
                    {
                        string itemId = Parameters["itemId"].ToString();
                        player.Inventory.Remove(itemId);
                    }
                    break;

                case ActionType.GrantGold:
                    if (Parameters.ContainsKey("amount"))
                    {
                        int amount = int.Parse(Parameters["amount"].ToString());
                        player.Gold += amount;
                    }
                    break;

                case ActionType.RemoveGold:
                    if (Parameters.ContainsKey("amount"))
                    {
                        int amount = int.Parse(Parameters["amount"].ToString());
                        player.Gold = System.Math.Max(0, player.Gold - amount);
                    }
                    break;

                case ActionType.GrantAbility:
                    if (Parameters.ContainsKey("abilityId"))
                    {
                        string abilityId = Parameters["abilityId"].ToString();
                        // TODO: Implement ability granting when ability system exists
                    }
                    break;

                case ActionType.ForceTravel:
                    if (Parameters.ContainsKey("targetRoomId"))
                    {
                        int targetRoomId = int.Parse(Parameters["targetRoomId"].ToString());
                        player.CurrentRoom = targetRoomId;
                    }
                    break;

                case ActionType.AddPartyMember:
                    if (Parameters.ContainsKey("recruitName"))
                    {
                        string recruitName = Parameters["recruitName"].ToString();
                        var recruit = player.Recruits.FirstOrDefault(r => r.Name == recruitName);
                        if (recruit != null && !player.ActiveParty.Contains(recruit) && player.ActiveParty.Count < 3)
                        {
                            player.ActiveParty.Add(recruit);
                        }
                    }
                    break;

                case ActionType.RemovePartyMember:
                    if (Parameters.ContainsKey("recruitName"))
                    {
                        string recruitName = Parameters["recruitName"].ToString();
                        var recruit = player.ActiveParty.FirstOrDefault(r => r.Name == recruitName);
                        if (recruit != null)
                        {
                            player.ActiveParty.Remove(recruit);
                        }
                    }
                    break;

                case ActionType.SpawnNPC:
                    if (Parameters.ContainsKey("npcName") && Parameters.ContainsKey("roomId"))
                    {
                        string npcName = Parameters["npcName"].ToString();
                        int roomId = int.Parse(Parameters["roomId"].ToString());

                        if (context.NPCs.ContainsKey(npcName) && context.Rooms.ContainsKey(roomId))
                        {
                            var npc = context.NPCs[npcName];
                            var room = context.Rooms[roomId];

                            if (!room.NPCs.Contains(npc))
                            {
                                room.NPCs.Add(npc);
                            }
                        }
                    }
                    break;

                case ActionType.RemoveNPC:
                    if (Parameters.ContainsKey("npcName") && Parameters.ContainsKey("roomId"))
                    {
                        string npcName = Parameters["npcName"].ToString();
                        int roomId = int.Parse(Parameters["roomId"].ToString());

                        if (context.NPCs.ContainsKey(npcName) && context.Rooms.ContainsKey(roomId))
                        {
                            var room = context.Rooms[roomId];
                            // Remove by name to ensure removal works
                            room.NPCs.RemoveAll(n => n.Name == npcName);
                        }
                    }
                    break;

                case ActionType.AdvanceTime:
                    if (Parameters.ContainsKey("hours"))
                    {
                        float hours = float.Parse(Parameters["hours"].ToString());

                        // Advance player time
                        player.CurrentHour += hours;

                        // Handle day overflow
                        while (player.CurrentHour >= 24)
                        {
                            player.CurrentHour -= 24;
                            player.CurrentDay++;
                        }

                        // Process time-dependent systems
                        // Quest completion checks
                        var questManager = GuildMaster.Services.ProgramStatics.questManager;
                        if (questManager != null)
                        {
                            questManager.CheckCompletedQuests();
                        }
                    }
                    break;

                case ActionType.AllyWithFaction:
                    if (Parameters.ContainsKey("factionId"))
                    {
                        string factionId = Parameters["factionId"].ToString();
                        if (!player.AlliedFactions.Contains(factionId))
                        {
                            player.AlliedFactions.Add(factionId);
                        }
                    }
                    break;

                case ActionType.BreakAlliance:
                    if (Parameters.ContainsKey("factionId"))
                    {
                        string factionId = Parameters["factionId"].ToString();
                        if (player.AlliedFactions.Contains(factionId))
                        {
                            player.AlliedFactions.Remove(factionId);
                        }
                    }
                    break;

                case ActionType.UnlockRegion:
                    if (Parameters.ContainsKey("regionId"))
                    {
                        string regionId = Parameters["regionId"].ToString();
                        if (!player.UnlockedRegions.Contains(regionId))
                        {
                            player.UnlockedRegions.Add(regionId);
                        }
                    }
                    break;

                case ActionType.LockRegion:
                    if (Parameters.ContainsKey("regionId"))
                    {
                        string regionId = Parameters["regionId"].ToString();
                        if (player.UnlockedRegions.Contains(regionId))
                        {
                            player.UnlockedRegions.Remove(regionId);
                        }
                    }
                    break;

                case ActionType.DisplayMessage:
                    if (Parameters.ContainsKey("message"))
                    {
                        string message = Parameters["message"].ToString();
                        Services.AnsiConsole.MarkupLine(message);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Types of actions that events can execute
    /// </summary>
    public enum ActionType
    {
        SetQuestFlag,       // Update a quest flag
        GrantItem,          // Add item to player inventory
        RemoveItem,         // Remove item from player inventory
        GrantGold,          // Give player gold
        RemoveGold,         // Take gold from player
        GrantAbility,       // Unlock new ability
        ForceTravel,        // Move player to different room
        AddPartyMember,     // Add recruit to active party
        RemovePartyMember,  // Remove recruit from active party
        SpawnNPC,           // Add NPC to a room
        RemoveNPC,          // Remove NPC from a room
        AdvanceTime,        // Advance game time and process time-dependent systems
        AllyWithFaction,    // Form an alliance with a faction
        BreakAlliance,      // Break an alliance with a faction
        UnlockRegion,       // Unlock a region for access
        LockRegion,         // Lock a region (remove access)
        DisplayMessage      // Display a message to the player
    }
}
