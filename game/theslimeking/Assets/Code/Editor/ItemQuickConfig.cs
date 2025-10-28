using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using SlimeMec.Gameplay;

namespace SlimeKing.Editor
{
    /// <summary>
    /// Utilitário para configuração rápida de itens baseado nos prefabs item_RoundedRockA e item_RedFruit.
    /// Adiciona todos os componentes necessários e configura automaticamente.
    /// </summary>
    public static class ItemQuickConfig
    {
        // Paths dos assets necessários
        private const string ROCK_PREFAB_PATH = "Assets/External/AssetStore/SlimeMec/_Prefabs/Items/item_RoundedRockA.prefab";
        private const string FRUIT_PREFAB_PATH = "Assets/External/AssetStore/SlimeMec/_Prefabs/Items/item_RedFruit.prefab";
        private const string ITEM_MATERIAL_PATH = "Assets/External/AssetStore/SlimeMec/_Art/Materials/sprite_lit_default.mat";
        private const string ROCK_CONTROLLER_PATH = "Assets/External/AssetStore/SlimeMec/_Animation/Item/RoundedRockA/item_RoundedRockA.controller";
        private const string FRUIT_CONTROLLER_PATH = "Assets/External/AssetStore/SlimeMec/_Animation/Item/RedFruit/item_RedFruit.controller";

        [MenuItem("GameObject/Quick Config/🪨 Configure as Item", false, 0)]
        public static void ConfigureAsItem(MenuCommand menuCommand)
        {
            GameObject targetObject = menuCommand.context as GameObject;
            if (targetObject == null)
            {
                Debug.LogError("⚠️ ItemQuickConfig: Nenhum GameObject selecionado!");
                return;
            }
            Undo.RegisterCompleteObjectUndo(targetObject, "Configure as Item");
            try
            {
                ConfigureItemComponents(targetObject);
                Debug.Log($"✅ Item configurado com sucesso: {targetObject.name}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Erro ao configurar item: {e.Message}");
            }
        }

        [MenuItem("GameObject/Quick Config/🪨 Configure as Item", true)]
        public static bool ValidateConfigureAsItem()
        {
            return Selection.activeGameObject != null;
        }

        public static void ConfigureItemComponents(GameObject targetObject)
        {
            // 1. Configurar Tag e Layer
            targetObject.tag = "Untagged";
            targetObject.layer = 0; // Default layer

            // 2. Configurar SpriteRenderer
            ConfigureSpriteRenderer(targetObject);

            // 3. Configurar Animator
            ConfigureAnimator(targetObject);

            // 4. Configurar Collider2D
            ConfigureCollider(targetObject);

            // 5. Adicionar scripts de gameplay
            ConfigureGameplayScripts(targetObject);

            EditorUtility.SetDirty(targetObject);
        }

        private static void ConfigureSpriteRenderer(GameObject targetObject)
        {
            SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = targetObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 0;
            spriteRenderer.color = Color.white;
            Material itemMaterial = AssetDatabase.LoadAssetAtPath<Material>(ITEM_MATERIAL_PATH);
            if (itemMaterial != null)
                spriteRenderer.material = itemMaterial;
            // Tenta carregar sprite do prefab de referência
            if (spriteRenderer.sprite == null)
                LoadItemSprite(spriteRenderer, targetObject.name);
        }

        private static void LoadItemSprite(SpriteRenderer spriteRenderer, string objectName)
        {
            string searchTerm = objectName.ToLower().Contains("fruit") ? "RedFruit" : "RoundedRockA";
            string[] spriteGuids = AssetDatabase.FindAssets($"{searchTerm} t:Sprite", new[] { "Assets/External/AssetStore/SlimeMec/_Art" });
            if (spriteGuids.Length > 0)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(spriteGuids[0]);
                Sprite itemSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if (itemSprite != null)
                {
                    spriteRenderer.sprite = itemSprite;
                    Debug.Log($"🎨 Sprite carregado: {itemSprite.name}");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Nenhum sprite de item encontrado. Defina o sprite manualmente.");
            }
        }

        private static void ConfigureAnimator(GameObject targetObject)
        {
            Animator animator = targetObject.GetComponent<Animator>();
            if (animator == null)
                animator = targetObject.AddComponent<Animator>();
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            // Seleciona controller conforme nome
            if (animator.runtimeAnimatorController == null)
            {
                RuntimeAnimatorController controller = null;
                if (targetObject.name.ToLower().Contains("fruit"))
                    controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(FRUIT_CONTROLLER_PATH);
                else
                    controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ROCK_CONTROLLER_PATH);
                if (controller != null)
                {
                    animator.runtimeAnimatorController = controller;
                    Debug.Log($"🎬 Animator Controller configurado: {controller.name}");
                }
                else
                {
                    Debug.LogWarning("⚠️ Animator Controller não encontrado para o item.");
                }
            }
        }

        private static void ConfigureCollider(GameObject targetObject)
        {
            Collider2D[] existingColliders = targetObject.GetComponents<Collider2D>();
            foreach (var collider in existingColliders)
                Undo.DestroyObjectImmediate(collider);
            // Adiciona collider conforme tipo
            if (targetObject.name.ToLower().Contains("fruit"))
            {
                CircleCollider2D circle = targetObject.AddComponent<CircleCollider2D>();
                circle.isTrigger = false;
                circle.offset = Vector2.zero;
                circle.radius = 0.18f;
                Debug.Log("🔘 CircleCollider2D configurado para fruta");
            }
            else
            {
                // Rock usa geralmente Circle ou Box
                CircleCollider2D circle = targetObject.AddComponent<CircleCollider2D>();
                circle.isTrigger = false;
                circle.offset = Vector2.zero;
                circle.radius = 0.22f;
                Debug.Log("🟤 CircleCollider2D configurado para pedra");
            }
        }

        private static void ConfigureGameplayScripts(GameObject targetObject)
        {
            // BounceHandler
            if (targetObject.GetComponent<BounceHandler>() == null)
                targetObject.AddComponent<BounceHandler>();
            // ItemCollectable
            if (targetObject.GetComponent<ItemCollectable>() == null)
                targetObject.AddComponent<ItemCollectable>();
            // Se for fruta, adiciona ItemBuffHandler
            if (targetObject.name.ToLower().Contains("fruit"))
            {
                if (targetObject.GetComponent<ItemBuffHandler>() == null)
                    targetObject.AddComponent<ItemBuffHandler>();
            }
        }
    }
}
