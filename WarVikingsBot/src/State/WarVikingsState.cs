using System.Collections.Generic;
using System.Linq;
using WarVikingsBot.Models;

namespace WarVikingsBot.State
{
    public class WarVikingsState
    {
        public CommandEffectType ActiveCommandEffect { get; set; }
        
        public Dictionary<string, Territory> Territories { get; set; } = new Dictionary<string, Territory>();
        
        public Dictionary<int, List<Army>> PlayerArmies { get; set; } = new Dictionary<int, List<Army>>();
        
        public Dictionary<int, int> ValhallaArmies { get; set; } = new Dictionary<int, int>();
        
        public Dictionary<int, List<Ship>> PlayerShips { get; set; } = new Dictionary<int, List<Ship>>();
        
        public Dictionary<int, string> CommanderLocation { get; set; } = new Dictionary<int, string>();
        
        public Dictionary<int, List<string>> TerritoryCards { get; set; } = new Dictionary<int, List<string>>();
        
        public Dictionary<int, string> ObjectiveCards { get; set; } = new Dictionary<int, string>();
        
        public Dictionary<int, Dictionary<GodType, List<bool>>> GodPowerCards { get; set; } = new Dictionary<int, Dictionary<GodType, List<bool>>>();
        
        public int CurrentPlayer { get; set; }
        
        public int CurrentRound { get; set; } = 1;
        
        public bool IsFirstRound => CurrentRound == 1;
        
        public int GetPlayerTerritoryCount(int playerId)
        {
            return Territories.Values.Count(t => t.OccupiedByPlayer == playerId);
        }
        
        public int GetPlayerArmyCount(int playerId, string? territoryName = null)
        {
            if (string.IsNullOrEmpty(territoryName))
            {
                return PlayerArmies.ContainsKey(playerId) 
                    ? PlayerArmies[playerId].Sum(a => a.Value) 
                    : 0;
            }
            
            return Territories.ContainsKey(territoryName) 
                ? Territories[territoryName].ArmyCount 
                : 0;
        }
        
        public int GetValhallaArmyCount(int playerId)
        {
            return ValhallaArmies.ContainsKey(playerId) ? ValhallaArmies[playerId] : 0;
        }
        
        public bool CanAddToValhalla(int playerId)
        {
            return GetValhallaArmyCount(playerId) < 6;
        }
        
        public int GetPlayerShipCount(int playerId)
        {
            if (!PlayerShips.ContainsKey(playerId))
                return 0;
            
            return PlayerShips[playerId].Count(s => !s.IsDestroyed);
        }
        
        public bool CanBuildShip(int playerId)
        {
            return GetValhallaArmyCount(playerId) >= 1 && GetPlayerShipCount(playerId) < 5;
        }
        
        public List<string> GetPlayerTerritories(int playerId)
        {
            return Territories.Values
                .Where(t => t.OccupiedByPlayer == playerId)
                .Select(t => t.Name)
                .ToList();
        }
        
        public List<string> GetAttackableTerritories(int playerId)
        {
            var playerTerritories = GetPlayerTerritories(playerId);
            var attackable = new List<string>();
            
            foreach (var territoryName in playerTerritories)
            {
                if (!Territories.ContainsKey(territoryName))
                    continue;
                
                var territory = Territories[territoryName];
                if (!territory.CanAttack)
                    continue;
                
                foreach (var adjacentName in territory.AdjacentTerritories)
                {
                    if (!Territories.ContainsKey(adjacentName))
                        continue;
                    
                    var adjacent = Territories[adjacentName];
                    if (adjacent.OccupiedByPlayer != playerId && adjacent.OccupiedByPlayer.HasValue)
                    {
                        attackable.Add(adjacentName);
                    }
                }
            }
            
            return attackable.Distinct().ToList();
        }
        
        public int CalculateArmiesFromTerritories(int playerId)
        {
            var territoryCount = GetPlayerTerritoryCount(playerId);
            var armies = territoryCount / 2;
            return territoryCount < 6 ? Math.Max(armies, 3) : armies;
        }
        
        public int GetTerritoryCardCount(int playerId)
        {
            return TerritoryCards.ContainsKey(playerId) ? TerritoryCards[playerId].Count : 0;
        }
        
        public bool MustTradeCards(int playerId)
        {
            return GetTerritoryCardCount(playerId) >= 5;
        }
        
        public bool HasCommanderInTerritory(int playerId, string territoryName)
        {
            return CommanderLocation.ContainsKey(playerId) && 
                   CommanderLocation[playerId] == territoryName;
        }
        
        public bool CanUseCommandEffect(int playerId, string territoryName)
        {
            return HasCommanderInTerritory(playerId, territoryName);
        }
    }
}

