using UnityEngine;
using UnityEngine.InputSystem;
using TheSlimeKing.UI;
using SlimeKing.Core;

namespace TheSlimeKing.Inventory
{
    /// <summary>
    /// Gerencia os 4 quick slots mapeados aos Skill1-4 do InputSystem.
    /// Detecta input e usa itens atribuídos aos quick slots.
    /// Atualiza a UI dos quick slots no HUD.
    /// </summary>
    public class QuickSlotManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private QuickSlotUI[] quickSlotUIs = new QuickSlotUI[4];

        private InputSystem_Actions inputActions;

        #region Unity Lifecycle
        private void Awake()
        {
            // Inicializar InputSystem_Actions
            inputActions = new InputSystem_Actions();
        }

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

        private void OnEnable()
        {
            // Habilitar mapa Gameplay e subscrever aos eventos Skill
            inputActions.Gameplay.Enable();
            inputActions.Gameplay.Skill1.performed += OnSkill1Input;
            inputActions.Gameplay.Skill2.performed += OnSkill2Input;
            inputActions.Gameplay.Skill3.performed += OnSkill3Input;
            inputActions.Gameplay.Skill4.performed += OnSkill4Input;

            // Subscrever ao evento de pause para desabilitar/habilitar quick slots
            if (PauseManager.Instance != null)
            {
                PauseManager.OnPauseStateChanged += OnPauseStateChanged;
            }
        }

        private void OnDisable()
        {
            // Desinscrever dos eventos Skill
            inputActions.Gameplay.Skill1.performed -= OnSkill1Input;
            inputActions.Gameplay.Skill2.performed -= OnSkill2Input;
            inputActions.Gameplay.Skill3.performed -= OnSkill3Input;
            inputActions.Gameplay.Skill4.performed -= OnSkill4Input;
            inputActions.Gameplay.Disable();

            // Desinscrever do evento de pause
            if (PauseManager.Instance != null)
            {
                PauseManager.OnPauseStateChanged -= OnPauseStateChanged;
            }
        }

        private void OnDestroy()
        {
            // Desinscrever-se dos eventos
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
                InventoryManager.Instance.OnQuickSlotsChanged -= RefreshUI;
            }

            inputActions?.Dispose();
        }
        #endregion

        #region Input Handlers
        private void OnSkill1Input(InputAction.CallbackContext context)
        {
            InventoryManager.Instance?.UseQuickSlot(0);
        }

        private void OnSkill2Input(InputAction.CallbackContext context)
        {
            InventoryManager.Instance?.UseQuickSlot(1);
        }

        private void OnSkill3Input(InputAction.CallbackContext context)
        {
            InventoryManager.Instance?.UseQuickSlot(2);
        }

        private void OnSkill4Input(InputAction.CallbackContext context)
        {
            InventoryManager.Instance?.UseQuickSlot(3);
        }

        /// <summary>
        /// Desabilita/habilita os Skill inputs quando o jogo está pausado/despausado.
        /// Isso previne que os itens sejam usados quando o inventário está aberto.
        /// </summary>
        private void OnPauseStateChanged(bool isPaused)
        {
            if (isPaused)
            {
                // Jogo pausado (inventário aberto) - desabilitar uso de quick slots
                inputActions.Gameplay.Disable();
                UnityEngine.Debug.Log("[QuickSlotManager] Gameplay desabilitado (inventário aberto)");
            }
            else
            {
                // Jogo despausado - habilitar uso de quick slots
                inputActions.Gameplay.Enable();
                UnityEngine.Debug.Log("[QuickSlotManager] Gameplay habilitado (inventário fechado)");
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
            UnityEngine.Debug.Log($"[QuickSlotManager] RefreshUI chamado. Quick slots configurados: {quickSlotUIs.Length}");

            for (int i = 0; i < quickSlotUIs.Length; i++)
            {
                if (quickSlotUIs[i] != null)
                {
                    UnityEngine.Debug.Log($"[QuickSlotManager] Atualizando quick slot {i}");
                    quickSlotUIs[i].Refresh();
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"[QuickSlotManager] Quick slot UI {i} é NULL!");
                }
            }
        }
        #endregion
    }
}
