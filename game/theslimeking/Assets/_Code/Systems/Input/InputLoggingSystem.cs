using UnityEngine;
using UnityEngine.InputSystem;

namespace SlimeKing.Core
{
    /// <summary>
    /// Sistema de log de inputs para debug e monitoramento de controles.
    /// Registra automaticamente o tipo de dispositivo detectado e todos os inputs do jogador.
    /// </summary>
    public class InputLoggingSystem : MonoBehaviour
    {
        [Header("⚙️ Configurações")]
        [SerializeField] private bool enableLogging = true;
        [SerializeField] private bool logInputType = true;
        [SerializeField] private bool logButtonPresses = true;
        [SerializeField] private bool useColoredLogs = true;

        private InputSystem_Actions _inputActions;
        private string _currentControlType = "Nenhum";
        private bool _hasLoggedControlType = false;

        private void OnEnable()
        {
            if (!enableLogging) return;

            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();

            SubscribeToAllInputs();
            DetectControlType();
        }

        private void OnDisable()
        {
            if (_inputActions == null) return;

            UnsubscribeFromAllInputs();
            _inputActions.Disable();
        }

        /// <summary>
        /// Detecta e registra o tipo de controle usado (Teclado, Gamepad, etc.)
        /// </summary>
        private void DetectControlType()
        {
            if (!logInputType) return;

            // Aguarda um pequeno delay para garantir que o sistema de input está pronto
            if (!_hasLoggedControlType)
            {
                LogInputType();
                _hasLoggedControlType = true;
            }
        }

        private void LogInputType()
        {
            string deviceType = "Desconhecido";

            // Verifica qual dispositivo está ativo
            if (Gamepad.current != null)
            {
                deviceType = GetGamepadType();
            }
            else if (Keyboard.current != null)
            {
                deviceType = "Teclado e Mouse";
            }
            else if (Joystick.current != null)
            {
                deviceType = "Joystick";
            }

            _currentControlType = deviceType;

            PrintMessage($"🎮 <b>TIPO DE CONTROLE DETECTADO:</b> {deviceType}", Color.cyan);
        }

        /// <summary>
        /// Obtém o tipo específico de gamepad
        /// </summary>
        private string GetGamepadType()
        {
            var gamepad = Gamepad.current;
            if (gamepad == null) return "Gamepad Desconhecido";

            string layout = gamepad.layout;

            if (layout.Contains("XInput")) return "Xbox Controller (XInput)";
            if (layout.Contains("DualShock") || layout.Contains("DualSense")) return "PlayStation Controller";
            if (layout.Contains("Switch")) return "Nintendo Switch Controller";
            if (layout.Contains("Stadia")) return "Google Stadia Controller";

            return $"Gamepad ({layout})";
        }

        /// <summary>
        /// Subscreve a todos os eventos de input do Gameplay
        /// </summary>
        private void SubscribeToAllInputs()
        {
            // Movimento
            _inputActions.Gameplay.Move.performed += LogMovementInput;
            _inputActions.Gameplay.Move.canceled += LogMovementCanceled;

            // Ações principais
            _inputActions.Gameplay.Attack.performed += (ctx) => LogButton("ATAQUE", "Espaço / B (Gamepad)");
            _inputActions.Gameplay.Interact.performed += (ctx) => LogButton("INTERAGIR", "E / A (Gamepad)");
            _inputActions.Gameplay.Crouch.performed += (ctx) => LogButton("AGACHAR", "X / X (Gamepad)");
            _inputActions.Gameplay.Crouch.canceled += (ctx) => LogButton("AGACHAR CANCELADO", "X / X (Gamepad)");

            // Inventário
            _inputActions.Gameplay.OpenInventory.performed += (ctx) => LogButton("ABRIR INVENTÁRIO", "I / Select (Gamepad)");

            // Quick Slots
            _inputActions.Gameplay.UseItem1.performed += (ctx) => LogButton("USAR ITEM 1", "1 / LB (Gamepad)");
            _inputActions.Gameplay.UseItem2.performed += (ctx) => LogButton("USAR ITEM 2", "2 / LT (Gamepad)");
            _inputActions.Gameplay.UseItem3.performed += (ctx) => LogButton("USAR ITEM 3", "3 / RB (Gamepad)");
            _inputActions.Gameplay.UseItem4.performed += (ctx) => LogButton("USAR ITEM 4", "4 / RT (Gamepad)");

            // Pausa
            _inputActions.Gameplay.PauseGame.performed += (ctx) => LogButton("PAUSAR JOGO", "Esc / Start (Gamepad)");
        }

        private void UnsubscribeFromAllInputs()
        {
            if (_inputActions == null) return;

            // Movimento
            _inputActions.Gameplay.Move.performed -= LogMovementInput;
            _inputActions.Gameplay.Move.canceled -= LogMovementCanceled;

            // Ações principais
            _inputActions.Gameplay.Attack.performed -= (ctx) => LogButton("ATAQUE", "Espaço / B (Gamepad)");
            _inputActions.Gameplay.Interact.performed -= (ctx) => LogButton("INTERAGIR", "E / A (Gamepad)");
            _inputActions.Gameplay.Crouch.performed -= (ctx) => LogButton("AGACHAR", "X / X (Gamepad)");
            _inputActions.Gameplay.Crouch.canceled -= (ctx) => LogButton("AGACHAR CANCELADO", "X / X (Gamepad)");

            // Inventário
            _inputActions.Gameplay.OpenInventory.performed -= (ctx) => LogButton("ABRIR INVENTÁRIO", "I / Select (Gamepad)");

            // Quick Slots
            _inputActions.Gameplay.UseItem1.performed -= (ctx) => LogButton("USAR ITEM 1", "1 / LB (Gamepad)");
            _inputActions.Gameplay.UseItem2.performed -= (ctx) => LogButton("USAR ITEM 2", "2 / LT (Gamepad)");
            _inputActions.Gameplay.UseItem3.performed -= (ctx) => LogButton("USAR ITEM 3", "3 / RB (Gamepad)");
            _inputActions.Gameplay.UseItem4.performed -= (ctx) => LogButton("USAR ITEM 4", "4 / RT (Gamepad)");

            // Pausa
            _inputActions.Gameplay.PauseGame.performed -= (ctx) => LogButton("PAUSAR JOGO", "Esc / Start (Gamepad)");
        }

        /// <summary>
        /// Log de input de movimento
        /// </summary>
        private void LogMovementInput(InputAction.CallbackContext ctx)
        {
            if (!logButtonPresses) return;

            Vector2 movementInput = ctx.ReadValue<Vector2>();
            string direction = GetMovementDirection(movementInput);

            LogButton("MOVIMENTO", direction);
        }

        /// <summary>
        /// Log de cancelamento de movimento
        /// </summary>
        private void LogMovementCanceled(InputAction.CallbackContext ctx)
        {
            if (!logButtonPresses) return;

            LogButton("MOVIMENTO PARADO", "---");
        }

        /// <summary>
        /// Converte valores de movimento em direção legível
        /// </summary>
        private string GetMovementDirection(Vector2 movement)
        {
            if (movement.magnitude < 0.1f) return "Parado";

            string direction = "";

            if (movement.y > 0.5f) direction += "Cima ";
            if (movement.y < -0.5f) direction += "Baixo ";
            if (movement.x > 0.5f) direction += "Direita";
            if (movement.x < -0.5f) direction += "Esquerda";

            return direction.Trim();
        }

        /// <summary>
        /// Log genérico de botão pressionado
        /// </summary>
        private void LogButton(string actionName, string controlInfo)
        {
            if (!logButtonPresses) return;

            string timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff");
            string message = $"[{timestamp}] ▶ {actionName} ({controlInfo})";

            PrintMessage(message, Color.yellow);
        }

        /// <summary>
        /// Sistema de impressão de mensagem centralizado com suporte a cores
        /// </summary>
        private void PrintMessage(string message, Color color)
        {
            if (!enableLogging) return;

            if (useColoredLogs)
            {
                UnityEngine.Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>");
            }
            else
            {
                UnityEngine.Debug.Log($"[InputLoggingSystem] {message}");
            }
        }

        /// <summary>
        /// Obtém informações do tipo de controle atual
        /// </summary>
        public string GetCurrentControlType() => _currentControlType;

        /// <summary>
        /// Habilita/desabilita o sistema de log
        /// </summary>
        public void SetLoggingEnabled(bool enabled)
        {
            enableLogging = enabled;
        }
    }
}
