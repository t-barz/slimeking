using UnityEngine;
using UnityEngine.Rendering.Universal;
using TheSlimeKing.Core;

namespace TheSlimeKing.Gameplay
{
    /// <summary>
    /// Gerencia as mudanças visuais do slime durante o crescimento
    /// </summary>
    public class SlimeGrowthVisuals : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private SpriteRenderer _bodyRenderer;
        [SerializeField] private Transform _colliderTransform;
        [SerializeField] private Animator _animator;

        [Header("Colliders")]
        [SerializeField] private CircleCollider2D _physicsCollider;
        [SerializeField] private CircleCollider2D _interactionCollider;

        [Header("Ajuste de Colliders")]
        [SerializeField] private float _physicsColliderSizeMultiplier = 0.9f;
        [SerializeField] private float _interactionColliderSizeMultiplier = 1.1f;

        // Cache de componentes opcionais
        private ParticleSystem[] _attachedParticleSystems;
        private Light2D _attachedLight;

        private void Start()
        {
            // Registra no evento de mudança de estágio
            if (PlayerGrowth.Instance != null)
            {
                PlayerGrowth.Instance.OnGrowthStageChanged.AddListener(HandleGrowthChange);
            }

            // Guarda referência para sistemas de partículas
            _attachedParticleSystems = GetComponentsInChildren<ParticleSystem>();

            // Tenta encontrar light (pacote URP)
            _attachedLight = GetComponentInChildren<Light2D>();

            // Realiza ajustes iniciais
            if (PlayerGrowth.Instance != null)
            {
                HandleGrowthChange(PlayerGrowth.Instance.GetCurrentStage());
            }
        }

        private void OnDestroy()
        {
            // Remove o listener
            if (PlayerGrowth.Instance != null)
            {
                PlayerGrowth.Instance.OnGrowthStageChanged.RemoveListener(HandleGrowthChange);
            }
        }

        /// <summary>
        /// Trata as mudanças visuais em resposta a uma mudança de estágio
        /// </summary>
        private void HandleGrowthChange(SlimeStage newStage)
        {
            if (PlayerGrowth.Instance == null) return;

            // Obtém configuração atual
            SlimeGrowthStage stageConfig = PlayerGrowth.Instance.GetCurrentStageConfig();
            if (stageConfig == null) return;

            // Aplica sprite se não estiver usando animator
            if (_bodyRenderer != null && _animator == null && stageConfig.SlimeSprite != null)
            {
                _bodyRenderer.sprite = stageConfig.SlimeSprite;
            }

            // Ajusta colliders de acordo com o tamanho
            float newSize = stageConfig.SizeMultiplier;
            UpdateColliders(newSize);

            // Ajusta escala de partículas
            UpdateParticleEffects(newSize);

            // Ajusta intensidade da luz, se houver
            UpdateLightIntensity(newSize);
        }

        /// <summary>
        /// Atualiza tamanho dos colliders baseado no novo tamanho
        /// </summary>
        private void UpdateColliders(float sizeMultiplier)
        {
            if (_physicsCollider != null)
            {
                _physicsCollider.radius = sizeMultiplier * _physicsColliderSizeMultiplier;
            }

            if (_interactionCollider != null)
            {
                _interactionCollider.radius = sizeMultiplier * _interactionColliderSizeMultiplier;
            }
        }

        /// <summary>
        /// Atualiza as partículas para escalar com o tamanho do slime
        /// </summary>
        private void UpdateParticleEffects(float sizeMultiplier)
        {
            if (_attachedParticleSystems == null) return;

            foreach (ParticleSystem ps in _attachedParticleSystems)
            {
                var main = ps.main;
                main.startSizeMultiplier = sizeMultiplier;

                // Escala emissão com tamanho
                var emission = ps.emission;
                if (emission.enabled)
                {
                    emission.rateOverTimeMultiplier *= sizeMultiplier;
                }
            }
        }

        /// <summary>
        /// Atualiza intensidade da luz baseado no tamanho
        /// </summary>
        private void UpdateLightIntensity(float sizeMultiplier)
        {
            if (_attachedLight != null)
            {
                _attachedLight.intensity = Mathf.Lerp(0.5f, 1.5f, sizeMultiplier / 2f);
                _attachedLight.pointLightOuterRadius *= sizeMultiplier;
            }
        }
    }
}
