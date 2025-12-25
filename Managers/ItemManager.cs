using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;
using GuildMaster.Helpers;
using GuildMaster.Data;

namespace GuildMaster.Managers
{
    public class ItemManager
    {
        private readonly GameContext context;
        private Random random => ProgramStatics.Random;

        public ItemManager(GameContext gameContext)
        {
            context = gameContext;
        }

        public void HandleTakeCommand(string input)
        {
            var player = context.Player;
            var currentRoom = context.Rooms[player.CurrentRoom];
            var itemDescriptions = context.ItemDescriptions;

            string[] parts = input.Split(' ', 2);
            if (parts.Length < 2)
            {
                AnsiConsole.MarkupLine("Take what?");
                return;
            }

            string itemToTake = parts[1].ToLower();

            // Handle "take all" command
            if (itemToTake == "all")
            {
                var lootableItems = new List<string>();

                // Find all lootable items in the room
                foreach (string item in currentRoom.Items)
                {
                    if (itemDescriptions.ContainsKey(currentRoom.NumericId) &&
                        itemDescriptions[currentRoom.NumericId].ContainsKey(item) &&
                        itemDescriptions[currentRoom.NumericId][item].IsLootable)
                    {
                        lootableItems.Add(item);
                    }
                }

                if (lootableItems.Count == 0)
                {
                    AnsiConsole.MarkupLine("There's nothing here you can take.");
                    return;
                }

                // Take all lootable items
                foreach (string item in lootableItems)
                {
                    player.Inventory.Add(item);
                    currentRoom.Items.Remove(item);
                    player.TakenItems.Add($"room{currentRoom.NumericId}_{item}");
                    AnsiConsole.MarkupLine($"You take the {item}.");
                }

                AnsiConsole.MarkupLine($"\nYou took {lootableItems.Count} item(s).");
                return;
            }

            // Try to find item by exact match or short name
            string matchedItem = null;
            foreach (string item in currentRoom.Items)
            {
                // Check exact match
                if (item.ToLower() == itemToTake)
                {
                    matchedItem = item;
                    break;
                }

                // Check short name match
                if (itemDescriptions.ContainsKey(currentRoom.NumericId) &&
                    itemDescriptions[currentRoom.NumericId].ContainsKey(item) &&
                    !string.IsNullOrEmpty(itemDescriptions[currentRoom.NumericId][item].ShortName) &&
                    itemDescriptions[currentRoom.NumericId][item].ShortName.ToLower() == itemToTake)
                {
                    matchedItem = item;
                    break;
                }
            }

            if (matchedItem != null)
            {
                if (itemDescriptions.ContainsKey(currentRoom.NumericId) &&
                    itemDescriptions[currentRoom.NumericId].ContainsKey(matchedItem) &&
                    !itemDescriptions[currentRoom.NumericId][matchedItem].IsLootable)
                {
                    AnsiConsole.MarkupLine($"You can't take the {matchedItem}.");
                    return;
                }

                player.Inventory.Add(matchedItem);
                currentRoom.Items.Remove(matchedItem);
                player.TakenItems.Add($"room{currentRoom.NumericId}_{matchedItem}");
                AnsiConsole.MarkupLine($"\nYou take the {matchedItem}.");

                if (player.TakenItems.Count == 1)  // First item ever taken
                {
                    ProgramStatics.messageManager.CheckAndShowMessage("first_item_pickup");
                }
                return;
            }

            // No match found
            AnsiConsole.MarkupLine($"You don't see a {itemToTake} here.");
        }

        public void HandleEquipCommand(string input)
        {
            var player = context.Player;
            string[] parts = input.Split(' ', 2);

            if (parts.Length < 2)
            {
                AnsiConsole.MarkupLine("Equip what?");
                return;
            }

            string itemInput = parts[1].ToLower().Trim();
            string itemName = null;

            // Get list of equipment items in inventory
            var equipmentItems = player.Inventory
                .Where(item => EquipmentData.AllEquipment.ContainsKey(item.ToLower()))
                .ToList();

            if (equipmentItems.Count == 0)
            {
                AnsiConsole.MarkupLine("You don't have any equipment to equip.");
                return;
            }

            // Check if input is a number (1-based index)
            if (int.TryParse(itemInput, out int index) && index >= 1 && index <= equipmentItems.Count)
            {
                itemName = equipmentItems[index - 1];
            }
            else
            {
                // Try to find item by exact match or short name
                string matchedItem = null;
                foreach (string item in equipmentItems)
                {
                    // Check exact match
                    if (item.ToLower() == itemInput)
                    {
                        matchedItem = item;
                        break;
                    }

                    // Check short name match
                    var equipmentCheck = EquipmentData.GetEquipment(item);
                    if (equipmentCheck != null &&
                        !string.IsNullOrEmpty(equipmentCheck.ShortName) &&
                        equipmentCheck.ShortName.ToLower() == itemInput)
                    {
                        matchedItem = item;
                        break;
                    }
                }

                if (matchedItem != null)
                {
                    itemName = matchedItem;
                }
                else
                {
                    AnsiConsole.MarkupLine($"You don't have any equipment matching '{itemInput}'.");
                    return;
                }
            }

            // Try to get the equipment from EquipmentData
            var equipment = EquipmentData.GetEquipment(itemName);

            if (equipment == null)
            {
                AnsiConsole.MarkupLine($"You can't equip the {itemName}.");
                return;
            }

            // Equip the item and get the old equipment back
            var oldEquipment = player.EquipItem(equipment);

            // Remove new equipment from inventory
            player.Inventory.Remove(itemName);

            // Add old equipment to inventory (if there was one)
            if (oldEquipment != null)
            {
                player.Inventory.Add(oldEquipment.Name.ToLower());
                AnsiConsole.MarkupLine($"\nYou unequip your {oldEquipment.Name} and equip the {equipment.Name}.");
            }
            else
            {
                AnsiConsole.MarkupLine($"\nYou equip the {equipment.Name}.");
            }

            // Show stat changes
            AnsiConsole.MarkupLine($"[dim]Type 'stats' to see your updated character sheet.[/]");
        }

        public void HandleUnequipCommand(string input)
        {
            var player = context.Player;
            string[] parts = input.Split(' ', 2);

            if (parts.Length < 2)
            {
                AnsiConsole.MarkupLine("Unequip what? (weapon, armor, helm, ring)");
                return;
            }

            string slotInput = parts[1].ToLower().Trim();
            EquipmentSlot? slot = null;

            // Parse slot name
            if (slotInput == "weapon" || slotInput == "w")
            {
                slot = EquipmentSlot.Weapon;
            }
            else if (slotInput == "armor" || slotInput == "a")
            {
                slot = EquipmentSlot.Armor;
            }
            else if (slotInput == "helm" || slotInput == "helmet" || slotInput == "h")
            {
                slot = EquipmentSlot.Helm;
            }
            else if (slotInput == "ring" || slotInput == "r")
            {
                slot = EquipmentSlot.Ring;
            }
            else
            {
                AnsiConsole.MarkupLine($"Invalid equipment slot '{slotInput}'. Valid slots: weapon, armor, helm, ring");
                return;
            }

            // Get the currently equipped item in that slot
            var equippedItem = player.GetEquipmentInSlot(slot.Value);

            if (equippedItem == null)
            {
                AnsiConsole.MarkupLine($"You don't have anything equipped in your {slot.Value.ToString().ToLower()} slot.");
                return;
            }

            // Unequip it (equip null to clear the slot)
            switch (slot.Value)
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

            // Add to inventory
            player.Inventory.Add(equippedItem.Name.ToLower());

            AnsiConsole.MarkupLine($"\nYou unequip your {equippedItem.Name}.");
            AnsiConsole.MarkupLine($"[dim]Type 'stats' to see your updated character sheet.[/]");
        }

        public void HandleUseCommand(string input)
        {
            var player = context.Player;
            string[] parts = input.Split(' ');
            if (parts.Length < 2)
            {
                AnsiConsole.MarkupLine("Use what?");
                return;
            }

            string itemName;
            string targetName = null;

            // Check for "use [item] on [target]" format
            int onIndex = Array.IndexOf(parts, "on");
            if (onIndex > 0 && onIndex < parts.Length - 1)
            {
                itemName = string.Join(" ", parts.Skip(1).Take(onIndex - 1)).ToLower();
                targetName = string.Join(" ", parts.Skip(onIndex + 1)).ToLower();
            }
            else
            {
                itemName = string.Join(" ", parts.Skip(1)).ToLower();
            }

            if (!player.Inventory.Contains(itemName))
            {
                AnsiConsole.MarkupLine($"You don't have a {itemName}.");
                return;
            }

            // Find the item's effect
            Item itemData = null;
            string effectId = null;

            foreach (var roomItems in context.ItemDescriptions.Values)
            {
                if (roomItems.ContainsKey(itemName))
                {
                    itemData = roomItems[itemName];
                    if (itemData.IsConsumable && !string.IsNullOrEmpty(itemData.EffectId))
                    {
                        effectId = itemData.EffectId;
                        break;
                    }
                }
            }

            if (itemData == null || effectId == null)
            {
                AnsiConsole.MarkupLine($"You can't use the {itemName}.");
                return;
            }

            if (!context.Effects.ContainsKey(effectId))
            {
                AnsiConsole.MarkupLine($"The {itemName} doesn't seem to work properly.");
                return;
            }

            var effect = context.Effects[effectId];

            if (!effect.IsUsableOutsideCombat)
            {
                AnsiConsole.MarkupLine($"The {itemName} can only be used in combat.");
                return;
            }

            // Determine target
            Character target = player;
            string targetDisplayName = "You";

            if (!string.IsNullOrEmpty(targetName))
            {
                if (targetName == "me" || targetName == "self" || targetName == "myself")
                {
                    target = player;
                    targetDisplayName = "You";
                }
                else
                {
                    var ally = player.ActiveParty.FirstOrDefault(a =>
                        a.Name.ToLower() == targetName ||
                        a.Name.ToLower().Contains(targetName));

                    if (ally == null)
                    {
                        AnsiConsole.MarkupLine($"No party member named '{targetName}' found.");
                        if (player.ActiveParty.Count > 0)
                        {
                            AnsiConsole.MarkupLine($"Active party members: {string.Join(", ", player.ActiveParty.Select(a => a.Name))}");
                        }
                        return;
                    }
                    target = ally;
                    targetDisplayName = ally.Name;
                }
            }

            // For party-wide effects, always use player as origin
            if (effect.TargetsParty)
            {
                ApplyPartyEffect(itemName, effect);
            }
            else
            {
                ApplyItemEffect(itemName, effect, target, targetDisplayName);
            }

            player.Inventory.Remove(itemName);
        }

        private void ApplyItemEffect(string itemName, Effect effect, Character target, string targetDisplayName)
        {
            var player = context.Player;

            switch (effect.Type)
            {
                case EffectType.Heal:
                    int healAmount = RollDice(effect.DiceCount, effect.DiceSides, effect.Bonus);
                    int actualHeal = Math.Min(healAmount, target.MaxHealth - target.Health);
                    target.Health += actualHeal;

                    if (target == player)
                    {
                        AnsiConsole.MarkupLine($"\n[#90FF90]You drink the {itemName}![/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"\n[#90FF90]You give the {itemName} to {targetDisplayName}![/]");
                    }

                    AnsiConsole.MarkupLine($"(Rolled {effect.DiceCount}d{effect.DiceSides}+{effect.Bonus} for {healAmount} healing)");

                    if (actualHeal > 0)
                    {
                        AnsiConsole.MarkupLine($"{targetDisplayName} restored [#90FF90]{actualHeal} health[/]! (HP: {target.Health}/{target.MaxHealth})");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"{targetDisplayName} already at full health!");
                    }
                    break;

                case EffectType.RestoreEnergy:
                    int energyAmount = RollDice(effect.DiceCount, effect.DiceSides, effect.Bonus);
                    int actualEnergy = Math.Min(energyAmount, target.MaxEnergy - target.Energy);
                    target.Energy += actualEnergy;

                    if (target == player)
                    {
                        AnsiConsole.MarkupLine($"\n[#75C8FF]You drink the {itemName}![/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"\n[#75C8FF]You give the {itemName} to {targetDisplayName}![/]");
                    }

                    AnsiConsole.MarkupLine($"(Rolled {effect.DiceCount}d{effect.DiceSides}+{effect.Bonus} for {energyAmount} energy)");

                    if (actualEnergy > 0)
                    {
                        AnsiConsole.MarkupLine($"{targetDisplayName} restored [#75C8FF]{actualEnergy} energy[/]! (EP: {target.Energy}/{target.MaxEnergy})");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"{targetDisplayName} already at full energy!");
                    }
                    break;
            }
        }

        private void ApplyPartyEffect(string itemName, Effect effect)
        {
            var player = context.Player;

            switch (effect.Type)
            {
                case EffectType.PartyRestore:
                    AnsiConsole.MarkupLine($"\n[#fff394]You read the {itemName} aloud![/]");
                    AnsiConsole.MarkupLine("The scroll's runes glow brightly as healing magic washes over the party!");
                    AnsiConsole.MarkupLine("");

                    // Heal player
                    int pHeal = RollDice(effect.DiceCount, effect.DiceSides, effect.Bonus);
                    int pEnergy = RollDice(effect.DiceCount, effect.DiceSides, effect.Bonus);
                    int actualPHeal = Math.Min(pHeal, player.MaxHealth - player.Health);
                    int actualPEnergy = Math.Min(pEnergy, player.MaxEnergy - player.Energy);
                    player.Health += actualPHeal;
                    player.Energy += actualPEnergy;

                    if (actualPHeal > 0 || actualPEnergy > 0)
                    {
                        AnsiConsole.Markup("You: ");
                        if (actualPHeal > 0) AnsiConsole.Markup($"+[#90FF90]{actualPHeal} HP[/] ");
                        if (actualPEnergy > 0) AnsiConsole.Markup($"+[#75C8FF]{actualPEnergy} EP[/]");
                        AnsiConsole.MarkupLine($" (HP: {player.Health}/{player.MaxHealth}, EP: {player.Energy}/{player.MaxEnergy})");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("You: Already at full health and energy!");
                    }

                    // Heal party members
                    foreach (var ally in player.ActiveParty)
                    {
                        int aHeal = RollDice(effect.DiceCount, effect.DiceSides, effect.Bonus);
                        int aEnergy = RollDice(effect.DiceCount, effect.DiceSides, effect.Bonus);
                        int actualAHeal = Math.Min(aHeal, ally.MaxHealth - ally.Health);
                        int actualAEnergy = Math.Min(aEnergy, ally.MaxEnergy - ally.Energy);
                        ally.Health += actualAHeal;
                        ally.Energy += actualAEnergy;

                        if (actualAHeal > 0 || actualAEnergy > 0)
                        {
                            AnsiConsole.Markup($"{ally.Name}: ");
                            if (actualAHeal > 0) AnsiConsole.Markup($"+[#90FF90]{actualAHeal} HP[/] ");
                            if (actualAEnergy > 0) AnsiConsole.Markup($"+[#75C8FF]{actualAEnergy} EP[/]");
                            AnsiConsole.MarkupLine($" (HP: {ally.Health}/{ally.MaxHealth}, EP: {ally.Energy}/{ally.MaxEnergy})");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine($"{ally.Name}: Already at full health and energy!");
                        }
                    }

                    AnsiConsole.MarkupLine("\nThe scroll crumbles to dust.");
                    break;
            }
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

    }
}