using UnityEditor;
using UnityEngine;
using SlimeKing.Core;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Adds DialogueManager to the active scene if missing.
    /// </summary>
    public static class DialogueManagerSetup
    {
        [MenuItem("Extra Tools/Setup/Add DialogueManager to Scene")] 
        public static void AddDialogueManager()
        {
            var existing = Object.FindObjectOfType<DialogueManager>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog("DialogueManager", "DialogueManager already exists in the scene.", "OK");
                return;
            }

            var go = new GameObject("DialogueManager");
            go.AddComponent<DialogueManager>();
            Selection.activeGameObject = go;
            EditorUtility.DisplayDialog("DialogueManager", "DialogueManager added to scene.", "OK");
        }
    }
}
