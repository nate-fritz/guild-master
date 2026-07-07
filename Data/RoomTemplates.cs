using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using GuildMaster.Models;

namespace GuildMaster.Data
{
    /// <summary>
    /// Serializable room definitions. rooms.json (wwwroot/data/rooms.json) is the
    /// source of truth for room content; RoomTemplateStore parses it once at app
    /// startup and builds fresh Room instances for each new/loaded game.
    /// </summary>
    public class RoomTemplate
    {
        public int NumericId { get; set; }
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public Dictionary<string, int> Exits { get; set; } = new();
        public List<string> Items { get; set; } = new();
        public List<RoomNpcRef> Npcs { get; set; } = new();
        public List<RoomNpcRef> OriginalNpcs { get; set; } = new();
        public Dictionary<string, string> DescriptionVariants { get; set; } = new();
        public List<RoomObjectTemplate> Objects { get; set; } = new();
        public string? PuzzleId { get; set; }
        public bool CanRespawn { get; set; }
        public float RespawnTimeHours { get; set; } = 48f;
    }

    /// <summary>
    /// How a room references an NPC:
    /// - "clone": a fresh copy of the named NPC from NPCData (npcs[name].Clone())
    /// - "shared": the single shared instance from NPCData (dialogue state persists; unique story NPCs)
    /// - "inline": a one-off NPC defined entirely in this file (stats in Inline)
    /// </summary>
    public class RoomNpcRef
    {
        public string Mode { get; set; } = "clone";
        public string? Name { get; set; }
        public InlineNpc? Inline { get; set; }
    }

    public class InlineNpc
    {
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
    }

    public class RoomObjectTemplate
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string[] Aliases { get; set; } = Array.Empty<string>();
        public string? DefaultDescription { get; set; }
        public string? LookedAtDescription { get; set; }
        public bool IsInteractable { get; set; }
        public bool IsHidden { get; set; }
        public string? InteractionType { get; set; }
        public string? RequiredItem { get; set; }
        public string? OnInteractMessage { get; set; }
        public string? OnFailMessage { get; set; }
    }

    public static class RoomTemplateStore
    {
        private static List<RoomTemplate>? templates;

        public static bool IsLoaded => templates != null;

        private static JsonSerializerOptions JsonOptions => new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>Parses rooms.json content and validates room-to-room references. Call once at startup.</summary>
        public static void Load(string json)
        {
            var parsed = JsonSerializer.Deserialize<List<RoomTemplate>>(json, JsonOptions)
                ?? throw new InvalidDataException("rooms.json parsed to null");
            ValidateStructure(parsed);
            templates = parsed;
        }

        /// <summary>Builds fresh Room instances for a new game session, resolving NPC references.</summary>
        public static Dictionary<int, Room> BuildRooms(Dictionary<string, NPC> npcs)
        {
            if (templates == null)
                throw new InvalidOperationException("Room templates not loaded. RoomTemplateStore.Load must run at startup before any game is created.");

            var errors = new List<string>();
            var rooms = new Dictionary<int, Room>();

            foreach (var t in templates)
            {
                var room = new Room
                {
                    NumericId = t.NumericId,
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Exits = new Dictionary<string, int>(t.Exits),
                    Items = new List<string>(t.Items),
                    DescriptionVariants = new Dictionary<string, string>(t.DescriptionVariants),
                    PuzzleId = t.PuzzleId,
                    CanRespawn = t.CanRespawn,
                    RespawnTimeHours = t.RespawnTimeHours
                };

                foreach (var o in t.Objects)
                {
                    room.Objects.Add(new RoomObject
                    {
                        Id = o.Id,
                        Name = o.Name,
                        Aliases = (string[])o.Aliases.Clone(),
                        DefaultDescription = o.DefaultDescription,
                        LookedAtDescription = o.LookedAtDescription,
                        IsInteractable = o.IsInteractable,
                        IsHidden = o.IsHidden,
                        InteractionType = o.InteractionType,
                        RequiredItem = o.RequiredItem,
                        OnInteractMessage = o.OnInteractMessage,
                        OnFailMessage = o.OnFailMessage
                    });
                }

                foreach (var npcRef in t.Npcs)
                {
                    var npc = ResolveNpc(npcRef, npcs, t.NumericId, errors);
                    if (npc != null) room.NPCs.Add(npc);
                }
                foreach (var npcRef in t.OriginalNpcs)
                {
                    var npc = ResolveNpc(npcRef, npcs, t.NumericId, errors);
                    if (npc != null) room.OriginalNPCs.Add(npc);
                }

                rooms[t.NumericId] = room;
            }

            if (errors.Count > 0)
                throw new InvalidDataException("Room content errors:\n" + string.Join("\n", errors));

            return rooms;
        }

        private static NPC? ResolveNpc(RoomNpcRef npcRef, Dictionary<string, NPC> npcs, int roomId, List<string> errors)
        {
            switch (npcRef.Mode)
            {
                case "clone":
                case "shared":
                    if (npcRef.Name == null || !npcs.TryGetValue(npcRef.Name, out var source))
                    {
                        errors.Add($"Room {roomId}: unknown NPC \"{npcRef.Name}\" (mode {npcRef.Mode})");
                        return null;
                    }
                    return npcRef.Mode == "clone" ? source.Clone() : source;

                case "inline":
                    if (npcRef.Inline == null)
                    {
                        errors.Add($"Room {roomId}: inline NPC ref missing \"inline\" body");
                        return null;
                    }
                    return BuildInlineNpc(npcRef.Inline);

                default:
                    errors.Add($"Room {roomId}: unknown NPC mode \"{npcRef.Mode}\"");
                    return null;
            }
        }

        private static NPC BuildInlineNpc(InlineNpc i)
        {
            return new NPC
            {
                Name = i.Name,
                ShortDescription = i.ShortDescription,
                Description = i.Description,
                IsHostile = i.IsHostile,
                Health = i.Health,
                MaxHealth = i.MaxHealth,
                Energy = i.Energy,
                MaxEnergy = i.MaxEnergy,
                AttackDamage = i.AttackDamage,
                Defense = i.Defense,
                DamageCount = i.DamageCount,
                DamageDie = i.DamageDie,
                DamageBonus = i.DamageBonus,
                Speed = i.Speed,
                IsBackRow = i.IsBackRow,
                EnergyRegenPerTurn = i.EnergyRegenPerTurn,
                MinGold = i.MinGold,
                MaxGold = i.MaxGold,
                ExperienceReward = i.ExperienceReward,
                Role = Enum.TryParse<EnemyRole>(i.Role, out var role) ? role : EnemyRole.Melee,
                AbilityNames = new List<string>(i.AbilityNames),
                LootTable = new Dictionary<string, int>(i.LootTable),
                PreCombatDialogue = i.PreCombatDialogue ?? "",
                CurrentDialogueNode = i.CurrentDialogueNode,
                RecruitableAfterDefeat = i.RecruitableAfterDefeat,
                RecruitClass = i.RecruitClass,
                YieldDialogue = i.YieldDialogue,
                AcceptDialogue = i.AcceptDialogue,
                Dialogue = new Dictionary<string, DialogueNode>()
            };
        }

        /// <summary>Structural validation that needs no NPC dictionary: ids and exit targets.</summary>
        private static void ValidateStructure(List<RoomTemplate> parsed)
        {
            var errors = new List<string>();
            var ids = new HashSet<int>();

            foreach (var t in parsed)
            {
                if (!ids.Add(t.NumericId))
                    errors.Add($"Duplicate room id {t.NumericId} (\"{t.Title}\")");
                if (string.IsNullOrWhiteSpace(t.Title))
                    errors.Add($"Room {t.NumericId}: empty title");
            }

            foreach (var t in parsed)
            {
                foreach (var exit in t.Exits)
                {
                    if (!ids.Contains(exit.Value))
                        errors.Add($"Room {t.NumericId} (\"{t.Title}\"): exit \"{exit.Key}\" points to missing room {exit.Value}");
                }
            }

            if (errors.Count > 0)
                throw new InvalidDataException("rooms.json failed validation:\n" + string.Join("\n", errors));
        }

        // ---------------------------------------------------------------
        // Generation support (used by tools/RoomJsonGenerator, not the game)
        // ---------------------------------------------------------------

        /// <summary>Converts legacy code-built rooms into templates, classifying each NPC as shared/clone/inline.</summary>
        public static List<RoomTemplate> ToTemplates(Dictionary<int, Room> rooms, Dictionary<string, NPC> npcs)
        {
            var result = new List<RoomTemplate>();
            foreach (var room in rooms.Values.OrderBy(r => r.NumericId))
            {
                var t = new RoomTemplate
                {
                    NumericId = room.NumericId,
                    Id = room.Id,
                    Title = room.Title,
                    Description = room.Description,
                    Exits = new Dictionary<string, int>(room.Exits),
                    Items = new List<string>(room.Items),
                    DescriptionVariants = new Dictionary<string, string>(room.DescriptionVariants),
                    PuzzleId = room.PuzzleId,
                    CanRespawn = room.CanRespawn,
                    RespawnTimeHours = room.RespawnTimeHours
                };

                foreach (var o in room.Objects)
                {
                    if (o.State.Count > 0 || o.HasBeenExamined)
                        throw new InvalidDataException($"Room {room.NumericId} object \"{o.Id}\" has runtime state at init; template schema doesn't carry it");
                    t.Objects.Add(new RoomObjectTemplate
                    {
                        Id = o.Id,
                        Name = o.Name,
                        Aliases = (string[])o.Aliases.Clone(),
                        DefaultDescription = o.DefaultDescription,
                        LookedAtDescription = o.LookedAtDescription,
                        IsInteractable = o.IsInteractable,
                        IsHidden = o.IsHidden,
                        InteractionType = o.InteractionType,
                        RequiredItem = o.RequiredItem,
                        OnInteractMessage = o.OnInteractMessage,
                        OnFailMessage = o.OnFailMessage
                    });
                }

                foreach (var npc in room.NPCs)
                    t.Npcs.Add(ClassifyNpc(npc, npcs, room.NumericId));
                foreach (var npc in room.OriginalNPCs)
                    t.OriginalNpcs.Add(ClassifyNpc(npc, npcs, room.NumericId));

                result.Add(t);
            }
            return result;
        }

        public static string ToJson(List<RoomTemplate> roomTemplates)
            => JsonSerializer.Serialize(roomTemplates, JsonOptions);

        private static RoomNpcRef ClassifyNpc(NPC npc, Dictionary<string, NPC> npcs, int roomId)
        {
            // Shared: the exact instance from the NPC dictionary (unique story NPCs)
            var sharedKey = npcs.FirstOrDefault(kv => ReferenceEquals(kv.Value, npc)).Key;
            if (sharedKey != null)
                return new RoomNpcRef { Mode = "shared", Name = sharedKey };

            // Clone: field-identical to a fresh clone of a dictionary NPC
            foreach (var kv in npcs)
            {
                if (kv.Value.Name == npc.Name && NpcMatchesClone(npc, kv.Value.Clone()))
                    return new RoomNpcRef { Mode = "clone", Name = kv.Key };
            }

            // Inline: defined ad hoc in RoomData (or mutated after cloning)
            if (npc.Dialogue != null && npc.Dialogue.Count > 0)
                throw new InvalidDataException($"Room {roomId}: NPC \"{npc.Name}\" needs inline serialization but has dialogue - schema can't carry dialogue inline");

            return new RoomNpcRef
            {
                Mode = "inline",
                Name = npc.Name,
                Inline = new InlineNpc
                {
                    Name = npc.Name,
                    ShortDescription = npc.ShortDescription,
                    Description = npc.Description,
                    IsHostile = npc.IsHostile,
                    Health = npc.Health,
                    MaxHealth = npc.MaxHealth,
                    Energy = npc.Energy,
                    MaxEnergy = npc.MaxEnergy,
                    AttackDamage = npc.AttackDamage,
                    Defense = npc.Defense,
                    DamageCount = npc.DamageCount,
                    DamageDie = npc.DamageDie,
                    DamageBonus = npc.DamageBonus,
                    Speed = npc.Speed,
                    IsBackRow = npc.IsBackRow,
                    EnergyRegenPerTurn = npc.EnergyRegenPerTurn,
                    MinGold = npc.MinGold,
                    MaxGold = npc.MaxGold,
                    ExperienceReward = npc.ExperienceReward,
                    Role = npc.Role.ToString(),
                    AbilityNames = new List<string>(npc.AbilityNames),
                    LootTable = new Dictionary<string, int>(npc.LootTable),
                    PreCombatDialogue = npc.PreCombatDialogue,
                    CurrentDialogueNode = npc.CurrentDialogueNode,
                    RecruitableAfterDefeat = npc.RecruitableAfterDefeat,
                    RecruitClass = npc.RecruitClass,
                    YieldDialogue = npc.YieldDialogue,
                    AcceptDialogue = npc.AcceptDialogue
                }
            };
        }

        /// <summary>Compares an NPC against a fresh clone of a candidate source, across every field Clone() copies.</summary>
        public static bool NpcMatchesClone(NPC a, NPC b)
        {
            return a.Name == b.Name
                && a.Health == b.Health
                && a.MaxHealth == b.MaxHealth
                && a.Energy == b.Energy
                && a.MaxEnergy == b.MaxEnergy
                && a.AttackDamage == b.AttackDamage
                && a.Defense == b.Defense
                && a.DamageCount == b.DamageCount
                && a.DamageDie == b.DamageDie
                && a.DamageBonus == b.DamageBonus
                && a.Speed == b.Speed
                && a.IsBackRow == b.IsBackRow
                && a.EnergyRegenPerTurn == b.EnergyRegenPerTurn
                && a.ShortDescription == b.ShortDescription
                && a.Description == b.Description
                && a.CurrentDialogueNode == b.CurrentDialogueNode
                && a.PreCombatDialogue == b.PreCombatDialogue
                && a.IsHostile == b.IsHostile
                && a.MinGold == b.MinGold
                && a.MaxGold == b.MaxGold
                && a.ExperienceReward == b.ExperienceReward
                && a.Role == b.Role
                && a.RecruitableAfterDefeat == b.RecruitableAfterDefeat
                && a.RecruitClass == b.RecruitClass
                && a.YieldDialogue == b.YieldDialogue
                && a.AcceptDialogue == b.AcceptDialogue
                && a.AbilityNames.SequenceEqual(b.AbilityNames)
                && a.LootTable.Count == b.LootTable.Count
                && a.LootTable.All(kv => b.LootTable.TryGetValue(kv.Key, out var v) && v == kv.Value)
                && (a.Dialogue?.Count ?? 0) == (b.Dialogue?.Count ?? 0);
        }
    }
}
