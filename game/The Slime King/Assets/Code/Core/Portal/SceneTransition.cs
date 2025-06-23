using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TheSlimeKing.Core.Portal
{
    /// <summary>
    /// Gerencia os efeitos visuais de transição entre cenas
    /// Utiliza um canvas com um painel que faz fade in/out para criar uma transição suave
    /// </summary>
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _fullscreenPanel;
        [SerializeField] private Color _fadeColor = Color.black;
        [SerializeField] private AnimationCurve _fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        private Coroutine _currentAnimation = null;

        private void Awake()
        {
            // Se não tiver sido associado no inspector, tenta encontrar automaticamente
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
                _canvasGroup = GetComponentInChildren<CanvasGroup>();

            if (_fullscreenPanel == null)
                _fullscreenPanel = GetComponentInChildren<Image>();

            // Configura a cor do painel
            if (_fullscreenPanel != null)
                _fullscreenPanel.color = _fadeColor;

            // Configura para não iniciar visível
            if (_canvasGroup != null)
                _canvasGroup.alpha = 0;

            // Certifica-se que o canvas está configurado para ficar sobre tudo
            Canvas canvas = GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 9999;
            }
        }

        /// <summary>
        /// Faz a transição aparecer gradualmente
        /// </summary>
        /// <param name="duration">Duração da animação em segundos</param>
        public void FadeIn(float duration = 1.0f)
        {
            // Interrompe qualquer animação em andamento
            if (_currentAnimation != null)
                StopCoroutine(_currentAnimation);

            _currentAnimation = StartCoroutine(FadeAnimation(0, 1, duration, _fadeInCurve));
        }

        /// <summary>
        /// Faz a transição desaparecer gradualmente
        /// </summary>
        /// <param name="duration">Duração da animação em segundos</param>
        public void FadeOut(float duration = 1.0f)
        {
            // Interrompe qualquer animação em andamento
            if (_currentAnimation != null)
                StopCoroutine(_currentAnimation);

            _currentAnimation = StartCoroutine(FadeAnimation(1, 0, duration, _fadeOutCurve));
        }

        /// <summary>
        /// Anima o fade do canvas entre dois valores de alpha
        /// </summary>
        private IEnumerator FadeAnimation(float startAlpha, float endAlpha, float duration, AnimationCurve curve)
        {
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                float normalizedTime = elapsedTime / duration;
                float curveValue = curve.Evaluate(normalizedTime);

                _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, curveValue);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Certifica-se que o valor final é exatamente o alvo
            _canvasGroup.alpha = endAlpha;
            _currentAnimation = null;
        }
    }
}
