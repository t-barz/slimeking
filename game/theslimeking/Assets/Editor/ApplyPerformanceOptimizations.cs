using UnityEngine;
using UnityEditor;
using SlimeKing.Performance;
using SlimeKing.Systems.Core;

namespace SlimeKing.Editor
{
    /// <summary>
    /// Script temporário para aplicar otimizações de performance na cena InitialForest
    /// </summary>
    public static class ApplyPerformanceOptimizations
    {
        [MenuItem("Extra Tools/Performance/Apply All Optimizations to Scene")]
        public static void ApplyAllOptimizations()
        {
            int changes = 0;
            
            // 1. Substituir sistemas LOD antigos
            changes += ReplaceOldLODSystems();
            
            // 2. Adicionar StaggeredUpdateManager se não existir
            changes += AddStaggeredUpdateManager();
            
            // 3. Criar pool de cristais
            changes += CreateCrystalPool();
            
            if (changes > 0)
            {
                EditorUtility.DisplayDialog(
                    "Otimizações Aplicadas",
                    $"{changes} otimização(ões) aplicada(s) com sucesso!\n\n" +
                    "Verifique o console para detalhes.",
                    "OK"
                );
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Nenhuma Mudança",
                    "Todas as otimizações já estão aplicadas na cena.",
                    "OK"
                );
            }
        }
        
        private static int ReplaceOldLODSystems()
        {
            int replaced = 0;
            
            // Encontra TreeLODSystem
            var treeLODSystems = Object.FindObjectsByType<TreeLODSystem>(FindObjectsSortMode.None);
            foreach (var oldSystem in treeLODSystems)
            {
                GameObject obj = oldSystem.gameObject;
                
                // Remove sistema antigo
                Undo.DestroyObjectImmediate(oldSystem);
                
                // Adiciona novo sistema
                UnifiedLODSystem newSystem = Undo.AddComponent<UnifiedLODSystem>(obj);
                
                // Configura
                SerializedObject so = new SerializedObject(newSystem);
                so.FindProperty("targetTags").ClearArray();
                so.FindProperty("targetTags").InsertArrayElementAtIndex(0);
                so.FindProperty("targetTags").GetArrayElementAtIndex(0).stringValue = "WindShaker";
                so.FindProperty("targetTags").InsertArrayElementAtIndex(1);
                so.FindProperty("targetTags").GetArrayElementAtIndex(1).stringValue = "Prop";
                so.FindProperty("nearDistance").floatValue = 15f;
                so.FindProperty("mediumDistance").floatValue = 25f;
                so.FindProperty("farDistance").floatValue = 35f;
                so.FindProperty("cullDistance").floatValue = 50f;
                so.FindProperty("updateInterval").floatValue = 0.5f;
                so.ApplyModifiedProperties();
                
                replaced++;
                Debug.Log($"[PerformanceOptimizations] Substituído TreeLODSystem por UnifiedLODSystem em '{obj.name}'");
            }
            
            // Encontra AggressiveTreeOptimizer
            var aggressiveOptimizers = Object.FindObjectsByType<AggressiveTreeOptimizer>(FindObjectsSortMode.None);
            foreach (var oldSystem in aggressiveOptimizers)
            {
                GameObject obj = oldSystem.gameObject;
                
                // Remove sistema antigo
                Undo.DestroyObjectImmediate(oldSystem);
                
                // Adiciona novo sistema se ainda não tiver
                if (obj.GetComponent<UnifiedLODSystem>() == null)
                {
                    UnifiedLODSystem newSystem = Undo.AddComponent<UnifiedLODSystem>(obj);
                    
                    // Configura
                    SerializedObject so = new SerializedObject(newSystem);
                    so.FindProperty("targetTags").ClearArray();
                    so.FindProperty("targetTags").InsertArrayElementAtIndex(0);
                    so.FindProperty("targetTags").GetArrayElementAtIndex(0).stringValue = "WindShaker";
                    so.ApplyModifiedProperties();
                }
                
                replaced++;
                Debug.Log($"[PerformanceOptimizations] Substituído AggressiveTreeOptimizer por UnifiedLODSystem em '{obj.name}'");
            }
            
            return replaced;
        }
        
        private static int AddStaggeredUpdateManager()
        {
            // Verifica se já existe
            var existing = Object.FindFirstObjectByType<StaggeredUpdateManager>();
            if (existing != null)
            {
                Debug.Log("[PerformanceOptimizations] StaggeredUpdateManager já existe na cena");
                return 0;
            }
            
            // Cria novo GameObject
            GameObject obj = new GameObject("StaggeredUpdateManager");
            Undo.RegisterCreatedObjectUndo(obj, "Create StaggeredUpdateManager");
            
            // Adiciona componente
            var manager = Undo.AddComponent<StaggeredUpdateManager>(obj);
            
            // Configura
            SerializedObject so = new SerializedObject(manager);
            so.FindProperty("updatesPerFrame").intValue = 10;
            so.FindProperty("showDebug").boolValue = false;
            so.ApplyModifiedProperties();
            
            Debug.Log("[PerformanceOptimizations] StaggeredUpdateManager criado");
            return 1;
        }
        
        private static int CreateCrystalPool()
        {
            // Verifica se já existe um pool de cristais
            var existingPools = Object.FindObjectsByType<ObjectPool>(FindObjectsSortMode.None);
            foreach (var existingPool in existingPools)
            {
                if (existingPool.gameObject.name.Contains("Crystal"))
                {
                    Debug.Log("[PerformanceOptimizations] Crystal Pool já existe na cena");
                    return 0;
                }
            }
            
            // Busca prefab de cristal
            string[] guids = AssetDatabase.FindAssets("t:Prefab Crystal", new[] { "Assets/_Prefabs/Items" });
            
            if (guids.Length == 0)
            {
                Debug.LogWarning("[PerformanceOptimizations] Nenhum prefab de cristal encontrado. Pool não criado.");
                return 0;
            }
            
            // Pega o primeiro cristal encontrado
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            GameObject crystalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            // Cria GameObject do pool
            GameObject poolObj = new GameObject("CrystalPool");
            Undo.RegisterCreatedObjectUndo(poolObj, "Create Crystal Pool");
            
            // Adiciona ObjectPool
            var pool = Undo.AddComponent<ObjectPool>(poolObj);
            
            // Configura
            SerializedObject so = new SerializedObject(pool);
            so.FindProperty("prefab").objectReferenceValue = crystalPrefab;
            so.FindProperty("initialSize").intValue = 20;
            so.FindProperty("maxSize").intValue = 50;
            so.FindProperty("expandIfNeeded").boolValue = true;
            so.FindProperty("showDebug").boolValue = false;
            so.ApplyModifiedProperties();
            
            Debug.Log($"[PerformanceOptimizations] Crystal Pool criado com prefab: {crystalPrefab.name}");
            return 1;
        }
    }
}
