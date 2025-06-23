using UnityEngine;

namespace TheSlimeKing.Gameplay
{
    /// <summary>
    /// Controla a visibilidade dos objetos visuais do Slime baseado na direção
    /// Agora com suporte a um único Animator e compatibilidade com URP
    /// </summary>
    public class SlimeVisualController : MonoBehaviour
    {
        [Header("Sprites por Direção")]
        [SerializeField] private GameObject frontSprite;
        [SerializeField] private GameObject backSprite;
        [SerializeField] private GameObject sideSprite;

        [Header("Efeitos Visuais")]
        [SerializeField] private GameObject vfxFront;
        [SerializeField] private GameObject vfxBack;
        [SerializeField] private GameObject vfxSide;

        [Header("Sombra")]
        [SerializeField] private GameObject shadowObject;

        [Header("Configurações")]
        [SerializeField] private float directionThreshold = 0.5f;

        // Estado atual
        private SlimeDirection _currentDirection = SlimeDirection.Front;
        private bool _isFacingRight = true; // Nova propriedade para rastrear a orientação

        // Enum para direções
        public enum SlimeDirection
        {
            Front,
            Back,
            Side
        }

        private void Awake()
        {
            // Configuração inicial - só mostra front e shadow conforme regras
            UpdateVisibility(SlimeDirection.Front);
        }

        /// <summary>
        /// Atualiza a direção visual do Slime com base no vetor de movimento
        /// </summary>
        public void UpdateDirection(Vector2 direction)
        {
            // Se a magnitude for muito pequena, mantemos a direção atual
            if (direction.magnitude < 0.1f)
                return;

            // Normaliza a direção
            direction = direction.normalized;

            // Determina a direção baseada no vetor de movimento
            SlimeDirection newDirection;

            // Se o movimento vertical for significativo
            if (Mathf.Abs(direction.y) > directionThreshold)
            {
                // Movimento para cima = costas visíveis
                if (direction.y > 0)
                {
                    newDirection = SlimeDirection.Back;
                }
                // Movimento para baixo = frente visível
                else
                {
                    newDirection = SlimeDirection.Front;
                }
            }
            // Se não, considera movimento lateral
            else
            {
                newDirection = SlimeDirection.Side;

                // Atualiza a direção para qual o slime está voltado
                _isFacingRight = direction.x >= 0;

                // Aplica flip horizontal baseado na direção horizontal
                if (sideSprite != null)
                {
                    SpriteRenderer renderer = sideSprite.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        // Se mover para esquerda, aplica flip
                        renderer.flipX = !_isFacingRight;
                    }
                }

                // Também aplica flip nos VFX laterais se existirem
                if (vfxSide != null)
                {
                    foreach (SpriteRenderer renderer in vfxSide.GetComponentsInChildren<SpriteRenderer>())
                    {
                        renderer.flipX = !_isFacingRight;
                    }
                }
            }

            // Se a direção mudou, atualiza a visibilidade
            if (newDirection != _currentDirection)
            {
                UpdateVisibility(newDirection);
                _currentDirection = newDirection;
            }
        }

        /// <summary>
        /// Atualiza a visibilidade dos sprites baseado na direção
        /// </summary>
        public void UpdateVisibility(SlimeDirection direction)
        {
            // Atualiza a direção atual
            _currentDirection = direction;

            // Configura a visibilidade dos sprites baseado na direção
            if (frontSprite != null) frontSprite.SetActive(direction == SlimeDirection.Front);
            if (backSprite != null) backSprite.SetActive(direction == SlimeDirection.Back);
            if (sideSprite != null) sideSprite.SetActive(direction == SlimeDirection.Side);

            // Configura a visibilidade dos VFX correspondentes
            if (vfxFront != null) vfxFront.SetActive(direction == SlimeDirection.Front);
            if (vfxBack != null) vfxBack.SetActive(direction == SlimeDirection.Back);
            if (vfxSide != null) vfxSide.SetActive(direction == SlimeDirection.Side);
        }

        /// <summary>
        /// Obtém a direção visual atual do slime
        /// </summary>
        public SlimeDirection GetCurrentDirection()
        {
            return _currentDirection;
        }

        /// <summary>
        /// Retorna se o slime está virado para a direita
        /// </summary>
        public bool IsFacingRight()
        {
            return _isFacingRight;
        }

        /// <summary>
        /// Define a opacidade (alpha) de todos os sprites do slime
        /// Usado pelo sistema de Stealth para tornar o slime semi-transparente quando escondido
        /// </summary>
        /// <param name="alpha">Valor entre 0 (transparente) e 1 (opaco)</param>
        public void SetAlpha(float alpha)
        {
            // Limita o valor entre 0 e 1
            alpha = Mathf.Clamp01(alpha);

            // Aplica alpha a todos os sprites direcionais
            ApplyAlphaToGameObject(frontSprite, alpha);
            ApplyAlphaToGameObject(backSprite, alpha);
            ApplyAlphaToGameObject(sideSprite, alpha);

            // Aplica alpha aos efeitos visuais
            ApplyAlphaToGameObject(vfxFront, alpha);
            ApplyAlphaToGameObject(vfxBack, alpha);
            ApplyAlphaToGameObject(vfxSide, alpha);
        }

        /// <summary>
        /// Aplica alpha a todos os SpriteRenderers em um GameObject e seus filhos
        /// </summary>
        private void ApplyAlphaToGameObject(GameObject obj, float alpha)
        {
            if (obj == null) return;

            // Aplica aos SpriteRenderers do objeto e seus filhos
            foreach (SpriteRenderer renderer in obj.GetComponentsInChildren<SpriteRenderer>())
            {
                Color color = renderer.color;
                color.a = alpha;
                renderer.color = color;
            }
        }
    }
}
