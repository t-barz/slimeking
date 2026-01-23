using UnityEngine;
using System.Collections.Generic;

namespace SlimeKing.Performance
{
    /// <summary>
    /// Otimizador AGRESSIVO para árvores. Desabilita completamente objetos fora da câmera.
    /// Use este quando TreeLODSystem não for suficiente.
    /// </summary>
    public class AggressiveTreeOptimizer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float updateInterval = 0.5f;
        [SerializeField] private float cullDistance = 30f;
        [SerializeField] private bool disableAnimators = true;
        [SerializeField] private bool disableRenderers = true;
        
        [Header("Target Tags")]
        [SerializeField] private string[] targetTags = new string[] { "WindShaker" };
        
        [Header("Debug")]
        [SerializeField] private bool showDebug = false;
        
        private Camera mainCamera;
        private List<TreeData> trees = new List<TreeData>();
        private float nextUpdateTime = 0f;
        private int activeCount = 0;
        private int disabledCount = 0;
        
        private void Start()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("[AggressiveTreeOptimizer] Main Camera não encontrada!");
                enabled = false;
                return;
            }
            
            CacheTrees();
            
            if (showDebug)
            {
                Debug.Log($"[AggressiveTreeOptimizer] Inicializado com {trees.Count} árvores");
            }
        }
        
        private void Update()
        {
            if (Time.time < nextUpdateTime) return;
            
            nextUpdateTime = Time.time + updateInterval;
            OptimizeTrees();
        }
        
        private void CacheTrees()
        {
            trees.Clear();
            
            foreach (string tag in targetTags)
            {
                GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject obj in objects)
                {
                    trees.Add(new TreeData(obj));
                }
            }
        }
        
        private void OptimizeTrees()
        {
            Vector3 cameraPos = mainCamera.transform.position;
            activeCount = 0;
            disabledCount = 0;
            
            foreach (TreeData tree in trees)
            {
                if (tree.gameObject == null) continue;
                
                float sqrDistance = (tree.transform.position - cameraPos).sqrMagnitude;
                bool shouldBeActive = sqrDistance <= (cullDistance * cullDistance);
                
                if (shouldBeActive != tree.isActive)
                {
                    tree.isActive = shouldBeActive;
                    
                    if (disableAnimators && tree.animator != null)
                    {
                        tree.animator.enabled = shouldBeActive;
                    }
                    
                    if (disableRenderers && tree.spriteRenderer != null)
                    {
                        tree.spriteRenderer.enabled = shouldBeActive;
                    }
                }
                
                if (tree.isActive)
                    activeCount++;
                else
                    disabledCount++;
            }
        }
        
        private void OnGUI()
        {
            if (!showDebug) return;
            
            GUILayout.BeginArea(new Rect(10, 250, 300, 100));
            GUILayout.Label($"<b>Aggressive Tree Optimizer</b>");
            GUILayout.Label($"Total: {trees.Count}");
            GUILayout.Label($"Active: {activeCount} | Disabled: {disabledCount}");
            GUILayout.EndArea();
        }
        
        private class TreeData
        {
            public GameObject gameObject;
            public Transform transform;
            public Animator animator;
            public SpriteRenderer spriteRenderer;
            public bool isActive = true;
            
            public TreeData(GameObject obj)
            {
                gameObject = obj;
                transform = obj.transform;
                animator = obj.GetComponent<Animator>();
                spriteRenderer = obj.GetComponent<SpriteRenderer>();
            }
        }
    }
}
