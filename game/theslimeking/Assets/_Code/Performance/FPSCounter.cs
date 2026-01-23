using UnityEngine;
using TMPro;

namespace SlimeKing.Performance
{
    /// <summary>
    /// Contador de FPS simples para monitoramento de performance.
    /// Exibe FPS atual, médio, mínimo e máximo na tela.
    /// </summary>
    public class FPSCounter : MonoBehaviour
    {
        [Header("Display Settings")]
        [SerializeField] private bool showFPS = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.F3;
        [SerializeField] private TextMeshProUGUI fpsText;
        
        [Header("Update Settings")]
        [SerializeField] private float updateInterval = 0.5f;
        
        [Header("Color Thresholds")]
        [SerializeField] private float goodFPS = 60f;
        [SerializeField] private float okayFPS = 30f;
        [SerializeField] private Color goodColor = Color.green;
        [SerializeField] private Color okayColor = Color.yellow;
        [SerializeField] private Color badColor = Color.red;
        
        private float deltaTime = 0f;
        private float fps = 0f;
        private float minFPS = float.MaxValue;
        private float maxFPS = 0f;
        private float avgFPS = 0f;
        private int frameCount = 0;
        private float fpsSum = 0f;
        private float nextUpdate = 0f;
        
        private void Start()
        {
            if (fpsText == null)
            {
                CreateFPSText();
            }
            
            UpdateDisplay();
        }
        
        private void Update()
        {
            // Toggle display
            if (Input.GetKeyDown(toggleKey))
            {
                showFPS = !showFPS;
                UpdateDisplay();
            }
            
            if (!showFPS) return;
            
            // Calculate FPS
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            fps = 1.0f / deltaTime;
            
            // Track stats
            frameCount++;
            fpsSum += fps;
            
            if (fps < minFPS) minFPS = fps;
            if (fps > maxFPS) maxFPS = fps;
            
            avgFPS = fpsSum / frameCount;
            
            // Update display at interval
            if (Time.time >= nextUpdate)
            {
                nextUpdate = Time.time + updateInterval;
                UpdateDisplay();
            }
        }
        
        private void UpdateDisplay()
        {
            if (fpsText == null) return;
            
            fpsText.gameObject.SetActive(showFPS);
            
            if (!showFPS) return;
            
            // Format text
            fpsText.text = $"FPS: {fps:F0}\n" +
                          $"Avg: {avgFPS:F0} | Min: {minFPS:F0} | Max: {maxFPS:F0}\n" +
                          $"Frame: {frameCount}";
            
            // Color based on performance
            if (fps >= goodFPS)
                fpsText.color = goodColor;
            else if (fps >= okayFPS)
                fpsText.color = okayColor;
            else
                fpsText.color = badColor;
        }
        
        private void CreateFPSText()
        {
            // Create canvas if needed
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("FPS Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            
            // Create text object
            GameObject textObj = new GameObject("FPS Counter");
            textObj.transform.SetParent(canvas.transform, false);
            
            fpsText = textObj.AddComponent<TextMeshProUGUI>();
            fpsText.fontSize = 24;
            fpsText.alignment = TextAlignmentOptions.TopRight;
            fpsText.color = Color.white;
            
            // Position at top-right
            RectTransform rectTransform = textObj.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(-10, -10);
            rectTransform.sizeDelta = new Vector2(200, 100);
        }
        
        /// <summary>
        /// Reseta as estatísticas de FPS
        /// </summary>
        public void ResetStats()
        {
            minFPS = float.MaxValue;
            maxFPS = 0f;
            avgFPS = 0f;
            frameCount = 0;
            fpsSum = 0f;
        }
        
        /// <summary>
        /// Retorna o FPS atual
        /// </summary>
        public float GetCurrentFPS() => fps;
        
        /// <summary>
        /// Retorna o FPS médio
        /// </summary>
        public float GetAverageFPS() => avgFPS;
        
        /// <summary>
        /// Retorna o FPS mínimo
        /// </summary>
        public float GetMinFPS() => minFPS;
        
        /// <summary>
        /// Retorna o FPS máximo
        /// </summary>
        public float GetMaxFPS() => maxFPS;
    }
}
