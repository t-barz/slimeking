using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

namespace TheSlimeKing.Dialogue
{
    /// <summary>
    /// Componente que gerencia a interface visual do diálogo e a navegação entre textos.
    /// Controla a exibição do Canvas, integração com TypewriterEffect e eventos de conclusão.
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        #region Serialized Fields
        
        [Header("UI References")]
        [Tooltip("Panel principal do diálogo")]
        [SerializeField] private GameObject dialoguePanel;
        
        [Tooltip("Imagem de fundo do diálogo")]
        [SerializeField] private Image backgroundImage;
        
        [Tooltip("Texto do diálogo (TextMeshPro)")]
        [SerializeField] private TextMeshProUGUI dialogueText;
        
        [Tooltip("Indicador visual de 'pressione para continuar'")]
        [SerializeField] private GameObject continueIndicator;
        
        [Header("Settings")]
        [Tooltip("Sorting order do Canvas")]
        [SerializeField] private int sortingOrder = 100;
        
        #endregion
        
        #region Private Fields
        
        private List<string> currentTexts;
        private int currentTextIndex;
        private TypewriterEffect typewriter;
        private UnityEvent onDialogueComplete;
        private bool isActive;
        private Canvas canvas;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Verifica se o diálogo está ativo.
        /// </summary>
        public bool IsActive => isActive;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            ValidateReferences();
            
            // Obter Canvas e configurar sorting order
            canvas = GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.sortingOrder = sortingOrder;
            }
            
            // Obter TypewriterEffect do texto
            if (dialogueText != null)
            {
                typewriter = dialogueText.GetComponent<TypewriterEffect>();
                if (typewriter == null)
                {
                    Debug.LogWarning($"[DialogueUI] TypewriterEffect não encontrado em {dialogueText.name}. Adicionando componente.");
                    typewriter = dialogueText.gameObject.AddComponent<TypewriterEffect>();
                }
            }
            
            // Iniciar com diálogo escondido
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(false);
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Mostra o diálogo com a lista de textos localizados fornecida.
        /// </summary>
        /// <param name="localizedTexts">Lista de LocalizedString a serem exibidos</param>
        /// <param name="onComplete">Evento chamado quando o diálogo é completado</param>
        public void Show(List<LocalizedString> localizedTexts, UnityEvent onComplete = null)
        {
            if (isActive)
            {
                Debug.LogWarning("[DialogueUI] Diálogo já está ativo. Ignorando nova requisição.");
                return;
            }
            
            if (localizedTexts == null || localizedTexts.Count == 0)
            {
                Debug.LogWarning("[DialogueUI] Lista de textos está vazia. Não é possível mostrar diálogo.");
                return;
            }
            
            onDialogueComplete = onComplete;
            StartCoroutine(LoadAndShowDialogue(localizedTexts));
        }
        
        /// <summary>
        /// Esconde o diálogo e limpa o estado.
        /// </summary>
        public void Hide()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
            
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(false);
            }
            
            currentTexts = null;
            currentTextIndex = 0;
            isActive = false;
            
            if (dialogueText != null)
            {
                dialogueText.text = "";
            }
            
            // Disparar evento de fim de diálogo
            DialogueEvents.InvokeDialogueEnd();
        }
        
        /// <summary>
        /// Chamado quando o jogador pressiona o botão de continuar.
        /// Completa o texto atual ou avança para o próximo.
        /// </summary>
        public void OnContinuePressed()
        {
            if (!isActive) return;
            
            // Se o typewriter está digitando, completar instantaneamente
            if (typewriter != null && typewriter.IsTyping)
            {
                typewriter.CompleteInstantly();
                ShowContinueIndicator();
                return;
            }
            
            // Caso contrário, avançar para o próximo texto
            ShowNextText();
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Valida se todas as referências necessárias estão configuradas.
        /// </summary>
        private void ValidateReferences()
        {
            bool hasErrors = false;
            
            if (dialoguePanel == null)
            {
                Debug.LogError($"[DialogueUI] dialoguePanel não está configurado em {gameObject.name}");
                hasErrors = true;
            }
            
            if (dialogueText == null)
            {
                Debug.LogError($"[DialogueUI] dialogueText não está configurado em {gameObject.name}");
                hasErrors = true;
            }
            
            if (continueIndicator == null)
            {
                Debug.LogWarning($"[DialogueUI] continueIndicator não está configurado em {gameObject.name}");
            }
            
            if (hasErrors)
            {
                Debug.LogError("[DialogueUI] Configuração incompleta! Use a ferramenta 'Setup Dialogue NPC' para configurar automaticamente.");
            }
        }
        
        /// <summary>
        /// Carrega os textos localizados e inicia a exibição do diálogo.
        /// </summary>
        private IEnumerator LoadAndShowDialogue(List<LocalizedString> localizedTexts)
        {
            currentTexts = new List<string>();
            
            // Carregar todos os textos localizados
            foreach (var localizedString in localizedTexts)
            {
                if (localizedString == null || localizedString.IsEmpty)
                {
                    Debug.LogWarning("[DialogueUI] LocalizedString vazio encontrado. Pulando.");
                    continue;
                }
                
                // Carregar texto de forma síncrona (para simplicidade)
                string text = localizedString.GetLocalizedString();
                
                if (string.IsNullOrEmpty(text))
                {
                    Debug.LogWarning($"[DialogueUI] Falha ao carregar texto localizado. Usando fallback.");
                    text = "[Texto não disponível]";
                }
                
                currentTexts.Add(text);
            }
            
            if (currentTexts.Count == 0)
            {
                Debug.LogError("[DialogueUI] Nenhum texto válido foi carregado.");
                yield break;
            }
            
            // Ativar diálogo
            isActive = true;
            currentTextIndex = 0;
            
            // Disparar evento de início de diálogo
            DialogueEvents.InvokeDialogueStart();
            
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }
            
            // Mostrar primeiro texto
            ShowCurrentText();
        }
        
        /// <summary>
        /// Exibe o texto atual usando o TypewriterEffect.
        /// </summary>
        private void ShowCurrentText()
        {
            if (currentTexts == null || currentTextIndex >= currentTexts.Count)
            {
                Debug.LogError("[DialogueUI] Índice de texto inválido.");
                return;
            }
            
            // Esconder indicador de continuar
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(false);
            }
            
            // Iniciar digitação do texto atual
            string text = currentTexts[currentTextIndex];
            
            if (typewriter != null)
            {
                typewriter.StartTyping(text, OnTextComplete);
            }
            else
            {
                // Fallback se não houver typewriter
                if (dialogueText != null)
                {
                    dialogueText.text = text;
                }
                OnTextComplete();
            }
        }
        
        /// <summary>
        /// Chamado quando o TypewriterEffect completa a digitação do texto atual.
        /// </summary>
        private void OnTextComplete()
        {
            ShowContinueIndicator();
        }
        
        /// <summary>
        /// Mostra o indicador de continuar.
        /// </summary>
        private void ShowContinueIndicator()
        {
            if (continueIndicator != null)
            {
                continueIndicator.SetActive(true);
            }
        }
        
        /// <summary>
        /// Avança para o próximo texto ou completa o diálogo.
        /// </summary>
        private void ShowNextText()
        {
            currentTextIndex++;
            
            // Verificar se há mais textos
            if (currentTextIndex < currentTexts.Count)
            {
                ShowCurrentText();
            }
            else
            {
                // Não há mais textos, completar diálogo
                CompleteDialogue();
            }
        }
        
        /// <summary>
        /// Completa o diálogo, invoca eventos e esconde a UI.
        /// </summary>
        private void CompleteDialogue()
        {
            // Invocar eventos antes de esconder
            UnityEvent callback = onDialogueComplete;
            onDialogueComplete = null;
            
            // Esconder UI
            Hide();
            
            // Invocar callback após esconder
            callback?.Invoke();
        }
        
        #endregion
    }
}
