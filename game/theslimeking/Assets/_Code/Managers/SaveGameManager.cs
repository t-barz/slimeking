using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using SlimeKing.Core;
using SlimeKing.Systems.SaveSystem;
using SlimeKing.Items;
using SlimeKing.Data;
using SlimeKing.Gameplay;
using System.Collections.Generic;

namespace SlimeKing.Core
{
    /// <summary>
    /// Gerencia o sistema de save/load do jogo.
    /// Salva automaticamente ao abrir inventário e carrega ao pressionar pause.
    /// </summary>
    public class SaveGameManager : ManagerSingleton<SaveGameManager>
    {
        #region Inspector Settings

        [Header("Save Settings")]
        [SerializeField] private string saveFileName = "savegame.json";
        [SerializeField] private bool useEncryption = false;
        [SerializeField] private bool prettyPrint = true;

        [Header("Auto Save Settings")]
        [SerializeField] private bool autoSaveOnInventoryOpen = true;
        [SerializeField] private bool autoLoadOnPauseOpen = false;

        [Header("Debug")]
        [SerializeField] private bool enableDetailedLogs = true;

        #endregion

        #region Properties

        private string SaveFilePath => Path.Combine(Application.persistentDataPath, saveFileName);
        private SaveGameData currentSaveData;
        private static SaveGameData pendingLoadData; // Static para persistir entre reloads de cena
        private PlayerInput playerInput;
        private InputAction inventoryAction;
        private InputAction pauseAction;
        private bool actionsSubscribed;

        #endregion

        #region Events

        public event Action<SaveGameData> OnGameSaved;
        public event Action<SaveGameData> OnGameLoaded;
        public event Action<string> OnSaveError;

        #endregion

        #region Initialization

        protected override void Initialize()
        {
            currentSaveData = new SaveGameData();
            
            if (enableDetailedLogs)
            {
                enableLogs = true;
            }
            
            Log("SaveGameManager initialized");
            
            // Verifica se há dados pendentes de aplicação (após reload de cena)
            if (pendingLoadData != null)
            {
                Log("Aplicando dados pendentes após reload de cena");
                currentSaveData = pendingLoadData;
                pendingLoadData = null;
                
                // Aguarda um frame para garantir que todos os managers foram inicializados
                StartCoroutine(ApplyDataAfterSceneLoad());
            }
            
            StartCoroutine(WaitForPlayerInputAndSubscribe());
        }

        private System.Collections.IEnumerator ApplyDataAfterSceneLoad()
        {
            // Aguarda 2 frames para garantir inicialização completa
            yield return null;
            yield return null;
            
            Log("Aplicando dados do save...");
            ApplyAllData();
            Log("Dados aplicados com sucesso!");
            OnGameLoaded?.Invoke(currentSaveData);
        }

        private System.Collections.IEnumerator ApplyDataAfterDelay()
        {
            // Aguarda 2 frames para garantir que managers estejam prontos
            yield return null;
            yield return null;
            
            Log("Aplicando dados do save (mesma cena)...");
            ApplyAllData();
            Log("Jogo carregado com sucesso!");
            OnGameLoaded?.Invoke(currentSaveData);
        }

        private System.Collections.IEnumerator WaitForPlayerInputAndSubscribe()
        {
            while (playerInput == null)
            {
                playerInput = FindObjectOfType<PlayerInput>();
                yield return null;
            }

            SetupInputActions();
        }

        private void SetupInputActions()
        {
            if (playerInput == null || playerInput.actions == null)
            {
                LogWarning("PlayerInput ou actions não encontrado");
                return;
            }

            inventoryAction = playerInput.actions.FindAction("Gameplay/Inventory", throwIfNotFound: false);
            pauseAction = playerInput.actions.FindAction("Gameplay/Pause", throwIfNotFound: false);

            if (inventoryAction == null)
            {
                LogWarning("Ação 'Inventory' não encontrada");
            }

            if (pauseAction == null)
            {
                LogWarning("Ação 'Pause' não encontrada");
            }

            SubscribeToInputActions();
        }

        private void SubscribeToInputActions()
        {
            if (actionsSubscribed) return;

            if (inventoryAction != null && autoSaveOnInventoryOpen)
            {
                inventoryAction.performed += OnInventoryPressed;
            }

            if (pauseAction != null && autoLoadOnPauseOpen)
            {
                pauseAction.performed += OnPausePressed;
            }

            actionsSubscribed = true;
            Log("Input actions subscribed");
        }

        private void UnsubscribeFromInputActions()
        {
            if (!actionsSubscribed) return;

            if (inventoryAction != null)
            {
                inventoryAction.performed -= OnInventoryPressed;
            }

            if (pauseAction != null)
            {
                pauseAction.performed -= OnPausePressed;
            }

            actionsSubscribed = false;
        }

        #endregion

        #region Input Callbacks

        private void OnInventoryPressed(InputAction.CallbackContext context)
        {
            if (autoSaveOnInventoryOpen)
            {
                Log("Inventário aberto - salvando jogo");
                SaveGame();
            }
        }

        private void OnPausePressed(InputAction.CallbackContext context)
        {
            if (autoLoadOnPauseOpen)
            {
                Log("Pause pressionado - carregando jogo");
                LoadGame();
            }
        }

        #endregion

        #region Save Game

        public bool SaveGame()
        {
            try
            {
                Log("Iniciando salvamento...");

                currentSaveData = new SaveGameData
                {
                    saveTimestamp = DateTime.Now,
                    currentSceneName = SceneManager.GetActiveScene().name
                };

                CollectPlayerData();
                CollectWorldData();
                CollectSceneData();
                CollectNPCData();
                CollectQuestData();
                CollectGameFlags();

                string json = JsonUtility.ToJson(currentSaveData, prettyPrint);

                if (useEncryption)
                {
                    json = EncryptData(json);
                }

                File.WriteAllText(SaveFilePath, json);

                Log($"Jogo salvo: {SaveFilePath}");
                OnGameSaved?.Invoke(currentSaveData);
                return true;
            }
            catch (Exception e)
            {
                LogError($"Erro ao salvar: {e.Message}");
                OnSaveError?.Invoke(e.Message);
                return false;
            }
        }

        private void CollectPlayerData()
        {
            var playerData = new PlayerSaveData();

            var playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerData.currentScene = SceneManager.GetActiveScene().name;
                playerData.position = playerController.transform.position;
            }

            if (InventoryManager.HasInstance)
            {
                var inventory = InventoryManager.Instance.GetAllItems();
                foreach (var item in inventory)
                {
                    if (item.Key != null)
                    {
                        playerData.inventory.Add(new InventoryItemSaveData(
                            item.Key.name,
                            item.Value,
                            100f
                        ));
                    }
                }
            }

            if (GameManager.HasInstance)
            {
                var crystalCounts = GameManager.Instance.GetAllCrystalCounts();
                foreach (var crystal in crystalCounts)
                {
                    playerData.crystalCounts[crystal.Key.ToString()] = crystal.Value;
                }
            }

            currentSaveData.playerData = playerData;
            Log($"Player data: {playerData.inventory.Count} itens");
        }

        private void CollectWorldData()
        {
            var worldData = new WorldSaveData
            {
                currentDay = 1,
                currentSeason = "Spring",
                currentHour = 8,
                currentMinute = 0,
                totalPlayTime = Time.time
            };

            currentSaveData.worldData = worldData;
        }

        private void CollectSceneData()
        {
            currentSaveData.scenesData.Clear();
            string currentScene = SceneManager.GetActiveScene().name;
            var sceneData = new SceneSaveData(currentScene);
            currentSaveData.scenesData.Add(sceneData);
        }

        private void CollectNPCData()
        {
            currentSaveData.npcsData.Clear();
        }

        private void CollectQuestData()
        {
            currentSaveData.questsData.Clear();
        }

        private void CollectGameFlags()
        {
            currentSaveData.gameFlagsData = new GameFlagsSaveData();
        }

        #endregion

        #region Load Game

        public bool LoadGame()
        {
            try
            {
                if (!File.Exists(SaveFilePath))
                {
                    LogWarning("Save não encontrado");
                    return false;
                }

                Log("Carregando jogo...");

                string json = File.ReadAllText(SaveFilePath);

                if (useEncryption)
                {
                    json = DecryptData(json);
                }

                currentSaveData = JsonUtility.FromJson<SaveGameData>(json);

                if (currentSaveData == null)
                {
                    LogError("Falha ao deserializar");
                    return false;
                }

                // Verifica se precisa trocar de cena
                string savedScene = currentSaveData.currentSceneName;
                string currentScene = SceneManager.GetActiveScene().name;

                if (savedScene != currentScene)
                {
                    // Salva os dados para aplicar após reload
                    pendingLoadData = currentSaveData;
                    
                    // Precisa carregar a cena salva primeiro
                    Log($"Carregando cena salva: {savedScene}");
                    SceneManager.LoadScene(savedScene, LoadSceneMode.Single);
                    
                    // Retorna true mas os dados serão aplicados no Initialize após reload
                    return true;
                }
                else
                {
                    // Mesma cena, aguarda managers estarem prontos
                    Log("Mesma cena - aguardando managers...");
                    StartCoroutine(ApplyDataAfterDelay());
                }

                return true;
            }
            catch (Exception e)
            {
                LogError($"Erro ao carregar: {e.Message}");
                OnSaveError?.Invoke(e.Message);
                return false;
            }
        }

        private void ApplyAllData()
        {
            ApplyPlayerData();
            ApplyWorldData();
            ApplySceneData();
            ApplyNPCData();
            ApplyQuestData();
            ApplyGameFlags();
        }

        private void ApplyPlayerData()
        {
            if (currentSaveData.playerData == null)
            {
                LogWarning("PlayerData é null");
                return;
            }

            var playerData = currentSaveData.playerData;
            Log($"Aplicando PlayerData: {playerData.inventory.Count} itens, {playerData.crystalCounts.Count} tipos de cristais");

            var playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.transform.position = playerData.position;
                Log($"Posição do player restaurada: {playerData.position}");
            }
            else
            {
                LogWarning("PlayerController não encontrado");
            }

            if (InventoryManager.HasInstance)
            {
                InventoryManager.Instance.Clear();
                
                foreach (var item in playerData.inventory)
                {
                    ItemData itemData = Resources.Load<ItemData>($"Items/{item.itemID}");
                    if (itemData != null)
                    {
                        InventoryManager.Instance.AddItem(itemData, item.quantity);
                        Log($"Item restaurado: {item.itemID} x{item.quantity}");
                    }
                    else
                    {
                        LogWarning($"Item não encontrado: {item.itemID}");
                    }
                }
            }
            else
            {
                LogWarning("InventoryManager não encontrado");
            }

            // Restaura cristais - LIMPA TODOS ANTES
            if (GameManager.HasInstance && playerData.crystalCounts != null)
            {
                Log("Restaurando cristais...");
                
                // Primeiro, zera todos os cristais
                foreach (CrystalType crystalType in System.Enum.GetValues(typeof(CrystalType)))
                {
                    int currentCount = GameManager.Instance.GetCrystalCount(crystalType);
                    if (currentCount > 0)
                    {
                        GameManager.Instance.RemoveCrystal(crystalType, currentCount);
                    }
                }

                // Depois, adiciona os valores salvos
                foreach (var crystal in playerData.crystalCounts)
                {
                    if (Enum.TryParse<CrystalType>(crystal.Key, out CrystalType crystalType))
                    {
                        if (crystal.Value > 0)
                        {
                            GameManager.Instance.AddCrystal(crystalType, crystal.Value);
                            Log($"Cristal restaurado: {crystalType} x{crystal.Value}");
                        }
                    }
                }
            }
            else
            {
                LogWarning("GameManager não encontrado ou crystalCounts é null");
            }

            Log($"Player data aplicado: {playerData.inventory.Count} itens");
        }

        private void ApplyWorldData() { }
        private void ApplySceneData() { }
        private void ApplyNPCData() { }
        private void ApplyQuestData() { }
        private void ApplyGameFlags() { }

        #endregion

        #region Utility Methods

        public bool HasSaveGame()
        {
            return File.Exists(SaveFilePath);
        }

        public bool DeleteSaveGame()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath);
                    Log("Save deletado");
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                LogError($"Erro ao deletar: {e.Message}");
                return false;
            }
        }

        public SaveGameInfo GetSaveInfo()
        {
            if (!HasSaveGame()) return null;

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                if (useEncryption) json = DecryptData(json);

                var saveData = JsonUtility.FromJson<SaveGameData>(json);
                return new SaveGameInfo
                {
                    saveDate = saveData.saveTimestamp,
                    sceneName = saveData.currentSceneName,
                    playTime = saveData.worldData.totalPlayTime
                };
            }
            catch
            {
                return null;
            }
        }

        private string EncryptData(string data)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            return Convert.ToBase64String(bytes);
        }

        private string DecryptData(string data)
        {
            byte[] bytes = Convert.FromBase64String(data);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        #endregion

        #region Lifecycle

        protected override void OnManagerDestroy()
        {
            UnsubscribeFromInputActions();
            base.OnManagerDestroy();
        }

        #endregion
    }

    [Serializable]
    public class SaveGameInfo
    {
        public DateTime saveDate;
        public string sceneName;
        public float playTime;
    }
}
