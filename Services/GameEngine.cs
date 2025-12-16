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
            else if (slot == 4) // Autosave slot
            {
                var slotInfo = await tempLoadManager.GetAutosaveSlotInfoAsync();
                if (slotInfo.Exists)
                {
                    if (await tempLoadManager.LoadGameAsync(4))
                    {
                        // Load successful - initialize managers
                        InitializeManagersAfterLoad();
                        isWaitingForLoadSlot = false;
                        tempLoadManager = null;

                        AnsiConsole.MarkupLine("\n[#00FF00]Autosave loaded successfully![/]");
                        DisplayStats();
                        return true;
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("\n[red]Failed to load autosave.[/]");
                        return false;
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("\n[red]Autosave slot is empty![/]");
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
            string noteText = "You pick up the note, unfold it, and begin reading the letter addressed to you.<br><br>" +
                "\"Dear " + player.Name + ",<br><br>" +
                "Sorry I couldn't stay to greet you — urgent business pulled me away, and you happened to arrive at the perfect (and slightly unconscious) moment.<br><br>" +
                "We spoke briefly last night — at least, you spoke, and I assumed you were lucid. You told me your name and hinted at a past life of adventuring, so I'm officially handing you the reins of the *former* Adventurer's Guild.<br><br>" +
                "It's just you for now. Over the next year, see if you can revive the place: recruit around ten members and scrape together at least 100 gold. If you manage that, wonderful. If not… well, perhaps I put too much faith in the stranger who face-planted outside my door.<br><br>" +
                "I'll check in at year's end.<br><br>" +
                "Good luck!<br><br>" +
                "Signed,<br><br>" +
                "Alaron, Ex-Guildmaster.\"";

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
            string openingText = $"Good morning, {player.Name}.<br><br>You wake up in a bed that isn't yours, in a small room that you've never seen before..<br><br>A folded note sits on the nightstand beside you.";
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
                    string afterNoteText = "After you finish reading the letter, you notice a door to the east.";
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
                    string afterNoteText = "After you finish reading the letter, you notice a door to the east.";
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
            else if (input == "quit" || input == "qq" || input == "exit")
            {
                await HandleQuitCommand();
                return;
            }
            else if (input == "autocombat")
            {
                if (gameContext?.Player != null)
                {
                    gameContext.Player.AutoCombatEnabled = !gameContext.Player.AutoCombatEnabled;
                    string status = gameContext.Player.AutoCombatEnabled ? "[#00FF00]enabled[/]" : "[#FF0000]disabled[/]";
                    AnsiConsole.MarkupLine($"\nAutocombat {status}.");
                    if (gameContext.Player.AutoCombatEnabled)
                    {
                        AnsiConsole.MarkupLine("[dim]Party members will now use AI to select abilities during combat.[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[dim]You will manually control party members during combat.[/]");
                    }
                }
            }
            else if (input == "tutorials" || input == "tutorials on" || input == "tutorials off")
            {
                if (gameContext?.Player != null)
                {
                    if (input == "tutorials")
                    {
                        // Toggle
                        gameContext.Player.TutorialsEnabled = !gameContext.Player.TutorialsEnabled;
                    }
                    else if (input == "tutorials on")
                    {
                        gameContext.Player.TutorialsEnabled = true;
                    }
                    else if (input == "tutorials off")
                    {
                        gameContext.Player.TutorialsEnabled = false;
                    }

                    string status = gameContext.Player.TutorialsEnabled ? "[#00FF00]enabled[/]" : "[#FF0000]disabled[/]";
                    AnsiConsole.MarkupLine($"\nTutorials are now {status}.");
                }
            }
            else if (input == "settings")
            {
                ShowSettingsMenu();
            }
            else if (input.StartsWith("tpto "))
            {
                string roomArg = input.Substring(5).Trim();
                if (int.TryParse(roomArg, out int roomId))
                {
                    gameController?.TeleportToRoom(roomId);
                }
                else
                {
                    AnsiConsole.MarkupLine("[#FF0000]Usage: tpto <room number>[/]");
                }
            }
            else if (input.StartsWith("setlevel "))
            {
                string levelArg = input.Substring(9).Trim();
                if (int.TryParse(levelArg, out int targetLevel))
                {
                    gameController?.SetPlayerLevel(targetLevel);
                }
                else
                {
                    AnsiConsole.MarkupLine("[#FF0000]Usage: setlevel <level 1-20>[/]");
                }
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

        private async Task HandleQuitCommand()
        {
            // Autosave before quitting
            if (saveManager != null && gameContext?.Player != null)
            {
                await saveManager.AutoSaveAsync();
            }

            // Reset game state
            if (gameContext != null)
            {
                gameContext.Player = null;
            }

            // Clear the console
            console.Clear();

            // Show title screen via Home.razor's ShowTitle method by calling ShowStartMenu
            ShowStartMenu();

            // Trigger state change to update UI
            stateChangedCallback?.Invoke();
        }

        private void ShowSettingsMenu()
        {
            if (gameContext?.Player == null)
                return;

            while (true)
            {
                // Display settings menu
                AnsiConsole.MarkupLine("\n═══════════════════════════════════════════════════════════════════");
                AnsiConsole.MarkupLine("                           [#FFFF00]SETTINGS[/]");
                AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
                AnsiConsole.MarkupLine("");

                string tutorialsStatus = gameContext.Player.TutorialsEnabled ? "[#00FF00]ON[/]" : "[#FF0000]OFF[/]";
                string autoCombatStatus = gameContext.Player.AutoCombatEnabled ? "[#00FF00]ON[/]" : "[#FF0000]OFF[/]";

                AnsiConsole.MarkupLine($"1. Tutorials ................ [{tutorialsStatus}]");
                AnsiConsole.MarkupLine($"2. Auto-Combat .............. [{autoCombatStatus}]");
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine("Enter a number to toggle, or press Enter to return.");
                AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
                AnsiConsole.MarkupLine("");

                // Get user input
                string input = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrEmpty(input))
                {
                    // Exit settings menu
                    AnsiConsole.MarkupLine("\nReturning to game...\n");
                    DisplayStats();
                    break;
                }
                else if (input == "1")
                {
                    // Toggle tutorials
                    gameContext.Player.TutorialsEnabled = !gameContext.Player.TutorialsEnabled;
                    string newStatus = gameContext.Player.TutorialsEnabled ? "[#00FF00]enabled[/]" : "[#FF0000]disabled[/]";
                    AnsiConsole.MarkupLine($"\nTutorials {newStatus}.");
                }
                else if (input == "2")
                {
                    // Toggle auto-combat
                    gameContext.Player.AutoCombatEnabled = !gameContext.Player.AutoCombatEnabled;
                    string newStatus = gameContext.Player.AutoCombatEnabled ? "[#00FF00]enabled[/]" : "[#FF0000]disabled[/]";
                    AnsiConsole.MarkupLine($"\nAuto-Combat {newStatus}.");
                    if (gameContext.Player.AutoCombatEnabled)
                    {
                        AnsiConsole.MarkupLine("[dim]Party members will now use AI to select abilities during combat.[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[dim]You will manually control party members during combat.[/]");
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("\n[#FF0000]Invalid choice. Please enter 1, 2, or press Enter to exit.[/]");
                }

                AnsiConsole.MarkupLine("");
            }
        }

        public void ShowStartMenu()
        {
            console.MarkupLine("");
            console.MarkupLine("[#FA935F]██╗    ██╗███████╗██╗      ██████╗ ██████╗ ███╗   ███╗███████╗[/]");
            console.MarkupLine("[#FA8448]██║    ██║██╔════╝██║     ██╔════╝██╔═══██╗████╗ ████║██╔════╝[/]");
            console.MarkupLine("[#FC7938]██║ █╗ ██║█████╗  ██║     ██║     ██║   ██║██╔████╔██║█████╗  [/]");
            console.MarkupLine("[#FA6419]██║███╗██║██╔══╝  ██║     ██║     ██║   ██║██║╚██╔╝██║██╔══╝  [/]");
            console.MarkupLine("[#FA5A0A]╚███╔███╔╝███████╗███████╗╚██████╗╚██████╔╝██║ ╚═╝ ██║███████╗[/]");
            console.MarkupLine("[#BA3E00] ╚══╝╚══╝ ╚══════╝╚══════╝ ╚═════╝ ╚═════╝ ╚═╝     ╚═╝╚══════╝[/]");
            console.MarkupLine("");
            console.MarkupLine("[#FA935F]                    ████████╗ ██████╗ [/]");
            console.MarkupLine("[#FA8448]                    ╚══██╔══╝██╔═══██╗[/]");
            console.MarkupLine("[#FC7938]                       ██║   ██║   ██║[/]");
            console.MarkupLine("[#FA6419]                       ██║   ██║   ██║[/]");
            console.MarkupLine("[#FA5A0A]                       ██║   ╚██████╔╝[/]");
            console.MarkupLine("[#BA3E00]                       ╚═╝    ╚═════╝ [/]");
            console.MarkupLine("");
            console.MarkupLine("[#FA935F] ██████╗ ██╗   ██╗██╗██╗     ██████╗     ███╗   ███╗ █████╗ ███████╗████████╗███████╗██████╗ [/]");
            console.MarkupLine("[#FA8448]██╔════╝ ██║   ██║██║██║     ██╔══██╗    ████╗ ████║██╔══██╗██╔════╝╚══██╔══╝██╔════╝██╔══██╗[/]");
            console.MarkupLine("[#FC7938]██║  ███╗██║   ██║██║██║     ██║  ██║    ██╔████╔██║███████║███████╗   ██║   █████╗  ██████╔╝[/]");
            console.MarkupLine("[#FA6419]██║   ██║██║   ██║██║██║     ██║  ██║    ██║╚██╔╝██║██╔══██║╚════██║   ██║   ██╔══╝  ██╔══██╗[/]");
            console.MarkupLine("[#FA5A0A]╚██████╔╝╚██████╔╝██║███████╗██████╔╝    ██║ ╚═╝ ██║██║  ██║███████║   ██║   ███████╗██║  ██║[/]");
            console.MarkupLine("[#BA3E00] ╚═════╝  ╚═════╝ ╚═╝╚══════╝╚═════╝     ╚═╝     ╚═╝╚═╝  ╚═╝╚══════╝   ╚═╝   ╚══════╝╚═╝  ╚═╝[/]");
            console.MarkupLine("");
            console.MarkupLine("[dim]                                                             Type '/help' for command list[/]");
            console.MarkupLine("");
            console.MarkupLine("");
            console.MarkupLine("[#FFD700]1. New Game[/]");
            console.MarkupLine("[#90FF90]2. Load Game[/]");
            console.MarkupLine("");
            console.MarkupLine("[dim](Enter a number to choose)[/]");
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
