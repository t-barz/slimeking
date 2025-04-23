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

    private GameObject[] frontObjects;  // Objects with "front" in their name
    private GameObject[] backObjects;   // Objects with "back" in their name
    private GameObject[] sideObjects;   // Objects with "side" in their name

    private Vector2 moveInput;           // Stores the current movement input vector
    private Rigidbody2D rb;             // Reference to the Rigidbody2D component
    private Animator animator;           // Reference to the Animator component
    private bool isFacingLeft = false;   // Tracks whether the character is facing left

    private void Awake()
    {
        // Get required components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Initialize visual objects arrays by searching child objects by name
        frontObjects = GetObjectsByNameContains("front");
        backObjects = GetObjectsByNameContains("back");
        sideObjects = GetObjectsByNameContains("side");
    }

    /// <summary>
    /// Gets all child GameObjects that contain the specified string in their name
    /// </summary>
    private GameObject[] GetObjectsByNameContains(string nameContains)
    {
        // Get all child transforms
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        
        // Count objects that match the name criteria
        int matchCount = 0;
        foreach (Transform child in allChildren)
        {
            if (child != transform && child.name.ToLower().Contains(nameContains))
            {
                matchCount++;
            }
        }

        // Create and fill array with matching objects
        GameObject[] matchingObjects = new GameObject[matchCount];
        int index = 0;
        foreach (Transform child in allChildren)
        {
            if (child != transform && child.name.ToLower().Contains(nameContains))
            {
                matchingObjects[index] = child.gameObject;
                index++;
            }
        }

        return matchingObjects;
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

    private void SetActiveObjects(GameObject[] objects, bool active)
    {
        foreach (var obj in objects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }

    private void FlipSideObjects(bool faceLeft)
    {
        foreach (var obj in sideObjects)
        {
            if (obj != null)
            {
                Vector3 scale = obj.transform.localScale;
                scale.x = faceLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                obj.transform.localScale = scale;
            }
        }
    }
}