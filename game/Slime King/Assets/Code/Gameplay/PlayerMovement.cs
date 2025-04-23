using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the player's movement and visual state based on direction using the new Input System
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference movementAction; // Reference to the movement input action

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // Controls how fast the player moves

    [Header("Visual Components")]
    [SerializeField] private GameObject[] frontObjects;  // Objects shown when facing front/down
    [SerializeField] private GameObject[] backObjects;   // Objects shown when facing back/up
    [SerializeField] private GameObject[] sideObjects;   // Objects shown when facing left/right

    private Vector2 moveInput;           // Stores the current movement input vector
    private Rigidbody2D rb;             // Reference to the Rigidbody2D component
    private Animator animator;           // Reference to the Animator component
    private bool isFacingLeft = false;   // Tracks whether the character is facing left

    private void Awake()
    {
        // Get required components at startup
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Set initial visual state to face front/down
        UpdateVisualState(Vector2.down);
    }

    private void OnEnable()
    {
        // Subscribe to input action events when the component is enabled
        movementAction.action.performed += OnMovementPerformed;
        movementAction.action.canceled += OnMovementCanceled;
        movementAction.action.Enable();
    }

    private void OnDisable()
    {
        // Unsubscribe from input action events when the component is disabled
        movementAction.action.performed -= OnMovementPerformed;
        movementAction.action.canceled -= OnMovementCanceled;
        movementAction.action.Disable();
    }

    /// <summary>
    /// Called when movement input is performed
    /// </summary>
    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        // Get the input vector and update visual state if there's movement
        moveInput = context.ReadValue<Vector2>();
        if (moveInput != Vector2.zero)
        {
            UpdateVisualState(moveInput);
            animator.SetBool("isWalking", true); // Set walking animation state
        }
    }

    /// <summary>
    /// Called when movement input is canceled (released)
    /// </summary>
    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        // Reset movement input when no keys are pressed
        moveInput = Vector2.zero;
        animator.SetBool("isWalking", false); // Stop walking animation
    }

    private void FixedUpdate()
    {
        // Update physics-based movement in FixedUpdate for consistency
        rb.linearVelocity = moveInput * moveSpeed;
    }

    /// <summary>
    /// Updates which visual objects are shown based on movement direction
    /// </summary>
    /// <param name="direction">The movement direction vector</param>
    private void UpdateVisualState(Vector2 direction)
    {
        // Compare absolute values to determine if movement is more horizontal or vertical
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY)
        {
            // Horizontal movement is dominant
            SetActiveObjects(sideObjects, true);
            SetActiveObjects(frontObjects, false);
            SetActiveObjects(backObjects, false);

            // Update facing direction if needed
            bool shouldFaceLeft = direction.x < 0;
            if (shouldFaceLeft != isFacingLeft)
            {
                isFacingLeft = shouldFaceLeft;
                FlipSideObjects(isFacingLeft);
            }
        }
        else
        {
            // Vertical movement is dominant
            if (direction.y > 0)
            {
                // Moving up
                SetActiveObjects(backObjects, true);
                SetActiveObjects(frontObjects, false);
                SetActiveObjects(sideObjects, false);
            }
            else
            {
                // Moving down
                SetActiveObjects(frontObjects, true);
                SetActiveObjects(backObjects, false);
                SetActiveObjects(sideObjects, false);
            }
        }
    }

    /// <summary>
    /// Helper method to enable/disable arrays of GameObjects
    /// </summary>
    /// <param name="objects">Array of GameObjects to modify</param>
    /// <param name="active">Whether the objects should be active or not</param>
    private void SetActiveObjects(GameObject[] objects, bool active)
    {
        foreach (var obj in objects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }

    /// <summary>
    /// Flips the side-view objects horizontally when facing left
    /// </summary>
    /// <param name="faceLeft">Whether the character should face left</param>
    private void FlipSideObjects(bool faceLeft)
    {
        foreach (var obj in sideObjects)
        {
            if (obj != null)
            {
                Vector3 scale = obj.transform.localScale;
                // Flip the X scale while preserving its absolute value
                scale.x = faceLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                obj.transform.localScale = scale;
            }
        }
    }
}