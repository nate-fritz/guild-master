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

        public bool IsInMenu => currentMenu != MenuState.None;

        public MenuManager(GameContext ctx, GuildManager guild, UIManager ui, SaveGameManager save, QuestManager quest)
        {
            context = ctx;
            guildManager = guild;
            uiManager = ui;
            saveManager = save;
            questManager = quest;
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
                    if (player.Recruits.Count > 0)
                    {
                        currentMenu = MenuState.None; // Exit guild menu state
                        questManager.StartQuestMenu();
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
            if (input == "0")
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
            if (input == "0")
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
            else
            {
                await saveManager.DisplaySaveMenuAsync();
            }
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
    }
}
