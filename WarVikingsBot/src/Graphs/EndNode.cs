namespace WarVikingsBot.Graphs
{
    public class EndNode : NonInteractiveNode
    {
        public string Message { get; set; } = "End of Action";
        
        public EndNode(string? message = null)
        {
            if (!string.IsNullOrEmpty(message))
                Message = message.Trim();
        }
        
        public override Node? GetNext()
        {
            return null;
        }
        
        public string GetMessage()
        {
            return Message;
        }
    }
}

