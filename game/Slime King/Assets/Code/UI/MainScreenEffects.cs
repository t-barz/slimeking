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

    private float currentFadeTime;
    private bool startFade;

    private void Start()
    {
        // Configura estado inicial
        logoImage.color = new Color(1f, 1f, 1f, 0f);
        startFade = false;

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
            
            // Fade do logo
            logoImage.color = new Color(1f, 1f, 1f, alpha);
        }

        // Move o background
        MoveBackground();
    }

    private void MoveBackground()
    {
        // ...existing code for MoveBackground method...
    }
}