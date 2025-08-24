using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindManager : MonoBehaviour
{
    [Header("🌪️ Configurações de Vento")]
    [SerializeField] private GameObject windPrefab;
    [SerializeField] private float windSpawnFrequency = 3f;

    [Header("📦 Outros Objetos")]
    [SerializeField] private GameObject[] otherObjectsPrefabs;
    [SerializeField] private float otherObjectsSpawnFrequency = 5f;

    [Header("🎯 Configurações de Área")]
    [SerializeField] private Vector2 spawnAreaCenter = Vector2.zero;
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10f, 6f);

    [Header("🔧 Opções Avançadas")]
    [SerializeField] private bool autoStart = true;
    [SerializeField] private bool spawnAsChildren = true;
    [SerializeField] private bool showSpawnArea = true;
    [SerializeField] private bool enableLogs = false;

    [Header("⚡ Performance")]
    [SerializeField] private int maxActiveWinds = 5;
    [SerializeField] private float cleanupInterval = 2f;

    // OTIMIZADO: Pools para evitar garbage collection
    private Queue<GameObject> windPool = new Queue<GameObject>();
    private List<GameObject> activeWinds = new List<GameObject>();

    private Coroutine windSpawnCoroutine;
    private Coroutine cleanupCoroutine;
    private bool isSpawning = false;

    // OTIMIZADO: Cache para otimização
    private WaitForSeconds windSpawnWait;
    private WaitForSeconds cleanupWait;
    private Vector3 worldSpawnCenter;
    private Camera mainCamera;

    #region Unity Lifecycle

    void Start()
    {
        InitializeManager();

        if (autoStart)
        {
            StartSpawning();
        }
    }

    #endregion

    #region Initialization - OTIMIZADO

    /// <summary>
    /// OTIMIZADO: Inicialização com pre-cache de componentes
    /// </summary>
    private void InitializeManager()
    {
        // Cache de WaitForSeconds
        windSpawnWait = new WaitForSeconds(windSpawnFrequency);
        cleanupWait = new WaitForSeconds(cleanupInterval);

        // Cache da camera principal
        mainCamera = Camera.main;

        // Calcula centro mundial
        worldSpawnCenter = transform.position + (Vector3)spawnAreaCenter;

        // NOVO: Pre-instancia alguns ventos no pool
        PrewarmWindPool();
    }

    /// <summary>
    /// NOVO: Pre-aquece o pool de ventos para melhor performance
    /// </summary>
    private void PrewarmWindPool()
    {
        if (windPrefab == null) return;

        for (int i = 0; i < 3; i++) // Pre-instancia 3 objetos
        {
            GameObject pooledWind = Instantiate(windPrefab);
            pooledWind.SetActive(false);
            pooledWind.transform.SetParent(transform);
            windPool.Enqueue(pooledWind);
        }
    }

    #endregion

    #region Spawn Control - OTIMIZADO

    public void StartSpawning()
    {
        if (isSpawning) return;

        isSpawning = true;

        if (windPrefab != null && windSpawnFrequency > 0f)
        {
            windSpawnCoroutine = StartCoroutine(SpawnWindRoutine());
        }

        // NOVO: Inicia rotina de cleanup
        cleanupCoroutine = StartCoroutine(CleanupRoutine());
    }

    public void StopSpawning()
    {
        if (!isSpawning) return;

        isSpawning = false;

        if (windSpawnCoroutine != null)
        {
            StopCoroutine(windSpawnCoroutine);
            windSpawnCoroutine = null;
        }

        if (cleanupCoroutine != null)
        {
            StopCoroutine(cleanupCoroutine);
            cleanupCoroutine = null;
        }
    }

    #endregion

    #region Spawn Routines - OTIMIZADO

    /// <summary>
    /// OTIMIZADO: Controla limite máximo de ventos ativos
    /// </summary>
    private IEnumerator SpawnWindRoutine()
    {
        while (isSpawning)
        {
            // OTIMIZADO: Só spawna se não atingiu o limite
            if (activeWinds.Count < maxActiveWinds)
            {
                SpawnWind();
            }

            yield return windSpawnWait;
        }
    }

    /// <summary>
    /// NOVO: Rotina de limpeza para objetos inativos
    /// </summary>
    private IEnumerator CleanupRoutine()
    {
        while (isSpawning)
        {
            yield return cleanupWait;
            CleanupInactiveWinds();
        }
    }

    #endregion

    #region Wind Management - OTIMIZADO

    /// <summary>
    /// OTIMIZADO: Usa object pooling para spawning
    /// </summary>
    public void SpawnWind()
    {
        if (windPrefab == null) return;

        GameObject wind = GetPooledWind();
        if (wind == null) return;

        // Configura posição e ativa o objeto
        Vector3 spawnPosition = GetRandomSpawnPosition();
        wind.transform.position = spawnPosition;
        wind.SetActive(true);

        // Adiciona à lista de ativos
        activeWinds.Add(wind);

        if (enableLogs)
        {
            Debug.Log($"WindManager: Vento ativado em {spawnPosition}");
        }
    }

    /// <summary>
    /// OTIMIZADO: Object pooling para reutilização de objetos
    /// </summary>
    private GameObject GetPooledWind()
    {
        // Tenta pegar do pool primeiro
        if (windPool.Count > 0)
        {
            return windPool.Dequeue();
        }

        // Se pool vazio, instancia novo
        GameObject newWind = Instantiate(windPrefab);
        if (spawnAsChildren)
        {
            newWind.transform.SetParent(transform);
        }

        return newWind;
    }

    /// <summary>
    /// NOVO: Remove ventos inativos da lista e retorna ao pool
    /// </summary>
    private void CleanupInactiveWinds()
    {
        for (int i = activeWinds.Count - 1; i >= 0; i--)
        {
            if (activeWinds[i] == null || !activeWinds[i].activeInHierarchy)
            {
                if (activeWinds[i] != null)
                {
                    ReturnWindToPool(activeWinds[i]);
                }
                activeWinds.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// NOVO: Retorna vento ao pool para reutilização
    /// </summary>
    private void ReturnWindToPool(GameObject wind)
    {
        wind.SetActive(false);
        wind.transform.position = Vector3.zero;

        // OTIMIZADO: Limita tamanho do pool
        if (windPool.Count < 10)
        {
            windPool.Enqueue(wind);
        }
        else
        {
            Destroy(wind); // Destrói se pool cheio
        }
    }

    #endregion

    #region Utility Methods - OTIMIZADO

    /// <summary>
    /// OTIMIZADO: Usa centro mundial cacheado
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        float halfWidth = spawnAreaSize.x * 0.5f;
        float halfHeight = spawnAreaSize.y * 0.5f;

        float randomX = Random.Range(worldSpawnCenter.x - halfWidth, worldSpawnCenter.x + halfWidth);
        float randomY = Random.Range(worldSpawnCenter.y - halfHeight, worldSpawnCenter.y + halfHeight);

        return new Vector3(randomX, randomY, worldSpawnCenter.z);
    }

    /// <summary>
    /// OTIMIZADO: Cleanup melhorado
    /// </summary>
    public void ClearAllActiveWinds()
    {
        foreach (GameObject wind in activeWinds)
        {
            if (wind != null)
            {
                ReturnWindToPool(wind);
            }
        }
        activeWinds.Clear();
    }

    #endregion

    #region Public API

    public void SetWindSpawnFrequency(float newFrequency)
    {
        windSpawnFrequency = newFrequency;
        windSpawnWait = new WaitForSeconds(windSpawnFrequency);

        if (isSpawning && windSpawnCoroutine != null)
        {
            StopCoroutine(windSpawnCoroutine);
            windSpawnCoroutine = StartCoroutine(SpawnWindRoutine());
        }
    }

    public void ForceSpawnWind() => SpawnWind();

    #endregion

    #region Properties

    public bool IsSpawning => isSpawning;
    public int ActiveWindsCount => activeWinds.Count;
    public int PooledWindsCount => windPool.Count;
    public float WindSpawnFrequency => windSpawnFrequency;

    #endregion

    #region Debug

    private void OnDrawGizmos()
    {
        if (!showSpawnArea) return;

        Vector3 worldCenter = transform.position + (Vector3)spawnAreaCenter;
        Vector3 size = new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f);

        Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
        Gizmos.DrawCube(worldCenter, size);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(worldCenter, size);

#if UNITY_EDITOR
        string label = $"Wind Area\nActive: {(Application.isPlaying ? activeWinds.Count : 0)}\nPooled: {(Application.isPlaying ? windPool.Count : 0)}";
        UnityEditor.Handles.Label(worldCenter + Vector3.up * (spawnAreaSize.y * 0.5f + 1f), label);
#endif
    }

    private void OnDestroy()
    {
        StopSpawning();
    }

    #endregion
}
