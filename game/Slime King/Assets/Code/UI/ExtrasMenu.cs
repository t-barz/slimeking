#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class ExtrasMenu
{
    #region Estrutura de Pastas

    private static readonly string[] PastasRaiz = {
        "Art",
        "Audio",
        "Code",
        "Documentation",
        "Editor",
        "Prefabs",
        "Resources",
        "Scenes",
        "ScriptableObjects",
        "Settings",
        "Shaders",
        "Testing",
        "VFX"
    };

    private static readonly Dictionary<string, string[]> SubPastas = new Dictionary<string, string[]>
    {
        {
            "Art", new[] {
                "Animations",
                "Materials",
                "Models",
                "Sprites",
                "Textures",
                "UI"
            }
        },
        {
            "Audio", new[] {
                "Music",
                "SFX",
                "Ambience",
                "Voice"
            }
        },
        {
            "Code", new[] {
                "Core",
                "Gameplay",
                "Player",
                "Enemies",
                "NPCs",
                "UI",
                "Utils",
                "Systems",
                "Managers"
            }
        },
        {
            "Prefabs", new[] {
                "Player",
                "Enemies",
                "NPCs",
                "Environment",
                "Items",
                "UI",
                "Effects"
            }
        },
        {
            "Resources", new[] {
                "Data",
                "Configs",
                "Localization"
            }
        },
        {
            "Scenes", new[] {
                "Main",
                "Levels",
                "UI",
                "Testing"
            }
        },
        {
            "ScriptableObjects", new[] {
                "Items",
                "Abilities",
                "Characters",
                "Quests",
                "Dialogue"
            }
        },
        {
            "Settings", new[] {
                "Rendering",
                "Physics",
                "Input",
                "Audio"
            }
        },
        {
            "VFX", new[] {
                "Particles",
                "Materials",
                "Shaders",
                "Textures"
            }
        }
    };

    [MenuItem("Extras/Criar Estrutura de Pastas")]
    public static void CriarEstruturaDePastas()
    {
        foreach (string pasta in PastasRaiz)
        {
            CriarPasta("Assets/" + pasta);

            if (SubPastas.TryGetValue(pasta, out string[] subPastas))
            {
                foreach (string subPasta in subPastas)
                {
                    CriarPasta($"Assets/{pasta}/{subPasta}");
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Estrutura de pastas criada com sucesso!");
    }

    private static void CriarPasta(string caminho)
    {
        if (!AssetDatabase.IsValidFolder(caminho))
        {
            string pastaParente = Path.GetDirectoryName(caminho).Replace('\\', '/');
            string nomePasta = Path.GetFileName(caminho);

            if (!AssetDatabase.IsValidFolder(pastaParente))
                CriarPasta(pastaParente);

            AssetDatabase.CreateFolder(pastaParente, nomePasta);
            Debug.Log($"Pasta criada: {caminho}");
        }
    }

    #endregion

    #region Reorganização de Pastas

    [MenuItem("Extras/Reorganizar Estrutura de Pastas Existente")]
    public static void ReorganizarEstruturaDePastas()
    {
        // Primeiro garantir que a estrutura básica exista
        CriarEstruturaDePastas();

        // Mapeamento de extensões para pastas destino
        Dictionary<string, string> mapeamentoExtensoes = new Dictionary<string, string>
        {
            // Scripts
            {".cs", "Assets/Code"},
            
            // Assets visuais
            {".png", "Assets/Art/Sprites"},
            {".jpg", "Assets/Art/Textures"},
            {".jpeg", "Assets/Art/Textures"},
            {".tga", "Assets/Art/Textures"},
            {".psd", "Assets/Art/Sprites"},
            
            // Modelos 3D
            {".fbx", "Assets/Art/Models"},
            {".obj", "Assets/Art/Models"},
            {".blend", "Assets/Art/Models"},
            
            // Materiais
            {".mat", "Assets/Art/Materials"},
            {".physicMaterial", "Assets/Settings/Physics"},
            {".physicsMaterial2D", "Assets/Settings/Physics"},
            
            // Áudio
            {".mp3", "Assets/Audio/SFX"},
            {".wav", "Assets/Audio/SFX"},
            {".ogg", "Assets/Audio/SFX"},
            
            // Animações
            {".anim", "Assets/Art/Animations"},
            {".controller", "Assets/Art/Animations"},
            
            // Scenes
            {".unity", "Assets/Scenes/Main"},
            
            // Prefabs
            {".prefab", "Assets/Prefabs"},
            
            // ScriptableObjects
            {".asset", "Assets/ScriptableObjects"},
            
            // Shaders
            {".shader", "Assets/Shaders"},
            {".compute", "Assets/Shaders"}
        };

        // Lista especial para scripts, para serem categorizados por conteúdo
        List<string> scriptsPaths = new List<string>();

        // Obter todos os arquivos em Assets (excluindo pastas específicas como Library, Packages, etc)
        string[] todosArquivos = Directory.GetFiles("Assets", "*.*", SearchOption.AllDirectories)
            .Where(p => !p.StartsWith("Assets/Editor/") &&
                        !p.StartsWith("Assets/Plugins/") &&
                        !p.Contains("/Editor/") &&
                        !p.Contains("/Resources/"))
            .ToArray();

        foreach (string arquivo in todosArquivos)
        {
            string extensao = Path.GetExtension(arquivo).ToLower();

            // Ignorar .meta e pastas
            if (extensao == ".meta" || Directory.Exists(arquivo))
                continue;

            // Tratar scripts separadamente para categorização
            if (extensao == ".cs")
            {
                scriptsPaths.Add(arquivo);
                continue;
            }

            if (mapeamentoExtensoes.TryGetValue(extensao, out string pastaDest))
            {
                string nomeArquivo = Path.GetFileName(arquivo);
                string destino = $"{pastaDest}/{nomeArquivo}";

                // Evitar mover para o mesmo lugar
                if (arquivo != destino && !File.Exists(destino))
                {
                    try
                    {
                        string pastaDestino = Path.GetDirectoryName(destino);
                        if (!Directory.Exists(pastaDestino))
                            Directory.CreateDirectory(pastaDestino);

                        AssetDatabase.MoveAsset(arquivo, destino);
                        Debug.Log($"Movido: {arquivo} -> {destino}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Erro ao mover {arquivo}: {e.Message}");
                    }
                }
            }
        }

        // Categorizar scripts
        ReorganizarScripts(scriptsPaths);

        AssetDatabase.Refresh();
        Debug.Log("Reorganização de pastas concluída!");
    }

    private static void ReorganizarScripts(List<string> scripts)
    {
        Dictionary<string, string> categoriasScripts = new Dictionary<string, string>
        {
            {"Player", "Assets/Code/Player"},
            {"Slime", "Assets/Code/Player"},
            {"Character", "Assets/Code/Player"},
            {"Movement", "Assets/Code/Player"},
            {"Controller", "Assets/Code/Player"},

            {"Enemy", "Assets/Code/Enemies"},
            {"AI", "Assets/Code/Enemies"},
            {"Boss", "Assets/Code/Enemies"},

            {"NPC", "Assets/Code/NPCs"},
            {"Dialogue", "Assets/Code/NPCs"},
            {"Quest", "Assets/Code/NPCs"},

            {"UI", "Assets/Code/UI"},
            {"HUD", "Assets/Code/UI"},
            {"Menu", "Assets/Code/UI"},
            {"Panel", "Assets/Code/UI"},
            {"Button", "Assets/Code/UI"},
            {"Canvas", "Assets/Code/UI"},

            {"Manager", "Assets/Code/Managers"},
            {"Service", "Assets/Code/Managers"},
            {"System", "Assets/Code/Systems"},

            {"Item", "Assets/Code/Gameplay"},
            {"Inventory", "Assets/Code/Gameplay"},
            {"Ability", "Assets/Code/Gameplay"},
            {"Weapon", "Assets/Code/Gameplay"},
            {"Combat", "Assets/Code/Gameplay"},

            {"Utils", "Assets/Code/Utils"},
            {"Helper", "Assets/Code/Utils"},
            {"Extension", "Assets/Code/Utils"}
        };

        foreach (string scriptPath in scripts)
        {
            string nomeArquivo = Path.GetFileNameWithoutExtension(scriptPath);
            string pastaDest = "Assets/Code";

            // Verificar se o nome do arquivo corresponde a alguma categoria
            foreach (var categoria in categoriasScripts)
            {
                if (nomeArquivo.Contains(categoria.Key))
                {
                    pastaDest = categoria.Value;
                    break;
                }
            }

            string destino = $"{pastaDest}/{Path.GetFileName(scriptPath)}";

            // Evitar mover para o mesmo lugar
            if (scriptPath != destino && !File.Exists(destino))
            {
                try
                {
                    string pastaDestino = Path.GetDirectoryName(destino);
                    if (!Directory.Exists(pastaDestino))
                        Directory.CreateDirectory(pastaDestino);

                    AssetDatabase.MoveAsset(scriptPath, destino);
                    Debug.Log($"Script movido: {scriptPath} -> {destino}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Erro ao mover script {scriptPath}: {e.Message}");
                }
            }
        }
    }

    #endregion

    #region Reorganização de Objetos na Cena

    // Categorias para organização da cena
    private static readonly string[] CategoriasCena = {
        "PLAYER",
        "ENVIRONMENT",
        "ENEMIES",
        "NPCS",
        "UI",
        "EFFECTS",
        "MANAGERS",
        "ELEMENTS",
        "REGIONS",
        "OTHERS"
    };

    [MenuItem("Extras/Reorganizar Objetos na Cena")]
    public static void ReorganizarObjetosDaCena()
    {
        // Criar objetos vazios para cada categoria
        Dictionary<string, GameObject> categorias = new Dictionary<string, GameObject>();

        foreach (string categoria in CategoriasCena)
        {
            // Verificar se já existe
            GameObject existente = GameObject.Find(categoria);

            if (existente == null)
            {
                GameObject novaCategoria = new GameObject(categoria);
                Undo.RegisterCreatedObjectUndo(novaCategoria, "Criar categoria");
                categorias.Add(categoria, novaCategoria);
            }
            else
            {
                categorias.Add(categoria, existente);
            }
        }

        // Obter todos os GameObjects raiz da cena
        List<GameObject> rootObjects = new List<GameObject>();
        Scene cenaAtual = EditorSceneManager.GetActiveScene();
        cenaAtual.GetRootGameObjects(rootObjects);

        // Excluir da lista os objetos de categorias que já foram criados
        rootObjects = rootObjects.Where(go => !CategoriasCena.Contains(go.name)).ToList();

        // Dicionário para mapear palavras-chave para categorias
        Dictionary<string, string> mapeamentoPalavrasChave = new Dictionary<string, string>
        {
            {"player", "PLAYER"},
            {"slime", "PLAYER"},
            {"character", "PLAYER"},
            {"protagonist", "PLAYER"},

            {"terrain", "ENVIRONMENT"},
            {"ground", "ENVIRONMENT"},
            {"landscape", "ENVIRONMENT"},
            {"tree", "ENVIRONMENT"},
            {"rock", "ENVIRONMENT"},
            {"platform", "ENVIRONMENT"},
            {"background", "ENVIRONMENT"},
            {"prop", "ENVIRONMENT"},
            {"decoration", "ENVIRONMENT"},

            {"enemy", "ENEMIES"},
            {"monster", "ENEMIES"},
            {"boss", "ENEMIES"},
            {"opponent", "ENEMIES"},
            {"grunt", "ENEMIES"},

            {"npc", "NPCS"},
            {"civilian", "NPCS"},
            {"villager", "NPCS"},
            {"shopkeeper", "NPCS"},
            {"merchant", "NPCS"},
            {"quest", "NPCS"},

            {"canvas", "UI"},
            {"ui", "UI"},
            {"hud", "UI"},
            {"menu", "UI"},
            {"button", "UI"},
            {"panel", "UI"},
            {"text", "UI"},
            {"slider", "UI"},

            {"particle", "EFFECTS"},
            {"effect", "EFFECTS"},
            {"vfx", "EFFECTS"},
            {"fx", "EFFECTS"},
            {"light", "EFFECTS"},

            {"manager", "MANAGERS"},
            {"controller", "MANAGERS"},
            {"system", "MANAGERS"},
            {"spawner", "MANAGERS"},
            {"director", "MANAGERS"},
            {"handler", "MANAGERS"},

            {"item", "ELEMENTS"},
            {"pickup", "ELEMENTS"},
            {"collectible", "ELEMENTS"},
            {"weapon", "ELEMENTS"},
            {"powerup", "ELEMENTS"},
            {"trigger", "ELEMENTS"},

            {"zone", "REGIONS"},
            {"area", "REGIONS"},
            {"region", "REGIONS"},
            {"section", "REGIONS"},
            {"volume", "REGIONS"},
            {"checkpoint", "REGIONS"},
            {"spawn", "REGIONS"}
        };

        // Processar cada GameObject
        foreach (GameObject go in rootObjects)
        {
            string categoria = "OTHERS";
            string nomeLowercase = go.name.ToLower();

            // Verificar se o nome contém alguma palavra-chave
            foreach (var mapeamento in mapeamentoPalavrasChave)
            {
                if (nomeLowercase.Contains(mapeamento.Key))
                {
                    categoria = mapeamento.Value;
                    break;
                }
            }

            // Também verificar componentes para melhor categorização
            if (categoria == "OTHERS")
            {
                if (go.GetComponentInChildren<Camera>() != null)
                    categoria = "PLAYER";
                else if (go.GetComponentInChildren<Canvas>() != null)
                    categoria = "UI";
                else if (go.GetComponentInChildren<ParticleSystem>() != null)
                    categoria = "EFFECTS";
            }

            // Verificar se o GO já está nas categorias
            if (!CategoriasCena.Contains(go.name))
            {
                Undo.SetTransformParent(go.transform, categorias[categoria].transform, "Organizando GameObjects");
                Debug.Log($"Movido: {go.name} -> {categoria}");
            }
        }

        EditorSceneManager.MarkSceneDirty(cenaAtual);
        Debug.Log("Objetos da cena reorganizados com sucesso!");
    }

    [MenuItem("Extras/Agrupar Seleção")]
    public static void AgruparSelecao()
    {
        if (Selection.gameObjects.Length <= 1)
        {
            Debug.LogWarning("Selecione pelo menos dois objetos para agrupar.");
            return;
        }

        GameObject container = new GameObject("Grupo");
        Undo.RegisterCreatedObjectUndo(container, "Criar grupo");

        foreach (GameObject go in Selection.gameObjects)
        {
            Undo.SetTransformParent(go.transform, container.transform, "Agrupando objetos");
        }

        Selection.activeGameObject = container;
        Debug.Log("Objetos agrupados com sucesso!");
    }

    #endregion

    #region Criação de Prefabs do Slime

    [MenuItem("Extras/Criar Prefabs do Slime")]
    public static void CriarPrefabsSlime()
    {
        // Garantir que as pastas existam
        string prefabsPath = "Assets/Prefabs/Player";
        CriarPasta(prefabsPath);

        // Criar prefabs para cada estágio do Slime
        string[] estagiosSlime = { "SlimeSmall", "SlimeMedium", "SlimeLarge", "SlimeKing" };

        foreach (string estagio in estagiosSlime)
        {
            GameObject slimeObj = new GameObject(estagio);

            // Adicionar componentes básicos
            slimeObj.AddComponent<SpriteRenderer>();
            slimeObj.AddComponent<Rigidbody2D>();
            slimeObj.AddComponent<CircleCollider2D>();

            // Criar o prefab
            string prefabPath = $"{prefabsPath}/{estagio}.prefab";
            bool sucesso = false;

            PrefabUtility.SaveAsPrefabAsset(slimeObj, prefabPath, out sucesso);

            if (sucesso)
                Debug.Log($"Prefab '{estagio}' criado com sucesso em {prefabPath}");
            else
                Debug.LogError($"Erro ao criar prefab '{estagio}'");

            // Destruir o objeto temporário
            GameObject.DestroyImmediate(slimeObj);
        }

        AssetDatabase.Refresh();
    }

    #endregion
}
#endif
