using UnityEngine;

namespace SlimeMec.Gameplay
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
            {
                Debug.LogError($"BushDestruct: Nenhum Animator encontrado em {gameObject.name}. Adicione um Animator component.");
                enabled = false;
                return;
            }

            // Verifica se o trigger existe no Animator
            ValidateAnimatorTriggers();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableDebugLogs)
                Debug.Log($"BushDestruct: Inicializado em {gameObject.name}");
#endif
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
                Debug.LogWarning($"BushDestruct: Trigger 'Destroy' não encontrado no Animator de {gameObject.name}");
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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableDebugLogs)
                Debug.Log($"BushDestruct: Ativando trigger 'Destroy' em {gameObject.name}");
#endif

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
            isDestroyed = false;
            Debug.Log($"BushDestruct: Bush {gameObject.name} resetado");
        }

        [ContextMenu("Debug Info")]
        private void DebugInfo()
        {
            Debug.Log($"=== BushDestruct Debug Info ===");
            Debug.Log($"GameObject: {gameObject.name}");
            Debug.Log($"Is Destroyed: {isDestroyed}");
            Debug.Log($"Animator: {(animator != null ? "Found" : "Missing")}");
        }
        #endregion
    }
}