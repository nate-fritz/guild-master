using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;
using GuildMaster.Data;
using GuildMaster.Helpers;

namespace GuildMaster.Managers
{
    public class QuestManager
    {
        private readonly GameContext context;
        private Random random = new Random();

        public QuestManager(GameContext gameContext)
        {
            context = gameContext;
        }

        public void ManageQuests()
        {
            var player = context.Player;

            AnsiConsole.MarkupLine("[#ac00db]    ╔════════════════════════════════════════════════════════╗[/]");
            AnsiConsole.MarkupLine("[#9700c1]    ║   ██████╗  ██╗   ██╗███████╗███████╗████████╗███████╗  ║[/]");
            AnsiConsole.MarkupLine("[#8500ab]    ║  ██╔═══██╗ ██║   ██║██╔════╝██╔════╝╚══██╔══╝██╔════╝  ║[/]");
            AnsiConsole.MarkupLine("[#730096]    ║  ██║   ██║ ██║   ██║█████╗  ███████╗   ██║   ███████╗  ║[/]");
            AnsiConsole.MarkupLine("[#620081]    ║  ██║   ██║ ██║   ██║██╔══╝  ╚════██║   ██║   ╚════██║  ║[/]");
            AnsiConsole.MarkupLine("[#51006c]    ║  ╚██████▀▄╗╚██████╔╝███████╗███████║   ██║   ███████║  ║[/]");
            AnsiConsole.MarkupLine("[#420057]    ║   ╚════╝ ╚╝ ╚═════╝ ╚══════╝╚══════╝   ╚═╝   ╚══════╝  ║[/]");
            AnsiConsole.MarkupLine("[#350046]    ╚════════════════════════════════════════════════════════╝[/]");

            CheckCompletedQuests();

            AnsiConsole.MarkupLine("\n=== Active Quests ===");
            if (player.ActiveQuests.Count == 0)
            {
                AnsiConsole.MarkupLine("No active quests.");
            }
            else
            {
                foreach (var quest in player.ActiveQuests)
                {
                    float timeRemaining = (quest.StartDay * 24 + quest.StartTime + quest.Duration) -
                                         (player.CurrentDay * 24 + player.CurrentHour);
                    string status = timeRemaining > 0 ?
                        $"{timeRemaining:F1} hours remaining" :
                        "COMPLETE - Return to collect rewards!";
                    AnsiConsole.MarkupLine($"- {quest.Name} ({quest.AssignedRecruit.Name}) - {status}");
                }
            }

            var availableRecruits = player.Recruits.Where(r =>
                !player.ActiveParty.Contains(r) &&
                !r.IsOnQuest &&
                !r.IsResting).ToList();

            AnsiConsole.MarkupLine($"\n=== Available Recruits ({availableRecruits.Count}) ===");
            if (availableRecruits.Count == 0)
            {
                AnsiConsole.MarkupLine("No recruits available for quests.");
                if (player.Recruits.Any(r => r.IsResting))
                {
                    foreach (var resting in player.Recruits.Where(r => r.IsResting))
                    {
                        float restRemaining = (resting.RestUntilDay * 24 + resting.RestUntil) -
                                            (player.CurrentDay * 24 + player.CurrentHour);
                        AnsiConsole.MarkupLine($"- {resting.Name} is resting ({restRemaining:F1} hours remaining)");
                    }
                }
            }
            else
            {
                foreach (var recruit in availableRecruits)
                {
                    AnsiConsole.MarkupLine($"- {recruit.Name} ({recruit.Class})");
                }
            }

            AnsiConsole.MarkupLine("\n=== Options ===");
            if (availableRecruits.Count > 0)
            {
                AnsiConsole.MarkupLine("1. Send Recruit on Quest");
            }
            if (player.ActiveQuests.Any(q => q.IsComplete))
            {
                AnsiConsole.MarkupLine("2. Collect Quest Rewards");
            }
            AnsiConsole.MarkupLine("0. Back");

            Console.Write("\nChoice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    if (availableRecruits.Count > 0)
                        SendRecruitOnQuest(availableRecruits);
                    break;
                case "2":
                    if (player.ActiveQuests.Any(q => q.IsComplete))
                        CollectQuestRewards();
                    break;
                case "0":
                    return;  // Return to guild menu
            }

            ManageQuests();  // Loop back to quest menu
        }

        private void SendRecruitOnQuest(List<Recruit> availableRecruits)
        {
            var player = context.Player;

            AnsiConsole.MarkupLine("\nSelect recruit to send:");
            for (int i = 0; i < availableRecruits.Count; i++)
            {
                AnsiConsole.MarkupLine($"{i + 1}. {availableRecruits[i].Name} ({availableRecruits[i].Class})");
            }
            AnsiConsole.MarkupLine("0. Cancel");

            Console.Write("Choice: ");
            if (!int.TryParse(Console.ReadLine(), out int recruitChoice) ||
                recruitChoice < 0 || recruitChoice > availableRecruits.Count)
            {
                return;
            }

            if (recruitChoice == 0) return;

            var selectedRecruit = availableRecruits[recruitChoice - 1];

            var quests = QuestData.GetAvailableQuests();
            AnsiConsole.MarkupLine($"\n=== Available Quests for {selectedRecruit.Name} ===");

            for (int i = 0; i < quests.Count; i++)
            {
                var q = quests[i];
                AnsiConsole.MarkupLine($"\n{i + 1}. {q.Name} [{q.Difficulty}]");
                AnsiConsole.MarkupLine($"   {q.Description}");
                AnsiConsole.MarkupLine($"   Duration: {q.Duration} hours | Gold: {q.MinGold}-{q.MaxGold}");
                AnsiConsole.MarkupLine($"   Success Chance: ~{q.BaseSuccessChance}%");
            }
            AnsiConsole.MarkupLine("\n0. Cancel");

            Console.Write("\nSelect quest: ");
            if (!int.TryParse(Console.ReadLine(), out int questChoice) ||
                questChoice < 0 || questChoice > quests.Count)
            {
                return;
            }

            if (questChoice == 0) return;

            var selectedQuest = quests[questChoice - 1];

            selectedQuest.AssignedRecruit = selectedRecruit;
            selectedQuest.StartDay = player.CurrentDay;
            selectedQuest.StartTime = player.CurrentHour;
            selectedQuest.IsActive = true;

            selectedRecruit.IsOnQuest = true;
            player.ActiveQuests.Add(selectedQuest);

            AnsiConsole.MarkupLine($"\n{selectedRecruit.Name} has embarked on: {selectedQuest.Name}!");
            AnsiConsole.MarkupLine($"They will return in {selectedQuest.Duration} hours.");
            AnsiConsole.MarkupLine("\nPress Enter to continue...");
            Console.ReadLine();
        }

        public void CheckCompletedQuests()
        {
            var player = context.Player;

            foreach (var quest in player.ActiveQuests)
            {
                float questEndTime = (quest.StartDay * 24 + quest.StartTime + quest.Duration);
                float currentTime = (player.CurrentDay * 24 + player.CurrentHour);

                if (currentTime >= questEndTime && !quest.IsComplete)
                {
                    quest.IsComplete = true;

                    int roll = random.Next(1, 101);
                    quest.WasSuccessful = roll <= quest.BaseSuccessChance;
                }
            }
        }

        private void CollectQuestRewards()
        {
            var player = context.Player;
            var completedQuests = player.ActiveQuests.Where(q => q.IsComplete).ToList();

            foreach (var quest in completedQuests)
            {
                AnsiConsole.MarkupLine($"\n=== {quest.Name} Complete ===");
                AnsiConsole.MarkupLine($"{quest.AssignedRecruit.Name} has returned!");

                if (quest.WasSuccessful)
                {
                    AnsiConsole.MarkupLine("[#00FF00]Quest Successful![/]");

                    int gold = random.Next(quest.MinGold, quest.MaxGold + 1);
                    player.Gold += gold;
                    AnsiConsole.MarkupLine($"Earned: {gold} gold");

                    // Award XP
                    quest.AssignedRecruit.Experience += quest.BaseExperienceReward;
                    AnsiConsole.MarkupLine($"{quest.AssignedRecruit.Name} gained [#00FFFF]{quest.BaseExperienceReward} experience[/]!");

                    // Check for level up
                    if (quest.AssignedRecruit.CheckLevelUp())
                    {
                        AnsiConsole.MarkupLine($"\n[#FFD700]★ LEVEL UP! {quest.AssignedRecruit.Name} is now level {quest.AssignedRecruit.Level}! ★[/]");
                        quest.AssignedRecruit.ApplyLevelUpBonuses();
                        AnsiConsole.MarkupLine($"HP: {quest.AssignedRecruit.MaxHealth} | EP: {quest.AssignedRecruit.MaxEnergy} | ATK: {quest.AssignedRecruit.AttackDamage} | DEF: {quest.AssignedRecruit.Defense}");
                    }

                    foreach (var reward in quest.ItemRewards)
                    {
                        if (random.Next(100) < reward.Value)
                        {
                            player.Inventory.Add(reward.Key);
                            AnsiConsole.MarkupLine($"Found: {TextHelper.CapitalizeFirst(reward.Key)}");
                        }
                    }

                    if (!string.IsNullOrEmpty(quest.PotentialRecruit))
                    {
                        int recruitChance = quest.Difficulty == "Easy" ? 10 :
                                          quest.Difficulty == "Medium" ? 20 : 40;
                        if (random.Next(100) < recruitChance)
                        {
                            AnsiConsole.MarkupLine($"[#fff394]New recruit found: {quest.PotentialRecruit}![/]");
                            player.Recruits.Add(new Recruit(quest.PotentialRecruit, "Fighter", player.CurrentDay));
                        }
                    }

                    quest.AssignedRecruit.IsResting = true;
                    quest.AssignedRecruit.RestUntil = player.CurrentHour + 4;
                    quest.AssignedRecruit.RestUntilDay = player.CurrentDay;

                    // Handle day rollover
                    if (quest.AssignedRecruit.RestUntil >= 24)
                    {
                        quest.AssignedRecruit.RestUntil -= 24;
                        quest.AssignedRecruit.RestUntilDay++;
                    }

                    AnsiConsole.MarkupLine($"{quest.AssignedRecruit.Name} will rest for 4 hours.");
                }
                else
                {
                    AnsiConsole.MarkupLine("[#FF0000]Quest Failed![/]");
                    // Half XP on failure
                    quest.AssignedRecruit.Experience += quest.FailedExperienceReward;
                    AnsiConsole.MarkupLine($"{quest.AssignedRecruit.Name} gained [#00FFFF]{quest.FailedExperienceReward} experience[/] from the attempt.");

                    // Check for level up even on failure
                    if (quest.AssignedRecruit.CheckLevelUp())
                    {
                        AnsiConsole.MarkupLine($"\n[#FFD700]★ LEVEL UP! {quest.AssignedRecruit.Name} is now level {quest.AssignedRecruit.Level}! ★[/]");
                        quest.AssignedRecruit.ApplyLevelUpBonuses();
                        AnsiConsole.MarkupLine($"HP: {quest.AssignedRecruit.MaxHealth} | EP: {quest.AssignedRecruit.MaxEnergy} | ATK: {quest.AssignedRecruit.AttackDamage} | DEF: {quest.AssignedRecruit.Defense}");
                    }

                    AnsiConsole.MarkupLine("No rewards earned.");

                    quest.AssignedRecruit.IsResting = true;
                    quest.AssignedRecruit.RestUntil = player.CurrentHour + 16;
                    quest.AssignedRecruit.RestUntilDay = player.CurrentDay;

                    // Handle day rollover
                    while (quest.AssignedRecruit.RestUntil >= 24)
                    {
                        quest.AssignedRecruit.RestUntil -= 24;
                        quest.AssignedRecruit.RestUntilDay++;
                    }

                    AnsiConsole.MarkupLine($"{quest.AssignedRecruit.Name} needs 16 hours to recover from the failure.");
                }

                quest.AssignedRecruit.IsOnQuest = false;
                player.ActiveQuests.Remove(quest);
            }

            AnsiConsole.MarkupLine("\nPress Enter to continue...");
            Console.ReadLine();
        }
    }
}