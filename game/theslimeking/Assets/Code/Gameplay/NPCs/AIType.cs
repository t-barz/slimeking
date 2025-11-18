namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Tipos de IA de movimento para NPCs.
    /// </summary>
    public enum AIType
    {
        /// <summary>
        /// NPC estático - não se move
        /// </summary>
        Static,

        /// <summary>
        /// NPC vagueia aleatoriamente em uma área
        /// </summary>
        Wander,

        /// <summary>
        /// NPC patrulha entre pontos definidos
        /// </summary>
        Patrol
    }
}
