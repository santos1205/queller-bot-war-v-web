using System.Collections.Generic;

namespace WarVikingsBot.Graphs
{
    /// <summary>
    /// Grafo da Fase 3: Deslocamento de Exércitos
    /// Implementa a terceira fase do turno do jogo War Vikings.
    /// </summary>
    public static class Phase3Graph
    {
        public static Graph Create()
        {
            // Criar todos os nós
            var start = new StartNode { Id = "phase3_start" };
            
            var action1 = new PerformActionNode("═══════════════════════════════════════\n  FASE 3: DESLOCAMENTO DE EXÉRCITOS\n═══════════════════════════════════════")
            {
                Id = "phase3_action1"
            };
            
            // Verifica se há possibilidades de deslocamento
            var checkCanMove = new BinaryConditionNode("Você tem possibilidades de deslocamento? (territórios contíguos com mais de 1 exército)")
            {
                Id = "phase3_check_can_move"
            };
            
            // Se não pode deslocar, termina
            var noMovement = new PerformActionNode("Você não tem possibilidades de deslocamento (precisa de territórios contíguos com mais de 1 exército).")
            {
                Id = "phase3_no_movement"
            };
            
            // Pergunta se quer deslocar
            var askMovement = new BinaryConditionNode("Você quer realizar um deslocamento de exércitos? (1 por turno, exceto após conquista)")
            {
                Id = "phase3_ask_movement"
            };
            
            // Se não quer deslocar, termina
            var endMovement = new PerformActionNode("Você decidiu não realizar deslocamento de exércitos.")
            {
                Id = "phase3_end_movement"
            };
            
            // Se quer deslocar, mostra territórios de origem disponíveis
            var showSources = new PerformActionNode("Identificando territórios de origem que podem deslocar exércitos...")
            {
                Id = "phase3_show_sources"
            };
            
            // Define o território de origem (por enquanto, usa o primeiro disponível)
            // TODO: Implementar seleção real do usuário
            var setSource = new ExecuteActionNode(
                "Selecionando território de origem do deslocamento...",
                "set_movement_source"
            )
            {
                Id = "phase3_set_source"
            };
            
            // Mostra destinos disponíveis
            var showTargets = new PerformActionNode("Identificando territórios de destino contíguos...")
            {
                Id = "phase3_show_targets"
            };
            
            // Define o território de destino (por enquanto, usa o primeiro disponível)
            // TODO: Implementar seleção real do usuário
            var setTarget = new ExecuteActionNode(
                "Selecionando território de destino do deslocamento...",
                "set_movement_target"
            )
            {
                Id = "phase3_set_target"
            };
            
            // Pergunta quantos exércitos mover
            var askArmies = new PerformActionNode("Quantos exércitos você quer mover? (mínimo 1, deixando pelo menos 1 no território de origem)")
            {
                Id = "phase3_ask_armies"
            };
            
            // Executa o deslocamento
            var executeMovement = new ExecuteActionNode(
                "Executando deslocamento de exércitos...",
                "execute_movement"
            )
            {
                Id = "phase3_execute_movement"
            };
            
            // Mensagem de confirmação
            var confirmMovement = new PerformActionNode("Deslocamento realizado com sucesso.")
            {
                Id = "phase3_confirm_movement"
            };
            
            var end = new EndNode("Fase 3 concluída. Deslocamento de exércitos finalizado.")
            {
                Id = "phase3_end"
            };
            
            // Conectar os nós
            start.Next = action1;
            action1.Next = checkCanMove;
            
            // Se não pode deslocar, termina
            checkCanMove.FalseNode = noMovement;
            noMovement.Next = end;
            
            // Se pode deslocar, pergunta se quer deslocar
            checkCanMove.TrueNode = askMovement;
            
            // Se não quer deslocar, termina
            askMovement.FalseNode = endMovement;
            endMovement.Next = end;
            
            // Se quer deslocar, mostra fontes e destinos
            askMovement.TrueNode = showSources;
            showSources.Next = setSource;
            setSource.Next = showTargets;
            showTargets.Next = setTarget;
            setTarget.Next = askArmies;
            askArmies.Next = executeMovement;
            executeMovement.Next = confirmMovement;
            confirmMovement.Next = end;
            
            // Criar lista de todos os nós
            var allNodes = new List<Node>
            {
                start,
                action1,
                checkCanMove,
                noMovement,
                askMovement,
                endMovement,
                showSources,
                setSource,
                showTargets,
                setTarget,
                askArmies,
                executeMovement,
                confirmMovement,
                end
            };
            
            return new Graph("phase_3", start, allNodes, "Phase3Graph.cs");
        }
    }
}

