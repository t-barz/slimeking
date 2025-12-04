using UnityEngine;
using UnityEditor;
using SlimeKing.Gameplay;

/// <summary>
/// Editor customizado para facilitar a configuração de PickupItems.
/// Adiciona botões de atalho e validações no Inspector.
/// </summary>
[CustomEditor(typeof(PickupItem))]
public class SetupPickupItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Desenha o Inspector padrão
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Ações Rápidas", EditorStyles.boldLabel);

        PickupItem pickupItem = (PickupItem)target;

        // Botão para configurar Collider2D como Trigger
        if (GUILayout.Button("Configurar Collider como Trigger"))
        {
            Collider2D collider = pickupItem.GetComponent<Collider2D>();
            if (collider != null)
            {
                Undo.RecordObject(collider, "Configurar Collider como Trigger");
                collider.isTrigger = true;
                EditorUtility.SetDirty(collider);
                Debug.Log($"Collider2D de {pickupItem.gameObject.name} configurado como Trigger");
            }
            else
            {
                Debug.LogWarning($"Nenhum Collider2D encontrado em {pickupItem.gameObject.name}");
            }
        }

        // Botão para adicionar CircleCollider2D se não existir
        if (GUILayout.Button("Adicionar CircleCollider2D"))
        {
            if (pickupItem.GetComponent<CircleCollider2D>() == null)
            {
                Undo.AddComponent<CircleCollider2D>(pickupItem.gameObject);
                CircleCollider2D collider = pickupItem.GetComponent<CircleCollider2D>();
                collider.isTrigger = true;
                collider.radius = 0.5f;
                EditorUtility.SetDirty(pickupItem.gameObject);
                Debug.Log($"CircleCollider2D adicionado a {pickupItem.gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"{pickupItem.gameObject.name} já possui um CircleCollider2D");
            }
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox(
            "Dicas:\n" +
            "• O Collider2D deve estar configurado como Trigger\n" +
            "• Configure o ItemData com os dados do item\n" +
            "• Ajuste pauseDuration para controlar quanto tempo o player fica parado\n" +
            "• Use enableDebugLogs para debug durante desenvolvimento",
            MessageType.Info
        );
    }
}
