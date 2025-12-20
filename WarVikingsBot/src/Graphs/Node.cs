namespace WarVikingsBot.Graphs
{
    public abstract class Node
    {
        public string Id { get; set; } = string.Empty;
        
        public static bool IsValidId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;
            
            if (!char.IsLower(id[0]))
                return false;
            
            foreach (char c in id)
            {
                if (!char.IsLower(c) && c != '_' && !char.IsDigit(c))
                    return false;
            }
            
            return true;
        }
    }
}

