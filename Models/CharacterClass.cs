using GuildMaster.Data;
using System.Collections.Generic;

namespace GuildMaster.Models
{
    public abstract class CharacterClass
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int BaseHealth { get; set; }
        public int BaseEnergy { get; set; }
        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public int BaseSpeed { get; set; }
        public bool PreferBackRow { get; set; }

        public abstract List<Ability> GetClassAbilities();
        public abstract void ApplyClassBonuses(Character character);
    }

    public class Legionnaire : CharacterClass
    {
        public Legionnaire()
        {
            Name = "Legionnaire";
            Description = "Heavily armored warriors who excel at protecting allies and absorbing damage.";
            BaseHealth = 25;
            BaseEnergy = 8;
            BaseAttack = 2;
            BaseDefense = 3;
            BaseSpeed = 1;
            PreferBackRow = false; // Front row tank
        }

        public override List<Ability> GetClassAbilities()
        {
            return new List<Ability>
            {
                AbilityData.ShieldBash,           // Level 1
                AbilityData.Cleave,               // Level 2
                AbilityData.BattleCryLegionnaire, // Level 3 (replaced by War Cry at level 20)
                AbilityData.RendingStrike,        // Level 5
                AbilityData.SunderArmor,          // Level 10
                AbilityData.ShieldWall,           // Level 10
                AbilityData.DevastatingSlam,      // Level 15
                AbilityData.Whirlwind,            // Level 20
                AbilityData.WarCry                // Level 20 (replaces Battle Cry)
            };
        }

        public override void ApplyClassBonuses(Character character)
        {
            character.MaxHealth = BaseHealth;
            character.Health = BaseHealth;
            character.MaxEnergy = BaseEnergy;
            character.Energy = 0; // Legionnaires start combat at 0 EP, generate EP from attacks
            character.AttackDamage = BaseAttack;
            character.Defense = BaseDefense;
            character.Speed = BaseSpeed;
            character.IsBackRow = PreferBackRow;
            character.EquippedWeapon = EquipmentData.GetEquipment("worn gladius");
        }
    }

    public class Venator : CharacterClass
    {
        public Venator()
        {
            Name = "Venator";
            Description = "Swift rangers who strike from a distance and excel at hitting multiple targets.";
            BaseHealth = 18;
            BaseEnergy = 12;
            BaseAttack = 3;
            BaseDefense = 1;
            BaseSpeed = 3;
            PreferBackRow = true; // Ranged DPS
        }

        public override List<Ability> GetClassAbilities()
        {
            return new List<Ability>
            {
                AbilityData.MultiShot,
                AbilityData.PiercingArrow,
                AbilityData.CoveringShot,
                AbilityData.EvasiveFire,
                AbilityData.BarbedArrow,
                AbilityData.FrostArrow,
                AbilityData.ThunderVolley
            };
        }

        public override void ApplyClassBonuses(Character character)
        {
            character.MaxHealth = BaseHealth;
            character.Health = BaseHealth;
            character.MaxEnergy = BaseEnergy;
            character.Energy = BaseEnergy;
            character.AttackDamage = BaseAttack;
            character.Defense = BaseDefense;
            character.Speed = BaseSpeed;
            character.IsBackRow = PreferBackRow;
            character.EquippedWeapon = EquipmentData.GetEquipment("hunter's bow");
        }
    }

    public class Oracle : CharacterClass
    {
        public Oracle()
        {
            Name = "Oracle";
            Description = "Mystical casters who wield healing magic and elemental forces to aid allies.";
            BaseHealth = 15;
            BaseEnergy = 18;
            BaseAttack = 1;
            BaseDefense = 0;
            BaseSpeed = 2;
            PreferBackRow = true; // Caster/support
        }

        public override List<Ability> GetClassAbilities()
        {
            return new List<Ability>
            {
                AbilityData.Heal,
                AbilityData.Lightning,
                AbilityData.Blessing,
                AbilityData.Barrier,
                AbilityData.FlameStrike,
                AbilityData.Frostbolt,
                AbilityData.Venom,
                AbilityData.DivineWrath
            };
        }

        public override void ApplyClassBonuses(Character character)
        {
            character.MaxHealth = BaseHealth;
            character.Health = BaseHealth;
            character.MaxEnergy = BaseEnergy;
            character.Energy = BaseEnergy;
            character.AttackDamage = BaseAttack;
            character.Defense = BaseDefense;
            character.Speed = BaseSpeed;
            character.IsBackRow = PreferBackRow;
            character.EquippedWeapon = EquipmentData.GetEquipment("ash staff");
        }
    }
}