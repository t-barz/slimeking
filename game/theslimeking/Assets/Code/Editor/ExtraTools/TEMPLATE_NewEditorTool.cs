// 🛠️ TEMPLATE PARA NOVAS FERRAMENTAS DE EDITOR
//
// ⚠️ POLÍTICA OBRIGATÓRIA DE MENUS:
// TODOS os menus devem estar sob "Extra Tools/"
// NUNCA criar menus raiz como "SlimeKing/", "ProjectName/", etc.
//
// Use este template ao criar novas ferramentas de editor.

using UnityEngine;
using UnityEditor;

namespace ExtraTools.Editor
{
    /// <summary>
    /// [DESCRIÇÃO DA SUA FERRAMENTA]
    /// </summary>
    public static class YourToolName
    {
        // ✅ EXEMPLO CORRETO: Menu Item
        [MenuItem("Extra Tools/[CATEGORIA]/🔧 Your Tool Name")]
        public static void ExecuteYourTool()
        {
            // Implementação aqui
            UnityEngine.Debug.Log("[Extra Tools] Your Tool executed!");
        }

        // ✅ EXEMPLO CORRETO: Menu Item com validação
        [MenuItem("Extra Tools/[CATEGORIA]/🔧 Your Tool Name", true)]
        public static bool ValidateYourTool()
        {
            return Selection.activeGameObject != null;
        }

        // ✅ EXEMPLO CORRETO: Context Menu
        [MenuItem("GameObject/Extra Tools/🔧 Configure as Something")]
        public static void ConfigureAsSomething()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                UnityEngine.Debug.LogWarning("[Extra Tools] No GameObject selected!");
                return;
            }

            // Configuração aqui
            UnityEngine.Debug.Log($"[Extra Tools] Configured {selected.name}");
        }

        // ✅ EXEMPLO CORRETO: Asset Creation Menu
        [MenuItem("Assets/Create/Extra Tools/🎯 Your Custom Asset")]
        public static void CreateYourAsset()
        {
            // Criação de asset aqui
        }
    }

    // ✅ EXEMPLO CORRETO: Editor Window
    public class YourToolWindow : EditorWindow
    {
        [MenuItem("Extra Tools/[CATEGORIA]/🏠 Your Tool Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<YourToolWindow>("Your Tool");
            window.minSize = new Vector2(400, 300);
        }

        void OnGUI()
        {
            GUILayout.Label("Your Tool Window", EditorStyles.boldLabel);

            if (GUILayout.Button("Execute Action"))
            {
                UnityEngine.Debug.Log("[Extra Tools] Action executed from window!");
            }
        }
    }
}

/* 
CATEGORIAS DISPONÍVEIS (use uma existente ou crie nova):

- Tests/          - Para testes e validações
- Setup/          - Para configuração e integração  
- NPC/            - Para ferramentas de NPC
- Camera/         - Para ferramentas de câmera
- Scene Tools/    - Para ferramentas de cena
- Quest System/   - Para sistema de quests
- Project/        - Para ferramentas de projeto
- Post Processing/ - Para volumes e efeitos
- Debug/          - Para ferramentas de debug
- Assets/         - Para manipulação de assets

EMOJIS RECOMENDADOS:

🏠 Interface/Window    🔧 Setup/Config       🎭 NPC
⚡ Quick Action       🎬 Scene              💨 Spray/Brush  
🎯 Quest/Target       📁 Project/Folder     💬 Dialogue
🌿 Vegetation         🪨 Items              📦 Pushable
✅ Validation         🔍 Analysis           🎨 Visual/Sprite
📊 Stats/Data         ⚙️ Settings           🌐 Global

❌ NÃO FAZER:
[MenuItem("SlimeKing/My Tool")]           // ERRADO!
[MenuItem("The Slime King/My Tool")]      // ERRADO!
[MenuItem("ProjectName/My Tool")]         // ERRADO!
[MenuItem("MyTool/Action")]               // ERRADO!

✅ SEMPRE FAZER:
[MenuItem("Extra Tools/Category/My Tool")]   // CORRETO!
*/