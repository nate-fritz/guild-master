using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildMaster.Models
{
    public class Quest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Duration { get; set; }  // Hours to complete
        public string Difficulty { get; set; }  // "Easy", "Medium", "Hard"

        // Success rates based on recruit class/level
        public int BaseSuccessChance { get; set; }

        // Rewards
        public int MinGold { get; set; }
        public int MaxGold { get; set; }
        public Dictionary<string, int> ItemRewards { get; set; }  // itemId -> drop %
        public string PotentialRecruit { get; set; }  // NPC name if recruit reward possible
        public int BaseExperienceReward { get; set; }
        public int FailedExperienceReward => BaseExperienceReward / 2; // Half XP on failure

        // Active quest tracking
        public Recruit AssignedRecruit { get; set; }
        public float StartTime { get; set; }  // Day + hour when started
        public int StartDay { get; set; }
        public bool IsActive { get; set; }
        public bool IsComplete { get; set; }
        public bool WasSuccessful { get; set; }

        public float GetCompletionTime() => StartTime + Duration;
    }
}
