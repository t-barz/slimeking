using UnityEngine;
using UnityEditor;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Ferramenta para configurar um GameObject como NPC automaticamente.
    /// Adiciona colliders, rigidbody e configurações necessárias para detecção de ataques do Player.
    /// </summary>
    public static class NPCSetupTool
    {
        // Menu principal
        [MenuItem("Extra Tools/Setup/Setup as NPC", true)]
        private static bool ValidateSetupAsNPC()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("Extra Tools/Setup/Setup as NPC")]
        public static void SetupAsNPC()
        {
            ExecuteSetup();
        }

        // Menu de contexto (clique direito)
        [MenuItem("GameObject/Extra Tools/Setup as NPC", true)]
        private static bool ValidateContextSetupAsNPC()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("GameObject/Extra Tools/Setup as NPC")]
        public static void ContextSetupAsNPC()
        {
            ExecuteSetup();
        }

        private static void ExecuteSetup()
        {
            if (Selection.activeGameObject == null)
            {
                UnityEngine.Debug.LogWarning("[NPCSetupTool] Nenhum GameObject selecionado.");
                return;
            }

            GameObject target = Selection.activeGameObject;
            UnityEngine.Debug.Log($"[NPCSetupTool] Configurando '{target.name}' como NPC...");

            // Registra operação para Undo
            Undo.RegisterCompleteObjectUndo(target, "Setup as NPC");

            try
            {
                SetupAttributesHandler(target);
                SetupController(target);
                SetupColliders(target);
                SetupRigidbody(target);
                SetupTag(target);

                UnityEngine.Debug.Log($"[NPCSetupTool] ✅ '{target.name}' configurado como NPC com sucesso!");

                // Marca objeto como modificado para salvar
                EditorUtility.SetDirty(target);
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"[NPCSetupTool] ❌ Erro ao configurar NPC: {ex.Message}");
            }
        }

        private static void SetupAttributesHandler(GameObject target)
        {
            // Remove handler existente para evitar duplicatas
            var existingHandler = target.GetComponent<TheSlimeKing.NPCs.NPCAttributesHandler>();
            if (existingHandler != null)
            {
                Undo.DestroyObjectImmediate(existingHandler);
            }

            // Adiciona novo NPCAttributesHandler
            var attributesHandler = Undo.AddComponent<TheSlimeKing.NPCs.NPCAttributesHandler>(target);

            UnityEngine.Debug.Log($"[NPCSetupTool] NPCAttributesHandler adicionado com valores padrão");
        }

        private static void SetupController(GameObject target)
        {
            // Remove controller existente para evitar duplicatas
            var existingController = target.GetComponent<TheSlimeKing.NPCs.NPCBaseController>();
            if (existingController != null)
            {
                Undo.DestroyObjectImmediate(existingController);
            }

            // Adiciona NPCBaseController
            var controller = Undo.AddComponent<TheSlimeKing.NPCs.NPCBaseController>(target);

            UnityEngine.Debug.Log($"[NPCSetupTool] NPCBaseController adicionado");
        }

        private static void SetupColliders(GameObject target)
        {
            // Remove colliders existentes para evitar conflitos
            var existingColliders = target.GetComponents<Collider2D>();
            for (int i = existingColliders.Length - 1; i >= 0; i--)
            {
                Undo.DestroyObjectImmediate(existingColliders[i]);
            }

            // Collider principal (bloqueio físico)
            CircleCollider2D physicsCollider = Undo.AddComponent<CircleCollider2D>(target);
            physicsCollider.radius = 0.12f;
            physicsCollider.offset = new Vector2(0f, 0.12f);
            physicsCollider.isTrigger = false; // Bloqueia movimento do Player

            // Collider de detecção de ataque
            CircleCollider2D attackDetector = Undo.AddComponent<CircleCollider2D>(target);
            attackDetector.radius = 0.15f;
            attackDetector.offset = new Vector2(0f, 0.12f);
            attackDetector.isTrigger = true; // Detecta vfx_* do Player

            UnityEngine.Debug.Log($"[NPCSetupTool] Colliders configurados: Physics (radius: {physicsCollider.radius}) + Attack Detector (radius: {attackDetector.radius})");
        }

        private static void SetupRigidbody(GameObject target)
        {
            // Remove rigidbody existente
            var existingRb = target.GetComponent<Rigidbody2D>();
            if (existingRb != null)
            {
                Undo.DestroyObjectImmediate(existingRb);
            }

            // Adiciona novo Rigidbody2D
            Rigidbody2D rb = Undo.AddComponent<Rigidbody2D>(target);
            rb.bodyType = RigidbodyType2D.Kinematic; // Não afetado por física
            rb.freezeRotation = true; // Não rotaciona
            rb.gravityScale = 0f; // Sem gravidade

            UnityEngine.Debug.Log($"[NPCSetupTool] Rigidbody2D configurado como Kinematic");
        }

        private static void SetupTag(GameObject target)
        {
            // Verifica se a tag "Enemy" existe
            if (!DoesTagExist("Enemy"))
            {
                // Se não existe, usar "Untagged" e avisar
                target.tag = "Untagged";
                UnityEngine.Debug.LogWarning($"[NPCSetupTool] ⚠️ Tag 'Enemy' não encontrada. Configure manualmente a tag '{target.name}' no Inspector.");
                UnityEngine.Debug.LogWarning($"[NPCSetupTool] Para criar a tag: Edit > Project Settings > Tags and Layers > Tags");
            }
            else
            {
                // Se existe, aplicar
                target.tag = "Enemy";
                UnityEngine.Debug.Log($"[NPCSetupTool] Tag configurada como 'Enemy'");
            }
        }

        private static bool DoesTagExist(string tagName)
        {
            // Verifica se a tag existe na lista de tags do projeto
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
            return System.Array.Exists(tags, tag => tag == tagName);
        }
    }
}