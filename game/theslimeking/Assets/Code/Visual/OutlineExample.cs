using UnityEngine;
using SlimeMec.Visual;

namespace SlimeKing.Examples
{
    /// <summary>
    /// Exemplo simples de como usar o sistema de outline.
    /// Este script demonstra as principais funcionalidades do OutlineController.
    /// </summary>
    public class OutlineExample : MonoBehaviour
    {
        [Header("🔲 Outline Settings")]
        [Tooltip("Cor do outline")]
        [SerializeField] private Color outlineColor = Color.white;

        [Tooltip("Ativar detecção automática do player")]
        [SerializeField] private bool enableAutoDetection = true;

        [Tooltip("Raio de detecção (se auto detection estiver ativo)")]
        [SerializeField] private float detectionRadius = 2.0f;

        [Tooltip("Espessura do outline")]
        [SerializeField, Range(0.01f, 0.1f)] private float outlineSize = 0.04f;

        [Header("🎮 Manual Controls")]
        [Tooltip("Tecla para ativar/desativar outline manualmente")]
        [SerializeField] private KeyCode toggleKey = KeyCode.Space;

        [Tooltip("Mostrar logs de debug")]
        [SerializeField] private bool showDebugLogs = true;

        private OutlineController outlineController;

        void Start()
        {
            SetupOutline();
        }

        void Update()
        {
            HandleManualControls();
        }

        private void SetupOutline()
        {
            // Configura o outline usando o utilitário
            if (enableAutoDetection)
            {
                outlineController = SlimeKing.Visual.OutlineUtility.SetupAutoOutline(gameObject, outlineColor, detectionRadius);
            }
            else
            {
                outlineController = SlimeKing.Visual.OutlineUtility.SetupManualOutline(gameObject, outlineColor);
            }

            if (outlineController == null)
            {
                if (showDebugLogs)
                    Debug.LogError($"[OutlineExample] Falha ao configurar outline em '{gameObject.name}'");
                enabled = false;
                return;
            }

            // Aplica configurações adicionais
            outlineController.UpdateOutlineSize(outlineSize);

            if (showDebugLogs)
            {
                Debug.Log($"[OutlineExample] ✅ Outline configurado em '{gameObject.name}'");
                if (!enableAutoDetection)
                {
                    Debug.Log($"[OutlineExample] Pressione '{toggleKey}' para testar o outline");
                }
            }
        }

        private void HandleManualControls()
        {
            if (!enableAutoDetection && Input.GetKeyDown(toggleKey))
            {
                if (outlineController != null)
                {
                    outlineController.ToggleOutline();

                    if (showDebugLogs)
                    {
                        string status = outlineController.IsOutlineActive ? "ativado" : "desativado";
                        Debug.Log($"[OutlineExample] Outline {status}");
                    }
                }
            }
        }

        /// <summary>
        /// Métodos públicos para serem chamados por outros scripts ou eventos de UI
        /// </summary>
        public void ActivateOutline()
        {
            if (outlineController != null)
            {
                outlineController.ActivateOutline();
            }
        }

        public void DeactivateOutline()
        {
            if (outlineController != null)
            {
                outlineController.DeactivateOutline();
            }
        }

        public void ToggleOutline()
        {
            if (outlineController != null)
            {
                outlineController.ToggleOutline();
            }
        }

        public void ChangeOutlineColor(Color newColor)
        {
            outlineColor = newColor;
            if (outlineController != null)
            {
                outlineController.UpdateOutlineColor(newColor);
            }
        }

        // Métodos para o Inspector
        void OnValidate()
        {
            if (Application.isPlaying && outlineController != null)
            {
                outlineController.UpdateOutlineColor(outlineColor);
                outlineController.UpdateOutlineSize(outlineSize);
                outlineController.SetDetectionRadius(detectionRadius);
                outlineController.SetAutoDetection(enableAutoDetection);
            }
        }
    }
}