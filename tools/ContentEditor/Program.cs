// GuildMaster Content Editor - local editing tool for wwwroot/data/rooms.json.
//
//   dotnet run --project tools/ContentEditor
//
// Serves a UI at http://localhost:5100 and reads/writes the game's rooms.json
// directly. All parsing/validation goes through the game's own
// RoomTemplateStore, so the editor can never write content the game rejects.
// Every save keeps a timestamped backup beside the file.

using System.Diagnostics;
using GuildMaster.Data;

var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
var roomsPath = Path.Combine(repoRoot, "wwwroot", "data", "rooms.json");

var npcsPath = Path.Combine(repoRoot, "wwwroot", "data", "npcs.json");
NpcTemplateStore.Load(File.ReadAllText(npcsPath));

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5100");
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Current room content, raw
app.MapGet("/api/rooms", () => Results.Text(File.ReadAllText(roomsPath), "application/json"));

// NPC names the game actually knows, for reference dropdowns
app.MapGet("/api/npcs", () => NPCData.InitializeNPCs().Keys.OrderBy(k => k).ToList());

// Full NPC content (stats + dialogue trees), raw
app.MapGet("/api/npcs-full", () => Results.Text(File.ReadAllText(npcsPath), "application/json"));

// Validate + save NPCs with backup
app.MapPut("/api/npcs-full", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    string json = await reader.ReadToEndAsync();

    try
    {
        NpcTemplateStore.Load(json);   // structural + dialogue-target validation
        NpcTemplateStore.BuildNpcs();  // must construct cleanly
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }

    var backup = npcsPath + "." + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".bak";
    File.Copy(npcsPath, backup, overwrite: false);
    await File.WriteAllTextAsync(npcsPath, json);
    return Results.Ok(new { saved = true, backup = Path.GetFileName(backup) });
});

// Validate + save with backup. Returns validation errors instead of writing bad content.
app.MapPut("/api/rooms", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    string json = await reader.ReadToEndAsync();

    try
    {
        RoomTemplateStore.Load(json);   // full structural validation, throws with details
        var npcs = NPCData.InitializeNPCs();
        RoomTemplateStore.BuildRooms(npcs);   // NPC reference validation
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }

    var backup = roomsPath + "." + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".bak";
    File.Copy(roomsPath, backup, overwrite: false);
    await File.WriteAllTextAsync(roomsPath, json);
    return Results.Ok(new { saved = true, backup = Path.GetFileName(backup) });
});

Console.WriteLine($"GuildMaster Content Editor - editing {roomsPath}");
Console.WriteLine("UI: http://localhost:5100  (Ctrl+C to stop)");
try { Process.Start(new ProcessStartInfo("http://localhost:5100") { UseShellExecute = true }); } catch { }

app.Run();
