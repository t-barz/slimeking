using UnityEngine;
using UnityEditor;
using SlimeKing.Gameplay;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Utilitário para configuração rápida de objetos empurráveis (PushableObject).
    /// Adiciona todos os componentes necessários e configura automaticamente.
    /// 
    /// FUNCIONALIDADES:
    /// • Adiciona CircleCollider2D configurado como Trigger
    /// • Adiciona e configura Rigidbody2D para jogo 2D
    /// • Adiciona componente PushableObject
    /// • Configura todas as propriedades para funcionamento imediato
    /// • Aplica configurações padrão baseadas nas melhores práticas
    /// 
    /// USO:
    /// • Selecione um GameObject na hierarquia
    /// • Clique em GameObject > Quick Config > 📦 Configure as Pushable Object
    /// • O objeto será automaticamente configurado para ser empurrável
    /// </summary>
    public static class PushableObjectQuickConfig
    {
        [MenuItem("GameObject/Extra Tools/📦 Configure as Pushable Object", false, 0)]
        public static void ConfigureAsPushableObject(MenuCommand menuCommand)
        {
            // Obtém o GameObject selecionado
            GameObject targetObject = menuCommand.context as GameObject;

            if (targetObject == null)
            {
                UnityEngine.Debug.LogError("⚠️ PushableObjectQuickConfig: Nenhum GameObject selecionado!");
                return;
            }

            // Registra para Undo
            Undo.RegisterCompleteObjectUndo(targetObject, "Configure as Pushable Object");

            try
            {
                ConfigurePushableObjectComponents(targetObject);
                UnityEngine.Debug.Log($"✅ PushableObject configurado com sucesso: {targetObject.name}");
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"❌ Erro ao configurar PushableObject: {e.Message}");
            }
        }

        /// <summary>
        /// Valida se o menu deve aparecer (só quando há GameObject selecionado)
        /// </summary>
        [MenuItem("GameObject/Extra Tools/📦 Configure as Pushable Object", true)]
        public static bool ValidateConfigureAsPushableObject()
        {
            return Selection.activeGameObject != null;
        }

        /// <summary>
        /// Configura todos os componentes necessários para um PushableObject funcional
        /// </summary>
        /// <param name="targetObject">GameObject a ser configurado</param>
        public static void ConfigurePushableObjectComponents(GameObject targetObject)
        {
            UnityEngine.Debug.Log($"🔧 Iniciando configuração do PushableObject: {targetObject.name}");

            // 1. Configurar CircleCollider2D como Trigger
            ConfigureCollider(targetObject);

            // 2. Configurar Rigidbody2D para jogo 2D
            ConfigureRigidbody(targetObject);

            // 3. Adicionar e configurar PushableObject component
            ConfigurePushableObject(targetObject);

            // 4. Ajustar configurações gerais do GameObject
            ConfigureGeneralSettings(targetObject);

            // Marcar objeto como modificado
            EditorUtility.SetDirty(targetObject);

            UnityEngine.Debug.Log($"📦 Configuração do PushableObject concluída: {targetObject.name}");
        }

        /// <summary>
        /// Configura CircleCollider2D como Trigger para detecção do Player
        /// </summary>
        private static void ConfigureCollider(GameObject targetObject)
        {
            // Remove colliders existentes para evitar conflitos
            Collider2D[] existingColliders = targetObject.GetComponents<Collider2D>();
            foreach (var collider in existingColliders)
            {
                Undo.DestroyObjectImmediate(collider);
            }

            // Adiciona CircleCollider2D
            CircleCollider2D circleCollider = Undo.AddComponent<CircleCollider2D>(targetObject);

            // Configura como Trigger para detecção
            circleCollider.isTrigger = true;

            // Define raio baseado no tamanho do objeto (com margem)
            SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                // Calcula raio baseado no sprite
                Bounds spriteBounds = spriteRenderer.sprite.bounds;
                float maxSize = Mathf.Max(spriteBounds.size.x, spriteBounds.size.y);
                circleCollider.radius = maxSize * 0.7f; // 70% do tamanho do sprite
            }
            else
            {
                // Raio padrão se não há sprite
                circleCollider.radius = 0.75f;
            }

            UnityEngine.Debug.Log($"🔴 CircleCollider2D configurado (raio: {circleCollider.radius:F2})");
        }

        /// <summary>
        /// Configura Rigidbody2D para física 2D apropriada
        /// </summary>
        private static void ConfigureRigidbody(GameObject targetObject)
        {
            // Remove Rigidbody2D existente se houver
            Rigidbody2D existingRigidbody = targetObject.GetComponent<Rigidbody2D>();
            if (existingRigidbody != null)
            {
                Undo.DestroyObjectImmediate(existingRigidbody);
            }

            // Adiciona novo Rigidbody2D
            Rigidbody2D rigidbody = Undo.AddComponent<Rigidbody2D>(targetObject);

            // Configura para jogo 2D top-down
            rigidbody.bodyType = RigidbodyType2D.Dynamic;  // Unity 6.2+ compatible
            rigidbody.gravityScale = 0f;                   // Remove gravidade
            rigidbody.mass = 100000f;                      // Massa alta para objetos pesados
            rigidbody.linearDamping = 5f;                  // Resistência para parada natural (Unity 6.2+)
            rigidbody.angularDamping = 5f;                 // Resistência de rotação (Unity 6.2+)
            rigidbody.freezeRotation = false;              // Permite rotação durante movimento
            rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Melhor detecção de colisão

            UnityEngine.Debug.Log($"🎯 Rigidbody2D configurado para jogo 2D (massa: {rigidbody.mass})");
        }

        /// <summary>
        /// Adiciona e configura o componente PushableObject com valores padrão
        /// </summary>
        private static void ConfigurePushableObject(GameObject targetObject)
        {
            // Remove PushableObject existente se houver
            PushableObject existingPushable = targetObject.GetComponent<PushableObject>();
            if (existingPushable != null)
            {
                Undo.DestroyObjectImmediate(existingPushable);
            }

            // Adiciona PushableObject
            PushableObject pushableObject = Undo.AddComponent<PushableObject>(targetObject);

            // As configurações padrão são definidas no script via SerializeField
            // Não é possível modificar via código, pois são campos privados
            // O usuário pode ajustar no Inspector conforme necessário

            UnityEngine.Debug.Log($"📦 PushableObject adicionado com configurações padrão");
        }

        /// <summary>
        /// Configura configurações gerais do GameObject
        /// </summary>
        private static void ConfigureGeneralSettings(GameObject targetObject)
        {
            // Garante que o objeto está na layer padrão
            if (targetObject.layer == 0)
            {
                UnityEngine.Debug.Log($"🏷️ Layer mantida: Default");
            }

            // Se não tem tag específica, mantém Untagged
            if (targetObject.CompareTag("Untagged"))
            {
                UnityEngine.Debug.Log($"🏷️ Tag mantida: Untagged");
            }

            // Configura SpriteRenderer se existir
            ConfigureSpriteRenderer(targetObject);
        }

        /// <summary>
        /// Configura SpriteRenderer se presente
        /// </summary>
        private static void ConfigureSpriteRenderer(GameObject targetObject)
        {
            SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;

            // Garante que o sorting está correto para jogo 2D
            if (spriteRenderer.sortingLayerName != "Default")
            {
                spriteRenderer.sortingLayerName = "Default";
            }

            // Define ordem de sorting baseada na posição Y (padrão para 2D top-down)
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-targetObject.transform.position.y * 100);

            UnityEngine.Debug.Log($"🎨 SpriteRenderer configurado (sortingOrder: {spriteRenderer.sortingOrder})");
        }

        #region Métodos Helper Públicos

        /// <summary>
        /// Verifica se um GameObject já está configurado como PushableObject
        /// </summary>
        /// <param name="gameObject">GameObject para verificar</param>
        /// <returns>True se já está configurado</returns>
        public static bool IsPushableObjectConfigured(GameObject gameObject)
        {
            if (gameObject == null) return false;

            bool hasPushableObject = gameObject.GetComponent<PushableObject>() != null;
            bool hasCollider = gameObject.GetComponent<CircleCollider2D>() != null;
            bool hasRigidbody = gameObject.GetComponent<Rigidbody2D>() != null;

            return hasPushableObject && hasCollider && hasRigidbody;
        }

        /// <summary>
        /// Valida se um GameObject pode ser configurado como PushableObject
        /// </summary>
        /// <param name="gameObject">GameObject para validar</param>
        /// <returns>Mensagem de erro se houver, null se válido</returns>
        public static string ValidatePushableObjectSetup(GameObject gameObject)
        {
            if (gameObject == null)
                return "GameObject é null";

            // Verifica se já tem PushableObject configurado
            if (IsPushableObjectConfigured(gameObject))
                return "GameObject já está configurado como PushableObject";

            // Verifica se tem SpriteRenderer (recomendado mas não obrigatório)
            if (gameObject.GetComponent<SpriteRenderer>() == null)
                return "Recomenda-se ter um SpriteRenderer para visualização";

            return null; // Válido
        }

        #endregion

        #region Debug Helpers

        /// <summary>
        /// Exibe informações detalhadas de um PushableObject no Console
        /// </summary>
        [MenuItem("GameObject/Extra Tools/📦 Debug Pushable Object Info")]
        public static void DebugPushableObjectInfo()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                UnityEngine.Debug.Log("⚠️ Nenhum GameObject selecionado!");
                return;
            }

            UnityEngine.Debug.Log($"📦 === DEBUG INFO: {selectedObject.name} ===");

            // Verifica configuração
            bool isConfigured = IsPushableObjectConfigured(selectedObject);
            UnityEngine.Debug.Log($"✅ Configurado como PushableObject: {isConfigured}");

            // Componentes
            var pushable = selectedObject.GetComponent<PushableObject>();
            var collider = selectedObject.GetComponent<CircleCollider2D>();
            var rigidbody = selectedObject.GetComponent<Rigidbody2D>();

            UnityEngine.Debug.Log($"📦 PushableObject: {(pushable != null ? "✅" : "❌")}");
            UnityEngine.Debug.Log($"🔴 CircleCollider2D: {(collider != null ? "✅" : "❌")}");
            if (collider != null) UnityEngine.Debug.Log($"   - Trigger: {collider.isTrigger}, Raio: {collider.radius}");

            UnityEngine.Debug.Log($"🎯 Rigidbody2D: {(rigidbody != null ? "✅" : "❌")}");
            if (rigidbody != null) UnityEngine.Debug.Log($"   - BodyType: {rigidbody.bodyType}, Gravity: {rigidbody.gravityScale}, Mass: {rigidbody.mass}");

            // Validação
            string validation = ValidatePushableObjectSetup(selectedObject);
            if (validation != null) UnityEngine.Debug.LogWarning($"⚠️ Validação: {validation}");
        }

        [MenuItem("GameObject/Extra Tools/📦 Debug Pushable Object Info", true)]
        public static bool ValidateDebugPushableObjectInfo()
        {
            return Selection.activeGameObject != null;
        }

        #endregion
    }
}
