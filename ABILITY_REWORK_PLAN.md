# Ability System Rework - Planning Document

**Status:** Planning Phase
**Started:** 2026-01-08
**Target Implementation:** TBD

---

## Executive Summary

This document tracks the planned overhaul of the combat ability system. The goal is to:
1. Fill massive level gaps (6-9, 11-14, 16-19) with new abilities
2. Rebalance EP costs and regeneration rates
3. Improve tactical depth with new ability types (CC, dispels, energy manipulation)
4. Update autocombat AI to handle support abilities
5. Fix UX issues (pre-combat warnings)

---

## Current State Analysis

### Existing Abilities by Class

See `ABILITY_ANALYSIS.csv` for complete breakdown of all 24 current abilities.

**Distribution:**
- Legionnaire: 9 abilities (levels 1, 2, 3, 5, 10, 10, 15, 20, 20)
- Venator: 7 abilities (levels 1, 2, 3, 4, 5, 15, 20)
- Oracle: 8 abilities (levels 1, 2, 3, 5, 10, 15, 15, 20)

### Critical Issues

#### 1. Massive Level Gaps
**Problem:** Players go many levels without new tactical options.

- **Levels 6-9:** No abilities for ANY class (4-level gap)
- **Levels 11-14:** No abilities for ANY class (4-level gap)
- **Levels 16-19:** No abilities for ANY class (4-level gap)
- **Venator Specific:** 10-level gap between Barbed Arrow (5) and Frost Arrow (15)

**Impact:**
- Reduces tactical variety during mid-game
- Makes leveling feel unrewarding
- Limits player experimentation and strategy development

#### 2. EP Economy Imbalance
**Problem:** Oracle can spam Lightning Bolt indefinitely with no resource management.

**Oracle EP Math:**
```
Max EP: 18
EP Regen per turn: 20% of max = 3.6 EP/turn
Lightning Bolt cost: 4 EP
Lightning Bolt damage: 1d8+1 (avg 5.5, ignores armor)

Result: Cast every ~1.1 turns with minimal downtime
```

**Comparison:**
- Venator: 5% per turn + 5% per attack = slower regen, more management required
- Legionnaire: 0% per turn, 20% per attack = forced to use basic attacks to build resources

**Why This Is Broken:**
- Lightning Bolt is one of the strongest single-target abilities (ignores armor)
- No tactical decision-making required - just spam best ability
- Other classes must manage resources carefully, Oracle doesn't

#### 3. Autocombat AI Handicap
**Problem:** AI filters out all support abilities, making it significantly weaker than manual combat.

**Current AI Behavior (CombatManager.cs:1768-1771):**
```csharp
var affordableAbilities = ally.Abilities.Where(a =>
    ally.Energy >= a.EnergyCost &&
    !a.Name.ToLower().Contains("heal") &&
    !a.Name.ToLower().Contains("barrier") &&
    !a.Name.ToLower().Contains("blessing")).ToList();
```

**Filtered Out:**
- Heal (Oracle's core survival tool)
- Barrier (15 HP shield, free)
- Blessing (+2 attack party buff)
- Any future support abilities with these keywords

**Impact:**
- Oracle AI never heals injured allies (major handicap)
- Party never receives defensive buffs during autocombat
- Gap will widen as we add more support/CC abilities
- One tester already reports autocombat feels inferior

#### 4. Shield Wall Design Issue
**Problem:** Prevents all attacks for 3 turns, making it nearly useless.

**Current Design:**
- Cost: 0 EP (free)
- Effect: +Defense for party for 3 turns
- Drawback: Cannot attack at all during effect
- Cooldown: 10 turns

**Why It's Problematic:**
- 3 turns of no damage output is devastating to DPS
- In most fights, offense > defense (kill faster = less damage taken)
- Only useful in extreme niche scenarios (boss with huge AOE attack?)
- Feels like a trap option that punishes players for using it

#### 5. Pre-Combat UX Confusion
**Problem:** Players don't realize combat is starting immediately after dialogue.

**Current Message:**
```
"So, you've come to take all of this from me? Let's get this over with."

Press Enter to continue

⚔ COMBAT BEGINS ⚔
```

**Issue:** "Press Enter to continue" is ambiguous - continue to what? More dialogue? Movement?

**Proposed Fix:**
```
"So, you've come to take all of this from me? Let's get this over with."

Press Enter to begin combat

⚔ COMBAT BEGINS ⚔
```

---

## Proposed Solutions

### 1. New Abilities - Fill the Gaps

**Target Distribution:**
- Add 3-5 new abilities per class
- Focus on levels 6-9, 11-14, 16-19
- Introduce new ability types for tactical depth

**New Ability Types to Add:**
- **Root/Immobilize:** Prevent enemy from acting but can still be hit (weaker than stun)
- **Silence:** Prevent enemy ability usage (forces basic attacks only)
- **Dispel:** Remove debuffs from allies OR buffs from enemies
- **Energy Drain:** Reduce enemy EP, preventing ability usage
- **Energy Share:** Transfer EP to ally (support utility)
- **Revive:** Bring knocked out ally back to combat (resurrection)
- **Damage Reflect:** Return % of damage taken back to attacker
- **Counter Stance:** Automatic retaliation on next incoming attack
- **Mark Target:** Debuff enemy to take increased damage from all sources

**Design Philosophy by Class:**
- **Legionnaire:** Protection, crowd control, party buffs. Tank who enables team.
- **Venator:** Multi-target damage, resource efficiency, mobility. Consistent DPS.
- **Oracle:** Healing, debuffs, magical damage, support utility. Keeps party alive.

### 2. EP Economy Rebalance

**Goals:**
- Require tactical resource management from all classes
- Prevent infinite ability spam
- Make basic attacks feel valuable, not just filler

**Proposed Changes:**

**Oracle Adjustments:**
```
Current: 20% regen/turn (3.6 EP), 5% per attack (0.9 EP)
Proposed Option A: 15% regen/turn (2.7 EP), 5% per attack
Proposed Option B: 10% regen/turn (1.8 EP), 10% per attack
```

**Option A:** Reduces passive regen, forces more basic attacks to sustain Lightning Bolt spam
**Option B:** Shifts to hybrid regen model, rewards mixing attacks with spells

**Ability Cost Adjustments:**
- Lightning Bolt: 4 EP → 5 EP (requires 2 turns to recast)
- Blessing: 5 EP → 4 EP (makes it more accessible)
- Heal: Consider 3 EP → 2 EP (encourage more frequent healing)

**Alternative Approach - Cooldowns:**
Instead of pure EP cost, add cooldowns to powerful spells:
- Lightning Bolt: 4 EP + 2 turn cooldown
- Divine Wrath: 6 EP + 3 turn cooldown
- Forces ability rotation instead of spam

### 3. Autocombat AI Overhaul

**Current Problem:** Hardcoded exclusion list is brittle and incomplete.

**Proposed Solution:** Context-aware ability selection.

**New AI Logic:**
```
For each ally's turn:
1. Check HP status:
   - If ally below 40% HP → Prioritize Heal/Barrier on self
   - If other ally below 30% HP → Prioritize Heal on them

2. Check party buffs:
   - If no attack buff active && can afford Blessing → Cast Blessing
   - If taking heavy damage && can afford Shield Wall → Consider defensive buff

3. Check enemy threats:
   - If enemy has dangerous ability ready → Prioritize Silence/Stun
   - If multiple enemies alive → Prioritize AOE abilities

4. Default to damage:
   - Select highest damage/EP ratio ability available
   - Target lowest HP enemy
```

**Implementation Approach:**
- Remove hardcoded filters
- Add ability classification system (Damage, Healing, Buff, Debuff, CC)
- Create priority queues based on combat state
- Allow AI to use full toolkit intelligently

### 4. Shield Wall Redesign

**Option A - Reduce Duration:**
- Duration: 3 turns → 1 turn
- Makes it a tactical "oh shit" button instead of long commitment

**Option B - Allow Attacks:**
- Remove "cannot attack" restriction
- Reduce defense bonus to compensate
- Becomes pure defensive buff like Blessing but for defense

**Option C - Emergency Block:**
- New design: "Next incoming attack deals 0 damage and generates EP"
- Single-use defensive counter
- Rewards timing and prediction

**Recommendation:** Option A or C. Option B makes it too similar to existing buffs.

### 5. Pre-Combat Message Update

**Implementation:**
Search for all pre-combat dialogue triggers and update the input prompt.

**Files to Check:**
- `Data/NPCData.cs` - NPC combat dialogue
- `Data/EventDataDefinitions.cs` - Event-triggered combat
- `Managers/DialogueManager.cs` - Dialogue display system

**Change:**
```csharp
// Before
TextHelper.WaitForInput();

// After
TextHelper.WaitForInput("Press Enter to begin combat");
```

Or update at display level if centralized.

---

## New Ability Design (Draft Ideas)

### Legionnaire New Abilities

**Level 6: Pommel Strike**
- Cost: 2 EP
- Damage: 1d4+1 (low)
- Effect: Silence target for 2 turns (cannot use abilities)
- Type: Single Target, Melee
- Rationale: Cheap CC, tactical counter to enemy casters

**Level 8: Rallying Shout**
- Cost: 3 EP
- Effect: Remove one debuff from each party member + 10% max HP heal
- Type: Party Buff
- Rationale: Cleanse utility, gives tank support option

**Level 12: Iron Will**
- Cost: 0 EP
- Effect: Next attack that would reduce you to 0 HP instead leaves you at 1 HP
- Cooldown: 15 turns
- Type: Self Buff
- Rationale: Emergency survival, rewards good timing

**Level 14: Challenge**
- Cost: 3 EP
- Effect: Taunt single enemy for 3 turns, reduce their damage by 30%
- Type: Single Target Debuff
- Rationale: Single-target tank tool, different from AOE Battle Cry

**Level 18: Vengeful Strike**
- Cost: 4 EP
- Damage: 1d10 + (% of missing HP as bonus)
- Type: Single Target, Melee
- Rationale: Comeback mechanic, rewards aggressive tanking

### Venator New Abilities

**Level 6: Crippling Shot**
- Cost: 3 EP
- Damage: 1d4+2
- Effect: Root target for 2 turns (cannot move/act but can be hit)
- Type: Single Target, Ranged
- Rationale: CC without full stun, tactical positioning

**Level 8: Volley**
- Cost: 2 EP
- Damage: 1d3+0 to 2 random enemies
- Type: Multi-Target, Ranged
- Rationale: Cheap multi-target, resource efficient

**Level 11: Hunter's Mark**
- Cost: 2 EP
- Effect: Marked target takes +30% damage from all sources for 4 turns
- Type: Single Target Debuff, Ranged
- Rationale: Support DPS, helps entire party

**Level 13: Explosive Arrow**
- Cost: 5 EP
- Damage: 1d6+3 to target, 1d4+0 to adjacent enemies
- Type: AOE, Ranged
- Rationale: Mid-tier AOE, bridges gap to Thunder Volley

**Level 17: Phase Shift**
- Cost: 0 EP
- Effect: Become untargetable for 1 turn, restore 4 EP
- Cooldown: 10 turns
- Type: Self Buff
- Rationale: Defensive utility + resource generation

### Oracle New Abilities

**Level 4: Rejuvenation**
- Cost: 2 EP
- Effect: Target ally regenerates 3 HP per turn for 3 turns (9 total)
- Type: Heal, Ranged
- Rationale: Cheaper healing option, HoT instead of burst

**Level 6: Dispel Magic**
- Cost: 3 EP
- Effect: Remove all debuffs from ally OR all buffs from enemy
- Type: Utility, Ranged
- Rationale: Tactical counter, dual-purpose support

**Level 8: Arcane Missiles**
- Cost: 2 EP
- Damage: 1d4+1 to 3 random enemies
- Type: Multi-Target, Ranged
- Rationale: Cheap AOE damage, fills Venator-style gap

**Level 11: Mind Spike**
- Cost: 3 EP
- Damage: 1d4+1
- Effect: Drain 4 EP from target
- Type: Single Target, Ranged
- Rationale: Energy denial, prevents enemy abilities

**Level 13: Protective Ward**
- Cost: 4 EP
- Effect: All party members gain 8 HP shield for 3 turns
- Type: Party Buff
- Rationale: Proactive healing alternative

**Level 17: Resurrection**
- Cost: 8 EP
- Effect: Revive knocked out ally with 50% HP
- Cooldown: Combat once per combat
- Type: Heal, Ranged
- Rationale: Ultimate support, prevents party wipes

---

## Implementation Checklist

### Phase 1: Core Ability System
- [ ] Design 3-5 new abilities per class (15 total minimum)
- [ ] Add ability definitions to `Data/AbilityData.cs`
- [ ] Update `GetAbilityByName()` switch statement
- [ ] Update class ability lists in `Models/CharacterClass.cs`
- [ ] Implement new ability types (Root, Silence, Dispel, etc.)

### Phase 2: EP Economy
- [ ] Adjust Oracle EP regeneration rate
- [ ] Rebalance ability costs (Lightning Bolt, Blessing, Heal)
- [ ] Test resource management across all classes
- [ ] Consider adding cooldowns to prevent ability spam

### Phase 3: Combat Mechanics
- [ ] Implement Root status effect (immobilize but vulnerable)
- [ ] Implement Silence status effect (disable abilities)
- [ ] Implement Mark/Vulnerability debuff (increase damage taken)
- [ ] Implement Energy Drain mechanics
- [ ] Implement Revive/Resurrection system
- [ ] Add damage reflection system

### Phase 4: Autocombat AI
- [ ] Remove hardcoded ability filters
- [ ] Add ability classification system (tag abilities by purpose)
- [ ] Implement context-aware priority system
- [ ] Add healing logic (use Heal when ally below X%)
- [ ] Add buff logic (use Blessing/Shield Wall appropriately)
- [ ] Add CC logic (use Stun/Silence on dangerous enemies)
- [ ] Test autocombat vs manual combat balance

### Phase 5: UX Improvements
- [ ] Update pre-combat message to say "Press Enter to begin combat"
- [ ] Find all pre-combat dialogue instances
- [ ] Update TextHelper or centralized input system
- [ ] Test all pre-combat encounters

### Phase 6: Shield Wall Redesign
- [ ] Choose redesign approach (reduce duration vs emergency block)
- [ ] Implement changes
- [ ] Update ability description
- [ ] Test in combat scenarios

### Phase 7: Testing & Balance
- [ ] Playthrough with each class to test new abilities
- [ ] Verify level gaps are filled appropriately
- [ ] Confirm EP management feels tactical
- [ ] Test autocombat performance
- [ ] Gather feedback from testers

### Phase 8: Documentation
- [ ] Update `ABILITY_ANALYSIS.csv` with new abilities
- [ ] Document all changes in PROJECT_LOG.md
- [ ] Update CONTENT_CREATION_GUIDE.md if needed
- [ ] Add notes for future ability designers

---

## Open Questions

1. **XP Curve Adjustment:** Should we also adjust XP required per level to make early progression faster?
2. **Combat Encounter Density:** User mentioned adding more early combat encounters. How many and where?
3. **Ability Flavor:** Should we add more lore/flavor text to abilities? Currently very mechanical.
4. **Ultimate Abilities:** Should level 20 abilities feel more "ultimate"? Current ones are good but not game-changing.
5. **Multi-Class Abilities:** Any plans for recruits to learn abilities from other classes? (e.g., Braxus learns a heal?)
6. **Ability Customization:** Future: Let players choose between 2 abilities at each level? (Like talent trees)
7. **Consumable Scrolls:** Should more abilities be available as scrolls for non-class users?

---

## Notes from User

- "Oracle early game needs help but don't just buff HP/defense - fix it with abilities"
- "More combat encounters early on" - specific locations/enemies TBD
- "Fine tune XP required to level" - make early game progression smoother
- "Autocombat tester says it's inferior" - confirms AI needs work
- "Open the faucet on abilities" - users want more options earlier

---

## Success Metrics

How do we know the rework succeeded?

1. **No level gaps over 2 levels** - Every 1-2 levels should grant new ability
2. **EP management matters** - All classes should need to balance abilities and basic attacks
3. **Autocombat competitive** - AI should win ~80% of fights manual player would win
4. **Tactical depth** - At any given turn, player should have 3+ meaningful choices
5. **Class identity** - Each class should feel distinct in playstyle and role
6. **Oracle survival** - Oracle should survive early game without major stat buffs

---

## Version History

- **2026-01-08:** Initial planning document created
- **Next Update:** After ability design approval and before implementation
