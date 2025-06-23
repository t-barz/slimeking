using System.Collections.Generic;
using UnityEngine;

namespace TheSlimeKing.Gameplay.Stealth
{
    /// <summary>
    /// Implementa o sistema de stealth do jogo, permitindo que o personagem se esconda em objetos de cobertura
    /// </summary>
    public class StealthController : MonoBehaviour
    {
        [Header("Configurações de Stealth")]
        [SerializeField] private float detectionRadius = 1.0f;
        [SerializeField] private LayerMask coverObjectLayers;
        [SerializeField] private GameObject stealthEffectPrefab;
        [SerializeField] private GameObject stealthIconPrefab;

        [Header("Referências")]
        [SerializeField] private SlimeMovement movementController;
        [SerializeField] private SlimeVisualController visualController;
        [SerializeField] private SlimeAnimationController animationController;

        // Estado de Stealth
        private StealthState _currentState = StealthState.Normal;
        private bool _isInCoverArea = false;
        private GameObject _currentCoverObject = null;
        private GameObject _stealthEffectInstance = null;
        private GameObject _stealthIconInstance = null;

        // Lista de tags que representam objetos de cobertura
        private readonly HashSet<string> _coverTags = new HashSet<string> { "Grass", "Bush", "Rock", "Tree" };

        private void Awake()
        {
            // Busca controladores se não forem atribuídos no Inspector
            if (movementController == null)
                movementController = GetComponent<SlimeMovement>();

            if (visualController == null)
                visualController = GetComponent<SlimeVisualController>();

            if (animationController == null)
                animationController = GetComponent<SlimeAnimationController>();

            // Inicializa no estado normal
            SetStealthState(StealthState.Normal);
        }

        private void Update()
        {
            // Verifica continuamente por objetos de cobertura
            CheckForCoverObjects();

            // Atualiza o estado de stealth baseado nas condições
            UpdateStealthState();
        }

        /// <summary>
        /// Verifica objetos próximos que possam servir como cobertura
        /// </summary>
        private void CheckForCoverObjects()
        {
            bool wasInCoverArea = _isInCoverArea;
            _isInCoverArea = false;
            _currentCoverObject = null;

            // Encontra todos os colliders próximos
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, coverObjectLayers);

            foreach (var collider in colliders)
            {
                // Verifica se o objeto possui uma tag válida de cobertura
                if (_coverTags.Contains(collider.tag))
                {
                    _isInCoverArea = true;
                    _currentCoverObject = collider.gameObject;
                    break;
                }
            }

            // Notifica sobre a mudança de estado de cobertura
            if (wasInCoverArea != _isInCoverArea)
            {
                OnCoverAreaStatusChanged(_isInCoverArea);
            }
        }

        /// <summary>
        /// Chamado quando o personagem entra ou sai de uma área de cobertura
        /// </summary>
        private void OnCoverAreaStatusChanged(bool isInCoverArea)
        {
            // Atualiza o feedback visual se necessário
            UpdateVisualFeedback();
        }

        /// <summary>
        /// Chamado pelo SlimeInputHandler quando o jogador agacha
        /// </summary>
        public void OnCrouch(bool crouchState)
        {
            // Atualiza o estado de stealth baseado no crouch e área de cobertura
            UpdateStealthState();
        }

        /// <summary>
        /// Atualiza o estado de stealth baseado nas condições atuais
        /// </summary>
        private void UpdateStealthState()
        {
            // Obtém o estado de agachamento do controlador de movimento
            bool isCrouched = GetCrouchState();

            if (isCrouched && _isInCoverArea)
            {
                // Agachado + Em cobertura = Escondido
                SetStealthState(StealthState.Hidden);
            }
            else if (isCrouched && !_isInCoverArea)
            {
                // Agachado sem cobertura = Exposto
                SetStealthState(StealthState.Exposed);
            }
            else if (isCrouched)
            {
                // Apenas agachado
                SetStealthState(StealthState.Crouched);
            }
            else
            {
                // Normal
                SetStealthState(StealthState.Normal);
            }
        }

        /// <summary>
        /// Atualiza o estado de stealth e aplica os efeitos correspondentes
        /// </summary>
        private void SetStealthState(StealthState newState)
        {
            // Se não houver mudança, sair
            if (newState == _currentState)
                return;

            StealthState previousState = _currentState;
            _currentState = newState;

            // Aplica os efeitos do novo estado
            ApplyStateEffects(previousState, newState);

            // Atualiza o feedback visual
            UpdateVisualFeedback();

            // Log para debug
            Debug.Log($"Stealth State changed: {previousState} -> {_currentState}");
        }

        /// <summary>
        /// Aplica os efeitos de estado (movimento, detecção, visual)
        /// </summary>
        private void ApplyStateEffects(StealthState previousState, StealthState newState)
        {
            // Detecção de mudança no bloqueio de movimento
            bool wasMobilityBlocked = IsMobilityBlocked(previousState);
            bool isMobilityBlocked = IsMobilityBlocked(newState);

            // Se o movimento agora está bloqueado e antes não estava
            if (isMobilityBlocked && !wasMobilityBlocked)
            {
                // Bloqueia o movimento
                movementController?.DisableControl();
            }
            // Se o movimento agora está desbloqueado e antes estava bloqueado
            else if (!isMobilityBlocked && wasMobilityBlocked)
            {
                // Desbloqueia o movimento
                movementController?.EnableControl();
            }

            // Aplica alterações visuais baseadas no estado
            if (visualController != null)
            {
                switch (newState)
                {
                    case StealthState.Hidden:
                        // Aplicar efeito de vinheta escura (Implementado em UpdateVisualFeedback)
                        visualController.SetAlpha(0.7f); // Torna o slime mais transparente quando escondido
                        break;
                    case StealthState.Crouched:
                    case StealthState.Exposed:
                        // Deixar o slime normal, mas animação agachada
                        visualController.SetAlpha(1.0f);
                        break;
                    case StealthState.Normal:
                        // Slime completamente visível
                        visualController.SetAlpha(1.0f);
                        break;
                }
            }

            // Aplica animações correspondentes
            if (animationController != null)
            {
                switch (newState)
                {
                    case StealthState.Hidden:
                    case StealthState.Crouched:
                    case StealthState.Exposed:
                        animationController.PlayShrinkAnimation();
                        break;
                    case StealthState.Normal:
                        animationController.PlayGrowAnimation();
                        break;
                }
            }
        }

        /// <summary>
        /// Atualiza o feedback visual baseado no estado atual
        /// </summary>
        private void UpdateVisualFeedback()
        {
            // Gerencia o efeito de vinheta quando escondido
            if (_currentState == StealthState.Hidden)
            {
                // Cria o efeito visual de stealth se não existir
                if (_stealthEffectInstance == null && stealthEffectPrefab != null)
                {
                    _stealthEffectInstance = Instantiate(stealthEffectPrefab, transform);
                }

                // Cria o ícone de "escondido" se não existir
                if (_stealthIconInstance == null && stealthIconPrefab != null)
                {
                    _stealthIconInstance = Instantiate(stealthIconPrefab, transform.position + Vector3.up * 1.0f, Quaternion.identity);
                    _stealthIconInstance.transform.SetParent(transform);
                }
            }
            else
            {
                // Remove o efeito visual quando não está escondido
                if (_stealthEffectInstance != null)
                {
                    Destroy(_stealthEffectInstance);
                    _stealthEffectInstance = null;
                }

                // Remove o ícone "escondido"
                if (_stealthIconInstance != null)
                {
                    Destroy(_stealthIconInstance);
                    _stealthIconInstance = null;
                }
            }
        }

        /// <summary>
        /// Verifica se o estado bloqueia o movimento
        /// </summary>
        private bool IsMobilityBlocked(StealthState state)
        {
            return state == StealthState.Crouched ||
                   state == StealthState.Hidden ||
                   state == StealthState.Exposed;
        }

        /// <summary>
        /// Obtém o estado atual de visibilidade do stealth
        /// </summary>
        public StealthState GetStealthState()
        {
            return _currentState;
        }

        /// <summary>
        /// Verifica se o personagem está escondido
        /// </summary>
        public bool IsHidden()
        {
            return _currentState == StealthState.Hidden;
        }

        /// <summary>
        /// Verifica se o personagem está detectável por inimigos
        /// </summary>
        public bool IsDetectable()
        {
            return _currentState == StealthState.Normal || _currentState == StealthState.Exposed;
        }        /// <summary>
                 /// Acessa o estado de agachamento do personagem
                 /// </summary>
        private bool GetCrouchState()
        {
            // Verifica se temos acesso ao controlador de movimento
            if (movementController != null)
            {
                return movementController.IsCrouching();
            }

            // Fallback: verifica o estado atual
            return _currentState == StealthState.Crouched ||
                   _currentState == StealthState.Hidden ||
                   _currentState == StealthState.Exposed;
        }

        /// <summary>
        /// Para visualização no editor: desenha o raio de detecção de cobertura
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
