namespace SlimeKing.Gameplay.UI
{
    /// <summary>
    /// Estados possíveis da tela inicial durante a sequência de apresentação
    /// </summary>
    public enum TitleScreenState
    {
        /// <summary>
        /// Estado inicial - todos os elementos escondidos
        /// </summary>
        Initial,
        
        /// <summary>
        /// Logo da empresa visível
        /// </summary>
        CompanyLogo,
        
        /// <summary>
        /// Transição entre logo da empresa e elementos do jogo
        /// </summary>
        Transition,
        
        /// <summary>
        /// Fundo e logo do jogo visíveis
        /// </summary>
        GameTitle,
        
        /// <summary>
        /// Menu completo pronto para interação do usuário
        /// </summary>
        MenuReady
    }
}