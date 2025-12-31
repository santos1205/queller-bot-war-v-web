using System;
using System.Collections.Generic;
using System.Linq;
using WarVikingsBot.Crawler;

namespace WarVikingsBot.Cli
{
    /// <summary>
    /// Interface CLI básica para interagir com o GraphCrawler.
    /// </summary>
    public class CliInterface
    {
        private GraphCrawler _crawler;
        
        public CliInterface(GraphCrawler crawler)
        {
            _crawler = crawler;
        }
        
        public void Run()
        {
            while (!_crawler.IsAtEnd())
            {
                // Exibir mensagem atual
                var message = _crawler.GetMessage();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine();
                    Console.WriteLine(message.Trim());
                }
                
                // Obter opções disponíveis
                var options = _crawler.GetOptions();
                
                if (options.Count > 0)
                {
                    // Exibir opções formatadas
                    DisplayOptions(options);
                    
                    // Aguardar input do usuário
                    var input = GetUserInput();
                    
                    // Processar comandos especiais
                    if (ProcessSpecialCommands(input))
                        continue;
                    
                    // Validar e processar opção
                    if (ValidateAndProcessOption(input, options))
                        continue;
                    
                    Console.WriteLine("Opção inválida. Tente novamente.");
                }
                else
                {
                    // Sem opções, apenas aguardar Enter
                    Console.WriteLine("\n[Pressione Enter para continuar]");
                    Console.ReadLine();
                }
            }
            
            // Exibir mensagem final
            var finalMessage = _crawler.GetMessage();
            if (!string.IsNullOrWhiteSpace(finalMessage))
            {
                Console.WriteLine();
                Console.WriteLine(finalMessage.Trim());
            }
            
            Console.WriteLine("\n═══════════════════════════════════════");
            Console.WriteLine("Grafo concluído!");
            Console.WriteLine("═══════════════════════════════════════");
        }
        
        private void DisplayOptions(List<string> options)
        {
            if (options.Count == 1 && string.IsNullOrEmpty(options[0]))
            {
                // PerformActionNode - apenas Enter
                Console.WriteLine("\n[Pressione Enter para continuar]");
            }
            else if (options.Count == 2 && options.Contains("true") && options.Contains("false"))
            {
                // BinaryConditionNode
                Console.WriteLine("\n[true/false ou t/f]");
            }
            else
            {
                // MultipleChoiceNode - as opções já estão na mensagem
                Console.WriteLine($"\n[Digite 1-{options.Count}]");
            }
        }
        
        private string GetUserInput()
        {
            Console.Write("> ");
            return Console.ReadLine()?.Trim() ?? "";
        }
        
        private bool ProcessSpecialCommands(string input)
        {
            var command = input.ToLower();
            
            switch (command)
            {
                case "help":
                    ShowHelp();
                    return true;
                    
                case "undo":
                    if (_crawler.CanUndo())
                    {
                        _crawler.Undo();
                        Console.WriteLine("✓ Última escolha desfeita.");
                    }
                    else
                    {
                        Console.WriteLine("✗ Não há escolhas para desfazer.");
                    }
                    return true;
                    
                case "exit":
                    Console.WriteLine("Saindo...");
                    Environment.Exit(0);
                    return true;
                    
                default:
                    return false;
            }
        }
        
        private bool ValidateAndProcessOption(string input, List<string> options)
        {
            // PerformActionNode (apenas Enter)
            if (options.Count == 1 && string.IsNullOrEmpty(options[0]))
            {
                if (string.IsNullOrEmpty(input))
                {
                    _crawler.Proceed("");
                    return true;
                }
                return false;
            }
            
            // BinaryConditionNode
            if (options.Contains("true") && options.Contains("false"))
            {
                var normalized = input.ToLower();
                if (normalized == "true" || normalized == "t")
                {
                    _crawler.Proceed("true");
                    return true;
                }
                if (normalized == "false" || normalized == "f")
                {
                    _crawler.Proceed("false");
                    return true;
                }
                return false;
            }
            
            // MultipleChoiceNode
            if (int.TryParse(input, out int index))
            {
                if (index >= 1 && index <= options.Count)
                {
                    _crawler.Proceed(index.ToString());
                    return true;
                }
            }
            
            return false;
        }
        
        private void ShowHelp()
        {
            Console.WriteLine();
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("  Comandos Disponíveis");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("  help  - Mostra esta ajuda");
            Console.WriteLine("  undo  - Desfaz a última escolha");
            Console.WriteLine("  exit  - Sai do programa");
            Console.WriteLine();
            Console.WriteLine("Durante o jogo:");
            Console.WriteLine("  - Pressione Enter para continuar ações");
            Console.WriteLine("  - Digite 'true'/'t' ou 'false'/'f' para perguntas sim/não");
            Console.WriteLine("  - Digite o número da opção para escolhas múltiplas");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine();
        }
    }
}

