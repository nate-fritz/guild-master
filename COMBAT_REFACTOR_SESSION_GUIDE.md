# CombatManager Refactoring - Session Guide

**Purpose:** Complete guide to split CombatManager.cs into CombatManager + AbilityExecutor
**Estimated Time:** 2-3 hours
**Complexity:** HIGH
**Risk Level:** MEDIUM-HIGH

---

## PRE-SESSION CHECKLIST

### ✅ Before Starting
- [ ] Confirm current CombatManager.cs builds successfully
- [ ] Have a clean working tree (git status)
- [ ] Create a new branch: `git checkout -b refactor/combat-manager-split`
- [ ] Read this entire document first
- [ ] Have REFACTORING_ANALYSIS.md open for reference

### ⚠️ Critical Context
- **File Size:** CombatManager.cs is 4,208 lines
- **Lines to Extract:** ~1,500 lines (ability execution logic)
- **Lines to Keep:** ~2,700 lines (combat flow/UI)
- **Current State:** All combat works, event system recently added
- **Testing Required:** EXTENSIVE - combat is core gameplay

---

## PROJECT ARCHITECTURE OVERVIEW

### Combat System Flow
```
Player Input → GameController → CombatManager
                                      ↓
                          ┌───────────┴───────────┐
                          ↓                       ↓
                    Combat Flow              Ability Execution
                  (turn management)          (damage, effects)
                          ↓                       ↓
                    ProcessNextTurn()      ExecuteShieldBash()
                    ShowPlayerMenu()       CalculateAbilityDamage()
                    HandleEnemyTurn()      ApplyStatusEffect()
```

**After Refactoring:**
```
CombatManager (combat flow) ──calls──> AbilityExecutor (ability logic)
```

### Current Dependencies
```csharp
CombatManager depends on:
- GameContext (player, NPCs, rooms)
- MessageManager (optional - for priority messages)
- GameEngine (callback for state changes)
- AbilityData (ability definitions)
- EffectData (status effect definitions)
```

### Key Classes to Understand
```csharp
// Models/Character.cs
public class Character
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Energy { get; set; }
    public int AttackDamage { get; set; }
    public int Defense { get; set; }
    public Equipment EquippedWeapon { get; set; }
    public List<Ability> Abilities { get; set; }
    // ... more properties
}

// Models/Ability.cs
public class Ability
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int EnergyCost { get; set; }
    public int DiceCount { get; set; }
    public int DiceSides { get; set; }
    public AbilityTarget TargetType { get; set; }
    // ... more properties
}

// StatusEffect enum
public enum StatusEffect
{
    Stunned,
    Taunted,
    Bleeding,
    Poisoned,
    Burning,
    Slowed,
    CannotAttack,
    // ... more
}
```

---

## STEP-BY-STEP REFACTORING PLAN

### STEP 1: Read and Understand CombatManager Structure

**File Location:** `/home/sinogue/GuildMaster/Managers/CombatManager.cs`

**Key Sections to Review:**
```bash
# Line ranges (approximate):
Lines 1-100:    Imports, class declaration, fields
Lines 91-163:   StartCombat() - KEEP
Lines 164-311:  ProcessNextTurn() - KEEP
Lines 312-430:  Menu display methods - KEEP
Lines 432-692:  Input handlers - KEEP
Lines 1250-1350: ExecuteWhirlwind(), ExecuteBattleCry() - EXTRACT
Lines 1445-1515: ApplyCombatEffect methods - EXTRACT
Lines 2349-2410: CalculateAbilityDamage() - EXTRACT
Lines 2645-3800: ExecuteAbilityForCharacter() and all Execute*Generic() - EXTRACT
Lines 2815-2900: ApplyStatusEffect() - EXTRACT
```

**Fields in CombatManager to Note:**
```csharp
// Status effect tracking - needed by AbilityExecutor
private Dictionary<Character, Dictionary<StatusEffect, int>> statusEffects;
private Dictionary<Character, Character> taunters;
private Dictionary<Character, int> battleCryTurns;
private Dictionary<Character, int> buffedAttack;
private Dictionary<Character, int> buffedDefense;
private Dictionary<Character, int> warCryDamageBoost;
private Dictionary<Character, Dictionary<string, int>> abilityCooldowns;
private Dictionary<Character, bool> evasiveFireActive;
private Dictionary<Character, int> barrierAbsorption;
```

### STEP 2: Create AbilityExecutor.cs

**File Location:** `/home/sinogue/GuildMaster/Managers/AbilityExecutor.cs`

**Template Structure:**
```csharp
using GuildMaster.Services;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;
using GuildMaster.Data;

namespace GuildMaster.Managers
{
    /// <summary>
    /// Handles execution of all combat abilities and their effects
    /// Extracted from CombatManager to improve code organization
    /// </summary>
    public class AbilityExecutor
    {
        private readonly GameContext context;
        private readonly CombatManager combatManager;
        private readonly MessageManager messageManager;

        // Status effect tracking (shared with CombatManager)
        private Dictionary<Character, Dictionary<StatusEffect, int>> statusEffects;
        private Dictionary<Character, Character> taunters;
        private Dictionary<Character, int> battleCryTurns;
        private Dictionary<Character, int> buffedAttack;
        private Dictionary<Character, int> buffedDefense;
        private Dictionary<Character, int> warCryDamageBoost;
        private Dictionary<Character, Dictionary<string, int>> abilityCooldowns;
        private Dictionary<Character, bool> evasiveFireActive;
        private Dictionary<Character, int> barrierAbsorption;

        public AbilityExecutor(
            GameContext gameContext,
            CombatManager manager,
            MessageManager msgManager = null)
        {
            context = gameContext;
            combatManager = manager;
            messageManager = msgManager;

            // Initialize tracking dictionaries
            statusEffects = new Dictionary<Character, Dictionary<StatusEffect, int>>();
            taunters = new Dictionary<Character, Character>();
            battleCryTurns = new Dictionary<Character, int>();
            buffedAttack = new Dictionary<Character, int>();
            buffedDefense = new Dictionary<Character, int>();
            warCryDamageBoost = new Dictionary<Character, int>();
            abilityCooldowns = new Dictionary<Character, Dictionary<string, int>>();
            evasiveFireActive = new Dictionary<Character, bool>();
            barrierAbsorption = new Dictionary<Character, int>();
        }

        // PUBLIC METHODS - Called by CombatManager

        /// <summary>
        /// Main entry point for executing abilities
        /// </summary>
        public bool ExecuteAbilityForCharacter(Ability ability, Character character, List<NPC> enemies, Player player, NPC preselectedTarget = null)
        {
            // Copy from CombatManager.cs line ~2645
        }

        /// <summary>
        /// Calculate damage for an ability
        /// </summary>
        public int CalculateAbilityDamage(Character character, Ability ability)
        {
            // Copy from CombatManager.cs line ~2349
        }

        /// <summary>
        /// Apply a status effect to a character
        /// </summary>
        public void ApplyStatusEffect(Character target, StatusEffect effect, int duration, Character source = null)
        {
            // Copy from CombatManager.cs line ~2815
        }

        // PRIVATE METHODS - Ability implementations

        private bool ExecuteShieldBashGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            // Copy from CombatManager.cs
        }

        // ... all other Execute*Generic methods

        // HELPER METHODS

        /// <summary>
        /// Access CombatManager's RollDice method
        /// </summary>
        private int RollDice(int count, int sides, int bonus)
        {
            return combatManager.RollDice(count, sides, bonus);
        }
    }
}
```

### STEP 3: Methods to Extract (Complete List)

**Copy these methods FROM CombatManager TO AbilityExecutor:**

#### Core Execution Methods
```
✓ ExecuteAbilityForCharacter() - Line ~2645 (MAIN ROUTER)
✓ ExecuteAbility() - Line ~2596 (Player-specific router)
✓ ExecuteEnemyAbility() - Line ~1648 (Enemy-specific router)
```

#### Calculation Methods
```
✓ CalculateAbilityDamage() - Line ~2349
```

#### Effect Application
```
✓ ApplyStatusEffect() - Line ~2815
✓ ApplyCombatSingleEffect() - Line ~1445
✓ ApplyCombatPartyEffect() - Line ~1481
```

#### Legionnaire Abilities
```
✓ ExecuteShieldBashGeneric() - Line ~2968
✓ ExecuteTauntGeneric() - Line ~2992
✓ ExecuteShieldWallGeneric() - Line ~3023
✓ ExecuteCleaveGeneric() - Line ~3051
```

#### Venator Abilities
```
✓ ExecuteMultiShotGeneric() - Line ~3081
✓ ExecutePiercingArrowGeneric() - Line ~3108
✓ ExecuteEvasiveFireGeneric() - Line ~3130
✓ ExecuteCoveringShotGeneric() - Line ~3148
✓ ExecuteBarbedArrowGeneric() - Line ~3495
```

#### Oracle Abilities
```
✓ ExecuteHealGeneric() - Line ~3172
✓ ExecuteLightningBoltGeneric() - Line ~3206
✓ ExecuteBlessingGeneric() - Line ~3228
✓ ExecuteFlameStrikeGeneric() - Line ~3244
✓ ExecuteFrostboltGeneric() - Line ~3525
```

#### Generic Abilities
```
✓ ExecuteRendingStrikeGeneric() - Line ~3266
```

#### Legacy Abilities
```
✓ ExecuteWhirlwind() - Line ~1253
✓ ExecutePowerAttack() - Line ~1288
✓ ExecuteBattleCry() - Line ~1349
```

#### Helper Methods
```
✓ NeedsEnemyTarget() - Line ~1023
✓ HandleCharacterAbilities() - Line ~1202 (if used)
✓ HandleCombatItems() - Line ~1375 (effect application)
```

### STEP 4: Update CombatManager to Use AbilityExecutor

**Changes to CombatManager.cs:**

#### 1. Add Field
```csharp
public class CombatManager
{
    // ... existing fields ...
    private AbilityExecutor abilityExecutor;
```

#### 2. Update Constructor
```csharp
public CombatManager(GameContext gameContext, Action onCombatEnd, Action stateChanged)
{
    context = gameContext;
    onCombatEndCallback = onCombatEnd;
    onStateChangedCallback = stateChanged;

    // Initialize AbilityExecutor
    abilityExecutor = new AbilityExecutor(gameContext, this, ProgramStatics.messageManager);
}
```

#### 3. Make RollDice Public
```csharp
// Change from:
private int RollDice(int count, int sides, int bonus)

// To:
public int RollDice(int count, int sides, int bonus)
```

#### 4. Replace All Ability Calls
**Find and replace pattern:**
```csharp
// OLD:
ExecuteAbilityForCharacter(ability, character, enemies, player);

// NEW:
abilityExecutor.ExecuteAbilityForCharacter(ability, character, enemies, player);
```

**Search for all calls to these methods and update:**
- `ExecuteAbilityForCharacter(`
- `ExecuteAbility(`
- `ExecuteEnemyAbility(`
- `CalculateAbilityDamage(`
- `ApplyStatusEffect(`
- `ApplyCombatSingleEffect(`
- `ApplyCombatPartyEffect(`
- `ExecuteWhirlwind(`
- `ExecutePowerAttack(`
- `ExecuteBattleCry(`
- `HandleCharacterAbilities(`
- `HandleCombatItems(`

#### 5. Remove Extracted Methods
**Delete these methods from CombatManager** (they're now in AbilityExecutor):
- All Execute*Generic methods
- CalculateAbilityDamage
- ApplyStatusEffect
- ApplyCombatEffect methods
- Legacy ability methods

### STEP 5: Handle Shared State

**Problem:** Both classes need access to status effect dictionaries

**Solution:** AbilityExecutor maintains its own state but provides access methods

**In AbilityExecutor.cs:**
```csharp
// Public accessors for CombatManager to check status effects
public Dictionary<Character, Dictionary<StatusEffect, int>> GetStatusEffects()
{
    return statusEffects;
}

public bool HasStatusEffect(Character character, StatusEffect effect)
{
    return statusEffects.ContainsKey(character) &&
           statusEffects[character].ContainsKey(effect);
}

public void ClearStatusEffects()
{
    statusEffects.Clear();
    taunters.Clear();
    battleCryTurns.Clear();
    buffedAttack.Clear();
    buffedDefense.Clear();
    warCryDamageBoost.Clear();
    abilityCooldowns.Clear();
    evasiveFireActive.Clear();
    barrierAbsorption.Clear();
}
```

**In CombatManager.cs:**
```csharp
// Update any code that accesses statusEffects directly
// OLD:
if (statusEffects.ContainsKey(character) && statusEffects[character].ContainsKey(StatusEffect.Stunned))

// NEW:
if (abilityExecutor.HasStatusEffect(character, StatusEffect.Stunned))
```

### STEP 6: Build and Fix Compilation Errors

**Run:**
```bash
dotnet build 2>&1 | grep -i error
```

**Common Issues:**

1. **Missing method references**
   - Check if method is called but not extracted
   - Add public accessor in AbilityExecutor

2. **Field access issues**
   - CombatManager trying to access private AbilityExecutor fields
   - Add public property or method

3. **Circular dependencies**
   - Both classes reference each other (this is OK)
   - Make sure constructors don't create infinite loop

4. **Missing using statements**
   - Copy all using statements from CombatManager to AbilityExecutor

### STEP 7: Testing Protocol

**Manual Testing Checklist:**

#### Basic Combat
- [ ] Start new game
- [ ] Enter combat (attack an enemy)
- [ ] Combat UI displays correctly
- [ ] Can select actions (attack, ability, item, flee)

#### Player Abilities (Test each class)
**Legionnaire:**
- [ ] Shield Bash (stun works)
- [ ] Taunt (enemy targets you)
- [ ] Shield Wall (defense buff)
- [ ] Cleave (hits multiple enemies)

**Venator:**
- [ ] Multi-Shot (hits multiple enemies)
- [ ] Piercing Arrow (ignores armor)
- [ ] Evasive Fire (evasion buff)
- [ ] Covering Shot (protects ally)

**Oracle:**
- [ ] Heal (restores HP)
- [ ] Lightning Bolt (damage)
- [ ] Blessing (buff)
- [ ] Flame Strike (fire damage)

#### Status Effects
- [ ] Stun prevents action
- [ ] Taunt forces targeting
- [ ] Bleeding/Poison deals DOT
- [ ] Buffs increase stats

#### Enemy Abilities
- [ ] Enemies use abilities
- [ ] Enemy abilities deal damage
- [ ] Enemy buffs/debuffs work

#### Combat Flow
- [ ] Turn order correct (speed-based)
- [ ] Energy costs deducted
- [ ] Cooldowns work
- [ ] Victory/defeat handled
- [ ] XP/loot awarded

#### Party Combat
- [ ] Recruit 2+ party members
- [ ] Party members use abilities
- [ ] Party healing works
- [ ] Party buffs work

#### Edge Cases
- [ ] Out of energy (can't use ability)
- [ ] Dead party member (skips turn)
- [ ] Last enemy killed (combat ends)
- [ ] Flee from combat

### STEP 8: Performance Check

**After refactoring:**
```bash
# Check file sizes
wc -l /home/sinogue/GuildMaster/Managers/CombatManager.cs
wc -l /home/sinogue/GuildMaster/Managers/AbilityExecutor.cs

# Should be approximately:
# CombatManager: ~2,700 lines
# AbilityExecutor: ~1,500 lines
```

---

## CRITICAL DEPENDENCIES TO UNDERSTAND

### CombatManager Methods Called BY AbilityExecutor

**AbilityExecutor will need to call these CombatManager methods:**
```csharp
// Random number generation
combatManager.RollDice(count, sides, bonus)

// Combat state (if needed)
// Make these public if AbilityExecutor needs them:
// - activeEnemies
// - currentState
// - context.Player
```

### Methods That Reference Each Other

**Be careful with these - they call ability methods:**
```csharp
// In CombatManager - these CALL ability executor:
HandleAbilitySelection() → calls ExecuteAbilityForCharacter()
ProcessPlayerTurn() → calls ExecuteAbility()
HandleEnemyTurn() → calls ExecuteEnemyAbility()
ExecuteAIAction() → calls ExecuteAbilityForCharacter()
```

---

## COMMON PITFALLS TO AVOID

### ❌ Don't Do This:
1. **Don't move UI methods** to AbilityExecutor
   - Keep ShowAbilityMenu, ShowItemMenu in CombatManager

2. **Don't move input handling** to AbilityExecutor
   - Keep HandleAbilitySelection in CombatManager

3. **Don't break the callback chain**
   - onCombatEndCallback must still work
   - stateChangedCallback must still work

4. **Don't duplicate status effect dictionaries**
   - AbilityExecutor should own them
   - CombatManager accesses via methods

5. **Don't forget to update ALL ability calls**
   - Search thoroughly for every Execute* call

### ✅ Do This:
1. **Copy methods exactly** - don't optimize while refactoring
2. **Test after each major change** - don't batch all changes
3. **Keep commit history** - commit after each successful step
4. **Preserve comments** - copy all documentation
5. **Match code style** - use existing patterns

---

## SUCCESS CRITERIA

### ✅ Refactoring is Complete When:
- [ ] Build succeeds with 0 errors (warnings OK)
- [ ] CombatManager.cs is ~2,700 lines
- [ ] AbilityExecutor.cs is ~1,500 lines
- [ ] All combat tests pass (see Testing Protocol)
- [ ] No console errors in browser
- [ ] Combat feels identical to before
- [ ] Status effects still work
- [ ] Party combat still works
- [ ] Can complete full combat encounter
- [ ] XP/loot rewards correctly

### 📊 Quality Metrics
- **Code organization:** ✅ Logical separation of concerns
- **Maintainability:** ✅ Easy to add new abilities
- **Performance:** ✅ No slowdown vs before
- **Readability:** ✅ Clear method names
- **Documentation:** ✅ Comments preserved

---

## QUICK REFERENCE - METHOD LOCATIONS

### In CombatManager (line numbers approximate)
```
91-163:   StartCombat()
164-311:  ProcessNextTurn()
1253:     ExecuteWhirlwind() → EXTRACT
1288:     ExecutePowerAttack() → EXTRACT
1349:     ExecuteBattleCry() → EXTRACT
1445:     ApplyCombatSingleEffect() → EXTRACT
1481:     ApplyCombatPartyEffect() → EXTRACT
2349:     CalculateAbilityDamage() → EXTRACT
2596:     ExecuteAbility() → EXTRACT
2645:     ExecuteAbilityForCharacter() → EXTRACT (MAIN ROUTER)
2815:     ApplyStatusEffect() → EXTRACT
2968+:    All Execute*Generic() methods → EXTRACT
```

### Search Commands
```bash
# Find all ability execution methods
grep -n "Execute.*Generic\|ExecuteAbility\|ExecutePowerAttack\|ExecuteWhirlwind\|ExecuteBattleCry" CombatManager.cs

# Find all damage calculations
grep -n "CalculateAbilityDamage\|CalculateDamage" CombatManager.cs

# Find all status effect applications
grep -n "ApplyStatusEffect\|ApplyCombat.*Effect" CombatManager.cs

# Find all ability calls (to update)
grep -n "ExecuteAbilityForCharacter\|abilityExecutor\." CombatManager.cs
```

---

## ROLLBACK PLAN

If refactoring fails:
```bash
# Discard all changes
git reset --hard HEAD
git checkout main

# Or restore from backup
cp CombatManager.cs.backup Managers/CombatManager.cs
```

**Before starting, create backup:**
```bash
cp /home/sinogue/GuildMaster/Managers/CombatManager.cs /home/sinogue/GuildMaster/Managers/CombatManager.cs.backup
```

---

## FINAL NOTES

### Time Estimates
- **Step 1 (Analysis):** 15 minutes
- **Step 2 (Create AbilityExecutor):** 30 minutes
- **Step 3 (Extract methods):** 60 minutes
- **Step 4 (Update CombatManager):** 30 minutes
- **Step 5 (Handle shared state):** 20 minutes
- **Step 6 (Build/fix):** 20 minutes
- **Step 7 (Testing):** 45 minutes
- **Step 8 (Verification):** 10 minutes
- **TOTAL:** ~3.5 hours

### Key Success Factors
1. **Read this entire guide before starting**
2. **Work methodically, not quickly**
3. **Test frequently**
4. **Commit after each successful step**
5. **Don't rush the testing phase**

### When to Ask for Help
- Build errors persist after 15 minutes
- Combat behavior changes unexpectedly
- Status effects stop working
- Circular dependency errors
- Can't find a method that's being called

---

## ADDITIONAL CONTEXT

### Recent Changes to Be Aware Of
- **Event system** just added - don't break it
- **Progressive guild rooms** - don't affect
- **Equipment bonuses** - recently fixed
- **Text wrapping** - recently cleaned up
- **Quest flags** - recently added to save system

### Files That Import CombatManager
```bash
grep -r "using.*CombatManager\|CombatManager " --include="*.cs" .
```
These may need updates if public API changes.

### Related Documentation
- `REFACTORING_ANALYSIS.md` - Initial analysis
- `SESSION_PROGRESSIVE_GUILD_SUMMARY.md` - Recent features
- `TEST_ROOMS_GUIDE.md` - Testing environment

---

**Good luck! This is a big refactor but very achievable with careful, methodical work.**
