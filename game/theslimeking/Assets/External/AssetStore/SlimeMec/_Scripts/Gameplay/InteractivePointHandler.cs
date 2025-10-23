using UnityEngine;
using UnityEngine.InputSystem;
using SlimeMec.Visual;
using System.Collections;

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

        [Header("Fade Animation Settings")]
        [Tooltip("Duração do fade in/out dos botões (segundos)")]
        [SerializeField] private float fadeDuration = 0.3f;

        [Tooltip("Curva de animação para o fade")]
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Outline Settings")]
        [SerializeField] private OutlineController outlineController; // Controlador de outline otimizado
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
        protected InputType _currentInputType = InputType.Keyboard;
        private InputType _lastInputType = InputType.Keyboard;

        // Timer para verificação de input
        private float _inputCheckTimer = 0f;

        // Sistema de fade
        private Coroutine _currentFadeCoroutine;
        private Transform _currentActiveButton;
        private bool _isVisible = false;

        // Hash da tag para performance
        private static readonly int PlayerTagHash = "Player".GetHashCode();
        #endregion

        #region Protected Properties
        /// <summary>
        /// Propriedade protegida para acesso ao estado de proximidade do Player por classes filhas.
        /// </summary>
        protected bool IsPlayerInRange => _isPlayerInRange;
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
        protected void InitializeComponents()
        {
            // Auto-encontra os subobjetos se não foram configurados
            if (keyboardButtons == null) keyboardButtons = transform.Find("keyboard");
            if (gamepadButtons == null) gamepadButtons = transform.Find("gamepad");
            if (playstationButtons == null) playstationButtons = transform.Find("playstation");
            if (xboxButtons == null) xboxButtons = transform.Find("xbox");
            if (switchButtons == null) switchButtons = transform.Find("switch");

            // Auto-encontra o OutlineController se não foi configurado
            if (outlineController == null)
            {
                outlineController = GetComponent<OutlineController>();
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
                outlineController.UpdateOutlineColor(interactionOutlineColor);
                outlineController.ShowOutline(false); // Inicia desativado
            }

            // Desativa todos os renderers inicialmente
            HideAllButtonsImmediate();
            _isPlayerInRange = false;

            // Detecta o tipo de input inicial
            _currentInputType = DetectCurrentInputType();
        }

        /// <summary>
        /// Detecta o tipo de input atualmente sendo usado pelo jogador.
        /// </summary>
        protected InputType DetectCurrentInputType()
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
        /// Desativa todos os botões imediatamente (usado na inicialização).
        /// </summary>
        private void HideAllButtonsImmediate()
        {
            // Desativa todos os renderers diretamente
            if (_keyboardRenderer != null) _keyboardRenderer.enabled = false;
            if (_gamepadRenderer != null) _gamepadRenderer.enabled = false;
            if (_playstationRenderer != null) _playstationRenderer.enabled = false;
            if (_xboxRenderer != null) _xboxRenderer.enabled = false;
            if (_switchRenderer != null) _switchRenderer.enabled = false;

            // Também desativa usando SpriteRenderer se existir
            SetButtonAlpha(keyboardButtons, 0f);
            SetButtonAlpha(gamepadButtons, 0f);
            SetButtonAlpha(playstationButtons, 0f);
            SetButtonAlpha(xboxButtons, 0f);
            SetButtonAlpha(switchButtons, 0f);

            // Reseta o estado
            _isVisible = false;
            _currentActiveButton = null;
        }

        /// <summary>
        /// Define o alpha de um botão específico.
        /// </summary>
        private void SetButtonAlpha(Transform buttonTransform, float alpha)
        {
            if (buttonTransform == null) return;

            SpriteRenderer spriteRenderer = buttonTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
                spriteRenderer.enabled = alpha > 0f;
            }
        }

        /// <summary>
        /// Desativa todos os renderers de botões com fade suave.
        /// </summary>
        protected void HideAllButtons()
        {
            if (!_isVisible) return;

            // Para qualquer fade em progresso
            if (_currentFadeCoroutine != null)
            {
                StopCoroutine(_currentFadeCoroutine);
            }

            // Inicia fade out do botão ativo
            if (_currentActiveButton != null)
            {
                _currentFadeCoroutine = StartCoroutine(FadeOut(_currentActiveButton));
            }

            _isVisible = false;
        }

        /// <summary>
        /// Ativa a exibição dos botões de interação com fade suave baseado no input atual.
        /// </summary>
        protected void ShowInteractionButtons()
        {
            // Obtém o transform correspondente ao input atual
            Transform targetButton = GetButtonTransformForInputType(_currentInputType);

            // Se já está visível com o mesmo botão, não precisa fazer nada
            if (_isVisible && _currentActiveButton == targetButton && targetButton != null)
                return;

            // Para qualquer fade em progresso
            if (_currentFadeCoroutine != null)
            {
                StopCoroutine(_currentFadeCoroutine);
            }

            // Se há um botão ativo diferente, fade out primeiro
            if (_currentActiveButton != null && _currentActiveButton != targetButton)
            {
                _currentFadeCoroutine = StartCoroutine(FadeOutThenIn(_currentActiveButton, targetButton));
            }
            else if (targetButton != null)
            {
                // Fade in direto do novo botão
                _currentActiveButton = targetButton;
                _currentFadeCoroutine = StartCoroutine(FadeIn(targetButton));
            }
            else
            {
                // Fallback para keyboard se o botão específico não existir
                if (keyboardButtons != null)
                {
                    _currentActiveButton = keyboardButtons;
                    _currentFadeCoroutine = StartCoroutine(FadeIn(keyboardButtons));
                }
            }

            _isPlayerInRange = true;
            _isVisible = true;

            // Ativa o outline de interação
            if (outlineController != null && enableOutlineOnInteraction)
            {
                outlineController.ShowOutline(true);

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
                outlineController.ShowOutline(false);

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

        /// <summary>
        /// Obtém o transform correspondente ao tipo de input.
        /// </summary>
        private Transform GetButtonTransformForInputType(InputType inputType)
        {
            return inputType switch
            {
                InputType.Keyboard => keyboardButtons,
                InputType.Gamepad => gamepadButtons,
                InputType.PlayStation => playstationButtons,
                InputType.Xbox => xboxButtons,
                InputType.Switch => switchButtons,
                _ => keyboardButtons // Fallback
            };
        }

        #endregion

        #region Fade Animation Methods
        /// <summary>
        /// Faz fade in de um botão específico.
        /// </summary>
        private IEnumerator FadeIn(Transform buttonTransform)
        {
            if (buttonTransform == null) yield break;

            SpriteRenderer spriteRenderer = buttonTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) yield break;

            // Ativa o renderer e define alpha inicial
            spriteRenderer.enabled = true;
            Color startColor = spriteRenderer.color;
            startColor.a = 0f;
            spriteRenderer.color = startColor;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                float curveValue = fadeCurve.Evaluate(t);

                Color currentColor = spriteRenderer.color;
                currentColor.a = curveValue;
                spriteRenderer.color = currentColor;

                yield return null;
            }

            // Garante alpha final
            Color finalColor = spriteRenderer.color;
            finalColor.a = 1f;
            spriteRenderer.color = finalColor;

            _currentFadeCoroutine = null;
        }

        /// <summary>
        /// Faz fade out de um botão específico.
        /// </summary>
        private IEnumerator FadeOut(Transform buttonTransform)
        {
            if (buttonTransform == null) yield break;

            SpriteRenderer spriteRenderer = buttonTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) yield break;

            Color startColor = spriteRenderer.color;
            float startAlpha = startColor.a;

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                float curveValue = fadeCurve.Evaluate(1f - t); // Inverte a curva para fade out

                Color currentColor = spriteRenderer.color;
                currentColor.a = startAlpha * curveValue;
                spriteRenderer.color = currentColor;

                yield return null;
            }

            // Desativa o renderer no final
            spriteRenderer.enabled = false;
            _currentActiveButton = null;
            _currentFadeCoroutine = null;
        }

        /// <summary>
        /// Faz fade out de um botão e depois fade in de outro.
        /// </summary>
        private IEnumerator FadeOutThenIn(Transform outButton, Transform inButton)
        {
            // Fade out do botão atual
            yield return StartCoroutine(FadeOut(outButton));

            // Fade in do novo botão
            if (inButton != null)
            {
                _currentActiveButton = inButton;
                yield return StartCoroutine(FadeIn(inButton));
            }
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
            // Verifica se é o Player usando CompareTag para performance
            if (other.CompareTag("Player"))
            {
                _currentInputType = DetectCurrentInputType();
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
                outlineController.ShowOutline(true);
                Debug.Log("Outline forçado ON");
            }
        }

        [ContextMenu("Test Disable Outline")]
        private void TestDisableOutline()
        {
            if (Application.isPlaying && outlineController != null)
            {
                outlineController.ShowOutline(false);
                Debug.Log("Outline forçado OFF");
            }
        }
#endif
        #endregion
    }
}
