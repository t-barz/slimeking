using UnityEngine;
using System.Collections;

/// <summary>
/// PortalManager Ultra-Otimizado com Preload Inteligente
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PortalManager : MonoBehaviour
{
    [Header("Configurações do Portal")]
    [SerializeField] private string portalID = "";
    [SerializeField] private string destinationScene;
    [SerializeField] private Vector2 targetPosition = Vector2.zero;

    [Header("Otimizações")]
    [SerializeField] private bool preloadOnStart = true;
    [SerializeField] private bool preloadOnPlayerNear = true;
    [SerializeField] private float preloadDistance = 5f;

    [Header("Configurações Visuais")]
    [SerializeField] private Color portalColor = Color.black;

    private bool isTransitioning = false;
    private bool isPreloaded = false;
    private bool playerNearby = false;

    // Cache ultra-otimizado
    private Collider2D cachedCollider;
    private GameManager cachedGameManager;
    private Transform cachedTransform;
    private GameObject cachedPlayer;

    // Constantes
    private const string PLAYER_TAG = "Player";
    private const float PRELOAD_CHECK_INTERVAL = 1f;

    #region Unity Lifecycle
    private void Start()
    {
        InitializePortalFast();

        if (preloadOnStart)
        {
            PreloadDestinationScene();
        }

        if (preloadOnPlayerNear)
        {
            StartCoroutine(MonitorPlayerDistance());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG) && !isTransitioning && CanUsePortal())
        {
            // Preload de emergência se não foi feito
            if (!isPreloaded)
            {
                PreloadDestinationScene();
            }

            StartCoroutine(UsePortalFast());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            playerNearby = false;
        }
    }
    #endregion

    #region Inicialização Otimizada
    /// <summary>
    /// Inicialização ultra-rápida
    /// </summary>
    private void InitializePortalFast()
    {
        // Cache de componentes
        cachedCollider = GetComponent<Collider2D>();
        cachedTransform = transform;
        cachedGameManager = GameManager.Instance;

        cachedCollider.isTrigger = true;

        if (string.IsNullOrEmpty(portalID))
        {
            portalID = $"Portal_{gameObject.name}_{GetInstanceID()}";
        }

        Debug.Log($"[PortalManager] Portal rápido '{portalID}' inicializado");
    }

    /// <summary>
    /// Pré-carrega cena de destino
    /// </summary>
    private void PreloadDestinationScene()
    {
        if (isPreloaded || string.IsNullOrEmpty(destinationScene)) return;

        if (cachedGameManager != null)
        {
            cachedGameManager.PreloadScene(destinationScene);
            isPreloaded = true;
            Debug.Log($"[PortalManager] Portal '{portalID}' - Cena '{destinationScene}' pré-carregada");
        }
    }
    #endregion

    #region Sistema de Proximidade
    /// <summary>
    /// Monitora distância do player para preload inteligente
    /// </summary>
    private IEnumerator MonitorPlayerDistance()
    {
        WaitForSeconds checkInterval = new WaitForSeconds(PRELOAD_CHECK_INTERVAL);

        while (true)
        {
            if (!isPreloaded && !playerNearby)
            {
                if (cachedPlayer == null)
                {
                    cachedPlayer = GameObject.FindGameObjectWithTag(PLAYER_TAG);
                }

                if (cachedPlayer != null)
                {
                    float distance = Vector2.Distance(cachedTransform.position, cachedPlayer.transform.position);

                    if (distance <= preloadDistance)
                    {
                        playerNearby = true;
                        PreloadDestinationScene();
                        Debug.Log($"[PortalManager] Player próximo - Preload ativado: {destinationScene}");
                    }
                }
            }

            yield return checkInterval;
        }
    }
    #endregion

    #region Portal Otimizado
    /// <summary>
    /// Uso ultra-rápido do portal
    /// </summary>
    private IEnumerator UsePortalFast()
    {
        isTransitioning = true;

        Debug.Log($"[PortalManager] FAST Portal '{portalID}' -> {destinationScene}");

        try
        {
            // Transição instantânea se em modo ultra-rápido
            cachedGameManager.ChangeScene(destinationScene, targetPosition, portalColor);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PortalManager] Erro portal '{portalID}': {e.Message}");
            isTransitioning = false;
        }

        yield return null;
    }

    /// <summary>
    /// Verificação rápida se portal pode ser usado
    /// </summary>
    private bool CanUsePortal()
    {
        return cachedGameManager != null && !string.IsNullOrEmpty(destinationScene);
    }
    #endregion

    #region Métodos Públicos
    /// <summary>
    /// Força preload da cena de destino
    /// </summary>
    [ContextMenu("Force Preload Scene")]
    public void ForcePreloadScene()
    {
        PreloadDestinationScene();
    }

    /// <summary>
    /// Define nova cena e faz preload automático
    /// </summary>
    public void SetDestinationSceneWithPreload(string sceneName)
    {
        destinationScene = sceneName;
        isPreloaded = false;

        if (preloadOnStart)
        {
            PreloadDestinationScene();
        }

        Debug.Log($"[PortalManager] Nova cena '{sceneName}' definida com preload");
    }
    #endregion
}
