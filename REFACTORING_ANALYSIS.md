# GuildMaster Refactoring Analysis

**Date:** 2025-12-24
**Session:** Combat Manager Split & Code Cleanup

---

## TASK 1: CombatManager Analysis

### Current State
- **File Size:** 4,208 lines
- **Status:** EXTREMELY LARGE - needs splitting
- **Complexity:** High - handles combat flow AND ability execution

### Methods Identified for Extraction to AbilityExecutor

#### Ability Execution Methods (~40 methods)
**Legionnaire Abilities:**
- `ExecuteShieldBashGeneric()` - Shield bash attack with stun
- `ExecuteTauntGeneric()` - Taunt enemies to attack character
- `ExecuteShieldWallGeneric()` - Defensive stance
- `ExecuteCleaveGeneric()` - AOE melee attack
- `ExecutePowerAttack()` - Legacy power attack

**Venator Abilities:**
- `ExecuteMultiShotGeneric()` - Multi-target ranged attack
- `ExecutePiercingArrowGeneric()` - Armor-piercing shot
- `ExecuteEvasiveFireGeneric()` - Evasion buff
- `ExecuteCoveringShotGeneric()` - Defensive cover fire
- `ExecuteBarbedArrowGeneric()` - DOT arrow

**Oracle Abilities:**
- `ExecuteHealGeneric()` - Heal allies
- `ExecuteLightningBoltGeneric()` - Lightning damage
- `ExecuteBlessingGeneric()` - Buff allies
- `ExecuteFlameStrikeGeneric()` - Fire damage
- `ExecuteFrostboltGeneric()` - Ice damage with slow

**Generic Abilities:**
- `ExecuteRendingStrikeGeneric()` - Bleed effect attack

**Legacy Abilities:**
- `ExecuteWhirlwind()` - Old whirlwind attack
- `ExecuteBattleCry()` - Old battle cry buff

####  Router Methods
- `ExecuteAbilityForCharacter()` - Main ability router for any character
- `ExecuteAbility()` - Player-specific ability router
- `ExecuteEnemyAbility()` - Enemy-specific ability router

#### Support Methods
- `CalculateAbilityDamage()` - Damage calculation formula
- `ApplyStatusEffect()` - Apply status effects (stun, taunt, DOT, etc.)
- `ApplyCombatSingleEffect()` - Apply item effects to single target
- `ApplyCombatPartyEffect()` - Apply item effects to party
- `NeedsEnemyTarget()` - Check if ability requires targeting

### Methods to KEEP in CombatManager

#### Combat Flow
- `StartCombat()` - Initialize combat
- `ProcessNextTurn()` - Turn order management
- `CompleteTurn()` - Advance to next turn
- `HandleCombatEnd()` - Victory/defeat handling
- `EndCombat()` - Cleanup

#### Turn Processing
- `ProcessPlayerTurn()` - Player action menu
- `ProcessPartyMemberTurn()` - AI party member actions
- `HandleEnemyTurn()` - Enemy AI
- `HandlePartyMemberTurn()` - Recruit turn logic
- `ExecuteAIAction()` - AI decision making

#### UI/Menu Methods
- `ShowPlayerActionMenu()` - Display player options
- `ShowPartyMemberActionMenu()` - Display party options
- `ShowAbilityMenu()` - List available abilities
- `ShowItemMenu()` - List usable items
- `ShowStatusBar()` - Display health/energy
- `DisplayCombatPositioning()` - Show battle formation

#### Input Handling
- `ProcessCombatInput()` - Route player input
- `HandleActionSelection()` - Process action choice
- `HandleAbilitySelection()` - Process ability choice
- `HandleItemSelection()` - Process item choice
- `HandleAttackTargetSelection()` - Choose attack target
- `HandleAbilityTargetSelection()` - Choose ability target

#### Attack Methods (Keep - these are direct attacks, not abilities)
- `ExecutePlayerAttack()` - Basic player melee/ranged
- `ExecutePartyMemberAttack()` - Basic recruit attack
- `StartPlayerAttack()` - Initiate attack sequence
- `StartPartyMemberAttack()` - Initiate recruit attack

#### Utility
- `RollDice()` - Random number generation
- `SelectBestTarget()` - AI targeting
- `DetermineTurnOrder()` - Speed-based initiative
- `CheckVictoryConditions()` - Win/lose check

### Estimated Impact
- **Lines to Move:** ~1,500 lines (ability execution)
- **Lines to Keep:** ~2,700 lines (combat flow)
- **Risk Level:** MEDIUM-HIGH (complex dependencies)
- **Testing Required:** EXTENSIVE

### Recommended Approach

**Option A: Full Split (Recommended)**
1. Create `AbilityExecutor.cs` with ~1,500 lines
2. Move all ability execution methods
3. CombatManager keeps references to AbilityExecutor
4. Update all ability calls to use executor

**Option B: Incremental Split (Safer)**
1. Create AbilityExecutor with core methods first
2. Test thoroughly
3. Move additional methods in phases
4. Gradual migration

**Decision:** Proceed with **Option A** but test thoroughly at each step.

---

## TASK 2: Console Code Analysis ✅ COMPLETED

### Files Checked
- ✅ GameController.cs
- ✅ UIManager.cs - **FIXED**
- ✅ MenuManager.cs
- ✅ CombatManager.cs - **FIXED**
- ✅ DialogueManager.cs
- ✅ SaveGameManager.cs
- ✅ ItemManager.cs

### Console Usage Found and Fixed

**UIManager.cs - 6 issues fixed:**
- Line 23: Removed `Console.Clear()` - doesn't work in browser
- Line 50: Removed `Console.ForegroundColor = ConsoleColor.White;`
- Line 63: Removed `Console.ResetColor();`
- Line 77: Removed `Console.ResetColor();`
- Line 83: Removed `Console.ForegroundColor = ConsoleColor.Cyan;`
- Line 93: Removed `Console.ResetColor();`

**CombatManager.cs - 10 issues fixed:**
- Lines 1229, 1231: Removed ForegroundColor/ResetColor around ability selection
- Lines 1317, 1319: Removed ForegroundColor/ResetColor around target selection
- Lines 1400, 1402: Removed ForegroundColor/ResetColor around item selection
- Lines 1995, 2007: Removed ForegroundColor/ResetColor around death menu (also added proper [red] markup to death message)
- Lines 3190, 3192: Removed ForegroundColor/ResetColor around heal target selection

### Summary
All legacy console color commands removed. These were ineffective in Blazor WebAssembly environment since the app uses Spectre.Console's AnsiConsole.MarkupLine with color markup tags. The custom Console wrapper (GuildMaster.Services.Console) for ReadLine/Write is properly implemented and compatible with Blazor.

---

## TASK 3: Data Loading Patterns ✅ COMPLETED

### Files Reviewed
- ✅ RoomData.cs
- ✅ NPCData.cs
- ✅ ItemData.cs
- ✅ EquipmentData.cs
- ✅ QuestData.cs
- ✅ AbilityData.cs
- ✅ EffectData.cs
- ❌ DialogueData.cs (doesn't exist - dialogue is in NPCData)

### Pattern Analysis

**Three Different Patterns Found:**

1. **Static Constructor Pattern** (EquipmentData.cs)
   - Static class with static Dictionary field
   - Static constructor initializes data once on first access
   - Private helper methods for organization
   - **Pros:** Data loaded once, memory efficient
   - **Cons:** Can't be refreshed without app restart

2. **Factory Method Pattern** (Most common)
   - Static class with public `Initialize*()` or `Get*()` method
   - Returns new Dictionary/List on each call
   - Used by: RoomData, NPCData, ItemData, EffectData, QuestData
   - **Pros:** Fresh data each call, flexible
   - **Cons:** Memory overhead from recreating large datasets

3. **Static Fields Pattern** (AbilityData.cs)
   - Static class with public static fields for each instance
   - Each ability is a static singleton
   - **Pros:** Direct access, no method calls
   - **Cons:** Flat structure, harder to enumerate/search

### Inconsistencies Found

| File | Pattern | Method Name | Returns |
|------|---------|-------------|---------|
| EquipmentData.cs | Static Constructor | (none) | Static field |
| RoomData.cs | Factory Method | `InitializeRooms()` | New Dictionary |
| NPCData.cs | Factory Method | `InitializeNPCs()` | New Dictionary |
| ItemData.cs | Factory Method | `InitializeItemDescriptions()` | New Dictionary |
| EffectData.cs | Factory Method | `InitializeEffects()` | New Dictionary |
| QuestData.cs | Factory Method | `GetAvailableQuests()` | New List |
| AbilityData.cs | Static Fields | (none) | Direct access |

**Naming Inconsistencies:**
- Most use "Initialize" prefix (RoomData, NPCData, ItemData, EffectData)
- QuestData uses "Get" prefix
- Return type naming varies (Rooms, NPCs, ItemDescriptions, Effects, AvailableQuests)

### Recommendations

**For Future Refactoring:**
1. **Standardize on Factory Method pattern** for all data classes
2. **Consistent naming convention**: `Initialize<DataType>()` (e.g., `InitializeRooms()`, `InitializeNPCs()`)
3. **Consider caching**: Store returned dictionaries to avoid recreation
4. **Performance optimization**: For large datasets that don't change (Rooms, NPCs), consider static constructor pattern like EquipmentData

**Current Status:** No immediate action needed. Patterns work correctly but could be standardized for maintainability.

---

## TASK 4: SaveGameManager Audit ✅ COMPLETED

### Currently Saved ✅
- ✅ Player stats (HP, MaxHP, Energy, MaxEnergy, XP, Level, Attack, Defense, Speed)
- ✅ Player equipment (Weapon, Armor, Helm, Ring)
- ✅ Player class
- ✅ Current room
- ✅ Inventory items
- ✅ Taken items (HashSet)
- ✅ Examined items (HashSet)
- ✅ Guild recruits (with full stats, equipment, quest/rest status)
- ✅ Active party members
- ✅ Active quests (with full state)
- ✅ Completed quest IDs
- ✅ Quest flags (Dictionary<string, bool>)
- ✅ Triggered event IDs (HashSet)
- ✅ NPC dialogue states
- ✅ Removed NPCs (by room)
- ✅ Priority messages shown
- ✅ Player settings (AutoCombat, Tutorials, Gore)
- ✅ Time (CurrentDay, CurrentHour)

### Missing State ⚠️

**Critical - Should be added:**
1. **GameContext.TotalRecruitsEver** - Field exists in GameState.cs (line 61) but NOT saved or loaded
   - Used for milestone tracking
   - Impact: Milestones that depend on total recruit count won't work correctly after load

2. **GameContext.CompletedMilestones** - Field exists in GameState.cs (line 62) but NOT saved or loaded
   - HashSet tracking completed milestone IDs
   - Impact: Milestones will reset after save/load, causing duplicate triggers

3. **GameContext.RoomStateOverrides** - Field exists in GameState.cs (line 63) but NOT saved or loaded
   - Dictionary<int, string> for dynamic room state changes
   - Impact: Progressive guild hall system and other room state changes won't persist

**Minor - Consider adding:**
4. **Player.PreviousRoom** - Not in GameState at all
   - Used for navigation history
   - Impact: Low - only affects edge cases with room transitions

5. **Player.RoomNumbersEnabled** - Not in GameState at all
   - Debug setting for displaying room IDs
   - Impact: Very low - user preference, defaults to false

6. **Player.ThreeMemberCombatCount** - Not in GameState at all
   - Stat tracking for milestone system
   - Impact: Medium - milestone "win 10 fights with 3 members" won't track correctly

**Not Missing (by design):**
- GameContext.NoteText - Static content, recreated on load with player name interpolation ✅
- NPCs, Rooms, ItemDescriptions, Effects - Loaded from static data files ✅

---

## TASK 5: Code Quality Issues ✅ COMPLETED

### File Size Analysis

| File | Lines | Status |
|------|-------|--------|
| CombatManager.cs | 4,198 | ⚠️ CRITICAL - Needs splitting (see TASK 1) |
| SaveGameManager.cs | 1,099 | ⚠️ Large - Consider splitting load/save |
| UIManager.cs | 1,074 | ⚠️ Large - Consider splitting by UI section |
| DialogueManager.cs | 555 | ✅ Acceptable |
| ItemManager.cs | 527 | ✅ Acceptable |
| GameController.cs | 522 | ✅ Acceptable |

**Average method count in CombatManager:** 91 methods, 4,198 lines = ~46 lines/method average

### Debug Code Issues ⚠️

**CombatManager.cs contains 50+ DEBUG statements:**
```csharp
AnsiConsole.MarkupLine("[dim]DEBUG: message[/]");
```
- Lines affected: 95, 107, 129, 134, 148, 153, 168, 173, 177, 182, 195, 201, 209, 215, 217, 298, 434, 438, 471, etc.
- **Recommendation:** Remove debug statements OR wrap in `#if DEBUG` blocks
- **Impact:** Performance degradation, cluttered output in production

### Random Instance Issues ⚠️

**Multiple `new Random()` instances found:**
- QuestManager.cs:16 - `private Random random = new Random();`
- CombatManager.cs:63 - `private Random random = new Random();`
- CombatManager.cs:3823 - `Random rng = new Random();` (local instance!)
- ItemManager.cs:16 - `private Random random = new Random();`

**Problem:** Creating multiple Random instances in quick succession can produce identical sequences
**Recommendation:** Use a shared static Random instance OR use `Random.Shared` (C# 6+)

### Code Comments

**TODO/FIXME found:**
- EventData.cs:187 - `// TODO: Implement ability granting when ability system exists`

**Status:** Only 1 TODO found - good code hygiene ✅

### Using Statement Analysis

**Files with `using System;`:** 32 files
- Most legitimate (DateTime, Exception, etc.)
- Many files import `System` but only use a few types
- **Recommendation:** Consider more specific using statements for clarity

### Code Quality Summary

**Major Issues:**
1. ⚠️ CombatManager.cs - 4,198 lines (needs TASK 1 refactoring)
2. ⚠️ 50+ DEBUG statements in CombatManager.cs
3. ⚠️ Multiple Random instance creation pattern

**Minor Issues:**
4. ℹ️ SaveGameManager and UIManager approaching 1,000+ lines
5. ℹ️ One TODO comment in EventData.cs
6. ℹ️ Broad `using System;` imports

**Good Practices Found:**
- ✅ Consistent naming conventions
- ✅ Good use of regions for organization in large files
- ✅ Minimal TODO/FIXME debt
- ✅ Proper use of nullable reference annotations

---

## Summary & Prioritized Action Items

### Completed ✅
1. ✅ **TASK 1** - CombatManager Analysis (4,208 lines → needs split into CombatManager + AbilityExecutor)
2. ✅ **TASK 2** - Console Code Cleanup (16 issues fixed in UIManager.cs and CombatManager.cs)
3. ✅ **TASK 3** - Data Loading Pattern Analysis (3 patterns identified, recommendations documented)
4. ✅ **TASK 4** - SaveGameManager Audit (3 critical missing fields identified)
5. ✅ **TASK 5** - Code Quality Scan (50+ DEBUG statements, Random instances, file size issues)

### Priority 1 - Critical Fixes 🔴

**1.1 Fix SaveGameManager Missing State**
- Add save/load for `GameContext.TotalRecruitsEver`
- Add save/load for `GameContext.CompletedMilestones`
- Add save/load for `GameContext.RoomStateOverrides`
- **Impact:** HIGH - Milestone system and progressive guild hall broken without these
- **Effort:** LOW - 20 minutes
- **Files:** Managers/SaveGameManager.cs

**1.2 Split CombatManager (TASK 1)**
- Extract ~1,500 lines of ability execution to AbilityExecutor.cs
- Leave ~2,700 lines of combat flow in CombatManager.cs
- **Impact:** HIGH - Maintainability, readability, testing
- **Effort:** HIGH - 3-4 hours (see COMBAT_REFACTOR_SESSION_GUIDE.md)
- **Files:** Managers/CombatManager.cs → CombatManager.cs + AbilityExecutor.cs

### Priority 2 - Important Improvements 🟡

**2.1 Remove DEBUG Statements**
- Remove or guard 50+ DEBUG statements in CombatManager.cs
- **Impact:** MEDIUM - Performance, cleaner output
- **Effort:** LOW - 10 minutes (find/replace)
- **Files:** Managers/CombatManager.cs

**2.2 Fix Random Instance Pattern**
- Create shared static Random instance
- Replace all `new Random()` calls
- **Impact:** MEDIUM - RNG quality, thread safety
- **Effort:** LOW - 15 minutes
- **Files:** CombatManager.cs, QuestManager.cs, ItemManager.cs

**2.3 Add Missing Player State to SaveGameManager**
- Consider adding `Player.RoomNumbersEnabled`
- Consider adding `Player.ThreeMemberCombatCount`
- Consider adding `Player.PreviousRoom`
- **Impact:** LOW-MEDIUM - User preferences and milestone tracking
- **Effort:** LOW - 10 minutes
- **Files:** Models/GameState.cs, Managers/SaveGameManager.cs

### Priority 3 - Future Enhancements 🔵

**3.1 Standardize Data Loading Patterns**
- Consolidate on Factory Method pattern
- Consistent naming (`Initialize*()` prefix)
- Consider caching for performance
- **Impact:** LOW - Code consistency
- **Effort:** MEDIUM - 2 hours
- **Files:** All Data/*.cs files

**3.2 Split Large Managers**
- SaveGameManager (1,099 lines) → SaveGameManager + LoadGameManager
- UIManager (1,074 lines) → Split by UI section
- **Impact:** LOW - Maintainability for future development
- **Effort:** MEDIUM - 2 hours each
- **Files:** SaveGameManager.cs, UIManager.cs

### Estimated Total Time

- Priority 1: ~4 hours
- Priority 2: ~40 minutes
- Priority 3: ~6 hours
- **Grand Total:** ~11 hours

### Session Achievements

**Lines of Code Cleaned:** 16 console-specific code removals
**Build Status:** ✅ SUCCESS (no errors, warnings expected)
**Files Analyzed:** 40+ files across Data, Managers, Models, Services
**Documentation Created:**
- REFACTORING_ANALYSIS.md (this file)
- COMBAT_REFACTOR_SESSION_GUIDE.md
- CONTENT_CREATION_GUIDE.md

---

**Session Date:** 2025-12-24
**Status:** Analysis Complete, Ready for Implementation
