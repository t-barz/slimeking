using UnityEngine;
using UnityEngine.InputSystem;
using SlimeKing.Gameplay;

/// <summary>
/// Controls warp points that can move the player to a specified destination using different movement types
/// </summary>
public class WarpPointController : MonoBehaviour
{
    /// <summary>
    /// Defines different types of movement for warping the player
    /// </summary>
    public enum WarpType
    {
        Teleport,   // Instantly moves the player
        Slide,      // Moves the player with a sliding animation
        Jump        // Moves the player in an arc with a jumping animation
    }

    [Header("Input")]
    [SerializeField] private InputActionReference interactAction;  // Reference to the interaction input action

    [Header("Warp Settings")]
    [SerializeField] private WarpType warpType;                   // The type of movement to use
    [SerializeField] private Transform destinationPoint;          // The target location to move the player to
    [SerializeField] private float warpDuration = 1f;            // Duration of the warp animation in seconds

    [Header("Animation Parameters")]
    [SerializeField] private string slideAnimationTrigger = "Slide";  // Name of the slide animation trigger parameter
    [SerializeField] private string jumpAnimationTrigger = "Jump";    // Name of the jump animation trigger parameter

    private bool playerInRange;      // Tracks if the player is within interaction range
    private GameObject player;       // Reference to the player GameObject

    /// <summary>
    /// Subscribe to input events when the component is enabled
    /// </summary>
    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed += OnInteract;
            interactAction.action.Enable();
        }
    }

    /// <summary>
    /// Unsubscribe from input events when the component is disabled
    /// </summary>
    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteract;
            interactAction.action.Disable();
        }
    }

    /// <summary>
    /// Handles the interaction input event
    /// </summary>
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (playerInRange)
        {
            WarpPlayer();
        }
    }

    /// <summary>
    /// Triggered when an object enters the warp point's trigger zone
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;
        }
    }

    /// <summary>
    /// Triggered when an object exits the warp point's trigger zone
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    /// <summary>
    /// Initiates the warping process based on the selected warp type
    /// </summary>
    private void WarpPlayer()
    {
        if (destinationPoint == null || player == null) return;

        // Cache the PlayerMovement component
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement == null) return;

        switch (warpType)
        {
            case WarpType.Teleport:
                player.transform.position = destinationPoint.position;
                break;

            case WarpType.Slide:
                MovePlayerCoroutine(playerMovement, true);
                break;

            case WarpType.Jump:
                MovePlayerCoroutine(playerMovement, false);
                break;
        }
    }

    /// <summary>
    /// Coroutine that handles animated movement of the player to the destination
    /// </summary>
    /// <param name="animationTrigger">The name of the animation trigger to activate</param>
    private void MovePlayerCoroutine(PlayerMovement playerMovement, bool isSlide)
    {
        // Trigger the appropriate movement action
        if (isSlide)
        {
            playerMovement.Slide(destinationPoint.position);
        }
        else
        {
            playerMovement.Jump(destinationPoint.position);
        }

    }
}