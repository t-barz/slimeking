using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using TheSlimeKing.Core.Dialogue;
using TheSlimeKing.Core;

namespace TheSlimeKing.Editor.Dialogue
{
    [CustomEditor(typeof(DialogueData))]
    public class DialogueDataEditor : UnityEditor.Editor
    {
        // ReorderableList para as linhas de diálogo
        private ReorderableList _linesList;

        // Referência para o LocalizationManager
        private LocalizationManager _localizationManager;

        // Cache para nomes traduções das chaves
        private Dictionary<string, string> _cachedLocalizedTexts = new Dictionary<string, string>();

        private void OnEnable()
        {
            // Busca o LocalizationManager
            _localizationManager = LocalizationManager.Instance;

            // Inicializa a lista reordenável para as linhas de diálogo
            SerializedProperty linesProperty = serializedObject.FindProperty("Lines");
            _linesList = new ReorderableList(
                serializedObject,
                linesProperty,
                true, // draggable
                true, // displayHeader
                true, // add button
                true  // remove button
            );

            // Configura o cabeçalho da lista
            _linesList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Linhas de Diálogo");
            };

            // Configura a renderização de cada elemento
            _linesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty element = linesProperty.GetArrayElementAtIndex(index);
                SerializedProperty textKeyProp = element.FindPropertyRelative("TextKey");
                SerializedProperty speakerNameKeyProp = element.FindPropertyRelative("SpeakerNameKey");
                SerializedProperty speakerSpriteProp = element.FindPropertyRelative("SpeakerSprite");

                rect.y += 2;
                float lineHeight = EditorGUIUtility.singleLineHeight;
                float padding = 2f;

                // Área para o falante
                Rect speakerRect = new Rect(rect.x, rect.y, rect.width, lineHeight);
                EditorGUI.PropertyField(speakerRect, speakerNameKeyProp, new GUIContent("Falante"));

                // Área para o sprite
                Rect spriteRect = new Rect(rect.x, speakerRect.y + lineHeight + padding, rect.width, lineHeight);
                EditorGUI.PropertyField(spriteRect, speakerSpriteProp, new GUIContent("Imagem"));

                // Área para a chave de texto
                Rect textKeyRect = new Rect(rect.x, spriteRect.y + lineHeight + padding, rect.width, lineHeight);
                EditorGUI.PropertyField(textKeyRect, textKeyProp, new GUIContent("Chave de Texto"));

                // Exibe o texto traduzido como amostra, se disponível
                if (_localizationManager != null && !string.IsNullOrEmpty(textKeyProp.stringValue))
                {
                    string textKey = textKeyProp.stringValue;
                    string localizedText;

                    // Usa o cache para evitar chamadas repetidas ao LocalizationManager
                    if (!_cachedLocalizedTexts.TryGetValue(textKey, out localizedText))
                    {
                        localizedText = _localizationManager.GetLocalizedText(textKey);
                        _cachedLocalizedTexts[textKey] = localizedText;
                    }

                    Rect previewRect = new Rect(
                        rect.x,
                        textKeyRect.y + lineHeight + padding,
                        rect.width,
                        lineHeight * 2
                    );

                    // Estilo para exibir a prévia
                    GUIStyle previewStyle = new GUIStyle(EditorStyles.helpBox);
                    previewStyle.wordWrap = true;
                    previewStyle.normal.textColor = Color.gray;

                    EditorGUI.LabelField(previewRect, "Prévia: " + localizedText, previewStyle);
                }
            };

            // Define a altura de cada elemento
            _linesList.elementHeightCallback = (int index) =>
            {
                float lineHeight = EditorGUIUtility.singleLineHeight;
                float padding = 2f;

                // Altura básica para falante + sprite + chave de texto
                float height = (lineHeight * 3) + (padding * 3);

                // Adiciona espaço para visualização do texto traduzido
                height += lineHeight * 2 + padding;

                return height;
            };

            // Configura o callback para adicionar um novo elemento
            _linesList.onAddCallback = (ReorderableList list) =>
            {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;

                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("TextKey").stringValue = "";
                element.FindPropertyRelative("SpeakerNameKey").stringValue = "";
                element.FindPropertyRelative("SpeakerSprite").objectReferenceValue = null;
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // ID do diálogo
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DialogueID"));

            // Status de visualização (somente leitura)
            DialogueData dialogueData = (DialogueData)target;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Toggle("Já exibido", dialogueData.HasBeenShown);
            EditorGUI.EndDisabledGroup();

            // Botão para resetar o status de visualização (apenas em modo de edição)
            if (dialogueData.HasBeenShown)
            {
                if (GUILayout.Button("Resetar Status (Não Exibido)"))
                {
                    dialogueData.HasBeenShown = false;
                    EditorUtility.SetDirty(dialogueData);
                }
            }

            EditorGUILayout.Space();

            // Desenha a lista de linhas de diálogo
            _linesList.DoLayoutList();

            // Aplica as mudanças
            serializedObject.ApplyModifiedProperties();

            // Botões úteis
            EditorGUILayout.Space();

            // Botão para testar diálogo
            GUI.enabled = Application.isPlaying && DialogueManager.Instance != null;
            if (GUILayout.Button("Testar Diálogo (somente Play Mode)"))
            {
                DialogueManager.Instance.StartDialogue(dialogueData);
            }
            GUI.enabled = true;

            // Botão para localizar usos deste diálogo
            if (GUILayout.Button("Localizar Usos na Cena"))
            {
                FindDialogueUsages(dialogueData);
            }
        }

        /// <summary>
        /// Busca por objetos na cena que usam este diálogo.
        /// </summary>
        private void FindDialogueUsages(DialogueData dialogueData)
        {
            DialogueTrigger[] triggers = GameObject.FindObjectsOfType<DialogueTrigger>();
            List<DialogueTrigger> matchingTriggers = new List<DialogueTrigger>();

            // Busca por DialogueTriggers que usam este diálogo
            foreach (var trigger in triggers)
            {
                var serializedTrigger = new SerializedObject(trigger);
                var dialogueDataRef = serializedTrigger.FindProperty("_dialogueData");

                if (dialogueDataRef.objectReferenceValue == dialogueData)
                {
                    matchingTriggers.Add(trigger);
                }
            }

            // Mostra os resultados
            if (matchingTriggers.Count > 0)
            {
                Debug.Log($"Encontrado {matchingTriggers.Count} uso(s) do diálogo '{dialogueData.name}':");

                foreach (var trigger in matchingTriggers)
                {
                    Debug.Log($"- {trigger.gameObject.name}", trigger.gameObject);

                    // Seleciona o primeiro objeto encontrado
                    if (matchingTriggers.IndexOf(trigger) == 0)
                    {
                        Selection.activeGameObject = trigger.gameObject;
                        EditorGUIUtility.PingObject(trigger.gameObject);
                    }
                }
            }
            else
            {
                Debug.Log($"Nenhum uso do diálogo '{dialogueData.name}' foi encontrado na cena atual.");
            }
        }
    }

    /// <summary>
    /// Editor customizado para DialogueTrigger
    /// </summary>
    [CustomEditor(typeof(DialogueTrigger))]
    public class DialogueTriggerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Desenha o inseptor original para as propriedades de InteractableObject
            DrawDefaultInspector();

            // Adiciona botão para testar o diálogo
            DialogueTrigger trigger = (DialogueTrigger)target;

            EditorGUILayout.Space();
            GUI.enabled = Application.isPlaying && DialogueManager.Instance != null;

            if (GUILayout.Button("Testar Diálogo (somente Play Mode)"))
            {
                trigger.Interact(null);
            }

            GUI.enabled = true;
        }
    }
}
