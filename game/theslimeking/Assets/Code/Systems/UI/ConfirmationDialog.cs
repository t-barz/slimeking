using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace TheSlimeKing.UI
{
    /// <summary>
    /// Diálogo de confirmação genérico com botões Sim/Não.
    /// </summary>
    public class ConfirmationDialog : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI messageText;

        [Header("Buttons")]
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        private Action onConfirm;
        private Action onCancel;

        private void Awake()
        {
            // Configura os listeners dos botões
            if (yesButton != null)
            {
                yesButton.onClick.AddListener(OnYesClicked);
            }

            if (noButton != null)
            {
                noButton.onClick.AddListener(OnNoClicked);
            }

            // Começa oculto
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        /// <summary>
        /// Mostra o diálogo de confirmação.
        /// </summary>
        /// <param name="message">Mensagem a exibir</param>
        /// <param name="onConfirmCallback">Callback quando Sim é clicado</param>
        /// <param name="onCancelCallback">Callback quando Não é clicado (opcional)</param>
        public void Show(string message, Action onConfirmCallback, Action onCancelCallback = null)
        {
            // Atualiza a mensagem
            if (messageText != null)
            {
                messageText.text = message;
            }

            // Armazena os callbacks
            onConfirm = onConfirmCallback;
            onCancel = onCancelCallback;

            // Mostra o painel
            if (panel != null)
            {
                panel.SetActive(true);
            }
        }

        /// <summary>
        /// Oculta o diálogo de confirmação.
        /// </summary>
        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }

            onConfirm = null;
            onCancel = null;
        }

        /// <summary>
        /// Callback quando o botão Sim é clicado.
        /// </summary>
        private void OnYesClicked()
        {
            onConfirm?.Invoke();
            Hide();
        }

        /// <summary>
        /// Callback quando o botão Não é clicado.
        /// </summary>
        private void OnNoClicked()
        {
            onCancel?.Invoke();
            Hide();
        }

        private void OnDestroy()
        {
            if (yesButton != null)
            {
                yesButton.onClick.RemoveListener(OnYesClicked);
            }

            if (noButton != null)
            {
                noButton.onClick.RemoveListener(OnNoClicked);
            }
        }
    }
}
