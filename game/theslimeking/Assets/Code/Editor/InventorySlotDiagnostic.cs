using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TheSlimeKing.UI;

namespace SlimeKing.Editor
{
    /// <summary>
    /// Ferramenta de diagnóstico e correção automática para slots do inventário.
    /// Menu: Tools/Inventory/Diagnose and Fix Slots
    /// </summary>
    public class InventorySlotDiagnostic : EditorWindow
    {
        [MenuItem("Tools/Inventory/Diagnose and Fix Slots")]
        public static void ShowWindow()
        {
            GetWindow<InventorySlotDiagnostic>("Inventory Slot Diagnostic");
        }

        private void OnGUI()
        {
            GUILayout.Label("Diagnóstico de Slots do Inventário", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Diagnosticar Slots na Cena Atual", GUILayout.Height(40)))
            {
                DiagnoseSlots();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Corrigir Automaticamente Todos os Slots", GUILayout.Height(40)))
            {
                FixAllSlots();
            }
        }

        private static void DiagnoseSlots()
        {
            InventorySlotUI[] slots = FindObjectsOfType<InventorySlotUI>(true);
            
            UnityEngine.Debug.Log($"=== DIAGNÓSTICO DE SLOTS DO INVENTÁRIO ===");
            UnityEngine.Debug.Log($"Total de slots encontrados: {slots.Length}");
            UnityEngine.Debug.Log("");

            int slotsWithProblems = 0;

            foreach (var slot in slots)
            {
                UnityEngine.Debug.Log($"Analisando: {slot.gameObject.name}");
                
                // Verifica Image
                Image[] images = slot.GetComponentsInChildren<Image>(true);
                UnityEngine.Debug.Log($"  - Total de Images encontrados: {images.Length}");
                
                foreach (var img in images)
                {
                    UnityEngine.Debug.Log($"    • {img.gameObject.name} (no próprio slot: {img.gameObject == slot.gameObject})");
                }

                // Verifica se tem Image filho (não o próprio)
                bool hasChildImage = false;
                foreach (var img in images)
                {
                    if (img.gameObject != slot.gameObject)
                    {
                        hasChildImage = true;
                        break;
                    }
                }

                if (!hasChildImage)
                {
                    UnityEngine.Debug.LogWarning($"  ⚠️ PROBLEMA: '{slot.gameObject.name}' não tem Image filho para exibir ícones!");
                    slotsWithProblems++;
                }

                UnityEngine.Debug.Log("");
            }

            UnityEngine.Debug.Log($"=== RESUMO ===");
            UnityEngine.Debug.Log($"Slots com problemas: {slotsWithProblems}/{slots.Length}");
            
            if (slotsWithProblems > 0)
            {
                UnityEngine.Debug.LogWarning("Use 'Corrigir Automaticamente Todos os Slots' para criar a estrutura correta.");
            }
            else
            {
                UnityEngine.Debug.Log("✅ Todos os slots estão configurados corretamente!");
            }
        }

        private static void FixAllSlots()
        {
            InventorySlotUI[] slots = FindObjectsOfType<InventorySlotUI>(true);
            
            UnityEngine.Debug.Log($"=== CORREÇÃO AUTOMÁTICA DE SLOTS ===");
            UnityEngine.Debug.Log($"Corrigindo {slots.Length} slots...");
            UnityEngine.Debug.Log("");

            int fixedCount = 0;

            foreach (var slot in slots)
            {
                if (FixSlot(slot))
                {
                    fixedCount++;
                }
            }

            UnityEngine.Debug.Log($"=== RESULTADO ===");
            UnityEngine.Debug.Log($"✅ {fixedCount} slots corrigidos com sucesso!");
            
            // Marca a cena como modificada
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
            );
        }

        private static bool FixSlot(InventorySlotUI slot)
        {
            // Verifica se já tem Image filho
            Image[] images = slot.GetComponentsInChildren<Image>(true);
            bool hasChildImage = false;
            
            foreach (var img in images)
            {
                if (img.gameObject != slot.gameObject)
                {
                    hasChildImage = true;
                    UnityEngine.Debug.Log($"  ✓ '{slot.gameObject.name}' já tem Image filho: {img.gameObject.name}");
                    return false;
                }
            }

            if (!hasChildImage)
            {
                // Cria GameObject filho para o ícone
                GameObject iconObj = new GameObject("Icon");
                iconObj.transform.SetParent(slot.transform, false);
                
                // Adiciona RectTransform
                RectTransform iconRect = iconObj.AddComponent<RectTransform>();
                iconRect.anchorMin = Vector2.zero;
                iconRect.anchorMax = Vector2.one;
                iconRect.sizeDelta = Vector2.zero;
                iconRect.anchoredPosition = Vector2.zero;
                
                // Adiciona Image
                Image iconImage = iconObj.AddComponent<Image>();
                iconImage.raycastTarget = false;
                iconImage.preserveAspect = true;
                
                UnityEngine.Debug.Log($"  ✅ '{slot.gameObject.name}' corrigido! Criado GameObject 'Icon' com Image.");
                return true;
            }

            return false;
        }
    }
}
