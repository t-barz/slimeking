using UnityEngine;
using PixeLadder.EasyTransition;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Componente que detecta colisão do Player e executa teletransporte entre cenas diferentes.
    /// 
    /// ESTRUTURA BÁSICA - Aguardando discussão sobre implementação.
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

        [Tooltip("Tempo de espera após carregamento antes do fade in (segundos)")]
        [SerializeField] private float delayBeforeFadeIn = 1f;

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

            ExecuteCrossSceneTeleport();
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
        /// TODO: Implementar lógica de teletransporte cross-scene.
        /// </summary>
        private void ExecuteCrossSceneTeleport()
        {
            isTeleporting = true;

            // TODO: Validações necessárias
            // - Verificar se destinationSceneName não está vazio
            // - Verificar se a cena existe nas Build Settings
            // - Verificar se transitionEffect está atribuído
            // - Verificar se PlayerController existe

            if (enableDebugLogs)
            {
                Debug.Log($"[CrossSceneTeleportPoint] Executando teletransporte:");
                Debug.Log($"  - Cena destino: {destinationSceneName}");
                Debug.Log($"  - Posição destino: {destinationPosition}");
            }

            // TODO: Implementar sequência de teletransporte
            // Questões para discussão:
            // 1. Como garantir que o Player persiste entre cenas?
            // 2. Quando aplicar DontDestroyOnLoad no Player?
            // 3. Como sincronizar a transição visual com o carregamento da cena?
            // 4. Como reposicionar o Player na nova cena?
            // 5. Como garantir que a câmera segue o Player corretamente?
            // 6. Quando reabilitar o movimento do Player?

            isTeleporting = false;
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
