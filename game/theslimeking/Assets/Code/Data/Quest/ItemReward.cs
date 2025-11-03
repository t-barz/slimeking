using UnityEngine;
using TheSlimeKing.Inventory;

namespace TheSlimeKing.Quest
{
    /// <summary>
    /// Representa uma recompensa de item para quests
    /// </summary>
    [System.Serializable]
    public class ItemReward
    {
        public ItemData item;
        public int quantity = 1;
    }
}
