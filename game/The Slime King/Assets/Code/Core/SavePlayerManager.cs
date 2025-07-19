using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Gerencia o sistema de salvamento e carregamento das informações do jogador.
/// Suporta salvamento automático e 3 slots de save manual.
/// Salva EntityStatus, posição de respawn (cena e coordenadas).
/// </summary>
public class SavePlayerManager : MonoBehaviour
{
    #region Constants
    private const string AUTO_SAVE_PREFIX = "AutoSave_";
    private const string SAVE_SLOT_PREFIX = "SaveSlot_";
    private const int MAX_SAVE_SLOTS = 3;
    #endregion

    #region Singleton
    public static SavePlayerManager Instance { get; private set; }
    #endregion

    #region Configuration
    [Header("Configurações de Save")]
    [Tooltip("Ativa o salvamento automático")]
    [SerializeField] private bool enableAutoSave = true;

    [Tooltip("Intervalo em segundos para salvamento automático")]
    [SerializeField] private float autoSaveInterval = 30f;

    [Tooltip("Tag do jogador")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Ativa logs de debug")]
    [SerializeField] private bool enableDebugLogs = true;
    #endregion

    #region Private Variables
    private float lastAutoSaveTime;
    private GameObject cachedPlayer;
    private EntityStatus cachedEntityStatus;
    #endregion

    #region Unity Events
    /// <summary>
    /// Inicialização do Singleton
    /// </summary>
    private void Awake()
    {
        // Implementa Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Registra eventos de cena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Inicialização após carregamento
    /// </summary>
    private void Start()
    {
        // Carrega dados salvos ao iniciar
        LoadPlayerData();
    }

    /// <summary>
    /// Atualização contínua para auto-save
    /// </summary>
    private void Update()
    {
        // Auto-save periódico
        if (enableAutoSave && Time.time - lastAutoSaveTime >= autoSaveInterval)
        {
            AutoSavePlayerData();
            lastAutoSaveTime = Time.time;
        }
    }

    /// <summary>
    /// Limpeza ao destruir
    /// </summary>
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    #region Data Structures
    /// <summary>
    /// Estrutura de dados do jogador para salvamento
    /// </summary>
    [System.Serializable]
    public class PlayerSaveData
    {
        // Dados de EntityStatus
        public float currentHealth;
        public float maxHealth;
        public float currentMana;
        public float maxMana;
        public float speed;
        public float attackDamage;
        public float defensePower;

        // Dados de posição e respawn
        public string currentSceneName;
        public Vector3 currentPosition;
        public string respawnSceneName;
        public Vector3 respawnPosition;

        // Metadados
        public DateTime saveTime;
        public float playTime;

        /// <summary>
        /// Construtor vazio para serialização
        /// </summary>
        public PlayerSaveData() { }

        /// <summary>
        /// Construtor com dados do jogador
        /// </summary>
        public PlayerSaveData(EntityStatus entityStatus, Transform playerTransform, string sceneName, DateTime saveTime, float playTime)
        {
            // EntityStatus
            if (entityStatus != null)
            {
                this.currentHealth = entityStatus.currentHP;
                this.maxHealth = entityStatus.GetMaxHP();
                this.currentMana = 0f; // Sistema de mana não implementado ainda
                this.maxMana = 0f;
                this.speed = entityStatus.GetSpeed();
                this.attackDamage = entityStatus.GetAttack();
                this.defensePower = entityStatus.GetDefense();
            }

            // Posição e cena
            this.currentSceneName = sceneName;
            this.currentPosition = playerTransform != null ? playerTransform.position : Vector3.zero;
            this.respawnSceneName = sceneName; // Por padrão, respawn na cena atual
            this.respawnPosition = this.currentPosition;

            // Metadados
            this.saveTime = saveTime;
            this.playTime = playTime;
        }
    }
    #endregion

    #region Scene Management
    /// <summary>
    /// Callback chamado quando uma nova cena é carregada
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[SavePlayerManager] Cena carregada: {scene.name}");
        }

        // Limpa cache ao carregar nova cena
        cachedPlayer = null;
        cachedEntityStatus = null;

        // Carrega dados salvos para a nova cena
        StartCoroutine(LoadPlayerDataCoroutine());
    }

    /// <summary>
    /// Corrotina para carregar dados com delay
    /// </summary>
    private System.Collections.IEnumerator LoadPlayerDataCoroutine()
    {
        // Espera alguns frames para garantir que o jogador foi instanciado
        yield return new WaitForSeconds(0.5f);
        LoadPlayerData();
    }
    #endregion

    #region Core Save/Load Methods
    /// <summary>
    /// Obtém referências cached do jogador e EntityStatus
    /// </summary>
    private void GetPlayerReferences()
    {
        if (cachedPlayer == null)
        {
            cachedPlayer = GameObject.FindGameObjectWithTag(playerTag);

            // Se encontrou o jogador pela primeira vez, define posição inicial como respawn
            if (cachedPlayer != null && !PlayerPrefs.HasKey("RespawnX"))
            {
                SetRespawnPoint(cachedPlayer.transform.position);

                if (enableDebugLogs)
                {
                    Debug.Log($"[SavePlayerManager] Posição inicial definida como respawn: {cachedPlayer.transform.position}");
                }
            }
        }

        if (cachedPlayer != null && cachedEntityStatus == null)
        {
            cachedEntityStatus = cachedPlayer.GetComponent<EntityStatus>();
        }
    }

    /// <summary>
    /// Carrega dados salvos do jogador (auto-save ou slot específico)
    /// </summary>
    public void LoadPlayerData(int saveSlot = -1)
    {
        GetPlayerReferences();

        if (cachedPlayer == null)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning("[SavePlayerManager] Jogador não encontrado para carregar dados");
            }
            return;
        }

        string saveKey = saveSlot >= 0 ? $"{SAVE_SLOT_PREFIX}{saveSlot}" : AUTO_SAVE_PREFIX;

        if (PlayerPrefs.HasKey(saveKey + "Data"))
        {
            string saveJson = PlayerPrefs.GetString(saveKey + "Data");
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(saveJson);

            if (saveData != null)
            {
                ApplyPlayerData(saveData);

                if (enableDebugLogs)
                {
                    string slotInfo = saveSlot >= 0 ? $"Slot {saveSlot}" : "Auto-Save";
                    Debug.Log($"[SavePlayerManager] Dados carregados do {slotInfo}");
                }
            }
        }
        else
        {
            // Se não há save, mas não há respawn definido, usa posição atual como respawn
            if (!PlayerPrefs.HasKey("RespawnX") && cachedPlayer != null)
            {
                SetRespawnPoint(cachedPlayer.transform.position);

                if (enableDebugLogs)
                {
                    Debug.Log("[SavePlayerManager] Nenhum save encontrado, posição atual definida como respawn inicial");
                }
            }
            else if (enableDebugLogs)
            {
                Debug.Log("[SavePlayerManager] Nenhum save encontrado, usando valores padrão");
            }
        }
    }

    /// <summary>
    /// Aplica os dados carregados ao jogador
    /// </summary>
    private void ApplyPlayerData(PlayerSaveData saveData)
    {
        if (cachedPlayer == null || cachedEntityStatus == null) return;

        // Aplica dados de EntityStatus
        cachedEntityStatus.currentHP = (int)saveData.currentHealth;
        cachedEntityStatus.currentLevel = Mathf.Max(1, (int)(saveData.maxHealth / cachedEntityStatus.baseHP));

        // Aplica posição (se estiver na mesma cena)
        if (saveData.currentSceneName == SceneManager.GetActiveScene().name)
        {
            cachedPlayer.transform.position = saveData.currentPosition;

            if (enableDebugLogs)
            {
                Debug.Log($"[SavePlayerManager] Posição aplicada: {saveData.currentPosition}");
            }
        }
    }

    /// <summary>
    /// Salva dados do jogador automaticamente
    /// </summary>
    public void AutoSavePlayerData()
    {
        SavePlayerData(-1); // -1 indica auto-save
    }

    /// <summary>
    /// Salva dados do jogador em um slot específico
    /// </summary>
    public void SavePlayerData(int saveSlot = -1)
    {
        GetPlayerReferences();

        if (cachedPlayer == null || cachedEntityStatus == null)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning("[SavePlayerManager] Jogador ou EntityStatus não encontrado para salvar");
            }
            return;
        }

        // Cria dados de save
        PlayerSaveData saveData = new PlayerSaveData(
            cachedEntityStatus,
            cachedPlayer.transform,
            SceneManager.GetActiveScene().name,
            DateTime.Now,
            Time.time
        );

        // Serializa e salva
        string saveJson = JsonUtility.ToJson(saveData, true);
        string saveKey = saveSlot >= 0 ? $"{SAVE_SLOT_PREFIX}{saveSlot}" : AUTO_SAVE_PREFIX;

        PlayerPrefs.SetString(saveKey + "Data", saveJson);
        PlayerPrefs.SetString(saveKey + "Timestamp", DateTime.Now.ToBinary().ToString());
        PlayerPrefs.Save();

        if (enableDebugLogs)
        {
            string slotInfo = saveSlot >= 0 ? $"Slot {saveSlot}" : "Auto-Save";
            Debug.Log($"[SavePlayerManager] Dados salvos no {slotInfo}");
        }
    }
    #endregion

    #region Respawn Management
    /// <summary>
    /// Define um novo ponto de respawn
    /// </summary>
    public void SetRespawnPoint(Vector3 position, string sceneName = null)
    {
        string scene = sceneName ?? SceneManager.GetActiveScene().name;

        PlayerPrefs.SetFloat("RespawnX", position.x);
        PlayerPrefs.SetFloat("RespawnY", position.y);
        PlayerPrefs.SetFloat("RespawnZ", position.z);
        PlayerPrefs.SetString("RespawnScene", scene);
        PlayerPrefs.Save();

        if (enableDebugLogs)
        {
            Debug.Log($"[SavePlayerManager] Novo ponto de respawn definido: {position} na cena {scene}");
        }
    }

    /// <summary>
    /// Define a posição atual do jogador como ponto de respawn
    /// </summary>
    public void SetCurrentPositionAsRespawn()
    {
        GetPlayerReferences();

        if (cachedPlayer != null)
        {
            SetRespawnPoint(cachedPlayer.transform.position);

            if (enableDebugLogs)
            {
                Debug.Log($"[SavePlayerManager] Posição atual do jogador definida como respawn: {cachedPlayer.transform.position}");
            }
        }
        else if (enableDebugLogs)
        {
            Debug.LogWarning("[SavePlayerManager] Jogador não encontrado para definir respawn");
        }
    }

    /// <summary>
    /// Respawna o jogador no último ponto de respawn salvo
    /// </summary>
    public void RespawnPlayer()
    {
        GetPlayerReferences();

        if (cachedPlayer == null)
        {
            Debug.LogWarning("[SavePlayerManager] Jogador não encontrado para respawn");
            return;
        }

        // Verifica se há ponto de respawn salvo
        if (PlayerPrefs.HasKey("RespawnX"))
        {
            Vector3 respawnPosition = new Vector3(
                PlayerPrefs.GetFloat("RespawnX"),
                PlayerPrefs.GetFloat("RespawnY"),
                PlayerPrefs.GetFloat("RespawnZ")
            );

            string respawnScene = PlayerPrefs.GetString("RespawnScene", SceneManager.GetActiveScene().name);

            // Se for a mesma cena, apenas reposiciona
            if (respawnScene == SceneManager.GetActiveScene().name)
            {
                cachedPlayer.transform.position = respawnPosition;

                // Restaura HP completo no respawn
                if (cachedEntityStatus != null)
                {
                    cachedEntityStatus.currentHP = cachedEntityStatus.GetMaxHP();
                }

                if (enableDebugLogs)
                {
                    Debug.Log($"[SavePlayerManager] Jogador respawnado em: {respawnPosition}");
                }
            }
            else
            {
                // Carrega a cena de respawn usando PlayerPrefs
                PlayerPrefs.SetFloat("PortalDestinationX", respawnPosition.x);
                PlayerPrefs.SetFloat("PortalDestinationY", respawnPosition.y);
                PlayerPrefs.Save();

                SceneManager.LoadScene(respawnScene);
            }
        }
        else
        {
            Debug.LogWarning("[SavePlayerManager] Nenhum ponto de respawn definido");
        }
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Verifica se existe um save em um slot específico
    /// </summary>
    public bool HasSaveData(int saveSlot = -1)
    {
        string saveKey = saveSlot >= 0 ? $"{SAVE_SLOT_PREFIX}{saveSlot}" : AUTO_SAVE_PREFIX;
        return PlayerPrefs.HasKey(saveKey + "Data");
    }

    /// <summary>
    /// Obtém informações de um save sem carregá-lo
    /// </summary>
    public SaveSlotInfo GetSaveSlotInfo(int saveSlot = -1)
    {
        string saveKey = saveSlot >= 0 ? $"{SAVE_SLOT_PREFIX}{saveSlot}" : AUTO_SAVE_PREFIX;

        if (!PlayerPrefs.HasKey(saveKey + "Data"))
        {
            return null;
        }

        string saveJson = PlayerPrefs.GetString(saveKey + "Data");
        PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(saveJson);

        return new SaveSlotInfo
        {
            slotNumber = saveSlot,
            saveTime = saveData.saveTime,
            currentSceneName = saveData.currentSceneName,
            playerLevel = Mathf.Max(1, (int)(saveData.maxHealth / 100)), // Aproximação baseada no HP
            playTime = saveData.playTime
        };
    }

    /// <summary>
    /// Deleta um save específico
    /// </summary>
    public void DeleteSaveData(int saveSlot)
    {
        if (saveSlot < 0 || saveSlot >= MAX_SAVE_SLOTS) return;

        string saveKey = $"{SAVE_SLOT_PREFIX}{saveSlot}";
        PlayerPrefs.DeleteKey(saveKey + "Data");
        PlayerPrefs.DeleteKey(saveKey + "Timestamp");
        PlayerPrefs.Save();

        if (enableDebugLogs)
        {
            Debug.Log($"[SavePlayerManager] Save do Slot {saveSlot} deletado");
        }
    }

    /// <summary>
    /// Força um salvamento imediato (útil antes de fechar o jogo)
    /// </summary>
    public void ForceSave()
    {
        AutoSavePlayerData();
    }
    #endregion

    #region Data Structures
    /// <summary>
    /// Informações sobre um slot de save
    /// </summary>
    [System.Serializable]
    public class SaveSlotInfo
    {
        public int slotNumber;
        public DateTime saveTime;
        public string currentSceneName;
        public int playerLevel;
        public float playTime;
    }
    #endregion
}
