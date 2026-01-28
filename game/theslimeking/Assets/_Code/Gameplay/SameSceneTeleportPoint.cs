using System.Collections;
using UnityEngine;
using PixeLadder.EasyTransition;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Componente que detecta colisão do Player e executa teletransporte dentro da mesma cena.
    /// Utiliza o Easy Transition para criar uma experiência fluida de teletransporte.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class SameSceneTeleportPoint : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Teleport Configuration")]
        [Tooltip("Posição de destino do teletransporte")]
        [SerializeField] private Vector3 destinationPosition;

        [Tooltip("Efeito de transição a ser utilizado (CircleEffect recomendado)")]
        [SerializeField] private TransitionEffect transitionEffect;

        [Tooltip("Tempo de espera após reposicionamento antes do fade in (segundos)")]
        [SerializeField] private float delayBeforeFadeIn = 1f;

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

        private void Awake()
        {
            triggerCollider = GetComponent<BoxCollider2D>();

            if (triggerCollider == null)
            {
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
                return;
            }

            StartCoroutine(ExecuteTeleport());
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

        private IEnumerator ExecuteTeleport()
        {
            isTeleporting = true;

            if (!ValidateTeleport())
            {
                isTeleporting = false;
                yield break;
            }

            if (playerRigidbody == null)
            {
                playerRigidbody = PlayerController.Instance.GetComponent<Rigidbody2D>();
            }

            PlayerController.Instance.DisableMovement();

            if (playerRigidbody != null)
            {
                playerRigidbody.linearVelocity = Vector2.zero;
            }

            yield return TeleportTransitionHelper.ExecuteTransition(
                transitionEffect,
                RepositionPlayerAndCamera,
                delayBeforeFadeIn,
                enableDebugLogs
            );

            PlayerController.Instance.EnableMovement();

            isTeleporting = false;
        }

        private void RepositionPlayerAndCamera()
        {
            if (PlayerController.Instance == null)
            {
                return;
            }

            if (cameraTransform == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    cameraTransform = mainCamera.transform;
                }
            }

            Vector3 cameraOffset = Vector3.zero;
            if (cameraTransform != null)
            {
                cameraOffset = cameraTransform.position - PlayerController.Instance.transform.position;
            }

            PlayerController.Instance.transform.position = destinationPosition;

            if (SlimeKing.Core.CameraManager.HasInstance)
            {
                SlimeKing.Core.CameraManager.Instance.ForceCinemachineSetup();
            }

            if (cameraTransform != null)
            {
                Vector3 newCameraPosition = destinationPosition + cameraOffset;
                cameraTransform.position = newCameraPosition;
            }
        }

        private bool ValidateTeleport()
        {
            if (transitionEffect == null)
            {
                return false;
            }

            if (PlayerController.Instance == null)
            {
                return false;
            }

            if (SceneTransitioner.Instance == null)
            {
                return false;
            }

            return true;
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

            if (destinationPosition != Vector3.zero)
            {
                Gizmos.color = gizmoColor;
                Gizmos.DrawLine(transform.position, destinationPosition);
                Gizmos.DrawWireSphere(destinationPosition, 0.5f);

                Vector3 direction = (destinationPosition - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Vector3 arrowTip = destinationPosition - direction * 0.5f;
                    Gizmos.DrawLine(arrowTip, arrowTip + Quaternion.Euler(0, 0, 45) * -direction * 0.3f);
                    Gizmos.DrawLine(arrowTip, arrowTip + Quaternion.Euler(0, 0, -45) * -direction * 0.3f);
                }

#if UNITY_EDITOR
                string labelText = $"Same-Scene Teleport\n→ {destinationPosition}";
                UnityEditor.Handles.Label(
                    transform.position + Vector3.up * 0.5f,
                    labelText,
                    new UnityEngine.GUIStyle()
                    {
                        normal = new UnityEngine.GUIStyleState() { textColor = gizmoColor },
                        alignment = UnityEngine.TextAnchor.MiddleCenter,
                        fontSize = 10
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
