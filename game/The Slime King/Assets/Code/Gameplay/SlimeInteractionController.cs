using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal; // Para Light2D e recursos URP

namespace TheSlimeKing.Gameplay
{
    /// <summary>
    /// Gerencia as interações do slime com objetos no ambiente
    /// Compatível com URP 2D no Unity 6
    /// </summary>
    public class SlimeInteractionController : MonoBehaviour
    {
        [Header("Configurações de Detecção")]
        [SerializeField] private float interactionRadius = 1.5f;
        [SerializeField] private LayerMask interactableLayers;

        [Header("Referencias")]
        [SerializeField] private SlimeAnimationController animationController;
        [SerializeField] private SlimeVisualController visualController;

        [Header("Efeitos URP")]
        [SerializeField] private Light2D interactionLight;
        [SerializeField] private float interactionLightIntensity = 1f;
        [SerializeField] private Color interactionLightColor = Color.white;
        [SerializeField] private float interactionLightPulseSpeed = 2f;
        [SerializeField] private ParticleSystem interactionHintParticles;
        [SerializeField] private GameObject interactionIconPrefab;

        // Objetos interativos próximos
        private readonly List<IInteractable> _nearbyInteractables = new List<IInteractable>();

        // Interactable atual (o mais próximo/prioritário)
        private IInteractable _currentInteractable;
        private GameObject _currentInteractionIcon;

        // Controle de luz pulsante
        private float _pulseTimer = 0f;
        private float _baseLightIntensity; private void Awake()
        {
            // Busca controllers no mesmo GameObject primeiro
            if (animationController == null)
                animationController = GetComponent<SlimeAnimationController>();

            if (visualController == null)
                visualController = GetComponent<SlimeVisualController>();

            // Configura a luz de interação URP
            if (interactionLight != null)
            {
                _baseLightIntensity = interactionLightIntensity;
                interactionLight.intensity = 0f; // Começa desligada
                interactionLight.color = interactionLightColor;
                interactionLight.lightType = Light2D.LightType.Point;
            }

            // Desliga partículas de dica
            if (interactionHintParticles != null)
            {
                interactionHintParticles.Stop();
                interactionHintParticles.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            DetectInteractables();
            UpdateCurrentInteractable();
            UpdateInteractionEffects();
        }

        /// <summary>
        /// Detecta objetos interativos próximos
        /// </summary>
        private void DetectInteractables()
        {
            // Limpa a lista atual
            _nearbyInteractables.Clear();

            // Detecta colliders interativos próximos
            Collider2D[] colliders = Physics2D.OverlapCircleAll(
                transform.position,
                interactionRadius,
                interactableLayers
            );

            // Adiciona interactables válidos à lista
            foreach (Collider2D collider in colliders)
            {
                IInteractable interactable = collider.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanInteract())
                {
                    _nearbyInteractables.Add(interactable);
                }
            }
        }

        /// <summary>
        /// Atualiza qual é o objeto interativo atual (o mais próximo/prioritário)
        /// </summary>
        private void UpdateCurrentInteractable()
        {
            // Se não há interactables próximos
            if (_nearbyInteractables.Count == 0)
            {
                if (_currentInteractable != null)
                {
                    // Desativa o feedback visual do anterior
                    _currentInteractable.HideInteractionPrompt();
                    _currentInteractable = null;

                    // Desativa efeitos URP
                    DisableInteractionEffects();
                }
                return;
            }

            // Encontra o interactable mais próximo ou com maior prioridade
            IInteractable bestInteractable = _nearbyInteractables[0];
            float closestDistance = Vector3.Distance(transform.position, GetInteractablePosition(bestInteractable));

            foreach (IInteractable interactable in _nearbyInteractables)
            {
                float distance = Vector3.Distance(transform.position, GetInteractablePosition(interactable));

                // Leva em consideração a prioridade e a distância
                if ((interactable.GetInteractionPriority() > bestInteractable.GetInteractionPriority()) ||
                    (interactable.GetInteractionPriority() == bestInteractable.GetInteractionPriority() &&
                     distance < closestDistance))
                {
                    bestInteractable = interactable;
                    closestDistance = distance;
                }
            }

            // Se mudou o interactable atual
            if (_currentInteractable != bestInteractable)
            {
                // Desativa o feedback do anterior
                if (_currentInteractable != null)
                {
                    _currentInteractable.HideInteractionPrompt();
                }

                // Ativa o feedback do novo
                bestInteractable.ShowInteractionPrompt();

                _currentInteractable = bestInteractable;

                // Ativa efeitos URP para o novo interactable
                EnableInteractionEffects();
            }
        }

        /// <summary>
        /// Atualiza os efeitos visuais URP para interação
        /// </summary>
        private void UpdateInteractionEffects()
        {
            if (_currentInteractable == null)
            {
                return;
            }

            // Posiciona os efeitos perto do objeto interativo
            Vector3 interactablePos = GetInteractablePosition(_currentInteractable);

            // Atualiza a luz pulsante
            if (interactionLight != null)
            {
                _pulseTimer += Time.deltaTime * interactionLightPulseSpeed;
                float pulse = Mathf.Sin(_pulseTimer) * 0.5f + 0.5f;
                interactionLight.intensity = _baseLightIntensity * pulse;

                // Posiciona a luz no meio do caminho entre o slime e o interactable
                interactionLight.transform.position = Vector3.Lerp(
                    transform.position,
                    interactablePos,
                    0.7f
                );
            }

            // Atualiza o ícone de interação (se existe)
            if (_currentInteractionIcon != null)
            {
                // Posiciona o ícone acima do interactable
                _currentInteractionIcon.transform.position = interactablePos + Vector3.up * 0.75f;

                // Faz o ícone oscilar suavemente
                Vector3 iconScale = Vector3.one * (0.8f + Mathf.Sin(_pulseTimer * 0.8f) * 0.2f);
                _currentInteractionIcon.transform.localScale = iconScale;
            }
        }

        /// <summary>
        /// Ativa os efeitos visuais URP da interação
        /// </summary>
        private void EnableInteractionEffects()
        {
            // Ativa luz de interação
            if (interactionLight != null)
            {
                interactionLight.gameObject.SetActive(true);

                // Ajusta cor conforme tipo de interação
                if (_currentInteractable != null)
                {
                    InteractionType type = _currentInteractable.GetInteractionType();
                    switch (type)
                    {
                        case InteractionType.Shrink:
                            interactionLight.color = new Color(0.2f, 0.7f, 1f); // Azul
                            break;
                        case InteractionType.Jump:
                            interactionLight.color = new Color(0.2f, 1f, 0.3f); // Verde
                            break;
                        case InteractionType.Collect:
                            interactionLight.color = new Color(1f, 0.8f, 0.2f); // Amarelo/dourado
                            break;
                        default:
                            interactionLight.color = interactionLightColor; // Padrão
                            break;
                    }
                }
            }

            // Ativa partículas
            if (interactionHintParticles != null)
            {
                interactionHintParticles.gameObject.SetActive(true);

                // Posiciona as partículas próximas ao objeto interativo
                if (_currentInteractable != null)
                {
                    MonoBehaviour mono = _currentInteractable as MonoBehaviour;
                    if (mono != null)
                    {
                        interactionHintParticles.transform.position = mono.transform.position;
                    }
                }

                interactionHintParticles.Play();
            }

            // Cria ícone de interação se não existir
            if (interactionIconPrefab != null && _currentInteractionIcon == null)
            {
                _currentInteractionIcon = Instantiate(interactionIconPrefab);
            }
        }

        /// <summary>
        /// Desativa os efeitos visuais URP da interação
        /// </summary>
        private void DisableInteractionEffects()
        {
            // Desativa luz
            if (interactionLight != null)
            {
                interactionLight.intensity = 0f;
                interactionLight.gameObject.SetActive(false);
            }

            // Desativa partículas
            if (interactionHintParticles != null)
            {
                interactionHintParticles.Stop();
                interactionHintParticles.gameObject.SetActive(false);
            }

            // Remove ícone
            if (_currentInteractionIcon != null)
            {
                Destroy(_currentInteractionIcon);
                _currentInteractionIcon = null;
            }
        }

        /// <summary>
        /// Executa a interação com o objeto atual
        /// </summary>
        public void Interact()
        {
            if (_currentInteractable != null)
            {
                // Executa a interação
                InteractionType type = _currentInteractable.GetInteractionType();
                _currentInteractable.Interact(gameObject);

                // Ativa a animação correspondente - conforme documento técnico
                if (animationController != null)
                {
                    switch (type)
                    {
                        case InteractionType.Shrink:
                            animationController.PlayShrinkAnimation();
                            break;
                        case InteractionType.Jump:
                            animationController.PlayJumpAnimation();
                            break;
                        default:
                            // Outras interações não têm animações especiais
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Tenta interagir com o objeto interativo atual
        /// </summary>
        public void TryInteract()
        {
            if (_currentInteractable != null && _currentInteractable.CanInteract())
            {
                // Executa a interação
                _currentInteractable.Interact(this.gameObject);

                // Animação de interação (se aplicável)
                if (animationController != null)
                {
                    // Podemos adicionar uma animação específica para interação no futuro
                    // Por enquanto, usamos uma animação existente
                    animationController.PlayShrinkAnimation();
                }

                // Efeitos visuais de interação
                if (interactionHintParticles != null)
                {
                    // Burst de partículas
                    interactionHintParticles.Play();
                }
            }
        }

        /// <summary>
        /// Obtém a posição de um objeto interativo
        /// </summary>
        private Vector3 GetInteractablePosition(IInteractable interactable)
        {
            // Tenta obter um Transform do interactable
            MonoBehaviour monoBehaviour = interactable as MonoBehaviour;

            if (monoBehaviour != null)
            {
                return monoBehaviour.transform.position;
            }

            // Fallback para a posição do próprio slime
            return transform.position;
        }

        /// <summary>
        /// Para debug: desenha o raio de interação
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }

    /// <summary>
    /// Interface para objetos com os quais o slime pode interagir
    /// </summary>
    public interface IInteractable
    {
        bool CanInteract();
        void Interact(GameObject initiator);
        void ShowInteractionPrompt();
        void HideInteractionPrompt();
        int GetInteractionPriority();
        InteractionType GetInteractionType();
    }

    /// <summary>
    /// Tipos de interação possíveis
    /// </summary>
    public enum InteractionType
    {
        Generic,
        Shrink,
        Jump,
        Collect,
        Talk
    }
}
