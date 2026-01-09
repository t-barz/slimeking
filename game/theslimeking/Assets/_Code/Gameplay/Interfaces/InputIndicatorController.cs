using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Controla a exibição de indicadores de input específicos baseado no tipo de input do jogador.
    /// Detecta automaticamente entre teclado e controlador, exibindo apenas o indicador correspondente.
    /// </summary>
    public class InputIndicatorController : MonoBehaviour
    {
        #region Fields
        [Header("Input Indicator GameObjects")]
        [SerializeField] private GameObject keyboardIndicator;
        [SerializeField] private GameObject gamepadIndicator;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Collider2D _collider;
        private GameObject _currentActiveIndicator;
        private Coroutine _fadeCoroutine;
        private InputType _currentInputType = InputType.Keyboard;
        private bool _isPlayerInRange = false;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            _collider = GetComponent<Collider2D>();

            if (_collider == null)
            {
                return;
            }

            _collider.isTrigger = true;
            HideAllIndicators();
        }

        private void Update()
        {
            if (_isPlayerInRange)
            {
                InputType detectedInput = DetectCurrentInputType();
                if (detectedInput != _currentInputType)
                {
                    _currentInputType = detectedInput;
                    UpdateIndicator();
                }
                else if (_currentInputType == InputType.Gamepad && keyboardIndicator != null && keyboardIndicator.activeSelf)
                {
                    // Se gamepad está ativo, garante que keyboard fica inativo
                    keyboardIndicator.SetActive(false);
                }
                else if (_currentInputType == InputType.Keyboard && gamepadIndicator != null && gamepadIndicator.activeSelf)
                {
                    // Se keyboard está ativo, garante que gamepad fica inativo
                    gamepadIndicator.SetActive(false);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInRange = true;
                _currentInputType = DetectCurrentInputType();
                ShowIndicator();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInRange = false;
                HideIndicator();
            }
        }
        #endregion

        #region Private Methods
        private InputType DetectCurrentInputType()
        {
            // Verifica se há qualquer gamepad conectado
            if (Gamepad.current != null || Gamepad.all.Count > 0)
            {
                return InputType.Gamepad;
            }

            return InputType.Keyboard;
        }

        private GameObject GetIndicatorForInputType(InputType inputType)
        {
            return inputType switch
            {
                InputType.Keyboard => keyboardIndicator,
                InputType.Gamepad => gamepadIndicator,
                _ => keyboardIndicator
            };
        }

        private void ShowIndicator()
        {
            GameObject targetIndicator = GetIndicatorForInputType(_currentInputType);

            if (targetIndicator == null)
            {
                return;
            }

            if (_currentActiveIndicator == targetIndicator)
            {
                return;
            }

            // Garante que o indicador não-selecionado está desativado
            GameObject otherIndicator = _currentInputType == InputType.Keyboard ? gamepadIndicator : keyboardIndicator;
            if (otherIndicator != null && otherIndicator.activeSelf)
            {
                otherIndicator.SetActive(false);
            }

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            if (_currentActiveIndicator != null)
            {
                _fadeCoroutine = StartCoroutine(FadeOutThenIn(_currentActiveIndicator, targetIndicator));
            }
            else
            {
                _fadeCoroutine = StartCoroutine(FadeIn(targetIndicator));
            }
        }

        private void HideIndicator()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            if (_currentActiveIndicator != null)
            {
                _fadeCoroutine = StartCoroutine(FadeOut(_currentActiveIndicator));
            }
        }

        private void UpdateIndicator()
        {
            GameObject targetIndicator = GetIndicatorForInputType(_currentInputType);

            if (targetIndicator == null || _currentActiveIndicator == targetIndicator)
            {
                return;
            }

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            _fadeCoroutine = StartCoroutine(FadeOutThenIn(_currentActiveIndicator, targetIndicator));
        }

        private void HideAllIndicators()
        {
            if (keyboardIndicator != null)
            {
                keyboardIndicator.SetActive(false);
            }

            if (gamepadIndicator != null)
            {
                gamepadIndicator.SetActive(false);
            }

            _currentActiveIndicator = null;
        }

        private void SetIndicatorAlpha(GameObject indicator, float alpha)
        {
            if (indicator == null) return;
            // Alpha control would go here if needed
        }

        private CanvasGroup GetCanvasGroup(GameObject indicator)
        {
            if (indicator == null) return null;
            return indicator.GetComponent<CanvasGroup>();
        }
        #endregion

        #region Fade Animation
        private IEnumerator FadeIn(GameObject indicator)
        {
            if (indicator == null) yield break;

            _currentActiveIndicator = indicator;
            indicator.SetActive(true);

            CanvasGroup canvasGroup = GetCanvasGroup(indicator);
            if (canvasGroup == null)
            {
                // Se não houver CanvasGroup, apenas ativa o objeto
                _fadeCoroutine = null;
                yield break;
            }

            canvasGroup.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                float curveValue = fadeCurve.Evaluate(t);
                canvasGroup.alpha = curveValue;
                yield return null;
            }

            canvasGroup.alpha = 1f;
            _fadeCoroutine = null;
        }

        private IEnumerator FadeOut(GameObject indicator)
        {
            if (indicator == null) yield break;

            CanvasGroup canvasGroup = GetCanvasGroup(indicator);
            if (canvasGroup == null)
            {
                // Se não houver CanvasGroup, apenas desativa o objeto
                indicator.SetActive(false);
                _currentActiveIndicator = null;
                _fadeCoroutine = null;
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                float curveValue = fadeCurve.Evaluate(1f - t);
                canvasGroup.alpha = curveValue;
                yield return null;
            }

            canvasGroup.alpha = 0f;
            indicator.SetActive(false);
            _currentActiveIndicator = null;
            _fadeCoroutine = null;
        }

        private IEnumerator FadeOutThenIn(GameObject outIndicator, GameObject inIndicator)
        {
            yield return StartCoroutine(FadeOut(outIndicator));
            yield return StartCoroutine(FadeIn(inIndicator));
        }
        #endregion
    }
}

