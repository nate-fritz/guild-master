using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildMaster.Models
{
    public class Character
    {
        // Descriptive Attributes
        public string Name { get; set; }
        public CharacterClass Class { get; set; }

        // Physical Attributes
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }

        // Combat Attributes
        public int AttackDamage { get; set; }
        public int Defense { get; set; }
        public int DamageCount { get; set; } = 1;
        public int DamageDie { get; set; } = 4;
        public int DamageBonus { get; set; } = 0;
        public int Speed { get; set; } = 2;
        public bool IsBackRow { get; set; } = false;
        public int EnergyRegenPerTurn { get; set; } = 1;

        // Statuses
        public bool IsAlive => Health > 0;
        public Dictionary<string, int> ActiveStatusEffects { get; set; } = new Dictionary<string, int>();

        // Recruitment Properties
        public bool RecruitableAfterDefeat { get; set; } = false;
        public string RecruitClass { get; set; } = "Fighter";
        public string YieldDialogue { get; set; }
        public string AcceptDialogue { get; set; }

        // Leveling Properties
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public int ExperienceToNextLevel { get; set; } = 100;

        // Quest Properties
        public bool IsOnQuest { get; set; }
        public bool IsResting { get; set; }
        public float RestUntil { get; set; }  // Game time when rest ends
        public int RestUntilDay { get; set; }

        // Equipment Properties
        public Equipment EquippedWeapon { get; set; }
        public Equipment EquippedArmor { get; set; }
        public Equipment EquippedHelm { get; set; }
        public Equipment EquippedRing { get; set; }

        // Calculate total stats including equipment bonuses
        public int TotalAttack => AttackDamage + GetEquipmentBonus("attack");
        public int TotalDefense => Defense + GetEquipmentBonus("defense");
        public int TotalSpeed => Speed + GetEquipmentBonus("speed");
        public int TotalMaxHealth => MaxHealth + GetEquipmentBonus("health");
        public int TotalMaxEnergy => MaxEnergy + GetEquipmentBonus("energy");

        // Damage Over Time tracking
        public List<DamageOverTime> ActiveDOTs { get; set; }

        // Actions
        public void TakeDamage(int damage)
        {
            int actualDamage = Math.Max(1, damage - Defense);
            Health = Math.Max(0, Health - actualDamage);
        }

        public void Heal(int amount)
        {
            Health = Math.Min(TotalMaxHealth, Health + amount);
        }

        public void RestoreEnergy(int amount)
        {
            Energy = Math.Min(TotalMaxEnergy, Energy + amount);
        }

        public void FullRestore()
        {
            Health = TotalMaxHealth;

            // Check class-specific EP rest behavior
            if (Class != null && Class.ResetEpOnRest)
            {
                // Reset EP to 0 (for classes like Legionnaire)
                Energy = 0;
            }
            else
            {
                // Restore EP to max (for classes like Venator and Oracle)
                Energy = TotalMaxEnergy;
            }
        }

        public int GetXPForNextLevel(int level)
        {
            // XP curve: 100 * level (so level 2 needs 200, level 3 needs 300, etc.)
            return 100 * level;
        }

        public bool CheckLevelUp()
        {
            if (Experience >= ExperienceToNextLevel)
            {
                Level++;
                Experience -= ExperienceToNextLevel;
                ExperienceToNextLevel = GetXPForNextLevel(Level);
                return true;
            }
            return false;
        }

        public void ApplyLevelUpBonuses()
        {
            if (Class == null) return;

            switch (Class.Name)
            {
                case "Legionnaire":
                    MaxHealth += 5;
                    Health += 5; // Also heal by the amount gained
                    MaxEnergy += 1;
                    Energy += 1;

                    // +1 Attack every even level
                    if (Level % 2 == 0)
                        AttackDamage += 1;

                    // +1 Defense every 2 levels
                    if (Level % 2 == 0)
                        Defense += 1;
                    break;

                case "Venator":
                    MaxHealth += 3;
                    Health += 3;
                    MaxEnergy += 2;
                    Energy += 2;
                    AttackDamage += 1; // Every level

                    // +1 Defense every 3 levels
                    if (Level % 3 == 0)
                        Defense += 1;

                    // +1 Speed every 5 levels
                    if (Level % 5 == 0)
                        Speed += 1;
                    break;

                case "Oracle":
                    MaxHealth += 2;
                    Health += 2;
                    MaxEnergy += 3;
                    Energy += 3;

                    // +1 Attack every 2 levels
                    if (Level % 2 == 0)
                        AttackDamage += 1;

                    // +1 Defense every 3 levels
                    if (Level % 3 == 0)
                        Defense += 1;
                    break;
            }
        }

        // Get total bonus from all equipped items for a specific stat
        public int GetEquipmentBonus(string stat)
        {
            int bonus = 0;

            switch (stat.ToLower())
            {
                case "attack":
                    if (EquippedWeapon != null) bonus += EquippedWeapon.AttackBonus;
                    if (EquippedArmor != null) bonus += EquippedArmor.AttackBonus;
                    if (EquippedHelm != null) bonus += EquippedHelm.AttackBonus;
                    if (EquippedRing != null) bonus += EquippedRing.AttackBonus;
                    break;
                case "defense":
                    if (EquippedWeapon != null) bonus += EquippedWeapon.DefenseBonus;
                    if (EquippedArmor != null) bonus += EquippedArmor.DefenseBonus;
                    if (EquippedHelm != null) bonus += EquippedHelm.DefenseBonus;
                    if (EquippedRing != null) bonus += EquippedRing.DefenseBonus;
                    break;
                case "speed":
                    if (EquippedWeapon != null) bonus += EquippedWeapon.SpeedBonus;
                    if (EquippedArmor != null) bonus += EquippedArmor.SpeedBonus;
                    if (EquippedHelm != null) bonus += EquippedHelm.SpeedBonus;
                    if (EquippedRing != null) bonus += EquippedRing.SpeedBonus;
                    break;
                case "health":
                    if (EquippedWeapon != null) bonus += EquippedWeapon.HealthBonus;
                    if (EquippedArmor != null) bonus += EquippedArmor.HealthBonus;
                    if (EquippedHelm != null) bonus += EquippedHelm.HealthBonus;
                    if (EquippedRing != null) bonus += EquippedRing.HealthBonus;
                    break;
                case "energy":
                    if (EquippedWeapon != null) bonus += EquippedWeapon.EnergyBonus;
                    if (EquippedArmor != null) bonus += EquippedArmor.EnergyBonus;
                    if (EquippedHelm != null) bonus += EquippedHelm.EnergyBonus;
                    if (EquippedRing != null) bonus += EquippedRing.EnergyBonus;
                    break;
            }

            return bonus;
        }

        // Equip an item in the appropriate slot
        public Equipment EquipItem(Equipment newEquipment)
        {
            Equipment oldEquipment = null;

            switch (newEquipment.Slot)
            {
                case EquipmentSlot.Weapon:
                    oldEquipment = EquippedWeapon;
                    EquippedWeapon = newEquipment;
                    break;
                case EquipmentSlot.Armor:
                    oldEquipment = EquippedArmor;
                    EquippedArmor = newEquipment;
                    break;
                case EquipmentSlot.Helm:
                    oldEquipment = EquippedHelm;
                    EquippedHelm = newEquipment;
                    break;
                case EquipmentSlot.Ring:
                    oldEquipment = EquippedRing;
                    EquippedRing = newEquipment;
                    break;
            }

            return oldEquipment;  // Return the old equipment (could be null)
        }

        // Get equipment in a specific slot
        public Equipment GetEquipmentInSlot(EquipmentSlot slot)
        {
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    return EquippedWeapon;
                case EquipmentSlot.Armor:
                    return EquippedArmor;
                case EquipmentSlot.Helm:
                    return EquippedHelm;
                case EquipmentSlot.Ring:
                    return EquippedRing;
                default:
                    return null;
            }
        }

        // Apply a DOT effect
        public void ApplyDOT(DamageOverTime dot)
        {
            if (ActiveDOTs == null)
                ActiveDOTs = new List<DamageOverTime>();

            // Check if this DOT type already exists
            var existing = ActiveDOTs.FirstOrDefault(d => d.Type == dot.Type);

            if (existing != null)
            {
                // Refresh duration and update damage if new DOT is stronger
                existing.RemainingTurns = Math.Max(existing.RemainingTurns, dot.RemainingTurns);
                existing.DamagePerTurn = Math.Max(existing.DamagePerTurn, dot.DamagePerTurn);
            }
            else
            {
                // Add new DOT
                ActiveDOTs.Add(dot);
            }
        }

        // Process all DOTs and return total damage taken
        public int ProcessDOTs()
        {
            if (ActiveDOTs == null)
                ActiveDOTs = new List<DamageOverTime>();

            int totalDamage = 0;
            var dotsToRemove = new List<DamageOverTime>();

            foreach (var dot in ActiveDOTs)
            {
                totalDamage += dot.DamagePerTurn;
                dot.RemainingTurns--;

                if (dot.RemainingTurns <= 0)
                {
                    dotsToRemove.Add(dot);
                }
            }

            // Remove expired DOTs
            foreach (var dot in dotsToRemove)
            {
                ActiveDOTs.Remove(dot);
            }

            return totalDamage;
        }

        // Get active DOT names for display
        public List<string> GetActiveDOTNames()
        {
            var names = new List<string>();

            if (ActiveDOTs == null)
                return names;

            foreach (var dot in ActiveDOTs)
            {
                switch (dot.Type)
                {
                    case DamageType.Bleed:
                        names.Add("🩸 Bleeding");
                        break;
                    case DamageType.Fire:
                        names.Add("🔥 Burning");
                        break;
                    case DamageType.Poison:
                        names.Add("☠️ Poisoned");
                        break;
                    case DamageType.Ice:
                        names.Add("❄️ Frozen");
                        break;
                }
            }
            return names;
        }

        // Check if character has a specific DOT
        public bool HasDOT(DamageType type)
        {
            if (ActiveDOTs == null)
                return false;

            return ActiveDOTs.Any(d => d.Type == type);
        }

    }
}