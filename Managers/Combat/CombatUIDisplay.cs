using GuildMaster.Models;
using GuildMaster.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using static GuildMaster.Managers.CombatManager;

namespace GuildMaster.Managers.Combat
{
    /// <summary>
    /// Handles all combat UI rendering including health/energy bars, status display, and combat visuals
    /// </summary>
    public class CombatUIDisplay
    {
        private readonly GameContext context;

        public CombatUIDisplay(GameContext ctx)
        {
            context = ctx;
        }

        public void ShowStatusBar()
        {
            var player = context.Player;
            int hour = (int)player.CurrentHour;
            int minutes = (int)((player.CurrentHour - hour) * 60);
            string timeOfDay = hour < 12 ? "AM" : "PM";
            int displayHour = hour > 12 ? hour - 12 : hour;
            if (displayHour == 0) displayHour = 12;

            AnsiConsole.MarkupLine($"\n<span class='stats-bar'>[HP: {player.Health}/{player.MaxHealth} | EP: {player.Energy}/{player.MaxEnergy} | Day {player.CurrentDay}, {displayHour}:{minutes:D2} {timeOfDay} | Gold: {player.Gold} | Recruits: {player.Recruits.Count}/10]</span>");
        }

        public string GenerateHealthBar(int current, int max)
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

        public string GenerateEnergyBar(int current, int max)
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

        public string GetFormattedStatusEffects(Character character,
            Dictionary<Character, Dictionary<StatusEffect, int>> statusEffects,
            Dictionary<Character, bool> evasiveFireActive,
            Dictionary<Character, int> barrierAbsorption)
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

        public void DisplayCombatStatus(Player player, List<NPC> enemies, Combatant currentTurn,
            Dictionary<Character, Dictionary<StatusEffect, int>> statusEffects,
            Dictionary<Character, bool> evasiveFireActive,
            Dictionary<Character, int> barrierAbsorption)
        {
            AnsiConsole.MarkupLine("");

            // Get status effects for current turn character
            string statusEffectsStr = GetFormattedStatusEffects(currentTurn.Character, statusEffects, evasiveFireActive, barrierAbsorption);
            string turnHeader = $"{currentTurn.Name}'s Turn{statusEffectsStr}";

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

        public void DisplayStatusEffect(string effectType, string targetName, bool isTargetPlayer, string baseMessage, string[] gradientColors)
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

        public void DisplayTypedDamage(int damage, DamageType type)
        {
            string typeColor = type switch
            {
                DamageType.Fire => "#FF4500",
                DamageType.Ice => "#87CEEB",
                DamageType.Lightning => "#FFD700",
                DamageType.Poison => "#9370DB",
                DamageType.Physical => "#FA8A8A",
                _ => "#FA8A8A"
            };

            string typeText = type switch
            {
                DamageType.Fire => "🔥 FIRE",
                DamageType.Ice => "❄️ ICE",
                DamageType.Lightning => "⚡ LIGHTNING",
                DamageType.Poison => "☠️ POISON",
                DamageType.Physical => "⚔️ PHYSICAL",
                _ => "⚔️ PHYSICAL"
            };

            AnsiConsole.MarkupLine($"[{typeColor}]{typeText} DAMAGE: {damage}[/]");
        }

        public void DisplayLevelUpStats(Character character)
        {
            AnsiConsole.MarkupLine($"\n[#FFD700]╔═══════════════════════════════════════════╗[/]");
            AnsiConsole.MarkupLine($"[#FFD700]║          ⭐ LEVEL UP! ⭐                 ║[/]");
            AnsiConsole.MarkupLine($"[#FFD700]╠═══════════════════════════════════════════╣[/]");
            AnsiConsole.MarkupLine($"[#FFD700]║  {character.Name} reached Level {character.Level}!{new string(' ', 35 - character.Name.Length - character.Level.ToString().Length)}║[/]");
            AnsiConsole.MarkupLine($"[#FFD700]╠═══════════════════════════════════════════╣[/]");
            AnsiConsole.MarkupLine($"[#FFD700]║  Max Health:  {character.MaxHealth} {"",4} ║[/]");
            AnsiConsole.MarkupLine($"[#FFD700]║  Max Energy:  {character.MaxEnergy} {"",4} ║[/]");
            AnsiConsole.MarkupLine($"[#FFD700]║  Attack:      {character.AttackDamage} {"",4} ║[/]");
            AnsiConsole.MarkupLine($"[#FFD700]║  Defense:     {character.Defense} {"",4} ║[/]");
            AnsiConsole.MarkupLine($"[#FFD700]║  Speed:       {character.Speed} {"",4} ║[/]");
            AnsiConsole.MarkupLine($"[#FFD700]╚═══════════════════════════════════════════╝[/]");
        }
    }
}
