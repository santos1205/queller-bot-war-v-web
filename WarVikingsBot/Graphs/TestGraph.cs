using System.Collections.Generic;
using WarVikingsBot.Graphs;

namespace WarVikingsBot.Graphs
{
    /// <summary>
    /// Grafo de teste que demonstra o funcionamento de todos os tipos de nós.
    /// Este grafo serve para validar o sistema de grafos de decisão.
    /// </summary>
    public static class TestGraph
    {
        public static Graph Create()
        {
            // Criar todos os nós
            var start = new StartNode { Id = "start" };
            
            var action1 = new PerformActionNode("Bem-vindo ao War Vikings Bot!")
            {
                Id = "action1"
            };
            
            var action2 = new PerformActionNode("Este é um grafo de teste para validar o sistema.")
            {
                Id = "action2"
            };
            
            var question1 = new BinaryConditionNode("Você quer continuar o teste?")
            {
                Id = "question1"
            };
            
            var actionYes = new PerformActionNode("Ótimo! Vamos continuar com uma escolha múltipla.")
            {
                Id = "action_yes"
            };
            
            var actionNo = new PerformActionNode("Entendido. Mesmo assim, vamos mostrar uma escolha múltipla.")
            {
                Id = "action_no"
            };
            
            var multipleChoice = new MultipleChoiceNode("Escolha uma opção para testar:\n  1. Testar PerformActionNode\n  2. Testar navegação automática\n  3. Testar finalização do grafo")
            {
                Id = "multiple_choice"
            };
            
            var option1Action = new PerformActionNode("✓ Opção 1 selecionada: Teste de PerformActionNode concluído!")
            {
                Id = "option1_action"
            };
            
            var option2Action = new PerformActionNode("✓ Opção 2 selecionada: Navegação automática funcionando corretamente!")
            {
                Id = "option2_action"
            };
            
            var option3Action = new PerformActionNode("✓ Opção 3 selecionada: Sistema de finalização validado!")
            {
                Id = "option3_action"
            };
            
            var finalAction = new PerformActionNode("Teste concluído com sucesso! O sistema de grafos está funcionando.")
            {
                Id = "final_action"
            };
            
            var end = new EndNode("Fim do teste. O sistema está pronto para uso!")
            {
                Id = "end"
            };
            
            // Conectar os nós
            start.Next = action1;
            action1.Next = action2;
            action2.Next = question1;
            
            question1.TrueNode = actionYes;
            question1.FalseNode = actionNo;
            
            actionYes.Next = multipleChoice;
            actionNo.Next = multipleChoice;
            
            multipleChoice.Options = new List<Node?> { option1Action, option2Action, option3Action };
            
            option1Action.Next = finalAction;
            option2Action.Next = finalAction;
            option3Action.Next = finalAction;
            
            finalAction.Next = end;
            
            // Criar lista de todos os nós
            var allNodes = new List<Node>
            {
                start,
                action1,
                action2,
                question1,
                actionYes,
                actionNo,
                multipleChoice,
                option1Action,
                option2Action,
                option3Action,
                finalAction,
                end
            };
            
            return new Graph("test_graph", start, allNodes, "TestGraph.cs");
        }
    }
}

