# 🎮 Bem-vindo ao The Slime King

## 👋 Olá

Seja bem-vindo à equipe de desenvolvimento de **The Slime King**! Este documento vai te ajudar a se orientar rapidamente no projeto.

---

## 🎯 O que é The Slime King?

**The Slime King** é um RPG 2D top-down cozy de exploração e aventura onde você controla um slime branco raro que, através de suas ações e interações com o mundo, naturalmente ganha reconhecimento e influência até se tornar o primeiro Rei Slime da história.

### Pilares do Jogo

1. **Exploração Orgânica** - Sem objetivos forçados, descubra no seu ritmo
2. **Progressão Natural** - Reconhecimento vem de ações significativas
3. **Atmosfera Cozy** - Relaxante, contemplativo, sem pressão de tempo
4. **Stealth Criativo** - Mecânica de agachar permite abordagens não-violentas
5. **Puzzles Integrados** - Cada puzzle faz sentido na lore do mundo

### Diferenciais

- Sistema de IA robusto com 18 estados diferentes
- Mecânica de stealth única usando agachar
- 10 Reis Monstros com personalidades distintas
- Puzzles criativos integrados à narrativa
- Progressão visual impressionante (slime cresce e ganha aura)

---

## 📚 Primeiros Passos

### 1. Leia a Documentação Essencial

**Comece por aqui (nesta ordem):**

1. **[GameDesign.md](GameDesign.md)** (5 min)
   - Visão geral simplificada do jogo
   - Conceitos principais
   - Mecânicas core

2. **[GDD-v8-Summary.md](GDD-v8-Summary.md)** (10 min)
   - Resumo das mudanças recentes
   - Novidades da versão 8.0
   - Estatísticas do projeto

3. **[INDEX.md](INDEX.md)** (5 min)
   - Navegação pela documentação
   - Onde encontrar cada informação
   - Guia por função (designer, programador, etc.)

**Depois, conforme sua função:**

#### Se você é Programador

- [Quick-Reference.md](Quick-Reference.md) - Sua bíblia diária
- [Implementation-Guide.md](Implementation-Guide.md) - Roadmap de implementação
- [The-Slime-King-GDD-v8.md](The-Slime-King-GDD-v8.md) seções 8-11 - Sistemas técnicos

#### Se você é Designer

- [The-Slime-King-GDD-v8.md](The-Slime-King-GDD-v8.md) seções 1-3, 11 - Design e puzzles
- [Quick-Reference.md](Quick-Reference.md) - Referência de mecânicas

#### Se você é Artista

- [The-Slime-King-GDD-v8.md](The-Slime-King-GDD-v8.md) seções 2, 3, 4, 12 - Visual e mundo
- [Implementation-Guide.md](Implementation-Guide.md) seção "Assets Necessários"

#### Se você é Sound Designer

- [The-Slime-King-GDD-v8.md](The-Slime-King-GDD-v8.md) seção 12.2 - Direção sonora
- [Implementation-Guide.md](Implementation-Guide.md) seção "Assets Necessários"

### 2. Configure seu Ambiente

**Para Programadores:**

```bash
# Clone o repositório
git clone [URL_DO_REPOSITORIO]

# Abra no Unity 6.2
# Certifique-se de ter URP instalado

# Instale packages necessários:
# - Cinemachine
# - Input System
# - TextMeshPro
# - 2D Sprite
```

**Para Artistas:**

- Configure software de pixel art (Aseprite, Photoshop, etc.)
- Resolução base: 320x180 (upscaled para 1920x1080)
- Sprites: 16x16px base (varia por criatura)
- Paleta: 64 cores por bioma

**Para Sound Designers:**

- Configure DAW preferida
- Formatos: WAV (SFX), OGG Vorbis (música)
- Áudio posicional 2D

### 3. Explore o Projeto

**Estrutura de Pastas:**

```
Assets/
├── Scripts/          # Código C#
├── Prefabs/          # Prefabs reutilizáveis
├── ScriptableObjects/ # Dados (Quests, AI, Items)
├── Sprites/          # Arte 2D
├── Audio/            # Música e SFX
├── Scenes/           # Cenas Unity
└── Docs/             # Esta documentação
```

---

## 🎮 Conceitos Principais

### O Slime Protagonista

**Slime Branco Raro:**

- Único slime capaz de absorver todos os elementos
- Começa pequeno (16x16px) e cresce até 56x56px
- Ganha aura visual conforme conquista reconhecimento
- Não busca ativamente ser rei — isso acontece naturalmente

### Os 10 Reis Monstros

Não são vilões, mas figuras respeitadas que governam domínios:

1. **Rainha Melífera** (Nature) - Perfeccionista matemática
2. **Imperador Escavarrok** (Earth) - Filósofo paciente
3. **Imperatriz Nictófila** (Ice) - Misteriosa e poética
4. **Sultan Escamífero** (Fire) - Competitivo e veloz
5. **Rainha Formicida** (Shadow) - Mente coletiva
6. **Duquesa Solibrida** (Dark) - Mestra das ilusões
7. **Príncipe Fulgorante** (Air) - Regente elétrico
8. **Conde Castoro** (Earth/Water) - Construtor comunitário
9. **Matriarca Flores** (Nature Growth) - Guardiã gentil
10. **Luminescente** (All Elements) - Guardião cristalino

### Mecânicas Únicas

**Agachar (Stealth):**

- Pressionar B/Circle/B/Ctrl
- Torna-se indetectável atrás de objetos
- Velocidade reduzida para 40%
- Abre possibilidades para puzzles

**Sistema de IA:**

- 18 estados diferentes (10 inimigos, 8 aliados)
- Percepção: visão, audição, proximidade
- Comportamentos únicos por criatura

**Progressão Orgânica:**

- Reputação invisível (não exibida)
- Evolução baseada em ações significativas
- Reconhecimento vem naturalmente

---

## 🛠️ Ferramentas e Tecnologias

### Engine e Linguagens

- **Unity 6.2** com **URP** (Universal Render Pipeline)
- **C#** para programação
- **Git** para controle de versão

### Packages Unity

- Cinemachine (câmera)
- Input System (controles)
- TextMeshPro (UI)
- 2D Sprite (sprites e animação)

### Ferramentas Recomendadas

- **Código:** Visual Studio / Rider
- **Arte:** Aseprite / Photoshop
- **Som:** Reaper / Audacity
- **Versionamento:** Git / GitHub Desktop

---

## 📅 Onde Estamos?

### Status Atual: **Pré-Alpha**

**Fase Atual:** Documentação completa (v8.0) ✅

**Próxima Fase:** Alpha (Q4 2025)

- Sistema de movimentação completo
- IA básica (4 estados)
- 2 biomas jogáveis
- 2 Reis Monstros
- 5 puzzles
- Sistema de quests básico

**Timeline Geral:**

- **Alpha:** Q4 2025 (6 meses)
- **Beta:** Q1 2026 (3 meses)
- **Gold:** Q3 2026 (6 meses)
- **Lançamento:** Q4 2026

---

## 🤝 Como Contribuir

### Workflow de Desenvolvimento

1. **Pegue uma Task**
   - Verifique o board de tarefas
   - Atribua a task para você
   - Mova para "In Progress"

2. **Desenvolva**
   - Crie branch: `feature/nome-da-feature`
   - Desenvolva seguindo padrões do projeto
   - Teste localmente

3. **Commit e Push**

   ```bash
   git add .
   git commit -m "feat: descrição clara da mudança"
   git push origin feature/nome-da-feature
   ```

4. **Pull Request**
   - Crie PR para `develop`
   - Descreva mudanças claramente
   - Aguarde code review

5. **Code Review**
   - Responda comentários
   - Faça ajustes necessários
   - Merge após aprovação

### Padrões de Código

**Nomenclatura:**

```csharp
// Classes: PascalCase
public class PlayerController { }

// Métodos: PascalCase
public void MovePlayer() { }

// Variáveis privadas: camelCase
private float moveSpeed;

// Variáveis públicas: PascalCase
public float MoveSpeed;

// Constantes: UPPER_SNAKE_CASE
private const int MAX_HEALTH = 100;
```

**Comentários:**

```csharp
// Comentários simples para lógica complexa
// Evite comentários óbvios

/// <summary>
/// XML comments para métodos públicos
/// </summary>
public void PublicMethod() { }
```

---

## 💬 Comunicação

### Canais

- **Discord:** Canal principal de comunicação
- **Trello/Jira:** Gerenciamento de tarefas
- **GitHub:** Code reviews e issues
- **Reuniões:** Semanais (segundas 10h)

### Dúvidas?

- **Design:** Pergunte ao líder de design
- **Código:** Pergunte ao líder técnico
- **Arte:** Pergunte ao diretor de arte
- **Geral:** Canal #general no Discord

---

## 📖 Recursos Adicionais

### Documentação

- [GDD Completo](The-Slime-King-GDD-v8.md)
- [Referência Rápida](Quick-Reference.md)
- [Guia de Implementação](Implementation-Guide.md)
- [Changelog](CHANGELOG.md)

### Inspirações

- **Stardew Valley** - Ritmo relaxante
- **Spiritfarer** - Atmosfera emocional
- **A Short Hike** - Exploração contemplativa
- **Slime Rancher** - Mecânicas de slime

### Tutoriais Unity

- [Unity Learn](https://learn.unity.com/)
- [Brackeys YouTube](https://www.youtube.com/user/Brackeys)
- [Code Monkey YouTube](https://www.youtube.com/c/CodeMonkeyUnity)

---

## ✅ Checklist de Onboarding

- [ ] Li GameDesign.md
- [ ] Li GDD-v8-Summary.md
- [ ] Li INDEX.md
- [ ] Configurei ambiente de desenvolvimento
- [ ] Clonei repositório
- [ ] Abri projeto no Unity
- [ ] Entrei no Discord
- [ ] Me apresentei para a equipe
- [ ] Peguei minha primeira task
- [ ] Li documentação específica da minha função
- [ ] Fiz meu primeiro commit

---

## 🎉 Bem-vindo à Equipe

Estamos animados para ter você conosco! The Slime King é um projeto ambicioso e sua contribuição será fundamental para o sucesso do jogo.

**Lembre-se:**

- Não tenha medo de fazer perguntas
- Colaboração é essencial
- Divirta-se no processo!

**Vamos criar algo incrível juntos! 🎮✨**

---

**Dúvidas?** Entre em contato com [Líder do Projeto]

**Última Atualização:** 2025  
**Versão:** 1.0
