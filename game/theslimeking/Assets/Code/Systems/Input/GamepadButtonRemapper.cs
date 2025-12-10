using UnityEngine;
using UnityEngine.InputSystem;

namespace SlimeKing.Core
{
    /// <summary>
    /// Sistema de remapeamento de botões de gamepad para suportar diferentes layouts.
    /// Permite que gamepads com botões invertidos (como Switch ou gamepads genéricos)
    /// funcionem corretamente remapeando os inputs em tempo de execução.
    /// </summary>
    public class GamepadButtonRemapper : MonoBehaviour
    {
        public static GamepadButtonRemapper Instance { get; private set; }

        [System.Serializable]
        public struct GamepadLayoutPreset
        {
            public string name;
            public bool isEnabled;
            public bool swapEastSouth;  // Troca buttonEast com buttonSouth
            public bool swapNorthWest;  // Troca buttonNorth com buttonWest
        }

        [Header("⚙️ Presets de Layout")]
        [SerializeField] private GamepadLayoutPreset[] layoutPresets = new GamepadLayoutPreset[]
        {
            new GamepadLayoutPreset { name = "Xbox (Padrão)", isEnabled = true, swapEastSouth = false, swapNorthWest = false },
            new GamepadLayoutPreset { name = "PlayStation", isEnabled = true, swapEastSouth = false, swapNorthWest = false },
            new GamepadLayoutPreset { name = "Nintendo Switch", isEnabled = true, swapEastSouth = true, swapNorthWest = true },
            new GamepadLayoutPreset { name = "Genérico Invertido", isEnabled = true, swapEastSouth = true, swapNorthWest = false }
        };

        [Header("🎮 Seleção de Layout")]
        [SerializeField] private int selectedPresetIndex = 0;

        private GamepadLayoutPreset _currentLayout;
        private InputSystem_Actions _inputActions;

        private void OnEnable()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();

            ApplyLayoutPreset(selectedPresetIndex);

            UnityEngine.Debug.Log($"[GamepadButtonRemapper] Layout '{_currentLayout.name}' aplicado");
        }

        private void OnDisable()
        {
            if (_inputActions != null)
            {
                _inputActions.Disable();
            }
        }

        /// <summary>
        /// Aplica um preset de layout de gamepad
        /// </summary>
        public void ApplyLayoutPreset(int presetIndex)
        {
            if (presetIndex < 0 || presetIndex >= layoutPresets.Length)
            {
                UnityEngine.Debug.LogWarning($"[GamepadButtonRemapper] Índice de preset inválido: {presetIndex}");
                return;
            }

            _currentLayout = layoutPresets[presetIndex];
            selectedPresetIndex = presetIndex;

            UnityEngine.Debug.Log($"[GamepadButtonRemapper] Aplicando layout: {_currentLayout.name}");
            UnityEngine.Debug.Log($"  Swap East/South: {_currentLayout.swapEastSouth}");
            UnityEngine.Debug.Log($"  Swap North/West: {_currentLayout.swapNorthWest}");
        }

        /// <summary>
        /// Aplica um layout customizado
        /// </summary>
        public void ApplyCustomLayout(bool swapEastSouth, bool swapNorthWest)
        {
            _currentLayout = new GamepadLayoutPreset
            {
                name = "Customizado",
                isEnabled = true,
                swapEastSouth = swapEastSouth,
                swapNorthWest = swapNorthWest
            };

            UnityEngine.Debug.Log($"[GamepadButtonRemapper] Layout customizado aplicado");
            UnityEngine.Debug.Log($"  Swap East/South: {swapEastSouth}");
            UnityEngine.Debug.Log($"  Swap North/West: {swapNorthWest}");
        }

        /// <summary>
        /// Obtém o layout atualmente ativo
        /// </summary>
        public GamepadLayoutPreset GetCurrentLayout() => _currentLayout;

        /// <summary>
        /// Obtém o nome do layout atual
        /// </summary>
        public string GetCurrentLayoutName() => _currentLayout.name;

        /// <summary>
        /// Verifica se um botão deve ser remapeado
        /// </summary>
        public bool IsButtonSwapped(string buttonName)
        {
            if (buttonName == "buttonEast" || buttonName == "buttonSouth")
                return _currentLayout.swapEastSouth;

            if (buttonName == "buttonNorth" || buttonName == "buttonWest")
                return _currentLayout.swapNorthWest;

            return false;
        }
    }
}
