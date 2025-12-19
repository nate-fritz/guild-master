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
            Load
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
    }
}
