using UnityEngine;
using UnityEditor;

namespace SlimeKing.Editor
{
    public class RemoveInventoryCloseButton : EditorWindow
    {
        [MenuItem("SlimeKing/Setup/Remove Inventory Close Button")]
        public static void RemoveCloseButton()
        {
            GameObject inventoryCanvas = GameObject.Find("InventoryCanvas");
            
            if (inventoryCanvas == null)
            {
                EditorUtility.DisplayDialog("Erro", "InventoryCanvas não encontrado na cena!", "OK");
                return;
            }

            Transform inventoryPanel = inventoryCanvas.transform.Find("InventoryPanel");
            if (inventoryPanel == null)
            {
                EditorUtility.DisplayDialog("Erro", "InventoryPanel não encontrado!", "OK");
                return;
            }

            Transform closeButton = inventoryPanel.Find("CloseButton");
            if (closeButton != null)
            {
                DestroyImmediate(closeButton.gameObject);
                
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
                );
                
                EditorUtility.DisplayDialog(
                    "Sucesso", 
                    "CloseButton removido com sucesso!\n\nO inventário agora só pode ser fechado via input (ESC ou botão Inventory).", 
                    "OK"
                );
            }
            else
            {
                EditorUtility.DisplayDialog("Info", "CloseButton já foi removido ou não existe.", "OK");
            }
        }
    }
}
