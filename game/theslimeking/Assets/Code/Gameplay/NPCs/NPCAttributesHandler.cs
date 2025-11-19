using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Tipos de NPCs disponíveis no jogo.
/// </summary>
public enum NPCType
{
    Abelha,
    Helpy,
    SlimeAmarelo,
    SlimeAzul,
    SlimeVerde,
    SlimeVermelho
}

/// <summary>
/// Gerencia todos os atributos de NPCs.
/// Responsável por controlar pontos de vida, ataque, defesa, velocidade e sistema de drops.
/// Esta classe deve ser anexada ao GameObject do NPC para funcionamento correto.
/// </summary>
public class NPCAttributesHandler : MonoBehaviour
{
    #region Inspector Configuration

    [Header("Tipo do NPC")]
    [SerializeField] private NPCType npcType = NPCType.Abelha;

    [Header("Configurações de Atributos Base")]
    [SerializeField] private int baseHealthPoints = 3;
    [SerializeField] private int baseAttack = 1;
    [SerializeField] private int baseDefense = 0;
    [SerializeField] private int baseSpeed = 2;

    [Header("Sistema de Drops")]
    [SerializeField] private List<NPCDropItem> dropItems = new List<NPCDropItem>();

    [Header("Configurações de Debug")]
    [SerializeField] private bool enableLogs = false;
    [SerializeField] private bool enableDebugGizmos = true;

    #endregion

    #region Private Variables

    private int _currentHealthPoints;
    private int _currentAttack;
    private int _currentDefense;
    private int _currentSpeed;

    #endregion

    #region Public Properties

    /// <summary>
    /// Tipo do NPC.
    /// </summary>
    public NPCType NPCType => npcType;

    /// <summary>
    /// Pontos de vida atuais do NPC.
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
                Debug.Log($"[NPCAttributes] {gameObject.name} Health Points alterado: {oldValue} → {_currentHealthPoints}");
            }

            OnHealthChanged?.Invoke(_currentHealthPoints, MaxHealthPoints);

            if (_currentHealthPoints <= 0)
            {
                OnNPCDied?.Invoke();
            }
        }
    }

    /// <summary>
    /// Pontos de vida máximos do NPC.
    /// </summary>
    public int MaxHealthPoints => baseHealthPoints;

    /// <summary>
    /// Valor de ataque atual do NPC.
    /// </summary>
    public int CurrentAttack
    {
        get => _currentAttack;
        set
        {
            int oldValue = _currentAttack;
            _currentAttack = Mathf.Max(0, value);

            if (enableLogs && oldValue != _currentAttack)
            {
                Debug.Log($"[NPCAttributes] {gameObject.name} Attack alterado: {oldValue} → {_currentAttack}");
            }
        }
    }

    /// <summary>
    /// Valor de defesa atual do NPC.
    /// </summary>
    public int CurrentDefense
    {
        get => _currentDefense;
        set
        {
            int oldValue = _currentDefense;
            _currentDefense = Mathf.Max(0, value);

            if (enableLogs && oldValue != _currentDefense)
            {
                Debug.Log($"[NPCAttributes] {gameObject.name} Defense alterado: {oldValue} → {_currentDefense}");
            }
        }
    }

    /// <summary>
    /// Velocidade atual do NPC.
    /// </summary>
    public int CurrentSpeed
    {
        get => _currentSpeed;
        set
        {
            int oldValue = _currentSpeed;
            _currentSpeed = Mathf.Max(0, value);

            if (enableLogs && oldValue != _currentSpeed)
            {
                Debug.Log($"[NPCAttributes] {gameObject.name} Speed alterado: {oldValue} → {_currentSpeed}");
            }
        }
    }

    /// <summary>
    /// Lista de itens que o NPC pode dropar.
    /// </summary>
    public List<NPCDropItem> DropItems => dropItems;

    #endregion

    #region Events

    /// <summary>
    /// Evento disparado quando os pontos de vida mudam.
    /// Parâmetros: (currentHealth, maxHealth)
    /// </summary>
    public event Action<int, int> OnHealthChanged;

    /// <summary>
    /// Evento disparado quando o NPC morre.
    /// </summary>
    public event Action OnNPCDied;

    /// <summary>
    /// Evento disparado quando itens são dropados.
    /// Parâmetros: (dropedItems)
    /// </summary>
    public event Action<List<GameObject>> OnItemsDropped;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        InitializeAttributes();
    }

    private void Start()
    {
        if (enableLogs)
        {
            Debug.Log($"[NPCAttributes] Sistema de atributos do NPC {gameObject.name} inicializado.");
            LogCurrentAttributes();
        }
    }

    private void OnDrawGizmos()
    {
        if (!enableDebugGizmos) return;

        Gizmos.color = Color.red;
        Vector3 position = transform.position + Vector3.up * 1f;

#if UNITY_EDITOR
        UnityEditor.Handles.Label(position, $"HP: {CurrentHealthPoints}/{MaxHealthPoints}\n" +
                                           $"ATK: {CurrentAttack} | DEF: {CurrentDefense}\n" +
                                           $"SPD: {CurrentSpeed}");
#endif
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Aplica dano ao NPC.
    /// </summary>
    /// <param name="damage">Quantidade de dano a ser aplicada</param>
    /// <param name="ignoreDefense">Se true, ignora a defesa do NPC</param>
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
            Debug.Log($"[NPCAttributes] {gameObject.name} recebeu dano: {damage} | Dano final: {finalDamage} | HP restante: {CurrentHealthPoints}");
        }

        return finalDamage;
    }

    /// <summary>
    /// Cura o NPC.
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
            Debug.Log($"[NPCAttributes] {gameObject.name} foi curado: {healAmount} | Cura real: {actualHeal} | HP atual: {CurrentHealthPoints}");
        }

        return actualHeal;
    }

    /// <summary>
    /// Executa o sistema de drop de itens quando o NPC morre.
    /// </summary>
    public void ExecuteDrops()
    {
        if (dropItems == null || dropItems.Count == 0) return;

        List<GameObject> droppedItems = new List<GameObject>();

        foreach (var dropItem in dropItems)
        {
            if (dropItem.itemPrefab == null) continue;

            // Verifica chance de drop
            float randomValue = UnityEngine.Random.Range(0f, 100f);
            if (randomValue <= dropItem.dropChance)
            {
                // Instancia o item na posição do NPC
                Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * 0.5f;
                Vector3 dropPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
                GameObject droppedItem = Instantiate(dropItem.itemPrefab, dropPosition, Quaternion.identity);
                droppedItems.Add(droppedItem);

                if (enableLogs)
                {
                    Debug.Log($"[NPCAttributes] {gameObject.name} dropou: {dropItem.itemPrefab.name} (chance: {dropItem.dropChance}%)");
                }
            }
        }

        OnItemsDropped?.Invoke(droppedItems);
    }

    /// <summary>
    /// Obtém um resumo de todos os atributos atuais.
    /// </summary>
    /// <returns>String formatada com todos os atributos</returns>
    public string GetAttributesSummary()
    {
        return $"{gameObject.name} - HP: {CurrentHealthPoints}/{MaxHealthPoints} | " +
               $"ATK: {CurrentAttack} | DEF: {CurrentDefense} | SPD: {CurrentSpeed}";
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

        if (enableLogs)
        {
            Debug.Log($"[NPCAttributes] {gameObject.name} atributos inicializados com valores base.");
        }
    }

    /// <summary>
    /// Exibe no log todos os atributos atuais (apenas para debug).
    /// </summary>
    private void LogCurrentAttributes()
    {
        if (!enableLogs) return;

        Debug.Log($"[NPCAttributes] {gameObject.name} estado atual dos atributos:\n{GetAttributesSummary()}");
    }

    #endregion
}

/// <summary>
/// Estrutura para definir itens que podem ser dropados pelo NPC.
/// </summary>
[System.Serializable]
public struct NPCDropItem
{
    [Header("Configuração do Drop")]
    [Tooltip("Prefab do item que será dropado")]
    public GameObject itemPrefab;

    [Tooltip("Chance de drop em porcentagem (0-100)")]
    [Range(0f, 100f)]
    public float dropChance;
}
