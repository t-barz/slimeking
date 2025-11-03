# ALPHA 1 - Checklist Completo

**Data Alvo:** Janeiro 2026 (2 meses restantes)  
**Progresso Atual:** 30% → Meta: 55%  
**Objetivo:** Vertical Slice Interno jogável (12-15 minutos)

---

## 📊 Visão Geral

### O que já temos ✅

**Sistemas Completos (30%):**

- ✅ Arquitetura base (Managers, Singletons, Events)
- ✅ Sistema de Inventário (20 slots + equipamentos + quick slots)
- ✅ Sistema de Diálogo (manager + UI + localização)
- ✅ Sistema de Quest (core: manager + events + save)
- ✅ Sistema de NPC (base + 3 tipos de IA)
- ✅ Sistema de Save/Load (estrutura de dados)
- ✅ Ferramentas de Editor (12+ ferramentas)
- ✅ Ninho do Slime (90% - tutorial básico)

### O que falta (25% para chegar a 55%)

**Sistemas Pendentes:**

- 📋 Quest UI completa (log + tracker HUD)
- 📋 Mecânica de Agachar (stealth básico)
- 📋 Sistema de Cristais Elementais (contador UI)
- 📋 Sistema de Habilidades (2 habilidades Tier 1)
- 📋 Floresta Calma (3 áreas + NPCs + quest + puzzles)
- 📋 HUD completo (HP, Stamina, Cristais)
- 📋 Polimento e balanceamento

---

## 🎯 Checklist Detalhado por Sprint

### 🔥 Sprint 1: Quest UI + HUD Básico (2 semanas)

**Objetivo:** Completar sistema de quest e HUD essencial

#### Quest UI (1 semana)

- [ ] **Quest Log UI**
  - [ ] Criar painel de quest log (lista de quests ativas)
  - [ ] Mostrar título, descrição, objetivos
  - [ ] Indicador de progresso (ex: 3/5 flores coletadas)
  - [ ] Botão para expandir/colapsar detalhes
  - [ ] Atalho de teclado (Tab ou J)
  
- [ ] **Quest Tracker HUD**
  - [ ] Painel pequeno no canto da tela
  - [ ] Mostrar quest ativa atual
  - [ ] Progresso em tempo real
  - [ ] Animação ao atualizar progresso
  - [ ] Opção de minimizar/expandir
  
- [ ] **Integração com Gameplay**
  - [ ] Conectar ItemCollectable ao QuestManager
  - [ ] Testar coleta de itens → atualização de quest
  - [ ] Integrar QuestGiver com DialogueManager
  - [ ] Testar fluxo completo: aceitar → progredir → completar

#### HUD Básico (1 semana)

- [ ] **HP Bar**
  - [ ] Barra visual de HP (verde)
  - [ ] Animação de dano (shake + flash vermelho)
  - [ ] Animação de cura (brilho verde)
  - [ ] Números de HP (atual/máximo)
  
- [ ] **Stamina Bar**
  - [ ] Barra visual de Stamina (azul)
  - [ ] Regeneração automática (visual)
  - [ ] Depleção ao usar habilidades
  - [ ] Indicador de "sem stamina"
  
- [ ] **Contador de Cristais Elementais**
  - [ ] Ícone de cristal + número
  - [ ] Animação ao coletar (+1 popup)
  - [ ] Separar por tipo (Nature, Fire, etc.)
  - [ ] Tooltip ao passar mouse

**Entrega Sprint 1:**

- Quest system 100% funcional
- HUD essencial implementado
- Integração quest + coleta testada

---

### 🏰 Sprint 2: Ninho do Slime - Puzzle Final (1 semana)

**Objetivo:** Completar tutorial com puzzle introdutório

#### Puzzle de Placas de Pressão

- [ ] **Mecânica de Peso**
  - [ ] Implementar sistema de peso do slime
  - [ ] Placas de pressão (trigger ao pisar)
  - [ ] Objetos empurráveis (pedras pequenas)
  - [ ] Física de empurrar (Rigidbody2D)
  
- [ ] **Design do Puzzle**
  - [ ] 3 placas de pressão
  - [ ] 2 pedras empurráveis
  - [ ] 1 porta que abre ao ativar todas as placas
  - [ ] Pista visual (marcas no chão)
  
- [ ] **Recompensa**
  - [ ] Primeiro cristal elemental (Nature)
  - [ ] Animação de coleta especial
  - [ ] Tutorial de cristais (popup)
  - [ ] Desbloqueio de saída para Floresta

#### Polimento da Caverna

- [ ] **Iluminação**
  - [ ] URP 2D Lights (tochas, cristais)
  - [ ] Sombras suaves
  - [ ] Brilho de cristais
  
- [ ] **Partículas Ambientais**
  - [ ] Poeira flutuante
  - [ ] Brilho de cristais
  - [ ] Goteiras (se houver água)
  
- [ ] **SFX de Ambiente**
  - [ ] Goteiras (loop)
  - [ ] Ecos de passos
  - [ ] Som de vento distante
  - [ ] Som de cristais (hum baixo)

**Entrega Sprint 2:**

- Ninho do Slime 100% completo
- Puzzle funcional e polido
- Transição para Floresta preparada

---

### 🌿 Sprint 3-5: Floresta Calma (6 semanas)

#### Sprint 3: Clareira de Entrada (2 semanas)

**Semana 1: Level Design**

- [ ] **Tileset de Floresta**
  - [ ] Grama (3 variações)
  - [ ] Terra/caminho
  - [ ] Árvores (troncos + copas)
  - [ ] Flores (3 cores)
  - [ ] Pedras decorativas
  
- [ ] **Layout da Clareira**
  - [ ] Desenhar mapa (papel/digital)
  - [ ] Implementar no Unity (Tilemap)
  - [ ] Teleport point caverna → floresta
  - [ ] Colliders de árvores e obstáculos
  - [ ] Áreas de spawn de itens

**Semana 2: Vegetação Interativa**

- [ ] **Arbustos Destrutíveis**
  - [ ] Reutilizar BushDestruct
  - [ ] 10-15 arbustos na clareira
  - [ ] Drop de materiais (50% chance)
  
- [ ] **Flores Cristalinas**
  - [ ] Sprite de flor cristalina (8x8)
  - [ ] Prefab coletável
  - [ ] 10 flores espalhadas
  - [ ] Respawn após 5 minutos
  
- [ ] **Iluminação Natural**
  - [ ] Luz global (dia)
  - [ ] Sombras de árvores
  - [ ] Raios de sol (god rays)

**Entrega Sprint 3:**

- Clareira de Entrada jogável
- Transição caverna → floresta funcional
- Vegetação interativa implementada

---

#### Sprint 4: Caminho dos Cervos + Quest (2 semanas)

**Semana 1: NPCs Básicos**

- [ ] **Cervo-Broto**
  - [ ] Sprite 16x16 (idle + walk)
  - [ ] Animação de caminhada
  - [ ] NPCWanderAI configurado
  - [ ] 3 Cervos-Broto no caminho
  - [ ] Comportamento passivo (foge se atacado)
  
- [ ] **Esquilo Coletor**
  - [ ] Sprite 16x16 (idle + talk)
  - [ ] NPCStaticAI (fica na árvore)
  - [ ] NPCDialogue configurado
  - [ ] QuestGiverController configurado
  - [ ] Diálogo de introdução (PT-BR + EN)

**Semana 2: Quest + Puzzle**

- [ ] **Quest: "Colete 5 Flores Cristalinas"**
  - [ ] Criar CollectQuestData
  - [ ] Configurar recompensa (10 Cristais Nature)
  - [ ] Diálogo de aceitar quest
  - [ ] Diálogo de completar quest
  - [ ] Testar fluxo completo
  
- [ ] **Puzzle: Ponte de Vinhas**
  - [ ] Mecânica de crescimento de plantas
  - [ ] Placa de pressão ativa crescimento
  - [ ] Vinhas crescem formando ponte
  - [ ] Animação de crescimento (2s)
  - [ ] Acesso à Colmeia desbloqueado

**Entrega Sprint 4:**

- Caminho dos Cervos completo
- 3 NPCs funcionais
- 1 quest funcional end-to-end
- 1 puzzle de crescimento

---

#### Sprint 5: Colmeia Pequena (2 semanas)

**Semana 1: Área da Colmeia**

- [ ] **Estrutura de Mel**
  - [ ] Tileset de mel (hexágonos)
  - [ ] Plataformas de mel (sticky)
  - [ ] Física de sticky surface
  - [ ] Layout vertical (3 níveis)
  
- [ ] **Abelha Cristalina**
  - [ ] Sprite 16x16 (idle + fly)
  - [ ] Animação de voo
  - [ ] NPCPatrolAI configurado
  - [ ] 3 Abelhas patrulhando
  - [ ] Comportamento neutro (não ataca)
  
- [ ] **Cristais Verdes**
  - [ ] 5 cristais Nature espalhados
  - [ ] Posições estratégicas (requerem exploração)
  - [ ] Animação de coleta

**Semana 2: Puzzle Geométrico**

- [ ] **Pilares Hexagonais**
  - [ ] 3 pilares com cristais
  - [ ] Sistema de ativação (clicar/interagir)
  - [ ] Ordem correta: 1 → 3 → 2
  - [ ] Pista visual (flores no chão formam padrão)
  
- [ ] **Recompensa**
  - [ ] 15 Cristais Nature
  - [ ] Baú com item especial
  - [ ] Teleport point de volta à caverna
  - [ ] Atalho desbloqueado

**Entrega Sprint 5:**

- Colmeia Pequena completa
- 3 Abelhas patrulhando
- Puzzle geométrico funcional
- Loop completo: Caverna → Floresta → Caverna

---

### 🥷 Sprint 6: Mecânica de Agachar (2 semanas)

**Objetivo:** Implementar stealth básico

#### Semana 1: Input e Animação

- [ ] **Sistema de Input**
  - [ ] Input de agachar (segurar Ctrl/B/Circle)
  - [ ] Toggle crouch state no PlayerController
  - [ ] Restrição de movimento (velocidade = 0)
  - [ ] Cancelar ao soltar botão
  
- [ ] **Animação**
  - [ ] Sprite de slime achatado (16x8)
  - [ ] Animação de transição (0.3s)
  - [ ] Idle agachado
  - [ ] Transição de volta (0.3s)
  
- [ ] **SFX**
  - [ ] Som de agachar (squish)
  - [ ] Som de levantar (pop)

#### Semana 2: Sistema de Stealth

- [ ] **Detecção de Cobertura**
  - [ ] Raycast para detectar arbustos/objetos
  - [ ] Tag "Cover" em objetos
  - [ ] Indicador visual (ícone de olho)
  - [ ] Estado "hidden" quando coberto
  
- [ ] **Integração com IA**
  - [ ] NPCs não detectam jogador agachado + coberto
  - [ ] Quebrar perseguição se esconder
  - [ ] Teste com Abelhas (adicionar IA de Chase)
  
- [ ] **Puzzle de Stealth**
  - [ ] Área com 2 Abelhas guardas
  - [ ] Arbustos estratégicos
  - [ ] Objetivo: passar sem ser visto
  - [ ] Tutorial visual (ícone de agachar)

**Entrega Sprint 6:**

- Mecânica de agachar funcional
- Sistema de stealth básico
- Integração com IA testada
- Puzzle de stealth na Floresta

---

### ⚡ Sprint 7: Sistema de Habilidades (3 semanas)

**Objetivo:** 2 habilidades Tier 1 funcionais (Nature + Fire)

#### Semana 1: Infraestrutura

- [ ] **ScriptableObject de Habilidade**
  - [ ] AbilityData (nome, descrição, custo, cooldown)
  - [ ] Enum de elementos (Nature, Fire, Water, etc.)
  - [ ] Enum de tier (1, 2, 3)
  
- [ ] **Sistema de Stamina**
  - [ ] PlayerAttributesHandler: adicionar Stamina (100)
  - [ ] Regeneração automática (10/s)
  - [ ] Depleção ao usar habilidade
  - [ ] Indicador de "sem stamina"
  
- [ ] **Sistema de Cooldown**
  - [ ] AbilityManager (gerencia cooldowns)
  - [ ] UI de cooldown (overlay circular)
  - [ ] Bloqueio de input durante cooldown
  
- [ ] **UI de Habilidades**
  - [ ] 4 slots no HUD (Q, E, R, F)
  - [ ] Ícone da habilidade
  - [ ] Indicador de cooldown
  - [ ] Indicador de stamina insuficiente

#### Semana 2: Habilidade Nature - Crescimento Rápido

- [ ] **Implementação**
  - [ ] Input (Q)
  - [ ] Custo: 20 Stamina
  - [ ] Cooldown: 5s
  - [ ] Efeito: Cria vinhas em área 3x3
  
- [ ] **Mecânica**
  - [ ] Vinhas crescem do chão
  - [ ] Duram 10 segundos
  - [ ] Podem ser usadas como plataforma
  - [ ] Bloqueiam inimigos
  
- [ ] **VFX e SFX**
  - [ ] Partículas verdes (folhas)
  - [ ] Som de crescimento (whoosh + rustle)
  - [ ] Animação de vinhas crescendo
  
- [ ] **Teste**
  - [ ] Usar para resolver puzzle de ponte
  - [ ] Usar para bloquear Abelhas
  - [ ] Testar cooldown e stamina

#### Semana 3: Habilidade Fire - Bola de Fogo

- [ ] **Implementação**
  - [ ] Input (E)
  - [ ] Custo: 15 Stamina
  - [ ] Cooldown: 3s
  - [ ] Efeito: Projétil de fogo
  
- [ ] **Mecânica**
  - [ ] Projétil viaja em linha reta
  - [ ] Velocidade: 10 unidades/s
  - [ ] Dano: 10 HP
  - [ ] Explode ao colidir
  
- [ ] **VFX e SFX**
  - [ ] Partículas de fogo (trail)
  - [ ] Som de lançamento (whoosh)
  - [ ] Som de explosão (boom)
  - [ ] Screen shake ao explodir
  
- [ ] **Teste**
  - [ ] Destruir arbustos
  - [ ] Atacar Abelhas (se hostis)
  - [ ] Testar cooldown e stamina

**Entrega Sprint 7:**

- Sistema de habilidades funcional
- 2 habilidades Tier 1 implementadas
- Sistema de Stamina integrado
- VFX e SFX polidos

---

### 🎨 Sprint 8: Polimento e Balanceamento (2 semanas)

**Objetivo:** Preparar vertical slice para testes internos

#### Semana 1: Balanceamento

- [ ] **Economia de Cristais**
  - [ ] Ajustar drops de cristais
  - [ ] Balancear recompensas de quests
  - [ ] Testar progressão (10-15 cristais em 15 min)
  
- [ ] **Dificuldade de Puzzles**
  - [ ] Testar com jogadores frescos
  - [ ] Ajustar pistas visuais
  - [ ] Adicionar hints se necessário
  
- [ ] **Stamina e Habilidades**
  - [ ] Ajustar custos de stamina
  - [ ] Ajustar cooldowns
  - [ ] Testar regeneração
  
- [ ] **Flow de Gameplay**
  - [ ] Playthrough completo (15-20 min)
  - [ ] Identificar pontos de frustração
  - [ ] Ajustar ritmo (pacing)

#### Semana 2: Juice e Polimento

- [ ] **Screen Shake**
  - [ ] Ao atacar
  - [ ] Ao usar habilidades
  - [ ] Ao coletar cristais
  - [ ] Ao completar quest
  
- [ ] **Partículas de Impacto**
  - [ ] Ao destruir arbustos
  - [ ] Ao coletar itens
  - [ ] Ao ativar puzzles
  
- [ ] **Sons de UI**
  - [ ] Abrir/fechar menus
  - [ ] Aceitar/completar quest
  - [ ] Coletar itens
  - [ ] Notificações
  
- [ ] **Transições Suaves**
  - [ ] Fade in/out entre cenas
  - [ ] Animações de UI
  - [ ] Transições de câmera

#### Bug Fixing e Performance

- [ ] **Testes Completos**
  - [ ] Playthrough completo 3x
  - [ ] Testar todos os sistemas
  - [ ] Testar edge cases
  
- [ ] **Correção de Bugs**
  - [ ] Lista de bugs críticos
  - [ ] Priorizar por severidade
  - [ ] Corrigir todos os críticos
  
- [ ] **Performance**
  - [ ] Profiling (CPU + GPU)
  - [ ] Otimizar gargalos
  - [ ] Target: 60 FPS estável

#### Build e Documentação

- [ ] **Build Standalone**
  - [ ] Build para Windows (64-bit)
  - [ ] Testar build em máquina limpa
  - [ ] Verificar tamanho (~200-300 MB)
  
- [ ] **Documentação**
  - [ ] Controles (teclado + gamepad)
  - [ ] Objetivos do Alpha
  - [ ] Formulário de feedback
  - [ ] Instruções de instalação

**Entrega Sprint 8:**

- Alpha 1 polido e balanceado
- Build standalone funcional
- Documentação completa
- Pronto para testes internos

---

## 📊 Resumo de Entregas

### Conteúdo

- ✅ **Ninho do Slime** (100%)
  - Tutorial completo
  - Puzzle de placas de pressão
  - Transição para Floresta
  
- ✅ **Floresta Calma** (3 áreas)
  - Clareira de Entrada
  - Caminho dos Cervos
  - Colmeia Pequena
  
- ✅ **NPCs** (7 total)
  - 3 Cervos-Broto (passivos)
  - 3 Abelhas Cristalinas (patrulha)
  - 1 Esquilo Coletor (quest giver)
  
- ✅ **Conteúdo**
  - 1 quest funcional
  - 3 puzzles (placas, vinhas, hexágonos)
  - 15-20 minutos de gameplay

### Sistemas

- ✅ **Quest System** (100%)
  - Quest log UI
  - Quest tracker HUD
  - Integração completa
  
- ✅ **HUD Completo**
  - HP Bar
  - Stamina Bar
  - Contador de Cristais
  - Habilidades (4 slots)
  
- ✅ **Mecânica de Agachar**
  - Input e animação
  - Sistema de stealth
  - Integração com IA
  
- ✅ **Sistema de Habilidades**
  - 2 habilidades Tier 1
  - Sistema de cooldown
  - Sistema de Stamina
  - VFX e SFX

### Polimento

- ✅ Balanceamento completo
- ✅ Juice (shake, particles, sounds)
- ✅ Bug fixing
- ✅ Performance otimizada
- ✅ Build standalone

---

## 📅 Cronograma Detalhado

| Sprint | Duração | Início | Fim | Entrega |
|--------|---------|--------|-----|---------|
| Sprint 1 | 2 sem | 04/11 | 17/11 | Quest UI + HUD |
| Sprint 2 | 1 sem | 18/11 | 24/11 | Ninho completo |
| Sprint 3 | 2 sem | 25/11 | 08/12 | Clareira |
| Sprint 4 | 2 sem | 09/12 | 22/12 | Caminho + Quest |
| Sprint 5 | 2 sem | 23/12 | 05/01 | Colmeia |
| Sprint 6 | 2 sem | 06/01 | 19/01 | Agachar |
| Sprint 7 | 3 sem | 20/01 | 09/02 | Habilidades |
| Sprint 8 | 2 sem | 10/02 | 23/02 | Polimento |

**Total:** 16 semanas (4 meses)  
**Data de Conclusão:** 23/02/2026  
**Buffer:** 1 semana para imprevistos

---

## 🎯 Métricas de Sucesso

### Gameplay

- ✅ 12-15 minutos de gameplay polido
- ✅ 2 biomas jogáveis (Ninho + Floresta)
- ✅ 1 quest completável
- ✅ 3 puzzles funcionais
- ✅ 7 NPCs com IA funcional

### Sistemas

- ✅ Quest system 100% funcional
- ✅ HUD completo e informativo
- ✅ Mecânica de agachar + stealth
- ✅ 2 habilidades elementais
- ✅ Sistema de cristais funcionando

### Qualidade

- ✅ 60 FPS estável (PC)
- ✅ 0 bugs críticos
- ✅ Feedback visual/sonoro polido
- ✅ Transições suaves
- ✅ Balanceamento testado

### Testes

- ✅ 5-10 testers internos
- ✅ Feedback coletado
- ✅ Métricas de gameplay
- ✅ Lista de melhorias para Alpha 2

---

## 🚨 Riscos e Mitigações

### Riscos Identificados

1. **Sistema de Habilidades complexo**
   - Risco: Pode levar mais de 3 semanas
   - Mitigação: Simplificar mecânicas, focar em 2 habilidades apenas

2. **Level Design da Floresta**
   - Risco: Pode ser muito grande/pequeno
   - Mitigação: Prototipar no papel primeiro, iterar rapidamente

3. **Balanceamento de Stamina**
   - Risco: Pode ficar frustrante ou trivial
   - Mitigação: Playtests frequentes, ajustes iterativos

4. **Performance com partículas**
   - Risco: Muitas partículas podem causar lag
   - Mitigação: Object pooling, limitar partículas simultâneas

5. **Integração de sistemas**
   - Risco: Bugs ao integrar Quest + Dialogue + NPC
   - Mitigação: Testes unitários, integração gradual

### Plano B

Se atrasarmos:

- **Cortar:** Sistema de Evolução (mover para Alpha 2)
- **Simplificar:** Apenas 1 habilidade (Nature)
- **Reduzir:** Floresta com 2 áreas ao invés de 3
- **Adiar:** Polimento visual (focar em funcionalidade)

---

## 📝 Notas Finais

### Prioridades Absolutas (Não Negociáveis)

1. ✅ Quest system funcional end-to-end
2. ✅ HUD completo (HP, Stamina, Cristais)
3. ✅ Floresta Calma jogável (mínimo 2 áreas)
4. ✅ 1 quest completável
5. ✅ 2 puzzles funcionais
6. ✅ Mecânica de agachar
7. ✅ 1 habilidade elemental (mínimo)
8. ✅ Build standalone funcional

### Nice to Have (Se der tempo)

- 🎨 Música de fundo
- 🎨 Mais variações de NPCs
- 🎨 Animações de idle para NPCs
- 🎨 Partículas ambientais avançadas
- 🎨 Sistema de achievements
- 🎨 Tutorial tooltips

### Após Alpha 1

**Feedback a Coletar:**

- Dificuldade dos puzzles
- Clareza dos objetivos
- Feeling do movimento
- Balanceamento de stamina
- Diversão das habilidades
- Bugs encontrados
- Sugestões de melhoria

**Próximos Passos (Alpha 2):**

- Sistema de Evolução
- +6 Habilidades Tier 1
- Expandir Floresta (60%)
- Primeiro Rei Monstro
- Sistema de Amizade
- Save/Load completo

---

**Documento criado:** 03/11/2025  
**Última atualização:** 03/11/2025  
**Responsável:** Equipe de Desenvolvimento  
**Status:** 🟢 Em Progresso (30% → 55%)
