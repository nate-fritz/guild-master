# Active Playthrough Testing Notes

**Session Date:** 2026-01-10
**Tester:** User
**Build:** Latest (dungeon test area + Oracle abilities)

---

## ✅ FIXED (2026-01-10) - Gate Puzzle Keys Not Working
- **Was:** "use keys on gate" command failed with cast error in loaded save games
- **Root Cause:** Puzzle state boolean values deserialized as JsonElement, not bool
- **Fixed:** Created GetPuzzleStateBool() helper to safely handle both cases
- **Status:** RESOLVED - Tested and working in both fresh games and loaded saves

## ✅ UPDATED (2026-01-10) - Caelia Dialogue Enhanced
- **Changed:** Rewrote Caelia's dialogue to emphasize decades-long partnership with Quintus
- **Additions:** Reveals shared history, long investigation of Ordo Dissolutus, former guild connection
- **Impact:** Better narrative cohesion, establishes player as key operative in their investigation
- **Status:** Complete - Ready for player feedback

---

## Respawn Behavior Testing

### ✅ Working Correctly
- **Farm Bandits:** Correctly stopped respawning after Bandit Warlord defeat
  - Expected behavior: Farm encounters should end after quest completion
  - Status: WORKING AS INTENDED

### ✅ FIXED (2026-01-09)
- **Enemy Respawning System**
  - **Was:** No enemies respawning anywhere in the game
  - **Root Cause:** Room respawn tracking (LastClearedDay, LastClearedHour) not saved/loaded
  - **Fixed:** Added RoomRespawnData to save system, respawn timing now persists
  - **Status:** RESOLVED - Enemies should now respawn after RespawnTimeHours (16-48 hours)
  - **Needs Testing:** Playthrough verification of actual respawn behavior

---

## Combat Balance Observations

*(Testing in progress - notes will be added as playthrough continues)*

---

## Bug Reports

### ✅ FIXED (2026-01-09) - All Issues Below

All bugs below were fixed in the 2026-01-09 update. Details in PROJECT_LOG.md.

### ✅ Marcus Reverting to Original Dialogue After Quest Complete
**Status:** RESOLVED
**Fix:** Moved Marcus safety check before early return in CheckAndUpdateTimers() (DialogueManager.cs:809-814)

### ✅ Malformed Color Tag on Enemy Respawn Message
**Status:** RESOLVED
**Fix:** Removed nested brackets from respawn message (GameController.cs:438)

### ✅ Caelia Quest Dialogue Not Appearing on First Conversation
**Status:** RESOLVED
**Fix:** Added passphrase option to first_greeting node (NPCData.cs:409-414)

### ✅ NPC Name Displaying Twice in Room Description
**Status:** RESOLVED
**Fix:** Removed redundant Name field, kept only ShortDescription (GameController.cs:65)

---

## Feature Requests / UX Issues

### ✅ FIXED (2026-01-09)

1. **Pre-Combat Warning**
   - **Was:** "Press Enter to continue" (ambiguous)
   - **Fixed:** Changed to "[Red]Press Enter to begin combat[/]" with red color
   - Status: RESOLVED (GameEngine.cs:1056)

2. **Remove Real-World References (Rome/Roman)**
   - **Was:** 4 instances of "Rome", "Roman", "Pax Romana" in dialogue/descriptions
   - **Fixed:** All replaced with fantasy-appropriate terms:
     - "romanesque" → "classical"
     - "Pax Romana" → "Imperial Peace"
     - "defend Rome" → "defend the Empire"
   - Status: RESOLVED (RoomData.cs:631,652; NPCData.cs:1787,1796)

---

## Performance Notes

*(None yet this session)*

---

## 🆕 NEW: Dungeon Test Area (2026-01-10)

**Access:** `tpto 900` from anywhere

### Structure Testing
- [ ] **Hub Room (900):** Verify description and `down` command to Floor 1
- [ ] **Floor Navigation:** Test `up` and `down` commands between floors
- [ ] **Room Connectivity:** Verify all exits work correctly
  - Linear floors: 901→905, 911→915
  - Branching floors: 906→910, 916→920

### Enemy Testing
- [ ] **Floor 1 (Levels 1-5):** Satyr, Harpy, Giant Scorpion, Skeleton Warrior
  - Verify enemy stats, abilities, and loot drops
  - Test XP rewards are appropriate for level range
- [ ] **Floor 2 (Levels 6-10):** Centaur Scout, Gorgon, Bronze Automaton, Fury
  - Test enemy abilities (Piercing Arrow, Entropy Bolt, Flame Bolt)
- [ ] **Floor 3 (Levels 11-15):** Minotaur, Medusa, Cyclops, Lamia
  - Test harder enemies with multiple abilities
- [ ] **Floor 4 (Levels 16-20):** Hydra, Chimera, Cerberus, Titan
  - Test end-game difficulty and multi-attack enemies

### Equipment Testing
- [ ] **Floor 1 Treasury (Room 905):** Bronze tier equipment
  - Verify `take all` picks up all 4 items
  - Test `look gladius`, `look bow`, `look staff`, `look breastplate`
  - Equip and verify stat changes
- [ ] **Floor 2 Armory (Room 910):** Enchanted tier
- [ ] **Floor 3 Vault (Room 915):** Legendary tier
- [ ] **Floor 4 Chamber (Room 920):** Divine tier

### Future Features (Not Yet Implemented)
- ⚠️ **Door Unlock System:** Currently `down` exits always available (should require defeating enemies)
- ⚠️ **Reset Mechanic:** Going `up` should reset floors (not implemented yet)

---

## 🆕 NEW: Oracle Abilities Testing (2026-01-10)

### Heal (Buffed - Level Scaling)
- [ ] **Level 1:** Should heal 4-10 HP (2d4+1)
- [ ] **Level 5:** Should heal 8-14 HP (2d4+5)
- [ ] **Level 10:** Should heal 13-19 HP (2d4+10)
- [ ] **Level 15:** Should heal 18-24 HP (2d4+15)
- [ ] **Level 20:** Should heal 23-29 HP (2d4+20)
- [ ] Verify scales properly throughout dungeon levels

### Befuddle (Level 2, 2 EP) - Confusion
- [ ] Cast on enemy in multi-enemy fight
- [ ] Verify confused enemy attacks random ally (not player)
- [ ] Cast on solo enemy
- [ ] Verify solo enemy skips turn when confused
- [ ] Verify duration is exactly 1 turn
- [ ] Check status display shows "Confused" icon

### Rejuvenation (Level 4, 2 EP) - HoT
- [ ] Cast on party member with damaged health
- [ ] Verify heals 3 HP on caster's turn
- [ ] Verify heals 3 HP on buffed ally's turn
- [ ] Verify total healing = 9 HP over 3 turns (3 HP × 3 turns)
- [ ] Check status display shows "Regenerating" icon with turn counter
- [ ] Verify effect disappears after 3 turns

### Ice Shards (Level 8, 2 EP) - Multi-target
- [ ] Cast with 5+ enemies present
- [ ] Verify hits exactly 3 random enemies
- [ ] Verify damage is 1d4+1 per target (2-5 HP)
- [ ] Check ice damage color coding (should be ice blue)
- [ ] Test with fewer than 3 enemies (should hit all available)
- [ ] Test with 1 enemy (should hit once)

### Protective Ward (Level 13, 4 EP) - Party Shield
- [ ] Cast with full party (player + 2 allies)
- [ ] Verify all 3 characters receive 8 HP shield
- [ ] Take damage and verify shield absorbs before health
- [ ] Verify shield value decreases correctly
- [ ] Test shield lasting 3 turns
- [ ] Verify shield disappears after 3 turns
- [ ] Test with partial damage (shield blocks some, not all)
- [ ] Compare to Barrier (single-target shield)

### Status Effects System
- [ ] Verify Confused enemies show proper icon/status
- [ ] Verify Regenerating shows proper icon/status with turn counter
- [ ] Verify Protective Ward shows shield value in status display
- [ ] Test multiple status effects on same character
- [ ] Verify status cleanup when duration expires

---

## Party Equipment Management

### Current Behavior (Guild Menu)
- ✅ Can equip items on party members through guild menu
- ✅ "Manage Equipment" option shows numbered inventory
- ✅ Can unequip items with `unequip weapon/armor/helm/ring`

### Requested Improvements
- [ ] **Add to Party Menu:** Equipment management should also be accessible from party menu
- [ ] **Direct Commands:** Consider `equip <item> on <member>` syntax for convenience
- [ ] Test equipment stat changes apply correctly to party members
- [ ] Verify equipment bonuses show in combat

---

## Next Testing Focus

- **Priority 1:** Test all Oracle abilities in dungeon combat
- **Priority 2:** Verify status effects work correctly (Confused, Regenerating, Protective Ward)
- **Priority 3:** Test equipment progression through dungeon floors
- Continue playthrough to verify respawn behavior in cave
- Verify all recent bug fixes (Jan 5-9) are working in integrated playthrough
