using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Gerencia a visualização de vidas do personagem na UI.
/// Usa o HeartPrefab para cada vida (máximo 10, começa com 3).
/// </summary>
public class HealthDisplay : MonoBehaviour
{
    #region Settings
    [Header("Settings")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private int maxHearts = 10;
    [SerializeField] private int startingHearts = 3;

    [Header("Bounce Animation")]
    [SerializeField] private bool enableBounceAnimation = true;
    [SerializeField] private float bounceDuration = 0.5f;

    [SerializeField] private float delayBeforeFirstHeart = 0f;
    [SerializeField] private float delayBetweenHearts = 0.1f;
    #endregion

    #region Sprites
    [Header("Heart Sprites")]
    [SerializeField] private Sprite heartFullSprite;      // ui_hearthCounterOK
    [SerializeField] private Sprite heartEmptySprite;     // ui_hearthCounterNOK
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    #endregion

    #region Private Variables
    private List<Image> heartImages = new List<Image>();
    private int currentHearts;
    private int currentMaxHearts;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        InitializeDisplay();
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Inicializa o display de vidas
    /// </summary>
    private void InitializeDisplay()
    {
        currentHearts = startingHearts;
        currentMaxHearts = startingHearts;

        CreateHearts(startingHearts);
        UpdateDisplay();

        Log($"Health display initialized with {startingHearts} hearts");
    }

    /// <summary>
    /// Cria os corações na UI
    /// </summary>
    private void CreateHearts(int count)
    {
        ClearHearts();

        for (int i = 0; i < count; i++)
        {
            GameObject heart = Instantiate(heartPrefab, transform);
            Image heartImage = heart.GetComponent<Image>();

            if (heartImage != null)
            {
                heartImages.Add(heartImage);

                // Inicia invisível para a animação
                if (enableBounceAnimation)
                {
                    heart.transform.localScale = Vector3.zero;
                }
            }
        }

        Log($"Created {count} heart instances");

        // Inicia animação sequencial
        if (enableBounceAnimation)
        {
            StartCoroutine(AnimateHeartsSequentially());
        }
    }

    /// <summary>
    /// Remove todos os corações existentes
    /// </summary>
    private void ClearHearts()
    {
        foreach (var heart in heartImages)
        {
            if (heart != null)
            {
                Destroy(heart.gameObject);
            }
        }
        heartImages.Clear();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Define o número de vidas atual
    /// </summary>
    public void SetCurrentHearts(int amount)
    {
        currentHearts = Mathf.Clamp(amount, 0, currentMaxHearts);
        UpdateDisplay();
        Log($"Current hearts set to {currentHearts}");
    }

    /// <summary>
    /// Adiciona vidas (cura)
    /// </summary>
    public void AddHearts(int amount)
    {
        SetCurrentHearts(currentHearts + amount);
    }

    /// <summary>
    /// Remove vidas (dano)
    /// </summary>
    public void RemoveHearts(int amount)
    {
        SetCurrentHearts(currentHearts - amount);
    }

    /// <summary>
    /// Aumenta o máximo de vidas (evolução do personagem)
    /// </summary>
    public void IncreaseMaxHearts(int amount)
    {
        int newMax = Mathf.Min(currentMaxHearts + amount, maxHearts);

        if (newMax > currentMaxHearts)
        {
            currentMaxHearts = newMax;
            CreateHearts(currentMaxHearts);
            currentHearts = currentMaxHearts; // Cura ao evoluir
            UpdateDisplay();
            Log($"Max hearts increased to {currentMaxHearts}");
        }
    }
    #endregion

    #region Display Update
    /// <summary>
    /// Atualiza a visualização dos corações
    /// </summary>
    private void UpdateDisplay()
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (heartImages[i] != null)
            {
                heartImages[i].sprite = i < currentHearts ? heartFullSprite : heartEmptySprite;
            }
        }
    }
    #endregion

    #region Animation
    /// <summary>
    /// Anima os corações sequencialmente da esquerda para direita
    /// </summary>
    private System.Collections.IEnumerator AnimateHeartsSequentially()
    {
        // Aguarda delay inicial antes do primeiro coração
        if (delayBeforeFirstHeart > 0)
        {
            yield return new WaitForSeconds(delayBeforeFirstHeart);
        }

        for (int i = 0; i < heartImages.Count; i++)
        {
            if (heartImages[i] != null)
            {
                StartCoroutine(BounceHeart(heartImages[i].transform));
                yield return new WaitForSeconds(delayBetweenHearts);
            }
        }
    }

    /// <summary>
    /// Anima um coração individual com efeito de bounce
    /// </summary>
    private System.Collections.IEnumerator BounceHeart(Transform heartTransform)
    {
        float elapsed = 0f;
        Vector3 originalScale = Vector3.one;

        // Animação de entrada com bounce
        while (elapsed < bounceDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / bounceDuration;

            // Curva de bounce (overshoots e volta)
            float bounceProgress = Mathf.Sin(progress * Mathf.PI);
            float scale = Mathf.Lerp(0f, 1f, progress);

            // Adiciona um pequeno overshoot no meio da animação
            if (progress < 0.5f)
            {
                scale += bounceProgress * 0.2f;
            }

            heartTransform.localScale = originalScale * scale;

            yield return null;
        }

        // Garante escala final correta
        heartTransform.localScale = originalScale;
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Log condicional
    /// </summary>
    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[HealthDisplay] {message}");
        }
    }
    #endregion
}
