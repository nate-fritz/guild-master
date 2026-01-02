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
    /// Handles execution of all combat abilities and their effects.
    /// Extracted from CombatManager to improve code organization.
    /// </summary>
    public class AbilityExecutor
    {
        private readonly GameContext context;
        private readonly CombatManager combatManager;
        private readonly MessageManager? messageManager;

        // Status effect tracking (shared with CombatManager)
        private Dictionary<Character, Dictionary<CombatManager.StatusEffect, int>> statusEffects;
        private Dictionary<Character, Character> taunters;
        private Dictionary<Character, int> battleCryTurns;
        private Dictionary<Character, int> buffedAttack;
        private Dictionary<Character, int> buffedDefense;
        private Dictionary<Character, int> warCryDamageBoost;
        private Dictionary<Character, Dictionary<string, int>> abilityCooldowns;
        private Dictionary<Character, bool> evasiveFireActive;
        private Dictionary<Character, int> barrierAbsorption;

        // Additional state for ability execution
        private NPC? preselectedTarget;
        private Recruit? currentActingPartyMember;
        private Random random => ProgramStatics.Random;

        public AbilityExecutor(
            GameContext gameContext,
            CombatManager manager,
            MessageManager? msgManager = null)
        {
            context = gameContext;
            combatManager = manager;
            messageManager = msgManager;

            // Initialize tracking dictionaries (will be replaced by shared dictionaries)
            statusEffects = new Dictionary<Character, Dictionary<CombatManager.StatusEffect, int>>();
            taunters = new Dictionary<Character, Character>();
            battleCryTurns = new Dictionary<Character, int>();
            buffedAttack = new Dictionary<Character, int>();
            buffedDefense = new Dictionary<Character, int>();
            warCryDamageBoost = new Dictionary<Character, int>();
            abilityCooldowns = new Dictionary<Character, Dictionary<string, int>>();
            evasiveFireActive = new Dictionary<Character, bool>();
            barrierAbsorption = new Dictionary<Character, int>();
        }

        /// <summary>
        /// Initialize shared state dictionaries from CombatManager
        /// This ensures cooldowns and other state are synchronized
        /// </summary>
        public void InitializeSharedState(
            Dictionary<Character, Dictionary<CombatManager.StatusEffect, int>> sharedStatusEffects,
            Dictionary<Character, Character> sharedTaunters,
            Dictionary<Character, int> sharedBattleCryTurns,
            Dictionary<Character, int> sharedBuffedAttack,
            Dictionary<Character, int> sharedBuffedDefense,
            Dictionary<Character, int> sharedWarCryDamageBoost,
            Dictionary<Character, Dictionary<string, int>> sharedAbilityCooldowns,
            Dictionary<Character, bool> sharedEvasiveFireActive,
            Dictionary<Character, int> sharedBarrierAbsorption)
        {
            statusEffects = sharedStatusEffects;
            taunters = sharedTaunters;
            battleCryTurns = sharedBattleCryTurns;
            buffedAttack = sharedBuffedAttack;
            buffedDefense = sharedBuffedDefense;
            warCryDamageBoost = sharedWarCryDamageBoost;
            abilityCooldowns = sharedAbilityCooldowns;
            evasiveFireActive = sharedEvasiveFireActive;
            barrierAbsorption = sharedBarrierAbsorption;
        }

        // PUBLIC METHODS - Called by CombatManager

        /// <summary>
        /// Access CombatManager's RollDice method
        /// </summary>
        private int RollDice(int count, int sides, int bonus)
        {
            return combatManager.RollDice(count, sides, bonus);
        }

        /// <summary>
        /// Access CombatManager's ShowAbilityMenu method
        /// </summary>
        private void ShowAbilityMenu()
        {
            combatManager.ShowAbilityMenu();
        }

        /// <summary>
        /// Access CombatManager's ShowPartyMemberAbilityMenu method
        /// </summary>
        private void ShowPartyMemberAbilityMenu()
        {
            combatManager.ShowPartyMemberAbilityMenu();
        }

        /// <summary>
        /// Access CombatManager's GetKillFlavorText method
        /// </summary>
        private string GetKillFlavorText(string killerName, string victimName, Equipment weapon, bool goreEnabled)
        {
            return combatManager.GetKillFlavorText(killerName, victimName, weapon, goreEnabled);
        }

        /// <summary>
        /// Access CombatManager's DisplayStatusEffect method
        /// </summary>
        private void DisplayStatusEffect(string effectType, string targetName, bool isTargetPlayer, string baseMessage, string[] gradientColors)
        {
            combatManager.DisplayStatusEffect(effectType, targetName, isTargetPlayer, baseMessage, gradientColors);
        }

        /// <summary>
        /// Set preselected target for ability execution
        /// </summary>
        public void SetPreselectedTarget(NPC target)
        {
            preselectedTarget = target;
        }

        /// <summary>
        /// Set current acting party member
        /// </summary>
        public void SetCurrentActingPartyMember(Recruit recruit)
        {
            currentActingPartyMember = recruit;
        }

        // PUBLIC ACCESSORS for shared state

        /// <summary>
        /// Get status effects dictionary for reading
        /// </summary>
        public Dictionary<Character, Dictionary<CombatManager.StatusEffect, int>> GetStatusEffects()
        {
            return statusEffects;
        }


        /// <summary>
        /// Clear all status effects (called when combat ends)
        /// </summary>
        public void ClearStatusEffects()
        {
            statusEffects.Clear();
            taunters.Clear();
            battleCryTurns.Clear();
            buffedAttack.Clear();
            buffedDefense.Clear();
            warCryDamageBoost.Clear();
            abilityCooldowns.Clear();
            evasiveFireActive.Clear();
            barrierAbsorption.Clear();
        }

        // ABILITY EXECUTION METHODS
        // These will be extracted from CombatManager.cs

        // TODO: Extract methods from CombatManager
        // - ExecuteAbilityForCharacter()
        // - CalculateAbilityDamage()
        // - ApplyStatusEffect()
        // - All Execute*Generic() methods

        // ============================================
        // EXTRACTED METHODS FROM COMBATMANAGER
        // ============================================

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
        private void ApplyCombatSingleEffect(string itemName, Effect effect, Character character)
        {
            int rollAmount = RollDice(effect.DiceCount, effect.DiceSides, effect.Bonus);

            switch (effect.Type)
            {
                case EffectType.Heal:
                    int actualHeal = Math.Min(rollAmount, character.MaxHealth - character.Health);
                    character.Health += actualHeal;

                    AnsiConsole.MarkupLine($"\n[#00FF00]{character.Name} uses the {itemName}![/]");
                    AnsiConsole.MarkupLine($"(Rolled {effect.DiceCount}d{effect.DiceSides}+{effect.Bonus} for [#00FF00]{rollAmount} hit points[/]!)");
                    if (actualHeal < rollAmount)
                    {
                        AnsiConsole.MarkupLine($"[#808080]({character.Name} was healed for {actualHeal} as they're near full health)[/]");
                    }
                    break;

                case EffectType.RestoreEnergy:
                    int actualEnergyRestore = Math.Min(rollAmount, character.MaxEnergy - character.Energy);
                    character.Energy += actualEnergyRestore;

                    AnsiConsole.MarkupLine($"\n[#0080FF]{character.Name} uses the {itemName}![/]");
                    AnsiConsole.MarkupLine($"(Rolled {effect.DiceCount}d{effect.DiceSides}+{effect.Bonus} for [#0080FF]{rollAmount} energy[/]!)");
                    if (actualEnergyRestore < rollAmount)
                    {
                        AnsiConsole.MarkupLine($"[#808080]({character.Name} restored {actualEnergyRestore} energy as they're near full energy)[/]");
                    }
                    break;

                default:
                    AnsiConsole.MarkupLine($"\n[#FFFF00]{character.Name} uses the {itemName}, but nothing seems to happen.[/]");
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

        private void ApplyStatusEffect(Character target, CombatManager.StatusEffect effect, int duration, Character source = null)
        {
            if (!statusEffects.ContainsKey(target))
                statusEffects[target] = new Dictionary<CombatManager.StatusEffect, int>();

            statusEffects[target][effect] = duration;

            // Special handling for taunt
            if (effect == CombatManager.StatusEffect.Taunted && source != null)
                taunters[target] = source;

            SyncStatusToCharacter(target);
        }

        private bool HasStatusEffect(Character character, CombatManager.StatusEffect effect)
        {
            return statusEffects.ContainsKey(character) &&
                   statusEffects[character].ContainsKey(effect) &&
                   statusEffects[character][effect] > 0;
        }

        private int GetStatusDuration(Character character, CombatManager.StatusEffect effect)
        {
            if (!statusEffects.ContainsKey(character) || !statusEffects[character].ContainsKey(effect))
                return 0;
            return statusEffects[character][effect];
        }

        private void ReduceStatusDurations(Character character)
        {
            if (!statusEffects.ContainsKey(character)) return;

            var expiredEffects = new List<CombatManager.StatusEffect>();
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
                if (effect == CombatManager.StatusEffect.Taunted)
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
        CombatManager.StatusEffect.CannotAttack,
        CombatManager.StatusEffect.Stunned,
        CombatManager.StatusEffect.Taunted,
        CombatManager.StatusEffect.Evasive
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
                            CombatManager.StatusEffect.CannotAttack => "🛡️ Shield Wall",
                            CombatManager.StatusEffect.Stunned => "⚡ Stunned",
                            CombatManager.StatusEffect.Taunted => "😡 Taunted",
                            CombatManager.StatusEffect.Evasive => "💨 Evasive",
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

        public bool ExecuteAbilityForCharacter(Ability ability, Character character, List<NPC> enemies, Player player, NPC preselectedTarget = null)
        {
            try
            {
                // Set the preselected target for use in SelectEnemyTarget
                this.preselectedTarget = preselectedTarget;

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

                bool abilitySuccess;
                switch (ability.Name)
            {
                // Legionnaire Abilities
                case "Shield Bash":
                    abilitySuccess = ExecuteShieldBashGeneric(ability, character, enemies);
                    break;
                case "Taunt":
                case "Battle Cry":
                    abilitySuccess = ExecuteTauntGeneric(ability, character, enemies);
                    break;
                case "Shield Wall":
                    abilitySuccess = ExecuteShieldWallGeneric(ability, character, player);
                    break;
                case "Cleave":
                    abilitySuccess = ExecuteCleaveGeneric(ability, character, enemies);
                    break;
                case "Rending Strike":
                    abilitySuccess = ExecuteRendingStrikeGeneric(ability, character, enemies);
                    break;

                // Venator Abilities
                case "Multi-Shot":
                    abilitySuccess = ExecuteMultiShotGeneric(ability, character, enemies);
                    break;
                case "Piercing Arrow":
                    abilitySuccess = ExecutePiercingArrowGeneric(ability, character, enemies);
                    break;
                case "Evasive Fire":
                    abilitySuccess = ExecuteEvasiveFireGeneric(ability, character);
                    break;
                case "Covering Shot":
                    abilitySuccess = ExecuteCoveringShotGeneric(ability, character, enemies);
                    break;

                // Oracle Abilities
                case "Heal":
                    abilitySuccess = ExecuteHealGeneric(ability, character, player);
                    break;
                case "Lightning Bolt":
                    abilitySuccess = ExecuteLightningBoltGeneric(ability, character, enemies);
                    break;
                case "Blessing":
                    abilitySuccess = ExecuteBlessingGeneric(ability, character, player);
                    break;
                case "Flame Strike":
                    abilitySuccess = ExecuteFlameStrikeGeneric(ability, character, enemies);
                    break;

                // Level 5 Abilities
                case "Barbed Arrow":
                    abilitySuccess = ExecuteBarbedArrowGeneric(ability, character, enemies);
                    break;
                case "Frostbolt":
                    abilitySuccess = ExecuteFrostboltGeneric(ability, character, enemies);
                    break;

                // Level 10 Abilities
                case "Sunder Armor":
                    abilitySuccess = ExecuteSunderArmorGeneric(ability, character, enemies);
                    break;
                case "Frost Arrow":
                    abilitySuccess = ExecuteFrostArrowGeneric(ability, character, enemies);
                    break;
                case "Venom":
                    abilitySuccess = ExecuteVenomGeneric(ability, character, enemies);
                    break;

                // Level 15 Abilities
                case "Devastating Slam":
                    abilitySuccess = ExecuteDevastingSlamGeneric(ability, character, enemies);
                    break;
                case "Thunder Volley":
                    abilitySuccess = ExecuteThunderVolleyGeneric(ability, character, enemies);
                    break;
                case "Divine Wrath":
                    abilitySuccess = ExecuteDivineWrathGeneric(ability, character, enemies);
                    break;

                // Level 20 Abilities
                case "Whirlwind":
                    abilitySuccess = ExecuteWhirlwindGeneric(ability, character, enemies);
                    break;
                case "War Cry":
                    abilitySuccess = ExecuteWarCryGeneric(ability, character, enemies, player);
                    break;

                default:
                    AnsiConsole.MarkupLine($"[#FF0000]Ability '{ability.Name}' not yet implemented for {character.Name}![/]");
                    abilitySuccess = false;
                    break;
                }

                // If ability failed, refund the EP cost
                if (!abilitySuccess)
                {
                    character.Energy += ability.EnergyCost;
                }

                return abilitySuccess;
            }
            finally
            {
                // Always clear the preselected target after execution to avoid stale references
                this.preselectedTarget = null;
            }
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

            ApplyStatusEffect(target, CombatManager.StatusEffect.Stunned, 1);
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
                ApplyStatusEffect(enemy, CombatManager.StatusEffect.Taunted, 2, character);
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

            ApplyStatusEffect(character, CombatManager.StatusEffect.CannotAttack, 3);
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
            string choice = Console.ReadLine();

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
                        ApplyStatusEffect(target, CombatManager.StatusEffect.Stunned, 1);

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
                    ApplyStatusEffect(enemy, CombatManager.StatusEffect.Stunned, 1);
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
                ApplyStatusEffect(enemy, CombatManager.StatusEffect.Taunted, 2, character);
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
    }
}
