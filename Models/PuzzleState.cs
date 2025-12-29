using System.Collections.Generic;

namespace GuildMaster.Models
{
    public class PuzzleState
    {
        public string PuzzleId { get; set; }  // Unique identifier, e.g., "bandit_vault_puzzle"
        public int RoomId { get; set; }  // Which room this puzzle is in
        public bool IsSolved { get; set; } = false;
        public Dictionary<string, object> CurrentState { get; set; } = new Dictionary<string, object>();
        public int Attempts { get; set; } = 0;
        public int MaxAttempts { get; set; } = 0;  // 0 = unlimited
        public string OnSolveQuestFlag { get; set; }  // Optional: Quest flag to set when solved
        public string OnSolveMessage { get; set; }  // Message displayed when puzzle is solved
        public List<string> UnlockedExits { get; set; } = new List<string>();  // Exits that unlock when solved

        public PuzzleState()
        {
            CurrentState = new Dictionary<string, object>();
            UnlockedExits = new List<string>();
        }
    }
}
