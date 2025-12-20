using System.Collections.Generic;

namespace WarVikingsBot.Graphs
{
    public class BinaryConditionNode : InteractiveNode
    {
        public string Condition { get; set; } = string.Empty;
        public Node? TrueNode { get; set; }
        public Node? FalseNode { get; set; }
        
        public BinaryConditionNode(string condition)
        {
            Condition = condition.Trim();
        }
        
        public override string GetMessage()
        {
            return Condition;
        }
        
        public override List<string> GetOptions()
        {
            return new List<string> { "true", "false" };
        }
        
        public override Node? GetNext(string option)
        {
            option = option.ToLower().Trim();
            if (option == "true" || option == "t")
                return TrueNode;
            else if (option == "false" || option == "f")
                return FalseNode;
            
            return null;
        }
    }
}

