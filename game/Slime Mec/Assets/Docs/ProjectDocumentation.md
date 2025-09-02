# Documentação Detalhada do Projeto Slime Mec

## Visão Geral

O projeto **Slime Mec** é um jogo 2D desenvolvido na Unity 6000.2.0b13, utilizando o Universal Render Pipeline (URP) para gráficos otimizados. O jogo apresenta mecânicas avançadas de destruição de objetos, sistema de lançamento de itens com física realista, efeitos visuais dinâmicos e um sistema de vento interativo.

---

## Arquitetura do Projeto

### Estrutura de Diretórios

- **_Animation/**: Animações e controladores de animação organizados por tipo
  - `Attack01/`: Animações de ataque com eventos
  - `BushA/`, `BushA2/`, `BushB/`: Animações de arbustos (Idle, Shake, Destroy)
  - `Tree01/`: Animações de árvores
  - `White Slime/`: Animações do personagem principal

- **_Art/**: Assets visuais organizados
  - `Materials/`: Materiais de renderização
  - `Sprites/`: Sprites e texturas do jogo

- **_Audio/**: Sistema de áudio
  - `Music/`: Trilhas sonoras
  - `SFX/`: Efeitos sonoros

- **_Prefabs/**: Prefabs organizados por categoria
  - `Characters/`: Personagens e entidades
  - `Environment/`: Objetos do ambiente
  - `Items/`: Itens coletáveis
  - `Systems/`: Sistemas de gameplay
  - `UI/`: Interface do usuário

- **_Scripts/**: Scripts organizados por funcionalidade
  - `Core/`: Funcionalidades centrais
  - `Gameplay/`: Mecânicas de jogo
  - `Managers/`: Gerenciadores de sistemas
  - `Debug/`: Ferramentas de debug
  - `Editor/`: Ferramentas do editor
  - `Utils/`: Utilitários gerais

- **_Settings/**: Configurações do projeto
  - Render Pipeline settings
  - Input System configurations
  - Scene templates

---

## Scripts de Gameplay Principais

### 1. **PlayerController.cs**

**Localização**: `Assets/_Scripts/Gameplay/`

**Descrição**: Controlador principal do personagem jogador com sistema avançado de movimento e combate.

**Componentes Requeridos**:

- `Rigidbody2D`: Para física de movimento
- `Animator`: Para controle de animações
- `SpriteRenderer`: Para renderização visual

**Funcionalidades Principais**:

- Sistema de movimento suave com aceleração/desaceleração
- Controle direcional visual (Sul, Norte, Lateral)
- Sistema de ataque com múltiplas direções
- Integração com Input System
- Sistema de VFX direcionais
- Integração com sistema de atributos

**Enum VisualDirection**:

- `South`: Frente (padrão)
- `North`: Costas
- `Side`: Lateral (East/West)

### 2. **AttackHandler.cs**

**Localização**: `Assets/_Scripts/Gameplay/`

**Descrição**: Sistema avançado de detecção e processamento de ataques com área retangular.

**Funcionalidades**:

- Detecção retangular usando `Physics2D.OverlapBox`
- Sistema de offset dinâmico baseado na direção
- Cache de componentes para otimização
- Suporte para ataques frontais e laterais

**Enum AttackDirection**:

- `South`: Sul - para baixo
- `North`: Norte - para cima
- `East`: Leste - para direita
- `West`: Oeste - para esquerda

**Métodos Principais**:

- `PerformAttack(bool isAttackingSideways, AttackDirection direction)`
- `ClearComponentCache()`: Limpa cache para otimização de memória

### 3. **WindController.cs**

**Localização**: `Assets/_Scripts/Gameplay/`

**Descrição**: Controla objetos de vento que se movem pela tela e ativam animações de shake.

**Funcionalidades**:

- Movimento horizontal otimizado
- Sistema de detecção de objetos para shake
- Auto-destruição inteligente
- Controle de frequência de verificação

**Enum MovementDirection**:

- `Left`: Movimento para esquerda
- `Right`: Movimento para direita

### 4. **WindManager.cs**

**Localização**: `Assets/_Scripts/Gameplay/`

**Descrição**: Gerenciador responsável por spawnar e controlar objetos de vento.

**Funcionalidades**:

- Sistema de pool de objetos para performance
- Spawn automático com frequência configurável
- Limpeza automática de objetos inativos
- Área de spawn configurável

### 5. **DropController.cs**

**Localização**: `Assets/_Scripts/Gameplay/`

**Descrição**: Sistema de drop de itens com configurações flexíveis.

**Funcionalidades**:

- Quantidade aleatória de drops configurável
- Lista de prefabs para seleção aleatória
- Posicionamento automático
- Sistema de logs para debug

**Propriedades Públicas**:

- `PrefabCount`: Quantidade de prefabs disponíveis
- `MinDropCount`/`MaxDropCount`: Range de quantidade
- `HasValidPrefabs`: Validação da lista

### 6. **BounceHandler.cs**

**Localização**: `Assets/_Scripts/Gameplay/`

**Descrição**: Sistema avançado de física para objetos que quicam com efeitos visuais.

**Componentes Requeridos**:

- `Rigidbody2D`: Para física 2D

**Funcionalidades Principais**:

- Lançamento com força e ângulo configuráveis
- Sistema de quicadas sequenciais com redução de força
- Efeitos de sombra dinâmicos baseados na velocidade
- Auto-destruição opcional

**Parâmetros Configuráveis**:

- `minLaunchForce`/`maxLaunchForce`: Range de força
- `bounceCount`: Número de quicadas
- `forceReductionFactor`: Redução de força por quicada
- `shadowObject`: GameObject para efeito de sombra

### 7. **BushDestruct.cs**

**Localização**: `Assets/_Scripts/Gameplay/`

**Descrição**: Componente para objetos destrutíveis do ambiente.

**Namespace**: `SlimeMec.Gameplay`

**Funcionalidades**:

- Proteção contra destruição múltipla
- Integração com animações via Animator
- Integração automática com DropController
- Sistema de validação de triggers

**Métodos Principais**:

- `TakeDamage()`: Recebe dano e ativa destruição
- `DestroyObject()`: Destroi o GameObject (chamado por Animation Event)

### 8. **PlayerAttributesHandler.cs**

**Localização**: `Assets/_Scripts/Gameplay/`

**Descrição**: Gerenciador completo dos atributos do personagem.

**Atributos Gerenciados**:

- Health Points (vida)
- Attack (ataque)
- Defense (defesa)
- Speed (velocidade)
- Skill Points (pontos de habilidade)

**Eventos Disponíveis**:

- `OnHealthChanged`: Mudança nos pontos de vida
- `OnPlayerDied`: Morte do jogador
- `OnSkillPointsChanged`: Mudança nos pontos de habilidade

### 9. **RandomStyle.cs**

**Localização**: `Assets/_Scripts/Gameplay/`

**Descrição**: Componente utilitário para variação visual automática.

**Funcionalidades**:

- Randomização de escala (uniforme ou não)
- Randomização de cor (lista ou interpolação)
- Aplicação automática no Start
- Métodos para controle manual

**Suporte para Componentes**:

- `SpriteRenderer`: Para sprites 2D
- `Renderer`: Para objetos 3D

### 10. **SelfDestruct.cs**

**Localização**: `Assets/_Scripts/Gameplay/`

**Descrição**: Utilitário simples para auto-destruição de GameObjects.

**Funcionalidades**:

- Destruição automática com delay configurável
- Métodos para destruição manual
- Destruição com delay específico

---

## Prefabs Principais

### Personagens (`_Prefabs/Characters/`)

#### **chr_whiteslime.prefab**

- **Descrição**: Personagem principal jogável
- **Componentes**:
  - `PlayerController`: Controle de movimento e ações
  - `PlayerAttributesHandler`: Gerenciamento de atributos
  - `Rigidbody2D`: Física de movimento
  - `Animator`: Animações do personagem
  - `SpriteRenderer`: Renderização visual
  - `Collider2D`: Detecção de colisão

#### **attack01_vfx.prefab**

- **Descrição**: Efeito visual de ataque
- **Componentes**:
  - `AttackHandler`: Detecção de alvos
  - `SelfDestruct`: Auto-destruição após animação
  - `Animator`: Animação do efeito
  - `SpriteRenderer`: Renderização do efeito

### Ambiente (`_Prefabs/Environment/`)

#### **bushA2.prefab** / **bushB.prefab**

- **Descrição**: Arbustos destrutíveis do ambiente
- **Componentes**:
  - `BushDestruct`: Lógica de destruição
  - `DropController`: Sistema de drop de itens
  - `Animator`: Animações (Idle, Shake, Destroy)
  - `SpriteRenderer`: Renderização
  - `Collider2D`: Detecção de ataques (Tag: "Destructable")

#### **env_tree01.prefab**

- **Descrição**: Árvore decorativa do ambiente
- **Componentes**:
  - `RandomStyle`: Variação visual automática
  - `SpriteRenderer`: Renderização
  - `Collider2D`: Física do ambiente

#### **WindManager.prefab**

- **Descrição**: Gerenciador do sistema de vento
- **Componentes**:
  - `WindManager`: Controle de spawn de ventos
  - Configurações de área e frequência

#### **wind_vfx.prefab**

- **Descrição**: Efeito de vento individual
- **Componentes**:
  - `WindController`: Movimento e detecção
  - `SpriteRenderer`: Renderização do vento
  - `Collider2D`: Área de influência

### Itens (`_Prefabs/Items/`)

#### **appleA.prefab** / **redFruit.prefab** / **mushroomA.prefab**

- **Descrição**: Itens coletáveis que podem ser dropados
- **Componentes**:
  - `BounceHandler`: Física de quicadas ao ser dropado
  - `RandomStyle`: Variação visual
  - `SpriteRenderer`: Renderização
  - `Collider2D`: Detecção de coleta

#### **fireStarA.prefab**

- **Descrição**: Item especial com efeitos
- **Componentes**:
  - `BounceHandler`: Física avançada
  - `RandomStyle`: Variação de aparência
  - `SpriteRenderer`: Renderização com efeitos
  - `Collider2D`: Área de coleta

---

## Sistema de Animações

### Controladores de Animação

#### **bushA.controller** / **bushA2.controller** / **bushB.controller**

- **Parâmetros**:
  - `Shake` (Trigger): Ativa animação de balanço
  - `Destroy` (Trigger): Ativa animação de destruição
- **Estados**:
  - `Idle`: Estado padrão
  - `Shake`: Balanço causado pelo vento
  - `Destroy`: Sequência de destruição com Animation Event

#### **attack01_vfx.controller**

- **Estados**:
  - `attack01_vfx_Clip`: Animação do efeito de ataque
- **Animation Events**:
  - `DestroyObject` (tempo 0.49s): Auto-destruição do efeito

### Animation Events

- **DestroyObject**: Chamado ao final de animações de destruição
- **ResetAttackState**: Reseta estado de ataque (White Slime)

---

## Dependências e Pacotes

### Pacotes Unity Principais

```json
{
  "com.unity.2d.animation": "12.0.2",
  "com.unity.2d.sprite": "1.0.0",
  "com.unity.inputsystem": "1.14.2",
  "com.unity.render-pipelines.universal": "17.2.0",
  "com.unity.timeline": "1.8.9",
  "com.unity.ugui": "2.0.0"
}
```

### Recursos de Arte

- **Aseprite Support**: `com.unity.2d.aseprite": "2.0.1"`
- **PSD Importer**: `com.unity.2d.psdimporter": "11.0.1"`
- **Sprite Shape**: `com.unity.2d.spriteshape": "12.0.1"`

### Ferramentas de Desenvolvimento

- **Visual Scripting**: `com.unity.visualscripting": "1.9.8"`
- **Test Framework**: `com.unity.test-framework": "1.5.1"`
- **IDE Support**: Rider e Visual Studio

---

## Configurações do Projeto

### Universal Render Pipeline (URP)

- **Asset Principal**: `cfg_universalrp.asset`
- **Renderer 2D**: `cfg_renderer2d.asset`
- **Configurações Globais**: `sst_universalrenderpipelineglobalsettings.asset`

### Sistema de Input

- **Actions**: `InputSystem_Actions.inputactions`
- **Script Gerado**: `InputSystem_Actions.cs`
- **Mapeamentos**:
  - Movement (WASD/Arrow Keys)
  - Attack (Space/Mouse)
  - Interact (E)
  - Use Items (1-4)

### Performance e Otimização

- **Object Pooling**: Implementado no WindManager
- **Component Caching**: AttackHandler e outros sistemas
- **Batch Operations**: Redução de calls desnecessárias
- **Smart Updates**: Controle de frequência de verificações

---

## Tags e Layers

### Tags Principais

- **"Destructable"**: Objetos que podem ser atacados
- **"Player"**: Personagem jogador
- **"Item"**: Itens coletáveis

### Layers

- **Default (0)**: Objetos padrão
- **Player**: Camada do jogador
- **Environment**: Objetos do ambiente
- **Items**: Itens coletáveis
- **Wind Shakers (6)**: Objetos afetados pelo vento

---

## Fluxo de Gameplay

### 1. **Inicialização**

1. PlayerAttributesHandler inicializa atributos
2. WindManager inicia spawn de ventos
3. PlayerController configura input system

### 2. **Loop Principal**

1. Player move usando WASD
2. Ataques detectam objetos "Destructable"
3. BushDestruct ativa animação e drop
4. DropController instancia itens com BounceHandler
5. WindController move ventos e ativa shakes

### 3. **Sistemas de Feedback**

1. Eventos de Animation chamam métodos específicos
2. Gizmos mostram áreas de debug no editor
3. Logs fornecem informações de desenvolvimento

---

## Ferramentas de Debug

### Scripts de Debug

- **WindSystemDebugger.cs**: Debug específico do sistema de vento
- **Context Menus**: Ações rápidas no editor (Test Destroy, Debug Info)

### Visualização no Editor

- **Gizmos**: Áreas de ataque, spawn zones, movimento
- **Handles**: Labels informativos no Scene View
- **Debug Logs**: Sistema configurável por componente

---

## Boas Práticas Implementadas

### Performance

- Object pooling para objetos frequentes
- Cache de componentes para evitar GetComponent
- Early exit patterns
- Batch operations

### Código Limpo

- Namespaces organizados (`SlimeMec.Gameplay`)
- Documentação XML detalhada
- Separação clara de responsabilidades
- Validações de null e edge cases

### Manutenibilidade

- Configurações expostas no Inspector
- Sistema de eventos desacoplado
- Enums para type safety
- Context menus para debug

---

## Histórico de Desenvolvimento

### Versão Atual (Unity 6000.2.0b13)

- **02/09/2025**: Documentação completa criada
- Implementação do sistema de bounce com efeitos de sombra
- Integração entre DropController e BushDestruct
- Sistema de vento otimizado com object pooling
- Sistema de ataque com detecção retangular avançada

### Principais Melhorias Implementadas

- **Performance**: Object pooling, component caching, batch operations
- **Usabilidade**: Context menus, gizmos de debug, logs configuráveis
- **Manutenibilidade**: Namespaces organizados, documentação XML, validações

---

## Arquitetura de Código

### Padrões Utilizados

- **Component Pattern**: Cada funcionalidade em componente separado
- **Object Pooling**: WindManager para objetos frequentes
- **Event System**: Comunicação desacoplada entre sistemas
- **Strategy Pattern**: Diferentes tipos de ataques e movimentos

### Otimizações Implementadas

- Cache de componentes com Dictionary
- Early exit patterns
- Reduced frequency checks
- Batch processing para detecções

---

## Troubleshooting

### Problemas Comuns

#### **Objetos não são detectados pelo ataque**

- Verificar se têm a tag "Destructable"
- Confirmar se estão na layer correta (destructableLayerMask)
- Validar se têm componente BushDestruct

#### **Animações não funcionam**

- Verificar se Animator Controller está configurado
- Confirmar se triggers existem (Shake, Destroy)
- Validar Animation Events (DestroyObject)

#### **Sistema de vento não funciona**

- Verificar WindManager prefab na cena
- Confirmar windPrefab está configurado
- Validar layer mask para windShakers

#### **Itens não fazem bounce**

- Verificar se têm componente BounceHandler
- Confirmar Rigidbody2D está configurado
- Validar configurações de força e bounce count

---

## Licença e Créditos

### Informações Legais

- **Projeto**: Slime Mec
- **Desenvolvedor Principal**: T-Barz
- **Engine**: Unity 6000.2.0b13
- **Repositório**: [slimeking](https://github.com/t-barz/slimeking)

### Dependências de Terceiros

- Unity Technologies (Engine e Packages)
- Universal Render Pipeline
- Input System Package
- 2D Animation Package

### Política de Uso

Este projeto é de propriedade privada e está sujeito às políticas de uso estabelecidas pelo desenvolvedor principal.

---

## Contato e Suporte

### Para Desenvolvedores

- **Documentação Adicional**: Consulte os arquivos na pasta `Docs/`
- **Guias Específicos**: `WIND_SYSTEM_SETUP_GUIDE.md`, `BoasPraticas.md`
- **Ferramentas de Debug**: Use os Context Menus nos componentes

### Referências Úteis

- [Unity Documentation](https://docs.unity3d.com/)
- [URP Documentation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)
- [Input System Documentation](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest)

---

*Documentação atualizada em: 02 de Setembro de 2025*

## Licença

Este projeto é propriedade de **T-Barz** e está sujeito às suas políticas de uso.

---

## Histórico de Alterações

- **02/09/2025**: Documentação inicial criada.
