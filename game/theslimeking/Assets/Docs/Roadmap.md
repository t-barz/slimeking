# 🗺️ Roadmap de Desenvolvimento - The Slime King

## 🎯 Visão Geral

Este roadmap centraliza **TODAS** as tarefas de desenvolvimento do projeto, organizadas por prioridade e fase. **SEMPRE consulte este documento antes de iniciar qualquer desenvolvimento.**

---

## 🏗️ **FASE 1: ARQUITETURA CORE (PRIORIDADE MÁXIMA)**

### 🎮 **Managers Core - Singletons**

#### ✅ **CONCLUÍDO**

- [x] **Análise e Redesign da Arquitetura**: Simplificação de 8+ managers para apenas 3 essenciais
- [x] **Documentação Atualizada**: Novo design document com arquitetura simplificada

#### 🔨 **EM DESENVOLVIMENTO**

**1.1 GameManager Implementation**

- [x] **Criar classe base ManagerSingleton<T>**
  - ✅ Padrão singleton com DontDestroyOnLoad
  - ✅ Sistema de logs opcional por manager
  - ✅ Template base para outros managers
  
- [x] **Implementar GameManager completo**
  - ✅ Sistema de estados (Playing, Paused, MainMenu, Loading, Settings)
  - ✅ Sistema temporal (dia/noite, estações, clima)
  - ✅ Coordenação de evolução do slime
  - ✅ Eventos globais de comunicação
  
- [x] **Criar enums e estruturas de dados**
  - ✅ GameState, SlimeStage, Season, WeatherType
  - ✅ ElementType, BiomeType, TimeOfDay
  - ✅ Sistema de eventos GameEvents

**1.2 AudioManager Implementation**

- [ ] **Sistema de Audio Pool**
  - Pool otimizado de AudioSources para SFX
  - AudioSource dedicado para música
  - Sistema de fade in/out para transições
  
- [ ] **Sistema de Volume**
  - Controles separados: Master, Music, SFX
  - Persistência via SaveManager
  - Aplicação em tempo real
  
- [ ] **Coleções de Audio**
  - Sistema de carregamento de AudioClips
  - Suporte a múltiplas variações de SFX
  - Música adaptativa por bioma/clima

**1.3 SaveManager Implementation**

- [ ] **Sistema de Persistência**
  - Serialização JSON segura
  - Validação de dados salvos
  - Recuperação de erros de corrupção
  
- [ ] **Estrutura GameData**
  - Progressão do slime (estágio, XP elemental)
  - Biomas desbloqueados
  - Conquistas e marcos
  - Configurações do jogador
  
- [ ] **Auto-Save System**
  - Salvamento automático configurável
  - Pontos de checkpoint importantes
  - Indicador visual de salvamento

**1.4 SceneTransitionManager Implementation** ✅ **CONCLUÍDO**

- [x] **Sistema de Transições de Cena**
  - ✅ SceneTransitionManager como Singleton
  - ✅ Efeito CellularEffect para transições
  - ✅ Carregamento assíncrono de cenas
  - ✅ Sistema de fallback para segurança
  
- [x] **Integração com TitleScreen**
  - ✅ TitleScreenController atualizado
  - ✅ Transição TitleScreen → InitialCave
  - ✅ Configuração via Inspector
  - ✅ Logs de debug implementados

---

## 🎯 **FASE 2: SCENE CONTROLLERS**

### 🏞️ **Sistema de Controllers por Bioma**

**2.1 Base Controller System**

- [ ] **Criar SceneControllerBase abstrato**
  - Template comum para todos os controllers de cena
  - Sistema de inicialização e cleanup
  - Comunicação com GameManager via eventos
  
- [ ] **Sistema de Spawn Points**
  - Pontos de entrada para cada bioma
  - Transições suaves entre cenas
  - Preservação de estado do slime

**2.2 Controllers Específicos (Por Prioridade)**

**Alta Prioridade:**

- [ ] **NestController** - Ninho do Slime (Tutorial)
  - Sistema de expansão do lar
  - Tutorial de controles básicos
  - Centro de salvamento e descanso
  
- [ ] **ForestController** - Floresta Calma
  - Sistema de criaturas (Cervos-Broto, Esquilos, Ouriços)
  - Sistema de clima dinâmico
  - Spawn de cristais Nature/Earth/Air

**Média Prioridade:**

- [ ] **LakeController** - Lago Espelhado
  - Sistema aquático e reflexos
  - Criaturas aquáticas específicas
  - Mecânicas de natação

- [ ] **RockController** - Área Rochosa
  - Sistema de escalada e plataformas
  - Golems e criaturas rochosas
  - Cristais Earth/Fire

**Baixa Prioridade:**

- [ ] **SwampController** - Pântano das Névoas
- [ ] **VolcanoController** - Câmaras de Lava
- [ ] **SnowController** - Pico Nevado

---

## 🔄 **FASE 3: SISTEMAS DE APOIO**

### 🎮 **Gameplay Systems**

**3.1 Player System**

- [ ] **PlayerController base**
  - Movimentação 2D top-down
  - Sistema de input via Unity Input System
  - Estados do player (Moving, Idle, Interacting)
  
- [ ] **Slime Evolution System**
  - Visualização da evolução (Baby → Adult → Large → King)
  - Sistema de XP elemental
  - Desbloqueio de habilidades

**3.2 Creature System**

- [ ] **Base para todas as criaturas**
  - IA comportamental simples
  - Sistema de amizade/interação
  - Spawning dinâmico por bioma

**3.3 Weather System**

- [ ] **Sistema climático dinâmico**
  - Transições suaves entre condições
  - Efeitos visuais por clima
  - Impacto no gameplay

---

## 📋 **FASE 4: INTEGRAÇÃO E POLISH**

### 🔧 **Integration Tasks**

- [ ] **Teste de integração entre managers**
- [ ] **Otimização de performance**
- [ ] **Sistema de debugging em runtime**
- [ ] **Documentação técnica final**

### 🎨 **Polish Tasks**

- [ ] **Efeitos visuais e partículas**
- [ ] **Animações de transição**
- [ ] **Feedback audiovisual**
- [ ] **Balanceamento de gameplay**

---

## 📊 **STATUS GERAL**

| **Componente** | **Status** | **Prioridade** | **Estimativa** |
|----------------|------------|----------------|----------------|
| **Arquitetura Core** | ✅ Planejado | 🔴 Crítica | 1 semana |
| **GameManager** | ✅ Concluído | 🔴 Crítica | 2 dias |
| **SceneTransitionManager** | ✅ Concluído | 🟡 Alta | 1 dia |
| **AudioManager** | ⏳ Pendente | 🟡 Alta | 1 dia |
| **SaveManager** | ⏳ Pendente | 🟡 Alta | 1 dia |
| **NestController** | ⏳ Pendente | 🟡 Alta | 2 dias |
| **ForestController** | ⏳ Pendente | 🟡 Alta | 2 dias |
| **Player System** | ⏳ Pendente | 🟠 Média | 3 dias |
| **Outros Controllers** | ⏳ Pendente | 🔵 Baixa | 1 semana |

---

## 🎯 **PRÓXIMOS PASSOS IMEDIATOS**

1. **[CONCLUÍDO]** ✅ Implementar classe base `ManagerSingleton<T>`
2. **[CONCLUÍDO]** ✅ Criar e testar `GameManager` completo
3. **[CONCLUÍDO]** ✅ Implementar `SceneTransitionManager` com efeito cellular
4. **[PRÓXIMO]** Implementar `AudioManager` e `SaveManager`
5. **[ESTA SEMANA]** Criar `NestController` para tutorial
6. **[PRÓXIMA SEMANA]** Implementar `ForestController` e sistema de criaturas

---
