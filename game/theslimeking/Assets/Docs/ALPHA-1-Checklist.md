# The Slime King - ALPHA 1 Checklist

**Versão:** 1.0  
**Data:** 14 de Novembro de 2025  
**Deadline:** Janeiro 2026 (2 meses)  
**Objetivo:** Demonstrar core gameplay com 2 biomas jogáveis (12-15 min gameplay)

---

## 📊 Resumo Executivo

### Status Atual

- **Progresso Geral:** 65% completo ⬆️
- **ALPHA 1 Systems:** 85% completo ⬆️
- **Estimativa:** 4 semanas restantes (com Gen AI)
- **Risco:** MUITO BAIXO (sistemas core + conteúdo avançado)

### Entregas Mínimas

- 12-15 minutos de gameplay polido
- 2 biomas completos (Ninho + Clareira)
- 3 NPCs funcionais + 2 inimigos
- Sistema de quest básico
- ✅ Sistema de stealth avançado (IMPLEMENTADO) ⭐
- Build standalone para testes internos

---

## ✅ **SISTEMAS JÁ COMPLETOS** (Não necessitam trabalho adicional)

### Core Systems (100% Completo)

- [x] **PlayerController** - Movimento 8 direções + sistema de stealth avançado ⭐
- [x] **PlayerAttributesHandler** - HP, atributos, skill points
- [x] **Sistema de Inventário** - 20 slots + equipamentos + quick slots
- [x] **Sistema de Diálogo** - DialogueManager + UI + localização
- [x] **Sistema de Quest** - QuestManager + UI + save/load completo
- [x] **Sistema de NPC** - NPCController + IA básica (Wander, Patrol, Static)
- [x] **Sistema de Save/Load** - Estrutura de dados completa
- [x] **UI/UX Foundations** - PauseMenu, ConfirmationDialog, InteractionIcons

### Biomas (100% Completo)

- [x] **Ninho do Slime** - Tutorial + puzzle stealth + empurrar pedra
- [x] **Floresta Calma - Clareira de Entrada** - Transição + pontos de coleta
- [x] **Floresta Calma - Caminho dos Cervos** - Área navegável + NPCs + quest
- [x] **Floresta Calma - Colmeia Pequena** - Área de desafio + puzzles

---

## 📋 **PENDENTE PARA ALPHA 1**

## 🎮 **1. BIOMAS & CONTEÚDO**

### 1.1 Floresta Calma - Expansão (4 semanas)

#### **Caminho dos Cervos** ✅ CONCLUÍDO

- [x] **Área Linear Navegável**
  - [x] Level design com 3-4 seções conectadas
  - [x] Pontos de coleta estratégicos (flores, cristais)
  - [x] Obstáculos simples (arbustos, pedras)
  - **✅ Critério:** Jogador consegue navegar de ponta a ponta em 2-3 min

- [x] **NPC: Cervo-Broto**
  - [x] Prefab com NPCController + AI Wander
  - [x] Animação básica (idle, walk)
  - [x] Comportamento passivo (foge se atacado)
  - **✅ Critério:** NPC vagueia naturalmente pela área

#### **Colmeia Pequena** ✅ CONCLUÍDO

- [x] **Área de Desafio**
  - [x] Level design vertical (plataformas)
  - [x] Puzzle simples usando mecânica de agachar
  - [x] Recompensa: cristais elementais
  - **✅ Critério:** Puzzle solucionável em 1-2 tentativas

- [x] **NPC: Abelha Cristalina (Patrulha)**
  - [x] Prefab com NPCController + AI Patrol
  - [x] Rota de patrulha predefinida
  - [x] Comportamento neutro (não ataca por padrão)
  - **✅ Critério:** Patrulha consistente, não trava em obstáculos

### 1.2 Inimigos Básicos (1 semana)

#### **Abelha Agressiva**

- [ ] **Comportamento de Perseguição**
  - [ ] AI Chase implementada
  - [ ] Detecção por proximidade (3 unidades)
  - [ ] Retorna ao ponto original se perder jogador
  - **Critério:** Persegue consistentemente, não fica presa

- [ ] **Sistema de Combate**
  - [ ] HP: 15 pontos
  - [ ] Dano ao jogador: 5 pontos por contato
  - [ ] Animação de ataque simples
  - **Critério:** Combate funcional sem bugs de colisão

#### **Arbusto Espinhoso**

- [ ] **Inimigo Estático**
  - [ ] HP: 25 pontos
  - [ ] Dano por contato: 8 pontos
  - [ ] Animação de "ferroada" quando tocado
  - **Critério:** Dano consistente, feedback visual claro

---

## 🎯 **2. SISTEMAS & MECÂNICAS**

### 2.1 Quest System - UI Completa (1 semana)

- [ ] **Quest Log UI**
  - [ ] Painel acessível via pause menu
  - [ ] Lista de quests ativas com progresso
  - [ ] Descrição detalhada de cada quest
  - **Critério:** Interface intuitiva, dados sempre atualizados

- [ ] **Quest Tracker HUD**
  - [ ] Indicador discreto na tela principal
  - [ ] Progresso da quest principal visível
  - [ ] Atualização automática em tempo real
  - **Critério:** Não interfere no gameplay, sempre preciso

### 2.2 Sistema de Habilidades Básico (2 semanas)

#### **Nature (Tier 1) - Crescimento Rápido**

- [ ] **Implementação**
  - [ ] Spawn temporário de plataforma vegetal
  - [ ] Cooldown: 8 segundos
  - [ ] Custo: 10 cristais Nature
  - [ ] Input: tecla Q ou equivalente
  - **Critério:** Habilidade responsiva, timing adequado

#### **Fire (Tier 1) - Bola de Fogo**

- [ ] **Implementação**
  - [ ] Projétil com trajetória reta
  - [ ] Dano: 15 pontos
  - [ ] Cooldown: 6 segundos
  - [ ] Custo: 12 cristais Fire
  - [ ] Input: tecla E ou equivalente
  - **Critério:** Precisão consistente, dano confiável

### 2.3 HUD & Interface (1 semana)

- [ ] **HP Bar**
  - [ ] Barra visual no canto superior esquerdo
  - [ ] Animação suave para mudanças
  - [ ] Indicador visual quando low HP
  - **Critério:** Sempre atualizada, feedback claro

- [ ] **Contadores de Cristais Elementais**
  - [ ] Ícones para Nature, Fire, Water, Shadow, Earth, Air
  - [ ] Contador numérico para cada tipo
  - [ ] Posição: canto superior direito
  - **Critério:** Atualizados em tempo real, fácil leitura

### ✅ 2.4 Mecânica de Agachar - Sistema Avançado (IMPLEMENTADO - 14 Nov 2025) ⭐

- [x] **Sistema de Stealth Completo**
  - [x] Fade visual semi-transparente após 2 segundos agachado
  - [x] Detecção de cobertura usando Physics2D + sorting Y
  - [x] Multi-SpriteRenderer: fade aplicado a todos subobjetos
  - [x] StealthEvents: comunicação desacoplada para IA
  - [x] GameManager integration: `IsPlayerInStealth()` para inimigos
  - **✅ Critério:** Sistema completo e funcional

- [x] **Feedback Visual Avançado**
  - [x] Fade suave de 0.5s duração
  - [x] Alpha configurável (padrão 0.5f)
  - [x] Preservação de cores RGB originais
  - [x] Debug gizmos no Scene View
  - [x] Logs detalhados para debugging
  - **✅ Critério:** Feedback profissional implementado

---

## 🎲 **3. CONTEÚDO ESPECÍFICO**

### ✅ 3.1 Quest Principal - "Colete 5 Flores Cristalinas" (CONCLUÍDO)

- [x] **Configuração da Quest**
  - [x] QuestGiverController no Esquilo Coletor
  - [x] CollectQuestData configurada (5x Flor Cristalina)
  - [x] Recompensa: 25 cristais Nature + item de cura
  - **✅ Critério:** Quest aceita, tracked e completa corretamente

- [x] **Itens de Coleta**
  - [x] 7-8 Flores Cristalinas espalhadas pelos biomas
  - [x] ItemCollectable configurado corretamente
  - [x] Feedback visual/sonoro na coleta
  - **✅ Critério:** Coleta responsiva, progresso atualizado

### ✅ 3.2 NPC: Esquilo Coletor - Quest Giver (CONCLUÍDO)

- [x] **Configuração de Diálogo**
  - [x] Diálogo inicial (apresentação + quest offer)
  - [x] Diálogo de progresso ("ainda coletando...")
  - [x] Diálogo de completion (entrega + recompensa)
  - **✅ Critério:** Fluxo de diálogo completo sem bugs

- [x] **Integração Quest System**
  - [x] QuestGiverController configurado
  - [x] Quest aceita via escolha de diálogo
  - [x] Quest entregue via diálogo
  - **✅ Critério:** Integração perfeita quest + diálogo

---

## 🛠️ **4. POLIMENTO & QUALIDADE**

### 4.1 Polimento Visual (1 semana - distribuído)

- [x] **Iluminação (URP 2D)**
  - [x] Ninho: luz suave e aconchegante
  - [x] Floresta: luz natural filtrada
  - [x] Transições suaves entre áreas
  - **Critério:** Atmosfera consistente e agradável

- [ ] **Partículas Ambientais**
  - [ ] Ninho: poeira cintilante, goteiras
  - [ ] Floresta: pólen, folhas caindo
  - [ ] Moderadas (performance)
  - **Critério:** Ambiente imersivo sem impacto de performance

### 4.2 Audio & SFX (0.5 semana)

- [ ] **Sons Ambientais**
  - [ ] Ninho: eco de goteiras, cristais ressonando
  - [ ] Floresta: pássaros, vento nas folhas
  - [ ] Volume balanceado
  - **Critério:** Áudio não invasivo, melhora imersão

- [ ] **SFX de Ações**
  - [ ] Coleta de itens (som satisfatório)
  - [ ] Uso de habilidades (feedback sonoro)
  - [ ] Interações com NPCs
  - **Critério:** Feedback claro para todas as ações

### 4.3 Balanceamento (0.5 semana)

- [ ] **Valores de Gameplay**
  - [ ] HP do jogador: 100 pontos
  - [ ] Dano dos inimigos balanceado
  - [ ] Cooldowns das habilidades
  - [ ] Custo/recompensa de cristais
  - **Critério:** Gameplay desafiador mas justo

---

## 🧪 **5. TESTES & VALIDAÇÃO**

### 5.1 Testes Técnicos (0.5 semana)

- [ ] **Testes de Sistema**
  - [ ] Save/Load funciona em todas as situações
  - [ ] Performance estável (60 FPS mínimo)
  - [ ] Sem memory leaks após 30 min de jogo
  - [ ] UI responsiva em diferentes resoluções
  - **Critério:** Zero bugs críticos

### 5.2 Testes de Gameplay (0.5 semana)

- [ ] **Fluxo Completo**
  - [ ] Tutorial → Quest → Combate → Conclusão
  - [ ] Tempo de gameplay: 12-15 minutos
  - [ ] Curva de dificuldade apropriada
  - [ ] Onboarding claro para novos jogadores
  - **Critério:** Experiência coesa e satisfatória

### 5.3 Build & Deploy (0.5 semana)

- [ ] **Build Standalone**
  - [ ] Build Windows 64-bit funcional
  - [ ] Tamanho otimizado (<500MB)
  - [ ] Assets comprimidos apropriadamente
  - [ ] Configurações de qualidade definidas
  - **Critério:** Build stable para distribuição interna

---

## 📅 **CRONOGRAMA ATUALIZADO (4 semanas restantes)**

### **Semanas 1-2: Inimigos + HUD** (14-28 Nov 2025)

- Abelha Agressiva + Arbusto Espinhoso
- HUD completo (HP + Cristais)
- Sistema de Habilidades (Nature + Fire Tier 1)

### **Semanas 3-4: Polimento + Testes** (28 Nov - 12 Dez 2025)

- Audio/Visual polish
- Balanceamento + Build final
- Testes e validação completa

---

## 🚨 **CRITÉRIOS DE SUCESSO**

### Mínimo Viável (MVP)

- [ ] 2 biomas navegáveis sem bugs críticos
- [ ] 1 quest completa funcional
- [ ] 3 NPCs + 2 inimigos operacionais
- [ ] 2 habilidades elementais funcionais
- [ ] Save/Load básico funcional

### Ideal (Nice to Have)

- [ ] Polimento visual/sonoro completo
- [ ] Balanceamento refinado
- [ ] Performance otimizada
- [ ] UX intuitiva para novos jogadores
- [ ] Build otimizada (<300MB)

### Critérios de Rejeição

- ❌ Bugs que impedem progressão
- ❌ Performance <45 FPS em hardware mínimo
- ❌ Save/Load não funcional
- ❌ Quest system quebrado
- ❌ Gameplay <10 minutos ou >20 minutos

---

## 🎯 **RESPONSABILIDADES**

### **Developer (Solo)**

- Implementação de todos os sistemas
- Level design dos biomas
- Configuração de NPCs e inimigos
- Integração e testes

### **Gen AI Assistant**

- Code generation e boilerplate
- Debug e troubleshooting
- Refatoração e otimização
- Documentação de sistemas

---

## 📈 **TRACKING DE PROGRESSO**

**Atualização Semanal Obrigatória:**

- [x] Semana 1 (21 Nov): 65% completo ⬆️
- [ ] Semana 2 (28 Nov): ___% completo
- [ ] Semana 3 (05 Dez): ___% completo
- [ ] Semana 4 (12 Dez): 100% completo 🎉

**Target Final:** 12 de Dezembro de 2025 - 100% ALPHA 1 Completo ⬆️ (Adiantado 4 semanas!)

---

## 📝 Changelog v1.2 (14 Nov 2025)

### ✅ Progresso Significativo - FLORESTA CALMA CONCLUÍDA

**Conteúdo Implementado:**

- **Floresta Calma - Caminho dos Cervos**: Área navegável + NPCs + quest completa
- **Floresta Calma - Colmeia Pequena**: Área de desafio + puzzles + patrulha
- **Quest "Colete 5 Flores Cristalinas"**: Sistema completo funcional
- **NPCs**: Cervo-Broto (wander) + Esquilo Coletor (quest giver) + Abelha Cristalina (patrol)

**Impacto no Progresso:**

- Progresso Geral: 45% → 65% (+20%)
- Alpha 1 Systems: 70% → 85% (+15%)
- Tempo restante: 7 → 4 semanas (-3 semanas)
- Target: 9 Jan → 12 Dez 2025 (Adiantado 4 semanas!)

**Próximos Passos:**

- Foco em inimigos básicos (Abelha Agressiva, Arbusto Espinhoso)
- HUD + sistema de habilidades (Nature, Fire)
- Polimento final e build standalone

---

## 📝 Changelog v1.1 (14 Nov 2025)

### ✅ Sistema de Stealth Avançado - IMPLEMENTADO

**Detalhes Técnicos:**

- **PlayerController**: Sistema multi-SpriteRenderer para fade em todos subobjetos
- **StealthEvents**: Comunicação desacoplada entre PlayerController ↔ GameManager ↔ IA
- **GameManager**: Métodos `IsPlayerInStealth()`, `IsPlayerCrouching()`, `HasPlayerCover()`
- **Configurações**: Timer 2s, fade 0.5s, alpha 0.5f, raio detecção 1.5f

**Impacto no Progresso:**

- Progresso Geral: 40% → 45% (+5%)
- Alpha 1 Systems: 60% → 70% (+10%)
- Tempo restante: 8 → 7 semanas (-1 semana)

**Próximos Passos:**

- Integração com IA de inimigos (usar `GameManager.Instance.IsPlayerInStealth()`)
- Testes de gameplay com sistema de stealth
- Polimento de animações e feedback visual

---

**Última Atualização:** 14 de Novembro de 2025  
**Próxima Revisão:** 21 de Novembro de 2025
