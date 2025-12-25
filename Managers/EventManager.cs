using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;
using GuildMaster.Data;

namespace GuildMaster.Managers
{
    /// <summary>
    /// Manages event triggers that automatically fire when entering rooms under specific conditions
    /// </summary>
    public class EventManager
    {
        private readonly GameContext context;
        private List<EventData> allEvents;
        private HashSet<string> triggeredEventIds;

        public EventManager(GameContext gameContext)
        {
            context = gameContext;
            allEvents = new List<EventData>();
            triggeredEventIds = new HashSet<string>();
        }

        /// <summary>
        /// Loads all event definitions
        /// </summary>
        public void LoadEvents()
        {
            allEvents.Clear();

            // Load events from EventDataDefinitions
            var definedEvents = EventDataDefinitions.GetAllEvents();
            allEvents.AddRange(definedEvents);

            if (context.Player.DebugLogsEnabled)
            {
                AnsiConsole.MarkupLine($"[dim]Loaded {allEvents.Count} event(s)[/]");
            }
        }

        /// <summary>
        /// Adds a custom event to the event list (for dynamic event creation)
        /// </summary>
        public void AddEvent(EventData eventData)
        {
            if (eventData == null)
                return;

            // Remove any existing event with the same ID
            allEvents.RemoveAll(e => e.EventId == eventData.EventId);

            // Add the new event
            allEvents.Add(eventData);
        }

        /// <summary>
        /// Checks if any events should trigger for a specific room
        /// Returns the highest priority event that meets all conditions
        /// </summary>
        public EventData CheckForEvent(int roomId)
        {
            // Get all events for this room
            var roomEvents = allEvents
                .Where(e => e.TriggerRoomId == roomId)
                .OrderByDescending(e => e.Priority)
                .ToList();

            // Check each event in priority order
            foreach (var evt in roomEvents)
            {
                // Skip if this is a one-time event that already triggered
                if (evt.IsOneTime && triggeredEventIds.Contains(evt.EventId))
                    continue;

                // Evaluate all conditions
                if (EvaluateConditions(evt))
                {
                    return evt;
                }
            }

            return null; // No event triggered
        }

        /// <summary>
        /// Evaluates all conditions for an event
        /// Returns true only if ALL conditions are met (AND logic)
        /// </summary>
        private bool EvaluateConditions(EventData evt)
        {
            // No conditions means event always triggers (if not already triggered)
            if (evt.Conditions == null || evt.Conditions.Count == 0)
                return true;

            // All conditions must be true
            foreach (var condition in evt.Conditions)
            {
                if (!condition.Evaluate(context, triggeredEventIds))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Executes all actions for an event
        /// </summary>
        public void ExecuteActions(EventData evt)
        {
            if (evt.Actions == null || evt.Actions.Count == 0)
                return;

            foreach (var action in evt.Actions)
            {
                action.Execute(context);
            }
        }

        /// <summary>
        /// Marks an event as triggered (for one-time events)
        /// </summary>
        public void MarkEventTriggered(string eventId)
        {
            if (!string.IsNullOrEmpty(eventId))
            {
                triggeredEventIds.Add(eventId);
            }
        }

        /// <summary>
        /// Checks if a specific event has already been triggered
        /// </summary>
        public bool HasEventTriggered(string eventId)
        {
            return triggeredEventIds.Contains(eventId);
        }

        /// <summary>
        /// Gets the set of triggered event IDs (for save/load)
        /// </summary>
        public HashSet<string> GetTriggeredEvents()
        {
            return new HashSet<string>(triggeredEventIds);
        }

        /// <summary>
        /// Sets the triggered event IDs (for save/load)
        /// </summary>
        public void SetTriggeredEvents(HashSet<string> triggered)
        {
            triggeredEventIds = triggered ?? new HashSet<string>();
        }

        /// <summary>
        /// Clears all triggered events (for testing/debugging)
        /// </summary>
        public void ClearTriggeredEvents()
        {
            triggeredEventIds.Clear();
        }

        /// <summary>
        /// Gets count of loaded events
        /// </summary>
        public int GetEventCount()
        {
            return allEvents.Count;
        }

        /// <summary>
        /// Gets all events for a specific room (for debugging)
        /// </summary>
        public List<EventData> GetEventsForRoom(int roomId)
        {
            return allEvents.Where(e => e.TriggerRoomId == roomId).ToList();
        }
    }
}
