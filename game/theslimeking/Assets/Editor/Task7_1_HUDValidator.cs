using UnityEngine;
using UnityEditor;
using SlimeKing.Core;
using TMPro;
using System.Collections.Generic;

namespace TheSlimeKing.Editor
{
    /// <summary>
    /// Editor tool for validating Task 7.1: HUD Crystal Counter Validation
    /// Provides automated checks and manual testing helpers for crystal HUD functionality
    /// </summary>
    public class Task7_1_HUDValidator : EditorWindow
    {
        #region Window Setup
        [MenuItem("The Slime King/Tests/Task 7.1 - HUD Validator")]
        public static void ShowWindow()
        {
            var window = GetWindow<Task7_1_HUDValidator>("Task 7.1: HUD Validator");
            window.minSize = new Vector2(500, 700);
        }
        #endregion

        #region GUI State
        private Vector2 scrollPosition;
        private bool showAutomatedTests = true;
        private bool showManualTests = true;
        private bool showQuickActions = true;
        private bool showValidationResults = true;
        
        private Dictionary<CrystalType, ValidationResult> validationResults = new Dictionary<CrystalType, ValidationResult>();
        private bool hasRunValidation = false;
        #endregion

        #region Validation Data
        private class ValidationResult
        {
            public bool TextReferenceValid;
            public bool FormatCorrect;
            public bool ColorAppropriate;
            public bool IconVisible;
            public string CurrentText;
            public Color CurrentColor;
            public string Issues;
        }
        #endregion

        #region GUI Drawing
        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            DrawHeader();
            EditorGUILayout.Space(10);
            
            DrawRequirementsSection();
            EditorGUILayout.Space(10);
            
            if (showQuickActions)
            {
                DrawQuickActionsSection();
                EditorGUILayout.Space(10);
            }
            
            if (showAutomatedTests)
            {
                DrawAutomatedTestsSection();
                EditorGUILayout.Space(10);
            }
            
            if (showManualTests)
            {
                DrawManualTestsSection();
                EditorGUILayout.Space(10);
            }
            
            if (showValidationResults && hasRunValidation)
            {
                DrawValidationResultsSection();
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("Task 7.1: Validar HUD de Cristais", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Esta ferramenta ajuda a validar que os contadores de cristais no HUD estão funcionando corretamente.\n\n" +
                "Requirements: 3.1, 3.2, 3.3, 3.4",
                MessageType.Info
            );
        }

        private void DrawRequirementsSection()
        {
            EditorGUILayout.LabelField("Requirements Checklist", EditorStyles.boldLabel);
            
            EditorGUILayout.HelpBox(
                "✓ 3.1: CrystalContainer contém marcador visual para cada tipo\n" +
                "✓ 3.2: Count_Text atualiza quando cristal é coletado\n" +
                "✓ 3.3: Formato 'x{número}' é aplicado corretamente\n" +
                "✓ 3.4: Contador mostra 'x0' quando zero\n" +
                "✓ 3.5: HUD se inscreve em OnCrystalCountChanged",
                MessageType.None
            );
        }

        private void DrawQuickActionsSection()
        {
            showQuickActions = EditorGUILayout.Foldout(showQuickActions, "Quick Actions", true, EditorStyles.foldoutHeader);
            if (!showQuickActions) return;
            
            EditorGUI.indentLevel++;
            
            EditorGUILayout.HelpBox("Use estas ações rápidas para testar o HUD durante Play Mode", MessageType.Info);
            
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add 1 to All Crystals", GUILayout.Height(30)))
            {
                AddCrystalsToAll(1);
            }
            if (GUILayout.Button("Add 10 to All Crystals", GUILayout.Height(30)))
            {
                AddCrystalsToAll(10);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reset All Counters", GUILayout.Height(30)))
            {
                ResetAllCrystals();
            }
            if (GUILayout.Button("Set Random Values", GUILayout.Height(30)))
            {
                SetRandomCrystalValues();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Individual Crystal Tests:", EditorStyles.boldLabel);
            
            foreach (CrystalType type in System.Enum.GetValues(typeof(CrystalType)))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(type.ToString(), GUILayout.Width(100));
                
                if (GUILayout.Button("+1", GUILayout.Width(40)))
                {
                    AddCrystal(type, 1);
                }
                if (GUILayout.Button("+5", GUILayout.Width(40)))
                {
                    AddCrystal(type, 5);
                }
                if (GUILayout.Button("+10", GUILayout.Width(50)))
                {
                    AddCrystal(type, 10);
                }
                if (GUILayout.Button("Reset", GUILayout.Width(60)))
                {
                    ResetCrystal(type);
                }
                
                // Show current count
                if (GameManager.Instance != null)
                {
                    int count = GameManager.Instance.GetCrystalCount(type);
                    EditorGUILayout.LabelField($"Current: {count}", GUILayout.Width(80));
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUI.EndDisabledGroup();
            
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("⚠ Entre em Play Mode para usar as Quick Actions", MessageType.Warning);
            }
            
            EditorGUI.indentLevel--;
        }

        private void DrawAutomatedTestsSection()
        {
            showAutomatedTests = EditorGUILayout.Foldout(showAutomatedTests, "Automated Validation", true, EditorStyles.foldoutHeader);
            if (!showAutomatedTests) return;
            
            EditorGUI.indentLevel++;
            
            EditorGUILayout.HelpBox(
                "Executa validações automáticas da estrutura do HUD e referências",
                MessageType.Info
            );
            
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            
            if (GUILayout.Button("Run Automated Validation", GUILayout.Height(40)))
            {
                RunAutomatedValidation();
            }
            
            EditorGUI.EndDisabledGroup();
            
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("⚠ Entre em Play Mode para executar validação automática", MessageType.Warning);
            }
            
            EditorGUI.indentLevel--;
        }

        private void DrawManualTestsSection()
        {
            showManualTests = EditorGUILayout.Foldout(showManualTests, "Manual Test Checklist", true, EditorStyles.foldoutHeader);
            if (!showManualTests) return;
            
            EditorGUI.indentLevel++;
            
            EditorGUILayout.HelpBox(
                "Siga esta checklist para validação manual completa:",
                MessageType.Info
            );
            
            EditorGUILayout.LabelField("Test 7.1.1: Nature Crystal", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("□ Coletar cristal Nature");
            EditorGUILayout.LabelField("□ Verificar contador atualiza (x0 → x1)");
            EditorGUILayout.LabelField("□ Verificar formato 'x{número}'");
            EditorGUILayout.LabelField("□ Verificar cor verde/nature");
            EditorGUILayout.LabelField("□ Verificar ícone visível");
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("Test 7.1.2: Fire Crystal", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("□ Coletar cristal Fire");
            EditorGUILayout.LabelField("□ Verificar contador atualiza");
            EditorGUILayout.LabelField("□ Verificar cor vermelha/laranja");
            EditorGUILayout.LabelField("□ Verificar ícone visível");
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("Test 7.1.3-7.1.6: Water, Shadow, Earth, Air", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("□ Repetir testes para cada tipo");
            EditorGUILayout.LabelField("□ Verificar cores apropriadas");
            EditorGUILayout.LabelField("□ Verificar ícones corretos");
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("Test 7.1.7: Multiple Collections", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("□ Coletar 5 cristais do mesmo tipo");
            EditorGUILayout.LabelField("□ Verificar incremento (x1→x2→x3→x4→x5)");
            EditorGUILayout.LabelField("□ Verificar atualização imediata");
            EditorGUILayout.Space(5);
            
            EditorGUILayout.LabelField("Test 7.1.8: Mixed Collections", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("□ Coletar cristais em ordem aleatória");
            EditorGUILayout.LabelField("□ Verificar contadores independentes");
            EditorGUILayout.LabelField("□ Verificar sem cross-contamination");
            
            EditorGUI.indentLevel--;
        }

        private void DrawValidationResultsSection()
        {
            showValidationResults = EditorGUILayout.Foldout(showValidationResults, "Validation Results", true, EditorStyles.foldoutHeader);
            if (!showValidationResults) return;
            
            EditorGUI.indentLevel++;
            
            bool allPassed = true;
            foreach (var kvp in validationResults)
            {
                CrystalType type = kvp.Key;
                ValidationResult result = kvp.Value;
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                string status = result.TextReferenceValid && result.FormatCorrect ? "✓" : "✗";
                EditorGUILayout.LabelField($"{status} {type}", EditorStyles.boldLabel);
                
                EditorGUI.indentLevel++;
                
                DrawValidationItem("Text Reference", result.TextReferenceValid);
                DrawValidationItem("Format Correct", result.FormatCorrect);
                DrawValidationItem("Color Appropriate", result.ColorAppropriate);
                DrawValidationItem("Icon Visible", result.IconVisible);
                
                if (!string.IsNullOrEmpty(result.CurrentText))
                {
                    EditorGUILayout.LabelField($"Current Text: '{result.CurrentText}'");
                }
                
                if (!string.IsNullOrEmpty(result.Issues))
                {
                    EditorGUILayout.HelpBox(result.Issues, MessageType.Warning);
                    allPassed = false;
                }
                
                EditorGUI.indentLevel--;
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(3);
            }
            
            EditorGUILayout.Space(5);
            
            if (allPassed)
            {
                EditorGUILayout.HelpBox("✓ All automated validations passed!", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("✗ Some validations failed. Check issues above.", MessageType.Warning);
            }
            
            EditorGUI.indentLevel--;
        }

        private void DrawValidationItem(string label, bool passed)
        {
            string icon = passed ? "✓" : "✗";
            Color color = passed ? Color.green : Color.red;
            
            EditorGUILayout.BeginHorizontal();
            
            Color oldColor = GUI.contentColor;
            GUI.contentColor = color;
            EditorGUILayout.LabelField(icon, GUILayout.Width(20));
            GUI.contentColor = oldColor;
            
            EditorGUILayout.LabelField(label);
            
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Quick Actions Implementation
        private void AddCrystalsToAll(int amount)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("[Task7_1_HUDValidator] GameManager.Instance is null!");
                return;
            }
            
            foreach (CrystalType type in System.Enum.GetValues(typeof(CrystalType)))
            {
                GameManager.Instance.AddCrystal(type, amount);
            }
            
            Debug.Log($"[Task7_1_HUDValidator] Added {amount} to all crystal types");
        }

        private void ResetAllCrystals()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("[Task7_1_HUDValidator] GameManager.Instance is null!");
                return;
            }
            
            foreach (CrystalType type in System.Enum.GetValues(typeof(CrystalType)))
            {
                int currentCount = GameManager.Instance.GetCrystalCount(type);
                if (currentCount > 0)
                {
                    GameManager.Instance.RemoveCrystal(type, currentCount);
                }
            }
            
            Debug.Log("[Task7_1_HUDValidator] Reset all crystal counters to 0");
        }

        private void SetRandomCrystalValues()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("[Task7_1_HUDValidator] GameManager.Instance is null!");
                return;
            }
            
            ResetAllCrystals();
            
            foreach (CrystalType type in System.Enum.GetValues(typeof(CrystalType)))
            {
                int randomAmount = Random.Range(0, 50);
                if (randomAmount > 0)
                {
                    GameManager.Instance.AddCrystal(type, randomAmount);
                }
            }
            
            Debug.Log("[Task7_1_HUDValidator] Set random crystal values");
        }

        private void AddCrystal(CrystalType type, int amount)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("[Task7_1_HUDValidator] GameManager.Instance is null!");
                return;
            }
            
            GameManager.Instance.AddCrystal(type, amount);
            Debug.Log($"[Task7_1_HUDValidator] Added {amount} {type} crystal(s)");
        }

        private void ResetCrystal(CrystalType type)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("[Task7_1_HUDValidator] GameManager.Instance is null!");
                return;
            }
            
            int currentCount = GameManager.Instance.GetCrystalCount(type);
            if (currentCount > 0)
            {
                GameManager.Instance.RemoveCrystal(type, currentCount);
            }
            
            Debug.Log($"[Task7_1_HUDValidator] Reset {type} crystal counter to 0");
        }
        #endregion

        #region Automated Validation
        private void RunAutomatedValidation()
        {
            validationResults.Clear();
            hasRunValidation = true;
            
            Debug.Log("[Task7_1_HUDValidator] Starting automated validation...");
            
            // Find CrystalHUDController
            var hudController = FindObjectOfType<TheSlimeKing.UI.CrystalHUDController>();
            if (hudController == null)
            {
                Debug.LogError("[Task7_1_HUDValidator] CrystalHUDController not found in scene!");
                EditorUtility.DisplayDialog("Validation Failed", 
                    "CrystalHUDController not found in scene. Make sure it's attached to CanvasHUD.", 
                    "OK");
                return;
            }
            
            // Find CrystalContainer
            Transform canvasHUD = GameObject.Find("CanvasHUD")?.transform;
            if (canvasHUD == null)
            {
                Debug.LogError("[Task7_1_HUDValidator] CanvasHUD not found in scene!");
                return;
            }
            
            Transform crystalContainer = canvasHUD.Find("CrystalContainer");
            if (crystalContainer == null)
            {
                Debug.LogError("[Task7_1_HUDValidator] CrystalContainer not found!");
                return;
            }
            
            // Validate each crystal type
            foreach (CrystalType type in System.Enum.GetValues(typeof(CrystalType)))
            {
                ValidationResult result = ValidateCrystalType(type, crystalContainer);
                validationResults[type] = result;
            }
            
            Debug.Log("[Task7_1_HUDValidator] Automated validation complete!");
            
            Repaint();
        }

        private ValidationResult ValidateCrystalType(CrystalType type, Transform crystalContainer)
        {
            ValidationResult result = new ValidationResult();
            List<string> issues = new List<string>();
            
            // Find crystal GameObject
            string crystalName = $"Crystal_{type}";
            Transform crystalTransform = crystalContainer.Find(crystalName);
            
            if (crystalTransform == null)
            {
                issues.Add($"GameObject '{crystalName}' not found");
                result.TextReferenceValid = false;
                result.FormatCorrect = false;
                result.ColorAppropriate = false;
                result.IconVisible = false;
                result.Issues = string.Join("\n", issues);
                return result;
            }
            
            // Find Count_Text
            Transform countTextTransform = crystalTransform.Find("Count_Text");
            if (countTextTransform == null)
            {
                issues.Add("Count_Text not found");
                result.TextReferenceValid = false;
            }
            else
            {
                result.TextReferenceValid = true;
                
                // Get TextMeshProUGUI component
                TextMeshProUGUI textComponent = countTextTransform.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    result.CurrentText = textComponent.text;
                    result.CurrentColor = textComponent.color;
                    
                    // Validate format (should be "x{number}")
                    if (result.CurrentText.StartsWith("x"))
                    {
                        result.FormatCorrect = true;
                    }
                    else
                    {
                        result.FormatCorrect = false;
                        issues.Add($"Format incorrect: '{result.CurrentText}' (should start with 'x')");
                    }
                    
                    // Validate color (basic check - not white/black)
                    if (textComponent.color != Color.white && textComponent.color != Color.black)
                    {
                        result.ColorAppropriate = true;
                    }
                    else
                    {
                        result.ColorAppropriate = false;
                        issues.Add("Color may not be appropriate (white or black)");
                    }
                }
                else
                {
                    issues.Add("TextMeshProUGUI component not found");
                    result.FormatCorrect = false;
                }
            }
            
            // Find Icon
            Transform iconTransform = crystalTransform.Find("Icon");
            if (iconTransform == null)
            {
                issues.Add("Icon not found");
                result.IconVisible = false;
            }
            else
            {
                result.IconVisible = iconTransform.gameObject.activeSelf;
                if (!result.IconVisible)
                {
                    issues.Add("Icon is not active");
                }
            }
            
            result.Issues = string.Join("\n", issues);
            return result;
        }
        #endregion
    }
}
