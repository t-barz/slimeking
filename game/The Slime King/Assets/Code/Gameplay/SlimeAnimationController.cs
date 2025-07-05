using UnityEngine;
using System.Reflection;
using System.Collections;

namespace TheSlimeKing.Gameplay
{
    /// <summary>
    /// Gerencia os parâmetros do Animator para o Slime
    /// Compatível com URP 2D no Unity 6
    /// Versão refatorada para usar apenas um Animator
    /// </summary>
    public class SlimeAnimationController : MonoBehaviour
    {
        [Header("Referência - Animator Único")]
        [SerializeField] private Animator animator; [Header("Nomes de Parâmetros - Conforme Documento")]
        // Direção removida conforme solicitado - não deve existir no Animator
        // [SerializeField] private string directionParam = "Direction";   // 0=Front, 1=Back, 2=Side
        [SerializeField] private string facingRightParam = "FacingRight"; // True quando virado para direita
        [SerializeField] private string sleepingParam = "isSleeping";
        [SerializeField] private string hidingParam = "isHiding"; // Mantido para compatibilidade, use IsHidingHash
        [SerializeField] private string walkingParam = "isWalking";
        [SerializeField] private string shrinkTrigger = "Shrink";
        [SerializeField] private string jumpTrigger = "Jump";
        [SerializeField] private string attack01Trigger = "Attack01";
        [SerializeField] private string attack02Trigger = "Attack02";

        [Header("Configurações")]
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private float attackAnimationSpeedMultiplier = 1.0f;

        [Header("VFX - URP")]
        [SerializeField] private ParticleSystem attackVfx;
        [SerializeField] private ParticleSystem specialAttackVfx;
        [SerializeField] private MonoBehaviour attackPostProcessingEffect;
        [SerializeField, Range(0f, 1f)] private float attackEffectWeight = 0f;

        // Controle de cooldown
        private float _attack01Timer = 0f;
        private float _attack02Timer = 0f;

        // Estado atual
        private bool _isSleeping = false;
        private bool _isHiding = false;
        private bool _isWalking = false;
        private SlimeVisualController.SlimeDirection _currentDirection = SlimeVisualController.SlimeDirection.Front;
        private bool _isFacingRight = true;

        // Referência ao controlador visual
        private SlimeVisualController _visualController;


        private static readonly int IsHidingHash = Animator.StringToHash("isHiding");


        private void Awake()
        {
            // Busca o controlador visual no mesmo GameObject
            _visualController = GetComponent<SlimeVisualController>();
            if (_visualController == null)
                _visualController = GetComponentInParent<SlimeVisualController>();

            // Busca o Animator automaticamente se não for atribuído
            if (animator == null)
            {
                // Tenta buscar no mesmo GameObject
                animator = GetComponent<Animator>();

                // Busca nos filhos se não encontrar no GameObject atual
                if (animator == null)
                    animator = GetComponentInChildren<Animator>();
            }

            // Configura efeito personalizado de post-processing (se presente)
            attackEffectWeight = 0f;
            if (attackPostProcessingEffect != null)
            {
                attackPostProcessingEffect.enabled = false;
            }
        }

        private void Update()
        {
            // Atualiza timers de cooldown
            if (_attack01Timer > 0)
                _attack01Timer -= Time.deltaTime;

            if (_attack02Timer > 0)
                _attack02Timer -= Time.deltaTime;

            // Verificação periódica do estado isHiding no Animator
            if (animator != null)
            {
                bool currentHidingState = animator.GetBool(IsHidingHash);
                if (currentHidingState != _isHiding)
                {
                    Debug.LogWarning($"Inconsistência detectada: _isHiding={_isHiding}, mas animator.isHiding={currentHidingState}");
                }
            }

            // Fade out no efeito de post-processing para ataques
            if (attackEffectWeight > 0)
            {
                attackEffectWeight = Mathf.Max(0, attackEffectWeight - Time.deltaTime * 2f);

                // Atualiza o componente de efeito personalizado, se existir
                if (attackPostProcessingEffect != null)
                {
                    attackPostProcessingEffect.enabled = attackEffectWeight > 0;

                    // Para componentes que possuem uma propriedade weight, tenta usá-la via reflexão
                    var weightProperty = attackPostProcessingEffect.GetType().GetProperty("weight");
                    if (weightProperty != null)
                    {
                        weightProperty.SetValue(attackPostProcessingEffect, attackEffectWeight);
                    }
                }
            }
        }

        #region Métodos Públicos para animações

        /// <summary>
        /// Atualiza as animações com base na direção de movimento
        /// e sincroniza com SlimeVisualController
        /// </summary>
        public void UpdateMovementAnimation(Vector2 direction, bool isMoving)
        {
            _isWalking = isMoving;

            // Atualiza o estado de caminhada
            if (animator != null)
                animator.SetBool(walkingParam, _isWalking);

            // Se não está se movendo, não atualiza a direção
            if (!isMoving || direction.magnitude < 0.1f)
                return;

            // Obtém a direção visual do SlimeVisualController
            if (_visualController != null)
            {
                _visualController.UpdateDirection(direction);

                // Sincroniza os estados com o controlador visual
                _currentDirection = _visualController.GetCurrentDirection();
                _isFacingRight = _visualController.IsFacingRight();
            }            // Atualiza parâmetros do animator
            if (animator != null)
            {
                // Define apenas se está virado para direita ou esquerda
                animator.SetBool(facingRightParam, _isFacingRight);
            }
        }

        /// <summary>
        /// Executa animação de encolher
        /// </summary>
        public void PlayShrinkAnimation()
        {
            if (animator != null)
                animator.SetTrigger(shrinkTrigger);
        }

        /// <summary>
        /// Ativa a animação de esconder (agachar) do slime.
        /// </summary>
        public void PlayHideAnimation()
        {
            if (animator != null)
            {
                _isHiding = true;
                animator.SetBool(IsHidingHash, true);
                Debug.Log($"PlayHideAnimation: Set isHiding = true. Animator param value now: {animator.GetBool(IsHidingHash)}");
            }
            else
            {
                Debug.LogWarning("PlayHideAnimation: Animator reference is null!");
            }
        }

        /// <summary>
        /// Desativa a animação de esconder (agachar) do slime.
        /// </summary>
        public void StopHideAnimation()
        {
            if (animator != null)
            {
                Debug.Log($"[StopHideAnimation] Antes: animator.isHiding = {animator.GetBool(IsHidingHash)}");
                _isHiding = false;
                animator.SetBool(IsHidingHash, false);
                Debug.Log($"[StopHideAnimation] Depois: animator.isHiding = {animator.GetBool(IsHidingHash)}");

                // Inicia coroutine para forçar o parâmetro a ficar falso por vários frames
                StartCoroutine(ForceHideOff());
            }
            else
            {
                Debug.LogWarning("StopHideAnimation: Animator reference is null!");
            }
        }

        /// <summary>
        /// Executa animação de salto
        /// </summary>
        public void PlayJumpAnimation()
        {
            if (animator != null)
                animator.SetTrigger(jumpTrigger);
        }

        /// <summary>
        /// Executa animação de ataque básico (se não estiver em cooldown)
        /// </summary>
        public bool PlayAttack1Animation()
        {
            // Verifica se está em cooldown
            if (_attack01Timer <= 0)
            {
                if (animator != null)
                {
                    animator.SetTrigger(attack01Trigger);

                }

                // Ativa efeito de partículas para o ataque
                if (attackVfx != null)
                    attackVfx.Play();

                // Ativa efeito de post-processing
                attackEffectWeight = 0.5f;
                if (attackPostProcessingEffect != null)
                    attackPostProcessingEffect.enabled = true;

                // Inicia o cooldown
                _attack01Timer = attackCooldown;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Executa animação de ataque especial (se não estiver em cooldown)
        /// </summary>
        public bool PlayAttack2Animation()
        {
            // Verifica se está em cooldown
            if (_attack02Timer <= 0)
            {
                if (animator != null)
                {
                    animator.SetTrigger(attack02Trigger);

                    // Ajusta a velocidade da animação de ataque
                    animator.SetFloat("AttackSpeed", attackAnimationSpeedMultiplier);
                }

                // Ativa efeito de partículas para o ataque especial
                if (specialAttackVfx != null)
                    specialAttackVfx.Play();

                // Ativa efeito de post-processing com mais intensidade
                attackEffectWeight = 1.0f;
                if (attackPostProcessingEffect != null)
                    attackPostProcessingEffect.enabled = true;

                // Inicia o cooldown
                _attack02Timer = attackCooldown * 2f;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Define se o slime está dormindo
        /// </summary>
        public void SetSleeping(bool sleeping)
        {
            _isSleeping = sleeping;
            if (animator != null)
                animator.SetBool(sleepingParam, _isSleeping);
        }

        /// <summary>
        /// Define se o slime está escondido
        /// </summary>
        public void SetHiding(bool hiding)
        {
            _isHiding = hiding;
            if (animator != null)
                animator.SetBool(IsHidingHash, _isHiding); // Usando o hash para garantir consistência
        }

        /// <summary>
        /// Define se o slime está caminhando
        /// </summary>
        public void SetWalking(bool walking)
        {
            _isWalking = walking;
            if (animator != null)
                animator.SetBool(walkingParam, _isWalking);
        }

        /// <summary>
        /// Executa animação de crescer (oposto de encolher)
        /// Usado pelo sistema de Stealth para retornar ao tamanho normal
        /// </summary>
        public void PlayGrowAnimation()
        {
            // Como não existe um trigger específico para crescer,
            // desativa o estado de encolher/esconder
            if (animator != null)
            {
                animator.SetBool(IsHidingHash, false);
                _isHiding = false; // Atualiza também a variável interna para manter consistência
                Debug.Log("PlayGrowAnimation: Definindo isHiding = false");

                // Se houver um parâmetro específico para crescer no futuro, use-o aqui
                // animator.SetTrigger("Grow");
            }
        }

        /// <summary>
        /// Evento chamado pelo AnimationEvent ao final da animação de ataque
        /// </summary>
        public void OnAttackAnimationEnd()
        {
            // Notifica o SlimeMovement que a animação de ataque terminou
            SlimeMovement movement = GetComponent<SlimeMovement>();
            if (movement != null)
            {
                movement.EndAttack();
                Debug.Log("Fim da animação de ataque - liberando movimento");
            }
        }

        private IEnumerator ForceHideOff()
        {
            Debug.Log("[ForceHideOff] Iniciando forçamento do parâmetro isHiding = false");

            // Força o parâmetro a ficar falso por vários frames consecutivos
            for (int i = 0; i < 5; i++)
            {
                _isHiding = false;
                animator.SetBool(IsHidingHash, false);
                Debug.Log($"[ForceHideOff] Frame {i}: animator.isHiding = {animator.GetBool(IsHidingHash)}");
                yield return null; // Aguarda o próximo frame
            }

            Debug.Log("[ForceHideOff] Finalizado");
        }

        #endregion
    }
}
