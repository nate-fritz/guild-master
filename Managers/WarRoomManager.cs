using System;
using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;
using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;

namespace GuildMaster.Managers
{
    /// <summary>
    /// Manages the War Room strategic layer (Act III).
    /// Handles seals, squads, crises, and turn-based strategy gameplay.
    /// </summary>
    public class WarRoomManager
    {
        private readonly GameContext context;
        private readonly SquadCombatResolver combatResolver;
        private readonly CrisisGenerator crisisGenerator;

        public WarRoomState WarRoomState { get; private set; }

        public WarRoomManager(GameContext gameContext)
        {
            context = gameContext;
            combatResolver = new SquadCombatResolver();
            crisisGenerator = new CrisisGenerator();
            WarRoomState = new WarRoomState();
        }

        /// <summary>
        /// Initializes the War Room with default seals.
        /// </summary>
        public void InitializeWarRoom()
        {
            WarRoomState.IsActive = true;
            WarRoomState.CurrentTurn = 1;
            WarRoomState.CurrentActionPoints = WarRoomState.ActionPointsPerTurn;

            // Display tutorial explanation
            AnsiConsole.MarkupLine("\n[bold yellow]═══════════════════════════════════════════════════════[/]");
            AnsiConsole.MarkupLine("[bold yellow]                  WAR ROOM - TUTORIAL                  [/]");
            AnsiConsole.MarkupLine("[bold yellow]═══════════════════════════════════════════════════════[/]\n");

            AnsiConsole.MarkupLine("[bold]WHAT IS THE WAR ROOM?[/]");
            AnsiConsole.MarkupLine("The War Room is your strategic command center for managing");
            AnsiConsole.MarkupLine("guild operations across the realm during times of crisis.\n");

            AnsiConsole.MarkupLine("[bold]THE FIVE SEALS:[/]");
            AnsiConsole.MarkupLine("Ancient magical barriers protect the realm from otherworldly");
            AnsiConsole.MarkupLine("threats. Each seal has [green]100 Integrity[/] and is located in a");
            AnsiConsole.MarkupLine("different region. [red]If a seal falls to 0, the realm is lost![/]\n");

            AnsiConsole.MarkupLine("[bold]CRISES:[/]");
            AnsiConsole.MarkupLine("Dangerous events that threaten specific seals. Each crisis");
            AnsiConsole.MarkupLine("has a [yellow]countdown timer[/] and [yellow]difficulty rating[/]. If not resolved");
            AnsiConsole.MarkupLine("in time, the targeted seal takes damage!\n");

            AnsiConsole.MarkupLine("[bold]SQUADS:[/]");
            AnsiConsole.MarkupLine("Form teams of up to 4 recruits to handle threats. You can:");
            AnsiConsole.MarkupLine("  • [cyan]Assign to Crisis[/] - Send squad to resolve an active threat");
            AnsiConsole.MarkupLine("  • [cyan]Assign to Seal[/] - Station squad to defend a seal proactively\n");

            AnsiConsole.MarkupLine("[bold]ACTION POINTS (AP):[/]");
            AnsiConsole.MarkupLine("You have [green]3 AP per turn[/]. Most actions cost 1 AP.");
            AnsiConsole.MarkupLine("Plan carefully! When you [yellow]End Turn[/], all actions resolve:\n");
            AnsiConsole.MarkupLine("  • Squads fight assigned crises");
            AnsiConsole.MarkupLine("  • Unresolved crises damage their targeted seals");
            AnsiConsole.MarkupLine("  • New crises may appear");
            AnsiConsole.MarkupLine("  • Your AP resets to 3\n");

            AnsiConsole.MarkupLine("[bold red]YOUR GOAL:[/]");
            AnsiConsole.MarkupLine("[bold]Keep all five seals intact while managing threats across[/]");
            AnsiConsole.MarkupLine("[bold]the realm. Balance offense and defense to survive![/]\n");

            AnsiConsole.MarkupLine("[dim]Press Enter to begin...[/]");
            Console.ReadLine();

            // Create the five seals
            WarRoomState.Seals = new List<Seal>
            {
                new Seal
                {
                    Id = "seal_north",
                    Name = "Northern Seal",
                    Description = "Guards against northern threats",
                    Location = "Frostpeak Mountains",
                    Integrity = 100,
                    MaxIntegrity = 100
                },
                new Seal
                {
                    Id = "seal_south",
                    Name = "Southern Seal",
                    Description = "Guards against southern threats",
                    Location = "Sunfire Desert",
                    Integrity = 100,
                    MaxIntegrity = 100
                },
                new Seal
                {
                    Id = "seal_east",
                    Name = "Eastern Seal",
                    Description = "Guards against eastern threats",
                    Location = "Dawnsea Cliffs",
                    Integrity = 100,
                    MaxIntegrity = 100
                },
                new Seal
                {
                    Id = "seal_west",
                    Name = "Western Seal",
                    Description = "Guards against western threats",
                    Location = "Twilight Forest",
                    Integrity = 100,
                    MaxIntegrity = 100
                },
                new Seal
                {
                    Id = "seal_central",
                    Name = "Central Seal",
                    Description = "The heart of the seal network",
                    Location = "Imperial Capital",
                    Integrity = 100,
                    MaxIntegrity = 100
                }
            };

            AnsiConsole.MarkupLine("\n[green]War Room initialized! The five seals are active.[/]");
        }

        /// <summary>
        /// Displays the War Room status overview.
        /// </summary>
        public void DisplayWarRoomStatus()
        {
            AnsiConsole.MarkupLine("\n[bold]=== WAR ROOM ===[/]");
            AnsiConsole.MarkupLine($"Turn: {WarRoomState.CurrentTurn} | Action Points: {WarRoomState.CurrentActionPoints}/{WarRoomState.ActionPointsPerTurn}");
            AnsiConsole.MarkupLine("");

            // Display seals
            AnsiConsole.MarkupLine("[bold]=== SEALS ===[/]");
            foreach (var seal in WarRoomState.Seals)
            {
                string color = seal.GetStatusColor();
                AnsiConsole.MarkupLine($"[{color}]{seal.Name}[/]: {seal.Integrity}/{seal.MaxIntegrity} ({seal.Location})");
            }

            AnsiConsole.MarkupLine("");

            // Display active crises
            AnsiConsole.MarkupLine($"[bold]=== ACTIVE CRISES ({WarRoomState.ActiveCrises.Count}) ===[/]");
            if (WarRoomState.ActiveCrises.Count == 0)
            {
                AnsiConsole.MarkupLine("[dim]No active crises[/]");
            }
            else
            {
                foreach (var crisis in WarRoomState.ActiveCrises)
                {
                    var seal = WarRoomState.Seals.FirstOrDefault(s => s.Id == crisis.TargetSealId);
                    string sealName = seal?.Name ?? "Unknown";
                    string turnColor = crisis.GetTurnColor();
                    AnsiConsole.MarkupLine($"[{turnColor}]{crisis.Name}[/] - Threatens {sealName} - [{turnColor}]{crisis.TurnsRemaining} turns remaining[/] (Difficulty: {crisis.DifficultyRating})");
                }
            }

            AnsiConsole.MarkupLine("");

            // Display squads
            AnsiConsole.MarkupLine($"[bold]=== SQUADS ({WarRoomState.Squads.Count}) ===[/]");
            if (WarRoomState.Squads.Count == 0)
            {
                AnsiConsole.MarkupLine("[dim]No squads formed[/]");
            }
            else
            {
                foreach (var squad in WarRoomState.Squads)
                {
                    string status = squad.IsDeployed() ? $"[yellow]{squad.GetAssignmentDescription()}[/]" : "[green]Available[/]";
                    AnsiConsole.MarkupLine($"{squad.Name} ({squad.Members.Count}/{squad.MaxSize} members, Power: {squad.GetCombatPower()}) - {status}");
                }
            }

            AnsiConsole.MarkupLine("");
        }

        /// <summary>
        /// Creates a new squad from available recruits.
        /// </summary>
        public bool CreateSquad(string squadName, List<Recruit> members)
        {
            if (members.Count == 0 || members.Count > 4)
            {
                AnsiConsole.MarkupLine("[red]Squad must have 1-4 members.[/]");
                return false;
            }

            var squad = new Squad
            {
                Id = $"squad_{WarRoomState.Squads.Count + 1}",
                Name = squadName,
                Members = new List<Recruit>(members),
                MaxSize = 4
            };

            WarRoomState.Squads.Add(squad);
            AnsiConsole.MarkupLine($"[green]Squad '{squadName}' created with {members.Count} members.[/]");
            return true;
        }

        /// <summary>
        /// Assigns a squad to handle a crisis.
        /// </summary>
        public bool AssignSquadToCrisis(Squad squad, CrisisEvent crisis)
        {
            if (squad.IsDeployed())
            {
                AnsiConsole.MarkupLine($"[red]{squad.Name} is already deployed.[/]");
                return false;
            }

            if (!WarRoomState.SpendActionPoints(1))
            {
                AnsiConsole.MarkupLine("[red]Not enough action points.[/]");
                return false;
            }

            squad.AssignedCrisisId = crisis.Id;
            AnsiConsole.MarkupLine($"[green]{squad.Name} assigned to handle {crisis.Name}.[/]");
            return true;
        }

        /// <summary>
        /// Assigns a squad to defend a seal.
        /// </summary>
        public bool AssignSquadToSeal(Squad squad, Seal seal)
        {
            if (squad.IsDeployed())
            {
                AnsiConsole.MarkupLine($"[red]{squad.Name} is already deployed.[/]");
                return false;
            }

            if (!WarRoomState.SpendActionPoints(1))
            {
                AnsiConsole.MarkupLine("[red]Not enough action points.[/]");
                return false;
            }

            squad.AssignedSealId = seal.Id;
            AnsiConsole.MarkupLine($"[green]{squad.Name} assigned to defend {seal.Name}.[/]");
            return true;
        }

        /// <summary>
        /// Processes the end of turn - resolves combats, advances crises, generates new crises.
        /// </summary>
        public void ProcessEndOfTurn()
        {
            AnsiConsole.MarkupLine("\n[bold]=== PROCESSING TURN ===[/]");

            // Resolve squad vs crisis combats
            var assignedSquads = WarRoomState.Squads.Where(s => s.AssignedCrisisId != null).ToList();
            foreach (var squad in assignedSquads)
            {
                var crisis = WarRoomState.ActiveCrises.FirstOrDefault(c => c.Id == squad.AssignedCrisisId);
                if (crisis != null)
                {
                    var result = combatResolver.ResolveCombat(squad, crisis);
                    AnsiConsole.MarkupLine($"\n{combatResolver.GetCombatDescription(result)}");

                    // Apply casualties
                    if (result.Casualties > 0)
                    {
                        squad.ApplyCasualties(result.Casualties);
                        WarRoomState.TotalCasualties += result.Casualties;
                    }

                    // Resolve crisis if successful
                    if (result.Success)
                    {
                        crisis.IsResolved = true;
                        WarRoomState.ActiveCrises.Remove(crisis);
                        WarRoomState.ResolvedCrises.Add(crisis);
                    }

                    // Unassign squad
                    squad.AssignedCrisisId = null;
                }
            }

            // Advance unresolved crises (those still active)
            var unresolvedCrises = WarRoomState.ActiveCrises.Where(c => !c.IsResolved).ToList();
            foreach (var crisis in unresolvedCrises)
            {
                crisis.DecrementTurns();

                // Check if squads are defending the threatened seal
                var defendingSquads = WarRoomState.Squads.Where(s => s.AssignedSealId == crisis.TargetSealId).ToList();
                int totalDefense = defendingSquads.Sum(s => s.GetCombatPower());

                // If crisis timer runs out
                if (crisis.TurnsRemaining <= 0)
                {
                    var seal = WarRoomState.Seals.FirstOrDefault(s => s.Id == crisis.TargetSealId);
                    if (seal != null)
                    {
                        // Defenders can mitigate damage
                        int damage = crisis.SealDamage;
                        if (totalDefense > 0)
                        {
                            float mitigation = (float)totalDefense / crisis.DifficultyRating;
                            damage = (int)(damage * (1.0f - mitigation * 0.5f));  // Max 50% mitigation
                        }

                        seal.TakeDamage(damage);
                        AnsiConsole.MarkupLine($"\n[red]{crisis.Name} damages {seal.Name} for {damage} integrity![/]");

                        // Remove crisis
                        crisis.IsResolved = true;
                        WarRoomState.ActiveCrises.Remove(crisis);
                        WarRoomState.ResolvedCrises.Add(crisis);
                    }
                }
            }

            // Generate new crisis
            if (crisisGenerator.ShouldGenerateCrisis(WarRoomState.CurrentTurn, WarRoomState.ActiveCrises.Count))
            {
                var newCrisis = crisisGenerator.GenerateCrisis(WarRoomState.Seals, WarRoomState.CurrentTurn);
                WarRoomState.ActiveCrises.Add(newCrisis);
                AnsiConsole.MarkupLine($"\n[yellow]NEW CRISIS:[/] {newCrisis.Name} threatens {WarRoomState.Seals.FirstOrDefault(s => s.Id == newCrisis.TargetSealId)?.Name}!");
            }

            // Check for game over
            if (WarRoomState.HasAnySealFailed())
            {
                AnsiConsole.MarkupLine("\n[red bold]A SEAL HAS FALLEN! THE REALM IS DOOMED![/]");
                // Game over logic would go here
            }

            // Start new turn
            WarRoomState.StartNewTurn();
            AnsiConsole.MarkupLine($"\n[green]Turn {WarRoomState.CurrentTurn} begins. Action points restored to {WarRoomState.ActionPointsPerTurn}.[/]");
        }

        /// <summary>
        /// Gets the War Room state for saving.
        /// </summary>
        public WarRoomState GetWarRoomState()
        {
            return WarRoomState;
        }

        /// <summary>
        /// Restores War Room state from save.
        /// </summary>
        public void SetWarRoomState(WarRoomState state)
        {
            WarRoomState = state ?? new WarRoomState();
        }
    }
}
