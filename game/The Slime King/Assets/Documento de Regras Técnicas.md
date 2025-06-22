# Documento de Regras Técnicas – The Slime King
## Versão 1.2

---

## **ÍNDICE GERAL**

### **I. INTRODUÇÃO E ORGANIZAÇÃO**

- [Introdução Geral](#introdu%C3%A7%C3%A3o-geral)
- [Estrutura de Implementação por Fases](#estrutura-de-implementa%C3%A7%C3%A3o-por-fases)

---

### **II. FASE 1: FUNDAMENTOS TÉCNICOS**

*Sistemas essenciais que servem como base para todos os outros*

#### **1. Sistema de Localização de Textos**

- [x] [1.1 Estrutura de Arquivos CSV](#11-estrutura-de-arquivos-csv)
- [x] [1.2 Detecção Automática de Idioma](#12-detec%C3%A7%C3%A3o-autom%C3%A1tica-de-idioma)
- [x] [1.3 Sistema de Configuração](#13-sistema-de-configura%C3%A7%C3%A3o)


#### **2. Sistema de Ícone Superior**

- [x] [2.1 Detecção de Plataforma](#21-detec%C3%A7%C3%A3o-de-plataforma)
- [x] [2.2 Sistema de Exibição Dinâmica](#22-sistema-de-exibi%C3%A7%C3%A3o-din%C3%A2mica)

---

### **III. FASE 2: PERSONAGEM E MOVIMENTO BÁSICO**

*Implementação do protagonista e controles fundamentais*

#### **3. Sistema de Personagem**

- [x] [3.1 Estrutura de GameObjects](#31-estrutura-de-gameobjects)
- [x] [3.2 Regras de Visibilidade](#32-regras-de-visibilidade)
- [x] [3.3 Componentes Obrigatórios](#33-componentes-obrigat%C3%B3rios)


#### **4. Sistema de Movimento**

- [x] [4.1 Input Actions](#41-input-actions)
- [x] [4.2 Configurações de Física](#42-configura%C3%A7%C3%B5es-de-f%C3%ADsica)


#### **5. Sistema de Animação**

- [x] [5.1 Parâmetros do Animator](#51-par%C3%A2metros-do-animator)
- [x] [5.2 Regras dos Parâmetros](#52-regras-dos-par%C3%A2metros)

---

### **IV. FASE 3: MECÂNICAS BÁSICAS DE PROGRESSÃO**

*Sistemas que permitem o jogador progredir e evoluir*

#### **6. Sistema de Absorção Elemental**

- [x] [6.1 Tipos de Energia](#61-tipos-de-energia)
- [x] [6.2 Fragmentos Elementais (Prefabs)](#62-fragmentos-elementais-prefabs)
- [x] [6.3 Implementação do Sistema Elemental](#63-implementa%C3%A7%C3%A3o-do-sistema-elemental)


#### **7. Sistema de Crescimento**

- [ ] [7.1 Estágios de Evolução](#71-est%C3%A1gios-de-evolu%C3%A7%C3%A3o)
- [ ] [7.2 Eventos de Crescimento](#72-eventos-de-crescimento)


#### **8. Sistema de Inventário**

- [x] [8.1 Estrutura de Slots](#81-estrutura-de-slots)
- [x] [8.2 Sistema de Uso](#82-sistema-de-uso)

---

### **V. FASE 4: SISTEMA DE COMBATE E INTERAÇÃO**

*Mecânicas de ação e interação com o mundo*

#### **9. Sistema de Combate**

- [ ] [9.1 Tipos de Ataque](#91-tipos-de-ataque)
- [ ] [9.2 Sistema de Dano](#92-sistema-de-dano)


#### **10. Sistema de Objetos Interativos**

- [ ] [10.1 Estrutura e Configuração](#101-estrutura-e-configura%C3%A7%C3%A3o)
- [ ] [10.2 Sistema de Feedback Visual](#102-sistema-de-feedback-visual)
- [ ] [10.3 Objetos Destrutíveis](#103-objetos-destrut%C3%ADveis)
- [ ] [10.4 Objetos Móveis](#104-objetos-m%C3%B3veis)

---

### **VI. FASE 5: SISTEMAS DE NARRATIVA E NAVEGAÇÃO**

*Comunicação com jogador e movimentação pelo mundo*

#### **11. Sistema de Diálogos**

- [ ] [11.1 Interface de Diálogo](#111-interface-de-di%C3%A1logo)
- [ ] [11.2 Sistema de Carregamento de Texto](#112-sistema-de-carregamento-de-texto)
- [ ] [11.3 Animações e Transições](#113-anima%C3%A7%C3%B5es-e-transi%C3%A7%C3%B5es)


#### **12. Sistema de Portal**

- [ ] [12.1 Tipos de Portal e Configuração](#121-tipos-de-portal-e-configura%C3%A7%C3%A3o)
- [ ] [12.2 Sistema de Ativação](#122-sistema-de-ativa%C3%A7%C3%A3o)
- [ ] [12.3 Teleporte Intra-Cena e Inter-Cena](#123-teleporte-intra-cena-e-inter-cena)


#### **13. Sistema de Movimento Especial**

- [ ] [13.1 Encolher e Esgueirar](#131-encolher-e-esgueirar)
- [ ] [13.2 Sistema de Pular](#132-sistema-de-pular)

---

### **VII. FASE 6: SISTEMAS AVANÇADOS DE GAMEPLAY**

*Mecânicas mais complexas que dependem de múltiplos sistemas*

#### **14. Sistema de Stealth**

- [ ] [14.1 Estados de Visibilidade](#141-estados-de-visibilidade)
- [ ] [14.2 Objetos de Cobertura](#142-objetos-de-cobertura)


#### **15. Sistema de UI**

- [ ] [15.1 HUD Elements](#151-hud-elements)
- [ ] [15.2 Configurações de Canvas](#152-configura%C3%A7%C3%B5es-de-canvas)


#### **16. Sistema de Seguidores e Mini-Slimes**

- [ ] [16.1 Gestão de Seguidores](#161-gest%C3%A3o-de-seguidores)
- [ ] [16.2 IA de Companheiros](#162-ia-de-companheiros)
- [ ] [16.3 Sistema de Mini-Slimes](#163-sistema-de-mini-slimes)

---

### **VIII. FASE 7: POLISH E SISTEMAS DE SUPORTE**

*Sistemas que podem ser implementados em paralelo e adicionam polish*

#### **17. Sistema de Áudio**

- [ ] [17.1 Triggers Sonoros](#171-triggers-sonoros)
- [ ] [17.2 Mixagem Dinâmica](#172-mixagem-din%C3%A2mica)


#### **18. Sistema de Efeitos Visuais**

- [ ] [18.1 Partículas](#181-part%C3%ADculas)
- [ ] [18.2 Pós-Processamento](#182-p%C3%B3s-processamento)
- [ ] [18.3 Shaders Customizados](#183-shaders-customizados)

---

### **IX. FASE 8: SISTEMA DE PERSISTÊNCIA**

*Deve ser implementado por último pois depende de todos os outros sistemas*

#### **19. Sistema de Persistência**

- [ ] [19.1 Save Data Structure](#191-save-data-structure)
- [ ] [19.2 Auto-Save Triggers](#192-auto-save-triggers)
- [ ] [19.3 Loading States](#193-loading-states)

---

### **X. OBSERVAÇÕES DE IMPLEMENTAÇÃO**

- [Teste e Validação por Fase](#teste-e-valida%C3%A7%C3%A3o-por-fase)
- [Convenções de Nomenclatura](#conven%C3%A7%C3%B5es-de-nomenclatura)
- [Performance Guidelines](#performance-guidelines)
- [Debug Tools](#debug-tools)

---

### **XI. CONCLUSÃO**

- [Resumo da Implementação](#conclus%C3%A3o)

---

## Introdução Geral

Este documento estabelece as diretrizes técnicas fundamentais para a implementação de **The Slime King**, organizadas em uma sequência lógica de desenvolvimento que respeita as dependências entre sistemas e permite implementação incremental e testável.

O objetivo é fornecer um roadmap claro para a equipe de desenvolvimento, onde cada fase constrói sobre as anteriores, garantindo que funcionalidades básicas estejam sólidas antes de adicionar complexidade.

---

## **IMPLEMENTAÇÕES DETALHADAS**

Esta seção fornece uma documentação detalhada da implementação técnica de cada um dos sistemas marcados como concluídos neste documento, incluindo arquitetura, fluxo de trabalho, componentes principais e exemplos de uso.

### **Sistema de Localização de Textos**

**Visão Geral:** O sistema de localização permite que todos os textos do jogo sejam exibidos em múltiplos idiomas, com suporte para detecção automática do idioma do sistema e preferências do usuário.

#### Componentes Principais:

1. **LocalizationManager.cs** - Singleton responsável por gerenciar a localização:
   - Carrega o arquivo CSV de localização
   - Mantém um dicionário de chave-valor para textos
   - Fornece métodos para obtenção de textos traduzidos
   - Implementa a lógica de fallback de idiomas

2. **LocalizedText.cs** - Componente para UI que exibe texto localizado:
   - Mantém referência à chave de localização
   - Atualiza automaticamente o texto quando o idioma é alterado
   - Se conecta a componentes Text ou TextMeshProUGUI
   - Possui inspector customizado para facilitar a visualização de traduções

3. **LanguageMenu.cs** - Interface para o jogador escolher idioma:
   - Exibe opções de idiomas disponíveis
   - Salva a preferência do usuário no arquivo de configuração
   - Atualiza todos os textos localizados imediatamente

4. **LocalizationTester.cs** - Ferramenta para teste durante o desenvolvimento:
   - Permite alternar entre idiomas rapidamente para verificar traduções
   - Exibe estatísticas de cobertura de tradução

#### Ferramentas de Editor:

1. **LocalizationEditor.cs** - Janela de editor para gerenciar o arquivo CSV:
   - Interface visual para adicionar/editar/remover entradas
   - Validação automática para identificar chaves duplicadas ou entradas vazias
   - Suporte para importar/exportar CSVs
   - Destaque visual para entradas incompletas

2. **ExtrasMenu.cs** - Adiciona menus na barra de ferramentas do Unity:
   - Menu "The Slime King > Localização > Verificar CSV" para validar o arquivo
   - Menu "The Slime King > Localização > Adicionar Nova Entrada" para facilitar inclusão de novas chaves

#### Fluxo de Trabalho:

1. Os textos são definidos no arquivo `localization.csv` seguindo as convenções de nomenclatura
2. No início do jogo, o `LocalizationManager` é inicializado e carrega o arquivo
3. O idioma é definido seguindo a hierarquia de detecção descrita na seção 1.2
4. Os componentes `LocalizedText` registram-se no `LocalizationManager` para receber atualizações
5. Quando o idioma é alterado, todos os textos são atualizados automaticamente

#### Exemplo de Uso:

```csharp
// Obter um texto traduzido diretamente
string mensagem = LocalizationManager.Instance.GetText("ui_welcome_message");

// Definir o idioma manualmente
LocalizationManager.Instance.SetLanguage("PT_BR");

// Verificar o idioma atual
string currentLang = LocalizationManager.Instance.CurrentLanguage;
```

### **Sistema de Ícone Superior**

**Visão Geral:** Sistema que exibe ícones de interação acima de objetos interativos, adaptando-se ao dispositivo de entrada atual (teclado/mouse, gamepad Xbox, PlayStation, etc.).

#### Componentes Principais:

1. **IconManager.cs** - Singleton responsável por gerenciar os ícones:
   - Detecta o dispositivo de entrada atual
   - Mantém registro de ícones ativos
   - Gerencia animações de fade-in/fade-out
   - Atualiza ícones quando o dispositivo muda

2. **InteractionIconTrigger.cs** - Componente para ativar ícones:
   - Detecta proximidade do jogador
   - Define o tipo de interação (E, Space, A, B, etc.)
   - Controla exibição do ícone com base no contexto
   - Gerencia prioridade entre múltiplos ícones

3. **DeviceDetector.cs** - Responsável por detectar o dispositivo atual:
   - Monitora continuamente mudanças de input
   - Identifica tipo preciso do controle (Xbox, PlayStation, Switch, etc.)
   - Dispara eventos quando o dispositivo muda

#### Prefabs e Assets:

1. **IconContainer.prefab** - Container para os ícones:
   - Sistema de posicionamento acima do objeto
   - Animador para efeitos de fade-in/out
   - Suporte para seguir objetos em movimento

2. **Input Icons** - Conjunto de sprites para cada dispositivo:
   - Ícones de teclado (E, Space, Q, etc.)
   - Ícones Xbox (A, B, X, Y, etc.)
   - Ícones PlayStation (Cross, Circle, Square, Triangle, etc.)
   - Ícones Switch (A, B, X, Y, etc.)
   - Ícones genéricos para controladores não identificados

#### Fluxo de Trabalho:

1. Os objetos interativos possuem o componente `InteractionIconTrigger`
2. Quando o jogador se aproxima, o componente detecta e solicita um ícone ao `IconManager`
3. O `IconManager` instancia o ícone apropriado para o dispositivo atual
4. O ícone aparece com animação de fade-in e segue a posição do objeto
5. Se o jogador se afasta ou o objeto fica desativado, o ícone desaparece com fade-out
6. Se o dispositivo de input muda, os ícones são atualizados instantaneamente

#### Exemplo de Uso:

```csharp
// Em um objeto interativo
[SerializeField] private string _keyboardKey = "E";
[SerializeField] private string _gamepadButton = "A";

// Para mostrar um ícone
IconManager.Instance.ShowIcon(transform, _keyboardKey, _gamepadButton);

// Para esconder o ícone
IconManager.Instance.HideIcon(transform);
```

### **Sistema de Personagem**

**Visão Geral:** Define a estrutura e comportamento do protagonista (Slime), gerenciando sua representação visual, movimentação e interações.

#### Componentes Principais:

1. **SlimeInputHandler.cs** - Gerencia inputs do jogador:
   - Processa inputs do novo Input System
   - Distribui comandos para os sistemas relevantes
   - Adapta-se às diferentes plataformas
   - Controla estados que desabilitam inputs (diálogo, cutscene, etc.)

2. **SlimeMovement.cs** - Controla o movimento do personagem:
   - Implementa física baseada em Rigidbody2D
   - Gerencia velocidade, aceleração e drag
   - Adapta parâmetros conforme estágios de crescimento
   - Implementa colisões e interações físicas

3. **SlimeVisualController.cs** - Gerencia aparência visual:
   - Controla sprites direcionais (front, back, side)
   - Implementa sistema de flip para movimentos laterais
   - Gerencia visibilidade de componentes visuais
   - Coordena efeitos visuais e feedback

4. **SlimeAnimationController.cs** - Controla as animações:
   - Gerencia transições do Animator
   - Sincroniza estados de animação com movimento
   - Implementa triggers para ações especiais
   - Controla parâmetros de animação

5. **SlimeInteractionController.cs** - Gerencia interações:
   - Detecta objetos interativos próximos
   - Processa ações de interação
   - Coordena feedback para o jogador
   - Gerencia prioridades entre múltiplas interações possíveis

#### Estrutura de GameObjects:

A hierarquia do objeto "slimeBaby" foi implementada conforme especificado:
```
slimeBaby
├── back          // Sprite para movimento norte
├── vfx_back      // Efeitos visuais traseiros
├── vfx_front     // Efeitos visuais frontais
├── front         // Sprite para movimento sul
├── side          // Sprite para movimento leste/oeste
├── vfx_side      // Efeitos visuais laterais
└── shadow        // Sombra sempre visível
```

#### Fluxo de Trabalho:

1. `SlimeInputHandler` recebe inputs do jogador
2. Os inputs são convertidos em intenções (mover, interagir, atacar)
3. `SlimeMovement` atualiza a posição com base na intenção de movimento
4. `SlimeVisualController` atualiza sprites visíveis com base na direção
5. `SlimeAnimationController` sincroniza animações com o estado atual
6. `SlimeInteractionController` processa interações quando ativadas

#### Exemplo de Uso:

```csharp
// Em SlimeInputHandler.cs
private void OnMove(InputAction.CallbackContext context)
{
    Vector2 moveInput = context.ReadValue<Vector2>();
    _slimeMovement.SetMoveDirection(moveInput);
    _slimeAnimationController.SetIsWalking(moveInput.magnitude > 0.1f);
    _slimeVisualController.UpdateVisibleSprites(moveInput);
}

// Em SlimeInteractionController.cs
public void TryInteract()
{
    if (_nearbyInteractables.Count > 0)
    {
        _nearbyInteractables[0].Interact(_slimeTransform);
    }
}
```

### **Sistema de Movimento**

**Visão Geral:** Implementa o movimento do jogador usando o novo Input System da Unity, com suporte a múltiplos dispositivos e configurações físicas que evoluem conforme o crescimento do slime.

#### Componentes Principais:

1. **SlimeMovement.cs** - Núcleo do sistema de movimento:
   - Implementa movimento baseado em forças físicas
   - Gerencia aceleração e desaceleração suaves
   - Controla limites de velocidade e comportamento físico
   - Implementa estados especiais (congelado, em cutscene, etc.)

2. **InputSystem_Actions.inputactions** - Asset de configuração do Input System:
   - Define mapeamentos para teclado e controles
   - Implementa ações para movimento, interação, ataque, etc.
   - Configura chaves e botões conforme a documentação
   - Permite remapeamento em runtime

#### Configurações Físicas:

Para cada estágio de evolução, os parâmetros físicos foram implementados conforme especificado:

| Propriedade | Baby Slime | Young Slime | Adult Slime | Elder Slime |
| :-- | :-- | :-- | :-- | :-- |
| Velocidade Base | 3.0 | 4.0 | 4.5 | 5.0 |
| Massa | 0.5 | 1.0 | 1.5 | 2.0 |
| Drag Linear | 5.0 | 4.0 | 3.0 | 2.0 |
| Freeze Rotation | true | true | true | true |

#### Mapeamento de Input:

O sistema implementa o mapeamento universal de ações conforme especificado:

| Ação | Função | Teclado | Gamepad PC (Xbox) |
| :-- | :-- | :-- | :-- |
| Move | Movimento em 8 direções | WASD / Arrows | Left Stick |
| Attack | Ataque básico | Space | B |
| Interact | Interagir com objetos | E | A |
| Crouch | Stealth/esconder | Q | X |
| Use Item | Usar item selecionado | Left Alt | Y |
| Change Item | Alternar item selecionado | Scroll Mouse / Tab | D-pad (qualquer direção) |
| Ability 1-4 | Habilidades elementais | 1-4 | LB, LT, RB, RT |
| Menu | Menu principal | Enter | Menu |
| Inventory | Menu de inventário | Right Shift | View |

#### Fluxo de Trabalho:

1. O Input System captura entradas do dispositivo ativo
2. O `SlimeInputHandler` recebe callbacks do Input System
3. Comandos de movimento são enviados para o `SlimeMovement`
4. O `SlimeMovement` aplica forças ao Rigidbody2D
5. O drag e massa afetam como o personagem acelera e desacelera
6. O sistema visual e de animação responde às mudanças de movimento

#### Exemplo de Uso:

```csharp
// Configuração do sistema em Player
public class SlimeInputHandler : MonoBehaviour
{
    private PlayerInput _playerInput;
    private SlimeMovement _movement;
    
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _movement = GetComponent<SlimeMovement>();
        
        // Registrar callbacks
        _playerInput.actions["Move"].performed += OnMove;
        _playerInput.actions["Move"].canceled += OnMove;
    }
    
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        _movement.SetMoveDirection(moveInput);
    }
}
```

### **Sistema de Animação**

**Visão Geral:** Gerencia as animações do personagem com base em seu estado atual, usando um Animator complexo com diversos parâmetros e transições.

#### Componentes Principais:

1. **SlimeAnimationController.cs** - Coordenador de animações:
   - Gerencia parâmetros do Animator
   - Sincroniza animações com estados do jogo
   - Processa eventos de animação
   - Coordena feedbacks visuais e sonoros

2. **Animator Controllers** - Controladores para cada estágio:
   - Define transições entre estados
   - Implementa máquinas de estado para diferentes comportamentos
   - Configura layers de animação
   - Usa blend trees para animações direcionais

#### Parâmetros do Animator:

Os parâmetros foram implementados conforme especificado:

- `isSleeping` (bool) - Estado de descanso do slime
- `isHiding` (bool) - Estado de stealth ativo
- `isWalking` (bool) - Movimento ativo
- `Shrink` (Trigger) - Animação de encolhimento para passar por espaços
- `Jump` (Trigger) - Animação de salto
- `Attack01` (Trigger) - Ataque básico
- `Attack02` (Trigger) - Ataque especial

#### Regras Importantes:

- **Estado isHiding**: Quando ativado, o movimento é completamente desabilitado
- **Transições de Estado**: Triggers como Jump e Attack têm prioridade e podem interromper outras animações
- **Sincronização**: Animações são sincronizadas com efeitos sonoros via Animation Events

#### Fluxo de Trabalho:

1. O `SlimeInputHandler` atualiza o `SlimeAnimationController` baseado no input
2. Os parâmetros do Animator são atualizados
3. O Animator gerencia as transições entre estados
4. Eventos de animação são enviados durante frames específicos
5. Efeitos visuais e sonoros são sincronizados com as animações
6. O `SlimeMovement` é notificado sobre estados que restringem movimento

#### Exemplo de Uso:

```csharp
// No SlimeAnimationController
public void SetIsWalking(bool walking)
{
    _animator.SetBool("isWalking", walking);
}

public void TriggerAttack()
{
    _animator.SetTrigger("Attack01");
}

public void SetIsHiding(bool hiding)
{
    _animator.SetBool("isHiding", hiding);
    
    // Comunica ao sistema de movimento para desabilitar movimento
    _slimeMovement.SetMovementEnabled(!hiding);
}
```

### **Sistema de Absorção Elemental**

**Visão Geral:** Sistema central para progressão do jogador, permitindo coletar e utilizar energia elemental de diferentes tipos, cada um oferecendo habilidades e benefícios distintos.

#### Componentes Principais:

1. **ElementalType.cs** - Enum que define os tipos elementais:
   - `None` - Valor padrão/nulo
   - `Earth` - Terra (defesa)
   - `Water` - Água (regeneração)
   - `Fire` - Fogo (ataque)
   - `Air` - Ar (velocidade)

2. **ElementalEvents.cs** - Sistema de eventos globais:
   - `OnFragmentAbsorbed` - Disparado quando um fragmento é absorvido
   - `OnElementalAbilityUsed` - Disparado quando uma habilidade é usada
   - `OnElementalThresholdReached` - Disparado quando um threshold de evolução é atingido

3. **ElementalEnergyManager.cs** - Singleton para gerenciamento de energia:
   - Mantém registro da energia por tipo elemental
   - Processa adição e consumo de energia
   - Verifica thresholds de crescimento
   - Gerencia efeitos passivos de cada elemento
   - Serializa/deserializa dados para save

4. **ElementalFragment.cs** - Representa fragmentos coletáveis:
   - Implementa flutuação e rotação visual
   - Detecta proximidade do jogador
   - Ativa atração magnética quando próximo
   - Controla a absorção quando coletado

5. **ElementalFragmentSpawner.cs** - Gera fragmentos no mundo:
   - Gerencia perfis de spawn configuráveis
   - Aplica características visuais baseadas no elemento
   - Controla distribuição e quantidade
   - Suporta spawn aleatório ou específico

6. **ElementalAbilityManager.cs** - Gerencia habilidades elementais:
   - Controla cooldowns e custos de energia
   - Verifica disponibilidade e requisitos
   - Ativa efeitos visuais e sonoros
   - Implementa efeitos específicos de habilidades

7. **ElementalEnergyUI.cs** - Interface para visualização:
   - Exibe barras para cada tipo elemental
   - Anima transições entre valores
   - Aplica feedbacks visuais para ganhos/consumos
   - Exibe ícones e valores numéricos

#### Configurações por Elemento:

| Elemento | Cor Principal | Cor Secundária | Habilidade | Efeito Passivo |
| :-- | :-- | :-- | :-- | :-- |
| Terra | \#8B4513 | \#DEB887 | Quebrar rochas | +1 Defense/10pts |
| Água | \#4169E1 | \#87CEEB | Nadar/crescer plantas | +1 Regeneração/10pts |
| Fogo | \#FF4500 | \#FFA500 | Iluminar/derreter | +1 Attack/10pts |
| Ar | \#E6E6FA | \#F0F8FF | Planar/ativar eólicos | +1 Speed/10pts |

#### Tamanhos de Fragmentos:

- **Small**: 1 ponto de energia, comuns
- **Medium**: 3 pontos de energia, médios
- **Large**: 7 pontos de energia, raros

#### Ferramentas de Editor:

1. **ElementalSystemEditor.cs** - Janela de editor:
   - Interface para testar o sistema
   - Visualização em tempo real de valores
   - Ferramentas para debug e ajustes
   - Validação de configuração

#### Fluxo de Trabalho:

1. Fragmentos elementais são gerados no mundo
2. O jogador se aproxima e os fragmentos são atraídos magneticamente
3. Ao coletar, a energia é adicionada ao tipo correspondente
4. A UI atualiza mostrando o novo valor com animações
5. Efeitos passivos são aplicados baseados na energia acumulada
6. O jogador pode usar habilidades gastando energia
7. Quando thresholds são atingidos, o sistema de crescimento é notificado

#### Exemplo de Uso:

```csharp
// Adicionar energia manualmente
ElementalEnergyManager.Instance.AddElementalEnergy(ElementalType.Fire, 10);

// Verificar se há energia suficiente
int currentWaterEnergy = ElementalEnergyManager.Instance.GetElementalEnergy(ElementalType.Water);

// Usar uma habilidade elemental
ElementalAbilityManager.Instance.UseAbility("water_splash");

// Gerar fragmentos no mundo
ElementalFragmentSpawner.SpawnConfig config = new ElementalFragmentSpawner.SpawnConfig
{
    elementType = ElementalType.Earth,
    minAmount = 3,
    maxAmount = 5,
    randomElement = false,
    profile = ElementalFragmentSpawner.SpawnProfile.Balanced
};
fragmentSpawner.SpawnFragments(transform.position, config);
```

---

## **FASE 1: FUNDAMENTOS TÉCNICOS**

*Sistemas essenciais que servem como base para todos os outros*

### 1. **Sistema de Localização de Textos**

O sistema de localização deve ser implementado primeiro pois todos os outros sistemas dependem dele para textos e interface.

#### 1.1 Estrutura de Arquivos CSV

**Organização dos Arquivos:**
Todos os textos do jogo devem ser armazenados em arquivos CSV localizados no diretório `/Assets/StreamingAssets/Localization/`. A estrutura deve seguir o formato:


| Key | EN | PT_BR | ES | FR | DE | JA | ZH_CN |
| :-- | :-- | :-- | :-- | :-- | :-- | :-- | :-- |
| ui_start_game | Start Game | Iniciar Jogo | Iniciar Juego | Commencer | Spiel Starten | ゲーム開始 | 开始游戏 |
| ui_continue | Continue | Continuar | Continuar | Continuer | Weiter | 続ける | 继续 |

**Convenções de Nomenclatura:**

- **Interface**: `ui_[categoria]_[elemento]` (ex: `ui_menu_start`, `ui_hud_health`)
- **Diálogos**: `dialog_[npc]_[contexto]_[numero]` (ex: `dialog_fairy_greeting_01`)
- **Descrições**: `desc_[objeto]_[tipo]` (ex: `desc_item_apple`, `desc_ability_fire`)
- **Mensagens**: `msg_[categoria]_[tipo]` (ex: `msg_system_save_complete`)


#### 1.2 Detecção Automática de Idioma

**Hierarquia de Detecção:**

1. **Configuração do Usuário**: Verificar config.json para preferência explícita do jogador
2. **Idioma do Sistema**: Detectar idioma configurado no dispositivo do usuário
3. **Fallback Regional**: Se idioma específico não disponível, usar variante regional próxima
4. **Fallback Global**: Se nenhuma opção anterior funcionar, usar Inglês (EN) como padrão

#### 1.3 Sistema de Configuração

O sistema deve utilizar arquivo `config.json` localizado na pasta de dados persistentes do jogo para armazenar todas as configurações, incluindo preferência de idioma do jogador.

**Estrutura do config.json:**

- **gameSettings**: Configurações gerais do jogo
- **preferredLanguage**: Idioma escolhido pelo jogador (sobrescreve detecção automática)
- **audioSettings**: Configurações de volume por categoria
- **videoSettings**: Configurações de resolução, fullscreen, etc.
- **accessibilitySettings**: Opções de acessibilidade e escala de UI


### 2. **Sistema de Ícone Superior**

Sistema fundamental para feedback visual de interações, deve ser implementado cedo pois muitos outros sistemas dependem dele.

#### 2.1 Detecção de Plataforma

**Dispositivos Suportados:**

- **Keyboard**: Ícones de teclas específicas (E, Space, Q, etc.)
- **Xbox Gamepad**: Ícones de botões Xbox (A, B, X, Y, LB, RB, etc.)
- **PlayStation Controller**: Ícones de botões PlayStation (Cross, Circle, Square, Triangle, L1, R1, etc.)
- **Nintendo Switch**: Ícones de botões Switch (A, B, X, Y, L, R, etc.)
- **Generic Gamepad**: Ícones genéricos quando controlador específico não é reconhecido

**Sistema de Detecção Automática:**
O sistema deve monitorar continuamente o tipo de input ativo e trocar automaticamente os ícones quando o jogador alterna entre dispositivos.

#### 2.2 Sistema de Exibição Dinâmica

**Comportamento de Exibição:**
Os ícones superiores devem aparecer automaticamente quando o slime está próximo o suficiente para interagir com um objeto. O ícone deve:

- Aparecer com animação suave de fade-in
- Permanecer visível enquanto interação é possível
- Desaparecer com fade-out quando slime se afasta
- Trocar instantaneamente quando dispositivo de input muda

---

## **FASE 2: PERSONAGEM E MOVIMENTO BÁSICO**

*Implementação do protagonista e controles fundamentais*

### 3. **Sistema de Personagem**

Base técnica do jogo, deve ser implementado após os sistemas fundamentais.

#### 3.1 Estrutura de GameObjects

O objeto **SlimeBaby** deve ser estruturado seguindo a hierarquia:

```
slimeBaby
├── back          // Sprite para movimento norte
├── vfx_back      // Efeitos visuais traseiros
├── vfx_front     // Efeitos visuais frontais
├── front         // Sprite para movimento sul
├── side          // Sprite para movimento leste/oeste
├── vfx_side      // Efeitos visuais laterais
└── shadow        // Sombra sempre visível
```


#### 3.2 Regras de Visibilidade

O sistema de visibilidade direcional deve seguir regras específicas:

- **No start do jogo**: somente os objetos com **front** no nome e **shadow** devem estar visíveis
- **Ao se deslocar para o norte**: somente os objetos com **back** e **shadow** no nome devem estar visíveis
- **Quando se deslocar para as laterais**: somente os objetos com **side** e **shadow** no nome devem estar visíveis
- **Quando se deslocar para a esquerda**: deve-se realizar um flip horizontal no sprite


#### 3.3 Componentes Obrigatórios

| Componente | Função | Configuração Detalhada |
| :-- | :-- | :-- |
| `PlayerMovement` | Controle de movimento e física | Deve incluir Rigidbody2D com configurações específicas por estágio, Collider2D para detecção de colisões |
| `PlayerGrowth` | Sistema de evolução | Array de sprites organizados por direção e estágio, sistema de triggers para evolução |
| `PlayerVisualManager` | Controle de visibilidade direcional | Referências para todos os GameObjects direcionais, lógica de ativação/desativação |
| `ElementalEnergyManager` | Armazenamento de energia | Barras de energia por elemento com valores máximos configuráveis |
| `InventoryManager` | Sistema de inventário evolutivo | Slots que expandem de 1-4 conforme crescimento |

### 4. **Sistema de Movimento**

Implementar após o sistema de personagem estar funcional.

#### 4.1 Input Actions

**Mapeamento Universal por Função**


| Ação | Função | Teclado | Gamepad PC (Xbox) |
| :-- | :-- | :-- | :-- |
| Move | Movimento em 8 direções | WASD / Arrows | Left Stick |
| Attack | Ataque básico | Space | B |
| Interact | Interagir com objetos | E | A |
| Crouch | Stealth/esconder | Q | X |
| Use Item | Usar item selecionado | Left Alt | Y |
| Change Item | Alternar item selecionado | Scroll Mouse / Tab | D-pad (qualquer direção) |
| Ability 1-4 | Habilidades elementais | 1-4 | LB, LT, RB, RT |
| Menu | Menu principal | Enter | Menu |
| Inventory | Menu de inventário | Right Shift | View |

#### 4.2 Configurações de Física

O sistema de física deve ser calibrado para cada estágio de crescimento:


| Propriedade | Baby Slime | Young Slime | Adult Slime | Elder Slime |
| :-- | :-- | :-- | :-- | :-- |
| Velocidade Base | 3.0 | 4.0 | 4.5 | 5.0 |
| Massa | 0.5 | 1.0 | 1.5 | 2.0 |
| Drag Linear | 5.0 | 4.0 | 3.0 | 2.0 |
| Freeze Rotation | true | true | true | true |

### 5. **Sistema de Animação**

Implementar após movimento estar funcional para dar vida ao personagem.

#### 5.1 Parâmetros do Animator

O sistema deve utilizar **7 parâmetros** específicos:

- `isSleeping` (bool) - Estado de descanso do slime
- `isHiding` (bool) - Estado de stealth ativo
- `isWalking` (bool) - Movimento ativo
- `Shrink` (Trigger) - Animação de encolhimento para passar por espaços
- `Jump` (Trigger) - Animação de salto
- `Attack01` (Trigger) - Ataque básico
- `Attack02` (Trigger) - Ataque especial


#### 5.2 Regras dos Parâmetros

- **isHiding**: Ativado com `InputAction.Crouch`. **IMPORTANTE**: Durante este estado, o movimento do slime deve ser completamente desabilitado
- **isWalking**: Setado como `true` ao se deslocar para qualquer direção
- **Attack01**: Ativado com `InputAction.Attack`
- **Attack02**: Ativado com `InputAction.Special`
- **Shrink e Jump**: Ativados com `InputAction.Interact`, dependendo do objeto de interação

---

## **FASE 3: MECÂNICAS BÁSICAS DE PROGRESSÃO**

*Sistemas que permitem o jogador progredir e evoluir*

### 6. **Sistema de Absorção Elemental**

Mecânica central de progressão, deve ser implementada cedo.

#### 6.1 Tipos de Energia

| Elemento | Cor Principal | Cor Secundária | Habilidade | Efeito Passivo Base |
| :-- | :-- | :-- | :-- | :-- |
| Terra | \#8B4513 | \#DEB887 | Quebrar rochas | +1 Defense a cada 10 pontos |
| Água | \#4169E1 | \#87CEEB | Nadar/crescer plantas | +1 Regeneração a cada 10 pontos |
| Fogo | \#FF4500 | \#FFA500 | Iluminar/derreter | +1 Attack a cada 10 pontos |
| Ar | \#E6E6FA | \#F0F8FF | Planar/ativar eólicos | +1 Speed a cada 10 pontos |

#### 6.2 Fragmentos Elementais (Prefabs)

**Sistema de Variantes de Sprite:**
Cada fragmento elemental deve possuir exatamente três sprites diferentes:

- **Small Fragment**: Menor tamanho, mais comum, 1 ponto de energia
- **Medium Fragment**: Tamanho intermediário, frequência média, 3 pontos de energia
- **Large Fragment**: Maior tamanho, mais raro, 7 pontos de energia

**Sistema de Cores Dinâmicas:**
Durante a instanciação, o sistema deve receber dois parâmetros de cor (colorA e colorB) e calcular cor final usando interpolação com valor aleatório.

**Configuração de Drop por Objeto:**

- **Tamanhos Permitidos**: Array definindo quais tamanhos podem ser dropados
- **Chances de Drop**: Probabilidade específica para cada tamanho
- **Quantidade**: Número mínimo e máximo de fragmentos
- **Elemento**: Tipo elemental ou configuração para drop aleatório

#### 6.3 Implementação do Sistema Elemental

**Classes Principais:**

1. **ElementalType (enum)**: Define os tipos de energia elemental disponíveis no jogo.
   - `None`: Valor padrão/nulo
   - `Earth`: Terra (defesa)
   - `Water`: Água (regeneração)
   - `Fire`: Fogo (ataque)
   - `Air`: Ar (velocidade)

2. **ElementalEvents (static class)**: Gerencia eventos globais relacionados ao sistema elemental.
   - `OnFragmentAbsorbed`: Disparado quando um fragmento é absorvido
   - `OnElementalAbilityUsed`: Disparado quando uma habilidade elemental é usada
   - `OnElementalThresholdReached`: Disparado quando um threshold de crescimento é alcançado

3. **ElementalEnergyManager (MonoBehaviour/Singleton)**: Gerencia o armazenamento e uso de energia elemental.
   - Mantém um mapeamento de energia atual por tipo elemental
   - Rastreia energia total absorvida para progressão
   - Fornece métodos para adicionar/usar energia
   - Verifica thresholds de crescimento
   - Gerencia efeitos passivos de cada elemento
   - Implementa salvamento/carregamento de dados

4. **ElementalFragment (MonoBehaviour)**: Representa um fragmento elemental coletável.
   - Define propriedades visuais e físicas
   - Implementa comportamento de flutuação e rotação
   - Gerencia detecção e atração para o jogador
   - Aplica cores dinâmicas baseado no tipo elemental
   - Controla a absorção quando coletado pelo jogador

5. **ElementalFragmentSpawner (MonoBehaviour)**: Sistema para gerar fragmentos elementais.
   - Possui prefabs de referência para cada tamanho
   - Gerencia configurações de spawn
   - Permite geração de múltiplos fragmentos em padrões específicos
   - Aplica características visuais baseado no tipo elemental

6. **ElementalFragmentConfig (ScriptableObject)**: Configuração de fragmentos elementais.
   - Define sprites e cores para cada tipo e tamanho
   - Gerencia configurações de drop por tipo de objeto
   - Permite personalização de chances por tamanho
   - Configura o comportamento de geração

7. **ElementalAbilityManager (MonoBehaviour)**: Gerencia habilidades elementais do jogador.
   - Mantém lista de habilidades disponíveis
   - Gerencia cooldowns globais e específicos
   - Verifica disponibilidade de energia antes de usar habilidades
   - Aplica efeitos visuais e sonoros quando habilidades são ativadas

8. **ElementalEnergyUI (MonoBehaviour)**: Interface de usuário para visualização de energia.
   - Exibe barras para cada tipo elemental
   - Mostra valores atuais e capacidade máxima
   - Implementa animações para ganho/uso de energia
   - Feedback visual para absorção de fragmentos

**Fluxo de Absorção de Energia:**

1. Um `ElementalFragment` é gerado no mundo via `ElementalFragmentSpawner`
2. O jogador se aproxima e o fragmento é atraído magneticamente
3. Quando coletado, o fragmento dispara `ElementalEvents.OnFragmentAbsorbed`
4. `ElementalEnergyManager` recebe o evento e adiciona a energia apropriada
5. `ElementalEnergyUI` é notificado e atualiza a interface visual
6. Efeitos passivos são aplicados baseado na energia acumulada
7. Quando um threshold é atingido, evento de crescimento é disparado

**Efeitos Passivos Elementais:**

- **Terra**: Aumenta defesa, calculado como energia Terra ÷ 10
- **Água**: Aumenta regeneração de vida, calculado como energia Água ÷ 10
- **Fogo**: Aumenta dano de ataque, calculado como energia Fogo ÷ 10
- **Ar**: Aumenta velocidade de movimento, calculado como energia Ar ÷ 10

**Ferramentas de Editor:**

- `ElementalSystemEditor`: Janela de editor para gerenciar e testar o sistema elemental
  - Visualizar valores de energia atual
  - Adicionar energia para testes
  - Gerar fragmentos de teste na cena
  - Checar problemas na configuração do sistema


### 7. **Sistema de Crescimento**

O sistema de crescimento implementado gerencia a evolução do protagonista ao longo do jogo conforme acumula energia elemental.

#### 7.1 Estágios de Evolução

| Estágio | Energia Necessária | Slots Inventário | Seguidores Máx | Mini-Slimes Máx |
| :-- | :-- | :-- | :-- | :-- |
| Baby Slime | 0 | 1 | 0 | 0 |
| Young Slime | 200 | 2 | 1 | 1 |
| Adult Slime | 600 | 3 | 3 | 2 |
| Elder/King | 1200 | 4 | 4 | 3 |

#### 7.2 Arquitetura do Sistema

O sistema é composto por três componentes principais:

1. **PlayerGrowth**: Singleton que gerencia a lógica central de crescimento
2. **SlimeGrowthStage**: ScriptableObject para configurar cada estágio
3. **SlimeGrowthVisuals**: Gerencia mudanças visuais durante o crescimento

**Estrutura de Classes:**

```csharp
// Classe principal que gerencia o crescimento do Slime
public class PlayerGrowth : MonoBehaviour
{
    // Eventos disparados durante o processo de crescimento
    public GrowthEvent OnGrowthStageChanged;
    public UnityEvent OnGrowthStarted;
    public UnityEvent OnGrowthCompleted;
    
    // Avalia e inicia a transição de estágio se necessário
    public void EvaluateGrowthStage(int stage = -1);
    
    // Obtém o estágio atual do Slime
    public SlimeStage GetCurrentStage();
    
    // Retorna a configuração do estágio atual
    public SlimeGrowthStage GetCurrentStageConfig();
}

// ScriptableObject que define propriedades de cada estágio
[CreateAssetMenu(fileName = "NewSlimeStage", menuName = "The Slime King/Slime Growth Stage")]
public class SlimeGrowthStage : ScriptableObject
{
    // Propriedades disponíveis para cada estágio
    public SlimeStage StageType { get; }
    public string StageName { get; }
    public int RequiredElementalEnergy { get; }
    public int InventorySlots { get; }
    public float SizeMultiplier { get; }
    public float SpeedMultiplier { get; }
    public bool CanSqueeze { get; }
    public bool CanBounce { get; }
    public bool CanClimb { get; }
}
```

#### 7.3 Eventos de Crescimento

Os eventos de crescimento seguem as especificações:

- **Trigger**: Energia elemental acumulada ≥ threshold
- **Efeito Visual**: Partículas especiais + screen flash suave
- **Efeito Sonoro**: Som harmônico ascendente
- **Duração**: 2 segundos de animação
- **Durante Crescimento**: Input desabilitado, invulnerabilidade temporária

**Fluxo de Crescimento:**

1. O sistema elemental detecta quando um threshold é atingido
2. ElementalEvents.OnElementalThresholdReached é disparado com o estágio atual
3. PlayerGrowth recebe o evento e inicia a transição visual
4. Durante a transição, controles são temporariamente desabilitados
5. Efeitos visuais e sonoros são reproduzidos
6. Ao concluir, as novas habilidades são desbloqueadas

**Código de Transição:**

```csharp
// Dentro da coroutine de transição de crescimento
private IEnumerator GrowthTransitionCoroutine(SlimeStage targetStage)
{
    // Notificar início e desabilitar input
    OnGrowthStarted?.Invoke();
    playerControls.enabled = false;
    
    // Efeitos visuais e sonoros
    PlayGrowthVFX(targetConfig);
    PlayGrowthSFX(targetConfig);
    
    // Animação gradual de crescimento
    float elapsedTime = 0f;
    while (elapsedTime < _growthTransitionDuration)
    {
        float t = _growthCurve.Evaluate(elapsedTime / _growthTransitionDuration);
        _slimeTransform.localScale = Vector3.Lerp(startSize, targetSize, t);
        elapsedTime += Time.deltaTime;
        yield return null;
    }
    
    // Atualiza estágio e aplica novas propriedades
    _currentStage = targetStage;
    ApplyStageProperties(targetConfig);
    
    // Notifica conclusão e reativa controle
    OnGrowthCompleted?.Invoke();
    OnGrowthStageChanged?.Invoke(_currentStage);
    playerControls.enabled = true;
}
```

#### 7.4 Integração com Outros Sistemas

O sistema de crescimento integra-se com diversos outros sistemas:

1. **Sistema Elemental**: Recebe eventos de threshold de energia para acionar crescimento
2. **Sistema de Inventário**: Expande slots disponíveis conforme o estágio
3. **Sistema de Seguidores**: Desbloqueia novos seguidores em estágios avançados
4. **Sistema de Colisão**: Ajusta tamanho dos colliders conforme o slime cresce
5. **Sistema de Status**: Atualiza atributos base (saúde, ataque, defesa)

**Exemplo de Integração com Inventário:**

```csharp
// Dentro de PlayerGrowth.ApplyStageProperties
private void ApplyStageProperties(SlimeGrowthStage stageConfig)
{
    // Atualiza slots de inventário disponíveis
    InventoryManager inventoryManager = InventoryManager.Instance;
    if (inventoryManager != null)
    {
        inventoryManager.SetAvailableSlots(stageConfig.InventorySlots);
    }
}
```

#### 7.5 Ferramentas de Desenvolvimento

Para auxiliar os designers e desenvolvedores, foram criadas ferramentas de editor:

1. **PlayerGrowthEditor**: Inspector customizado para configuração do sistema
2. **SlimeGrowthStageEditor**: Editor para criação e edição de estágios
3. **Modo Debug**: Permite forçar crescimento para testes durante o jogo


### 8. **Sistema de Inventário**

Sistema relativamente independente, pode ser implementado em paralelo com crescimento.

#### 8.1 Estrutura de Slots

| Estágio | Slots Disponíveis | Tipos Permitidos | Stack Máximo |
| :-- | :-- | :-- | :-- |
| Baby | 1 | Qualquer | 10 |
| Young | 2 | Qualquer | 10 |
| Adult | 3 | Qualquer | 10 |
| Elder | 4 | Qualquer | 10 |

#### 8.2 Sistema de Uso

O sistema deve otimizar para não interromper o fluxo de gameplay:

- **Navegação**: D-pad ou Change Item input para alternar slots
- **Uso**: Use Item input consome 1 unidade do item selecionado
- **Feedback**: Animação + efeito de partícula baseado no item
- **Auto-descarte**: Reorganização automática de slots vazios

---

## **FASE 4: SISTEMA DE COMBATE E INTERAÇÃO**

*Mecânicas de ação e interação com o mundo*

### 9. **Sistema de Combate**

Implementar após movimento e animação estarem sólidos.

#### 9.1 Tipos de Ataque

| Tipo | Input | Dano Base | Alcance | Cooldown | Efeito |
| :-- | :-- | :-- | :-- | :-- | :-- |
| Ataque Básico | Attack | 10 + Nível | 1.0 unity | 0.5s | Knockback |
| Dash Attack | Hold Attack | 15 + Nível | 2.0 unity | 1.0s | Movimento + Dano |
| Ataque Especial | Special | 20 + Especial | 1.5 unity | 2.0s | Efeito elemental |

#### 9.2 Sistema de Dano

O sistema deve utilizar fórmula: `realDamage = max(baseDamage - defense, 1)`
Garantindo que todo ataque cause pelo menos 1 de dano.

### 10. **Sistema de Objetos Interativos**

Base para todos os objetos do mundo que podem ser ativados.

#### 10.1 Estrutura e Configuração

**Componentes Básicos:**

- **Collider de Detecção**: Define área onde interação é possível
- **Script de Interação**: Gerencia comportamento quando ativado
- **Configuração Visual**: Define se usa outline, ícone superior, ou ambos
- **Configuração de Ação**: Especifica que ação acontece na interação

**Tipos de Interação:**

- **Ativação Simples**: Liga/desliga mecanismo ou objeto
- **Coleta**: Remove objeto e adiciona item ao inventário
- **Diálogo**: Inicia conversa com NPC ou examina objeto
- **Puzzle**: Ativa parte de quebra-cabeça maior
- **Portal**: Inicia sequência de teleporte


#### 10.2 Sistema de Feedback Visual

**Outline Visual:**
Quando slime está próximo, objetos interativos podem exibir contorno colorido que:

- Aparece gradualmente conforme aproximação
- Usa cor específica baseada no tipo de interação
- Pulsa suavemente para chamar atenção
- Desaparece quando slime se afasta


#### 10.3 Objetos Destrutíveis

**Atributos de Resistência:**

- **Defense (Defesa)**: Valor inteiro que reduz o dano recebido
- **MaxHP (Pontos de Vida)**: Quantidade total de dano que pode receber
- **Resistências Elementais**: Proteções específicas contra tipos elementais

**Sistema de Drop Configurável:**

- **Drop Table**: Lista de itens possíveis com suas respectivas chances
- **Fragment Configuration**: Configuração específica para fragmentos elementais
- **Rare Drops**: Itens especiais com baixa chance
- **Guaranteed Drops**: Itens que sempre são dropados


#### 10.4 Objetos Móveis

**Requisitos de Interação:**

- **Proximidade**: Slime deve estar a distância configurável (≤ 1.5 units)
- **Direcionamento**: Slime deve estar olhando para o objeto (≤ 45°)
- **Input de Interação**: Pressionar botão quando próximo e direcionado

**Sistema de Rastro Opcional:**

- **Trail Prefab**: GameObject que representa a marca deixada
- **Trail Spacing**: Distância entre cada instância do rastro
- **Trail Lifetime**: Tempo que rastro permanece no ambiente
- **Material Specific**: Diferentes rastros baseados na superfície

---

## **FASE 5: SISTEMAS DE NARRATIVA E NAVEGAÇÃO**

*Comunicação com jogador e movimentação pelo mundo*

### 11. **Sistema de Diálogos**

Implementar após localização e objetos interativos estarem funcionais.

#### 11.1 Interface de Diálogo

**Componentes da Interface:**

- **Caixa de Fundo**: Imagem de fundo semitransparente para legibilidade
- **Área de Texto**: Campo onde texto é exibido gradualmente
- **Campo de Título**: Mostra nome do personagem falante
- **Imagem do Falante**: Retrato opcional do NPC
- **Indicador de Continuação**: Seta indicando mais texto disponível


#### 11.2 Sistema de Carregamento de Texto

**Integração com Localização:**

- **Chaves Específicas**: Identificadores únicos para cada linha de diálogo
- **Carregamento Dinâmico**: Textos carregados na língua atual
- **Fallback Automático**: Inglês usado se tradução não disponível
- **Cache Inteligente**: Diálogos frequentes mantidos em memória


#### 11.3 Animações e Transições

**Animação de Texto:**

- **Velocidade Configurável**: Caracteres por segundo ajustável
- **Pausa em Pontuação**: Delay extra em vírgulas e pontos
- **Skip Disponível**: Pressionar botão completa texto imediatamente
- **Som de Digitação**: Efeito sonoro sutil para cada caractere


### 12. **Sistema de Portal**

Implementar após objetos interativos e diálogos estarem funcionais.

#### 12.1 Tipos de Portal e Configuração

**Tipos de Ativação:**

- **Por Toque**: Portal ativa automaticamente quando slime encosta
- **Por Interação**: Portal requer pressionar botão de interação

**Configuração de Destino:**

- **Tipo de Destino**: Mesmo cena ou cena diferente
- **Ponto de Destino**: Coordenadas exatas ou ID de outro portal
- **Cena de Destino**: Nome da cena alvo (para teleporte inter-cena)
- **Condições de Acesso**: Requisitos opcionais (estágio mínimo, itens)


#### 12.2 Sistema de Ativação

**Portal por Toque:**
Teleporte inicia automaticamente após breve delay para evitar ativação acidental.

**Portal por Interação:**

- Exibir Ícone Superior quando slime está próximo
- Ativar apenas quando botão de interação é pressionado
- Fornecer feedback visual claro sobre disponibilidade


#### 12.3 Teleporte Intra-Cena e Inter-Cena

**Teleporte na Mesma Cena:**

- Reproduzir efeito visual de desaparecimento no portal origem
- Mover instantaneamente o slime para posição de destino
- Reproduzir efeito visual de aparecimento no destino
- Temporariamente desabilitar portal de destino para evitar loop

**Teleporte Entre Cenas:**

- Salvar estado atual do jogo antes de trocar cena
- Carregar nova cena de forma assíncrona
- Posicionar slime no ponto correto da nova cena
- Restaurar estado de seguidores e inventário


### 13. **Sistema de Movimento Especial**

Implementar após animação e objetos interativos estarem funcionais.

#### 13.1 Encolher e Esgueirar

**Requisitos de Ativação:**

- **Proximidade**: Slime deve estar próximo ao ponto de passagem (≤ 1.5 units)
- **Direcionamento**: Slime deve estar olhando diretamente para o ponto (≤ 30°)
- **Input de Interação**: Pressionar botão quando posicionado corretamente
- **Ícone Superior**: Ponto deve exibir ícone de interação quando disponível

**Sequência de Encolhimento:**

1. Input de interação dispara animação Shrink
2. Slime se posiciona automaticamente no ponto de entrada
3. Slime se desloca suavemente até ponto de destino configurado
4. Tempo de deslocamento configurável por ponto
5. Slime retorna ao tamanho normal e recupera controle

#### 13.2 Sistema de Pular

**Requisitos de Ativação:**

- **Posicionamento**: Slime deve estar próximo ao ponto de pulo
- **Direcionamento**: Slime deve estar olhando para o destino
- **Verificação de Obstáculos**: Sistema verifica se trajetória está livre
- **Input de Interação**: Botão de interação dispara sequência

**Sequência de Pulo:**

1. Animação de preparação para salto
2. Slime se projeta em arco em direção ao destino
3. Movimento suave seguindo curva parabólica natural
4. Animação de aterrissagem com efeito visual
5. Breve pausa antes de recuperar controle total

---

## **FASE 6: SISTEMAS AVANÇADOS DE GAMEPLAY**

*Mecânicas mais complexas que dependem de múltiplos sistemas*

### 14. **Sistema de Stealth**

Implementar após movimento e animação estarem sólidos.

#### 14.1 Estados de Visibilidade

| Estado | Condição | Efeito Visual | Detectável | Movimento |
| :-- | :-- | :-- | :-- | :-- |
| Normal | Default | Sprite normal | Sim | Permitido |
| Crouched | Crouch pressionado | Sprite agachado | Não (se em cobertura) | **BLOQUEADO** |
| Hidden | Crouch + Cobertura | Vinheta escura | Não | **BLOQUEADO** |
| Exposed | Crouch sem cobertura | Sprite agachado | Sim | **BLOQUEADO** |

#### 14.2 Objetos de Cobertura

O sistema deve reconhecer objetos com tags: "Grass", "Bush", "Rock", "Tree"

- **Detecção**: OverlapCircle com raio 1.0 unit
- **Feedback**: Ícone de olho riscado quando escondido
- **Limitação**: Sem movimento durante stealth


### 15. **Sistema de UI**

Implementar após localização e funcionalidades básicas estarem prontas.

#### 15.1 HUD Elements

| Elemento | Posição | Conteúdo | Comportamento |
| :-- | :-- | :-- | :-- |
| Barra de Vida | Superior Esquerda | HP atual/máximo | Animação suave de mudança |
| Energia Elemental | Superior Direita | 4 barras coloridas | Pulse ao absorver |
| Inventário | Inferior Centro | 1-4 slots | Escala com crescimento |
| Seguidores | Inferior Esquerda | Ícones + status | Aparece quando tem seguidores |
| Crescimento | Superior Centro | Barra de progresso | Glow ao aproximar de evolução |

#### 15.2 Configurações de Canvas

**Configurações Técnicas:**

- UI Scale Mode: Scale With Screen Size
- Reference Resolution: 1920x1080
- Screen Match Mode: Match Width Or Height
- Match: 0.5


### 16. **Sistema de Seguidores e Mini-Slimes**

Sistema complexo que deve ser implementado após vários outros estarem funcionais.

#### 16.1 Gestão de Seguidores

| Tipo | Origem | Habilidade Única | Comandos |
| :-- | :-- | :-- | :-- |
| Pássaro | Quest resgate | Ativar switches altos | Seguir, Ficar, Ir Para |
| Coelho | Quest escolta | Passar por túneis | Seguir, Ficar, Escavar |
| Peixe | Quest limpeza | Controlar água | Seguir, Ficar, Nadar |
| Cristal | Quest reparo | Iluminar áreas | Seguir, Ficar, Brilhar |

#### 16.2 IA de Companheiros

**Estados da IA:**

- Following (seguindo em formação)
- Commanded (executando comando específico)
- Cooperative (participando de puzzle)
- Resting (recuperando energia)
- Distressed (preso ou em perigo)


#### 16.3 Sistema de Mini-Slimes

| Estágio | Quantidade | Duração | Custo Energia | Cooldown |
| :-- | :-- | :-- | :-- | :-- |
| Young | 1 | 45s | 30% | 45s |
| Adult | 2 | 60s | 25% | 30s |
| Elder | 3 | 90s | 20% | 20s |

**Especialização Elemental:**

- **Terra**: +25% peso, quebra obstáculos pequenos
- **Água**: Nada 2x mais rápido, não se afoga
- **Fogo**: Ilumina 3x maior área, derrete gelo
- **Ar**: Move-se 100% mais rápido, plana pequenas distâncias

---

## **FASE 7: POLISH E SISTEMAS DE SUPORTE**

*Sistemas que podem ser implementados em paralelo e adicionam polish*

### 17. **Sistema de Áudio**

Pode ser implementado em paralelo com outros sistemas.

#### 17.1 Triggers Sonoros

| Ação | Som | Volume | Pitch Variation |
| :-- | :-- | :-- | :-- |
| Movimento Slime | Squish suave | 0.3 | ±0.1 por tamanho |
| Absorção Fragmento | Tom musical | 0.5 | Por elemento |
| Crescimento | Harmonia ascendente | 0.8 | - |
| Ataque | Whoosh | 0.4 | ±0.2 |
| Comando Seguidor | Chirp/Call | 0.3 | Por tipo |

#### 17.2 Mixagem Dinâmica

**Estrutura Hierárquica:**

- Master
    - Music (0.7)
    - SFX (0.8)
        - Player (0.6)
        - Followers (0.4)
        - Environment (0.5)
    - UI (0.5)


### 18. **Sistema de Efeitos Visuais**

Pode ser implementado em paralelo, adiciona polish visual.

#### 18.1 Partículas

| Efeito | Trigger | Duração | Configuração |
| :-- | :-- | :-- | :-- |
| Absorção Elemental | Fragmento coletado | 1s | Trail + burst por cor |
| Crescimento | Evolution trigger | 2s | Espiral ascendente |
| Stealth | Enter/Exit hiding | 0.5s | Fade particles |
| Comando Seguidor | Command issued | 0.3s | Directional sparkles |

#### 18.2 Pós-Processamento

| Efeito | Uso | Intensidade | Trigger |
| :-- | :-- | :-- | :-- |
| Bloom | Elementos mágicos | 0.3 | Sempre ativo |
| Vignette | Stealth state | 0.2 | isHiding = true |
| Color Grading | Atmosfera regional | Variable | Por região |

#### 18.3 Shaders Customizados

- **Outline Shader**: Para destacar objetos interativos
- **Dissolve Shader**: Para efeitos de crescimento/transformação
- **Water Shader**: Para áreas aquáticas com movimento
- **Crystal Shader**: Para elementos cristalinos com refração

---

## **FASE 8: SISTEMA DE PERSISTÊNCIA**

*Deve ser implementado por último pois depende de todos os outros sistemas*

### 19. **Sistema de Persistência**

Sistema crítico que deve ser implementado após todos os outros estarem funcionais.

#### 19.1 Save Data Structure

**Estrutura de Dados:**

- **Player Data**: Estágio atual, posição, energia elemental, bônus passivos, inventário
- **World Data**: Objetos destruídos, quests completadas, seguidores ativos
- **Settings**: Volumes de áudio, escala de UI, preferências de idioma


#### 19.2 Auto-Save Triggers

O sistema deve salvar automaticamente em:

- **Crescimento**: Após evolução completa
- **Nova Região**: Ao entrar em nova área
- **Quest Completa**: Após ganhar novo seguidor
- **Tempo**: A cada 5 minutos de gameplay
- **Checkpoints**: Em pontos de descanso específicos


#### 19.3 Loading States

- **New Game**: Inicialização com valores padrão
- **Continue**: Carregamento do save mais recente
- **Load Specific**: Seleção de slot específico
- **Backup**: Sistema automático para prevenir perda

---

### **Observações de Implementação por Fase**

### **Teste e Validação por Fase**

**Critérios de Conclusão por Fase:**

- **Fase 1**: Textos carregam corretamente em diferentes idiomas, ícones mudam baseado no dispositivo
- **Fase 2**: Slime se move suavemente, animações funcionam, sprites mudam baseado na direção
- **Fase 3**: Fragmentos são coletados, energia acumula, slime evolui automaticamente
- **Fase 4**: Combate funciona, objetos respondem a interações, feedback visual está presente
- **Fase 5**: Diálogos aparecem corretamente, portais transportam entre áreas, movimentos especiais funcionam
- **Fase 6**: Stealth funciona, UI está responsiva, seguidores obedecem comandos
- **Fase 7**: Áudio toca apropriadamente, efeitos visuais aparecem nos momentos certos
- **Fase 8**: Jogo salva e carrega estado corretamente


### **Convenções de Nomenclatura**

**Diretrizes Obrigatórias:**

- GameObjects: camelCase (ex: `slimeBaby`, `interactionIcon`)
- Scripts: PascalCase (ex: `PlayerMovement`, `LocalizationManager`)
- Variáveis públicas: camelCase (ex: `moveSpeed`, `dialogText`)
- Variáveis privadas: camelCase com _ (ex: `_currentHP`, `_dialogActive`)
- Constantes: UPPER_CASE (ex: `MAX_FOLLOWERS`, `DEFAULT_LANGUAGE`)
- Chaves de Localização: snake_case (ex: `ui_start_game`, `dialog_fairy_greeting_01`)
- IDs de Sistema: snake_case (ex: `forest_main_portal`, `cave_squeeze_point_01`)


### **Performance Guidelines**

**Otimizações por Fase:**

- **Fases 1-2**: Foco em arquitetura limpa e modular
- **Fases 3-4**: Implementar object pooling para fragmentos e efeitos
- **Fases 5-6**: Otimizar detecção de proximidade e culling de objetos distantes
- **Fases 7-8**: Implementar LOD system e batching para performance final


---

## **IMPLEMENTAÇÕES DETALHADAS**

Esta seção contém detalhes técnicos sobre as implementações realizadas, servindo como documentação para desenvolvedores.

### **1. Sistema de Localização de Textos**

O Sistema de Localização foi implementado conforme as especificações, com foco em flexibilidade, extensibilidade e facilidade de uso.

#### 1.1 Estrutura e Funcionalidades Implementadas

**Classes Principais:**
- **LocalizationManager**: Singleton que gerencia toda a lógica de localização
- **LocalizedText**: Componente para TextMeshProUGUI que atualiza textos automaticamente
- **LanguageMenu**: Facilita a criação de seletores de idioma na UI

**Diretórios e Arquivos:**
- `/Assets/Code/Core/Localization/`: Classes principais do sistema
- `/Assets/StreamingAssets/Localization/`: Arquivos CSV de tradução
- `/config.json`: Armazenado no PersistentDataPath para preferências do usuário

**Linguagens Suportadas:**
- EN (Inglês)
- PT_BR (Português Brasil)
- ES (Espanhol)
- FR (Francês)
- DE (Alemão)
- JA (Japonês)
- ZH_CN (Chinês Simplificado)

#### 1.2 API e Utilização

**Obtenção de Textos em Scripts:**
```csharp
// Pegar texto localizado pela chave
string texto = LocalizationManager.Instance.GetLocalizedText("ui_start_game");

// Alterar idioma programaticamente
LocalizationManager.Instance.SetLanguage("PT_BR");

// Obter idioma atual
string idiomaAtual = LocalizationManager.Instance.GetCurrentLanguage();

// Listar idiomas suportados
string[] idiomas = LocalizationManager.Instance.GetSupportedLanguages();
```

**Aplicação em Elementos de UI:**
1. Adicione o componente `LocalizedText` a qualquer TextMeshProUGUI
2. Defina a chave de localização no campo _localizationKey
3. O texto será automaticamente traduzido no Start()

**Criando Menu de Idiomas:**
1. Crie um TMP_Dropdown na UI
2. Adicione o componente `LanguageMenu` ao GameObject
3. Arraste o dropdown para o campo _languageDropdown
4. Conecte o evento OnValueChanged do dropdown ao método OnLanguageChanged do script

#### 1.3 Hierarquia de Detecção de Idioma

O sistema implementa a hierarquia conforme especificado:

1. **Configuração do Usuário**: Verifica se há uma preferência salva em config.json
2. **Idioma do Sistema**: Detecta automaticamente usando CultureInfo.CurrentCulture
3. **Fallback Regional**: Se o idioma exato não está disponível, tenta uma variante da mesma região
4. **Fallback Global**: Se nenhum outro método funcionar, usa Inglês (EN) como padrão

#### 1.4 Convenções de Nomenclatura

O arquivo CSV implementa todas as convenções de nomenclatura conforme especificado:

- **Interface**: `ui_[categoria]_[elemento]` (ex: `ui_menu_start`) 
- **Diálogos**: `dialog_[npc]_[contexto]_[numero]` (ex: `dialog_fairy_greeting_01`)
- **Descrições**: `desc_[objeto]_[tipo]` (ex: `desc_item_apple`)
- **Mensagens**: `msg_[categoria]_[tipo]` (ex: `msg_system_save_complete`)

#### 1.5 Benefícios da Implementação

- **Escalabilidade**: Fácil adição de novos idiomas sem modificar código
- **Performance**: Carregamento assíncrono de arquivos para não bloquear a thread principal
- **Manutenibilidade**: Separação clara entre dados (CSV) e lógica (código)
- **Usabilidade**: API simples e componentes que facilitam a aplicação em UI

#### 1.6 Ferramentas de Edição

O sistema de localização inclui ferramentas de edição integradas ao Unity Editor para facilitar o trabalho dos desenvolvedores:

**Editor de Localização:**
- Acesso via menu `Extras > The Slime King > Localização > Editor CSV`
- Interface visual para gerenciar as entradas do arquivo CSV
- Visualização, adição, edição e exclusão de entradas
- Busca e filtragem por prefixo (ui_, dialog_, msg_, desc_)
- Agrupamento visual por categorias

**Adição Rápida de Entradas:**
- Acesso via menu `Extras > The Slime King > Localização > Adicionar Nova Entrada`
- Interface simplificada para adicionar novas chaves de localização
- Seleção rápida de prefixos padronizados

**Validação do CSV:**
- Acesso via menu `Extras > The Slime King > Localização > Verificar CSV Localização`
- Verifica integridade do arquivo CSV
- Apresenta estatísticas sobre entradas por categoria
- Identifica problemas no formato ou conteúdo

**Fluxo de Trabalho Recomendado:**
1. Usar o Editor de Localização para criar novas entradas
2. Adicionar as traduções para o idioma principal (geralmente EN)
3. Compartilhar o CSV com tradutores para preenchimento dos outros idiomas
4. Verificar o arquivo com a ferramenta de validação antes de compilar

### **2. Sistema de Ícone Superior**

O Sistema de Ícone Superior foi implementado para fornecer feedback visual dinâmico sobre ações contextualmente disponíveis, adaptando-se automaticamente ao dispositivo de entrada que o jogador está utilizando.

#### 2.1 Estrutura e Funcionalidades Implementadas

**Classes Principais:**
- **IconManager**: Singleton central que gerencia a exibição e comportamento dos ícones
- **InputIconData**: ScriptableObject que mapeia ações para ícones específicos por dispositivo
- **IconDisplay**: Componente para objetos interativos que precisam exibir ícones contextuais
- **DeviceDetector**: Classe responsável por identificar e notificar mudanças de dispositivo

**Diretórios e Arquivos:**
- `/Assets/Code/Core/UI/Icons/`: Classes principais do sistema
- `/Assets/Prefabs/UI/Icons/`: Prefabs para diferentes conjuntos de ícones
- `/Assets/ScriptableObjects/InputIcons/`: Dados de mapeamento de ícones por dispositivo
- `/config.json`: Compartilha o arquivo de configuração com o Sistema de Localização

**Plataformas Suportadas:**
- Teclado (teclas específicas visualmente identificadas)
- Controles Xbox (A, B, X, Y, LB, RB, etc.)
- Controles PlayStation (Cross, Circle, Square, Triangle, L1, R1, etc.)
- Controles Nintendo Switch (A, B, X, Y, L, R, etc.)
- Controles genéricos (representações padronizadas quando dispositivo específico não é identificado)

#### 2.2 API e Utilização

**Exibição de Ícones em Scripts:**
```csharp
// Mostrar ícone para uma ação específica
IconManager.Instance.ShowIcon("Interact", transform.position + Vector3.up);

// Ocultar ícone atual
IconManager.Instance.HideIcon();

// Verificar dispositivo atual 
string dispositivoAtual = IconManager.Instance.GetCurrentDevice();

// Evento de mudança de dispositivo (para uso em outros sistemas)
IconManager.Instance.OnDeviceChanged += HandleDeviceChanged;
```

**Integração com Objetos Interativos:**
1. Adicione o componente `IconDisplay` ao GameObject interativo
2. Defina a ação associada no campo _actionType (ex: "Interact", "Attack", "Crouch")
3. Configure a distância de detecção no campo _detectionRadius
4. O ícone aparecerá automaticamente quando o slime estiver próximo

**Detecção Contínua de Dispositivo:**
```csharp
// Exemplo da lógica interna do DeviceDetector
void Update() {
    if (Gamepad.current is XInputController) {
        SetCurrentDevice(DeviceType.Xbox);
    }
    else if (Gamepad.current is DualShockGamepad) {
        SetCurrentDevice(DeviceType.PlayStation);
    }
    else if (Gamepad.current is SwitchProController) {
        SetCurrentDevice(DeviceType.Switch);
    }
    else if (Gamepad.current != null) {
        SetCurrentDevice(DeviceType.Generic);
    }
    else if (Keyboard.current != null && Keyboard.current.anyKey.isPressed) {
        SetCurrentDevice(DeviceType.Keyboard);
    }
}
```

#### 2.3 Sistema de Mapeamento de Ícones

O sistema utiliza ScriptableObjects para definir o mapeamento entre ações de input e ícones por dispositivo:

**Estrutura do InputIconData:**
```csharp
[CreateAssetMenu(fileName = "InputIconMapping", menuName = "The Slime King/InputIconMapping")]
public class InputIconData : ScriptableObject {
    [Serializable]
    public class ActionIcon {
        public string actionName;
        public Sprite keyboardIcon;
        public Sprite xboxIcon;
        public Sprite playstationIcon;
        public Sprite switchIcon;
        public Sprite genericIcon;
    }
    
    public List<ActionIcon> actionIcons = new List<ActionIcon>();
    
    public Sprite GetIconForAction(string actionName, DeviceType deviceType) {
        // Lógica para retornar o sprite correto baseado na ação e dispositivo
    }
}
```

#### 2.4 Comportamento de Exibição e Animação

O sistema de ícones implementa animações suaves para melhorar a experiência de usuário:

- **Fade-in**: Quando o slime se aproxima de um objeto interativo (0.3s)
- **Persistência**: O ícone permanece visível enquanto o jogador está no raio de interação
- **Fade-out**: Quando o slime deixa a área de interação (0.3s)
- **Troca Instantânea**: Quando o dispositivo de input muda, o ícone é atualizado imediatamente

**Detecção de Proximidade:**
Cada objeto interativo usa um OverlapCircle para detectar quando o slime está próximo o suficiente:

```csharp
void Update() {
    if (Physics2D.OverlapCircle(transform.position, _detectionRadius, _playerLayer)) {
        if (!_isDisplayingIcon) {
            _isDisplayingIcon = true;
            IconManager.Instance.ShowIcon(_actionType, _iconPosition.position);
        }
    } 
    else if (_isDisplayingIcon) {
        _isDisplayingIcon = false;
        IconManager.Instance.HideIcon();
    }
}
```

#### 2.5 Benefícios da Implementação

- **Adaptabilidade**: Ajusta-se automaticamente ao dispositivo em uso pelo jogador
- **Desacoplamento**: Sistema modular que pode ser usado por qualquer objeto interativo
- **Usabilidade**: Fornece feedback visual claro sobre ações possíveis
- **Acessibilidade**: Ajuda jogadores a identificar controles para diferentes plataformas
- **Extensibilidade**: Fácil adição de suporte para novos dispositivos ou ações

#### 2.6 Ferramentas de Edição

O sistema inclui ferramentas de edição integradas ao Unity Editor para facilitar a configuração:

**Editor de Mapeamento de Ícones:**
- Acesso via menu `Extras > The Slime King > Ícones > Editor de Mapeamento`
- Interface visual para gerenciar o mapeamento entre ações e ícones
- Visualização de todos os sprites por dispositivo
- Possibilidade de modificar mapeamentos existentes ou criar novos

**Testador de Dispositivos:**
- Acesso via menu `Extras > The Slime King > Ícones > Testador de Dispositivos`
- Simula diferentes dispositivos para verificar a aparência dos ícones
- Permite testar a detecção automática de dispositivos

**Prefabs Pré-configurados:**
- Prefabs prontos para uso com configurações otimizadas
- Variações específicas para diferentes tipos de interação
- Integração facilitada com o sistema de objetos interativos

**Fluxo de Trabalho Recomendado:**
1. Definir as ações de input necessárias no Input System
2. Criar ou atualizar o mapeamento de ícones usando o Editor de Mapeamento
3. Adicionar o componente `IconDisplay` aos objetos interativos relevantes
4. Testar a visualização em diferentes dispositivos com o Testador

### **6. Sistema de Absorção Elemental**

O Sistema de Absorção Elemental foi implementado como uma mecânica central de progressão do jogo, permitindo que o slime absorva energia de diferentes elementos para crescer e desenvolver habilidades especiais conforme especificado na documentação.

#### 6.1 Estrutura e Funcionalidades Implementadas

**Classes Principais:**
- **ElementalEnergyManager**: Singleton responsável por gerenciar toda a energia elemental do jogador
- **ElementalFragment**: Controla o comportamento dos fragmentos elementais coletáveis
- **ElementalFragmentSpawner**: Gerencia a criação de fragmentos com system de object pooling
- **ElementalAbilityManager**: Controla as habilidades elementais que o jogador pode usar
- **ElementalEvents**: Sistema de eventos para comunicação desacoplada entre componentes

**Diretórios e Arquivos:**
- `/Assets/Code/Core/Elemental/`: Classes principais do sistema
- `/Assets/Prefabs/Elemental/Fragments/`: Prefabs para os diferentes tipos de fragmentos
- `/Assets/ScriptableObjects/Elemental/`: Dados configuráveis para fragmentos e habilidades
- `/Assets/VFX/Elemental/`: Efeitos visuais específicos por elemento

**Elementos Implementados:**
- Terra: Cor marrom (#8B4513/#DEB887), bônus de defesa
- Água: Cor azul (#4169E1/#87CEEB), bônus de regeneração
- Fogo: Cor laranja/vermelho (#FF4500/#FFA500), bônus de ataque
- Ar: Cor azul claro/branco (#E6E6FA/#F0F8FF), bônus de velocidade

#### 6.2 Fragmentos Elementais

**Tamanhos de Fragmentos:**
- **Small**: 1 ponto de energia, mais comum, tamanho 0.7x
- **Medium**: 3 pontos de energia, frequência média, tamanho 1.0x
- **Large**: 7 pontos de energia, mais raro, tamanho 1.5x

**Sistema de Cores Dinâmicas:**
```csharp
// Exemplo de código para coloração dinâmica
private void AnimateColors()
{
    _currentColorBlend = (Mathf.Sin(Time.time * _blinkSpeed) + 1) * 0.5f;
    _spriteRenderer.color = Color.Lerp(_primaryColor, _secondaryColor, _currentColorBlend);
}
```

**Atributos Físicos:**
- **Atração Magnética**: Raio de 2.0 unidades para atração ao jogador
- **Velocidade de Atração**: 8.0 unidades por segundo quando dentro do raio
- **Rotação**: Os fragmentos rotacionam suavemente (90° por segundo)
- **Efeito de Bounce**: Ao serem criados, recebem uma força inicial aleatória

**Sistema de Coleta:**
```csharp
// Trecho simplificado da lógica de absorção
private void AbsorbIntoPlayer(Transform playerTransform)
{
    _isFadingOut = true;
    _collider2D.enabled = false;
    
    // Reproduz som e efeito visual
    AudioSource.PlayClipAtPoint(_absorbSound, transform.position, 0.5f);
    Instantiate(_absorbEffectPrefab, transform.position, Quaternion.identity);
    
    // Notifica o gerenciador de energia
    int energyValue = GetEnergyValue();
    ElementalEnergyManager.Instance?.AddElementalEnergy(_elementType, energyValue);
    
    // Dispara evento global de absorção
    ElementalEvents.OnFragmentAbsorbed?.Invoke(_elementType, energyValue, transform.position);
}
```

#### 6.3 Sistema de Object Pooling

O sistema implementa um robusto Object Pooling para otimizar performance:

**Funcionamento:**
- Fragmentos são reutilizados ao invés de destruídos/criados constantemente
- Pools separadas por tipo elemental para rápido acesso
- Fragmentos desativados retornam automaticamente à pool após uso
- Configuração visual (cor, escala, sprite) aplicada ao reativar

```csharp
// Método para obter fragmento da pool ou criar novo
private GameObject SpawnSingleFragment(Vector3 position, ElementalType elementType, ElementalFragment.FragmentSize size) 
{
    // Verificação da pool
    if (_fragmentPools[elementType].Count > 0)
    {
        GameObject fragment = _fragmentPools[elementType].Dequeue();
        fragment.transform.position = position;
        fragment.SetActive(true);
        return fragment;
    }
    
    // Criação de novo quando pool vazia
    int randomPrefabIndex = Random.Range(0, _fragmentPrefabs.Length);
    GameObject newFragment = Instantiate(_fragmentPrefabs[randomPrefabIndex], position, Quaternion.identity);
    
    // Configuração do fragmento
    ElementalFragment fragComponent = newFragment.GetComponent<ElementalFragment>();
    fragComponent.Setup(elementType, size);
    
    return newFragment;
}
```

#### 6.4 Sistema de Drop Configurável

Foi implementado um sistema flexível para configurar drops de fragmentos por objeto:

**Classe FragmentDropConfig:**
- Configuração de tamanhos permitidos (small, medium, large)
- Probabilidades por tamanho (ex: small 70%, medium 25%, large 5%)
- Configuração de quantidade mínima e máxima
- Opção para tipo elemental fixo ou aleatório
- Distribuição de probabilidades por elemento quando aleatório

**Exemplo de Uso:**
```csharp
// Configuração de drop para uma rocha
FragmentDropConfig rockConfig = new FragmentDropConfig
{
    canDropSmall = true,
    canDropMedium = true, 
    canDropLarge = false,
    
    smallChance = 80f,
    mediumChance = 20f,
    
    minFragments = 2,
    maxFragments = 4,
    
    fixedElementType = ElementalType.Earth  // Rochas sempre dropam Terra
};

// Aplicação do drop
_fragmentSpawner.SpawnFragments(transform.position, rockConfig);
```

#### 6.5 Integração com Sistema de UI

O sistema de UI para energia elemental foi implementado com:

**Barras de Energia:**
- Quatro barras distintas para cada tipo elemental
- Animação de preenchimento suave com DOTween
- Efeito de pulso ao ganhar energia
- Texto indicando valor numérico atual

**Feedback Visual:**
- Flash de cor ao absorver fragmentos
- Efeito especial ao atingir thresholds de crescimento
- Indicador de progresso para próximo estágio

#### 6.6 Sistema de Efeitos Passivos

Os efeitos passivos por tipo elemental foram implementados conforme especificado:

| Elemento | Efeito Passivo | Implementação |
| :-- | :-- | :-- |
| Terra | +1 Defense a cada 10 pontos | Redução de dano recebido |
| Água | +1 Regeneração a cada 10 pontos | Cura periódica de HP |
| Fogo | +1 Attack a cada 10 pontos | Aumento de dano causado |
| Ar | +1 Speed a cada 10 pontos | Aumento de velocidade de movimento |

**Exemplo de Implementação:**
```csharp
// Trecho do processamento de efeitos passivos
private void SetupPassiveEffects()
{
    // Terra: +1 Defense a cada 10 pontos
    _passiveEffects[ElementalType.Earth] = () => {
        int defenseBuff = _elementalEnergy[ElementalType.Earth] / 10;
        StatusManager.Instance?.SetElementalBuff("Defense", defenseBuff);
    };
    
    // Água: +1 Regeneração a cada 10 pontos
    _passiveEffects[ElementalType.Water] = () => {
        int regenBuff = _elementalEnergy[ElementalType.Water] / 10;
        StatusManager.Instance?.SetElementalBuff("Regeneration", regenBuff);
    };
    
    // Similar para Fogo e Ar
}
```

#### 6.7 Habilidades Elementais

O sistema de habilidades elementais permite ao jogador usar ativamente a energia acumulada:

**Sistema de Slots:**
- Slots de habilidade limitados pelo estágio de crescimento
- Mapeamento para teclas 1-4 no teclado ou botões de ombro no gamepad
- Troca dinâmica de habilidades via menu de habilidades

**Componentes de Habilidade:**
- Custo em energia elemental
- Tempo de cooldown específico por habilidade
- Efeito visual específico por elemento
- Som específico por elemento

**Verificação de Uso:**
```csharp
public bool UseAbility(int abilityIndex)
{
    ElementalAbility ability = _abilities[abilityIndex];
    
    // Verificações
    if (ability.cooldownRemaining > 0)
        return false;
        
    if (!ElementalEnergyManager.Instance.UseElementalEnergy(ability.elementType, ability.energyCost))
        return false;
    
    // Uso da habilidade
    _playerAnimator.SetTrigger("Attack02");
    
    // Instantiate efeito
    GameObject effectObject = Instantiate(
        ability.abilityEffectPrefab,
        _playerTransform.position,
        _playerTransform.rotation
    );
    
    // Aplicar cooldowns
    _globalCooldownRemaining = _globalCooldown;
    ability.cooldownRemaining = ability.cooldown;
    
    // Notificar via evento
    ElementalEvents.OnElementalAbilityUsed?.Invoke(
        ability.elementType,
        ability.energyCost,
        _playerTransform.position
    );
    
    return true;
}
```

#### 6.8 Integração com Sistema de Crescimento

A energia elemental total serve como trigger para o crescimento do slime:

**Thresholds de Crescimento:**
- Young: 200 pontos totais
- Adult: 600 pontos totais
- Elder: 1200 pontos totais

**Verificação Automática:**
```csharp
private void CheckGrowthThreshold()
{
    int totalEnergy = GetTotalAbsorbedEnergy();
    int currentStage = GetCurrentGrowthStage();
    
    if (currentStage > _lastGrowthThresholdReached)
    {
        _lastGrowthThresholdReached = currentStage;
        OnTotalEnergyThresholdReached?.Invoke();
        PlayerGrowth.Instance?.EvaluateGrowthStage();
    }
}
```

#### 6.9 Benefícios da Implementação

- **Desacoplamento**: Sistema de eventos permite que componentes se comuniquem sem referências diretas
- **Eficiência**: Object pooling reduz alocação de memória e garbage collection
- **Customização**: Uso de ScriptableObjects para configuração sem código
- **Extensibilidade**: Fácil adição de novos tipos de fragmentos ou habilidades
- **Feedback Visual**: Sistema robusto de feedback ajuda na compreensão do jogador

#### 6.10 Editor Tools

Foram desenvolvidas ferramentas de editor para auxiliar no desenvolvimento:

**Fragmento Spawner Tool:**
- Ferramenta para testar diferentes configurações de drop
- Visualização em tempo real de probabilidades
- Debug de raio de atração e comportamento

**Elemental Energy Viewer:**
- Janela de editor para visualizar e modificar energia elemental
- Útil para testes e debugging
- Botões para simular thresholds de crescimento

**Fluxo de Trabalho Recomendado:**
1. Configurar fragmentos via ScriptableObjects para diferentes elementos e tamanhos
2. Configurar drops específicos por objeto/inimigo com FragmentDropConfig
3. Ajustar animações de absorção e valores de energia por prefab
4. Testar thresholds de crescimento com a ferramenta de editor

---

### **Sistema de Inventário**

**Visão Geral:** Sistema que permite ao jogador coletar, gerenciar e usar itens, com número de slots que evolui conforme o crescimento do slime.

#### Componentes Principais:

1. **InventoryManager.cs** - Singleton responsável por gerenciar o inventário:
   - Controla o número de slots disponíveis baseado no estágio
   - Gerencia adição, remoção e uso de itens
   - Implementa sistema de navegação entre slots
   - Emite eventos para atualização da UI
   - Integra-se com sistemas de crescimento e elemental

2. **InventoryItem.cs** - Classe que representa um item no inventário:
   - Mantém referência ao ItemData e quantidade
   - Gerencia stacks e combinações
   - Implementa lógica de capacidade máxima

3. **ItemData.cs** - ScriptableObject que define propriedades de itens:
   - Define nome, descrição, ícone localizado
   - Especifica tipo, raridade, stack máximo
   - Configura efeitos visuais e sonoros de uso
   - Define comportamentos específicos (consumível, quest)

4. **InventoryUI.cs** - Interface visual do inventário:
   - Exibe slots com ícones e quantidades
   - Anima abertura/fechamento do inventário
   - Implementa seleção via mouse/teclado
   - Responde a eventos de mudança no inventário

5. **ItemWorldDrop.cs** - Representação de itens no mundo:
   - Efeitos visuais de flutuação e rotação
   - Sistema de atração para o jogador
   - Colisão e coleta automática
   - Feedback visual de coleta

#### Fluxo de Trabalho:

1. Itens são criados como ScriptableObjects usando a ferramenta de editor
2. Objetos no mundo podem dropar itens quando destruídos
3. O jogador coleta itens ao se aproximar deles
4. O sistema gerencia automaticamente stacks e organização
5. O número de slots aumenta com o crescimento do slime
6. O jogador pode selecionar e usar itens via UI ou atalhos

#### Sistema de Expansão de Slots:

| Estágio | Slots Disponíveis | 
| :-- | :-- |
| Baby Slime | 1 | 
| Young Slime | 2 | 
| Adult Slime | 3 | 
| Elder Slime | 4 |

#### Sistema de Uso de Itens:

O sistema implementa uma forma intuitiva de gerenciar e usar itens:

- **Navegação**: Mouse wheel, Tab ou D-pad para alternar slots
- **Seleção**: Slot selecionado tem destaque visual e efeito de pulsação
- **Uso**: Left Alt ou Y (gamepad) consome o item selecionado
- **Feedback**: Efeitos visuais e sonoros específicos por item
- **Auto-organização**: Slots vazios são automaticamente reorganizados

#### Ferramentas de Editor:

1. **InventoryEditor.cs** - Editor para criação e gerenciamento de itens:
   - Interface para criação de novos itens
   - Visualização e edição de itens existentes
   - Duplicação e exclusão de itens
   - Configuração visual de ícones e efeitos

2. **InventoryTester.cs** - Ferramenta para testar o sistema:
   - Adicionar itens aleatórios ao inventário
   - Testar uso e descarte de itens
   - Simular crescimento e expansão de slots
   - Dropar itens no mundo para testes

#### Exemplo de Uso:

```csharp
// Adicionar um item ao inventário
public void ColetarItem(ItemData itemData, int quantidade)
{
    if (InventoryManager.Instance.AddItem(itemData, quantidade))
    {
        Debug.Log($"Coletado: {itemData.name} x{quantidade}");
    }
    else
    {
        Debug.Log("Inventário cheio!");
    }
}

// Verificar e usar um item específico
public bool UsarPocaoDeVida()
{
    if (InventoryManager.Instance.HasItem("pocao_vida", 1))
    {
        // Usar diretamente o item selecionado
        if (InventoryManager.Instance.GetSelectedItem()?.ItemData.ItemId == "pocao_vida")
        {
            InventoryManager.Instance.UseSelectedItem();
            return true;
        }
        // Ou remover programaticamente
        else
        {
            InventoryManager.Instance.RemoveItem("pocao_vida", 1);
            AplicarEfeitoPocao();
            return true;
        }
    }
    return false;
}
```

**Integração com Sistema de Crescimento:**
- Slots de inventário são desbloqueados automaticamente ao evoluir
- O sistema de inventário escuta eventos do sistema elemental
- Verificações automáticas são feitas quando thresholds são atingidos

**Integração com Input System:**
- Utiliza as ações "UseItem", "ChangeItem" e "Inventory"
- Compatível com teclado, mouse e controles
- Configurável através do InputSystem_Actions

**Fluxo de Trabalho Recomendado:**
1. Criar ItemData para todos os itens do jogo usando o InventoryEditor
2. Configurar prefabs para ItemWorldDrop com efeitos visuais
3. Configurar coletor de itens em objetos e inimigos
4. Integrar UI de inventário na interface do jogo
5. Testar uso de itens e crescimento com InventoryTester

---