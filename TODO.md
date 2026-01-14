# GuildMaster - TODO List

**Last Updated:** 2026-01-10

## Immediate Priority (Next Session)

### UX Improvements
- [ ] **Add equipment management to party menu**
  - Currently only accessible through guild menu
  - Add "Manage Equipment" option when viewing party member details in party menu
  - Should match guild menu functionality (equip by number, unequip by slot)
  - Files to modify: Managers/MenuManager.cs, Managers/UIManager.cs

- [ ] **Direct equipment commands for party members**
  - Syntax: `equip <item> on <party member>`
  - Example: `equip bronze gladius on valeria`
  - Alternative: `give <item> to <party member>`
  - Should work from anywhere (not just in menus)
  - Files to modify: Managers/ItemManager.cs, Services/GameEngine.cs

### Dungeon Test Area Completion
- [ ] **Implement door unlock system**
  - Defeat all enemies in current floor to unlock `down` exit
  - Add check before allowing floor progression
  - Display message: "The way down is now clear" or similar
  - Files to modify: Managers/CombatManager.cs (end of combat), Data/RoomData.cs (conditional exits)

- [ ] **Implement dungeon reset mechanic**
  - Going `up` from any floor returns to hub (room 900)
  - Resets all floors: respawns enemies, restores items
  - Add confirmation prompt: "Leave the dungeon? Progress will be lost. (yes/no)"
  - Files to modify: Services/GameEngine.cs (up command handler), Data/RoomData.cs

### Oracle Abilities Testing
- [ ] **Test Befuddle (Confused status)**
  - Verify enemies attack allies when confused
  - Verify solo enemies skip turn when confused
  - Test duration (should be 1 turn)
  - Check display shows confused icon/status

- [ ] **Test Rejuvenation (Regenerating status)**
  - Verify heals 3 HP per turn for 3 turns
  - Test on both player and party members
  - Verify triggers on both player turns and ally turns
  - Check cleanup after 3 turns

- [ ] **Test Protective Ward**
  - Verify shields all party members (8 HP each)
  - Test shield absorption before health damage
  - Verify 3-turn duration
  - Test with multiple party members taking damage

- [ ] **Test Ice Shards**
  - Verify hits exactly 3 random enemies
  - Check ice damage color coding
  - Test with fewer than 3 enemies
  - Verify damage calculation (1d4+1)

- [ ] **Test Heal scaling**
  - Verify healing increases with Oracle level
  - Test at levels 1, 5, 10, 15, 20
  - Compare to old flat 2d4+2 value

## Ability Rework (Ongoing)

### Venator Abilities (Next Class)
- [ ] Review current Venator abilities in ABILITY_REWORK_PLAN.md
- [ ] Implement Venator philosophy: Burst marksman with reload pattern
- [ ] Add new abilities (TBD based on plan)
- [ ] Test in dungeon at appropriate levels
- [ ] Balance EP costs and damage values

### Legionnaire Abilities
- [ ] Review current Legionnaire abilities in ABILITY_REWORK_PLAN.md
- [ ] Implement Legionnaire philosophy: Tank with high multi-target damage
- [ ] Add/revise abilities for build-and-spend pattern
- [ ] Implement early single-target taunt (0 EP, 2 turn CD)
- [ ] Revise Shield Wall (1 turn duration, blocks next attack)
- [ ] Test tanking effectiveness in dungeon

### Balance Pass
- [ ] Playtest all three classes through dungeon floors
- [ ] Adjust EP costs based on actual combat flow
- [ ] Tune damage/healing values for proper power curve
- [ ] Verify class identity is clear and distinct
- [ ] Document final values in ABILITY_REWORK_PLAN.md

## Known Issues (Lower Priority)

### UX Issue: Room 53 Passphrase Command
- Status: Documented in PROJECT_LOG.md (2026-01-09)
- Priority: Medium
- Needs design discussion on approach:
  - Add tutorial message for puzzle commands?
  - Add to help files?
  - Add inspectable clue in room?
  - General puzzle hints system?

### Balance Issue: Oracle Survivability (Pre-Rework)
- Status: Being addressed by ability rework
- Priority: High (but work is in progress)
- Oracle's 0 defense + 15 HP makes mid-game content very difficult
- Protective Ward and better healing should help
- May need stat adjustments after ability testing

## Future Enhancements (Long-term)

### Dungeon System Evolution
- [ ] Randomized dungeon generation for Act II/III
- [ ] Procedural room layouts
- [ ] Random enemy spawns based on floor level
- [ ] Random loot tables
- [ ] Boss rooms and special encounters

### Equipment System
- [ ] Equipment comparison tooltips
- [ ] Visual indication of better/worse equipment
- [ ] Quick-equip best item for class type
- [ ] Equipment sets with bonuses

### Combat Enhancements
- [ ] Combat log/history
- [ ] Damage numbers display improvements
- [ ] Status effect tooltips with detailed info
- [ ] AOE ability targeting indicators

### Quality of Life
- [ ] Auto-sort inventory by type
- [ ] Item filtering/search
- [ ] Bulk item actions (sell all junk, etc.)
- [ ] Party formation/positioning system
- [ ] Quick-save/quick-load hotkeys

## Completed Today (2026-01-10)
- ✅ Dungeon test area (rooms 900-920)
- ✅ 16 mythological enemies added
- ✅ Equipment progression (4 tiers)
- ✅ Oracle Heal scaling implementation
- ✅ Oracle Befuddle ability
- ✅ Oracle Rejuvenation ability
- ✅ Oracle Ice Shards ability
- ✅ Oracle Protective Ward ability
- ✅ Confused status effect
- ✅ Regenerating status effect
- ✅ Protective Ward shield system
- ✅ Bug fix: Game crash on Oracle selection
- ✅ Bug fix: Item visibility in dungeon rooms
