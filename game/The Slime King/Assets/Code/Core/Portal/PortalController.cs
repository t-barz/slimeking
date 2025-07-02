using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheSlimeKing.Core.Portal
{
    /// <summary>
    /// Versão simplificada do controlador de portal.
    /// Carrega uma nova cena quando o jogador entra em contato com o objeto.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PortalController : MonoBehaviour
    {
        [Header("Configuração do Portal")]
        [Tooltip("Nome da cena que será carregada ao tocar no portal")]
        [SerializeField] private string targetSceneName;

        [Header("Depuração")]
        [Tooltip("Mostra mensagens de depuração no console")]
        [SerializeField] private bool showDebugMessages = true;

        private void Awake()
        {
            // Garante que o Collider2D está configurado como trigger
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && !col.isTrigger)
            {
                col.isTrigger = true;
                if (showDebugMessages)
                    Debug.Log("PortalController: Collider2D foi configurado automaticamente como trigger.");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Verifica se o objeto que entrou no trigger é o jogador
            if (other.CompareTag("Player"))
            {
                if (showDebugMessages)
                    Debug.Log($"PortalController: Jogador detectado. Carregando cena '{targetSceneName}'.");

                // Verifica se o nome da cena de destino está configurado
                if (string.IsNullOrEmpty(targetSceneName))
                {
                    Debug.LogError("PortalController: Nome da cena de destino não está configurado!");
                    return;
                }

                // Verifica se a cena existe no build
                if (!IsSceneInBuild(targetSceneName))
                {
                    Debug.LogError($"PortalController: A cena '{targetSceneName}' não existe no build do jogo!");
                    return;
                }

                // Carrega a cena de destino
                SceneManager.LoadScene(targetSceneName);
            }
        }

        /// <summary>
        /// Verifica se uma cena existe no build do jogo
        /// </summary>
        private bool IsSceneInBuild(string sceneName)
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string name = System.IO.Path.GetFileNameWithoutExtension(path);
                if (name == sceneName)
                    return true;
            }
            return false;
        }
    }
}