using UnityEngine;
using System.Collections.Generic;
using SlimeKing.Systems.Core;

namespace SlimeKing.Performance
{
    /// <summary>
    /// Sistema de LOD (Level of Detail) unificado e otimizado.
    /// Substitui TreeLODSystem e AggressiveTreeOptimizer com implementação mais eficiente.
    /// 
    /// PERFORMANCE:
    /// - Usa PlayerCache ao invés de Camera.main
    /// - sqrMagnitude ao invés de Distance
    /// - Update interval configurável
    /// - Spatial partitioning para grandes quantidades
    /// </summary>
    public class UnifiedLODSystem : MonoBehaviour
    {
        [Header("LOD Settings")]
        [SerializeField] private bool enableLOD = true;
        [SerializeField] private float updateInterval = 0.5f;
        
        [Header("Distance Thresholds")]
        [Tooltip("Distância para LOD Near - Tudo ativo")]
        [SerializeField] private float nearDistance = 15f;
        
        [Tooltip("Distância para LOD Medium - Desabilita animação")]
        [SerializeField] private float mediumDistance = 25f;
        
        [Tooltip("Distância para LOD Far - Desabilita rendering")]
        [SerializeField] private float farDistance = 35f;
        
        [Tooltip("Distância para Culling - Desabilita objeto")]
        [SerializeField] private float cullDistance = 50f;
        
        [Header("Target Tags")]
        [SerializeField] private string[] targetTags = new string[] { "WindShaker", "Prop", "Enemy" };
        
        [Header("Optimization")]
        [Tooltip("Usar spatial partitioning para grandes quantidades (>100 objetos)")]
        [SerializeField] private bool useSpatialPartitioning = false;
        [SerializeField] private float spatialCellSize = 20f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebug = false;
        [SerializeField] private bool showGizmos = false;
        
        private Transform playerTransform;
        private List<LODObject> objects = new List<LODObject>();
        private Dictionary<Vector2Int, List<LODObject>> spatialGrid;
        private float nextUpdateTime = 0f;
        
        // Stats
        private int nearCount = 0;
        private int mediumCount = 0;
        private int farCount = 0;
        private int culledCount = 0;
        
        // Cache de distâncias ao quadrado
        private float nearDistanceSqr;
        private float mediumDistanceSqr;
        private float farDistanceSqr;
        private float cullDistanceSqr;
        
        #region Unity Lifecycle
        
        private void Start()
        {
            // Cache distâncias ao quadrado
            CacheDistances();
            
            // Cache objetos
            CacheObjects();
            
            // Inicializa spatial grid se necessário
            if (useSpatialPartitioning && objects.Count > 100)
            {
                InitializeSpatialGrid();
            }
            
            if (showDebug)
            {
                Debug.Log($"[UnifiedLODSystem] Inicializado com {objects.Count} objetos");
            }
        }
        
        private void Update()
        {
            if (!enableLOD || Time.time < nextUpdateTime) return;
            
            nextUpdateTime = Time.time + updateInterval;
            
            // Usa PlayerCache ao invés de Camera.main
            playerTransform = PlayerCache.PlayerTransform;
            if (playerTransform == null) return;
            
            UpdateLOD();
        }
        
        #endregion
        
        #region Initialization
        
        private void CacheDistances()
        {
            nearDistanceSqr = nearDistance * nearDistance;
            mediumDistanceSqr = mediumDistance * mediumDistance;
            farDistanceSqr = farDistance * farDistance;
            cullDistanceSqr = cullDistance * cullDistance;
        }
        
        private void CacheObjects()
        {
            objects.Clear();
            
            foreach (string tag in targetTags)
            {
                GameObject[] tagged = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject obj in tagged)
                {
                    objects.Add(new LODObject(obj));
                }
            }
        }
        
        private void InitializeSpatialGrid()
        {
            spatialGrid = new Dictionary<Vector2Int, List<LODObject>>();
            
            foreach (LODObject obj in objects)
            {
                Vector2Int cell = GetCell(obj.transform.position);
                if (!spatialGrid.ContainsKey(cell))
                {
                    spatialGrid[cell] = new List<LODObject>();
                }
                spatialGrid[cell].Add(obj);
            }
            
            if (showDebug)
            {
                Debug.Log($"[UnifiedLODSystem] Spatial grid inicializado com {spatialGrid.Count} células");
            }
        }
        
        #endregion
        
        #region LOD Update
        
        private void UpdateLOD()
        {
            Vector3 playerPos = playerTransform.position;
            
            nearCount = 0;
            mediumCount = 0;
            farCount = 0;
            culledCount = 0;
            
            if (useSpatialPartitioning && spatialGrid != null)
            {
                UpdateLODSpatial(playerPos);
            }
            else
            {
                UpdateLODDirect(playerPos);
            }
        }
        
        private void UpdateLODDirect(Vector3 playerPos)
        {
            foreach (LODObject obj in objects)
            {
                if (obj.gameObject == null) continue;
                
                // OTIMIZADO: sqrMagnitude ao invés de Distance
                float sqrDistance = (obj.transform.position - playerPos).sqrMagnitude;
                LODLevel newLevel = GetLODLevel(sqrDistance);
                
                if (obj.currentLevel != newLevel)
                {
                    ApplyLOD(obj, newLevel);
                    obj.currentLevel = newLevel;
                }
                
                UpdateStats(newLevel);
            }
        }
        
        private void UpdateLODSpatial(Vector3 playerPos)
        {
            Vector2Int playerCell = GetCell(playerPos);
            int cellRadius = Mathf.CeilToInt(cullDistance / spatialCellSize);
            
            // Apenas processa células próximas ao player
            for (int x = -cellRadius; x <= cellRadius; x++)
            {
                for (int y = -cellRadius; y <= cellRadius; y++)
                {
                    Vector2Int cell = playerCell + new Vector2Int(x, y);
                    
                    if (spatialGrid.TryGetValue(cell, out List<LODObject> cellObjects))
                    {
                        foreach (LODObject obj in cellObjects)
                        {
                            if (obj.gameObject == null) continue;
                            
                            float sqrDistance = (obj.transform.position - playerPos).sqrMagnitude;
                            LODLevel newLevel = GetLODLevel(sqrDistance);
                            
                            if (obj.currentLevel != newLevel)
                            {
                                ApplyLOD(obj, newLevel);
                                obj.currentLevel = newLevel;
                            }
                            
                            UpdateStats(newLevel);
                        }
                    }
                }
            }
        }
        
        private LODLevel GetLODLevel(float sqrDistance)
        {
            if (sqrDistance > cullDistanceSqr) return LODLevel.Culled;
            if (sqrDistance > farDistanceSqr) return LODLevel.Far;
            if (sqrDistance > mediumDistanceSqr) return LODLevel.Medium;
            return LODLevel.Near;
        }
        
        private void ApplyLOD(LODObject obj, LODLevel level)
        {
            switch (level)
            {
                case LODLevel.Near:
                    obj.gameObject.SetActive(true);
                    if (obj.animator != null) obj.animator.enabled = true;
                    if (obj.spriteRenderer != null) obj.spriteRenderer.enabled = true;
                    break;
                    
                case LODLevel.Medium:
                    obj.gameObject.SetActive(true);
                    if (obj.animator != null) obj.animator.enabled = false;
                    if (obj.spriteRenderer != null) obj.spriteRenderer.enabled = true;
                    break;
                    
                case LODLevel.Far:
                    obj.gameObject.SetActive(true);
                    if (obj.animator != null) obj.animator.enabled = false;
                    if (obj.spriteRenderer != null) obj.spriteRenderer.enabled = false;
                    break;
                    
                case LODLevel.Culled:
                    obj.gameObject.SetActive(false);
                    break;
            }
        }
        
        private void UpdateStats(LODLevel level)
        {
            switch (level)
            {
                case LODLevel.Near: nearCount++; break;
                case LODLevel.Medium: mediumCount++; break;
                case LODLevel.Far: farCount++; break;
                case LODLevel.Culled: culledCount++; break;
            }
        }
        
        #endregion
        
        #region Spatial Grid
        
        private Vector2Int GetCell(Vector3 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt(position.x / spatialCellSize),
                Mathf.FloorToInt(position.y / spatialCellSize)
            );
        }
        
        #endregion
        
        #region Debug
        
        private void OnGUI()
        {
            if (!showDebug) return;
            
            GUILayout.BeginArea(new Rect(10, 100, 300, 150));
            GUILayout.Label($"<b>Unified LOD System</b>");
            GUILayout.Label($"Total Objects: {objects.Count}");
            GUILayout.Label($"Near: {nearCount} | Medium: {mediumCount}");
            GUILayout.Label($"Far: {farCount} | Culled: {culledCount}");
            if (useSpatialPartitioning && spatialGrid != null)
            {
                GUILayout.Label($"Spatial Cells: {spatialGrid.Count}");
            }
            GUILayout.EndArea();
        }
        
        private void OnDrawGizmos()
        {
            if (!showGizmos || playerTransform == null) return;
            
            Vector3 playerPos = playerTransform.position;
            
            // Near distance
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerPos, nearDistance);
            
            // Medium distance
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerPos, mediumDistance);
            
            // Far distance
            Gizmos.color = new Color(1f, 0.5f, 0f); // Orange
            Gizmos.DrawWireSphere(playerPos, farDistance);
            
            // Cull distance
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerPos, cullDistance);
            
            // Spatial grid
            if (useSpatialPartitioning && spatialGrid != null)
            {
                Gizmos.color = new Color(0f, 1f, 1f, 0.3f); // Cyan transparente
                foreach (var cell in spatialGrid.Keys)
                {
                    Vector3 cellCenter = new Vector3(
                        cell.x * spatialCellSize + spatialCellSize * 0.5f,
                        cell.y * spatialCellSize + spatialCellSize * 0.5f,
                        0f
                    );
                    Gizmos.DrawWireCube(cellCenter, new Vector3(spatialCellSize, spatialCellSize, 0f));
                }
            }
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Força atualização imediata do LOD
        /// </summary>
        public void ForceUpdate()
        {
            nextUpdateTime = 0f;
        }
        
        /// <summary>
        /// Recarrega todos os objetos
        /// </summary>
        public void RefreshObjects()
        {
            CacheObjects();
            if (useSpatialPartitioning && objects.Count > 100)
            {
                InitializeSpatialGrid();
            }
        }
        
        #endregion
        
        #region Data Structures
        
        private enum LODLevel
        {
            Near,
            Medium,
            Far,
            Culled
        }
        
        private class LODObject
        {
            public GameObject gameObject;
            public Transform transform;
            public Animator animator;
            public SpriteRenderer spriteRenderer;
            public LODLevel currentLevel;
            
            public LODObject(GameObject obj)
            {
                gameObject = obj;
                transform = obj.transform;
                animator = obj.GetComponent<Animator>();
                spriteRenderer = obj.GetComponent<SpriteRenderer>();
                currentLevel = LODLevel.Near;
                
                // Configura Animator culling mode
                if (animator != null)
                {
                    animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
                }
            }
        }
        
        #endregion
    }
}
