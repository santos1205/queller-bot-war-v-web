namespace WarVikingsBot.Graphs
{
    public class JumpToGraphNode : NonInteractiveNode
    {
        public string TargetGraphId { get; set; } = string.Empty;
        public Node? Next { get; set; }
        
        public JumpToGraphNode(string targetGraphId)
        {
            TargetGraphId = targetGraphId;
        }
        
        public override Node? GetNext()
        {
            return Next;
        }
    }
}

