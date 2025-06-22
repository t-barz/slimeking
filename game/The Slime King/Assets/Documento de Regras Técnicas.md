# Documento de Regras Técnicas – The Slime King
## Versão 1.0

---

## **ÍNDICE GERAL**

### **I. INTRODUÇÃO E ORGANIZAÇÃO**

- [Introdução Geral](#introdu%C3%A7%C3%A3o-geral)
- [Estrutura de Implementação por Fases](#estrutura-de-implementa%C3%A7%C3%A3o-por-fases)

---

### **II. FASE 1: FUNDAMENTOS TÉCNICOS**

*Sistemas essenciais que servem como base para todos os outros*

#### **1. Sistema de Localização de Textos**

- [1.1 Estrutura de Arquivos CSV](#11-estrutura-de-arquivos-csv)
- [1.2 Detecção Automática de Idioma](#12-detec%C3%A7%C3%A3o-autom%C3%A1tica-de-idioma)
- [1.3 Sistema de Configuração](#13-sistema-de-configura%C3%A7%C3%A3o)


#### **2. Sistema de Ícone Superior**

- [2.1 Detecção de Plataforma](#21-detec%C3%A7%C3%A3o-de-plataforma)
- [2.2 Sistema de Exibição Dinâmica](#22-sistema-de-exibi%C3%A7%C3%A3o-din%C3%A2mica)

---

### **III. FASE 2: PERSONAGEM E MOVIMENTO BÁSICO**

*Implementação do protagonista e controles fundamentais*

#### **3. Sistema de Personagem**

- [3.1 Estrutura de GameObjects](#31-estrutura-de-gameobjects)
- [3.2 Regras de Visibilidade](#32-regras-de-visibilidade)
- [3.3 Componentes Obrigatórios](#33-componentes-obrigat%C3%B3rios)


#### **4. Sistema de Movimento**

- [4.1 Input Actions](#41-input-actions)
- [4.2 Configurações de Física](#42-configura%C3%A7%C3%B5es-de-f%C3%ADsica)


#### **5. Sistema de Animação**

- [5.1 Parâmetros do Animator](#51-par%C3%A2metros-do-animator)
- [5.2 Regras dos Parâmetros](#52-regras-dos-par%C3%A2metros)

---

### **IV. FASE 3: MECÂNICAS BÁSICAS DE PROGRESSÃO**

*Sistemas que permitem o jogador progredir e evoluir*

#### **6. Sistema de Absorção Elemental**

- [6.1 Tipos de Energia](#61-tipos-de-energia)
- [6.2 Fragmentos Elementais (Prefabs)](#62-fragmentos-elementais-prefabs)


#### **7. Sistema de Crescimento**

- [7.1 Estágios de Evolução](#71-est%C3%A1gios-de-evolu%C3%A7%C3%A3o)
- [7.2 Eventos de Crescimento](#72-eventos-de-crescimento)


#### **8. Sistema de Inventário**

- [8.1 Estrutura de Slots](#81-estrutura-de-slots)
- [8.2 Sistema de Uso](#82-sistema-de-uso)

---

### **V. FASE 4: SISTEMA DE COMBATE E INTERAÇÃO**

*Mecânicas de ação e interação com o mundo*

#### **9. Sistema de Combate**

- [9.1 Tipos de Ataque](#91-tipos-de-ataque)
- [9.2 Sistema de Dano](#92-sistema-de-dano)


#### **10. Sistema de Objetos Interativos**

- [10.1 Estrutura e Configuração](#101-estrutura-e-configura%C3%A7%C3%A3o)
- [10.2 Sistema de Feedback Visual](#102-sistema-de-feedback-visual)
- [10.3 Objetos Destrutíveis](#103-objetos-destrut%C3%ADveis)
- [10.4 Objetos Móveis](#104-objetos-m%C3%B3veis)

---

### **VI. FASE 5: SISTEMAS DE NARRATIVA E NAVEGAÇÃO**

*Comunicação com jogador e movimentação pelo mundo*

#### **11. Sistema de Diálogos**

- [11.1 Interface de Diálogo](#111-interface-de-di%C3%A1logo)
- [11.2 Sistema de Carregamento de Texto](#112-sistema-de-carregamento-de-texto)
- [11.3 Animações e Transições](#113-anima%C3%A7%C3%B5es-e-transi%C3%A7%C3%B5es)


#### **12. Sistema de Portal**

- [12.1 Tipos de Portal e Configuração](#121-tipos-de-portal-e-configura%C3%A7%C3%A3o)
- [12.2 Sistema de Ativação](#122-sistema-de-ativa%C3%A7%C3%A3o)
- [12.3 Teleporte Intra-Cena e Inter-Cena](#123-teleporte-intra-cena-e-inter-cena)


#### **13. Sistema de Movimento Especial**

- [13.1 Encolher e Esgueirar](#131-encolher-e-esgueirar)
- [13.2 Sistema de Pular](#132-sistema-de-pular)

---

### **VII. FASE 6: SISTEMAS AVANÇADOS DE GAMEPLAY**

*Mecânicas mais complexas que dependem de múltiplos sistemas*

#### **14. Sistema de Stealth**

- [14.1 Estados de Visibilidade](#141-estados-de-visibilidade)
- [14.2 Objetos de Cobertura](#142-objetos-de-cobertura)


#### **15. Sistema de UI**

- [15.1 HUD Elements](#151-hud-elements)
- [15.2 Configurações de Canvas](#152-configura%C3%A7%C3%B5es-de-canvas)


#### **16. Sistema de Seguidores e Mini-Slimes**

- [16.1 Gestão de Seguidores](#161-gest%C3%A3o-de-seguidores)
- [16.2 IA de Companheiros](#162-ia-de-companheiros)
- [16.3 Sistema de Mini-Slimes](#163-sistema-de-mini-slimes)

---

### **VIII. FASE 7: POLISH E SISTEMAS DE SUPORTE**

*Sistemas que podem ser implementados em paralelo e adicionam polish*

#### **17. Sistema de Áudio**

- [17.1 Triggers Sonoros](#171-triggers-sonoros)
- [17.2 Mixagem Dinâmica](#172-mixagem-din%C3%A2mica)


#### **18. Sistema de Efeitos Visuais**

- [18.1 Partículas](#181-part%C3%ADculas)
- [18.2 Pós-Processamento](#182-p%C3%B3s-processamento)
- [18.3 Shaders Customizados](#183-shaders-customizados)

---

### **IX. FASE 8: SISTEMA DE PERSISTÊNCIA**

*Deve ser implementado por último pois depende de todos os outros sistemas*

#### **19. Sistema de Persistência**

- [19.1 Save Data Structure](#191-save-data-structure)
- [19.2 Auto-Save Triggers](#192-auto-save-triggers)
- [19.3 Loading States](#193-loading-states)

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

- **Keyboard/Mouse**: Ícones de teclas específicas (E, Space, Q, etc.)
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


### 7. **Sistema de Crescimento**

Implementar após absorção elemental estar funcional.

#### 7.1 Estágios de Evolução

| Estágio | Energia Necessária | Slots Inventário | Seguidores Máx | Mini-Slimes Máx |
| :-- | :-- | :-- | :-- | :-- |
| Baby Slime | 0 | 1 | 0 | 0 |
| Young Slime | 200 | 2 | 1 | 1 |
| Adult Slime | 600 | 3 | 3 | 2 |
| Elder/King | 1200 | 4 | 4 | 3 |

#### 7.2 Eventos de Crescimento

Os eventos de crescimento devem seguir as especificações:

- **Trigger**: Energia elemental acumulada ≥ threshold
- **Efeito Visual**: Partículas especiais + screen flash suave
- **Efeito Sonoro**: Som harmônico ascendente
- **Duração**: 2 segundos de animação
- **Durante Crescimento**: Input desabilitado, invulnerabilidade temporária


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

## **Observações de Implementação por Fase**

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