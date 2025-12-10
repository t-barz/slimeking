using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using SlimeKing.Core;

namespace ExtraTools.Editor
{
    public class GamepadRemapperCreator
    {
        [MenuItem("Extra Tools/Setup/Create Gamepad Button Remapper")]
        public static void CreateGamepadRemapper()
        {
            // Verifica se já existe
            GamepadButtonRemapper existingRemapper = GameObject.FindFirstObjectByType<GamepadButtonRemapper>();
            if (existingRemapper != null)
            {
                EditorUtility.DisplayDialog(
                    "Gamepad Button Remapper Já Existe",
                    $"Um GamepadButtonRemapper já existe na cena: {existingRemapper.gameObject.name}",
                    "OK"
                );
                return;
            }

            // Cria novo GameObject
            GameObject remapperGO = new GameObject("[System] GamepadButtonRemapper");
            GamepadButtonRemapper remapper = remapperGO.AddComponent<GamepadButtonRemapper>();

            // Marca para salvamento
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            // Seleciona o objeto criado
            Selection.activeGameObject = remapperGO;

            EditorUtility.DisplayDialog(
                "Gamepad Button Remapper Criado",
                "GameObject '[System] GamepadButtonRemapper' foi criado com sucesso!\n\n" +
                "Você pode alterar o layout na Inspector.",
                "OK"
            );
        }
    }
}
