using System.Collections.Generic;

namespace WarVikingsBot.Graphs
{
    /// <summary>
    /// Grafo da Fase 4: Recebimento de Carta de Território
    /// Implementa a quarta fase do turno do jogo War Vikings.
    /// </summary>
    public static class Phase4Graph
    {
        public static Graph Create()
        {
            // Criar todos os nós
            var start = new StartNode { Id = "phase4_start" };
            
            var action1 = new PerformActionNode("═══════════════════════════════════════\n  FASE 4: RECEBIMENTO DE CARTA DE TERRITÓRIO\n═══════════════════════════════════════")
            {
                Id = "phase4_action1"
            };
            
            // Verifica se conquistou território adversário neste turno
            var checkConquest = new BinaryConditionNode("Você conquistou pelo menos um território adversário neste turno?")
            {
                Id = "phase4_check_conquest"
            };
            
            // Se não conquistou, termina
            var noConquest = new PerformActionNode("Você não conquistou territórios adversários neste turno. Não recebe carta de território.")
            {
                Id = "phase4_no_conquest"
            };
            
            // Se conquistou, recebe carta
            var receiveCard = new ExecuteActionNode(
                "Recebendo carta de território do território conquistado...",
                "receive_territory_card"
            )
            {
                Id = "phase4_receive_card"
            };
            
            // Mostra mensagem de confirmação
            var confirmCard = new PerformActionNode("Carta de território recebida com sucesso.")
            {
                Id = "phase4_confirm_card"
            };
            
            // Verifica se acumulou 5+ cartas (forçar troca)
            var checkFiveCards = new BinaryConditionNode("Você tem 5 ou mais cartas de território agora?")
            {
                Id = "phase4_check_five_cards"
            };
            
            // Se tem 5+ cartas, deve trocar
            var mustTrade = new PerformActionNode("Você DEVE trocar cartas agora (obrigatório com 5+ cartas).")
            {
                Id = "phase4_must_trade"
            };
            
            // Chama o grafo de troca de cartas
            var jumpToTrade = new JumpToGraphNode("card_trade")
            {
                Id = "phase4_jump_trade"
            };
            
            // Se não tem 5+ cartas, termina
            var endNoTrade = new PerformActionNode("Você não precisa trocar cartas agora.")
            {
                Id = "phase4_end_no_trade"
            };
            
            var end = new EndNode("Fase 4 concluída. Carta de território recebida.")
            {
                Id = "phase4_end"
            };
            
            // Conectar os nós
            start.Next = action1;
            action1.Next = checkConquest;
            
            // Se não conquistou, termina
            checkConquest.FalseNode = noConquest;
            noConquest.Next = end;
            
            // Se conquistou, recebe carta
            checkConquest.TrueNode = receiveCard;
            receiveCard.Next = confirmCard;
            confirmCard.Next = checkFiveCards;
            
            // Se tem 5+ cartas, deve trocar
            checkFiveCards.TrueNode = mustTrade;
            mustTrade.Next = jumpToTrade;
            jumpToTrade.Next = end;
            
            // Se não tem 5+ cartas, termina
            checkFiveCards.FalseNode = endNoTrade;
            endNoTrade.Next = end;
            
            // Criar lista de todos os nós
            var allNodes = new List<Node>
            {
                start,
                action1,
                checkConquest,
                noConquest,
                receiveCard,
                confirmCard,
                checkFiveCards,
                mustTrade,
                jumpToTrade,
                endNoTrade,
                end
            };
            
            return new Graph("phase_4", start, allNodes, "Phase4Graph.cs");
        }
    }
}

