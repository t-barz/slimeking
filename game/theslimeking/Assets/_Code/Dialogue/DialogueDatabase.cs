using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Lightweight dialogue repository with optional JSON loading via Resources.
    /// </summary>
    [Serializable]
    public class DialogueDatabase
    {
        #region Fields
        [Serializable]
        private class DialogueContainer
        {
            public List<DialogueEntry> dialogues = new List<DialogueEntry>();
        }

        private readonly Dictionary<string, DialogueEntry> dialogues = new Dictionary<string, DialogueEntry>();
        #endregion

        #region Public Methods
        public void Clear()
        {
            dialogues.Clear();
        }

        public void LoadFromTextAsset(TextAsset json)
        {
            dialogues.Clear();
            if (json == null || string.IsNullOrEmpty(json.text)) return;

            var container = JsonUtility.FromJson<DialogueContainer>(json.text);
            if (container?.dialogues == null) return;

            foreach (var d in container.dialogues)
            {
                if (!string.IsNullOrEmpty(d.id))
                {
                    dialogues[d.id] = d;
                }
            }
        }

        public void LoadFromResources(string resourcePath)
        {
            var ta = Resources.Load<TextAsset>(resourcePath);
            LoadFromTextAsset(ta);
        }

        public bool TryGetDialogueEntry(string dialogueId, out DialogueEntry entry)
        {
            return dialogues.TryGetValue(dialogueId, out entry);
        }
        #endregion
    }
}
