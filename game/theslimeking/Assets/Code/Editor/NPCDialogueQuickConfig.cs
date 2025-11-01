using UnityEngine;
using UnityEditor;
using SlimeMec.Gameplay.NPCs;

namespace SlimeKing.Editor
{
    /// <summary>
    /// Utilitário para configuração rápida de NPCs com sistema de diálogo.
    /// Adiciona todos os componentes necessários e configura automaticamente.
    /// </summary>
    public static class NPCDialogueQuickConfig
    {
        // Paths dos assets necessários
        private const string INTERACTION_ICON_PREFAB_PATH = "Assets/Game/Prefabs/UI/InteractionIcon.prefab";

        [MenuItem("GameObject/SlimeKing/Configure as Dialogue NPC", false, 10)]
        public static void ConfigureAsDialogueNPC(MenuCommand menuCommand)
        {
            // Obtém o GameObject selecionado
            GameObject targetObject = menuCommand.context as GameObject;

            if (targetObject == null)
            {
                Debug.LogError("⚠️ NPCDialogueQuickConfig: Nenhum GameObject selecionado!");
                return;
            }

            // Registra para Undo
            Undo.RegisterCompleteObjectUndo(targetObject, "Configure as Dialogue NPC");

            try
            {
                ConfigureDialogueComponents(targetObject);
                Debug.Log($"✅ NPC de diálogo configurado com sucesso: {targetObject.name}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Erro ao configurar NPC de diálogo: {e.Message}");
            }
        }

        /// <summary>
        /// Valida se o menu deve aparecer (só quando há GameObject selecionado)
        /// </summary>
        [MenuItem("GameObject/SlimeKing/Configure as Dialogue NPC", true)]
        public static bool ValidateConfigureAsDialogueNPC()
        {
            return Selection.activeGameObject != null;
        }

        /// <summary>
        /// Configura todos os componentes necessários para um NPC de diálogo.
        /// </summary>
        public static void ConfigureDialogueComponents(GameObject targetObject)
        {
            // 1. Adicionar NPCDialogueInteraction (se não existir)
            NPCDialogueInteraction dialogueInteraction = targetObject.GetComponent<NPCDialogueInteraction>();
            if (dialogueInteraction == null)
            {
                dialogueInteraction = Undo.AddComponent<NPCDialogueInteraction>(targetObject);
                Debug.Log("💬 NPCDialogueInteraction adicionado");
            }
            else
            {
                Debug.Log("✅ NPCDialogueInteraction já existe, mantendo configuração");
            }

            // 2. Configurar CircleCollider2D (se não existir ou não estiver configurado como trigger)
            ConfigureInteractionCollider(targetObject);

            // 3. Carregar e atribuir prefab do InteractionIcon
            ConfigureInteractionIcon(targetObject, dialogueInteraction);

            // 4. Configurar valores padrão
            ConfigureDefaultValues(targetObject, dialogueInteraction);

            // Marcar objeto como modificado
            EditorUtility.SetDirty(targetObject);
        }

        /// <summary>
        /// Configura o CircleCollider2D para detecção de proximidade.
        /// </summary>
        private static void ConfigureInteractionCollider(GameObject targetObject)
        {
            CircleCollider2D circleCollider = targetObject.GetComponent<CircleCollider2D>();
            
            if (circleCollider == null)
            {
                circleCollider = Undo.AddComponent<CircleCollider2D>(targetObject);
                Debug.Log("🔘 CircleCollider2D adicionado");
            }

            // Configura como trigger com raio padrão
            circleCollider.isTrigger = true;
            circleCollider.radius = 2.5f;
            circleCollider.offset = Vector2.zero;

            Debug.Log($"🔘 CircleCollider2D configurado como trigger (raio: {circleCollider.radius})");
        }

        /// <summary>
        /// Carrega e atribui o prefab do InteractionIcon ao componente NPCDialogueInteraction.
        /// </summary>
        private static void ConfigureInteractionIcon(GameObject targetObject, NPCDialogueInteraction dialogueInteraction)
        {
            // Carrega o prefab do InteractionIcon
            GameObject interactionIconPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(INTERACTION_ICON_PREFAB_PATH);

            if (interactionIconPrefab == null)
            {
                Debug.LogWarning($"⚠️ Prefab do InteractionIcon não encontrado em: {INTERACTION_ICON_PREFAB_PATH}");
                return;
            }

            // Usa SerializedObject para atribuir o prefab ao campo privado
            SerializedObject serializedObject = new SerializedObject(dialogueInteraction);
            SerializedProperty iconPrefabProperty = serializedObject.FindProperty("interactionIconPrefab");

            if (iconPrefabProperty != null)
            {
                iconPrefabProperty.objectReferenceValue = interactionIconPrefab;
                serializedObject.ApplyModifiedProperties();
                Debug.Log($"🎯 Prefab do InteractionIcon atribuído: {interactionIconPrefab.name}");
            }
            else
            {
                Debug.LogWarning("⚠️ Não foi possível encontrar o campo 'interactionIconPrefab' no NPCDialogueInteraction");
            }
        }

        /// <summary>
        /// Configura valores padrão para o componente NPCDialogueInteraction.
        /// </summary>
        private static void ConfigureDefaultValues(GameObject targetObject, NPCDialogueInteraction dialogueInteraction)
        {
            SerializedObject serializedObject = new SerializedObject(dialogueInteraction);

            // Configura raio de interação padrão
            SerializedProperty radiusProperty = serializedObject.FindProperty("interactionRadius");
            if (radiusProperty != null && radiusProperty.floatValue == 0f)
            {
                radiusProperty.floatValue = 2.5f;
            }

            // Configura iconAnchor (usa o próprio transform do NPC se não estiver definido)
            SerializedProperty anchorProperty = serializedObject.FindProperty("iconAnchor");
            if (anchorProperty != null && anchorProperty.objectReferenceValue == null)
            {
                anchorProperty.objectReferenceValue = targetObject.transform;
            }

            // Configura dialogueId padrão se estiver vazio
            SerializedProperty dialogueIdProperty = serializedObject.FindProperty("dialogueId");
            if (dialogueIdProperty != null && string.IsNullOrEmpty(dialogueIdProperty.stringValue))
            {
                dialogueIdProperty.stringValue = $"npc_{targetObject.name.ToLower().Replace(" ", "_")}";
                Debug.Log($"💬 Dialogue ID padrão configurado: {dialogueIdProperty.stringValue}");
            }

            // Configura botão de interação padrão
            SerializedProperty buttonProperty = serializedObject.FindProperty("interactionButton");
            if (buttonProperty != null && string.IsNullOrEmpty(buttonProperty.stringValue))
            {
                buttonProperty.stringValue = "Interact";
            }

            serializedObject.ApplyModifiedProperties();
            Debug.Log("⚙️ Valores padrão configurados");
        }

        #region Utilitários de Debug

        [MenuItem("GameObject/SlimeKing/Show Dialogue NPC Info", false, 11)]
        public static void ShowDialogueNPCInfo()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                Debug.LogWarning("⚠️ Nenhum GameObject selecionado!");
                return;
            }

            NPCDialogueInteraction dialogueInteraction = selectedObject.GetComponent<NPCDialogueInteraction>();
            if (dialogueInteraction == null)
            {
                Debug.LogWarning($"⚠️ {selectedObject.name} não possui componente NPCDialogueInteraction!");
                return;
            }

            Debug.Log($"=== DIALOGUE NPC INFO: {selectedObject.name} ===");
            
            SerializedObject serializedObject = new SerializedObject(dialogueInteraction);
            
            SerializedProperty dialogueIdProp = serializedObject.FindProperty("dialogueId");
            SerializedProperty radiusProp = serializedObject.FindProperty("interactionRadius");
            SerializedProperty anchorProp = serializedObject.FindProperty("iconAnchor");
            SerializedProperty prefabProp = serializedObject.FindProperty("interactionIconPrefab");
            SerializedProperty buttonProp = serializedObject.FindProperty("interactionButton");

            Debug.Log($"Dialogue ID: {dialogueIdProp?.stringValue ?? "N/A"}");
            Debug.Log($"Interaction Radius: {radiusProp?.floatValue ?? 0f}");
            Debug.Log($"Icon Anchor: {(anchorProp?.objectReferenceValue != null ? anchorProp.objectReferenceValue.name : "None")}");
            Debug.Log($"Icon Prefab: {(prefabProp?.objectReferenceValue != null ? prefabProp.objectReferenceValue.name : "None")}");
            Debug.Log($"Interaction Button: {buttonProp?.stringValue ?? "N/A"}");

            CircleCollider2D collider = selectedObject.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                Debug.Log($"CircleCollider2D: Trigger={collider.isTrigger}, Radius={collider.radius}");
            }
            else
            {
                Debug.Log("CircleCollider2D: Not found");
            }

            Debug.Log("================================");
        }

        [MenuItem("GameObject/SlimeKing/Show Dialogue NPC Info", true)]
        public static bool ValidateShowDialogueNPCInfo()
        {
            return Selection.activeGameObject != null;
        }

        #endregion
    }
}
