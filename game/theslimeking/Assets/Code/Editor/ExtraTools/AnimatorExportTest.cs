using UnityEngine;
using UnityEditor;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Script de teste específico para validar a correção do IndexOutOfRangeException
    /// </summary>
    public static class AnimatorExportTest
    {
        [MenuItem("Extra Tools/Tests/🧪 Test Animator Export (Fix Verification)")]
        public static void TestAnimatorExportFix()
        {
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject == null)
            {
                EditorUtility.DisplayDialog("Test Animator Export", "Por favor, selecione um GameObject com Animator na hierarquia primeiro.", "OK");
                return;
            }

            Animator animator = selectedObject.GetComponent<Animator>();

            if (animator == null)
            {
                EditorUtility.DisplayDialog("Test Animator Export", "O GameObject selecionado não possui um componente Animator.", "OK");
                return;
            }

            UnityEngine.Debug.Log($"🧪 Testando export de Animator para: {selectedObject.name}");
            UnityEngine.Debug.Log($"📊 Controller: {(animator.runtimeAnimatorController ? animator.runtimeAnimatorController.name : "None")}");
            UnityEngine.Debug.Log($"📊 Parameter Count: {animator.parameterCount}");
            UnityEngine.Debug.Log($"📊 Layer Count: {animator.layerCount}");

            try
            {
                // Testa acesso aos parâmetros individualmente
                for (int i = 0; i < animator.parameterCount; i++)
                {
                    var param = animator.GetParameter(i);
                    UnityEngine.Debug.Log($"✅ Parameter [{i}]: {param.name} ({param.type})");
                }

                // Chama a função de export
                UnifiedExtraTools.MenuExportGameObjectStructure();

                UnityEngine.Debug.Log("✅ Teste de Animator export concluído com sucesso! IndexOutOfRangeException corrigido.");
                EditorUtility.DisplayDialog("Test Success",
                    $"Export do Animator '{selectedObject.name}' realizado com sucesso!\nVerifique a pasta Logs/ para o arquivo de saída.\n\nIndexOutOfRangeException foi corrigido!",
                    "OK");
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"❌ Erro durante o teste de export: {ex.Message}");
                UnityEngine.Debug.LogError($"Stack Trace: {ex.StackTrace}");
                EditorUtility.DisplayDialog("Test Error",
                    $"Erro durante o export:\n{ex.Message}",
                    "OK");
            }
        }

        [MenuItem("Extra Tools/Tests/🧪 Test Animator Export (Fix Verification)", true)]
        public static bool TestAnimatorExportFixValidation()
        {
            GameObject selected = Selection.activeGameObject;
            return selected != null && selected.GetComponent<Animator>() != null;
        }
    }
}