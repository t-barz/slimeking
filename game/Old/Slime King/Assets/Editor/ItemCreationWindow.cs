using UnityEngine;
using UnityEditor;

namespace SlimeKing.Core.Inventory.Editor
{
    public class ItemCreationWindow : EditorWindow
    {
        private string itemName = "";
        private string description = "";
        private Sprite icon;
        private bool isStackable;
        private int maxStackSize = 1;
        private ItemType itemType;
        private string savePath = "Assets/Resources/Items";

        [MenuItem("SlimeKing/Inventory/Create New Item")]
        public static void ShowWindow()
        {
            GetWindow<ItemCreationWindow>("Create Item");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create New Item", EditorStyles.boldLabel);

            itemName = EditorGUILayout.TextField("Item Name", itemName);
            description = EditorGUILayout.TextField("Description", description);
            icon = (Sprite)EditorGUILayout.ObjectField("Icon", icon, typeof(Sprite), false);

            isStackable = EditorGUILayout.Toggle("Is Stackable", isStackable);
            if (isStackable)
            {
                maxStackSize = EditorGUILayout.IntField("Max Stack Size", maxStackSize);
            }

            itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", itemType);

            savePath = EditorGUILayout.TextField("Save Path", savePath);

            if (GUILayout.Button("Create Item"))
            {
                CreateItem();
            }
        }

        private void CreateItem()
        {
            if (string.IsNullOrEmpty(itemName))
            {
                EditorUtility.DisplayDialog("Error", "Item name cannot be empty", "OK");
                return;
            }

            // Create the item asset
            var item = CreateInstance<ItemData>(); item.ItemName = itemName;
            item.Description = description;
            item.Icon = icon;
            item.IsStackable = isStackable;
            item.MaxStackSize = maxStackSize;
            item.Type = itemType;

            // Ensure directory exists
            if (!System.IO.Directory.Exists(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
            }

            // Create asset file
            string assetPath = $"{savePath}/{itemName}.asset";
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            AssetDatabase.CreateAsset(item, assetPath);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = item;

            Clear();
        }

        private void Clear()
        {
            itemName = "";
            description = "";
            icon = null;
            isStackable = false;
            maxStackSize = 1;
            itemType = ItemType.Consumable;
        }
    }
}
