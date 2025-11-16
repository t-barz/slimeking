using System.Collections;
using UnityEngine;
using SlimeKing.Gameplay;
using SlimeKing.Core;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Controlador para objetos que podem ser empurrados pelo jogador.
    /// 
    /// RESPONSABILIDADES:
    /// • Detecta colisão com o Player através de Trigger
    /// • Responde ao input de interação do Player
    /// • Move o objeto em múltiplas direções configuradas (Norte, Sul, Leste, Oeste)
    /// • Determina direção automaticamente baseada na posição do Player
    /// • Rotaciona o objeto durante o movimento
    /// • Controla velocidade e duração do movimento
    /// • Suporta movimento de objeto irmão sincronizado (sem rotação)
    /// 
    /// DEPENDÊNCIAS:
    /// • Collider2D configurado como Trigger para detecção do Player
    /// • Rigidbody2D para movimento físico suave
    /// 
    /// CONFIGURAÇÃO:
    /// • Anexar a um GameObject com Collider2D (isTrigger = true)
    /// • Selecionar múltiplas direções permitidas no Inspector
    /// • Configurar velocidade e duração no Inspector
    /// • Opcional: Configurar objeto irmão para movimento sincronizado
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class PushableObject : MonoBehaviour, IInteractable
    {
        #region Enums

        /// <summary>
        /// Direções válidas para movimento do objeto empurrável
        /// </summary>
        [System.Flags]
        public enum PushDirection
        {
            None = 0,
            North = 1 << 0,  // Cima (Y+)
            South = 1 << 1,  // Baixo (Y-)
            East = 1 << 2,   // Direita (X+)
            West = 1 << 3    // Esquerda (X-)
        }

        #endregion

        #region Inspector Configuration

        [Header("⚙️ Configurações de Movimento")]
        [Tooltip("Direções possíveis para movimento (selecione múltiplas)")]
        [SerializeField] private PushDirection allowedDirections = PushDirection.North;

        [Tooltip("Velocidade de movimento do objeto (unidades por segundo)")]
        [SerializeField] private float moveSpeed = 3f;

        [Tooltip("Duração do movimento em segundos")]
        [SerializeField] private float moveDuration = 2f;

        [Tooltip("Velocidade de rotação durante o movimento (graus por segundo)")]
        [SerializeField] private float rotationSpeed = 90f;

        [Header("🔢 Configurações de Uso")]
        [Tooltip("Número máximo de vezes que o objeto pode ser empurrado (-1 = ilimitado)")]
        [SerializeField] private int maxUses = -1;

        [Header("🎧 Configurações de Áudio")]
        [Tooltip("Som reproduzido quando o objeto começar a se mover")]
        [SerializeField] private AudioClip pushSound;

        [Tooltip("Som reproduzido durante o movimento (loop)")]
        [SerializeField] private AudioClip movingSound;

        [Header("🔧 Debug")]
        [Tooltip("Ativar logs de debug para este objeto")]
        [SerializeField] private bool enableDebugLogs = false;

        [Header("🔗 Configurações de Objeto Conectado")]
        [Tooltip("GameObject irmão que será movido junto (sem rotação) com este objeto (opcional)")]
        [SerializeField] private GameObject siblingObject;

        #endregion

        #region Private Variables

        private Rigidbody2D _rigidbody2D;
        private AudioSource _audioSource;
        private Rigidbody2D _siblingRigidbody2D; // Rigidbody2D do objeto irmão

        private bool _playerInRange = false;
        private bool _isMoving = false;
        private int _currentUses = 0;  // Contador de usos atuais

        private Vector2 _moveDirection;
        private PushDirection _currentDirection; // Direção atual sendo usada
        private Coroutine _movementCoroutine;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeComponents();
            SetupMovementDirection();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
                LogDebug($"Player entrou na área de interação do objeto {name}");

                // Se necessário, podemos adicionar feedback visual aqui
                // Por exemplo, mudar a cor do sprite para indicar que pode ser empurrado
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
                LogDebug($"Player saiu da área de interação do objeto {name}");

                // Remove feedback visual se houver
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Inicializa componentes obrigatórios
        /// </summary>
        private void InitializeComponents()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();

            // Configura o próprio Rigidbody2D
            ConfigureRigidbodyForMovement(_rigidbody2D);

            // Configura objeto irmão se especificado
            SetupSiblingRigidbody();

            // Tenta obter AudioSource, cria um se não existir
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Configura AudioSource
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f; // 2D sound
        }

        /// <summary>
        /// Configura o Rigidbody2D do objeto irmão se especificado
        /// </summary>
        private void SetupSiblingRigidbody()
        {
            if (siblingObject != null)
            {
                _siblingRigidbody2D = siblingObject.GetComponent<Rigidbody2D>();

                if (_siblingRigidbody2D != null)
                {
                    LogDebug($"Objeto irmão configurado: {siblingObject.name}");
                    ConfigureSiblingRigidbodyForMovement(_siblingRigidbody2D);
                }
                else
                {
                    LogDebug($"AVISO: Objeto irmão '{siblingObject.name}' não possui Rigidbody2D");
                }
            }
            else
            {
                LogDebug("Nenhum objeto irmão configurado - apenas este objeto será movido");
            }
        }

        /// <summary>
        /// Configura um Rigidbody2D para movimento controlado
        /// </summary>
        /// <param name="rigidbody">Rigidbody2D a ser configurado</param>
        private void ConfigureRigidbodyForMovement(Rigidbody2D rigidbody)
        {
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            rigidbody.gravityScale = 0f;
            rigidbody.mass = 100000f; // Massa alta para objetos pesados
            rigidbody.linearDamping = 5f;
            rigidbody.angularDamping = 5f;
            rigidbody.freezeRotation = false;
            rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        /// <summary>
        /// Configura um Rigidbody2D do objeto irmão (SEM rotação)
        /// </summary>
        /// <param name="rigidbody">Rigidbody2D do objeto irmão a ser configurado</param>
        private void ConfigureSiblingRigidbodyForMovement(Rigidbody2D rigidbody)
        {
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            rigidbody.gravityScale = 0f;
            rigidbody.mass = 100000f; // Massa alta para objetos pesados
            rigidbody.linearDamping = 5f;
            rigidbody.angularDamping = 5f;
            rigidbody.freezeRotation = true; // ROTAÇÃO CONGELADA para objeto irmão
            rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        /// <summary>
        /// Converte a direção enum para Vector2
        /// </summary>
        private void SetupMovementDirection()
        {
            // Este método agora será chamado dinamicamente quando necessário
            LogDebug($"Direções permitidas configuradas: {allowedDirections}");
        }

        /// <summary>
        /// Determina a direção de movimento baseada na posição do player
        /// </summary>
        /// <param name="playerTransform">Transform do player</param>
        /// <returns>Direção válida para movimento ou None se não houver</returns>
        private PushDirection DetermineMovementDirection(Transform playerTransform)
        {
            if (playerTransform == null) return PushDirection.None;

            Vector2 playerPosition = playerTransform.position;

            // Usa a posição do objeto que será movido para calcular direção
            Vector2 targetObjectPosition;
            if (HasSiblingObject)
            {
                targetObjectPosition = siblingObject.transform.position;
                LogDebug($"Usando posição do objeto irmão: {targetObjectPosition}");
            }
            else
            {
                targetObjectPosition = transform.position;
                LogDebug($"Usando posição própria: {targetObjectPosition}");
            }

            Vector2 pushDirection = (targetObjectPosition - playerPosition).normalized; // Direção do empurrão

            LogDebug($"Player pos: {playerPosition}, Target Object pos: {targetObjectPosition}, Push direction: {pushDirection}");
            LogDebug($"Allowed directions: {allowedDirections}");

            // Encontra a direção mais próxima baseada na direção do empurrão
            PushDirection bestDirection = PushDirection.None;
            float bestDot = -1f;

            // Verifica cada direção permitida
            if ((allowedDirections & PushDirection.North) != 0)
            {
                float dot = Vector2.Dot(pushDirection, Vector2.up); // Empurrão para norte
                LogDebug($"Norte - dot: {dot}, pushDirection: {pushDirection}, Vector2.up: {Vector2.up}");
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestDirection = PushDirection.North;
                    LogDebug($"Norte é a melhor direção até agora com dot: {dot}");
                }
            }

            if ((allowedDirections & PushDirection.South) != 0)
            {
                Vector2 southVector = new Vector2(0, -1); // Explícito para debug
                float dot = Vector2.Dot(pushDirection, southVector); // Empurrão para sul
                LogDebug($"Sul - dot: {dot}, pushDirection: {pushDirection}, southVector: {southVector}");
                LogDebug($"Comparando: bestDot atual = {bestDot}, novo dot = {dot}");
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestDirection = PushDirection.South;
                    LogDebug($"Sul é a melhor direção até agora com dot: {dot}");
                }
            }

            if ((allowedDirections & PushDirection.East) != 0)
            {
                float dot = Vector2.Dot(pushDirection, Vector2.right); // Empurrão para leste
                LogDebug($"Leste - dot: {dot}, pushDirection: {pushDirection}, Vector2.right: {Vector2.right}");
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestDirection = PushDirection.East;
                    LogDebug($"Leste é a melhor direção até agora com dot: {dot}");
                }
            }

            if ((allowedDirections & PushDirection.West) != 0)
            {
                Vector2 westVector = new Vector2(-1, 0); // Explícito para debug
                float dot = Vector2.Dot(pushDirection, westVector); // Empurrão para oeste
                LogDebug($"Oeste - dot: {dot}, pushDirection: {pushDirection}, westVector: {westVector}");
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestDirection = PushDirection.West;
                    LogDebug($"Oeste é a melhor direção até agora com dot: {dot}");
                }
            }

            LogDebug($"Melhor direção encontrada: {bestDirection} com dot: {bestDot}, threshold: 0.3");

            // Só aceita se o player está numa posição razoável (dot > 0.3 para evitar movimentos diagonais)
            if (bestDot > 0.3f)
            {
                LogDebug($"Direção aceita: {bestDirection}");
                return bestDirection;
            }

            LogDebug($"Nenhuma direção válida - dot {bestDot} não passou do threshold 0.3");
            return PushDirection.None;
        }

        /// <summary>
        /// Converte direção para Vector2 de movimento
        /// </summary>
        /// <param name="direction">Direção a converter</param>
        /// <returns>Vector2 normalizado</returns>
        private Vector2 DirectionToVector(PushDirection direction)
        {
            Vector2 result;
            switch (direction)
            {
                case PushDirection.North:
                    result = Vector2.up;
                    break;
                case PushDirection.South:
                    result = new Vector2(0, -1); // Explícito
                    break;
                case PushDirection.East:
                    result = Vector2.right;
                    break;
                case PushDirection.West:
                    result = new Vector2(-1, 0); // Explícito
                    break;
                default:
                    result = Vector2.zero;
                    break;
            }

            LogDebug($"DirectionToVector: {direction} -> {result}");
            return result;
        }

        /// <summary>
        /// Verifica se ainda há usos disponíveis
        /// </summary>
        /// <returns>True se pode ser usado, false se atingiu o limite</returns>
        private bool HasUsesRemaining()
        {
            if (maxUses < 0) return true; // Ilimitado
            return _currentUses < maxUses;
        }

        /// <summary>
        /// Incrementa o contador de usos
        /// </summary>
        private void IncrementUses()
        {
            _currentUses++;
            LogDebug($"Uso incrementado: {_currentUses}/{(maxUses < 0 ? "∞" : maxUses.ToString())}");
        }

        /// <summary>
        /// Retorna quantos usos restam
        /// </summary>
        /// <returns>Número de usos restantes (-1 se ilimitado)</returns>
        private int GetRemainingUses()
        {
            if (maxUses < 0) return -1; // Ilimitado
            return Mathf.Max(0, maxUses - _currentUses);
        }

        /// <summary>
        /// Calcula a direção da rotação baseada na direção do movimento
        /// </summary>
        /// <param name="direction">Direção do movimento</param>
        /// <returns>1f para sentido horário, -1f para sentido anti-horário</returns>
        private float GetRotationDirection(PushDirection direction)
        {
            switch (direction)
            {
                case PushDirection.East:   // Leste -> horário
                case PushDirection.South:  // Sul -> horário
                    return -1f;

                case PushDirection.North:  // Norte -> anti-horário
                case PushDirection.West:   // Oeste -> anti-horário
                    return 1f;

                default:
                    return 1f; // Padrão horário
            }
        }

        #endregion

        #region IInteractable Implementation

        /// <summary>
        /// Implementa IInteractable.TryInteract()
        /// Tenta empurrar o objeto se as condições permitirem
        /// </summary>
        /// <param name="player">Transform do Player que está tentando interagir</param>
        /// <returns>True se a interação foi bem-sucedida</returns>
        public bool TryInteract(Transform player)
        {
            if (!CanInteract(player))
            {
                if (!HasUsesRemaining())
                {
                    LogDebug("Interação negada - limite de usos atingido");
                }
                else
                {
                    LogDebug("Interação negada - condições não atendidas");
                }
                return false;
            }

            // Determina a direção baseada na posição do player
            PushDirection chosenDirection = DetermineMovementDirection(player);
            if (chosenDirection == PushDirection.None)
            {
                LogDebug("Interação negada - player não está em posição válida para empurrar");
                return false;
            }

            _currentDirection = chosenDirection;
            _moveDirection = DirectionToVector(_currentDirection);

            IncrementUses();
            StartPushMovement();
            LogDebug($"Player {player.name} empurrou o objeto {name} na direção {_currentDirection}");
            return true;
        }

        /// <summary>
        /// Implementa IInteractable.CanInteract()
        /// Verifica se pode empurrar o objeto
        /// </summary>
        /// <param name="player">Transform do Player</param>
        /// <returns>True se pode interagir</returns>
        public bool CanInteract(Transform player)
        {
            return _playerInRange && !_isMoving && HasUsesRemaining();
        }

        /// <summary>
        /// Implementa IInteractable.GetInteractionPrompt()
        /// Retorna texto de prompt para o UI
        /// </summary>
        /// <returns>Texto de prompt</returns>
        public string GetInteractionPrompt()
        {
            if (_isMoving)
                return "";

            if (!HasUsesRemaining())
                return $"Objeto esgotado ({_currentUses}/{maxUses})";

            // Mostra direções permitidas
            string directionsText = GetAllowedDirectionsText();

            if (maxUses < 0)
            {
                return $"Pressione [E] para empurrar ({directionsText})";
            }
            else
            {
                int remaining = GetRemainingUses();
                return $"Pressione [E] para empurrar ({directionsText}) [{remaining} restantes]";
            }
        }

        /// <summary>
        /// Retorna texto com as direções permitidas
        /// </summary>
        /// <returns>String formatada com direções</returns>
        private string GetAllowedDirectionsText()
        {
            var directions = new System.Collections.Generic.List<string>();

            if ((allowedDirections & PushDirection.North) != 0) directions.Add("Norte");
            if ((allowedDirections & PushDirection.South) != 0) directions.Add("Sul");
            if ((allowedDirections & PushDirection.East) != 0) directions.Add("Leste");
            if ((allowedDirections & PushDirection.West) != 0) directions.Add("Oeste");

            return string.Join("/", directions);
        }

        /// <summary>
        /// Implementa IInteractable.GetInteractionPriority()
        /// Prioridade para sistemas de múltiplas interações
        /// </summary>
        /// <returns>Prioridade da interação</returns>
        public int GetInteractionPriority()
        {
            // Prioridade padrão para objetos empurráveis
            return 100;
        }

        #endregion

        #region Public Methods (Legacy)

        /// <summary>
        /// Método público para ser chamado quando o Player pressionar o botão de interação
        /// Este método deve ser chamado pelo sistema de interação do Player
        /// </summary>
        public void OnPlayerInteract()
        {
            if (!_playerInRange)
            {
                LogDebug("Player não está na área de interação");
                return;
            }

            if (_isMoving)
            {
                LogDebug("Objeto já está se movendo, ignorando interação");
                return;
            }

            if (!HasUsesRemaining())
            {
                LogDebug("Limite de usos atingido, ignorando interação");
                return;
            }

            // Precisa do Transform do player para determinar direção
            // Para compatibilidade, usa uma direção padrão se não conseguir determinar
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                PushDirection chosenDirection = DetermineMovementDirection(player.transform);
                if (chosenDirection != PushDirection.None)
                {
                    _currentDirection = chosenDirection;
                    _moveDirection = DirectionToVector(_currentDirection);

                    IncrementUses();
                    StartPushMovement();
                    LogDebug($"Player empurrou objeto na direção {_currentDirection}");
                }
                else
                {
                    LogDebug("Player não está em posição válida para empurrar");
                }
            }
            else
            {
                LogDebug("Player não encontrado para determinar direção");
            }
        }

        /// <summary>
        /// Verifica se o Player está na área de interação
        /// </summary>
        public bool IsPlayerInRange => _playerInRange;

        /// <summary>
        /// Verifica se o objeto está atualmente se movendo
        /// </summary>
        public bool IsMoving => _isMoving;

        /// <summary>
        /// Retorna o número atual de usos
        /// </summary>
        public int CurrentUses => _currentUses;

        /// <summary>
        /// Retorna o número máximo de usos (-1 se ilimitado)
        /// </summary>
        public int MaxUses => maxUses;

        /// <summary>
        /// Retorna quantos usos restam (-1 se ilimitado)
        /// </summary>
        public int RemainingUses => GetRemainingUses();

        /// <summary>
        /// Verifica se o objeto ainda pode ser usado
        /// </summary>
        public bool CanBeUsed => HasUsesRemaining();

        /// <summary>
        /// Retorna lista dos GameObjects que serão movidos
        /// </summary>
        public GameObject[] MovedObjects
        {
            get
            {
                var objects = new System.Collections.Generic.List<GameObject> { gameObject };
                if (HasSiblingObject)
                {
                    objects.Add(siblingObject);
                }
                return objects.ToArray();
            }
        }

        /// <summary>
        /// Verifica se há um objeto irmão configurado e válido
        /// </summary>
        public bool HasSiblingObject => siblingObject != null && _siblingRigidbody2D != null;

        /// <summary>
        /// Retorna o GameObject irmão configurado
        /// </summary>
        public GameObject SiblingObject => siblingObject;

        #endregion

        #region Movement Logic

        /// <summary>
        /// Inicia o movimento de empurrar o objeto
        /// </summary>
        private void StartPushMovement()
        {
            if (_movementCoroutine != null)
            {
                StopCoroutine(_movementCoroutine);
            }

            _movementCoroutine = StartCoroutine(PushMovementCoroutine());
        }

        /// <summary>
        /// Corrotina que executa o movimento e rotação dos objetos
        /// </summary>
        private IEnumerator PushMovementCoroutine()
        {
            _isMoving = true;

            // Reproduz som de início do movimento
            PlayPushSound();

            string objectsInfo = HasSiblingObject
                ? $"'{name}' (com rotação) e objeto irmão '{siblingObject.name}' (sem rotação)"
                : $"'{name}'";

            LogDebug($"Iniciando movimento de {objectsInfo} na direção {_currentDirection} por {moveDuration} segundos");
            LogDebug($"Vetor de movimento: {_moveDirection}");

            float elapsedTime = 0f;
            Vector2 targetVelocity = _moveDirection * moveSpeed;
            LogDebug($"Velocidade alvo: {targetVelocity} (moveSpeed: {moveSpeed})");
            float rotationDirection = GetRotationDirection(_currentDirection);

            // Som contínuo de movimento
            if (movingSound != null)
            {
                _audioSource.clip = movingSound;
                _audioSource.loop = true;
                _audioSource.Play();
            }

            LogDebug($"Rotação configurada: {(rotationDirection > 0 ? "Horário" : "Anti-horário")}");

            while (elapsedTime < moveDuration)
            {
                // Aplica movimento ao próprio objeto
                _rigidbody2D.linearVelocity = targetVelocity;

                // Aplica movimento ao objeto irmão se configurado (SEM rotação)
                if (HasSiblingObject)
                {
                    _siblingRigidbody2D.linearVelocity = targetVelocity;
                }

                // Aplica rotação APENAS ao próprio PushableObject
                float rotationThisFrame = rotationSpeed * rotationDirection * Time.deltaTime;
                transform.Rotate(0f, 0f, rotationThisFrame);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Para o movimento de ambos os objetos
            _rigidbody2D.linearVelocity = Vector2.zero;

            if (HasSiblingObject)
            {
                _siblingRigidbody2D.linearVelocity = Vector2.zero;
            }

            // Para o som
            if (_audioSource.isPlaying && _audioSource.loop)
            {
                _audioSource.Stop();
            }

            _isMoving = false;
            _movementCoroutine = null;

            LogDebug("Movimento concluído");
        }

        #endregion

        #region Audio

        /// <summary>
        /// Reproduz som de início do movimento
        /// </summary>
        private void PlayPushSound()
        {
            if (pushSound != null)
            {
                _audioSource.PlayOneShot(pushSound);
            }
        }

        #endregion

        #region Debug

        /// <summary>
        /// Log controlado por flag de debug
        /// </summary>
        private void LogDebug(string message)
        {
            if (enableDebugLogs)
            {
                UnityEngine.Debug.Log($"[PushableObject-{name}] {message}");
            }
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            // Determina qual objeto usar como referência para as setas
            Vector3 referencePosition = HasSiblingObject ? siblingObject.transform.position : transform.position;

            // Desenha setas para todas as direções permitidas na posição do objeto que será movido
            if ((allowedDirections & PushDirection.North) != 0)
                DrawDirectionArrow(referencePosition, Vector3.up, Color.green);

            if ((allowedDirections & PushDirection.South) != 0)
                DrawDirectionArrow(referencePosition, Vector3.down, Color.green);

            if ((allowedDirections & PushDirection.East) != 0)
                DrawDirectionArrow(referencePosition, Vector3.right, Color.green);

            if ((allowedDirections & PushDirection.West) != 0)
                DrawDirectionArrow(referencePosition, Vector3.left, Color.green);

            // Conecta visualmente com objeto irmão se configurado
            if (HasSiblingObject)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, siblingObject.transform.position);

                // Desenha um pequeno círculo no PushableObject (detector)
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, 0.3f);

                // Desenha um círculo maior no objeto que será movido
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(siblingObject.transform.position, 0.5f);
            }
            else
            {
                // Se não há irmão, desenha círculo no próprio objeto
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, 0.4f);
            }

            // Desenha texto com informações
            Vector3 textPos = referencePosition + Vector3.up * 1f;

#if UNITY_EDITOR
            string usesText = maxUses < 0 ? "∞" : $"{_currentUses}/{maxUses}";
            string siblingInfo = HasSiblingObject ? $"\nIrmão: {siblingObject.name} (sem rotação)" : "\nSem irmão";
            string directionsText = GetAllowedDirectionsText();
            string positionInfo = HasSiblingObject ? "\n(Setas baseadas no objeto irmão)" : "\n(Setas baseadas neste objeto)";
            UnityEditor.Handles.Label(textPos, $"Direções: {directionsText}\nSpeed: {moveSpeed}\nDuration: {moveDuration}s\nUsos: {usesText}{siblingInfo}{positionInfo}");
#endif
        }

        /// <summary>
        /// Desenha uma seta de direção nos Gizmos
        /// </summary>
        /// <param name="startPosition">Posição inicial da seta</param>
        /// <param name="direction">Direção da seta</param>
        /// <param name="color">Cor da seta</param>
        private void DrawDirectionArrow(Vector3 startPosition, Vector3 direction, Color color)
        {
            Gizmos.color = color;
            Vector3 endPos = startPosition + direction * 2f;

            // Desenha linha da direção
            Gizmos.DrawLine(startPosition, endPos);

            // Desenha seta na ponta
            Vector3 arrowHead1 = endPos + (Quaternion.Euler(0, 0, 45) * -direction * 0.5f);
            Vector3 arrowHead2 = endPos + (Quaternion.Euler(0, 0, -45) * -direction * 0.5f);

            Gizmos.DrawLine(endPos, arrowHead1);
            Gizmos.DrawLine(endPos, arrowHead2);
        }

        #endregion
    }
}
