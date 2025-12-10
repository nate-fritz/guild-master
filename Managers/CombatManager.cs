using GuildMaster.Services;
using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuildMaster.Models;
using GuildMaster.Data;
using GuildMaster.Helpers;

namespace GuildMaster.Managers
{
    public class CombatManager
    {
        public enum StatusEffect
        {
            Stunned,
            Taunted,
            CannotAttack,
            Evasive
        }

        public enum CombatState
        {
            NotInCombat,
            SelectingAction,
            SelectingAttackTarget,
            SelectingAbility,
            SelectingAbilityTarget,
            SelectingItem,
            ProcessingTurn,
            DeathMenu,
            RecruitmentPrompt,
            CombatEnded
        }

        // Replace existing individual tracking with unified system
        private Dictionary<Character, Dictionary<StatusEffect, int>> statusEffects = new Dictionary<Character, Dictionary<StatusEffect, int>>();
        private Dictionary<Character, Character> taunters = new Dictionary<Character, Character>(); // Who is taunting whom

        // Combat state management
        private CombatState currentState = CombatState.NotInCombat;
        private List<NPC>? activeEnemies;
        private Room? combatRoom;
        private List<Combatant>? turnOrder;
        private int currentTurnIndex = 0;
        private int baseDefense = 0;
        private bool isDefending = false;
        private bool combatActive = true;
        private List<string>? currentConsumables;
        private List<NPC>? currentTargetList;
        private List<NPC>? recruitableNPCs;
        private int currentRecruitIndex = 0;
        private Ability? pendingAbility;
        private Character? abilityCharacter;
        private NPC? preselectedTarget;  // For passing target to ability executors

        private GameContext context;
        private Random random = new Random();

        // Turn delay configuration (in milliseconds)
        private const int TURN_DELAY_MS = 1000;
        private Action? onStateChanged;

        public bool IsInCombat => currentState != CombatState.NotInCombat && currentState != CombatState.CombatEnded;


        // Buff tracking
        private Dictionary<Character, int> battleCryTurns = new Dictionary<Character, int>();
        private Dictionary<Character, int> buffedAttack = new Dictionary<Character, int>();
        private Dictionary<Character, int> buffedDefense = new Dictionary<Character, int>();
        private Dictionary<Character, Dictionary<string, int>> abilityCooldowns = new Dictionary<Character, Dictionary<string, int>>();
        private Dictionary<Character, bool> evasiveFireActive = new Dictionary<Character, bool>();
        private Dictionary<Character, int> barrierAbsorption = new Dictionary<Character, int>();

        // Callback for when player dies
        private Action onPlayerDeath;

        public CombatManager(GameContext gameContext, Action onPlayerDeathCallback, Action stateChangedCallback = null)
        {
            this.context = gameContext;
            this.onPlayerDeath = onPlayerDeathCallback;
            this.onStateChanged = stateChangedCallback;
        }

        public void StartCombat(List<NPC> enemies, Room currentRoom)
        {
            try
            {
                AnsiConsole.MarkupLine("[dim]DEBUG: StartCombat called[/]");
                var player = context.Player;

                // Initialize combat state
                activeEnemies = enemies;
                combatRoom = currentRoom;
                currentState = CombatState.ProcessingTurn;
                currentTurnIndex = 0;
                baseDefense = player.Defense;
                isDefending = false;
                combatActive = true;

                AnsiConsole.MarkupLine("[dim]DEBUG: Combat state initialized[/]");

                // Pulsing red "COMBAT BEGINS" effect with animation
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine("<span class='combat-glow'>⚔ COMBAT BEGINS ⚔</span>");
                AnsiConsole.MarkupLine("");

                if (enemies.Count == 1)
                {
                    AnsiConsole.MarkupLine($"\nYou are fighting [#fc3838]{enemies[0].Name}![/]");
                }
                else
                {
                    Console.Write("\nYou are fighting: ");
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (i > 0) Console.Write(" and ");
                        AnsiConsole.Markup($"[#fc3838]{enemies[i].Name}[/]");
                    }
                    AnsiConsole.MarkupLine("!");
                }

                AnsiConsole.MarkupLine("[dim]DEBUG: About to roll initiative[/]");

                // Roll initiative
                turnOrder = RollInitiative(player, enemies);

                AnsiConsole.MarkupLine("[dim]DEBUG: Initiative rolled[/]");

                // Display initiative results
                AnsiConsole.MarkupLine("\nInitiative Order:");
                foreach (var combatant in turnOrder)
                {
                    string name = combatant.IsPlayer ? (combatant.Character == player ? "You" : combatant.Character.Name) : combatant.Name;
                    AnsiConsole.MarkupLine($"  {name}: {combatant.InitiativeRoll} (Speed: {combatant.Character.Speed})");
                }
                AnsiConsole.MarkupLine("");

                AnsiConsole.MarkupLine("[dim]DEBUG: About to process first turn[/]");

                // Start the first turn
                ProcessNextTurn();

                AnsiConsole.MarkupLine("[dim]DEBUG: ProcessNextTurn completed[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[#FF0000]FATAL ERROR in StartCombat: {ex.Message}[/]");
                AnsiConsole.MarkupLine($"[#808080]{ex.StackTrace}[/]");
                combatActive = false;
                currentState = CombatState.CombatEnded;
            }
        }

        private void ProcessNextTurn()
        {
            try
            {
                AnsiConsole.MarkupLine("[dim]DEBUG: ProcessNextTurn called[/]");
                var player = context.Player;

                if (turnOrder == null || activeEnemies == null)
                {
                    AnsiConsole.MarkupLine("[dim]DEBUG: turnOrder or activeEnemies is null[/]");
                    return;
                }

                AnsiConsole.MarkupLine($"[dim]DEBUG: turnOrder.Count={turnOrder.Count}, currentTurnIndex={currentTurnIndex}[/]");

                // Check if combat should end
                if (!combatActive || !player.IsAlive || !activeEnemies.Any(e => e.Health > 0))
                {
                    AnsiConsole.MarkupLine("[dim]DEBUG: Combat should end[/]");
                    HandleCombatEnd(player, activeEnemies, combatRoom, combatActive);
                    currentState = CombatState.CombatEnded;
                    return;
                }

                // Move to next valid combatant
                while (currentTurnIndex < turnOrder.Count)
                {
                    AnsiConsole.MarkupLine($"[dim]DEBUG: Processing turn index {currentTurnIndex}[/]");
                    var combatant = turnOrder[currentTurnIndex];

                    // Skip dead combatants
                    if (!combatant.IsAlive)
                    {
                        AnsiConsole.MarkupLine($"[dim]DEBUG: Skipping dead combatant[/]");
                        currentTurnIndex++;
                        continue;
                    }

                    // Check if combat should continue
                    if (!player.IsAlive || !activeEnemies.Any(e => e.Health > 0) || !combatActive)
                    {
                        AnsiConsole.MarkupLine("[dim]DEBUG: Combat should end (mid-turn check)[/]");
                        HandleCombatEnd(player, activeEnemies, combatRoom, combatActive);
                        currentState = CombatState.CombatEnded;
                        return;
                    }

                    AnsiConsole.MarkupLine($"[dim]DEBUG: About to display combat status[/]");
                    DisplayCombatStatus(player, activeEnemies, combatant);
                    AnsiConsole.MarkupLine($"[dim]DEBUG: Displayed combat status[/]");

                    if (combatant.IsPlayer && combatant.Character == player)
                {
                    // Process DOT damage at the start of player turn
                    if (player.ActiveDOTs != null && player.ActiveDOTs.Count > 0)
                    {
                        // Get the first DOT type BEFORE processing
                        DamageType firstDotType = player.ActiveDOTs[0].Type;

                        int dotDamage = player.ProcessDOTs();
                        if (dotDamage > 0)
                        {
                            player.Health -= dotDamage;

                            string dotColor = GetDOTColor(firstDotType);
                            string dotTypeName = GetDOTTypeName(firstDotType);
                            AnsiConsole.MarkupLine($"\n[{dotColor}]You take {dotDamage} damage from {dotTypeName}![/]");

                            if (player.Health <= 0)
                            {
                                AnsiConsole.MarkupLine($"[#FF0000]You have been defeated![/]");
                                combatActive = false;
                                HandleCombatEnd(player, activeEnemies, combatRoom, combatActive);
                                currentState = CombatState.CombatEnded;
                                return;
                            }
                        }
                    }

                    if (HasStatusEffect(player, StatusEffect.CannotAttack))
                    {
                        AnsiConsole.MarkupLine("\nYou cannot attack while maintaining your defensive stance!");
                        currentTurnIndex++;
                        ProcessNextTurn();
                        return;
                    }

                    // Show player action menu and wait for input
                    ShowPlayerActionMenu();
                    return;
                }
                else if (combatant.IsPlayer && combatant.Character is Recruit)
                {
                    HandlePartyMemberTurn(combatant.Character as Recruit, activeEnemies);
                }
                else
                {
                    HandleEnemyTurn(combatant.Character as NPC, player);

                    if (isDefending)
                    {
                        player.Defense = baseDefense;
                        isDefending = false;
                        AnsiConsole.MarkupLine("\n[#808080]Your defense returns to normal.[/]");
                    }
                }

                // Handle buff duration at end of turn
                if (battleCryTurns.ContainsKey(combatant.Character) && battleCryTurns[combatant.Character] > 0)
                {
                    battleCryTurns[combatant.Character]--;
                    if (battleCryTurns[combatant.Character] == 0)
                    {
                        buffedAttack.Remove(combatant.Character);
                        buffedDefense.Remove(combatant.Character);
                        AnsiConsole.MarkupLine($"[#808080]{combatant.Character.Name}'s battle cry effect wears off.[/]");
                    }
                }
                ReduceStatusDurations(combatant.Character);

                // Move to next turn and schedule with delay
                currentTurnIndex++;
                _ = ScheduleNextTurnAsync();
                return;
            }

            // If we've gone through all combatants, reset to the beginning
            if (currentTurnIndex >= turnOrder.Count)
            {
                AnsiConsole.MarkupLine("[dim]DEBUG: Resetting turn order[/]");
                currentTurnIndex = 0;
                _ = ScheduleNextTurnAsync();
            }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[#FF0000]FATAL ERROR in ProcessNextTurn: {ex.Message}[/]");
                AnsiConsole.MarkupLine($"[#808080]{ex.StackTrace}[/]");
                combatActive = false;
                currentState = CombatState.CombatEnded;
            }
        }

        private void ShowPlayerActionMenu()
        {
            var player = context.Player;
            var itemDescriptions = context.ItemDescriptions;

            currentState = CombatState.SelectingAction;

            AnsiConsole.MarkupLine("\n1. Attack");
            AnsiConsole.MarkupLine("2. Abilities");
            AnsiConsole.MarkupLine("3. Defend");
            AnsiConsole.MarkupLine("4. Flee");

            // Check if player has any consumable items
            var consumables = player.Inventory.Where(item =>
                itemDescriptions.Values.Any(room =>
                    room.ContainsKey(item) && room[item].IsConsumable)).ToList();

            currentConsumables = consumables;

            if (consumables.Count > 0)
            {
                AnsiConsole.MarkupLine("5. Items");
            }

            AnsiConsole.MarkupLine("");
            ShowStatusBar();
            AnsiConsole.MarkupLine("[dim](Enter a number to choose your action)[/]");
        }

        private void ShowStatusBar()
        {
            var player = context.Player;
            int hour = (int)player.CurrentHour;
            int minutes = (int)((player.CurrentHour - hour) * 60);
            string timeOfDay = hour < 12 ? "AM" : "PM";
            int displayHour = hour > 12 ? hour - 12 : hour;
            if (displayHour == 0) displayHour = 12;

            AnsiConsole.MarkupLine($"\n<span class='stats-bar'>[HP: {player.Health}/{player.MaxHealth} | EP: {player.Energy}/{player.MaxEnergy} | Day {player.CurrentDay}, {displayHour}:{minutes:D2} {timeOfDay} | Gold: {player.Gold} | Recruits: {player.Recruits.Count}/10]</span>");
        }

        public bool ProcessCombatInput(string input)
        {
            AnsiConsole.MarkupLine($"[dim]DEBUG: ProcessCombatInput called with input='{input}', IsInCombat={IsInCombat}, currentState={currentState}[/]");

            if (!IsInCombat)
            {
                AnsiConsole.MarkupLine("[dim]DEBUG: Not in combat, returning false[/]");
                return false;
            }

            var player = context.Player;

            switch (currentState)
            {
                case CombatState.SelectingAction:
                    HandleActionSelection(input);
                    break;

                case CombatState.SelectingAttackTarget:
                    HandleAttackTargetSelection(input);
                    break;

                case CombatState.SelectingAbility:
                    HandleAbilitySelection(input);
                    break;

                case CombatState.SelectingAbilityTarget:
                    HandleAbilityTargetSelection(input);
                    break;

                case CombatState.SelectingItem:
                    HandleItemSelection(input);
                    break;

                case CombatState.DeathMenu:
                    HandleDeathMenuSelection(input);
                    break;

                case CombatState.RecruitmentPrompt:
                    AnsiConsole.MarkupLine("[dim]DEBUG: Handling recruitment selection[/]");
                    HandleRecruitmentSelection(input);
                    break;

                default:
                    AnsiConsole.MarkupLine($"[dim]DEBUG: Unhandled state: {currentState}[/]");
                    return false;
            }

            return true;
        }

        private void HandleActionSelection(string input)
        {
            var player = context.Player;

            switch (input)
            {
                case "1": // Attack
                    StartPlayerAttack();
                    break;

                case "2": // Abilities
                    ShowAbilityMenu();
                    break;

                case "3": // Defend
                    isDefending = true;
                    if (player.Defense == 0)
                    {
                        player.Defense = 1;
                    }
                    else
                    {
                        player.Defense = baseDefense * 2;
                    }
                    AnsiConsole.MarkupLine($"\nYou brace for impact! Defense increased to [#03A1FC]{player.Defense}[/]");
                    CompleteTurn();
                    break;

                case "4": // Flee
                    AnsiConsole.MarkupLine("\nYou flee from combat!");
                    combatActive = false;
                    if (activeEnemies != null)
                        HandleCombatEnd(player, activeEnemies, combatRoom, combatActive);
                    currentState = CombatState.CombatEnded;
                    break;

                case "5": // Items
                    if (currentConsumables != null && currentConsumables.Count > 0)
                    {
                        ShowItemMenu();
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("\nInvalid choice! Please choose again.");
                        ShowPlayerActionMenu();
                    }
                    break;

                default:
                    AnsiConsole.MarkupLine("\nInvalid choice! Please choose again.");
                    ShowPlayerActionMenu();
                    break;
            }
        }

        private void CompleteTurn()
        {
            AnsiConsole.MarkupLine($"[dim]DEBUG: CompleteTurn called, currentTurnIndex={currentTurnIndex}[/]");
            currentTurnIndex++;
            currentState = CombatState.ProcessingTurn;
            AnsiConsole.MarkupLine($"[dim]DEBUG: About to call ProcessNextTurn directly (no delay), new index={currentTurnIndex}[/]");

            // TEMPORARY FIX: Call directly without delay to test if async is causing the freeze
            try
            {
                ProcessNextTurn();
                AnsiConsole.MarkupLine($"[dim]DEBUG: ProcessNextTurn completed, invoking onStateChanged[/]");
                onStateChanged?.Invoke();
                AnsiConsole.MarkupLine($"[dim]DEBUG: onStateChanged invoked[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error in CompleteTurn: {ex.Message}[/]");
                AnsiConsole.MarkupLine($"[dim]{ex.StackTrace}[/]");
                currentState = CombatState.CombatEnded;
                combatActive = false;
            }
        }

        private async Task ScheduleNextTurnAsync()
        {
            try
            {
                AnsiConsole.MarkupLine($"[dim]DEBUG: ScheduleNextTurnAsync executing, about to delay {TURN_DELAY_MS}ms, currentState={currentState}[/]");

                // Use ConfigureAwait(false) to avoid deadlocks
                await Task.Delay(TURN_DELAY_MS).ConfigureAwait(false);

                AnsiConsole.MarkupLine($"[dim]DEBUG: Delay complete, calling ProcessNextTurn, currentState={currentState}, combatActive={combatActive}[/]");
                ProcessNextTurn();
                AnsiConsole.MarkupLine($"[dim]DEBUG: ProcessNextTurn returned, about to invoke onStateChanged, currentState={currentState}[/]");

                if (onStateChanged != null)
                {
                    AnsiConsole.MarkupLine($"[dim]DEBUG: Invoking onStateChanged[/]");
                    onStateChanged.Invoke();
                    AnsiConsole.MarkupLine($"[dim]DEBUG: onStateChanged completed[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[dim]DEBUG: onStateChanged is null, skipping[/]");
                }
            }
            catch (Exception ex)
            {
                // Log the exception so it doesn't crash the browser
                AnsiConsole.MarkupLine($"[red]Error in combat turn processing: {ex.Message}[/]");
                AnsiConsole.MarkupLine($"[dim]{ex.StackTrace}[/]");
                // Reset combat state to prevent soft-lock
                currentState = CombatState.CombatEnded;
                combatActive = false;
                // Try to trigger UI update even after error
                onStateChanged?.Invoke();
            }
        }

        private void StartPlayerAttack()
        {
            if (activeEnemies == null)
                return;

            var aliveEnemies = activeEnemies.Where(e => e.Health > 0).ToList();

            if (aliveEnemies.Count == 1)
            {
                // Auto-target single enemy
                ExecutePlayerAttack(aliveEnemies[0]);
            }
            else
            {
                // Show target selection
                currentTargetList = aliveEnemies;
                currentState = CombatState.SelectingAttackTarget;

                AnsiConsole.MarkupLine("\nChoose target:");
                for (int i = 0; i < aliveEnemies.Count; i++)
                {
                    AnsiConsole.MarkupLine($"{i + 1}. {aliveEnemies[i].Name} (HP: {aliveEnemies[i].Health}/{aliveEnemies[i].MaxHealth})");
                }
                AnsiConsole.MarkupLine("\n[dim](Enter target number)[/]");
            }
        }

        private void HandleAttackTargetSelection(string input)
        {
            if (currentTargetList == null)
                return;

            if (int.TryParse(input, out int targetIndex) && targetIndex > 0 && targetIndex <= currentTargetList.Count)
            {
                ExecutePlayerAttack(currentTargetList[targetIndex - 1]);
            }
            else
            {
                AnsiConsole.MarkupLine("Invalid target! Please choose again.");
                StartPlayerAttack();
            }
        }

        private void ExecutePlayerAttack(NPC target)
        {
            var player = context.Player;

            int damageRoll = GetWeaponDamage(player);
            string diceString = GetWeaponDiceString(player);

            AnsiConsole.MarkupLine($"\nYou attack {target.Name} with your {player.EquippedWeapon}!");
            AnsiConsole.MarkupLine($"(Rolled {diceString} for [#FA8A8A]{damageRoll} damage[/]!)");

            target.Health -= damageRoll;

            if (target.Health <= 0)
            {
                AnsiConsole.MarkupLine($"\n[#90FF90]{target.Name} is defeated![/]");
            }

            CompleteTurn();
        }

        private void ShowAbilityMenu()
        {
            currentState = CombatState.SelectingAbility;
            var player = context.Player;

            AnsiConsole.MarkupLine("\n[#FFD700]== Your Abilities ==[/]");

            var abilities = player.Class.GetClassAbilities();
            if (abilities == null || abilities.Count == 0)
            {
                AnsiConsole.MarkupLine("\nYou have no abilities available.");
                AnsiConsole.MarkupLine("[dim](Press 0 to go back)[/]");
                return;
            }

            for (int i = 0; i < abilities.Count; i++)
            {
                var ability = abilities[i];
                AnsiConsole.MarkupLine($"{i + 1}. {ability.Name} (EP: {ability.EnergyCost})");
            }
            AnsiConsole.MarkupLine("0. Back");
            AnsiConsole.MarkupLine("\n[dim](Enter ability number)[/]");
        }

        private void HandleAbilitySelection(string input)
        {
            AnsiConsole.MarkupLine($"[dim]DEBUG: HandleAbilitySelection called with input='{input}'[/]");

            if (input == "0")
            {
                ShowPlayerActionMenu();
                return;
            }

            var player = context.Player;
            var abilities = player.Class.GetClassAbilities();

            if (abilities == null || abilities.Count == 0)
            {
                ShowPlayerActionMenu();
                return;
            }

            if (int.TryParse(input, out int abilityIndex) && abilityIndex > 0 && abilityIndex <= abilities.Count)
            {
                var ability = abilities[abilityIndex - 1];
                AnsiConsole.MarkupLine($"[dim]DEBUG: Selected ability '{ability.Name}', energyCost={ability.EnergyCost}[/]");

                // Check energy cost
                if (player.Energy < ability.EnergyCost)
                {
                    AnsiConsole.MarkupLine($"\nNot enough energy! You have {player.Energy} EP but need {ability.EnergyCost} EP.");
                    ShowAbilityMenu();
                    return;
                }

                // Store ability and character for target selection
                pendingAbility = ability;
                abilityCharacter = player;

                // Check if ability needs target selection
                AnsiConsole.MarkupLine($"[dim]DEBUG: Checking target requirements, NeedsEnemyTarget={NeedsEnemyTarget(ability)}[/]");
                if (activeEnemies != null && NeedsEnemyTarget(ability))
                {
                    var aliveEnemies = activeEnemies.Where(e => e.Health > 0).ToList();
                    AnsiConsole.MarkupLine($"[dim]DEBUG: aliveEnemies.Count={aliveEnemies.Count}[/]");

                    if (aliveEnemies.Count == 1)
                    {
                        AnsiConsole.MarkupLine($"[dim]DEBUG: Single enemy, auto-targeting[/]");
                        // Auto-target single enemy
                        ExecuteAbilityForCharacter(ability, player, activeEnemies, player);
                        CompleteTurn();
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[dim]DEBUG: Multiple enemies, showing target selection[/]");
                        // Show target selection
                        currentTargetList = aliveEnemies;
                        currentState = CombatState.SelectingAbilityTarget;
                        AnsiConsole.MarkupLine("\n[#90FF90]Choose target:[/]");
                        for (int i = 0; i < aliveEnemies.Count; i++)
                        {
                            AnsiConsole.MarkupLine($"{i + 1}. {aliveEnemies[i].Name} (HP: {aliveEnemies[i].Health}/{aliveEnemies[i].MaxHealth})");
                        }
                        ShowStatusBar();
                        AnsiConsole.MarkupLine("[dim](Enter target number)[/]");
                    }
                }
                else
                {
                    // No target needed, execute directly
                    if (activeEnemies != null)
                    {
                        ExecuteAbilityForCharacter(ability, player, activeEnemies, player);
                    }
                    CompleteTurn();
                }
            }
            else
            {
                AnsiConsole.MarkupLine("Invalid ability! Please choose again.");
                ShowAbilityMenu();
            }
        }

        private bool NeedsEnemyTarget(Ability ability)
        {
            // Area-of-effect abilities hit all enemies automatically, no target selection needed
            if (ability.Type == AbilityType.AreaOfEffect)
            {
                return false;
            }

            // Abilities that don't target enemies (buffs, heals, self-buffs)
            return ability.Name switch
            {
                "Shield Wall" => false,
                "Evasive Fire" => false,
                "Blessing" => false,
                "Heal" => false,
                "Battle Cry" => false,
                "Barrier" => false,
                _ => true  // Single-target abilities need target selection
            };
        }

        private void HandleAbilityTargetSelection(string input)
        {
            AnsiConsole.MarkupLine($"[dim]DEBUG: HandleAbilityTargetSelection called with input='{input}'[/]");

            if (currentTargetList == null || pendingAbility == null || abilityCharacter == null || activeEnemies == null)
            {
                AnsiConsole.MarkupLine("[#FF0000]Error: Invalid combat state![/]");
                ShowPlayerActionMenu();
                return;
            }

            if (!int.TryParse(input, out int targetIndex) || targetIndex < 1 || targetIndex > currentTargetList.Count)
            {
                AnsiConsole.MarkupLine("[#FF0000]Invalid target! Please choose again.[/]");

                // Re-show target selection
                AnsiConsole.MarkupLine("\n[#90FF90]Choose target:[/]");
                for (int i = 0; i < currentTargetList.Count; i++)
                {
                    var enemy = currentTargetList[i];
                    AnsiConsole.MarkupLine($"{i + 1}. {enemy.Name} (HP: {enemy.Health}/{enemy.MaxHealth})");
                }
                ShowStatusBar();
                AnsiConsole.MarkupLine("[dim](Enter target number)[/]");
                return;
            }

            AnsiConsole.MarkupLine($"[dim]DEBUG: Valid target selected, executing ability[/]");

            // Execute ability with selected target
            preselectedTarget = currentTargetList[targetIndex - 1];
            AnsiConsole.MarkupLine($"[dim]DEBUG: About to execute ability '{pendingAbility?.Name}' on {preselectedTarget?.Name}[/]");
            ExecuteAbilityForCharacter(pendingAbility, abilityCharacter, activeEnemies, context.Player, preselectedTarget);
            AnsiConsole.MarkupLine($"[dim]DEBUG: Ability executed, clearing state[/]");

            // Clear pending ability state
            pendingAbility = null;
            abilityCharacter = null;
            currentTargetList = null;
            preselectedTarget = null;

            AnsiConsole.MarkupLine($"[dim]DEBUG: Calling CompleteTurn[/]");
            CompleteTurn();
            AnsiConsole.MarkupLine($"[dim]DEBUG: CompleteTurn returned[/]");
        }

        private void ShowItemMenu()
        {
            currentState = CombatState.SelectingItem;

            if (currentConsumables == null || currentConsumables.Count == 0)
            {
                ShowPlayerActionMenu();
                return;
            }

            AnsiConsole.MarkupLine("\n[#90FF90]== Your Items ==[/]");
            for (int i = 0; i < currentConsumables.Count; i++)
            {
                AnsiConsole.MarkupLine($"{i + 1}. {TextHelper.CapitalizeFirst(currentConsumables[i])}");
            }
            AnsiConsole.MarkupLine("0. Back");
            AnsiConsole.MarkupLine("\n[dim](Enter item number)[/]");
        }

        private void HandleItemSelection(string input)
        {
            if (input == "0")
            {
                ShowPlayerActionMenu();
                return;
            }

            if (currentConsumables == null || currentConsumables.Count == 0)
            {
                ShowPlayerActionMenu();
                return;
            }

            if (int.TryParse(input, out int itemIndex) && itemIndex > 0 && itemIndex <= currentConsumables.Count)
            {
                var player = context.Player;
                var item = currentConsumables[itemIndex - 1];
                var itemDescriptions = context.ItemDescriptions;

                // Get item data and apply its effect
                var itemData = itemDescriptions.Values
                    .Where(room => room.ContainsKey(item))
                    .Select(room => room[item])
                    .FirstOrDefault();

                if (itemData != null && context.Effects.ContainsKey(itemData.EffectId))
                {
                    var effect = context.Effects[itemData.EffectId];

                    // Handle party-wide effects
                    if (effect.TargetsParty)
                    {
                        ApplyCombatPartyEffect(item, effect, player);
                    }
                    else
                    {
                        // Handle single-target effects
                        ApplyCombatSingleEffect(item, effect, player);
                    }

                    player.Inventory.Remove(item);
                    AnsiConsole.MarkupLine($"The {item} has been consumed.");
                }
                else
                {
                    // Fallback if item data not found
                    AnsiConsole.MarkupLine($"\nYou use {TextHelper.CapitalizeFirst(item)}!");
                    player.Inventory.Remove(item);
                }

                CompleteTurn();
            }
            else
            {
                AnsiConsole.MarkupLine("Invalid item! Please choose again.");
                ShowItemMenu();
            }
        }

        private bool HandleCharacterAbilities(Character character, List<NPC> enemies, Player player)
        {
            var abilities = character.Class?.GetClassAbilities() ?? new List<Ability>();

            if (abilities.Count == 0)
            {
                AnsiConsole.MarkupLine($"\n{character.Name} doesn't know any special abilities yet!");
                return false;
            }

            AnsiConsole.MarkupLine($"\n{character.Name}'s Abilities:");
            for (int i = 0; i < abilities.Count; i++)
            {
                var ability = abilities[i];
                string energyColor = character.Energy >= ability.EnergyCost ? "#FFFF00" : "#808080";
                AnsiConsole.MarkupLine($"{i + 1}. {ability.Name} ([{energyColor}]{ability.EnergyCost} EP[/]) - {ability.Description} (Current EP: [#00FFFF]{character.Energy}/{character.MaxEnergy}[/])");
            }
            AnsiConsole.MarkupLine("0. Back");

            Console.Write("Choose ability: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            string abilityChoice = Console.ReadLine();
            Console.ResetColor();

            if (abilityChoice == "0")
                return false;

            if (!int.TryParse(abilityChoice, out int index) || index < 1 || index > abilities.Count)
            {
                AnsiConsole.MarkupLine("Invalid choice!");
                return false;
            }

            var selectedAbility = abilities[index - 1];

            if (character.Energy < selectedAbility.EnergyCost)
            {
                AnsiConsole.MarkupLine($"\nNot enough energy! {character.Name} needs {selectedAbility.EnergyCost} EP.");
                return false;
            }

            return ExecuteAbilityForCharacter(selectedAbility, character, enemies, player);
        }

        private bool ExecuteWhirlwind(Player player, List<NPC> enemies)
        {
            if (player.Energy < AbilityData.WhirlwindAttack.EnergyCost)
            {
                AnsiConsole.MarkupLine($"\nNot enough energy! You need {AbilityData.WhirlwindAttack.EnergyCost} EP.");
                return false;
            }

            player.Energy -= AbilityData.WhirlwindAttack.EnergyCost;
            AnsiConsole.MarkupLine($"\n[#FF00FF]You spin with your weapon extended, striking all enemies![/]");

            var targetsHit = enemies.Where(e => e.Health > 0).ToList();
            if (targetsHit.Count == 0)
            {
                AnsiConsole.MarkupLine("No enemies to hit!");
                return true;
            }

            foreach (var enemy in targetsHit)
            {
                int aoeDamage = RollDice(AbilityData.WhirlwindAttack.DiceCount,
                                        AbilityData.WhirlwindAttack.DiceSides,
                                        AbilityData.WhirlwindAttack.Bonus);
                enemy.Health -= aoeDamage;
                AnsiConsole.MarkupLine($"You hit {enemy.Name}! (Rolled {AbilityData.WhirlwindAttack.DiceString} for [#FA8A8A]{aoeDamage} damage[/]!)");

                if (enemy.Health <= 0)
                {
                    AnsiConsole.MarkupLine($"[#90FF90]{enemy.Name} is defeated![/]");
                }
            }
            return true;
        }

        private bool ExecutePowerAttack(Player player, List<NPC> enemies)
        {
            if (player.Energy < AbilityData.PowerAttack.EnergyCost)
            {
                AnsiConsole.MarkupLine($"\nNot enough energy! You need {AbilityData.PowerAttack.EnergyCost} EP.");
                return false;
            }

            NPC powerTarget = null;
            var powerEnemies = enemies.Where(e => e.Health > 0).ToList();

            if (powerEnemies.Count == 0)
            {
                AnsiConsole.MarkupLine("No enemies to attack!");
                return false;
            }
            else if (powerEnemies.Count == 1)
            {
                powerTarget = powerEnemies[0];
            }
            else
            {
                AnsiConsole.MarkupLine("\nChoose target:");
                for (int i = 0; i < powerEnemies.Count; i++)
                {
                    AnsiConsole.MarkupLine($"{i + 1}. {powerEnemies[i].Name} (HP: {powerEnemies[i].Health}/{powerEnemies[i].MaxHealth})");
                }
                Console.Write("Target: ");

                Console.ForegroundColor = ConsoleColor.Cyan;
                string targetChoice = Console.ReadLine();
                Console.ResetColor();

                if (int.TryParse(targetChoice, out int targetIndex) && targetIndex > 0 && targetIndex <= powerEnemies.Count)
                {
                    powerTarget = powerEnemies[targetIndex - 1];
                }
                else
                {
                    AnsiConsole.MarkupLine("Invalid target!");
                    return false;
                }
            }

            player.Energy -= AbilityData.PowerAttack.EnergyCost;
            int powerDamage = RollDice(AbilityData.PowerAttack.DiceCount,
                                      AbilityData.PowerAttack.DiceSides,
                                      AbilityData.PowerAttack.Bonus);

            AnsiConsole.MarkupLine($"\n[#FF0000]You deliver a powerful blow to {powerTarget.Name}![/]");
            AnsiConsole.MarkupLine($"(Rolled {AbilityData.PowerAttack.DiceString} for [#FA8A8A]{powerDamage} damage[/]!)");
            powerTarget.Health -= powerDamage;

            if (powerTarget.Health <= 0)
            {
                AnsiConsole.MarkupLine($"[#90FF90]{powerTarget.Name} is defeated![/]");
            }
            return true;
        }

        private bool ExecuteBattleCry(Player player)
        {
            if (player.Energy < AbilityData.BattleCry.EnergyCost)
            {
                AnsiConsole.MarkupLine($"\nNot enough energy! You need {AbilityData.BattleCry.EnergyCost} EP.");
                return false;
            }

            player.Energy -= AbilityData.BattleCry.EnergyCost;
            AnsiConsole.MarkupLine($"\n[#FFFF00]You let out an inspiring battle cry! The party feels invigorated![/]");

            battleCryTurns[player] = 3;
            buffedAttack[player] = 2;
            buffedDefense[player] = 1;

            foreach (var ally in player.ActiveParty.Where(a => a.Health > 0))
            {
                battleCryTurns[ally] = 3;
                buffedAttack[ally] = 2;
                buffedDefense[ally] = 1;
            }

            AnsiConsole.MarkupLine($"[#00FFFF]Party gains +2 attack and +1 defense for 3 turns![/]");
            return true;
        }

        private bool HandleCombatItems(Player player, List<string> consumableItems)
        {
            if (consumableItems.Count == 0)
            {
                AnsiConsole.MarkupLine("\nNo usable items!");
                return false;
            }

            AnsiConsole.MarkupLine("\nItems:");
            for (int i = 0; i < consumableItems.Count; i++)
            {
                var itemData = context.ItemDescriptions.Values
                    .Where(room => room.ContainsKey(consumableItems[i]))
                    .Select(room => room[consumableItems[i]])
                    .FirstOrDefault();

                if (itemData != null && context.Effects.ContainsKey(itemData.EffectId))
                {
                    var effect = context.Effects[itemData.EffectId];
                    AnsiConsole.MarkupLine($"{i + 1}. {TextHelper.CapitalizeFirst(consumableItems[i])} - {effect.Description}");
                }
            }
            AnsiConsole.MarkupLine("0. Back");

            Console.Write("Use item: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            string itemChoice = Console.ReadLine();
            Console.ResetColor();

            if (itemChoice == "0")
            {
                return false;
            }

            if (int.TryParse(itemChoice, out int itemIndex) && itemIndex > 0 && itemIndex <= consumableItems.Count)
            {
                string usedItem = consumableItems[itemIndex - 1];

                var itemData = context.ItemDescriptions.Values
                    .Where(room => room.ContainsKey(usedItem))
                    .Select(room => room[usedItem])
                    .FirstOrDefault();

                if (itemData != null && context.Effects.ContainsKey(itemData.EffectId))
                {
                    var effect = context.Effects[itemData.EffectId];

                    // Handle party-wide effects
                    if (effect.TargetsParty)
                    {
                        ApplyCombatPartyEffect(usedItem, effect, player);
                    }
                    else
                    {
                        // Handle single-target effects
                        ApplyCombatSingleEffect(usedItem, effect, player);
                    }

                    player.Inventory.Remove(usedItem);
                    AnsiConsole.MarkupLine($"The {usedItem} has been consumed.");
                    return true;
                }
            }
            else
            {
                AnsiConsole.MarkupLine("Invalid choice!");
            }
            return false;
        }

        private void ApplyCombatSingleEffect(string itemName, Effect effect, Player player)
        {
            int rollAmount = RollDice(effect.DiceCount, effect.DiceSides, effect.Bonus);

            switch (effect.Type)
            {
                case EffectType.Heal:
                    int actualHeal = Math.Min(rollAmount, player.MaxHealth - player.Health);
                    player.Health += actualHeal;

                    AnsiConsole.MarkupLine($"\n[#00FF00]You use the {itemName}![/]");
                    AnsiConsole.MarkupLine($"(Rolled {effect.DiceCount}d{effect.DiceSides}+{effect.Bonus} for [#00FF00]{rollAmount} hit points[/]!)");
                    if (actualHeal < rollAmount)
                    {
                        AnsiConsole.MarkupLine($"[#808080](You were healed for {actualHeal} as you're near full health)[/]");
                    }
                    break;

                case EffectType.RestoreEnergy:
                    int actualEnergyRestore = Math.Min(rollAmount, player.MaxEnergy - player.Energy);
                    player.Energy += actualEnergyRestore;

                    AnsiConsole.MarkupLine($"\n[#0080FF]You use the {itemName}![/]");
                    AnsiConsole.MarkupLine($"(Rolled {effect.DiceCount}d{effect.DiceSides}+{effect.Bonus} for [#0080FF]{rollAmount} energy[/]!)");
                    if (actualEnergyRestore < rollAmount)
                    {
                        AnsiConsole.MarkupLine($"[#808080](You restored {actualEnergyRestore} energy as you're near full energy)[/]");
                    }
                    break;

                default:
                    AnsiConsole.MarkupLine($"\n[#FFFF00]You use the {itemName}, but nothing seems to happen.[/]");
                    break;
            }
        }

        private void ApplyCombatPartyEffect(string itemName, Effect effect, Player player)
        {
            int rollAmount = RollDice(effect.DiceCount, effect.DiceSides, effect.Bonus);

            AnsiConsole.MarkupLine($"\n[#FFD700]You use the {itemName} on the entire party![/]");
            AnsiConsole.MarkupLine($"(Rolled {effect.DiceCount}d{effect.DiceSides}+{effect.Bonus} for [#FFD700]{rollAmount} points[/]!)");

            switch (effect.Type)
            {
                case EffectType.PartyRestore:
                    // Apply to player
                    int playerHealthGain = Math.Min(rollAmount, player.MaxHealth - player.Health);
                    int playerEnergyGain = Math.Min(rollAmount, player.MaxEnergy - player.Energy);
                    player.Health += playerHealthGain;
                    player.Energy += playerEnergyGain;

                    AnsiConsole.MarkupLine($"[#00FF00]You restore {playerHealthGain} health and {playerEnergyGain} energy![/]");

                    // Apply to party members
                    foreach (var ally in player.ActiveParty)
                    {
                        int allyHealthGain = Math.Min(rollAmount, ally.MaxHealth - ally.Health);
                        int allyEnergyGain = Math.Min(rollAmount, ally.MaxEnergy - ally.Energy);
                        ally.Health += allyHealthGain;
                        ally.Energy += allyEnergyGain;

                        AnsiConsole.MarkupLine($"[#00FF00]{ally.Name} restores {allyHealthGain} health and {allyEnergyGain} energy![/]");
                    }
                    break;

                default:
                    AnsiConsole.MarkupLine($"[#FFFF00]The {itemName} has no effect in combat.[/]");
                    break;
            }
        }

        private void HandlePartyMemberTurn(Recruit ally, List<NPC> enemies)
        {
            // Process DOT damage at the start of ally turn
            if (ally.ActiveDOTs != null && ally.ActiveDOTs.Count > 0)
            {
                // Get the first DOT type BEFORE processing
                DamageType firstDotType = ally.ActiveDOTs[0].Type;

                int dotDamage = ally.ProcessDOTs();
                if (dotDamage > 0)
                {
                    ally.Health -= dotDamage;

                    string dotColor = GetDOTColor(firstDotType);
                    string dotTypeName = GetDOTTypeName(firstDotType);
                    AnsiConsole.MarkupLine($"\n[{dotColor}]{ally.Name} takes {dotDamage} damage from {dotTypeName}![/]");

                    if (ally.Health <= 0)
                    {
                        AnsiConsole.MarkupLine($"[#FF0000]{ally.Name} has been defeated![/]");
                        return;
                    }
                }
            }

            // Check if stunned
            if (HasStatusEffect(ally, StatusEffect.Stunned))
            {
                AnsiConsole.MarkupLine($"\n{ally.Name} is stunned and loses their turn!");
                return;
            }

            var aliveEnemies = enemies.Where(e => e.Health > 0).ToList();

            if (aliveEnemies.Count > 0)
            {
                bool validAction = false;
                while (!validAction)
                {
                    AnsiConsole.MarkupLine($"\n[{ally.Name}'s turn]");
                    AnsiConsole.MarkupLine("1. Attack");
                    AnsiConsole.MarkupLine("2. Abilities");
                    AnsiConsole.MarkupLine("3. Defend");

                    Console.Write("Choose action: ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    string choice = Console.ReadLine();
                    Console.ResetColor();

                    switch (choice)
                    {
                        case "1": // Attack
                            NPC target = null;

                            if (aliveEnemies.Count == 1)
                            {
                                target = aliveEnemies[0];
                            }
                            else
                            {
                                AnsiConsole.MarkupLine("\nChoose target:");
                                for (int i = 0; i < aliveEnemies.Count; i++)
                                {
                                    AnsiConsole.MarkupLine($"{i + 1}. {aliveEnemies[i].Name} (HP: {aliveEnemies[i].Health}/{aliveEnemies[i].MaxHealth})");
                                }
                                Console.Write("Target: ");

                                Console.ForegroundColor = ConsoleColor.Cyan;
                                string targetChoice = Console.ReadLine();
                                Console.ResetColor();

                                if (int.TryParse(targetChoice, out int targetIndex) && targetIndex > 0 && targetIndex <= aliveEnemies.Count)
                                {
                                    target = aliveEnemies[targetIndex - 1];
                                }
                                else
                                {
                                    AnsiConsole.MarkupLine("Invalid target!");
                                    break;
                                }
                            }

                            // Get weapon damage for recruit
                            int damage = GetWeaponDamage(ally);
                            string diceString = GetWeaponDiceString(ally);

                            AnsiConsole.MarkupLine($"\n{ally.Name} attacks {target.Name}!");
                            AnsiConsole.MarkupLine($"(Rolled {diceString} for [#FA8A8A]{damage} damage[/]!)");
                            target.Health -= damage;

                            if (target.Health <= 0)
                            {
                                AnsiConsole.MarkupLine($"[#90FF90]{target.Name} is defeated![/]");
                            }
                            validAction = true;
                            break;

                        case "2": // Abilities
                            validAction = HandleCharacterAbilities(ally, aliveEnemies, context.Player);
                            break;

                        case "3": // Defend
                            AnsiConsole.MarkupLine($"\n{ally.Name} takes a defensive stance!");
                            ally.Defense = Math.Max(1, ally.Defense * 2);
                            validAction = true;
                            break;

                        default:
                            AnsiConsole.MarkupLine("\nInvalid choice! Please choose again.");
                            break;
                    }
                }
                // Note: Thread.Sleep removed for web compatibility
            }
        }

        private void HandleEnemyTurn(NPC attackingEnemy, Player player)
        {
            if (attackingEnemy != null && attackingEnemy.Health > 0)
            {
                // Process DOT damage at the start of enemy turn
                // Process DOT damage at the start of enemy turn
                if (attackingEnemy.ActiveDOTs != null && attackingEnemy.ActiveDOTs.Count > 0)
                {
                    // Get the first DOT type BEFORE processing (in case they all expire)
                    DamageType firstDotType = attackingEnemy.ActiveDOTs[0].Type;

                    int dotDamage = attackingEnemy.ProcessDOTs();
                    if (dotDamage > 0)
                    {
                        attackingEnemy.Health -= dotDamage;

                        // Display DOT damage with appropriate color
                        string dotColor = GetDOTColor(firstDotType);
                        string dotTypeName = GetDOTTypeName(firstDotType);
                        AnsiConsole.MarkupLine($"[{dotColor}]{attackingEnemy.Name} takes {dotDamage} damage from {dotTypeName}![/]");

                        if (attackingEnemy.Health <= 0)
                        {
                            AnsiConsole.MarkupLine($"[#90FF90]{attackingEnemy.Name} succumbs to the damage![/]");
                            return;
                        }
                    }
                }

                // Check if stunned
                if (HasStatusEffect(attackingEnemy, StatusEffect.Stunned))
                {
                    AnsiConsole.MarkupLine($"\n{attackingEnemy.Name} is stunned and loses their turn!");
                    return;
                }

                List<Character> possibleTargets = new List<Character>();
                if (player.Health > 0) possibleTargets.Add(player);
                possibleTargets.AddRange(player.ActiveParty.Where(a => a.Health > 0));

                if (possibleTargets.Count > 0)
                {
                    Character target;

                    // Check for taunt effects - if this enemy is taunted, must attack taunter
                    if (taunters.ContainsKey(attackingEnemy) && taunters[attackingEnemy].Health > 0)
                    {
                        target = taunters[attackingEnemy];
                        AnsiConsole.MarkupLine($"\n{attackingEnemy.Name} is forced to attack {(target == player ? "you" : target.Name)}!");
                    }
                    else
                    {
                        target = possibleTargets[random.Next(possibleTargets.Count)];
                    }

                    int enemyDamage = attackingEnemy.AttackDamage;
                    string targetName = target == player ? "you" : target.Name;
                    AnsiConsole.MarkupLine($"\n{attackingEnemy.Name} attacks {targetName}!");

                    // Check for Evasive Fire
                    if (target == player && evasiveFireActive.ContainsKey(player) && evasiveFireActive[player])
                    {
                        AnsiConsole.MarkupLine($"[#90FF90]You dodge the attack and fire back![/]");

                        var evasiveFireAbility = AbilityData.EvasiveFire;
                        int counterDamage = RollDice(evasiveFireAbility.DiceCount, evasiveFireAbility.DiceSides, evasiveFireAbility.Bonus);
                        attackingEnemy.Health -= counterDamage;
                        AnsiConsole.MarkupLine($"Your counter-attack hits for [#FA8A8A]{counterDamage} damage[/]!");

                        if (attackingEnemy.Health <= 0)
                        {
                            AnsiConsole.MarkupLine($"[#90FF90]{attackingEnemy.Name} is defeated by your counter-attack![/]");
                        }

                        evasiveFireActive[player] = false;
                        return;
                    }

                    // Calculate base damage
                    int actualDamage = Math.Max(1, enemyDamage - target.Defense);
                    AnsiConsole.MarkupLine($"(Attack: {enemyDamage} - Defense: {target.Defense} = [#FA8A8A]{actualDamage} potential damage[/]!)");

                    // Check for barrier absorption
                    if (barrierAbsorption.ContainsKey(target) && barrierAbsorption[target] > 0)
                    {
                        int absorbed = Math.Min(actualDamage, barrierAbsorption[target]);
                        int remaining = actualDamage - absorbed;

                        AnsiConsole.MarkupLine($"[#75C8FF]The magical barrier absorbs {absorbed} damage![/]");

                        // Heal for 10% of absorbed damage
                        int healAmount = Math.Max(1, absorbed / 10);
                        target.Health = Math.Min(target.MaxHealth, target.Health + healAmount);
                        AnsiConsole.MarkupLine($"[#90FF90]You are healed for {healAmount} health from the absorbed energy![/]");

                        // Reduce barrier
                        barrierAbsorption[target] -= absorbed;
                        if (barrierAbsorption[target] <= 0)
                        {
                            barrierAbsorption.Remove(target);
                            AnsiConsole.MarkupLine($"[#808080]The barrier shatters![/]");
                        }

                        // Apply remaining damage
                        if (remaining > 0)
                        {
                            target.TakeDamage(remaining);
                            AnsiConsole.MarkupLine($"[#FA8A8A]{remaining} damage gets through![/]");
                        }
                    }
                    else
                    {
                        // Normal damage
                        target.TakeDamage(actualDamage);
                    }

                    // Note: Thread.Sleep removed for web compatibility
                }
            }
        }

        private void HandleCombatEnd(Player player, List<NPC> enemies, Room? currentRoom, bool combatActive)
        {
            if (!player.IsAlive)
            {
                ShowDeathMenu();
            }
            else if (!enemies.Any(e => e.Health > 0))
            {
                HandleVictory(player, enemies, currentRoom);
            }
            else
            {
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"[#FCFC7F]╔══════════════════╗[/]");
                AnsiConsole.MarkupLine($"[#FCFC7F]║    FLED COMBAT   ║[/]");
                AnsiConsole.MarkupLine($"[#FCFC7F]╚══════════════════╝[/]");
                CleanupCombat(player);
            }
        }

        private void ShowDeathMenu()
        {
            currentState = CombatState.DeathMenu;

            Console.ForegroundColor = ConsoleColor.Red;
            AnsiConsole.MarkupLine("\n**** YOU DIED ****");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("    ╔═══════════════════════════════════════════════════════════════════════════════════╗");
            AnsiConsole.MarkupLine("    ║      ██████╗  █████╗ ███╗   ███╗███████╗    ██████╗ ██╗   ██╗███████╗██████╗      ║");
            AnsiConsole.MarkupLine("    ║      ██╔════╝ ██╔══██╗████╗ ████║██╔════╝  ██╔═══██╗██║   ██║██╔════╝██╔══██╗     ║");
            AnsiConsole.MarkupLine("    ║      ██║  ███╗███████║██╔████╔██║█████╗    ██║   ██║██║   ██║█████╗  ██████╔╝     ║");
            AnsiConsole.MarkupLine("    ║      ██║   ██║██╔══██║██║╚██╔╝██║██╔══╝    ██║   ██║╚██╗ ██╔╝██╔══╝  ██╔══██╗     ║");
            AnsiConsole.MarkupLine("    ║      ╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗  ╚██████╔╝ ╚████╔╝ ███████╗██║  ██║     ║");
            AnsiConsole.MarkupLine("    ║       ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝   ╚═════╝   ╚═══╝  ╚══════╝╚═╝  ╚═╝     ║");
            AnsiConsole.MarkupLine("    ╚═══════════════════════════════════════════════════════════════════════════════════╝");
            AnsiConsole.MarkupLine("");
            Console.ResetColor();

            AnsiConsole.MarkupLine("\n[#FFD700]What would you like to do?[/]");
            AnsiConsole.MarkupLine("1. Load a saved game");
            AnsiConsole.MarkupLine("2. Start a new game");
            AnsiConsole.MarkupLine("3. Quit");
            AnsiConsole.MarkupLine("\n[dim](Enter choice number)[/]");
        }

        private void HandleDeathMenuSelection(string input)
        {
            var player = context.Player;

            switch (input)
            {
                case "1":
                    // NOTE: Death screen load is for console app only - not used in web version
                    // Entire case 1 is commented out - web version doesn't have death screen reload
                    ShowDeathMenu(); // Just show menu again
                    break;

                case "2":
                    // Start new game - use callback to signal main menu
                    player.Health = player.MaxHealth;
                    player.Energy = player.MaxEnergy;
                    currentState = CombatState.CombatEnded;
                    onPlayerDeath?.Invoke();  // This will go to ShowMainMenu
                    break;

                case "3":
                    // Quit game
                    AnsiConsole.MarkupLine("\nThanks for playing!");
                    Environment.Exit(0);
                    break;

                default:
                    AnsiConsole.MarkupLine("[red]Invalid choice. Please choose 1, 2, or 3.[/]");
                    ShowDeathMenu();
                    break;
            }
        }

        private void CleanupCombat(Player player)
        {
            // Clear DOT effects after combat
            if (player.ActiveDOTs != null)
                player.ActiveDOTs.Clear();

            foreach (var recruit in player.ActiveParty)
            {
                if (recruit.ActiveDOTs != null)
                    recruit.ActiveDOTs.Clear();
            }

            ClearCombatStatusEffects(player);
            currentState = CombatState.CombatEnded;
        }

        private void HandleVictory(Player player, List<NPC> enemies, Room? currentRoom)
        {
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("<span class='victory-glow' style='font-size: 1.3em;'>⚔ ✦ VICTORY ✦ ⚔</span>");
            AnsiConsole.MarkupLine($"\n<span class='victory-text'>You defeated all enemies!</span>");

            ClearCombatStatusEffects(player);

            // Check for any recruitable NPCs
            recruitableNPCs = enemies.Where(e => e.RecruitableAfterDefeat).ToList();
            currentRecruitIndex = 0;

            if (recruitableNPCs.Count > 0)
            {
                ShowRecruitmentPrompt();
            }
            else
            {
                FinishVictory(player, enemies, currentRoom);
            }
        }

        private void ShowRecruitmentPrompt()
        {
            AnsiConsole.MarkupLine("[dim]DEBUG: ShowRecruitmentPrompt called[/]");
            if (recruitableNPCs == null || currentRecruitIndex >= recruitableNPCs.Count)
            {
                AnsiConsole.MarkupLine("[dim]DEBUG: No recruitable NPCs, finishing victory[/]");
                var player = context.Player;
                FinishVictory(player, activeEnemies?.ToList() ?? new List<NPC>(), combatRoom);
                return;
            }

            currentState = CombatState.RecruitmentPrompt;
            AnsiConsole.MarkupLine($"[dim]DEBUG: Set currentState to RecruitmentPrompt, IsInCombat={IsInCombat}[/]");
            var npc = recruitableNPCs[currentRecruitIndex];

            AnsiConsole.MarkupLine($"\n{npc.Name} yields, breathing heavily.");
            AnsiConsole.MarkupLine($"\"{npc.YieldDialogue}\"");
            AnsiConsole.MarkupLine($"\n1. Welcome to the guild, {npc.Name}.");
            AnsiConsole.MarkupLine("\n[dim](Enter 1 to recruit)[/]");
        }

        private void HandleRecruitmentSelection(string input)
        {
            if (recruitableNPCs == null || currentRecruitIndex >= recruitableNPCs.Count)
                return;

            var player = context.Player;
            var npc = recruitableNPCs[currentRecruitIndex];

            if (input == "1")
            {
                Recruit newRecruit = new Recruit(npc.Name, npc.RecruitClass, player.CurrentDay);
                player.Recruits.Add(newRecruit);

                // AUTO-JOIN PARTY for first 2 recruits
                if (player.Recruits.Count <= 2 && player.ActiveParty.Count < 3)
                {
                    player.ActiveParty.Add(newRecruit);
                    AnsiConsole.MarkupLine($"\"{npc.AcceptDialogue}\"");
                    AnsiConsole.MarkupLine($"\n[cyan]{npc.Name} has joined your party![/]");

                    // Show party tutorial on FIRST recruit
                    if (player.Recruits.Count == 1)
                    {
                        ProgramStatics.messageManager.CheckAndShowMessage("first_party_member", npc.Name);
                    }
                }
                else
                {
                    AnsiConsole.MarkupLine($"\"{npc.AcceptDialogue}\"");
                    AnsiConsole.MarkupLine($"\n[cyan]{npc.Name} has joined your guild![/]");
                    AnsiConsole.MarkupLine($"[{npc.Name} heads to the guild hall.]");

                    // Show guild tutorial on THIRD recruit (party is now full)
                    if (player.Recruits.Count == 3)
                    {
                        ProgramStatics.messageManager.CheckAndShowMessage("guild_management_unlocked");
                    }
                }

                // Move to next recruit or finish victory
                currentRecruitIndex++;
                if (currentRecruitIndex < recruitableNPCs.Count)
                {
                    ShowRecruitmentPrompt();
                }
                else
                {
                    FinishVictory(player, activeEnemies?.ToList() ?? new List<NPC>(), combatRoom);
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[#FF0000]Invalid choice. Enter '1' to continue.[/]");
                ShowRecruitmentPrompt();
            }
        }

        private void FinishVictory(Player player, List<NPC> enemies, Room? currentRoom)
        {

            // Loot from all enemies
            int totalGold = 0;
            foreach (var enemy in enemies)
            {
                int goldDrop = random.Next(enemy.MinGold, enemy.MaxGold + 1);
                totalGold += goldDrop;
                currentRoom.NPCs.Remove(enemy);
            }
            player.Gold += totalGold;
            AnsiConsole.MarkupLine($"\nYou looted [#FCBA03]{totalGold} gold pieces[/] total!");

            // Check for item drops
            List<string> droppedItems = new List<string>();
            foreach (var enemy in enemies)
            {
                foreach (var kvp in enemy.LootTable)
                {
                    if (random.Next(100) < kvp.Value)
                    {
                        droppedItems.Add(kvp.Key);
                        player.Inventory.Add(kvp.Key);
                    }
                }
            }

            if (droppedItems.Count > 0)
            {
                AnsiConsole.MarkupLine("\nItems found:");
                foreach (string item in droppedItems)
                {
                    AnsiConsole.MarkupLine($"[#00FFFF]+ {TextHelper.CapitalizeFirst(item)}[/]");
                }
            }
            if (!ProgramStatics.messageManager.GetShownMessages().Contains("first_combat_victory"))
            {
                ProgramStatics.messageManager.CheckAndShowMessage("first_combat_victory");
            }

            // Award XP to all party members who participated
            int totalXP = enemies.Sum(e => e.ExperienceReward);

            player.Experience += totalXP;
            AnsiConsole.MarkupLine($"\nYou gained [#00FFFF]{totalXP} experience[/]!");

            foreach (var ally in player.ActiveParty)
            {
                ally.Experience += totalXP;
                AnsiConsole.MarkupLine($"{ally.Name} gained [#00FFFF]{totalXP} experience[/]!");
            }

            // Check for level ups
            if (player.CheckLevelUp())
            {
                AnsiConsole.MarkupLine($"\n[#FFD700]★ LEVEL UP! You are now level {player.Level}! ★[/]");
                player.ApplyLevelUpBonuses();
                DisplayLevelUpStats(player);
            }

            foreach (var ally in player.ActiveParty)
            {
                if (ally.CheckLevelUp())
                {
                    AnsiConsole.MarkupLine($"\n[#FFD700]★ LEVEL UP! {ally.Name} is now level {ally.Level}! ★[/]");
                    ally.ApplyLevelUpBonuses();
                    DisplayLevelUpStats(ally);
                }
            }

            if (currentRoom != null)
            {
                currentRoom.MarkCleared(player.CurrentDay, player.CurrentHour);
            }

            CleanupCombat(player);
        }

        private List<Combatant> RollInitiative(Player player, List<NPC> enemies)
        {
            List<Combatant> combatants = new List<Combatant>();

            AnsiConsole.MarkupLine($"\n[#FCFC7F]Rolling for initiative! (1d20 + Speed)[/]");
            // Note: Thread.Sleep removed for web compatibility

            // Roll for player
            int playerRoll = RollDice(1, 20);
            int playerInit = playerRoll + player.Speed;
            AnsiConsole.MarkupLine($"You roll: {playerRoll} + {player.Speed} = [#00FFFF]{playerInit}[/]");
            combatants.Add(new Combatant(player, playerInit, true));

            // Roll for party members
            foreach (var ally in player.ActiveParty)  // Should be using the player parameter, not a static reference
            {
                if (ally.Health > 0)
                {
                    int allyRoll = RollDice(1, 20);
                    int allyInit = allyRoll + ally.Speed;
                    AnsiConsole.MarkupLine($"{ally.Name} rolls: {allyRoll} + {ally.Speed} = [#00FFFF]{allyInit}[/]");
                    combatants.Add(new Combatant(ally, allyInit, true));
                }
            }

            // Roll for each enemy
            foreach (var enemy in enemies)
            {
                int enemyRoll = RollDice(1, 20);
                int enemyInit = enemyRoll + enemy.Speed;
                AnsiConsole.MarkupLine($"{enemy.Name} rolls: {enemyRoll} + {enemy.Speed} = [#FA8A8A]{enemyInit}[/]");
                combatants.Add(new Combatant(enemy, enemyInit, false));
            }

            // Sort by initiative (highest first)
            combatants.Sort((a, b) => b.InitiativeRoll.CompareTo(a.InitiativeRoll));

            // Handle ties
            for (int i = 0; i < combatants.Count - 1; i++)
            {
                if (combatants[i].InitiativeRoll == combatants[i + 1].InitiativeRoll)
                {
                    if (combatants[i].Character.Speed != combatants[i + 1].Character.Speed)
                    {
                        continue;
                    }
                    if (random.Next(2) == 0)
                    {
                        var temp = combatants[i];
                        combatants[i] = combatants[i + 1];
                        combatants[i + 1] = temp;
                    }
                }
            }

            return combatants;
        }

        private string GetWeaponDiceString(Character character)
        {
            if (character.EquippedWeapon != null)
            {
                return character.EquippedWeapon.DiceString;
            }
            return "1d4+1"; // Fallback for unarmed
        }

        private int GetWeaponDamage(Character character)
        {
            if (character.EquippedWeapon != null)
            {
                return RollDice(character.EquippedWeapon.DiceCount,
                               character.EquippedWeapon.DiceSides,
                               character.EquippedWeapon.Bonus);
            }
            return RollDice(1, 4, 1); // Fallback for unarmed
        }

        private int RollDice(int count, int sides, int modifier = 0)
        {
            int total = modifier;
            for (int i = 0; i < count; i++)
            {
                total += random.Next(1, sides + 1);
            }
            return total;
        }

        private string GenerateHealthBar(int current, int max)
        {
            if (max <= 0) return "[#808080][DEAD][/]     ";

            float percentage = (float)current / max;
            int filledSegments = (int)Math.Round(percentage * 10);
            int emptySegments = 10 - filledSegments;

            string color;
            if (percentage >= 0.6f)
                color = "#90FF90";
            else if (percentage >= 0.3f)
                color = "#FCFC7F";
            else if (percentage > 0)
                color = "#FF9999";
            else
                return "[#808080][DEAD][/]     ";

            // Build the bar
            string bar = $"[{color}]";
            bar += new string('█', filledSegments);
            bar += "[/]";
            bar += $"[#404040]";
            bar += new string('░', emptySegments);
            bar += "[/]";

            return bar;
        }

        private void DisplayCombatStatus(Player player, List<NPC> enemies, Combatant currentTurn)
        {
            AnsiConsole.MarkupLine("");

            // Get status effects for current turn character
            string statusEffects = GetFormattedStatusEffects(currentTurn.Character);
            string turnHeader = $"{currentTurn.Name}'s Turn{statusEffects}";

            AnsiConsole.MarkupLine($"═══════════════════════════════════════════════════════════════════════");
            AnsiConsole.MarkupLine($"                        {turnHeader}");
            AnsiConsole.MarkupLine($"═══════════════════════════════════════════════════════════════════════");

            // Your Party section
            AnsiConsole.MarkupLine($" [#75C8FF]YOUR PARTY[/]");

            // Player
            string playerHealthBar = GenerateHealthBar(player.Health, player.MaxHealth);
            string playerEnergyBar = GenerateEnergyBar(player.Energy, player.MaxEnergy);
            string playerHP = $"{player.Health}/{player.MaxHealth}".PadLeft(9);  // "9999/9999" = 9 chars max
            string playerEP = $"{player.Energy}/{player.MaxEnergy}".PadLeft(9);
            AnsiConsole.MarkupLine($" {"You",-12} HP:{playerHealthBar} {playerHP}  EP:{playerEnergyBar} {playerEP}");

            // Party members
            foreach (var ally in player.ActiveParty.Where(a => a.Health > 0))
            {
                string allyHealthBar = GenerateHealthBar(ally.Health, ally.MaxHealth);
                string allyEnergyBar = GenerateEnergyBar(ally.Energy, ally.MaxEnergy);
                string allyName = ally.Name.Length > 12 ? ally.Name.Substring(0, 12) : ally.Name;
                string allyHP = $"{ally.Health}/{ally.MaxHealth}".PadLeft(9);
                string allyEP = $"{ally.Energy}/{ally.MaxEnergy}".PadLeft(9);
                AnsiConsole.MarkupLine($" {allyName,-12} HP:{allyHealthBar} {allyHP}  EP:{allyEnergyBar} {allyEP}");
            }

            // Spacing
            if (enemies.Any(e => e.Health > 0))
            {
                AnsiConsole.MarkupLine("");

                // Enemies section (they don't use energy, so no EP bars)
                AnsiConsole.MarkupLine($" [#FA8A8A]ENEMIES[/]");

                foreach (var enemy in enemies.Where(e => e.Health > 0))
                {
                    string enemyBar = GenerateHealthBar(enemy.Health, enemy.MaxHealth);
                    string enemyName = enemy.Name.Length > 12 ? enemy.Name.Substring(0, 12) : enemy.Name;
                    string enemyHP = $"{enemy.Health}/{enemy.MaxHealth}".PadLeft(9);
                    AnsiConsole.MarkupLine($" {enemyName,-12} HP:{enemyBar} {enemyHP}");
                }
            }

            AnsiConsole.MarkupLine($"═══════════════════════════════════════════════════════════════════════");
        }

        private void DisplayStatusEffect(string effectType, string targetName, bool isTargetPlayer, string baseMessage, string[] gradientColors)
        {
            // Get emoji and CSS class for the effect type
            string emoji = effectType switch
            {
                "BLEEDING" => "🩸",
                "POISONED" => "☠️",
                "BURNING" => "🔥",
                "STUNNED" => "⚡",
                "FROZEN" => "❄️",
                _ => "💥"
            };

            // Get the appropriate CSS class for each effect type
            string cssClass = effectType switch
            {
                "FROZEN" => "ice-damage",
                "POISONED" => "poison-damage",
                "BURNING" => "fire-damage",
                "STUNNED" => "lightning-damage",
                "BLEEDING" => "bleed-damage",
                _ => "combat-glow"
            };

            // Color the target name based on friendly/enemy
            string nameColor = isTargetPlayer ? "#07f79b" : "#f76457";
            string coloredName = $"[{nameColor}]{targetName}[/]";

            // Build the full message with colored name
            string fullMessage = baseMessage.Replace("{TARGET}", coloredName);

            // Single line with CSS class effect - smaller text
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"<span class='{cssClass}' style='font-size: 0.9em;'>{emoji} {fullMessage}</span>");
            AnsiConsole.MarkupLine("");
            // Note: Thread.Sleep removed for web compatibility - Blazor WASM runs on UI thread
        }

        private string GenerateEnergyBar(int current, int max)
        {
            if (max <= 0) return "[#404040][EMPTY][/]     ";

            float percentage = (float)current / max;
            int filledSegments = (int)Math.Round(percentage * 10);
            int emptySegments = 10 - filledSegments;

            string color;
            if (percentage >= 0.6f)
                color = "#B388FF"; // Darker pastel purple  
            else if (percentage >= 0.3f)
                color = "#9575CD"; // Medium purple
            else if (percentage > 0)
                color = "#7E57C2"; // Deep purple
            else
                return "[#404040][EMPTY][/]     ";

            // Build the bar
            string bar = $"[{color}]";
            bar += new string('█', filledSegments);
            bar += "[/]";
            bar += $"[#404040]";
            bar += new string('░', emptySegments);
            bar += "[/]";

            return bar;
        }

        private bool ExecuteAbility(Ability ability, Player player, List<NPC> enemies)
        {
            player.Energy -= ability.EnergyCost;

            switch (ability.Name)
            {
                // Legionnaire Abilities
                case "Shield Bash":
                    return ExecuteShieldBashGeneric(ability, player, enemies);
                case "Taunt":
                    return ExecuteTauntGeneric(ability, player, enemies);
                case "Shield Wall":
                    return ExecuteShieldWallGeneric(ability, player, player);
                case "Cleave":
                    return ExecuteCleaveGeneric(ability, player, enemies);

                // Venator Abilities  
                case "Multi-Shot":
                    return ExecuteMultiShotGeneric(ability, player, enemies);
                case "Piercing Arrow":
                    return ExecutePiercingArrowGeneric(ability, player, enemies);
                case "Evasive Fire":
                    return ExecuteEvasiveFireGeneric(ability, player);
                case "Covering Shot":
                    return ExecuteCoveringShotGeneric(ability, player, enemies);

                // Oracle Abilities
                case "Heal":
                    return ExecuteHealGeneric(ability, player, player);
                case "Lightning Bolt":
                    return ExecuteLightningBoltGeneric(ability, player, enemies);
                case "Blessing":
                    return ExecuteBlessingGeneric(ability, player, player);
                case "Flame Strike":
                    return ExecuteFlameStrikeGeneric(ability, player, enemies);

                // Legacy abilities (if you still have these)
                case "Whirlwind Attack":
                    return ExecuteWhirlwind(player, enemies);
                case "Power Attack":
                    return ExecutePowerAttack(player, enemies);
                case "Battle Cry":
                    return ExecuteBattleCry(player);

                default:
                    AnsiConsole.MarkupLine($"[#FF0000]Ability '{ability.Name}' not yet implemented![/]");
                    return false;
            }
        }

        private bool ExecuteAbilityForCharacter(Ability ability, Character character, List<NPC> enemies, Player player, NPC preselectedTarget = null)
        {
            character.Energy -= ability.EnergyCost;

            switch (ability.Name)
            {
                // Legionnaire Abilities
                case "Shield Bash":
                    return ExecuteShieldBashGeneric(ability, character, enemies);
                case "Taunt":
                    return ExecuteTauntGeneric(ability, character, enemies);
                case "Shield Wall":
                    return ExecuteShieldWallGeneric(ability, character, player);
                case "Cleave":
                    return ExecuteCleaveGeneric(ability, character, enemies);
                case "Rending Strike":  
                    return ExecuteRendingStrikeGeneric(ability, character, enemies);

                // Venator Abilities  
                case "Multi-Shot":
                    return ExecuteMultiShotGeneric(ability, character, enemies);
                case "Piercing Arrow":
                    return ExecutePiercingArrowGeneric(ability, character, enemies);
                case "Evasive Fire":
                    return ExecuteEvasiveFireGeneric(ability, character);
                case "Covering Shot":
                    return ExecuteCoveringShotGeneric(ability, character, enemies);

                // Oracle Abilities
                case "Heal":
                    return ExecuteHealGeneric(ability, character, player);
                case "Lightning Bolt":
                    return ExecuteLightningBoltGeneric(ability, character, enemies);
                case "Blessing":
                    return ExecuteBlessingGeneric(ability, character, player);
                case "Flame Strike":
                    return ExecuteFlameStrikeGeneric(ability, character, enemies);

                // Level 5 Abilities
                case "Barbed Arrow":
                    return ExecuteBarbedArrowGeneric(ability, character, enemies);
                case "Frostbolt":
                    return ExecuteFrostboltGeneric(ability, character, enemies);

                // Level 10 Abilities
                case "Sunder Armor":
                    return ExecuteSunderArmorGeneric(ability, character, enemies);
                case "Frost Arrow":
                    return ExecuteFrostArrowGeneric(ability, character, enemies);
                case "Venom":
                    return ExecuteVenomGeneric(ability, character, enemies);

                // Level 15 Abilities
                case "Devastating Slam":
                    return ExecuteDevastingSlamGeneric(ability, character, enemies);
                case "Thunder Volley":
                    return ExecuteThunderVolleyGeneric(ability, character, enemies);
                case "Divine Wrath":
                    return ExecuteDivineWrathGeneric(ability, character, enemies);

                default:
                    AnsiConsole.MarkupLine($"[#FF0000]Ability '{ability.Name}' not yet implemented for {character.Name}![/]");
                    return false;
            }
        }


        private bool HealTarget(Ability ability, Character caster, Character target, string targetName)
        {
            int healAmount = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);
            int actualHeal = Math.Min(healAmount, target.MaxHealth - target.Health);

            target.Health += actualHeal;

            AnsiConsole.MarkupLine($"\n[#90FF90]You cast healing magic on {targetName.ToLower()}![/]");
            AnsiConsole.MarkupLine($"(Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for [#90FF90]{healAmount} healing[/]!)");

            if (actualHeal > 0)
            {
                AnsiConsole.MarkupLine($"{targetName} restored [#90FF90]{actualHeal} health[/]! (HP: {target.Health}/{target.MaxHealth})");
            }
            else
            {
                AnsiConsole.MarkupLine($"{targetName} already at full health!");
            }

            return true;
        }

        // Helper method for enemy targeting
        private NPC SelectEnemyTarget(List<NPC> enemies)
        {
            // If we have a preselected target (from new non-blocking system), use it
            if (preselectedTarget != null)
            {
                return preselectedTarget;
            }

            // Old blocking code - only used for AI/recruit turns
            var aliveEnemies = enemies.Where(e => e.Health > 0).ToList();

            if (aliveEnemies.Count == 0)
            {
                AnsiConsole.MarkupLine("No enemies to target!");
                return null;
            }

            if (aliveEnemies.Count == 1)
            {
                return aliveEnemies[0];
            }

            // For AI, just pick first target
            return aliveEnemies[0];
        }

        private bool IsAbilityOnCooldown(Character character, string abilityName)
        {
            if (!abilityCooldowns.ContainsKey(character))
                return false;

            if (!abilityCooldowns[character].ContainsKey(abilityName))
                return false;

            return abilityCooldowns[character][abilityName] > 0;
        }

        private void SetAbilityCooldown(Character character, string abilityName, int turns)
        {
            if (!abilityCooldowns.ContainsKey(character))
                abilityCooldowns[character] = new Dictionary<string, int>();

            abilityCooldowns[character][abilityName] = turns;
        }

        private int GetAbilityCooldown(Character character, string abilityName)
        {
            if (!abilityCooldowns.ContainsKey(character) || !abilityCooldowns[character].ContainsKey(abilityName))
                return 0;

            return abilityCooldowns[character][abilityName];
        }

        private void ApplyStatusEffect(Character target, StatusEffect effect, int duration, Character source = null)
        {
            if (!statusEffects.ContainsKey(target))
                statusEffects[target] = new Dictionary<StatusEffect, int>();

            statusEffects[target][effect] = duration;

            // Special handling for taunt
            if (effect == StatusEffect.Taunted && source != null)
                taunters[target] = source;

            SyncStatusToCharacter(target);
        }

        private bool HasStatusEffect(Character character, StatusEffect effect)
        {
            return statusEffects.ContainsKey(character) &&
                   statusEffects[character].ContainsKey(effect) &&
                   statusEffects[character][effect] > 0;
        }

        private int GetStatusDuration(Character character, StatusEffect effect)
        {
            if (!statusEffects.ContainsKey(character) || !statusEffects[character].ContainsKey(effect))
                return 0;
            return statusEffects[character][effect];
        }

        private void ReduceStatusDurations(Character character)
        {
            if (!statusEffects.ContainsKey(character)) return;

            var expiredEffects = new List<StatusEffect>();
            foreach (var effect in statusEffects[character].ToList())
            {
                statusEffects[character][effect.Key]--;
                if (statusEffects[character][effect.Key] <= 0)
                {
                    expiredEffects.Add(effect.Key);
                }
            }

            // Clean up expired effects
            foreach (var effect in expiredEffects)
            {
                statusEffects[character].Remove(effect);
                if (effect == StatusEffect.Taunted)
                    taunters.Remove(character);
            }

            SyncStatusToCharacter(character);
        }

        private void SyncStatusToCharacter(Character character)
        {
            character.ActiveStatusEffects.Clear();

            if (statusEffects.ContainsKey(character))
            {
                foreach (var effect in statusEffects[character])
                {
                    if (effect.Value > 0)
                    {
                        character.ActiveStatusEffects[effect.Key.ToString()] = effect.Value;
                    }
                }
            }
        }

        private void ClearCombatStatusEffects(Player player)
        {
            var combatOnlyEffects = new[] {
        StatusEffect.CannotAttack,
        StatusEffect.Stunned,
        StatusEffect.Taunted,
        StatusEffect.Evasive
    };

            foreach (var effect in combatOnlyEffects)
            {
                if (statusEffects.ContainsKey(player) && statusEffects[player].ContainsKey(effect))
                {
                    statusEffects[player].Remove(effect);
                }
            }

            // Clear from party members too
            foreach (var ally in player.ActiveParty)
            {
                foreach (var effect in combatOnlyEffects)
                {
                    if (statusEffects.ContainsKey(ally) && statusEffects[ally].ContainsKey(effect))
                    {
                        statusEffects[ally].Remove(effect);
                    }
                }
            }

            // Sync to character displays
            SyncStatusToCharacter(player);
            foreach (var ally in player.ActiveParty)
            {
                SyncStatusToCharacter(ally);
            }
        }

        private string GetFormattedStatusEffects(Character character)
        {
            var effects = new List<string>();

            if (statusEffects.ContainsKey(character))
            {
                foreach (var effect in statusEffects[character])
                {
                    if (effect.Value > 0)
                    {
                        // Format status effect names nicely with emojis
                        string effectName = effect.Key switch
                        {
                            StatusEffect.CannotAttack => "🛡️ Shield Wall",
                            StatusEffect.Stunned => "⚡ Stunned",
                            StatusEffect.Taunted => "😡 Taunted",
                            StatusEffect.Evasive => "💨 Evasive",
                            _ => effect.Key.ToString()
                        };
                        effects.Add(effectName);
                    }
                }
            }

            // Check for other temporary effects
            if (evasiveFireActive.ContainsKey(character) && evasiveFireActive[character])
            {
                effects.Add("💨 Evasive Fire");
            }

            if (barrierAbsorption.ContainsKey(character) && barrierAbsorption[character] > 0)
            {
                effects.Add($"🔷 Barrier ({barrierAbsorption[character]})");
            }

            // Add DOT effects
            var dotNames = character.GetActiveDOTNames();
            effects.AddRange(dotNames);

            return effects.Count > 0 ? $" [{string.Join(", ", effects)}]" : "";
        }

        private void DisplayLevelUpStats(Character character)
        {
            AnsiConsole.MarkupLine($"HP: {character.MaxHealth} | EP: {character.MaxEnergy} | ATK: {character.AttackDamage} | DEF: {character.Defense} | SPD: {character.Speed}");
        }

        private bool ExecuteShieldBashGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);

            AnsiConsole.MarkupLine($"\n[#FF0000]{character.Name} bashes {target.Name} with their shield![/]");
            AnsiConsole.MarkupLine($"(Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for [#FA8A8A]{damage} damage[/]!)");

            target.Health -= damage;

            ApplyStatusEffect(target, StatusEffect.Stunned, 1);
            AnsiConsole.MarkupLine($"[#FFFF00]{target.Name} is stunned for 1 turn![/]");

            if (target.Health <= 0)
            {
                AnsiConsole.MarkupLine($"[#90FF90]{target.Name} is defeated![/]");
            }
            return true;
        }

        private bool ExecuteTauntGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            AnsiConsole.MarkupLine($"\n[#FFFF00]{character.Name} shouts a challenge, drawing all enemies' attention![/]");

            var aliveEnemies = enemies.Where(e => e.Health > 0).ToList();
            foreach (var enemy in aliveEnemies)
            {
                ApplyStatusEffect(enemy, StatusEffect.Taunted, 2, character);
            }

            AnsiConsole.MarkupLine($"[#00FFFF]All enemies will target {character.Name} for the next 2 turns![/]");
            return true;
        }

        private bool ExecuteShieldWallGeneric(Ability ability, Character character, Player player)
        {
            if (IsAbilityOnCooldown(character, "Shield Wall"))
            {
                int cooldownRemaining = GetAbilityCooldown(character, "Shield Wall");
                AnsiConsole.MarkupLine($"\nShield Wall is on cooldown for {cooldownRemaining} more turns!");
                return false;
            }

            AnsiConsole.MarkupLine($"\n[#FFD700]{character.Name} raises their shield and forms a protective barrier![/]");
            AnsiConsole.MarkupLine($"[#00FFFF]The entire party gains +2 defense for 3 turns![/]");
            AnsiConsole.MarkupLine($"[#FF9999]{character.Name} cannot attack while maintaining the shield wall![/]");

            // Apply defense buff to party
            buffedDefense[player] = 2;
            battleCryTurns[player] = 3;

            foreach (var ally in player.ActiveParty.Where(a => a.Health > 0))
            {
                buffedDefense[ally] = 2;
                battleCryTurns[ally] = 3;
            }

            ApplyStatusEffect(character, StatusEffect.CannotAttack, 3);
            SetAbilityCooldown(character, "Shield Wall", 10);
            return true;
        }

        private bool ExecuteCleaveGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var aliveEnemies = enemies.Where(e => e.Health > 0).Take(3).ToList();

            if (aliveEnemies.Count == 0)
            {
                AnsiConsole.MarkupLine("No enemies to cleave!");
                return false;
            }

            AnsiConsole.MarkupLine($"\n[#FF0000]{character.Name} swings their weapon in a wide arc![/]");

            foreach (var enemy in aliveEnemies)
            {
                int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);
                int actualDamage = Math.Max(1, damage - enemy.Defense);
                enemy.Health -= actualDamage;

                AnsiConsole.MarkupLine($"{character.Name} cleaves {enemy.Name}! (Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for [#FA8A8A]{actualDamage} damage[/]!)");

                if (enemy.Health <= 0)
                {
                    AnsiConsole.MarkupLine($"[#90FF90]{enemy.Name} is defeated![/]");
                }
            }
            return true;
        }

        private bool ExecuteMultiShotGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var aliveEnemies = enemies.Where(e => e.Health > 0).ToList();
            if (aliveEnemies.Count == 0)
            {
                AnsiConsole.MarkupLine("No enemies to target!");
                return false;
            }

            AnsiConsole.MarkupLine($"\n[#90FF90]{character.Name} fires arrows at all enemies![/]");

            foreach (var enemy in aliveEnemies)
            {
                int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);
                enemy.Health -= damage;
                AnsiConsole.MarkupLine($"{character.Name} hits {enemy.Name}! (Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for [#FA8A8A]{damage} damage[/]!)");

                if (enemy.Health <= 0)
                {
                    AnsiConsole.MarkupLine($"[#90FF90]{enemy.Name} is defeated![/]");
                }
            }
            return true;
        }

        private bool ExecutePiercingArrowGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);

            AnsiConsole.MarkupLine($"\n[#90FF90]{character.Name} fires a piercing arrow at {target.Name}![/]");
            AnsiConsole.MarkupLine($"(Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for [#FA8A8A]{damage} damage[/]!)");
            AnsiConsole.MarkupLine($"[#FFFF00]The arrow ignores all armor![/]");

            target.Health -= damage;

            if (target.Health <= 0)
            {
                AnsiConsole.MarkupLine($"[#90FF90]{target.Name} is defeated![/]");
            }
            return true;
        }

        private bool ExecuteEvasiveFireGeneric(Ability ability, Character character)
        {
            if (IsAbilityOnCooldown(character, "Evasive Fire"))
            {
                int cooldownRemaining = GetAbilityCooldown(character, "Evasive Fire");
                AnsiConsole.MarkupLine($"\nEvasive Fire is on cooldown for {cooldownRemaining} more turns!");
                return false;
            }

            AnsiConsole.MarkupLine($"\n[#90FF90]{character.Name} readies for evasive maneuvers![/]");
            AnsiConsole.MarkupLine($"[#00FFFF]The next attack against {character.Name} will be dodged and countered![/]");

            evasiveFireActive[character] = true;
            SetAbilityCooldown(character, "Evasive Fire", 10);

            return true;
        }

        private bool ExecuteCoveringShotGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            var weapon = character.EquippedWeapon ?? EquipmentData.GetEquipment("rusty dagger");
            int coveringDice = Math.Max(3, weapon.DiceSides - 2);
            int damage = RollDice(weapon.DiceCount, coveringDice, weapon.Bonus);

            AnsiConsole.MarkupLine($"\n[#90FF90]{character.Name} fires a quick covering shot at {target.Name}![/]");
            AnsiConsole.MarkupLine($"(Rolled {weapon.DiceCount}d{coveringDice}+{weapon.Bonus} for [#FA8A8A]{damage} damage[/]!)");

            target.Health -= damage;

            character.Energy = Math.Min(character.MaxEnergy, character.Energy + 2);
            AnsiConsole.MarkupLine($"[#00FFFF]{character.Name} regains 2 energy! (EP: {character.Energy}/{character.MaxEnergy})[/]");

            if (target.Health <= 0)
            {
                AnsiConsole.MarkupLine($"[#90FF90]{target.Name} is defeated![/]");
            }
            return true;
        }

        private bool ExecuteHealGeneric(Ability ability, Character caster, Player player)
        {
            var targets = new List<Character> { player };
            targets.AddRange(player.ActiveParty.Where(a => a.Health > 0));

            if (targets.Count == 1)
            {
                return HealTarget(ability, caster, player, caster == player ? "You" : player.Name);
            }

            AnsiConsole.MarkupLine("\nWho do you want to heal?");
            for (int i = 0; i < targets.Count; i++)
            {
                string name = targets[i] == player ? "Player" : targets[i].Name;
                AnsiConsole.MarkupLine($"{i + 1}. {name} (HP: {targets[i].Health}/{targets[i].MaxHealth})");
            }

            Console.Write("Target: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            string choice = Console.ReadLine();
            Console.ResetColor();

            if (!int.TryParse(choice, out int index) || index < 1 || index > targets.Count)
            {
                AnsiConsole.MarkupLine("Invalid target!");
                return false;
            }

            var target = targets[index - 1];
            string targetName = target == player ? "the player" : target.Name;

            return HealTarget(ability, caster, target, targetName);
        }

        private bool ExecuteLightningBoltGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);

            AnsiConsole.MarkupLine($"\n[#FFFF00]{character.Name} calls down a lightning bolt on {target.Name}![/]");
            AnsiConsole.MarkupLine($"(Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for {GetTypedDamageMarkup(damage, DamageType.Lightning)})");

            // Apply lightning damage (has chance to stun)
            ApplyDamageWithType(character, target, damage, DamageType.Lightning);

            if (target.Health <= 0)
            {
                AnsiConsole.MarkupLine($"[#90FF90]{target.Name} is defeated![/]");
            }
            return true;
        }

        private bool ExecuteBlessingGeneric(Ability ability, Character caster, Player player)
        {
            AnsiConsole.MarkupLine($"\n[#FFD700]{caster.Name} invokes a divine blessing on the party![/]");
            AnsiConsole.MarkupLine($"[#00FFFF]All party members gain +2 attack for 4 turns![/]");

            buffedAttack[player] = 2;
            battleCryTurns[player] = 4;

            foreach (var ally in player.ActiveParty.Where(a => a.Health > 0))
            {
                buffedAttack[ally] = 2;
                battleCryTurns[ally] = 4;
            }

            return true;
        }
        private bool ExecuteFlameStrikeGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);

            AnsiConsole.MarkupLine($"\n[#FF4500]{character.Name} strikes {target.Name} with holy flames![/]");
            AnsiConsole.MarkupLine($"(Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for {GetTypedDamageMarkup(damage, DamageType.Fire)})");

            // Apply fire damage (creates burn DOT)
            ApplyDamageWithType(character, target, damage, DamageType.Fire);

            if (target.Health <= 0)
            {
                AnsiConsole.MarkupLine($"[#90FF90]{target.Name} is defeated![/]");
            }
            return true;
        }

        private bool ExecuteRendingStrikeGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);
            string color = GetDamageTypeColor(DamageType.Bleed);

            AnsiConsole.MarkupLine($"\n[#8B0000]{character.Name} delivers a vicious rending strike to {target.Name}![/]");
            AnsiConsole.MarkupLine($"(Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for [{color}]{damage} Bleed damage[/]!)");

            // Apply bleed damage (creates DOT)
            ApplyDamageWithType(character, target, damage, DamageType.Bleed);

            if (target.Health <= 0)
            {
                AnsiConsole.MarkupLine($"[#90FF90]{target.Name} is defeated![/]");
            }
            return true;
        }

        private string GetDOTColor(DamageType type)
        {
            switch (type)
            {
                // Physical DOTs
                case DamageType.Bleed:
                    return "#8B0000";  // Dark red for bleeding

                // Magical DOTs
                case DamageType.Fire:
                    return "#FF4500";  // Bright red-orange for burning
                case DamageType.Poison:
                    return "#00FF00";  // Bright green for poison
                case DamageType.Ice:
                    return "#87CEEB";  // Light blue for ice
                case DamageType.Lightning:
                    return "#FFFF00";  // Yellow for lightning
                default:
                    return "#FFFFFF";  // White for physical
            }
        }

        private string GetDOTTypeName(DamageType type)
        {
            switch (type)
            {
                case DamageType.Bleed:
                    return "bleeding";
                case DamageType.Fire:
                    return "burning";
                case DamageType.Poison:
                    return "poison";
                case DamageType.Ice:
                    return "frostbite";
                case DamageType.Lightning:
                    return "shock";
                default:
                    return "damage over time";
            }
        }

        private void ApplyDamageWithType(Character attacker, Character target, int baseDamage, DamageType damageType, string attackName = "attack")
        {
            int actualDamage = baseDamage;

            // Apply damage type effects
            switch (damageType)
            {
                case DamageType.Physical:
                    // Normal physical damage
                    target.TakeDamage(actualDamage);
                    break;

                case DamageType.Fire:
                    // Fire: Full damage + burn DOT
                    target.TakeDamage(actualDamage);

                    // Apply burn (3 damage per turn for 3 turns)
                    int burnDamage = Math.Max(2, actualDamage / 3);
                    var burnDOT = new DamageOverTime(DamageType.Fire, burnDamage, 3, attacker.Name);
                    target.ApplyDOT(burnDOT);

                    string[] fireGradient = { "#FF7F50", "#FF6347", "#FF4500" }; // Light to dark orange
                    DisplayStatusEffect("BURNING",
                        target == context.Player ? "You" : target.Name,
                        target == context.Player || context.Player.ActiveParty.Contains(target as Recruit),
                        "{TARGET} is engulfed in flames!",
                        fireGradient);
                    break;

                case DamageType.Ice:
                    // Ice: Full damage + defense reduction
                    target.TakeDamage(actualDamage);

                    // Reduce defense by 1 for 2 turns (we'll implement this with status effects)
                    // TODO: Implement defense reduction status effect

                    string[] iceGradient = { "#B0E0E6", "#87CEEB", "#00FFFF" }; // Light to dark cyan
                    DisplayStatusEffect("FROZEN",
                        target == context.Player ? "You" : target.Name,
                        target == context.Player || context.Player.ActiveParty.Contains(target as Recruit),
                        "Ice crystals form on {TARGET}'s armor!",
                        iceGradient);
                    break;

                case DamageType.Lightning:
                    // Lightning: Full damage + stun chance
                    target.TakeDamage(actualDamage);

                    // 30% chance to stun for 1 turn
                    if (random.Next(100) < 30)
                    {
                        ApplyStatusEffect(target, StatusEffect.Stunned, 1);

                        string[] lightningGradient = { "#FFA500", "#FFD700", "#FFFF00" }; // Light to dark yellow
                        DisplayStatusEffect("STUNNED",
                            target == context.Player ? "You" : target.Name,
                            target == context.Player || context.Player.ActiveParty.Contains(target as Recruit),
                            "Electricity crackles through {TARGET}'s body!",
                            lightningGradient);
                    }
                    break;

                case DamageType.Poison:
                    // Poison: No upfront damage, strong DOT
                    int poisonDamage = Math.Max(3, actualDamage);
                    var poisonDOT = new DamageOverTime(DamageType.Poison, poisonDamage, 4, attacker.Name);
                    target.ApplyDOT(poisonDOT);

                    string[] poisonGradient = { "#90EE90", "#32CD32", "#00FF00" }; // Light to dark green
                    DisplayStatusEffect("POISONED",
                        target == context.Player ? "You" : target.Name,
                        target == context.Player || context.Player.ActiveParty.Contains(target as Recruit),
                        "Deadly venom courses through {TARGET}'s veins!",
                        poisonGradient);
                    break;

                case DamageType.Bleed:
                    // Bleed: Full damage + bleed DOT
                    target.TakeDamage(actualDamage);

                    // Apply bleed (2 damage per turn for 3 turns)
                    int bleedDamage = Math.Max(2, actualDamage / 3);
                    var bleedDOT = new DamageOverTime(DamageType.Bleed, bleedDamage, 3, attacker.Name);
                    target.ApplyDOT(bleedDOT);

                    string[] bleedGradient = { "#CD5C5C", "#8B0000", "#800000" }; // Light to dark red
                    DisplayStatusEffect("BLEEDING",
                        target == context.Player ? "You" : target.Name,
                        target == context.Player || context.Player.ActiveParty.Contains(target as Recruit),
                        "{TARGET} suffers a grievous wound!",
                        bleedGradient);
                    break;
            }
        }

        // Get the display color for a damage type
        private string GetDamageTypeColor(DamageType type)
        {
            switch (type)
            {
                // Physical damage types
                case DamageType.Bleed:
                    return "#8B0000";  // Dark red for bleeding
                case DamageType.Crush:
                    return "#8B4513";  // Brown for crushing/sundering
                case DamageType.Concussive:
                    return "#696969";  // Gray for blunt impact

                // Magical damage types
                case DamageType.Fire:
                    return "#FF4500";  // Bright red-orange
                case DamageType.Ice:
                    return "#87CEEB";  // Light blue
                case DamageType.Lightning:
                    return "#FFFF00";  // Yellow
                case DamageType.Poison:
                    return "#00FF00";  // Bright green

                case DamageType.Physical:
                default:
                    return "#FA8A8A";  // Default damage color (pinkish)
            }
        }

        // Display damage with appropriate color based on type
        private void DisplayTypedDamage(int damage, DamageType type)
        {
            AnsiConsole.Markup(GetTypedDamageMarkup(damage, type));
        }

        // Get styled damage markup with effects for different damage types
        private string GetTypedDamageMarkup(int damage, DamageType type)
        {
            string typeName = type == DamageType.Physical ? "" : $" {type}";

            switch (type)
            {
                case DamageType.Fire:
                    // Pulsing fire damage
                    return $"<span class='fire-damage'>{damage}{typeName} damage!</span>";

                case DamageType.Ice:
                    // Shimmering ice damage
                    return $"<span class='ice-damage'>{damage}{typeName} damage!</span>";

                case DamageType.Lightning:
                    // Crackling lightning damage
                    return $"<span class='lightning-damage'>{damage}{typeName} damage!</span>";

                case DamageType.Poison:
                    // Dripping poison damage
                    return $"<span class='poison-damage'>{damage}{typeName} damage!</span>";

                case DamageType.Physical:
                default:
                    // Standard physical damage
                    string color = GetDamageTypeColor(type);
                    return $"[{color}]{damage}{typeName} damage![/]";
            }
        }

        // ============================================
        // LEVEL 5 ABILITY EXECUTIONS
        // ============================================

        private bool ExecuteBarbedArrowGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);

            AnsiConsole.MarkupLine($"\n[#FF0000]{character.Name} fires a barbed arrow at {target.Name}![/]");
            string damageText = GetTypedDamageMarkup(damage, DamageType.Physical);
            AnsiConsole.MarkupLine($"(Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for {damageText})");

            // Apply physical damage with bleed DOT (higher than normal physical)
            target.TakeDamage(damage);

            // Apply strong bleed DOT (4 damage per turn for 3 turns)
            int bleedDamage = Math.Max(4, damage / 2);
            var bleedDOT = new DamageOverTime(DamageType.Physical, bleedDamage, 3, character.Name);
            target.ApplyDOT(bleedDOT);

            AnsiConsole.MarkupLine($"\n[#FF0000]{target.Name} is bleeding from the barbed arrow![/]");

            if (target.Health <= 0)
            {
                AnsiConsole.MarkupLine($"[#90FF90]{target.Name} is defeated![/]");
            }
            return true;
        }

        private bool ExecuteFrostboltGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);

            AnsiConsole.MarkupLine($"\n[#87CEEB]{character.Name} launches a frostbolt at {target.Name}![/]");
            string damageText = GetTypedDamageMarkup(damage, DamageType.Ice);
            AnsiConsole.MarkupLine($"(Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for {damageText})");

            // Apply ice damage (full damage + defense reduction)
            ApplyDamageWithType(character, target, damage, DamageType.Ice);

            // Ice damage reduces attack as well (per description "weakens enemy attacks")
            AnsiConsole.MarkupLine($"[#87CEEB]{target.Name}'s attacks are weakened by frost![/]");

            if (target.Health <= 0)
            {
                AnsiConsole.MarkupLine($"[#90FF90]{target.Name} is defeated![/]");
            }
            return true;
        }

        // ============================================
        // LEVEL 10 ABILITY EXECUTIONS
        // ============================================

        private bool ExecuteSunderArmorGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);

            AnsiConsole.MarkupLine($"\n[#FF4500]{character.Name} delivers a crushing blow to {target.Name}'s armor![/]");
            string damageText = GetTypedDamageMarkup(damage, DamageType.Physical);
            AnsiConsole.MarkupLine($"(Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for {damageText})");

            target.TakeDamage(damage);

            // Permanently reduce defense for this combat (minimum 0)
            int defenseReduction = 2;
            target.Defense = Math.Max(0, target.Defense - defenseReduction);
            AnsiConsole.MarkupLine($"\n[#FFFF00]{target.Name}'s armor is sundered! Defense reduced by {defenseReduction}![/]");

            if (target.Health <= 0)
            {
                AnsiConsole.MarkupLine($"[#90FF90]{target.Name} is defeated![/]");
            }
            return true;
        }

        private bool ExecuteFrostArrowGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);

            AnsiConsole.MarkupLine($"\n[#87CEEB]{character.Name} fires a frost-covered arrow at {target.Name}![/]");
            string damageText = GetTypedDamageMarkup(damage, DamageType.Ice);
            AnsiConsole.MarkupLine($"(Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for {damageText})");

            // Apply ice damage (full damage + defense reduction)
            ApplyDamageWithType(character, target, damage, DamageType.Ice);

            // Additional slow effect - reduce speed
            if (target.Speed > 0)
            {
                target.Speed = Math.Max(0, target.Speed - 1);
                AnsiConsole.MarkupLine($"[#87CEEB]{target.Name} is slowed by freezing ice![/]");
            }

            if (target.Health <= 0)
            {
                AnsiConsole.MarkupLine($"[#90FF90]{target.Name} is defeated![/]");
            }
            return true;
        }

        private bool ExecuteVenomGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            AnsiConsole.MarkupLine($"\n<span class='poison-damage' style='font-size: 0.95em;'>{character.Name} curses {target.Name} with deadly venom!</span>");

            // Pure poison DOT - strong damage over time (6 damage per turn for 4 turns)
            int poisonDamage = 6;
            var poisonDOT = new DamageOverTime(DamageType.Poison, poisonDamage, 4, character.Name);
            target.ApplyDOT(poisonDOT);

            AnsiConsole.MarkupLine($"<span class='poison-damage' style='font-size: 0.95em;'>{target.Name} is poisoned! They will take {poisonDamage} damage per turn for 4 turns!</span>");

            return true;
        }

        // ============================================
        // LEVEL 15 ABILITY EXECUTIONS
        // ============================================

        private bool ExecuteDevastingSlamGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var aliveEnemies = enemies.Where(e => e.Health > 0).Take(3).ToList();

            if (aliveEnemies.Count == 0)
            {
                AnsiConsole.MarkupLine("No enemies to strike!");
                return false;
            }

            AnsiConsole.MarkupLine($"\n[#FF0000]{character.Name} slams the ground with devastating force![/]");

            foreach (var enemy in aliveEnemies)
            {
                int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);
                int actualDamage = Math.Max(1, damage - enemy.Defense);
                enemy.Health -= actualDamage;

                AnsiConsole.MarkupLine($"{character.Name} strikes {enemy.Name}! (Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for [#FA8A8A]{actualDamage} damage[/]!)");

                // 50% chance to stun each enemy
                if (random.Next(100) < 50)
                {
                    ApplyStatusEffect(enemy, StatusEffect.Stunned, 1);
                    AnsiConsole.MarkupLine($"[#FFFF00]{enemy.Name} is stunned by the devastating impact![/]");
                }

                if (enemy.Health <= 0)
                {
                    AnsiConsole.MarkupLine($"[#90FF90]{enemy.Name} is defeated![/]");
                }
            }
            return true;
        }

        private bool ExecuteThunderVolleyGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var aliveEnemies = enemies.Where(e => e.Health > 0).ToList();

            if (aliveEnemies.Count == 0)
            {
                AnsiConsole.MarkupLine("No enemies to target!");
                return false;
            }

            AnsiConsole.MarkupLine($"\n[#FFFF00]{character.Name} fires a volley of lightning-charged arrows![/]");

            foreach (var enemy in aliveEnemies)
            {
                int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);

                string damageText = GetTypedDamageMarkup(damage, DamageType.Lightning);
                AnsiConsole.MarkupLine($"{character.Name} strikes {enemy.Name}! (Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for {damageText})");

                // Apply lightning damage (has stun chance built into ApplyDamageWithType)
                ApplyDamageWithType(character, enemy, damage, DamageType.Lightning);

                if (enemy.Health <= 0)
                {
                    AnsiConsole.MarkupLine($"[#90FF90]{enemy.Name} is defeated![/]");
                }
            }
            return true;
        }

        private bool ExecuteDivineWrathGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);

            AnsiConsole.MarkupLine($"\n[#FFD700]{character.Name} calls down divine wrath upon {target.Name}![/]");
            string damageText = GetTypedDamageMarkup(damage, DamageType.Fire);
            AnsiConsole.MarkupLine($"(Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for {damageText})");

            // Apply fire damage (full damage + strong burn DOT)
            ApplyDamageWithType(character, target, damage, DamageType.Fire);

            AnsiConsole.MarkupLine($"[#FFD700]Holy flames engulf {target.Name} with righteous fury![/]");

            if (target.Health <= 0)
            {
                AnsiConsole.MarkupLine($"[#90FF90]{target.Name} is defeated![/]");
            }
            return true;
        }

    }
}