using System.Collections.Generic;

namespace WarVikingsBot.Graphs
{
    /// <summary>
    /// Grafo de Troca de Cartas
    /// Sub-grafo usado na Fase 1 para trocar cartas de território.
    /// </summary>
    public static class CardTradeGraph
    {
        public static Graph Create()
        {
            // Criar todos os nós
            var start = new StartNode { Id = "card_trade_start" };
            
            var action1 = new PerformActionNode("═══════════════════════════════════════\n  TROCA DE CARTAS\n═══════════════════════════════════════")
            {
                Id = "card_trade_action1"
            };
            
            var showCards = new PerformActionNode("Mostrando suas cartas de território...")
            {
                Id = "card_trade_show_cards"
            };
            
            var checkThreeSame = new BinaryConditionNode("Você tem 3 cartas com a mesma figura?")
            {
                Id = "card_trade_check_same"
            };
            
            var checkThreeDifferent = new BinaryConditionNode("Você tem 3 cartas com figuras diferentes?")
            {
                Id = "card_trade_check_different"
            };
            
            var tradeSameAction = new PerformActionNode("Troque 3 cartas iguais e receba exércitos.")
            {
                Id = "card_trade_same"
            };
            
            var tradeDifferentAction = new PerformActionNode("Troque 3 cartas diferentes e receba exércitos.")
            {
                Id = "card_trade_different"
            };
            
            var showArmies = new PerformActionNode("Exércitos recebidos pela troca calculados.")
            {
                Id = "card_trade_show_armies"
            };
            
            var removeCards = new PerformActionNode("Remova as 3 cartas trocadas do seu baralho.")
            {
                Id = "card_trade_remove"
            };
            
            var returnNode = new ReturnFromGraphNode
            {
                Id = "card_trade_return"
            };
            
            // Conectar os nós
            start.Next = action1;
            action1.Next = showCards;
            showCards.Next = checkThreeSame;
            
            // Se tem 3 iguais
            checkThreeSame.TrueNode = tradeSameAction;
            tradeSameAction.Next = showArmies;
            
            // Se não tem 3 iguais, verifica 3 diferentes
            checkThreeSame.FalseNode = checkThreeDifferent;
            checkThreeDifferent.TrueNode = tradeDifferentAction;
            tradeDifferentAction.Next = showArmies;
            
            // Se não tem nenhum dos dois, retorna
            checkThreeDifferent.FalseNode = new PerformActionNode("Você não pode trocar cartas agora (precisa de 3 iguais ou 3 diferentes).")
            {
                Id = "card_trade_cannot"
            };
            ((PerformActionNode)checkThreeDifferent.FalseNode).Next = returnNode;
            
            showArmies.Next = removeCards;
            removeCards.Next = returnNode;
            
            // Criar lista de todos os nós
            var allNodes = new List<Node>
            {
                start,
                action1,
                showCards,
                checkThreeSame,
                checkThreeDifferent,
                tradeSameAction,
                tradeDifferentAction,
                showArmies,
                removeCards,
                returnNode
            };
            
            // Adicionar nó de "não pode trocar" se foi criado
            if (checkThreeDifferent.FalseNode != null)
            {
                allNodes.Add(checkThreeDifferent.FalseNode);
            }
            
            return new Graph("card_trade", start, allNodes, "CardTradeGraph.cs");
        }
    }
}

