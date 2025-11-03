using System;

namespace TheSlimeKing.Inventory
{
    /// <summary>
    /// Estrutura de dados para salvar o estado completo do inventário.
    /// Contém todos os itens, equipamentos e quick slots.
    /// </summary>
    [Serializable]
    public class InventorySaveData
    {
        /// <summary>
        /// Array de todos os itens salvos no inventário (apenas slots não-vazios).
        /// </summary>
        public ItemSaveData[] items;

        /// <summary>
        /// IDs dos equipamentos nos 3 slots (Amulet, Ring, Cape).
        /// Null ou string vazia indica slot vazio.
        /// </summary>
        public string[] equipmentIDs = new string[3];

        /// <summary>
        /// IDs dos itens atribuídos aos 4 quick slots (Up, Down, Left, Right).
        /// Null ou string vazia indica slot vazio.
        /// </summary>
        public string[] quickSlotIDs = new string[4];
    }

    /// <summary>
    /// Estrutura de dados para salvar um item individual do inventário.
    /// </summary>
    [Serializable]
    public class ItemSaveData
    {
        /// <summary>
        /// ID único do item (nome do ScriptableObject).
        /// </summary>
        public string itemID;

        /// <summary>
        /// Quantidade do item no slot.
        /// </summary>
        public int quantity;

        /// <summary>
        /// Índice do slot no inventário (0-19).
        /// </summary>
        public int slotIndex;
    }
}
