using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TheSlimeKing.Core;

namespace TheSlimeKing.Core.Dialogue
{
    /// <summary>
    /// Gerencia o sistema de diálogos, controlando a exibição de textos, animações e interações.
    /// Este é um singleton que coordena todo o fluxo de diálogos do jogo.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("Componentes da UI")]
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image _speakerImage;
        [SerializeField] private GameObject _continueIndicator;

        [Header("Configurações de Animação")]
        [SerializeField] private float _charactersPerSecond = 30f;
        [SerializeField] private float _commaDelay = 0.1f;
        [SerializeField] private float _periodDelay = 0.3f;
        [SerializeField] private AudioClip _typingSoundEffect;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float _typingSoundVolume = 0.2f;

        // Estado interno
        private Queue<DialogueLine> _dialogueLines = new Queue<DialogueLine>();
        private bool _isDialogueActive = false;
        private bool _isTyping = false;
        private AudioSource _audioSource;
        private Coroutine _typingCoroutine = null;
        private bool _skipRequested = false;
        private Action _onDialogueComplete = null;

        // Eventos
        public event Action OnDialogueStart;
        public event Action OnDialogueLine;
        public event Action OnDialogueEnd;

        private void Awake()
        {
            // Configuração do singleton
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Inicialização de componentes
            _audioSource = gameObject.AddComponent<AudioSource>();
            _dialoguePanel.SetActive(false);
        }

        /// <summary>
        /// Inicia um diálogo com as linhas especificadas.
        /// </summary>
        /// <param name="dialogueData">Dados do diálogo a ser apresentado</param>
        /// <param name="onComplete">Callback a ser chamado quando o diálogo terminar</param>
        public void StartDialogue(DialogueData dialogueData, Action onComplete = null)
        {
            // Verificação de segurança
            if (dialogueData == null || dialogueData.Lines.Count == 0)
            {
                Debug.LogWarning("Tentativa de iniciar um diálogo vazio ou nulo.");
                return;
            }

            _dialogueLines.Clear();
            _onDialogueComplete = onComplete;

            // Enfileira as linhas do diálogo
            foreach (var line in dialogueData.Lines)
            {
                _dialogueLines.Enqueue(line);
            }

            // Ativa o painel de diálogo e dispara evento
            _dialoguePanel.SetActive(true);
            _isDialogueActive = true;
            _continueIndicator.SetActive(false);
            OnDialogueStart?.Invoke();

            // Inicia o processamento da primeira linha
            DisplayNextLine();
        }

        /// <summary>
        /// Inicia um diálogo simples com uma única linha.
        /// </summary>
        /// <param name="localizationKey">Chave de localização do texto a ser exibido</param>
        /// <param name="speakerName">Nome de quem está falando (opcional)</param>
        /// <param name="onComplete">Callback a ser chamado quando o diálogo terminar</param>
        public void StartSimpleDialogue(string localizationKey, string speakerName = "", Action onComplete = null)
        {
            DialogueData simpleDialogue = ScriptableObject.CreateInstance<DialogueData>();
            DialogueLine line = new DialogueLine
            {
                TextKey = localizationKey,
                SpeakerNameKey = speakerName
            };

            simpleDialogue.Lines = new List<DialogueLine> { line };
            StartDialogue(simpleDialogue, onComplete);
        }

        /// <summary>
        /// Avança para a próxima linha do diálogo ou finaliza se não houver mais linhas.
        /// </summary>
        public void ContinueDialogue()
        {
            // Se ainda estiver digitando, completa instantaneamente
            if (_isTyping)
            {
                _skipRequested = true;
                return;
            }

            // Se não há mais linhas, finaliza o diálogo
            if (_dialogueLines.Count == 0)
            {
                EndDialogue();
                return;
            }

            // Exibe a próxima linha
            DisplayNextLine();
        }

        /// <summary>
        /// Encerra o diálogo atual.
        /// </summary>
        public void EndDialogue()
        {
            _dialoguePanel.SetActive(false);
            _isDialogueActive = false;
            _continueIndicator.SetActive(false);

            // Limpa qualquer texto residual
            _dialogueText.text = string.Empty;

            // Dispara evento de fim de diálogo
            OnDialogueEnd?.Invoke();

            // Executa callback de conclusão, se existir
            _onDialogueComplete?.Invoke();
            _onDialogueComplete = null;
        }

        /// <summary>
        /// Verifica se há um diálogo ativo atualmente.
        /// </summary>
        /// <returns>True se um diálogo estiver ativo</returns>
        public bool IsDialogueActive()
        {
            return _isDialogueActive;
        }

        /// <summary>
        /// Exibe a próxima linha do diálogo com animação de digitação.
        /// </summary>
        private void DisplayNextLine()
        {
            if (_dialogueLines.Count == 0) return;

            // Pega a próxima linha da fila
            DialogueLine line = _dialogueLines.Dequeue();

            // Configura o nome do falante, se houver
            if (!string.IsNullOrEmpty(line.SpeakerNameKey))
            {
                string speakerName = LocalizationManager.Instance.GetLocalizedText(line.SpeakerNameKey);
                _nameText.text = speakerName;
                _nameText.gameObject.SetActive(true);
            }
            else
            {
                _nameText.gameObject.SetActive(false);
            }

            // Configura a imagem do falante, se houver
            if (line.SpeakerSprite != null)
            {
                _speakerImage.sprite = line.SpeakerSprite;
                _speakerImage.gameObject.SetActive(true);
            }
            else
            {
                _speakerImage.gameObject.SetActive(false);
            }

            // Obtém o texto localizado
            string localizedText = LocalizationManager.Instance.GetLocalizedText(line.TextKey);

            // Inicia a animação de digitação
            _continueIndicator.SetActive(false);
            if (_typingCoroutine != null)
            {
                StopCoroutine(_typingCoroutine);
            }
            _typingCoroutine = StartCoroutine(TypeTextCoroutine(localizedText));
            OnDialogueLine?.Invoke();
        }

        /// <summary>
        /// Coroutine que anima o texto caractere por caractere.
        /// </summary>
        private IEnumerator TypeTextCoroutine(string text)
        {
            _isTyping = true;
            _skipRequested = false;
            _dialogueText.text = "";

            float inverseTypingSpeed = 1f / _charactersPerSecond;
            float typingSoundTimer = 0f;

            for (int i = 0; i < text.Length; i++)
            {
                // Verifica se o usuário pediu para pular a animação
                if (_skipRequested)
                {
                    _dialogueText.text = text;
                    break;
                }

                // Adiciona um caractere por vez
                _dialogueText.text += text[i];

                // Reproduz som de digitação em intervalos
                typingSoundTimer -= Time.deltaTime;
                if (typingSoundTimer <= 0 && _typingSoundEffect != null)
                {
                    _audioSource.PlayOneShot(_typingSoundEffect, _typingSoundVolume);
                    typingSoundTimer = inverseTypingSpeed * 2f; // Som a cada 2 caracteres
                }

                // Adiciona pausas em pontuações
                if (text[i] == ',')
                {
                    yield return new WaitForSeconds(_commaDelay);
                }
                else if (text[i] == '.' || text[i] == '!' || text[i] == '?')
                {
                    yield return new WaitForSeconds(_periodDelay);
                }
                else
                {
                    yield return new WaitForSeconds(inverseTypingSpeed);
                }
            }

            // Texto completo, mostra indicador de continuação
            _isTyping = false;
            _continueIndicator.SetActive(true);
            _typingCoroutine = null;
        }
    }
}
