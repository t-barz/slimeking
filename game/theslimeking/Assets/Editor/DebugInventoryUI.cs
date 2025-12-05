using UnityEngine;
using UnityEditor;
using TheSlimeKing.Inventory;
using TheSlimeKing.UI;
using UnityEngine.UI;

/// <summary>
/// Script de debug para verificar o estado do inventário e UI.
/// Menu: Extra Tools/Inventory/Debug Inventory State
/// </summary>
public class DebugInventoryUI : EditorWindow
{
    [MenuItem("Extra Tools/Inventory/Debug Inventory State")]
    public static void DebugInventory()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Aviso", "Este debug só funciona em Play Mode!", "OK");
            return;
        }

        string debugInfo = "=== DEBUG INVENTÁRIO ===\n\n";

        // Verifica InventoryManager
        if (InventoryManager.Instance == null)
        {
            debugInfo += "❌ InventoryManager.Instance é NULL!\n";
        }
        else
        {
            debugInfo += "✅ InventoryManager encontrado\n";
            
            for (int i = 0; i < 12; i++)
            {
                InventorySlot slot = InventoryManager.Instance.GetSlot(i);
                if (slot != null && !slot.IsEmpty)
                {
                    debugInfo += $"  Slot {i}: {slot.item.itemName} (qty: {slot.quantity})\n";
                    debugInfo += $"    - Icon: {(slot.item.icon != null ? "✅" : "❌ NULL")}\n";
                }
            }
        }

        debugInfo += "\n";

        // Verifica InventoryUI
        SlimeKing.UI.InventoryUI inventoryUI = FindObjectOfType<SlimeKing.UI.InventoryUI>();
        
        if (inventoryUI == null)
        {
            debugInfo += "❌ InventoryUI não encontrado!\n";
        }
        else
        {
            debugInfo += "✅ InventoryUI encontrado\n";
            debugInfo += $"  - IsOpen: {inventoryUI.IsOpen}\n";
            
            // Verifica slotsContainer via reflection
            var field = typeof(SlimeKing.UI.InventoryUI).GetField("slotsContainer", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                Transform container = field.GetValue(inventoryUI) as Transform;
                debugInfo += $"  - SlotsContainer: {(container != null ? "✅" : "❌ NULL")}\n";
                
                if (container != null)
                {
                    InventorySlotUI[] slots = container.GetComponentsInChildren<InventorySlotUI>(true);
                    debugInfo += $"  - Slots encontrados: {slots.Length}\n";
                    
                    for (int i = 0; i < Mathf.Min(slots.Length, 12); i++)
                    {
                        var slotField = typeof(InventorySlotUI).GetField("iconImage", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        
                        if (slotField != null)
                        {
                            Image iconImage = slotField.GetValue(slots[i]) as Image;
                            debugInfo += $"    Slot {i}: iconImage = {(iconImage != null ? "✅" : "❌ NULL")}";
                            
                            if (iconImage != null)
                            {
                                debugInfo += $", enabled = {iconImage.enabled}, sprite = {(iconImage.sprite != null ? iconImage.sprite.name : "NULL")}";
                            }
                            debugInfo += "\n";
                        }
                    }
                }
            }
        }

        UnityEngine.Debug.Log(debugInfo);
        EditorUtility.DisplayDialog("Debug Inventário", debugInfo, "OK");
    }
}
