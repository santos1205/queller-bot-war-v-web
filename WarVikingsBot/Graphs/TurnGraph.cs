using System.Collections.Generic;

namespace WarVikingsBot.Graphs
{
    /// <summary>
    /// Grafo principal que orquestra um turno completo do jogo War Vikings.
    /// Conecta as 4 fases em sequência: Fase 1 → Fase 2 → Fase 3 → Fase 4
    /// </summary>
    public static class TurnGraph
    {
        public static Graph Create()
        {
            // Criar todos os nós
            var start = new StartNode { Id = "turn_start" };
            
            // Mensagem de início do turno
            var turnStart = new PerformActionNode("═══════════════════════════════════════\n  INÍCIO DO TURNO\n═══════════════════════════════════════")
            {
                Id = "turn_start_msg"
            };
            
            // Fase 1: Recebimento de Exércitos
            var jumpPhase1 = new JumpToGraphNode("phase_1")
            {
                Id = "turn_jump_phase1"
            };
            
            // Fase 2: Ataques
            var jumpPhase2 = new JumpToGraphNode("phase_2")
            {
                Id = "turn_jump_phase2"
            };
            
            // Fase 3: Deslocamento de Exércitos
            var jumpPhase3 = new JumpToGraphNode("phase_3")
            {
                Id = "turn_jump_phase3"
            };
            
            // Fase 4: Recebimento de Carta de Território
            var jumpPhase4 = new JumpToGraphNode("phase_4")
            {
                Id = "turn_jump_phase4"
            };
            
            // Mensagem de fim do turno
            var turnEnd = new PerformActionNode("═══════════════════════════════════════\n  FIM DO TURNO\n═══════════════════════════════════════")
            {
                Id = "turn_end_msg"
            };
            
            var end = new EndNode("Turno completo concluído. Todas as 4 fases foram executadas.")
            {
                Id = "turn_end"
            };
            
            // Conectar os nós em sequência
            start.Next = turnStart;
            turnStart.Next = jumpPhase1;
            
            // Após Fase 1, vai para Fase 2
            jumpPhase1.Next = jumpPhase2;
            
            // Após Fase 2, vai para Fase 3
            jumpPhase2.Next = jumpPhase3;
            
            // Após Fase 3, vai para Fase 4
            jumpPhase3.Next = jumpPhase4;
            
            // Após Fase 4, finaliza o turno
            jumpPhase4.Next = turnEnd;
            turnEnd.Next = end;
            
            // Criar lista de todos os nós
            var allNodes = new List<Node>
            {
                start,
                turnStart,
                jumpPhase1,
                jumpPhase2,
                jumpPhase3,
                jumpPhase4,
                turnEnd,
                end
            };
            
            return new Graph("turn", start, allNodes, "TurnGraph.cs");
        }
    }
}
