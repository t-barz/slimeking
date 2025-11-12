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
    /// • Move o objeto em uma direção configurada (Norte, Sul, Leste, Oeste)
    /// • Rotaciona o objeto durante o movimento
    /// • Controla velocidade e duração do movimento
    /// 
    /// DEPENDÊNCIAS:
    /// • Collider2D configurado como Trigger para detecção do Player
    /// • Rigidbody2D para movimento físico suave
    /// 
    /// CONFIGURAÇÃO:
    /// • Anexar a um GameObject com Collider2D (isTrigger = true)
    /// • Configurar direção de movimento, velocidade e duração no Inspector
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class PushableObject : MonoBehaviour, IInteractable
    {
        #region Enums

        /// <summary>
        /// Direções válidas para movimento do objeto empurrável
        /// </summary>
        public enum PushDirection
        {
            North,  // Cima (Y+)
            South,  // Baixo (Y-)
            East,   // Direita (X+)
            West    // Esquerda (X-)
        }

        #endregion

        #region Inspector Configuration

        [Header("⚙️ Configurações de Movimento")]
        [Tooltip("Direção do movimento quando o objeto for empurrado")]
        [SerializeField] private PushDirection pushDirection = PushDirection.North;

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
        }        /// <summary>
                 /// Converte a direção enum para Vector2
                 /// </summary>
        private void SetupMovementDirection()
        {
            switch (pushDirection)
            {
                case PushDirection.North:
                    _moveDirection = Vector2.up;
                    break;
                case PushDirection.South:
                    _moveDirection = Vector2.down;
                    break;
                case PushDirection.East:
                    _moveDirection = Vector2.right;
                    break;
                case PushDirection.West:
                    _moveDirection = Vector2.left;
                    break;
            }

            LogDebug($"Direção de movimento configurada: {pushDirection} -> {_moveDirection}");
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
        /// <returns>1f para sentido horário, -1f para sentido anti-horário</returns>
        private float GetRotationDirection()
        {
            switch (pushDirection)
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

            IncrementUses();
            StartPushMovement();
            LogDebug($"Player {player.name} empurrou o objeto {name}");
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

            if (maxUses < 0)
            {
                return $"Pressione [E] para empurrar ({pushDirection})";
            }
            else
            {
                int remaining = GetRemainingUses();
                return $"Pressione [E] para empurrar ({pushDirection}) [{remaining} restantes]";
            }
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

            IncrementUses();
            StartPushMovement();
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

            LogDebug($"Iniciando movimento de {objectsInfo} na direção {pushDirection} por {moveDuration} segundos"); float elapsedTime = 0f;
            Vector2 targetVelocity = _moveDirection * moveSpeed;
            float rotationDirection = GetRotationDirection();

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
                Debug.Log($"[PushableObject-{name}] {message}");
            }
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            // Desenha seta indicando direção do movimento
            Vector3 direction = Vector3.zero;

            switch (pushDirection)
            {
                case PushDirection.North:
                    direction = Vector3.up;
                    break;
                case PushDirection.South:
                    direction = Vector3.down;
                    break;
                case PushDirection.East:
                    direction = Vector3.right;
                    break;
                case PushDirection.West:
                    direction = Vector3.left;
                    break;
            }

            Gizmos.color = Color.green;
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + direction * 2f;

            // Desenha linha da direção
            Gizmos.DrawLine(startPos, endPos);

            // Desenha seta na ponta
            Vector3 arrowHead1 = endPos + (Quaternion.Euler(0, 0, 45) * -direction * 0.5f);
            Vector3 arrowHead2 = endPos + (Quaternion.Euler(0, 0, -45) * -direction * 0.5f);

            Gizmos.DrawLine(endPos, arrowHead1);
            Gizmos.DrawLine(endPos, arrowHead2);

            // Conecta visualmente com objeto irmão se configurado
            if (HasSiblingObject)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, siblingObject.transform.position);

                // Desenha seta no objeto irmão (apenas movimento, sem rotação)
                Vector3 siblingStart = siblingObject.transform.position;
                Vector3 siblingEnd = siblingStart + direction * 1.5f; // Seta menor para indicar sem rotação
                Gizmos.color = Color.blue; // Cor diferente para indicar que não rotaciona
                Gizmos.DrawLine(siblingStart, siblingEnd);

                Vector3 siblingArrow1 = siblingEnd + (Quaternion.Euler(0, 0, 45) * -direction * 0.3f);
                Vector3 siblingArrow2 = siblingEnd + (Quaternion.Euler(0, 0, -45) * -direction * 0.3f);
                Gizmos.DrawLine(siblingEnd, siblingArrow1);
                Gizmos.DrawLine(siblingEnd, siblingArrow2);
            }            // Desenha texto com informações
            Vector3 textPos = startPos + Vector3.up * 0.5f;

#if UNITY_EDITOR
            string usesText = maxUses < 0 ? "∞" : $"{_currentUses}/{maxUses}";
            string siblingInfo = HasSiblingObject ? $"\nIrmão: {siblingObject.name} (sem rotação)" : "\nSem irmão";
            UnityEditor.Handles.Label(textPos, $"{pushDirection}\nSpeed: {moveSpeed}\nDuration: {moveDuration}s\nUsos: {usesText}{siblingInfo}");
#endif
        }

        #endregion
    }
}
