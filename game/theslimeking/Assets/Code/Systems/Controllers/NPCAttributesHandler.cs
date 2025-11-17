using UnityEngine;
using System;

namespace SlimeMec.Systems.Controllers
{
    /// <summary>
    /// Gerencia todos os atributos de um NPC.
    /// Responsável por controlar pontos de vida, ataque, defesa, velocidade e sistema de relacionamento.
    /// Esta classe deve ser anexada ao GameObject do NPC para funcionamento correto.
    /// </summary>
    public class NPCAttributesHandler : MonoBehaviour
    {
        #region Inspector Configuration

        [Header("Configurações de Atributos Base")]
        [SerializeField] private int baseHealth = 100;
        [SerializeField] private int baseAttack = 10;
        [SerializeField] private int baseDefense = 5;
        [SerializeField] private int baseSpeed = 2;

        [Header("Configurações de Relacionamento")]
        [Tooltip("Pontos de relacionamento com o jogador (-100 a 100). <0=Hostil, 0-10=Neutro, >10=Amigável")]
        [SerializeField] [Range(-100, 100)] private int relationshipPoints = 0;

        [Header("Configurações de Debug")]
        [SerializeField] private bool enableLogs = false;
        [SerializeField] private bool enableDebugGizmos = true;

        #endregion

        #region Private Variables

        private int _currentHealth;
        private int _currentAttack;
        private int _currentDefense;
        private int _currentSpeed;

        #endregion

        #region Public Properties

        /// <summary>
        /// Pontos de vida atuais do NPC.
        /// </summary>
        public int CurrentHealth
        {
            get => _currentHealth;
            set
            {
                int oldValue = _currentHealth;
                _currentHealth = Mathf.Clamp(value, 0, MaxHealth);

                if (enableLogs && oldValue != _currentHealth)
                {
                    Debug.Log($"[NPCAttributes] {gameObject.name} Health alterado: {oldValue} → {_currentHealth}");
                }

                OnHealthChanged?.Invoke(_currentHealth, MaxHealth);

                if (_currentHealth <= 0 && oldValue > 0)
                {
                    OnNPCDied?.Invoke();
                    if (enableLogs)
                    {
                        Debug.Log($"[NPCAttributes] {gameObject.name} morreu!");
                    }
                }
            }
        }

        /// <summary>
        /// Pontos de vida máximos do NPC.
        /// </summary>
        public int MaxHealth
        {
            get => baseHealth;
        }

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
        /// Pontos de relacionamento com o jogador.
        /// </summary>
        public int RelationshipPoints
        {
            get => relationshipPoints;
            private set
            {
                int oldValue = relationshipPoints;
                relationshipPoints = Mathf.Clamp(value, -100, 100);

                if (enableLogs && oldValue != relationshipPoints)
                {
                    Debug.Log($"[NPCAttributes] {gameObject.name} Relationship alterado: {oldValue} → {relationshipPoints}");
                }

                OnRelationshipChanged?.Invoke(relationshipPoints);
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
        /// Evento disparado quando o NPC morre.
        /// </summary>
        public event Action OnNPCDied;

        /// <summary>
        /// Evento disparado quando o relacionamento muda.
        /// Parâmetros: (newRelationshipPoints)
        /// </summary>
        public event Action<int> OnRelationshipChanged;

        /// <summary>
        /// Evento disparado quando o NPC recebe dano.
        /// Parâmetros: (damageAmount)
        /// </summary>
        public event Action<int> OnDamageTaken;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Inicializa os atributos do NPC
            InitializeAttributes();
        }

        private void Start()
        {
            if (enableLogs)
            {
                Debug.Log($"[NPCAttributes] Sistema de atributos do NPC '{gameObject.name}' inicializado.");
                LogCurrentAttributes();
            }
        }

        private void OnDrawGizmos()
        {
            if (!enableDebugGizmos) return;

            // Desenha informações dos atributos no Scene View
            Vector3 position = transform.position + Vector3.up * 1.5f;

#if UNITY_EDITOR
            UnityEditor.Handles.Label(position, 
                $"HP: {CurrentHealth}/{MaxHealth}\n" +
                $"ATK: {CurrentAttack} | DEF: {CurrentDefense}\n" +
                $"SPD: {CurrentSpeed} | REL: {RelationshipPoints}");
#endif
        }

        #endregion

        #region Public Methods - Combat

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

            if (!ignoreDefense && CurrentDefense > 0)
            {
                // Calcula redução de dano baseada na defesa
                // Fórmula: defense * 100 / (defense + 100)
                float damageReduction = (CurrentDefense * 100f) / (CurrentDefense + 100f);
                finalDamage = damage - Mathf.RoundToInt((damage * damageReduction) / 100f);
                finalDamage = Mathf.Max(1, finalDamage); // Garante pelo menos 1 de dano
            }

            CurrentHealth -= finalDamage;

            if (enableLogs)
            {
                Debug.Log($"[NPCAttributes] {gameObject.name} recebeu dano: {damage} | Dano final: {finalDamage} | HP restante: {CurrentHealth}");
            }

            OnDamageTaken?.Invoke(finalDamage);

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

            int oldHealth = CurrentHealth;
            CurrentHealth += healAmount;
            int actualHeal = CurrentHealth - oldHealth;

            if (enableLogs)
            {
                Debug.Log($"[NPCAttributes] {gameObject.name} curado: {healAmount} | Cura real: {actualHeal} | HP atual: {CurrentHealth}");
            }

            return actualHeal;
        }

        #endregion

        #region Public Methods - Relationship

        /// <summary>
        /// Modifica o relacionamento com o jogador.
        /// </summary>
        /// <param name="amount">Quantidade a modificar (positivo ou negativo)</param>
        public void ModifyRelationship(int amount)
        {
            RelationshipPoints += amount;
        }

        /// <summary>
        /// Aumenta o relacionamento com o jogador.
        /// </summary>
        /// <param name="amount">Quantidade a aumentar</param>
        public void IncreaseRelationship(int amount)
        {
            if (amount > 0)
            {
                RelationshipPoints += amount;
            }
        }

        /// <summary>
        /// Diminui o relacionamento com o jogador.
        /// </summary>
        /// <param name="amount">Quantidade a diminuir</param>
        public void DecreaseRelationship(int amount)
        {
            if (amount > 0)
            {
                RelationshipPoints -= amount;
            }
        }

        /// <summary>
        /// Verifica se o NPC é hostil ao jogador.
        /// </summary>
        /// <returns>True se RelationshipPoints < 0</returns>
        public bool IsHostile()
        {
            return RelationshipPoints < 0;
        }

        /// <summary>
        /// Verifica se o NPC é amigável ao jogador.
        /// </summary>
        /// <returns>True se RelationshipPoints > 10</returns>
        public bool IsFriendly()
        {
            return RelationshipPoints > 10;
        }

        /// <summary>
        /// Verifica se o NPC é neutro ao jogador.
        /// </summary>
        /// <returns>True se 0 <= RelationshipPoints <= 10</returns>
        public bool IsNeutral()
        {
            return RelationshipPoints >= 0 && RelationshipPoints <= 10;
        }

        #endregion

        #region Public Methods - Utility

        /// <summary>
        /// Obtém um resumo de todos os atributos atuais.
        /// </summary>
        /// <returns>String formatada com todos os atributos</returns>
        public string GetAttributesSummary()
        {
            return $"HP: {CurrentHealth}/{MaxHealth} | " +
                   $"ATK: {CurrentAttack} | DEF: {CurrentDefense} | " +
                   $"SPD: {CurrentSpeed} | REL: {RelationshipPoints}";
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Inicializa os atributos com os valores base.
        /// </summary>
        private void InitializeAttributes()
        {
            _currentHealth = baseHealth;
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

            Debug.Log($"[NPCAttributes] {gameObject.name} estado atual:\n{GetAttributesSummary()}");
        }

        #endregion
    }
}
