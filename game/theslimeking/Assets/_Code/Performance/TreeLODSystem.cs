using UnityEngine;
using System.Collections.Generic;

namespace SlimeKing.Performance
{
    /// <summary>
    /// Sistema de LOD (Level of Detail) otimizado para árvores e objetos com WindShaker.
    /// Desabilita componentes desnecessários baseado na distância da câmera.
    /// </summary>
    public class TreeLODSystem : MonoBehaviour
    {
        [Header("LOD Settings")]
        [SerializeField] private bool enableLOD = true;
        [SerializeField] private float updateInterval = 0.5f; // Update a cada 0.5s
        
        [Header("Distance Thresholds")]
        [SerializeField] private float nearDistance = 15f;    // Tudo ativo
        [SerializeField] private float mediumDistance = 25f;  // Desabilita animação
        [SerializeField] private float farDistance = 35f;     // Desabilita rendering
        [SerializeField] private float cullDistance = 50f;    // Desabilita objeto
        
        [Header("Target Tags")]
        [SerializeField] private string[] targetTags = new string[] { "WindShaker" };
        
        [Header("Debug")]
        [SerializeField] private bool showDebug = false;
        
        private Camera mainCamera;
        private List<TreeLOD> trees = new List<TreeLOD>();
        private float nextUpdateTime = 0f;
        
        // Stats
        private int nearCount = 0;
        private int mediumCount = 0;
        private int farCount = 0;
        private int culledCount = 0;
        
        private void Start()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("[TreeLODSystem] Main Camera não encontrada!");
                enabled = false;
                return;
            }
            
            CacheTrees();
            
            if (showDebug)
            {
                Debug.Log($"[TreeLODSystem] Inicializado com {trees.Count} árvores");
            }
        }
        
        private void Update()
        {
            if (!enableLOD || Time.time < nextUpdateTime) return;
            
            nextUpdateTime = Time.time + updateInterval;
            UpdateLOD();
        }
        
        private void CacheTrees()
        {
            trees.Clear();
            
            foreach (string tag in targetTags)
            {
                GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject obj in objects)
                {
                    trees.Add(new TreeLOD(obj));
                }
            }
        }
        
        private void UpdateLOD()
        {
            Vector3 cameraPos = mainCamera.transform.position;
            
            nearCount = 0;
            mediumCount = 0;
            farCount = 0;
            culledCount = 0;
            
            foreach (TreeLOD tree in trees)
            {
                if (tree.gameObject == null) continue;
                
                float sqrDistance = (tree.transform.position - cameraPos).sqrMagnitude;
                LODLevel newLevel = GetLODLevel(sqrDistance);
                
                if (tree.currentLevel != newLevel)
                {
                    ApplyLOD(tree, newLevel);
                    tree.currentLevel = newLevel;
                }
                
                // Count stats
                switch (newLevel)
                {
                    case LODLevel.Near: nearCount++; break;
                    case LODLevel.Medium: mediumCount++; break;
                    case LODLevel.Far: farCount++; break;
                    case LODLevel.Culled: culledCount++; break;
                }
            }
        }
        
        private LODLevel GetLODLevel(float sqrDistance)
        {
            if (sqrDistance > cullDistance * cullDistance)
                return LODLevel.Culled;
            else if (sqrDistance > farDistance * farDistance)
                return LODLevel.Far;
            else if (sqrDistance > mediumDistance * mediumDistance)
                return LODLevel.Medium;
            else
                return LODLevel.Near;
        }
        
        private void ApplyLOD(TreeLOD tree, LODLevel level)
        {
            switch (level)
            {
                case LODLevel.Near:
                    // Tudo ativo
                    tree.gameObject.SetActive(true);
                    if (tree.animator != null) tree.animator.enabled = true;
                    if (tree.spriteRenderer != null) tree.spriteRenderer.enabled = true;
                    break;
                    
                case LODLevel.Medium:
                    // Desabilita animação de vento
                    tree.gameObject.SetActive(true);
                    if (tree.animator != null) tree.animator.enabled = false;
                    if (tree.spriteRenderer != null) tree.spriteRenderer.enabled = true;
                    break;
                    
                case LODLevel.Far:
                    // Desabilita rendering
                    tree.gameObject.SetActive(true);
                    if (tree.animator != null) tree.animator.enabled = false;
                    if (tree.spriteRenderer != null) tree.spriteRenderer.enabled = false;
                    break;
                    
                case LODLevel.Culled:
                    // Desabilita objeto completamente
                    tree.gameObject.SetActive(false);
                    break;
            }
        }
        
        private void OnGUI()
        {
            if (!showDebug) return;
            
            GUILayout.BeginArea(new Rect(10, 100, 300, 150));
            GUILayout.Label($"<b>Tree LOD System</b>");
            GUILayout.Label($"Total Trees: {trees.Count}");
            GUILayout.Label($"Near: {nearCount} | Medium: {mediumCount}");
            GUILayout.Label($"Far: {farCount} | Culled: {culledCount}");
            GUILayout.EndArea();
        }
        
        private enum LODLevel
        {
            Near,
            Medium,
            Far,
            Culled
        }
        
        private class TreeLOD
        {
            public GameObject gameObject;
            public Transform transform;
            public Animator animator;
            public SpriteRenderer spriteRenderer;
            public LODLevel currentLevel;
            
            public TreeLOD(GameObject obj)
            {
                gameObject = obj;
                transform = obj.transform;
                animator = obj.GetComponent<Animator>();
                spriteRenderer = obj.GetComponent<SpriteRenderer>();
                currentLevel = LODLevel.Near;
            }
        }
    }
}
