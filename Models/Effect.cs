using System;

namespace GuildMaster.Models
{
    public class Effect
    {
        public EffectType Type { get; set; }
        public int DiceCount { get; set; }
        public int DiceSides { get; set; }
        public int Bonus { get; set; }
        public string Description { get; set; }
        public bool IsUsableOutsideCombat { get; set; } = false;
        public bool TargetsParty { get; set; } = false;
        public int Duration { get; set; } = 0;  // In hours, for future buffs
    }

    public enum EffectType
    {
        Heal,
        RestoreEnergy,
        PartyRestore,
        BuffAttack,    // For future scrolls
        BuffDefense    // For future scrolls
    }
}