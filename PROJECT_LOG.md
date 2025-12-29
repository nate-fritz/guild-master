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
