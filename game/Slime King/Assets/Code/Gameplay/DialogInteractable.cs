using UnityEngine;
using System.Collections.Generic;
using SlimeKing.UI;
using SlimeKing.Core;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Componente para objetos e NPCs que podem mostrar diálogos quando interagidos.
    /// </summary>
    public class DialogInteractable : Interactable
    {
        [Header("Configurações de Diálogo")]
        [Tooltip("Título do diálogo (nome do NPC ou item)")]
        [SerializeField] private string dialogTitle = "";

        [Tooltip("O texto de diálogo a ser exibido")]
        [TextArea(3, 10)]
        [SerializeField] private string dialogText;

        [Tooltip("Ícone opcional do falante")]
        [SerializeField] private Sprite speakerIcon;

        [Tooltip("Lista de diálogos para conversas mais complexas")]
        [SerializeField] private List<string> dialogSequence;

        [Tooltip("Lista de títulos para diálogos sequenciais")]
        [SerializeField] private List<string> titleSequence;

        [Tooltip("Efeito visual de interatividade")]
        [SerializeField] private GameObject visualCue;

        private int currentDialogIndex = 0;
        private SpriteRenderer visualCueRenderer; // Referência para o SpriteRenderer

        private void Start()
        {
            // Inicializa o renderer e garante que começa oculto
            if (visualCue != null)
            {
                visualCueRenderer = visualCue.GetComponent<SpriteRenderer>();
                if (visualCueRenderer != null)
                {
                    visualCueRenderer.enabled = false;
                }
                else
                {
                    visualCue.SetActive(false);
                    Debug.LogWarning("Visual cue GameObject does not have a SpriteRenderer component. Disabling visual cue.");
                }
            }
        }

        /// <summary>
        /// Implementação da interação para exibir um diálogo
        /// </summary>
        public override void Interact()
        {
            // Se tivermos uma sequência de diálogos, usamos ela
            if (dialogSequence != null && dialogSequence.Count > 0)
            {
                ShowSequentialDialog();
            }
            // Caso contrário, mostramos o diálogo único
            else if (!string.IsNullOrEmpty(dialogText))
            {
                ShowSingleDialog();
            }
        }        /// <summary>
                 /// Exibe um único diálogo
                 /// </summary>
        private void ShowSingleDialog()
        {
            if (SlimeKing.UI.DialogManager.Instance != null)
            {
                // Se já temos um diálogo ativo, fechamos
                if (SlimeKing.UI.DialogManager.Instance.IsDialogActive())
                {
                    SlimeKing.UI.DialogManager.Instance.CloseDialog();
                }
                else
                {
                    // Caso contrário, mostramos o novo diálogo
                    SlimeKing.UI.DialogManager.Instance.ShowDialog(dialogText, dialogTitle, speakerIcon);
                }
            }
        }

        /// <summary>
        /// Exibe uma sequência de diálogos, avançando um de cada vez
        /// </summary>
        private void ShowSequentialDialog()
        {
            if (SlimeKing.UI.DialogManager.Instance == null || dialogSequence.Count == 0)
                return;

            // Se tem um diálogo ativo
            if (SlimeKing.UI.DialogManager.Instance.IsDialogActive())
            {
                // Avança para o próximo diálogo na sequência
                currentDialogIndex++;

                // Se chegou ao final, fecha o diálogo e reseta
                if (currentDialogIndex >= dialogSequence.Count)
                {
                    SlimeKing.UI.DialogManager.Instance.CloseDialog();
                    currentDialogIndex = 0;
                    return;
                }                // Determina o título para este diálogo
                string currentTitle = dialogTitle;
                if (titleSequence != null && titleSequence.Count > currentDialogIndex)
                {
                    currentTitle = titleSequence[currentDialogIndex];
                }

                // Mostra o próximo diálogo
                SlimeKing.UI.DialogManager.Instance.ShowDialog(
                    dialogSequence[currentDialogIndex],
                    currentTitle,
                    speakerIcon
                );
            }
            else
            {                // Inicia do primeiro diálogo
                currentDialogIndex = 0;

                // Determina o título para o primeiro diálogo
                string currentTitle = dialogTitle;
                if (titleSequence != null && titleSequence.Count > 0)
                {
                    currentTitle = titleSequence[0];
                }

                SlimeKing.UI.DialogManager.Instance.ShowDialog(
                    dialogSequence[currentDialogIndex],
                    currentTitle,
                    speakerIcon
                );
            }
        }        /// <summary>
                 /// Mostra o feedback visual quando o jogador está perto
                 /// </summary>
        protected override void ShowVisualFeedback()
        {
            // Verifica se já temos a referência para o SpriteRenderer
            if (visualCueRenderer == null && visualCue != null)
            {
                visualCueRenderer = visualCue.GetComponent<SpriteRenderer>();
            }

            // Ativa apenas o SpriteRenderer, mantendo o GameObject sempre ativo
            if (visualCueRenderer != null)
            {
                visualCueRenderer.enabled = true;
            }
        }        /// <summary>
                 /// Oculta o feedback visual quando o jogador se afasta
                 /// </summary>
        protected override void HideVisualFeedback()
        {
            // Verifica se já temos a referência para o SpriteRenderer
            if (visualCueRenderer == null && visualCue != null)
            {
                visualCueRenderer = visualCue.GetComponent<SpriteRenderer>();
            }

            // Desativa apenas o SpriteRenderer, mantendo o GameObject sempre ativo
            if (visualCueRenderer != null)
            {
                visualCueRenderer.enabled = false;
            }

            // Assegura que o diálogo seja fechado se o jogador sair da área
            if (SlimeKing.UI.DialogManager.Instance != null && SlimeKing.UI.DialogManager.Instance.IsDialogActive())
            {
                SlimeKing.UI.DialogManager.Instance.CloseDialog();
                currentDialogIndex = 0;
            }
        }
    }
}
