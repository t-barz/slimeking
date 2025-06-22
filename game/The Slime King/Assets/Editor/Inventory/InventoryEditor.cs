using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TheSlimeKing.Core.Inventory;

namespace TheSlimeKing.Editor
{
    /// <summary>
    /// Editor para criar e gerenciar itens do jogo
    /// </summary>
    public class InventoryEditor : EditorWindow
    {
        private Vector2 _scrollPosition;
        private ItemData _selectedItem;
        private List<ItemData> _allItems = new List<ItemData>();
        private string _searchString = "";
        private bool _showCreateSection = false;
        private ItemData _newItem;

        // Propriedades para criação de novo item
        private string _newItemName = "Novo Item";
        private string _newItemDesc = "Descrição do item";
        private Sprite _newItemIcon;
        private GameObject _newItemPrefab;
        private ItemType _newItemType = ItemType.Consumable;
        private ItemRarity _newItemRarity = ItemRarity.Common;
        private int _newItemMaxStack = 10;
        private bool _newItemConsumable = true;
        private bool _newItemQuest = false;

        [MenuItem("The Slime King/Inventário/Editor de Itens")]
        public static void ShowWindow()
        {
            GetWindow<InventoryEditor>("Editor de Itens");
        }

        private void OnEnable()
        {
            RefreshItemsList();
        }

        private void OnGUI()
        {
            GUILayout.Label("Sistema de Inventário - Editor de Itens", EditorStyles.boldLabel);

            // Área de busca
            EditorGUILayout.BeginHorizontal();
            _searchString = EditorGUILayout.TextField("Buscar:", _searchString);
            if (GUILayout.Button("Limpar", GUILayout.Width(60)))
            {
                _searchString = "";
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndHorizontal();

            // Barra de ações
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Atualizar Lista"))
            {
                RefreshItemsList();
            }

            if (GUILayout.Button("Criar Novo Item"))
            {
                _showCreateSection = !_showCreateSection;
                if (_showCreateSection && _newItem == null)
                {
                    _newItem = CreateInstance<ItemData>();
                }
            }
            EditorGUILayout.EndHorizontal();

            // Setor de criação de novo item
            if (_showCreateSection)
            {
                DrawCreateItemSection();
            }

            EditorGUILayout.Space();

            // Separador
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // Lista de itens existentes
            GUILayout.Label("Itens Disponíveis:", EditorStyles.boldLabel);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (var item in _allItems)
            {
                // Se tiver busca, filtra
                if (!string.IsNullOrEmpty(_searchString))
                {
                    if (!item.name.ToLower().Contains(_searchString.ToLower()))
                        continue;
                }

                EditorGUILayout.BeginHorizontal("box");

                // Ícone do item
                if (item.Icon != null)
                {
                    GUILayout.Label(AssetPreview.GetAssetPreview(item.Icon), GUILayout.Width(64), GUILayout.Height(64));
                }
                else
                {
                    GUILayout.Label("Sem ícone", GUILayout.Width(64), GUILayout.Height(64));
                }

                // Informações do item
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(item.name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"Tipo: {item.Type}, Raridade: {item.Rarity}");
                EditorGUILayout.LabelField($"Stack Máx: {item.MaxStack}, Consumível: {(item.IsConsumable ? "Sim" : "Não")}");
                EditorGUILayout.EndVertical();

                // Botões de ação
                EditorGUILayout.BeginVertical(GUILayout.Width(80));
                if (GUILayout.Button("Editar"))
                {
                    Selection.activeObject = item;
                    EditorGUIUtility.PingObject(item);
                }

                if (GUILayout.Button("Duplicar"))
                {
                    DuplicateItem(item);
                }

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Excluir"))
                {
                    if (EditorUtility.DisplayDialog("Excluir Item",
                        $"Tem certeza que deseja excluir o item '{item.name}'?",
                        "Sim", "Cancelar"))
                    {
                        DeleteItem(item);
                    }
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawCreateItemSection()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Criar Novo Item", EditorStyles.boldLabel);

            _newItemName = EditorGUILayout.TextField("Nome do Item:", _newItemName);
            _newItemDesc = EditorGUILayout.TextField("Descrição:", _newItemDesc);
            _newItemIcon = (Sprite)EditorGUILayout.ObjectField("Ícone:", _newItemIcon, typeof(Sprite), false);
            _newItemPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab (Opcional):", _newItemPrefab, typeof(GameObject), false);
            _newItemType = (ItemType)EditorGUILayout.EnumPopup("Tipo de Item:", _newItemType);
            _newItemRarity = (ItemRarity)EditorGUILayout.EnumPopup("Raridade:", _newItemRarity);
            _newItemMaxStack = EditorGUILayout.IntSlider("Stack Máximo:", _newItemMaxStack, 1, 99);
            _newItemConsumable = EditorGUILayout.Toggle("É Consumível:", _newItemConsumable);
            _newItemQuest = EditorGUILayout.Toggle("É Item de Quest:", _newItemQuest);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Criar e Salvar"))
            {
                CreateNewItem();
            }

            if (GUILayout.Button("Cancelar"))
            {
                _showCreateSection = false;
                _newItem = null;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void RefreshItemsList()
        {
            _allItems.Clear();
            string[] guids = AssetDatabase.FindAssets("t:ItemData");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
                if (item != null)
                {
                    _allItems.Add(item);
                }
            }
        }

        private void CreateNewItem()
        {
            // Validações básicas
            if (string.IsNullOrEmpty(_newItemName.Trim()))
            {
                EditorUtility.DisplayDialog("Erro", "O nome do item não pode estar vazio.", "OK");
                return;
            }

            // Criar pasta se não existir
            string folder = "Assets/ScriptableObjects/Inventory/Items";
            if (!AssetDatabase.IsValidFolder(folder))
            {
                string parentFolder = "Assets/ScriptableObjects/Inventory";
                if (!AssetDatabase.IsValidFolder(parentFolder))
                {
                    string baseFolder = "Assets/ScriptableObjects";
                    if (!AssetDatabase.IsValidFolder(baseFolder))
                    {
                        AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
                    }
                    AssetDatabase.CreateFolder(baseFolder, "Inventory");
                }
                AssetDatabase.CreateFolder(parentFolder, "Items");
            }

            // Criar novo SO
            ItemData newItem = CreateInstance<ItemData>();

            // Configurar valores via SerializedObject para modificar propriedades privadas
            SerializedObject so = new SerializedObject(newItem);

            // Nome no Inspector
            newItem.name = _newItemName;

            // Valores das propriedades
            so.FindProperty("_nameKey").stringValue = "item_" + _newItemName.ToLower().Replace(" ", "_");
            so.FindProperty("_descriptionKey").stringValue = "item_" + _newItemName.ToLower().Replace(" ", "_") + "_desc";
            so.FindProperty("_icon").objectReferenceValue = _newItemIcon;
            so.FindProperty("_prefab").objectReferenceValue = _newItemPrefab;
            so.FindProperty("_itemType").enumValueIndex = (int)_newItemType;
            so.FindProperty("_rarity").enumValueIndex = (int)_newItemRarity;
            so.FindProperty("_maxStack").intValue = _newItemMaxStack;
            so.FindProperty("_isConsumable").boolValue = _newItemConsumable;
            so.FindProperty("_isQuest").boolValue = _newItemQuest;

            so.ApplyModifiedProperties();

            // Salvar asset
            string assetPath = $"{folder}/{_newItemName.Replace(" ", "")}.asset";
            AssetDatabase.CreateAsset(newItem, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.SaveAssets();

            // Selecionar o novo item
            Selection.activeObject = newItem;
            EditorGUIUtility.PingObject(newItem);

            // Atualizar lista
            RefreshItemsList();

            // Resetar estado do editor
            _showCreateSection = false;
            _newItem = null;
            _newItemName = "Novo Item";
            _newItemDesc = "Descrição do item";
            _newItemIcon = null;
            _newItemPrefab = null;
            _newItemType = ItemType.Consumable;
            _newItemRarity = ItemRarity.Common;
            _newItemMaxStack = 10;
            _newItemConsumable = true;
            _newItemQuest = false;

            // Mensagem de sucesso
            EditorUtility.DisplayDialog("Sucesso", "Item criado com sucesso!", "OK");
        }

        private void DuplicateItem(ItemData sourcesItem)
        {
            // Criar pasta se não existir
            string folder = "Assets/ScriptableObjects/Inventory/Items";
            if (!AssetDatabase.IsValidFolder(folder))
            {
                string parentFolder = "Assets/ScriptableObjects/Inventory";
                if (!AssetDatabase.IsValidFolder(parentFolder))
                {
                    string baseFolder = "Assets/ScriptableObjects";
                    if (!AssetDatabase.IsValidFolder(baseFolder))
                    {
                        AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
                    }
                    AssetDatabase.CreateFolder(baseFolder, "Inventory");
                }
                AssetDatabase.CreateFolder(parentFolder, "Items");
            }

            // Criar cópia
            ItemData newItem = Instantiate(sourcesItem);
            newItem.name = $"{sourcesItem.name}_Cópia";

            // Salvar asset
            string assetPath = $"{folder}/{newItem.name.Replace(" ", "")}.asset";
            AssetDatabase.CreateAsset(newItem, AssetDatabase.GenerateUniqueAssetPath(assetPath));
            AssetDatabase.SaveAssets();

            // Selecionar o novo item
            Selection.activeObject = newItem;
            EditorGUIUtility.PingObject(newItem);

            // Atualizar lista
            RefreshItemsList();

            // Mensagem de sucesso
            EditorUtility.DisplayDialog("Sucesso", "Item duplicado com sucesso!", "OK");
        }

        private void DeleteItem(ItemData item)
        {
            string path = AssetDatabase.GetAssetPath(item);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();

            // Atualizar lista
            RefreshItemsList();
        }
    }
}
