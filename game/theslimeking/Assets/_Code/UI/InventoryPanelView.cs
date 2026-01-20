using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SlimeKing.Core;
using SlimeKing.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

        private readonly List<SlotRuntime> slotViews = new();
        private bool slotsBuilt;
        private bool subscribed;
        private Coroutine waitForManagerCoroutine;
        private ItemData selectedItem;
        private SlotRuntime focusedSlot;

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
        }

        private void HandleItemRemoved(ItemData itemData, int remainingQuantity)
        {
            RefreshInventoryContents();
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
                .OrderBy(kvp => kvp.Key != null ? kvp.Key.GetLocalizedName() : string.Empty, StringComparer.OrdinalIgnoreCase)
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

            foreach (ItemData item in flattenedItems)
            {
                if (index >= slotViews.Count)
                {
                    break;
                }

                slotViews[index].Bind(item);
                index++;
            }

            for (; index < slotViews.Count; index++)
            {
                slotViews[index].Clear();
            }

            bool restoredSelection = TryFocusSelectedItemSlot();
            if (!restoredSelection)
            {
                FocusFirstAvailableSlotInternal();
            }
        }

        public void FocusFirstAvailableSlot()
        {
            if (!slotsBuilt)
            {
                BuildSlotsIfNeeded();
            }

            FocusFirstAvailableSlotInternal();
        }

        private void FocusFirstAvailableSlotInternal()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            SlotRuntime slot = slotViews.FirstOrDefault(s => s.Item != null);
            if (slot != null)
            {
                slot.Focus();
                return;
            }

            ClearFocusedSlot();
            ClearOwnedSelection();
            ShowEmptyDetails();
        }

        private void SetFocusedSlot(SlotRuntime slot)
        {
            if (slot == null)
            {
                return;
            }

            if (focusedSlot == slot)
            {
                focusedSlot.SetSelectionState(true);
                return;
            }

            ClearFocusedSlot();

            focusedSlot = slot;
            focusedSlot.SetSelectionState(true);
        }

        private void ClearFocusedSlot()
        {
            if (focusedSlot == null)
            {
                return;
            }

            focusedSlot.SetSelectionState(false);
            focusedSlot = null;
        }

        private bool TryFocusSelectedItemSlot()
        {
            if (selectedItem == null)
            {
                return false;
            }

            SlotRuntime slot = slotViews.FirstOrDefault(s => s.Item == selectedItem);
            if (slot == null)
            {
                selectedItem = null;
                return false;
            }

            slot.Focus();
            return true;
        }

        private void ClearOwnedSelection()
        {
            ClearFocusedSlot();
            EventSystem current = EventSystem.current;
            if (current == null)
            {
                return;
            }

            GameObject selectedObject = current.currentSelectedGameObject;
            if (selectedObject == null)
            {
                return;
            }

            if (selectedObject.transform.IsChildOf(transform))
            {
                current.SetSelectedGameObject(null);
            }
        }

        private void HandleSlotFocused(SlotRuntime slot)
        {
            if (slot?.Item == null)
            {
                ClearFocusedSlot();
                ShowEmptyDetails();
                return;
            }

            SetFocusedSlot(slot);
            int quantity = InventoryManager.HasInstance ? InventoryManager.Instance.GetItemCount(slot.Item) : 0;
            ShowDetails(slot.Item, quantity);
        }

        private void ShowDetails(ItemData itemData, int quantity)
        {
            selectedItem = itemData;

            if (itemNameText != null)
            {
                itemNameText.text = itemData?.GetLocalizedName() ?? string.Empty;
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
            ClearFocusedSlot();
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
            private readonly Selectable selectable;
            private readonly EventTrigger eventTrigger;
            private readonly Outline selectionOutline;

            public ItemData Item { get; private set; }

            public SlotRuntime(RectTransform anchor, InventoryPanelView owner)
            {
                this.owner = owner;
                slotAnchor = anchor;

                selectable = EnsureSelectable(anchor);
                selectionOutline = EnsureSelectionOutline(anchor);
                if (selectionOutline != null)
                {
                    selectionOutline.effectColor = Color.yellow;
                    selectionOutline.effectDistance = new Vector2(3.5f, -3.5f);
                    selectionOutline.useGraphicAlpha = false;
                    selectionOutline.enabled = false;
                }
                eventTrigger = anchor.GetComponent<EventTrigger>() ?? anchor.gameObject.AddComponent<EventTrigger>();
                iconImage = CreateIcon(anchor, owner);

                RegisterFocusCallbacks();
                Clear();
            }

            public void Bind(ItemData itemData)
            {
                Item = itemData;

                owner.ConfigureIconRect(slotAnchor, iconImage.rectTransform);

                if (selectable != null)
                {
                    selectable.interactable = itemData != null;
                }

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

            }

            public void Clear()
            {
                Item = null;
                iconImage.enabled = false;
                SetSelectionState(false);

                if (selectable != null)
                {
                    selectable.interactable = false;
                }
            }

            public void SetSelectionState(bool isSelected)
            {
                if (selectionOutline != null)
                {
                    selectionOutline.enabled = isSelected;
                }
            }

            public void Focus()
            {
                if (Item == null)
                {
                    return;
                }

                bool selectionApplied = false;

                if (selectable != null && selectable.IsActive() && selectable.IsInteractable())
                {
                    EventSystem current = EventSystem.current;
                    if (current != null)
                    {
                        selectable.Select();
                        selectionApplied = true;
                    }
                }

                if (!selectionApplied)
                {
                    owner.HandleSlotFocused(this);
                }
            }

            private void RegisterFocusCallbacks()
            {
                if (eventTrigger == null)
                {
                    return;
                }

                if (eventTrigger.triggers == null)
                {
                    eventTrigger.triggers = new List<EventTrigger.Entry>();
                }

                RegisterEvent(EventTriggerType.PointerEnter);
                RegisterEvent(EventTriggerType.Select);
                RegisterEvent(EventTriggerType.PointerClick);
            }

            private void RegisterEvent(EventTriggerType eventType)
            {
                var entry = new EventTrigger.Entry { eventID = eventType };
                entry.callback.AddListener(_ => HandleFocusEvent());
                eventTrigger.triggers.Add(entry);
            }

            private void HandleFocusEvent()
            {
                if (Item == null)
                {
                    owner.ShowEmptyDetails();
                    return;
                }

                owner.HandleSlotFocused(this);
            }

            private static Selectable EnsureSelectable(RectTransform anchor)
            {
                if (anchor == null)
                {
                    return null;
                }

                Selectable slotSelectable = anchor.GetComponent<Selectable>();
                if (slotSelectable != null)
                {
                    return slotSelectable;
                }

                Button button = anchor.gameObject.AddComponent<Button>();
                button.transition = Selectable.Transition.None;

                Image existingImage = anchor.GetComponent<Image>();
                if (existingImage == null)
                {
                    existingImage = anchor.gameObject.AddComponent<Image>();
                    existingImage.color = Color.clear;
                }

                button.targetGraphic = existingImage;

                return button;
            }

            private static Outline EnsureSelectionOutline(RectTransform anchor)
            {
                if (anchor == null)
                {
                    return null;
                }

                Graphic graphic = anchor.GetComponent<Graphic>();
                if (graphic == null)
                {
                    Image fallbackImage = anchor.gameObject.AddComponent<Image>();
                    fallbackImage.color = Color.clear;
                    graphic = fallbackImage;
                }

                Outline outline = anchor.GetComponent<Outline>();
                if (outline == null)
                {
                    outline = anchor.gameObject.AddComponent<Outline>();
                }

                return outline;
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

        }
    }
}
