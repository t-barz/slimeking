using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheSlimeKing.Gameplay
{
    /// <summary>
    /// Responsável pelo movimento do Slime baseado no Input System
    /// Compatível com URP 2D no Unity 6
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class SlimeMovement : MonoBehaviour, IPlayerController
    {
        [Header("Configurações de Movimento")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private float deceleration = 20f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Referências")]
        [SerializeField] private SlimeVisualController visualController;
        [SerializeField] private SlimeAnimationController animationController;
        [SerializeField] private GameObject attackCollider;  // Referência ao GameObject com o collider de ataque

        // Input System
        private Vector2 _moveInput;
        private bool _isCrouching;

        // Componentes
        private Rigidbody2D _rb;
        private SlimeStats _stats;

        // Estado
        private Vector2 _currentVelocity;
        private Vector2 _lastNonZeroDirection = Vector2.down; // Começa olhando para frente
        private bool _controlEnabled = true;
        private bool _inSpecialMovement = false;
        private float _currentScale = 1.0f;
        public bool isAttacking = false;

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
            // Verifica se o controle está habilitado
            if (!_controlEnabled)
                return;

            if (isAttacking)
                return;

            // Obtém o componente AttackCollider para verificar cooldown
            SlimeAttackCollider attackComponent = null;
            if (attackCollider != null)
            {
                attackComponent = attackCollider.GetComponent<SlimeAttackCollider>();
            }

            if (animationController != null)
            {
                animationController.PlayAttack1Animation();

                // Executa o ataque usando o novo sistema
                if (attackCollider != null && attackComponent != null)
                {
                    // Configura o AttackCollider para usar ataque básico
                    attackComponent.SetAttackType(false); // Ataque normal (não especial)

                    // Tenta realizar o ataque em todos os alvos dentro do alcance
                    attackComponent.TryAttack();
                    isAttacking = true;
                }
            }
        }

        /// <summary>
        /// Acionado quando o botão de ataque especial é pressionado
        /// </summary>
        public void OnSpecialAttackPressed()
        {
            // Verifica se o controle está habilitado
            if (!_controlEnabled)
                return;

            // Obtém o componente AttackCollider para verificar cooldown
            SlimeAttackCollider attackComponent = null;
            if (attackCollider != null)
            {
                attackComponent = attackCollider.GetComponent<SlimeAttackCollider>();
            }

            // Não há mais verificações sobre se um ataque especial pode ser realizado
            // Todos os ataques são permitidos a qualquer momento

            if (animationController != null)
            {
                animationController.PlayAttack2Animation();

                // Executa o ataque usando o novo sistema
                if (attackCollider != null && attackComponent != null)
                {
                    // Configura o AttackCollider para usar ataque especial
                    attackComponent.SetAttackType(true); // Ataque especial

                    // Tenta realizar o ataque em todos os alvos dentro do alcance
                    bool attackExecuted = attackComponent.TryAttack();

                    if (attackExecuted)
                    {
                        Debug.Log("Ataque especial executado");
                    }
                }
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
            // Verifica se o controle está habilitado
            if (!_controlEnabled)
                return;

            // Obtém o controlador de interação
            var interactionController = GetComponent<SlimeInteractionController>();
            if (interactionController != null)
            {
                // Tenta realizar a interação - só funciona se houver um objeto interativo próximo
                // O método retorna true se a interação foi realizada com sucesso
                bool interacted = interactionController.Interact();

                // Opcionalmente, você poderia adicionar feedback sonoro/visual quando não há objetos para interagir
                if (!interacted)
                {
                    // Não há nada com que interagir - pode adicionar feedback aqui (som, animação, etc.)
                }
            }
        }

        #endregion

        #region IPlayerController Implementation

        /// <summary>
        /// Desativa o controle do jogador
        /// </summary>
        public void DisableControl()
        {
            _controlEnabled = false;
            _moveInput = Vector2.zero;
            _rb.linearVelocity = Vector2.zero;

            // Desativa animação de movimento se existir um controlador de animação
            if (animationController != null)
            {
                // Comentado até termos detalhes da implementação do SlimeAnimationController
                // animationController.SetAnimation("Idle");
            }

            _inSpecialMovement = true;
        }

        /// <summary>
        /// Reativa o controle do jogador
        /// </summary>
        public void EnableControl()
        {
            _controlEnabled = true;
            _inSpecialMovement = false;

            // Restaura escala normal se necessário
            if (_currentScale != 1.0f)
                SetScale(1.0f);
        }

        /// <summary>
        /// Move o jogador para uma posição específica
        /// </summary>
        public void MoveToPosition(Vector2 position, bool immediate = false)
        {
            if (immediate)
            {
                transform.position = position;
                _rb.position = position;
                _rb.linearVelocity = Vector2.zero;
            }
            else
            {
                _rb.MovePosition(position);
            }
        }

        /// <summary>
        /// Define a escala visual do jogador
        /// </summary>
        public void SetScale(float scale)
        {
            _currentScale = scale;
            transform.localScale = new Vector3(scale, scale, scale);

            // Ajusta o collider se necessário
            CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();
            if (capsuleCollider != null)
            {
                capsuleCollider.size = new Vector2(capsuleCollider.size.x, capsuleCollider.size.y * scale);
            }

            // Atualiza visualização, se necessário
            if (visualController != null)
            {
                // Atualiza visual com base na última direção conhecida
                visualController.UpdateDirection(_lastNonZeroDirection);
            }
        }

        /// <summary>
        /// Atualiza a direção visual do jogador
        /// </summary>
        public void SetDirection(Vector2 direction)
        {
            if (direction != Vector2.zero)
                _lastNonZeroDirection = direction.normalized;

            // Atualiza visualização baseada na direção
            if (visualController != null)
                visualController.UpdateDirection(_lastNonZeroDirection);
        }

        /// <summary>
        /// Obtém a posição atual do jogador
        /// </summary>
        public Vector2 GetPosition()
        {
            return transform.position;
        }

        /// <summary>
        /// Obtém a direção atual do jogador
        /// </summary>
        public Vector2 GetDirection()
        {
            return _lastNonZeroDirection;
        }

        /// <summary>
        /// Verifica se o personagem está agachado
        /// Método adicionado para compatibilidade com o sistema de Stealth
        /// </summary>
        public bool IsCrouching()
        {
            return _isCrouching;
        }

        /// <summary>
        /// Desliza o slime até uma posição específica durante um tempo determinado
        /// </summary>
        /// <param name="destination">Posição de destino</param>
        /// <param name="duration">Duração do movimento em segundos</param>
        public void Slide(Vector2 destination, float duration)
        {
            // Calcula a direção para o destino
            Vector2 direction = (destination - (Vector2)transform.position).normalized;

            // Atualiza a direção visual do slime
            SetDirection(direction);

            // Inicia a coroutine de deslizamento
            StartCoroutine(SlideCoroutine(destination, duration));
        }

        /// <summary>
        /// Coroutine que controla o movimento suave do slime durante o deslizamento
        /// </summary>
        private IEnumerator SlideCoroutine(Vector2 destination, float duration)
        {
            // Desabilita o controle do jogador durante o slide
            DisableControl();

            // Salva a posição inicial
            Vector2 startPosition = transform.position;

            // Tempo decorrido
            float elapsed = 0f;

            // Se existe animationController, poderia acionar uma animação de deslizamento aqui
            if (animationController != null)
            {
                // Supondo que exista um método para animação de slide
                animationController.PlayShrinkAnimation();
            }

            while (elapsed < duration)
            {
                // Calcula o progresso (0 a 1) linear
                float t = elapsed / duration;

                // Interpola a posição com movimento linear
                Vector2 newPosition = Vector2.Lerp(startPosition, destination, t);

                // Move o slime para a posição interpolada
                MoveToPosition(newPosition, true);

                // Atualiza o tempo decorrido
                elapsed += Time.deltaTime;

                // Espera até o próximo frame
                yield return null;
            }

            // Garante que a posição final seja exatamente a de destino
            MoveToPosition(destination, true);

            // Reativa o controle do jogador
            EnableControl();
        }

        internal void EndAttack()
        {
            isAttacking = false;
        }

        // Método DeactivateAttackCollider removido pois não é mais necessário com o novo sistema de ataque
        // O collider de ataque agora permanece sempre ativo e TryAttack é chamado quando necessário

        #endregion
    }
}
