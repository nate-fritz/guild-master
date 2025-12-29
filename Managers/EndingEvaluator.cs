using System;
using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;

namespace GuildMaster.Managers
{
    /// <summary>
    /// Evaluates player choices to determine ending eligibility.
    /// Choices are tracked using quest flags with the "choice_" prefix.
    /// </summary>
    public class EndingEvaluator
    {
        private readonly GameContext context;

        public EndingEvaluator(GameContext gameContext)
        {
            context = gameContext;
        }

        /// <summary>
        /// Defines all possible endings and their requirements.
        /// </summary>
        private Dictionary<string, EndingDefinition> GetEndingDefinitions()
        {
            return new Dictionary<string, EndingDefinition>
            {
                // Example endings - these will be defined based on actual story content
                ["heroic_victory"] = new EndingDefinition
                {
                    Name = "Heroic Victory",
                    Description = "You led your guild to victory with honor and compassion.",
                    RequiredFlags = new List<string>
                    {
                        "choice_spared_enemy",
                        "choice_helped_civilians",
                        "choice_rejected_dark_power"
                    },
                    ForbiddenFlags = new List<string>
                    {
                        "choice_betrayed_ally",
                        "choice_accepted_corruption"
                    }
                },
                ["pragmatic_victory"] = new EndingDefinition
                {
                    Name = "Pragmatic Victory",
                    Description = "You won the war through strategic choices, not all of them honorable.",
                    RequiredFlags = new List<string>
                    {
                        "choice_tactical_sacrifice",
                        "choice_allied_with_merchant_guild"
                    },
                    ForbiddenFlags = new List<string>()
                },
                ["dark_victory"] = new EndingDefinition
                {
                    Name = "Dark Victory",
                    Description = "You seized power through ruthless means and dark magic.",
                    RequiredFlags = new List<string>
                    {
                        "choice_accepted_corruption",
                        "choice_sacrificed_ally"
                    },
                    ForbiddenFlags = new List<string>
                    {
                        "choice_rejected_dark_power"
                    }
                },
                ["peaceful_resolution"] = new EndingDefinition
                {
                    Name = "Peaceful Resolution",
                    Description = "You found a way to end the conflict without bloodshed.",
                    RequiredFlags = new List<string>
                    {
                        "choice_negotiated_peace",
                        "choice_united_factions",
                        "choice_discovered_truth"
                    },
                    ForbiddenFlags = new List<string>()
                }
            };
        }

        /// <summary>
        /// Gets all endings the player currently qualifies for.
        /// Returns them in priority order (most restrictive first).
        /// </summary>
        public List<EndingResult> GetEligibleEndings()
        {
            var definitions = GetEndingDefinitions();
            var eligible = new List<EndingResult>();

            foreach (var kvp in definitions)
            {
                if (IsEligibleForEnding(kvp.Value))
                {
                    eligible.Add(new EndingResult
                    {
                        Id = kvp.Key,
                        Name = kvp.Value.Name,
                        Description = kvp.Value.Description,
                        MeetsRequirements = true
                    });
                }
            }

            // Sort by number of required flags (most specific endings first)
            return eligible.OrderByDescending(e =>
                definitions[e.Id].RequiredFlags.Count).ToList();
        }

        /// <summary>
        /// Gets the primary ending the player qualifies for (highest priority).
        /// </summary>
        public EndingResult? GetPrimaryEnding()
        {
            var eligible = GetEligibleEndings();
            return eligible.FirstOrDefault();
        }

        /// <summary>
        /// Checks if player is eligible for a specific ending.
        /// </summary>
        public bool IsEligibleForEnding(string endingId)
        {
            var definitions = GetEndingDefinitions();
            if (!definitions.ContainsKey(endingId))
                return false;

            return IsEligibleForEnding(definitions[endingId]);
        }

        /// <summary>
        /// Internal eligibility check based on ending definition.
        /// </summary>
        private bool IsEligibleForEnding(EndingDefinition ending)
        {
            // Check all required flags are set
            foreach (var flag in ending.RequiredFlags)
            {
                if (!context.GetQuestFlag(flag))
                    return false;
            }

            // Check none of the forbidden flags are set
            foreach (var flag in ending.ForbiddenFlags)
            {
                if (context.GetQuestFlag(flag))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a summary of the player's major choices for debugging/review.
        /// </summary>
        public List<string> GetPlayerChoices()
        {
            var choices = new List<string>();

            // Get all quest flags that start with "choice_"
            if (context.Player?.QuestFlags != null)
            {
                foreach (var flag in context.Player.QuestFlags)
                {
                    if (flag.Key.StartsWith("choice_") && flag.Value)
                    {
                        choices.Add(flag.Key);
                    }
                }
            }

            return choices;
        }

        /// <summary>
        /// Displays a report of eligible endings (for debugging).
        /// </summary>
        public void DisplayEndingReport()
        {
            var choices = GetPlayerChoices();
            Console.WriteLine("\n=== Player Choices ===");
            if (choices.Count == 0)
            {
                Console.WriteLine("No major choices made yet.");
            }
            else
            {
                foreach (var choice in choices)
                {
                    Console.WriteLine($"- {choice}");
                }
            }

            Console.WriteLine("\n=== Eligible Endings ===");
            var eligible = GetEligibleEndings();
            if (eligible.Count == 0)
            {
                Console.WriteLine("No endings unlocked yet.");
            }
            else
            {
                foreach (var ending in eligible)
                {
                    Console.WriteLine($"- {ending.Name}: {ending.Description}");
                }
            }
        }
    }

    /// <summary>
    /// Defines the requirements for a specific ending.
    /// </summary>
    public class EndingDefinition
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> RequiredFlags { get; set; } = new();
        public List<string> ForbiddenFlags { get; set; } = new();
    }

    /// <summary>
    /// Result object for ending evaluation.
    /// </summary>
    public class EndingResult
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public bool MeetsRequirements { get; set; }
    }
}
