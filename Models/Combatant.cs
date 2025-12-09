using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildMaster.Models
{
    public class Combatant
    {
        public Character Character { get; set; }
        public int InitiativeRoll { get; set; }
        public bool IsPlayer { get; set; }

        public Combatant(Character character, int initiativeRoll, bool isPlayer = false)
        {
            Character = character;
            InitiativeRoll = initiativeRoll;
            IsPlayer = isPlayer;
        }

        public string Name => Character.Name;

        public bool IsAlive => Character.Health > 0;
    }
}
