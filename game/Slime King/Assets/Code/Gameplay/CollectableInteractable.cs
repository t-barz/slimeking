using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Implementa um objeto coletável que se move em direção ao jogador quando próximo ou por interação
/// </summary>
public class CollectableInteractable : Interactable
{
    [Header("Configurações de Coleta")]
    [Tooltip("Velocidade de movimento até o jogador")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("Distância mínima para considerar coletado")]
    [SerializeField] private float collectDistance = 0.1f;

    [Tooltip("Prefab do efeito visual de coleta")]
    [SerializeField] private GameObject collectEffectPrefab;

    [Tooltip("Se verdadeiro, move automaticamente quando próximo. Se falso, requer interação")]
    [SerializeField] private bool autoCollectInRange = true;

    [Tooltip("Referência para a ação de interação")]
    [SerializeField] private InputActionReference interactAction;

    private Transform playerTransform;
    private bool isMovingToPlayer;
    private GameObject currentTarget;

    protected virtual void Start()
    {
        // Encontra o jogador na cena
        currentTarget = GameObject.FindGameObjectWithTag("Player");
        if (currentTarget != null)
        {
            playerTransform = currentTarget.transform;
        }
    }

    protected virtual void OnEnable()
    {
        if (!autoCollectInRange && interactAction != null)
        {
            interactAction.action.performed += OnInteractPerformed;
            interactAction.action.Enable();
        }
    }


    protected virtual void OnDisable()
    {
        if (!autoCollectInRange && interactAction != null)
        {
            interactAction.action.performed -= OnInteractPerformed;
            interactAction.action.Disable();
        }
    }

    protected virtual void Update()
    {
        if (autoCollectInRange && PlayerInRange && !isMovingToPlayer)
        {
            StartCoroutine(MoveToPlayer());
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (PlayerInRange && !isMovingToPlayer)
        {
            StartCoroutine(MoveToPlayer());
        }
    }

    /// <summary>
    /// Coroutine que move o objeto até o jogador
    /// </summary>
    private IEnumerator MoveToPlayer()
    {
        isMovingToPlayer = true;

        while (PlayerInRange && playerTransform != null)
        {
            // Calcula a direção e distância até o jogador
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            float distanceToPlayer = directionToPlayer.magnitude;

            // Se chegou perto o suficiente, coleta o item
            if (distanceToPlayer <= collectDistance)
            {
                CollectItem();
                yield break;
            }

            // Move em direção ao jogador
            transform.position = Vector3.MoveTowards(
                transform.position,
                playerTransform.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        isMovingToPlayer = false;
    }

    /// <summary>
    /// Chamado quando o item é coletado
    /// </summary>
    protected virtual void CollectItem()
    {
        // Spawna o efeito visual se existir
        if (collectEffectPrefab != null)
        {
            Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
        }

        // Aqui você pode adicionar lógica específica de coleta
        // como adicionar ao inventário, aumentar pontuação, etc.

        // Destroi o objeto coletável
        Destroy(gameObject);
    }

    public override void Interact()
    {
        if (!autoCollectInRange && PlayerInRange && !isMovingToPlayer)
        {
            StartCoroutine(MoveToPlayer());
        }
    }
}