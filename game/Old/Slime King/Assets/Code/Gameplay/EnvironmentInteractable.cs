using UnityEngine;
using SlimeKing.Core;
using SlimeKing.Gameplay;
using SlimeKing.Gameplay.Combat;
using SlimeKing.Gameplay.Items;
using System.Collections.Generic;

/// <summary>
/// Controla interações com elementos do ambiente que reagem à presença 
/// do jogador ou criaturas, como grama balançando ou água ondulando.
/// </summary>
public class EnvironmentInteractable : Interactable, IDamageable
{
    [Header("Health Settings")]
    [Tooltip("Pontos de vida/resistência inicial do objeto")]
    [SerializeField] private float maxHealth = 1f;

    [Header("Visual Settings")]
    [Tooltip("Animator que controla o efeito visual")]
    [SerializeField] private Animator animator;

    [Tooltip("Tag das entidades que podem ativar o efeito")]
    [SerializeField] private string[] interactableTags = { "Player", "Creature" };

    [Header("Destruction Settings")]
    [Tooltip("Se verdadeiro, o objeto pode ser destruído após receber dano")]
    [SerializeField] private bool isDestructible = false;

    [Tooltip("Quantidade de hits necessários para destruir o objeto")]
    [SerializeField, Min(1)] private int resistance = 1;

    [Header("Drop Settings")]
    [Tooltip("Chance de dropar um item ao ser destruído (0-100%)")]
    [SerializeField, Range(0, 100)] private float dropChance = 20f;

    [Tooltip("Lista de prefabs que podem ser dropados")]
    [SerializeField] private GameObject[] possibleDrops;

    [Tooltip("Quantidade máxima de itens que podem ser dropados")]
    [SerializeField, Min(1)] private int maxDropCount = 1;

    [Header("Hit Effect")]
    [Tooltip("Efeito visual instanciado ao receber dano")]
    [SerializeField] private GameObject hitEffectPrefab; [Header("Audio Settings")]
    [Tooltip("Lista de sons que podem ser reproduzidos durante a interação")]
    [SerializeField] private AudioClip[] interactionSounds;

    [Tooltip("Volume do som")]
    [Range(0, 1)]
    [SerializeField] private float volume = 0.5f;

    #region Private Fields
    private Transform cachedTransform;
    private GameObject particleObject;
    private float currentHealth;
    private static readonly int IsShaking = Animator.StringToHash("Shake");
    private static readonly int IsDestroying = Animator.StringToHash("Destroy");
    private Dictionary<GameObject, Rigidbody2D> cachedRigidbodies;
    #endregion

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        // Cache components and references
        animator ??= GetComponent<Animator>();
        cachedTransform = transform;
        particleObject = transform.Find("particulas")?.gameObject;

        // Initialize cache and state
        cachedRigidbodies = new Dictionary<GameObject, Rigidbody2D>(maxDropCount);
        currentHealth = maxHealth;
    }

    private void OnDestroy()
    {
        cachedRigidbodies?.Clear();
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
        if (animator != null)
        {
            animator.SetTrigger(IsShaking);
        }

        if (particleObject != null)
        {
            particleObject.SetActive(true);
        }

        if (interactionSounds != null && interactionSounds.Length > 0)
        {
            AudioClip randomSound = interactionSounds[Random.Range(0, interactionSounds.Length)];
            GetComponent<AudioSource>()?.PlayOneShot(randomSound, volume);
        }
    }

    public void TriggerDestroy()
    {
        SpawnDrops();

        if (animator != null)
        {
            animator.SetTrigger(IsDestroying); if (particleObject != null)
            {
                particleObject.SetActive(true);
            }

            if (interactionSounds != null && interactionSounds.Length > 0)
            {
                AudioClip randomSound = interactionSounds[Random.Range(0, interactionSounds.Length)];
                CombatEffectsManager.Instance.PlayHitSound(cachedTransform.position, false);
            }
        }
        else
        {
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

    public bool TakeDamage(float damage, GameObject source = null)
    {
        if (currentHealth <= 0) return false;

        currentHealth = Mathf.Max(0, currentHealth - damage);

        // Use CombatEffectsManager to spawn pooled effect
        CombatEffectsManager.Instance.SpawnHitEffect(cachedTransform.position, Quaternion.identity);

        TriggerShake();

        if (currentHealth <= 0)
        {
            TriggerDestroy();
            return true;
        }

        return true;
    }

    private void SpawnDrops()
    {
        if (possibleDrops == null || possibleDrops.Length == 0 ||
            Random.Range(0f, 100f) > dropChance) return;

        int dropCount = Random.Range(1, maxDropCount + 1);
        var position = cachedTransform.position;

        for (int i = 0; i < dropCount; i++)
        {
            GameObject dropPrefab = possibleDrops[Random.Range(0, possibleDrops.Length)];
            if (dropPrefab == null) continue;

            SpawnSingleDrop(dropPrefab, position);
        }
    }

    private void SpawnSingleDrop(GameObject prefab, Vector3 basePosition)
    {
        // Calculate position with offset
        Vector2 randomOffset = Random.insideUnitCircle * 0.3f;
        Vector3 dropPosition = basePosition + new Vector3(randomOffset.x, randomOffset.y, 0);

        // Use ItemPool to instantiate
        var droppedItem = GameUtilities.ItemPool.GetItem(prefab, dropPosition, Quaternion.identity);
        if (droppedItem == null) return;

        // Use Rigidbody2D cache
        if (!cachedRigidbodies.TryGetValue(droppedItem, out var rb))
        {
            rb = droppedItem.GetComponent<Rigidbody2D>();
            if (rb != null)
                cachedRigidbodies[droppedItem] = rb;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.AddForce(new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(0.5f, 1f)
            ) * 2f, ForceMode2D.Impulse);
        }

        // Setup return to pool when collected
        if (droppedItem.TryGetComponent<ICollectable>(out var collectable))
        {
            collectable.OnCollected += () =>
            {
                GameUtilities.ItemPool.ReturnToPool(droppedItem, prefab);
                if (rb != null)
                    cachedRigidbodies.Remove(droppedItem);
            };
        }
    }
}