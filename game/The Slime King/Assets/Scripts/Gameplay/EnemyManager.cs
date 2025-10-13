using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SlimeKing.Core;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Manager para controle de spawn e gerenciamento de inimigos.
    /// Usa object pooling para performance otimizada.
    /// </summary>
    public class EnemyManager : ManagerBase<EnemyManager>
    {
        [Header("Enemy Configuration")]
        [SerializeField] private EnemyData[] enemyTypes;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private int poolSize = 20;
        
        [Header("Spawn Settings")]
        [SerializeField] private float baseSpawnInterval = 5f;
        [SerializeField] private int maxEnemiesAlive = 10;
        [SerializeField] private bool autoSpawnEnabled = false;
        
        [Header("Events")]
        public UnityEvent<GameObject> OnEnemySpawned = new UnityEvent<GameObject>();
        public UnityEvent<GameObject> OnEnemyDied = new UnityEvent<GameObject>();
        public UnityEvent OnAllEnemiesDefeated = new UnityEvent();
        
        // Pool system
        private Dictionary<EnemyType, Queue<GameObject>> enemyPools;
        private Dictionary<EnemyType, EnemyData> enemyDataDict;
        
        // Active enemies tracking
        private List<GameObject> activeEnemies;
        private int totalEnemiesSpawned = 0;
        
        // Spawn system
        private Coroutine autoSpawnCoroutine;
        
        // Properties
        public int ActiveEnemiesCount => activeEnemies.Count;
        public int TotalEnemiesSpawned => totalEnemiesSpawned;
        public bool HasActiveEnemies => activeEnemies.Count > 0;
        
        protected override void Initialize()
        {
            SetupEnemyData();
            InitializePools();
            activeEnemies = new List<GameObject>();
            
            if (autoSpawnEnabled)
            {
                StartAutoSpawn();
            }
            
            Log("EnemyManager initialized");
        }
        
        #region Setup
        
        /// <summary>
        /// Configura o dictionary de dados dos inimigos
        /// </summary>
        private void SetupEnemyData()
        {
            enemyDataDict = new Dictionary<EnemyType, EnemyData>();
            
            foreach (var enemyData in enemyTypes)
            {
                if (!enemyDataDict.ContainsKey(enemyData.type))
                {
                    enemyDataDict[enemyData.type] = enemyData;
                }
            }
        }
        
        /// <summary>
        /// Inicializa os pools de inimigos
        /// </summary>
        private void InitializePools()
        {
            enemyPools = new Dictionary<EnemyType, Queue<GameObject>>();
            
            foreach (var enemyData in enemyTypes)
            {
                if (enemyData.prefab != null)
                {
                    CreatePool(enemyData.type, enemyData.prefab);
                }
            }
        }
        
        /// <summary>
        /// Cria um pool para um tipo específico de inimigo
        /// </summary>
        private void CreatePool(EnemyType enemyType, GameObject prefab)
        {
            var pool = new Queue<GameObject>();
            var poolParent = new GameObject($"Pool_{enemyType}");
            poolParent.transform.SetParent(transform);
            
            for (int i = 0; i < poolSize / enemyTypes.Length; i++)
            {
                GameObject enemy = Instantiate(prefab, poolParent.transform);
                enemy.SetActive(false);
                
                // Adiciona componente Enemy se não existir
                if (enemy.GetComponent<Enemy>() == null)
                {
                    enemy.AddComponent<Enemy>();
                }
                
                pool.Enqueue(enemy);
            }
            
            enemyPools[enemyType] = pool;
            Log($"Created pool for {enemyType} with {pool.Count} objects");
        }
        
        #endregion
        
        #region Spawning
        
        /// <summary>
        /// Spawna um inimigo específico
        /// </summary>
        public GameObject SpawnEnemy(EnemyType enemyType, Vector3 position, Quaternion rotation = default)
        {
            if (!enemyDataDict.ContainsKey(enemyType))
            {
                LogWarning($"Enemy type {enemyType} not found");
                return null;
            }
            
            if (activeEnemies.Count >= maxEnemiesAlive)
            {
                LogWarning("Maximum enemies limit reached");
                return null;
            }
            
            GameObject enemy = GetPooledEnemy(enemyType);
            if (enemy == null)
            {
                LogWarning($"No available enemy of type {enemyType} in pool");
                return null;
            }
            
            // Posiciona e ativa o inimigo
            enemy.transform.position = position;
            enemy.transform.rotation = rotation == default ? Quaternion.identity : rotation;
            enemy.SetActive(true);
            
            // Inicializa o componente Enemy
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.Initialize(enemyDataDict[enemyType]);
            }
            
            // Adiciona à lista de ativos
            activeEnemies.Add(enemy);
            totalEnemiesSpawned++;
            
            OnEnemySpawned?.Invoke(enemy);
            Log($"Spawned {enemyType} at {position}. Active enemies: {activeEnemies.Count}");
            
            return enemy;
        }
        
        /// <summary>
        /// Spawna um inimigo em um ponto de spawn aleatório
        /// </summary>
        public GameObject SpawnEnemyAtRandomPoint(EnemyType enemyType)
        {
            if (spawnPoints.Length == 0)
            {
                LogWarning("No spawn points configured");
                return null;
            }
            
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            return SpawnEnemy(enemyType, randomSpawnPoint.position, randomSpawnPoint.rotation);
        }
        
        /// <summary>
        /// Spawna múltiplos inimigos
        /// </summary>
        public void SpawnEnemies(EnemySpawnData[] spawnData)
        {
            StartCoroutine(SpawnEnemiesCoroutine(spawnData));
        }
        
        /// <summary>
        /// Corrotina para spawnar inimigos com delay
        /// </summary>
        private IEnumerator SpawnEnemiesCoroutine(EnemySpawnData[] spawnData)
        {
            foreach (var data in spawnData)
            {
                for (int i = 0; i < data.count; i++)
                {
                    SpawnEnemy(data.enemyType, data.spawnPosition);
                    
                    if (data.spawnDelay > 0f)
                    {
                        yield return new WaitForSeconds(data.spawnDelay);
                    }
                }
            }
        }
        
        #endregion
        
        #region Pool Management
        
        /// <summary>
        /// Obtém um inimigo do pool
        /// </summary>
        private GameObject GetPooledEnemy(EnemyType enemyType)
        {
            if (!enemyPools.ContainsKey(enemyType))
                return null;
                
            var pool = enemyPools[enemyType];
            
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            
            // Se o pool estiver vazio, cria um novo objeto
            if (enemyDataDict.ContainsKey(enemyType) && enemyDataDict[enemyType].prefab != null)
            {
                GameObject newEnemy = Instantiate(enemyDataDict[enemyType].prefab);
                if (newEnemy.GetComponent<Enemy>() == null)
                {
                    newEnemy.AddComponent<Enemy>();
                }
                return newEnemy;
            }
            
            return null;
        }
        
        /// <summary>
        /// Retorna um inimigo para o pool
        /// </summary>
        public void ReturnEnemyToPool(GameObject enemy, EnemyType enemyType)
        {
            if (enemy == null) return;
            
            // Remove da lista de ativos
            activeEnemies.Remove(enemy);
            
            // Desativa o objeto
            enemy.SetActive(false);
            
            // Retorna para o pool
            if (enemyPools.ContainsKey(enemyType))
            {
                enemyPools[enemyType].Enqueue(enemy);
            }
            
            OnEnemyDied?.Invoke(enemy);
            Log($"Enemy {enemyType} returned to pool. Active enemies: {activeEnemies.Count}");
            
            // Verifica se todos os inimigos foram derrotados
            if (activeEnemies.Count == 0)
            {
                OnAllEnemiesDefeated?.Invoke();
                Log("All enemies defeated");
            }
        }
        
        #endregion
        
        #region Enemy Management
        
        /// <summary>
        /// Encontra o inimigo mais próximo de uma posição
        /// </summary>
        public GameObject GetNearestEnemy(Vector3 position, float maxDistance = float.MaxValue)
        {
            GameObject nearestEnemy = null;
            float nearestDistance = maxDistance;
            
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null && enemy.activeInHierarchy)
                {
                    float distance = Vector3.Distance(position, enemy.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEnemy = enemy;
                    }
                }
            }
            
            return nearestEnemy;
        }
        
        /// <summary>
        /// Obtém todos os inimigos em um raio
        /// </summary>
        public List<GameObject> GetEnemiesInRadius(Vector3 position, float radius)
        {
            List<GameObject> enemiesInRadius = new List<GameObject>();
            
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null && enemy.activeInHierarchy)
                {
                    float distance = Vector3.Distance(position, enemy.transform.position);
                    if (distance <= radius)
                    {
                        enemiesInRadius.Add(enemy);
                    }
                }
            }
            
            return enemiesInRadius;
        }
        
        /// <summary>
        /// Remove todos os inimigos ativos
        /// </summary>
        public void ClearAllEnemies()
        {
            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                GameObject enemy = activeEnemies[i];
                if (enemy != null)
                {
                    Enemy enemyComponent = enemy.GetComponent<Enemy>();
                    if (enemyComponent != null)
                    {
                        ReturnEnemyToPool(enemy, enemyComponent.EnemyType);
                    }
                    else
                    {
                        activeEnemies.RemoveAt(i);
                        enemy.SetActive(false);
                    }
                }
            }
            
            Log("All enemies cleared");
        }
        
        #endregion
        
        #region Auto Spawn
        
        /// <summary>
        /// Inicia o spawn automático
        /// </summary>
        public void StartAutoSpawn()
        {
            if (autoSpawnCoroutine == null)
            {
                autoSpawnCoroutine = StartCoroutine(AutoSpawnCoroutine());
                Log("Auto spawn started");
            }
        }
        
        /// <summary>
        /// Para o spawn automático
        /// </summary>
        public void StopAutoSpawn()
        {
            if (autoSpawnCoroutine != null)
            {
                StopCoroutine(autoSpawnCoroutine);
                autoSpawnCoroutine = null;
                Log("Auto spawn stopped");
            }
        }
        
        /// <summary>
        /// Corrotina de spawn automático
        /// </summary>
        private IEnumerator AutoSpawnCoroutine()
        {
            while (autoSpawnEnabled)
            {
                yield return new WaitForSeconds(baseSpawnInterval);
                
                if (activeEnemies.Count < maxEnemiesAlive && enemyTypes.Length > 0)
                {
                    EnemyType randomType = enemyTypes[Random.Range(0, enemyTypes.Length)].type;
                    SpawnEnemyAtRandomPoint(randomType);
                }
            }
        }
        
        #endregion
        
        #region Settings
        
        /// <summary>
        /// Define o limite máximo de inimigos vivos
        /// </summary>
        public void SetMaxEnemiesAlive(int max)
        {
            maxEnemiesAlive = Mathf.Max(1, max);
            Log($"Max enemies alive set to {maxEnemiesAlive}");
        }
        
        /// <summary>
        /// Define o intervalo de spawn automático
        /// </summary>
        public void SetSpawnInterval(float interval)
        {
            baseSpawnInterval = Mathf.Max(0.1f, interval);
            Log($"Spawn interval set to {baseSpawnInterval}s");
        }
        
        /// <summary>
        /// Habilita/desabilita spawn automático
        /// </summary>
        public void SetAutoSpawn(bool enabled)
        {
            autoSpawnEnabled = enabled;
            
            if (enabled)
            {
                StartAutoSpawn();
            }
            else
            {
                StopAutoSpawn();
            }
        }
        
        #endregion
        
        #region Cleanup
        
        private void OnDestroy()
        {
            StopAutoSpawn();
        }
        
        #endregion
    }
}