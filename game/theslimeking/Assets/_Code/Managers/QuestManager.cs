using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TheSlimeKing.Inventory;
using SlimeKing.Core;
using UnityDebug = UnityEngine.Debug;

namespace TheSlimeKing.Quest
{
    /// <summary>
    /// Gerenciador central do Quest System.
    /// Coordena todas as quests ativas, completadas e integração com outros sistemas.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance { get; private set; }
        
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
        [SerializeField] private bool showGizmos = true;
        #endregion
        
        #region Private Variables
        private List<QuestProgress> activeQuests = new List<QuestProgress>();
        private List<string> completedQuestIDs = new List<string>();
        private Dictionary<string, QuestProgress> activeQuestsDict = new Dictionary<string, QuestProgress>();
        private Dictionary<string, QuestGiverController> questGiverCache = new Dictionary<string, QuestGiverController>();
        #endregion
        
        #region Unity Lifecycle
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
        private void Initialize()
        {
            activeQuests = new List<QuestProgress>();
            completedQuestIDs = new List<string>();
            activeQuestsDict = new Dictionary<string, QuestProgress>();
            questGiverCache = new Dictionary<string, QuestGiverController>();
            
            if (enableDebugLogs)
                UnityDebug.Log("[QuestManager] Initialized");
        }
        #endregion
        
        #region Event Subscription
        private void SubscribeToEvents()
        {
            // Subscribe to inventory events for automatic progress tracking
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged += OnInventoryChanged;
            }
            
            // Subscribe to save events
            SaveEvents.OnGameSaving += SaveQuestData;
            SaveEvents.OnGameLoading += LoadQuestData;
        }
        
        private void UnsubscribeFromEvents()
        {
            // Unsubscribe from inventory events
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged -= OnInventoryChanged;
            }
            
            // Unsubscribe from save events
            SaveEvents.OnGameSaving -= SaveQuestData;
            SaveEvents.OnGameLoading -= LoadQuestData;
        }
        
        /// <summary>
        /// Callback quando inventário muda - verifica se algum item coletado pertence a quest ativa.
        /// </summary>
        private void OnInventoryChanged()
        {
            // Verifica todas as quests ativas para atualizar progresso
            foreach (QuestProgress progress in activeQuests)
            {
                if (progress.isReadyToTurnIn)
                    continue; // Já está completa
                
                // Conta quantos itens da quest o jogador tem no inventário
                int itemCount = CountItemInInventory(progress.questData.itemToCollect);
                
                // Atualiza progresso se mudou
                if (itemCount != progress.currentProgress)
                {
                    UpdateQuestProgress(progress.questID, itemCount);
                }
            }
        }
        #endregion
        
        #region Public Methods
        /// <summary>
        /// Aceita uma quest e adiciona à lista de quests ativas.
        /// </summary>
        /// <param name="quest">Quest a ser aceita</param>
        /// <returns>True se quest foi aceita com sucesso</returns>
        public bool AcceptQuest(CollectQuestData quest)
        {
            if (quest == null)
            {
                UnityDebug.LogError("[QuestManager] Tentativa de aceitar quest nula.");
                return false;
            }
            
            // Valida se pode aceitar a quest
            if (!CanAcceptQuest(quest))
            {
                if (enableDebugLogs)
                    UnityDebug.Log($"[QuestManager] Não é possível aceitar quest '{quest.questName}'.");
                return false;
            }
            
            // Cria progresso da quest
            QuestProgress progress = new QuestProgress(quest);
            
            // Adiciona à lista ativa e cache
            activeQuests.Add(progress);
            activeQuestsDict[quest.questID] = progress;
            
            // Dispara evento
            QuestEvents.QuestAccepted(quest);
            
            if (enableDebugLogs)
                UnityDebug.Log($"[QuestManager] Quest aceita: '{quest.questName}' (ID: {quest.questID})");
            
            return true;
        }
        
        /// <summary>
        /// Verifica se uma quest pode ser aceita pelo jogador.
        /// Valida requisitos, duplicatas e se já foi completada.
        /// </summary>
        /// <param name="quest">Quest a verificar</param>
        /// <returns>True se quest pode ser aceita</returns>
        public bool CanAcceptQuest(CollectQuestData quest)
        {
            if (quest == null)
                return false;
            
            // Verifica se quest já está ativa
            if (IsQuestActive(quest.questID))
            {
                if (enableDebugLogs)
                    UnityDebug.Log($"[QuestManager] Quest '{quest.questName}' já está ativa.");
                return false;
            }
            
            // Verifica se quest já foi completada (e não é repetível)
            if (!quest.isRepeatable && IsQuestCompleted(quest.questID))
            {
                if (enableDebugLogs)
                    UnityDebug.Log($"[QuestManager] Quest '{quest.questName}' já foi completada e não é repetível.");
                return false;
            }
            
            // Verifica requisitos de reputação
            if (GameManager.HasInstance && GameManager.Instance.GetReputation() < quest.minimumReputation)
            {
                if (enableDebugLogs)
                    UnityDebug.Log($"[QuestManager] Reputação insuficiente para quest '{quest.questName}'. Necessário: {quest.minimumReputation}, Atual: {GameManager.Instance.GetReputation()}");
                return false;
            }
            
            // Verifica quests prerequisite
            if (quest.prerequisiteQuests != null && quest.prerequisiteQuests.Count > 0)
            {
                foreach (CollectQuestData prerequisite in quest.prerequisiteQuests)
                {
                    if (prerequisite != null && !IsQuestCompleted(prerequisite.questID))
                    {
                        if (enableDebugLogs)
                            UnityDebug.Log($"[QuestManager] Quest prerequisite '{prerequisite.questName}' não completada.");
                        return false;
                    }
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Verifica se uma quest está ativa.
        /// </summary>
        /// <param name="questID">ID da quest</param>
        /// <returns>True se quest está ativa</returns>
        public bool IsQuestActive(string questID)
        {
            return activeQuestsDict.ContainsKey(questID);
        }
        
        /// <summary>
        /// Verifica se uma quest foi completada.
        /// </summary>
        /// <param name="questID">ID da quest</param>
        /// <returns>True se quest foi completada</returns>
        public bool IsQuestCompleted(string questID)
        {
            return completedQuestIDs.Contains(questID);
        }
        
        /// <summary>
        /// Obtém o progresso de uma quest ativa.
        /// </summary>
        /// <param name="questID">ID da quest</param>
        /// <returns>Progresso da quest ou null se não encontrada</returns>
        public QuestProgress GetQuestProgress(string questID)
        {
            if (activeQuestsDict.TryGetValue(questID, out QuestProgress progress))
            {
                return progress;
            }
            
            return null;
        }
        
        /// <summary>
        /// Obtém lista de todas as quests ativas.
        /// </summary>
        /// <returns>Lista de quests ativas</returns>
        public List<QuestProgress> GetActiveQuests()
        {
            return activeQuests;
        }
        
        /// <summary>
        /// Registra um Quest Giver no cache do manager.
        /// </summary>
        /// <param name="giver">Quest Giver a registrar</param>
        public void RegisterQuestGiver(QuestGiverController giver)
        {
            if (giver == null)
                return;
            
            string giverID = giver.gameObject.GetInstanceID().ToString();
            
            if (!questGiverCache.ContainsKey(giverID))
            {
                questGiverCache[giverID] = giver;
                
                if (enableDebugLogs)
                    UnityDebug.Log($"[QuestManager] Quest Giver registrado: {giver.gameObject.name}");
            }
        }
        
        /// <summary>
        /// Verifica se uma quest está pronta para ser entregue.
        /// </summary>
        /// <param name="questID">ID da quest</param>
        /// <returns>True se quest está pronta para entrega</returns>
        public bool IsQuestReadyToTurnIn(string questID)
        {
            QuestProgress progress = GetQuestProgress(questID);
            return progress != null && progress.isReadyToTurnIn;
        }
        
        /// <summary>
        /// Entrega uma quest completada, remove itens e dá recompensas.
        /// </summary>
        /// <param name="questID">ID da quest a entregar</param>
        /// <returns>True se quest foi entregue com sucesso</returns>
        public bool TurnInQuest(string questID)
        {
            QuestProgress progress = GetQuestProgress(questID);
            
            if (progress == null)
            {
                UnityDebug.LogError($"[QuestManager] Quest '{questID}' não encontrada.");
                return false;
            }
            
            if (!progress.isReadyToTurnIn)
            {
                UnityDebug.LogWarning($"[QuestManager] Quest '{progress.questData.questName}' não está pronta para entrega.");
                return false;
            }
            
            CollectQuestData questData = progress.questData;
            
            // Remove itens coletados do inventário
            if (InventoryManager.Instance != null)
            {
                bool removed = InventoryManager.Instance.RemoveItem(questData.itemToCollect, questData.quantityRequired);
                
                if (!removed)
                {
                    UnityDebug.LogError($"[QuestManager] Falha ao remover itens da quest '{questData.questName}'.");
                    return false;
                }
            }
            
            // Dá recompensas
            GiveRewards(questData);
            
            // Remove da lista ativa
            activeQuests.Remove(progress);
            activeQuestsDict.Remove(questID);
            
            // Adiciona à lista de completadas
            if (!completedQuestIDs.Contains(questID))
            {
                completedQuestIDs.Add(questID);
            }
            
            // Dispara eventos
            QuestEvents.QuestCompleted(questData, questData.itemRewards);
            QuestEvents.QuestTurnedIn(questID);
            
            if (enableDebugLogs)
                UnityDebug.Log($"[QuestManager] Quest entregue: '{questData.questName}'");
            
            return true;
        }
        #endregion
        
        #region Private Methods
        /// <summary>
        /// Callback quando item é coletado - atualiza progresso de quests relacionadas.
        /// </summary>
        /// <param name="item">Item coletado</param>
        /// <param name="quantity">Quantidade coletada</param>
        private void OnItemCollected(ItemData item, int quantity)
        {
            if (item == null)
                return;
            
            // Verifica todas as quests ativas
            foreach (QuestProgress progress in activeQuests)
            {
                // Verifica se item pertence a esta quest
                if (progress.questData.itemToCollect == item && !progress.isReadyToTurnIn)
                {
                    // Conta total de itens no inventário
                    int totalItems = CountItemInInventory(item);
                    
                    // Atualiza progresso
                    UpdateQuestProgress(progress.questID, totalItems);
                }
            }
        }
        
        /// <summary>
        /// Atualiza o progresso de uma quest específica.
        /// </summary>
        /// <param name="questID">ID da quest</param>
        /// <param name="newProgress">Novo valor de progresso</param>
        private void UpdateQuestProgress(string questID, int newProgress)
        {
            QuestProgress progress = GetQuestProgress(questID);
            
            if (progress == null)
            {
                UnityDebug.LogWarning($"[QuestManager] Quest '{questID}' não encontrada para atualizar progresso.");
                return;
            }
            
            // Garante que não ultrapassa o target
            int clampedProgress = Mathf.Min(newProgress, progress.targetProgress);
            
            // Só atualiza se mudou
            if (progress.currentProgress == clampedProgress)
                return;
            
            progress.currentProgress = clampedProgress;
            
            // Dispara evento de progresso
            QuestEvents.QuestProgressChanged(questID, progress.currentProgress, progress.targetProgress);
            
            if (enableDebugLogs)
                UnityDebug.Log($"[QuestManager] Quest '{progress.questData.questName}' progresso: {progress.currentProgress}/{progress.targetProgress}");
            
            // Verifica se completou
            CheckQuestCompletion(progress);
        }
        
        /// <summary>
        /// Verifica se uma quest foi completada e marca como pronta para entrega.
        /// </summary>
        /// <param name="progress">Progresso da quest</param>
        private void CheckQuestCompletion(QuestProgress progress)
        {
            if (progress == null)
                return;
            
            // Verifica se completou e ainda não está marcada como pronta
            if (progress.IsComplete() && !progress.isReadyToTurnIn)
            {
                progress.isReadyToTurnIn = true;
                
                // Dispara evento
                QuestEvents.QuestReadyToTurnIn(progress.questID);
                
                if (enableDebugLogs)
                    UnityDebug.Log($"[QuestManager] Quest '{progress.questData.questName}' pronta para entregar!");
            }
        }
        
        /// <summary>
        /// Conta quantos itens de um tipo específico o jogador tem no inventário.
        /// </summary>
        /// <param name="item">Item a contar</param>
        /// <returns>Quantidade total no inventário</returns>
        private int CountItemInInventory(ItemData item)
        {
            if (item == null || InventoryManager.Instance == null)
                return 0;
            
            int totalCount = 0;
            InventorySlot[] slots = InventoryManager.Instance.GetAllSlots();
            
            foreach (InventorySlot slot in slots)
            {
                if (slot != null && slot.item == item)
                {
                    totalCount += slot.quantity;
                }
            }
            
            return totalCount;
        }
        
        /// <summary>
        /// Dá recompensas ao jogador ao completar quest.
        /// </summary>
        /// <param name="quest">Quest completada</param>
        private void GiveRewards(CollectQuestData quest)
        {
            if (quest == null)
                return;
            
            // Valida se há espaço no inventário para recompensas
            if (!ValidateInventorySpace(quest))
            {
                UnityDebug.LogError($"[QuestManager] Inventário cheio! Não é possível entregar quest '{quest.questName}' sem espaço para recompensas.");
                return;
            }
            
            // Adiciona recompensas de itens ao inventário
            if (quest.itemRewards != null && quest.itemRewards.Count > 0)
            {
                foreach (ItemReward reward in quest.itemRewards)
                {
                    if (reward.item != null && InventoryManager.Instance != null)
                    {
                        bool added = InventoryManager.Instance.AddItem(reward.item, reward.quantity);
                        
                        if (added)
                        {
                            if (enableDebugLogs)
                                UnityDebug.Log($"[QuestManager] Recompensa recebida: {reward.item.itemName} x{reward.quantity}");
                        }
                        else
                        {
                            UnityDebug.LogWarning($"[QuestManager] Inventário cheio! Não foi possível adicionar recompensa: {reward.item.itemName}");
                        }
                    }
                }
            }
            
            // Adiciona reputação
            if (quest.reputationReward > 0)
            {
                if (GameManager.HasInstance)
                {
                    GameManager.Instance.AddReputation(quest.reputationReward);
                    
                    if (enableDebugLogs)
                        UnityDebug.Log($"[QuestManager] Reputação recebida: +{quest.reputationReward}");
                }
                else
                {
                    UnityDebug.LogWarning("[QuestManager] GameManager não encontrado para adicionar reputação.");
                }
            }
        }
        
        /// <summary>
        /// Valida se um questID é único e não está duplicado.
        /// </summary>
        /// <param name="questID">ID da quest a validar</param>
        /// <returns>True se ID é válido e único</returns>
        private bool ValidateQuestID(string questID)
        {
            if (string.IsNullOrEmpty(questID))
            {
                UnityDebug.LogError("[QuestManager] Quest ID não pode ser nulo ou vazio.");
                return false;
            }
            
            // Verifica duplicatas em quests ativas
            if (activeQuestsDict.ContainsKey(questID))
            {
                if (enableDebugLogs)
                    UnityDebug.Log($"[QuestManager] Quest ID '{questID}' já está ativa.");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Encontra QuestData por ID (com cache para performance).
        /// </summary>
        /// <param name="questID">ID da quest</param>
        /// <returns>CollectQuestData ou null se não encontrada</returns>
        private CollectQuestData FindQuestDataByID(string questID)
        {
            if (string.IsNullOrEmpty(questID))
                return null;
            
            // Primeiro verifica no cache de quests ativas
            if (activeQuestsDict.TryGetValue(questID, out QuestProgress progress))
            {
                return progress.questData;
            }
            
            // Se não encontrou, procura em todos os Quest Givers registrados
            foreach (var giver in questGiverCache.Values)
            {
                if (giver == null)
                    continue;
                
                var availableQuests = giver.GetAvailableQuests();
                if (availableQuests != null)
                {
                    foreach (var quest in availableQuests)
                    {
                        if (quest != null && quest.questID == questID)
                        {
                            return quest;
                        }
                    }
                }
            }
            
            if (enableDebugLogs)
                UnityDebug.LogWarning($"[QuestManager] Quest com ID '{questID}' não encontrada.");
            
            return null;
        }
        
        /// <summary>
        /// Valida se há espaço no inventário para as recompensas da quest.
        /// </summary>
        /// <param name="quest">Quest a validar</param>
        /// <returns>True se há espaço suficiente</returns>
        private bool ValidateInventorySpace(CollectQuestData quest)
        {
            if (quest == null || InventoryManager.Instance == null)
                return false;
            
            // Se não há recompensas de itens, não precisa validar
            if (quest.itemRewards == null || quest.itemRewards.Count == 0)
                return true;
            
            // Conta quantos slots vazios são necessários
            int slotsNeeded = 0;
            
            foreach (ItemReward reward in quest.itemRewards)
            {
                if (reward.item == null)
                    continue;
                
                // Sistema não empilhável - cada item precisa de um slot
                {
                    // Item não stackable precisa de slot próprio
                    slotsNeeded++;
                }
            }
            
            // Conta slots vazios disponíveis
            int emptySlots = 0;
            InventorySlot[] allSlots = InventoryManager.Instance.GetAllSlots();
            
            foreach (InventorySlot slot in allSlots)
            {
                if (slot != null && slot.IsEmpty)
                {
                    emptySlots++;
                }
            }
            
            bool hasSpace = emptySlots >= slotsNeeded;
            
            if (!hasSpace && enableDebugLogs)
            {
                UnityDebug.LogWarning($"[QuestManager] Inventário não tem espaço suficiente. Necessário: {slotsNeeded}, Disponível: {emptySlots}");
            }
            
            return hasSpace;
        }
        #endregion
        
        #region Save/Load
        /// <summary>
        /// Serializa dados de quests para salvamento.
        /// Chamado automaticamente quando SaveEvents.OnGameSaving é disparado.
        /// </summary>
        /// <returns>Dados serializados de quests</returns>
        private object SaveQuestData()
        {
            QuestSaveData saveData = new QuestSaveData();
            
            // Serializa quests ativas com progresso
            foreach (QuestProgress progress in activeQuests)
            {
                if (progress == null || progress.questData == null)
                    continue;
                
                QuestProgressData progressData = new QuestProgressData
                {
                    questID = progress.questID,
                    currentProgress = progress.currentProgress,
                    isReadyToTurnIn = progress.isReadyToTurnIn
                };
                
                saveData.activeQuests.Add(progressData);
            }
            
            // Serializa lista de quests completadas
            saveData.completedQuestIDs = new List<string>(completedQuestIDs);
            
            if (enableDebugLogs)
            {
                UnityDebug.Log($"[QuestManager] Salvando dados: {saveData.activeQuests.Count} quests ativas, {saveData.completedQuestIDs.Count} completadas");
            }
            
            return saveData;
        }
        
        /// <summary>
        /// Deserializa e restaura dados de quests do salvamento.
        /// Chamado automaticamente quando SaveEvents.OnGameLoading é disparado.
        /// </summary>
        /// <param name="data">Dados a serem carregados</param>
        private void LoadQuestData(object data)
        {
            if (data == null)
            {
                if (enableDebugLogs)
                    UnityDebug.Log("[QuestManager] Nenhum dado de quest para carregar.");
                return;
            }
            
            // Tenta fazer cast para QuestSaveData
            if (!(data is QuestSaveData saveData))
            {
                UnityDebug.LogWarning("[QuestManager] Dados de save não são do tipo QuestSaveData.");
                return;
            }
            
            // Limpa dados atuais
            activeQuests.Clear();
            activeQuestsDict.Clear();
            completedQuestIDs.Clear();
            
            // Restaura quests completadas
            if (saveData.completedQuestIDs != null)
            {
                completedQuestIDs = new List<string>(saveData.completedQuestIDs);
            }
            
            // Restaura quests ativas com progresso
            if (saveData.activeQuests != null)
            {
                foreach (QuestProgressData progressData in saveData.activeQuests)
                {
                    if (progressData == null || string.IsNullOrEmpty(progressData.questID))
                        continue;
                    
                    // Busca QuestData original por ID
                    CollectQuestData questData = FindQuestDataByID(progressData.questID);
                    
                    if (questData == null)
                    {
                        UnityDebug.LogWarning($"[QuestManager] Quest com ID '{progressData.questID}' não encontrada ao carregar save. Pulando...");
                        continue;
                    }
                    
                    // Cria progresso restaurado
                    QuestProgress progress = new QuestProgress(questData)
                    {
                        currentProgress = progressData.currentProgress,
                        isReadyToTurnIn = progressData.isReadyToTurnIn
                    };
                    
                    // Adiciona à lista ativa e cache
                    activeQuests.Add(progress);
                    activeQuestsDict[progressData.questID] = progress;
                }
            }
            
            if (enableDebugLogs)
            {
                UnityDebug.Log($"[QuestManager] Dados carregados: {activeQuests.Count} quests ativas, {completedQuestIDs.Count} completadas");
            }
        }
        #endregion
        
        #region Debug Methods
        /// <summary>
        /// Força uma quest a ser completada instantaneamente (apenas para debug).
        /// </summary>
        /// <param name="questID">ID da quest a completar</param>
        public void ForceCompleteQuest(string questID)
        {
            QuestProgress progress = GetQuestProgress(questID);
            
            if (progress == null)
            {
                UnityDebug.LogWarning($"[QuestManager] Quest '{questID}' não encontrada para forçar completar.");
                return;
            }
            
            // Define progresso como completo
            progress.currentProgress = progress.targetProgress;
            progress.isReadyToTurnIn = true;
            
            // Dispara eventos
            QuestEvents.QuestProgressChanged(questID, progress.currentProgress, progress.targetProgress);
            QuestEvents.QuestReadyToTurnIn(questID);
            
            if (enableDebugLogs)
                UnityDebug.Log($"[QuestManager] [DEBUG] Quest '{progress.questData.questName}' forçada a completar.");
        }
        
        /// <summary>
        /// Reseta o progresso de uma quest ativa (apenas para debug).
        /// </summary>
        /// <param name="questID">ID da quest a resetar</param>
        public void ResetQuest(string questID)
        {
            QuestProgress progress = GetQuestProgress(questID);
            
            if (progress == null)
            {
                UnityDebug.LogWarning($"[QuestManager] Quest '{questID}' não encontrada para resetar.");
                return;
            }
            
            // Reseta progresso
            progress.currentProgress = 0;
            progress.isReadyToTurnIn = false;
            
            // Dispara evento
            QuestEvents.QuestProgressChanged(questID, progress.currentProgress, progress.targetProgress);
            
            if (enableDebugLogs)
                UnityDebug.Log($"[QuestManager] [DEBUG] Quest '{progress.questData.questName}' resetada.");
        }
        
        /// <summary>
        /// Limpa todas as quests ativas e completadas (apenas para debug).
        /// </summary>
        public void ClearAllQuests()
        {
            int activeCount = activeQuests.Count;
            int completedCount = completedQuestIDs.Count;
            
            // Limpa todas as listas
            activeQuests.Clear();
            activeQuestsDict.Clear();
            completedQuestIDs.Clear();
            
            if (enableDebugLogs)
                UnityDebug.Log($"[QuestManager] [DEBUG] Todas as quests limpas. ({activeCount} ativas, {completedCount} completadas)");
        }
        
        /// <summary>
        /// Loga o estado atual de todas as quests no console (apenas para debug).
        /// </summary>
        public void DebugLogQuestState()
        {
            UnityDebug.Log("=== QUEST MANAGER STATE ===");
            UnityDebug.Log($"Active Quests: {activeQuests.Count}");
            
            foreach (QuestProgress progress in activeQuests)
            {
                if (progress == null || progress.questData == null)
                    continue;
                
                string status = progress.isReadyToTurnIn ? "READY" : "IN PROGRESS";
                UnityDebug.Log($"  - {progress.questData.questName} ({progress.questID}): {progress.currentProgress}/{progress.targetProgress} [{status}]");
            }
            
            UnityDebug.Log($"Completed Quests: {completedQuestIDs.Count}");
            foreach (string questID in completedQuestIDs)
            {
                UnityDebug.Log($"  - {questID}");
            }
            
            UnityDebug.Log("===========================");
        }
        #endregion
        
        #region Gizmos
        private void OnDrawGizmos()
        {
            if (!showGizmos) return;
            
            // Desenhar informações de debug
            if (activeQuests != null && activeQuests.Count > 0)
            {
                Gizmos.color = Color.yellow;
                // TODO: Add visual debug information when needed
            }
        }
        #endregion
    }
}
