# Active Playthrough Testing Notes

**Session Date:** 2026-01-09
**Tester:** User
**Build:** Latest (post-bug fixes from Jan 9)

---

## ✅ FIXED (2026-01-09) - Gate Puzzle Keys Not Working
- **Was:** "use keys on gate" command failed with cast error in loaded save games
- **Root Cause:** Puzzle state boolean values deserialized as JsonElement, not bool
- **Fixed:** Created GetPuzzleStateBool() helper to safely handle both cases
- **Status:** RESOLVED - Tested and working in both fresh games and loaded saves

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

## Next Testing Focus

- Continue playthrough to verify respawn behavior in cave
- Test Oracle combat balance with current abilities
- Verify all recent bug fixes (Jan 5-7) are working in integrated playthrough
- Check for any new issues introduced by recent changes
