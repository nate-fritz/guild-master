using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildMaster.Models
{
    public class Room
    {
        public int NumericId { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Dictionary<string, int> Exits { get; set; }
        public List<string> Items { get; set; }
        public List<NPC> NPCs { get; set; }

        // Respawn properties
        public bool CanRespawn { get; set; } = false;
        public float RespawnTimeHours { get; set; } = 16f;
        public int LastClearedDay { get; set; } = -1;
        public float LastClearedHour { get; set; } = -1f;
        public List<NPC> OriginalNPCs { get; set; }  // Store original NPCs for respawning

        public Room()
        {
            Exits = new Dictionary<string, int>();
            Items = new List<string>();
            NPCs = new List<NPC>();
            OriginalNPCs = new List<NPC>();
        }

        public bool ShouldRespawn(int currentDay, float currentHour)
        {
            if (!CanRespawn || LastClearedDay < 0) return false;

            float timeSinceCleared = (currentDay * 24 + currentHour) - (LastClearedDay * 24 + LastClearedHour);
            return timeSinceCleared >= RespawnTimeHours;
        }

        public void RespawnEnemies()
        {
            NPCs.Clear();
            foreach (var npc in OriginalNPCs)
            {
                NPCs.Add(npc.Clone());
            }
        }

        public void MarkCleared(int currentDay, float currentHour)
        {
            if (CanRespawn && NPCs.All(n => !n.IsHostile || n.Health <= 0))
            {
                LastClearedDay = currentDay;
                LastClearedHour = currentHour;
            }
        }
    }
}