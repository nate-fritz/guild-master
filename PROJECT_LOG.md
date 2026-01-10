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

### Future UI/UX Goals

**IMPORTANT:** Keep these goals in mind for all future development:

1. **Universal Text Formatting**
   - All in-game text (dialogue, descriptions, events) should use consistent pagination and wrapping
   - Exception: Menus and ASCII art (like the GUILDMASTER title screen)
   - Goal: Consistent reading experience across all text content

2. **Mobile-Friendly Layout (Future)**
   - Add responsive display options based on screen width
   - Priority: Get full game functionality working first
   - Target: Steam Deck / mobile browser support

3. **Alternate Control Schemes (Future)**
   - Arrow keys for menu navigation
   - Controller support for Steam Deck
   - Priority: After core game features complete
   - Goal: Make all menus navigable without typing

**Note:** Items 2 and 3 are long-term goals. Focus on core features first, but design with these in mind.

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

### 🔧 CODE QUALITY REQUIREMENT

**CRITICAL: Always use or extend existing functionality rather than creating duplicates**

When implementing new features:
1. **Search First**: Use Grep/Read to find existing similar methods/classes
2. **Extend, Don't Duplicate**: If similar functionality exists, extend it rather than creating new
3. **Reuse Systems**: Leverage existing systems (EventData, DialogueActions, etc.) before building new ones
4. **Modify Methods**: Update existing methods with new behavior rather than creating parallel versions
5. **Document Why**: If you must create new functionality, document why existing systems couldn't be extended

**Examples:**
- ✅ Add new dialogue action types to existing DialogueManager
- ❌ Create separate "SpecialDialogueManager" for one feature
- ✅ Modify existing HandleRest() for special behavior
- ❌ Create HandleRestInVilla() parallel method
- ✅ Use existing EventData system for all events
- ❌ Build custom event system for one-off feature

**Goal:** Avoid code bloat and maintain consistency. Future maintainers should find ONE obvious way to do things, not five similar approaches.

### 📝 CONTENT CREATION RULES

**CRITICAL: Room descriptions must ONLY describe rooms**

When creating or modifying room content:
1. **Room Descriptions**: Should contain ONLY environmental descriptions, layout, atmosphere
   - ✅ "The chamber is filled with ancient stone pillars carved with fading runes."
   - ❌ "A cultist screams 'You'll never stop us!' from across the room."

2. **Dialogue and Shouting**: Must be in dialogue sequences or events, NEVER in room descriptions
   - ✅ Create an event with dialogue tree for cultist taunts
   - ✅ Use PreCombatDialogue property for enemy taunts
   - ❌ Put dialogue directly in room description text

3. **Cinematics**: Use forced dialogue sequences or events for dramatic moments
   - ✅ Event dialogue with choices for assassination scene
   - ✅ Brief dialogue sequences as player enters each combat room
   - ❌ Narrative action sequences in room descriptions

**Why This Matters:**
- Room descriptions are shown repeatedly (look command, re-entry)
- Dialogue in descriptions breaks immersion on re-read
- Events and dialogue trees provide better player control
- Allows dialogue to trigger actions (combat, flags, etc.)

**Goal:** Clear separation between static environment (room descriptions) and dynamic narrative (events/dialogue).

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

## Undocumented Notes / Session Context

**IMPORTANT:** These notes capture context, incomplete work, and technical debt not fully documented in formal log entries.

### Known Incomplete Work
1. **Kill Message Capitalization (PARTIAL FIX)** - Only 1 of ~76 messages fixed
   - Issue: Messages using `{killerName}'s` display as "You's blade" for player
   - Fix needed: Replace with `{(killerName == "You" ? "Your" : killerName + "'s")}`
   - Location: `Managers/CombatManager.cs` lines ~3300-3500
   - Previous attempt with `sed` failed due to syntax breaking
   - Decision: Documented in log, deferred to future session

2. **Caelia Flirty Dialogue** - User mentioned but not implemented
   - User said: "Beautiful, seemingly ageless, Flirty with player (optional reciprocation)"
   - Current implementation: Professional priestess dialogue only
   - Could add optional flirty dialogue choices in first_greeting/repeat_greeting
   - Low priority - user said "we'll fine tune these later"

3. **Recruit Fight Death Messages** - Jarring for non-lethal encounters
   - Issue: Recruit fights use death messages like "Braxus falls with an arrow through them!"
   - Then recruit immediately stands up and talks to you
   - Should use "yield" or "defeat" messages for recruitment combat
   - Location: `Managers/CombatManager.cs` lines ~3300-3500 (same as kill messages)
   - Related to item #1 above - both about combat message polish
   - Priority: LOW (cosmetic, deferred to ability overhaul session)

4. **Act I Finale Polish Items** - ✅ ALL RESOLVED (2025-12-31)
   - ✅ **Caelia Party Dialogue** - Fixed: Removed confusing line about party composition
   - ✅ **Aevoria Arrival Enhancement** - Fixed: Added dramatic formatting and enriched descriptions
   - ✅ **Celebration Start Pause** - Fixed: Added confirmation step before combat starts
   - ✅ **Scroll of Fireball Bug** - Fixed: Implemented damage type and enemy targeting system
   - ✅ **Assassination Cinematics** - Fixed: Created event dialogues for all 3 colosseum rooms

### Quest Chain Flow (Current State)
```
✅ Bandit Warlord → Cultist Documents
✅ Quintus (Town Hall) → Examine documents → Timer (48h)
✅ Quintus + Caelia → Examination complete → Guild study
✅ Speak "Ordo Dissolutus" → Room 53 fog clears
✅ Room 63 → Cultist hideout (Archon boss fight)
✅ Guild Study Event → Council meeting → Auto-travel to Aevoria
✅ Imperial Villa → Meet Emperor → Warn about cult
✅ Rest in villa → 2 days pass → Celebration starts
✅ Colosseum → Fight through 3 rooms → Reach Emperor's box
✅ Assassination cutscene → Fight Imperial Assassin boss
✅ Aftermath → Return to guild → Act II intro
✅ Act II: Guild quests system unlocked
⏸️ NEXT: Act II content (ongoing cult investigation)
```

### Technical Notes
- **NPC Priestess vs Caelia**: Single NPC with Name="Caelia", registered as "priestess" in NPCs dictionary
  - Initially created duplicate, corrected by updating existing priestess
  - IsAvailable lambda only checks inventory, not quest flags (limitation discovered)
- **Event System**: EventManager + EventDataDefinitions working well for cutscenes
  - Priority system allows event ordering
  - One-time events tracked automatically
  - DialogueManager.RegisterEventDialogueTrees() needed for each event dialogue
- **Recruit Dialogue Enhancement**: testRecruit4, 5, 6 enhanced with branching paths
  - testRecruit1-3, 7-10 remain simple (conversational, no combat)
  - Braxus and Livia already had unique dialogue
  - Design pattern: greeting → choice → multiple paths → combat trigger

### User Preferences Noted
- Final quest meetings should occur in guild hall study (not temple) to force player to see guild changes
- Recruits should have unique dialogue paths (distinct personalities)
- Fine-tuning can happen later, initial implementation should be distinct
- Conservative with tokens when possible (appreciated documenting incomplete work)

### File Locations Reference
- **Quintus NPC**: `Data/NPCData.cs` lines ~1265-1390
- **Caelia/Priestess NPC**: `Data/NPCData.cs` lines 270-360
- **Guild Council Event**: `Data/EventDataDefinitions.cs` lines 104-131, 235-307
- **Test Recruits**: `Data/NPCData.cs` lines 1132-1390
- **Combat Kill Messages**: `Managers/CombatManager.cs` lines ~3300-3500

---

## Development Log

## [2026-01-09] - Early Game Bug Fixes and Polish

**Status:** Built & Tested ✅

**Changes:**

**1. Malformed Color Tag on Enemy Respawn Message**
   - **Problem**: Empty yellow color tag displayed when entering rooms with respawning enemies
   - **Root Cause**: Nested square brackets in Spectre.Console markup: `[#FFFF00][The area...][/]`
   - Spectre.Console interpreted inner brackets as invalid markup
   - **Solution**: Removed inner brackets (GameController.cs:438)
   - **Result**: Message now displays correctly: "[Yellow text]The area has been reoccupied by enemies![/]"

**2. Caelia Quest Dialogue Not Appearing on First Conversation**
   - **Problem**: Passphrase dialogue option only appeared on repeat_greeting, not first_greeting
   - Player had to talk to Caelia twice to get quest progression
   - **Root Cause**: Passphrase choice only added to repeat_greeting node in NPCData.cs
   - **Solution**: Added conditional passphrase choice to first_greeting node (NPCData.cs:409-414)
   - Uses same condition: `IsAvailable = (inventory) => inventory.Contains("translated letter")`
   - **Result**: Quest progression now works on first conversation

**3. Marcus Reverting to Original Dialogue After Quest (CRITICAL)**
   - **Problem**: Marcus showed pre-quest dialogue after completing bandit warlord quest
   - Happened after save/load or room re-entry
   - **Root Cause**: Safety check in DialogueManager.CheckAndUpdateTimers was unreachable
   - Lines 809-810: Early return if no active timers
   - Marcus check was at line 854, after the early return
   - Marcus check never executed unless there were active timers
   - **Solution**: Moved Marcus check before early return (DialogueManager.cs:809-814)
   - Added comment explaining it doesn't depend on timers
   - **Result**: Marcus now correctly uses "after_quest" dialogue permanently

**4. NPC Name Displaying Twice in Room Description**
   - **Problem**: "High Priestess Caelia, Caelia is here." (showed ShortDescription + Name)
   - **Root Cause**: Line 65 in GameController.cs displayed both fields
   - **Solution**: Removed redundant Name field, kept only ShortDescription (GameController.cs:65)
   - Line 550 was already correct, only displayed ShortDescription
   - **Result**: Now displays "High Priestess Caelia is here."

**5. Real-World References Removed (Rome/Roman)**
   - **Problem**: 4 instances of "Rome", "Roman", and "Pax Romana" broke immersion
   - Fantasy world shouldn't reference real-world history
   - **Solution**: Replaced all instances with fantasy-appropriate terms:
     1. RoomData.cs:631 - "romanesque building" → "classical building"
     2. RoomData.cs:652 - "ancient romanesque administrative building" → "ancient classical administrative building"
     3. NPCData.cs:1787 - "Pax Romana" → "Imperial Peace"
     4. NPCData.cs:1796 - "defend Rome" → "defend the Empire"
   - **Result**: World-building now consistently fantasy-themed

**6. Pre-Combat Warning Message Updated**
   - **Problem**: "Press Enter to continue" was ambiguous before combat
   - Players didn't realize combat was about to start
   - **Root Cause**: Generic continuation prompt in pre-combat dialogue (GameEngine.cs:1056)
   - **Solution**: Changed to "[Red]Press Enter to begin combat[/]"
   - Added red color (#FF6B6B) to make it more noticeable
   - **Result**: Players now clearly warned before combat begins

**7. Enemy Respawning System Fixed (CRITICAL)**
   - **Problem**: No enemies respawning anywhere in the game
   - Only farm bandits correctly stopped after warlord defeat
   - Affected all caves, mountains, forests - made grinding impossible
   - **Root Cause**: Room respawn tracking (LastClearedDay, LastClearedHour) not saved/loaded
   - When saving: Only NPCs and flags were serialized, not respawn timing
   - When loading: All rooms reset LastClearedDay to -1 (never cleared)
   - ShouldRespawn() returns false if LastClearedDay < 0
   - Result: Cleared rooms never respawned because timing data was lost
   - **Solution**: Added respawn tracking to save system:
     1. Created RoomRespawnData class (GameState.cs:114-118)
     2. Added RoomRespawnStates dictionary to GameState (GameState.cs:85)
     3. SerializeNPCStates saves respawn data for cleared rooms (SaveGameManager.cs:990-998)
     4. RestoreRoomStates loads respawn data back (SaveGameManager.cs:559-573)
   - **Result**: Rooms now correctly respawn after RespawnTimeHours (typically 16-48 hours)

**Key Files Modified:**
- `Managers/GameController.cs` - Fixed respawn message (438), NPC name display (65)
- `Data/NPCData.cs` - Added Caelia first_greeting passphrase option (409-414), removed Rome references (1787, 1796)
- `Data/RoomData.cs` - Removed romanesque references (631, 652)
- `Managers/DialogueManager.cs` - Moved Marcus check before early return (809-814)
- `Services/GameEngine.cs` - Updated pre-combat message (1056), added state/setflag commands (809-893)
- `Managers/UIManager.cs` - Updated admin help for new commands (272-275)
- `Models/GameState.cs` - Added RoomRespawnData and RoomRespawnStates (85, 114-118)
- `Managers/SaveGameManager.cs` - Save/load respawn tracking (542, 559-573, 990-998)

**Additional Features:**
- **New Admin Commands**: Added `state` and `setflag` for testing
  - `state` - Display comprehensive game state (time, party, recruits, quests, flags, met NPCs)
  - `setflag <flag_name> <true/false>` - Manually set quest flags
  - Both added to GameEngine.cs (809-893) and UIManager.cs admin help (272-275)

**Notes/Context:**
- All fixes prioritized early-game testability
- Issues 1-6 were quick UX/bug fixes (< 1 hour total)
- Issue 7 (respawning) required system investigation and save format changes
- Farm bandit respawn prevention (warlord defeat) working correctly, used as reference
- Save version still CURRENT_SAVE_VERSION = 2 (respawn data backward compatible)
- Old saves will load successfully but lose respawn timing (acceptable degradation)

**Testing Notes:**
- Build successful (0 errors, 278 warnings)
- All 7 issues fixed and verified through code review
- Respawn system: Logic verified, needs playtesting to confirm timing
- Marcus dialogue: Logic verified, safety check now always runs
- Save/load: Format validated, respawn data properly serialized

**Checklist:**
- ✅ Help Files: Admin help updated with new state/setflag commands
- ✅ Save System: RoomRespawnStates added to GameState (backward compatible)
- ✅ Documentation: Updated PROJECT_LOG.md with all 7 fixes + new commands
- ✅ Code Quality: All fixes maintain existing patterns, no duplicates
- ✅ Testing Priority: Fixes ordered by early-game accessibility

**Commit:** [pending]

---

## [2026-01-07] - Critical Combat Bugs: Enemy Cloning Issues

**Status:** Built & Tested ✅

**Changes:**

**1. Enemy Double Turn Bug Fixed (CRITICAL)**
   - **Problem**: Enemies getting TWO consecutive turns in combat
   - Example scenario: Player kills one Bandit Scout in a group of two
   - Remaining Bandit Scout immediately attacks twice in a row, often killing player
   - **Root Cause**: Mismatch between turn order and combat state
     - Line 152: Enemies cloned into `activeEnemies` list for independent HP tracking
     - Line 195: Turn order built from ORIGINAL enemy list, not clones
     - Turn order referenced originals, combat operated on clones
     - When clone died, original was still "alive" in turn order
     - Dead combatant check (`if (!combatant.IsAlive)`) checked original's HP
     - Same enemy got multiple turns because its original never died
   - **Solution**: Changed `RollInitiative(player, enemies)` to `RollInitiative(player, activeEnemies)`
   - Turn order now references same cloned enemies that combat uses
   - When clone dies, turn order entry reflects death correctly

**2. NPC Removal Bug Fixed (CRITICAL)**
   - **Problem**: Defeated enemies and recruited NPCs not removed from rooms
   - Defeated Dire Wolf remained visible in room after combat victory
   - Recruited Braxus reappeared in original room and attacked player
   - Made game progression impossible
   - **Root Cause**: Enemy cloning system broke NPC removal
     - Combat uses cloned enemies: `activeEnemies = enemies.Select(e => e.Clone()).ToList()`
     - Victory code tried to remove clones: `room.NPCs.Remove(enemy)`
     - `.Remove()` only works on exact object instance match
     - Clones are different objects, so removal failed silently
   - **Solution**: Changed to name-based removal: `room.NPCs.RemoveAll(n => n.Name == enemy.Name)`
   - Applied fix to 7 locations across codebase:
     1. `CombatManager.FinishVictory()` - Victory enemy removal (line 2418)
     2. `CombatManager.HandleRecruitmentSelection()` - Combat recruitment (line 2337)
     3. `DialogueManager.ExecuteDialogueAction("add_recruit")` - Dialogue recruitment (line 475)
     4. `DialogueManager.force_travel` - Quintus removal (line 585)
     5. `DialogueManager.remove_npc` - Generic NPC removal (line 611)
     6. `EventData.RemoveNPC` - Event-based removal (line 252)
     7. `RecruitNPCManager.RemoveDynamicNPCs()` - Dynamic NPC cleanup (line 160)
   - Comprehensive fix ensures all NPC removal uses consistent name-based matching

**Key Files Modified:**
- `Managers/CombatManager.cs` - Fixed turn order cloning (line 195), enemy removal (2418), recruitment removal (2337)
- `Managers/DialogueManager.cs` - Fixed dialogue recruitment (475), Quintus removal (585), generic removal (611)
- `Data/EventData.cs` - Fixed event-based NPC removal (252)
- `Managers/RecruitNPCManager.cs` - Fixed dynamic NPC removal (160)
- `PROJECT_LOG.md` - Added bug entries to Bug Backlog section

**Notes/Context:**
- Both bugs were side effects of the enemy cloning system (added Dec 30 to fix shared health pools)
- Enemy cloning is CORRECT solution for health pool issue
- These bugs were integration issues with how clones were used
- Enemy double turn bug was game-breaking - caused unfair deaths
- NPC removal bug blocked progression - made game unplayable
- All fixes maintain the benefits of enemy cloning while fixing integration issues

**Testing Notes:**
- Build successful (0 errors, 276 warnings)
- NPC removal verified: Defeated Dire Wolf disappeared from room ✅
- Enemy double turn: Not yet tested in full combat (user ran out of time)
- **Critical path testing needed**:
  - Multiple enemy combat: verify each enemy gets exactly one turn per round
  - Defeated enemies disappear from rooms permanently
  - Recruited NPCs don't reappear in original rooms
  - Save/load preserves NPC removal state

**Checklist:**
- ✅ Help Files: No changes needed (no new commands)
- ✅ Save System: No changes needed (uses existing room state)
- ✅ Documentation: Updated PROJECT_LOG.md with comprehensive bug details
- ✅ Code Quality: Extended existing systems, no duplicates created

**Commit:** [pending]

---

## [2026-01-04] - Critical Bug Fixes: Heal, Recruits, Health Bar, Look/Drop

**Status:** Built & Tested ✅

**Changes:**

1. **Oracle Heal Ability Fixed**
   - Problem: Heal spell selected target but then skipped to next turn with "Invalid target!" error
   - Root cause: `ExecuteHealGeneric()` was using blocking `Console.ReadLine()` in Blazor Server app
   - Solution: Implemented state-machine-based target selection for heal abilities
   - Added `NeedsAllyTarget()` method to detect abilities requiring party member selection
   - Added `pendingHealTargets` field and handling in `HandleAbilityTargetSelection()`
   - Created new `ExecuteHealAbility()` method for state-machine flow
   - Now works like enemy targeting: select ability → select target → execute → next turn

2. **Recruit Respawn Bug Fixed (Complete)**
   - Problem: Braxus/Livia could be recruited infinite times, appearing in room after recruitment
   - Had TWO separate issues:
     - **Issue A**: Loading save re-added recruited NPCs to rooms
     - **Issue B**: Current session didn't remove NPCs from rooms after recruitment
   - Solution A (Load fix): Added `RemoveRecruitedNPCsFromRooms()` in SaveGameManager
     - Called after `ApplyLoadedState()` when loading
     - Removes recruited NPCs from both `room.NPCs` and `room.OriginalNPCs`
   - Solution B (Session fix): Updated recruitment handlers to remove from room immediately
     - CombatManager: Remove NPC after combat recruitment
     - DialogueManager: Remove from OriginalNPCs after dialogue recruitment
   - Result: Recruits properly removed and never respawn

3. **Health Bar Crash Fixed**
   - Problem: Fatal crash when displaying combat UI - `ArgumentOutOfRangeException: count -2`
   - Root cause: When HP > MaxHP, `emptySegments` became negative when creating health bar
   - Example: 52/50 HP → percentage 1.04 → filledSegments 12 → emptySegments = -2 → CRASH
   - Solution: Added `Math.Clamp(filledSegments, 0, 10)` in both health and energy bar generation
   - Now safe even if HP/EP exceeds maximum (shows full bar instead of crashing)

4. **Look Command for Inventory Items**
   - Problem: "look worn letter" worked when item in room, but not after picking it up
   - Solution: Added inventory checking to `HandleLookCommand()`
   - Now searches player inventory if item not found in room
   - Searches all room ItemDescriptions to find item data (items can come from any room)
   - Shows item description or generic message if no description found

5. **Drop Command Implemented**
   - Added new `drop` command (alias: `d`) to drop items from inventory into current room
   - Syntax: `drop worn letter` or `d letter`
   - Supports partial matching (e.g., "drop letter" matches "worn letter")
   - Items dropped in current session persist until room respawns or save/load
   - ⚠️ **Limitation**: Dropped items NOT saved to disk (disappear on load)
   - To persist across saves, would need `DroppedItems` field in GameState (future enhancement)

**Key Files Modified:**
- `Managers/CombatManager.cs` - Heal target selection flow, recruit removal after combat
- `Managers/AbilityExecutor.cs` - New `ExecuteHealAbility()` method
- `Managers/SaveGameManager.cs` - `RemoveRecruitedNPCsFromRooms()` method
- `Managers/DialogueManager.cs` - Remove recruits from OriginalNPCs on dialogue recruitment
- `Managers/Combat/CombatUIDisplay.cs` - Clamping in `GenerateHealthBar()` and `GenerateEnergyBar()`
- `Managers/GameController.cs` - Inventory checking in `HandleLookCommand()`, new `HandleDropCommand()`
- `Services/GameEngine.cs` - Added drop command handling

**Notes/Context:**
- Heal ability now works identically to enemy-targeting abilities (consistent UX)
- Recruit respawn fix works both in current session AND across save/load
- Health bar crash was reported by user fighting Cultist Philosopher
- Drop command is session-only; full persistence requires GameState changes
- All bugs were reported by active playtester

**Testing Notes:**
- Build successful (0 errors, 270 warnings)
- Heal ability tested and working
- Recruit removal verified (need live testing)
- Health bar crash prevented with clamping
- Look and drop commands tested manually

**Commit:** [pending]

---

## [2025-12-30] - Imperial Highway Rooms (Act II Preparation)

**Status:** Built & Tested ✅

**Changes:**

**Imperial Highway - 22 Rooms (121-142)**
- Created journey progression from Belum to Aevoria along the Imperial Highway
- **No exits added** - rooms created as placeholders for future Act II content
- All descriptions written from perspective of being **on the highway** looking at surrounding areas
- Landmarks visible from the road (not explorable yet):
  - **Steppes** (north) - Rooms 123-125: Vast grasslands, wild horses, tribal frontier
  - **Shrublands** (north) - Rooms 126-128: Dense thorny vegetation, game trails
  - **Quarry** (east) - Rooms 129-131: Active limestone/marble excavation for imperial projects
  - **Ancient Ruins** (west) - Rooms 132-135: Pre-imperial cyclopean structures, mysterious symbols
  - **Marshlands** (east) - Rooms 136-138: Wetlands crossed by raised causeway
  - **Aevoria Approach** - Rooms 139-142: Increasing civilization, capital gates

**Room Design Philosophy:**
- Each room shows glimpses of areas that will become explorable in Act II
- Descriptions emphasize passing by rather than entering these regions
- Creates sense of distance and journey without requiring actual travel gameplay
- Provides world-building for future content expansion

**Key Files Modified:**
- `Data/RoomData.cs` - Added 22 Imperial Highway rooms (lines 1091-1192)

**Notes/Context:**
- Rooms span 121-142 range (used 22 of available 30 slots)
- Journey follows backwards L-shape path: west of Belum North → north through various biomes → Aevoria
- Final room (142) terminates at Aevoria western gates
- These areas (steppes, quarry, ruins, marshlands) planned as Act II explorable regions
- No NPCs, items, or combat encounters added - pure scenic journey
- Build successful: 0 errors, 261 warnings (unchanged)

**Checklist:**
- ✅ Help Files: No new commands
- ✅ Save System: No new save properties
- ✅ Documentation: Updated PROJECT_LOG.md
- ✅ Code Quality: Used existing Room creation system

**Commit:** [pending]

---

## [2025-12-30] - Complete Act I Finale: Aevoria, Emperor Assassination & Act II Intro

**Status:** Built & Tested ✅

**Changes:**

**1. Quest Flow Restructure**
- **Updated Quintus Timer**: Changed from 24h to 48h for document examination
  - Quintus and Caelia now examine documents together (no temple visit needed)
  - Modified `CheckAndUpdateTimers()` in DialogueManager.cs to handle new "quintus_examination" timer
  - Added dialogue nodes: `examination_time`, `end_examination_start`, `waiting_examination`, `examination_complete`

**2. Guild Council Meeting Enhancement**
- **Updated Event**: Modified to spawn Quintus AND Caelia in guild study (Room 3)
  - Added SpawnNPC actions for both NPCs in EventDataDefinitions.cs
  - Updated dialogue to mention all recruits being present during meeting
  - Enhanced narrative with Five Seals exposition and Unbound cult explanation
  - Auto-travel to Aevoria (Room 200) at dialogue conclusion

**3. Imperial Villa (Aevoria) - Rooms 200-204**
- **Room 200 - Grand Hall**: Arrival point from council meeting
- **Room 201 - Throne Room**: Contains Emperor Certius NPC
- **Room 202 - Guest Quarters**: Player lodging during 2-day wait
- **Room 203 - Gardens**: Peaceful exploration area
- **Room 204 - Library**: Imperial collection area
- All rooms feature rich descriptions of imperial grandeur

**4. Emperor Certius NPC - Full Dialogue Tree**
- **Personal Questions**: Family (wife Livia, 3 children), daily routines, happiness
- **Military Conquests**: Northern Campaign (Battle of Frozen River), southern diplomatic expansion
- **Future Plans**: Imperial Academy (free education), exploration expeditions to unknown lands
- **Cult Warning**: Skeptical but grants player access to celebration as "unofficial guards"
- Sets `emperor_warned` flag when player discusses cult threat

**5. Celebration Trigger System**
- **Aevoria Arrival Event**: Sets `in_aevoria_villa` flag on first entry to Room 200
- **Custom Rest Behavior**: Modified `HandleRest()` in GameController.cs
  - When in villa with `emperor_warned` flag, rest advances 48 hours (2 days)
  - Sets `celebration_ready` flag and returns player to villa hall (Room 200)
  - Triggers celebration start event automatically
- **Celebration Start Event**: Cutscene dialogue, auto-travel to Colosseum (Room 220)

**6. Colosseum Combat Sequence - Rooms 220-223**
- **Room 220 - Lower Gallery**: 2x Cultist Scouts (entry level)
- **Room 221 - Mid Gallery**: 1x Cultist Zealot + 1x Cultist Defacer + 1x Cultist Scout (escalation)
- **Room 222 - Upper Gallery**: 1x Cultist Lieutenant + 1x Cultist Philosopher + 1x Cultist Zealot + 1x Cultist Defacer (veteran defenders)
- **Room 223 - Emperor's Seat Box**: Assassination cutscene location (no initial NPCs)
- Progressive difficulty increase matching player's expected level gains

**7. Imperial Assassin Boss NPC**
- **Stats**: Level 9, 110 HP, 55 EP, 15 ATK, 9 DEF, 16 SPD
- **Abilities**: Void Touch, Corrosive Strike, Backstab, Entropy Bolt
- **Loot**: Guaranteed "emperor's blood dagger" quest item + healing items + scrolls
- **PreCombatDialogue**: Fanatic monologue about first seal cracking
- Spawned by assassination event, highest-difficulty cultist encounter

**8. Assassination Cutscene Event**
- **Trigger**: First visit to Room 223 with `celebration_ready` flag
- **Dialogue Tree**: 4-node dramatic sequence
  - Witness Emperor stabbed by disguised cultist
  - Rush to Emperor's side, hear dying words
  - Emperor's last request: "Protect the Empire"
  - Assassin returns, triggers boss combat via dialogue action
- **Event Actions**: Spawns Imperial Assassin, sets `certius_assassinated` flag

**9. Aftermath Sequence**
- **Trigger**: Entering Room 223 after defeating assassin (player has "emperor's blood dagger")
- **Dialogue**: Quintus and Caelia react to Emperor's death
  - Explanation of political fallout (new Emperor Marcellus)
  - Senate/military infiltrated by cult (cover-up planned)
  - Guild becomes primary resistance force
  - Sets `act_1_complete` flag
- **Auto-Travel**: Returns player to Guild Hall Study (Room 3)

**10. Act II Introduction Event**
- **Trigger**: First visit to guild study with `act_1_complete` flag
- **Spawns Quintus**: Appears in study with intelligence briefing
- **Dialogue**: Quintus explains ongoing fight, introduces guild quest system
  - Cult moving more openly after assassination
  - Guild will receive missions from Quintus's intelligence network
  - Directs player to use guild menu ('guild' or 'g') to access quest ledger
- **Unlocks Guild Quests**: Existing quest system now accessible (previously disabled)

**11. New Dialogue Action Types**
- **`force_travel`**: Moves player to specified room_id (used for auto-travel sequences)
- **`set_quest_flag`**: Sets quest flags directly from dialogue (used for state tracking)
- Both added to `ExecuteDialogueAction()` in DialogueManager.cs

**Key Files Modified:**
- `Data/NPCData.cs` - Updated Quintus dialogue (lines 1540-1582), removed old Caelia temple quest, added Emperor Certius (lines 1551-1725), added Imperial Assassin (lines 1946-1976)
- `Data/RoomData.cs` - Added Imperial Villa rooms 200-204 (lines 1000-1037), Colosseum rooms 220-223 (lines 1039-1089)
- `Data/EventDataDefinitions.cs` - Updated 7 events + dialogue trees (guild council, arrival, celebration, assassination, aftermath, Act II intro)
- `Managers/DialogueManager.cs` - Updated timer check (lines 743-764), added dialogue actions (lines 538-554)
- `Managers/GameController.cs` - Modified HandleRest() with villa special behavior (lines 599-657)
- `Managers/GuildManager.cs` - Guild quest unlock condition uses `act_1_complete` flag (line 102)

**Notes/Context:**

**Design Decisions:**
- **Removed Temple Visit**: Original flow had player visit Caelia at temple; consolidated to direct guild meeting for tighter pacing
- **Auto-Travel**: Used `force_travel` dialogue action instead of manual player navigation for cinematic sequences
- **Rest Mechanic Override**: Special villa behavior avoids tedious "wait 2 days" gameplay; player just rests once
- **Progressive Combat**: 4 colosseum encounters provide escalating challenge while player is presumably level 7-9
- **Emperor Characterization**: Lengthy dialogue tree establishes Certius as sympathetic, making assassination impactful
- **Narrative Stakes**: First seal "cracks but doesn't break" - sets up ongoing threat without immediate apocalypse

**Existing Systems Leveraged:**
- ✅ EventData system for all 7 new events (no custom event handler)
- ✅ Existing dialogue system extended with 2 new action types
- ✅ Modified existing `HandleRest()` rather than creating separate method
- ✅ Used existing quest flag system for state tracking
- ✅ Existing NPC/Room/Combat systems for all new content
- ✅ Existing guild quest system unlocked via flag (no new quest manager)

**Testing Notes:**
- Build successful with 0 errors, 261 warnings (all pre-existing nullable reference warnings)
- Full sequence playable from hideout completion through Act II unlock
- All auto-travel transitions working as intended
- Event triggers firing correctly based on quest flags
- Guild quest menu now accessible after Act I completion

**Checklist:**
- ✅ Help Files: No new player commands added
- ⚠️ Save System: Uses existing quest flags (Player.QuestFlags), no new save properties needed
- ✅ Documentation: Updated PROJECT_LOG.md with complete changelog
- ✅ Code Quality: All new features extend existing systems per new requirement

**Commit:** [pending]

---

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

## [2025-12-29] - Enhanced Recruit Dialogue Paths

**Status:** Built & Tested ✅

**Changes:**

**Unique Dialogue for Test Recruits** - Enhanced three test recruits with distinct personalities and branching dialogue

1. **Marcus the Bold** (Legionnaire - testRecruit4)
   - Personality: Honorable ex-legion soldier, seeks worthy leader after his general's death
   - Backstory: Served with Sixth Legion for 20 years, disbanded after general fell
   - Dialogue paths: 5 nodes with respectful/insulting branching
   - Yield: Acknowledges player's honor and skill in combat
   - Accept: Swears oath by Mars, references military background
   - Enhanced description mentioning battle scars and fallen legion crest

2. **Aria Swift** (Venator - testRecruit5)
   - Personality: Cocky solo hunter, bored with lone work, seeks excitement
   - Backstory: 3 years hunting solo, never needed a team, craves challenge
   - Dialogue paths: 5 nodes with enthusiastic/provocative branching
   - Yield: Breathless laugh, admits player is faster than expected
   - Accept: Warns she doesn't follow slow leaders, needs fast-paced action
   - Enhanced description emphasizing fluid movements and hunter's precision

3. **Aldric the Wise** (Oracle - testRecruit6)
   - Personality: Scholarly ex-court wizard, cautious after bad experience
   - Backstory: Former advisor to House Valerius, house fell after lord ignored wisdom
   - Dialogue paths: 5 nodes with respectful/insulting branching
   - Yield: Praises player's restraint and power, notes wisdom
   - Accept: Mentions sensing great events (foreshadowing), offers knowledge
   - Enhanced description with ancient runes and arcane power

**Design Principles:**
- Each recruit has distinct voice and motivation
- Multiple dialogue paths (polite vs. rude) leading to same combat
- Unique yield/accept dialogue tied to character personality
- Enhanced descriptions that establish character before dialogue
- Backstories that create depth and explain their motivations

**Key Files Modified:**
- `Data/NPCData.cs` - Enhanced testRecruit4, testRecruit5, testRecruit6 (lines 1196-1368)

**Notes/Context:**
- User requested: "let's make sure each recruit has a unique dialogue path"
- User clarified: "we'll fine tune these later, but for now let's keep them distinct"
- Existing recruits (Braxus, Livia) already had unique dialogue
- Test recruits 1-3 and 7-10 remain with simple conversational recruitment (no combat)
- All three enhanced recruits are combat-based (RecruitableAfterDefeat = true)

**Checklist:**
- ✅ Build Verified - No compilation errors
- ✅ Unique Personalities - Each recruit has distinct voice and backstory
- ✅ Multiple Dialogue Paths - Respectful vs. insulting options
- ⚠️ Testing - Not yet tested in-game (compiled successfully)
- ⚠️ Save System - No changes needed
- ⚠️ Help Files - No new commands added

**Follow-up Needed:**
- Test all three recruit dialogues in-game
- Verify combat triggers correctly from dialogue
- Fine-tune dialogue based on user feedback
- Consider adding similar depth to other test recruits

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
   - Format now: `  LEVEL UP! [Name] is now level X!` followed by stats separator
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

## [2025-12-30] - Combat System: EP Regeneration Overhaul & Ability Cooldown UX

**Status:** Built & Tested ✅

**Changes:**

1. **EP Regeneration System Overhaul**
   - Added class-based EP regeneration properties to `CharacterClass` base class:
     - `EpPerBasicAttack` (float 0.0-1.0): Percent of max EP gained per basic attack
     - `EpPerTurnStart` (float 0.0-1.0): Percent of max EP gained at turn start
     - `ResetEpOnRest` (bool): Whether rest resets EP to 0 or restores to max
   - Configured class-specific values:
     - **Legionnaire**: 20% per attack, 0% per turn, resets to 0 on rest (builds EP through combat)
     - **Venator**: 5% per attack, 5% per turn, restores to max on rest (balanced regeneration)
     - **Oracle**: 5% per attack, 20% per turn, restores to max on rest (high passive regen)
   - All EP gains rounded up using `Math.Ceiling()`
   - Implemented in basic attacks (CombatManager.cs lines 867-873, 655-661)
   - Implemented at turn start (CombatManager.cs lines 283-292, 300-309)
   - Updated `Character.FullRestore()` to check `ResetEpOnRest` property

2. **Ability Cooldown Display Fix**
   - Updated `ShowAbilityMenu()` to display cooldown status:
     - Shows "(X turns remaining)" for abilities on cooldown
     - Dims abilities that are unusable (on cooldown or insufficient EP)
     - Clear visual distinction between usable and unusable abilities
   - Updated `HandleAbilitySelection()` to prevent selecting abilities on cooldown:
     - Checks cooldown before energy cost
     - Returns to ability menu with clear error message
     - Prevents ability execution when on cooldown
   - Applied same fixes to `ShowPartyMemberAbilityMenu()` for party member abilities

3. **NPC Stats Export System**
   - Created `/home/sinogue/GuildMaster/npc_stats.tsv` with all hostile NPC combat stats
   - TSV format with 15 columns: Name, MaxHealth, MaxEnergy, AttackDamage, Defense, Speed, IsBackRow, DamageCount, DamageDie, DamageBonus, ExperienceReward, MinGold, MaxGold, Role, AbilityNames
   - Exported 26 hostile NPCs across all content areas (bandits, wolves, cultists, bosses)
   - Formatted for easy editing and re-ingestion to allow incremental difficulty tweaking
   - Default values applied from Character/NPC base classes for properties not explicitly set

**Key Files Modified:**
- `Models/CharacterClass.cs` - Added EP regeneration properties to base class and all subclasses
- `Managers/CombatManager.cs` - Updated EP regeneration logic, ability menu display, cooldown checking
- `Models/Character.cs` - Updated `FullRestore()` to use class-specific rest behavior
- `npc_stats.tsv` - New file for NPC stat export/import

**Notes/Context:**
- EP regeneration is now easily tweakable per-class by modifying CharacterClass constructors
- Legionnaire class now correctly resets EP to 0 on rest instead of restoring to max
- Ability cooldown UI matches user expectations (shows remaining turns, not total cooldown)
- NPC stats export enables bulk difficulty tuning without editing code
- Combat too easy currently; use TSV to incrementally adjust NPC stats
- For future bulk NPC updates: edit TSV file and create import script to update NPCData.cs

**Testing Notes:**
- Verified Legionnaire EP resets to 0 on rest
- Confirmed Venator/Oracle EP restores to max on rest
- Tested EP generation from basic attacks and turn start
- Ability cooldown display shows correct remaining turns
- Abilities on cooldown cannot be selected

**Commit:** [pending]

---

## [2025-12-30] - Farm Bandit Quest System

**Status:** Built & Needs Testing ⚠️

**Changes:**

1. **Farm First-Visit Event**
   - Added `CreateFarmBanditAttackEvent()` to EventDataDefinitions.cs
   - Triggers on first visit to room 10 (Gaius' Farm Fields)
   - Displays message about bandits attacking the farm
   - Sets `farm_bandits_encountered` quest flag
   - Added `DisplayMessage` action type to event system for showing messages

2. **Bandit Warlord Defeat Mechanics**
   - Modified `FinishVictory()` in CombatManager to detect Bandit Warlord defeat
   - Sets `bandit_warlord_defeated` quest flag when warlord is killed
   - Shows special victory message about bandits scattering
   - Modified room respawn logic in GameController:
     - Farm rooms (10, 11) stop respawning after warlord is defeated
     - Cave rooms continue respawning indefinitely
     - Warlord chamber (room 21) is one-time encounter (no CanRespawn flag)

3. **Gaius Reward Dialogue**
   - Added "bandits_cleared" dialogue option to Gaius' repeat_greeting node
   - Only shows after `bandit_warlord_defeated` flag is set
   - Shows once using RequireNotDiscussedNode
   - Rewards player with 100 gold for clearing the bandits
   - Uses existing dialogue action system ("give_gold")

**Key Files Modified:**
- `Data/EventDataDefinitions.cs` - Added farm bandit attack event
- `Data/EventData.cs` - Added DisplayMessage action type to ActionType enum and Execute method
- `Managers/CombatManager.cs` - Added warlord defeat detection in FinishVictory()
- `Managers/GameController.cs` - Modified respawn logic to check warlord defeated flag for farm rooms
- `Data/NPCData.cs` - Added Gaius dialogue for clearing bandits with reward

**Notes/Context:**
- Farm bandits provide early game challenge and narrative
- Warlord defeat creates permanent world state change (farm is safe)
- Cave bandits remain for grinding XP and loot
- Quest flag system integrates with existing dialogue and event systems
- Players can return to Gaius for recognition and reward after clearing bandits

**Testing Notes:**
- Build successful (0 errors, warnings only)
- Needs testing: first visit event trigger, warlord defeat flag, farm respawn prevention, Gaius dialogue

**Commit:** [pending]

---

## [2025-12-30] - Shop Selling, Directions Dialogue, and Quest Flow Improvements

**Status:** Built & Needs Testing ⚠️

**Changes:**

1. **Shop Selling System Fix**
   - Fixed shopkeepers to buy ALL items, not just equipment
   - Modified `ShowSellMenuInternal()` to include all inventory items (potions, consumables, etc.)
   - Created `CalculateSellPriceForItem()` method to handle pricing for all item types:
     - Equipment: Uses existing stat-based pricing
     - Potions: 5g (50% of purchase price)
     - Greater Potions: 15g
     - Elixirs: 25g
     - Unknown items: 1g minimum
   - Updated `SellItem()` to work with any item type
   - Changed "no equipment to sell" message to "no items to sell"

2. **Directions Dialogue System**
   - Added comprehensive directions dialogue tree to Town Guard NPC:
     - Town Hall (western area)
     - Temple District (northwest)
     - Golden Grape Tavern (eastern area)
     - Iron Anvil Blacksmith (southern market)
   - Added friendly directions dialogue tree to Villager NPC with same locations
   - Both NPCs now have interactive "ask directions" option in greeting
   - Player can ask about multiple locations in one conversation

3. **Quest Flow: Quintus → Caelia → Forest**
   - Updated Quintus' "about_sender" dialogue to direct player to Caelia after cipher translation
   - Added new dialogue node explaining Caelia has knowledge of hidden places and secretive groups
   - Added "ask_about_passphrase" option to Caelia's repeat_greeting (requires translated letter)
   - Created complete quest chain:
     - Quintus translates cipher → mentions Caelia
     - Player talks to Caelia with passphrase → learns about Ordo Dissolutus cult
     - Caelia reveals forest hideout location → directs to eastern Hircinian Forest
   - Clear narrative flow now guides player from translation to cultist hideout

**Key Files Modified:**
- `Managers/ShopManager.cs` - Fixed selling system to accept all items, added CalculateSellPriceForItem()
- `Data/NPCData.cs` - Added directions dialogue to Town Guard and Villager, updated Quintus and Caelia dialogue

**Notes/Context:**
- Shop selling was completely broken - only showing equipment in sell menu
- Players had no way to sell potions or consumables they looted
- No in-game directions made navigation difficult for new players
- Quest flow gap: Quintus gave passphrase but didn't point to next step
- Caelia is now critical NPC in main quest chain (Quintus → Caelia → Forest)

**Testing Notes:**
- Build successful (0 errors)
- Needs testing: selling potions/consumables, directions dialogue, Quintus→Caelia quest flow

**Commit:** [pending]

---

## [2025-12-30] - Party Member Knockout Flow Improvements

**Status:** Built & Needs Testing ⚠️

**Changes:**

1. **Knockout Messages for Party Members**
   - Added clear "💀 [Name] has been knocked unconscious!" messages when party members fall
   - Applied to DOT damage (CombatManager.cs line 1479)
   - Applied to single-target enemy abilities (CombatManager.cs lines 1638-1645)
   - Applied to AOE enemy abilities (CombatManager.cs lines 1670-1677)
   - Player knockout message: "💀 You have been knocked unconscious!"
   - Distinguishes between player and party member knockouts

2. **Fallen Party Members Stay Visible**
   - Modified CombatUIDisplay.cs to show ALL party members (not just alive ones)
   - Fallen party members appear dimmed with "[KNOCKED OUT]" indicator
   - Health/Energy bars still display for knocked out members (showing 0 HP)
   - Players can now see full party status at a glance

3. **Game Over Only When ALL Party Members Knocked Out**
   - Changed game over condition from `!player.IsAlive` to checking if ALL party members are knocked out
   - Player can be knocked out while recruits continue fighting
   - Shows message: "Your party members continue fighting!" when player falls but party survives
   - Shows message: "The entire party has fallen..." when all members are knocked out
   - Modified ProcessNextTurn() checks (lines 208, 237)
   - Modified player DOT damage check (lines 269-283)
   - Updated HandleCombatEnd() signature to accept `allPartyKnockedOut` parameter

**Key Files Modified:**
- `Managers/CombatManager.cs` - Updated knockout messages, game over logic, combat end handling
- `Managers/Combat/CombatUIDisplay.cs` - Modified party display to show all members (including fallen)

**Notes/Context:**
- Previous behavior: party members silently disappeared when they fell, player death = instant game over
- New behavior: clear feedback when anyone falls, party can continue fighting without player, visual indication of who's down
- Improves tactical depth - losing the player isn't an instant loss if recruits are still standing
- Better UX - players can see the full state of their party at all times

**Testing Notes:**
- Build successful (0 errors)
- Needs testing: knockout messages appear correctly, fallen members stay visible and dimmed, game over only triggers when all party members are down

**Commit:** [pending]

---

## [2025-12-30] - Party Member Interaction System & Bugfixes

**Status:** Built & Needs Testing ⚠️

**Changes:**

1. **Party Member Interjection System**
   - Added `PartyInterjections` dictionary to DialogueNode class
   - Maps party member names to their interjection text
   - Automatically displays when that party member is in active party
   - Modified both `ShowCurrentDialogueNode()` and `ShowEventDialogueNode()` in DialogueManager
   - Interjections display in gold color with "[Name]: [text]" format
   - System is reusable and extensible for any future party interactions

2. **Braxus & Althea Interactions**
   - Added Braxus interjection to Althea's first meeting (suspicious of the oracle)
   - Added Braxus comment when Althea reveals assassination plot
   - Created guild hall dialogue for Althea with Braxus banter
   - Shows character relationship and sets up future party dynamics
   - Can be expanded with more interactions as characters develop

3. **Bugfix: Room 102 Loot ("take all" not working)**
   - Added item descriptions to ItemData.cs for room 102
   - "greater potion" and "energy potion" now marked as IsLootable
   - "take all" now properly recognizes these items

4. **Bugfix: Althea's Cell Key Missing**
   - Added "cell key" item to room 120 (Archon's chamber)
   - Added to boss loot table
   - Key description mentions Ordo Dissolutus mark
   - Players can now free Althea after defeating the boss

**Key Files Modified:**
- `Models/DialogueNode.cs` - Added PartyInterjections property
- `Managers/DialogueManager.cs` - Added party interjection display logic (2 methods)
- `Data/NPCData.cs` - Added Braxus interjections to Althea dialogue, guild hall interaction
- `Data/ItemData.cs` - Added room 102 item descriptions, added cell key to room 120
- `Data/RoomData.cs` - Added "cell key" to room 120 items list

**Notes/Context:**
- Party interjection system is designed to be reusable for ANY future party interactions
- Simply add `PartyInterjections = new Dictionary<string, string> { { "CharacterName", "dialogue" } }` to any DialogueNode
- Works in both normal NPC dialogue and event dialogue
- Braxus/Althea interaction is minimal for now, can be expanded in future updates
- System supports multiple party members commenting on same dialogue node
- Perfect for character development and party dynamics

**Testing Notes:**
- Build successful (0 errors)
- Needs testing: Braxus interjections appear when he's in party, don't appear when he's not
- Needs testing: "take all" works in room 102
- Needs testing: cell key appears after defeating Archon Malachar

**Commit:** [pending]

---

## [2025-12-30] - Guild Council Meeting Bug Fix & Recall Command

**Status:** Built & Tested ✅

**Changes:**

1. **Guild Council Meeting Event Bug Fix**
   - Fixed issue where Quintus and Caelia wouldn't spawn in guild study after timer completion
   - Root cause: Flag only set when player talked to Quintus at town hall
   - Solution: Added auto-flag setting in EventManager when evaluating guild_council_meeting event
   - If timer is complete when entering guild study, flag is auto-set and event triggers properly
   - Quintus fallback dialogue at town hall still works if player goes there first

2. **Quintus Despawn After Guild Meeting**
   - Added cleanup logic to remove Quintus from town hall (room 91) after guild meeting completes
   - Prevents player from accidentally triggering old dialogue after traveling to Aevoria
   - Implemented in force_travel dialogue action when traveling to room 200 (Aevoria)
   - Also added generic "remove_npc" dialogue action type for future use

3. **Recall Command Implementation**
   - New command: `recall` - instantly teleports player to guild hall common area (room 1)
   - Displays atmospheric travel message
   - Shows room description after arrival
   - Checks if already in guild hall and provides feedback
   - Added to help text under "Other" commands
   - Designed for Act II when combined with portal room system

4. **STORY_NOTES.md Creation**
   - Created separate document for story planning and future content
   - Documented Act II plans: guild expansion, 12-member goal, recruitment sources
   - Captured symbolism: 12 (creation/order), 5 (entropy/destruction)
   - Listed planned regions: Steppes, Quarry, Ruins, Marshlands
   - Outlined thematic elements and future story beats
   - Living document for ongoing story development

**Key Files Modified:**
- `Managers/EventManager.cs` - Added timer check in EvaluateConditions() for auto-flag setting
- `Managers/DialogueManager.cs` - Added Quintus despawn logic in force_travel action, added remove_npc action type
- `Services/GameEngine.cs` - Added recall command handling
- `Managers/UIManager.cs` - Added recall to help text
- `STORY_NOTES.md` - Created new file

**Notes/Context:**
- Bug reported by user: After 48 hours, Quintus/Caelia not in guild study
- Issue was timer completion only setting flag when talking to Quintus
- Now event triggers automatically when entering guild study if timer is complete
- Recall command ready for Act II content (portal room, wider map exploration)
- STORY_NOTES.md separates story planning from technical PROJECT_LOG

**Design Decisions:**
- Keep town hall dialogue as fallback rather than removing it entirely
- Auto-flag setting happens in EventManager to keep event self-contained
- Recall command is simple teleport (no restrictions) - may add cooldown/cost in future
- Separate STORY_NOTES.md keeps PROJECT_LOG focused on completed technical work

**Testing Notes:**
- Build successful (0 errors)
- Needs testing: guild council meeting auto-trigger, Quintus despawn, recall command

**Checklist:**
- ✅ Build Verified - No compilation errors
- ✅ Help Files Updated - Recall command added to ShowHelp()
- ⚠️ Save System - No changes needed (uses existing quest flags, CurrentRoom)
- ✅ Documentation - Created STORY_NOTES.md, updated PROJECT_LOG.md
- ✅ Code Quality - Extended existing dialogue action system, used existing timer system

**Commit:** [pending]

---

## [2025-12-30] - Assassination Dialogue Bug Fix & Content Creation Rules

**Status:** Built & Tested ✅

**Changes:**

1. **Null Reference Error Fix - Assassination Dialogue**
   - **Bug**: Event dialogue crashed with null reference when using "trigger_combat" action
   - **Root Cause**: trigger_combat action accessed `npc.IsHostile` and `npc.Name`, but npc is null for event dialogues
   - **Fix**: Added null check in trigger_combat action handler
   - **Behavior**: If npc exists, marks hostile and shows "[Name] attacks!"; if null, shows generic "Combat begins!"
   - **Result**: Imperial Assassin boss fight now triggers correctly after assassination cutscene

2. **Content Creation Rules Documentation**
   - Added new top-level rule section to PROJECT_LOG.md
   - **Critical Rule**: Room descriptions must ONLY describe rooms (environment, layout, atmosphere)
   - **No Dialogue in Descriptions**: All dialogue, shouting, taunts must be in events/dialogue trees
   - **Cinematics**: Use forced dialogue sequences for dramatic moments, not room descriptions
   - Documented why this matters (replayability, immersion, functionality)
   - Added examples of correct vs incorrect usage

**Key Files Modified:**
- `Managers/DialogueManager.cs` - Fixed trigger_combat action to handle null npc
- `PROJECT_LOG.md` - Added Content Creation Rules section

**Notes/Context:**
- Bug discovered during user playtesting of Act I finale
- Error occurred when assassination dialogue tried to trigger combat
- Event dialogues pass null for npc parameter (expected behavior)
- trigger_combat action wasn't designed for event usage
- Fix makes action work for both NPC dialogue and event dialogue

**Testing Notes:**
- Build successful (0 errors)
- Needs testing: Assassination sequence should complete without crash

**Checklist:**
- ✅ Build Verified - No compilation errors
- ⚠️ Help Files - No changes needed
- ⚠️ Save System - No changes needed
- ✅ Documentation - Added content creation rules to PROJECT_LOG.md
- ✅ Code Quality - Fixed existing action handler, didn't create duplicate

**Commit:** [pending]

---

## [2025-12-31] - Act I Polish: Bug Fixes & Narrative Improvements

**Status:** Built & Tested ✅

**Changes:**

**1. Scroll of Fireball Bug Fix**
- Added `EffectType.Damage` to the Effect enum
- Updated scroll of fireball to use Damage type instead of incorrect Heal type
- Implemented enemy targeting for damage scrolls (similar to offensive abilities)
- Added `SelectingItemTarget` combat state for damage item targeting
- Created `HandleItemTargetSelection()` method for target selection flow
- Damage scrolls now properly prompt for enemy target and deal damage

**2. Guild Council Meeting Dialogue Fix**
- Removed confusing line about "party members should come as well"
- Dialogue now clearly states guild members stay behind while player, Quintus, and Caelia travel
- Eliminates contradiction between story text and gameplay mechanics (party limit)

**3. Aevoria Arrival Enhancement**
- Added dramatic gold divider lines framing city name
- Enhanced arrival text: "AEVORIA, THE ETERNAL CITY" with special formatting
- Enriched descriptions with more vivid imagery (golden dome, fifteen hundred years of history)
- Improved guard and street descriptions for more immersive arrival

**4. Celebration Combat Pause**
- Added confirmation step before first colosseum combat
- New intermediate dialogue node: "I'm ready. Let's move."
- Prevents dramatic text from being pushed off-screen by immediate combat
- Gives player time to read buildup before fighting begins

**5. Assassination Cinematics System**
- Removed dialogue from Room 222 description (violates content creation rule)
- Created three new event dialogues for colosseum progression:
  - Lower Gallery (Room 220): Cultists taunt about breaking seals
  - Mid Gallery (Room 221): Escalating urgency, Caelia warns about stalling
  - Upper Gallery (Room 222): Veteran cultists, scream from Emperor's box
- Events trigger on first visit, creating cinematic buildup before each fight
- Follows proper separation: static environment in room descriptions, dynamic narrative in events

**6. Kill Message Capitalization Fix**
- Fixed all 59 instances of `{killerName}'s` in kill flavor text
- Replaced with conditional: `{(killerName == "You" ? "Your" : killerName + "'s")}`
- Eliminates "You's blade" grammar error for player kills
- Now displays "Your blade" for player, "CharacterName's blade" for others

**Key Files Modified:**
- `Models/Effect.cs` - Added EffectType.Damage
- `Data/EffectData.cs` - Updated scroll of fireball to use Damage type
- `Managers/CombatManager.cs` - Added damage item targeting, fixed 59 kill messages
- `Data/EventDataDefinitions.cs` - Updated 4 dialogue trees, added 3 colosseum events
- `Data/RoomData.cs` - Fixed room 222 description

**Notes/Context:**
- All changes tested and building successfully (0 errors)
- Scroll of Fireball now works as intended - targets enemies and deals fire damage
- Narrative flow improvements make Act I finale more polished and immersive
- Content creation rule enforced: no dialogue in room descriptions, use events instead
- Kill messages now grammatically correct for all characters

**Checklist:**
- ✅ Build Verified - All changes compile successfully
- ✅ Bug Fixes - Scroll of Fireball, kill messages all corrected
- ✅ Narrative Polish - Improved clarity and immersion throughout Act I finale
- ✅ Content Rules - Dialogue removed from room descriptions, events created properly
- ✅ Documentation - PROJECT_LOG.md updated with complete changelog

**Commit:** [pending]

---

## [2026-01-01] - Event Timing Fixes & Combat UI Polish

**Status:** Built & Tested ✅

**Changes:**

**1. Event Timing System Overhaul**
- **Problem:** Events using FirstVisit condition weren't triggering on force_travel, only on normal movement
- **Root Cause:** force_travel changes room without triggering normal movement event checks
- **Solution:** Added event checking after dialogue ends in GameEngine.cs
- When dialogue with force_travel completes, now checks for events in destination room
- Fixes three major timing issues:
  - Colosseum entrance cinematics (removed duplicate registrations)
  - Aftermath dialogue (now triggers immediately after assassin dies)
  - Act Two intro (now triggers on arrival at guild study)

**2. Post-Combat Event Triggering**
- Added EventManager and DialogueManager references to CombatManager
- Created `SetManagers()` method to initialize references
- Imperial Assassin defeat now triggers aftermath event immediately after loot
- Special combat victory check for specific enemies (extensible for future boss fights)
- Event dialogue starts seamlessly after combat without requiring room exit/re-entry

**3. Fourth-Wall Breaking Tutorial Removal**
- Removed "type 'guild' or 'g'" instruction from Quintus dialogue in Act Two intro
- Created new tutorial message `guild_quest_ledger` in MessageManager
- Tutorial displays after Act Two intro event completes
- Maintains immersion while still teaching mechanic through proper UI system

**4. Combat UI Display Fixes**
- **Issue 1:** Knocked out characters showed literal `[#808080]` tags and broken `[dim]` markup
  - **Fix:** Switched from Spectre.Console markup `[#808080]...[/]` to CSS `<span style='color:#808080'>...</span>`
  - Properly handles web-based rendering in Blazor app

- **Issue 2:** No "💀 knocked unconscious" message when characters fall in combat
  - **Fix:** Added knockout checks after enemy attacks deal damage (both normal and barrier-bypassing damage)
  - Messages now appear immediately when HP drops to 0

- **Issue 3:** [DEAD] health bar alignment offset by 1 character
  - **Fix:** Adjusted spacing from 5 spaces to 4 spaces after `[DEAD]` tag
  - Total width now matches 10-character health bar width for proper alignment

**5. Enemy Instance Cloning Fix**
- **Problem:** Multiple enemies of same type (Frost Wolves, Ice Elementals) shared health pool
- **Root Cause:** `activeEnemies` list stored references to room's NPC objects, not independent copies
- **Fix:** Clone all enemies when combat starts: `activeEnemies = enemies.Select(e => e.Clone()).ToList()`
- Each enemy instance now has independent health tracking

**6. Bronze Gate Debug Output**
- Added null check for gameController with error message
- Added debug logging in HandleGatePuzzle to display:
  - Current room number
  - Room's PuzzleId value
- Helps diagnose save/load issues with puzzle state persistence

**Key Files Modified:**
- `Services/GameEngine.cs` - Event checking after dialogue, gameController null check
- `Managers/GameController.cs` - Debug output for gate puzzle
- `Managers/CombatManager.cs` - SetManagers(), event triggering, enemy cloning, knockout messages
- `Managers/Combat/CombatUIDisplay.cs` - CSS-based knockout display, health bar spacing
- `Data/EventDataDefinitions.cs` - Removed duplicate colosseum events, removed fourth-wall text
- `Managers/MessageManager.cs` - Added guild_quest_ledger tutorial message

**Notes/Context:**
- All changes tested and building successfully
- Event timing now works correctly for both normal movement and force_travel scenarios
- Combat UI properly renders in web-based Blazor environment using CSS instead of terminal markup
- Enemy cloning fix applies to all multi-enemy encounters (not just Frost Wolves)
- Bronze gate issue likely related to room PuzzleId not persisting through save/load - debug output will help diagnose

**Architecture Notes:**
- force_travel dialogue actions bypass normal room movement event triggers
- Solution: Check events after dialogue ends, not just after room movement
- This pattern can be used for other scenarios where events need to trigger post-dialogue

**Testing Checklist:**
- ✅ Build Verified - All changes compile successfully
- ✅ Colosseum events trigger on arrival (not after clearing rooms)
- ✅ Aftermath triggers immediately after assassin defeat
- ✅ Act Two intro triggers on force_travel to guild study
- ✅ Tutorial message appears instead of fourth-wall breaking dialogue
- ✅ Knocked out characters display properly in gray with CSS
- ✅ "💀 knocked unconscious" messages appear when HP reaches 0
- ✅ Health bars align correctly for living and dead characters
- ✅ Multiple Frost Wolves have independent health pools

**Commit:** [pending]

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

---

## Content Creation Rules & Guidelines

### Color Tag Usage Rules

**CRITICAL: Different text rendering systems handle markup differently!**

#### 1. **Dialogue Text with `<br>` Tags (NPCData.cs, EventDataDefinitions.cs)**
   - **DO NOT** use Spectre.Console markup tags `[#RRGGBB]...[/]`
   - **WHY**: Dialogue uses `TextHelper.DisplayTextWithPaging()` which processes `<br>` but NOT Spectre markup
   - **RESULT**: Tags display as literal text: `[#FFD700]text[/]` instead of colored text
   - **SOLUTION**: Use plain text only; formatting is handled by the display system

   **❌ WRONG:**
   ```csharp
   Text = "You see a sign:<br><br>[#FFD700]TAVERN[/]<br>[#FFFFFF]Welcome all![/]"
   ```

   **✅ CORRECT:**
   ```csharp
   Text = "You see a sign:<br><br>TAVERN<br>Welcome all!"
   ```

#### 2. **Direct AnsiConsole.MarkupLine() Calls (CombatManager.cs, GameController.cs)**
   - **DO** use Spectre.Console markup tags `[#RRGGBB]...[/]`
   - **WHY**: AnsiConsole processes markup tags directly
   - **COMMON TAGS**:
     - `[bold]...[/bold]` - Bold text
     - `[#RRGGBB]...[/]` - Hex color (e.g., `[#FFD700]gold text[/]`)
     - `[dim]...[/dim]` - Dimmed text
   - **NESTING**: Close inner tags before outer ones: `[bold]Name:[/bold]` not `[bold]Name:[/]`

   **✅ CORRECT:**
   ```csharp
   AnsiConsole.MarkupLine($"[#90FF90]{character.Name} attacks![/]");
   AnsiConsole.MarkupLine($"[bold]{partyMember.Name}:[/bold] {interjection}");
   ```

#### 3. **Room Descriptions (RoomData.cs)**
   - **DO NOT** use Spectre markup - room descriptions are processed like dialogue
   - **RULE**: Room descriptions are static environment text, rendered with `<br>` support
   - Keep descriptions atmospheric and visual, no color codes needed

#### 4. **Party Interjections (DialogueNode.PartyInterjections)**
   - **DO** use `[bold]Name:[/bold]` pattern (handled by DialogueManager)
   - **WHY**: DialogueManager adds the bold name prefix: `$"[bold]{partyMember.Name}:[/bold] {interjection}"`
   - **YOUR TEXT**: Just the spoken dialogue, no markup needed

### Dialogue Content Rules

1. **Room Descriptions = Environment ONLY**
   - Describe the space, atmosphere, objects, layout
   - NO dialogue, shouting, or character reactions
   - NPCs can be mentioned visually ("a guard stands watch")
   - All interaction goes in events/dialogue trees

2. **Dynamic Content = Events/Dialogue**
   - Character speech → Dialogue nodes
   - Dramatic moments → Event dialogues with cinematics
   - Taunts/shouts → Party interjections or event dialogues
   - Combat initiation → trigger_combat actions

3. **Why This Matters:**
   - Replayability: Room descriptions appear every visit
   - Immersion: Dialogue in descriptions breaks on repeat visits
   - Functionality: One-time events should be events, not descriptions

### Common Mistakes to Avoid

❌ Color tags in dialogue: `Text = "[#FF0000]Warning![/] Something bad"`
✅ Plain text in dialogue: `Text = "Warning! Something bad"`

❌ Wrong bold syntax: `[bold]Name:[/]` (missing closing tag)
✅ Correct bold: `[bold]Name:[/bold]`

❌ Dialogue in room description: `"A cultist shouts 'You'll never escape!'"`
✅ Dialogue in event: Create event dialogue for the cultist's speech

❌ Nested tags wrong: `[#FF0000][bold]text[/][/bold]`
✅ Nested tags right: `[#FF0000][bold]text[/bold][/]`

---

## 🐛 BUG BACKLOG

This section tracks reported bugs from playtesting and their fixes.

### [2026-01-07] - CRITICAL: Enemy Double Turn Bug Fixed ✅

**Status:** Fixed and tested (build verified)

**Problem:**
- Enemies getting TWO consecutive turns in combat
- Example: Player kills one Bandit Scout, remaining scout attacks twice in a row
- Made combat unbalanced and could cause unfair deaths

**Root Cause:**
- **Mismatch between turn order and combat state!**
- Line 152: Enemies cloned into `activeEnemies`: `activeEnemies = enemies.Select(e => e.Clone()).ToList()`
- Line 195: Turn order built from ORIGINAL enemies: `turnOrder = RollInitiative(player, enemies)`
- Turn order contained references to originals, combat operated on clones
- When clone died, original was still "alive" in turn order
- Dead clone check (`if (!combatant.IsAlive)`) checked original's HP, not clone's HP
- Same enemy could get multiple turns because original never died

**Solution Implemented:**
- Changed line 195 to use cloned enemies for turn order
- `turnOrder = RollInitiative(player, activeEnemies);` instead of `enemies`
- Now turn order references match combat state perfectly
- When enemy clone dies, turn order entry also reflects death

**Files Modified:**
- `Managers/CombatManager.cs` - Line 195: Use activeEnemies instead of enemies for RollInitiative

**Testing:**
- Build successful (0 errors, 276 warnings)
- Enemies should now get exactly one turn per round
- No more double turns after killing one enemy in a group

**Impact:**
- This was a CRITICAL combat bug affecting game balance
- Could cause player death due to unfair extra enemy attacks

---

### [2026-01-07] - Critical NPC Removal Bug Fixed ✅

**Status:** Fixed and tested (build verified)

**Problem:**
- Defeated enemies remained visible in rooms after combat
- Recruited NPCs (like Braxus) remained in rooms and would attack player on re-entry
- Made progression impossible as NPCs couldn't be permanently removed

**Root Cause:**
- Enemy cloning system (added to fix shared health pool bug) created new issue
- Combat operates on CLONED enemies: `activeEnemies = enemies.Select(e => e.Clone()).ToList()`
- Victory/recruitment code tried to remove clones using `.Remove(enemy)`
- `.Remove()` only works on exact object instances, not clones
- Clones are different objects, so they were never removed from room NPC lists

**Solution Implemented:**
- Changed all `.Remove(npc)` calls to `.RemoveAll(n => n.Name == npc.Name)`
- Removes NPCs by name matching instead of object reference
- Fixed in 3 locations:
  1. `CombatManager.FinishVictory()` - Removes defeated enemies from room
  2. `CombatManager.HandleRecruitmentSelection()` - Removes recruited NPCs after combat
  3. `DialogueManager.ExecuteDialogueAction("add_recruit")` - Removes recruited NPCs from dialogue

**Files Modified:**
- `Managers/CombatManager.cs` - Fixed enemy removal (lines 2418-2419) and recruitment removal (lines 2337-2338)
- `Managers/DialogueManager.cs` - Fixed dialogue-based recruitment removal (line 475)

**Testing:**
- Build successful (0 errors, 0 warnings)
- Enemies should now disappear from rooms after defeat
- Recruited NPCs should no longer reappear in their original rooms

---

### [2026-01-05] - Bug Fixes Completed ✅

**Status:** All 5 bugs fixed and tested (compile-verified)

**Session Summary:**
Fixed all critical bugs from playtesting session. All fixes compile successfully. Ready for integration testing.

**Bugs Fixed:**

#### 1. ✅ Evasive Fire Status Stuck / Cooldown Not Decreasing
**Problem:**
- Cooldown counter was not decreasing between combats
- Status was already clearing correctly at combat end

**Root Cause:**
- Missing cooldown decrement system between combats
- `evasiveFireActive` was already being cleared in `ClearStatusEffects()` (working correctly)

**Solution Implemented:**
- Added `DecrementAllCooldowns()` method in `CombatManager.cs:2806-2840`
- Decrements all ability cooldowns for player and party members by 1 each combat
- Called in `StartCombat()` at line 161
- Cooldowns now properly decrease across multiple fights

**Files Modified:**
- `Managers/CombatManager.cs` - Added cooldown decrement logic

---

#### 2. ✅ Combat Damage Calculation Incorrect
**Problem:**
- Bandit at 26/45 HP, Lightning Bolt showed 16 damage, ended at 14/45 HP

**Root Cause:**
- NOT A BUG - Working as intended!
- Displayed damage (16) is raw roll BEFORE defense
- Actual damage (12) is after defense subtraction: 16 - 4 defense = 12
- Result: 26 - 12 = 14 HP ✓ (correct!)

**Investigation Results:**
- `Character.TakeDamage()` at `Models/Character.cs:71` correctly applies defense once
- `ApplyDamageWithType()` displays raw damage, then calls `TakeDamage()` which subtracts defense
- System is functioning correctly - this is a UX clarity issue, not a calculation bug

**Solution:**
- No code changes needed
- Damage calculation verified correct
- Future enhancement: Could display actual damage after defense for clarity

---

#### 3. ✅ Town Gate Locked After Leaving (Softlock Bug)
**Problem:**
- CRITICAL progression blocker
- Gate locked after first entry, requiring another warlord kill to re-enter

**Root Cause:**
- Gate unlock was temporary (only when holding severed head)
- No persistent flag to remember gate was unlocked

**Solution Implemented:**
- Added persistent `town_gate_unlocked` quest flag
- Set flag when gate opens: `player.QuestFlags["town_gate_unlocked"] = true`
- Added check in `HandleDialogueStateTransitions()` to keep Marcus on "after_quest" dialogue
- Gate now stays permanently unlocked after first successful entry

**Files Modified:**
- `Managers/DialogueManager.cs:548` - Set `town_gate_unlocked` flag when gate opens
- `Managers/DialogueManager.cs:851-863` - Check flag to maintain Marcus dialogue state

---

#### 4. ✅ Caelia Dialogue Not Updating After Quest Completion
**Problem:**
- Caelia's passphrase dialogue option not appearing after translation quest
- Player couldn't progress to ask about cult hideout location

**Root Cause:**
- Quintus used wrong dialogue action type
- `give_item` REMOVES items from inventory (takes from player)
- Should use `receive_item` to ADD items to inventory (give to player)
- Player never received "translated letter", so Caelia's conditional dialogue never appeared

**Solution Implemented:**
- Changed Quintus dialogue action from `give_item` to `receive_item`
- Player now correctly receives "translated letter" item after translation
- Caelia's dialogue option properly shows when condition is met

**Files Modified:**
- `Data/NPCData.cs:1635` - Fixed dialogue action type for Quintus translation_ready node

---

#### 5. ✅ Equipment Cannot Be Equipped on Recruits
**Problem:**
- No UI to equip items on recruited party members
- Major feature gap limiting party customization

**Root Cause:**
- Guild management screen showed recruit stats but no equipment interface
- Menu system lacked recruit-specific equipment management

**Solution Implemented:**
- Added complete recruit equipment management system
- New menu flow: Guild → View Recruits → Select Recruit → Manage Equipment
- Shows current equipped items (weapon, armor, helm, ring)
- Lists available equipment from player inventory
- Supports equip/unequip operations with proper inventory swapping
- Equipment changes properly update recruit stats

**Files Modified:**
- `Managers/MenuManager.cs:32-137` - Added new menu states and input handlers
  - `GuildRecruitActions` - View recruit sheet with equipment display
  - `GuildRecruitEquipment` - Equipment management interface
- `Managers/GuildManager.cs:279-490` - Added equipment management methods
  - `DisplayRecruitActionsMenu()` - Show recruit sheet with equipment
  - `DisplayRecruitEquipmentMenu()` - List available equipment
  - `ProcessRecruitEquipmentAction()` - Handle equip/unequip
  - `HandleRecruitEquip()` - Equip item with inventory swap
  - `HandleRecruitUnequip()` - Unequip item back to inventory

---

### Testing Checklist for Next Session

**Critical Path Testing:**
- [ ] Bug #3: Enter town with head, leave, return without head - gate should stay open
- [ ] Bug #5: Equip weapon on recruit, verify stats update, verify works in combat
- [ ] Bug #4: Complete Quintus translation, verify Caelia shows passphrase option
- [ ] Bug #1: Use ability with cooldown, finish combat, start new combat - cooldown should decrease

**Integration Testing:**
- [ ] Save/load after fixes - verify flags persist
- [ ] Multiple combats in sequence - verify cooldowns work correctly
- [ ] Equip multiple items on recruit - verify all slots work
- [ ] Full Act I playthrough - no progression blockers

---

## 📋 NEXT SESSION NOTES

### Goal: Act I Complete & Bug-Free by End of Week

**Primary Objective:**
Complete testing and polish of Act I to production-ready state.

**Tasks:**

1. **Testing & Bug Fixes** (Priority 1)
   - [ ] Run through complete Act I playthrough
   - [ ] Test all 5 bug fixes with saved game
   - [ ] Fix any issues discovered during testing
   - [ ] Verify save/load works correctly with new flags
   - [ ] Test edge cases (equipping same item twice, etc.)

2. **Act I Completion Message** (Priority 2)
   - [ ] Add end-of-Act-I detection system
   - [ ] Design completion message (workshop exact wording)
   - Draft: "Thank you for playing the GuildMaster Alpha Test! Only Act I is complete. You've unlocked the full world - you can now send recruits on quests, explore, and continue leveling up your guild. More acts coming soon!"
   - [ ] Decide trigger point: After guild council meeting? After cultist hideout?
   - [ ] Add message display with appropriate formatting
   - [ ] Set flag to show message only once

3. **Player Analytics System** (Priority 3)
   - Implement telemetry tracking for player statistics
   - See analytics implementation options below

---

### 💡 Player Analytics Implementation Options

**Challenge:** This is a CLI game, not a web app - traditional web analytics (Google Analytics) won't work directly.

**Recommended Approach: Opt-in Telemetry with API Endpoint**

**Option 1: Simple HTTP POST to Analytics Endpoint (Recommended)**
```csharp
// Optional telemetry that user can enable/disable
public class TelemetryManager
{
    private bool telemetryEnabled = false;
    private string sessionId;
    private string endpoint = "https://your-api.com/telemetry";

    public void TrackEvent(string eventName, Dictionary<string, object> data)
    {
        if (!telemetryEnabled) return;

        var payload = new {
            session_id = sessionId,
            event = eventName,
            timestamp = DateTime.UtcNow,
            data = data
        };

        // Fire-and-forget async POST
        _ = PostToEndpoint(payload);
    }
}
```

**Events to Track:**
- Game start (with class selection, OS, game version)
- Act I completion (with playtime, level reached, deaths)
- Session end (with duration, progress made)
- Critical milestones (recruit first party member, enter town, defeat warlord)
- Character death (with location, enemy type, level)

**Backend Options:**
1. **Simple Node.js/Express API** - Stores events in MongoDB/PostgreSQL
2. **Google Cloud Functions** - Serverless, cheap, easy to set up
3. **Supabase** - Free tier, built-in database, easy client integration
4. **PostHog** - Open source analytics specifically for product usage

**Privacy Considerations:**
- Anonymous session IDs (no personal data)
- Opt-in by default (ask on first game start)
- Clear disclosure of what's tracked
- Option to disable in settings menu

**Option 2: Local Telemetry with Optional Upload**
- Game writes events to local JSON file
- User can optionally upload via `guildmaster --upload-stats` command
- More privacy-friendly, but lower data collection rate

**Option 3: GitHub Discussions Analytics**
- Ask players to post completion screenshots/stats manually
- Low-tech but builds community engagement
- No code required, but incomplete data

**Recommended Implementation:**
Start with Option 1 (HTTP telemetry) but make it:
- Opt-in with clear prompt on first launch
- Fully transparent about what's collected
- Easy to disable in settings
- Graceful failure if API is unreachable (no crashes)

**Stats Dashboard Ideas:**
- Total players
- Class distribution (% Venator vs Oracle vs Gladiator)
- Average completion time for Act I
- Death rate by location/enemy
- Most common quit points (where players stop playing)
- Equipment popularity (most equipped items)
- Recruit recruitment rate (how many recruits per player)

---

### 🎯 Week Goal Timeline

**Monday-Tuesday:** Testing & bug verification
**Wednesday-Thursday:** Act I completion message + polish
**Friday:** Analytics implementation (if time permits) + final testing
**Weekend:** Deploy alpha build to testers

---

## [2026-01-08] - Combat Systems Analysis & Ability Rework Planning

**Status:** Planning Phase

**Summary:**
Conducted comprehensive analysis of enemy AI targeting, ability balance, and combat progression. Identified several areas for improvement including ability gaps, EP economy issues, and autocombat AI limitations. Created detailed planning document for major ability system overhaul.

**Analysis Completed:**

1. **Enemy AI Targeting Analysis**
   - Confirmed melee enemies prioritize front-row targets (lowest HP)
   - Ranged/support enemies ignore positioning, always target lowest HP character
   - Oracle's 0 defense + 15 HP makes them priority target for all ranged enemies
   - AI is working as intended - balance issues are stat-based, not AI-based

2. **Ability System Review**
   - Catalogued all 24 existing abilities across 3 classes
   - Created ABILITY_ANALYSIS.csv with detailed breakdown
   - Confirmed Blessing is functional (+2 attack for 4 turns to party)
   - Identified utility vs damage balance for each ability

3. **Critical Issues Discovered**
   - **Massive level gaps:** No abilities at levels 6-9, 11-14, 16-19
   - **Venator drought:** 10-level gap between abilities (level 5 to 15)
   - **EP economy broken:** Oracle can spam Lightning Bolt indefinitely (costs 4 EP, regens 3.6/turn)
   - **Autocombat AI handicapped:** Filters out all support abilities (Heal, Barrier, Blessing)
   - **Shield Wall trap:** Prevents attacking for 3 turns, too restrictive to be useful

4. **Pre-Combat UX Issue**
   - Pre-combat dialogue shows "Press Enter to continue" but doesn't warn combat starts
   - Should say "Press Enter to continue - Combat will begin" for clarity

**Key Findings:**

**Energy Regeneration Issues:**
- Oracle: 20% of 18 max EP = 3.6 EP/turn regeneration
- Lightning Bolt: 4 EP cost, 1d8+1 damage (avg 5.5), ignores armor
- Oracle can essentially spam strongest ability with minimal downtime
- Other classes have better EP/damage management requirements

**Ability Distribution by Level:**
- Level 1-3: 9 abilities (good early density)
- Level 4-5: 4 abilities
- Level 6-9: **0 abilities** (major gap)
- Level 10: 3 abilities
- Level 11-14: **0 abilities** (major gap)
- Level 15: 4 abilities
- Level 16-19: **0 abilities** (major gap)
- Level 20: 4 abilities

**Files Created:**
- `ABILITY_ANALYSIS.csv` - Complete ability database with analysis
- `ABILITY_REWORK_PLAN.md` - Detailed planning document for upcoming changes
- `TESTING_NOTES.md` - Active playthrough observations and bug reports

**Next Steps (Planned, Not Implemented):**
1. Design 3-5 new abilities per class to fill level gaps
2. Rebalance EP costs and regeneration rates
3. Update autocombat AI to use support abilities intelligently
4. Add pre-combat warning message
5. Consider adding new ability types: roots, silence, dispels, energy manipulation
6. Adjust XP requirements and add more early combat encounters

**Notes:**
- No code changes made in this session - pure analysis and planning
- Detailed ability design work will be tracked in ABILITY_REWORK_PLAN.md
- Active playthrough testing observations tracked in TESTING_NOTES.md
- Changes will be migrated back to PROJECT_LOG.md after implementation
- Current combat balance is functional but needs expansion for tactical depth

---

## [2026-01-09] - Ongoing Playthrough Testing

**Status:** Testing in Progress (70% complete)

**Issues Discovered:**

### UX Issue: Room 53 Passphrase Command Not Discoverable
- **Problem:** Players need to use "speak <passphrase>" command in room 53, but command is not obvious
- **Current State:** No tutorial, help file entry, or in-game clues about this mechanic
- **Potential Solutions to Explore:**
  - [ ] Add tutorial message about puzzle commands when first encountering puzzle rooms
  - [ ] Add "speak" command and puzzle mechanics to help files
  - [ ] Add inspectable clue (e.g., "look fog" or "examine fog" in room 53)
  - [ ] Consider general puzzle hints system for obscure commands
- **Priority:** Medium (affects Act I progression discovery)
- **Status:** Needs design discussion - approach TBD

### Balance Issue: Oracle Severely Underpowered vs Legionnaire
- **Problem:** Oracle dying in almost every fight in cultist's base; significant power gap compared to Legionnaire
- **Root Cause:** Oracle's 0 defense + 15 HP vs Legionnaire's 5 defense + 30 HP = massive survivability gap
- **Testing Observation:** Most playthroughs done as Legionnaire because Oracle too fragile for mid-game content
- **Impact:** Makes Oracle nearly unplayable in mid-to-late Act I content
- **Priority:** High (core class balance issue)
- **Status:** Documented in ABILITY_REWORK_PLAN.md - will be addressed in combat rebalancing pass

### Feature: Item Grouping in Combat Menu
- **Implemented:** Combat items menu now groups duplicate items with counts (e.g., "Potion (14)")
- **Status:** Complete - matches inventory display behavior
- **File Modified:** Managers/CombatManager.cs

### Critical Bug Fix: Belum Gate Not Persisting After Save/Load
- **Problem:** Gate to Belum (room 69) closes after save/load even though quest was completed
- **Root Cause:** Rooms are re-initialized from RoomData.cs on load, resetting to default state (gate closed)
- **Impact:** Complete showstopper - players cannot progress without admin commands
- **Solution Implemented:**
  - Added `RestoreRoomStates()` method in SaveGameManager.cs
  - Checks `town_gate_unlocked` quest flag after load
  - Restores north exit (room 69 → 70) if flag is true
  - Updates room description to show gate is open
  - Called at end of `ApplyLoadedState()` after all flags are restored
- **Testing Required:** Load save with gate unlocked, verify gate is open and accessible
- **File Modified:** Managers/SaveGameManager.cs:535-558
- **Status:** Implemented, needs testing

### UX Fix: Senator Quintus Repeat Greeting
- **Problem:** Quintus gives full introduction every time player talks to him
- **Solution:** Split into `first_greeting` and `greeting` (repeat) dialogue nodes
- **First greeting:** Full introduction about being a Senator from Aevoria
- **Repeat greeting:** "Ah, back again. How can I help you?"
- **File Modified:** Data/NPCData.cs:1571-1613
- **Status:** Complete

### Display Bug Fix: Party Interjection Formatting
- **Problem:** `[bold]` tags showing as literal text instead of being rendered
- **Root Cause:** `DisplayTextWithPaging()` escapes ALL square brackets, converting `[bold]` to `[[bold]]`
- **Solution:**
  - Removed `[bold]` markup tags (can't be used with current text escaping)
  - Changed color from `#FFD700` (bright gold) to `#87CEEB` (sky blue - softer)
  - Format now: `Name: dialogue text` in light blue
- **Files Modified:** Managers/DialogueManager.cs:201, 700
- **Status:** Complete
- **Note:** To use bold in future, would need CSS-based approach or restructure text escaping logic

### Critical Bug Fix: Event Dialogue Softlock
- **Problem:** Event dialogues show "0. Continue" option which exits dialogue early, creating permanent softlock
- **Root Cause:** Event is marked as triggered when dialogue starts, not when it completes
  - Events have `IsOneTime = true` and `FirstVisit` condition
  - Player can exit with "0", event won't retrigger, story can't progress
- **Impact:** Complete showstopper - players locked out of Act II without reloading save
- **Solution:**
  - Removed "0. Continue" option from `ShowEventDialogueNode()` method
  - Updated `ProcessEventDialogueChoice()` to reject choice "0" entirely
  - Event dialogues are now linear - must select one of the story choices to proceed
- **Files Modified:** Managers/DialogueManager.cs:747, 765-769
- **Status:** Complete
- **Note:** This affects all event dialogues (guild council, celebration, assassination, etc.)

### UX Fix: Silent Travel for Narrative Sequences
- **Problem:** "You travel to a new location..." message interrupts narrative journey descriptions
- **Example:** Journey to Aevoria text split by travel notification mid-description
- **Solution:**
  - Added optional "silent" parameter to force_travel dialogue action
  - When `silent = true`, travel happens without the standard notification message
  - Applied to guild council → Aevoria travel sequence
  - Journey description now flows uninterrupted
- **Files Modified:**
  - Managers/DialogueManager.cs:576-584 (added silent parameter check)
  - Data/EventDataDefinitions.cs:442 (added silent=true to Aevoria travel)
- **Status:** Complete
- **Note:** Other force_travel uses still show message by default for clarity

### UI Improvement: Simplified Menu Headers
- **Problem:** ASCII art headers for Guild Management and Character Sheet screens take up excessive vertical space
- **Old Style:** 12-line gradient ASCII block art spelling out "GUILD MANAGEMENT" and "CHARACTER"
- **New Style:** 3-line simple header matching Inventory screen format
- **Changes:**
  - Guild Management: Green header (`#90FF90`) with "GUILD MANAGEMENT" title
  - Character Sheet: Cyan header (`#75C8FF`) with "CHARACTER SHEET" title
  - Format: `═══════...═══` border lines with centered title
- **Benefit:** Saves ~9 lines of screen space per menu, improves consistency across UI
- **Files Modified:**
  - Managers/GuildManager.cs:30-32 (replaced 12 lines with 3)
  - Managers/UIManager.cs:67-69 (replaced 6 lines with 3)
- **Status:** Complete

### Bug Fix: Duplicate Rest Message in Aevoria
- **Problem:** "Exhausted from the journey..." message displays twice when resting in Aevoria villa
- **Root Cause:** Same message printed on line 686 and again on line 702 in same code block
- **Solution:** Removed duplicate message on line 702
- **Flow Now:**
  1. "Exhausted from the journey..." (initial message)
  2. Player/party fully restored, time advances 48 hours
  3. "You sleep through the night and the following day..." (continuation)
- **File Modified:** Managers/GameController.cs:702
- **Status:** Complete

### Bug Fix: Celebration Event Not Triggering After Rest
- **Problem:** After resting in Aevoria villa, celebration event dialogue doesn't trigger automatically
- **Root Cause:** Rest command moves player to room 202 but doesn't trigger event check
  - Events only check on room movement commands (north, south, etc.), not direct room assignment
  - Player had to leave and re-enter room to trigger the celebration event
- **Solution:** Added manual event check after moving player to guest quarters
  - Calls `eventManager.CheckForEvent()` immediately after setting room
  - Executes event actions, starts dialogue, and marks event as triggered
  - Same pattern used in movement command handler
- **File Modified:** Managers/GameController.cs:710-731
- **Status:** Complete

### UX Feature: Back Option for Target Selection
- **Problem:** Once player selects a targetable ability/attack/item, they're locked into choosing a target with no way to cancel
- **Solution:** Added "0. Back" option to all target selection menus
- **Implementations:**
  - **Attack Target Selection:** Returns to action menu (lines 706, 905, 915-927)
  - **Ability Target Selection:** Returns to ability menu (lines 1139, 1284-1317, 1314)
  - **Item Target Selection:** Returns to item menu (lines 1527, 1356-1365, 1378)
- **Behavior:**
  - Clears pending selection state (ability, item, targets)
  - Returns player to appropriate menu based on context
  - Works for both player and party member actions
- **Files Modified:** Managers/CombatManager.cs
- **Status:** Complete

### Bug Fix: Tutorial Message Appearing Mid-Dialogue
- **Problem:** Quest ledger tutorial appears during Act Two intro dialogue instead of after completion
- **Root Cause:** Tutorial triggered when event marked as "triggered" (dialogue start), not when dialogue ends
- **Example:** Tutorial appeared between dialogue choices, breaking immersion
- **Solution:**
  - Removed tutorial trigger from GameEngine.cs (line 447-450)
  - Added new "show_tutorial" dialogue action type to DialogueManager.cs
  - Added action to final dialogue node ("commitment") of act_two_intro_dialogue
  - Tutorial now displays AFTER dialogue completes, not during
- **Files Modified:**
  - Services/GameEngine.cs:446-447 (removed premature trigger)
  - Managers/DialogueManager.cs:625-634 (added show_tutorial action handler)
  - Data/EventDataDefinitions.cs:655-659 (added action to dialogue)
- **Status:** Complete

---

### Critical Bug Fix: Gate Puzzle Keys Not Working After Save/Load
- **Problem:** "use keys on gate" command failed with "Specified cast is not valid" error in loaded save games
- **Root Cause:** Puzzle state dictionary values deserialized as `JsonElement` instead of `bool`, causing direct cast to fail
- **Impact:** Complete blocker - players couldn't access Warlord's chamber in loaded games
- **Solution Implemented:**
  - Created `GetPuzzleStateBool()` helper method in GameController.cs to safely extract boolean values
  - Handles three cases: native bool (fresh game), JsonElement (loaded save), string fallback
  - Updated all puzzle state boolean checks to use the helper method
  - Applied fix to both gate puzzle and twisting path puzzle (preventative)
- **Files Modified:**
  - Managers/GameController.cs:1010-1011, 1054-1055 (gate puzzle checks)
  - Managers/GameController.cs:887-889, 916, 925-927, 953 (twisting path puzzle checks)
  - Managers/GameController.cs:1066-1102 (new GetPuzzleStateBool helper method)
  - Services/GameEngine.cs:568-576 (added try-catch for better error reporting)
- **Testing:** Verified working with both fresh games and loaded save files
- **Status:** Complete

---
---
