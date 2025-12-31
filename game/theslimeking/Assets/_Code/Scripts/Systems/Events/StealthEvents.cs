using System;

namespace SlimeKing.Core
{
    /// <summary>
    /// Classe estática centralizada para eventos do sistema de stealth.
    /// Permite comunicação desacoplada entre PlayerController, GameManager e sistemas de IA.
    /// </summary>
    public static class StealthEvents
    {
        #region Stealth State Events

        /// <summary>
        /// Disparado quando o jogador entra no estado stealth (semi-transparente).
        /// Quando disparar: Após PlayerController detectar cobertura e aplicar fade.
        /// </summary>
        public static event Action OnPlayerEnteredStealth;

        /// <summary>
        /// Disparado quando o jogador sai do estado stealth.
        /// Quando disparar: Após PlayerController perder cobertura ou parar de agachar.
        /// </summary>
        public static event Action OnPlayerExitedStealth;

        /// <summary>
        /// Disparado quando o estado de cobertura muda (com/sem cobertura).
        /// Quando disparar: Após PlayerController verificar Physics2D.OverlapCircle.
        /// Parâmetro: hasCover - se o jogador tem cobertura válida
        /// </summary>
        public static event Action<bool> OnPlayerCoverStateChanged;

        #endregion

        #region Helper Methods

        /// <summary>
        /// Dispara evento OnPlayerEnteredStealth com null-conditional operator.
        /// </summary>
        public static void PlayerEnteredStealth()
        {
            OnPlayerEnteredStealth?.Invoke();
        }

        /// <summary>
        /// Dispara evento OnPlayerExitedStealth com null-conditional operator.
        /// </summary>
        public static void PlayerExitedStealth()
        {
            OnPlayerExitedStealth?.Invoke();
        }

        /// <summary>
        /// Dispara evento OnPlayerCoverStateChanged com null-conditional operator.
        /// </summary>
        /// <param name="hasCover">Se o jogador tem cobertura válida</param>
        public static void PlayerCoverStateChanged(bool hasCover)
        {
            OnPlayerCoverStateChanged?.Invoke(hasCover);
        }

        #endregion
    }
}
