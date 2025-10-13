using System;
using System.IO;
using UnityEngine;

namespace SlimeKing.Core
{
    /// <summary>
    /// Manager para persistência de dados do jogador e configurações.
    /// Sistema simples usando JSON e PlayerPrefs.
    /// </summary>
    public class SaveManager : ManagerBase<SaveManager>
    {
        [Header("Save Settings")]
        [SerializeField] private bool autoSaveEnabled = true;
        [SerializeField] private float autoSaveInterval = 300f; // 5 minutos
        [SerializeField] private bool encryptSaveData = false; // Simplificado - sem criptografia
        
        // Dados atuais
        private GameData currentGameData;
        private GameSettings currentSettings;
        
        // Paths dos arquivos
        private string saveFilePath;
        private string settingsFilePath;
        
        // Properties
        public GameData CurrentGameData => currentGameData;
        public GameSettings CurrentSettings => currentSettings;
        public bool HasSaveFile => File.Exists(saveFilePath);
        
        protected override void Initialize()
        {
            SetupFilePaths();
            LoadSettings();
            LoadGameData();
            
            if (autoSaveEnabled)
            {
                InvokeRepeating(nameof(AutoSave), autoSaveInterval, autoSaveInterval);
            }
            
            Log("SaveManager initialized");
        }
        
        #region Setup
        
        /// <summary>
        /// Configura os caminhos dos arquivos
        /// </summary>
        private void SetupFilePaths()
        {
            string persistentPath = Application.persistentDataPath;
            saveFilePath = Path.Combine(persistentPath, "savegame.json");
            settingsFilePath = Path.Combine(persistentPath, "settings.json");
            
            Log($"Save file path: {saveFilePath}");
            Log($"Settings file path: {settingsFilePath}");
        }
        
        #endregion
        
        #region Game Data
        
        /// <summary>
        /// Salva os dados do jogo
        /// </summary>
        public bool SaveGameData()
        {
            try
            {
                if (currentGameData == null)
                {
                    currentGameData = new GameData();
                }
                
                currentGameData.UpdateSaveTime();
                string jsonData = JsonUtility.ToJson(currentGameData, true);
                File.WriteAllText(saveFilePath, jsonData);
                
                Log("Game data saved successfully");
                return true;
            }
            catch (Exception e)
            {
                LogError($"Failed to save game data: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Carrega os dados do jogo
        /// </summary>
        public bool LoadGameData()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    string jsonData = File.ReadAllText(saveFilePath);
                    currentGameData = JsonUtility.FromJson<GameData>(jsonData);
                    
                    Log($"Game data loaded successfully - Level: {currentGameData.playerLevel}");
                    return true;
                }
                else
                {
                    Log("No save file found, creating new game data");
                    currentGameData = new GameData();
                    return false;
                }
            }
            catch (Exception e)
            {
                LogError($"Failed to load game data: {e.Message}");
                currentGameData = new GameData();
                return false;
            }
        }
        
        /// <summary>
        /// Cria novos dados de jogo
        /// </summary>
        public void CreateNewGameData()
        {
            currentGameData = new GameData();
            SaveGameData();
            Log("New game data created");
        }
        
        /// <summary>
        /// Deleta o arquivo de save
        /// </summary>
        public bool DeleteSaveFile()
        {
            try
            {
                if (File.Exists(saveFilePath))
                {
                    File.Delete(saveFilePath);
                    currentGameData = new GameData();
                    Log("Save file deleted");
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                LogError($"Failed to delete save file: {e.Message}");
                return false;
            }
        }
        
        #endregion
        
        #region Settings
        
        /// <summary>
        /// Salva as configurações
        /// </summary>
        public bool SaveSettings()
        {
            try
            {
                if (currentSettings == null)
                {
                    currentSettings = new GameSettings();
                }
                
                string jsonData = JsonUtility.ToJson(currentSettings, true);
                File.WriteAllText(settingsFilePath, jsonData);
                
                // Também salva no PlayerPrefs como backup
                SaveSettingsToPlayerPrefs();
                
                Log("Settings saved successfully");
                return true;
            }
            catch (Exception e)
            {
                LogError($"Failed to save settings: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Carrega as configurações
        /// </summary>
        public bool LoadSettings()
        {
            try
            {
                if (File.Exists(settingsFilePath))
                {
                    string jsonData = File.ReadAllText(settingsFilePath);
                    currentSettings = JsonUtility.FromJson<GameSettings>(jsonData);
                    
                    Log("Settings loaded successfully");
                    return true;
                }
                else
                {
                    Log("No settings file found, creating default settings");
                    currentSettings = new GameSettings();
                    SaveSettings();
                    return false;
                }
            }
            catch (Exception e)
            {
                LogError($"Failed to load settings: {e.Message}");
                
                // Fallback para PlayerPrefs
                LoadSettingsFromPlayerPrefs();
                return false;
            }
        }
        
        /// <summary>
        /// Salva configurações no PlayerPrefs (backup)
        /// </summary>
        private void SaveSettingsToPlayerPrefs()
        {
            PlayerPrefs.SetFloat("Settings_MasterVolume", currentSettings.masterVolume);
            PlayerPrefs.SetFloat("Settings_MusicVolume", currentSettings.musicVolume);
            PlayerPrefs.SetFloat("Settings_SFXVolume", currentSettings.sfxVolume);
            PlayerPrefs.SetInt("Settings_QualityLevel", currentSettings.qualityLevel);
            PlayerPrefs.SetInt("Settings_Fullscreen", currentSettings.fullscreen ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// Carrega configurações do PlayerPrefs (fallback)
        /// </summary>
        private void LoadSettingsFromPlayerPrefs()
        {
            currentSettings = new GameSettings();
            currentSettings.masterVolume = PlayerPrefs.GetFloat("Settings_MasterVolume", 1f);
            currentSettings.musicVolume = PlayerPrefs.GetFloat("Settings_MusicVolume", 0.8f);
            currentSettings.sfxVolume = PlayerPrefs.GetFloat("Settings_SFXVolume", 1f);
            currentSettings.qualityLevel = PlayerPrefs.GetInt("Settings_QualityLevel", 2);
            currentSettings.fullscreen = PlayerPrefs.GetInt("Settings_Fullscreen", 1) == 1;
        }
        
        #endregion
        
        #region Player Progress Helpers
        
        /// <summary>
        /// Adiciona experiência ao jogador
        /// </summary>
        public void AddExperience(float exp)
        {
            if (currentGameData != null)
            {
                currentGameData.playerExp += exp;
                Log($"Added {exp} experience. Total: {currentGameData.playerExp}");
            }
        }
        
        /// <summary>
        /// Define o nível do jogador
        /// </summary>
        public void SetPlayerLevel(int level)
        {
            if (currentGameData != null)
            {
                currentGameData.playerLevel = Mathf.Max(1, level);
                Log($"Player level set to {currentGameData.playerLevel}");
            }
        }
        
        /// <summary>
        /// Atualiza o high score
        /// </summary>
        public void UpdateHighScore(int score)
        {
            if (currentGameData != null && score > currentGameData.highScore)
            {
                currentGameData.highScore = score;
                Log($"New high score: {score}");
            }
        }
        
        /// <summary>
        /// Adiciona uma conquista
        /// </summary>
        public void UnlockAchievement(string achievementId)
        {
            if (currentGameData != null && !currentGameData.unlockedAchievements.Contains(achievementId))
            {
                currentGameData.unlockedAchievements.Add(achievementId);
                Log($"Achievement unlocked: {achievementId}");
            }
        }
        
        /// <summary>
        /// Verifica se uma conquista foi desbloqueada
        /// </summary>
        public bool IsAchievementUnlocked(string achievementId)
        {
            return currentGameData?.unlockedAchievements.Contains(achievementId) ?? false;
        }
        
        /// <summary>
        /// Adiciona tempo de jogo
        /// </summary>
        public void AddPlayTime(float time)
        {
            if (currentGameData != null)
            {
                currentGameData.totalPlayTime += time;
            }
        }
        
        #endregion
        
        #region Auto Save
        
        /// <summary>
        /// Auto save automático
        /// </summary>
        private void AutoSave()
        {
            if (autoSaveEnabled && currentGameData != null)
            {
                SaveGameData();
                Log("Auto save completed");
            }
        }
        
        /// <summary>
        /// Habilita/desabilita auto save
        /// </summary>
        public void SetAutoSave(bool enabled)
        {
            autoSaveEnabled = enabled;
            
            if (enabled)
            {
                CancelInvoke(nameof(AutoSave));
                InvokeRepeating(nameof(AutoSave), autoSaveInterval, autoSaveInterval);
            }
            else
            {
                CancelInvoke(nameof(AutoSave));
            }
            
            Log($"Auto save {(enabled ? "enabled" : "disabled")}");
        }
        
        #endregion
        
        #region Unity Events
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveGameData();
                SaveSettings();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                SaveGameData();
                SaveSettings();
            }
        }
        
        private void OnDestroy()
        {
            SaveGameData();
            SaveSettings();
        }
        
        #endregion
    }
}