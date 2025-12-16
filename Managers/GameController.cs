using GuildMaster.Services;
using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
﻿using System;
using System.Linq;
using System.Threading;
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

        public GameController(GameContext gameContext, CombatManager combatMgr, SaveGameManager saveMgr)
        {
            context = gameContext;
            combatManager = combatMgr;
            saveManager = saveMgr;
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
                AnsiConsole.MarkupLine($"\n<span class='room-title'>[{currentRoomObj.Title}]</span>");
                TextHelper.DisplayTextWithPaging(currentRoomObj.Description, "#FA935F");

                if (currentRoomObj.NPCs.Count > 0)
                {
                    AnsiConsole.MarkupLine($"\n{currentRoomObj.NPCs[0].ShortDescription}, {currentRoomObj.NPCs[0].Name} is here.");
                }

                if (currentRoomObj.Items.Count > 0)
                {
                    AnsiConsole.MarkupLine("\nYou notice the following items in the room:");
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
                    foreach (string direction in currentRoomObj.Exits.Keys)
                    {
                        AnsiConsole.MarkupLine($"- {direction}");
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

                AnsiConsole.MarkupLine("");
            }
        }

        public void MovePlayer(string direction)
        {
            var player = context.Player;
            var rooms = context.Rooms;

            Room currentRoomObj = rooms[player.CurrentRoom];

            if (currentRoomObj.Exits.ContainsKey(direction))
            {
                player.PreviousRoom = player.CurrentRoom; // Track previous room for flee
                player.CurrentRoom = currentRoomObj.Exits[direction];
                Room newRoom = rooms[player.CurrentRoom];

                player.CurrentHour += 0.25f;
                if (player.CurrentHour >= 24.0f)
                {
                    player.CurrentHour -= 24.0f;
                    player.CurrentDay++;
                    AnsiConsole.MarkupLine($"\n[A new day dawns - Day {player.CurrentDay}]");
                }

                if (newRoom.ShouldRespawn(player.CurrentDay, player.CurrentHour))
                {
                    newRoom.RespawnEnemies();
                    AnsiConsole.MarkupLine("[#FFFF00][The area has been reoccupied by enemies!][/]");
                }

                AnsiConsole.MarkupLine($"\nYou move {direction} to {newRoom.Title}.");
                AnsiConsole.MarkupLine("\n");
                AnsiConsole.MarkupLine($"\n<span class='room-title'>[{newRoom.Title}]</span>");
                TextHelper.DisplayTextWithPaging(newRoom.Description, "#FA935F");

                if (newRoom.Exits.Count > 0)
                {
                    string[] directionOrder = { "north", "east", "south", "west" };
                    var orderedExits = directionOrder.Where(dir => newRoom.Exits.ContainsKey(dir));
                    string exitList = string.Join(", ", orderedExits);
                    AnsiConsole.MarkupLine($"\n[Exits: {exitList}]");
                }

                // Check for hostile NPCs and start combat
                if (newRoom.NPCs.Count > 0)
                {
                    var hostileNPCs = newRoom.NPCs.Where(n => n.IsHostile).ToList();
                    if (hostileNPCs.Count > 0)
                    {
                        if (hostileNPCs.Count == 1)
                        {
                            AnsiConsole.MarkupLine($"\n{hostileNPCs[0].ShortDescription} attacks!");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine($"\n{hostileNPCs.Count} enemies attack!");
                        }
                        combatManager.StartCombat(hostileNPCs, newRoom);
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

        public void TeleportToRoom(int roomId)
        {
            var player = context.Player;
            var rooms = context.Rooms;

            if (!rooms.ContainsKey(roomId))
            {
                AnsiConsole.MarkupLine($"\n[#FF0000]Room {roomId} does not exist.[/]");
                return;
            }

            player.CurrentRoom = roomId;
            Room newRoom = rooms[roomId];

            AnsiConsole.MarkupLine($"\n[#00FFFF]You teleport to {newRoom.Title}.[/]");
            AnsiConsole.MarkupLine("\n");
            AnsiConsole.MarkupLine($"\n<span class='room-title'>[{newRoom.Title}]</span>");
            TextHelper.DisplayTextWithPaging(newRoom.Description, "#FA935F");

            if (newRoom.Exits.Count > 0)
            {
                string[] directionOrder = { "north", "east", "south", "west" };
                var orderedExits = directionOrder.Where(dir => newRoom.Exits.ContainsKey(dir));
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

        public bool HandleQuit()
        {
            bool validResponse = false;
            while (!validResponse)
            {
                Console.Write("\nWould you like to save your game before quitting? (y/n): ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                string saveChoice = Console.ReadLine()?.ToLower().Trim();
                Console.ResetColor();

                if (saveChoice == "y" || saveChoice == "yes")
                {
                    AnsiConsole.MarkupLine("\nSaving game...");
                    // NOTE: This HandleQuit method is for console app only - not used in web version
                    // await saveManager.SaveGameAsync();
                    validResponse = true;
                }
                else if (saveChoice == "n" || saveChoice == "no")
                {
                    AnsiConsole.MarkupLine("Game not saved.");
                    validResponse = true;
                }
                else
                {
                    AnsiConsole.MarkupLine("Please enter 'y' for yes or 'n' for no.");
                }
            }

            AnsiConsole.MarkupLine("\nGiving up?  You'll be back!  Thanks for playing.");
            Console.Write("Press Enter to exit the game...");
            Console.ReadLine();

            AnsiConsole.MarkupLine("Exiting the game...");
            return true;
        }
    }
}