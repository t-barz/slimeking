using UnityEngine;
using UnityEditor;
using TheSlimeKing.Quest;
using System.IO;

namespace TheSlimeKing.Editor.Quest
{
    /// <summary>
    /// Ferramenta de criação rápida de quests via menu Tools
    /// </summary>
    public static class QuestCreationTool
    {
        /// <summary>
        /// Cria um novo CollectQuestData via menu Tools
        /// </summary>
        [MenuItem("Tools/Quest System/Create Collect Quest")]
        public static void CreateCollectQuest()
        {
            // Abre diálogo para escolher localização
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Collect Quest",
                "NewCollectQuest",
                "asset",
                "Escolha onde salvar a nova quest",
                "Assets/Data/Quests"
            );
            
            // Se usuário cancelou, retorna
            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("[QuestCreationTool] Criação de quest cancelada.");
                return;
            }
            
            // Cria novo CollectQuestData
            CollectQuestData newQuest = ScriptableObject.CreateInstance<CollectQuestData>();
            
            // Configura valores padrão
            string fileName = Path.GetFileNameWithoutExtension(path);
            newQuest.questName = fileName;
            newQuest.description = "Descrição da quest aqui...";
            newQuest.quantityRequired = 1;
            newQuest.reputationReward = 10;
            newQuest.isRepeatable = false;
            
            // Cria asset
            AssetDatabase.CreateAsset(newQuest, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            // Seleciona o asset criado
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = newQuest;
            
            Debug.Log($"[QuestCreationTool] Quest criada com sucesso: {path}");
        }
        
        /// <summary>
        /// Cria pasta de Quests se não existir
        /// </summary>
        [MenuItem("Tools/Quest System/Create Quest Folder Structure")]
        public static void CreateQuestFolderStructure()
        {
            string basePath = "Assets/Data/Quests";
            
            // Cria pasta base se não existir
            if (!AssetDatabase.IsValidFolder(basePath))
            {
                // Cria Assets/Data se não existir
                if (!AssetDatabase.IsValidFolder("Assets/Data"))
                {
                    AssetDatabase.CreateFolder("Assets", "Data");
                }
                
                // Cria Assets/Data/Quests
                AssetDatabase.CreateFolder("Assets/Data", "Quests");
                
                Debug.Log($"[QuestCreationTool] Estrutura de pastas criada: {basePath}");
            }
            else
            {
                Debug.Log($"[QuestCreationTool] Estrutura de pastas já existe: {basePath}");
            }
            
            AssetDatabase.Refresh();
        }
    }
}
