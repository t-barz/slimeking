using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor script que adiciona menu de contexto para configurar objetos como Pickup Items
/// </summary>
public static class SetupPickupItemEditor
{
    [MenuItem("GameObject/Extra Tools/Configure as Pickup Item", false, 0)]
    private static void ConfigureAsPickupItem(MenuCommand menuCommand)
    {
        GameObject selectedObject = menuCommand.context as GameObject;
        ConfigureAsPickupItemPublic(selectedObject);
    }

    public static void ConfigureAsPickupItemPublic(GameObject selectedObject)
    {
        if (selectedObject == null)
        {
            Debug.LogWarning("Nenhum objeto selecionado!");
            return;
        }

        Undo.RegisterCompleteObjectUndo(selectedObject, "Configure as Pickup Item");

        // 1. Remove componentes desnecessários
        RemoveUnnecessaryComponents(selectedObject);

        // 2. Adiciona/Configura Animator
        SetupAnimator(selectedObject);

        // 3. Adiciona/Configura CircleCollider2D
        SetupCollider(selectedObject);

        // 4. Adiciona ItemPickup se não existir
        SetupItemPickup(selectedObject);

        // 5. Cria shadow child se não existir
        SetupShadowChild(selectedObject);

        EditorUtility.SetDirty(selectedObject);
        Debug.Log($"✅ '{selectedObject.name}' configurado como Pickup Item!");
    }

    [MenuItem("GameObject/Extra Tools/Configure as Pickup Item", true)]
    private static bool ValidateConfigureAsPickupItem()
    {
        return Selection.activeGameObject != null;
    }

    private static void RemoveUnnecessaryComponents(GameObject obj)
    {
        // Remove Rigidbody2D
        var rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Undo.DestroyObjectImmediate(rb);
            Debug.Log("Removido: Rigidbody2D");
        }

        // Remove BounceHandler
        var bounceHandler = obj.GetComponent<BounceHandler>();
        if (bounceHandler != null)
        {
            Undo.DestroyObjectImmediate(bounceHandler);
            Debug.Log("Removido: BounceHandler");
        }

        // Remove ItemBuffHandler
        var buffHandler = obj.GetComponent<SlimeMec.Gameplay.ItemBuffHandler>();
        if (buffHandler != null)
        {
            Undo.DestroyObjectImmediate(buffHandler);
            Debug.Log("Removido: ItemBuffHandler");
        }
    }

    private static void SetupAnimator(GameObject obj)
    {
        var animator = obj.GetComponent<Animator>();
        if (animator == null)
        {
            animator = Undo.AddComponent<Animator>(obj);
            Debug.Log("Adicionado: Animator");
        }

        // Remove o controller (deixa null como no item_MushroomA)
        animator.runtimeAnimatorController = null;
        animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
    }

    private static void SetupCollider(GameObject obj)
    {
        var collider = obj.GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = Undo.AddComponent<CircleCollider2D>(obj);
            Debug.Log("Adicionado: CircleCollider2D");
        }

        // Configura como no item_MushroomA
        collider.radius = 0.22f;
        collider.offset = Vector2.zero;
        collider.isTrigger = true;
    }

    private static void SetupItemPickup(GameObject obj)
    {
        var itemPickup = obj.GetComponent<SlimeMec.Gameplay.ItemPickup>();
        if (itemPickup == null)
        {
            Undo.AddComponent<SlimeMec.Gameplay.ItemPickup>(obj);
            Debug.Log("Adicionado: ItemPickup");
        }
    }

    private static void SetupShadowChild(GameObject obj)
    {
        // Verifica se já existe um child chamado shadowA
        Transform existingShadow = obj.transform.Find("shadowA");
        if (existingShadow != null)
        {
            Debug.Log("Shadow child já existe");
            return;
        }

        // Cria novo GameObject shadow
        GameObject shadow = new GameObject("shadowA");
        Undo.RegisterCreatedObjectUndo(shadow, "Create Shadow");
        
        shadow.transform.SetParent(obj.transform);
        shadow.transform.localPosition = Vector3.zero;
        shadow.transform.localRotation = Quaternion.identity;
        shadow.transform.localScale = Vector3.one;

        // Adiciona SpriteRenderer
        var spriteRenderer = Undo.AddComponent<SpriteRenderer>(shadow);
        
        // Tenta encontrar o sprite de shadow
        string[] guids = AssetDatabase.FindAssets("shadowA t:Sprite");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            Sprite shadowSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (shadowSprite != null)
            {
                spriteRenderer.sprite = shadowSprite;
                Debug.Log($"Shadow sprite configurado: {path}");
            }
        }
        else
        {
            Debug.LogWarning("Sprite 'shadowA' não encontrado. Configure manualmente.");
        }

        spriteRenderer.sortingOrder = 0;
        spriteRenderer.color = Color.white;

        Debug.Log("Criado: shadowA child");
    }
}
