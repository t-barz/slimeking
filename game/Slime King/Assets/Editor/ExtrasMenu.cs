using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class ExtrasMenu
{
    [MenuItem("Extras/Criar Estrutura de Pastas")]
    public static void GenerateFolders()
    {
        List<string> folders = new List<string>
        {
            "Art",
            "Art/Characters",
            "Art/Characters/Player",
            "Art/Characters/Enemies",
            "Art/Environment",
            "Art/Environment/Tilesets",
            "Art/Environment/Props",
            "Art/Environment/Backgrounds",
            "Art/UI",
            "Art/UI/Icons",
            "Art/UI/Layouts",
            "Audio",
            "Audio/Music",
            "Audio/SFX",
            "Code",
            "Code/Core",
            "Code/Gameplay",
            "Code/UI",
            "Prefabs",
            "Prefabs/Characters",
            "Prefabs/Environment",
            "Prefabs/UI",
            "Scenes",
            "Settings",
            "Shaders",
            "Testing"
        };

        foreach (string folder in folders)
        {
            string path = $"Assets/{folder}";
            CreateFolderRecursive(path);
        }

        AssetDatabase.Refresh();
        Debug.Log("✅ Estrutura de pastas criada!");
    }

        private static void CreateFolderRecursive(string path)
    {
        string[] folders = path.Split('/');
        string parentPath = "";
        string currentPath = "";
    
        for (int i = 0; i < folders.Length; i++)
        {
            string folder = folders[i];
            
            if (i == 0) // Assets folder
            {
                currentPath = folder;
                continue;
            }
    
            parentPath = currentPath;
            currentPath += "/" + folder;
    
            if (!AssetDatabase.IsValidFolder(currentPath))
            {
                string parentFolder = parentPath;
                string newFolderName = folder;
                
                AssetDatabase.CreateFolder(parentFolder, newFolderName);
                Debug.Log($"Criando pasta: {currentPath}");
            }
        }
    }

    /// <summary>
    /// Adiciona um item de menu "Quick Group Selected" ao menu GameObject no Unity.
    /// Atalho: Ctrl+G (Cmd+G no macOS).
    /// </summary>
    [MenuItem("GameObject/Quick Group Selected %g")] // Ctrl+G
    private static void GroupSelected()
    {
        if (!Selection.activeTransform) return;
        
        // Cria um GameObject vazio para servir como grupo
        var groupObj = new GameObject(Selection.activeTransform.name + " Group");
        Undo.RegisterCreatedObjectUndo(groupObj, "Group Selected");
        
        // Define o pai como o mesmo dos objetos selecionados
        groupObj.transform.SetParent(Selection.activeTransform.parent, false);
        
        // Move os objetos selecionados para o grupo
        foreach (var transform in Selection.transforms)
        {
            Undo.SetTransformParent(transform, groupObj.transform, "Group Selected");
        }
        
        // Seleciona o novo grupo
        Selection.activeGameObject = groupObj;
    }
}
