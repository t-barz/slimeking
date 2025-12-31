using SlimeKing.Gameplay;
using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Ponto de movimento especial do Slime (Jump ou Shrink).
    /// Herda de InteractivePointHandler para integração com sistema de interação existente.
    /// 
    /// FUNCIONALIDADES:
    /// • Define tipo de movimento especial (Jump/Shrink)
    /// • Especifica ponto de destino do movimento
    /// • Interface pública para consulta de propriedades
    /// • Integração com sistema de interação via outline
    /// 
    /// USO:
    /// • Adicionar ao GameObject que representa o ponto de movimento especial
    /// • Configurar tipo de movimento no Inspector
    /// • Definir ponto de destino (Transform)
    /// • Outras classes podem consultar via GetMovementType() e GetDestinationPoint()
    /// </summary>
    public class SpecialMovementPoint : InteractivePointHandler
    {
        #region Enums
        /// <summary>
        /// Tipos de movimento especial disponíveis.
        /// </summary>
        public enum MovementType
        {
            Jump,   // Movimento de pulo
            Shrink  // Movimento de encolhimento
        }
        #endregion

        #region Serialized Fields
        [Header("🎯 Special Movement Settings")]
        [Tooltip("Tipo de movimento especial que este ponto representa")]
        [SerializeField] private MovementType movementType = MovementType.Jump;

        [Tooltip("Ponto de destino para onde o player será movido")]
        [SerializeField] private Transform destinationPoint;

        [Tooltip("Tempo em segundos para completar o movimento até o destino")]
        [SerializeField] private float movementDuration = 2f;

        [Header("🔍 Detection Settings")]
        [Tooltip("Layers que representam o Player para detecção de contato")]
        [SerializeField] private LayerMask playerLayers = 1; // Layer 0 (Default) por padrão
        #endregion

        #region Private Variables
        // Controle de contato com o Player
        private bool _playerInContact = false;
        private Collider2D _playerCollider = null;
        private Collider2D _triggerCollider = null;
        #endregion

        #region Unity Lifecycle
        protected virtual void Start()
        {
            InitializeComponents();
            ValidateConfiguration();
            InitializeTriggerCollider();
        }
        protected virtual void OnDrawGizmos()
        {
            DrawMovementGizmos();
        }

        protected virtual void OnDrawGizmosSelected()
        {
            DrawDetailedGizmos();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Verifica se o objeto que entrou é um Player
            if (IsPlayerLayer(other.gameObject.layer))
            {
                _playerInContact = true;
                _playerCollider = other;

                _currentInputType = DetectCurrentInputType();
                ShowInteractionButtons();}
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Verifica se o objeto que saiu é o Player em contato
            if (other == _playerCollider)
            {
                _playerInContact = false;
                _playerCollider = null;
                HideAllButtons();}
        }
        #endregion

        #region Public Methods - Property Accessors
        /// <summary>
        /// Retorna o tipo de movimento especial deste ponto.
        /// </summary>
        /// <returns>MovementType (Jump ou Shrink)</returns>
        public MovementType GetMovementType()
        {
            return movementType;
        }

        /// <summary>
        /// Retorna o Transform do ponto de destino.
        /// </summary>
        /// <returns>Transform do destino ou null se não configurado</returns>
        public Transform GetDestinationPoint()
        {
            return destinationPoint;
        }

        /// <summary>
        /// Retorna a posição do ponto de destino.
        /// </summary>
        /// <returns>Vector3 da posição de destino</returns>
        public Vector3 GetDestinationPosition()
        {
            return destinationPoint != null ? destinationPoint.position : Vector3.zero;
        }

        /// <summary>
        /// Retorna a duração configurada para o movimento.
        /// </summary>
        /// <returns>Tempo em segundos</returns>
        public float GetMovementDuration()
        {
            return movementDuration;
        }

        /// <summary>
        /// Verifica se o Player está em contato com este ponto.
        /// </summary>
        /// <returns>True se Player está em contato</returns>
        public bool IsPlayerInContact()
        {
            return _playerInContact;
        }

        /// <summary>
        /// Retorna o Collider2D do Player em contato (se houver).
        /// </summary>
        /// <returns>Collider2D do Player ou null</returns>
        public Collider2D GetPlayerCollider()
        {
            return _playerCollider;
        }

        /// <summary>
        /// Retorna o nome personalizado do movimento.
        /// </summary>
        /// <returns>String com nome do movimento ou nome baseado no tipo</returns>
        public string GetMovementName()
        {
            return movementType switch
            {
                MovementType.Jump => "Jump Movement",
                MovementType.Shrink => "Shrink Movement",
                _ => "Special Movement"
            };
        }

        /// <summary>
        /// Retorna a descrição do movimento.
        /// </summary>
        /// <returns>String com descrição do movimento</returns>
        public string GetMovementDescription()
        {
            return movementType switch
            {
                MovementType.Jump => "Player jumps to the destination point",
                MovementType.Shrink => "Player shrinks and moves to the destination point",
                _ => "Special movement to destination point"
            };
        }

        /// <summary>
        /// Verifica se o ponto de movimento está configurado corretamente.
        /// </summary>
        /// <returns>True se válido, false caso contrário</returns>
        public bool IsValidMovementPoint()
        {
            return destinationPoint != null && movementDuration > 0f;
        }

        /// <summary>
        /// Retorna a distância até o ponto de destino.
        /// </summary>
        /// <returns>Distância em unidades ou -1 se destino não configurado</returns>
        public float GetDistanceToDestination()
        {
            if (destinationPoint == null) return -1f;
            return Vector3.Distance(transform.position, destinationPoint.position);
        }

        /// <summary>
        /// Calcula a velocidade necessária para completar o movimento no tempo configurado.
        /// </summary>
        /// <returns>Velocidade em unidades por segundo</returns>
        public float GetRequiredSpeed()
        {
            if (!IsValidMovementPoint()) return 0f;

            float distance = GetDistanceToDestination();
            return distance / movementDuration;
        }
        #endregion

        #region Public Methods - Configuration
        /// <summary>
        /// Define o tipo de movimento especial.
        /// </summary>
        /// <param name="type">Novo tipo de movimento</param>
        public void SetMovementType(MovementType type)
        {
            movementType = type;
        }

        /// <summary>
        /// Define o ponto de destino.
        /// </summary>
        /// <param name="destination">Transform do novo destino</param>
        public void SetDestinationPoint(Transform destination)
        {
            destinationPoint = destination;
        }

        /// <summary>
        /// Define a duração do movimento.
        /// </summary>
        /// <param name="duration">Duração em segundos</param>
        public void SetMovementDuration(float duration)
        {
            movementDuration = Mathf.Max(0.1f, duration); // Mínimo de 0.1s
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Valida a configuração do ponto de movimento.
        /// </summary>
        private void ValidateConfiguration()
        {
            if (destinationPoint == null)
            {}

            // Validação adicional: verificar se destino não é o próprio objeto
            if (destinationPoint == transform)
            {destinationPoint = null;
            }

            // Validação da duração
            if (movementDuration <= 0f)
            {movementDuration = 1f;
            }
        }

        /// <summary>
        /// Inicializa o Collider2D trigger para detecção do Player.
        /// </summary>
        private void InitializeTriggerCollider()
        {
            _triggerCollider = GetComponent<Collider2D>();

            if (_triggerCollider == null)
            {CircleCollider2D autoCollider = gameObject.AddComponent<CircleCollider2D>();
                autoCollider.isTrigger = true;
                autoCollider.radius = 1f;
                _triggerCollider = autoCollider;
            }
            else
            {
                // Garante que o collider seja trigger
                _triggerCollider.isTrigger = true;
            }
        }

        /// <summary>
        /// Verifica se uma layer corresponde ao Player.
        /// </summary>
        /// <param name="layer">Layer a verificar</param>
        /// <returns>True se for layer do Player</returns>
        private bool IsPlayerLayer(int layer)
        {
            return (playerLayers.value & (1 << layer)) != 0;
        }

        /// <summary>
        /// Desenha gizmos para visualizar o movimento no editor.
        /// </summary>
        private void DrawMovementGizmos()
        {
            if (destinationPoint == null) return;

            // Cor baseada no tipo de movimento
            Color gizmoColor = movementType switch
            {
                MovementType.Jump => Color.green,
                MovementType.Shrink => Color.blue,
                _ => Color.white
            };

            Gizmos.color = gizmoColor;

            // Linha conectando origem ao destino
            Gizmos.DrawLine(transform.position, destinationPoint.position);

            // Esfera no destino
            Gizmos.DrawWireSphere(destinationPoint.position, 0.3f);

            // Ícone no ponto de origem baseado no tipo
            if (movementType == MovementType.Jump)
            {
                // Desenha um "^" para jump
                Gizmos.DrawWireCube(transform.position, Vector3.one * 0.2f);
            }
            else
            {
                // Desenha um círculo menor para shrink
                Gizmos.DrawWireSphere(transform.position, 0.15f);
            }

            // Desenha área de detecção se collider existir
            if (_triggerCollider != null)
            {
                Gizmos.color = gizmoColor * 0.3f;
                if (_triggerCollider is CircleCollider2D circleCollider)
                {
                    Gizmos.DrawWireSphere(transform.position, circleCollider.radius);
                }
                else if (_triggerCollider is BoxCollider2D boxCollider)
                {
                    Gizmos.DrawWireCube(transform.position, boxCollider.size);
                }
            }
        }

        /// <summary>
        /// Desenha gizmos detalhados quando selecionado.
        /// </summary>
        private void DrawDetailedGizmos()
        {
            if (destinationPoint == null) return;

            // Seta indicando direção
            Vector3 direction = (destinationPoint.position - transform.position).normalized;
            Vector3 arrowStart = transform.position + direction * 0.1f;
            Vector3 arrowEnd = destinationPoint.position - direction * 0.1f;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(arrowStart, direction * Vector3.Distance(arrowStart, arrowEnd));

            // Informações de debug
            Vector3 midPoint = (transform.position + destinationPoint.position) * 0.5f;

#if UNITY_EDITOR
            string debugInfo = $"{GetMovementName()}\n" +
                              $"Distance: {GetDistanceToDestination():F1}\n" +
                              $"Duration: {movementDuration:F1}s\n" +
                              $"Speed: {GetRequiredSpeed():F1}\n" +
                              $"Player Contact: {_playerInContact}";

            UnityEditor.Handles.Label(midPoint, debugInfo);
#endif
        }
        #endregion


    }
}
