# Test Rooms Guide - Recruit Testing

## Quick Start

To access the test recruit area:
```
teleport 999
```

To return to the guild hall:
```
teleport 1
```

---

## Test Rooms Layout (999-991)

The 9 test rooms are connected in a linear path from north to south:

```
999 (Valeria)
 ↓
998 (Darius)
 ↓
997 (Lyra)
 ↓
996 (Marcus the Bold)
 ↓
995 (Aria Swift)
 ↓
994 (Aldric the Wise)
 ↓
993 (Thora)
 ↓
992 (Fenris)
 ↓
991 (Celestia)
```

You can walk through them with `north` and `south` commands, or teleport directly to any room ID.

---

## Test Recruits Overview

### Room 999 - Valeria
- **Class:** Legionnaire
- **Gender:** Female
- **Type:** Conversational
- **How to recruit:** Talk to her and select "Join the guild?"

### Room 998 - Darius
- **Class:** Venator
- **Gender:** Male
- **Type:** Conversational
- **How to recruit:** Talk to him and select "Join us?"

### Room 997 - Lyra
- **Class:** Oracle
- **Gender:** Female
- **Type:** Conversational
- **How to recruit:** Talk to her and select "Will you join?"

### Room 996 - Marcus the Bold
- **Class:** Legionnaire
- **Gender:** Male
- **Type:** Combat Recruit
- **How to recruit:** Talk to him (triggers combat) → Defeat him → He'll join
- **Stats:** 10 HP, 2 Attack, 1 Defense, 1 Speed

### Room 995 - Aria Swift
- **Class:** Venator
- **Gender:** Female
- **Type:** Combat Recruit
- **How to recruit:** Talk to her (triggers combat) → Defeat her → She'll join
- **Stats:** 8 HP, 3 Attack, 0 Defense, 2 Speed

### Room 994 - Aldric the Wise
- **Class:** Oracle
- **Gender:** Male
- **Type:** Combat Recruit
- **How to recruit:** Talk to him (triggers combat) → Defeat him → He'll join
- **Stats:** 6 HP, 4 Attack, 0 Defense, 1 Speed

### Room 993 - Thora
- **Class:** Legionnaire
- **Gender:** Female
- **Type:** Conversational
- **How to recruit:** Talk to her and select "Join us!"

### Room 992 - Fenris
- **Class:** Venator
- **Gender:** Male
- **Type:** Conversational
- **How to recruit:** Talk to him and select "Join the guild?"

### Room 991 - Celestia
- **Class:** Oracle
- **Gender:** Female
- **Type:** Conversational
- **How to recruit:** Talk to her and select "Please do!"

---

## Testing Progressive Guild Rooms

With these 9 test recruits available, you can easily test all guild room unlocks:

| Recruits | Unlocks | Room Name | Room ID |
|----------|---------|-----------|---------|
| 4 | Training Yard | 64 |
| 6 | Armory (with Guild Armorer vendor) | 65 |
| 8 | Treasury (with class-specific rings) | 66 |
| 10 | Portal Chamber (fast travel to 3 locations) | 67 |

### Testing Workflow:

1. **Start fresh** or with existing game
2. **Recruit 4 members** → Training Yard unlocks (west exit appears in Room 4)
3. **Recruit 6 members** → Armory unlocks (east exit appears in Room 4)
4. **Recruit 8 members** → Treasury unlocks (east exit appears in Armory)
5. **Recruit 10 members** → Portal Chamber unlocks (south exit appears in Treasury)

---

## Class Distribution

The 9 test recruits provide an even class distribution:

- **Legionnaire:** 3 recruits (Valeria, Marcus the Bold, Thora)
- **Venator:** 3 recruits (Darius, Aria Swift, Fenris)
- **Oracle:** 3 recruits (Lyra, Aldric the Wise, Celestia)

---

## Gender Distribution

- **Female:** 5 recruits (Valeria, Lyra, Aria Swift, Thora, Celestia)
- **Male:** 4 recruits (Darius, Marcus the Bold, Aldric the Wise, Fenris)

---

## Recruitment Type Distribution

- **Conversational:** 6 recruits (Valeria, Darius, Lyra, Thora, Fenris, Celestia)
- **Combat:** 3 recruits (Marcus the Bold, Aria Swift, Aldric the Wise)

---

## Notes

- These are **testing rooms only** - they're not part of the normal game world
- Use the `teleport` command to access them
- All test recruits will appear in the guild hall after recruitment
- Combat recruits are intentionally weak to make testing easier
- These rooms can be removed later or kept for debugging purposes
