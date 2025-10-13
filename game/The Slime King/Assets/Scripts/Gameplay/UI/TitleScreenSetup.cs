using UnityEngine;

namespace SlimeKing.Gameplay.UI
{
    /// <summary>
    /// Utilitário para configuração inicial da cena da tela inicial.
    /// Garante que os managers necessários estejam presentes e configurados.
    /// </summary>
    public class TitleScreenSetup : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        #region Unity Lifecycle

        private void Awake()
        {
            SetupScene();
        }

        #endregion

        #region Setup

        /// <summary>
        /// Configura a cena garantindo que todos os managers necessários estejam presentes
        /// </summary>
        private void SetupScene()
        {
            // GameManager não está disponível ainda - implementar quando necessário
            Log("Title screen setup completed");
        }

        #endregion

        #region Debug

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        private void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[TitleScreenSetup] {message}");
            }
        }

        #endregion
    }
}