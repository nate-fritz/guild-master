// One-time migration tool: generates wwwroot/data/rooms.json from the legacy
// code-built rooms in RoomData.InitializeRoomsLegacy, then round-trips the JSON
// through RoomTemplateStore.BuildRooms and diffs the result against the legacy
// output field-by-field. Exit code 0 = written and parity verified.

using GuildMaster.Data;
using GuildMaster.Models;

var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
var outPath = Path.Combine(repoRoot, "wwwroot", "data", "rooms.json");

var npcsOutPath = Path.Combine(repoRoot, "wwwroot", "data", "npcs.json");
Console.WriteLine("Generating npcs.json from legacy NPCData...");
var legacyNpcs = NPCData.InitializeNPCsLegacy();
var npcsJson = NpcTemplateStore.ToJson(NpcTemplateStore.ToTemplates(legacyNpcs));
File.WriteAllText(npcsOutPath, npcsJson);
NpcTemplateStore.Load(npcsJson);
var npcRoundTrip = NpcTemplateStore.ToJson(NpcTemplateStore.ToTemplates(NpcTemplateStore.BuildNpcs()));
if (npcsJson != npcRoundTrip)
{
    Console.WriteLine("NPC PARITY FAILED: round-trip JSON differs");
    return 1;
}
Console.WriteLine($"NPC PARITY OK: {legacyNpcs.Count} NPCs round-trip identical ({npcsJson.Length:N0} chars)");

Console.WriteLine("Building legacy rooms...");
// Classification must use the SAME npcs dict the rooms were built with, so
// ReferenceEquals can detect shared instances.
var npcs = NPCData.InitializeNPCsLegacy();
var legacyRooms = RoomData.InitializeRoomsLegacy(npcs);

Console.WriteLine("Classifying NPC references and serializing...");
var templates = RoomTemplateStore.ToTemplates(legacyRooms, npcs);

var json = RoomTemplateStore.ToJson(templates);
Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
File.WriteAllText(outPath, json);
Console.WriteLine($"Wrote {templates.Count} rooms to {outPath} ({json.Length:N0} chars)");

var modeCounts = templates.SelectMany(t => t.Npcs.Concat(t.OriginalNpcs))
    .GroupBy(n => n.Mode).ToDictionary(g => g.Key, g => g.Count());
Console.WriteLine("NPC reference modes: " + string.Join(", ", modeCounts.Select(kv => $"{kv.Key}={kv.Value}")));

// ---- Parity check: JSON-built rooms vs legacy-built rooms ----
Console.WriteLine("\nRunning parity check...");
RoomTemplateStore.Load(json);

var npcsA = NPCData.InitializeNPCsLegacy();
var npcsB = NPCData.InitializeNPCsLegacy();
var legacy = RoomData.InitializeRoomsLegacy(npcsA);
var fromJson = RoomTemplateStore.BuildRooms(npcsB);

var problems = new List<string>();

if (!legacy.Keys.OrderBy(k => k).SequenceEqual(fromJson.Keys.OrderBy(k => k)))
    problems.Add($"Room id sets differ: legacy={legacy.Count}, json={fromJson.Count}");

foreach (var id in legacy.Keys.Intersect(fromJson.Keys))
{
    var a = legacy[id];
    var b = fromJson[id];
    void Check(string field, object? va, object? vb)
    {
        if (!Equals(va, vb)) problems.Add($"Room {id} {field}: legacy=\"{va}\" json=\"{vb}\"");
    }

    Check("Id", a.Id, b.Id);
    Check("Title", a.Title, b.Title);
    Check("Description", a.Description, b.Description);
    Check("PuzzleId", a.PuzzleId, b.PuzzleId);
    Check("CanRespawn", a.CanRespawn, b.CanRespawn);
    Check("RespawnTimeHours", a.RespawnTimeHours, b.RespawnTimeHours);

    if (!a.Exits.OrderBy(kv => kv.Key).SequenceEqual(b.Exits.OrderBy(kv => kv.Key)))
        problems.Add($"Room {id} exits differ");
    if (!a.Items.SequenceEqual(b.Items))
        problems.Add($"Room {id} items differ");
    if (!a.DescriptionVariants.OrderBy(kv => kv.Key).SequenceEqual(b.DescriptionVariants.OrderBy(kv => kv.Key)))
        problems.Add($"Room {id} description variants differ");

    if (a.Objects.Count != b.Objects.Count)
        problems.Add($"Room {id} object count: legacy={a.Objects.Count} json={b.Objects.Count}");
    else
        for (int i = 0; i < a.Objects.Count; i++)
        {
            var oa = a.Objects[i]; var ob = b.Objects[i];
            if (oa.Id != ob.Id || oa.Name != ob.Name || !oa.Aliases.SequenceEqual(ob.Aliases)
                || oa.DefaultDescription != ob.DefaultDescription || oa.LookedAtDescription != ob.LookedAtDescription
                || oa.IsInteractable != ob.IsInteractable || oa.IsHidden != ob.IsHidden
                || oa.InteractionType != ob.InteractionType || oa.RequiredItem != ob.RequiredItem
                || oa.OnInteractMessage != ob.OnInteractMessage || oa.OnFailMessage != ob.OnFailMessage)
                problems.Add($"Room {id} object[{i}] \"{oa.Id}\" differs");
        }

    CompareNpcLists(id, "NPCs", a.NPCs, b.NPCs, npcsA, npcsB, problems);
    CompareNpcLists(id, "OriginalNPCs", a.OriginalNPCs, b.OriginalNPCs, npcsA, npcsB, problems);
}

if (problems.Count == 0)
{
    Console.WriteLine($"PARITY OK: {legacy.Count} rooms identical between legacy code and rooms.json");
    return 0;
}

Console.WriteLine($"PARITY FAILED: {problems.Count} differences");
foreach (var p in problems.Take(40)) Console.WriteLine("  " + p);
return 1;

static void CompareNpcLists(int roomId, string label, List<NPC> la, List<NPC> lb,
    Dictionary<string, NPC> npcsA, Dictionary<string, NPC> npcsB, List<string> problems)
{
    if (la.Count != lb.Count)
    {
        problems.Add($"Room {roomId} {label} count: legacy={la.Count} json={lb.Count}");
        return;
    }
    for (int i = 0; i < la.Count; i++)
    {
        var a = la[i]; var b = lb[i];
        if (!RoomTemplateStore.NpcMatchesClone(a, b))
            problems.Add($"Room {roomId} {label}[{i}] \"{a.Name}\" fields differ");

        // Shared-instance semantics must match: if legacy NPC is the dict instance,
        // the JSON-built NPC must be its dict's instance too (and vice versa).
        bool aShared = npcsA.Values.Any(v => ReferenceEquals(v, a));
        bool bShared = npcsB.Values.Any(v => ReferenceEquals(v, b));
        if (aShared != bShared)
            problems.Add($"Room {roomId} {label}[{i}] \"{a.Name}\" shared-instance mismatch: legacy={aShared} json={bShared}");
    }
}
