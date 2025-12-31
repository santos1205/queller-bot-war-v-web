using System.Collections.Generic;
using System.Linq;
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
            
            AutoCrawl();
        }
        
        public bool IsAtEnd()
        {
            // Se há um jump ativo, não é o fim (ainda há grafos para processar)
            if (_jumpStack.Count > 0)
                return false;
            
            return _currentNode is EndNode;
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
                
                AddToMessageBuffer(_currentNode);
                
                // Executar ação se for ExecuteActionNode
                if (_currentNode is ExecuteActionNode executeNode)
                {
                    ExecuteAction(executeNode);
                }
                
                // Verificar se é BinaryConditionNode com condições automáticas
                if (_currentNode is BinaryConditionNode binaryNode)
                {
                    var autoResult = EvaluateAutoCondition(binaryNode);
                    if (autoResult.HasValue)
                    {
                        // Condição automática - seguir automaticamente
                        _currentNode = autoResult.Value ? binaryNode.TrueNode : binaryNode.FalseNode;
                        continue;
                    }
                    // Se não for automática, para e aguarda interação do usuário
                    break;
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
            
            // Verificar se tem territórios que podem atacar
            if (condition.Contains("territórios que podem atacar") || condition.Contains("territorios que podem atacar"))
            {
                var sources = _state.GetAttackSourceTerritories(playerId);
                return sources.Count > 0;
            }
            
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
                    
                case "move_armies_after_conquest":
                case "move_armies":
                    if (!string.IsNullOrEmpty(state.CurrentCombatSourceTerritory) &&
                        !string.IsNullOrEmpty(state.CurrentCombatTargetTerritory))
                    {
                        // Por padrão, move 1 exército (mínimo)
                        // TODO: Permitir que o usuário escolha quantos mover
                        state.MoveArmiesAfterConquest(
                            playerId,
                            state.CurrentCombatSourceTerritory,
                            state.CurrentCombatTargetTerritory,
                            1 // Por padrão, move 1 exército
                        );
                    }
                    break;
                    
                case "set_combat_source":
                    // Define o território de origem do combate
                    // Por enquanto, usa o primeiro território atacável do jogador
                    // TODO: Implementar seleção real do usuário
                    var sources = state.GetAttackSourceTerritories(playerId);
                    if (sources.Count > 0)
                    {
                        state.CurrentCombatSourceTerritory = sources[0];
                    }
                    break;
                    
                case "set_combat_target":
                    // Define o território alvo do combate
                    // Por enquanto, usa o primeiro território atacável a partir da origem
                    // TODO: Implementar seleção real do usuário
                    if (!string.IsNullOrEmpty(state.CurrentCombatSourceTerritory))
                    {
                        var targets = state.GetAttackableTargetsFromSource(playerId, state.CurrentCombatSourceTerritory);
                        if (targets.Count > 0)
                        {
                            state.CurrentCombatTargetTerritory = targets[0];
                        }
                    }
                    break;
                    
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

