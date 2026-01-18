using System.Collections.Generic;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Represents the transient state of an active dialogue session.
    /// </summary>
    public class DialogueState
    {
        #region Fields
        public string currentDialogueId;
        public string npcId;
        public string npcLocalizedName;
        public List<string> pages = new List<string>();
        public int currentPageIndex;
        public bool isActive;
        #endregion

        #region Public Methods
        public void Reset()
        {
            currentDialogueId = null;
            npcId = null;
            npcLocalizedName = null;
            pages.Clear();
            currentPageIndex = 0;
            isActive = false;
        }

        public bool IsLastPage()
        {
            return pages.Count == 0 || currentPageIndex >= pages.Count - 1;
        }
        #endregion
    }
}
