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
        private Action<NPC>? openShopCallback;  // Callback to open shop for a vendor

        // State for non-blocking dialogue
        private NPC? currentNPC;
        private Room? currentRoom;
        private string? currentNode;
        private List<DialogueNode.Choice>? currentChoices;
        private bool isInDialogue = false;

        // Event dialogue trees (not tied to NPCs)
        private Dictionary<string, Dictionary<string, DialogueNode>> eventDialogueTrees;
        private bool isEventDialogue = false;  // Track if current dialogue is from an event
        private string? currentDialogueTreeId = null;  // Track current event dialogue tree ID

        public DialogueManager(GameContext gameContext, Action onRecruitmentCallback = null)
        {
            context = gameContext;
            recruitmentCallback = onRecruitmentCallback;
            eventDialogueTrees = new Dictionary<string, Dictionary<string, DialogueNode>>();
        }

        public void SetOpenShopCallback(Action<NPC> callback)
        {
            openShopCallback = callback;
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

            // Determine starting node with first_greeting/repeat_greeting support
            if (!string.IsNullOrEmpty(npc.CurrentDialogueNode) &&
                npc.Dialogue.ContainsKey(npc.CurrentDialogueNode))
            {
                currentNode = npc.CurrentDialogueNode;
            }
            else
            {
                // Check if this is the first time meeting this NPC
                bool isFirstMeeting = !context.Player.MetNPCs.Contains(npc.Name);

                if (isFirstMeeting && npc.Dialogue.ContainsKey("first_greeting"))
                {
                    currentNode = "first_greeting";
                    context.Player.MetNPCs.Add(npc.Name);
                }
                else if (npc.Dialogue.ContainsKey("repeat_greeting"))
                {
                    currentNode = "repeat_greeting";
                }
                else
                {
                    currentNode = "greeting";
                }
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

            // Track that this node has been visited
            if (!player.VisitedDialogueNodes.ContainsKey(currentNPC.Name))
            {
                player.VisitedDialogueNodes[currentNPC.Name] = new HashSet<string>();
            }
            player.VisitedDialogueNodes[currentNPC.Name].Add(currentNode);

            // Substitute placeholders in dialogue text
            string displayText = SubstituteDialogueText(node.Text);
            TextHelper.DisplayTextWithPaging(displayText, "#90FF90");

            // Execute any dialogue actions
            if (node.Action != null)
            {
                ExecuteDialogueAction(node.Action, currentNPC, currentRoom);
            }

            // Build choices list first - filter by inventory AND topic tracking
            currentChoices = node.Choices.Where(choice =>
                choice.IsAvailable(player.Inventory) &&
                IsChoiceAvailableByTopicTracking(choice, currentNPC.Name)).ToList();

            // Auto-inject shop option for vendors
            if (currentNPC.IsVendor)
            {
                var shopChoice = new DialogueNode.Choice
                {
                    choiceText = "I'd like to see your wares.",
                    nextNodeID = "open_shop" // Special marker for shop
                };
                currentChoices.Add(shopChoice);
            }

            // NOW check if there are any choices (after vendor shop injection)
            if (currentChoices.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[#808080](Conversation ends)[/]");
                EndDialogue();
                return;
            }

            AnsiConsole.MarkupLine("");

            for (int i = 0; i < currentChoices.Count; i++)
            {
                // Substitute placeholders in choice text
                string displayChoice = SubstituteDialogueText(currentChoices[i].choiceText);

                // Wrap the choice text
                string wrappedChoice = Helpers.TextHelper.WrapText(displayChoice);
                string[] choiceLines = wrappedChoice.Split('\n');

                // Display first line with number prefix
                AnsiConsole.MarkupLine($"[#00FFFF]{i + 1}. {choiceLines[0]}[/]");

                // Display subsequent lines with indentation to align with first line
                for (int j = 1; j < choiceLines.Length; j++)
                {
                    AnsiConsole.MarkupLine($"[#00FFFF]   {choiceLines[j]}[/]");
                }
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

            AnsiConsole.MarkupLine($"\n<span class='stats-bar'>[HP: {player.Health}/{player.MaxHealth} | EP: {player.Energy}/{player.MaxEnergy} | Day {player.CurrentDay}, {displayHour}:{minutes:D2} {timeOfDay} | Gold: {player.Gold} | Recruits: {player.Recruits.Count}/10]</span>");
        }

        /// <summary>
        /// Substitutes placeholders in dialogue text with actual player/game values
        /// Supported placeholders: {player.name}, {player.class}, {player.level}, {npc.name}
        /// </summary>
        private string SubstituteDialogueText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var player = context.Player;

            // Replace player placeholders (case-insensitive)
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\{player\.name\}", player.Name, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\{player\.class\}", player.Class?.Name ?? "Adventurer", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            text = System.Text.RegularExpressions.Regex.Replace(text, @"\{player\.level\}", player.Level.ToString(), System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Replace NPC placeholders if we have a current NPC
            if (currentNPC != null)
            {
                text = System.Text.RegularExpressions.Regex.Replace(text, @"\{npc\.name\}", currentNPC.Name, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            return text;
        }

        /// <summary>
        /// Checks if a dialogue choice is available based on topic tracking conditions
        /// </summary>
        private bool IsChoiceAvailableByTopicTracking(DialogueNode.Choice choice, string npcName)
        {
            var player = context.Player;

            // If no topic tracking conditions, always available
            if (string.IsNullOrEmpty(choice.RequireDiscussedNode) &&
                string.IsNullOrEmpty(choice.RequireNotDiscussedNode))
            {
                return true;
            }

            // Get visited nodes for this NPC
            HashSet<string> visitedNodes = player.VisitedDialogueNodes.ContainsKey(npcName)
                ? player.VisitedDialogueNodes[npcName]
                : new HashSet<string>();

            // Check RequireDiscussedNode condition
            if (!string.IsNullOrEmpty(choice.RequireDiscussedNode))
            {
                if (!visitedNodes.Contains(choice.RequireDiscussedNode))
                {
                    return false;  // Required node not visited
                }
            }

            // Check RequireNotDiscussedNode condition
            if (!string.IsNullOrEmpty(choice.RequireNotDiscussedNode))
            {
                if (visitedNodes.Contains(choice.RequireNotDiscussedNode))
                {
                    return false;  // Node was visited, but we require it NOT to be
                }
            }

            return true;
        }

        public bool ProcessDialogueChoice(string input)
        {
            if (!isInDialogue)
                return false;

            // Handle event dialogue separately
            if (isEventDialogue && !string.IsNullOrEmpty(currentDialogueTreeId))
            {
                ProcessEventDialogueChoice(input, currentDialogueTreeId);
                return true;
            }

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

            // Handle special "open_shop" choice for vendors
            if (selectedChoice.nextNodeID == "open_shop")
            {
                // End dialogue and open shop
                var vendorNPC = currentNPC;
                EndDialogue();

                // Open the shop via callback
                if (openShopCallback != null)
                {
                    openShopCallback(vendorNPC);
                }
                else
                {
                    AnsiConsole.MarkupLine("[#FF0000]Shop system is not available.[/]");
                }
                return true;
            }

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

            // Handle special cases after dialogue ends (only for NPC dialogue)
            if (!isEventDialogue && currentNPC != null)
            {
                HandlePostDialogueEffects(currentNode, currentNPC, currentRoom);

                // NEW BEHAVIOR: Only save dialogue position if node explicitly marks itself as permanent
                // Otherwise, always reset to greeting (first_greeting or repeat_greeting)
                if (!string.IsNullOrEmpty(currentNode) &&
                    currentNPC.Dialogue.ContainsKey(currentNode) &&
                    currentNPC.Dialogue[currentNode].PermanentlyEndsDialogue)
                {
                    // This node locks the dialogue permanently
                    currentNPC.CurrentDialogueNode = currentNode;
                }
                else
                {
                    // Default: Reset to greeting (will use first_greeting or repeat_greeting)
                    currentNPC.CurrentDialogueNode = null;
                }
            }

            // Clear state
            isInDialogue = false;
            isEventDialogue = false;
            currentDialogueTreeId = null;
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

                case "open_gate":
                    // Open the gate from Room 69 (Belum Approach) to Room 70 (Belum Town Square)
                    if (context.Rooms.ContainsKey(69))
                    {
                        if (!context.Rooms[69].Exits.ContainsKey("north"))
                        {
                            context.Rooms[69].Exits.Add("north", 70);
                            // Update room description to reflect the open gate
                            context.Rooms[69].Description = "The town walls of Belum rise before you, built of weathered grey stone. Guards patrol the battlements above. The main gate stands open, its heavy iron portcullis raised. A veteran guard named Marcus stands watch, nodding respectfully as you approach.";
                            AnsiConsole.MarkupLine($"\n[#00FF00]You hear the sound of the great gate creaking open...[/]");
                        }
                    }
                    // Update Marcus to use "after_quest" dialogue from now on
                    npc.CurrentDialogueNode = "after_quest";
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

        /// <summary>
        /// Registers an event dialogue tree for use by the event system
        /// </summary>
        public void RegisterEventDialogue(string treeId, Dictionary<string, DialogueNode> dialogueTree)
        {
            if (string.IsNullOrEmpty(treeId) || dialogueTree == null)
                return;

            eventDialogueTrees[treeId] = dialogueTree;
        }

        /// <summary>
        /// Starts dialogue using an event dialogue tree ID (without requiring an NPC)
        /// </summary>
        public void StartEventDialogue(string dialogueTreeId, string startNode = "start")
        {
            if (string.IsNullOrEmpty(dialogueTreeId))
                return;

            if (!eventDialogueTrees.ContainsKey(dialogueTreeId))
            {
                AnsiConsole.MarkupLine($"[#FF0000]Event dialogue tree '{dialogueTreeId}' not found.[/]");
                return;
            }

            currentNPC = null;  // No NPC for event dialogue
            currentRoom = context.Rooms[context.Player.CurrentRoom];
            isInDialogue = true;
            isEventDialogue = true;
            currentDialogueTreeId = dialogueTreeId;
            currentNode = startNode;

            AnsiConsole.MarkupLine("");  // Add spacing
            ShowEventDialogueNode(dialogueTreeId);
        }

        /// <summary>
        /// Shows the current dialogue node for event dialogue
        /// </summary>
        private void ShowEventDialogueNode(string dialogueTreeId)
        {
            var player = context.Player;

            if (!eventDialogueTrees[dialogueTreeId].ContainsKey(currentNode))
            {
                AnsiConsole.MarkupLine("Something went wrong with the event dialogue.");
                EndDialogue();
                return;
            }

            DialogueNode node = eventDialogueTrees[dialogueTreeId][currentNode];

            // Track that this node has been visited (use dialogueTreeId as the "NPC name" for events)
            if (!player.VisitedDialogueNodes.ContainsKey(dialogueTreeId))
            {
                player.VisitedDialogueNodes[dialogueTreeId] = new HashSet<string>();
            }
            player.VisitedDialogueNodes[dialogueTreeId].Add(currentNode);

            // Substitute placeholders in event dialogue text
            string displayText = SubstituteDialogueText(node.Text);
            TextHelper.DisplayTextWithPaging(displayText, "#90FF90");

            // Execute any dialogue actions
            if (node.Action != null)
            {
                ExecuteDialogueAction(node.Action, null, currentRoom);
            }

            // Build choices list - filter by inventory AND topic tracking
            currentChoices = node.Choices.Where(choice =>
                choice.IsAvailable(player.Inventory) &&
                IsChoiceAvailableByTopicTracking(choice, dialogueTreeId)).ToList();

            // Check if there are any choices
            if (currentChoices.Count == 0)
            {
                if (context.Player.DebugLogsEnabled)
                {
                    AnsiConsole.MarkupLine("\n[#808080](Event concludes)[/]");
                }
                EndDialogue();
                return;
            }

            AnsiConsole.MarkupLine("");

            for (int i = 0; i < currentChoices.Count; i++)
            {
                // Substitute placeholders in choice text
                string displayChoice = SubstituteDialogueText(currentChoices[i].choiceText);

                // Wrap the choice text
                string wrappedChoice = Helpers.TextHelper.WrapText(displayChoice);
                string[] choiceLines = wrappedChoice.Split('\n');

                // Display first line with number prefix
                AnsiConsole.MarkupLine($"[#00FFFF]{i + 1}. {choiceLines[0]}[/]");

                // Display subsequent lines with indentation to align with first line
                for (int j = 1; j < choiceLines.Length; j++)
                {
                    AnsiConsole.MarkupLine($"[#00FFFF]   {choiceLines[j]}[/]");
                }
            }
            AnsiConsole.MarkupLine("[#808080]0. Continue[/]");
            AnsiConsole.MarkupLine("");
            ShowStatusBar();
            AnsiConsole.MarkupLine("[dim](Enter a number to respond)[/]");
        }

        /// <summary>
        /// Processes dialogue choice for event dialogue
        /// Must be called from the existing ProcessDialogueChoice when isEventDialogue is true
        /// </summary>
        private void ProcessEventDialogueChoice(string input, string dialogueTreeId)
        {
            if (!int.TryParse(input, out int choice))
            {
                AnsiConsole.MarkupLine("[#FF0000]Please enter a number.[/]");
                return;
            }

            if (choice == 0)
            {
                AnsiConsole.MarkupLine("\n[#808080]You continue...[/]");
                EndDialogue();
                return;
            }

            if (choice < 1 || choice > currentChoices.Count)
            {
                AnsiConsole.MarkupLine($"[#FF0000]Please choose a number between 1 and {currentChoices.Count}, or 0 to continue.[/]");
                return;
            }

            var selectedChoice = currentChoices[choice - 1];

            // Handle choice actions
            if (selectedChoice.Action != null)
            {
                ExecuteDialogueAction(selectedChoice.Action, null, currentRoom);
            }

            currentNode = selectedChoice.nextNodeID;
            AnsiConsole.MarkupLine("");

            ShowEventDialogueNode(dialogueTreeId);
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