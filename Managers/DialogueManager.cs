using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;
using GuildMaster.Helpers;

namespace GuildMaster.Managers
{
    public class DialogueManager
    {
        private readonly GameContext context;
        private readonly Action recruitmentCallback;  // To refresh guild after recruitment

        // State for non-blocking dialogue
        private NPC? currentNPC;
        private Room? currentRoom;
        private string? currentNode;
        private List<DialogueNode.Choice>? currentChoices;
        private bool isInDialogue = false;

        public DialogueManager(GameContext gameContext, Action onRecruitmentCallback = null)
        {
            context = gameContext;
            recruitmentCallback = onRecruitmentCallback;
        }

        public bool IsInDialogue => isInDialogue;

        public void HandleTalkCommand(string input)
        {
            var player = context.Player;
            var currentRoomObj = context.Rooms[player.CurrentRoom];

            if (currentRoomObj.NPCs.Count == 0)
            {
                AnsiConsole.MarkupLine("\nThere's no one here for you to talk to.");
                return;
            }

            NPC npc = null;
            string[] parts = input.Split(' ', 2);

            if (parts.Length > 1)
            {
                // User specified a target
                string target = parts[1].ToLower();

                // Try to find NPC by name or description
                npc = currentRoomObj.NPCs.FirstOrDefault(n =>
                    n.Name.ToLower() == target ||
                    n.Name.ToLower().Contains(target) ||
                    (n.ShortDescription != null && n.ShortDescription.ToLower().Contains(target))
                );

                if (npc == null)
                {
                    // Try matching against just the key word from the description
                    npc = currentRoomObj.NPCs.FirstOrDefault(n =>
                    {
                        if (string.IsNullOrEmpty(n.ShortDescription)) return false;

                        string[] descWords = n.ShortDescription.ToLower()
                            .Replace("a ", "").Replace("an ", "").Replace("the ", "")
                            .Split(' ');

                        return descWords.Any(word => word == target || word.Contains(target));
                    });
                }

                if (npc == null)
                {
                    AnsiConsole.MarkupLine($"\nYou don't see anyone matching '{parts[1]}' here.");
                    if (currentRoomObj.NPCs.Count == 1)
                    {
                        AnsiConsole.MarkupLine($"You can talk to {currentRoomObj.NPCs[0].Name}.");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("You can talk to:");
                        foreach (var n in currentRoomObj.NPCs)
                        {
                            AnsiConsole.MarkupLine($"  - {n.Name} ({n.ShortDescription})");
                        }
                    }
                    return;
                }
            }
            else
            {
                // No target specified
                if (currentRoomObj.NPCs.Count > 1)
                {
                    AnsiConsole.MarkupLine("\nMultiple people are here. Who do you want to talk to?");
                    foreach (var n in currentRoomObj.NPCs)
                    {
                        AnsiConsole.MarkupLine($"  - {n.Name} ({n.ShortDescription})");
                    }
                    AnsiConsole.MarkupLine("\nUse: talk <name>");
                    return;
                }
                npc = currentRoomObj.NPCs[0];
            }

            StartDialogue(npc, currentRoomObj);
        }

        private void StartDialogue(NPC npc, Room room)
        {
            currentNPC = npc;
            currentRoom = room;
            isInDialogue = true;

            // Determine starting node
            if (npc.CurrentDialogueNode == "greeting")
            {
                currentNode = "greeting";
            }
            else
            {
                currentNode = "main_hub";
            }

            AnsiConsole.MarkupLine($"\nYou approach {npc.Name}.\n");

            ShowCurrentDialogueNode();
        }

        private void ShowCurrentDialogueNode()
        {
            var player = context.Player;

            if (!currentNPC.Dialogue.ContainsKey(currentNode))
            {
                AnsiConsole.MarkupLine("Something went wrong with the conversation.");
                EndDialogue();
                return;
            }

            DialogueNode node = currentNPC.Dialogue[currentNode];

            TextHelper.DisplayTextWithPaging(node.Text, "#90FF90");

            // Execute any dialogue actions
            if (node.Action != null)
            {
                ExecuteDialogueAction(node.Action, currentNPC, currentRoom);
            }

            if (node.Choices.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[#808080][Conversation ends][/]");
                EndDialogue();
                return;
            }

            AnsiConsole.MarkupLine("");
            currentChoices = node.Choices.Where(choice => choice.IsAvailable(player.Inventory)).ToList();

            for (int i = 0; i < currentChoices.Count; i++)
            {
                AnsiConsole.MarkupLine($"[#00FFFF]{i + 1}. {currentChoices[i].choiceText}[/]");
            }
            AnsiConsole.MarkupLine("[#808080]0. End conversation[/]");
            AnsiConsole.MarkupLine("");
            ShowStatusBar();
            AnsiConsole.MarkupLine("[dim](Enter a number to respond)[/]");
        }

        private void ShowStatusBar()
        {
            var player = context.Player;
            int hour = (int)player.CurrentHour;
            int minutes = (int)((player.CurrentHour - hour) * 60);
            string timeOfDay = hour < 12 ? "AM" : "PM";
            int displayHour = hour > 12 ? hour - 12 : hour;
            if (displayHour == 0) displayHour = 12;

            AnsiConsole.MarkupLine($"<span class='stats-bar'>[HP: {player.Health}/{player.MaxHealth} | EP: {player.Energy}/{player.MaxEnergy} | Day {player.CurrentDay}, {displayHour}:{minutes:D2} {timeOfDay} | Gold: {player.Gold} | Recruits: {player.Recruits.Count}/10]</span>");
        }

        public bool ProcessDialogueChoice(string input)
        {
            if (!isInDialogue)
                return false;

            if (!int.TryParse(input, out int choice))
            {
                AnsiConsole.MarkupLine("[#FF0000]Please enter a number.[/]");
                return true; // Still in dialogue, just invalid input
            }

            if (choice == 0)
            {
                AnsiConsole.MarkupLine("\n[#808080]You end the conversation.[/]");
                EndDialogue();
                return true;
            }

            if (choice < 1 || choice > currentChoices.Count)
            {
                AnsiConsole.MarkupLine($"[#FF0000]Please choose a number between 1 and {currentChoices.Count}, or 0 to end the conversation.[/]");
                return true; // Still in dialogue, just invalid input
            }

            var selectedChoice = currentChoices[choice - 1];

            // Handle choice actions (like giving items)
            if (selectedChoice.Action != null)
            {
                ExecuteDialogueAction(selectedChoice.Action, currentNPC, currentRoom);
            }

            currentNode = selectedChoice.nextNodeID;
            AnsiConsole.MarkupLine("");

            ShowCurrentDialogueNode();

            return true; // Input was handled by dialogue system
        }

        private void EndDialogue()
        {
            if (!isInDialogue)
                return;

            // Handle special cases after dialogue ends
            HandlePostDialogueEffects(currentNode, currentNPC, currentRoom);

            currentNPC.CurrentDialogueNode = currentNode;

            // Clear state
            isInDialogue = false;
            currentNPC = null;
            currentRoom = null;
            currentChoices = null;
        }

        private void ExecuteDialogueAction(DialogueAction action, NPC npc, Room currentRoom)
        {
            var player = context.Player;

            switch (action.Type)
            {
                case "give_item":
                    if (action.Parameters.ContainsKey("item"))
                    {
                        string item = action.Parameters["item"].ToString();
                        if (player.Inventory.Contains(item))
                        {
                            player.Inventory.Remove(item);
                            // Item given is handled by dialogue text
                        }
                    }
                    break;

                case "receive_item":
                    if (action.Parameters.ContainsKey("item"))
                    {
                        string item = action.Parameters["item"].ToString();
                        player.Inventory.Add(item);
                        AnsiConsole.MarkupLine($"\n[#00FFFF]You received: {TextHelper.CapitalizeFirst(item)}[/]");
                    }
                    break;

                case "add_recruit":
                    if (action.Parameters.ContainsKey("class"))
                    {
                        string recruitClass = action.Parameters["class"].ToString();
                        Recruit newRecruit = new Recruit(npc.Name, recruitClass, player.CurrentDay);
                        player.Recruits.Add(newRecruit);

                        // Remove NPC from room
                        currentRoom.NPCs.Remove(npc);

                        // AUTO-JOIN PARTY for first 2 recruits
                        if (player.Recruits.Count <= 2 && player.ActiveParty.Count < 3)
                        {
                            player.ActiveParty.Add(newRecruit);
                            AnsiConsole.MarkupLine($"\n[#00FF00]{npc.Name} has joined your party![/]");

                            // Show party tutorial on FIRST recruit
                            if (player.Recruits.Count == 1)
                            {
                                ProgramStatics.messageManager.CheckAndShowMessage("first_party_member", npc.Name);
                            }
                        }
                        else
                        {
                            AnsiConsole.MarkupLine($"\n[#00FF00]{npc.Name} has joined your guild![/]");
                            AnsiConsole.MarkupLine($"[{npc.Name} heads to the guild hall.]");

                            // Show guild tutorial on THIRD recruit (party is now full)
                            if (player.Recruits.Count == 3)
                            {
                                ProgramStatics.messageManager.CheckAndShowMessage("guild_management_unlocked");
                            }
                        }

                        // Still show these milestones
                        if (player.Recruits.Count == 5)
                            ProgramStatics.messageManager.CheckAndShowMessage("halfway_to_goal");
                        else if (player.Recruits.Count == 10)
                            ProgramStatics.messageManager.CheckAndShowMessage("tenth_recruit");

                        // Check win condition
                        if (player.Gold >= 1000 && player.Recruits.Count >= 10)
                            ProgramStatics.messageManager.CheckAndShowMessage("win_conditions_met");

                        recruitmentCallback?.Invoke();
                    }
                    break;

                case "trigger_combat":
                    npc.IsHostile = true;
                    AnsiConsole.MarkupLine($"\n[#FF0000]{npc.Name} attacks![/]");
                    // Combat will be triggered when conversation ends
                    break;
            }
        }

        private void HandlePostDialogueEffects(string finalNode, NPC npc, Room currentRoom)
        {
            // Handle combat trigger for fighter
            if (finalNode == "reject_guild" && npc.Name == "Braxus")
            {
                npc.IsHostile = true;
                // Combat manager will handle this when we return to game loop
            }
        }
    }

    // Update the DialogueNode model
    public static class DialogueNodeExtensions
    {
        public static DialogueNode WithAction(this DialogueNode node, DialogueAction action)
        {
            node.Action = action;
            return node;
        }
    }

    // Add to Models/DialogueNode.cs or create new file
    public class DialogueAction
    {
        public string Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public DialogueAction()
        {
            Parameters = new Dictionary<string, object>();
        }
    }
}