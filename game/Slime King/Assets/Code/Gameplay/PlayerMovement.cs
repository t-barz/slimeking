using UnityEngine;
using UnityEngine.InputSystem;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Controla o movimento básico do jogador e gerencia os inputs do Input System.
    /// Esta classe atua como um coordenador principal para as diferentes 
    /// funcionalidades do jogador, delegando responsabilidades para classes especializadas.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerVisualManager))]
    [RequireComponent(typeof(PlayerCombat))]
    [RequireComponent(typeof(PlayerActionController))]
    [RequireComponent(typeof(PlayerAudioManager))]
    public class PlayerMovement : MonoBehaviour
    {
        #region Campos Serializados
        [Header("Input")]
        [SerializeField] private InputActionReference movementAction;
        [SerializeField] private InputActionReference attackAction;
        [SerializeField] private InputActionReference interactAction;
        [SerializeField] private InputActionReference crouchAction;
        [SerializeField] private InputActionReference jumpAction;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        #endregion

        #region Campos Privados
        private Vector2 moveInput;
        private Rigidbody2D rb;
        private Animator animator;
        private PlayerVisualManager visualManager;
        private PlayerCombat combat;
        private PlayerActionController actionController;
        private PlayerAudioManager audioManager;
        private Collider2D[] playerColliders;
        private bool isHiding;
        #endregion

        #region Propriedades Públicas
        public bool HasMovementInput => moveInput != Vector2.zero;
        public bool IsHiding => isHiding;
        public Vector2 CurrentMoveInput => moveInput;
        #endregion

        #region Métodos Unity
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            visualManager = GetComponent<PlayerVisualManager>();
            combat = GetComponent<PlayerCombat>();
            actionController = GetComponent<PlayerActionController>();
            audioManager = GetComponent<PlayerAudioManager>();
            playerColliders = GetComponents<Collider2D>();
        }

        private void Start()
        {
            visualManager.UpdateVisualState(Vector2.down);
        }

        private void OnEnable()
        {
            EnableInput();
        }

        private void OnDisable()
        {
            //DisableInput();
        }

        private void FixedUpdate()
        {
            UpdateMovement();
        }
        #endregion

        #region Manipulação de Input
        private void EnableInput()
        {
            if (movementAction != null)
            {
                movementAction.action.performed += OnMovementPerformed;
                movementAction.action.canceled += OnMovementCanceled;
                movementAction.action.Enable();
            }

            if (attackAction != null)
            {
                attackAction.action.performed += OnAttackPerformed;
                attackAction.action.Enable();
            }

            if (interactAction != null)
            {
                interactAction.action.performed += OnInteractPerformed;
                interactAction.action.Enable();
            }

            if (crouchAction != null)
            {
                crouchAction.action.started += OnCrouchStarted;
                crouchAction.action.canceled += OnCrouchCanceled;
                crouchAction.action.Enable();
            }

            if (jumpAction != null)
            {
                jumpAction.action.performed += OnJumpPerformed;
                jumpAction.action.Enable();
            }
        }

        private void DisableInput()
        {
            if (movementAction != null)
            {
                movementAction.action.performed -= OnMovementPerformed;
                movementAction.action.canceled -= OnMovementCanceled;
                movementAction.action.Disable();
            }
            if (attackAction != null)
            {
                attackAction.action.performed -= OnAttackPerformed;
                attackAction.action.Disable();
            }

            if (interactAction != null)
            {
                interactAction.action.performed -= OnInteractPerformed;
                interactAction.action.Disable();
            }

            if (crouchAction != null)
            {
                crouchAction.action.started -= OnCrouchStarted;
                crouchAction.action.canceled -= OnCrouchCanceled;
                crouchAction.action.Disable();
            }

            if (jumpAction != null)
            {
                jumpAction.action.performed -= OnJumpPerformed;
                jumpAction.action.Disable();
            }
        }

        private void OnMovementPerformed(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
            if (moveInput != Vector2.zero && !combat.IsAttacking)
            {
                visualManager.UpdateVisualState(moveInput);
                animator.SetBool("isWalking", true);
            }
        }

        private void OnMovementCanceled(InputAction.CallbackContext context)
        {
            moveInput = Vector2.zero;
            animator.SetBool("isWalking", false);
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            if (!combat.IsAttacking && !actionController.IsJumping && !actionController.IsSliding)
            {
                combat.TryAttack();
                moveInput = Vector2.zero;
            }
        }

        private void OnCrouchStarted(InputAction.CallbackContext context)
        {
            isHiding = true;
            animator.SetBool("isHiding", true);
        }

        private void OnCrouchCanceled(InputAction.CallbackContext context)
        {
            isHiding = false;
            animator.SetBool("isHiding", false);
            EnableColliders();
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            // Verifica se há algum objeto interativo próximo 
            // Essa lógica será tratada pelo InteractionManager
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            if (!combat.IsAttacking && !actionController.IsSliding && !isHiding)
            {
                // Calcula a direção do pulo baseada na direção atual do jogador
                Vector2 jumpDirection = moveInput.normalized;
                if (jumpDirection == Vector2.zero)
                {
                    // Se não houver direção de movimento, usa a direção para onde o jogador está olhando
                    jumpDirection = visualManager.IsFacingLeft ? Vector2.left : Vector2.right;
                }

                // Calcula o destino do pulo (uma pequena distância na direção atual)
                Vector3 jumpDestination = transform.position + new Vector3(jumpDirection.x, jumpDirection.y, 0f) * 2f;

                // Executa o pulo
                actionController.Jump(jumpDestination);

                // Reproduz som de pulo
                audioManager.PlayJumpSound();
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Obstáculo") && isHiding)
            {
                DisableColliders();
            }
        }

        private void DisableColliders()
        {
            foreach (var collider in playerColliders)
            {
                collider.enabled = false;
            }
        }

        private void EnableColliders()
        {
            foreach (var collider in playerColliders)
            {
                collider.enabled = true;
            }
        }
        #endregion

        #region Sistema de Movimento
        private void UpdateMovement()
        {
            Debug.Log($"Move Input: {moveInput}, Is Attacking: {combat.IsAttacking}, Is Sliding: {actionController.IsSliding}, Is Jumping: {actionController.IsJumping}, Is Hiding: {isHiding}");
            if (!combat.IsAttacking && !actionController.IsSliding && !actionController.IsJumping && !isHiding)
            {
                Debug.Log("Updating Player Movement");
                rb.linearVelocity = moveInput * moveSpeed * Time.fixedDeltaTime * 120f;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        public void Jump(Vector3 destination)
        {
            if (!combat.IsAttacking && !actionController.IsSliding)
            {
                actionController.Jump(destination);
            }
        }

        public void Slide(Vector3 destination)
        {
            if (!combat.IsAttacking && !actionController.IsJumping)
            {
                actionController.Slide(destination);
            }
        }
        #endregion
    }
}