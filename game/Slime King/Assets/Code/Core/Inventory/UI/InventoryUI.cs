using UnityEngine;
using System.Collections.Generic;

namespace SlimeKing.Core.Inventory.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Transform slotsContainer;

        private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();

        private void Start()
        {
            if (inventory == null)
            {
                inventory = GetComponent<Inventory>();
                if (inventory == null)
                {
                    Debug.LogError("InventoryUI: Nenhum componente Inventory encontrado!");
                    return;
                }
            }

            inventory.OnSizeChanged += UpdateSlots;
            UpdateSlots(inventory.Size);
        }

        private void OnDestroy()
        {
            if (inventory != null)
                inventory.OnSizeChanged -= UpdateSlots;
        }

        private void UpdateSlots(int newSize)
        {
            // Remove slots excedentes
            while (slotUIs.Count > newSize)
            {
                var slot = slotUIs[slotUIs.Count - 1];
                slotUIs.RemoveAt(slotUIs.Count - 1);
                Destroy(slot.gameObject);
            }

            // Adiciona novos slots necessários
            while (slotUIs.Count < newSize)
            {
                var slotGO = Instantiate(slotPrefab, slotsContainer);
                var slotUI = slotGO.GetComponent<InventorySlotUI>();

                if (slotUI == null)
                {
                    Debug.LogError("InventoryUI: Prefab do slot não contém o componente InventorySlotUI!");
                    continue;
                }

                slotUIs.Add(slotUI);
            }

            // Atualiza todos os slots
            for (int i = 0; i < slotUIs.Count; i++)
            {
                slotUIs[i].Initialize(inventory.GetSlot(i));
            }
        }
    }
}
