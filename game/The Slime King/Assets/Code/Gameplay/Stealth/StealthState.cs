namespace TheSlimeKing.Gameplay.Stealth
{
    /// <summary>
    /// Define os possíveis estados de visibilidade do sistema de stealth
    /// </summary>
    public enum StealthState
    {
        /// <summary>
        /// Estado padrão: completamente visível e movimento normal
        /// </summary>
        Normal,

        /// <summary>
        /// Estado agachado: sprite agachado, mas ainda detectável
        /// </summary>
        Crouched,

        /// <summary>
        /// Estado escondido: não detectável por inimigos, vinheta escura
        /// </summary>
        Hidden,

        /// <summary>
        /// Estado exposto: agachado sem cobertura, detectável 
        /// </summary>
        Exposed
    }
}
