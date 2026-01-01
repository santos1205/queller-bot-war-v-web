using WarVikingsBot.State;

namespace WarVikingsBot.AI
{
    /// <summary>
    /// Contexto de decisão para o bot.
    /// Contém todas as informações necessárias para tomar uma decisão estratégica.
    /// </summary>
    public class DecisionContext
    {
        /// <summary>
        /// Estado atual do jogo
        /// </summary>
        public WarVikingsState State { get; set; } = null!;
        
        /// <summary>
        /// ID do jogador (bot) que está tomando a decisão
        /// </summary>
        public int PlayerId { get; set; }
        
        /// <summary>
        /// Objetivo do bot no jogo
        /// </summary>
        public BotObjective Objective { get; set; }
        
        /// <summary>
        /// Parâmetros específicos do objetivo (ex: número de territórios a conquistar)
        /// </summary>
        public Dictionary<string, object> ObjectiveParameters { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// Rodada atual do jogo
        /// </summary>
        public int CurrentRound => State.CurrentRound;
        
        /// <summary>
        /// Número de territórios do bot
        /// </summary>
        public int TerritoryCount => State.GetPlayerTerritoryCount(PlayerId);
        
        /// <summary>
        /// Número total de exércitos do bot
        /// </summary>
        public int TotalArmies => State.GetPlayerArmyCount(PlayerId);
        
        /// <summary>
        /// Número de exércitos no Valhalla
        /// </summary>
        public int ValhallaArmies => State.GetValhallaArmyCount(PlayerId);
        
        /// <summary>
        /// Número de cartas de território
        /// </summary>
        public int TerritoryCards => State.GetTerritoryCardCount(PlayerId);
        
        /// <summary>
        /// Lista de territórios do bot
        /// </summary>
        public List<string> PlayerTerritories => State.GetPlayerTerritories(PlayerId);
        
        /// <summary>
        /// Lista de territórios que podem ser atacados a partir dos territórios do bot
        /// </summary>
        public List<string> AttackableTerritories => State.GetAttackableTerritories(PlayerId);
        
        /// <summary>
        /// Lista de territórios que podem ser usados como origem de ataque
        /// </summary>
        public List<string> AttackSourceTerritories => State.GetAttackSourceTerritories(PlayerId);
    }
}

