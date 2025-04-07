using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// Classe responsável por reorganizar objetos na cena do Unity.
/// Permite agrupar objetos por componentes, tags ou nomes normalizados, e renomear objetos sequencialmente.
/// </summary>
public class SceneReorganizer : EditorWindow
{
    private bool processRootObjectsOnly = true; // Se true, processa apenas objetos raiz
    private bool groupByComponents = false; // Se true, agrupa objetos por componentes
    private bool groupByTags = false; // Se true, agrupa objetos por tags
    private bool renameSequentially = true; // Se true, renomeia objetos sequencialmente

    /// <summary>
    /// Exibe a janela do Scene Reorganizer no menu Extras.
    /// </summary>
    [MenuItem("Extras/Scene Reorganizer")]
    public static void ShowWindow()
    {
        GetWindow<SceneReorganizer>("Scene Reorganizer");
    }

    /// <summary>
    /// Método OnGUI é chamado para desenhar a interface gráfica da janela.
    /// </summary>
    void OnGUI()
    {
        GUILayout.Label("Scene Object Reorganizer", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        processRootObjectsOnly = EditorGUILayout.Toggle("Process Root Objects Only", processRootObjectsOnly);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Grouping Method", EditorStyles.boldLabel);

        bool oldGroupByComponents = groupByComponents;
        groupByComponents = EditorGUILayout.Toggle("Group By Components", groupByComponents);
        if (groupByComponents && groupByComponents != oldGroupByComponents)
            groupByTags = false;

        bool oldGroupByTags = groupByTags;
        groupByTags = EditorGUILayout.Toggle("Group By Tags", groupByTags);
        if (groupByTags && groupByTags != oldGroupByTags)
            groupByComponents = false;

        EditorGUILayout.Space();

        renameSequentially = EditorGUILayout.Toggle("Rename Sequentially", renameSequentially);

        EditorGUILayout.Space();

        if (GUILayout.Button("Reorganize Scene"))
        {
            ReorganizeScene();
        }
    }

    /// <summary>
    /// Reorganiza os objetos na cena com base nas configurações selecionadas.
    /// </summary>
    void ReorganizeScene()
    {
        // Obtém objetos da cena baseado na configuração
        GameObject[] sceneObjects;

        if (processRootObjectsOnly)
        {
            sceneObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        }
        else
        {
            sceneObjects = FindObjectsOfType<GameObject>();
        }

        // Dicionário para agrupar objetos
        Dictionary<string, List<GameObject>> objectGroups = new Dictionary<string, List<GameObject>>();

        foreach (GameObject obj in sceneObjects)
        {
            // Pula objetos que são filhos se estamos processando apenas objetos raiz
            if (processRootObjectsOnly == false && obj.transform.parent != null)
                continue;

            // Determina a chave de agrupamento baseada no método selecionado
            string groupKey = DetermineGroupKey(obj);

            // Adiciona o objeto ao grupo apropriado
            if (!objectGroups.ContainsKey(groupKey))
            {
                objectGroups[groupKey] = new List<GameObject>();
            }
            objectGroups[groupKey].Add(obj);
        }

        // Cria pastas e organiza objetos
        foreach (var group in objectGroups)
        {
            // Só cria grupos se houver múltiplos objetos do mesmo tipo
            if (group.Value.Count > 1)
            {
                // Cria o GameObject para agrupar
                GameObject folder = new GameObject(group.Key + "s");
                Undo.RegisterCreatedObjectUndo(folder, "Create Group");

                // Move objetos para a pasta
                for (int i = 0; i < group.Value.Count; i++)
                {
                    GameObject obj = group.Value[i];

                    // Normaliza o nome do objeto
                    string normalizedName = NormalizeName(obj.name);

                    // Adiciona números sequenciais se habilitado
                    if (renameSequentially)
                    {
                        normalizedName += "_" + (i + 1);
                    }

                    // Renomeia o objeto se o nome mudou
                    if (obj.name != normalizedName)
                    {
                        Undo.RecordObject(obj, "Normalize Name");
                        obj.name = normalizedName;
                    }

                    // Move o objeto para a pasta mantendo a posição mundial
                    Undo.SetTransformParent(obj.transform, folder.transform, "Group Object");
                }
            }
        }

        Debug.Log("Scene reorganization complete!");
    }

    /// <summary>
    /// Determina a chave de agrupamento para um objeto com base nas configurações selecionadas.
    /// </summary>
    /// <param name="obj">O objeto para determinar a chave de agrupamento.</param>
    /// <returns>A chave de agrupamento.</returns>
    string DetermineGroupKey(GameObject obj)
    {
        // Agrupa por componentes se selecionado
        if (groupByComponents)
        {
            Component[] components = obj.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component != null && !(component is Transform))
                {
                    // Usa o primeiro componente que não é Transform como chave
                    return component.GetType().Name;
                }
            }
        }

        // Agrupa por tags se selecionado
        if (groupByTags && obj.tag != "Untagged")
        {
            return obj.tag;
        }

        // Caso contrário, agrupa pelo nome normalizado
        return NormalizeName(obj.name);
    }

    /// <summary>
    /// Normaliza o nome de um objeto removendo parênteses, números, sufixos comuns e espaços desnecessários.
    /// </summary>
    /// <param name="name">O nome original do objeto.</param>
    /// <returns>O nome normalizado.</returns>
    string NormalizeName(string name)
    {
        // Remove parênteses e seu conteúdo (como em "Cube (1)")
        string normalized = Regex.Replace(name, @"\s*\([^)]*\)", "");

        // Remove números e espaços no final (como em "Cube 1")
        normalized = Regex.Replace(normalized, @"\s*\d+\s*$", "");

        // Remove sufixo "Clone"
        normalized = Regex.Replace(normalized, @"\s*Clone$", "");

        // Remove outros indicadores comuns de duplicação
        normalized = Regex.Replace(normalized, @"\s*copy$", "", RegexOptions.IgnoreCase);
        normalized = Regex.Replace(normalized, @"\s*duplicate$", "", RegexOptions.IgnoreCase);

        return normalized.Trim();
    }
}