# Documentação Técnica - Slime King

## Arquitetura do Sistema

### Estrutura de Namespaces

```
SlimeKing
├── Core
│   ├── Utils
│   ├── Cutscenes
│   └── GameUtilities
├── Gameplay
│   ├── Combat
│   ├── Items
│   ├── NPCActions
│   └── Environment
├── UI
│   ├── Controls
│   └── Effects
└── Utils
```

## Sistemas Principais

### Sistema de Input
- Baseado no novo Input System da Unity
- Suporte para múltiplos dispositivos
- Ações mapeadas via InputActionReference
- Sistema de controles virtuais para mobile

### Sistema de Movimento
```csharp
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    // Gerencia movimento, animações e estados do jogador
    // Integra com outros sistemas via componentes
}
```

### Sistema de Combate
```csharp
public class PlayerCombat : MonoBehaviour
{
    // Sistema de combate com:
    // - Detecção de hits
    // - Cálculo de dano
    // - Efeitos visuais
    // - Estados de ataque
}
```

### Sistema de Stealth
- Gerenciado pelo PlayerMovement
- Integração com VignetteController para efeitos visuais
- Manipulação de colisores durante stealth
- Estados de esconderijo

### Sistema de Efeitos Visuais
```csharp
public class VignetteController : MonoBehaviour
{
    // Controle de efeitos de pós-processamento
    // Transições suaves de efeitos
    // Integração via reflection para máxima compatibilidade
}
```

## Componentes do Jogador

### PlayerMovement
- Controle principal do personagem
- Gerenciamento de estados
- Integração com sistemas de input
- Coordenação com outros componentes

### PlayerVisualManager
- Gerenciamento de sprites e animações
- Atualização visual baseada em estados
- Feedback visual para ações

### PlayerCombat
- Sistema de combate
- Gerenciamento de ataques
- Cálculo de dano
- Efeitos de combate

### PlayerActionController
- Ações especiais (pulo, deslize)
- Estados especiais de movimento
- Coordenação com animações

### PlayerAudioManager
- Gerenciamento de efeitos sonoros
- Sincronização com ações
- Sistema de áudio adaptativo

## Sistema de Interação

### Interactable
Base para objetos interativos:
```csharp
public interface IInteractable
{
    void Interact();
    bool CanInteract();
}
```

Tipos de Interativos:
- ButtonPromptInteractable
- CollectableInteractable
- EnvironmentInteractable
- OutlineInteractable

## Sistema de Cutscenes

### Eventos de Cutscene
- AnimationEvent
- CameraEvent
- MovementEvent
- SpawnEvent
- WaitEvent

### Gerenciamento
```csharp
public class CutsceneDefinition
{
    // Define sequência de eventos
    // Controla fluxo da cutscene
    // Gerencia transições
}
```

## Sistema de UI

### Controles Mobile
- VirtualJoystick para movimento
- VirtualButton para ações
- Layout adaptativo
- Feedback visual

### Efeitos Visuais
- Sistema de vinheta
- Outlines interativos
- Efeitos de pós-processamento
- Transições suaves

## Utilidades e Helpers

### GameUtilities
- Funções utilitárias compartilhadas
- Acesso a sistemas globais
- Helpers comuns

### ItemPool
- Sistema de object pooling
- Gerenciamento de recursos
- Otimização de performance

## Considerações de Performance

### Otimizações
1. Object Pooling para objetos frequentes
2. Uso eficiente de física
3. Gerenciamento de estados via flags
4. Minimização de alocações em runtime

### Boas Práticas
1. Uso de RequireComponent para dependências
2. Inicialização lazy de recursos pesados
3. Cleanup apropriado em OnDestroy
4. Tratamento de erros robusto

## Pipeline de Renderização

### Universal Render Pipeline (URP)
- Configuração de pós-processamento
- Efeitos de vinheta
- Shaders customizados
- Otimizações gráficas

## Debugging e Logging

### Sistema de Log
- Mensagens informativas
- Avisos de configuração
- Erros de runtime
- Stacktraces para debugging

## Extensibilidade

### Pontos de Extensão
1. Sistema de plugins
2. Interfaces para novas mecânicas
3. Eventos customizáveis
4. Sistema modular
