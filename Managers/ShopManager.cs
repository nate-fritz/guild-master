using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;
using GuildMaster.Data;
using GuildMaster.Helpers;

namespace GuildMaster.Managers
{
    public class ShopManager
    {
        private readonly GameContext context;
        private readonly UIManager uiManager;

        // State for non-blocking shop
        private NPC? currentVendor;
        private string currentShopState = "main"; // "main", "buy", "sell"
        private bool isInShop = false;
        private List<KeyValuePair<string, int>>? currentBuyList;
        private List<string>? currentSellList;

        public ShopManager(GameContext gameContext, UIManager uiManagerInstance)
        {
            context = gameContext;
            uiManager = uiManagerInstance;
        }

        public bool IsInShop => isInShop;

        private string GetDottedLine(string itemName, string price, int totalWidth = 67)
        {
            // Format: " 1. Item Name ................ 50 gold"
            // Calculate available space for dots (accounting for leading space, number, dot, space, item name, space, price)
            int dotsSpace = totalWidth - itemName.Length - price.Length - 4; // 4 accounts for " . " before item and space before price
            string dots = new string('.', System.Math.Max(1, dotsSpace));
            return $"{itemName} {dots} {price}";
        }

        public void StartShop(NPC vendor)
        {
            if (vendor == null || !vendor.IsVendor)
            {
                AnsiConsole.MarkupLine("[#FF0000]This NPC is not a vendor.[/]");
                return;
            }

            currentVendor = vendor;
            isInShop = true;
            currentShopState = "main";

            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            // Main shop menu
            AnsiConsole.MarkupLine("\n═══════════════════════════════════════════════════════════════════");
            AnsiConsole.MarkupLine($"                     Trading with {currentVendor.Name}");
            AnsiConsole.MarkupLine($"                          Your Gold: {context.Player.Gold}");
            AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine(" 1. Buy Items");
            AnsiConsole.MarkupLine(" 2. Sell Items");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine(" Enter a number to choose, or press 0 to exit.");
            AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
        }

        public bool ProcessShopInput(string input)
        {
            if (!isInShop)
                return false;

            input = input?.Trim() ?? "";

            if (currentShopState == "main")
            {
                return ProcessMainMenuInput(input);
            }
            else if (currentShopState == "buy")
            {
                return ProcessBuyMenuInput(input);
            }
            else if (currentShopState == "sell")
            {
                return ProcessSellMenuInput(input);
            }

            return true;
        }

        private bool ProcessMainMenuInput(string input)
        {
            if (input == "0" || string.IsNullOrEmpty(input))
            {
                AnsiConsole.MarkupLine($"\n[dim]You leave {currentVendor.Name}'s shop.[/]");
                EndShop();
                return false; // Return false to allow game engine to show status bar
            }
            else if (input == "1")
            {
                currentShopState = "buy";
                ShowBuyMenuInternal();
                return true;
            }
            else if (input == "2")
            {
                currentShopState = "sell";
                ShowSellMenuInternal();
                return true;
            }
            else
            {
                AnsiConsole.MarkupLine("\n[#FF0000]Invalid choice. Please enter 1, 2, or 0 to leave.[/]");
                ShowMainMenu(); // Redisplay the menu
                return true;
            }
        }

        private void EndShop()
        {
            isInShop = false;
            currentVendor = null;
            currentShopState = "main";
            currentBuyList = null;
            currentSellList = null;
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

        private void ShowBuyMenuInternal()
        {
            AnsiConsole.MarkupLine("\n═══════════════════════════════════════════════════════════════════");
            AnsiConsole.MarkupLine($"                     {currentVendor.Name} - Buy Items");
            AnsiConsole.MarkupLine($"                          Your Gold: {context.Player.Gold}");
            AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
            AnsiConsole.MarkupLine("");

            if (currentVendor.ShopInventory.Count == 0)
            {
                AnsiConsole.MarkupLine(" [dim]The vendor has nothing for sale right now.[/]");
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine(" Press Enter to return");
                AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
                currentBuyList = new List<KeyValuePair<string, int>>();
                return;
            }

            // Display items for sale
            int index = 1;
            currentBuyList = currentVendor.ShopInventory.ToList();
            foreach (var kvp in currentBuyList)
            {
                string itemName = kvp.Key;
                int price = kvp.Value;

                // Check if item exists in EquipmentData (weapons, armor, etc)
                string displayName;
                if (EquipmentData.AllEquipment.ContainsKey(itemName.ToLower()))
                {
                    var equipment = EquipmentData.GetEquipment(itemName);
                    displayName = equipment.Name;
                }
                else
                {
                    // Item is not equipment (potions, quest items, etc), just capitalize it
                    displayName = TextHelper.CapitalizeFirst(itemName);
                }

                string priceText = $"{price} gold";
                string dottedLine = GetDottedLine(displayName, priceText);

                AnsiConsole.MarkupLine($" {index}. {dottedLine}");
                index++;
            }

            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine(" Enter number to buy, or press 0 to return.");
            AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
        }

        private bool ProcessBuyMenuInput(string input)
        {
            if (input == "0" || string.IsNullOrEmpty(input))
            {
                currentShopState = "main";
                ShowMainMenu();
                return true;
            }

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= currentBuyList.Count)
            {
                var selectedItem = currentBuyList[choice - 1];
                BuyItem(selectedItem.Key, selectedItem.Value);
                ShowBuyMenuInternal(); // Redisplay the buy menu
                return true;
            }
            else
            {
                AnsiConsole.MarkupLine("\n[#FF0000]Invalid choice.[/]");
                return true;
            }
        }

        private void BuyItem(string itemName, int price)
        {
            if (context.Player.Gold < price)
            {
                AnsiConsole.MarkupLine($"\n[#FF0000]You don't have enough gold. You need {price}g but only have {context.Player.Gold}g.[/]");
                return;
            }

            context.Player.Gold -= price;
            context.Player.Inventory.Add(itemName);

            // Get proper display name for the purchase message
            string displayName;
            if (EquipmentData.AllEquipment.ContainsKey(itemName.ToLower()))
            {
                var equipment = EquipmentData.GetEquipment(itemName);
                displayName = equipment.Name;
            }
            else
            {
                displayName = TextHelper.CapitalizeFirst(itemName);
            }

            AnsiConsole.MarkupLine($"\n[#00FF00]You purchased {displayName} for {price}g![/]");
            AnsiConsole.MarkupLine($"[dim]Gold remaining: {context.Player.Gold}g[/]");
        }

        private void ShowSellMenuInternal()
        {
            AnsiConsole.MarkupLine("\n═══════════════════════════════════════════════════════════════════");
            AnsiConsole.MarkupLine($"                     {currentVendor.Name} - Sell Items");
            AnsiConsole.MarkupLine($"                          Your Gold: {context.Player.Gold}");
            AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
            AnsiConsole.MarkupLine("");

            // Get unique items and filter to only equipment (items that exist in EquipmentData)
            var allUniqueItems = context.Player.Inventory.Distinct().ToList();
            var equipmentItems = allUniqueItems
                .Where(item => EquipmentData.AllEquipment.ContainsKey(item.ToLower()))
                .ToList();

            if (equipmentItems.Count == 0)
            {
                AnsiConsole.MarkupLine(" [dim]You have no equipment to sell.[/]");
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine(" Press Enter to return");
                AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
                currentSellList = new List<string>();
                return;
            }

            // Build the sell list with unique items grouped properly
            var itemGroups = new Dictionary<string, int>();
            foreach (var item in equipmentItems)
            {
                if (!itemGroups.ContainsKey(item))
                {
                    itemGroups[item] = context.Player.Inventory.Count(i => i == item);
                }
            }

            // Display equipment items
            int index = 1;
            currentSellList = itemGroups.Keys.ToList();
            foreach (var itemName in currentSellList)
            {
                int count = itemGroups[itemName];
                var equipment = EquipmentData.GetEquipment(itemName);

                string displayName = equipment != null ? equipment.Name : itemName;
                int sellPrice = CalculateSellPrice(equipment);
                string priceText = $"{sellPrice} gold (you have: {count})";
                string dottedLine = GetDottedLine(displayName, priceText);

                AnsiConsole.MarkupLine($" {index}. {dottedLine}");
                index++;
            }

            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine(" Enter number to sell, or press 0 to return.");
            AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
        }

        private bool ProcessSellMenuInput(string input)
        {
            if (input == "0" || string.IsNullOrEmpty(input))
            {
                currentShopState = "main";
                ShowMainMenu();
                return true;
            }

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= currentSellList.Count)
            {
                string selectedItem = currentSellList[choice - 1];
                SellItem(selectedItem);
                ShowSellMenuInternal(); // Redisplay the sell menu
                return true;
            }
            else
            {
                AnsiConsole.MarkupLine("\n[#FF0000]Invalid choice.[/]");
                return true;
            }
        }

        private void SellItem(string itemName)
        {
            if (!context.Player.Inventory.Contains(itemName))
            {
                AnsiConsole.MarkupLine($"\n[#FF0000]You don't have {itemName} in your inventory.[/]");
                return;
            }

            var equipment = EquipmentData.GetEquipment(itemName);
            int sellPrice = CalculateSellPrice(equipment);

            context.Player.Inventory.Remove(itemName);
            context.Player.Gold += sellPrice;

            string displayName = equipment.Name;
            AnsiConsole.MarkupLine($"\n[#00FF00]You sold {displayName} for {sellPrice}g![/]");
            AnsiConsole.MarkupLine($"[dim]Gold: {context.Player.Gold}g[/]");
        }

        private int CalculateSellPrice(Equipment equipment)
        {
            if (equipment == null)
                return 1;

            // Calculate base value from equipment bonuses
            int baseValue = 10;
            baseValue += equipment.AttackBonus * 5;
            baseValue += equipment.DefenseBonus * 5;
            baseValue += equipment.SpeedBonus * 3;
            baseValue += equipment.HealthBonus * 2;
            baseValue += equipment.EnergyBonus * 2;

            // Apply vendor's buyback multiplier
            return (int)(baseValue * currentVendor.BuybackMultiplier);
        }
    }
}
