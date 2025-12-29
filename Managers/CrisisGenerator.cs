using System;
using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;

namespace GuildMaster.Managers
{
    /// <summary>
    /// Generates crisis events based on game state and progression.
    /// </summary>
    public class CrisisGenerator
    {
        private readonly Random random;
        private int nextCrisisId = 1;

        public CrisisGenerator()
        {
            random = Services.ProgramStatics.Random;
        }

        /// <summary>
        /// Generates a new crisis event.
        /// </summary>
        public CrisisEvent GenerateCrisis(List<Seal> seals, int turnNumber)
        {
            // Select random seal to threaten
            var seal = seals[random.Next(seals.Count)];

            // Select random crisis type
            var crisisTypes = Enum.GetValues(typeof(CrisisType)).Cast<CrisisType>().ToList();
            var type = crisisTypes[random.Next(crisisTypes.Count)];

            // Scale difficulty with turn number
            int baseDifficulty = 40 + (turnNumber * 5);
            int difficulty = baseDifficulty + random.Next(-10, 11);

            var crisis = new CrisisEvent
            {
                Id = $"crisis_{nextCrisisId++}",
                Name = GenerateCrisisName(type),
                Description = GenerateCrisisDescription(type, seal.Name),
                TargetSealId = seal.Id,
                Type = type,
                TurnsRemaining = random.Next(2, 4),  // 2-3 turns
                DifficultyRating = difficulty,
                SealDamage = 15 + random.Next(5, 16)  // 15-30 damage
            };

            return crisis;
        }

        /// <summary>
        /// Generates a descriptive name for a crisis.
        /// </summary>
        private string GenerateCrisisName(CrisisType type)
        {
            return type switch
            {
                CrisisType.DemonIncursion => GetRandomName(new[]
                {
                    "Demon Incursion", "Infernal Assault", "Hellgate Opening",
                    "Demonic Invasion", "Abyssal Breach"
                }),
                CrisisType.CultistRitual => GetRandomName(new[]
                {
                    "Cultist Ritual", "Dark Ceremony", "Forbidden Rite",
                    "Blood Sacrifice", "Summoning Circle"
                }),
                CrisisType.MonsterHorde => GetRandomName(new[]
                {
                    "Monster Horde", "Beast Stampede", "Creature Swarm",
                    "Wild Hunt", "Feral Onslaught"
                }),
                CrisisType.Sabotage => GetRandomName(new[]
                {
                    "Sabotage Attempt", "Internal Betrayal", "Infiltration",
                    "Treachery Plot", "Corruption Within"
                }),
                CrisisType.NaturalDisaster => GetRandomName(new[]
                {
                    "Earthquake", "Violent Storm", "Magical Eruption",
                    "Reality Tear", "Planar Rift"
                }),
                CrisisType.Corruption => GetRandomName(new[]
                {
                    "Spreading Corruption", "Magical Blight", "Shadow Plague",
                    "Void Contamination", "Entropic Decay"
                }),
                _ => "Unknown Crisis"
            };
        }

        /// <summary>
        /// Generates a description for a crisis.
        /// </summary>
        private string GenerateCrisisDescription(CrisisType type, string sealName)
        {
            return type switch
            {
                CrisisType.DemonIncursion => $"Demons are assaulting the {sealName}!",
                CrisisType.CultistRitual => $"Cultists are performing dark rituals near the {sealName}.",
                CrisisType.MonsterHorde => $"A massive horde of monsters advances on the {sealName}.",
                CrisisType.Sabotage => $"Saboteurs threaten to damage the {sealName} from within.",
                CrisisType.NaturalDisaster => $"A catastrophic event threatens the {sealName}.",
                CrisisType.Corruption => $"Corruption spreads toward the {sealName}.",
                _ => $"An unknown threat endangers the {sealName}."
            };
        }

        /// <summary>
        /// Gets a random name from an array.
        /// </summary>
        private string GetRandomName(string[] names)
        {
            return names[random.Next(names.Length)];
        }

        /// <summary>
        /// Determines if a new crisis should be generated this turn.
        /// </summary>
        public bool ShouldGenerateCrisis(int turnNumber, int activeCrisisCount)
        {
            // Higher turn numbers = more likely to generate crises
            int maxCrises = 2 + (turnNumber / 5);
            if (activeCrisisCount >= maxCrises)
                return false;

            // Base 40% chance, increases with turn number
            float chance = 0.4f + (turnNumber * 0.02f);
            chance = Math.Min(0.8f, chance);  // Cap at 80%

            return random.NextDouble() < chance;
        }
    }
}
