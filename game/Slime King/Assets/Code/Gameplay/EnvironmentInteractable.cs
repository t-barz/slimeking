using UnityEngine;

/// <summary>
/// Controla interações com elementos do ambiente que reagem à presença 
/// do jogador ou criaturas, como grama balançando ou água ondulando.
/// </summary>
public class EnvironmentInteractable : Interactable
{
    [Header("Visual Settings")]
    [Tooltip("Animator que controla o efeito visual")]
    [SerializeField] private Animator animator;

    [Tooltip("Tag das entidades que podem ativar o efeito")]
    [SerializeField] private string[] interactableTags = { "Player", "Creature" };

    [Header("Audio Settings")]
    [Tooltip("Som reproduzido durante a interação")]
    [SerializeField] private AudioClip interactionSound;
    [Tooltip("Volume do som")]
    [Range(0, 1)]
    [SerializeField] private float volume = 0.5f;

    private AudioSource audioSource;
    private float lastEffectTime;
    private static readonly int IsShaking = Animator.StringToHash("Shake");
    private static readonly int IsDestroying = Animator.StringToHash("Destroy");

    private GameObject particleObject;

    private void Awake()
    {
        // Get or add required components
        if (animator == null) animator = GetComponent<Animator>();

        // Cache particles reference
        particleObject = transform.Find("particulas")?.gameObject;

        if (interactionSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = interactionSound;
            audioSource.volume = volume;
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.playOnAwake = false;
        }
    }

    /// <summary>
    /// Destroi este objeto imediatamente
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsValidInteractor(other.tag))
        {
            TriggerShake();
        }
    }

    private bool IsValidInteractor(string tag)
    {
        return System.Array.Exists(interactableTags, t => t == tag);
    }

    private void TriggerShake()
    {
        // Trigger shake animation
        if (animator != null)
        {
            animator.SetTrigger(IsShaking);
        }

        // Activate particles
        if (particleObject != null)
        {
            particleObject.SetActive(true);
        }

        // Play sound if configured
        if (audioSource != null && interactionSound != null)
        {
            audioSource.Play();
        }
    }

    /// <summary>
    /// Inicia a animação de destruição do objeto
    /// </summary>
    public void TriggerDestroy()
    {
        if (animator != null)
        {
            animator.SetTrigger(IsDestroying);

            // Activate particles
            if (particleObject != null)
            {
                particleObject.SetActive(true);
            }

            // Play sound if configured
            if (audioSource != null && interactionSound != null)
            {
                audioSource.Play();
            }
        }
        else
        {
            // If no animator, destroy immediately
            Destroy(gameObject);
        }
    }

    protected override void ShowVisualFeedback()
    {
        // Optional: Add highlight or other visual cue when player is near
    }

    protected override void HideVisualFeedback()
    {
        // Optional: Remove highlight or visual cue
    }

    public override void Interact()
    {
        // This type auto-interacts on collision, no manual interaction needed
    }
}