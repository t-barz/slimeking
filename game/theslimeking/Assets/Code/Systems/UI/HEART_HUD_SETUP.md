# Heart HUD System Implementation Guide

## Visão Geral

Sistema de HUD de vida com corações que se integra automaticamente ao `PlayerAttributesHandler`. Os corações mostram a vida atual e reproduzem animações quando há mudanças.

## Scripts Criados

### 1. HeartUIElement.cs

- Componente individual de coração
- Gerencia estado visual (cheio/vazio)
- Animação de bounce quando há mudança de estado
- Localização: `Assets/💻 Code/Systems/UI/HeartUIElement.cs`

### 2. HealthUIManager.cs

- Gerenciador principal do sistema
- Conecta-se automaticamente ao PlayerAttributesHandler
- Cria e organiza layout de corações
- Localização: `Assets/💻 Code/Systems/UI/HealthUIManager.cs`

## Setup no Unity

### 1. Criar o Prefab de Coração

1. **Criar GameObject base:**
   - Hierarquia: botão direito → UI → Image
   - Nome: `HeartPrefab`

2. **Configurar Image Component:**
   - Source Image: `ui_hearthCounterOK.png`
   - Preserve Aspect: true
   - Raycast Target: false (otimização)

3. **Adicionar HeartUIElement:**
   - Add Component → HeartUIElement
   - Heart Full Sprite: `ui_hearthCounterOK.png`
   - Heart Empty Sprite: `ui_hearthCounterNOK.png`
   - Bounce Scale: 1.2
   - Bounce Duration: 0.3

4. **Salvar como Prefab:**
   - Arrastar para pasta `Assets/Game/Prefabs/UI/`

### 2. Configurar o Canvas HUD

1. **Localizar Canvas HUD existente** (já existe na cena 2_InitialCave)

2. **Criar Container de Corações:**

   ```
   CanvasHUD
   ├── HeartsContainer (GameObject vazio)
   │   ├── RectTransform
   │   │   ├── Anchor: Top-Left
   │   │   ├── Position: (20, -20, 0)
   │   │   └── Size: (350, 100)
   │   └── HealthUIManager Component
   ```

3. **Configurar HealthUIManager:**
   - Heart Full Sprite: `ui_hearthCounterOK.png`
   - Heart Empty Sprite: `ui_hearthCounterNOK.png`
   - Heart Prefab: arrastar o prefab criado
   - Hearts Container: referenciar o próprio GameObject
   - Hearts Per Row: 10
   - Heart Spacing: 35
   - Row Spacing: 35
   - Find Player Automatically: true

### 3. Integração com PlayerAttributesHandler

O sistema se conecta automaticamente ao `PlayerAttributesHandler` através dos eventos:

- `OnHealthChanged`: atualiza display de corações
- Busca automática pelo componente na cena

## Funcionamento

### Estados dos Corações

- **Cheio**: Mostra `ui_hearthCounterOK.png`
- **Vazio**: Mostra `ui_hearthCounterNOK.png`

### Animações

- **Bounce**: Quando coração ganha/perde vida
- **Escala**: 1.0 → 1.2 → 1.0
- **Duração**: 0.3 segundos configurável

### Layout Automático

- Organização em grid (10 corações por linha padrão)
- Espaçamento configurável entre corações
- Suporte para 1-30+ corações

## Testes de Integração

### 1. Verificar PlayerAttributesHandler

```csharp
// No código existente, usar:
playerAttributes.TakeDamage(1); // Remove 1 coração
playerAttributes.Heal(1);       // Restaura 1 coração
playerAttributes.FullHeal();    // Restaura todos
```

### 2. Debug Visual

- Ativar `Enable Logs` nos componentes
- Verificar Console para eventos de saúde
- Scene View: Gizmos do PlayerAttributesHandler

### 3. Cenários de Teste

1. **Vida Inicial**: 3/3 corações cheios
2. **Tomar Dano**: Corações mudam para vazio com animação
3. **Cura**: Corações vazios voltam a ficar cheios
4. **Morte**: Todos corações vazios

## Estrutura de Arquivos

```
Assets/
├── 💻 Code/Systems/UI/
│   ├── HeartUIElement.cs
│   ├── HealthUIManager.cs
│   └── HEART_HUD_SETUP.md
├── Art/Sprites/UI/
│   ├── ui_hearthCounterOK.png
│   └── ui_hearthCounterNOK.png
└── Game/Prefabs/UI/
    └── HeartPrefab.prefab
```

## Configurações Avançadas

### Customização de Sprites

```csharp
// Via código
heartElement.SetSprites(novoSpriteCheio, novoSpriteVazio);

// Ou via Inspector no prefab
```

### Layout Personalizado

```csharp
// Via HealthUIManager
manager.ReconfigureHearts(novoMaximo);
```

### Animações Personalizadas

```csharp
// Modificar curva de bounce no Inspector
// Animation Curve: personalizar no HeartUIElement
```

## Compatibilidade

- Unity 6.2+
- Universal Render Pipeline (URP)
- Namespace: `SlimeKing.Core.UI`
- Sistema de Eventos do PlayerAttributesHandler

## Troubleshooting

### Corações não aparecem

1. Verificar se PlayerAttributesHandler está na cena
2. Confirmar referências no HealthUIManager
3. Checar se sprites estão configurados

### Animações não funcionam

1. Verificar se Time.timeScale > 0
2. Confirmar curvas de animação configuradas
3. Log de debug ativado para verificar eventos

### Performance

- Raycast Target desabilitado nos corações
- Animações utilizam Corrotines eficientes
- Layout calculado apenas quando necessário

## Próximos Passos

1. Implementar no Unity seguindo este guia
2. Testar integração com PlayerAttributesHandler
3. Ajustar posicionamento e espaçamento conforme design
4. Validar em diferentes resoluções de tela
