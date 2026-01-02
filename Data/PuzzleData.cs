using System.Collections.Generic;
using GuildMaster.Models;

namespace GuildMaster.Data
{
    public static class PuzzleData
    {
        // This will be populated with actual puzzles later
        // Example structure:

        /*
        public static Dictionary<string, PuzzleState> GetAllPuzzles()
        {
            return new Dictionary<string, PuzzleState>
            {
                // Future puzzles will be added here
            };
        }
        */

        // Placeholder - returns empty for now
        public static Dictionary<string, PuzzleState> GetAllPuzzles()
        {
            return new Dictionary<string, PuzzleState>
            {
                {
                    "twisting_path_puzzle", new PuzzleState
                    {
                        PuzzleId = "twisting_path_puzzle",
                        RoomId = 57,
                        IsSolved = false,
                        MaxAttempts = 0,  // Unlimited attempts
                        OnSolveMessage = null,  // Custom messages per path
                        UnlockedExits = new List<string>(),  // Exits unlocked individually
                        CurrentState = new Dictionary<string, object>
                        {
                            // West path clues
                            { "examined_bones", false },
                            { "examined_tracks", false },
                            { "examined_branches", false },
                            { "west_path_revealed", false },

                            // North path clues
                            { "examined_bootprints", false },
                            { "examined_vines", false },
                            { "north_path_revealed", false }
                        }
                    }
                },
                {
                    "warlord_chamber_gates", new PuzzleState
                    {
                        PuzzleId = "warlord_chamber_gates",
                        RoomId = 18,  // Shared between rooms 18 and 20
                        IsSolved = false,
                        MaxAttempts = 0,  // Unlimited attempts
                        OnSolveMessage = null,  // Custom message per gate
                        UnlockedExits = new List<string>(),  // Will unlock based on which gate is opened
                        CurrentState = new Dictionary<string, object>
                        {
                            { "iron_gate_unlocked", false },
                            { "bronze_gate_unlocked", false }
                        }
                    }
                },
                {
                    "foggy_clearing_puzzle", new PuzzleState
                    {
                        PuzzleId = "foggy_clearing_puzzle",
                        RoomId = 53,
                        IsSolved = false,
                        MaxAttempts = 0,  // Unlimited attempts to speak the passphrase
                        OnSolveMessage = "The fog clears, revealing a path to the east.",
                        UnlockedExits = new List<string> { "east" },  // Opens east to Room 100
                        CurrentState = new Dictionary<string, object>
                        {
                            { "fog_cleared", false }
                        }
                    }
                },
                {
                    "ritual_chamber_cipher", new PuzzleState
                    {
                        PuzzleId = "ritual_chamber_cipher",
                        RoomId = 105,
                        IsSolved = false,
                        MaxAttempts = 0,  // Unlimited attempts
                        OnSolveMessage = "The iron gate shudders and swings open with a grinding sound. The passage south is now accessible.",
                        UnlockedExits = new List<string> { "south" },  // Opens south to Room 108
                        CurrentState = new Dictionary<string, object>
                        {
                            { "gate_unlocked", false },
                            { "correct_answer", "nihil" }  // The word that unmakes all things (nothing/void)
                        }
                    }
                },
                {
                    "library_book_puzzle", new PuzzleState
                    {
                        PuzzleId = "library_book_puzzle",
                        RoomId = 108,
                        IsSolved = false,
                        MaxAttempts = 0,  // No puzzle mechanics yet, just lore/flavor
                        OnSolveMessage = null,
                        UnlockedExits = new List<string>(),
                        CurrentState = new Dictionary<string, object>
                        {
                            { "books_examined", false }
                        }
                    }
                }
            };
        }
    }
}
