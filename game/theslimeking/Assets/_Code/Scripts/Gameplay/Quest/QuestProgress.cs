using UnityEngine;

namespace TheSlimeKing.Quest
{
    /// <summary>
    /// Classe que rastreia o progresso de uma quest ativa
    /// </summary>
    [System.Serializable]
    public class QuestProgress
    {
        #region Public Fields
        public string questID;
        public CollectQuestData questData;
        public int currentProgress;
        public int targetProgress;
        public bool isReadyToTurnIn;
        #endregion
        
        #region Constructor
        public QuestProgress(CollectQuestData data)
        {
            questID = data.questID;
            questData = data;
            currentProgress = 0;
            targetProgress = data.quantityRequired;
            isReadyToTurnIn = false;
        }
        #endregion
        
        #region Public Methods
        /// <summary>
        /// Calcula a porcentagem de progresso da quest
        /// </summary>
        public float GetProgressPercentage()
        {
            if (targetProgress == 0) return 0f;
            return (float)currentProgress / targetProgress;
        }
        
        /// <summary>
        /// Verifica se a quest está completa
        /// </summary>
        public bool IsComplete()
        {
            return currentProgress >= targetProgress;
        }
        #endregion
    }
}
