using System.Collections.Generic;
using System.Linq;

namespace WarVikingsBot.Graphs
{
    public class Graph
    {
        public string Id { get; set; } = string.Empty;
        public StartNode? RootNode { get; set; }
        public List<Node> AllNodes { get; set; } = new List<Node>();
        public string? SourceFile { get; set; }
        
        public Graph(string id, StartNode rootNode, List<Node> allNodes, string? sourceFile = null)
        {
            Id = id;
            RootNode = rootNode;
            AllNodes = allNodes;
            SourceFile = sourceFile;
        }
        
        public Node? GetNodeById(string id)
        {
            return AllNodes.FirstOrDefault(n => n.Id == id);
        }
        
        public List<string> GetJumpTargets()
        {
            return AllNodes
                .OfType<JumpToGraphNode>()
                .Select(j => j.TargetGraphId)
                .ToList();
        }
    }
}

