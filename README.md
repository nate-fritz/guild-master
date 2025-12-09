# GuildMaster

A text-based RPG game built with Blazor WebAssembly featuring guild management, turn-based combat, and questing mechanics.

## Play Online

Play the game at: [https://sinogue.github.io/GuildMaster/](https://sinogue.github.io/GuildMaster/)

## Features

- Three character classes: Legionnaire, Venator, and Oracle
- Turn-based tactical combat with abilities and status effects
- Guild management system - recruit and manage party members
- Quest system with varying difficulty levels
- Equipment and inventory system
- Save/Load game functionality using browser localStorage
- Animated combat effects and visual polish

## Local Development

### Prerequisites

- .NET 8.0 SDK or later

### Running Locally

```bash
dotnet restore
dotnet run
```

Navigate to `http://localhost:5035` in your browser.

### Building for Production

```bash
dotnet publish -c Release -o release
```

The built files will be in the `release/wwwroot` directory.

## Technologies

- Blazor WebAssembly
- C# / .NET 8
- HTML/CSS
- Browser localStorage API

## License

MIT License
