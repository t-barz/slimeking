using UnityEngine;
using SlimeKing.Core;
using SlimeKing.UI;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Implementação para objetos que exigem prompt de botão na UI
    /// </summary>
    public class ButtonPromptInteractable : Interactable
    {
        [Header("Configurações de UI")]
        [Tooltip("Prefab com elemento UI do botão")]
        [SerializeField] private GameObject buttonPromptPrefab;

        [Tooltip("Offset de posição do prompt")]
        [SerializeField] private Vector2 uiOffset = new Vector2(0, 1.5f);

        private GameObject currentPrompt;

        protected override void ShowVisualFeedback()
        {
            base.ShowVisualFeedback();
            if (buttonPromptPrefab != null)
            {
                Vector3 spawnPosition = transform.position + (Vector3)uiOffset;
                currentPrompt = Instantiate(buttonPromptPrefab, spawnPosition, Quaternion.identity);

                // Set input type to Action2
                var inputController = currentPrompt.GetComponent<InputController>();
                if (inputController != null)
                {
                    inputController.SetInputType(InputController.InputType.Action2);
                }

                // Disable AnchorOnScreen component if it exists
                var anchorComponent = currentPrompt.GetComponent<AnchorOnScreen>();
                if (anchorComponent != null)
                {
                    anchorComponent.enabled = false;
                }
            }
        }

        protected override void HideVisualFeedback()
        {
            base.HideVisualFeedback();
            if (currentPrompt != null)
            {
                Destroy(currentPrompt);
            }
        }

        public override void Interact()
        {
        }
    }
}
