using System;

namespace GuildMaster.Helpers
{
    /// <summary>
    /// Helper methods for standardized menu input validation across the application.
    /// Provides consistent handling of back/continue actions and menu choice parsing.
    /// </summary>
    public static class MenuInputHelper
    {
        /// <summary>
        /// Checks if input represents "back" or "continue" action.
        /// Accepts both "0" and empty string (Enter key).
        /// </summary>
        /// <param name="input">The user input to validate</param>
        /// <returns>True if input is "0" or empty/whitespace, false otherwise</returns>
        /// <remarks>
        /// Use this method ONLY when "0" is the sole option in a menu (e.g., "Press Enter to go back").
        /// For menus with multiple numbered options, use IsBack() instead.
        /// </remarks>
        public static bool IsBackOrContinue(string input)
        {
            return input?.Trim() == "0" || string.IsNullOrWhiteSpace(input);
        }

        /// <summary>
        /// Checks if input is exactly "0" for back/exit action.
        /// Does NOT accept empty string - user must explicitly type "0".
        /// </summary>
        /// <param name="input">The user input to validate</param>
        /// <returns>True if input is exactly "0", false otherwise</returns>
        /// <remarks>
        /// Use this method when other numbered options are available in the menu
        /// (e.g., "1. Option A", "2. Option B", "0. Back").
        /// </remarks>
        public static bool IsBack(string input)
        {
            return input?.Trim() == "0";
        }

        /// <summary>
        /// Validates and parses menu choice within a specified range.
        /// </summary>
        /// <param name="input">The user input to parse</param>
        /// <param name="maxOption">The maximum valid option number (inclusive)</param>
        /// <param name="choice">Output parameter containing the parsed choice if valid</param>
        /// <returns>True if input is a valid integer within [1..maxOption], false otherwise</returns>
        /// <remarks>
        /// This method validates choices in the range [1..maxOption].
        /// It does NOT accept "0" as a valid choice - use IsBack() or IsBackOrContinue() for that.
        /// </remarks>
        public static bool TryParseMenuChoice(string input, int maxOption, out int choice)
        {
            if (int.TryParse(input?.Trim(), out choice))
            {
                return choice >= 1 && choice <= maxOption;
            }
            choice = 0;
            return false;
        }

        /// <summary>
        /// Validates and parses menu choice within a custom range.
        /// </summary>
        /// <param name="input">The user input to parse</param>
        /// <param name="minOption">The minimum valid option number (inclusive)</param>
        /// <param name="maxOption">The maximum valid option number (inclusive)</param>
        /// <param name="choice">Output parameter containing the parsed choice if valid</param>
        /// <returns>True if input is a valid integer within [minOption..maxOption], false otherwise</returns>
        /// <remarks>
        /// Useful for special cases like save/load slots (e.g., slots 1-4).
        /// </remarks>
        public static bool TryParseMenuChoice(string input, int minOption, int maxOption, out int choice)
        {
            if (int.TryParse(input?.Trim(), out choice))
            {
                return choice >= minOption && choice <= maxOption;
            }
            choice = 0;
            return false;
        }
    }
}
