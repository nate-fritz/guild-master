using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;

namespace GuildMaster.Managers
{
    /// <summary>
    /// Manages faction relationships and provides query methods for faction status.
    /// </summary>
    public class FactionManager
    {
        private readonly GameContext context;

        public FactionManager(GameContext gameContext)
        {
            context = gameContext;
        }

        /// <summary>
        /// Checks if the player is allied with a specific faction.
        /// </summary>
        public bool IsAlliedWith(string factionId)
        {
            return context.Player?.AlliedFactions?.Contains(factionId) ?? false;
        }

        /// <summary>
        /// Forms an alliance with a faction.
        /// </summary>
        public void AllyWithFaction(string factionId)
        {
            if (context.Player?.AlliedFactions != null && !context.Player.AlliedFactions.Contains(factionId))
            {
                context.Player.AlliedFactions.Add(factionId);
            }
        }

        /// <summary>
        /// Breaks an alliance with a faction.
        /// </summary>
        public void BreakAlliance(string factionId)
        {
            if (context.Player?.AlliedFactions != null && context.Player.AlliedFactions.Contains(factionId))
            {
                context.Player.AlliedFactions.Remove(factionId);
            }
        }

        /// <summary>
        /// Gets a list of all allied factions.
        /// </summary>
        public List<string> GetAlliedFactions()
        {
            return context.Player?.AlliedFactions?.ToList() ?? new List<string>();
        }

        /// <summary>
        /// Gets the count of allied factions.
        /// </summary>
        public int GetAlliedFactionCount()
        {
            return context.Player?.AlliedFactions?.Count ?? 0;
        }

        /// <summary>
        /// Checks if the player is allied with all specified factions.
        /// </summary>
        public bool IsAlliedWithAll(params string[] factionIds)
        {
            if (context.Player?.AlliedFactions == null)
                return false;

            foreach (var factionId in factionIds)
            {
                if (!context.Player.AlliedFactions.Contains(factionId))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the player is allied with any of the specified factions.
        /// </summary>
        public bool IsAlliedWithAny(params string[] factionIds)
        {
            if (context.Player?.AlliedFactions == null)
                return false;

            foreach (var factionId in factionIds)
            {
                if (context.Player.AlliedFactions.Contains(factionId))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Displays faction status for debugging.
        /// </summary>
        public void DisplayFactionStatus()
        {
            var factions = GetAlliedFactions();

            Services.AnsiConsole.MarkupLine("\n=== Allied Factions ===");
            if (factions.Count == 0)
            {
                Services.AnsiConsole.MarkupLine("[dim]No allied factions yet.[/]");
            }
            else
            {
                foreach (var faction in factions)
                {
                    Services.AnsiConsole.MarkupLine($"- {faction}");
                }
            }
        }
    }
}
