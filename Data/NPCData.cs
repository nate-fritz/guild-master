using GuildMaster.Models;
using System.Collections.Generic;

namespace GuildMaster.Data
{
    /// <summary>
    /// NPC content - stats, loot, shops, and dialogue trees - lives in
    /// wwwroot/data/npcs.json (edit it directly or with tools/ContentEditor).
    /// NpcTemplateStore loads it at startup; this class remains as the stable
    /// entry point the rest of the game calls.
    /// The original code-built NPCs were migrated 2026-07-09 (commit 7a1668c)
    /// with round-trip parity verified; legacy code removed after playtest.
    /// </summary>
    public static class NPCData
    {
        public static Dictionary<string, NPC> InitializeNPCs()
        {
            return NpcTemplateStore.BuildNpcs();
        }
    }
}
