using UnityEngine;
using UnityEngine.InputSystem;
using TheSlimeKing.Gameplay;

public class SlidePointController : MonoBehaviour
{
    [Header("Configurações de Deslizamento")]
    [Tooltip("Ponto de destino para onde o jogador será movido")]
    [SerializeField] private Transform destinationPoint;

    [Tooltip("Duração do movimento de deslizamento em segundos")]
    [SerializeField] private float slideDuration = 0.5f;

    [Header("Configurações de Input")]
    [Tooltip("Ação de input que ativa o deslizamento")]
    [SerializeField] private InputActionReference slideAction;

    private bool playerInRange = false;
    private bool isListeningToInput = false;
    private GameObject currentPlayer = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Verificações de configuração
        ValidateConfiguration();
    }

    // Update is called once per frame
    void Update()
    {
        // Gerencia os listeners de input baseado na presença do jogador
        ManageInputAction();
    }

    // Verifica se todas as configurações necessárias estão presentes
    private void ValidateConfiguration()
    {
        // Verificar se o ponto de destino está configurado
        if (destinationPoint == null)
        {
            Debug.LogWarning($"SlidePointController ({gameObject.name}): Ponto de destino não configurado!");
        }

        // Verificar se a ação de input está configurada
        if (slideAction == null)
        {
            Debug.LogWarning($"SlidePointController ({gameObject.name}): Ação de input não configurada!");
        }
        else
        {
            // Garante que a ação esteja habilitada
            if (!slideAction.action.enabled)
            {
                Debug.Log($"SlidePointController ({gameObject.name}): Habilitando a ação de input {slideAction.action.name}");
                slideAction.action.Enable();
            }
        }

        // Verificação do collider
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider == null)
        {
            Debug.LogError($"SlidePointController ({gameObject.name}): CircleCollider2D não encontrado!");
        }
        else if (!circleCollider.isTrigger)
        {
            Debug.LogError($"SlidePointController ({gameObject.name}): CircleCollider2D deve ser configurado como trigger!");
        }

        Debug.Log($"SlidePointController ({gameObject.name}) inicializado com sucesso.");
    }

    // Gerencia os listeners de input
    private void ManageInputAction()
    {
        if (slideAction != null)
        {
            // Ativa a escuta do input quando o jogador estiver no alcance
            if (playerInRange && !isListeningToInput)
            {
                Debug.Log($"SlidePointController: Adicionando listener para a ação {slideAction.action.name}");
                slideAction.action.performed += OnSlideActionPerformed;
                isListeningToInput = true;
            }
            // Desativa a escuta do input quando o jogador sair do alcance
            else if (!playerInRange && isListeningToInput)
            {
                Debug.Log($"SlidePointController: Removendo listener da ação {slideAction.action.name}");
                slideAction.action.performed -= OnSlideActionPerformed;
                isListeningToInput = false;
                currentPlayer = null;
            }
        }
    }

    // Callback quando a ação de deslizar é executada
    private void OnSlideActionPerformed(InputAction.CallbackContext context)
    {
        Debug.Log($"SlidePointController ({gameObject.name}): Ação de deslizamento ativada!");

        if (currentPlayer != null && destinationPoint != null)
        {
            SlimeMovement slimeMovement = currentPlayer.GetComponent<SlimeMovement>();
            if (slimeMovement != null)
            {
                Debug.Log($"SlidePointController: Iniciando deslizamento para {destinationPoint.position}");
                slimeMovement.Slide(destinationPoint.position, slideDuration);
            }
            else
            {
                Debug.LogError("SlidePointController: O jogador não possui componente SlimeMovement!");
            }
        }
        else
        {
            Debug.LogError($"SlidePointController: Não é possível iniciar deslizamento. Jogador: {(currentPlayer != null)}, Destino: {(destinationPoint != null)}");
        }
    }

    // Triggers para detecção do jogador
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            currentPlayer = other.gameObject;
            Debug.Log($"SlidePointController ({gameObject.name}): Jogador entrou na área");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log($"SlidePointController ({gameObject.name}): Jogador saiu da área");
        }
    }

    // Desenha a linha de deslizamento na cena para visualização
    private void OnDrawGizmos()
    {
        if (destinationPoint != null)
        {
            // Cores para destacar o caminho de deslizamento
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destinationPoint.position);

            // Desenha uma esfera no ponto de destino
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(destinationPoint.position, 0.2f);
        }
    }

    // Garante que os listeners sejam removidos quando o objeto for desabilitado
    private void OnDisable()
    {
        if (slideAction != null && isListeningToInput)
        {
            slideAction.action.performed -= OnSlideActionPerformed;
            isListeningToInput = false;
        }
    }
}
