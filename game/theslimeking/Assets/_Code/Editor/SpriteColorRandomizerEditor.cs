using UnityEngine;
using UnityEditor;
using TheSlimeKing.Systems.Visual;

namespace TheSlimeKing.Editor
{
    /// <summary>
    /// Custom editor for SpriteColorRandomizer with preview and manual controls
    /// </summary>
    [CustomEditor(typeof(SpriteColorRandomizer))]
    public class SpriteColorRandomizerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            SpriteColorRandomizer randomizer = (SpriteColorRandomizer)target;

            // Draw default inspector
            DrawDefaultInspector();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Manual Controls", EditorStyles.boldLabel);

            // Manual randomize button
            if (GUILayout.Button("Randomize Color Now", GUILayout.Height(30)))
            {
                Undo.RecordObject(randomizer.GetComponent<SpriteRenderer>(), "Randomize Color");
                randomizer.RandomizeColor();
                EditorUtility.SetDirty(randomizer.GetComponent<SpriteRenderer>());
            }

            // Reset button
            if (GUILayout.Button("Reset to White"))
            {
                Undo.RecordObject(randomizer.GetComponent<SpriteRenderer>(), "Reset Color");
                randomizer.ResetColor();
                EditorUtility.SetDirty(randomizer.GetComponent<SpriteRenderer>());
            }

            EditorGUILayout.Space(5);

            // Current color display
            SpriteRenderer sr = randomizer.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                EditorGUILayout.LabelField("Current Color:", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ColorField("", sr.color);
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}
