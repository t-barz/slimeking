using UnityEngine;
using System.Collections.Generic;
using TheSlimeKing.Inventory;

namespace TheSlimeKing.Quest
{
    /// <summary>
    /// ScriptableObject que define dados de uma quest de coleta
    /// </summary>
    [CreateAssetMenu(fileName = "CollectQuest", menuName = "Quest System/Collect Quest")]
    public class CollectQuestData : ScriptableObject
    {
        #region Quest Info
        [Header("Quest Info")]
        public string questID;
        public string questName;
        [TextArea(3, 6)]
        public string description;
        #endregion
        
        #region Objective
        [Header("Objective")]
        public ItemData itemToCollect;
        public int quantityRequired = 1;
        #endregion
        
        #region Rewards
        [Header("Rewards")]
        public List<ItemReward> itemRewards = new List<ItemReward>();
        public int reputationReward = 10;
        #endregion
        
        #region Requirements
        [Header("Requirements")]
        public int minimumReputation = 0;
        public List<CollectQuestData> prerequisiteQuests = new List<CollectQuestData>();
        #endregion
        
        #region Settings
        [Header("Settings")]
        public bool isRepeatable = false;
        #endregion
        
        #region Validation
        private void OnValidate()
        {
            // Gera ID único se não fornecido
            if (string.IsNullOrEmpty(questID))
            {
                questID = $"quest_{name}_{GetInstanceID()}";
            }
            
            // Valida item to collect
            if (itemToCollect == null)
            {
                Debug.LogWarning($"[Quest: {questName}] Item to collect não configurado!");
            }
            
            // Valida quantidade
            if (quantityRequired <= 0)
            {
                Debug.LogWarning($"[Quest: {questName}] Quantity required deve ser maior que 0!");
                quantityRequired = 1;
            }
        }
        #endregion
    }
}
