using UnityEngine;

namespace TheSlimeKing.NPCs
{
    /// <summary>
    /// Handler base para atributos de NPCs.
    /// Gerencia vida, ataque, defesa e velocidade dos NPCs.
    /// </summary>
    public class NPCAttributesHandler : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Atributos Base")]
        [SerializeField] protected int baseHealthPoints = 3;
        [SerializeField] protected int baseAttack = 1;
        [SerializeField] protected int baseDefense = 1;
        [SerializeField] protected float baseSpeed = 2.0f;

        [Header("Debug")]
        [SerializeField] protected bool enableLogs = false;
        #endregion

        #region Private Variables
        protected int _currentHealthPoints;
        protected int _currentAttack;
        protected int _currentDefense;
        protected float _currentSpeed;
        protected bool _isInitialized = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// Pontos de vida atuais do NPC.
        /// </summary>
        public int CurrentHealthPoints
        {
            get 
            {
                if (!_isInitialized)
                {
                    UnityEngine.Debug.LogWarning($"[NPCAttributes] {gameObject.name} - CurrentHealthPoints acessado antes da inicialização!");
                    InitializeAttributes();
                }
                return _currentHealthPoints;
            }
            protected set
            {
                int oldValue = _currentHealthPoints;
                _currentHealthPoints = Mathf.Max(0, value);

                if (enableLogs && oldValue != _currentHealthPoints)
                {
                    UnityEngine.Debug.Log($"[NPCAttributes] {gameObject.name} - HealthPoints alterado: {oldValue} → {_currentHealthPoints}");
                }
            }
        }

        /// <summary>
        /// Valor de ataque atual do NPC.
        /// </summary>
        public int CurrentAttack
        {
            get 
            {
                if (!_isInitialized)
                {
                    UnityEngine.Debug.LogWarning($"[NPCAttributes] {gameObject.name} - CurrentAttack acessado antes da inicialização!");
                    InitializeAttributes();
                }
                return _currentAttack;
            }
            protected set
            {
                int oldValue = _currentAttack;
                _currentAttack = Mathf.Max(0, value);

                if (enableLogs && oldValue != _currentAttack)
                {
                    UnityEngine.Debug.Log($"[NPCAttributes] {gameObject.name} - Attack alterado: {oldValue} → {_currentAttack}");
                }
            }
        }

        /// <summary>
        /// Valor de defesa atual do NPC.
        /// </summary>
        public int CurrentDefense
        {
            get 
            {
                if (!_isInitialized)
                {
                    UnityEngine.Debug.LogWarning($"[NPCAttributes] {gameObject.name} - CurrentDefense acessado antes da inicialização!");
                    InitializeAttributes();
                }
                return _currentDefense;
            }
            protected set
            {
                int oldValue = _currentDefense;
                _currentDefense = Mathf.Max(0, value);

                if (enableLogs && oldValue != _currentDefense)
                {
                    UnityEngine.Debug.Log($"[NPCAttributes] {gameObject.name} - Defense alterado: {oldValue} → {_currentDefense}");
                }
            }
        }

        /// <summary>
        /// Velocidade atual do NPC.
        /// </summary>
        public float CurrentSpeed
        {
            get 
            {
                if (!_isInitialized)
                {
                    UnityEngine.Debug.LogWarning($"[NPCAttributes] {gameObject.name} - CurrentSpeed acessado antes da inicialização!");
                    InitializeAttributes();
                }
                return _currentSpeed;
            }
            protected set
            {
                float oldValue = _currentSpeed;
                _currentSpeed = Mathf.Max(0f, value);

                if (enableLogs && !Mathf.Approximately(oldValue, _currentSpeed))
                {
                    UnityEngine.Debug.Log($"[NPCAttributes] {gameObject.name} - Speed alterado: {oldValue:F2} → {_currentSpeed:F2}");
                }
            }
        }

        /// <summary>
        /// Verifica se o NPC está morto.
        /// </summary>
        public bool IsDead => CurrentHealthPoints <= 0;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            InitializeAttributes();
        }

        protected virtual void Start()
        {
            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[NPCAttributes] {gameObject.name} - Sistema de atributos inicializado.");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Causa dano ao NPC.
        /// </summary>
        /// <param name="damage">Quantidade de dano</param>
        /// <returns>True se o NPC morreu com o dano</returns>
        public virtual bool TakeDamage(int damage)
        {
            if (IsDead)
            {
                if (enableLogs)
                {
                    UnityEngine.Debug.Log($"[NPCAttributes] {gameObject.name} - Já está morto, ignorando dano");
                }
                return true;
            }

            int oldHealth = CurrentHealthPoints;
            CurrentHealthPoints -= damage;

            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[NPCAttributes] {gameObject.name} - Dano recebido: {damage}. Vida: {oldHealth} → {CurrentHealthPoints}");
            }

            bool died = CurrentHealthPoints <= 0;
            if (died)
            {
                OnDeath();
            }

            return died;
        }

        /// <summary>
        /// Cura o NPC.
        /// </summary>
        /// <param name="amount">Quantidade de cura</param>
        public virtual void Heal(int amount)
        {
            if (amount <= 0) return;

            CurrentHealthPoints += amount;

            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[NPCAttributes] {gameObject.name} - Curado: +{amount} HP. Vida atual: {CurrentHealthPoints}");
            }
        }

        /// <summary>
        /// Reseta os atributos para os valores base.
        /// </summary>
        public virtual void ResetAttributes()
        {
            InitializeAttributes();
            
            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[NPCAttributes] {gameObject.name} - Atributos resetados");
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Inicializa os atributos com os valores base.
        /// </summary>
        protected virtual void InitializeAttributes()
        {
            _currentHealthPoints = baseHealthPoints;
            _currentAttack = baseAttack;
            _currentDefense = baseDefense;
            _currentSpeed = baseSpeed;
            _isInitialized = true;

            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[NPCAttributes] {gameObject.name} - Atributos inicializados - HP: {_currentHealthPoints}, ATK: {_currentAttack}, DEF: {_currentDefense}, SPD: {_currentSpeed:F2}");
            }
        }

        /// <summary>
        /// Chamado quando o NPC morre. Override para implementar comportamento específico.
        /// </summary>
        protected virtual void OnDeath()
        {
            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[NPCAttributes] {gameObject.name} - NPC morreu!");
            }
            
            // Verifica se existe componente DropController e executa o drop
            var dropController = GetComponent<DropController>();
            if (dropController != null)
            {
                dropController.DropItems();
                if (enableLogs)
                {
                    UnityEngine.Debug.Log($"[NPCAttributes] {gameObject.name} - Drops executados!");
                }
            }
            
            // Implementação base: desativar GameObject
            gameObject.SetActive(false);
        }
        #endregion

        #region Debug Methods
        /// <summary>
        /// Exibe informações de debug dos atributos atuais.
        /// </summary>
        [ContextMenu("Debug Attributes")]
        public void DebugAttributes()
        {
            UnityEngine.Debug.Log($"=== {gameObject.name} ATTRIBUTES ===");
            UnityEngine.Debug.Log($"Health: {CurrentHealthPoints}/{baseHealthPoints}");
            UnityEngine.Debug.Log($"Attack: {CurrentAttack} (base: {baseAttack})");
            UnityEngine.Debug.Log($"Defense: {CurrentDefense} (base: {baseDefense})");
            UnityEngine.Debug.Log($"Speed: {CurrentSpeed:F2} (base: {baseSpeed:F2})");
            UnityEngine.Debug.Log($"Is Dead: {IsDead}");
            UnityEngine.Debug.Log($"Is Initialized: {_isInitialized}");
        }
        #endregion
    }
}