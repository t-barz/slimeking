using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using SlimeKing.Core;

namespace SlimeKing.UI
{
    /// <summary>
    /// Gerencia a visualização de fragmentos de elementos na UI.
    /// Cada tipo de fragmento tem seu próprio prefab e contador.
    /// Conectado ao GameManager para atualizar automaticamente quando cristais são coletados.
    /// </summary>
    public class FragmentDisplay : MonoBehaviour
    {
    #region Fragment Data
    [System.Serializable]
    public class FragmentType
    {
        public string elementName;
        public GameObject prefab;
        [HideInInspector] public GameObject instance;
        [HideInInspector] public TextMeshProUGUI counterText;
        [HideInInspector] public int count;
    }
    #endregion

    #region Settings
    [Header("Fragment Prefabs")]
    [SerializeField] private List<FragmentType> fragmentTypes = new List<FragmentType>();

    [Header("Animation Settings")]
    [SerializeField] private bool enableBounceAnimation = true;
    [SerializeField] private float bounceDuration = 0.5f;
    [SerializeField] private float delayBeforeFirstFragment = 0f;
    [SerializeField] private float delayBetweenFragments = 0.1f;

    [Header("Layout Settings")]
    [SerializeField] private float spacing = 10f;
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        SetupLayoutGroup();
    }

    private void Start()
    {
        InitializeDisplay();
        ConnectToGameManager();
    }

    private void OnEnable()
    {
        ConnectToGameManager();
    }

    private void OnDisable()
    {
        DisconnectFromGameManager();
    }

    private void OnDestroy()
    {
        DisconnectFromGameManager();
    }
    #endregion

    #region GameManager Integration
    /// <summary>
    /// Conecta ao GameManager para escutar eventos de cristais
    /// </summary>
    private void ConnectToGameManager()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.OnCrystalCountChanged += HandleCrystalCountChanged;
            
            // Sincroniza contadores atuais
            SyncWithGameManager();
        }
    }

    /// <summary>
    /// Desconecta do GameManager
    /// </summary>
    private void DisconnectFromGameManager()
    {
        if (GameManager.HasInstance)
        {
            GameManager.Instance.OnCrystalCountChanged -= HandleCrystalCountChanged;
        }
    }

    /// <summary>
    /// Sincroniza os contadores com os valores atuais do GameManager
    /// </summary>
    private void SyncWithGameManager()
    {
        if (!GameManager.HasInstance) return;

        var allCounts = GameManager.Instance.GetAllCrystalCounts();
        
        foreach (var kvp in allCounts)
        {
            string elementName = kvp.Key.ToString();
            int count = kvp.Value;
            
            var fragmentType = fragmentTypes.Find(f => f.elementName.Equals(elementName, System.StringComparison.OrdinalIgnoreCase));
            if (fragmentType != null)
            {
                fragmentType.count = count;
                UpdateDisplay(fragmentType);
            }
        }
        
    }

    /// <summary>
    /// Handler para eventos de mudança de contagem de cristais do GameManager
    /// </summary>
    private void HandleCrystalCountChanged(CrystalType crystalType, int newCount)
    {
        string elementName = crystalType.ToString();
        
        var fragmentType = fragmentTypes.Find(f => f.elementName.Equals(elementName, System.StringComparison.OrdinalIgnoreCase));
        
        if (fragmentType != null)
        {
            fragmentType.count = newCount;
            UpdateDisplay(fragmentType);
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Configura o HorizontalLayoutGroup para posicionar os fragmentos
    /// </summary>
    private void SetupLayoutGroup()
    {
        // Adiciona ou obtém o HorizontalLayoutGroup
        HorizontalLayoutGroup layoutGroup = GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
        }

        // Configura para posicionar da direita para esquerda
        layoutGroup.childAlignment = TextAnchor.UpperRight;
        layoutGroup.reverseArrangement = false;
        layoutGroup.spacing = spacing;
        layoutGroup.childControlWidth = false;
        layoutGroup.childControlHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = false;

    }

    /// <summary>
    /// Inicializa o display de fragmentos
    /// </summary>
    private void InitializeDisplay()
    {
        CreateFragments();
        UpdateAllDisplays();
    }

    /// <summary>
    /// Cria os fragmentos na UI
    /// </summary>
    private void CreateFragments()
    {
        ClearFragments();

        for (int i = 0; i < fragmentTypes.Count; i++)
        {
            var fragmentType = fragmentTypes[i];

            if (fragmentType.prefab != null)
            {
                // Instancia o prefab
                fragmentType.instance = Instantiate(fragmentType.prefab, transform);

                // Busca o componente de texto no prefab
                fragmentType.counterText = fragmentType.instance.GetComponentInChildren<TextMeshProUGUI>();

                // Inicializa o contador
                fragmentType.count = 0;

                // Inicia invisível para a animação
                if (enableBounceAnimation)
                {
                    fragmentType.instance.transform.localScale = Vector3.zero;
                }

            }
        }

        // Inicia animação sequencial
        if (enableBounceAnimation)
        {
            StartCoroutine(AnimateFragmentsSequentially());
        }
    }

    /// <summary>
    /// Remove todos os fragmentos existentes
    /// </summary>
    private void ClearFragments()
    {
        foreach (var fragmentType in fragmentTypes)
        {
            if (fragmentType.instance != null)
            {
                Destroy(fragmentType.instance);
                fragmentType.instance = null;
                fragmentType.counterText = null;
            }
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Adiciona fragmentos de um tipo específico
    /// </summary>
    public void AddFragments(string elementName, int amount)
    {
        var fragmentType = fragmentTypes.Find(f => f.elementName.Equals(elementName, System.StringComparison.OrdinalIgnoreCase));

        if (fragmentType != null)
        {
            fragmentType.count += amount;
            UpdateDisplay(fragmentType);
        }
    }

    /// <summary>
    /// Remove fragmentos de um tipo específico
    /// </summary>
    public void RemoveFragments(string elementName, int amount)
    {
        var fragmentType = fragmentTypes.Find(f => f.elementName.Equals(elementName, System.StringComparison.OrdinalIgnoreCase));

        if (fragmentType != null)
        {
            fragmentType.count = Mathf.Max(0, fragmentType.count - amount);
            UpdateDisplay(fragmentType);
        }
    }

    /// <summary>
    /// Define a quantidade de fragmentos de um tipo específico
    /// </summary>
    public void SetFragments(string elementName, int amount)
    {
        var fragmentType = fragmentTypes.Find(f => f.elementName.Equals(elementName, System.StringComparison.OrdinalIgnoreCase));

        if (fragmentType != null)
        {
            fragmentType.count = Mathf.Max(0, amount);
            UpdateDisplay(fragmentType);
        }
    }

    /// <summary>
    /// Obtém a quantidade de fragmentos de um tipo específico
    /// </summary>
    public int GetFragments(string elementName)
    {
        var fragmentType = fragmentTypes.Find(f => f.elementName.Equals(elementName, System.StringComparison.OrdinalIgnoreCase));
        return fragmentType?.count ?? 0;
    }

    /// <summary>
    /// Reseta todos os fragmentos para zero
    /// </summary>
    public void ResetAllFragments()
    {
        foreach (var fragmentType in fragmentTypes)
        {
            fragmentType.count = 0;
            UpdateDisplay(fragmentType);
        }
    }
    #endregion

    #region Display Update
    /// <summary>
    /// Atualiza a visualização de um fragmento específico
    /// </summary>
    private void UpdateDisplay(FragmentType fragmentType)
    {
        if (fragmentType.counterText != null)
        {
            fragmentType.counterText.text = fragmentType.count.ToString();
        }
    }

    /// <summary>
    /// Atualiza todos os displays
    /// </summary>
    private void UpdateAllDisplays()
    {
        foreach (var fragmentType in fragmentTypes)
        {
            UpdateDisplay(fragmentType);
        }
    }
    #endregion

    #region Animation
    /// <summary>
    /// Anima os fragmentos sequencialmente
    /// </summary>
    private System.Collections.IEnumerator AnimateFragmentsSequentially()
    {
        // Aguarda delay inicial antes do primeiro fragmento
        if (delayBeforeFirstFragment > 0)
        {
            yield return new WaitForSeconds(delayBeforeFirstFragment);
        }

        foreach (var fragmentType in fragmentTypes)
        {
            if (fragmentType.instance != null)
            {
                StartCoroutine(BounceFragment(fragmentType.instance.transform));
                yield return new WaitForSeconds(delayBetweenFragments);
            }
        }
    }

    /// <summary>
    /// Anima um fragmento individual com efeito de bounce
    /// </summary>
    private System.Collections.IEnumerator BounceFragment(Transform fragmentTransform)
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

            fragmentTransform.localScale = originalScale * scale;

            yield return null;
        }

        // Garante escala final correta
        fragmentTransform.localScale = originalScale;
    }
    #endregion

    #region Editor Helper
#if UNITY_EDITOR
    [ContextMenu("Setup Default Fragments")]
    private void SetupDefaultFragments()
    {
        fragmentTypes.Clear();

        string[] elementNames = { "Earth", "Nature", "Water", "Air", "Dark", "Fire" };

        foreach (var name in elementNames)
        {
            fragmentTypes.Add(new FragmentType
            {
                elementName = name,
                prefab = null // Será configurado manualmente no Inspector
            });
        }

    }
#endif
    #endregion
    }
}
