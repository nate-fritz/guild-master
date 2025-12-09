using GuildMaster.Services;
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
            AnsiConsole.MarkupLine("[#90FF90]                                    ██████╗ ██╗   ██╗██╗██╗     ██████╗                             [/]");
            AnsiConsole.MarkupLine("[#7FE67F]                                   ██╔════╝ ██║   ██║██║██║     ██╔══██╗                            [/]");
            AnsiConsole.MarkupLine("[#7FE67F]                                   ██║  ███╗██║   ██║██║██║     ██║  ██║                            [/]");
            AnsiConsole.MarkupLine("[#6FCC6F]                                   ██║   ██║██║   ██║██║██║     ██║  ██║                            [/]");
            AnsiConsole.MarkupLine("[#6FCC6F]                                   ╚██████╔╝╚██████╔╝██║███████╗██████╔╝                            [/]");
            AnsiConsole.MarkupLine("[#5FB35F]                                    ╚═════╝  ╚═════╝ ╚═╝╚══════╝╚═════╝                             [/]");
            AnsiConsole.MarkupLine("[#5FB35F]      ███╗   ███╗ █████╗ ███╗   ██╗ █████╗  ██████╗ ███████╗███╗   ███╗███████╗███╗   ██╗████████╗  [/]");
            AnsiConsole.MarkupLine("[#4F994F]      ████╗ ████║██╔══██╗████╗  ██║██╔══██╗██╔════╝ ██╔════╝████╗ ████║██╔════╝████╗  ██║╚══██╔══╝  [/]");
            AnsiConsole.MarkupLine("[#4F994F]      ██╔████╔██║███████║██╔██╗ ██║███████║██║  ███╗█████╗  ██╔████╔██║█████╗  ██╔██╗ ██║   ██║     [/]");
            AnsiConsole.MarkupLine("[#3F7F3F]      ██║╚██╔╝██║██╔══██║██║╚██╗██║██╔══██║██║   ██║██╔══╝  ██║╚██╔╝██║██╔══╝  ██║╚██╗██║   ██║     [/]");
            AnsiConsole.MarkupLine("[#3F7F3F]      ██║ ╚═╝ ██║██║  ██║██║ ╚████║██║  ██║╚██████╔╝███████╗██║ ╚═╝ ██║███████╗██║ ╚████║   ██║     [/]");
            AnsiConsole.MarkupLine("[#2F662F]      ╚═╝     ╚═╝╚═╝  ╚═╝╚═╝  ╚═══╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝╚═╝     ╚═╝╚══════╝╚═╝  ╚═══╝   ╚═╝     [/]");

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

                    AnsiConsole.MarkupLine($"{i + 1}. {recruit.Name} - {recruit.Class} - {health} - Joined Day {recruit.RecruitedDay} {status}");
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
                    AnsiConsole.MarkupLine($"- {member.Name} ({member.Class})");
                }
            }

            AnsiConsole.MarkupLine("\n=== Options ===");
            if (player.Recruits.Count > 0)
            {
                AnsiConsole.MarkupLine("1. Manage Party (Add/Remove members)");
                AnsiConsole.MarkupLine("2. View Recruit Details");
                AnsiConsole.MarkupLine("3. Manage Quests");
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
                AnsiConsole.MarkupLine("0. Back");
                return;
            }

            AnsiConsole.MarkupLine("\nWho would you like to add?");
            for (int i = 0; i < available.Count; i++)
            {
                AnsiConsole.MarkupLine($"{i + 1}. {available[i].Name} ({available[i].Class}) - HP: {available[i].Health}/{available[i].MaxHealth}");
            }
            AnsiConsole.MarkupLine("0. Back");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose)[/]");
        }

        public void DisplayRemoveFromPartyMenu()
        {
            var player = context.Player;

            if (player.ActiveParty.Count == 0)
            {
                AnsiConsole.MarkupLine("\nNo one in party to remove.");
                AnsiConsole.MarkupLine("0. Back");
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
            AnsiConsole.MarkupLine($"╔═══════════════════════════════════════╗");
            AnsiConsole.MarkupLine($"║         CHARACTER SHEET               ║");
            AnsiConsole.MarkupLine($"╠═══════════════════════════════════════╣");
            AnsiConsole.MarkupLine($"║ Name:    {recruit.Name,-28} ║");
            AnsiConsole.MarkupLine($"║ Class:   {recruit.Class,-28} ║");
            AnsiConsole.MarkupLine($"║ Status:  {GetRecruitStatus(recruit),-28} ║");
            AnsiConsole.MarkupLine($"╠═══════════════════════════════════════╣");
            AnsiConsole.MarkupLine($"║ Health:  {recruit.Health,3}/{recruit.MaxHealth,-28} ║");
            AnsiConsole.MarkupLine($"║ Energy:  {recruit.Energy,3}/{recruit.MaxEnergy,-28} ║");
            AnsiConsole.MarkupLine($"║ Attack:  1d4+{recruit.AttackDamage,-24} ║");
            AnsiConsole.MarkupLine($"║ Defense: {recruit.Defense,-28} ║");
            AnsiConsole.MarkupLine($"║ Speed:   {recruit.Speed,-28} ║");
            AnsiConsole.MarkupLine($"╠═══════════════════════════════════════╣");
            AnsiConsole.MarkupLine($"║ Recruited: Day {recruit.RecruitedDay,-22} ║");
            AnsiConsole.MarkupLine($"╚═══════════════════════════════════════╝");

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
    }
}
