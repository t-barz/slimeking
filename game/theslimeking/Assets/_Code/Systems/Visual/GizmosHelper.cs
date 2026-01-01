using UnityEngine;

namespace SlimeKing.Visual
{
    /// <summary>
    /// Helper para desenhar Gizmos que correspondem exatamente ao formato dos Collider2D.
    /// Suporta todos os tipos de Collider2D incluindo PolygonCollider2D.
    /// </summary>
    public class GizmosHelper : MonoBehaviour
    {
        [Header("Gizmo Settings")]
        [Tooltip("Cor do gizmo")]
        [SerializeField] private Color gizmoColor = Color.green;

        [Tooltip("Desenhar apenas as bordas do collider")]
        [SerializeField] private bool wireframe = true;

        [Tooltip("Desenhar preenchido (só funciona se wireframe = false)")]
        [SerializeField] private bool filled = false;

        [Tooltip("Espessura das linhas (para wireframe)")]
        [SerializeField, Range(0.01f, 0.1f)] private float lineWidth = 0.02f;

        [Tooltip("Mostrar apenas quando selecionado")]
        [SerializeField] private bool onlyWhenSelected = false;

        [Tooltip("Incluir colliders inativos")]
        [SerializeField] private bool includeInactive = false;

        [Header("🔍 Debug Info")]
        [Tooltip("Mostrar informações do collider")]
        [SerializeField] private bool showDebugInfo = false;

        [Tooltip("Cor do texto de debug")]
        [SerializeField] private Color debugTextColor = Color.white;

        void OnDrawGizmos()
        {
            if (onlyWhenSelected) return;
            DrawColliderGizmos();
        }

        void OnDrawGizmosSelected()
        {
            if (!onlyWhenSelected) return;
            DrawColliderGizmos();
        }

        private void DrawColliderGizmos()
        {
            var colliders = includeInactive 
                ? GetComponentsInChildren<Collider2D>(true) 
                : GetComponentsInChildren<Collider2D>();

            foreach (var collider in colliders)
            {
                if (collider == null) continue;
                
                DrawCollider2D(collider);
            }
        }

        private void DrawCollider2D(Collider2D collider)
        {
            Gizmos.color = gizmoColor;
            
            switch (collider)
            {
                case BoxCollider2D boxCollider:
                    DrawBoxCollider(boxCollider);
                    break;
                    
                case CircleCollider2D circleCollider:
                    DrawCircleCollider(circleCollider);
                    break;
                    
                case CapsuleCollider2D capsuleCollider:
                    DrawCapsuleCollider(capsuleCollider);
                    break;
                    
                case PolygonCollider2D polygonCollider:
                    DrawPolygonCollider(polygonCollider);
                    break;
                    
                case EdgeCollider2D edgeCollider:
                    DrawEdgeCollider(edgeCollider);
                    break;
                    
                case CompositeCollider2D compositeCollider:
                    DrawCompositeCollider(compositeCollider);
                    break;
            }

            if (showDebugInfo)
            {
                DrawDebugInfo(collider);
            }
        }

        private void DrawBoxCollider(BoxCollider2D boxCollider)
        {
            Vector2 size = boxCollider.size;
            Vector2 offset = boxCollider.offset;
            Transform transform = boxCollider.transform;

            Vector3[] corners = new Vector3[4];
            Vector2 halfSize = size * 0.5f;
            
            corners[0] = new Vector2(-halfSize.x, -halfSize.y) + offset;
            corners[1] = new Vector2(halfSize.x, -halfSize.y) + offset;
            corners[2] = new Vector2(halfSize.x, halfSize.y) + offset;
            corners[3] = new Vector2(-halfSize.x, halfSize.y) + offset;

            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = transform.TransformPoint(corners[i]);
            }

            if (wireframe)
            {
                for (int i = 0; i < 4; i++)
                {
                    int next = (i + 1) % 4;
                    Gizmos.DrawLine(corners[i], corners[next]);
                }
            }
            else if (filled)
            {
                // Desenha as diagonais para simular preenchimento
                Gizmos.DrawLine(corners[0], corners[2]);
                Gizmos.DrawLine(corners[1], corners[3]);
            }
        }

        private void DrawCircleCollider(CircleCollider2D circleCollider)
        {
            Vector3 center = circleCollider.transform.TransformPoint(circleCollider.offset);
            float radius = circleCollider.radius * Mathf.Max(
                circleCollider.transform.lossyScale.x, 
                circleCollider.transform.lossyScale.y
            );

            if (wireframe)
            {
                Gizmos.DrawWireSphere(center, radius);
            }
            else if (filled)
            {
                Gizmos.DrawSphere(center, radius);
            }
        }

        private void DrawCapsuleCollider(CapsuleCollider2D capsuleCollider)
        {
            Vector2 size = capsuleCollider.size;
            Vector2 offset = capsuleCollider.offset;
            Transform transform = capsuleCollider.transform;
            CapsuleDirection2D direction = capsuleCollider.direction;

            Vector3 center = transform.TransformPoint(offset);
            
            float radius, height;
            Vector3 directionVector;

            if (direction == CapsuleDirection2D.Vertical)
            {
                radius = size.x * 0.5f * transform.lossyScale.x;
                height = size.y * transform.lossyScale.y;
                directionVector = transform.up;
            }
            else
            {
                radius = size.y * 0.5f * transform.lossyScale.y;
                height = size.x * transform.lossyScale.x;
                directionVector = transform.right;
            }

            float halfHeight = Mathf.Max(0, (height - radius * 2) * 0.5f);
            Vector3 topCenter = center + directionVector * halfHeight;
            Vector3 bottomCenter = center - directionVector * halfHeight;

            if (wireframe)
            {
                Gizmos.DrawWireSphere(topCenter, radius);
                Gizmos.DrawWireSphere(bottomCenter, radius);
                
                Vector3 perpendicular = Vector3.Cross(directionVector, Vector3.forward).normalized;
                Gizmos.DrawLine(topCenter + perpendicular * radius, bottomCenter + perpendicular * radius);
                Gizmos.DrawLine(topCenter - perpendicular * radius, bottomCenter - perpendicular * radius);
            }
            else if (filled)
            {
                Gizmos.DrawSphere(topCenter, radius);
                Gizmos.DrawSphere(bottomCenter, radius);
            }
        }

        private void DrawPolygonCollider(PolygonCollider2D polygonCollider)
        {
            for (int pathIndex = 0; pathIndex < polygonCollider.pathCount; pathIndex++)
            {
                Vector2[] path = polygonCollider.GetPath(pathIndex);
                if (path.Length < 2) continue;

                Vector3[] worldPoints = new Vector3[path.Length];
                for (int i = 0; i < path.Length; i++)
                {
                    Vector2 localPoint = path[i] + polygonCollider.offset;
                    worldPoints[i] = polygonCollider.transform.TransformPoint(localPoint);
                }

                if (wireframe)
                {
                    for (int i = 0; i < worldPoints.Length; i++)
                    {
                        int nextIndex = (i + 1) % worldPoints.Length;
                        Gizmos.DrawLine(worldPoints[i], worldPoints[nextIndex]);
                    }
                }
                else if (filled && worldPoints.Length >= 3)
                {
                    // Triangulação simples para polígonos convexos
                    for (int i = 1; i < worldPoints.Length - 1; i++)
                    {
                        Gizmos.DrawLine(worldPoints[0], worldPoints[i]);
                        Gizmos.DrawLine(worldPoints[i], worldPoints[i + 1]);
                        Gizmos.DrawLine(worldPoints[i + 1], worldPoints[0]);
                    }
                }
            }
        }

        private void DrawEdgeCollider(EdgeCollider2D edgeCollider)
        {
            Vector2[] points = edgeCollider.points;
            if (points.Length < 2) return;

            Vector3[] worldPoints = new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 localPoint = points[i] + edgeCollider.offset;
                worldPoints[i] = edgeCollider.transform.TransformPoint(localPoint);
            }

            for (int i = 0; i < worldPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(worldPoints[i], worldPoints[i + 1]);
            }
        }

        private void DrawCompositeCollider(CompositeCollider2D compositeCollider)
        {
            for (int pathIndex = 0; pathIndex < compositeCollider.pathCount; pathIndex++)
            {
                Vector2[] path = new Vector2[compositeCollider.GetPathPointCount(pathIndex)];
                compositeCollider.GetPath(pathIndex, path);

                if (path.Length < 2) continue;

                Vector3[] worldPoints = new Vector3[path.Length];
                for (int i = 0; i < path.Length; i++)
                {
                    worldPoints[i] = compositeCollider.transform.TransformPoint(path[i]);
                }

                if (wireframe)
                {
                    for (int i = 0; i < worldPoints.Length; i++)
                    {
                        int nextIndex = (i + 1) % worldPoints.Length;
                        Gizmos.DrawLine(worldPoints[i], worldPoints[nextIndex]);
                    }
                }
            }
        }

        private void DrawDebugInfo(Collider2D collider)
        {
#if UNITY_EDITOR
            Vector3 position = collider.bounds.center + Vector3.up * (collider.bounds.size.y * 0.5f + 0.3f);
            
            string info = $"{collider.GetType().Name}\n";
            info += $"Enabled: {collider.enabled}\n";
            info += $"Trigger: {collider.isTrigger}";

            if (collider is PolygonCollider2D polygon)
            {
                info += $"\nPaths: {polygon.pathCount}";
                info += $"\nPoints: {polygon.GetTotalPointCount()}";
            }

            UnityEditor.Handles.color = debugTextColor;
            UnityEditor.Handles.Label(position, info);
#endif
        }

        #region Public Methods

        /// <summary>
        /// Força a atualização dos gizmos
        /// </summary>
        [ContextMenu("🔄 Refresh Gizmos")]
        public void RefreshGizmos()
        {
#if UNITY_EDITOR
            UnityEditor.SceneView.RepaintAll();
#endif
        }

        /// <summary>
        /// Mostra informações de todos os colliders
        /// </summary>
        [ContextMenu("📊 Show Collider Info")]
        public void ShowColliderInfo()
        {
            var colliders = GetComponentsInChildren<Collider2D>(true);
            
            UnityEngine.Debug.Log($"=== COLLIDER INFO ===");
            UnityEngine.Debug.Log($"GameObject: {gameObject.name}");
            UnityEngine.Debug.Log($"Total Colliders: {colliders.Length}");
            
            foreach (var collider in colliders)
            {
                UnityEngine.Debug.Log($"• {collider.GetType().Name} - {collider.name} (Enabled: {collider.enabled})");
            }
            UnityEngine.Debug.Log($"===================");
        }

        #endregion
    }
}
