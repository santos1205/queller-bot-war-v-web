using System.Collections.Generic;

namespace WarVikingsBot.Graphs
{
    /// <summary>
    /// Grafo de Resolução de Combate Terrestre
    /// Implementa a resolução de um combate entre atacante e defensor.
    /// </summary>
    public static class CombatGraph
    {
        public static Graph Create()
        {
            // Criar todos os nós
            var start = new StartNode { Id = "combat_start" };
            
            var action1 = new PerformActionNode("═══════════════════════════════════════\n  RESOLUÇÃO DE COMBATE\n═══════════════════════════════════════")
            {
                Id = "combat_action1"
            };
            
            // Mostra informações do combate
            var showCombatInfo = new PerformActionNode("Preparando combate: verificando exércitos e dados disponíveis...")
            {
                Id = "combat_show_info"
            };
            
            // Verifica se há comandante (para efeitos de comando)
            var checkCommander = new BinaryConditionNode("O comandante está presente no território de origem?")
            {
                Id = "combat_check_commander"
            };
            
            // Se há comandante, mostra efeito de comando disponível
            var showCommandEffect = new PerformActionNode("Efeito de Comando disponível! (será aplicado na rolagem)")
            {
                Id = "combat_show_command_effect"
            };
            
            // Pergunta se quer usar poder dos deuses (antes da rolagem)
            var askGodPower = new BinaryConditionNode("Você quer invocar o poder de um deus? (antes da rolagem)")
            {
                Id = "combat_ask_god_power"
            };
            
            // Se quer usar poder dos deuses, mostra opções (será implementado depois)
            var useGodPower = new PerformActionNode("Poder dos deuses será invocado. (implementação futura)")
            {
                Id = "combat_use_god_power"
            };
            
            // Rola os dados
            var rollDice = new PerformActionNode("Rolando dados de combate...")
            {
                Id = "combat_roll_dice"
            };
            
            // Mostra resultados da rolagem
            var showRolls = new PerformActionNode("Resultados da rolagem calculados.")
            {
                Id = "combat_show_rolls"
            };
            
            // Compara os dados
            var compareDice = new PerformActionNode("Comparando dados: maior com maior, segundo com segundo...")
            {
                Id = "combat_compare_dice"
            };
            
            // Mostra perdas
            var showLosses = new PerformActionNode("Perdas calculadas: exércitos derrotados em combate.")
            {
                Id = "combat_show_losses"
            };
            
            // Aplica perdas ao estado
            var applyLosses = new PerformActionNode("Aplicando perdas ao estado do jogo...")
            {
                Id = "combat_apply_losses"
            };
            
            // Verifica se o território foi conquistado
            var checkConquest = new BinaryConditionNode("O território foi conquistado? (todos os exércitos defensores foram destruídos)")
            {
                Id = "combat_check_conquest"
            };
            
            // Se foi conquistado, mostra mensagem
            var showConquest = new PerformActionNode("Território conquistado! Você deve mover exércitos para o território conquistado.")
            {
                Id = "combat_show_conquest"
            };
            
            // Pergunta quantos exércitos mover (mínimo 1, máximo 3)
            var askMoveArmies = new PerformActionNode("Quantos exércitos você quer mover para o território conquistado? (mínimo 1, máximo 3)")
            {
                Id = "combat_ask_move_armies"
            };
            
            // Move os exércitos
            var moveArmies = new PerformActionNode("Movendo exércitos para o território conquistado...")
            {
                Id = "combat_move_armies"
            };
            
            // Se não foi conquistado, mostra mensagem
            var noConquest = new PerformActionNode("Território não foi conquistado. Combate finalizado.")
            {
                Id = "combat_no_conquest"
            };
            
            var end = new EndNode("Combate resolvido.")
            {
                Id = "combat_end"
            };
            
            // Conectar os nós
            start.Next = action1;
            action1.Next = showCombatInfo;
            showCombatInfo.Next = checkCommander;
            
            // Se há comandante, mostra efeito
            checkCommander.TrueNode = showCommandEffect;
            showCommandEffect.Next = askGodPower;
            
            // Se não há comandante, pula para pergunta de poder dos deuses
            checkCommander.FalseNode = askGodPower;
            
            // Se quer usar poder dos deuses
            askGodPower.TrueNode = useGodPower;
            useGodPower.Next = rollDice;
            
            // Se não quer usar poder dos deuses
            askGodPower.FalseNode = rollDice;
            
            // Sequência de resolução
            rollDice.Next = showRolls;
            showRolls.Next = compareDice;
            compareDice.Next = showLosses;
            showLosses.Next = applyLosses;
            applyLosses.Next = checkConquest;
            
            // Se foi conquistado
            checkConquest.TrueNode = showConquest;
            showConquest.Next = askMoveArmies;
            askMoveArmies.Next = moveArmies;
            moveArmies.Next = end;
            
            // Se não foi conquistado
            checkConquest.FalseNode = noConquest;
            noConquest.Next = end;
            
            // Criar lista de todos os nós
            var allNodes = new List<Node>
            {
                start,
                action1,
                showCombatInfo,
                checkCommander,
                showCommandEffect,
                askGodPower,
                useGodPower,
                rollDice,
                showRolls,
                compareDice,
                showLosses,
                applyLosses,
                checkConquest,
                showConquest,
                askMoveArmies,
                moveArmies,
                noConquest,
                end
            };
            
            return new Graph("combat", start, allNodes, "CombatGraph.cs");
        }
    }
}

