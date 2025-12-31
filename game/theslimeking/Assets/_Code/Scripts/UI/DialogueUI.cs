using UnityEngine;
using TMPro;
using System.Collections;
using SlimeKing.Systems;

namespace SlimeKing.Systems.UI
{
    /// <summary>
    /// Gerencia a exibição visual da caixa de diálogo com efeito typewriter.
    /// Implementa IDialogueUI para comunicação com DialogueManager.
    /// </summary>
    public class DialogueUI : MonoBehaviour, IDialogueUI
    {
        #region Inspector Fields

        [Header("UI References")]
        [Tooltip("Componente TextMeshProUGUI para exibir o texto do diálogo")]
        [SerializeField] private TextMeshProUGUI dialogueText;

        [Tooltip("GameObject da caixa de diálogo (será mostrado/ocultado)")]
        [SerializeField] private GameObject dialogueBox;

        [Tooltip("Indicador visual de continuação (seta ou ícone)")]
        [SerializeField] private GameObject continueIndicator;

        [Header("Typewriter Settings")]
        [Tooltip("Velocidade do efeito typewriter em caracteres por segundo")]
        [SerializeField] private float typewriterSpeed = 50f;

        [Tooltip("Se true, pressionar botão durante typewriter completa o texto")]
        [SerializeField] private bool skipOnInput = true;

        [Header("Animation")]
        [Tooltip("Duração da animação de fade in em segundos")]
        [SerializeField] private float fadeInDuration = 0.3f;

        [Tooltip("Duração da animação de fade out em segundos")]
        [SerializeField] private float fadeOutDuration = 0.3f;

        [Header("Input")]
        [Tooltip("Nome do botão de interação no Input Manager")]
        [SerializeField] private string interactionButton = "Interact";

        #endregion

        #region Private Fields

        /// <summary>
        /// CanvasGroup para controlar fade in/out
        /// </summary>
        private CanvasGroup canvasGroup;

        /// <summary>
        /// Coroutine do efeito typewriter
        /// </summary>
        private Coroutine typewriterCoroutine;

        /// <summary>
        /// Texto completo da página atual
        /// </summary>
        private string currentFullText;

        /// <summary>
        /// Flag indicando se o texto está completo
        /// </summary>
        private bool isTextComplete;

        /// <summary>
        /// Flag indicando se há mais páginas após a atual
        /// </summary>
        private bool hasMorePages;

        /// <summary>
        /// Flag indicando se o typewriter está ativo
        /// </summary>
        private bool isTypewriterActive;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Validar componentes necessários
            ValidateComponents();

            // Obter ou adicionar CanvasGroup
            canvasGroup = dialogueBox.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = dialogueBox.AddComponent<CanvasGroup>();
                UnityEngine.Debug.LogWarning("[DialogueUI] CanvasGroup not found on dialogueBox. Adding component.");
            }

            // Inicializar invisível
            canvasGroup.alpha = 0f;
            dialogueBox.SetActive(false);

            if (continueIndicator != null)
            {
                continueIndicator.SetActive(false);
            }

            // Registrar com DialogueManager
            if (DialogueManager.HasInstance)
            {
                DialogueManager.Instance.SetDialogueUI(this);
            }
        }

        private void Update()
        {
            // Verificar input para avançar diálogo ou completar texto
            if (dialogueBox.activeSelf && Input.GetButtonDown(interactionButton))
            {
                HandleInteractionInput();
            }
        }

        #endregion

        #region IDialogueUI Implementation

        /// <summary>
        /// Exibe o diálogo com o texto especificado e inicia o efeito typewriter.
        /// </summary>
        /// <param name="text">Texto a ser exibido</param>
        /// <param name="hasMorePages">Se há mais páginas após esta</param>
        public void ShowDialogue(string text, bool hasMorePages)
        {
            if (string.IsNullOrEmpty(text))
            {
                UnityEngine.Debug.LogWarning("[DialogueUI] ShowDialogue called with null or empty text");
                return;
            }

            this.currentFullText = text;
            this.hasMorePages = hasMorePages;
            this.isTextComplete = false;

            // Ativar caixa de diálogo
            dialogueBox.SetActive(true);

            // Ocultar indicador de continuação inicialmente
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(false);
            }

            // Iniciar fade in
            StartCoroutine(FadeIn());

            // Iniciar efeito typewriter
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
            }
            typewriterCoroutine = StartCoroutine(TypewriterEffect());
        }

        /// <summary>
        /// Oculta a caixa de diálogo com animação de fade out.
        /// </summary>
        public void HideDialogue()
        {
            // Parar typewriter se estiver ativo
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            // Iniciar fade out
            StartCoroutine(FadeOut());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Completa instantaneamente o texto da página atual, pulando a animação typewriter.
        /// </summary>
        public void CompleteCurrentText()
        {
            if (isTextComplete)
                return;

            // Parar coroutine do typewriter
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                typewriterCoroutine = null;
            }

            // Exibir texto completo
            dialogueText.text = currentFullText;
            isTextComplete = true;
            isTypewriterActive = false;

            // Mostrar indicador de continuação se houver mais páginas
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(hasMorePages);
            }
        }

        /// <summary>
        /// Verifica se o texto atual está completo.
        /// </summary>
        /// <returns>True se o texto está completo</returns>
        public bool IsTextComplete()
        {
            return isTextComplete;
        }

        /// <summary>
        /// Verifica se o efeito typewriter está ativo.
        /// </summary>
        /// <returns>True se o typewriter está ativo</returns>
        public bool IsTypewriterActive()
        {
            return isTypewriterActive;
        }

        /// <summary>
        /// Define a velocidade do typewriter para o próximo diálogo.
        /// Útil para NPCs com velocidades customizadas.
        /// </summary>
        /// <param name="speed">Velocidade em caracteres por segundo (0 para instantâneo)</param>
        public void SetTypewriterSpeed(float speed)
        {
            typewriterSpeed = Mathf.Max(0f, speed);
        }

        /// <summary>
        /// Obtém a velocidade atual do typewriter.
        /// </summary>
        /// <returns>Velocidade em caracteres por segundo</returns>
        public float GetTypewriterSpeed()
        {
            return typewriterSpeed;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Valida se todos os componentes necessários estão atribuídos.
        /// </summary>
        private void ValidateComponents()
        {
            if (dialogueText == null)
            {
                UnityEngine.Debug.LogError("[DialogueUI] dialogueText is not assigned!");
            }

            if (dialogueBox == null)
            {
                UnityEngine.Debug.LogError("[DialogueUI] dialogueBox is not assigned!");
            }

            if (continueIndicator == null)
            {
                UnityEngine.Debug.LogWarning("[DialogueUI] continueIndicator is not assigned. Continuation indicator will not be shown.");
            }
        }

        /// <summary>
        /// Trata o input de interação do jogador.
        /// </summary>
        private void HandleInteractionInput()
        {
            if (isTypewriterActive && skipOnInput)
            {
                // Completar texto se typewriter está ativo
                CompleteCurrentText();
            }
            else if (isTextComplete)
            {
                // Avançar para próxima página se texto está completo
                if (DialogueManager.HasInstance)
                {
                    DialogueManager.Instance.NextPage();
                }
            }
        }

        #endregion

        #region Coroutines

        /// <summary>
        /// Coroutine que implementa o efeito typewriter, revelando texto caractere por caractere.
        /// </summary>
        private IEnumerator TypewriterEffect()
        {
            isTypewriterActive = true;
            isTextComplete = false;

            // Limpar texto inicial
            dialogueText.text = "";

            // Calcular delay entre caracteres baseado na velocidade
            float delay = typewriterSpeed > 0 ? 1f / typewriterSpeed : 0f;

            // Se velocidade é 0, exibir texto instantaneamente
            if (delay <= 0)
            {
                dialogueText.text = currentFullText;
                isTextComplete = true;
                isTypewriterActive = false;

                // Mostrar indicador de continuação
                if (continueIndicator != null)
                {
                    continueIndicator.SetActive(hasMorePages);
                }

                yield break;
            }

            // Revelar texto caractere por caractere
            for (int i = 0; i < currentFullText.Length; i++)
            {
                dialogueText.text = currentFullText.Substring(0, i + 1);
                yield return new WaitForSeconds(delay);
            }

            // Texto completo
            isTextComplete = true;
            isTypewriterActive = false;

            // Mostrar indicador de continuação se houver mais páginas
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(hasMorePages);
            }

            typewriterCoroutine = null;
        }

        /// <summary>
        /// Coroutine para animação de fade in da caixa de diálogo.
        /// </summary>
        private IEnumerator FadeIn()
        {
            float elapsed = 0f;

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        /// <summary>
        /// Coroutine para animação de fade out da caixa de diálogo.
        /// </summary>
        private IEnumerator FadeOut()
        {
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;

            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            dialogueBox.SetActive(false);

            // Ocultar indicador de continuação
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(false);
            }
        }

        #endregion

        #region Editor Validation

        private void OnValidate()
        {
            // Garantir valores válidos
            if (typewriterSpeed < 0)
            {
                typewriterSpeed = 0;
            }

            if (fadeInDuration < 0)
            {
                fadeInDuration = 0;
            }

            if (fadeOutDuration < 0)
            {
                fadeOutDuration = 0;
            }
        }

        #endregion
    }
}
