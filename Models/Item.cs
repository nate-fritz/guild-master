using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildMaster.Models
{
    public class Item
    {
        public string Description { get; set; }
        public string EmptyDescription { get; set; }  // New: description when contents removed
        public bool IsLootable { get; set; } = true;
        public bool IsConsumable { get; set; } = false;
        public string EffectId { get; set; }

        // Container properties
        public bool IsContainer { get; set; } = false;
        public List<string> Contents { get; set; }
        public string DiscoveryMessage { get; set; }  // New: custom message when finding items

        public Item()
        {
            Contents = new List<string>();
        }
    }
}