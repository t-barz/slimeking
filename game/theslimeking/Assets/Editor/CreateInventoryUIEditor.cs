using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

/// <summary>
/// Editor script para criar automaticamente a estrutura completa do Inventory UI
/// </summary>
public static class CreateInventoryUIEditor
{
    [MenuItem("GameObject/Extra Tools/Create Inventory UI", false, 0)]
    private static void CreateInventoryUI(MenuCommand menuCommand)
    {
        CreateInventoryUIPublic();
    }

    public static void CreateInventoryUIPublic()
    {
        // 1. Criar Canvas
        GameObject canvasGO = new GameObject("InventoryCanvas");
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Inventory UI");

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGO.AddComponent<GraphicRaycaster>();
        CanvasGroup canvasGroup = canvasGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Adiciona o script InventoryUI
        var inventoryUI = canvasGO.AddComponent<SlimeKing.UI.InventoryUI>();

        // 2. Criar Panel Principal
        GameObject panelGO = new GameObject("InventoryPanel");
        panelGO.transform.SetParent(canvasGO.transform, false);

        RectTransform panelRect = panelGO.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(600, 450);
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0.36f, 0.25f, 0.2f, 1f); // Marrom escuro

        // 3. Criar Título
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panelGO.transform, false);

        RectTransform titleRect = titleGO.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(500, 60);
        titleRect.anchoredPosition = new Vector2(0, -10);

        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "INVENTÁRIO";
        titleText.fontSize = 36;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        titleText.fontStyle = FontStyles.Bold;

        // 4. Criar Botão de Fechar
        GameObject closeButtonGO = new GameObject("CloseButton");
        closeButtonGO.transform.SetParent(panelGO.transform, false);

        RectTransform closeRect = closeButtonGO.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1f, 1f);
        closeRect.anchorMax = new Vector2(1f, 1f);
        closeRect.pivot = new Vector2(1f, 1f);
        closeRect.sizeDelta = new Vector2(40, 40);
        closeRect.anchoredPosition = new Vector2(-10, -10);

        Image closeImage = closeButtonGO.AddComponent<Image>();
        closeImage.color = new Color(1f, 0.27f, 0.27f, 1f); // Vermelho

        Button closeButton = closeButtonGO.AddComponent<Button>();
        ColorBlock colors = closeButton.colors;
        colors.normalColor = new Color(1f, 0.27f, 0.27f, 1f);
        colors.highlightedColor = new Color(1f, 0.4f, 0.4f, 1f);
        colors.pressedColor = new Color(0.8f, 0f, 0f, 1f);
        closeButton.colors = colors;

        // Texto do botão
        GameObject closeTextGO = new GameObject("Text");
        closeTextGO.transform.SetParent(closeButtonGO.transform, false);

        RectTransform closeTextRect = closeTextGO.AddComponent<RectTransform>();
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.sizeDelta = Vector2.zero;
        closeTextRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI closeText = closeTextGO.AddComponent<TextMeshProUGUI>();
        closeText.text = "X";
        closeText.fontSize = 24;
        closeText.alignment = TextAlignmentOptions.Center;
        closeText.color = Color.white;

        // Configurar evento do botão
        closeButton.onClick.AddListener(() => {
            var invUI = closeButtonGO.GetComponentInParent<SlimeKing.UI.InventoryUI>();
            if (invUI != null) invUI.CloseInventory();
        });

        // 5. Criar Container dos Slots
        GameObject slotsContainerGO = new GameObject("SlotsContainer");
        slotsContainerGO.transform.SetParent(panelGO.transform, false);

        RectTransform slotsRect = slotsContainerGO.AddComponent<RectTransform>();
        slotsRect.anchorMin = new Vector2(0.5f, 0.5f);
        slotsRect.anchorMax = new Vector2(0.5f, 0.5f);
        slotsRect.pivot = new Vector2(0.5f, 0.5f);
        slotsRect.sizeDelta = new Vector2(520, 330);
        slotsRect.anchoredPosition = new Vector2(0, -30);

        GridLayoutGroup grid = slotsContainerGO.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(120, 100);
        grid.spacing = new Vector2(10, 10);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 4;

        // 6. Criar 12 Slots
        for (int i = 0; i < 12; i++)
        {
            GameObject slotGO = new GameObject($"Slot_{i}");
            slotGO.transform.SetParent(slotsContainerGO.transform, false);

            Image slotImage = slotGO.AddComponent<Image>();
            slotImage.color = new Color(0.55f, 0.44f, 0.28f, 1f); // Marrom médio

            // Adiciona outline
            Outline outline = slotGO.AddComponent<Outline>();
            outline.effectColor = Color.white;
            outline.effectDistance = new Vector2(2, -2);
        }

        // Configurar referências no InventoryUI
        var inventoryUIType = inventoryUI.GetType();
        var panelField = inventoryUIType.GetField("inventoryPanel", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var canvasGroupField = inventoryUIType.GetField("canvasGroup", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (panelField != null) panelField.SetValue(inventoryUI, panelGO);
        if (canvasGroupField != null) canvasGroupField.SetValue(inventoryUI, canvasGroup);

        // Desativa o panel inicialmente
        panelGO.SetActive(false);

        // Seleciona o canvas criado
        Selection.activeGameObject = canvasGO;

        Debug.Log("✅ Inventory UI criado com sucesso!");
        Debug.Log("📝 Configure as referências no Inspector se necessário.");
    }

    [MenuItem("GameObject/Extra Tools/Create Inventory UI", true)]
    private static bool ValidateCreateInventoryUI()
    {
        // Verifica se já existe um InventoryCanvas na cena
        var existing = GameObject.Find("InventoryCanvas");
        return existing == null;
    }
}
