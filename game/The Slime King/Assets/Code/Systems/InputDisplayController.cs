using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ExtraTools
{
    /// <summary>
    /// InputDisplayController - Gerencia a exibição visual dos ícones de controle
    /// baseado no dispositivo de entrada atualmente ativo.
    /// 
    /// Integra-se com o InputManager existente para detectar mudanças de dispositivo
    /// e atualizar automaticamente a interface para mostrar apenas o ícone correspondente.
    /// 
    /// Funcionalidades:
    /// - Detecção automática do tipo de controle (Xbox, PlayStation, Switch, Teclado)
    /// - Fallback padrão para teclado quando não consegue identificar o dispositivo
    /// - Atualização em tempo real quando o jogador troca de dispositivo
    /// - Integração com a arquitetura de eventos do InputManager
    /// - Performance otimizada com verificações periódicas
    /// </summary>
    public class InputDisplayController : MonoBehaviour
    {
        #region Inspector Fields
        [Header("Input Icons")]
        [SerializeField] private GameObject gamepadIcon;
        [SerializeField] private GameObject playstationIcon;
        [SerializeField] private GameObject switchIcon;
        [SerializeField] private GameObject xboxIcon;
        [SerializeField] private GameObject keyboardIcon;

        [Header("Settings")]
        [SerializeField] private float updateInterval = 0.3f;
        [SerializeField] private bool autoDetectOnStart = true;
        [SerializeField] private bool logDeviceChanges = false;
        #endregion

        #region Private Fields
        private InputDevice lastActiveDevice;
        private float lastUpdateTime;
        private ControllerType currentControllerType = ControllerType.Keyboard;
        #endregion

        #region Public Types
        public enum ControllerType
        {
            Keyboard,
            Xbox,
            PlayStation,
            Switch,
            GenericGamepad
        }
        #endregion

        #region Events
        /// <summary>Evento disparado quando o tipo de controle muda</summary>
        public System.Action<ControllerType> OnControllerTypeChanged;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            if (autoDetectOnStart)
            {
                DetectAndUpdateInputDisplay();
            }

            // Registra callbacks para mudanças de dispositivo
            InputSystem.onDeviceChange += OnDeviceChange;

            // Se o InputManager existir, conecta aos seus eventos
            if (InputManager.Instance != null)
            {
                ConnectToInputManager();
            }
        }

        private void OnDestroy()
        {
            // Remove callbacks ao destruir o objeto
            InputSystem.onDeviceChange -= OnDeviceChange;

            if (InputManager.Instance != null)
            {
                DisconnectFromInputManager();
            }
        }

        private void Update()
        {
            // Verifica periodicamente se houve mudança de dispositivo ativo
            if (Time.time - lastUpdateTime > updateInterval)
            {
                DetectAndUpdateInputDisplay();
                lastUpdateTime = Time.time;
            }
        }
        #endregion

        #region InputManager Integration
        private void ConnectToInputManager()
        {
            // Conecta aos eventos do InputManager para detectar uso de UI ou Gameplay
            InputManager.Instance.OnNavigate += OnInputUsed;
            InputManager.Instance.OnSubmit += OnInputUsed;
            InputManager.Instance.OnCancel += OnInputUsed;
            InputManager.Instance.OnMove += OnInputUsed;
        }

        private void DisconnectFromInputManager()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnNavigate -= OnInputUsed;
                InputManager.Instance.OnSubmit -= OnInputUsed;
                InputManager.Instance.OnCancel -= OnInputUsed;
                InputManager.Instance.OnMove -= OnInputUsed;
            }
        }

        private void OnInputUsed(Vector2 _) => ForceUpdateDisplay();
        private void OnInputUsed() => ForceUpdateDisplay();
        #endregion

        #region Device Detection
        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added ||
                change == InputDeviceChange.Enabled ||
                change == InputDeviceChange.Reconnected)
            {
                DetectAndUpdateInputDisplay();
            }
        }

        private void DetectAndUpdateInputDisplay()
        {
            InputDevice activeDevice = GetActiveInputDevice();

            if (activeDevice != lastActiveDevice)
            {
                lastActiveDevice = activeDevice;
                ControllerType newControllerType = GetControllerType(activeDevice);

                if (newControllerType != currentControllerType)
                {
                    currentControllerType = newControllerType;
                    UpdateIconDisplay(currentControllerType);
                    OnControllerTypeChanged?.Invoke(currentControllerType);

                    if (logDeviceChanges)
                    {
                        Debug.Log($"[InputDisplay] Dispositivo alterado para: {currentControllerType} ({activeDevice?.displayName ?? "None"})");
                    }
                }
            }
        }

        private InputDevice GetActiveInputDevice()
        {
            // Prioridade 1: Verifica se alguma tecla do teclado foi pressionada recentemente
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.anyKey.isPressed)
            {
                return keyboard;
            }

            // Prioridade 2: Verifica gamepads conectados e com entrada recente
            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad != null && IsGamepadActive(gamepad))
                {
                    return gamepad;
                }
            }

            // Prioridade 3: Retorna o gamepad atual se disponível
            if (Gamepad.current != null)
            {
                return Gamepad.current;
            }

            // Fallback: Retorna teclado
            return keyboard;
        }

        private bool IsGamepadActive(Gamepad gamepad)
        {
            // Verifica se algum controle do gamepad está sendo usado
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

        private ControllerType GetControllerType(InputDevice device)
        {
            if (device == null)
                return ControllerType.Keyboard;

            // Verifica o tipo básico do dispositivo
            switch (device)
            {
                case Keyboard _:
                    return ControllerType.Keyboard;

                case Gamepad gamepad:
                    // Detecção baseada no nome do dispositivo
                    return DetectControllerByName(gamepad.displayName);

                default:
                    return ControllerType.Keyboard;
            }
        }

        private ControllerType DetectControllerByName(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName))
                return ControllerType.Keyboard;

            string name = deviceName.ToLower();

            // Detecção Xbox (inclui variantes comuns)
            if (name.Contains("xbox") || name.Contains("xinput") || name.Contains("360") ||
                name.Contains("one") || name.Contains("series") || name.Contains("microsoft"))
                return ControllerType.Xbox;

            // Detecção PlayStation (inclui variantes comuns)
            if (name.Contains("playstation") || name.Contains("dualshock") ||
                name.Contains("dualsense") || name.Contains("ps4") || name.Contains("ps5") ||
                name.Contains("sony") || name.Contains("wireless controller"))
                return ControllerType.PlayStation;

            // Detecção Nintendo Switch (inclui variantes comuns)
            if (name.Contains("switch") || name.Contains("pro controller") || name.Contains("joycon") ||
                name.Contains("joy-con") || name.Contains("nintendo"))
                return ControllerType.Switch;

            // Fallback: Se não conseguir identificar o gamepad, assume teclado
            return ControllerType.Keyboard;
        }
        #endregion

        #region Icon Management
        private void UpdateIconDisplay(ControllerType controllerType)
        {
            // Esconde todos os ícones primeiro
            HideAllIcons();

            // Exibe apenas o ícone correspondente
            switch (controllerType)
            {
                case ControllerType.Keyboard:
                    ShowIcon(keyboardIcon);
                    break;

                case ControllerType.Xbox:
                    ShowIcon(xboxIcon);
                    break;

                case ControllerType.PlayStation:
                    ShowIcon(playstationIcon);
                    break;

                case ControllerType.Switch:
                    ShowIcon(switchIcon);
                    break;

                case ControllerType.GenericGamepad:
                    ShowIcon(gamepadIcon);
                    break;
            }
        }

        private void HideAllIcons()
        {
            SetIconVisibility(gamepadIcon, false);
            SetIconVisibility(playstationIcon, false);
            SetIconVisibility(switchIcon, false);
            SetIconVisibility(xboxIcon, false);
            SetIconVisibility(keyboardIcon, false);
        }

        private void ShowIcon(GameObject icon)
        {
            SetIconVisibility(icon, true);
        }

        private void SetIconVisibility(GameObject icon, bool visible)
        {
            if (icon != null)
            {
                icon.SetActive(visible);
            }
            else if (visible && logDeviceChanges)
            {
                Debug.LogWarning($"[InputDisplay] Ícone não configurado para {currentControllerType}");
            }
        }
        #endregion

        #region Public API
        /// <summary>
        /// Força uma atualização manual da detecção de dispositivo
        /// </summary>
        public void ForceUpdateDisplay()
        {
            DetectAndUpdateInputDisplay();
        }

        /// <summary>
        /// Define manualmente um tipo específico de controle (bypassa detecção automática)
        /// </summary>
        /// <param name="type">Tipo de controle a ser exibido</param>
        public void SetControllerType(ControllerType type)
        {
            currentControllerType = type;
            UpdateIconDisplay(type);
            OnControllerTypeChanged?.Invoke(type);
        }

        /// <summary>
        /// Obtém o tipo de controle atualmente detectado
        /// </summary>
        public ControllerType GetCurrentControllerType()
        {
            return currentControllerType;
        }

        /// <summary>
        /// Verifica se um tipo específico de controle está conectado
        /// </summary>
        public bool IsControllerTypeConnected(ControllerType type)
        {
            switch (type)
            {
                case ControllerType.Keyboard:
                    return Keyboard.current != null;

                case ControllerType.Xbox:
                    return Gamepad.all.Any(g => GetControllerType(g) == ControllerType.Xbox);

                case ControllerType.PlayStation:
                    return Gamepad.all.Any(g => GetControllerType(g) == ControllerType.PlayStation);

                case ControllerType.Switch:
                    return Gamepad.all.Any(g => GetControllerType(g) == ControllerType.Switch);

                case ControllerType.GenericGamepad:
                    return Gamepad.all.Any(g => GetControllerType(g) == ControllerType.GenericGamepad);

                default:
                    return false;
            }
        }
        #endregion

        #region Editor Helpers
#if UNITY_EDITOR
        [Space]
        [Header("Debug (Editor Only)")]
        [SerializeField] private bool showDebugInfo = false;

        private void OnGUI()
        {
            if (!showDebugInfo) return;

            var style = new GUIStyle(GUI.skin.box);
            style.fontSize = 12;
            style.normal.textColor = Color.white;

            GUILayout.BeginArea(new Rect(10, 10, 300, 150), style);
            GUILayout.Label("Input Display Debug");
            GUILayout.Label($"Tipo Atual: {currentControllerType}");
            GUILayout.Label($"Dispositivo: {lastActiveDevice?.displayName ?? "None"}");
            GUILayout.Label($"Gamepads: {Gamepad.all.Count}");
            GUILayout.Label($"Teclado: {(Keyboard.current != null ? "Conectado" : "Desconectado")}");
            GUILayout.EndArea();
        }
#endif
        #endregion
    }
}