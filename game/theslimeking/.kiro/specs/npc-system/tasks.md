# Implementation Plan - Sistema de NPCs

## Nota sobre Sistema Existente

O projeto já possui um sistema básico de NPCs em `Assets/Code/Gameplay/NPCs/` com componentes como NPCController, NPCBehavior, NPCFriendship e NPCDialogue. Este plano de implementação irá criar um novo sistema baseado em Behavior Graph conforme especificado nos requisitos, que coexistirá ou substituirá o sistema atual.

- [x] 1. Criar estrutura base de ScriptableObjects para configuração de NPCs

  - Criar NPCDropData ScriptableObject em `Assets/Code/Systems/Data/` com sistema de drops configurável (itemPrefab, dropChance, minQuantity, maxQuantity, guaranteedDrop)
  - Criar NPCInteractionData ScriptableObject em `Assets/Code/Systems/Data/` com tipos de interação (Dialogue, ItemDelivery, QuestActivation, QuestCompletion, Shop)
  - Adicionar validação de dados no Inspector usando OnValidate()
  - Adicionar menu items [CreateAssetMenu] para facilitar criação
  - _Requirements: 5.1, 5.4, 4.1, 4.2, 4.3_

- [x] 2. Implementar NPCAttributesHandler para gerenciamento de atributos

  - Criar classe NPCAttributesHandler em `Assets/Code/Systems/Controllers/` seguindo padrão Handler do projeto (baseado em PlayerAttributesHandler)
  - Implementar propriedades de atributos base (baseHealth, baseAttack, baseDefense, baseSpeed) com getters públicos (CurrentHealth, CurrentAttack, CurrentDefense, CurrentSpeed, MaxHealth)
  - Implementar sistema de relacionamento com propriedade RelationshipPoints e métodos públicos (ModifyRelationship, IncreaseRelationship, DecreaseRelationship)
  - Implementar métodos IsHostile() (RelationshipPoints < 0), IsFriendly() (RelationshipPoints > 10), IsNeutral() (0 <= RelationshipPoints <= 10)
  - Implementar sistema de eventos usando System.Action (OnHealthChanged, OnNPCDied, OnRelationshipChanged, OnDamageTaken)
  - Implementar método TakeDamage(int damage, bool ignoreDefense) com cálculo de defesa (redução baseada em fórmula: defense * 100 / (defense + 100))
  - Implementar método Heal(int healAmount) com clamp no MaxHealth
  - Implementar método GetAttributesSummary() para debug
  - Adicionar sistema de logs configurável (enableLogs) e debug gizmos (enableDebugGizmos) no Inspector
  - Implementar OnDrawGizmos() para exibir informações de atributos usando Handles.Label
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 3.1, 3.2, 3.3, 3.4, 7.5, 9.4_

- [x] 3. Refatorar NPCController - Estrutura base e inicialização

  - Refatorar classe NPCController existente em `Assets/Code/Gameplay/NPCs/NPCController.cs` para seguir nova arquitetura
  - Criar arquivo NPCEnums.cs com enums (NPCCategory: Enemy/Friendly/Neutral/Boss, MovementPattern: Idle/PatrolPoints/CircularPatrol/ChaseTarget, NPCState: Idle/Patrolling/Chasing/Attacking/Interacting/Dead)
  - Implementar propriedades configuráveis no Inspector organizadas por regiões (#region Category Settings, Movement Settings, Combat Settings, etc)
  - Implementar Awake() com cache de componentes (Rigidbody2D, Animator, SpriteRenderer) e validação de componentes obrigatórios
  - Implementar integração com NPCAttributesHandler (GetComponent e subscribe a eventos)
  - Adicionar referências a NPCDropData e NPCInteractionData[] no Inspector
  - Adicionar sistema de logs (enableLogs) e debug gizmos (enableDebugGizmos) configuráveis
  - Implementar método GetCurrentState() para expor estado atual
  - _Requirements: 7.1, 7.2, 7.4, 7.5_

- [x] 4. Implementar sistema de movimentação do NPCController

  - Implementar padrão Idle (NPC parado, velocity = Vector2.zero)
  - Implementar padrão PatrolPoints com navegação sequencial entre waypoints (Transform[] patrolPoints, int currentPatrolIndex, float waypointReachThreshold)
  - Implementar padrão CircularPatrol com movimento em área circular (Vector2 patrolCenter, float patrolRadius, float circularPatrolSpeed)
  - Implementar padrão ChaseTarget para perseguição de alvos (Transform currentTarget, usar Rigidbody2D.velocity)
  - Implementar aceleração e desaceleração suave no movimento (float acceleration, float deceleration)
  - Sincronizar moveSpeed com NPCAttributesHandler.CurrentSpeed
  - Implementar métodos públicos de controle de movimento (SetMovementPattern, SetPatrolPoints, SetCircularPatrol, ChaseTarget, StopMovement)
  - Adicionar validação de configuração de patrulha no Awake() (verificar se patrolPoints não está vazio quando pattern = PatrolPoints)
  - Implementar lógica de movimento em FixedUpdate() para física
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

- [x] 5. Implementar sistema de combate do NPCController

  - Implementar detecção de alvos usando Physics2D.OverlapCircle com LayerMask (targetLayers) e detectionRange
  - Implementar verificação de alvo em attack range usando IsTargetInRange(Transform target, float range)
  - Implementar método PerformAttack() com cooldown (float attackCooldown, float lastAttackTime)
  - Implementar OnAttackAnimationHit() para ser chamado por Animation Event do Animator
  - Implementar aplicação de dano ao alvo usando CurrentAttack do AttributesHandler (verificar se alvo tem componente que recebe dano)
  - Integrar com sistema de animação do Animator (SetTrigger para "Attack", SetFloat para "Speed")
  - Adicionar reprodução de efeitos visuais (GameObject attackVFX) e sonoros de ataque (AudioClip[] attackSounds com seleção aleatória)
  - Implementar lógica de detecção em Update() e ataque baseado em estado
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5_

- [x] 6. Implementar sistema de interação do NPCController

  - Implementar método Interact(GameObject interactor) para processar interações com jogador
  - Implementar método CanInteractWith(GameObject interactor) verificando RelationshipPoints e interactionRange
  - Implementar processamento de diferentes tipos de interação baseado em NPCInteractionData.InteractionType (Dialogue, ItemDelivery, QuestActivation, QuestCompletion, Shop)
  - Implementar verificação de interactionRange usando Distance check
  - Integrar com NPCInteractionData[] ScriptableObject (iterar por interações disponíveis)
  - Disparar eventos apropriados para sistemas externos (chamar DialogueManager para Dialogue, InventoryManager para ItemDelivery, QuestManager para quests)
  - Implementar verificação de requiredRelationshipPoints antes de permitir interação
  - Implementar suporte para interações oneTimeOnly (marcar consumed após uso)
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5_

- [x] 7. Implementar sistema de drops ao derrotar NPC

  - Implementar método ProcessDrops() chamado quando OnNPCDied é disparado (subscribe no Start)
  - Implementar cálculo de probabilidade baseado em dropChance usando Random.Range(0f, 100f)
  - Implementar geração de quantidade aleatória entre minQuantity e maxQuantity usando Random.Range
  - Implementar instanciamento de prefabs de itens na posição do NPC usando Instantiate(itemPrefab, transform.position, Quaternion.identity)
  - Implementar lógica de guaranteedDrop (sempre dropar pelo menos um item se true)
  - Integrar com NPCDropData ScriptableObject (iterar por possibleDrops)
  - Adicionar pequeno offset aleatório na posição de spawn dos itens para evitar sobreposição
  - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5_

- [x] 8. Implementar sistema de categorias de NPC com Behavior Graphs

  - Adicionar propriedade GameObject behaviorGraphPrefab no Inspector (referência ao prefab do Behavior Graph)
  - Implementar carregamento de Behavior Graph baseado em NPCCategory no Start() (instanciar behaviorGraphPrefab)
  - Implementar integração com Behavior Graph da Unity 6.2 (adicionar componente BehaviorGraphAgent se disponível)
  - Expor variáveis para o Behavior Graph como propriedades públicas (currentState, isAlive, canMove, canAttack, currentHealth, relationshipPoints, hasTarget, distanceToTarget, targetInAttackRange, currentVelocity, moveDirection)
  - Implementar métodos de callback do Behavior Graph (OnStateEnter(string stateName), OnStateExit(string stateName), OnTransition(string fromState, string toState))
  - Implementar envio de eventos para o Behavior Graph (OnTargetDetected(Transform target), OnTargetLost(), OnHealthLow(float percentage))
  - Implementar método OnBehaviorGraphStateChanged(string stateName) para sincronização de estados (atualizar currentState enum)
  - Validar assignment de Behavior Graph no Awake() (Debug.LogWarning se behaviorGraphPrefab for null)
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6, 8.7_

- [x] 9. Implementar sistema de debug visual com Gizmos no NPCController

  - Implementar OnDrawGizmos() no NPCController
  - Desenhar attack range como esfera wireframe vermelha (Gizmos.color = Color.red, Gizmos.DrawWireSphere)
  - Desenhar detection range como esfera wireframe amarela (Gizmos.color = Color.yellow, Gizmos.DrawWireSphere)
  - Desenhar patrol points com linhas verdes conectando waypoints (Gizmos.color = Color.green, Gizmos.DrawLine entre pontos)
  - Desenhar circular patrol como círculo wireframe azul (Gizmos.color = Color.blue, usar loop para desenhar círculo)
  - Desenhar linha vermelha do NPC ao alvo atual (Gizmos.color = Color.red, Gizmos.DrawLine se currentTarget != null)
  - Desenhar interaction range como esfera wireframe verde (Gizmos.color = Color.green, Gizmos.DrawWireSphere)
  - Exibir informações textuais (Health, State, Relationship Points) usando Handles.Label acima do NPC
  - Adicionar toggle enableDebugGizmos no Inspector para controlar visibilidade
  - _Requirements: 9.1, 9.2, 9.3, 9.5_

- [x] 10. Implementar integração com sistemas existentes do jogo

  - Integrar detecção e ataque ao PlayerController (verificar tag "Player" e obter componente PlayerAttributesHandler para aplicar dano)
  - Integrar com GameManager para registro de NPCs ativos (chamar GameManager.Instance.RegisterNPC(this) no Start e UnregisterNPC no OnDestroy)
  - Integrar com AudioManager para reprodução de sons (AudioManager.Instance.PlaySFX para sons de ataque e morte)
  - Integrar drops com sistema de itens coletáveis existente (garantir que prefabs dropados tenham componente ItemCollectable)
  - Implementar resposta a eventos de pause do GameManager (subscribe a GameManager.OnGameStateChanged e pausar movimento/ações quando pausado)
  - Adicionar suporte para interação via Input System do jogador (verificar input de interação quando jogador está próximo)
  - _Requirements: 7.8_

- [x] 11. Criar prefabs de exemplo para cada categoria de NPC

  - Criar prefab "NPC_Enemy_Example" em `Assets/Game/Prefabs/NPCs/` com configuração hostil (NPCCategory.Enemy, RelationshipPoints = -10, comportamento agressivo)
  - Criar prefab "NPC_Friendly_Example" com configuração amigável (NPCCategory.Friendly, RelationshipPoints = 15, sem ataque)
  - Criar prefab "NPC_Neutral_Example" com configuração neutra (NPCCategory.Neutral, RelationshipPoints = 5, ataca apenas quando provocado)
  - Criar prefab "NPC_Boss_Example" com configuração de chefe (NPCCategory.Boss, atributos elevados, drops especiais)
  - Configurar Behavior Graphs específicos para cada categoria (criar assets de Behavior Graph se disponível na Unity 6.2)
  - Adicionar exemplos de drops (criar NPCDropData assets) e interações (criar NPCInteractionData assets)
  - Criar documento de uso dos prefabs em `Assets/Docs/Systems/NPCs/NPC_Prefabs_Guide.md`
  - _Requirements: 8.2, 8.3, 8.4, 8.5, 8.6_

- [x] 12. Criar testes de integração para validação do sistema

  - Criar cena de teste "NPCSystemTest" em `Assets/Scenes/Tests/`
  - Testar comunicação entre NPCController e NPCAttributesHandler (verificar eventos de dano e morte)
  - Testar transições de estado baseadas em eventos (Idle -> Chasing -> Attacking)
  - Testar interação com PlayerController (aproximação, detecção, ataque)
  - Testar sistema de drops e coleta de itens (derrotar NPC e verificar spawn de itens)
  - Testar comportamento de cada categoria de NPC (Enemy persegue, Friendly não ataca, Neutral retalia, Boss usa Behavior Graph)
  - Validar integração com Behavior Graph (se disponível)
  - Documentar resultados dos testes
  - _Requirements: Todos_
