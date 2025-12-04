# The Slime King - Roadmap de Desenvolvimento

**Versão:** 2.7  
**Última Atualização:** 15/11/2025  
**Baseado em:** GDD v10.1 + TechMapping atual + Análise Completa de Código  
**Metodologia:** Desenvolvimento Iterativo em 6 Milestones

**Changelog v2.8:**

- ✅ **Sistema de Cristais Elementais COMPLETO** ⭐⭐
- ✅ **CrystalElementalData**: ScriptableObject com 6 tipos elementais
- ✅ **CrystalCounterUI**: Interface em tempo real com cores temáticas
- ✅ **GameManager**: Sistema integrado de contadores por tipo
- ✅ **ItemCollectable**: Correções críticas para atração/coleta
- ✅ **HUDContextMenu**: Ferramenta de setup automático de HUD
- ✅ **Auto-criação de GameManager**: Sistema inteligente de inicialização
- ✅ **Documentação expandida**: 4 novos guias técnicos
- ✅ Progresso geral atualizado: 60% → 65%

**Changelog v2.7:**

- ✅ **Sistema de HUD de Vida IMPLEMENTADO** ⭐
- ✅ **HealthUIManager**: Gerenciador principal com layout em grid
- ✅ **HeartUIElement**: Componente de coração com animações bounce
- ✅ **Integração automática** com PlayerAttributesHandler via eventos
- ✅ **Sprites visuais**: ui_hearthCounterOK/NOK implementados
- ✅ **Documentação completa**: HEART_HUD_SETUP.md criado
- ✅ Progresso geral atualizado: 55% → 60%

**Changelog v2.6:**

- ✅ **Ninho do Slime COMPLETO** (90% → 100%) ⭐
- ✅ **Floresta Calma - Clareira de Entrada COMPLETO** (0% → 100%) ⭐
- ✅ **Puzzle de stealth introdutório implementado** (fendas + empurrar pedra) ⭐
- ✅ **Sistema de Puzzles expandido** (8 categorias documentadas) ⭐
- ✅ Sistema de Combate simplificado (removido dano flutuante, críticos, abordagens alt.)
- ✅ GDD atualizado para v10.1
- ✅ Progresso geral atualizado: 35% → 40%

**Changelog v2.3:**

- ✅ Sistema de Quest INICIADO (QuestManager + QuestEvents + QuestGiverController)
- ✅ Correções de bugs críticos (delegate signature mismatch)
- ✅ Ferramentas de Editor expandidas (QuestSystemTestSceneSetup)
- ✅ Progresso geral atualizado: 25% → 27%

**Changelog v2.2:**

- ✅ Atualizado progresso de sistemas implementados (Nov 2025)
- ✅ Sistema de Inventário COMPLETO (20 slots + 3 equipamentos + 4 quick slots)
- ✅ Sistema de Diálogo COMPLETO (DialogueManager + UI + Localização)
- ✅ Sistema de UI/UX expandido (Pause Menu, Confirmation Dialog, Interaction Icons)
- ✅ Ferramentas de Editor expandidas (NPCDialogueQuickConfig, SceneSetupTool)
- ✅ Progresso geral atualizado: 15% → 25%

---

## 🎉 Sistemas Recentemente Implementados (Novembro 2025) - Atualização v2.6

### Sistema de Quest ✅ COMPLETO

- **QuestManager**: Gerenciamento centralizado de quests ✅
- **QuestEvents**: Sistema de eventos para comunicação desacoplada ✅
- **SaveEvents**: Sistema de eventos para save/load ✅
- **QuestGiverController**: Controller para NPCs que oferecem quests ✅
- **QuestNotificationController**: Notificações de quest na tela ✅
- **CollectQuestData**: ScriptableObject para quests de coleta ✅
- **ItemReward**: Sistema de recompensas de itens ✅
- **QuestProgress**: Tracking de progresso de quests ✅
- **QuestSaveData**: Persistência de dados de quest ✅
- **QuestChoiceUI**: UI para escolhas de quest ✅
- **DialogueChoiceHandler**: Integração com sistema de diálogo ✅
- **QuestManagerEditor**: Editor customizado para QuestManager ✅
- **QuestSystemTestSceneSetup**: Ferramenta de editor para setup de cena de teste ✅

**Funcionalidades Completas:**

- ✅ Aceitar quests via diálogo
- ✅ Entregar quests via diálogo
- ✅ Tracking automático de progresso de coleta
- ✅ Sistema de recompensas (itens + reputação)
- ✅ Persistência de dados (save/load)
- ✅ Integração completa com InventoryManager
- ✅ Indicadores visuais em NPCs

**Tipos de Quest Implementados:**

- ✅ Collect Quest (coletar X itens)

**Pendente:**

- [ ] Quest Log UI (lista completa de quests)
- [ ] Quest Tracker HUD (progresso na tela)
- [ ] Tipos adicionais de quest (Defeat, Deliver, Explore, Interact, Escort)

### Sistema de NPC 🚧 EM PROGRESSO

- **NPCController**: Controller base para NPCs ✅
- **NPCBehavior**: Comportamento base de NPCs ✅
- **NPCDialogue**: Sistema de diálogo para NPCs ✅
- **NPCDialogueInteraction**: Interação de diálogo com NPCs ✅
- **NPCFriendship**: Sistema de amizade com NPCs ✅
- **NPCData**: ScriptableObject com dados de NPC ✅
- **NPCConfigData**: Configuração de NPCs ✅
- **DialogueData**: Dados de diálogo ✅
- **LocalizedDialogueData**: Diálogos localizados ✅
- **FriendshipData**: Dados de amizade ✅
- **NPCEnums**: Enumerações de NPC (tipos, estados) ✅

**IA Básica Implementada:**

- **NPCWanderAI**: IA de vagueio aleatório ✅
- **NPCPatrolAI**: IA de patrulha em pontos ✅
- **NPCStaticAI**: IA estática (sem movimento) ✅

**Ferramentas de Editor:**

- **NPCDialogueQuickConfig**: Setup rápido de NPCs com diálogo ✅
- **NPCAnimatorSetup**: Setup de animadores ✅
- **NPCGizmosDrawer**: Visualização de gizmos ✅
- **NPCDataGenerator**: Gerador de dados de NPC ✅
- **NPCComponentConfigurator**: Configurador de componentes ✅
- **NPCBatchConfigurator**: Configuração em lote ✅

**Pendente:**

- [ ] Estados de IA avançados (Alert, Chase, Attack, Flee, Stunned)
- [ ] Sistema de percepção (visão, audição, proximidade)
- [ ] Integração completa com sistema de amizade
- [ ] Comportamentos diários (schedule)

### Sistema de Inventário ✅ COMPLETO

- **InventoryManager**: Gerenciamento de 20 slots + 3 equipamentos + 4 quick slots
- **InventoryUI**: Interface completa com grid, equipamentos e ações
- **ItemData**: ScriptableObject robusto com tipos e raridades
- **QuickSlotManager**: Sistema de quick slots integrado ao HUD
- **InventorySaveData**: Sistema de persistência de dados

### Sistema de Diálogo ✅ COMPLETO

- **DialogueManager**: Gerenciamento centralizado de diálogos
- **DialogueUI**: Interface com typewriter effect e escolhas
- **LocalizationManager**: Suporte a múltiplos idiomas (PT-BR, EN)
- **InteractionIcon**: Ícones flutuantes sobre NPCs
- **NPCDialogueQuickConfig**: Ferramenta de editor para setup rápido

### UI/UX Expandido ✅

- **PauseMenu**: Menu de pausa completo com acesso ao inventário
- **ConfirmationDialog**: Sistema de confirmação genérico
- **ItemActionPanel**: Painel de ações para itens
- **QuickSlotSelectionPanel**: Seleção de quick slots

### Ferramentas de Editor ✅

- **UnifiedExtraTools**: Ferramentas unificadas de desenvolvimento
- **SceneSetupTool**: Setup automático de cenas
- **DialogueSystemTestSceneSetup**: Cena de teste de diálogo
- **QuestSystemTestSceneSetup**: Cena de teste de quest ✅ NOVO

---

## 📊 Resumo Executivo

### Status Atual

- **Progresso Geral:** 65% completo ⬆️ (+5% desde última atualização)
- **Milestone Atual:** ALPHA 1 (Vertical Slice Interno)
- **Próximo Marco:** Dezembro 2025 ⚡ (Adiantado 4 semanas!)
- **Lançamento Previsto:** Maio 2027 ⚡
- **Aceleração:** Gen AI (Vibe Coding) - Redução de 33-37% no tempo
- **Sistemas Recentes:** Sistema de cristais elementais completo + HUD tools + 3 áreas da Floresta Calma

### Milestones Planejados (Acelerados)

1. 🟢 **ALPHA 1** - Jan 2026 (2 meses) ⚡ - Vertical Slice Interno
2. � **ALPAHA 2** - Abr 2026 (5 meses) ⚡ - Vertical Slice Público
3. � **SBETA** - Out 2026 (10 meses) ⚡ - Feature Complete
4. � **STLEAM NEXT FEST** - Jan 2027 (13 meses) ⚡ - Demo Pública
5. 🚀 **RELEASE** - Mai 2027 (16 meses) ⚡ - Lançamento v1.0
6. 🎁 **POST-RELEASE** - Jul 2027 (18 meses) - Suporte e DLC

### Entregas por Milestone (Ajustadas)

| Milestone | Gameplay | Biomas | Reis | Sistemas | NPCs/Inimigos | Status |
|-----------|----------|--------|------|----------|---------------|--------|
| ALPHA 1 | 12-15 min | 2 (mínimo) | 0 | 65% ⬆️ | 3/2 | 🚧 Em Progresso |
| ALPHA 2 | 40-50 min | 2 (60%) | 1 | 60% | 7/4 | 📋 Planejado |
| BETA | 8-10 horas | 5 (3 completos) | 3 | 90% | 20+/10+ | 📋 Planejado |
| NEXT FEST | 2-2.5 horas | 3 (demo) | 2 | 95% | 15+/8+ | 📋 Planejado |
| RELEASE | 20-30 horas | 7 completos | 10 | 100% | 40+/20+ | 📋 Planejado |
| POST-RELEASE | +DLC | +DLC | +DLC | 100% | +DLC | 🔮 Futuro |

### Impacto do Gen AI (Vibe Coding)

**Redução de Tempo por Milestone:**

- ALPHA 1: 12 sem → 8 sem (33% mais rápido)
- ALPHA 2: 16 sem → 10 sem (37% mais rápido)
- BETA: 28 sem → 18 sem (36% mais rápido)
- NEXT FEST: 12 sem → 8 sem (33% mais rápido)
- RELEASE: 32 sem → 20 sem (37% mais rápido)

**Total:** 100 semanas → 64 semanas (36% de redução)  
**Economia:** ~9 meses de desenvolvimento

---

## ⚡ Metodologia: Vibe Coding com Gen AI

### Aceleração de Desenvolvimento

Este projeto utiliza **Vibe Coding** (desenvolvimento assistido por Gen AI) para acelerar significativamente o processo de desenvolvimento:

**Áreas de Maior Impacto:**

- �  **Código Boilerplate:** 60-70% mais rápido (sistemas, managers, ScriptableObjects)
- 🎨 **Implementação de Sistemas:** 40-50% mais rápido (IA, puzzles, quests)
- 🐛 **Debug e Refatoração:** 30-40% mais rápido (identificação e correção de bugs)
- 📝 **Documentação:** 50-60% mais rápido (comentários, READMEs, specs)
- 🎮 **Prototipagem:** 70-80% mais rápido (testes rápidos de mecânicas)

**Áreas de Menor Impacto:**

- 🎨 Arte e Animação: Gen AI limitado (ainda requer trabalho manual)
- 🎵 Áudio e Música: Gen AI limitado (composição manual)
- 🎯 Game Design: Gen AI auxilia, mas decisões são humanas
- 🧪 Playtesting: Requer jogadores reais

**Resultado Esperado:**

- Redução média de **35-40%** no tempo de desenvolvimento
- Economia de **~9 meses** no cronograma total
- Mais tempo para polimento e iteração

---

## 📊 Status Geral do Projeto

### Legenda

- ✅ **Completo** - Implementado e funcional
- 🚧 **Em Progresso** - Parcialmente implementado
- 📋 **Planejado** - Especificado no GDD, aguardando implementação
- 🔮 **Futuro** - Planejado para versões posteriores
- ⚡ **Acelerado** - Milestone acelerada com Gen AI

---

## 🎯 Milestones de Desenvolvimento

### 🟢 **ALPHA 1 - Vertical Slice Interno** (Foco Atual)

**Data Alvo:** Janeiro 2026 (2 meses) ⚡ Acelerado com Gen AI  
**Objetivo:** Demonstrar core gameplay com 2 biomas jogáveis

**Conteúdo:**

- ✅ **Ninho do Slime** (100% completo) ⬆️
  - ✅ Tutorial de movimento, ataque, destruição, coleta
  - ✅ **Puzzle de stealth introdutório implementado** ⭐ NOVO
    - ✅ **Tipo: Stealth Timing** - Esgueirar por fendas usando sistema de agachar
    - ✅ **Tipo: Física & Empurrar** - Puzzle de empurrar pedra para liberar escada
    - ✅ **Tipo: Busca & Transporte** - Recompensa: cogumelos de cura acessíveis
  
- ✅ **Floresta Calma - Clareira de Entrada** (100% completo) ⭐ NOVO
  - ✅ Transição suave da caverna
  - ✅ Introdução ao ambiente florestal
  - ✅ Pontos de coleta básicos
  
  - 📋 **3 NPCs básicos:**
    - 1 Cervo-Broto (passivo, wander)
    - 1 Esquilo Coletor (quest giver)
    - 1 Abelha Cristalina (patrulha)
  
  - 📋 **2 Inimigos básicos:**
    - Abelha Agressiva (persegue jogador)
    - Arbusto Espinhoso (estático, dano por contato)
  
  - 📋 **Conteúdo:**
    - 1 quest simples: "Colete 5 Flores Cristalinas" (**Tipo: Busca & Transporte**)
    - **Puzzle 1: Ponte de Vinhas** (**Tipo: Plataforma + Elementais**)
      - Usar habilidade Nature para fazer vinhas crescerem
      - Criar plataformas temporárias para atravessar
    - **Puzzle 2: Pilares Hexagonais** (**Tipo: Lógica + Física**)
      - Empurrar pilares para formar padrão geométrico
      - Sequência correta ativa portal

**Sistemas (MVP):**

- ✅ **Mecânica de Agachar Avançada** (sistema de stealth completo implementado) ⭐ NOVO
  - ✅ **Sistema de Stealth Visual**: Fade semi-transparente após 2s agachado
  - ✅ **Detecção de Cobertura**: Physics2D.OverlapCircle + sorting Y
  - ✅ **Multi-SpriteRenderer**: Fade aplicado a todos subobjetos visuais
  - ✅ **StealthEvents**: Comunicação desacoplada para sistemas de IA
  - ✅ **GameManager Integration**: Estado acessível para inimigos
- ✅ Sistema de Cristais Elementais (contador UI + 6 tipos + auto-criação GameManager) ⭐
- 📋 Quest System básico (1 tipo: Collect)
- 📋 2 Habilidades Elementais Tier 1 (Nature + Fire)
- 🎆 **Sistema de Puzzles Expandido** (8 categorias identificadas) ⭐ NOVO
  - ✅ **Física & Empurrar**: Pedras, objetos pesados (já implementado)
  - ✅ **Stealth Timing**: Agachar + timing + cobertura (sistema avançado implementado) ⭐
  - ✅ **Busca & Transporte**: Quest de coleta (já implementado)
  - ✅ **Quebra & Destruição**: RockDestruct + BushDestruct (já implementado)
  - 📋 **Plataforma**: Navegação vertical precisa
  - 📋 **Elementais**: Habilidades para ativar mecanismos
  - 📋 **Lógica**: Padrões e sequências
  - 📋 **Ambientais**: Interação com cenário
- ⏸️ Sistema de Evolução (ADIADO para Alpha 2)
- ⏸️ Outras habilidades (ADIADO para Alpha 2)

**Entrega:**

- 12-15 minutos de gameplay polido
- Build standalone (PC)
- Testes internos (5-10 pessoas)

**Estimativa com Gen AI:**

- Sem Gen AI: ~12 semanas
- Com Gen AI: ~8 semanas (redução de 33%)
- Prazo com buffer: 10 semanas (2.5 meses)

---

### 🟢 **ALPHA 2 - Vertical Slice Público**

**Data Alvo:** Abril 2026 (5 meses) ⚡ Acelerado com Gen AI  
**Objetivo:** Expandir conteúdo e preparar para testes externos

**Conteúdo:**

- **Ninho do Slime** (Expandido)
  - Tutorial de agachar e habilidades
  - Área secreta com recompensa
  - Polimento visual e sonoro
  - Música tema da caverna
  
- **Floresta Calma** (Expandida 60%)
  - +2 áreas: Bosque Profundo, Colmeia Suspensa
  - +4 NPCs (2 Cervos, 1 Esquilo, 1 Abelha)
  - +2 Inimigos (Lobo Selvagem, Vespa Gigante)
  - +2 quests + 2 puzzles
  - Rainha Melífera (primeiro Rei Monstro - versão simplificada)

**Sistemas:**

- Sistema de Evolução (Filhote → Adulto → Grande Slime)
- Sistema de Amizade (3 níveis - simplificado)
- +6 Habilidades Elementais (completar Tier 1: 4 elementos)
- Sistema de Aura Elemental (brilho monocromático)
- IA de Inimigos (6 estados: Idle, Patrol, Alert, Chase, Attack, Return)
- Save/Load básico (posição, inventário, progresso)
- Quest System (2 tipos: Collect + Defeat)

**Entrega:**

- 40-50 minutos de gameplay
- Closed Alpha (30-50 testers selecionados)
- Feedback e iteração rápida

**Estimativa com Gen AI:**

- Sem Gen AI: ~16 semanas
- Com Gen AI: ~10 semanas (redução de 37%)
- Prazo com buffer: 12 semanas (3 meses adicionais)

---

### 🟡 **BETA - Feature Complete**

**Data Alvo:** Outubro 2026 (10 meses) ⚡ Acelerado com Gen AI  
**Objetivo:** Todos os sistemas implementados, 60% do conteúdo

**Conteúdo:**

- **3 Biomas Completos:**
  - Ninho do Slime (100%)
  - Floresta Calma (100%) - 2 Reis Monstros
  - Lago Espelhado (100%) - 1 Rei Monstro
  
- **2 Biomas Parciais:**
  - Área Rochosa (60%)
  - Pântano das Névoas (40%)

**Sistemas:**

- Sistema de Evolução completo (até Rei Slime - 5 reconhecimentos)
- 21 Habilidades Elementais (7 elementos x 3 tiers)
- Sistema de Seguidores (até 3)
- Sistema de Lar (4 expansões)
- IA completa (10 estados)
- Quest System completo (40+ quests)
- Cutscene System (4 tipos)
- Ciclo Dia/Noite (24 min = 1 dia)
- UI/UX completo
- Sistema de Puzzles (mecânicas principais)

**Entrega:**

- 8-10 horas de gameplay
- Open Beta (500-1000 testers)
- Balanceamento e polimento

**Estimativa com Gen AI:**

- Sem Gen AI: ~28 semanas
- Com Gen AI: ~18 semanas (redução de 36%)
- Prazo com buffer: 22 semanas (5.5 meses adicionais)

- 8-12 horas de gameplay
- Open Beta (1000+ testers)
- Balanceamento e polimento

---

### 🔵 **STEAM NEXT FEST BETA**

**Data Alvo:** Janeiro 2027 (13 meses) ⚡ Acelerado com Gen AI  
**Objetivo:** Demo pública para Steam Next Fest

**Conteúdo:**

- **Demo Polida (2-2.5 horas):**
  - Ninho do Slime (100% polido)
  - Floresta Calma (100% polido)
  - Lago Espelhado (primeiras 2 áreas)
  - 2 Reis Monstros jogáveis (Rainha Melífera + Imperador Escavarrok)
  
**Foco:**

- Polimento extremo da demo (juice, feedback, transições)
- Trailer de anúncio (1-2 minutos)
- Página Steam otimizada (screenshots, GIFs, descrição)
- Presskit completo
- Wishlist campaign (redes sociais, influencers)
- Localização EN + PT-BR

**Entrega:**

- Demo standalone (Steam)
- Participação no Steam Next Fest (Fevereiro 2027)
- Meta: 5000+ wishlists
- Coleta de feedback e métricas

**Estimativa com Gen AI:**

- Sem Gen AI: ~12 semanas
- Com Gen AI: ~8 semanas (redução de 33%)
- Prazo com buffer: 10 semanas (2.5 meses adicionais)

---

### 🚀 **RELEASE - Versão 1.0**

**Data Alvo:** Maio 2027 (16 meses) ⚡ Acelerado com Gen AI  
**Objetivo:** Lançamento completo no Steam e Switch

**Conteúdo:**

- **7 Biomas Completos:**
  - Ninho do Slime
  - Floresta Calma
  - Lago Espelhado
  - Área Rochosa
  - Pântano das Névoas
  - Câmaras de Lava
  - Pico Nevado
  
- **10 Reis Monstros**
- **80+ Quests** (reduzido de 100+ para escopo realista)
- **40+ Puzzles** (reduzido de 50+ para escopo realista)
- **Sistema Sazonal completo**

**Sistemas:**

- Todos os sistemas 100% implementados
- Evolução até Rei Slime Transcendente (10 reconhecimentos)
- Achievements completos (30-40 achievements)
- Localização (EN, PT-BR, ES, FR, DE)
- Cloud Save (Steam)
- Controller support completo (Xbox, PlayStation, Switch)
- Acessibilidade básica

**Entrega:**

- 20-30 horas de gameplay (ajustado para realista)
- Lançamento Steam (PC)
- Switch port em desenvolvimento paralelo
- Day 1 patch preparado
- Suporte pós-lançamento planejado

**Estimativa com Gen AI:**

- Sem Gen AI: ~32 semanas
- Com Gen AI: ~20 semanas (redução de 37%)
- Prazo com buffer: 24 semanas (6 meses adicionais)

---

### 🎁 **POST-RELEASE - Suporte e Expansão**

**Data Alvo:** Julho 2027 (2 meses pós-lançamento)  
**Objetivo:** Suporte contínuo e preparação para DLC

**Atividades:**

- **Mês 1-2 (Maio-Junho 2027):**
  - Bug fixing crítico (hotfixes)
  - Patches de balanceamento baseados em métricas
  - Otimização de performance (PC e Switch)
  - Suporte à comunidade (Discord, Steam forums)
  - Análise de analytics e feedback
  
- **Mês 3-4 (Julho-Agosto 2027):**
  - Patch de conteúdo gratuito (QoL improvements)
  - Planejamento de DLC baseado em feedback
  - Conceito de novos biomas
  - Protótipos de novas mecânicas
  
**Possíveis DLCs (Q4 2027 - Q1 2028):**

- **DLC 1: "Cavernas Sombrias"** (3-4 horas)
  - Duquesa Solibrida expandida
  - Novo bioma: Cavernas Profundas
  - 10+ quests, 5+ puzzles
  - Preço: $4.99
  
- **DLC 2: "Templo Cristalino"** (3-4 horas)
  - Grão-Sacerdote Luminescente
  - Novo bioma: Templo Ancestral
  - 10+ quests, 5+ puzzles
  - Preço: $4.99
  
- **DLC 3: "Jardim Secreto"** (2-3 horas)
  - Matriarca Flores expandida
  - Área secreta na Floresta Calma
  - 8+ quests, 4+ puzzles
  - Preço: $3.99

**Estimativa com Gen AI:**

- Cada DLC: ~8-10 semanas de desenvolvimento
- Lançamento trimestral (Q4 2027, Q1 2028, Q2 2028)

---

## 📅 Timeline Visual (Acelerada com Gen AI)

```
2025 Nov ████████████████████████████████████████████████████████████████ (Atual - 65% completo) ⬆️
         ↑ Sistema de cristais elementais + HUD completo + 3 áreas Floresta Calma
2026 Jan ████████████████ ALPHA 1 ✓ ⚡
2026 Abr ████████████████ ALPHA 2 ✓ ⚡
2026 Out ████████████████ BETA ✓ ⚡
2027 Jan ████████████████ STEAM NEXT FEST ✓ ⚡
2027 Mai ████████████████ RELEASE 🚀 ⚡
2027 Jul ████████████████ POST-RELEASE 🎁
```

**Total de Desenvolvimento:** 18 meses (Nov 2025 → Mai 2027)  
**Economia com Gen AI:** ~9 meses (de 27 meses para 18 meses)  
**Progresso Atual:** 65% (Nov 2025) - Muito Adiantado! ✅✅

### Comparação: Tradicional vs Gen AI

| Milestone | Tradicional | Com Gen AI | Economia |
|-----------|-------------|------------|----------|
| ALPHA 1 | 12 semanas | 8 semanas | 4 semanas |
| ALPHA 2 | 16 semanas | 10 semanas | 6 semanas |
| BETA | 28 semanas | 18 semanas | 10 semanas |
| NEXT FEST | 12 semanas | 8 semanas | 4 semanas |
| RELEASE | 32 semanas | 20 semanas | 12 semanas |
| **TOTAL** | **100 semanas** | **64 semanas** | **36 semanas** |
| | **(23 meses)** | **(16 meses)** | **(9 meses)** |

## 📊 Tracking de Cronograma (Original vs Atual)

### Histórico de Atualizações de Timeline

| Milestone | Data Original | Data Atual | Status | Variação | Motivo |
|-----------|---------------|------------|--------|----------|---------|
| **ALPHA 1** | Fev 2026 | **Jan 2026** | 🟢 Adiantado | **-4 semanas** | Sistemas base já implementados |
| **ALPHA 2** | Jun 2026 | **Abr 2026** | 🟡 Mantido | -2 semanas | Aceleração com Gen AI |
| **BETA** | Jan 2027 | **Out 2026** | 🟡 Mantido | -3 meses | Progressão acelerada |
| **STEAM NEXT FEST** | Abr 2027 | **Jan 2027** | 🟡 Mantido | -3 meses | Conforme planejamento |
| **RELEASE** | Ago 2027 | **Mai 2027** | 🟡 Mantido | -3 meses | Timeline Gen AI |
| **POST-RELEASE** | Out 2027 | **Jul 2027** | 🟡 Mantido | -3 meses | Acompanha release |

### Análise de Desvios

**🟢 Adiantamentos Identificados:**

- **ALPHA 1**: Adiantado 4 semanas devido a sistemas core já implementados
  - PlayerController, Quest, Inventário, Diálogo já 90%+ completos
  - Biomas base (Ninho + Clareira) já implementados
  - Foco mudou para conteúdo ao invés de sistemas básicos

**🔄 Riscos Monitorados:**

- **Dependência de Gen AI**: 35% da aceleração depende de produtividade assistida
- **Scope Creep**: Novos features podem atrasar milestones
- **Polimento**: Tempo adequado para polish pode ser subestimado

**📈 Fatores de Aceleração:**

- Sistemas arquiteturais robustos já implementados
- Ferramentas de editor customizadas (12+ ferramentas)
- Pipeline de desenvolvimento otimizado
- Gen AI reduzindo 35-40% do tempo de código

### Monitoramento Ativo

**Próximas Revisões:**

- [ ] **28 Nov 2025**: Review Sprint 2 ALPHA 1
- [ ] **12 Dez 2025**: Review Sprint 4 ALPHA 1  
- [ ] **02 Jan 2026**: Review final ALPHA 1
- [ ] **09 Jan 2026**: Entrega ALPHA 1

**Métricas de Acompanhamento:**

- **Velocity Semanal**: Target 12.5% progresso/semana ALPHA 1
- **Burndown de Features**: Tracking via ALPHA-1-Checklist.md
- **Performance Gen AI**: % de código gerado vs manual
- **Qualidade**: Bugs/semana, tempo de polish necessário

---

## 🎯 Fase 1: Fundação Técnica (Core Systems)

### 1.1 Arquitetura Base ✅ COMPLETO

- [x] Unity 6.2 com URP configurado
- [x] Estrutura de pastas organizada (Code/Data/Editor/Gameplay/Systems/Visual)
- [x] Sistema de Managers (ManagerSingleton pattern) ⭐ ROBUSTO
- [x] GameManager (ciclo de vida, preload de cenas)
- [x] CameraManager (Cinemachine integrado)
- [x] SimpleCameraFollow (câmera simples para testes)
- [x] SceneTransitionManager (transições visuais)
- [x] DialogueManager (gerenciamento de diálogos)
- [x] DialogueChoiceHandler (escolhas em diálogos)
- [x] LocalizationManager (i18n PT-BR/EN)
- [x] Input System (Unity Input System)
- [x] ScreenEffectsManager (vinheta, transições)

**Nota:** Todos os managers utilizam o padrão ManagerSingleton<T> para garantir consistência e evitar duplicação de código.

### 1.2 Sistema de Cenas e Transições ✅

- [x] TeleportManager (teleporte entre pontos)
- [x] TeleportPoint (pontos de teleporte)
- [x] Cross-scene teleport (teleporte entre cenas)
- [x] ScreenEffectsManager (vinheta, transições)
- [x] TeleportTransitionHelper (efeitos visuais)
- [x] SceneSetupValidator (validação automática de cenas) ⭐ ROBUSTO
- [x] InitialCaveScreenController (controle da cena tutorial)
- [x] TitleScreenController (tela inicial com sequência animada)

**Nota:** O SceneSetupValidator verifica automaticamente a presença de managers essenciais em cada cena, prevenindo erros de runtime.

### 1.3 Ferramentas de Editor ✅ COMPLETO

**Ferramentas Gerais:**

- [x] UnifiedExtraTools (ferramentas gerais unificadas)
- [x] CameraSetupTools (setup de câmera)
- [x] SceneSetupTool (setup automático de cenas)
- [x] GizmosHelper (visualização de colliders)
- [x] PolygonGizmosHelper (gizmos de polígonos)
- [x] ProjectSettingsExporter (exportação de settings)
- [x] CreateExampleItems (criação de itens de exemplo)

**Ferramentas de Configuração Rápida:**

- [x] BushQuickConfig (configuração de arbustos)
- [x] ItemQuickConfig (configuração de itens)
- [x] NPCDialogueQuickConfig (configuração rápida de NPCs com diálogo)

**Ferramentas de NPC (QuickWins):**

- [x] NPCAnimatorSetup (setup de animadores)
- [x] NPCGizmosDrawer (visualização de gizmos de NPC)
- [x] NPCDataGenerator (gerador de dados de NPC)
- [x] NPCComponentConfigurator (configurador de componentes)
- [x] NPCBatchConfigurator (configuração em lote)

**Ferramentas de Setup de Cena:**

- [x] DialogueSystemTestSceneSetup (setup de cena de teste de diálogo)
- [x] QuestSystemTestSceneSetup (setup de cena de teste de quest) ⭐ NOVO

**Editores Customizados:**

- [x] QuestManagerEditor (editor customizado para QuestManager) ⭐ NOVO
- [x] ItemRewardDrawer (drawer customizado para recompensas)

---

## 🎮 Fase 2: Gameplay Core

### 2.1 Controle do Jogador ✅ COMPLETO (Core)

**Implementado:**

- [x] PlayerController (movimento básico 8 direções)
- [x] PlayerInput (Unity Input System)
- [x] Rigidbody2D physics
- [x] Animação básica (Animator)
- [x] AttackHandler (ataque básico)
- [x] **PlayerAttributesHandler (HP, atributos, skill points)** ✅
- [x] SpecialMovementPoint (encolher/deslizar)
- [x] InteractivePointHandler (pontos de interação)
- [x] **Mecânica de Agachar COMPLETA** ✅
  - [x] Input de agachar (segurar botão)
  - [x] Animação de achatar (parâmetro IsHiding)
  - [x] Sistema de stealth básico (slime fica parado)
  - [x] Integração com Animator
  - [x] Lógica de movimento restrito durante agachar
  - [x] Estados visuais (direction, crouch animation)

**Pendente:**

- [ ] Sistema completo de detecção de cobertura para stealth
- [ ] Puzzles específicos usando mecânica de agachar
  - [ ] Indicador visual (ícone de olho)
  - [ ] Integração com sistema de inimigos
- [ ] Movimento gelatinoso aprimorado (bounce animation)
- [ ] Rastro de gosma visual
- [ ] Espremer por espaços apertados

### 2.2 Sistema de Atributos ✅ COMPLETO (Core)

- [x] **PlayerAttributesHandler (HP, atributos básicos)** ✅
- [x] **TakeDamage / Heal** ✅
- [x] **Skill Points (adicionar/gastar)** ✅
- [x] **Sistema de eventos (OnHealthChanged, OnPlayerDied, OnSkillPointsChanged)** ✅
- [x] **Integração com InventoryManager para consumíveis** ✅
- [ ] Sistema de Evolução (Filhote → Adulto → Grande → Rei → Transcendente)
- [ ] Sistema de Reputação (invisível, 5 níveis)
- [ ] Tracking de conquistas para evolução

### 2.3 Sistema de Combate 📋

- [x] AttackHandler (ataque básico)
- [x] Detecção de colisão com inimigos
- [ ] Resistências elementais

### 2.4 Sistema de Itens ✅ COMPLETO

**Sistema de Coleta:**

- [x] ItemCollectable (coleta de itens + atração magnética) ⭐ ATUALIZADO
- [x] CollectableItemData (ScriptableObject legado)
- [x] BounceHandler (física de bounce)
- [x] ItemBuffHandler (buffs temporários)
- [x] DropController (drop de itens)

**Sistema Moderno de Itens:**

- [x] ItemData (ScriptableObject completo)
- [x] ItemType (enum: Consumable, Material, Quest, Equipment)
- [x] EquipmentType (enum: Amulet, Ring, Cape)
- [x] ItemReward (sistema de recompensas)

**Sistema de Cristais Elementais:** ⭐ NOVO

- [x] CrystalType (enum: Nature, Fire, Water, Shadow, Earth, Air)
- [x] CrystalElementalData (ScriptableObject de configuração)
- [x] CrystalCounterUI (interface em tempo real)
- [x] GameManager (contadores integrados por tipo)
- [x] Auto-criação de GameManager quando necessário
- [x] Atração magnética inteligente (2.5f unidades, 4.0f velocidade)
- [x] Coleta automática com detecção de proximidade (0.5f unidades)
- [x] Sistema de timeout para evitar cristais "órfãos"

**Itens de Exemplo Criados:**

- [x] CogumeloDeCura (item de cura)
- [x] FrutaDeCura (item de cura)
- [x] CristalElemental (cristal coletável)
- [x] MaterialDeNinho (material de crafting)
- [x] 6 tipos de cristais elementais configuráveis

### 2.5 Sistema de Inventário ✅ COMPLETO

- [x] **Estrutura do Inventário** ⭐ IMPLEMENTADO
  - [x] Grid 5x4 (20 slots fixos)
  - [x] InventoryManager (singleton)
  - [x] InventorySlot (classe de dados)
  - [x] Stacking automático (máx 99 por slot)
  - [x] 3 slots de equipamento (Amulet, Ring, Cape)
  - [x] 4 quick slots (direcionais do controle)
  
- [x] **UI do Inventário** ⭐ IMPLEMENTADO
  - [x] InventoryUI (painel principal)
  - [x] InventorySlotUI (representação visual)
  - [x] EquipmentSlotUI (slots de equipamento)
  - [x] QuickSlotUI (HUD quick slots)
  - [x] ItemActionPanel (ações: Usar/Equipar/Atribuir/Descartar)
  - [x] QuickSlotSelectionPanel (seleção de quick slot)
  - [x] ConfirmationDialog (confirmação de ações)
  
- [x] **Tipos de Itens** ⭐ IMPLEMENTADO
  - [x] Consumíveis (poções, comida, buffs)
  - [x] Materiais de Crafting
  - [x] Itens de Quest (não descartáveis)
  - [x] Equipamentos (amuletos, anéis, capas)
  
- [x] **Gerenciamento** ⭐ IMPLEMENTADO
  - [x] Adicionar/Remover itens
  - [x] Usar/Equipar/Descartar
  - [x] Atribuir a quick slots
  - [x] Sistema de save/load (InventorySaveData)
  - [x] Integração com PauseMenu
  
- [ ] **Pendente (Alpha 2)**
  - [ ] Drag and drop para reorganizar
  - [ ] Filtros por categoria
  - [ ] Borda colorida por raridade
  - [ ] Dividir stacks
  - [ ] Sistema de favoritar

### 2.6 Sistema de Diálogo ✅ COMPLETO

- [x] **DialogueManager** ⭐ IMPLEMENTADO
  - [x] Singleton pattern (ManagerSingleton)
  - [x] Sistema de eventos (OnDialogueStart/End)
  - [x] Controle de fluxo de diálogo
  - [x] Integração com LocalizationManager
  - [x] Suporte a múltiplos idiomas
  
- [x] **UI de Diálogo** ⭐ IMPLEMENTADO
  - [x] DialogueUI (implementa IDialogueUI)
  - [x] Caixa de diálogo com fade in/out
  - [x] Portrait do NPC (animado)
  - [x] Efeito de digitação (typewriter)
  - [x] Indicador de "mais texto"
  - [x] Botão de continuar/skip
  - [x] Sistema de escolhas (botões)
  
- [x] **LocalizationManager** ⭐ IMPLEMENTADO
  - [x] Singleton pattern
  - [x] Carregamento de JSON (PT-BR, EN)
  - [x] Cache de diálogos em memória
  - [x] Fallback para inglês
  - [x] Suporte a múltiplos idiomas
  
- [x] **Ferramentas de Editor** ⭐ IMPLEMENTADO
  - [x] NPCDialogueQuickConfig (setup rápido de NPCs)
  - [x] DialogueSystemTestSceneSetup (cena de teste)
  - [x] DialogueSystemSettings (configurações centralizadas)
  
- [x] **InteractionIcon** ⭐ IMPLEMENTADO
  - [x] Ícone flutuante sobre NPCs
  - [x] Animações (fade, bounce)
  - [x] Segue posição do NPC em world space
  
- [ ] **Pendente (Alpha 2)**
  - [ ] Expressões faciais (feliz, triste, surpreso, bravo)
  - [ ] Partículas emocionais
  - [ ] SFX de emoção
  - [ ] Sistema de memória (tracking de diálogos)
  - [ ] Diálogos condicionais (baseado em progresso)
  - [ ] Integração com Quest System
  - [ ] Integração com Friendship System

### 2.7 Sistema de Save/Load 🚧 EM PROGRESSO

**Estrutura de Dados Implementada:**

- [x] InventorySaveData (dados de inventário) ✅
- [x] QuestSaveData (dados de quests) ✅
- [x] SaveEvents (sistema de eventos de save) ✅

**Pendente:**

- [ ] **Pontos de Save**
  - [ ] Save automático (cenas, quests, evolução, 5 min)
  - [ ] Save manual (pontos de descanso, fogueiras)
  - [ ] Animação e confirmação visual
  
- [ ] **Dados Salvos Adicionais**
  - [ ] Progresso do Jogador (posição, evolução, HP, reputação, cristais)
  - [ ] Progresso de Mundo (NPCs, diálogos, amizades, Reis)
  - [ ] Expansões do Lar
  - [ ] Mundo Persistente (itens coletados, baús, puzzles, áreas)
  - [ ] Configurações
  
- [ ] **Slots de Save**
  - [ ] 3 slots independentes
  - [ ] Screenshot do último save
  - [ ] Informações (nome, tempo, evolução, Reis)
  - [ ] Copiar/Deletar/Renomear
  
- [ ] **Sistema de Backup**
  - [ ] Auto-backup a cada 30 min
  - [ ] Mantém últimos 3 backups
  - [ ] Recuperação de save corrompido
  
- [ ] **Cloud Save (Steam)**
  - [ ] Sincronização automática
  - [ ] Resolução de conflitos
  - [ ] Indicador de sincronização
  
- [ ] **Morte e Respawn**
  - [ ] Sem punição (mantém tudo)
  - [ ] Respawn no último save
  - [ ] Opções: Respawn/Load Save/Main Menu
  
- [ ] **Implementação Técnica**
  - [ ] SaveManager centralizado
  - [ ] JSON serializado
  - [ ] Criptografia leve (anti-cheat)
  - [ ] Compressão

---

## 🌟 Fase 3: Sistemas Elementais

### 3.1 Cristais Elementais ✅ COMPLETO ⭐

- [x] Prefab de cristal básico (crystalA)
- [x] 6 tipos de cristais implementados (Nature, Fire, Water, Shadow, Earth, Air)
- [x] Sistema de contador completo (não ocupa inventário) ⭐
- [x] CrystalElementalData (ScriptableObject de configuração)
- [x] CrystalCounterUI (interface em tempo real com cores temáticas)
- [x] GameManager (contadores integrados por tipo)
- [x] ItemCollectable (atração magnética + coleta automática)
- [x] Auto-criação de GameManager quando necessário
- [ ] Nodos de cristal no mundo (respawn diário)
- [ ] Drops de inimigos
- [ ] Recompensas de puzzles/quests

### 3.2 Sistema de Árvore de Habilidades 📋

- [ ] **Estrutura da Árvore**
  - [ ] 7 árvores elementais independentes
  - [ ] 3 tiers verticais por árvore
  - [ ] Progressão linear (Tier 1 → 2 → 3)
  - [ ] Total: 21 habilidades (7 × 3)
  
- [ ] **Desbloqueio**
  - [ ] Árvore desbloqueia ao evoluir para Adulto
  - [ ] Tier 2 desbloqueia ao evoluir para Grande Slime
  - [ ] Tier 3 desbloqueia ao evoluir para Rei Slime
  
- [ ] **Custo de Habilidades**
  - [ ] Tier 1: 10-15 cristais elementais
  - [ ] Tier 2: 25-35 cristais elementais
  - [ ] Tier 3: 50-75 cristais elementais
  
- [ ] **Habilidades por Elemento**
  - [ ] Nature (3 tiers): Crescimento Rápido, Espinhos Defensivos, Jardim Selvagem
  - [ ] Water (3 tiers): Jato d'Água, Escudo Aquático, Tsunami
  - [ ] Fire (3 tiers): Bola de Fogo, Trilha Flamejante, Explosão Solar
  - [ ] Shadow (3 tiers): Passo Sombrio, Camuflagem, Clone Sombrio
  - [ ] Earth (3 tiers): Pilar de Pedra, Tremor, Fortaleza de Pedra
  - [ ] Air (3 tiers): Rajada de Vento, Levitação, Tornado
  - [ ] Ice (3 tiers): Lança de Gelo, Caminho Gelado, Nevasca
  
- [ ] **UI da Árvore**
  - [ ] Atalho: H/Y/Triangle/X
  - [ ] Tabs para cada elemento
  - [ ] Visualização vertical (Tier 1 → 2 → 3)
  - [ ] Habilidades bloqueadas (cinza + cadeado)
  - [ ] Informações detalhadas (nome, ícone, descrição, custo, stats)
  - [ ] Vídeo preview (GIF animado)
  - [ ] Animação de desbloqueio
  
- [ ] **Equipando Habilidades**
  - [ ] 4 slots (Q/E/R/F ou ZL/L/R/ZR)
  - [ ] Drag and drop para reorganizar
  - [ ] Presets salvos (até 3 loadouts)
  - [ ] Troca de loadout fora de combate (3s cast)
  - [ ] Sistema de sinergias (combos de habilidades)
  
- [ ] **Progressão e Balanceamento**
  - [ ] Economia de cristais (~200 por elemento no final)
  - [ ] Custo total: ~700 cristais (100 por elemento)
  - [ ] Sem respec (decisões permanentes)
  - [ ] Incentiva múltiplos playthroughs

### 3.3 Sistema de Habilidades Elementais (Implementação) 📋

- [ ] Árvore de Habilidades (ScriptableObject-based)
- [ ] 7 árvores elementais (3 tiers cada)
- [ ] 4 slots de habilidades (Q, E, R, F)
- [ ] Sistema de cooldown
- [ ] Sinergias entre habilidades
- [ ] UI de seleção de habilidades

**Habilidades Prioritárias (Tier 1):**

- [ ] Nature: Crescimento Rápido
- [ ] Water: Jato d'Água
- [ ] Fire: Bola de Fogo
- [ ] Shadow: Passo Sombrio

### 3.3 Sistema de Aura Elemental 📋

- [ ] Visual de aura (shader/particle system)
- [ ] Progressão de aura (1ª, 3ª, 5ª, 10ª)
- [ ] Cores por elemento (tabela do GDD)
- [ ] Padrões visuais (hexágonos, ondas, cristais)
- [ ] Som elemental ao mover
- [ ] Coroa flutuante (10 reconhecimentos)

### 3.4 Cristais de Pacto 📋

- [ ] 10 Cristais de Pacto únicos
- [ ] Câmara dos Pactos (lar)
- [ ] Sistema de buffs por cristal
- [ ] Visual único por cristal
- [ ] Coleção persistente

---

## 🤖 Fase 4: Sistema de IA

### 4.1 IA de NPCs 🚧 EM PROGRESSO

**Sistema Base Implementado:**

- [x] NPCController (controller base) ✅
- [x] NPCBehavior (comportamento base) ✅
- [x] NPCData (ScriptableObject com dados) ✅
- [x] NPCConfigData (configuração) ✅
- [x] NPCEnums (tipos e estados) ✅

**IA Básica Implementada:**

- [x] NPCStaticAI (IA estática - sem movimento) ✅
- [x] NPCWanderAI (IA de vagueio aleatório) ✅
- [x] NPCPatrolAI (IA de patrulha em pontos) ✅

**Estados de IA Pendentes:**

- [ ] Alert (alerta)
- [ ] Investigate (investigar)
- [ ] Chase (perseguir)
- [ ] Attack (atacar)
- [ ] Search (procurar)
- [ ] Return (retornar)
- [ ] Flee (fugir)
- [ ] Stunned (atordoado)

### 4.2 Sistema de Percepção 📋

- [ ] Visão (Line of Sight, cone 90-120°)
- [ ] Audição (raio 5-8 unidades)
- [ ] Proximidade (trigger 2-3 unidades)
- [ ] Detecção de stealth (cobertura)
- [ ] Sentidos aguçados (alguns inimigos)

### 4.3 Sistema de Interação com NPCs 🚧 EM PROGRESSO

**Implementado:**

- [x] NPCDialogue (sistema de diálogo) ✅
- [x] NPCDialogueInteraction (interação de diálogo) ✅
- [x] NPCFriendship (sistema de amizade) ✅
- [x] DialogueData (dados de diálogo) ✅
- [x] LocalizedDialogueData (diálogos localizados) ✅
- [x] FriendshipData (dados de amizade) ✅
- [x] QuestGiverController (NPCs que dão quests) ✅

**Pendente:**

- [ ] Sistema de amizade completo (5 níveis com progressão)
- [ ] Comportamento diário (schedule)
- [ ] Reações dinâmicas ao jogador
- [ ] Memória de interações
- [ ] Diálogos condicionais baseados em progresso

---

## 🌍 Fase 5: Mundo e Biomas

### 5.1 Ninho do Slime (Tutorial)

> **Nota:** Biomas serão desenvolvidos em fases iterativas (Alpha → Beta → Release)

#### **ALPHA (Versão Atual)** ✅

- [x] Caverna inicial (cena 1_InitialCave)
- [x] InitialCaveScreenController
- [x] Tutorial de movimento (WASD/Analógico)
- [x] Tutorial de encolher/deslizar (SpecialMovementPoint)
- [x] Tutorial de destruir/atacar objetos (BushDestruct, RockDestruct)
- [x] Sistema de drop de itens (DropController)
- [x] Sistema de atração/absorção de itens (ItemCollectable)
- [ ] **Puzzle simples de introdução** 📋
  - [ ] Puzzle de sequência de cristais
  - [ ] Recompensa: Primeiro cristal elemental

#### **BETA (Expansão Planejada)** 📋

- [ ] Tutorial de agachar/stealth
- [ ] Tutorial de habilidades elementais
- [ ] NPCs tutoriais (Slime Ancião)
- [ ] Área secreta com recompensa
- [ ] Conexão visual com outros biomas
- [ ] Lore ambiental (cristais antigos)

#### **RELEASE (Versão Final)** 📋

- [ ] Cutscene de despertar (intro cinemática)
- [ ] Diálogos com Slime Ancião
- [ ] Quest tutorial completa
- [ ] Easter eggs e segredos
- [ ] Polimento visual e sonoro
- [ ] Música tema da caverna

---

### 5.2 Floresta Calma (Nature/Earth/Air)

#### **ALPHA (Recorte Inicial)** 📋

**Objetivo:** Primeiro bioma explorável fora da caverna

**Área Implementada:**

- [ ] **Clareira de Entrada** (conexão com caverna)
  - [ ] Transição visual caverna → floresta
  - [ ] Teleport point funcional
  - [ ] Árvores e vegetação básica
  - [ ] Iluminação natural (dia)
  
- [ ] **Caminho dos Cervos** (área linear)
  - [ ] 3-5 Cervos-Broto (NPCs passivos)
  - [ ] Flores cristalinas coletáveis
  - [ ] Arbustos destrutíveis
  - [ ] 1 puzzle simples de crescimento de plantas
  
- [ ] **Colmeia Pequena** (área de desafio)
  - [ ] 2-3 Abelhas Cristalinas (NPCs neutros)
  - [ ] Plataformas de mel
  - [ ] Cristais verdes (Nature) coletáveis
  - [ ] Puzzle geométrico básico

**NPCs Alpha:**

- [ ] Cervo-Broto (3 unidades, comportamento passivo)
- [ ] Abelha Cristalina (3 unidades, patrulha simples)
- [ ] Esquilo Coletor (1 unidade, quest giver básico)

**Mecânicas Alpha:**

- [ ] Sistema de crescimento de plantas (básico)
- [ ] Plataformas de mel (sticky surfaces)
- [ ] Coleta de flores cristalinas
- [ ] Primeiro puzzle de Nature element

**Conteúdo Alpha:**

- [ ] 1 quest simples (Esquilo Coletor)
- [ ] 1 puzzle ambiental
- [ ] 5-10 minutos de exploração
- [ ] Conexão de volta para caverna

#### **BETA (Expansão Média)** 📋

**Área Adicional:**

- [ ] Bosque Profundo (área de exploração)
- [ ] Colmeia Suspensa (área vertical)
- [ ] Lago Pequeno (transição para Lago Espelhado)
- [ ] Clareira da Rainha (domínio da Rainha Melífera)

**NPCs Beta:**

- [ ] +5 Cervos-Broto (comportamentos variados)
- [ ] +10 Abelhas Cristalinas (colmeia ativa)
- [ ] +3 Esquilos Coletores (quests)
- [ ] Rainha Melífera (primeiro Rei Monstro)

**Mecânicas Beta:**

- [ ] Sistema de amizade com Cervos
- [ ] Colmeia interativa (estrutura 3D)
- [ ] Crescimento de plantas avançado
- [ ] Puzzle geométrico completo (Jardim Geométrico)

**Conteúdo Beta:**

- [ ] 5 quests de NPCs
- [ ] 3 puzzles ambientais
- [ ] Desafio da Rainha Melífera
- [ ] Ritual de Reconhecimento
- [ ] 30-45 minutos de exploração

#### **RELEASE (Versão Final)** 📋

**Área Completa:**

- [ ] Jardim Secreto (área escondida)
- [ ] Árvore Ancestral (landmark)
- [ ] Caverna de Cristais (dungeon pequeno)
- [ ] Observatório Noturno (Imperatriz Nictófila)

**NPCs Release:**

- [ ] População completa (20+ criaturas)
- [ ] Imperatriz Nictófila (segundo Rei Monstro)
- [ ] NPCs únicos com histórias
- [ ] Criaturas noturnas (ciclo dia/noite)

**Mecânicas Release:**

- [ ] Ciclo dia/noite completo
- [ ] Sistema sazonal (primavera/verão)
- [ ] Ecossistema dinâmico
- [ ] Seguidores recrutáveis
- [ ] Expansão do lar (Jardim de Cristais)

**Conteúdo Release:**

- [ ] 15+ quests
- [ ] 8+ puzzles
- [ ] 2 Reis Monstros
- [ ] Áreas secretas e easter eggs
- [ ] 2-3 horas de exploração completa

---

### 5.3 Biomas Futuros (Pós-Alpha)

#### **Lago Espelhado** (Water/Air) 🔮

**Alpha:** Não incluído  
**Beta:** Área inicial (30% do bioma)  
**Release:** Bioma completo com Imperador Escavarrok

#### **Área Rochosa** (Earth/Fire) 🔮

**Alpha:** Não incluído  
**Beta:** Área inicial (30% do bioma)  
**Release:** Bioma completo com Conde Castoro

#### **Pântano das Névoas** (Shadow/Water/Nature) 🔮

**Alpha:** Não incluído  
**Beta:** Não incluído  
**Release:** Bioma completo com Rainha Formicida

#### **Câmaras de Lava** (Fire/Earth) 🔮

**Alpha:** Não incluído  
**Beta:** Não incluído  
**Release:** Bioma completo com Sultan Escamífero

#### **Pico Nevado** (Air/Water/Ice) 🔮

**Alpha:** Não incluído  
**Beta:** Não incluído  
**Release:** Bioma completo com Príncipe Fulgorante

---

### 5.4 Sistema de Ambiente 🚧 EM PROGRESSO

**Implementado:**

- [x] WindManager (gerenciamento de vento) ✅
- [x] WindController (controle de vento) ✅
- [x] WindEmulator (efeitos de vento) ✅
- [x] BushShake (arbustos balançando) ✅
- [x] BushDestruct (arbustos destrutíveis) ✅
- [x] RockDestruct (rochas destrutíveis) ✅
- [x] SetupVisualEnvironment (variações visuais) ✅
- [x] RandomStyle (estilos aleatórios) ✅
- [x] PuddleDrop (gotas em poças) ✅
- [x] SelfDestruct (auto-destruição de objetos) ✅

**Pendente:**

- [ ] Sistema Dia/Noite (24 min = 1 dia)
- [ ] Sistema Sazonal (7 dias = 1 estação)
- [ ] Iluminação dinâmica (URP 2D Lights)
- [ ] Bioluminescência
- [ ] Partículas ambientais avançadas
- [ ] Clima dinâmico (chuva, neve, névoa)

---

## 🏰 Fase 6: Sistema de Lar

### 6.1 Caverna Principal 📋

- [ ] Save point
- [ ] Sistema de descanso (cura completa)
- [ ] Fast travel hub
- [ ] Decoração personalizável

### 6.2 Expansões do Lar 📋

- [ ] **Jardim de Cristais**
  - [ ] Desbloqueio: Amizade nível 3 com Cervos-Broto
  - [ ] Gera 1 cristal/dia
  
- [ ] **Lago Interno**
  - [ ] Desbloqueio: Amizade nível 4 com Castores
  - [ ] Cura contínua (+5 HP/s)
  
- [ ] **Sótão Panorâmico**
  - [ ] Desbloqueio: Amizade nível 4 com Borboletas
  - [ ] Previsão climática
  
- [ ] **Câmara dos Pactos**
  - [ ] Desbloqueio: Primeiro Ritual de Reconhecimento
  - [ ] 10 pedestais para Cristais de Pacto
  - [ ] Sistema de buffs

### 6.3 Sistema de Construção 📋

- [ ] Coleta de materiais
- [ ] Receitas de construção
- [ ] Animação de construção
- [ ] Progresso visual
- [ ] NPCs visitantes

---

## 👑 Fase 7: Reis Monstros

### 7.1 Sistema de Reconhecimento 📋

- [ ] Sistema de reputação (tracking invisível)
- [ ] Rumores (NPCs falam sobre Reis)
- [ ] Descoberta de domínios
- [ ] Observação do Rei
- [ ] Desafio oferecido
- [ ] Ritual de Reconhecimento (cutscene)
- [ ] Recebimento de Aura + Cristal de Pacto

### 7.2 Os Dez Reis (Progressão Livre) 📋

**Tier 1 - Introdutórios:**

- [ ] **Rainha Melífera** (Nature + Earth + Air)
  - [ ] Domínio: Floresta Calma
  - [ ] Desafio: Construir estrutura geométrica
  - [ ] Puzzle: Jardim Geométrico
  
- [ ] **Conde Castoro** (Earth + Water)
  - [ ] Domínio: Área Rochosa
  - [ ] Desafio: Construir barragem funcional
  - [ ] Puzzle: Engenharia Hidráulica

**Tier 2 - Intermediários:**

- [ ] **Imperador Escavarrok** (Earth + Shadow)
  - [ ] Domínio: Profundezas
  - [ ] Desafio: Navegar túneis escuros
  - [ ] Puzzle: Câmara do Eco
  
- [ ] **Imperatriz Nictófila** (Ice + Air + Shadow)
  - [ ] Domínio: Floresta Calma (noite)
  - [ ] Desafio: Seguir padrão das estrelas
  - [ ] Puzzle: Constelação Perdida
  
- [ ] **Matriarca Flores** (Nature Growth)
  - [ ] Domínio: Jardim Secreto
  - [ ] Desafio: Curar jardim doente
  - [ ] Puzzle: Ecologia e cura

**Tier 3 - Avançados:**

- [ ] **Sultan Escamífero** (Fire + Air)
  - [ ] Domínio: Câmaras de Lava
  - [ ] Desafio: Corrida flamejante
  - [ ] Puzzle: Corrida Flamejante
  
- [ ] **Rainha Formicida** (Shadow + Earth + Nature)
  - [ ] Domínio: Pântano das Névoas
  - [ ] Desafio: Restaurar equilíbrio
  - [ ] Puzzle: Equilíbrio do Pântano
  
- [ ] **Príncipe Fulgorante** (Air + Fire - Eletricidade)
  - [ ] Domínio: Pico Nevado (tempestades)
  - [ ] Desafio: Corrida contra relâmpagos
  - [ ] Puzzle: Timing elétrico

**Tier 4 - Desafiadores:**

- [ ] **Duquesa Solibrida** (Dark + Shadow)
  - [ ] Domínio: Cavernas Sombrias
  - [ ] Desafio: Puzzle de ilusões
  - [ ] Puzzle: Infiltração Silenciosa
  
- [ ] **Grão-Sacerdote Luminescente** (All Elements)
  - [ ] Domínio: Templo Cristalino
  - [ ] Desafio: Harmonizar todos elementos
  - [ ] Puzzle: Harmonia Elemental Final

---

## 🧩 Fase 8: Sistema de Puzzles

### 8.1 Mecânicas de Puzzle 📋

- [ ] Sistema de interação com objetos
- [ ] Interruptores e botões
- [ ] Alavancas e botões
- [ ] Cristais ativáveis
- [ ] Espelhos e reflexos
- [ ] Plataformas móveis
- [ ] Portais

### 8.2 Puzzles Implementados 📋

- [ ] Jardim Geométrico (Rainha Melífera)
- [ ] Câmara do Eco (Imperador Escavarrok)
- [ ] Constelação Perdida (Imperatriz Nictófila)
- [ ] Corrida Flamejante (Sultan Escamífero)
- [ ] Equilíbrio do Pântano (Rainha Formicida)
- [ ] Reflexos Espelhados (Lago Espelhado)
- [ ] Engenharia Hidráulica (Área Rochosa)
- [ ] Infiltração Silenciosa (Pântano das Névoas)

### 8.3 Sistema de Hints 📋

- [ ] Pistas visuais sutis
- [ ] NPCs dão dicas
- [ ] Sistema de hint progressivo
- [ ] Sem penalidade por usar hints

---

## 📜 Fase 9: Sistema de Quests

### 9.1 Quest System Core ✅ COMPLETO

**Sistema Base:**

- [x] **QuestManager** (singleton, gerenciamento centralizado) ✅
- [x] **QuestEvents** (sistema de eventos desacoplado) ✅
- [x] **SaveEvents** (eventos de save/load) ✅
- [x] **QuestGiverController** (NPCs que oferecem quests) ✅
- [x] **QuestNotificationController** (notificações na tela) ✅
- [x] **QuestProgress** (tracking de progresso) ✅
- [x] **QuestSaveData** (persistência de dados) ✅

**Tipos de Quest:**

- [x] **CollectQuestData** (ScriptableObject para quests de coleta) ✅
- [ ] DefeatQuestData (derrotar inimigos)
- [ ] DeliverQuestData (entregar itens)
- [ ] ExploreQuestData (explorar áreas)
- [ ] InteractQuestData (interagir com objetos)
- [ ] EscortQuestData (escoltar NPCs)

**Sistema de Recompensas:**

- [x] **ItemReward** (recompensas de itens) ✅
- [x] **ItemRewardDrawer** (editor customizado) ✅

### 9.2 Quest UI 🚧 EM PROGRESSO

**Implementado:**

- [x] QuestChoiceUI (escolhas de quest) ✅
- [x] QuestNotificationController (notificações) ✅
- [x] Notificações de progresso (eventos) ✅
- [x] Sistema de recompensas (itens + reputação) ✅

**Pendente:**

- [ ] Quest log UI (lista completa de quests)
- [ ] Quest tracker HUD (progresso na tela)
- [ ] Marcadores no mapa
- [ ] Quest details panel (detalhes da quest)

### 9.3 Quests Principais 📋

- [ ] 5 quests para evolução Adulto
- [ ] 15 quests para evolução Grande Slime
- [ ] 30+ quests para evolução Rei Slime
- [ ] Quests de amizade (por espécie)
- [ ] Quests de Reis Monstros
- [ ] Side quests opcionais

### 9.4 Ferramentas de Editor ✅ COMPLETO

- [x] **QuestSystemTestSceneSetup** (cena de teste automática) ✅
- [x] **QuestManagerEditor** (inspetor customizado) ✅
- [x] **ItemRewardDrawer** (drawer para recompensas) ✅

---

## 🎬 Fase 10: Sistema de Cutscenes

### 10.1 Cutscene System 📋

- [ ] Timeline-based cutscenes
- [ ] 4 tipos de cutscenes:
  - [ ] Dialogue (10-30s)
  - [ ] Cinematic (15-60s)
  - [ ] Ritual (25-30s)
  - [ ] Discovery (3-5s)

### 10.2 Cutscenes Principais 📋

- [ ] Despertar do Slime (intro)
- [ ] Primeiro encontro com NPC
- [ ] Descoberta de cada bioma
- [ ] 10 Rituais de Reconhecimento
- [ ] Cerimônia de Coroação (5 reconhecimentos)
- [ ] Grande Cerimônia (10 reconhecimentos)
- [ ] Ending

### 10.3 Sistema de Skip 📋

- [ ] Todas cutscenes puláveis
- [ ] Delay de 3-5s antes de permitir skip
- [ ] Indicador visual de skip
- [ ] Salvar estado de cutscenes vistas

---

## 🐾 Fase 11: Sistema de Seguidores

### 11.1 Follower System 📋

- [ ] Sistema de recrutamento
- [ ] Capacidade por evolução (1/3/5/10)
- [ ] IA de seguidor (follow, attack, wait)
- [ ] Sistema de comandos
- [ ] HP e XP de seguidores
- [ ] Retorno ao lar se derrotado

### 11.2 Tipos de Seguidores 📋

- [ ] **Combatentes:**
  - [ ] Esquilo Coletor
  - [ ] Golem de Pedra
  - [ ] Lobo Cristalino
  
- [ ] **Suporte:**
  - [ ] Borboleta Mineral (cura)
  - [ ] Rã-Eco (buff)
  - [ ] Fada Cristal (regeneração)
  
- [ ] **Utilitários:**
  - [ ] Coruja-Cristal (iluminação)
  - [ ] Lontra Cristalina (natação)
  - [ ] Raposa-Vento (velocidade)

---

## 🎨 Fase 12: Arte e Áudio

### 12.1 Arte 🚧

- [x] Sprite do slime branco (16x16)
- [x] VFX básicos (absorve, attack, hit, notHit)
- [x] Cristais básicos
- [x] Props básicos (caverna, escadaria)
- [x] Arbustos e rochas destrutíveis
- [ ] Sprites de evolução (24x24, 32x32, 40x40, 56x56)
- [ ] Sprites de aura elemental
- [ ] Sprites de habilidades elementais
- [ ] Tilesets de 7 biomas
- [ ] Sprites de NPCs (30+ espécies)
- [ ] Sprites de Reis Monstros (10)
- [ ] UI completa
- [ ] Partículas e VFX avançados

### 12.2 Animações 📋

- [ ] Slime: idle, walk, attack, crouch, hurt, death
- [ ] Evolução: transformação visual
- [ ] Habilidades: 21+ animações (7 elementos x 3 tiers)
- [ ] NPCs: comportamentos únicos
- [ ] Reis Monstros: animações majestosas
- [ ] Ambiente: vento, água, fogo, etc.

### 12.3 Áudio 📋

- [ ] Música adaptativa (camadas por contexto)
- [ ] Trilha por bioma (7)
- [ ] Trilha de combate
- [ ] Trilha de Reis Monstros (10)
- [ ] SFX de movimento
- [ ] SFX de habilidades
- [ ] SFX de ambiente
- [ ] SFX de UI
- [ ] Vozes de NPCs (opcional)

---

## 🖥️ Fase 13: UI/UX

### 13.1 HUD �

- [x] QuickSlotUI (4 quick slots no HUD) ⭐ IMPLEMENTADO
- [x] QuickSlotManager (gerenciamento de quick slots) ⭐ IMPLEMENTADO
- [ ] HP Bar
- [ ] Contador de Cristais Elementais
- [ ] Habilidades equipadas (com cooldowns)
- [ ] Quest Tracker
- [ ] Minimapa
- [ ] Indicador de stealth

### 13.2 Menus 🚧

- [x] TitleScreenController (tela inicial) ⭐ IMPLEMENTADO
- [x] PauseMenu (menu de pausa completo) ⭐ IMPLEMENTADO
- [x] InventoryUI (inventário completo) ⭐ IMPLEMENTADO
- [x] ConfirmationDialog (diálogos de confirmação) ⭐ IMPLEMENTADO
- [ ] Menu principal (expandido)
- [ ] Árvore de Habilidades
- [ ] Quest Log
- [ ] Mapa
- [ ] Coleção (Cristais de Pacto)
- [ ] Configurações
- [ ] Créditos

### 13.3 Feedback Visual 🚧 EM PROGRESSO

**Implementado:**

- [x] InteractionIcon (ícone de interação sobre NPCs) ✅
- [x] DialogueUI (feedback visual de diálogo) ✅
- [x] InventorySlotUI (feedback visual de slots) ✅
- [x] QuestNotificationController (notificações de quest) ✅
- [x] OutlineController (outline de sprites) ✅
- [x] OutlineUtility (utilitário para outline) ✅
- [x] OutlineExample (exemplo de uso) ✅
- [x] VFXOutlineObject (outline com VFX) ✅

**Pendente:**

- [ ] Dano flutuante
- [ ] Indicadores de buff/debuff
- [ ] Marcadores de quest no mundo
- [ ] Notificações de conquista
- [ ] Tutorial tooltips
- [ ] Feedback de coleta de itens aprimorado

---

## 🔧 Fase 14: Sistemas Técnicos

### 14.1 Save System 📋

- [ ] Save/Load de progresso
- [ ] Múltiplos slots de save
- [ ] Auto-save
- [ ] Cloud save (opcional)
- [ ] Dados salvos:
  - [ ] Posição do jogador
  - [ ] Inventário
  - [ ] Habilidades desbloqueadas
  - [ ] Quests completadas
  - [ ] Reis Monstros derrotados
  - [ ] Amizades
  - [ ] Expansões do lar
  - [ ] Cristais coletados

### 14.2 Settings System 📋

- [ ] Controles customizáveis
- [ ] Volume (master, music, sfx)
- [ ] Resolução e fullscreen
- [ ] V-Sync
- [ ] Qualidade gráfica
- [ ] Idioma
- [ ] Acessibilidade

### 14.3 Performance 📋

- [ ] Object Pooling (projéteis, partículas)
- [ ] Sprite Atlas
- [ ] Occlusion Culling
- [ ] LOD para auras
- [ ] IA otimizada (update a cada 0.1-0.2s)
- [ ] Target: 60 FPS (PC), 30 FPS (Switch portátil)

---

## 📊 Fase 15: Analytics e Balanceamento

### 15.1 Métricas 📋

- [ ] Tempo para cada evolução
- [ ] Sequência de Reis Monstros
- [ ] Taxa de conclusão de quests
- [ ] Puzzles resolvidos vs abandonados
- [ ] Taxa de uso de stealth vs combate
- [ ] Habilidades mais usadas
- [ ] Mortes por bioma
- [ ] Tempo de jogo total

### 15.2 Balanceamento 📋

- [ ] Curva de dificuldade
- [ ] Economia de cristais
- [ ] Dano de habilidades
- [ ] HP de inimigos
- [ ] Cooldowns
- [ ] Recompensas de quests
- [ ] Drop rates

---

## 🧪 Fase 16: Testes e Polimento

### 16.1 Testes 📋

- [ ] Playtest interno
- [ ] Closed beta
- [ ] Open beta
- [ ] Bug fixing
- [ ] Performance profiling
- [ ] Balanceamento baseado em feedback

### 16.2 Polimento 📋

- [ ] Juice (screen shake, particles, sounds)
- [ ] Transições suaves
- [ ] Feedback tátil (controller rumble)
- [ ] Animações de UI
- [ ] Loading screens
- [ ] Easter eggs
- [ ] Achievements

---

## 🚀 Fase 17: Lançamento

### 17.1 Preparação 📋

- [ ] Trailer
- [ ] Screenshots
- [ ] Descrição da loja
- [ ] Página Steam/Switch
- [ ] Press kit
- [ ] Marketing materials

### 17.2 Plataformas 🔮

- [ ] PC (Steam)
- [ ] Nintendo Switch
- [ ] Outras plataformas (futuro)

---

## 🎯 Prioridades Imediatas (Next Steps - Dezembro 2025)

> **📋 CHECKLIST COMPLETO:** Para detalhes completos do que falta para ALPHA 1, veja [ALPHA-1-Checklist.md](ALPHA-1-Checklist.md)

### ✅ Concluído Recentemente (Novembro 2025)

- ✅ Sistema de Inventário completo
- ✅ Sistema de Diálogo completo
- ✅ Sistema de Localização (PT-BR + EN)
- ✅ Sistema de Quest (core completo)
- ✅ Sistema de NPC (base + 3 tipos de IA)
- ✅ Sistema de Amizade (estrutura básica)
- ✅ Pause Menu e UI foundations
- ✅ Ferramentas de Editor expandidas (12+ ferramentas)
- ✅ Sistema de Outline visual
- ✅ Sistema de Save/Load (estrutura de dados)

### 🎯 O que Falta para ALPHA 1 (30% → 55%)

**Sistemas Pendentes:**

- 📋 Quest UI completa (log + tracker HUD) - 2 semanas
- 📋 HUD completo (HP, Cristais) - 1 semana
- 📋 Ninho do Slime - Puzzle final - 1 semana
- 📋 Floresta Calma (3 áreas + NPCs) - 6 semanas
- 📋 Mecânica de Agachar (stealth) - 2 semanas
- 📋 Sistema de Habilidades (2 habilidades) - 3 semanas
- 📋 Polimento e balanceamento - 2 semanas

**Total:** 16 semanas (4 meses) até 23/02/2026

### 🔥 Sprint 1: Quest UI + HUD (2 semanas) - 04/11 a 17/11

**Quest UI:**

- [ ] Quest log UI (lista de quests ativas)
- [ ] Quest tracker HUD (progresso na tela)
- [ ] Integração com coleta de itens
- [ ] Testar fluxo completo

**HUD Básico:**

- [ ] HP Bar (visual + animações)
- [ ] Contador de Cristais Elementais
- [ ] Polimento e feedback visual

### Sprint 2: Ninho do Slime - Puzzle (1 semana) - 18/11 a 24/11

- [ ] Polimento visual/sonoro da caverna
- [ ] Transição para Floresta

### Sprint 3-5: Floresta Calma (6 semanas) - 25/11 a 05/01

**Sprint 3:** Clareira de Entrada (2 sem)
**Sprint 4:** Caminho dos Cervos + Quest (2 sem)
**Sprint 5:** Colmeia Pequena (2 sem)

**Entregas:**

- 3 áreas jogáveis
- 7 NPCs (3 Cervos, 3 Abelhas, 1 Esquilo)
- 1 quest funcional
- 2 puzzles

### Sprint 6: Mecânica de Agachar (2 semanas) - 06/01 a 19/01

- [ ] Input e animação
- [ ] Sistema de stealth básico
- [ ] Integração com IA
- [ ] Puzzle de stealth

### Sprint 7: Sistema de Habilidades (3 semanas) - 20/01 a 09/02

- [ ] Infraestrutura (cooldown)
- [ ] 2 habilidades Tier 1 (Nature + Fire)
- [ ] VFX e SFX
- [ ] UI de habilidades

### Sprint 8: Polimento Alpha (2 semanas) - 10/02 a 23/02

- [ ] Balanceamento completo
- [ ] Juice (shake, particles, sounds)
- [ ] Bug fixing
- [ ] Build standalone

**📋 Detalhes completos:** [ALPHA-1-Checklist.md](ALPHA-1-Checklist.md)

---

## 📈 Estimativas de Tempo

### Desenvolvimento Core (Fases 1-4)

- **Estimativa:** 3-4 meses
- **Status:** ~70% completo ⬆️ (+10%)

### Conteúdo Principal (Fases 5-11)

- **Estimativa:** 8-12 meses
- **Status:** ~5% completo

### Arte e Áudio (Fase 12)

- **Estimativa:** 4-6 meses (paralelo)
- **Status:** ~10% completo

### UI/UX e Sistemas (Fases 13-14)

- **Estimativa:** 2-3 meses
- **Status:** ~35% completo ⬆️ (+20%)

### Testes e Polimento (Fases 15-16)

- **Estimativa:** 2-3 meses
- **Status:** 0% completo

### **Total Estimado:** 18-24 meses

### **Progresso Atual:** ~27% ⬆️ (+2% desde última atualização)

### **Sistemas Recentemente Implementados (Nov 2025):**

- ✅ Sistema de Inventário completo (20 slots + equipamentos + quick slots)
- ✅ Sistema de Diálogo completo (manager + UI + localização)
- ✅ Sistema de Quest completo (core) - QuestManager + Events + UI + Save ⭐ ATUALIZADO
- ✅ Sistema de NPC expandido (Controller + 3 tipos de IA + Friendship) ⭐ NOVO
- ✅ Sistema de Localização (PT-BR + EN)
- ✅ Sistema de Outline visual (sprites interativos) ⭐ NOVO
- ✅ Pause Menu funcional
- ✅ Ferramentas de Editor robustas (12+ ferramentas) ⭐ EXPANDIDO
- ✅ UI/UX foundations (ConfirmationDialog, InteractionIcon, ItemActionPanel, QuestNotification)
- ✅ Sistema de Save/Load (estrutura de dados para Inventory + Quest) ⭐ NOVO
- ✅ Sistema de Ambiente (vento, destruição, efeitos visuais) ⭐ NOVO

### **Métricas de Progresso Detalhadas:**

| Categoria | Progresso | Detalhes |
|-----------|-----------|----------|
| **Arquitetura Core** | 95% | Managers, Singletons, Scene Management, Events |
| **Sistema de Inventário** | 85% | Funcional, falta drag-and-drop e filtros |
| **Sistema de Diálogo** | 85% | Funcional, falta expressões e memória |
| **UI/UX** | 40% | Foundations prontas, falta HUD completo e menus |
| **Gameplay Core** | 45% | Movimento, combate, interação básicos |
| **Biomas** | 10% | Apenas Ninho do Slime (tutorial) |
| **NPCs/IA** | 30% | ⬆️ Sistema base + 3 tipos de IA implementados |
| **Habilidades** | 0% | Não iniciado |
| **Quests** | 60% | ⬆️ Core completo, falta UI completa e tipos adicionais |
| **Save/Load** | 35% | ⬆️ Estrutura de dados pronta (Inventory + Quest) |
| **Sistema de Ambiente** | 50% | ⬆️ Vento, destruição, efeitos visuais básicos |
| **Ferramentas de Editor** | 80% | ⬆️ Conjunto robusto de ferramentas implementado |

### **Próximos Marcos:**

- 🎯 **35% (Dez 2025):** Quest UI + HUD + Ninho completo + Floresta iniciada
- 🎯 **45% (Jan 2026):** Floresta Calma completa + Mecânica de Agachar
- 🎯 **55% (Fev 2026):** Sistema de Habilidades + Polimento + **ALPHA 1 RELEASE** 🚀

**📋 Cronograma detalhado:** [ALPHA-1-Checklist.md](ALPHA-1-Checklist.md)

---

## 🎓 Notas de Desenvolvimento

### Decisões de Design

- Progressão livre (qualquer ordem de Reis Monstros)
- Combate opcional (stealth/diplomacia viáveis)
- Sem timers ou pressão de tempo
- Atmosfera cozy e contemplativa
- Narrativa emergente (não linear)

### Decisões Técnicas Implementadas

- **Arquitetura de Managers:** Padrão ManagerSingleton<T> para consistência
- **Sistema de Inventário:** Slots fixos (20) para simplicidade e performance
- **Sistema de Diálogo:** JSON-based para fácil localização e edição
- **UI Modular:** Componentes reutilizáveis (ConfirmationDialog, ItemActionPanel)
- **Validação Automática:** SceneSetupValidator previne erros de configuração
- **Ferramentas de Editor:** NPCDialogueQuickConfig acelera criação de conteúdo

### Desafios Técnicos

- Sistema de aura visual escalável (10 níveis)
- IA robusta com 10 estados
- Stealth com detecção de cobertura
- Puzzles criativos e integrados à lore
- Performance em Switch (30 FPS estável)
- Integração de múltiplos sistemas (Inventário + Diálogo + Quests)

### Lições Aprendidas (Nov 2025)

- ✅ **Singleton Pattern:** ManagerSingleton<T> evita duplicação de código
- ✅ **ScriptableObjects:** Excelente para dados de itens, NPCs, quests e configurações
- ✅ **Modularidade:** UI modular facilita manutenção e expansão
- ✅ **Sistema de Eventos:** QuestEvents e SaveEvents permitem comunicação desacoplada
- ✅ **Validação:** SceneSetupValidator economiza tempo de debug
- ✅ **Ferramentas de Editor:** Aceleram criação de conteúdo significativamente (12+ ferramentas)
- ✅ **Organização de Código:** Estrutura clara (Code/Data/Editor/Gameplay/Systems/Visual)
- ✅ **IA Modular:** NPCs com diferentes tipos de IA (Static, Wander, Patrol) facilita expansão
- ⚠️ **Integração:** Sistemas complexos requerem planejamento cuidadoso
- ⚠️ **Performance:** Testar em hardware alvo (Switch) desde cedo
- ⚠️ **Documentação:** Manter TechMapping atualizado é essencial

### Oportunidades de Expansão

- DLC com novos biomas
- Novos Reis Monstros
- Modo New Game+
- Desafios diários
- Multiplayer cooperativo (futuro distante)
- Sistema de mods (Steam Workshop)

---

## 📦 Inventário de Sistemas Implementados (Análise de Código - Nov 2025)

### Managers (Assets/Code/Systems/Managers)

- ✅ ManagerSingleton<T> - Base para todos os managers
- ✅ GameManager - Ciclo de vida do jogo
- ✅ CameraManager - Gerenciamento de câmeras
- ✅ SceneTransitionManager - Transições de cena
- ✅ DialogueManager - Sistema de diálogos
- ✅ DialogueChoiceHandler - Escolhas em diálogos
- ✅ LocalizationManager - Localização (PT-BR/EN)

### Sistema de Inventário (Assets/Code/Systems/Inventory)

- ✅ InventoryManager - Gerenciamento de inventário
- ✅ InventorySlot - Dados de slot
- ✅ InventorySaveData - Persistência
- ✅ ItemData - ScriptableObject de itens
- ✅ ItemType - Enum de tipos
- ✅ EquipmentType - Enum de equipamentos
- ✅ QuickSlotManager - Quick slots

### Sistema de Quest (Assets/Code/Systems/QuestSystem + Gameplay/Quest)

- ✅ QuestManager - Gerenciamento centralizado
- ✅ QuestEvents - Sistema de eventos
- ✅ SaveEvents - Eventos de save/load
- ✅ QuestSaveData - Persistência
- ✅ QuestGiverController - NPCs que dão quests
- ✅ QuestNotificationController - Notificações
- ✅ QuestProgress - Tracking de progresso
- ✅ CollectQuestData - Quest de coleta
- ✅ ItemReward - Recompensas

### Sistema de NPC (Assets/Code/Gameplay/NPCs)

- ✅ NPCController - Controller base
- ✅ NPCBehavior - Comportamento base
- ✅ NPCDialogue - Sistema de diálogo
- ✅ NPCDialogueInteraction - Interação
- ✅ NPCFriendship - Sistema de amizade
- ✅ NPCStaticAI - IA estática
- ✅ NPCWanderAI - IA de vagueio
- ✅ NPCPatrolAI - IA de patrulha
- ✅ NPCData - ScriptableObject
- ✅ NPCConfigData - Configuração
- ✅ DialogueData - Dados de diálogo
- ✅ LocalizedDialogueData - Diálogos localizados
- ✅ FriendshipData - Dados de amizade
- ✅ NPCEnums - Enumerações

### Sistema de UI (Assets/Code/Systems/UI)

- ✅ InventoryUI - Interface de inventário
- ✅ InventorySlotUI - Slot visual
- ✅ EquipmentSlotUI - Slot de equipamento
- ✅ QuickSlotUI - Quick slot visual
- ✅ QuickSlotSelectionPanel - Seleção de quick slot
- ✅ ItemActionPanel - Ações de item
- ✅ DialogueUI - Interface de diálogo
- ✅ QuestChoiceUI - Escolhas de quest
- ✅ PauseMenu - Menu de pausa
- ✅ ConfirmationDialog - Diálogo de confirmação
- ✅ InteractionIcon - Ícone de interação

### Sistema de Gameplay (Assets/External/AssetStore/SlimeMec/_Scripts/Gameplay)

- ✅ PlayerController - Controle do jogador
- ✅ PlayerAttributesHandler - Atributos do jogador
- ✅ AttackHandler - Sistema de ataque
- ✅ ItemCollectable - Coleta de itens
- ✅ CollectableItemData - Dados de coletáveis
- ✅ ItemBuffHandler - Buffs de itens
- ✅ DropController - Drop de itens
- ✅ BounceHandler - Física de bounce
- ✅ BushDestruct - Arbustos destrutíveis
- ✅ RockDestruct - Rochas destrutíveis
- ✅ BushShake - Arbustos balançando
- ✅ SpecialMovementPoint - Movimento especial
- ✅ InteractivePointHandler - Pontos interativos
- ✅ ScreenEffectsManager - Efeitos de tela
- ✅ SetupVisualEnvironment - Ambiente visual
- ✅ RandomStyle - Estilos aleatórios
- ✅ WindManager - Gerenciamento de vento
- ✅ WindController - Controle de vento
- ✅ WindEmulator - Efeitos de vento
- ✅ SelfDestruct - Auto-destruição
- ✅ PerformanceSystemsIntegration - Integração de performance

### Sistema de Teleporte (Assets/Code/Gameplay)

- ✅ TeleportManager - Gerenciamento de teleporte
- ✅ TeleportPoint - Pontos de teleporte
- ✅ TeleportTransitionHelper - Transições de teleporte
- ✅ PuddleDrop - Gotas em poças

### Sistema Visual (Assets/Code/Visual + Shaders)

- ✅ OutlineController - Outline de sprites
- ✅ OutlineUtility - Utilitário de outline
- ✅ OutlineExample - Exemplo de uso
- ✅ VFXOutlineObject - Outline com VFX
- ✅ GizmosHelper - Visualização de gizmos
- ✅ PolygonGizmosHelper - Gizmos de polígonos
- ✅ SpriteOutline.shader - Shader de outline
- ✅ SpriteOutlineMaterial - Material de outline

### Controllers (Assets/Code/Systems/Controllers)

- ✅ InitialCaveScreenController - Tela inicial da caverna
- ✅ TitleScreenController - Tela de título
- ✅ SimpleCameraFollow - Câmera simples

### Ferramentas de Editor (Assets/Code/Editor)

- ✅ UnifiedExtraTools - Ferramentas unificadas
- ✅ BushQuickConfig - Config de arbustos
- ✅ ItemQuickConfig - Config de itens
- ✅ NPCDialogueQuickConfig - Config de NPCs
- ✅ CameraSetupTools - Setup de câmera
- ✅ SceneSetupTool - Setup de cena
- ✅ DialogueSystemTestSceneSetup - Teste de diálogo
- ✅ CreateExampleItems - Criar itens de exemplo
- ✅ ProjectSettingsExporter - Exportar settings
- ✅ NPCAnimatorSetup - Setup de animadores (QuickWins)
- ✅ NPCGizmosDrawer - Gizmos de NPC (QuickWins)
- ✅ NPCDataGenerator - Gerador de dados (QuickWins)
- ✅ NPCComponentConfigurator - Config de componentes (QuickWins)
- ✅ NPCBatchConfigurator - Config em lote (QuickWins)
- ✅ QuestManagerEditor - Editor de QuestManager (Assets/Editor/QuestSystem)
- ✅ ItemRewardDrawer - Drawer de recompensas (Assets/Editor/QuestSystem)
- ✅ HUDContextMenu - Setup automático de HUD (Hearts + Crystals) ⭐ NOVO

### Validadores (Assets/Code/Systems/Validators)

- ✅ SceneSetupValidator - Validação de cena

### Configurações (Assets/Code/Systems)

- ✅ DialogueSystemSettings - Settings de diálogo

---

---

## 📚 Documentos Relacionados

- **[ALPHA-1-Checklist.md](ALPHA-1-Checklist.md)** - Checklist completo e detalhado do ALPHA 1
- **[Roadmap-Analysis-Summary.md](Roadmap-Analysis-Summary.md)** - Análise do código atual e descobertas
- **[GDD v10.1](The-Slime-King-GDD-v10.1.md)** - Game Design Document completo
- **[TechMapping.md](../TechMapping.md)** - Mapeamento técnico de sistemas

---

## 🎮 Conquistas Recentes v2.6 (Nov 2025)

### 🌟 Biomas Implementados

**✅ Ninho do Slime (100% Completo)**

- Tutorial completo de mecânicas básicas
- **Puzzle de stealth introdutório**: Sistema de agachar para passar por fendas
- **Puzzle de empurrar pedra**: Mecânica de física para acessar áreas
- Recompensas: cogumelos de cura integrados ao sistema de inventário

**✅ Floresta Calma - Clareira de Entrada (100% Completo)**

- Transição suave da caverna para o ambiente florestal
- Introdução ao bioma da Floresta Calma
- Pontos de coleta básicos para familiarizar jogador com mecânicas

### 🎯 Sistema de Combate Simplificado

**Removidos (conforme design focus):**

- ❌ Dano flutuante (números na tela)
- ❌ Sistema de críticos (10% chance, 1.5x dano)  
- ❌ Abordagens alternativas complexas (stealth/diplomacia como alternativas de combate)

**Mantido:**

- ✅ Combate direto com timing e posicionamento
- ✅ Sistema balanceado de risco/recompensa
- ✅ Resistências elementais

### 📈 Impacto no Desenvolvimento

- **Progresso geral:** 35% → **40%** (+5%)
- **ALPHA 1:** Biomas base completos, foco agora em NPCs e conteúdo
- **GDD atualizado:** v9.0 → v10.0 com filosofia de combate simplificada
- **Timeline acelerada:** 2 biomas fundamentais prontos antes do previsto

---

## 📊 Resumo da Análise de Código v2.5 (Nov 2025)

### 🔍 Principais Descobrimentos

Esta atualização v2.5 baseou-se numa **análise completa e abrangente do código atual** que revelou implementações muito mais avançadas do que o roadmap anterior indicava:

**Sistemas Subestimados no Roadmap Anterior:**

- 🎯 **Sistema de Quest**: Completamente funcional com save/load, UI e integração NPC
- 🎮 **PlayerController**: Mecânica de agachar implementada com 2000+ linhas de código
- 💖 **PlayerAttributes**: Sistema completo de HP, atributos e skill points
- 🤖 **Sistema de NPC**: IA básica funcional com estados Wander, Patrol e Static
- 💬 **Sistema de Diálogo**: Integração completa com quest e localização
- 🎒 **Inventário**: 100% funcional com 20 slots + equipamentos + quick slots

**Progresso Real vs Documentado:**

| Sistema | Progresso Anterior | Progresso Real | Diferença |
|---------|-------------------|----------------|-----------|
| Quest | 70% | 95% | +25% |
| PlayerController | 60% | 90% | +30% |
| PlayerAttributes | 50% | 95% | +45% |
| NPC/IA | 40% | 80% | +40% |
| Diálogo | 85% | 95% | +10% |
| Inventário | 100% | 100% | 0% |

**Impacto na Timeline:**

- **Progresso Geral:** 30% → 35% (+5%)
- **ALPHA 1:** Sistema de milestone aumentado para 55% (era 50%)
- **Economia de Tempo:** ~2-3 semanas devido ao avanço não documentado
- **Próximas Prioridades:** Foco em conteúdo (biomas, NPCs) vs sistemas básicos

### 🚀 Recomendações para ALPHA 1

Com base nos descobrimentos, o foco para ALPHA 1 deve ser:

1. **Content Creation** (60% do esforço) - Biomas, NPCs, quests específicas
2. **System Integration** (25% do esforço) - Conectar sistemas existentes
3. **Polish & Bug Fixing** (15% do esforço) - Refinamento de sistemas existentes

**Sistemas que NÃO precisam de desenvolvimento base:** Quest, PlayerController, Inventário, Diálogo, NPC básico.

---

**Fim do Roadmap v2.6**

---

## 🎯 Roadmap Detalhado para ALPHA 1

> **⚠️ NOTA:** Esta seção contém um resumo. Para o checklist completo e detalhado, veja [ALPHA-1-Checklist.md](ALPHA-1-Checklist.md)

### 🔥 Sprint 1: Finalizar Ninho do Slime (2 semanas)

**Objetivo:** Completar o tutorial com puzzle introdutório

1. **Puzzle de Introdução**
   - ✅ Criar puzzle de empurrar pedra
   - ✅ Adicionar objetos empurráveis (pedras)
   - ✅ Tutorial visual (sem texto)

2. **Polimento da Caverna**
   - ✅ Ajustar iluminação (URP 2D Lights)
   - ✅ Adicionar partículas ambientais (poeira, cristais)
   - [ ] SFX de ambiente (goteiras, ecos)
   - ✅ Transição suave para Floresta

---

### 🌿 Sprint 2-4: Floresta Calma - Recorte Alpha (6 semanas)

#### Sprint 2: Clareira de Entrada (2 semanas)

1. **Level Design**

- ✅ Criar tileset de floresta (árvores, grama, flores)
- ✅ Layout da Clareira de Entrada
- ✅ Teleport point caverna ↔ floresta
- ✅ Iluminação natural (dia)

2. **Vegetação Interativa**
   - ✅ Arbustos destrutíveis (reutilizar sistema)
   - ✅ Flores cristalinas coletáveis
   - ✅ Árvores com animação de vento

#### Sprint 3: Caminho dos Cervos + NPCs (2 semanas)

1. **NPCs Básicos**
   - ✅ Sprite de Cervo-Broto (16x16)
   - ✅ IA passiva (wander behavior)
   - ✅ 3 Cervos-Broto no caminho
   - ✅ Sprite de Esquilo Coletor (16x16)
   - ✅ IA de quest giver básico
   - ✅ 1 Esquilo na árvore

2. **Quest Simples**
   - ✅ "Colete 5 Flores Cristalinas"
   - ✅ Sistema de tracking de quest
   - ✅ UI de quest (simples)
   - ✅ Recompensa: 10 Cristais Verdes

3. **Puzzle de Crescimento**
   - ✅ Mecânica de crescimento de plantas
   - ✅ Puzzle: Fazer ponte de vinhas
   - ✅ Recompensa: Acesso à Colmeia

#### Sprint 4: Colmeia Pequena (2 semanas)

1. **Área da Colmeia**
   - ✅ Estrutura de mel (plataformas)
   - ✅ Sprite de Abelha Cristalina (16x16)
   - ✅ IA de patrulha simples (3 abelhas)
   - ✅ Cristais Verdes coletáveis

2. **Puzzle Geométrico Básico**
   - ✅ 3 pilares hexagonais
   - ✅ Ativar na ordem correta
   - ✅ Pista visual (flores no chão)
   - ✅ Recompensa: 15 Cristais Verdes

3. **Conexão de Volta**
   - ✅ Teleport point floresta → caverna
   - ✅ Atalho desbloqueado

---

### ⚡ Sprint 5: Mecânica de Agachar (2 semanas)

**Objetivo:** Implementar stealth básico

1. **Input e Animação**
   - ✅ Input de agachar (segurar Ctrl/B)
   - ✅ Sprite de slime achatado
   - ✅ Animação de transição (0.3s)
   - ✅ Restrição de movimento (parado)

2. **Sistema de Stealth**
   - ✅ Detecção de cobertura (raycast)
   - ✅ Indicador visual (ícone de olho)
   - [ ] Integração com IA (quebrar perseguição)
   - [ ] SFX de agachar

3. **Teste na Floresta**
   - [ ] Adicionar 1 área com guardas (abelhas)
   - [ ] Puzzle de stealth simples
   - [ ] Tutorial visual de agachar

---

### 🎨 Sprint 6: Sistema de Evolução Básico (2 semanas)

**Objetivo:** Filhote → Adulto

1. **Tracking de Progresso**
   - [ ] Sistema de reputação (invisível)
   - [ ] Condições de evolução (5 quests, 3 puzzles)
   - [ ] UI de progresso (sutil)

2. **Evolução Visual**
   - [ ] Sprite Adulto (24x24)
   - [ ] Animação de transformação
   - [ ] Partículas de evolução
   - [ ] SFX de evolução

3. **Aura Básica**
   - [ ] Shader de brilho monocromático
   - [ ] Cor baseada no primeiro elemento
   - [ ] Som elemental ao mover

---

### 🔮 Sprint 7: Sistema de Habilidades (3 semanas)

**Objetivo:** 4 habilidades Tier 1 funcionais

1. **Infraestrutura**
   - [ ] ScriptableObject de habilidade
   - [ ] Sistema de cooldown
   - [ ] UI de habilidades (4 slots)

2. **Habilidades Tier 1**
   - [ ] Nature: Crescimento Rápido (vinhas)
   - [ ] Water: Jato d'Água (empurra)
   - [ ] Fire: Bola de Fogo (dano)
   - [ ] Shadow: Passo Sombrio (teleporte)

3. **VFX e SFX**
   - [ ] Partículas para cada habilidade
   - [ ] Sons de cast
   - [ ] Feedback visual (screen shake)

4. **Árvore de Habilidades (UI)**
   - [ ] Menu de habilidades
   - [ ] Sistema de desbloquear com cristais
   - [ ] Equipar habilidades nos slots

---

### 🎮 Sprint 8: Polimento Alpha (2 semanas)

**Objetivo:** Preparar vertical slice jogável

1. **Balanceamento**
   - [ ] Ajustar dificuldade dos puzzles
   - [ ] Balancear economia de cristais
   - [ ] Testar flow de 15-20 minutos

2. **Juice e Feedback**
   - [ ] Screen shake em ações importantes
   - [ ] Partículas de impacto
   - [ ] Sons de UI
   - [ ] Transições suaves

3. **Bug Fixing**
   - [ ] Testar todos os sistemas
   - [ ] Corrigir bugs críticos
   - [ ] Performance profiling

4. **Build Alpha**
   - [ ] Build standalone (PC)
   - [ ] Documentação de controles
   - [ ] Preparar para testes internos

---

### 📊 Resumo do Roadmap Alpha

**Total:** 8 sprints (19 semanas / ~4.5 meses)

**Entregas:**

- ✅ Ninho do Slime completo (tutorial + puzzle)
- ✅ Floresta Calma (3 áreas, 7 NPCs, 1 quest, 2 puzzles)
- ✅ Mecânica de Agachar (stealth)
- ✅ Sistema de Evolução (Filhote → Adulto)
- ✅ 4 Habilidades Elementais (Tier 1)
- ✅ Sistema de Cristais Elementais
- ✅ 15-20 minutos de gameplay polido

**Próximo Passo:** Playtest interno e feedback

---

## 📊 Resumo de Entregas ALPHA 1

### Conteúdo

✅ **Ninho do Slime** (100%) - Tutorial + Puzzle  
✅ **Floresta Calma** (3 áreas) - Clareira, Caminho, Colmeia  
✅ **7 NPCs** - 3 Cervos, 3 Abelhas, 1 Esquilo  
✅ **1 Quest** - "Colete 5 Flores Cristalinas"  
✅ **3 Puzzles** - Placas, Vinhas, Hexágonos  

### Sistemas

✅ **Quest System** (100%) - Log + Tracker + Integração  
✅ **HUD Completo** - HP, Cristais, Habilidades  
✅ **Mecânica de Agachar** - Stealth básico  
✅ **Sistema de Habilidades** - 2 habilidades Tier 1  
✅ **Polimento** - Juice, balanceamento, build  

### Métricas

✅ 12-15 minutos de gameplay polido  
✅ 60 FPS estável (PC)  
✅ 0 bugs críticos  
✅ Build standalone funcional  

**📋 Checklist completo:** [ALPHA-1-Checklist.md](ALPHA-1-Checklist.md)

---

**Fim do Roadmap v2.8 - Atualizado com sistema de cristais elementais completo (15 Nov 2025)**

---

## 📝 Changelog v2.8 (15 Nov 2025)

### ✅ Sistema de Cristais Elementais - COMPLETO ⭐⭐

**Arquivos Implementados:**

- **CrystalType.cs**: Enum com 6 tipos elementais (Nature, Fire, Water, Shadow, Earth, Air)
- **CrystalElementalData.cs**: ScriptableObject de configuração com cores e sprites
- **CrystalCounterUI.cs**: Interface em tempo real com cores temáticas por tipo
- **GameManager.cs**: Sistema integrado de contadores por tipo elemental
- **ItemCollectable.cs**: Correções críticas para atração magnética e coleta

**Funcionalidades Implementadas:**

- ✅ 6 tipos de cristais elementais configuráveis
- ✅ Atração magnética inteligente (2.5f unidades, 4.0f velocidade)
- ✅ Coleta automática com detecção de proximidade (0.5f unidades)
- ✅ Auto-criação de GameManager quando necessário
- ✅ Sistema de timeout para evitar cristais "órfãos"
- ✅ Interface colorizada por tipo elemental
- ✅ Integração completa com eventos do GameManager

**Correções Críticas:**

- 🔧 ItemCollectable agora funciona com apenas `crystalData` configurado
- 🔧 Não depende mais de `itemData` para atração magnética
- 🔧 GameManager é criado automaticamente se não existir na cena
- 🔧 Sistema de logs detalhado para debugging

**Ferramentas de Editor:**

- ✅ **HUDContextMenu**: Menu de contexto para Canvas objects
  - Setup automático de Heart HUD
  - Setup automático de Crystal Counters
  - Configuração completa de HUD em uma única operação
  - Posicionamento inteligente (top-left hearts, top-right crystals)

**Documentação Criada:**

- `HUD_ContextMenu_Guide.md` - Guia completo de uso
- `Crystal_Configuration_Guide.md` - Configuração detalhada de cristais
- `Crystal_Troubleshooting.md` - Solução de problemas
- `GameManager_AutoCreation_Solution.md` - Sistema de auto-criação

### 📊 Impacto no Cronograma Alpha 1

- **Progresso**: +5% (60% → 65%)
- **Systems**: +10% (85% → 95%)
- **Estimativa**: -1 semana (5 → 4 semanas restantes)
- **Risco**: Mantém-se MUITO BAIXO (sistemas críticos completos)

---

## 📝 Changelog v2.6.1 (14 Nov 2025)

### ✅ Sistema de Stealth Avançado - IMPLEMENTADO

- **PlayerController**: Expandido com sistema multi-SpriteRenderer
- **StealthEvents**: Sistema de comunicação desacoplada
- **GameManager**: Integration para consulta por sistemas de IA
- **Recursos**: Fade visual configurável, detecção de cobertura, debug completo
- **Status**: Sistema completo e funcional, pronto para integração com IA de inimigos

### 📊 Impacto no Cronograma Alpha 1

- **Progresso**: +5% (40% → 45%)
- **Systems**: +10% (60% → 70%)  
- **Estimativa**: -1 semana (8 → 7 semanas restantes)
- **Risco**: Mantém-se BAIXO (sistemas críticos implementados)
