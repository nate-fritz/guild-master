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

        public class Choice
        {
            public string choiceText { get; set; }
            public string nextNodeID { get; set; }
            public Func<List<string>, bool> IsAvailable { get; set; } = (inventory) => true;
            public DialogueAction Action { get; set; }  // ADD THIS LINE
        }

        public DialogueNode()
        {
            Choices = new List<Choice>();
        }
    }
}
