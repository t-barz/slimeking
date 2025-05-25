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
            DisableInput();
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
        #endregion

        #region Sistema de Movimento
        private void UpdateMovement()
        {
            if (!combat.IsAttacking && !actionController.IsSliding && !actionController.IsJumping)
            {
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