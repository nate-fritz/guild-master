using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
﻿using GuildMaster.Data;
using GuildMaster.Helpers;
using GuildMaster.Models;
using System;
using System.Linq;

namespace GuildMaster.Managers
{
    public class UIManager
    {
        private readonly GameContext context;

        public UIManager(GameContext gameContext)
        {
            context = gameContext;
        }

        public void ShowTitleScreen()
        {
            Console.Clear();
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#FA935F]██╗    ██╗███████╗██╗      ██████╗ ██████╗ ███╗   ███╗███████╗[/]");
            AnsiConsole.MarkupLine("[#FA8448]██║    ██║██╔════╝██║     ██╔════╝██╔═══██╗████╗ ████║██╔════╝[/]");
            AnsiConsole.MarkupLine("[#FC7938]██║ █╗ ██║█████╗  ██║     ██║     ██║   ██║██╔████╔██║█████╗  [/]");
            AnsiConsole.MarkupLine("[#FA6419]██║███╗██║██╔══╝  ██║     ██║     ██║   ██║██║╚██╔╝██║██╔══╝  [/]");
            AnsiConsole.MarkupLine("[#FA5A0A]╚███╔███╔╝███████╗███████╗╚██████╗╚██████╔╝██║ ╚═╝ ██║███████╗[/]");
            AnsiConsole.MarkupLine("[#BA3E00] ╚══╝╚══╝ ╚══════╝╚══════╝ ╚═════╝ ╚═════╝ ╚═╝     ╚═╝╚══════╝[/]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#FA935F]                    ████████╗ ██████╗ [/]");
            AnsiConsole.MarkupLine("[#FA8448]                    ╚══██╔══╝██╔═══██╗[/]");
            AnsiConsole.MarkupLine("[#FC7938]                       ██║   ██║   ██║[/]");
            AnsiConsole.MarkupLine("[#FA6419]                       ██║   ██║   ██║[/]");
            AnsiConsole.MarkupLine("[#FA5A0A]                       ██║   ╚██████╔╝[/]");
            AnsiConsole.MarkupLine("[#BA3E00]                       ╚═╝    ╚═════╝ [/]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#FA935F] ██████╗ ██╗   ██╗██╗██╗     ██████╗ ███╗   ███╗ █████╗ ███████╗████████╗███████╗██████╗ [/]");
            AnsiConsole.MarkupLine("[#FA8448]██╔════╝ ██║   ██║██║██║     ██╔══██╗████╗ ████║██╔══██╗██╔════╝╚══██╔══╝██╔════╝██╔══██╗[/]");
            AnsiConsole.MarkupLine("[#FC7938]██║  ███╗██║   ██║██║██║     ██║  ██║██╔████╔██║███████║███████╗   ██║   █████╗  ██████╔╝[/]");
            AnsiConsole.MarkupLine("[#FA6419]██║   ██║██║   ██║██║██║     ██║  ██║██║╚██╔╝██║██╔══██║╚════██║   ██║   ██╔══╝  ██╔══██╗[/]");
            AnsiConsole.MarkupLine("[#FA5A0A]╚██████╔╝╚██████╔╝██║███████╗██████╔╝██║ ╚═╝ ██║██║  ██║███████║   ██║   ███████╗██║  ██║[/]");
            AnsiConsole.MarkupLine("[#BA3E00] ╚═════╝  ╚═════╝ ╚═╝╚══════╝╚═════╝ ╚═╝     ╚═╝╚═╝  ╚═╝╚══════╝   ╚═╝   ╚══════╝╚═╝  ╚═╝[/]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#FA8448]                            A Text Adventure RPG Game[/]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#FA8448]                      ~.~.~.~.~.~.~.~.~.~.~.~.~.~.~.~.~.~.~[/]");
            AnsiConsole.MarkupLine("");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ShowNewGameHeader()
        {
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#659AFC]        ███╗   ██╗███████╗██╗    ██╗   ██████╗  █████╗ ███╗   ███╗███████╗     [/]");
            AnsiConsole.MarkupLine("[#4887FA]        ████╗  ██║██╔════╝██║    ██║  ██╔════╝ ██╔══██╗████╗ ████║██╔════╝     [/]");
            AnsiConsole.MarkupLine("[#3178F7]        ██╔██╗ ██║█████╗  ██║ █╗ ██║  ██║  ███╗███████║██╔████╔██║█████╗       [/]");
            AnsiConsole.MarkupLine("[#1C6DFF]        ██║╚██╗██║██╔══╝  ██║███╗██║  ██║   ██║██╔══██║██║╚██╔╝██║██╔══╝       [/]");
            AnsiConsole.MarkupLine("[#0A62FF]        ██║ ╚████║███████╗╚███╔███╔╝  ╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗     [/]");
            AnsiConsole.MarkupLine("[#0048C9]        ╚═╝  ╚═══╝╚══════╝ ╚══╝╚══╝    ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝     [/]");
            AnsiConsole.MarkupLine("");
            Console.ResetColor();
        }

        public void ShowCharacterStatsHeader()
        {
            AnsiConsole.MarkupLine("");

            AnsiConsole.MarkupLine("[#03C9D7]       ██████╗██╗  ██╗ █████╗ ██████╗  █████╗  ██████╗████████╗███████╗██████╗   [/]");
            AnsiConsole.MarkupLine("[#03B6C3]      ██╔════╝██║  ██║██╔══██╗██╔══██╗██╔══██╗██╔════╝╚══██╔══╝██╔════╝██╔══██╗  [/]");
            AnsiConsole.MarkupLine("[#0399A4]      ██║     ███████║███████║██████╔╝███████║██║        ██║   █████╗  ██████╔╝  [/]");
            AnsiConsole.MarkupLine("[#01838D]      ██║     ██╔══██║██╔══██║██╔══██╗██╔══██║██║        ██║   ██╔══╝  ██╔══██╗  [/]");
            AnsiConsole.MarkupLine("[#017077]      ╚██████╗██║  ██║██║  ██║██║  ██║██║  ██║╚██████╗   ██║   ███████╗██║  ██║  [/]");
            AnsiConsole.MarkupLine("[#015C62]       ╚═════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝ ╚═════╝   ╚═╝   ╚══════╝╚═╝  ╚═╝  [/]");
 
            Console.ResetColor();
        }

        public void DisplayStats()
        {
            var player = context.Player;
            Console.ForegroundColor = ConsoleColor.Cyan;

            int hour = (int)player.CurrentHour;
            int minutes = (int)((player.CurrentHour - hour) * 60);

            string timeOfDay = hour < 12 ? "AM" : "PM";
            int displayHour = hour > 12 ? hour - 12 : hour;
            if (displayHour == 0) displayHour = 12;

            AnsiConsole.MarkupLine($"\n<span class='stats-bar'>[HP: {player.Health}/{player.MaxHealth} | EP: {player.Energy}/{player.MaxEnergy} | Day {player.CurrentDay}, {displayHour}:{minutes:D2} {timeOfDay} | Gold: {player.Gold} | Recruits: {player.Recruits.Count}/10]</span>");
            Console.ResetColor();
        }

        public void ShowStats()
        {
            var player = context.Player;
            ShowCharacterStatsHeader();

            AnsiConsole.MarkupLine($"\nName: {player.Name}");

            if (player.Class != null)
            {
                AnsiConsole.MarkupLine($"Class: {player.Class.Name}");
                AnsiConsole.MarkupLine($"  {player.Class.Description}");
            }
            AnsiConsole.MarkupLine($"Class: {player.Class.Name} - Level {player.Level}");
            AnsiConsole.MarkupLine($"Experience: {player.Experience}/{player.ExperienceToNextLevel}");


            AnsiConsole.MarkupLine($"\nHealth: {player.Health}/{player.MaxHealth}");
            AnsiConsole.MarkupLine($"Energy: {player.Energy}/{player.MaxEnergy}");

            // Show equipped items
            AnsiConsole.MarkupLine($"\n=== Equipment ===");
            DisplayEquipmentSlot(player, EquipmentSlot.Weapon);
            DisplayEquipmentSlot(player, EquipmentSlot.Armor);
            DisplayEquipmentSlot(player, EquipmentSlot.Helm);
            DisplayEquipmentSlot(player, EquipmentSlot.Ring);

            // Show total stats (base + equipment bonuses)
            AnsiConsole.MarkupLine($"\n=== Total Stats ===");
            if (player.EquippedWeapon != null)
            {
                int avgDamage = (player.EquippedWeapon.DiceCount * (player.EquippedWeapon.DiceSides + 1) / 2) + player.EquippedWeapon.Bonus;
                AnsiConsole.MarkupLine($"Attack: {player.EquippedWeapon.DiceString} (avg: {avgDamage})");
            }
            else
            {
                AnsiConsole.MarkupLine($"Attack: Unarmed (1d4+1)");
            }

            int totalDefense = player.Defense + player.GetEquipmentBonus("defense");
            int totalSpeed = player.Speed + player.GetEquipmentBonus("speed");
            int totalMaxHealth = player.MaxHealth + player.GetEquipmentBonus("health");
            int totalMaxEnergy = player.MaxEnergy + player.GetEquipmentBonus("energy");

            string defenseDisplay = player.GetEquipmentBonus("defense") > 0
                ? $"{player.Defense} + {player.GetEquipmentBonus("defense")} = {totalDefense}"
                : $"{totalDefense}";
            string speedDisplay = player.GetEquipmentBonus("speed") != 0
                ? $"{player.Speed} + {player.GetEquipmentBonus("speed")} = {totalSpeed}"
                : $"{totalSpeed}";
            string healthDisplay = player.GetEquipmentBonus("health") > 0
                ? $"{player.MaxHealth} + {player.GetEquipmentBonus("health")} = {totalMaxHealth}"
                : $"{totalMaxHealth}";
            string energyDisplay = player.GetEquipmentBonus("energy") > 0
                ? $"{player.MaxEnergy} + {player.GetEquipmentBonus("energy")} = {totalMaxEnergy}"
                : $"{totalMaxEnergy}";

            AnsiConsole.MarkupLine($"Max Health: {healthDisplay}");
            AnsiConsole.MarkupLine($"Max Energy: {energyDisplay}");
            AnsiConsole.MarkupLine($"Defense: {defenseDisplay}");
            AnsiConsole.MarkupLine($"Speed: {speedDisplay}");

            // Show available abilities
            if (player.Class != null)
            {
                var allAbilities = player.Class.GetClassAbilities();
                // Filter by unlock level and hide Battle Cry if War Cry is available
                var abilities = allAbilities.Where(a => player.Level >= a.UnlockLevel).ToList();
                if (abilities.Any(a => a.Name == "War Cry"))
                {
                    abilities = abilities.Where(a => a.Name != "Battle Cry").ToList();
                }

                AnsiConsole.MarkupLine($"\nClass Abilities:");
                foreach (var ability in abilities)
                {
                    string costText = ability.EnergyCost > 0 ? $" ({ability.EnergyCost} EP)" : " (Cooldown)";
                    AnsiConsole.MarkupLine($"  - {ability.Name}{costText}");
                }
            }

            // Show current status effects
            if (player.ActiveStatusEffects.Count > 0)
            {
                AnsiConsole.MarkupLine($"\nActive Status Effects:");
                foreach (var effect in player.ActiveStatusEffects)
                {
                    AnsiConsole.MarkupLine($"  - {effect.Key} ({effect.Value} turns remaining)");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"\nStatus Effects: None");
            }
        }

        public void ShowInventory()
        {
            var player = context.Player;

            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#FA935F]═══════════════════════════════════════════════════════════════════[/]");
            AnsiConsole.MarkupLine("[#FA935F]                            INVENTORY                               [/]");
            AnsiConsole.MarkupLine("[#FA935F]═══════════════════════════════════════════════════════════════════[/]");
            AnsiConsole.MarkupLine("");

            // Show currently equipped items
            DisplayCurrentlyEquipped(player);

            // Show equipment in inventory
            DisplayEquipmentInInventory(player);

            // Show consumables
            DisplayConsumablesInInventory(player);

            // Show other items
            DisplayOtherItemsInInventory(player);

            // Show available commands
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#75C8FF]═══ Available Commands ═══[/]");
            AnsiConsole.MarkupLine("[dim]equip <#/name>  - Equip an item by number or name (e.g., 'equip 1' or 'equip armor')[/]");
            AnsiConsole.MarkupLine("[dim]unequip <slot>  - Unequip an item (weapon/armor/helm/ring)[/]");
            AnsiConsole.MarkupLine("[dim]use <item>      - Use a consumable item by name[/]");
        }

        public void ShowHelp()
        {
            AnsiConsole.MarkupLine("\nMovement:");
            AnsiConsole.MarkupLine("Move North:           n or north");
            AnsiConsole.MarkupLine("Move East:            e or east");
            AnsiConsole.MarkupLine("Move South:           s or south");
            AnsiConsole.MarkupLine("Move West:            w or west");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("Exploration:");
            AnsiConsole.MarkupLine("Look around:          l or look or look around");
            AnsiConsole.MarkupLine("Look at something:    look [object or person]");
            AnsiConsole.MarkupLine("Talk:                 talk or t");
            AnsiConsole.MarkupLine("Take object:          take [object]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("Items:");
            AnsiConsole.MarkupLine("Inventory:            inventory, inv, or i");
            AnsiConsole.MarkupLine("Equip item:           equip [item]");
            AnsiConsole.MarkupLine("Unequip item:         unequip [slot] or remove [slot]");
            AnsiConsole.MarkupLine("Use item:             use [item]");
            AnsiConsole.MarkupLine("Use on ally:          use [item] on [name]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("Other:");
            AnsiConsole.MarkupLine("Character Sheet:      stats");
            AnsiConsole.MarkupLine("Party Status:         party or p");
            AnsiConsole.MarkupLine("Guild Management:     guild or g");
            AnsiConsole.MarkupLine("Rest:                 rest");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("System:");
            AnsiConsole.MarkupLine("Save game:            save");
            AnsiConsole.MarkupLine("Load game:            load");
            AnsiConsole.MarkupLine("Quit the game:        qq or quit");
        }

        public void ShowMainMenu()
        {
            AnsiConsole.MarkupLine("\nMain Menu:");
            AnsiConsole.MarkupLine("1. Continue Game");
            AnsiConsole.MarkupLine("2. Start New Game");
            Console.Write("\nEnter your choice (1 or 2): ");
        }

        public void ShowPartyStatus()
        {
            var player = context.Player;

            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#FA935F]═══════════════════════════════════════════════════════════════════════════════[/]");
            AnsiConsole.MarkupLine("[#FA935F]                              ACTIVE PARTY STATUS                              [/]");
            AnsiConsole.MarkupLine("[#FA935F]═══════════════════════════════════════════════════════════════════════════════[/]");

            // Display player
            DisplayPartyMemberCard(player, "You", true);

            // Display party members
            for (int i = 0; i < player.ActiveParty.Count; i++)
            {
                DisplayPartyMemberCard(player.ActiveParty[i], player.ActiveParty[i].Name, false);
            }

            AnsiConsole.MarkupLine("[#FA935F]═══════════════════════════════════════════════════════════════════════════════[/]");

            // Show available commands
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#75C8FF]═══ Available Commands ═══[/]");
            AnsiConsole.MarkupLine("[dim]guild or g      - Manage party members and equipment[/]");
            AnsiConsole.MarkupLine("[dim]inventory or i  - View and manage your inventory[/]");
        }

        private string FormatPartyRow(string col1, string col2, string col3)
        {
            // Each column is 25 chars wide
            col1 = col1.PadRight(25);
            col2 = col2.PadRight(25);
            col3 = col3.PadRight(25);

            if (string.IsNullOrWhiteSpace(col3) && string.IsNullOrWhiteSpace(col2))
                return col1.TrimEnd();
            else if (string.IsNullOrWhiteSpace(col3))
                return $"{col1}│{col2}".TrimEnd();
            else
                return $"{col1}│{col2}│{col3}".TrimEnd();
        }

        private void DisplayEquipmentSlot(Character character, EquipmentSlot slot)
        {
            var equipment = character.GetEquipmentInSlot(slot);
            string slotName = slot.ToString();

            if (equipment != null)
            {
                AnsiConsole.MarkupLine($"{slotName}: [#00FFFF]{equipment.Name}[/]");

                // Show relevant stats for this equipment
                var bonuses = new List<string>();
                if (equipment.DiceCount > 0)
                    bonuses.Add($"Damage: {equipment.DiceString}");
                if (equipment.HealthBonus != 0)
                    bonuses.Add($"HP +{equipment.HealthBonus}");
                if (equipment.EnergyBonus != 0)
                    bonuses.Add($"EP +{equipment.EnergyBonus}");
                if (equipment.AttackBonus != 0)
                    bonuses.Add($"ATK +{equipment.AttackBonus}");
                if (equipment.DefenseBonus != 0)
                    bonuses.Add($"DEF +{equipment.DefenseBonus}");
                if (equipment.SpeedBonus != 0)
                    bonuses.Add($"SPD {(equipment.SpeedBonus > 0 ? "+" : "")}{equipment.SpeedBonus}");

                if (bonuses.Count > 0)
                {
                    // Use Markup.Escape to prevent [ ] from being interpreted as color codes
                    string bonusText = Markup.Escape($"[{string.Join(", ", bonuses)}]");
                    AnsiConsole.MarkupLine($"   [dim]{bonusText}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"{slotName}: [dim]Empty[/]");
            }
        }

        private void DisplayCurrentlyEquipped(Character character)
        {
            AnsiConsole.MarkupLine("[#90FF90]═══ Currently Equipped ═══[/]");

            DisplayEquippedSlot(character, EquipmentSlot.Weapon, "Weapon");
            DisplayEquippedSlot(character, EquipmentSlot.Armor, "Armor");
            DisplayEquippedSlot(character, EquipmentSlot.Helm, "Helm");
            DisplayEquippedSlot(character, EquipmentSlot.Ring, "Ring");

            // Show total bonuses
            var bonuses = new List<string>();
            if (character.GetEquipmentBonus("health") > 0)
                bonuses.Add($"HP+{character.GetEquipmentBonus("health")}");
            if (character.GetEquipmentBonus("energy") > 0)
                bonuses.Add($"EP+{character.GetEquipmentBonus("energy")}");
            if (character.GetEquipmentBonus("attack") > 0)
                bonuses.Add($"ATK+{character.GetEquipmentBonus("attack")}");
            if (character.GetEquipmentBonus("defense") > 0)
                bonuses.Add($"DEF+{character.GetEquipmentBonus("defense")}");
            if (character.GetEquipmentBonus("speed") != 0)
                bonuses.Add($"SPD{(character.GetEquipmentBonus("speed") > 0 ? "+" : "")}{character.GetEquipmentBonus("speed")}");

            if (bonuses.Count > 0)
                AnsiConsole.MarkupLine($"\n[#FFFF00]Total Bonuses:[/] {string.Join(" | ", bonuses)}");
            else
                AnsiConsole.MarkupLine($"\n[dim]Total Bonuses: None[/]");
        }

        private void DisplayEquippedSlot(Character character, EquipmentSlot slot, string slotName)
        {
            var equipment = character.GetEquipmentInSlot(slot);
            string slotDisplay = slotName.PadRight(10) + ":";

            if (equipment != null)
            {
                AnsiConsole.Markup($"{slotDisplay} [#00FFFF]{equipment.Name}[/]");

                // Show key stats inline
                var stats = new List<string>();
                if (equipment.DiceCount > 0)
                    stats.Add($"DMG: {equipment.DiceString}");
                if (equipment.DefenseBonus > 0)
                    stats.Add($"DEF+{equipment.DefenseBonus}");
                if (equipment.HealthBonus > 0)
                    stats.Add($"HP+{equipment.HealthBonus}");

                if (stats.Count > 0)
                    AnsiConsole.MarkupLine($" [dim]({string.Join(", ", stats)})[/]");
                else
                    AnsiConsole.MarkupLine("");
            }
            else
            {
                AnsiConsole.MarkupLine($"{slotDisplay} [dim]Empty[/]");
            }
        }

        private void DisplayEquipmentInInventory(Player player)
        {
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#90FF90]═══ Equipment in Inventory ═══[/]");

            var equipmentItems = player.Inventory
                .Where(item => EquipmentData.AllEquipment.ContainsKey(item.ToLower()))
                .ToList();

            if (equipmentItems.Count == 0)
            {
                AnsiConsole.MarkupLine("[dim]  No equipment[/]");
            }
            else
            {
                int index = 1;
                foreach (var itemName in equipmentItems)
                {
                    var equipment = EquipmentData.GetEquipment(itemName);
                    AnsiConsole.Markup($"  {index}. [#00FFFF]{equipment.Name}[/] ({equipment.Slot})");

                    var stats = new List<string>();
                    if (equipment.DiceCount > 0)
                        stats.Add($"DMG: {equipment.DiceString}");
                    if (equipment.HealthBonus != 0)
                        stats.Add($"HP+{equipment.HealthBonus}");
                    if (equipment.EnergyBonus != 0)
                        stats.Add($"EP+{equipment.EnergyBonus}");
                    if (equipment.AttackBonus != 0)
                        stats.Add($"ATK+{equipment.AttackBonus}");
                    if (equipment.DefenseBonus != 0)
                        stats.Add($"DEF+{equipment.DefenseBonus}");
                    if (equipment.SpeedBonus != 0)
                        stats.Add($"SPD{(equipment.SpeedBonus > 0 ? "+" : "")}{equipment.SpeedBonus}");

                    if (stats.Count > 0)
                        AnsiConsole.MarkupLine($" - [dim]{string.Join(", ", stats)}[/]");
                    else
                        AnsiConsole.MarkupLine("");

                    index++;
                }
            }
        }

        private void DisplayConsumablesInInventory(Player player)
        {
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#90FF90]═══ Consumables ═══[/]");

            // Get consumable items
            var consumables = new Dictionary<string, int>();
            foreach (var item in player.Inventory)
            {
                // Check if it's a consumable (has an effect)
                bool isConsumable = false;
                foreach (var roomItems in context.ItemDescriptions.Values)
                {
                    if (roomItems.ContainsKey(item) && roomItems[item].IsConsumable)
                    {
                        isConsumable = true;
                        break;
                    }
                }

                if (isConsumable)
                {
                    if (consumables.ContainsKey(item))
                        consumables[item]++;
                    else
                        consumables[item] = 1;
                }
            }

            if (consumables.Count == 0)
            {
                AnsiConsole.MarkupLine("[dim]  No consumables[/]");
            }
            else
            {
                foreach (var kvp in consumables)
                {
                    string itemName = TextHelper.CapitalizeFirst(kvp.Key);
                    AnsiConsole.MarkupLine($"  - {Markup.Escape(itemName)} x{kvp.Value}");
                }
            }
        }

        private void DisplayOtherItemsInInventory(Player player)
        {
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#90FF90]═══ Other Items ═══[/]");

            var otherItems = player.Inventory.Where(item => {
                // Not equipment
                if (EquipmentData.AllEquipment.ContainsKey(item.ToLower()))
                    return false;

                // Not consumable
                foreach (var roomItems in context.ItemDescriptions.Values)
                {
                    if (roomItems.ContainsKey(item) && roomItems[item].IsConsumable)
                        return false;
                }

                return true;
            }).ToList();

            if (otherItems.Count == 0)
            {
                AnsiConsole.MarkupLine("[dim]  No other items[/]");
            }
            else
            {
                foreach (var item in otherItems)
                {
                    string itemName = TextHelper.CapitalizeFirst(item);
                    AnsiConsole.MarkupLine($"  - {Markup.Escape(itemName)}");
                }
            }
        }

        private void EquipFromInventoryMenu(Player player)
        {
            var equipmentItems = player.Inventory
                .Where(item => EquipmentData.AllEquipment.ContainsKey(item.ToLower()))
                .ToList();

            if (equipmentItems.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]No equipment in inventory to equip.[/]");
                return;
            }

            AnsiConsole.MarkupLine("\n[#75C8FF]Select item to equip:[/]");
            for (int i = 0; i < equipmentItems.Count; i++)
            {
                var equipment = EquipmentData.GetEquipment(equipmentItems[i]);
                AnsiConsole.MarkupLine($"{i + 1}. {equipment.Name} ({equipment.Slot})");
            }
            AnsiConsole.MarkupLine("0. Cancel");

            Console.Write("\nChoice: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > equipmentItems.Count)
            {
                return;
            }

            if (choice == 0) return;

            string itemName = equipmentItems[choice - 1];
            var newEquipment = EquipmentData.GetEquipment(itemName);

            // Show comparison with currently equipped item
            var currentEquipment = player.GetEquipmentInSlot(newEquipment.Slot);

            AnsiConsole.MarkupLine($"\n[#00FFFF]{newEquipment.Name}[/] ({newEquipment.Slot})");
            ShowEquipmentStats(newEquipment);

            if (currentEquipment != null)
            {
                AnsiConsole.MarkupLine($"\n[#808080]Currently Equipped:[/] {currentEquipment.Name}");
                ShowEquipmentStats(currentEquipment);
                AnsiConsole.MarkupLine("");
                ShowStatComparison(currentEquipment, newEquipment);
            }

            Console.Write("\nEquip this item? (y/n): ");
            string confirm = Console.ReadLine()?.ToLower();

            if (confirm == "y" || confirm == "yes")
            {
                var oldEquipment = player.EquipItem(newEquipment);
                player.Inventory.Remove(itemName);

                if (oldEquipment != null)
                {
                    player.Inventory.Add(oldEquipment.Name.ToLower());
                    AnsiConsole.MarkupLine($"\nYou unequip {oldEquipment.Name} and equip {newEquipment.Name}.");
                }
                else
                {
                    AnsiConsole.MarkupLine($"\nYou equip {newEquipment.Name}.");
                }
            }
        }

        private void UnequipFromInventoryMenu(Player player)
        {
            var equippedSlots = new List<(EquipmentSlot slot, Equipment equipment)>();

            if (player.EquippedWeapon != null)
                equippedSlots.Add((EquipmentSlot.Weapon, player.EquippedWeapon));
            if (player.EquippedArmor != null)
                equippedSlots.Add((EquipmentSlot.Armor, player.EquippedArmor));
            if (player.EquippedHelm != null)
                equippedSlots.Add((EquipmentSlot.Helm, player.EquippedHelm));
            if (player.EquippedRing != null)
                equippedSlots.Add((EquipmentSlot.Ring, player.EquippedRing));

            if (equippedSlots.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]No equipment to unequip.[/]");
                return;
            }

            AnsiConsole.MarkupLine("\n[#75C8FF]Select item to unequip:[/]");
            for (int i = 0; i < equippedSlots.Count; i++)
            {
                AnsiConsole.MarkupLine($"{i + 1}. {equippedSlots[i].equipment.Name} ({equippedSlots[i].slot})");
            }
            AnsiConsole.MarkupLine("0. Cancel");

            Console.Write("\nChoice: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > equippedSlots.Count)
            {
                return;
            }

            if (choice == 0) return;

            var (slot, equipment) = equippedSlots[choice - 1];

            // Unequip
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    player.EquippedWeapon = null;
                    break;
                case EquipmentSlot.Armor:
                    player.EquippedArmor = null;
                    break;
                case EquipmentSlot.Helm:
                    player.EquippedHelm = null;
                    break;
                case EquipmentSlot.Ring:
                    player.EquippedRing = null;
                    break;
            }

            player.Inventory.Add(equipment.Name.ToLower());
            AnsiConsole.MarkupLine($"\nYou unequip {equipment.Name}.");
        }

        private void DropFromInventoryMenu(Player player)
        {
            if (player.Inventory.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]Inventory is empty.[/]");
                return;
            }

            AnsiConsole.MarkupLine("\n[#75C8FF]Select item to drop:[/]");
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                string itemName = TextHelper.CapitalizeFirst(player.Inventory[i]);
                AnsiConsole.MarkupLine($"{i + 1}. {itemName}");
            }
            AnsiConsole.MarkupLine("0. Cancel");

            Console.Write("\nChoice: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > player.Inventory.Count)
            {
                return;
            }

            if (choice == 0) return;

            string droppedItem = player.Inventory[choice - 1];

            Console.Write($"\nAre you sure you want to drop {TextHelper.CapitalizeFirst(droppedItem)}? (y/n): ");
            string confirm = Console.ReadLine()?.ToLower();

            if (confirm == "y" || confirm == "yes")
            {
                player.Inventory.RemoveAt(choice - 1);
                AnsiConsole.MarkupLine($"\nYou drop the {droppedItem}.");
            }
        }

        private void UseFromInventoryMenu(Player player)
        {
            var consumables = player.Inventory.Where(item =>
                context.ItemDescriptions.Values.Any(room =>
                    room.ContainsKey(item) && room[item].IsConsumable)).ToList();

            if (consumables.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]No consumables to use.[/]");
                return;
            }

            AnsiConsole.MarkupLine("\n[#75C8FF]Select consumable to use:[/]");
            for (int i = 0; i < consumables.Count; i++)
            {
                var itemData = context.ItemDescriptions.Values
                    .Where(room => room.ContainsKey(consumables[i]))
                    .Select(room => room[consumables[i]])
                    .FirstOrDefault();

                if (itemData != null && context.Effects.ContainsKey(itemData.EffectId))
                {
                    var effect = context.Effects[itemData.EffectId];
                    AnsiConsole.MarkupLine($"{i + 1}. {TextHelper.CapitalizeFirst(consumables[i])} - {effect.Description}");
                }
            }
            AnsiConsole.MarkupLine("0. Cancel");

            Console.Write("\nChoice: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > consumables.Count)
            {
                return;
            }

            if (choice == 0) return;

            // Use ItemManager's existing logic
            string useCommand = $"use {consumables[choice - 1]}";
            var itemManager = new ItemManager(context);
            itemManager.HandleUseCommand(useCommand);

        }

        private void ShowEquipmentStats(Equipment equipment)
        {
            var stats = new List<string>();

            if (equipment.DiceCount > 0)
            {
                int avgDamage = (equipment.DiceCount * (equipment.DiceSides + 1) / 2) + equipment.Bonus;
                stats.Add($"Damage: {equipment.DiceString} (avg: {avgDamage})");
            }
            if (equipment.HealthBonus != 0)
                stats.Add($"Health: +{equipment.HealthBonus}");
            if (equipment.EnergyBonus != 0)
                stats.Add($"Energy: +{equipment.EnergyBonus}");
            if (equipment.AttackBonus != 0)
                stats.Add($"Attack: +{equipment.AttackBonus}");
            if (equipment.DefenseBonus != 0)
                stats.Add($"Defense: +{equipment.DefenseBonus}");
            if (equipment.SpeedBonus != 0)
                stats.Add($"Speed: {(equipment.SpeedBonus > 0 ? "+" : "")}{equipment.SpeedBonus}");

            foreach (var stat in stats)
            {
                AnsiConsole.MarkupLine($"  {stat}");
            }

            if (stats.Count == 0)
            {
                AnsiConsole.MarkupLine("  [dim]No stat bonuses[/]");
            }
        }

        private void ShowStatComparison(Equipment current, Equipment newEq)
        {
            AnsiConsole.MarkupLine("[#FFFF00]Change:[/]");

            if (current.DiceCount > 0 || newEq.DiceCount > 0)
            {
                int currentAvg = (current.DiceCount * (current.DiceSides + 1) / 2) + current.Bonus;
                int newAvg = (newEq.DiceCount * (newEq.DiceSides + 1) / 2) + newEq.Bonus;
                int diff = newAvg - currentAvg;
                if (diff != 0)
                {
                    string color = diff > 0 ? "#00FF00" : "#FF0000";
                    AnsiConsole.MarkupLine($"  Damage: [{color}]{(diff > 0 ? "+" : "")}{diff}[/]");
                }
            }

            ShowStatDifference("Health", current.HealthBonus, newEq.HealthBonus);
            ShowStatDifference("Energy", current.EnergyBonus, newEq.EnergyBonus);
            ShowStatDifference("Attack", current.AttackBonus, newEq.AttackBonus);
            ShowStatDifference("Defense", current.DefenseBonus, newEq.DefenseBonus);
            ShowStatDifference("Speed", current.SpeedBonus, newEq.SpeedBonus);
        }

        private void ShowStatDifference(string statName, int currentValue, int newValue)
        {
            int diff = newValue - currentValue;
            if (diff != 0)
            {
                string color = diff > 0 ? "#00FF00" : "#FF0000";
                AnsiConsole.MarkupLine($"  {statName}: [{color}]{(diff > 0 ? "+" : "")}{diff}[/]");
            }
        }

        private void DisplayPartyMemberCard(Character character, string displayName, bool isPlayer)
        {
            AnsiConsole.MarkupLine("");

            // Header
            string className = character.Class?.Name ?? "Fighter";
            AnsiConsole.MarkupLine($"[#00FFFF]{displayName}[/] - {className} - Level {character.Level}");

            // Stats
            AnsiConsole.MarkupLine($"  HP: {character.Health}/{character.MaxHealth}  " +
                                  $"EP: {character.Energy}/{character.MaxEnergy}  " +
                                  $"ATK: {GetAttackDisplay(character)}  " +
                                  $"DEF: {character.Defense}  " +
                                  $"SPD: {character.Speed}");

            // Equipment box
            AnsiConsole.MarkupLine("  ┌─────────────────────────────────────────────┐");

            DisplayEquippedSlotCompact(character, EquipmentSlot.Weapon, "Weapon");
            DisplayEquippedSlotCompact(character, EquipmentSlot.Armor, "Armor");
            DisplayEquippedSlotCompact(character, EquipmentSlot.Helm, "Helm");
            DisplayEquippedSlotCompact(character, EquipmentSlot.Ring, "Ring");

            AnsiConsole.MarkupLine("  └─────────────────────────────────────────────┘");
        }

        private void DisplayEquippedSlotCompact(Character character, EquipmentSlot slot, string slotName)
        {
            var equipment = character.GetEquipmentInSlot(slot);
            string slotDisplay = $"│ {slotName}:".PadRight(11);

            if (equipment != null)
            {
                string equipName = equipment.Name;
                if (equipName.Length > 30) equipName = equipName.Substring(0, 27) + "...";

                string stats = "";
                if (equipment.DiceCount > 0)
                    stats = $"({equipment.DiceString})";
                else if (equipment.DefenseBonus > 0)
                    stats = $"(DEF+{equipment.DefenseBonus})";
                else if (equipment.HealthBonus > 0)
                    stats = $"(HP+{equipment.HealthBonus})";

                string fullText = $"{equipName} {stats}".PadRight(34);
                AnsiConsole.MarkupLine($"  {slotDisplay} {fullText}│");
            }
            else
            {
                AnsiConsole.MarkupLine($"  {slotDisplay} {"Empty".PadRight(34)}│");
            }
        }

        private string GetAttackDisplay(Character character)
        {
            if (character.EquippedWeapon != null)
            {
                return character.EquippedWeapon.DiceString;
            }
            return "1d4+1";
        }

        private void ManagePartyMemberEquipment(Character character, Player player)
        {
            bool inSubmenu = true;

            while (inSubmenu)
            {
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine($"[#FA935F]═══════════════════════════════════════════════════════════════════[/]");
                AnsiConsole.MarkupLine($"[#FA935F]                    Managing {character.Name}'s Equipment                    [/]");
                AnsiConsole.MarkupLine($"[#FA935F]═══════════════════════════════════════════════════════════════════[/]");
                AnsiConsole.MarkupLine("");

                // Show current equipment
                AnsiConsole.MarkupLine("[#90FF90]═══ Current Equipment ═══[/]");
                DisplayEquippedSlot(character, EquipmentSlot.Weapon, "Weapon");
                DisplayEquippedSlot(character, EquipmentSlot.Armor, "Armor");
                DisplayEquippedSlot(character, EquipmentSlot.Helm, "Helm");
                DisplayEquippedSlot(character, EquipmentSlot.Ring, "Ring");

                // Show available equipment in inventory
                var equipmentItems = player.Inventory
                    .Where(item => EquipmentData.AllEquipment.ContainsKey(item.ToLower()))
                    .ToList();

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine("[#90FF90]═══ Available Equipment in Inventory ═══[/]");

                if (equipmentItems.Count == 0)
                {
                    AnsiConsole.MarkupLine("[dim]  No equipment available[/]");
                }
                else
                {
                    for (int i = 0; i < equipmentItems.Count; i++)
                    {
                        var equipment = EquipmentData.GetEquipment(equipmentItems[i]);
                        AnsiConsole.Markup($"  {i + 1}. [#00FFFF]{equipment.Name}[/] ({equipment.Slot})");

                        var stats = new List<string>();
                        if (equipment.DiceCount > 0)
                            stats.Add($"DMG: {equipment.DiceString}");
                        if (equipment.DefenseBonus != 0)
                            stats.Add($"DEF+{equipment.DefenseBonus}");
                        if (equipment.HealthBonus != 0)
                            stats.Add($"HP+{equipment.HealthBonus}");

                        if (stats.Count > 0)
                            AnsiConsole.MarkupLine($" - [dim]{string.Join(", ", stats)}[/]");
                        else
                            AnsiConsole.MarkupLine("");
                    }
                }

                // Menu
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine("[#75C8FF]═══ Options ═══[/]");
                AnsiConsole.MarkupLine("1. Equip Item");
                AnsiConsole.MarkupLine("2. Unequip Item");
                AnsiConsole.MarkupLine("0. Back");

                Console.Write("\nChoice: ");
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        EquipItemOnPartyMember(character, player);
                        break;
                    case 2:
                        UnequipItemFromPartyMember(character, player);
                        break;
                    case 0:
                        inSubmenu = false;
                        break;
                }
            }
        }

        private void EquipItemOnPartyMember(Character character, Player player)
        {
            var equipmentItems = player.Inventory
                .Where(item => EquipmentData.AllEquipment.ContainsKey(item.ToLower()))
                .ToList();

            if (equipmentItems.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]No equipment in inventory.[/]");
                return;
            }

            AnsiConsole.MarkupLine("\n[#75C8FF]Select item to equip:[/]");
            for (int i = 0; i < equipmentItems.Count; i++)
            {
                var equipment = EquipmentData.GetEquipment(equipmentItems[i]);
                AnsiConsole.MarkupLine($"{i + 1}. {equipment.Name} ({equipment.Slot})");
            }
            AnsiConsole.MarkupLine("0. Cancel");

            Console.Write("\nChoice: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > equipmentItems.Count)
            {
                return;
            }

            if (choice == 0) return;

            string itemName = equipmentItems[choice - 1];
            var newEquipment = EquipmentData.GetEquipment(itemName);

            // Show what's changing
            var currentEquipment = character.GetEquipmentInSlot(newEquipment.Slot);

            AnsiConsole.MarkupLine($"\n[#00FFFF]{newEquipment.Name}[/] ({newEquipment.Slot})");
            ShowEquipmentStats(newEquipment);

            if (currentEquipment != null)
            {
                AnsiConsole.MarkupLine($"\n[#808080]Currently Equipped:[/] {currentEquipment.Name}");
                ShowEquipmentStats(currentEquipment);
                AnsiConsole.MarkupLine("");
                ShowStatComparison(currentEquipment, newEquipment);
            }

            Console.Write($"\nEquip on {character.Name}? (y/n): ");
            string confirm = Console.ReadLine()?.ToLower();

            if (confirm == "y" || confirm == "yes")
            {
                var oldEquipment = character.EquipItem(newEquipment);
                player.Inventory.Remove(itemName);

                if (oldEquipment != null)
                {
                    player.Inventory.Add(oldEquipment.Name.ToLower());
                    AnsiConsole.MarkupLine($"\n{character.Name} unequips {oldEquipment.Name} and equips {newEquipment.Name}.");
                }
                else
                {
                    AnsiConsole.MarkupLine($"\n{character.Name} equips {newEquipment.Name}.");
                }
            }
        }

        private void UnequipItemFromPartyMember(Character character, Player player)
        {
            var equippedSlots = new List<(EquipmentSlot slot, Equipment equipment)>();

            if (character.EquippedWeapon != null)
                equippedSlots.Add((EquipmentSlot.Weapon, character.EquippedWeapon));
            if (character.EquippedArmor != null)
                equippedSlots.Add((EquipmentSlot.Armor, character.EquippedArmor));
            if (character.EquippedHelm != null)
                equippedSlots.Add((EquipmentSlot.Helm, character.EquippedHelm));
            if (character.EquippedRing != null)
                equippedSlots.Add((EquipmentSlot.Ring, character.EquippedRing));

            if (equippedSlots.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[yellow]No equipment to unequip.[/]");
                return;
            }

            AnsiConsole.MarkupLine("\n[#75C8FF]Select item to unequip:[/]");
            for (int i = 0; i < equippedSlots.Count; i++)
            {
                AnsiConsole.MarkupLine($"{i + 1}. {equippedSlots[i].equipment.Name} ({equippedSlots[i].slot})");
            }
            AnsiConsole.MarkupLine("0. Cancel");

            Console.Write("\nChoice: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 0 || choice > equippedSlots.Count)
            {
                return;
            }

            if (choice == 0) return;

            var (slot, equipment) = equippedSlots[choice - 1];

            // Unequip
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    character.EquippedWeapon = null;
                    break;
                case EquipmentSlot.Armor:
                    character.EquippedArmor = null;
                    break;
                case EquipmentSlot.Helm:
                    character.EquippedHelm = null;
                    break;
                case EquipmentSlot.Ring:
                    character.EquippedRing = null;
                    break;
            }

            player.Inventory.Add(equipment.Name.ToLower());
            AnsiConsole.MarkupLine($"\n{character.Name} unequips {equipment.Name}.");
        }

    }
}