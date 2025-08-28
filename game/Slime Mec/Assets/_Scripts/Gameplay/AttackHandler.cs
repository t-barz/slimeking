using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Detecta objetos com tag "Destructable" e chama TakeDamage() neles durante ataques
    /// </summary>
    public class AttackHandler : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Attack Detection Settings")]
        [SerializeField] private float attackRadius = 1.5f;
        [SerializeField] private Vector2 attackOffset = Vector2.zero; // Offset do centro de ataque
        [SerializeField] private LayerMask destructableLayerMask = -1; // Layers dos objetos destrutíveis

        [Header("Debug")]
        [SerializeField] private bool showDebugGizmos = true;
        [SerializeField] private bool enableDebugLogs = false;
        #endregion

        #region Private Fields
        // Cache para performance
        private readonly Collider2D[] colliderCache = new Collider2D[8]; // Cache fixo para evitar allocations
        private Vector3[] gizmoPoints; // Cache dos pontos do gizmo

        // Propriedade para calcular posição de ataque com offset
        private Vector2 AttackCenter => (Vector2)transform.position + attackOffset;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            CacheGizmoPoints();

#if UNITY_EDITOR && DEVELOPMENT_BUILD
            if (enableDebugLogs)
                Debug.Log($"AttackHandler: Inicializado em {gameObject.name} com raio de ataque {attackRadius}");
#endif
        }
        #endregion

        #region Initialization
        private void CacheGizmoPoints()
        {
            // Pre-calcula pontos do gizmo para evitar recálculo todo frame
            const int segments = 32;
            gizmoPoints = new Vector3[segments + 1];

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * 2f * Mathf.PI / segments;
                gizmoPoints[i] = new Vector3(Mathf.Cos(angle) * attackRadius, Mathf.Sin(angle) * attackRadius, 0);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Executa o ataque, detectando e danificando objetos destrutíveis
        /// </summary>
        public void PerformAttack()
        {
            Debug.Log($"AttackHandler: Ataque executado");
            // Usa ContactFilter2D para detecção otimizada
            var filter = new ContactFilter2D();
            filter.SetLayerMask(destructableLayerMask);
            filter.useTriggers = true;

            int hitCount = Physics2D.OverlapCircle(AttackCenter, attackRadius, filter, colliderCache);

#if UNITY_EDITOR && DEVELOPMENT_BUILD
            if (enableDebugLogs)
                Debug.Log($"AttackHandler: Ataque executado, {hitCount} objetos detectados");
#endif

            // Early exit se não há detecções
            if (hitCount == 0) return;

            // Processa apenas os colliders detectados
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D col = colliderCache[i];
                if (col.gameObject == gameObject) continue; // Ignora o próprio atacante

                // Verifica se tem a tag "Destructable"
                if (col.CompareTag("Destructable"))
                {
                    // Tenta obter componente que implementa TakeDamage
                    var destructable = col.GetComponent<BushDestruct>();
                    if (destructable != null)
                    {
#if UNITY_EDITOR && DEVELOPMENT_BUILD
                        if (enableDebugLogs)
                            Debug.Log($"AttackHandler: Causando dano em '{col.name}'");
#endif
                        destructable.TakeDamage();
                    }
                    else
                    {
#if UNITY_EDITOR && DEVELOPMENT_BUILD
                        if (enableDebugLogs)
                            Debug.LogWarning($"AttackHandler: Objeto '{col.name}' tem tag 'Destructable' mas não tem componente BushDestruct");
#endif
                    }
                }
            }
        }
        #endregion

        #region Gizmos (Editor Only)
        private void OnDrawGizmos()
        {
            if (!showDebugGizmos) return;

            Gizmos.color = Color.green;
            DrawOptimizedCircleGizmo();

            // Desenha o centro de ataque se há offset
            if (attackOffset != Vector2.zero)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(AttackCenter, 0.1f);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!showDebugGizmos) return;

            Gizmos.color = Color.red;
            DrawOptimizedCircleGizmo();

            // Desenha o centro de ataque se há offset
            if (attackOffset != Vector2.zero)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(AttackCenter, 0.1f);

                // Desenha linha do transform até o centro de ataque
                Gizmos.color = Color.white;
                Gizmos.DrawLine(transform.position, AttackCenter);
            }
        }

        private void DrawOptimizedCircleGizmo()
        {
            if (gizmoPoints == null || gizmoPoints.Length == 0)
            {
                // Fallback se cache não foi inicializado
                DrawCircleGizmo(AttackCenter, attackRadius);
                return;
            }

            Vector3 attackCenter = AttackCenter;
            Vector3 lastPoint = attackCenter + gizmoPoints[0];

            for (int i = 1; i < gizmoPoints.Length; i++)
            {
                Vector3 newPoint = attackCenter + gizmoPoints[i];
                Gizmos.DrawLine(lastPoint, newPoint);
                lastPoint = newPoint;
            }
        }

        private void DrawCircleGizmo(Vector3 center, float radius)
        {
            const int segments = 32;
            float angle = 0f;
            Vector3 lastPoint = center + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                angle = i * 2f * Mathf.PI / segments;
                Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
                Gizmos.DrawLine(lastPoint, newPoint);
                lastPoint = newPoint;
            }
        }
        #endregion

        #region Context Menu (Editor Only)
        [ContextMenu("Test Attack")]
        private void TestAttack()
        {
            PerformAttack();
        }

        [ContextMenu("Debug Info")]
        private void DebugInfo()
        {
            Debug.Log($"=== AttackHandler Debug Info ===");
            Debug.Log($"GameObject: {gameObject.name}");
            Debug.Log($"Position: {transform.position}");
            Debug.Log($"Attack Center: {AttackCenter}");
            Debug.Log($"Attack Offset: {attackOffset}");
            Debug.Log($"Attack Radius: {attackRadius}");
            Debug.Log($"Layer Mask: {destructableLayerMask}");

            // Lista objetos detectados atualmente
            var filter = new ContactFilter2D();
            filter.SetLayerMask(destructableLayerMask);
            filter.useTriggers = true;

            int hitCount = Physics2D.OverlapCircle(AttackCenter, attackRadius, filter, colliderCache);
            Debug.Log($"Objetos destrutíveis detectados: {hitCount}");

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D col = colliderCache[i];
                if (col.gameObject != gameObject)
                {
                    bool hasDestructableTag = col.CompareTag("Destructable");
                    bool hasBushDestruct = col.GetComponent<BushDestruct>() != null;
                    Debug.Log($"  - {col.name} (Tag: {col.tag}, IsDestructable: {hasDestructableTag}, HasBushDestruct: {hasBushDestruct})");
                }
            }
        }
        #endregion
    }
}
