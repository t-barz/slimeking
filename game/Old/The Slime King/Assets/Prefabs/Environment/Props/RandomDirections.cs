using UnityEngine;
using UnityEditor;

public class RandomDirections : MonoBehaviour
{
    [Header("Configurações de Flip por Tag")]
    [Tooltip("Tag dos objetos que serão afetados")]
    [SerializeField] private string targetTag = "Environment";

    [Tooltip("Probabilidade de fazer flip horizontal (0 = nunca, 1 = sempre)")]
    [Range(0f, 1f)]
    [SerializeField] private float flipProbability = 0.5f;

    [Tooltip("Usar flip em SpriteRenderer ou Transform")]
    [SerializeField] private bool useTransformScale = false;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    /// <summary>
    /// Aplica flip aleatório a todos os objetos com a tag especificada
    /// </summary>
    [ContextMenu("Apply Random Flip to Tagged Objects")]
    public void ApplyRandomFlipToTaggedObjects()
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            Debug.LogError("[RandomDirections] Tag não especificada!");
            return;
        }

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);

        if (taggedObjects.Length == 0)
        {
            Debug.LogWarning($"[RandomDirections] Nenhum objeto encontrado com a tag '{targetTag}'");
            return;
        }

        int flippedCount = 0;

        foreach (GameObject obj in taggedObjects)
        {
            if (ApplyRandomFlipToObject(obj))
            {
                flippedCount++;
            }
        }

        Debug.Log($"[RandomDirections] Processados {taggedObjects.Length} objetos com tag '{targetTag}' - {flippedCount} foram flipados");
    }

    /// <summary>
    /// Aplica flip aleatório a um objeto específico
    /// </summary>
    private bool ApplyRandomFlipToObject(GameObject obj)
    {
        // Gera valor aleatório entre 0 e 1
        float randomValue = Random.Range(0f, 1f);

        // Verifica se deve fazer flip baseado na probabilidade
        bool shouldFlip = randomValue <= flipProbability;

        if (shouldFlip)
        {
            if (useTransformScale)
            {
                FlipUsingTransform(obj);
            }
            else
            {
                FlipUsingSpriteRenderer(obj);
            }

            if (showDebugLogs)
            {
                Debug.Log($"[RandomDirections] Objeto '{obj.name}' foi flipado horizontalmente");
            }

            return true;
        }
        else
        {
            if (showDebugLogs)
            {
                Debug.Log($"[RandomDirections] Objeto '{obj.name}' manteve direção original");
            }

            return false;
        }
    }

    /// <summary>
    /// Faz flip usando SpriteRenderer.flipX
    /// </summary>
    private void FlipUsingSpriteRenderer(GameObject obj)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX; // Toggle flip

            if (showDebugLogs)
            {
                Debug.Log($"[RandomDirections] Flip aplicado via SpriteRenderer no objeto '{obj.name}'");
            }
        }
        else
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"[RandomDirections] SpriteRenderer não encontrado em '{obj.name}' - usando Transform");
            }
            FlipUsingTransform(obj);
        }
    }

    /// <summary>
    /// Faz flip usando Transform.localScale
    /// </summary>
    private void FlipUsingTransform(GameObject obj)
    {
        Vector3 scale = obj.transform.localScale;
        scale.x *= -1f;
        obj.transform.localScale = scale;

        if (showDebugLogs)
        {
            Debug.Log($"[RandomDirections] Flip aplicado via Transform.localScale no objeto '{obj.name}'");
        }
    }

    /// <summary>
    /// Reseta todos os objetos com a tag para direção original
    /// </summary>
    [ContextMenu("Reset All Tagged Objects")]
    public void ResetAllTaggedObjects()
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            Debug.LogError("[RandomDirections] Tag não especificada!");
            return;
        }

        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject obj in taggedObjects)
        {
            ResetObjectDirection(obj);
        }

        Debug.Log($"[RandomDirections] {taggedObjects.Length} objetos com tag '{targetTag}' foram resetados");
    }

    /// <summary>
    /// Reseta direção de um objeto específico
    /// </summary>
    private void ResetObjectDirection(GameObject obj)
    {
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
        }

        // Garante que o scale X seja positivo
        Vector3 scale = obj.transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        obj.transform.localScale = scale;

        if (showDebugLogs)
        {
            Debug.Log($"[RandomDirections] Direção resetada para o objeto '{obj.name}'");
        }
    }

    /// <summary>
    /// Lista todas as tags disponíveis (para debug)
    /// </summary>
    [ContextMenu("List Available Tags")]
    public void ListAvailableTags()
    {
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

        Debug.Log("=== TAGS DISPONÍVEIS ===");
        for (int i = 0; i < tags.Length; i++)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tags[i]);
            Debug.Log($"{i + 1}. '{tags[i]}' - {objectsWithTag.Length} objetos");
        }
        Debug.Log("========================");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RandomDirections))]
public class RandomDirectionsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Ações Rápidas", EditorStyles.boldLabel);

        RandomDirections script = (RandomDirections)target;

        if (GUILayout.Button("🎲 Aplicar Flip Aleatório"))
        {
            script.ApplyRandomFlipToTaggedObjects();
        }

        if (GUILayout.Button("🔄 Resetar Todos os Objetos"))
        {
            script.ResetAllTaggedObjects();
        }

        if (GUILayout.Button("📋 Listar Tags Disponíveis"))
        {
            script.ListAvailableTags();
        }
    }
}
#endif
