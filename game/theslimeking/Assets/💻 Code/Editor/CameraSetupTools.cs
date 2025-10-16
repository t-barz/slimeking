#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using SlimeKing.Core;

namespace SlimeKing.Core.EditorTools
{
    public class CameraSetupTools : EditorWindow
    {
        [MenuItem("Tools/SlimeKing/Camera Setup/Add Camera Manager to Scene")]
        public static void AddCameraManagerToScene()
        {
            // Verifica se já existe um CameraManager na cena
            var existing = FindFirstObjectByType<CameraManager>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog("Camera Manager",
                    $"Camera Manager já existe na cena: {existing.gameObject.name}", "OK");
                Selection.activeGameObject = existing.gameObject;
                return;
            }

            // Cria um novo GameObject com CameraManager
            var cameraManagerObj = new GameObject("Camera Manager");
            cameraManagerObj.AddComponent<CameraManager>();

            // Posiciona no topo da hierarquia
            cameraManagerObj.transform.SetAsFirstSibling();

            // Seleciona o objeto criado
            Selection.activeGameObject = cameraManagerObj;

            Debug.Log("[CameraSetupTools] Camera Manager adicionado à cena");
            EditorUtility.DisplayDialog("Camera Manager",
                "Camera Manager adicionado com sucesso à cena!", "OK");
        }

        [MenuItem("Tools/SlimeKing/Camera Setup/Add Scene Validator to Scene")]
        public static void AddSceneValidatorToScene()
        {
            // Verifica se já existe um SceneSetupValidator na cena
            var existing = FindFirstObjectByType<SceneSetupValidator>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog("Scene Validator",
                    $"Scene Validator já existe na cena: {existing.gameObject.name}", "OK");
                Selection.activeGameObject = existing.gameObject;
                return;
            }

            // Cria um novo GameObject com SceneSetupValidator
            var validatorObj = new GameObject("Scene Validator");
            validatorObj.AddComponent<SceneSetupValidator>();

            // Posiciona no topo da hierarquia
            validatorObj.transform.SetAsFirstSibling();

            // Seleciona o objeto criado
            Selection.activeGameObject = validatorObj;

            Debug.Log("[CameraSetupTools] Scene Validator adicionado à cena");
            EditorUtility.DisplayDialog("Scene Validator",
                "Scene Validator adicionado com sucesso à cena!", "OK");
        }

        [MenuItem("Tools/SlimeKing/Camera Setup/Setup Complete Scene")]
        public static void SetupCompleteScene()
        {
            bool addedCameraManager = false;
            bool addedValidator = false;

            // Adiciona Camera Manager se não existir
            var existingCameraManager = FindFirstObjectByType<CameraManager>();
            if (existingCameraManager == null)
            {
                var cameraManagerObj = new GameObject("Camera Manager");
                cameraManagerObj.AddComponent<CameraManager>();
                cameraManagerObj.transform.SetAsFirstSibling();
                addedCameraManager = true;
            }

            // Adiciona Scene Validator se não existir
            var existingValidator = FindFirstObjectByType<SceneSetupValidator>();
            if (existingValidator == null)
            {
                var validatorObj = new GameObject("Scene Validator");
                validatorObj.AddComponent<SceneSetupValidator>();
                validatorObj.transform.SetAsFirstSibling();
                addedValidator = true;
            }

            string message = "Setup da cena concluído!\n\n";
            if (addedCameraManager) message += "✓ Camera Manager adicionado\n";
            else message += "✓ Camera Manager já existia\n";

            if (addedValidator) message += "✓ Scene Validator adicionado\n";
            else message += "✓ Scene Validator já existia\n";

            message += "\nA cena está pronta para uso!";

            Debug.Log("[CameraSetupTools] " + message.Replace("\n", " "));
            EditorUtility.DisplayDialog("Setup Completo", message, "OK");
        }

        [MenuItem("Tools/SlimeKing/Camera Setup/Validate Current Scene")]
        public static void ValidateCurrentScene()
        {
            var validator = FindFirstObjectByType<SceneSetupValidator>();
            if (validator != null)
            {
                validator.ValidateScene();
                EditorUtility.DisplayDialog("Validação",
                    "Validação executada! Verifique o Console para detalhes.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Validação",
                    "Scene Validator não encontrado na cena.\nUse 'Add Scene Validator to Scene' primeiro.", "OK");
            }
        }

        [MenuItem("Tools/SlimeKing/Camera Setup/Force Camera Refresh")]
        public static void ForceCameraRefresh()
        {
            if (Application.isPlaying)
            {
                var cameraManager = CameraManager.Instance;
                if (cameraManager != null)
                {
                    cameraManager.ForceRefresh();
                    EditorUtility.DisplayDialog("Camera Refresh",
                        "Refresh forçado executado! Verifique o Console para detalhes.", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Camera Refresh",
                        "Camera Manager não encontrado ou não está ativo.", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Camera Refresh",
                    "Esta função só funciona durante o Play Mode.", "OK");
            }
        }

        [MenuItem("Tools/SlimeKing/Camera Setup/Clean Old Files")]
        public static void CleanOldFiles()
        {
            string oldFilePath = "Assets/💻 Code/Editor/SceneSetupValidator.cs";
            if (System.IO.File.Exists(oldFilePath))
            {
                bool result = EditorUtility.DisplayDialog("Limpeza de Arquivos",
                    "Encontrado arquivo antigo do SceneSetupValidator na pasta Editor.\n\n" +
                    "Este arquivo foi movido para Assets/💻 Code/Systems/Validators/\n\n" +
                    "Deseja remover o arquivo antigo?", "Sim", "Não");

                if (result)
                {
                    AssetDatabase.DeleteAsset(oldFilePath);
                    AssetDatabase.Refresh();
                    EditorUtility.DisplayDialog("Limpeza Concluída",
                        "Arquivo antigo removido com sucesso!", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Limpeza",
                    "Nenhum arquivo antigo encontrado para remoção.", "OK");
            }
        }
    }
}
#endif