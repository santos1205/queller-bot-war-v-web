using System.Collections.Generic;

namespace WarVikingsBot.Graphs
{
    public class PerformActionNode : InteractiveNode
    {
        public string Action { get; set; } = string.Empty;
        public Node? Next { get; set; }
        
        public PerformActionNode(string action)
        {
            Action = action.Trim();
        }
        
        public override string GetMessage()
        {
            return Action;
        }
        
        public override List<string> GetOptions()
        {
            return new List<string> { "" };
        }
        
        public override Node? GetNext(string option)
        {
            return Next;
        }
    }
}

