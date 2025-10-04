using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Componente para rochas destrutíveis que podem receber múltiplos ataques antes de serem destruídas.
    /// 
    /// SISTEMA DE RESISTÊNCIA:
    /// - Compara ataque do player vs resistência da rocha
    /// - Se ataque ≤ resistência: VFX de falha, sem dano
    /// - Se ataque > resistência: VFX de acerto, calcula dano usando fórmula Ceil(ataque / (ataque - resistência))
    /// 
    /// FUNCIONALIDADES:
    /// - Sistema de múltiplos ataques com estados (normal → rachado → destruído)
    /// - VFX contextuais baseados no sucesso/falha do ataque
    /// - Integration com DropController para loot
    /// REQUISITOS EXTERNOS:
    /// • PlayerController com sistema de combate (tags e layers configurados)
    /// - Cache estático do PlayerAttributesSystem para performance
    /// • SFX configurados no AudioManager
    /// - Debug tools completos via Context Menu
    /// </summary>
    public class RockDestruct : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Rock Configuration")]
        [SerializeField] private int attacksToDestroy = 3;
        [Tooltip("Quantos ataques a rocha aguenta antes de ser completamente destruída")]

        [SerializeField] private int rockResistance = 5;
        [Tooltip("Resistência da rocha. Player precisa ter ataque maior para causar dano")]

        [Header("Visual Effects")]
        [SerializeField] private GameObject vfxHit;
        [Tooltip("Efeito visual quando o ataque é bem-sucedido")]

        [SerializeField] private GameObject vfxFailHit;
        [Tooltip("Efeito visual quando o ataque falha por resistência")]

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        #endregion

        #region Private Fields
        private Animator animator;
        private int currentAttacks = 0;
        private bool isCracked = false;
        private bool isDestroyed = false;

        // Cache para performance - evita múltiplas chamadas GetComponent
        private static readonly int CrackTriggerHash = Animator.StringToHash("Crack");
        private static readonly int DestroyTriggerHash = Animator.StringToHash("Destroy");

        // Cache do player para obter atributos de ataque
        private static PlayerAttributesSystem s_cachedPlayerAttributes;
        private static bool s_playerCacheInitialized = false;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            InitializeComponents();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            // Cache do componente Animator
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError($"RockDestruct: Nenhum Animator encontrado em {gameObject.name}. Adicione um Animator component.");
                enabled = false;
                return;
            }

            // Valida a configuração
            if (attacksToDestroy <= 0)
            {
                Debug.LogWarning($"RockDestruct: attacksToDestroy deve ser maior que 0 em {gameObject.name}. Definindo para 1.");
                attacksToDestroy = 1;
            }

            // Verifica se os triggers existem no Animator
            ValidateAnimatorTriggers();

            // Cache do player para obter atributos de ataque
            CachePlayerAttributes();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableDebugLogs)
                Debug.Log($"RockDestruct: Inicializado em {gameObject.name} - Ataques necessários: {attacksToDestroy}, Resistência: {rockResistance}");
#endif
        }

        private void CachePlayerAttributes()
        {
            if (!s_playerCacheInitialized)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    s_cachedPlayerAttributes = player.GetComponent<PlayerAttributesSystem>();
                    if (s_cachedPlayerAttributes == null)
                    {
                        Debug.LogWarning("RockDestruct: PlayerAttributesSystem não encontrado no Player. Sistema de resistência pode não funcionar corretamente.");
                    }
                }
                else
                {
                    Debug.LogWarning("RockDestruct: Player com tag 'Player' não encontrado. Sistema de resistência pode não funcionar corretamente.");
                }
                s_playerCacheInitialized = true;
            }
        }

        private void ValidateAnimatorTriggers()
        {
            if (animator == null) return;

            bool hasCrack = false;
            bool hasDestroy = false;

            // Verifica se os triggers existem no Animator Controller - otimizado para performance
            var parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].type == AnimatorControllerParameterType.Trigger)
                {
                    if (parameters[i].nameHash == CrackTriggerHash)
                        hasCrack = true;
                    else if (parameters[i].nameHash == DestroyTriggerHash)
                        hasDestroy = true;
                }

                // Early exit quando ambos triggers forem encontrados
                if (hasCrack && hasDestroy) break;
            }

            if (!hasCrack)
                Debug.LogWarning($"RockDestruct: Trigger 'Crack' não encontrado no Animator de {gameObject.name}");
            if (!hasDestroy)
                Debug.LogWarning($"RockDestruct: Trigger 'Destroy' não encontrado no Animator de {gameObject.name}");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Método público para receber dano de ataques.
        /// Agora considera resistência da rocha vs ataque do player.
        /// </summary>
        public void TakeDamage(Vector3? hitPosition = null)
        {
            if (isDestroyed) return;

            int playerAttack = GetPlayerAttack();
            Vector3 vfxPosition = hitPosition ?? transform.position;

            // Nova lógica: verificar se ataque supera resistência
            if (playerAttack <= rockResistance)
            {
                // Ataque muito fraco - VFX de falha sem dano
                PlayFailVFX(vfxPosition);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (enableDebugLogs)
                    Debug.Log($"RockDestruct: Ataque fraco ({playerAttack}) vs Resistência ({rockResistance}) - sem dano em {gameObject.name}");
#endif
                return;
            }

            // Ataque forte - calcular pontos de dano
            int damagePoints = CalculateDamagePoints(playerAttack, rockResistance);
            currentAttacks += damagePoints;

            PlayHitVFX(vfxPosition);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableDebugLogs)
                Debug.Log($"RockDestruct: Ataque ({playerAttack}) vs Resistência ({rockResistance}) = +{damagePoints} dano. Total: {currentAttacks}/{attacksToDestroy}");
#endif

            // Resto da lógica existente (crack e destroy)
            int attacksToeCrack = Mathf.CeilToInt(attacksToDestroy / 2f);

            if (!isCracked && currentAttacks >= attacksToeCrack)
            {
                isCracked = true;
                animator.SetTrigger(CrackTriggerHash);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (enableDebugLogs)
                    Debug.Log($"RockDestruct: Ativando trigger 'Crack' em {gameObject.name}");
#endif
            }

            if (currentAttacks >= attacksToDestroy)
            {
                isDestroyed = true;
                animator.SetTrigger(DestroyTriggerHash);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (enableDebugLogs)
                    Debug.Log($"RockDestruct: Ativando trigger 'Destroy' em {gameObject.name}");
#endif

                DropController dropController = GetComponent<DropController>();
                if (dropController != null)
                {
                    dropController.DropItems();
                }
            }
        }

        /// <summary>
        /// Sobrecarga para compatibilidade com código existente
        /// </summary>
        public void TakeDamage()
        {
            TakeDamage(null);
        }

        /// <summary>
        /// Destroi o GameObject onde este script está anexado.
        /// Método público que pode ser chamado por outros scripts ou eventos.
        /// </summary>
        public void DestroyObject()
        {
            Destroy(gameObject);
        }
        #endregion

        #region Damage Calculation
        /// <summary>
        /// Obtém o valor de ataque do player
        /// </summary>
        private int GetPlayerAttack()
        {
            if (s_cachedPlayerAttributes != null)
            {
                // Retorna o ataque atual do player
                return s_cachedPlayerAttributes.CurrentAttack;
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableDebugLogs)
                Debug.LogWarning("RockDestruct: PlayerAttributesSystem não encontrado. Usando ataque padrão (1).");
#endif
            return 1; // Fallback: ataque mínimo
        }

        /// <summary>
        /// Calcula pontos de dano baseado na fórmula: Ceil(ataque / (ataque - resistência))
        /// </summary>
        private int CalculateDamagePoints(int playerAttack, int rockResistance)
        {
            if (playerAttack <= rockResistance)
                return 0;

            float damage = (float)playerAttack / (playerAttack - rockResistance);
            return Mathf.CeilToInt(damage);
        }

        /// <summary>
        /// Instancia VFX de acerto bem-sucedido
        /// </summary>
        private void PlayHitVFX(Vector3 position)
        {
            if (vfxHit != null)
            {
                Instantiate(vfxHit, position, Quaternion.identity);
            }
        }

        /// <summary>
        /// Instancia VFX de ataque falhado por resistência
        /// </summary>
        private void PlayFailVFX(Vector3 position)
        {
            if (vfxFailHit != null)
            {
                Instantiate(vfxFailHit, position, Quaternion.identity);
            }
        }
        #endregion

        #region Public Properties (Read-Only)
        /// <summary>
        /// Retorna o número atual de ataques recebidos
        /// </summary>
        public int CurrentAttacks => currentAttacks;

        /// <summary>
        /// Retorna o número total de ataques necessários para destruir
        /// </summary>
        public int AttacksToDestroy => attacksToDestroy;

        /// <summary>
        /// Retorna se a rocha está no estado crackeado
        /// </summary>
        public bool IsCracked => isCracked;

        /// <summary>
        /// Retorna se a rocha está destruída
        /// </summary>
        public bool IsDestroyed => isDestroyed;

        /// <summary>
        /// Retorna a resistência da rocha
        /// </summary>
        public int RockResistance => rockResistance;

        /// <summary>
        /// Retorna o ataque atual do player (para debug)
        /// </summary>
        public int PlayerAttack => GetPlayerAttack();
        #endregion

        #region Context Menu (Editor Only)
        [ContextMenu("Test Single Attack")]
        private void TestSingleAttack()
        {
            TakeDamage();
        }

        [ContextMenu("Test Crack")]
        private void TestCrack()
        {
            int attacksToeCrack = Mathf.CeilToInt(attacksToDestroy / 2f);
            currentAttacks = attacksToeCrack - 1;
            TakeDamage();
        }

        [ContextMenu("Test Destroy")]
        private void TestDestroy()
        {
            currentAttacks = attacksToDestroy - 1;
            TakeDamage();
        }

        [ContextMenu("Reset Rock")]
        private void ResetRock()
        {
            currentAttacks = 0;
            isCracked = false;
            isDestroyed = false;
            Debug.Log($"RockDestruct: Rock {gameObject.name} resetado");
        }

        [ContextMenu("Debug Info")]
        private void DebugInfo()
        {
            Debug.Log($"=== RockDestruct Debug Info ===");
            Debug.Log($"GameObject: {gameObject.name}");
            Debug.Log($"Current Attacks: {currentAttacks}/{attacksToDestroy}");
            Debug.Log($"Attacks to Crack: {Mathf.CeilToInt(attacksToDestroy / 2f)}");
            Debug.Log($"Is Cracked: {isCracked}");
            Debug.Log($"Is Destroyed: {isDestroyed}");
            Debug.Log($"Animator: {(animator != null ? "Found" : "Missing")}");
            Debug.Log($"Rock Resistance: {rockResistance}");
            Debug.Log($"Player Attack: {GetPlayerAttack()}");
        }

        [ContextMenu("Test Weak Attack")]
        private void TestWeakAttack()
        {
            Debug.Log($"=== Testando Ataque Fraco ===");
            Debug.Log($"Simulando ataque com força 1 vs resistência {rockResistance}");

            // Backup do player cache
            var originalPlayer = s_cachedPlayerAttributes;

            // Cria player temporário com ataque fraco
            GameObject tempPlayer = new GameObject("TempPlayer");
            PlayerAttributesSystem tempAttributes = tempPlayer.AddComponent<PlayerAttributesSystem>();
            s_cachedPlayerAttributes = tempAttributes;

            // Simula ataque fraco
            TakeDamage(transform.position + Vector3.up);

            // Restaura cache original
            s_cachedPlayerAttributes = originalPlayer;
            DestroyImmediate(tempPlayer);
        }

        [ContextMenu("Test Strong Attack")]
        private void TestStrongAttack()
        {
            Debug.Log($"=== Testando Ataque Forte ===");
            int strongAttack = rockResistance + 5;
            Debug.Log($"Simulando ataque com força {strongAttack} vs resistência {rockResistance}");

            // Backup do player cache
            var originalPlayer = s_cachedPlayerAttributes;

            // TODO: Quando PlayerAttributesSystem permitir setar ataque, usar valor forte
            Debug.Log($"Nota: Usando ataque atual do player ({GetPlayerAttack()}) - implementar setter se necessário");

            TakeDamage(transform.position + Vector3.up);

            // Restaura cache original
            s_cachedPlayerAttributes = originalPlayer;
        }

        [ContextMenu("Debug Resistance Calculation")]
        private void DebugResistanceCalculation()
        {
            int playerAttack = GetPlayerAttack();
            Debug.Log($"=== Cálculo de Resistência ===");
            Debug.Log($"Player Attack: {playerAttack}");
            Debug.Log($"Rock Resistance: {rockResistance}");

            if (playerAttack <= rockResistance)
            {
                Debug.Log($"Resultado: ATAQUE FALHA (Attack {playerAttack} ≤ Resistance {rockResistance})");
                Debug.Log($"VFX: vfxFailHit será instanciado");
                Debug.Log($"Dano: 0 pontos");
            }
            else
            {
                int damagePoints = CalculateDamagePoints(playerAttack, rockResistance);
                Debug.Log($"Resultado: ATAQUE SUCESSO (Attack {playerAttack} > Resistance {rockResistance})");
                Debug.Log($"Fórmula: Ceil({playerAttack} / ({playerAttack} - {rockResistance})) = Ceil({playerAttack} / {playerAttack - rockResistance}) = {damagePoints}");
                Debug.Log($"VFX: vfxHit será instanciado");
                Debug.Log($"Dano: {damagePoints} pontos");
            }
        }

        [ContextMenu("Test Different Resistance Values")]
        private void TestDifferentResistanceValues()
        {
            int playerAttack = GetPlayerAttack();
            Debug.Log($"=== Teste de Diferentes Resistências (Player Attack: {playerAttack}) ===");

            for (int resistance = 1; resistance <= 10; resistance++)
            {
                if (playerAttack <= resistance)
                {
                    Debug.Log($"Resistência {resistance}: FALHA (sem dano)");
                }
                else
                {
                    float damage = (float)playerAttack / (playerAttack - resistance);
                    int damagePoints = Mathf.CeilToInt(damage);
                    Debug.Log($"Resistência {resistance}: SUCESSO - {damagePoints} pontos de dano");
                }
            }
        }
        #endregion
    }
}
