using UnityEngine;
using UnityEditor;
using TheSlimeKing.Inventory;
using System.IO;

namespace TheSlimeKing.Editor
{
    /// <summary>
    /// Editor utility to create example ItemData ScriptableObjects.
    /// Menu: Tools/The Slime King/Create Example Items
    /// </summary>
    public class CreateExampleItems
    {
        [MenuItem("Tools/The Slime King/Create Example Items")]
        public static void CreateItems()
        {
            string folderPath = "Assets/Resources/Items";

            // Ensure the folder exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }

            // 1. Poção de Cura (Consumable)
            CreateConsumableItem(
                folderPath,
                "PocaoDeCura",
                "Poção de Cura",
                healAmount: 30
            );

            // 2. Cristal Mágico (Material)
            CreateMaterialItem(
                folderPath,
                "CristalMagico",
                "Cristal Mágico"
            );

            // 3. Chave Antiga (Quest Item)
            CreateQuestItem(
                folderPath,
                "ChaveAntiga",
                "Chave Antiga"
            );

            // 4. Amuleto de Proteção (Equipment - Amulet)
            CreateEquipmentItem(
                folderPath,
                "AmuletoDeProtecao",
                "Amuleto de Proteção",
                EquipmentType.Amulet,
                defenseBonus: 15,
                speedBonus: 0
            );

            // 5. Anel de Velocidade (Equipment - Ring)
            CreateEquipmentItem(
                folderPath,
                "AnelDeVelocidade",
                "Anel de Velocidade",
                EquipmentType.Ring,
                defenseBonus: 0,
                speedBonus: 2
            );

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("✓ 5 example items created successfully in Assets/Resources/Items/");
            EditorUtility.DisplayDialog(
                "Success",
                "5 example items created successfully!\n\n" +
                "- Poção de Cura (Consumable)\n" +
                "- Cristal Mágico (Material)\n" +
                "- Chave Antiga (Quest Item)\n" +
                "- Amuleto de Proteção (Equipment)\n" +
                "- Anel de Velocidade (Equipment)\n\n" +
                "You can now assign icons to these items in the Inspector.",
                "OK"
            );
        }

        private static void CreateConsumableItem(string folderPath, string fileName, string itemName, int healAmount)
        {
            string assetPath = $"{folderPath}/{fileName}.asset";

            // Check if already exists
            ItemData existingItem = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            if (existingItem != null)
            {
                Debug.LogWarning($"Item '{fileName}' already exists. Skipping.");
                return;
            }

            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            item.itemName = itemName;
            item.type = ItemType.Consumable;
            item.isStackable = true;
            item.healAmount = healAmount;
            item.isQuestItem = false;

            AssetDatabase.CreateAsset(item, assetPath);
            Debug.Log($"Created: {assetPath}");
        }

        private static void CreateMaterialItem(string folderPath, string fileName, string itemName)
        {
            string assetPath = $"{folderPath}/{fileName}.asset";

            ItemData existingItem = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            if (existingItem != null)
            {
                Debug.LogWarning($"Item '{fileName}' already exists. Skipping.");
                return;
            }

            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            item.itemName = itemName;
            item.type = ItemType.Material;
            item.isStackable = true;
            item.isQuestItem = false;

            AssetDatabase.CreateAsset(item, assetPath);
            Debug.Log($"Created: {assetPath}");
        }

        private static void CreateQuestItem(string folderPath, string fileName, string itemName)
        {
            string assetPath = $"{folderPath}/{fileName}.asset";

            ItemData existingItem = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            if (existingItem != null)
            {
                Debug.LogWarning($"Item '{fileName}' already exists. Skipping.");
                return;
            }

            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            item.itemName = itemName;
            item.type = ItemType.Quest;
            item.isStackable = false;
            item.isQuestItem = true;

            AssetDatabase.CreateAsset(item, assetPath);
            Debug.Log($"Created: {assetPath}");
        }

        private static void CreateEquipmentItem(string folderPath, string fileName, string itemName, EquipmentType equipmentType, int defenseBonus, int speedBonus)
        {
            string assetPath = $"{folderPath}/{fileName}.asset";

            ItemData existingItem = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            if (existingItem != null)
            {
                Debug.LogWarning($"Item '{fileName}' already exists. Skipping.");
                return;
            }

            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            item.itemName = itemName;
            item.type = ItemType.Equipment;
            item.isStackable = false;
            item.equipmentType = equipmentType;
            item.defenseBonus = defenseBonus;
            item.speedBonus = speedBonus;
            item.isQuestItem = false;

            AssetDatabase.CreateAsset(item, assetPath);
            Debug.Log($"Created: {assetPath}");
        }
    }
}
