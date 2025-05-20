using System.Collections;
using UnityEngine;

/// <summary>
/// Implementa um coletável de energia elemental com quantidade específica
/// </summary>
public class EnergyCollectable : CollectableInteractable
{
    /// <summary>
    /// Define os tipos de energia elemental disponíveis
    /// </summary>
    public enum ElementType
    {
        Water,  // Energia de água
        Air,    // Energia de ar
        Earth,  // Energia de terra
        Fire    // Energia de fogo
    }

    [Header("Configurações de Energia")]
    [Tooltip("Tipo de energia elemental")]
    [SerializeField] private ElementType elementType = ElementType.Water;
    
    [Tooltip("Quantidade de energia para coletar")]
    [SerializeField] private int energyAmount = 1;

    [Header("Configurações Visuais")]
    [Tooltip("Cores para cada tipo de elemento")]

    [Header("Configurações de Áudio")]
    [SerializeField] private AudioClip[] collectSound;
    [Tooltip("Volume do som de coleta")]
    [SerializeField] private float volume = 1f;

    private AudioSource audioSource;
    [SerializeField] private Color[] elementColors = new Color[]
    {
        new Color(0, 0.4f, 1f),    // Água: Azul
        new Color(0.8f, 0.8f, 1f),  // Ar: Branco azulado
        new Color(0.4f, 0.2f, 0),   // Terra: Marrom
        new Color(1f, 0.4f, 0)      // Fogo: Vermelho
    };

    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();

        // Configura a cor baseada no tipo de elemento
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = elementColors[(int)elementType];
        }
        
        // Configura o AudioSource se houver som de coleta
        if (collectSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = collectSound != null && collectSound.Length > 0 ? collectSound[0] : null;
            audioSource.clip = collectSound[Random.Range(0, collectSound.Length)];
            audioSource.volume = volume;
            audioSource.playOnAwake = false;
        }
    }

    protected override void CollectItem()
    {
        // Toca o som de coleta se existir
        if (audioSource != null)
        {
            audioSource.Play();
        }
        
        // Configura a trigger de absorção no Animator
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Absorv");
            
            // Aguarda a animação terminar antes de coletar
            StartCoroutine(WaitForAbsorvAnimation());
        }
        else
        {
            FinishCollection();
        }
    }
    
    private IEnumerator WaitForAbsorvAnimation()
    {
        // Aguarda um frame para garantir que a animação começou
        yield return null;
        
        // Aguarda até a animação atual terminar
        var animator = GetComponent<Animator>();
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Destroy"))
        {
            yield return null;
        }
        
        FinishCollection();
    }
    
    private void FinishCollection()
    {
        base.CollectItem();
    }

    // Getters para acesso externo
    public ElementType Element => elementType;
    public int Amount => energyAmount;
}