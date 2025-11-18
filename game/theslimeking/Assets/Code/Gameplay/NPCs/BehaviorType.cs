namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Tipos de comportamento social de NPCs.
    /// </summary>
    public enum BehaviorType
    {
        /// <summary>
        /// NPC passivo - não reage ao jogador
        /// </summary>
        Passivo,

        /// <summary>
        /// NPC neutro - observa o jogador mas não ataca
        /// </summary>
        Neutro,

        /// <summary>
        /// NPC agressivo - ataca o jogador quando detectado
        /// </summary>
        Agressivo,

        /// <summary>
        /// NPC que oferece quests
        /// </summary>
        QuestGiver
    }
}
