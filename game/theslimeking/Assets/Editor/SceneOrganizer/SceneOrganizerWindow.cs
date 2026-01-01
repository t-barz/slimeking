using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace ExtraTools.Editor
{
    /// <summary>
    /// 🗂️ Scene Organizer Tool
    /// 
    /// Organiza a hierarquia da cena seguindo padrões do Coding Standards.
    /// Cria estrutura padronizada de organizadores e reorganiza GameObjects existentes.
    /// 
    /// Acesso: Menu > Extra Tools > Organize > Scene Hierarchy
    /// </summary>
    public class SceneOrganizerWindow : EditorWindow
    {
        private bool showPreview = true;
        private bool createSeparators = true;
        private bool renameObjects = true;
        private bool groupByCategory = true;
        private Vector2 scrollPosition;
        
        private static readonly string[] ORGANIZATION_STRUCTURE = new string[]
        {
            "--- SYSTEMS ---",
            "--- ENVIRONMENT ---",
            "Background",
            "Grid",
            "Scenario",
            "--- GAMEPLAY ---",
            "Player",
            "NPCs",
            "Enemies",
            "Items",
            "--- MECHANICS ---",
            "Mechanics",
            "SpawnPoints",
            "Triggers",
            "--- EFFECTS ---",
            "Lighting",
            "ParticleSystems",
            "PostProcessing",
            "--- UI ---"
        };

        [MenuItem("Extra Tools/Organize/Scene Hierarchy")]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneOrganizerWindow>("🗂️ Scene Organizer");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("🗂️ Scene Hierarchy Organizer", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Organiza a hierarquia da cena ativa seguindo os padrões definidos no Coding Standards.",
                MessageType.Info
            );

            EditorGUILayout.Space(10);
            
            // Options
            EditorGUILayout.LabelField("⚙️ Options", EditorStyles.boldLabel);
            createSeparators = EditorGUILayout.Toggle("Criar Separadores (--- ---)", createSeparators);
            renameObjects = EditorGUILayout.Toggle("Renomear Objetos Fora do Padrão", renameObjects);
            groupByCategory = EditorGUILayout.Toggle("Agrupar por Categoria", groupByCategory);
            showPreview = EditorGUILayout.Toggle("Mostrar Preview", showPreview);

            EditorGUILayout.Space(10);

            // Current Scene Info
            var activeScene = EditorSceneManager.GetActiveScene();
            EditorGUILayout.LabelField("📄 Cena Ativa", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Nome: {activeScene.name}");
            EditorGUILayout.LabelField($"Objetos Raiz: {activeScene.rootCount}");

            EditorGUILayout.Space(10);

            // Preview
            if (showPreview)
            {
                EditorGUILayout.LabelField("👁️ Preview da Estrutura", EditorStyles.boldLabel);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
                
                foreach (var item in ORGANIZATION_STRUCTURE)
                {
                    if (item.StartsWith("---"))
                    {
                        GUI.color = Color.gray;
                        EditorGUILayout.LabelField($"  {item}", EditorStyles.miniLabel);
                        GUI.color = Color.white;
                    }
                    else
                    {
                        EditorGUILayout.LabelField($"  ├── {item}");
                    }
                }
                
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.Space(20);

            // Action Buttons
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("🚀 Organizar Cena", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog(
                    "Confirmar Organização",
                    $"Isso irá reorganizar a hierarquia da cena '{activeScene.name}'.\n\n" +
                    "Esta operação pode ser desfeita (Ctrl+Z).\n\nContinuar?",
                    "Sim, Organizar",
                    "Cancelar"))
                {
                    OrganizeScene();
                }
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(10);

            if (GUILayout.Button("📖 Abrir Coding Standards"))
            {
                var path = "Assets/Docs/CodingStandards.md";
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                if (asset != null)
                {
                    AssetDatabase.OpenAsset(asset);
                }
                else
                {
                    Debug.LogWarning($"Coding Standards não encontrado em: {path}");
                }
            }
        }

        private void OrganizeScene()
        {
            Undo.SetCurrentGroupName("Organize Scene Hierarchy");
            int undoGroup = Undo.GetCurrentGroup();

            var activeScene = EditorSceneManager.GetActiveScene();
            var rootObjects = activeScene.GetRootGameObjects();

            Debug.Log($"[Scene Organizer] Iniciando organização da cena '{activeScene.name}'...");

            // 1. Criar estrutura de organizadores
            Dictionary<string, GameObject> organizers = CreateOrganizers(rootObjects);

            // 2. Reorganizar objetos existentes
            ReorganizeExistingObjects(rootObjects, organizers);

            // 3. Ordenar hierarquia
            SortHierarchy(organizers);

            // 4. Marcar cena como modificada
            EditorSceneManager.MarkSceneDirty(activeScene);

            Undo.CollapseUndoOperations(undoGroup);

            Debug.Log($"[Scene Organizer] ✅ Cena organizada com sucesso!");
            EditorUtility.DisplayDialog(
                "Organização Concluída",
                $"A cena '{activeScene.name}' foi reorganizada seguindo os padrões.\n\n" +
                $"Use Ctrl+Z para desfazer se necessário.",
                "OK"
            );
        }

        private Dictionary<string, GameObject> CreateOrganizers(GameObject[] existingRoots)
        {
            var organizers = new Dictionary<string, GameObject>();

            foreach (var name in ORGANIZATION_STRUCTURE)
            {
                // Verificar se já existe
                var existing = existingRoots.FirstOrDefault(go => go.name == name);
                
                if (existing != null)
                {
                    organizers[name] = existing;
                    continue;
                }

                // Criar novo organizador
                GameObject organizer;
                
                if (name.StartsWith("---"))
                {
                    if (createSeparators)
                    {
                        organizer = new GameObject(name);
                        organizer.SetActive(false);
                        Undo.RegisterCreatedObjectUndo(organizer, "Create Separator");
                        organizers[name] = organizer;
                    }
                }
                else
                {
                    organizer = new GameObject(name);
                    Undo.RegisterCreatedObjectUndo(organizer, "Create Organizer");
                    organizers[name] = organizer;
                }
            }

            return organizers;
        }

        private void ReorganizeExistingObjects(GameObject[] rootObjects, Dictionary<string, GameObject> organizers)
        {
            foreach (var obj in rootObjects)
            {
                // Pular organizadores
                if (organizers.ContainsValue(obj))
                    continue;

                // Determinar categoria
                string category = DetermineCategory(obj);
                
                if (!string.IsNullOrEmpty(category) && organizers.ContainsKey(category))
                {
                    Undo.SetTransformParent(obj.transform, organizers[category].transform, "Reorganize Object");
                }

                // Renomear se necessário
                if (renameObjects)
                {
                    string newName = GetStandardizedName(obj.name);
                    if (newName != obj.name)
                    {
                        Undo.RecordObject(obj, "Rename Object");
                        obj.name = newName;
                    }
                }
            }
        }

        private string DetermineCategory(GameObject obj)
        {
            string name = obj.name.ToLower();

            // Systems
            if (name.Contains("manager") || name.Contains("eventsystem"))
                return organizers.ContainsKey("--- SYSTEMS ---") ? "--- SYSTEMS ---" : null;

            // Environment
            if (name.Contains("background") || name.Contains("sky"))
                return "Background";
            if (name.Contains("grid") || name.Contains("tilemap"))
                return "Grid";
            if (name.Contains("scenario") || name.Contains("rock") || name.Contains("grass") || 
                name.Contains("mushroom") || name.Contains("cave") || name.Contains("prop") ||
                name.Contains("env_"))
                return "Scenario";

            // Gameplay
            if (name.Contains("player"))
                return "Player";
            if (name.Contains("npc") || name.Contains("rick") || name.Contains("helpy"))
                return "NPCs";
            if (name.Contains("enemy") || name.Contains("bee") || name.Contains("gobu"))
                return "Enemies";
            if (name.Contains("apple") || name.Contains("crystal") || name.Contains("item"))
                return "Items";

            // Mechanics
            if (name.Contains("teleport") || name.Contains("shrink") || name.Contains("rolling") ||
                name.Contains("mechanics") || name.Contains("puzzle"))
                return "Mechanics";
            if (name.Contains("spawn"))
                return "SpawnPoints";
            if (name.Contains("trigger"))
                return "Triggers";

            // Effects
            if (name.Contains("light") || name.Contains("global volume"))
                return "Lighting";
            if (name.Contains("particle"))
                return "ParticleSystems";
            if (name.Contains("postprocess") || name.Contains("volume"))
                return "PostProcessing";

            // UI
            if (name.Contains("canvas") || name.Contains("hud"))
                return organizers.ContainsKey("--- UI ---") ? "--- UI ---" : null;

            return null;
        }

        private string GetStandardizedName(string originalName)
        {
            // Remover prefixos comuns
            string name = originalName;
            name = name.Replace("art_", "");
            name = name.Replace("env_", "");
            name = name.Replace("prop_", "");
            name = name.Replace("item_", "");
            
            // Remover numeração automática do Unity
            if (name.Contains(" (") && name.EndsWith(")"))
            {
                int parenIndex = name.LastIndexOf(" (");
                string suffix = name.Substring(parenIndex);
                if (System.Text.RegularExpressions.Regex.IsMatch(suffix, @" \(\d+\)$"))
                {
                    // Converter para underscore + número
                    string number = suffix.Trim('(', ')', ' ');
                    name = name.Substring(0, parenIndex) + "_" + number.PadLeft(2, '0');
                }
            }

            // Converter para PascalCase (simplificado)
            if (name.Contains("_"))
            {
                var parts = name.Split('_');
                name = string.Join("", parts.Select(p => 
                    char.ToUpper(p[0]) + p.Substring(1).ToLower()
                ));
            }
            else if (char.IsLower(name[0]))
            {
                name = char.ToUpper(name[0]) + name.Substring(1);
            }

            return name;
        }

        private void SortHierarchy(Dictionary<string, GameObject> organizers)
        {
            int siblingIndex = 0;
            foreach (var name in ORGANIZATION_STRUCTURE)
            {
                if (organizers.ContainsKey(name))
                {
                    organizers[name].transform.SetSiblingIndex(siblingIndex++);
                }
            }
        }

        private Dictionary<string, GameObject> organizers = new Dictionary<string, GameObject>();
    }
}
