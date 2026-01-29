using UnityEngine;

namespace TheSlimeKing.Systems.Visual
{
    /// <summary>
    /// Randomizes the Transform scale on instantiation.
    /// Works both in Play Mode and Edit Mode.
    /// 
    /// Supports multiple scale variation techniques:
    /// - Uniform: Same scale on all axes (maintains proportions)
    /// - Non-Uniform: Independent scale per axis
    /// - Proportional: Scale based on percentage of original
    /// - Fixed Range: Random value between min/max
    /// - Stepped: Discrete scale values (e.g., 0.5x, 1x, 1.5x, 2x)
    /// </summary>
    [ExecuteAlways]
    public class TransformScaleRandomizer : MonoBehaviour
    {
        #region Settings / Configuration
        [Header("Configuration")]
        [SerializeField] private ScaleVariationTechnique technique = ScaleVariationTechnique.Uniform;
        
        [Header("Uniform Scale Settings")]
        [SerializeField] [Range(0.1f, 5f)] private float minUniformScale = 0.8f;
        [SerializeField] [Range(0.1f, 5f)] private float maxUniformScale = 1.2f;
        
        [Header("Non-Uniform Scale Settings")]
        [SerializeField] [Range(0.1f, 5f)] private float minScaleX = 0.8f;
        [SerializeField] [Range(0.1f, 5f)] private float maxScaleX = 1.2f;
        [SerializeField] [Range(0.1f, 5f)] private float minScaleY = 0.8f;
        [SerializeField] [Range(0.1f, 5f)] private float maxScaleY = 1.2f;
        [SerializeField] [Range(0.1f, 5f)] private float minScaleZ = 1f;
        [SerializeField] [Range(0.1f, 5f)] private float maxScaleZ = 1f;
        
        [Header("Proportional Scale Settings")]
        [SerializeField] [Range(0.1f, 3f)] private float minPercentage = 0.8f;
        [SerializeField] [Range(0.1f, 3f)] private float maxPercentage = 1.2f;
        [SerializeField] private bool maintainProportions = true;
        
        [Header("Stepped Scale Settings")]
        [SerializeField] private float[] scaleSteps = new float[] { 0.5f, 0.75f, 1f, 1.25f, 1.5f, 2f };
        
        [Header("Options")]
        [SerializeField] private bool randomizeOnAwake = true;
        [SerializeField] private bool useLocalScale = true;
        [SerializeField] private bool clampToPositive = true;
        #endregion

        #region Debug
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private bool showOriginalScale = false;
        [SerializeField] private bool showGizmos = false;
        private Vector3 originalScale;
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
                RandomizeScale();
            }
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Initialize();
                
                // Randomize in Edit Mode when component is added or enabled
                if (!hasBeenRandomized)
                {
                    RandomizeScale();
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
            if (!hasBeenRandomized)
            {
                originalScale = useLocalScale ? transform.localScale : transform.lossyScale;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Manually trigger scale randomization
        /// </summary>
        public void RandomizeScale()
        {
            Vector3 newScale = GenerateScaleByTechnique();
            
            if (clampToPositive)
            {
                newScale.x = Mathf.Max(0.01f, newScale.x);
                newScale.y = Mathf.Max(0.01f, newScale.y);
                newScale.z = Mathf.Max(0.01f, newScale.z);
            }

            if (useLocalScale)
            {
                transform.localScale = newScale;
            }
            else
            {
                // For world scale, we need to adjust based on parent scale
                if (transform.parent != null)
                {
                    Vector3 parentScale = transform.parent.lossyScale;
                    transform.localScale = new Vector3(
                        newScale.x / parentScale.x,
                        newScale.y / parentScale.y,
                        newScale.z / parentScale.z
                    );
                }
                else
                {
                    transform.localScale = newScale;
                }
            }

            hasBeenRandomized = true;
            Log($"Scale randomized using {technique}: {newScale}");
        }

        /// <summary>
        /// Reset to original scale
        /// </summary>
        public void ResetScale()
        {
            Vector3 resetScale = showOriginalScale ? originalScale : Vector3.one;
            
            if (useLocalScale)
            {
                transform.localScale = resetScale;
            }
            else
            {
                if (transform.parent != null)
                {
                    Vector3 parentScale = transform.parent.lossyScale;
                    transform.localScale = new Vector3(
                        resetScale.x / parentScale.x,
                        resetScale.y / parentScale.y,
                        resetScale.z / parentScale.z
                    );
                }
                else
                {
                    transform.localScale = resetScale;
                }
            }
            
            hasBeenRandomized = false;
            Log("Scale reset to original");
        }

        /// <summary>
        /// Change the variation technique at runtime
        /// </summary>
        public void SetTechnique(ScaleVariationTechnique newTechnique)
        {
            technique = newTechnique;
        }

        /// <summary>
        /// Set a specific uniform scale
        /// </summary>
        public void SetUniformScale(float scale)
        {
            Vector3 newScale = Vector3.one * scale;
            
            if (useLocalScale)
            {
                transform.localScale = newScale;
            }
            else
            {
                if (transform.parent != null)
                {
                    Vector3 parentScale = transform.parent.lossyScale;
                    transform.localScale = new Vector3(
                        newScale.x / parentScale.x,
                        newScale.y / parentScale.y,
                        newScale.z / parentScale.z
                    );
                }
                else
                {
                    transform.localScale = newScale;
                }
            }
        }
        #endregion

        #region Private Methods
        private Vector3 GenerateScaleByTechnique()
        {
            switch (technique)
            {
                case ScaleVariationTechnique.Uniform:
                    return GenerateUniformScale();
                
                case ScaleVariationTechnique.NonUniform:
                    return GenerateNonUniformScale();
                
                case ScaleVariationTechnique.Proportional:
                    return GenerateProportionalScale();
                
                case ScaleVariationTechnique.Stepped:
                    return GenerateSteppedScale();
                
                default:
                    return Vector3.one;
            }
        }

        private Vector3 GenerateUniformScale()
        {
            float randomScale = Random.Range(minUniformScale, maxUniformScale);
            return Vector3.one * randomScale;
        }

        private Vector3 GenerateNonUniformScale()
        {
            return new Vector3(
                Random.Range(minScaleX, maxScaleX),
                Random.Range(minScaleY, maxScaleY),
                Random.Range(minScaleZ, maxScaleZ)
            );
        }

        private Vector3 GenerateProportionalScale()
        {
            float percentage = Random.Range(minPercentage, maxPercentage);
            
            if (maintainProportions)
            {
                return originalScale * percentage;
            }
            else
            {
                return new Vector3(
                    originalScale.x * Random.Range(minPercentage, maxPercentage),
                    originalScale.y * Random.Range(minPercentage, maxPercentage),
                    originalScale.z * Random.Range(minPercentage, maxPercentage)
                );
            }
        }

        private Vector3 GenerateSteppedScale()
        {
            if (scaleSteps == null || scaleSteps.Length == 0)
            {
                Log("Scale steps array is empty! Using scale of 1.");
                return Vector3.one;
            }

            int randomIndex = Random.Range(0, scaleSteps.Length);
            float selectedStep = scaleSteps[randomIndex];
            
            return Vector3.one * selectedStep;
        }
        #endregion

        #region Utilities
        private void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[TransformScaleRandomizer] {gameObject.name}: {message}", this);
            }
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            // Draw original scale bounds in yellow
            Gizmos.color = Color.yellow;
            Vector3 size = originalScale;
            Gizmos.DrawWireCube(transform.position, size);

            // Draw current scale bounds in green
            Gizmos.color = Color.green;
            Vector3 currentSize = useLocalScale ? transform.localScale : transform.lossyScale;
            Gizmos.DrawWireCube(transform.position, currentSize);
        }
        #endregion
    }

    /// <summary>
    /// Available scale variation techniques
    /// </summary>
    public enum ScaleVariationTechnique
    {
        Uniform,        // Same scale on all axes
        NonUniform,     // Independent scale per axis
        Proportional,   // Scale based on percentage of original
        Stepped         // Discrete scale values
    }
}
