namespace TheSlimeKing.Core.Combat
{
    /// <summary>
    /// Define os tipos de ataque disponíveis para o jogador
    /// </summary>
    public enum AttackType
    {
        /// <summary>
        /// Ataque básico - rápido e curto alcance
        /// </summary>
        Basic,

        /// <summary>
        /// Dash Attack - movimento com dano
        /// </summary>
        Dash,

        /// <summary>
        /// Ataque especial - baseado em energia elemental
        /// </summary>
        Special,

        /// <summary>
        /// Ataque carregado - mais forte que o básico
        /// </summary>
        Charged
    }
}
