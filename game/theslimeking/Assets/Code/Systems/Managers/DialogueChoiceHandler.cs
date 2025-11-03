using UnityEngine;
using System.Collections.Generic;
using TheSlimeKing.Quest;

namespace SlimeMec.Systems
{
    /// <summary>
    /// Gerencia opções dinâmicas de quest em diálogos.
    /// Adiciona automaticamente opções de "Aceitar Quest" e "Entregar Quest" baseado no estado do QuestGiver.
    /// </summary>
    public class DialogueChoiceHandler : MonoBehaviour
    {
        public static DialogueChoiceHandler Instance { get; private set; }

        #region Singleton
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Inspector Variables
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        #endregion

        #region Private Variables
        /// <summary>
        /// QuestGiver atualmente registrado para o diálogo ativo
        /// </summary>
        private QuestGiverController currentQuestGiver;

        /// <summary>
        /// Lista de opções de quest disponíveis para o diálogo atual
        /// </summary>
        private List<QuestDialogueChoice> currentQuestChoices = new List<QuestDialogueChoice>();
        #endregion

        #region Initialization
        private void Initialize()
        {
            currentQuestGiver = null;
            currentQuestChoices = new List<QuestDialogueChoice>();

            if (enableDebugLogs)
                Debug.Log("[DialogueChoiceHandler] Initialized");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Registra um QuestGiver para o diálogo atual.
        /// Gera automaticamente opções de quest baseado no estado do QuestGiver.
        /// </summary>
        /// <param name="questGiver">QuestGiver a registrar</param>
        public void RegisterQuestGiver(QuestGiverController questGiver)
        {
            if (questGiver == null)
            {
                Debug.LogWarning("[DialogueChoiceHandler] Tentativa de registrar QuestGiver nulo.");
                return;
            }

            currentQuestGiver = questGiver;
            currentQuestChoices.Clear();

            // Gera opções de quest
            GenerateQuestChoices();

            if (enableDebugLogs)
                Debug.Log($"[DialogueChoiceHandler] QuestGiver registrado: {questGiver.gameObject.name} com {currentQuestChoices.Count} opções de quest.");
        }

        /// <summary>
        /// Limpa o QuestGiver registrado e opções de quest.
        /// Deve ser chamado quando o diálogo termina.
        /// </summary>
        public void ClearQuestGiver()
        {
            currentQuestGiver = null;
            currentQuestChoices.Clear();

            if (enableDebugLogs)
                Debug.Log("[DialogueChoiceHandler] QuestGiver limpo.");
        }

        /// <summary>
        /// Obtém as opções de quest disponíveis para o diálogo atual.
        /// </summary>
        /// <returns>Lista de opções de quest</returns>
        public List<QuestDialogueChoice> GetQuestChoices()
        {
            return currentQuestChoices;
        }

        /// <summary>
        /// Executa a ação de uma opção de quest.
        /// </summary>
        /// <param name="choice">Opção de quest a executar</param>
        public void ExecuteQuestChoice(QuestDialogueChoice choice)
        {
            if (choice == null)
            {
                Debug.LogWarning("[DialogueChoiceHandler] Tentativa de executar opção de quest nula.");
                return;
            }

            if (QuestManager.Instance == null)
            {
                Debug.LogError("[DialogueChoiceHandler] QuestManager não encontrado.");
                return;
            }

            switch (choice.type)
            {
                case QuestChoiceType.AcceptQuest:
                    if (choice.questData != null)
                    {
                        bool accepted = QuestManager.Instance.AcceptQuest(choice.questData);
                        if (accepted && enableDebugLogs)
                            Debug.Log($"[DialogueChoiceHandler] Quest aceita: {choice.questData.questName}");
                    }
                    break;

                case QuestChoiceType.TurnInQuest:
                    if (choice.questData != null)
                    {
                        bool turnedIn = QuestManager.Instance.TurnInQuest(choice.questData.questID);
                        if (turnedIn && enableDebugLogs)
                            Debug.Log($"[DialogueChoiceHandler] Quest entregue: {choice.questData.questName}");
                    }
                    break;
            }

            // Atualiza opções após executar ação
            GenerateQuestChoices();
        }

        /// <summary>
        /// Verifica se há opções de quest disponíveis.
        /// </summary>
        /// <returns>True se há opções de quest</returns>
        public bool HasQuestChoices()
        {
            return currentQuestChoices.Count > 0;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gera opções de quest baseado no estado do QuestGiver atual.
        /// </summary>
        private void GenerateQuestChoices()
        {
            currentQuestChoices.Clear();

            if (currentQuestGiver == null || QuestManager.Instance == null)
                return;

            List<CollectQuestData> availableQuests = currentQuestGiver.GetAvailableQuests();

            if (availableQuests == null || availableQuests.Count == 0)
                return;

            foreach (CollectQuestData quest in availableQuests)
            {
                if (quest == null)
                    continue;

                // Verifica se quest está pronta para entrega
                if (QuestManager.Instance.IsQuestReadyToTurnIn(quest.questID))
                {
                    QuestDialogueChoice turnInChoice = new QuestDialogueChoice
                    {
                        type = QuestChoiceType.TurnInQuest,
                        questData = quest,
                        choiceText = $"Entregar Quest: {quest.questName}"
                    };
                    currentQuestChoices.Add(turnInChoice);

                    if (enableDebugLogs)
                        Debug.Log($"[DialogueChoiceHandler] Opção de entrega adicionada: {quest.questName}");
                }
                // Verifica se quest pode ser aceita
                else if (QuestManager.Instance.CanAcceptQuest(quest))
                {
                    QuestDialogueChoice acceptChoice = new QuestDialogueChoice
                    {
                        type = QuestChoiceType.AcceptQuest,
                        questData = quest,
                        choiceText = $"Aceitar Quest: {quest.questName}"
                    };
                    currentQuestChoices.Add(acceptChoice);

                    if (enableDebugLogs)
                        Debug.Log($"[DialogueChoiceHandler] Opção de aceitar adicionada: {quest.questName}");
                }
            }
        }
        #endregion

        #region Unity Lifecycle
        private void OnEnable()
        {
            // Escuta evento de fim de diálogo para limpar QuestGiver
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

        private void OnDialogueEnded()
        {
            ClearQuestGiver();
        }
        #endregion
    }

    /// <summary>
    /// Tipo de opção de quest em diálogo.
    /// </summary>
    public enum QuestChoiceType
    {
        AcceptQuest,
        TurnInQuest
    }

    /// <summary>
    /// Representa uma opção de quest em diálogo.
    /// </summary>
    [System.Serializable]
    public class QuestDialogueChoice
    {
        public QuestChoiceType type;
        public CollectQuestData questData;
        public string choiceText;
    }
}
