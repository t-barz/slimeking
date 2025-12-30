# The Slime King - Game Design Document

## 1. Visão Geral do Jogo

**Título:** The Slime King  
**Gênero:** RPG de Exploração com Puzzle  
**Estilo:** Cozy, ritmo relaxado, poucos combates  
**Plataforma:** PC  
**Público-Alvo:** [A PREENCHER]

---

## 2. Conceito Principal

Um jogo de RPG de exploração onde o jogador controla uma criatura limo (Slime) que evolui através da absorção de fragmentos elementais. O foco é exploração, puzzle-solving e progressão gradual em um mundo acessível e agradável.

---

## 3. Progressão do Personagem

### 3.1 Evolução do Personagem

- **Formas:** Filhote → Jovem → Adulto → Rei
- **Mecanismo:** Baseado em XP acumulada (não há níveis explícitos)
- **XP:** Ganho através da absorção de fragmentos elementais
- **Vidas:**
  - Inicial: 3 vidas
  - Máximo: 10 vidas
  - Aumenta conforme evolução do personagem
  - Vidas perdidas podem ser recuperadas com itens ou habilidades
- **Limite de Seguidores:**
  - Filhote: 0
  - Jovem: 1
  - Adulto: 2
  - Rei: 4

### 3.2 Elementais

Seis tipos de fragmentos elementais:

- Água
- Fogo
- Natureza
- Terra
- Ar
- Escuridão

**Mecânica:** Fragmentos são absorvidos automaticamente ao entrar em contato com o personagem.

---

## 4. Árvore de Habilidades

### 4.1 Sistema de Compra

- **Custo:** Fragmentos elementais específicos
- **Exemplo:** Ataque de fogo (10x Fogo) | Habilidade de gelo (10x Água + 5x Ar)
- **Desbloqueio:** [A PREENCHER]

---

## 5. Inventário e Itens

### 5.1 Capacidade

- **Total:** 12 slots (3 linhas × 4 colunas)
- **Empilhamento:** Itens não são empilháveis - cada um ocupa 1 slot

### 5.2 Tipos de Itens

1. **Consumíveis:** Recuperam vida ou aplicam buffs
2. **Materiais:** Usados em construção ou para ativar habilidades
3. **Itens de Missão:** Necessários para completar quests

### 5.3 Coleta

- **Fragmentos:** Absorvidos automaticamente
- **Demais itens:** Recolhidos via botão de interação (próximo ao personagem)

---

## 6. Sistema de Slots Rápidos

- **Função:** Consumir/ativar itens e habilidades sem abrir painéis
- **Acesso:** Botões do controle
- **Restrição:** Apenas itens/habilidades marcados como "aptos para slot rápido"
- **Quantidade de slots:** 4 slots

---

## 7. Mecânicas de Movimento e Ação

### 7.1 Ações do Personagem

- Mover
- Atacar
- Abaixar (esconder)
- Interagir (objetos, NPCs, pontos especiais)
- Usar itens

### 7.2 Mecanismo de Esconderijo

- **Ativação:** Abaixar estando atrás de um objeto
- **Efeito:** Personagem fica imóvel
- **Invisibilidade:** NPCs não detectam o personagem se permanecer escondido
- **Uso:** Evasão, puzzle-solving, stealth

### 7.3 Pontos de Interação do Mapa

1. **Estreitamento:** Personagem se esgueira automaticamente de um ponto a outro
2. **Ponto de Pulo:** Personagem pula automaticamente para outro ponto
3. **Objetos Móveis:** Podem ser deslocados ao interagir (ex: pedra)
4. **[A PREENCHER]**

**Nota:** Movimentação em pontos especiais é automática - controle devolvido ao final da animação.

---

## 8. Combate e Destruição de Objetos

### 8.1 Objetos Destrutíveis

- **Resistência:** Cada objeto possui um nível de resistência
- **Ataque Necessário:** Personagem precisa de poder de ataque suficiente
- **Drops:** Itens e fragmentos elementais ao destruir
- **Reações:** Alguns objetos reagem ao contato (ex: arbustos balançam)

### 8.2 Frequência de Combate

Baixa - o jogo prioriza exploração e puzzle-solving.

---

## 9. Dia e Noite

- **Ciclo:** Cada hora no jogo = 10 minutos no mundo real
- **Impacto:** [A PREENCHER - mecânicas afetadas]
- **Detalhes técnicos:** [A PREENCHER]

---

## 10. Sistema de Clima e Estações

### 10.1 Clima

- **Mecânica:** Sorteado a cada dia de jogo
- **Exemplos:** Ensolarado, nublado, chuva leve, tempestade, nevoeiro, neve
- **Duração:** [A PREENCHER]

### 10.2 Estações

- **Ciclo:** Muda a cada 7 dias do mundo real
- **Estações:** Primavera, Verão, Outono, Inverno
- **Restrições:** Alguns climas só ocorrem em estações específicas
  - Exemplo: Neve apenas no Inverno

---

## 11. NPCs e Fações

### 11.1 Tipos de NPCs

**Alinhamento:**

- Aliado
- Neutro
- Inimigo

**Comportamento:**

- **Agressivo:** Sempre ataca inimigos ao detectá-los
- **Reativo:** Ataca apenas se for atacado
- **Passivo:** Sempre tenta fugir de inimigos

### 11.2 Relações com o Personagem

- **Mudança:** Através de quests ou ações do jogador
- **Exemplo:** Atacar colmeia de abelhas as torna inimigas
- **[A PREENCHER]:** Outras ações que afetam relações

### 11.3 Seguidores

- **Ativação:** Falar com aliado para pedir que seja seguidor
- **Aceitação:** Nem todos aceitam ser seguidores
- **Limite:** Respeitado conforme forma do personagem
- **Ajuda:** Participam em combates, quests e puzzles

---

## 12. Sistema de Quests

### 12.1 Tipos de Quests

1. **Coleta e Entrega:** Reunir itens específicos
2. **Destruição:** Destruir objetos ou NPCs específicos
3. **Escolta:** Acompanhar NPC até um ponto
4. **Exploração:** Chegar até um ponto específico do mapa
5. **[A PREENCHER]**

### 12.2 Recompensas

**[A PREENCHER]**

---

## 13. Mundo do Jogo

### 13.1 Estrutura do Mapa

- **Composição:** Múltiplos cenários conectados
- **Navegação:** Personagem pode transitar entre áreas
- **Exemplos de Áreas:** Montanha, Caverna, Lago, Praia, [A PREENCHER]

### 13.2 Carregamento de Cenas

- **Sistema:** Cada área é uma Scene na Unity
- **Conexão:** Mecanismo que carrega nova cena e posiciona personagem em ponto específico
- **[A PREENCHER]:** Detalhos técnicos de carregamento

### 13.3 Objetos Reativos

- **Animações:** Arbustos balançam ao contato
- **Interatividade:** Reações ao jogador ou NPCs passar próximo
- **[A PREENCHER]:** Mais exemplos

---

## 14. Arte e Apresentação

### 14.1 Estilo Visual

**[A PREENCHER]**

### 14.2 Câmera

**[A PREENCHER]**

### 14.3 Animações

**[A PREENCHER]**

---

## 15. Áudio

### 15.1 Música

**[A PREENCHER]**

### 15.2 Efeitos Sonoros

**[A PREENCHER]**

---

## 16. Interface (UI)

### 16.1 Menu Principal

**[A PREENCHER]**

### 16.2 Tela de Jogo

#### 16.2.1 HUD Principal

- **Contador de Vidas:** Exibido no canto superior esquerdo
  - Representação: Corações usando HeartPrefab
  - Vida cheia: ui_hearthCounterOK.png
  - Vida perdida (recuperável): ui_hearthCounterNOK.png
  - Inicial: 3 vidas
  - Máximo: 10 vidas
  - Aumenta conforme evolução do personagem
- Inventário (12 slots)
- Árvore de Habilidades
- Status do Personagem
- Slots Rápidos (4 slots)
- Mapa
- **[A PREENCHER]**

### 16.3 Opções e Configurações

**[A PREENCHER]**

---

## 17. Controles

### 17.1 Teclado e Gamepad

**[A PREENCHER - mapeamento específico]**

**Nota:** Jogo suporta APENAS Teclado e Gamepad. Mouse e Touch NÃO são suportados.

---

## 18. Dificuldade e Acessibilidade

### 18.1 Modos de Dificuldade

**[A PREENCHER]**

### 18.2 Acessibilidade

**[A PREENCHER]**

---

## 19. Padrões de Código

### 19.1 Convenções de Nomenclatura

#### Namespace

- **Padrão:** `TheSlimeKing.{Categoria}`
- **Exemplos:** `TheSlimeKing.UI`, `TheSlimeKing.Gameplay`, `TheSlimeKing.Managers`

#### Classes

- **PascalCase** para todos os nomes de classes
- **Sufixos específicos por categoria:**
  - **Managers/Controllers:** Suffix `Manager` ou `Controller`
    - Exemplo: `GameManager`, `TitleScreenController`, `InitialCaveController`
  - **UI:** Sem sufixo específico, nome descritivo
    - Exemplo: `HealthDisplay`, `InventoryPanel`, `QuestLog`
  - **Gameplay:** Sem sufixo específico, nome descritivo
    - Exemplo: `PlayerMovement`, `ElementalFragment`, `DestructibleObject`
  - **Environments:** Suffix `Ambience` ou nome descritivo
    - Exemplo: `CaveAmbience`, `WeatherSystem`, `DayNightCycle`
  - **NPCs:** Suffix específico do comportamento
    - Exemplo: `NPCBehavior`, `FollowerAI`, `EnemyController`
  - **Items:** Sem sufixo, nome do tipo
    - Exemplo: `Consumable`, `Material`, `QuestItem`

#### Variáveis

- **camelCase** para campos privados
- **PascalCase** para propriedades públicas
- **Prefixo `_`** opcional para campos privados (não usado no projeto atual)
- **SerializeField** sempre privado

#### Métodos

- **PascalCase** para todos os métodos
- **Nomes descritivos** que indicam ação clara
- **Exemplos:** `InitializeScreen()`, `UpdateDisplay()`, `SetCurrentHearts()`

#### Constantes e Enums

- **PascalCase** para enums e seus valores
- **UPPER_SNAKE_CASE** para constantes (quando necessário)

### 19.2 Organização de Arquivos

```
Assets/_Code/
├── Environments/     # Ambientação, clima, dia/noite
├── Gameplay/         # Mecânicas de jogo, player, fragmentos
├── Managers/         # Controllers de cena, game manager
├── NPCs/            # IA e comportamento de NPCs
├── UI/              # Interface e HUD
└── Items/           # Sistema de itens (consumíveis, materiais, etc)
```

### 19.3 Estrutura de Classes

Todas as classes MonoBehaviour devem seguir esta estrutura:

```csharp
#region Settings / Configuration
[Header("...")]
[SerializeField] private ...
#endregion

#region References
// Referências para outros componentes
#endregion

#region Debug
[Header("Debug")]
[SerializeField] private bool enableDebugLogs = true;
#endregion

#region Private Variables
// Variáveis privadas não serializadas
#endregion

#region Unity Lifecycle
private void Awake() { }
private void Start() { }
private void Update() { }
// etc
#endregion

#region Initialization
// Métodos de inicialização
#endregion

#region Public Methods
// API pública da classe
#endregion

#region Private Methods
// Métodos privados auxiliares
#endregion

#region Utilities
// Métodos utilitários (Log, etc)
#endregion
```

### 19.4 Comentários e Documentação

- **Summary XML** obrigatório em classes públicas e métodos públicos
- **Comentários inline** quando a lógica não é óbvia
- **Evitar** comentários óbvios que apenas repetem o código
- **Usar** comentários para explicar "por quê", não "o quê"

---

## 20. Histórico de Desenvolvimentos

**[A PREENCHER]**

---

## 21. Próximas Fases

- [ ] Definir estilo visual
- [ ] Detalhar mapeamento de controles
- [ ] Descrever mecanismos exatos de IA dos NPCs
- [ ] Definir balanceamento de custos de habilidades
- [ ] Criar mapas conceituais do mundo
- [ ] Escrever narrativa e diálogos
- [ ] **[A PREENCHER]**

---

**Última Atualização:** 15 de Dezembro de 2025  
**Versão:** 0.2 (Padrões de Código Definidos)
