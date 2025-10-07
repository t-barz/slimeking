using UnityEngine;
using System;

/// <summary>
/// Sistema de atributos do personagem jogador.
/// Responsável por controlar pontos de vida, ataque, defesa, velocidade e sistema de habilidades.
/// Esta classe deve ser anexada ao GameObject do jogador para funcionamento correto.
/// Segue o padrão System das Boas Práticas para funcionalidades complexas e modulares.
/// </summary>
public class PlayerAttributesSystem : MonoBehaviour
{
    #region Inspector Configuration

    [Header("Configurações de Atributos Base")]
    [SerializeField] private int baseHealthPoints = 10;
    [SerializeField] private int baseAttack = 1;
    [SerializeField] private int baseDefense = 0;
    [SerializeField] private float baseMoveSpeed = 5f;

    [Header("Configurações de Combate")]
    [SerializeField] private float baseAttackRange = 1f;
    [SerializeField] private float baseAttackDuration = 0.5f;

    [Header("Configurações de Movimento")]
    [SerializeField] private float baseAcceleration = 10f;
    [SerializeField] private float baseDeceleration = 10f;

    [Header("Configurações de Debug")]
    [SerializeField] private bool enableLogs = false;
    [SerializeField] private bool enableDebugGizmos = true;

    #endregion

    #region Private Variables

    private int _currentHealthPoints;
    private int _currentAttack;
    private int _currentDefense;
    private float _currentMoveSpeed;
    private float _currentAttackRange;
    private float _currentAttackDuration;
    private float _currentAcceleration;
    private float _currentDeceleration;
    private int _totalSkillPoints;
    private int _currentSkillPoints;

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
        get => baseAttack;
        set
        {
            int oldValue = _currentAttack;
            _currentAttack = Mathf.Max(0, value);

            if (enableLogs && oldValue != _currentAttack)
            {
                Debug.Log($"[PlayerAttributes] Attack alterado: {oldValue} → {_currentAttack}");
            }
        }
    }

    /// <summary>
    /// Valor de defesa atual do jogador.
    /// </summary>
    public int CurrentDefense
    {
        get => baseDefense;
        set
        {
            int oldValue = _currentDefense;
            _currentDefense = Mathf.Max(0, value);

            if (enableLogs && oldValue != _currentDefense)
            {
                Debug.Log($"[PlayerAttributes] Defense alterado: {oldValue} → {_currentDefense}");
            }
        }
    }

    /// <summary>
    /// Velocidade de movimento atual do jogador.
    /// </summary>
    public float CurrentMoveSpeed
    {
        get => _currentMoveSpeed;
        set
        {
            float oldValue = _currentMoveSpeed;
            _currentMoveSpeed = Mathf.Max(0, value);

            if (enableLogs && !Mathf.Approximately(oldValue, _currentMoveSpeed))
            {
                Debug.Log($"[PlayerAttributes] Move Speed alterado: {oldValue:F2} → {_currentMoveSpeed:F2}");
            }
        }
    }

    /// <summary>
    /// Alcance de ataque atual do jogador.
    /// </summary>
    public float CurrentAttackRange
    {
        get => _currentAttackRange;
        set
        {
            float oldValue = _currentAttackRange;
            _currentAttackRange = Mathf.Max(0, value);

            if (enableLogs && !Mathf.Approximately(oldValue, _currentAttackRange))
            {
                Debug.Log($"[PlayerAttributes] Attack Range alterado: {oldValue:F2} → {_currentAttackRange:F2}");
            }
        }
    }

    /// <summary>
    /// Duração do ataque atual do jogador.
    /// </summary>
    public float CurrentAttackDuration
    {
        get => _currentAttackDuration;
        set
        {
            float oldValue = _currentAttackDuration;
            _currentAttackDuration = Mathf.Max(0.1f, value);

            if (enableLogs && !Mathf.Approximately(oldValue, _currentAttackDuration))
            {
                Debug.Log($"[PlayerAttributes] Attack Duration alterado: {oldValue:F2} → {_currentAttackDuration:F2}");
            }
        }
    }

    /// <summary>
    /// Aceleração atual do jogador.
    /// </summary>
    public float CurrentAcceleration
    {
        get => _currentAcceleration;
        set
        {
            float oldValue = _currentAcceleration;
            _currentAcceleration = Mathf.Max(0, value);

            if (enableLogs && !Mathf.Approximately(oldValue, _currentAcceleration))
            {
                Debug.Log($"[PlayerAttributes] Acceleration alterado: {oldValue:F2} → {_currentAcceleration:F2}");
            }
        }
    }

    /// <summary>
    /// Desaceleração atual do jogador.
    /// </summary>
    public float CurrentDeceleration
    {
        get => _currentDeceleration;
        set
        {
            float oldValue = _currentDeceleration;
            _currentDeceleration = Mathf.Max(0, value);

            if (enableLogs && !Mathf.Approximately(oldValue, _currentDeceleration))
            {
                Debug.Log($"[PlayerAttributes] Deceleration alterado: {oldValue:F2} → {_currentDeceleration:F2}");
            }
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
                                           $"SPD: {CurrentMoveSpeed:F1} | SP: {CurrentSkillPoints}/{TotalSkillPoints}");
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
               $"SPD: {CurrentMoveSpeed:F1} | SP: {CurrentSkillPoints}/{TotalSkillPoints}";
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
        _currentMoveSpeed = baseMoveSpeed;
        _currentAttackRange = baseAttackRange;
        _currentAttackDuration = baseAttackDuration;
        _currentAcceleration = baseAcceleration;
        _currentDeceleration = baseDeceleration;
        _totalSkillPoints = 0;
        _currentSkillPoints = 0;

        if (enableLogs)
        {
            Debug.Log("[PlayerAttributes] Atributos inicializados com valores base.");
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
