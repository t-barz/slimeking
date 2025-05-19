using UnityEngine;

/// <summary>
/// Permite ancorar um objeto em diferentes posições da tela com offset configurável.
/// </summary>
public class AnchorOnScreen : MonoBehaviour
{
    [System.Serializable]
    public enum ScreenAnchor
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    [Header("Configurações de Ancoragem")]
    [Tooltip("Posição na tela onde o objeto será ancorado")]
    [SerializeField] private ScreenAnchor anchor = ScreenAnchor.TopLeft;
    
    [Tooltip("Distância adicional da posição ancorada (X,Y)")]
    [SerializeField] private Vector2 offset = Vector2.zero;

    [Header("Configurações Avançadas")]
    [Tooltip("Atualiza a posição continuamente")]
    [SerializeField] private bool updateContinuously = false;

    private Camera mainCamera;
    private Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Camera principal não encontrada!");
            enabled = false;
            return;
        }

        // Calcula as dimensões do objeto
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            objectWidth = spriteRenderer.bounds.size.x;
            objectHeight = spriteRenderer.bounds.size.y;
        }

        UpdatePosition();
    }

    private void Update()
    {
        if (updateContinuously)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        Vector3 targetPosition = GetAnchorPosition();
        transform.position = targetPosition;
    }

    private Vector3 GetAnchorPosition()
    {
        float x = 0, y = 0;
        
        // Calcula posição X baseada na âncora
        switch (anchor)
        {
            case ScreenAnchor.TopLeft:
            case ScreenAnchor.MiddleLeft:
            case ScreenAnchor.BottomLeft:
                x = -screenBounds.x + (objectWidth / 2);
                break;
            
            case ScreenAnchor.TopRight:
            case ScreenAnchor.MiddleRight:
            case ScreenAnchor.BottomRight:
                x = screenBounds.x - (objectWidth / 2);
                break;
            
            case ScreenAnchor.TopCenter:
            case ScreenAnchor.MiddleCenter:
            case ScreenAnchor.BottomCenter:
                x = 0;
                break;
        }

        // Calcula posição Y baseada na âncora
        switch (anchor)
        {
            case ScreenAnchor.TopLeft:
            case ScreenAnchor.TopCenter:
            case ScreenAnchor.TopRight:
                y = screenBounds.y - (objectHeight / 2);
                break;
            
            case ScreenAnchor.BottomLeft:
            case ScreenAnchor.BottomCenter:
            case ScreenAnchor.BottomRight:
                y = -screenBounds.y + (objectHeight / 2);
                break;
            
            case ScreenAnchor.MiddleLeft:
            case ScreenAnchor.MiddleCenter:
            case ScreenAnchor.MiddleRight:
                y = 0;
                break;
        }

        // Aplica o offset
        x += offset.x;
        y += offset.y;

        return new Vector3(x, y, transform.position.z);
    }
}