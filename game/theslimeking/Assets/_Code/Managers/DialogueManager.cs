using UnityEngine;
using UnityEngine.Localization.Settings;
using SlimeKing.Gameplay;

namespace SlimeKing.Core
{
    /// <summary>
    /// Global dialogue orchestrator. Coordinates localization and the HUD panel.
    /// </summary>
    public class DialogueManager : ManagerSingleton<DialogueManager>
    {
        #region Fields
        [Header("UI")]
        [SerializeField] private SlimeKing.Visual.DialogueUIController ui;

        [Header("Localization")]
        [SerializeField] private string dialogueTable = "DialogueTexts";

        [Header("Pagination")]
        [SerializeField] private int maxCharactersPerPage = 120;

        private readonly DialogueState state = new DialogueState();
        #endregion

        #region Unity Lifecycle
        protected override void Initialize()
        {
            EnsureUIRef();
            state.Reset();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Check if a dialogue is currently active.
        /// </summary>
        public bool IsDialogueActive => state.isActive;

        /// <summary>
        /// Start a dialogue by direct localization text key (no database lookup).
        /// Key format: "Table/Entry" (e.g., "DialogueTexts/NPC001") or plain entry using dialogueTable.
        /// </summary>
        public void StartDialogueByTextKey(string npcNameKey, string textKey)
        {
            if (string.IsNullOrEmpty(npcNameKey) || string.IsNullOrEmpty(textKey)) return;

            state.Reset();
            state.isActive = true;
            state.npcId = npcNameKey;
            state.currentDialogueId = textKey;

            var npcName = GetNPCNameLocalized(npcNameKey);
            state.npcLocalizedName = npcName;

            var localizedText = GetDialogueTextLocalized(textKey);
            state.pages = DialoguePageManager.GetPages(localizedText, maxCharactersPerPage);
            state.currentPageIndex = 0;

            if (ui != null)
            {
                ui.Show();
                ui.SetNPCName(npcName);
                ui.DisplayText(state.pages[0]);
            }
        }

        /// <summary>
        /// Advance to next page or close the dialogue.
        /// </summary>
        public void Advance()
        {
            if (!state.isActive) return;

            if (state.IsLastPage())
            {
                CloseDialogue();
                return;
            }

            state.currentPageIndex++;
            if (ui != null)
            {
                var page = state.pages[state.currentPageIndex];
                ui.DisplayText(page);
            }
        }

        /// <summary>
        /// Close and hide the dialogue panel.
        /// </summary>
        public void CloseDialogue()
        {
            if (!state.isActive) return;
            state.Reset();
            if (ui != null)
            {
                ui.Hide();
            }
        }
        #endregion

        #region Private Methods
        private void EnsureUIRef()
        {
            if (ui == null)
            {
                ui = FindDialogueControllerEvenIfInactive();
            }

            if (ui != null)
            {
                ui.AutoBindIfNull();
                // Don't call Hide here - let the controller handle its initial state
            }
        }

        private SlimeKing.Visual.DialogueUIController FindDialogueControllerEvenIfInactive()
        {
            // HUD starts inactive, so search including disabled objects.
            var controllers = Resources.FindObjectsOfTypeAll<SlimeKing.Visual.DialogueUIController>();
            foreach (var controller in controllers)
            {
                var go = controller.gameObject;
                if (!go.scene.IsValid() || !go.scene.isLoaded)
                {
                    continue;
                }

                return controller;
            }

            var hud = GameObject.Find("GameHUD");
            if (hud == null)
            {
                return null;
            }

            var panel = hud.transform.Find("NPCDialoguePanel");
            if (panel == null)
            {
                return null;
            }

            var controllerFound = panel.GetComponent<SlimeKing.Visual.DialogueUIController>();
            if (controllerFound != null)
            {
                return controllerFound;
            }

            return panel.gameObject.AddComponent<SlimeKing.Visual.DialogueUIController>();
        }

        private string GetNPCNameLocalized(string npcNameKey)
        {
            if (string.IsNullOrEmpty(npcNameKey))
                return string.Empty;

            string table = "NPCsNames";
            string entry = npcNameKey;

            var slashIndex = npcNameKey.IndexOf('/');
            if (slashIndex > 0)
            {
                table = npcNameKey.Substring(0, slashIndex);
                entry = npcNameKey.Substring(slashIndex + 1);
            }

            if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(entry))
                return string.Empty;

            var localized = LocalizationSettings.StringDatabase.GetLocalizedString(table, entry);
            return localized ?? entry;
        }

        /// <summary>
        /// Supports keys in the form "Table/Entry" or plain "Entry" (using dialogueTable).
        /// </summary>
        private string GetDialogueTextLocalized(string keyOrTableAndKey)
        {
            if (string.IsNullOrEmpty(keyOrTableAndKey)) return string.Empty;

            string table = dialogueTable;
            string entry = keyOrTableAndKey;

            var slashIndex = keyOrTableAndKey.IndexOf('/');
            if (slashIndex > 0)
            {
                table = keyOrTableAndKey.Substring(0, slashIndex);
                entry = keyOrTableAndKey.Substring(slashIndex + 1);
            }

            if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(entry))
                return string.Empty;

            var localized = LocalizationSettings.StringDatabase.GetLocalizedString(table, entry);
            return localized ?? string.Empty;
        }
        #endregion
    }
}
