using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

namespace SlimeKing.Visual
{
    /// <summary>
    /// Controls the in-game dialogue UI (NPC name + text + simple paging).
    /// Attach to the GameHUD/NPCDialoguePanel object and bind references.
    /// </summary>
    public class DialogueUIController : MonoBehaviour
    {
        #region Fields
        [Header("Bindings")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TMP_Text npcNameText;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private LocalizeStringEvent dialogueLocalizeEvent;
        [Header("Animation")]
        [SerializeField] private float fadeInOutDuration = 0.15f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (dialoguePanel == null) dialoguePanel = gameObject;
            HideImmediate();
        }
        #endregion

        #region Public Methods
        /// <summary>Show the dialogue panel.</summary>
        public void Show()
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(true);
        }

        /// <summary>Hide the dialogue panel.</summary>
        public void Hide()
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }

        /// <summary>Set NPC localized name.</summary>
        public void SetNPCName(string nameText)
        {
            if (npcNameText != null) npcNameText.text = nameText ?? string.Empty;
        }

        /// <summary>Display localized text by raw string (already localized).</summary>
        public void DisplayText(string localizedText)
        {
            if (dialogueText != null) dialogueText.text = localizedText ?? string.Empty;
        }

        /// <summary>Display via LocalizeStringEvent (table + entry key). Optional, if wired.</summary>
        public void DisplayLocalizedEntry(string tableCollection, string entryKey)
        {
            if (dialogueLocalizeEvent == null) return;
            var r = dialogueLocalizeEvent.StringReference;
            r.TableReference = tableCollection;
            r.TableEntryReference = entryKey;
            dialogueLocalizeEvent.RefreshString();
        }
        #endregion

        #region Private Methods
        private void HideImmediate()
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Auto-binds TMP components from known child names if fields are null.
        /// </summary>
        public void AutoBindIfNull()
        {
            if (npcNameText == null)
            {
                var nameTr = transform.Find("NPCName");
                if (nameTr != null) npcNameText = nameTr.GetComponent<TMP_Text>();
            }
            if (dialogueText == null)
            {
                var textTr = transform.Find("NPCDialogueText");
                if (textTr != null) dialogueText = textTr.GetComponent<TMP_Text>();
            }
        }
        #endregion
    }
}
