using GuildMaster.Services;
using AnsiConsole = GuildMaster.Services.AnsiConsole;
using System.Linq;
using GuildMaster.Models;

namespace GuildMaster.Managers
{
    public class PuzzleManager
    {
        private GameContext _context;

        public PuzzleManager(GameContext context)
        {
            _context = context;
        }

        // Get puzzle state (returns null if doesn't exist)
        public PuzzleState GetPuzzleState(string puzzleId)
        {
            if (!_context.PuzzleStates.ContainsKey(puzzleId))
            {
                // Puzzle doesn't exist yet - this will be created when puzzle is designed
                return null;
            }
            return _context.PuzzleStates[puzzleId];
        }

        // Check if puzzle is solved
        public bool IsPuzzleSolved(string puzzleId)
        {
            var state = GetPuzzleState(puzzleId);
            return state != null && state.IsSolved;
        }

        // Mark puzzle as solved
        public void SolvePuzzle(string puzzleId)
        {
            var state = GetPuzzleState(puzzleId);
            if (state == null) return;

            state.IsSolved = true;

            // Display success message
            if (!string.IsNullOrEmpty(state.OnSolveMessage))
            {
                AnsiConsole.MarkupLine(state.OnSolveMessage);
            }

            // Set quest flag if specified
            if (!string.IsNullOrEmpty(state.OnSolveQuestFlag))
            {
                _context.Player.QuestFlags[state.OnSolveQuestFlag] = true;
            }

            // Unlock exits if specified
            if (state.UnlockedExits.Count > 0)
            {
                var room = _context.Rooms.Values.FirstOrDefault(r => r.NumericId == state.RoomId);
                if (room != null)
                {
                    foreach (var exitDirection in state.UnlockedExits)
                    {
                        // Logic to unlock exit - depends on how you implement locked exits
                        // This is placeholder for future implementation
                        // Could be adding to room.Exits dictionary, or removing from blocked list
                    }
                }
            }
        }

        // Attempt puzzle (increment attempts, check max)
        public bool CanAttemptPuzzle(string puzzleId)
        {
            var state = GetPuzzleState(puzzleId);
            if (state == null) return false;
            if (state.IsSolved) return false;
            if (state.MaxAttempts > 0 && state.Attempts >= state.MaxAttempts) return false;
            return true;
        }

        public void IncrementAttempts(string puzzleId)
        {
            var state = GetPuzzleState(puzzleId);
            if (state != null)
            {
                state.Attempts++;
            }
        }

        // Update puzzle state value
        public void SetPuzzleStateValue(string puzzleId, string key, object value)
        {
            var state = GetPuzzleState(puzzleId);
            if (state != null)
            {
                state.CurrentState[key] = value;
            }
        }

        public object GetPuzzleStateValue(string puzzleId, string key)
        {
            var state = GetPuzzleState(puzzleId);
            if (state == null) return null;
            return state.CurrentState.ContainsKey(key) ? state.CurrentState[key] : null;
        }
    }
}
