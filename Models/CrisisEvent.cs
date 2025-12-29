namespace GuildMaster.Models
{
    /// <summary>
    /// Represents a crisis event that threatens a seal.
    /// Crises must be handled by assigning squads or they will damage seals.
    /// </summary>
    public class CrisisEvent
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string TargetSealId { get; set; } = "";  // Which seal this crisis threatens
        public int TurnsRemaining { get; set; } = 3;  // How many turns until it damages the seal
        public int DifficultyRating { get; set; } = 50;  // Combat power needed to handle it
        public int SealDamage { get; set; } = 20;  // How much damage it deals if not handled
        public bool IsResolved { get; set; } = false;
        public CrisisType Type { get; set; }

        /// <summary>
        /// Gets a description of the crisis urgency.
        /// </summary>
        public string GetUrgencyDescription()
        {
            if (TurnsRemaining <= 1) return "[red]CRITICAL[/]";
            if (TurnsRemaining == 2) return "[orange]URGENT[/]";
            return "[yellow]MODERATE[/]";
        }

        /// <summary>
        /// Gets the color for displaying turns remaining.
        /// </summary>
        public string GetTurnColor()
        {
            if (TurnsRemaining <= 1) return "red";
            if (TurnsRemaining == 2) return "orange";
            return "yellow";
        }

        /// <summary>
        /// Decrements the turns remaining.
        /// </summary>
        public void DecrementTurns()
        {
            if (TurnsRemaining > 0)
                TurnsRemaining--;
        }
    }

    /// <summary>
    /// Types of crisis events that can occur.
    /// </summary>
    public enum CrisisType
    {
        DemonIncursion,     // Demons attacking a seal
        CultistRitual,      // Cultists performing dark rituals
        MonsterHorde,       // Monster army advancing
        Sabotage,           // Internal sabotage attempt
        NaturalDisaster,    // Earthquake, storm, etc. weakening seal
        Corruption          // Magical corruption spreading
    }
}
