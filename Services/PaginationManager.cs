using System.Collections.Generic;
using System.Linq;

namespace GuildMaster.Services
{
    public class PaginationManager
    {
        private List<string> paginatedLines = new List<string>();
        private int currentPage = 0;
        private int linesPerPage = 10;
        private string? currentColor = null;

        public bool HasMorePages => paginatedLines.Count > 0 && currentPage * linesPerPage < paginatedLines.Count;

        public void SetPaginatedContent(string[] lines, string? color = null, int linesPerPage = 10)
        {
            this.paginatedLines = lines.ToList();
            this.currentPage = 0;
            this.linesPerPage = linesPerPage;
            this.currentColor = color;
        }

        public void Clear()
        {
            paginatedLines.Clear();
            currentPage = 0;
            currentColor = null;
        }

        public void ShowNextPage()
        {
            if (!HasMorePages)
            {
                return;
            }

            int startIndex = currentPage * linesPerPage;
            int endIndex = System.Math.Min(startIndex + linesPerPage, paginatedLines.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                if (!string.IsNullOrEmpty(currentColor))
                {
                    string escapedLine = paginatedLines[i].Replace("[", "[[").Replace("]", "]]");
                    AnsiConsole.MarkupLine($"[{currentColor}]{escapedLine}[/]");
                }
                else
                {
                    string escapedLine = paginatedLines[i].Replace("[", "[[").Replace("]", "]]");
                    AnsiConsole.MarkupLine(escapedLine);
                }
            }

            currentPage++;

            if (HasMorePages)
            {
                int remainingLines = paginatedLines.Count - (currentPage * linesPerPage);
                AnsiConsole.MarkupLine($"\n[dim]Press Enter to continue... ({remainingLines} more lines)[/]");
            }
        }
    }
}
