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
            return _currentNode is EndNode;
        }
        
        public string GetMessage()
        {
            return _messageBuffer;
        }
        
        public List<string> GetOptions()
        {
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
            
            while (!IsAtEnd() && _currentNode != null)
            {
                AddToMessageBuffer(_currentNode);
                
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

