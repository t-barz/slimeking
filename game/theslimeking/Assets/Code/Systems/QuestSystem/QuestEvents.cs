using System;
using System.Collections.Generic;

namespace TheSlimeKing.Quest
{
    /// <summary>
    /// Classe estática centralizada para eventos do Quest System.
    /// Permite comunicação desacoplada entre componentes do sistema de quests.
    /// </summary>
    public static class QuestEvents
    {
        #region Quest Lifecycle Events
        
        /// <summary>
        /// Disparado quando uma quest é aceita pelo jogador.
        /// Quando disparar: Após QuestManager.AcceptQuest() adicionar quest à lista ativa.
        /// </summary>
        public static event Action<CollectQuestData> OnQuestAccepted;
        
        /// <summary>
        /// Disparado quando o progresso de uma quest muda.
        /// Quando disparar: Após QuestManager.UpdateQuestProgress() atualizar contador.
        /// Parâmetros: questID, currentProgress, targetProgress
        /// </summary>
        public static event Action<string, int, int> OnQuestProgressChanged;
        
        /// <summary>
        /// Disparado quando uma quest está pronta para ser entregue.
        /// Quando disparar: Após QuestManager.CheckQuestCompletion() marcar quest como completa.
        /// Parâmetro: questID
        /// </summary>
        public static event Action<string> OnQuestReadyToTurnIn;
        
        /// <summary>
        /// Disparado quando uma quest é completada e recompensas são dadas.
        /// Quando disparar: Após QuestManager.TurnInQuest() dar recompensas ao jogador.
        /// Parâmetros: quest data, lista de recompensas
        /// </summary>
        public static event Action<CollectQuestData, List<ItemReward>> OnQuestCompleted;
        
        /// <summary>
        /// Disparado quando uma quest é entregue ao NPC.
        /// Quando disparar: Após QuestManager.TurnInQuest() mover quest para lista de completadas.
        /// Parâmetro: questID
        /// </summary>
        public static event Action<string> OnQuestTurnedIn;
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Dispara evento OnQuestAccepted com null-conditional operator.
        /// </summary>
        /// <param name="quest">Dados da quest aceita</param>
        public static void QuestAccepted(CollectQuestData quest)
        {
            OnQuestAccepted?.Invoke(quest);
        }
        
        /// <summary>
        /// Dispara evento OnQuestProgressChanged com null-conditional operator.
        /// </summary>
        /// <param name="questID">ID da quest</param>
        /// <param name="current">Progresso atual</param>
        /// <param name="target">Progresso alvo</param>
        public static void QuestProgressChanged(string questID, int current, int target)
        {
            OnQuestProgressChanged?.Invoke(questID, current, target);
        }
        
        /// <summary>
        /// Dispara evento OnQuestReadyToTurnIn com null-conditional operator.
        /// </summary>
        /// <param name="questID">ID da quest pronta para entrega</param>
        public static void QuestReadyToTurnIn(string questID)
        {
            OnQuestReadyToTurnIn?.Invoke(questID);
        }
        
        /// <summary>
        /// Dispara evento OnQuestCompleted com null-conditional operator.
        /// </summary>
        /// <param name="quest">Dados da quest completada</param>
        /// <param name="rewards">Lista de recompensas recebidas</param>
        public static void QuestCompleted(CollectQuestData quest, List<ItemReward> rewards)
        {
            OnQuestCompleted?.Invoke(quest, rewards);
        }
        
        /// <summary>
        /// Dispara evento OnQuestTurnedIn com null-conditional operator.
        /// </summary>
        /// <param name="questID">ID da quest entregue</param>
        public static void QuestTurnedIn(string questID)
        {
            OnQuestTurnedIn?.Invoke(questID);
        }
        
        #endregion
    }
}
