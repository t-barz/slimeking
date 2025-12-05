using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using TheSlimeKing.UI;

/// <summary>
/// Script de editor para verificar e corrigir referências dos slots do inventário.
/// Menu: Extra Tools/Inventory/Fix Slot References
/// </summary>
public class FixInventorySlotReferences : EditorWindow
{
    [MenuItem("Extra Tools/Inventory/Fix Slot References")]
    public static void FixReferences()
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

        // Obtém todos os InventorySlotUI
        InventorySlotUI[] slots = slotsContainer.GetComponentsInChildren<InventorySlotUI>(true);

        if (slots.Length == 0)
        {
            EditorUtility.DisplayDialog("Erro", "Nenhum InventorySlotUI encontrado!", "OK");
            return;
        }

        int fixedCount = 0;
        string report = $"Verificando {slots.Length} slots...\n\n";

        foreach (InventorySlotUI slot in slots)
        {
            bool needsFix = false;
            
            // Busca Image para ícone
            Image iconImage = null;
            Image[] images = slot.GetComponentsInChildren<Image>(true);
            
            // Procura por uma Image que não seja a do próprio slot (background)
            foreach (Image img in images)
            {
                if (img.gameObject != slot.gameObject && 
                    (img.gameObject.name.Contains("Icon") || img.gameObject.name.Contains("Image")))
                {
                    iconImage = img;
                    break;
                }
            }

            // Se não encontrou, usa a segunda Image (primeira é o background)
            if (iconImage == null && images.Length > 1)
            {
                iconImage = images[1];
            }

            // Busca TextMeshProUGUI para quantidade
            TextMeshProUGUI quantityText = slot.GetComponentInChildren<TextMeshProUGUI>(true);

            // Busca Button
            Button button = slot.GetComponent<Button>();

            // Atualiza referências via SerializedObject
            SerializedObject serializedSlot = new SerializedObject(slot);
            
            SerializedProperty iconProp = serializedSlot.FindProperty("iconImage");
            SerializedProperty textProp = serializedSlot.FindProperty("quantityText");
            SerializedProperty buttonProp = serializedSlot.FindProperty("button");

            if (iconProp != null && iconImage != null && iconProp.objectReferenceValue != iconImage)
            {
                iconProp.objectReferenceValue = iconImage;
                needsFix = true;
                report += $"✅ {slot.gameObject.name}: iconImage corrigido\n";
            }

            if (textProp != null && quantityText != null && textProp.objectReferenceValue != quantityText)
            {
                textProp.objectReferenceValue = quantityText;
                needsFix = true;
                report += $"✅ {slot.gameObject.name}: quantityText corrigido\n";
            }

            if (buttonProp != null && button != null && buttonProp.objectReferenceValue != button)
            {
                buttonProp.objectReferenceValue = button;
                needsFix = true;
                report += $"✅ {slot.gameObject.name}: button corrigido\n";
            }

            if (needsFix)
            {
                serializedSlot.ApplyModifiedProperties();
                EditorUtility.SetDirty(slot);
                fixedCount++;
            }
        }

        report += $"\n{fixedCount} slots corrigidos!";
        
        UnityEngine.Debug.Log($"[FixInventorySlotReferences]\n{report}");
        EditorUtility.DisplayDialog("Concluído", report, "OK");
    }
}
