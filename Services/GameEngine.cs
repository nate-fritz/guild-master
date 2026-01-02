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
        private PuzzleManager? puzzleManager;
        private EventManager? eventManager;
        private GameController? gameController;
        private EndingEvaluator? endingEvaluator;
        private WarRoomManager? warRoomManager;
        private Action? stateChangedCallback;
        private PaginationManager paginationManager;

        public bool IsGameStarted => gameContext?.Player != null;

        // Check if we're in an interactive state that displays its own status bar
        public bool IsInInteractiveState =>
            (combatManager != null && combatManager.IsInCombat) ||
            (dialogueManager != null && dialogueManager.IsInDialogue) ||
            (menuManager != null && menuManager.IsInMenu) ||
            (gameController?.shopManager != null && gameController.shopManager.IsInShop) ||
            (gameController?.questManager != null && gameController.questManager.IsInQuestMenu);

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
                gameContext.PuzzleStates = PuzzleData.GetAllPuzzles();
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
        private GameState? pendingLoadedState = null;  // Store loaded state temporarily for message restoration
        private bool isInPreCombatDialogue = false;
        private List<NPC>? pendingCombatEnemies = null;
        private Room? pendingCombatRoom = null;

        public bool IsWaitingForLoadSlot => isWaitingForLoadSlot;
        public bool IsInPreCombatDialogue => isInPreCombatDialogue;

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
            ProgramStatics.questManager = questManager;
            guildManager = new GuildManager(gameContext, questManager, messageManager);
            saveManager = new SaveGameManager(gameContext, storageService);
            warRoomManager = new WarRoomManager(gameContext);
            menuManager = new MenuManager(gameContext, guildManager, uiManager, saveManager, questManager, warRoomManager);
            combatManager = new CombatManager(gameContext, () => { }, stateChangedCallback);
            dialogueManager = new DialogueManager(gameContext);
            puzzleManager = new PuzzleManager(gameContext);
            eventManager = new EventManager(gameContext);
            ProgramStatics.eventManager = eventManager;
            eventManager.LoadEvents();
            combatManager.SetManagers(eventManager, dialogueManager);
            endingEvaluator = new EndingEvaluator(gameContext);

            // Restore shown messages and triggered events from loaded state
            // (must happen AFTER MessageManager and EventManager creation)
            if (tempLoadManager != null)
            {
                var loadedState = tempLoadManager.GetLastLoadedState();
                if (loadedState != null)
                {
                    // Restore shown messages
                    if (loadedState.ShownMessages != null)
                    {
                        messageManager.SetShownMessages(loadedState.ShownMessages);
                    }

                    // Restore triggered event IDs
                    if (loadedState.TriggeredEventIds != null)
                    {
                        eventManager.SetTriggeredEvents(loadedState.TriggeredEventIds);
                    }
                }
            }

            // Register event dialogue trees
            EventDataDefinitions.RegisterEventDialogueTrees(dialogueManager);

            var recruitNPCManager = new RecruitNPCManager(gameContext);
            gameController = new GameController(gameContext, combatManager, saveManager, questManager, recruitNPCManager);
            gameController.SetGameEngine(this);
            gameController.eventManager = eventManager;
            gameController.dialogueManager = dialogueManager;
            gameController.puzzleManager = puzzleManager;

            // Set up dialogue->shop callback (shop manager will be created on first use)
            dialogueManager.SetOpenShopCallback((vendor) =>
            {
                // Lazy initialization of shop manager
                if (gameController.shopManager == null)
                    gameController.shopManager = new ShopManager(gameContext, uiManager);
                gameController.shopManager.StartShop(vendor);
            });
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
            var puzzleStates = PuzzleData.GetAllPuzzles();

            // Initialize game context
            gameContext = new GameContext
            {
                Player = player,
                NPCs = npcs,
                Rooms = rooms,
                ItemDescriptions = itemDescriptions,
                Effects = effects,
                PuzzleStates = puzzleStates,
                NoteText = GuildMaster.Data.NarrativeData.GenerateWelcomeNote(player.Name, player.Class.Name)
            };

            // Initialize managers
            uiManager = new UIManager(gameContext);
            var messageManager = new MessageManager(gameContext, uiManager);
            ProgramStatics.messageManager = messageManager;
            itemManager = new ItemManager(gameContext);
            questManager = new QuestManager(gameContext);
            ProgramStatics.questManager = questManager;
            guildManager = new GuildManager(gameContext, questManager, messageManager);
            saveManager = new SaveGameManager(gameContext, storageService);
            warRoomManager = new WarRoomManager(gameContext);
            menuManager = new MenuManager(gameContext, guildManager, uiManager, saveManager, questManager, warRoomManager);
            combatManager = new CombatManager(gameContext, () => { }, stateChangedCallback);
            dialogueManager = new DialogueManager(gameContext);
            puzzleManager = new PuzzleManager(gameContext);
            eventManager = new EventManager(gameContext);
            ProgramStatics.eventManager = eventManager;
            eventManager.LoadEvents();
            combatManager.SetManagers(eventManager, dialogueManager);
            endingEvaluator = new EndingEvaluator(gameContext);

            // Register event dialogue trees
            EventDataDefinitions.RegisterEventDialogueTrees(dialogueManager);

            var recruitNPCManager = new RecruitNPCManager(gameContext);
            gameController = new GameController(gameContext, combatManager, saveManager, questManager, recruitNPCManager);
            gameController.SetGameEngine(this);
            gameController.eventManager = eventManager;
            gameController.dialogueManager = dialogueManager;
            gameController.puzzleManager = puzzleManager;

            // Set up dialogue->shop callback (shop manager will be created on first use)
            dialogueManager.SetOpenShopCallback((vendor) =>
            {
                // Lazy initialization of shop manager
                if (gameController.shopManager == null)
                    gameController.shopManager = new ShopManager(gameContext, uiManager);
                gameController.shopManager.StartShop(vendor);
            });

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
                if (!paginationManager.HasMorePages)
                {
                    // Pagination complete - show status bar after final page
                    if (player.CurrentRoom == 1)
                    {
                        // Check if we just finished reading the note (by checking if we're in room 1)
                        string afterNoteText = "After you finish reading the letter, you notice a door to the east.";
                        AnsiConsole.MarkupLine("");
                        TextHelper.DisplayTextWithPaging(afterNoteText, "#FA935F");
                        ProgramStatics.messageManager?.CheckAndShowMessage("first_movement_tutorial");
                    }
                    // Status bar will be shown by Home.razor after command completes
                }
                return;
            }

            // Check if we're in combat mode first
            if (player.DebugLogsEnabled)
            {
                AnsiConsole.MarkupLine($"[dim]DEBUG GameEngine: combatManager != null: {combatManager != null}, IsInCombat: {combatManager?.IsInCombat}[/]");
            }
            if (combatManager != null && combatManager.IsInCombat)
            {
                if (player.DebugLogsEnabled)
                {
                    AnsiConsole.MarkupLine("[dim]DEBUG GameEngine: Routing to combat manager[/]");
                }
                // Route input to combat system
                combatManager.ProcessCombatInput(input);
                return;
            }

            // Check if combat just ended and player wants to load a game
            if (combatManager != null && combatManager.ShouldShowLoadMenu)
            {
                if (menuManager != null)
                    await menuManager.ShowLoadMenuAsync();
                return;
            }

            AnsiConsole.MarkupLine("[dim]DEBUG GameEngine: Not in combat, continuing to next checks[/]");

            // Check if we're in pre-combat dialogue
            if (isInPreCombatDialogue)
            {
                ProcessPreCombatDialogueInput(input);
                return;
            }

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
                        else if (eventManager != null)
                        {
                            // Check for events after dialogue ends (handles force_travel events)
                            EventData triggeredEvent = eventManager.CheckForEvent(player.CurrentRoom);

                            if (triggeredEvent != null)
                            {
                                // Execute event actions first
                                eventManager.ExecuteActions(triggeredEvent);

                                // Trigger associated dialogue tree (if specified)
                                if (!string.IsNullOrEmpty(triggeredEvent.DialogueTreeId))
                                {
                                    dialogueManager.StartEventDialogue(triggeredEvent.DialogueTreeId);
                                }

                                // Mark event as triggered if one-time
                                if (triggeredEvent.IsOneTime)
                                {
                                    eventManager.MarkEventTriggered(triggeredEvent.EventId);
                                }

                                // Show guild quest ledger tutorial after Act Two intro
                                if (triggeredEvent.EventId == "act_two_intro" && ProgramStatics.messageManager != null)
                                {
                                    ProgramStatics.messageManager.CheckAndShowMessage("guild_quest_ledger");
                                }
                            }
                        }
                        // Status bar will be shown by Home.razor after command completes
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
                // Status bar will be shown by Home.razor after command completes
                return;
            }

            // Check if we're in a shop
            if (gameController?.shopManager != null && gameController.shopManager.IsInShop)
            {
                // Route input to shop system
                gameController.shopManager.ProcessShopInput(input);
                // Status bar will be shown by Home.razor after command completes
                return;
            }

            // Check if we're in a quest menu
            if (gameController?.questManager != null && gameController.questManager.IsInQuestMenu)
            {
                // Route input to quest system
                gameController.questManager.ProcessQuestInput(input);
                // Status bar will be shown by Home.razor after command completes
                return;
            }

            input = input.ToLower().Trim();

            // Special Look Note Command for Starting Room only
            if ((input == "look note" || input == "l note") && player.CurrentRoom == 1)
            {
                TextHelper.DisplayTextWithPaging(gameContext.NoteText, "#90FF90");

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
            else if (input == "up" || input == "u")
            {
                gameController?.MovePlayer("up");
            }
            else if (input == "down" || input == "d")
            {
                gameController?.MovePlayer("down");
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
                // Check for gate puzzle interactions first (accept "key" or "keys")
                bool hasKeyReference = input.Contains("key on") || input.Contains("keys on");
                bool hasGateReference = input.Contains("on gate") || input.Contains("on iron gate") || input.Contains("on bronze gate");

                if (hasKeyReference && hasGateReference)
                {
                    if (gameController == null)
                    {
                        AnsiConsole.MarkupLine("\n[#FF0000]ERROR: Game controller not initialized.[/]");
                        return;
                    }
                    gameController.HandleGatePuzzle(input);
                }
                else
                {
                    itemManager?.HandleUseCommand(input);
                }
            }
            else if (input.StartsWith("examine "))
            {
                // Allow "examine" as an alias for "look"
                gameController?.HandleLookCommand(input.Replace("examine", "look"));
            }
            else if (input.StartsWith("pull "))
            {
                gameController?.HandleInteractionCommand("pull", input);
            }
            else if (input.StartsWith("push "))
            {
                gameController?.HandleInteractionCommand("push", input);
            }
            else if (input.StartsWith("ring "))
            {
                gameController?.HandleInteractionCommand("ring", input);
            }
            else if (input.StartsWith("step "))
            {
                gameController?.HandleInteractionCommand("step", input);
            }
            else if (input.StartsWith("move "))
            {
                gameController?.HandleInteractionCommand("move", input);
            }
            else if (input.StartsWith("set "))
            {
                gameController?.HandleInteractionCommand("set", input);
            }
            else if (input.StartsWith("ride "))
            {
                gameController?.HandleInteractionCommand("ride", input);
            }
            else if (input.StartsWith("search "))
            {
                gameController?.HandleInteractionCommand("search", input);
            }
            else if (input.StartsWith("speak "))
            {
                gameController?.HandleSpeakCommand(input);
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
            else if (input == "recall")
            {
                // Teleport player to guild hall common area
                if (player.CurrentRoom == 1)
                {
                    AnsiConsole.MarkupLine("[#FFFF00]You're already in the guild hall![/]");
                }
                else
                {
                    player.CurrentRoom = 1;
                    var guildHall = gameContext.Rooms[1];

                    AnsiConsole.MarkupLine("\n[#00FFFF]You concentrate and feel the familiar pull of the guild hall...[/]");
                    AnsiConsole.MarkupLine("[#00FFFF]The world blurs around you, and in an instant, you're standing in the guild hall common area.[/]\n");

                    // Display room title
                    string roomTitle = guildHall.Title;
                    if (player.RoomNumbersEnabled)
                    {
                        roomTitle = $"{guildHall.Title} [RoomID: 1]";
                    }
                    AnsiConsole.MarkupLine($"\n<span class='room-title'>[{roomTitle}]</span>");

                    // Display room description
                    TextHelper.DisplayTextWithPaging(guildHall.Description, "#FA935F");
                }
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
                menuManager?.ShowSettingsMenu();
            }
            else if (input.StartsWith("shop ") || input.StartsWith("trade "))
            {
                string npcName = input.Contains("shop ") ? input.Substring(5).Trim() : input.Substring(6).Trim();
                HandleShopCommand(npcName);
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
            else if (input.StartsWith("giveitem "))
            {
                string itemName = input.Substring(9).Trim();
                if (!string.IsNullOrEmpty(itemName))
                {
                    gameContext.Player.Inventory.Add(itemName);
                    AnsiConsole.MarkupLine($"\n[#00FF00]Added '{itemName}' to inventory.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[#FF0000]Usage: giveitem <item name>[/]");
                }
            }
            else if (input.StartsWith("givegold "))
            {
                string amountArg = input.Substring(9).Trim();
                if (int.TryParse(amountArg, out int amount) && amount > 0)
                {
                    gameContext.Player.Gold += amount;
                    AnsiConsole.MarkupLine($"\n[#00FF00]Added {amount} gold. New total: {gameContext.Player.Gold}g[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[#FF0000]Usage: givegold <amount>[/]");
                }
            }
            else if (input == "flags")
            {
                AnsiConsole.MarkupLine("\n[#FFD700]═══ Quest Flags ═══[/]");
                if (gameContext.Player.QuestFlags.Count == 0)
                {
                    AnsiConsole.MarkupLine("[dim]No quest flags set.[/]");
                }
                else
                {
                    foreach (var flag in gameContext.Player.QuestFlags.OrderBy(f => f.Key))
                    {
                        string value = flag.Value ? "[#00FF00]TRUE[/]" : "[#FF0000]FALSE[/]";
                        AnsiConsole.MarkupLine($"  {flag.Key}: {value}");
                    }
                }
                AnsiConsole.MarkupLine("");
            }
            else if (input == "showdebug")
            {
                gameContext.Player.DebugLogsEnabled = !gameContext.Player.DebugLogsEnabled;
                string status = gameContext.Player.DebugLogsEnabled ? "[#00FF00]enabled[/]" : "[#FF0000]disabled[/]";
                AnsiConsole.MarkupLine($"\nDebug logs {status}.");
            }
            else if (input == "roomnumbers")
            {
                gameContext.Player.RoomNumbersEnabled = !gameContext.Player.RoomNumbersEnabled;
                string status = gameContext.Player.RoomNumbersEnabled ? "[#00FF00]enabled[/]" : "[#FF0000]disabled[/]";
                AnsiConsole.MarkupLine($"\nRoom numbers {status}.");
            }
            else if (input == "warroom")
            {
                gameContext.Player.WarRoomEnabled = !gameContext.Player.WarRoomEnabled;
                string status = gameContext.Player.WarRoomEnabled ? "[#00FF00]enabled[/]" : "[#FF0000]disabled[/]";
                string visibility = gameContext.Player.WarRoomEnabled ? "appear" : "be hidden";
                AnsiConsole.MarkupLine($"\nWar Room access {status}.");
                AnsiConsole.MarkupLine($"[dim]War Room will now {visibility} in the Guild menu.[/]");
            }
            else if (input == "/adminhelp" || input == "adminhelp")
            {
                uiManager?.ShowAdminHelp();
            }
            else
            {
                AnsiConsole.MarkupLine("\nCommand not recognized. Type [cyan]help[/] to see available commands.");
            }

            // Show status bar after command completes, ready for next input
            // Status bar will be shown by Home.razor after command completes
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


        private void HandleShopCommand(string npcName)
        {
            if (gameContext?.Player == null || gameContext?.Rooms == null || gameController == null)
                return;

            // Get the current room
            if (!gameContext.Rooms.ContainsKey(gameContext.Player.CurrentRoom))
                return;

            Room currentRoom = gameContext.Rooms[gameContext.Player.CurrentRoom];

            // Find the NPC in the current room
            var npc = currentRoom.NPCs.FirstOrDefault(n =>
                n.Name.Equals(npcName, System.StringComparison.OrdinalIgnoreCase));

            if (npc == null)
            {
                AnsiConsole.MarkupLine($"\n[#FF0000]There is no one named '{npcName}' here.[/]");
                return;
            }

            if (!npc.IsVendor)
            {
                AnsiConsole.MarkupLine($"\n[#FF0000]{npc.Name} is not a vendor.[/]");
                return;
            }

            // Lazy initialization of shop manager on first use
            if (gameController.shopManager == null)
            {
                gameController.shopManager = new ShopManager(gameContext, uiManager);
            }

            // Open the shop (non-blocking, will wait for next input)
            gameController.shopManager.StartShop(npc);
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
            console.MarkupLine("[dim]                                                             Type 'help' for command list[/]");
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

            AnsiConsole.MarkupLine($"\n<span class='stats-bar'>[HP: {player.Health}/{player.TotalMaxHealth} | EP: {player.Energy}/{player.TotalMaxEnergy} | Day {player.CurrentDay}, {displayHour}:{minutes:D2} {timeOfDay} | Gold: {player.Gold} | Recruits: {player.Recruits.Count}/10]</span>");
        }

        public void StartPreCombatDialogue(List<NPC> enemies, Room room)
        {
            // Store pending combat info
            pendingCombatEnemies = enemies;
            pendingCombatRoom = room;
            isInPreCombatDialogue = true;

            // Display pre-combat dialogue from enemies
            foreach (var enemy in enemies)
            {
                if (!string.IsNullOrEmpty(enemy.PreCombatDialogue))
                {
                    AnsiConsole.MarkupLine("");
                    TextHelper.DisplayTextWithPaging(enemy.PreCombatDialogue, "#fc3838");
                    break; // Only show dialogue from first enemy with dialogue
                }
            }

            // Show continuation prompt
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("Press Enter to continue");
        }

        public void ProcessPreCombatDialogueInput(string input)
        {
            // Accept Enter key or "0" to continue to combat
            if (GuildMaster.Helpers.MenuInputHelper.IsBackOrContinue(input))
            {
                // Clear pre-combat dialogue state
                isInPreCombatDialogue = false;

                // Start combat with pending enemies
                if (pendingCombatEnemies != null && pendingCombatRoom != null && combatManager != null)
                {
                    combatManager.StartCombat(pendingCombatEnemies, pendingCombatRoom);
                }

                // Clear pending combat info
                pendingCombatEnemies = null;
                pendingCombatRoom = null;
            }
        }
    }
}
