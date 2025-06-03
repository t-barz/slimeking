using UnityEngine;

namespace SlimeKing.Utils
{
    /// <summary>
    /// Applies water effect to sprites or tiles with a specific color.
    /// Attach this to GameObjects with renderers that should have water effect applied.
    /// </summary>
    public class WaterEffectController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Renderer targetRenderer;
        
        [Header("Water Settings")]
        [SerializeField] private Color detectionColor = new Color(0, 0, 1, 1);
        [Range(0, 1)]
        [SerializeField] private float colorTolerance = 0.2f;
        [SerializeField] private Color waterColor = new Color(0.2f, 0.5f, 0.8f, 0.7f);
        [Range(0, 1)]
        [SerializeField] private float transparency = 0.7f;
        
        [Header("Wave Settings")]
        [Range(0, 10)]
        [SerializeField] private float waveSpeed = 2f;
        [Range(0, 50)]
        [SerializeField] private float waveFrequency = 10f;
        [Range(0, 0.1f)]
        [SerializeField] private float waveAmplitude = 0.01f;
        
        [Header("Reflection Settings")]
        [Range(0, 1)]
        [SerializeField] private float reflectionIntensity = 0.5f;
        [Range(0, 1)]
        [SerializeField] private float glossiness = 0.7f;
        [Range(0, 10)]
        [SerializeField] private float rimPower = 3f;
        [SerializeField] private Color rimColor = new Color(1, 1, 1, 0.5f);
        
        private Material waterMaterial;
        private static readonly int DetectionColorProperty = Shader.PropertyToID("_DetectionColor");
        private static readonly int ColorToleranceProperty = Shader.PropertyToID("_ColorTolerance");
        private static readonly int WaterColorProperty = Shader.PropertyToID("_WaterColor");
        private static readonly int WaterTransparencyProperty = Shader.PropertyToID("_WaterTransparency");
        private static readonly int WaveSpeedProperty = Shader.PropertyToID("_WaveSpeed");
        private static readonly int WaveFrequencyProperty = Shader.PropertyToID("_WaveFrequency");
        private static readonly int WaveAmplitudeProperty = Shader.PropertyToID("_WaveAmplitude");
        private static readonly int ReflectionIntensityProperty = Shader.PropertyToID("_ReflectionIntensity");
        private static readonly int GlossinessProperty = Shader.PropertyToID("_Glossiness");
        private static readonly int RimPowerProperty = Shader.PropertyToID("_RimPower");
        private static readonly int RimColorProperty = Shader.PropertyToID("_RimColor");
        
        private void Awake()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponent<Renderer>();
            
            if (targetRenderer == null)
            {
                Debug.LogError("No renderer found for water effect on " + gameObject.name);
                enabled = false;
                return;
            }
            
            // Create instance of water material
            waterMaterial = new Material(Shader.Find("SlimeKing/2D Water Effect"));
            targetRenderer.material = waterMaterial;
            
            // Apply initial settings
            UpdateShaderProperties();
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying && waterMaterial != null)
                UpdateShaderProperties();
        }
        
        /// <summary>
        /// Updates all shader properties based on the current settings
        /// </summary>
        public void UpdateShaderProperties()
        {
            if (waterMaterial == null)
                return;
                
            waterMaterial.SetColor(DetectionColorProperty, detectionColor);
            waterMaterial.SetFloat(ColorToleranceProperty, colorTolerance);
            waterMaterial.SetColor(WaterColorProperty, waterColor);
            waterMaterial.SetFloat(WaterTransparencyProperty, transparency);
            waterMaterial.SetFloat(WaveSpeedProperty, waveSpeed);
            waterMaterial.SetFloat(WaveFrequencyProperty, waveFrequency);
            waterMaterial.SetFloat(WaveAmplitudeProperty, waveAmplitude);
            waterMaterial.SetFloat(ReflectionIntensityProperty, reflectionIntensity);
            waterMaterial.SetFloat(GlossinessProperty, glossiness);
            waterMaterial.SetFloat(RimPowerProperty, rimPower);
            waterMaterial.SetColor(RimColorProperty, rimColor);
        }
        
        /// <summary>
        /// Change the wave animation speed at runtime
        /// </summary>
        public void SetWaveSpeed(float speed)
        {
            waveSpeed = speed;
            if (waterMaterial != null)
                waterMaterial.SetFloat(WaveSpeedProperty, waveSpeed);
        }
        
        /// <summary>
        /// Change water transparency at runtime
        /// </summary>
        public void SetTransparency(float alpha)
        {
            transparency = Mathf.Clamp01(alpha);
            if (waterMaterial != null)
                waterMaterial.SetFloat(WaterTransparencyProperty, transparency);
        }
        
        /// <summary>
        /// Change water color at runtime
        /// </summary>
        public void SetWaterColor(Color color)
        {
            waterColor = color;
            if (waterMaterial != null)
                waterMaterial.SetColor(WaterColorProperty, waterColor);
        }
    }
}
