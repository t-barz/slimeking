using UnityEngine;
using UnityEditor;
using SlimeKing.Performance;

namespace SlimeKing.Editor
{
    /// <summary>
    /// Helper para aplicar otimizações de performance nas cenas.
    /// 
    /// Menu: Extra Tools/Performance/Apply Optimizations
    /// </summary>
    public static class PerformanceOptimizationHelper
    {
        [MenuItem("Extra Tools/Performance/Replace Old LOD Systems")]
        public static void ReplaceOldLODSystems()
        {
            int replaced = 0;
            
            // Encontra TreeLODSystem usando reflection para evitar dependência direta
            var treeLODType = System.Type.GetType("SlimeKing.Performance.TreeLODSystem, Assembly-CSharp");
            if (treeLODType != null)
            {
                var treeLODSystems = Object.FindObjectsByType(treeLODType, FindObjectsSortMode.None);
                foreach (var oldSystemObj in treeLODSystems)
                {
                    var oldSystem = oldSystemObj as MonoBehaviour;
                    if (oldSystem == null) continue;
                    
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
                    so.ApplyModifiedProperties();
                    
                    replaced++;
                    Debug.Log($"[PerformanceHelper] Substituído TreeLODSystem por UnifiedLODSystem em '{obj.name}'");
                }
            }
            
            // Encontra AggressiveTreeOptimizer usando reflection
            var aggressiveType = System.Type.GetType("SlimeKing.Performance.AggressiveTreeOptimizer, Assembly-CSharp");
            if (aggressiveType != null)
            {
                var aggressiveOptimizers = Object.FindObjectsByType(aggressiveType, FindObjectsSortMode.None);
                foreach (var oldSystemObj in aggressiveOptimizers)
                {
                    var oldSystem = oldSystemObj as MonoBehaviour;
                    if (oldSystem == null) continue;
                    
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
                    Debug.Log($"[PerformanceHelper] Substituído AggressiveTreeOptimizer por UnifiedLODSystem em '{obj.name}'");
                }
            }
            
            if (replaced > 0)
            {
                EditorUtility.DisplayDialog(
                    "LOD Systems Substituídos",
                    $"Substituídos {replaced} sistema(s) LOD antigo(s) por UnifiedLODSystem.\n\nVerifique as configurações no Inspector.",
                    "OK"
                );
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Nenhum Sistema Encontrado",
                    "Não foram encontrados sistemas LOD antigos na cena atual.",
                    "OK"
                );
            }
        }
        
        [MenuItem("Extra Tools/Performance/Add Staggered Update Manager")]
        public static void AddStaggeredUpdateManager()
        {
            // Verifica se já existe
            var existing = Object.FindFirstObjectByType<SlimeKing.Systems.Core.StaggeredUpdateManager>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog(
                    "Já Existe",
                    "StaggeredUpdateManager já existe na cena.",
                    "OK"
                );
                Selection.activeGameObject = existing.gameObject;
                return;
            }
            
            // Cria novo GameObject
            GameObject obj = new GameObject("StaggeredUpdateManager");
            Undo.RegisterCreatedObjectUndo(obj, "Create StaggeredUpdateManager");
            
            // Adiciona componente
            var manager = Undo.AddComponent<SlimeKing.Systems.Core.StaggeredUpdateManager>(obj);
            
            // Configura
            SerializedObject so = new SerializedObject(manager);
            so.FindProperty("updatesPerFrame").intValue = 10;
            so.FindProperty("showDebug").boolValue = false;
            so.ApplyModifiedProperties();
            
            Selection.activeGameObject = obj;
            
            Debug.Log("[PerformanceHelper] StaggeredUpdateManager criado");
            
            EditorUtility.DisplayDialog(
                "Manager Criado",
                "StaggeredUpdateManager criado com sucesso!\n\nEste manager será DontDestroyOnLoad automaticamente.",
                "OK"
            );
        }
        
        [MenuItem("Extra Tools/Performance/Create Crystal Pool")]
        public static void CreateCrystalPool()
        {
            // Busca prefab de cristal
            string[] guids = AssetDatabase.FindAssets("t:Prefab Crystal", new[] { "Assets/_Prefabs/Items" });
            
            if (guids.Length == 0)
            {
                EditorUtility.DisplayDialog(
                    "Prefab Não Encontrado",
                    "Não foi encontrado nenhum prefab de cristal em Assets/_Prefabs/Items.\n\nCrie o pool manualmente.",
                    "OK"
                );
                return;
            }
            
            // Pega o primeiro cristal encontrado
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            GameObject crystalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            // Cria GameObject do pool
            GameObject poolObj = new GameObject("CrystalPool");
            Undo.RegisterCreatedObjectUndo(poolObj, "Create Crystal Pool");
            
            // Adiciona ObjectPool
            var pool = Undo.AddComponent<SlimeKing.Systems.Core.ObjectPool>(poolObj);
            
            // Configura
            SerializedObject so = new SerializedObject(pool);
            so.FindProperty("prefab").objectReferenceValue = crystalPrefab;
            so.FindProperty("initialSize").intValue = 20;
            so.FindProperty("maxSize").intValue = 50;
            so.FindProperty("expandIfNeeded").boolValue = true;
            so.FindProperty("showDebug").boolValue = false;
            so.ApplyModifiedProperties();
            
            Selection.activeGameObject = poolObj;
            
            Debug.Log($"[PerformanceHelper] Crystal Pool criado com prefab: {crystalPrefab.name}");
            
            EditorUtility.DisplayDialog(
                "Pool Criado",
                $"Crystal Pool criado com sucesso!\n\nPrefab: {crystalPrefab.name}\nInicial: 20 objetos\nMáximo: 50 objetos",
                "OK"
            );
        }
        
        [MenuItem("Extra Tools/Performance/Show Performance Report")]
        public static void ShowPerformanceReport()
        {
            string report = "=== PERFORMANCE OPTIMIZATION REPORT ===\n\n";
            
            // LOD Systems
            var unifiedLOD = Object.FindObjectsByType<UnifiedLODSystem>(FindObjectsSortMode.None);
            var oldTreeLOD = Object.FindObjectsByType<TreeLODSystem>(FindObjectsSortMode.None);
            var oldAggressive = Object.FindObjectsByType<AggressiveTreeOptimizer>(FindObjectsSortMode.None);
            
            report += $"LOD Systems:\n";
            report += $"  ✅ UnifiedLODSystem: {unifiedLOD.Length}\n";
            report += $"  ⚠️ TreeLODSystem (antigo): {oldTreeLOD.Length}\n";
            report += $"  ⚠️ AggressiveTreeOptimizer (antigo): {oldAggressive.Length}\n\n";
            
            // Object Pools
            var pools = Object.FindObjectsByType<SlimeKing.Systems.Core.ObjectPool>(FindObjectsSortMode.None);
            report += $"Object Pools: {pools.Length}\n";
            foreach (var pool in pools)
            {
                var stats = pool.GetStats();
                report += $"  - {stats.prefabName}: {stats.totalObjects} objetos\n";
            }
            report += "\n";
            
            // Staggered Update
            var staggered = Object.FindFirstObjectByType<SlimeKing.Systems.Core.StaggeredUpdateManager>();
            if (staggered != null)
            {
                var stats = staggered.GetStats();
                report += $"Staggered Update Manager:\n";
                report += $"  ✅ Ativo\n";
                report += $"  Registrados: {stats.totalRegistered}\n";
                report += $"  Updates/Frame: {stats.updatesPerFrame}\n\n";
            }
            else
            {
                report += $"Staggered Update Manager:\n";
                report += $"  ❌ Não encontrado\n\n";
            }
            
            // Recomendações
            report += "RECOMENDAÇÕES:\n";
            if (oldTreeLOD.Length > 0 || oldAggressive.Length > 0)
            {
                report += "  ⚠️ Substituir sistemas LOD antigos\n";
                report += "     Menu: Extra Tools/Performance/Replace Old LOD Systems\n\n";
            }
            if (pools.Length == 0)
            {
                report += "  ⚠️ Criar pools para cristais e VFX\n";
                report += "     Menu: Extra Tools/Performance/Create Crystal Pool\n\n";
            }
            if (staggered == null)
            {
                report += "  ⚠️ Adicionar Staggered Update Manager\n";
                report += "     Menu: Extra Tools/Performance/Add Staggered Update Manager\n\n";
            }
            
            if (oldTreeLOD.Length == 0 && oldAggressive.Length == 0 && pools.Length > 0 && staggered != null)
            {
                report += "  ✅ Todas as otimizações aplicadas!\n";
            }
            
            Debug.Log(report);
            
            EditorUtility.DisplayDialog(
                "Performance Report",
                report,
                "OK"
            );
        }
    }
}
