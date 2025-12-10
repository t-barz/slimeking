using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheSlimeKing.UI;
using TheSlimeKing.Inventory;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Editor tool to create Quick Slots HUD in the scene
    /// </summary>
    public class QuickSlotsHUDCreator
    {
        [MenuItem("Extra Tools/Setup/Create Quick Slots HUD")]
        public static void CreateQuickSlotsHUD()
        {
            // Procura pelo Canvas principal na cena
            Canvas mainCanvas = Object.FindFirstObjectByType<Canvas>();

            if (mainCanvas == null)
            {
                Debug.LogError("[QuickSlotsHUDCreator] Nenhum Canvas encontrado na cena. Crie um primeiro.");
                return;
            }

            // Verifica se já existe um QuickSlotsContainer
            Transform existingContainer = mainCanvas.transform.Find("QuickSlotsContainer");
            if (existingContainer != null)
            {
                Debug.LogWarning("[QuickSlotsHUDCreator] QuickSlotsContainer já existe na cena.");
                return;
            }

            // Cria o container
            GameObject containerObj = new GameObject("QuickSlotsContainer");
            RectTransform containerRect = containerObj.AddComponent<RectTransform>();
            containerObj.transform.SetParent(mainCanvas.transform, false);

            // Configura a posição: bottom-center, 20px acima da borda
            containerRect.anchorMin = new Vector2(0.5f, 0f);
            containerRect.anchorMax = new Vector2(0.5f, 0f);
            containerRect.offsetMin = new Vector2(-200f, 20f); // Offset left
            containerRect.offsetMax = new Vector2(200f, 120f); // Offset right/top (4 slots de 80px + spacing)
            containerRect.pivot = new Vector2(0.5f, 0f);

            // Adiciona LayoutGroup para organizar os slots
            HorizontalLayoutGroup layoutGroup = containerObj.AddComponent<HorizontalLayoutGroup>();
            layoutGroup.spacing = 10f;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;

            // Cria os 4 quick slots
            for (int i = 0; i < 4; i++)
            {
                CreateQuickSlot(containerObj.transform, i);
            }

            // Adiciona QuickSlotManager ao container
            QuickSlotManager manager = containerObj.AddComponent<QuickSlotManager>();

            // Configura as referências de UI no manager via reflexão (ou manualmente)
            QuickSlotUI[] quickSlotUIs = containerObj.GetComponentsInChildren<QuickSlotUI>();

            // Usa reflexão para setar o campo privado quickSlotUIs
            System.Reflection.FieldInfo field = typeof(QuickSlotManager).GetField("quickSlotUIs",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null && quickSlotUIs.Length >= 4)
            {
                field.SetValue(manager, quickSlotUIs);
            }

            Debug.Log("[QuickSlotsHUDCreator] Quick Slots HUD criado com sucesso!");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static void CreateQuickSlot(Transform parent, int index)
        {
            // Cria o GameObject do slot
            GameObject slotObj = new GameObject($"QuickSlot_{index}");
            RectTransform slotRect = slotObj.AddComponent<RectTransform>();
            slotObj.transform.SetParent(parent, false);

            // Configura tamanho
            slotRect.sizeDelta = new Vector2(80f, 80f);

            // Adiciona Image para o fundo
            Image slotImage = slotObj.AddComponent<Image>();
            slotImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); // Fundo escuro semi-transparente

            // Cria o ícone do item
            GameObject iconObj = new GameObject("Icon");
            RectTransform iconRect = iconObj.AddComponent<RectTransform>();
            iconObj.transform.SetParent(slotObj.transform, false);
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            Image iconImage = iconObj.AddComponent<Image>();

            // Cria o texto de quantidade
            GameObject quantityObj = new GameObject("Quantity");
            RectTransform quantityRect = quantityObj.AddComponent<RectTransform>();
            quantityObj.transform.SetParent(slotObj.transform, false);
            quantityRect.anchorMin = new Vector2(1f, 0f);
            quantityRect.anchorMax = new Vector2(1f, 0f);
            quantityRect.pivot = new Vector2(1f, 0f);
            quantityRect.sizeDelta = new Vector2(30f, 20f);
            quantityRect.anchoredPosition = new Vector2(-5f, 5f);

            TextMeshProUGUI quantityText = quantityObj.AddComponent<TextMeshProUGUI>();
            quantityText.text = "";
            quantityText.fontSize = 20;
            quantityText.alignment = TextAlignmentOptions.BottomRight;
            quantityText.color = Color.white;

            // Adiciona o componente QuickSlotUI
            QuickSlotUI slotUI = slotObj.AddComponent<QuickSlotUI>();

            // Usa reflexão para setar os campos privados
            System.Reflection.FieldInfo iconField = typeof(QuickSlotUI).GetField("iconImage",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            System.Reflection.FieldInfo quantityField = typeof(QuickSlotUI).GetField("quantityText",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            System.Reflection.FieldInfo directionField = typeof(QuickSlotUI).GetField("slotDirection",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (iconField != null)
                iconField.SetValue(slotUI, iconImage);
            if (quantityField != null)
                quantityField.SetValue(slotUI, quantityText);
            if (directionField != null)
                directionField.SetValue(slotUI, index);
        }
    }
}
