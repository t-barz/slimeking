using UnityEngine;

namespace TheSlimeKing.Inventory
{
    /// <summary>
    /// Logger SUPER SIMPLES que mostra itens salvos ao carregar a cena
    /// </summary>
    public class SimpleItemLogger : MonoBehaviour
    {
        private void Start()
        {
            LogAllItemsInScene();
        }

        private void LogAllItemsInScene()
        {
            Debug.Log("═══════════════════════════════════════");
            Debug.Log("📦 ITENS SALVOS NA CENA");
            Debug.Log("═══════════════════════════════════════");

            // Encontra todos os ItemPickup na cena
            var allItems = FindObjectsOfType<SlimeKing.External.SlimeMec.Gameplay.ItemPickup>();
            
            int totalCollected = 0;
            int totalItems = allItems.Length;

            foreach (var item in allItems)
            {
                string itemID = item.gameObject.name;
                int wasCollected = PlayerPrefs.GetInt($"Item_{itemID}", 0);

                if (wasCollected == 1)
                {
                    totalCollected++;
                    Debug.Log($"✅ {itemID}: Coletado");
                }
                else
                {
                    Debug.Log($"❌ {itemID}: Não coletado");
                }
            }

            Debug.Log("───────────────────────────────────────");
            Debug.Log($"📊 Total: {totalCollected}/{totalItems} coletados");
            Debug.Log("═══════════════════════════════════════");
        }

        [ContextMenu("Show Items Status")]
        public void ShowItemsStatus()
        {
            LogAllItemsInScene();
        }
    }
}
