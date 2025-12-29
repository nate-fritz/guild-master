using System.Collections.Generic;

namespace GuildMaster.Models
{
    public class RoomObject
    {
        public string Id { get; set; }  // Unique identifier, e.g., "bandit_vault_door"
        public string Name { get; set; }  // Display name, e.g., "vault door"
        public string[] Aliases { get; set; }  // Alternative names, e.g., ["door", "iron door", "vault"]
        public string DefaultDescription { get; set; }  // What you see when examining
        public string LookedAtDescription { get; set; }  // Optional: Changes after first examination
        public bool HasBeenExamined { get; set; } = false;
        public bool IsInteractable { get; set; } = false;
        public bool IsHidden { get; set; } = false;  // If true, object won't show in room listing
        public string InteractionType { get; set; }  // "use", "move", "pull", "ring", "step", "set", etc.
        public Dictionary<string, object> State { get; set; } = new Dictionary<string, object>();
        public string RequiredItem { get; set; }  // Optional: Item needed to interact (e.g., "bronze_key")
        public string OnInteractMessage { get; set; }  // Message shown when interacted with
        public string OnFailMessage { get; set; }  // Message shown when interaction fails

        public RoomObject()
        {
            Aliases = new string[0];
            State = new Dictionary<string, object>();
        }
    }
}
