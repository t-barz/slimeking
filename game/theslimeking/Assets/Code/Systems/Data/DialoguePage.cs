using System;

namespace SlimeMec.Systems
{
    /// <summary>
    /// Representa uma página individual de diálogo.
    /// </summary>
    [Serializable]
    public class DialoguePage
    {
        /// <summary>
        /// Texto da página de diálogo.
        /// </summary>
        public string text;

        /// <summary>
        /// Nome do personagem falando (opcional).
        /// </summary>
        public string speakerName;

        /// <summary>
        /// ID do sprite do personagem (opcional).
        /// </summary>
        public string speakerSprite;

        public DialoguePage()
        {
            text = string.Empty;
            speakerName = string.Empty;
            speakerSprite = string.Empty;
        }

        public DialoguePage(string text)
        {
            this.text = text;
            speakerName = string.Empty;
            speakerSprite = string.Empty;
        }
    }
}
