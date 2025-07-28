using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cria instâncias de prefabs em intervalos regulares com variação de posição aleatória.
/// Otimizado para jogos 2D (apenas eixos X e Y).
/// </summary>
public class WindCreator : MonoBehaviour
{
    #region Serialized Fields
    [Header("Prefab Configuration")]
    [Tooltip("Prefab que será instanciado")]
    [SerializeField] private GameObject prefabToSpawn;

    [Tooltip("Frequência de spawn (instâncias por segundo)")]
    [SerializeField, Range(0.1f, 10f)] private float spawnFrequency = 1f;

    [Header("Position Settings")]
    [Tooltip("Range de variação aleatória na posição X")]
    [SerializeField, Range(0f, 10f)] private float randomRangeX = 2f;

    [Tooltip("Range de variação aleatória na posição Y")]
    [SerializeField, Range(0f, 10f)] private float randomRangeY = 2f;

    [Header("Advanced Settings")]
    [Tooltip("Limite máximo de instâncias ativas (0 = ilimitado)")]
    [SerializeField, Range(0, 100)] private int maxInstances = 0;

    [Tooltip("Destruir instâncias automaticamente após X segundos (0 = não destruir)")]
    [SerializeField, Range(0f, 60f)] private float autoDestroyTime = 10f;

    [Header("Debug & Visualization")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private bool showSpawnLogs = false;
    [SerializeField] private Color spawnAreaColor = Color.green;
    [SerializeField] private Color spawnPointColor = Color.red;
    #endregion

    #region Private Fields
    private Coroutine spawnCoroutine;
    private WaitForSeconds spawnInterval;
    private readonly List<GameObject> activeInstances = new List<GameObject>();
    private float lastSpawnFrequency;

    // Cache para otimização de performance
    private Transform cachedTransform;
    private Vector2 cachedBasePosition;
    private const float MINIMUM_SPAWN_FREQUENCY = 0.1f;
    private const float MAXIMUM_SPAWN_FREQUENCY = 10f;
    #endregion

    #region Constants
    private const float GIZMO_SPHERE_SIZE = 0.2f;
    private const float GIZMO_WIRE_SIZE = 0.1f;
    #endregion

    #region Properties
    /// <summary>
    /// Número de instâncias ativas atualmente
    /// </summary>
    public int ActiveInstancesCount => activeInstances.Count;

    /// <summary>
    /// Indica se o spawn automático está ativo
    /// </summary>
    public bool IsSpawning => spawnCoroutine != null;

    /// <summary>
    /// Posição base para spawn (sempre a posição deste objeto)
    /// </summary>
    public Vector2 BaseSpawnPosition => cachedBasePosition;

    /// <summary>
    /// Intervalo atual entre spawns
    /// </summary>
    public float CurrentSpawnInterval => 1f / spawnFrequency;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        // Cache do Transform para evitar chamadas repetidas
        cachedTransform = transform;
        UpdateCachedPosition();

        ValidateConfiguration();
        InitializeSystem();
    }

    private void Start()
    {
        StartAutoSpawn();
    }

    private void Update()
    {
        // Atualiza posição em cache se o transform mudou
        Vector2 currentPos = cachedTransform.position;
        if (Vector2.Distance(currentPos, cachedBasePosition) > 0.01f)
        {
            UpdateCachedPosition();
        }
    }

    private void OnDestroy()
    {
        StopAutoSpawn();
        ClearAllInstances();
    }

    private void OnValidate()
    {
        // Recalcula intervalo quando frequência muda no Inspector
        if (Application.isPlaying && lastSpawnFrequency != spawnFrequency)
        {
            UpdateSpawnInterval();
            lastSpawnFrequency = spawnFrequency;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        DrawSpawnAreaGizmos();
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Atualiza a posição em cache
    /// </summary>
    private void UpdateCachedPosition()
    {
        cachedBasePosition = cachedTransform.position;
    }

    /// <summary>
    /// Valida a configuração inicial
    /// </summary>
    private void ValidateConfiguration()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("[WindCreator] Prefab não configurado. Sistema não funcionará.");
        }

        if (spawnFrequency <= 0f)
        {
            Debug.LogWarning($"[WindCreator] Frequência inválida: {spawnFrequency}. Usando valor padrão.");
            spawnFrequency = 1f;
        }

        if (maxInstances < 0)
        {
            Debug.LogWarning($"[WindCreator] Limite de instâncias inválido: {maxInstances}. Usando 0 (ilimitado).");
            maxInstances = 0;
        }
    }

    /// <summary>
    /// Inicializa o sistema
    /// </summary>
    private void InitializeSystem()
    {
        UpdateSpawnInterval();
        lastSpawnFrequency = spawnFrequency;

        if (showSpawnLogs)
        {
            Debug.Log($"[WindCreator] Sistema inicializado. Frequência: {spawnFrequency}/s, Intervalo: {CurrentSpawnInterval:F2}s");
            Debug.Log($"[WindCreator] Instâncias serão criadas como objetos independentes na cena");
        }
    }

    /// <summary>
    /// Atualiza o intervalo de spawn baseado na frequência
    /// </summary>
    private void UpdateSpawnInterval()
    {
        spawnInterval = new WaitForSeconds(1f / spawnFrequency);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Inicia o spawn automático
    /// </summary>
    public void StartAutoSpawn()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("[WindCreator] Não é possível iniciar spawn sem prefab configurado.");
            return;
        }

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        spawnCoroutine = StartCoroutine(AutoSpawnCoroutine());

        if (showSpawnLogs)
        {
            Debug.Log("[WindCreator] Spawn automático iniciado");
        }
    }

    /// <summary>
    /// Para o spawn automático
    /// </summary>
    public void StopAutoSpawn()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        if (showSpawnLogs)
        {
            Debug.Log("[WindCreator] Spawn automático parado");
        }
    }

    /// <summary>
    /// Cria uma instância manualmente
    /// </summary>
    [ContextMenu("Spawn Instance Now")]
    public GameObject SpawnInstance()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("[WindCreator] Prefab não configurado para spawn.");
            return null;
        }

        // Verifica limite de instâncias
        if (maxInstances > 0 && activeInstances.Count >= maxInstances)
        {
            if (showSpawnLogs)
            {
                Debug.Log($"[WindCreator] Limite de instâncias atingido ({maxInstances}). Removendo a mais antiga.");
            }
            RemoveOldestInstance();
        }

        // Calcula posição com variação aleatória em 2D
        Vector2 spawnPosition = CalculateRandomSpawnPosition();

        // Instancia o prefab como objeto independente (sem parent)
        GameObject instance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        // Adiciona à lista de instâncias ativas
        activeInstances.Add(instance);

        // Configura destruição automática se necessário
        if (autoDestroyTime > 0f)
        {
            StartCoroutine(DestroyInstanceAfterTime(instance, autoDestroyTime));
        }

        if (showSpawnLogs)
        {
            Debug.Log($"[WindCreator] Instância independente criada: '{instance.name}' em {spawnPosition}");
        }

        return instance;
    }

    /// <summary>
    /// Remove todas as instâncias ativas
    /// </summary>
    [ContextMenu("Clear All Instances")]
    public void ClearAllInstances()
    {
        int removedCount = 0;

        for (int i = activeInstances.Count - 1; i >= 0; i--)
        {
            if (activeInstances[i] != null)
            {
                DestroyImmediate(activeInstances[i]);
                removedCount++;
            }
        }

        activeInstances.Clear();

        if (showSpawnLogs)
        {
            Debug.Log($"[WindCreator] {removedCount} instâncias independentes removidas");
        }
    }

    /// <summary>
    /// Configura novo prefab para spawn
    /// </summary>
    public void SetPrefab(GameObject newPrefab)
    {
        prefabToSpawn = newPrefab;

        if (showSpawnLogs)
        {
            Debug.Log($"[WindCreator] Prefab alterado para: {(newPrefab != null ? newPrefab.name : "null")}");
        }
    }

    /// <summary>
    /// Altera a frequência de spawn
    /// </summary>
    public void SetSpawnFrequency(float newFrequency)
    {
        spawnFrequency = Mathf.Clamp(newFrequency, MINIMUM_SPAWN_FREQUENCY, MAXIMUM_SPAWN_FREQUENCY);
        UpdateSpawnInterval();

        if (showSpawnLogs)
        {
            Debug.Log($"[WindCreator] Frequência alterada para: {spawnFrequency}/s");
        }
    }

    /// <summary>
    /// Configura ranges de variação aleatória em 2D
    /// </summary>
    public void SetRandomRanges(float rangeX, float rangeY)
    {
        randomRangeX = Mathf.Max(0f, rangeX);
        randomRangeY = Mathf.Max(0f, rangeY);

        if (showSpawnLogs)
        {
            Debug.Log($"[WindCreator] Ranges 2D alterados: X={randomRangeX}, Y={randomRangeY}");
        }
    }

    /// <summary>
    /// Habilita/desabilita o spawn automático
    /// </summary>
    public void SetAutoSpawnEnabled(bool enabled)
    {
        if (enabled)
        {
            StartAutoSpawn();
        }
        else
        {
            StopAutoSpawn();
        }
    }

    /// <summary>
    /// Retorna uma cópia do array de instâncias ativas
    /// </summary>
    public GameObject[] GetActiveInstances()
    {
        // Remove nulls primeiro
        activeInstances.RemoveAll(instance => instance == null);

        var result = new GameObject[activeInstances.Count];
        activeInstances.CopyTo(result);
        return result;
    }

    /// <summary>
    /// Verifica se uma instância específica está ativa
    /// </summary>
    public bool IsInstanceActive(GameObject instance)
    {
        return instance != null && activeInstances.Contains(instance);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Coroutine para spawn automático
    /// </summary>
    private IEnumerator AutoSpawnCoroutine()
    {
        while (true)
        {
            SpawnInstance();
            yield return spawnInterval;
        }
    }

    /// <summary>
    /// Calcula posição aleatória para spawn em 2D (otimizado)
    /// </summary>
    private Vector2 CalculateRandomSpawnPosition()
    {
        // Usa posição em cache para evitar acesso ao Transform
        Vector2 basePosition = cachedBasePosition;

        // Calcula variação aleatória de uma vez só para melhor performance
        float randomX = Random.Range(-randomRangeX, randomRangeX);
        float randomY = Random.Range(-randomRangeY, randomRangeY);

        // Retorna diretamente sem criar Vector2 intermediário
        return new Vector2(basePosition.x + randomX, basePosition.y + randomY);
    }

    /// <summary>
    /// Remove a instância mais antiga quando limite é atingido
    /// </summary>
    private void RemoveOldestInstance()
    {
        // Remove nulls primeiro
        activeInstances.RemoveAll(instance => instance == null);

        if (activeInstances.Count > 0)
        {
            GameObject oldest = activeInstances[0];
            activeInstances.RemoveAt(0);

            if (oldest != null)
            {
                Destroy(oldest);
            }
        }
    }

    /// <summary>
    /// Coroutine para destruir instância após tempo determinado
    /// </summary>
    private IEnumerator DestroyInstanceAfterTime(GameObject instance, float time)
    {
        yield return new WaitForSeconds(time);

        if (instance != null)
        {
            activeInstances.Remove(instance);
            Destroy(instance);

            if (showSpawnLogs)
            {
                Debug.Log($"[WindCreator] Instância '{instance.name}' destruída automaticamente após {time}s");
            }
        }
    }

    /// <summary>
    /// Desenha gizmos da área de spawn em 2D
    /// </summary>
    private void DrawSpawnAreaGizmos()
    {
        Vector2 center = BaseSpawnPosition;

        // Desenha área de spawn como retângulo 2D
        Gizmos.color = spawnAreaColor;
        Vector3 size = new Vector3(randomRangeX * 2, randomRangeY * 2, 0f);
        Gizmos.DrawWireCube(center, size);

        // Desenha ponto central de spawn
        Gizmos.color = spawnPointColor;
        Gizmos.DrawSphere(center, GIZMO_SPHERE_SIZE);

        // Desenha instâncias ativas
        if (Application.isPlaying && activeInstances.Count > 0)
        {
            Gizmos.color = Color.cyan;
            foreach (GameObject instance in activeInstances)
            {
                if (instance != null)
                {
                    Gizmos.DrawWireSphere(instance.transform.position, GIZMO_WIRE_SIZE);

                    // Linha conectando o WindCreator à instância
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(center, instance.transform.position);
                    Gizmos.color = Color.cyan;
                }
            }
        }

        // Label com informações
#if UNITY_EDITOR
        string info = $"Freq: {spawnFrequency:F1}/s\nAtivas: {ActiveInstancesCount}";
        if (maxInstances > 0)
        {
            info += $"/{maxInstances}";
        }
        info += $"\nIndependentes: {ActiveInstancesCount}";
        UnityEditor.Handles.Label(center + Vector2.up * 1f, info);
#endif
    }
    #endregion
}
