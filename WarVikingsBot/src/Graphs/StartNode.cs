namespace WarVikingsBot.Graphs
{
    public class StartNode : NonInteractiveNode
    {
        public Node? Next { get; set; }
        
        public StartNode()
        {
        }
        
        public override Node? GetNext()
        {
            return Next;
        }
    }
}

