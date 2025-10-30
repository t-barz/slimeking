using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using SlimeMec.Gameplay;

namespace SlimeKing.Editor
{
    /// <summary>
    /// Classe estática responsável por gerar ScriptableObjects de dados para NPCs.
    /// Cria NPCData, FriendshipData e DialogueData com valores padrão e estrutura organizada.
    /// </summary>
    public static class NPCDataGenerator
    {
        // Constantes de caminhos de assets
        private const string NPC_DATA_PATH = "Assets/Data/NPCs/";
        private const string FRIENDSHIP_DATA_PATH = "Assets/Data/NPCs/Friendship/";
        private const string DIALOGUE_DATA_PATH = "Assets/Data/NPCs/Dialogues/";
        private const string ANIMATOR_PATH = "Assets/Art/Animations/NPCs/";

        #region Directory Management (Subtask 5.1)

        /// <summary>
        /// Garante que um diretório existe, criando-o se necessário.
        /// </summary>
        /// <param name="path">Caminho do diretório a ser criado</param>
        /// <returns>Caminho do diretório criado ou existente</returns>
        public static string GetOrCreateDirectory(string path)
        {
            // Remove trailing slash se existir
            path = path.TrimEnd('/', '\\');

            // Verifica se o diretório já existe
            if (AssetDatabase.IsValidFolder(path))
            {
                return path;
            }

            // Divide o caminho em partes
            string[] folders = path.Split('/');
            string currentPath = folders[0];

            // Cria cada pasta na hierarquia
            for (int i = 1; i < folders.Length; i++)
            {
                string newPath = currentPath + "/" + folders[i];
                
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    string guid = AssetDatabase.CreateFolder(currentPath, folders[i]);
                    if (string.IsNullOrEmpty(guid))
                    {
                        Debug.LogError($"❌ Failed to create folder: {newPath}");
                        return null;
                    }
                    Debug.Log($"✅ Created directory: {newPath}");
                }
                
                currentPath = newPath;
            }

            return path;
        }

        #endregion

        #region ScriptableObject Creation Methods (Subtask 5.2)

        /// <summary>
        /// Cria um NPCData ScriptableObject com todos os campos populados.
        /// </summary>
        /// <param name="configData">Dados de configuração do NPC</param>
        /// <param name="friendshipData">Referência ao FriendshipData (opcional)</param>
        /// <param name="dialogueData">Referência ao DialogueData (opcional)</param>
        /// <returns>NPCData criado ou null se falhar</returns>
        public static NPCData CreateNPCData(NPCConfigData configData, FriendshipData friendshipData = null, DialogueData dialogueData = null)
        {
            if (configData == null || string.IsNullOrEmpty(configData.npcName))
            {
                Debug.LogError("❌ NPCConfigData is null or npcName is empty");
                return null;
            }

            // Garante que o diretório existe
            GetOrCreateDirectory(NPC_DATA_PATH);

            // Cria o caminho do asset
            string assetPath = $"{NPC_DATA_PATH}{configData.npcName}Data.asset";

            // Verifica se já existe e trata duplicatas (Subtask 5.3)
            assetPath = HandleExistingAsset(assetPath, configData.npcName, "Data");

            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogWarning("⚠️ User cancelled NPCData creation");
                return null;
            }

            // Cria o ScriptableObject
            NPCData npcData = ScriptableObject.CreateInstance<NPCData>();

            // Popula campos básicos
            npcData.npcName = configData.npcName;
            npcData.species = configData.speciesName;
            npcData.behaviorType = configData.behaviorType;
            npcData.aiType = configData.aiType;
            npcData.detectionRange = configData.detectionRange;

            // Popula configurações de IA
            if (configData.aiSettings != null)
            {
                npcData.wanderRadius = configData.aiSettings.wanderRadius;
                npcData.wanderSpeed = configData.aiSettings.wanderSpeed;
                npcData.patrolPoints = new List<Vector2>(configData.aiSettings.patrolPoints);
                npcData.patrolSpeed = configData.aiSettings.patrolSpeed;
            }

            // Referencia dados de sistemas
            npcData.friendshipData = friendshipData;
            npcData.dialogueData = dialogueData;

            // Salva o asset
            SaveAsset(npcData, assetPath);

            Debug.Log($"✅ NPCData created at: {assetPath}");
            return npcData;
        }

        /// <summary>
        /// Cria um FriendshipData ScriptableObject com 6 níveis padrão em português.
        /// </summary>
        /// <param name="speciesName">Nome da espécie</param>
        /// <returns>FriendshipData criado ou null se falhar</returns>
        public static FriendshipData CreateFriendshipData(string speciesName)
        {
            if (string.IsNullOrEmpty(speciesName))
            {
                Debug.LogError("❌ Species name is empty");
                return null;
            }

            // Garante que o diretório existe
            GetOrCreateDirectory(FRIENDSHIP_DATA_PATH);

            // Cria o caminho do asset
            string assetPath = $"{FRIENDSHIP_DATA_PATH}{speciesName}FriendshipData.asset";

            // Verifica se já existe
            FriendshipData existingData = AssetDatabase.LoadAssetAtPath<FriendshipData>(assetPath);
            if (existingData != null)
            {
                Debug.Log($"✅ Reusing existing FriendshipData at: {assetPath}");
                return existingData;
            }

            // Cria o ScriptableObject
            FriendshipData friendshipData = ScriptableObject.CreateInstance<FriendshipData>();
            friendshipData.speciesName = speciesName;
            friendshipData.maxLevel = 5;

            // Cria os 6 níveis padrão (0-5)
            friendshipData.levels = new List<FriendshipLevel>
            {
                new FriendshipLevel
                {
                    level = 0,
                    title = "Desconhecido",
                    description = "Você não conhece esta criatura ainda.",
                    unlockedBenefits = new List<string> { "Nenhum benefício" }
                },
                new FriendshipLevel
                {
                    level = 1,
                    title = "Conhecido",
                    description = "A criatura reconhece sua presença.",
                    unlockedBenefits = new List<string> { "NPC não foge mais de você" }
                },
                new FriendshipLevel
                {
                    level = 2,
                    title = "Amigável",
                    description = "A criatura demonstra confiança em você.",
                    unlockedBenefits = new List<string> { "Desconto de 10% em trocas", "Pode receber presentes" }
                },
                new FriendshipLevel
                {
                    level = 3,
                    title = "Amigo",
                    description = "Vocês desenvolveram uma amizade verdadeira.",
                    unlockedBenefits = new List<string> { "Desconto de 20% em trocas", "Ajuda em combate ocasionalmente" }
                },
                new FriendshipLevel
                {
                    level = 4,
                    title = "Melhor Amigo",
                    description = "Uma amizade profunda e duradoura.",
                    unlockedBenefits = new List<string> { "Desconto de 30% em trocas", "Ajuda em combate frequentemente", "Acesso a quests especiais" }
                },
                new FriendshipLevel
                {
                    level = 5,
                    title = "Companheiro Leal",
                    description = "Um vínculo inquebrantável de lealdade e confiança.",
                    unlockedBenefits = new List<string> { "Desconto de 50% em trocas", "Sempre ajuda em combate", "Acesso a todas as quests especiais", "Pode ser invocado como aliado" }
                }
            };

            // Salva o asset
            SaveAsset(friendshipData, assetPath);

            Debug.Log($"✅ FriendshipData created at: {assetPath}");
            return friendshipData;
        }

        /// <summary>
        /// Cria um DialogueData ScriptableObject com diálogo placeholder em português.
        /// </summary>
        /// <param name="npcName">Nome do NPC</param>
        /// <param name="behaviorType">Tipo de comportamento do NPC</param>
        /// <returns>DialogueData criado ou null se falhar</returns>
        public static DialogueData CreateDialogueData(string npcName, BehaviorType behaviorType)
        {
            if (string.IsNullOrEmpty(npcName))
            {
                Debug.LogError("❌ NPC name is empty");
                return null;
            }

            // Garante que o diretório existe
            GetOrCreateDirectory(DIALOGUE_DATA_PATH);

            // Cria o caminho do asset
            string assetPath = $"{DIALOGUE_DATA_PATH}{npcName}DialogueData.asset";

            // Verifica se já existe e trata duplicatas
            assetPath = HandleExistingAsset(assetPath, npcName, "DialogueData");

            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogWarning("⚠️ User cancelled DialogueData creation");
                return null;
            }

            // Cria o ScriptableObject
            DialogueData dialogueData = ScriptableObject.CreateInstance<DialogueData>();
            dialogueData.npcName = npcName;

            // Cria nó de diálogo inicial placeholder
            DialogueNode initialNode = new DialogueNode
            {
                id = "start",
                text = $"Olá! Eu sou {npcName}.\n\n[Diálogo placeholder - edite este ScriptableObject para adicionar diálogos personalizados]",
                portrait = null,
                choices = new List<DialogueChoice>
                {
                    new DialogueChoice
                    {
                        choiceText = "Até logo!",
                        nextNodeId = "",
                        condition = ""
                    }
                }
            };

            // Se for Quest Giver, adiciona diálogo de quest
            if (behaviorType == BehaviorType.QuestGiver)
            {
                initialNode.text = $"Olá! Eu sou {npcName}.\n\nPreciso de sua ajuda com algo importante.\n\n[Diálogo placeholder - edite este ScriptableObject para adicionar quests e diálogos personalizados]";
                
                initialNode.choices.Insert(0, new DialogueChoice
                {
                    choiceText = "Como posso ajudar?",
                    nextNodeId = "quest_offer",
                    condition = ""
                });

                // Adiciona nó de oferta de quest
                DialogueNode questNode = new DialogueNode
                {
                    id = "quest_offer",
                    text = "[Configure a quest aqui no ScriptableObject]\n\nVocê aceita me ajudar?",
                    portrait = null,
                    choices = new List<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            choiceText = "Sim, vou ajudar!",
                            nextNodeId = "",
                            condition = ""
                        },
                        new DialogueChoice
                        {
                            choiceText = "Não agora.",
                            nextNodeId = "",
                            condition = ""
                        }
                    }
                };

                dialogueData.dialogueNodes.Add(questNode);
            }

            dialogueData.dialogueNodes.Add(initialNode);

            // Salva o asset
            SaveAsset(dialogueData, assetPath);

            Debug.Log($"✅ DialogueData created at: {assetPath}");
            return dialogueData;
        }

        /// <summary>
        /// Carrega um asset existente ou cria um novo se não existir.
        /// </summary>
        /// <typeparam name="T">Tipo do ScriptableObject</typeparam>
        /// <param name="path">Caminho do asset</param>
        /// <returns>Asset carregado ou criado</returns>
        public static T LoadOrCreateAsset<T>(string path) where T : ScriptableObject
        {
            // Tenta carregar asset existente
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (asset != null)
            {
                return asset;
            }

            // Cria novo asset
            asset = ScriptableObject.CreateInstance<T>();
            SaveAsset(asset, path);

            return asset;
        }

        /// <summary>
        /// Salva um ScriptableObject usando otimização de AssetDatabase.
        /// </summary>
        /// <param name="asset">ScriptableObject a ser salvo</param>
        /// <param name="path">Caminho onde salvar o asset</param>
        public static void SaveAsset(ScriptableObject asset, string path)
        {
            if (asset == null || string.IsNullOrEmpty(path))
            {
                Debug.LogError("❌ Cannot save asset: asset is null or path is empty");
                return;
            }

            try
            {
                // Cria ou atualiza o asset
                AssetDatabase.CreateAsset(asset, path);

                Debug.Log($"✅ Asset saved successfully: {path}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Failed to save asset at {path}: {e.Message}");
            }
        }

        /// <summary>
        /// Salva múltiplos ScriptableObjects em batch usando otimização de AssetDatabase.
        /// Performance: Agrupa todas as operações de I/O em uma única transação.
        /// </summary>
        /// <param name="assets">Lista de tuplas (asset, path) para salvar</param>
        public static void SaveAssetsBatch(List<(ScriptableObject asset, string path)> assets)
        {
            if (assets == null || assets.Count == 0)
            {
                Debug.LogWarning("⚠️ No assets to save in batch");
                return;
            }

            try
            {
                // Otimização: agrupa todas as operações de asset em uma única transação
                AssetDatabase.StartAssetEditing();

                foreach (var (asset, path) in assets)
                {
                    if (asset != null && !string.IsNullOrEmpty(path))
                    {
                        AssetDatabase.CreateAsset(asset, path);
                        Debug.Log($"✅ Asset saved in batch: {path}");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Failed to save assets in batch: {e.Message}");
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                
                // Salva e atualiza o banco de dados uma única vez
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        #endregion

        #region Asset Existence Checking (Subtask 5.3)

        /// <summary>
        /// Verifica se um asset já existe e pergunta ao usuário o que fazer.
        /// </summary>
        /// <param name="assetPath">Caminho do asset</param>
        /// <param name="assetName">Nome do asset</param>
        /// <param name="assetType">Tipo do asset (para mensagem)</param>
        /// <returns>Caminho final do asset ou null se cancelado</returns>
        private static string HandleExistingAsset(string assetPath, string assetName, string assetType)
        {
            // Verifica se o asset já existe
            if (!File.Exists(assetPath))
            {
                return assetPath;
            }

            // Pergunta ao usuário o que fazer
            int choice = EditorUtility.DisplayDialogComplex(
                $"{assetType} Already Exists",
                $"An asset already exists at:\n{assetPath}\n\nWhat would you like to do?",
                "Overwrite",
                "Cancel",
                "Create New"
            );

            switch (choice)
            {
                case 0: // Overwrite
                    Debug.Log($"⚠️ Overwriting existing asset: {assetPath}");
                    // Deleta o asset existente
                    AssetDatabase.DeleteAsset(assetPath);
                    return assetPath;

                case 1: // Cancel
                    Debug.Log($"⚠️ User cancelled {assetType} creation");
                    return null;

                case 2: // Create New
                    // Gera novo nome com sufixo numérico
                    string newPath = GenerateUniqueAssetPath(assetPath, assetName, assetType);
                    Debug.Log($"✅ Creating new asset with unique name: {newPath}");
                    return newPath;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Gera um caminho de asset único adicionando sufixo numérico.
        /// </summary>
        /// <param name="originalPath">Caminho original do asset</param>
        /// <param name="assetName">Nome base do asset</param>
        /// <param name="assetType">Tipo do asset</param>
        /// <returns>Caminho único com sufixo numérico</returns>
        private static string GenerateUniqueAssetPath(string originalPath, string assetName, string assetType)
        {
            string directory = Path.GetDirectoryName(originalPath);
            string extension = Path.GetExtension(originalPath);
            int suffix = 1;

            string newPath;
            do
            {
                newPath = $"{directory}/{assetName}_{suffix}{extension}";
                suffix++;
            }
            while (File.Exists(newPath));

            return newPath;
        }

        #endregion
    }
}
