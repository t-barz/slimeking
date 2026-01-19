using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Provides smart pagination utilities for localized dialogue text.
    /// Breaks by sentence for better readability, then by word if sentences are too long.
    /// </summary>
    public static class DialoguePageManager
    {
        #region Public Methods
        /// <summary>
        /// Paginate text intelligently: by sentence first, then by word if necessary.
        /// </summary>
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

            // Split by sentence (period, exclamation, question mark)
            var sentences = SplitBySentence(fullText);
            
            var currentPage = "";
            
            foreach (var sentence in sentences)
            {
                var trimmed = sentence.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                // If adding this sentence would exceed limit
                if (!string.IsNullOrEmpty(currentPage) && (currentPage.Length + trimmed.Length + 1) > maxCharactersPerPage)
                {
                    // Flush current page
                    pages.Add(currentPage.Trim());
                    currentPage = "";
                }

                // If sentence itself is longer than max, break it by word
                if (trimmed.Length > maxCharactersPerPage && string.IsNullOrEmpty(currentPage))
                {
                    var sentencePages = SplitSentenceByWord(trimmed, maxCharactersPerPage);
                    for (int i = 0; i < sentencePages.Count - 1; i++)
                    {
                        pages.Add(sentencePages[i]);
                    }
                    currentPage = sentencePages[sentencePages.Count - 1];
                }
                else
                {
                    currentPage += (string.IsNullOrEmpty(currentPage) ? trimmed : " " + trimmed);
                }
            }

            // Add remaining content
            if (!string.IsNullOrEmpty(currentPage))
            {
                pages.Add(currentPage.Trim());
            }

            return pages.Count > 0 ? pages : new List<string> { fullText };
        }
        #endregion

        #region Utility Methods
        private static List<string> SplitBySentence(string text)
        {
            var sentences = new List<string>();
            // Match sentence ending with . ! or ?
            var matches = Regex.Matches(text, @"[^.!?]*[.!?]+");
            
            if (matches.Count == 0)
            {
                // No sentence terminators found, return whole text
                sentences.Add(text);
                return sentences;
            }

            foreach (Match match in matches)
            {
                sentences.Add(match.Value);
            }

            // Handle any remaining text after last terminator
            var lastMatch = matches[matches.Count - 1];
            var remaining = text.Substring(lastMatch.Index + lastMatch.Length).Trim();
            if (!string.IsNullOrEmpty(remaining))
            {
                sentences.Add(remaining);
            }

            return sentences;
        }

        private static List<string> SplitSentenceByWord(string sentence, int maxChars)
        {
            var pages = new List<string>();
            var words = sentence.Split(' ');
            var currentLine = "";

            foreach (var word in words)
            {
                var next = (string.IsNullOrEmpty(currentLine) ? word : " " + word);
                
                if (currentLine.Length + next.Length > maxChars && !string.IsNullOrEmpty(currentLine))
                {
                    pages.Add(currentLine.Trim());
                    currentLine = word;
                }
                else
                {
                    currentLine += next;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                pages.Add(currentLine.Trim());
            }

            return pages;
        }
        #endregion
    }
}
