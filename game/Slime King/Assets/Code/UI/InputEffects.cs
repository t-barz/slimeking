using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Controla efeitos de fade para objetos usando SortingGroup.
/// </summary>
public class InputEffects : MonoBehaviour
{
    [Header("Configurações de Fade")]
    [Tooltip("Tempo em segundos antes do objeto começar a aparecer")]
    [SerializeField] private float fadeDelay = 2f;
    
    [Tooltip("Duração em segundos do efeito de fade")]
    [SerializeField] private float fadeDuration = 1f;

    private SortingGroup sortingGroup;
    private SpriteRenderer[] spriteRenderers;
    private float currentTime;
    private bool startFade;

    private void Start()
    {
        // Obtém o componente SortingGroup
        sortingGroup = GetComponent<SortingGroup>();
        if (sortingGroup == null)
        {
            Debug.LogError("SortingGroup component not found!");
            enabled = false;
            return;
        }

        // Obtém todos os SpriteRenderers dentro do SortingGroup
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0)
        {
            Debug.LogError("No SpriteRenderers found in children!");
            enabled = false;
            return;
        }

        // Configura estado inicial
        SetAlpha(0f);
        startFade = false;
        currentTime = 0f;

        // Inicia o fade após o delay
        Invoke(nameof(StartFade), fadeDelay);
    }

    private void Update()
    {
        if (startFade && currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeDuration);
            SetAlpha(alpha);
        }
    }

    private void StartFade()
    {
        startFade = true;
    }

    private void SetAlpha(float alpha)
    {
        foreach (var renderer in spriteRenderers)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }
}