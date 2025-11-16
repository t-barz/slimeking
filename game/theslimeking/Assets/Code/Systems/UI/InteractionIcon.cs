using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SlimeKing.Systems.UI
{
    /// <summary>
    /// Ícone visual que aparece acima do NPC indicando possibilidade de interação.
    /// Segue a posição do NPC em world space e exibe animações de fade e bounce.
    /// </summary>
    public class InteractionIcon : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform iconTransform;
        
        [Header("Animation Settings")]
        [SerializeField] private float fadeSpeed = 5f;
        [SerializeField] private float bounceSpeed = 2f;
        [SerializeField] private float bounceHeight = 10f;
        
        [Header("Positioning")]
        [SerializeField] private Vector3 worldOffset = new Vector3(0, 1.5f, 0);
        
        private Transform target;
        private Camera mainCamera;
        private bool isVisible;
        private float bounceTimer;
        private Vector3 baseLocalPosition;
        
        private void Awake()
        {
            // Validar componentes necessários
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    UnityEngine.Debug.LogWarning("[InteractionIcon] CanvasGroup not found. Adding component.");
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
            
            if (iconTransform == null)
            {
                iconTransform = GetComponent<RectTransform>();
            }
            
            // Inicializar invisível
            canvasGroup.alpha = 0f;
            isVisible = false;
            
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                UnityEngine.Debug.LogError("[InteractionIcon] Main camera not found!");
            }
            
            baseLocalPosition = iconTransform.localPosition;
        }
        
        private void Update()
        {
            if (target != null)
            {
                UpdatePosition();
                UpdateBounceAnimation();
            }
            
            UpdateFade();
        }
        
        /// <summary>
        /// Exibe o ícone com animação de fade in.
        /// </summary>
        public void Show()
        {
            isVisible = true;
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Oculta o ícone com animação de fade out.
        /// </summary>
        public void Hide()
        {
            isVisible = false;
        }
        
        /// <summary>
        /// Define o alvo (NPC) que o ícone deve seguir.
        /// </summary>
        /// <param name="target">Transform do NPC</param>
        public void SetTarget(Transform target)
        {
            this.target = target;
        }
        
        /// <summary>
        /// Define o alvo (NPC) que o ícone deve seguir com offset customizado.
        /// </summary>
        /// <param name="target">Transform do NPC</param>
        /// <param name="offset">Offset em world space</param>
        public void SetTarget(Transform target, Vector3 offset)
        {
            this.target = target;
            this.worldOffset = offset;
        }
        
        /// <summary>
        /// Atualiza a posição do ícone para seguir o alvo em world space.
        /// Converte de world space para screen space.
        /// </summary>
        private void UpdatePosition()
        {
            if (target == null || mainCamera == null)
                return;
            
            // Calcular posição em world space com offset
            Vector3 worldPosition = target.position + worldOffset;
            
            // Converter para screen space
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
            
            // Verificar se está atrás da câmera
            if (screenPosition.z < 0)
            {
                // Ocultar se estiver atrás da câmera
                if (isVisible)
                {
                    Hide();
                }
                return;
            }
            
            // Atualizar posição do ícone
            iconTransform.position = screenPosition;
        }
        
        /// <summary>
        /// Atualiza a animação de bounce/pulse do ícone.
        /// </summary>
        private void UpdateBounceAnimation()
        {
            bounceTimer += Time.deltaTime * bounceSpeed;
            
            // Calcular offset vertical usando seno para movimento suave
            float bounceOffset = Mathf.Sin(bounceTimer) * bounceHeight;
            
            // Aplicar offset ao local position
            Vector3 newLocalPosition = baseLocalPosition;
            newLocalPosition.y += bounceOffset;
            iconTransform.localPosition = newLocalPosition;
        }
        
        /// <summary>
        /// Atualiza o fade in/out do ícone.
        /// </summary>
        private void UpdateFade()
        {
            float targetAlpha = isVisible ? 1f : 0f;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
            
            // Desativar GameObject quando completamente invisível para otimização
            if (!isVisible && canvasGroup.alpha < 0.01f)
            {
                canvasGroup.alpha = 0f;
                gameObject.SetActive(false);
            }
        }
        
        private void OnValidate()
        {
            // Garantir que componentes sejam atribuídos no editor
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
            
            if (iconTransform == null)
            {
                iconTransform = GetComponent<RectTransform>();
            }
        }
    }
}
