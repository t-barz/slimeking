using UnityEngine;

namespace TheSlimeKing.Systems.Visual
{
    /// <summary>
    /// Randomizes the SpriteRenderer color on instantiation if current color is white (255,255,255).
    /// Works both in Play Mode and Edit Mode.
    /// 
    /// Supports multiple color variation techniques:
    /// - Hue Shift: Maintains saturation and brightness, shifts hue
    /// - Full Random: Completely random RGB values
    /// - Palette: Choose from predefined color palette
    /// - Tint: Apply random tint to base color
    /// - Brightness: Vary only brightness
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteColorRandomizer : MonoBehaviour
    {
        #region Settings / Configuration
        [Header("Configuration")]
        [SerializeField] private ColorVariationTechnique technique = ColorVariationTechnique.HueShift;
        
        [Header("Hue Shift Settings")]
        [SerializeField] [Range(0f, 1f)] private float minHue = 0f;
        [SerializeField] [Range(0f, 1f)] private float maxHue = 1f;
        [SerializeField] [Range(0f, 1f)] private float saturation = 0.8f;
        [SerializeField] [Range(0f, 1f)] private float brightness = 0.8f;
        
        [Header("Full Random Settings")]
        [SerializeField] [Range(0f, 1f)] private float minR = 0f;
        [SerializeField] [Range(0f, 1f)] private float maxR = 1f;
        [SerializeField] [Range(0f, 1f)] private float minG = 0f;
        [SerializeField] [Range(0f, 1f)] private float maxG = 1f;
        [SerializeField] [Range(0f, 1f)] private float minB = 0f;
        [SerializeField] [Range(0f, 1f)] private float maxB = 1f;
        
        [Header("Palette Settings")]
        [SerializeField] private Color[] colorPalette = new Color[]
        {
            new Color(1f, 0.5f, 0.5f),  // Light Red
            new Color(0.5f, 1f, 0.5f),  // Light Green
            new Color(0.5f, 0.5f, 1f),  // Light Blue
            new Color(1f, 1f, 0.5f),    // Light Yellow
            new Color(1f, 0.5f, 1f),    // Light Magenta
            new Color(0.5f, 1f, 1f)     // Light Cyan
        };
        
        [Header("Tint Settings")]
        [SerializeField] private Color tintColor = Color.red;
        [SerializeField] [Range(0f, 1f)] private float tintStrength = 0.5f;
        
        [Header("Brightness Settings")]
        [SerializeField] [Range(0.3f, 1f)] private float minBrightness = 0.5f;
        [SerializeField] [Range(0.3f, 1f)] private float maxBrightness = 1f;
        
        [Header("Options")]
        [SerializeField] private bool randomizeOnAwake = true;
        [SerializeField] private bool preserveAlpha = true;
        #endregion

        #region References
        private SpriteRenderer spriteRenderer;
        #endregion

        #region Debug
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private bool showOriginalColor = false;
        private Color originalColor;
        #endregion

        #region Private Variables
        private bool hasBeenRandomized = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (!Application.isPlaying) return;
            
            Initialize();
            
            if (randomizeOnAwake && !hasBeenRandomized)
            {
                RandomizeColor();
            }
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Initialize();
                
                // Randomize in Edit Mode when component is added or enabled
                if (!hasBeenRandomized && IsWhiteColor())
                {
                    RandomizeColor();
                }
            }
            #endif
        }

        private void Reset()
        {
            Initialize();
            hasBeenRandomized = false;
        }
        #endregion

        #region Initialization
        private void Initialize()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            if (spriteRenderer != null && !hasBeenRandomized)
            {
                originalColor = spriteRenderer.color;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Manually trigger color randomization
        /// </summary>
        public void RandomizeColor()
        {
            if (spriteRenderer == null)
            {
                Initialize();
            }

            if (spriteRenderer == null)
            {
                Log("SpriteRenderer not found!");
                return;
            }

            // Only randomize if current color is white (255,255,255)
            if (!IsWhiteColor())
            {
                Log($"Color is not white. Current: {spriteRenderer.color}");
                return;
            }

            Color newColor = GenerateColorByTechnique();
            
            if (preserveAlpha)
            {
                newColor.a = spriteRenderer.color.a;
            }

            spriteRenderer.color = newColor;
            hasBeenRandomized = true;

            Log($"Color randomized using {technique}: {newColor}");
        }

        /// <summary>
        /// Reset to original white color
        /// </summary>
        public void ResetColor()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = showOriginalColor ? originalColor : Color.white;
                hasBeenRandomized = false;
                Log("Color reset to white");
            }
        }

        /// <summary>
        /// Change the variation technique at runtime
        /// </summary>
        public void SetTechnique(ColorVariationTechnique newTechnique)
        {
            technique = newTechnique;
        }
        #endregion

        #region Private Methods
        private bool IsWhiteColor()
        {
            if (spriteRenderer == null) return false;
            
            Color c = spriteRenderer.color;
            // Check if RGB is approximately (1, 1, 1) or (255, 255, 255)
            return Mathf.Approximately(c.r, 1f) && 
                   Mathf.Approximately(c.g, 1f) && 
                   Mathf.Approximately(c.b, 1f);
        }

        private Color GenerateColorByTechnique()
        {
            switch (technique)
            {
                case ColorVariationTechnique.HueShift:
                    return GenerateHueShiftColor();
                
                case ColorVariationTechnique.FullRandom:
                    return GenerateFullRandomColor();
                
                case ColorVariationTechnique.Palette:
                    return GeneratePaletteColor();
                
                case ColorVariationTechnique.Tint:
                    return GenerateTintColor();
                
                case ColorVariationTechnique.Brightness:
                    return GenerateBrightnessColor();
                
                default:
                    return Color.white;
            }
        }

        private Color GenerateHueShiftColor()
        {
            float randomHue = Random.Range(minHue, maxHue);
            return Color.HSVToRGB(randomHue, saturation, brightness);
        }

        private Color GenerateFullRandomColor()
        {
            return new Color(
                Random.Range(minR, maxR),
                Random.Range(minG, maxG),
                Random.Range(minB, maxB),
                1f
            );
        }

        private Color GeneratePaletteColor()
        {
            if (colorPalette == null || colorPalette.Length == 0)
            {
                Log("Color palette is empty! Using white.");
                return Color.white;
            }

            int randomIndex = Random.Range(0, colorPalette.Length);
            return colorPalette[randomIndex];
        }

        private Color GenerateTintColor()
        {
            return Color.Lerp(Color.white, tintColor, tintStrength);
        }

        private Color GenerateBrightnessColor()
        {
            float randomBrightness = Random.Range(minBrightness, maxBrightness);
            return new Color(randomBrightness, randomBrightness, randomBrightness, 1f);
        }
        #endregion

        #region Utilities
        private void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[SpriteColorRandomizer] {gameObject.name}: {message}", this);
            }
        }
        #endregion
    }

    /// <summary>
    /// Available color variation techniques
    /// </summary>
    public enum ColorVariationTechnique
    {
        HueShift,      // Shift hue while maintaining saturation/brightness
        FullRandom,    // Completely random RGB values
        Palette,       // Choose from predefined palette
        Tint,          // Apply tint to white
        Brightness     // Vary only brightness (grayscale)
    }
}
