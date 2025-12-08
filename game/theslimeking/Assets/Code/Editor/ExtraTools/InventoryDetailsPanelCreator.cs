using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Ferramenta de editor para criar automaticamente o painel de detalhes de item no InventoryCanvas.
    /// Menu: Extra Tools/Setup/Create Inventory Details Panel
    /// </summary>
    public static class InventoryDetailsPanelCreator
    {
        [MenuItem("Extra Tools/Setup/Create Inventory Details Panel")]
        public static void CreateItemDetailsPanel()
        {
            // Encontra o InventoryCanvas na cena ativa
            SlimeKing.UI.InventoryUI inventoryUI = Object.FindFirstObjectByType<SlimeKing.UI.InventoryUI>();

            if (inventoryUI == null)
            {
                EditorUtility.DisplayDialog("Erro", "InventoryUI não encontrado na cena atual. Certifique-se de que a cena 3_InitialForest está aberta.", "OK");
                return;
            }

            GameObject inventoryCanvas = inventoryUI.gameObject;
            Transform inventoryPanel = inventoryCanvas.transform.Find("InventoryPanel");

            if (inventoryPanel == null)
            {
                EditorUtility.DisplayDialog("Erro", "InventoryPanel não encontrado como filho de InventoryCanvas.", "OK");
                return;
            }

            // Verifica se já existe ItemDetailsPanel
            Transform existingPanel = inventoryCanvas.transform.Find("ItemDetailsPanel");
            if (existingPanel != null)
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "Painel Já Existe",
                    "ItemDetailsPanel já existe. Deseja recriá-lo?",
                    "Sim", "Cancelar");

                if (overwrite)
                {
                    Undo.DestroyObjectImmediate(existingPanel.gameObject);
                }
                else
                {
                    return;
                }
            }

            // Cria o GameObject principal do painel
            GameObject itemDetailsPanel = new GameObject("ItemDetailsPanel");
            Undo.RegisterCreatedObjectUndo(itemDetailsPanel, "Create Item Details Panel");
            itemDetailsPanel.transform.SetParent(inventoryCanvas.transform, false);

            // Adiciona RectTransform e configura layout
            RectTransform panelRect = itemDetailsPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(200f, 360f);
            panelRect.anchoredPosition = new Vector2(280f, 0f); // À direita do grid de slots

            // Adiciona Image como background
            Image panelBg = itemDetailsPanel.AddComponent<Image>();
            panelBg.color = new Color(0.2f, 0.15f, 0.1f, 0.9f); // Marrom escuro translúcido

            // Cria ícone grande
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(itemDetailsPanel.transform, false);

            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.5f, 1f);
            iconRect.anchorMax = new Vector2(0.5f, 1f);
            iconRect.pivot = new Vector2(0.5f, 1f);
            iconRect.sizeDelta = new Vector2(128f, 128f);
            iconRect.anchoredPosition = new Vector2(0f, -20f);

            Image iconImage = iconObj.AddComponent<Image>();
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;

            // Cria título
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(itemDetailsPanel.transform, false);

            RectTransform titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.sizeDelta = new Vector2(-20f, 30f);
            titleRect.anchoredPosition = new Vector2(0f, -160f);

            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "Nome do Item";
            titleText.fontSize = 18f;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(1f, 0.9f, 0.5f); // Amarelo claro
            titleText.raycastTarget = false;

            // Cria descrição
            GameObject descObj = new GameObject("Description");
            descObj.transform.SetParent(itemDetailsPanel.transform, false);

            RectTransform descRect = descObj.AddComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0f, 0f);
            descRect.anchorMax = new Vector2(1f, 1f);
            descRect.pivot = new Vector2(0.5f, 1f);
            descRect.offsetMin = new Vector2(10f, 10f); // Left, Bottom
            descRect.offsetMax = new Vector2(-10f, -200f); // Right, Top

            TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
            descText.text = "Selecione um item para ver sua descrição.";
            descText.fontSize = 14f;
            descText.alignment = TextAlignmentOptions.TopLeft;
            descText.color = new Color(0.9f, 0.9f, 0.9f);
            descText.textWrappingMode = TextWrappingModes.Normal;
            descText.raycastTarget = false;

            // Atribui referências ao InventoryUI usando SerializedObject
            SerializedObject serializedUI = new SerializedObject(inventoryUI);

            serializedUI.FindProperty("itemDetailsPanel").objectReferenceValue = itemDetailsPanel;
            serializedUI.FindProperty("detailsIconImage").objectReferenceValue = iconImage;
            serializedUI.FindProperty("detailsTitleText").objectReferenceValue = titleText;
            serializedUI.FindProperty("detailsDescriptionText").objectReferenceValue = descText;

            serializedUI.ApplyModifiedProperties();

            // Marca a cena como modificada
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log("[InventoryDetailsPanelCreator] ✅ ItemDetailsPanel criado com sucesso e referências configuradas no InventoryUI!");

            // Seleciona o painel criado
            Selection.activeGameObject = itemDetailsPanel;
        }

        [MenuItem("Extra Tools/Setup/Create Inventory Details Panel", true)]
        private static bool ValidateCreateItemDetailsPanel()
        {
            // Menu só fica ativo se houver InventoryUI na cena
            return Object.FindFirstObjectByType<SlimeKing.UI.InventoryUI>() != null;
        }
    }
}
