# 🤖 Sistema Extra Tools para NPCs - v2.0

Sistema completo de criação e configuração de NPCs no projeto SlimeKing. Agora com configuração total em um clique!

## 📋 Visão Geral

O sistema Extra Tools para NPCs automatiza completamente a criação e configuração de NPCs, transformando **qualquer GameObject** em um NPC funcional:

- **NPCController**: Sistema de movimentação e IA básica
- **Animator Controller**: Criação automática de parâmetros, estados e transições
- **Componentes Automáticos**: Rigidbody2D, Colliders, SpriteRenderers
- **Estrutura Visual**: Criação automática de objetos direcionais (front/back/side/vfx)
- **Sistema de Animações**: Associação automática de clipes de animação
- **Validação**: Ferramentas para verificar configurações

## 🛠️ Ferramentas Disponíveis

### **⭐ Extra Tools/NPC/Setup GameObject as NPC** (NOVA FUNCIONALIDADE)

**A ferramenta principal!** Configura completamente um GameObject selecionado para funcionar como NPC.

**🎯 Funcionalidades:**

- ✅ Adiciona todos os componentes obrigatórios (Rigidbody2D, Animator, Collider2D)
- ✅ Cria estrutura visual direcional automaticamente (front/back/side/vfx)
- ✅ Adiciona e configura NPCController
- ✅ Cria Animator Controller com parâmetros completos
- ✅ Aplica configurações finais (tags, layers, nomenclatura)
- ✅ Configura física adequada para NPCs 2D
- ✅ Sistema inteligente de detecção de conflitos

**🚀 Como usar:**

1. Selecione qualquer GameObject na hierarquia (pode ser um sprite, um empty, qualquer coisa!)
2. Execute `Extra Tools → NPC → Setup GameObject as NPC`
3. **PRONTO!** O GameObject será transformado em um NPC completo automaticamente

**💡 IMPORTANTE:** Funciona com qualquer GameObject - não é necessário ter um template pré-existente!

### **Extra Tools/NPC/Add NPCController to Selected**

Adiciona apenas o NPCController a um GameObject selecionado (método mais conservador).

**Como usar:**

1. Selecione um GameObject na hierarquia
2. Execute `Extra Tools → NPC → Add NPCController to Selected`

### **Extra Tools/NPC/Create NPC from Template**

Cria um NPC completo baseado no NPCTemplate existente na cena.

**Funcionalidades:**

- Duplica o NPCTemplate
- Adiciona NPCController automaticamente
- Configura objetos visuais direcionais
- Cria Animator Controller com parâmetros completos
- Configura componentes obrigatórios (Rigidbody2D, Animator)

**Como usar:**

1. Certifique-se de que existe um "NPCTemplate" na cena
2. Execute `Extra Tools → NPC → Create NPC from Template`
3. Um novo NPC será criado e selecionado automaticamente

### **Extra Tools/NPC/Add NPCController to Selected**

Adiciona NPCController a um GameObject selecionado.

**Funcionalidades:**

- Adiciona componentes obrigatórios se não existirem
- Configura NPCController automaticamente
- Cria Animator Controller personalizado
- Detecta e configura objetos visuais filhos

**Como usar:**

1. Selecione um GameObject na hierarquia
2. Execute `Extra Tools → NPC → Add NPCController to Selected`

### **Extra Tools/NPC/Create Animator Controller**

Cria um Animator Controller completo para NPCs.

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

## 📝 Changelog

### v1.0.0 (Atual)

- ✅ Criação automática de NPCController
- ✅ Geração de Animator Controller com parâmetros e transições
- ✅ Sistema de associação automática de animações
- ✅ Ferramentas de validação e debug
- ✅ Integração completa com estrutura do NPCTemplate
- ✅ Suporte a objetos visuais direcionais
- ✅ Sistema de movimento Wander implementado

### Planejado para v2.0.0

- 🔄 Sistema de Patrol com waypoints
- 🔄 Sistema de Follow com detecção de alvos
- 🔄 Editor visual para configuração de rotas
- 🔄 Sistema de LOD para otimização
- 🔄 Integração com sistema de diálogos
