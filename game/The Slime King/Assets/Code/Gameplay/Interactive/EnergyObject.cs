using UnityEngine;
using TheSlimeKing.Gameplay;
using TheSlimeKing.Core;

/// <summary>
/// Representa um objeto de energia coletável no jogo
/// </summary>
public class EnergyObject : MonoBehaviour
{
    [Header("Configurações de Energia")]
    [Tooltip("Quantidade de energia que será adicionada ao personagem")]
    [SerializeField] private int energyAmount = 10;

    [Tooltip("Tipo de elemento da energia")]
    [SerializeField] private ElementType elementType = ElementType.None;

    // Propriedades para acessar os valores
    public int EnergyAmount => energyAmount;
    public ElementType Element => elementType;

    [Header("Visual")]
    [Tooltip("Efeito de partícula para visualização")]
    [SerializeField] private ParticleSystem collectEffect;

    [Tooltip("Referência ao componente visual (opcional)")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Tooltip("Som reproduzido ao coletar")]
    [SerializeField] private AudioClip collectSound;

    [Tooltip("Animator para controle das animações")]
    [SerializeField] private Animator animator;

    [Header("Cores Elementais")]
    [Tooltip("Cor para energia sem elemento (None)")]
    [SerializeField] private Color noneElementColor = new Color(0.8f, 0.8f, 0.8f);

    [Tooltip("Cor para energia do elemento Terra (Earth)")]
    [SerializeField] private Color earthElementColor = new Color(0.5f, 0.3f, 0.0f);

    [Tooltip("Cor para energia do elemento Água (Water)")]
    [SerializeField] private Color waterElementColor = new Color(0.0f, 0.5f, 1.0f);

    [Tooltip("Cor para energia do elemento Fogo (Fire)")]
    [SerializeField] private Color fireElementColor = new Color(1.0f, 0.3f, 0.0f);

    [Tooltip("Cor para energia do elemento Ar (Air)")]
    [SerializeField] private Color airElementColor = new Color(0.7f, 1.0f, 1.0f);

    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade de deslocamento em direção ao jogador")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("Precisão para considerar chegada ao destino (valor pequeno)")]
    [SerializeField] private float arrivalPrecision = 0.05f;

    // Variáveis de controle
    private GameObject targetPlayer;
    private bool isMovingTowardsPlayer = false;

    void Start()
    {
        InitializeVisual();

        // Se o Animator não foi configurado, tenta encontrar no GameObject
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        // Se está se movendo em direção ao jogador
        if (isMovingTowardsPlayer && targetPlayer != null)
        {
            // Obtém a posição exata do pivot do jogador
            Vector3 playerPosition = targetPlayer.transform.position;

            // Calcula a distância exata entre a energia e o pivot do jogador
            Vector3 direction = playerPosition - transform.position;
            float distanceToPlayer = direction.magnitude;

            // Se a distância for muito pequena, considere que chegou ao destino
            if (distanceToPlayer <= arrivalPrecision)
            {
                // Posiciona exatamente no pivot do jogador para garantir precisão
                transform.position = playerPosition;

                // Aciona a trigger de absorção
                if (animator != null)
                {
                    animator.SetTrigger("Absorv");
                    Debug.Log("Trigger de absorção acionada - posição exata");
                }

                // Para de se mover em direção ao jogador
                isMovingTowardsPlayer = false;

                // Coleta a energia após a animação (deve ser chamado por um Animation Event)
                // Neste ponto não destruímos o objeto, apenas acionamos a animação
            }
            else
            {
                // Normaliza a direção
                direction.Normalize();

                // Move em direção ao jogador
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Inicializa a aparência visual baseada no tipo elemental
    /// </summary>
    private void InitializeVisual()
    {
        // Se tiver um sprite renderer, aplica a cor correspondente ao elemento
        if (spriteRenderer != null)
        {
            spriteRenderer.color = GetElementColor();
        }

        // Configura o sistema de partículas, se existir
        if (collectEffect != null)
        {
            var main = collectEffect.main;
            main.startColor = GetElementColor();
        }
    }

    /// <summary>
    /// Retorna a cor correspondente ao tipo de elemento
    /// </summary>
    private Color GetElementColor()
    {
        switch (elementType)
        {
            case ElementType.Earth:
                return earthElementColor;
            case ElementType.Water:
                return waterElementColor;
            case ElementType.Fire:
                return fireElementColor;
            case ElementType.Air:
                return airElementColor;
            default:
                return noneElementColor;
        }
    }

    /// <summary>
    /// Inicia o processo de movimentação em direção ao jogador
    /// </summary>
    /// <param name="playerObject">GameObject do jogador</param>
    public void MoveTowardsPlayer(GameObject playerObject)
    {
        targetPlayer = playerObject;
        isMovingTowardsPlayer = true;

        Debug.Log($"Energia começou a se mover em direção ao jogador: {elementType}");
    }

    /// <summary>
    /// Método a ser chamado por um Animation Event quando a animação de absorção terminar
    /// </summary>
    public void OnAbsorptionComplete()
    {
        // Verifica se existe um jogador alvo
        if (targetPlayer != null)
        {
            ApplyEnergyToPlayer();
        }
    }

    /// <summary>
    /// Aplica a energia ao jogador e reproduz efeitos visuais/sonoros
    /// </summary>
    private void ApplyEnergyToPlayer()
    {
        // Verifica se o coletor é o personagem do jogador
        SlimeStats slimeStats = targetPlayer.GetComponent<SlimeStats>();
        if (slimeStats != null)
        {
            // Aplica a energia ao personagem usando o método existente
            slimeStats.AddElementalEnergy(elementType, energyAmount);

            Debug.Log($"Energia absorvida: {energyAmount} pontos do elemento {elementType}");

            // Reproduz efeito de coleta, se existir
            if (collectEffect != null)
            {
                // Desacopla o efeito do objeto para que não seja destruído junto
                collectEffect.transform.SetParent(null);
                collectEffect.Play();

                // Destruir o sistema de partículas após a duração
                Destroy(collectEffect.gameObject, collectEffect.main.duration);
            }

            // Reproduz som de coleta, se disponível
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // Não destruímos o objeto automaticamente mais, isso deve ser feito 
            // via Animation Event ou outro controle externo quando apropriado
            // Destroy(gameObject);
        }
    }

    /// <summary>
    /// Destrói este objeto de energia
    /// Pode ser chamado por Animation Event
    /// </summary>
    public void DestroyEnergy()
    {
        Destroy(gameObject);
    }

    // Detecta colisão para iniciar movimentação em direção ao jogador
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o que colidiu é o jogador
        if (other.CompareTag("Player") && !isMovingTowardsPlayer)
        {
            MoveTowardsPlayer(other.gameObject);
        }
    }
}
