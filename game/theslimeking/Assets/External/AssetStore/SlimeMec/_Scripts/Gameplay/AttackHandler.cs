using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Direções específicas para ajuste dinâmico do offset de ataque
    /// </summary>
    public enum AttackDirection
    {
        South,  // Sul - para baixo
        North,  // Norte - para cima  
        East,   // Leste - para direita
        West    // Oeste - para esquerda
    }

    /// <summary>
    /// Detecta objetos com tag "Destructable" e chama TakeDamage() neles durante ataques.
    /// Suporta objetos com componentes BushDestruct e RockDestruct.
    /// 
    /// SISTEMA DE DETECÇÃO RETANGULAR:
    /// • Usa Physics2D.OverlapBox para área de ataque em formato retangular
    /// • Permite configurar largura e altura independentemente (attackSize)
    /// • Mais preciso que círculo para ataques direcionais (espadas, machados, etc.)
    /// • Melhor para ataques em linha reta ou áreas específicas
    /// • Inversão automática de dimensões para ataques laterais vs frontais
    /// 
    /// SISTEMA DE COMPONENTES DESTRUTÍVEIS:
    /// • BushDestruct: Para moitas e objetos que são destruídos em 1 hit
    /// • RockDestruct: Para rochas e objetos que requerem múltiplos hits
    /// • Cache separado para cada tipo de componente (performance otimizada)
    /// • Detecção automática do tipo de componente por objeto
    /// 
    /// SISTEMA DE OFFSET DINÂMICO:
    /// • attackOffset permite posicionar a área de detecção relativa ao transform
    /// • Offset ajusta automaticamente baseado na direção do ataque (Sul/Norte/Leste/Oeste)
    /// • Regras de transformação:
    ///   - Sul: offset original (referência)
    ///   - Norte: inverte Y (Sul → Norte multiplica Y por -1)
    ///   - Leste: troca X e Y (vertical → horizontal)
    ///   - Oeste: troca X e Y + inverte X (Leste → Oeste multiplica X por -1)
    /// • Offset é aplicado tanto na detecção quanto na visualização dos gizmos
    /// • Coordenadas locais: X (direita/esquerda), Y (cima/baixo)
    /// 
    /// VANTAGENS DA DETECÇÃO RETANGULAR:
    /// • Controle preciso da área de alcance horizontal vs vertical
    /// • Melhor para armas alongadas (espadas, lanças, etc.)
    /// • Evita hits acidentais em objetos muito acima/abaixo do jogador
    /// • Adaptação automática baseada na direção do ataque
    /// 
    /// VISUALIZAÇÃO NO EDITOR:
    /// • Gizmos retangulares mostram a área efetiva de ataque
    /// • Verde no modo normal, vermelho quando selecionado
    /// • Mostra dimensões adaptadas (horizontal vs vertical)
    /// </summary>
    public class AttackHandler : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Attack Detection Settings")]
        [SerializeField] private Vector2 attackSize = new Vector2(2f, 1.5f); // Tamanho da área de ataque (largura x altura)
        [SerializeField] private Vector2 attackOffset = Vector2.zero; // Offset da área de detecção em relação ao transform
        [SerializeField] private LayerMask destructableLayerMask = -1; // Layers dos objetos destrutíveis

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool showDebugGizmos = true;
        #endregion

        #region Private Fields
        // Cache para performance
        private readonly Collider2D[] colliderCache = new Collider2D[8]; // Cache fixo para evitar allocations

        // Estado atual do ataque
        private bool _lastAttackWasSideways = false; // Para inversão de tamanho
        private AttackDirection _currentDirection = AttackDirection.South; // Direção atual para ajuste de offset

        // Cache de componentes para evitar GetComponent repetidos
        private readonly System.Collections.Generic.Dictionary<Collider2D, BushDestruct> bushCache =
            new System.Collections.Generic.Dictionary<Collider2D, BushDestruct>();
        private readonly System.Collections.Generic.Dictionary<Collider2D, RockDestruct> rockCache =
            new System.Collections.Generic.Dictionary<Collider2D, RockDestruct>();

        // Propriedade para calcular centro da área de ataque com offset dinâmico
        private Vector2 AttackCenter => (Vector2)transform.position + GetDirectionalOffset();

        // Hash da tag para performance
        private static readonly int DestructableTagHash = "Destructable".GetHashCode();
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableDebugLogs)
                Debug.Log($"AttackHandler: Inicializado em {gameObject.name} com área de ataque {attackSize}");
#endif
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Calcula o offset da área de detecção baseado na direção atual do ataque.
        /// Aplica as transformações necessárias conforme as regras:
        /// • Vertical ↔ Horizontal: inverte X e Y
        /// • Sul → Norte: multiplica Y por -1  
        /// • Leste → Oeste: multiplica X por -1
        /// </summary>
        private Vector2 GetDirectionalOffset()
        {
            Vector2 adjustedOffset = attackOffset;

            switch (_currentDirection)
            {
                case AttackDirection.South:
                    // Mantém offset original (referência)
                    break;

                case AttackDirection.North:
                    // Sul → Norte: inverte Y
                    adjustedOffset.y = -attackOffset.y;
                    break;

                case AttackDirection.East:
                    // Vertical → Horizontal: troca X e Y
                    adjustedOffset = new Vector2(-attackOffset.y, attackOffset.x);
                    break;

                case AttackDirection.West:
                    // Vertical → Horizontal + Leste → Oeste: troca X/Y e inverte X
                    adjustedOffset = new Vector2(attackOffset.y, attackOffset.x);
                    break;
            }

            return adjustedOffset;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Executa o ataque, detectando e danificando objetos destrutíveis.
        /// Sistema otimizado com cache de componentes para evitar GetComponent repetidos.
        /// Usa detecção retangular para área de ataque mais precisa.
        /// </summary>
        /// <param name="isAttackingSideways">Se verdadeiro, inverte largura e altura da área de ataque</param>
        public void PerformAttack(bool isAttackingSideways = false)
        {
            // Mantém compatibilidade com código existente - usa direção padrão Sul
            PerformAttack(isAttackingSideways, AttackDirection.South);
        }

        /// <summary>
        /// Executa o ataque com direção específica para ajuste dinâmico do offset.
        /// Sistema otimizado com cache de componentes para evitar GetComponent repetidos.
        /// Usa detecção retangular para área de ataque mais precisa.
        /// </summary>
        /// <param name="isAttackingSideways">Se verdadeiro, inverte largura e altura da área de ataque</param>
        /// <param name="direction">Direção específica para ajuste do offset</param>
        public void PerformAttack(bool isAttackingSideways, AttackDirection direction)
        {
            // Atualiza a direção atual para cálculo do offset
            _currentDirection = direction;

            // Armazena o estado para inversão de tamanho
            _lastAttackWasSideways = isAttackingSideways;

            // Calcula o tamanho da área de ataque baseado na direção
            Vector2 effectiveAttackSize = CalculateEffectiveAttackSize(isAttackingSideways);

            // Usa a posição do transform mais o offset dinâmico como centro do ataque
            Vector2 attackCenter = AttackCenter;

            // Usa ContactFilter2D para detecção otimizada
            var filter = new ContactFilter2D();
            filter.SetLayerMask(destructableLayerMask);
            filter.useTriggers = true;

            int hitCount = Physics2D.OverlapBox(attackCenter, effectiveAttackSize, 0f, filter, colliderCache);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableDebugLogs)
            {
                string attackType = isAttackingSideways ? "lateral" : "frontal";
                string directionName = direction.ToString();
                Debug.Log($"AttackHandler: Ataque {attackType} executado na direção {directionName}, área {effectiveAttackSize}, offset {GetDirectionalOffset()}, {hitCount} objetos detectados");
            }
#endif

            // Early exit se não há detecções
            if (hitCount == 0) return;

            // Rastreia GameObjects já processados para evitar dano duplo
            var processedGameObjects = new System.Collections.Generic.HashSet<GameObject>();

            // Processa apenas os colliders detectados
            for (int i = 0; i < hitCount; i++)
            {
                Collider2D col = colliderCache[i];
                if (col.gameObject == gameObject) continue; // Ignora o próprio atacante

                // Pula se já processou este GameObject
                if (processedGameObjects.Contains(col.gameObject)) continue;
                processedGameObjects.Add(col.gameObject);

                bool damageDealt = false;

                // Verifica se é um NPC (NPCBaseController) PRIMEIRO
                var npcController = col.GetComponent<TheSlimeKing.NPCs.NPCBaseController>();
                if (npcController != null)
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    if (enableDebugLogs)
                        Debug.Log($"AttackHandler: Causando dano em NPC '{col.name}'");
#endif
                    npcController.TakeDamage();
                    damageDealt = true;
                }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                // Log de debug para investigar tag
                if (enableDebugLogs && !damageDealt)
                {
                    Debug.Log($"AttackHandler: Objeto '{col.name}' tem tag '{col.tag}', NPCBaseController: {(npcController != null ? "SIM" : "NÃO")}");
                }
#endif

                // Verifica se tem a tag "Destructable" ou "Enemy" (só se ainda não causou dano)
                if (!damageDealt && (col.CompareTag("Destructable") || col.CompareTag("Enemy")))
                {
                    // Tenta causar dano em BushDestruct
                    if (!bushCache.TryGetValue(col, out BushDestruct bushDestructable))
                    {
                        bushDestructable = col.GetComponent<BushDestruct>();
                        if (bushDestructable != null)
                        {
                            bushCache[col] = bushDestructable;
                        }
                    }

                    if (bushDestructable != null)
                    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        if (enableDebugLogs)
                            Debug.Log($"AttackHandler: Causando dano em moita '{col.name}'");
#endif
                        bushDestructable.TakeDamage();
                        damageDealt = true;
                    }

                    // Tenta causar dano em RockDestruct se não encontrou BushDestruct
                    if (!damageDealt)
                    {
                        if (!rockCache.TryGetValue(col, out RockDestruct rockDestructable))
                        {
                            rockDestructable = col.GetComponent<RockDestruct>();
                            if (rockDestructable != null)
                            {
                                rockCache[col] = rockDestructable;
                            }
                        }

                        if (rockDestructable != null)
                        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                            if (enableDebugLogs)
                                Debug.Log($"AttackHandler: Causando dano em rocha '{col.name}'");
#endif
                            // Passa o centro do collider para posicionamento preciso do VFX
                            rockDestructable.TakeDamage(col.bounds.center);
                            damageDealt = true;
                        }
                    }

                    // Log de aviso se não encontrou nenhum componente destrutível
                    if (!damageDealt)
                    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        if (enableDebugLogs)
                            Debug.LogWarning($"AttackHandler: Objeto '{col.name}' tem tag 'Destructable' mas não tem componente BushDestruct nem RockDestruct");
#endif
                    }
                }
            }
        }

        /// <summary>
        /// Calcula o tamanho efetivo da área de ataque baseado na direção.
        /// Para ataques laterais, inverte largura e altura para criar área vertical.
        /// </summary>
        /// <param name="isAttackingSideways">Se verdadeiro, inverte largura e altura</param>
        /// <returns>Tamanho efetivo da área de ataque</returns>
        private Vector2 CalculateEffectiveAttackSize(bool isAttackingSideways)
        {
            if (isAttackingSideways)
            {
                // Inverte largura e altura para ataques laterais
                return new Vector2(attackSize.y, attackSize.x);
            }

            // Retorna tamanho normal para ataques frontais/traseiros
            return attackSize;
        }

        /// <summary>
        /// Limpa o cache de componentes para liberar memória.
        /// Deve ser chamado quando objetos são destruídos ou a cena muda.
        /// </summary>
        public void ClearComponentCache()
        {
            bushCache.Clear();
            rockCache.Clear();
        }
        #endregion

        #region Gizmos (Editor Only)
        private void OnDrawGizmos()
        {
            if (!showDebugGizmos) return;

            Gizmos.color = Color.green;
            DrawAttackAreaGizmo();
        }

        private void OnDrawGizmosSelected()
        {
            if (!showDebugGizmos) return;

            Gizmos.color = Color.red;
            DrawAttackAreaGizmo();
        }

        private void DrawAttackAreaGizmo()
        {
            // Calcula tamanho efetivo baseado no último ataque
            Vector2 effectiveSize = CalculateEffectiveAttackSize(_lastAttackWasSideways);
            Vector3 center = AttackCenter; // Usa a propriedade que inclui o offset

            // Desenha retângulo wireframe da área de ataque
            DrawWireframeBox(center, effectiveSize);
        }

        private void DrawWireframeBox(Vector3 center, Vector2 size)
        {
            float halfWidth = size.x * 0.5f;
            float halfHeight = size.y * 0.5f;

            // Define os 4 cantos do retângulo
            Vector3[] corners = new Vector3[4];
            corners[0] = center + new Vector3(-halfWidth, -halfHeight, 0); // Inferior esquerdo
            corners[1] = center + new Vector3(halfWidth, -halfHeight, 0);  // Inferior direito
            corners[2] = center + new Vector3(halfWidth, halfHeight, 0);   // Superior direito
            corners[3] = center + new Vector3(-halfWidth, halfHeight, 0);  // Superior esquerdo

            // Desenha as 4 linhas do retângulo
            Gizmos.DrawLine(corners[0], corners[1]); // Inferior
            Gizmos.DrawLine(corners[1], corners[2]); // Direita
            Gizmos.DrawLine(corners[2], corners[3]); // Superior
            Gizmos.DrawLine(corners[3], corners[0]); // Esquerda
        }
        #endregion

        #region Context Menu (Editor Only)
        [ContextMenu("Debug Info")]
        private void DebugInfo()
        {
            Debug.Log($"=== AttackHandler Debug Info ===");
            Debug.Log($"GameObject: {gameObject.name}");
            Debug.Log($"Position: {transform.position}");
            Debug.Log($"Attack Size: {attackSize}");
            Debug.Log($"Last Attack Sideways: {_lastAttackWasSideways}");
            Debug.Log($"Effective Size: {CalculateEffectiveAttackSize(_lastAttackWasSideways)}");
            Debug.Log($"Layer Mask: {destructableLayerMask}");

            // Lista objetos detectados atualmente
            var filter = new ContactFilter2D();
            filter.SetLayerMask(destructableLayerMask);
            filter.useTriggers = true;

            int hitCount = Physics2D.OverlapBox(transform.position, attackSize, 0f, filter, colliderCache);
            Debug.Log($"Objetos destrutíveis detectados: {hitCount}");

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D col = colliderCache[i];
                if (col.gameObject != gameObject)
                {
                    bool hasDestructableTag = col.CompareTag("Destructable");
                    bool hasBushDestruct = col.GetComponent<BushDestruct>() != null;
                    bool hasRockDestruct = col.GetComponent<RockDestruct>() != null;
                    string destructType = hasBushDestruct ? "Bush" : (hasRockDestruct ? "Rock" : "None");
                    Debug.Log($"  - {col.name} (Tag: {col.tag}, IsDestructable: {hasDestructableTag}, Type: {destructType})");
                }
            }
        }
        #endregion
    }
}
