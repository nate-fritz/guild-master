using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
﻿using System;
using System.Linq;
using GuildMaster.Models;
using GuildMaster.Helpers;
using GuildMaster.Data;

namespace GuildMaster.Managers
{
    public class GameController
    {
        private readonly GameContext context;
        private readonly CombatManager combatManager;
        private readonly SaveGameManager saveManager;
        private readonly RecruitNPCManager recruitNPCManager;
        private GameEngine? gameEngine;
        public ShopManager shopManager;
        public QuestManager questManager;
        public EventManager? eventManager;
        public DialogueManager? dialogueManager;
        public PuzzleManager? puzzleManager;

        public GameController(GameContext gameContext, CombatManager combatMgr, SaveGameManager saveMgr, QuestManager questMgr, RecruitNPCManager recruitMgr)
        {
            context = gameContext;
            combatManager = combatMgr;
            saveManager = saveMgr;
            questManager = questMgr;
            recruitNPCManager = recruitMgr;
        }

        public void SetGameEngine(GameEngine engine)
        {
            gameEngine = engine;
        }

        public void HandleLookCommand(string input)
        {
            var player = context.Player;
            var rooms = context.Rooms;
            var itemDescriptions = context.ItemDescriptions;

            string[] parts = input.Split(' ', 2);
            Room currentRoomObj = rooms[player.CurrentRoom];

            if (parts.Length == 1 || (parts.Length == 2 && parts[1] == "around"))
            {
                // General room look
                AnsiConsole.MarkupLine("\n");

                // Display room title with optional room number
                string roomTitle = currentRoomObj.Title;
                if (player.RoomNumbersEnabled)
                {
                    roomTitle = $"{currentRoomObj.Title} [RoomID: {player.CurrentRoom}]";
                }
                AnsiConsole.MarkupLine($"\n<span class='room-title'>[{roomTitle}]</span>");

                TextHelper.DisplayTextWithPaging(GetRoomDescription(currentRoomObj), "#FA935F");

                if (currentRoomObj.NPCs.Count > 0)
                {
                    AnsiConsole.MarkupLine($"\n{currentRoomObj.NPCs[0].ShortDescription}, {currentRoomObj.NPCs[0].Name} is here.");
                }

                // Display items
                bool hasItems = currentRoomObj.Items.Count > 0;
                bool hasObjects = currentRoomObj.Objects.Count > 0;

                if (hasItems || hasObjects)
                {
                    AnsiConsole.MarkupLine("\nYou notice:");

                    // List takeable items
                    foreach (string item in currentRoomObj.Items)
                    {
                        // Check if item has a short name
                        string displayText = item;
                        if (itemDescriptions.ContainsKey(player.CurrentRoom) &&
                            itemDescriptions[player.CurrentRoom].ContainsKey(item) &&
                            !string.IsNullOrEmpty(itemDescriptions[player.CurrentRoom][item].ShortName))
                        {
                            string shortName = itemDescriptions[player.CurrentRoom][item].ShortName;
                            displayText = $"{item} ({shortName})";
                        }
                        AnsiConsole.MarkupLine($"- {displayText}");
                    }

                    // List room objects (only non-hidden ones)
                    foreach (var obj in currentRoomObj.Objects)
                    {
                        if (!obj.IsHidden)
                        {
                            AnsiConsole.MarkupLine($"- {obj.Name}");
                        }
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("\nYou don't see any notable items in this room.");
                }

                if (currentRoomObj.Exits.Count == 1)
                {
                    AnsiConsole.MarkupLine($"\nYou see an exit to {currentRoomObj.Exits.Keys.First()}");
                }
                else if (currentRoomObj.Exits.Count > 1)
                {
                    AnsiConsole.MarkupLine("\nYou see exits in the following directions:");

                    // Show cardinal directions first in standard order
                    string[] cardinalOrder = { "north", "east", "south", "west" };
                    foreach (string direction in cardinalOrder)
                    {
                        if (currentRoomObj.Exits.ContainsKey(direction))
                        {
                            AnsiConsole.MarkupLine($"- {direction}");
                        }
                    }

                    // Show vertical directions
                    if (currentRoomObj.Exits.ContainsKey("up"))
                    {
                        AnsiConsole.MarkupLine("- up");
                    }
                    if (currentRoomObj.Exits.ContainsKey("down"))
                    {
                        AnsiConsole.MarkupLine("- down");
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine("\nUh oh.  You can't find an exit.");
                }
            }
            else
            {
                // Specific examination
                string itemToExamine = parts[1].ToLower();

                // First check for room objects (puzzle system)
                var roomObject = FindRoomObject(currentRoomObj, itemToExamine);
                if (roomObject != null)
                {
                    // Mark as examined
                    if (!roomObject.HasBeenExamined)
                    {
                        roomObject.HasBeenExamined = true;
                        player.ExaminedItems.Add(roomObject.Id);

                        // Track examination for puzzle state
                        if (!string.IsNullOrEmpty(currentRoomObj.PuzzleId) && puzzleManager != null)
                        {
                            var puzzleState = puzzleManager.GetPuzzleState(currentRoomObj.PuzzleId);
                            if (puzzleState != null)
                            {
                                // Update puzzle state based on which object was examined
                                if (currentRoomObj.PuzzleId == "twisting_path_puzzle")
                                {
                                    if (roomObject.Id == "animal_bones")
                                    {
                                        puzzleState.CurrentState["examined_bones"] = true;
                                        // Reveal tracks after examining bones
                                        var tracksObj = currentRoomObj.Objects.FirstOrDefault(o => o.Id == "wolf_tracks");
                                        if (tracksObj != null)
                                        {
                                            tracksObj.IsHidden = false;
                                        }
                                    }
                                    else if (roomObject.Id == "wolf_tracks")
                                    {
                                        puzzleState.CurrentState["examined_tracks"] = true;
                                        // Reveal branches after examining tracks
                                        var branchesObj = currentRoomObj.Objects.FirstOrDefault(o => o.Id == "thick_branches");
                                        if (branchesObj != null)
                                        {
                                            branchesObj.IsHidden = false;
                                        }
                                    }
                                    else if (roomObject.Id == "thick_branches")
                                    {
                                        puzzleState.CurrentState["examined_branches"] = true;
                                    }
                                    else if (roomObject.Id == "boot_prints")
                                    {
                                        puzzleState.CurrentState["examined_bootprints"] = true;
                                        // Reveal vines after examining bootprints
                                        var vinesObj = currentRoomObj.Objects.FirstOrDefault(o => o.Id == "thick_vines");
                                        if (vinesObj != null)
                                        {
                                            vinesObj.IsHidden = false;
                                        }
                                    }
                                    else if (roomObject.Id == "thick_vines")
                                    {
                                        puzzleState.CurrentState["examined_vines"] = true;
                                    }
                                }
                            }
                        }
                    }

                    // Show appropriate description
                    string description = roomObject.HasBeenExamined && !string.IsNullOrEmpty(roomObject.LookedAtDescription)
                        ? roomObject.LookedAtDescription
                        : roomObject.DefaultDescription;

                    TextHelper.DisplayTextWithPaging(description);

                    // If interactable, hint at that
                    if (roomObject.IsInteractable)
                    {
                        string hintText = roomObject.InteractionType == "use"
                            ? "use [[item]] on [[object]]"
                            : $"{roomObject.InteractionType} [[object]]";
                        AnsiConsole.MarkupLine($"\n[dim](You can interact with this using: {hintText})[/]");
                    }

                    AnsiConsole.MarkupLine("");
                    return;
                }

                // Try to find item by exact match or short name
                string matchedItem = null;
                foreach (string item in currentRoomObj.Items)
                {
                    // Check exact match
                    if (item.ToLower() == itemToExamine)
                    {
                        matchedItem = item;
                        break;
                    }

                    // Check short name match
                    if (itemDescriptions.ContainsKey(player.CurrentRoom) &&
                        itemDescriptions[player.CurrentRoom].ContainsKey(item) &&
                        !string.IsNullOrEmpty(itemDescriptions[player.CurrentRoom][item].ShortName) &&
                        itemDescriptions[player.CurrentRoom][item].ShortName.ToLower() == itemToExamine)
                    {
                        matchedItem = item;
                        break;
                    }
                }

                if (matchedItem != null)
                {
                    if (itemDescriptions.ContainsKey(player.CurrentRoom) &&
                        itemDescriptions[player.CurrentRoom].ContainsKey(matchedItem))
                    {
                        var itemData = itemDescriptions[player.CurrentRoom][matchedItem];
                        string containerKey = $"room{player.CurrentRoom}_{matchedItem}";
                        bool wasExamined = player.ExaminedItems.Contains(containerKey);

                        string descriptionToShow = itemData.Description;
                        if (itemData.IsContainer && wasExamined && itemData.Contents.Count > 0 && !string.IsNullOrEmpty(itemData.EmptyDescription))
                        {
                            bool allTaken = itemData.Contents.All(item => player.TakenItems.Contains($"room{player.CurrentRoom}_{item}"));
                            if (allTaken)
                            {
                                descriptionToShow = itemData.EmptyDescription;
                            }
                        }

                        TextHelper.DisplayTextWithPaging(descriptionToShow);

                        if (itemData.IsContainer && itemData.Contents.Count > 0 && !wasExamined)
                        {
                            player.ExaminedItems.Add(containerKey);

                            if (!player.ExaminedItems.Any(e => e != containerKey))
                            {
                                ProgramStatics.messageManager.CheckAndShowMessage("first_container");
                            }

                            foreach (string contentItem in itemData.Contents)
                            {
                                currentRoomObj.Items.Add(contentItem);

                                if (contentItem == "amulet")
                                {
                                    ProgramStatics.messageManager.CheckAndShowMessage("found_quest_item_amulet");
                                }
                            }

                            if (!string.IsNullOrEmpty(itemData.DiscoveryMessage))
                            {
                                AnsiConsole.MarkupLine("");
                                TextHelper.DisplayTextWithMarkup(itemData.DiscoveryMessage);
                            }
                            else
                            {
                                if (itemData.Contents.Count == 1)
                                {
                                    string article = TextHelper.GetArticle(itemData.Contents[0]);
                                    AnsiConsole.MarkupLine($"\nYou found {article} {itemData.Contents[0]}!");
                                }
                                else
                                {
                                    AnsiConsole.MarkupLine($"\nInside you find: {string.Join(", ", itemData.Contents)}");
                                }
                            }
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"The {itemToExamine} doesn't seem particularly interesting.");
                    }
                }
                else
                {
                    // Check if item is in player's inventory
                    string inventoryMatch = player.Inventory.FirstOrDefault(item =>
                        item.ToLower() == itemToExamine ||
                        item.ToLower().Contains(itemToExamine));

                    if (inventoryMatch != null)
                    {
                        // Search all rooms for item description (items can originate from any room)
                        Item? foundItem = null;
                        foreach (var roomItems in itemDescriptions.Values)
                        {
                            if (roomItems.ContainsKey(inventoryMatch))
                            {
                                foundItem = roomItems[inventoryMatch];
                                break;
                            }
                        }

                        if (foundItem != null)
                        {
                            TextHelper.DisplayTextWithPaging(foundItem.Description);
                        }
                        else
                        {
                            // Generic description for items without specific data
                            AnsiConsole.MarkupLine($"You examine the {inventoryMatch}. Nothing particularly notable.");
                        }
                    }
                    else
                    {
                        // Check for NPCs
                        NPC targetNPC = currentRoomObj.NPCs.FirstOrDefault(npc =>
                            npc.Name.ToLower() == itemToExamine ||
                            npc.ShortDescription.ToLower() == itemToExamine ||
                            npc.ShortDescription.ToLower().Replace("a ", "").Replace("an ", "") == itemToExamine
                        );

                        if (targetNPC != null)
                        {
                            TextHelper.DisplayTextWithPaging(targetNPC.Description);
                        }
                        else
                        {
                            AnsiConsole.MarkupLine($"You don't see a {itemToExamine} here.");
                        }
                    }
                }

                AnsiConsole.MarkupLine("");
            }
        }

        public void HandleDropCommand(string input)
        {
            var player = context.Player;
            var rooms = context.Rooms;
            Room currentRoom = rooms[player.CurrentRoom];

            // Parse the item name from input (handle both "drop item" and "d item")
            string[] parts = input.Split(' ', 2);
            if (parts.Length < 2)
            {
                AnsiConsole.MarkupLine("Drop what? Specify an item to drop.");
                return;
            }

            string itemName = parts[1].ToLower().Trim();

            // Find the item in inventory (exact match or partial match)
            string matchedItem = player.Inventory.FirstOrDefault(item =>
                item.ToLower() == itemName ||
                item.ToLower().Contains(itemName));

            if (matchedItem == null)
            {
                AnsiConsole.MarkupLine($"You don't have '{itemName}' in your inventory.");
                return;
            }

            // Remove from inventory
            player.Inventory.Remove(matchedItem);

            // Add to room
            currentRoom.Items.Add(matchedItem);

            AnsiConsole.MarkupLine($"\nYou drop the {matchedItem}.");
        }

        public void MovePlayer(string direction)
        {
            var player = context.Player;
            var rooms = context.Rooms;

            Room currentRoomObj = rooms[player.CurrentRoom];

            if (currentRoomObj.Exits.ContainsKey(direction))
            {
                int destinationRoomId = currentRoomObj.Exits[direction];

                // Clear recruit NPCs from current room before leaving
                recruitNPCManager.ClearDynamicNPCsInRoom(player.CurrentRoom);

                player.PreviousRoom = player.CurrentRoom; // Track previous room for flee
                player.CurrentRoom = destinationRoomId;
                Room newRoom = rooms[player.CurrentRoom];

                player.CurrentHour += 0.25f;
                if (player.CurrentHour >= 24.0f)
                {
                    player.CurrentHour -= 24.0f;
                    player.CurrentDay++;
                    AnsiConsole.MarkupLine($"\n[A new day dawns - Day {player.CurrentDay}]");
                }

                // Check respawning, but prevent farm bandits from respawning after warlord is defeated
                bool isFarmRoom = player.CurrentRoom == 10 || player.CurrentRoom == 11;  // Gaius' Farm rooms
                bool warlordDefeated = player.QuestFlags.ContainsKey("bandit_warlord_defeated") &&
                                      player.QuestFlags["bandit_warlord_defeated"];

                if (newRoom.ShouldRespawn(player.CurrentDay, player.CurrentHour))
                {
                    // Don't respawn farm bandits if warlord is defeated
                    if (!(isFarmRoom && warlordDefeated))
                    {
                        newRoom.RespawnEnemies();
                        AnsiConsole.MarkupLine("[#FFFF00][The area has been reoccupied by enemies!][/]");
                    }
                }

                // Spawn recruit NPCs in new room
                recruitNPCManager.SpawnIdleRecruitsInRoom(player.CurrentRoom);

                AnsiConsole.MarkupLine($"\nYou move {direction} to {newRoom.Title}.");

                // Display room description first
                AnsiConsole.MarkupLine("\n");

                // Display room title with optional room number
                string roomTitle = newRoom.Title;
                if (player.RoomNumbersEnabled)
                {
                    roomTitle = $"{newRoom.Title} [RoomID: {player.CurrentRoom}]";
                }
                AnsiConsole.MarkupLine($"\n<span class='room-title'>[{roomTitle}]</span>");

                TextHelper.DisplayTextWithPaging(GetRoomDescription(newRoom), "#FA935F");

                // Check for events AFTER showing room description
                if (eventManager != null)
                {
                    EventData triggeredEvent = eventManager.CheckForEvent(player.CurrentRoom);

                    if (triggeredEvent != null)
                    {
                        // Execute event actions first
                        eventManager.ExecuteActions(triggeredEvent);

                        // Trigger associated dialogue tree (if specified)
                        if (!string.IsNullOrEmpty(triggeredEvent.DialogueTreeId) && dialogueManager != null)
                        {
                            dialogueManager.StartEventDialogue(triggeredEvent.DialogueTreeId);
                        }

                        // Mark event as triggered if one-time
                        if (triggeredEvent.IsOneTime)
                        {
                            eventManager.MarkEventTriggered(triggeredEvent.EventId);
                        }

                        // Check if ForceTravel action changed the room
                        if (player.CurrentRoom != newRoom.NumericId)
                        {
                            // Player was moved by event action - update to new room
                            newRoom = rooms[player.CurrentRoom];
                            // Spawn recruit NPCs in the new room
                            recruitNPCManager.SpawnIdleRecruitsInRoom(player.CurrentRoom);
                        }
                    }
                }

                // Check if we're now in dialogue after event processing
                bool inDialogue = dialogueManager != null && dialogueManager.IsInDialogue;

                // If event started dialogue, return early to let dialogue system take over
                if (inDialogue)
                {
                    return;
                }

                if (!inDialogue && newRoom.Exits.Count > 0)
                {
                    string[] directionOrder = { "north", "east", "south", "west" };
                    var orderedExits = directionOrder.Where(dir => newRoom.Exits.ContainsKey(dir)).ToList();

                    // Add vertical directions
                    if (newRoom.Exits.ContainsKey("up"))
                        orderedExits.Add("up");
                    if (newRoom.Exits.ContainsKey("down"))
                        orderedExits.Add("down");

                    string exitList = string.Join(", ", orderedExits);
                    AnsiConsole.MarkupLine($"\n[Exits: {exitList}]");
                }

                // Check for hostile NPCs and start combat (unless in dialogue)
                if (!inDialogue && newRoom.NPCs.Count > 0)
                {
                    var hostileNPCs = newRoom.NPCs.Where(n => n.IsHostile).ToList();
                    if (hostileNPCs.Count > 0)
                    {
                        // Check if any hostile NPCs have pre-combat dialogue
                        bool hasPreCombatDialogue = hostileNPCs.Any(npc => !string.IsNullOrEmpty(npc.PreCombatDialogue));

                        if (hasPreCombatDialogue && gameEngine != null)
                        {
                            // Show pre-combat dialogue and wait for player to continue
                            gameEngine.StartPreCombatDialogue(hostileNPCs, newRoom);
                        }
                        else
                        {
                            // No pre-combat dialogue, start combat immediately
                            if (hostileNPCs.Count == 1)
                            {
                                AnsiConsole.MarkupLine($"\n{hostileNPCs[0].ShortDescription} attacks!");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine($"\n{hostileNPCs.Count} enemies attack!");
                            }
                            combatManager.StartCombat(hostileNPCs, newRoom);
                        }
                        return;
                    }

                    var nonHostileNPCs = newRoom.NPCs.Where(n => !n.IsHostile).ToList();
                    if (nonHostileNPCs.Count > 0)
                    {
                        AnsiConsole.MarkupLine($"\n{nonHostileNPCs[0].ShortDescription} is here.");

                        // Show talk tutorial when entering the Crossroads (room 7) for the first time
                        if (player.CurrentRoom == 7)
                        {
                            ProgramStatics.messageManager?.CheckAndShowMessage("talk_tutorial");
                        }
                    }
                }
            }
            else
            {
                AnsiConsole.MarkupLine("\nYou can't go that way.");
            }
        }

        /// <summary>
        /// Gets the appropriate room description based on game state (e.g., recruit count for guild rooms)
        /// </summary>
        private string GetRoomDescription(Room room)
        {
            var player = context.Player;

            // Special handling for Room 4 (Common Area) based on recruit count
            if (room.NumericId == 4)
            {
                int recruitCount = player.Recruits.Count;

                if (recruitCount >= 10)
                    return room.GetDescription("recruits_10");
                else if (recruitCount >= 8)
                    return room.GetDescription("recruits_8");
                else if (recruitCount >= 6)
                    return room.GetDescription("recruits_6");
                else if (recruitCount >= 4)
                    return room.GetDescription("recruits_4");
            }

            // Default description
            return room.Description;
        }

        public void TeleportToRoom(int roomId)
        {
            var player = context.Player;
            var rooms = context.Rooms;

            if (!rooms.ContainsKey(roomId))
            {
                AnsiConsole.MarkupLine($"\n[#FF0000]Room {roomId} does not exist.[/]");
                return;
            }

            // Clear recruit NPCs from current room before teleporting
            recruitNPCManager.ClearDynamicNPCsInRoom(player.CurrentRoom);

            player.CurrentRoom = roomId;
            Room newRoom = rooms[roomId];

            // Spawn recruit NPCs in new room
            recruitNPCManager.SpawnIdleRecruitsInRoom(player.CurrentRoom);

            AnsiConsole.MarkupLine($"\n[#00FFFF]You teleport to {newRoom.Title}.[/]");
            AnsiConsole.MarkupLine("\n");
            AnsiConsole.MarkupLine($"\n<span class='room-title'>[{newRoom.Title}]</span>");
            TextHelper.DisplayTextWithPaging(GetRoomDescription(newRoom), "#FA935F");

            if (newRoom.Exits.Count > 0)
            {
                string[] directionOrder = { "north", "east", "south", "west" };
                var orderedExits = directionOrder.Where(dir => newRoom.Exits.ContainsKey(dir)).ToList();

                // Add vertical directions
                if (newRoom.Exits.ContainsKey("up"))
                    orderedExits.Add("up");
                if (newRoom.Exits.ContainsKey("down"))
                    orderedExits.Add("down");

                string exitList = string.Join(", ", orderedExits);
                AnsiConsole.MarkupLine($"\n[Exits: {exitList}]");
            }

            // Note: Skipping combat trigger for debug teleport - add if you want it
        }

        public void SetPlayerLevel(int targetLevel)
        {
            var player = context.Player;

            if (targetLevel < 1 || targetLevel > 20)
            {
                AnsiConsole.MarkupLine("\n[#FF0000]Level must be between 1 and 20.[/]");
                return;
            }

            if (player.Class == null)
            {
                AnsiConsole.MarkupLine("\n[#FF0000]Error: Player has no class![/]");
                return;
            }

            // Reset to level 1 base stats
            player.Class.ApplyClassBonuses(player);
            player.Level = 1;
            player.Experience = 0;
            player.ExperienceToNextLevel = player.GetXPForNextLevel(1);

            // Apply level-up bonuses for each level from 2 to target
            for (int i = 2; i <= targetLevel; i++)
            {
                player.Level = i;
                player.ApplyLevelUpBonuses();
                player.ExperienceToNextLevel = player.GetXPForNextLevel(i);
            }

            // Set experience to 0 for the target level
            player.Experience = 0;

            // Ensure health and energy are full
            player.Health = player.MaxHealth;
            player.Energy = player.MaxEnergy;

            AnsiConsole.MarkupLine($"\n[#00FF00]Level set to {targetLevel}![/]");
            AnsiConsole.MarkupLine($"[#FFFF00]Stats:[/] HP: {player.MaxHealth} | EP: {player.MaxEnergy} | ATK: {player.AttackDamage} | DEF: {player.Defense} | SPD: {player.Speed}");
        }

        public void HandleRest()
        {
            var player = context.Player;

            // Special handling for Aevoria Villa - resting triggers the celebration
            if (player.QuestFlags.ContainsKey("in_aevoria_villa") &&
                player.QuestFlags["in_aevoria_villa"] &&
                player.QuestFlags.ContainsKey("emperor_warned") &&
                player.QuestFlags["emperor_warned"])
            {
                AnsiConsole.MarkupLine("\nExhausted from the journey and the weight of tomorrow's task, you return to your guest quarters and fall into a deep sleep.");
                player.FullRestore();

                foreach (var member in player.ActiveParty)
                {
                    member.FullRestore();
                }

                // Advance time by 48 hours (2 days)
                player.CurrentHour += 48.0f;
                while (player.CurrentHour >= 24.0f)
                {
                    player.CurrentHour -= 24.0f;
                    player.CurrentDay++;
                }

                AnsiConsole.MarkupLine("\nExhausted from the journey and the weight of tomorrow's task, you return to your guest quarters and fall into a deep sleep.");
                AnsiConsole.MarkupLine("\nYou sleep through the night and the following day, recovering fully. Tomorrow, the celebration begins.");

                // Set the celebration ready flag
                player.QuestFlags["celebration_ready"] = true;

                // Move player to guest quarters - they'll wake up to the celebration event
                player.CurrentRoom = 202;

                AnsiConsole.MarkupLine("");
                return;
            }

            // Normal rest behavior
            AnsiConsole.MarkupLine("\nYou begin setting up a small camp and rest for a bit.");
            // Note: Thread.Sleep removed for web compatibility
            player.FullRestore();

            foreach (var member in player.ActiveParty)
            {
                member.FullRestore();
                AnsiConsole.MarkupLine($"{member.Name} also rests and recovers.");
            }

            player.CurrentHour += 8.0f;
            if (player.CurrentHour >= 24.0f)
            {
                player.CurrentHour -= 24.0f;
                player.CurrentDay++;
                AnsiConsole.MarkupLine($"\n[A new day dawns - Day {player.CurrentDay}]");
            }
            AnsiConsole.MarkupLine("\nYou wake up 8 hours later feeling very refreshed. Your wounds have fully healed.");
            AnsiConsole.MarkupLine("");
        }

        // NOTE: HandleQuit removed - console-specific code not used in Blazor version
        // Quit functionality is handled by GameEngine.HandleQuitCommand() with autosave

        // ============================================================================
        // PUZZLE SYSTEM METHODS
        // ============================================================================

        /// <summary>
        /// Helper: Find room object by name or alias
        /// Note: This finds ALL objects, including hidden ones, so players can examine them by name.
        /// Hidden objects just won't show in the room listing.
        /// </summary>
        private RoomObject FindRoomObject(Room room, string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();

            return room.Objects.FirstOrDefault(obj =>
                obj.Name.ToLower() == searchTerm ||
                (obj.Aliases != null && obj.Aliases.Any(alias => alias.ToLower() == searchTerm))
            );
        }

        /// <summary>
        /// Command: interact [object] or [action] [object]
        /// Examples: "pull lever", "ring bell", "step on platform", "move stone north"
        /// </summary>
        public void HandleInteractionCommand(string action, string input)
        {
            var player = context.Player;
            var rooms = context.Rooms;

            string[] args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (args.Length < 2)
            {
                AnsiConsole.MarkupLine($"\n{char.ToUpper(action[0]) + action.Substring(1)} what?");
                return;
            }

            string objectName = string.Join(" ", args.Skip(1));
            Room currentRoom = rooms[player.CurrentRoom];

            var roomObject = FindRoomObject(currentRoom, objectName);

            if (roomObject == null)
            {
                AnsiConsole.MarkupLine($"\nYou don't see any '{objectName}' here.");
                return;
            }

            if (!roomObject.IsInteractable)
            {
                AnsiConsole.MarkupLine($"\nYou can't interact with that.");
                return;
            }

            // Check if action matches object's interaction type
            if (roomObject.InteractionType.ToLower() != action.ToLower())
            {
                AnsiConsole.MarkupLine($"\nYou can't {action} that. Try: {roomObject.InteractionType}");
                return;
            }

            // Trigger interaction - puzzle-specific logic goes here
            HandleObjectInteraction(roomObject, null);
        }

        /// <summary>
        /// Centralized interaction handler
        /// This is where puzzle-specific logic will be called
        /// </summary>
        private void HandleObjectInteraction(RoomObject obj, Item usedItem)
        {
            var player = context.Player;
            var rooms = context.Rooms;

            // This method will check the room's PuzzleId and call appropriate puzzle logic
            Room currentRoom = rooms[player.CurrentRoom];

            if (string.IsNullOrEmpty(currentRoom.PuzzleId))
            {
                // No puzzle in this room, just show message
                AnsiConsole.MarkupLine(obj.OnInteractMessage ?? "\nNothing happens.");
                return;
            }

            // Get puzzle state
            var puzzleState = puzzleManager?.GetPuzzleState(currentRoom.PuzzleId);

            if (puzzleState == null)
            {
                AnsiConsole.MarkupLine("\nSomething seems wrong here...");
                return;
            }

            if (puzzleState.IsSolved)
            {
                AnsiConsole.MarkupLine("\nYou've already solved this.");
                return;
            }

            // FUTURE: Call puzzle-specific validation logic here
            // Custom puzzle handlers
            if (currentRoom.PuzzleId == "twisting_path_puzzle")
            {
                HandleTwistingPathPuzzle(obj, puzzleState, currentRoom);
            }
            else
            {
                // Default behavior for other puzzles
                puzzleManager?.IncrementAttempts(currentRoom.PuzzleId);
                AnsiConsole.MarkupLine(obj.OnInteractMessage ?? "\nYou interact with the object.");
            }
        }

        /// <summary>
        /// Handle the twisting path puzzle in room 57
        /// Two paths can be revealed:
        /// 1. West: examine bones, tracks, branches → push branches
        /// 2. North: examine bootprints, vines → move vines
        /// </summary>
        private void HandleTwistingPathPuzzle(RoomObject obj, PuzzleState state, Room room)
        {
            var player = context.Player;

            // Handle pushing branches (west path)
            if (obj.Id == "thick_branches")
            {
                bool examinedBones = (bool)(state.CurrentState["examined_bones"] ?? false);
                bool examinedTracks = (bool)(state.CurrentState["examined_tracks"] ?? false);
                bool westRevealed = (bool)(state.CurrentState["west_path_revealed"] ?? false);

                if (westRevealed)
                {
                    AnsiConsole.MarkupLine("\nThe path west is already clear.");
                    return;
                }

                if (!examinedBones && !examinedTracks)
                {
                    AnsiConsole.MarkupLine("\n[#FFAA00]The branches are too thick and tangled. You can't seem to find a good grip.[/]");
                    AnsiConsole.MarkupLine("[dim]Maybe if you looked around more carefully...[/]");
                    return;
                }

                if (!examinedBones || !examinedTracks)
                {
                    AnsiConsole.MarkupLine("\n[#FFAA00]You push against the branches, but you're not sure this is the right spot...[/]");
                    return;
                }

                // Success! Reveal west path
                AnsiConsole.MarkupLine("\n[#90FF90]You push through the tangle of branches. They give way easily, revealing a clear path to the west![/]");
                room.Exits["west"] = 58;
                state.CurrentState["west_path_revealed"] = true;

                // Check if puzzle is fully solved (both paths revealed)
                bool northRevealed = (bool)(state.CurrentState["north_path_revealed"] ?? false);
                if (northRevealed)
                {
                    state.IsSolved = true;
                }
            }
            // Handle moving vines (north path)
            else if (obj.Id == "thick_vines")
            {
                bool examinedBootprints = (bool)(state.CurrentState["examined_bootprints"] ?? false);
                bool examinedVines = (bool)(state.CurrentState["examined_vines"] ?? false);
                bool northRevealed = (bool)(state.CurrentState["north_path_revealed"] ?? false);

                if (northRevealed)
                {
                    AnsiConsole.MarkupLine("\nThe path north is already clear.");
                    return;
                }

                if (!examinedBootprints)
                {
                    AnsiConsole.MarkupLine("\n[#FFAA00]You tug at the vines, but you're not sure there's even a path here...[/]");
                    return;
                }

                if (!examinedVines)
                {
                    AnsiConsole.MarkupLine("\n[#FFAA00]The vines are thick. Maybe you should examine them more closely first.[/]");
                    return;
                }

                // Success! Reveal north path
                AnsiConsole.MarkupLine("\n[#90FF90]You push the vines aside like a curtain, revealing the path you came from to the north![/]");
                room.Exits["north"] = 56;
                state.CurrentState["north_path_revealed"] = true;

                // Check if puzzle is fully solved (both paths revealed)
                bool westRevealed = (bool)(state.CurrentState["west_path_revealed"] ?? false);
                if (westRevealed)
                {
                    state.IsSolved = true;
                }
            }
        }

        /// <summary>
        /// Handle the warlord chamber gate puzzle
        /// Requires BOTH iron key and bronze key to unlock EITHER gate
        /// </summary>
        public void HandleGatePuzzle(string input)
        {
            var player = context.Player;
            var currentRoom = context.Rooms[player.CurrentRoom];

            // Debug output
            if (player.DebugLogsEnabled)
            {
                AnsiConsole.MarkupLine($"[dim]DEBUG: HandleGatePuzzle called. Room {player.CurrentRoom}, PuzzleId: '{currentRoom.PuzzleId ?? "null"}'[/]");
            }

            // Check if we're in a room with the gate puzzle
            if (currentRoom.PuzzleId != "warlord_chamber_gates")
            {
                AnsiConsole.MarkupLine("\nThere's no gate here to use that on.");
                return;
            }

            var puzzleState = puzzleManager?.GetPuzzleState("warlord_chamber_gates");
            if (puzzleState == null)
            {
                AnsiConsole.MarkupLine("\nSomething seems wrong here...");
                return;
            }

            // Check if this gate is already unlocked
            bool isRoom18 = player.CurrentRoom == 18;
            bool isRoom20 = player.CurrentRoom == 20;
            bool ironGateUnlocked = (bool)(puzzleState.CurrentState["iron_gate_unlocked"] ?? false);
            bool bronzeGateUnlocked = (bool)(puzzleState.CurrentState["bronze_gate_unlocked"] ?? false);

            if ((isRoom18 && ironGateUnlocked) || (isRoom20 && bronzeGateUnlocked))
            {
                AnsiConsole.MarkupLine("\nThis gate is already unlocked.");
                return;
            }

            // Check if player has BOTH keys
            bool hasIronKey = player.Inventory.Contains("iron key");
            bool hasBronzeKey = player.Inventory.Contains("bronze key");

            if (!hasIronKey && !hasBronzeKey)
            {
                AnsiConsole.MarkupLine("\n[#FFAA00]You don't have any keys. The gate requires both an iron key and a bronze key.[/]");
                return;
            }
            else if (!hasIronKey)
            {
                AnsiConsole.MarkupLine("\n[#FFAA00]You have the bronze key, but the gate also requires an iron key.[/]");
                return;
            }
            else if (!hasBronzeKey)
            {
                AnsiConsole.MarkupLine("\n[#FFAA00]You have the iron key, but the gate also requires a bronze key.[/]");
                return;
            }

            // Player has both keys! Unlock the gate
            if (isRoom18)
            {
                AnsiConsole.MarkupLine("\n[#90FF90]You insert both keys into the iron gate's dual locks. With a satisfying *click*, the mechanisms turn and the gate swings open![/]");
                currentRoom.Exits["east"] = 21;
                puzzleState.CurrentState["iron_gate_unlocked"] = true;
            }
            else if (isRoom20)
            {
                AnsiConsole.MarkupLine("\n[#90FF90]You insert both keys into the bronze gate's dual locks. With a satisfying *click*, the mechanisms turn and the gate swings open![/]");
                currentRoom.Exits["west"] = 21;
                puzzleState.CurrentState["bronze_gate_unlocked"] = true;
            }

            // Check if both gates are unlocked (puzzle fully solved)
            if ((bool)(puzzleState.CurrentState["iron_gate_unlocked"] ?? false) &&
                (bool)(puzzleState.CurrentState["bronze_gate_unlocked"] ?? false))
            {
                puzzleState.IsSolved = true;
            }

            // Remove the keys from inventory (they stay in the locks)
            player.Inventory.Remove("iron key");
            player.Inventory.Remove("bronze key");
            AnsiConsole.MarkupLine("[dim]The keys remain in the locks.[/]");
        }

        public void HandleSpeakCommand(string input)
        {
            var player = context.Player;
            var currentRoom = context.Rooms[player.CurrentRoom];

            string[] args = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (args.Length < 2)
            {
                AnsiConsole.MarkupLine("\nSpeak what?");
                return;
            }

            // Get the passphrase (everything after "speak")
            string passphrase = string.Join(" ", args.Skip(1));

            // Check if we're in a room with a fog puzzle
            if (currentRoom.PuzzleId == "foggy_clearing_puzzle")
            {
                HandleFogPuzzle(passphrase, currentRoom);
            }
            else if (currentRoom.PuzzleId == "ritual_chamber_cipher")
            {
                HandleRitualCipherPuzzle(passphrase, currentRoom);
            }
            else
            {
                AnsiConsole.MarkupLine($"\nYou speak the words '{passphrase}' aloud. Nothing happens.");
            }
        }

        private void HandleFogPuzzle(string passphrase, Room room)
        {
            var puzzleState = puzzleManager?.GetPuzzleState("foggy_clearing_puzzle");
            if (puzzleState == null)
            {
                AnsiConsole.MarkupLine("\nSomething seems wrong here...");
                return;
            }

            if (puzzleState.IsSolved)
            {
                AnsiConsole.MarkupLine("\nThe fog has already cleared from this area.");
                return;
            }

            // Check if the passphrase is correct
            string correctPassphrase = "ordo dissolutus";
            if (passphrase.Trim().ToLower() == correctPassphrase)
            {
                // Correct passphrase! Clear the fog
                AnsiConsole.MarkupLine("\n[#90FF90]As you speak the ancient words 'Ordo Dissolutus,' the unnatural fog begins to shimmer and dissipate. The mist pulls back like a curtain, revealing a previously hidden path leading east into the depths of the forest.[/]");

                // Update room description permanently
                room.Description = "A clearing in the forest. The oppressive fog that once shrouded this place has lifted, revealing moss-covered stones arranged in a peculiar pattern. To the east, a narrow path winds deeper into the woods, previously concealed by the magical mist.";

                // Open the east exit to the cultist lair (Room 100)
                room.Exits["east"] = 100;

                // Mark puzzle as solved
                puzzleState.IsSolved = true;
                puzzleState.CurrentState["fog_cleared"] = true;
            }
            else
            {
                AnsiConsole.MarkupLine($"\nYou speak the words '{passphrase}' into the fog. The mist swirls slightly but does not dissipate. Perhaps these aren't the right words...");
            }
        }

        private void HandleRitualCipherPuzzle(string answer, Room room)
        {
            var puzzleState = puzzleManager?.GetPuzzleState("ritual_chamber_cipher");
            if (puzzleState == null)
            {
                AnsiConsole.MarkupLine("\nSomething seems wrong here...");
                return;
            }

            if (puzzleState.IsSolved)
            {
                AnsiConsole.MarkupLine("\nThe iron gate has already been opened. The passage south is accessible.");
                return;
            }

            // Check if the answer is correct
            string correctAnswer = puzzleState.CurrentState["correct_answer"]?.ToString() ?? "nihil";
            if (answer.Trim().ToLower() == correctAnswer)
            {
                // Correct answer! Open the gate
                AnsiConsole.MarkupLine("\n[#90FF90]As you speak the word 'Nihil,' the air in the chamber seems to shudder. The symbols on the pedestals pulse with dark light, and the iron gate groans as ancient mechanisms activate. With a grinding sound of metal on stone, the gate swings open, revealing a passage to the south.[/]");

                // Update room description
                room.Description = "A circular room with symbols painted on the floor in ash and charcoal. Six stone pedestals ring the chamber, each bearing carved symbols that now glow faintly with residual energy. The iron gate that once blocked the southern passage now stands open, its mechanisms unlocked by speaking the word that unmakes all things.";

                // Open the south exit to the library (Room 108)
                room.Exits["south"] = 108;

                // Mark puzzle as solved
                puzzleState.IsSolved = true;
                puzzleState.CurrentState["gate_unlocked"] = true;
            }
            else
            {
                // Wrong answer
                string[] wrongPhrases = new string[]
                {
                    $"\nYou speak the word '{answer}' clearly and firmly. The symbols flicker briefly, but the gate remains shut. That's not the answer.",
                    $"\nYou try '{answer},' but nothing happens. The gate stays locked. What truly unmakes all things?",
                    $"\nThe word '{answer}' echoes in the chamber, but the gate doesn't respond. You need to think more carefully about the riddle."
                };

                var random = new Random();
                AnsiConsole.MarkupLine(wrongPhrases[random.Next(wrongPhrases.Length)]);
            }
        }
    }
}