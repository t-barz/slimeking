# The Slime King - GDD v8.0 - Resumo Executivo

## 🎯 Mudanças Principais da v7 para v8

### 1. Narrativa Orgânica

**Antes (v7):** Slime busca ativamente se tornar Rei  
**Agora (v8):** Reconhecimento vem naturalmente através de ações significativas

### 2. Mecânica de Agachar (NOVA)

- Pressionar e segurar B/Circle/B/Ctrl para agachar
- Torna-se indetectável atrás de objetos
- Velocidade reduzida para 40%
- Abre possibilidades para puzzles de stealth

### 3. Sistema de IA Completo

**10 Estados de Inimigos:**

- Idle, Patrol, Alert, Investigate, Chase, Attack, Search, Return, Flee, Stunned

**4 Estados de Aliados Não-Combatentes:**

- Wander, Interact, Follow, Flee

**4 Estados de Aliados Combatentes:**

- Follow, Combat, Defend, Wait

### 4. Sistema de Quests Simplificado

**6 Tipos de Objetivos:**

- Collect, Defeat, Deliver, Explore, Interact, Escort

**Implementação:** ScriptableObject-based para fácil expansão

### 5. Sistema de Cutscenes

**4 Tipos:**

- Dialogue (diálogos simples)
- Cinematic (com movimento de câmera)
- Ritual (Reconhecimento de Reis Monstros)
- Discovery (descoberta de áreas)

### 6. Puzzles Criativos

**8 Exemplos Detalhados:**

1. Jardim Geométrico (Elemental + Lógica)
2. Câmara do Eco (Stealth + Audição)
3. Constelação Perdida (Lógica + Observação)
4. Corrida Flamejante (Velocidade + Plataforma)
5. Equilíbrio do Pântano (Lógica + Diplomacia)
6. Reflexos Espelhados (Ambiental + Física)
7. Peso e Contrapeso (Física + Lógica)
8. Infiltração Silenciosa (Stealth Puro)

---

## 📊 Estatísticas do Documento

- **Páginas:** ~60 (estimado)
- **Seções Principais:** 17
- **Exemplos de Código:** 15+
- **Puzzles Detalhados:** 8
- **Estados de IA:** 18 (total)
- **Tipos de Quest:** 6
- **Biomas:** 7 (incluindo Ninho)
- **Reis Monstros:** 10

---

## 🎮 Para Desenvolvedores

### Implementação Prioritária (Alpha)

1. **Sistema de Movimentação**
   - Movimento básico ✓
   - Agachar (NOVO)
   - Esquiva

2. **Sistema de IA**
   - Estados básicos: Idle, Patrol, Chase, Attack
   - Percepção: Visão e Audição
   - Detecção de stealth

3. **Sistema de Quests**
   - QuestManager
   - Tipos: Collect, Defeat, Deliver
   - UI básica

4. **Puzzles**
   - 2 puzzles elementais
   - 1 puzzle de stealth
   - Sistema de dicas

5. **Cutscenes**
   - Dialogue cutscenes
   - Ritual cutscenes (1 exemplo)

---

## 📁 Arquivos Relacionados

- **GDD Completo:** `The-Slime-King-GDD-v8.md`
- **Versão Anterior:** `The-Slime-King-GDD-v7.md`
- **Design Essencial:** `GameDesign.md`

---

## 🔄 Changelog v7 → v8

### Adicionado

- Mecânica de agachar com stealth
- Sistema completo de máquina de estados de IA
- Sistema de quests simplificado
- Sistema de cutscenes
- 8 exemplos detalhados de puzzles
- Código de implementação para sistemas principais

### Modificado

- Narrativa: progressão orgânica em vez de busca ativa
- Evolução: baseada em reputação invisível
- Reis Monstros: agora chamados consistentemente (não "monarcas")

### Mantido

- Sistema de Auras Elementais
- Cristais de Pacto
- Câmara dos Pactos
- Biomas e mundo
- Sistemas temporais

---

**Versão:** 8.0  
**Data de Criação:** 2025  
**Status:** Completo e pronto para implementação
