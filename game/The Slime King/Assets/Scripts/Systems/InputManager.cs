using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace SlimeKing.Systems
{
    /// <summary>
    /// Tipos de dispositivos de input suportados
    /// </summary>
    public enum InputType
    {
        Keyboard,
        Gamepad,
        Xbox,
        PlayStation,
        Switch
    }

    /// <summary>
    /// Manager de input simplificado usando o novo Input System.
    /// Fornece abstração para controles de teclado e gamepad.
    /// </summary>
    public class InputManager : Core.ManagerBase<InputManager>
    {
        [Header("Input Settings")]
        [SerializeField] private bool enableGamepadSupport = true;
        [SerializeField] private float deadzone = 0.2f;
        [SerializeField] private float inputTypeChangeDelay = 0.5f; // Delay mínimo entre mudanças de tipo
        [SerializeField] private bool preferGamepadOnStartup = true; // Se deve priorizar gamepad na inicialização

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

        // Input type tracking
        private InputType currentInputType = InputType.Keyboard;
        private InputType lastInputType = InputType.Keyboard;
        private float lastInputTypeChangeTime = 0f;

        // Properties para acesso rápido
        public Vector2 MoveInput => moveInput;
        public bool JumpPressed => jumpPressed;
        public bool AttackPressed => attackPressed;
        public bool PausePressed => pausePressed;
        public bool InteractPressed => interactPressed;

        // Device info
        public bool HasGamepad => Gamepad.current != null;
        public bool HasKeyboard => Keyboard.current != null;

        // Input type
        public InputType CurrentInputType => currentInputType;

        protected override void Initialize()
        {
            SetupInputActions();

            // Detecta automaticamente o dispositivo inicial
            InitialDeviceDetection();

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

        /// <summary>
        /// Detecta automaticamente o dispositivo inicial baseado na presença
        /// </summary>
        private void InitialDeviceDetection()
        {
            bool hasGamepad = enableGamepadSupport && Gamepad.current != null;
            bool hasKeyboard = Keyboard.current != null;

            if (preferGamepadOnStartup && hasGamepad)
            {
                // Prioridade: Gamepad (se preferido e conectado)
                currentInputType = DetectGamepadType(Gamepad.current);
                Log($"Initial device detected: {currentInputType} (Gamepad preferred)");
            }
            else if (hasKeyboard)
            {
                // Prioridade: Teclado
                currentInputType = InputType.Keyboard;
                Log($"Initial device detected: {currentInputType} (Keyboard)");
            }
            else if (hasGamepad)
            {
                // Fallback: Gamepad se teclado não disponível
                currentInputType = DetectGamepadType(Gamepad.current);
                Log($"Initial device detected: {currentInputType} (Gamepad fallback)");
            }
            else
            {
                // Fallback final: Teclado mesmo se não detectado
                currentInputType = InputType.Keyboard;
                Log($"No devices detected, defaulting to: {currentInputType}");
            }
        }

        #endregion

        #region Update

        private void Update()
        {
            UpdateInputs();
            UpdateInputType();
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

            if (enableGamepadSupport && Gamepad.current != null)
            {
                if (Gamepad.current.aButton.wasPressedThisFrame) jumpPressed = true;
                if (Gamepad.current.xButton.wasPressedThisFrame) attackPressed = true;
                if (Gamepad.current.startButton.wasPressedThisFrame) pausePressed = true;
                if (Gamepad.current.yButton.wasPressedThisFrame) interactPressed = true;
            }
        }

        /// <summary>
        /// Atualiza o tipo de input baseado no dispositivo ativo
        /// </summary>
        private void UpdateInputType()
        {
            // Só permite mudanças após o delay mínimo
            if (Time.time - lastInputTypeChangeTime < inputTypeChangeDelay)
                return;

            InputType detectedType = DetectCurrentInputType();

            if (detectedType != currentInputType)
            {
                lastInputType = currentInputType;
                currentInputType = detectedType;
                lastInputTypeChangeTime = Time.time;
                Log($"Input type changed from {lastInputType} to {currentInputType}");
            }
        }

        /// <summary>
        /// Detecta o tipo de input baseado no dispositivo ativo
        /// </summary>
        private InputType DetectCurrentInputType()
        {
            // Verifica se há gamepad conectado e ativo
            if (Gamepad.current != null)
            {
                // Verifica se há input no gamepad
                bool gamepadActive =
                    Gamepad.current.leftStick.ReadValue().magnitude > deadzone ||
                    Gamepad.current.rightStick.ReadValue().magnitude > deadzone ||
                    AnyGamepadButtonPressed();

                if (gamepadActive)
                {
                    // Detecta tipo específico de gamepad
                    return DetectGamepadType(Gamepad.current);
                }
            }

            // Verifica se há input de teclado
            bool keyboardActive = false;
            if (Keyboard.current != null)
            {
                keyboardActive = Keyboard.current.anyKey.isPressed;
            }

            if (keyboardActive)
            {
                return InputType.Keyboard;
            }

            // IMPORTANTE: Mantém o tipo atual quando não há atividade
            // Só muda quando há input ativo de um dispositivo diferente
            return currentInputType;
        }

        /// <summary>
        /// Detecta o tipo específico de gamepad
        /// </summary>
        private InputType DetectGamepadType(Gamepad gamepad)
        {
            string deviceName = gamepad.device.displayName.ToLower();

            if (deviceName.Contains("xbox") || deviceName.Contains("microsoft"))
                return InputType.Xbox;

            if (deviceName.Contains("playstation") || deviceName.Contains("dualshock") || deviceName.Contains("dualsense"))
                return InputType.PlayStation;

            if (deviceName.Contains("switch") || deviceName.Contains("nintendo"))
                return InputType.Switch;

            // Fallback para gamepad genérico
            return InputType.Gamepad;
        }

        /// <summary>
        /// Verifica se algum botão do gamepad foi pressionado
        /// </summary>
        private bool AnyGamepadButtonPressed()
        {
            if (Gamepad.current == null) return false;

            return Gamepad.current.aButton.isPressed ||
                   Gamepad.current.bButton.isPressed ||
                   Gamepad.current.xButton.isPressed ||
                   Gamepad.current.yButton.isPressed ||
                   Gamepad.current.startButton.isPressed ||
                   Gamepad.current.selectButton.isPressed ||
                   Gamepad.current.leftShoulder.isPressed ||
                   Gamepad.current.rightShoulder.isPressed ||
                   Gamepad.current.leftTrigger.ReadValue() > deadzone ||
                   Gamepad.current.rightTrigger.ReadValue() > deadzone;
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

            return keyboardInput || gamepadInput;
        }

        #endregion

        #region Device Management

        /// <summary>
        /// Re-detecta o dispositivo inicial (útil quando dispositivos são conectados/desconectados)
        /// </summary>
        public void RefreshDeviceDetection()
        {
            lastInputTypeChangeTime = 0f; // Reset do delay para permitir mudança imediata
            InitialDeviceDetection();
        }

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