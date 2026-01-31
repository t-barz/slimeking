using UnityEngine;

namespace TheSlimeKing.Systems.Debug
{
    /// <summary>
    /// Componente auxiliar para exibir gizmos circulares 2D no Editor.
    /// Útil para visualizar ranges de detecção, áreas de ataque, e outros raios.
    /// 
    /// Funciona tanto em modo Play quanto em modo Editor.
    /// </summary>
    public class CircleGizmo2D : MonoBehaviour
    {
        #region Settings / Configuration

        [Header("Circle Settings")]
        [SerializeField] private float radius = 1f;
        [SerializeField] private Color color = Color.green;
        [SerializeField] private bool filled = false;

        [Header("Display Options")]
        [SerializeField] private bool showInPlayMode = true;
        [SerializeField] private bool showInEditMode = true;
        [SerializeField] private bool showLabel = false;
        [SerializeField] private string labelText = "Range";

        [Header("Advanced")]
        [SerializeField] private int segments = 32;
        [SerializeField] private Vector2 offset = Vector2.zero;

        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            // Verifica se deve exibir no modo atual
            if (!Application.isPlaying && !showInEditMode)
                return;

            if (Application.isPlaying && !showInPlayMode)
                return;

            DrawCircle();
        }

        private void OnDrawGizmosSelected()
        {
            // Sempre desenha quando selecionado (com cor mais brilhante)
            Color originalColor = color;
            color = new Color(color.r * 1.5f, color.g * 1.5f, color.b * 1.5f, color.a);
            DrawCircle();
            color = originalColor;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Desenha o círculo usando Gizmos.
        /// </summary>
        private void DrawCircle()
        {
            Vector3 center = transform.position + (Vector3)offset;

            // Define a cor do gizmo
            Gizmos.color = color;

            if (filled)
            {
                // Desenha círculo preenchido usando múltiplos triângulos
                DrawFilledCircle(center, radius, segments);
            }
            else
            {
                // Desenha círculo vazado usando linhas
                DrawWireCircle(center, radius, segments);
            }

            // Desenha label se habilitado
            if (showLabel)
            {
                DrawLabel(center);
            }
        }

        /// <summary>
        /// Desenha um círculo vazado (wireframe).
        /// </summary>
        private void DrawWireCircle(Vector3 center, float radius, int segments)
        {
            float angleStep = 360f / segments;
            Vector3 previousPoint = center + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 currentPoint = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0
                );

                Gizmos.DrawLine(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }
        }

        /// <summary>
        /// Desenha um círculo preenchido usando mesh.
        /// </summary>
        private void DrawFilledCircle(Vector3 center, float radius, int segments)
        {
            float angleStep = 360f / segments;

            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * angleStep * Mathf.Deg2Rad;
                float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

                Vector3 point1 = center + new Vector3(
                    Mathf.Cos(angle1) * radius,
                    Mathf.Sin(angle1) * radius,
                    0
                );

                Vector3 point2 = center + new Vector3(
                    Mathf.Cos(angle2) * radius,
                    Mathf.Sin(angle2) * radius,
                    0
                );

                // Desenha triângulo do centro para os pontos da borda
                Gizmos.DrawLine(center, point1);
                Gizmos.DrawLine(point1, point2);
                Gizmos.DrawLine(point2, center);
            }
        }

        /// <summary>
        /// Desenha label com informações do círculo.
        /// </summary>
        private void DrawLabel(Vector3 center)
        {
#if UNITY_EDITOR
            Vector3 labelPosition = center + Vector3.up * (radius + 0.3f);
            string label = $"{labelText}\nRadius: {radius:F2}";
            
            GUIStyle style = new GUIStyle();
            style.normal.textColor = color;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 10;
            
            UnityEditor.Handles.Label(labelPosition, label, style);
#endif
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Define o raio do círculo em runtime.
        /// </summary>
        public void SetRadius(float newRadius)
        {
            radius = Mathf.Max(0f, newRadius);
        }

        /// <summary>
        /// Define a cor do círculo em runtime.
        /// </summary>
        public void SetColor(Color newColor)
        {
            color = newColor;
        }

        /// <summary>
        /// Define se o círculo deve ser preenchido.
        /// </summary>
        public void SetFilled(bool isFilled)
        {
            filled = isFilled;
        }

        /// <summary>
        /// Define o offset do círculo em relação ao transform.
        /// </summary>
        public void SetOffset(Vector2 newOffset)
        {
            offset = newOffset;
        }

        /// <summary>
        /// Define o texto da label.
        /// </summary>
        public void SetLabelText(string text)
        {
            labelText = text;
            showLabel = !string.IsNullOrEmpty(text);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Valida os valores no Inspector.
        /// </summary>
        private void OnValidate()
        {
            // Garante valores mínimos válidos
            radius = Mathf.Max(0.01f, radius);
            segments = Mathf.Max(3, segments);
            
            // Garante alpha válido
            color.a = Mathf.Clamp01(color.a);
        }

        #endregion
    }
}
