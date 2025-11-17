using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Ferramenta para validar que todos os menus seguem a política obrigatória "Extra Tools/"
    /// </summary>
    public class MenuPolicyValidator
    {
        [MenuItem("Extra Tools/Tests/🔍 Validate Menu Policy")]
        public static void ValidateMenuPolicy()
        {
            Debug.Log("=== Validating Menu Policy ===");

            var violations = FindMenuPolicyViolations();

            if (violations.Count == 0)
            {
                Debug.Log("<color=green>✅ ALL MENUS FOLLOW POLICY!</color>");
                Debug.Log("All MenuItem attributes use 'Extra Tools/' prefix correctly.");
            }
            else
            {
                Debug.LogError($"<color=red>❌ FOUND {violations.Count} MENU POLICY VIOLATIONS!</color>");
                Debug.LogError("The following files contain MenuItem attributes that violate the 'Extra Tools/' policy:");

                foreach (var violation in violations)
                {
                    Debug.LogError($"<color=yellow>File:</color> {violation.FilePath}");
                    Debug.LogError($"<color=yellow>Line {violation.LineNumber}:</color> {violation.MenuPath}");
                    Debug.LogError($"<color=yellow>Suggestion:</color> Change to 'Extra Tools/{violation.SuggestedCategory}/'");
                }

                Debug.LogError("\nPLEASE FIX THESE VIOLATIONS:");
                Debug.LogError("- All MenuItem paths must start with 'Extra Tools/'");
                Debug.LogError("- Assets creation menus should use 'Assets/Create/Extra Tools/'");
                Debug.LogError("- See README.md for complete policy details");
            }

            Debug.Log("=== Menu Policy Validation Complete ===");
        }

        private static List<MenuPolicyViolation> FindMenuPolicyViolations()
        {
            var violations = new List<MenuPolicyViolation>();

            // Find all C# files in the project
            string[] csharpFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

            // Regex pattern to find MenuItem attributes
            var menuItemPattern = @"\[MenuItem\s*\(\s*""([^""]+)""\s*[,\)]";

            foreach (string filePath in csharpFiles)
            {
                string[] lines = File.ReadAllLines(filePath);

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i].Trim();

                    var match = Regex.Match(line, menuItemPattern);
                    if (match.Success)
                    {
                        string menuPath = match.Groups[1].Value;

                        // Check if it violates the policy
                        if (IsMenuPolicyViolation(menuPath))
                        {
                            violations.Add(new MenuPolicyViolation
                            {
                                FilePath = GetRelativePath(filePath),
                                LineNumber = i + 1,
                                MenuPath = menuPath,
                                SuggestedCategory = SuggestCategory(menuPath)
                            });
                        }
                    }
                }
            }

            return violations;
        }

        private static bool IsMenuPolicyViolation(string menuPath)
        {
            // Allow Extra Tools menus
            if (menuPath.StartsWith("Extra Tools/"))
                return false;

            // Allow Assets/Create/Extra Tools menus
            if (menuPath.StartsWith("Assets/Create/Extra Tools/"))
                return false;

            // Allow standard Unity menus (Window/, Help/, etc.)
            string[] allowedUnityMenus = { "Window/", "Help/", "Edit/", "Assets/Create/", "Component/" };
            foreach (string allowedMenu in allowedUnityMenus)
            {
                if (menuPath.StartsWith(allowedMenu) && !menuPath.StartsWith("Assets/Create/SlimeKing/"))
                    return false;
            }

            // Allow GameObject context menus that lead to Extra Tools
            if (menuPath.StartsWith("GameObject/Extra Tools/"))
                return false;

            // Everything else is a violation
            return true;
        }

        private static string SuggestCategory(string menuPath)
        {
            // Try to suggest appropriate category based on menu path content
            string lowerPath = menuPath.ToLower();

            if (lowerPath.Contains("test"))
                return "Tests";
            if (lowerPath.Contains("setup") || lowerPath.Contains("config") || lowerPath.Contains("integrat"))
                return "Setup";
            if (lowerPath.Contains("npc"))
                return "NPC";
            if (lowerPath.Contains("camera"))
                return "Camera";
            if (lowerPath.Contains("scene"))
                return "Scene Tools";
            if (lowerPath.Contains("quest"))
                return "Quest System";
            if (lowerPath.Contains("project"))
                return "Project";
            if (lowerPath.Contains("post") || lowerPath.Contains("volume"))
                return "Post Processing";

            return "Category"; // Generic fallback
        }

        private static string GetRelativePath(string absolutePath)
        {
            string assetsPath = Application.dataPath;
            if (absolutePath.StartsWith(assetsPath))
            {
                return "Assets" + absolutePath.Substring(assetsPath.Length).Replace('\\', '/');
            }
            return absolutePath;
        }

        private class MenuPolicyViolation
        {
            public string FilePath { get; set; }
            public int LineNumber { get; set; }
            public string MenuPath { get; set; }
            public string SuggestedCategory { get; set; }
        }
    }
}