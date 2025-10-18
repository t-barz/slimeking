using UnityEngine;
using System.Collections;

/// <summary>
/// Controla o sistema de gotas que formam poças d'água através de animações.
/// Dispara triggers "Drop" nos animators dos prefabs de gota e poça em intervalos regulares.
/// </summary>
public class PuddleDrop : MonoBehaviour
{
    #region Serialized Fields
    [Header("Referências de Prefabs")]
    [Tooltip("Prefab que representa a gota individual")]
    [SerializeField] private GameObject gotaPrefab;
    [Tooltip("Prefab que representa a poça formada")]
    [SerializeField] private GameObject pocoAguaPrefab;

    [Header("Configuração de Spawn")]
    [Tooltip("Intervalo em segundos entre a geração de cada gota")]
    [Min(0.1f)]
    [SerializeField] private float intervaloEntreDrops = 2f;

    [Header("Debug")]
    [Tooltip("Ativar logs de debug para este componente")]
    [SerializeField] private bool enableDebugLogs = false;
    #endregion

    #region Private Fields
    private static readonly int DropTrigger = Animator.StringToHash("Drop");

    private Coroutine dropRoutine;
    private Animator gotaAnimator;
    private Animator pocoAnimator;
    private bool isInitialized = false;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Inicializa os componentes e valida as referências dos prefabs
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Inicia o loop de drops quando o objeto é ativado
    /// </summary>
    private void OnEnable()
    {
        if (isInitialized)
        {
            StartDropLoop();
        }
    }

    /// <summary>
    /// Para o loop de drops quando o objeto é desativado
    /// </summary>
    private void OnDisable()
    {
        StopDropLoop();
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Inicializa os componentes e faz cache dos animators
    /// </summary>
    private void InitializeComponents()
    {
        bool allComponentsValid = true;

        // Validação e cache do animator da gota
        if (gotaPrefab != null)
        {
            gotaAnimator = gotaPrefab.GetComponent<Animator>();
            if (gotaAnimator == null)
            {
                LogWarning($"Prefab da gota não possui Animator: {gotaPrefab.name}");
                allComponentsValid = false;
            }
        }
        else
        {
            LogWarning("Prefab da gota não foi atribuído");
            allComponentsValid = false;
        }

        // Validação e cache do animator da poça
        if (pocoAguaPrefab != null)
        {
            pocoAnimator = pocoAguaPrefab.GetComponent<Animator>();
            if (pocoAnimator == null)
            {
                LogWarning($"Prefab da poça não possui Animator: {pocoAguaPrefab.name}");
                allComponentsValid = false;
            }
        }
        else
        {
            LogWarning("Prefab da poça não foi atribuído");
            allComponentsValid = false;
        }

        // Validação do intervalo
        if (intervaloEntreDrops <= 0f)
        {
            LogWarning($"Intervalo entre drops inválido: {intervaloEntreDrops}. Usando valor padrão de 2f");
            intervaloEntreDrops = 2f;
        }

        isInitialized = allComponentsValid;

        if (isInitialized)
        {
            Log("Componentes inicializados com sucesso");
        }
        else
        {
            LogError("Falha na inicialização dos componentes. Verifique as referências no Inspector");
        }
    }
    #endregion

    #region Drop Loop Management
    /// <summary>
    /// Inicia o loop de geração de drops
    /// </summary>
    private void StartDropLoop()
    {
        if (!isInitialized)
        {
            LogWarning("Tentativa de iniciar loop sem inicialização adequada");
            return;
        }

        if (dropRoutine == null && intervaloEntreDrops > 0f)
        {
            dropRoutine = StartCoroutine(DropLoop());
            Log("Loop de drops iniciado");
        }
    }

    /// <summary>
    /// Para o loop de geração de drops
    /// </summary>
    private void StopDropLoop()
    {
        if (dropRoutine != null)
        {
            StopCoroutine(dropRoutine);
            dropRoutine = null;
            Log("Loop de drops interrompido");
        }
    }

    /// <summary>
    /// Corrotina que executa o loop infinito de drops
    /// </summary>
    private IEnumerator DropLoop()
    {
        // Pequena espera inicial para sincronização
        yield return null;

        while (true)
        {
            TriggerDrop();
            yield return new WaitForSeconds(intervaloEntreDrops);
        }
    }

    /// <summary>
    /// Dispara o trigger "Drop" nos animators dos prefabs
    /// </summary>
    private void TriggerDrop()
    {
        if (gotaAnimator != null)
        {
            gotaAnimator.SetTrigger(DropTrigger);
            Log("Trigger Drop disparado na gota");
        }

        if (pocoAnimator != null)
        {
            pocoAnimator.SetTrigger(DropTrigger);
            Log("Trigger Drop disparado na poça");
        }
    }
    #endregion

    #region Debug Logging
    /// <summary>
    /// Log normal com controle de debug
    /// </summary>
    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[PuddleDrop] {message}");
        }
    }

    /// <summary>
    /// Log de warning com controle de debug
    /// </summary>
    private void LogWarning(string message)
    {
        if (enableDebugLogs)
        {
            Debug.LogWarning($"[PuddleDrop] {message}");
        }
    }

    /// <summary>
    /// Log de error (sempre ativo independente do debug)
    /// </summary>
    private void LogError(string message)
    {
        Debug.LogError($"[PuddleDrop] {message}");
    }
    #endregion
}
