using System.Collections.Generic;
using System.Linq;

namespace GuildMaster.Models
{
    /// <summary>
    /// Represents a squad of recruits assigned to defend a seal or handle a crisis.
    /// </summary>
    public class Squad
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public List<Recruit> Members { get; set; } = new();
        public string? AssignedSealId { get; set; }  // Null if not assigned
        public string? AssignedCrisisId { get; set; }  // Null if not assigned
        public int MaxSize { get; set; } = 4;  // Maximum squad members

        /// <summary>
        /// Gets the total combat power of the squad.
        /// </summary>
        public int GetCombatPower()
        {
            int power = 0;
            foreach (var member in Members)
            {
                if (member != null)
                {
                    power += member.AttackDamage;
                    power += member.Defense;
                    power += member.MaxHealth / 5;  // Health contributes to survivability
                    power += member.Speed;
                }
            }
            return power;
        }

        /// <summary>
        /// Gets the average level of squad members.
        /// </summary>
        public int GetAverageLevel()
        {
            if (Members.Count == 0) return 1;
            return (int)Members.Average(m => m.Level);
        }

        /// <summary>
        /// Checks if the squad is at full strength.
        /// </summary>
        public bool IsFull()
        {
            return Members.Count >= MaxSize;
        }

        /// <summary>
        /// Checks if the squad is deployed (assigned to something).
        /// </summary>
        public bool IsDeployed()
        {
            return AssignedSealId != null || AssignedCrisisId != null;
        }

        /// <summary>
        /// Gets a description of squad assignment.
        /// </summary>
        public string GetAssignmentDescription()
        {
            if (AssignedSealId != null)
                return $"Defending seal: {AssignedSealId}";
            if (AssignedCrisisId != null)
                return $"Handling crisis: {AssignedCrisisId}";
            return "Unassigned";
        }

        /// <summary>
        /// Applies casualties to the squad after combat.
        /// </summary>
        public void ApplyCasualties(int casualties)
        {
            // Remove random members based on casualties
            var random = Services.ProgramStatics.Random;
            for (int i = 0; i < casualties && Members.Count > 0; i++)
            {
                int index = random.Next(Members.Count);
                Members.RemoveAt(index);
            }
        }
    }
}
