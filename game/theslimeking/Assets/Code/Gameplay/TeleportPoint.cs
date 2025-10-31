namespace PixeLadder.EasyTransition
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Componente que detecta colisão do Player e executa teletransporte com transição visual.
    /// Utiliza o Easy Transition para criar uma experiência fluida de teletransporte na mesma cena.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class TeleportPoint : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Teleport Configuration")]
        [Tooltip("Posição de destino do teletransporte")]
        [SerializeField] private Vector3 destinationPosition;

        [Tooltip("Efeito de transição a ser utilizado (CircleEffect recomendado)")]
        [SerializeField] private TransitionEffect transitionEffect;

        [Tooltip("Tempo de espera após reposicionamento antes do fade in (segundos)")]
        [SerializeField] private float delayBeforeFadeIn = 1f;

        [Header("Cross-Scene Configuration")]
        [Tooltip("Habilita teletransporte entre cenas diferentes")]
        [SerializeField] private bool isCrossSceneTeleport = false;

        [Tooltip("Nome da cena de destino (deve estar nas Build Settings)")]
        [SerializeField] private string destinationSceneName = "";

        [Header("Audio Configuration")]
        [Tooltip("Som reproduzido no início do teletransporte")]
        [SerializeField] private AudioClip teleportStartSound;

        [Tooltip("Som reproduzido durante a transição (whoosh/portal)")]
        [SerializeField] private AudioClip teleportMidSound;

        [Tooltip("Som reproduzido ao completar o teletransporte")]
        [SerializeField] private AudioClip teleportEndSound;

        [Header("Trigger Configuration")]
        [Tooltip("Tamanho do BoxCollider2D trigger")]
        [SerializeField] private Vector2 triggerSize = new Vector2(1f, 1f);

        [Tooltip("Offset do trigger em relação à posição do GameObject")]
        [SerializeField] private Vector2 triggerOffset = Vector2.zero;

        [Header("Debug")]
        [Tooltip("Habilita logs de debug para este ponto de teletransporte")]
        [SerializeField] private bool enableDebugLogs = false;

        [Tooltip("Habilita visualização de Gizmos no Editor")]
        [SerializeField] private bool enableGizmos = true;

        [Tooltip("Cor do Gizmo de visualização")]
        [SerializeField] private Color gizmoColor = Color.cyan;

        #endregion

        #region Private Fields

        private BoxCollider2D triggerCollider;
        private bool isTeleporting = false;
        private Transform cameraTransform;
        private Rigidbody2D playerRigidbody;

        #endregion

        #region Unity Lifecycle

        /// <summary>
        /// Inicializa componentes e valida configuração.
        /// </summary>
        private void Awake()
        {
            // Cache do BoxCollider2D
            triggerCollider = GetComponent<BoxCollider2D>();

            if (triggerCollider == null)
            {
                Debug.LogError($"TeleportPoint: BoxCollider2D não encontrado em '{gameObject.name}'!", this);
                enabled = false;
                return;
            }

            // Aplica configurações do trigger
            UpdateTriggerSize();

            if (enableDebugLogs)
                Debug.Log($"TeleportPoint '{gameObject.name}' inicializado com sucesso.", this);
        }

        /// <summary>
        /// Atualiza configurações do trigger quando valores são modificados no Inspector.
        /// </summary>
        private void OnValidate()
        {
            UpdateTriggerSize();
        }

        /// <summary>
        /// Detecta quando o Player entra no trigger e inicia o teletransporte imediatamente.
        /// </summary>
        /// <param name="other">Collider que entrou no trigger</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Valida se é o Player usando CompareTag (melhor performance)
            if (!other.CompareTag("Player"))
            {
                if (enableDebugLogs)
                    Debug.Log($"TeleportPoint: Objeto '{other.name}' não é Player, ignorando.", this);
                return;
            }

            // Previne múltiplos teletransportes simultâneos
            if (isTeleporting)
            {
                if (enableDebugLogs)
                    Debug.Log("TeleportPoint: Já está teletransportando, ignorando trigger.", this);
                return;
            }

            if (enableDebugLogs)
                Debug.Log($"TeleportPoint: Player detectado, iniciando teletransporte.", this);

            // Inicia o processo de teletransporte imediatamente
            StartCoroutine(ExecuteTeleport());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determina se este é um teletransporte entre cenas ou na mesma cena.
        /// </summary>
        /// <returns>True se é teletransporte entre cenas, false se é na mesma cena</returns>
        private bool IsCrossSceneTeleport()
        {
            return isCrossSceneTeleport && !string.IsNullOrEmpty(destinationSceneName);
        }

        /// <summary>
        /// Aplica as configurações de tamanho e offset ao BoxCollider2D.
        /// </summary>
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
        /// Executa o processo completo de teletransporte com transição visual.
        /// Roteia para same-scene ou cross-scene baseado na configuração.
        /// </summary>
        private IEnumerator ExecuteTeleport()
        {
            // Define flag para prevenir múltiplos teletransportes
            isTeleporting = true;

            // Valida se o teletransporte pode ser executado
            if (!ValidateTeleport())
            {
                isTeleporting = false;
                yield break;
            }

            // Determina o tipo de teletransporte e roteia adequadamente
            if (IsCrossSceneTeleport())
            {
                // === CROSS-SCENE TELEPORT PATH ===
                if (enableDebugLogs)
                    Debug.Log($"TeleportPoint: Iniciando teletransporte cross-scene para '{destinationSceneName}' na posição {destinationPosition}", this);

                // Valida se TeleportManager existe
                if (TeleportManager.Instance == null)
                {
                    Debug.LogError("TeleportPoint: TeleportManager.Instance não encontrado. " +
                                  "Adicione o TeleportManager à cena para usar teletransporte cross-scene.", this);
                    isTeleporting = false;
                    yield break;
                }

                // Delega para TeleportManager que orquestra todo o processo cross-scene
                TeleportManager.Instance.ExecuteCrossSceneTeleport(
                    destinationSceneName: destinationSceneName,
                    destinationPosition: destinationPosition,
                    transitionEffect: transitionEffect,
                    delayBeforeFadeIn: delayBeforeFadeIn,
                    startSound: teleportStartSound,
                    midSound: teleportMidSound,
                    endSound: teleportEndSound,
                    enableDebugLogs: enableDebugLogs
                );

                // Libera flag de teletransporte (TeleportManager gerencia seu próprio lock)
                isTeleporting = false;

                if (enableDebugLogs)
                    Debug.Log("TeleportPoint: Teletransporte cross-scene delegado ao TeleportManager.", this);
            }
            else
            {
                // === SAME-SCENE TELEPORT PATH (existing logic unchanged) ===
                if (enableDebugLogs)
                    Debug.Log($"TeleportPoint: Iniciando teletransporte same-scene para {destinationPosition}", this);

                // Cache do Rigidbody2D do Player
                if (playerRigidbody == null)
                {
                    playerRigidbody = PlayerController.Instance.GetComponent<Rigidbody2D>();
                }

                // Desabilita movimento do Player
                PlayerController.Instance.DisableMovement();

                // Zera a velocidade do Rigidbody2D para parar o movimento imediatamente
                if (playerRigidbody != null)
                {
                    playerRigidbody.linearVelocity = Vector2.zero;

                    if (enableDebugLogs)
                        Debug.Log("TeleportPoint: Velocidade do Player zerada.", this);
                }

                // Executa transição visual com callback de reposicionamento
                yield return TeleportTransitionHelper.ExecuteTransition(
                    transitionEffect,
                    RepositionPlayerAndCamera,
                    delayBeforeFadeIn,
                    enableDebugLogs
                );

                // Reabilita movimento do Player
                PlayerController.Instance.EnableMovement();

                // Libera flag de teletransporte
                isTeleporting = false;

                if (enableDebugLogs)
                    Debug.Log("TeleportPoint: Teletransporte same-scene completo!", this);
            }
        }

        /// <summary>
        /// Reposiciona o Player e a câmera para a posição de destino.
        /// Mantém o offset da câmera em relação ao Player.
        /// </summary>
        private void RepositionPlayerAndCamera()
        {
            if (PlayerController.Instance == null)
            {
                Debug.LogError("TeleportPoint: PlayerController.Instance não encontrado durante reposicionamento!", this);
                return;
            }

            // Cache da câmera principal se ainda não foi feito
            if (cameraTransform == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    cameraTransform = mainCamera.transform;
                }
                else
                {
                    Debug.LogWarning("TeleportPoint: Câmera principal não encontrada!", this);
                }
            }

            // Calcula offset da câmera antes do reposicionamento
            Vector3 cameraOffset = Vector3.zero;
            if (cameraTransform != null)
            {
                cameraOffset = cameraTransform.position - PlayerController.Instance.transform.position;

                if (enableDebugLogs)
                    Debug.Log($"TeleportPoint: Offset da câmera calculado: {cameraOffset}", this);
            }

            // Reposiciona o Player
            Vector3 oldPosition = PlayerController.Instance.transform.position;
            PlayerController.Instance.transform.position = destinationPosition;

            if (enableDebugLogs)
                Debug.Log($"TeleportPoint: Player reposicionado de {oldPosition} para {destinationPosition}", this);

            // Reposiciona a câmera mantendo o offset
            if (cameraTransform != null)
            {
                Vector3 newCameraPosition = destinationPosition + cameraOffset;
                cameraTransform.position = newCameraPosition;

                if (enableDebugLogs)
                    Debug.Log($"TeleportPoint: Câmera reposicionada para {newCameraPosition}", this);
            }
        }

        /// <summary>
        /// Valida se o teletransporte pode ser executado.
        /// Verifica todas as dependências necessárias.
        /// </summary>
        /// <returns>True se todas as validações passarem, false caso contrário</returns>
        private bool ValidateTeleport()
        {
            // Valida se transitionEffect está atribuído
            if (transitionEffect == null)
            {
                Debug.LogWarning($"TeleportPoint '{gameObject.name}': Efeito de transição não atribuído! Atribua um TransitionEffect (ex: CircleEffect) no Inspector.", this);
                return false;
            }

            // Valida se PlayerController.Instance existe
            if (PlayerController.Instance == null)
            {
                Debug.LogError($"TeleportPoint '{gameObject.name}': PlayerController.Instance não encontrado na cena! Certifique-se de que o Player está presente.", this);
                return false;
            }

            // Valida se SceneTransitioner.Instance existe
            if (SceneTransitioner.Instance == null)
            {
                Debug.LogError($"TeleportPoint '{gameObject.name}': SceneTransitioner.Instance não encontrado na cena! Adicione o prefab SceneTransitioner à cena.", this);
                return false;
            }

            // Todas as validações passaram
            return true;
        }

        #endregion

        #region Gizmos

        /// <summary>
        /// Desenha visualização do TeleportPoint no Editor para facilitar level design.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!enableGizmos)
                return;

            // Desenha área do trigger de ativação
            Gizmos.color = gizmoColor;

            // Se o collider existe, usa suas configurações
            if (triggerCollider != null)
            {
                Vector3 center = transform.position + (Vector3)triggerCollider.offset;
                Vector3 size = triggerCollider.size;
                DrawWireCube(center, size);
            }
            else
            {
                // Desenha preview do trigger mesmo sem o collider configurado
                Vector3 center = transform.position + (Vector3)triggerOffset;
                Vector3 size = triggerSize;
                DrawWireCube(center, size);
            }

            // Desenha linha e seta para o destino se configurado
            if (destinationPosition != Vector3.zero)
            {
                // Linha conectando origem ao destino
                Gizmos.color = gizmoColor;
                Gizmos.DrawLine(transform.position, destinationPosition);

                // Marcador no ponto de destino
                Gizmos.DrawWireSphere(destinationPosition, 0.5f);

                // Desenha seta indicando direção
                Vector3 direction = (destinationPosition - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Vector3 arrowTip = destinationPosition - direction * 0.5f;
                    Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized * 0.3f;

                    // Duas linhas formando a ponta da seta
                    Gizmos.DrawLine(arrowTip, arrowTip + Quaternion.Euler(0, 0, 45) * -direction * 0.3f);
                    Gizmos.DrawLine(arrowTip, arrowTip + Quaternion.Euler(0, 0, -45) * -direction * 0.3f);
                }

#if UNITY_EDITOR
                // Label com informações de debug
                string labelText;
                if (IsCrossSceneTeleport())
                {
                    // Cross-scene teleport - mostra nome da cena de destino
                    labelText = $"TeleportPoint (Cross-Scene)\n→ {destinationSceneName}\n{destinationPosition}";
                }
                else
                {
                    // Same-scene teleport
                    labelText = $"TeleportPoint\n→ {destinationPosition}";
                }

                UnityEditor.Handles.Label(
                    transform.position + Vector3.up * 0.5f,
                    labelText,
                    new UnityEngine.GUIStyle()
                    {
                        normal = new UnityEngine.GUIStyleState() { textColor = IsCrossSceneTeleport() ? Color.yellow : gizmoColor },
                        alignment = UnityEngine.TextAnchor.MiddleCenter,
                        fontSize = 10
                    }
                );
#endif
            }
        }

        /// <summary>
        /// Desenha um cubo wireframe (helper para Gizmos).
        /// </summary>
        private void DrawWireCube(Vector3 center, Vector3 size)
        {
            Vector3 halfSize = size * 0.5f;

            // Vértices do cubo
            Vector3[] vertices = new Vector3[8];
            vertices[0] = center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
            vertices[1] = center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
            vertices[2] = center + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
            vertices[3] = center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
            vertices[4] = center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
            vertices[5] = center + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            vertices[6] = center + new Vector3(halfSize.x, halfSize.y, halfSize.z);
            vertices[7] = center + new Vector3(-halfSize.x, halfSize.y, halfSize.z);

            // Desenha as 12 arestas do cubo
            // Face frontal
            Gizmos.DrawLine(vertices[0], vertices[1]);
            Gizmos.DrawLine(vertices[1], vertices[2]);
            Gizmos.DrawLine(vertices[2], vertices[3]);
            Gizmos.DrawLine(vertices[3], vertices[0]);

            // Face traseira
            Gizmos.DrawLine(vertices[4], vertices[5]);
            Gizmos.DrawLine(vertices[5], vertices[6]);
            Gizmos.DrawLine(vertices[6], vertices[7]);
            Gizmos.DrawLine(vertices[7], vertices[4]);

            // Conectando frente e trás
            Gizmos.DrawLine(vertices[0], vertices[4]);
            Gizmos.DrawLine(vertices[1], vertices[5]);
            Gizmos.DrawLine(vertices[2], vertices[6]);
            Gizmos.DrawLine(vertices[3], vertices[7]);
        }

        #endregion
    }
}
