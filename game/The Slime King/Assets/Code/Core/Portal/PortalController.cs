using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

        [Header("Efeito de Vinheta")]
        [Tooltip("Se marcado, ativa o efeito de vinheta antes de carregar a próxima cena")]
        [SerializeField] private bool useVignetteEffect = true;

        [Tooltip("Referência ao controlador de vinheta. Se null, tentará encontrar no nível da cena")]
        [SerializeField] private VignetteController vignetteController;

        [Tooltip("Duração do efeito de vinheta antes de carregar a próxima cena (em segundos)")]
        [SerializeField] private float vignetteTransitionDuration = 1f;

        [Tooltip("Se verdadeiro, aplica o efeito de vinheta instantaneamente sem animação")]
        [SerializeField] private bool immediateVignetteEffect = false;

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

            // Pré-valida a referência do controlador de vinheta se o efeito for utilizado
            if (useVignetteEffect && vignetteController == null)
            {
                // Não busca automaticamente no Awake para evitar overhead desnecessário
                // Isso será feito apenas quando necessário no momento da transição
                if (showDebugMessages)
                    Debug.Log("PortalController: VignetteController não está referenciado. Será buscado automaticamente quando necessário.");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Verifica se o objeto que entrou no trigger é o jogador
            if (other.CompareTag("Player"))
            {
                if (showDebugMessages)
                    Debug.Log($"PortalController: Jogador detectado. Iniciando transição para a cena '{targetSceneName}'.");

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

                if (useVignetteEffect)
                {
                    // Tenta iniciar o efeito de vinheta antes de carregar a cena
                    StartCoroutine(StartVignetteTransitionAndLoadScene());
                }
                else
                {
                    // Carrega a cena diretamente se não for usar o efeito de vinheta
                    if (showDebugMessages)
                        Debug.Log($"PortalController: Carregando cena '{targetSceneName}' sem efeito de vinheta.");

                    SceneManager.LoadScene(targetSceneName);
                }
            }
        }

        /// <summary>
        /// Inicia o efeito de vinheta e carrega a próxima cena após a transição
        /// </summary>
        private IEnumerator StartVignetteTransitionAndLoadScene()
        {
            // Tenta encontrar o controlador de vinheta se não estiver atribuído
            if (vignetteController == null)
            {
                vignetteController = FindAnyObjectByType<VignetteController>();

                if (vignetteController == null)
                {
                    Debug.LogWarning("PortalController: VignetteController não encontrado. Carregando cena sem efeito de vinheta.");
                    SceneManager.LoadScene(targetSceneName);
                    yield break;
                }
            }

            if (showDebugMessages)
                Debug.Log($"PortalController: Iniciando efeito de vinheta antes de carregar a cena '{targetSceneName}'.");

            // Inicia a transição de vinheta
            vignetteController.TransitionToTarget(immediateVignetteEffect, true);

            // Espera o tempo configurado para a duração do efeito
            yield return new WaitForSeconds(vignetteTransitionDuration);

            // Carrega a cena de destino após o efeito de vinheta
            if (showDebugMessages)
                Debug.Log($"PortalController: Carregando cena '{targetSceneName}' após efeito de vinheta.");

            SceneManager.LoadScene(targetSceneName);
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