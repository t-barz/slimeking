using UnityEngine;

/// <summary>
/// Utility class for drawing 2D debug gizmos with configurable properties
/// </summary>
public class GizmosUtils : MonoBehaviour
{
    public enum GizmoShape
    {
        Circle,         // 2D circle shape
        Square,         // 2D square shape
        WireCircle,    // 2D wire circle shape
        WireSquare     // 2D wire square shape
    }

    [Header("Gizmo Settings")]
    [SerializeField] private GizmoShape shape = GizmoShape.Circle;
    [SerializeField] private Color gizmoColor = Color.yellow;
    [SerializeField] private Vector2 gizmoSize = Vector2.one;
    [SerializeField] private bool showInGame = true;

    private void OnDrawGizmos()
    {
        if (!showInGame) return;
        
        // Store original color and matrix
        Color originalColor = Gizmos.color;
        Matrix4x4 originalMatrix = Gizmos.matrix;

        // Apply settings
        Gizmos.color = gizmoColor;
        // Use only x and y components for 2D
        Vector3 position = new Vector3(transform.position.x, transform.position.y, 0);
        Vector3 scale = new Vector3(gizmoSize.x, gizmoSize.y, 1);
        Gizmos.matrix = Matrix4x4.TRS(position, transform.rotation, scale);

        // Draw selected shape
        switch (shape)
        {
            case GizmoShape.Circle:
                DrawCircle(Vector3.zero, 0.5f, true);
                break;
            case GizmoShape.Square:
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                break;
            case GizmoShape.WireCircle:
                DrawCircle(Vector3.zero, 0.5f, false);
                break;
            case GizmoShape.WireSquare:
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                break;
        }

        // Restore original settings
        Gizmos.color = originalColor;
        Gizmos.matrix = originalMatrix;
    }

    /// <summary>
    /// Draws a circle using line segments
    /// </summary>
    private void DrawCircle(Vector3 center, float radius, bool filled)
    {
        const int segments = 32;
        float angleStep = 360f / segments;

        // Draw the outline
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius, 0);
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2) * radius, Mathf.Sin(angle2) * radius, 0);

            Gizmos.DrawLine(point1, point2);

            // If filled, draw lines from center to create triangles
            if (filled && i % 2 == 0)
            {
                Gizmos.DrawLine(center, point1);
                Gizmos.DrawLine(center, point2);
            }
        }
    }
}