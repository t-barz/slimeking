using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SlimeKing.Core;
using SlimeKing.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SlimeKing.UI
{
    /// <summary>
    /// Responsável por renderizar o inventário no painel dedicado.
    /// Cria ícones temporários sobre os placeholders de slots, reage aos eventos do InventoryManager
    /// e mantém o painel de detalhes sincronizado com o último item coletado.
    /// </summary>
    [DisallowMultipleComponent]
    public class InventoryPanelView : MonoBehaviour
    {
        [Header("Detail Panel")]
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemDescriptionText;
        [SerializeField] private CanvasGroup itemDetailsGroup;
        [SerializeField, TextArea] private string emptyInventoryTitle = "Inventário vazio";
        [SerializeField, TextArea] private string emptyInventoryDescription = "Colete itens pelo mundo para vê-los aqui.";

        [Header("Slot Visuals")]
        [SerializeField] private Sprite defaultIcon;
        [SerializeField] private TMP_FontAsset quantityFont;
        [SerializeField] private float quantityFontSize = 28f;
        [SerializeField] private Color quantityColor = Color.white;
        [SerializeField] private Vector2 quantityOffset = new Vector2(-12f, 12f);

        private readonly List<SlotRuntime> slotViews = new();
        private bool slotsBuilt;
        private bool subscribed;
        private Coroutine waitForManagerCoroutine;
        private ItemData selectedItem;

        private static readonly Vector2 StretchPadding = new Vector2(24f, 24f);

        private void Awake()
        {
            BuildSlotsIfNeeded();
        }

        private void OnEnable()
        {
            BuildSlotsIfNeeded();
            StartInventorySync();
        }

        private void OnDisable()
        {
            StopInventorySync();
        }

        private void OnDestroy()
        {
            StopInventorySync();
        }

        private void StartInventorySync()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (InventoryManager.HasInstance)
            {
                AttachInventoryListeners();
                RefreshInventoryContents();
            }
            else if (waitForManagerCoroutine == null)
            {
                waitForManagerCoroutine = StartCoroutine(WaitForInventoryManager());
            }
        }

        private void StopInventorySync()
        {
            if (waitForManagerCoroutine != null)
            {
                StopCoroutine(waitForManagerCoroutine);
                waitForManagerCoroutine = null;
            }

            DetachInventoryListeners();
        }

        private IEnumerator WaitForInventoryManager()
        {
            while (!InventoryManager.HasInstance)
            {
                yield return null;
            }

            waitForManagerCoroutine = null;
            AttachInventoryListeners();
            RefreshInventoryContents();
        }

        private void AttachInventoryListeners()
        {
            if (subscribed || !InventoryManager.HasInstance)
            {
                return;
            }

            InventoryManager.Instance.OnItemAdded += HandleItemAdded;
            InventoryManager.Instance.OnItemRemoved += HandleItemRemoved;
            subscribed = true;
        }

        private void DetachInventoryListeners()
        {
            if (!subscribed || !InventoryManager.HasInstance)
            {
                subscribed = false;
                return;
            }

            InventoryManager.Instance.OnItemAdded -= HandleItemAdded;
            InventoryManager.Instance.OnItemRemoved -= HandleItemRemoved;
            subscribed = false;
        }

        private void HandleItemAdded(ItemData itemData, int totalQuantity)
        {
            RefreshInventoryContents();
            ShowDetails(itemData, totalQuantity);
        }

        private void HandleItemRemoved(ItemData itemData, int remainingQuantity)
        {
            RefreshInventoryContents();

            if (remainingQuantity > 0)
            {
                ShowDetails(itemData, remainingQuantity);
                return;
            }

            if (selectedItem == itemData)
            {
                var replacement = slotViews.FirstOrDefault(slot => slot.Item != null)?.Item;
                if (replacement != null)
                {
                    int quantity = InventoryManager.HasInstance ? InventoryManager.Instance.GetItemCount(replacement) : 0;
                    ShowDetails(replacement, quantity);
                }
                else
                {
                    ShowEmptyDetails();
                }
            }
        }

        private void RefreshInventoryContents()
        {
            if (!slotsBuilt)
            {
                BuildSlotsIfNeeded();
            }

            if (slotViews.Count == 0)
            {
                ShowEmptyDetails();
                return;
            }

            if (!InventoryManager.HasInstance)
            {
                foreach (SlotRuntime slot in slotViews)
                {
                    slot.Clear();
                }

                ShowEmptyDetails();
                return;
            }

            Dictionary<ItemData, int> allItems = InventoryManager.Instance.GetAllItems();
            var orderedItems = allItems
                .OrderBy(kvp => kvp.Key.itemName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            int totalItems = 0;
            foreach (var pair in orderedItems)
            {
                totalItems += pair.Value;
            }

            List<ItemData> flattenedItems = new List<ItemData>(totalItems);
            foreach (var pair in orderedItems)
            {
                for (int i = 0; i < pair.Value; i++)
                {
                    flattenedItems.Add(pair.Key);
                }
            }

            int index = 0;
            ItemData firstItem = null;

            foreach (ItemData item in flattenedItems)
            {
                if (index >= slotViews.Count)
                {
                    break;
                }

                slotViews[index].Bind(item);
                firstItem ??= item;
                index++;
            }

            for (; index < slotViews.Count; index++)
            {
                slotViews[index].Clear();
            }

            if (selectedItem != null && orderedItems.All(pair => pair.Key != selectedItem))
            {
                selectedItem = null;
            }

            if (selectedItem == null)
            {
                if (firstItem != null)
                {
                    ShowDetails(firstItem, InventoryManager.Instance.GetItemCount(firstItem));
                }
                else
                {
                    ShowEmptyDetails();
                }
            }
        }

        private void ShowDetails(ItemData itemData, int quantity)
        {
            selectedItem = itemData;

            if (itemNameText != null)
            {
                itemNameText.text = itemData?.itemName ?? string.Empty;
            }

            if (itemDescriptionText != null)
            {
                itemDescriptionText.text = itemData != null ? itemData.GetFullDescription() : string.Empty;
            }

            if (itemDetailsGroup != null)
            {
                itemDetailsGroup.alpha = 1f;
                itemDetailsGroup.blocksRaycasts = true;
                itemDetailsGroup.interactable = true;
            }
        }

        private void ShowEmptyDetails()
        {
            selectedItem = null;

            if (itemNameText != null)
            {
                itemNameText.text = emptyInventoryTitle;
            }

            if (itemDescriptionText != null)
            {
                itemDescriptionText.text = emptyInventoryDescription;
            }

            if (itemDetailsGroup != null)
            {
                itemDetailsGroup.alpha = 0.75f;
                itemDetailsGroup.blocksRaycasts = false;
                itemDetailsGroup.interactable = false;
            }
        }

        private void BuildSlotsIfNeeded()
        {
            if (slotsBuilt)
            {
                return;
            }

            RectTransform root = GetComponent<RectTransform>();
            if (root == null)
            {
                Debug.LogWarning("[InventoryPanelView] RectTransform não encontrado no InventoryPanel.");
                return;
            }

            RectTransform[] anchors = root
                .GetComponentsInChildren<RectTransform>(true)
                .Where(rt => rt != root && rt.name.StartsWith("InventorySlot", StringComparison.Ordinal))
                .OrderBy(rt => rt.name, StringComparer.Ordinal)
                .ToArray();

            if (anchors.Length == 0)
            {
                Debug.LogWarning("[InventoryPanelView] Nenhum placeholder de slot (InventorySlot*) foi encontrado.");
                return;
            }

            foreach (RectTransform anchor in anchors)
            {
                slotViews.Add(new SlotRuntime(anchor, this));
            }

            slotsBuilt = true;
        }

        private TMP_FontAsset ResolveQuantityFont()
        {
            if (quantityFont != null)
            {
                return quantityFont;
            }

            if (itemNameText != null)
            {
                return itemNameText.font;
            }

            return TMP_Settings.defaultFontAsset;
        }

        private void ConfigureIconRect(RectTransform slotAnchor, RectTransform iconRect)
        {
            if (slotAnchor == null || iconRect == null)
            {
                return;
            }

            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.pivot = new Vector2(0.5f, 0.5f);
            iconRect.offsetMin = StretchPadding;
            iconRect.offsetMax = -StretchPadding;
        }

        private sealed class SlotRuntime
        {
            private readonly InventoryPanelView owner;
            private readonly RectTransform slotAnchor;
            private readonly Image iconImage;
            private readonly TextMeshProUGUI quantityLabel;

            public ItemData Item { get; private set; }

            public SlotRuntime(RectTransform anchor, InventoryPanelView owner)
            {
                this.owner = owner;
                slotAnchor = anchor;

                iconImage = CreateIcon(anchor, owner);
                quantityLabel = CreateQuantityLabel(anchor, owner);

                Clear();
            }

            public void Bind(ItemData itemData)
            {
                Item = itemData;

                owner.ConfigureIconRect(slotAnchor, iconImage.rectTransform);

                if (itemData == null)
                {
                    Clear();
                    return;
                }

                Sprite spriteToUse = itemData.icon != null ? itemData.icon : owner.defaultIcon;
                if (spriteToUse != null)
                {
                    iconImage.sprite = spriteToUse;
                    iconImage.preserveAspect = true;
                    iconImage.enabled = true;
                }
                else
                {
                    iconImage.enabled = false;
                }

                quantityLabel.enabled = false;
            }

            public void Clear()
            {
                Item = null;
                iconImage.enabled = false;
                quantityLabel.enabled = false;
            }

            private static Image CreateIcon(RectTransform anchor, InventoryPanelView owner)
            {
                var iconGO = new GameObject("RuntimeItemIcon", typeof(RectTransform), typeof(CanvasRenderer));
                iconGO.transform.SetParent(anchor, false);

                RectTransform iconRect = iconGO.GetComponent<RectTransform>();
                owner.ConfigureIconRect(anchor, iconRect);

                Image image = iconGO.AddComponent<Image>();
                image.raycastTarget = false;
                image.enabled = false;
                return image;
            }

            private static TextMeshProUGUI CreateQuantityLabel(RectTransform anchor, InventoryPanelView owner)
            {
                var qtyGO = new GameObject("RuntimeQuantityLabel", typeof(RectTransform), typeof(CanvasRenderer));
                qtyGO.transform.SetParent(anchor, false);

                RectTransform qtyRect = qtyGO.GetComponent<RectTransform>();
                qtyRect.anchorMin = qtyRect.anchorMax = new Vector2(1f, 0f);
                qtyRect.pivot = new Vector2(1f, 0f);
                qtyRect.anchoredPosition = owner.quantityOffset;

                TextMeshProUGUI label = qtyGO.AddComponent<TextMeshProUGUI>();
                label.font = owner.ResolveQuantityFont();
                label.fontSize = owner.quantityFontSize;
                label.color = owner.quantityColor;
                label.alignment = TextAlignmentOptions.BottomRight;
                label.raycastTarget = false;
                label.text = string.Empty;
                label.enabled = false;
                return label;
            }
        }
    }
}
