using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GuildMaster;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IStorageService, LocalStorageService>();
builder.Services.AddScoped<GameConsole>();
builder.Services.AddScoped<GameEngine>();

// Load room content before the app starts; the game can't run without it.
using (var contentClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
{
    var roomsJson = await contentClient.GetStringAsync("data/rooms.json");
    GuildMaster.Data.RoomTemplateStore.Load(roomsJson);
}

await builder.Build().RunAsync();
