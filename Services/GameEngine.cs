using GuildMaster.Data;
using GuildMaster.Helpers;
using GuildMaster.Managers;
using GuildMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GuildMaster.Services
{
    public class GameEngine
    {
        private readonly GameConsole console;
        private readonly IStorageService storageService;
        private GameContext? gameContext;
        private CombatManager? combatManager;
        private SaveGameManager? saveManager;
        private UIManager? uiManager;
        private ItemManager? itemManager;
        private QuestManager? questManager;
        private GuildManager? guildManager;
        private MenuManager? menuManager;
        private DialogueManager? dialogueManager;
        private GameController? gameController;
        private Action? stateChangedCallback;
        private PaginationManager paginationManager;

        public bool IsGameStarted => gameContext?.Player != null;

        public GameEngine(GameConsole gameConsole, IStorageService storageService)
        {
            console = gameConsole;
            this.storageService = storageService;
            paginationManager = new PaginationManager();
            TextHelper.PaginationManager = paginationManager;
        }

        public void SetStateChangedCallback(Action callback)
        {
            stateChangedCallback = callback;
        }

        private async Task HandleLoadCommand()
        {
            // Create temporary context for the load operation
            if (gameContext == null)
            {
                gameContext = new GameContext
                {
                    NPCs = NPCData.InitializeNPCs(),
                    ItemDescriptions = ItemData.InitializeItemDescriptions(),
                    Effects = EffectData.InitializeEffects()
                };
                gameContext.Rooms = RoomData.InitializeRooms(gameContext.NPCs);
            }

            // Create temporary save manager
            var tempSaveManager = new SaveGameManager(gameContext, storageService);

            // Show load menu
            AnsiConsole.MarkupLine("");
            bool hasSaves = await tempSaveManager.DisplayLoadMenuAsync();

            if (!hasSaves)
            {
                // No saves found, return to main menu
                gameContext = null;
                return;
            }

            // Wait for user input (this will be handled in the next command)
            // We need to set up a state to handle the load slot selection
            isWaitingForLoadSlot = true;
            tempLoadManager = tempSaveManager;
        }

        private bool isWaitingForLoadSlot = false;
        private SaveGameManager? tempLoadManager = null;

        public bool IsWaitingForLoadSlot => isWaitingForLoadSlot;

        private async Task<bool> HandleLoadSlotSelection(string input)
        {
            if (tempLoadManager == null)
                return false;

            if (!int.TryParse(input, out int slot))
            {
                AnsiConsole.MarkupLine("\n[dim]Invalid choice. Please try again.[/]");
                return false;
            }

            if (slot == 0)
            {
                // Cancel
                isWaitingForLoadSlot = false;
                tempLoadManager = null;
                gameContext = null;
                return true;
            }

            if (slot >= 1 && slot <= 3)
            {
                var slotInfo = await tempLoadManager.GetSlotInfoAsync(slot);
                if (slotInfo.Exists)
                {
                    if (await tempLoadManager.LoadGameAsync(slot))
                    {
                        // Load successful - initialize managers
                        InitializeManagersAfterLoad();
                        isWaitingForLoadSlot = false;
                        tempLoadManager = null;

                        AnsiConsole.MarkupLine("\n[#00FF00]Game loaded successfully![/]");
                        DisplayStats();
                        return true;
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("\n[red]Failed to load game.[/]");
                        return false;
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("\n[red]That slot is empty![/]");
                    return false;
                }
            }
            else
            {
                AnsiConsole.MarkupLine("\n[red]Invalid slot number.[/]");
                return false;
            }
        }

        private void InitializeManagersAfterLoad()
        {
            if (gameContext == null || gameContext.Player == null)
                return;

            // Initialize managers with loaded context
            uiManager = new UIManager(gameContext);
            var messageManager = new MessageManager(gameContext, uiManager);
            ProgramStatics.messageManager = messageManager;
            itemManager = new ItemManager(gameContext);
            questManager = new QuestManager(gameContext);
            guildManager = new GuildManager(gameContext, questManager, messageManager);
            saveManager = new SaveGameManager(gameContext, storageService);
            menuManager = new MenuManager(gameContext, guildManager, uiManager, saveManager, questManager);
            combatManager = new CombatManager(gameContext, () => { }, stateChangedCallback);
            dialogueManager = new DialogueManager(gameContext);
            gameController = new GameController(gameContext, combatManager, saveManager);
        }

        public void StartNewGame(string playerName, string className)
        {
            // Create player with selected class
            CharacterClass selectedClass = className switch
            {
                "Legionnaire" => new Legionnaire(),
                "Venator" => new Venator(),
                "Oracle" => new Oracle(),
                _ => new Legionnaire()
            };

            var player = new Player(playerName, selectedClass);

            // Initialize game data
            var npcs = NPCData.InitializeNPCs();
            var rooms = RoomData.InitializeRooms(npcs);
            var itemDescriptions = ItemData.InitializeItemDescriptions();
            var effects = EffectData.InitializeEffects();

            // Full note text
            string noteText = "\nYou pick up the note, unfold it, and begin reading the letter addressed to you. \n\n\n\"Dear " + player.Name + ", \n\nI'm sorry I couldn't be here when you awoke; I had to leave in a bit of a hurry.  I was terribly worried about leaving the guildhall unattended in my absence, but rather miraculously, you showed up on the road outside at exactly the right time!   You did appear to be slightly unconscious, but by the time I had to leave, you seemed like you were well on the road to recovery. \n\nHopefully you remember this, but last night, we spoke briefly.  You told me your name, and even mentioned that you were something of an adventurer in your youth.  As such, I've decided to leave you in charge of The (former) Adventurer's Guild. \n\nIt's just you for now, but hopefully you can do a much better job than I was capable of.  Give it a try, if only for a year.  See if you can recruit a few new members (maybe 10?) and replenish the guild's coffers (100 gold would be a good start!)  If you can't manage that, then perhaps I was wrong about you, but I hope I'm not.  I'll reach back out at the end of the year to see how things are going. \n\nTake care, and good luck! \n\nSigned,\n\nAlaron, Ex-Guildmaster.\"";

            // Initialize game context
            gameContext = new GameContext
            {
                Player = player,
                NPCs = npcs,
                Rooms = rooms,
                ItemDescriptions = itemDescriptions,
                Effects = effects,
                NoteText = noteText
            };

            // Initialize managers
            uiManager = new UIManager(gameContext);
            var messageManager = new MessageManager(gameContext, uiManager);
            ProgramStatics.messageManager = messageManager;
            itemManager = new ItemManager(gameContext);
            questManager = new QuestManager(gameContext);
            guildManager = new GuildManager(gameContext, questManager, messageManager);
            saveManager = new SaveGameManager(gameContext, storageService);
            menuManager = new MenuManager(gameContext, guildManager, uiManager, saveManager, questManager);
            combatManager = new CombatManager(gameContext, () => { }, stateChangedCallback);
            dialogueManager = new DialogueManager(gameContext);
            gameController = new GameController(gameContext, combatManager, saveManager);

            // Display opening narrative
            string openingText = $"Good morning, {player.Name}.\n\nYou wake up in a bed that isn't yours, in a small room that you've never seen before..\n\nA folded note sits on the nightstand beside you.";
            TextHelper.DisplayTextWithPaging(openingText, "#FA935F");
            AnsiConsole.MarkupLine("");
            messageManager.CheckAndShowMessage("game_start_commands");
        }

        public async Task ProcessCommand(string input)
        {
            // Check if waiting for load slot selection
            if (isWaitingForLoadSlot)
            {
                await HandleLoadSlotSelection(input);
                return;
            }

            // Allow load command even before game starts
            if (input?.ToLower().Trim() == "load")
            {
                await HandleLoadCommand();
                return;
            }

            if (gameContext == null || gameContext.Player == null)
                return;

            var player = gameContext.Player;

            // Check for pagination "more" command or empty input (if there are more pages)
            if (paginationManager.HasMorePages && (input.ToLower() == "more" || string.IsNullOrWhiteSpace(input)))
            {
                paginationManager.ShowNextPage();

                // Re-check if pagination is done after showing the page
                if (!paginationManager.HasMorePages && player.CurrentRoom == 1)
                {
                    // Check if we just finished reading the note (by checking if we're in room 1)
                    string afterNoteText = "\nAfter you finish reading the letter, you notice a door to the east.";
                    AnsiConsole.MarkupLine("");
                    TextHelper.DisplayTextWithPaging(afterNoteText, "#FA935F");
                    ProgramStatics.messageManager?.CheckAndShowMessage("first_movement_tutorial");
                }
                return;
            }

            // Check if we're in combat mode first
            AnsiConsole.MarkupLine($"[dim]DEBUG GameEngine: combatManager != null: {combatManager != null}, IsInCombat: {combatManager?.IsInCombat}[/]");
            if (combatManager != null && combatManager.IsInCombat)
            {
                AnsiConsole.MarkupLine("[dim]DEBUG GameEngine: Routing to combat manager[/]");
                // Route input to combat system
                combatManager.ProcessCombatInput(input);
                return;
            }
            AnsiConsole.MarkupLine("[dim]DEBUG GameEngine: Not in combat, continuing to next checks[/]");


            // Check if we're in dialogue mode
            if (dialogueManager != null && dialogueManager.IsInDialogue)
            {
                try
                {
                    // Route input to dialogue system
                    dialogueManager.ProcessDialogueChoice(input);

                    // Check if dialogue has ended after processing
                    if (!dialogueManager.IsInDialogue)
                    {
                        // Check for combat after dialogue choice
                        var currentRoom = gameContext.Rooms[player.CurrentRoom];
                        var hostileNPCs = currentRoom.NPCs.Where(n => n.IsHostile).ToList();
                        if (hostileNPCs.Count > 0)
                        {
                            if (combatManager == null)
                            {
                                AnsiConsole.MarkupLine("[#FF0000]ERROR: Combat manager is null![/]");
                                return;
                            }
                            combatManager.StartCombat(hostileNPCs, currentRoom);
                        }
                        else
                        {
                            // Dialogue ended without combat - show status bar
                            DisplayStats();
                        }
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[#FF0000]Error processing dialogue: {ex.Message}[/]");
                    AnsiConsole.MarkupLine($"[#808080]{ex.StackTrace}[/]");
                    // Try to recover by ending dialogue
                    if (dialogueManager.IsInDialogue)
                    {
                        AnsiConsole.MarkupLine("[#808080]Conversation forcibly ended due to error.[/]");
                    }
                }
                return;
            }

            // Check if we're in a menu
            if (menuManager != null && menuManager.IsInMenu)
            {
                // Route input to menu system
                await menuManager.ProcessMenuInputAsync(input);

                // Show status bar after menu action (unless still in menu)
                if (!menuManager.IsInMenu)
                {
                    DisplayStats();
                }
                return;
            }

            input = input.ToLower().Trim();

            // Special Look Note Command for Starting Room only
            if ((input == "look note" || input == "l note") && player.CurrentRoom == 1)
            {
                TextHelper.DisplayTextWithPaging(gameContext.NoteText, "#fff394");

                // Only show the follow-up text if pagination is complete
                if (!paginationManager.HasMorePages)
                {
                    string afterNoteText = "\nAfter you finish reading the letter, you notice a door to the east.";
                    AnsiConsole.MarkupLine("");
                    TextHelper.DisplayTextWithPaging(afterNoteText, "#FA935F");

                    ProgramStatics.messageManager?.CheckAndShowMessage("first_movement_tutorial");
                }
                return;
            }

            if (input == "l" || input == "look" || input == "look around" || input.StartsWith("look ") || input.StartsWith("l "))
            {
                gameController?.HandleLookCommand(input);
            }
            else if (input == "north" || input == "n")
            {
                gameController?.MovePlayer("north");
            }
            else if (input == "east" || input == "e")
            {
                gameController?.MovePlayer("east");
            }
            else if (input == "south" || input == "s")
            {
                gameController?.MovePlayer("south");
            }
            else if (input == "west" || input == "w")
            {
                gameController?.MovePlayer("west");
            }
            else if (input.StartsWith("talk") || input == "t")
            {
                dialogueManager?.HandleTalkCommand(input);

                var currentRoom = gameContext.Rooms[player.CurrentRoom];
                var hostileNPCs = currentRoom.NPCs.Where(n => n.IsHostile).ToList();
                if (hostileNPCs.Count > 0)
                {
                    combatManager?.StartCombat(hostileNPCs, currentRoom);
                }
            }
            else if (input.StartsWith("take"))
            {
                itemManager?.HandleTakeCommand(input);
            }
            else if (input.StartsWith("use "))
            {
                itemManager?.HandleUseCommand(input);
            }
            else if (input == "guild" || input == "g")
            {
                menuManager?.ShowGuildMenu();
            }
            else if (input == "party" || input == "p")
            {
                menuManager?.ShowPartyMenu();
            }
            else if (input == "/help" || input == "help")
            {
                uiManager?.ShowHelp();
            }
            else if (input == "rest")
            {
                gameController?.HandleRest();
            }
            else if (input == "stats")
            {
                menuManager?.ShowStatsMenu();
            }
            else if (input == "inventory" || input == "inv" || input == "i")
            {
                menuManager?.ShowInventoryMenu();
            }
            else if (input.StartsWith("equip "))
            {
                itemManager?.HandleEquipCommand(input);
            }
            else if (input.StartsWith("unequip ") || input.StartsWith("remove "))
            {
                itemManager?.HandleUnequipCommand(input);
            }
            else if (input == "save")
            {
                if (menuManager != null)
                    await menuManager.ShowSaveMenuAsync();
            }
            else if (input == "load")
            {
                if (menuManager != null)
                    await menuManager.ShowLoadMenuAsync();
            }
            else
            {
                AnsiConsole.MarkupLine("\nCommand not recognized. Type [cyan]/help[/] to see available commands.");
            }

            // Show status bar after command completes, ready for next input
            // (Combat, dialogue, and menus have their own status displays)
            if ((combatManager == null || !combatManager.IsInCombat) &&
                (dialogueManager == null || !dialogueManager.IsInDialogue) &&
                (menuManager == null || !menuManager.IsInMenu))
            {
                DisplayStats();
            }
        }

        public void DisplayStats()
        {
            if (gameContext?.Player == null) return;
            var player = gameContext.Player;

            int hour = (int)player.CurrentHour;
            int minutes = (int)((player.CurrentHour - hour) * 60);
            string timeOfDay = hour < 12 ? "AM" : "PM";
            int displayHour = hour > 12 ? hour - 12 : hour;
            if (displayHour == 0) displayHour = 12;

            AnsiConsole.MarkupLine($"\n<span class='stats-bar'>[HP: {player.Health}/{player.MaxHealth} | EP: {player.Energy}/{player.MaxEnergy} | Day {player.CurrentDay}, {displayHour}:{minutes:D2} {timeOfDay} | Gold: {player.Gold} | Recruits: {player.Recruits.Count}/10]</span>");
        }
    }
}
