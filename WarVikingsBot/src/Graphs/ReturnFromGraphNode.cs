namespace WarVikingsBot.Graphs
{
    public class ReturnFromGraphNode : NonInteractiveNode
    {
        public ReturnFromGraphNode()
        {
        }
        
        public override Node? GetNext()
        {
            return null;
        }
        
        public string GetMessage()
        {
            return "Continue from where you jumped to this graph.";
        }
    }
}

