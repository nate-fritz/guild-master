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
        private Random random => ProgramStatics.Random;

        // State machine fields
        private bool isInQuestMenu = false;
        private string currentQuestState = "main"; // "main", "select_recruit", "select_quest", "collect_rewards"
        private List<Recruit>? currentAvailableRecruits;
        private Recruit? selectedRecruit;
        private List<Quest>? currentQuests;

        public bool IsInQuestMenu => isInQuestMenu;

        public QuestManager(GameContext gameContext)
        {
            context = gameContext;
        }

        // Start quest menu (non-blocking)
        public void StartQuestMenu()
        {
            isInQuestMenu = true;
            currentQuestState = "main";
            DisplayMainMenu();
        }

        // Process input based on current state
        public bool ProcessQuestInput(string input)
        {
            switch (currentQuestState)
            {
                case "main":
                    return ProcessMainMenuInput(input);
                case "select_recruit":
                    return ProcessSelectRecruitInput(input);
                case "select_quest":
                    return ProcessSelectQuestInput(input);
                case "collect_rewards":
                    return ProcessCollectRewardsInput(input);
                default:
                    return true;
            }
        }

        private void DisplayMainMenu()
        {
            var player = context.Player;

            // Clear resting status for recruits whose rest time has passed
            CheckRestingRecruits();

            AnsiConsole.MarkupLine("[#ac00db]       ██████╗  ██╗   ██╗███████╗███████╗████████╗███████╗  [/]");
            AnsiConsole.MarkupLine("[#9700c1]      ██╔═══██╗ ██║   ██║██╔════╝██╔════╝╚══██╔══╝██╔════╝  [/]");
            AnsiConsole.MarkupLine("[#8500ab]      ██║   ██║ ██║   ██║█████╗  ███████╗   ██║   ███████╗  [/]");
            AnsiConsole.MarkupLine("[#730096]      ██║   ██║ ██║   ██║██╔══╝  ╚════██║   ██║   ╚════██║  [/]");
            AnsiConsole.MarkupLine("[#620081]      ╚██████▀▄╗╚██████╔╝███████╗███████║   ██║   ███████║  [/]");
            AnsiConsole.MarkupLine("[#51006c]       ╚════╝ ╚╝ ╚═════╝ ╚══════╝╚══════╝   ╚═╝   ╚══════╝  [/]");

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

            currentAvailableRecruits = player.Recruits.Where(r =>
                !player.ActiveParty.Contains(r) &&
                !r.IsOnQuest &&
                !r.IsResting).ToList();

            AnsiConsole.MarkupLine($"\n=== Available Recruits ({currentAvailableRecruits.Count}) ===");
            if (currentAvailableRecruits.Count == 0)
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
                foreach (var recruit in currentAvailableRecruits)
                {
                    AnsiConsole.MarkupLine($"- {recruit.Name} ({recruit.Class?.Name ?? "Unknown"})");
                }
            }

            AnsiConsole.MarkupLine("\n=== Options ===");
            if (currentAvailableRecruits.Count > 0)
            {
                AnsiConsole.MarkupLine("1. Send Recruit on Quest");
            }
            if (player.ActiveQuests.Any(q => q.IsComplete))
            {
                AnsiConsole.MarkupLine("2. Collect Quest Rewards");
            }
            AnsiConsole.MarkupLine("0. Back");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose)[/]");
        }

        private bool ProcessMainMenuInput(string input)
        {
            var player = context.Player;

            switch (input)
            {
                case "1":
                    if (currentAvailableRecruits != null && currentAvailableRecruits.Count > 0)
                    {
                        currentQuestState = "select_recruit";
                        DisplaySelectRecruitMenu();
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("\n[dim]No recruits available.[/]");
                        DisplayMainMenu();
                    }
                    break;
                case "2":
                    if (player.ActiveQuests.Any(q => q.IsComplete))
                    {
                        currentQuestState = "collect_rewards";
                        CollectQuestRewards();
                        // After collecting, return to main menu
                        currentQuestState = "main";
                        DisplayMainMenu();
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("\n[dim]No completed quests to collect.[/]");
                        DisplayMainMenu();
                    }
                    break;
                case "0":
                    isInQuestMenu = false;
                    return false; // Exit quest menu
                default:
                    AnsiConsole.MarkupLine("\n[dim]Invalid choice. Please try again.[/]");
                    DisplayMainMenu();
                    break;
            }

            return true;
        }

        private void DisplaySelectRecruitMenu()
        {
            if (currentAvailableRecruits == null || currentAvailableRecruits.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[dim]No recruits available.[/]");
                currentQuestState = "main";
                DisplayMainMenu();
                return;
            }

            AnsiConsole.MarkupLine("\nSelect recruit to send:");
            for (int i = 0; i < currentAvailableRecruits.Count; i++)
            {
                AnsiConsole.MarkupLine($"{i + 1}. {currentAvailableRecruits[i].Name} ({currentAvailableRecruits[i].Class?.Name ?? "Unknown"})");
            }
            AnsiConsole.MarkupLine("0. Cancel");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose)[/]");
        }

        private bool ProcessSelectRecruitInput(string input)
        {
            if (input == "0")
            {
                currentQuestState = "main";
                DisplayMainMenu();
                return true;
            }

            if (currentAvailableRecruits == null || !int.TryParse(input, out int recruitChoice) ||
                recruitChoice < 1 || recruitChoice > currentAvailableRecruits.Count)
            {
                AnsiConsole.MarkupLine("\n[dim]Invalid choice. Please try again.[/]");
                DisplaySelectRecruitMenu();
                return true;
            }

            selectedRecruit = currentAvailableRecruits[recruitChoice - 1];
            currentQuestState = "select_quest";
            DisplaySelectQuestMenu();
            return true;
        }

        private void DisplaySelectQuestMenu()
        {
            if (selectedRecruit == null)
            {
                currentQuestState = "main";
                DisplayMainMenu();
                return;
            }

            currentQuests = QuestData.GetAvailableQuests();

            // Filter out completed recruit quests
            currentQuests = currentQuests.Where(q => !context.Player.CompletedQuestIds.Contains(q.Id)).ToList();

            AnsiConsole.MarkupLine($"\n=== Available Quests for {selectedRecruit.Name} ===");

            for (int i = 0; i < currentQuests.Count; i++)
            {
                var q = currentQuests[i];
                AnsiConsole.MarkupLine($"\n{i + 1}. {q.Name} [{q.Difficulty}]");
                AnsiConsole.MarkupLine($"   {q.Description}");
                AnsiConsole.MarkupLine($"   Duration: {q.Duration} hours | Gold: {q.MinGold}-{q.MaxGold}");
                AnsiConsole.MarkupLine($"   Success Chance: ~{q.BaseSuccessChance}%");
            }
            AnsiConsole.MarkupLine("\n0. Cancel");
            AnsiConsole.MarkupLine("");
            AnsiConsole.MarkupLine("[dim](Enter a number to choose)[/]");
        }

        private bool ProcessSelectQuestInput(string input)
        {
            var player = context.Player;

            if (input == "0")
            {
                currentQuestState = "main";
                DisplayMainMenu();
                return true;
            }

            if (currentQuests == null || selectedRecruit == null ||
                !int.TryParse(input, out int questChoice) ||
                questChoice < 1 || questChoice > currentQuests.Count)
            {
                AnsiConsole.MarkupLine("\n[dim]Invalid choice. Please try again.[/]");
                DisplaySelectQuestMenu();
                return true;
            }

            var selectedQuest = currentQuests[questChoice - 1];

            selectedQuest.AssignedRecruit = selectedRecruit;
            selectedQuest.StartDay = player.CurrentDay;
            selectedQuest.StartTime = player.CurrentHour;
            selectedQuest.IsActive = true;

            selectedRecruit.IsOnQuest = true;
            player.ActiveQuests.Add(selectedQuest);

            AnsiConsole.MarkupLine($"\n[#90FF90]{selectedRecruit.Name} has embarked on: {selectedQuest.Name}![/]");
            AnsiConsole.MarkupLine($"They will return in {selectedQuest.Duration} hours.");

            // Return to main menu
            currentQuestState = "main";
            DisplayMainMenu();
            return true;
        }

        private bool ProcessCollectRewardsInput(string input)
        {
            // This state is auto-processed, just return to main
            currentQuestState = "main";
            DisplayMainMenu();
            return true;
        }

        private void CheckRestingRecruits()
        {
            var player = context.Player;
            float currentTime = (player.CurrentDay * 24 + player.CurrentHour);

            foreach (var recruit in player.Recruits)
            {
                if (recruit.IsResting)
                {
                    float restEndTime = (recruit.RestUntilDay * 24 + recruit.RestUntil);

                    // If rest time has passed, clear the resting flag
                    if (currentTime >= restEndTime)
                    {
                        recruit.IsResting = false;
                    }
                }
            }
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
                        // Check if player already has this recruit
                        bool alreadyHasRecruit = player.Recruits.Any(r => r.Name == quest.PotentialRecruit);

                        if (!alreadyHasRecruit)
                        {
                            int recruitChance = quest.Difficulty == "Easy" ? 10 :
                                              quest.Difficulty == "Medium" ? 20 : 40;
                            if (random.Next(100) < recruitChance)
                            {
                                AnsiConsole.MarkupLine($"[#fff394]New recruit found: {quest.PotentialRecruit}![/]");

                                // Look up the NPC from NPCData to get their proper class
                                if (context.NPCs.ContainsKey(quest.PotentialRecruit))
                                {
                                    var npc = context.NPCs[quest.PotentialRecruit];
                                    Recruit newRecruit = new Recruit(npc.Name, npc.Class?.Name ?? "Legionnaire", player.CurrentDay, npc.Class);
                                    player.Recruits.Add(newRecruit);

                                    // Mark quest as permanently completed if it rewards a recruit
                                    if (!player.CompletedQuestIds.Contains(quest.Id))
                                    {
                                        player.CompletedQuestIds.Add(quest.Id);
                                    }
                                }
                                else
                                {
                                    // Fallback if NPC not found in data
                                    player.Recruits.Add(new Recruit(quest.PotentialRecruit, "Legionnaire", player.CurrentDay));

                                    if (!player.CompletedQuestIds.Contains(quest.Id))
                                    {
                                        player.CompletedQuestIds.Add(quest.Id);
                                    }
                                }
                            }
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
        }
    }
}