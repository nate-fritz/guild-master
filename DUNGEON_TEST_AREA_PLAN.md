# Randomized Dungeon Test Area - Implementation Plan

**Status:** In Development
**Started:** 2026-01-10
**Purpose:** Quick ability testing + prototype for Act II/III randomized dungeons

---

## Overview

**Location:** Rooms 900-950
**Structure:** 4 floors, 5 rooms per floor, entrance hub
**Theme:** Ancient ruins of pre-imperial civilization with Greek/Roman mythological creatures

**Reset Mechanic:** Going "up" from any floor resets entire dungeon (layouts, spawns, buffs/progress)

---

## Room Layout

### Room 900: Entrance Hub
- **Title:** "Ancient Ruins - Entrance"
- **Description:** Crumbling stone archway leading to underground ruins
- **Exits:** down (to Floor 1), up (to surface - exits dungeon)
- **Features:** Safe zone, no enemies
- **Future:** Add NPC for cooldown resets, healing fountain?

### Floor 1 (Rooms 901-905) - LINEAR LAYOUT
**Enemy Level Range:** 1-5 (Satyr, Harpy, Giant Scorpion, Skeleton Warrior)
**Loot:** Bronze Age weapons + Bronze Breastplate

```
901 (Entry) → 902 → 903 → 904 → 905 (Exit Down)
    ↑                                   ↓
  (from 900)                      (to Floor 2)
```

**Room Details:**
- 901: Entry chamber, 2 Satyrs
- 902: Corridor, 1 Harpy + 1 Satyr
- 903: Chamber, 2 Giant Scorpions
- 904: Passage, 3 Skeleton Warriors
- 905: Exit chamber, 1 Harpy + 2 Satyrs + loot chest (Bronze equipment)

### Floor 2 (Rooms 906-910) - BRANCHING LAYOUT
**Enemy Level Range:** 6-10 (Centaur Scout, Gorgon, Bronze Automaton, Fury)
**Loot:** Enchanted weapons + Blessed Cuirass

```
           908 (treasure)
            |
906 (Entry)─907─909─910 (Exit Down)
    ↑                 ↓
  (from 905)     (to Floor 3)
```

**Room Details:**
- 906: Entry hall, 1 Bronze Automaton
- 907: Central chamber (crossroads), 2 Centaur Scouts
- 908: Side treasury (north branch), 1 Gorgon + gold/items
- 909: East passage, 2 Furies
- 910: Exit chamber, 1 Bronze Automaton + 1 Gorgon + loot chest (Enchanted equipment)

### Floor 3 (Rooms 911-915) - LINEAR LAYOUT
**Enemy Level Range:** 11-15 (Minotaur, Medusa, Cyclops, Lamia)
**Loot:** Legendary weapons + Griffon Hide Armor

```
911 (Entry) → 912 → 913 → 914 → 915 (Exit Down)
    ↑                                   ↓
  (from 910)                      (to Floor 4)
```

**Room Details:**
- 911: Entry chamber, 1 Minotaur
- 912: Long corridor, 2 Lamias
- 913: Grand hall, 1 Cyclops + 2 Skeleton Warriors (higher level)
- 914: Antechamber, 1 Medusa + 1 Minotaur
- 915: Exit chamber, 1 Cyclops + 1 Medusa + loot chest (Legendary equipment)

### Floor 4 (Rooms 916-920) - BRANCHING LAYOUT
**Enemy Level Range:** 16-20 (Hydra, Chimera, Cerberus, Titan)
**Loot:** Divine weapons + Aegis of the Gods

```
           918 (treasure)
            |
916 (Entry)─917─919─920 (Boss + Exit)
    ↑                 ↓
  (from 915)      (back to 900)
```

**Room Details:**
- 916: Entry hall, 1 Chimera + 1 Cerberus
- 917: Central chamber, 1 Hydra
- 918: Side treasury (north branch), 2 Chimeras + rare loot
- 919: Passage, 1 Titan
- 920: Final boss chamber, 1 Titan + 1 Hydra + loot chest (Divine equipment) + exit portal

---

## Implementation Checklist

### Phase 1: Room Creation ✅ (In Progress)
- [ ] Create Room 900 (Hub/Entrance)
- [ ] Create Floor 1 rooms (901-905) with linear layout
- [ ] Create Floor 2 rooms (906-910) with branching layout
- [ ] Create Floor 3 rooms (911-915) with linear layout
- [ ] Create Floor 4 rooms (916-920) with branching layout
- [ ] Add all rooms to RoomData.cs dictionary

### Phase 2: Enemy Population
- [ ] Add enemies to each room using OriginalNPCs
- [ ] Set CanRespawn = true for dungeon rooms
- [ ] Configure respawn times (or disable for test area?)

### Phase 3: Loot Implementation
- [ ] Add equipment drops to final room of each floor (905, 910, 915, 920)
- [ ] Consider adding items as room objects vs. enemy drops
- [ ] Add gold/potion drops to mid-floor treasure rooms (908, 918)

### Phase 4: Door Unlock System
- [ ] Create "locked exit" state for down stairs
- [ ] Check if all enemies defeated before allowing descent
- [ ] Add visual indicator when exit unlocks ("The way down opens!")

### Phase 5: Reset Mechanic
- [ ] Track dungeon state (current floor, cleared rooms, etc.)
- [ ] Implement reset on "up" command from any floor
- [ ] Reset all enemy spawns, loot, progress
- [ ] Return player to Room 900

### Phase 6: Testing
- [ ] Test room navigation (all exits work)
- [ ] Test enemy encounters at each level range
- [ ] Test loot drops
- [ ] Test door unlock system
- [ ] Test reset mechanic
- [ ] Test full 1-20 progression

---

## Future Enhancements (Act II/III)

### Procedural Generation
- Create 5-10 templates per floor
- Randomly select template on entry
- Randomize enemy spawns within template

### Buffs/Perks System
- Stackable buffs as player progresses deeper
- Examples: "+10% damage", "Heal 5 HP per kill", "+2 Speed"
- Carried through run, lost on reset

### Boss Mechanics
- Mini-bosses at floors 2 and 4
- Special loot from bosses
- Achievement tracking

### Difficulty Scaling
- Optional "hard mode" with tougher enemies
- Better rewards for higher difficulty

---

## Notes

- Enemies use new mythological creatures (Satyr, Harpy, Minotaur, etc.)
- Loot uses new dungeon equipment (Bronze Gladius, Hero's Blade, etc.)
- `tpto 900` command provides quick access for testing
- Dungeon is self-contained and doesn't affect main game progression
- Can be used to quickly test abilities at any level by progressing through floors
