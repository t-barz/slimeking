using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TheSlimeKing.Gameplay.Movement
{
    /// <summary>
    /// Classe base para pontos de movimento especial no jogo
    /// </summary>
    public abstract class SpecialMovement : MonoBehaviour
    {
        [Header("Configurações Básicas")]
        [Tooltip("ID único para este ponto de movimento")]
        [SerializeField] protected string _pointID;
        
        [Tooltip("Distância máxima para ativar o movimento")]
        [SerializeField] protected float _activationDistance = 1.5f;
        
        [Tooltip("Ângulo máximo de desvio para ativação")]
        [SerializeField] protected float _maxAngleDeviation = 30f;
        
        [Tooltip("Camada do jogador para detecção")]
        [SerializeField] protected LayerMask _playerLayer;
        
        [Tooltip("Tempo que leva para completar o movimento")]
        [SerializeField] protected float _movementDuration = 1f;
        
        [Header("Visual Feedback")]
        [Tooltip("GameObject com ícone visual que aparece quando movimento está disponível")]
        [SerializeField] protected GameObject _interactionIcon;
        
        [Tooltip("Efeito visual ao iniciar o movimento")]
        [SerializeField] protected GameObject _startEffect;
        
        [Tooltip("Efeito visual ao concluir o movimento")]
        [SerializeField] protected GameObject _endEffect;
        
        [Header("Audio Feedback")]
        [Tooltip("Som ao iniciar o movimento especial")]
        [SerializeField] protected AudioClip _startSound;
        
        [Tooltip("Som ao completar o movimento especial")]
        [SerializeField] protected AudioClip _endSound;
        
        [Header("Eventos")]
        [SerializeField] protected UnityEvent _onMovementStart;
        [SerializeField] protected UnityEvent _onMovementComplete;
        
        // Referências
        protected AudioSource _audioSource;
        protected Transform _playerTransform;
        protected Rigidbody2D _playerRigidbody;
        protected PlayerController _playerController;
        
        // Estado
        protected bool _isPlayerInRange = false;
        protected bool _isMovementInProgress = false;

        /// <summary>
        /// Inicialização básica dos componentes
        /// </summary>
        protected virtual void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.spatialBlend = 1.0f;
                _audioSource.minDistance = 2.0f;
                _audioSource.maxDistance = 20.0f;
            }
            
            if (_interactionIcon != null)
            {
                _interactionIcon.SetActive(false);
            }
        }
        
        /// <summary>
        /// Verifica se o jogador está em posição de ativar o movimento
        /// </summary>
        protected virtual void Update()
        {
            if (_isMovementInProgress)
                return;
                
            CheckPlayerProximity();
            
            // Exibir ou esconder ícone de interação
            if (_interactionIcon != null)
            {
                _interactionIcon.SetActive(_isPlayerInRange && !_isMovementInProgress);
            }
            
            // Verificar input de ativação
            if (_isPlayerInRange && Input.GetButtonDown("Interact"))
            {
                StartSpecialMovement();
            }
        }
        
        /// <summary>
        /// Verifica se o jogador está próximo e orientado corretamente
        /// </summary>
        protected virtual void CheckPlayerProximity()
        {
            // Procura o jogador se ainda não tiver referência
            if (_playerTransform == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    _playerTransform = playerObject.transform;
                    _playerRigidbody = playerObject.GetComponent<Rigidbody2D>();
                    _playerController = playerObject.GetComponent<PlayerController>();
                }
                else
                {
                    return;
                }
            }
            
            // Verifica distância
            float distance = Vector2.Distance(transform.position, _playerTransform.position);
            if (distance <= _activationDistance)
            {
                // Verifica ângulo
                Vector2 directionToPoint = (transform.position - _playerTransform.position).normalized;
                Vector2 playerForward = _playerTransform.right * Mathf.Sign(_playerTransform.localScale.x);
                float angle = Vector2.Angle(playerForward, directionToPoint);
                
                _isPlayerInRange = angle <= _maxAngleDeviation;
            }
            else
            {
                _isPlayerInRange = false;
            }
        }
        
        /// <summary>
        /// Inicia a sequência de movimento especial
        /// </summary>
        protected virtual void StartSpecialMovement()
        {
            if (_isMovementInProgress || _playerController == null)
                return;
                
            _isMovementInProgress = true;
            
            // Exibe efeito visual de início
            if (_startEffect != null)
            {
                Instantiate(_startEffect, transform.position, Quaternion.identity);
            }
            
            // Reproduz som de início
            if (_startSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_startSound);
            }
            
            // Desativa controle do jogador
            _playerController.SetControlEnabled(false);
            
            // Aciona evento de início
            _onMovementStart?.Invoke();
            
            // Inicia a sequência específica do movimento
            StartCoroutine(PerformSpecialMovement());
        }
        
        /// <summary>
        /// Implementação específica da sequência de movimento
        /// </summary>
        protected abstract IEnumerator PerformSpecialMovement();
        
        /// <summary>
        /// Finaliza a sequência de movimento especial
        /// </summary>
        protected virtual void CompleteMovement()
        {
            // Exibe efeito visual de conclusão
            if (_endEffect != null)
            {
                Instantiate(_endEffect, _playerTransform.position, Quaternion.identity);
            }
            
            // Reproduz som de conclusão
            if (_endSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_endSound);
            }
            
            // Reativa o controle do jogador
            _playerController.SetControlEnabled(true);
            
            // Aciona evento de conclusão
            _onMovementComplete?.Invoke();
            
            // Reseta estado
            _isMovementInProgress = false;
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Desenha gizmos para visualização no editor
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            // Desenha área de ativação
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _activationDistance);
            
            // Desenha direção de movimento
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, transform.right);
        }
#endif
    }
}
