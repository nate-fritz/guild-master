# Ability System Rework - Planning Document (REVISED)

**Status:** Planning Phase - Reorganized for Class Identity
**Started:** 2026-01-08
**Revised:** 2026-01-10
**Target Implementation:** Batched (2-3 abilities at a time, one class at a time)

---

## Class Design Philosophy

### Legionnaire - The Unstoppable Vanguard
**Resource Model:** Build/Spend (attack → attack → ability pattern)
**Damage Profile:** High multi-target, lower single-target (until level 15+)
**Survivability:** Tank with taunts and party protection
**Weapon Strategy:** Strongest basic attacks via best weapons in game
**Key Tool:** Shield Bash for single-target CC when needed

### Venator - The Burst Marksman
**Resource Model:** Burst/Reload (spend EP → recharge → repeat)
**Damage Profile:** High single-target, moderate multi-target
**Survivability:** Mobility and positioning
**Combat Pattern:** Opens strong, needs to manage "ammunition"
**Key Tools:** Covering Shot + one more EP recharge ability

### Oracle - The Arcane Battery
**Resource Model:** Sustained caster (abilities → potions → passive regen)
**Basic Attacks:** Rarely used (only in very long fights when desperate)
**Survivability:** Extremely fragile (0 def, 15 HP) - needs 1-2 defensive "oh shit" buttons
**Dependency:** Relies on Legionnaire to peel threats
**Key Tool:** Heal and support buffs

---

## Pre-Combat UX Message
**Status:** ✅ ALREADY FIXED - Skip this entirely

---

## Shield Wall Redesign
**New Design (Hybrid A+C):**
- Duration: 1 turn (not 3)
- Effect: Next incoming attack deals 0 damage
- Cost: 0 EP
- Cooldown: 10 turns
- Rationale: Reactive defensive tool, not a long commitment

---

## EP Economy Rebalance

### Oracle Adjustments (Option A)
```
Current: 20% regen/turn (3.6 EP), 5% per attack (0.9 EP)
New: 15% regen/turn (2.7 EP), 5% per attack (unchanged)

Lightning Bolt: 4 EP → 5 EP (requires ~2 turns to recast)
Blessing: 5 EP → 4 EP (more accessible)
```

**Rationale:** Oracle rarely basic attacks, so we reduce passive regen to require more tactical EP management without forcing them to mix in attacks like other classes.

---

## New Abilities - Organized by Class

## 🛡️ LEGIONNAIRE - The Unstoppable Vanguard

**Current Abilities:**
- Level 1: Shield Bash (3 EP, 1d4+2, Stun 1 turn) ✅ Single-target CC
- Level 2: Cleave (3 EP, 1d4+1, hits 3 enemies) ✅ Multi-target
- Level 3: Battle Cry (2 EP, Taunt all + 50% EP regen, 5 turn CD) ✅ AOE taunt
- Level 5: Rending Strike (3 EP, 1d6+2, Bleed) ✅ Single-target with DOT
- Level 10: Sunder Armor (4 EP, 1d8+2, Crush) ✅ Single-target armor break
- Level 10: Shield Wall (0 EP, +Defense 3 turns, can't attack, 10 turn CD) ⚠️ NEEDS REDESIGN
- Level 15: Devastating Slam (5 EP, 1d6+3, AOE stun) ✅ Multi-target CC
- Level 20: Whirlwind (24 EP, 2d10, hits all ignoring rows) ✅ Ultimate AOE
- Level 20: War Cry (0 EP, Taunt all + 75% EP + 20% party damage, 5 turn CD) ✅ Ultimate buff

**Gaps to Fill:** 4, 6-9, 11-14, 16-19

### PROPOSED NEW ABILITIES

#### ✅ Level 6: Provoke (Single-Target Taunt)
**PRIORITY: HIGH - Requested by user for early Oracle protection**
- Cost: 0 EP
- Effect: Taunt single enemy for 2 turns
- Cooldown: 2 turns
- Type: Single Target Debuff, Melee
- **Rationale:** Simple early peel tool. No bonuses like Battle Cry. Gives Legionnaire ability to protect Oracle when Battle Cry is on cooldown or when you only need to taunt one threat.
- **Fits Philosophy:** ✅ Tank/protection role

#### ⚠️ Level 8: Rallying Shout (Party Support)
- Cost: 3 EP
- Effect: Remove one debuff from each party member + restore 10% max HP to each
- Type: Party Buff
- **Rationale:** Cleanse utility + minor healing. Gives tank a support option.
- **Fits Philosophy:** ✅ Party protection, but competes with "high multi-target damage" identity
- **QUESTION:** Does this feel too supporty for the "whirlwind of shield and gladius" fantasy? Or is party support part of tank identity?

#### ❌ Level 12: Iron Will (Survival Tool)
- Cost: 0 EP
- Effect: Next attack that would reduce you to 0 HP leaves you at 1 HP instead
- Cooldown: 15 turns
- Type: Self Buff
- **Rationale:** Emergency survival, rewards timing
- **Fits Philosophy:** ⚠️ Defensive but doesn't add to multi-target damage fantasy
- **RECOMMENDATION:** Consider replacing with multi-target ability instead?

#### ❌ Level 14: Challenge (moved to Level 6 as Provoke)
~~Original: 3 EP taunt + 30% damage reduction~~
**Status:** Replaced by Provoke (level 6, simplified version)

#### 🔄 Level 16: Crushing Sweep (Multi-Target Damage)
- Cost: 4 EP
- Damage: 1d6+2 to up to 4 enemies in melee range
- Type: AOE, Melee
- **Rationale:** Mid-tier AOE between Cleave (level 2) and Devastating Slam (level 15). Fills gap and reinforces multi-target identity.
- **Fits Philosophy:** ✅ High multi-target damage

#### ❌ Level 18: Vengeful Strike (Single-Target Comeback)
- Cost: 4 EP
- Damage: 1d10 + (bonus based on % missing HP)
- Type: Single Target, Melee
- **Rationale:** Comeback mechanic when low HP
- **Fits Philosophy:** ❌ Single-target damage doesn't fit "high multi-target" philosophy
- **RECOMMENDATION:** Replace with another AOE or party support ability?

### LEGIONNAIRE SUMMARY
**Fits Philosophy:** Provoke (6), Crushing Sweep (16)
**Questionable Fit:** Rallying Shout (8), Iron Will (12)
**Doesn't Fit:** Vengeful Strike (18)
**Still Needed:** Abilities for levels 4, 7, 9, 11, 13, 14, 17, 19

---

## 🏹 VENATOR - The Burst Marksman

**Current Abilities:**
- Level 1: Multi-Shot (4 EP, 1d3+1, hits all enemies) ✅ AOE opener
- Level 2: Piercing Arrow (3 EP, 1d6+2, ignores armor) ✅ High single-target
- Level 3: Covering Shot (0 EP, 1d3, restores 2 EP) ✅ EP recharge
- Level 4: Evasive Fire (0 EP, dodge + counter, 10 turn CD) ✅ Defensive utility
- Level 5: Barbed Arrow (3 EP, 1d4+2, Bleed) ✅ Single-target + DOT
- Level 15: Frost Arrow (4 EP, 1d6+3, Weaken attacks) ✅ Single-target + debuff
- Level 20: Thunder Volley (6 EP, 1d8+2, AOE stun) ✅ Ultimate AOE

**Gaps to Fill:** 6-14, 16-19

### PROPOSED NEW ABILITIES

#### ✅ Level 6: Crippling Shot (Single-Target CC)
- Cost: 3 EP
- Damage: 1d4+2
- Effect: Root target for 2 turns (cannot move/act but can be hit)
- Type: Single Target, Ranged
- **Rationale:** CC without full stun, tactical positioning
- **Fits Philosophy:** ✅ Single-target damage + utility

#### ✅ Level 8: Volley (Cheap Multi-Target)
- Cost: 2 EP
- Damage: 1d3+0 to 2 random enemies
- Type: Multi-Target, Ranged
- **Rationale:** Cheap multi-target option for reload phase
- **Fits Philosophy:** ✅ Moderate multi-target, resource efficient

#### ✅ Level 11: Hunter's Mark (Support Debuff)
- Cost: 2 EP
- Effect: Marked target takes +30% damage from all sources for 4 turns
- Type: Single Target Debuff, Ranged
- **Rationale:** Helps entire party, tactical choice vs direct damage
- **Fits Philosophy:** ✅ Enhances single-target focus of entire team

#### ✅ Level 13: Explosive Arrow (Mid-Tier AOE)
- Cost: 5 EP
- Damage: 1d6+3 to target, 1d4+0 to adjacent enemies
- Type: AOE, Ranged
- **Rationale:** Bridges gap between Multi-Shot and Thunder Volley
- **Fits Philosophy:** ✅ Moderate multi-target damage

#### ✅ Level 17: Phase Shift (EP Recharge + Defense)
- Cost: 0 EP
- Effect: Become untargetable for 1 turn, restore 4 EP
- Cooldown: 10 turns
- Type: Self Buff
- **Rationale:** Second reload ability (alongside Covering Shot). Defensive + resource generation.
- **Fits Philosophy:** ✅ PERFECT - This is the second recharge ability you wanted!

### VENATOR SUMMARY
**Fits Philosophy:** ✅ All 5 proposed abilities fit perfectly
**Still Needed:** Abilities for levels 7, 9, 10, 12, 14, 16, 18, 19

---

## 🔮 ORACLE - The Arcane Battery

**Current Abilities:**
- Level 1: Heal (3 EP, 2d4+2 HP restore) ✅ Core healing
- Level 2: Lightning Bolt (4 EP → 5 EP, 1d8+1, ignores armor) ✅ Best damage option
- Level 3: Blessing (5 EP → 4 EP, +2 attack party for 4 turns) ✅ Party buff
- Level 5: Flame Strike (4 EP, 1d6+2, Fire DOT) ✅ Damage + burn
- Level 10: Barrier (0 EP, 15 HP shield + heal 10% blocked, 10 turn CD) ✅ DEFENSIVE "OH SHIT" BUTTON #1
- Level 15: Frostbolt (3 EP, 1d6+3, Weaken attacks) ✅ Damage + debuff
- Level 15: Divine Wrath (6 EP, 2d6+4, massive single-target) ✅ Burst finisher
- Level 20: Venom (3 EP, 0 damage, strong Poison DOT) ✅ Pure DOT

**Gaps to Fill:** 4, 6-9, 11-14, 16-19

### PROPOSED NEW ABILITIES

#### ✅ Level 4: Rejuvenation (Efficient Healing)
- Cost: 2 EP
- Effect: Target ally regenerates 3 HP per turn for 3 turns (9 total healing)
- Type: Heal Over Time, Ranged
- **Rationale:** Cheaper healing option. HoT instead of burst.
- **Fits Philosophy:** ✅ More healing options for sustained support

#### ✅ Level 6: Dispel Magic (Utility Counter)
- Cost: 3 EP
- Effect: Remove all debuffs from ally OR all buffs from enemy
- Type: Utility, Ranged
- **Rationale:** Tactical counter, dual-purpose support
- **Fits Philosophy:** ✅ Utility/support role

#### ✅ Level 8: Arcane Missiles (Multi-Target Damage)
- Cost: 2 EP
- Damage: 1d4+1 to 3 random enemies
- Type: Multi-Target, Ranged
- **Rationale:** Cheap AOE damage option
- **Fits Philosophy:** ✅ Cheap ability to avoid basic attacks

#### ✅ Level 11: Mind Spike (Energy Denial)
- Cost: 3 EP
- Damage: 1d4+1
- Effect: Drain 4 EP from target
- Type: Single Target, Ranged
- **Rationale:** Energy denial prevents enemy abilities
- **Fits Philosophy:** ✅ Tactical utility, keeps Oracle casting

#### ⚠️ Level 13: Protective Ward (Party Shield)
- Cost: 4 EP
- Effect: All party members gain 8 HP shield for 3 turns
- Type: Party Buff
- **Rationale:** Proactive healing alternative
- **Fits Philosophy:** ✅ DEFENSIVE "OH SHIT" BUTTON #2 - Protects party when getting overwhelmed
- **NOTE:** With Barrier (10) and this (13), Oracle has 2 defensive tools

#### ✅ Level 17: Resurrection (Ultimate Support)
- Cost: 8 EP
- Effect: Revive knocked out ally with 50% HP
- Cooldown: Once per combat
- Type: Heal, Ranged
- **Rationale:** Ultimate support, prevents party wipes
- **Fits Philosophy:** ✅ Never wants to basic attack - this keeps them casting even when desperate

### ORACLE SUMMARY
**Fits Philosophy:** ✅ All 6 proposed abilities fit
**Defensive "Oh Shit" Buttons:** Barrier (10), Protective Ward (13)
**Still Needed:** Abilities for levels 7, 9, 12, 14, 16, 18, 19

---

## Summary of Changes Needed

### What Fits Current Design Philosophy
**Legionnaire:** 2/6 proposals fit (Provoke, Crushing Sweep)
**Venator:** 5/5 proposals fit perfectly ✅
**Oracle:** 6/6 proposals fit perfectly ✅

### What Needs Replacement/Rethinking
**Legionnaire:**
- ❌ Rallying Shout (8) - Too supporty? Or is cleanse/heal part of tank identity?
- ❌ Iron Will (12) - Defensive but doesn't add to "high multi-target damage"
- ❌ Vengeful Strike (18) - Single-target doesn't fit philosophy

**Recommendation:** Legionnaire needs more multi-target damage abilities to fill the "whirlwind of shield and gladius" fantasy. Consider replacing questionable abilities with more AOE options.

### Remaining Level Gaps (After Proposed Abilities)
**Legionnaire:** 4, 7, 9, 11, 13, 14, 17, 19 (8 more abilities needed)
**Venator:** 7, 9, 10, 12, 14, 16, 18, 19 (8 more abilities needed)
**Oracle:** 7, 9, 12, 14, 16, 18, 19 (7 more abilities needed)

---

## Implementation Approach

**Batching Strategy:**
- 2-3 abilities at a time
- One class at a time
- Test in multiple encounters after each batch
- Tweak before moving to next batch
- Complete one class before moving to the next

**Suggested Order:**
1. Start with Legionnaire (needs most philosophical rework)
2. Then Venator (proposals already fit well)
3. Then Oracle (proposals already fit well)

---

## Open Questions for User

1. **Legionnaire Multi-Target Focus:**
   - Should we replace Iron Will (12), Rallying Shout (8), and Vengeful Strike (18) with more AOE damage abilities?
   - Or is party support/survival part of the tank identity?

2. **Single-Target Taunt (Provoke):**
   - Level 6 placement good?
   - 0 EP cost, 2 turn cooldown, 2 turn duration - does this feel right?

3. **Defensive Tools:**
   - Oracle has Barrier (10) and Protective Ward (13) as "oh shit" buttons - is that enough?
   - Should Legionnaire have more survival tools, or just lean into multi-target damage?

4. **Ability Distribution:**
   - We still have lots of level gaps (4, 7, 9, 11-14, 16-19 for all classes)
   - Should we aim to fill every level, or is having gaps okay?
   - Not every level needs an ability - what's the target density?

---

## Notes

- Pre-Combat UX already fixed - skipping
- Shield Wall redesign: 1 turn, blocks next attack, 0 EP, 10 turn CD
- Oracle EP regen: 20% → 15% passive, 5% per attack unchanged
- Lightning Bolt: 4 EP → 5 EP
- Blessing: 5 EP → 4 EP
