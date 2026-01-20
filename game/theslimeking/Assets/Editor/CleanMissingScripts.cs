using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class CleanMissingScripts
{
    [MenuItem("Tools/Clean Missing Scripts in Scene")]
    public static void CleanMissingScriptsInScene()
    {
        var scene = SceneManager.GetActiveScene();
        var rootObjects = scene.GetRootGameObjects();
        int totalRemoved = 0;

        foreach (var root in rootObjects)
        {
            var allTransforms = root.GetComponentsInChildren<Transform>(true);
            foreach (var t in allTransforms)
            {
                var go = t.gameObject;
                int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                if (removed > 0)
                {
                    Debug.Log($"Removed {removed} missing script(s) from '{go.name}'");
                    totalRemoved += removed;
                }
            }
        }

        if (totalRemoved > 0)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log($"Total: Removed {totalRemoved} missing script(s). Save the scene to persist changes.");
        }
        else
        {
            Debug.Log("No missing scripts found in the scene.");
        }
    }
}
