using UnityEditor;
using UnityEngine;
using TheSlimeKing.Dialogue;
using System.Linq;

namespace ExtraTools.Editor.NPC
{
    public static class DialogueNPCSetupTool
    {

        [MenuItem("Extra Tools/NPC/Setup Dialogue NPC", false, 10)]
        public static void SetupSelectedDialogueNPC()
        {
            foreach (var obj in Selection.gameObjects)
            {
                SetupDialogueNPC(obj);
            }
        }

        // Adiciona opção ao menu de contexto do GameObject (botão direito no Hierarchy)
        [MenuItem("GameObject/Extra Tools/Setup Dialogue NPC", false, 49)]
        public static void SetupDialogueNPC_Context(MenuCommand command)
        {
            var obj = command.context as GameObject;
            if (obj != null)
            {
                SetupDialogueNPC(obj);
            }
        }

        private static void SetupDialogueNPC(GameObject obj)
        {
            if (obj == null) return;

            // Adiciona DialogueNPC se não existir
            var dialogue = obj.GetComponent<DialogueNPC>();
            if (dialogue == null)
            {
                dialogue = obj.AddComponent<DialogueNPC>();
                Debug.Log($"[DialogueNPCSetupTool] DialogueNPC adicionado em {obj.name}");
            }

            // Garante Collider2D (BoxCollider2D por padrão)
            var collider = obj.GetComponent<Collider2D>();
            if (collider == null)
            {
                collider = obj.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                Debug.Log($"[DialogueNPCSetupTool] BoxCollider2D adicionado em {obj.name}");
            }
            else if (!collider.isTrigger)
            {
                collider.isTrigger = true;
                Debug.Log($"[DialogueNPCSetupTool] Collider2D ajustado para Trigger em {obj.name}");
            }

            // Tag opcional para organização
            if (obj.tag == "Untagged")
            {
                obj.tag = "NPC";
            }

            // Seleciona o objeto para fácil edição
            Selection.activeGameObject = obj;
        }
    }
}