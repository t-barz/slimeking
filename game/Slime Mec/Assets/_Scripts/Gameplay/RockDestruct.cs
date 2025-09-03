using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Componente para rochas destrutíveis que podem receber múltiplos ataques antes de serem destruídas
    /// </summary>
    public class RockDestruct : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Rock Configuration")]
        [SerializeField] private int attacksToDestroy = 3;
        [Tooltip("Quantos ataques a rocha aguenta antes de ser completamente destruída")]

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        #endregion

        #region Private Fields
        private Animator animator;
        private int currentAttacks = 0;
        private bool isCracked = false;
        private bool isDestroyed = false;

        // Cache para performance - evita múltiplas chamadas GetComponent
        private static readonly int CrackTriggerHash = Animator.StringToHash("Crack");
        private static readonly int DestroyTriggerHash = Animator.StringToHash("Destroy");
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            InitializeComponents();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            // Cache do componente Animator
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError($"RockDestruct: Nenhum Animator encontrado em {gameObject.name}. Adicione um Animator component.");
                enabled = false;
                return;
            }

            // Valida a configuração
            if (attacksToDestroy <= 0)
            {
                Debug.LogWarning($"RockDestruct: attacksToDestroy deve ser maior que 0 em {gameObject.name}. Definindo para 1.");
                attacksToDestroy = 1;
            }

            // Verifica se os triggers existem no Animator
            ValidateAnimatorTriggers();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableDebugLogs)
                Debug.Log($"RockDestruct: Inicializado em {gameObject.name} - Ataques necessários: {attacksToDestroy}");
#endif
        }

        private void ValidateAnimatorTriggers()
        {
            if (animator == null) return;

            bool hasCrack = false;
            bool hasDestroy = false;

            // Verifica se os triggers existem no Animator Controller - otimizado para performance
            var parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].type == AnimatorControllerParameterType.Trigger)
                {
                    if (parameters[i].nameHash == CrackTriggerHash)
                        hasCrack = true;
                    else if (parameters[i].nameHash == DestroyTriggerHash)
                        hasDestroy = true;
                }

                // Early exit quando ambos triggers forem encontrados
                if (hasCrack && hasDestroy) break;
            }

            if (!hasCrack)
                Debug.LogWarning($"RockDestruct: Trigger 'Crack' não encontrado no Animator de {gameObject.name}");
            if (!hasDestroy)
                Debug.LogWarning($"RockDestruct: Trigger 'Destroy' não encontrado no Animator de {gameObject.name}");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Método público para receber dano de ataques.
        /// Controla o sistema de múltiplos ataques com triggers para Crack e Destroy.
        /// </summary>
        public void TakeDamage()
        {
            if (isDestroyed) return;

            currentAttacks++;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableDebugLogs)
                Debug.Log($"RockDestruct: {gameObject.name} recebeu ataque {currentAttacks}/{attacksToDestroy}");
#endif

            // Calcula quando deve crackar (metade dos ataques necessários)
            int attacksToeCrack = Mathf.CeilToInt(attacksToDestroy / 2f);

            // Verifica se deve ativar trigger Crack
            if (!isCracked && currentAttacks >= attacksToeCrack)
            {
                isCracked = true;
                animator.SetTrigger(CrackTriggerHash);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (enableDebugLogs)
                    Debug.Log($"RockDestruct: Ativando trigger 'Crack' em {gameObject.name}");
#endif
            }

            // Verifica se deve ativar trigger Destroy
            if (currentAttacks >= attacksToDestroy)
            {
                isDestroyed = true;
                animator.SetTrigger(DestroyTriggerHash);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (enableDebugLogs)
                    Debug.Log($"RockDestruct: Ativando trigger 'Destroy' em {gameObject.name}");
#endif

                // Verifica se existe componente DropController e executa o drop
                DropController dropController = GetComponent<DropController>();
                if (dropController != null)
                {
                    dropController.DropItems();
                }
            }
        }

        /// <summary>
        /// Destroi o GameObject onde este script está anexado.
        /// Método público que pode ser chamado por outros scripts ou eventos.
        /// </summary>
        public void DestroyObject()
        {
            Destroy(gameObject);
        }
        #endregion

        #region Public Properties (Read-Only)
        /// <summary>
        /// Retorna o número atual de ataques recebidos
        /// </summary>
        public int CurrentAttacks => currentAttacks;

        /// <summary>
        /// Retorna o número total de ataques necessários para destruir
        /// </summary>
        public int AttacksToDestroy => attacksToDestroy;

        /// <summary>
        /// Retorna se a rocha está no estado crackeado
        /// </summary>
        public bool IsCracked => isCracked;

        /// <summary>
        /// Retorna se a rocha está destruída
        /// </summary>
        public bool IsDestroyed => isDestroyed;
        #endregion

        #region Context Menu (Editor Only)
        [ContextMenu("Test Single Attack")]
        private void TestSingleAttack()
        {
            TakeDamage();
        }

        [ContextMenu("Test Crack")]
        private void TestCrack()
        {
            int attacksToeCrack = Mathf.CeilToInt(attacksToDestroy / 2f);
            currentAttacks = attacksToeCrack - 1;
            TakeDamage();
        }

        [ContextMenu("Test Destroy")]
        private void TestDestroy()
        {
            currentAttacks = attacksToDestroy - 1;
            TakeDamage();
        }

        [ContextMenu("Reset Rock")]
        private void ResetRock()
        {
            currentAttacks = 0;
            isCracked = false;
            isDestroyed = false;
            Debug.Log($"RockDestruct: Rock {gameObject.name} resetado");
        }

        [ContextMenu("Debug Info")]
        private void DebugInfo()
        {
            Debug.Log($"=== RockDestruct Debug Info ===");
            Debug.Log($"GameObject: {gameObject.name}");
            Debug.Log($"Current Attacks: {currentAttacks}/{attacksToDestroy}");
            Debug.Log($"Attacks to Crack: {Mathf.CeilToInt(attacksToDestroy / 2f)}");
            Debug.Log($"Is Cracked: {isCracked}");
            Debug.Log($"Is Destroyed: {isDestroyed}");
            Debug.Log($"Animator: {(animator != null ? "Found" : "Missing")}");
        }
        #endregion
    }
}
