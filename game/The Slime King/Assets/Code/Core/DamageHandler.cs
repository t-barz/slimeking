using UnityEngine;
using System.Linq;
using System.Collections;

/// <summary>
/// Gerencia o comportamento de dano quando ocorre colisão com outros objetos.
/// </summary>
public class DamageHandler : MonoBehaviour
{
    #region Configurações
    [Header("Configurações de Debug")]
    [Tooltip("Ativa ou desativa os logs de debug")]
    [SerializeField] private bool enableDebugLogs = false;

    [Tooltip("Raio de busca para encontrar o jogador")]
    [SerializeField] private float detectionRadius = 0.5f;

    [Header("Configurações de Destruição")]
    [Tooltip("Duração do efeito de fade-out em segundos")]
    [SerializeField] private float fadeOutDuration = 1.0f;

    [Tooltip("Curva de animação para o fade-out (0,0 a 1,1)")]
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    #endregion

    #region Variáveis Privadas
    // Cache do componente EntityStatus para evitar GetComponent repetidos
    private EntityStatus myEntityStatus;

    // Cache do componente Animator
    private Animator animator;

    // Controla se o objeto já entrou no estado de dano
    private bool isDamagedStateActivated = false;

    // Controla se o objeto já entrou no estado de destruído
    private bool isDestroyedStateActivated = false;
    #endregion

    #region Inicialização
    private void Awake()
    {
        // Inicializa o EntityStatus
        myEntityStatus = GetComponent<EntityStatus>();

        if (myEntityStatus == null && enableDebugLogs)
        {
            Debug.LogWarning($"DamageHandler em {gameObject.name} não tem EntityStatus associado");
        }

        // Inicializa o Animator
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            // Tenta buscar no filho ou no pai se não encontrar diretamente
            animator = GetComponentInChildren<Animator>() ?? GetComponentInParent<Animator>();

            if (animator == null && enableDebugLogs)
            {
                Debug.LogWarning($"DamageHandler em {gameObject.name} não tem Animator associado");
            }
        }
    }
    #endregion

    #region Detecção de Colisão
    /// <summary>
    /// Chamado quando outro collider entra na área de trigger deste objeto.
    /// </summary>
    public void ProcessHit()
    {
        FindAndProcessNearestPlayer();
    }

    /// <summary>
    /// Busca o jogador mais próximo e processa a colisão se encontrado
    /// </summary>
    private void FindAndProcessNearestPlayer()
    {
        // Busca todos os colliders próximos dentro do raio de detecção
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        // Filtra apenas os que têm a tag "Player"
        GameObject nearestPlayer = nearbyColliders
            .Where(col => col.CompareTag("Player"))
            .OrderBy(col => Vector2.Distance(transform.position, col.transform.position))
            .FirstOrDefault()?.gameObject;

        if (nearestPlayer != null)
        {
            ProcessPlayerCollision(nearestPlayer);
        }
    }

    /// <summary>
    /// Processa a colisão com o jogador, aplicando dano se possível
    /// </summary>
    private void ProcessPlayerCollision(GameObject player)
    {
        // Log de debug
        if (enableDebugLogs)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            Debug.Log($"Player encontrado: {player.name} - Distância: {distance:F2}");
        }

        // Verifica se o EntityStatus foi encontrado
        if (myEntityStatus == null)
        {
            if (enableDebugLogs)
                Debug.LogWarning($"Este objeto ({gameObject.name}) não possui um EntityStatus válido");
            return;
        }

        // Tenta obter o EntityStatus do jogador
        EntityStatus playerStatus = player.GetComponent<EntityStatus>();
        if (playerStatus == null)
        {
            if (enableDebugLogs)
                Debug.Log("Player não possui componente EntityStatus");
            return;
        }

        // Agora que verificamos que ambos são válidos, podemos aplicar o dano
        int damageDealt = myEntityStatus.TakeDamage(playerStatus.GetAttack());

        // Log de dano
        if (enableDebugLogs)
        {
            Debug.Log($"{gameObject.name} recebeu {damageDealt} de dano. HP restante: {myEntityStatus.currentHP}");
        }

        // Atualiza o estado de animação com base na saúde atual
        UpdateAnimationState();
    }

    /// <summary>
    /// Atualiza os parâmetros do Animator com base na saúde atual
    /// </summary>
    private void UpdateAnimationState()
    {
        // Se não tiver Animator, não faz nada
        if (animator == null || myEntityStatus == null)
            return;

        // Calcula a metade da vida máxima
        int halfMaxHp = myEntityStatus.baseHP / 2;

        // Verifica se está destruído (HP <= 0)
        if (myEntityStatus.currentHP <= 0 && !isDestroyedStateActivated)
        {
            isDestroyedStateActivated = true;
            animator.SetBool("isDestroyed", true);

            if (enableDebugLogs)
                Debug.Log($"{gameObject.name} entrou no estado de destruído");

            // Processa os drops se tiver um DropManager
            DropManager dropManager = GetComponent<DropManager>();
            if (dropManager != null)
            {
                if (enableDebugLogs)
                    Debug.Log($"Gerando drops para {gameObject.name}");

                dropManager.DropItems();
            }

            // Inicia o efeito de fade-out e destruição
            StartCoroutine(FadeOutAndDestroy());
        }
        // Verifica se está danificado (HP <= metade do máximo)
        else if (myEntityStatus.currentHP <= halfMaxHp && !isDamagedStateActivated)
        {
            isDamagedStateActivated = true;
            animator.SetBool("isDamaged", true);

            if (enableDebugLogs)
                Debug.Log($"{gameObject.name} entrou no estado de danificado");
        }
    }

    /// <summary>
    /// Realiza o fade-out gradual de todos os sprites e destrói o objeto ao final.
    /// Remove todos os colliders para evitar interações durante o desaparecimento.
    /// </summary>
    private IEnumerator FadeOutAndDestroy()
    {
        // Obtém todos os SpriteRenderer do objeto e seus filhos
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        // Se não encontrar nenhum sprite, destrói imediatamente
        if (sprites.Length == 0)
        {
            Destroy(gameObject);
            yield break;
        }

        // NOVO: Remover todos os colliders do objeto e seus filhos
        RemoveAllColliders();

        // Guarda as cores originais de cada sprite
        Color[] originalColors = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            originalColors[i] = sprites[i].color;
        }

        // Tempo inicial
        float startTime = Time.time;
        float elapsedTime = 0f;

        // Continua enquanto não completar o tempo de fade
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime = Time.time - startTime;
            float normalizedTime = elapsedTime / fadeOutDuration;

            // Calcula o valor de alpha usando a curva de animação
            float alpha = fadeOutCurve.Evaluate(normalizedTime);

            // Aplica o alpha a todos os sprites
            for (int i = 0; i < sprites.Length; i++)
            {
                Color newColor = originalColors[i];
                newColor.a = alpha;
                sprites[i].color = newColor;
            }

            yield return null;
        }

        // Certifica-se que todos os sprites estão completamente transparentes
        foreach (SpriteRenderer sprite in sprites)
        {
            Color finalColor = sprite.color;
            finalColor.a = 0;
            sprite.color = finalColor;
        }

        // Aguarda um frame adicional para que o efeito visual seja percebido
        yield return null;

        // Destrói o objeto
        if (enableDebugLogs)
            Debug.Log($"{gameObject.name} foi destruído");

        Destroy(gameObject);
    }

    /// <summary>
    /// Remove todos os colliders do objeto e seus filhos para evitar interações durante o fade-out.
    /// </summary>
    private void RemoveAllColliders()
    {
        // Remove colliders 2D
        Collider2D[] colliders2D = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in colliders2D)
        {
            if (enableDebugLogs)
                Debug.Log($"Removendo collider: {collider.name}");

            Destroy(collider);
        }

        // Remove também colliders 3D caso existam
        Collider[] colliders3D = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders3D)
        {
            if (enableDebugLogs)
                Debug.Log($"Removendo collider 3D: {collider.name}");

            Destroy(collider);
        }

        // Se o objeto tiver Rigidbody, podemos também desativá-lo para evitar física
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.simulated = false;
        }
    }

    /// <summary>
    /// Desenha o raio de detecção no editor
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (enableDebugLogs)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
    #endregion
}
