using UnityEngine;
using System;

/// <summary>
/// Gerencia todos os atributos do personagem jogador.
/// Responsável por controlar pontos de vida, ataque, defesa, velocidade e sistema de habilidades.
/// Esta classe deve ser anexada ao GameObject do jogador para funcionamento correto.
/// </summary>
public class PlayerAttributesHandler : MonoBehaviour
{
    #region Inspector Configuration

    [Header("Configurações de Atributos Base")]
    [SerializeField] private int baseHealthPoints = 3;
    [SerializeField] private int baseAttack = 1;
    [SerializeField] private int baseDefense = 0;
    [SerializeField] private int baseSpeed = 2;

    [Header("Configurações de Debug")]
    [SerializeField] private bool enableLogs = true;
    [SerializeField] private bool enableDebugGizmos = true;

    #endregion

    #region Private Variables

    private int _currentHealthPoints;
    private int _currentAttack;
    private int _currentDefense;
    private int _currentSpeed;
    private int _totalSkillPoints;
    private int _currentSkillPoints;
    private bool _isInitialized = false;

    #endregion

    #region Public Properties

    /// <summary>
    /// Pontos de vida atuais do jogador.
    /// </summary>
    public int CurrentHealthPoints
    {
        get => _currentHealthPoints;
        set
        {
            int oldValue = _currentHealthPoints;
            _currentHealthPoints = Mathf.Clamp(value, 0, MaxHealthPoints);

            if (enableLogs && oldValue != _currentHealthPoints)
            {
                Debug.Log($"[PlayerAttributes] Health Points alterado: {oldValue} → {_currentHealthPoints}");
            }

            OnHealthChanged?.Invoke(_currentHealthPoints, MaxHealthPoints);

            if (_currentHealthPoints <= 0)
            {
                OnPlayerDied?.Invoke();
            }
        }
    }

    /// <summary>
    /// Pontos de vida máximos do jogador.
    /// </summary>
    public int MaxHealthPoints
    {
        get => baseHealthPoints;
    }

    /// <summary>
    /// Valor de ataque atual do jogador.
    /// </summary>
    public int CurrentAttack
    {
        get
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("[PlayerAttributes] CurrentAttack acessado antes da inicialização! Forçando InitializeAttributes()");
                InitializeAttributes();
            }

            return _currentAttack;
        }
        set
        {
            int oldValue = _currentAttack;
            _currentAttack = Mathf.Max(0, value);
        }
    }

    /// <summary>
    /// Valor de defesa atual do jogador.
    /// </summary>
    public int CurrentDefense
    {
        get => _currentDefense;
        set
        {
            int oldValue = _currentDefense;
            _currentDefense = Mathf.Max(0, value);
        }
    }

    /// <summary>
    /// Velocidade atual do jogador.
    /// </summary>
    public int CurrentSpeed
    {
        get => _currentSpeed;
        set
        {
            int oldValue = _currentSpeed;
            _currentSpeed = Mathf.Max(0, value);

        }
    }

    /// <summary>
    /// Total de pontos de habilidades obtidos desde o início do jogo.
    /// </summary>
    public int TotalSkillPoints
    {
        get => _totalSkillPoints;
        private set
        {
            int oldValue = _totalSkillPoints;
            _totalSkillPoints = Mathf.Max(0, value);

            if (enableLogs && oldValue != _totalSkillPoints)
            {
                Debug.Log($"[PlayerAttributes] Total Skill Points alterado: {oldValue} → {_totalSkillPoints}");
            }

            OnSkillPointsChanged?.Invoke(_currentSkillPoints, _totalSkillPoints);
        }
    }

    /// <summary>
    /// Pontos de habilidades disponíveis para uso.
    /// </summary>
    public int CurrentSkillPoints
    {
        get => _currentSkillPoints;
        private set
        {
            int oldValue = _currentSkillPoints;
            _currentSkillPoints = Mathf.Clamp(value, 0, _totalSkillPoints);

            if (enableLogs && oldValue != _currentSkillPoints)
            {
                Debug.Log($"[PlayerAttributes] Current Skill Points alterado: {oldValue} → {_currentSkillPoints}");
            }

            OnSkillPointsChanged?.Invoke(_currentSkillPoints, _totalSkillPoints);
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Evento disparado quando os pontos de vida mudam.
    /// Parâmetros: (currentHealth, maxHealth)
    /// </summary>
    public event Action<int, int> OnHealthChanged;

    /// <summary>
    /// Evento disparado quando o jogador morre.
    /// </summary>
    public event Action OnPlayerDied;

    /// <summary>
    /// Evento disparado quando os pontos de habilidade mudam.
    /// Parâmetros: (currentSkillPoints, totalSkillPoints)
    /// </summary>
    public event Action<int, int> OnSkillPointsChanged;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Inicializa os atributos do jogador
        InitializeAttributes();
    }

    private void Start()
    {
        if (enableLogs)
        {
            Debug.Log("[PlayerAttributes] Sistema de atributos do jogador inicializado.");
            LogCurrentAttributes();
        }
    }

    private void OnDrawGizmos()
    {
        if (!enableDebugGizmos) return;

        // Desenha informações dos atributos no Scene View
        Gizmos.color = Color.green;
        Vector3 position = transform.position + Vector3.up * 1f;

#if UNITY_EDITOR
        UnityEditor.Handles.Label(position, $"HP: {CurrentHealthPoints}/{MaxHealthPoints}\n" +
                                           $"ATK: {CurrentAttack} | DEF: {CurrentDefense}\n" +
                                           $"SPD: {CurrentSpeed} | SP: {CurrentSkillPoints}/{TotalSkillPoints}");
#endif
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Aplica dano ao jogador.
    /// </summary>
    /// <param name="damage">Quantidade de dano a ser aplicada</param>
    /// <param name="ignoreDefense">Se true, ignora a defesa do jogador</param>
    /// <returns>Dano real aplicado após cálculos de defesa</returns>
    public int TakeDamage(int damage, bool ignoreDefense = false)
    {
        if (damage <= 0) return 0;

        int finalDamage = damage;

        if (!ignoreDefense)
        {
            // Calcula redução de dano baseada na defesa
            int damageReduction = Mathf.RoundToInt((CurrentDefense * 100) / (CurrentDefense + 100));
            finalDamage = damage - Mathf.RoundToInt((damage * damageReduction) / 100);
        }

        CurrentHealthPoints -= finalDamage;

        // Aciona trigger Hit no Animator, se existir
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            int hitHash = Animator.StringToHash("Hit");
            animator.SetTrigger(hitHash);
        }

        if (enableLogs)
        {
            Debug.Log($"[PlayerAttributes] Dano recebido: {damage} | Dano final: {finalDamage} | HP restante: {CurrentHealthPoints}");
        }

        return finalDamage;
    }

    /// <summary>
    /// Cura o jogador.
    /// </summary>
    /// <param name="healAmount">Quantidade de cura</param>
    /// <returns>Quantidade real curada</returns>
    public int Heal(int healAmount)
    {
        if (healAmount <= 0) return 0;

        int oldHealth = CurrentHealthPoints;
        CurrentHealthPoints += healAmount;
        int actualHeal = CurrentHealthPoints - oldHealth;

        if (enableLogs)
        {
            Debug.Log($"[PlayerAttributes] Cura aplicada: {healAmount} | Cura real: {actualHeal} | HP atual: {CurrentHealthPoints}");
        }

        return actualHeal;
    }

    /// <summary>
    /// Adiciona pontos de habilidade ao jogador.
    /// </summary>
    /// <param name="points">Quantidade de pontos a adicionar</param>
    public void AddSkillPoints(int points)
    {
        if (points <= 0) return;

        TotalSkillPoints += points;
        CurrentSkillPoints += points;

        if (enableLogs)
        {
            Debug.Log($"[PlayerAttributes] {points} pontos de habilidade adicionados. Total: {TotalSkillPoints}, Disponível: {CurrentSkillPoints}");
        }
    }

    /// <summary>
    /// Gasta pontos de habilidade.
    /// </summary>
    /// <param name="points">Quantidade de pontos a gastar</param>
    /// <returns>True se teve pontos suficientes, false caso contrário</returns>
    public bool SpendSkillPoints(int points)
    {
        if (points <= 0 || CurrentSkillPoints < points)
        {
            if (enableLogs)
            {
                Debug.LogWarning($"[PlayerAttributes] Tentativa de gastar {points} pontos falhou. Disponível: {CurrentSkillPoints}");
            }
            return false;
        }

        CurrentSkillPoints -= points;

        if (enableLogs)
        {
            Debug.Log($"[PlayerAttributes] {points} pontos de habilidade gastos. Restante: {CurrentSkillPoints}");
        }

        return true;
    }

    /// <summary>
    /// Restaura completamente a vida do jogador.
    /// </summary>
    public void FullHeal()
    {
        CurrentHealthPoints = MaxHealthPoints;

        if (enableLogs)
        {
            Debug.Log($"[PlayerAttributes] Vida totalmente restaurada: {CurrentHealthPoints}");
        }
    }

    /// <summary>
    /// Obtém um resumo de todos os atributos atuais.
    /// </summary>
    /// <returns>String formatada com todos os atributos</returns>
    public string GetAttributesSummary()
    {
        return $"HP: {CurrentHealthPoints}/{MaxHealthPoints} | " +
               $"ATK: {CurrentAttack} | DEF: {CurrentDefense} | " +
               $"SPD: {CurrentSpeed} | SP: {CurrentSkillPoints}/{TotalSkillPoints}";
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Inicializa os atributos com os valores base.
    /// </summary>
    private void InitializeAttributes()
    {
        _currentHealthPoints = baseHealthPoints;
        _currentAttack = baseAttack;
        _currentDefense = baseDefense;
        _currentSpeed = baseSpeed;
        _totalSkillPoints = 0;
        _currentSkillPoints = 0;
        _isInitialized = true;

        if (enableLogs)
        {
            Debug.Log($"[PlayerAttributes] Atributos inicializados - Attack: {_currentAttack} (base: {baseAttack}), Defense: {_currentDefense}, Speed: {_currentSpeed}");
        }
    }

    /// <summary>
    /// Exibe no log todos os atributos atuais (apenas para debug).
    /// </summary>
    private void LogCurrentAttributes()
    {
        if (!enableLogs) return;

        Debug.Log($"[PlayerAttributes] Estado atual dos atributos:\n{GetAttributesSummary()}");
    }

    #endregion
}
