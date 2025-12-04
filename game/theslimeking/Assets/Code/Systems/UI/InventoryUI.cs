using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SlimeKing.UI
{
    /// <summary>
    /// Gerencia a interface do inventário.
    /// Exibe 12 slots (3 linhas x 4 colunas) centralizados na tela.
    /// Controlado pelo PauseManager.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        #region Inspector Settings

        [Header("UI Panels")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 0.3f;

        [Header("Debug")]
        [SerializeField] private bool enableLogs = false;

        #endregion

        #region Private Fields

        private Coroutine fadeCoroutine;
        private bool isOpen = false;

        #endregion

        #region Properties

        public bool IsOpen => isOpen;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Valida referências
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    UnityEngine.Debug.LogError("[InventoryUI] CanvasGroup not found! Adding component.");
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            // Estado inicial: oculto
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Abre o inventário com fade in.
        /// </summary>
        public void Show()
        {
            if (isOpen) return;

            LogMessage("Showing inventory");
            isOpen = true;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
            }

            // Para fade anterior se existir
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeIn());
        }

        /// <summary>
        /// Fecha o inventário com fade out.
        /// </summary>
        public void Hide()
        {
            if (!isOpen) return;

            LogMessage("Hiding inventory");
            isOpen = false;

            // Para fade anterior se existir
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeOut());
        }

        #endregion

        #region Fade Animations

        /// <summary>
        /// Corrotina de fade in (alpha 0 → 1).
        /// Usa Time.unscaledDeltaTime para funcionar durante pause.
        /// </summary>
        private IEnumerator FadeIn()
        {
            float elapsed = 0f;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;

            fadeCoroutine = null;
            LogMessage("Fade in completed");
        }

        /// <summary>
        /// Corrotina de fade out (alpha 1 → 0).
        /// Usa Time.unscaledDeltaTime para funcionar durante pause.
        /// </summary>
        private IEnumerator FadeOut()
        {
            float elapsed = 0f;

            canvasGroup.interactable = false;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            fadeCoroutine = null;
            LogMessage("Fade out completed");
        }

        #endregion

        #region Slot Interaction

        /// <summary>
        /// Callback quando um slot é clicado.
        /// TODO: Implementar lógica de interação com slots.
        /// </summary>
        public void OnSlotClicked(int slotIndex)
        {
            LogMessage($"Slot {slotIndex} clicked");
            // TODO: Implementar lógica de uso/seleção de item
        }

        #endregion

        #region Logging

        private void LogMessage(string message)
        {
            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[InventoryUI] {message}");
            }
        }

        #endregion
    }
}
