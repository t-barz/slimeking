using UnityEngine;
using UnityEditor;
using SlimeMec.Gameplay;

namespace SlimeKing.Editor
{
    /// <summary>
    /// Exemplo de uso e testes para o BushQuickConfig.
    /// Demonstra como usar o sistema de configuração rápida para moitas.
    /// </summary>
    public static class BushQuickConfigExample
    {
        [MenuItem("Tools/SlimeKing/🌿 Create Example Bush", false, 100)]
        public static void CreateExampleBush()
        {
            // Cria um GameObject vazio
            GameObject newBush = new GameObject("ExampleBush");

            // Posiciona na origin ou próximo ao objeto selecionado
            if (Selection.activeTransform != null)
            {
                newBush.transform.position = Selection.activeTransform.position + Vector3.right * 2f;
            }

            // Seleciona o novo objeto
            Selection.activeGameObject = newBush;

            // Aplica a configuração de bush
            BushQuickConfig.ConfigureAsBush(new MenuCommand(newBush));

            // Foca na Scene View
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.FrameSelected();
            }

            Debug.Log("✅ Bush de exemplo criada! Verifique o GameObject 'ExampleBush' na cena.");
        }

        [MenuItem("Tools/SlimeKing/🔍 Validate Bush Setup", false, 101)]
        public static void ValidateBushSetup()
        {
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject == null)
            {
                EditorUtility.DisplayDialog("Validação de Bush",
                    "❌ Nenhum GameObject selecionado!\n\nSelecione um GameObject para validar.", "OK");
                return;
            }

            var report = GenerateBushValidationReport(selectedObject);

            // Mostra o relatório em uma janela
            EditorUtility.DisplayDialog("Relatório de Validação de Bush", report.message, "OK");

            // Log detalhado no console
            if (report.isValid)
            {
                Debug.Log($"✅ {selectedObject.name} está configurado corretamente como bush!\n{report.details}");
            }
            else
            {
                Debug.LogWarning($"⚠️ {selectedObject.name} não está completamente configurado como bush.\n{report.details}");
            }
        }

        [MenuItem("Tools/SlimeKing/🔍 Validate Bush Setup", true)]
        public static bool ValidateValidateBushSetup()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("Tools/SlimeKing/📚 Bush Config Documentation", false, 200)]
        public static void ShowDocumentation()
        {
            string documentation = @"
🌿 BUSH QUICK CONFIG - DOCUMENTAÇÃO

═══════════════════════════════════════════════════════════════

📋 COMO USAR:
1. Selecione um GameObject na cena
2. Clique com botão direito → Quick Config → 🌿 Configure as Bush
3. O objeto será automaticamente configurado com todos os componentes

🔧 COMPONENTES ADICIONADOS:
• SpriteRenderer - Renderização do sprite da moita
• Animator - Sistema de animação (Idle, Shake, Destroy)
• CircleCollider2D - Trigger para detecção de player
• WindEmulator - Movimento automático por vento
• BushDestruct - Sistema de destruição (namespace SlimeMec.Gameplay)
• BushShake - Tremor quando player passa
• DropController - Sistema de drop de itens
• RandomStyle - Variações visuais aleatórias

⚙️ CONFIGURAÇÕES AUTOMÁTICAS:
• Tag: 'Destructable'
• Layer: Default (0)
• Escala: (0.91, 0.94, 1.0) - similar ao prefab original
• Material: sprite_lit_default.mat
• Collider: CircleCollider2D trigger (offset: 0, 0.15, radius: 0.15)

🎛️ CONFIGURAÇÃO AVANÇADA:
Use 'Bush Advanced Setup' para controlar quais componentes incluir:
• Sistema de Drop ✓/✗
• Efeito de Vento ✓/✗  
• Randomização Visual ✓/✗
• Shake ao Passar ✓/✗

🐛 DEBUG E VALIDAÇÃO:
• 'Show Bush Info' - Mostra informações detalhadas do objeto
• 'Validate Bush Setup' - Verifica se a configuração está correta
• 'Create Example Bush' - Cria uma bush de exemplo para testes

📁 ARQUIVOS RELACIONADOS:
• Prefab Referência: bushA2.prefab (SlimeMec)
• Controller: bushA2.controller
• Material: sprite_lit_default.mat
• Scripts: WindEmulator, BushDestruct, BushShake, DropController, RandomStyle

⚠️ TROUBLESHOOTING:
• Se algum componente não for adicionado, verifique se os scripts estão compilando
• Se o sprite não aparecer, defina manualmente no SpriteRenderer
• Para drops, configure a lista de prefabs no DropController
• Use namespace SlimeMec.Gameplay para componentes externos

═══════════════════════════════════════════════════════════════
";

            EditorUtility.DisplayDialog("Bush Quick Config - Documentação", documentation, "Fechar");
        }

        private struct ValidationReport
        {
            public bool isValid;
            public string message;
            public string details;
        }

        private static ValidationReport GenerateBushValidationReport(GameObject obj)
        {
            var report = new ValidationReport();
            var details = "";
            int validComponents = 0;
            int totalExpectedComponents = 8; // Número de componentes esperados

            // Verifica componentes essenciais
            if (obj.GetComponent<SpriteRenderer>() != null)
            {
                details += "✅ SpriteRenderer encontrado\n";
                validComponents++;
            }
            else
            {
                details += "❌ SpriteRenderer ausente\n";
            }

            if (obj.GetComponent<Animator>() != null)
            {
                details += "✅ Animator encontrado\n";
                validComponents++;
            }
            else
            {
                details += "❌ Animator ausente\n";
            }

            if (obj.GetComponent<CircleCollider2D>() != null)
            {
                details += "✅ CircleCollider2D encontrado\n";
                validComponents++;
            }
            else
            {
                details += "❌ CircleCollider2D ausente\n";
            }

            if (obj.GetComponent<WindEmulator>() != null)
            {
                details += "✅ WindEmulator encontrado\n";
                validComponents++;
            }
            else
            {
                details += "⚠️ WindEmulator ausente\n";
            }

            if (obj.GetComponent<BushDestruct>() != null)
            {
                details += "✅ BushDestruct encontrado\n";
                validComponents++;
            }
            else
            {
                details += "❌ BushDestruct ausente\n";
            }

            if (obj.GetComponent<BushShake>() != null)
            {
                details += "✅ BushShake encontrado\n";
                validComponents++;
            }
            else
            {
                details += "⚠️ BushShake ausente\n";
            }

            if (obj.GetComponent<DropController>() != null)
            {
                details += "✅ DropController encontrado\n";
                validComponents++;
            }
            else
            {
                details += "⚠️ DropController ausente\n";
            }

            if (obj.GetComponent<RandomStyle>() != null)
            {
                details += "✅ RandomStyle encontrado\n";
                validComponents++;
            }
            else
            {
                details += "⚠️ RandomStyle ausente\n";
            }

            // Verifica configurações
            details += "\n🔧 CONFIGURAÇÕES:\n";
            details += $"Tag: {obj.tag} {(obj.tag == "Destructable" ? "✅" : "⚠️")}\n";
            details += $"Layer: {LayerMask.LayerToName(obj.layer)} ({obj.layer}) {(obj.layer == 0 ? "✅" : "⚠️")}\n";

            var spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                details += $"Sprite: {(spriteRenderer.sprite != null ? spriteRenderer.sprite.name + " ✅" : "Nenhum ⚠️")}\n";
            }

            // Determina se é válido
            report.isValid = validComponents >= 5; // Pelo menos 5 componentes essenciais
            report.details = details;

            if (report.isValid)
            {
                report.message = $"✅ Bush Válida!\n\nComponentes: {validComponents}/{totalExpectedComponents}\n\nO objeto está configurado corretamente como uma bush.";
            }
            else
            {
                report.message = $"⚠️ Bush Incompleta\n\nComponentes: {validComponents}/{totalExpectedComponents}\n\nAlguns componentes essenciais estão ausentes. Use 'Configure as Bush' para corrigir.";
            }

            return report;
        }
    }
}