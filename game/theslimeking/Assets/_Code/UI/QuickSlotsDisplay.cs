using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SlimeKing.Core;
using SlimeKing.Items;

/// <summary>
/// Gerencia a visualização dos 4 Quick Slots na parte inferior central da tela.
/// Cada slot representa um item ou habilidade que pode ser rapidamente ativado.
/// </summary>
public class QuickSlotsDisplay : MonoBehaviour
{
    #region Settings
    [Header("Settings")]
    [SerializeField] private GameObject quickSlotPrefab;
    [SerializeField] private int slotCount = 4;

    [Header("Animation Settings")]
    [SerializeField] private bool enableBounceAnimation = true;
    [SerializeField] private float bounceDuration = 0.5f;
    [SerializeField] private float delayBeforeFirstSlot = 0f;
    [SerializeField] private float delayBetweenSlots = 0.1f;

    [Header("Layout Settings")]
    [SerializeField] private float spacing = 10f;
    [SerializeField] private float padding = 20f;

    [Header("Icon Settings")]
    [SerializeField] private Vector2 iconPadding = new Vector2(16f, 16f);
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    #endregion

    #region Private Variables
    private List<GameObject> quickSlotInstances = new List<GameObject>();
    private List<Image> slotIconImages = new List<Image>();
    private bool subscribedToManager;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        SetupLayoutGroup();
    }

    private void Start()
    {
        InitializeDisplay();
        SubscribeToQuickSlotManager();
    }

    private void OnEnable()
    {
        SubscribeToQuickSlotManager();
        RefreshAllSlots();
    }

    private void OnDisable()
    {
        UnsubscribeFromQuickSlotManager();
    }

    private void OnDestroy()
    {
        UnsubscribeFromQuickSlotManager();
    }
    #endregion

    #region QuickSlotManager Integration
    private void SubscribeToQuickSlotManager()
    {
        if (subscribedToManager)
        {
            return;
        }

        if (!QuickSlotManager.HasInstance)
        {
            StartCoroutine(WaitForQuickSlotManager());
            return;
        }

        QuickSlotManager.Instance.OnQuickSlotChanged += HandleQuickSlotChanged;
        subscribedToManager = true;
        RefreshAllSlots();
    }

    private System.Collections.IEnumerator WaitForQuickSlotManager()
    {
        while (!QuickSlotManager.HasInstance)
        {
            yield return null;
        }

        if (!subscribedToManager)
        {
            QuickSlotManager.Instance.OnQuickSlotChanged += HandleQuickSlotChanged;
            subscribedToManager = true;
            RefreshAllSlots();
        }
    }

    private void UnsubscribeFromQuickSlotManager()
    {
        if (!subscribedToManager)
        {
            return;
        }

        if (QuickSlotManager.HasInstance)
        {
            QuickSlotManager.Instance.OnQuickSlotChanged -= HandleQuickSlotChanged;
        }

        subscribedToManager = false;
    }

    private void HandleQuickSlotChanged(int slotIndex, ItemData item)
    {
        if (slotIndex < 0 || slotIndex >= slotIconImages.Count)
        {
            return;
        }

        UpdateSlotVisual(slotIndex, item);
    }

    private void RefreshAllSlots()
    {
        if (!QuickSlotManager.HasInstance)
        {
            return;
        }

        ItemData[] items = QuickSlotManager.Instance.GetAllQuickSlotItems();
        for (int i = 0; i < items.Length && i < slotIconImages.Count; i++)
        {
            UpdateSlotVisual(i, items[i]);
        }
    }

    private void UpdateSlotVisual(int slotIndex, ItemData item)
    {
        if (slotIndex < 0 || slotIndex >= slotIconImages.Count)
        {
            return;
        }

        Image iconImage = slotIconImages[slotIndex];
        if (iconImage == null)
        {
            return;
        }

        if (item != null && item.icon != null)
        {
            iconImage.sprite = item.icon;
            iconImage.preserveAspect = true;
            iconImage.enabled = true;
            Log($"Slot {slotIndex + 1} atualizado com {item.GetLocalizedName()}");
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            Log($"Slot {slotIndex + 1} limpo");
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Configura o HorizontalLayoutGroup para posicionar os slots de forma centralizada
    /// </summary>
    private void SetupLayoutGroup()
    {
        // Adiciona ou obtém o HorizontalLayoutGroup
        HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
        }

        // Configura para centralizar horizontalmente
        layoutGroup.childAlignment = TextAnchor.LowerCenter;
        layoutGroup.reverseArrangement = false;
        layoutGroup.spacing = spacing;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;

        // Configura padding
        LayoutElement layoutElement = GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = gameObject.AddComponent<LayoutElement>();
        }
        layoutElement.preferredHeight = 100f;

        Log("Layout group configured for centered bottom positioning");
    }

    /// <summary>
    /// Inicializa o display de quick slots
    /// </summary>
    private void InitializeDisplay()
    {
        CreateQuickSlots();
        Log($"Quick slots display initialized with {slotCount} slots");
    }

    /// <summary>
    /// Cria os quick slots na UI
    /// </summary>
    private void CreateQuickSlots()
    {
        ClearQuickSlots();

        if (quickSlotPrefab == null)
        {
            LogWarning("Quick Slot Prefab is not assigned!");
            return;
        }

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotInstance = Instantiate(quickSlotPrefab, transform);
            slotInstance.name = $"QuickSlot_{i + 1}";
            if (enableBounceAnimation)
            {
                slotInstance.transform.localScale = Vector3.zero;
            }
            quickSlotInstances.Add(slotInstance);

            Image iconImage = CreateSlotIcon(slotInstance, i);
            slotIconImages.Add(iconImage);

            Log($"Created quick slot {i + 1}");
        }

        if (enableBounceAnimation)
        {
            StartCoroutine(AnimateSlotsSequentially());
        }
    }

    /// <summary>
    /// Cria o ícone interno para exibir o item do slot
    /// </summary>
    private Image CreateSlotIcon(GameObject slotInstance, int slotIndex)
    {
        GameObject iconGO = new GameObject($"SlotIcon_{slotIndex + 1}", typeof(RectTransform), typeof(CanvasRenderer));
        iconGO.transform.SetParent(slotInstance.transform, false);

        RectTransform iconRect = iconGO.GetComponent<RectTransform>();
        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = Vector2.one;
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.offsetMin = iconPadding;
        iconRect.offsetMax = -iconPadding;

        Image iconImage = iconGO.AddComponent<Image>();
        iconImage.raycastTarget = false;
        iconImage.preserveAspect = true;
        iconImage.enabled = false;

        return iconImage;
    }

    /// <summary>
    /// Remove todos os quick slots existentes
    /// </summary>
    private void ClearQuickSlots()
    {
        foreach (var slot in quickSlotInstances)
        {
            if (slot != null)
            {
                Destroy(slot);
            }
        }
        quickSlotInstances.Clear();
        slotIconImages.Clear();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Obtém uma referência para um specific quick slot
    /// </summary>
    public GameObject GetQuickSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < quickSlotInstances.Count)
        {
            return quickSlotInstances[slotIndex];
        }

        LogWarning($"Quick slot index {slotIndex} is out of range!");
        return null;
    }

    /// <summary>
    /// Obtém todas as instâncias dos quick slots
    /// </summary>
    public List<GameObject> GetAllQuickSlots()
    {
        return new List<GameObject>(quickSlotInstances);
    }

    /// <summary>
    /// Define o conteúdo de um slot específico
    /// </summary>
    public void SetSlotContent(int slotIndex, Sprite icon, string itemName)
    {
        if (slotIndex < 0 || slotIndex >= slotIconImages.Count)
        {
            LogWarning($"Quick slot index {slotIndex} is out of range!");
            return;
        }

        Image iconImage = slotIconImages[slotIndex];
        if (iconImage != null)
        {
            if (icon != null)
            {
                iconImage.sprite = icon;
                iconImage.preserveAspect = true;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }
        }

        Log($"Set slot {slotIndex + 1} to {itemName}");
    }

    /// <summary>
    /// Limpa o conteúdo de um slot específico
    /// </summary>
    public void ClearSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slotIconImages.Count)
        {
            LogWarning($"Quick slot index {slotIndex} is out of range!");
            return;
        }

        Image iconImage = slotIconImages[slotIndex];
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        Log($"Cleared slot {slotIndex + 1}");
    }

    /// <summary>
    /// Limpa todos os slots
    /// </summary>
    public void ClearAllSlots()
    {
        for (int i = 0; i < slotIconImages.Count; i++)
        {
            ClearSlot(i);
        }

        Log("Cleared all quick slots");
    }
    #endregion

    #region Animation
    /// <summary>
    /// Anima os slots sequencialmente, da esquerda para a direita.
    /// </summary>
    private System.Collections.IEnumerator AnimateSlotsSequentially()
    {
        if (delayBeforeFirstSlot > 0f)
        {
            yield return new WaitForSeconds(delayBeforeFirstSlot);
        }

        for (int i = 0; i < quickSlotInstances.Count; i++)
        {
            var go = quickSlotInstances[i];
            if (go != null)
            {
                StartCoroutine(BounceSlot(go.transform));
                yield return new WaitForSeconds(delayBetweenSlots);
            }
        }
    }

    /// <summary>
    /// Anima um slot individual com efeito de bounce.
    /// </summary>
    private System.Collections.IEnumerator BounceSlot(Transform target)
    {
        float elapsed = 0f;
        Vector3 finalScale = Vector3.one;

        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / bounceDuration);

            float bounceProgress = Mathf.Sin(progress * Mathf.PI);
            float scale = Mathf.Lerp(0f, 1f, progress);
            if (progress < 0.5f)
            {
                scale += bounceProgress * 0.2f;
            }

            target.localScale = finalScale * scale;
            yield return null;
        }

        target.localScale = finalScale;
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Log condicional
    /// </summary>
    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[QuickSlotsDisplay] {message}");
        }
    }

    /// <summary>
    /// Log de aviso condicional
    /// </summary>
    private void LogWarning(string message)
    {
        if (enableDebugLogs)
        {
            Debug.LogWarning($"[QuickSlotsDisplay] {message}");
        }
    }
    #endregion
}