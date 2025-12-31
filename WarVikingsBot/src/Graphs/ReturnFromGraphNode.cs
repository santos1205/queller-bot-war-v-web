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
            return string.Empty; // Mensagem vazia - ReturnFromGraphNode é apenas um marcador interno
        }
    }
}

