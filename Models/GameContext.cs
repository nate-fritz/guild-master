using System.Collections.Generic;

namespace GuildMaster.Models
{
    public class GameContext
    {
        public Player Player { get; set; }
        public Dictionary<string, NPC> NPCs { get; set; }
        public Dictionary<int, Room> Rooms { get; set; }
        public Dictionary<int, Dictionary<string, Item>> ItemDescriptions { get; set; }
        public Dictionary<string, Effect> Effects { get; set; }
        public string NoteText { get; set; }

        // Milestone tracking for dynamic content
        public int TotalRecruitsEver { get; set; } = 0;
        public HashSet<string> CompletedMilestones { get; set; }
        public Dictionary<int, string> RoomStateOverrides { get; set; }

        // Puzzle system
        public Dictionary<string, PuzzleState> PuzzleStates { get; set; }

        public GameContext()
        {
            NPCs = new Dictionary<string, NPC>();
            Rooms = new Dictionary<int, Room>();
            ItemDescriptions = new Dictionary<int, Dictionary<string, Item>>();
            Effects = new Dictionary<string, Effect>();
            CompletedMilestones = new HashSet<string>();
            RoomStateOverrides = new Dictionary<int, string>();
            PuzzleStates = new Dictionary<string, PuzzleState>();
        }

        /// <summary>
        /// Helper method to check if a quest flag is set to true.
        /// Returns false if the flag doesn't exist.
        /// </summary>
        public bool GetQuestFlag(string flagId)
        {
            if (Player?.QuestFlags == null)
                return false;

            if (!Player.QuestFlags.ContainsKey(flagId))
                return false;

            return Player.QuestFlags[flagId];
        }

        /// <summary>
        /// Helper method to set a quest flag.
        /// </summary>
        public void SetQuestFlag(string flagId, bool value)
        {
            if (Player?.QuestFlags != null)
            {
                Player.QuestFlags[flagId] = value;
            }
        }
    }
}