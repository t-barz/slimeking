using UnityEngine;
using SlimeKing.Core;
using SlimeKing.Systems;
using SlimeKing.Gameplay;

namespace SlimeKing.Core
{
    /// <summary>
    /// Script responsável por inicializar todos os Managers na ordem correta.
    /// Deve ser colocado em um GameObject na primeira cena do jogo.
    /// </summary>
    public class ManagerInitializer : MonoBehaviour
    {
        [Header("Manager Prefabs (Optional)")]
        [SerializeField] private GameObject gameManagerPrefab;
        [SerializeField] private GameObject audioManagerPrefab;
        [SerializeField] private GameObject saveManagerPrefab;
        [SerializeField] private GameObject inputManagerPrefab;
        [SerializeField] private GameObject uiManagerPrefab;

        [Header("Settings")]
        [SerializeField] private bool initializeOnAwake = true;
        [SerializeField] private bool enableDebugLogs = true;

        private void Awake()
        {
            if (initializeOnAwake)
            {
                InitializeAllManagers();
            }
        }

        /// <summary>
        /// Inicializa todos os Managers na ordem correta
        /// </summary>
        public void InitializeAllManagers()
        {
            Log("Starting Manager initialization...");

            // Tier 1: Core Managers (sem dependências)
            InitializeCoreManagers();

            // Tier 3: System Managers (dependem de Core)
            InitializeSystemManagers();

            // Tier 2: Gameplay Managers (dependem de System e Core)
            InitializeGameplayManagers();

            Log("All Managers initialized successfully!");
        }

        /// <summary>
        /// Inicializa os Core Managers
        /// </summary>
        private void InitializeCoreManagers()
        {
            Log("Initializing Core Managers...");

            // GameManager - Coordenação geral
            InstantiateOrFindManager<GameManager>(gameManagerPrefab, "GameManager");

            // AudioManager - Sistema de áudio
            InstantiateOrFindManager<AudioManager>(audioManagerPrefab, "AudioManager");

            // SaveManager - Persistência de dados
            InstantiateOrFindManager<SaveManager>(saveManagerPrefab, "SaveManager");

            // UIManager - Interface e transições (já implementado)
            if (uiManagerPrefab != null)
            {
                InstantiateOrFindManager<SlimeKing.Systems.UI.UIManager>(uiManagerPrefab, "UIManager");
            }
        }

        /// <summary>
        /// Inicializa os System Managers
        /// </summary>
        private void InitializeSystemManagers()
        {
            Log("Initializing System Managers...");

            // InputManager - Sistema de input
            InstantiateOrFindManager<InputManager>(inputManagerPrefab, "InputManager");

            // CameraManager - Controle de câmera
            InstantiateOrFindManager<CameraManager>(null, "CameraManager");
        }

        /// <summary>
        /// Inicializa os Gameplay Managers
        /// </summary>
        private void InitializeGameplayManagers()
        {
            Log("Initializing Gameplay Managers...");

            // PlayerManager - Estado do jogador
            InstantiateOrFindManager<PlayerManager>(null, "PlayerManager");

            // EnemyManager - Controle de inimigos
            InstantiateOrFindManager<EnemyManager>(null, "EnemyManager");
        }

        /// <summary>
        /// Instancia ou encontra um Manager específico
        /// </summary>
        private void InstantiateOrFindManager<T>(GameObject prefab, string managerName) where T : MonoBehaviour
        {
            // Verifica se já existe uma instância
            T existingManager = FindFirstObjectByType<T>();

            if (existingManager != null)
            {
                Log($"{managerName} already exists in scene");
                return;
            }

            // Tenta instanciar do prefab
            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab);
                instance.name = managerName;
                Log($"{managerName} instantiated from prefab");
                return;
            }

            // Cria um GameObject vazio e adiciona o componente
            GameObject managerGO = new GameObject(managerName);
            managerGO.AddComponent<T>();
            Log($"{managerName} created as new GameObject");
        }

        /// <summary>
        /// Log com controle de debug
        /// </summary>
        private void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[ManagerInitializer] {message}");
            }
        }

        /// <summary>
        /// Método público para reinicializar todos os Managers
        /// </summary>
        [ContextMenu("Reinitialize All Managers")]
        public void ReinitializeAllManagers()
        {
            Log("Reinitializing all Managers...");
            InitializeAllManagers();
        }

        /// <summary>
        /// Verifica se todos os Managers essenciais estão presentes
        /// </summary>
        [ContextMenu("Check Manager Status")]
        public void CheckManagerStatus()
        {
            Log("Checking Manager status...");

            // Core Managers
            CheckManager<GameManager>("GameManager");
            CheckManager<AudioManager>("AudioManager");
            CheckManager<SaveManager>("SaveManager");
            CheckManager<SlimeKing.Systems.UI.UIManager>("UIManager");

            // System Managers
            CheckManager<InputManager>("InputManager");
            CheckManager<CameraManager>("CameraManager");

            // Gameplay Managers
            CheckManager<PlayerManager>("PlayerManager");
            CheckManager<EnemyManager>("EnemyManager");

            Log("Manager status check completed");
        }

        /// <summary>
        /// Verifica se um Manager específico está presente
        /// </summary>
        private void CheckManager<T>(string managerName) where T : MonoBehaviour
        {
            T manager = FindFirstObjectByType<T>();
            string status = manager != null ? "✓ Active" : "✗ Missing";
            Log($"{managerName}: {status}");
        }
    }
}