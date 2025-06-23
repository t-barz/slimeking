using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace TheSlimeKing.Gameplay.Interactive
{
    /// <summary>
    /// Componente para o jogador detectar e interagir com objetos interativos
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class InteractionDetector : MonoBehaviour
    {
        [Header("Configurações")]
        [SerializeField] private float _interactionRadius = 1.5f;
        [SerializeField] private LayerMask _interactableLayers;
        [SerializeField] private bool _showDebugGizmos = true;

        [Header("Input")]
        [SerializeField] private InputAction _interactAction;

        [Header("UI Feedback")]
        [SerializeField] private bool _showInteractionPrompt = true;
        [SerializeField] private GameObject _promptPrefab;
        [SerializeField] private Vector3 _promptOffset = new Vector3(0, 1.5f, 0);

        // Estado
        private List<IInteractable> _nearbyInteractables = new List<IInteractable>();
        private IInteractable _currentTarget;
        private GameObject _currentPrompt;

        private void Awake()
        {
            // Inicializa lista de objetos próximos
            _nearbyInteractables = new List<IInteractable>();

            // Configurar o prefab de prompt se necessário
            if (_showInteractionPrompt && _promptPrefab == null)
            {
                Debug.LogWarning("InteractionDetector: promptPrefab não foi configurado!");
            }
        }

        private void OnEnable()
        {
            // Habilita input
            if (_interactAction != null)
            {
                _interactAction.Enable();
                _interactAction.performed += OnInteractAction;
            }
            else
            {
                Debug.LogWarning("InteractionDetector: interactAction não foi configurado!");
            }
        }

        private void OnDisable()
        {
            // Desabilita input
            if (_interactAction != null)
            {
                _interactAction.Disable();
                _interactAction.performed -= OnInteractAction;
            }

            // Limpeza
            DestroyPrompt();
        }

        private void Update()
        {
            // Atualiza interactable mais próximo para foco
            UpdateClosestInteractable();

            // Atualiza prompt de interação
            UpdateInteractionPrompt();
        }

        /// <summary>
        /// Detecta e atualiza o interactable mais próximo
        /// </summary>
        private void UpdateClosestInteractable()
        {
            // Filtra apenas interagíveis que estão no alcance e podem interagir
            List<IInteractable> validInteractables = new List<IInteractable>();

            foreach (IInteractable interactable in _nearbyInteractables)
            {
                if (interactable != null && interactable.CanInteract(gameObject))
                {
                    validInteractables.Add(interactable);
                }
            }

            if (validInteractables.Count == 0)
            {
                _currentTarget = null;
                return;
            }

            // Se só há um, use-o
            if (validInteractables.Count == 1)
            {
                _currentTarget = validInteractables[0];
                return;
            }

            // Caso contrário, encontra o mais próximo
            float closestDistance = float.MaxValue;
            IInteractable closest = null;

            foreach (IInteractable interactable in validInteractables)
            {
                if (interactable.GetTransform() != null)
                {
                    float distance = Vector3.Distance(transform.position, interactable.GetTransform().position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = interactable;
                    }
                }
            }

            _currentTarget = closest;
        }

        /// <summary>
        /// Atualiza o prompt de interação na tela
        /// </summary>
        private void UpdateInteractionPrompt()
        {
            if (!_showInteractionPrompt)
                return;

            if (_currentTarget != null)
            {
                // Mostrar ou atualizar prompt
                if (_currentPrompt == null)
                {
                    CreatePrompt();
                }

                if (_currentPrompt != null)
                {
                    // Atualiza posição e texto
                    Vector3 promptPos = _currentTarget.GetTransform().position + _promptOffset;
                    _currentPrompt.transform.position = promptPos;

                    // Atualiza texto do prompt se tiver componente de texto
                    TMPro.TextMeshPro textComponent = _currentPrompt.GetComponentInChildren<TMPro.TextMeshPro>();
                    if (textComponent != null)
                    {
                        textComponent.text = _currentTarget.GetInteractionPrompt();
                    }
                }
            }
            else
            {
                // Destruir prompt se não houver alvo
                DestroyPrompt();
            }
        }

        /// <summary>
        /// Cria o prompt de interação
        /// </summary>
        private void CreatePrompt()
        {
            if (_promptPrefab != null)
            {
                _currentPrompt = Instantiate(_promptPrefab);

                // Configura para sempre olhar para a câmera se tiver Canvas
                Canvas canvas = _currentPrompt.GetComponentInChildren<Canvas>();
                if (canvas != null)
                {
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.worldCamera = Camera.main;
                }
            }
        }

        /// <summary>
        /// Destroi o prompt de interação
        /// </summary>
        private void DestroyPrompt()
        {
            if (_currentPrompt != null)
            {
                Destroy(_currentPrompt);
                _currentPrompt = null;
            }
        }

        /// <summary>
        /// Lida com a ação de interação do jogador
        /// </summary>
        private void OnInteractAction(InputAction.CallbackContext context)
        {
            if (_currentTarget != null)
            {
                _currentTarget.Interact(gameObject);
            }
        }

        #region Trigger Events

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Verifica se o objeto está na layer de interagíveis
            if (((1 << other.gameObject.layer) & _interactableLayers) == 0)
                return;

            // Adiciona à lista se implementar a interface
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null && !_nearbyInteractables.Contains(interactable))
            {
                _nearbyInteractables.Add(interactable);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Remove da lista quando sai do alcance
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null && _nearbyInteractables.Contains(interactable))
            {
                _nearbyInteractables.Remove(interactable);

                // Limpa o atual se for o mesmo
                if (_currentTarget == interactable)
                {
                    _currentTarget = null;
                }
            }
        }

        #endregion

        /// <summary>
        /// Desenha gizmos para visualizar a área de detecção
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!_showDebugGizmos)
                return;

            // Área de detecção
            Gizmos.color = new Color(0.2f, 1f, 0.3f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, _interactionRadius);

            // Linha para o alvo atual
            if (_currentTarget != null && Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _currentTarget.GetTransform().position);
            }
        }
    }
}
