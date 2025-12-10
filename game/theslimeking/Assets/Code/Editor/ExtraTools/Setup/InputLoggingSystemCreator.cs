using UnityEngine;
using UnityEditor;
using SlimeKing.Core;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Ferramenta para criar e configurar o InputLoggingSystem na cena 3_InitialForest
    /// </summary>
    public class InputLoggingSystemCreator
    {
        private const string MENU_PATH = "Extra Tools/Setup/Create InputLoggingSystem";

        [MenuItem(MENU_PATH)]
        public static void CreateInputLoggingSystem()
        {
            // Verifica se estamos na cena correta
            var currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            if (!currentScene.name.Contains("3_InitialForest"))
            {
                EditorUtility.DisplayDialog(
                    "Aviso",
                    "Abra a cena '3_InitialForest' para usar esta ferramenta.",
                    "OK"
                );
                return;
            }

            // Verifica se já existe
            var existing = Object.FindObjectOfType<InputLoggingSystem>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog(
                    "Aviso",
                    "InputLoggingSystem já existe na cena.",
                    "OK"
                );
                return;
            }

            // Cria novo GameObject vazio
            GameObject loggingObject = new GameObject("[Debug] InputLoggingSystem");
            loggingObject.transform.position = Vector3.zero;

            // Adiciona o componente
            InputLoggingSystem loggingSystem = loggingObject.AddComponent<InputLoggingSystem>();

            // Marca como dirty para salvar
            EditorUtility.SetDirty(loggingObject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(currentScene);

            EditorUtility.DisplayDialog(
                "Sucesso",
                "InputLoggingSystem criado com sucesso!\n\n" +
                "GameObject: [Debug] InputLoggingSystem\n" +
                "Componente: InputLoggingSystem\n\n" +
                "O sistema começará a registrar inputs automaticamente quando a cena rodar.",
                "OK"
            );

            // Seleciona o objeto criado
            Selection.activeGameObject = loggingObject;
        }

        [MenuItem(MENU_PATH, true)]
        public static bool ValidateCreateInputLoggingSystem()
        {
            // Menu só fica disponível se houver uma cena aberta
            return UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().isLoaded;
        }
    }
}
