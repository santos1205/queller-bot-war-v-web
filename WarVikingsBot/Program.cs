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
                var testGraph = TestGraph.Create();
                graphs[testGraph.Id] = testGraph;
                
                // Criar GraphCrawler com o grafo de teste
                var crawler = new GraphCrawler("test_graph", graphs, state);
                
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
