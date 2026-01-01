namespace WarVikingsBot.AI
{
    /// <summary>
    /// Representa os possíveis objetivos do bot no jogo.
    /// Baseado nas cartas-objetivo do jogo War Vikings.
    /// </summary>
    public enum BotObjective
    {
        /// <summary>
        /// Conquistar um número específico de territórios (ex: 18 territórios)
        /// </summary>
        ConquerTerritories,
        
        /// <summary>
        /// Conquistar territórios de uma região específica
        /// </summary>
        ConquerRegion,
        
        /// <summary>
        /// Eliminar um jogador específico
        /// </summary>
        EliminatePlayer,
        
        /// <summary>
        /// Conquistar um número específico de portos
        /// </summary>
        ConquerPorts,
        
        /// <summary>
        /// Objetivo genérico: expandir e fortalecer posição
        /// </summary>
        ExpandAndFortify
    }
}

