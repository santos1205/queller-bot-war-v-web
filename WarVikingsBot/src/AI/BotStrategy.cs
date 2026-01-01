using System;
using System.Collections.Generic;
using System.Linq;
using WarVikingsBot.Models;
using WarVikingsBot.State;

namespace WarVikingsBot.AI
{
    /// <summary>
    /// Sistema de decisão estratégica do bot.
    /// Analisa o estado do jogo e toma decisões baseadas no objetivo do bot.
    /// </summary>
    public class BotStrategy
    {
        private readonly DecisionContext _context;
        
        public BotStrategy(DecisionContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        /// <summary>
        /// Decide se o bot deve realizar um ataque neste momento.
        /// </summary>
        public bool ShouldAttack()
        {
            // Se não há territórios que podem atacar, não ataca
            if (!_context.AttackSourceTerritories.Any())
                return false;
            
            // Se não há alvos disponíveis, não ataca
            if (!_context.AttackableTerritories.Any())
                return false;
            
            // Estratégia baseada no objetivo
            switch (_context.Objective)
            {
                case BotObjective.ConquerTerritories:
                    return ShouldAttackForTerritoryConquest();
                    
                case BotObjective.ConquerRegion:
                    return ShouldAttackForRegionConquest();
                    
                case BotObjective.EliminatePlayer:
                    return ShouldAttackForElimination();
                    
                case BotObjective.ConquerPorts:
                    return ShouldAttackForPorts();
                    
                case BotObjective.ExpandAndFortify:
                default:
                    return ShouldAttackForExpansion();
            }
        }
        
        /// <summary>
        /// Seleciona o melhor território de origem para ataque.
        /// </summary>
        public string? SelectAttackSourceTerritory()
        {
            var availableSources = _context.AttackSourceTerritories;
            
            if (!availableSources.Any())
                return null;
            
            // Avalia cada território de origem e escolhe o melhor
            var scoredSources = availableSources
                .Select(territory => new
                {
                    Territory = territory,
                    Score = ScoreAttackSourceTerritory(territory)
                })
                .OrderByDescending(x => x.Score)
                .ToList();
            
            return scoredSources.First().Territory;
        }
        
        /// <summary>
        /// Seleciona o melhor território alvo para ataque a partir de um território de origem.
        /// </summary>
        public string? SelectAttackTargetTerritory(string sourceTerritory)
        {
            if (!_context.State.Territories.ContainsKey(sourceTerritory))
                return null;
            
            var source = _context.State.Territories[sourceTerritory];
            var availableTargets = source.AdjacentTerritories
                .Where(adjName => _context.State.Territories.ContainsKey(adjName))
                .Where(adjName => 
                {
                    var adj = _context.State.Territories[adjName];
                    return adj.OccupiedByPlayer.HasValue && 
                           adj.OccupiedByPlayer.Value != _context.PlayerId;
                })
                .ToList();
            
            if (!availableTargets.Any())
                return null;
            
            // Avalia cada alvo e escolhe o melhor
            var scoredTargets = availableTargets
                .Select(territory => new
                {
                    Territory = territory,
                    Score = ScoreAttackTargetTerritory(sourceTerritory, territory)
                })
                .OrderByDescending(x => x.Score)
                .ToList();
            
            return scoredTargets.First().Territory;
        }
        
        /// <summary>
        /// Decide quantos exércitos mover após conquista de território.
        /// </summary>
        public int DecideArmiesToMoveAfterConquest(string sourceTerritory, string conqueredTerritory)
        {
            if (!_context.State.Territories.ContainsKey(sourceTerritory) ||
                !_context.State.Territories.ContainsKey(conqueredTerritory))
                return 1; // Mínimo obrigatório
            
            var source = _context.State.Territories[sourceTerritory];
            var conquered = _context.State.Territories[conqueredTerritory];
            
            // Calcula quantos exércitos participaram do ataque
            int attackingArmies = Math.Min(source.ArmyCount - 1, 3); // Máximo 3 dados
            
            // Estratégia: mover o mínimo necessário se o território de origem está vulnerável
            // Mover mais se o território conquistado é valioso ou está exposto
            
            int armiesToMove = 1; // Mínimo
            
            // Se o território conquistado tem muitos adjacentes inimigos, mover mais
            int enemyAdjacents = conquered.AdjacentTerritories
                .Count(adjName => 
                    _context.State.Territories.ContainsKey(adjName) &&
                    _context.State.Territories[adjName].OccupiedByPlayer.HasValue &&
                    _context.State.Territories[adjName].OccupiedByPlayer.Value != _context.PlayerId);
            
            if (enemyAdjacents >= 2)
                armiesToMove = Math.Min(3, attackingArmies); // Máximo possível
            else if (enemyAdjacents == 1)
                armiesToMove = Math.Min(2, attackingArmies);
            
            // Se o território de origem ficaria muito fraco, mover menos
            int remainingInSource = source.ArmyCount - armiesToMove;
            if (remainingInSource < 2)
                armiesToMove = Math.Max(1, armiesToMove - 1);
            
            // Garante que não move mais do que participou do ataque
            armiesToMove = Math.Min(armiesToMove, attackingArmies);
            
            // Garante mínimo 1 e máximo 3
            return Math.Max(1, Math.Min(3, armiesToMove));
        }
        
        // ========== Métodos privados de avaliação ==========
        
        private bool ShouldAttackForTerritoryConquest()
        {
            // Objetivo: conquistar X territórios
            int targetCount = _context.ObjectiveParameters.ContainsKey("targetCount") 
                ? (int)_context.ObjectiveParameters["targetCount"] 
                : 18;
            
            int currentCount = _context.TerritoryCount;
            
            // Se está longe do objetivo, ataca agressivamente
            if (currentCount < targetCount * 0.7)
                return true;
            
            // Se está perto do objetivo, ataca apenas se tiver vantagem clara
            return EvaluateAttackAdvantage();
        }
        
        private bool ShouldAttackForRegionConquest()
        {
            // Objetivo: conquistar uma região específica
            string? targetRegion = _context.ObjectiveParameters.ContainsKey("targetRegion") 
                ? _context.ObjectiveParameters["targetRegion"].ToString() 
                : null;
            
            if (targetRegion == null)
                return ShouldAttackForExpansion();
            
            // Verifica se há alvos na região objetivo
            bool hasTargetsInRegion = _context.AttackableTerritories
                .Any(territory => 
                    _context.State.Territories.ContainsKey(territory) &&
                    _context.State.Territories[territory].Region == targetRegion);
            
            return hasTargetsInRegion && EvaluateAttackAdvantage();
        }
        
        private bool ShouldAttackForElimination()
        {
            // Objetivo: eliminar um jogador específico
            int? targetPlayer = _context.ObjectiveParameters.ContainsKey("targetPlayer") 
                ? (int?)_context.ObjectiveParameters["targetPlayer"] 
                : null;
            
            if (targetPlayer == null)
                return ShouldAttackForExpansion();
            
            // Verifica se há alvos do jogador alvo
            bool hasTargetPlayerTerritories = _context.AttackableTerritories
                .Any(territory => 
                    _context.State.Territories.ContainsKey(territory) &&
                    _context.State.Territories[territory].OccupiedByPlayer == targetPlayer);
            
            // Se há alvos do jogador alvo, ataca agressivamente
            if (hasTargetPlayerTerritories)
                return true;
            
            // Caso contrário, avalia vantagem geral
            return EvaluateAttackAdvantage();
        }
        
        private bool ShouldAttackForPorts()
        {
            // Objetivo: conquistar portos
            // Prioriza territórios com porto
            bool hasPortTargets = _context.AttackableTerritories
                .Any(territory => 
                    _context.State.Territories.ContainsKey(territory) &&
                    _context.State.Territories[territory].HasPort);
            
            if (hasPortTargets)
                return EvaluateAttackAdvantage();
            
            // Se não há portos disponíveis, usa estratégia de expansão
            return ShouldAttackForExpansion();
        }
        
        private bool ShouldAttackForExpansion()
        {
            // Estratégia genérica: expandir e fortalecer
            // Ataca se tiver vantagem clara ou se precisa expandir
            if (_context.TerritoryCount < 10)
                return true; // Expansão inicial agressiva
            
            return EvaluateAttackAdvantage();
        }
        
        private bool EvaluateAttackAdvantage()
        {
            // Avalia se há algum ataque vantajoso disponível
            foreach (var source in _context.AttackSourceTerritories)
            {
                var target = SelectAttackTargetTerritory(source);
                if (target == null)
                    continue;
                
                // Se há um ataque com vantagem numérica, ataca
                if (EvaluateAttackAdvantage(source, target))
                    return true;
            }
            
            return false;
        }
        
        private bool EvaluateAttackAdvantage(string source, string target)
        {
            if (!_context.State.Territories.ContainsKey(source) ||
                !_context.State.Territories.ContainsKey(target))
                return false;
            
            var sourceTerritory = _context.State.Territories[source];
            var targetTerritory = _context.State.Territories[target];
            
            // Calcula força de ataque (máximo 3 dados)
            int attackStrength = Math.Min(sourceTerritory.ArmyCount - 1, 3);
            
            // Calcula força de defesa (máximo 3 dados)
            int defenseStrength = Math.Min(targetTerritory.ArmyCount, 3);
            
            // Ataca se tiver vantagem numérica (mais dados)
            if (attackStrength > defenseStrength)
                return true;
            
            // Ataca se tiver igualdade e mais exércitos totais
            if (attackStrength == defenseStrength && 
                sourceTerritory.ArmyCount > targetTerritory.ArmyCount)
                return true;
            
            return false;
        }
        
        private int ScoreAttackSourceTerritory(string territory)
        {
            if (!_context.State.Territories.ContainsKey(territory))
                return 0;
            
            var t = _context.State.Territories[territory];
            int score = 0;
            
            // Mais exércitos = melhor (pode usar mais dados)
            score += t.ArmyCount * 10;
            
            // Comandante presente = bônus
            if (_context.State.HasCommanderInTerritory(_context.PlayerId, territory))
                score += 20;
            
            // Território com porto = bônus (estratégia naval futura)
            if (t.HasPort)
                score += 5;
            
            // Menos adjacentes inimigos = mais seguro
            int enemyAdjacents = t.AdjacentTerritories
                .Count(adjName => 
                    _context.State.Territories.ContainsKey(adjName) &&
                    _context.State.Territories[adjName].OccupiedByPlayer.HasValue &&
                    _context.State.Territories[adjName].OccupiedByPlayer.Value != _context.PlayerId);
            score -= enemyAdjacents * 5;
            
            return score;
        }
        
        private int ScoreAttackTargetTerritory(string source, string target)
        {
            if (!_context.State.Territories.ContainsKey(source) ||
                !_context.State.Territories.ContainsKey(target))
                return 0;
            
            var sourceTerritory = _context.State.Territories[source];
            var targetTerritory = _context.State.Territories[target];
            int score = 0;
            
            // Alvo mais fraco = melhor
            score += (10 - targetTerritory.ArmyCount) * 10;
            
            // Vantagem numérica = melhor
            int attackStrength = Math.Min(sourceTerritory.ArmyCount - 1, 3);
            int defenseStrength = Math.Min(targetTerritory.ArmyCount, 3);
            score += (attackStrength - defenseStrength) * 15;
            
            // Território com porto = bônus (se objetivo for conquistar portos)
            if (targetTerritory.HasPort && 
                _context.Objective == BotObjective.ConquerPorts)
                score += 50;
            
            // Região objetivo = bônus (se objetivo for conquistar região)
            if (_context.ObjectiveParameters.ContainsKey("targetRegion"))
            {
                string targetRegion = _context.ObjectiveParameters["targetRegion"].ToString() ?? "";
                if (targetTerritory.Region == targetRegion)
                    score += 30;
            }
            
            // Jogador alvo = bônus (se objetivo for eliminar jogador)
            if (_context.ObjectiveParameters.ContainsKey("targetPlayer"))
            {
                int targetPlayer = (int)_context.ObjectiveParameters["targetPlayer"];
                if (targetTerritory.OccupiedByPlayer == targetPlayer)
                    score += 40;
            }
            
            // Território isolado (poucos adjacentes) = mais fácil de defender após conquista
            int targetAdjacents = targetTerritory.AdjacentTerritories.Count;
            if (targetAdjacents <= 2)
                score += 10;
            
            return score;
        }
    }
}

