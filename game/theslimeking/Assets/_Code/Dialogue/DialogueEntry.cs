using System;
using System.Collections.Generic;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Data model for a single dialogue entry.
    /// </summary>
    [Serializable]
    public class DialogueEntry
    {
        #region Fields
        public string id;
        public string npcId;
        public string textKey; // Localization key for the main text
        public string audioClipKey; // Non-localized audio key/asset reference
        public string nextDialogueId;
        public List<string> prerequisites = new List<string>();
        public List<string> flags_to_activate = new List<string>();
        public List<string> flags_to_deactivate = new List<string>();
        #endregion
    }
}
