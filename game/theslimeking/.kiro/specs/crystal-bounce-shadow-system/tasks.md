# Implementation Plan

- [x] 1. Adicionar enum BounceState e refatorar gerenciamento de estados

  - Criar enum `BounceState` com estados: NotLaunched, Launching, Bouncing, Stopping, Stopped, ReadyForCollection
  - Adicionar propriedade privada `_currentState` e pública `CurrentState`
  - Adicionar propriedades derivadas `IsMoving` e `IsReadyForCollection`
  - Atualizar transições de estado em todos os métodos relevantes
  - _Requirements: 1.1, 1.6, 2.7_

- [x] 2. Implementar sistema de controle de colliders

  - Adicionar método privado `DisableAllColliders()` que desabilita todos os Collider2D do objeto
  - Adicionar método privado `EnableAllColliders()` que habilita todos os Collider2D do objeto
  - Adicionar método público `EnableColliders()` para controle externo
  - Adicionar método público `DisableColliders()` para controle externo
  - Cachear array de colliders no Awake para performance
  - Chamar `DisableAllColliders()` no Awake após inicialização
  - _Requirements: 7.1, 7.2, 7.6_

- [x] 3. Corrigir cálculo de escala da sombra

  - Modificar método `UpdateShadowEffect()` para usar valor absoluto da velocidade vertical
  - Alterar cálculo de `simulatedHeight` para: `float speed = Mathf.Abs(_rigidbody2D.linearVelocity.y)`
  - Alterar normalização para: `float normalizedHeight = Mathf.Clamp01(speed / maxSimulatedHeight)`
  - Manter interpolação de escala: `Mathf.Lerp(maxShadowScale, minShadowScale, normalizedHeight)`
  - Adicionar comentários explicando a lógica corrigida
  - _Requirements: 3.3, 3.4, 3.5, 3.9_

- [x] 4. Adicionar sincronização com ItemCollectable

  - Adicionar método privado `OnMovementStopped()` chamado quando movimento para
  - Buscar componente `ItemCollectable` no objeto
  - Obter `ActivationDelay` do ItemCollectable se existir
  - Usar `Invoke` para agendar habilitação de colliders após o delay
  - Adicionar método privado `EnableCollidersAndNotify()` que habilita colliders e atualiza estado para ReadyForCollection
  - Modificar `StopMovement()` para chamar `OnMovementStopped()` ao invés de habilitar colliders diretamente
  - _Requirements: 5.1, 5.2, 7.3, 7.4_

- [x] 5. Melhorar controle de Update da sombra

  - Modificar condição do Update para verificar `_hasBeenLaunched && _currentState != BounceState.Stopped && _currentState != BounceState.ReadyForCollection`
  - Adicionar método `ResetShadowToMaxScale()` que restaura sombra ao tamanho máximo
  - Chamar `ResetShadowToMaxScale()` quando movimento para completamente
  - Adicionar verificação de null para `_rigidbody2D` antes de acessar velocidade
  - _Requirements: 3.9, 4.4, 8.2, 8.4_

- [x] 6. Adicionar propriedades e métodos públicos para integração

  - Adicionar propriedade pública `BounceState CurrentState { get; }`
  - Adicionar propriedade pública `bool IsMoving { get; }` que retorna se está em Launching ou Bouncing
  - Adicionar propriedade pública `bool IsReadyForCollection { get; }` que retorna se está em ReadyForCollection
  - Modificar método `ResetLaunch()` para também resetar estado para NotLaunched
  - Adicionar cancelamento de Invoke de `EnableCollidersAndNotify` no ResetLaunch
  - _Requirements: 6.6, 5.4_
-

- [x] 7. Ajustar ItemCollectable para integração com BounceHandler

  - Adicionar método público `SetColliderEnabled(bool enabled)` que controla o collider do ItemCollectable
  - Adicionar propriedade pública `float ActivationDelay { get; }` que retorna o valor de `activationDelay`
  - Adicionar propriedade pública `bool IsActivationDelayComplete { get; }` que verifica se delay expirou
  - Modificar `CheckActivationDelay()` para também verificar se BounceHandler está pronto (se existir)
  - Adicionar verificação no Start para desabilitar collider se BounceHandler existir
  - _Requirements: 5.1, 5.2, 5.4_

- [ ] 8. Melhorar logs de debug e validações
  - Adicionar log quando colliders são desabilitados no Awake
  - Adicionar log quando colliders são habilitados após delay
  - Adicionar log de warning se valores de configuração são inválidos (min >= max, valores negativos)
  - Adicionar validação no Awake para verificar se shadowObject é filho do objeto principal
  - Modificar logs existentes para incluir estado atual do objeto
  - Adicionar log quando sincronização com ItemCollectable ocorre
  - _Requirements: 6.2, 6.5_

- [ ]* 9. Adicionar Context Menu para debug
  - Adicionar `[ContextMenu("Test Launch")]` que chama `LaunchItem()` em modo Play
  - Adicionar `[ContextMenu("Force Stop")]` que chama `StopMovementManually()`
  - Adicionar `[ContextMenu("Enable Colliders Now")]` que chama `EnableColliders()` imediatamente
  - Adicionar `[ContextMenu("Debug State")]` que loga estado completo do objeto
  - Adicionar `[ContextMenu("Reset Launch")]` que chama `ResetLaunch()`
  - Adicionar guards para verificar se está em Play mode
  - _Requirements: 6.3, 6.6_

- [x] 10. Melhorar Gizmos para visualização

  - Adicionar desenho de trajetória prevista baseada em força e ângulo
  - Usar cores diferentes para cada estado (verde = pronto, amarelo = quicando, vermelho = parado)
  - Desenhar indicador visual do raio de atração do ItemCollectable
  - Adicionar label com estado atual usando Handles.Label
  - Mostrar contador de quicadas atual vs total
  - _Requirements: 6.3_

- [ ] 11. Testar integração completa e ajustar valores
  - Criar cena de teste com múltiplos objetos crystalA
  - Testar lançamento, quicadas e parada
  - Verificar comportamento da sombra durante todo o ciclo
  - Confirmar que colliders são habilitados apenas após delay
  - Testar coleta após objeto estar pronto
  - Ajustar valores de configuração padrão se necessário
  - Verificar ausência de erros no console
  - _Requirements: 1.1, 2.1, 3.1, 4.1, 5.1, 7.1_
