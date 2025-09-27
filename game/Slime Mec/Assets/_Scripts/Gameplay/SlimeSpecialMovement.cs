using UnityEngine;
using System.Collections;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Classe base para movimentos especiais do Slime (Shrink, Jump).
    /// Herda de InteractivePointHandler e adiciona movimento simples até destino.
    /// 
    /// FUNCIONALIDADES:
    /// • Sistema de movimento com tempo controlado
    /// • Desativação automática de colliders durante movimento
    /// • Integração com sistema de animação
    /// • Herda comportamentos de InteractivePointHandler
    /// 
    /// FLUXO DE EXECUÇÃO:
    /// 1. Player entra na área → InteractivePointHandler mostra botões
    /// 2. Player pressiona interação → OnInteractPressed() é chamado
    /// 3. Verifica se pode mover → ExecuteMovement()
    /// 4. Triggera animação → Move até destino → Reativa colliders
    /// 
    /// DEPENDÊNCIAS:
    /// • InteractivePointHandler (classe pai)
    /// • PlayerController para movimento
    /// • Animator para animações
    /// </summary>
    public class SlimeSpecialMovement : InteractivePointHandler
    {
        #region Serialized Fields
        [Header("🎯 Movement Settings")]
        [Tooltip("Ponto de destino do movimento")]
        [SerializeField] protected Transform destinationPoint;

        [Tooltip("Tempo de deslocamento (em segundos)")]
        [SerializeField, Range(0.1f, 5f)] protected float movementDuration = 2f;

        [Header("🎬 Animation")]
        [Tooltip("Nome do trigger da animação")]
        [SerializeField] protected string animationTrigger = "SpecialMove";
        #endregion

        #region Protected Fields
        protected PlayerController _playerController;
        protected Animator _playerAnimator;
        protected Collider2D[] _playerColliders;
        protected bool _isMoving = false;
        #endregion

        #region Unity Lifecycle
        protected virtual void Start()
        {
            FindPlayerComponents();
            ValidateConfiguration();
        }

        protected virtual void OnDrawGizmos()
        {
            if (destinationPoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, destinationPoint.position);
                Gizmos.DrawWireSphere(destinationPoint.position, 0.3f);

                // Seta indicando direção
                Vector3 direction = (destinationPoint.position - transform.position).normalized;
                Vector3 arrowPos = destinationPoint.position - direction * 0.3f;
                Gizmos.DrawRay(arrowPos, direction * 0.2f);
            }

            // Área de interação herdada do InteractivePointHandler
            Collider2D trigger = GetComponent<Collider2D>();
            if (trigger != null && trigger.isTrigger)
            {
                Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
                if (trigger is CircleCollider2D circle)
                {
                    Gizmos.DrawWireSphere(transform.position, circle.radius);
                }
                else if (trigger is BoxCollider2D box)
                {
                    Gizmos.DrawWireCube(transform.position, box.size);
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Método chamado quando o player pressiona o botão de interação.
        /// Override do comportamento padrão do InteractivePointHandler.
        /// </summary>
        public virtual void OnInteractPressed()
        {
            if (!CanStartMovement()) return;

            StartCoroutine(ExecuteMovement());
        }

        /// <summary>
        /// Verifica se pode iniciar o movimento especial.
        /// </summary>
        public virtual bool CanStartMovement()
        {
            if (_isMoving)
            {
                return false;
            }

            if (destinationPoint == null)
            {
                Debug.LogError($"SlimeSpecialMovement: Destination point não definido em '{gameObject.name}'", this);
                return false;
            }

            if (!HasPlayerComponents())
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Executa a sequência completa de movimento especial.
        /// </summary>
        protected virtual IEnumerator ExecuteMovement()
        {
            _isMoving = true;

            // Triggera animação
            TriggerAnimation();

            // Desativa colliders do player
            DisablePlayerColliders();

            // Executa movimento até destino
            yield return StartCoroutine(MoveToDestination());

            // Reativa colliders do player
            EnablePlayerColliders();

            _isMoving = false;
        }

        /// <summary>
        /// Triggera a animação específica do movimento.
        /// </summary>
        protected virtual void TriggerAnimation()
        {
            if (_playerAnimator != null && !string.IsNullOrEmpty(animationTrigger))
            {
                _playerAnimator.SetTrigger(animationTrigger);
            }
        }

        /// <summary>
        /// Move o player até o ponto de destino.
        /// </summary>
        protected virtual IEnumerator MoveToDestination()
        {
            if (_playerController == null) yield break;

            Vector3 startPos = _playerController.transform.position;
            Vector3 endPos = destinationPoint.position;

            float elapsed = 0f;

            while (elapsed < movementDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / movementDuration;

                Vector3 currentPos = CalculatePosition(startPos, endPos, progress);
                _playerController.transform.position = currentPos;

                yield return null;
            }

            // Garante posição final exata
            _playerController.transform.position = endPos;
        }

        /// <summary>
        /// Calcula a posição durante o movimento.
        /// Método virtual para permitir diferentes tipos de movimento (linear, arco, etc).
        /// </summary>
        /// <param name="start">Posição inicial</param>
        /// <param name="end">Posição final</param>
        /// <param name="progress">Progresso do movimento (0-1)</param>
        /// <returns>Posição atual calculada</returns>
        protected virtual Vector3 CalculatePosition(Vector3 start, Vector3 end, float progress)
        {
            return Vector3.Lerp(start, end, progress);
        }

        /// <summary>
        /// Desativa todos os colliders do player.
        /// </summary>
        protected virtual void DisablePlayerColliders()
        {
            if (_playerColliders != null)
            {
                foreach (var collider in _playerColliders)
                {
                    if (collider != null)
                    {
                        collider.enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// Reativa todos os colliders do player.
        /// </summary>
        protected virtual void EnablePlayerColliders()
        {
            if (_playerColliders != null)
            {
                foreach (var collider in _playerColliders)
                {
                    if (collider != null)
                    {
                        collider.enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Encontra e cacheia os componentes do player.
        /// </summary>
        protected virtual void FindPlayerComponents()
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _playerController = playerObject.GetComponent<PlayerController>();
                _playerAnimator = playerObject.GetComponent<Animator>();
                _playerColliders = playerObject.GetComponentsInChildren<Collider2D>();
            }
            else
            {
                Debug.LogError("SlimeSpecialMovement: Player não encontrado (tag 'Player')", this);
            }
        }

        /// <summary>
        /// Verifica se tem todos os componentes necessários do player.
        /// </summary>
        protected virtual bool HasPlayerComponents()
        {
            return _playerController != null && _playerColliders != null && _playerColliders.Length > 0;
        }

        /// <summary>
        /// Valida a configuração da classe.
        /// </summary>
        protected virtual void ValidateConfiguration()
        {
            if (destinationPoint == null)
            {
                Debug.LogWarning($"SlimeSpecialMovement: Destination point não definido em '{gameObject.name}'", this);
            }

            if (movementDuration <= 0f)
            {
                movementDuration = 1f;
                Debug.LogWarning($"SlimeSpecialMovement: Movement duration inválida, usando 1s em '{gameObject.name}'", this);
            }

            // Verifica se tem collider trigger (necessário para InteractivePointHandler)
            Collider2D trigger = GetComponent<Collider2D>();
            if (trigger == null || !trigger.isTrigger)
            {
                Debug.LogError($"SlimeSpecialMovement: Collider2D com isTrigger=true é necessário em '{gameObject.name}'", this);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Verifica se está em movimento.
        /// </summary>
        public bool IsMoving => _isMoving;

        /// <summary>
        /// Ponto de destino atual.
        /// </summary>
        public Transform DestinationPoint => destinationPoint;

        /// <summary>
        /// Duração do movimento em segundos.
        /// </summary>
        public float MovementDuration => movementDuration;

        /// <summary>
        /// Progresso do movimento atual (0-1, -1 se não está em movimento).
        /// </summary>
        public float MovementProgress
        {
            get
            {
                // Esta propriedade pode ser implementada por classes filhas se necessário
                return _isMoving ? 0.5f : -1f; // Placeholder
            }
        }
        #endregion

        #region Context Menu (Editor Only)
#if UNITY_EDITOR
        [ContextMenu("🎯 Test Movement")]
        private void TestMovement()
        {
            if (Application.isPlaying)
            {
                OnInteractPressed();
            }
            else
            {
                Debug.LogWarning("SlimeSpecialMovement: Teste só funciona no Play Mode");
            }
        }

        [ContextMenu("🔍 Debug Movement Info")]
        private void DebugMovementInfo()
        {
            Debug.Log($"SlimeSpecialMovement Debug Info:" +
                      $"\n• GameObject: {gameObject.name}" +
                      $"\n• Is Moving: {_isMoving}" +
                      $"\n• Destination Point: {(destinationPoint != null ? destinationPoint.name : "NULL")}" +
                      $"\n• Movement Duration: {movementDuration}s" +
                      $"\n• Animation Trigger: '{animationTrigger}'" +
                      $"\n• Player Controller: {(_playerController != null ? "OK" : "NULL")}" +
                      $"\n• Player Animator: {(_playerAnimator != null ? "OK" : "NULL")}" +
                      $"\n• Player Colliders: {(_playerColliders != null ? _playerColliders.Length : 0)}" +
                      $"\n• Can Start Movement: {CanStartMovement()}", this);
        }

        [ContextMenu("🔧 Validate Setup")]
        private void ValidateSetup()
        {
            ValidateConfiguration();
            Debug.Log("Setup validation completed - check console for warnings/errors");
        }
#endif
        #endregion
    }
}