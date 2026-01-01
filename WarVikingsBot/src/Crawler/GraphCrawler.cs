using System;
using System.Collections.Generic;
using System.Linq;
using WarVikingsBot.AI;
using WarVikingsBot.Graphs;
using WarVikingsBot.State;

namespace WarVikingsBot.Crawler
{
    public class GraphCrawler
    {
        private Dictionary<string, Graph> _graphs;
        private WarVikingsState _state;
        private Node? _currentNode;
        private StartNode? _rootNode;
        private Stack<Node> _jumpStack;
        private List<string> _options;
        private string _messageBuffer;
        private BotStrategy? _botStrategy;
        
        /// <summary>
        /// Flag que armazena se o bot tem territórios que podem atacar.
        /// 
        /// Esta flag é definida quando o bot responde à pergunta "Você tem territórios que podem atacar?"
        /// e é usada para pular toda a fase de ataques e combate quando o bot não pode atacar.
        /// 
        /// Valores:
        /// - null: Ainda não foi avaliado
        /// - false: Bot NÃO tem territórios que podem atacar → PULA toda a fase de ataques
        /// - true: Bot TEM territórios que podem atacar → Permite avaliação estratégica
        /// 
        /// IMPORTANTE: Quando esta flag é false, TODAS as perguntas subsequentes de ataque
        /// são respondidas automaticamente como NÃO, sem nem verificar a estratégia do bot.
        /// </summary>
        private bool? _canAttackTerritories = null;
        
        public GraphCrawler(string graphId, Dictionary<string, Graph> graphs, WarVikingsState state)
        {
            _graphs = graphs;
            _state = state;
            _jumpStack = new Stack<Node>();
            _options = new List<string>();
            _messageBuffer = string.Empty;
            
            if (!_graphs.ContainsKey(graphId))
                throw new KeyNotFoundException($"Graph '{graphId}' not found.");
            
            var graph = _graphs[graphId];
            _rootNode = graph.RootNode;
            _currentNode = _rootNode;
            
            // Inicializar estratégia do bot se estiver em modo bot
            if (_state.IsBotMode)
            {
                try
                {
                    var context = new DecisionContext
                    {
                        State = _state,
                        PlayerId = _state.CurrentPlayer,
                        Objective = _state.BotObjective,
                        ObjectiveParameters = _state.BotObjectiveParameters
                    };
                    _botStrategy = new BotStrategy(context);
                }
                catch (Exception ex)
                {
                    // Se houver erro na inicialização, desativa modo bot
                    Console.WriteLine($"⚠️  Erro ao inicializar BotStrategy: {ex.Message}");
                    _state.IsBotMode = false;
                }
            }
            
            AutoCrawl();
        }
        
        public bool IsAtEnd()
        {
            // Se há um jump ativo, não é o fim (ainda há grafos para processar)
            if (_jumpStack.Count > 0)
                return false;
            
            return _currentNode is EndNode;
        }
        
        /// <summary>
        /// Verifica se o bot está em modo automático
        /// </summary>
        public bool IsBotMode()
        {
            return _state.IsBotMode;
        }
        
        public string GetMessage()
        {
            return _messageBuffer;
        }
        
        public List<string> GetOptions()
        {
            // Se chegou em um EndNode com jump ativo, não há opções mas ainda há processamento
            if (_currentNode is EndNode && _jumpStack.Count > 0)
            {
                return new List<string>();
            }
            
            if (_currentNode is InteractiveNode interactiveNode)
            {
                return interactiveNode.GetOptions();
            }
            return new List<string>();
        }
        
        public void Proceed(string option)
        {
            if (_currentNode is InteractiveNode interactiveNode)
            {
                _options.Add(option);
                _currentNode = interactiveNode.GetNext(option);
                AutoCrawl();
            }
            else if (_currentNode is EndNode && _jumpStack.Count > 0)
            {
                // Processar retorno automático de grafos
                AutoCrawl();
            }
            else if (!IsAtEnd() && !(_currentNode is InteractiveNode))
            {
                // Se não está no fim e não é interativo, processar automaticamente
                // (pode ser um nó não-interativo que precisa ser processado)
                AutoCrawl();
            }
        }
        
        /// <summary>
        /// Força o processamento automático de nós não-interativos.
        /// Útil quando não há opções mas ainda há processamento a fazer.
        /// </summary>
        public void ProcessAutomatic()
        {
            // Processar automaticamente apenas se não está no fim E não está em um nó interativo
            // Se está em um nó interativo, não processar automaticamente (aguardar input do usuário)
            if (!IsAtEnd() && !(_currentNode is InteractiveNode))
            {
                AutoCrawl();
            }
        }
        
        public bool CanUndo()
        {
            return _options.Count > 0;
        }
        
        public void Undo()
        {
            if (!CanUndo())
                return;
            
            _options.RemoveAt(_options.Count - 1);
            
            if (_rootNode == null)
                return;
            
            _currentNode = _rootNode;
            _jumpStack.Clear();
            _messageBuffer = string.Empty;
            
            AutoCrawl();
            
            foreach (var option in _options)
            {
                if (_currentNode is InteractiveNode interactiveNode)
                {
                    _currentNode = interactiveNode.GetNext(option);
                    AutoCrawl();
                }
            }
        }
        
        private void AutoCrawl()
        {
            _messageBuffer = string.Empty;
            
            while (_currentNode != null)
            {
                // Se chegou em um EndNode e há um jump ativo, retornar automaticamente ANTES de verificar IsAtEnd()
                if (_currentNode is EndNode && _jumpStack.Count > 0)
                {
                    AddToMessageBuffer(_currentNode);
                    var returnNode = HandleReturn();
                    if (returnNode != null)
                    {
                        _currentNode = returnNode;
                        // Se o nó retornado é um JumpToGraphNode, processar imediatamente
                        if (_currentNode is JumpToGraphNode nextJump)
                        {
                            HandleJump(nextJump);
                            // Continuar processando o grafo destino
                            continue;
                        }
                        // Se não é um JumpToGraphNode, continuar processando o nó retornado
                        continue;
                    }
                    // Se não há retorno, é o fim real
                    break;
                }
                
                // Se chegou em um EndNode sem jump ativo, é o fim real
                if (IsAtEnd())
                    break;
                
                /* ====================================================================================
                 * FASE DE ATAQUES E COMBATE - CÓDIGO DESABILITADO
                 * ====================================================================================
                 * TODO O CÓDIGO RELACIONADO A ATAQUES E COMBATE FOI COMENTADO (DESABILITADO)
                 * 
                 * Esta seção continha:
                 * - Lógica de pulo automático de nós de ataque quando bot não pode atacar
                 * - Avaliação automática de condições de ataque
                 * - Pulo de nós de ação relacionados a combate
                 * - Pulo do grafo de combate
                 * 
                 * Com este código desabilitado, a fase de ataques e combate NÃO será executada.
                 * ==================================================================================== */
                
                // Verificar se é BinaryConditionNode com condições automáticas ANTES de adicionar ao buffer
                if (_currentNode is BinaryConditionNode binaryNode)
                {
                    /* COMENTADO: Lógica de pulo pré-análise de nós de ataque
                    if (_canAttackTerritories.HasValue && _canAttackTerritories.Value == false)
                    {
                        var nodeId = binaryNode.Id?.ToLower() ?? "";
                        var condition = binaryNode.Condition.ToLower();
                        bool isAttackRelated = nodeId.Contains("ask_attack") || 
                                               nodeId == "phase2_ask_attack" ||
                                               nodeId.Contains("attack") ||
                                               condition.Contains("quer realizar um ataque") || 
                                               condition.Contains("quer atacar") ||
                                               condition.Contains("realizar um ataque") ||
                                               (condition.Contains("ataque") && condition.Contains("quer"));
                        
                        if (isAttackRelated)
                        {
                            _messageBuffer += binaryNode.Condition + "\n";
                            _messageBuffer += "\n🤖 [BOT] Decisão: NÃO (Pulando fase de ataques - sem territórios que podem atacar)\n";
                            _currentNode = binaryNode.FalseNode;
                            continue;
                        }
                    }
                    */
                    
                    // Avaliar condição automaticamente (sem lógica de ataque)
                    var autoResult = EvaluateAutoCondition(binaryNode);
                    if (autoResult.HasValue)
                    {
                        /* COMENTADO: Lógica de forçar false para ataques
                        if (_canAttackTerritories.HasValue && _canAttackTerritories.Value == false)
                        {
                            var nodeId = binaryNode.Id?.ToLower() ?? "";
                            var condition = binaryNode.Condition.ToLower();
                            bool isAttackRelated = nodeId.Contains("ask_attack") || 
                                                   nodeId == "phase2_ask_attack" ||
                                                   condition.Contains("quer realizar um ataque") || 
                                                   condition.Contains("quer atacar");
                            
                            if (isAttackRelated)
                            {
                                autoResult = false;
                            }
                        }
                        */
                        
                        // Condição automática - mostrar decisão do bot e seguir automaticamente
                        var decision = autoResult.Value ? "SIM" : "NÃO";
                        var botDecision = $"\n🤖 [BOT] Decisão: {decision}";
                        
                        /* COMENTADO: Contexto de decisão de ataque
                        if (_state.IsBotMode && _botStrategy != null)
                        {
                            var condition = binaryNode.Condition.ToLower();
                            if (condition.Contains("quer realizar um ataque") || 
                                condition.Contains("quer atacar") ||
                                condition.Contains("ask_attack"))
                            {
                                if (autoResult.Value)
                                {
                                    botDecision += " (Bot decidiu atacar baseado na estratégia e vantagem numérica)";
                                }
                                else
                                {
                                    botDecision += " (Bot decidiu não atacar - sem vantagem clara ou sem alvos adequados)";
                                }
                            }
                        }
                        */
                        
                        _messageBuffer += binaryNode.Condition + "\n";
                        _messageBuffer += botDecision + "\n";
                        _currentNode = autoResult.Value ? binaryNode.TrueNode : binaryNode.FalseNode;
                        continue;
                    }
                    // Se não for automática, adiciona ao buffer e para para aguardar interação
                    AddToMessageBuffer(_currentNode);
                    break;
                }
                
                /* COMENTADO: Lógica de pulo de nós de ataque/combate
                if (_canAttackTerritories.HasValue && _canAttackTerritories.Value == false)
                {
                    if (_currentNode is ExecuteActionNode executeActionCheck)
                    {
                        var actionId = executeActionCheck.ActionId?.ToLower() ?? "";
                        var message = executeActionCheck.Message?.ToLower() ?? "";
                        bool isAttackAction = actionId.Contains("combat") || 
                                             actionId.Contains("attack") ||
                                             message.Contains("combate") ||
                                             message.Contains("ataque") ||
                                             message.Contains("origem") ||
                                             message.Contains("alvo");
                        
                        if (isAttackAction)
                        {
                            _currentNode = GetNextNode(_currentNode);
                            continue;
                        }
                    }
                    
                    if (_currentNode is PerformActionNode performActionCheck)
                    {
                        var action = performActionCheck.Action?.ToLower() ?? "";
                        var nodeId = performActionCheck.Id?.ToLower() ?? "";
                        bool isAttackAction = action.Contains("combate") ||
                                             action.Contains("ataque") ||
                                             action.Contains("origem") ||
                                             action.Contains("alvo") ||
                                             nodeId.Contains("combat") ||
                                             nodeId.Contains("attack") ||
                                             nodeId.Contains("source") ||
                                             nodeId.Contains("target");
                        
                        if (isAttackAction && !action.Contains("não tem territórios") && !action.Contains("não há ataques"))
                        {
                            _currentNode = GetNextNode(_currentNode);
                            continue;
                        }
                    }
                    
                    if (_currentNode is JumpToGraphNode jumpGraphCheck)
                    {
                        var graphId = jumpGraphCheck.TargetGraphId?.ToLower() ?? "";
                        if (graphId.Contains("combat"))
                        {
                            _currentNode = GetNextNode(_currentNode);
                            continue;
                        }
                    }
                }
                */
                
                // Se for PerformActionNode sobre movimento de exércitos e estiver em modo bot,
                // mostrar decisão do bot antes da mensagem
                if (_currentNode is PerformActionNode performNode && 
                    _state.IsBotMode && 
                    _botStrategy != null &&
                    performNode.Action.Contains("Quantos exércitos você quer mover"))
                {
                    // Bot já vai decidir no ExecuteActionNode, mas vamos mostrar a decisão aqui
                    if (!string.IsNullOrEmpty(_state.CurrentCombatSourceTerritory) &&
                        !string.IsNullOrEmpty(_state.CurrentCombatTargetTerritory))
                    {
                        var armiesToMove = _botStrategy.DecideArmiesToMoveAfterConquest(
                            _state.CurrentCombatSourceTerritory,
                            _state.CurrentCombatTargetTerritory
                        );
                        _messageBuffer += $"\n🤖 [BOT] Decidiu mover {armiesToMove} exército(s) para o território conquistado\n";
                    }
                }
                
                AddToMessageBuffer(_currentNode);
                
                // Executar ação se for ExecuteActionNode
                if (_currentNode is ExecuteActionNode executeNode)
                {
                    ExecuteAction(executeNode);
                }
                
                if (_currentNode is InteractiveNode)
                    break;
                
                // Verificar se é JumpToGraphNode antes de chamar GetNextNode
                // porque HandleJump já atualiza _currentNode
                if (_currentNode is JumpToGraphNode jumpNode)
                {
                    HandleJump(jumpNode);
                    // _currentNode já foi atualizado para o root do grafo destino
                    // Continuar o loop para processar o novo nó
                    continue;
                }
                
                _currentNode = GetNextNode(_currentNode);
            }
        }
        
        /// <summary>
        /// Avalia condições que podem ser resolvidas automaticamente pelo estado.
        /// Retorna null se a condição precisa de interação do usuário.
        /// </summary>
        private bool? EvaluateAutoCondition(BinaryConditionNode node)
        {
            var condition = node.Condition.ToLower();
            var playerId = _state.CurrentPlayer;
            
            // Verificar condições conhecidas
            if (condition.Contains("primeira rodada") || condition.Contains("é a primeira rodada"))
            {
                return _state.IsFirstRound;
            }
            
            if (condition.Contains("check_first_round"))
            {
                return _state.IsFirstRound;
            }
            
            if (condition.Contains("check_conquered_this_turn"))
            {
                return _state.HasConqueredTerritoryThisTurn(playerId);
            }
            
            // ====================================================================================
            // AVALIAÇÃO: "Você tem territórios que podem atacar?" - SEMPRE RETORNA FALSE
            // ====================================================================================
            // Como toda a fase de ataques está desabilitada, esta pergunta sempre retorna FALSE
            // para garantir que a fase de ataques seja pulada automaticamente.
            // ====================================================================================
            if (condition.Contains("territórios que podem atacar") || condition.Contains("territorios que podem atacar"))
            {
                // SEMPRE retorna false quando a fase de ataques está desabilitada
                return false;
            }
            
            // ====================================================================================
            // AVALIAÇÃO: "Você tem possibilidades de deslocamento?"
            // ====================================================================================
            // Verifica se o jogador tem territórios contíguos com mais de 1 exército que podem
            // ser deslocados. Retorna true se houver possibilidades, false caso contrário.
            // ====================================================================================
            if (condition.Contains("possibilidades de deslocamento") || condition.Contains("possibilidade de deslocamento"))
            {
                // Verifica se há territórios que podem deslocar (territórios com > 1 exército)
                // O método GetMovementSourceTerritories já verifica se há destinos disponíveis
                var movementSources = _state.GetMovementSourceTerritories(playerId);
                return movementSources.Count > 0; // Retorna true se houver pelo menos uma possibilidade de deslocamento
            }
            
            /* ====================================================================================
             * AVALIAÇÃO: "Você tem territórios que podem atacar?" - CÓDIGO ORIGINAL DESABILITADO
             * ====================================================================================
             * TODO O CÓDIGO DE AVALIAÇÃO DE TERRITÓRIOS QUE PODEM ATACAR FOI COMENTADO (DESABILITADO)
             * 
             * Esta seção continha:
             * - Validação de primeira rodada
             * - Verificação de territórios com >= 2 exércitos
             * - Verificação de alvos adjacentes inimigos
             * - Definição da flag _canAttackTerritories
             * 
             * Com este código desabilitado, a pergunta não será respondida automaticamente.
             * ==================================================================================== */
            /* COMENTADO: Avaliação de territórios que podem atacar (código original)
            if (condition.Contains("territórios que podem atacar") || condition.Contains("territorios que podem atacar"))
            {
                if (_state.IsFirstRound)
                {
                    _canAttackTerritories = false;
                    return false;
                }
                
                var sources = _state.GetAttackSourceTerritories(playerId);
                if (sources.Count == 0)
                {
                    _canAttackTerritories = false;
                    return false;
                }
                
                bool hasAnyTarget = false;
                foreach (var source in sources)
                {
                    var targets = _state.GetAttackableTargetsFromSource(playerId, source);
                    if (targets.Count > 0)
                    {
                        hasAnyTarget = true;
                        break;
                    }
                }
                
                if (!hasAnyTarget)
                {
                    _canAttackTerritories = false;
                    return false;
                }
                
                _canAttackTerritories = true;
                return true;
            }
            */
            
            /* ====================================================================================
             * DECISÕES DO BOT - CÓDIGO DE ATAQUE DESABILITADO
             * ====================================================================================
             * TODO O CÓDIGO RELACIONADO A DECISÕES DE ATAQUE DO BOT FOI COMENTADO (DESABILITADO)
             * 
             * Esta seção continha:
             * - Avaliação de "Você quer realizar um ataque?"
             * - Validações de territórios que podem atacar
             * - Consulta à estratégia do bot para decidir se ataca
             * 
             * Com este código desabilitado, o bot NÃO tomará decisões de ataque.
             * ==================================================================================== */
            
            // Decisões do bot (se estiver em modo bot) - ATACAR DESABILITADO
            /* COMENTADO: Toda a lógica de decisão de ataque do bot
            if (_state.IsBotMode && _botStrategy != null)
            {
                var nodeId = node.Id?.ToLower() ?? "";
                bool isAttackQuestion = nodeId.Contains("ask_attack") || nodeId == "phase2_ask_attack" ||
                                       condition.Contains("quer realizar um ataque") || 
                                       condition.Contains("quer atacar") ||
                                       condition.Contains("realizar um ataque") ||
                                       (condition.Contains("ataque") && condition.Contains("quer"));
                
                if (isAttackQuestion)
                {
                    if (_canAttackTerritories.HasValue && _canAttackTerritories.Value == false)
                    {
                        return false;
                    }
                    
                    if (!_canAttackTerritories.HasValue)
                    {
                        var attackSources = _state.GetAttackSourceTerritories(playerId);
                        if (attackSources.Count == 0)
                        {
                            _canAttackTerritories = false;
                            return false;
                        }
                        
                        var attackableTargets = _state.GetAttackableTerritories(playerId);
                        if (attackableTargets.Count == 0)
                        {
                            _canAttackTerritories = false;
                            return false;
                        }
                        
                        _canAttackTerritories = true;
                    }
                    
                    if (_canAttackTerritories.HasValue && _canAttackTerritories.Value == false)
                    {
                        return false;
                    }
                    
                    var finalCheckSources = _state.GetAttackSourceTerritories(playerId);
                    var finalCheckTargets = _state.GetAttackableTerritories(playerId);
                    if (finalCheckSources.Count == 0 || finalCheckTargets.Count == 0)
                    {
                        _canAttackTerritories = false;
                        return false;
                    }
                    
                    try
                    {
                        return _botStrategy.ShouldAttack();
                    }
                    catch
                    {
                        _canAttackTerritories = false;
                        return false;
                    }
                }
            }
            */
            
            // Se não for uma condição automática, retorna null para aguardar interação
            return null;
        }
        
        private void ExecuteAction(ExecuteActionNode node)
        {
            var actionId = node.ActionId.ToLower();
            var state = _state;
            var playerId = state.CurrentPlayer;
            
            switch (actionId)
            {
                /* COMENTADO: Ações de combate desabilitadas
                case "resolve_combat":
                    if (!string.IsNullOrEmpty(state.CurrentCombatSourceTerritory) && 
                        !string.IsNullOrEmpty(state.CurrentCombatTargetTerritory))
                    {
                        state.CurrentCombatResult = state.ResolveCombat(
                            playerId,
                            state.CurrentCombatSourceTerritory,
                            state.CurrentCombatTargetTerritory
                        );
                    }
                    break;
                    
                case "apply_combat_losses":
                    if (state.CurrentCombatResult != null &&
                        !string.IsNullOrEmpty(state.CurrentCombatSourceTerritory) &&
                        !string.IsNullOrEmpty(state.CurrentCombatTargetTerritory))
                    {
                        state.ApplyCombatLosses(
                            playerId,
                            state.CurrentCombatSourceTerritory,
                            state.CurrentCombatTargetTerritory,
                            state.CurrentCombatResult
                        );
                    }
                    break;
                */
                    
                /* COMENTADO: Movimento de exércitos após conquista desabilitado
                case "move_armies_after_conquest":
                case "move_armies":
                    if (!string.IsNullOrEmpty(state.CurrentCombatSourceTerritory) &&
                        !string.IsNullOrEmpty(state.CurrentCombatTargetTerritory))
                    {
                        int armiesToMove = 1; // Padrão: mínimo
                        
                        if (state.IsBotMode && _botStrategy != null)
                        {
                            armiesToMove = _botStrategy.DecideArmiesToMoveAfterConquest(
                                state.CurrentCombatSourceTerritory,
                                state.CurrentCombatTargetTerritory
                            );
                            _messageBuffer += $"\n🤖 [BOT] Decidiu mover {armiesToMove} exército(s) para o território conquistado\n";
                        }
                        
                        state.MoveArmiesAfterConquest(
                            playerId,
                            state.CurrentCombatSourceTerritory,
                            state.CurrentCombatTargetTerritory,
                            armiesToMove
                        );
                    }
                    break;
                */
                    
                /* COMENTADO: Seleção de territórios de combate desabilitada
                case "set_combat_source":
                    // Define o território de origem do combate
                    if (state.IsBotMode && _botStrategy != null)
                    {
                        var selectedSource = _botStrategy.SelectAttackSourceTerritory();
                        if (!string.IsNullOrEmpty(selectedSource))
                        {
                            state.CurrentCombatSourceTerritory = selectedSource;
                            _messageBuffer += $"\n🤖 [BOT] Escolheu território de origem: {selectedSource}\n";
                        }
                    }
                    else
                    {
                        var sources = state.GetAttackSourceTerritories(playerId);
                        if (sources.Count > 0)
                        {
                            state.CurrentCombatSourceTerritory = sources[0];
                        }
                    }
                    break;
                    
                case "set_combat_target":
                    // Define o território alvo do combate
                    if (!string.IsNullOrEmpty(state.CurrentCombatSourceTerritory))
                    {
                        if (state.IsBotMode && _botStrategy != null)
                        {
                            var selectedTarget = _botStrategy.SelectAttackTargetTerritory(state.CurrentCombatSourceTerritory);
                            if (!string.IsNullOrEmpty(selectedTarget))
                            {
                                state.CurrentCombatTargetTerritory = selectedTarget;
                                _messageBuffer += $"\n🤖 [BOT] Escolheu território alvo: {selectedTarget}\n";
                            }
                        }
                        else
                        {
                            var targets = state.GetAttackableTargetsFromSource(playerId, state.CurrentCombatSourceTerritory);
                            if (targets.Count > 0)
                            {
                                state.CurrentCombatTargetTerritory = targets[0];
                            }
                        }
                    }
                    break;
                */
                    
                case "set_movement_source":
                    // Define o território de origem do deslocamento
                    // Por enquanto, usa o primeiro território que pode deslocar
                    // TODO: Implementar seleção real do usuário
                    var movementSources = state.GetMovementSourceTerritories(playerId);
                    if (movementSources.Count > 0)
                    {
                        state.CurrentMovementSourceTerritory = movementSources[0];
                    }
                    break;
                    
                case "set_movement_target":
                    // Define o território de destino do deslocamento
                    // Por enquanto, usa o primeiro território contíguo do mesmo jogador
                    // TODO: Implementar seleção real do usuário
                    if (!string.IsNullOrEmpty(state.CurrentMovementSourceTerritory))
                    {
                        var movementTargets = state.GetMovementTargetTerritories(playerId, state.CurrentMovementSourceTerritory);
                        if (movementTargets.Count > 0)
                        {
                            state.CurrentMovementTargetTerritory = movementTargets[0];
                        }
                    }
                    break;
                    
                case "execute_movement":
                    // Executa o deslocamento de exércitos
                    if (!string.IsNullOrEmpty(state.CurrentMovementSourceTerritory) &&
                        !string.IsNullOrEmpty(state.CurrentMovementTargetTerritory))
                    {
                        // Por padrão, move 1 exército (mínimo)
                        // TODO: Permitir que o usuário escolha quantos mover
                        int armiesToMove = state.CurrentMovementArmies > 0 
                            ? state.CurrentMovementArmies 
                            : 1;
                        
                        state.ExecuteMovement(
                            playerId,
                            state.CurrentMovementSourceTerritory,
                            state.CurrentMovementTargetTerritory,
                            armiesToMove
                        );
                    }
                    break;
                    
                case "receive_territory_card":
                    // Recebe uma carta de território
                    // Por enquanto, recebe uma carta do primeiro território conquistado
                    // TODO: Implementar seleção real do território
                    var conquered = state.GetConqueredTerritoriesThisTurn(playerId);
                    if (conquered.Count > 0)
                    {
                        state.ReceiveTerritoryCard(playerId, conquered[0]);
                    }
                    break;
                    
                case "clear_conquered_territories":
                    // Limpa o rastreamento de conquistas do turno anterior
                    state.ClearConqueredTerritoriesThisTurn(playerId);
                    break;
                    
                default:
                    // Ação desconhecida - não faz nada
                    break;
            }
        }
        
        private void AddToMessageBuffer(Node node)
        {
            if (node is EndNode endNode)
            {
                _messageBuffer += endNode.GetMessage() + "\n";
            }
            else if (node is InteractiveNode interactiveNode)
            {
                _messageBuffer += interactiveNode.GetMessage() + "\n";
            }
            else if (node is ReturnFromGraphNode returnNode)
            {
                _messageBuffer += returnNode.GetMessage() + "\n";
            }
            else if (node is PerformActionNode performNode)
            {
                _messageBuffer += performNode.GetMessage() + "\n";
            }
        }
        
        private Node? GetNextNode(Node node)
        {
            if (node is NonInteractiveNode nonInteractiveNode)
            {
                // JumpToGraphNode é tratado diretamente em AutoCrawl()
                // para evitar recursão infinita
                
                if (node is ReturnFromGraphNode)
                {
                    return HandleReturn();
                }
                
                var next = nonInteractiveNode.GetNext();
                return next;
            }
            
            return null;
        }
        
        private void HandleJump(JumpToGraphNode jumpNode)
        {
            _jumpStack.Push(jumpNode);
            
            if (!_graphs.ContainsKey(jumpNode.TargetGraphId))
                throw new KeyNotFoundException($"Target graph '{jumpNode.TargetGraphId}' not found.");
            
            var targetGraph = _graphs[jumpNode.TargetGraphId];
            _currentNode = targetGraph.RootNode;
        }
        
        private Node? HandleReturn()
        {
            if (_jumpStack.Count == 0)
                return null;
            
            var jumpNode = _jumpStack.Pop();
            if (jumpNode is JumpToGraphNode jump)
            {
                return jump.GetNext();
            }
            return null;
        }
        
        public WarVikingsState GetState()
        {
            return _state;
        }
    }
}

