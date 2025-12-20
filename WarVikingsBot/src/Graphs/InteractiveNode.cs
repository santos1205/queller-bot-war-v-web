using System.Collections.Generic;

namespace WarVikingsBot.Graphs
{
    public abstract class InteractiveNode : Node
    {
        public abstract string GetMessage();
        public abstract List<string> GetOptions();
        public abstract Node? GetNext(string option);
    }
}

