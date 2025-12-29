using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;

namespace GuildMaster.Managers
{
    /// <summary>
    /// Manages region access control and provides methods to check/modify region availability.
    /// </summary>
    public class RegionManager
    {
        private readonly GameContext context;

        // Define region boundaries
        private readonly Dictionary<string, RegionDefinition> regionDefinitions = new()
        {
            ["guild"] = new RegionDefinition
            {
                Id = "guild",
                Name = "Guild Hall",
                RoomIdStart = 1,
                RoomIdEnd = 7,
                IsStartingRegion = true
            },
            ["west"] = new RegionDefinition
            {
                Id = "west",
                Name = "Western Territory",
                RoomIdStart = 8,
                RoomIdEnd = 26,
                IsStartingRegion = true
            },
            ["east"] = new RegionDefinition
            {
                Id = "east",
                Name = "Eastern Territory",
                RoomIdStart = 40,
                RoomIdEnd = 61,
                IsStartingRegion = false
            },
            ["belum"] = new RegionDefinition
            {
                Id = "belum",
                Name = "Belum Wastes",
                RoomIdStart = 68,
                RoomIdEnd = 90,
                IsStartingRegion = false
            },
            // Act II regions (placeholder for future content)
            ["north"] = new RegionDefinition
            {
                Id = "north",
                Name = "Northern Reaches",
                RoomIdStart = 100,
                RoomIdEnd = 119,
                IsStartingRegion = false
            },
            ["south"] = new RegionDefinition
            {
                Id = "south",
                Name = "Southern Provinces",
                RoomIdStart = 120,
                RoomIdEnd = 139,
                IsStartingRegion = false
            }
        };

        public RegionManager(GameContext gameContext)
        {
            context = gameContext;
        }

        /// <summary>
        /// Gets the region ID for a specific room number.
        /// </summary>
        public string? GetRegionForRoom(int roomId)
        {
            foreach (var region in regionDefinitions.Values)
            {
                if (roomId >= region.RoomIdStart && roomId <= region.RoomIdEnd)
                {
                    return region.Id;
                }
            }

            return null; // Room not in any defined region
        }

        /// <summary>
        /// Checks if a room is accessible to the player.
        /// </summary>
        public bool CanAccessRoom(int roomId)
        {
            var regionId = GetRegionForRoom(roomId);
            if (regionId == null)
                return false; // Room doesn't exist in any region

            return IsRegionUnlocked(regionId);
        }

        /// <summary>
        /// Checks if a specific region is unlocked.
        /// </summary>
        public bool IsRegionUnlocked(string regionId)
        {
            return context.Player?.UnlockedRegions?.Contains(regionId) ?? false;
        }

        /// <summary>
        /// Unlocks a region.
        /// </summary>
        public void UnlockRegion(string regionId)
        {
            if (context.Player?.UnlockedRegions != null &&
                regionDefinitions.ContainsKey(regionId) &&
                !context.Player.UnlockedRegions.Contains(regionId))
            {
                context.Player.UnlockedRegions.Add(regionId);

                // Optionally show a message
                if (regionDefinitions.TryGetValue(regionId, out var region))
                {
                    Services.AnsiConsole.MarkupLine($"\n[green]New region unlocked: {region.Name}[/]");
                }
            }
        }

        /// <summary>
        /// Locks a region (removes access).
        /// </summary>
        public void LockRegion(string regionId)
        {
            if (context.Player?.UnlockedRegions != null &&
                context.Player.UnlockedRegions.Contains(regionId))
            {
                context.Player.UnlockedRegions.Remove(regionId);
            }
        }

        /// <summary>
        /// Gets a list of all unlocked regions.
        /// </summary>
        public List<string> GetUnlockedRegions()
        {
            return context.Player?.UnlockedRegions?.ToList() ?? new List<string>();
        }

        /// <summary>
        /// Gets the region definition for a specific region ID.
        /// </summary>
        public RegionDefinition? GetRegionDefinition(string regionId)
        {
            return regionDefinitions.TryGetValue(regionId, out var region) ? region : null;
        }

        /// <summary>
        /// Gets all region definitions.
        /// </summary>
        public Dictionary<string, RegionDefinition> GetAllRegions()
        {
            return new Dictionary<string, RegionDefinition>(regionDefinitions);
        }

        /// <summary>
        /// Displays region status for debugging.
        /// </summary>
        public void DisplayRegionStatus()
        {
            Services.AnsiConsole.MarkupLine("\n=== Region Access ===");

            foreach (var region in regionDefinitions.Values.OrderBy(r => r.RoomIdStart))
            {
                bool unlocked = IsRegionUnlocked(region.Id);
                string status = unlocked ? "[green]UNLOCKED[/]" : "[dim]LOCKED[/]";
                Services.AnsiConsole.MarkupLine($"{status} {region.Name} (Rooms {region.RoomIdStart}-{region.RoomIdEnd})");
            }
        }

        /// <summary>
        /// Gets the region message when attempting to access a locked region.
        /// </summary>
        public string GetLockedRegionMessage(string regionId)
        {
            if (regionDefinitions.TryGetValue(regionId, out var region))
            {
                return $"You cannot access {region.Name} yet. Complete more of the story to unlock this region.";
            }

            return "That area is not accessible yet.";
        }
    }

    /// <summary>
    /// Defines a region's properties.
    /// </summary>
    public class RegionDefinition
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int RoomIdStart { get; set; }
        public int RoomIdEnd { get; set; }
        public bool IsStartingRegion { get; set; }
    }
}
