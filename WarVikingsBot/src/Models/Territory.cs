using System.Collections.Generic;

namespace WarVikingsBot.Models
{
    public class Territory
    {
        public string Name { get; set; } = string.Empty;
        public TerritoryType Type { get; set; }
        public bool HasPort => Type == TerritoryType.ComPorto;
        public int? OccupiedByPlayer { get; set; }
        public int ArmyCount { get; set; }
        public string Region { get; set; } = string.Empty;
        public List<string> AdjacentTerritories { get; set; } = new List<string>();
        public bool IsOccupied => OccupiedByPlayer.HasValue && ArmyCount > 0;
        public bool CanAttack => ArmyCount >= 2;
    }
}
