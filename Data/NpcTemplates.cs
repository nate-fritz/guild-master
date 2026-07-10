using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GuildMaster.Managers;
using GuildMaster.Models;

namespace GuildMaster.Data
{
    /// <summary>
    /// Serializable NPC definitions including full dialogue trees.
    /// wwwroot/data/npcs.json is the source of truth; NpcTemplateStore parses it
    /// at startup and builds the NPC dictionary for each new/loaded game.
    /// </summary>
    public class NpcTemplate
    {
        public string Key { get; set; } = "";          // dictionary key ("Gaius", "Bandit"...)
        public string Name { get; set; } = "";
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public bool IsHostile { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public int AttackDamage { get; set; }
        public int Defense { get; set; }
        public int DamageCount { get; set; } = 1;
        public int DamageDie { get; set; } = 4;
        public int DamageBonus { get; set; } = 0;
        public int Speed { get; set; } = 2;
        public bool IsBackRow { get; set; }
        public int EnergyRegenPerTurn { get; set; } = 1;
        public int MinGold { get; set; } = 1;
        public int MaxGold { get; set; } = 5;
        public int ExperienceReward { get; set; } = 25;
        public string Role { get; set; } = "Melee";
        public List<string> AbilityNames { get; set; } = new();
        public Dictionary<string, int> LootTable { get; set; } = new();
        public string? PreCombatDialogue { get; set; }
        public string CurrentDialogueNode { get; set; } = "greeting";
        public bool RecruitableAfterDefeat { get; set; }
        public string RecruitClass { get; set; } = "Fighter";
        public string? YieldDialogue { get; set; }
        public string? AcceptDialogue { get; set; }
        public bool IsVendor { get; set; }
        public Dictionary<string, int> ShopInventory { get; set; } = new();
        public Dictionary<string, int> ShopPrices { get; set; } = new();
        public float BuybackMultiplier { get; set; } = 0.5f;
        public Dictionary<string, DialogueNodeTemplate> Dialogue { get; set; } = new();
    }

    public class DialogueNodeTemplate
    {
        public string? Text { get; set; }
        public bool PermanentlyEndsDialogue { get; set; }
        public Dictionary<string, string> PartyInterjections { get; set; } = new();
        public ActionTemplate? Action { get; set; }
        public List<ChoiceTemplate> Choices { get; set; } = new();
    }

    public class ChoiceTemplate
    {
        public string? Text { get; set; }
        public string? Next { get; set; }
        public string? RequiredItem { get; set; }
        public List<string>? RequiredAnyItems { get; set; }
        public string? RequireDiscussedNode { get; set; }
        public string? RequireNotDiscussedNode { get; set; }
        public ActionTemplate? Action { get; set; }
    }

    public class ActionTemplate
    {
        public string Type { get; set; } = "";
        public Dictionary<string, JsonElement> Parameters { get; set; } = new();
    }

    public static class NpcTemplateStore
    {
        private static List<NpcTemplate>? templates;
        public static bool IsLoaded => templates != null;

        private static JsonSerializerOptions JsonOptions => new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static void Load(string json)
        {
            var parsed = JsonSerializer.Deserialize<List<NpcTemplate>>(json, JsonOptions)
                ?? throw new InvalidDataException("npcs.json parsed to null");
            Validate(parsed);
            templates = parsed;
        }

        public static Dictionary<string, NPC> BuildNpcs()
        {
            if (templates == null)
                throw new InvalidOperationException("NPC templates not loaded. NpcTemplateStore.Load must run at startup.");

            var npcs = new Dictionary<string, NPC>();
            foreach (var t in templates)
            {
                var npc = new NPC
                {
                    Name = t.Name,
                    ShortDescription = t.ShortDescription,
                    Description = t.Description,
                    IsHostile = t.IsHostile,
                    Health = t.Health,
                    MaxHealth = t.MaxHealth,
                    Energy = t.Energy,
                    MaxEnergy = t.MaxEnergy,
                    AttackDamage = t.AttackDamage,
                    Defense = t.Defense,
                    DamageCount = t.DamageCount,
                    DamageDie = t.DamageDie,
                    DamageBonus = t.DamageBonus,
                    Speed = t.Speed,
                    IsBackRow = t.IsBackRow,
                    EnergyRegenPerTurn = t.EnergyRegenPerTurn,
                    MinGold = t.MinGold,
                    MaxGold = t.MaxGold,
                    ExperienceReward = t.ExperienceReward,
                    Role = Enum.TryParse<EnemyRole>(t.Role, out var role) ? role : EnemyRole.Melee,
                    AbilityNames = new List<string>(t.AbilityNames),
                    LootTable = new Dictionary<string, int>(t.LootTable),
                    PreCombatDialogue = t.PreCombatDialogue ?? "",
                    CurrentDialogueNode = t.CurrentDialogueNode,
                    RecruitableAfterDefeat = t.RecruitableAfterDefeat,
                    RecruitClass = t.RecruitClass,
                    YieldDialogue = t.YieldDialogue,
                    AcceptDialogue = t.AcceptDialogue,
                    IsVendor = t.IsVendor,
                    ShopInventory = new Dictionary<string, int>(t.ShopInventory),
                    ShopPrices = new Dictionary<string, int>(t.ShopPrices),
                    BuybackMultiplier = t.BuybackMultiplier,
                    Dialogue = t.Dialogue.ToDictionary(kv => kv.Key, kv => BuildNode(kv.Value))
                };
                npcs[t.Key] = npc;
            }
            return npcs;
        }

        private static DialogueNode BuildNode(DialogueNodeTemplate t)
        {
            return new DialogueNode
            {
                Text = t.Text,
                PermanentlyEndsDialogue = t.PermanentlyEndsDialogue,
                PartyInterjections = new Dictionary<string, string>(t.PartyInterjections),
                Action = BuildAction(t.Action),
                Choices = t.Choices.Select(c => new DialogueNode.Choice
                {
                    choiceText = c.Text,
                    nextNodeID = c.Next,
                    RequiredItem = c.RequiredItem,
                    RequiredAnyItems = c.RequiredAnyItems == null ? null : new List<string>(c.RequiredAnyItems),
                    RequireDiscussedNode = c.RequireDiscussedNode,
                    RequireNotDiscussedNode = c.RequireNotDiscussedNode,
                    Action = BuildAction(c.Action)
                }).ToList()
            };
        }

        private static DialogueAction? BuildAction(ActionTemplate? t)
        {
            if (t == null) return null;
            var action = new DialogueAction { Type = t.Type };
            foreach (var kv in t.Parameters)
            {
                action.Parameters[kv.Key] = kv.Value.ValueKind switch
                {
                    JsonValueKind.Number => kv.Value.TryGetInt32(out int i) ? i : kv.Value.GetDouble(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    _ => kv.Value.GetString() ?? ""
                };
            }
            return action;
        }

        private static void Validate(List<NpcTemplate> parsed)
        {
            var errors = new List<string>();
            var keys = new HashSet<string>();

            foreach (var t in parsed)
            {
                if (!keys.Add(t.Key))
                    errors.Add($"Duplicate NPC key \"{t.Key}\"");
                if (string.IsNullOrWhiteSpace(t.Name))
                    errors.Add($"NPC \"{t.Key}\": empty name");

                foreach (var (nodeId, node) in t.Dialogue)
                {
                    foreach (var c in node.Choices)
                    {
                        if (c.Next != null && c.Next != "end" && !t.Dialogue.ContainsKey(c.Next))
                            errors.Add($"NPC \"{t.Key}\" node \"{nodeId}\": choice leads to missing node \"{c.Next}\"");
                    }
                }
                // Note: CurrentDialogueNode may be absent from the tree - several
                // story NPCs get their entry node assigned at runtime by
                // DialogueManager state transitions, so that is not validated here.
            }

            if (errors.Count > 0)
                throw new InvalidDataException("npcs.json failed validation:\n" + string.Join("\n", errors));
        }

        // ------- generation support (tools only) -------

        public static List<NpcTemplate> ToTemplates(Dictionary<string, NPC> npcs)
        {
            return npcs.Select(kv => new NpcTemplate
            {
                Key = kv.Key,
                Name = kv.Value.Name,
                ShortDescription = kv.Value.ShortDescription,
                Description = kv.Value.Description,
                IsHostile = kv.Value.IsHostile,
                Health = kv.Value.Health,
                MaxHealth = kv.Value.MaxHealth,
                Energy = kv.Value.Energy,
                MaxEnergy = kv.Value.MaxEnergy,
                AttackDamage = kv.Value.AttackDamage,
                Defense = kv.Value.Defense,
                DamageCount = kv.Value.DamageCount,
                DamageDie = kv.Value.DamageDie,
                DamageBonus = kv.Value.DamageBonus,
                Speed = kv.Value.Speed,
                IsBackRow = kv.Value.IsBackRow,
                EnergyRegenPerTurn = kv.Value.EnergyRegenPerTurn,
                MinGold = kv.Value.MinGold,
                MaxGold = kv.Value.MaxGold,
                ExperienceReward = kv.Value.ExperienceReward,
                Role = kv.Value.Role.ToString(),
                AbilityNames = new List<string>(kv.Value.AbilityNames),
                LootTable = new Dictionary<string, int>(kv.Value.LootTable),
                PreCombatDialogue = kv.Value.PreCombatDialogue,
                CurrentDialogueNode = kv.Value.CurrentDialogueNode,
                RecruitableAfterDefeat = kv.Value.RecruitableAfterDefeat,
                RecruitClass = kv.Value.RecruitClass,
                YieldDialogue = kv.Value.YieldDialogue,
                AcceptDialogue = kv.Value.AcceptDialogue,
                IsVendor = kv.Value.IsVendor,
                ShopInventory = new Dictionary<string, int>(kv.Value.ShopInventory),
                ShopPrices = new Dictionary<string, int>(kv.Value.ShopPrices),
                BuybackMultiplier = kv.Value.BuybackMultiplier,
                Dialogue = (kv.Value.Dialogue ?? new Dictionary<string, DialogueNode>())
                    .ToDictionary(d => d.Key, d => NodeToTemplate(kv.Key, d.Key, d.Value))
            }).ToList();
        }

        private static DialogueNodeTemplate NodeToTemplate(string npcKey, string nodeId, DialogueNode n)
        {
            return new DialogueNodeTemplate
            {
                Text = n.Text,
                PermanentlyEndsDialogue = n.PermanentlyEndsDialogue,
                PartyInterjections = new Dictionary<string, string>(n.PartyInterjections),
                Action = ActionToTemplate(n.Action),
                Choices = n.Choices.Select(c =>
                {
                    // Lambdas can't serialize; all NPCData availability checks were
                    // converted to RequiredItem/RequiredAnyItems. A non-default lambda
                    // here means someone added a new one - fail loudly.
                    if (!c.IsAvailable(new List<string>()) && c.RequiredItem == null && c.RequiredAnyItems == null)
                        throw new InvalidDataException($"NPC \"{npcKey}\" node \"{nodeId}\": choice \"{c.choiceText}\" has a custom IsAvailable lambda; use RequiredItem/RequiredAnyItems instead");
                    return new ChoiceTemplate
                    {
                        Text = c.choiceText,
                        Next = c.nextNodeID,
                        RequiredItem = c.RequiredItem,
                        RequiredAnyItems = c.RequiredAnyItems,
                        RequireDiscussedNode = c.RequireDiscussedNode,
                        RequireNotDiscussedNode = c.RequireNotDiscussedNode,
                        Action = ActionToTemplate(c.Action)
                    };
                }).ToList()
            };
        }

        private static ActionTemplate? ActionToTemplate(DialogueAction? a)
        {
            if (a == null) return null;
            var t = new ActionTemplate { Type = a.Type };
            foreach (var kv in a.Parameters)
                t.Parameters[kv.Key] = JsonSerializer.SerializeToElement(kv.Value);
            return t;
        }

        public static string ToJson(List<NpcTemplate> npcTemplates)
            => JsonSerializer.Serialize(npcTemplates, JsonOptions);
    }
}
