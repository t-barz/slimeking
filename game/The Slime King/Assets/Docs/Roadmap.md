# 🗺️ Roadmap de Desenvolvimento - The Slime King

## 🎯 Visão Geral

Este roadmap centraliza **TODAS** as tarefas de desenvolvimento do projeto, organizadas por prioridade e fase. **SEMPRE consulte este documento antes de iniciar qualquer desenvolvimento.**

---

## 🏁 Fase 0 - Setup Inicial do Projeto [🔄 EM ANDAMENTO]

### ⚙️ Configuração Base

- [ ] **0.1** - Executar setup automático via `Extra Tools > Setup Projeto Completo`
- [ ] **0.2** - Configurar Input System (reiniciar Unity após setup)
- [ ] **0.3** - Verificar URP ativo no projeto
- [ ] **0.4** - Criar cena de teste inicial (`Game/Scenes/scn_testScene.unity`)
- [ ] **0.5** - Setup básico do Player GameObject com sprite placeholder

### 📋 Validação do Setup

- [ ] **0.6** - Testar GameManager funcionando
- [ ] **0.7** - Testar AudioManager reproduzindo som placeholder
- [ ] **0.8** - Testar InputManager capturando movimento
- [ ] **0.9** - Testar PlayerController movimentando na cena
- [ ] **0.10** - Configurar câmera seguindo player

**🎯 Meta da Fase 0:** Base técnica funcionando com player se movendo em cena de teste.

---

## 🏁 Fase 1 - Fundação do Gameplay [⏳ PRÓXIMA]

### 🎮 Sistema Core

- [ ] **1.1** - Implementar Input System completo (substituir placeholders)
- [ ] **1.2** - Sistema de cristais elementais básico
- [ ] **1.3** - Sistema de evolução do slime (4 estágios)
- [ ] **1.4** - Sistema de habilidades elementais (4 slots)
- [ ] **1.5** - Sistema de salvamento e carregamento

### 🎨 Arte e Animação Base

- [ ] **1.6** - Sprites do slime (4 estágios evolutivos)
- [ ] **1.7** - Animações básicas do slime (idle, walk, habilidades)
- [ ] **1.8** - Sistema de animação via Animator Controllers
- [ ] **1.9** - Efeitos visuais das habilidades elementais

### 🔊 Audio Base

- [ ] **1.10** - SFX básicos (movimento, habilidades, interação)
- [ ] **1.11** - Música ambiente base
- [ ] **1.12** - Sistema de áudio aleatório para evitar repetição

**🎯 Meta da Fase 1:** Slime controlável com evolução básica e habilidades elementais funcionando.

---

## 🏁 Fase 2 - Primeiro Bioma Completo [⏳ FUTURA]

### 🌱 Floresta Calma (Nature)

- [ ] **2.1** - Design e layout do bioma
- [ ] **2.2** - Sistema de criaturas (3 tipos iniciais)
- [ ] **2.3** - Sistema de amizade com criaturas
- [ ] **2.4** - Puzzles elementais básicos
- [ ] **2.5** - Sistema de cristais espalhados pelo bioma

### 🎨 Arte do Bioma

- [ ] **2.6** - Tileset da Floresta Calma
- [ ] **2.7** - Sprites das criaturas (Cervos-Broto, Esquilos, Ouriços)
- [ ] **2.8** - Animações das criaturas
- [ ] **2.9** - Efeitos de partículas ambientais

### 🌅 Sistema Temporal

- [ ] **2.10** - Ciclo dia/noite (24min = 1 dia)
- [ ] **2.11** - Criaturas diferentes por período
- [ ] **2.12** - Iluminação dinâmica
- [ ] **2.13** - Sistema de clima básico

**🎯 Meta da Fase 2:** Primeiro bioma jogável com criaturas, puzzles e ciclo temporal.

---

## 🏁 Fase 3 - Sistema de Lar [⏳ FUTURA]

### 🏠 Ninho do Slime

- [ ] **3.1** - Design da caverna inicial
- [ ] **3.2** - Sistema de melhorias do lar (4 áreas)
- [ ] **3.3** - Integração com sistema de amizade
- [ ] **3.4** - Benefícios das melhorias (cristais, cura, previsão)

### 💎 Sistema de Recursos

- [ ] **3.5** - Economia de cristais refinada
- [ ] **3.6** - Sistema de geração passiva
- [ ] **3.7** - Interface de gerenciamento do lar
- [ ] **3.8** - Sistema de conquistas de lar

**🎯 Meta da Fase 3:** Lar funcional que evolui com o progresso do jogador.

---

## 🏁 Fase 4 - Expansão de Biomas [⏳ FUTURA]

### 💧 Lago Espelhado (Water)

- [ ] **4.1** - Design e implementação do bioma aquático
- [ ] **4.2** - Criaturas aquáticas (3 tipos)
- [ ] **4.3** - Puzzles relacionados à água
- [ ] **4.4** - Integração com sistema temporal

### 🏔️ Área Rochosa (Earth)

- [ ] **4.5** - Design montanhoso e penhascos
- [ ] **4.6** - Criaturas terrestres (3 tipos)
- [ ] **4.7** - Puzzles de manipulação de terra
- [ ] **4.8** - Sistema de escalada/plataformas

### 🌫️ Pântano das Névoas (Shadow)

- [ ] **4.9** - Bioma com mecânicas de visibilidade
- [ ] **4.10** - Criaturas noturnas especiais
- [ ] **4.11** - Puzzles de sombra e luz
- [ ] **4.12** - Sistema de névoa dinâmica

**🎯 Meta da Fase 4:** 4 biomas únicos com mecânicas distintas.

---

## 🏁 Fase 5 - Biomas Finais [⏳ FUTURA]

### 🌋 Câmaras de Lava (Fire)

- [ ] **5.1** - Bioma com desafios de calor
- [ ] **5.2** - Criaturas de fogo resistentes
- [ ] **5.3** - Puzzles de manipulação de lava
- [ ] **5.4** - Sistema de temperatura/proteção

### ❄️ Pico Nevado (Air)

- [ ] **5.5** - Bioma final com condições extremas
- [ ] **5.6** - Criaturas árticas especiais
- [ ] **5.7** - Puzzles de controle de vento
- [ ] **5.8** - Boss final - Transformação em Rei Slime

**🎯 Meta da Fase 5:** Jogo completo com 6 biomas e conclusão da jornada.

---

## 🏁 Fase 6 - Polish e Lançamento [⏳ FUTURA]

### 🎨 Polish Visual

- [ ] **6.1** - Post-processing por bioma
- [ ] **6.2** - Efeitos de transição entre áreas
- [ ] **6.3** - UI/UX refinada
- [ ] **6.4** - Animações de feedback aprimoradas

### 🔊 Audio Completo

- [ ] **6.5** - Trilha sonora completa (7 temas)
- [ ] **6.6** - SFX refinados e variações
- [ ] **6.7** - Audio adaptativo por situação
- [ ] **6.8** - Mixagem e masterização final

### 🎮 Sistemas Finais

- [ ] **6.9** - Sistema de conquistas
- [ ] **6.10** - Opções de acessibilidade
- [ ] **6.11** - Múltiplos idiomas
- [ ] **6.12** - Performance optimization final

### 📦 Preparação para Lançamento

- [ ] **6.13** - Build final para múltiplas plataformas
- [ ] **6.14** - Testes finais e correção de bugs
- [ ] **6.15** - Marketing materials
- [ ] **6.16** - Distribuição e lançamento

**🎯 Meta da Fase 6:** Jogo pronto para lançamento comercial.

---

## 📊 Status Atual do Projeto

### ✅ Concluído

- Documentação base estruturada
- Padrões de desenvolvimento definidos

### 🔄 Em Andamento

- **Fase 0**: Setup inicial do projeto

### ⏳ Próximas Prioridades

1. **0.1 - 0.5**: Setup técnico completo
2. **0.6 - 0.10**: Validação da base
3. **1.1 - 1.5**: Sistemas fundamentais

---

## 🎯 Como Usar Este Roadmap

### 📋 Workflow Diário

1. **Verificar** status atual neste roadmap
2. **Escolher** próxima tarefa da fase atual
3. **Atualizar** status ao completar (`[ ]` → `[✅]`)
4. **Documentar** problemas ou mudanças necessárias

### 🔄 Atualizações do Roadmap

- **Semanalmente**: Revisar progresso e ajustar estimativas
- **Por fase**: Reavaliar próximas prioridades
- **Quando necessário**: Adicionar tarefas descobertas durante desenvolvimento

### 📝 Convenções de Status

- `[ ]` - Tarefa não iniciada
- `[🔄]` - Tarefa em andamento
- `[✅]` - Tarefa concluída
- `[❌]` - Tarefa cancelada/mudança de escopo
- `[⚠️]` - Tarefa com problemas/bloqueadores

---

## 🚀 Estimativas de Tempo (Solo Developer)

- **Fase 0**: 1-2 semanas
- **Fase 1**: 4-6 semanas  
- **Fase 2**: 6-8 semanas
- **Fase 3**: 3-4 semanas
- **Fase 4**: 12-16 semanas
- **Fase 5**: 8-10 semanas
- **Fase 6**: 4-6 semanas

**Total estimado**: 38-52 semanas (~1 ano)

---

## 💡 Notas de Desenvolvimento

### 🎯 Princípios de Priorização

1. **Funcionalidade antes de polish**
2. **Sistema completo antes de expandir**
3. **Testabilidade em cada etapa**
4. **Feedback loop curto entre implementação e teste**

### ⚠️ Riscos Identificados

- **Scope creep**: Manter foco nas fases definidas
- **Over-engineering**: Implementar só o necessário por fase
- **Burnout**: Celebrar marcos e manter breaks regulares

### 🔄 Pontos de Revisão

- **Final de cada fase**: Avaliar qualidade e ajustar roadmap
- **Mensalmente**: Revisar velocidade e reestimar prazos
- **Marcos importantes**: Considerar feedback de playtesters
