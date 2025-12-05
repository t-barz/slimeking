using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using TheSlimeKing.UI;

/// <summary>
/// Editor script para configurar automaticamente os 12 slots do inventário.
/// Menu: Extra Tools/Inventory/Configure 12 Slots
/// </summary>
public class ConfigureInventorySlots : EditorWindow
{
    [MenuItem("Extra Tools/Inventory/Configure 12 Slots")]
    public static void ConfigureSlots()
    {
        // Busca o InventoryUI na cena
        SlimeKing.UI.InventoryUI inventoryUI = FindObjectOfType<SlimeKing.UI.InventoryUI>();
        
        if (inventoryUI == null)
        {
            EditorUtility.DisplayDialog("Erro", "InventoryUI não encontrado na cena!", "OK");
            return;
        }

        // Busca o container de slots existente
        Transform slotsContainer = inventoryUI.transform.Find("InventoryPanel/SlotsContainer");
        
        if (slotsContainer == null)
        {
            EditorUtility.DisplayDialog("Erro", "SlotsContainer não encontrado! Estrutura esperada: InventoryUI/InventoryPanel/SlotsContainer", "OK");
            return;
        }

        // Obtém todos os slots existentes
        InventorySlotUI[] existingSlots = slotsContainer.GetComponentsInChildren<InventorySlotUI>(true);

        if (existingSlots.Length == 0)
        {
            EditorUtility.DisplayDialog("Erro", "Nenhum InventorySlotUI encontrado! A UI já deve ter os slots criados.", "OK");
            return;
        }

        if (existingSlots.Length != 12)
        {
            EditorUtility.DisplayDialog("Aviso", $"Encontrados {existingSlots.Length} slots. Esperado: 12 slots.\nVerifique a UI manualmente.", "OK");
        }

        // Conecta referência do container no InventoryUI
        SerializedObject serializedInventoryUI = new SerializedObject(inventoryUI);
        SerializedProperty slotsContainerProp = serializedInventoryUI.FindProperty("slotsContainer");
        slotsContainerProp.objectReferenceValue = slotsContainer;
        serializedInventoryUI.ApplyModifiedProperties();

        EditorUtility.SetDirty(inventoryUI);
        UnityEngine.Debug.Log($"[ConfigureInventorySlots] Configuração concluída! {existingSlots.Length} slots conectados ao InventoryUI.");
        EditorUtility.DisplayDialog("Sucesso", $"Configuração concluída!\n{existingSlots.Length} slots conectados ao InventoryUI.", "OK");
    }


}
