using System;

namespace GuildMaster.Services
{
    public static class Console
    {
        private static GameConsole? _gameConsole;
        
        public static void Initialize(GameConsole console)
        {
            _gameConsole = console;
        }
        
        public static void WriteLine(string? text = null)
        {
            _gameConsole?.WriteLine(text ?? "");
        }
        
        public static void WriteLine(object? value)
        {
            _gameConsole?.WriteLine(value?.ToString() ?? "");
        }
        
        public static void Write(string? text)
        {
            // For Write without newline, we'll still output but the next content continues
            if (!string.IsNullOrEmpty(text))
            {
                _gameConsole?.Markup(System.Web.HttpUtility.HtmlEncode(text));
            }
        }
        
        public static string? ReadLine()
        {
            return _gameConsole?.ReadLine() ?? "";
        }
        
        public static void Clear()
        {
            _gameConsole?.Clear();
        }
        
        public static ConsoleColor ForegroundColor { get; set; } = ConsoleColor.White;
        public static ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
        
        public static void ResetColor()
        {
            ForegroundColor = ConsoleColor.White;
            BackgroundColor = ConsoleColor.Black;
        }
    }
    
    public static class AnsiConsole
    {
        private static GameConsole? _gameConsole;
        public static bool ShowDebugMessages { get; set; } = false; // Set to true to show debug messages

        public static void Initialize(GameConsole console)
        {
            _gameConsole = console;
        }

        public static void MarkupLine(string markup)
        {
            // Filter out debug messages if ShowDebugMessages is false
            if (!ShowDebugMessages && markup.Contains("[dim]DEBUG:"))
            {
                return;
            }
            _gameConsole?.MarkupLine(markup);
        }

        public static void Markup(string markup)
        {
            // Filter out debug messages if ShowDebugMessages is false
            if (!ShowDebugMessages && markup.Contains("[dim]DEBUG:"))
            {
                return;
            }
            _gameConsole?.Markup(markup);
        }

        public static void WriteLine(string? text = null)
        {
            _gameConsole?.WriteLine(text ?? "");
        }

        public static void Clear()
        {
            _gameConsole?.Clear();
        }
    }
    
    public static class Markup
    {
        public static string Escape(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            return text.Replace("[", "[[").Replace("]", "]]");
        }
        
        public static string Remove(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            
            string result = text;
            result = System.Text.RegularExpressions.Regex.Replace(result, @"\[[^\]]*\]", "");
            result = result.Replace("[[", "[").Replace("]]", "]");
            return result;
        }
    }
}
