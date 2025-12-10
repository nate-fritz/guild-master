using GuildMaster.Services;
using Console = GuildMaster.Services.Console;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using GuildMaster.Models;

namespace GuildMaster.Managers
{
    public class MessageManager
    {
        private readonly GameContext context;
        private readonly UIManager uiManager;
        private HashSet<string> shownMessages = new HashSet<string>();

        // Different message types for different formatting
        public enum MessageType
        {
            Tutorial,    // Blue box - gameplay hints
            Narrative,   // Gold box - story beats
            Achievement, // Green box - milestones reached
            Hint        // Dim text - subtle reminders
        }

        public MessageManager(GameContext gameContext, UIManager uiManagerInstance)
        {
            context = gameContext;
            uiManager = uiManagerInstance;
        }

        public void CheckAndShowMessage(string triggerId, string customParam = null)
        {
            // Some messages should only show once ever
            if (shownMessages.Contains(triggerId))
                return;

            var message = GetMessage(triggerId, customParam);
            if (message != null)
            {
                DisplayMessage(message.Value.Item1, message.Value.Item2, triggerId);
                shownMessages.Add(triggerId);

                // Note: Thread.Sleep removed for web compatibility - Blazor WASM runs on UI thread
            }
        }

        private (string, MessageType)? GetMessage(string triggerId, string customParam = null)
        {
            var player = context.Player;

            return triggerId switch
            {
                // Container/Item tutorials
                "game_start_commands" =>
                    ("Type [cyan]look note[/] (or [cyan]l note[/]) to read the note.",
                     MessageType.Tutorial),

                "first_movement_tutorial" =>
                    ("To move into another room, type the direction ([cyan]north[/] or [cyan]n[/], [cyan]east[/] or [cyan]e[/], [cyan]south[/] or [cyan]s[/], [cyan]west[/] or [cyan]w[/]).\n\n To look around the room for other items or exits, type [cyan]look around[/], [cyan]look[/], or just [cyan]l[/] \n\nTo see a list of other available commands, type [cyan]/help[/]",
                     MessageType.Tutorial),

                "first_container" =>
                    ("This object contained a new item within. Try taking a [cyan]look[/] around the room to see what came out.  Then, you can [cyan]take[/] the item or even [cyan]take all[/] loose items in the room.",
                     MessageType.Tutorial),

                "found_quest_item_amulet" =>
                    ("You've found a quest item! Use [cyan]take amulet[/] to pick it up. Someone in these woods mentioned losing an amulet...",
                     MessageType.Tutorial),

                "first_item_pickup" =>
                    ("Items you take are added to your inventory. Use [cyan]inventory[/] or [cyan]i[/] to see what you're carrying.",
                     MessageType.Tutorial),

                // Combat tutorials
                "first_combat_victory" =>
                    ("Well fought! Combat rewards include gold and sometimes items. Make sure to [cyan]rest[/] if your health is low.",
                     MessageType.Tutorial),

                "low_health_warning" =>
                    ("Your health is dangerously low! Use [cyan]rest[/] to restore your health, or use a [cyan]potion[/] if you have one.",
                     MessageType.Tutorial),

                // Recruitment tutorials
                "first_party_member" =>
                    ($"{customParam} is now fighting alongside you! Your party members will help in combat and can be managed using the [cyan]party[/] or [cyan]p[/] command.",
                     MessageType.Tutorial),

                "guild_management_unlocked" =>
                    ("Your party is full! Additional recruits will wait at the guild hall. Use [cyan]guild[/] or [cyan]g[/] to manage your guild, swap party members, and send idle recruits on quests.",
                     MessageType.Tutorial),

                "party_full" =>
                    ("Your active party is full (3 members max)! Additional recruits can still help by going on quests. Visit the guild menu to manage quests.",
                     MessageType.Tutorial),

                "quest_system_unlock" =>
                    ("With recruits not in your active party, you can now send them on quests! Use the guild menu to assign quests and earn passive rewards.",
                     MessageType.Tutorial),

                // Narrative beats
                "halfway_to_goal" =>
                    ("Five brave souls have joined your cause. The guild hall no longer feels so empty. Word of your growing guild spreads through the land...",
                     MessageType.Narrative),

                "near_goal" =>
                    ("Eight members strong! The Adventurer's Guild is nearly restored to its former glory. You've almost proven yourself worthy...",
                     MessageType.Narrative),

                "tenth_recruit" =>
                    ("THE ADVENTURER'S GUILD IS RESTORED! Ten brave adventurers now call this hall home. Songs will be sung of your leadership!",
                     MessageType.Achievement),

                "first_100_gold" =>
                    ("You've amassed 100 gold pieces! Your guild's coffers are beginning to fill. Keep building your wealth!",
                     MessageType.Achievement),

                "win_conditions_met" =>
                    ("VICTORY! You've rebuilt the guild to its former glory - 10 recruits strong and 1000 gold in the coffers! The old Guildmaster would be proud!",
                     MessageType.Achievement),

                // Hints
                "hint_talk_to_npcs" =>
                    ("[dim]Hint: Some NPCs may join your guild if you help them or prove yourself worthy.[/]",
                     MessageType.Hint),

                "hint_explore_more" =>
                    ("[dim]The forest depths and mountain paths hold both danger and opportunity. Explore when you're ready.[/]",
                     MessageType.Hint),

                _ => null
            };
        }

        private void DisplayMessage(string text, MessageType type, string triggerId)
        {
            AnsiConsole.MarkupLine("");

            switch (type)
            {
                case MessageType.Tutorial:
                    AnsiConsole.MarkupLine("[#FA935F]═══════════════════════════════════════════════════════════════════[/]");
                    AnsiConsole.MarkupLine("[#FA935F]                            TUTORIAL                               [/]");
                    AnsiConsole.MarkupLine("[#FA935F]═══════════════════════════════════════════════════════════════════[/]");
                    WrapAndDisplay(text, "#FA935F");
                    AnsiConsole.MarkupLine("[#FA935F]═══════════════════════════════════════════════════════════════════[/]");
                    break;

                case MessageType.Narrative:
                    AnsiConsole.MarkupLine("[#FFD700]═══════════════════════════════════════════════════════════════════[/]");
                    WrapAndDisplay(text, "#FFD700");
                    AnsiConsole.MarkupLine("[#FFD700]═══════════════════════════════════════════════════════════════════[/]");
                    break;

                case MessageType.Achievement:
                    AnsiConsole.MarkupLine("[#90FF90]★ ═══════════════════════ ACHIEVEMENT ══════════════════════ ★[/]");
                    AnsiConsole.MarkupLine("");
                    WrapAndDisplay(text, "#90FF90");
                    AnsiConsole.MarkupLine("");
                    AnsiConsole.MarkupLine("[#90FF90]★ ═══════════════════════════════════════════════════════════ ★[/]");
                    break;

                case MessageType.Hint:
                    AnsiConsole.MarkupLine($"[dim]{text}[/]");
                    break;
            }

            // Display status bar after tutorial messages, but only for specific ones
            // that don't happen within commands (commands show status bar on completion)
            if (type == MessageType.Tutorial && (triggerId == "game_start_commands" || triggerId == "first_movement_tutorial"))
            {
                AnsiConsole.MarkupLine("");
                uiManager.DisplayStats();
            }
            else
            {
                AnsiConsole.MarkupLine("");
            }
        }

        private void WrapAndDisplay(string text, string color)
        {
            var wrapped = WrapTextForDisplay(text, 67);
            foreach (var line in wrapped)
            {
                AnsiConsole.MarkupLine($"[{color}]{line}[/]");
            }
        }

        private List<string> WrapTextForDisplay(string text, int width)
        {
            var lines = new List<string>();

            // Split by paragraph breaks first
            var paragraphs = text.Split(new[] { "\n\n", "\r\n\r\n" }, StringSplitOptions.None);

            foreach (var paragraph in paragraphs)
            {
                // Handle each paragraph separately
                var words = paragraph.Split(' ');
                var currentLine = "";

                foreach (var word in words)
                {
                    var testLine = currentLine.Length == 0 ? word : currentLine + " " + word;
                    // Remove markup for length calculation
                    var cleanTest = System.Text.RegularExpressions.Regex.Replace(testLine, @"\[[^\]]*\]", "");

                    if (cleanTest.Length <= width)
                    {
                        currentLine = testLine;
                    }
                    else
                    {
                        if (currentLine.Length > 0)
                            lines.Add(currentLine);
                        currentLine = word;
                    }
                }

                if (currentLine.Length > 0)
                    lines.Add(currentLine);

                // Add blank line between paragraphs (except after last one)
                if (paragraph != paragraphs[paragraphs.Length - 1])
                    lines.Add("");
            }

            return lines;
        }

        public HashSet<string> GetShownMessages() => new HashSet<string>(shownMessages);
        public void SetShownMessages(HashSet<string> messages) => shownMessages = new HashSet<string>(messages);
    }
}