using System.Collections.Generic;

namespace WarVikingsBot.Graphs
{
    /// <summary>
    /// Grafo da Fase 2: Ataques
    /// Implementa a segunda fase do turno do jogo War Vikings.
    /// </summary>
    public static class Phase2Graph
    {
        public static Graph Create()
        {
            // Criar todos os nós
            var start = new StartNode { Id = "phase2_start" };
            
            var action1 = new PerformActionNode("═══════════════════════════════════════\n  FASE 2: ATAQUES\n═══════════════════════════════════════")
            {
                Id = "phase2_action1"
            };
            
            // Verifica se é primeira rodada (sem ataques)
            var checkFirstRound = new BinaryConditionNode("É a primeira rodada do jogo?")
            {
                Id = "phase2_check_first_round"
            };
            
            // Se é primeira rodada, pula ataques
            var skipAttacks = new PerformActionNode("Primeira rodada: não há ataques. Apenas posicionamento de exércitos.")
            {
                Id = "phase2_skip_attacks"
            };
            
            // Se não é primeira rodada, verifica se há territórios que podem atacar
            var checkCanAttack = new BinaryConditionNode("Você tem territórios que podem atacar? (mínimo 2 exércitos)")
            {
                Id = "phase2_check_can_attack"
            };
            
            // Se não pode atacar, pula para o fim
            var noAttacks = new PerformActionNode("Você não tem territórios que podem atacar (precisa de pelo menos 2 exércitos por território).")
            {
                Id = "phase2_no_attacks"
            };
            
            // Loop de ataques: pergunta se quer atacar
            var askAttack = new BinaryConditionNode("Você quer realizar um ataque?")
            {
                Id = "phase2_ask_attack"
            };
            
            // Se não quer atacar, vai para o fim
            var endAttacks = new PerformActionNode("Você decidiu não realizar mais ataques.")
            {
                Id = "phase2_end_attacks"
            };
            
            // Se quer atacar, mostra territórios de origem disponíveis
            var showSources = new PerformActionNode("Identificando territórios de origem que podem atacar...")
            {
                Id = "phase2_show_sources"
            };
            
            // Pergunta qual território de origem usar (será um MultipleChoiceNode quando implementarmos a seleção)
            var selectSource = new PerformActionNode("Selecione o território de origem do ataque.")
            {
                Id = "phase2_select_source"
            };
            
            // Mostra alvos disponíveis
            var showTargets = new PerformActionNode("Identificando territórios inimigos que podem ser atacados...")
            {
                Id = "phase2_show_targets"
            };
            
            // Pergunta qual território atacar (será um MultipleChoiceNode quando implementarmos a seleção)
            var selectTarget = new PerformActionNode("Selecione o território alvo do ataque.")
            {
                Id = "phase2_select_target"
            };
            
            // Chama o grafo de combate
            var jumpToCombat = new JumpToGraphNode("combat")
            {
                Id = "phase2_jump_combat"
            };
            
            // Após o combate, volta para perguntar se quer atacar novamente
            var afterCombat = new PerformActionNode("Combate resolvido. Você pode realizar outro ataque se desejar.")
            {
                Id = "phase2_after_combat"
            };
            
            var end = new EndNode("Fase 2 concluída. Ataques finalizados.")
            {
                Id = "phase2_end"
            };
            
            // Conectar os nós
            start.Next = action1;
            action1.Next = checkFirstRound;
            
            // Se é primeira rodada, pula ataques
            checkFirstRound.TrueNode = skipAttacks;
            skipAttacks.Next = end;
            
            // Se não é primeira rodada, verifica se pode atacar
            checkFirstRound.FalseNode = checkCanAttack;
            checkCanAttack.FalseNode = noAttacks;
            noAttacks.Next = end;
            
            // Se pode atacar, pergunta se quer atacar
            checkCanAttack.TrueNode = askAttack;
            
            // Se não quer atacar, termina
            askAttack.FalseNode = endAttacks;
            endAttacks.Next = end;
            
            // Se quer atacar, mostra fontes e alvos
            askAttack.TrueNode = showSources;
            showSources.Next = selectSource;
            selectSource.Next = showTargets;
            showTargets.Next = selectTarget;
            selectTarget.Next = jumpToCombat;
            
            // Após o combate, volta para perguntar se quer atacar novamente
            jumpToCombat.Next = afterCombat;
            afterCombat.Next = askAttack; // Loop de volta para perguntar se quer atacar novamente
            
            // Criar lista de todos os nós
            var allNodes = new List<Node>
            {
                start,
                action1,
                checkFirstRound,
                skipAttacks,
                checkCanAttack,
                noAttacks,
                askAttack,
                endAttacks,
                showSources,
                selectSource,
                showTargets,
                selectTarget,
                jumpToCombat,
                afterCombat,
                end
            };
            
            return new Graph("phase_2", start, allNodes, "Phase2Graph.cs");
        }
    }
}

