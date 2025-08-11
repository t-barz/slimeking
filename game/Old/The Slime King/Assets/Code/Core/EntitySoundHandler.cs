using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Responsável por reproduzir sons relacionados a entidades que possuem EntityStatus.
/// Gerencia sons de ataque, dano, movimento, morte e outros efeitos.
/// </summary>
[RequireComponent(typeof(EntityStatus))]
public class EntitySoundHandler : MonoBehaviour
{
    /// <summary>
    /// Referência ao componente EntityStatus para monitorar mudanças de estado.
    /// </summary>
    private EntityStatus entityStatus;

    /// <summary>
    /// Fonte de áudio para reproduzir os sons da entidade.
    /// </summary>
    private AudioSource audioSource;

    [Header("Sons Básicos")]
    [Tooltip("Som reproduzido quando a entidade recebe dano")]
    [SerializeField] private AudioClip damageSound;

    [Tooltip("Som reproduzido quando a entidade morre")]
    [SerializeField] private AudioClip deathSound;

    [Tooltip("Sons de passos (reproduzidos ao caminhar)")]
    [SerializeField] private AudioClip[] footstepSounds;

    [Header("Sons de Combate")]
    [Tooltip("Sons reproduzidos ao realizar ataque básico")]
    [SerializeField] private AudioClip[] attackSounds;

    [Tooltip("Sons reproduzidos ao realizar ataque especial")]
    [SerializeField] private AudioClip[] specialAttackSounds;

    [Header("Sons de Estado")]
    [Tooltip("Som reproduzido ao se esconder/agachar")]
    [SerializeField] private AudioClip hideSound;

    [Tooltip("Som reproduzido ao aplicar ou receber buffs")]
    [SerializeField] private AudioClip buffSound;

    [Tooltip("Som reproduzido ao aplicar ou receber debuffs")]
    [SerializeField] private AudioClip debuffSound;

    [Header("Configurações")]
    [Range(0f, 1f)]
    [Tooltip("Variação aleatória de pitch para sons (0 = sem variação)")]
    [SerializeField] private float randomPitchVariation = 0.1f;

    // Controle interno
    private int currentHP;
    private Dictionary<StatusAttribute, List<StatusModifier>> previousModifiers = new Dictionary<StatusAttribute, List<StatusModifier>>();

    /// <summary>
    /// Inicializa o componente e registra os eventos necessários.
    /// </summary>
    private void Awake()
    {
        entityStatus = GetComponent<EntityStatus>();

        // Cria uma fonte de áudio se não existir
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Configuração básica da fonte de áudio
        audioSource.spatialBlend = 1.0f; // Som 3D
        audioSource.volume = 0.8f;
        audioSource.pitch = 1.0f;

        // Armazena o HP atual para detecção de dano
        currentHP = entityStatus.currentHP;

        // Inicializa o dicionário de modificadores anteriores
        foreach (StatusAttribute attr in System.Enum.GetValues(typeof(StatusAttribute)))
        {
            previousModifiers[attr] = new List<StatusModifier>();
        }
    }

    /// <summary>
    /// Registra o evento de morte quando o script é habilitado.
    /// </summary>
    private void OnEnable()
    {
        if (entityStatus != null)
            entityStatus.OnDeath += PlayDeathSound;
    }

    /// <summary>
    /// Remove o evento de morte quando o script é desabilitado.
    /// </summary>
    private void OnDisable()
    {
        if (entityStatus != null)
            entityStatus.OnDeath -= PlayDeathSound;
    }

    /// <summary>
    /// Verifica mudanças no estado da entidade e reproduz sons apropriados.
    /// </summary>
    private void Update()
    {
        // Verifica se tomou dano
        if (entityStatus.currentHP < currentHP)
        {
            PlayDamageSound();
            currentHP = entityStatus.currentHP;
        }

        // Detecta modificadores novos
        DetectModifierChanges();
    }

    /// <summary>
    /// Reproduz som de dano quando a entidade é ferida.
    /// </summary>
    public void PlayDamageSound()
    {
        if (damageSound != null && entityStatus.IsAlive())
            PlaySoundWithVariation(damageSound);
    }

    /// <summary>
    /// Reproduz som de morte quando a entidade morre.
    /// </summary>
    public void PlayDeathSound()
    {
        if (deathSound != null)
            PlaySoundWithVariation(deathSound);
    }

    /// <summary>
    /// Reproduz som de passos se o intervalo mínimo foi respeitado.
    /// </summary>
    public void PlayFootstepSound()
    {
        if (footstepSounds.Length == 0)
            return;

        int randomIndex = Random.Range(0, footstepSounds.Length);
        if (footstepSounds[randomIndex] != null)
        {
            PlaySoundWithVariation(footstepSounds[randomIndex]);
        }
    }

    /// <summary>
    /// Reproduz um som de ataque básico aleatório.
    /// </summary>
    public void PlayAttackSound()
    {
        if (attackSounds.Length == 0)
            return;

        int randomIndex = Random.Range(0, attackSounds.Length);
        if (attackSounds[randomIndex] != null)
            PlaySoundWithVariation(attackSounds[randomIndex]);
    }

    /// <summary>
    /// Reproduz um som de ataque especial aleatório.
    /// </summary>
    public void PlaySpecialAttackSound()
    {
        if (specialAttackSounds.Length == 0)
            return;

        int randomIndex = Random.Range(0, specialAttackSounds.Length);
        if (specialAttackSounds[randomIndex] != null)
            PlaySoundWithVariation(specialAttackSounds[randomIndex]);
    }

    /// <summary>
    /// Reproduz som de esconder/agachar.
    /// </summary>
    public void PlayHideSound()
    {
        if (hideSound != null)
            PlaySoundWithVariation(hideSound);
    }

    /// <summary>
    /// Verifica se houve mudanças nos modificadores e reproduz sons apropriados.
    /// </summary>
    private void DetectModifierChanges()
    {
        foreach (StatusModifier mod in entityStatus.modifiers)
        {
            // Verifica se é um modificador novo
            if (!IsModifierInList(mod, previousModifiers[mod.attribute]))
            {
                // É um buff ou debuff?
                if (mod.value > 0)
                {
                    if (buffSound != null)
                        PlaySoundWithVariation(buffSound);
                }
                else
                {
                    if (debuffSound != null)
                        PlaySoundWithVariation(debuffSound);
                }

                // Adiciona à lista de modificadores monitorados
                previousModifiers[mod.attribute].Add(mod);
            }
        }

        // Remove modificadores que não existem mais
        foreach (var attrList in previousModifiers)
        {
            attrList.Value.RemoveAll(mod => !entityStatus.modifiers.Contains(mod));
        }
    }

    /// <summary>
    /// Verifica se um modificador específico já está na lista.
    /// </summary>
    private bool IsModifierInList(StatusModifier modifier, List<StatusModifier> list)
    {
        foreach (var mod in list)
        {
            if (mod == modifier)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Reproduz um som com variação aleatória de pitch para naturalidade.
    /// </summary>
    private void PlaySoundWithVariation(AudioClip clip)
    {
        // Adiciona variação ao pitch
        float randomPitch = 1.0f + Random.Range(-randomPitchVariation, randomPitchVariation);
        audioSource.pitch = randomPitch;

        // Reproduz o som
        audioSource.PlayOneShot(clip);

        // Restaura o pitch
        audioSource.pitch = 1.0f;
    }
}