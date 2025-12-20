using System.Collections.Generic;
using System.Linq;

namespace WarVikingsBot.Graphs
{
    public class MultipleChoiceNode : InteractiveNode
    {
        public string Question { get; set; } = string.Empty;
        public List<Node?> Options { get; set; } = new List<Node?>();
        
        public MultipleChoiceNode(string question)
        {
            Question = question.Trim();
        }
        
        public override string GetMessage()
        {
            return Question;
        }
        
        public override List<string> GetOptions()
        {
            return Enumerable.Range(1, Options.Count)
                .Select(i => i.ToString())
                .ToList();
        }
        
        public override Node? GetNext(string option)
        {
            if (int.TryParse(option, out int index))
            {
                if (index >= 1 && index <= Options.Count)
                {
                    return Options[index - 1];
                }
            }
            
            return null;
        }
    }
}

