using UnityEngine;
using TheSlimeKing.UI;

namespace TheSlimeKing.Inventory
{
    /// <summary>
    /// Gerencia os 4 quick slots mapeados aos direcionais do gamepad.
    /// Detecta input e usa itens atribuídos aos direcionais.
    /// Atualiza a UI dos quick slots no HUD.
    /// </summary>
    public class QuickSlotManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private QuickSlotUI[] quickSlotUIs = new QuickSlotUI[4];

        #region Unity Lifecycle
        private void Start()
        {
            // Inscrever-se nos eventos do InventoryManager
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged += RefreshUI;
                InventoryManager.Instance.OnQuickSlotsChanged += RefreshUI;
            }

            // Configurar direções dos quick slots
            for (int i = 0; i < quickSlotUIs.Length; i++)
            {
                if (quickSlotUIs[i] != null)
                {
                    quickSlotUIs[i].SetDirection(i);
                }
            }

            // Atualizar UI inicial
            RefreshUI();
        }

        private void Update()
        {
            DetectQuickSlotInput();
        }

        private void OnDestroy()
        {
            // Desinscrever-se dos eventos
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
                InventoryManager.Instance.OnQuickSlotsChanged -= RefreshUI;
            }
        }
        #endregion

        #region Input Detection
        /// <summary>
        /// Detecta input dos 4 direcionais e usa o item correspondente.
        /// </summary>
        private void DetectQuickSlotInput()
        {
            // Direcional Cima (Up) - Quick Slot 0
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                InventoryManager.Instance?.UseQuickSlot(0);
            }
            // Direcional Baixo (Down) - Quick Slot 1
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                InventoryManager.Instance?.UseQuickSlot(1);
            }
            // Direcional Esquerda (Left) - Quick Slot 2
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                InventoryManager.Instance?.UseQuickSlot(2);
            }
            // Direcional Direita (Right) - Quick Slot 3
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                InventoryManager.Instance?.UseQuickSlot(3);
            }
        }
        #endregion

        #region UI Management
        /// <summary>
        /// Atualiza a UI de todos os quick slots.
        /// Limpa slot se item não existir mais no inventário.
        /// </summary>
        public void RefreshUI()
        {
            for (int i = 0; i < quickSlotUIs.Length; i++)
            {
                if (quickSlotUIs[i] != null)
                {
                    quickSlotUIs[i].Refresh();
                }
            }
        }
        #endregion
    }
}
