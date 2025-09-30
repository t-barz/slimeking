
using UnityEngine;
using System;

/// <summary>
/// Estrutura de dados que representa um ponto de teletransporte.
/// </summary>
public struct TeleportPointData
{
    /// <summary>ID único do ponto de teleporte.</summary>
    public string pointId;
    /// <summary>Nome amigável do ponto de teleporte.</summary>
    public string pointName;
    /// <summary>Nome da cena de destino.</summary>
    public string destinationSceneName;
    /// <summary>Posição de destino para o player.</summary>
    public Vector3 destinationPosition;
    /// <summary>Layers válidas para detectar o player.</summary>
    public LayerMask playerLayers;
}

/// <summary>
/// Componente responsável por definir e gerenciar um ponto de teletransporte na cena.
/// Detecta a presença do Player via trigger 2D e dispara eventos para integração com sistemas de transição de tela.
/// Os dados do ponto são configuráveis pelo Inspector.
/// </summary>
public class TeleportPointHandler : MonoBehaviour
{
    [Header("Configuração do Teleport Point")]
    [Tooltip("ID único do ponto de teleporte.")]
    [SerializeField] private string pointId;

    [Tooltip("Nome amigável do ponto de teleporte.")]
    [SerializeField] private string pointName;

    [Tooltip("Nome da cena de destino.")]
    [SerializeField] private string destinationSceneName;

    [Tooltip("Posição de destino para o player.")]
    [SerializeField] private Vector3 destinationPosition;

    [Tooltip("Layers válidas para detectar o player.")]
    [SerializeField] private LayerMask playerLayers;

    /// <summary>
    /// Evento disparado quando o Player entra no trigger deste ponto.
    /// O parâmetro é o GameObject do Player detectado.
    /// </summary>
    public event Action<GameObject> OnPlayerEnter;

    /// <summary>
    /// Evento disparado quando o Player sai do trigger deste ponto.
    /// O parâmetro é o GameObject do Player detectado.
    /// </summary>
    public event Action<GameObject> OnPlayerExit;

    /// <summary>
    /// Indica se o Player está atualmente próximo (dentro do trigger).
    /// </summary>
    public bool IsPlayerNearby { get; private set; } = false;

    /// <summary>
    /// Retorna os dados configurados deste ponto de teleporte.
    /// </summary>
    public TeleportPointData TeleportData => new TeleportPointData
    {
        pointId = pointId,
        pointName = pointName,
        destinationSceneName = destinationSceneName,
        destinationPosition = destinationPosition,
        playerLayers = playerLayers
    };

    /// <summary>
    /// Detecta a entrada de um objeto no trigger. Se for o Player, atualiza o estado e dispara o evento.
    /// </summary>
    /// <param name="other">Collider2D detectado</param>

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerNearby = true;
            Debug.Log($"[TeleportPointHandler] Player entrou no trigger do ponto '{pointId}'");

            // Inicia o efeito de vinheta fechando e, ao finalizar, notifica o TeleportScreenManager
            var effects = SlimeMec.Gameplay.ScreenEffectsManager.Instance;
            if (effects != null)
            {
                effects.CloseVignette(
                    duration: 1.0f,
                    maxIntensity: 1f,
                    onClosed: () =>
                    {
                        // Notifica o TeleportScreenManager para iniciar a transição
                        if (TeleportScreenManager.Instance != null)
                        {
                            TeleportScreenManager.Instance.StartTeleportTransition(this.TeleportData, other.gameObject);
                        }
                    }
                );
            }
            else
            {
                Debug.LogWarning("[TeleportPointHandler] ScreenEffectsManager não encontrado para efeito de vinheta.");
            }

            OnPlayerEnter?.Invoke(other.gameObject);
        }
    }

    // Desenha um Gizmo no editor para facilitar a visualização do ponto de teleporte
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.7f);

        // Destino na mesma cena
        if (!string.IsNullOrEmpty(destinationSceneName) && destinationSceneName == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, destinationPosition);
            Gizmos.DrawWireSphere(destinationPosition, 0.4f);
        }

        // Label
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1.1f, $"TP: {pointId}");
#endif
    }

    /// <summary>
    /// Detecta a saída de um objeto do trigger. Se for o Player, atualiza o estado e dispara o evento.
    /// </summary>
    /// <param name="other">Collider2D detectado</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerNearby = false;
            OnPlayerExit?.Invoke(other.gameObject);
        }
    }
}
