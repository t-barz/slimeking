using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace SlimeKing.Systems
{
    /// <summary>
    /// Manager de input simplificado usando o novo Input System.
    /// Fornece abstração para controles de teclado, mouse e gamepad.
    /// </summary>
    public class InputManager : Core.ManagerBase<InputManager>
    {
        [Header("Input Settings")]
        [SerializeField] private bool enableGamepadSupport = true;
        [SerializeField] private float deadzone = 0.2f;

        // Input Actions Asset (será configurado via Inspector)
        [Header("Input Actions")]
        [SerializeField] private InputActionAsset inputActions;

        // Cached input actions
        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction attackAction;
        private InputAction pauseAction;
        private InputAction interactAction;

        // Input state
        private Vector2 moveInput;
        private bool jumpPressed;
        private bool attackPressed;
        private bool pausePressed;
        private bool interactPressed;

        // Properties para acesso rápido
        public Vector2 MoveInput => moveInput;
        public bool JumpPressed => jumpPressed;
        public bool AttackPressed => attackPressed;
        public bool PausePressed => pausePressed;
        public bool InteractPressed => interactPressed;

        // Device info
        public bool HasGamepad => Gamepad.current != null;
        public bool HasKeyboard => Keyboard.current != null;
        public bool HasMouse => Mouse.current != null;

        protected override void Initialize()
        {
            SetupInputActions();
            Log("InputManager initialized");
        }

        #region Setup

        /// <summary>
        /// Configura as input actions
        /// </summary>
        private void SetupInputActions()
        {
            if (inputActions != null)
            {
                // Habilita o asset
                inputActions.Enable();

                // Cache das actions principais
                moveAction = inputActions.FindAction("Move");
                jumpAction = inputActions.FindAction("Jump");
                attackAction = inputActions.FindAction("Attack");
                pauseAction = inputActions.FindAction("Pause");
                interactAction = inputActions.FindAction("Interact");

                // Subscreve aos eventos
                if (jumpAction != null)
                    jumpAction.performed += OnJumpPerformed;

                if (attackAction != null)
                    attackAction.performed += OnAttackPerformed;

                if (pauseAction != null)
                    pauseAction.performed += OnPausePerformed;

                if (interactAction != null)
                    interactAction.performed += OnInteractPerformed;
            }
            else
            {
                LogWarning("Input Actions Asset not assigned, using fallback input");
                SetupFallbackInput();
            }
        }

        /// <summary>
        /// Configura input de fallback sem Input Actions Asset
        /// </summary>
        private void SetupFallbackInput()
        {
            Log("Using fallback input system");
        }

        #endregion

        #region Update

        private void Update()
        {
            UpdateInputs();
        }

        /// <summary>
        /// Atualiza os inputs a cada frame
        /// </summary>
        private void UpdateInputs()
        {
            if (inputActions != null)
            {
                // Move input (continuous)
                if (moveAction != null)
                {
                    Vector2 rawInput = moveAction.ReadValue<Vector2>();
                    moveInput = ApplyDeadzone(rawInput);
                }
            }
            else
            {
                // Fallback input usando Input System direto
                UpdateFallbackInput();
            }

            // Reset de inputs de um frame só
            jumpPressed = false;
            attackPressed = false;
            pausePressed = false;
            interactPressed = false;
        }

        /// <summary>
        /// Atualiza input de fallback
        /// </summary>
        private void UpdateFallbackInput()
        {
            // Movement
            Vector2 keyboardMove = Vector2.zero;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed) keyboardMove.y += 1;
                if (Keyboard.current.sKey.isPressed) keyboardMove.y -= 1;
                if (Keyboard.current.aKey.isPressed) keyboardMove.x -= 1;
                if (Keyboard.current.dKey.isPressed) keyboardMove.x += 1;
            }

            // Gamepad movement
            Vector2 gamepadMove = Vector2.zero;
            if (enableGamepadSupport && Gamepad.current != null)
            {
                gamepadMove = Gamepad.current.leftStick.ReadValue();
            }

            // Combina inputs
            moveInput = keyboardMove.magnitude > gamepadMove.magnitude ? keyboardMove : gamepadMove;
            moveInput = ApplyDeadzone(moveInput);

            // Button inputs
            if (Keyboard.current != null)
            {
                if (Keyboard.current.spaceKey.wasPressedThisFrame) jumpPressed = true;
                if (Keyboard.current.escapeKey.wasPressedThisFrame) pausePressed = true;
                if (Keyboard.current.eKey.wasPressedThisFrame) interactPressed = true;
            }

            if (Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame) attackPressed = true;
            }

            if (enableGamepadSupport && Gamepad.current != null)
            {
                if (Gamepad.current.aButton.wasPressedThisFrame) jumpPressed = true;
                if (Gamepad.current.xButton.wasPressedThisFrame) attackPressed = true;
                if (Gamepad.current.startButton.wasPressedThisFrame) pausePressed = true;
                if (Gamepad.current.yButton.wasPressedThisFrame) interactPressed = true;
            }
        }

        #endregion

        #region Input Events

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            jumpPressed = true;
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            attackPressed = true;
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            pausePressed = true;
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            interactPressed = true;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Aplica deadzone no input
        /// </summary>
        private Vector2 ApplyDeadzone(Vector2 input)
        {
            if (input.magnitude < deadzone)
            {
                return Vector2.zero;
            }

            return input.normalized * ((input.magnitude - deadzone) / (1f - deadzone));
        }

        /// <summary>
        /// Verifica se qualquer input foi pressionado
        /// </summary>
        public bool AnyInputPressed()
        {
            return jumpPressed || attackPressed || pausePressed || interactPressed || moveInput.magnitude > 0f;
        }

        /// <summary>
        /// Verifica se qualquer tecla foi pressionada (para UI)
        /// </summary>
        public bool AnyKeyPressed()
        {
            bool keyboardInput = false;
            bool gamepadInput = false;
            bool mouseInput = false;

            if (Keyboard.current != null)
            {
                keyboardInput = Keyboard.current.anyKey.wasPressedThisFrame;
            }

            if (enableGamepadSupport && Gamepad.current != null)
            {
                gamepadInput = Gamepad.current.aButton.wasPressedThisFrame ||
                              Gamepad.current.bButton.wasPressedThisFrame ||
                              Gamepad.current.xButton.wasPressedThisFrame ||
                              Gamepad.current.yButton.wasPressedThisFrame ||
                              Gamepad.current.startButton.wasPressedThisFrame ||
                              Gamepad.current.selectButton.wasPressedThisFrame ||
                              Gamepad.current.leftStick.ReadValue().magnitude > 0.1f ||
                              Gamepad.current.rightStick.ReadValue().magnitude > 0.1f;
            }

            if (Mouse.current != null)
            {
                mouseInput = Mouse.current.leftButton.wasPressedThisFrame ||
                           Mouse.current.rightButton.wasPressedThisFrame ||
                           Mouse.current.middleButton.wasPressedThisFrame;
            }

            return keyboardInput || gamepadInput || mouseInput;
        }

        /// <summary>
        /// Obtém a posição do mouse na tela
        /// </summary>
        public Vector2 GetMousePosition()
        {
            if (Mouse.current != null)
            {
                return Mouse.current.position.ReadValue();
            }
            return Vector2.zero;
        }

        /// <summary>
        /// Obtém a posição do mouse no mundo
        /// </summary>
        public Vector3 GetMouseWorldPosition(Camera camera = null)
        {
            if (camera == null)
                camera = Camera.main;

            if (camera != null && Mouse.current != null)
            {
                Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
                return camera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, camera.nearClipPlane));
            }

            return Vector3.zero;
        }

        #endregion

        #region Device Management

        /// <summary>
        /// Habilita/desabilita suporte a gamepad
        /// </summary>
        public void SetGamepadSupport(bool enabled)
        {
            enableGamepadSupport = enabled;
            Log($"Gamepad support {(enabled ? "enabled" : "disabled")}");
        }

        /// <summary>
        /// Define a deadzone para inputs analógicos
        /// </summary>
        public void SetDeadzone(float newDeadzone)
        {
            deadzone = Mathf.Clamp01(newDeadzone);
            Log($"Deadzone set to {deadzone}");
        }

        #endregion

        #region Cleanup

        private void OnDestroy()
        {
            // Limpa os event listeners
            if (jumpAction != null)
                jumpAction.performed -= OnJumpPerformed;

            if (attackAction != null)
                attackAction.performed -= OnAttackPerformed;

            if (pauseAction != null)
                pauseAction.performed -= OnPausePerformed;

            if (interactAction != null)
                interactAction.performed -= OnInteractPerformed;

            if (inputActions != null)
                inputActions.Disable();
        }

        #endregion
    }
}