// Headless regression harness: drives the real GameEngine through scripted
// playthroughs and asserts on the output. Exit code 0 = all scenarios pass.
//
//   dotnet run --project tools/PlaythroughHarness
//
// The full transcript of a failed run is written next to the binary
// (transcript-<scenario>.txt) for diagnosis.

using System.Text;
using System.Text.RegularExpressions;
using GuildMaster.Data;
using GuildMaster.Helpers;
using GuildMaster.Services;
using Console = System.Console;

var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
var roomsJson = File.ReadAllText(Path.Combine(repoRoot, "wwwroot", "data", "rooms.json"));
GuildMaster.Data.NpcTemplateStore.Load(File.ReadAllText(Path.Combine(repoRoot, "wwwroot", "data", "npcs.json")));

var scenarios = new List<Scenario> { IntroAndTutorials(), TravelCombatSaveLoad() };
bool allPassed = true;

foreach (var scenario in scenarios)
{
    var runner = new Runner(roomsJson);
    var result = await runner.Run(scenario);
    var transcriptPath = Path.Combine(AppContext.BaseDirectory, $"transcript-{scenario.Name}.txt");
    File.WriteAllText(transcriptPath, runner.Transcript);

    if (result == null)
    {
        Console.WriteLine($"PASS  {scenario.Name} ({scenario.Steps.Count} steps)");
    }
    else
    {
        allPassed = false;
        Console.WriteLine($"FAIL  {scenario.Name}: {result}");
        Console.WriteLine($"      transcript: {transcriptPath}");
        Console.WriteLine("      --- last output ---");
        foreach (var line in runner.LastStepOutput.Split('\n').TakeLast(25))
            Console.WriteLine("      " + line);
    }
}

return allPassed ? 0 : 1;

// ---------------------------------------------------------------------------
// Scenarios
// ---------------------------------------------------------------------------

static Scenario IntroAndTutorials() => new("intro-and-tutorials", "Harness", "Legionnaire")
{
    Steps =
    {
        Step.Cmd("look note",
            expect: new[] { "Dear Harness", "hundred gold would be a fine start", "door to the east", "To move into another room" }),
        Step.Cmd("look",
            expect: new[] { "The Guild Hall - Bedroom", "You notice:", "mug", "To examine the items" }),
        Step.Cmd("l mug",
            expect: new[] { "WORST Guildmaster" }),
        Step.Cmd("look",
            expect: new[] { "The Guild Hall - Bedroom" },
            forbid: new[] { "To examine the items" }),   // tutorial must not repeat
        Step.Cmd("xyzzy",
            expect: new[] { "not recognized" },
            allowUnrecognized: true),
    }
};

static Scenario TravelCombatSaveLoad() => new("travel-combat-save-load", "Harness", "Legionnaire")
{
    Steps =
    {
        Step.Cmd("look note", expect: new[] { "Dear Harness" }),
        Step.Cmd("east", expect: new[] { "Hallway" }),
        // The first-visit event narrative shows without blocking for input
        Step.Cmd("north", expect: new[] { "Common Area", "hang on to this feeling" }),
        Step.Cmd("north", expect: new[] { "Front Door" }),
        Step.Cmd("north", expect: new[] { "Dirt Road" }),
        Step.Cmd("north", expect: new[] { "crossroads", "farmer" }),
        // Examine an NPC by a keyword from its short description, not its name
        Step.Cmd("l farmer", expect: new[] { "burly man" }, forbid: new[] { "don't see" }),
        Step.Cmd("west", expect: new[] { "west of the crossroads" }),
        // Room 9 has a respawning hostile bandit; entering starts combat.
        // Enemy turns advance on their own (async pacing); the runner waits
        // for output to go quiet after every command, so by the time this
        // step is checked the action menu should be up.
        Step.Cmd("west", expect: new[] { "bend in the road", "COMBAT BEGINS", "Attack" }),
        Step.RepeatUntil("1", until: "VICTORY", max: 30,
            note: "basic-attack (and confirm target) until the bandit falls"),
        Step.Custom("gold drop within 1-3", transcript =>
        {
            // Last status bar reflects post-combat gold; bandit drops 1-3
            var m = Regex.Matches(transcript, @"Gold: (\d+)");
            if (m.Count == 0) return "no status bar with gold found";
            int gold = int.Parse(m[^1].Groups[1].Value);
            return gold is >= 1 and <= 3 ? null : $"gold {gold} outside bandit drop range 1-3";
        }),
        Step.Cmd("save", expect: new[] { "SAVE GAME" }),
        Step.Cmd("1", expect: new[] { "saved to slot 1" }),
        Step.Cmd("load", expect: new[] { "slot" }),
        Step.Cmd("1", expect: new[] { "loaded successfully" }),
        Step.Cmd("look", expect: new[] { "bend in the road" }),
    }
};

// ---------------------------------------------------------------------------
// Harness machinery
// ---------------------------------------------------------------------------

class Scenario
{
    public string Name { get; }
    public string PlayerName { get; }
    public string ClassName { get; }
    public List<Step> Steps { get; } = new();
    public Scenario(string name, string playerName, string className)
        => (Name, PlayerName, ClassName) = (name, playerName, className);
}

class Step
{
    public string? Command;
    public string[] Expect = Array.Empty<string>();
    public string[] Forbid = Array.Empty<string>();
    public bool AllowUnrecognized;
    public string? RepeatUntilPattern;
    public int MaxRepeats = 1;
    public string? Note;
    public Func<string, string?>? CustomCheck;

    public static Step Cmd(string command, string[] expect, string[]? forbid = null, bool allowUnrecognized = false)
        => new() { Command = command, Expect = expect, Forbid = forbid ?? Array.Empty<string>(), AllowUnrecognized = allowUnrecognized };

    public static Step RepeatUntil(string command, string until, int max, string note)
        => new() { Command = command, RepeatUntilPattern = until, MaxRepeats = max, Note = note, AllowUnrecognized = true };

    public static Step Custom(string note, Func<string, string?> check)
        => new() { Note = note, CustomCheck = check };

    public string Describe() => CustomCheck != null ? $"[check: {Note}]"
        : RepeatUntilPattern != null ? $"[repeat \"{Command}\" until /{RepeatUntilPattern}/: {Note}]"
        : $"\"{Command}\"";
}

class InMemoryStorage : IStorageService
{
    private readonly Dictionary<string, (string Content, DateTime Written)> store = new();
    public Task<string?> ReadTextAsync(string key)
        => Task.FromResult(store.TryGetValue(key, out var v) ? v.Content : (string?)null);
    public Task WriteTextAsync(string key, string content)
    { store[key] = (content, DateTime.UtcNow); return Task.CompletedTask; }
    public Task<bool> ExistsAsync(string key) => Task.FromResult(store.ContainsKey(key));
    public Task<DateTime?> GetLastWriteTimeAsync(string key)
        => Task.FromResult(store.TryGetValue(key, out var v) ? v.Written : (DateTime?)null);
    public Task CopyAsync(string sourceKey, string destKey)
    { if (store.TryGetValue(sourceKey, out var v)) store[destKey] = v; return Task.CompletedTask; }
}

class Runner
{
    private readonly GameConsole console = new();
    private readonly GameEngine engine;
    private readonly StringBuilder transcript = new();
    private readonly StringBuilder stepOutput = new();
    private static readonly string[] GlobalForbid = { "Exception", "Unhandled", "Object reference" };

    public string Transcript => transcript.ToString();
    public string LastStepOutput => stepOutput.ToString();

    public Runner(string roomsJson)
    {
        if (!RoomTemplateStore.IsLoaded)
            RoomTemplateStore.Load(roomsJson);

        GuildMaster.Services.Console.Initialize(this.console);
        AnsiConsole.Initialize(this.console);
        TextHelper.ViewportLinesPerPage = int.MaxValue;   // match web: no page breaks

        console.OnOutput += html =>
        {
            var text = System.Net.WebUtility.HtmlDecode(Regex.Replace(html, "<[^>]+>", ""));
            transcript.AppendLine(text);
            stepOutput.AppendLine(text);
        };

        engine = new GameEngine(console, new InMemoryStorage());
    }

    private async Task WaitForQuiet(int settleMs = 500, int capMs = 8000)
    {
        int lastLength;
        var start = Environment.TickCount64;
        do
        {
            lastLength = transcript.Length;
            await Task.Delay(settleMs);
            if (Environment.TickCount64 - start > capMs) return;
        } while (transcript.Length != lastLength);
    }

    public async Task<string?> Run(Scenario scenario)
    {
        transcript.AppendLine($"=== {scenario.Name} ===");
        engine.StartNewGame(scenario.PlayerName, scenario.ClassName);

        for (int i = 0; i < scenario.Steps.Count; i++)
        {
            var step = scenario.Steps[i];
            string where = $"step {i + 1} {step.Describe()}";

            if (step.CustomCheck != null)
            {
                var problem = step.CustomCheck(Transcript);
                if (problem != null) return $"{where}: {problem}";
                continue;
            }

            bool satisfied = false;
            for (int attempt = 0; attempt < step.MaxRepeats && !satisfied; attempt++)
            {
                stepOutput.Clear();
                transcript.AppendLine($"> {step.Command}");

                var task = engine.ProcessCommand(step.Command!);
                if (await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(10))) != task)
                    return $"{where}: timed out (engine awaiting input?)";
                await task;

                // Combat advances itself with async delays between turns; wait
                // until output stops flowing before asserting on it.
                await WaitForQuiet();

                if (!engine.IsInInteractiveState)
                    engine.DisplayStats();

                var output = stepOutput.ToString();

                foreach (var bad in GlobalForbid)
                    if (output.Contains(bad, StringComparison.OrdinalIgnoreCase))
                        return $"{where}: output contains \"{bad}\"";
                if (!step.AllowUnrecognized && output.Contains("not recognized", StringComparison.OrdinalIgnoreCase))
                    return $"{where}: command was not recognized";
                foreach (var bad in step.Forbid)
                    if (output.Contains(bad, StringComparison.OrdinalIgnoreCase))
                        return $"{where}: forbidden text \"{bad}\" present";

                if (step.RepeatUntilPattern != null)
                {
                    satisfied = Regex.IsMatch(output, step.RepeatUntilPattern, RegexOptions.IgnoreCase);
                }
                else
                {
                    foreach (var want in step.Expect)
                        if (!output.Contains(want, StringComparison.OrdinalIgnoreCase))
                            return $"{where}: expected \"{want}\" not found";
                    satisfied = true;
                }
            }

            if (!satisfied)
                return $"{where}: condition not met after {step.MaxRepeats} attempts";
        }

        return null;
    }
}
