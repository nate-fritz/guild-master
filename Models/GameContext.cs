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

        public GameContext()
        {
            NPCs = new Dictionary<string, NPC>();
            Rooms = new Dictionary<int, Room>();
            ItemDescriptions = new Dictionary<int, Dictionary<string, Item>>();
            Effects = new Dictionary<string, Effect>();
        }
    }
}