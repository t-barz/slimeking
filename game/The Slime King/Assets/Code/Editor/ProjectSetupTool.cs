using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace ExtraTools
{
    public class ProjectSetupTool : EditorWindow
    {
        [MenuItem("Extra Tools/Setup Projeto Completo")]
        public static void ShowWindow()
        {
            GetWindow<ProjectSetupTool>("Setup Projeto");
        }

        private void OnGUI()
        {
            GUILayout.Label("Setup do Projeto - The Slime King", EditorStyles.boldLabel);

            GUILayout.Space(10);

            if (GUILayout.Button("1. Criar Estrutura de Pastas", GUILayout.Height(30)))
            {
                CreateProjectStructure();
            }

            if (GUILayout.Button("2. Configurar Unity Settings", GUILayout.Height(30)))
            {
                ConfigureUnitySettings();
            }

            if (GUILayout.Button("3. Setup Completo", GUILayout.Height(40)))
            {
                SetupCompleto();
            }

            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Execute o Setup Completo para configurar tudo automaticamente", MessageType.Info);
        }

        private static void CreateProjectStructure()
        {
            // Estrutura simplificada para desenvolvedor solo
            Dictionary<string, string[]> folderStructure = new Dictionary<string, string[]>
            {
                { "Art", new string[] { "Sprites", "Materials", "Animations" } },
                { "Art/Animations", new string[] { "Controllers", "Clips" } },
                { "Audio", new string[] { "Music", "SFX" } },
                { "Code", new string[] { "Editor", "Gameplay", "Systems" } },
                { "Game", new string[] { "Scenes", "Prefabs", "Data" } },
                { "Settings", new string[] { "PostProcessing" } },
                { "External", new string[] { "AssetStore", "Plugins", "Libraries", "Tools" } }
            };

            foreach (var folder in folderStructure)
            {
                string mainFolder = "Assets/" + folder.Key;

                if (!AssetDatabase.IsValidFolder(mainFolder))
                {
                    string guid = AssetDatabase.CreateFolder("Assets", folder.Key);
                    Debug.Log($"Pasta criada: {mainFolder}");
                }

                foreach (var subFolder in folder.Value)
                {
                    string subFolderPath = mainFolder + "/" + subFolder;
                    if (!AssetDatabase.IsValidFolder(subFolderPath))
                    {
                        string[] parts = subFolder.Split('/');
                        string currentPath = mainFolder;

                        foreach (string part in parts)
                        {
                            string newPath = currentPath + "/" + part;
                            if (!AssetDatabase.IsValidFolder(newPath))
                            {
                                AssetDatabase.CreateFolder(currentPath, part);
                                Debug.Log($"Subpasta criada: {newPath}");
                            }
                            currentPath = newPath;
                        }
                    }
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("✅ Estrutura de pastas criada com sucesso!");
        }

        private static void ConfigureUnitySettings()
        {
            // Configurações Unity 6 + URP

            // Player Settings
            PlayerSettings.defaultScreenWidth = 3840; // 4K Width
            PlayerSettings.defaultScreenHeight = 2160; // 4K Height
            PlayerSettings.runInBackground = true;

            // Quality Settings para Pixel Art
            QualitySettings.antiAliasing = 0;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.vSyncCount = 1;

            // Physics2D Settings
            Physics2D.velocityIterations = 6;
            Physics2D.positionIterations = 2;
            Physics2D.gravity = new Vector2(0, -9.81f);

            Debug.Log("✅ Unity Settings configurados!");
        }

        private static void SetupCompleto()
        {
            CreateProjectStructure();
            ConfigureUnitySettings();

            EditorUtility.DisplayDialog("Setup Completo",
                "Projeto configurado com sucesso!\n\n" +
                "Próximos passos:\n" +
                "1. Configure URP se ainda não estiver ativo\n" +
                "2. Comece a desenvolver!",
                "OK");
        }
    }
}