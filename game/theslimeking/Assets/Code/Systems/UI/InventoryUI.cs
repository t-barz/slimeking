using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TheSlimeKing.Inventory;
using TheSlimeKing.UI;

namespace SlimeKing.UI
{
    /// <summary>
    /// Gerencia a interface do inventário.
    /// Exibe 12 slots (3 linhas x 4 colunas) centralizados na tela.
    /// Sincroniza automaticamente com o InventoryManager via eventos.
    /// Controlado pelo PauseManager.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        #region Inspector Settings

        [Header("UI Panels")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Slots")]
        [SerializeField] private Transform slotsContainer;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 0.3f;

        [Header("Debug")]
        [SerializeField] private bool enableInventoryLogs = false;

        #endregion

        #region Private Fields

        private Coroutine fadeCoroutine;
        private bool isOpen = false;
        private InventorySlotUI[] slotUIComponents = new InventorySlotUI[12];

        #endregion

        #region Properties

        public bool IsOpen => isOpen;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Valida referências
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    UnityEngine.Debug.LogError("[InventoryUI] CanvasGroup not found! Adding component.");
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            // Estado inicial: oculto
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void Start()
        {
            InitializeSlots();
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            UnsubscribeFromEvents();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Abre o inventário com fade in e sincroniza com o estado atual.
        /// </summary>
        public void Show()
        {
            if (isOpen) return;

            LogMessage("Showing inventory");
            isOpen = true;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
            }

            // Sincroniza UI com estado atual do inventário
            RefreshAllSlots();

            // Para fade anterior se existir
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeIn());
        }

        /// <summary>
        /// Fecha o inventário com fade out.
        /// </summary>
        public void Hide()
        {
            if (!isOpen) return;

            LogMessage("Hiding inventory");
            isOpen = false;

            // Para fade anterior se existir
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeOut());
        }

        #endregion

        #region Fade Animations

        /// <summary>
        /// Corrotina de fade in (alpha 0 → 1).
        /// Usa Time.unscaledDeltaTime para funcionar durante pause.
        /// </summary>
        private IEnumerator FadeIn()
        {
            float elapsed = 0f;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;

            fadeCoroutine = null;
            LogMessage("Fade in completed");
        }

        /// <summary>
        /// Corrotina de fade out (alpha 1 → 0).
        /// Usa Time.unscaledDeltaTime para funcionar durante pause.
        /// </summary>
        private IEnumerator FadeOut()
        {
            float elapsed = 0f;

            canvasGroup.interactable = false;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            fadeCoroutine = null;
            LogMessage("Fade out completed");
        }

        #endregion

        #region Slot Management

        /// <summary>
        /// Inicializa as referências aos 12 slots de UI.
        /// Obtém os componentes InventorySlotUI dos filhos do container.
        /// </summary>
        private void InitializeSlots()
        {
            if (slotsContainer == null)
            {
                UnityEngine.Debug.LogError("[InventoryUI] Slots container não configurado!");
                return;
            }

            // Obtém todos os InventorySlotUI do container
            InventorySlotUI[] foundSlots = slotsContainer.GetComponentsInChildren<InventorySlotUI>(true);

            if (foundSlots.Length < 12)
            {
                UnityEngine.Debug.LogError($"[InventoryUI] Esperado 12 slots, encontrados {foundSlots.Length}!");
                return;
            }

            // Armazena referências aos primeiros 12 slots
            for (int i = 0; i < 12; i++)
            {
                slotUIComponents[i] = foundSlots[i];
            }

            LogMessage($"Inicializados {slotUIComponents.Length} slots");
        }

        /// <summary>
        /// Inscreve-se nos eventos do InventoryManager.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (InventoryManager.Instance == null)
            {
                UnityEngine.Debug.LogError("[InventoryUI] InventoryManager.Instance não encontrado!");
                return;
            }

            InventoryManager.Instance.OnInventoryChanged += RefreshAllSlots;
            LogMessage("Inscrito nos eventos do InventoryManager");
        }

        /// <summary>
        /// Desinscreve-se dos eventos do InventoryManager.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged -= RefreshAllSlots;
                LogMessage("Desinscrito dos eventos do InventoryManager");
            }
        }

        /// <summary>
        /// Atualiza todos os 12 slots com o estado atual do inventário.
        /// </summary>
        private void RefreshAllSlots()
        {
            if (InventoryManager.Instance == null)
            {
                UnityEngine.Debug.LogWarning("[InventoryUI] InventoryManager.Instance não disponível para refresh");
                return;
            }

            UnityEngine.Debug.Log($"[InventoryUI] 🔄 Atualizando todos os slots. UI está {(isOpen ? "ABERTA" : "FECHADA")}");

            for (int i = 0; i < 12; i++)
            {
                if (slotUIComponents[i] != null)
                {
                    InventorySlot slot = InventoryManager.Instance.GetSlot(i);
                    slotUIComponents[i].Setup(slot, i);
                    
                    if (!slot.IsEmpty)
                    {
                        UnityEngine.Debug.Log($"[InventoryUI] ✅ Slot {i} atualizado com '{slot.item.itemName}'");
                    }
                }
            }

            LogMessage("Todos os slots atualizados");
        }

        #endregion

        #region Slot Interaction

        /// <summary>
        /// Callback quando um slot é clicado.
        /// </summary>
        public void OnSlotClicked(int slotIndex)
        {
            LogMessage($"Slot {slotIndex} clicked");
            // Futura implementação: usar item, equipar, etc.
        }

        #endregion

        #region Logging

        private void LogMessage(string message)
        {
            if (enableInventoryLogs)
            {
                UnityEngine.Debug.Log($"[InventoryUI] {message}");
            }
        }

        #endregion
    }
}
