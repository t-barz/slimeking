using UnityEngine;

namespace SlimeMec.Systems.Data
{
    /// <summary>
    /// ScriptableObject que define um item que pode ser dropado por um NPC ao ser derrotado.
    /// Contém informações sobre o item, probabilidade de drop e quantidade.
    /// </summary>
    [CreateAssetMenu(fileName = "NPCDropData", menuName = "Game/NPC/Drop Data")]
    public class NPCDropData : ScriptableObject
    {
        [Header("Item Configuration")]
        [Tooltip("Prefab do item que será instanciado quando dropado")]
        public GameObject itemPrefab;

        [Header("Drop Probability")]
        [Tooltip("Chance de drop em porcentagem (0-100)")]
        [Range(0f, 100f)]
        public float dropChance = 50f;

        [Tooltip("Se true, sempre dropa pelo menos um item independente da chance")]
        public bool guaranteedDrop = false;

        [Header("Quantity Configuration")]
        [Tooltip("Quantidade mínima de itens a dropar")]
        [Min(1)]
        public int minQuantity = 1;

        [Tooltip("Quantidade máxima de itens a dropar")]
        [Min(1)]
        public int maxQuantity = 1;

        [Header("Debug")]
        [Tooltip("Nome descritivo do drop para facilitar identificação")]
        public string dropName;

        /// <summary>
        /// Valida os dados do drop no Inspector.
        /// Garante que maxQuantity >= minQuantity e que itemPrefab está atribuído.
        /// </summary>
        private void OnValidate()
        {
            // Garantir que maxQuantity não seja menor que minQuantity
            if (maxQuantity < minQuantity)
            {
                maxQuantity = minQuantity;
            }

            // Validar se itemPrefab está atribuído
            if (itemPrefab == null)
            {
                Debug.LogWarning($"[NPCDropData] '{name}' não possui itemPrefab atribuído!", this);
            }

            // Validar dropChance
            if (dropChance < 0f)
            {
                dropChance = 0f;
            }
            else if (dropChance > 100f)
            {
                dropChance = 100f;
            }

            // Atualizar dropName se estiver vazio
            if (string.IsNullOrEmpty(dropName) && itemPrefab != null)
            {
                dropName = itemPrefab.name;
            }
        }

        /// <summary>
        /// Retorna uma quantidade aleatória de itens baseada em minQuantity e maxQuantity.
        /// </summary>
        public int GetRandomQuantity()
        {
            return Random.Range(minQuantity, maxQuantity + 1);
        }

        /// <summary>
        /// Verifica se o drop deve ocorrer baseado na dropChance.
        /// </summary>
        public bool ShouldDrop()
        {
            if (guaranteedDrop)
            {
                return true;
            }

            float roll = Random.Range(0f, 100f);
            return roll <= dropChance;
        }
    }
}
