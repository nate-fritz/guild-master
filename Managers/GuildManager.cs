using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;

namespace GuildMaster.Managers
{
    public class GuildManager
    {
        private readonly GameContext context;
        private readonly QuestManager questManager;
        private readonly MessageManager messageManager;

        public GuildManager(GameContext gameContext, QuestManager questMgr, MessageManager msgManager)
        {
            context = gameContext;
            questManager = questMgr;
            messageManager = msgManager;
        }

        // Display methods (non-blocking, just show menus)
        public void DisplayGuildMenu()
        {
            var player = context.Player;

            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[#90FF90]═══════════════════════════════════════════════════════════════════[/]");
            AnsiConsole.MarkupLine("[#90FF90]                         GUILD MANAGEMENT                            [/]");
            AnsiConsole.MarkupLine("[#90FF90]═══════════════════════════════════════════════════════════════════[/]");

            int daysRemaining = 365 - player.CurrentDay;
            AnsiConsole.MarkupLine($"\n=== Progress Report ===");
            AnsiConsole.MarkupLine($"Day: {player.CurrentDay}/365 ({daysRemaining} days remaining)");
            AnsiConsole.MarkupLine($"Recruits: {player.Recruits.Count}/10");
            AnsiConsole.MarkupLine($"Gold: {player.Gold}/1000");

            if (player.Gold >= 1000 && player.Recruits.Count >= 10)
            {
                AnsiConsole.MarkupLine("[#00FF00]WIN CONDITIONS MET![/]");
            }

            AnsiConsole.MarkupLine($"\n=== Guild Roster ({player.Recruits.Count} Members) ===");
            if (player.Recruits.Count == 0)
            {
                AnsiConsole.MarkupLine("No recruits yet. Get out there and build your guild!");
            }
            else
            {
                for (int i = 0; i < player.Recruits.Count; i++)
                {
                    var recruit = player.Recruits[i];
                    string status = player.ActiveParty.Contains(recruit) ? "[IN PARTY]" : "[AT GUILD]";
                    string health = $"HP: {recruit.Health}/{recruit.MaxHealth}";

                    // Show quest/rest status
                    if (recruit.IsOnQuest)
                    {
                        status = "[ON QUEST]";
                    }
                    else if (recruit.IsResting)
                    {
                        float restRemaining = (recruit.RestUntilDay * 24 + recruit.RestUntil) -
                                            (player.CurrentDay * 24 + player.CurrentHour);
                        status = $"[RESTING - {restRemaining:F1}h]";
                    }

                    AnsiConsole.MarkupLine($"{i + 1}. {recruit.Name} - {recruit.Class?.Name ?? "Unknown"} - {health} - Joined Day {recruit.RecruitedDay} {status}");
                }
            }

            AnsiConsole.MarkupLine($"\n=== Active Party ({player.ActiveParty.Count}/3 Max) ===");
            if (player.ActiveParty.Count == 0)
            {
                AnsiConsole.MarkupLine("No party members. Recruit allies to fight alongside you!");
            }
            else
            {
                foreach (var member in player.ActiveParty)
                {
                    AnsiConsole.MarkupLine($"- {member.Name} ({member.Class?.Name ?? "Unknown"})");
                }
            }

            AnsiConsole.MarkupLine("\n=== Options ===");
            if (player.Recruits.Count > 0)
            {
                AnsiConsole.MarkupLine("1. Manage Party (Add/Remove members)");
                AnsiConsole.MarkupLine("2. View Recruit Details");
                // Quest board unlocks after completing Act I
                if (context.GetQuestFlag("act_1_complete"))
                {
                    AnsiConsole.MarkupLine("3. Manage Quests");
                }
                // War Room (dev/test feature - toggle with 'warroom' command)
                if (player.WarRoomEnabled)
                {
                    AnsiConsole.MarkupLine("4. Enter War Room [yellow](DEV/TEST)[/]");
                }
            }
            AnsiConsole.MarkupLine("0. Return to Game");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose)[/]");
        }

        public void DisplayManagePartyMenu()
        {
            AnsiConsole.MarkupLine("\n=== Manage Party ===");
            AnsiConsole.MarkupLine("1. Add to Party");
            AnsiConsole.MarkupLine("2. Remove from Party");
            AnsiConsole.MarkupLine("0. Back");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose)[/]");
        }

        public bool HasRecruitsAvailableToAdd()
        {
            var player = context.Player;
            return player.Recruits.Any(r =>
                !player.ActiveParty.Contains(r) &&
                !r.IsOnQuest &&
                !r.IsResting);
        }

        public void DisplayAddToPartyMenu()
        {
            var player = context.Player;
            var available = player.Recruits.Where(r =>
                !player.ActiveParty.Contains(r) &&
                !r.IsOnQuest &&
                !r.IsResting).ToList();

            if (available.Count == 0)
            {
                AnsiConsole.MarkupLine("\nNo recruits available! They may be resting or on quests.");
                AnsiConsole.MarkupLine("\nPress Enter to go back");
                return;
            }

            AnsiConsole.MarkupLine("\nWho would you like to add?");
            for (int i = 0; i < available.Count; i++)
            {
                AnsiConsole.MarkupLine($"{i + 1}. {available[i].Name} ({available[i].Class?.Name ?? "Unknown"}) - HP: {available[i].Health}/{available[i].MaxHealth}");
            }
            AnsiConsole.MarkupLine("0. Back");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose)[/]");
        }

        public bool HasPartyMembersToRemove()
        {
            var player = context.Player;
            return player.ActiveParty.Count > 0;
        }

        public void DisplayRemoveFromPartyMenu()
        {
            var player = context.Player;

            if (player.ActiveParty.Count == 0)
            {
                AnsiConsole.MarkupLine("\nNo one in party to remove.");
                AnsiConsole.MarkupLine("\nPress Enter to go back");
                return;
            }

            AnsiConsole.MarkupLine("\nWho would you like to remove?");
            for (int i = 0; i < player.ActiveParty.Count; i++)
            {
                AnsiConsole.MarkupLine($"{i + 1}. {player.ActiveParty[i].Name}");
            }
            AnsiConsole.MarkupLine("0. Back");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose)[/]");
        }

        public void DisplayRecruitDetailsMenu()
        {
            var player = context.Player;

            AnsiConsole.MarkupLine("\nWhich recruit would you like to view?");
            for (int i = 0; i < player.Recruits.Count; i++)
            {
                AnsiConsole.MarkupLine($"{i + 1}. {player.Recruits[i].Name}");
            }
            AnsiConsole.MarkupLine("0. Back");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose)[/]");
        }

        // Process methods (handle input, return true if action completed)
        public bool ProcessAddToParty(string input)
        {
            var player = context.Player;
            var available = player.Recruits.Where(r =>
                !player.ActiveParty.Contains(r) &&
                !r.IsOnQuest &&
                !r.IsResting).ToList();

            if (!int.TryParse(input, out int idx) || idx < 1 || idx > available.Count)
            {
                AnsiConsole.MarkupLine("\n[dim]Invalid choice. Please try again.[/]");
                return false;
            }

            player.ActiveParty.Add(available[idx - 1]);
            AnsiConsole.MarkupLine($"\n[#90FF90]{available[idx - 1].Name} joined the party![/]");

            if (player.ActiveParty.Count == 1)
                messageManager.CheckAndShowMessage("party_member_added", available[idx - 1].Name);
            else if (player.ActiveParty.Count == 3)
                messageManager.CheckAndShowMessage("party_full");

            return true;
        }

        public bool ProcessRemoveFromParty(string input)
        {
            var player = context.Player;

            if (!int.TryParse(input, out int idx) || idx < 1 || idx > player.ActiveParty.Count)
            {
                AnsiConsole.MarkupLine("\n[dim]Invalid choice. Please try again.[/]");
                return false;
            }

            var removed = player.ActiveParty[idx - 1];
            player.ActiveParty.RemoveAt(idx - 1);
            AnsiConsole.MarkupLine($"\n[#90FF90]{removed.Name} left the party.[/]");
            return true;
        }

        public bool ProcessRecruitDetails(string input)
        {
            var player = context.Player;

            if (!int.TryParse(input, out int idx) || idx < 1 || idx > player.Recruits.Count)
            {
                AnsiConsole.MarkupLine("\n[dim]Invalid choice. Please try again.[/]");
                return false;
            }

            var recruit = player.Recruits[idx - 1];

            // Create a character sheet box
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"═══════════════════════════════════════");
            AnsiConsole.MarkupLine($"         CHARACTER SHEET               ");
            AnsiConsole.MarkupLine($"═══════════════════════════════════════");
            AnsiConsole.MarkupLine($" Name:    {recruit.Name,-28} ");
            AnsiConsole.MarkupLine($" Class:   {recruit.Class?.Name ?? "Unknown",-28} ");
            AnsiConsole.MarkupLine($" Status:  {GetRecruitStatus(recruit),-28} ");
            AnsiConsole.MarkupLine($"═══════════════════════════════════════");
            AnsiConsole.MarkupLine($" Health:  {recruit.Health,3}/{recruit.MaxHealth,-28} ");
            AnsiConsole.MarkupLine($" Energy:  {recruit.Energy,3}/{recruit.MaxEnergy,-28} ");
            AnsiConsole.MarkupLine($" Attack:  1d4+{recruit.AttackDamage,-24} ");
            AnsiConsole.MarkupLine($" Defense: {recruit.Defense,-28} ");
            AnsiConsole.MarkupLine($" Speed:   {recruit.Speed,-28} ");
            AnsiConsole.MarkupLine($"═══════════════════════════════════════");
            AnsiConsole.MarkupLine($" Equipment:                            ");
            AnsiConsole.MarkupLine($" Weapon:  {recruit.EquippedWeapon?.Name ?? "None",-28} ");
            AnsiConsole.MarkupLine($" Armor:   {recruit.EquippedArmor?.Name ?? "None",-28} ");
            AnsiConsole.MarkupLine($" Helm:    {recruit.EquippedHelm?.Name ?? "None",-28} ");
            AnsiConsole.MarkupLine($" Ring:    {recruit.EquippedRing?.Name ?? "None",-28} ");
            AnsiConsole.MarkupLine($"═══════════════════════════════════════");
            AnsiConsole.MarkupLine($" Recruited: Day {recruit.RecruitedDay,-22} ");
            AnsiConsole.MarkupLine($"═══════════════════════════════════════");

            return true;
        }

        private string GetRecruitStatus(Recruit recruit)
        {
            var player = context.Player;

            if (recruit.IsOnQuest)
                return "On Quest";
            else if (recruit.IsResting)
            {
                float restRemaining = (recruit.RestUntilDay * 24 + recruit.RestUntil) -
                                    (player.CurrentDay * 24 + player.CurrentHour);
                return $"Resting ({restRemaining:F1}h)";
            }
            else if (player.ActiveParty.Contains(recruit))
                return "In Party";
            else
                return "At Guild";
        }

        public void DisplayRecruitActionsMenu(Recruit recruit)
        {
            // Display character sheet first
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine($"═══════════════════════════════════════");
            AnsiConsole.MarkupLine($"         CHARACTER SHEET               ");
            AnsiConsole.MarkupLine($"═══════════════════════════════════════");
            AnsiConsole.MarkupLine($" Name:    {recruit.Name,-28} ");
            AnsiConsole.MarkupLine($" Class:   {recruit.Class?.Name ?? "Unknown",-28} ");
            AnsiConsole.MarkupLine($" Status:  {GetRecruitStatus(recruit),-28} ");
            AnsiConsole.MarkupLine($"═══════════════════════════════════════");
            AnsiConsole.MarkupLine($" Health:  {recruit.Health,3}/{recruit.MaxHealth,-28} ");
            AnsiConsole.MarkupLine($" Energy:  {recruit.Energy,3}/{recruit.MaxEnergy,-28} ");
            AnsiConsole.MarkupLine($" Attack:  1d4+{recruit.AttackDamage,-24} ");
            AnsiConsole.MarkupLine($" Defense: {recruit.Defense,-28} ");
            AnsiConsole.MarkupLine($" Speed:   {recruit.Speed,-28} ");
            AnsiConsole.MarkupLine($"═══════════════════════════════════════");
            AnsiConsole.MarkupLine($" Equipment:                            ");
            AnsiConsole.MarkupLine($" Weapon:  {recruit.EquippedWeapon?.Name ?? "None",-28} ");
            AnsiConsole.MarkupLine($" Armor:   {recruit.EquippedArmor?.Name ?? "None",-28} ");
            AnsiConsole.MarkupLine($" Helm:    {recruit.EquippedHelm?.Name ?? "None",-28} ");
            AnsiConsole.MarkupLine($" Ring:    {recruit.EquippedRing?.Name ?? "None",-28} ");
            AnsiConsole.MarkupLine($"═══════════════════════════════════════");
            AnsiConsole.MarkupLine($" Recruited: Day {recruit.RecruitedDay,-22} ");
            AnsiConsole.MarkupLine($"═══════════════════════════════════════");

            // Show action options
            AnsiConsole.MarkupLine("\n=== Actions ===");
            AnsiConsole.MarkupLine("1. Manage Equipment");
            AnsiConsole.MarkupLine("0. Back");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose)[/]");
        }

        public void DisplayRecruitEquipmentMenu(Recruit recruit)
        {
            var player = context.Player;

            AnsiConsole.MarkupLine($"\n=== Managing Equipment for {recruit.Name} ===");
            AnsiConsole.MarkupLine("\nCurrent Equipment:");
            AnsiConsole.MarkupLine($" Weapon: {recruit.EquippedWeapon?.Name ?? "None"}");
            AnsiConsole.MarkupLine($" Armor:  {recruit.EquippedArmor?.Name ?? "None"}");
            AnsiConsole.MarkupLine($" Helm:   {recruit.EquippedHelm?.Name ?? "None"}");
            AnsiConsole.MarkupLine($" Ring:   {recruit.EquippedRing?.Name ?? "None"}");

            // Get equipment from player inventory
            var equipmentItems = player.Inventory
                .Where(item => Data.EquipmentData.AllEquipment.ContainsKey(item.ToLower()))
                .ToList();

            AnsiConsole.MarkupLine("\nAvailable Equipment in Inventory:");
            if (equipmentItems.Count == 0)
            {
                AnsiConsole.MarkupLine(" [dim]No equipment available[/]");
            }
            else
            {
                for (int i = 0; i < equipmentItems.Count; i++)
                {
                    var eq = Data.EquipmentData.GetEquipment(equipmentItems[i]);
                    AnsiConsole.MarkupLine($" {i + 1}. {eq.Name} ({eq.Slot})");
                }
            }

            AnsiConsole.MarkupLine("\nActions:");
            if (equipmentItems.Count > 0)
            {
                AnsiConsole.MarkupLine(" Enter number to equip item");
            }
            AnsiConsole.MarkupLine(" Type 'unequip weapon/armor/helm/ring' to unequip");
            AnsiConsole.MarkupLine(" 0. Back");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a choice)[/]");
        }

        public void ProcessRecruitEquipmentAction(Recruit recruit, string input)
        {
            var player = context.Player;

            // Check if unequip command
            if (input.ToLower().StartsWith("unequip "))
            {
                string slot = input.Substring(8).Trim().ToLower();
                HandleRecruitUnequip(recruit, slot);
                return;
            }

            // Try to parse as equipment index
            var equipmentItems = player.Inventory
                .Where(item => Data.EquipmentData.AllEquipment.ContainsKey(item.ToLower()))
                .ToList();

            if (int.TryParse(input, out int idx) && idx >= 1 && idx <= equipmentItems.Count)
            {
                string itemName = equipmentItems[idx - 1];
                HandleRecruitEquip(recruit, itemName);
            }
            else
            {
                AnsiConsole.MarkupLine("\n[dim]Invalid choice. Please try again.[/]");
            }
        }

        private void HandleRecruitEquip(Recruit recruit, string itemName)
        {
            var player = context.Player;
            var equipment = Data.EquipmentData.GetEquipment(itemName);

            if (equipment == null)
            {
                AnsiConsole.MarkupLine($"\n[dim]Cannot equip {itemName}.[/]");
                return;
            }

            // Equip the item and get the old equipment back
            var oldEquipment = recruit.EquipItem(equipment);

            // Remove new equipment from player inventory
            player.Inventory.Remove(itemName);

            // Add old equipment back to player inventory if there was one
            if (oldEquipment != null)
            {
                player.Inventory.Add(oldEquipment.Name);
                AnsiConsole.MarkupLine($"\n[#90FF90]{recruit.Name} equipped {equipment.Name}. Returned {oldEquipment.Name} to inventory.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"\n[#90FF90]{recruit.Name} equipped {equipment.Name}.[/]");
            }
        }

        private void HandleRecruitUnequip(Recruit recruit, string slotName)
        {
            var player = context.Player;
            EquipmentSlot slot;

            switch (slotName)
            {
                case "weapon":
                    slot = EquipmentSlot.Weapon;
                    break;
                case "armor":
                    slot = EquipmentSlot.Armor;
                    break;
                case "helm":
                    slot = EquipmentSlot.Helm;
                    break;
                case "ring":
                    slot = EquipmentSlot.Ring;
                    break;
                default:
                    AnsiConsole.MarkupLine($"\n[dim]Unknown slot: {slotName}. Use weapon, armor, helm, or ring.[/]");
                    return;
            }

            var equipment = recruit.GetEquipmentInSlot(slot);
            if (equipment == null)
            {
                AnsiConsole.MarkupLine($"\n[dim]{recruit.Name} has nothing equipped in {slotName} slot.[/]");
                return;
            }

            // Unequip by equipping null
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    recruit.EquippedWeapon = null;
                    break;
                case EquipmentSlot.Armor:
                    recruit.EquippedArmor = null;
                    break;
                case EquipmentSlot.Helm:
                    recruit.EquippedHelm = null;
                    break;
                case EquipmentSlot.Ring:
                    recruit.EquippedRing = null;
                    break;
            }

            // Add to player inventory
            player.Inventory.Add(equipment.Name);
            AnsiConsole.MarkupLine($"\n[#90FF90]Unequipped {equipment.Name} from {recruit.Name}. Added to inventory.[/]");
        }
    }
}
