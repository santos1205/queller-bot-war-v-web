using System.Collections.Generic;

namespace WarVikingsBot.Graphs
{
    /// <summary>
    /// Grafo da Fase 1: Recebimento de Exércitos
    /// Implementa a primeira fase do turno do jogo War Vikings.
    /// </summary>
    public static class Phase1Graph
    {
        public static Graph Create()
        {
            // Criar todos os nós
            var start = new StartNode { Id = "phase1_start" };
            
            var action1 = new PerformActionNode("═══════════════════════════════════════\n  FASE 1: RECEBIMENTO DE EXÉRCITOS\n═══════════════════════════════════════")
            {
                Id = "phase1_action1"
            };
            
            var checkCards = new BinaryConditionNode("Você tem 5 ou mais cartas de território?")
            {
                Id = "phase1_check_cards"
            };
            
            // Caminho: precisa trocar cartas (5+)
            var mustTradeAction = new PerformActionNode("Você DEVE trocar cartas agora (obrigatório com 5+ cartas).")
            {
                Id = "phase1_must_trade"
            };
            
            var jumpToTrade = new JumpToGraphNode("card_trade")
            {
                Id = "phase1_jump_trade"
            };
            
            // Caminho: não precisa trocar obrigatoriamente
            var checkOptionalTrade = new BinaryConditionNode("Você quer trocar cartas agora? (opcional)")
            {
                Id = "phase1_check_optional_trade"
            };
            
            var jumpToOptionalTrade = new JumpToGraphNode("card_trade")
            {
                Id = "phase1_jump_optional_trade"
            };
            
            // Após troca (ou se não trocou)
            var calculateTerritories = new PerformActionNode("Calculando exércitos por territórios possuídos...\n(Regra: territórios ÷ 2, mínimo 3 se tiver menos de 6 territórios)")
            {
                Id = "phase1_calc_territories"
            };
            
            var calculateRegions = new PerformActionNode("Calculando exércitos por regiões conquistadas...\n(Regra: cada região completa conquistada dá exércitos extras conforme tabela)")
            {
                Id = "phase1_calc_regions"
            };
            
            var showTotal = new PerformActionNode("Total de exércitos recebidos calculado.\n(Exércitos por territórios + Exércitos por regiões + Exércitos por troca de cartas)")
            {
                Id = "phase1_show_total"
            };
            
            var allocateAction = new PerformActionNode("Aloque os exércitos recebidos nos seus territórios.")
            {
                Id = "phase1_allocate"
            };
            
            var end = new EndNode("Fase 1 concluída. Exércitos recebidos e alocados.")
            {
                Id = "phase1_end"
            };
            
            // Conectar os nós
            start.Next = action1;
            action1.Next = checkCards;
            
            // Se tem 5+ cartas, deve trocar
            checkCards.TrueNode = mustTradeAction;
            mustTradeAction.Next = jumpToTrade;
            jumpToTrade.Next = calculateTerritories;
            
            // Se não tem 5+ cartas, pergunta se quer trocar
            checkCards.FalseNode = checkOptionalTrade;
            checkOptionalTrade.TrueNode = jumpToOptionalTrade;
            jumpToOptionalTrade.Next = calculateTerritories;
            checkOptionalTrade.FalseNode = calculateTerritories;
            
            calculateTerritories.Next = calculateRegions;
            calculateRegions.Next = showTotal;
            showTotal.Next = allocateAction;
            allocateAction.Next = end;
            
            // Criar lista de todos os nós
            var allNodes = new List<Node>
            {
                start,
                action1,
                checkCards,
                mustTradeAction,
                jumpToTrade,
                checkOptionalTrade,
                jumpToOptionalTrade,
                calculateTerritories,
                calculateRegions,
                showTotal,
                allocateAction,
                end
            };
            
            return new Graph("phase_1", start, allNodes, "Phase1Graph.cs");
        }
    }
}

