using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

namespace SlimeKing.Core
{
    /// <summary>
    /// Controla a transição da vinheta de fade in/out.
    /// Permite configurar valores iniciais e finais, além de controlar a transição via código.
    /// </summary>
    public class VignetteController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Volume volumeComponent;

        [Header("Initial Values")]
        [SerializeField] private Color startColor = Color.black;
        [SerializeField] private float startIntensity = 1f;
        [SerializeField] private float startSmoothness = 1f;
        [SerializeField] private bool startRounded = true;

        [Header("Target Values")]
        [SerializeField] private Color targetColor = new Color(0.18f, 0.13f, 0.08f, 1f);
        [SerializeField] private float targetIntensity = 0.3f;
        [SerializeField] private float targetSmoothness = 0.5f;
        [SerializeField] private bool targetRounded = false;

        [Header("Transition Settings")]
        [SerializeField] private float delayBeforeTransition = 5f;
        [SerializeField] private float transitionDuration = 1f;
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private VolumeProfile profile;
        private Vignette vignette;
        private bool isTransitioning;

        private void Awake()
        {
            InitializeVignette();
        }

        private void InitializeVignette()
        {
            if (volumeComponent == null)
            {
                volumeComponent = GetComponent<Volume>();
            }

            if (volumeComponent == null)
            {
                Debug.LogError("Volume component não encontrado no VignetteController!", this);
                return;
            }

            profile = volumeComponent.profile;
            if (profile == null)
            {
                Debug.LogError("Volume Profile não encontrado!", this);
                return;
            }

            if (!profile.TryGet<Vignette>(out vignette))
            {
                vignette = profile.Add<Vignette>(false);
                if (vignette == null)
                {
                    Debug.LogError("Falha ao adicionar a sobreposição de Vinheta!", this);
                    return;
                }
            }

            SetInitialValues();
        }

        private void Start()
        {
            StartInitialTransition();
        }

        private void SetInitialValues()
        {
            if (vignette == null) return;

            vignette.color.Override(startColor);
            vignette.intensity.Override(startIntensity);
            vignette.smoothness.Override(startSmoothness);
            vignette.rounded.Override(startRounded);
        }

        private void StartInitialTransition()
        {
            if (vignette == null) return;
            StartCoroutine(TransitionAfterDelay());
        }

        /// <summary>
        /// Inicia uma transição da vinheta para os valores alvo.
        /// </summary>
        /// <param name="immediate">Se true, a transição acontece instantaneamente</param>
        public void TransitionToTarget(bool immediate = false)
        {
            if (vignette == null) return;

            if (immediate)
            {
                vignette.color.Override(targetColor);
                vignette.intensity.Override(targetIntensity);
                vignette.smoothness.Override(targetSmoothness);
                vignette.rounded.Override(targetRounded);
            }
            else
            {
                StartCoroutine(TransitionRoutine());
            }
        }

        private IEnumerator TransitionAfterDelay()
        {
            yield return new WaitForSeconds(delayBeforeTransition);
            TransitionToTarget();
        }

        private IEnumerator TransitionRoutine()
        {
            if (isTransitioning) yield break;
            isTransitioning = true;

            float elapsedTime = 0f;
            Color startingColor = vignette.color.value;
            float startingIntensity = vignette.intensity.value;
            float startingSmoothness = vignette.smoothness.value;
            bool startingRounded = vignette.rounded.value;

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = transitionCurve.Evaluate(elapsedTime / transitionDuration);

                vignette.color.Override(Color.Lerp(startingColor, targetColor, t));
                vignette.intensity.Override(Mathf.Lerp(startingIntensity, targetIntensity, t));
                vignette.smoothness.Override(Mathf.Lerp(startingSmoothness, targetSmoothness, t));

                // Apenas mude o valor arredondado no final da transição para evitar artefatos visuais
                if (t > 0.9f && startingRounded != targetRounded)
                {
                    vignette.rounded.Override(targetRounded);
                }

                yield return null;
            }

            // Garantir que chegamos aos valores exatos do alvo
            vignette.color.Override(targetColor);
            vignette.intensity.Override(targetIntensity);
            vignette.smoothness.Override(targetSmoothness);
            vignette.rounded.Override(targetRounded);

            isTransitioning = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying && profile != null)
            {
                SetInitialValues();
            }
        }
#endif
    }
}
