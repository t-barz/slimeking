using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Controla a exibição de outline quando o player se aproxima.
    /// Cria um sprite duplicado atrás do original para simular o efeito de outline.
    /// </summary>
    public class OutlineController : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private float detectionRange = 3f;

        [SerializeField]
        private Color outlineColor = new Color(1f, 1f, 0f, 1f);

        [SerializeField]
        private float outlineWidth = 0.05f;

        [SerializeField]
        private float fadeDuration = 0.3f;

        private SpriteRenderer spriteRenderer;
        private GameObject outlineObject;
        private SpriteRenderer outlineSpriteRenderer;
        private Material outlineMaterial;
        private Transform playerTransform;
        private bool isOutlineActive = false;
        private Coroutine fadeCoroutine;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            UnityEngine.Debug.LogError($"[OutlineController] AWAKE em {gameObject.name}");
        }

        private void OnEnable()
        {
            UnityEngine.Debug.LogError($"[OutlineController] OnEnable em {gameObject.name}, enabled={enabled}");
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                UnityEngine.Debug.LogError($"[OutlineController] SpriteRenderer NÃO encontrado!");
                enabled = false;
                return;
            }

            CreateOutlineSprite();
            FindPlayer();
            UnityEngine.Debug.Log($"[OutlineController] Inicialização completa, enabled={enabled}");
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                FindPlayer();
                return;
            }

            SyncFlip();
            CheckPlayerDistance();
        }

        private void OnDisable()
        {
            UnityEngine.Debug.LogWarning($"[OutlineController] OnDisable em {gameObject.name}");
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }
            
            if (outlineObject != null)
            {
                Destroy(outlineObject);
            }

            if (outlineMaterial != null)
            {
                Destroy(outlineMaterial);
                outlineMaterial = null;
            }
        }

        #endregion

        #region Private Methods

        private void CreateOutlineSprite()
        {
            outlineObject = new GameObject("Outline");
            outlineObject.transform.SetParent(transform);
            outlineObject.transform.localPosition = Vector3.zero;
            outlineObject.transform.localRotation = Quaternion.identity;
            outlineObject.transform.localScale = Vector3.one * (1f + outlineWidth);

            outlineSpriteRenderer = outlineObject.AddComponent<SpriteRenderer>();
            outlineSpriteRenderer.sprite = spriteRenderer.sprite;
            outlineSpriteRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            outlineSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

            // Usar shader customizado para outline sólido
            Shader outlineShader = Shader.Find("Custom/SpriteOutlineURP");
            if (outlineShader != null)
            {
                outlineMaterial = new Material(outlineShader);
                Color initialColor = outlineColor;
                initialColor.a = 0f;
                outlineMaterial.SetColor("_OutlineColor", initialColor);
                outlineSpriteRenderer.material = outlineMaterial;
                UnityEngine.Debug.Log($"[Outline] Criado com alpha inicial: {outlineMaterial.GetColor("_OutlineColor").a}");
            }

            SyncFlip();
            outlineObject.SetActive(true);
            outlineSpriteRenderer.enabled = false;  // Iniciar desativado
        }

        private void SyncFlip()
        {
            if (outlineSpriteRenderer != null && spriteRenderer != null)
            {
                outlineSpriteRenderer.flipX = spriteRenderer.flipX;
                outlineSpriteRenderer.flipY = spriteRenderer.flipY;
            }
        }

        private void FindPlayer()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
                UnityEngine.Debug.Log($"[Outline] Player encontrado: {playerObject.name}");
            }
            else
            {
                UnityEngine.Debug.LogWarning($"[Outline] Player NÃO encontrado! Tag 'Player' existe?");
            }
        }

        private void CheckPlayerDistance()
        {
            if (playerTransform == null)
                return;

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            bool shouldShowOutline = distanceToPlayer <= detectionRange;

            if (shouldShowOutline && !isOutlineActive)
            {
                UnityEngine.Debug.Log($"[Outline] Ativando - Distância: {distanceToPlayer:F2}");
                ActivateOutline();
            }
            else if (!shouldShowOutline && isOutlineActive)
            {
                UnityEngine.Debug.Log($"[Outline] Desativando - Distância: {distanceToPlayer:F2}");
                DeactivateOutline();
            }
        }

        private void ActivateOutline()
        {
            if (outlineObject != null && !isOutlineActive)
            {
                isOutlineActive = true;
                outlineSpriteRenderer.enabled = true;
                
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                
                fadeCoroutine = StartCoroutine(FadeOutline(1f));
            }
        }

        private void DeactivateOutline()
        {
            if (outlineObject != null && isOutlineActive)
            {
                isOutlineActive = false;
                
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                
                fadeCoroutine = StartCoroutine(FadeOutline(0f));
            }
        }

        private System.Collections.IEnumerator FadeOutline(float targetAlpha)
        {
            if (outlineSpriteRenderer == null)
                yield break;

            float startAlpha = GetOutlineAlpha();
            float elapsed = 0f;
            UnityEngine.Debug.Log($"[Outline] Fade iniciado: {startAlpha:F2} -> {targetAlpha:F2}");

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                SetOutlineAlpha(currentAlpha);
                yield return null;
            }

            SetOutlineAlpha(targetAlpha);
            
            // Desativar o renderer quando o fade terminar com alpha = 0
            if (Mathf.Approximately(targetAlpha, 0f))
            {
                outlineSpriteRenderer.enabled = false;
            }
            
            UnityEngine.Debug.Log($"[Outline] Fade completo. Alpha final: {GetOutlineAlpha():F2}");
            fadeCoroutine = null;
        }

        private void SetOutlineAlpha(float alpha)
        {
            if (outlineMaterial == null)
                return;

            Color currentColor = outlineMaterial.GetColor("_OutlineColor");
            currentColor.a = alpha;
            outlineMaterial.SetColor("_OutlineColor", currentColor);
        }

        private float GetOutlineAlpha()
        {
            if (outlineMaterial == null)
                return 0f;

            return outlineMaterial.GetColor("_OutlineColor").a;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Mostra ou esconde o outline manualmente
        /// </summary>
        public void ShowOutline(bool show)
        {
            if (show && !isOutlineActive)
            {
                ActivateOutline();
            }
            else if (!show && isOutlineActive)
            {
                DeactivateOutline();
            }
        }

        /// <summary>
        /// Atualiza a cor do outline
        /// </summary>
        public void UpdateOutlineColor(Color color)
        {
            outlineColor = color;
            if (outlineMaterial != null)
            {
                float currentAlpha = GetOutlineAlpha();
                color.a = currentAlpha;
                outlineMaterial.SetColor("_OutlineColor", color);
            }
        }

        /// <summary>
        /// Retorna se o outline está ativo no momento
        /// </summary>
        public bool IsOutlineActive => isOutlineActive;

        /// <summary>
        /// Define o alcance de detecção do player
        /// </summary>
        public void SetDetectionRange(float range)
        {
            detectionRange = Mathf.Max(0, range);
        }

        #endregion
    }
}
