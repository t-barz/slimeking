using UnityEngine;

namespace SlimeKing.Gameplay
{
    public class WindEmulator : MonoBehaviour
{
    [Header("🌊 Configurações de Shake")]
    [SerializeField] private string shakeTriggerName = "Shake";

    [Header("⏰ Configurações de Timing")]
    [SerializeField] private Vector2 shakeIntervalRange = new Vector2(2f, 5f);
    [SerializeField] private bool autoStart = true;

    [Header("Debug")]
    [SerializeField] private bool enableLogs = false;

    // Cached components para performance
    private Animator cachedAnimator;
    private int shakeTriggerHash;

    // Timing variables
    private float nextShakeTime;
    private float currentInterval;

    private void Start()
    {
        InitializeComponents();

        if (autoStart)
        {
            SetRandomInterval();
        }
    }

    private void Update()
    {
        // Verifica se é hora de fazer shake
        if (autoStart && Time.time >= nextShakeTime)
        {
            TriggerShake();
            SetRandomInterval();
        }
    }

    #region Initialization

    private void InitializeComponents()
    {
        // Cache do Animator
        cachedAnimator = GetComponent<Animator>();

        if (cachedAnimator == null)
        {return;
        }

        // Cache do hash da trigger para performance
        shakeTriggerHash = Animator.StringToHash(shakeTriggerName);

        if (enableLogs)
        {}
    }

    private void SetRandomInterval()
    {
        // Gera um intervalo aleatório dentro do range configurado
        currentInterval = Random.Range(shakeIntervalRange.x, shakeIntervalRange.y);
        nextShakeTime = Time.time + currentInterval;

        if (enableLogs)
        {}
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Aciona a trigger Shake do animator
    /// </summary>
    public void TriggerShake()
    {
        if (cachedAnimator == null)
        {return;
        }

        // Aciona a trigger usando hash (performance)
        cachedAnimator.SetTrigger(shakeTriggerHash);

        if (enableLogs)
        {}
    }

    /// <summary>
    /// Inicia o sistema automático de shake
    /// </summary>
    public void StartAutoShake()
    {
        autoStart = true;
        SetRandomInterval();
    }

    /// <summary>
    /// Para o sistema automático de shake
    /// </summary>
    public void StopAutoShake()
    {
        autoStart = false;
    }

    /// <summary>
    /// Força um shake imediatamente e redefine o timer
    /// </summary>
    public void ForceShake()
    {
        TriggerShake();
        SetRandomInterval();
    }

    #endregion

    #region Properties

    public bool HasAnimator => cachedAnimator != null;
    public float CurrentInterval => currentInterval;
    public float TimeUntilNextShake => Mathf.Max(0, nextShakeTime - Time.time);
    public bool IsAutoShaking => autoStart;

    #endregion
}
}
