using UnityEngine;
using UnityEditor;
using SlimeKing.Gameplay;

namespace SlimeKing.EditorTools
{
    /// <summary>
    /// Editor customizado para facilitar a configuração de objetos DialogInteractable
    /// </summary>
    [CustomEditor(typeof(DialogInteractable))]
    public class DialogInteractableEditor : Editor
    {
        private SerializedProperty dialogTitleProp;
        private SerializedProperty dialogTextProp;
        private SerializedProperty speakerIconProp;
        private SerializedProperty dialogSequenceProp;
        private SerializedProperty titleSequenceProp;
        private SerializedProperty visualCueProp;

        private void OnEnable()
        {
            dialogTitleProp = serializedObject.FindProperty("dialogTitle");
            dialogTextProp = serializedObject.FindProperty("dialogText");
            speakerIconProp = serializedObject.FindProperty("speakerIcon");
            dialogSequenceProp = serializedObject.FindProperty("dialogSequence");
            titleSequenceProp = serializedObject.FindProperty("titleSequence");
            visualCueProp = serializedObject.FindProperty("visualCue");
        }

        public override void OnInspectorGUI()
        {
            // Atualiza o objeto serializado
            serializedObject.Update();

            // Título personalizado
            EditorGUILayout.LabelField("Configurações de Diálogo", EditorStyles.boldLabel);

            // Define o modo de diálogo
            bool useSequentialDialogs = EditorGUILayout.Toggle("Usar Diálogos Sequenciais", dialogSequenceProp.arraySize > 0); if (useSequentialDialogs)
            {
                EditorGUILayout.PropertyField(dialogSequenceProp, new GUIContent("Sequência de Diálogos"), true);
                EditorGUILayout.PropertyField(titleSequenceProp, new GUIContent("Sequência de Títulos"), true);
                EditorGUILayout.HelpBox("A sequência de diálogos será exibida um após o outro a cada interação. Se desejar, adicione títulos correspondentes a cada diálogo.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.PropertyField(dialogTitleProp, new GUIContent("Título do Diálogo"));
                EditorGUILayout.PropertyField(dialogTextProp, new GUIContent("Texto do Diálogo"));
            }

            EditorGUILayout.PropertyField(speakerIconProp, new GUIContent("Ícone do Falante"));
            EditorGUILayout.PropertyField(visualCueProp, new GUIContent("Indicador Visual"));

            // Botão para criar um indicador visual
            if (visualCueProp.objectReferenceValue == null)
            {
                if (GUILayout.Button("Criar Indicador Visual Padrão"))
                {
                    CreateVisualCue();
                }
            }

            // Botão para testar diálogo
            if (Application.isPlaying && GUILayout.Button("Testar Diálogo"))
            {
                DialogInteractable dialogInteractable = (DialogInteractable)target;
                dialogInteractable.Interact();
            }

            // Aplica as modificações
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Cria um indicador visual básico para o objeto interativo
        /// </summary>
        private void CreateVisualCue()
        {
            DialogInteractable dialogInteractable = (DialogInteractable)target;

            // Cria um sprite para servir como indicador visual
            GameObject visualCue = new GameObject("VisualCue");
            visualCue.transform.SetParent(dialogInteractable.transform);
            visualCue.transform.localPosition = new Vector3(0, 1.5f, 0);

            SpriteRenderer spriteRenderer = visualCue.AddComponent<SpriteRenderer>();
            Texture2D texture = EditorGUIUtility.FindTexture("console.infoicon") as Texture2D;
            if (texture != null)
            {
                Rect rect = new Rect(0, 0, texture.width, texture.height);
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                spriteRenderer.sprite = Sprite.Create(texture, rect, pivot);
            }
            spriteRenderer.sortingOrder = 10;

            visualCueProp.objectReferenceValue = visualCue;
            serializedObject.ApplyModifiedProperties();

            // Desativa o indicador por padrão
            visualCue.SetActive(false);
        }
    }
}
