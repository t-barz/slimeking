using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gerenciador responsável por spawnar objetos de vento e outros objetos em área configurável.
/// Controla frequência de spawn e posicionamento aleatório dentro da área definida.
/// </summary>
public class WindManager : MonoBehaviour
{
    [Header("🌪️ Configurações de Vento")]
    [Tooltip("Prefab do GameObject que representa o vento")]
    [SerializeField] private GameObject windPrefab;

    [Tooltip("Frequência de spawn do vento (segundos entre spawns)")]
    [SerializeField] private float windSpawnFrequency = 3f;

    [Header("📦 Outros Objetos")]
    [Tooltip("Lista de prefabs opcionais para spawn")]
    [SerializeField] private GameObject[] otherObjectsPrefabs;

    [Tooltip("Frequência de spawn dos outros objetos (segundos entre spawns)")]
    [SerializeField] private float otherObjectsSpawnFrequency = 5f;

    [Header("🎯 Configurações de Área")]
    [Tooltip("Centro da área de spawn (relativo à posição deste objeto)")]
    [SerializeField] private Vector2 spawnAreaCenter = Vector2.zero;

    [Tooltip("Tamanho da área de spawn (largura x altura)")]
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10f, 6f);

    [Header("🔧 Opções Avançadas")]
    [Tooltip("Se true, inicia o spawn automaticamente")]
    [SerializeField] private bool autoStart = true;

    [Tooltip("Se true, spawna objetos como filhos deste GameObject")]
    [SerializeField] private bool spawnAsChildren = true;

    [Tooltip("Se true, mostra a área de spawn no Scene View")]
    [SerializeField] private bool showSpawnArea = true;

    [Tooltip("Se true, mostra logs de debug no console")]
    [SerializeField] private bool enableLogs = false;

    // Controles internos de spawn
    private Coroutine windSpawnCoroutine;
    private Coroutine otherObjectsSpawnCoroutine;
    private bool isSpawning = false;

    // Lista para controle de objetos spawnados (opcional)
    private List<GameObject> spawnedObjects = new List<GameObject>();

    /// <summary>
    /// Inicialização do manager
    /// </summary>
    void Start()
    {
        if (autoStart)
        {
            StartSpawning();
        }
    }

    /// <summary>
    /// Inicia o sistema de spawn para vento e outros objetos
    /// </summary>
    public void StartSpawning()
    {
        if (isSpawning) return;

        isSpawning = true;

        // Inicia spawn de vento se o prefab estiver configurado
        if (windPrefab != null && windSpawnFrequency > 0f)
        {
            windSpawnCoroutine = StartCoroutine(SpawnWindRoutine());
            if (enableLogs)
            {
                Debug.Log($"WindManager: Iniciado spawn de vento (frequência: {windSpawnFrequency}s)");
            }
        }

        // Inicia spawn de outros objetos se houver prefabs configurados
        if (otherObjectsPrefabs != null && otherObjectsPrefabs.Length > 0 && otherObjectsSpawnFrequency > 0f)
        {
            otherObjectsSpawnCoroutine = StartCoroutine(SpawnOtherObjectsRoutine());
            if (enableLogs)
            {
                Debug.Log($"WindManager: Iniciado spawn de outros objetos (frequência: {otherObjectsSpawnFrequency}s)");
            }
        }
    }

    /// <summary>
    /// Para o sistema de spawn
    /// </summary>
    public void StopSpawning()
    {
        if (!isSpawning) return;

        isSpawning = false;

        // Para spawn de vento
        if (windSpawnCoroutine != null)
        {
            StopCoroutine(windSpawnCoroutine);
            windSpawnCoroutine = null;
        }

        // Para spawn de outros objetos
        if (otherObjectsSpawnCoroutine != null)
        {
            StopCoroutine(otherObjectsSpawnCoroutine);
            otherObjectsSpawnCoroutine = null;
        }

        if (enableLogs)
        {
            Debug.Log("WindManager: Sistema de spawn parado");
        }
    }

    /// <summary>
    /// Corrotina responsável pelo spawn contínuo de objetos de vento
    /// </summary>
    private IEnumerator SpawnWindRoutine()
    {
        while (isSpawning)
        {
            SpawnWind();
            yield return new WaitForSeconds(windSpawnFrequency);
        }
    }

    /// <summary>
    /// Corrotina responsável pelo spawn contínuo de outros objetos
    /// </summary>
    private IEnumerator SpawnOtherObjectsRoutine()
    {
        while (isSpawning)
        {
            SpawnRandomOtherObject();
            yield return new WaitForSeconds(otherObjectsSpawnFrequency);
        }
    }

    /// <summary>
    /// Spawna um objeto de vento em posição aleatória
    /// </summary>
    public void SpawnWind()
    {
        if (windPrefab == null) return;

        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject spawnedWind = Instantiate(windPrefab, spawnPosition, Quaternion.identity);

        if (spawnAsChildren)
        {
            spawnedWind.transform.SetParent(transform);
        }

        spawnedObjects.Add(spawnedWind);

        if (enableLogs)
        {
            Debug.Log($"WindManager: Vento spawnado em {spawnPosition}");
        }
    }

    /// <summary>
    /// Spawna um objeto aleatório da lista de outros objetos
    /// </summary>
    public void SpawnRandomOtherObject()
    {
        if (otherObjectsPrefabs == null || otherObjectsPrefabs.Length == 0) return;

        // Seleciona um prefab aleatório
        int randomIndex = Random.Range(0, otherObjectsPrefabs.Length);
        GameObject selectedPrefab = otherObjectsPrefabs[randomIndex];

        if (selectedPrefab == null) return;

        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject spawnedObject = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        if (spawnAsChildren)
        {
            spawnedObject.transform.SetParent(transform);
        }

        spawnedObjects.Add(spawnedObject);

        if (enableLogs)
        {
            Debug.Log($"WindManager: Objeto '{selectedPrefab.name}' spawnado em {spawnPosition}");
        }
    }

    /// <summary>
    /// Calcula uma posição aleatória dentro da área de spawn configurada
    /// </summary>
    private Vector3 GetRandomSpawnPosition()
    {
        // Calcula os limites da área de spawn
        Vector3 worldCenter = transform.position + (Vector3)spawnAreaCenter;
        float minX = worldCenter.x - spawnAreaSize.x * 0.5f;
        float maxX = worldCenter.x + spawnAreaSize.x * 0.5f;
        float minY = worldCenter.y - spawnAreaSize.y * 0.5f;
        float maxY = worldCenter.y + spawnAreaSize.y * 0.5f;

        // Gera posição aleatória dentro dos limites
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        return new Vector3(randomX, randomY, worldCenter.z);
    }

    /// <summary>
    /// Força o spawn imediato de um objeto de vento
    /// </summary>
    public void ForceSpawnWind()
    {
        SpawnWind();
    }

    /// <summary>
    /// Força o spawn imediato de um objeto aleatório
    /// </summary>
    public void ForceSpawnOtherObject()
    {
        SpawnRandomOtherObject();
    }

    /// <summary>
    /// Limpa todos os objetos spawnados
    /// </summary>
    public void ClearAllSpawnedObjects()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();

        if (enableLogs)
        {
            Debug.Log("WindManager: Todos os objetos spawnados foram removidos");
        }
    }

    /// <summary>
    /// Altera a frequência de spawn do vento em tempo de execução
    /// </summary>
    public void SetWindSpawnFrequency(float newFrequency)
    {
        windSpawnFrequency = newFrequency;

        // Reinicia o spawn de vento se estiver ativo
        if (isSpawning && windSpawnCoroutine != null)
        {
            StopCoroutine(windSpawnCoroutine);
            windSpawnCoroutine = StartCoroutine(SpawnWindRoutine());
        }
    }

    /// <summary>
    /// Altera a frequência de spawn dos outros objetos em tempo de execução
    /// </summary>
    public void SetOtherObjectsSpawnFrequency(float newFrequency)
    {
        otherObjectsSpawnFrequency = newFrequency;

        // Reinicia o spawn de outros objetos se estiver ativo
        if (isSpawning && otherObjectsSpawnCoroutine != null)
        {
            StopCoroutine(otherObjectsSpawnCoroutine);
            otherObjectsSpawnCoroutine = StartCoroutine(SpawnOtherObjectsRoutine());
        }
    }

    /// <summary>
    /// Desenha a área de spawn no Scene View para visualização
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showSpawnArea) return;

        // Calcula posição e tamanho da área
        Vector3 worldCenter = transform.position + (Vector3)spawnAreaCenter;
        Vector3 size = new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.1f);

        // Desenha área de spawn
        Gizmos.color = new Color(0f, 1f, 1f, 0.3f); // Cyan transparente
        Gizmos.DrawCube(worldCenter, size);

        // Desenha contorno da área
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(worldCenter, size);

        // Label com informações
#if UNITY_EDITOR
        Vector3 labelPos = worldCenter + Vector3.up * (spawnAreaSize.y * 0.5f + 1f);
        string label = $"Wind Spawn Area\nWind: {windSpawnFrequency}s\nOthers: {otherObjectsSpawnFrequency}s";
        UnityEditor.Handles.Label(labelPos, label);
#endif
    }

    /// <summary>
    /// Cleanup quando o objeto é destruído
    /// </summary>
    private void OnDestroy()
    {
        StopSpawning();
    }

    /// <summary>
    /// Propriedades públicas para acesso externo
    /// </summary>
    public bool IsSpawning => isSpawning;
    public int SpawnedObjectsCount => spawnedObjects.Count;
    public float WindSpawnFrequency => windSpawnFrequency;
    public float OtherObjectsSpawnFrequency => otherObjectsSpawnFrequency;
}
