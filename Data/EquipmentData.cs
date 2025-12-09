using System.Collections.Generic;
using GuildMaster.Models;

namespace GuildMaster.Data
{
    public static class EquipmentData
    {
        // Master dictionary of all equipment
        public static Dictionary<string, Equipment> AllEquipment = new Dictionary<string, Equipment>();

        static EquipmentData()
        {
            InitializeWeapons();
            InitializeArmor();
            InitializeHelms();
            InitializeRings();
        }

        private static void InitializeWeapons()
        {
            // Starter Weapons
            AllEquipment.Add("rusty dagger", new Equipment
            {
                Name = "Rusty Dagger",
                Description = "A worn dagger with a rusted blade. Better than nothing.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 4,
                Bonus = 2,
                Value = 5
            });

            AllEquipment.Add("worn gladius", new Equipment
            {
                Name = "Worn Gladius",
                Description = "A legionnaire's short sword, showing signs of many battles.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 6,
                Bonus = 1,
                Value = 10
            });

            AllEquipment.Add("hunter's bow", new Equipment
            {
                Name = "Hunter's Bow",
                Description = "A reliable bow used for hunting game in the forest.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 4,
                Bonus = 3,
                Value = 12
            });

            AllEquipment.Add("ash staff", new Equipment
            {
                Name = "Ash Staff",
                Description = "A simple wooden staff, humming with latent magical energy.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 4,
                Bonus = 1,
                Value = 8
            });

            // Common Weapons (loot drops)
            AllEquipment.Add("iron sword", new Equipment
            {
                Name = "Iron Sword",
                Description = "A well-forged iron sword with a sharp edge.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 6,
                Bonus = 2,
                Value = 15
            });

            AllEquipment.Add("battle axe", new Equipment
            {
                Name = "Battle Axe",
                Description = "A heavy two-handed axe that deals devastating blows.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 8,
                Bonus = 1,
                Value = 25
            });
        }

        private static void InitializeArmor()
        {
            AllEquipment.Add("leather armor", new Equipment
            {
                Name = "Leather Armor",
                Description = "Supple leather armor that provides basic protection.",
                Slot = EquipmentSlot.Armor,
                DefenseBonus = 1,
                Value = 20
            });

            AllEquipment.Add("chainmail", new Equipment
            {
                Name = "Chainmail",
                Description = "Interlocking metal rings form a sturdy protective layer.",
                Slot = EquipmentSlot.Armor,
                DefenseBonus = 2,
                SpeedBonus = -1,  // Heavy armor slows you down
                Value = 50
            });

            AllEquipment.Add("plate armor", new Equipment
            {
                Name = "Plate Armor",
                Description = "Heavy plates of steel offering maximum protection.",
                Slot = EquipmentSlot.Armor,
                DefenseBonus = 3,
                SpeedBonus = -1,
                Value = 100
            });
        }

        private static void InitializeHelms()
        {
            AllEquipment.Add("leather cap", new Equipment
            {
                Name = "Leather Cap",
                Description = "A simple leather cap that protects your head.",
                Slot = EquipmentSlot.Helm,
                DefenseBonus = 1,
                Value = 15
            });

            AllEquipment.Add("iron helm", new Equipment
            {
                Name = "Iron Helm",
                Description = "A sturdy iron helmet with good visibility.",
                Slot = EquipmentSlot.Helm,
                DefenseBonus = 1,
                HealthBonus = 5,
                Value = 40
            });

            AllEquipment.Add("crown of focus", new Equipment
            {
                Name = "Crown of Focus",
                Description = "A mystical circlet that enhances mental clarity.",
                Slot = EquipmentSlot.Helm,
                EnergyBonus = 5,
                Value = 75
            });
        }

        private static void InitializeRings()
        {
            AllEquipment.Add("ring of vitality", new Equipment
            {
                Name = "Ring of Vitality",
                Description = "A simple band that strengthens your life force.",
                Slot = EquipmentSlot.Ring,
                HealthBonus = 10,
                Value = 50
            });

            AllEquipment.Add("ring of swiftness", new Equipment
            {
                Name = "Ring of Swiftness",
                Description = "This ring makes your movements feel lighter and faster.",
                Slot = EquipmentSlot.Ring,
                SpeedBonus = 1,
                Value = 60
            });

            AllEquipment.Add("ring of power", new Equipment
            {
                Name = "Ring of Power",
                Description = "Ancient runes on this ring amplify your strength.",
                Slot = EquipmentSlot.Ring,
                AttackBonus = 2,
                Value = 80
            });

            AllEquipment.Add("amulet", new Equipment
            {
                Name = "Weather-worn Amulet",
                Description = "A tarnished silver amulet, worn by time and elements.",
                Slot = EquipmentSlot.Ring,  // Using ring slot for amulets
                DefenseBonus = 1,
                HealthBonus = 5,
                Value = 30
            });
        }

        // Helper method to get equipment by name (case-insensitive)
        public static Equipment GetEquipment(string equipmentName)
        {
            string key = equipmentName.ToLower();
            if (AllEquipment.ContainsKey(key))
            {
                return AllEquipment[key].Clone();  // Return a copy
            }

            // Return a basic weapon as fallback
            return AllEquipment["rusty dagger"].Clone();
        }

        // Get equipment for a specific slot
        public static List<Equipment> GetEquipmentBySlot(EquipmentSlot slot)
        {
            var items = new List<Equipment>();
            foreach (var kvp in AllEquipment)
            {
                if (kvp.Value.Slot == slot)
                {
                    items.Add(kvp.Value.Clone());
                }
            }
            return items;
        }
    }
}