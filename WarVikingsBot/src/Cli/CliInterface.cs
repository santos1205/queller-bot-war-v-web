using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            // Loop principal de turnos: Bot → Jogador → Bot → Jogador...
            while (true)
            {
                // ====================================================================================
                // TURNO DO BOT
                // ====================================================================================
                RunTurn();
                
                // ====================================================================================
                // TURNO DO JOGADOR
                // ====================================================================================
                Console.WriteLine("\n═══════════════════════════════════════");
                Console.WriteLine("Agora é a vez do jogador!");
                Console.WriteLine("═══════════════════════════════════════");
                Console.WriteLine("\n[Pressione Enter após completar seu turno no tabuleiro físico]");
                Console.ReadLine();
                
                // Reiniciar o crawler para o próximo turno do bot
                _crawler.Reset();
            }
        }
        
        private void RunTurn()
        {
            while (!_crawler.IsAtEnd())
            {
                // PRIMEIRO: Processar automaticamente qualquer coisa que precise ser processada
                // Isso garante que EndNodes com jump ativo sejam processados ANTES de verificar opções
                _crawler.ProcessAutomatic();
                
                // Exibir mensagem atual (após processamento automático)
                var message = _crawler.GetMessage();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Console.WriteLine();
                    Console.WriteLine(message.Trim());
                }
                
                // Obter opções disponíveis (após processamento automático)
                var options = _crawler.GetOptions();
                
                if (options.Count > 0)
                {
                    // Exibir opções formatadas
                    DisplayOptions(options);
                    
                    // Verificar se é a mensagem de alocação de exércitos (sempre pede confirmação)
                    var currentMessage = _crawler.GetMessage();
                    bool isAllocationMessage = !string.IsNullOrEmpty(currentMessage) && 
                                               currentMessage.Contains("Aloque os exércitos");
                    
                    // PerformActionNode em modo bot: avançar automaticamente após 1 segundo
                    // EXCETO se for a mensagem de alocação de exércitos
                    if (options.Count == 1 && string.IsNullOrEmpty(options[0]) && 
                        _crawler.IsBotMode() && !isAllocationMessage)
                    {
                        Thread.Sleep(1000); // Aguarda 1 segundo
                        _crawler.Proceed(""); // Avança automaticamente
                        continue;
                    }
                    
                    // Se for mensagem de alocação ou não estiver em modo bot, aguardar input
                    if (isAllocationMessage && _crawler.IsBotMode())
                    {
                        // Mostra mensagem de confirmação para alocação
                        Console.WriteLine("\n[Pressione Enter após alocar os exércitos no tabuleiro físico]");
                    }
                    
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
                    // Sem opções - pode ser EndNode com jump ativo ou fim real
                    // Se não está no fim, processar automaticamente novamente (pode haver mais processamento)
                    if (!_crawler.IsAtEnd())
                    {
                        // Processar automaticamente novamente (pode haver mais retornos de grafos)
                        _crawler.ProcessAutomatic();
                        // Continuar o loop para verificar novamente
                        continue;
                    }
                    // Se está no fim, apenas aguardar Enter (ou avançar automaticamente em modo bot)
                    if (_crawler.IsBotMode())
                    {
                        Thread.Sleep(1000); // Aguarda 1 segundo em modo bot
                    }
                    else
                    {
                        Console.WriteLine("\n[Pressione Enter para continuar]");
                        Console.ReadLine();
                    }
                }
            }
            
            // Exibir mensagem final do turno
            var finalMessage = _crawler.GetMessage();
            if (!string.IsNullOrWhiteSpace(finalMessage))
            {
                Console.WriteLine();
                Console.WriteLine(finalMessage.Trim());
            }
            
            Console.WriteLine("\n═══════════════════════════════════════");
            Console.WriteLine("Turno do bot concluído!");
            Console.WriteLine("═══════════════════════════════════════");
        }
        
        private void DisplayOptions(List<string> options)
        {
            if (options.Count == 1 && string.IsNullOrEmpty(options[0]))
            {
                // PerformActionNode - apenas Enter
                // Em modo bot, não mostra mensagem e avança automaticamente após 2 segundos
                if (!_crawler.IsBotMode())
                {
                    Console.WriteLine("\n[Pressione Enter para continuar]");
                }
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

