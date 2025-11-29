using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace TheSlimeKing.Dialogue
{
    /// <summary>
    /// Componente responsável pelo efeito de digitação letra por letra (typewriter effect).
    /// Exibe texto caractere por caractere com velocidade configurável.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TypewriterEffect : MonoBehaviour
    {
        #region Serialized Fields
        
        [Header("Typewriter Settings")]
        [Tooltip("Velocidade de digitação em caracteres por segundo")]
        [SerializeField] private float charactersPerSecond = 30f;
        
        [Tooltip("Delay adicional para pontuação (segundos)")]
        [SerializeField] private float punctuationDelay = 0.1f;
        
        [Tooltip("Pular espaços sem delay")]
        [SerializeField] private bool skipSpaces = true;
        
        [Header("Audio (Optional)")]
        [Tooltip("Som de digitação (opcional)")]
        [SerializeField] private AudioClip typingSound;
        
        [Tooltip("Volume do som de digitação")]
        [SerializeField] [Range(0f, 1f)] private float typingSoundVolume = 0.5f;
        
        #endregion
        
        #region Private Fields
        
        private TextMeshProUGUI targetText;
        private Coroutine typingCoroutine;
        private bool isTyping;
        private AudioSource audioSource;
        private Action currentOnComplete;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Verifica se o efeito de digitação está em execução.
        /// </summary>
        public bool IsTyping => isTyping;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            targetText = GetComponent<TextMeshProUGUI>();
            
            if (targetText == null)
            {
                Debug.LogError($"[TypewriterEffect] TextMeshProUGUI não encontrado em {gameObject.name}");
            }
            
            // Criar AudioSource se som está configurado
            if (typingSound != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = typingSound;
                audioSource.volume = typingSoundVolume;
                audioSource.playOnAwake = false;
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Inicia o efeito de digitação para o texto fornecido.
        /// </summary>
        /// <param name="text">Texto a ser exibido</param>
        /// <param name="onComplete">Callback chamado quando a digitação termina</param>
        public void StartTyping(string text, Action onComplete = null)
        {
            if (targetText == null)
            {
                Debug.LogError("[TypewriterEffect] TextMeshProUGUI não está configurado");
                onComplete?.Invoke();
                return;
            }
            
            // Parar digitação anterior se existir
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            
            currentOnComplete = onComplete;
            typingCoroutine = StartCoroutine(TypeText(text, onComplete));
        }
        
        /// <summary>
        /// Completa instantaneamente a exibição do texto atual.
        /// </summary>
        public void CompleteInstantly()
        {
            if (!isTyping) return;
            
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            
            isTyping = false;
            
            // Invocar callback se existir
            Action callback = currentOnComplete;
            currentOnComplete = null;
            callback?.Invoke();
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Coroutine que exibe o texto caractere por caractere.
        /// </summary>
        private IEnumerator TypeText(string text, Action onComplete)
        {
            isTyping = true;
            targetText.text = "";
            
            if (string.IsNullOrEmpty(text))
            {
                isTyping = false;
                onComplete?.Invoke();
                yield break;
            }
            
            foreach (char c in text)
            {
                targetText.text += c;
                
                // Reproduzir som de digitação
                PlayTypingSound();
                
                // Calcular delay para este caractere
                float delay = GetCharacterDelay(c);
                
                if (delay > 0)
                {
                    yield return new WaitForSeconds(delay);
                }
            }
            
            isTyping = false;
            typingCoroutine = null;
            currentOnComplete = null;
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// Calcula o delay apropriado para um caractere específico.
        /// </summary>
        private float GetCharacterDelay(char c)
        {
            // Espaços não têm delay se skipSpaces está ativo
            if (skipSpaces && char.IsWhiteSpace(c))
            {
                return 0f;
            }
            
            // Pontuação tem delay adicional
            if (IsPunctuation(c))
            {
                return (1f / charactersPerSecond) + punctuationDelay;
            }
            
            // Delay normal
            return 1f / charactersPerSecond;
        }
        
        /// <summary>
        /// Verifica se um caractere é pontuação.
        /// </summary>
        private bool IsPunctuation(char c)
        {
            return c == '.' || c == ',' || c == '!' || c == '?' || c == ';' || c == ':';
        }
        
        /// <summary>
        /// Reproduz o som de digitação se configurado.
        /// </summary>
        private void PlayTypingSound()
        {
            if (audioSource != null && typingSound != null)
            {
                audioSource.PlayOneShot(typingSound, typingSoundVolume);
            }
        }
        
        #endregion
    }
}
