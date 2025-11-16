using UnityEngine;
using SlimeMec.Visual;

namespace SlimeKing.Visual
{
    /// <summary>
    /// Utilitário para configurar rapidamente objetos com outline.
    /// Facilita a aplicação do sistema de outline a GameObjects.
    /// </summary>
    public static class OutlineUtility
    {
        private const string OUTLINE_SHADER_NAME = "SlimeKing/SpriteOutline";
        private const string OUTLINE_MATERIAL_NAME = "SpriteOutlineMaterial";

        /// <summary>
        /// Configura um GameObject com outline automático para detecção de player.
        /// </summary>
        /// <param name="gameObject">GameObject que receberá o outline</param>
        /// <param name="outlineColor">Cor do outline (padrão: branco)</param>
        /// <param name="detectionRadius">Raio de detecção do player (padrão: 2.0)</param>
        /// <returns>O componente OutlineController criado</returns>
        public static OutlineController SetupAutoOutline(GameObject gameObject, Color outlineColor = default, float detectionRadius = 2.0f)
        {
            if (gameObject == null)
            {
                UnityEngine.Debug.LogError("[OutlineUtility] GameObject é null!");
                return null;
            }

            if (outlineColor == default)
                outlineColor = Color.white;

            // Verifica se tem SpriteRenderer
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                UnityEngine.Debug.LogError($"[OutlineUtility] GameObject '{gameObject.name}' não tem SpriteRenderer!");
                return null;
            }

            // Aplica o material correto
            if (!SetupOutlineMaterial(spriteRenderer))
            {
                UnityEngine.Debug.LogWarning($"[OutlineUtility] Não foi possível configurar material para '{gameObject.name}', mas continuando...");
            }

            // Adiciona o OutlineController se não existir
            OutlineController outlineController = gameObject.GetComponent<OutlineController>();
            if (outlineController == null)
            {
                outlineController = gameObject.AddComponent<OutlineController>();
            }

            // Configura para detecção automática
            outlineController.UpdateOutlineColor(outlineColor);
            outlineController.SetDetectionRadius(detectionRadius);
            outlineController.SetAutoDetection(true);
            outlineController.SetFadeEnabled(true);
            outlineController.UpdateOutlineSize(0.04f);

            UnityEngine.Debug.Log($"[OutlineUtility] ✅ Outline configurado em '{gameObject.name}' - Cor: {outlineColor}, Raio: {detectionRadius}");
            return outlineController;
        }

        /// <summary>
        /// Configura um GameObject com outline manual (sem detecção automática).
        /// </summary>
        /// <param name="gameObject">GameObject que receberá o outline</param>
        /// <param name="outlineColor">Cor do outline (padrão: branco)</param>
        /// <returns>O componente OutlineController criado</returns>
        public static OutlineController SetupManualOutline(GameObject gameObject, Color outlineColor = default)
        {
            if (gameObject == null)
            {
                UnityEngine.Debug.LogError("[OutlineUtility] GameObject é null!");
                return null;
            }

            if (outlineColor == default)
                outlineColor = Color.white;

            // Verifica se tem SpriteRenderer
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                UnityEngine.Debug.LogError($"[OutlineUtility] GameObject '{gameObject.name}' não tem SpriteRenderer!");
                return null;
            }

            // Aplica o material correto
            if (!SetupOutlineMaterial(spriteRenderer))
            {
                UnityEngine.Debug.LogWarning($"[OutlineUtility] Não foi possível configurar material para '{gameObject.name}', mas continuando...");
            }

            // Adiciona o OutlineController se não existir
            OutlineController outlineController = gameObject.GetComponent<OutlineController>();
            if (outlineController == null)
            {
                outlineController = gameObject.AddComponent<OutlineController>();
            }

            // Configura para controle manual
            outlineController.UpdateOutlineColor(outlineColor);
            outlineController.SetAutoDetection(false);
            outlineController.SetFadeEnabled(true);
            outlineController.UpdateOutlineSize(0.04f);

            UnityEngine.Debug.Log($"[OutlineUtility] ✅ Outline manual configurado em '{gameObject.name}' - Cor: {outlineColor}");
            return outlineController;
        }

        /// <summary>
        /// Configura o material correto para outline em um SpriteRenderer.
        /// </summary>
        /// <param name="spriteRenderer">SpriteRenderer que receberá o material</param>
        /// <returns>True se conseguiu configurar o material</returns>
        public static bool SetupOutlineMaterial(SpriteRenderer spriteRenderer)
        {
            if (spriteRenderer == null)
            {
                UnityEngine.Debug.LogError("[OutlineUtility] SpriteRenderer é null!");
                return false;
            }

            // Tenta encontrar o shader correto
            Shader outlineShader = Shader.Find(OUTLINE_SHADER_NAME);
            if (outlineShader == null)
            {
                UnityEngine.Debug.LogError($"[OutlineUtility] Shader '{OUTLINE_SHADER_NAME}' não encontrado!");
                return false;
            }

            // Verifica se o material atual já usa o shader correto
            Material currentMaterial = spriteRenderer.material;
            if (currentMaterial != null && currentMaterial.shader == outlineShader)
            {
                UnityEngine.Debug.Log($"[OutlineUtility] Material já está correto em '{spriteRenderer.name}'");
                return true;
            }

            // Tenta carregar o material padrão
            Material outlineMaterial = Resources.Load<Material>(OUTLINE_MATERIAL_NAME);
            if (outlineMaterial != null)
            {
                spriteRenderer.material = outlineMaterial;
                UnityEngine.Debug.Log($"[OutlineUtility] Material padrão aplicado em '{spriteRenderer.name}'");
                return true;
            }

            // Cria um novo material com o shader correto
            Material newMaterial = new Material(outlineShader);
            newMaterial.name = "Generated_SpriteOutlineMaterial";

            // Copia a textura atual se existir
            if (currentMaterial != null && currentMaterial.HasProperty("_MainTex"))
            {
                newMaterial.SetTexture("_MainTex", currentMaterial.GetTexture("_MainTex"));
            }

            // Configura propriedades padrão
            SetDefaultOutlineProperties(newMaterial);

            spriteRenderer.material = newMaterial;
            UnityEngine.Debug.Log($"[OutlineUtility] Novo material criado e aplicado em '{spriteRenderer.name}'");
            return true;
        }

        /// <summary>
        /// Define propriedades padrão para um material de outline.
        /// </summary>
        /// <param name="material">Material a ser configurado</param>
        public static void SetDefaultOutlineProperties(Material material)
        {
            if (material == null) return;

            material.SetColor("_Color", Color.white);
            material.SetColor("_OutlineColor", Color.white);
            material.SetFloat("_OutlineSize", 0.03f);
            material.SetFloat("_AlphaThreshold", 0.5f);
            material.SetFloat("_ShowOutline", 0f);
            material.SetFloat("_EnableOutline", 0f);
        }

        /// <summary>
        /// Remove o outline de um GameObject.
        /// </summary>
        /// <param name="gameObject">GameObject do qual remover o outline</param>
        public static void RemoveOutline(GameObject gameObject)
        {
            if (gameObject == null) return;

            OutlineController outlineController = gameObject.GetComponent<OutlineController>();
            if (outlineController != null)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(outlineController);
                }
                else
                {
                    Object.DestroyImmediate(outlineController);
                }
                UnityEngine.Debug.Log($"[OutlineUtility] Outline removido de '{gameObject.name}'");
            }
        }

        /// <summary>
        /// Verifica se um GameObject tem outline configurado corretamente.
        /// </summary>
        /// <param name="gameObject">GameObject a verificar</param>
        /// <returns>True se tem outline configurado</returns>
        public static bool HasValidOutline(GameObject gameObject)
        {
            if (gameObject == null) return false;

            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            OutlineController outlineController = gameObject.GetComponent<OutlineController>();

            if (spriteRenderer == null || outlineController == null) return false;

            Material material = spriteRenderer.material;
            if (material == null) return false;

            Shader outlineShader = Shader.Find(OUTLINE_SHADER_NAME);
            return material.shader == outlineShader;
        }
    }
}
