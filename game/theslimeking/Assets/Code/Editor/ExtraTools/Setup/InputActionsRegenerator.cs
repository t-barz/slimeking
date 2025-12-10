using UnityEditor;
using UnityEngine;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Menu tool to regenerate InputSystem_Actions.cs from InputSystem_Actions.inputactions
    /// </summary>
    public class InputActionsRegenerator
    {
        [MenuItem("Extra Tools/Setup/Regenerate InputSystem_Actions")]
        public static void RegenerateInputActions()
        {
            string inputActionsPath = "Assets/Settings/InputSystem_Actions.inputactions";

            var inputActionsAsset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(inputActionsPath);

            if (inputActionsAsset == null)
            {
                Debug.LogError($"[InputActionsRegenerator] Could not find InputSystem_Actions.inputactions at {inputActionsPath}");
                return;
            }

            // Re-import the asset to trigger code generation
            AssetDatabase.ImportAsset(inputActionsPath, ImportAssetOptions.ForceUpdate);

            Debug.Log("[InputActionsRegenerator] InputSystem_Actions.cs has been regenerated successfully!");
        }
    }
}
