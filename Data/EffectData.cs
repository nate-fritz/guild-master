using System.Collections.Generic;
using GuildMaster.Models;

namespace GuildMaster.Data
{
    public static class EffectData
    {
        public static Dictionary<string, Effect> InitializeEffects()
        {
            var effects = new Dictionary<string, Effect>();

            // Healing Effects
            effects.Add("lesser_healing", new Effect
            {
                Type = EffectType.Heal,
                DiceCount = 1,
                DiceSides = 6,
                Bonus = 4,
                Description = "Restores 1d6+4 health",
                IsUsableOutsideCombat = true
            });

            // Energy Restoration
            effects.Add("energy_potion", new Effect
            {
                Type = EffectType.RestoreEnergy,
                DiceCount = 1,
                DiceSides = 4,
                Bonus = 2,
                Description = "Restores 1d4+2 energy",
                IsUsableOutsideCombat = true
            });

            // Party-wide Effects
            effects.Add("restoration_scroll", new Effect
            {
                Type = EffectType.PartyRestore,
                DiceCount = 1,
                DiceSides = 3,
                Bonus = 1,
                Description = "Restores 1d3+1 health and energy to all party members",
                IsUsableOutsideCombat = true,
                TargetsParty = true
            });

            return effects;
        }
    }
}