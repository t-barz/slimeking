using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Controls the player's movement and visual state based on direction using the new Input System
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference movementAction;    // Reference to the movement input action

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;                  // Controls how fast the player moves
    [SerializeField] private float jumpHeight = 2f;                 // Controls the height of the jump arc

    // References to different visual states of the player
    private GameObject[] frontObjects;    // Objects with "front" in their name
    private GameObject[] backObjects;     // Objects with "back" in their name
    private GameObject[] sideObjects;     // Objects with "side" in their name
    private Collider2D[] playerColliders; // Array to store all player colliders

    // Movement and state variables
    private Vector2 moveInput;            // Stores the current movement input vector
    private Rigidbody2D rb;              // Reference to the Rigidbody2D component
    private Animator animator;            // Reference to the Animator component
    private bool isFacingLeft = false;    // Tracks whether the character is facing left
    private bool isSliding = false;       // Tracks if the player is currently sliding
    private bool isJumping = false;       // Tracks if the player is currently jumping

    [Header("Audio Settings")]
    [Tooltip("Som reproduzido durante a movimentação")]
    [SerializeField] private AudioClip walkingSound;
    [Tooltip("Som reproduzido ao pular")]
    [SerializeField] private AudioClip jumpSound;
    [Tooltip("Som reproduzido ao deslizar")]
    [SerializeField] private AudioClip slideSound;
    [Tooltip("Volume dos sons")]
    [Range(0, 1)]
    [SerializeField] private float soundVolume = 1f;

    // Add AudioSource to the existing private fields
    private AudioSource audioSource;

    /// <summary>
    /// Initialize components and gather child objects on startup
    /// </summary>
    private void Awake()
    {
        // Get required components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerColliders = GetComponents<Collider2D>();

        // Initialize visual objects arrays by searching child objects by name
        frontObjects = GetObjectsByNameContains("front");
        backObjects = GetObjectsByNameContains("back");
        sideObjects = GetObjectsByNameContains("side");

        // Setup audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = soundVolume;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // Set to 2D sound
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

    /// <summary>
    /// Set initial visual state
    /// </summary>
    private void Start()
    {
        UpdateVisualState(Vector2.down); // Set initial visual state to face front/down
    }

    /// <summary>
    /// Subscribe to input events when enabled
    /// </summary>
    private void OnEnable()
    {
        if (movementAction != null)
        {
            movementAction.action.performed += OnMovementPerformed;
            movementAction.action.canceled += OnMovementCanceled;
            movementAction.action.Enable();
        }
    }

    /// <summary>
    /// Unsubscribe from input events when disabled
    /// </summary>
    private void OnDisable()
    {
        if (movementAction != null)
        {
            movementAction.action.performed -= OnMovementPerformed;
            movementAction.action.canceled -= OnMovementCanceled;
            movementAction.action.Disable();
        }

        // Stop any playing sounds
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Handles movement input when keys are pressed
    /// </summary>
    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput != Vector2.zero)
        {
            UpdateVisualState(moveInput);
            animator.SetBool("isWalking", true);
        }
    }

    /// <summary>
    /// Handles when movement keys are released
    /// </summary>
    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
        animator.SetBool("isWalking", false);

        // Stop walking sound
        if (audioSource.isPlaying && audioSource.clip == walkingSound)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Updates physics-based movement
    /// </summary>
    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed * Time.fixedDeltaTime * 120f;
    }

    /// <summary>
    /// Initiates the slide action with movement to destination
    /// </summary>
    /// <param name="destination">Target position to move to</param>
    public void Slide(Vector3 destination)
    {
        if (!isSliding && animator != null)
        {
            // Play slide sound
            if (slideSound != null && audioSource != null)
            {
                audioSource.Stop();
                audioSource.loop = false;
                audioSource.clip = slideSound;
                audioSource.Play();
            }
            StartCoroutine(SlideCoroutine(destination));
        }
    }

    /// <summary>
    /// Manages the slide state, colliders, and movement to destination
    /// </summary>
    private IEnumerator SlideCoroutine(Vector3 destination)
    {
        isSliding = true;
        const float MOVE_DURATION = 0.75f;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        // Disable colliders during slide
        foreach (var collider in playerColliders)
        {
            collider.enabled = false;
        }

        animator.SetTrigger("Shrink");


        while (elapsedTime < MOVE_DURATION)
        {
            // Calcula a fração do tempo decorrido
            float t = elapsedTime / MOVE_DURATION;
            // Interpola a posição entre o início e o destino
            transform.position = Vector3.Lerp(startPosition, destination, t);

            elapsedTime += Time.deltaTime;
            yield return null; // espera o próximo frame
        }

        // Garante que a posição final seja exatamente o destino
        //transform.position = destination;


        // Re-enable colliders
        foreach (var collider in playerColliders)
        {
            collider.enabled = true;
        }

        isSliding = false;
    }

    /// <summary>
    /// Initiates the jump action with movement to destination
    /// </summary>
    /// <param name="destination">Target position to move to</param>
    public void Jump(Vector3 destination)
    {
        if (!isJumping && animator != null)
        {
            // Play jump sound
            if (jumpSound != null)
            {
                audioSource.loop = false;
                audioSource.clip = jumpSound;
                audioSource.Play();
            }
            StartCoroutine(JumpCoroutine(destination));
        }
    }

    /// <summary>
    /// Manages the jump state and movement to destination
    /// </summary>
    private IEnumerator JumpCoroutine(Vector3 destination)
    {
        isJumping = true;
        const float MOVE_DURATION = 0.5f;

        animator.SetTrigger("Jump");
        Vector3 startPosition = transform.position;
        float startTime = Time.time;
        float totalDistance = Vector3.Distance(startPosition, destination);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
               !animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            float elapsedTime = Time.time - startTime;
            float remainingTime = Mathf.Max(MOVE_DURATION - elapsedTime, Time.deltaTime);

            Vector3 currentPosition = transform.position;
            Vector3 toDestination = destination - currentPosition;

            if (toDestination.magnitude > 0.01f)
            {
                float speed = toDestination.magnitude / remainingTime;

                // Calculate progress for arc movement (0 to 1)
                float progress = 1f - (toDestination.magnitude / totalDistance);
                float verticalOffset = jumpHeight * Mathf.Sin(progress * Mathf.PI);

                // Move towards destination with arc
                Vector3 targetPosition = Vector3.MoveTowards(
                    currentPosition,
                    destination,
                    speed * Time.deltaTime
                );
                targetPosition.y += verticalOffset;

                transform.position = targetPosition;
            }

            yield return null;
        }

        transform.position = destination;
        isJumping = false;
    }

    /// <summary>
    /// Updates the visual representation based on movement direction
    /// </summary>
    private void UpdateVisualState(Vector2 direction)
    {
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY)
        {
            // Horizontal movement
            SetActiveObjects(sideObjects, true);
            SetActiveObjects(frontObjects, false);
            SetActiveObjects(backObjects, false);

            bool shouldFaceLeft = direction.x < 0;
            if (shouldFaceLeft != isFacingLeft)
            {
                isFacingLeft = shouldFaceLeft;
                FlipSideObjects(isFacingLeft);
            }
        }
        else
        {
            // Vertical movement
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
    /// Activates or deactivates a group of GameObjects
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
    /// Flips the scale of side objects to face left or right
    /// </summary>
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

    /// <summary>
    /// Tenta reproduzir o som de movimento
    /// </summary>
    public void PlayWalkingSound()
    {
        if (walkingSound != null && audioSource != null)
        {
            audioSource.loop = false;
            audioSource.clip = walkingSound;
            audioSource.Play();
        }
    }
}