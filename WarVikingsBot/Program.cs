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
                state.CurrentPlayer = 1; // Jogador 1 inicia
                
                // Verificar se deve usar dados de teste (via variável de ambiente ou argumento)
                bool useTestData = Environment.GetEnvironmentVariable("USE_TEST_DATA") == "true" ||
                                 (args.Length > 0 && args[0] == "--test-data");
                
                if (useTestData)
                {
                    state.CurrentRound = 2; // Rodada 2 para permitir ataques
                    state.InitializeTestData();
                    Console.WriteLine("⚠️  MODO DE TESTE ATIVADO: Dados de teste inicializados.");
                    Console.WriteLine();
                }
                else
                {
                    state.CurrentRound = 1; // Primeira rodada
                }
                
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
                
                // Grafo da Fase 4 - Recebimento de Carta de Território
                var phase4Graph = Phase4Graph.Create();
                graphs[phase4Graph.Id] = phase4Graph;
                
                // Grafo principal do turno - conecta todas as fases
                var turnGraph = TurnGraph.Create();
                graphs[turnGraph.Id] = turnGraph;
                
                // Criar GraphCrawler com o grafo principal do turno
                var crawler = new GraphCrawler("turn", graphs, state);
                
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
