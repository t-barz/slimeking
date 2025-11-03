using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TheSlimeKing.Quest
{
    /// <summary>
    /// Controller anexado a NPCs que oferecem quests ao jogador.
    /// Gerencia indicadores visuais e integração com QuestManager.
    /// </summary>
    public class QuestGiverController : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Quest Configuration")]
        [SerializeField] private List<CollectQuestData> availableQuests = new List<CollectQuestData>();
        
        [Header("Visual Indicators")]
        [SerializeField] private GameObject questAvailableIndicator; // ! amarelo
        [SerializeField] private GameObject questReadyIndicator; // ! dourado
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private bool showGizmos = true;
        #endregion
        
        #region Unity Lifecycle
        private void Start()
        {
            RegisterWithQuestManager();
            UpdateIndicators();
        }
        
        private void OnEnable()
        {
            SubscribeToEvents();
        }
        
        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }
        #endregion
        
        #region Initialization
        /// <summary>
        /// Registra este Quest Giver no QuestManager para cache e gerenciamento.
        /// </summary>
        private void RegisterWithQuestManager()
        {
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.RegisterQuestGiver(this);
                
                if (enableDebugLogs)
                    Debug.Log($"[QuestGiver] {gameObject.name} registered with QuestManager");
            }
            else
            {
                Debug.LogWarning($"[QuestGiver] {gameObject.name} - QuestManager.Instance is null!");
            }
        }
        #endregion
        
        #region Event Subscription
        /// <summary>
        /// Inscreve-se nos eventos de quest para atualizar indicadores automaticamente.
        /// </summary>
        private void SubscribeToEvents()
        {
            QuestEvents.OnQuestAccepted += OnQuestAccepted;
            QuestEvents.OnQuestReadyToTurnIn += OnQuestReadyChanged;
            QuestEvents.OnQuestTurnedIn += OnQuestTurnedIn;
        }
        
        /// <summary>
        /// Desinscreve-se dos eventos de quest.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            QuestEvents.OnQuestAccepted -= OnQuestAccepted;
            QuestEvents.OnQuestReadyToTurnIn -= OnQuestReadyChanged;
            QuestEvents.OnQuestTurnedIn -= OnQuestTurnedIn;
        }
        #endregion
        
        #region Public Methods
        /// <summary>
        /// Retorna lista de quests disponíveis neste Quest Giver.
        /// </summary>
        /// <returns>Lista de CollectQuestData</returns>
        public List<CollectQuestData> GetAvailableQuests()
        {
            return availableQuests;
        }
        
        /// <summary>
        /// Verifica se este Quest Giver tem alguma quest disponível para aceitar.
        /// </summary>
        /// <returns>True se há quest disponível</returns>
        public bool HasQuestAvailable()
        {
            if (QuestManager.Instance == null || availableQuests == null || availableQuests.Count == 0)
                return false;
            
            return availableQuests.Any(q => q != null && QuestManager.Instance.CanAcceptQuest(q));
        }
        
        /// <summary>
        /// Verifica se este Quest Giver tem alguma quest pronta para entregar.
        /// </summary>
        /// <returns>True se há quest pronta para entrega</returns>
        public bool HasQuestReadyToTurnIn()
        {
            if (QuestManager.Instance == null || availableQuests == null || availableQuests.Count == 0)
                return false;
            
            return availableQuests.Any(q => q != null && QuestManager.Instance.IsQuestReadyToTurnIn(q.questID));
        }
        #endregion
        
        #region Private Methods
        /// <summary>
        /// Callback quando uma quest é aceita.
        /// </summary>
        /// <param name="quest">Quest que foi aceita</param>
        private void OnQuestAccepted(CollectQuestData quest)
        {
            if (quest == null)
                return;
            
            // Atualiza indicadores apenas se quest pertence a este giver
            if (availableQuests.Any(q => q != null && q.questID == quest.questID))
            {
                UpdateIndicators();
            }
        }
        
        /// <summary>
        /// Callback quando uma quest é entregue.
        /// </summary>
        /// <param name="questID">ID da quest entregue</param>
        private void OnQuestTurnedIn(string questID)
        {
            if (string.IsNullOrEmpty(questID))
                return;
            
            // Atualiza indicadores apenas se quest pertence a este giver
            if (availableQuests.Any(q => q != null && q.questID == questID))
            {
                UpdateIndicators();
            }
        }
        
        /// <summary>
        /// Callback quando quest fica pronta para entregar.
        /// </summary>
        /// <param name="questID">ID da quest pronta</param>
        private void OnQuestReadyChanged(string questID)
        {
            if (string.IsNullOrEmpty(questID))
                return;
            
            // Atualiza indicadores apenas se quest pertence a este giver
            if (availableQuests.Any(q => q != null && q.questID == questID))
            {
                UpdateIndicators();
            }
        }
        
        /// <summary>
        /// Atualiza indicadores visuais baseado no estado das quests.
        /// Prioridade: Quest Ready > Quest Available > Nenhum indicador
        /// </summary>
        private void UpdateIndicators()
        {
            bool hasAvailable = HasQuestAvailable();
            bool hasReady = HasQuestReadyToTurnIn();
            
            // Quest pronta tem prioridade sobre quest disponível
            if (questAvailableIndicator != null)
                questAvailableIndicator.SetActive(hasAvailable && !hasReady);
            
            if (questReadyIndicator != null)
                questReadyIndicator.SetActive(hasReady);
            
            if (enableDebugLogs)
                Debug.Log($"[QuestGiver] {gameObject.name} - Available: {hasAvailable}, Ready: {hasReady}");
        }
        #endregion
        
        #region Gizmos
        /// <summary>
        /// Desenha gizmos no Editor para visualizar Quest Givers na cena.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!showGizmos) return;
            
            // Desenha esfera amarela indicando Quest Giver
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.5f);
            
            // Desenha linha conectando ao transform
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
        }
        #endregion
    }
}
