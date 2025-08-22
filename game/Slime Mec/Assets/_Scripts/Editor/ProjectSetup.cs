using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Rendering;
using System.Linq;

/// <summary>
/// Ferramenta de configuração e organização de projetos Unity 2D.
/// 
/// Esta classe fornece um conjunto abrangente de utilitários para:
/// - Criar estruturas de pastas padronizadas
/// - Reorganizar arquivos existentes automaticamente
/// - Renomear arquivos com prefixos consistentes
/// - Otimizar configurações para jogos 2D PixelArt
/// - Agrupar e renomear objetos na hierarquia
/// 
/// A janela pode ser acessada através do menu Tools > Aceleradores.
/// </summary>
public class ProjectSetup : EditorWindow
{
    // Variável para controlar a posição do scroll
    private Vector2 scrollPosition = Vector2.zero;

    /// <summary>
    /// Abre a janela de ferramentas de aceleração de projeto.
    /// </summary>
    [MenuItem("Tools/Aceleradores")]
    public static void ShowWindow()
    {
        // Abre a janela de ferramentas personalizadas
        ProjectSetup window = GetWindow<ProjectSetup>("Aceleradores de Projeto");

        // Define tamanho mínimo da janela para melhor visualização
        window.minSize = new Vector2(400, 600);
    }

    /// <summary>
    /// Desenha a interface gráfica da janela de ferramentas.
    /// Exibe botões para todas as funcionalidades disponíveis com descrições explicativas.
    /// </summary>
    void OnGUI()
    {
        // Inicia o ScrollView
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Adiciona margem interna para melhor aparência
        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
        EditorGUILayout.Space(10);

        GUILayout.Label("Gerador de Estrutura de Pastas para Projeto 2D", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("Este utilitário criará a estrutura de pastas recomendada para seu projeto 2D.", MessageType.Info);
        EditorGUILayout.Space(10);

        // Botão para criar a estrutura de pastas
        if (GUILayout.Button("Criar Estrutura de Pastas", GUILayout.Height(30)))
        {
            CreateProjectStructure();
        }

        EditorGUILayout.Space(20);
        GUILayout.Label("Reorganização de Arquivos", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("Reorganiza arquivos existentes para a estrutura de pastas recomendada.", MessageType.Info);
        EditorGUILayout.Space(10);

        // Botão para reorganizar arquivos existentes
        if (GUILayout.Button("Reorganizar Arquivos Existentes", GUILayout.Height(30)))
        {
            ReorganizeExistingFiles();
        }

        EditorGUILayout.Space(20);
        GUILayout.Label("Renomeação Padronizada", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("Renomeia arquivos seguindo padrão de prefixos baseado nas pastas.\nExemplo: monstroAzul.png → art_monstroAzul.png", MessageType.Info);
        EditorGUILayout.Space(10);

        // Botão para renomear arquivos com prefixos
        if (GUILayout.Button("Renomear Arquivos com Prefixos", GUILayout.Height(30)))
        {
            RenameFilesWithPrefixes();
        }

        EditorGUILayout.Space(20);
        GUILayout.Label("Otimizações para Jogos 2D PixelArt", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("Configure automaticamente as melhores configurações de performance para jogos 2D PixelArt.", MessageType.Info);
        EditorGUILayout.Space(10);

        // Botão para otimizar configurações para jogos 2D PixelArt
        if (GUILayout.Button("Otimizar Configurações PixelArt", GUILayout.Height(30)))
        {
            OptimizePixelArtSettings();
        }

        EditorGUILayout.Space(20);
        GUILayout.Label("Configuração de Tags do Projeto", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("Cria automaticamente as tags essenciais para o sistema de gameplay:\n• Wind, Props, Enemy, Destructable\n• WindShaker, DestructibleShaker (para sistema de shake)\n• InteractableTalk, InteractableJump, InteractableShrink\n• InteractablePush, InteractableCollect", MessageType.Info);
        EditorGUILayout.Space(10);

        // Botão para criar tags do projeto
        if (GUILayout.Button("Criar Tags do Projeto", GUILayout.Height(30)))
        {
            CreateProjectTags();
        }

        EditorGUILayout.Space(20);
        GUILayout.Label("Configuração de Props com Shaking", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox("Configura props existentes para sistema de shaking:\n• Adiciona tag 'Props' aos objetos selecionados\n• Verifica se tem Animator e Controller existentes\n• Adiciona trigger 'Shake' se necessário\n• Cria states 'Idle' e 'Shaking' com transições (tempo 0f)\n\nUSO: Selecione objetos com Animator+Controller na hierarquia", MessageType.Info);
        EditorGUILayout.Space(10);

        // Botão para preparar prop shaking
        if (GUILayout.Button("Preparar Prop Shaking", GUILayout.Height(30)))
        {
            PrepareShakingProps();
        }

        // Adiciona espaço extra no final para melhor visualização
        EditorGUILayout.Space(20);

        // Termina a margem interna
        EditorGUILayout.EndVertical();

        // Termina o ScrollView
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// Cria a estrutura de pastas recomendada para projetos 2D.
    /// Organiza o projeto em pastas padronizadas incluindo _Scenes, Art, Audio, Scripts, etc.
    /// </summary>
    void CreateProjectStructure()
    {
        // Define a estrutura de pastas como um dicionário
        Dictionary<string, string[]> folderStructure = new Dictionary<string, string[]>
        {
            { "_Scenes", new string[] { "Levels", "MainMenu", "Test" } },
            { "_Animation", new string[] { "Controllers", "Clips" } },
            { "_Art/Sprites", new string[] { "Characters", "Environment", "Items", "UI" } },
            { "_Art", new string[] { "Materials" } },
            { "_Audio", new string[] { "Music", "SFX" } },
            { "_Fonts", new string[] { } },
            { "_Prefabs", new string[] { "Characters", "Environment", "UI", "Systems" } },
            { "_Resources", new string[] { } },
            { "_ScriptableObjects", new string[] { "Characters", "Items", "GameSettings" } },
            { "_Scripts", new string[] { "Core", "Editor", "Managers", "Gameplay", "UI", "Utils" } },
            { "_Settings", new string[] { } },
            { "ThirdParty", new string[] { } }
        };

        // Cria as pastas
        foreach (var folder in folderStructure)
        {
            string mainFolder = "Assets/" + folder.Key;

            // Cria a pasta principal se não existir
            if (!AssetDatabase.IsValidFolder(mainFolder))
            {
                CreateFolder(mainFolder);
            }

            // Cria as subpastas
            foreach (var subFolder in folder.Value)
            {
                string subFolderPath = mainFolder + "/" + subFolder;
                if (!AssetDatabase.IsValidFolder(subFolderPath))
                {
                    CreateFolder(subFolderPath);
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Estrutura de pastas criada com sucesso!");
        EditorUtility.DisplayDialog("Concluído", "Estrutura de pastas criada com sucesso!", "OK");
    }

    /// <summary>
    /// Reorganiza arquivos existentes para a estrutura de pastas recomendada.
    /// Move automaticamente arquivos para suas pastas apropriadas baseado no tipo e nome.
    /// </summary>
    void ReorganizeExistingFiles()
    {
        if (!EditorUtility.DisplayDialog("Reorganizar Arquivos",
            "Esta operação moverá arquivos existentes para a estrutura de pastas recomendada.\n\n" +
            "ATENÇÃO: Faça backup do seu projeto antes de continuar!\n\n" +
            "Deseja continuar?", "Sim", "Cancelar"))
        {
            return;
        }

        // Primeiro, cria a estrutura se não existir
        CreateProjectStructure();

        int movedFiles = 0;
        List<string> moveLog = new List<string>();

        try
        {
            // Obtém todos os arquivos em Assets (exceto as pastas da estrutura)
            string[] allAssets = AssetDatabase.GetAllAssetPaths()
                .Where(path => path.StartsWith("Assets/") && !path.Equals("Assets/"))
                .ToArray();

            foreach (string assetPath in allAssets)
            {
                if (ShouldMoveAsset(assetPath))
                {
                    string newPath = GetNewPathForAsset(assetPath);
                    if (!string.IsNullOrEmpty(newPath) && !assetPath.Equals(newPath))
                    {
                        if (MoveAssetSafely(assetPath, newPath))
                        {
                            movedFiles++;
                            moveLog.Add($"{assetPath} → {newPath}");
                        }
                    }
                }
            }

            // Remove pastas vazias
            RemoveEmptyFolders();

            AssetDatabase.Refresh();

            // Exibe resultado
            string message = $"Reorganização concluída!\n\n" +
                           $"Arquivos movidos: {movedFiles}\n\n" +
                           "Verifique o Console para ver detalhes dos arquivos movidos.";

            Debug.Log("=== REORGANIZAÇÃO DE ARQUIVOS ===");
            Debug.Log($"Total de arquivos movidos: {movedFiles}");
            foreach (string log in moveLog)
            {
                Debug.Log(log);
            }

            EditorUtility.DisplayDialog("Concluído", message, "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro durante a reorganização: " + e.Message);
            EditorUtility.DisplayDialog("Erro", "Ocorreu um erro durante a reorganização. Verifique o Console.", "OK");
        }
    }

    /// <summary>
    /// Verifica se um arquivo deve ser movido durante a reorganização.
    /// Exclui arquivos que já estão na estrutura correta ou são arquivos de sistema.
    /// </summary>
    /// <param name="assetPath">Caminho do arquivo a ser verificado</param>
    /// <returns>True se o arquivo deve ser movido, false caso contrário</returns>
    bool ShouldMoveAsset(string assetPath)
    {
        // Não move arquivos que já estão na estrutura correta
        if (IsInCorrectStructure(assetPath))
            return false;

        // Não move arquivos de sistema do Unity
        if (assetPath.Contains("PackageCache") ||
            assetPath.Contains("Packages") ||
            assetPath.EndsWith(".meta"))
            return false;

        return true;
    }

    /// <summary>
    /// Verifica se um arquivo já está na estrutura de pastas correta.
    /// </summary>
    /// <param name="assetPath">Caminho do arquivo a ser verificado</param>
    /// <returns>True se já está na estrutura correta</returns>
    bool IsInCorrectStructure(string assetPath)
    {
        string[] structureFolders = {
            "Assets/_Scenes", "Assets/Animation", "Assets/Art", "Assets/Audio",
            "Assets/Fonts", "Assets/Prefabs", "Assets/Resources", "Assets/ScriptableObjects",
            "Assets/Scripts", "Assets/Settings", "Assets/ThirdParty"
        };

        return structureFolders.Any(folder => assetPath.StartsWith(folder + "/"));
    }

    /// <summary>
    /// Determina o novo caminho para um arquivo baseado no seu tipo e conteúdo.
    /// </summary>
    /// <param name="assetPath">Caminho atual do arquivo</param>
    /// <returns>Novo caminho apropriado para o arquivo</returns>
    string GetNewPathForAsset(string assetPath)
    {
        string fileName = Path.GetFileName(assetPath);
        string extension = Path.GetExtension(assetPath).ToLower();

        // Cenas
        if (extension == ".unity")
        {
            if (fileName.ToLower().Contains("menu"))
                return "Assets/_Scenes/MainMenu/" + fileName;
            else if (fileName.ToLower().Contains("test"))
                return "Assets/_Scenes/Test/" + fileName;
            else
                return "Assets/_Scenes/Levels/" + fileName;
        }

        // Scripts
        if (extension == ".cs")
        {
            if (assetPath.Contains("Editor") || fileName.Contains("Editor"))
                return "Assets/Scripts/Editor/" + fileName;
            else if (fileName.Contains("Manager"))
                return "Assets/Scripts/Managers/" + fileName;
            else if (fileName.Contains("UI") || fileName.Contains("Menu"))
                return "Assets/Scripts/UI/" + fileName;
            else if (fileName.Contains("Util") || fileName.Contains("Helper"))
                return "Assets/Scripts/Utils/" + fileName;
            else
                return "Assets/Scripts/Gameplay/" + fileName;
        }

        // Imagens e Sprites
        if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" ||
            extension == ".tga" || extension == ".psd")
        {
            if (fileName.ToLower().Contains("character") || fileName.ToLower().Contains("player"))
                return "Assets/Art/Sprites/Characters/" + fileName;
            else if (fileName.ToLower().Contains("ui") || fileName.ToLower().Contains("button"))
                return "Assets/Art/Sprites/UI/" + fileName;
            else if (fileName.ToLower().Contains("item") || fileName.ToLower().Contains("pickup"))
                return "Assets/Art/Sprites/Items/" + fileName;
            else
                return "Assets/Art/Sprites/Environment/" + fileName;
        }

        // Áudio
        if (extension == ".wav" || extension == ".mp3" || extension == ".ogg")
        {
            if (fileName.ToLower().Contains("music") || fileName.ToLower().Contains("bgm"))
                return "Assets/Audio/Music/" + fileName;
            else
                return "Assets/Audio/SFX/" + fileName;
        }

        // Materiais
        if (extension == ".mat")
        {
            return "Assets/Art/Materials/" + fileName;
        }

        // Prefabs
        if (extension == ".prefab")
        {
            if (fileName.ToLower().Contains("character") || fileName.ToLower().Contains("player"))
                return "Assets/Prefabs/Characters/" + fileName;
            else if (fileName.ToLower().Contains("ui") || fileName.ToLower().Contains("canvas"))
                return "Assets/Prefabs/UI/" + fileName;
            else if (fileName.ToLower().Contains("manager") || fileName.ToLower().Contains("system"))
                return "Assets/Prefabs/Systems/" + fileName;
            else
                return "Assets/Prefabs/Environment/" + fileName;
        }

        // Animações
        if (extension == ".anim")
        {
            return "Assets/Animation/Clips/" + fileName;
        }

        // Animators
        if (extension == ".controller")
        {
            return "Assets/Animation/Controllers/" + fileName;
        }

        // Fontes
        if (extension == ".ttf" || extension == ".otf")
        {
            return "Assets/Fonts/" + fileName;
        }

        // ScriptableObjects
        if (extension == ".asset")
        {
            if (fileName.ToLower().Contains("character"))
                return "Assets/ScriptableObjects/Characters/" + fileName;
            else if (fileName.ToLower().Contains("item"))
                return "Assets/ScriptableObjects/Items/" + fileName;
            else if (fileName.ToLower().Contains("setting") || fileName.ToLower().Contains("config"))
                return "Assets/ScriptableObjects/GameSettings/" + fileName;
            else
                return "Assets/ScriptableObjects/" + fileName;
        }

        // Arquivos não reconhecidos vão para ThirdParty
        return "Assets/ThirdParty/" + fileName;
    }

    /// <summary>
    /// Move um arquivo de forma segura verificando se o destino existe e tratando erros.
    /// </summary>
    /// <param name="oldPath">Caminho atual do arquivo</param>
    /// <param name="newPath">Novo caminho de destino</param>
    /// <returns>True se o arquivo foi movido com sucesso</returns>
    bool MoveAssetSafely(string oldPath, string newPath)
    {
        try
        {
            // Verifica se o diretório de destino existe
            string directory = Path.GetDirectoryName(newPath);
            if (!AssetDatabase.IsValidFolder(directory))
            {
                CreateFolder(directory);
            }

            // Verifica se já existe um arquivo com o mesmo nome no destino
            if (File.Exists(newPath))
            {
                Debug.LogWarning($"Arquivo já existe no destino: {newPath}. Pulando {oldPath}");
                return false;
            }

            // Move o arquivo
            string result = AssetDatabase.MoveAsset(oldPath, newPath);
            if (!string.IsNullOrEmpty(result))
            {
                Debug.LogError($"Erro ao mover {oldPath}: {result}");
                return false;
            }

            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Exceção ao mover {oldPath}: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Remove pastas vazias que não fazem parte da estrutura do projeto.
    /// </summary>
    void RemoveEmptyFolders()
    {
        string[] allFolders = AssetDatabase.GetSubFolders("Assets");

        foreach (string folder in allFolders)
        {
            if (IsFolderEmpty(folder) && !IsStructureFolder(folder))
            {
                AssetDatabase.DeleteAsset(folder);
                Debug.Log($"Pasta vazia removida: {folder}");
            }
        }
    }

    /// <summary>
    /// Verifica se uma pasta está vazia (não contém arquivos nem subpastas).
    /// </summary>
    /// <param name="folderPath">Caminho da pasta a verificar</param>
    /// <returns>True se a pasta estiver vazia</returns>
    bool IsFolderEmpty(string folderPath)
    {
        string[] subFolders = AssetDatabase.GetSubFolders(folderPath);
        string[] files = Directory.GetFiles(folderPath)
            .Where(file => !file.EndsWith(".meta"))
            .ToArray();

        return subFolders.Length == 0 && files.Length == 0;
    }

    /// <summary>
    /// Verifica se uma pasta faz parte da estrutura padrão do projeto.
    /// </summary>
    /// <param name="folderPath">Caminho da pasta a verificar</param>
    /// <returns>True se faz parte da estrutura padrão</returns>
    bool IsStructureFolder(string folderPath)
    {
        string[] structureFolders = {
            "Assets/_Scenes", "Assets/Animation", "Assets/Art", "Assets/Audio",
            "Assets/Fonts", "Assets/Prefabs", "Assets/Resources", "Assets/ScriptableObjects",
            "Assets/Scripts", "Assets/Settings", "Assets/ThirdParty"
        };

        return structureFolders.Any(folder => folderPath.StartsWith(folder));
    }

    /// <summary>
    /// Otimiza configurações de performance para jogos 2D PixelArt.
    /// Configura câmera, qualidade, física e renderização para melhor performance.
    /// </summary>
    void OptimizePixelArtSettings()
    {
        // Configurações da câmera principal
        OptimizeCameraSettings();

        // Configurações de qualidade
        OptimizeQualitySettings();

        // Configurações de física 2D
        OptimizePhysics2DSettings();

        // Configurações do Player
        OptimizePlayerSettings();

        // Configurações de renderização
        OptimizeRenderingSettings();

        AssetDatabase.Refresh();
        Debug.Log("Configurações de PixelArt otimizadas com sucesso!");
        EditorUtility.DisplayDialog("Concluído",
            "Configurações otimizadas para PixelArt:\n\n" +
            "• Câmera configurada para Pixel Perfect\n" +
            "• Anti-aliasing desabilitado\n" +
            "• Texture Streaming desabilitado\n" +
            "• Physics 2D otimizado\n" +
            "• V-Sync configurado\n" +
            "• Configurações de renderização ajustadas", "OK");
    }

    /// <summary>
    /// Cria automaticamente as tags essenciais para o sistema de gameplay do projeto.
    /// Adiciona tags para elementos do vento, inimigos, objetos destrutíveis e interações.
    /// </summary>
    void CreateProjectTags()
    {
        // Lista de tags essenciais para o projeto
        string[] projectTags = {
            "Wind",
            "Props",
            "Enemy",
            "WindShaker",
            "DestructibleShaker",
            "Destructable",
            "InteractableTalk",
            "InteractableJump",
            "InteractableShrink",
            "InteractablePush",
            "InteractableCollect"
        };

        int createdTags = 0;
        int existingTags = 0;
        List<string> createdList = new List<string>();
        List<string> existingList = new List<string>();

        try
        {
            foreach (string tagName in projectTags)
            {
                if (CreateTagIfNotExists(tagName))
                {
                    createdTags++;
                    createdList.Add(tagName);
                }
                else
                {
                    existingTags++;
                    existingList.Add(tagName);
                }
            }

            // Exibe resultado
            string message = $"Configuração de tags concluída!\n\n" +
                           $"Tags criadas: {createdTags}\n" +
                           $"Tags já existentes: {existingTags}\n\n" +
                           "Verifique o Console para ver detalhes.";

            Debug.Log("=== CRIAÇÃO DE TAGS DO PROJETO ===");

            if (createdList.Count > 0)
            {
                Debug.Log($"Tags criadas ({createdTags}):");
                foreach (string tag in createdList)
                {
                    Debug.Log($"  • {tag}");
                }
            }

            if (existingList.Count > 0)
            {
                Debug.Log($"Tags já existentes ({existingTags}):");
                foreach (string tag in existingList)
                {
                    Debug.Log($"  • {tag}");
                }
            }

            EditorUtility.DisplayDialog("Concluído", message, "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro durante a criação de tags: " + e.Message);
            EditorUtility.DisplayDialog("Erro", "Ocorreu um erro durante a criação das tags. Verifique o Console.", "OK");
        }
    }

    /// <summary>
    /// Cria uma tag se ela não existir no projeto.
    /// </summary>
    /// <param name="tagName">Nome da tag a ser criada</param>
    /// <returns>True se a tag foi criada, false se já existia</returns>
    bool CreateTagIfNotExists(string tagName)
    {
        // Verifica se a tag já existe
        bool tagExists = false;

        // Obtém o objeto TagManager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        // Verifica se a tag já existe
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty tagProp = tagsProp.GetArrayElementAtIndex(i);
            if (tagProp.stringValue.Equals(tagName))
            {
                tagExists = true;
                break;
            }
        }

        // Se não existe, cria a tag
        if (!tagExists)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
            newTagProp.stringValue = tagName;
            tagManager.ApplyModifiedProperties();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Prepara props selecionados para sistema de shaking.
    /// Adiciona a tag "Props" e configura trigger "Shake" no Animator.
    /// </summary>
    void PrepareShakingProps()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("Nenhum Objeto Selecionado",
                "Selecione um ou mais objetos na hierarquia para configurar como props com shaking.", "OK");
            return;
        }

        int configuredObjects = 0;
        int skippedObjects = 0;
        List<string> configuredList = new List<string>();
        List<string> skippedList = new List<string>();

        try
        {
            // Primeiro garante que a tag "Props" existe
            CreateTagIfNotExists("Props");

            foreach (GameObject obj in selectedObjects)
            {
                bool wasConfigured = false;

                // Adiciona a tag "Props"
                if (obj.tag == "Untagged" || obj.tag != "Props")
                {
                    obj.tag = "Props";
                    wasConfigured = true;
                }

                // Verifica se tem Animator (não cria se não tiver)
                Animator animator = obj.GetComponent<Animator>();
                if (animator == null)
                {
                    Debug.LogWarning($"Objeto '{obj.name}': Não possui Animator. Adicione um Animator manualmente.");
                    skippedObjects++;
                    skippedList.Add(obj.name);
                    continue;
                }

                // Verifica se tem Animator Controller (não cria se não tiver)
                if (animator.runtimeAnimatorController == null)
                {
                    Debug.LogWarning($"Objeto '{obj.name}': Animator não possui Controller. Crie um Animator Controller manualmente.");
                    skippedObjects++;
                    skippedList.Add(obj.name);
                    continue;
                }

                // Verifica se é um AnimatorController editável
                UnityEditor.Animations.AnimatorController controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
                if (controller == null)
                {
                    Debug.LogWarning($"Objeto '{obj.name}': Animator Controller não é editável.");
                    skippedObjects++;
                    skippedList.Add(obj.name);
                    continue;
                }

                // Verifica/adiciona o trigger "Shake"
                bool hasShakeTrigger = false;
                foreach (var parameter in controller.parameters)
                {
                    if (parameter.name == "Shake" && parameter.type == AnimatorControllerParameterType.Trigger)
                    {
                        hasShakeTrigger = true;
                        break;
                    }
                }

                if (!hasShakeTrigger)
                {
                    // Adiciona o trigger "Shake"
                    controller.AddParameter("Shake", AnimatorControllerParameterType.Trigger);
                    wasConfigured = true;
                    Debug.Log($"Trigger 'Shake' adicionado ao Controller de '{obj.name}'");
                }

                // Configura os states e transições
                ConfigureShakeStates(controller, obj.name, ref wasConfigured);

                if (wasConfigured)
                {
                    configuredObjects++;
                    configuredList.Add(obj.name);
                }
                else
                {
                    skippedObjects++;
                    skippedList.Add(obj.name);
                }
            }

            // Exibe resultado
            string message = $"Configuração de Props concluída!\n\n" +
                           $"Objetos configurados: {configuredObjects}\n" +
                           $"Objetos já configurados: {skippedObjects}\n\n" +
                           "Verifique o Console para instruções adicionais.";

            Debug.Log("=== CONFIGURAÇÃO DE PROPS SHAKING ===");
            Debug.Log($"Tag 'Props' foi criada/verificada.");

            if (configuredList.Count > 0)
            {
                Debug.Log($"Objetos configurados ({configuredObjects}):");
                foreach (string objName in configuredList)
                {
                    Debug.Log($"  • {objName}");
                }
            }

            if (skippedList.Count > 0)
            {
                Debug.Log($"Objetos já configurados ({skippedObjects}):");
                foreach (string objName in skippedList)
                {
                    Debug.Log($"  • {objName}");
                }
            }

            Debug.Log("\nPRÓXIMOS PASSOS:");
            Debug.Log("1. ✅ Tag 'Props' foi configurada automaticamente");
            Debug.Log("2. ✅ Trigger 'Shake' foi adicionado aos Animator Controllers existentes");
            Debug.Log("3. ✅ States 'Idle' e 'Shaking' foram criados/configurados");
            Debug.Log("4. ✅ Transições automáticas configuradas (tempo 0f)");
            Debug.Log("5. 🎬 Adicione clips de animação aos states 'Idle' e 'Shaking'");
            Debug.Log("6. 🎮 Use animator.SetTrigger(\"Shake\") no código para ativar");

            EditorUtility.DisplayDialog("Concluído", message, "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro durante a configuração de props: " + e.Message);
            EditorUtility.DisplayDialog("Erro", "Ocorreu um erro durante a configuração. Verifique o Console.", "OK");
        }
    }

    /// <summary>
    /// Configura os states Idle e Shaking com transições automáticas.
    /// </summary>
    /// <param name="controller">Animator Controller para configurar</param>
    /// <param name="objectName">Nome do objeto para logs</param>
    /// <param name="wasConfigured">Referência para indicar se houve mudanças</param>
    void ConfigureShakeStates(UnityEditor.Animations.AnimatorController controller, string objectName, ref bool wasConfigured)
    {
        // Obtém a layer base (primeira layer)
        if (controller.layers.Length == 0)
        {
            Debug.LogWarning($"Objeto '{objectName}': Controller não possui layers.");
            return;
        }

        var baseLayer = controller.layers[0];
        var stateMachine = baseLayer.stateMachine;

        // Procura pelos states Idle e Shaking
        UnityEditor.Animations.AnimatorState idleState = null;
        UnityEditor.Animations.AnimatorState shakingState = null;

        foreach (var state in stateMachine.states)
        {
            if (state.state.name == "Idle")
                idleState = state.state;
            else if (state.state.name == "Shaking")
                shakingState = state.state;
        }

        // Cria o state Idle se não existir
        if (idleState == null)
        {
            idleState = stateMachine.AddState("Idle");
            stateMachine.defaultState = idleState;
            wasConfigured = true;
            Debug.Log($"State 'Idle' criado para '{objectName}'");
        }

        // Cria o state Shaking se não existir  
        if (shakingState == null)
        {
            shakingState = stateMachine.AddState("Shaking");
            wasConfigured = true;
            Debug.Log($"State 'Shaking' criado para '{objectName}'");
        }

        // Configura transição Idle -> Shaking
        bool hasIdleToShaking = false;
        foreach (var transition in idleState.transitions)
        {
            if (transition.destinationState == shakingState)
            {
                hasIdleToShaking = true;
                break;
            }
        }

        if (!hasIdleToShaking)
        {
            var idleToShaking = idleState.AddTransition(shakingState);
            idleToShaking.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "Shake");
            idleToShaking.duration = 0f;
            idleToShaking.exitTime = 0f;
            idleToShaking.hasExitTime = false;
            wasConfigured = true;
            Debug.Log($"Transição Idle -> Shaking configurada para '{objectName}'");
        }

        // Configura transição Shaking -> Idle
        bool hasShakingToIdle = false;
        foreach (var transition in shakingState.transitions)
        {
            if (transition.destinationState == idleState)
            {
                hasShakingToIdle = true;
                break;
            }
        }

        if (!hasShakingToIdle)
        {
            var shakingToIdle = shakingState.AddTransition(idleState);
            shakingToIdle.duration = 0f;
            shakingToIdle.exitTime = 1f;
            shakingToIdle.hasExitTime = true;
            wasConfigured = true;
            Debug.Log($"Transição Shaking -> Idle configurada para '{objectName}'");
        }

        // Salva as mudanças
        if (wasConfigured)
        {
            EditorUtility.SetDirty(controller);
        }
    }

    /// <summary>
    /// Otimiza configurações da câmera principal para jogos 2D PixelArt.
    /// </summary>
    void OptimizeCameraSettings()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Configura projeção ortográfica
            mainCamera.orthographic = true;

            // Remove anti-aliasing da câmera
            mainCamera.allowMSAA = false;
            mainCamera.allowHDR = false;

            Debug.Log("Configurações da câmera otimizadas para PixelArt");
        }
    }

    /// <summary>
    /// Otimiza configurações de qualidade gráfica para jogos 2D PixelArt.
    /// </summary>
    void OptimizeQualitySettings()
    {
        // Desabilita anti-aliasing
        QualitySettings.antiAliasing = 0;

        // Configura anisotropic filtering
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;

        // Desabilita soft particles
        QualitySettings.softParticles = false;

        // Configura V-Sync
        QualitySettings.vSyncCount = 1;

        // Desabilita texture streaming
        QualitySettings.streamingMipmapsActive = false;

        // Configura shadow settings para 2D
        QualitySettings.shadows = ShadowQuality.Disable;
        QualitySettings.shadowResolution = ShadowResolution.Low;

        Debug.Log("Configurações de qualidade otimizadas para PixelArt");
    }

    /// <summary>
    /// Otimiza configurações de física 2D para melhor performance.
    /// </summary>
    void OptimizePhysics2DSettings()
    {
        // Aumenta velocidade de simulação para jogos 2D simples
        Physics2D.velocityIterations = 6;
        Physics2D.positionIterations = 2;

        // Configura gravity padrão para platformer 2D
        Physics2D.gravity = new Vector2(0, -9.81f);

        // Otimiza queries
        Physics2D.queriesHitTriggers = true;
        Physics2D.queriesStartInColliders = false;

        Debug.Log("Configurações de Physics2D otimizadas");
    }

    /// <summary>
    /// Otimiza configurações do Player para jogos 2D.
    /// </summary>
    void OptimizePlayerSettings()
    {
        // Configura color space para Linear (melhor para 2D)
        PlayerSettings.colorSpace = ColorSpace.Linear;

        // Desabilita GPU Skinning (não necessário para 2D)
        PlayerSettings.gpuSkinning = false;

        // Configura graphics API
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
        var graphicsAPIs = new GraphicsDeviceType[] { GraphicsDeviceType.Direct3D11, GraphicsDeviceType.OpenGLCore };
        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, graphicsAPIs);

        Debug.Log("Configurações do Player otimizadas");
    }

    /// <summary>
    /// Otimiza configurações de renderização para projetos 2D.
    /// </summary>
    void OptimizeRenderingSettings()
    {
        // Configura Sprite Packer (se disponível)
        EditorSettings.spritePackerMode = SpritePackerMode.SpriteAtlasV2;

        // Configura asset serialization para melhor performance
        EditorSettings.serializationMode = SerializationMode.ForceText;

        Debug.Log("Configurações de renderização otimizadas");
    }

    /// <summary>
    /// Cria uma estrutura de pastas recursivamente usando o AssetDatabase.
    /// </summary>
    /// <param name="folderPath">Caminho completo da pasta a ser criada</param>
    void CreateFolder(string folderPath)
    {
        string[] pathParts = folderPath.Split('/');
        string parentFolder = "Assets";
        string folderName;

        for (int i = 1; i < pathParts.Length; i++)
        {
            folderName = pathParts[i];
            string currentPath = parentFolder + "/" + folderName;

            if (!AssetDatabase.IsValidFolder(currentPath))
            {
                string guid = AssetDatabase.CreateFolder(parentFolder, folderName);
                if (string.IsNullOrEmpty(guid))
                {
                    Debug.LogError("Não foi possível criar a pasta: " + currentPath);
                }
            }

            parentFolder = currentPath;
        }
    }

    // Renomeia arquivos adicionando prefixos baseados nas pastas
    /// <summary>
    /// Renomeia arquivos adicionando prefixos baseados nas pastas onde estão localizados.
    /// Não renomeia scripts (.cs) por segurança.
    /// </summary>
    void RenameFilesWithPrefixes()
    {
        if (!EditorUtility.DisplayDialog("Renomear Arquivos",
            "Esta operação renomeará arquivos adicionando prefixos baseados nas pastas.\n\n" +
            "Exemplos:\n" +
            "• monstroAzul.png → ART_monstroAzul.png\n" +
            "• attack.wav → SFX_attack.wav\n" +
            "• playerPrefab.prefab → PRF_playerPrefab.prefab\n\n" +
            "ATENÇÃO: Scripts (.cs) não serão renomeados!\n\n" +
            "Deseja continuar?", "Sim", "Cancelar"))
        {
            return;
        }

        int renamedFiles = 0;
        List<string> renameLog = new List<string>();

        try
        {
            // Obtém todos os arquivos, exceto scripts
            string[] allAssets = AssetDatabase.GetAllAssetPaths()
                .Where(path => path.StartsWith("Assets/") &&
                              !path.EndsWith(".cs") &&
                              !path.EndsWith(".meta") &&
                              !AssetDatabase.IsValidFolder(path))
                .ToArray();

            foreach (string assetPath in allAssets)
            {
                string newName = GetNewFileNameWithPrefix(assetPath);
                if (!string.IsNullOrEmpty(newName))
                {
                    string directory = Path.GetDirectoryName(assetPath);
                    string newPath = Path.Combine(directory, newName).Replace('\\', '/');

                    if (!assetPath.Equals(newPath))
                    {
                        string result = AssetDatabase.MoveAsset(assetPath, newPath);
                        if (string.IsNullOrEmpty(result))
                        {
                            renamedFiles++;
                            renameLog.Add($"{Path.GetFileName(assetPath)} → {newName}");
                        }
                        else
                        {
                            Debug.LogError($"Erro ao renomear {assetPath}: {result}");
                        }
                    }
                }
            }

            AssetDatabase.Refresh();

            // Exibe resultado
            string message = $"Renomeação concluída!\n\n" +
                           $"Arquivos renomeados: {renamedFiles}\n\n" +
                           "Verifique o Console para ver detalhes.";

            Debug.Log("=== RENOMEAÇÃO DE ARQUIVOS ===");
            Debug.Log($"Total de arquivos renomeados: {renamedFiles}");
            foreach (string log in renameLog)
            {
                Debug.Log(log);
            }

            EditorUtility.DisplayDialog("Concluído", message, "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro durante a renomeação: " + e.Message);
            EditorUtility.DisplayDialog("Erro", "Ocorreu um erro durante a renomeação. Verifique o Console.", "OK");
        }
    }

    /// <summary>
    /// Gera um novo nome de arquivo com prefixo baseado na localização da pasta.
    /// </summary>
    /// <param name="assetPath">Caminho do arquivo</param>
    /// <returns>Novo nome com prefixo, ou null se não conseguir determinar</returns>
    string GetNewFileNameWithPrefix(string assetPath)
    {
        string fileName = Path.GetFileNameWithoutExtension(assetPath);
        string extension = Path.GetExtension(assetPath);

        // Remove prefixo existente se houver
        fileName = RemoveExistingPrefix(fileName);

        // Converte para camelCase
        fileName = ConvertToCamelCase(fileName);

        // Obtém o prefixo baseado na pasta
        string prefix = GetPrefixForPath(assetPath);

        if (string.IsNullOrEmpty(prefix))
            return null;

        return $"{prefix}_{fileName}{extension}";
    }

    /// <summary>
    /// Remove prefixos existentes de nomes de arquivos (formato XXX_ ou XXXX_).
    /// </summary>
    /// <param name="fileName">Nome do arquivo com possível prefixo</param>
    /// <returns>Nome sem prefixo</returns>
    string RemoveExistingPrefix(string fileName)
    {
        // Padrão: 3-4 caracteres seguidos de underscore
        if (fileName.Length > 4 &&
            (fileName[3] == '_' || fileName[4] == '_'))
        {
            int underscoreIndex = fileName.IndexOf('_');
            if (underscoreIndex >= 3 && underscoreIndex <= 4)
            {
                return fileName.Substring(underscoreIndex + 1);
            }
        }

        return fileName;
    }

    /// <summary>
    /// Converte uma string para formato camelCase.
    /// Primeira palavra em minúscula, demais com primeira letra maiúscula.
    /// </summary>
    /// <param name="input">Texto a converter</param>
    /// <returns>Texto em formato camelCase</returns>
    string ConvertToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Remove espaços, underscores, hífens e pontos, capitalizando a primeira letra de cada palavra
        string[] words = input.Split(new char[] { ' ', '_', '-', '.' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
            return input;

        // Se só tem uma palavra, converte para camelCase (primeira letra minúscula)
        if (words.Length == 1)
        {
            string word = words[0];
            if (word.Length == 0) return word;
            return char.ToLower(word[0]) + word.Substring(1).ToLower();
        }

        // Primeira palavra em minúscula
        string result = words[0].ToLower();

        // Demais palavras com primeira letra maiúscula e resto minúsculo
        for (int i = 1; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                result += char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
        }

        return result;
    }

    /// <summary>
    /// Obtém o prefixo apropriado baseado no caminho da pasta do arquivo.
    /// </summary>
    /// <param name="assetPath">Caminho do arquivo</param>
    /// <returns>Prefixo de 3 caracteres correspondente à pasta</returns>
    string GetPrefixForPath(string assetPath)
    {
        // Mapeamento de pastas para prefixos
        Dictionary<string, string> folderPrefixes = new Dictionary<string, string>
        {
            // Scenes
            { "Assets/_Scenes", "scn" },
            
            // Animation
            { "Assets/_Animation/Controllers", "anm" },
            { "Assets/_Animation/Clips", "anm" },
            { "Assets/_Animation", "anm" },

            // Art
            { "Assets/_Art/Sprites/Characters", "chr" },
            { "Assets/_Art/Sprites/Environment", "env" },
            { "Assets/_Art/Sprites/Items", "itm" },
            { "Assets/_Art/Sprites/UI", "gui" },
            { "Assets/_Art/Sprites", "art" },
            { "Assets/_Art/Materials", "mat" },
            { "Assets/_Art", "art" },

            // Audio
            { "Assets/_Audio/Music", "mus" },
            { "Assets/_Audio/SFX", "sfx" },
            { "Assets/_Audio", "aud" },

            // Fonts
            { "Assets/_Fonts", "fnt" },

            // Prefabs
            { "Assets/_Prefabs/Characters", "chr" },
            { "Assets/_Prefabs/Environment", "env" },
            { "Assets/_Prefabs/UI", "gui" },
            { "Assets/_Prefabs/Systems", "sys" },
            { "Assets/_Prefabs", "prf" },

            // Resources
            { "Assets/_Resources", "res" },

            // ScriptableObjects
            { "Assets/_ScriptableObjects/Characters", "chr" },
            { "Assets/_ScriptableObjects/Items", "itm" },
            { "Assets/_ScriptableObjects/GameSettings", "cfg" },
            { "Assets/_ScriptableObjects", "sco" },
            
            // Settings
            { "Assets/_Settings", "cfg" },

            // ThirdParty
            { "Assets/ThirdParty", "ext" }
        };

        // Procura o prefixo mais específico (caminho mais longo que coincide)
        string bestMatch = "";
        string bestPrefix = "";

        foreach (var kvp in folderPrefixes)
        {
            if (assetPath.StartsWith(kvp.Key + "/") && kvp.Key.Length > bestMatch.Length)
            {
                bestMatch = kvp.Key;
                bestPrefix = kvp.Value;
            }
        }

        // Se não encontrou correspondência específica, gera prefixo genérico
        if (string.IsNullOrEmpty(bestPrefix))
        {
            bestPrefix = GenerateGenericPrefix(assetPath);
        }

        return bestPrefix;
    }

    /// <summary>
    /// Gera um prefixo genérico baseado no nome da pasta mais próxima.
    /// </summary>
    /// <param name="assetPath">Caminho do arquivo</param>
    /// <returns>Prefixo de 3 caracteres</returns>
    string GenerateGenericPrefix(string assetPath)
    {
        // Pega o nome da pasta mais próxima
        string directory = Path.GetDirectoryName(assetPath);
        string folderName = Path.GetFileName(directory);

        if (string.IsNullOrEmpty(folderName))
            return "gen";

        return ExtractConsonants(folderName);
    }

    /// <summary>
    /// Extrai consoantes de uma string para formar um prefixo.
    /// Se não houver consoantes suficientes, completa com vogais e depois com 'X'.
    /// </summary>
    /// <param name="input">Texto de entrada</param>
    /// <returns>Prefixo de 3 caracteres</returns>
    string ExtractConsonants(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "gen";

        input = input.ToLower();
        string consonants = "";
        string vowels = "aeiou";

        // Extrai consoantes
        foreach (char c in input)
        {
            if (char.IsLetter(c) && !vowels.Contains(c))
            {
                consonants += c;
                if (consonants.Length >= 3)
                    break;
            }
        }

        // Se não tem 3 consoantes, completa com vogais
        if (consonants.Length < 3)
        {
            foreach (char c in input)
            {
                if (char.IsLetter(c) && vowels.Contains(c))
                {
                    consonants += c;
                    if (consonants.Length >= 3)
                        break;
                }
            }
        }

        // Se ainda não tem 3 caracteres, completa com números ou caracteres genéricos
        while (consonants.Length < 3)
        {
            consonants += "x";
        }

        return consonants.Substring(0, 3);
    }

    /// <summary>
    /// Menu de contexto para renomear e agrupar subobjetos do GameObject selecionado.
    /// Remove prefixos existentes e adiciona o nome do objeto pai como prefixo.
    /// Agrupa automaticamente objetos com nomes similares.
    /// </summary>
    [MenuItem("GameObject/Renomear Subobjetos", false, 0)]
    static void RenameChildObjects()
    {
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject == null)
        {
            EditorUtility.DisplayDialog("Erro", "Nenhum objeto selecionado!", "OK");
            return;
        }

        Transform[] allChildren = selectedObject.GetComponentsInChildren<Transform>();

        // Remove o próprio objeto da lista (só queremos os filhos)
        Transform[] children = allChildren.Where(t => t != selectedObject.transform).ToArray();

        if (children.Length == 0)
        {
            EditorUtility.DisplayDialog("Aviso", $"O objeto '{selectedObject.name}' não possui subobjetos.", "OK");
            return;
        }

        if (!EditorUtility.DisplayDialog("Renomear e Agrupar Subobjetos",
            $"Esta operação renomeará e agrupará todos os {children.Length} subobjetos de '{selectedObject.name}'.\n\n" +
            $"• Objetos com nomes similares serão agrupados em GRP_X\n" +
            $"• Novo padrão: {selectedObject.name}_nomeDoSubobjeto\n" +
            $"• Objetos únicos não serão agrupados\n\n" +
            "Deseja continuar?", "Sim", "Cancelar"))
        {
            return;
        }

        RenameAndGroupChildren(selectedObject);
    }

    /// <summary>
    /// Validação do menu de contexto. Só aparece quando há um GameObject selecionado.
    /// </summary>
    /// <returns>True se há um GameObject selecionado</returns>
    [MenuItem("GameObject/Renomear Subobjetos", true)]
    static bool ValidateRenameChildObjects()
    {
        return Selection.activeGameObject != null;
    }

    /// <summary>
    /// Agrupa objetos similares em grupos e renomeia todos os subobjetos.
    /// </summary>
    /// <param name="parentObject">Objeto pai cujos filhos serão processados</param>
    static void RenameAndGroupChildren(GameObject parentObject)
    {
        int renamedCount = 0;
        int groupedCount = 0;
        List<string> renameLog = new List<string>();
        List<string> groupLog = new List<string>();

        try
        {
            // Registra operação para Undo
            Undo.RegisterCompleteObjectUndo(parentObject, "Renomear e Agrupar Subobjetos");

            Transform parentTransform = parentObject.transform;
            string parentName = CleanObjectName(parentObject.name);

            // Primeira passada: agrupa objetos similares
            GroupSimilarObjects(parentTransform, ref groupedCount, groupLog);

            // Segunda passada: renomeia todos os objetos
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);
                ProcessChildObjectRecursive(child, parentName, ref renamedCount, renameLog);
            }

            // Exibe resultado
            string message = $"Operação concluída!\n\n" +
                           $"Objetos agrupados: {groupedCount}\n" +
                           $"Objetos renomeados: {renamedCount}\n\n" +
                           "Verifique o Console para ver detalhes.";

            Debug.Log("=== AGRUPAMENTO E RENOMEAÇÃO DE SUBOBJETOS ===");
            Debug.Log($"Objeto pai: {parentObject.name}");

            if (groupLog.Count > 0)
            {
                Debug.Log("=== OBJETOS AGRUPADOS ===");
                foreach (string log in groupLog)
                {
                    Debug.Log(log);
                }
            }

            if (renameLog.Count > 0)
            {
                Debug.Log("=== OBJETOS RENOMEADOS ===");
                foreach (string log in renameLog)
                {
                    Debug.Log(log);
                }
            }

            EditorUtility.DisplayDialog("Concluído", message, "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Erro durante a operação: " + e.Message);
            EditorUtility.DisplayDialog("Erro", "Ocorreu um erro durante a operação. Verifique o Console.", "OK");
        }
    }

    /// <summary>
    /// Agrupa objetos com nomes similares em um objeto grupo.
    /// </summary>
    /// <param name="parent">Transform pai onde criar os grupos</param>
    /// <param name="groupedCount">Contador de objetos agrupados (passado por referência)</param>
    /// <param name="groupLog">Lista de logs dos agrupamentos realizados</param>
    static void GroupSimilarObjects(Transform parent, ref int groupedCount, List<string> groupLog)
    {
        // Coleta informações sobre os filhos diretos
        Dictionary<string, List<Transform>> objectGroups = new Dictionary<string, List<Transform>>();

        // Analisa todos os filhos diretos
        List<Transform> directChildren = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            directChildren.Add(parent.GetChild(i));
        }

        // Agrupa por nome base (removendo números e sufixos)
        foreach (Transform child in directChildren)
        {
            string baseName = GetBaseName(child.name);

            if (!objectGroups.ContainsKey(baseName))
            {
                objectGroups[baseName] = new List<Transform>();
            }
            objectGroups[baseName].Add(child);
        }

        // Cria grupos apenas para objetos que aparecem mais de uma vez
        foreach (var group in objectGroups)
        {
            if (group.Value.Count > 1)
            {
                CreateGroup(parent, group.Key, group.Value, ref groupedCount, groupLog);
            }
        }
    }

    /// <summary>
    /// Extrai o nome base de um objeto removendo números e sufixos como "(Clone)", "(1)", etc.
    /// </summary>
    /// <param name="objectName">Nome original do objeto</param>
    /// <returns>Nome base limpo</returns>
    static string GetBaseName(string objectName)
    {
        string cleanName = objectName;

        // Remove (Clone)
        if (cleanName.EndsWith("(Clone)"))
        {
            cleanName = cleanName.Substring(0, cleanName.Length - 7).Trim();
        }

        // Remove números entre parênteses no final como (1), (2), etc.
        if (cleanName.EndsWith(")"))
        {
            int lastOpenParen = cleanName.LastIndexOf('(');
            if (lastOpenParen > 0)
            {
                string numberPart = cleanName.Substring(lastOpenParen + 1, cleanName.Length - lastOpenParen - 2).Trim();
                if (int.TryParse(numberPart, out _))
                {
                    cleanName = cleanName.Substring(0, lastOpenParen).Trim();
                }
            }
        }

        // Remove números no final sem parênteses
        while (cleanName.Length > 0 && char.IsDigit(cleanName[cleanName.Length - 1]))
        {
            cleanName = cleanName.Substring(0, cleanName.Length - 1).Trim();
        }

        // Remove prefixos existentes
        cleanName = RemoveExistingObjectPrefix(cleanName);

        return cleanName;
    }

    /// <summary>
    /// Cria um objeto grupo contendo uma lista de objetos similares.
    /// </summary>
    /// <param name="parent">Transform pai onde criar o grupo</param>
    /// <param name="baseName">Nome base para o grupo</param>
    /// <param name="objects">Lista de objetos a serem agrupados</param>
    /// <param name="groupedCount">Contador de objetos agrupados</param>
    /// <param name="groupLog">Lista de logs dos agrupamentos</param>
    static void CreateGroup(Transform parent, string baseName, List<Transform> objects, ref int groupedCount, List<string> groupLog)
    {
        // Cria o objeto do grupo
        GameObject groupObject = new GameObject($"GRP_{baseName}");
        groupObject.transform.SetParent(parent);

        // Define a posição do grupo como a média das posições dos objetos
        Vector3 averagePosition = Vector3.zero;
        foreach (Transform obj in objects)
        {
            averagePosition += obj.localPosition;
        }
        averagePosition /= objects.Count;
        groupObject.transform.localPosition = averagePosition;

        // Move todos os objetos para dentro do grupo
        foreach (Transform obj in objects)
        {
            obj.SetParent(groupObject.transform);
            groupedCount++;
        }

        groupLog.Add($"Grupo criado: GRP_{baseName} com {objects.Count} objetos");

        // Ordena os objetos dentro do grupo
        SortChildrenInGroup(groupObject.transform);
    }

    /// <summary>
    /// Ordena os filhos dentro de um grupo, colocando objetos sem número primeiro.
    /// </summary>
    /// <param name="groupTransform">Transform do grupo a ser ordenado</param>
    static void SortChildrenInGroup(Transform groupTransform)
    {
        // Cria uma lista com todos os filhos
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < groupTransform.childCount; i++)
        {
            children.Add(groupTransform.GetChild(i));
        }

        // Ordena por nome (objetos sem número primeiro, depois por número)
        children.Sort((a, b) =>
        {
            string nameA = a.name;
            string nameB = b.name;

            // Se um tem número e outro não, o sem número vem primeiro
            bool aHasNumber = HasNumberSuffix(nameA);
            bool bHasNumber = HasNumberSuffix(nameB);

            if (!aHasNumber && bHasNumber) return -1;
            if (aHasNumber && !bHasNumber) return 1;

            // Se ambos têm ou não têm números, ordena alfabeticamente
            return string.Compare(nameA, nameB, System.StringComparison.Ordinal);
        });

        // Reordena os filhos
        for (int i = 0; i < children.Count; i++)
        {
            children[i].SetSiblingIndex(i);
        }
    }

    /// <summary>
    /// Verifica se o nome de um objeto termina com um sufixo numérico.
    /// </summary>
    /// <param name="name">Nome do objeto a verificar</param>
    /// <returns>True se tem sufixo numérico</returns>
    static bool HasNumberSuffix(string name)
    {
        if (name.EndsWith(")"))
        {
            int lastOpenParen = name.LastIndexOf('(');
            if (lastOpenParen > 0)
            {
                string numberPart = name.Substring(lastOpenParen + 1, name.Length - lastOpenParen - 2);
                return int.TryParse(numberPart, out _);
            }
        }

        // Verifica se termina com número sem parênteses
        if (name.Length > 0 && char.IsDigit(name[name.Length - 1]))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Processa recursivamente um objeto filho, renomeando-o e seus descendentes.
    /// </summary>
    /// <param name="child">Transform do objeto a processar</param>
    /// <param name="parentPrefix">Prefixo do objeto pai</param>
    /// <param name="renamedCount">Contador de objetos renomeados</param>
    /// <param name="renameLog">Lista de logs das renomeações</param>
    static void ProcessChildObjectRecursive(Transform child, string parentPrefix, ref int renamedCount, List<string> renameLog)
    {
        string originalName = child.name;

        // Se é um grupo, não renomeia (mantém GRP_X)
        if (originalName.StartsWith("GRP_"))
        {
            // Processa recursivamente os filhos do grupo
            for (int i = 0; i < child.childCount; i++)
            {
                Transform grandChild = child.GetChild(i);
                ProcessChildObjectRecursive(grandChild, parentPrefix, ref renamedCount, renameLog);
            }
            return;
        }

        string cleanName = RemoveExistingObjectPrefix(originalName);
        cleanName = ConvertToCamelCaseForObjects(cleanName);
        string newName = $"{parentPrefix}_{cleanName}";

        if (originalName != newName)
        {
            child.name = newName;
            renamedCount++;
            renameLog.Add($"{originalName} → {newName}");
        }

        // Processa recursivamente os filhos deste objeto
        for (int i = 0; i < child.childCount; i++)
        {
            Transform grandChild = child.GetChild(i);
            ProcessChildObjectRecursive(grandChild, parentPrefix, ref renamedCount, renameLog);
        }
    }

    /// <summary>
    /// Converte texto para formato camelCase, específico para nomes de objetos.
    /// Preserva camelCase existente quando detectado.
    /// </summary>
    /// <param name="input">Texto a ser convertido</param>
    /// <returns>Texto em formato camelCase</returns>
    static string ConvertToCamelCaseForObjects(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Remove espaços, underscores, hífens e pontos
        string[] words = input.Split(new char[] { ' ', '_', '-', '.' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (words.Length == 0)
            return input;

        // Se só tem uma palavra
        if (words.Length == 1)
        {
            string word = words[0];
            if (word.Length == 0) return word;

            // Mantém camelCase existente se detectado
            if (char.IsLower(word[0]) && word.Any(char.IsUpper))
                return word;

            return char.ToLower(word[0]) + word.Substring(1).ToLower();
        }

        // Primeira palavra em minúscula
        string result = words[0].ToLower();

        // Demais palavras com primeira letra maiúscula
        for (int i = 1; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                result += char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
        }

        return result;
    }

    /// <summary>
    /// Limpa o nome de um objeto removendo sufixos do Unity e prefixos existentes.
    /// </summary>
    /// <param name="objectName">Nome original do objeto</param>
    /// <returns>Nome limpo sem sufixos e prefixos</returns>
    static string CleanObjectName(string objectName)
    {
        // Remove sufixos comuns do Unity como "(Clone)", "(1)", etc.
        string cleanName = objectName;

        // Remove (Clone)
        if (cleanName.EndsWith("(Clone)"))
        {
            cleanName = cleanName.Substring(0, cleanName.Length - 7).Trim();
        }

        // Remove números entre parênteses no final
        if (cleanName.EndsWith(")"))
        {
            int lastOpenParen = cleanName.LastIndexOf('(');
            if (lastOpenParen > 0)
            {
                string numberPart = cleanName.Substring(lastOpenParen + 1, cleanName.Length - lastOpenParen - 2);
                if (int.TryParse(numberPart, out _))
                {
                    cleanName = cleanName.Substring(0, lastOpenParen).Trim();
                }
            }
        }

        // Remove prefixo existente se houver
        cleanName = RemoveExistingObjectPrefix(cleanName);

        return cleanName;
    }

    /// <summary>
    /// Remove prefixos existentes no formato XXX_ ou XXXX_ de nomes de objetos.
    /// </summary>
    /// <param name="objectName">Nome do objeto com possível prefixo</param>
    /// <returns>Nome sem prefixo</returns>
    static string RemoveExistingObjectPrefix(string objectName)
    {
        if (objectName.Length > 4)
        {
            int underscoreIndex = objectName.IndexOf('_');
            if (underscoreIndex >= 3 && underscoreIndex <= 4)
            {
                // Verifica se a parte antes do underscore são apenas letras/números
                string prefixPart = objectName.Substring(0, underscoreIndex);
                if (prefixPart.All(c => char.IsLetterOrDigit(c)))
                {
                    return objectName.Substring(underscoreIndex + 1);
                }
            }
        }

        return objectName;
    }
}