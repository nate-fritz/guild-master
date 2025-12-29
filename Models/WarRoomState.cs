using System.Collections.Generic;

namespace GuildMaster.Models
{
    /// <summary>
    /// Represents the complete state of the War Room strategic layer.
    /// </summary>
    public class WarRoomState
    {
        public bool IsActive { get; set; } = false;  // War Room unlocked?
        public int CurrentTurn { get; set; } = 1;
        public int ActionPointsPerTurn { get; set; } = 3;  // How many actions player can take per turn
        public int CurrentActionPoints { get; set; } = 3;

        // Seals
        public List<Seal> Seals { get; set; } = new();

        // Squads
        public List<Squad> Squads { get; set; } = new();

        // Active crises
        public List<CrisisEvent> ActiveCrises { get; set; } = new();

        // Resolved crises (for history/stats)
        public List<CrisisEvent> ResolvedCrises { get; set; } = new();

        // Casualties
        public int TotalCasualties { get; set; } = 0;

        /// <summary>
        /// Starts a new turn.
        /// </summary>
        public void StartNewTurn()
        {
            CurrentTurn++;
            CurrentActionPoints = ActionPointsPerTurn;
        }

        /// <summary>
        /// Spends action points.
        /// </summary>
        public bool SpendActionPoints(int amount)
        {
            if (CurrentActionPoints >= amount)
            {
                CurrentActionPoints -= amount;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if any seal has failed (reached 0 integrity).
        /// </summary>
        public bool HasAnySealFailed()
        {
            foreach (var seal in Seals)
            {
                if (seal.Integrity <= 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the average seal integrity across all seals.
        /// </summary>
        public float GetAverageSealIntegrity()
        {
            if (Seals.Count == 0) return 0f;

            float total = 0f;
            foreach (var seal in Seals)
            {
                total += seal.GetIntegrityPercent();
            }
            return total / Seals.Count;
        }

        /// <summary>
        /// Gets count of unassigned squads.
        /// </summary>
        public int GetUnassignedSquadCount()
        {
            int count = 0;
            foreach (var squad in Squads)
            {
                if (!squad.IsDeployed())
                    count++;
            }
            return count;
        }
    }
}
