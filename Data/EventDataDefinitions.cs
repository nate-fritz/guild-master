using System.Collections.Generic;
using GuildMaster.Models;
using GuildMaster.Managers;

namespace GuildMaster.Data
{
    /// <summary>
    /// Contains sample event definitions demonstrating the event system capabilities
    /// </summary>
    public static class EventDataDefinitions
    {
        /// <summary>
        /// Returns a list of all defined events
        /// Call this from EventManager.LoadEvents() to load events into the game
        /// </summary>
        public static List<EventData> GetAllEvents()
        {
            var events = new List<EventData>();

            // Add all event definitions
            events.Add(CreateGuildHallWelcomeEvent());
            events.Add(CreateTenthRecruitCelebrationEvent());
            // events.Add(CreateAssassinationEvent());  // Example from prompt - requires specific rooms/dialogue

            return events;
        }

        /// <summary>
        /// Example: Simple welcome event when first entering the Guild Hall Common Area
        /// </summary>
        private static EventData CreateGuildHallWelcomeEvent()
        {
            return new EventData("guild_hall_welcome", 4)  // Room 4 is Common Area
            {
                Priority = 10,
                IsOneTime = true,
                DialogueTreeId = "guild_hall_welcome_dialogue",  // Reference to dialogue tree
                Conditions = new List<EventCondition>
                {
                    // Triggers only on first visit to this room
                    new EventCondition(ConditionType.FirstVisit, "guild_hall_welcome", true)
                },
                Actions = new List<EventAction>
                {
                    // Set a quest flag to track that we've been welcomed
                    new EventAction(ActionType.SetQuestFlag)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "flagId", "guild_hall_visited" },
                            { "value", true }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Example: Celebration event when recruiting the 10th guild member
        /// Demonstrates recruit count condition and multiple actions
        /// </summary>
        private static EventData CreateTenthRecruitCelebrationEvent()
        {
            return new EventData("tenth_recruit_celebration", 4)  // Room 4 is Common Area
            {
                Priority = 100,
                IsOneTime = true,
                DialogueTreeId = "",  // Could add dialogue here for a celebration scene
                Conditions = new List<EventCondition>
                {
                    // Only trigger when player has exactly 10 recruits (or more)
                    new EventCondition(ConditionType.MinRecruitCount, "", true, 10),
                    // And hasn't triggered this event before
                    new EventCondition(ConditionType.FirstVisit, "tenth_recruit_celebration", true)
                },
                Actions = new List<EventAction>
                {
                    // Grant bonus gold for achievement
                    new EventAction(ActionType.GrantGold)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "amount", 100 }
                        }
                    },
                    // Set quest flag
                    new EventAction(ActionType.SetQuestFlag)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "flagId", "legendary_guild_achieved" },
                            { "value", true }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Example from prompt: Complex assassination cutscene with dialogue and forced travel
        /// NOTE: This requires custom room IDs and dialogue tree that may not exist in your game
        /// Uncomment and customize for your specific game content
        /// </summary>
        private static EventData CreateAssassinationEvent()
        {
            return new EventData("aevoria_assassination", 312)  // Example: Room 312 - Throne Room
            {
                Priority = 100,
                IsOneTime = true,
                DialogueTreeId = "assassination_scene",  // Must be registered with DialogueManager
                Conditions = new List<EventCondition>
                {
                    // Requires quest flag to be set (e.g., "cult_outpost_cleared")
                    new EventCondition(ConditionType.QuestFlagSet, "cult_outpost_cleared", true),
                    // First time visiting after the flag is set
                    new EventCondition(ConditionType.FirstVisit, "aevoria_assassination", true)
                },
                Actions = new List<EventAction>
                {
                    // Set quest flag indicating assassination happened
                    new EventAction(ActionType.SetQuestFlag)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "flagId", "certius_assassinated" },
                            { "value", true }
                        }
                    },
                    // Force player to travel to different room (escape sequence)
                    new EventAction(ActionType.ForceTravel)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "targetRoomId", 314 },  // Example: Room 314 - Hidden Passage
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Example: Event that grants an item when player has enough gold
        /// </summary>
        private static EventData CreateMerchantGiftEvent()
        {
            return new EventData("merchant_gift", 10)  // Example room ID
            {
                Priority = 50,
                IsOneTime = true,
                DialogueTreeId = "",
                Conditions = new List<EventCondition>
                {
                    // Requires at least 500 gold
                    new EventCondition(ConditionType.MinGold, "", true, 500),
                    // First visit to this room with enough gold
                    new EventCondition(ConditionType.FirstVisit, "merchant_gift", true)
                },
                Actions = new List<EventAction>
                {
                    // Grant a special item
                    new EventAction(ActionType.GrantItem)
                    {
                        Parameters = new Dictionary<string, object>
                        {
                            { "itemId", "merchant's token" }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Example: Demonstrates how to register event dialogue trees
        /// Call this from GameEngine after DialogueManager is created
        /// </summary>
        public static void RegisterEventDialogueTrees(DialogueManager dialogueManager)
        {
            // Register guild hall welcome dialogue
            var guildHallWelcomeDialogue = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode
                {
                    Text = "As you step into the common area of the guild hall for the first time, you pause to take it in.<br><br>It is far too large a space for just one person, but you briefly imagine what this place felt like when it was full of brothers and sisters in arms, united in a common purpose.<br><br>You had never planned for anything like this, but the idea suddenly feels like one worth working towards.",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "I'll be back soon..  and not alone.",
                            nextNodeID = "end"
                        }
                    }
                },
                ["end"] = new DialogueNode
                {
                    Text = "You hang on to this feeling for just a moment longer, then get back to the task at hand.",
                    Choices = new List<DialogueNode.Choice>() // No choices - dialogue ends
                }
            };
            dialogueManager.RegisterEventDialogue("guild_hall_welcome_dialogue", guildHallWelcomeDialogue);

            // Example: Register assassination scene dialogue
            var assassinationDialogue = new Dictionary<string, DialogueNode>
            {
                ["start"] = new DialogueNode
                {
                    Text = "As you enter the throne room, you witness a shocking scene unfold before your eyes...<br><br>The emperor collapses, and chaos erupts around you!",
                    Choices = new List<DialogueNode.Choice>
                    {
                        new DialogueNode.Choice
                        {
                            choiceText = "Try to help the emperor",
                            nextNodeID = "help_attempt"
                        },
                        new DialogueNode.Choice
                        {
                            choiceText = "Look for the assassin",
                            nextNodeID = "search_assassin"
                        }
                    }
                },
                ["help_attempt"] = new DialogueNode
                {
                    Text = "Before you can reach the emperor, Quintus grabs your arm.<br><br>\"It's too late! We need to get you out of here - NOW!\"",
                    Choices = new List<DialogueNode.Choice>() // No choices - will end dialogue
                },
                ["search_assassin"] = new DialogueNode
                {
                    Text = "You scan the crowd frantically, but the assassin has vanished into the chaos.<br><br>Quintus appears at your side, pulling you toward a hidden exit.",
                    Choices = new List<DialogueNode.Choice>() // No choices - will end dialogue
                }
            };

            dialogueManager.RegisterEventDialogue("assassination_scene", assassinationDialogue);
        }
    }
}
