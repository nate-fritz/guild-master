# GuildMaster - Content Creation Guide (SOP)

**Purpose:** Standard Operating Procedures for adding new content to GuildMaster
**Audience:** Content creators, developers, or AI assistants adding game content
**Last Updated:** 2025-12-24

---

## TABLE OF CONTENTS
1. [Adding New Events/Cutscenes](#1-adding-new-eventscutscenes)
2. [Adding New NPCs](#2-adding-new-npcs)
3. [Adding New Rooms](#3-adding-new-rooms)
4. [Adding Dialogue Trees](#4-adding-dialogue-trees)
5. [Adding Tutorial Messages](#5-adding-tutorial-messages)
6. [Adding Quests](#6-adding-quests)
7. [Adding Items](#7-adding-items)
8. [Testing Your Content](#8-testing-your-content)

---

## 1. ADDING NEW EVENTS/CUTSCENES

**Events trigger automatically when entering a room under specific conditions**

### File Location
`/home/sinogue/GuildMaster/Data/EventDataDefinitions.cs`

### Step-by-Step Process

#### Step 1: Create Event Definition Method
```csharp
private static EventData CreateYourEventName()
{
    return new EventData("unique_event_id", ROOM_ID)  // Use room numeric ID
    {
        Priority = 50,  // Higher = triggers first if multiple events
        IsOneTime = true,  // false = can trigger multiple times
        DialogueTreeId = "dialogue_tree_id",  // Optional - see dialogue section
        Conditions = new List<EventCondition>
        {
            // Add conditions (see below)
        },
        Actions = new List<EventAction>
        {
            // Add actions (see below)
        }
    };
}
```

#### Step 2: Add to GetAllEvents()
```csharp
public static List<EventData> GetAllEvents()
{
    var events = new List<EventData>();

    events.Add(CreateGuildHallWelcomeEvent());
    events.Add(CreateTenthRecruitCelebrationEvent());
    events.Add(CreateYourEventName());  // ADD YOUR EVENT HERE

    return events;
}
```

### Event Conditions Reference

#### Available Condition Types
```csharp
// First time entering this room
new EventCondition(ConditionType.FirstVisit, "event_id", true)

// First visit AFTER a quest flag is set
new EventCondition(ConditionType.FirstVisitAfterFlag, "quest_flag_name", true)

// Check if quest flag is true/false
new EventCondition(ConditionType.QuestFlagSet, "flag_name", true)  // or false

// Player has/doesn't have item
new EventCondition(ConditionType.HasItem, "item_name", true)  // or false

// Quest completed/not completed
new EventCondition(ConditionType.QuestCompleted, "quest_id", true)  // or false

// Quest active/not active
new EventCondition(ConditionType.QuestActive, "quest_id", true)  // or false

// Minimum recruit count
new EventCondition(ConditionType.MinRecruitCount, "", true, 5)  // 5+ recruits

// Minimum gold
new EventCondition(ConditionType.MinGold, "", true, 100)  // 100+ gold

// Minimum level
new EventCondition(ConditionType.MinLevel, "", true, 5)  // Level 5+
```

### Event Actions Reference

#### Available Action Types
```csharp
// Set a quest flag
new EventAction(ActionType.SetQuestFlag)
{
    Parameters = new Dictionary<string, object>
    {
        { "flagId", "flag_name" },
        { "value", true }  // or false
    }
}

// Give item to player
new EventAction(ActionType.GrantItem)
{
    Parameters = new Dictionary<string, object>
    {
        { "itemId", "item_name" }
    }
}

// Remove item from player
new EventAction(ActionType.RemoveItem)
{
    Parameters = new Dictionary<string, object>
    {
        { "itemId", "item_name" }
    }
}

// Give gold
new EventAction(ActionType.GrantGold)
{
    Parameters = new Dictionary<string, object>
    {
        { "amount", 100 }
    }
}

// Take gold
new EventAction(ActionType.RemoveGold)
{
    Parameters = new Dictionary<string, object>
    {
        { "amount", 50 }
    }
}

// Force player to different room (with message)
new EventAction(ActionType.ForceTravel)
{
    Parameters = new Dictionary<string, object>
    {
        { "targetRoomId", 314 }
    }
}

// Add recruit to active party
new EventAction(ActionType.AddPartyMember)
{
    Parameters = new Dictionary<string, object>
    {
        { "recruitName", "Silvacis" }
    }
}

// Remove recruit from active party
new EventAction(ActionType.RemovePartyMember)
{
    Parameters = new Dictionary<string, object>
    {
        { "recruitName", "Silvacis" }
    }
}

// Spawn NPC in room
new EventAction(ActionType.SpawnNPC)
{
    Parameters = new Dictionary<string, object>
    {
        { "npcName", "Merchant" },
        { "roomId", 10 }
    }
}

// Remove NPC from room
new EventAction(ActionType.RemoveNPC)
{
    Parameters = new Dictionary<string, object>
    {
        { "npcName", "Merchant" },
        { "roomId", 10 }
    }
}
```

### Complete Event Example
```csharp
private static EventData CreateTreasureDiscovery()
{
    return new EventData("forest_treasure_found", 45)  // Room 45
    {
        Priority = 100,
        IsOneTime = true,
        DialogueTreeId = "treasure_discovery_dialogue",
        Conditions = new List<EventCondition>
        {
            new EventCondition(ConditionType.HasItem, "rusty key", true),
            new EventCondition(ConditionType.FirstVisit, "forest_treasure_found", true)
        },
        Actions = new List<EventAction>
        {
            new EventAction(ActionType.GrantItem)
            {
                Parameters = new Dictionary<string, object>
                {
                    { "itemId", "ancient sword" }
                }
            },
            new EventAction(ActionType.SetQuestFlag)
            {
                Parameters = new Dictionary<string, object>
                {
                    { "flagId", "found_ancient_sword" },
                    { "value", true }
                }
            }
        }
    };
}
```

### Registering Event Dialogue

**If your event has DialogueTreeId, register it:**

```csharp
public static void RegisterEventDialogueTrees(DialogueManager dialogueManager)
{
    // ... existing registrations ...

    // Add your dialogue
    var yourDialogue = new Dictionary<string, DialogueNode>
    {
        ["start"] = new DialogueNode
        {
            Text = "Your dialogue text here.<br><br>Use <br><br> for paragraph breaks.",
            Choices = new List<DialogueNode.Choice>
            {
                new DialogueNode.Choice
                {
                    choiceText = "Continue",
                    nextNodeID = "end"
                }
            }
        },
        ["end"] = new DialogueNode
        {
            Text = "Final text.",
            Choices = new List<DialogueNode.Choice>()  // Empty = dialogue ends
        }
    };

    dialogueManager.RegisterEventDialogue("your_dialogue_tree_id", yourDialogue);
}
```

---

## 2. ADDING NEW NPCs

**NPCs are characters the player can interact with (talk, fight, recruit)**

### File Location
`/home/sinogue/GuildMaster/Data/NPCData.cs`

### Step-by-Step Process

#### Step 1: Create NPC Definition
```csharp
// Add to InitializeNPCs() method
public static Dictionary<string, NPC> InitializeNPCs()
{
    var npcs = new Dictionary<string, NPC>();

    // ... existing NPCs ...

    // YOUR NEW NPC
    NPC yourNPC = new NPC
    {
        Name = "NPC Name",
        ShortDescription = "A brief one-line description",
        Description = "Full description when examined.<br><br>Can have multiple paragraphs.",

        // Combat stats (if enemy or recruitable)
        MaxHealth = 20,
        Health = 20,
        MaxEnergy = 10,
        Energy = 10,
        AttackDamage = 5,
        Defense = 2,
        Speed = 3,
        Level = 1,

        // Equipment (optional)
        EquippedWeapon = EquipmentData.GetEquipment("iron sword"),
        EquippedArmor = EquipmentData.GetEquipment("leather armor"),

        // Behavior flags
        IsHostile = false,  // true = attacks on sight
        IsRecrutable = false,  // true = can join guild
        IsMerchant = false,  // true = runs a shop

        // Dialogue (see dialogue section)
        CurrentDialogueNode = "greeting",
        Dialogue = new Dictionary<string, DialogueNode>()
    };

    npcs.Add("NPC Name", yourNPC);

    return npcs;
}
```

### NPC Types

#### Friendly NPC (Dialogue Only)
```csharp
NPC merchant = new NPC
{
    Name = "Friendly Merchant",
    ShortDescription = "A cheerful trader",
    Description = "A portly merchant with a wide smile.",
    IsHostile = false,
    IsRecrutable = false,
    IsMerchant = true,
    CurrentDialogueNode = "greeting",
    Dialogue = new Dictionary<string, DialogueNode>()
};
```

#### Enemy NPC
```csharp
NPC bandit = new NPC
{
    Name = "Bandit",
    ShortDescription = "A rough-looking brigand",
    Description = "A dangerous criminal armed with a rusty blade.",
    MaxHealth = 25,
    Health = 25,
    MaxEnergy = 8,
    Energy = 8,
    AttackDamage = 6,
    Defense = 1,
    Speed = 4,
    IsHostile = true,
    EquippedWeapon = EquipmentData.GetEquipment("rusty dagger"),
    Abilities = new List<Ability>
    {
        AbilityData.GetAbility("power_strike")
    }
};
```

#### Recruitable NPC
```csharp
NPC ranger = new NPC
{
    Name = "Silvacis",
    ShortDescription = "A skilled ranger",
    Description = "An elven ranger with keen eyes and a ready bow.",
    MaxHealth = 18,
    Health = 18,
    MaxEnergy = 12,
    Energy = 12,
    AttackDamage = 5,
    Defense = 1,
    Speed = 6,
    IsRecrutable = true,
    RecruitmentQuestId = "find_amulet",  // Optional - quest required
    Class = new Venator(),  // Character class
    EquippedWeapon = EquipmentData.GetEquipment("hunter's bow"),
    Abilities = new List<Ability>
    {
        AbilityData.GetAbility("quick_shot"),
        AbilityData.GetAbility("multi_shot")
    },
    CurrentDialogueNode = "greeting",
    Dialogue = new Dictionary<string, DialogueNode>()
};
```

### Adding NPC to a Room

**In RoomData.cs:**
```csharp
// For unique NPCs (only one in game)
room.NPCs.Add(npcs["NPC Name"]);

// For generic NPCs that can appear multiple times
room.NPCs.Add(npcs["Bandit"].Clone());
```

---

## 3. ADDING NEW ROOMS

**Rooms are locations the player can explore**

### File Location
`/home/sinogue/GuildMaster/Data/RoomData.cs`

### Step-by-Step Process

#### Step 1: Create Room Definition
```csharp
// In InitializeRooms() method
public static Dictionary<int, Room> InitializeRooms(Dictionary<string, NPC> npcs)
{
    var rooms = new Dictionary<int, Room>();

    // ... existing rooms ...

    // YOUR NEW ROOM
    Room yourRoom = CreateRoom(
        999,  // Unique room ID number
        "uniqueRoomKey",  // Unique string identifier
        "Room Title",  // Display title
        "Room description. This is what the player sees when entering.<br><br>Use <br><br> for paragraph breaks."
    );

    // Add exits
    yourRoom.Exits.Add("north", 100);  // north leads to room 100
    yourRoom.Exits.Add("south", 101);  // south leads to room 101
    yourRoom.Exits.Add("east", 102);
    yourRoom.Exits.Add("west", 103);
    yourRoom.Exits.Add("up", 104);
    yourRoom.Exits.Add("down", 105);

    // Add items (floor loot)
    yourRoom.Items.Add("potion");
    yourRoom.Items.Add("rusty sword");

    // Add NPCs
    yourRoom.NPCs.Add(npcs["Friendly NPC"]);
    yourRoom.NPCs.Add(npcs["Enemy"].Clone());  // Clone for multiples

    // Add to dictionary
    rooms.Add(999, yourRoom);

    return rooms;
}
```

#### Step 2: Connect to Existing Rooms
```csharp
// Find the room that should connect to yours
Room existingRoom = CreateRoom(100, "existingRoom", "Old Room", "Description");
existingRoom.Exits.Add("south", 999);  // Add exit to your new room
```

### Room Features

#### Dynamic Descriptions (Based on Conditions)
```csharp
Room commonArea = CreateRoom(4, "commonArea", "Common Area", "Default description");

// Add variants based on game state
commonArea.DescriptionVariants.Add("recruits_4", "Description when 4+ recruits");
commonArea.DescriptionVariants.Add("recruits_10", "Description when 10+ recruits");
commonArea.DescriptionVariants.Add("quest_completed", "Description after quest");
```

#### Progressive/Unlockable Rooms
```csharp
// Room that unlocks based on recruit count
Room secretRoom = CreateRoom(65, "secretRoom", "Secret Room", "Hidden chamber");
secretRoom.Exits.Add("west", 4);  // Exit back

// In the connecting room (Room 4), add conditional exit
// This is handled by GameController checking recruit count
// The exit appears/disappears automatically
```

### Adding Item Descriptions for Room Items

**If room has lootable items, add to ItemData.cs:**
```csharp
// In InitializeItemDescriptions()
{999, new Dictionary<string, Item>()  // Your room ID
    {
        {"potion", new Item {
            Description = "A small healing potion.",
            ShortName = "potion",
            IsLootable = true,
            IsConsumable = true,
            EffectId = "lesser_healing"
        }},
        {"rusty sword", new Item {
            Description = "An old rusty sword.",
            ShortName = "sword",
            IsLootable = true
        }}
    }
}
```

---

## 4. ADDING DIALOGUE TREES

**Dialogue trees control NPC conversations with choices**

### File Location
`/home/sinogue/GuildMaster/Data/NPCData.cs` (for NPC dialogue)
`/home/sinogue/GuildMaster/Data/EventDataDefinitions.cs` (for event dialogue)

### Dialogue Writing Style

**IMPORTANT: All dialogue should use quotation marks and narrative descriptions:**

```csharp
// ✅ CORRECT - Narrative outside quotes, dialogue inside quotes
Text = "Gaius smiles warmly. \"Hello there, friend. Welcome to Belum!\""

// ❌ WRONG - No narrative description, no quotation marks
Text = "Hello there, friend. Welcome to Belum!"
```

**Format:**
- **Narrative action/description** (outside quotes) + **actual spoken dialogue** (inside \" quotes)
- Use `\"` to escape quotes in C# strings
- Add character actions, expressions, and body language outside the quotes
- Only put the words they actually speak inside the quotes

**Examples:**
```csharp
Text = "The guard glances at you briefly. \"Move along, citizen.\""
Text = "Silvacis' eyes widen with interest. \"The old guild hall? Interesting...\""
Text = "Marcus scowls. \"Bandits. A large group has made camp in caves to the southwest.\""
```

### Structure
```
Dialogue Tree
  ├─ Node "first_greeting" (first time meeting)
  │   ├─ Text: "He rises to meet you. \"Greetings! Name's Gaius.\""
  │   ├─ Choice 1: "Who are you?" → Node "about"
  │   ├─ Choice 2: "Goodbye" → Node "end"
  │
  ├─ Node "repeat_greeting" (subsequent meetings)
  │   ├─ Text: "Gaius greets you. \"Good to see you again!\""
  │   ├─ Choice 1: "About that quest..." → Node "quest" (only if not discussed)
  │   ├─ Choice 2: "Goodbye" → Node "end"
  │
  ├─ Node "greeting" (fallback/backward compatibility)
  │   ├─ Text: "He greets you. \"Hello, traveler!\""
  │   └─ (Same as first_greeting)
  │
  └─ Node "end"
      ├─ Text: "He waves. \"Safe travels!\""
      └─ (No choices = conversation ends)
```

### First Meeting vs. Repeat Greetings

**NPCs should introduce themselves on first meeting, then use shorter greetings afterward.**

The dialogue system automatically tracks which NPCs the player has met. Use these node names:
- `first_greeting` - Shown the first time player talks to this NPC
- `repeat_greeting` - Shown on all subsequent conversations
- `greeting` - Fallback if first/repeat not defined

**Example - Gaius the Farmer:**
```csharp
// First time meeting - full introduction
farmer.Dialogue.Add("first_greeting", new DialogueNode()
{
    Text = "As you approach the farmer's stand, he rises to meet you with a smile.<br><br>\"Greetings, friend. Haven't seen you 'round these parts before. Name's Gaius.\"",
    Choices =
    {
        new DialogueNode.Choice { choiceText = "\"Where am I?\"", nextNodeID = "ask_about_area" },
        new DialogueNode.Choice { choiceText = "\"I should get going.\"", nextNodeID = "end" }
    }
});

// Subsequent meetings - acknowledges prior meeting
farmer.Dialogue.Add("repeat_greeting", new DialogueNode()
{
    Text = "Gaius looks up from arranging his wares and greets you with a familiar smile.<br><br>\"Good to see you again, {player.name}. What can I do for you?\"",
    Choices =
    {
        new DialogueNode.Choice { choiceText = "\"About that forest...\"", nextNodeID = "ask_about_forest" },
        new DialogueNode.Choice { choiceText = "\"Just passing through.\"", nextNodeID = "end" }
    }
});
```

### Topic Tracking and Acknowledgment

**NPCs can acknowledge topics you've already discussed.**

Use `RequireDiscussedNode` and `RequireNotDiscussedNode` on choices to show/hide them based on conversation history:

```csharp
farmer.Dialogue.Add("repeat_greeting", new DialogueNode()
{
    Text = "Gaius greets you warmly. \"Back again? What can I help with?\"",
    Choices =
    {
        // Only show if player hasn't asked about forest yet
        new DialogueNode.Choice {
            choiceText = "\"Tell me about that forest.\"",
            nextNodeID = "ask_about_forest",
            RequireNotDiscussedNode = "ask_about_forest"  // Hide if already discussed
        },

        // Only show if player HAS discussed the guild
        new DialogueNode.Choice {
            choiceText = "\"Any news on recruits?\"",
            nextNodeID = "recruit_update",
            RequireDiscussedNode = "explain_guild"  // Show only after discussing guild
        },

        // Always available
        new DialogueNode.Choice {
            choiceText = "\"Just saying hello.\"",
            nextNodeID = "end"
        }
    }
});
```

**How it works:**
- `RequireDiscussedNode = "node_name"` - Choice only appears if player has visited that dialogue node
- `RequireNotDiscussedNode = "node_name"` - Choice only appears if player has NOT visited that dialogue node
- System tracks visited nodes per NPC automatically
- Use this to prevent repetitive conversations and acknowledge player's actions

**Example - Silvacis acknowledges if you've discussed the guild:**
```csharp
ranger.Dialogue.Add("repeat_greeting", new DialogueNode()
{
    Text = "Silvacis looks up from his search and nods in recognition. \"Ah, it's you again.\"",
    Choices =
    {
        // Only if guild hasn't been mentioned
        new DialogueNode.Choice {
            choiceText = "\"I'm rebuilding the Adventurer's Guild. Could use your help.\"",
            nextNodeID = "mention_guild_first",
            RequireNotDiscussedNode = "mention_guild_first"
        },

        // Only if guild HAS been mentioned (amulet quest active)
        new DialogueNode.Choice {
            choiceText = "\"Still looking for that amulet?\"",
            nextNodeID = "main_hub",
            RequireDiscussedNode = "mention_guild_first"
        },

        new DialogueNode.Choice {
            choiceText = "\"Just passing through.\"",
            nextNodeID = "goodbye"
        }
    }
});
```

### Step-by-Step Process

#### Step 1: Create Dialogue Nodes
```csharp
// In NPC definition
yourNPC.Dialogue.Add("first_greeting", new DialogueNode()
{
    Text = "A wandering merchant looks up from his wares and greets you with a smile. \"Hello there!<br><br>What brings you here?\"",
    Choices = new List<DialogueNode.Choice>
    {
        new DialogueNode.Choice
        {
            choiceText = "\"Who are you?\"",
            nextNodeID = "about"
        },
        new DialogueNode.Choice
        {
            choiceText = "\"Can you help me?\"",
            nextNodeID = "help"
        },
        new DialogueNode.Choice
        {
            choiceText = "\"Goodbye.\"",
            nextNodeID = "end"
        }
    }
});

yourNPC.Dialogue.Add("repeat_greeting", new DialogueNode()
{
    Text = "The merchant nods as you approach. \"Ah, back again! How can I help you today?\"",
    Choices = new List<DialogueNode.Choice>
    {
        new DialogueNode.Choice
        {
            choiceText = "\"What do you sell?\"",
            nextNodeID = "shop"
        },
        new DialogueNode.Choice
        {
            choiceText = "\"Goodbye.\"",
            nextNodeID = "end"
        }
    }
});

yourNPC.Dialogue.Add("about", new DialogueNode()
{
    Text = "The merchant spreads his arms wide. \"I'm a wandering merchant.<br><br>I sell rare goods to adventurers.\"",
    Choices = new List<DialogueNode.Choice>
    {
        new DialogueNode.Choice
        {
            choiceText = "\"What do you sell?\"",
            nextNodeID = "shop"
        },
        new DialogueNode.Choice
        {
            choiceText = "\"Talk about something else.\"",
            nextNodeID = "repeat_greeting"
        }
    }
});

yourNPC.Dialogue.Add("end", new DialogueNode()
{
    Text = "The merchant waves as you leave. \"Farewell, traveler!\"",
    Choices = new List<DialogueNode.Choice>()  // Empty = ends conversation
});
```

### Dialogue Reset Behavior - IMPORTANT!

**By default, ALL conversations reset to the appropriate greeting when they end.**

This means you don't have to worry about accidentally "locking" NPCs in dialogue nodes. When a conversation ends:
- **First time meeting**: Next conversation starts at `first_greeting` (or `greeting` if not defined)
- **Subsequent meetings**: Next conversation starts at `repeat_greeting` (or `greeting` if not defined)

**No special node names required!** You can name your nodes anything you want.

```csharp
// ✅ ANY ending node - Conversation always resets to greeting
farmer.Dialogue.Add("farewell", new DialogueNode()
{
    Text = "The farmer waves. \"Farewell! Come back anytime.\"",
    Choices = new List<DialogueNode.Choice>()  // Empty = conversation ends & resets
});

farmer.Dialogue.Add("busy_working", new DialogueNode()
{
    Text = "The farmer returns to his work. \"Sorry, I'm busy right now.\"",
    Choices = new List<DialogueNode.Choice>()  // Empty = conversation ends & resets
});
```

**Both of these will reset to greeting when the conversation ends!**

#### Permanently Ending Dialogue (Advanced)

**Only use this for special cases where an NPC should refuse to talk again:**

Set `PermanentlyEndsDialogue = true` on a node to lock dialogue at that position:

```csharp
// NPC refuses to talk after you insult them
farmer.Dialogue.Add("insulted", new DialogueNode()
{
    Text = "The farmer glares at you. \"Get out of my sight! I never want to speak to you again!\"",
    PermanentlyEndsDialogue = true,  // NPC is locked at this node
    Choices = new List<DialogueNode.Choice>()
});
```

**What happens:**
- When this conversation ends, NPC stays at the "insulted" node
- Next time you talk to them, conversation starts at "insulted" again
- Since it has no choices, it immediately ends
- Result: NPC permanently won't talk to you

**Use cases for PermanentlyEndsDialogue:**
- NPC is angry and refuses further conversation
- Quest failed permanently due to player choice
- NPC leaves/dies as part of story

**Don't use this for:**
- Normal conversation endings (they reset automatically now!)
- Merchant/vendor conversations
- Any NPC that should remain talkable

#### Dynamic Text Substitution (Placeholders)

**You can use placeholders in dialogue text and choices that get replaced at runtime:**

Available placeholders (case-insensitive):
- `{player.name}` - Player's name
- `{player.class}` - Player's class (Legionnaire, Venator, Oracle)
- `{player.level}` - Player's level number
- `{npc.name}` - Current NPC's name

**Example:**
```csharp
farmer.Dialogue.Add("greeting", new DialogueNode()
{
    Text = "Welcome, {player.name}! I see you're a {player.class}. Impressive!",
    Choices =
    {
        new DialogueNode.Choice
        {
            choiceText = "Greetings, {npc.name}. I'm here to help.",
            nextNodeID = "offer_help"
        }
    }
});
```

**Displays as:** "Welcome, Sinogue! I see you're a Legionnaire. Impressive!"

### Advanced Dialogue Features

#### Conditional Choices (Show Only If...)
```csharp
new DialogueNode.Choice
{
    choiceText = "About that amulet...",
    nextNodeID = "amulet_quest",
    IsAvailable = (inventory) => inventory.Contains("mysterious amulet")
}
```

#### Dialogue Actions (Do Something When Choice Selected)
```csharp
new DialogueNode.Choice
{
    choiceText = "Here's the item you wanted.",
    nextNodeID = "thank_you",
    Action = new DialogueAction
    {
        Type = DialogueActionType.RemoveItem,
        ItemName = "quest item",
        QuestFlagToSet = "quest_completed",
        QuestFlagValue = true
    }
}
```

#### Open Shop
```csharp
new DialogueNode.Choice
{
    choiceText = "Let's trade.",
    nextNodeID = "open_shop"  // Special: triggers shop UI
}
```

#### Recruit NPC
```csharp
new DialogueNode.Choice
{
    choiceText = "Will you join my guild?",
    nextNodeID = "recruitment"
    // Recruitment handled by game logic
}
```

### Text Formatting

**Use browser-style formatting:**
```csharp
Text = "First paragraph.<br><br>Second paragraph.<br><br>Third paragraph."

// NOT this (legacy console):
Text = "First paragraph.\n\nSecond paragraph."  // BAD
```

**Color markup (Spectre.Console style):**
```csharp
Text = "You found a [#FFD700]golden key[/]!<br><br>It glimmers in the light."
```

---

## 5. ADDING TUTORIAL MESSAGES

**Priority messages show once to guide new players**

### File Location
`/home/sinogue/GuildMaster/Managers/MessageManager.cs`

### Step-by-Step Process

#### Step 1: Add Message Definition
```csharp
// In MessageManager constructor
public MessageManager(GameContext gameContext, UIManager uiMgr)
{
    // ... existing code ...

    // YOUR NEW MESSAGE
    messages.Add("message_id", new PriorityMessage
    {
        Id = "message_id",
        Title = "Message Title",  // Optional header
        Content = "Your tutorial message here.<br><br>Explain the feature.",
        Priority = MessagePriority.Medium,  // High, Medium, or Low
        Category = "tutorial"  // or "hint", "warning", "info"
    });
}
```

#### Step 2: Trigger Message
```csharp
// In the code where you want to show the message
ProgramStatics.messageManager?.CheckAndShowMessage("message_id");
```

### Message Priority Levels
```csharp
MessagePriority.High    // Shows immediately, important
MessagePriority.Medium  // Shows at appropriate time
MessagePriority.Low     // Shows when convenient
```

### Example Messages
```csharp
// First combat
messages.Add("first_combat", new PriorityMessage
{
    Id = "first_combat",
    Title = "Combat Tutorial",
    Content = "You've entered combat!<br><br>Choose actions with numbers:<br>1 = Attack<br>2 = Ability<br>3 = Item<br>4 = Flee",
    Priority = MessagePriority.High,
    Category = "tutorial"
});

// First recruitment
messages.Add("first_recruit", new PriorityMessage
{
    Id = "first_recruit",
    Title = "Guild Management",
    Content = "You've recruited your first member!<br><br>Use 'guild' to manage your party.",
    Priority = MessagePriority.Medium,
    Category = "tutorial"
});
```

### Triggering Conditions
```csharp
// In GameEngine.cs or Manager classes

// First time player does something
if (player.ThreeMemberCombatCount == 1)
{
    ProgramStatics.messageManager?.CheckAndShowMessage("three_member_party");
}

// After specific quest
if (player.CompletedQuestIds.Contains("tutorial_quest"))
{
    ProgramStatics.messageManager?.CheckAndShowMessage("quest_complete_tutorial");
}

// When reaching milestone
if (player.Recruits.Count == 10)
{
    ProgramStatics.messageManager?.CheckAndShowMessage("max_recruits_reached");
}
```

---

## 6. ADDING QUESTS

**Quests are missions for recruits or the player**

### File Location
`/home/sinogue/GuildMaster/Data/QuestData.cs` (if exists) or manually in code

### Quest Structure
```csharp
Quest newQuest = new Quest
{
    Id = "unique_quest_id",
    Name = "Quest Name",
    Description = "Quest description that players see.",
    Difficulty = "Easy",  // Easy, Medium, Hard, Legendary
    Duration = 2.0f,  // Hours of in-game time
    MinGold = 50,  // Min reward
    MaxGold = 100,  // Max reward
    BaseSuccessChance = 70,  // Base % chance
    BaseExperienceReward = 50,  // XP reward
    ItemRewards = new Dictionary<string, int>
    {
        { "potion", 2 },  // item name, quantity
        { "rusty sword", 1 }
    },
    PotentialRecruit = "Ranger Name"  // Optional: reward recruit
};
```

---

## 7. ADDING ITEMS

**Items are objects players can collect and use**

### File Locations
- **Equipment:** `/home/sinogue/GuildMaster/Data/EquipmentData.cs`
- **Consumables:** `/home/sinogue/GuildMaster/Data/EffectData.cs`
- **Generic Items:** `/home/sinogue/GuildMaster/Data/ItemData.cs`

### Equipment (Weapons/Armor)
```csharp
// In EquipmentData.cs InitializeEquipment()
AllEquipment.Add("magic sword", new Weapon
{
    Name = "Magic Sword",
    ShortName = "sword",
    Slot = EquipmentSlot.Weapon,
    DiceCount = 2,
    DiceSides = 6,
    AttackBonus = 2,
    DefenseBonus = 0,
    SpeedBonus = 0,
    HealthBonus = 0,
    EnergyBonus = 0,
    Value = 200  // Gold price
});

AllEquipment.Add("steel armor", new Armor
{
    Name = "Steel Armor",
    ShortName = "armor",
    Slot = EquipmentSlot.Armor,
    DefenseBonus = 5,
    SpeedBonus = -1,  // Penalty
    HealthBonus = 10,
    Value = 150
});

AllEquipment.Add("power ring", new Equipment
{
    Name = "Ring of Power",
    ShortName = "ring",
    Slot = EquipmentSlot.Ring,
    AttackBonus = 3,
    DefenseBonus = 1,
    EnergyBonus = 5,
    Value = 300
});
```

### Consumable Items
```csharp
// In EffectData.cs InitializeEffects()
effects.Add("greater_healing", new Effect
{
    Name = "Greater Healing Potion",
    Description = "Restores a large amount of health.",
    TargetType = EffectTarget.SingleAlly,
    HealAmount = 50,  // HP restored
    EnergyRestore = 0,
    AttackBuff = 0,
    DefenseBuff = 0,
    Duration = 0  // Instant effect
});

effects.Add("strength_potion", new Effect
{
    Name = "Potion of Strength",
    Description = "Temporarily increases attack power.",
    TargetType = EffectTarget.SingleAlly,
    HealAmount = 0,
    AttackBuff = 5,  // +5 attack
    Duration = 3  // Lasts 3 turns
});
```

### Generic Items (Quest Items, Keys, etc.)
```csharp
// In ItemData.cs, add to room's item dictionary
{42, new Dictionary<string, Item>()
    {
        {"ancient key", new Item {
            Description = "An ornate key covered in mysterious runes.",
            ShortName = "key",
            IsLootable = true,
            IsConsumable = false  // Can't be used/eaten
        }},
        {"quest letter", new Item {
            Description = "A sealed letter addressed to the king.",
            ShortName = "letter",
            IsLootable = true
        }}
    }
}
```

---

## 8. TESTING YOUR CONTENT

### Pre-Deployment Checklist

#### For Events
- [ ] Event ID is unique
- [ ] Room ID exists
- [ ] All quest flags referenced exist or will be created
- [ ] DialogueTreeId is registered (if used)
- [ ] Conditions make logical sense
- [ ] Actions have correct parameters
- [ ] Event triggers at expected time
- [ ] Event doesn't trigger repeatedly if one-time
- [ ] Test with `flags` command to verify

#### For NPCs
- [ ] NPC name is unique
- [ ] Stats are balanced for level/area
- [ ] Equipment exists in EquipmentData
- [ ] Abilities exist in AbilityData
- [ ] Dialogue tree is complete (no dead ends)
- [ ] NPC appears in correct room
- [ ] Hostile NPCs attack correctly
- [ ] Recruitable NPCs can be recruited
- [ ] Merchant NPCs open shop

#### For Rooms
- [ ] Room ID is unique
- [ ] Room key is unique
- [ ] All exits lead to valid rooms
- [ ] Connected rooms have return exits
- [ ] Items are defined in ItemData
- [ ] NPCs are defined in NPCData
- [ ] Description uses <br><br> for breaks
- [ ] Room is accessible from existing areas

#### For Dialogue
- [ ] All nextNodeID values exist as nodes
- [ ] No orphaned nodes (unreachable)
- [ ] At least one path leads to "end"
- [ ] Conditional choices work as expected
- [ ] Text uses <br><br> not \n\n
- [ ] All special keywords (open_shop, etc.) correct

#### For Items
- [ ] Item name is unique
- [ ] Equipment stats are balanced
- [ ] Effects work as described
- [ ] Item appears in correct locations
- [ ] "take" and "use" commands work
- [ ] Short names work for quick pickup

### Manual Testing Commands
```bash
# Teleport to room to test
tpto 999

# Give item to test
giveitem ancient sword

# Give gold to test purchases
givegold 1000

# Check quest flags
flags

# Set level to test scaling
setlevel 5

# Enable room numbers for navigation
roomnumbers
```

### Build and Test
```bash
# Build project
dotnet build

# Check for errors
dotnet build 2>&1 | grep -i error

# Test in browser
# Navigate to your room, trigger your event, talk to your NPC
```

---

## BEST PRACTICES

### Writing Style
- **Use active voice:** "You see..." not "There is..."
- **Be concise:** Players skim long text
- **Create atmosphere:** Use sensory details
- **Stay consistent:** Match existing tone

### Technical Guidelines
- **IDs:** Use snake_case (quest_id, event_id, npc_name)
- **Names:** Use Title Case ("Ancient Sword")
- **Breaks:** Always use `<br><br>` for paragraphs
- **Colors:** Use hex codes (#FFD700) for consistency
- **Testing:** Test thoroughly before considering complete

### Content Balance
- **Items:** Price = roughly 10× level requirement
- **NPCs:** Health = 15-25 per level, Attack = 3-8 per level
- **Quests:** Duration 1-4 hours, Success 50-90%
- **Events:** Don't spam - 1-2 per major area max

---

## QUICK REFERENCE - FILE LOCATIONS

```
Events:              Data/EventDataDefinitions.cs
NPCs:                Data/NPCData.cs
Rooms:               Data/RoomData.cs
Dialogue:            Data/NPCData.cs (NPC) or EventDataDefinitions.cs (Event)
Items (Generic):     Data/ItemData.cs
Equipment:           Data/EquipmentData.cs
Effects:             Data/EffectData.cs
Abilities:           Data/AbilityData.cs
Messages:            Managers/MessageManager.cs
Quests:              (Manual in game code for now)
```

---

## COMMON MISTAKES TO AVOID

❌ **Don't:**
- Use `\n` for line breaks (use `<br><br>`)
- Forget to register event dialogue
- Create circular dialogue (no way to exit)
- Use same room ID twice
- Make items without ItemData entry
- Create events that always trigger
- Forget to test with fresh save

✅ **Do:**
- Test every dialogue path
- Check all item pickups work
- Verify events trigger correctly
- Balance NPC stats appropriately
- Write clear descriptions
- Follow existing patterns
- Document your additions

---

## GETTING HELP

### Debug Commands
```
flags          - View all quest flags
tpto [room]    - Teleport to room
giveitem [x]   - Get item
givegold [x]   - Get gold
roomnumbers    - Show room IDs
adminhelp      - See all debug commands
```

### Common Issues

**Event won't trigger:**
- Check conditions are met
- Verify event is in GetAllEvents()
- Check room ID is correct
- Test with `flags` command

**NPC won't talk:**
- Verify dialogue tree has "greeting" or CurrentDialogueNode
- Check NPC is in room
- Ensure no syntax errors in dialogue

**Item can't be picked up:**
- Check item is in ItemData for that room
- Verify IsLootable = true
- Check item name matches exactly

**Room can't be reached:**
- Verify exits connect both ways
- Check room IDs are correct
- Test navigation path

---

**Happy content creating! Your additions make GuildMaster better for everyone.**
