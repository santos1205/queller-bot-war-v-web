namespace WarVikingsBot.Models
{
    public class Ship
    {
        public string Id { get; set; } = string.Empty;
        public string TerritoryName { get; set; } = string.Empty;
        public int PlayerId { get; set; }
        public bool IsInCombat { get; set; }
        public bool IsDestroyed { get; set; }
    }
}
