using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if DOTWEEN_INSTALLED
using DG.Tweening;
#endif

namespace TheSlimeKing.Core.Dialogue
{
    /// <summary>
    /// Gerencia a interface visual do sistema de diálogos, incluindo animações e transições.
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        [Header("Componentes de Interface")]
        [SerializeField] private CanvasGroup _dialogueCanvasGroup;
        [SerializeField] private RectTransform _dialoguePanel;
        [SerializeField] private RectTransform _namePanel;
        [SerializeField] private RectTransform _continueIndicator;

        [Header("Configurações de Animação")]
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _fadeOutDuration = 0.2f;
        [SerializeField] private float _panelScaleDuration = 0.4f;
        [SerializeField] private float _indicatorBounceDuration = 0.5f;

        [Header("Referências")]
        [SerializeField] private DialogueManager _dialogueManager;

        private Vector3 _initialIndicatorScale;
        private Vector3 _targetIndicatorScale;
        private Coroutine _indicatorAnimationCoroutine = null;

        private void Awake()
        {
            // Inicialização de componentes e valores
            if (_dialogueManager == null)
            {
                _dialogueManager = DialogueManager.Instance;
            }

            _initialIndicatorScale = _continueIndicator.localScale;
            _targetIndicatorScale = _initialIndicatorScale * 1.2f;

            // Configura CanvasGroup para estar invisível inicialmente
            _dialogueCanvasGroup.alpha = 0;
            _dialoguePanel.localScale = Vector3.zero;
            _namePanel.localScale = Vector3.zero;
        }

        private void OnEnable()
        {
            // Registra nos eventos do DialogueManager
            _dialogueManager.OnDialogueStart += HandleDialogueStart;
            _dialogueManager.OnDialogueLine += HandleDialogueLine;
            _dialogueManager.OnDialogueEnd += HandleDialogueEnd;
        }

        private void OnDisable()
        {
            // Remove registro dos eventos
            if (_dialogueManager != null)
            {
                _dialogueManager.OnDialogueStart -= HandleDialogueStart;
                _dialogueManager.OnDialogueLine -= HandleDialogueLine;
                _dialogueManager.OnDialogueEnd -= HandleDialogueEnd;
            }
        }

        /// <summary>
        /// Manipula o evento de início de diálogo.
        /// </summary>
        private void HandleDialogueStart()
        {
#if DOTWEEN_INSTALLED
            _dialogueCanvasGroup.DOKill();
            _dialoguePanel.DOKill();
            _namePanel.DOKill();

            _dialogueCanvasGroup.DOFade(1f, _fadeInDuration);
            _dialoguePanel.DOScale(1f, _panelScaleDuration).SetEase(Ease.OutBack);
            _namePanel.DOScale(1f, _panelScaleDuration).SetEase(Ease.OutBack);
#else
            _dialogueCanvasGroup.alpha = 1f;
            _dialoguePanel.localScale = Vector3.one;
            _namePanel.localScale = Vector3.one;
#endif
        }

        /// <summary>
        /// Manipula o evento de exibição de linha de diálogo.
        /// </summary>
        private void HandleDialogueLine()
        {
            // Pode adicionar efeitos visuais adicionais quando uma nova linha é exibida
        }

        /// <summary>
        /// Manipula o evento de fim de diálogo.
        /// </summary>
        private void HandleDialogueEnd()
        {
#if DOTWEEN_INSTALLED
            _dialogueCanvasGroup.DOKill();
            _dialoguePanel.DOKill();
            _namePanel.DOKill();

            Sequence fadeOutSequence = DOTween.Sequence();
            fadeOutSequence.Append(_dialogueCanvasGroup.DOFade(0f, _fadeOutDuration));
            fadeOutSequence.Join(_dialoguePanel.DOScale(0f, _fadeOutDuration).SetEase(Ease.InBack));
            fadeOutSequence.Join(_namePanel.DOScale(0f, _fadeOutDuration).SetEase(Ease.InBack));
#else
            _dialogueCanvasGroup.alpha = 0f;
            _dialoguePanel.localScale = Vector3.zero;
            _namePanel.localScale = Vector3.zero;
#endif
        }

        private void Update()
        {
            // Anima o indicador de continuação quando estiver visível
            if (_continueIndicator.gameObject.activeSelf && _indicatorAnimationCoroutine == null)
            {
                _indicatorAnimationCoroutine = StartCoroutine(AnimateContinueIndicator());
            }
        }

        /// <summary>
        /// Coroutine que anima o indicador de continuação com um efeito de pulsação.
        /// </summary>
        private IEnumerator AnimateContinueIndicator()
        {
            while (_continueIndicator.gameObject.activeSelf)
            {
#if DOTWEEN_INSTALLED
                _continueIndicator.DOScale(_targetIndicatorScale, _indicatorBounceDuration / 2).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(_indicatorBounceDuration / 2);
                _continueIndicator.DOScale(_initialIndicatorScale, _indicatorBounceDuration / 2).SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(_indicatorBounceDuration / 2);
#else
                yield return null;
#endif
            }
            _indicatorAnimationCoroutine = null;
        }
    }
}

// Se ocorrer erro de DG.Tweening, instale o pacote DOTween (Demigiant) via Package Manager ou Asset Store.
