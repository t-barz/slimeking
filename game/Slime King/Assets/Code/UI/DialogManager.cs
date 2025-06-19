using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using UnityEngine.InputSystem;

namespace SlimeKing.UI
{
    /// <summary>
    /// Gerencia a exibição de diálogos na interface do usuário.
    /// </summary>
    public class DialogManager : MonoBehaviour
    {
        public static DialogManager Instance { get; private set; }

        [Header("Referências UI")]
        [SerializeField] private GameObject dialogBox;
        [SerializeField] private TextMeshProUGUI dialogText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image speakerIcon;

        [Header("Configurações")]
        [SerializeField] private float typingSpeed = 0.05f;
        [Tooltip("Tempo para o fade in/out da caixa de diálogo")]
        [SerializeField] private float fadeTime = 0.1f;

        private bool isDialogActive = false;
        private bool isTyping = false;
        private string currentFullText = "";
        private Action onDialogComplete;
        private CanvasGroup canvasGroup;
        private Coroutine fadeCoroutine;
        private Coroutine typewriterCoroutine;

        [Header("Input System")]
        [SerializeField] private InputActionReference interactAction; // Referência à ação de interação

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // Configura o Input System
                if (interactAction != null)
                {
                    interactAction.action.Enable();
                    interactAction.action.performed += OnInteractInput;
                }
            }
            else
            {
                Destroy(gameObject);
            }

            // Garante que a caixa de diálogo começa invisível
            if (dialogBox != null)
            {
                canvasGroup = dialogBox.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = dialogBox.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 0f;
                dialogBox.SetActive(true);
            }
        }

        /// <summary>
        /// Callback para quando o botão de interação é pressionado
        /// </summary>
        private void OnInteractInput(InputAction.CallbackContext context)
        {
            if (isDialogActive)
            {
                // Se estiver digitando o texto, pula a animação
                if (isTyping)
                {
                    SkipTypewriter();
                }
                // Se não estiver digitando, fecha o diálogo
                else
                {
                    CloseDialog();
                }
            }
        }

        private void Start()
        {
            if (dialogBox == null)
            {
                dialogBox = GameObject.Find("ui_dialogbox");

                if (dialogBox != null)
                {
                    // Tenta encontrar os componentes automaticamente
                    TextMeshProUGUI[] textComponents = dialogBox.GetComponentsInChildren<TextMeshProUGUI>();
                    if (textComponents.Length >= 2)
                    {
                        // Assumindo que o primeiro texto é o título e o segundo é o conteúdo do diálogo
                        titleText = textComponents[0];
                        dialogText = textComponents[1];
                    }
                    else if (textComponents.Length == 1)
                    {
                        dialogText = textComponents[0];
                    }

                    speakerIcon = dialogBox.GetComponentInChildren<Image>();

                    // Obtém ou adiciona o componente CanvasGroup para controlar o fade
                    canvasGroup = dialogBox.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        canvasGroup = dialogBox.AddComponent<CanvasGroup>();
                    }

                    // Garante que a caixa de diálogo começa invisível mas ativa
                    canvasGroup.alpha = 0f;
                    dialogBox.SetActive(true);
                }
                else
                {
                    Debug.LogError("Dialog Manager: Não foi possível encontrar a caixa de diálogo \"ui_dialogbox\"");
                }
            }
        }

        /// <summary>
        /// Exibe uma caixa de diálogo com o texto especificado
        /// </summary>
        /// <param name="text">O texto a ser exibido</param>
        /// <param name="title">Título do diálogo (nome do NPC ou item)</param>
        /// <param name="icon">Ícone opcional para exibir</param>
        /// <param name="onComplete">Ação a ser executada quando o diálogo for fechado</param>
        public void ShowDialog(string text, string title = "", Sprite icon = null, Action onComplete = null)
        {
            if (dialogBox == null)
            {
                Debug.LogError("Dialog Manager: Caixa de diálogo não encontrada!");
                return;
            }

            // Cancela qualquer fade que esteja em progresso
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            isDialogActive = true;

            // Salva o texto para o efeito de typewriter
            currentFullText = text;

            // Cancela qualquer animação de texto em andamento
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                isTyping = false;
            }

            // Inicialmente, deixa o texto vazio (vai ser preenchido gradualmente)
            if (dialogText != null)
                dialogText.text = string.Empty;

            if (titleText != null)
                titleText.text = title;

            if (speakerIcon != null)
            {
                if (icon != null)
                {
                    speakerIcon.sprite = icon;
                    speakerIcon.gameObject.SetActive(true);
                }
                else
                {
                    speakerIcon.gameObject.SetActive(false);
                }
            }

            onDialogComplete = onComplete;

            // Inicia o fade in
            fadeCoroutine = StartCoroutine(FadeIn());

            // Inicia o efeito de typewriter após o fade in
            typewriterCoroutine = StartCoroutine(TypeText(currentFullText));
        }

        /// <summary>
        /// Fecha a caixa de diálogo atual
        /// </summary>
        public void CloseDialog()
        {
            if (dialogBox == null) return;

            // Para qualquer animação de texto
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                isTyping = false;
            }

            // Inicia a coroutine de fade out
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeOut());
            isDialogActive = false;  // Marca como inativo imediatamente para evitar múltiplas chamadas
        }

        public bool IsDialogActive()
        {
            return isDialogActive;
        }

        // O método Update foi removido pois agora usamos o Input System com eventos

        /// <summary>
        /// Pula a animação de digitação e mostra o texto completo imediatamente
        /// </summary>
        public void SkipTypewriter()
        {
            if (isTyping && dialogText != null)
            {
                // Para a coroutine de digitação
                if (typewriterCoroutine != null)
                {
                    StopCoroutine(typewriterCoroutine);
                }

                // Mostra o texto completo
                dialogText.text = currentFullText;
                isTyping = false;
            }
        }

        /// <summary>
        /// Coroutine para fazer o fade in da caixa de diálogo
        /// </summary>
        private IEnumerator FadeIn()
        {
            if (canvasGroup != null)
            {
                float elapsedTime = 0f;
                canvasGroup.alpha = 0f;
                dialogBox.SetActive(true);

                while (elapsedTime < fadeTime)
                {
                    canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                canvasGroup.alpha = 1f;

                // Inicia a digitação do texto se o texto do diálogo estiver definido
                if (!string.IsNullOrEmpty(dialogText.text))
                {
                    StartCoroutine(TypeText(dialogText.text));
                }
            }
        }

        /// <summary>
        /// Coroutine para fazer o fade out da caixa de diálogo
        /// </summary>
        private IEnumerator FadeOut()
        {
            if (canvasGroup != null)
            {
                float elapsedTime = 0f;
                canvasGroup.alpha = 1f;

                while (elapsedTime < fadeTime)
                {
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                canvasGroup.alpha = 0f;

                // Invoca o callback se existir
                onDialogComplete?.Invoke();
                onDialogComplete = null;
            }
        }

        /// <summary>
        /// Coroutine para digitação do texto letra por letra (efeito máquina de escrever)
        /// </summary>
        private IEnumerator TypeText(string text)
        {
            if (dialogText != null)
            {
                isTyping = true;
                dialogText.text = string.Empty;

                // Espera o fade in terminar para começar a digitar
                if (canvasGroup != null && canvasGroup.alpha < 1f)
                {
                    yield return new WaitUntil(() => canvasGroup.alpha >= 1f);
                }

                // Efeito de digitação
                foreach (char letter in text.ToCharArray())
                {
                    // Se o usuário pulou a animação, saia da coroutine
                    if (!isTyping)
                    {
                        break;
                    }

                    dialogText.text += letter;
                    yield return new WaitForSeconds(typingSpeed);
                }

                // Garante que todo o texto está visível no final
                dialogText.text = text;
                isTyping = false;
            }
        }

        private void OnDestroy()
        {
            // Limpa o evento do Input System
            if (interactAction != null)
            {
                interactAction.action.performed -= OnInteractInput;
            }
        }
    }
}
