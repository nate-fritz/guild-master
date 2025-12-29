using System;
using GuildMaster.Models;

namespace GuildMaster.Managers
{
    /// <summary>
    /// Handles auto-resolution of squad vs crisis combat.
    /// </summary>
    public class SquadCombatResolver
    {
        private readonly Random random;

        public SquadCombatResolver()
        {
            random = Services.ProgramStatics.Random;
        }

        /// <summary>
        /// Resolves combat between a squad and a crisis.
        /// Returns a combat result with outcome and casualties.
        /// </summary>
        public SquadCombatResult ResolveCombat(Squad squad, CrisisEvent crisis)
        {
            var result = new SquadCombatResult
            {
                SquadName = squad.Name,
                CrisisName = crisis.Name,
                SquadPower = squad.GetCombatPower(),
                CrisisDifficulty = crisis.DifficultyRating
            };

            // Calculate success chance based on power ratio
            float powerRatio = (float)result.SquadPower / crisis.DifficultyRating;
            float baseSuccessChance = Math.Min(0.95f, Math.Max(0.05f, powerRatio * 0.6f));

            // Add randomness
            int roll = random.Next(1, 101);
            result.Success = roll <= (baseSuccessChance * 100);

            // Calculate casualties
            if (result.Success)
            {
                // Victory casualties: fewer if squad was stronger
                if (powerRatio >= 1.5f)
                    result.Casualties = 0;  // Overwhelming victory
                else if (powerRatio >= 1.2f)
                    result.Casualties = random.Next(0, 2);  // Moderate victory
                else
                    result.Casualties = random.Next(1, 3);  // Narrow victory
            }
            else
            {
                // Defeat casualties: more if squad was weaker
                if (powerRatio <= 0.5f)
                    result.Casualties = random.Next(2, squad.Members.Count + 1);  // Crushing defeat
                else if (powerRatio <= 0.8f)
                    result.Casualties = random.Next(1, Math.Max(2, squad.Members.Count));  // Heavy defeat
                else
                    result.Casualties = random.Next(0, 2);  // Narrow defeat
            }

            // Cap casualties at squad size
            result.Casualties = Math.Min(result.Casualties, squad.Members.Count);

            return result;
        }

        /// <summary>
        /// Gets a text description of the combat result.
        /// </summary>
        public string GetCombatDescription(SquadCombatResult result)
        {
            string description = $"{result.SquadName} engaged {result.CrisisName}.\n";

            if (result.Success)
            {
                if (result.Casualties == 0)
                    description += $"[green]Complete victory! No casualties.[/]";
                else if (result.Casualties == 1)
                    description += $"[green]Victory![/] 1 casualty sustained.";
                else
                    description += $"[green]Victory![/] {result.Casualties} casualties sustained.";
            }
            else
            {
                if (result.Casualties == 0)
                    description += $"[orange]Mission failed, but squad retreated safely.[/]";
                else if (result.Casualties == 1)
                    description += $"[red]Mission failed.[/] 1 casualty sustained.";
                else
                    description += $"[red]Mission failed.[/] {result.Casualties} casualties sustained.";
            }

            return description;
        }
    }

    /// <summary>
    /// Result of squad combat.
    /// </summary>
    public class SquadCombatResult
    {
        public string SquadName { get; set; } = "";
        public string CrisisName { get; set; } = "";
        public int SquadPower { get; set; }
        public int CrisisDifficulty { get; set; }
        public bool Success { get; set; }
        public int Casualties { get; set; }
    }
}
