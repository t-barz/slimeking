using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using SlimeKing.Gameplay;

namespace SlimeKing.Core
{
    /// <summary>
    /// Global dialogue orchestrator. Coordinates data, localization and the HUD panel.
    /// </summary>
    public class DialogueManager : ManagerSingleton<DialogueManager>
    {
        #region Fields
        [Header("UI")]
        [SerializeField] private SlimeKing.Visual.DialogueUIController ui;

        [Header("Localization")]
        [SerializeField] private string dialogueTable = "DialogueTexts";

        [Header("Database")] 
        [SerializeField] private TextAsset dialoguesJson; // Optional: assign to load at startup
        [SerializeField] private string resourcesJsonPath = "Dialogues/dialogues"; // Resources path without extension

        [Header("Pagination")]
        [SerializeField] private int maxCharactersPerPage = 200;

        private readonly DialogueDatabase database = new DialogueDatabase();
        private readonly DialogueState state = new DialogueState();
        #endregion

        #region Unity Lifecycle
        protected override void Initialize()
        {
            LoadDatabase();
            EnsureUIRef();
            state.Reset();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start a dialogue by NPC and entry ID (from database).
        /// </summary>
        public void StartDialogue(string npcId, string dialogueId)
        {
            if (string.IsNullOrEmpty(npcId) || string.IsNullOrEmpty(dialogueId)) return;
            if (!database.TryGetDialogueEntry(dialogueId, out var entry)) return;

            state.Reset();
            state.isActive = true;
            state.npcId = npcId;
            state.currentDialogueId = dialogueId;

            var npcName = GetNPCNameLocalized(npcId);
            state.npcLocalizedName = npcName;

            var localizedText = GetDialogueTextLocalized(entry.textKey);
            state.pages = DialoguePageManager.GetPages(localizedText, maxCharactersPerPage);
            state.currentPageIndex = 0;

            if (ui != null)
            {
                ui.Show();
                ui.SetNPCName(npcName);
                ui.DisplayText(state.pages[0]);
                ui.UpdatePageIndicator(1, state.pages.Count);
            }
        }

        /// <summary>
        /// Start a dialogue by direct localization text key (no database lookup).
        /// Useful for simple NPC interactions without full DialogueEntry data.
        /// </summary>
        public void StartDialogueByTextKey(string npcId, string textKey)
        {
            if (string.IsNullOrEmpty(npcId) || string.IsNullOrEmpty(textKey)) return;

            state.Reset();
            state.isActive = true;
            state.npcId = npcId;
            state.currentDialogueId = textKey; // Use textKey as fallback ID

            var npcName = GetNPCNameLocalized(npcId);
            state.npcLocalizedName = npcName;

            var localizedText = GetDialogueTextLocalized(textKey);
            state.pages = DialoguePageManager.GetPages(localizedText, maxCharactersPerPage);
            state.currentPageIndex = 0;

            if (ui != null)
            {
                ui.Show();
                ui.SetNPCName(npcName);
                ui.DisplayText(state.pages[0]);
                ui.UpdatePageIndicator(1, state.pages.Count);
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
                ui.UpdatePageIndicator(state.currentPageIndex + 1, state.pages.Count);
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
        private void LoadDatabase()
        {
            if (dialoguesJson != null)
            {
                database.LoadFromTextAsset(dialoguesJson);
                return;
            }

            if (!string.IsNullOrEmpty(resourcesJsonPath))
            {
                database.LoadFromResources(resourcesJsonPath);
            }
        }

        private void EnsureUIRef()
        {
            // Try existing reference
            if (ui == null)
            {
                var go = GameObject.Find("GameHUD");
                if (go != null)
                {
                    var panel = go.transform.Find("NPCDialoguePanel");
                    if (panel != null)
                    {
                        ui = panel.GetComponent<SlimeKing.Visual.DialogueUIController>();
                        if (ui == null)
                        {
                            ui = panel.gameObject.AddComponent<SlimeKing.Visual.DialogueUIController>();
                        }
                    }
                }
            }

            // Ensure bindings and hidden state
            if (ui != null)
            {
                ui.AutoBindIfNull();
                ui.Hide();
            }
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
