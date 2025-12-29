namespace GuildMaster.Models
{
    /// <summary>
    /// Represents one of the five seals protecting the realm.
    /// Seals can be weakened by crisis events and must be defended.
    /// </summary>
    public class Seal
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Integrity { get; set; } = 100;  // 0-100, game over if any reach 0
        public int MaxIntegrity { get; set; } = 100;
        public string Location { get; set; } = "";  // Where the seal is located
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Damages the seal's integrity.
        /// </summary>
        public void TakeDamage(int amount)
        {
            Integrity = Math.Max(0, Integrity - amount);
            if (Integrity == 0)
            {
                IsActive = false;
            }
        }

        /// <summary>
        /// Repairs the seal's integrity.
        /// </summary>
        public void Repair(int amount)
        {
            Integrity = Math.Min(MaxIntegrity, Integrity + amount);
            if (Integrity > 0)
            {
                IsActive = true;
            }
        }

        /// <summary>
        /// Gets the integrity percentage (0.0 to 1.0).
        /// </summary>
        public float GetIntegrityPercent()
        {
            return (float)Integrity / MaxIntegrity;
        }

        /// <summary>
        /// Gets a color for displaying seal status based on integrity.
        /// </summary>
        public string GetStatusColor()
        {
            float percent = GetIntegrityPercent();
            if (percent >= 0.75f) return "green";
            if (percent >= 0.5f) return "yellow";
            if (percent >= 0.25f) return "orange";
            return "red";
        }
    }
}
