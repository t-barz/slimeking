using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ExtraTools
{
    /// <summary>
    /// TitleScreenController - Controla a sequência de animações da tela inicial.
    /// Sequência: música (1s delay) → centerLogo fade in/out + background fade in → gameTitle fade in → wsLogo fade in
    /// </summary>
    public class TitleScreenController : MonoBehaviour
    {
        #region UI References
        [Header("UI Elements")]
        [SerializeField] private Image centerLogo;
        [SerializeField] private Image wsLogo;
        [SerializeField] private Image background;
        [SerializeField] private Image gameTitle;
        #endregion

        #region Animation Settings
        [Header("Animation Timings")]
        [SerializeField] private float musicDelay = 1f;
        [SerializeField] private float centerLogoFadeInDuration = 1.5f;
        [SerializeField] private float centerLogoVisibleDuration = 2f;
        [SerializeField] private float centerLogoFadeOutDuration = 1.5f;
        [SerializeField] private float backgroundFadeInDuration = 2f;
        [SerializeField] private float gameTitleFadeInDuration = 1.5f;
        [SerializeField] private float wsLogoFadeInDuration = 1f;

        [Header("Animation Curves")]
        [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        #endregion

        #region Control
        [Header("Control")]
        [SerializeField] private bool autoStart = true;
        [SerializeField] private bool skipOnInput = true;
        [SerializeField] private KeyCode skipKey = KeyCode.Space;
        #endregion

        private bool sequenceRunning = false;
        private bool sequenceCompleted = false;

        /// <summary>
        /// Evento disparado quando toda a sequência termina
        /// </summary>
        public System.Action OnSequenceCompleted;

        private void Start()
        {
            InitializeElements();

            if (autoStart)
            {
                StartTitleSequence();
            }
        }

        private void Update()
        {
            if (skipOnInput && sequenceRunning && !sequenceCompleted)
            {
                if (Input.GetKeyDown(skipKey) || Input.anyKeyDown)
                {
                    SkipToEnd();
                }
            }
        }

        /// <summary>
        /// Inicializa todos os elementos como invisíveis
        /// </summary>
        private void InitializeElements()
        {
            SetImageAlpha(centerLogo, 0f);
            SetImageAlpha(wsLogo, 0f);
            SetImageAlpha(background, 0f);
            SetImageAlpha(gameTitle, 0f);

            Debug.Log("[TitleScreen] Elementos inicializados como invisíveis");
        }

        /// <summary>
        /// Inicia a sequência completa da tela de título
        /// </summary>
        public void StartTitleSequence()
        {
            if (sequenceRunning) return;

            StartCoroutine(TitleSequenceCoroutine());
        }

        /// <summary>
        /// Pula para o final da sequência (todos os elementos visíveis)
        /// </summary>
        public void SkipToEnd()
        {
            if (sequenceCompleted) return;

            StopAllCoroutines();

            SetImageAlpha(centerLogo, 0f);    // Logo central some
            SetImageAlpha(background, 1f);     // Background visível
            SetImageAlpha(gameTitle, 1f);      // Título visível
            SetImageAlpha(wsLogo, 1f);         // WS Logo visível

            sequenceRunning = false;
            sequenceCompleted = true;

            Debug.Log("[TitleScreen] Sequência pulada - todos elementos finais visíveis");
            OnSequenceCompleted?.Invoke();
        }

        /// <summary>
        /// Sequência principal: música → centerLogo → background + fadeOut centerLogo → gameTitle → wsLogo
        /// </summary>
        private IEnumerator TitleSequenceCoroutine()
        {
            sequenceRunning = true;
            sequenceCompleted = false;

            Debug.Log("[TitleScreen] Iniciando sequência da tela de título");

            // Fase 1: Aguarda delay e inicia música
            yield return new WaitForSecondsRealtime(musicDelay);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMenuMusic(crossfade: true);
                Debug.Log("[TitleScreen] Música iniciada");
            }

            // Fase 2: Center Logo fade in
            yield return StartCoroutine(FadeImageIn(centerLogo, centerLogoFadeInDuration));

            // Fase 3: Center Logo fica visível
            yield return new WaitForSecondsRealtime(centerLogoVisibleDuration);

            // Fase 4: Background fade in + Center Logo fade out (simultâneo)
            StartCoroutine(FadeImageOut(centerLogo, centerLogoFadeOutDuration));
            yield return StartCoroutine(FadeImageIn(background, backgroundFadeInDuration));

            // Fase 5: Game Title fade in (quando background estiver totalmente visível)
            yield return StartCoroutine(FadeImageIn(gameTitle, gameTitleFadeInDuration));

            // Fase 6: WS Logo fade in (quando gameTitle estiver totalmente visível)
            yield return StartCoroutine(FadeImageIn(wsLogo, wsLogoFadeInDuration));

            // Sequência concluída
            sequenceRunning = false;
            sequenceCompleted = true;

            Debug.Log("[TitleScreen] Sequência de animação concluída");
            OnSequenceCompleted?.Invoke();
        }

        /// <summary>
        /// Fade in de uma imagem
        /// </summary>
        private IEnumerator FadeImageIn(Image image, float duration)
        {
            if (image == null) yield break;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / duration;
                float alpha = fadeInCurve.Evaluate(progress);

                SetImageAlpha(image, alpha);
                yield return null;
            }

            SetImageAlpha(image, 1f);
            Debug.Log($"[TitleScreen] {image.name} fade in concluído");
        }

        /// <summary>
        /// Fade out de uma imagem
        /// </summary>
        private IEnumerator FadeImageOut(Image image, float duration)
        {
            if (image == null) yield break;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / duration;
                float alpha = fadeOutCurve.Evaluate(progress);

                SetImageAlpha(image, alpha);
                yield return null;
            }

            SetImageAlpha(image, 0f);
            Debug.Log($"[TitleScreen] {image.name} fade out concluído");
        }

        /// <summary>
        /// Define alpha de uma imagem
        /// </summary>
        private void SetImageAlpha(Image image, float alpha)
        {
            if (image == null) return;

            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        #region Debug Helpers
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool debugMode = false;

        [UnityEngine.ContextMenu("Test Music")]
        private void TestMusic()
        {
            if (Application.isPlaying)
            {
                Debug.Log("[TitleScreen] Testando música...");
                if (AudioManager.Instance != null)
                {
                    Debug.Log("[TitleScreen] AudioManager encontrado, tentando tocar música");
                    AudioManager.Instance.PlayMenuMusic(crossfade: false);
                }
                else
                {
                    Debug.LogError("[TitleScreen] AudioManager.Instance é null!");
                }
            }
        }

        [UnityEngine.ContextMenu("Test Title Sequence")]
        private void TestTitleSequence()
        {
            if (Application.isPlaying)
            {
                InitializeElements();
                StartTitleSequence();
            }
        }

        [UnityEngine.ContextMenu("Skip to End")]
        private void TestSkipToEnd()
        {
            if (Application.isPlaying)
                SkipToEnd();
        }

        [UnityEngine.ContextMenu("Reset Elements")]
        private void TestResetElements()
        {
            if (Application.isPlaying)
                InitializeElements();
        }
#endif
        #endregion
    }
}