using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Componente que ativa a trigger "Shake" no Animator de objetos que colidem com este.
/// Também inicia a simulação de um sistema de partículas configurado e reproduz sons.
/// </summary>
public class ShakeHandler : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Nome exato da trigger no Animator do objeto")]
    [SerializeField] private string triggerName = "Shake";

    [Tooltip("Ativar apenas para objetos com tag específica (deixe vazio para todos)")]
    [SerializeField] private string targetTag = "Player";

    [Header("Efeitos de Partículas")]
    [Tooltip("Sistema de partículas que será ativado junto com a trigger")]
    [SerializeField] private ParticleSystem particleEffect;

    [Header("Efeitos Sonoros")]
    [Tooltip("Lista de sons possíveis para tocar ao acionar o shake")]
    [SerializeField] private List<AudioClip> shakeSounds = new List<AudioClip>();

    [Tooltip("Volume dos sons de shake")]
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.7f;

    [Tooltip("Variação de pitch para sons (±)")]
    [Range(0f, 0.3f)]
    [SerializeField] private float pitchVariation = 0.1f;

    [Tooltip("Ativar logs de debug")]
    [SerializeField] private bool enableDebugLogs = false;

    // AudioSource para efeitos sonoros
    private AudioSource audioSource;

    /// <summary>
    /// Inicializa o componente e configura o AudioSource
    /// </summary>
    private void Start()
    {
        SetupAudioSource();
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
    /// Detecta quando um objeto entra na área de trigger
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se precisa filtrar por tag
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
            return;

        // Tenta obter o Animator do objeto que colidiu
        Animator animator = GetComponent<Animator>();

        // Se encontrou um Animator, ativa a trigger
        if (animator != null)
        {
            animator.SetTrigger(triggerName);

            // Reproduz um som aleatório
            PlayRandomShakeSound();

            // Inicia a simulação do sistema de partículas
            PlayParticleEffect();

            if (enableDebugLogs)
            {
                Debug.Log($"Trigger '{triggerName}' ativada com efeitos de partículas e som");
            }
        }
        else if (enableDebugLogs)
        {
            Debug.LogWarning($"Objeto não possui componente Animator");
        }
    }

    /// <summary>
    /// Seleciona e toca um som aleatório da lista de sons de shake
    /// </summary>
    private void PlayRandomShakeSound()
    {
        // Verifica se temos sons para tocar
        if (shakeSounds == null || shakeSounds.Count == 0 || audioSource == null)
            return;

        // Seleciona um som aleatório da lista
        AudioClip selectedSound = shakeSounds[Random.Range(0, shakeSounds.Count)];

        // Verifica se o som selecionado é válido
        if (selectedSound == null)
            return;

        // Aplica uma pequena variação aleatória no pitch para evitar monotonia
        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);

        // Toca o som no volume configurado
        audioSource.PlayOneShot(selectedSound, soundVolume);

        if (enableDebugLogs)
            Debug.Log($"Tocando som de shake: {selectedSound.name}");
    }

    /// <summary>
    /// Inicia a simulação do sistema de partículas configurado
    /// </summary>
    private void PlayParticleEffect()
    {
        if (particleEffect == null)
        {
            if (enableDebugLogs)
                Debug.Log("Nenhum sistema de partículas configurado");
            return;
        }

        // Reinicia e ativa o sistema de partículas
        particleEffect.Stop(true); // Para qualquer simulação atual e limpa as partículas
        particleEffect.Play(true); // Inicia nova simulação

        if (enableDebugLogs)
            Debug.Log($"Iniciando sistema de partículas: {particleEffect.name}");
    }
}
