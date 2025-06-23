using UnityEngine;
using System.Collections;
using TheSlimeKing.Gameplay.Interactive;

namespace TheSlimeKing.Core.Portal
{
    /// <summary>
    /// Controla uma instância específica de portal no mundo de jogo
    /// Oferece suporte para ativação por toque ou interação manual
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PortalController : InteractableObject
    {
        [Header("Configuração do Portal")]
        [Tooltip("ID único deste portal, usado para referenciar como destino")]
        [SerializeField] private string _portalID;
        [Tooltip("Tipo de ativação do portal")]
        [SerializeField] private PortalActivationType _activationType = PortalActivationType.Interaction;

        [Header("Destino do Portal")]
        [Tooltip("Este portal teleporta para outra cena")]
        [SerializeField] private bool _isScenePortal = false;
        [Tooltip("Nome da cena de destino (obrigatório se isScenePortal = true)")]
        [SerializeField] private string _targetSceneName;
        [Tooltip("ID do portal de destino na mesma cena ou na cena alvo")]
        [SerializeField] private string _targetPortalID;
        [Tooltip("Posição específica de destino (usado apenas se targetPortalID estiver vazio)")]
        [SerializeField] private Vector3 _targetPosition;
        [Tooltip("Rotação de destino")]
        [SerializeField] private Vector3 _targetRotation;

        [Header("Offset de Saída")]
        [Tooltip("Offset aplicado à posição do jogador quando ele sai do portal")]
        [SerializeField] private Vector3 _exitOffset = new Vector3(0, 1f, 0);

        [Header("Efeitos Visuais")]
        [Tooltip("Partículas que ficam ativas enquanto o portal está disponível")]
        [SerializeField] private GameObject _portalIdleEffect;
        [Tooltip("Animação que ocorre quando o portal é ativado")]
        [SerializeField] private GameObject _portalActivationEffect;

        [Header("Condições de Acesso")]
        [Tooltip("Estágio mínimo de crescimento necessário para usar o portal")]
        [SerializeField] private int _requiredGrowthStage = 0;
        [Tooltip("O portal está inicialmente ativo")]
        [SerializeField] private bool _startActive = true;

        // Propriedades privadas
        private float _activationTimer = 0f;
        private bool _isActivating = false;
        private const float TOUCH_ACTIVATION_DELAY = 0.5f;

        // Propriedades públicas
        public string PortalID => _portalID;

        protected override void Awake()
        {
            base.Awake();

            // Inicia ativo ou inativo conforme configuração
            gameObject.SetActive(_startActive);

            // Ativa efeito de idle
            if (_portalIdleEffect != null)
            {
                _portalIdleEffect.SetActive(true);
            }
        }

        protected virtual void Start()
        {
            // Configura a mensagem de interação correta baseada no tipo de portal
            if (_activationType == PortalActivationType.Interaction)
            {
                SetInteractionPrompt("Pressione E para entrar no portal"); // Usa o método público para definir o prompt
            }
        }

        protected virtual void Update()
        {
            // Processa ativação por toque quando jogador está na área
            if (_activationType == PortalActivationType.Touch && _isPlayerNearby && !_isActivating)
            {
                _activationTimer += Time.deltaTime;

                if (_activationTimer >= TOUCH_ACTIVATION_DELAY)
                {
                    _isActivating = true;
                    _activationTimer = 0f;

                    // Ativa o portal
                    ActivatePortal(GameObject.FindGameObjectWithTag("Player"));
                }
            }
            else if (!_isPlayerNearby)
            {
                // Resetar timer quando jogador sai da área
                _activationTimer = 0f;
            }
        }

        /// <summary>
        /// Executa a ação de ativação do portal
        /// </summary>
        private void ActivatePortal(GameObject player)
        {
            // Verifica se o crescimento do player atende ao requisito
            if (_requiredGrowthStage > 0)
            {
                var playerGrowth = player.GetComponent<TheSlimeKing.Core.PlayerGrowth>(); // Corrige o namespace para TheSlimeKing.Core
                if (playerGrowth != null && (int)playerGrowth.GetCurrentStage() < _requiredGrowthStage) // Usa GetCurrentStage() para obter o estágio atual
                {
                    Debug.Log("Portal requer estágio de crescimento maior que o atual");
                    return;
                }
            }

            // Ativa efeito de ativação
            if (_portalActivationEffect != null)
            {
                Instantiate(_portalActivationEffect, transform.position, Quaternion.identity);
            }

            // Executar teleporte baseado no tipo
            if (_isScenePortal)
            {
                // Portal entre cenas
                if (!string.IsNullOrEmpty(_targetSceneName))
                {
                    PortalManager.Instance.TeleportToScene(_targetSceneName, _targetPortalID,
                        string.IsNullOrEmpty(_targetPortalID) ? _targetPosition : (Vector3?)null,
                        _targetRotation);
                }
                else
                {
                    Debug.LogError("Portal entre cenas configurado sem cena de destino!");
                }
            }
            else
            {
                // Portal na mesma cena
                Vector3 destination;
                Vector3? lookDir = null;

                // Determina o destino
                if (!string.IsNullOrEmpty(_targetPortalID))
                {
                    // Busca o portal alvo pelo ID
                    PortalController targetPortal = FindTargetPortal(_targetPortalID);
                    if (targetPortal != null)
                    {
                        destination = targetPortal.transform.position + targetPortal.GetExitOffset();
                        lookDir = targetPortal.transform.eulerAngles;

                        // Desativa temporariamente o portal de destino para evitar loop
                        StartCoroutine(TemporarilyDisablePortal(targetPortal));
                    }
                    else
                    {
                        Debug.LogError($"Portal de destino com ID {_targetPortalID} não encontrado!");
                        return;
                    }
                }
                else
                {
                    // Usa posição específica configurada
                    destination = _targetPosition;
                    lookDir = _targetRotation;
                }

                // Executa o teleporte
                PortalManager.Instance.TeleportIntraSameCene(player, destination, lookDir);
            }
        }

        /// <summary>
        /// Busca um portal específico na cena pelo ID
        /// </summary>
        private PortalController FindTargetPortal(string id)
        {
            PortalController[] portals = FindObjectsByType<PortalController>(FindObjectsSortMode.None);
            foreach (var portal in portals)
            {
                if (portal.PortalID == id)
                    return portal;
            }
            return null;
        }

        /// <summary>
        /// Desativa temporariamente um portal para evitar teleportes em loop
        /// </summary>
        private IEnumerator TemporarilyDisablePortal(PortalController portal)
        {
            bool wasInteractable = portal.IsPlayerNearby(); // Substituir acesso direto por método público
            portal.SetInteractable(false); // Usa o método público para desativar a interatividade

            yield return new WaitForSeconds(1.5f);

            // Restaura o estado anterior
            SetInteractable(true); // Usa o método público para reativar a interatividade
        }

        /// <summary>
        /// Override do método de interação da base InteractableObject
        /// </summary>
        public override void Interact(GameObject interactor)
        {
            // Se for portal por interação, processa normalmente
            if (_activationType == PortalActivationType.Interaction)
            {
                base.Interact(interactor);
                ActivatePortal(interactor);
            }
        }

        /// <summary>
        /// Retorna o offset de saída deste portal
        /// </summary>
        public Vector3 GetExitOffset()
        {
            return _exitOffset;
        }

        /// <summary>
        /// Ativa ou desativa o portal
        /// </summary>
        public void SetPortalActive(bool active)
        {
            SetInteractable(active); // Usa o método público herdado para definir se é interagível

            if (_portalIdleEffect != null)
            {
                _portalIdleEffect.SetActive(active);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Desenha gizmos para visualização no editor
        /// </summary>
        private void OnDrawGizmos()
        {
            // Desenha a direção de saída
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, _exitOffset);
            Gizmos.DrawSphere(transform.position + _exitOffset, 0.2f);

            // Se tiver destino específico, desenha uma linha até o destino
            if (!_isScenePortal && string.IsNullOrEmpty(_targetPortalID) && _targetPosition != Vector3.zero)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, _targetPosition);
                Gizmos.DrawSphere(_targetPosition, 0.3f);
            }
        }
#endif
    }

    /// <summary>
    /// Define o tipo de ativação do portal
    /// </summary>
    public enum PortalActivationType
    {
        /// <summary>
        /// Portal ativa quando jogador toca nele
        /// </summary>
        Touch,

        /// <summary>
        /// Portal requer interação explícita do jogador
        /// </summary>
        Interaction
    }
}
