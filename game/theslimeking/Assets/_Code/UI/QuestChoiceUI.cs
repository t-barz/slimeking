using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SlimeKing.Systems;

namespace SlimeKing.Systems.UI
{
    /// <summary>
    /// Gerencia a exibição de opções de quest em diálogos.
    /// Exibe botões dinâmicos para aceitar ou entregar quests.
    /// </summary>
    public class QuestChoiceUI : MonoBehaviour
    {
        #region Inspector Variables
        [Header("UI References")]
        [Tooltip("GameObject do painel de escolhas (será mostrado/ocultado)")]
        [SerializeField] private GameObject choicePanel;

        [Tooltip("Transform onde os botões de escolha serão instanciados")]
        [SerializeField] private Transform choiceButtonContainer;

        [Tooltip("Prefab do botão de escolha")]
        [SerializeField] private GameObject choiceButtonPrefab;

        [Tooltip("Botão para fechar o painel sem escolher nada")]
        [SerializeField] private Button closeButton;

        [Header("Settings")]
        [Tooltip("Espaçamento vertical entre botões")]
        [SerializeField] private float buttonSpacing = 10f;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        #endregion

        #region Private Variables
        /// <summary>
        /// Lista de botões de escolha instanciados
        /// </summary>
        private List<GameObject> choiceButtons = new List<GameObject>();

        /// <summary>
        /// CanvasGroup para controlar fade in/out
        /// </summary>
        private CanvasGroup canvasGroup;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Validar componentes
            ValidateComponents();

            // Obter ou adicionar CanvasGroup
            canvasGroup = choicePanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = choicePanel.AddComponent<CanvasGroup>();
            }

            // Inicializar invisível
            canvasGroup.alpha = 0f;
            choicePanel.SetActive(false);

            // Configurar botão de fechar
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClicked);
            }
        }

        private void OnEnable()
        {
            // Escuta evento de fim de diálogo para exibir opções de quest
            if (DialogueManager.HasInstance)
            {
                DialogueManager.Instance.OnDialogueEnded.AddListener(OnDialogueEnded);
            }
        }

        private void OnDisable()
        {
            // Remove listener
            if (DialogueManager.HasInstance)
            {
                DialogueManager.Instance.OnDialogueEnded.RemoveListener(OnDialogueEnded);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Exibe o painel de escolhas de quest.
        /// </summary>
        /// <param name="choices">Lista de opções de quest</param>
        public void ShowQuestChoices(List<QuestDialogueChoice> choices)
        {
            if (choices == null || choices.Count == 0)
            {
                if (enableDebugLogs)
                    UnityEngine.Debug.Log("[QuestChoiceUI] Nenhuma opção de quest para exibir.");
                return;
            }

            // Limpa botões anteriores
            ClearChoiceButtons();

            // Cria botões para cada opção
            foreach (QuestDialogueChoice choice in choices)
            {
                CreateChoiceButton(choice);
            }

            // Exibe painel
            choicePanel.SetActive(true);
            canvasGroup.alpha = 1f;

            if (enableDebugLogs)
                UnityEngine.Debug.Log($"[QuestChoiceUI] Exibindo {choices.Count} opções de quest.");
        }

        /// <summary>
        /// Oculta o painel de escolhas de quest.
        /// </summary>
        public void HideQuestChoices()
        {
            // Limpa botões
            ClearChoiceButtons();

            // Oculta painel
            canvasGroup.alpha = 0f;
            choicePanel.SetActive(false);

            // Despausa o jogador
            if (DialogueManager.HasInstance)
            {
                DialogueManager.Instance.SetPausePlayerDuringDialogue(false);
                // Força despausa chamando método privado via reflexão (workaround)
                var method = typeof(DialogueManager).GetMethod("UnpausePlayer", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                method?.Invoke(DialogueManager.Instance, null);
            }

            if (enableDebugLogs)
                UnityEngine.Debug.Log("[QuestChoiceUI] Painel de escolhas ocultado.");
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Valida se todos os componentes necessários estão atribuídos.
        /// </summary>
        private void ValidateComponents()
        {
            if (choicePanel == null)
            {
                UnityEngine.Debug.LogError("[QuestChoiceUI] choicePanel não está atribuído!");
            }

            if (choiceButtonContainer == null)
            {
                UnityEngine.Debug.LogError("[QuestChoiceUI] choiceButtonContainer não está atribuído!");
            }

            if (choiceButtonPrefab == null)
            {
                UnityEngine.Debug.LogError("[QuestChoiceUI] choiceButtonPrefab não está atribuído!");
            }

            if (closeButton == null)
            {
                UnityEngine.Debug.LogWarning("[QuestChoiceUI] closeButton não está atribuído.");
            }
        }

        /// <summary>
        /// Cria um botão de escolha para uma opção de quest.
        /// </summary>
        /// <param name="choice">Opção de quest</param>
        private void CreateChoiceButton(QuestDialogueChoice choice)
        {
            if (choiceButtonPrefab == null || choiceButtonContainer == null)
                return;

            // Instancia botão
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            choiceButtons.Add(buttonObj);

            // Configura texto do botão
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = choice.choiceText;
            }

            // Configura ação do botão
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnChoiceButtonClicked(choice));
            }

            if (enableDebugLogs)
                UnityEngine.Debug.Log($"[QuestChoiceUI] Botão criado: {choice.choiceText}");
        }

        /// <summary>
        /// Limpa todos os botões de escolha instanciados.
        /// </summary>
        private void ClearChoiceButtons()
        {
            foreach (GameObject button in choiceButtons)
            {
                if (button != null)
                {
                    Destroy(button);
                }
            }

            choiceButtons.Clear();
        }

        /// <summary>
        /// Callback quando um botão de escolha é clicado.
        /// </summary>
        /// <param name="choice">Opção de quest selecionada</param>
        private void OnChoiceButtonClicked(QuestDialogueChoice choice)
        {
            if (enableDebugLogs)
                UnityEngine.Debug.Log($"[QuestChoiceUI] Escolha selecionada: {choice.choiceText}");

            // Executa ação da escolha
            if (DialogueChoiceHandler.Instance != null)
            {
                DialogueChoiceHandler.Instance.ExecuteQuestChoice(choice);
            }

            // Atualiza opções (pode ter mudado após executar ação)
            UpdateQuestChoices();
        }

        /// <summary>
        /// Callback quando o botão de fechar é clicado.
        /// </summary>
        private void OnCloseButtonClicked()
        {
            if (enableDebugLogs)
                UnityEngine.Debug.Log("[QuestChoiceUI] Botão de fechar clicado.");

            HideQuestChoices();
        }

        /// <summary>
        /// Callback quando o diálogo termina.
        /// Verifica se há opções de quest para exibir.
        /// </summary>
        private void OnDialogueEnded()
        {
            if (DialogueChoiceHandler.Instance == null)
                return;

            if (DialogueChoiceHandler.Instance.HasQuestChoices())
            {
                List<QuestDialogueChoice> choices = DialogueChoiceHandler.Instance.GetQuestChoices();
                ShowQuestChoices(choices);
            }
        }

        /// <summary>
        /// Atualiza as opções de quest exibidas.
        /// Útil após executar uma ação que pode mudar as opções disponíveis.
        /// </summary>
        private void UpdateQuestChoices()
        {
            if (DialogueChoiceHandler.Instance == null)
            {
                HideQuestChoices();
                return;
            }

            if (DialogueChoiceHandler.Instance.HasQuestChoices())
            {
                List<QuestDialogueChoice> choices = DialogueChoiceHandler.Instance.GetQuestChoices();
                ShowQuestChoices(choices);
            }
            else
            {
                HideQuestChoices();
            }
        }
        #endregion
    }
}
