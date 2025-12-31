using System.Collections.Generic;
using System.IO;
using System.Linq;
using SlimeKing.Core;
using SlimeKing.Gameplay;
using UnityEngine;

namespace SlimeKing.Systems
{
    /// <summary>
    /// Gerencia o carregamento e acesso a textos localizados de diálogos.
    /// Carrega arquivos JSON do diretório de diálogos e mantém cache em memória.
    /// </summary>
    public class LocalizationManager : ManagerSingleton<LocalizationManager>
    {
        #region Inspector Settings

        [Header("Localization Settings")]
        [SerializeField] private LanguageCode currentLanguage = LanguageCode.EN;
        
        [SerializeField] private string dialoguesPath = "Assets/Data/Dialogues/";
        
        [Tooltip("Se true, carrega todos os diálogos na inicialização. Se false, carrega sob demanda.")]
        [SerializeField] private bool preloadAllDialogues = false;

        #endregion

        #region Private Fields

        /// <summary>
        /// Cache de diálogos carregados. Key = dialogueId, Value = LocalizedDialogueData
        /// </summary>
        private Dictionary<string, LocalizedDialogueData> dialogueCache = new Dictionary<string, LocalizedDialogueData>();

        /// <summary>
        /// Mensagem padrão exibida quando um diálogo não é encontrado
        /// </summary>
        private const string DIALOGUE_NOT_FOUND_MESSAGE = "[Dialogue not found]";

        #endregion

        #region Initialization

        protected override void Initialize()
        {
            Log($"Initializing LocalizationManager with language: {currentLanguage}");
            Log($"Dialogues path: {dialoguesPath}");

            // Valida o caminho de diálogos
            if (!Directory.Exists(dialoguesPath))
            {
                LogWarning($"Dialogues directory not found: {dialoguesPath}. Creating directory...");
                try
                {
                    Directory.CreateDirectory(dialoguesPath);
                    Log($"Created dialogues directory: {dialoguesPath}");
                }
                catch (System.Exception ex)
                {
                    LogError($"Failed to create dialogues directory: {ex.Message}");
                }
            }

            // Pré-carrega todos os diálogos se configurado
            if (preloadAllDialogues)
            {
                LoadAllDialogues();
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Obtém os dados de diálogo localizados para o ID especificado.
        /// Usa o idioma atual configurado com fallback automático.
        /// </summary>
        /// <param name="dialogueId">ID único do diálogo</param>
        /// <returns>LocalizedDialogueData ou null se não encontrado</returns>
        public LocalizedDialogueData GetLocalizedDialogue(string dialogueId)
        {
            if (string.IsNullOrEmpty(dialogueId))
            {
                LogError("GetLocalizedDialogue called with null or empty dialogueId");
                return CreateErrorDialogue(dialogueId);
            }

            // Verifica se está no cache
            if (dialogueCache.ContainsKey(dialogueId))
            {
                Log($"Dialogue '{dialogueId}' found in cache");
                return dialogueCache[dialogueId];
            }

            // Tenta carregar do arquivo JSON
            Log($"Dialogue '{dialogueId}' not in cache. Loading from file...");
            LocalizedDialogueData dialogueData = LoadDialogueFromFile(dialogueId);

            if (dialogueData != null)
            {
                // Adiciona ao cache
                dialogueCache[dialogueId] = dialogueData;
                Log($"Dialogue '{dialogueId}' loaded and cached successfully");
                return dialogueData;
            }

            // Não encontrado - retorna diálogo de erro
            LogError($"Dialogue '{dialogueId}' not found in path: {dialoguesPath}");
            return CreateErrorDialogue(dialogueId);
        }

        /// <summary>
        /// Define o idioma atual do sistema de localização.
        /// </summary>
        /// <param name="language">Código do idioma a ser definido</param>
        public void SetLanguage(LanguageCode language)
        {
            if (currentLanguage != language)
            {
                Log($"Changing language from {currentLanguage} to {language}");
                currentLanguage = language;
            }
        }

        /// <summary>
        /// Obtém o idioma atual configurado.
        /// </summary>
        /// <returns>Código do idioma atual</returns>
        public LanguageCode GetCurrentLanguage()
        {
            return currentLanguage;
        }

        /// <summary>
        /// Obtém o código do idioma atual como string.
        /// </summary>
        /// <returns>String do código de idioma (ex: "EN", "BR")</returns>
        public string GetCurrentLanguageCode()
        {
            return currentLanguage.ToString();
        }

        /// <summary>
        /// Recarrega todos os diálogos do disco, limpando o cache.
        /// Útil para desenvolvimento ou quando arquivos JSON são modificados em runtime.
        /// </summary>
        public void ReloadDialogues()
        {
            Log("Reloading all dialogues...");
            dialogueCache.Clear();

            if (preloadAllDialogues)
            {
                LoadAllDialogues();
            }

            Log("Dialogues reloaded successfully");
        }

        /// <summary>
        /// Limpa o cache de diálogos, liberando memória.
        /// </summary>
        public void ClearCache()
        {
            Log($"Clearing dialogue cache ({dialogueCache.Count} entries)");
            dialogueCache.Clear();
        }

        /// <summary>
        /// Verifica se um diálogo específico está carregado no cache.
        /// </summary>
        /// <param name="dialogueId">ID do diálogo</param>
        /// <returns>True se o diálogo está no cache</returns>
        public bool IsDialogueCached(string dialogueId)
        {
            return dialogueCache.ContainsKey(dialogueId);
        }

        /// <summary>
        /// Obtém o número de diálogos atualmente em cache.
        /// </summary>
        /// <returns>Número de diálogos em cache</returns>
        public int GetCachedDialogueCount()
        {
            return dialogueCache.Count;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Carrega um diálogo específico de um arquivo JSON.
        /// </summary>
        /// <param name="dialogueId">ID do diálogo a carregar</param>
        /// <returns>LocalizedDialogueData ou null se falhar</returns>
        private LocalizedDialogueData LoadDialogueFromFile(string dialogueId)
        {
            string filePath = Path.Combine(dialoguesPath, $"{dialogueId}.json");

            // Verifica se o arquivo existe
            if (!File.Exists(filePath))
            {
                LogWarning($"Dialogue file not found: {filePath}");
                return null;
            }

            try
            {
                // Lê o conteúdo do arquivo
                string jsonContent = File.ReadAllText(filePath);

                if (string.IsNullOrEmpty(jsonContent))
                {
                    LogError($"Dialogue file is empty: {filePath}");
                    return null;
                }

                // Parse do JSON usando JsonUtility
                LocalizedDialogueData dialogueData = ScriptableObject.CreateInstance<LocalizedDialogueData>();
                JsonUtility.FromJsonOverwrite(jsonContent, dialogueData);

                // Valida os dados carregados
                if (string.IsNullOrEmpty(dialogueData.dialogueId))
                {
                    LogWarning($"Dialogue loaded from '{filePath}' has no dialogueId. Using filename as ID.");
                    dialogueData.dialogueId = dialogueId;
                }

                if (dialogueData.localizations == null || dialogueData.localizations.Count == 0)
                {
                    LogError($"Dialogue '{dialogueId}' has no localizations");
                    return null;
                }

                Log($"Successfully loaded dialogue '{dialogueId}' from file");
                return dialogueData;
            }
            catch (System.Exception ex)
            {
                LogError($"Failed to parse JSON file '{filePath}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Carrega todos os arquivos JSON do diretório de diálogos.
        /// </summary>
        private void LoadAllDialogues()
        {
            if (!Directory.Exists(dialoguesPath))
            {
                LogWarning($"Cannot preload dialogues. Directory not found: {dialoguesPath}");
                return;
            }

            try
            {
                string[] jsonFiles = Directory.GetFiles(dialoguesPath, "*.json");
                Log($"Found {jsonFiles.Length} dialogue files to preload");

                int successCount = 0;
                foreach (string filePath in jsonFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    LocalizedDialogueData dialogueData = LoadDialogueFromFile(fileName);

                    if (dialogueData != null)
                    {
                        dialogueCache[fileName] = dialogueData;
                        successCount++;
                    }
                }

                Log($"Preloaded {successCount} out of {jsonFiles.Length} dialogues successfully");
            }
            catch (System.Exception ex)
            {
                LogError($"Failed to preload dialogues: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria um diálogo de erro padrão quando um diálogo não é encontrado.
        /// </summary>
        /// <param name="dialogueId">ID do diálogo que não foi encontrado</param>
        /// <returns>LocalizedDialogueData com mensagem de erro</returns>
        private LocalizedDialogueData CreateErrorDialogue(string dialogueId)
        {
            LocalizedDialogueData errorDialogue = ScriptableObject.CreateInstance<LocalizedDialogueData>();
            errorDialogue.dialogueId = dialogueId ?? "unknown";
            errorDialogue.shortDescription = "Error dialogue";

            // Cria uma localização de erro para todos os idiomas
            LocalizedText errorText = new LocalizedText
            {
                language = "EN",
                pages = new List<DialoguePage>
                {
                    new DialoguePage { text = DIALOGUE_NOT_FOUND_MESSAGE }
                }
            };

            errorDialogue.localizations = new List<LocalizedText> { errorText };

            return errorDialogue;
        }

        #endregion

        #region Cleanup

        protected override void OnManagerDestroy()
        {
            // Limpa o cache ao destruir o manager
            ClearCache();
        }

        #endregion
    }
}
