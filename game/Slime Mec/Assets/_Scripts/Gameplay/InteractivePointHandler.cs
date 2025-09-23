using UnityEngine;
using UnityEngine.InputSystem;
using SlimeMec.Visual;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Tipos de input suportados para exibição de botões específicos.
    /// </summary>
    public enum InputType
    {
        Keyboard,     // Fallback padrão
        Gamepad,      // Controlador genérico
        PlayStation,  // DualShock/DualSense
        Xbox,         // Xbox Controller
        Switch        // Nintendo Switch Pro Controller
    }

    /// <summary>
    /// Controla a exibição de botões de interação específicos baseado no tipo de input do jogador.
    /// Detecta automaticamente o tipo de controlador e exibe apenas os botões correspondentes.
    /// 
    /// SISTEMA DE DETECÇÃO DE INPUT:
    /// • Detecta objetos com tag "Player" em trigger collision 2D
    /// • Identifica automaticamente o tipo de input sendo usado
    /// • Exibe apenas o subobjeto correspondente ao input ativo
    /// • Fallback para keyboard se nenhum controlador for detectado
    /// 
    /// TIPOS DE INPUT SUPORTADOS:
    /// • Keyboard: Teclado/Mouse (fallback padrão)
    /// • Gamepad: Controlador genérico
    /// • PlayStation: DualShock 4/DualSense
    /// • Xbox: Xbox One/Series Controller
    /// • Switch: Nintendo Switch Pro Controller
    /// 
    /// CONFIGURAÇÃO NECESSÁRIA:
    /// • GameObject deve ter Collider2D com isTrigger = true
    /// • 5 subobjetos filhos: "keyboard", "gamepad", "playstation", "xbox", "switch"
    /// • Todos os subobjetos devem ter Renderer desativado inicialmente
    /// • Input System Package instalado
    /// </summary>
    public class InteractivePointHandler : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Interactive Elements")]
        [SerializeField] private Transform keyboardButtons;     // Botões de teclado/mouse
        [SerializeField] private Transform gamepadButtons;      // Botões de controlador genérico
        [SerializeField] private Transform playstationButtons;  // Botões de PlayStation
        [SerializeField] private Transform xboxButtons;         // Botões de Xbox
        [SerializeField] private Transform switchButtons;       // Botões de Nintendo Switch

        [Header("Outline Effect")]
        [SerializeField] private OutlineShaderController outlineController; // Controlador de outline via shader
        [SerializeField] private bool enableOutlineOnInteraction = true;    // Ativar outline quando player se aproxima
        [SerializeField] private Color interactionOutlineColor = Color.cyan; // Cor do outline de interação

        [Header("Settings")]
        [SerializeField] private bool enableDebugLogs = false; // Para debug opcional
        [SerializeField] private float inputCheckInterval = 0.1f; // Intervalo para verificar mudanças de input
        #endregion

        #region Private Fields
        // Cache dos renderers para performance
        private Renderer _keyboardRenderer;
        private Renderer _gamepadRenderer;
        private Renderer _playstationRenderer;
        private Renderer _xboxRenderer;
        private Renderer _switchRenderer;

        // Estado atual da interação
        private bool _isPlayerInRange = false;
        private InputType _currentInputType = InputType.Keyboard;
        private InputType _lastInputType = InputType.Keyboard;

        // Timer para verificação de input
        private float _inputCheckTimer = 0f;

        // Hash da tag para performance
        private static readonly int PlayerTagHash = "Player".GetHashCode();
        #endregion

        #region Unity Lifecycle
        void Start()
        {
            InitializeComponents();
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Inicializa os componentes necessários e validações.
        /// </summary>
        private void InitializeComponents()
        {
            // Auto-encontra os subobjetos se não foram configurados
            if (keyboardButtons == null) keyboardButtons = transform.Find("keyboard");
            if (gamepadButtons == null) gamepadButtons = transform.Find("gamepad");
            if (playstationButtons == null) playstationButtons = transform.Find("playstation");
            if (xboxButtons == null) xboxButtons = transform.Find("xbox");
            if (switchButtons == null) switchButtons = transform.Find("switch");

            // Auto-encontra o OutlineShaderController se não foi configurado
            if (outlineController == null)
            {
                outlineController = GetComponent<OutlineShaderController>();
            }

            // Valida que pelo menos keyboard existe (fallback obrigatório)
            if (keyboardButtons == null)
            {
                Debug.LogError($"InteractivePointHandler: Subobjeto 'keyboard' não encontrado em '{gameObject.name}'. " +
                               "Este é o fallback obrigatório!");
                enabled = false;
                return;
            }

            // Obtém todos os renderers
            _keyboardRenderer = keyboardButtons?.GetComponent<Renderer>();
            _gamepadRenderer = gamepadButtons?.GetComponent<Renderer>();
            _playstationRenderer = playstationButtons?.GetComponent<Renderer>();
            _xboxRenderer = xboxButtons?.GetComponent<Renderer>();
            _switchRenderer = switchButtons?.GetComponent<Renderer>();

            // Valida que keyboard tem renderer (obrigatório)
            if (_keyboardRenderer == null)
            {
                Debug.LogError($"InteractivePointHandler: Renderer não encontrado no subobjeto 'keyboard' de '{gameObject.name}'");
                enabled = false;
                return;
            }

            // Configura o outline controller se disponível
            if (outlineController != null && enableOutlineOnInteraction)
            {
                outlineController.SetOutlineColor(interactionOutlineColor);
                outlineController.DisableOutline(); // Inicia desativado
            }

            // Desativa todos os renderers inicialmente
            HideAllButtons();
            _isPlayerInRange = false;

            // Detecta o tipo de input inicial
            _currentInputType = DetectCurrentInputType();
            _lastInputType = _currentInputType;
        }

        /// <summary>
        /// Detecta o tipo de input atualmente sendo usado pelo jogador.
        /// </summary>
        private InputType DetectCurrentInputType()
        {
            // Verifica se há controles conectados
            if (Gamepad.current != null)
            {
                Debug.Log("Gamepad conectado");
                string deviceName = Gamepad.current.displayName.ToLower();
                Debug.Log(deviceName);

                // Identifica por nome do dispositivo
                if (deviceName.Contains("dualshock") || deviceName.Contains("dualsense") ||
                    deviceName.Contains("playstation") || deviceName.Contains("ps4") || deviceName.Contains("ps5"))
                    return InputType.PlayStation;

                if (deviceName.Contains("xbox") || deviceName.Contains("xinput"))
                    return InputType.Xbox;

                if (deviceName.Contains("switch") || deviceName.Contains("pro controller"))
                    return InputType.Switch;

                // Gamepad genérico
                return InputType.Gamepad;
            }
            else
            {
                Debug.Log("Nenhum gamepad conectado");

                if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame)
                    return InputType.Keyboard;
            }

            // Fallback
            return InputType.Keyboard;
        }

        /// <summary>
        /// Verifica se houve mudança no tipo de input e atualiza a exibição.
        /// </summary>
        private void CheckInputTypeChange()
        {
            _currentInputType = DetectCurrentInputType();

            if (_currentInputType != _lastInputType)
            {
                _lastInputType = _currentInputType;

                if (_isPlayerInRange)
                {
                    ShowInteractionButtons();
                }
            }
        }

        /// <summary>
        /// Desativa todos os renderers de botões.
        /// </summary>
        private void HideAllButtons()
        {
            if (_keyboardRenderer != null) _keyboardRenderer.enabled = false;
            if (_gamepadRenderer != null) _gamepadRenderer.enabled = false;
            if (_playstationRenderer != null) _playstationRenderer.enabled = false;
            if (_xboxRenderer != null) _xboxRenderer.enabled = false;
            if (_switchRenderer != null) _switchRenderer.enabled = false;
        }

        /// <summary>
        /// Ativa a exibição dos botões de interação baseado no input atual.
        /// </summary>
        private void ShowInteractionButtons()
        {
            // Desativa todos primeiro
            HideAllButtons();

            // Ativa apenas o renderer correspondente ao input atual
            Renderer targetRenderer = GetRendererForInputType(_currentInputType);

            if (targetRenderer != null)
            {
                targetRenderer.enabled = true;
                _isPlayerInRange = true;
            }
            else
            {
                // Fallback para keyboard se o renderer específico não existir
                if (_keyboardRenderer != null)
                {
                    _keyboardRenderer.enabled = true;
                    _isPlayerInRange = true;
                }
            }

            // Ativa o outline de interação
            if (outlineController != null && enableOutlineOnInteraction)
            {
                outlineController.EnableOutline();

                if (enableDebugLogs)
                    Debug.Log($"InteractivePointHandler: Outline ativado para '{gameObject.name}'", this);
            }
        }

        /// <summary>
        /// Desativa a exibição dos botões de interação.
        /// </summary>
        private void HideInteractionButtons()
        {
            HideAllButtons();
            _isPlayerInRange = false;

            // Desativa o outline de interação
            if (outlineController != null && enableOutlineOnInteraction)
            {
                outlineController.DisableOutline();

                if (enableDebugLogs)
                    Debug.Log($"InteractivePointHandler: Outline desativado para '{gameObject.name}'", this);
            }
        }

        /// <summary>
        /// Obtém o renderer correspondente ao tipo de input.
        /// </summary>
        private Renderer GetRendererForInputType(InputType inputType)
        {
            return inputType switch
            {
                InputType.Keyboard => _keyboardRenderer,
                InputType.Gamepad => _gamepadRenderer,
                InputType.PlayStation => _playstationRenderer,
                InputType.Xbox => _xboxRenderer,
                InputType.Switch => _switchRenderer,
                _ => _keyboardRenderer // Fallback
            };
        }
        #endregion

        #region Trigger Events
        /// <summary>
        /// Detecta quando o Player entra na área de trigger.
        /// Ativa automaticamente a exibição dos botões de interação.
        /// </summary>
        /// <param name="other">Collider que entrou no trigger</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            DetectCurrentInputType();
            ShowInteractionButtons();
            // Verifica se é o Player usando CompareTag para performance
            if (other.CompareTag("Player"))
            {
                ShowInteractionButtons();
            }

        }

        /// <summary>
        /// Detecta quando o Player sai da área de trigger.
        /// Desativa automaticamente a exibição dos botões de interação.
        /// </summary>
        /// <param name="other">Collider que saiu do trigger</param>
        private void OnTriggerExit2D(Collider2D other)
        {
            // Verifica se é o Player usando CompareTag para performance
            if (other.CompareTag("Player"))
            {
                HideInteractionButtons();
            }
        }
        #endregion

        #region Context Menu (Editor Only)
#if UNITY_EDITOR
        [ContextMenu("Test Show Buttons")]
        private void TestShowButtons()
        {
            if (Application.isPlaying)
                ShowInteractionButtons();
        }

        [ContextMenu("Test Hide Buttons")]
        private void TestHideButtons()
        {
            if (Application.isPlaying)
                HideInteractionButtons();
        }

        [ContextMenu("Force Keyboard")]
        private void ForceKeyboard()
        {
            if (Application.isPlaying)
            {
                _currentInputType = InputType.Keyboard;
                if (_isPlayerInRange) ShowInteractionButtons();
            }
        }

        [ContextMenu("Debug Info")]
        private void DebugInfo()
        {
            Debug.Log($"InteractivePointHandler Info:" +
                      $"\n• Input: {_currentInputType} | Player In Range: {_isPlayerInRange}" +
                      $"\n• Keyboard: {(_keyboardRenderer != null ? (_keyboardRenderer.enabled ? "ON" : "OFF") : "NULL")}" +
                      $"\n• PlayStation: {(_playstationRenderer != null ? (_playstationRenderer.enabled ? "ON" : "OFF") : "NULL")}" +
                      $"\n• Xbox: {(_xboxRenderer != null ? (_xboxRenderer.enabled ? "ON" : "OFF") : "NULL")}" +
                      $"\n• Outline Controller: {(outlineController != null ? "FOUND" : "NULL")}" +
                      $"\n• Outline Active: {(outlineController != null ? outlineController.IsOutlineActive : false)}" +
                      $"\n• Trigger: {(GetComponent<Collider2D>()?.isTrigger ?? false)}");
        }

        [ContextMenu("Test Enable Outline")]
        private void TestEnableOutline()
        {
            if (Application.isPlaying && outlineController != null)
            {
                outlineController.EnableOutline();
                Debug.Log("Outline forçado ON");
            }
        }

        [ContextMenu("Test Disable Outline")]
        private void TestDisableOutline()
        {
            if (Application.isPlaying && outlineController != null)
            {
                outlineController.DisableOutline();
                Debug.Log("Outline forçado OFF");
            }
        }
#endif
        #endregion
    }
}
