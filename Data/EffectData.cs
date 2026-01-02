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

            // Greater Healing
            effects.Add("greater_healing", new Effect
            {
                Type = EffectType.Heal,
                DiceCount = 2,
                DiceSides = 8,
                Bonus = 8,
                Description = "Restores 2d8+8 health",
                IsUsableOutsideCombat = true
            });

            // Greater Energy
            effects.Add("greater_energy", new Effect
            {
                Type = EffectType.RestoreEnergy,
                DiceCount = 2,
                DiceSides = 6,
                Bonus = 6,
                Description = "Restores 2d6+6 energy",
                IsUsableOutsideCombat = true
            });

            // Elixir of Vigor - restores both health and energy
            effects.Add("elixir_of_vigor", new Effect
            {
                Type = EffectType.PartyRestore,
                DiceCount = 2,
                DiceSides = 6,
                Bonus = 4,
                Description = "Restores 2d6+4 health and energy to the user",
                IsUsableOutsideCombat = true
            });

            // Antidote - cures poison (basic healing for now)
            effects.Add("antidote", new Effect
            {
                Type = EffectType.Heal,
                DiceCount = 1,
                DiceSides = 4,
                Bonus = 2,
                Description = "Cures poison and restores 1d4+2 health",
                IsUsableOutsideCombat = true
            });

            // Scroll of Fireball - combat damage
            effects.Add("scroll_fireball", new Effect
            {
                Type = EffectType.Damage,
                DiceCount = 3,
                DiceSides = 6,
                Bonus = 6,
                Description = "Deals 3d6+6 fire damage to target enemy",
                IsUsableOutsideCombat = false
            });

            // Scroll of Healing - stronger than potion
            effects.Add("scroll_healing", new Effect
            {
                Type = EffectType.Heal,
                DiceCount = 3,
                DiceSides = 8,
                Bonus = 10,
                Description = "Restores 3d8+10 health",
                IsUsableOutsideCombat = true
            });

            // Scroll of Protection - temporary defense buff
            effects.Add("scroll_protection", new Effect
            {
                Type = EffectType.Heal, // Note: Using Heal as placeholder
                DiceCount = 0,
                DiceSides = 0,
                Bonus = 5,
                Description = "Grants +5 defense for the next combat",
                IsUsableOutsideCombat = true
            });

            // Scroll of Haste - temporary speed buff
            effects.Add("scroll_haste", new Effect
            {
                Type = EffectType.Heal, // Note: Using Heal as placeholder
                DiceCount = 0,
                DiceSides = 0,
                Bonus = 10,
                Description = "Grants +10 speed for the next combat",
                IsUsableOutsideCombat = true
            });

            // Teleport Scroll - returns to town
            effects.Add("teleport_scroll", new Effect
            {
                Type = EffectType.Heal, // Note: Using Heal as placeholder
                DiceCount = 0,
                DiceSides = 0,
                Bonus = 0,
                Description = "Teleports you back to Belum Town Square",
                IsUsableOutsideCombat = true
            });

            return effects;
        }
    }
}