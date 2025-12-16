namespace GuildMaster.Data
{
    public class Ability
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int EnergyCost { get; set; }
        public int DiceCount { get; set; }
        public int DiceSides { get; set; }
        public int Bonus { get; set; }
        public string DiceString => $"{DiceCount}d{DiceSides}+{Bonus}";
        public AbilityType Type { get; set; }
        public bool IsRanged { get; set; } = false; // Melee by default
        public bool CanTargetBackRow { get; set; } = true; // Can target back row by default (most abilities can)
        public int UnlockLevel { get; set; } = 1; // Level at which this ability is unlocked (default: 1)
    }

    public enum AbilityType
    {
        SingleTarget,
        AreaOfEffect,
        Buff,
        Heal
    }

    public static class AbilityData
    {
        public static Ability Whirlwind = new Ability
        {
            Name = "Whirlwind",
            Description = "Devastating AOE attack that hits all enemies, ignoring row positioning",
            EnergyCost = 24,  // ~90% of level 20 Legionnaire's max EP (27)
            DiceCount = 2,
            DiceSides = 10,
            Bonus = 0,
            Type = AbilityType.AreaOfEffect,
            IsRanged = false,
            UnlockLevel = 20
        };

        // Keep old WhirlwindAttack for backwards compatibility with non-Legionnaire uses
        public static Ability WhirlwindAttack = new Ability
        {
            Name = "Whirlwind Attack",
            Description = "Hit all enemies",
            EnergyCost = 3,
            DiceCount = 1,
            DiceSides = 3,
            Bonus = 1,
            Type = AbilityType.AreaOfEffect
        };

        public static Ability PowerAttack = new Ability
        {
            Name = "Power Attack",
            Description = "Heavy damage to single target",
            EnergyCost = 2,
            DiceCount = 1,
            DiceSides = 6,
            Bonus = 3,
            Type = AbilityType.SingleTarget
        };

        public static Ability BattleCry = new Ability
        {
            Name = "Battle Cry",
            Description = "Boost party attack and defense for 3 turns",
            EnergyCost = 4,
            Type = AbilityType.Buff
        };

        // Legionnaire Abilities
        public static Ability ShieldBash = new Ability
        {
            Name = "Shield Bash",
            Description = "Stun target for 1 turn and deal damage",
            EnergyCost = 3,
            DiceCount = 1,
            DiceSides = 4,
            Bonus = 2,
            Type = AbilityType.SingleTarget,
            IsRanged = false,
            UnlockLevel = 1
        };

        public static Ability BattleCryLegionnaire = new Ability
        {
            Name = "Battle Cry",
            Description = "Taunt all enemies for 2 turns and generate EP equal to 50% of max EP (5 turn cooldown)",
            EnergyCost = 2,
            Type = AbilityType.Buff,
            UnlockLevel = 3
        };

        // Keep old Taunt for backward compatibility
        public static Ability Taunt => BattleCryLegionnaire;

        public static Ability WarCry = new Ability
        {
            Name = "War Cry",
            Description = "Ultimate battle cry that taunts all enemies, generates 75% max EP, and increases all party members' damage by 20% for 3 turns (5 turn cooldown)",
            EnergyCost = 0,  // Free ability
            Type = AbilityType.Buff,
            UnlockLevel = 20
        };

        public static Ability Cleave = new Ability
        {
            Name = "Cleave",
            Description = "Strike up to 3 enemies in front of you",
            EnergyCost = 3,
            DiceCount = 1,
            DiceSides = 4,
            Bonus = 1,
            Type = AbilityType.AreaOfEffect,
            IsRanged = false,
            UnlockLevel = 2
        };

        public static Ability ShieldWall = new Ability
        {
            Name = "Shield Wall",
            Description = "Boost party defense for 3 turns, can't attack during effect (10 turn cooldown)",
            EnergyCost = 0,
            Type = AbilityType.Buff,
            UnlockLevel = 10
        };

        public static Ability RendingStrike = new Ability
        {
            Name = "Rending Strike",
            Description = "Vicious strike that causes bleeding",
            EnergyCost = 3,
            DiceCount = 1,
            DiceSides = 6,
            Bonus = 2,
            Type = AbilityType.SingleTarget,
            IsRanged = false,
            UnlockLevel = 5
        };

        // Venator Abilities
        public static Ability MultiShot = new Ability
        {
            Name = "Multi-Shot",
            Description = "Hit all enemies for reduced damage",
            EnergyCost = 4,
            DiceCount = 1,
            DiceSides = 3,
            Bonus = 1,
            Type = AbilityType.AreaOfEffect,
            IsRanged = true,
            UnlockLevel = 1
        };

        public static Ability PiercingArrow = new Ability
        {
            Name = "Piercing Arrow",
            Description = "Ignores armor completely",
            EnergyCost = 3,
            DiceCount = 1,
            DiceSides = 6,
            Bonus = 2,
            Type = AbilityType.SingleTarget,
            IsRanged = true,
            UnlockLevel = 2
        };

        public static Ability CoveringShot = new Ability
        {
            Name = "Covering Shot",
            Description = "Efficient shot that restores 2 energy",
            EnergyCost = 0,
            DiceCount = 1,
            DiceSides = 3,  // Reduced damage (d3 instead of normal d4+)
            Bonus = 0,      // 50-75% of normal damage
            Type = AbilityType.SingleTarget,
            IsRanged = true,
            UnlockLevel = 3
        };

        public static Ability EvasiveFire = new Ability
        {
            Name = "Evasive Fire",
            Description = "Dodge the next attack and counter with a volley (10 turn cooldown)",
            EnergyCost = 0,
            DiceCount = 1,  // For the counter-attack damage
            DiceSides = 6,
            Bonus = 1,
            Type = AbilityType.Buff,
            IsRanged = true
        };

        // Oracle Abilities
        public static Ability Heal = new Ability
        {
            Name = "Heal",
            Description = "Restore health to target ally",
            EnergyCost = 3,
            DiceCount = 2,
            DiceSides = 4,
            Bonus = 2,
            Type = AbilityType.Heal,
            IsRanged = true,
            UnlockLevel = 1
        };

        public static Ability Lightning = new Ability
        {
            Name = "Lightning Bolt",
            Description = "Magical damage that cannot be blocked",
            EnergyCost = 4,
            DiceCount = 1,
            DiceSides = 8,
            Bonus = 1,
            Type = AbilityType.SingleTarget,
            IsRanged = true,
            UnlockLevel = 2
        };

        public static Ability Blessing = new Ability
        {
            Name = "Blessing",
            Description = "Boost all party members' attack for 4 turns",
            EnergyCost = 5,
            Type = AbilityType.Buff,
            IsRanged = true,
            UnlockLevel = 3
        };

        public static Ability Barrier = new Ability
        {
            Name = "Barrier",
            Description = "Absorb the next 15 damage and heal for 10% of blocked damage (10 turn cooldown)",
            EnergyCost = 0,
            DiceCount = 15,  // Using DiceCount to store absorption amount for now
            DiceSides = 1,   // Will just be 15 * 1 = 15 absorption
            Bonus = 0,
            Type = AbilityType.Buff,
            IsRanged = true,
            UnlockLevel = 10
        };

        public static Ability FlameStrike = new Ability
        {
            Name = "Flame Strike",
            Description = "Strike an enemy with holy flames, burning them over time",
            EnergyCost = 4,
            DiceCount = 1,
            DiceSides = 6,
            Bonus = 2,
            Type = AbilityType.SingleTarget,
            IsRanged = true,
            UnlockLevel = 5
        };

        public static Ability TestStatus = new Ability
        {
            Name = "Test Status",
            Description = "Apply test status to self",
            EnergyCost = 1,
            Type = AbilityType.Buff
        };

        // ============================================
        // LEVEL 5 ABILITIES
        // ============================================

        // Legionnaire Level 5 - Already have Rending Strike

        // Venator Level 5
        public static Ability BarbedArrow = new Ability
        {
            Name = "Barbed Arrow",
            Description = "Arrow with barbed tip causes severe bleeding",
            EnergyCost = 3,
            DiceCount = 1,
            DiceSides = 4,
            Bonus = 2,
            Type = AbilityType.SingleTarget,
            IsRanged = true,
            UnlockLevel = 5
        };

        // Oracle Level 15
        public static Ability Frostbolt = new Ability
        {
            Name = "Frostbolt",
            Description = "Bolt of ice that weakens enemy attacks",
            EnergyCost = 3,
            DiceCount = 1,
            DiceSides = 6,
            Bonus = 3,
            Type = AbilityType.SingleTarget,
            IsRanged = true,
            UnlockLevel = 15
        };

        // ============================================
        // LEVEL 10 ABILITIES
        // ============================================

        // Legionnaire Level 10
        public static Ability SunderArmor = new Ability
        {
            Name = "Sunder Armor",
            Description = "Crushing blow that breaks enemy defenses",
            EnergyCost = 4,
            DiceCount = 1,
            DiceSides = 8,
            Bonus = 2,
            Type = AbilityType.SingleTarget,
            IsRanged = false,
            UnlockLevel = 10
        };

        // Venator Level 10
        public static Ability FrostArrow = new Ability
        {
            Name = "Frost Arrow",
            Description = "Freezing arrow that slows and weakens enemies",
            EnergyCost = 4,
            DiceCount = 1,
            DiceSides = 6,
            Bonus = 3,
            Type = AbilityType.SingleTarget,
            IsRanged = true,
            UnlockLevel = 15
        };

        // Oracle Level 20
        public static Ability Venom = new Ability
        {
            Name = "Venom",
            Description = "Curse that poisons the target over time",
            EnergyCost = 3,
            DiceCount = 0,  // No upfront damage
            DiceSides = 0,
            Bonus = 0,
            Type = AbilityType.SingleTarget,
            IsRanged = true,
            UnlockLevel = 20
        };

        // ============================================
        // LEVEL 15 ABILITIES
        // ============================================

        // Legionnaire Level 15
        public static Ability DevastatingSlam = new Ability
        {
            Name = "Devastating Slam",
            Description = "Massive strike that can stun multiple foes",
            EnergyCost = 5,
            DiceCount = 1,
            DiceSides = 6,
            Bonus = 3,
            Type = AbilityType.AreaOfEffect,
            IsRanged = false,
            UnlockLevel = 15
        };

        // Venator Level 15
        public static Ability ThunderVolley = new Ability
        {
            Name = "Thunder Volley",
            Description = "Lightning-charged volley that can stun all enemies",
            EnergyCost = 6,
            DiceCount = 1,
            DiceSides = 8,
            Bonus = 2,
            Type = AbilityType.AreaOfEffect,
            IsRanged = true,
            UnlockLevel = 20
        };

        // Oracle Level 15
        public static Ability DivineWrath = new Ability
        {
            Name = "Divine Wrath",
            Description = "Holy fire that burns enemies with divine fury",
            EnergyCost = 6,
            DiceCount = 2,
            DiceSides = 6,
            Bonus = 4,
            Type = AbilityType.SingleTarget,
            IsRanged = true,
            UnlockLevel = 15
        };

        // Helper method to get an ability by name
        public static Ability GetAbilityByName(string name)
        {
            return name switch
            {
                // Legionnaire Abilities
                "Shield Bash" => ShieldBash,
                "Taunt" => Taunt,
                "Battle Cry" => BattleCryLegionnaire,
                "Cleave" => Cleave,
                "Shield Wall" => ShieldWall,
                "Rending Strike" => RendingStrike,
                "Sunder Armor" => SunderArmor,
                "Devastating Slam" => DevastatingSlam,
                "Whirlwind" => Whirlwind,
                "War Cry" => WarCry,

                // Venator Abilities
                "Multi-Shot" => MultiShot,
                "Piercing Arrow" => PiercingArrow,
                "Covering Shot" => CoveringShot,
                "Evasive Fire" => EvasiveFire,
                "Barbed Arrow" => BarbedArrow,
                "Frost Arrow" => FrostArrow,
                "Thunder Volley" => ThunderVolley,

                // Oracle Abilities
                "Heal" => Heal,
                "Lightning Bolt" => Lightning,
                "Blessing" => Blessing,
                "Barrier" => Barrier,
                "Flame Strike" => FlameStrike,
                "Frostbolt" => Frostbolt,
                "Venom" => Venom,
                "Divine Wrath" => DivineWrath,

                // Other
                "Whirlwind Attack" => WhirlwindAttack,
                "Power Attack" => PowerAttack,

                _ => null
            };
        }

    }
}