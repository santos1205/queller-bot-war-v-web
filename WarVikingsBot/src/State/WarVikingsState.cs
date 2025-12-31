using System;
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
        
        // Rastreamento de trocas de cartas (para valores progressivos)
        public Dictionary<int, int> CardTradeCount { get; set; } = new Dictionary<int, int>();
        
        public int CurrentPlayer { get; set; }
        
        public int CurrentRound { get; set; } = 1;
        
        public bool IsFirstRound => CurrentRound == 1;
        
        // Propriedades temporárias para combate atual
        public string? CurrentCombatSourceTerritory { get; set; }
        public string? CurrentCombatTargetTerritory { get; set; }
        public CombatResult? CurrentCombatResult { get; set; }
        
        // Propriedades temporárias para deslocamento atual
        public string? CurrentMovementSourceTerritory { get; set; }
        public string? CurrentMovementTargetTerritory { get; set; }
        public int CurrentMovementArmies { get; set; }
        
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
        
        // Novos métodos para Fase 1
        
        /// <summary>
        /// Obtém todas as regiões conquistadas completamente por um jogador.
        /// Uma região é considerada conquistada se todos os territórios da região pertencem ao jogador.
        /// </summary>
        public List<string> GetConqueredRegions(int playerId)
        {
            var conqueredRegions = new List<string>();
            var allRegions = Territories.Values.Select(t => t.Region).Distinct().ToList();
            
            foreach (var region in allRegions)
            {
                var regionTerritories = Territories.Values
                    .Where(t => t.Region == region)
                    .ToList();
                
                if (regionTerritories.Count == 0)
                    continue;
                
                // Verifica se todos os territórios da região pertencem ao jogador
                bool allConquered = regionTerritories.All(t => t.OccupiedByPlayer == playerId);
                
                if (allConquered)
                {
                    conqueredRegions.Add(region);
                }
            }
            
            return conqueredRegions;
        }
        
        /// <summary>
        /// Calcula exércitos recebidos por regiões conquistadas.
        /// Valores padrão: será implementado conforme tabela do tabuleiro.
        /// Por enquanto, retorna valores fixos por região.
        /// </summary>
        public int CalculateArmiesFromRegions(int playerId)
        {
            var conqueredRegions = GetConqueredRegions(playerId);
            int totalArmies = 0;
            
            // Valores padrão por região (será ajustado conforme tabela do tabuleiro)
            // Por enquanto, cada região conquistada dá 2 exércitos
            // TODO: Implementar valores reais da tabela do tabuleiro
            foreach (var region in conqueredRegions)
            {
                totalArmies += 2; // Valor temporário - será substituído por valores da tabela
            }
            
            return totalArmies;
        }
        
        /// <summary>
        /// Verifica se o jogador pode trocar cartas (tem pelo menos 3 cartas).
        /// </summary>
        public bool CanTradeCards(int playerId)
        {
            return GetTerritoryCardCount(playerId) >= 3;
        }
        
        /// <summary>
        /// Verifica se o jogador tem 3 cartas iguais para trocar.
        /// </summary>
        public bool HasThreeSameCards(int playerId)
        {
            if (!TerritoryCards.ContainsKey(playerId))
                return false;
            
            var cards = TerritoryCards[playerId];
            if (cards.Count < 3)
                return false;
            
            // Agrupa cartas por nome e verifica se há pelo menos 3 iguais
            var grouped = cards.GroupBy(c => c);
            return grouped.Any(g => g.Count() >= 3);
        }
        
        /// <summary>
        /// Verifica se o jogador tem 3 cartas diferentes para trocar.
        /// </summary>
        public bool HasThreeDifferentCards(int playerId)
        {
            if (!TerritoryCards.ContainsKey(playerId))
                return false;
            
            var cards = TerritoryCards[playerId];
            if (cards.Count < 3)
                return false;
            
            // Verifica se há pelo menos 3 cartas diferentes
            return cards.Distinct().Count() >= 3;
        }
        
        /// <summary>
        /// Calcula quantos exércitos o jogador receberá pela próxima troca de cartas.
        /// Valores progressivos: 4, 6, 8, 10, 12, 14, 16, 18, 20...
        /// </summary>
        public int GetNextCardTradeArmies(int playerId)
        {
            var tradeCount = CardTradeCount.ContainsKey(playerId) ? CardTradeCount[playerId] : 0;
            // Valores progressivos: 4, 6, 8, 10, 12, 14, 16, 18, 20...
            return 4 + (tradeCount * 2);
        }
        
        /// <summary>
        /// Obtém lista de cartas que podem ser trocadas (3 iguais ou 3 diferentes).
        /// </summary>
        public List<string> GetTradeableCards(int playerId)
        {
            if (!TerritoryCards.ContainsKey(playerId))
                return new List<string>();
            
            var cards = TerritoryCards[playerId];
            if (cards.Count < 3)
                return new List<string>();
            
            var tradeable = new List<string>();
            
            // Verifica 3 cartas iguais
            var grouped = cards.GroupBy(c => c);
            foreach (var group in grouped)
            {
                if (group.Count() >= 3)
                {
                    tradeable.AddRange(group.Take(3));
                }
            }
            
            // Se não tem 3 iguais, verifica 3 diferentes
            if (tradeable.Count == 0 && cards.Distinct().Count() >= 3)
            {
                tradeable.AddRange(cards.Distinct().Take(3));
            }
            
            return tradeable;
        }
        
        // Métodos para Fase 2: Ataques
        
        /// <summary>
        /// Obtém territórios de origem que podem atacar (mínimo 2 exércitos, sendo 1 de ocupação).
        /// </summary>
        public List<string> GetAttackSourceTerritories(int playerId)
        {
            var playerTerritories = GetPlayerTerritories(playerId);
            var attackSources = new List<string>();
            
            foreach (var territoryName in playerTerritories)
            {
                if (!Territories.ContainsKey(territoryName))
                    continue;
                
                var territory = Territories[territoryName];
                // Precisa ter pelo menos 2 exércitos (1 de ocupação + 1 para atacar)
                if (territory.ArmyCount >= 2)
                {
                    attackSources.Add(territoryName);
                }
            }
            
            return attackSources;
        }
        
        /// <summary>
        /// Obtém territórios inimigos que podem ser atacados a partir de um território de origem.
        /// </summary>
        public List<string> GetAttackableTargetsFromSource(int playerId, string sourceTerritory)
        {
            if (!Territories.ContainsKey(sourceTerritory))
                return new List<string>();
            
            var source = Territories[sourceTerritory];
            if (source.OccupiedByPlayer != playerId)
                return new List<string>();
            
            var targets = new List<string>();
            
            foreach (var adjacentName in source.AdjacentTerritories)
            {
                if (!Territories.ContainsKey(adjacentName))
                    continue;
                
                var adjacent = Territories[adjacentName];
                // Território inimigo (ocupado por outro jogador)
                if (adjacent.OccupiedByPlayer.HasValue && adjacent.OccupiedByPlayer != playerId)
                {
                    targets.Add(adjacentName);
                }
            }
            
            return targets;
        }
        
        /// <summary>
        /// Calcula quantos dados o atacante pode rolar (máximo 3, limitado pelo número de exércitos - 1 de ocupação).
        /// </summary>
        public int GetAttackerDiceCount(int playerId, string sourceTerritory)
        {
            if (!Territories.ContainsKey(sourceTerritory))
                return 0;
            
            var territory = Territories[sourceTerritory];
            if (territory.OccupiedByPlayer != playerId)
                return 0;
            
            // Máximo 3 dados, limitado por (exércitos - 1 de ocupação)
            return Math.Min(3, Math.Max(0, territory.ArmyCount - 1));
        }
        
        /// <summary>
        /// Calcula quantos dados o defensor pode rolar (máximo 3, limitado pelo número de exércitos).
        /// </summary>
        public int GetDefenderDiceCount(string targetTerritory)
        {
            if (!Territories.ContainsKey(targetTerritory))
                return 0;
            
            var territory = Territories[targetTerritory];
            // Máximo 3 dados, limitado pelo número de exércitos
            return Math.Min(3, territory.ArmyCount);
        }
        
        /// <summary>
        /// Rola dados de combate (simulação - retorna valores aleatórios de 1 a 6).
        /// </summary>
        public List<int> RollDice(int count)
        {
            var random = new Random();
            var rolls = new List<int>();
            
            for (int i = 0; i < count; i++)
            {
                rolls.Add(random.Next(1, 7)); // Dado de 6 faces: 1-6
            }
            
            return rolls.OrderByDescending(r => r).ToList(); // Ordena do maior para o menor
        }
        
        /// <summary>
        /// Resolve um combate entre atacante e defensor.
        /// Retorna o resultado do combate com rolagens, comparações e perdas.
        /// </summary>
        public CombatResult ResolveCombat(int attackerId, string sourceTerritory, string targetTerritory)
        {
            var result = new CombatResult();
            
            // Calcula quantos dados cada lado pode rolar
            int attackerDice = GetAttackerDiceCount(attackerId, sourceTerritory);
            int defenderDice = GetDefenderDiceCount(targetTerritory);
            
            // Rola os dados
            result.AttackerRolls = RollDice(attackerDice);
            result.DefenderRolls = RollDice(defenderDice);
            
            // Compara os dados (maior com maior, segundo com segundo, etc.)
            int comparisons = Math.Min(result.AttackerRolls.Count, result.DefenderRolls.Count);
            
            for (int i = 0; i < comparisons; i++)
            {
                var comparison = new DiceComparison
                {
                    AttackerValue = result.AttackerRolls[i],
                    DefenderValue = result.DefenderRolls[i]
                };
                
                result.Comparisons.Add(comparison);
                
                // Empate = vitória do defensor
                if (comparison.AttackerWins)
                {
                    result.DefenderLosses++;
                }
                else
                {
                    result.AttackerLosses++;
                }
            }
            
            // Verifica se o território foi conquistado (todos os exércitos defensores foram destruídos)
            if (!Territories.ContainsKey(targetTerritory))
            {
                result.TerritoryConquered = false;
                return result;
            }
            
            var target = Territories[targetTerritory];
            int remainingDefenders = target.ArmyCount - result.DefenderLosses;
            result.TerritoryConquered = remainingDefenders <= 0;
            
            return result;
        }
        
        /// <summary>
        /// Aplica as perdas de um combate ao estado do jogo.
        /// </summary>
        public void ApplyCombatLosses(int attackerId, string sourceTerritory, string targetTerritory, CombatResult result)
        {
            // Remove exércitos do atacante
            if (Territories.ContainsKey(sourceTerritory))
            {
                var source = Territories[sourceTerritory];
                source.ArmyCount = Math.Max(1, source.ArmyCount - result.AttackerLosses); // Mínimo 1 (ocupação)
            }
            
            // Remove exércitos do defensor
            if (Territories.ContainsKey(targetTerritory))
            {
                var target = Territories[targetTerritory];
                target.ArmyCount = Math.Max(0, target.ArmyCount - result.DefenderLosses);
                
                // Se foi conquistado, transfere a propriedade
                if (result.TerritoryConquered)
                {
                    target.OccupiedByPlayer = attackerId;
                    target.ArmyCount = 0; // Será preenchido pelo movimento de exércitos
                }
            }
        }
        
        /// <summary>
        /// Move exércitos do território de origem para o território conquistado.
        /// Regra: mínimo 1, máximo 3, e nunca mais do que os exércitos que participaram do ataque.
        /// </summary>
        public void MoveArmiesAfterConquest(int attackerId, string sourceTerritory, string targetTerritory, int armiesToMove)
        {
            if (!Territories.ContainsKey(sourceTerritory) || !Territories.ContainsKey(targetTerritory))
                return;
            
            var source = Territories[sourceTerritory];
            var target = Territories[targetTerritory];
            
            if (source.OccupiedByPlayer != attackerId || target.OccupiedByPlayer != attackerId)
                return;
            
            // Calcula quantos exércitos participaram do ataque (exércitos - 1 de ocupação)
            int attackingArmies = source.ArmyCount - 1;
            
            // Limita o movimento: mínimo 1, máximo 3, e nunca mais do que os exércitos que participaram
            int actualMove = Math.Max(1, Math.Min(3, Math.Min(armiesToMove, attackingArmies)));
            
            // Garante que não move mais do que tem disponível (deixando pelo menos 1 de ocupação)
            actualMove = Math.Min(actualMove, source.ArmyCount - 1);
            
            // Move os exércitos
            source.ArmyCount -= actualMove;
            target.ArmyCount += actualMove;
        }
        
        // Métodos para Fase 3: Deslocamento de Exércitos
        
        /// <summary>
        /// Obtém territórios que podem deslocar exércitos (mais de 1 exército, com territórios contíguos do mesmo jogador).
        /// </summary>
        public List<string> GetMovementSourceTerritories(int playerId)
        {
            var playerTerritories = GetPlayerTerritories(playerId);
            var sources = new List<string>();
            
            foreach (var territoryName in playerTerritories)
            {
                if (!Territories.ContainsKey(territoryName))
                    continue;
                
                var territory = Territories[territoryName];
                // Precisa ter mais de 1 exército (1 de ocupação + pelo menos 1 para mover)
                if (territory.ArmyCount <= 1)
                    continue;
                
                // Verifica se tem pelo menos um território contíguo do mesmo jogador
                bool hasAdjacentFriendly = false;
                foreach (var adjacentName in territory.AdjacentTerritories)
                {
                    if (!Territories.ContainsKey(adjacentName))
                        continue;
                    
                    var adjacent = Territories[adjacentName];
                    if (adjacent.OccupiedByPlayer == playerId)
                    {
                        hasAdjacentFriendly = true;
                        break;
                    }
                }
                
                if (hasAdjacentFriendly)
                {
                    sources.Add(territoryName);
                }
            }
            
            return sources;
        }
        
        /// <summary>
        /// Obtém territórios de destino para deslocamento a partir de um território de origem.
        /// </summary>
        public List<string> GetMovementTargetTerritories(int playerId, string sourceTerritory)
        {
            if (!Territories.ContainsKey(sourceTerritory))
                return new List<string>();
            
            var source = Territories[sourceTerritory];
            if (source.OccupiedByPlayer != playerId)
                return new List<string>();
            
            var targets = new List<string>();
            
            foreach (var adjacentName in source.AdjacentTerritories)
            {
                if (!Territories.ContainsKey(adjacentName))
                    continue;
                
                var adjacent = Territories[adjacentName];
                // Território contíguo do mesmo jogador
                if (adjacent.OccupiedByPlayer == playerId)
                {
                    targets.Add(adjacentName);
                }
            }
            
            return targets;
        }
        
        /// <summary>
        /// Executa um deslocamento de exércitos entre dois territórios do mesmo jogador.
        /// Regra: mínimo 1 exército, deixando pelo menos 1 no território de origem.
        /// </summary>
        public void ExecuteMovement(int playerId, string sourceTerritory, string targetTerritory, int armiesToMove)
        {
            if (!Territories.ContainsKey(sourceTerritory) || !Territories.ContainsKey(targetTerritory))
                return;
            
            var source = Territories[sourceTerritory];
            var target = Territories[targetTerritory];
            
            if (source.OccupiedByPlayer != playerId || target.OccupiedByPlayer != playerId)
                return;
            
            // Verifica se são contíguos
            if (!source.AdjacentTerritories.Contains(targetTerritory))
                return;
            
            // Calcula quantos exércitos podem ser movidos (exércitos - 1 de ocupação)
            int availableArmies = source.ArmyCount - 1;
            
            // Limita o movimento: mínimo 1, máximo o disponível
            int actualMove = Math.Max(1, Math.Min(armiesToMove, availableArmies));
            
            // Move os exércitos
            source.ArmyCount -= actualMove;
            target.ArmyCount += actualMove;
        }
    }
}
