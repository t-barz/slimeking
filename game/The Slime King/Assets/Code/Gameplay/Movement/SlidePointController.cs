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

    [Header("Visual no Editor")]
    [Tooltip("Cor da linha no editor que conecta o ponto inicial ao destino")]
    [SerializeField] private Color pathColor = Color.cyan;

    [Tooltip("Cor da esfera no ponto de destino")]
    [SerializeField] private Color destinationColor = Color.blue;

    [Header("Configurações de Input")]
    [Tooltip("Ação de input que ativa o deslizamento")]
    [SerializeField] private InputActionReference interactAction;

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
        if (interactAction == null)
        {
            Debug.LogWarning($"SlidePointController ({gameObject.name}): Ação de input não configurada!");
        }
        else
        {
            // Garante que a ação esteja habilitada
            if (!interactAction.action.enabled)
            {
                Debug.Log($"SlidePointController ({gameObject.name}): Habilitando a ação de input {interactAction.action.name}");
                interactAction.action.Enable();
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

    }

    // Gerencia os listeners de input
    private void ManageInputAction()
    {
        if (interactAction != null)
        {
            // Ativa a escuta do input quando o jogador estiver no alcance
            if (playerInRange && !isListeningToInput)
            {
                interactAction.action.performed += OnInteractActionPerformed;
                isListeningToInput = true;
            }
            // Desativa a escuta do input quando o jogador sair do alcance
            else if (!playerInRange && isListeningToInput)
            {
                interactAction.action.performed -= OnInteractActionPerformed;
                isListeningToInput = false;
                currentPlayer = null;
            }
        }
    }

    // Callback quando a ação de deslizar é executada
    private void OnInteractActionPerformed(InputAction.CallbackContext context)
    {

        if (currentPlayer != null && destinationPoint != null)
        {
            SlimeMovement slimeMovement = currentPlayer.GetComponent<SlimeMovement>();
            if (slimeMovement != null)
            {
                slimeMovement.Slide(destinationPoint.position, slideDuration);
            }
        }
    }

    // Triggers para detecção do jogador
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            currentPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    // Desenha a linha de deslizamento na cena para visualização
    private void OnDrawGizmos()
    {
        if (destinationPoint != null)
        {
            // Desenha uma linha do ponto de origem até o destino com a cor configurada
            Gizmos.color = pathColor;
            Gizmos.DrawLine(transform.position, destinationPoint.position);

            // Desenha uma esfera no ponto de destino com a cor configurada
            Gizmos.color = destinationColor;
            Gizmos.DrawSphere(destinationPoint.position, 0.2f);
        }
    }

    // Desenha gizmos adicionais quando o objeto está selecionado
    private void OnDrawGizmosSelected()
    {
        if (destinationPoint != null)
        {
            // Desenha uma linha pontilhada quando selecionado para destacar melhor
            Gizmos.color = new Color(pathColor.r, pathColor.g, pathColor.b, 0.5f);
            Gizmos.DrawLine(transform.position, destinationPoint.position);

            // Desenha um círculo no ponto de origem para indicar a área de detecção
            CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
            if (circleCollider != null)
            {
                // Usa uma cor semi-transparente para o círculo de detecção
                Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
                Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
            }

            // Destaca o ponto de destino com uma esfera maior quando selecionado
            Gizmos.color = new Color(destinationColor.r, destinationColor.g, destinationColor.b, 0.7f);
            Gizmos.DrawSphere(destinationPoint.position, 0.25f);
        }
    }

    // Garante que os listeners sejam removidos quando o objeto for desabilitado
    private void OnDisable()
    {
        if (interactAction != null && isListeningToInput)
        {
            interactAction.action.performed -= OnInteractActionPerformed;
            isListeningToInput = false;
        }
    }
}
