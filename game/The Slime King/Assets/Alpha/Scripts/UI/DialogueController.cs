using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Alpha.UI
{
    /// <summary>
    /// Sistema mínimo de diálogo para Demo Alpha
    /// Controla caixas de diálogo, avanço de texto e bloqueio de input
    /// </summary>
    public class DialogueController : MonoBehaviour
    {
        #region Singleton
        public static DialogueController Instance { get; private set; }
        #endregion

        #region Events
        public static event Action OnDialogueStarted;
        public static event Action OnDialogueEnded;
        public static event Action<string> OnDialogueTextChanged;
        public static event Action OnDialogueAdvanced;
        #endregion

        #region Serialized Fields
        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private Image speakerPortrait;
        [SerializeField] private Button continueButton;
        [SerializeField] private GameObject continueIndicator;

        [Header("Animation Settings")]
        [SerializeField] private bool useTypewriterEffect = true;
        [SerializeField] private float typewriterSpeed = 0.05f;
        [SerializeField] private bool allowSkipTyping = true;
        [SerializeField] private AudioClip typingSound;

        [Header("Input Settings")]
        [SerializeField] private KeyCode advanceKey = KeyCode.Space;
        [SerializeField] private KeyCode skipKey = KeyCode.Escape;
        [SerializeField] private bool blockGameplayInput = true;

        [Header("Visual Settings")]
        [SerializeField] private float panelFadeSpeed = 5f;
        [SerializeField] private CanvasGroup dialogueCanvasGroup;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        #endregion

        #region Private Fields
        private Queue<DialogueEntry> dialogueQueue = new Queue<DialogueEntry>();
        private DialogueEntry currentDialogue;
        private bool isDialogueActive = false;
        private bool isTyping = false;
        private bool isWaitingForInput = false;
        private Coroutine typewriterCoroutine;
        private Coroutine fadeCoroutine;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeSingleton();
        }

        private void Start()
        {
            InitializeDialogueUI();
            SetupEventListeners();
        }

        private void Update()
        {
            HandleInput();
        }

        private void OnDestroy()
        {
            CleanupEventListeners();
        }
        #endregion

        #region Initialization
        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                // Não usar DontDestroyOnLoad para diálogo (cada cena pode ter seu próprio)
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeDialogueUI()
        {
            // Esconde o painel inicialmente
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            if (dialogueCanvasGroup != null)
            {
                dialogueCanvasGroup.alpha = 0f;
            }

            // Configura botão de continuar
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(AdvanceDialogue);
            }

            // Esconde indicador de continuar
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(false);
            }

            if (enableDebugLogs)
                Debug.Log("[DialogueController] Dialogue UI initialized");
        }

        private void SetupEventListeners()
        {
            // TODO: Conectar a eventos de gameplay se necessário
        }

        private void CleanupEventListeners()
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(AdvanceDialogue);
            }

            OnDialogueStarted = null;
            OnDialogueEnded = null;
            OnDialogueTextChanged = null;
            OnDialogueAdvanced = null;
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Inicia um diálogo com uma única mensagem
        /// </summary>
        public void StartDialogue(string text, string speakerName = "", Sprite portrait = null)
        {
            var dialogue = new DialogueEntry
            {
                text = text,
                speakerName = speakerName,
                portrait = portrait
            };

            StartDialogue(new List<DialogueEntry> { dialogue });
        }

        /// <summary>
        /// Inicia um diálogo com múltiplas mensagens
        /// </summary>
        public void StartDialogue(List<DialogueEntry> dialogues)
        {
            if (isDialogueActive)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[DialogueController] Dialogue already active!");
                return;
            }

            // Adiciona diálogos à fila
            dialogueQueue.Clear();
            foreach (var dialogue in dialogues)
            {
                dialogueQueue.Enqueue(dialogue);
            }

            // Inicia o diálogo
            isDialogueActive = true;
            ShowDialoguePanel();
            ProcessNextDialogue();

            // Bloqueia input de gameplay
            if (blockGameplayInput)
            {
                SetGameplayInputBlocked(true);
            }

            OnDialogueStarted?.Invoke();

            if (enableDebugLogs)
                Debug.Log($"[DialogueController] Started dialogue with {dialogues.Count} entries");
        }

        /// <summary>
        /// Avança para o próximo diálogo ou finaliza
        /// </summary>
        public void AdvanceDialogue()
        {
            if (!isDialogueActive) return;

            if (isTyping && allowSkipTyping)
            {
                // Pula a animação de digitação
                SkipTypewriter();
                return;
            }

            if (!isWaitingForInput) return;

            if (dialogueQueue.Count > 0)
            {
                // Próximo diálogo
                ProcessNextDialogue();
                OnDialogueAdvanced?.Invoke();
            }
            else
            {
                // Finaliza diálogo
                EndDialogue();
            }
        }

        /// <summary>
        /// Finaliza o diálogo imediatamente
        /// </summary>
        public void EndDialogue()
        {
            if (!isDialogueActive) return;

            StopAllCoroutines();
            dialogueQueue.Clear();
            isDialogueActive = false;
            isTyping = false;
            isWaitingForInput = false;

            HideDialoguePanel();

            // Restaura input de gameplay
            if (blockGameplayInput)
            {
                SetGameplayInputBlocked(false);
            }

            OnDialogueEnded?.Invoke();

            if (enableDebugLogs)
                Debug.Log("[DialogueController] Dialogue ended");
        }

        /// <summary>
        /// Verifica se está em diálogo
        /// </summary>
        public bool IsDialogueActive() => isDialogueActive;

        /// <summary>
        /// Verifica se está esperando input
        /// </summary>
        public bool IsWaitingForInput() => isWaitingForInput;
        #endregion

        #region Dialogue Processing
        private void ProcessNextDialogue()
        {
            if (dialogueQueue.Count == 0) return;

            currentDialogue = dialogueQueue.Dequeue();
            DisplayDialogue(currentDialogue);
        }

        private void DisplayDialogue(DialogueEntry dialogue)
        {
            // Atualiza nome do falante
            if (speakerNameText != null)
            {
                speakerNameText.text = dialogue.speakerName;
                speakerNameText.gameObject.SetActive(!string.IsNullOrEmpty(dialogue.speakerName));
            }

            // Atualiza retrato
            if (speakerPortrait != null)
            {
                speakerPortrait.sprite = dialogue.portrait;
                speakerPortrait.gameObject.SetActive(dialogue.portrait != null);
            }

            // Esconde indicador de continuar
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(false);
            }

            isWaitingForInput = false;

            // Inicia animação de texto
            if (useTypewriterEffect && dialogueText != null)
            {
                StartTypewriter(dialogue.text);
            }
            else
            {
                SetDialogueText(dialogue.text);
                OnTypewriterFinished();
            }
        }

        private void SetDialogueText(string text)
        {
            if (dialogueText != null)
            {
                dialogueText.text = text;
            }

            OnDialogueTextChanged?.Invoke(text);
        }
        #endregion

        #region Typewriter Effect
        private void StartTypewriter(string text)
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
            }

            typewriterCoroutine = StartCoroutine(TypewriterCoroutine(text));
        }

        private IEnumerator TypewriterCoroutine(string text)
        {
            isTyping = true;
            SetDialogueText("");

            for (int i = 0; i <= text.Length; i++)
            {
                string currentText = text.Substring(0, i);
                SetDialogueText(currentText);

                // Toca som de digitação
                if (typingSound != null && i < text.Length)
                {
                    // TODO: Tocar som via AudioManager
                }

                yield return new WaitForSeconds(typewriterSpeed);
            }

            OnTypewriterFinished();
        }

        private void SkipTypewriter()
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                SetDialogueText(currentDialogue.text);
                OnTypewriterFinished();
            }
        }

        private void OnTypewriterFinished()
        {
            isTyping = false;
            isWaitingForInput = true;

            // Mostra indicador de continuar
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(true);
            }
        }
        #endregion

        #region UI Management
        private void ShowDialoguePanel()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            if (dialogueCanvasGroup != null)
            {
                fadeCoroutine = StartCoroutine(FadeCanvasGroup(dialogueCanvasGroup, 1f, panelFadeSpeed));
            }
        }

        private void HideDialoguePanel()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            if (dialogueCanvasGroup != null)
            {
                fadeCoroutine = StartCoroutine(FadeCanvasGroup(dialogueCanvasGroup, 0f, panelFadeSpeed, () =>
                {
                    if (dialoguePanel != null)
                    {
                        dialoguePanel.SetActive(false);
                    }
                }));
            }
            else if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float speed, Action onComplete = null)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;
            float duration = Mathf.Abs(targetAlpha - startAlpha) / speed;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            onComplete?.Invoke();
        }
        #endregion

        #region Input Handling
        private void HandleInput()
        {
            if (!isDialogueActive) return;

            // Avançar diálogo
            if (Input.GetKeyDown(advanceKey))
            {
                AdvanceDialogue();
            }

            // Pular diálogo
            if (Input.GetKeyDown(skipKey))
            {
                EndDialogue();
            }
        }

        private void SetGameplayInputBlocked(bool blocked)
        {
            // TODO: Integrar com InputManager para bloquear input de gameplay
            // Exemplo: InputManager.Instance.SetInputBlocked(blocked);

            if (enableDebugLogs)
                Debug.Log($"[DialogueController] Gameplay input blocked: {blocked}");
        }
        #endregion

        #region Dialogue Creation Helpers
        /// <summary>
        /// Cria uma entrada de diálogo simples
        /// </summary>
        public static DialogueEntry CreateDialogue(string text, string speaker = "", Sprite portrait = null)
        {
            return new DialogueEntry
            {
                text = text,
                speakerName = speaker,
                portrait = portrait
            };
        }

        /// <summary>
        /// Cria múltiplas entradas de diálogo
        /// </summary>
        public static List<DialogueEntry> CreateDialogueChain(params string[] texts)
        {
            var dialogues = new List<DialogueEntry>();
            foreach (var text in texts)
            {
                dialogues.Add(CreateDialogue(text));
            }
            return dialogues;
        }
        #endregion

        #region Debug & Testing
        [ContextMenu("Debug - Test Simple Dialogue")]
        private void DebugTestSimpleDialogue()
        {
            StartDialogue("This is a test dialogue message!");
        }

        [ContextMenu("Debug - Test Multi Dialogue")]
        private void DebugTestMultiDialogue()
        {
            var dialogues = CreateDialogueChain(
                "Hello there, adventurer!",
                "Welcome to the world of slimes.",
                "Press space to continue, escape to skip."
            );
            StartDialogue(dialogues);
        }

        [ContextMenu("Debug - End Dialogue")]
        private void DebugEndDialogue()
        {
            EndDialogue();
        }
        #endregion
    }

    /// <summary>
    /// Estrutura de uma entrada de diálogo
    /// </summary>
    [System.Serializable]
    public struct DialogueEntry
    {
        public string text;
        public string speakerName;
        public Sprite portrait;
    }
}