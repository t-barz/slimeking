using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SlimeKing.Performance
{
    /// <summary>
    /// Advanced performance optimizer with multiple optimization strategies.
    /// Includes culling, component optimization, and frequency scaling.
    /// 
    /// Features:
    /// - Distance-based culling and optimization
    /// - Frustum culling for camera-based optimization
    /// - Component caching (Rigidbody2D, SpriteRenderer, Animator, etc.)
    /// - Update frequency scaling based on distance
    /// - Particle system and audio optimization
    /// </summary>
    public class BasicPerformanceOptimizer : MonoBehaviour
    {
        #region Fields
        
        [Header("Distance-Based Optimization")]
        [SerializeField] private bool enableCullingOptimization = true;
        [SerializeField] private bool enablePhysicsOptimization = true;
        [SerializeField] private bool enableRenderingOptimization = true;
        [SerializeField] private bool enableAudioOptimization = true;
        [SerializeField] private bool enableAnimationOptimization = true;
        [SerializeField] private bool enableParticleOptimization = true;
        [SerializeField] private bool enableColliderOptimization = true;
        
        [Header("Distance Thresholds")]
        [SerializeField] private float nearDistance = 10f;
        [SerializeField] private float mediumDistance = 20f;
        [SerializeField] private float farDistance = 30f;
        [SerializeField] private float cullingDistance = 40f;
        
        [Header("Frustum Culling")]
        [SerializeField] private bool enableFrustumCulling = true;
        [SerializeField] private float frustumPadding = 5f;
        
        [Header("Update Frequency")]
        [SerializeField] private float nearUpdateInterval = 0.016f; // ~60fps
        [SerializeField] private float mediumUpdateInterval = 0.033f; // ~30fps
        [SerializeField] private float farUpdateInterval = 0.1f; // ~10fps
        
        [Header("Debug")]
        [SerializeField] private bool showLogs = false;
        [SerializeField] private bool showGizmos = false;

        private Transform playerTransform;
        private Camera playerCamera;
        private List<OptimizedObject> optimizedObjects = new List<OptimizedObject>();
        private Coroutine optimizationCoroutine;
        
        // Cached bounds for frustum culling
        private Plane[] cameraFrustumPlanes = new Plane[6];
        
        // Performance tracking
        private int lastCulledCount = 0;
        private int lastPhysicsDisabledCount = 0;
        private int lastRenderingDisabledCount = 0;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Start()
        {
            InitializeOptimizer();
        }
        
        private void OnDestroy()
        {
            if (optimizationCoroutine != null)
            {
                StopCoroutine(optimizationCoroutine);
            }
        }
        
        private void OnDrawGizmos()
        {
            if (!showGizmos || playerTransform == null) return;
            
            Vector3 playerPos = playerTransform.position;
            
            // Draw distance circles
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerPos, nearDistance);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerPos, mediumDistance);
            
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(playerPos, farDistance);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerPos, cullingDistance);
        }
        
        #endregion
        
        #region Public Methods
        
        [ContextMenu("Force Optimization")]
        public void ForceOptimization()
        {
            if (playerTransform != null)
            {
                OptimizeAllObjects();
            }
        }
        
        [ContextMenu("Reset All Objects")]
        public void ResetAllObjects()
        {
            foreach (OptimizedObject obj in optimizedObjects)
            {
                if (obj.gameObject != null)
                {
                    RestoreOriginalState(obj);
                }
            }
        }
        
        #endregion
        
        #region Private Methods
        
        private void InitializeOptimizer()
        {
            // Find player
            var player = FindObjectOfType<SlimeKing.Gameplay.PlayerController>();
            if (player != null)
            {
                playerTransform = player.transform;
            }
            
            // Find camera
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
            }
            
            // Find and cache all destructible objects
            CacheOptimizableObjects();
            
            // Start optimization coroutine
            optimizationCoroutine = StartCoroutine(OptimizationLoop());
        }
        
        private void CacheOptimizableObjects()
        {
            optimizedObjects.Clear();
            
            // Find all destructible objects
            GameObject[] destructibles = GameObject.FindGameObjectsWithTag("Destructable");
            foreach (GameObject obj in destructibles)
            {
                optimizedObjects.Add(new OptimizedObject(obj));
            }
            
            // Find other optimizable objects (you can add more tags here)
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject obj in enemies)
            {
                optimizedObjects.Add(new OptimizedObject(obj));
            }
            
        }
        
        private IEnumerator OptimizationLoop()
        {
            while (true)
            {
                if (playerTransform != null)
                {
                    OptimizeAllObjects();
                }
                
                yield return new WaitForSeconds(nearUpdateInterval);
            }
        }
        
        private void OptimizeAllObjects()
        {
            Vector3 playerPos = playerTransform.position;
            
            // Update camera frustum planes for frustum culling
            if (enableFrustumCulling && playerCamera != null)
            {
                GeometryUtility.CalculateFrustumPlanes(playerCamera, cameraFrustumPlanes);
            }
            
            int culledCount = 0;
            int physicsDisabledCount = 0;
            int renderingDisabledCount = 0;
            
            for (int i = 0; i < optimizedObjects.Count; i++)
            {
                OptimizedObject obj = optimizedObjects[i];
                if (obj.gameObject == null) continue;
                
                // Skip if not time to update this object
                if (Time.time < obj.nextUpdateTime) continue;
                
                float distance = Vector3.Distance(playerPos, obj.transform.position);
                obj.lastDistance = distance;
                
                // Determine optimization level
                OptimizationLevel newLevel = GetOptimizationLevel(obj, distance);
                
                // Apply optimizations if level changed
                if (obj.currentLevel != newLevel)
                {
                    ApplyOptimizationLevel(obj, newLevel);
                    obj.currentLevel = newLevel;
                }
                
                // Set next update time based on distance
                obj.nextUpdateTime = Time.time + GetUpdateInterval(distance);
                
                // Count optimizations for debug
                if (newLevel == OptimizationLevel.Culled) culledCount++;
                if (obj.rigidbody2D != null && !obj.rigidbody2D.simulated) physicsDisabledCount++;
                if (obj.spriteRenderer != null && !obj.spriteRenderer.enabled) renderingDisabledCount++;
            }
            
            // Update performance tracking
            lastCulledCount = culledCount;
            lastPhysicsDisabledCount = physicsDisabledCount;
            lastRenderingDisabledCount = renderingDisabledCount;
        }
        
        private OptimizationLevel GetOptimizationLevel(OptimizedObject obj, float distance)
        {
            // Check frustum culling first
            if (enableFrustumCulling && playerCamera != null)
            {
                obj.isInFrustum = IsInCameraFrustum(obj);
                if (!obj.isInFrustum && distance > nearDistance)
                {
                    return OptimizationLevel.Culled;
                }
            }
            
            // Distance-based optimization levels
            if (distance > cullingDistance)
            {
                return OptimizationLevel.Culled;
            }
            else if (distance > farDistance)
            {
                return OptimizationLevel.Far;
            }
            else if (distance > mediumDistance)
            {
                return OptimizationLevel.Medium;
            }
            else if (distance > nearDistance)
            {
                return OptimizationLevel.Near;
            }
            
            return OptimizationLevel.None;
        }
        
        private bool IsInCameraFrustum(OptimizedObject obj)
        {
            if (playerCamera == null) return true;
            
            // Get object bounds
            Bounds bounds = GetObjectBounds(obj);
            if (bounds.size == Vector3.zero) return true; // No renderer, assume visible
            
            // Expand bounds by padding
            bounds.Expand(frustumPadding);
            
            // Test against frustum planes
            return GeometryUtility.TestPlanesAABB(cameraFrustumPlanes, bounds);
        }
        
        private Bounds GetObjectBounds(OptimizedObject obj)
        {
            if (obj.spriteRenderer != null)
            {
                return obj.spriteRenderer.bounds;
            }
            
            // Fallback to collider bounds
            if (obj.collider2D != null)
            {
                return obj.collider2D.bounds;
            }
            
            // Fallback to transform position
            return new Bounds(obj.transform.position, Vector3.one);
        }
        
        private void ApplyOptimizationLevel(OptimizedObject obj, OptimizationLevel level)
        {
            switch (level)
            {
                case OptimizationLevel.None:
                    RestoreOriginalState(obj);
                    break;
                    
                case OptimizationLevel.Near:
                    ApplyLightOptimization(obj);
                    break;
                    
                case OptimizationLevel.Medium:
                    ApplyMediumOptimization(obj);
                    break;
                    
                case OptimizationLevel.Far:
                    ApplyHeavyOptimization(obj);
                    break;
                    
                case OptimizationLevel.Culled:
                    ApplyCulling(obj);
                    break;
            }
        }
        
        private void RestoreOriginalState(OptimizedObject obj)
        {
            // Restore GameObject active state
            if (!obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.SetActive(true);
            }
            
            // Restore physics
            if (obj.rigidbody2D != null)
            {
                obj.rigidbody2D.simulated = obj.originalRigidbodySimulated;
            }
            
            // Restore rendering
            if (obj.spriteRenderer != null)
            {
                obj.spriteRenderer.enabled = obj.originalSpriteRendererEnabled;
            }
            
            // Restore animation
            if (obj.animator != null)
            {
                obj.animator.enabled = obj.originalAnimatorEnabled;
            }
            
            // Restore collider
            if (obj.collider2D != null)
            {
                obj.collider2D.enabled = obj.originalColliderEnabled;
            }
            
            // Restore audio
            if (obj.audioSource != null)
            {
                obj.audioSource.enabled = obj.originalAudioSourceEnabled;
            }
            
            // Restore particles
            if (obj.particleSystem != null && obj.originalParticleSystemPlaying)
            {
                if (!obj.particleSystem.isPlaying)
                {
                    obj.particleSystem.Play();
                }
            }
        }
        
        private void ApplyLightOptimization(OptimizedObject obj)
        {
            // Keep everything enabled but optimize particles and audio
            RestoreOriginalState(obj);
            
            // Light particle optimization
            if (enableParticleOptimization && obj.particleSystem != null)
            {
                if (obj.particleSystem.isPlaying)
                {
                    var main = obj.particleSystem.main;
                    main.maxParticles = Mathf.Max(1, main.maxParticles / 2);
                }
            }
        }
        
        private void ApplyMediumOptimization(OptimizedObject obj)
        {
            // Disable non-critical audio
            if (enableAudioOptimization && obj.audioSource != null)
            {
                obj.audioSource.enabled = false;
            }
            
            // Pause particles
            if (enableParticleOptimization && obj.particleSystem != null)
            {
                obj.particleSystem.Pause();
            }
            
            // Keep physics, rendering, and animation active
        }
        
        private void ApplyHeavyOptimization(OptimizedObject obj)
        {
            // Disable physics
            if (enablePhysicsOptimization && obj.rigidbody2D != null)
            {
                obj.rigidbody2D.simulated = false;
            }
            
            // Disable animation
            if (enableAnimationOptimization && obj.animator != null)
            {
                obj.animator.enabled = false;
            }
            
            // Disable non-trigger colliders
            if (enableColliderOptimization && obj.collider2D != null && !obj.collider2D.isTrigger)
            {
                obj.collider2D.enabled = false;
            }
            
            // Disable audio
            if (enableAudioOptimization && obj.audioSource != null)
            {
                obj.audioSource.enabled = false;
            }
            
            // Stop particles
            if (enableParticleOptimization && obj.particleSystem != null)
            {
                obj.particleSystem.Stop();
            }
            
            // Keep rendering for visual continuity
        }
        
        private void ApplyCulling(OptimizedObject obj)
        {
            if (enableCullingOptimization)
            {
                obj.gameObject.SetActive(false);
            }
            else
            {
                // If culling is disabled, apply heavy optimization instead
                ApplyHeavyOptimization(obj);
                
                // Also disable rendering
                if (enableRenderingOptimization && obj.spriteRenderer != null)
                {
                    obj.spriteRenderer.enabled = false;
                }
            }
        }
        
        private float GetUpdateInterval(float distance)
        {
            if (distance <= nearDistance)
            {
                return nearUpdateInterval;
            }
            else if (distance <= mediumDistance)
            {
                return mediumUpdateInterval;
            }
            else
            {
                return farUpdateInterval;
            }
        }
        
        #endregion
        
        #region Utility Methods
        
        #endregion
        
        #region Nested Classes
        
        /// <summary>
        /// Container for all cached components of an optimized object
        /// </summary>
        private class OptimizedObject
        {
            public GameObject gameObject;
            public Transform transform;
            public Rigidbody2D rigidbody2D;
            public SpriteRenderer spriteRenderer;
            public Animator animator;
            public Collider2D collider2D;
            public AudioSource audioSource;
            public ParticleSystem particleSystem;
            
            // Original states for restoration
            public bool originalRigidbodySimulated;
            public bool originalSpriteRendererEnabled;
            public bool originalAnimatorEnabled;
            public bool originalColliderEnabled;
            public bool originalAudioSourceEnabled;
            public bool originalParticleSystemPlaying;
            
            // Current optimization state
            public float lastDistance;
            public bool isInFrustum;
            public float nextUpdateTime;
            public OptimizationLevel currentLevel;
            
            public OptimizedObject(GameObject obj)
            {
                gameObject = obj;
                transform = obj.transform;
                rigidbody2D = obj.GetComponent<Rigidbody2D>();
                spriteRenderer = obj.GetComponent<SpriteRenderer>();
                animator = obj.GetComponent<Animator>();
                collider2D = obj.GetComponent<Collider2D>();
                audioSource = obj.GetComponent<AudioSource>();
                particleSystem = obj.GetComponent<ParticleSystem>();
                
                // Store original states
                if (rigidbody2D) originalRigidbodySimulated = rigidbody2D.simulated;
                if (spriteRenderer) originalSpriteRendererEnabled = spriteRenderer.enabled;
                if (animator) originalAnimatorEnabled = animator.enabled;
                if (collider2D) originalColliderEnabled = collider2D.enabled;
                if (audioSource) originalAudioSourceEnabled = audioSource.enabled;
                if (particleSystem) originalParticleSystemPlaying = particleSystem.isPlaying;
                
                currentLevel = OptimizationLevel.None;
            }
        }
        
        private enum OptimizationLevel
        {
            None,       // No optimization
            Near,       // Light optimization
            Medium,     // Moderate optimization
            Far,        // Heavy optimization
            Culled      // Completely culled
        }
        
        #endregion
    }
}