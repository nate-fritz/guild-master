namespace GuildMaster.Models
{
    public class Equipment
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortName { get; set; }  // Optional short name for partial matching
        public EquipmentSlot Slot { get; set; }
        public int Value { get; set; }

        // Weapon-specific properties
        public int DiceCount { get; set; }
        public int DiceSides { get; set; }
        public int Bonus { get; set; }
        public string DiceString => DiceCount > 0 ? $"{DiceCount}d{DiceSides}+{Bonus}" : "N/A";

        // Stat bonuses (apply to any equipment type)
        public int HealthBonus { get; set; }
        public int EnergyBonus { get; set; }
        public int AttackBonus { get; set; }
        public int DefenseBonus { get; set; }
        public int SpeedBonus { get; set; }

        public Equipment()
        {
            Name = "Empty";
            Description = "No equipment in this slot";
            Slot = EquipmentSlot.Weapon;
            Value = 0;
        }

        public Equipment(string name, EquipmentSlot slot)
        {
            Name = name;
            Slot = slot;
            Description = "";
            Value = 0;
        }

        // Create a copy of this equipment
        public Equipment Clone()
        {
            return new Equipment
            {
                Name = this.Name,
                Description = this.Description,
                ShortName = this.ShortName,
                Slot = this.Slot,
                Value = this.Value,
                DiceCount = this.DiceCount,
                DiceSides = this.DiceSides,
                Bonus = this.Bonus,
                HealthBonus = this.HealthBonus,
                EnergyBonus = this.EnergyBonus,
                AttackBonus = this.AttackBonus,
                DefenseBonus = this.DefenseBonus,
                SpeedBonus = this.SpeedBonus
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public enum EquipmentSlot
    {
        Weapon,
        Armor,
        Helm,
        Ring
    }
}