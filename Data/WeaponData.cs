using System.Collections.Generic;
using GuildMaster.Models;

namespace GuildMaster.Data
{
    public class Weapon
    {
        public string Name { get; set; } = "";
        public int DiceCount { get; set; }
        public int DiceSides { get; set; }
        public int Bonus { get; set; }
        public string DiceString => $"{DiceCount}d{DiceSides}+{Bonus}";
        public int Value { get; set; }
    }

    public static class WeaponData
    {
        public static Dictionary<string, Weapon> Weapons = new Dictionary<string, Weapon>
        {
            {"rusty dagger", new Weapon { Name = "Rusty Dagger", DiceCount = 1, DiceSides = 4, Bonus = 2, Value = 5 }},
            {"iron gladius", new Weapon { Name = "Iron Gladius", DiceCount = 1, DiceSides = 6, Bonus = 2, Value = 15 }},
            {"battle axe", new Weapon { Name = "Battle Axe", DiceCount = 1, DiceSides = 8, Bonus = 1, Value = 25 }},
            {"worn gladius", new Weapon { Name = "Worn Gladius", DiceCount = 1, DiceSides = 6, Bonus = 1, Value = 10 }},
            {"hunter's bow", new Weapon { Name = "Hunter's Bow", DiceCount = 1, DiceSides = 4, Bonus = 3, Value = 12 }},
            {"ash staff", new Weapon { Name = "Ash Staff", DiceCount = 1, DiceSides = 4, Bonus = 1, Value = 8 }}
        };

        public static Weapon GetWeapon(string weaponName)
        {
            string key = weaponName.ToLower();
            return Weapons.ContainsKey(key) ? Weapons[key] : Weapons["rusty dagger"];
        }
    }
}