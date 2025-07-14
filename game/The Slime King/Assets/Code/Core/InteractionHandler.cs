using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Monitora eventos de interação e registra logs no console.
/// Pode ser usado como componente independente ou como base para sistemas de interação.
/// A interação só ocorrerá quando o jogador estiver em colisão com este objeto.
/// </summary>
public class InteractionHandler : MonoBehaviour
{
    #region Configurações e Campos

    [Header("Configurações")]
    [Tooltip("Mensagem personalizada para exibir no log quando a interação ocorrer")]
    [SerializeField] private string logMessage = "Ação de interação detectada!";

    [Tooltip("Exibir informações adicionais no log")]
    [SerializeField] private bool detailedLog = false;

    [Tooltip("Exibir mensagem apenas na primeira interação")]
    [SerializeField] private bool logOnlyOnce = false;

    [Tooltip("Mostrar mensagem quando o jogador entrar/sair da área de interação")]
    [SerializeField] private bool logRangeEvents = false;

    [Tooltip("Forçar verificação de colisão todo frame (use se houver problemas de detecção)")]
    [SerializeField] private bool forceCollisionCheck = true;

    [Tooltip("Raio de verificação para detecção manual de colisão")]
    [SerializeField] private float detectionRadius = 1.5f;

    /// <summary>
    /// Controla se a primeira interação já foi registrada
    /// </summary>
    private bool firstInteractionLogged = false;

    /// <summary>
    /// Indica se o player está dentro da área de colisão deste objeto
    /// </summary>
    private bool playerInRange = false;

    /// <summary>
    /// Referência ao GameObject do player para verificação manual
    /// </summary>
    private GameObject playerObject = null;

    #endregion

    #region Ciclo de Vida do Unity

    /// <summary>
    /// Chamado quando o objeto é ativado. Registra callbacks e verifica componentes.
    /// </summary>
    private void OnEnable()
    {
        // Tenta encontrar o player uma vez para cache
        playerObject = GameObject.FindGameObjectWithTag("Player");

        // Registra o callback para a ação de interação
        if (InputManager.Instance != null)
        {
            Debug.Log($"[{gameObject.name}] Registrando callbacks no InputManager");
            // Remove primeiro para evitar duplicação (por segurança)
            InputManager.Instance.InteractAction.performed -= OnInteractPerformed;
            // Adiciona o callback
            InputManager.Instance.InteractAction.performed += OnInteractPerformed;
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] InputManager não encontrado!");

            // Agenda uma tentativa posterior para garantir o registro
            Invoke("TryRegisterInput", 0.5f);
        }

        // Verifica se tem um collider com trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogWarning("Este objeto não tem um Collider2D. Adicione um para detectar quando o player está na área.");
        }
        else if (!collider.isTrigger)
        {
            Debug.LogWarning("O Collider2D deste objeto não está configurado como trigger. A detecção de área pode não funcionar corretamente.");
        }
    }

    /// <summary>
    /// Tenta registrar o input caso o InputManager não estivesse disponível inicialmente
    /// </summary>
    private void TryRegisterInput()
    {
        if (InputManager.Instance != null)
        {
            Debug.Log($"[{gameObject.name}] Tentativa posterior: Registrando callbacks no InputManager");
            // Remove primeiro para evitar duplicação (por segurança)
            InputManager.Instance.InteractAction.performed -= OnInteractPerformed;
            // Adiciona o callback
            InputManager.Instance.InteractAction.performed += OnInteractPerformed;
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] InputManager ainda não está disponível");
        }
    }

    /// <summary>
    /// Chamado quando o objeto é desativado. Remove callbacks registrados.
    /// </summary>
    protected virtual void OnDisable()
    {
        // Remove o callback quando o objeto for desativado
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractAction.performed -= OnInteractPerformed;
        }

        // Cancela qualquer invocação pendente
        CancelInvoke("TryRegisterInput");
    }

    /// <summary>
    /// Chamado uma vez por quadro, se o MonoBehaviour estiver ativado.
    /// </summary>
    private void Update()
    {
        // Visualização de debug
        if (playerInRange)
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.up * 1f, Color.green);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.up * 0.5f, Color.red);
        }

        // Verificação manual de colisão (mais confiável que OnTriggerStay2D em alguns casos)
        if (forceCollisionCheck)
        {
            CheckPlayerManually();
        }

        // Tenta garantir que o registro no InputManager está funcionando
        EnsureInputRegistration();
    }

    /// <summary>
    /// Verifica manualmente se o player está na área de interação
    /// </summary>
    private void CheckPlayerManually()
    {
        // Se não temos referência ao player, tenta encontrá-lo
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null) return;
        }

        // Verifica a distância entre o objeto e o player
        float distance = Vector2.Distance(transform.position, playerObject.transform.position);
        bool playerNearby = distance <= detectionRadius;

        // Atualiza o estado se necessário
        if (playerNearby != playerInRange)
        {
            playerInRange = playerNearby;

            if (logRangeEvents)
            {
                if (playerInRange)
                {
                    Debug.Log($"[Manual] Player entrou na área de interação de {gameObject.name}");
                }
                else
                {
                    Debug.Log($"[Manual] Player saiu da área de interação de {gameObject.name}");
                }
            }
        }
    }

    /// <summary>
    /// Garante que o registro no InputManager está funcionando
    /// </summary>
    private void EnsureInputRegistration()
    {
        // A cada 3 segundos (aproximadamente)
        if (Time.frameCount % 180 == 0)
        {
            if (InputManager.Instance != null)
            {
                // Primeiro remove para evitar duplicações
                InputManager.Instance.InteractAction.performed -= OnInteractPerformed;

                // Depois registra novamente
                InputManager.Instance.InteractAction.performed += OnInteractPerformed;
            }
        }
    }

    #endregion

    #region Gerenciamento de Input

    /// <summary>
    /// Chamado quando o jogador realiza uma ação de interação.
    /// Só processa a interação se o player estiver na área deste objeto.
    /// </summary>
    /// <param name="ctx">Contexto do evento de input</param>
    protected virtual void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        // Só processa a interação se o player estiver na área
        if (!playerInRange)
        {
            if (detailedLog)
            {
                Debug.Log($"Tentativa de interação ignorada - player não está na área de {gameObject.name}");
            }
            return;
        }

        // Verifica se deve registrar apenas uma vez
        if (logOnlyOnce && firstInteractionLogged)
        {
            return;
        }

        // Registra o log básico
        if (detailedLog)
        {
            Debug.Log($"{logMessage} | Objeto: {gameObject.name} | Valor: {ctx.ReadValueAsObject()} | Fase: {ctx.phase} | Timestamp: {Time.time:F2}s");
        }
        else
        {
            Debug.Log($"{logMessage} | Objeto: {gameObject.name}");
        }

        // Marca que a primeira interação foi registrada
        firstInteractionLogged = true;

        // Chama o método virtual que pode ser sobrescrito por classes derivadas
        OnInteractionDetected();
    }

    #endregion

    #region Detecção de Colisão

    /// <summary>
    /// Chamado quando um outro collider entra no trigger deste objeto.
    /// Detecta quando o player entra na área de interação.
    /// </summary>
    /// <param name="other">O collider que entrou no trigger</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (logRangeEvents)
            {
                Debug.Log($"Player entrou na área de interação de {gameObject.name}");
            }
        }
    }

    /// <summary>
    /// Chamado continuamente enquanto outro collider permanece no trigger deste objeto.
    /// Garante que o estado playerInRange permaneça consistente.
    /// </summary>
    /// <param name="other">O collider que está dentro do trigger</param>
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerInRange)
        {
            // Corrige o estado caso tenha ocorrido alguma inconsistência
            playerInRange = true;

            if (logRangeEvents)
            {
                Debug.Log($"Estado de detecção corrigido: player está na área de interação de {gameObject.name}");
            }
        }
    }

    /// <summary>
    /// Chamado quando um outro collider sai do trigger deste objeto.
    /// Detecta quando o player sai da área de interação.
    /// </summary>
    /// <param name="other">O collider que saiu do trigger</param>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (logRangeEvents)
            {
                Debug.Log($"Player saiu da área de interação de {gameObject.name}");
            }
        }
    }

    #endregion

    #region Métodos de Interação

    /// <summary>
    /// Método virtual que pode ser sobrescrito por classes derivadas para
    /// adicionar comportamentos personalizados quando uma interação for detectada.
    /// </summary>
    protected virtual void OnInteractionDetected()
    {
        // Implementação vazia por padrão
        // Classes derivadas podem implementar comportamentos específicos aqui
    }

    #endregion

    #region API Pública

    /// <summary>
    /// Reseta o controle de primeira interação, permitindo que
    /// logs sejam exibidos novamente se a opção logOnlyOnce estiver ativa.
    /// </summary>
    public void ResetInteractionControl()
    {
        firstInteractionLogged = false;
    }

    /// <summary>
    /// Verifica se o player está na área de interação deste objeto.
    /// </summary>
    /// <returns>True se o player estiver na área, False caso contrário</returns>
    public bool IsPlayerInRange()
    {
        return playerInRange;
    }

    /// <summary>
    /// Define manualmente se o player está na área de interação
    /// </summary>
    /// <param name="inRange">Estado a ser definido</param>
    public void SetPlayerInRange(bool inRange)
    {
        playerInRange = inRange;
    }

    /// <summary>
    /// Força a verificação de colisão manual
    /// </summary>
    public void ForceCollisionCheck()
    {
        CheckPlayerManually();
    }

    /// <summary>
    /// Testa se o InputManager está respondendo corretamente
    /// </summary>
    [ContextMenu("Testar Conexão com InputManager")]
    public void TestInputConnection()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogError($"[{gameObject.name}] InputManager não encontrado!");
            return;
        }

        Debug.Log($"[{gameObject.name}] Testando conexão com InputManager...");

        // Remover e registrar novamente para garantir
        InputManager.Instance.InteractAction.performed -= OnInteractPerformed;
        InputManager.Instance.InteractAction.performed += OnInteractPerformed;

        // Simulando um log de confirmação
        Debug.Log($"[{gameObject.name}] Conexão reestabelecida. Pressione o botão de interação para testar.");
    }

    #endregion
}