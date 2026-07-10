using GuildMaster.Models;
using System.Collections.Generic;

namespace GuildMaster.Data
{
    /// <summary>
    /// Room content lives in wwwroot/data/rooms.json (edit it directly or with
    /// tools/ContentEditor). RoomTemplateStore loads it at startup; this class
    /// remains as the stable entry point the rest of the game calls.
    /// The original code-built rooms were migrated 2026-07-06 (commit 9a8c6b9)
    /// with field-by-field parity verified; legacy code removed after playtest.
    /// </summary>
    public static class RoomData
    {
        public static Dictionary<int, Room> InitializeRooms(Dictionary<string, NPC> npcs)
        {
            return RoomTemplateStore.BuildRooms(npcs);
        }
    }
}
