using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;

namespace SlimeKing.UI
{
    /// <summary>
    /// Controla a detecção de dispositivos de entrada e atualiza o Animator 
    /// com o tipo de dispositivo e input atuais.
    /// </summary>
    public class InputController : MonoBehaviour
    {
        /// <summary>
        /// Define os tipos de input que podem ser exibidos
        /// </summary>
        public enum InputType
        {
            Start,      // Botão de início/pausa
            Action1,    // Ação primária
            Action2,    // Ação secundária
            Movement    // Movimentação
        }

        [Header("Input Configuration")]
        [Tooltip("Tipo de input que este controlador representa")]
        [SerializeField] private InputType inputType;

        private Animator animator;

        /// <summary>
        /// Constantes que definem os tipos de dispositivos de entrada.
        /// Estes valores são usados no parâmetro "Platform" do Animator.
        /// </summary>
        private const int PLATFORM_KEYBOARD = 1;
        private const int PLATFORM_XBOX = 2;
        private const int PLATFORM_PLAYSTATION = 3;
        private const int PLATFORM_SWITCH = 4;
        private const int PLATFORM_OTHER = 5;

        private void Awake()
        {
            // Ativa o objeto filho correspondente ao InputType
            foreach (Transform child in transform)
            {
                bool shouldBeActive = child.name.Equals(inputType.ToString(),
                    System.StringComparison.OrdinalIgnoreCase);
                child.gameObject.SetActive(shouldBeActive);
            }
        }

        /// <summary>
        /// Inicializa o componente e realiza a primeira detecção de dispositivo.
        /// </summary>
        private void Start()
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator not found on GameObject!");
                return;
            }
            UpdateInputDevice();
        }

        /// <summary>
        /// Verifica mudanças no dispositivo de entrada a cada frame.
        /// </summary>
        private void Update()
        {
            if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
            {
                UpdateInputDevice();
            }
        }

        /// <summary>
        /// Atualiza o parâmetro "Platform" no Animator baseado no dispositivo atual.
        /// </summary>
        private void UpdateInputDevice()
        {
            int platformType = DeterminePlatform();
            animator.SetInteger("Platform", platformType);
            LogInputDevice(platformType);
        }

        /// <summary>
        /// Determina qual plataforma está sendo usada baseado no dispositivo de entrada atual.
        /// </summary>
        /// <returns>
        /// Retorna um inteiro representando a plataforma:
        /// 1: Teclado
        /// 2: Xbox
        /// 3: PlayStation
        /// 4: Nintendo Switch
        /// 5: Outros controles
        /// </returns>
        private int DeterminePlatform()
        {
            if (Gamepad.current == null) return PLATFORM_KEYBOARD;

            return Gamepad.current switch
            {
                XInputController => PLATFORM_XBOX,
                DualShockGamepad => PLATFORM_PLAYSTATION,
                SwitchProControllerHID => PLATFORM_SWITCH,
                _ => PLATFORM_OTHER
            };
        }

        /// <summary>
        /// Registra no console o dispositivo de entrada atual.
        /// </summary>
        /// <param name="platform">O ID da plataforma detectada</param>
        private void LogInputDevice(int platform)
        {
            string deviceName = platform switch
            {
                PLATFORM_KEYBOARD => "Keyboard",
                PLATFORM_XBOX => "Xbox Controller",
                PLATFORM_PLAYSTATION => "PlayStation Controller",
                PLATFORM_SWITCH => "Nintendo Switch Controller",
                _ => $"Other Controller ({Gamepad.current.GetType().Name})"
            };

            string inputTypeName = inputType.ToString();
            Debug.Log($"Active Input Device: {deviceName} | Input Type: {inputTypeName}");
        }

        /// <summary>
        /// Define o tipo de input e atualiza o estado visual
        /// </summary>
        /// <param name="newType">Novo tipo de input a ser configurado</param>
        public void SetInputType(InputType newType)
        {
            if (inputType == newType) return;

            inputType = newType;

            // Atualiza os objetos filhos
            foreach (Transform child in transform)
            {
                bool shouldBeActive = child.name.Equals(inputType.ToString(),
                    System.StringComparison.OrdinalIgnoreCase);
                child.gameObject.SetActive(shouldBeActive);
            }

            // Atualiza o Animator
            if (animator != null)
            {
                animator.SetInteger("InputType", (int)inputType);
            }
        }

        /// <summary>
        /// Retorna o tipo de input configurado para este controlador
        /// </summary>
        public InputType GetInputType() => inputType;
    }
}