using GuildMaster.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildMaster.Models
{
    public class DialogueNode
    {
        public string Text { get; set; }
        public List<Choice> Choices { get; set; }
        public DialogueAction Action { get; set; }  // ADD THIS LINE

        // If true, this node permanently locks dialogue at this position (NPC won't talk again)
        // If false (default), conversation resets to greeting when it ends
        public bool PermanentlyEndsDialogue { get; set; } = false;

        public class Choice
        {
            public string choiceText { get; set; }
            public string nextNodeID { get; set; }
            public Func<List<string>, bool> IsAvailable { get; set; } = (inventory) => true;
            public DialogueAction Action { get; set; }  // ADD THIS LINE

            // Topic tracking - show choice only if node has/hasn't been visited
            public string RequireDiscussedNode { get; set; } = null;  // Show only if this node was visited
            public string RequireNotDiscussedNode { get; set; } = null;  // Show only if this node was NOT visited
        }

        public DialogueNode()
        {
            Choices = new List<Choice>();
        }
    }
}
