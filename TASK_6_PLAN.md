# Task 6 Implementation Plan: Idle Recruits in Guild Hall

## Overview
Make idle recruits (not in active party) appear as NPCs in guild hall rooms so the guild feels more alive.

## Guild Hall Rooms (from RoomData.cs)
- Room 1: guildHallBedroom
- Room 2: guildHallHallway
- Room 3: guildHallStudy
- Room 4: guildHallCommonArea

## Recruit Placement Rules
- **Legionnaires** → Common Area (Room 4)
- **Venators** → Study (Room 3)
- **Oracles** → Study (Room 3)

## Implementation Steps

### Step 1: Create RecruitNPCManager
Create `/Managers/RecruitNPCManager.cs` with:
```csharp
public class RecruitNPCManager
{
    private GameContext gameContext;
    private Dictionary<int, List<NPC>> dynamicNPCs; // Room -> Dynamic NPCs

    public void SpawnIdleRecruitsInRoom(int roomId)
    {
        // 1. Check if room is a guild hall room
        if (!IsGuildHallRoom(roomId)) return;

        // 2. Get idle recruits (not in active party)
        var idleRecruits = GetIdleRecruits();

        // 3. For each idle recruit, create temporary NPC based on class
        // 4. Add to room.NPCs
        // 5. Track in dynamicNPCs dictionary
    }

    public void ClearDynamicNPCsInRoom(int roomId)
    {
        // Remove dynamically spawned recruits from room
    }

    private bool IsGuildHallRoom(int roomId)
    {
        return roomId >= 1 && roomId <= 4;
    }

    private List<Recruit> GetIdleRecruits()
    {
        // Return recruits not in active party
    }

    private NPC CreateRecruitNPC(Recruit recruit)
    {
        // Create NPC representation with simple dialogue
    }
}
```

### Step 2: Add Recruit Dialogue
Simple dialogue templates:
- **Generic**: "Ready when you need me."
- **Legionnaire**: "Just keeping my blade sharp."
- **Venator**: "Waiting for the next hunt."
- **Oracle**: "Studying these ancient tomes while I wait."

### Step 3: Hook into Room Entry/Exit
Find where the player enters/exits rooms (likely in GameEngine.cs or GameController.cs):
```csharp
// On room enter:
recruitNPCManager.ClearDynamicNPCsInRoom(previousRoom);
recruitNPCManager.SpawnIdleRecruitsInRoom(newRoom);

// On room exit:
recruitNPCManager.ClearDynamicNPCsInRoom(currentRoom);
```

### Step 4: Prevent Duplicates
Ensure recruits don't appear if they're already in:
- player.ActiveParty
- Already spawned in another guild room

### Step 5: Room-Class Mapping
```csharp
private int GetPreferredRoom(Recruit recruit)
{
    return recruit.Class.Name switch
    {
        "Legionnaire" => 4, // Common Area
        "Venator" => 3,     // Study
        "Oracle" => 3,      // Study
        _ => 4              // Default to Common Area
    };
}
```

## Files to Modify
1. **Create:** `/Managers/RecruitNPCManager.cs`
2. **Modify:** Find room entry/exit handler (GameEngine.cs or GameController.cs)
3. **Optional:** Add to GameContext for access

## Testing Checklist
- [ ] Idle recruits appear in appropriate guild hall rooms
- [ ] Active party members do NOT appear as NPCs
- [ ] NPCs disappear when leaving guild area
- [ ] Dialogue works for spawned NPCs
- [ ] No duplicates across rooms
- [ ] Works correctly after save/load

## Edge Cases to Handle
- What if guild has no recruits? (Nothing spawns)
- What if all recruits are in active party? (Nothing spawns)
- Player logs out in guild hall? (Dynamic NPCs should not save)

## Integration with Task 5 (Optional Enhancement)
Could use milestone system to unlock new guild rooms:
- 3 recruits → Unlock Training Yard (new room)
- 5 recruits → Unlock Barracks (new room)
- Update room placement when new rooms unlocked
