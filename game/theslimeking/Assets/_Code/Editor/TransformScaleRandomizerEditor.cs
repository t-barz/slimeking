using UnityEngine;
using UnityEditor;
using TheSlimeKing.Systems.Visual;

namespace TheSlimeKing.Editor
{
    /// <summary>
    /// Custom editor for TransformScaleRandomizer with preview and manual controls
    /// </summary>
    [CustomEditor(typeof(TransformScaleRandomizer))]
    public class TransformScaleRandomizerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            TransformScaleRandomizer randomizer = (TransformScaleRandomizer)target;

            // Draw default inspector
            DrawDefaultInspector();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Manual Controls", EditorStyles.boldLabel);

            // Manual randomize button
            if (GUILayout.Button("Randomize Scale Now", GUILayout.Height(30)))
            {
                Undo.RecordObject(randomizer.transform, "Randomize Scale");
                randomizer.RandomizeScale();
                EditorUtility.SetDirty(randomizer.transform);
            }

            // Reset button
            if (GUILayout.Button("Reset to Original Scale"))
            {
                Undo.RecordObject(randomizer.transform, "Reset Scale");
                randomizer.ResetScale();
                EditorUtility.SetDirty(randomizer.transform);
            }

            EditorGUILayout.Space(5);

            // Quick scale buttons
            EditorGUILayout.LabelField("Quick Scale Presets:", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("0.5x"))
            {
                Undo.RecordObject(randomizer.transform, "Set Scale 0.5x");
                randomizer.SetUniformScale(0.5f);
                EditorUtility.SetDirty(randomizer.transform);
            }
            
            if (GUILayout.Button("0.75x"))
            {
                Undo.RecordObject(randomizer.transform, "Set Scale 0.75x");
                randomizer.SetUniformScale(0.75f);
                EditorUtility.SetDirty(randomizer.transform);
            }
            
            if (GUILayout.Button("1x"))
            {
                Undo.RecordObject(randomizer.transform, "Set Scale 1x");
                randomizer.SetUniformScale(1f);
                EditorUtility.SetDirty(randomizer.transform);
            }
            
            if (GUILayout.Button("1.5x"))
            {
                Undo.RecordObject(randomizer.transform, "Set Scale 1.5x");
                randomizer.SetUniformScale(1.5f);
                EditorUtility.SetDirty(randomizer.transform);
            }
            
            if (GUILayout.Button("2x"))
            {
                Undo.RecordObject(randomizer.transform, "Set Scale 2x");
                randomizer.SetUniformScale(2f);
                EditorUtility.SetDirty(randomizer.transform);
            }
            
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Current scale display
            EditorGUILayout.LabelField("Current Scale:", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.Vector3Field("Local Scale", randomizer.transform.localScale);
            EditorGUILayout.Vector3Field("World Scale", randomizer.transform.lossyScale);
            EditorGUI.EndDisabledGroup();
        }
    }
}
