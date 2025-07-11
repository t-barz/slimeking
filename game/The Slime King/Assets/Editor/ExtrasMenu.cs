#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Para Light2D
using UnityEngine.SceneManagement; // Para Scene
using UnityEngine.UI; // Para Canvas, Text, etc.
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class ExtrasMenu
{

    /// <summary>
    /// Adiciona as tags essenciais do projeto conforme o GDD.
    /// </summary>
    [MenuItem("Extras/The Slime King/Configurar Tags do Projeto")]
    public static void SetupProjectTags()
    {
        // Tags essenciais conforme o GDD (ajuste conforme necessário)
        string[] requiredTags = new string[]
        {
            "Player",
            "Enemy",
            "Destructible",
            "Attack",
            "SpecialAttack",
            "EnemyAttack",
            "EnemySpecialAttack",
            "Interactable",
            "Follower",
            "Item",
            "Platform",
            "Projectile"
        };

        // Caminho do TagManager
        string tagManagerPath = "ProjectSettings/TagManager.asset";
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(tagManagerPath)[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        bool changed = false;

        foreach (string tag in requiredTags)
        {
            bool exists = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue == tag)
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                tagsProp.GetArrayElementAtIndex(0).stringValue = tag;
                Debug.Log($"Tag adicionada: {tag}");
                changed = true;
            }
        }

        if (changed)
        {
            tagManager.ApplyModifiedProperties();
            EditorUtility.DisplayDialog("Tags do Projeto", "Tags essenciais configuradas com sucesso!", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Tags do Projeto", "Todas as tags essenciais já estavam configuradas.", "OK");
        }
    }

    /// <summary>
    /// Adiciona um item de menu "Extras > The Slime King > Verificar CSV Localização" para validar o arquivo de localização.
    /// </summary>
    [MenuItem("Extras/The Slime King/Localização/Verificar CSV Localização")]
    public static void ValidateLocalizationCSV()
    {
        string csvPath = "Assets/StreamingAssets/Localization/localization.csv";
        if (!File.Exists(csvPath))
        {
            EditorUtility.DisplayDialog("Erro", "Arquivo de localização não encontrado:\n" + csvPath, "OK");
            return;
        }

        try
        {
            string[] lines = File.ReadAllLines(csvPath);
            if (lines.Length == 0)
            {
                EditorUtility.DisplayDialog("Erro", "Arquivo de localização está vazio!", "OK");
                return;
            }

            // Verifica o cabeçalho
            string[] headers = lines[0].Split(',');
            if (headers.Length < 2 || headers[0] != "Key")
            {
                EditorUtility.DisplayDialog("Erro", "Cabeçalho do arquivo de localização inválido!", "OK");
                return;
            }

            // Conta entradas por prefixo
            Dictionary<string, int> prefixCount = new Dictionary<string, int>();
            int totalEntries = 0;

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i].Trim())) continue;

                string[] values = lines[i].Split(',');
                if (values.Length > 0)
                {
                    string key = values[0];
                    string prefix = GetKeyPrefix(key);

                    if (!prefixCount.ContainsKey(prefix))
                    {
                        prefixCount[prefix] = 0;
                    }

                    prefixCount[prefix]++;
                    totalEntries++;
                }
            }

            // Constrói mensagem de estatísticas
            System.Text.StringBuilder stats = new System.Text.StringBuilder();
            stats.AppendLine("Arquivo de localização validado com sucesso!");
            stats.AppendLine($"Total de entradas: {totalEntries}");
            stats.AppendLine($"Idiomas suportados: {headers.Length - 1}");
            stats.AppendLine("\nEntradas por prefixo:");

            foreach (var entry in prefixCount.OrderByDescending(e => e.Value))
            {
                stats.AppendLine($"- {entry.Key}: {entry.Value}");
            }

            EditorUtility.DisplayDialog("Validação Concluída", stats.ToString(), "OK");
        }
        catch (System.Exception ex)
        {
            EditorUtility.DisplayDialog("Erro", "Erro ao validar arquivo de localização:\n" + ex.Message, "OK");
        }
    }

    /// <summary>
    /// Extrai o prefixo da chave de localização (ui_, dialog_, desc_, msg_, etc.)
    /// </summary>
    private static string GetKeyPrefix(string key)
    {
        if (string.IsNullOrEmpty(key)) return "sem_prefixo";

        int underscorePos = key.IndexOf('_');
        if (underscorePos > 0)
        {
            return key.Substring(0, underscorePos + 1);
        }

        return "sem_prefixo";
    }

    /// <summary>
    /// Adiciona um item de menu "Extras > Criar Estrutura de Pastas" ao Editor.
    /// Este código só será executado no ambiente de desenvolvimento da Unity.
    /// </summary>
    [MenuItem("Extras/The Slime King/Criar Estrutura de Pastas")]
    public static void GenerateFolders()
    {
        List<string> folders = new List<string>
        {
            // Estrutura básica existente
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
            "Testing",
            
            // Novas pastas específicas para o jogo
            "Art/Characters/Player/Stages",          // Para os 4 estágios do slime
            "Art/Characters/Enemies/Bosses",         // Para os "Reis" mencionados no GDD
            "Art/Effects",                           // Para efeitos visuais
            "Art/Effects/Elemental",                 // Efeitos elementais
            "Audio/Ambient",                         // Sons ambientais para atmosfera "cozy"
            "Code/ElementSystem",                    // Sistema elemental
            "Code/Gameplay/Growth",                  // Sistema de crescimento do slime
            "Code/Gameplay/Stealth",                 // Sistema de stealth
            "Prefabs/Elements",                      // Prefabs relacionados aos elementos
            "Prefabs/Interactables",                 // Objetos interativos
            "Scenes/Regions",                        // Para as seis regiões principais
            "Scenes/Cutscenes",                      // Para as cutscenes cinematográficas
            "ScriptableObjects",                     // Para dados do jogo
            "ScriptableObjects/Elements",            // Dados dos elementos
            "ScriptableObjects/Enemies",             // Dados dos inimigos
            "ScriptableObjects/Growth",              // Dados de crescimento do slime
        };

        foreach (string folder in folders)
        {
            string path = $"Assets/{folder}";
            CreateFolderRecursive(path);
        }

        AssetDatabase.Refresh();
        Debug.Log("✅ Estrutura de pastas criada!");
    }

    /// <summary>
    /// Método auxiliar para criar pastas recursivamente.
    /// </summary>
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
    /// Adiciona um item de menu para agrupar objetos selecionados no Editor.
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


    /// <summary>
    /// Reorganiza a estrutura de pastas e arquivos existentes
    /// </summary>
    [MenuItem("Extras/The Slime King/Reorganizar Projeto")]
    public static void ReorganizeProjectStructure()
    {
        // Confirmação do usuário para evitar reorganização acidental
        if (!EditorUtility.DisplayDialog(
            "Reorganizar Projeto",
            "Esta ação irá reorganizar arquivos do projeto em suas pastas apropriadas. Isso pode alterar referências. Recomenda-se fazer um backup do projeto antes.\n\nDeseja continuar?",
            "Sim, reorganizar", "Cancelar"))
        {
            Debug.Log("⚠️ Reorganização cancelada pelo usuário.");
            return;
        }

        bool changesApplied = false;
        int filesReorganized = 0;

        try
        {
            // Busca todos os arquivos de script e verifica se estão no diretório correto
            var codeFiles = Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.Contains("/Editor/"))
                .ToArray();

            foreach (var file in codeFiles)
            {
                // Move scripts relacionados a inimigos
                if (Path.GetFileName(file).Contains("Enemy") || Path.GetFileName(file).Contains("Boss"))
                {
                    string targetDirectory;

                    if (Path.GetFileName(file).Contains("Data") || file.Contains("ScriptableObject"))
                    {
                        // ScriptableObjects para os dados de inimigo
                        targetDirectory = "Assets/ScriptableObjects/Enemies";

                        if (Path.GetFileName(file).Contains("Boss"))
                            targetDirectory += "/Bosses";
                        else
                            targetDirectory += "/Common";
                    }
                    else if (Path.GetFileName(file).Contains("Behavior") || Path.GetFileName(file).Contains("AI"))
                    {
                        // Comportamentos de IA
                        targetDirectory = "Assets/Code/Gameplay/Enemies/Behaviors";
                    }
                    else
                    {
                        // Scripts de inimigos gerais
                        targetDirectory = "Assets/Code/Gameplay/Enemies";

                        if (Path.GetFileName(file).Contains("Boss"))
                            targetDirectory += "/Bosses";
                        else
                            targetDirectory += "/Basic";
                    }

                    // Garante que o diretório existe
                    if (!Directory.Exists(targetDirectory))
                        Directory.CreateDirectory(targetDirectory);

                    // Novo caminho para o arquivo
                    string newPath = Path.Combine(targetDirectory, Path.GetFileName(file));

                    // Move apenas se não for o mesmo caminho
                    if (file != newPath && !File.Exists(newPath))
                    {
                        File.Move(file, newPath);
                        filesReorganized++;
                        changesApplied = true;
                    }
                }

                // Outros tipos de arquivos podem ser organizados aqui...
            }

            // Organizar prefabs
            var prefabs = Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories);
            foreach (var prefab in prefabs)
            {
                if (Path.GetFileName(prefab).Contains("Enemy") || Path.GetFileName(prefab).Contains("Boss"))
                {
                    string targetDirectory;

                    if (Path.GetFileName(prefab).Contains("Boss"))
                        targetDirectory = "Assets/Prefabs/Characters/Enemies/Bosses";
                    else
                        targetDirectory = "Assets/Prefabs/Characters/Enemies/Common";

                    // Garante que o diretório existe
                    if (!Directory.Exists(targetDirectory))
                        Directory.CreateDirectory(targetDirectory);

                    // Novo caminho para o prefab
                    string newPath = Path.Combine(targetDirectory, Path.GetFileName(prefab));

                    // Move apenas se não for o mesmo caminho
                    if (prefab != newPath && !File.Exists(newPath))
                    {
                        AssetDatabase.MoveAsset(prefab, newPath);
                        filesReorganized++;
                        changesApplied = true;
                    }
                }
            }

            if (changesApplied)
            {
                AssetDatabase.Refresh();
                Debug.Log($"✅ Projeto reorganizado! {filesReorganized} arquivos foram movidos para pastas apropriadas.");
            }
            else
            {
                Debug.Log("✅ Análise concluída! Todos os arquivos já estão organizados corretamente.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Erro ao reorganizar projeto: {ex.Message}");
        }
    }

    /// <summary>
    /// Reorganiza objetos na cena atual agrupando-os por tipo e funcionalidade
    /// </summary>
    [MenuItem("Extras/The Slime King/Reorganizar Objetos na Cena")]
    public static void ReorganizeSceneHierarchy()
    {
        // Confirmação do usuário para evitar reorganização acidental
        if (!EditorUtility.DisplayDialog(
            "Reorganizar Objetos na Cena",
            "Esta ação irá reorganizar os objetos da cena atual, agrupando-os por tipo e funcionalidade.\n\nDeseja continuar?",
            "Sim, reorganizar", "Cancelar"))
        {
            Debug.Log("⚠️ Reorganização da cena cancelada pelo usuário.");
            return;
        }

        // Lista para categorias principais
        Dictionary<string, GameObject> categories = new Dictionary<string, GameObject>();

        try
        {
            // Raízes para categorias principais
            string[] mainCategories = new string[]
            {
                "ENVIRONMENT",
                "CHARACTERS",
                "UI",
                "LIGHTS",
                "EFFECTS",
                "CAMERAS",
                "AUDIO"
            };

            // Criar as categorias principais se não existirem
            foreach (var category in mainCategories)
            {
                GameObject categoryRoot = GameObject.Find(category);

                if (!categoryRoot)
                {
                    categoryRoot = new GameObject(category);
                    Undo.RegisterCreatedObjectUndo(categoryRoot, "Create Category Root");
                }

                categories.Add(category, categoryRoot);
            }

            // Obter todos os GameObjects raiz na cena
            List<GameObject> rootObjects = new List<GameObject>();
            Scene currentScene = EditorSceneManager.GetActiveScene();
            currentScene.GetRootGameObjects(rootObjects);

            int objectsReorganized = 0;

            // Iterar por todos os GameObjects raiz (exceto nossas categorias)
            foreach (var rootObj in rootObjects.ToArray()) // ToArray para evitar problemas de coleção modificada
            {
                // Ignora as categorias que já criamos
                if (mainCategories.Contains(rootObj.name))
                    continue;

                // Decide a categoria baseado no nome e componentes
                GameObject targetCategory = DetermineCategory(rootObj, categories);

                if (targetCategory)
                {
                    // Move o objeto para dentro da categoria
                    Undo.SetTransformParent(rootObj.transform, targetCategory.transform, "Reorganize Scene Objects");
                    objectsReorganized++;
                }
            }

            // Organiza objetos dentro das categorias, se necessário
            OrganizeWithinCategories(categories);

            // Remove categorias vazias
            foreach (var category in mainCategories)
            {
                if (categories[category].transform.childCount == 0)
                {
                    GameObject.DestroyImmediate(categories[category]);
                }
            }

            Debug.Log($"✅ Cena reorganizada! {objectsReorganized} objetos foram organizados em categorias.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Erro ao reorganizar objetos na cena: {ex.Message}");
        }
    }

    /// <summary>
    /// Determina em qual categoria um GameObject deve ser colocado
    /// </summary>
    private static GameObject DetermineCategory(GameObject obj, Dictionary<string, GameObject> categories)
    {
        // Verificar componentes para determinar categoria
        if (obj.GetComponent<Camera>() || obj.name.Contains("Camera") || obj.name.Contains("cam"))
        {
            return categories["CAMERAS"];
        }
        else if (obj.GetComponent<Canvas>() || obj.name.Contains("UI") ||
                 obj.GetComponent<Text>() || obj.GetComponent<Image>() ||
                 obj.GetComponent<Button>())
        {
            return categories["UI"];
        }
        else if (obj.GetComponent<Light>() || obj.GetComponent<Light2D>() || obj.name.Contains("Light"))
        {
            return categories["LIGHTS"];
        }
        else if (obj.GetComponent<AudioSource>() || obj.name.Contains("Audio") || obj.name.Contains("Sound"))
        {
            return categories["AUDIO"];
        }
        else if (obj.GetComponent<ParticleSystem>() || obj.name.Contains("Particle") ||
                 obj.name.Contains("Effect") || obj.name.Contains("VFX"))
        {
            return categories["EFFECTS"];
        }
        else if (obj.GetComponent<Rigidbody>() || obj.GetComponent<Rigidbody2D>() ||
                 obj.name.Contains("Player") || obj.name.Contains("Slime") ||
                 obj.name.Contains("Enemy") || obj.name.Contains("NPC") ||
                 obj.GetComponent<Animator>())
        {
            return categories["CHARACTERS"];
        }
        else // Default para Environment se não se encaixar em nenhuma categoria específica
        {
            return categories["ENVIRONMENT"];
        }
    }

    /// <summary>
    /// Organiza objetos dentro de cada categoria, criando subcategorias conforme necessário
    /// </summary>
    private static void OrganizeWithinCategories(Dictionary<string, GameObject> categories)
    {        // Organização dentro da categoria Environment
        if (categories["ENVIRONMENT"].transform.childCount > 0)
        {
            Dictionary<string, GameObject> envSubcategories = new Dictionary<string, GameObject>
            {
                { "Props", CreateSubcategory("Props", categories["ENVIRONMENT"]) },
                { "Terrain", CreateSubcategory("Terrain", categories["ENVIRONMENT"]) },
                { "Platforms", CreateSubcategory("Platforms", categories["ENVIRONMENT"]) },
                { "Interactables", CreateSubcategory("Interactables", categories["ENVIRONMENT"]) }
            };

            OrganizeObjectsIntoSubcategories(categories["ENVIRONMENT"], envSubcategories);
        }

        // Organização dentro da categoria Characters
        if (categories["CHARACTERS"].transform.childCount > 0)
        {
            Dictionary<string, GameObject> charSubcategories = new Dictionary<string, GameObject>
            {
                { "Player", CreateSubcategory("Player", categories["CHARACTERS"]) },
                { "Enemies", CreateSubcategory("Enemies", categories["CHARACTERS"]) },
                { "NPCs", CreateSubcategory("NPCs", categories["CHARACTERS"]) }
            };

            OrganizeObjectsIntoSubcategories(categories["CHARACTERS"], charSubcategories);
        }

        // Adicionar mais subcategorias se necessário para outras categorias
    }

    /// <summary>
    /// Cria uma subcategoria dentro de uma categoria pai
    /// </summary>
    private static GameObject CreateSubcategory(string name, GameObject parent)
    {
        // Verifica se a subcategoria já existe
        Transform existingCategory = parent.transform.Find(name);
        if (existingCategory)
            return existingCategory.gameObject;

        // Cria nova subcategoria
        GameObject subCategory = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(subCategory, "Create Subcategory");
        subCategory.transform.SetParent(parent.transform);
        return subCategory;
    }

    /// <summary>
    /// Organiza objetos nas subcategorias apropriadas
    /// </summary>
    private static void OrganizeObjectsIntoSubcategories(GameObject category, Dictionary<string, GameObject> subcategories)
    {
        // Obtém todos os objetos filhos da categoria principal (exceto as subcategorias)
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in category.transform)
        {
            if (!subcategories.Values.Any(sc => sc.transform == child))
                children.Add(child.gameObject);
        }

        foreach (var obj in children)
        {
            GameObject targetSubcategory = null;            // Determina a subcategoria com base no nome e componentes do objeto
            string objName = obj.name.ToLower();

            if (category.name == "ENVIRONMENT")
            {
                if (objName.Contains("platform") || objName.Contains("ground"))
                    targetSubcategory = subcategories["Platforms"];
                else if (objName.Contains("terrain") || objName.Contains("landscape"))
                    targetSubcategory = subcategories["Terrain"];
                else if (objName.Contains("interact") || objName.Contains("pickup") || objName.Contains("button"))
                    targetSubcategory = subcategories["Interactables"];
                else
                    targetSubcategory = subcategories["Props"];
            }
            else if (category.name == "CHARACTERS")
            {
                if (objName.Contains("player") || objName.Contains("slime"))
                    targetSubcategory = subcategories["Player"];
                else if (objName.Contains("enemy") || objName.Contains("boss"))
                    targetSubcategory = subcategories["Enemies"];
                else
                    targetSubcategory = subcategories["NPCs"];
            }

            // Move para a subcategoria identificada
            if (targetSubcategory != null)
            {
                Undo.SetTransformParent(obj.transform, targetSubcategory.transform, "Organize Into Subcategory");
            }
        }

        // Remove subcategorias vazias
        foreach (var subcategory in subcategories.Values.ToList())
        {
            if (subcategory.transform.childCount == 0)
            {
                GameObject.DestroyImmediate(subcategory);
            }
        }
    }
}
#endif
