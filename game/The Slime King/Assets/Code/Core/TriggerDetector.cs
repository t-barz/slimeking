using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Detecta quando o Player entra na área de um trigger e exibe o ícone de controle adequado.
/// Este script deve ser anexado a um GameObject que tenha um CircleCollider2D configurado como trigger.
/// </summary>
public class TriggerDetector : MonoBehaviour
{
    [Header("Configurações de Log")]
    [Tooltip("Mensagem personalizada para exibir no log")]
    [SerializeField] private string mensagemLog = "Player detectado na área!";

    [Tooltip("Registrar quando o player entra na área")]
    [SerializeField] private bool logarNaEntrada = false;

    [Tooltip("Registrar enquanto o player permanece na área")]
    [SerializeField] private bool logarNaPermanencia = false;

    [Tooltip("Registrar quando o player sai da área")]
    [SerializeField] private bool logarNaSaida = false;

    [Tooltip("Intervalo em segundos entre logs durante permanência (0 = todo frame)")]
    [SerializeField] private float intervaloLogPermanencia = 1.0f;

    [Header("Referências de Plataformas")]
    [Tooltip("GameObject representando controle genérico")]
    [SerializeField] private GameObject gamepadObj;

    [Tooltip("GameObject representando controle PlayStation")]
    [SerializeField] private GameObject playstationObj;

    [Tooltip("GameObject representando controle Switch")]
    [SerializeField] private GameObject switchObj;

    [Tooltip("GameObject representando controle Xbox")]
    [SerializeField] private GameObject xboxObj;

    [Tooltip("GameObject representando teclado/mouse")]
    [SerializeField] private GameObject keyboardObj;

    [Header("Configurações de Offset")]
    [Tooltip("Offset de posição para o ícone de gamepad genérico")]
    [SerializeField] private Vector3 gamepadOffset = Vector3.zero;

    [Tooltip("Offset de posição para o ícone de PlayStation")]
    [SerializeField] private Vector3 playstationOffset = Vector3.zero;

    [Tooltip("Offset de posição para o ícone de Switch")]
    [SerializeField] private Vector3 switchOffset = Vector3.zero;

    [Tooltip("Offset de posição para o ícone de Xbox")]
    [SerializeField] private Vector3 xboxOffset = Vector3.zero;

    [Tooltip("Offset de posição para o ícone de teclado/mouse")]
    [SerializeField] private Vector3 keyboardOffset = Vector3.zero;

    // Armazena as posições originais dos objetos
    private Vector3 gamepadOriginalPos;
    private Vector3 playstationOriginalPos;
    private Vector3 switchOriginalPos;
    private Vector3 xboxOriginalPos;
    private Vector3 keyboardOriginalPos;

    // Controle de tempo para os logs de permanência
    private float ultimoLogTime = 0f;

    // Controle se o player está na área
    private bool playerNaArea = false;

    private void Start()
    {
        // Salva as posições originais dos objetos
        if (gamepadObj != null) gamepadOriginalPos = gamepadObj.transform.localPosition;
        if (playstationObj != null) playstationOriginalPos = playstationObj.transform.localPosition;
        if (switchObj != null) switchOriginalPos = switchObj.transform.localPosition;
        if (xboxObj != null) xboxOriginalPos = xboxObj.transform.localPosition;
        if (keyboardObj != null) keyboardOriginalPos = keyboardObj.transform.localPosition;

        // Inicialmente, desativa todos os objetos de plataforma
        DesativarTodasPlataformas();
    }

    /// <summary>
    /// Chamado quando outro collider entra no trigger
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            playerNaArea = true;

            if (logarNaEntrada)
            {
                Debug.Log($"{mensagemLog} (Entrada)");
            }

            // Ativa apenas o objeto correspondente à plataforma atual
            AtivarPlataformaCorreta();
        }
    }

    /// <summary>
    /// Chamado enquanto outro collider permanece no trigger
    /// </summary>
    private void OnTriggerStay2D(Collider2D other)
    {
        // Verifica se é o Player e se deve logar na permanência
        if (other.CompareTag("Player") && logarNaPermanencia)
        {
            // Verifica se já passou o intervalo desde o último log
            if (Time.time >= ultimoLogTime + intervaloLogPermanencia)
            {
                Debug.Log($"{mensagemLog} (Permanência)");
                ultimoLogTime = Time.time;
            }

            // Verifica periodicamente a plataforma para casos onde o jogador muda de controle
            // durante o gameplay (a cada 1 segundo)
            if (Time.time >= ultimoLogTime + 1.0f)
            {
                AtivarPlataformaCorreta();
            }
        }
    }

    /// <summary>
    /// Chamado quando outro collider sai do trigger
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        // Verifica se o objeto que saiu tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            playerNaArea = false;

            if (logarNaSaida)
            {
                Debug.Log($"{mensagemLog} (Saída)");
            }

            // Desativa todos os objetos de plataforma
            DesativarTodasPlataformas();
        }
    }

    /// <summary>
    /// Detecta qual plataforma está sendo usada e ativa apenas o objeto correspondente
    /// </summary>
    private void AtivarPlataformaCorreta()
    {
        // Primeiro desativa todos
        DesativarTodasPlataformas();

        // Detecta a plataforma atual
        string plataformaAtual = DetectarPlataformaAtual();

        // Ativa apenas o objeto correspondente e aplica offset
        switch (plataformaAtual)
        {
            case "playstation":
                if (playstationObj != null)
                {
                    playstationObj.transform.localPosition = playstationOriginalPos + playstationOffset;
                    playstationObj.SetActive(true);
                }
                break;

            case "xbox":
                if (xboxObj != null)
                {
                    xboxObj.transform.localPosition = xboxOriginalPos + xboxOffset;
                    xboxObj.SetActive(true);
                }
                break;

            case "switch":
                if (switchObj != null)
                {
                    switchObj.transform.localPosition = switchOriginalPos + switchOffset;
                    switchObj.SetActive(true);
                }
                break;

            case "gamepad":
                if (gamepadObj != null)
                {
                    gamepadObj.transform.localPosition = gamepadOriginalPos + gamepadOffset;
                    gamepadObj.SetActive(true);
                }
                break;

            case "keyboard":
            default:
                if (keyboardObj != null)
                {
                    keyboardObj.transform.localPosition = keyboardOriginalPos + keyboardOffset;
                    keyboardObj.SetActive(true);
                }
                break;
        }

        // Após ativar o objeto, verifique seus renderers
        switch (plataformaAtual)
        {
            case "playstation":
                VerificarECorrigirVisibilidade(playstationObj);
                break;

            case "xbox":
                VerificarECorrigirVisibilidade(xboxObj);
                break;
            case "switch":
                VerificarECorrigirVisibilidade(switchObj);
                break;
            case "gamepad":
                VerificarECorrigirVisibilidade(gamepadObj);
                break;
            case "keyboard":
                VerificarECorrigirVisibilidade(keyboardObj);
                break;
        }
    }

    private void VerificarECorrigirVisibilidade(GameObject obj)
    {
        if (obj == null) return;

        // Verifica se o objeto tem renderer e se está visível
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);
        Debug.Log($"Objeto '{obj.name}' tem {renderers.Length} renderers");

        foreach (var renderer in renderers)
        {
            renderer.enabled = true;

            // Verifica se o sprite está válido
            if (renderer.sprite == null)
                Debug.LogError($"Renderer {renderer.name} não tem sprite atribuído!");

            // Verifica cor/transparência
            Color cor = renderer.color;
            if (cor.a < 0.1f)
            {
                Debug.LogWarning($"Renderer {renderer.name} está quase transparente (alpha={cor.a}), corrigindo...");
                cor.a = 1.0f;
                renderer.color = cor;
            }
        }
    }

    /// <summary>
    /// Desativa todos os objetos de plataforma e restaura suas posições originais
    /// </summary>
    private void DesativarTodasPlataformas()
    {
        if (gamepadObj != null)
        {
            gamepadObj.transform.localPosition = gamepadOriginalPos;
            gamepadObj.SetActive(false);
        }

        if (playstationObj != null)
        {
            playstationObj.transform.localPosition = playstationOriginalPos;
            playstationObj.SetActive(false);
        }

        if (switchObj != null)
        {
            switchObj.transform.localPosition = switchOriginalPos;
            switchObj.SetActive(false);
        }

        if (xboxObj != null)
        {
            xboxObj.transform.localPosition = xboxOriginalPos;
            xboxObj.SetActive(false);
        }

        if (keyboardObj != null)
        {
            keyboardObj.transform.localPosition = keyboardOriginalPos;
            keyboardObj.SetActive(false);
        }
    }

    /// <summary>
    /// Detecta qual plataforma de controle está sendo usada atualmente
    /// </summary>
    /// <returns>Nome da plataforma: "keyboard", "playstation", "xbox", "switch" ou "gamepad"</returns>
    private string DetectarPlataformaAtual()
    {
        // Usando o novo Input System para detectar qual dispositivo está ativo
        if (Gamepad.current == null)
        {
            return "keyboard"; // Teclado e mouse
        }

        // Detecta o tipo específico de gamepad
        var gamepad = Gamepad.current;
        string deviceName = gamepad.displayName.ToLower();

        // Verifica o tipo de controle baseado no nome do dispositivo
        if (deviceName.Contains("playstation") || deviceName.Contains("ps4") || deviceName.Contains("ps5") || deviceName.Contains("dualshock") || deviceName.Contains("dualsense"))
        {
            return "playstation";
        }
        else if (deviceName.Contains("xbox"))
        {
            return "xbox";
        }
        else if (deviceName.Contains("switch") || deviceName.Contains("nintendo"))
        {
            return "switch";
        }

        // Controle genérico
        return "gamepad";
    }

    /// <summary>
    /// Ativa manualmente um dos objetos para testes
    /// </summary>
    /// <param name="plataforma">Nome da plataforma: "keyboard", "playstation", "xbox", "switch" ou "gamepad"</param>
    public void AtivarPlataformaManualmente(string plataforma)
    {
        if (!playerNaArea) return; // Só ativa se o player estiver na área

        DesativarTodasPlataformas();

        switch (plataforma.ToLower())
        {
            case "playstation":
                if (playstationObj != null)
                {
                    playstationObj.transform.localPosition = playstationOriginalPos + playstationOffset;
                    playstationObj.SetActive(true);
                }
                break;

            case "xbox":
                if (xboxObj != null)
                {
                    xboxObj.transform.localPosition = xboxOriginalPos + xboxOffset;
                    xboxObj.SetActive(true);
                }
                break;

            case "switch":
                if (switchObj != null)
                {
                    switchObj.transform.localPosition = switchOriginalPos + switchOffset;
                    switchObj.SetActive(true);
                }
                break;

            case "gamepad":
                if (gamepadObj != null)
                {
                    gamepadObj.transform.localPosition = gamepadOriginalPos + gamepadOffset;
                    gamepadObj.SetActive(true);
                }
                break;

            case "keyboard":
                if (keyboardObj != null)
                {
                    keyboardObj.transform.localPosition = keyboardOriginalPos + keyboardOffset;
                    keyboardObj.SetActive(true);
                }
                break;
        }
    }
}
