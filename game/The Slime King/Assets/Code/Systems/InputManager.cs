using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ExtraTools
{
    /// <summary>
    /// InputManager - Wrapper centralizado para o Input System do The Slime King.
    /// Responsável por:
    /// 1. Instanciar e gerenciar o InputSystem_Actions gerado automaticamente.
    /// 2. Expor eventos tipados para sistemas específicos (UI, Gameplay, System).
    /// 3. Controlar habilitação/desabilitação de mapas conforme contexto.
    /// 4. Fornecer API limpa para outros scripts sem dependência direta do asset gerado.
    /// 
    /// Padrões adotados:
    /// - Singleton (lifetime persistente entre cenas via DontDestroyOnLoad).
    /// - Event-driven architecture para desacoplamento.
    /// - Separação clara entre contextos (UI vs Gameplay vs System).
    /// 
    /// Uso:
    /// - Telas/Menus: Habilitar apenas UI + System
    /// - Gameplay: Habilitar Gameplay + System
    /// - TitleScreen/Cinematics: Habilitar apenas System (Skip)
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        #region Singleton
        /// <summary>
        /// Instância única ativa do InputManager. Criada no primeiro Awake encontrado.
        /// </summary>
        public static InputManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeInputManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Input System Instance
        private InputSystem_Actions inputActions;

        /// <summary>
        /// Referência direta ao asset de Input Actions (para casos específicos).
        /// Preferencialmente use os eventos públicos ao invés de acessar diretamente.
        /// </summary>
        public InputSystem_Actions InputActions => inputActions;
        #endregion

        #region Context Management
        [Header("Context Settings")]
        [SerializeField] private bool enableUIOnStart = false;
        [SerializeField] private bool enableGameplayOnStart = false; // Se ambos false, nada habilita até contexto definido

        /// <summary>
        /// Estado atual dos mapas de input
        /// </summary>
        public bool IsUIEnabled => inputActions?.UI.enabled ?? false;
        public bool IsGameplayEnabled => inputActions?.Gameplay.enabled ?? false;
        // Mapa System removido: usamos UI para navegação e eventos globais, e Gameplay para ações in-game
        #endregion

        #region UI Events
        /// <summary>Evento disparado quando o jogador navega nos menus (Vector2: direção)</summary>
        public event Action<Vector2> OnNavigate;

        /// <summary>Evento disparado quando o jogador confirma uma seleção</summary>
        public event Action OnSubmit;

        /// <summary>Evento disparado quando o jogador cancela/volta</summary>
        public event Action OnCancel;

        /// <summary>Evento disparado quando o mouse se move (Vector2: posição)</summary>
        public event Action<Vector2> OnPoint;
        #endregion

        #region Gameplay Events
        /// <summary>Evento disparado quando o jogador se move (Vector2: direção normalizada)</summary>
        public event Action<Vector2> OnMove;

        /// <summary>Evento disparado quando o jogador ataca (started, performed, canceled)</summary>
        public event Action<InputAction.CallbackContext> OnAttack;

        /// <summary>Evento disparado quando o jogador interage (started, performed, canceled)</summary>
        public event Action<InputAction.CallbackContext> OnInteract;


        /// <summary>Evento disparado quando o jogador agacha</summary>
        public event Action<InputAction.CallbackContext> OnCrouch;

        /// <summary>Eventos para uso de itens (1-4)</summary>
        public event Action OnUseItem1;
        public event Action OnUseItem2;
        public event Action OnUseItem3;
        public event Action OnUseItem4;
        #endregion

        #region System Events
        /// <summary>Evento disparado quando o jogador abre o menu (pausa implícita)</summary>
        public event Action OnMenu;

        /// <summary>Evento disparado quando o jogador abre o inventário</summary>
        public event Action OnInventory;

        /// <summary>Evento disparado quando o jogador pula/avança telas (TitleScreen, cutscenes)</summary>
        public event Action OnSkip;

        /// <summary>Evento disparado quando o dispositivo de entrada ativo muda (InputDevice: novo dispositivo)</summary>
        public event Action<InputDevice> OnDeviceChanged;
        #endregion

        #region Device Detection
        private InputDevice lastActiveDevice;
        private float deviceCheckInterval = 0.1f;
        private float lastDeviceCheckTime;
        #endregion

        #region Initialization
        /// <summary>
        /// Inicializa o InputManager: cria instância do InputSystem_Actions e configura callbacks.
        /// </summary>
        private void InitializeInputManager()
        {
            // Cria a instância do Input Actions
            inputActions = new InputSystem_Actions();

            // Configura callbacks para todos os eventos
            SetupUICallbacks();
            SetupGameplayCallbacks();
            SetupSystemCallbacks();

            // Configura estado inicial dos mapas
            SetInitialMapStates();

            Debug.Log("[InputManager] Inicializado com sucesso");
        }

        private void SetupUICallbacks()
        {
            if (inputActions?.UI == null) return;

            inputActions.UI.Navigate.performed += ctx => OnNavigate?.Invoke(ctx.ReadValue<Vector2>());
            inputActions.UI.Submit.performed += ctx => OnSubmit?.Invoke();
            inputActions.UI.Cancel.performed += ctx => OnCancel?.Invoke();
            inputActions.UI.Point.performed += ctx => OnPoint?.Invoke(ctx.ReadValue<Vector2>());
        }

        private void SetupGameplayCallbacks()
        {
            if (inputActions?.Gameplay == null) return;

            inputActions.Gameplay.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
            inputActions.Gameplay.Move.canceled += ctx => OnMove?.Invoke(Vector2.zero);

            inputActions.Gameplay.Attack.started += ctx => OnAttack?.Invoke(ctx);
            inputActions.Gameplay.Attack.performed += ctx => OnAttack?.Invoke(ctx);
            inputActions.Gameplay.Attack.canceled += ctx => OnAttack?.Invoke(ctx);

            inputActions.Gameplay.Interact.started += ctx => OnInteract?.Invoke(ctx);
            inputActions.Gameplay.Interact.performed += ctx => OnInteract?.Invoke(ctx);
            inputActions.Gameplay.Interact.canceled += ctx => OnInteract?.Invoke(ctx);


            inputActions.Gameplay.Crouch.started += ctx => OnCrouch?.Invoke(ctx);
            inputActions.Gameplay.Crouch.performed += ctx => OnCrouch?.Invoke(ctx);
            inputActions.Gameplay.Crouch.canceled += ctx => OnCrouch?.Invoke(ctx);

            inputActions.Gameplay.UseItem1.performed += ctx => OnUseItem1?.Invoke();
            inputActions.Gameplay.UseItem2.performed += ctx => OnUseItem2?.Invoke();
            inputActions.Gameplay.UseItem3.performed += ctx => OnUseItem3?.Invoke();
            inputActions.Gameplay.UseItem4.performed += ctx => OnUseItem4?.Invoke();
        }

        private void SetupSystemCallbacks()
        {
            // Não existe mais action map System. Reaproveitamos:
            // Menu  -> Gameplay.Menu ou UI.Cancel como fallback
            // Inventory -> Gameplay.Inventory
            // Skip -> UI.Submit / UI.Cancel / Gameplay.Attack (primeiro evento que ocorrer)

            if (inputActions == null) return;

            // Gameplay bindings (sempre disponíveis se inputActions instanciado)
            inputActions.Gameplay.Menu.performed += ctx => OnMenu?.Invoke();
            inputActions.Gameplay.Inventory.performed += ctx => OnInventory?.Invoke();
            inputActions.Gameplay.Attack.performed += ctx => OnSkip?.Invoke(); // fallback

            // UI bindings para Skip
            inputActions.UI.Submit.performed += ctx => OnSkip?.Invoke();
            inputActions.UI.Cancel.performed += ctx => OnSkip?.Invoke();
        }

        private void SetInitialMapStates()
        {
            // Desabilita todos os mapas inicialmente
            inputActions?.UI.Disable();
            inputActions?.Gameplay.Disable();

            // Habilita conforme configuração inicial
            if (enableUIOnStart) EnableUI();
            if (enableGameplayOnStart) EnableGameplay();
        }
        #endregion

        #region Map Control
        /// <summary>
        /// Habilita o mapa UI (navegação de menus)
        /// </summary>
        public void EnableUI()
        {
            inputActions?.UI.Enable();
            Debug.Log("[InputManager] Mapa UI habilitado");
        }

        /// <summary>
        /// Desabilita o mapa UI
        /// </summary>
        public void DisableUI()
        {
            inputActions?.UI.Disable();
            Debug.Log("[InputManager] Mapa UI desabilitado");
        }

        /// <summary>
        /// Habilita o mapa Gameplay (controle do jogador)
        /// </summary>
        public void EnableGameplay()
        {
            inputActions?.Gameplay.Enable();
            Debug.Log("[InputManager] Mapa Gameplay habilitado");
        }

        /// <summary>
        /// Desabilita o mapa Gameplay
        /// </summary>
        public void DisableGameplay()
        {
            inputActions?.Gameplay.Disable();
            Debug.Log("[InputManager] Mapa Gameplay desabilitado");
        }

        // Mapa System removido: funções de Enable/Disable System não são mais necessárias

        /// <summary>
        /// Configura mapas para contexto de menu/UI (UI + System ativos)
        /// </summary>
        public void SetMenuContext()
        {
            DisableGameplay();
            EnableUI();
            Debug.Log("[InputManager] Contexto alterado para Menu");
        }

        /// <summary>
        /// Configura mapas para contexto de gameplay (Gameplay + System ativos)
        /// </summary>
        public void SetGameplayContext()
        {
            DisableUI();
            EnableGameplay();
            Debug.Log("[InputManager] Contexto alterado para Gameplay");
        }

        /// <summary>
        /// Configura mapas para contexto de tela inicial/cutscene (apenas System ativo)
        /// </summary>
        public void SetTitleScreenContext()
        {
            // Para TitleScreen: habilita apenas UI para permitir Submit/Cancel como Skip
            DisableGameplay();
            EnableUI();
            Debug.Log("[InputManager] Contexto alterado para TitleScreen");
        }

        /// <summary>
        /// Desabilita todos os mapas (útil para pausas completas ou transições)
        /// </summary>
        public void DisableAllMaps()
        {
            DisableUI();
            DisableGameplay();
            Debug.Log("[InputManager] Todos os mapas desabilitados");
        }
        #endregion

        #region Cleanup
        private void OnDestroy()
        {
            if (inputActions != null)
            {
                inputActions.Dispose();
                inputActions = null;
            }
        }

        private void OnDisable()
        {
            inputActions?.Disable();
        }

        private void OnEnable()
        {
            // Reaplica estado dos mapas se necessário
            if (inputActions != null)
            {
                SetInitialMapStates();
            }
        }
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool debugMode = false;

        private void Update()
        {
            // Verifica mudanças de dispositivo periodicamente para performance
            if (Time.time - lastDeviceCheckTime >= deviceCheckInterval)
            {
                CheckForDeviceChange();
                lastDeviceCheckTime = Time.time;
            }

            if (debugMode)
            {
                // Log estado dos mapas quando mudam
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    Debug.Log($"[InputManager] Estado dos mapas - UI: {IsUIEnabled}, Gameplay: {IsGameplayEnabled}");
                }
            }
        }

        [UnityEngine.ContextMenu("Test Menu Context")]
        private void TestMenuContext()
        {
            if (Application.isPlaying)
                SetMenuContext();
        }

        [UnityEngine.ContextMenu("Test Gameplay Context")]
        private void TestGameplayContext()
        {
            if (Application.isPlaying)
                SetGameplayContext();
        }

        [UnityEngine.ContextMenu("Test TitleScreen Context")]
        private void TestTitleScreenContext()
        {
            if (Application.isPlaying)
                SetTitleScreenContext();
        }
#endif
        #endregion

        #region Device Detection
        /// <summary>
        /// Verifica se houve mudança no dispositivo ativo e notifica ouvintes
        /// </summary>
        private void CheckForDeviceChange()
        {
            InputDevice currentActiveDevice = GetCurrentActiveDevice();

            if (currentActiveDevice != lastActiveDevice)
            {
                lastActiveDevice = currentActiveDevice;
                OnDeviceChanged?.Invoke(currentActiveDevice);

                Debug.Log($"[InputManager] Dispositivo alterado para: {currentActiveDevice?.displayName ?? "None"}");
            }
        }

        /// <summary>
        /// Determina qual é o dispositivo atualmente ativo baseado em uso recente
        /// </summary>
        private InputDevice GetCurrentActiveDevice()
        {
            // Prioridade 1: Teclado se alguma tecla foi pressionada
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.anyKey.isPressed)
            {
                return keyboard;
            }

            // Prioridade 2: Gamepad ativo
            if (Gamepad.current != null && IsGamepadActive(Gamepad.current))
            {
                return Gamepad.current;
            }

            // Fallback: mantém último dispositivo conhecido ou teclado
            return lastActiveDevice ?? keyboard;
        }

        /// <summary>
        /// Verifica se um gamepad está sendo usado ativamente
        /// </summary>
        private bool IsGamepadActive(Gamepad gamepad)
        {
            const float deadzone = 0.1f;

            return gamepad.leftStick.ReadValue().magnitude > deadzone ||
                   gamepad.rightStick.ReadValue().magnitude > deadzone ||
                   gamepad.buttonSouth.isPressed ||
                   gamepad.buttonEast.isPressed ||
                   gamepad.buttonWest.isPressed ||
                   gamepad.buttonNorth.isPressed ||
                   gamepad.leftShoulder.isPressed ||
                   gamepad.rightShoulder.isPressed ||
                   gamepad.leftTrigger.isPressed ||
                   gamepad.rightTrigger.isPressed ||
                   gamepad.dpad.ReadValue().magnitude > deadzone;
        }
        #endregion
    }
}