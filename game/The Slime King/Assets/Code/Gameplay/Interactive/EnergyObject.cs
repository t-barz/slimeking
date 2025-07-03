using UnityEngine;
using TheSlimeKing.Gameplay;
using TheSlimeKing.Core;
using System.Collections;
using System.Collections.Generic;

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

    [Tooltip("Sons que podem ser reproduzidos ao coletar (escolhido aleatoriamente)")]
    [SerializeField] private List<AudioClip> collectSounds = new List<AudioClip>();

    [Tooltip("Animator para controle das animações")]
    [SerializeField] private Animator animator;

    [Header("Cores Elementais")]
    [Tooltip("Cor para energia sem elemento (None)")]
    [SerializeField] private Color noneElementColor = new Color(1f, 1f, 1f);

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

    [Tooltip("Duração mínima do movimento após lançamento (em segundos)")]
    [SerializeField] private float minMoveDuration = 0.1f;

    [Tooltip("Duração máxima do movimento após lançamento (em segundos)")]
    [SerializeField] private float maxMoveDuration = 0.2f;

    [Header("Depuração")]
    [Tooltip("Mostrar mensagens de debug no console")]
    [SerializeField] private bool _showDebugMessages = false;

    // Variáveis de controle
    private GameObject targetPlayer;
    private bool isMovingTowardsPlayer = false;
    private Rigidbody2D rb;
    private bool movementLimited = false;
    private float moveTimer = 0f;
    private float currentMoveDuration; // Duração atual para este objeto
    private bool _soundPlayed = false; // Flag para controlar a execução única do som

    void Start()
    {
        InitializeVisual();

        // Se o Animator não foi configurado, tenta encontrar no GameObject
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        rb = GetComponent<Rigidbody2D>();

        // Gera um tempo de duração aleatório para este objeto
        GenerateRandomMoveDuration();
    }

    /// <summary>
    /// Gera uma duração aleatória de movimento dentro do intervalo configurado
    /// </summary>
    private void GenerateRandomMoveDuration()
    {
        currentMoveDuration = Random.Range(minMoveDuration, maxMoveDuration);
    }

    void Update()
    {
        // Se o Rigidbody2D está ativo e o objeto está em movimento
        if (rb != null && !movementLimited && rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            // Incrementa o timer de movimento
            moveTimer += Time.deltaTime;

            // Se excedeu o tempo de movimento gerado aleatoriamente
            if (moveTimer >= currentMoveDuration)
            {
                // Para o movimento do objeto
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                movementLimited = true;
                if (_showDebugMessages)
                    Debug.Log($"Movimento do objeto de energia limitado após {currentMoveDuration:F2} segundos");
            }
        }

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
                    if (_showDebugMessages)
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
    /// Retorna a cor correspondente ao tipo de elemento, 
    /// com chance de variar entre a cor definida e a cor atual
    /// </summary>
    private Color GetElementColor()
    {
        // Cor base do elemento conforme o tipo
        Color baseColor;
        switch (elementType)
        {
            case ElementType.Earth:
                baseColor = earthElementColor;
                break;
            case ElementType.Water:
                baseColor = waterElementColor;
                break;
            case ElementType.Fire:
                baseColor = fireElementColor;
                break;
            case ElementType.Air:
                baseColor = airElementColor;
                break;
            default:
                baseColor = noneElementColor;
                break;
        }

        // Se já tiver um spriteRenderer com cor definida, tem chance de manter a cor atual
        if (spriteRenderer != null && Random.value > 0.7f) // 30% de chance de manter a cor atual
        {
            // Mistura a cor base com a cor atual para criar uma variação sutil
            return Color.Lerp(baseColor, spriteRenderer.color, 0.4f);
        }

        // Adiciona uma pequena variação aleatória à cor base para mais dinamismo
        return new Color(
            Mathf.Clamp01(baseColor.r + Random.Range(-0.1f, 0.1f)),
            Mathf.Clamp01(baseColor.g + Random.Range(-0.1f, 0.1f)),
            Mathf.Clamp01(baseColor.b + Random.Range(-0.1f, 0.1f)),
            baseColor.a
        );
    }

    /// <summary>
    /// Inicia o processo de movimentação em direção ao jogador
    /// </summary>
    /// <param name="playerObject">GameObject do jogador</param>
    public void MoveTowardsPlayer(GameObject playerObject)
    {
        targetPlayer = playerObject;
        isMovingTowardsPlayer = true;
        moveTimer = 0f;

        if (_showDebugMessages)
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

    public void PlaySound()
    {
        // Se o som já foi tocado uma vez, não toca novamente
        if (_soundPlayed) return;

        // Se houver sons configurados, toca um aleatório
        if (collectSounds != null && collectSounds.Count > 0)
        {
            AudioClip randomClip = collectSounds[Random.Range(0, collectSounds.Count)];
            AudioSource.PlayClipAtPoint(randomClip, transform.position);

            // Marca que o som foi tocado
            _soundPlayed = true;

            if (_showDebugMessages)
                Debug.Log("Som de coleta de energia reproduzido");
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

            if (_showDebugMessages)
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

    /// <summary>
    /// Reinicia o estado de movimento do objeto
    /// Chamado quando o objeto é reciclado do pool
    /// </summary>
    public void ResetMovementState()
    {
        movementLimited = false;
        moveTimer = 0f;
        isMovingTowardsPlayer = false;
        targetPlayer = null;
        _soundPlayed = false; // Reseta a flag de som para permitir que o objeto toque novamente ao ser reutilizado

        // Gera uma nova duração aleatória
        GenerateRandomMoveDuration();

        if (_showDebugMessages)
            Debug.Log($"Estado de movimento reiniciado com nova duração: {currentMoveDuration:F2} segundos");
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
