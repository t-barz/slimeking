using UnityEngine;
using UnityEngine.UI;

public class MainScreenEffects : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image logoImage;

    [Header("Configurações do Logo")]
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeInDelay = 0.5f;

    [Header("Configurações do Background")]
    [SerializeField] private float moveSpeedX = 0.5f;
    [SerializeField] private float moveSpeedY = 0.5f;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private Vector2 maxOffset = new Vector2(1f, 1f);

    private Vector3 initialBackgroundPosition;
    private float currentFadeTime;
    private float currentMoveTime;
    private bool startFade;
    private bool isMoving = true;

    private void Start()
    {
        // Configura estado inicial
        initialBackgroundPosition = backgroundImage.transform.position;
        logoImage.color = new Color(1f, 1f, 1f, 0f);
        startFade = false;
        currentMoveTime = 0f;

        // Inicia o fade após o delay
        Invoke(nameof(StartFade), fadeInDelay);
    }

    private void StartFade()
    {
        startFade = true;
    }

    private void Update()
    {
        // Controla o fade do logo
        if (startFade && currentFadeTime < fadeInDuration)
        {
            currentFadeTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, currentFadeTime / fadeInDuration);
            logoImage.color = new Color(1f, 1f, 1f, alpha);
        }

        // Move o background
        MoveBackground();
    }

    private void MoveBackground()
{
    if (!isMoving) return;

    // Atualiza o tempo de movimento
    currentMoveTime += Time.deltaTime;
    if (currentMoveTime >= moveDuration)
    {
        isMoving = false;
        return;
    }

    // Calcula o movimento baseado na velocidade e tempo
    float moveX = moveSpeedX * Time.deltaTime;
    float moveY = moveSpeedY * Time.deltaTime;

    // Obtém a posição atual
    Vector3 currentPosition = backgroundImage.transform.position;
    
    // Calcula o offset atual em relação à posição inicial
    float currentOffsetX = Mathf.Abs(currentPosition.x - initialBackgroundPosition.x);
    float currentOffsetY = Mathf.Abs(currentPosition.y - initialBackgroundPosition.y);

    // Verifica se pode mover em cada direção
    bool canMoveX = currentOffsetX < maxOffset.x;
    bool canMoveY = currentOffsetY < maxOffset.y;

    // Aplica o movimento apenas se não atingiu o offset máximo
    Vector3 newPosition = new Vector3(
        currentPosition.x + (canMoveX ? moveX : 0),
        currentPosition.y - (canMoveY ? moveY : 0),
        currentPosition.z
    );

    // Atualiza a posição
    backgroundImage.transform.position = newPosition;

    // Para o movimento se atingiu ambos os offsets
    if (!canMoveX && !canMoveY)
    {
        isMoving = false;
    }
}
}