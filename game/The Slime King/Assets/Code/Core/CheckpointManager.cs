using UnityEngine;

/// <summary>
/// Exemplo de checkpoint que integra com o SavePlayerManager.
/// Automaticamente define pontos de respawn e pode salvar o progresso do jogador.
/// </summary>
public class CheckpointManager : MonoBehaviour
{
    [Header("Configurações do Checkpoint")]
    [Tooltip("Se deve salvar automaticamente quando ativado")]
    [SerializeField] private bool autoSaveOnActivation = true;

    [Tooltip("Se deve definir como ponto de respawn")]
    [SerializeField] private bool setAsRespawnPoint = true;

    [Tooltip("Efeito visual para checkpoint ativado")]
    [SerializeField] private GameObject activationEffect;

    [Tooltip("Som de ativação do checkpoint")]
    [SerializeField] private AudioClip activationSound;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private bool isActivated = false;
    private AudioSource audioSource;

    /// <summary>
    /// Inicialização do checkpoint
    /// </summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Detecta quando o jogador entra no checkpoint
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }

    /// <summary>
    /// Ativa o checkpoint e integra com o SavePlayerManager
    /// </summary>
    private void ActivateCheckpoint()
    {
        isActivated = true;

        if (enableDebugLogs)
        {
            Debug.Log($"[CheckpointManager] Checkpoint ativado: {gameObject.name}");
        }

        // Define como ponto de respawn
        if (setAsRespawnPoint && SavePlayerManager.Instance != null)
        {
            SavePlayerManager.Instance.SetRespawnPoint(transform.position);
        }

        // Salva automaticamente se configurado
        if (autoSaveOnActivation && SavePlayerManager.Instance != null)
        {
            SavePlayerManager.Instance.AutoSavePlayerData();
        }

        // Efeitos visuais e sonoros
        PlayActivationEffects();
    }

    /// <summary>
    /// Reproduz efeitos de ativação
    /// </summary>
    private void PlayActivationEffects()
    {
        // Ativa efeito visual
        if (activationEffect != null)
        {
            activationEffect.SetActive(true);
        }

        // Reproduz som
        if (audioSource != null && activationSound != null)
        {
            audioSource.PlayOneShot(activationSound);
        }
    }

    /// <summary>
    /// Método público para ativar manualmente (útil para outros scripts)
    /// </summary>
    public void ForceActivation()
    {
        if (!isActivated)
        {
            ActivateCheckpoint();
        }
    }

    /// <summary>
    /// Reseta o checkpoint (para teste ou reutilização)
    /// </summary>
    public void ResetCheckpoint()
    {
        isActivated = false;

        if (activationEffect != null)
        {
            activationEffect.SetActive(false);
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[CheckpointManager] Checkpoint resetado: {gameObject.name}");
        }
    }
}
