using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PixeLadder.EasyTransition;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Componente que detecta colisão do Player e executa teletransporte entre cenas diferentes.
    /// 
    /// FLUXO DE TELETRANSPORTE:
    /// 1. Player entra no trigger → Desabilita movimento
    /// 2. Fade out (tela escurece)
    /// 3. Carrega nova cena usando LoadSceneAsync
    /// 4. Reposiciona player na nova cena
    /// 5. Fade in inicia → Reabilita movimento
    /// 6. Fade in completa
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class CrossSceneTeleportPoint : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Destination Configuration")]
        [Tooltip("Nome da cena de destino (deve estar nas Build Settings)")]
        [SerializeField] private string destinationSceneName = "";

        [Tooltip("Posição de destino na nova cena")]
        [SerializeField] private Vector3 destinationPosition;

        [Header("Transition Configuration")]
        [Tooltip("Efeito de transição a ser utilizado")]
        [SerializeField] private TransitionEffect transitionEffect;

        [Tooltip("Tempo de espera após reposicionamento antes do fade in (segundos)")]
        [SerializeField] private float delayBeforeFadeIn = 0.5f;

        [Header("Audio Configuration")]
        [Tooltip("Som reproduzido no início do teletransporte")]
        [SerializeField] private AudioClip teleportStartSound;

        [Tooltip("Som reproduzido durante a transição")]
        [SerializeField] private AudioClip teleportMidSound;

        [Tooltip("Som reproduzido ao completar o teletransporte")]
        [SerializeField] private AudioClip teleportEndSound;

        [Header("Trigger Configuration")]
        [Tooltip("Tamanho do BoxCollider2D trigger")]
        [SerializeField] private Vector2 triggerSize = new Vector2(1f, 1f);

        [Tooltip("Offset do trigger em relação à posição do GameObject")]
        [SerializeField] private Vector2 triggerOffset = Vector2.zero;

        [Header("Debug")]
        [Tooltip("Habilita logs de debug")]
        [SerializeField] private bool enableDebugLogs = false;

        [Tooltip("Habilita visualização de Gizmos no Editor")]
        [SerializeField] private bool enableGizmos = true;

        [Tooltip("Cor do Gizmo de visualização")]
        [SerializeField] private Color gizmoColor = Color.yellow;

        #endregion

        #region Private Fields

        private BoxCollider2D triggerCollider;
        private bool isTeleporting = false;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            triggerCollider = GetComponent<BoxCollider2D>();

            if (triggerCollider == null)
            {
                Debug.LogError($"[CrossSceneTeleportPoint] BoxCollider2D não encontrado em {gameObject.name}");
                enabled = false;
                return;
            }

            UpdateTriggerSize();
        }

        private void OnValidate()
        {
            UpdateTriggerSize();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (isTeleporting)
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[CrossSceneTeleportPoint] Teletransporte já em progresso, ignorando.");
                }
                return;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[CrossSceneTeleportPoint] Player entrou no trigger. Iniciando teletransporte para '{destinationSceneName}'");
            }

            StartCoroutine(ExecuteCrossSceneTeleport());
        }

        #endregion

        #region Private Methods

        private void UpdateTriggerSize()
        {
            if (triggerCollider == null)
                triggerCollider = GetComponent<BoxCollider2D>();

            if (triggerCollider != null)
            {
                triggerCollider.size = triggerSize;
                triggerCollider.offset = triggerOffset;
                triggerCollider.isTrigger = true;
            }
        }

        /// <summary>
        /// Executa o teletransporte entre cenas.
        /// Sequência: Desabilita movimento → Fade out → Carrega cena → Reposiciona → Fade in (reabilita movimento)
        /// </summary>
        private IEnumerator ExecuteCrossSceneTeleport()
        {
            isTeleporting = true;

            // Validações
            if (!ValidateTeleport())
            {
                isTeleporting = false;
                yield break;
            }

            // Encontra o Player na cena atual
            PlayerController currentPlayer = FindPlayerInScene();
            if (currentPlayer == null)
            {
                Debug.LogError("[CrossSceneTeleportPoint] Player não encontrado ao iniciar teletransporte!");
                isTeleporting = false;
                yield break;
            }

            // FASE 1: Desabilita movimento do player
            if (enableDebugLogs)
            {
                Debug.Log("[CrossSceneTeleportPoint] Fase 1 - Desabilitando movimento do player");
            }

            currentPlayer.DisableMovement();

            // Zera velocidade do Rigidbody2D
            Rigidbody2D playerRb = currentPlayer.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
            }

            // FASE 2: Reproduz som de início
            if (enableDebugLogs)
            {
                Debug.Log("[CrossSceneTeleportPoint] Fase 2 - Reproduzindo som de início");
            }

            PlaySound(teleportStartSound);

            // FASE 3: Transfere execução para o SceneTransitioner que persiste entre cenas
            // IMPORTANTE: Este CrossSceneTeleportPoint será destruído quando a cena carregar,
            // então precisamos executar o resto no SceneTransitioner
            if (enableDebugLogs)
            {
                Debug.Log("[CrossSceneTeleportPoint] Fase 3 - Transferindo execução para SceneTransitioner");
            }

            SceneTransitioner.Instance.StartCoroutine(ExecuteTransitionSequence());

            isTeleporting = false;
        }

        /// <summary>
        /// Executa a sequência completa de transição no SceneTransitioner (que persiste entre cenas).
        /// Esta coroutine sobrevive ao carregamento da cena porque roda no SceneTransitioner.
        /// </summary>
        private IEnumerator ExecuteTransitionSequence()
        {
            // FASE 1: Fade out
            if (enableDebugLogs)
            {
                Debug.Log("[CrossSceneTeleportPoint] Executando fade out");
            }

            yield return ExecuteFadeOut();

            // FASE 2: Som durante transição
            PlaySound(teleportMidSound);

            // FASE 3: Carrega cena e reposiciona
            if (enableDebugLogs)
            {
                Debug.Log("[CrossSceneTeleportPoint] Carregando nova cena");
            }

            yield return LoadAndRepositionPlayer();

            // FASE 4: Fade in
            if (enableDebugLogs)
            {
                Debug.Log("[CrossSceneTeleportPoint] Executando fade in");
            }

            yield return ExecuteFadeIn();

            // FASE 5: Som de chegada
            PlaySound(teleportEndSound);

            if (enableDebugLogs)
            {
                Debug.Log($"[CrossSceneTeleportPoint] Teletransporte para '{destinationSceneName}' concluído!");
            }
        }

        /// <summary>
        /// Executa apenas o fade out (escurece a tela).
        /// </summary>
        private IEnumerator ExecuteFadeOut()
        {
            // Acessa a imagem de transição através de reflexão
            var transitionImageField = typeof(SceneTransitioner).GetField(
                "transitionImageInstance",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

            if (transitionImageField == null)
            {
                Debug.LogError("[CrossSceneTeleportPoint] Não foi possível acessar transitionImageInstance!");
                yield break;
            }

            UnityEngine.UI.Image transitionImage = transitionImageField.GetValue(SceneTransitioner.Instance) as UnityEngine.UI.Image;

            if (transitionImage == null)
            {
                Debug.LogError("[CrossSceneTeleportPoint] transitionImageInstance é nulo!");
                yield break;
            }

            // Prepara o material e ativa a imagem
            transitionImage.gameObject.SetActive(true);
            Material materialInstance = new Material(transitionEffect.transitionMaterial);
            transitionImage.material = materialInstance;
            transitionEffect.SetEffectProperties(materialInstance);

            if (enableDebugLogs)
                Debug.Log("[CrossSceneTeleportPoint] Executando fade out...");

            // Executa animação de fade out
            yield return transitionEffect.AnimateOut(transitionImage);

            if (enableDebugLogs)
                Debug.Log("[CrossSceneTeleportPoint] Fade out completo");
        }

        /// <summary>
        /// Executa apenas o fade in (clareia a tela).
        /// </summary>
        private IEnumerator ExecuteFadeIn()
        {
            // Aguarda delay configurável antes do fade in
            if (delayBeforeFadeIn > 0)
            {
                if (enableDebugLogs)
                    Debug.Log($"[CrossSceneTeleportPoint] Aguardando {delayBeforeFadeIn}s antes do fade in...");

                yield return new WaitForSeconds(delayBeforeFadeIn);
            }

            // Acessa a imagem de transição através de reflexão
            var transitionImageField = typeof(SceneTransitioner).GetField(
                "transitionImageInstance",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

            if (transitionImageField == null)
            {
                Debug.LogError("[CrossSceneTeleportPoint] Não foi possível acessar transitionImageInstance!");
                yield break;
            }

            UnityEngine.UI.Image transitionImage = transitionImageField.GetValue(SceneTransitioner.Instance) as UnityEngine.UI.Image;

            if (transitionImage == null)
            {
                Debug.LogError("[CrossSceneTeleportPoint] transitionImageInstance é nulo!");
                yield break;
            }

            if (enableDebugLogs)
                Debug.Log("[CrossSceneTeleportPoint] Executando fade in...");

            // Executa animação de fade in
            yield return transitionEffect.AnimateIn(transitionImage);

            if (enableDebugLogs)
                Debug.Log("[CrossSceneTeleportPoint] Fade in completo");

            // Cleanup: desativa a imagem e limpa o material
            Material materialInstance = transitionImage.material;
            transitionImage.gameObject.SetActive(false);

            if (materialInstance != null)
            {
                UnityEngine.Object.DestroyImmediate(materialInstance);
            }
        }

        /// <summary>
        /// Carrega a nova cena e reposiciona o player.
        /// Executado durante o mid-transition (tela escura).
        /// </summary>
        private IEnumerator LoadAndRepositionPlayer()
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[CrossSceneTeleportPoint] Carregando cena '{destinationSceneName}'...");
            }

            // Carrega nova cena usando LoadSceneAsync (modo Single destrói a cena anterior)
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(destinationSceneName, LoadSceneMode.Single);

            if (loadOperation == null)
            {
                Debug.LogError($"[CrossSceneTeleportPoint] Falha ao iniciar carregamento de '{destinationSceneName}'!");
                yield break;
            }

            // Aguarda carregamento completo
            while (!loadOperation.isDone)
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[CrossSceneTeleportPoint] Progresso: {loadOperation.progress * 100f:F1}%");
                }
                yield return null;
            }

            Debug.Log($"[CrossSceneTeleportPoint] Cena '{SceneManager.GetActiveScene().name}' carregada");

            // Aguarda frames para garantir que todos os Awake/Start sejam executados
            yield return null;
            yield return null;
            yield return new WaitForEndOfFrame();

            // IMPORTANTE: Após LoadSceneMode.Single, o Player antigo foi destruído
            // Precisamos encontrar o Player que existe na nova cena
            PlayerController newPlayer = FindPlayerInScene();

            if (newPlayer == null)
            {
                Debug.LogError("[CrossSceneTeleportPoint] Player não encontrado na nova cena!");
                yield break;
            }

            Debug.Log($"[CrossSceneTeleportPoint] Player encontrado na nova cena: {newPlayer.name}");
            Debug.Log($"[CrossSceneTeleportPoint] Posição atual do Player: {newPlayer.transform.position}");
            Debug.Log($"[CrossSceneTeleportPoint] Posição destino: {destinationPosition}");

            // Reposiciona player
            RepositionPlayer(newPlayer, destinationPosition);

            Debug.Log($"[CrossSceneTeleportPoint] Posição após reposicionamento: {newPlayer.transform.position}");

            // Reabilita movimento ANTES do fade in começar
            if (enableDebugLogs)
            {
                Debug.Log("[CrossSceneTeleportPoint] Reabilitando movimento do player");
            }

            newPlayer.EnableMovement();
        }

        /// <summary>
        /// Valida se o teletransporte pode ser executado.
        /// </summary>
        private bool ValidateTeleport()
        {
            // Valida nome da cena
            if (string.IsNullOrEmpty(destinationSceneName))
            {
                Debug.LogError("[CrossSceneTeleportPoint] Nome da cena de destino está vazio!");
                return false;
            }

            // Valida se a cena existe nas Build Settings
            if (!IsSceneInBuildSettings(destinationSceneName))
            {
                Debug.LogError($"[CrossSceneTeleportPoint] Cena '{destinationSceneName}' não encontrada nas Build Settings!");
                return false;
            }

            // Valida TransitionEffect
            if (transitionEffect == null)
            {
                Debug.LogError("[CrossSceneTeleportPoint] TransitionEffect não está atribuído!");
                return false;
            }

            // Valida PlayerController (busca na cena atual)
            PlayerController currentPlayer = FindPlayerInScene();
            if (currentPlayer == null)
            {
                Debug.LogError("[CrossSceneTeleportPoint] Player não encontrado na cena atual!");
                return false;
            }

            // Valida SceneTransitioner
            if (SceneTransitioner.Instance == null)
            {
                Debug.LogError("[CrossSceneTeleportPoint] SceneTransitioner.Instance não encontrado!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifica se uma cena existe nas Build Settings.
        /// </summary>
        private bool IsSceneInBuildSettings(string sceneName)
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                if (sceneNameFromPath == sceneName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Encontra o Player na cena atual.
        /// Usa múltiplas estratégias para garantir que o Player seja encontrado.
        /// </summary>
        private PlayerController FindPlayerInScene()
        {
            // Estratégia 1: Tenta via PlayerController.Instance (pode estar atualizado após Awake)
            if (PlayerController.Instance != null)
            {
                Debug.Log("[CrossSceneTeleportPoint] Player encontrado via Instance");
                return PlayerController.Instance;
            }

            // Estratégia 2: Busca por tag "Player"
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                PlayerController controller = playerObject.GetComponent<PlayerController>();
                if (controller != null)
                {
                    Debug.Log("[CrossSceneTeleportPoint] Player encontrado via tag");
                    return controller;
                }
            }

            // Estratégia 3: Busca por nome conhecido "chr_whiteslime"
            playerObject = GameObject.Find("chr_whiteslime");
            if (playerObject != null)
            {
                PlayerController controller = playerObject.GetComponent<PlayerController>();
                if (controller != null)
                {
                    Debug.Log("[CrossSceneTeleportPoint] Player encontrado via nome");
                    return controller;
                }
            }

            // Estratégia 4: Busca por tipo (último recurso)
            PlayerController foundController = FindFirstObjectByType<PlayerController>();
            if (foundController != null)
            {
                Debug.Log("[CrossSceneTeleportPoint] Player encontrado via FindFirstObjectByType");
                return foundController;
            }

            return null;
        }

        /// <summary>
        /// Reposiciona o player na nova cena e ajusta a câmera.
        /// </summary>
        private void RepositionPlayer(PlayerController player, Vector3 position)
        {
            if (player == null)
            {
                Debug.LogError("[CrossSceneTeleportPoint] PlayerController é null ao tentar reposicionar!");
                return;
            }

            Debug.Log($"[CrossSceneTeleportPoint] RepositionPlayer - Posição atual: {player.transform.position}");
            Debug.Log($"[CrossSceneTeleportPoint] RepositionPlayer - Posição destino: {position}");

            // Reposiciona o player
            player.transform.position = position;

            // Zera velocidade do Rigidbody2D para evitar movimento residual
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
            }

            Debug.Log($"[CrossSceneTeleportPoint] RepositionPlayer - Posição APÓS atribuição: {player.transform.position}");

            // Força a câmera a seguir o player imediatamente
            if (Camera.main != null)
            {
                Vector3 cameraPosition = new Vector3(
                    position.x,
                    position.y,
                    Camera.main.transform.position.z
                );
                
                Debug.Log($"[CrossSceneTeleportPoint] RepositionPlayer - Posição da câmera ANTES: {Camera.main.transform.position}");
                Camera.main.transform.position = cameraPosition;
                Debug.Log($"[CrossSceneTeleportPoint] RepositionPlayer - Posição da câmera DEPOIS: {Camera.main.transform.position}");
            }
            else
            {
                Debug.LogWarning("[CrossSceneTeleportPoint] Camera.main é null!");
            }

            // Reconfigura Cinemachine se disponível
            if (SlimeKing.Core.CameraManager.HasInstance)
            {
                Debug.Log("[CrossSceneTeleportPoint] CameraManager encontrado, reconfigurando Cinemachine...");
                SlimeKing.Core.CameraManager.Instance.ForceCinemachineSetup();
                Debug.Log("[CrossSceneTeleportPoint] Cinemachine reconfigurada");
            }
            else
            {
                Debug.LogWarning("[CrossSceneTeleportPoint] CameraManager não encontrado!");
            }

            // Log final de verificação
            Debug.Log($"[CrossSceneTeleportPoint] RepositionPlayer FINAL - Posição do Player: {player.transform.position}");
        }

        /// <summary>
        /// Reproduz um som se o AudioClip estiver configurado.
        /// </summary>
        private void PlaySound(AudioClip clip)
        {
            if (clip == null) return;

            AudioSource.PlayClipAtPoint(clip, Camera.main != null ? Camera.main.transform.position : Vector3.zero, 1f);

            if (enableDebugLogs)
            {
                Debug.Log($"[CrossSceneTeleportPoint] Som reproduzido: {clip.name}");
            }
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            if (!enableGizmos)
                return;

            Gizmos.color = gizmoColor;

            if (triggerCollider != null)
            {
                Vector3 center = transform.position + (Vector3)triggerCollider.offset;
                Vector3 size = triggerCollider.size;
                DrawWireCube(center, size);
            }
            else
            {
                Vector3 center = transform.position + (Vector3)triggerOffset;
                Vector3 size = triggerSize;
                DrawWireCube(center, size);
            }

            if (!string.IsNullOrEmpty(destinationSceneName))
            {
                Gizmos.color = gizmoColor;
                
                // Desenha marcador de destino (não podemos desenhar em outra cena, então apenas indicamos)
                Vector3 labelPosition = transform.position + Vector3.up * 1.5f;

#if UNITY_EDITOR
                string labelText = $"Cross-Scene Teleport\n→ {destinationSceneName}\n{destinationPosition}";
                UnityEditor.Handles.Label(
                    labelPosition,
                    labelText,
                    new UnityEngine.GUIStyle()
                    {
                        normal = new UnityEngine.GUIStyleState() { textColor = gizmoColor },
                        alignment = UnityEngine.TextAnchor.MiddleCenter,
                        fontSize = 10,
                        fontStyle = UnityEngine.FontStyle.Bold
                    }
                );
#endif
            }
        }

        private void DrawWireCube(Vector3 center, Vector3 size)
        {
            Vector3 halfSize = size * 0.5f;

            Vector3[] vertices = new Vector3[8];
            vertices[0] = center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
            vertices[1] = center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
            vertices[2] = center + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
            vertices[3] = center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
            vertices[4] = center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
            vertices[5] = center + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            vertices[6] = center + new Vector3(halfSize.x, halfSize.y, halfSize.z);
            vertices[7] = center + new Vector3(-halfSize.x, halfSize.y, halfSize.z);

            Gizmos.DrawLine(vertices[0], vertices[1]);
            Gizmos.DrawLine(vertices[1], vertices[2]);
            Gizmos.DrawLine(vertices[2], vertices[3]);
            Gizmos.DrawLine(vertices[3], vertices[0]);

            Gizmos.DrawLine(vertices[4], vertices[5]);
            Gizmos.DrawLine(vertices[5], vertices[6]);
            Gizmos.DrawLine(vertices[6], vertices[7]);
            Gizmos.DrawLine(vertices[7], vertices[4]);

            Gizmos.DrawLine(vertices[0], vertices[4]);
            Gizmos.DrawLine(vertices[1], vertices[5]);
            Gizmos.DrawLine(vertices[2], vertices[6]);
            Gizmos.DrawLine(vertices[3], vertices[7]);
        }

        #endregion
    }
}
