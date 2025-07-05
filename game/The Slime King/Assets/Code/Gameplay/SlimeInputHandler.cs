using UnityEngine;
using UnityEngine.InputSystem;

namespace TheSlimeKing.Gameplay
{
    /// <summary>
    /// Gerencia os inputs do jogador e os encaminha para os controladores do Slime
    /// Compatível com Input System e URP no Unity 6
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class SlimeInputHandler : MonoBehaviour
    {
        [Header("Referências de Controladores")]
        [SerializeField] private SlimeMovement movementController;
        [SerializeField] private SlimeInteractionController interactionController;
        [SerializeField] private SlimeAnimationController animationController;

        [Header("Debug")]
        [SerializeField] private bool logInputEvents = false;

        // Referência ao componente PlayerInput
        private PlayerInput _playerInput;

        // Estado de controle de input
        private bool _inputEnabled = true; private void Awake()
        {
            // Obtem referência ao componente PlayerInput
            _playerInput = GetComponent<PlayerInput>();

            // Busca todos os controladores automaticamente se estiverem no mesmo GameObject
            if (movementController == null)
                movementController = GetComponent<SlimeMovement>();

            if (interactionController == null)
                interactionController = GetComponent<SlimeInteractionController>();

            if (animationController == null)
                animationController = GetComponent<SlimeAnimationController>();

            // Configura o action map inicial
            _playerInput.defaultActionMap = "Gameplay";
        }

        private void Start()
        {
            // Input System Events - para compatibilidade com Unity 6
            _playerInput.onActionTriggered += OnInputActionTriggered;
        }

        private void OnDestroy()
        {
            // Limpa os event listeners
            if (_playerInput != null)
                _playerInput.onActionTriggered -= OnInputActionTriggered;
        }

        /// <summary>
        /// Manipula todos os eventos de input via Unity 6 Input System
        /// </summary>
        private void OnInputActionTriggered(InputAction.CallbackContext context)
        {
            if (!_inputEnabled) return;

            if (logInputEvents)
                Debug.Log($"Input: {context.action.name}, phase: {context.phase}");

            // Ensure correct processing based on action name and phase
            string actionName = context.action.name;

            switch (actionName)
            {
                case "Move":
                    if (movementController != null)
                    {
                        // Extraímos o valor diretamente e chamamos o método adaptado
                        Vector2 moveValue = context.ReadValue<Vector2>();
                        movementController.OnMoveVector(moveValue);
                    }
                    break;

                case "Crouch":
                    if (movementController != null)
                    {
                        if (context.phase == InputActionPhase.Performed)
                        {
                            // Botão pressionado
                            movementController.OnCrouchPressed(true);
                        }
                        else if (context.phase == InputActionPhase.Canceled)
                        {
                            // Botão liberado
                            movementController.OnCrouchRelease();
                        }
                    }
                    break;

                case "Attack":
                    if (movementController != null && context.phase == InputActionPhase.Performed)
                    {
                        // Chamamos o método sem parâmetro, já que precisamos apenas saber quando o botão foi pressionado
                        movementController.OnAttackPressed();
                    }
                    break;
                case "Special":
                    if (movementController != null && context.phase == InputActionPhase.Performed)
                    {
                        // Chamamos o método sem parâmetro, já que precisamos apenas saber quando o botão foi pressionado
                        movementController.OnSpecialAttackPressed();
                    }
                    break;

                case "Interact":
                    if (context.phase == InputActionPhase.Performed)
                    {
                        if (interactionController != null)
                            interactionController.Interact();

                        if (movementController != null)
                        {
                            // Chamamos o método sem parâmetro, já que precisamos apenas saber quando o botão foi pressionado
                            movementController.OnInteractPressed();
                        }
                    }
                    break;

                case "Menu":
                    if (context.phase == InputActionPhase.Performed)
                        OnMenuPressed();
                    break;
            }
        }

        /// <summary>
        /// Callback para ação de movimento
        /// </summary>
        public void OnMove(InputValue value)
        {
            if (!_inputEnabled) return;

            if (movementController != null)
            {
                // Usar ReadValue para obter o Vector2 de forma segura
                Vector2 moveVector = value.Get<Vector2>();
                movementController.OnMoveVector(moveVector);
            }
        }

        /// <summary>
        /// Callback para ação de agachamento
        /// </summary>
        public void OnCrouch(InputValue value)
        {
            if (!_inputEnabled) return;

            if (movementController != null)
            {
                // Verificar se o botão está pressionado ou solto
                bool isPressed = value.isPressed;

                if (isPressed)
                {
                    // Botão foi pressionado
                    movementController.OnCrouchPressed(true);
                }
                else
                {
                    // Botão foi solto
                    movementController.OnCrouchRelease();
                }
            }
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Debug.Log("Crouch pressionado");
            }
            else if (context.canceled)
            {
                Debug.Log("Crouch solto");
            }
        }

        /// <summary>
        /// Callback para ação de ataque
        /// </summary>
        public void OnAttack(InputValue value)
        {
            if (!_inputEnabled) return;

            // Usar IsPressed() para verificar se o botão foi pressionado
            if (value.isPressed && movementController != null)
                movementController.OnAttackPressed();
        }

        /// <summary>
        /// Callback para ação de ataque especial
        /// </summary>
        public void OnSpecial(InputValue value)
        {
            if (!_inputEnabled) return;

            // Usar IsPressed() para verificar se o botão foi pressionado
            if (value.isPressed && movementController != null)
                movementController.OnSpecialAttackPressed();
        }

        /// <summary>
        /// Callback para ação de interação
        /// </summary>
        public void OnInteract(InputValue value)
        {
            if (!_inputEnabled) return;

            // Usar IsPressed() para verificar se o botão foi pressionado
            if (value.isPressed)
            {
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
        /// Callback para abertura do menu
        /// </summary>
        public void OnMenuPressed()
        {
            // Lógica do menu será implementada em outro sistema
            Debug.Log("Menu button pressed");
        }

        /// <summary>
        /// Habilita ou desabilita o input do jogador
        /// </summary>
        public void EnableInput(bool enabled)
        {
            _inputEnabled = enabled;

            // Se o input foi desabilitado, reset os valores para evitar movimento contínuo
            if (!enabled && movementController != null)
            {
                // Simula input zero para parar movimento
                Vector2 zeroVector = Vector2.zero;
                movementController.OnMoveVector(zeroVector);
            }
        }

        /// <summary>
        /// Troca entre action maps diferentes
        /// </summary>
        public void SwitchActionMap(string actionMapName)
        {
            _playerInput.SwitchCurrentActionMap(actionMapName);
        }

        // Nota: Métodos simplificados são usados para compatibilidade com Unity 6
        // Agora usamos métodos adaptados que aceitam diretamente Vector2 ou bool
    }
}
