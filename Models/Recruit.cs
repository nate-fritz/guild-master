using GuildMaster.Data;
using GuildMaster.Models;
using System.Xml.Linq;

public class Recruit : Character
{
    // Removed the string Class property - now using CharacterClass from base Character class
    public int RecruitedDay { get; set; }

    public Recruit(string name, string className, int currentDay, CharacterClass characterClass = null)
    {
        Name = name;
        RecruitedDay = currentDay;

        if (characterClass != null)
        {
            Class = characterClass;
            characterClass.ApplyClassBonuses(this);
        }
        else
        {
            // Legacy fallback - determine class by className string
            switch (className.ToLower())
            {
                case "legionnaire":
                    Class = new Legionnaire();
                    Class.ApplyClassBonuses(this);
                    break;
                case "venator":
                    Class = new Venator();
                    Class.ApplyClassBonuses(this);
                    break;
                case "oracle":
                    Class = new Oracle();
                    Class.ApplyClassBonuses(this);
                    break;
                default:
                    // Default fighter stats - no class object
                    Class = null;
                    MaxHealth = 15;
                    Health = 15;
                    MaxEnergy = 10;
                    Energy = 10;
                    AttackDamage = 2;
                    Defense = 1;
                    Speed = 2;
                    break;
            }
        }

        // Set weapon based on class
        if (Class != null)
        {
            switch (Class.Name)
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
            EquippedWeapon = EquipmentData.GetEquipment("rusty dagger");
        }
    }
}