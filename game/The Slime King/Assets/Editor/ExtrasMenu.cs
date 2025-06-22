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
    /// Cria rapidamente um prefab básico do Slime em seu estágio inicial
    /// </summary>
    [MenuItem("Extras/The Slime King/Criar Prefab de Slime Básico")]
    public static void CreateBasicSlimePrefab()
    {
        // Cria o GameObject base
        GameObject slimeObj = new GameObject("BabySlime");

        // Adiciona componentes básicos
        slimeObj.AddComponent<SpriteRenderer>();
        slimeObj.AddComponent<Rigidbody2D>().gravityScale = 0f; // Zero gravity para movimento top-down

        // Adiciona collider circular (apropriado para um slime)
        CircleCollider2D collider = slimeObj.AddComponent<CircleCollider2D>();
        collider.radius = 0.5f;

        // Cria o prefab
        string prefabPath = "Assets/Prefabs/Characters/Player/BabySlime.prefab";

        // Garante que o diretório existe
        string directory = Path.GetDirectoryName(prefabPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Cria o prefab
        PrefabUtility.SaveAsPrefabAsset(slimeObj, prefabPath);

        // Destrói o objeto temporário
        Object.DestroyImmediate(slimeObj);

        Debug.Log($"✅ Prefab do Baby Slime criado em: {prefabPath}");

        // Seleciona e destaca o prefab no Project
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
        EditorGUIUtility.PingObject(Selection.activeObject);
    }

    /// <summary>
    /// Cria uma cena para cada região do jogo
    /// </summary>
    [MenuItem("Extras/The Slime King/Criar Cenas para Regiões")]
    public static void CreateRegionScenes()
    {
        // Lista de regiões do jogo conforme o GDD
        string[] regions = new string[] {
            "Caverna_Inicial",
            "Floresta_Sussurrante",
            "Cavernas_Cristalinas",
            "Lagos_Serenos",
            "Picos_Ventosos",
            "Nucleo_Elemental"
        };

        // Garante que o diretório existe
        string directory = "Assets/Scenes/Regions";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Cria uma cena para cada região
        foreach (string region in regions)
        {
            string scenePath = $"{directory}/{region}.unity";

            // Cria uma nova cena vazia
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Adiciona componentes básicos comuns em todas as cenas
            GameObject globalLight = new GameObject("Global Light");
            globalLight.AddComponent<Light2D>().lightType = Light2D.LightType.Global;

            GameObject cameraObj = new GameObject("Main Camera");
            cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";

            // Salva a cena
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"✅ Cena {region} criada em: {scenePath}");
        }

        AssetDatabase.Refresh();
        Debug.Log("✅ Todas as cenas de regiões foram criadas!");
    }

    /// <summary>
    /// Cria scriptable objects para os quatro elementos básicos do jogo
    /// </summary>
    [MenuItem("Extras/The Slime King/Criar Elementos Básicos")]
    public static void CreateElementalScriptableObjects()
    {
        // Garante que o diretório existe
        string directory = "Assets/ScriptableObjects/Elements";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Informações dos elementos conforme o GDD
        (string name, Color color)[] elements = new[] {
            ("Terra", new Color(0.5f, 0.3f, 0.1f)), // Marrom
            ("Agua", new Color(0.2f, 0.5f, 0.9f)),  // Azul
            ("Fogo", new Color(0.9f, 0.2f, 0.1f)),  // Vermelho
            ("Ar", new Color(0.9f, 0.9f, 0.9f))     // Branco
        };

        // Para cada elemento, cria um scriptable object
        foreach (var element in elements)
        {
            // Verifica se já existe o script ElementalType
            MonoScript script = AssetDatabase.FindAssets("ElementalType t:MonoScript")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<MonoScript>(path))
                .FirstOrDefault();

            if (script == null)
            {
                // Cria o script se não existir
                CreateElementalTypeScript();
                AssetDatabase.Refresh();

                // Tenta encontrar novamente
                script = AssetDatabase.FindAssets("ElementalType t:MonoScript")
                    .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                    .Select(path => AssetDatabase.LoadAssetAtPath<MonoScript>(path))
                    .FirstOrDefault();

                if (script == null)
                {
                    Debug.LogError("❌ Não foi possível criar o script ElementalType.");
                    return;
                }
            }

            // Cria o scriptable object
            string assetPath = $"{directory}/{element.name}.asset";

            // Cria a instância do scriptable object
            var scriptableObj = ScriptableObject.CreateInstance(script.GetClass());

            // Define as propriedades (usando reflexão)
            var nameProperty = scriptableObj.GetType().GetProperty("ElementName");
            if (nameProperty != null) nameProperty.SetValue(scriptableObj, element.name);

            var colorProperty = scriptableObj.GetType().GetProperty("ElementColor");
            if (colorProperty != null) colorProperty.SetValue(scriptableObj, element.color);

            // Salva o asset
            AssetDatabase.CreateAsset(scriptableObj, assetPath);
            Debug.Log($"✅ Elemento {element.name} criado em: {assetPath}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ Todos os elementos elementais foram criados!");
    }

    /// <summary>
    /// Cria o script ElementalType para os scriptable objects elementais
    /// </summary>
    private static void CreateElementalTypeScript()
    {
        string directory = "Assets/Code/ElementSystem";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string scriptPath = $"{directory}/ElementalType.cs";
        string scriptContent = @"using UnityEngine;

/// <summary>
/// Representa um tipo elemental no jogo The Slime King
/// </summary>
[CreateAssetMenu(fileName = ""NewElement"", menuName = ""The Slime King/Elemental Type"")]
public class ElementalType : ScriptableObject
{
    [SerializeField] private string elementName;
    [SerializeField] private Color elementColor;
    [SerializeField] [TextArea(3, 5)] private string description;
    [SerializeField] private float damageModifier = 1.0f;
    [SerializeField] private float absorptionRate = 1.0f;
    
    // Propriedades públicas
    public string ElementName => elementName;
    public Color ElementColor => elementColor;
    public string Description => description;
    public float DamageModifier => damageModifier;
    public float AbsorptionRate => absorptionRate;
    
    // Métodos que podem ser implementados posteriormente
    public ParticleSystem GetAbsorptionEffect() 
    {
        // Implementação futura
        return null;
    }
    
    public AudioClip GetAbsorptionSound()
    {
        // Implementação futura
        return null;
    }
}
";

        File.WriteAllText(scriptPath, scriptContent);
        Debug.Log($"✅ Script ElementalType criado em: {scriptPath}");
    }

    /// <summary>
    /// Cria a estrutura de pastas para os códigos relacionados aos inimigos
    /// </summary>
    [MenuItem("Extras/The Slime King/Criar Estrutura de Inimigos")]
    public static void CreateEnemyCodeStructure()
    {
        // Lista de pastas específicas para inimigos
        List<string> enemyFolders = new List<string>
        {
            "Code/Gameplay/Enemies",
            "Code/Gameplay/Enemies/Basic",
            "Code/Gameplay/Enemies/Bosses",
            "Code/Gameplay/Enemies/AI",
            "Code/Gameplay/Enemies/Behaviors",
            "ScriptableObjects/Enemies/Common",
            "ScriptableObjects/Enemies/Bosses",
            "ScriptableObjects/Enemies/Behaviors",
            "Art/Characters/Enemies/Animations",
            "Art/Characters/Enemies/SpriteSheets",
            "Prefabs/Characters/Enemies/Common",
            "Prefabs/Characters/Enemies/Bosses",
            "Audio/SFX/Enemies"
        };

        // Cria as pastas
        foreach (string folder in enemyFolders)
        {
            string path = $"Assets/{folder}";
            CreateFolderRecursive(path);
        }

        // Cria alguns arquivos básicos de exemplo
        CreateEnemyScripts();

        AssetDatabase.Refresh();
        Debug.Log("✅ Estrutura de pastas para inimigos criada!");
    }

    /// <summary>
    /// Cria scripts básicos para inimigos
    /// </summary>
    private static void CreateEnemyScripts()
    {
        // Script base para comportamentos de inimigos
        string baseEnemyBehaviorPath = "Assets/Code/Gameplay/Enemies/Behaviors/BaseEnemyBehavior.cs";
        string baseEnemyBehaviorContent = @"using UnityEngine;
using TheSlimeKing.Gameplay;

namespace TheSlimeKing.Gameplay.Enemies
{
    /// <summary>
    /// Classe base para comportamentos de inimigos
    /// </summary>
    public abstract class BaseEnemyBehavior : MonoBehaviour
    {
        [SerializeField] protected EnemyStats enemyStats;

        protected virtual void Awake()
        {
            if (enemyStats == null)
                enemyStats = GetComponent<EnemyStats>();
        }

        protected virtual void OnEnable()
        {
            if (enemyStats != null)
                enemyStats.OnAggroStateChanged += HandleAggroChanged;
        }

        protected virtual void OnDisable()
        {
            if (enemyStats != null)
                enemyStats.OnAggroStateChanged -= HandleAggroChanged;
        }

        protected virtual void HandleAggroChanged(bool isAggroed)
        {
            // Implementação específica em subclasses
        }

        public abstract void PerformBehavior();
    }
}";
        // Criar script de comportamento de patrulha
        string patrolBehaviorPath = "Assets/Code/Gameplay/Enemies/Behaviors/PatrolBehavior.cs";
        string patrolBehaviorContent = @"using UnityEngine;

namespace TheSlimeKing.Gameplay.Enemies
{
    /// <summary>
    /// Comportamento de patrulha para inimigos
    /// </summary>
    public class PatrolBehavior : BaseEnemyBehavior
    {
        [SerializeField] private Transform[] waypoints;
        [SerializeField] private float waypointPauseTime = 1f;
        
        private int currentWaypointIndex;
        private bool isWaiting;
        private float waitTimer;
        
        public override void PerformBehavior()
        {
            if (waypoints == null || waypoints.Length == 0)
                return;
                
            // Lógica de patrulha a ser implementada
        }
        
        protected override void HandleAggroChanged(bool isAggroed)
        {
            // Quando aggro muda, ajusta o comportamento
            enabled = !isAggroed; // Desativa patrulha quando agressivo
        }
    }
}";
        // Garantir diretorios existam
        Directory.CreateDirectory(Path.GetDirectoryName(baseEnemyBehaviorPath));
        Directory.CreateDirectory(Path.GetDirectoryName(patrolBehaviorPath));

        // Escrever arquivos
        File.WriteAllText(baseEnemyBehaviorPath, baseEnemyBehaviorContent);
        File.WriteAllText(patrolBehaviorPath, patrolBehaviorContent);

        Debug.Log($"✅ Scripts de exemplo para inimigos criados!");
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
