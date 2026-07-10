using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;

namespace GuildMaster.Managers
{
    /// <summary>
    /// Rules for the Ancient Ruins dungeon (rooms 900-920): the way down to the
    /// next floor stays barred until every enemy on the current floor is dead.
    /// Cleared-state is computed live from room NPC lists, so it survives
    /// save/load for free and re-locks naturally when the dungeon resets.
    /// </summary>
    public static class DungeonRules
    {
        // gate room -> (first room, last room) of the floor it closes off
        private static readonly Dictionary<int, (int Start, int End)> Gates = new()
        {
            [905] = (901, 905),   // Floor 1 -> Floor 2
            [910] = (906, 910),   // Floor 2 -> Floor 3
            [915] = (911, 915),   // Floor 3 -> Floor 4
        };

        public static bool IsDungeonRoom(int roomId) => roomId >= 900 && roomId <= 920;

        public static bool IsGateRoom(int roomId) => Gates.ContainsKey(roomId);

        /// <summary>The floor range containing this room, if it's a dungeon floor room.</summary>
        public static (int Start, int End)? FloorOf(int roomId)
        {
            foreach (var range in Gates.Values)
                if (roomId >= range.Start && roomId <= range.End)
                    return range;
            if (roomId >= 916 && roomId <= 920) return (916, 920);   // final floor, no gate below
            return null;
        }

        public static int LivingEnemiesOnFloor(Dictionary<int, Room> rooms, int start, int end)
        {
            int count = 0;
            for (int id = start; id <= end; id++)
                if (rooms.TryGetValue(id, out var room))
                    count += room.NPCs.Count(n => n.IsHostile && n.Health > 0);
            return count;
        }

        /// <summary>True if moving down from this room is currently barred.</summary>
        public static bool IsDescentBlocked(Dictionary<int, Room> rooms, int roomId, out int enemiesRemaining)
        {
            enemiesRemaining = 0;
            if (!Gates.TryGetValue(roomId, out var floor)) return false;
            enemiesRemaining = LivingEnemiesOnFloor(rooms, floor.Start, floor.End);
            return enemiesRemaining > 0;
        }
    }
}
