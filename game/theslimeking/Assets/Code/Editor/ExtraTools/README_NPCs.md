# 🤖 Sistema Extra Tools para NPCs - v2.1

Sistema completo de criação e configuração de NPCs no projeto SlimeKing. Agora com configuração inteligente de animators existentes!

## 📋 Visão Geral

O sistema Extra Tools para NPCs automatiza completamente a criação e configuração de NPCs, transformando **qualquer GameObject** em um NPC funcional:

- **NPCController**: Sistema de movimentação e IA básica
- **Animator Inteligente**: Configuração automática de animators existentes com preservação dos estados
- **Componentes Automáticos**: Rigidbody2D, Colliders, SpriteRenderers
- **Estrutura Visual**: Criação automática de objetos direcionais (front/back/side/vfx)
- **Sistema de Animações**: Preserva clipes e estados existentes
- **Validação**: Ferramentas para verificar configurações

## 🛠️ Ferramentas Disponíveis

### **⭐ Extra Tools/NPC/Setup GameObject as NPC** (FUNCIONALIDADE APRIMORADA!)

**A ferramenta principal!** Configura completamente um GameObject selecionado para funcionar como NPC.

**🎯 Funcionalidades:**

- ✅ Adiciona todos os componentes obrigatórios (Rigidbody2D, Animator, Collider2D)
- ✅ Cria estrutura visual direcional automaticamente (front/back/side/vfx)
- ✅ Adiciona e configura NPCController
- ✅ **NOVO**: Configura animator existente em vez de criar novo
- ✅ **NOVO**: Preserva estados existentes (Idle, Walk, Attack, Hit, Unique, Die, etc.)
- ✅ **NOVO**: Adiciona apenas parâmetros necessários para movimento (isWalking, FacingRight)
- ✅ Aplica configurações finais (tags, layers, nomenclatura)
- ✅ Configura física adequada para NPCs 2D
- ✅ Sistema inteligente de detecção de conflitos

**🚀 Como usar:**

1. Selecione qualquer GameObject na hierarquia (pode ter animator existente!)
2. Execute `Extra Tools → NPC → Setup GameObject as NPC` (ou clique direito → Extra Tools → Setup as NPC)
3. **PRONTO!** O GameObject será configurado como NPC preservando animações existentes

**💡 MUDANÇA IMPORTANTE (v2.1):** Agora preserva animators existentes! Se o GameObject já possui um Animator Controller (como `art_beeA.controller`), a ferramenta:

- ✅ **Mantém** todos os estados existentes (Idle, Walk, Attack, Hit, Unique, Die)
- ✅ **Preserva** todas as transições e clips de animação
- ✅ **Adiciona** apenas os parâmetros necessários para movimento (`isWalking`, `FacingRight`)
- ✅ **Configura** o Animator para funcionar otimamente com NPCs

### **Extra Tools/NPC/Configure Existing Animator** (NOVA FERRAMENTA)

Configura especificamente um Animator existente para trabalhar com NPCController.

**Como usar:**

1. Selecione um GameObject com Animator
2. Execute `Extra Tools → NPC → Configure Existing Animator`
3. O animator será configurado mantendo estados existentes

### **Extra Tools/NPC/Create NEW Animator Controller** (RENOMEADA)

Cria um Animator Controller completamente novo para NPCs (útil quando não há animator).

**Parâmetros criados:**

- `isWalking` (Bool): Controla animação de movimento
- `FacingRight` (Bool): Controla direção do sprite

**Estados criados:**

- `Idle`: Estado padrão (parado)
- `Walking`: Estado de movimento

**Transições criadas:**

- Idle ↔ Walking baseadas no parâmetro `isWalking`
- Duração de transição: 0.1s
- Sem exit time para responsividade

**Como usar:**

1. Selecione um GameObject com Animator
2. Execute `Extra Tools → NPC → Create Animator Controller`
3. O controller será criado em `Assets/Art/Animations/NPCs/`

### **Extra Tools/NPC/Validate Animator Setup**

Valida se o Animator Controller está configurado corretamente.

**Verificações realizadas:**

- Presença dos parâmetros obrigatórios
- Existência dos estados necessários
- Configuração adequada das transições

**Como usar:**

1. Selecione um NPC na hierarquia
2. Execute `Extra Tools → NPC → Validate Animator Setup`
3. Verifique os resultados no Console

### **Extra Tools/NPC/Configure Visual Objects**

Reconfigura as referências dos objetos visuais direcionais.

**Como usar:**

1. Selecione um NPC que já possui NPCController
2. Execute `Extra Tools → NPC → Configure Visual Objects`

### **Extra Tools/NPC/Test NPC Movement**

Alterna entre modo Idle e Wander durante runtime para testes.

**Como usar:**

1. Entre no Play Mode
2. Selecione um NPC na hierarquia
3. Execute `Extra Tools → NPC → Test NPC Movement`

### **Extra Tools/NPC/Auto-Assign Animations**

Busca e associa animações automaticamente aos estados do Animator.

**Padrões de busca para Idle:**

- `{NPCName}_Idle`
- `{npcname}_idle`
- `art_{npcname}`
- `idle_{npcname}`
- `{NPCName}`

**Padrões de busca para Walking:**

- `{NPCName}_Walking`
- `{NPCName}_Walk`
- `{npcname}_walking`
- `{npcname}_walk`
- `walk_{npcname}`

**Pastas pesquisadas:**

- `Assets/Art/Animations/NPCs`
- `Assets/Art/Animations`
- `Assets/External/AssetStore/SlimeMec/Art/Animations`

**Como usar:**

1. Selecione um NPC com Animator Controller configurado
2. Execute `Extra Tools → NPC → Auto-Assign Animations`

### **Extra Tools/NPC/Create Animation Clips**

Cria clipes de animação básicos (vazio) para um NPC.

**Clipes criados:**

- `{NPCName}_Idle.anim`: Duração 1s, loop
- `{NPCName}_Walking.anim`: Duração 0.5s, loop

**Como usar:**

1. Selecione um NPC na hierarquia
2. Execute `Extra Tools → NPC → Create Animation Clips`

### **Extra Tools/NPC/List Available Animations**

Lista todas as animações disponíveis no projeto para debug.

**Como usar:**

- Execute `Extra Tools → NPC → List Available Animations`
- Verifique as animações disponíveis no Console

## 📁 Estrutura de Arquivos Criados

```
Assets/
├── Art/
│   └── Animations/
│       └── NPCs/
│           ├── {NPCName}_Controller.controller
│           ├── {NPCName}_Idle.anim
│           └── {NPCName}_Walking.anim
└── Code/
    └── Editor/
        └── ExtraTools/
            ├── NPCCreationTool.cs
            ├── NPCAnimationHelper.cs
            └── README_NPCs.md (este arquivo)
```

## 🎯 Estrutura Esperada do NPCTemplate

O NPCTemplate deve seguir esta estrutura hierárquica:

```
NPCTemplate
├── front (GameObject com SpriteRenderer)
├── back (GameObject com SpriteRenderer)
├── side (GameObject com SpriteRenderer)
├── vfx_front (GameObject com SpriteRenderer) [opcional]
├── vfx_back (GameObject com SpriteRenderer) [opcional]
└── vfx_side (GameObject com SpriteRenderer) [opcional]
```

**Componentes no GameObject principal:**

- `Animator`: Com runtime controller configurado
- `SortingGroup`: Para ordenação de sprites
- `NPCAttributesHandler`: Para sistema de atributos [opcional]
- `RandomStyle`: Para randomização visual [opcional]

## ⚙️ Configurações do NPCController

### Tipos de Movimento (MovementType)

- **Idle**: NPC fica parado
- **Wander**: Movimento randômico em torno da posição inicial
- **Patrol**: Patrulha entre pontos (futuro)
- **Follow**: Segue um alvo (futuro)

### Configurações de Movimento

- **moveSpeed**: Velocidade máxima (padrão: 2.0f)
- **acceleration**: Taxa de aceleração (padrão: 8.0f)
- **deceleration**: Taxa de desaceleração (padrão: 8.0f)

### Configurações de IA (Wander)

- **minIdleTime**: Tempo mínimo parado (padrão: 2s)
- **maxIdleTime**: Tempo máximo parado (padrão: 5s)
- **minMoveTime**: Tempo mínimo em movimento (padrão: 1s)
- **maxMoveTime**: Tempo máximo em movimento (padrão: 3s)
- **wanderRadius**: Raio de movimento randômico (padrão: 3.0f)

## 🚀 Fluxo de Trabalho Recomendado

### Para NPCs Novos

1. **Preparação**: Crie um NPCTemplate na cena com a estrutura visual
2. **Criação**: `Extra Tools → NPC → Create NPC from Template`
3. **Animações**: `Extra Tools → NPC → Create Animation Clips` (se necessário)
4. **Associação**: `Extra Tools → NPC → Auto-Assign Animations`
5. **Validação**: `Extra Tools → NPC → Validate Animator Setup`
6. **Teste**: `Extra Tools → NPC → Test NPC Movement` (em Play Mode)

### Para NPCs Existentes

1. **Adição de Controller**: `Extra Tools → NPC → Add NPCController to Selected`
2. **Configuração de Animator**: `Extra Tools → NPC → Create Animator Controller`
3. **Associação de Animações**: `Extra Tools → NPC → Auto-Assign Animations`

## 🔧 Troubleshooting

### "NPCTemplate não encontrado na cena"

- Certifique-se de que existe um GameObject chamado exatamente "NPCTemplate"
- O template deve estar ativo na hierarquia

### "Nenhuma animação encontrada"

- Verifique se as animações seguem as convenções de nome
- Use `Extra Tools → NPC → List Available Animations` para ver animações disponíveis
- Considere usar `Extra Tools → NPC → Create Animation Clips` para criar clips básicos

### "Animator Controller possui problemas"

- Execute `Extra Tools → NPC → Validate Animator Setup` para diagnosticar
- Recrie o controller com `Extra Tools → NPC → Create Animator Controller`

### NPCs não se movem

- Verifique se o NPCController está configurado como Wander
- Use `Extra Tools → NPC → Test NPC Movement` em Play Mode
- Certifique-se de que o GameObject possui Rigidbody2D

## 📚 Integração com Outros Sistemas

### PlayerController

- Usa os mesmos parâmetros do Animator (`isWalking`, `FacingRight`)
- Compatível com o sistema visual direcional
- Integra com NPCAttributesHandler

### Sistemas de IA Futuros

- Estrutura preparada para Patrol e Follow
- Extensível via enum MovementType
- Métodos públicos para controle externo

### Sistema de Atributos

- Integração opcional com NPCAttributesHandler
- Sincronização automática de velocidade
- Suporte a buffs/debuffs dinâmicos

---

## 💡 Dicas e Boas Práticas

1. **Nomeação**: Use nomes descritivos para NPCs (ex: "Bee", "Slime", "Guard")
2. **Organização**: Mantenha animações organizadas por tipo de NPC
3. **Performance**: Use `enableDebugGizmos = false` em builds de produção
4. **Teste**: Sempre teste o movimento em Play Mode após configurar
5. **Backup**: Use controle de versão para controllers de animação

## 🆕 Nova Funcionalidade v2.1: Animators Existentes

### Como funciona com Animators Pré-existentes

A partir da v2.1, o **Setup GameObject as NPC** foi completamente reformulado para trabalhar inteligentemente com animators existentes:

#### ✅ **Exemplo Prático: NPC_art_beeA**

Quando você executa "Setup as NPC" em um GameObject que já possui um Animator Controller (como o `art_beeA.controller`), a ferramenta:

**PRESERVA:**

- ✅ Todos os estados existentes: `Idle`, `Walk`, `Attack`, `Hit`, `Unique`, `Die`
- ✅ Todas as transições configuradas
- ✅ Todos os parâmetros originais: `Hit` (Trigger), `Attack` (Trigger)
- ✅ Todas as animações e clips associados
- ✅ Configurações de timing e duração

**ADICIONA APENAS:**

- ➕ Parâmetro `isWalking` (Bool) - se não existir
- ➕ Parâmetro `FacingRight` (Bool) - se não existir
- ➕ Transições de movimento: `Idle` ↔ `Walk` baseadas em `isWalking`

**CONFIGURA:**

- ⚙️ Apply Root Motion = false (otimização para NPCs 2D)
- ⚙️ Culling Mode = AlwaysAnimate (performance consistent)

#### 🔄 **Fluxo de Trabalho Recomendado**

```text
1. Artist cria Animator Controller completo com todos os estados
   ↓
2. Animator é testado e validado separadamente
   ↓
3. Developer executa "Setup as NPC" 
   ↓
4. NPCController usa estados existentes + parâmetros básicos de movimento
   ↓
5. NPC funcional com animações completas preservadas!
```

#### 🎯 **Compatibilidade Total**

- **Animators Simples**: Apenas Idle/Walk → Adiciona parâmetros necessários
- **Animators Complexos**: Idle/Walk/Attack/Hit/Unique/Die → Preserva tudo + adiciona movimento
- **Animators Vazios**: Sem controller → Cria controller básico novo

#### 💡 **Exemplo de Log da Ferramenta**

```console
[NPCCreationTool] 📋 Parâmetros existentes: Hit (Trigger), Attack (Trigger)
[NPCCreationTool] ➕ Parâmetro 'isWalking' (Bool) adicionado - necessário para movimento
[NPCCreationTool] ➕ Parâmetro 'FacingRight' (Bool) adicionado - necessário para direção
[NPCCreationTool] 🔄 Configurando transições de movimento entre 'Idle' e 'Walk'...
[NPCCreationTool] ➕ Transição criada: Idle → Walk (quando isWalking = true)
[NPCCreationTool] ➕ Transição criada: Walk → Idle (quando isWalking = false)
[NPCCreationTool] 📊 Estados: Idle, Walk, Attack, Hit, Unique, Die
[NPCCreationTool] 💡 O NPC manterá os estados existentes (Idle, Walk, Attack, Hit, Unique, Die)
[NPCCreationTool] 💡 Apenas parâmetros básicos de movimento (isWalking, FacingRight) foram adicionados se necessário
```

## 📝 Changelog

### v2.1.4 (Atual - 19/11/2024)

- ✅ **PARÂMETROS COMPLETOS**: Agora configura também triggers `Hit` e `Attack` se não existirem
- ✅ **SISTEMA COMBAT COMPLETO**: Parâmetros necessários para combat system são criados automaticamente:
  - 🚶 `isWalking` (Bool) - Para controle de movimento
  - 🎯 `FacingRight` (Bool) - Para direção visual  
  - ⚔️ `Attack` (Trigger) - Para sistema de ataque
  - 💥 `Hit` (Trigger) - Para sistema de dano
- ✅ **COMPATIBILIDADE TOTAL**: Detecta parâmetros existentes e adiciona apenas os ausentes
- ✅ **LOGS DETALHADOS**: Feedback claro sobre quais parâmetros foram criados vs mantidos

### v2.1.3 (19/11/2024)

- ✅ **REVOLUCIONÁRIO**: `ConfigureAllTransitions` substitui `ConfigureMovementTransitions`
- ✅ **CONFIGURAÇÃO COMPLETA**: Agora configura **TODAS** as transições existentes baseadas nos parâmetros:
  - 🚶 **Movimento**: Idle ↔ Walk (baseado em `isWalking`)
  - ⚔️ **Ataque**: Any State → Attack (baseado em `Attack` trigger)
  - 💥 **Dano**: Any State → Hit (baseado em `Hit` trigger)
  - 🔄 **Conclusão**: Attack/Hit/Unique → Idle (via exit time)
  - 💀 **Morte**: Hit → Die (via exit time)
  - ⭐ **Especial**: Idle/Walk → Unique (via exit time)
- ✅ **INTELIGENTE**: Detecta, atualiza ou cria transições conforme necessário
- ✅ **PRESERVA**: Mantém transições existentes corretas intactas
- ✅ **OTIMIZA**: Propriedades de transição para responsividade máxima
- ✅ **UNIVERSAL**: Funciona com qualquer complexity de Animator Controller

### v2.1.2 (19/11/2024)

- ✅ **MELHORADO**: `ConfigureMovementTransitions` agora configura **TODAS** as transições existentes
- ✅ **FIX**: Atualiza transições existentes que não possuem condições corretas de `isWalking`
- ✅ **INTELIGENTE**: Remove condições antigas e adiciona condições adequadas
- ✅ **PRESERVA**: Mantém todas as outras transições (Attack, Hit, etc.) intactas
- ✅ **OTIMIZA**: Define propriedades otimizadas (`hasExitTime = false`, `duration = 0.1f`)
- ✅ **FUNCIONAL**: Setup as NPC agora realmente configura animators complexos completamente

### v2.1.1 (19/11/2024)

- ✅ **NOVO**: Configuração automática de transições de movimento
- ✅ **NOVO**: Cria transições `Idle` ↔ `Walk` baseadas em `isWalking`
- ✅ **NOVO**: Detecta e preserva transições existentes
- ✅ **NOVO**: Configura estado padrão como `Idle` automaticamente
- ✅ Logs detalhados para debugging de transições

### v2.1.0 (19/11/2024)

- ✅ **BREAKING CHANGE**: Setup as NPC agora preserva animators existentes
- ✅ Configuração inteligente de parâmetros (adiciona apenas o necessário)
- ✅ Preservação completa de estados e transições existentes
- ✅ Nova ferramenta: "Configure Existing Animator"
- ✅ Renomeação: "Create Animator Controller" → "Create NEW Animator Controller"
- ✅ Logs detalhados para debugging
- ✅ Compatibilidade total com animators complexos (art_beeA, etc.)
- ✅ Otimizações automáticas (Root Motion, Culling Mode)

### v2.0.0

- ✅ Funcionalidade "Setup GameObject as NPC" universal
- ✅ Remoção da dependência de templates
- ✅ Criação automática de estrutura visual
- ✅ Sistema de validação aprimorado

### v1.0.0

- ✅ Criação automática de NPCController
- ✅ Geração de Animator Controller com parâmetros e transições
- ✅ Sistema de associação automática de animações
- ✅ Ferramentas de validação e debug
- ✅ Integração completa com estrutura do NPCTemplate
- ✅ Suporte a objetos visuais direcionais
- ✅ Sistema de movimento Wander implementado

### Planejado para v3.0.0

- 🔄 Sistema de Patrol com waypoints
- 🔄 Sistema de Follow com detecção de alvos
- 🔄 Editor visual para configuração de rotas
- 🔄 Sistema de LOD para otimização
- 🔄 Integração com sistema de diálogos
