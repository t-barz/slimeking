using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using TheSlimeKing.Core.Dialogue;
using TheSlimeKing.Core;

namespace TheSlimeKing.Editor.Dialogue
{
    public class DialogueEditorWindow : EditorWindow
    {
        // Categorias de diálogos
        private enum DialogueCategory
        {
            Tutorial,
            NPC,
            Item,
            Environment,
            Quest,
            Cutscene,
            Other
        }

        // Estado da janela
        private string _dialogueID = "";
        private string _speakerKey = "";
        private string _textKey = "";
        private DialogueCategory _selectedCategory = DialogueCategory.NPC;
        private List<DialogueLine> _dialogueLines = new List<DialogueLine>();
        private Vector2 _scrollPosition = Vector2.zero;

        // Variáveis para criação rápida
        private string _baseKey = "";
        private int _lineCount = 1;
        private bool _showQuickCreation = false;
        private bool _addToLocalizationFile = true;

        [MenuItem("The Slime King/Ferramentas/Editor de Diálogos")]
        public static void ShowWindow()
        {
            // Abre a janela
            DialogueEditorWindow window = GetWindow<DialogueEditorWindow>("Editor de Diálogos");
            window.minSize = new Vector2(500, 600);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            // Título
            GUIStyle titleStyle = new GUIStyle(EditorStyles.largeLabel);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.fontSize = 16;
            EditorGUILayout.LabelField("Editor de Diálogos - The Slime King", titleStyle);

            EditorGUILayout.Space(10);

            // Área de rolagem
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Informações básicas do diálogo
            EditorGUILayout.LabelField("Informações do Diálogo", EditorStyles.boldLabel);

            // ID do diálogo
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID do Diálogo:", GUILayout.Width(120));
            _dialogueID = EditorGUILayout.TextField(_dialogueID);
            EditorGUILayout.EndHorizontal();

            // Categoria do diálogo
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Categoria:", GUILayout.Width(120));
            _selectedCategory = (DialogueCategory)EditorGUILayout.EnumPopup(_selectedCategory);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Criação rápida de chaves de diálogo
            _showQuickCreation = EditorGUILayout.Foldout(_showQuickCreation, "Criação Rápida de Chaves");

            if (_showQuickCreation)
            {
                EditorGUI.indentLevel++;

                // Campo para o prefixo base das chaves
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Prefixo de Chave:", GUILayout.Width(120));
                _baseKey = EditorGUILayout.TextField(_baseKey);
                EditorGUILayout.EndHorizontal();

                // Número de linhas a criar
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Número de Linhas:", GUILayout.Width(120));
                _lineCount = EditorGUILayout.IntField(_lineCount);
                EditorGUILayout.EndHorizontal();

                // Opção para adicionar ao arquivo de localização
                _addToLocalizationFile = EditorGUILayout.Toggle("Adicionar ao CSV de Localização", _addToLocalizationFile);

                // Botão para gerar as linhas
                if (GUILayout.Button("Gerar Linhas"))
                {
                    GenerateDialogueLines();
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            // Linhas de diálogo
            EditorGUILayout.LabelField("Linhas de Diálogo", EditorStyles.boldLabel);

            // Cabeçalho da tabela
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Falante", EditorStyles.boldLabel, GUILayout.Width(120));
            EditorGUILayout.LabelField("Chave de Texto", EditorStyles.boldLabel, GUILayout.Width(200));
            EditorGUILayout.LabelField("Prévia (se disponível)", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            // Desenha cada linha
            for (int i = 0; i < _dialogueLines.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                // Campo para o falante
                _dialogueLines[i].SpeakerNameKey = EditorGUILayout.TextField(_dialogueLines[i].SpeakerNameKey, GUILayout.Width(120));

                // Campo para a chave de texto
                _dialogueLines[i].TextKey = EditorGUILayout.TextField(_dialogueLines[i].TextKey, GUILayout.Width(200));

                // Prévia do texto localizado (se disponível)
                if (LocalizationManager.Instance != null)
                {
                    string localizedText = LocalizationManager.Instance.GetLocalizedText(_dialogueLines[i].TextKey);
                    EditorGUILayout.LabelField(localizedText, EditorStyles.wordWrappedLabel);
                }
                else
                {
                    EditorGUILayout.LabelField("(LocalizationManager não disponível)");
                }

                // Botão para remover a linha
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _dialogueLines.RemoveAt(i);
                    i--; // Ajusta o índice após remover
                }

                EditorGUILayout.EndHorizontal();
            }

            // Botão para adicionar nova linha
            if (GUILayout.Button("Adicionar Linha"))
            {
                _dialogueLines.Add(new DialogueLine());
            }

            EditorGUILayout.Space(10);

            // Botões de ação
            EditorGUILayout.BeginHorizontal();

            // Limpa os campos
            if (GUILayout.Button("Limpar"))
            {
                ClearForm();
            }

            // Cria o ScriptableObject de diálogo
            if (GUILayout.Button("Criar Diálogo"))
            {
                CreateDialogueAsset();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Gera linhas de diálogo com base no prefixo e quantidade especificados.
        /// </summary>
        private void GenerateDialogueLines()
        {
            if (string.IsNullOrEmpty(_baseKey))
            {
                EditorUtility.DisplayDialog("Erro", "Informe um prefixo de chave válido.", "OK");
                return;
            }

            _lineCount = Mathf.Max(1, _lineCount); // Mínimo de 1 linha
            _dialogueLines.Clear();

            for (int i = 1; i <= _lineCount; i++)
            {
                string lineKey = $"{_baseKey}_{i:D2}";

                DialogueLine line = new DialogueLine
                {
                    SpeakerNameKey = _speakerKey,
                    TextKey = lineKey
                };

                _dialogueLines.Add(line);

                // Adiciona a chave ao arquivo de localização, se solicitado
                if (_addToLocalizationFile && LocalizationManager.Instance != null)
                {
                    // Implementação simplificada - em uma aplicação real, isso adicionaria a chave ao CSV
                    Debug.Log($"Adicionando chave de localização: {lineKey}");
                    // Aqui você integraria com seu sistema real de localização
                }
            }
        }

        /// <summary>
        /// Limpa o formulário.
        /// </summary>
        private void ClearForm()
        {
            _dialogueID = "";
            _speakerKey = "";
            _textKey = "";
            _baseKey = "";
            _dialogueLines.Clear();
        }

        /// <summary>
        /// Cria um asset de diálogo com base nas informações preenchidas.
        /// </summary>
        private void CreateDialogueAsset()
        {
            // Validações
            if (string.IsNullOrEmpty(_dialogueID))
            {
                EditorUtility.DisplayDialog("Erro", "O ID do diálogo não pode estar vazio.", "OK");
                return;
            }

            if (_dialogueLines.Count == 0)
            {
                EditorUtility.DisplayDialog("Erro", "O diálogo deve ter pelo menos uma linha.", "OK");
                return;
            }

            // Cria o ScriptableObject
            DialogueData dialogueData = ScriptableObject.CreateInstance<DialogueData>();
            dialogueData.DialogueID = _dialogueID;
            dialogueData.Lines = new List<DialogueLine>(_dialogueLines);

            // Define o nome do arquivo baseado na categoria
            string categoryFolder = _selectedCategory.ToString();
            string safeName = GetSafeFileName(_dialogueID);

            // Cria a pasta se necessário
            string folderPath = $"Assets/Resources/Dialogues/{categoryFolder}";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Salva o asset
            string assetPath = $"{folderPath}/{safeName}.asset";
            AssetDatabase.CreateAsset(dialogueData, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Seleciona o asset no Project
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = dialogueData;

            EditorUtility.DisplayDialog("Sucesso", $"Diálogo criado com sucesso!\nSalvo em: {assetPath}", "OK");

            // Limpa o formulário
            ClearForm();
        }

        /// <summary>
        /// Converte um ID em um nome de arquivo válido.
        /// </summary>
        private string GetSafeFileName(string input)
        {
            // Remove caracteres inválidos para nome de arquivo
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string result = input;

            foreach (char c in invalidChars)
            {
                result = result.Replace(c, '_');
            }

            return result;
        }
    }
}
