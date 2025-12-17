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
            ChangingRow,
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
        private Recruit? currentActingPartyMember;  // Track which party member is currently taking their turn

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
        private Dictionary<Character, int> warCryDamageBoost = new Dictionary<Character, int>(); // Stores turns remaining for 20% damage boost
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

                // Display positioning
                DisplayCombatPositioning(player, enemies);

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
                    // Only set to CombatEnded if we're not in recruitment or death menu
                    if (currentState != CombatState.RecruitmentPrompt && currentState != CombatState.DeathMenu)
                    {
                        currentState = CombatState.CombatEnded;
                    }
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
                    return; // Wait for player input, don't advance turn automatically
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

            // Change Row option (now option 3)
            string rowAction = player.IsBackRow ? "Engage" : "Disengage";
            AnsiConsole.MarkupLine($"3. {rowAction}");

            // Check if player has any consumable items
            var consumables = player.Inventory.Where(item =>
                itemDescriptions.Values.Any(room =>
                    room.ContainsKey(item) && room[item].IsConsumable)).ToList();

            currentConsumables = consumables;

            // Items option (now option 4)
            if (consumables.Count > 0)
            {
                AnsiConsole.MarkupLine("4. Items");
            }

            // Flee option (now option 5 or 4 depending on items)
            int fleeOption = consumables.Count > 0 ? 5 : 4;
            AnsiConsole.MarkupLine($"{fleeOption}. Flee");

            AnsiConsole.MarkupLine("");
            ShowStatusBar();
            AnsiConsole.MarkupLine("[dim](Enter a number to choose your action)[/]");
        }

        private void ShowPartyMemberActionMenu()
        {
            if (currentActingPartyMember == null)
                return;

            currentState = CombatState.SelectingAction;

            AnsiConsole.MarkupLine($"\n[#FFFF00]== {currentActingPartyMember.Name}'s Turn ==[/]");
            AnsiConsole.MarkupLine("1. Attack");
            AnsiConsole.MarkupLine("2. Abilities");

            // Change Row option
            string rowAction = currentActingPartyMember.IsBackRow ? "Engage" : "Disengage";
            AnsiConsole.MarkupLine($"3. {rowAction}");

            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"[HP: {currentActingPartyMember.Health}/{currentActingPartyMember.MaxHealth} | EP: {currentActingPartyMember.Energy}/{currentActingPartyMember.MaxEnergy}]");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose action)[/]");
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

        private void DisplayCombatPositioning(Player player, List<NPC> enemies)
        {
            // Build ally positioning strings
            var allyFrontRow = new List<string>();
            var allyBackRow = new List<string>();

            // Add player
            if (!player.IsBackRow)
                allyFrontRow.Add("You");
            else
                allyBackRow.Add("You");

            // Add recruits
            foreach (var recruit in player.Recruits.Where(r => r.IsAlive))
            {
                if (!recruit.IsBackRow)
                    allyFrontRow.Add(recruit.Name);
                else
                    allyBackRow.Add(recruit.Name);
            }

            // Build enemy positioning strings
            var enemyFrontRow = new List<string>();
            var enemyBackRow = new List<string>();

            foreach (var enemy in enemies.Where(e => e.Health > 0))
            {
                if (!enemy.IsBackRow)
                    enemyFrontRow.Add(enemy.Name);
                else
                    enemyBackRow.Add(enemy.Name);
            }

            // Positioning is now displayed inline with character stats in DisplayCombatStatus
            // Old positioning display removed
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
            // Check if we're controlling a party member or the player
            if (currentActingPartyMember != null)
            {
                HandlePartyMemberActionSelection(input);
                return;
            }

            var player = context.Player;

            switch (input)
            {
                case "1": // Attack
                    StartPlayerAttack();
                    break;

                case "2": // Abilities
                    ShowAbilityMenu();
                    break;

                case "3": // Engage/Disengage (Change Row)
                    HandleRowChange(player);
                    break;

                case "4": // Items or Flee (depending on if items exist)
                    if (currentConsumables != null && currentConsumables.Count > 0)
                    {
                        ShowItemMenu();
                    }
                    else
                    {
                        // No items, so option 4 is Flee
                        AnsiConsole.MarkupLine("\nYou flee from combat!");
                        player.CurrentRoom = player.PreviousRoom; // Return to previous room
                        combatActive = false;
                        if (activeEnemies != null)
                            HandleCombatEnd(player, activeEnemies, combatRoom, combatActive);
                        currentState = CombatState.CombatEnded;
                        ShowStatusBar(); // Display player status after fleeing
                    }
                    break;

                case "5": // Flee (when items exist)
                    if (currentConsumables != null && currentConsumables.Count > 0)
                    {
                        AnsiConsole.MarkupLine("\nYou flee from combat!");
                        player.CurrentRoom = player.PreviousRoom; // Return to previous room
                        combatActive = false;
                        if (activeEnemies != null)
                            HandleCombatEnd(player, activeEnemies, combatRoom, combatActive);
                        currentState = CombatState.CombatEnded;
                        ShowStatusBar(); // Display player status after fleeing
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

        private void HandlePartyMemberActionSelection(string input)
        {
            if (currentActingPartyMember == null || activeEnemies == null)
                return;

            switch (input)
            {
                case "1": // Attack
                    StartPartyMemberAttack();
                    break;

                case "2": // Abilities
                    ShowPartyMemberAbilityMenu();
                    break;

                case "3": // Engage/Disengage (Change Row)
                    HandleRowChange(currentActingPartyMember);
                    break;

                default:
                    AnsiConsole.MarkupLine("\nInvalid choice! Please choose again.");
                    ShowPartyMemberActionMenu();
                    break;
            }
        }

        private void StartPartyMemberAttack()
        {
            if (currentActingPartyMember == null || activeEnemies == null)
                return;

            var aliveEnemies = activeEnemies.Where(e => e.Health > 0).ToList();

            if (aliveEnemies.Count == 1)
            {
                // Auto-target single enemy
                ExecutePartyMemberAttack(aliveEnemies[0]);
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

        private void ExecutePartyMemberAttack(NPC target)
        {
            if (currentActingPartyMember == null)
                return;

            // Check if party member is in back row and trying to use melee attack
            bool isBasicAttackMelee = currentActingPartyMember.Class is Legionnaire; // Legionnaire = melee, Venator/Oracle = ranged
            if (currentActingPartyMember.IsBackRow && isBasicAttackMelee)
            {
                AnsiConsole.MarkupLine($"\n[#FF0000]{currentActingPartyMember.Name} cannot use melee attacks from the back row![/]");
                ShowPartyMemberActionMenu();
                return;
            }

            int damage = GetWeaponDamage(currentActingPartyMember);
            string diceString = GetWeaponDiceString(currentActingPartyMember);

            AnsiConsole.MarkupLine($"\n{currentActingPartyMember.Name} attacks {target.Name}!");
            AnsiConsole.MarkupLine($"(Rolled {diceString} for [#FA8A8A]{damage} damage[/]!)");
            target.Health -= damage;

            // Legionnaires generate EP from basic attacks (25% of max EP)
            if (currentActingPartyMember.Class is Legionnaire)
            {
                int epGain = Math.Max(1, currentActingPartyMember.MaxEnergy / 4); // 25% of max EP, minimum 1
                currentActingPartyMember.Energy = Math.Min(currentActingPartyMember.MaxEnergy, currentActingPartyMember.Energy + epGain);
                AnsiConsole.MarkupLine($"<span style='color:#00FFFF'>{currentActingPartyMember.Name} gains {epGain} EP! (EP: {currentActingPartyMember.Energy}/{currentActingPartyMember.MaxEnergy})</span>");
            }

            if (target.Health <= 0)
            {
                string flavorText = GetKillFlavorText(currentActingPartyMember.Name, target.Name, currentActingPartyMember.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
            }

            // Clear the current acting party member and complete turn
            currentActingPartyMember = null;
            CompleteTurn();
        }

        private void ShowPartyMemberAbilityMenu()
        {
            if (currentActingPartyMember == null)
                return;

            currentState = CombatState.SelectingAbility;

            AnsiConsole.MarkupLine($"\n[#FFD700]== {currentActingPartyMember.Name}'s Abilities ==[/]");

            var allAbilities = currentActingPartyMember.Class?.GetClassAbilities() ?? new List<Ability>();
            // Filter abilities by unlock level
            var abilities = allAbilities.Where(a => currentActingPartyMember.Level >= a.UnlockLevel).ToList();
            // War Cry replaces Battle Cry at level 20 - hide Battle Cry if War Cry is available
            if (abilities.Any(a => a.Name == "War Cry"))
            {
                abilities = abilities.Where(a => a.Name != "Battle Cry").ToList();
            }

            if (abilities.Count == 0)
            {
                AnsiConsole.MarkupLine($"\n{currentActingPartyMember.Name} has no abilities available.");
                AnsiConsole.MarkupLine("[dim](Press 0 to go back)[/]");
                return;
            }

            for (int i = 0; i < abilities.Count; i++)
            {
                var ability = abilities[i];
                string energyColor = currentActingPartyMember.Energy >= ability.EnergyCost ? "#FFFF00" : "#808080";
                AnsiConsole.MarkupLine($"{i + 1}. {ability.Name} ([{energyColor}]{ability.EnergyCost} EP[/]) - {ability.Description}");
            }
            AnsiConsole.MarkupLine("0. Back");
            AnsiConsole.MarkupLine("\n[dim](Enter ability number)[/]");
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
                AnsiConsole.MarkupLine($"[dim]DEBUG: ProcessNextTurn completed, currentState={currentState}, IsInCombat={IsInCombat}[/]");
                AnsiConsole.MarkupLine($"[dim]DEBUG: About to invoke onStateChanged[/]");
                onStateChanged?.Invoke();
                AnsiConsole.MarkupLine($"[dim]DEBUG: onStateChanged completed, currentState={currentState}, IsInCombat={IsInCombat}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error in CompleteTurn: {ex.Message}[/]");
                AnsiConsole.MarkupLine($"[dim]{ex.StackTrace}[/]");
                currentState = CombatState.CombatEnded;
                combatActive = false;
            }
        }

        private void HandleRowChange(Character character)
        {
            // Toggle row position
            character.IsBackRow = !character.IsBackRow;
            string newRow = character.IsBackRow ? "back row" : "front row";
            AnsiConsole.MarkupLine($"\n[#FFFF00]{character.Name} moves to the {newRow}.[/]");

            // Complete the turn
            CompleteTurn();
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
                // Check if we're controlling a party member or the player
                if (currentActingPartyMember != null)
                {
                    ExecutePartyMemberAttack(currentTargetList[targetIndex - 1]);
                }
                else
                {
                    ExecutePlayerAttack(currentTargetList[targetIndex - 1]);
                }
            }
            else
            {
                AnsiConsole.MarkupLine("Invalid target! Please choose again.");
                if (currentActingPartyMember != null)
                {
                    StartPartyMemberAttack();
                }
                else
                {
                    StartPlayerAttack();
                }
            }
        }

        private void ExecutePlayerAttack(NPC target)
        {
            var player = context.Player;

            // Check if player is in back row and trying to use melee attack
            bool isBasicAttackMelee = player.Class is Legionnaire; // Legionnaire = melee, Venator/Oracle = ranged
            if (player.IsBackRow && isBasicAttackMelee)
            {
                AnsiConsole.MarkupLine("\n[#FF0000]You cannot use melee attacks from the back row![/]");
                ShowPlayerActionMenu();
                return;
            }

            int damageRoll = GetWeaponDamage(player);
            string diceString = GetWeaponDiceString(player);

            AnsiConsole.MarkupLine($"\nYou attack {target.Name} with your {player.EquippedWeapon}!");
            AnsiConsole.MarkupLine($"(Rolled {diceString} for [#FA8A8A]{damageRoll} damage[/]!)");

            target.Health -= damageRoll;

            // Legionnaires generate EP from basic attacks (25% of max EP)
            if (player.Class is Legionnaire)
            {
                int epGain = Math.Max(1, player.MaxEnergy / 4); // 25% of max EP, minimum 1
                player.Energy = Math.Min(player.MaxEnergy, player.Energy + epGain);
                AnsiConsole.MarkupLine($"<span style='color:#00FFFF'>You gain {epGain} EP from your attack! (EP: {player.Energy}/{player.MaxEnergy})</span>");
            }

            if (target.Health <= 0)
            {
                string flavorText = GetKillFlavorText("You", target.Name, player.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine($"\n{flavorText}");
            }

            CompleteTurn();
        }

        private void ShowAbilityMenu()
        {
            currentState = CombatState.SelectingAbility;
            var player = context.Player;

            AnsiConsole.MarkupLine("\n[#FFD700]== Your Abilities ==[/]");

            var allAbilities = player.Class.GetClassAbilities();
            // Filter abilities by unlock level
            var abilities = allAbilities?.Where(a => player.Level >= a.UnlockLevel).ToList() ?? new List<Ability>();
            // War Cry replaces Battle Cry at level 20 - hide Battle Cry if War Cry is available
            if (abilities.Any(a => a.Name == "War Cry"))
            {
                abilities = abilities.Where(a => a.Name != "Battle Cry").ToList();
            }

            if (abilities.Count == 0)
            {
                AnsiConsole.MarkupLine("\nYou have no abilities available.");
                AnsiConsole.MarkupLine("[dim](Press 0 to go back)[/]");
                return;
            }

            for (int i = 0; i < abilities.Count; i++)
            {
                var ability = abilities[i];
                string energyColor = player.Energy >= ability.EnergyCost ? "#FFFF00" : "#808080";
                AnsiConsole.MarkupLine($"{i + 1}. {ability.Name} ([{energyColor}]{ability.EnergyCost} EP[/]) - {ability.Description}");
            }
            AnsiConsole.MarkupLine("0. Back");
            AnsiConsole.MarkupLine("\n[dim](Enter ability number)[/]");
        }

        private void HandleAbilitySelection(string input)
        {
            AnsiConsole.MarkupLine($"[dim]DEBUG: HandleAbilitySelection called with input='{input}'[/]");

            if (input == "0")
            {
                if (currentActingPartyMember != null)
                {
                    ShowPartyMemberActionMenu();
                }
                else
                {
                    ShowPlayerActionMenu();
                }
                return;
            }

            // Determine which character is using abilities
            Character actingCharacter = currentActingPartyMember != null ? (Character)currentActingPartyMember : context.Player;
            var allAbilities = actingCharacter.Class?.GetClassAbilities() ?? new List<Ability>();
            // Filter abilities by unlock level
            var abilities = allAbilities.Where(a => actingCharacter.Level >= a.UnlockLevel).ToList();
            // War Cry replaces Battle Cry at level 20 - hide Battle Cry if War Cry is available
            if (abilities.Any(a => a.Name == "War Cry"))
            {
                abilities = abilities.Where(a => a.Name != "Battle Cry").ToList();
            }

            if (abilities.Count == 0)
            {
                if (currentActingPartyMember != null)
                {
                    ShowPartyMemberActionMenu();
                }
                else
                {
                    ShowPlayerActionMenu();
                }
                return;
            }

            if (int.TryParse(input, out int abilityIndex) && abilityIndex > 0 && abilityIndex <= abilities.Count)
            {
                var ability = abilities[abilityIndex - 1];
                AnsiConsole.MarkupLine($"[dim]DEBUG: Selected ability '{ability.Name}', energyCost={ability.EnergyCost}[/]");

                // Check energy cost
                if (actingCharacter.Energy < ability.EnergyCost)
                {
                    AnsiConsole.MarkupLine($"\nNot enough energy! {actingCharacter.Name} has {actingCharacter.Energy} EP but needs {ability.EnergyCost} EP.");
                    if (currentActingPartyMember != null)
                    {
                        ShowPartyMemberAbilityMenu();
                    }
                    else
                    {
                        ShowAbilityMenu();
                    }
                    return;
                }

                // Store ability and character for target selection
                pendingAbility = ability;
                abilityCharacter = actingCharacter;

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
                        ExecuteAbilityForCharacter(ability, actingCharacter, activeEnemies, context.Player);
                        // Clear party member after ability execution
                        currentActingPartyMember = null;
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
                        ExecuteAbilityForCharacter(ability, actingCharacter, activeEnemies, context.Player);
                    }
                    // Clear party member after ability execution
                    currentActingPartyMember = null;
                    CompleteTurn();
                }
            }
            else
            {
                AnsiConsole.MarkupLine("Invalid ability! Please choose again.");
                if (currentActingPartyMember != null)
                {
                    ShowPartyMemberAbilityMenu();
                }
                else
                {
                    ShowAbilityMenu();
                }
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
                "War Cry" => false,
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
                if (currentActingPartyMember != null)
                {
                    ShowPartyMemberActionMenu();
                }
                else
                {
                    ShowPlayerActionMenu();
                }
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
            currentActingPartyMember = null;

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
            var allAbilities = character.Class?.GetClassAbilities() ?? new List<Ability>();
            var abilities = allAbilities.Where(a => character.Level >= a.UnlockLevel).ToList();

            // War Cry replaces Battle Cry at level 20
            if (abilities.Any(a => a.Name == "War Cry"))
            {
                abilities = abilities.Where(a => a.Name != "Battle Cry").ToList();
            }

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
                    string flavorText = GetKillFlavorText("You", enemy.Name, player.EquippedWeapon, context.Player.GoreEnabled);
                    AnsiConsole.MarkupLine(flavorText);
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
                string flavorText = GetKillFlavorText("You", powerTarget.Name, player.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
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

            // Check if autocombat is enabled
            var player = context.Player;
            if (player.AutoCombatEnabled)
            {
                // Use AI to select action
                ExecuteAIAction(ally, enemies);
            }
            else
            {
                // Set current acting party member and show action menu
                currentActingPartyMember = ally;
                ShowPartyMemberActionMenu();
            }
        }

        private void ExecuteAIAction(Recruit ally, List<NPC> enemies)
        {
            AnsiConsole.MarkupLine($"\n[#FFFF00]{ally.Name}'s turn[/]");

            var allAbilities = ally.Class?.GetClassAbilities() ?? new List<Ability>();
            var abilities = allAbilities.Where(a => ally.Level >= a.UnlockLevel).ToList();

            // War Cry replaces Battle Cry at level 20
            if (abilities.Any(a => a.Name == "War Cry"))
            {
                abilities = abilities.Where(a => a.Name != "Battle Cry").ToList();
            }

            var aliveEnemies = enemies.Where(e => e.Health > 0).ToList();

            if (aliveEnemies.Count == 0)
            {
                CompleteTurn();
                return;
            }

            // AI Decision Making
            // 1. If low on health (< 40%), try to use healing ability
            if (ally.Health < ally.MaxHealth * 0.4)
            {
                var healingAbility = abilities.FirstOrDefault(a =>
                    a.Name.ToLower().Contains("heal") &&
                    ally.Energy >= a.EnergyCost);

                if (healingAbility != null)
                {
                    AnsiConsole.MarkupLine($"[dim]{ally.Name} uses {healingAbility.Name}![/]");
                    ExecuteAbilityForCharacter(healingAbility, ally, activeEnemies, context.Player);
                    CompleteTurn();
                    return;
                }
            }

            // 2. Try to use a damaging ability if we have enough energy
            var affordableAbilities = abilities.Where(a =>
                ally.Energy >= a.EnergyCost &&
                !a.Name.ToLower().Contains("heal") &&
                !a.Name.ToLower().Contains("barrier") &&
                !a.Name.ToLower().Contains("blessing")).ToList();

            if (affordableAbilities.Count > 0)
            {
                // Prefer abilities that cost more energy (usually more powerful)
                var bestAbility = affordableAbilities.OrderByDescending(a => a.EnergyCost).First();

                // Select target - prioritize enemies with lower health
                NPC target = aliveEnemies.OrderBy(e => e.Health).First();

                AnsiConsole.MarkupLine($"[dim]{ally.Name} uses {bestAbility.Name} on {target.Name}![/]");

                // Set target for single-target abilities
                if (bestAbility.Type == AbilityType.SingleTarget)
                {
                    preselectedTarget = target;
                }

                ExecuteAbilityForCharacter(bestAbility, ally, activeEnemies, context.Player);
                CompleteTurn();
                return;
            }

            // 3. Fall back to basic attack
            NPC attackTarget = aliveEnemies.OrderBy(e => e.Health).First();
            int damage = GetWeaponDamage(ally);
            string diceString = GetWeaponDiceString(ally);

            AnsiConsole.MarkupLine($"[dim]{ally.Name} attacks {attackTarget.Name}![/]");
            AnsiConsole.MarkupLine($"(Rolled {diceString} for [#FA8A8A]{damage} damage[/]!)");
            attackTarget.Health -= damage;

            if (attackTarget.Health <= 0)
            {
                string flavorText = GetKillFlavorText(ally.Name, attackTarget.Name, ally.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
            }

            CompleteTurn();
        }

        private bool ExecuteEnemyAbility(Ability ability, NPC enemy, Character target, List<Character> possibleTargets, Player player)
        {
            // Deduct energy cost
            enemy.Energy -= ability.EnergyCost;

            string enemyName = enemy.Name;
            string targetName = target == player ? "you" : target.Name;

            // Handle different ability types
            switch (ability.Type)
            {
                case AbilityType.SingleTarget:
                    // Single target damage ability
                    int damage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);
                    AnsiConsole.MarkupLine($"\n[#FF0000]{enemyName} uses {ability.Name} on {targetName}![/]");
                    AnsiConsole.MarkupLine($"(Rolled {ability.DiceCount}d{ability.DiceSides}+{ability.Bonus} for [#FA8A8A]{damage} damage[/]!)");

                    // Apply damage reduction for back row if applicable
                    int actualDamage = Math.Max(1, damage - target.Defense);
                    if (target.IsBackRow && !ability.IsRanged)
                    {
                        int reducedDamage = actualDamage / 2;
                        int finalDamage = Math.Max(1, reducedDamage);
                        AnsiConsole.MarkupLine($"(Reduced to [#FA8A8A]{finalDamage}[/] due to back row positioning!)");
                        actualDamage = finalDamage;
                    }

                    target.TakeDamage(actualDamage);

                    if (target.Health <= 0)
                    {
                        AnsiConsole.MarkupLine($"[#90FF90]{targetName} is defeated![/]");
                    }
                    return true;

                case AbilityType.AreaOfEffect:
                    // AOE damage ability - hits all possible targets
                    AnsiConsole.MarkupLine($"\n[#FF0000]{enemyName} uses {ability.Name}![/]");

                    foreach (var aoeTarget in possibleTargets.Where(t => t.Health > 0))
                    {
                        int aoeDamage = RollDice(ability.DiceCount, ability.DiceSides, ability.Bonus);
                        int aoeActualDamage = Math.Max(1, aoeDamage - aoeTarget.Defense);

                        // Apply back row protection for melee AOE
                        if (aoeTarget.IsBackRow && !ability.IsRanged)
                        {
                            aoeActualDamage = Math.Max(1, aoeActualDamage / 2);
                        }

                        aoeTarget.TakeDamage(aoeActualDamage);
                        string aoeTargetName = aoeTarget == player ? "You" : aoeTarget.Name;
                        AnsiConsole.MarkupLine($"  {aoeTargetName} take{(aoeTarget == player ? "" : "s")} [#FA8A8A]{aoeActualDamage} damage[/]!");

                        if (aoeTarget.Health <= 0)
                        {
                            AnsiConsole.MarkupLine($"  [#90FF90]{aoeTargetName} {(aoeTarget == player ? "are" : "is")} defeated![/]");
                        }
                    }
                    return true;

                default:
                    // For other ability types (buffs, heals, etc.), just use basic attack for now
                    AnsiConsole.MarkupLine($"[dim]{enemyName} attempted to use {ability.Name}, but that ability type isn't implemented for enemies yet.[/]");
                    return false;
            }
        }

        private Character SelectBestTarget(NPC enemy, List<Character> possibleTargets)
        {
            if (possibleTargets.Count == 0)
                return null;

            if (possibleTargets.Count == 1)
                return possibleTargets[0];

            // Melee enemies prioritize front row targets first
            if (enemy.Role == EnemyRole.Melee)
            {
                var frontRowTargets = possibleTargets.Where(t => !t.IsBackRow).ToList();

                // If there are front row targets, select the lowest health one
                if (frontRowTargets.Count > 0)
                {
                    return frontRowTargets.OrderBy(t => t.Health).First();
                }
                // Otherwise fall back to back row targets (lowest health)
                return possibleTargets.OrderBy(t => t.Health).First();
            }
            else
            {
                // Ranged and support enemies just target lowest health
                return possibleTargets.OrderBy(t => t.Health).First();
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
                        // Use improved AI target selection
                        target = SelectBestTarget(attackingEnemy, possibleTargets);
                    }

                    // Check if enemy should use an ability instead of basic attack
                    if (attackingEnemy.AbilityNames != null && attackingEnemy.AbilityNames.Count > 0)
                    {
                        // Get list of usable abilities (those the enemy has enough energy for)
                        var usableAbilities = new List<Ability>();
                        foreach (var abilityName in attackingEnemy.AbilityNames)
                        {
                            var ability = AbilityData.GetAbilityByName(abilityName);
                            if (ability != null && attackingEnemy.Energy >= ability.EnergyCost)
                            {
                                usableAbilities.Add(ability);
                            }
                        }

                        // 40% chance to use an ability if available
                        if (usableAbilities.Count > 0 && random.Next(100) < 40)
                        {
                            // For now, just pick a random usable ability
                            // TODO: Implement smarter ability selection (AOE when multiple targets, heals when ally hurt, etc.)
                            var selectedAbility = usableAbilities[random.Next(usableAbilities.Count)];

                            // Execute the ability
                            // Note: We need to convert possibleTargets to List<NPC> for enemy abilities
                            // For now, enemies will only use offensive abilities
                            var enemyTargets = new List<NPC>(); // Empty for now, enemies attack player/party
                            bool abilityExecuted = ExecuteEnemyAbility(selectedAbility, attackingEnemy, target, possibleTargets, player);

                            if (abilityExecuted)
                            {
                                // Regenerate energy at end of turn
                                attackingEnemy.Energy = Math.Min(attackingEnemy.MaxEnergy, attackingEnemy.Energy + attackingEnemy.EnergyRegenPerTurn);
                                return; // Ability was used, turn is over
                            }
                            // If ability execution failed, fall through to basic attack
                        }
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

                    // Check for back row protection (50% damage reduction from melee attacks)
                    // Ranged enemies' basic attacks are considered ranged
                    bool isRangedAttack = attackingEnemy.Role == EnemyRole.Ranged;
                    if (target.IsBackRow && !isRangedAttack)
                    {
                        int reducedDamage = actualDamage / 2;
                        int finalDamage = Math.Max(1, reducedDamage); // Ensure at least 1 damage
                        AnsiConsole.MarkupLine($"(Attack: {enemyDamage} - Defense: {target.Defense} = {actualDamage}, reduced to [#FA8A8A]{finalDamage}[/] due to back row positioning!)");
                        actualDamage = finalDamage;
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"(Attack: {enemyDamage} - Defense: {target.Defense} = [#FA8A8A]{actualDamage} potential damage[/]!)");
                    }

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

                    // Regenerate energy at end of turn
                    attackingEnemy.Energy = Math.Min(attackingEnemy.MaxEnergy, attackingEnemy.Energy + attackingEnemy.EnergyRegenPerTurn);

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
       
                AnsiConsole.MarkupLine($"   FLED COMBAT   ");

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
            AnsiConsole.MarkupLine("[dim]DEBUG: CleanupCombat called[/]");
            // Clear DOT effects after combat
            if (player.ActiveDOTs != null)
                player.ActiveDOTs.Clear();

            foreach (var recruit in player.ActiveParty)
            {
                if (recruit.ActiveDOTs != null)
                    recruit.ActiveDOTs.Clear();
            }

            ClearCombatStatusEffects(player);
            AnsiConsole.MarkupLine($"[dim]DEBUG: CleanupCombat setting currentState to CombatEnded (was {currentState})[/]");
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

            AnsiConsole.MarkupLine($"[dim]DEBUG: About to set currentState to RecruitmentPrompt (was {currentState})[/]");
            currentState = CombatState.RecruitmentPrompt;
            AnsiConsole.MarkupLine($"[dim]DEBUG: Set currentState to RecruitmentPrompt, IsInCombat={IsInCombat}, currentState={currentState}[/]");
            var npc = recruitableNPCs[currentRecruitIndex];

            AnsiConsole.MarkupLine($"\n{npc.Name} yields, breathing heavily.");
            AnsiConsole.MarkupLine($"\"{npc.YieldDialogue}\"");
            AnsiConsole.MarkupLine($"\n1. Welcome to the guild, {npc.Name}.");
            AnsiConsole.MarkupLine("\n[dim](Enter 1 to recruit)[/]");
            AnsiConsole.MarkupLine($"[dim]DEBUG: Exiting ShowRecruitmentPrompt, currentState={currentState}, IsInCombat={IsInCombat}[/]");
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

            // Show autocombat tutorial after SECOND combat with 3+ party members (player + 3 recruits)
            if (player.ActiveParty.Count >= 3 && ProgramStatics.messageManager != null)
            {
                player.ThreeMemberCombatCount++;

                // Show tutorial on the SECOND combat with 3+ members
                if (player.ThreeMemberCombatCount == 2)
                {
                    ProgramStatics.messageManager.CheckAndShowMessage("autocombat_tutorial");
                }
            }

            CleanupCombat(player);
            ShowStatusBar();
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
                // Show weapon dice + level bonus
                return $"{character.EquippedWeapon.DiceCount}d{character.EquippedWeapon.DiceSides}+{character.Level}";
            }
            return $"1d4+{character.Level}"; // Fallback for unarmed
        }

        private int GetWeaponDamage(Character character)
        {
            if (character.EquippedWeapon != null)
            {
                // New formula: (Weapon Dice) + Level
                int weaponRoll = RollDice(character.EquippedWeapon.DiceCount,
                                         character.EquippedWeapon.DiceSides,
                                         0); // Don't use weapon bonus, just dice
                return weaponRoll + character.Level;
            }
            // Fallback for unarmed: 1d4 + Level
            return RollDice(1, 4, 0) + character.Level;
        }

        private int CalculateAbilityDamage(Character character, Ability ability)
        {
            // New formula: (Weapon Dice + Level) + (Ability Dice + Level)
            // This applies only to damaging abilities (abilities with DiceCount > 0)

            // Weapon portion: Weapon Dice + Level
            int weaponPortion = 0;
            if (character.EquippedWeapon != null)
            {
                int weaponRoll = RollDice(character.EquippedWeapon.DiceCount,
                                         character.EquippedWeapon.DiceSides,
                                         0);
                weaponPortion = weaponRoll + character.Level;
            }
            else
            {
                // Unarmed fallback
                weaponPortion = RollDice(1, 4, 0) + character.Level;
            }

            // Ability portion: Ability Dice + Level (ignore old Bonus property)
            int abilityPortion = 0;
            if (ability.DiceCount > 0)
            {
                int abilityRoll = RollDice(ability.DiceCount, ability.DiceSides, 0);
                abilityPortion = abilityRoll + character.Level;
            }

            return weaponPortion + abilityPortion;
        }

        private string GetAbilityDiceString(Character character, Ability ability)
        {
            // Show the formula: (WeaponDice+Level) + (AbilityDice+Level)
            string weaponPart = character.EquippedWeapon != null
                ? $"({character.EquippedWeapon.DiceCount}d{character.EquippedWeapon.DiceSides}+{character.Level})"
                : $"(1d4+{character.Level})";

            if (ability.DiceCount > 0)
            {
                string abilityPart = $"({ability.DiceCount}d{ability.DiceSides}+{character.Level})";
                return $"{weaponPart} + {abilityPart}";
            }

            return weaponPart;
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

            // Original colors with CSS glow effect
            string color;
            string cssClass;
            if (percentage >= 0.6f)
            {
                color = "#90FF90";
                cssClass = "hp-bar-high";
            }
            else if (percentage >= 0.3f)
            {
                color = "#FCFC7F";
                cssClass = "hp-bar-mid";
            }
            else if (percentage > 0)
            {
                color = "#FF9999";
                cssClass = "hp-bar-low";
            }
            else
                return "[#808080][DEAD][/]     ";

            // Build the bar with CSS glow effect
            string bar = $"<span class='{cssClass}'>[{color}]";
            bar += new string('█', filledSegments);
            bar += "[/]</span>";
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
            string playerRow = player.IsBackRow ? "<span style='color:#00FF00'>[Back]</span>" : "<span style='color:#00FF00'>[Front]</span>";
            AnsiConsole.MarkupLine($" {"You",-12} HP:{playerHealthBar} {playerHP}  EP:{playerEnergyBar} {playerEP}  {playerRow}");

            // Party members
            foreach (var ally in player.ActiveParty.Where(a => a.Health > 0))
            {
                string allyHealthBar = GenerateHealthBar(ally.Health, ally.MaxHealth);
                string allyEnergyBar = GenerateEnergyBar(ally.Energy, ally.MaxEnergy);
                string allyName = ally.Name.Length > 12 ? ally.Name.Substring(0, 12) : ally.Name;
                string allyHP = $"{ally.Health}/{ally.MaxHealth}".PadLeft(9);
                string allyEP = $"{ally.Energy}/{ally.MaxEnergy}".PadLeft(9);
                string allyRow = ally.IsBackRow ? "<span style='color:#00FF00'>[Back]</span>" : "<span style='color:#00FF00'>[Front]</span>";
                AnsiConsole.MarkupLine($" {allyName,-12} HP:{allyHealthBar} {allyHP}  EP:{allyEnergyBar} {allyEP}  {allyRow}");
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
                    string enemyRow = enemy.IsBackRow ? "<span style='color:#FF0000'>[Back]</span>" : "<span style='color:#FF0000'>[Front]</span>";
                    // 27 spaces to align row indicator with party members above (total line = 71 chars to fit border)
                    AnsiConsole.MarkupLine($" {enemyName,-12} HP:{enemyBar} {enemyHP}                           {enemyRow}");
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

            // When EP is 0, show all empty segments
            if (percentage == 0)
            {
                string bar = "[#404040]";
                bar += new string('░', 10);
                bar += "[/]";
                return bar;
            }

            // Original colors with CSS glow effect
            string color;
            string cssClass;
            if (percentage >= 0.6f)
            {
                color = "#B388FF";
                cssClass = "ep-bar-high";
            }
            else if (percentage >= 0.3f)
            {
                color = "#9575CD";
                cssClass = "ep-bar-mid";
            }
            else if (percentage > 0)
            {
                color = "#7E57C2";
                cssClass = "ep-bar-low";
            }
            else
            {
                // Fallback - should never hit this due to check above
                string bar = "[#404040]";
                bar += new string('░', 10);
                bar += "[/]";
                return bar;
            }

            // Build the bar with CSS glow effect
            string bar2 = $"<span class='{cssClass}'>[{color}]";
            bar2 += new string('█', filledSegments);
            bar2 += "[/]</span>";
            bar2 += $"[#404040]";
            bar2 += new string('░', emptySegments);
            bar2 += "[/]";

            return bar2;
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
                case "Battle Cry":
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

                default:
                    AnsiConsole.MarkupLine($"[#FF0000]Ability '{ability.Name}' not yet implemented![/]");
                    return false;
            }
        }

        private bool ExecuteAbilityForCharacter(Ability ability, Character character, List<NPC> enemies, Player player, NPC preselectedTarget = null)
        {
            // Check if character is in back row and trying to use melee ability
            if (character.IsBackRow && !ability.IsRanged)
            {
                string characterName = character == player ? "You" : character.Name;
                string verb = character == player ? "cannot use" : "cannot use";
                AnsiConsole.MarkupLine($"\n[#FF0000]{characterName} {verb} melee abilities from the back row![/]");

                // Show appropriate menu based on who is acting
                if (character == player)
                {
                    ShowAbilityMenu();
                }
                else if (currentActingPartyMember != null && currentActingPartyMember == character)
                {
                    ShowPartyMemberAbilityMenu();
                }

                return false;
            }

            character.Energy -= ability.EnergyCost;

            switch (ability.Name)
            {
                // Legionnaire Abilities
                case "Shield Bash":
                    return ExecuteShieldBashGeneric(ability, character, enemies);
                case "Taunt":
                case "Battle Cry":
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

                // Level 20 Abilities
                case "Whirlwind":
                    return ExecuteWhirlwindGeneric(ability, character, enemies);
                case "War Cry":
                    return ExecuteWarCryGeneric(ability, character, enemies, player);

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

            int damage = CalculateAbilityDamage(character, ability);
            string diceString = GetAbilityDiceString(character, ability);

            AnsiConsole.MarkupLine($"\n[#FF0000]{character.Name} bashes {target.Name} with their shield![/]");
            AnsiConsole.MarkupLine($"(Rolled {diceString} for [#FA8A8A]{damage} damage[/]!)");

            target.Health -= damage;

            ApplyStatusEffect(target, StatusEffect.Stunned, 1);
            AnsiConsole.MarkupLine($"[#FFFF00]{target.Name} is stunned for 1 turn![/]");

            if (target.Health <= 0)
            {
                string flavorText = GetKillFlavorText(character.Name, target.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
            }
            return true;
        }

        private bool ExecuteTauntGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            // Check cooldown (5 turns)
            if (IsAbilityOnCooldown(character, "Battle Cry"))
            {
                int cooldownRemaining = GetAbilityCooldown(character, "Battle Cry");
                AnsiConsole.MarkupLine($"\nBattle Cry is on cooldown for {cooldownRemaining} more turns!");
                return false;
            }

            AnsiConsole.MarkupLine($"\n[#FFFF00]{character.Name} shouts a battle cry, drawing all enemies' attention![/]");

            var aliveEnemies = enemies.Where(e => e.Health > 0).ToList();
            foreach (var enemy in aliveEnemies)
            {
                ApplyStatusEffect(enemy, StatusEffect.Taunted, 2, character);
            }

            AnsiConsole.MarkupLine($"[#00FFFF]All enemies will target {character.Name} for the next 2 turns![/]");

            // Generate EP equal to 50% of max EP
            int epGain = character.MaxEnergy / 2; // 50% of max EP, rounded down
            character.Energy = Math.Min(character.MaxEnergy, character.Energy + epGain);
            AnsiConsole.MarkupLine($"<span style='color:#00FFFF'>{character.Name} gains {epGain} EP from Battle Cry! (EP: {character.Energy}/{character.MaxEnergy})</span>");

            // Set cooldown (5 turns)
            SetAbilityCooldown(character, "Battle Cry", 5);

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
                int damage = CalculateAbilityDamage(character, ability);
                int actualDamage = Math.Max(1, damage - enemy.Defense);
                enemy.Health -= actualDamage;

                string diceString = GetAbilityDiceString(character, ability);
                AnsiConsole.MarkupLine($"{character.Name} cleaves {enemy.Name}! (Rolled {diceString} for [#FA8A8A]{actualDamage} damage[/]!)");

                if (enemy.Health <= 0)
                {
                    string flavorText = GetKillFlavorText(character.Name, enemy.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                    AnsiConsole.MarkupLine(flavorText);
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
                int damage = CalculateAbilityDamage(character, ability);
                enemy.Health -= damage;
                string diceString = GetAbilityDiceString(character, ability);
                AnsiConsole.MarkupLine($"{character.Name} hits {enemy.Name}! (Rolled {diceString} for [#FA8A8A]{damage} damage[/]!)");

                if (enemy.Health <= 0)
                {
                    string flavorText = GetKillFlavorText(character.Name, enemy.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                    AnsiConsole.MarkupLine(flavorText);
                }
            }
            return true;
        }

        private bool ExecutePiercingArrowGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = CalculateAbilityDamage(character, ability);
            string diceString = GetAbilityDiceString(character, ability);

            AnsiConsole.MarkupLine($"\n[#90FF90]{character.Name} fires a piercing arrow at {target.Name}![/]");
            AnsiConsole.MarkupLine($"(Rolled {diceString} for [#FA8A8A]{damage} damage[/]!)");
            AnsiConsole.MarkupLine($"[#FFFF00]The arrow ignores all armor![/]");

            target.Health -= damage;

            if (target.Health <= 0)
            {
                string flavorText = GetKillFlavorText(character.Name, target.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
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

            int damage = CalculateAbilityDamage(character, ability);
            string diceString = GetAbilityDiceString(character, ability);

            AnsiConsole.MarkupLine($"\n[#90FF90]{character.Name} fires a quick covering shot at {target.Name}![/]");
            AnsiConsole.MarkupLine($"(Rolled {diceString} for [#FA8A8A]{damage} damage[/]!)");

            target.Health -= damage;

            character.Energy = Math.Min(character.MaxEnergy, character.Energy + 2);
            AnsiConsole.MarkupLine($"[#00FFFF]{character.Name} regains 2 energy! (EP: {character.Energy}/{character.MaxEnergy})[/]");

            if (target.Health <= 0)
            {
                string flavorText = GetKillFlavorText(character.Name, target.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
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

            int damage = CalculateAbilityDamage(character, ability);
            string diceString = GetAbilityDiceString(character, ability);

            AnsiConsole.MarkupLine($"\n[#FFFF00]{character.Name} calls down a lightning bolt on {target.Name}![/]");
            AnsiConsole.MarkupLine($"(Rolled {diceString} for {GetTypedDamageMarkup(damage, DamageType.Lightning)})");

            // Apply lightning damage (has chance to stun)
            ApplyDamageWithType(character, target, damage, DamageType.Lightning);

            if (target.Health <= 0)
            {
                string flavorText = GetKillFlavorText(character.Name, target.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
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

            int damage = CalculateAbilityDamage(character, ability);
            string diceString = GetAbilityDiceString(character, ability);

            AnsiConsole.MarkupLine($"\n[#FF4500]{character.Name} strikes {target.Name} with holy flames![/]");
            AnsiConsole.MarkupLine($"(Rolled {diceString} for {GetTypedDamageMarkup(damage, DamageType.Fire)})");

            // Apply fire damage (creates burn DOT)
            ApplyDamageWithType(character, target, damage, DamageType.Fire);

            if (target.Health <= 0)
            {
                string flavorText = GetKillFlavorText(character.Name, target.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
            }
            return true;
        }

        private bool ExecuteRendingStrikeGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = CalculateAbilityDamage(character, ability);
            string diceString = GetAbilityDiceString(character, ability);
            string color = GetDamageTypeColor(DamageType.Bleed);

            AnsiConsole.MarkupLine($"\n[#8B0000]{character.Name} delivers a vicious rending strike to {target.Name}![/]");
            AnsiConsole.MarkupLine($"(Rolled {diceString} for [{color}]{damage} Bleed damage[/]!)");

            // Apply bleed damage (creates DOT)
            ApplyDamageWithType(character, target, damage, DamageType.Bleed);

            if (target.Health <= 0)
            {
                string flavorText = GetKillFlavorText(character.Name, target.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
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

            int damage = CalculateAbilityDamage(character, ability);
            string diceString = GetAbilityDiceString(character, ability);

            AnsiConsole.MarkupLine($"\n[#FF0000]{character.Name} fires a barbed arrow at {target.Name}![/]");
            string damageText = GetTypedDamageMarkup(damage, DamageType.Physical);
            AnsiConsole.MarkupLine($"(Rolled {diceString} for {damageText})");

            // Apply physical damage with bleed DOT (higher than normal physical)
            target.TakeDamage(damage);

            // Apply strong bleed DOT (4 damage per turn for 3 turns)
            int bleedDamage = Math.Max(4, damage / 2);
            var bleedDOT = new DamageOverTime(DamageType.Physical, bleedDamage, 3, character.Name);
            target.ApplyDOT(bleedDOT);

            AnsiConsole.MarkupLine($"\n[#FF0000]{target.Name} is bleeding from the barbed arrow![/]");

            if (target.Health <= 0)
            {
                string flavorText = GetKillFlavorText(character.Name, target.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
            }
            return true;
        }

        private bool ExecuteFrostboltGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = CalculateAbilityDamage(character, ability);
            string diceString = GetAbilityDiceString(character, ability);

            AnsiConsole.MarkupLine($"\n[#87CEEB]{character.Name} launches a frostbolt at {target.Name}![/]");
            string damageText = GetTypedDamageMarkup(damage, DamageType.Ice);
            AnsiConsole.MarkupLine($"(Rolled {diceString} for {damageText})");

            // Apply ice damage (full damage + defense reduction)
            ApplyDamageWithType(character, target, damage, DamageType.Ice);

            // Ice damage reduces attack as well (per description "weakens enemy attacks")
            AnsiConsole.MarkupLine($"[#87CEEB]{target.Name}'s attacks are weakened by frost![/]");

            if (target.Health <= 0)
            {
                string flavorText = GetKillFlavorText(character.Name, target.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
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

            int damage = CalculateAbilityDamage(character, ability);
            string diceString = GetAbilityDiceString(character, ability);

            AnsiConsole.MarkupLine($"\n[#FF4500]{character.Name} delivers a crushing blow to {target.Name}'s armor![/]");
            string damageText = GetTypedDamageMarkup(damage, DamageType.Physical);
            AnsiConsole.MarkupLine($"(Rolled {diceString} for {damageText})");

            target.TakeDamage(damage);

            // Permanently reduce defense for this combat (minimum 0)
            int defenseReduction = 2;
            target.Defense = Math.Max(0, target.Defense - defenseReduction);
            AnsiConsole.MarkupLine($"\n[#FFFF00]{target.Name}'s armor is sundered! Defense reduced by {defenseReduction}![/]");

            if (target.Health <= 0)
            {
                string flavorText = GetKillFlavorText(character.Name, target.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
            }
            return true;
        }

        private bool ExecuteFrostArrowGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = CalculateAbilityDamage(character, ability);
            string diceString = GetAbilityDiceString(character, ability);

            AnsiConsole.MarkupLine($"\n[#87CEEB]{character.Name} fires a frost-covered arrow at {target.Name}![/]");
            string damageText = GetTypedDamageMarkup(damage, DamageType.Ice);
            AnsiConsole.MarkupLine($"(Rolled {diceString} for {damageText})");

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
                string flavorText = GetKillFlavorText(character.Name, target.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
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
                int damage = CalculateAbilityDamage(character, ability);
                int actualDamage = Math.Max(1, damage - enemy.Defense);
                enemy.Health -= actualDamage;

                string diceString = GetAbilityDiceString(character, ability);
                AnsiConsole.MarkupLine($"{character.Name} strikes {enemy.Name}! (Rolled {diceString} for [#FA8A8A]{actualDamage} damage[/]!)");

                // 50% chance to stun each enemy
                if (random.Next(100) < 50)
                {
                    ApplyStatusEffect(enemy, StatusEffect.Stunned, 1);
                    AnsiConsole.MarkupLine($"[#FFFF00]{enemy.Name} is stunned by the devastating impact![/]");
                }

                if (enemy.Health <= 0)
                {
                    string flavorText = GetKillFlavorText(character.Name, enemy.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                    AnsiConsole.MarkupLine(flavorText);
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
                int damage = CalculateAbilityDamage(character, ability);
                string diceString = GetAbilityDiceString(character, ability);

                string damageText = GetTypedDamageMarkup(damage, DamageType.Lightning);
                AnsiConsole.MarkupLine($"{character.Name} strikes {enemy.Name}! (Rolled {diceString} for {damageText})");

                // Apply lightning damage (has stun chance built into ApplyDamageWithType)
                ApplyDamageWithType(character, enemy, damage, DamageType.Lightning);

                if (enemy.Health <= 0)
                {
                    string flavorText = GetKillFlavorText(character.Name, enemy.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                    AnsiConsole.MarkupLine(flavorText);
                }
            }
            return true;
        }

        private bool ExecuteDivineWrathGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var target = SelectEnemyTarget(enemies);
            if (target == null) return false;

            int damage = CalculateAbilityDamage(character, ability);
            string diceString = GetAbilityDiceString(character, ability);

            AnsiConsole.MarkupLine($"\n[#FFD700]{character.Name} calls down divine wrath upon {target.Name}![/]");
            string damageText = GetTypedDamageMarkup(damage, DamageType.Fire);
            AnsiConsole.MarkupLine($"(Rolled {diceString} for {damageText})");

            // Apply fire damage (full damage + strong burn DOT)
            ApplyDamageWithType(character, target, damage, DamageType.Fire);

            AnsiConsole.MarkupLine($"[#FFD700]Holy flames engulf {target.Name} with righteous fury![/]");

            if (target.Health <= 0)
            {
                string flavorText = GetKillFlavorText(character.Name, target.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                AnsiConsole.MarkupLine(flavorText);
            }
            return true;
        }

        // ============================================
        // LEVEL 20 ABILITIES
        // ============================================

        private bool ExecuteWhirlwindGeneric(Ability ability, Character character, List<NPC> enemies)
        {
            var aliveEnemies = enemies.Where(e => e.Health > 0).ToList();

            if (aliveEnemies.Count == 0)
            {
                AnsiConsole.MarkupLine("No enemies to strike!");
                return false;
            }

            AnsiConsole.MarkupLine($"\n<span style='color:#FF0000; font-weight:bold;'>{character.Name} spins in a devastating whirlwind, striking all enemies!</span>");

            foreach (var enemy in aliveEnemies)
            {
                int damage = CalculateAbilityDamage(character, ability);
                // Whirlwind ignores row positioning and armor partially
                int actualDamage = Math.Max(1, damage - (enemy.Defense / 2));
                enemy.Health -= actualDamage;

                string diceString = GetAbilityDiceString(character, ability);
                AnsiConsole.MarkupLine($"{character.Name} strikes {enemy.Name}! (Rolled {diceString} for [#FA8A8A]{actualDamage} damage[/]!)");

                if (enemy.Health <= 0)
                {
                    string flavorText = GetKillFlavorText(character.Name, enemy.Name, character.EquippedWeapon, context.Player.GoreEnabled);
                    AnsiConsole.MarkupLine(flavorText);
                }
            }
            return true;
        }

        private bool ExecuteWarCryGeneric(Ability ability, Character character, List<NPC> enemies, Player player)
        {
            // Check cooldown (5 turns)
            if (IsAbilityOnCooldown(character, "War Cry"))
            {
                int cooldownRemaining = GetAbilityCooldown(character, "War Cry");
                AnsiConsole.MarkupLine($"\nWar Cry is on cooldown for {cooldownRemaining} more turns!");
                return false;
            }

            AnsiConsole.MarkupLine($"\n<span style='color:#FFD700; font-weight:bold;'>{character.Name} unleashes a devastating war cry![/]</span>");

            // Taunt all enemies for 2 turns
            var aliveEnemies = enemies.Where(e => e.Health > 0).ToList();
            foreach (var enemy in aliveEnemies)
            {
                ApplyStatusEffect(enemy, StatusEffect.Taunted, 2, character);
            }
            AnsiConsole.MarkupLine($"[#00FFFF]All enemies will target {character.Name} for the next 2 turns![/]");

            // Generate EP equal to 75% of max EP
            int epGain = (character.MaxEnergy * 3) / 4; // 75% of max EP, rounded down
            character.Energy = Math.Min(character.MaxEnergy, character.Energy + epGain);
            AnsiConsole.MarkupLine($"<span style='color:#00FFFF'>{character.Name} gains {epGain} EP from War Cry! (EP: {character.Energy}/{character.MaxEnergy})</span>");

            // Increase all party members' damage by 20% for 3 turns
            warCryDamageBoost[player] = 3;
            foreach (var ally in player.ActiveParty.Where(a => a.Health > 0))
            {
                warCryDamageBoost[ally] = 3;
            }
            AnsiConsole.MarkupLine($"<span style='color:#FFD700'>The entire party's damage is increased by 20% for 3 turns!</span>");

            // Set cooldown (5 turns)
            SetAbilityCooldown(character, "War Cry", 5);

            return true;
        }

        // ============================================
        // KILL FLAVOR TEXT SYSTEM
        // ============================================

        private string GetWeaponType(Equipment weapon)
        {
            if (weapon == null) return "unarmed";

            string weaponName = weapon.ShortName.ToLower();

            // Categorize weapons by type
            if (weaponName.Contains("sword") || weaponName.Contains("gladius") || weaponName.Contains("blade"))
                return "sword";
            if (weaponName.Contains("bow"))
                return "bow";
            if (weaponName.Contains("staff") || weaponName.Contains("wand"))
                return "staff";
            if (weaponName.Contains("dagger") || weaponName.Contains("knife"))
                return "dagger";
            if (weaponName.Contains("axe"))
                return "axe";
            if (weaponName.Contains("hammer") || weaponName.Contains("mace") || weaponName.Contains("club"))
                return "hammer";
            if (weaponName.Contains("spear") || weaponName.Contains("lance") || weaponName.Contains("pike"))
                return "spear";

            return "unarmed";
        }

        private string GetKillFlavorText(string killerName, string victimName, Equipment weapon, bool goreEnabled)
        {
            string weaponType = GetWeaponType(weapon);
            Random rng = new Random();

            // Define flavor text collections by weapon type
            var flavorTexts = new Dictionary<string, (List<string> clean, List<string> gore)>
            {
                ["sword"] = (
                    new List<string>
                    {
                        $"[#90FF90]{victimName} falls to {killerName}'s blade![/]",
                        $"[#90FF90]{killerName}'s sword finds its mark. {victimName} collapses![/]",
                        $"[#90FF90]With a swift strike, {killerName} defeats {victimName}![/]",
                        $"[#90FF90]{victimName} crumples under {killerName}'s assault![/]",
                        $"[#90FF90]{killerName}'s blade cuts through {victimName}'s defenses![/]",
                        $"[#90FF90]A decisive blow from {killerName} ends the fight![/]",
                        $"[#90FF90]{victimName} staggers and falls, defeated![/]",
                        $"[#90FF90]{killerName} strikes true! {victimName} is vanquished![/]",
                        $"[#90FF90]The clash of steel ends with {victimName} on the ground![/]",
                        $"[#90FF90]{victimName} falls before {killerName}'s superior swordplay![/]"
                    },
                    new List<string>
                    {
                        $"[#FA8A8A]{killerName}'s blade cleaves through {victimName}, spraying crimson![/]",
                        $"[#FA8A8A]{victimName}'s head flies from their shoulders![/]",
                        $"[#FA8A8A]{killerName} runs {victimName} through! Blood pools on the ground![/]",
                        $"[#FA8A8A]With a vicious slash, {killerName} disembowels {victimName}![/]",
                        $"[#FA8A8A]{victimName}'s blood paints the battlefield as they fall![/]",
                        $"[#FA8A8A]{killerName}'s blade cuts deep! {victimName} dies choking on blood![/]",
                        $"[#FA8A8A]A arterial spray erupts as {killerName} cuts down {victimName}![/]",
                        $"[#FA8A8A]{victimName}'s entrails spill out from the mortal wound![/]",
                        $"[#FA8A8A]{killerName} hacks {victimName} apart in a gory display![/]",
                        $"[#FA8A8A]{victimName}'s body crumples, soaked in their own blood![/]"
                    }
                ),
                ["bow"] = (
                    new List<string>
                    {
                        $"[#90FF90]{killerName}'s arrow strikes true! {victimName} falls![/]",
                        $"[#90FF90]An arrow pierces {victimName}'s heart. They collapse![/]",
                        $"[#90FF90]{killerName}'s shot finds its mark. {victimName} is defeated![/]",
                        $"[#90FF90]A perfect shot from {killerName} ends {victimName}![/]",
                        $"[#90FF90]{victimName} crumples with an arrow in their chest![/]",
                        $"[#90FF90]{killerName}'s arrow flies swift and deadly![/]",
                        $"[#90FF90]The arrow finds a gap in {victimName}'s armor![/]",
                        $"[#90FF90]{victimName} falls with a strangled gasp![/]",
                        $"[#90FF90]{killerName}'s marksmanship proves superior![/]",
                        $"[#90FF90]A fatal shot brings {victimName} down![/]"
                    },
                    new List<string>
                    {
                        $"[#FA8A8A]{killerName}'s arrow punches through {victimName}'s throat in a spray of blood![/]",
                        $"[#FA8A8A]The arrow pins {victimName} to the ground, blood gurgling from the wound![/]",
                        $"[#FA8A8A]{victimName}'s eye socket becomes a bloody crater![/]",
                        $"[#FA8A8A]{killerName}'s arrow severs {victimName}'s spine! They drop like a puppet![/]",
                        $"[#FA8A8A]Blood erupts from {victimName}'s chest as the arrow pierces clean through![/]",
                        $"[#FA8A8A]The arrow shatters {victimName}'s skull in a crimson explosion![/]",
                        $"[#FA8A8A]{victimName} convulses violently as the arrow finds their heart![/]",
                        $"[#FA8A8A]{killerName}'s shot rips through {victimName}'s neck, nearly decapitating them![/]",
                        $"[#FA8A8A]Blood pours from {victimName}'s mouth as they collapse, arrow-riddled![/]",
                        $"[#FA8A8A]The arrow punches through ribs and organs! {victimName} dies screaming![/]"
                    }
                ),
                ["staff"] = (
                    new List<string>
                    {
                        $"[#90FF90]{killerName}'s magic overwhelms {victimName}![/]",
                        $"[#90FF90]Arcane energy consumes {victimName}![/]",
                        $"[#90FF90]{victimName} falls to {killerName}'s sorcery![/]",
                        $"[#90FF90]Mystical forces end {victimName}'s life![/]",
                        $"[#90FF90]{killerName}'s spell proves fatal![/]",
                        $"[#90FF90]Magic energy crackles as {victimName} collapses![/]",
                        $"[#90FF90]The staff's power strikes down {victimName}![/]",
                        $"[#90FF90]Ethereal flames engulf {victimName}![/]",
                        $"[#90FF90]{victimName} succumbs to {killerName}'s magic![/]",
                        $"[#90FF90]A surge of power ends {victimName}'s resistance![/]"
                    },
                    new List<string>
                    {
                        $"[#FA8A8A]{victimName}'s flesh melts from their bones under {killerName}'s spell![/]",
                        $"[#FA8A8A]Arcane fire immolates {victimName}! They scream as they burn![/]",
                        $"[#FA8A8A]{killerName}'s magic tears {victimName} apart from the inside![/]",
                        $"[#FA8A8A]Blood boils and skin peels as magic consumes {victimName}![/]",
                        $"[#FA8A8A]{victimName}'s body explodes in a shower of gore![/]",
                        $"[#FA8A8A]Dark magic withers {victimName} into a husk![/]",
                        $"[#FA8A8A]Lightning chars {victimName}'s corpse black![/]",
                        $"[#FA8A8A]{victimName}'s organs liquefy from the magical assault![/]",
                        $"[#FA8A8A]Frost shatters {victimName}'s frozen body into bloody chunks![/]",
                        $"[#FA8A8A]{killerName}'s spell rips {victimName} apart at the seams![/]"
                    }
                ),
                ["dagger"] = (
                    new List<string>
                    {
                        $"[#90FF90]{killerName} strikes a vital point! {victimName} falls![/]",
                        $"[#90FF90]A swift stab ends {victimName}![/]",
                        $"[#90FF90]{killerName}'s blade finds the gap in {victimName}'s armor![/]",
                        $"[#90FF90]{victimName} collapses from {killerName}'s precise strike![/]",
                        $"[#90FF90]Quick and deadly, {killerName} defeats {victimName}![/]",
                        $"[#90FF90]The dagger's edge proves lethal![/]",
                        $"[#90FF90]{killerName} delivers the killing blow![/]",
                        $"[#90FF90]{victimName} staggers back and falls![/]",
                        $"[#90FF90]A flash of steel ends the fight![/]",
                        $"[#90FF90]{killerName}'s speed overwhelms {victimName}![/]"
                    },
                    new List<string>
                    {
                        $"[#FA8A8A]{killerName} slits {victimName}'s throat! Blood sprays everywhere![/]",
                        $"[#FA8A8A]The dagger plunges into {victimName}'s kidney! They scream and fall![/]",
                        $"[#FA8A8A]{killerName} stabs repeatedly! {victimName} dies in a pool of blood![/]",
                        $"[#FA8A8A]{victimName}'s guts spill out from the vicious stab![/]",
                        $"[#FA8A8A]Blood gushes from {victimName}'s punctured lung![/]",
                        $"[#FA8A8A]{killerName} drives the blade through {victimName}'s eye![/]",
                        $"[#FA8A8A]The dagger twists in {victimName}'s ribcage, blood pouring out![/]",
                        $"[#FA8A8A]{victimName} chokes on their own blood as the dagger finds home![/]",
                        $"[#FA8A8A]{killerName} carves {victimName} up like a butcher![/]",
                        $"[#FA8A8A]Arterial spray paints the ground as {victimName} bleeds out![/]"
                    }
                ),
                ["axe"] = (
                    new List<string>
                    {
                        $"[#90FF90]{killerName}'s axe brings {victimName} down![/]",
                        $"[#90FF90]A crushing blow defeats {victimName}![/]",
                        $"[#90FF90]{killerName} cleaves through {victimName}![/]",
                        $"[#90FF90]The axe splits {victimName}'s defenses![/]",
                        $"[#90FF90]{victimName} falls to the mighty axe![/]",
                        $"[#90FF90]With brutal force, {killerName} ends {victimName}![/]",
                        $"[#90FF90]The heavy blade finds its mark![/]",
                        $"[#90FF90]{victimName} crumples under the axe's weight![/]",
                        $"[#90FF90]{killerName}'s savage strike is decisive![/]",
                        $"[#90FF90]A powerful chop ends the battle![/]"
                    },
                    new List<string>
                    {
                        $"[#FA8A8A]{killerName}'s axe splits {victimName}'s skull like firewood![/]",
                        $"[#FA8A8A]The axe blade buries itself in {victimName}'s chest, spraying blood![/]",
                        $"[#FA8A8A]{killerName} cleaves {victimName} nearly in two![/]",
                        $"[#FA8A8A]Bone and flesh yield to the brutal axe blow![/]",
                        $"[#FA8A8A]{victimName}'s torso erupts in gore as the axe connects![/]",
                        $"[#FA8A8A]The axe shears through {victimName}'s neck, severing the head![/]",
                        $"[#FA8A8A]{killerName} hacks {victimName} apart with savage chops![/]",
                        $"[#FA8A8A]Ribs shatter and organs burst from the axe's impact![/]",
                        $"[#FA8A8A]{victimName}'s body splits open, innards spilling out![/]",
                        $"[#FA8A8A]Blood and viscera fly as {killerName}'s axe strikes home![/]"
                    }
                ),
                ["hammer"] = (
                    new List<string>
                    {
                        $"[#90FF90]{killerName}'s hammer strikes true! {victimName} falls![/]",
                        $"[#90FF90]A crushing blow ends {victimName}![/]",
                        $"[#90FF90]{victimName} crumples under the hammer's weight![/]",
                        $"[#90FF90]The mighty hammer brings {victimName} down![/]",
                        $"[#90FF90]{killerName} smashes through {victimName}'s defense![/]",
                        $"[#90FF90]With overwhelming force, {killerName} defeats {victimName}![/]",
                        $"[#90FF90]The hammer blow proves fatal![/]",
                        $"[#90FF90]{victimName} staggers and collapses![/]",
                        $"[#90FF90]Brutal strength wins the day![/]",
                        $"[#90FF90]{killerName}'s mace shatters {victimName}![/]"
                    },
                    new List<string>
                    {
                        $"[#FA8A8A]{killerName}'s hammer caves in {victimName}'s skull![/]",
                        $"[#FA8A8A]Bones shatter as the hammer pulverizes {victimName}![/]",
                        $"[#FA8A8A]{victimName}'s ribcage collapses inward, puncturing organs![/]",
                        $"[#FA8A8A]The hammer crushes {victimName} into a bloody pulp![/]",
                        $"[#FA8A8A]{victimName}'s head explodes like an overripe melon![/]",
                        $"[#FA8A8A]Blood and brain matter spray as the hammer connects![/]",
                        $"[#FA8A8A]{killerName} bashes {victimName} into a broken heap![/]",
                        $"[#FA8A8A]Vertebrae shatter under the hammer's brutal impact![/]",
                        $"[#FA8A8A]{victimName}'s chest caves in with a sickening crunch![/]",
                        $"[#FA8A8A]The hammer turns {victimName}'s body into a mangled mess![/]"
                    }
                ),
                ["spear"] = (
                    new List<string>
                    {
                        $"[#90FF90]{killerName}'s spear pierces through {victimName}![/]",
                        $"[#90FF90]A thrust ends {victimName}'s resistance![/]",
                        $"[#90FF90]{victimName} falls to the spear's reach![/]",
                        $"[#90FF90]The spear finds its mark! {victimName} is defeated![/]",
                        $"[#90FF90]{killerName} strikes from range, downing {victimName}![/]",
                        $"[#90FF90]With precision, {killerName} defeats {victimName}![/]",
                        $"[#90FF90]The spear's point proves deadly![/]",
                        $"[#90FF90]{victimName} collapses, impaled![/]",
                        $"[#90FF90]A calculated thrust ends the fight![/]",
                        $"[#90FF90]{killerName}'s reach advantage wins the day![/]"
                    },
                    new List<string>
                    {
                        $"[#FA8A8A]{killerName}'s spear bursts through {victimName}'s back![/]",
                        $"[#FA8A8A]The spear skewers {victimName}, blood running down the shaft![/]",
                        $"[#FA8A8A]{victimName} writhes on the spear, choking on blood![/]",
                        $"[#FA8A8A]{killerName} impales {victimName}, lifting them off the ground![/]",
                        $"[#FA8A8A]The spear pierces {victimName}'s heart, blood fountaining out![/]",
                        $"[#FA8A8A]{victimName}'s guts spill out around the spear shaft![/]",
                        $"[#FA8A8A]{killerName} drives the spear through {victimName}'s throat![/]",
                        $"[#FA8A8A]Blood gushes from the puncture wound as {victimName} dies![/]",
                        $"[#FA8A8A]The spear's blade tears through organs and bone![/]",
                        $"[#FA8A8A]{victimName} slides off the spear, leaving a trail of blood![/]"
                    }
                ),
                ["unarmed"] = (
                    new List<string>
                    {
                        $"[#90FF90]{killerName} strikes down {victimName} with bare hands![/]",
                        $"[#90FF90]A powerful blow defeats {victimName}![/]",
                        $"[#90FF90]{victimName} falls to {killerName}'s martial prowess![/]",
                        $"[#90FF90]{killerName} overpowers {victimName}![/]",
                        $"[#90FF90]Superior technique wins the day![/]",
                        $"[#90FF90]{victimName} crumples under the assault![/]",
                        $"[#90FF90]{killerName}'s fists prove deadly![/]",
                        $"[#90FF90]A decisive strike ends {victimName}![/]",
                        $"[#90FF90]{victimName} falls, overwhelmed![/]",
                        $"[#90FF90]{killerName}'s raw strength prevails![/]"
                    },
                    new List<string>
                    {
                        $"[#FA8A8A]{killerName} snaps {victimName}'s neck with a brutal twist![/]",
                        $"[#FA8A8A]Ribs crack and splinter under {killerName}'s assault![/]",
                        $"[#FA8A8A]{killerName} crushes {victimName}'s windpipe with bare hands![/]",
                        $"[#FA8A8A]{victimName} chokes on blood as their jaw is shattered![/]",
                        $"[#FA8A8A]{killerName} pounds {victimName}'s face into pulp![/]",
                        $"[#FA8A8A]Teeth and blood fly as {killerName} beats {victimName} senseless![/]",
                        $"[#FA8A8A]{victimName}'s skull fractures under the brutal blows![/]",
                        $"[#FA8A8A]{killerName} tears {victimName} apart with savage violence![/]",
                        $"[#FA8A8A]Blood sprays from {victimName}'s ruptured organs![/]",
                        $"[#FA8A8A]{victimName}'s spine snaps like a twig in {killerName}'s grip![/]"
                    }
                )
            };

            // Get appropriate message list
            var (cleanMessages, goreMessages) = flavorTexts.ContainsKey(weaponType)
                ? flavorTexts[weaponType]
                : flavorTexts["unarmed"];

            // Select random message from appropriate list
            var messageList = goreEnabled ? goreMessages : cleanMessages;
            string message = messageList[rng.Next(messageList.Count)];

            // Fix grammar when the player is the killer
            if (killerName == "You")
            {
                // Fix possessives: "You's" -> "your"
                message = message.Replace("You's", "your");

                // Fix subject case in certain contexts
                message = message.Replace("from You ends", "from you ends");
                message = message.Replace("with You", "with you");
                message = message.Replace("before You's", "before your");
            }

            return message;
        }

    }
}