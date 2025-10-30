# The Slime King - Roadmap de Desenvolvimento

**Versão:** 2.1  
**Última Atualização:** 29/10/2025  
**Baseado em:** GDD v9.0 + TechMapping atual  
**Metodologia:** Desenvolvimento Iterativo em 6 Milestones

**Changelog v2.1:**

- ✅ Adicionado Sistema de Inventário detalhado (Fase 2.5)
- ✅ Adicionado Sistema de Diálogo (Fase 2.6)
- ✅ Adicionado Sistema de Save/Load (Fase 2.7)
- ✅ Adicionado Sistema de Árvore de Habilidades (Fase 3.2)
- ✅ Baseado no GDD v9.0 atualizado

---

## 📊 Resumo Executivo

### Status Atual

- **Progresso Geral:** 15% completo
- **Milestone Atual:** ALPHA 1 (Vertical Slice Interno)
- **Próximo Marco:** Janeiro 2026 ⚡
- **Lançamento Previsto:** Maio 2027 ⚡
- **Aceleração:** Gen AI (Vibe Coding) - Redução de 33-37% no tempo

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
| ALPHA 1 | 12-15 min | 2 (mínimo) | 0 | 35% | 3/2 | 🚧 Em Progresso |
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

- ✅ **Ninho do Slime** (90% completo)
  - ✅ Tutorial de movimento, ataque, destruição, coleta
  - 📋 Puzzle de introdução (placas de pressão + peso)
  
- 📋 **Floresta Calma - Recorte Mínimo** (0% completo)
  - 📋 **3 áreas compactas:**
    - Clareira de Entrada (transição da caverna)
    - Caminho dos Cervos (área linear)
    - Colmeia Pequena (área de desafio)
  
  - 📋 **3 NPCs básicos:**
    - 1 Cervo-Broto (passivo, wander)
    - 1 Esquilo Coletor (quest giver)
    - 1 Abelha Cristalina (patrulha)
  
  - 📋 **2 Inimigos básicos:**
    - Abelha Agressiva (persegue jogador)
    - Arbusto Espinhoso (estático, dano por contato)
  
  - 📋 **Conteúdo:**
    - 1 quest simples: "Colete 5 Flores Cristalinas"
    - 2 puzzles: Ponte de Vinhas + Pilares Hexagonais

**Sistemas (MVP):**

- 📋 Mecânica de Agachar (stealth básico - parado apenas)
- 📋 Sistema de Cristais Elementais (contador UI)
- 📋 Quest System básico (1 tipo: Collect)
- 📋 2 Habilidades Elementais Tier 1 (Nature + Fire)
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
2025 Nov ████████████████ (Atual - 15% completo)
2026 Jan ████████████████ ALPHA 1 ✓ ⚡
2026 Abr ████████████████ ALPHA 2 ✓ ⚡
2026 Out ████████████████ BETA ✓ ⚡
2027 Jan ████████████████ STEAM NEXT FEST ✓ ⚡
2027 Mai ████████████████ RELEASE 🚀 ⚡
2027 Jul ████████████████ POST-RELEASE 🎁
```

**Total de Desenvolvimento:** 18 meses (Nov 2025 → Mai 2027)  
**Economia com Gen AI:** ~9 meses (de 27 meses para 18 meses)

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

---

## 🎯 Fase 1: Fundação Técnica (Core Systems)

### 1.1 Arquitetura Base ✅

- [x] Unity 6.2 com URP configurado
- [x] Estrutura de pastas organizada
- [x] Sistema de Managers (Singleton pattern)
- [x] GameManager (ciclo de vida, preload de cenas)
- [x] CameraManager (Cinemachine integrado)
- [x] SceneTransitionManager (transições visuais)
- [x] Input System (Unity Input System)

### 1.2 Sistema de Cenas e Transições ✅

- [x] TeleportManager (teleporte entre pontos)
- [x] TeleportPoint (pontos de teleporte)
- [x] Cross-scene teleport (teleporte entre cenas)
- [x] ScreenEffectsManager (vinheta, transições)
- [x] TeleportTransitionHelper (efeitos visuais)
- [x] SceneSetupValidator (validação de cenas)

### 1.3 Ferramentas de Editor ✅

- [x] ExtraTools (ferramentas gerais)
- [x] CameraSetupTools (setup de câmera)
- [x] BushQuickConfig (configuração de arbustos)
- [x] ItemQuickConfig (configuração de itens)
- [x] GizmosHelper (visualização de colliders)
- [x] PolygonGizmosHelper (gizmos de polígonos)
- [x] ProjectSettingsExporterWindow (exportação de settings)

---

## 🎮 Fase 2: Gameplay Core

### 2.1 Controle do Jogador 🚧

- [x] PlayerController (movimento básico 8 direções)
- [x] PlayerInput (Unity Input System)
- [x] Rigidbody2D physics
- [x] Animação básica (Animator)
- [ ] **Mecânica de Agachar (NOVA)** 📋
  - [ ] Input de agachar (segurar botão)
  - [ ] Animação de achatar verticalmente
  - [ ] Sistema de stealth (detecção de cobertura)
  - [ ] Indicador visual (ícone de olho)
  - [ ] Restrição de movimento (parado quando agachado)
- [ ] Movimento gelatinoso aprimorado (bounce animation)
- [ ] Rastro de gosma visual
- [ ] Espremer por espaços apertados

### 2.2 Sistema de Atributos 🚧

- [x] PlayerAttributesHandler (HP, atributos básicos)
- [x] TakeDamage / Heal
- [x] Skill Points (adicionar/gastar)
- [ ] Sistema de Stamina (100 pontos, regeneração)
- [ ] Sistema de Evolução (Filhote → Adulto → Grande → Rei → Transcendente)
- [ ] Sistema de Reputação (invisível, 5 níveis)
- [ ] Tracking de conquistas para evolução

### 2.3 Sistema de Combate 📋

- [x] AttackHandler (ataque básico)
- [x] Detecção de colisão com inimigos
- [ ] Sistema de Stamina para habilidades
- [ ] Dano flutuante (números na tela)
- [ ] Sistema de críticos (10% chance, 1.5x dano)
- [ ] Resistências elementais
- [ ] Abordagens alternativas (stealth, diplomacia, tática)

### 2.4 Sistema de Itens 🚧

- [x] ItemCollectable (coleta de itens)
- [x] CollectableItemData (ScriptableObject)
- [x] BounceHandler (física de bounce)
- [x] ItemBuffHandler (buffs temporários)
- [x] DropController (drop de itens)
- [ ] Sistema de Inventário (20-40 slots)
- [ ] Categorização de itens
- [ ] UI de inventário

### 2.5 Sistema de Inventário 📋

- [ ] **Estrutura do Inventário**
  - [ ] Grid 5x4 (20 slots iniciais)
  - [ ] Expansível para 5x8 (40 slots)
  - [ ] Drag and drop para reorganizar
  - [ ] Stacking automático (máx 99 por slot)
  
- [ ] **UI do Inventário**
  - [ ] Atalho: Tab/Touchpad/View/-
  - [ ] Pausa o jogo quando aberto
  - [ ] Informações detalhadas ao passar mouse
  - [ ] Filtros por categoria
  - [ ] Borda colorida por raridade
  
- [ ] **Tipos de Itens**
  - [ ] Consumíveis (poções, comida, buffs)
  - [ ] Materiais de Crafting
  - [ ] Itens de Quest (não descartáveis)
  - [ ] Equipamentos (amuletos, anéis, capas)
  
- [ ] **Gerenciamento**
  - [ ] Usar/Equipar/Descartar/Dividir Stack
  - [ ] Sistema de favoritar
  - [ ] Notificação de inventário cheio
  - [ ] Itens no chão (5 min antes de desaparecer)

### 2.6 Sistema de Diálogo 📋

- [ ] **Tipos de Diálogo**
  - [ ] Diálogo Linear (sem escolhas)
  - [ ] Diálogo com Escolhas (2-4 opções)
  - [ ] Diálogo Condicional (baseado em contexto)
  
- [ ] **UI de Diálogo**
  - [ ] Caixa de diálogo (20% da tela, parte inferior)
  - [ ] Portrait do NPC (64x64, animado)
  - [ ] Efeito de digitação (30 char/s)
  - [ ] Indicador de "mais texto"
  - [ ] Botão de skip (após 2s)
  
- [ ] **Animações e Áudio**
  - [ ] Portrait anima (idle breathing)
  - [ ] Expressões (feliz, triste, surpreso, bravo)
  - [ ] Partículas emocionais
  - [ ] Som de "blip" durante digitação
  - [ ] SFX de emoção
  
- [ ] **Sistema de Memória**
  - [ ] Tracking de diálogos vistos
  - [ ] NPCs não repetem informações
  - [ ] Referências a conversas anteriores
  
- [ ] **Integração**
  - [ ] Quest System (iniciar/progresso/conclusão)
  - [ ] Friendship System (aumenta amizade)
  - [ ] Reputation System (reações baseadas em reputação)
  
- [ ] **DialogueData ScriptableObject**
  - [ ] Estrutura de nodes
  - [ ] Sistema de escolhas
  - [ ] Condições e efeitos
  - [ ] Suporte a localização

### 2.7 Sistema de Save/Load 📋

- [ ] **Pontos de Save**
  - [ ] Save automático (cenas, quests, evolução, 5 min)
  - [ ] Save manual (pontos de descanso, fogueiras)
  - [ ] Animação e confirmação visual
  
- [ ] **Dados Salvos**
  - [ ] Progresso do Jogador (posição, evolução, HP, stamina, reputação, cristais)
  - [ ] Inventário (itens, equipamentos, habilidades)
  - [ ] Progresso de Mundo (quests, NPCs, diálogos, amizades, Reis)
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
  - [ ] JSON serializado
  - [ ] Criptografia leve (anti-cheat)
  - [ ] Compressão
  - [ ] SaveData ScriptableObject

---

## 🌟 Fase 3: Sistemas Elementais

### 3.1 Cristais Elementais 📋

- [x] Prefab de cristal básico (crystalA)
- [ ] 7 tipos de cristais (Nature, Earth, Air, Water, Fire, Shadow, Ice)
- [ ] Sistema de contador (não ocupa inventário)
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
- [ ] Sistema de custo de Stamina
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

### 4.1 IA de Inimigos 📋

- [ ] FSM (Finite State Machine) base
- [ ] 10 estados de IA:
  - [ ] Idle (ocioso)
  - [ ] Patrol (patrulha)
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

### 4.3 IA de NPCs Amigáveis 📋

- [ ] Sistema de diálogo
- [ ] Sistema de amizade (5 níveis)
- [ ] Comportamento diário (schedule)
- [ ] Reações ao jogador
- [ ] Quest givers

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
  - [ ] Puzzle de placas de pressão (peso)
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

### 5.4 Sistema de Ambiente 🚧

- [x] WindManager (vento)
- [x] WindController (controle de vento)
- [x] WindEmulator (efeitos de vento)
- [x] BushShake (arbustos balançando)
- [x] SetupVisualEnvironment (variações visuais)
- [x] RandomStyle (estilos aleatórios)
- [ ] Sistema Dia/Noite (24 min = 1 dia)
- [ ] Sistema Sazonal (7 dias = 1 estação)
- [ ] Iluminação dinâmica (URP 2D Lights)
- [ ] Bioluminescência
- [ ] Partículas ambientais

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
  - [ ] Puzzle: Peso e Contrapeso

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
- [ ] Placas de pressão
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
- [ ] Peso e Contrapeso (Área Rochosa)
- [ ] Infiltração Silenciosa (Pântano das Névoas)

### 8.3 Sistema de Hints 📋

- [ ] Pistas visuais sutis
- [ ] NPCs dão dicas
- [ ] Sistema de hint progressivo
- [ ] Sem penalidade por usar hints

---

## 📜 Fase 9: Sistema de Quests

### 9.1 Quest System Core 📋

- [ ] Quest ScriptableObject
- [ ] QuestManager
- [ ] 6 tipos de objetivos:
  - [ ] Collect (coletar)
  - [ ] Defeat (derrotar)
  - [ ] Deliver (entregar)
  - [ ] Explore (explorar)
  - [ ] Interact (interagir)
  - [ ] Escort (escoltar)

### 9.2 Quest Tracking 📋

- [ ] Quest log UI
- [ ] Quest tracker (HUD)
- [ ] Marcadores no mapa
- [ ] Notificações de progresso
- [ ] Sistema de recompensas

### 9.3 Quests Principais 📋

- [ ] 5 quests para evolução Adulto
- [ ] 15 quests para evolução Grande Slime
- [ ] 30+ quests para evolução Rei Slime
- [ ] Quests de amizade (por espécie)
- [ ] Quests de Reis Monstros
- [ ] Side quests opcionais

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

### 13.1 HUD 📋

- [ ] HP Bar
- [ ] Stamina Bar
- [ ] Contador de Cristais Elementais
- [ ] Habilidades equipadas (com cooldowns)
- [ ] Quest Tracker
- [ ] Minimapa
- [ ] Indicador de stealth

### 13.2 Menus 📋

- [ ] Menu principal
- [ ] Menu de pausa
- [ ] Inventário
- [ ] Árvore de Habilidades
- [ ] Quest Log
- [ ] Mapa
- [ ] Coleção (Cristais de Pacto)
- [ ] Configurações
- [ ] Créditos

### 13.3 Feedback Visual 📋

- [ ] Dano flutuante
- [ ] Indicadores de buff/debuff
- [ ] Outline de interação
- [ ] Marcadores de quest
- [ ] Notificações de conquista
- [ ] Tutorial tooltips

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
- [ ] Custos de Stamina
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

## 🎯 Prioridades Imediatas (Next Steps)

### Sprint 1: Mecânica de Agachar

1. Implementar input de agachar
2. Animação de achatar
3. Sistema de detecção de cobertura
4. Integração com IA (stealth)
5. Primeiro puzzle usando agachar

### Sprint 2: Sistema de Evolução

1. Tracking de reputação
2. Condições de evolução
3. Sprites de evolução (24x24, 32x32)
4. Animação de transformação
5. Sistema de aura básico

### Sprint 3: Primeiro Bioma Completo

1. Floresta Calma (level design)
2. NPCs básicos (Cervos, Esquilos, Abelhas)
3. Sistema de amizade
4. Primeiro puzzle (Jardim Geométrico)
5. Rainha Melífera (primeiro Rei Monstro)

### Sprint 4: Sistema de Habilidades

1. Árvore de Habilidades (UI)
2. 4 habilidades Tier 1 (uma de cada elemento prioritário)
3. Sistema de cooldown
4. Sistema de Stamina
5. VFX de habilidades

---

## 📈 Estimativas de Tempo

### Desenvolvimento Core (Fases 1-4)

- **Estimativa:** 3-4 meses
- **Status:** ~60% completo

### Conteúdo Principal (Fases 5-11)

- **Estimativa:** 8-12 meses
- **Status:** ~5% completo

### Arte e Áudio (Fase 12)

- **Estimativa:** 4-6 meses (paralelo)
- **Status:** ~10% completo

### UI/UX e Sistemas (Fases 13-14)

- **Estimativa:** 2-3 meses
- **Status:** ~15% completo

### Testes e Polimento (Fases 15-16)

- **Estimativa:** 2-3 meses
- **Status:** 0% completo

### **Total Estimado:** 18-24 meses

### **Progresso Atual:** ~15%

---

## 🎓 Notas de Desenvolvimento

### Decisões de Design

- Progressão livre (qualquer ordem de Reis Monstros)
- Combate opcional (stealth/diplomacia viáveis)
- Sem timers ou pressão de tempo
- Atmosfera cozy e contemplativa
- Narrativa emergente (não linear)

### Desafios Técnicos

- Sistema de aura visual escalável (10 níveis)
- IA robusta com 10 estados
- Stealth com detecção de cobertura
- Puzzles criativos e integrados à lore
- Performance em Switch (30 FPS estável)

### Oportunidades de Expansão

- DLC com novos biomas
- Novos Reis Monstros
- Modo New Game+
- Desafios diários
- Multiplayer cooperativo (futuro distante)

---

**Fim do Roadmap v1.0**

---

## 🎯 Prioridades Imediatas (Roadmap para ALPHA)

### 🔥 Sprint 1: Finalizar Ninho do Slime (2 semanas)

**Objetivo:** Completar o tutorial com puzzle introdutório

1. **Puzzle de Introdução**
   - [ ] Criar puzzle de placas de pressão (peso)
   - [ ] Implementar mecânica de peso do slime
   - [ ] Adicionar objetos empurráveis (pedras)
   - [ ] Recompensa: Primeiro cristal elemental
   - [ ] Tutorial visual (sem texto)

2. **Polimento da Caverna**
   - [ ] Ajustar iluminação (URP 2D Lights)
   - [ ] Adicionar partículas ambientais (poeira, cristais)
   - [ ] SFX de ambiente (goteiras, ecos)
   - [ ] Transição suave para Floresta

---

### 🌿 Sprint 2-4: Floresta Calma - Recorte Alpha (6 semanas)

#### Sprint 2: Clareira de Entrada (2 semanas)

1. **Level Design**
   - [ ] Criar tileset de floresta (árvores, grama, flores)
   - [ ] Layout da Clareira de Entrada
   - [ ] Teleport point caverna ↔ floresta
   - [ ] Iluminação natural (dia)

2. **Vegetação Interativa**
   - [ ] Arbustos destrutíveis (reutilizar sistema)
   - [ ] Flores cristalinas coletáveis
   - [ ] Árvores com animação de vento

#### Sprint 3: Caminho dos Cervos + NPCs (2 semanas)

1. **NPCs Básicos**
   - [ ] Sprite de Cervo-Broto (16x16)
   - [ ] IA passiva (wander behavior)
   - [ ] 3 Cervos-Broto no caminho
   - [ ] Sprite de Esquilo Coletor (16x16)
   - [ ] IA de quest giver básico
   - [ ] 1 Esquilo na árvore

2. **Quest Simples**
   - [ ] "Colete 5 Flores Cristalinas"
   - [ ] Sistema de tracking de quest
   - [ ] UI de quest (simples)
   - [ ] Recompensa: 10 Cristais Verdes

3. **Puzzle de Crescimento**
   - [ ] Mecânica de crescimento de plantas
   - [ ] Puzzle: Fazer ponte de vinhas
   - [ ] Recompensa: Acesso à Colmeia

#### Sprint 4: Colmeia Pequena (2 semanas)

1. **Área da Colmeia**
   - [ ] Estrutura de mel (plataformas)
   - [ ] Sprite de Abelha Cristalina (16x16)
   - [ ] IA de patrulha simples (3 abelhas)
   - [ ] Cristais Verdes coletáveis

2. **Puzzle Geométrico Básico**
   - [ ] 3 pilares hexagonais
   - [ ] Ativar na ordem correta
   - [ ] Pista visual (flores no chão)
   - [ ] Recompensa: 15 Cristais Verdes

3. **Conexão de Volta**
   - [ ] Teleport point floresta → caverna
   - [ ] Atalho desbloqueado

---

### ⚡ Sprint 5: Mecânica de Agachar (2 semanas)

**Objetivo:** Implementar stealth básico

1. **Input e Animação**
   - [ ] Input de agachar (segurar Ctrl/B)
   - [ ] Sprite de slime achatado
   - [ ] Animação de transição (0.3s)
   - [ ] Restrição de movimento (parado)

2. **Sistema de Stealth**
   - [ ] Detecção de cobertura (raycast)
   - [ ] Indicador visual (ícone de olho)
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
   - [ ] Sistema de Stamina (100 pontos)
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

**Fim do Roadmap v1.1 - Atualizado com foco em desenvolvimento iterativo (Alpha → Beta → Release)**
