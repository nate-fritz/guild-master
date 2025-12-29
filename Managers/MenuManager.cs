using GuildMaster.Services;
using GuildMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GuildMaster.Managers
{
    public class MenuManager
    {
        public enum MenuState
        {
            None,
            GuildMain,
            GuildManageParty,
            GuildManagePartyAdd,
            GuildManagePartyRemove,
            GuildRecruitDetails,
            WarRoomMain,
            Stats,
            Inventory,
            Party,
            Save,
            Load,
            Settings
        }

        private MenuState currentMenu = MenuState.None;
        private GameContext context;
        private GuildManager guildManager;
        private UIManager uiManager;
        private SaveGameManager saveManager;
        private QuestManager questManager;
        private WarRoomManager warRoomManager;

        public bool IsInMenu => currentMenu != MenuState.None;

        public MenuManager(GameContext ctx, GuildManager guild, UIManager ui, SaveGameManager save, QuestManager quest, WarRoomManager warRoom)
        {
            context = ctx;
            guildManager = guild;
            uiManager = ui;
            saveManager = save;
            questManager = quest;
            warRoomManager = warRoom;
        }

        public void ShowGuildMenu()
        {
            currentMenu = MenuState.GuildMain;
            guildManager.DisplayGuildMenu();
        }

        public void ShowStatsMenu()
        {
            uiManager.ShowStats();
            // Stats is display-only - no menu state needed
        }

        public void ShowInventoryMenu()
        {
            uiManager.ShowInventory();
            // Inventory is now display-only - no menu state needed
        }

        public void ShowPartyMenu()
        {
            uiManager.ShowPartyStatus();
            // Party is now display-only - no menu state needed
        }

        public async Task ShowSaveMenuAsync()
        {
            currentMenu = MenuState.Save;
            await saveManager.DisplaySaveMenuAsync();
        }

        public async Task ShowLoadMenuAsync()
        {
            currentMenu = MenuState.Load;
            if (!await saveManager.DisplayLoadMenuAsync())
            {
                currentMenu = MenuState.None;
            }
        }

        public void ShowSettingsMenu()
        {
            currentMenu = MenuState.Settings;
            DisplaySettingsMenu();
        }

        private void DisplaySettingsMenu()
        {
            var player = context.Player;

            AnsiConsole.MarkupLine("\n═══════════════════════════════════════════════════════════════════");
            AnsiConsole.MarkupLine("                           [#FFFF00]SETTINGS[/]");
            AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
            AnsiConsole.MarkupLine("");

            string tutorialsStatus = player.TutorialsEnabled ? "[#00FF00]ON[/]" : "[#FF0000]OFF[/]";
            string autoCombatStatus = player.AutoCombatEnabled ? "[#00FF00]ON[/]" : "[#FF0000]OFF[/]";
            string goreStatus = player.GoreEnabled ? "[#00FF00]ON[/]" : "[#FF0000]OFF[/]";

            AnsiConsole.MarkupLine($"1. Tutorials ................ {tutorialsStatus}");
            AnsiConsole.MarkupLine($"2. Auto-Combat .............. {autoCombatStatus}");
            AnsiConsole.MarkupLine($"3. Gore Mode ................ {goreStatus}");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("Enter a number to toggle, or [#808080]0[/] to return.");
            AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
            AnsiConsole.MarkupLine("");
        }

        public async Task ProcessMenuInputAsync(string input)
        {
            switch (currentMenu)
            {
                case MenuState.GuildMain:
                    ProcessGuildMainInput(input);
                    break;
                case MenuState.GuildManageParty:
                    ProcessGuildManagePartyInput(input);
                    break;
                case MenuState.GuildManagePartyAdd:
                    ProcessGuildManagePartyAddInput(input);
                    break;
                case MenuState.GuildManagePartyRemove:
                    ProcessGuildManagePartyRemoveInput(input);
                    break;
                case MenuState.GuildRecruitDetails:
                    ProcessGuildRecruitDetailsInput(input);
                    break;
                case MenuState.WarRoomMain:
                    ProcessWarRoomMainInput(input);
                    break;
                case MenuState.Save:
                    await ProcessSaveInputAsync(input);
                    break;
                case MenuState.Load:
                    await ProcessLoadInputAsync(input);
                    break;
                case MenuState.Settings:
                    ProcessSettingsInput(input);
                    break;
            }
        }

        private void ProcessGuildMainInput(string input)
        {
            var player = context.Player;

            switch (input)
            {
                case "1":
                    if (player.Recruits.Count > 0)
                    {
                        currentMenu = MenuState.GuildManageParty;
                        guildManager.DisplayManagePartyMenu();
                    }
                    break;
                case "2":
                    if (player.Recruits.Count > 0)
                    {
                        currentMenu = MenuState.GuildRecruitDetails;
                        guildManager.DisplayRecruitDetailsMenu();
                    }
                    break;
                case "3":
                    if (player.Recruits.Count > 0 && context.GetQuestFlag("act_1_complete"))
                    {
                        currentMenu = MenuState.None; // Exit guild menu state
                        questManager.StartQuestMenu();
                    }
                    else if (player.Recruits.Count > 0 && !context.GetQuestFlag("act_1_complete"))
                    {
                        AnsiConsole.MarkupLine("\n[dim]The quest board is not yet available. Complete Act I first.[/]");
                        guildManager.DisplayGuildMenu();
                    }
                    break;
                case "4":
                    if (player.Recruits.Count > 0)
                    {
                        currentMenu = MenuState.WarRoomMain;
                        ShowWarRoomMenu();
                    }
                    break;
                case "0":
                    currentMenu = MenuState.None;
                    break;
                default:
                    AnsiConsole.MarkupLine("[dim]Invalid choice. Please try again.[/]");
                    guildManager.DisplayGuildMenu();
                    break;
            }
        }

        private void ProcessGuildManagePartyInput(string input)
        {
            var player = context.Player;

            switch (input)
            {
                case "1":
                    if (player.ActiveParty.Count >= 3)
                    {
                        AnsiConsole.MarkupLine("\nParty is full! Remove someone first.");
                        guildManager.DisplayManagePartyMenu();
                    }
                    else
                    {
                        currentMenu = MenuState.GuildManagePartyAdd;
                        guildManager.DisplayAddToPartyMenu();
                    }
                    break;
                case "2":
                    if (player.ActiveParty.Count == 0)
                    {
                        AnsiConsole.MarkupLine("\nNo one in party to remove.");
                        guildManager.DisplayManagePartyMenu();
                    }
                    else
                    {
                        currentMenu = MenuState.GuildManagePartyRemove;
                        guildManager.DisplayRemoveFromPartyMenu();
                    }
                    break;
                case "0":
                    currentMenu = MenuState.GuildMain;
                    guildManager.DisplayGuildMenu();
                    break;
                default:
                    AnsiConsole.MarkupLine("[dim]Invalid choice. Please try again.[/]");
                    guildManager.DisplayManagePartyMenu();
                    break;
            }
        }

        private void ProcessGuildManagePartyAddInput(string input)
        {
            // Check if recruits are available - if not, accept Enter key as well as "0"
            bool hasRecruits = guildManager.HasRecruitsAvailableToAdd();
            bool isBack = hasRecruits ? GuildMaster.Helpers.MenuInputHelper.IsBack(input) : GuildMaster.Helpers.MenuInputHelper.IsBackOrContinue(input);

            if (isBack)
            {
                currentMenu = MenuState.GuildManageParty;
                guildManager.DisplayManagePartyMenu();
                return;
            }

            if (guildManager.ProcessAddToParty(input))
            {
                currentMenu = MenuState.GuildManageParty;
                guildManager.DisplayManagePartyMenu();
            }
            else
            {
                guildManager.DisplayAddToPartyMenu();
            }
        }

        private void ProcessGuildManagePartyRemoveInput(string input)
        {
            // Check if party members exist - if not, accept Enter key as well as "0"
            bool hasPartyMembers = guildManager.HasPartyMembersToRemove();
            bool isBack = hasPartyMembers ? GuildMaster.Helpers.MenuInputHelper.IsBack(input) : GuildMaster.Helpers.MenuInputHelper.IsBackOrContinue(input);

            if (isBack)
            {
                currentMenu = MenuState.GuildManageParty;
                guildManager.DisplayManagePartyMenu();
                return;
            }

            if (guildManager.ProcessRemoveFromParty(input))
            {
                currentMenu = MenuState.GuildManageParty;
                guildManager.DisplayManagePartyMenu();
            }
            else
            {
                guildManager.DisplayRemoveFromPartyMenu();
            }
        }

        private void ProcessGuildRecruitDetailsInput(string input)
        {
            if (input == "0")
            {
                currentMenu = MenuState.GuildMain;
                guildManager.DisplayGuildMenu();
                return;
            }

            if (guildManager.ProcessRecruitDetails(input))
            {
                // Stay in the same menu
                guildManager.DisplayRecruitDetailsMenu();
            }
            else
            {
                guildManager.DisplayRecruitDetailsMenu();
            }
        }

        private async Task ProcessSaveInputAsync(string input)
        {
            if (input == "0")
            {
                currentMenu = MenuState.None;
                return;
            }

            if (await saveManager.ProcessSaveInputAsync(input))
            {
                currentMenu = MenuState.None;
            }
            else if (!saveManager.IsWaitingForConfirmation)
            {
                // Only redisplay menu if we're not waiting for confirmation
                // (e.g., user entered invalid input)
                await saveManager.DisplaySaveMenuAsync();
            }
            // If waiting for confirmation, don't redisplay - the prompt is already visible
        }

        private async Task ProcessLoadInputAsync(string input)
        {
            if (input == "0")
            {
                currentMenu = MenuState.None;
                return;
            }

            if (await saveManager.ProcessLoadInputAsync(input))
            {
                currentMenu = MenuState.None;
            }
            else
            {
                await saveManager.DisplayLoadMenuAsync();
            }
        }

        private void ProcessSettingsInput(string input)
        {
            var player = context.Player;

            switch (input)
            {
                case "1":
                    // Toggle tutorials
                    player.TutorialsEnabled = !player.TutorialsEnabled;
                    string tutStatus = player.TutorialsEnabled ? "[#00FF00]enabled[/]" : "[#FF0000]disabled[/]";
                    AnsiConsole.MarkupLine($"\nTutorials {tutStatus}.");
                    DisplaySettingsMenu();
                    break;
                case "2":
                    // Toggle auto-combat
                    player.AutoCombatEnabled = !player.AutoCombatEnabled;
                    string autoStatus = player.AutoCombatEnabled ? "[#00FF00]enabled[/]" : "[#FF0000]disabled[/]";
                    AnsiConsole.MarkupLine($"\nAuto-Combat {autoStatus}.");
                    if (player.AutoCombatEnabled)
                    {
                        AnsiConsole.MarkupLine("[dim]Party members will now use AI to select abilities during combat.[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[dim]You will manually control party members during combat.[/]");
                    }
                    DisplaySettingsMenu();
                    break;
                case "3":
                    // Toggle gore mode
                    player.GoreEnabled = !player.GoreEnabled;
                    string goreStatus = player.GoreEnabled ? "[#00FF00]enabled[/]" : "[#FF0000]disabled[/]";
                    AnsiConsole.MarkupLine($"\nGore Mode {goreStatus}.");
                    if (player.GoreEnabled)
                    {
                        AnsiConsole.MarkupLine("[dim]Combat kill messages will now display graphic descriptions.[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[dim]Combat kill messages will use standard descriptions.[/]");
                    }
                    DisplaySettingsMenu();
                    break;
                case "0":
                    // Exit settings menu
                    currentMenu = MenuState.None;
                    AnsiConsole.MarkupLine("\nReturning to game...");
                    break;
                default:
                    AnsiConsole.MarkupLine("[dim]Invalid choice. Please enter 1, 2, 3, or 0.[/]");
                    DisplaySettingsMenu();
                    break;
            }
        }

        private void ShowWarRoomMenu()
        {
            // Initialize War Room if not active
            if (!warRoomManager.WarRoomState.IsActive)
            {
                warRoomManager.InitializeWarRoom();
            }

            // Display War Room status
            warRoomManager.DisplayWarRoomStatus();

            // Display menu options
            AnsiConsole.MarkupLine("[bold]=== ACTIONS ===[/]");
            AnsiConsole.MarkupLine("1. Create Squad (1 AP) - [dim]Form a team from your recruits[/]");
            AnsiConsole.MarkupLine("2. Assign Squad to Crisis (1 AP) - [dim]Send squad to resolve active threat[/]");
            AnsiConsole.MarkupLine("3. Assign Squad to Seal (1 AP) - [dim]Station squad to defend a seal[/]");
            AnsiConsole.MarkupLine("4. End Turn (Process all actions) - [dim]Resolve combat & spawn new threats[/]");
            AnsiConsole.MarkupLine("5. View Squad Details - [dim]See squad composition and stats[/]");
            AnsiConsole.MarkupLine("0. Return to Guild");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose)[/]");
        }

        private void ProcessWarRoomMainInput(string input)
        {
            var player = context.Player;

            switch (input)
            {
                case "1":
                    CreateSquadFlow();
                    break;
                case "2":
                    AssignSquadToCrisisFlow();
                    break;
                case "3":
                    AssignSquadToSealFlow();
                    break;
                case "4":
                    warRoomManager.ProcessEndOfTurn();
                    ShowWarRoomMenu();
                    break;
                case "5":
                    ViewSquadDetailsFlow();
                    break;
                case "0":
                    currentMenu = MenuState.GuildMain;
                    guildManager.DisplayGuildMenu();
                    break;
                default:
                    AnsiConsole.MarkupLine("[dim]Invalid choice. Please try again.[/]");
                    ShowWarRoomMenu();
                    break;
            }
        }

        private void CreateSquadFlow()
        {
            var player = context.Player;
            var availableRecruits = player.Recruits.Where(r =>
                !r.IsOnQuest &&
                !r.IsResting &&
                !player.ActiveParty.Contains(r) &&
                !warRoomManager.WarRoomState.Squads.Any(s => s.Members.Any(m => m.Name == r.Name))
            ).ToList();

            if (availableRecruits.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[red]No recruits available for squad creation![/]");
                AnsiConsole.MarkupLine("[dim]Recruits may be on quests, resting, in your party, or already in squads.[/]");
                ShowWarRoomMenu();
                return;
            }

            if (!warRoomManager.WarRoomState.SpendActionPoints(1))
            {
                AnsiConsole.MarkupLine("\n[red]Not enough action points![/]");
                ShowWarRoomMenu();
                return;
            }

            AnsiConsole.MarkupLine("\n[bold]=== Create Squad ===[/]");
            AnsiConsole.MarkupLine("Available recruits:");
            for (int i = 0; i < availableRecruits.Count; i++)
            {
                var r = availableRecruits[i];
                AnsiConsole.MarkupLine($"{i + 1}. {r.Name} ({r.Class?.Name}) - HP: {r.Health}/{r.MaxHealth}, ATK: {r.AttackDamage}, DEF: {r.Defense}");
            }

            AnsiConsole.MarkupLine("\n[dim]Enter recruit numbers separated by commas (e.g., 1,2,3) - Max 4 members:[/]");
            AnsiConsole.MarkupLine("[yellow]NOTE: This is a test UI. In final version, this will be more polished.[/]");

            // For now, just create a simple test squad with the first available recruit
            // This is temporary test code
            var testSquad = new List<Recruit> { availableRecruits[0] };
            string squadName = $"Squad {warRoomManager.WarRoomState.Squads.Count + 1}";
            warRoomManager.CreateSquad(squadName, testSquad);

            AnsiConsole.MarkupLine("\n[dim]Press Enter to continue...[/]");
            ShowWarRoomMenu();
        }

        private void AssignSquadToCrisisFlow()
        {
            if (warRoomManager.WarRoomState.ActiveCrises.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]No active crises to assign squads to![/]");
                ShowWarRoomMenu();
                return;
            }

            if (warRoomManager.WarRoomState.Squads.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]No squads available! Create a squad first.[/]");
                ShowWarRoomMenu();
                return;
            }

            var availableSquads = warRoomManager.WarRoomState.Squads.Where(s => !s.IsDeployed()).ToList();
            if (availableSquads.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]All squads are already deployed![/]");
                ShowWarRoomMenu();
                return;
            }

            AnsiConsole.MarkupLine("\n[bold]=== Assign Squad to Crisis ===[/]");
            AnsiConsole.MarkupLine("[yellow]NOTE: This is a test UI. In final version, this will allow selection.[/]");

            // For testing, automatically assign first available squad to first crisis
            var squad = availableSquads[0];
            var crisis = warRoomManager.WarRoomState.ActiveCrises[0];

            warRoomManager.AssignSquadToCrisis(squad, crisis);

            AnsiConsole.MarkupLine("\n[dim]Press Enter to continue...[/]");
            ShowWarRoomMenu();
        }

        private void AssignSquadToSealFlow()
        {
            if (warRoomManager.WarRoomState.Squads.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]No squads available! Create a squad first.[/]");
                ShowWarRoomMenu();
                return;
            }

            var availableSquads = warRoomManager.WarRoomState.Squads.Where(s => !s.IsDeployed()).ToList();
            if (availableSquads.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]All squads are already deployed![/]");
                ShowWarRoomMenu();
                return;
            }

            AnsiConsole.MarkupLine("\n[bold]=== Assign Squad to Seal ===[/]");
            AnsiConsole.MarkupLine("[yellow]NOTE: This is a test UI. In final version, this will allow selection.[/]");

            // For testing, automatically assign first available squad to first seal
            var squad = availableSquads[0];
            var seal = warRoomManager.WarRoomState.Seals[0];

            warRoomManager.AssignSquadToSeal(squad, seal);

            AnsiConsole.MarkupLine("\n[dim]Press Enter to continue...[/]");
            ShowWarRoomMenu();
        }

        private void ViewSquadDetailsFlow()
        {
            if (warRoomManager.WarRoomState.Squads.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]No squads created yet![/]");
                ShowWarRoomMenu();
                return;
            }

            AnsiConsole.MarkupLine("\n[bold]=== Squad Details ===[/]");
            foreach (var squad in warRoomManager.WarRoomState.Squads)
            {
                AnsiConsole.MarkupLine($"\n[cyan]{squad.Name}[/] (Power: {squad.GetCombatPower()})");
                AnsiConsole.MarkupLine($"Status: {(squad.IsDeployed() ? $"[yellow]{squad.GetAssignmentDescription()}[/]" : "[green]Available[/]")}");
                AnsiConsole.MarkupLine("Members:");
                foreach (var member in squad.Members)
                {
                    AnsiConsole.MarkupLine($"  - {member.Name} ({member.Class?.Name}) - HP: {member.Health}/{member.MaxHealth}");
                }
            }

            AnsiConsole.MarkupLine("\n[dim]Press Enter to continue...[/]");
            ShowWarRoomMenu();
        }
    }
}
