using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SlimeKing.Core;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Handler para gerenciar interações do Player com objetos do ambiente
    /// 
    /// RESPONSABILIDADES:
    /// • Detecta objetos IInteractable próximos ao Player
    /// • Gerencia prioridades quando múltiplas interações estão disponíveis
    /// • Executa interações quando o Player pressiona o botão
    /// • Fornece informações para UI de prompts de interação
    /// 
    /// DEPENDÊNCIAS:
    /// • Deve ser anexado ao GameObject do Player
    /// • Requer Collider2D configurado como Trigger para detecção
    /// 
    /// USO:
    /// • O PlayerController chama ProcessInteractionInput() quando input é detectado
    /// • Automaticamente detecta objetos IInteractable em range
    /// • Prioriza interações baseado no valor de GetInteractionPriority()
    /// </summary>
    public class InteractionHandler : MonoBehaviour
    {
        #region Inspector Configuration

        [Header("Detection Configuration")]
        [Tooltip("Raio de detecção para objetos interativos")]
        [SerializeField] private float interactionRange = 1.5f;

        [Tooltip("Layers que contêm objetos interativos")]
        [SerializeField] private LayerMask interactionLayers = -1;

        [Header("Debug")]
        [Tooltip("Ativar logs de debug para interações")]
        [SerializeField] private bool enableDebugLogs = true; // Temporariamente true para debug

        [Tooltip("Mostrar gizmos de range de interação")]
        [SerializeField] private bool showInteractionGizmos = true;

        #endregion

        #region Private Variables

        private readonly List<IInteractable> _nearbyInteractables = new List<IInteractable>();
        private IInteractable _currentBestInteractable;
        
        // OTIMIZAÇÃO: Cache para evitar operações custosas a cada frame
        private float _lastUpdateTime;
        private const float UPDATE_INTERVAL = 0.1f; // Atualiza a cada 100ms ao invés de todo frame

        #endregion

        #region Public Properties

        /// <summary>
        /// Retorna o melhor objeto interativo atualmente disponível
        /// </summary>
        public IInteractable CurrentBestInteractable => _currentBestInteractable;

        /// <summary>
        /// Verifica se há alguma interação disponível
        /// </summary>
        public bool HasAvailableInteraction => _currentBestInteractable != null;

        #endregion

        #region Unity Lifecycle

        private void Update()
        {
            // OTIMIZAÇÃO: Só atualiza a cada UPDATE_INTERVAL segundos
            if (Time.time - _lastUpdateTime >= UPDATE_INTERVAL)
            {
                UpdateNearbyInteractables();
                _lastUpdateTime = Time.time;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                if (!_nearbyInteractables.Contains(interactable))
                {
                    _nearbyInteractables.Add(interactable);
                    LogDebug($"Objeto interativo detectado: {other.name}");
                }
            }
            else
            {
                LogDebug($"Objeto SEM IInteractable detectado: {other.name}");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                _nearbyInteractables.Remove(interactable);
                LogDebug($"Objeto interativo saiu de range: {other.name}");

                // Se era o melhor objeto, força reavaliação
                if (_currentBestInteractable == interactable)
                {
                    _currentBestInteractable = null;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Processa input de interação do Player
        /// Tenta interagir com o melhor objeto disponível
        /// </summary>
        /// <returns>True se uma interação foi executada com sucesso</returns>
        public bool ProcessInteractionInput()
        {
            if (_currentBestInteractable == null)
            {
                LogDebug("Nenhuma interação disponível");
                return false;
            }

            bool success = _currentBestInteractable.TryInteract(transform);

            if (success)
            {
                LogDebug($"Interação bem-sucedida com {_currentBestInteractable}");
            }
            else
            {
                LogDebug($"Interação falhou com {_currentBestInteractable}");
            }

            return success;
        }

        /// <summary>
        /// Retorna o prompt de interação atual para UI
        /// </summary>
        /// <returns>String com prompt ou vazia se não há interação</returns>
        public string GetCurrentInteractionPrompt()
        {
            return _currentBestInteractable?.GetInteractionPrompt() ?? "";
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// OTIMIZADO: Atualiza lista de objetos interativos e determina o melhor
        /// </summary>
        private void UpdateNearbyInteractables()
        {
            // OTIMIZAÇÃO: Remove objetos nulos de forma mais eficiente
            for (int i = _nearbyInteractables.Count - 1; i >= 0; i--)
            {
                var interactable = _nearbyInteractables[i];
                if (interactable == null || 
                    (interactable as MonoBehaviour) == null ||
                    !((MonoBehaviour)interactable).gameObject.activeInHierarchy)
                {
                    _nearbyInteractables.RemoveAt(i);
                }
            }

            // OTIMIZAÇÃO: Encontra o melhor objeto sem criar listas temporárias
            IInteractable bestInteractable = null;
            int highestPriority = int.MinValue;

            foreach (var interactable in _nearbyInteractables)
            {
                if (interactable.CanInteract(transform))
                {
                    int priority = interactable.GetInteractionPriority();
                    if (priority > highestPriority)
                    {
                        highestPriority = priority;
                        bestInteractable = interactable;
                    }
                }
            }

            // Só loga se mudou
            if (_currentBestInteractable != bestInteractable)
            {
                LogDebug($"Melhor interação mudou de {_currentBestInteractable?.GetType().Name} para {bestInteractable?.GetType().Name}");
                _currentBestInteractable = bestInteractable;
            }
        }

        /// <summary>
        /// Detecta objetos interativos em um raio específico usando Physics2D
        /// Método alternativo para caso o Trigger não seja suficiente
        /// </summary>
        private void DetectInteractablesInRange()
        {
            var colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactionLayers);

            _nearbyInteractables.Clear();

            foreach (var collider in colliders)
            {
                var interactable = collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    _nearbyInteractables.Add(interactable);
                }
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
                UnityEngine.Debug.Log($"[InteractionHandler-{name}] {message}");
            }
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (!showInteractionGizmos) return;

            // Desenha range de interação como círculo 2D
            Gizmos.color = Color.cyan;
            DrawWireCircle2D(transform.position, interactionRange);

            // Desenha objetos interativos detectados
            if (Application.isPlaying && _nearbyInteractables != null)
            {
                foreach (var interactable in _nearbyInteractables)
                {
                    if (interactable != null && interactable as MonoBehaviour != null)
                    {
                        var interactableTransform = ((MonoBehaviour)interactable).transform;

                        // Cor diferente para o melhor objeto
                        Gizmos.color = interactable == _currentBestInteractable ? Color.green : Color.yellow;
                        Gizmos.DrawLine(transform.position, interactableTransform.position);
                        DrawWireCircle2D(interactableTransform.position, 0.2f);
                    }
                }
            }
        }

        /// <summary>
        /// Desenha um círculo 2D usando Gizmos (compatível com Unity)
        /// </summary>
        private void DrawWireCircle2D(Vector3 center, float radius, int segments = 32)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = center + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 nextPoint = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0
                );

                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }

        #endregion
    }
}
