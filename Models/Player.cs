using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuildMaster.Data;

namespace GuildMaster.Models
{
    public class Player : Character
    {
        public List<string> Inventory { get; set; }
        public HashSet<string> TakenItems { get; set; }
        public HashSet<string> ExaminedItems { get; set; }
        public int Gold { get; set; }
        public List<Recruit> Recruits { get; set; }
        public List<Recruit> ActiveParty { get; set; }
        public float CurrentHour { get; set; }
        public int CurrentDay { get; set; }
        public int CurrentRoom { get; set; }
        public List<Quest> ActiveQuests { get; set; }
        public bool AutoCombatEnabled { get; set; } = false;

        public Player(string name = "Adventurer", CharacterClass characterClass = null)
        {
            Name = name;

            // Apply class or use defaults
            if (characterClass != null)
            {
                Class = characterClass;
                characterClass.ApplyClassBonuses(this);
            }
            else
            {
                // Default stats if no class specified (for legacy saves)
                MaxHealth = 20;
                Health = 20;
                MaxEnergy = 10;
                Energy = 10;
                AttackDamage = 1;
                Defense = 0;
                Speed = 1;
            }

            if (characterClass != null)
            {
                switch (characterClass.Name)
                {
                    case "Legionnaire":
                        EquippedWeapon = EquipmentData.GetEquipment("worn gladius");
                        break;
                    case "Venator":
                        EquippedWeapon = EquipmentData.GetEquipment("hunter's bow");
                        break;
                    case "Oracle":
                        EquippedWeapon = EquipmentData.GetEquipment("ash staff");
                        break;
                    default:
                        EquippedWeapon = EquipmentData.GetEquipment("rusty dagger");
                        break;
                }
            }
            else
            {
                EquippedWeapon = EquipmentData.GetEquipment("rusty dagger"); // Legacy fallback
            }

            Inventory = new List<string>();
            TakenItems = new HashSet<string>();
            ExaminedItems = new HashSet<string>();
            Recruits = new List<Recruit>();
            ActiveParty = new List<Recruit>();
            Gold = 0;
            CurrentHour = 8.0f;
            CurrentDay = 1;
            CurrentRoom = 1;

            ActiveQuests = new List<Quest>();
        }
    }
}