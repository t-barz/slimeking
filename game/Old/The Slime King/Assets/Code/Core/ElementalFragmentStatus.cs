using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Define as propriedades e comportamentos de um fragmento elemental coletável.
/// </summary>
public class ElementalFragmentStatus : MonoBehaviour
{
    /// <summary>
    /// Tipos de elementos disponíveis para fragmentos elementais.
    /// </summary>
    public enum ElementType
    {
        Fire,
        Water,
        Earth,
        Air
    }

    [Header("Configurações do Fragmento")]
    [Tooltip("O tipo de elemento deste fragmento")]
    [SerializeField] private ElementType elementType = ElementType.Fire;

    [Tooltip("Quantidade de pontos que este fragmento vale")]
    [SerializeField] private int pointValue = 1;

    [Tooltip("Efeito visual ao coletar (opcional)")]
    [SerializeField] private GameObject collectEffect;

    [Header("Configurações de Movimentação")]
    [Tooltip("Velocidade de deslocamento em direção ao jogador")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("Aceleração do movimento ao se aproximar do jogador")]
    [SerializeField] private float acceleration = 1.5f;

    [Tooltip("Distância mínima para considerar que chegou ao jogador")]
    [SerializeField] private float collectDistance = 0.5f;

    [Tooltip("Atraso inicial antes de começar a se mover (segundos)")]
    [SerializeField] private float initialDelay = 0.2f;

    [Header("Configurações de Aparência")]
    [Tooltip("Variação máxima de matiz da cor (0-1)")]
    [Range(0f, 0.1f)]
    [SerializeField] private float hueVariation = 0.05f;

    [Tooltip("Variação máxima de saturação da cor (0-1)")]
    [Range(0f, 0.2f)]
    [SerializeField] private float saturationVariation = 0.1f;

    [Tooltip("Variação máxima de brilho da cor (0-1)")]
    [Range(0f, 0.2f)]
    [SerializeField] private float valueVariation = 0.1f;

    [Header("Configurações de Som")]
    [Tooltip("Lista de sons possíveis para tocar ao coletar o fragmento")]
    [SerializeField] private List<AudioClip> absorbSounds = new List<AudioClip>();

    [Tooltip("Volume dos sons de absorção")]
    [Range(0f, 1f)]
    [SerializeField] private float absorbSoundVolume = 0.7f;

    [Tooltip("Variação de pitch para sons (±)")]
    [Range(0f, 0.3f)]
    [SerializeField] private float pitchVariation = 0.1f;

    [Header("Configurações de Debug")]
    [Tooltip("Ativa logs de debug para este fragmento")]
    [SerializeField] private bool enableDebugLogs = false;

    // Variáveis de estado
    private bool isMovingToPlayer = false;
    private Transform playerTransform;
    private float currentSpeed;
    private Vector3 initialPosition;
    private float movementTimer;
    private Animator animator;

    // Cache do AudioSource
    private AudioSource audioSource;

    /// <summary>
    /// Obtém o tipo de elemento deste fragmento.
    /// </summary>
    public ElementType Type => elementType;

    /// <summary>
    /// Obtém a quantidade de pontos que este fragmento vale.
    /// </summary>
    public int Points => pointValue;

    /// <summary>
    /// Inicializa o fragmento com uma pequena variação de cor
    /// </summary>
    private void Start()
    {
        // Busca o animator no objeto ou seus filhos
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // Inicializa o AudioSource para os efeitos sonoros
        SetupAudioSource();

        ApplyColorVariation();
        initialPosition = transform.position;
    }

    /// <summary>
    /// Configura o AudioSource para tocar os sons
    /// </summary>
    private void SetupAudioSource()
    {
        // Busca AudioSource existente ou cria um novo
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();

            // Configura o AudioSource para sons de efeito
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 1f; // Som 3D
        }
    }

    /// <summary>
    /// Atualiza o movimento do fragmento em direção ao jogador
    /// </summary>
    private void Update()
    {
        if (isMovingToPlayer && playerTransform != null)
        {
            // Incrementa o timer de movimento
            movementTimer += Time.deltaTime;

            // Só começa a mover após o delay inicial
            if (movementTimer < initialDelay)
                return;

            // Calcula a distância atual até o jogador
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            // Aumenta a velocidade gradualmente conforme se aproxima do jogador
            float speedMultiplier = 1 + (acceleration * (1 - distanceToPlayer / collectDistance));
            currentSpeed = moveSpeed * speedMultiplier * Time.deltaTime;

            // Move em direção ao jogador
            transform.position = Vector3.MoveTowards(
                transform.position,
                playerTransform.position,
                currentSpeed);

            // Verifica se chegou perto o suficiente para coletar
            if (distanceToPlayer <= collectDistance)
            {
                if (enableDebugLogs)
                    Debug.Log($"Fragmento {elementType} chegou ao jogador");

                OnCollect();
            }
        }
    }

    /// <summary>
    /// Detecta quando um objeto entra no trigger do fragmento
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto é o jogador
        if (other.CompareTag("Player") && !isMovingToPlayer)
        {
            playerTransform = other.transform;
            isMovingToPlayer = true;
            movementTimer = 0f;

            if (enableDebugLogs)
                Debug.Log($"Fragmento {elementType} atraído pelo jogador");

            // Opcional: Efeito visual ou sonoro para indicar a atração
        }
    }

    /// <summary>
    /// Aplica uma pequena variação aleatória na cor dos sprites do objeto
    /// </summary>
    private void ApplyColorVariation()
    {
        // Obtém todos os SpriteRenderer do objeto e filhos
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        if (renderers.Length == 0)
            return;

        foreach (SpriteRenderer renderer in renderers)
        {
            // Obtém a cor original do sprite
            Color originalColor = renderer.color;

            // Converte para HSV para fazer ajustes mais naturais
            Color.RGBToHSV(originalColor, out float h, out float s, out float v);

            // Aplica variações aleatórias dentro dos limites definidos
            h += Random.Range(-hueVariation, hueVariation);
            s += Random.Range(-saturationVariation, saturationVariation);
            v += Random.Range(-valueVariation, valueVariation);

            // Mantém os valores dentro de limites válidos (0-1)
            h = Mathf.Repeat(h, 1f); // Hue é cíclico, então usamos Repeat
            s = Mathf.Clamp01(s);
            v = Mathf.Clamp01(v);

            // Converte de volta para RGB
            Color variedColor = Color.HSVToRGB(h, s, v);

            // Mantém o alpha original
            variedColor.a = originalColor.a;

            // Aplica a nova cor
            renderer.color = variedColor;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"Variação de cor aplicada ao fragmento {elementType}");
        }
    }

    /// <summary>
    /// Configura o tipo e valor do fragmento.
    /// </summary>
    public void SetFragmentValues(ElementType type, int points)
    {
        elementType = type;
        pointValue = points;

        if (enableDebugLogs)
        {
            Debug.Log($"Fragmento configurado: {elementType}, {pointValue} pontos");
        }

        // Aplica variação de cor quando os valores são alterados programaticamente
        ApplyColorVariation();
    }

    /// <summary>
    /// Chamado quando o fragmento é coletado.
    /// </summary>
    public void OnCollect()
    {
        // Spawna efeito de coleta se configurado
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // Toca um som de absorção aleatório
        //PlayRandomAbsorbSound();

        // Ativa a trigger de absorção no animator
        if (animator != null)
        {
            animator.SetTrigger("Absorv");

            // Se tiver uma animação para tocar, esperamos ela terminar antes de destruir
            StartCoroutine(DestroyAfterAnimation());
        }
        else
        {
            // Se não tiver animator, destrói imediatamente
            DestroyFragment();
        }

        if (enableDebugLogs)
        {
            Debug.Log($"Fragmento {elementType} coletado: +{pointValue} pontos");
        }
    }

    /// <summary>
    /// Seleciona e toca um som aleatório da lista de sons de absorção
    /// </summary>
    public void PlayRandomAbsorbSound()
    {
        // Verifica se temos sons para tocar
        if (absorbSounds == null || absorbSounds.Count == 0 || audioSource == null)
            return;

        // Seleciona um som aleatório da lista
        AudioClip selectedSound = absorbSounds[Random.Range(0, absorbSounds.Count)];

        // Verifica se o som selecionado é válido
        if (selectedSound == null)
            return;

        // Aplica uma pequena variação aleatória no pitch para evitar monotonia
        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);

        // Toca o som no volume configurado
        audioSource.PlayOneShot(selectedSound, absorbSoundVolume);

        if (enableDebugLogs)
        {
            Debug.Log($"Tocando som de absorção: {selectedSound.name}");
        }
    }

    /// <summary>
    /// Espera a animação terminar antes de destruir o fragmento
    /// </summary>
    private IEnumerator DestroyAfterAnimation()
    {
        // Espera um frame para garantir que a animação começou
        yield return null;

        // Se tiver um animator, espera até a animação terminar
        if (animator != null)
        {
            // Obtém a duração da animação atual
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float animationTime = stateInfo.length;

            // Espera pelo menos 0.5 segundos ou a duração da animação
            yield return new WaitForSeconds(Mathf.Max(0.5f, animationTime));
        }

        // Destrói o fragmento
        DestroyFragment();
    }

    /// <summary>
    /// Destrói o fragmento
    /// </summary>
    private void DestroyFragment()
    {
        Destroy(gameObject);
    }
}
