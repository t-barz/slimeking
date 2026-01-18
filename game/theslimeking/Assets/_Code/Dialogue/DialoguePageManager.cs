using System;
using System.Collections.Generic;
using System.Text;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Provides simple pagination utilities for localized dialogue text.
    /// </summary>
    public static class DialoguePageManager
    {
        #region Public Methods
        public static List<string> GetPages(string fullText, int maxCharactersPerPage = 300, int maxLinesPerPage = 0, char pageBreak = '|')
        {
            var pages = new List<string>();
            if (string.IsNullOrEmpty(fullText))
            {
                pages.Add(string.Empty);
                return pages;
            }

            // Explicit manual breaks take precedence.
            if (fullText.IndexOf(pageBreak) >= 0)
            {
                var manual = fullText.Split(pageBreak);
                foreach (var p in manual)
                {
                    pages.Add(p.Trim());
                }
                return pages;
            }

            // Naive word-wrapping by character budget.
            var words = fullText.Split(new[] { ' ' }, StringSplitOptions.None);
            var sb = new StringBuilder();
            var lineCount = 0;

            void FlushPage()
            {
                if (sb.Length > 0)
                {
                    pages.Add(sb.ToString().Trim());
                    sb.Clear();
                    lineCount = 0;
                }
            }

            foreach (var w in words)
            {
                var next = (sb.Length == 0 ? w : " " + w);
                if (sb.Length + next.Length > maxCharactersPerPage)
                {
                    FlushPage();
                }

                sb.Append(next);

                if (maxLinesPerPage > 0 && next.Contains("\n"))
                {
                    lineCount += CountLines(next);
                    if (lineCount >= maxLinesPerPage)
                    {
                        FlushPage();
                    }
                }
            }

            FlushPage();
            return pages.Count > 0 ? pages : new List<string> { fullText };
        }
        #endregion

        #region Utility Methods
        private static int CountLines(string s)
        {
            var count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '\n') count++;
            }
            return Math.Max(1, count);
        }
        #endregion
    }
}
