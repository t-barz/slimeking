#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TheSlimeKing.Editor
{
    public class LocalizationEditor : EditorWindow
    {
        // Constantes
        private const string CSV_PATH = "Assets/StreamingAssets/Localization/localization.csv";

        // Variáveis de estado da janela
        private Vector2 scrollPosition;
        private List<LocalizationEntry> entries = new List<LocalizationEntry>();
        private Dictionary<string, int> headerIndexMap = new Dictionary<string, int>();
        private string[] headers;

        // Variáveis para adicionar novas entradas
        private string newEntryKey = "";
        private string[] newEntryValues;
        private bool showAddEntryPanel = false;
        private string searchFilter = "";
        private bool groupByPrefix = false;

        // Variáveis para filtragem
        private string[] prefixFilters = { "all", "ui_", "dialog_", "desc_", "msg_" };
        private int selectedPrefixFilter = 0;

        // Variáveis para edição
        private int editingEntryIndex = -1;
        private string editingKey = "";
        private string[] editingValues;

        // Mensagens de status
        private string statusMessage = "";
        private float statusMessageTime = 0f;
        private bool showStatusMessage = false;

        // Adiciona item de menu para abrir a janela
        [MenuItem("Extras/The Slime King/Localização/Editor CSV")]
        public static void ShowWindow()
        {
            var window = GetWindow<LocalizationEditor>("Localization Editor");
            window.minSize = new Vector2(800, 600);
            window.LoadCsv();
        }

        // Adiciona item de menu para adicionar nova entrada rapidamente
        [MenuItem("Extras/The Slime King/Localização/Adicionar Nova Entrada")]
        public static void AddNewEntry()
        {
            var window = GetWindow<LocalizationEditor>("Nova Entrada Localização");
            window.minSize = new Vector2(600, 400);
            window.LoadCsv();
            window.showAddEntryPanel = true;
        }

        private void OnEnable()
        {
            LoadCsv();
        }

        private void LoadCsv()
        {
            entries.Clear();
            headerIndexMap.Clear();

            try
            {
                // Verifica se o arquivo existe
                if (!File.Exists(CSV_PATH))
                {
                    Debug.LogError("Arquivo CSV não encontrado: " + CSV_PATH);
                    return;
                }

                // Lê todas as linhas do arquivo
                string[] lines = File.ReadAllLines(CSV_PATH);

                if (lines.Length > 0)
                {
                    // Processa o cabeçalho
                    headers = ParseCsvLine(lines[0]);

                    // Mapeia os cabeçalhos para seus índices
                    for (int i = 0; i < headers.Length; i++)
                    {
                        headerIndexMap[headers[i]] = i;
                    }

                    // Inicializa arrays para novas entradas
                    newEntryValues = new string[headers.Length - 1];
                    editingValues = new string[headers.Length - 1];

                    // Processa as linhas de dados
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string line = lines[i].Trim();
                        if (string.IsNullOrEmpty(line)) continue;

                        string[] values = ParseCsvLine(line);
                        if (values.Length > 0)
                        {
                            LocalizationEntry entry = new LocalizationEntry
                            {
                                Key = values[0],
                                Values = new string[headers.Length - 1]
                            };

                            // Preenche os valores nas línguas
                            for (int j = 1; j < headers.Length && j < values.Length; j++)
                            {
                                entry.Values[j - 1] = values[j];
                            }

                            entries.Add(entry);
                        }
                    }

                    SetStatusMessage("Arquivo CSV carregado com sucesso!", true);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Erro ao carregar o arquivo CSV: " + ex.Message);
                SetStatusMessage("Erro ao carregar o arquivo CSV: " + ex.Message, false);
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);

            // Barra de ferramentas
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Recarregar", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                LoadCsv();
            }

            if (GUILayout.Button("Salvar", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                SaveCsv();
            }

            GUILayout.FlexibleSpace();

            // Filtro por prefixo
            EditorGUILayout.LabelField("Filtrar por:", EditorStyles.miniLabel, GUILayout.Width(60));
            selectedPrefixFilter = EditorGUILayout.Popup(selectedPrefixFilter, prefixFilters, GUILayout.Width(100));

            // Campo de pesquisa
            EditorGUILayout.LabelField("Buscar:", EditorStyles.miniLabel, GUILayout.Width(50));
            searchFilter = EditorGUILayout.TextField(searchFilter, GUILayout.Width(200));

            EditorGUILayout.EndHorizontal();

            // Botões de ação
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Adicionar Nova Entrada", GUILayout.Height(30)))
            {
                showAddEntryPanel = !showAddEntryPanel;
                editingEntryIndex = -1;  // Cancela qualquer edição em andamento
            }

            if (GUILayout.Button("Agrupar por Prefixo", GUILayout.Height(30)))
            {
                groupByPrefix = !groupByPrefix;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Painel para adicionar nova entrada
            if (showAddEntryPanel)
            {
                DrawAddEntryPanel();
            }

            // Mensagem de status
            if (showStatusMessage)
            {
                EditorGUILayout.HelpBox(statusMessage, statusMessage.Contains("sucesso") ? MessageType.Info : MessageType.Error);

                if (EditorApplication.timeSinceStartup - statusMessageTime > 5.0f)
                {
                    showStatusMessage = false;
                }
            }

            EditorGUILayout.Space(10);

            // Lista de entradas
            DrawEntriesList();
        }

        private void DrawAddEntryPanel()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Nova Entrada de Localização", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Prefixo:", GUILayout.Width(50));

            // Botões de prefixo
            if (GUILayout.Button("ui_", EditorStyles.miniButtonLeft, GUILayout.Width(40)))
            {
                if (!newEntryKey.StartsWith("ui_")) newEntryKey = "ui_" + newEntryKey;
            }

            if (GUILayout.Button("dialog_", EditorStyles.miniButtonMid, GUILayout.Width(60)))
            {
                if (!newEntryKey.StartsWith("dialog_")) newEntryKey = "dialog_" + newEntryKey;
            }

            if (GUILayout.Button("desc_", EditorStyles.miniButtonMid, GUILayout.Width(50)))
            {
                if (!newEntryKey.StartsWith("desc_")) newEntryKey = "desc_" + newEntryKey;
            }

            if (GUILayout.Button("msg_", EditorStyles.miniButtonRight, GUILayout.Width(50)))
            {
                if (!newEntryKey.StartsWith("msg_")) newEntryKey = "msg_" + newEntryKey;
            }

            EditorGUILayout.EndHorizontal();

            // Campo para a chave
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Chave:", GUILayout.Width(50));
            newEntryKey = EditorGUILayout.TextField(newEntryKey);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Campos para valores em diferentes idiomas
            for (int i = 1; i < headers.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(headers[i] + ":", GUILayout.Width(60));
                newEntryValues[i - 1] = EditorGUILayout.TextField(newEntryValues[i - 1]);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(10);

            // Botões
            EditorGUILayout.BeginHorizontal();

            // Verificação se a key está em branco ou se já existe
            bool keyIsValid = !string.IsNullOrEmpty(newEntryKey) &&
                             !entries.Any(e => e.Key.Equals(newEntryKey, StringComparison.OrdinalIgnoreCase));

            // Botão de adicionar
            GUI.enabled = keyIsValid;
            if (GUILayout.Button("Adicionar", GUILayout.Height(30)))
            {
                AddNewEntryToList();
            }
            GUI.enabled = true;

            if (GUILayout.Button("Cancelar", GUILayout.Height(30)))
            {
                ClearNewEntryFields();
                showAddEntryPanel = false;
            }

            EditorGUILayout.EndHorizontal();

            // Mensagem de aviso se a chave já existe
            if (!string.IsNullOrEmpty(newEntryKey) &&
                entries.Any(e => e.Key.Equals(newEntryKey, StringComparison.OrdinalIgnoreCase)))
            {
                EditorGUILayout.HelpBox("Esta chave já existe!", MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawEntriesList()
        {
            // Filtrar as entradas
            var filteredEntries = entries
                .Where(e => string.IsNullOrEmpty(searchFilter) ||
                            e.Key.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            e.Values.Any(v => v != null && v.IndexOf(searchFilter, StringComparison.OrdinalIgnoreCase) >= 0))
                .Where(e => selectedPrefixFilter == 0 ||
                            prefixFilters[selectedPrefixFilter] == "all" ||
                            e.Key.StartsWith(prefixFilters[selectedPrefixFilter], StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Header da tabela
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("Chave", EditorStyles.toolbarButton, GUILayout.Width(200));

            for (int i = 1; i < headers.Length; i++)
            {
                EditorGUILayout.LabelField(headers[i], EditorStyles.toolbarButton, GUILayout.MinWidth(100));
            }

            EditorGUILayout.LabelField("Ações", EditorStyles.toolbarButton, GUILayout.Width(120));
            EditorGUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Se estiver agrupando por prefixo
            if (groupByPrefix)
            {
                var groupedEntries = filteredEntries
                    .GroupBy(e => GetPrefix(e.Key))
                    .OrderBy(g => g.Key);

                foreach (var group in groupedEntries)
                {
                    // Header do grupo
                    EditorGUILayout.Space(5);

                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(group.Key, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"({group.Count()} entradas)");
                    EditorGUILayout.EndHorizontal();

                    // Entradas no grupo
                    foreach (var entry in group.OrderBy(e => e.Key))
                    {
                        DrawEntryRow(entry);
                    }
                }
            }
            else
            {
                // Lista simples de entradas
                foreach (var entry in filteredEntries.OrderBy(e => e.Key))
                {
                    DrawEntryRow(entry);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawEntryRow(LocalizationEntry entry)
        {
            int entryIndex = entries.IndexOf(entry);

            // Se estiver editando esta entrada
            if (editingEntryIndex == entryIndex)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // Campo da chave
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Chave:", GUILayout.Width(50));
                editingKey = EditorGUILayout.TextField(editingKey);
                EditorGUILayout.EndHorizontal();

                // Campos para valores em diferentes idiomas
                for (int i = 1; i < headers.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(headers[i] + ":", GUILayout.Width(60));
                    editingValues[i - 1] = EditorGUILayout.TextField(editingValues[i - 1]);
                    EditorGUILayout.EndHorizontal();
                }

                // Botões
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Salvar", GUILayout.Height(25)))
                {
                    // Verifica se a nova chave já existe (exceto a própria entrada)
                    bool keyExists = entries.Any(e => e != entry &&
                                    e.Key.Equals(editingKey, StringComparison.OrdinalIgnoreCase));

                    if (keyExists)
                    {
                        SetStatusMessage("Não é possível salvar: chave já existe!", false);
                    }
                    else
                    {
                        // Atualiza a entrada
                        entry.Key = editingKey;
                        for (int i = 0; i < editingValues.Length; i++)
                        {
                            entry.Values[i] = editingValues[i];
                        }

                        editingEntryIndex = -1;
                        SetStatusMessage("Entrada atualizada com sucesso!", true);
                    }
                }

                if (GUILayout.Button("Cancelar", GUILayout.Height(25)))
                {
                    editingEntryIndex = -1;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            else
            {
                // Exibição normal da entrada
                EditorGUILayout.BeginHorizontal();

                // Chave
                EditorGUILayout.SelectableLabel(entry.Key, EditorStyles.label, GUILayout.Width(200), GUILayout.Height(20));

                // Valores
                for (int i = 0; i < entry.Values.Length; i++)
                {
                    EditorGUILayout.SelectableLabel(entry.Values[i], EditorStyles.label, GUILayout.MinWidth(100), GUILayout.Height(20));
                }

                // Botões de ação
                if (GUILayout.Button("Editar", EditorStyles.miniButtonLeft, GUILayout.Width(50)))
                {
                    editingEntryIndex = entryIndex;
                    editingKey = entry.Key;
                    Array.Copy(entry.Values, editingValues, entry.Values.Length);
                }

                if (GUILayout.Button("Excluir", EditorStyles.miniButtonRight, GUILayout.Width(50)))
                {
                    if (EditorUtility.DisplayDialog("Confirmar exclusão",
                        $"Tem certeza que deseja excluir a entrada '{entry.Key}'?",
                        "Sim", "Não"))
                    {
                        entries.Remove(entry);
                        SaveCsv();
                        SetStatusMessage("Entrada excluída com sucesso!", true);
                        GUIUtility.ExitGUI(); // Para evitar erro na UI
                    }
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void AddNewEntryToList()
        {
            // Verifica se a chave já existe
            if (entries.Any(e => e.Key.Equals(newEntryKey, StringComparison.OrdinalIgnoreCase)))
            {
                SetStatusMessage("Esta chave já existe!", false);
                return;
            }

            // Cria nova entrada
            var newEntry = new LocalizationEntry
            {
                Key = newEntryKey,
                Values = new string[headers.Length - 1]
            };

            // Copia os valores
            Array.Copy(newEntryValues, newEntry.Values, newEntryValues.Length);

            // Adiciona à lista
            entries.Add(newEntry);

            // Salva CSV
            SaveCsv();

            // Limpa campos
            ClearNewEntryFields();

            SetStatusMessage($"Nova entrada '{newEntryKey}' adicionada com sucesso!", true);
        }

        private void ClearNewEntryFields()
        {
            newEntryKey = "";
            for (int i = 0; i < newEntryValues.Length; i++)
            {
                newEntryValues[i] = "";
            }
        }

        private void SaveCsv()
        {
            try
            {
                // Garante que o diretório existe
                string directory = Path.GetDirectoryName(CSV_PATH);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Cria um StringBuilder para construir o conteúdo do CSV
                StringBuilder csv = new StringBuilder();

                // Adiciona cabeçalho
                csv.AppendLine(string.Join(",", headers));

                // Adiciona entradas
                foreach (var entry in entries.OrderBy(e => e.Key))
                {
                    // Prepara array para linha completa
                    string[] line = new string[headers.Length];
                    line[0] = entry.Key;

                    // Adiciona valores
                    for (int i = 0; i < entry.Values.Length; i++)
                    {
                        string value = entry.Values[i] ?? "";

                        // Coloca entre aspas se contiver vírgula ou nova linha
                        if (value.Contains(",") || value.Contains("\n") || value.Contains("\""))
                        {
                            value = value.Replace("\"", "\"\"");
                            value = $"\"{value}\"";
                        }

                        line[i + 1] = value;
                    }

                    // Adiciona linha
                    csv.AppendLine(string.Join(",", line));
                }

                // Escreve no arquivo
                File.WriteAllText(CSV_PATH, csv.ToString());

                AssetDatabase.Refresh();
                SetStatusMessage("Arquivo CSV salvo com sucesso!", true);
            }
            catch (Exception ex)
            {
                Debug.LogError("Erro ao salvar o arquivo CSV: " + ex.Message);
                SetStatusMessage("Erro ao salvar o arquivo CSV: " + ex.Message, false);
            }
        }

        private string[] ParseCsvLine(string line)
        {
            List<string> result = new List<string>();
            StringBuilder current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                // Se o caractere atual é aspas
                if (c == '"')
                {
                    // Se o próximo caractere também é aspas, é um escape
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++; // Pula o próximo caractere
                    }
                    else
                    {
                        // Alterna o estado de "em aspas"
                        inQuotes = !inQuotes;
                    }
                }
                // Se o caractere atual é vírgula e não estamos em aspas
                else if (c == ',' && !inQuotes)
                {
                    // Finaliza o campo atual
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    // Adiciona o caractere ao campo atual
                    current.Append(c);
                }
            }

            // Adiciona o último campo
            result.Add(current.ToString());

            return result.ToArray();
        }

        private string GetPrefix(string key)
        {
            if (string.IsNullOrEmpty(key)) return "Sem Prefixo";

            int underscorePos = key.IndexOf('_');
            if (underscorePos > 0)
            {
                return key.Substring(0, underscorePos + 1);
            }

            return "Sem Prefixo";
        }

        private void SetStatusMessage(string message, bool isSuccess)
        {
            statusMessage = message;
            showStatusMessage = true;
            statusMessageTime = (float)EditorApplication.timeSinceStartup;
        }
    }

    // Classe para armazenar entradas de localização
    public class LocalizationEntry
    {
        public string Key { get; set; }
        public string[] Values { get; set; }
    }
}
#endif
