using UnityEngine;
using SlimeKing.Core;
using SlimeKing.Items;
using System.Collections.Generic;

namespace SlimeKing.Core
{
    /// <summary>
    /// Gerencia o inventário do jogador.
    /// Responsável por adicionar, remover e consultar itens.
    /// </summary>
    public class InventoryManager : ManagerSingleton<InventoryManager>
    {
        [Header("Inventory Configuration")]
        [SerializeField] private int maxSlots = 16;
        [SerializeField] private bool enableDebugLogs = true;

        /// <summary>
        /// Quantidade total de slots disponíveis no inventário.
        /// </summary>
        public int MaxSlots => maxSlots;

        // Dicionário de itens: ItemData -> quantidade
        private Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();

        // Evento disparado quando inventário muda
        public event System.Action<ItemData, int> OnItemAdded;
        public event System.Action<ItemData, int> OnItemRemoved;
        public event System.Action OnInventoryFull;

        /// <summary>
        /// Inicialização do InventoryManager
        /// </summary>
        protected override void Initialize()
        {
            inventory = new Dictionary<ItemData, int>();
            Log("InventoryManager inicializado");
        }

        /// <summary>
        /// Tenta adicionar item ao inventário
        /// </summary>
        /// <param name="itemData">Dados do item a adicionar</param>
        /// <param name="quantity">Quantidade a adicionar</param>
        /// <returns>True se adicionado com sucesso, false se inventário cheio</returns>
        public bool AddItem(ItemData itemData, int quantity = 1)
        {
            if (itemData == null || quantity <= 0)
            {
                LogWarning($"Tentativa inválida de adicionar item: itemData={itemData}, quantity={quantity}");
                return false;
            }

            // Verifica se há espaço no inventário
            if (GetTotalSlots() + quantity > maxSlots)
            {
                Log($"Inventário cheio! Não é possível adicionar {itemData.GetLocalizedName()} (x{quantity})");
                OnInventoryFull?.Invoke();
                return false;
            }

            // Adiciona ou incrementa item existente
            if (inventory.ContainsKey(itemData))
            {
                inventory[itemData] += quantity;
            }
            else
            {
                inventory[itemData] = quantity;
            }

            Log($"Item adicionado: {itemData.GetLocalizedName()} (x{quantity})");
            OnItemAdded?.Invoke(itemData, inventory[itemData]);
            return true;
        }

        /// <summary>
        /// Remove item do inventário
        /// </summary>
        /// <param name="itemData">Dados do item a remover</param>
        /// <param name="quantity">Quantidade a remover</param>
        /// <returns>True se removido com sucesso, false se quantidade insuficiente</returns>
        public bool RemoveItem(ItemData itemData, int quantity = 1)
        {
            if (itemData == null || !inventory.ContainsKey(itemData))
            {
                LogWarning($"Item não encontrado no inventário: {itemData?.GetLocalizedName()}");
                return false;
            }

            int currentQuantity = inventory[itemData];
            if (currentQuantity < quantity)
            {
                LogWarning($"Quantidade insuficiente de {itemData.GetLocalizedName()}");
                return false;
            }

            int remainingQuantity = currentQuantity - quantity;
            if (remainingQuantity <= 0)
            {
                inventory.Remove(itemData);
                remainingQuantity = 0;
            }
            else
            {
                inventory[itemData] = remainingQuantity;
            }

            Log($"Item removido: {itemData.GetLocalizedName()} (x{quantity})");
            OnItemRemoved?.Invoke(itemData, remainingQuantity);
            return true;
        }

        /// <summary>
        /// Obtém a quantidade de um item específico
        /// </summary>
        public int GetItemCount(ItemData itemData)
        {
            return inventory.ContainsKey(itemData) ? inventory[itemData] : 0;
        }

        /// <summary>
        /// Obtém todos os itens do inventário
        /// </summary>
        public Dictionary<ItemData, int> GetAllItems()
        {
            return new Dictionary<ItemData, int>(inventory);
        }

        /// <summary>
        /// Obtém o total de slots utilizados
        /// </summary>
        public int GetTotalSlots()
        {
            int total = 0;
            foreach (var count in inventory.Values)
            {
                total += count;
            }
            return total;
        }

        /// <summary>
        /// Obtém slots vazios disponíveis
        /// </summary>
        public int GetAvailableSlots()
        {
            return maxSlots - GetTotalSlots();
        }

        /// <summary>
        /// Limpa todo o inventário
        /// </summary>
        public void Clear()
        {
            inventory.Clear();
            Log("Inventário limpo");
        }

        /// <summary>
        /// Verifica se inventário está cheio
        /// </summary>
        public bool IsFull()
        {
            return GetTotalSlots() >= maxSlots;
        }
    }
}
