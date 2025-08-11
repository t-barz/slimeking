using UnityEngine;
using UnityEngine.InputSystem;
using SlimeKing.Core;
using SlimeKing.UI;
using UnityEngine.UI;

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

        [Tooltip("Descrição da ação que o botão executa")]
        [SerializeField] private string actionDescription = "Interagir";

        [Header("Input System")]
        [Tooltip("Referência à ação de interação")]
        [SerializeField] private InputActionReference interactAction;

        [Header("Efeito Visual")]
        [SerializeField] private GameObject visualCue;
        [SerializeField] private SpriteRenderer visualCueRenderer;

        private GameObject currentPrompt; private void Start()
        {
            // Inicializa o renderer e garante que começa oculto
            if (visualCue != null)
            {
                visualCueRenderer = visualCue.GetComponent<SpriteRenderer>();
                if (visualCueRenderer != null)
                {
                    visualCueRenderer.enabled = false;
                }
            }
        }

        protected override void ShowVisualFeedback()
        {
            // Ativa o feedback visual
            if (visualCueRenderer == null && visualCue != null)
            {
                visualCueRenderer = visualCue.GetComponent<SpriteRenderer>();
            }

            if (visualCueRenderer != null)
            {
                visualCueRenderer.enabled = true;
            }

            // Cria o prompt de botão
            if (buttonPromptPrefab != null)
            {
                Vector3 spawnPosition = transform.position + (Vector3)uiOffset;
                currentPrompt = Instantiate(buttonPromptPrefab, spawnPosition, Quaternion.identity);

                // Set input type based on the action
                var inputController = currentPrompt.GetComponent<InputController>();
                if (inputController != null)
                {
                    inputController.SetInputType(InputController.InputType.Action2);
                    // Atualiza o texto de ação se houver
                    var promptText = currentPrompt.GetComponentInChildren<Text>();
                    if (promptText != null && interactAction != null)
                    {
                        promptText.text = actionDescription;
                    }
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
            // Desativa o feedback visual
            if (visualCueRenderer == null && visualCue != null)
            {
                visualCueRenderer = visualCue.GetComponent<SpriteRenderer>();
            }

            if (visualCueRenderer != null)
            {
                visualCueRenderer.enabled = false;
            }

            // Remove o prompt de botão
            if (currentPrompt != null)
            {
                Destroy(currentPrompt);
            }
        }

        /// <summary>
        /// Obtém o texto formatado para a tecla associada à ação
        /// </summary>
        private string GetActionBindingText(InputAction action)
        {
            if (action == null) return "?";

            // Verifica se estamos usando teclado/mouse ou gamepad
            bool isGamepad = Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame;

            // Obtém o binding path para o device ativo
            int bindingIndex = isGamepad ? 1 : 0; // Assume que 0 é teclado e 1 é gamepad
            if (bindingIndex >= action.bindings.Count)
                bindingIndex = 0;

            string bindingPath = action.bindings[bindingIndex].effectivePath;

            // Traduz o path para um texto amigável
            string displayString = InputControlPath.ToHumanReadableString(
                bindingPath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );

            return $"[{displayString}]";
        }

        public override void Interact()
        {
            Debug.Log($"Botão {actionDescription} pressionado!");
            // Implemente aqui o comportamento específico da interação
        }
    }
}
