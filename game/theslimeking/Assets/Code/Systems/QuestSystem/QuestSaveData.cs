using System.Collections.Generic;

namespace TheSlimeKing.Quest
{
    /// <summary>
    /// Dados de save para o Quest System
    /// </summary>
    [System.Serializable]
    public class QuestSaveData
    {
        public List<QuestProgressData> activeQuests = new List<QuestProgressData>();
        public List<string> completedQuestIDs = new List<string>();
    }

    /// <summary>
    /// Dados serializáveis de progresso de quest
    /// </summary>
    [System.Serializable]
    public class QuestProgressData
    {
        public string questID;
        public int currentProgress;
        public bool isReadyToTurnIn;
    }
}
