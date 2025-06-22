using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Collections;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Sistema de gerenciamento de localização.
    /// Implementa a funcionalidade de carregamento de arquivos CSV de texto localizado.
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        #region Singleton
        private static LocalizationManager _instance;
        public static LocalizationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("LocalizationManager");
                    _instance = go.AddComponent<LocalizationManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLocalization();
        }
        #endregion

        #region Variables
        [Header("Settings")]
        [SerializeField] private string _defaultLanguage = "EN";
        [SerializeField] private bool _autoDetectLanguage = true;

        [Header("File Path")]
        [SerializeField] private string _csvFilePath = "Localization/localization.csv";

        // Lista de idiomas suportados
        private readonly string[] _supportedLanguages = { "EN", "PT_BR", "ES", "FR", "DE", "JA", "ZH_CN" };

        // Dicionário com todos os textos localizados
        private Dictionary<string, Dictionary<string, string>> _localizedTexts;

        // Idioma atual
        private string _currentLanguage;

        // Configuração do usuário
        private UserConfig _userConfig;

        // Caminho absoluto para o arquivo de configuração
        private string _configPath;
        #endregion

        #region Initialization
        private void InitializeLocalization()
        {
            // Inicializa o dicionário
            _localizedTexts = new Dictionary<string, Dictionary<string, string>>();

            // Define caminho do arquivo de configuração
            _configPath = Path.Combine(Application.persistentDataPath, "config.json");

            // Inicializa a configuração do usuário
            LoadUserConfig();

            // Configura o idioma
            SetupLanguage();

            // Carrega os arquivos de localização
            StartCoroutine(LoadLocalizationFiles());
        }

        private void LoadUserConfig()
        {
            if (File.Exists(_configPath))
            {
                string json = File.ReadAllText(_configPath);
                _userConfig = JsonUtility.FromJson<UserConfig>(json);
            }
            else
            {
                _userConfig = new UserConfig();
                SaveUserConfig();
            }
        }

        private void SaveUserConfig()
        {
            string json = JsonUtility.ToJson(_userConfig, true);
            File.WriteAllText(_configPath, json);
        }

        private void SetupLanguage()
        {
            if (!string.IsNullOrEmpty(_userConfig.preferredLanguage))
            {
                // 1. Configuração do Usuário
                _currentLanguage = _userConfig.preferredLanguage;
            }
            else if (_autoDetectLanguage)
            {
                // 2. Idioma do Sistema
                _currentLanguage = DetectSystemLanguage();
            }
            else
            {
                // Fallback Global
                _currentLanguage = _defaultLanguage;
            }

            Debug.Log($"[LocalizationManager] Language set to: {_currentLanguage}");
        }
        #endregion

        #region Language Detection
        /// <summary>
        /// Detecta o idioma do sistema e mapeia para um idioma suportado
        /// </summary>
        public string DetectSystemLanguage()
        {
            // Obtém o código de idioma do sistema
            string systemLanguage = CultureInfo.CurrentCulture.Name;

            // Tenta encontrar um idioma correspondente
            foreach (string language in _supportedLanguages)
            {
                if (systemLanguage.StartsWith(language, StringComparison.OrdinalIgnoreCase) ||
                    systemLanguage.Replace("-", "_").StartsWith(language, StringComparison.OrdinalIgnoreCase))
                {
                    return language;
                }
            }

            // 3. Fallback Regional
            foreach (string language in _supportedLanguages)
            {
                string region = systemLanguage.Split('-')[0];
                if (language.StartsWith(region, StringComparison.OrdinalIgnoreCase))
                {
                    return language;
                }
            }

            // 4. Fallback Global
            return _defaultLanguage;
        }
        #endregion

        #region CSV Loading
        private IEnumerator LoadLocalizationFiles()
        {
            // Monta o caminho do arquivo CSV
            string filePath = Path.Combine(Application.streamingAssetsPath, _csvFilePath);

            UnityWebRequest www;

            // Verifica se é uma URL ou caminho local
            if (filePath.Contains("://"))
            {
                www = UnityWebRequest.Get(filePath);
            }
            else
            {
                www = UnityWebRequest.Get(new Uri(filePath).AbsoluteUri);
            }

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[LocalizationManager] Error loading localization file: {www.error}");
            }
            else
            {
                try
                {
                    string csv = www.downloadHandler.text;
                    ParseCSV(csv);
                    Debug.Log("[LocalizationManager] Localization file loaded successfully");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[LocalizationManager] Error parsing CSV: {e.Message}");
                }
            }
        }

        private void ParseCSV(string csvContent)
        {
            // Limpa o dicionário atual
            _localizedTexts.Clear();

            using (var reader = new StringReader(csvContent))
            {
                // Lê a primeira linha (cabeçalho)
                string headerLine = reader.ReadLine();
                if (headerLine == null)
                {
                    Debug.LogError("[LocalizationManager] CSV file is empty or invalid");
                    return;
                }

                // Divide o cabeçalho em colunas
                string[] headers = headerLine.Split(',');

                // Verifica se há cabeçalhos suficientes
                if (headers.Length < 2)
                {
                    Debug.LogError("[LocalizationManager] CSV header is invalid, needs at least Key and one language");
                    return;
                }

                // Inicializa dicionários para cada idioma
                for (int i = 1; i < headers.Length; i++)
                {
                    string lang = headers[i].Trim();
                    _localizedTexts[lang] = new Dictionary<string, string>();
                }

                // Processa cada linha do CSV
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Divide a linha em valores
                    string[] values = ParseCSVLine(line);

                    // Verifica se há valores suficientes
                    if (values.Length < 2) continue;

                    // A primeira coluna é a chave
                    string key = values[0].Trim();

                    // As outras colunas são os valores para cada idioma
                    for (int i = 1; i < values.Length && i < headers.Length; i++)
                    {
                        string lang = headers[i].Trim();
                        string text = values[i].Trim();

                        // Adiciona a entrada ao dicionário
                        _localizedTexts[lang][key] = text;
                    }
                }
            }
        }

        private string[] ParseCSVLine(string line)
        {
            List<string> result = new List<string>();
            StringBuilder currentValue = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                // Se encontrou aspas, inverte o estado
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                // Se encontrou vírgula e não está dentro de aspas, finaliza o valor atual
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentValue.ToString());
                    currentValue.Clear();
                }
                // Caso contrário, adiciona o caractere ao valor atual
                else
                {
                    currentValue.Append(c);
                }
            }

            // Adiciona o último valor
            result.Add(currentValue.ToString());

            return result.ToArray();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Obtém o texto localizado para a chave especificada no idioma atual
        /// </summary>
        /// <param name="key">Chave do texto</param>
        /// <returns>Texto localizado ou chave se não encontrado</returns>
        public string GetLocalizedText(string key)
        {
            // Verifica se o idioma atual existe no dicionário
            if (_localizedTexts == null || !_localizedTexts.ContainsKey(_currentLanguage))
            {
                // Tenta usar o idioma padrão
                if (_localizedTexts != null && _localizedTexts.ContainsKey(_defaultLanguage) &&
                    _localizedTexts[_defaultLanguage].ContainsKey(key))
                {
                    return _localizedTexts[_defaultLanguage][key];
                }

                // Se não encontrar, retorna a chave
                return key;
            }

            // Verifica se a chave existe para o idioma atual
            if (_localizedTexts[_currentLanguage].ContainsKey(key))
            {
                return _localizedTexts[_currentLanguage][key];
            }

            // Tenta usar o idioma padrão
            if (_localizedTexts.ContainsKey(_defaultLanguage) && _localizedTexts[_defaultLanguage].ContainsKey(key))
            {
                return _localizedTexts[_defaultLanguage][key];
            }

            // Se não encontrar, retorna a chave
            return key;
        }

        /// <summary>
        /// Define o idioma atual
        /// </summary>
        /// <param name="language">Código do idioma</param>
        public void SetLanguage(string language)
        {
            foreach (string supportedLang in _supportedLanguages)
            {
                if (supportedLang.Equals(language, StringComparison.OrdinalIgnoreCase))
                {
                    _currentLanguage = supportedLang;
                    _userConfig.preferredLanguage = supportedLang;
                    SaveUserConfig();
                    Debug.Log($"[LocalizationManager] Language changed to: {_currentLanguage}");
                    return;
                }
            }

            Debug.LogWarning($"[LocalizationManager] Language {language} not supported, using default");
            _currentLanguage = _defaultLanguage;
        }

        /// <summary>
        /// Obtém o idioma atual
        /// </summary>
        public string GetCurrentLanguage()
        {
            return _currentLanguage;
        }

        /// <summary>
        /// Obtém a lista de idiomas suportados
        /// </summary>
        public string[] GetSupportedLanguages()
        {
            return _supportedLanguages;
        }

        /// <summary>
        /// Recarrega os arquivos de localização
        /// </summary>
        public void ReloadLocalizationFiles()
        {
            StartCoroutine(LoadLocalizationFiles());
        }
        #endregion
    }

    [Serializable]
    public class UserConfig
    {
        public string preferredLanguage = "";
        public UserConfig() { }
    }
}
