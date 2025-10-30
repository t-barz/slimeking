# The Slime King - Changelog

## [v8.0] - 2025

### 🎉 Grandes Adições

#### Mecânica de Agachar

- Implementada mecânica de stealth completa
- Jogador pode agachar atrás de objetos para ficar indetectável
- Velocidade reduzida para 40% enquanto agachado
- Abre possibilidades para puzzles de stealth e infiltração
- Integrada ao sistema de IA para detecção

#### Sistema de IA Completo

- **10 Estados de Inimigos:** Idle, Patrol, Alert, Investigate, Chase, Attack, Search, Return, Flee, Stunned
- **4 Estados de Aliados Não-Combatentes:** Wander, Interact, Follow, Flee
- **4 Estados de Aliados Combatentes:** Follow, Combat, Defend, Wait
- Sistema de percepção: Visão (line of sight), Audição, Proximidade
- Perfis de comportamento via ScriptableObjects
- Memória de curto prazo para IA

#### Sistema de Quests

- 6 tipos de objetivos: Collect, Defeat, Deliver, Explore, Interact, Escort
- Implementação via ScriptableObjects
- QuestManager para rastreamento
- UI minimalista de quest log e tracker
- 5 exemplos completos de quests

#### Sistema de Cutscenes

- 4 tipos: Dialogue, Cinematic, Ritual, Discovery
- Todas as cutscenes são puláveis
- Sistema de triggers para ativação
- Implementação modular e reutilizável

#### Puzzles Criativos

- 8 puzzles detalhados com implementação Unity
- Integrados à lore e mecânicas do jogo
- Múltiplas soluções quando possível
- Sistema de dicas progressivas
- Exemplos incluem: stealth, lógica, física, elementos

### 🔄 Mudanças Narrativas

#### Progressão Orgânica

- **Antes:** Slime busca ativamente se tornar Rei
- **Agora:** Reconhecimento vem naturalmente através de ações
- Sistema de reputação invisível (não exibido ao jogador)
- Rituais de Reconhecimento são oferecidos, não buscados
- Título de Rei Slime emerge naturalmente

#### Evolução Baseada em Experiências

- Evolução não é mais apenas "conquistar X auras"
- Requer combinação de: quests completadas, puzzles resolvidos, amizades, expansões do lar
- Mais orgânico e menos "gamificado"

### 📝 Documentação

#### Novo Conteúdo

- Seção completa de IA e Comportamento (Seção 8)
- Seção de Sistema de Quests (Seção 9)
- Seção de Sistema de Cutscenes (Seção 10)
- Seção de Sistema de Puzzles (Seção 11)
- 15+ exemplos de código Unity
- Diagramas de fluxo para sistemas

#### Melhorias

- Índice completo com links
- Resumo executivo (GDD-v8-Summary.md)
- Changelog detalhado (este arquivo)
- Estrutura mais clara e navegável

### 🛠️ Sistemas Técnicos

#### Implementação Unity 6.2

- Código exemplo para todos os sistemas principais
- Uso de URP (Universal Render Pipeline)
- ScriptableObjects para dados modulares
- Event System para comunicação
- Object Pooling para performance

### 🎨 Design de Gameplay

#### Stealth vs Combate

- Jogador pode escolher abordagem
- Stealth: agachar, cobertura, timing
- Combate: habilidades elementais, esquiva
- Diplomacia: pacificar inimigos, usar aliados
- Puzzles podem ter soluções stealth ou combate

#### Puzzles Integrados

- Cada puzzle faz sentido na lore
- Utilizam mecânicas estabelecidas
- Dificuldade progressiva
- Feedback visual claro
- Sem time limits (exceto puzzles específicos)

---

## [v7.3] - 2024

### Adicionado

- Sistema de Cristais de Pacto
- Câmara dos Pactos
- Sistema de Auras Elementais progressivo
- Rituais de Reconhecimento detalhados
- 10 Reis Monstros (anteriormente "monarcas")
- Modos de jogo: Clássico (5 auras) e Épico (10 auras)

### Modificado

- Sistema de evolução baseado em auras
- Progressão visual de aura mais detalhada
- Buffs da Câmara dos Pactos

---

## [v7.0] - 2024

### Adicionado

- Conceito inicial do jogo
- 6 biomas principais
- Sistema de evolução (4 estágios)
- Sistema de habilidades elementais
- Ciclos temporais (dia/noite, sazonal)
- Sistema de expansão do lar
- Direção visual e sonora

---

## Notas de Versão

### v8.0 - Foco em Sistemas de Gameplay

Esta versão consolida os documentos anteriores e adiciona sistemas cruciais que estavam faltando:

- IA robusta e comportamental
- Quests modulares e expansíveis
- Cutscenes não-intrusivas
- Puzzles criativos e integrados à lore
- Mecânica de stealth completa

### Próximos Passos

- Implementação Alpha (Q4 2025)
- Testes de gameplay dos sistemas de IA e stealth
- Balanceamento de puzzles
- Expansão do sistema de quests
- Polimento visual e sonoro

---

**Mantido por:** [Seu Nome/Equipe]  
**Última Atualização:** 2025
