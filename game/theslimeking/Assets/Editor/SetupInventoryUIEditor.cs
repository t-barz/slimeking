using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using SlimeKing.UI;

namespace SlimeKing.Editor
{
    public class SetupInventoryUIEditor : EditorWindow
    {
        [MenuItem("SlimeKing/Setup/Configure Inventory UI")]
        public static void ConfigureInventoryUI()
        {
            // Encontra o InventoryCanvas na cena
            GameObject inventoryCanvas = GameObject.Find("InventoryCanvas");
            
            if (inventoryCanvas == null)
            {
                EditorUtility.DisplayDialog("Erro", "InventoryCanvas não encontrado na cena!", "OK");
                return;
            }

            // 1. Adiciona ou obtém o componente InventoryUI
            InventoryUI inventoryUI = inventoryCanvas.GetComponent<InventoryUI>();
            if (inventoryUI == null)
            {
                inventoryUI = inventoryCanvas.AddComponent<InventoryUI>();
            }

            // 2. Configura referências
            Transform inventoryPanel = inventoryCanvas.transform.Find("InventoryPanel");
            CanvasGroup canvasGroup = inventoryCanvas.GetComponent<CanvasGroup>();

            if (inventoryPanel != null)
            {
                SerializedObject so = new SerializedObject(inventoryUI);
                so.FindProperty("inventoryPanel").objectReferenceValue = inventoryPanel.gameObject;
                so.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
                so.ApplyModifiedProperties();
            }

            // 3. Adiciona Button components aos slots
            Transform slotsContainer = inventoryPanel?.Find("SlotsContainer");
            if (slotsContainer != null)
            {
                int slotsConfigured = 0;
                for (int i = 0; i < 12; i++)
                {
                    Transform slot = slotsContainer.Find($"Slot_{i}");
                    if (slot != null)
                    {
                        Button btn = slot.GetComponent<Button>();
                        if (btn == null)
                        {
                            btn = slot.gameObject.AddComponent<Button>();
                        }
                        
                        // Configura o target graphic (Image)
                        Image img = slot.GetComponent<Image>();
                        if (img != null)
                        {
                            btn.targetGraphic = img;
                        }

                        // Remove listeners antigos e adiciona novo
                        btn.onClick.RemoveAllListeners();
                        int slotIndex = i; // Captura o índice
                        btn.onClick.AddListener(() => inventoryUI.OnSlotClicked(slotIndex));
                        
                        // Configura Outline
                        Outline outline = slot.GetComponent<Outline>();
                        if (outline != null)
                        {
                            outline.effectColor = Color.white;
                            outline.effectDistance = new Vector2(2f, -2f);
                            outline.useGraphicAlpha = true;
                            EditorUtility.SetDirty(outline);
                        }
                        
                        EditorUtility.SetDirty(btn);
                        slotsConfigured++;
                    }
                }
            }

            // Marca a cena como modificada
            EditorUtility.SetDirty(inventoryCanvas);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );

            EditorUtility.DisplayDialog(
                "Sucesso", 
                "Inventory UI configurado com sucesso!\n\n" +
                "✓ InventoryUI component adicionado\n" +
                "✓ Referências configuradas\n" +
                "✓ 12 slots com Button components\n" +
                "✓ Outline configurado nos slots", 
                "OK"
            );
        }
    }
}
