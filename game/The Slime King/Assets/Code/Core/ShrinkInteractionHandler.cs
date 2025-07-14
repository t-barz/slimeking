using System.Collections;
using UnityEngine;

/// <summary>
/// Componente que ativa a trigger "Shrink" no Animator do Player quando este interage com o objeto.
/// Herda de InteractionHandler e implementa a funcionalidade específica para encolher o jogador.
/// Também move o player para uma posição de destino após encolher.
/// </summary>
public class ShrinkInteractionHandler : InteractionHandler
{
    [Header("Configurações de Encolhimento")]
    [Tooltip("Permitir múltiplas ativações da trigger")]
    [SerializeField] private bool allowMultipleActivations = true;

    [Tooltip("Tempo em segundos para resetar a trigger automaticamente (0 = não resetar)")]
    [SerializeField] private float autoResetTime = 0f;

    [Tooltip("Nome exato da trigger no Animator do player")]
    [SerializeField] private string triggerName = "Shrink";

    [Header("Configurações de Movimento")]
    [Tooltip("Posição para onde o player deve se mover após encolher")]
    [SerializeField] private Transform destinationPoint;

    [Tooltip("Tempo em segundos para o player se deslocar até o destino")]
    [SerializeField] private float movementDuration = 1.0f;

    [Tooltip("Curva de interpolação para o movimento (opcional)")]
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // Controla se a trigger já foi ativada
    private bool triggerActivated = false;

    // Referência ao último player que interagiu
    private Animator playerAnimator = null;
    private GameObject playerObject = null;
    private Collider2D[] playerColliders = null;

    // Timer para resetar a trigger
    private float resetTimer = 0f;

    // Controle da coroutine de movimento
    private Coroutine movementCoroutine = null;

    /// <summary>
    /// Verifica se o ponto de destino está configurado
    /// </summary>
    private void Start()
    {
        if (destinationPoint == null)
        {
            Debug.LogWarning("Ponto de destino não configurado. O player não será movido após encolher.");
        }
    }

    /// <summary>
    /// Sobrescreve o método da classe base para implementar a ativação da trigger "Shrink".
    /// É chamado quando o jogador interage com este objeto enquanto está na sua área de colisão.
    /// </summary>
    protected override void OnInteractionDetected()
    {
        // Executa o comportamento da classe base primeiro
        base.OnInteractionDetected();

        // Verifica se já ativou a trigger e se não permite múltiplas ativações
        if (triggerActivated && !allowMultipleActivations)
        {
            Debug.Log($"Trigger '{triggerName}' já está ativada e múltiplas ativações estão desabilitadas.");
            return;
        }

        // Encontra o objeto Player na área
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player não encontrado para ativar trigger de encolhimento.");
            return;
        }

        // Salva referência ao player
        playerObject = player;

        // Obtém o componente Animator
        playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator == null)
        {
            Debug.LogWarning("Player não possui componente Animator.");
            return;
        }

        // Ativa a trigger
        playerAnimator.SetTrigger(triggerName);
        Debug.Log($"Trigger '{triggerName}' ativada no Animator do Player.");

        // Atualiza estado
        triggerActivated = true;

        // Cache todos os colliders do player
        playerColliders = playerObject.GetComponentsInChildren<Collider2D>();

        // Desativa os colliders do player
        SetPlayerCollidersEnabled(false);

        // Inicia o movimento do player se o ponto de destino estiver configurado
        if (destinationPoint != null)
        {
            // Cancela qualquer movimento em andamento
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
            }

            // Inicia a nova coroutine de movimento
            movementCoroutine = StartCoroutine(MovePlayerToDestination());
        }

        // Inicia timer se auto-reset estiver configurado
        if (autoResetTime > 0f)
        {
            resetTimer = Time.time + autoResetTime;
            // Inicia verificação do timer apenas se ainda não estiver verificando
            if (!IsInvoking(nameof(CheckResetTimer)))
            {
                InvokeRepeating(nameof(CheckResetTimer), 0.1f, 0.1f);
            }
        }
    }

    /// <summary>
    /// Coroutine para mover o player suavemente até a posição de destino
    /// </summary>
    private IEnumerator MovePlayerToDestination()
    {
        if (playerObject == null || destinationPoint == null) yield break;

        Vector3 startPosition = playerObject.transform.position;
        Vector3 targetPosition = destinationPoint.position;
        float startTime = Time.time;
        float elapsedTime = 0f;

        Debug.Log($"Iniciando deslocamento do player para o destino. Duração: {movementDuration} segundos");

        // Executa o movimento suave
        while (elapsedTime < movementDuration)
        {
            elapsedTime = Time.time - startTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / movementDuration);

            // Aplica a curva de interpolação, se houver
            float curveValue = movementCurve.Evaluate(normalizedTime);

            // Interpola a posição
            playerObject.transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

            yield return null;
        }

        // Garante que o player chegue exatamente na posição final
        playerObject.transform.position = targetPosition;

        Debug.Log("Player chegou ao destino. Reativando colliders.");

        // Reativa os colliders quando chegar ao destino
        SetPlayerCollidersEnabled(true);

        movementCoroutine = null;
    }

    /// <summary>
    /// Ativa ou desativa todos os colliders do player
    /// </summary>
    private void SetPlayerCollidersEnabled(bool enabled)
    {
        if (playerColliders == null || playerColliders.Length == 0) return;

        foreach (var collider in playerColliders)
        {
            if (collider != null)
            {
                collider.enabled = enabled;
            }
        }

        Debug.Log($"Colliders do player {(enabled ? "ativados" : "desativados")}");
    }

    /// <summary>
    /// Verifica se é hora de resetar a trigger.
    /// Chamado periodicamente se o auto-reset estiver ativado.
    /// </summary>
    private void CheckResetTimer()
    {
        if (resetTimer <= Time.time && triggerActivated)
        {
            ResetShrinkTrigger();
            CancelInvoke(nameof(CheckResetTimer));
        }
    }

    /// <summary>
    /// Reseta a trigger de encolhimento, permitindo nova ativação.
    /// </summary>
    public void ResetShrinkTrigger()
    {
        triggerActivated = false;
        Debug.Log($"Trigger '{triggerName}' resetada e pronta para nova ativação.");

        // Se ainda tiver referência ao Animator, reseta a trigger nele também
        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger(triggerName);
        }

        // Reativa os colliders caso ainda estejam desativados
        SetPlayerCollidersEnabled(true);
    }

    /// <summary>
    /// Chamado quando o objeto é desabilitado. Limpa qualquer estado pendente.
    /// </summary>
    protected override void OnDisable()
    {
        // Chama o método da classe base primeiro
        base.OnDisable();

        // O comportamento de desinscrição de eventos deve ser gerenciado pela classe base

        // Interrompe a coroutine de movimento se estiver em andamento
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        // Reativa os colliders do player para garantir que não fiquem desativados permanentemente
        SetPlayerCollidersEnabled(true);

        // Cancela qualquer invocação pendente
        CancelInvoke(nameof(CheckResetTimer));

        // Reseta estados
        triggerActivated = false;
        playerAnimator = null;
        playerObject = null;
        playerColliders = null;
    }
}