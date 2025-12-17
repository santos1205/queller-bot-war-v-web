namespace WarVikingsBot.Models
{
    public class Army
    {
        public ArmyType Type { get; set; }
        public int Value => (int)Type;
        public string TerritoryName { get; set; } = string.Empty;
        public int PlayerId { get; set; }
    }
}
