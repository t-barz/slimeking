using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeKing.Core.UI
{
    /// <summary>
    /// Componente individual de coração no HUD de vida.
    /// Responsável por gerenciar o estado visual (cheio/vazio) e animações de dano.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class HeartUIElement : MonoBehaviour
    {
        #region Inspector Configuration

        [Header("Configuração de Sprites")]
        [SerializeField] private Sprite heartFullSprite;
        [SerializeField] private Sprite heartEmptySprite;

        [Header("Configuração de Animação")]
        [SerializeField] private float bounceScale = 1.2f;
        [SerializeField] private float bounceDuration = 0.3f;
        [SerializeField] private AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Configurações de Debug")]
        [SerializeField] private bool enableLogs = false;

        #endregion

        #region Private Variables

        private Image _heartImage;
        private bool _isFull;
        private Vector3 _originalScale;
        private Coroutine _bounceCoroutine;

        #endregion

        #region Public Properties

        /// <summary>
        /// Estado atual do coração (cheio ou vazio).
        /// </summary>
        public bool IsFull
        {
            get => _isFull;
            set
            {
                bool wasChanged = _isFull != value;
                _isFull = value;
                UpdateVisual();

                if (wasChanged && enableLogs)
                {
                    Debug.Log($"[HeartUIElement] Estado alterado para: {(value ? "Cheio" : "Vazio")}");
                }
            }
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Obtém o componente Image
            _heartImage = GetComponent<Image>();
            _originalScale = transform.localScale;

            // Valida configuração
            if (heartFullSprite == null || heartEmptySprite == null)
            {
                Debug.LogError($"[HeartUIElement] Sprites não configurados no objeto {gameObject.name}!");
            }

            if (enableLogs)
            {
                Debug.Log($"[HeartUIElement] Inicializado: {gameObject.name}");
            }
        }

        private void Start()
        {
            // Inicializa como coração cheio por padrão
            IsFull = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reproduz animação de bounce quando o coração perde/ganha vida.
        /// </summary>
        public void PlayBounceAnimation()
        {
            if (_bounceCoroutine != null)
            {
                StopCoroutine(_bounceCoroutine);
            }

            _bounceCoroutine = StartCoroutine(BounceAnimationCoroutine());

            if (enableLogs)
            {
                Debug.Log($"[HeartUIElement] Animação de bounce iniciada em {gameObject.name}");
            }
        }

        /// <summary>
        /// Define os sprites do coração.
        /// </summary>
        /// <param name="fullSprite">Sprite para coração cheio</param>
        /// <param name="emptySprite">Sprite para coração vazio</param>
        public void SetSprites(Sprite fullSprite, Sprite emptySprite)
        {
            heartFullSprite = fullSprite;
            heartEmptySprite = emptySprite;
            UpdateVisual();

            if (enableLogs)
            {
                Debug.Log($"[HeartUIElement] Sprites atualizados em {gameObject.name}");
            }
        }

        /// <summary>
        /// Para todas as animações em execução.
        /// </summary>
        public void StopAnimations()
        {
            if (_bounceCoroutine != null)
            {
                StopCoroutine(_bounceCoroutine);
                _bounceCoroutine = null;
            }

            transform.localScale = _originalScale;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Atualiza o sprite visual baseado no estado atual.
        /// </summary>
        private void UpdateVisual()
        {
            if (_heartImage == null) return;

            _heartImage.sprite = _isFull ? heartFullSprite : heartEmptySprite;
        }

        /// <summary>
        /// Corrotina que executa a animação de bounce.
        /// </summary>
        private IEnumerator BounceAnimationCoroutine()
        {
            Vector3 targetScale = _originalScale * bounceScale;
            float elapsedTime = 0f;

            // Fase de expansão
            while (elapsedTime < bounceDuration / 2f)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / (bounceDuration / 2f);
                float curveValue = bounceCurve.Evaluate(progress);

                transform.localScale = Vector3.Lerp(_originalScale, targetScale, curveValue);
                yield return null;
            }

            elapsedTime = 0f;

            // Fase de contração
            while (elapsedTime < bounceDuration / 2f)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / (bounceDuration / 2f);
                float curveValue = bounceCurve.Evaluate(progress);

                transform.localScale = Vector3.Lerp(targetScale, _originalScale, curveValue);
                yield return null;
            }

            // Garante que retorne ao tamanho original
            transform.localScale = _originalScale;
            _bounceCoroutine = null;
        }

        #endregion
    }
}