using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheSlimeKing.Gameplay
{
    /// <summary>
    /// Gerencia os inputs do jogador e os encaminha para os controladores do Slime
    /// Refatorado para usar InputActionReference diretamente
    /// Compatível com Input System e URP no Unity 6
    /// 
    /// Mapeamento de ações do Input System:
    /// - movementAction     -> Move          (Vector2) - Movimento vetorial 2D do personagem
    /// - attackAction       -> Attack        (Button)  - Ataque básico
    /// - specialAttackAction -> SpecialAttack (Button)  - Ataque especial
    /// - interactAction     -> Interact      (Button)  - Interação com objetos
    /// - crouchAction       -> Crouch        (Button)  - Agachamento/esconder
    /// - menuAction         -> Menu          (Button)  - Abrir menu de pausa
    /// - inventoryAction    -> Inventory     (Button)  - Abrir inventário
    /// - useItem1Action     -> UseItem1      (Button)  - Usar item do slot 1
    /// - useItem2Action     -> UseItem2      (Button)  - Usar item do slot 2
    /// - useItem3Action     -> UseItem3      (Button)  - Usar item do slot 3
    /// - useItem4Action     -> UseItem4      (Button)  - Usar item do slot 4
    /// </summary>
    public class SlimeInputHandler : MonoBehaviour
    {
        [Header("Input Actions")]
        [Tooltip("Ação de movimento vetorial 2D")]
        [SerializeField] private InputActionReference movementAction; // Move

        [Tooltip("Ação de ataque básico")]
        [SerializeField] private InputActionReference attackAction; // Attack

        [Tooltip("Ação de ataque especial")]
        [SerializeField] private InputActionReference specialAttackAction; // SpecialAttack

        [Tooltip("Ação de interação com objetos")]
        [SerializeField] private InputActionReference interactAction; // Interact

        [Tooltip("Ação de agachamento")]
        [SerializeField] private InputActionReference crouchAction; // Crouch

        [Tooltip("Ação de abertura de menu")]
        [SerializeField] private InputActionReference menuAction; // Menu

        [Tooltip("Ação de abertura de inventário")]
        [SerializeField] private InputActionReference inventoryAction; // Inventory

        [Header("Item Actions")]
        [Tooltip("Ação para usar o item 1")]
        [SerializeField] private InputActionReference useItem1Action; // UseItem1

        [Tooltip("Ação para usar o item 2")]
        [SerializeField] private InputActionReference useItem2Action; // UseItem2

        [Tooltip("Ação para usar o item 3")]
        [SerializeField] private InputActionReference useItem3Action; // UseItem3

        [Tooltip("Ação para usar o item 4")]
        [SerializeField] private InputActionReference useItem4Action; // UseItem4

        [Header("Referências de Controladores")]
        [SerializeField] private SlimeMovement movementController;
        [SerializeField] private SlimeInteractionController interactionController;
        [SerializeField] private SlimeAnimationController animationController;

        [Header("Debug")]
        [SerializeField] private bool logInputEvents = false;

        // Estado de controle de input
        private bool _inputEnabled = true;

        private void Awake()
        {
            // Busca todos os controladores automaticamente se estiverem no mesmo GameObject
            if (movementController == null)
                movementController = GetComponent<SlimeMovement>();

            if (interactionController == null)
                interactionController = GetComponent<SlimeInteractionController>();

            if (animationController == null)
                animationController = GetComponent<SlimeAnimationController>();

            // Verifica se todas as InputActionReferences estão configuradas
            ValidateInputActionReferences();
        }

        private void Start()
        {
            // Habilita todos os inputs
            EnableInput(true);
        }

        private void OnDestroy()
        {
            // Desabilita todos os inputs
            EnableInput(false);
        }
        private void ValidateInputActionReferences()
        {
            // Log de avisos para referências de ações não configuradas
            if (movementAction == null)
                Debug.LogWarning("SlimeInputHandler: movementAction (Move) não configurada!");

            if (attackAction == null)
                Debug.LogWarning("SlimeInputHandler: attackAction (Attack) não configurada!");

            if (specialAttackAction == null)
                Debug.LogWarning("SlimeInputHandler: specialAttackAction (SpecialAttack) não configurada!");

            if (interactAction == null)
                Debug.LogWarning("SlimeInputHandler: interactAction (Interact) não configurada!");

            if (crouchAction == null)
                Debug.LogWarning("SlimeInputHandler: crouchAction (Crouch) não configurada!");

            if (menuAction == null)
                Debug.LogWarning("SlimeInputHandler: menuAction (Menu) não configurada!");

            if (inventoryAction == null)
                Debug.LogWarning("SlimeInputHandler: inventoryAction (Inventory) não configurada!");

            // Validação das ações de item
            if (useItem1Action == null)
                Debug.LogWarning("SlimeInputHandler: useItem1Action (UseItem1) não configurada!");

            if (useItem2Action == null)
                Debug.LogWarning("SlimeInputHandler: useItem2Action (UseItem2) não configurada!");

            if (useItem3Action == null)
                Debug.LogWarning("SlimeInputHandler: useItem3Action (UseItem3) não configurada!");

            if (useItem4Action == null)
                Debug.LogWarning("SlimeInputHandler: useItem4Action (UseItem4) não configurada!");
        }

        /// <summary>
        /// Configura os listeners para as ações de movimento
        /// </summary>
        private void SetupMovementCallbacks(bool enable)
        {
            if (movementAction != null && movementAction.action != null)
            {
                if (enable)
                {
                    movementAction.action.performed += OnMoveActionPerformed;
                    movementAction.action.canceled += OnMoveActionCanceled;

                    if (logInputEvents)
                        Debug.Log("Habilitando callbacks de movimento");

                    // Garante que a ação está habilitada
                    if (!movementAction.action.enabled)
                        movementAction.action.Enable();
                }
                else
                {
                    movementAction.action.performed -= OnMoveActionPerformed;
                    movementAction.action.canceled -= OnMoveActionCanceled;

                    if (logInputEvents)
                        Debug.Log("Desabilitando callbacks de movimento");
                }
            }
        }

        /// <summary>
        /// Callback para o evento de movimento
        /// </summary>
        private void OnMoveActionPerformed(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (movementController != null)
            {
                // Extraímos o valor diretamente e chamamos o método adaptado
                Vector2 moveValue = context.ReadValue<Vector2>();

                if (logInputEvents)
                    Debug.Log($"Movimento: {moveValue}");

                movementController.OnMoveVector(moveValue);
            }
        }

        /// <summary>
        /// Callback para o evento de cancelamento de movimento
        /// </summary>
        private void OnMoveActionCanceled(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (movementController != null)
            {
                // Movimento zero quando o input é cancelado
                if (logInputEvents)
                    Debug.Log("Movimento cancelado");

                movementController.OnMoveVector(Vector2.zero);
            }
        }

        /// <summary>
        /// Configura os listeners para a ação de agachamento
        /// </summary>
        private void SetupCrouchCallbacks(bool enable)
        {
            if (crouchAction != null && crouchAction.action != null)
            {
                if (enable)
                {
                    crouchAction.action.performed += OnCrouchActionPerformed;
                    crouchAction.action.canceled += OnCrouchActionCanceled;

                    if (logInputEvents)
                        Debug.Log("Habilitando callbacks de agachamento");

                    // Garante que a ação está habilitada
                    if (!crouchAction.action.enabled)
                        crouchAction.action.Enable();
                }
                else
                {
                    crouchAction.action.performed -= OnCrouchActionPerformed;
                    crouchAction.action.canceled -= OnCrouchActionCanceled;

                    if (logInputEvents)
                        Debug.Log("Desabilitando callbacks de agachamento");
                }
            }
        }

        /// <summary>
        /// Callback para o evento de agachamento (botão pressionado)
        /// </summary>
        private void OnCrouchActionPerformed(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (movementController != null)
            {
                if (logInputEvents)
                    Debug.Log("Botão de agachar pressionado");

                movementController.OnCrouchPressed(true);
            }
        }

        /// <summary>
        /// Callback para o evento de agachamento (botão liberado)
        /// </summary>
        private void OnCrouchActionCanceled(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (movementController != null)
            {
                if (logInputEvents)
                    Debug.Log("Botão de agachar liberado");

                movementController.OnCrouchRelease();
            }
        }

        /// <summary>
        /// Configura os listeners para a ação de ataque
        /// </summary>
        private void SetupAttackCallbacks(bool enable)
        {
            if (attackAction != null && attackAction.action != null)
            {
                if (enable)
                {
                    attackAction.action.performed += OnAttackActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Habilitando callbacks de ataque");

                    // Garante que a ação está habilitada
                    if (!attackAction.action.enabled)
                        attackAction.action.Enable();
                }
                else
                {
                    attackAction.action.performed -= OnAttackActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Desabilitando callbacks de ataque");
                }
            }
        }

        /// <summary>
        /// Callback para o evento de ataque
        /// </summary>
        private void OnAttackActionPerformed(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (movementController != null)
            {
                if (logInputEvents)
                    Debug.Log("Botão de ataque pressionado");

                movementController.OnAttackPressed();
            }
        }

        /// <summary>
        /// Configura os listeners para a ação de ataque especial
        /// </summary>
        private void SetupSpecialAttackCallbacks(bool enable)
        {
            if (specialAttackAction != null && specialAttackAction.action != null)
            {
                if (enable)
                {
                    specialAttackAction.action.performed += OnSpecialAttackActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Habilitando callbacks de ataque especial");

                    // Garante que a ação está habilitada
                    if (!specialAttackAction.action.enabled)
                        specialAttackAction.action.Enable();
                }
                else
                {
                    specialAttackAction.action.performed -= OnSpecialAttackActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Desabilitando callbacks de ataque especial");
                }
            }
        }

        /// <summary>
        /// Callback para o evento de ataque especial
        /// </summary>
        private void OnSpecialAttackActionPerformed(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (movementController != null)
            {
                if (logInputEvents)
                    Debug.Log("Botão de ataque especial pressionado");

                movementController.OnSpecialAttackPressed();
            }
        }

        /// <summary>
        /// Configura os listeners para a ação de interação
        /// </summary>
        private void SetupInteractCallbacks(bool enable)
        {
            if (interactAction != null && interactAction.action != null)
            {
                if (enable)
                {
                    interactAction.action.performed += OnInteractActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Habilitando callbacks de interação");

                    // Garante que a ação está habilitada
                    if (!interactAction.action.enabled)
                        interactAction.action.Enable();
                }
                else
                {
                    interactAction.action.performed -= OnInteractActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Desabilitando callbacks de interação");
                }
            }
        }

        /// <summary>
        /// Callback para o evento de interação
        /// </summary>
        private void OnInteractActionPerformed(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (interactionController != null)
            {
                if (logInputEvents)
                    Debug.Log("Botão de interação pressionado");

                interactionController.Interact();
            }

            if (movementController != null)
            {
                movementController.OnInteractPressed();
            }
        }

        /// <summary>
        /// Configura os listeners para a ação de menu
        /// </summary>
        private void SetupMenuCallbacks(bool enable)
        {
            if (menuAction != null && menuAction.action != null)
            {
                if (enable)
                {
                    menuAction.action.performed += OnMenuActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Habilitando callbacks de menu");

                    // Garante que a ação está habilitada
                    if (!menuAction.action.enabled)
                        menuAction.action.Enable();
                }
                else
                {
                    menuAction.action.performed -= OnMenuActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Desabilitando callbacks de menu");
                }
            }
        }

        /// <summary>
        /// Callback para o evento de menu
        /// </summary>
        private void OnMenuActionPerformed(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (logInputEvents)
                Debug.Log("Botão de menu pressionado");

            HandleMenuButtonPressed();
        }

        /// <summary>
        /// Callback para abertura do menu
        /// </summary>
        private void HandleMenuButtonPressed()
        {
            // Lógica do menu será implementada em outro sistema
            Debug.Log("Menu button pressed");
        }

        /// <summary>
        /// Configura os listeners para a ação de inventário
        /// </summary>
        private void SetupInventoryCallbacks(bool enable)
        {
            if (inventoryAction != null && inventoryAction.action != null)
            {
                if (enable)
                {
                    inventoryAction.action.performed += OnInventoryActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Habilitando callbacks de inventário");

                    // Garante que a ação está habilitada
                    if (!inventoryAction.action.enabled)
                        inventoryAction.action.Enable();
                }
                else
                {
                    inventoryAction.action.performed -= OnInventoryActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Desabilitando callbacks de inventário");
                }
            }
        }

        /// <summary>
        /// Callback para o evento de abertura do inventário
        /// </summary>
        private void OnInventoryActionPerformed(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (logInputEvents)
                Debug.Log("Botão de inventário pressionado");

            HandleInventoryButtonPressed();
        }

        /// <summary>
        /// Callback para abertura do inventário
        /// </summary>
        private void HandleInventoryButtonPressed()
        {
            // Lógica de abertura do inventário - será implementada em outro sistema
            Debug.Log("Inventory button pressed");

            // TODO: Implementar chamada para o sistema de inventário quando estiver disponível
        }

        /// <summary>
        /// Configura os listeners para a ação de usar item 1
        /// </summary>
        private void SetupUseItem1Callbacks(bool enable)
        {
            if (useItem1Action != null && useItem1Action.action != null)
            {
                if (enable)
                {
                    useItem1Action.action.performed += OnUseItem1ActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Habilitando callbacks de uso do item 1");

                    // Garante que a ação está habilitada
                    if (!useItem1Action.action.enabled)
                        useItem1Action.action.Enable();
                }
                else
                {
                    useItem1Action.action.performed -= OnUseItem1ActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Desabilitando callbacks de uso do item 1");
                }
            }
        }

        /// <summary>
        /// Callback para o evento de usar item 1
        /// </summary>
        private void OnUseItem1ActionPerformed(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (logInputEvents)
                Debug.Log("Botão de usar item 1 pressionado");

            HandleUseItem(1);
        }

        /// <summary>
        /// Configura os listeners para a ação de usar item 2
        /// </summary>
        private void SetupUseItem2Callbacks(bool enable)
        {
            if (useItem2Action != null && useItem2Action.action != null)
            {
                if (enable)
                {
                    useItem2Action.action.performed += OnUseItem2ActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Habilitando callbacks de uso do item 2");

                    // Garante que a ação está habilitada
                    if (!useItem2Action.action.enabled)
                        useItem2Action.action.Enable();
                }
                else
                {
                    useItem2Action.action.performed -= OnUseItem2ActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Desabilitando callbacks de uso do item 2");
                }
            }
        }

        /// <summary>
        /// Callback para o evento de usar item 2
        /// </summary>
        private void OnUseItem2ActionPerformed(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (logInputEvents)
                Debug.Log("Botão de usar item 2 pressionado");

            HandleUseItem(2);
        }

        /// <summary>
        /// Configura os listeners para a ação de usar item 3
        /// </summary>
        private void SetupUseItem3Callbacks(bool enable)
        {
            if (useItem3Action != null && useItem3Action.action != null)
            {
                if (enable)
                {
                    useItem3Action.action.performed += OnUseItem3ActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Habilitando callbacks de uso do item 3");

                    // Garante que a ação está habilitada
                    if (!useItem3Action.action.enabled)
                        useItem3Action.action.Enable();
                }
                else
                {
                    useItem3Action.action.performed -= OnUseItem3ActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Desabilitando callbacks de uso do item 3");
                }
            }
        }

        /// <summary>
        /// Callback para o evento de usar item 3
        /// </summary>
        private void OnUseItem3ActionPerformed(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (logInputEvents)
                Debug.Log("Botão de usar item 3 pressionado");

            HandleUseItem(3);
        }

        /// <summary>
        /// Configura os listeners para a ação de usar item 4
        /// </summary>
        private void SetupUseItem4Callbacks(bool enable)
        {
            if (useItem4Action != null && useItem4Action.action != null)
            {
                if (enable)
                {
                    useItem4Action.action.performed += OnUseItem4ActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Habilitando callbacks de uso do item 4");

                    // Garante que a ação está habilitada
                    if (!useItem4Action.action.enabled)
                        useItem4Action.action.Enable();
                }
                else
                {
                    useItem4Action.action.performed -= OnUseItem4ActionPerformed;

                    if (logInputEvents)
                        Debug.Log("Desabilitando callbacks de uso do item 4");
                }
            }
        }

        /// <summary>
        /// Callback para o evento de usar item 4
        /// </summary>
        private void OnUseItem4ActionPerformed(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (logInputEvents)
                Debug.Log("Botão de usar item 4 pressionado");

            HandleUseItem(4);
        }

        /// <summary>
        /// Manipula o uso do item com base no slot (1-4)
        /// </summary>
        private void HandleUseItem(int itemSlot)
        {
            // Lógica de uso do item - será implementada em outro sistema
            Debug.Log($"Usando item do slot {itemSlot}");

            // TODO: Implementar chamada para o sistema de itens quando estiver disponível
            // Por exemplo: itemController.UseItem(itemSlot);
        }

        /// <summary>
        /// Habilita ou desabilita o input do jogador
        /// </summary>
        public void EnableInput(bool enabled)
        {
            _inputEnabled = enabled;

            // Gerencia todos os callbacks de input
            SetupMovementCallbacks(enabled);
            SetupCrouchCallbacks(enabled);
            SetupAttackCallbacks(enabled);
            SetupSpecialAttackCallbacks(enabled);
            SetupInteractCallbacks(enabled);
            SetupMenuCallbacks(enabled);
            SetupInventoryCallbacks(enabled);
            SetupUseItem1Callbacks(enabled);
            SetupUseItem2Callbacks(enabled);
            SetupUseItem3Callbacks(enabled);
            SetupUseItem4Callbacks(enabled);

            // Se o input foi desabilitado, reset os valores para evitar movimento contínuo
            if (!enabled && movementController != null)
            {
                // Simula input zero para parar movimento
                Vector2 zeroVector = Vector2.zero;
                movementController.OnMoveVector(zeroVector);
            }
        }

        /// <summary>
        /// Métodos legados mantidos para compatibilidade
        /// Estes podem ser usados para callbacks diretos do Input System se necessário
        /// </summary>

        #region Legacy Input System Compatibility

        /// <summary>
        /// Callback para ação de movimento (compatibilidade legada)
        /// </summary>
        public void OnMove(InputValue value)
        {
            if (!_inputEnabled || movementController == null) return;

            // Usar ReadValue para obter o Vector2 de forma segura
            Vector2 moveVector = value.Get<Vector2>();

            if (logInputEvents)
                Debug.Log($"OnMove legado: {moveVector}");

            movementController.OnMoveVector(moveVector);
        }

        /// <summary>
        /// Callback para ação de agachamento (compatibilidade legada)
        /// </summary>
        public void OnCrouch(InputValue value)
        {
            if (!_inputEnabled || movementController == null) return;

            // Verificar se o botão está pressionado ou solto
            bool isPressed = value.isPressed;

            if (logInputEvents)
                Debug.Log($"OnCrouch legado: {isPressed}");

            if (isPressed)
            {
                movementController.OnCrouchPressed(true);
            }
            else
            {
                movementController.OnCrouchRelease();
            }
        }

        /// <summary>
        /// Callback para ação de ataque (compatibilidade legada)
        /// </summary>
        public void OnAttack(InputValue value)
        {
            if (!_inputEnabled || movementController == null) return;

            // Usar IsPressed para verificar se o botão foi pressionado
            if (value.isPressed)
            {
                if (logInputEvents)
                    Debug.Log("OnAttack legado");

                movementController.OnAttackPressed();
            }
        }

        /// <summary>
        /// Callback para ação de ataque especial (compatibilidade legada)
        /// </summary>
        public void OnSpecial(InputValue value)
        {
            if (!_inputEnabled || movementController == null) return;

            // Usar IsPressed para verificar se o botão foi pressionado
            if (value.isPressed)
            {
                if (logInputEvents)
                    Debug.Log("OnSpecial legado (SpecialAttack)");

                movementController.OnSpecialAttackPressed();
            }
        }

        /// <summary>
        /// Callback para ação de interação (compatibilidade legada)
        /// </summary>
        public void OnInteract(InputValue value)
        {
            if (!_inputEnabled) return;

            // Usar IsPressed para verificar se o botão foi pressionado
            if (value.isPressed)
            {
                if (logInputEvents)
                    Debug.Log("OnInteract legado");

                if (interactionController != null)
                {
                    interactionController.Interact();
                }

                // Também repassa para o controlador de movimento para a parte de animação
                if (movementController != null)
                    movementController.OnInteractPressed();
            }
        }

        /// <summary>
        /// Callback para ação de inventário (compatibilidade legada)
        /// </summary>
        public void OnInventory(InputValue value)
        {
            if (!_inputEnabled) return;

            // Usar IsPressed para verificar se o botão foi pressionado
            if (value.isPressed)
            {
                if (logInputEvents)
                    Debug.Log("OnInventory legado");

                // Chama o método de manipulação do inventário
                HandleInventoryButtonPressed();
            }
        }

        /// <summary>
        /// Callback para ação de usar o item 1 (compatibilidade legada)
        /// </summary>
        public void OnUseItem1(InputValue value)
        {
            if (!_inputEnabled) return;

            // Usar IsPressed para verificar se o botão foi pressionado
            if (value.isPressed)
            {
                if (logInputEvents)
                    Debug.Log("OnUseItem1 legado");

                // Chama o método de usar item
                HandleUseItem(1);
            }
        }

        /// <summary>
        /// Callback para ação de usar o item 2 (compatibilidade legada)
        /// </summary>
        public void OnUseItem2(InputValue value)
        {
            if (!_inputEnabled) return;

            // Usar IsPressed para verificar se o botão foi pressionado
            if (value.isPressed)
            {
                if (logInputEvents)
                    Debug.Log("OnUseItem2 legado");

                // Chama o método de usar item
                HandleUseItem(2);
            }
        }

        /// <summary>
        /// Callback para ação de usar o item 3 (compatibilidade legada)
        /// </summary>
        public void OnUseItem3(InputValue value)
        {
            if (!_inputEnabled) return;

            // Usar IsPressed para verificar se o botão foi pressionado
            if (value.isPressed)
            {
                if (logInputEvents)
                    Debug.Log("OnUseItem3 legado");

                // Chama o método de usar item
                HandleUseItem(3);
            }
        }

        /// <summary>
        /// Callback para ação de usar o item 4 (compatibilidade legada)
        /// </summary>
        public void OnUseItem4(InputValue value)
        {
            if (!_inputEnabled) return;

            // Usar IsPressed para verificar se o botão foi pressionado
            if (value.isPressed)
            {
                if (logInputEvents)
                    Debug.Log("OnUseItem4 legado");

                // Chama o método de usar item
                HandleUseItem(4);
            }
        }

        #endregion
    }
}
