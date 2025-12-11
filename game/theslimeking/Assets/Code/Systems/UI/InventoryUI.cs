using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections;
using TMPro;
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

        [Header("Item Details Panel")]
        [SerializeField] private GameObject itemDetailsPanel;
        [SerializeField] private Image detailsIconImage;
        [SerializeField] private TextMeshProUGUI detailsTitleText;
        [SerializeField] private TextMeshProUGUI detailsDescriptionText;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 0.3f;

        [Header("Debug")]
        [SerializeField] private bool enableInventoryLogs = false;

        #endregion

        #region Private Fields

        private Coroutine fadeCoroutine;
        private bool isOpen = false;
        private InventorySlotUI[] slotUIComponents = new InventorySlotUI[12];
        private int currentSelectedIndex = -1;
        private InputSystem_Actions inputActions;
        private bool isInputSubscribed = false;
        private float timeWhenOpened = -999f; // Timestamp de quando o inventário foi aberto
        private int swapFirstSlotIndex = -1; // Índice do primeiro slot selecionado para swap (-1 = nenhum)

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
            InitializeInputSystem();

            // Inscreve-se no evento de seleção de slot
            InventorySlotUI.OnSlotSelected += UpdateItemDetails;

            // Oculta painel de detalhes inicialmente
            if (itemDetailsPanel != null)
            {
                itemDetailsPanel.SetActive(false);
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            UnsubscribeFromEvents();
            InventorySlotUI.OnSlotSelected -= UpdateItemDetails;
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
            timeWhenOpened = Time.realtimeSinceStartup; // Registra o tempo em que foi aberto
            currentSelectedIndex = -1;

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

            // Habilitar mapa InventoryNavigation
            if (inputActions != null)
            {
                inputActions.InventoryNavigation.Enable();
                LogMessage("InventoryNavigation map enabled");
            }
        }

        /// <summary>
        /// Fecha o inventário com fade out.
        /// Retorna false se o inventário foi aberto muito recentemente (proteção contra fechamento duplo).
        /// </summary>
        public bool Hide()
        {
            if (!isOpen) return false;

            // Proteção: não fechar se foi aberto há menos de 0.1 segundos
            // Isso previne transições acidentais logo após abrir
            float timeSinceOpened = Time.realtimeSinceStartup - timeWhenOpened;
            if (timeSinceOpened < 0.1f)
            {
                LogMessage($"Hide() called too soon after Show() ({timeSinceOpened:F3}s). Ignoring to prevent accidental close.");
                return false;
            }

            LogMessage("Hiding inventory");
            isOpen = false;

            // Desabilitar mapa InventoryNavigation
            if (inputActions != null)
            {
                inputActions.InventoryNavigation.Disable();
                LogMessage("InventoryNavigation map disabled");
            }

            // Para fade anterior se existir
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeOut());
            return true;
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

            // Seleciona o primeiro slot com item
            SelectFirstAvailableSlot();

            // Ativa input de navegação
            EnableNavigationInput();

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

            // Desativa input de navegação
            DisableNavigationInput();

            // Deseleciona o slot atual
            if (currentSelectedIndex >= 0 && currentSelectedIndex < 12)
            {
                slotUIComponents[currentSelectedIndex].SetSelected(false);
            }
            currentSelectedIndex = -1;

            // Limpa o estado de swap
            if (swapFirstSlotIndex >= 0 && swapFirstSlotIndex < 12)
            {
                slotUIComponents[swapFirstSlotIndex].SetSwapSelected(false);
            }
            swapFirstSlotIndex = -1;

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

        #region Navigation

        /// <summary>
        /// Inicializa o InputSystem_Actions para navegação do inventário.
        /// </summary>
        private void InitializeInputSystem()
        {
            if (PlayerController.Instance != null)
            {
                inputActions = PlayerController.Instance.GetInputActions();

                if (inputActions != null)
                {
                    LogMessage("Input system connected for inventory navigation");
                }
                else
                {
                    UnityEngine.Debug.LogWarning("[InventoryUI] Failed to get InputActions from PlayerController");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("[InventoryUI] PlayerController.Instance not found during Initialize");
            }
        }

        /// <summary>
        /// Habilita o input de navegação do inventário (mapa UI.Navigate) e quick slot assignment.
        /// </summary>
        private void EnableNavigationInput()
        {
            if (inputActions == null || isInputSubscribed)
                return;

            try
            {
                inputActions.InventoryNavigation.Navigate.performed += OnNavigateInput;
                inputActions.InventoryNavigation.SelectItem.performed += OnSubmitInput;
                inputActions.InventoryNavigation.CloseInventory.performed += OnCancelInput;

                // Subscibe aos botões de atribuição rápida
                inputActions.InventoryNavigation.AddToSlot1.performed += (ctx) => OnAssignToQuickSlot(0);
                inputActions.InventoryNavigation.AddToSlot2.performed += (ctx) => OnAssignToQuickSlot(1);
                inputActions.InventoryNavigation.AddToSlot3.performed += (ctx) => OnAssignToQuickSlot(2);
                inputActions.InventoryNavigation.AddToSlot4.performed += (ctx) => OnAssignToQuickSlot(3);

                isInputSubscribed = true;
                LogMessage("Navigation input enabled");
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogWarning($"[InventoryUI] Failed to subscribe to Navigate input: {ex.Message}");
            }
        }

        /// <summary>
        /// Desabilita o input de navegação do inventário.
        /// </summary>
        private void DisableNavigationInput()
        {
            if (inputActions == null || !isInputSubscribed)
                return;

            try
            {
                inputActions.InventoryNavigation.Navigate.performed -= OnNavigateInput;
                inputActions.InventoryNavigation.SelectItem.performed -= OnSubmitInput;
                inputActions.InventoryNavigation.CloseInventory.performed -= OnCancelInput;

                // Unsubscribe dos botões de atribuição rápida
                inputActions.InventoryNavigation.AddToSlot1.performed -= (ctx) => OnAssignToQuickSlot(0);
                inputActions.InventoryNavigation.AddToSlot2.performed -= (ctx) => OnAssignToQuickSlot(1);
                inputActions.InventoryNavigation.AddToSlot3.performed -= (ctx) => OnAssignToQuickSlot(2);
                inputActions.InventoryNavigation.AddToSlot4.performed -= (ctx) => OnAssignToQuickSlot(3);

                isInputSubscribed = false;
                LogMessage("Navigation input disabled");
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogWarning($"[InventoryUI] Failed to unsubscribe from Navigate input: {ex.Message}");
            }
        }

        /// <summary>
        /// Callback para o input de navegação (cima/baixo/esquerda/direita).
        /// Implementa navegação linear (apenas cima/baixo) pulando slots vazios.
        /// </summary>
        private void OnNavigateInput(InputAction.CallbackContext context)
        {
            if (!isOpen)
                return;

            Vector2 direction = context.ReadValue<Vector2>();

            if (direction.y > 0.5f) // Cima
            {
                NavigateUp();
            }
            else if (direction.y < -0.5f) // Baixo
            {
                NavigateDown();
            }
            else if (direction.x > 0.5f) // Direita
            {
                NavigateRight();
            }
            else if (direction.x < -0.5f) // Esquerda
            {
                NavigateLeft();
            }
        }

        /// <summary>
        /// Navega para cima (slot anterior).
        /// </summary>
        private void NavigateUp()
        {
            if (currentSelectedIndex < 0)
            {
                SelectFirstAvailableSlot();
                return;
            }

            int newIndex = currentSelectedIndex - 4; // Grid 3 colunas: -4 linhas

            if (newIndex < 0)
            {
                // Fica no mesmo se não há espaço acima
                return;
            }

            SelectSlot(newIndex, skipEmpty: true);
        }

        /// <summary>
        /// Navega para baixo (próximo slot).
        /// </summary>
        private void NavigateDown()
        {
            if (currentSelectedIndex < 0)
            {
                SelectFirstAvailableSlot();
                return;
            }

            int newIndex = currentSelectedIndex + 4; // Grid 3 colunas: +4 linhas

            if (newIndex >= 12)
            {
                // Fica no mesmo se não há espaço abaixo
                return;
            }

            SelectSlot(newIndex, skipEmpty: true);
        }

        /// <summary>
        /// Navega para a direita (próximo slot na mesma linha).
        /// </summary>
        private void NavigateRight()
        {
            if (currentSelectedIndex < 0)
            {
                SelectFirstAvailableSlot();
                return;
            }

            int row = currentSelectedIndex / 4;
            int col = currentSelectedIndex % 4;

            col++;
            if (col >= 4)
            {
                // Fica na mesma coluna se está na borda direita
                return;
            }

            int newIndex = row * 4 + col;
            SelectSlot(newIndex, skipEmpty: true);
        }

        /// <summary>
        /// Navega para a esquerda (slot anterior na mesma linha).
        /// </summary>
        private void NavigateLeft()
        {
            if (currentSelectedIndex < 0)
            {
                SelectFirstAvailableSlot();
                return;
            }

            int row = currentSelectedIndex / 4;
            int col = currentSelectedIndex % 4;

            col--;
            if (col < 0)
            {
                // Fica na mesma coluna se está na borda esquerda
                return;
            }

            int newIndex = row * 4 + col;
            SelectSlot(newIndex, skipEmpty: true);
        }

        /// <summary>
        /// Seleciona o primeiro slot disponível (com item ou vazio, dependendo do filtro).
        /// </summary>
        private void SelectFirstAvailableSlot()
        {
            for (int i = 0; i < 12; i++)
            {
                if (slotUIComponents[i] != null)
                {
                    SelectSlot(i, skipEmpty: false);
                    return;
                }
            }
        }

        /// <summary>
        /// Seleciona um slot específico por índice.
        /// </summary>
        /// <param name="index">Índice do slot (0-11)</param>
        /// <param name="skipEmpty">Se true, pula para o próximo slot não-vazio se o slot for vazio</param>
        private void SelectSlot(int index, bool skipEmpty = false)
        {
            // Valida índice
            if (index < 0 || index >= 12)
            {
                UnityEngine.Debug.LogWarning($"[InventoryUI] Tentativa de selecionar slot inválido: {index}");
                return;
            }

            // Se skipEmpty está ativo e o slot é vazio, encontra o próximo não-vazio
            if (skipEmpty && InventoryManager.Instance != null)
            {
                InventorySlot slot = InventoryManager.Instance.GetSlot(index);
                if (slot != null && slot.IsEmpty)
                {
                    // Procura para baixo primeiro
                    for (int i = index + 1; i < 12; i++)
                    {
                        if (InventoryManager.Instance.GetSlot(i) != null && !InventoryManager.Instance.GetSlot(i).IsEmpty)
                        {
                            index = i;
                            break;
                        }
                    }
                    // Se não encontrou para baixo, procura para cima
                    if (InventoryManager.Instance.GetSlot(index).IsEmpty)
                    {
                        for (int i = index - 1; i >= 0; i--)
                        {
                            if (InventoryManager.Instance.GetSlot(i) != null && !InventoryManager.Instance.GetSlot(i).IsEmpty)
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                }
            }

            // Deseleciona o slot anterior (mas não se ele está em modo swap)
            if (currentSelectedIndex >= 0 && currentSelectedIndex < 12 && slotUIComponents[currentSelectedIndex] != null)
            {
                // Só remove a seleção se não é o slot em modo swap
                if (currentSelectedIndex != swapFirstSlotIndex)
                {
                    slotUIComponents[currentSelectedIndex].SetSelected(false);
                }
            }

            // Seleciona o novo slot
            if (slotUIComponents[index] != null)
            {
                currentSelectedIndex = index;
                // Só muda a cor se o novo slot não está em modo swap
                if (index != swapFirstSlotIndex)
                {
                    slotUIComponents[index].SetSelected(true);
                }
                LogMessage($"Selected slot {index}");
            }
        }

        /// <summary>
        /// Callback para o input de submit (A/Enter/Space).
        /// Implementa a lógica de swap de slots.
        /// Primeiro clique: marca o slot para swap (cor azulada).
        /// Segundo clique: executa o swap com o slot marcado.
        /// </summary>
        private void OnSubmitInput(InputAction.CallbackContext context)
        {
            if (!isOpen || currentSelectedIndex < 0)
                return;

            // Se nenhum slot foi marcado para swap ainda
            if (swapFirstSlotIndex < 0)
            {
                // Marca o slot atual
                swapFirstSlotIndex = currentSelectedIndex;
                slotUIComponents[swapFirstSlotIndex].SetSwapSelected(true);
                LogMessage($"Primeiro slot marcado para swap: {swapFirstSlotIndex}");
            }
            else if (swapFirstSlotIndex == currentSelectedIndex)
            {
                // Se clicar no mesmo slot, desmarca
                slotUIComponents[swapFirstSlotIndex].SetSwapSelected(false);
                swapFirstSlotIndex = -1;
                LogMessage($"Swap cancelado");
            }
            else
            {
                // Executa o swap entre os dois slots
                int secondSlotIndex = currentSelectedIndex;

                if (InventoryManager.Instance != null)
                {
                    bool success = InventoryManager.Instance.SwapSlots(swapFirstSlotIndex, secondSlotIndex);

                    if (success)
                    {
                        LogMessage($"Swap executado: slot {swapFirstSlotIndex} <-> slot {secondSlotIndex}");
                        RefreshAllSlots(); // Atualiza a visualização de todos os slots

                        // Se o primeiro slot estava em mode swap selection, ainda mantém seleção
                        slotUIComponents[swapFirstSlotIndex].SetSwapSelected(false);
                        swapFirstSlotIndex = -1;
                    }
                }
            }
        }

        /// <summary>
        /// Callback para o input de cancel (B/Escape).
        /// Cancela a operação de swap se houver um slot marcado.
        /// </summary>
        private void OnCancelInput(InputAction.CallbackContext context)
        {
            if (!isOpen)
                return;

            // Se há um slot em modo swap, cancela a operação
            if (swapFirstSlotIndex >= 0)
            {
                slotUIComponents[swapFirstSlotIndex].SetSwapSelected(false);
                swapFirstSlotIndex = -1;
                LogMessage("Operação de swap cancelada pelo usuário");
            }
            else
            {
                // Se não há swap ativo, fechar o inventário
                Hide();
                LogMessage("Inventário fechado via tecla Cancel (I)");
            }
        }

        /// <summary>
        /// Callback para atribuir item selecionado a um slot de acesso rápido.
        /// Valida se o item é consumível antes de atribuir.
        /// </summary>
        /// <param name="slotIndex">Índice do slot rápido (0-3)</param>
        private void OnAssignToQuickSlot(int slotIndex)
        {
            LogMessage($"🎯 OnAssignToQuickSlot chamado: slotIndex={slotIndex}, isOpen={isOpen}, currentSelectedIndex={currentSelectedIndex}");

            if (!isOpen || currentSelectedIndex < 0 || InventoryManager.Instance == null)
            {
                LogMessage($"❌ Atribuição cancelada: isOpen={isOpen}, currentSelectedIndex={currentSelectedIndex}, InventoryManager={InventoryManager.Instance != null}");
                return;
            }

            // Obtém o item do slot selecionado
            InventorySlot slot = InventoryManager.Instance.GetSlot(currentSelectedIndex);

            if (slot == null || slot.IsEmpty)
            {
                LogMessage($"Tentativa de atribuir item de slot vazio: {currentSelectedIndex}");
                return;
            }

            // Valida se é um item consumível
            if (slot.item.type != ItemType.Consumable)
            {
                LogMessage($"Apenas itens consumíveis podem ser atribuídos a slots rápidos. Item: {slot.item.itemName} é {slot.item.type}");
                return;
            }

            // Atribui o item ao slot rápido usando instanceID específico (não apenas ItemData)
            InventoryManager.Instance.AssignQuickSlot(slot.instanceID, slotIndex);
            LogMessage($"Item '{slot.item.itemName}' (ID {slot.instanceID}) atribuído ao slot rápido {slotIndex + 1}");
            // TODO: Adicionar feedback visual/audio (efeito animado, som de confirmação, etc)
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

        #region Item Details

        /// <summary>
        /// Atualiza o painel de detalhes com as informações do item selecionado.
        /// Se item for null, oculta o painel ou exibe mensagem placeholder.
        /// </summary>
        private void UpdateItemDetails(ItemData item)
        {
            if (itemDetailsPanel == null)
            {
                LogMessage("Item details panel não configurado");
                return;
            }

            if (item == null)
            {
                // Nenhum item selecionado - oculta painel ou exibe placeholder
                itemDetailsPanel.SetActive(false);
                LogMessage("Ocultando painel de detalhes (sem item selecionado)");
                return;
            }

            // Exibe painel
            itemDetailsPanel.SetActive(true);

            // Atualiza ícone
            if (detailsIconImage != null)
            {
                if (item.icon != null)
                {
                    detailsIconImage.enabled = true;
                    detailsIconImage.sprite = item.icon;
                }
                else
                {
                    detailsIconImage.enabled = false;
                }
            }

            // Atualiza título
            if (detailsTitleText != null)
            {
                // Usa localização se disponível
                if (!string.IsNullOrEmpty(item.localizationKey))
                {
                    var localizedString = new LocalizedString("Items", item.localizationKey);
                    var operation = localizedString.GetLocalizedStringAsync();
                    operation.Completed += (op) =>
                    {
                        if (detailsTitleText != null)
                        {
                            detailsTitleText.text = op.Result;
                        }
                    };
                }
                else
                {
                    detailsTitleText.text = item.itemName;
                }
            }

            // Atualiza descrição
            if (detailsDescriptionText != null)
            {
                // Usa localização se disponível (chave + "D")
                if (!string.IsNullOrEmpty(item.localizationKey))
                {
                    var localizedString = new LocalizedString("Items", item.localizationKey + "D");
                    var operation = localizedString.GetLocalizedStringAsync();
                    operation.Completed += (op) =>
                    {
                        if (detailsDescriptionText != null)
                        {
                            if (!string.IsNullOrEmpty(op.Result))
                            {
                                detailsDescriptionText.text = op.Result;
                            }
                            else
                            {
                                detailsDescriptionText.text = "<i>Sem descrição disponível</i>";
                            }
                        }
                    };
                }
                else if (!string.IsNullOrEmpty(item.description))
                {
                    detailsDescriptionText.text = item.description;
                }
                else
                {
                    detailsDescriptionText.text = "<i>Sem descrição disponível</i>";
                }
            }

            LogMessage($"Exibindo detalhes do item: {item.itemName}");
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
