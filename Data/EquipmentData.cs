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
                ShortName = "dagger",
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
                ShortName = "gladius",
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
                ShortName = "bow",
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
                ShortName = "staff",
                Description = "A simple wooden staff, humming with latent magical energy.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 4,
                Bonus = 1,
                Value = 8
            });

            // Common Weapons (loot drops)
            AllEquipment.Add("iron gladius", new Equipment
            {
                Name = "Iron Gladius",
                ShortName = "gladius",
                Description = "A well-forged iron gladius with a sharp edge.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 6,
                Bonus = 2,
                Value = 15
            });

            AllEquipment.Add("battle axe", new Equipment
            {
                Name = "Battle Axe",
                ShortName = "axe",
                Description = "A heavy two-handed axe that deals devastating blows.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 8,
                Bonus = 1,
                Value = 25
            });

            AllEquipment.Add("steel gladius", new Equipment
            {
                Name = "Steel Gladius",
                ShortName = "gladius",
                Description = "A superior gladius forged from high-quality steel.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 8,
                Bonus = 2,
                Value = 30
            });

            // Mid-Tier Weapons (Guild Armory)
            AllEquipment.Add("mithril sword", new Equipment
            {
                Name = "Mithril Sword",
                ShortName = "sword",
                Description = "A lightweight blade forged from rare mithril, perfectly balanced.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 2,
                DiceSides = 6,
                Bonus = 3,
                Value = 200
            });

            AllEquipment.Add("elven bow", new Equipment
            {
                Name = "Elven Bow",
                ShortName = "bow",
                Description = "Crafted by elven artisans, this bow is both beautiful and deadly.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 2,
                DiceSides = 4,
                Bonus = 4,
                Value = 180
            });

            AllEquipment.Add("arcane staff", new Equipment
            {
                Name = "Arcane Staff",
                ShortName = "staff",
                Description = "Ancient runes carved into this staff pulse with magical power.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 8,
                Bonus = 4,
                Value = 160
            });

            AllEquipment.Add("war hammer", new Equipment
            {
                Name = "War Hammer",
                ShortName = "hammer",
                Description = "A massive two-handed hammer that crushes armor and bone alike.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 2,
                DiceSides = 8,
                Bonus = 1,
                Value = 220
            });

            // Dungeon Floor 1 Weapons (Levels 1-5, Bronze Age)
            AllEquipment.Add("bronze gladius", new Equipment
            {
                Name = "Bronze Gladius",
                ShortName = "gladius",
                Description = "An ancient bronze blade recovered from the ruins.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 6,
                Bonus = 3,
                Value = 20
            });

            AllEquipment.Add("reinforced bow", new Equipment
            {
                Name = "Reinforced Bow",
                ShortName = "bow",
                Description = "A sturdy bow with bronze reinforcements.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 6,
                Bonus = 2,
                Value = 18
            });

            AllEquipment.Add("bronze staff", new Equipment
            {
                Name = "Bronze Staff",
                ShortName = "staff",
                Description = "A staff topped with a bronze orb that hums with ancient power.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 6,
                Bonus = 1,
                Value = 16
            });

            // Dungeon Floor 2 Weapons (Levels 6-10, Enchanted)
            AllEquipment.Add("enchanted spatha", new Equipment
            {
                Name = "Enchanted Spatha",
                ShortName = "spatha",
                Description = "A longer sword imbued with protective enchantments.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 8,
                Bonus = 4,
                Value = 80
            });

            AllEquipment.Add("stormbow", new Equipment
            {
                Name = "Stormbow",
                ShortName = "bow",
                Description = "This bow crackles with electrical energy when drawn.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 8,
                Bonus = 3,
                Value = 75
            });

            AllEquipment.Add("crystal staff", new Equipment
            {
                Name = "Crystal Staff",
                ShortName = "staff",
                Description = "A staff crowned with a pulsing crystal that amplifies magic.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 1,
                DiceSides = 6,
                Bonus = 5,
                Value = 70
            });

            // Dungeon Floor 3 Weapons (Levels 11-15, Legendary)
            AllEquipment.Add("hero's blade", new Equipment
            {
                Name = "Hero's Blade",
                ShortName = "blade",
                Description = "A legendary sword said to have slain a minotaur.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 2,
                DiceSides = 6,
                Bonus = 4,
                Value = 180
            });

            AllEquipment.Add("gorgon's bane", new Equipment
            {
                Name = "Gorgon's Bane",
                ShortName = "bow",
                Description = "A bow carved from petrified wood, deadly against mythical beasts.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 2,
                DiceSides = 6,
                Bonus = 3,
                Value = 170
            });

            AllEquipment.Add("medusa's wand", new Equipment
            {
                Name = "Medusa's Wand",
                ShortName = "wand",
                Description = "A wand fashioned from a petrified serpent, radiating dark power.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 2,
                DiceSides = 4,
                Bonus = 6,
                Value = 165
            });

            // Dungeon Floor 4 Weapons (Levels 16-20, Divine)
            AllEquipment.Add("titan's maul", new Equipment
            {
                Name = "Titan's Maul",
                ShortName = "maul",
                Description = "A colossal weapon wielded by the titans of old.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 2,
                DiceSides = 10,
                Bonus = 4,
                Value = 300
            });

            AllEquipment.Add("olympian greatbow", new Equipment
            {
                Name = "Olympian Greatbow",
                ShortName = "greatbow",
                Description = "A divine bow blessed by the gods themselves.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 2,
                DiceSides = 8,
                Bonus = 6,
                Value = 280
            });

            AllEquipment.Add("divine scepter", new Equipment
            {
                Name = "Divine Scepter",
                ShortName = "scepter",
                Description = "A scepter that channels the power of the divine realm.",
                Slot = EquipmentSlot.Weapon,
                DiceCount = 3,
                DiceSides = 6,
                Bonus = 3,
                Value = 290
            });
        }

        private static void InitializeArmor()
        {
            AllEquipment.Add("leather armor", new Equipment
            {
                Name = "Leather Armor",
                ShortName = "armor",
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
                ShortName = "armor",
                Description = "Heavy plates of steel offering maximum protection.",
                Slot = EquipmentSlot.Armor,
                DefenseBonus = 3,
                SpeedBonus = -1,
                Value = 100
            });

            // Mid-Tier Armor (Guild Armory)
            AllEquipment.Add("dragon scale armor", new Equipment
            {
                Name = "Dragon Scale Armor",
                ShortName = "armor",
                Description = "Armor crafted from the scales of a dragon, nearly impenetrable.",
                Slot = EquipmentSlot.Armor,
                DefenseBonus = 4,
                HealthBonus = 15,
                Value = 300
            });

            AllEquipment.Add("mithril chainmail", new Equipment
            {
                Name = "Mithril Chainmail",
                ShortName = "chainmail",
                Description = "Lightweight mithril links provide excellent protection without hindering movement.",
                Slot = EquipmentSlot.Armor,
                DefenseBonus = 3,
                SpeedBonus = 1,
                Value = 250
            });

            // Dungeon Floor 1 Armor (Levels 1-5, Bronze Age)
            AllEquipment.Add("bronze breastplate", new Equipment
            {
                Name = "Bronze Breastplate",
                ShortName = "breastplate",
                Description = "An ancient bronze cuirass, still sturdy after centuries.",
                Slot = EquipmentSlot.Armor,
                DefenseBonus = 2,
                Value = 25
            });

            // Dungeon Floor 2 Armor (Levels 6-10, Enchanted)
            AllEquipment.Add("blessed cuirass", new Equipment
            {
                Name = "Blessed Cuirass",
                ShortName = "cuirass",
                Description = "Steel armor blessed by oracles, offering protection and vitality.",
                Slot = EquipmentSlot.Armor,
                DefenseBonus = 3,
                HealthBonus = 10,
                Value = 90
            });

            // Dungeon Floor 3 Armor (Levels 11-15, Legendary)
            AllEquipment.Add("griffon hide armor", new Equipment
            {
                Name = "Griffon Hide Armor",
                ShortName = "armor",
                Description = "Armor crafted from the hide of a slain griffon, incredibly durable.",
                Slot = EquipmentSlot.Armor,
                DefenseBonus = 4,
                HealthBonus = 12,
                SpeedBonus = 1,
                Value = 200
            });

            // Dungeon Floor 4 Armor (Levels 16-20, Divine)
            AllEquipment.Add("aegis of the gods", new Equipment
            {
                Name = "Aegis of the Gods",
                ShortName = "aegis",
                Description = "Divine armor said to have been forged on Mount Olympus itself.",
                Slot = EquipmentSlot.Armor,
                DefenseBonus = 5,
                HealthBonus = 20,
                SpeedBonus = 2,
                Value = 350
            });
        }

        private static void InitializeHelms()
        {
            AllEquipment.Add("leather cap", new Equipment
            {
                Name = "Leather Cap",
                ShortName = "cap",
                Description = "A simple leather cap that protects your head.",
                Slot = EquipmentSlot.Helm,
                DefenseBonus = 1,
                Value = 15
            });

            AllEquipment.Add("iron helm", new Equipment
            {
                Name = "Iron Helm",
                ShortName = "helm",
                Description = "A sturdy iron helmet with good visibility.",
                Slot = EquipmentSlot.Helm,
                DefenseBonus = 1,
                HealthBonus = 5,
                Value = 40
            });

            AllEquipment.Add("crown of focus", new Equipment
            {
                Name = "Crown of Focus",
                ShortName = "crown",
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
                ShortName = "ring",
                Description = "A simple band that strengthens your life force.",
                Slot = EquipmentSlot.Ring,
                HealthBonus = 10,
                Value = 50
            });

            AllEquipment.Add("ring of swiftness", new Equipment
            {
                Name = "Ring of Swiftness",
                ShortName = "ring",
                Description = "This ring makes your movements feel lighter and faster.",
                Slot = EquipmentSlot.Ring,
                SpeedBonus = 1,
                Value = 60
            });

            AllEquipment.Add("ring of power", new Equipment
            {
                Name = "Ring of Power",
                ShortName = "ring",
                Description = "Ancient runes on this ring amplify your strength.",
                Slot = EquipmentSlot.Ring,
                AttackBonus = 2,
                Value = 80
            });

            // Class-Specific Rings (Guild Treasury)
            AllEquipment.Add("legionnaire's ring", new Equipment
            {
                Name = "Legionnaire's Ring",
                ShortName = "ring",
                Description = "A ring bearing the sigil of the ancient legions. It bolsters strength and fortitude.",
                Slot = EquipmentSlot.Ring,
                HealthBonus = 20,
                AttackBonus = 2,
                DefenseBonus = 1,
                Value = 150
            });

            AllEquipment.Add("venator's ring", new Equipment
            {
                Name = "Venator's Ring",
                ShortName = "ring",
                Description = "Worn by master hunters, this ring enhances precision and agility.",
                Slot = EquipmentSlot.Ring,
                SpeedBonus = 2,
                AttackBonus = 3,
                Value = 150
            });

            AllEquipment.Add("oracle's ring", new Equipment
            {
                Name = "Oracle's Ring",
                ShortName = "ring",
                Description = "A mystical ring that channels arcane energies, favored by oracles and mages.",
                Slot = EquipmentSlot.Ring,
                EnergyBonus = 15,
                AttackBonus = 2,
                HealthBonus = 10,
                Value = 150
            });

            // Note: The "amulet" from the Silvacis quest is NOT equipment - it's a quest item only
            // It should remain in ItemData.cs but not here, so it cannot be equipped
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