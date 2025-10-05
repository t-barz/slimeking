using UnityEngine;
using UnityEngine.InputSystem;

namespace ExtraTools
{
    /// <summary>
    /// Script de debug para verificar a estrutura de ícones de input
    /// </summary>
    public class InputIconDebugger : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private GameObject inputButton;

        private void Start()
        {
            if (enableDebugLogs)
            {
                DebugInputButtonStructure();
                DebugInputDevices();
            }
        }

        private void DebugInputButtonStructure()
        {
            if (inputButton == null)
            {
                Debug.LogError("[InputIconDebugger] inputButton não está atribuído!");
                return;
            }

            Debug.Log($"[InputIconDebugger] Estrutura do inputButton '{inputButton.name}':");
            Debug.Log($"[InputIconDebugger] - Ativo: {inputButton.activeInHierarchy}");
            Debug.Log($"[InputIconDebugger] - Número de filhos: {inputButton.transform.childCount}");

            for (int i = 0; i < inputButton.transform.childCount; i++)
            {
                Transform child = inputButton.transform.GetChild(i);
                Debug.Log($"[InputIconDebugger] - Filho {i}: '{child.name}' (Ativo: {child.gameObject.activeInHierarchy})");
            }

            // Busca pelos ícones específicos
            var gamepadIcon = inputButton.transform.Find("gamepad");
            var playstationIcon = inputButton.transform.Find("playstation");
            var switchIcon = inputButton.transform.Find("switch");
            var xboxIcon = inputButton.transform.Find("xbox");
            var keyboardIcon = inputButton.transform.Find("keyboard");

            Debug.Log($"[InputIconDebugger] Ícones encontrados:");
            Debug.Log($"[InputIconDebugger] - gamepad: {(gamepadIcon != null ? $"'{gamepadIcon.name}' (Ativo: {gamepadIcon.gameObject.activeInHierarchy})" : "NÃO ENCONTRADO")}");
            Debug.Log($"[InputIconDebugger] - playstation: {(playstationIcon != null ? $"'{playstationIcon.name}' (Ativo: {playstationIcon.gameObject.activeInHierarchy})" : "NÃO ENCONTRADO")}");
            Debug.Log($"[InputIconDebugger] - switch: {(switchIcon != null ? $"'{switchIcon.name}' (Ativo: {switchIcon.gameObject.activeInHierarchy})" : "NÃO ENCONTRADO")}");
            Debug.Log($"[InputIconDebugger] - xbox: {(xboxIcon != null ? $"'{xboxIcon.name}' (Ativo: {xboxIcon.gameObject.activeInHierarchy})" : "NÃO ENCONTRADO")}");
            Debug.Log($"[InputIconDebugger] - keyboard: {(keyboardIcon != null ? $"'{keyboardIcon.name}' (Ativo: {keyboardIcon.gameObject.activeInHierarchy})" : "NÃO ENCONTRADO")}");
        }

        private void DebugInputDevices()
        {
            Debug.Log($"[InputIconDebugger] Dispositivos de Input disponíveis:");
            
            var keyboard = Keyboard.current;
            var gamepad = Gamepad.current;
            
            Debug.Log($"[InputIconDebugger] - Teclado: {(keyboard != null ? $"'{keyboard.displayName}'" : "NÃO ENCONTRADO")}");
            Debug.Log($"[InputIconDebugger] - Gamepad: {(gamepad != null ? $"'{gamepad.displayName}'" : "NÃO ENCONTRADO")}");

            if (InputManager.Instance != null)
            {
                Debug.Log($"[InputIconDebugger] - InputManager ativo: SIM");
            }
            else
            {
                Debug.LogWarning($"[InputIconDebugger] - InputManager ativo: NÃO");
            }
        }

        [UnityEngine.ContextMenu("Debug Input Structure")]
        private void ManualDebug()
        {
            if (Application.isPlaying)
            {
                DebugInputButtonStructure();
                DebugInputDevices();
            }
        }
    }
}