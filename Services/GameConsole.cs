using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GuildMaster.Services
{
    public class GameConsole
    {
        public event Action<string>? OnOutput;
        public event Action? OnClear;
        public event Action<string>? OnInputRequested;
        
        private Queue<string> inputQueue = new Queue<string>();
        private TaskCompletionSource<string>? pendingInput;
        
        public void WriteLine(string text = "")
        {
            string html = System.Web.HttpUtility.HtmlEncode(text);
            OnOutput?.Invoke(html);
        }

        public void MarkupLine(string markup)
        {
            string html = ConvertMarkupToHtml(markup);
            OnOutput?.Invoke(html);
        }
        
        public void Markup(string markup)
        {
            string html = ConvertMarkupToHtml(markup);
            OnOutput?.Invoke(html);
        }

        public void Clear()
        {
            OnClear?.Invoke();
        }
        
        public void QueueInput(string input)
        {
            if (pendingInput != null)
            {
                var tcs = pendingInput;
                pendingInput = null;
                tcs.SetResult(input);
            }
            else
            {
                inputQueue.Enqueue(input);
            }
        }
        
        public string? ReadLine()
        {
            // If there's queued input, return it immediately
            if (inputQueue.Count > 0)
            {
                return inputQueue.Dequeue();
            }

            // Signal that we need input and wait synchronously
            // This is a workaround - in web context we'll use the async version
            OnInputRequested?.Invoke("awaiting_input");

            // For synchronous calls in web context, return empty to exit menus gracefully
            // The real input will come through QueueInput
            return "";
        }
        
        public async Task<string> ReadLineAsync()
        {
            if (inputQueue.Count > 0)
            {
                return inputQueue.Dequeue();
            }
            
            pendingInput = new TaskCompletionSource<string>();
            OnInputRequested?.Invoke("awaiting_input");
            return await pendingInput.Task;
        }
        
        public bool HasPendingInput => pendingInput != null;

        private string ConvertMarkupToHtml(string markup)
        {
            if (string.IsNullOrEmpty(markup))
                return "";

            string result = markup;

            // Step 1: Handle escaped brackets - convert to temporary placeholders
            result = result.Replace("[[", "\x01LEFTBRACKET\x01");
            result = result.Replace("]]", "\x01RIGHTBRACKET\x01");

            // Step 2: Extract and protect HTML tags (like <span class='...'>)
            var htmlPattern = new Regex(@"<([^>]+)>");
            var htmlTags = new List<(string placeholder, string tag)>();
            int htmlTagIndex = 0;

            result = htmlPattern.Replace(result, match =>
            {
                string placeholder = $"\x03HTMLTAG{htmlTagIndex}\x03";
                htmlTags.Add((placeholder, match.Value));
                htmlTagIndex++;
                return placeholder;
            });

            // Step 3: Extract and protect markup tags, then HTML encode the rest
            var tagPattern = new Regex(@"\[([^\[\]]+)\]");
            var tags = new List<(string placeholder, string tag)>();
            int tagIndex = 0;

            result = tagPattern.Replace(result, match =>
            {
                string placeholder = $"\x02TAG{tagIndex}\x02";
                tags.Add((placeholder, match.Value));
                tagIndex++;
                return placeholder;
            });

            // Step 4: HTML encode the content (now safe since tags are protected)
            result = System.Web.HttpUtility.HtmlEncode(result);

            // Step 5: Restore the markup tags
            foreach (var (placeholder, tag) in tags)
            {
                result = result.Replace(System.Web.HttpUtility.HtmlEncode(placeholder), tag);
            }

            // Step 6: Restore HTML tags (unencoded so they render as HTML)
            foreach (var (placeholder, tag) in htmlTags)
            {
                result = result.Replace(System.Web.HttpUtility.HtmlEncode(placeholder), tag);
            }

            // Step 7: Process markup tags (innermost first)
            int maxIterations = 50;
            int iterations = 0;

            while (iterations < maxIterations)
            {
                var match = Regex.Match(result, @"\[([^\[\]/]+)\]([^\[]*?)\[/\]");
                if (!match.Success)
                    break;

                string fullMatch = match.Value;
                string colorCode = match.Groups[1].Value;
                string content = match.Groups[2].Value;

                string cssColor = ConvertColorCode(colorCode);
                string replacement = $"<span style=\"color:{cssColor}\">{content}</span>";

                result = result.Replace(fullMatch, replacement);
                iterations++;
            }

            // Step 8: Clean up any remaining closing tags
            result = Regex.Replace(result, @"\[/[^\]]*\]", "");

            // Step 9: Restore escaped brackets as literal brackets
            result = result.Replace("\x01LEFTBRACKET\x01", "[");
            result = result.Replace("\x01RIGHTBRACKET\x01", "]");

            return result;
        }

        private string ConvertColorCode(string code)
        {
            code = code.ToLower().Trim();
            
            if (code.StartsWith("#"))
                return code;
                
            return code switch
            {
                "cyan" => "#00ffff",
                "red" => "#ff4444",
                "green" => "#44ff44",
                "blue" => "#4444ff",
                "yellow" => "#ffff44",
                "white" => "#ffffff",
                "dim" => "#888888",
                "bold" => "#ffffff",
                "orange" => "#ff8800",
                "magenta" => "#ff44ff",
                "grey" or "gray" => "#888888",
                _ => "#ffffff"
            };
        }
    }
}
