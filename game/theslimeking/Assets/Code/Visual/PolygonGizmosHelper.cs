using UnityEngine;

namespace SlimeKing.Visual
{
    /// <summary>
    /// Versão especializada do GizmosHelper otimizada especificamente para PolygonCollider2D.
    /// Oferece visualização mais detalhada e opções específicas para polígonos.
    /// </summary>
    public class PolygonGizmosHelper : MonoBehaviour
    {
        [Header("🔺 Polygon Gizmo Settings")]
        [Tooltip("Cor das bordas do polígono")]
        [SerializeField] private Color edgeColor = Color.green;

        [Tooltip("Cor dos vértices")]
        [SerializeField] private Color vertexColor = Color.red;

        [Tooltip("Cor de preenchimento (se ativo)")]
        [SerializeField] private Color fillColor = new Color(0, 1, 0, 0.2f);

        [Tooltip("Desenhar as bordas do polígono")]
        [SerializeField] private bool showEdges = true;

        [Tooltip("Desenhar os vértices do polígono")]
        [SerializeField] private bool showVertices = true;

        [Tooltip("Desenhar preenchimento do polígono")]
        [SerializeField] private bool showFill = false;

        [Tooltip("Tamanho dos vértices")]
        [SerializeField, Range(0.05f, 0.5f)] private float vertexSize = 0.1f;

        [Tooltip("Espessura das bordas")]
        [SerializeField, Range(0.01f, 0.1f)] private float edgeThickness = 0.02f;

        [Header("🔍 Polygon Analysis")]
        [Tooltip("Mostrar informações detalhadas")]
        [SerializeField] private bool showDetailedInfo = false;

        [Tooltip("Mostrar números dos vértices")]
        [SerializeField] private bool showVertexNumbers = false;

        [Tooltip("Mostrar centro de massa")]
        [SerializeField] private bool showCentroid = true;

        [Tooltip("Cor do texto de informações")]
        [SerializeField] private Color infoTextColor = Color.white;

        [Header("⚙️ Display Options")]
        [Tooltip("Mostrar apenas quando selecionado")]
        [SerializeField] private bool onlyWhenSelected = false;

        [Tooltip("Incluir paths inativos")]
        [SerializeField] private bool includeAllPaths = true;

        private PolygonCollider2D[] polygonColliders;

        void OnDrawGizmos()
        {
            if (onlyWhenSelected) return;
            DrawPolygonGizmos();
        }

        void OnDrawGizmosSelected()
        {
            if (!onlyWhenSelected) return;
            DrawPolygonGizmos();
        }

        private void DrawPolygonGizmos()
        {
            // Cache dos colliders para performance
            if (polygonColliders == null)
            {
                polygonColliders = GetComponentsInChildren<PolygonCollider2D>();
            }

            foreach (var polygonCollider in polygonColliders)
            {
                if (polygonCollider == null || !polygonCollider.enabled) continue;

                DrawPolygonCollider(polygonCollider);
            }
        }

        private void DrawPolygonCollider(PolygonCollider2D polygonCollider)
        {
            for (int pathIndex = 0; pathIndex < polygonCollider.pathCount; pathIndex++)
            {
                Vector2[] path = polygonCollider.GetPath(pathIndex);
                if (path.Length < 3) continue;

                Vector3[] worldPoints = ConvertToWorldPoints(polygonCollider, path);

                if (showFill)
                {
                    DrawPolygonFill(worldPoints);
                }

                if (showEdges)
                {
                    DrawPolygonEdges(worldPoints);
                }

                if (showVertices)
                {
                    DrawPolygonVertices(worldPoints, pathIndex);
                }

                if (showCentroid)
                {
                    DrawPolygonCentroid(worldPoints);
                }

                if (showDetailedInfo)
                {
                    DrawDetailedInfo(polygonCollider, pathIndex, worldPoints);
                }
            }
        }

        private Vector3[] ConvertToWorldPoints(PolygonCollider2D polygonCollider, Vector2[] path)
        {
            Vector3[] worldPoints = new Vector3[path.Length];

            for (int i = 0; i < path.Length; i++)
            {
                Vector2 localPoint = path[i] + polygonCollider.offset;
                worldPoints[i] = polygonCollider.transform.TransformPoint(localPoint);
            }

            return worldPoints;
        }

        private void DrawPolygonFill(Vector3[] points)
        {
            Gizmos.color = fillColor;

            // Triangulação simples usando método fan para polígonos convexos
            if (points.Length >= 3)
            {
                for (int i = 1; i < points.Length - 1; i++)
                {
                    // Cria triângulos a partir do primeiro vértice
                    Vector3[] triangle = { points[0], points[i], points[i + 1] };
                    DrawTriangle(triangle);
                }
            }
        }

        private void DrawTriangle(Vector3[] triangle)
        {
            // Desenha um triângulo preenchido usando múltiplas linhas
            Vector3 center = (triangle[0] + triangle[1] + triangle[2]) / 3f;

            for (int i = 0; i < 3; i++)
            {
                Vector3 edge = Vector3.Lerp(triangle[i], triangle[(i + 1) % 3], 0.5f);
                Gizmos.DrawLine(center, edge);
            }
        }

        private void DrawPolygonEdges(Vector3[] points)
        {
            Gizmos.color = edgeColor;

            for (int i = 0; i < points.Length; i++)
            {
                int nextIndex = (i + 1) % points.Length;
                Gizmos.DrawLine(points[i], points[nextIndex]);

                // Desenha linhas mais grossas se necessário
                if (edgeThickness > 0.02f)
                {
                    Vector3 perpendicular = Vector3.Cross(points[nextIndex] - points[i], Vector3.forward).normalized;
                    Vector3 offset = perpendicular * edgeThickness * 0.5f;

                    Gizmos.DrawLine(points[i] + offset, points[nextIndex] + offset);
                    Gizmos.DrawLine(points[i] - offset, points[nextIndex] - offset);
                }
            }
        }

        private void DrawPolygonVertices(Vector3[] points, int pathIndex)
        {
            Gizmos.color = vertexColor;

            for (int i = 0; i < points.Length; i++)
            {
                // Desenha o vértice
                Gizmos.DrawWireSphere(points[i], vertexSize);

                // Desenha número do vértice se habilitado
                if (showVertexNumbers)
                {
#if UNITY_EDITOR
                    UnityEditor.Handles.color = infoTextColor;
                    UnityEditor.Handles.Label(points[i] + Vector3.up * (vertexSize + 0.1f), $"P{pathIndex}.{i}");
#endif
                }
            }
        }

        private void DrawPolygonCentroid(Vector3[] points)
        {
            Vector3 centroid = CalculateCentroid(points);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(centroid, vertexSize * 1.5f);
            Gizmos.DrawRay(centroid, Vector3.up * 0.3f);

#if UNITY_EDITOR
            if (showDetailedInfo)
            {
                UnityEditor.Handles.color = infoTextColor;
                UnityEditor.Handles.Label(centroid + Vector3.up * 0.4f, "Centroid");
            }
#endif
        }

        private Vector3 CalculateCentroid(Vector3[] points)
        {
            Vector3 centroid = Vector3.zero;
            foreach (var point in points)
            {
                centroid += point;
            }
            return centroid / points.Length;
        }

        private void DrawDetailedInfo(PolygonCollider2D polygonCollider, int pathIndex, Vector3[] points)
        {
#if UNITY_EDITOR
            Vector3 infoPosition = CalculateCentroid(points) + Vector3.up * 0.6f;
            
            string info = $"Path {pathIndex}\n";
            info += $"Vertices: {points.Length}\n";
            info += $"Area: {CalculatePolygonArea(points):F2}\n";
            info += $"Perimeter: {CalculatePolygonPerimeter(points):F2}";
            
            UnityEditor.Handles.color = infoTextColor;
            UnityEditor.Handles.Label(infoPosition, info);
#endif
        }

        private float CalculatePolygonArea(Vector3[] points)
        {
            float area = 0f;
            int n = points.Length;

            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                area += points[i].x * points[j].y;
                area -= points[j].x * points[i].y;
            }

            return Mathf.Abs(area) * 0.5f;
        }

        private float CalculatePolygonPerimeter(Vector3[] points)
        {
            float perimeter = 0f;

            for (int i = 0; i < points.Length; i++)
            {
                int nextIndex = (i + 1) % points.Length;
                perimeter += Vector3.Distance(points[i], points[nextIndex]);
            }

            return perimeter;
        }

        #region Public Methods

        /// <summary>
        /// Força a atualização do cache de colliders
        /// </summary>
        [ContextMenu("🔄 Refresh Polygon Cache")]
        public void RefreshPolygonCache()
        {
            polygonColliders = null;
#if UNITY_EDITOR
            UnityEditor.SceneView.RepaintAll();
#endif
        }

        /// <summary>
        /// Mostra estatísticas detalhadas de todos os polígonos
        /// </summary>
        [ContextMenu("📊 Show Polygon Statistics")]
        public void ShowPolygonStatistics()
        {
            var colliders = GetComponentsInChildren<PolygonCollider2D>();

            Debug.Log($"=== POLYGON STATISTICS ===");
            Debug.Log($"GameObject: {gameObject.name}");
            Debug.Log($"Total PolygonCollider2D: {colliders.Length}");

            foreach (var collider in colliders)
            {
                Debug.Log($"\n🔺 {collider.name}:");
                Debug.Log($"  Paths: {collider.pathCount}");
                Debug.Log($"  Total Points: {collider.GetTotalPointCount()}");
                Debug.Log($"  Enabled: {collider.enabled}");
                Debug.Log($"  Is Trigger: {collider.isTrigger}");

                for (int i = 0; i < collider.pathCount; i++)
                {
                    var path = collider.GetPath(i);
                    var worldPoints = ConvertToWorldPoints(collider, path);
                    Debug.Log($"  Path {i}: {path.Length} points, Area: {CalculatePolygonArea(worldPoints):F2}");
                }
            }
            Debug.Log($"========================");
        }

        #endregion
    }
}