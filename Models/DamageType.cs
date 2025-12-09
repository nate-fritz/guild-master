using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildMaster.Models
{
    public enum DamageType
    {
        Physical,    // Normal weapon damage (no special effect)

        // Physical Variants (for martial classes)
        Bleed,       // Physical DOT - bleeding wounds
        Crush,       // Reduces defense - armor breaking
        Concussive,  // Stun chance - blunt trauma

        // Magical Types (for casters)
        Fire,        // Magical burn DOT
        Ice,         // Reduces defense - freezing
        Lightning,   // Stun chance - electrical
        Poison       // Magical strong DOT
    }
}
