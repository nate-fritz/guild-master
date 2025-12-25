using GuildMaster.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildMaster.Helpers
{
    public static class TextHelper
    {
        // Flag to disable paging (for web version)
        public static bool DisablePaging { get; set; } = true;

        // Pagination manager for web version
        public static PaginationManager? PaginationManager { get; set; }

        public static string WrapText(string text, int maxLineLength = 80)
        {
            List<string> wrappedLines = new List<string>();

            // First normalize all line breaks to <br> for consistent processing
            // Replace \r\n, \n\n, and single \n with <br>
            text = text.Replace("\r\n", "<br>");
            text = text.Replace("\n\n", "<br><br>");  // Double newlines = paragraph breaks
            text = text.Replace("\n", " ");  // Single newlines become spaces (soft wrap)

            // Split by <br> tags (these are hard line breaks)
            var segments = System.Text.RegularExpressions.Regex.Split(
                text,
                @"<br\s*/?>",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i].Trim();

                // Empty segments represent consecutive <br> tags (paragraph breaks)
                if (string.IsNullOrWhiteSpace(segment))
                {
                    // Add blank line for paragraph breaks if we have content already
                    if (wrappedLines.Count > 0)
                        wrappedLines.Add(" ");  // Use space instead of empty string for better rendering
                    continue;
                }

                // Wrap this segment
                var words = segment.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var currentLine = "";

                foreach (var word in words)
                {
                    var testLine = currentLine.Length == 0 ? word : currentLine + " " + word;
                    // Remove markup for length calculation
                    var cleanTest = System.Text.RegularExpressions.Regex.Replace(testLine, @"\[[^\]]*\]", "");

                    if (cleanTest.Length <= maxLineLength)
                    {
                        currentLine = testLine;
                    }
                    else
                    {
                        if (currentLine.Length > 0)
                            wrappedLines.Add(currentLine);
                        currentLine = word;
                    }
                }

                if (currentLine.Length > 0)
                    wrappedLines.Add(currentLine);
            }

            return string.Join("\n", wrappedLines);
        }

        public static void DisplayTextWithPaging(string text, int maxLineLength = 80, int linesPerPage = 16, string color = null)
        {
            string wrappedText = WrapText(text, maxLineLength);
            string[] lines = wrappedText.Split('\n');

            // For web version with DisablePaging, use pagination manager
            if (DisablePaging && PaginationManager != null)
            {
                // If text is short enough, display all at once
                if (lines.Length <= linesPerPage)
                {
                    foreach (var line in lines)
                    {
                        DisplayLine(line, color);
                    }
                    return;
                }

                // Set up pagination and show first page
                PaginationManager.SetPaginatedContent(lines, color, linesPerPage);
                PaginationManager.ShowNextPage();
                return;
            }

            // If text is short enough, display all at once
            if (lines.Length <= linesPerPage)
            {
                foreach (var line in lines)
                {
                    DisplayLine(line, color);
                }
                return;
            }

            // Display text in pages (for console version with synchronous blocking)
            for (int i = 0; i < lines.Length; i += linesPerPage)
            {
                for (int j = i; j < Math.Min(i + linesPerPage, lines.Length); j++)
                {
                    DisplayLine(lines[j], color);
                }

                // Show continuation prompt if there are more lines
                if (i + linesPerPage < lines.Length)
                {
                    int remainingLines = lines.Length - (i + linesPerPage);
                    AnsiConsole.MarkupLine($"\n[dim][[Press Enter to continue... ({remainingLines} more lines)]][/]");
                    GuildMaster.Services.Console.ReadLine();
                    AnsiConsole.MarkupLine("");
                }
            }
        }

        public static void DisplayTextWithPaging(string text, string color)
        {
            DisplayTextWithPaging(text, 80, 16, color);
        }

        private static void DisplayLine(string line, string color)
        {
            if (!string.IsNullOrEmpty(color))
            {
                string escapedLine = line.Replace("[", "[[").Replace("]", "]]");
                AnsiConsole.MarkupLine($"[{color}]{escapedLine}[/]");
            }
            else
            {
                string escapedLine = line.Replace("[", "[[").Replace("]", "]]");
                AnsiConsole.MarkupLine(escapedLine);
            }
        }

        public static void DisplayColoredText(string text, string color, int maxLineLength = 80, int linesPerPage = 16)
        {
            DisplayTextWithPaging(text, maxLineLength, linesPerPage, color);
        }

        public static string CapitalizeFirst(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string GetArticle(string word)
        {
            if (string.IsNullOrEmpty(word)) return "a";

            char firstLetter = char.ToLower(word[0]);
            bool startsWithVowel = "aeiou".Contains(firstLetter);

            if (word.ToLower().StartsWith("hour") || word.ToLower().StartsWith("honor"))
                return "an";
            if (word.ToLower().StartsWith("uni") || word.ToLower().StartsWith("use"))
                return "a";

            return startsWithVowel ? "an" : "a";
        }

        public static void DisplayTextWithMarkup(string text, int maxLineLength = 80)
        {
            string[] lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            foreach (string line in lines)
            {
                AnsiConsole.MarkupLine(line);
            }
        }
    }
}
