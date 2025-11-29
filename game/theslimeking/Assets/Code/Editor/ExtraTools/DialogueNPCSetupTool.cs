using UnityEngine;
using UnityEditor;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using TheSlimeKing.Dialogue;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Ferramenta para configurar NPCs com o sistema de diálogo automaticamente.
    /// </summary>
    public static class DialogueNPCSetupTool
    {
        /// <summary>
        /// Configura um GameObject como NPC de diálogo.
        /// </summary>
        public static void SetupDialogueNPC(GameObject npc)
        {
            if (npc == null)
            {
                Debug.LogError("[DialogueNPCSetupTool] GameObject é null!");
                return;
            }
            
            Debug.Log($"[DialogueNPCSetupTool] Configurando {npc.name} como NPC de diálogo...");
            
            // 1. Adicionar DialogueNPC component se não existir
            DialogueNPC dialogueNPC = npc.GetComponent<DialogueNPC>();
            if (dialogueNPC == null)
            {
                dialogueNPC = npc.AddComponent<DialogueNPC>();
                Debug.Log($"[DialogueNPCSetupTool] DialogueNPC component adicionado.");
            }
            else
            {
                Debug.Log($"[DialogueNPCSetupTool] DialogueNPC component já existe.");
            }
            
            // 2. Configurar BoxCollider2D como trigger
            BoxCollider2D collider = npc.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = npc.AddComponent<BoxCollider2D>();
                Debug.Log($"[DialogueNPCSetupTool] BoxCollider2D adicionado.");
            }
            
            collider.isTrigger = true;
            
            // Ajustar tamanho do collider baseado no SpriteRenderer se existir
            SpriteRenderer spriteRenderer = npc.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                collider.size = spriteRenderer.sprite.bounds.size;
                collider.offset = spriteRenderer.sprite.bounds.center;
                Debug.Log($"[DialogueNPCSetupTool] Collider ajustado ao tamanho do sprite.");
            }
            
            // 3. Buscar ou criar DialogueCanvas na cena
            DialogueUI dialogueUI = Object.FindFirstObjectByType<DialogueUI>();
            
            if (dialogueUI == null)
            {
                Debug.Log($"[DialogueNPCSetupTool] DialogueCanvas não encontrado. Criando...");
                
                // Verificar se o prefab existe
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Game/Prefabs/UI/DialogueCanvas.prefab");
                
                if (prefab != null)
                {
                    // Instanciar prefab
                    GameObject canvasInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                    dialogueUI = canvasInstance.GetComponent<DialogueUI>();
                    Debug.Log($"[DialogueNPCSetupTool] DialogueCanvas instanciado do prefab.");
                }
                else
                {
                    // Criar usando o setup script
                    GameObject canvasObj = DialogueCanvasSetup.CreateDialogueCanvas();
                    dialogueUI = canvasObj.GetComponent<DialogueUI>();
                    Debug.Log($"[DialogueNPCSetupTool] DialogueCanvas criado programaticamente.");
                }
            }
            else
            {
                Debug.Log($"[DialogueNPCSetupTool] DialogueCanvas já existe na cena.");
            }
            
            // 4. Configurar tag do NPC
            if (npc.tag != "NPC")
            {
                npc.tag = "NPC";
                Debug.Log($"[DialogueNPCSetupTool] Tag 'NPC' configurada.");
            }
            
            // 5. Adicionar entrada de localização padrão (se não houver textos)
            SerializedObject serializedNPC = new SerializedObject(dialogueNPC);
            SerializedProperty dialogueTextsProperty = serializedNPC.FindProperty("dialogueTexts");
            
            if (dialogueTextsProperty.arraySize == 0)
            {
                // Adicionar uma entrada vazia para o desenvolvedor configurar
                dialogueTextsProperty.InsertArrayElementAtIndex(0);
                serializedNPC.ApplyModifiedProperties();
                Debug.Log($"[DialogueNPCSetupTool] Entrada de diálogo padrão adicionada. Configure no Inspector.");
            }
            
            // 6. Criar indicador de interação se não existir
            Transform indicatorTransform = npc.transform.Find("InteractionIndicator");
            if (indicatorTransform == null)
            {
                GameObject indicator = new GameObject("InteractionIndicator");
                indicator.transform.SetParent(npc.transform);
                indicator.transform.localPosition = new Vector3(0, 1f, 0);
                
                // Adicionar SpriteRenderer com sprite simples
                SpriteRenderer indicatorRenderer = indicator.AddComponent<SpriteRenderer>();
                indicatorRenderer.sortingOrder = 100;
                
                // Tentar carregar sprite de indicador
                Sprite indicatorSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/uii_inputAct1.png");
                if (indicatorSprite != null)
                {
                    indicatorRenderer.sprite = indicatorSprite;
                }
                else
                {
                    // Criar sprite temporário
                    Texture2D tex = new Texture2D(32, 32);
                    Color[] pixels = new Color[32 * 32];
                    for (int i = 0; i < pixels.Length; i++)
                        pixels[i] = Color.yellow;
                    tex.SetPixels(pixels);
                    tex.Apply();
                    indicatorRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
                }
                
                indicator.SetActive(false);
                
                // Configurar referência no DialogueNPC
                serializedNPC.FindProperty("interactionIndicator").objectReferenceValue = indicator;
                serializedNPC.ApplyModifiedProperties();
                
                Debug.Log($"[DialogueNPCSetupTool] Indicador de interação criado.");
            }
            
            // Marcar cena como modificada
            EditorUtility.SetDirty(npc);
            if (dialogueUI != null)
            {
                EditorUtility.SetDirty(dialogueUI.gameObject);
            }
            
            Debug.Log($"[DialogueNPCSetupTool] ✅ Setup completo para {npc.name}!");
            EditorUtility.DisplayDialog("Setup Dialogue NPC", 
                $"✅ {npc.name} configurado como NPC de diálogo!\n\n" +
                $"Próximos passos:\n" +
                $"1. Configure os textos localizados no Inspector\n" +
                $"2. Ajuste o raio de interação se necessário\n" +
                $"3. Adicione eventos opcionais (quests, cutscenes, etc.)", 
                "OK");
        }
    }
}
