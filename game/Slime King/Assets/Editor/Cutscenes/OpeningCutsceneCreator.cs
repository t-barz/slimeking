using UnityEngine;
using UnityEditor;
using System.IO;

namespace SlimeKing.Core.Cutscenes.Editor
{
    public class OpeningCutsceneCreator : EditorWindow
    {
        [MenuItem("SlimeKing/Create Opening Cutscene")]
        public static void CreateOpeningCutscene()
        {
            CreateRequiredFolders();

            // Primeiro, certifique-se de que todos os scripts existam no projeto
            var monoscriptGuids = AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets" });
            string cutsceneDefinitionPath = null;
            string animSetBoolPath = null;
            string waitEventPath = null;
            string dialogueEventPath = null;

            foreach (var guid in monoscriptGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string filename = Path.GetFileNameWithoutExtension(path);

                switch (filename)
                {
                    case "CutsceneDefinition":
                        cutsceneDefinitionPath = path;
                        break;
                    case "AnimationSetBoolEvent":
                        animSetBoolPath = path;
                        break;
                    case "WaitEvent":
                        waitEventPath = path;
                        break;
                    case "DialogueEvent":
                        dialogueEventPath = path;
                        break;
                }
            }

            if (cutsceneDefinitionPath == null || animSetBoolPath == null ||
                waitEventPath == null || dialogueEventPath == null)
            {
                Debug.LogError("Não foi possível encontrar todos os scripts necessários!");
                return;
            }

            // Cria os eventos
            var wakeUpEvent = ScriptableObject.CreateInstance<AnimationSetBoolEvent>();
            AssetDatabase.CreateAsset(wakeUpEvent, "Assets/Resources/Cutscenes/Events/WakeUpEvent.asset");

            var waitEvent = ScriptableObject.CreateInstance<WaitEvent>();
            AssetDatabase.CreateAsset(waitEvent, "Assets/Resources/Cutscenes/Events/WaitEvent.asset");

            var dialogueEvent = ScriptableObject.CreateInstance<DialogueEvent>();
            AssetDatabase.CreateAsset(dialogueEvent, "Assets/Resources/Cutscenes/Events/WakeUpDialogue.asset");

            // Força um refresh após criar os assets
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Configura o evento de acordar
            SerializedObject wakeUpSerializedObject = new SerializedObject(wakeUpEvent);
            wakeUpSerializedObject.FindProperty("canBeSkipped").boolValue = true;
            wakeUpSerializedObject.FindProperty("duration").floatValue = 0.1f;
            wakeUpSerializedObject.FindProperty("targetTag").stringValue = "Player";
            wakeUpSerializedObject.FindProperty("parameterName").stringValue = "isSleeping";
            wakeUpSerializedObject.FindProperty("value").boolValue = false;
            wakeUpSerializedObject.FindProperty("initialDelay").floatValue = 2f;
            wakeUpSerializedObject.ApplyModifiedProperties();

            // Configura o evento de espera
            SerializedObject waitSerializedObject = new SerializedObject(waitEvent);
            waitSerializedObject.FindProperty("canBeSkipped").boolValue = true;
            waitSerializedObject.FindProperty("duration").floatValue = 3f;
            waitSerializedObject.ApplyModifiedProperties();

            // Configura o evento de diálogo
            SerializedObject dialogueSerializedObject = new SerializedObject(dialogueEvent);
            dialogueSerializedObject.FindProperty("canBeSkipped").boolValue = true;
            dialogueSerializedObject.FindProperty("duration").floatValue = 5f;

            var dialogueLines = dialogueSerializedObject.FindProperty("dialogueLines");
            dialogueLines.ClearArray();

            // Primeira linha
            dialogueLines.InsertArrayElementAtIndex(0);
            var line1 = dialogueLines.GetArrayElementAtIndex(0);
            line1.FindPropertyRelative("characterName").stringValue = "Player";
            line1.FindPropertyRelative("text").stringValue = "A noite passada foi assustadora! Ainda bem que achei esse lugar para me esconder.";
            line1.FindPropertyRelative("displayDuration").floatValue = 3f;

            // Segunda linha
            dialogueLines.InsertArrayElementAtIndex(1);
            var line2 = dialogueLines.GetArrayElementAtIndex(1);
            line2.FindPropertyRelative("characterName").stringValue = "Player";
            line2.FindPropertyRelative("text").stringValue = "Vamos ver se as coisas estão mais calmas.";
            line2.FindPropertyRelative("displayDuration").floatValue = 2f;

            dialogueSerializedObject.ApplyModifiedProperties();

            // Cria e configura a cutscene principal
            var cutscene = ScriptableObject.CreateInstance<CutsceneDefinition>();
            AssetDatabase.CreateAsset(cutscene, "Assets/Resources/Cutscenes/OpeningCutscene.asset");

            SerializedObject cutsceneSerializedObject = new SerializedObject(cutscene);
            var events = cutsceneSerializedObject.FindProperty("events");
            events.ClearArray();

            events.InsertArrayElementAtIndex(0);
            events.GetArrayElementAtIndex(0).objectReferenceValue = wakeUpEvent;

            events.InsertArrayElementAtIndex(1);
            events.GetArrayElementAtIndex(1).objectReferenceValue = waitEvent;

            events.InsertArrayElementAtIndex(2);
            events.GetArrayElementAtIndex(2).objectReferenceValue = dialogueEvent;

            cutsceneSerializedObject.FindProperty("disablePlayerControl").boolValue = true;
            cutsceneSerializedObject.FindProperty("canBeSkipped").boolValue = true;

            cutsceneSerializedObject.ApplyModifiedProperties();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Cutscene inicial criada com sucesso!");
        }

        private static void CreateRequiredFolders()
        {
            string[] folders = new[]
            {
                "Assets/Resources",
                "Assets/Resources/Cutscenes",
                "Assets/Resources/Cutscenes/Events"
            };

            foreach (var folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    string parentPath = Path.GetDirectoryName(folder).Replace('\\', '/');
                    string folderName = Path.GetFileName(folder);
                    AssetDatabase.CreateFolder(parentPath, folderName);
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
