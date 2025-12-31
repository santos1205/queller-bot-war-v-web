using System.Collections.Generic;

namespace WarVikingsBot.Graphs
{
    /// <summary>
    /// Nó que executa uma ação no estado do jogo.
    /// Diferente do PerformActionNode que apenas exibe mensagens,
    /// este nó executa uma ação real no WarVikingsState.
    /// </summary>
    public class ExecuteActionNode : InteractiveNode
    {
        public string Message { get; set; } = string.Empty;
        public string ActionId { get; set; } = string.Empty; // ID da ação a executar
        public Node? Next { get; set; }
        
        public ExecuteActionNode(string message, string actionId)
        {
            Message = message.Trim();
            ActionId = actionId.Trim();
        }
        
        public override string GetMessage()
        {
            return Message;
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

