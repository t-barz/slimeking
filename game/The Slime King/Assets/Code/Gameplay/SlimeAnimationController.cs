using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

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

        [Header("Ataques - GameObjects")]
        [Tooltip("GameObject a ser instanciado durante o ataque básico")]
        [SerializeField] private GameObject attackPrefab;
        [Tooltip("GameObject a ser instanciado durante o ataque especial")]
        [SerializeField] private GameObject specialAttackPrefab;
        [Tooltip("Offset de posição para instanciar o objeto de ataque (relativo à posição do slime)")]
        [SerializeField] private Vector3 attackSpawnOffset = Vector3.zero;
        [Tooltip("Duração em segundos até desativar o objeto instanciado (0 = não desativar)")]
        [SerializeField] private float attackObjectDuration = 0.5f;

        [Header("Otimização - Object Pooling")]
        [Tooltip("Ativar o sistema de Object Pooling para melhorar a performance")]
        [SerializeField] private bool useObjectPooling = true;
        [Tooltip("Quantidade inicial de objetos de ataque básico no pool")]
        [SerializeField] private int attackPoolSize = 5;
        [Tooltip("Quantidade inicial de objetos de ataque especial no pool")]
        [SerializeField] private int specialAttackPoolSize = 3;

        [Header("Efeitos de Impacto")]
        [Tooltip("Prefab para o efeito visual de impacto quando um objeto recebe dano")]
        [SerializeField] private GameObject impactEffectPrefab;
        [Tooltip("Duração em segundos até desativar o efeito de impacto")]
        [SerializeField] private float impactEffectDuration = 0.3f;
        [Tooltip("Quantidade inicial de objetos de efeito de impacto no pool")]
        [SerializeField] private int impactEffectPoolSize = 10;

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

        // Pools de objetos para ataques
        private ObjectPool _attackObjectPool;
        private ObjectPool _specialAttackObjectPool;
        private ObjectPool _impactEffectPool;
        private Transform _poolContainer;

        private static readonly int IsHidingHash = Animator.StringToHash("isHiding");

        // Variável estática para acesso global
        private static SlimeAnimationController _instance;

        /// <summary>
        /// Retorna a instância estática do SlimeAnimationController
        /// </summary>
        /// <returns>A instância do SlimeAnimationController ou null se não existir</returns>
        public static SlimeAnimationController GetInstance()
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<SlimeAnimationController>();
            }
            return _instance;
        }

        /// <summary>
        /// Retorna o prefab configurado para efeitos de impacto
        /// </summary>
        /// <returns>O prefab de efeito de impacto ou null se não estiver configurado</returns>
        public GameObject GetImpactPrefab()
        {
            return impactEffectPrefab;
        }

        /// <summary>
        /// Cria um efeito de impacto simples diretamente sem usar o pool
        /// Utilizado como último recurso quando o sistema de pool falhar
        /// </summary>
        /// <param name="position">Posição onde criar o efeito</param>
        public static void CreateSimpleImpactEffect(Vector3 position)
        {
            if (_instance == null || _instance.impactEffectPrefab == null)
            {
                Debug.LogWarning("Não é possível criar efeito simples - instância ou prefab não disponível");
                return;
            }

            try
            {
                // Cria o objeto diretamente sem usar o pool
                GameObject effect = Instantiate(_instance.impactEffectPrefab, position, Quaternion.identity);

                // Configura tamanho e ordenação
                effect.transform.localScale = Vector3.one * 1.5f;
                ConfigureSortingOrderForEffect(effect);

                // Ativa partículas se houver
                ParticleSystem[] particleSystems = effect.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem ps in particleSystems)
                {
                    ps.Play(true);
                }

                // Destrói após a duração
                Destroy(effect, _instance.impactEffectDuration);

                Debug.Log($"Efeito simples criado na posição {position}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao criar efeito simples: {e.Message}");
            }
        }


        private void Awake()
        {
            // Define a instância estática para acesso global
            _instance = this;

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

            // Inicializa o sistema de object pooling se estiver ativado
            if (useObjectPooling)
            {
                // Verifica se o prefab de impacto está configurado
                if (impactEffectPrefab == null)
                {
                    Debug.LogError("SlimeAnimationController: Prefab de efeito de impacto não configurado! Configure-o no Inspector.");

                    // Tenta buscar um efeito padrão de Resources
                    impactEffectPrefab = Resources.Load<GameObject>("Effects/DefaultImpact");
                    if (impactEffectPrefab != null)
                    {
                        Debug.Log("Carregado prefab de efeito de impacto padrão dos Resources.");
                    }
                }

                InitializeObjectPools();

                // Verificação adicional após inicialização
                if (_impactEffectPool == null && impactEffectPrefab != null)
                {
                    Debug.LogError("Falha ao inicializar o pool de efeitos de impacto. Tentando novamente...");

                    // Tenta inicializar especificamente o pool de efeitos de impacto
                    Transform impactPoolParent = new GameObject("ImpactEffectPool").transform;
                    impactPoolParent.SetParent(_poolContainer != null ? _poolContainer : transform);
                    _impactEffectPool = new ObjectPool(impactEffectPrefab, impactEffectPoolSize, impactPoolParent);
                }
            }

            // Registra a instância estática
            _instance = this;

            // Log para confirmação
            Debug.Log($"SlimeAnimationController inicializado. Object pooling: {(useObjectPooling ? "Ativado" : "Desativado")}, " +
                      $"Pool de impacto: {(_impactEffectPool != null ? "Inicializado" : "Não inicializado")}, " +
                      $"Prefab de impacto: {(impactEffectPrefab != null ? impactEffectPrefab.name : "Não configurado")}");

            // Pré-aquece o método estático
            if (impactEffectPrefab != null)
            {
                Invoke("PrewarmImpactEffect", 0.5f);
            }
        }

        /// <summary>
        /// Inicializa os pools de objetos para os ataques e efeitos
        /// </summary>
        private void InitializeObjectPools()
        {
            // Cria um container para os objetos do pool
            _poolContainer = new GameObject("AttackObjectPools").transform;
            _poolContainer.SetParent(transform);
            _poolContainer.localPosition = Vector3.zero;

            // Inicializa o pool para objetos de ataque básico
            if (attackPrefab != null)
            {
                Transform basicPoolParent = new GameObject("BasicAttackPool").transform;
                basicPoolParent.SetParent(_poolContainer);
                _attackObjectPool = new ObjectPool(attackPrefab, attackPoolSize, basicPoolParent);
            }

            // Inicializa o pool para objetos de ataque especial
            if (specialAttackPrefab != null)
            {
                Transform specialPoolParent = new GameObject("SpecialAttackPool").transform;
                specialPoolParent.SetParent(_poolContainer);
                _specialAttackObjectPool = new ObjectPool(specialAttackPrefab, specialAttackPoolSize, specialPoolParent);
            }

            // Inicializa o pool para efeitos de impacto
            if (impactEffectPrefab != null)
            {
                Transform impactPoolParent = new GameObject("ImpactEffectPool").transform;
                impactPoolParent.SetParent(_poolContainer);
                _impactEffectPool = new ObjectPool(impactEffectPrefab, impactEffectPoolSize, impactPoolParent);

                // Verifica se o prefab tem os componentes visuais necessários
                bool hasRenderer = impactEffectPrefab.GetComponent<Renderer>() != null ||
                                   impactEffectPrefab.GetComponentInChildren<Renderer>() != null;
                bool hasParticles = impactEffectPrefab.GetComponent<ParticleSystem>() != null ||
                                   impactEffectPrefab.GetComponentInChildren<ParticleSystem>() != null;
                bool hasAnimator = impactEffectPrefab.GetComponent<Animator>() != null;

                if (!hasRenderer && !hasParticles && !hasAnimator)
                {
                    Debug.LogWarning("O prefab de efeito de impacto não possui componentes de renderização visíveis (Renderer, ParticleSystem ou Animator).");
                }

                Debug.Log($"Pool de efeitos de impacto inicializado com {impactEffectPoolSize} objetos. " +
                          $"Renderer: {(hasRenderer ? "Sim" : "Não")}, " +
                          $"Particles: {(hasParticles ? "Sim" : "Não")}, " +
                          $"Animator: {(hasAnimator ? "Sim" : "Não")}");
            }
            else
            {
                Debug.LogWarning("impactEffectPrefab não configurado. Efeitos de impacto não serão exibidos.");
            }
        }

        /// <summary>
        /// Cria um efeito visual de impacto na posição especificada
        /// </summary>
        /// <param name="position">Posição onde o efeito será exibido</param>
        public static void CreateImpactEffect(Vector3 position)
        {
            // Verificação da instância do SlimeAnimationController
            if (_instance == null)
            {
                Debug.LogError("CreateImpactEffect: Instância do SlimeAnimationController é null!");

                // Tenta encontrar uma instância ativa na cena se não houver referência
                _instance = FindAnyObjectByType<SlimeAnimationController>();

                if (_instance == null)
                {
                    Debug.LogError("CreateImpactEffect: Não foi possível encontrar uma instância de SlimeAnimationController na cena!");
                    return;
                }
            }

            // Verificação do prefab de efeito de impacto
            if (_instance.impactEffectPrefab == null)
            {
                Debug.LogError("CreateImpactEffect: Prefab de efeito de impacto não está configurado no SlimeAnimationController!");
                return;
            }

            GameObject impactEffect = null;

            // Tenta obter um objeto do pool
            if (_instance._impactEffectPool != null)
            {
                impactEffect = _instance._impactEffectPool.GetObject();
            }

            // Se não conseguiu obter do pool, instancia diretamente (fallback)
            if (impactEffect == null)
            {
                Debug.LogWarning("CreateImpactEffect: Usando instanciação direta como fallback (o pool pode não estar funcionando).");
                impactEffect = Instantiate(_instance.impactEffectPrefab);

                // Destrói o objeto após o tempo especificado
                Destroy(impactEffect, _instance.impactEffectDuration);
            }

            // Garante que o objeto existe
            if (impactEffect == null)
            {
                Debug.LogError("CreateImpactEffect: Falha ao criar efeito de impacto!");
                return;
            }

            // Ajusta a posição Z para garantir que fique visível na frente
            Vector3 adjustedPosition = new Vector3(position.x, position.y, position.z - 0.5f);
            impactEffect.transform.position = adjustedPosition;
            impactEffect.transform.rotation = Quaternion.identity;
            impactEffect.transform.localScale = Vector3.one * 1.5f; // Aumenta o tamanho para melhor visibilidade

            // Configura a ordem de camada para ficar na frente
            ConfigureSortingOrderForEffect(impactEffect);

            // Força o objeto a ficar ativo
            impactEffect.SetActive(true);

            // Registra a hierarquia do objeto para depuração
            Debug.Log($"Hierarquia do efeito de impacto:");
            foreach (Transform child in impactEffect.transform)
            {
                Debug.Log($"- Filho: {child.name}, Ativo: {child.gameObject.activeSelf}");

                // Verifica componentes de renderização nos filhos
                Renderer childRenderer = child.GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    Debug.Log($"  - Renderer: {childRenderer.enabled}, Visível: {childRenderer.isVisible}");
                }

                // Verifica sistemas de partículas nos filhos
                ParticleSystem childPS = child.GetComponent<ParticleSystem>();
                if (childPS != null)
                {
                    Debug.Log($"  - ParticleSystem: Ativo: {childPS.isPlaying}");
                }
            }

            // Ativa explicitamente todos os renderers
            Renderer[] renderers = impactEffect.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }

            // Ativa todos os sistemas de partículas
            ParticleSystem[] particleSystems = impactEffect.GetComponentsInChildren<ParticleSystem>(true);
            if (particleSystems.Length > 0)
            {
                foreach (ParticleSystem ps in particleSystems)
                {
                    ps.Stop(true);
                    ps.Clear(true);
                    ps.Play(true);
                }
                Debug.Log($"Ativados {particleSystems.Length} sistemas de partículas");
            }
            else
            {
                Debug.LogWarning("Nenhum ParticleSystem encontrado no efeito de impacto!");
            }

            // Reinicia qualquer animator
            Animator anim = impactEffect.GetComponent<Animator>();
            if (anim != null)
            {
                anim.enabled = true;
                anim.Rebind();
                anim.Update(0f);
            }

            // Verifica se há um MonoBehaviour no objeto para usar coroutines
            MonoBehaviour monoBehaviour = impactEffect.GetComponent<MonoBehaviour>();
            bool canUseCoroutine = (monoBehaviour != null);

            // Se estiver usando o pool e puder usar coroutine, programa o retorno através do pool
            if (_instance._impactEffectPool != null && canUseCoroutine)
            {
                _instance._impactEffectPool.ReturnObject(impactEffect, _instance.impactEffectDuration);
                Debug.Log($"Objeto programado para retornar ao pool após {_instance.impactEffectDuration} segundos");
            }
            else
            {
                // Usa a coroutine do próprio SlimeAnimationController ou Destroy diretamente
                if (_instance != null)
                {
                    _instance.StartCoroutine(_instance.DeactivateAndDestroy(impactEffect, _instance.impactEffectDuration));
                    Debug.Log("Usando coroutine do SlimeAnimationController para destruir o efeito");
                }
                else
                {
                    Debug.LogWarning("Usando Destroy direto como último recurso");
                    Destroy(impactEffect, _instance != null ? _instance.impactEffectDuration : 0.5f);
                }
            }

            Debug.Log($"Efeito de impacto criado na posição {adjustedPosition} (original: {position})");
        }

        /// <summary>
        /// Desativa um gameObject após um determinado tempo
        /// </summary>
        /// <param name="obj">Objeto a ser desativado</param>
        /// <param name="delay">Tempo em segundos até a desativação</param>
        /// <returns>IEnumerator para uso com Coroutines</returns>
        private IEnumerator DeactivateAfterDelay(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (obj != null && obj.activeSelf)
            {
                obj.SetActive(false);
            }
        }

        /// <summary>
        /// Desativa um gameObject e depois o destrói após um determinado tempo
        /// </summary>
        /// <param name="obj">Objeto a ser desativado e destruído</param>
        /// <param name="delay">Tempo em segundos até a desativação</param>
        /// <returns>IEnumerator para uso com Coroutines</returns>
        private IEnumerator DeactivateAndDestroy(GameObject obj, float delay)
        {
            Debug.Log($"DeactivateAndDestroy: Objeto {obj?.name} será destruído em {delay} segundos");

            // Espera o tempo especificado
            yield return new WaitForSeconds(delay);

            // Verifica se o objeto ainda existe antes de manipulá-lo
            if (obj != null)
            {
                Debug.Log($"DeactivateAndDestroy: Destruindo objeto {obj.name}");
                Destroy(obj);
            }
        }

        /// <summary>
        /// Configura a ordem de camada para o efeito de impacto ficar visível na frente
        /// </summary>
        /// <param name="effectObject">O objeto de efeito a ser configurado</param>
        private static void ConfigureSortingOrderForEffect(GameObject effectObject)
        {
            const int EFFECT_SORTING_ORDER = 500;  // Ordem de camada alta para ficar na frente

            // Ajusta SpriteRenderers
            SpriteRenderer[] spriteRenderers = effectObject.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                sr.sortingOrder = EFFECT_SORTING_ORDER;
                Debug.Log($"SpriteRenderer '{sr.name}' configurado com sortingOrder={sr.sortingOrder}");
            }

            // Ajusta Particle Systems
            ParticleSystemRenderer[] psRenderers = effectObject.GetComponentsInChildren<ParticleSystemRenderer>(true);
            foreach (ParticleSystemRenderer psr in psRenderers)
            {
                psr.sortingOrder = EFFECT_SORTING_ORDER;
                Debug.Log($"ParticleSystemRenderer '{psr.name}' configurado com sortingOrder={psr.sortingOrder}");
            }

            // Outros tipos de renderer que possam existir
            Renderer[] renderers = effectObject.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renderers)
            {
                if (!(r is SpriteRenderer) && !(r is ParticleSystemRenderer))
                {
                    r.sortingOrder = EFFECT_SORTING_ORDER;
                    Debug.Log($"Renderer '{r.name}' configurado com sortingOrder={r.sortingOrder}");
                }
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
        /// Executa animação de ataque básico e instancia um GameObject na posição do slime (se não estiver em cooldown)
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

                // Instancia o objeto de ataque usando o método auxiliar
                GameObject spawnedObject = InstantiateAttackObject();
                if (spawnedObject != null)
                {
                    // Registra para depuração
                    Debug.Log($"Objeto de ataque instanciado em {spawnedObject.transform.position}, direção: {_currentDirection}, facingRight: {_isFacingRight}");
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
        /// Executa animação de ataque especial e instancia um GameObject de ataque especial (se não estiver em cooldown)
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

                // Instancia o objeto de ataque especial usando o método auxiliar
                GameObject spawnedObject = InstantiateAttackObject(specialAttackPrefab);
                if (spawnedObject != null)
                {
                    // Registra para depuração
                    Debug.Log($"Objeto de ataque especial instanciado em {spawnedObject.transform.position}");

                    // Podemos aplicar configurações específicas ao objeto de ataque especial aqui
                    // Por exemplo, adicionar um componente ou configurar propriedades
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

        /// <summary>
        /// Obtém um GameObject do pool ou instancia um novo para o ataque na posição adequada
        /// </summary>
        /// <param name="prefab">O prefab a ser instanciado. Se null, usa o attackPrefab configurado.</param>
        /// <param name="customOffset">Offset personalizado. Se null, usa o attackSpawnOffset configurado.</param>
        /// <param name="duration">Duração até desativar/destruir o objeto. Se menor que zero, usa o attackObjectDuration configurado.</param>
        /// <returns>O GameObject instanciado ou null se falhar</returns>
        public GameObject InstantiateAttackObject(GameObject prefab = null, Vector3? customOffset = null, float duration = -1)
        {
            // Usa os valores padrão se não forem especificados
            GameObject objectToSpawn = prefab != null ? prefab : attackPrefab;
            Vector3 offset = customOffset.HasValue ? customOffset.Value : attackSpawnOffset;
            float objectDuration = duration >= 0 ? duration : attackObjectDuration;

            if (objectToSpawn == null)
                return null;

            // Calcula a posição baseada na direção atual do slime
            Vector3 spawnPosition = transform.position + offset;

            // Ajusta a posição com base na direção que o slime está olhando
            if (_currentDirection == SlimeVisualController.SlimeDirection.Side)
            {
                // Se estiver olhando para o lado, ajusta o offset X baseado na direção
                float directionMultiplier = _isFacingRight ? 1f : -1f;
                spawnPosition.x += Mathf.Abs(offset.x) * directionMultiplier;
            }
            else if (_currentDirection == SlimeVisualController.SlimeDirection.Back)
            {
                // Se estiver olhando para trás, ajusta o offset Y
                //spawnPosition.y += Mathf.Abs(offset.y);
            }
            else // Front (default)
            {
                // Se estiver olhando para frente, inverte o offset Y
                spawnPosition.y -= Mathf.Abs(offset.y);
            }

            GameObject attackObject;

            // Usa object pooling se ativado e se for um dos prefabs configurados
            if (useObjectPooling)
            {
                // Escolhe o pool apropriado com base no prefab
                ObjectPool targetPool = null;

                if (objectToSpawn == attackPrefab && _attackObjectPool != null)
                {
                    targetPool = _attackObjectPool;
                }
                else if (objectToSpawn == specialAttackPrefab && _specialAttackObjectPool != null)
                {
                    targetPool = _specialAttackObjectPool;
                }

                // Se encontrou um pool adequado, usa-o
                if (targetPool != null)
                {
                    attackObject = targetPool.GetObject();

                    // Configura o objeto obtido do pool
                    if (attackObject != null)
                    {
                        attackObject.transform.position = spawnPosition;
                        attackObject.SetActive(true);

                        // Programa o retorno do objeto para o pool
                        if (objectDuration > 0)
                        {
                            targetPool.ReturnObject(attackObject, objectDuration);
                        }
                    }
                }
                else
                {
                    // Se não existe pool para esse prefab, usa a instanciação tradicional
                    attackObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

                    // Destrói o objeto após o tempo definido
                    if (objectDuration > 0)
                    {
                        Destroy(attackObject, objectDuration);
                    }
                }
            }
            else
            {
                // Sem object pooling - instancia o objeto na posição calculada
                attackObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);

                // Destrói o objeto após o tempo definido
                if (objectDuration > 0)
                {
                    Destroy(attackObject, objectDuration);
                }
            }

            // Se algo deu errado na instanciação, retorna null
            if (attackObject == null)
                return null;

            // Garantimos que o objeto pai mantém rotação neutra e está ativo
            attackObject.transform.rotation = Quaternion.identity;

            // Configura a visibilidade dos subobjetos de ataque com base na direção do slime
            ConfigureAttackObjectVisibility(attackObject);

            // Forçar o objeto a ser visível (verificação adicional)
            if (!attackObject.activeSelf)
            {
                Debug.LogWarning($"Objeto de ataque estava inativo após configuração! Ativando manualmente.");
                attackObject.SetActive(true);
            }

            return attackObject;
        }

        /// <summary>
        /// Configura a visibilidade dos subobjetos de ataque baseado na direção do slime
        /// </summary>
        /// <param name="attackObject">O objeto de ataque principal que contém os subobjetos</param>
        private void ConfigureAttackObjectVisibility(GameObject attackObject)
        {
            if (attackObject == null)
            {
                Debug.LogError("ConfigureAttackObjectVisibility: objeto de ataque é null!");
                return;
            }

            // Verificação para depuração
            Debug.Log($"Configurando visibilidade para: {attackObject.name}, ativo: {attackObject.activeSelf}, direção: {_currentDirection}");

            // Busca pelos subobjetos específicos com caminho completo para depuração
            Transform attackFront = FindChildRecursively(attackObject.transform, "attack_front");
            Transform attackBack = FindChildRecursively(attackObject.transform, "attack_back");
            Transform attackSide = FindChildRecursively(attackObject.transform, "attack_side");

            // Registra o resultado da busca para depuração
            Debug.Log($"Subobjetos encontrados - front: {(attackFront != null ? "Sim" : "Não")}, " +
                      $"back: {(attackBack != null ? "Sim" : "Não")}, side: {(attackSide != null ? "Sim" : "Não")}");

            // Lista os filhos do objeto para depuração
            string childrenNames = "";
            foreach (Transform child in attackObject.transform)
            {
                childrenNames += child.name + ", ";
            }
            Debug.Log($"Filhos do objeto de ataque: {childrenNames}");

            bool foundAnyAttackObjects = (attackFront != null || attackBack != null || attackSide != null);

            // Se não encontrou nenhum dos subobjetos específicos, pode ser um prefab com estrutura diferente
            if (!foundAnyAttackObjects)
            {
                Debug.LogWarning("Não foram encontrados subobjetos attack_front, attack_back ou attack_side no objeto de ataque");
                // No caso de não encontrar nenhum subobjeto específico, não modificamos a visibilidade
                // Isso pode acontecer se o prefab tiver uma estrutura diferente
                return;
            }

            // IMPORTANTE: Não desativamos todos os filhos, apenas os que controlamos especificamente
            // Desativar outros objetos como partículas ou componentes pode causar problemas
            if (attackFront != null) attackFront.gameObject.SetActive(false);
            if (attackBack != null) attackBack.gameObject.SetActive(false);
            if (attackSide != null) attackSide.gameObject.SetActive(false);

            // Ativa apenas o objeto correspondente à direção atual
            if (_currentDirection == SlimeVisualController.SlimeDirection.Front)
            {
                if (attackFront != null)
                {
                    attackFront.gameObject.SetActive(true);
                    Debug.Log($"Ativando attack_front: {attackFront.gameObject.activeSelf}");
                }
                else
                {
                    Debug.LogWarning("Subobjeto attack_front não encontrado, mas direção do slime é Front");
                }
            }
            else if (_currentDirection == SlimeVisualController.SlimeDirection.Back)
            {
                if (attackBack != null)
                {
                    attackBack.gameObject.SetActive(true);
                    Debug.Log($"Ativando attack_back: {attackBack.gameObject.activeSelf}");
                }
                else
                {
                    Debug.LogWarning("Subobjeto attack_back não encontrado, mas direção do slime é Back");
                }
            }
            else if (_currentDirection == SlimeVisualController.SlimeDirection.Side)
            {
                if (attackSide != null)
                {
                    attackSide.gameObject.SetActive(true);

                    // Configura o flip do attack_side com base na direção do slime
                    Vector3 sideScale = attackSide.localScale;
                    // Garante que o flip é aplicado corretamente
                    // Se _isFacingRight for true, a escala X deve ser positiva (olhando para direita)
                    // Se _isFacingRight for false, a escala X deve ser negativa (olhando para esquerda/espelhado)
                    sideScale.x = _isFacingRight ? -Mathf.Abs(sideScale.x) : Mathf.Abs(sideScale.x);
                    attackSide.localScale = sideScale;

                    Debug.Log($"Ativando attack_side com escala X: {sideScale.x} (facingRight: {_isFacingRight}), ativo: {attackSide.gameObject.activeSelf}");
                }
                else
                {
                    Debug.LogWarning("Subobjeto attack_side não encontrado, mas direção do slime é Side");
                }
            }

            // Verificação final de sanidade
            bool anyActive = (attackFront != null && attackFront.gameObject.activeSelf) ||
                            (attackBack != null && attackBack.gameObject.activeSelf) ||
                            (attackSide != null && attackSide.gameObject.activeSelf);

            if (!anyActive)
            {
                Debug.LogWarning("Nenhum subobjeto de ataque foi ativado! Verificar hierarquia do prefab.");
            }
        }

        /// <summary>
        /// Encontra um filho pelo nome recursivamente na hierarquia de objetos
        /// </summary>
        private Transform FindChildRecursively(Transform parent, string childName)
        {
            if (parent == null)
            {
                Debug.LogError("FindChildRecursively: parent é null!");
                return null;
            }

            if (string.IsNullOrEmpty(childName))
            {
                Debug.LogError("FindChildRecursively: childName é null ou vazio!");
                return null;
            }

            // Verifica se o nome do próprio pai corresponde
            if (parent.name.Equals(childName, System.StringComparison.OrdinalIgnoreCase))
            {
                return parent;
            }

            // Verifica diretamente no objeto pai (busca case-sensitive padrão)
            Transform directChild = parent.Find(childName);
            if (directChild != null)
            {
                Debug.Log($"Encontrado diretamente: {childName} em {parent.name}");
                return directChild;
            }

            // Tenta uma busca insensível a maiúsculas/minúsculas entre os filhos diretos
            foreach (Transform child in parent)
            {
                if (child.name.Equals(childName, System.StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log($"Encontrado (case-insensitive): {childName} como {child.name}");
                    return child;
                }
            }

            // Busca recursivamente em todos os filhos
            foreach (Transform child in parent)
            {
                Transform found = FindChildRecursively(child, childName);
                if (found != null)
                    return found;
            }

            return null;
        }

        /// <summary>
        /// Classe para gerenciar um pool de objetos reutilizáveis
        /// </summary>
        private class ObjectPool
        {
            private GameObject prefab;
            private List<GameObject> pooledObjects;
            private Transform poolParent;
            private int initialPoolSize;

            public ObjectPool(GameObject prefabToPool, int initialSize, Transform parent)
            {
                if (prefabToPool == null)
                {
                    Debug.LogWarning("Tentativa de criar um Object Pool com prefab null");
                    return;
                }

                prefab = prefabToPool;
                initialPoolSize = initialSize;
                poolParent = parent;
                pooledObjects = new List<GameObject>(initialSize);

                // Preenche o pool inicialmente
                for (int i = 0; i < initialSize; i++)
                {
                    CreateNewPooledObject();
                }
            }

            /// <summary>
            /// Cria um novo objeto para o pool
            /// </summary>
            private GameObject CreateNewPooledObject()
            {
                GameObject newObject = Object.Instantiate(prefab, poolParent);
                newObject.SetActive(false);
                pooledObjects.Add(newObject);
                return newObject;
            }

            /// <summary>
            /// Obtém um objeto do pool ou cria um novo se necessário
            /// </summary>
            public GameObject GetObject()
            {
                // Verifica se o prefab é válido
                if (prefab == null)
                    return null;

                GameObject pooledObject = null;

                // Procura por um objeto inativo no pool
                for (int i = 0; i < pooledObjects.Count; i++)
                {
                    if (pooledObjects[i] != null && !pooledObjects[i].activeInHierarchy)
                    {
                        pooledObject = pooledObjects[i];
                        break;
                    }
                }

                // Se não encontrar nenhum objeto disponível, cria um novo
                if (pooledObject == null)
                {
                    pooledObject = CreateNewPooledObject();
                    Debug.Log($"Object Pool: Criando novo objeto para o pool pois nenhum estava disponível");
                }

                // Resetar o objeto para o estado inicial antes de retorná-lo
                ResetObjectToOriginalState(pooledObject);

                Debug.Log($"Object Pool: Obtendo objeto {pooledObject.name} do pool");
                return pooledObject;
            }

            /// <summary>
            /// Reseta um objeto do pool para seu estado original
            /// </summary>
            private void ResetObjectToOriginalState(GameObject pooledObject)
            {
                if (pooledObject == null) return;

                // Resetar a transformação do objeto
                pooledObject.transform.localPosition = Vector3.zero;
                pooledObject.transform.localRotation = Quaternion.identity;
                pooledObject.transform.localScale = Vector3.one;

                // Se o objeto tiver um componente Renderer, garantir que está visível
                Renderer renderer = pooledObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;
                }

                // Verificar também renderers nos filhos
                Renderer[] childRenderers = pooledObject.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer childRenderer in childRenderers)
                {
                    childRenderer.enabled = true;
                }

                // Garante que todos os sistemas de partículas sejam redefinidos
                ParticleSystem[] particleSystems = pooledObject.GetComponentsInChildren<ParticleSystem>(true);
                foreach (ParticleSystem ps in particleSystems)
                {
                    ps.Stop(true);
                    ps.Clear(true);
                    ps.time = 0f;
                }

                // Reset any animator if present
                Animator animator = pooledObject.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Rebind();
                    animator.Update(0f);
                }

                // Desative qualquer corrotina ou comportamento contínuo que possa estar em execução
                MonoBehaviour[] behaviors = pooledObject.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour behavior in behaviors)
                {
                    behavior.StopAllCoroutines();
                }
            }

            /// <summary>
            /// Retorna um objeto para o pool (desativa em vez de destruir)
            /// </summary>
            public void ReturnObject(GameObject objectToReturn, float delay = 0f)
            {
                if (objectToReturn == null)
                    return;

                // Se houver delay, usa uma coroutine
                if (delay > 0)
                {
                    MonoBehaviour mb = objectToReturn.GetComponent<MonoBehaviour>();
                    if (mb != null)
                    {
                        mb.StartCoroutine(ReturnObjectAfterDelay(objectToReturn, delay));
                    }
                    else
                    {
                        Debug.LogWarning("Não foi possível iniciar coroutine para delay - desativando objeto imediatamente");
                        objectToReturn.SetActive(false);
                    }
                }
                else
                {
                    objectToReturn.SetActive(false);
                }
            }

            private IEnumerator ReturnObjectAfterDelay(GameObject objectToReturn, float delay)
            {
                yield return new WaitForSeconds(delay);
                if (objectToReturn != null)
                    objectToReturn.SetActive(false);
            }

            /// <summary>
            /// Limpa o pool e destrói todos os objetos
            /// </summary>
            public void ClearPool()
            {
                foreach (var obj in pooledObjects)
                {
                    if (obj != null)
                        Object.Destroy(obj);
                }
                pooledObjects.Clear();
            }
        }
        #endregion

        private void OnDestroy()
        {
            Debug.Log("SlimeAnimationController sendo destruído, limpando pools de objetos...");

            // Verifica se esta é a instância registrada
            if (_instance == this)
            {
                _instance = null;
                Debug.Log("Instância estática do SlimeAnimationController foi removida");
            }

            // Limpa os pools ao destruir o controlador
            if (useObjectPooling)
            {
                if (_attackObjectPool != null)
                {
                    _attackObjectPool.ClearPool();
                    Debug.Log("Pool de objetos de ataque básico limpo");
                }

                if (_specialAttackObjectPool != null)
                {
                    _specialAttackObjectPool.ClearPool();
                    Debug.Log("Pool de objetos de ataque especial limpo");
                }

                if (_impactEffectPool != null)
                {
                    _impactEffectPool.ClearPool();
                    Debug.Log("Pool de efeitos de impacto limpo");
                }

                if (_poolContainer != null)
                {
                    Destroy(_poolContainer.gameObject);
                    Debug.Log("Container de pools destruído");
                }
            }
        }

        /// <summary>
        /// Método para pré-aquecer o efeito de impacto
        /// </summary>
        private void PrewarmImpactEffect()
        {
            if (impactEffectPrefab != null)
            {
                // Cria e imediatamente esconde um efeito de impacto para garantir que tudo esteja carregado
                Vector3 hiddenPosition = new Vector3(1000, 1000, 1000); // Fora da visão
                GameObject effect = CreateSimpleImpactEffectInternal(hiddenPosition);
                if (effect != null)
                {
                    effect.SetActive(false);
                    Destroy(effect, 0.1f);
                    Debug.Log("Sistema de efeitos de impacto pré-aquecido com sucesso");
                }
            }
        }

        /// <summary>
        /// Versão interna do CreateSimpleImpactEffect que retorna o GameObject criado
        /// </summary>
        private static GameObject CreateSimpleImpactEffectInternal(Vector3 position)
        {
            if (_instance == null || _instance.impactEffectPrefab == null)
                return null;

            try
            {
                GameObject effect = Instantiate(_instance.impactEffectPrefab, position, Quaternion.identity);
                effect.transform.localScale = Vector3.one * 1.5f;
                ConfigureSortingOrderForEffect(effect);

                ParticleSystem[] particleSystems = effect.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem ps in particleSystems)
                {
                    ps.Play(true);
                }

                return effect;
            }
            catch
            {
                return null;
            }
        }
    }
}
