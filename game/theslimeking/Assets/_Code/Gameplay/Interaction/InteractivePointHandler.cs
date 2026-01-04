using UnityEngine;
using UnityEngine.InputSystem;
using SlimeKing.Visual;
using System.Collections;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Controla a exibição de botões de interação específicos baseado no tipo de input do jogador.
    /// Detecta automaticamente entre teclado e controlador, exibindo apenas os botões correspondentes.
    /// 
    /// SISTEMA DE DETECÇÃO DE INPUT:
    /// • Detecta objetos com tag "Player" em trigger collision 2D
    /// • Identifica automaticamente se está usando teclado ou gamepad
    /// • Exibe apenas o subobjeto correspondente ao input ativo
    /// 
    /// TIPOS DE INPUT SUPORTADOS:
    /// • Keyboard: Teclado/Mouse (fallback padrão)
    /// • Gamepad: Qualquer controlador conectado
    /// 
    /// CONFIGURAÇÃO NECESSÁRIA:
    /// • GameObject deve ter Collider2D com isTrigger = true
    /// • 2 subobjetos filhos: "keyboard", "gamepad"
    /// • Todos os subobjetos devem ter SpriteRenderer desativado inicialmente
    /// • Input System Package instalado
    /// </summary>
    public class InteractivePointHandler : MonoBehaviour
    {
        #region Fields
        [Header("Interactive Elements")]
        [SerializeField] private Transform keyboardButtons;
        [SerializeField] private Transform gamepadButtons;

        [Header("Fade Animation Settings")]
        [Tooltip("Duração do fade in/out dos botões (segundos)")]
        [SerializeField] private float fadeDuration = 0.3f;
        [Tooltip("Curva de animação para o fade")]
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Outline Settings")]
        [SerializeField] private OutlineController outlineController;
        [SerializeField] private bool enableOutlineOnInteraction = true;
        [SerializeField] private Color interactionOutlineColor = Color.cyan;

        private SpriteRenderer _keyboardRenderer;
        private SpriteRenderer _gamepadRenderer;

        private bool _isPlayerInRange = false;
        protected InputType _currentInputType = InputType.Keyboard;
        private Coroutine _currentFadeCoroutine;
        private Transform _currentActiveButton;
        private bool _isVisible = false;

        protected bool IsPlayerInRange => _isPlayerInRange;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            InitializeComponents();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _currentInputType = DetectCurrentInputType();
                ShowInteractionButtons();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                HideInteractionButtons();
            }
        }
        #endregion

        #region Private Methods
        protected void InitializeComponents()
        {
            if (keyboardButtons == null) keyboardButtons = transform.Find("keyboard");
            if (gamepadButtons == null) gamepadButtons = transform.Find("gamepad");

            if (outlineController == null)
            {
                outlineController = GetComponent<OutlineController>();
            }

            if (keyboardButtons == null)
            {
                enabled = false;
                return;
            }

            _keyboardRenderer = keyboardButtons.GetComponent<SpriteRenderer>();
            _gamepadRenderer = gamepadButtons?.GetComponent<SpriteRenderer>();

            if (_keyboardRenderer == null)
            {
                enabled = false;
                return;
            }

            if (outlineController != null && enableOutlineOnInteraction)
            {
                outlineController.UpdateOutlineColor(interactionOutlineColor);
                outlineController.ShowOutline(false);
            }

            HideAllButtonsImmediate();
            _isPlayerInRange = false;
            _currentInputType = DetectCurrentInputType();
        }

        protected InputType DetectCurrentInputType()
        {
            if (Gamepad.current != null)
            {
                return InputType.Gamepad;
            }

            return InputType.Keyboard;
        }

        private void HideAllButtonsImmediate()
        {
            if (_keyboardRenderer != null) _keyboardRenderer.enabled = false;
            if (_gamepadRenderer != null) _gamepadRenderer.enabled = false;

            SetButtonAlpha(keyboardButtons, 0f);
            SetButtonAlpha(gamepadButtons, 0f);

            _isVisible = false;
            _currentActiveButton = null;
        }

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

        protected void HideAllButtons()
        {
            if (!_isVisible) return;

            if (_currentFadeCoroutine != null)
            {
                StopCoroutine(_currentFadeCoroutine);
            }

            if (_currentActiveButton != null)
            {
                _currentFadeCoroutine = StartCoroutine(FadeOut(_currentActiveButton));
            }

            _isVisible = false;
        }

        protected void ShowInteractionButtons()
        {
            Transform targetButton = GetButtonTransformForInputType(_currentInputType);

            if (_isVisible && _currentActiveButton == targetButton && targetButton != null)
                return;

            if (_currentFadeCoroutine != null)
            {
                StopCoroutine(_currentFadeCoroutine);
            }

            if (_currentActiveButton != null && _currentActiveButton != targetButton)
            {
                _currentFadeCoroutine = StartCoroutine(FadeOutThenIn(_currentActiveButton, targetButton));
            }
            else if (targetButton != null)
            {
                _currentActiveButton = targetButton;
                _currentFadeCoroutine = StartCoroutine(FadeIn(targetButton));
            }
            else
            {
                if (keyboardButtons != null)
                {
                    _currentActiveButton = keyboardButtons;
                    _currentFadeCoroutine = StartCoroutine(FadeIn(keyboardButtons));
                }
            }

            _isPlayerInRange = true;
            _isVisible = true;

            if (outlineController != null && enableOutlineOnInteraction)
            {
                outlineController.ShowOutline(true);
            }
        }

        private void HideInteractionButtons()
        {
            HideAllButtons();
            _isPlayerInRange = false;

            if (outlineController != null && enableOutlineOnInteraction)
            {
                outlineController.ShowOutline(false);
            }
        }

        private Transform GetButtonTransformForInputType(InputType inputType)
        {
            return inputType switch
            {
                InputType.Keyboard => keyboardButtons,
                InputType.Gamepad => gamepadButtons,
                _ => keyboardButtons
            };
        }
        #endregion

        #region Fade Animation
        private IEnumerator FadeIn(Transform buttonTransform)
        {
            if (buttonTransform == null) yield break;

            SpriteRenderer spriteRenderer = buttonTransform.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) yield break;

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

            Color finalColor = spriteRenderer.color;
            finalColor.a = 1f;
            spriteRenderer.color = finalColor;

            _currentFadeCoroutine = null;
        }

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
                float curveValue = fadeCurve.Evaluate(1f - t);

                Color currentColor = spriteRenderer.color;
                currentColor.a = startAlpha * curveValue;
                spriteRenderer.color = currentColor;

                yield return null;
            }

            spriteRenderer.enabled = false;
            _currentActiveButton = null;
            _currentFadeCoroutine = null;
        }

        private IEnumerator FadeOutThenIn(Transform outButton, Transform inButton)
        {
            yield return StartCoroutine(FadeOut(outButton));

            if (inButton != null)
            {
                _currentActiveButton = inButton;
                yield return StartCoroutine(FadeIn(inButton));
            }
        }
        #endregion

        #region Editor Only
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

        [ContextMenu("Test Enable Outline")]
        private void TestEnableOutline()
        {
            if (Application.isPlaying && outlineController != null)
            {
                outlineController.ShowOutline(true);
            }
        }

        [ContextMenu("Test Disable Outline")]
        private void TestDisableOutline()
        {
            if (Application.isPlaying && outlineController != null)
            {
                outlineController.ShowOutline(false);
            }
        }
#endif
        #endregion
    }
}
