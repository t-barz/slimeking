using UnityEngine;
using UnityEditor;
using TMPro;
using TheSlimeKing.UI;

/// <summary>
/// Editor script para remover/desabilitar os textos de quantidade dos slots do inventário.
/// Como o sistema não empilha itens, o texto de quantidade não é necessário.
/// Menu: Extra Tools/Inventory/Remove Quantity Text
/// </summary>
public class RemoveInventoryQuantityText : EditorWindow
{
    [MenuItem("Extra Tools/Inventory/Remove Quantity Text")]
    public static void RemoveQuantityText()
    {
        // Busca o InventoryUI na cena
        SlimeKing.UI.InventoryUI inventoryUI = FindObjectOfType<SlimeKing.UI.InventoryUI>();
        
        if (inventoryUI == null)
        {
            EditorUtility.DisplayDialog("Erro", "InventoryUI não encontrado na cena!", "OK");
            return;
        }

        // Busca o container de slots
        Transform slotsContainer = inventoryUI.transform.Find("InventoryPanel/SlotsContainer");
        
        if (slotsContainer == null)
        {
            EditorUtility.DisplayDialog("Erro", "SlotsContainer não encontrado!", "OK");
            return;
        }

        // Obtém todos os slots
        InventorySlotUI[] slots = slotsContainer.GetComponentsInChildren<InventorySlotUI>(true);

        if (slots.Length == 0)
        {
            EditorUtility.DisplayDialog("Erro", "Nenhum slot encontrado!", "OK");
            return;
        }

        int removedCount = 0;
        int disabledCount = 0;

        foreach (InventorySlotUI slot in slots)
        {
            // Busca o TextMeshProUGUI no slot
            TextMeshProUGUI[] texts = slot.GetComponentsInChildren<TextMeshProUGUI>(true);
            
            foreach (TextMeshProUGUI text in texts)
            {
                // Verifica se é o texto de quantidade (geralmente chamado "Quantity" ou similar)
                if (text.gameObject.name.Contains("Quantity") || 
                    text.gameObject.name.Contains("Amount") ||
                    text.gameObject.name.Contains("Count"))
                {
                    // Opção 1: Desabilitar o GameObject (recomendado - mantém a estrutura)
                    text.gameObject.SetActive(false);
                    disabledCount++;
                    
                    // Opção 2: Destruir o GameObject (descomente se preferir remover completamente)
                    // DestroyImmediate(text.gameObject);
                    // removedCount++;
                    
                    EditorUtility.SetDirty(text.gameObject);
                }
            }
            
            EditorUtility.SetDirty(slot.gameObject);
        }

        string message = $"Processados {slots.Length} slots.\n";
        
        if (disabledCount > 0)
        {
            message += $"Desabilitados {disabledCount} textos de quantidade.";
        }
        
        if (removedCount > 0)
        {
            message += $"Removidos {removedCount} textos de quantidade.";
        }
        
        if (disabledCount == 0 && removedCount == 0)
        {
            message += "Nenhum texto de quantidade encontrado.";
        }

        UnityEngine.Debug.Log($"[RemoveInventoryQuantityText] {message}");
        EditorUtility.DisplayDialog("Concluído", message, "OK");
    }
}
