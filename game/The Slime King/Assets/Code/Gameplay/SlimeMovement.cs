using UnityEngine;
using UnityEngine.InputSystem;

namespace TheSlimeKing.Gameplay
{
    /// <summary>
    /// Responsável pelo movimento do Slime baseado no Input System
    /// Compatível com URP 2D no Unity 6
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class SlimeMovement : MonoBehaviour
    {
        [Header("Configurações de Movimento")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private float deceleration = 20f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Referências")]
        [SerializeField] private SlimeVisualController visualController;
        [SerializeField] private SlimeAnimationController animationController;

        // Input System
        private Vector2 _moveInput;
        private bool _isCrouching;

        // Componentes
        private Rigidbody2D _rb;
        private SlimeStats _stats;

        // Estado
        private Vector2 _currentVelocity;
        private Vector2 _lastNonZeroDirection = Vector2.down; // Começa olhando para frente

        private void Awake()
        {
            // Componentes no mesmo GameObject
            _rb = GetComponent<Rigidbody2D>();
            _stats = GetComponent<SlimeStats>();

            // Busca controladores no mesmo GameObject primeiro
            if (visualController == null)
                visualController = GetComponent<SlimeVisualController>();

            if (animationController == null)
                animationController = GetComponent<SlimeAnimationController>();

            // URP 6 - Configuração específica para o Rigidbody2D
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        private void FixedUpdate()
        {
            MoveSlime();
            UpdateDirection();
        }

        /// <summary>
        /// Movimenta o Slime com base no input e aplica aceleração/desaceleração
        /// </summary>
        private void MoveSlime()
        {
            // Calcula a velocidade alvo baseada no input e na velocidade do estágio atual
            Vector2 targetVelocity = _moveInput * moveSpeed;            // Aplica multiplicador de velocidade (removida dependência de GetCurrentStageSpeedMultiplier)
            if (_stats != null)
                targetVelocity *= 1.0f; // Valor temporário - implementar integração com SlimeStats posteriormente

            // Aplica aceleração ou desaceleração suave
            if (_moveInput.magnitude > 0.1f)
            {
                // Acelerando
                _currentVelocity = Vector2.MoveTowards(
                    _currentVelocity,
                    targetVelocity,
                    acceleration * Time.fixedDeltaTime
                );
            }
            else
            {
                // Desacelerando
                _currentVelocity = Vector2.MoveTowards(
                    _currentVelocity,
                    Vector2.zero,
                    deceleration * Time.fixedDeltaTime
                );
            }

            // Aplica a velocidade ao Rigidbody
            _rb.linearVelocity = _currentVelocity;

            // Atualiza o parâmetro de animação
            bool isWalking = _currentVelocity.magnitude > 0.1f;
            if (animationController != null)
                animationController.SetWalking(isWalking);
        }

        /// <summary>
        /// Atualiza a direção visual do Slime
        /// </summary>
        private void UpdateDirection()
        {
            // Guarda a última direção não-zero
            if (_currentVelocity.magnitude > 0.1f)
            {
                _lastNonZeroDirection = _currentVelocity.normalized;
            }

            // Atualiza visualização baseada na direção atual
            if (visualController != null)
            {
                visualController.UpdateDirection(_lastNonZeroDirection);
            }

            // Atualiza animação baseada na direção e velocidade
            if (animationController != null)
            {
                animationController.UpdateMovementAnimation(_lastNonZeroDirection, _currentVelocity.magnitude > 0.1f);
            }
        }

        #region Métodos para o Input System (callbacks adaptados para Unity 6)

        /// <summary>
        /// Acionado quando há entrada no vetor de movimento
        /// </summary>
        public void OnMoveVector(Vector2 input)
        {
            _moveInput = input;
        }

        /// <summary>
        /// Acionado quando o botão de agachar é ativado/desativado
        /// </summary>
        public void OnCrouchBool(bool crouchState)
        {
            _isCrouching = crouchState;

            // Animação de encolher quando agachado
            if (animationController != null && crouchState)
            {
                animationController.PlayShrinkAnimation();
            }
        }

        /// <summary>
        /// Acionado quando o botão de ataque é pressionado
        /// </summary>
        public void OnAttackPressed()
        {
            if (animationController != null)
            {
                animationController.PlayAttack1Animation();
            }
        }

        /// <summary>
        /// Acionado quando o botão de ataque especial é pressionado
        /// </summary>
        public void OnSpecialAttackPressed()
        {
            if (animationController != null)
            {
                animationController.PlayAttack2Animation();
            }
        }

        /// <summary>
        /// Acionado quando o botão de salto é pressionado
        /// </summary>
        public void OnJumpPressed()
        {
            if (animationController != null)
            {
                animationController.PlayJumpAnimation();
            }
        }

        /// <summary>
        /// Acionado quando o botão de interação é pressionado
        /// </summary>
        public void OnInteractPressed()
        {
            // Delegado para o controlador de interação
            var interactionController = GetComponent<SlimeInteractionController>();
            if (interactionController != null)
            {
                interactionController.TryInteract();
            }
        }

        #endregion
    }
}
