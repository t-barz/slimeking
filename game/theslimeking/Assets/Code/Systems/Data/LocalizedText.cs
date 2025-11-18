using System;
using System.Collections.Generic;

namespace SlimeMec.Systems
{
    /// <summary>
    /// Representa o texto localizado para um idioma específico.
    /// </summary>
    [Serializable]
    public class LocalizedText
    {
        /// <summary>
        /// Código do idioma (ex: "EN", "BR", "ES").
        /// </summary>
        public string language;

        /// <summary>
        /// Lista de páginas de diálogo para este idioma.
        /// </summary>
        public List<DialoguePage> pages;

        public LocalizedText()
        {
            language = "EN";
            pages = new List<DialoguePage>();
        }
    }
}
