using UnityEngine;

namespace TheSlimeKing.Core.Inventory
{
    /// <summary>
    /// Editor de teste para o sistema de inventário
    /// </summary>
    public class InventoryTester : MonoBehaviour
    {
        [Header("Itens de Teste")]
        [SerializeField] private ItemData[] _testItems;

        [Header("Configuração")]
        [SerializeField] private KeyCode _addItemKey = KeyCode.I;
        [SerializeField] private KeyCode _removeItemKey = KeyCode.O;
        [SerializeField] private KeyCode _dropItemKey = KeyCode.P;
        [SerializeField] private KeyCode _toggleInventoryKey = KeyCode.Tab;
        [SerializeField] private KeyCode _increaseSlotKey = KeyCode.PageUp;
        [SerializeField] private KeyCode _decreaseSlotKey = KeyCode.PageDown;

        [Header("Prefabs")]
        [SerializeField] private GameObject _itemDropPrefab;

        // Referencias
        private InventoryUI _inventoryUI;

        private void Start()
        {
            // Encontrar UI do inventário
            _inventoryUI = FindObjectOfType<InventoryUI>();
        }

        private void Update()
        {
            // Processar input para testes
            if (InventoryManager.Instance == null)
                return;

            // Adicionar item aleatório
            if (Input.GetKeyDown(_addItemKey) && _testItems != null && _testItems.Length > 0)
            {
                int randomIndex = Random.Range(0, _testItems.Length);
                int randomQuantity = Random.Range(1, 5);

                if (InventoryManager.Instance.AddItem(_testItems[randomIndex], randomQuantity))
                {
                    Debug.Log($"Item adicionado: {_testItems[randomIndex].name} x{randomQuantity}");
                }
                else
                {
                    Debug.Log("Não foi possível adicionar o item. Inventário cheio?");
                }
            }

            // Remover item selecionado
            if (Input.GetKeyDown(_removeItemKey))
            {
                InventoryItem selectedItem = InventoryManager.Instance.GetSelectedItem();
                if (selectedItem != null && !selectedItem.IsEmpty)
                {
                    if (InventoryManager.Instance.RemoveItem(selectedItem.ItemData.ItemId, 1))
                    {
                        Debug.Log($"Item removido: {selectedItem.ItemData.name}");
                    }
                }
                else
                {
                    Debug.Log("Nenhum item selecionado para remover");
                }
            }

            // Dropar item no mundo
            if (Input.GetKeyDown(_dropItemKey))
            {
                InventoryItem selectedItem = InventoryManager.Instance.GetSelectedItem();
                if (selectedItem != null && !selectedItem.IsEmpty && _itemDropPrefab != null)
                {
                    // Remover do inventário
                    if (InventoryManager.Instance.RemoveItem(selectedItem.ItemData.ItemId, 1))
                    {
                        // Criar item no mundo
                        GameObject itemObj = Instantiate(_itemDropPrefab,
                            transform.position + new Vector3(0, 0.5f, 0),
                            Quaternion.identity);

                        ItemWorldDrop itemDrop = itemObj.GetComponent<ItemWorldDrop>();
                        if (itemDrop != null)
                        {
                            itemDrop.SetItem(selectedItem.ItemData, 1);

                            // Aplicar força aleatória
                            Vector2 randomDirection = new Vector2(
                                Random.Range(-1f, 1f),
                                Random.Range(0.5f, 1f)
                            );

                            itemDrop.ApplyInitialForce(randomDirection, 2f);

                            Debug.Log($"Item dropado no mundo: {selectedItem.ItemData.name}");
                        }
                    }
                }
            }

            // Alternar interface do inventário
            if (Input.GetKeyDown(_toggleInventoryKey) && _inventoryUI != null)
            {
                _inventoryUI.ToggleInventory();
            }

            // Aumentar slots disponíveis (para teste)
            if (Input.GetKeyDown(_increaseSlotKey))
            {
                int currentSlots = InventoryManager.Instance.GetAvailableSlots();
                InventoryManager.Instance.SetAvailableSlots(currentSlots + 1);
                Debug.Log($"Slots aumentados para {InventoryManager.Instance.GetAvailableSlots()}");
            }

            // Diminuir slots disponíveis (para teste)
            if (Input.GetKeyDown(_decreaseSlotKey))
            {
                int currentSlots = InventoryManager.Instance.GetAvailableSlots();
                InventoryManager.Instance.SetAvailableSlots(currentSlots - 1);
                Debug.Log($"Slots reduzidos para {InventoryManager.Instance.GetAvailableSlots()}");
            }
        }
    }
}
