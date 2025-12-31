using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Componente para objetos destrutíveis que podem receber dano e ativar animação de destruição
    /// </summary>
    public class BushDestruct : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        #endregion

        #region Private Fields
        private Animator animator;
        private bool isDestroyed = false;

        // Cache para performance - evita múltiplas chamadas GetComponent
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
            {enabled = false;
                return;
            }

            // Verifica se o trigger existe no Animator
            ValidateAnimatorTriggers();

            // Initialization complete
        }

        private void ValidateAnimatorTriggers()
        {
            if (animator == null) return;

            bool hasDestroy = false;

            // Verifica se o trigger existe no Animator Controller - otimizado para performance
            var parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].type == AnimatorControllerParameterType.Trigger &&
                    parameters[i].nameHash == DestroyTriggerHash)
                {
                    hasDestroy = true;
                    break; // Early exit para performance
                }
            }

            if (!hasDestroy)
            {
                // Destroy trigger not found
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Método público para receber dano de ataques.
        /// Implementa proteção contra destruição múltipla para evitar bugs.
        /// </summary>
        public void TakeDamage()
        {
            if (isDestroyed) return;

            isDestroyed = true;

            // Damage taken

            // Usa hash para performance em vez de string
            animator.SetTrigger(DestroyTriggerHash);

            // Verifica se existe componente DropController e executa o drop
            DropController dropController = GetComponent<DropController>();
            if (dropController != null)
            {
                dropController.DropItems();
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

        #region Context Menu (Editor Only)
        [ContextMenu("Test Destroy")]
        private void TestDestroy()
        {
            TakeDamage();
        }

        [ContextMenu("Reset Bush")]
        private void ResetBush()
        {
            isDestroyed = false;}

        [ContextMenu("Debug Info")]
        private void DebugInfo()
        {UnityEngine.Debug.Log($"Animator: {(animator != null ? "Found" : "Missing")}");
        }
        #endregion
    }
}