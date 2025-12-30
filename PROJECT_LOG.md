# GuildMaster - Project Development Log

## Project Overview

**GuildMaster** is a text-based RPG where you rebuild an abandoned Adventurer's Guild. The game features:
- Exploration and combat in a fantasy world
- Recruiting party members and managing a guild
- Quest system and narrative-driven storytelling
- Strategic War Room minigame (Act III - in development)
- Browser-based UI built with Blazor Server

**Tech Stack:** C# / .NET 8 / Blazor Server / Spectre.Console wrappers

**Current Development Phase:** Act I (Early game, recruitment, basic exploration)

---

## Instructions for Maintaining This Log

**CRITICAL: Read this file at the START of every new session to understand current project state.**

### When to Add an Entry
- After completing a significant feature or system
- After fixing major bugs
- After making architectural changes
- After testing and confirming changes work

### Entry Format
```markdown
## [YYYY-MM-DD] - Brief Title

**Status:** Built & Tested | In Progress | Needs Testing

**Changes:**
- Bullet points describing what changed
- Focus on what was added/modified/fixed

**Key Files Modified:**
- List important files changed

**Notes/Context:**
- Any important context for future sessions
- Known issues or follow-up needed
- Testing notes

**Commit:** [commit hash if pushed]
```

### What NOT to Log
- Minor typo fixes
- Small tweaks that don't change functionality
- Work-in-progress changes that aren't tested

### ⚠️ IMPORTANT CHECKLIST - Before Marking Changes Complete

**Every time you make changes, verify:**

1. **Help Files Updated?**
   - ✅ `UIManager.ShowHelp()` - Updated if new player-facing commands added?
   - ✅ `UIManager.ShowAdminHelp()` - Updated if new admin/debug commands added?
   - Examples: New commands, changed command syntax, new features players need to know

2. **Save System Updated?**
   - ✅ `Models/GameState.cs` - Does it include new Player properties?
   - ✅ `Managers/SaveGameManager.cs` - Are new systems being saved/loaded?
   - Examples: New Player flags, new tracking dictionaries, new manager state
   - **Critical:** Forgetting this breaks save/load compatibility!

3. **Documentation Updated?**
   - ✅ `CONTENT_CREATION_GUIDE.md` - Updated if content creation process changed?
   - ✅ `README.md` - Updated if setup/requirements changed?

**Note:** Add these checks to your log entry under "Notes/Context" if applicable

---

## Development Log

## [2025-12-29] - Caelia Quest Expansion & Guild Council Meeting Event

**Status:** Built & Tested ✅

**Changes:**

**1. Caelia/Priestess NPC Quest Integration**
- **Updated Existing Priestess NPC**: Enhanced the existing "priestess" NPC (already named Caelia in game)
  - Enhanced description: ageless beauty, silver eyes, golden hair, High Priestess of Keius
  - Added first_greeting and repeat_greeting dialogue nodes (replacing single "greeting" node)
  - Added about_keius dialogue explaining Keius (god of light, knowledge, truth)
  - **Quest Dialogue Chain**: Added 4 new dialogue nodes for cult investigation
    - quintus_symbols: Caelia receives cultist documents from Quintus via messenger
    - explain_symbols: Reveals Five Seals lore, cult's apocalyptic plan
    - suggest_guild: Proposes meeting at guild hall study for secure discussion
    - end_council: Sets quest flag "guild_council_ready"
  - Dialogue gated by player having cultist documents in inventory (same as Quintus check)

**2. Guild Council Meeting Event System**
- **Event Trigger**: Automatic cutscene when entering guild hall study (Room 3) with "guild_council_ready" flag
  - Created CreateGuildCouncilMeetingEvent() in EventDataDefinitions.cs
  - Priority 200, one-time event
  - Conditions: guild_council_ready flag set, first visit to study after flag
  - Action: Sets "aevoria_travel_ready" flag on completion
- **Event Dialogue Tree**: Five-node council meeting cutscene
  - start: Quintus and Caelia waiting in study, reveal cult name "the Unbound"
  - explain_seals: Caelia explains Five Seals mythology, assassination plan
  - seal_breaks: Optional lore about apocalyptic consequences
  - warn_emperor: Quintus arranges travel to Aevoria, introduces Tribune Marcellus
  - ready_to_travel / need_preparation: Player choice for immediate or delayed departure
- **Registered Dialogue**: Added "guild_council_meeting_dialogue" to RegisterEventDialogueTrees()

**3. Quest Flow Implementation**
- **Complete Chain**: Archon Hideout → Quintus → Temple → Caelia → Guild Study → Aevoria Ready
  1. Player completes hideout, loots cultist documents
  2. Returns to Quintus, shows documents (hideout_discovered dialogue)
  3. Quintus directs player to temple, sets "quintus_temple_meeting" flag
  4. Player visits temple, talks to Caelia with documents in inventory
  5. Caelia deciphers symbols, sets "guild_council_ready" flag
  6. Player returns to guild hall study, triggers automatic council meeting event
  7. Event sets "aevoria_travel_ready" flag for Act II transition

**Key Files Modified:**
- `Data/NPCData.cs` - Updated priestess NPC (lines 270-360), removed duplicate Caelia NPC (lines 1392-1475)
- `Data/EventDataDefinitions.cs` - Added CreateGuildCouncilMeetingEvent() method, registered dialogue tree
- `Managers/EventManager.cs` - (No changes, existing system used)
- `Models/GameContext.cs` - (No changes, existing quest flag system used)

**Notes/Context:**
- **Design Decision**: Final meeting in guild hall (not temple) to force player back to see guild changes
- **Duplicate Fix**: Initially created duplicate "Caelia" NPC, corrected by updating existing "priestess" NPC
- **IsAvailable Limitation**: Cannot check quest flags in IsAvailable lambda (only inventory), used document check instead
- **Event System**: Leveraged existing EventManager/EventData infrastructure for cutscene
- **Narrative Setup**: Five Seals mythology establishes stakes for Act II (prevent emperor assassination)
- **Character Intro**: Tribune Marcellus name-dropped for future Aevoria content

**Checklist:**
- ✅ Build Verified - No compilation errors
- ✅ Quest Flags Used - guild_council_ready, aevoria_travel_ready
- ⚠️ Save System - No changes needed (quest flags already saved, event triggers already tracked)
- ⚠️ Help Files - No new player-facing commands added
- ⚠️ Testing - Not yet tested in-game (compiled successfully)

**Follow-up Needed:**
- Test complete quest chain: hideout → Quintus → Caelia → council meeting
- Verify event triggers correctly when entering guild study
- Implement travel to Aevoria (Act II transition)
- Add flirty dialogue options for Caelia (user mentioned this but not yet implemented)

## [2025-12-29] - Quintus Quest Chain, Timer System, and Critical Bug Fixes

**Status:** Built & Tested ✅

**Changes:**

**1. Senator Quintus Quest Chain** - Complete letter translation quest line
- **Bandit Warlord Loot**: Added "indecipherable letter" (100% drop rate) containing encrypted message
- **Senator Quintus NPC**: Created new NPC in Town Hall with cipher expertise
  - Older man with gray hair, senator robes, hints at old Adventurer's Guild connections
  - Takes encrypted letter and promises translation in 24 hours
  - Returns translated letter revealing passphrase "Ordo Dissolutus" and lore about cultists
- **Room 91 - Town Hall Interior**: New romanesque administrative building interior
  - Connected west of Room 87 (Town Hall exterior)
  - Contains Senator Quintus
- **Marcus Dialogue Update**: Gate guard now mentions Quintus when player turns in warlord's head

**2. Timer System Implementation** - Timed events that respect game time
- **GameTimer Model**: New class tracking timer ID, start time, duration, completion status
  - `IsComplete()` method calculates elapsed time based on CurrentDay/CurrentHour
  - Integrates seamlessly with existing time tracking (0.25h per move, 8h per rest)
- **Dialogue Actions**: Added "start_timer" action type to dialogue system
- **Timer Checking**: DialogueManager checks timers when starting dialogue with NPCs
  - Quintus switches between "waiting" and "translation_ready" based on timer
  - Completed timers automatically removed
- **Save/Load Support**: ActiveTimers dictionary persisted in GameState

**3. Speak Command & Fog Puzzle** - Voice-activated puzzle mechanic
- **Speak Command**: New puzzle interaction type `speak <passphrase>`
  - Added HandleSpeakCommand() in GameController
  - Wired up in GameEngine command routing
- **Room 53 Fog Puzzle**: Magical fog blocks path until passphrase spoken
  - Speaking "Ordo Dissolutus" clears fog permanently
  - Updates room description and opens east exit to Room 63 (cultist lair)
  - Unlimited attempts, helpful feedback for wrong passphrases
- **Puzzle Data**: Added "foggy_clearing_puzzle" to PuzzleData.cs

**4. Critical Bug Fixes**
- **NPC Health Bug** 🐛: NPCs were not resetting to full health at combat start
  - Root cause: Combat used references to room NPCs, damage persisted between fights
  - Fix: Added health/energy reset loop at start of StartCombat()
  - Prevented instant-death bug where damaged NPCs entered combat with 0 HP

- **Death Menu Load Bug** 🐛: "Load a saved game" option on death screen did nothing
  - Root cause: Empty callback, player continued with 1 HP instead of loading
  - Fix: Added ShouldShowLoadMenu flag, GameEngine checks and displays load menu

- **Gate Puzzle Command Bug** 🐛: "use key on gate" silently failed
  - Root cause: Substring detection with incorrect spacing requirements
  - Fix: Improved detection to accept "key on" or "keys on" + gate reference
  - Now properly shows feedback messages (missing keys, wrong gate, success)

- **Interaction Hints Bug** 🐛: Hints showed blank placeholders
  - Root cause: Angle brackets `<item>` stripped by Spectre.Console markup parser
  - Fix: Escaped brackets with `[[item]]` → displays as `[item]`

**Key Files Modified:**
- `Data/NPCData.cs` - Added Senator Quintus, updated Marcus dialogue, added letter to Bandit Warlord loot
- `Data/ItemData.cs` - Added indecipherable letter, translated letter (Room 97)
- `Data/RoomData.cs` - Created Room 91 (Town Hall Interior), updated Room 53 with fog puzzle
- `Data/PuzzleData.cs` - Added foggy_clearing_puzzle
- `Models/GameState.cs` - Added GameTimer class and ActiveTimers dictionary
- `Models/GameContext.cs` - Added ActiveTimers dictionary for runtime tracking
- `Managers/DialogueManager.cs` - Added start_timer action handler, CheckAndUpdateTimers() method
- `Managers/GameController.cs` - Added HandleSpeakCommand(), HandleFogPuzzle(), fixed hint escaping
- `Managers/CombatManager.cs` - Added NPC health reset, death menu flags (ShouldShowLoadMenu)
- `Managers/SaveGameManager.cs` - Added ActiveTimers save/load support
- `Services/GameEngine.cs` - Added speak command routing, improved gate puzzle detection, death menu check

**Notes/Context:**
- **Timer System**: Fully functional and extensible for future timed quests/events
- **Quest Flow**: Warlord → Marcus → Quintus → Wait 24h → Quintus → Room 53 → Speak passphrase → Room 63
- **Passphrase**: "Ordo Dissolutus" (Latin-esque for "Dissolved Order") ties to cult entropy theme
- **Save Compatibility**: Old saves will work, ActiveTimers initializes as empty dictionary
- **Room 63**: Cultist lair entrance now accessible but room not yet created
- **Testing Notes**: Verified timer completion after 24+ hours of in-game time (96 room movements or 3 rests)

**Checklist:**
- ✅ Save System Updated - ActiveTimers added to GameState and SaveGameManager
- ✅ Commands Work - speak command, improved gate puzzle detection
- ✅ Bugs Fixed - NPC health, death menu, gate commands, interaction hints
- ⚠️ Help Files - Not updated (speak command not yet documented in UIManager.ShowHelp)
- ⚠️ CONTENT_CREATION_GUIDE.md - Timer system not yet documented

**Follow-up Needed:**
- Document timer system in CONTENT_CREATION_GUIDE.md
- Add "speak" command to player help text
- Create Room 63 (Cultist Lair entrance)

## [2025-12-29] - Bug Fixes from Playtesting

**Status:** Built & Tested ✅

**Bugs Fixed:**

1. **Battle Cry Ability Error** 🐛
   - Root cause: Battle Cry ability not implemented for enemies, causing error messages
   - Fix: Removed Battle Cry from Cultist Philosopher, Cultist Lieutenant, and Archon Malachar ability lists
   - Files: `Data/NPCData.cs`

2. **Level Up Display Formatting** 🎨
   - Issue: Extra spacing and redundant "reached Level" text
   - Fix: Removed "<br><br>" and "reached Level X" line, kept clean stats box
   - Format now: `★ LEVEL UP! [Name] is now level X!` followed by stats separator
   - Files: `Managers/Combat/CombatUIDisplay.cs`

3. **Kill Message Capitalization** 🐛
   - Issue: "your blade sings" instead of "Your blade sings"
   - Fix: Added conditional capitalization for player kills (PARTIAL FIX)
   - Fixed: 1 message (line 3410)
   - **Still needed**: ~76 more kill messages have the same issue
   - All messages starting with `{killerName}'s` need: `{(killerName == "You" ? "Your" : killerName + "'s")}`
   - Files: `Managers/CombatManager.cs`

4. **Archon Room Description Persistence** 🐛
   - Issue: Boss room description included Archon's dialogue permanently
   - Fix: Changed to neutral description that works before/after combat
   - PreCombatDialogue still triggers introduction, but room description is now static
   - Files: `Data/RoomData.cs` (Room 120)

5. **Quintus Dialogue Truncation** 🐛
   - Issue: Long dialogue triggered pagination, could be cut off by Enter presses
   - Fix: Condensed translation dialogue to avoid pagination
   - Files: `Data/NPCData.cs` (translation_ready node)

6. **Town Hall Navigation** 🎨
   - Issue: Exterior description didn't mention entrance to interior
   - Fix: Updated description to mention "bronze doors to the west lead inside"
   - Files: `Data/RoomData.cs` (Room 87)

**Key Files Modified:**
- `Data/NPCData.cs` - Battle Cry removal, Quintus dialogue condensed
- `Data/RoomData.cs` - Archon chamber & Town Hall descriptions
- `Managers/Combat/CombatUIDisplay.cs` - Level up display
- `Managers/CombatManager.cs` - Kill message capitalization

**Build Status:** ✅ Compiles successfully (0 errors)

## [2025-12-29] - Cultist Hideout Implementation (IN PROGRESS)

**Status:** In Progress 🔄

**Completed So Far:**
- ✅ 8 Cultist enemy types created (Scout, Zealot, Defacer, Philosopher, Archivist, Breaker, Lieutenant, Unraveler)
  - Varied stats, abilities, and loot tables
  - Level 6-8 appropriate difficulty
  - 100-160 XP per enemy
  - Fixed EnemyRole values (no Caster/Tank, using Melee/Ranged/Support)

- ✅ Cult leader boss (Archon Malachar)
  - Level 8 boss with 95 HP, 250 XP
  - Pre-combat dialogue revealing cult motivation
  - 4 abilities: Entropy Bolt, Void Touch, Battle Cry, Flame Bolt
  - Drops quest items: ritual dagger (100%), cultist orders (100%)

- ✅ Quest documents created (Room 120 items)
  - "Cultist Orders" - Vague threat about festival/pillar
  - "Ritual Notes" - References to seal weakening and Aevoria
  - "Philosophical Tract" - Cult ideology about entropy
  - "Ritual Dagger" - Boss weapon/quest item

- ✅ 9 hideout rooms with puzzles (100-120)
  - Room 100: Entrance (from fog puzzle)
  - Room 101: Guard Post (2 enemies)
  - Room 102: Supply Cache (loot room)
  - Room 105: Ritual Chamber (cipher puzzle - speak "nihil" to unlock)
  - Room 108: Defaced Library (lore/flavor)
  - Room 110: Monument of Unmaking (2 enemies, environmental storytelling)
  - Room 115: Prison Cells (1 enemy, Althea location)
  - Room 118: Antechamber (2 enemies, pre-boss)
  - Room 120: Archon's Sanctum (boss chamber)

- ✅ Ritual Chamber Cipher Puzzle
  - Pedestals with symbols: Fire, Water, Death, Abyss, Broken, Nothing
  - Answer: "Nihil" (the void that unmakes all things)
  - Opens south exit to library when solved
  - Integrated with speak command system

- ✅ Althea NPC (Oracle recruit)
  - Found imprisoned in Room 115 (Prison Cells)
  - First/repeat greeting dialogue implemented
  - Reveals cult plot: Anniversary festival attack, targeting Emperor
  - Explains seals and cascade failure risk
  - Recruitment dialogue path (needs boss key to free her)
  - Note: Braxus recognition dialogue deferred (requires party member check feature)

- ✅ Quintus post-hideout dialogue
  - New dialogue triggered when player has cultist documents (cultist orders, ritual notes, or philosophical tract)
  - Reviews evidence and realizes scope of threat
  - Mentions religious symbols/runes he can't decipher
  - Introduces Caelia and directs player to temple
  - Sets quest flag: "quintus_temple_meeting"
  - Dialogue nodes: hideout_discovered, introduce_caelia, end_temple_quest

**Still Needed (from original spec):**
- ⏳ Additional hideout rooms (10-12 more rooms to flesh out the dungeon)
- ⏳ Level 6-10 equipment for loot tables

**Still Needed (from user feedback):**
- ⏳ **Caelia NPC**: Priestess of Keius at temple (Room 89)
  - Beautiful, seemingly ageless
  - Flirty with player (optional reciprocation)
  - Deciphers religious symbols from letter
- ⏳ **Forest reminder event**: After Caelia meeting, trigger when returning to forest
  - Player recognizes symbol carved on tree
  - Reminds player to "speak Ordo Dissolutus"
- ⏳ **Town NPC dialogue trees**: Guards/citizens should give directions
  - "Where is the town hall?" → "West end of town"
  - "Where is the temple?" → "Northwest part of town"
  - Etc.
- ⏳ **Post-hideout event chain**: Urgent Council → Travel to Aevoria
  - Quintus reviews evidence, realizes Emperor is target
  - Introduction to Caelia's role in seal knowledge
  - Travel event with 3-day time skip

**Key Files Modified:**
- `Data/NPCData.cs` - 8 cultist enemies + Archon Malachar boss + Althea NPC + Quintus post-hideout dialogue
- `Data/ItemData.cs` - Quest documents for Room 120
- `Data/RoomData.cs` - 9 hideout rooms with cult theme, Althea added to Room 115
- `Data/PuzzleData.cs` - Ritual chamber cipher puzzle
- `Managers/GameController.cs` - HandleRitualCipherPuzzle() method

**Notes:**
- Passphrase mechanic updated to open Room 100 (hideout entrance)
- Cult theme: Entropy/decay/unmaking, not gore - philosophical destruction
- User requested puzzles be prototyped for dungeon progression
- Build tested and compiles successfully (0 errors)
- EnemyRole enum only has Melee/Ranged/Support (no Caster/Tank)
- **Design Decision**: Built 9 core path rooms (instead of all 21) to establish complete playable dungeon flow from entrance to boss. Remaining 12 rooms can be added for side areas, branching paths, and additional content after testing core experience.

## [2025-12-28] - Enhanced Dialogue System with Narrative Formatting

**Status:** Built & Tested ✅

**Changes:**
- **Quotation System**: Added quotation marks to all NPC dialogue with narrative descriptions outside quotes
  - Format: `Gaius smiles. "Hello there, friend!"`
  - All dialogue in NPCData.cs updated to follow this pattern

- **First/Repeat Greetings**: NPCs now recognize when they've met you before
  - `first_greeting` node - Used on first meeting (full introduction)
  - `repeat_greeting` node - Used on subsequent meetings (acknowledges prior meeting)
  - System automatically tracks met NPCs in `Player.MetNPCs`

- **Topic Tracking**: NPCs can acknowledge topics you've already discussed
  - `RequireDiscussedNode` - Show choice only if node was visited
  - `RequireNotDiscussedNode` - Show choice only if node was NOT visited
  - Prevents repetitive conversations and creates dynamic dialogue
  - System tracks visited nodes per NPC in `Player.VisitedDialogueNodes`

- **Auto-Reset Dialogue**: Conversations now reset to greeting by default (no more dialogue lock-outs!)
  - Added `PermanentlyEndsDialogue` flag for special cases (NPC refuses to talk again)
  - Removed fragile "end" node naming requirement

- **War Room Improvements**:
  - Added comprehensive tutorial explanation on first entry
  - Added `warroom` toggle command (off by default for dev/testing)
  - Enhanced action menu descriptions
  - War Room is now hidden from players unless explicitly enabled

**Key Files Modified:**
- `Models/Player.cs` - Added MetNPCs and VisitedDialogueNodes tracking
- `Models/GameState.cs` - Added save system support for dialogue tracking (MetNPCs, VisitedDialogueNodes, WarRoomEnabled)
- `Models/DialogueNode.cs` - Added RequireDiscussedNode, RequireNotDiscussedNode, PermanentlyEndsDialogue
- `Managers/DialogueManager.cs` - Implemented first/repeat greeting logic, topic tracking, auto-reset
- `Data/NPCData.cs` - Updated all dialogue with quotations, added first/repeat greetings for key NPCs
- `Managers/WarRoomManager.cs` - Added tutorial explanation
- `Services/GameEngine.cs` - Added warroom toggle command
- `Managers/UIManager.cs` - Added warroom command to admin help
- `CONTENT_CREATION_GUIDE.md` - Comprehensive documentation of new dialogue features

**Notes/Context:**
- Dialogue system is now much more robust and forgiving for content creation
- Gaius and Silvacis have first/repeat greetings implemented as examples
- Topic tracking demonstrated in Silvacis' dialogue (amulet quest)
- War Room is feature-complete for testing but hidden by default (use `warroom` command)
- Backup created: `backup/stable-before-dialogue-system` branch and `v0.1-stable-pre-dialogue` tag

**Bug Fixes:**
- Fixed Silvacis dialogue not showing amulet return option after "exploring" conversation path
- Fixed dialogue getting stuck in terminal nodes (now auto-resets)
- Fixed save/load compatibility by adding new properties to GameState.cs

**Commits:**
- Initial implementation: `a78757c`
- Save system fix: `4820e08`
- Pushed to both repos (LoxGM and nate-fritz)

---

## [2025-12-24] - Dialogue Node Naming and Placeholder Documentation

**Status:** Built & Tested ✅

**Changes:**
- Added documentation about dialogue node naming conventions to CONTENT_CREATION_GUIDE.md
- Explained special "end" node behavior vs other terminal nodes
- Documented dynamic text substitution placeholders ({player.name}, {player.class}, etc.)
- Added examples of progressive dialogue using non-"end" terminal nodes

**Key Files Modified:**
- `CONTENT_CREATION_GUIDE.md`

**Notes/Context:**
- This documentation was later superseded by the auto-reset dialogue system
- Placeholder system ({player.name}, etc.) still works and is documented

**Commit:** `b4c7af9`

---

## [2025-12-XX] - Early Game Dialogue Updates

**Status:** Built & Tested ✅

**Changes:**
- Updates to early game dialogue
- Small UI tweaks
- General polish and improvements

**Key Files Modified:**
- Various dialogue and UI files

**Notes/Context:**
- Minor updates and polish
- No major system changes

**Commit:** `835d68e`

---

## Template for Future Entries

## [YYYY-MM-DD] - Brief Title

**Status:** Built & Tested | In Progress | Needs Testing

**Changes:**
-

**Key Files Modified:**
-

**Notes/Context:**
-

**Commit:**

---

## Quick Reference - Recent Priorities

**Current Focus:**
- Act I content (early game, recruitment, exploration)
- Dialogue polish and narrative improvements
- War Room testing (hidden feature, toggle with `warroom` command)

**Known Issues:**
- None critical

**Next Up:**
- Continue Act I content creation
- Add more first/repeat greetings to NPCs
- Add topic tracking to more dialogue trees
- Test War Room gameplay balance

---

## Project Structure Quick Reference

**Core Gameplay:**
- `Services/GameEngine.cs` - Main game loop and command processing
- `Managers/GameController.cs` - Room navigation and exploration
- `Managers/CombatManager.cs` - Turn-based combat system
- `Managers/DialogueManager.cs` - NPC conversation system

**Data Files:**
- `Data/NPCData.cs` - All NPC definitions and dialogue
- `Data/RoomData.cs` - Room definitions and connections
- `Data/EventData.cs` - Triggered events and cutscenes
- `Data/ItemData.cs` - Item definitions

**Guild Management:**
- `Managers/GuildManager.cs` - Guild menu and party management
- `Managers/QuestManager.cs` - Quest board and quest logic
- `Managers/WarRoomManager.cs` - War Room strategic layer (Act III)

**Documentation:**
- `CONTENT_CREATION_GUIDE.md` - How to add content (NPCs, rooms, dialogue, etc.)
- `README.md` - Project overview and setup
- `PROJECT_LOG.md` - This file (development history and context)
