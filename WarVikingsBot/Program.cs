using System;
using System.Collections.Generic;
using WarVikingsBot.Cli;
using WarVikingsBot.Crawler;
using WarVikingsBot.Graphs;
using WarVikingsBot.State;

namespace WarVikingsBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("  War Vikings Bot - CLI");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine();
            Console.WriteLine("Bem-vindo ao War Vikings Bot!");
            Console.WriteLine("Sistema de IA para jogar War Vikings solo.");
            Console.WriteLine();
            Console.WriteLine("Digite 'help' para mais informações.");
            Console.WriteLine("Digite 'exit' para sair.");
            Console.WriteLine();
            
            try
            {
                // Criar estado inicial do jogo
                var state = new WarVikingsState();
                
                // Criar e registrar grafos
                var graphs = new Dictionary<string, Graph>();
                
                // Grafo de teste
                var testGraph = TestGraph.Create();
                graphs[testGraph.Id] = testGraph;
                
                // Grafo da Fase 1 - Recebimento de Exércitos
                var phase1Graph = Phase1Graph.Create();
                graphs[phase1Graph.Id] = phase1Graph;
                
                // Grafo de Troca de Cartas
                var cardTradeGraph = CardTradeGraph.Create();
                graphs[cardTradeGraph.Id] = cardTradeGraph;
                
                // Grafo da Fase 2 - Ataques
                var phase2Graph = Phase2Graph.Create();
                graphs[phase2Graph.Id] = phase2Graph;
                
                // Grafo de Combate
                var combatGraph = CombatGraph.Create();
                graphs[combatGraph.Id] = combatGraph;
                
                // Grafo da Fase 3 - Deslocamento de Exércitos
                var phase3Graph = Phase3Graph.Create();
                graphs[phase3Graph.Id] = phase3Graph;
                
                // Criar GraphCrawler com o grafo da Fase 1 (ou test_graph para testes)
                var crawler = new GraphCrawler("phase_1", graphs, state);
                
                // Criar e executar interface CLI
                var cli = new CliInterface(crawler);
                cli.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("═══════════════════════════════════════");
                Console.WriteLine("  ERRO");
                Console.WriteLine("═══════════════════════════════════════");
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine("Pressione qualquer tecla para sair...");
                Console.ReadKey();
            }
        }
    }
}
