using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;


/// <summary>
/// Controla a detecção de dispositivos de entrada e atualiza o Animator 
/// com o tipo de dispositivo atual.
/// </summary>
public class InputController : MonoBehaviour
{
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

        Debug.Log($"Active Input Device: {deviceName}");
    }
}