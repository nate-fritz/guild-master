using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;
using GuildMaster.Data;

namespace GuildMaster.Managers
{
    public class ShopManager
    {
        private readonly GameContext context;
        private readonly UIManager uiManager;

        public ShopManager(GameContext gameContext, UIManager uiManagerInstance)
        {
            context = gameContext;
            uiManager = uiManagerInstance;
        }

        private string GetDottedLine(string itemName, string price, int totalWidth = 67)
        {
            // Format: " 1. Item Name ................ 50 gold"
            // Calculate available space for dots (accounting for leading space, number, dot, space, item name, space, price)
            int dotsSpace = totalWidth - itemName.Length - price.Length - 4; // 4 accounts for " . " before item and space before price
            string dots = new string('.', System.Math.Max(1, dotsSpace));
            return $"{itemName} {dots} {price}";
        }

        public void ShowShop(NPC vendor)
        {
            if (vendor == null || !vendor.IsVendor)
            {
                AnsiConsole.MarkupLine("[#FF0000]This NPC is not a vendor.[/]");
                return;
            }

            while (true)
            {
                // Main shop menu
                AnsiConsole.MarkupLine("\n═══════════════════════════════════════════════════════════════════");
                AnsiConsole.MarkupLine($"                     Trading with {vendor.Name}");
                AnsiConsole.MarkupLine($"                          Your Gold: {context.Player.Gold}");
                AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine(" 1. Buy Items");
                AnsiConsole.MarkupLine(" 2. Sell Items");
                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine(" Enter a number to choose, or press Enter to exit.");
                AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
                AnsiConsole.MarkupLine("");

                string input = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrEmpty(input))
                {
                    AnsiConsole.MarkupLine($"\n[dim]You leave {vendor.Name}'s shop.[/]");
                    break;
                }
                else if (input == "1")
                {
                    ShowBuyMenu(vendor);
                }
                else if (input == "2")
                {
                    ShowSellMenu(vendor);
                }
                else
                {
                    AnsiConsole.MarkupLine("\n[#FF0000]Invalid choice. Please enter 1, 2, or press Enter to leave.[/]");
                }
            }
        }

        private void ShowBuyMenu(NPC vendor)
        {
            while (true)
            {
                AnsiConsole.MarkupLine("\n═══════════════════════════════════════════════════════════════════");
                AnsiConsole.MarkupLine($"                     {vendor.Name} - Buy Items");
                AnsiConsole.MarkupLine($"                          Your Gold: {context.Player.Gold}");
                AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
                AnsiConsole.MarkupLine("");

                if (vendor.ShopInventory.Count == 0)
                {
                    AnsiConsole.MarkupLine(" [dim]The vendor has nothing for sale right now.[/]");
                    AnsiConsole.MarkupLine("");
                    AnsiConsole.MarkupLine(" Press Enter to return.");
                    AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
                    Console.ReadLine();
                    return;
                }

                // Display items for sale
                int index = 1;
                var itemList = vendor.ShopInventory.ToList();
                foreach (var kvp in itemList)
                {
                    string itemName = kvp.Key;
                    int price = kvp.Value;
                    var equipment = EquipmentData.GetEquipment(itemName);

                    string displayName = equipment != null ? equipment.Name : itemName;
                    string priceText = $"{price} gold";
                    string dottedLine = GetDottedLine(displayName, priceText);

                    AnsiConsole.MarkupLine($" {index}. {dottedLine}");
                    index++;
                }

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine(" Enter number to buy, or press Enter to return.");
                AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
                AnsiConsole.MarkupLine("");

                string input = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrEmpty(input))
                {
                    return;
                }

                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= itemList.Count)
                {
                    var selectedItem = itemList[choice - 1];
                    BuyItem(vendor, selectedItem.Key, selectedItem.Value);
                }
                else
                {
                    AnsiConsole.MarkupLine("\n[#FF0000]Invalid choice.[/]");
                }
            }
        }

        private void BuyItem(NPC vendor, string itemName, int price)
        {
            if (context.Player.Gold < price)
            {
                AnsiConsole.MarkupLine($"\n[#FF0000]You don't have enough gold. You need {price}g but only have {context.Player.Gold}g.[/]");
                return;
            }

            context.Player.Gold -= price;
            context.Player.Inventory.Add(itemName);

            AnsiConsole.MarkupLine($"\n[#00FF00]You purchased {itemName} for {price}g![/]");
            AnsiConsole.MarkupLine($"[dim]Gold remaining: {context.Player.Gold}g[/]");
        }

        private void ShowSellMenu(NPC vendor)
        {
            while (true)
            {
                AnsiConsole.MarkupLine("\n═══════════════════════════════════════════════════════════════════");
                AnsiConsole.MarkupLine($"                     {vendor.Name} - Sell Items");
                AnsiConsole.MarkupLine($"                          Your Gold: {context.Player.Gold}");
                AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
                AnsiConsole.MarkupLine("");

                if (context.Player.Inventory.Count == 0)
                {
                    AnsiConsole.MarkupLine(" [dim]You have nothing to sell.[/]");
                    AnsiConsole.MarkupLine("");
                    AnsiConsole.MarkupLine(" Press Enter to return.");
                    AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
                    Console.ReadLine();
                    return;
                }

                // Display player's inventory
                int index = 1;
                var uniqueItems = context.Player.Inventory.Distinct().ToList();
                foreach (var itemName in uniqueItems)
                {
                    int count = context.Player.Inventory.Count(i => i == itemName);
                    var equipment = EquipmentData.GetEquipment(itemName);

                    string displayName = equipment != null ? equipment.Name : itemName;
                    int sellPrice = CalculateSellPrice(vendor, equipment);
                    string priceText = $"{sellPrice} gold (you have: {count})";
                    string dottedLine = GetDottedLine(displayName, priceText);

                    AnsiConsole.MarkupLine($" {index}. {dottedLine}");
                    index++;
                }

                AnsiConsole.MarkupLine("");
                AnsiConsole.MarkupLine(" Enter number to sell, or press Enter to return.");
                AnsiConsole.MarkupLine("═══════════════════════════════════════════════════════════════════");
                AnsiConsole.MarkupLine("");

                string input = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrEmpty(input))
                {
                    return;
                }

                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= uniqueItems.Count)
                {
                    string selectedItem = uniqueItems[choice - 1];
                    SellItem(vendor, selectedItem);
                }
                else
                {
                    AnsiConsole.MarkupLine("\n[#FF0000]Invalid choice.[/]");
                }
            }
        }

        private void SellItem(NPC vendor, string itemName)
        {
            if (!context.Player.Inventory.Contains(itemName))
            {
                AnsiConsole.MarkupLine($"\n[#FF0000]You don't have {itemName} in your inventory.[/]");
                return;
            }

            var equipment = EquipmentData.GetEquipment(itemName);
            int sellPrice = CalculateSellPrice(vendor, equipment);

            context.Player.Inventory.Remove(itemName);
            context.Player.Gold += sellPrice;

            AnsiConsole.MarkupLine($"\n[#00FF00]You sold {itemName} for {sellPrice}g![/]");
            AnsiConsole.MarkupLine($"[dim]Gold: {context.Player.Gold}g[/]");
        }

        private int CalculateSellPrice(NPC vendor, Equipment equipment)
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
            return (int)(baseValue * vendor.BuybackMultiplier);
        }
    }
}
