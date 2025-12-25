using GuildMaster.Models;
using GuildMaster.Data;
using GuildMaster.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuildMaster.Managers
{
    public class RecruitNPCManager
    {
        private GameContext gameContext;
        private Dictionary<int, List<NPC>> dynamicNPCs; // Track dynamically spawned NPCs per room

        // Configuration: Base guild hall rooms (always accessible)
        private static readonly List<int> baseGuildRooms = new List<int> { 1, 2, 3, 4 };

        // Configuration: Progressive guild rooms (unlock based on recruit count)
        private static readonly Dictionary<int, int> progressiveGuildRooms = new Dictionary<int, int>
        {
            { 64, 4 },   // Training Yard - unlocks with 4 recruits
            { 65, 6 },   // Armory - unlocks with 6 recruits
            { 66, 8 },   // Treasury - unlocks with 8 recruits
            { 67, 10 }   // Portal Room - unlocks with 10 recruits
        };

        // Configuration: Class-to-room preferences
        private static readonly Dictionary<string, int> classRoomPreferences = new Dictionary<string, int>
        {
            { "Legionnaire", 64 },  // Training Yard (or Common Area if not unlocked)
            { "Venator", 65 },      // Armory (or Study if not unlocked)
            { "Oracle", 3 }         // Study
        };

        public RecruitNPCManager(GameContext ctx)
        {
            gameContext = ctx;
            dynamicNPCs = new Dictionary<int, List<NPC>>();
        }

        /// <summary>
        /// Updates guild room exits based on which rooms are currently unlocked
        /// Should be called after loading a game or gaining recruits
        /// </summary>
        public void UpdateGuildRoomExits()
        {
            var rooms = gameContext.Rooms;

            // Room 4 (Common Area) - Add west exit to Training Yard (64) if unlocked
            if (rooms.ContainsKey(4) && rooms.ContainsKey(64))
            {
                if (IsRoomUnlocked(64))
                {
                    if (!rooms[4].Exits.ContainsKey("west"))
                        rooms[4].Exits.Add("west", 64);
                }
                else
                {
                    rooms[4].Exits.Remove("west");
                }
            }

            // Room 4 (Common Area) - Add east exit to Armory (65) if unlocked
            if (rooms.ContainsKey(4) && rooms.ContainsKey(65))
            {
                if (IsRoomUnlocked(65))
                {
                    if (!rooms[4].Exits.ContainsKey("east"))
                        rooms[4].Exits.Add("east", 65);
                }
                else
                {
                    rooms[4].Exits.Remove("east");
                }
            }

            // Room 65 (Armory) - Add east exit to Treasury (66) if unlocked
            if (rooms.ContainsKey(65) && rooms.ContainsKey(66))
            {
                if (IsRoomUnlocked(66))
                {
                    if (!rooms[65].Exits.ContainsKey("east"))
                        rooms[65].Exits.Add("east", 66);
                }
                else
                {
                    rooms[65].Exits.Remove("east");
                }
            }

            // Room 66 (Treasury) - Add south exit to Portal Room (67) if unlocked
            if (rooms.ContainsKey(66) && rooms.ContainsKey(67))
            {
                if (IsRoomUnlocked(67))
                {
                    if (!rooms[66].Exits.ContainsKey("south"))
                        rooms[66].Exits.Add("south", 67);
                }
                else
                {
                    rooms[66].Exits.Remove("south");
                }
            }
        }

        /// <summary>
        /// Spawns idle recruits as NPCs in the appropriate guild hall room
        /// </summary>
        public void SpawnIdleRecruitsInRoom(int roomId)
        {
            // Update guild room exits based on current recruit count
            UpdateGuildRoomExits();

            // Only spawn in guild hall rooms
            if (!IsGuildHallRoom(roomId))
                return;

            // Get the room
            if (!gameContext.Rooms.ContainsKey(roomId))
                return;

            var room = gameContext.Rooms[roomId];

            // Get idle recruits who should be in this room
            var idleRecruitsForRoom = GetIdleRecruitsForRoom(roomId);

            if (idleRecruitsForRoom.Count == 0)
                return;

            // Initialize tracking list for this room if needed
            if (!dynamicNPCs.ContainsKey(roomId))
                dynamicNPCs[roomId] = new List<NPC>();

            // Create and spawn NPC for each idle recruit
            foreach (var recruit in idleRecruitsForRoom)
            {
                var recruitNPC = CreateRecruitNPC(recruit);
                room.NPCs.Add(recruitNPC);
                dynamicNPCs[roomId].Add(recruitNPC);
            }
        }

        /// <summary>
        /// Clears all dynamically spawned recruit NPCs from a room
        /// </summary>
        public void ClearDynamicNPCsInRoom(int roomId)
        {
            if (!dynamicNPCs.ContainsKey(roomId))
                return;

            if (!gameContext.Rooms.ContainsKey(roomId))
                return;

            var room = gameContext.Rooms[roomId];
            var npcsToRemove = dynamicNPCs[roomId];

            // Remove each dynamic NPC from the room
            foreach (var npc in npcsToRemove)
            {
                room.NPCs.Remove(npc);
            }

            // Clear the tracking list
            dynamicNPCs[roomId].Clear();
        }

        /// <summary>
        /// Checks if a room is a guild hall room (either base or unlocked progressive)
        /// </summary>
        private bool IsGuildHallRoom(int roomId)
        {
            // Check base guild rooms
            if (baseGuildRooms.Contains(roomId))
                return true;

            // Check progressive rooms (only if unlocked)
            if (progressiveGuildRooms.ContainsKey(roomId))
            {
                int requiredRecruits = progressiveGuildRooms[roomId];
                return gameContext.Player.Recruits.Count >= requiredRecruits;
            }

            return false;
        }

        /// <summary>
        /// Checks if a progressive guild room is unlocked
        /// </summary>
        public bool IsRoomUnlocked(int roomId)
        {
            // Base rooms always unlocked
            if (baseGuildRooms.Contains(roomId))
                return true;

            // Check progressive rooms
            if (progressiveGuildRooms.ContainsKey(roomId))
            {
                int requiredRecruits = progressiveGuildRooms[roomId];
                return gameContext.Player.Recruits.Count >= requiredRecruits;
            }

            return false;
        }

        /// <summary>
        /// Gets all idle recruits (not in active party) that belong in the specified room
        /// </summary>
        private List<Recruit> GetIdleRecruitsForRoom(int roomId)
        {
            var player = gameContext.Player;
            var idleRecruits = new List<Recruit>();

            foreach (var recruit in player.Recruits)
            {
                // Skip if recruit is in active party
                if (player.ActiveParty.Contains(recruit))
                    continue;

                // Check if this recruit belongs in this room
                int preferredRoom = GetPreferredRoom(recruit);
                if (preferredRoom == roomId)
                {
                    idleRecruits.Add(recruit);
                }
            }

            return idleRecruits;
        }

        /// <summary>
        /// Determines which guild hall room a recruit should appear in based on their class
        /// Falls back to base rooms if progressive rooms aren't unlocked yet
        /// </summary>
        private int GetPreferredRoom(Recruit recruit)
        {
            // Get preferred room from configuration
            int preferredRoom = 4; // Default to Common Area

            if (classRoomPreferences.ContainsKey(recruit.Class.Name))
            {
                preferredRoom = classRoomPreferences[recruit.Class.Name];
            }

            // Check if preferred room is unlocked
            if (IsRoomUnlocked(preferredRoom))
            {
                return preferredRoom;
            }

            // Fallback logic if preferred room not unlocked
            return recruit.Class.Name switch
            {
                "Legionnaire" => 4,  // Common Area
                "Venator" => 3,      // Study
                "Oracle" => 3,       // Study
                _ => 4               // Default to Common Area
            };
        }

        /// <summary>
        /// Creates an NPC representation of a recruit for guild hall appearance
        /// </summary>
        private NPC CreateRecruitNPC(Recruit recruit)
        {
            var npc = new NPC
            {
                Name = recruit.Name,
                ShortDescription = $"{recruit.Name} is here, waiting for orders.",
                Description = $"{recruit.Name}, a {recruit.Class.Name} in your guild.",
                IsHostile = false,
                IsVendor = false
            };

            // Set dialogue based on class
            npc.Dialogue = new Dictionary<string, DialogueNode>();

            var greetingNode = new DialogueNode
            {
                Text = GetRecruitDialogue(recruit),
                Action = new DialogueAction { Type = "none" }
            };

            npc.Dialogue["greeting"] = greetingNode;

            return npc;
        }

        /// <summary>
        /// Gets contextual dialogue for a recruit based on their class
        /// </summary>
        private string GetRecruitDialogue(Recruit recruit)
        {
            return recruit.Class.Name switch
            {
                "Legionnaire" => "Just keeping my blade sharp. Ready when you need me.",
                "Venator" => "Waiting for the next hunt. Point me at the target.",
                "Oracle" => "Studying these ancient tomes while I wait. There's always more to learn.",
                _ => "Ready when you need me."
            };
        }
    }
}
