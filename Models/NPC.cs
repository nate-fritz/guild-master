using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildMaster.Models
{
    public enum EnemyRole
    {
        Melee,
        Ranged,
        Support
    }

    public class NPC : Character
    {
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string CurrentDialogueNode { get; set; } = "greeting";
        public Dictionary<string, DialogueNode> Dialogue { get; set; }
        public bool IsHostile { get; set; } = false;
        public int MinGold { get; set; } = 1;
        public int MaxGold { get; set; } = 5;
        public Dictionary<string, int> LootTable { get; set; } = new Dictionary<string, int>();
        public int ExperienceReward { get; set; } = 25; // Default 25 XP
        public EnemyRole Role { get; set; } = EnemyRole.Melee; // Default to melee
        public List<string> AbilityNames { get; set; } = new List<string>(); // List of ability names this enemy can use

        public NPC()
        {
            Dialogue = new Dictionary<string, DialogueNode>();
        }

        // Clone method to create independent copies
        public NPC Clone()
        {
            var cloned = new NPC
            {
                // Character properties
                Name = this.Name,
                Health = this.Health,
                MaxHealth = this.MaxHealth,
                Energy = this.Energy,
                MaxEnergy = this.MaxEnergy,
                AttackDamage = this.AttackDamage,
                Defense = this.Defense,
                DamageCount = this.DamageCount,
                DamageDie = this.DamageDie,
                DamageBonus = this.DamageBonus,
                Speed = this.Speed,
                IsBackRow = this.IsBackRow,
                EnergyRegenPerTurn = this.EnergyRegenPerTurn,

                // NPC specific properties
                ShortDescription = this.ShortDescription,
                Description = this.Description,
                CurrentDialogueNode = this.CurrentDialogueNode,
                IsHostile = this.IsHostile,
                MinGold = this.MinGold,
                MaxGold = this.MaxGold,
                Role = this.Role,
                AbilityNames = new List<string>(this.AbilityNames),

                // Recruitment properties
                RecruitableAfterDefeat = this.RecruitableAfterDefeat,
                RecruitClass = this.RecruitClass,
                YieldDialogue = this.YieldDialogue,
                AcceptDialogue = this.AcceptDialogue,

                // Clone the loot table (create new dictionary)
                LootTable = new Dictionary<string, int>(this.LootTable),

                // Clone the dialogue (create new dictionary with same references - dialogue nodes can be shared)
                Dialogue = new Dictionary<string, DialogueNode>(this.Dialogue)
            };

            return cloned;
        }
    }
}