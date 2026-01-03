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

        private SpriteRenderer spriteRenderer;
        private GameObject outlineObject;
        private SpriteRenderer outlineSpriteRenderer;
        private Transform playerTransform;
        private bool isOutlineActive = false;

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                enabled = false;
                return;
            }

            CreateOutlineSprite();
            FindPlayer();
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
            if (outlineObject != null)
            {
                Destroy(outlineObject);
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

            Shader outlineShader = Shader.Find("Custom/SpriteOutlineURP");
            if (outlineShader != null)
            {
                Material outlineMat = new Material(outlineShader);
                outlineMat.SetColor("_OutlineColor", outlineColor);
                outlineSpriteRenderer.material = outlineMat;
            }

            SyncFlip();

            outlineObject.SetActive(false);
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
                ActivateOutline();
            }
            else if (!shouldShowOutline && isOutlineActive)
            {
                DeactivateOutline();
            }
        }

        private void ActivateOutline()
        {
            if (outlineObject != null)
            {
                outlineObject.SetActive(true);
                isOutlineActive = true;
            }
        }

        private void DeactivateOutline()
        {
            if (outlineObject != null)
            {
                outlineObject.SetActive(false);
                isOutlineActive = false;
            }
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
            if (outlineSpriteRenderer != null && outlineSpriteRenderer.material != null)
            {
                outlineSpriteRenderer.material.SetColor("_OutlineColor", color);
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
