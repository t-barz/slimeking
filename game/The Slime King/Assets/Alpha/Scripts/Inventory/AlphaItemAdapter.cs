using UnityEngine;
using System;
using SlimeMec.Alpha.Inventory;
// Adiciona namespace legacy onde ItemCollectable está definido
using SlimeMec.Gameplay; // Necessário para acessar ItemCollectable em Assets/_OLD/_Scripts/Gameplay

namespace SlimeMec.Alpha.Inventory
{
    /// <summary>
    /// Adapter que integra ItemCollectable existente com o sistema Alpha de inventário
    /// sem modificar o código original. Atua como bridge entre os sistemas.
    /// </summary>
    public class AlphaItemAdapter : MonoBehaviour
    {
        #region Events
        public static event Action<string, int> OnItemCollectedForInventory;
        #endregion

        #region Serialized Fields
        [Header("Item Configuration")]
        [SerializeField] private string itemId = "default_item";
        [SerializeField] private int quantity = 1;
        [SerializeField] private bool autoAddToInventory = true;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        #endregion

        #region Components
        private ItemCollectable originalItemCollectable; // Referência ao componente legado
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
            SetupEventListeners();
        }

        private void OnDestroy()
        {
            CleanupEventListeners();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            // Tenta obter componente legacy
            originalItemCollectable = GetComponent<ItemCollectable>();
            if (originalItemCollectable == null && enableDebugLogs)
            {
                Debug.LogWarning($"[AlphaItemAdapter] ItemCollectable não encontrado em {gameObject.name}. Adapter ainda funcionará via chamada manual (OnItemCollectedByPlayer). Se este item deveria ser coletável, verifique se o script legacy está presente.");
            }
        }

        private void SetupEventListeners()
        {
            // TODO: Conectar aos eventos do ItemCollectable quando disponíveis
            // Exemplo: ItemCollectable.OnItemCollected += HandleItemCollected;
        }

        private void CleanupEventListeners()
        {
            // TODO: Desconectar eventos
            // Exemplo: ItemCollectable.OnItemCollected -= HandleItemCollected;
        }
        #endregion

        #region Item Collection Integration
        /// <summary>
        /// Método que deve ser chamado quando o ItemCollectable original for coletado
        /// </summary>
        public void HandleItemCollected()
        {
            if (autoAddToInventory)
            {
                AddToInventory();
            }

            if (enableDebugLogs)
                Debug.Log($"AlphaItemAdapter: Item {itemId} collected, quantity: {quantity}");
        }

        /// <summary>
        /// Adiciona o item ao sistema de inventário Alpha
        /// </summary>
        private void AddToInventory()
        {
            if (InventoryCore.Instance != null)
            {
                bool success = InventoryCore.Instance.AddItem(itemId, quantity);

                if (success)
                {
                    OnItemCollectedForInventory?.Invoke(itemId, quantity);

                    if (enableDebugLogs)
                        Debug.Log($"AlphaItemAdapter: Successfully added {quantity}x {itemId} to inventory");
                }
                else
                {
                    if (enableDebugLogs)
                        Debug.LogWarning($"AlphaItemAdapter: Failed to add {itemId} to inventory - possibly full");
                }
            }
            else
            {
                if (enableDebugLogs)
                    Debug.LogError("AlphaItemAdapter: InventoryCore instance not found!");
            }
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Configura os dados do item dinamicamente
        /// </summary>
        public void SetItemData(string id, int qty)
        {
            itemId = id;
            quantity = qty;

            if (enableDebugLogs)
                Debug.Log($"AlphaItemAdapter: Item data set to {qty}x {id}");
        }

        /// <summary>
        /// Força a coleta do item (útil para testes)
        /// </summary>
        [ContextMenu("Force Collect Item")]
        public void ForceCollectItem()
        {
            HandleItemCollected();
        }
        #endregion

        #region Integration Helpers
        /// <summary>
        /// Método para ser chamado pelo ItemCollectable via UnityEvent ou código
        /// </summary>
        public void OnItemCollectedByPlayer()
        {
            HandleItemCollected();
        }

        /// <summary>
        /// Obtém informações do item para o sistema de UI
        /// </summary>
        public (string id, int quantity) GetItemInfo()
        {
            return (itemId, quantity);
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmosSelected()
        {
            if (!enabled) return;

            // Desenha ícone de inventário quando selecionado
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);

            // Label com informações do item
#if UNITY_EDITOR
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 0.7f,
                $"Item: {itemId}\nQty: {quantity}"
            );
#endif
        }
        #endregion
    }
}