using UnityEngine;
using TMPro;

namespace SlimeKing.UI
{
    /// <summary>
    /// Gerencia a exibição da caixa de diálogo na tela.
    /// </summary>
    public class DialogueUIManager : MonoBehaviour
    {
        public static DialogueUIManager Instance { get; private set; }

        [Header("Referências UI")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TMP_Text dialogueText;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            HideDialogue();
        }

        /// <summary>
        /// Exibe o painel de diálogo com o texto informado.
        /// </summary>
        public void ShowDialogue(string text)
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(true);
            if (dialogueText != null)
                dialogueText.text = text;
        }

        /// <summary>
        /// Oculta o painel de diálogo.
        /// </summary>
        public void HideDialogue()
        {
            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }
    }
}