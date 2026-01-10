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
        }

        public void ShowCharacterStatsHeader()
        {
            AnsiConsole.MarkupLine("");

            AnsiConsole.MarkupLine("[#75C8FF]═══════════════════════════════════════════════════════════════════[/]");
            AnsiConsole.MarkupLine("[#75C8FF]                       CHARACTER SHEET                              [/]");
            AnsiConsole.MarkupLine("[#75C8FF]═══════════════════════════════════════════════════════════════════[/]");
        }

        public void DisplayStats()
        {
            var player = context.Player;

            int hour = (int)player.CurrentHour;
            int minutes = (int)((player.CurrentHour - hour) * 60);

            string timeOfDay = hour < 12 ? "AM" : "PM";
            int displayHour = hour > 12 ? hour - 12 : hour;
            if (displayHour == 0) displayHour = 12;

            AnsiConsole.MarkupLine($"\n<span class='stats-bar'>[HP: {player.Health}/{player.MaxHealth} | EP: {player.Energy}/{player.MaxEnergy} | Day {player.CurrentDay}, {displayHour}:{minutes:D2} {timeOfDay} | Gold: {player.Gold} | Recruits: {player.Recruits.Count}/10]</span>");
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
            AnsiConsole.MarkupLine("Trading:");
            AnsiConsole.MarkupLine("Open vendor shop:     shop [vendor] or trade [vendor]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("Other:");
            AnsiConsole.MarkupLine("Character Sheet:      stats");
            AnsiConsole.MarkupLine("Party Status:         party or p");
            AnsiConsole.MarkupLine("Guild Management:     guild or g");
            AnsiConsole.MarkupLine("Rest:                 rest");
            AnsiConsole.MarkupLine("Return to guild:      recall");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("Settings:");
            AnsiConsole.MarkupLine("Settings menu:        settings");
            AnsiConsole.MarkupLine("Toggle tutorials:     tutorials [on/off]");
            AnsiConsole.MarkupLine("Toggle autocombat:    autocombat [on/off]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("System:");
            AnsiConsole.MarkupLine("Save game:            save");
            AnsiConsole.MarkupLine("Load game:            load");
            AnsiConsole.MarkupLine("Quit the game:        qq or quit");
            AnsiConsole.MarkupLine("");
        }

        public void ShowAdminHelp()
        {
            AnsiConsole.MarkupLine("[#FC7938]                        ADMIN / DEBUG COMMANDS                        [/]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("Teleportation:");
            AnsiConsole.MarkupLine("Teleport to room:     tpto [room number]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("Item & Gold:");
            AnsiConsole.MarkupLine("Give item:            giveitem [item name]");
            AnsiConsole.MarkupLine("Give gold:            givegold [amount]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("Character:");
            AnsiConsole.MarkupLine("Set level:            setlevel [level 1-20]");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("Display:");
            AnsiConsole.MarkupLine("Toggle room numbers:  roomnumbers");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("Debug:");
            AnsiConsole.MarkupLine("View game state:      state");
            AnsiConsole.MarkupLine("View quest flags:     flags");
            AnsiConsole.MarkupLine("Set quest flag:       setflag [flag_name] [true/false]");
            AnsiConsole.MarkupLine("Toggle debug logs:    showdebug");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("Development/Testing:");
            AnsiConsole.MarkupLine("Toggle War Room:      warroom");
            AnsiConsole.MarkupLine("                      (Shows/hides War Room in Guild menu)");
            AnsiConsole.MarkupLine("");
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

        // ============================================================================
        // DISPLAY HELPER METHODS FOR PARTY STATUS
        // ============================================================================

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

    }
}