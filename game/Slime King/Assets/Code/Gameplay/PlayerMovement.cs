using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

/// <summary>
/// Controls the player's movement and visual state based on direction
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // Controls how fast the player moves

    [Header("Visual Components")]
    // Arrays to store different visual states of the character
    [SerializeField] private GameObject[] frontObjects;  // Objects shown when facing front/down
    [SerializeField] private GameObject[] backObjects;   // Objects shown when facing back/up
    [SerializeField] private GameObject[] sideObjects;   // Objects shown when facing left/right

    // Movement input from the Input System
    private Vector2 moveInput;
    // Reference to the Rigidbody2D component for physics-based movement
    private Rigidbody2D rb;
    // Tracks whether the character is facing left (for horizontal flipping)
    private bool isFacingLeft = false;

    private void Awake()
    {
        // Get the Rigidbody2D component at startup
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Set initial visual state to face front/down
        UpdateVisualState(Vector2.down);
    }

    private void FixedUpdate()
    {
        // Update physics-based movement in FixedUpdate for consistency
        rb.linearVelocity = moveInput * moveSpeed;
    }

    /// <summary>
    /// Called by the Input System when movement input changes
    /// </summary>
    public void OnMove(InputValue value)
    {
        // Get the current input vector (x,y between -1 and 1)
        moveInput = value.Get<Vector2>();
        
        // Only update visuals if there's actual movement
        if (moveInput != Vector2.zero)
        {
            UpdateVisualState(moveInput);
        }
    }

    /// <summary>
    /// Updates which visual objects are shown based on movement direction
    /// </summary>
    private void UpdateVisualState(Vector2 direction)
    {
        // Compare absolute values to determine if movement is more horizontal or vertical
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY)
        {
            // If X movement is greater, character is moving horizontally
            SetActiveObjects(sideObjects, true);
            SetActiveObjects(frontObjects, false);
            SetActiveObjects(backObjects, false);

            // Determine if character should face left or right
            bool shouldFaceLeft = direction.x < 0;
            if (shouldFaceLeft != isFacingLeft)
            {
                isFacingLeft = shouldFaceLeft;
                FlipSideObjects(isFacingLeft);
            }
        }
        else
        {
            // If Y movement is greater or equal, character is moving vertically
            if (direction.y > 0)
            {
                // Moving upward
                SetActiveObjects(backObjects, true);
                SetActiveObjects(frontObjects, false);
                SetActiveObjects(sideObjects, false);
            }
            else
            {
                // Moving downward or idle
                SetActiveObjects(frontObjects, true);
                SetActiveObjects(backObjects, false);
                SetActiveObjects(sideObjects, false);
            }
        }
    }

    /// <summary>
    /// Helper method to enable/disable arrays of GameObjects
    /// </summary>
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