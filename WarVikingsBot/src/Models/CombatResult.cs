using System.Collections.Generic;

namespace WarVikingsBot.Models
{
    public class CombatResult
    {
        public List<int> AttackerRolls { get; set; } = new List<int>();
        public List<int> DefenderRolls { get; set; } = new List<int>();
        public List<DiceComparison> Comparisons { get; set; } = new List<DiceComparison>();
        public int AttackerLosses { get; set; }
        public int DefenderLosses { get; set; }
        public bool TerritoryConquered { get; set; }
    }
    
    public class DiceComparison
    {
        public int AttackerValue { get; set; }
        public int DefenderValue { get; set; }
        public bool AttackerWins => AttackerValue > DefenderValue;
        public bool DefenderWins => AttackerValue <= DefenderValue;
    }
}
