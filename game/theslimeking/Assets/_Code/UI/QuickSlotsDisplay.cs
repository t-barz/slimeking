using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    #endregion

    #region Private Variables
    private List<GameObject> quickSlotInstances = new List<GameObject>();
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        SetupLayoutGroup();
    }

    private void Start()
    {
        InitializeDisplay();
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

            Log($"Created quick slot {i + 1}");
        }

        if (enableBounceAnimation)
        {
            StartCoroutine(AnimateSlotsSequentially());
        }
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
        GameObject slot = GetQuickSlot(slotIndex);
        if (slot != null)
        {
            // Procura pela Image no slot
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.sprite = icon;
            }

            Log($"Set slot {slotIndex + 1} to {itemName}");
        }
    }

    /// <summary>
    /// Limpa o conteúdo de um slot específico
    /// </summary>
    public void ClearSlot(int slotIndex)
    {
        GameObject slot = GetQuickSlot(slotIndex);
        if (slot != null)
        {
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.sprite = null;
            }

            Log($"Cleared slot {slotIndex + 1}");
        }
    }

    /// <summary>
    /// Limpa todos os slots
    /// </summary>
    public void ClearAllSlots()
    {
        for (int i = 0; i < quickSlotInstances.Count; i++)
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