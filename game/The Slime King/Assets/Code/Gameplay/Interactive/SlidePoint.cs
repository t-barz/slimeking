using UnityEngine;
using UnityEngine.InputSystem;
using TheSlimeKing.Gameplay;

public class SlidePoint : MonoBehaviour
{
    [Header("Configurações de Deslizamento")]
    [Tooltip("Ponto de destino para onde o jogador será movido")]
    [SerializeField] private Transform destinationPoint;

    [Tooltip("Duração do movimento de deslizamento em segundos")]
    [SerializeField] private float slideDuration = 0.2f;

    [Header("Configurações de Input")]
    [Tooltip("Ação de input que ativa o deslizamento")]
    [SerializeField] private InputActionReference slideAction;

    private bool playerInRange = false;
    private bool isListeningToInput = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("UE");
        // Verificar se o ponto de destino está configurado
        if (destinationPoint == null)
        {
            Debug.LogWarning("Ponto de destino não configurado no SlidePoint: " + gameObject.name);
        }

        // Verificar se a ação de input está configurada
        if (slideAction == null)
        {
            Debug.LogWarning("Ação de input não configurada no SlidePoint: " + gameObject.name);
        }
        else
        {
            // Garante que a ação esteja habilitada
            if (!slideAction.action.enabled)
            {
                Debug.LogWarning("A ação de input está desabilitada. Ativando-a agora.");
                slideAction.action.Enable();
            }

            Debug.Log($"SlidePoint inicializado. Ação de input: {slideAction.action.name}, Está ativada: {slideAction.action.enabled}");
        }

        // Verificação do collider
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            Debug.LogError($"SlidePoint ({gameObject.name}) não tem um CircleCollider2D!");
        }
        else if (!circleCollider.isTrigger)
        {
            Debug.LogError($"CircleCollider2D em {gameObject.name} não está configurado como trigger!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Gerenciamento da ação de input
        ManageInputAction();
    }

    // Gerencia a ativação/desativação da ação de input
    private void ManageInputAction()
    {
        if (slideAction != null)
        {
            Debug.Log($"ManageInputAction - playerInRange: {playerInRange}, isListeningToInput: {isListeningToInput}, " +
                      $"Ação: {slideAction.action.name}, Ativada: {slideAction.action.enabled}");

            // Ativa a escuta do input quando o jogador estiver no alcance
            if (playerInRange && !isListeningToInput)
            {
                Debug.Log($"Adicionando listener à ação de slide: {slideAction.action.name}");
                slideAction.action.performed += OnSlideActionPerformed;

                // Garante que a ação esteja habilitada
                if (!slideAction.action.enabled)
                {
                    Debug.Log("Habilitando a ação de slide que estava desabilitada");
                    slideAction.action.Enable();
                }

                isListeningToInput = true;
            }
            // Desativa a escuta do input quando o jogador sair do alcance
            else if (!playerInRange && isListeningToInput)
            {
                Debug.Log($"Removendo listener da ação de slide: {slideAction.action.name}");
                slideAction.action.performed -= OnSlideActionPerformed;
                isListeningToInput = false;

            }
        }
    }

    // Chamado quando a ação de slide é realizada
    private void OnSlideActionPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Ação de deslizamento ativada!");

        // Busca a referência ao jogador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SlimeMovement slimeMovement = player.GetComponent<SlimeMovement>();
            if (slimeMovement != null && destinationPoint != null)
            {
                Debug.Log("Iniciando deslizamento para: " + destinationPoint.position);
                slimeMovement.Slide(destinationPoint.position, slideDuration);
            }
            else
            {
                Debug.LogError($"SlimeMovement ou destinationPoint não encontrado! SlimeMovement: {(slimeMovement != null)}, DestinationPoint: {(destinationPoint != null)}");
            }
        }
        else
        {
            Debug.LogError("Jogador não encontrado com a tag 'Player'!");
        }
    }

    // Chamado quando outro collider entra no trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que entrou em colisão é o jogador
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Jogador entrou na área de deslizamento.");
        }
    }

    // Chamado quando outro collider sai do trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        // Verifica se o objeto que saiu da colisão é o jogador
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Jogador saiu da área de deslizamento.");
        }
    }

    // Método público para verificar se o jogador está na área de deslizamento
    public bool IsPlayerInRange()
    {
        return playerInRange;
    }

    // Importante para garantir que os event listeners sejam removidos
    private void OnDisable()
    {
        if (slideAction != null && isListeningToInput)
        {
            Debug.Log($"OnDisable: Removendo listener da ação de slide: {slideAction.action.name}");
            slideAction.action.performed -= OnSlideActionPerformed;
            isListeningToInput = false;
        }
    }

    // Verificações adicionais quando a aplicação é encerrada
    private void OnApplicationQuit()
    {
        if (slideAction != null && isListeningToInput)
        {
            Debug.Log("OnApplicationQuit: Limpando listeners pendentes");
            slideAction.action.performed -= OnSlideActionPerformed;
            isListeningToInput = false;
        }
    }
}