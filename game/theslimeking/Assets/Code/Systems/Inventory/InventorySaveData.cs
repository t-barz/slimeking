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
        /// Índices dos slots do inventário atribuídos aos 4 quick slots (Up, Down, Left, Right).
        /// -1 indica slot vazio.
        /// Mudança: agora armazena índice do slot ao invés de ItemData ID para suportar múltiplos itens do mesmo tipo.
        /// </summary>
        public int[] quickSlotIndices = new int[4] { -1, -1, -1, -1 };
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

        /// <summary>
        /// ID único da instância (6 dígitos: 100000-999999).
        /// Cada item tem um instanceID único, mesmo se forem do mesmo tipo.
        /// </summary>
        public int instanceID;
    }
}
