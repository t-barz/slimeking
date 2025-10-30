# The Slime King – Game Design Document v9.0

## 📋 Índice

1. [Conceito e Visão Geral](#1-conceito-e-visão-geral)
2. [Gameplay e Mecânicas Principais](#2-gameplay-e-mecânicas-principais)
3. [Os Dez Reis Monstros](#3-os-dez-reis-monstros)
4. [Mundo do Jogo e Ambientação](#4-mundo-do-jogo-e-ambientação)
5. [Sistemas Temporais e Climáticos](#5-sistemas-temporais-e-climáticos)
6. [Sistema de Construção e Expansão do Lar](#6-sistema-de-construção-e-expansão-do-lar)
7. [Sistema de Inventário](#7-sistema-de-inventário)
8. [Sistema de Diálogo](#8-sistema-de-diálogo)
9. [Sistema de Árvore de Habilidades](#9-sistema-de-árvore-de-habilidades)
10. [Sistema de Save/Load](#10-sistema-de-saveload)
11. [Sistema de IA e Comportamento](#11-sistema-de-ia-e-comportamento)
12. [Sistema de Quests](#12-sistema-de-quests)
13. [Sistema de Cutscenes](#13-sistema-de-cutscenes)
14. [Sistema de Puzzles](#14-sistema-de-puzzles)
15. [Direção Visual e Sonora](#15-direção-visual-e-sonora)
16. [Controles e Interface](#16-controles-e-interface)
17. [Sistemas Técnicos](#17-sistemas-técnicos)
18. [Performance e Otimização](#18-performance-e-otimização)
19. [Métricas e Analytics](#19-métricas-e-analytics)
20. [Conclusão](#20-conclusão)

---

## 1. Conceito e Visão Geral

### 1.1 Conceito Central

**The Slime King** é um RPG de aventura 2D top-down em pixel art de mundo aberto focado em exploração orgânica, puzzles criativos e progressão natural através de interações significativas com o mundo e seus habitantes.

O jogador controla um slime branco recém-nascido que desperta em uma caverna nas Montanhas Cristalinas de Aethros. Diferente de outros slimes, este possui a rara capacidade de absorver e canalizar essências elementais. Sua jornada não é uma busca deliberada por poder ou título, mas sim uma série de experiências, descobertas e conexões que naturalmente o levam a crescer em influência e respeito.

**Pilares de Design:**

- **Exploração Orgânica:** Sem objetivos forçados, o jogador descobre o mundo no seu próprio ritmo
- **Progressão Natural:** Poder e reconhecimento vêm através de ações significativas, não de grinding
- **Atmosfera Cozy:** Sem pressão de tempo, foco em contemplação e descoberta
- **Interações Significativas:** Cada NPC, puzzle e desafio contribui para a narrativa pessoal do slime
- **Stealth Criativo:** Mecânica de agachar (parado) permite abordagens não-violentas e puzzles únicos
- **Progressão Livre:** Jogador escolhe livremente qual Rei Monstro enfrentar e em que ordem

### 1.2 Premissa Narrativa

O slime desperta em seu ninho numa caverna escura, sem memórias ou propósito claro. As Montanhas Cristalinas de Aethros são governadas por **dez poderosos Reis Monstros** — criaturas lendárias que conquistaram seus títulos através de feitos extraordinários e reconhecimento dos Espíritos Elementais.

Os slimes sempre foram vistos como criaturas simples, habitantes de cavernas sem ambições. Ninguém jamais imaginou que um slime pudesse aspirar a algo maior. E o protagonista não aspira — pelo menos não inicialmente.

**A jornada começa com curiosidade simples:**

- Explorar a caverna natal
- Encontrar comida e recursos
- Conhecer outras criaturas
- Resolver pequenos problemas

**Mas cada ação tem consequências:**

- Ajudar um NPC em dificuldade gera gratidão e histórias
- Resolver um puzzle antigo desperta a atenção de Espíritos Elementais
- Vencer um desafio imposto por um Rei Monstro demonstra capacidade inesperada
- Desenvolver o lar atrai visitantes e admiradores

**O reconhecimento vem naturalmente:**
Conforme o slime interage com o mundo, sua reputação cresce organicamente. NPCs começam a falar sobre "aquele slime diferente". Espíritos Elementais observam com interesse crescente. Reis Monstros ouvem rumores e decidem testar pessoalmente esta criatura incomum.

**Rituais de Reconhecimento não são buscados, são oferecidos:**
Quando um Rei Monstro reconhece o valor do slime, ele oferece um **Ritual de Reconhecimento** — não como teste de entrada, mas como reconhecimento de feitos já realizados. O slime não precisa provar nada; ele já provou através de suas ações no mundo.

**O título de Rei Slime emerge naturalmente:**
Não há momento em que o slime decide "vou me tornar rei". Em vez disso, após acumular reconhecimento suficiente, influência genuína e respeito de múltiplos Reis Monstros, os próprios Espíritos Elementais manifestam-se e declaram: "Você já é um rei. Apenas não sabia ainda."

### 1.3 Experiência de Jogo

**Atmosfera:**

- Cozy e contemplativa, sem timers ou pressão
- Mundo vivo que reage às ações do jogador
- Sensação de descoberta constante
- Progressão satisfatória e visível

**Gameplay Core:**

- Exploração livre de 7 biomas distintos
- Puzzles ambientais criativos que respeitam a lore
- Combate opcional com mecânicas de stealth
- Interações profundas com NPCs únicos
- Desenvolvimento orgânico do lar
- Sistema de habilidades elementais progressivo

**Diferencial:**
Ao contrário de RPGs tradicionais onde o jogador persegue objetivos claros, The Slime King permite que a história emerja das escolhas e interações do jogador. Não há "quest principal" linear — apenas um mundo rico esperando para ser explorado e influenciado. O jogador tem total liberdade para escolher qual Rei Monstro enfrentar e em que ordem.

---

## 2. Gameplay e Mecânicas Principais

### 2.1 Loop Principal de Gameplay

**Ciclo de Exploração e Crescimento:**

1. **Explorar** → Descobrir novos biomas, NPCs, puzzles e segredos
2. **Interagir** → Ajudar NPCs, resolver problemas, completar desafios
3. **Absorver** → Coletar Cristais Elementais e essências
4. **Evoluir** → Desbloquear habilidades e crescer em tamanho/poder
5. **Desenvolver** → Expandir o lar e atrair visitantes
6. **Reconhecer** → Receber reconhecimento de Reis Monstros
7. **Repetir** → Novas áreas e possibilidades se abrem

**Progressão Livre:**

- Jogador pode explorar biomas em qualquer ordem (respeitando barreiras naturais)
- Reis Monstros podem ser encontrados em sequências diferentes
- Não há ordem "correta" - cada jogador cria sua própria jornada
- Puzzles podem ser resolvidos quando o jogador tiver as habilidades necessárias
- Desenvolvimento do lar acontece conforme recursos e amizades são conquistados

### 2.2 Mecânicas de Movimentação

#### 2.2.1 Movimentação Básica

**Controles:**

- **Analógico Esquerdo / WASD:** Movimento em 8 direções
- **Velocidade Base:** 3.5 unidades/segundo (Filhote)
- **Aceleração:** 0.2 segundos para velocidade máxima
- **Desaceleração:** 0.15 segundos para parar completamente

**Características do Slime:**

- Movimento fluido e gelatinoso (animação de "bounce")
- Deixa rastro sutil de gosma que desaparece após 2 segundos
- Pode se espremer por espaços apertados (1 tile de largura)
- Não pode pular, mas pode escalar superfícies inclinadas suaves

#### 2.2.2 Mecânica de Agachar (NOVA)

**IMPORTANTE: SLIME FICA COMPLETAMENTE PARADO QUANDO AGACHADO**

**Ativação:**

- **Botão:** Pressionar e segurar B/Circle/B/Ctrl
- **Transição:** 0.3 segundos para agachar completamente
- **Visual:** Slime achata verticalmente, aumenta área horizontal

**Efeitos Mecânicos:**

**Stealth:**

- Slime agachado atrás de objetos torna-se **indetectável** para a maioria dos inimigos
- Objetos válidos para cobertura: rochas, arbustos, colunas, móveis
- Sistema de linha de visão: se não há linha direta entre inimigo e slime, stealth ativo
- Indicador visual: Ícone de olho fechado aparece quando indetectável

**Restrições Importantes:**

- **SLIME FICA COMPLETAMENTE PARADO** - Não pode se mover enquanto agachado
- Não pode usar habilidades elementais enquanto agachado
- Pode interagir com objetos baixos inacessíveis normalmente
- Deve soltar o botão para voltar a se mover

**Detecção:**

- Inimigos com sentidos aguçados (marcados com ícone de nariz) podem detectar por proximidade
- Sair de cobertura = detecção imediata se inimigo estiver olhando

**Aplicações em Puzzles:**

- Esconder-se de guardas em patrulha (timing é crucial - esperar passar)
- Acessar túneis baixos e passagens secretas
- Ativar placas de pressão que requerem forma achatada
- Observar padrões de patrulha de inimigos sem ser visto
- Evitar armadilhas ativadas por altura

**Aplicações em Exploração:**

- Descobrir áreas secretas acessíveis apenas agachado
- Evitar confrontos esperando inimigos passarem
- Observar criaturas tímidas sem assustá-las
- Aguardar momento certo para coletar recursos em áreas perigosas

**Código Exemplo:**

```csharp
void Update()
{
    isCrouching = Input.GetButton("Crouch");
    
    if (isCrouching)
    {
        // Slime fica parado
        rb.velocity = Vector2.zero;
        // Verifica se está atrás de cobertura
        isHidden = CheckCoverBehind();
    }
    else
    {
        // Movimento normal
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rb.velocity = moveInput.normalized * moveSpeed;
    }
}
```

### 2.3 Sistema de Evolução Orgânica

A evolução não é baseada em "conquistar X auras", mas sim em **acúmulo natural de experiências e reconhecimento**.

#### 2.3.1 Estágios Evolutivos

**Filhote (Estado Inicial)**

- **Tamanho:** Pequeno (sprite base 16x16 pixels)
- **Habilidades:** Movimento, ataque corpo-a-corpo básico, agachar
- **Capacidades:** Nenhum seguidor, sem habilidades elementais
- **Aura:** Nenhuma
- **Duração Típica:** 2-4 horas de gameplay

**Como Evoluir para Adulto:**

- Completar pelo menos 5 quests de NPCs
- Resolver 3 puzzles ambientais
- Receber primeiro Ritual de Reconhecimento de qualquer Rei Monstro
- Desenvolver pelo menos 1 expansão do lar

**Adulto (Primeira Evolução)**

- **Tamanho:** Médio (sprite 24x24 pixels, 1.5x maior)
- **Habilidades:** Desbloqueia 4 slots de habilidades elementais
- **Capacidades:** Pode ter 1 seguidor aliado
- **Aura:** Brilho monocromático sutil (~1.2x do slime)
- **Duração Típica:** 8-12 horas de gameplay

**Como Evoluir para Grande Slime:**

- Receber reconhecimento de pelo menos 3 Reis Monstros (qualquer ordem)
- Completar 15 quests de NPCs
- Resolver 8 puzzles ambientais
- Desenvolver pelo menos 3 expansões do lar
- Alcançar nível 5+ de amizade com 10 espécies diferentes

**Grande Slime (Segunda Evolução)**

- **Tamanho:** Grande (sprite 32x32 pixels, 2x maior)
- **Habilidades:** Acesso a habilidades elementais avançadas e combinações
- **Capacidades:** Pode ter até 3 seguidores aliados
- **Aura:** Padrão visual complexo com 2-3 cores (~1.5x do slime), partículas leves
- **Duração Típica:** 15-25 horas de gameplay

**Como Evoluir para Rei Slime:**

- Receber reconhecimento de pelo menos 5 Reis Monstros (qualquer combinação, qualquer ordem)
- Completar 30+ quests de NPCs
- Resolver 15+ puzzles ambientais
- Desenvolver todas as 4 expansões do lar
- Alcançar nível 5+ de amizade com 20+ espécies diferentes

**Rei Slime (Evolução Final - 5 Reconhecimentos)**

- **Tamanho:** Muito Grande (sprite 40x40 pixels, 2.5x maior)
- **Habilidades:** Maestria completa dos elementos conquistados
- **Capacidades:** Pode ter até 5 seguidores aliados
- **Aura:** Majestosa e multicolorida (~2x do slime), pulsante
- **Efeito:** Radiância constante, trilha de luz ao mover
- **Reconhecimento:** Cerimônia de Coroação

**Rei Slime Transcendente (Evolução Máxima - 10 Reconhecimentos - Opcional)**

- **Tamanho:** Máximo (sprite 56x56 pixels, 3.5x maior)
- **Habilidades:** Transcendência elemental completa
- **Capacidades:** Pode ter até 10 seguidores aliados
- **Aura:** Forma coroa flutuante etérea de cristal multicolorido (~3x do slime)
- **Efeito:** Luz transcendental, cristais dançam ao redor, presença majestosa
- **Reconhecimento:** Grande Cerimônia de Coroação (para completistas)

#### 2.3.2 Sistema de Reconhecimento Progressivo

**Reputação Invisível:**
O jogo rastreia internamente um sistema de "reputação" que não é exibido numericamente ao jogador, mas influencia como o mundo reage:

**Níveis de Reputação:**

1. **Desconhecido (0-100 pontos):** NPCs tratam slime como criatura comum
2. **Notado (101-300 pontos):** Alguns NPCs comentam sobre "aquele slime diferente"
3. **Respeitado (301-600 pontos):** NPCs procuram ativamente o slime para ajuda
4. **Influente (601-1000 pontos):** Reis Monstros começam a ouvir sobre o slime
5. **Lendário (1001+ pontos):** Reconhecimento universal, status de realeza

**Como Ganhar Reputação:**

- Completar quest de NPC: +10-50 pontos (dependendo da complexidade)
- Resolver puzzle ambiental: +15 pontos
- Vencer desafio de Rei Monstro: +100 pontos
- Receber Ritual de Reconhecimento: +200 pontos
- Desenvolver expansão do lar: +30 pontos
- Alcançar novo nível de amizade com espécie: +20 pontos
- Descobrir área secreta: +25 pontos
- Derrotar inimigo elite: +40 pontos

### 2.4 Sistema de Aura Elemental

Cada Ritual de Reconhecimento marca permanentemente o slime com uma **Aura Elemental** que se sobrepõe às anteriores.

#### 2.4.1 Progressão Visual de Aura

**1ª Aura (Após Primeiro Reconhecimento):**

- Brilho monocromático sutil
- Cor baseada no elemento do Rei Monstro
- Tamanho: ~1.2x do slime
- Sem padrão especial, apenas luz suave
- Som: Leve zumbido elemental ao mover

**3ª Aura (Após Terceiro Reconhecimento):**

- Padrão visual simples começa a aparecer
- Duas ou três cores começam a se sobrepor (blend harmônico)
- Tamanho: ~1.5x do slime
- Partículas leves começam a circular
- Som: Harmonia de dois tons ao usar habilidades

**5ª Aura (Após Quinto Reconhecimento - Rei Slime):**

- Padrão complexo (hexágonos, ondas, ou cristais flutuando)
- Múltiplas cores harmoniosas dançando
- Tamanho: ~2x do slime
- Aura pulsante com ritmo constante
- Som: Acorde completo de cristal ao mover
- **Efeito Especial:** Coroa etérea começa a se formar

**10ª Aura (Após Décimo Reconhecimento - Rei Slime Transcendente - Opcional):**

- Aura transcendental impossível de ignorar
- Padrão forma **coroa flutuante completa** de cristal multicolorido
- Tamanho: ~3x do slime
- Luz constante e brilhante, cristais fluem como constelação
- Som: Harmonia elemental constante e majestosa
- **Efeito Especial:** Ambiente ao redor reage à presença (flores brilham, água reflete cores)

#### 2.4.2 Tabela de Cores de Aura por Elemento

| Elemento | Rei Monstro | Cor Primária | Cor Secundária | Padrão Visual |
|:--|:--|:--|:--|:--|
| Nature | Rainha Melífera | Verde vibrante | Dourado | Hexágonos geométricos |
| Water | Imperador Escavarrok | Azul cristalino | Prateado | Reflexos de espelho, ondas |
| Ice | Imperatriz Nictófila | Púrpura-Azul | Branco gelo | Padrão de constelação |
| Fire | Sultan Escamífero | Vermelho-Laranja | Dourado | Forma dinâmica (sempre mudando) |
| Shadow | Rainha Formicida | Verde-Roxo | Preto | Pontos sincronizados |
| Dark | Duquesa Solibrida | Preto-Roxo | Roxo profundo | Absorve e reflete luz |
| Air | Príncipe Fulgorante | Amarelo-Branco | Azul claro | Arcos de energia |
| Earth | Conde Castoro | Marrom-Verde | Ouro | Textura orgânica |
| Nature Growth | Matriarca Flores | Rosa-Verde | Branco | Pétalas flutuantes |
| All Elements | Luminescente | Multicolorido | Branco puro | Coroa flutuante completa |

### 2.5 Sistema de Cristais

#### 2.5.1 Cristais Elementais (Moeda de Habilidades)

**Função:** Moeda para desbloquear habilidades na Árvore de Habilidades

**Tipos:**

- Cristal Verde (Nature)
- Cristal Marrom (Earth)
- Cristal Branco (Air)
- Cristal Azul (Water)
- Cristal Vermelho (Fire)
- Cristal Roxo (Shadow)
- Cristal Ciano (Ice)

**Como Obter:**

- Resolver puzzles ambientais: 3-10 cristais
- Completar quests de NPCs: 5-15 cristais
- Derrotar inimigos: 1-3 cristais (drop aleatório)
- Encontrar em baús escondidos: 10-25 cristais
- Coletar em nodos de cristal no mundo: 1 cristal (respawn diário)
- Recompensa de Ritual de Reconhecimento: 50 cristais do elemento correspondente

**Armazenamento:**

- Não ocupam espaço no inventário
- Exibidos como contador na UI (ex: "Cristais Verdes: 47")
- Podem ser gastos na Árvore de Habilidades a qualquer momento

#### 2.5.2 Cristais de Pacto (Colecionáveis Únicos)

**Função:** Objetos simbólicos que representam reconhecimento de Reis Monstros

**Características:**

- Únicos e não-consumíveis
- Recebidos após completar Ritual de Reconhecimento
- Podem ser instalados na Câmara dos Pactos para buffs
- Cada cristal tem aparência e efeitos únicos

**Cristais de Pacto por Rei Monstro:**

| Rei Monstro | Cristal | Cor | Forma | Buff no Lar |
|:--|:--|:--|:--|:--|
| Rainha Melífera | Cristal da Harmonia | Dourado | Hexágono perfeito | +10% velocidade de coleta |
| Imperador Escavarrok | Cristal das Profundezas | Prateado | Espelho líquido | +5% Defesa permanente |
| Imperatriz Nictófila | Cristal Estelar | Roxo-Azul | Constelação | Visão noturna no lar |
| Sultan Escamífero | Cristal Acelerado | Vermelho-Laranja | Forma dinâmica | +8% velocidade de movimento |
| Rainha Formicida | Cristal Coletivo | Verde-Roxo | Múltiplos cristais | Aliados +5% dano |
| Duquesa Solibrida | Cristal Sombrio | Preto-Roxo | Forma abstrata | +3% evasão |
| Príncipe Fulgorante | Cristal Elétrico | Amarelo-Branco | Zigzag | +10% velocidade de ataque |
| Conde Castoro | Cristal Madeira | Marrom-Verde | Árvore | +15% velocidade construção |
| Matriarca Flores | Cristal Floral | Rosa-Verde | Flor | +2 HP regen/segundo |
| Luminescente | Cristal Unificado | Multicolorido | Coroa miniatura | +20% todos os buffs |

### 2.6 Sistema de Habilidades Elementais

#### 2.6.1 Slots de Habilidades

**Desbloqueio:** Ao evoluir para Adulto (após primeiro Ritual de Reconhecimento)

**4 Slots Mapeados:**

- **Slot 1 (ZL/L2/LT/Q):** Habilidade elemental primária
- **Slot 2 (L/L1/LB/E):** Habilidade elemental secundária
- **Slot 3 (R/R1/RB/R):** Habilidade elemental terciária
- **Slot 4 (ZR/R2/RT/F):** Habilidade elemental poderosa (ultimate)

**Customização:**

- Jogador pode equipar qualquer habilidade desbloqueada em qualquer slot
- Habilidades podem ser trocadas a qualquer momento (fora de combate)
- Cada habilidade tem cooldown individual
- Algumas habilidades têm sinergias quando usadas em sequência

#### 2.6.2 Árvore de Habilidades

**Estrutura:**

- 7 árvores elementais (Nature, Earth, Air, Water, Fire, Shadow, Ice)
- Cada árvore tem 3 tiers de habilidades
- Tier 1: Habilidades básicas (custo: 10-15 cristais)
- Tier 2: Habilidades intermediárias (custo: 25-35 cristais)
- Tier 3: Habilidades avançadas (custo: 50-75 cristais)

**Exemplos de Habilidades por Elemento:**

**Nature (Verde):**

- **Tier 1 - Crescimento Rápido:** Cria vinhas que prendem inimigos por 3s (Cooldown: 8s, Stamina: 15)
- **Tier 2 - Espinhos Defensivos:** Cria barreira de espinhos que reflete 30% do dano (Cooldown: 15s, Stamina: 20)
- **Tier 3 - Jardim Selvagem:** Transforma área em jardim que cura aliados e danifica inimigos (Cooldown: 30s, Stamina: 30)

**Water (Azul):**

- **Tier 1 - Jato d'Água:** Dispara jato que empurra inimigos (Cooldown: 5s, Stamina: 10)
- **Tier 2 - Escudo Aquático:** Cria bolha que absorve 50% do dano por 5s (Cooldown: 12s, Stamina: 20)
- **Tier 3 - Tsunami:** Onda massiva que atravessa tela inteira (Cooldown: 25s, Stamina: 30)

**Fire (Vermelho):**

- **Tier 1 - Bola de Fogo:** Projétil que causa dano em área (Cooldown: 6s, Stamina: 15)
- **Tier 2 - Trilha Flamejante:** Deixa rastro de fogo que persiste por 8s (Cooldown: 14s, Stamina: 20)
- **Tier 3 - Explosão Solar:** Explosão massiva centrada no slime (Cooldown: 35s, Stamina: 30)

**Shadow (Roxo):**

- **Tier 1 - Passo Sombrio:** Teleporte curto (5 unidades) (Cooldown: 7s, Stamina: 15)
- **Tier 2 - Camuflagem:** Torna-se invisível por 6s ou até atacar (Cooldown: 20s, Stamina: 25)
- **Tier 3 - Clone Sombrio:** Cria clone que atrai inimigos por 10s (Cooldown: 30s, Stamina: 30)

### 2.7 Sistema de Combate

**Filosofia:** Combate é opcional e pode ser evitado através de stealth, diplomacia ou puzzles alternativos.

#### 2.7.1 Mecânicas de Combate

**Ataque Corpo-a-Corpo:**

- **Botão:** A/X/A/Space
- **Dano Base:** 5 (Filhote) → 15 (Rei Slime)
- **Alcance:** 1.5 unidades
- **Cooldown:** 0.5 segundos
- **Animação:** Slime se estica e bate no inimigo

**Stamina:**

- **Total:** 100 pontos
- **Regeneração:** 10 pontos/segundo (fora de combate)
- **Uso:** Habilidades elementais (10-30 pontos por habilidade)

**Sistema de Dano:**

- Inimigos têm HP visível em barra acima da cabeça
- Dano exibido como números flutuantes
- Críticos (10% chance): 1.5x dano, número amarelo
- Resistências elementais: Alguns inimigos resistem a certos elementos

#### 2.7.2 Abordagens de Combate

**Agressiva:**

- Confronto direto usando habilidades elementais
- Maior risco, maior recompensa (mais drops)
- Adequado para jogadores que gostam de ação

**Stealth:**

- Usar agachar para evitar detecção
- Passar por inimigos sem confronto
- Menor risco, sem recompensas de combate
- Adequado para jogadores pacifistas

**Tática:**

- Usar ambiente a favor (empurrar inimigos em armadilhas)
- Atrair inimigos para áreas vantajosas
- Combinar habilidades para sinergias
- Adequado para jogadores estratégicos

**Diplomática:**

- Alguns inimigos podem ser pacificados com itens
- NPCs aliados podem intervir se amizade for alta
- Certos Cristais de Pacto intimidam inimigos fracos
- Adequado para jogadores sociais

### 2.8 Sistema de Seguidores

**Desbloqueio:** Ao evoluir para Adulto

**Capacidade:**

- Adulto: 1 seguidor
- Grande Slime: 3 seguidores
- Rei Slime (5 Reconhecimentos): 5 seguidores
- Rei Slime Transcendente (10 Reconhecimentos): 10 seguidores

**Como Recrutar:**

- Alcançar nível 5 de amizade com uma espécie
- Completar quest específica da espécie
- Convidar criatura para seguir (algumas aceitam, outras recusam baseado em personalidade)

**Comportamento de Seguidores:**

- Seguem o slime mantendo distância de 2-3 unidades
- Atacam inimigos que atacam o slime
- Podem ser comandados para aguardar em posição
- Têm HP próprio e podem ser derrotados (retornam ao lar após 1 dia)
- Ganham XP e ficam mais fortes com o tempo

**Tipos de Seguidores:**

- **Combatentes:** Focam em atacar inimigos (ex: Esquilo Coletor, Golem de Pedra)
- **Suporte:** Curam ou buffam o slime (ex: Borboleta Mineral, Rã-Eco)
- **Utilitários:** Ajudam em exploração (ex: Coruja-Cristal ilumina, Lontra Cristalina nada mais rápido)

---

## 3. Os Dez Reis Monstros

### 3.1 Conceito Geral

Os **Reis Monstros** não são antagonistas, mas figuras respeitadas que governam seus domínios com sabedoria. Cada um representa uma filosofia de liderança e maestria elemental. Eles não testam o slime por desconfiança, mas por curiosidade genuína — "Será que esta criatura realmente é tão especial quanto dizem?"

**Progressão Livre:**

- Jogador pode enfrentar os Reis em QUALQUER ORDEM
- Não há sequência "correta" ou obrigatória
- Cada Rei oferece desafio único independente da ordem
- Jogador decide quantos Reis enfrentar (mínimo 5 para Rei Slime, 10 para Transcendente)

**Estrutura de Encontro:**

1. **Rumores:** NPCs mencionam o Rei Monstro e suas lendas
2. **Descoberta:** Jogador encontra o domínio do Rei Monstro
3. **Observação:** Rei Monstro observa o slime de longe, avaliando
4. **Desafio:** Rei Monstro oferece desafio (não como teste de entrada, mas como reconhecimento de potencial)
5. **Ritual:** Se slime supera desafio, Rei Monstro oferece Ritual de Reconhecimento
6. **Pacto:** Slime recebe Aura Elemental e Cristal de Pacto

### 3.2 Os Dez Reis Monstros (Resumo)

**Nota:** Jogador pode enfrentar em qualquer ordem. Descrições completas disponíveis em documento separado.

#### 3.2.1 Rainha Melífera, a Arquiteta Dourada

- **Elemento:** Nature + Earth + Air
- **Bioma:** Floresta Calma (Primavera, Manhã, Sol Claro)
- **Desafio:** Construir estrutura geometricamente perfeita
- **Filosofia:** Ordem através da cooperação perfeita

#### 3.2.2 Imperador Escavarrok, o Senhor das Profundezas

- **Elemento:** Earth + Shadow
- **Bioma:** Área Rochosa (Profundezas, Qualquer horário)
- **Desafio:** Navegar túneis escuros usando vibrações
- **Filosofia:** Paciência e persistência vencem montanhas

#### 3.2.3 Imperatriz Nictófila, a Rainha da Noite Profunda

- **Elemento:** Ice + Air + Shadow
- **Bioma:** Floresta Calma (Noite, Céu Limpo)
- **Desafio:** Seguir padrão das estrelas
- **Filosofia:** Beleza existe na escuridão

#### 3.2.4 Sultan Escamífero, o Vencedor de Corridas

- **Elemento:** Fire + Air
- **Bioma:** Câmaras de Lava (Tarde, Calor Intenso)
- **Desafio:** Corrida através dos desfiladeiros flamejantes
- **Filosofia:** Velocidade é liberdade

#### 3.2.5 Rainha Formicida, a Estrategista Coletiva

- **Elemento:** Shadow + Earth + Nature
- **Bioma:** Pântano das Névoas (Qualquer horário, Névoa Densa)
- **Desafio:** Restaurar equilíbrio do ecossistema
- **Filosofia:** Juntos somos invencíveis

#### 3.2.6 Duquesa Solibrida, a Rainha do Escuro

- **Elemento:** Dark + Shadow
- **Desafio:** Puzzle de ilusões e percepção
- **Filosofia:** Verdade escondida nas sombras

#### 3.2.7 Príncipe Fulgorante, o Regente Elétrico

- **Elemento:** Air + Fire (Eletricidade)
- **Desafio:** Corrida contra relâmpagos
- **Filosofia:** Liberdade através da velocidade

#### 3.2.8 Conde Castoro, o Construtor Comunitário

- **Elemento:** Earth + Water
- **Desafio:** Construir barragem funcional
- **Filosofia:** Comunidade constrói futuro

#### 3.2.9 Matriarca Flores, a Guardiã Gentil

- **Elemento:** Nature Growth
- **Desafio:** Curar jardim doente
- **Filosofia:** Gentileza é força verdadeira

#### 3.2.10 Grão-Sacerdote Luminescente, o Guardião Cristalino

- **Elemento:** All Elements
- **Desafio:** Harmonizar todos os elementos simultaneamente
- **Filosofia:** Harmonia é poder supremo

---

## 4. Mundo do Jogo e Ambientação

### 4.1 Montanhas Cristalinas de Aethros

**Lore:**
Região montanhosa rica em cristais elementais que emanam energia mágica. Dez Reis Monstros governam diferentes domínios, cada um representando maestria sobre elementos específicos. Slimes sempre foram considerados criaturas simples, mas o protagonista é um Slime Branco raro com capacidade única de absorver essências elementais.

### 4.2 Biomas Principais

#### 4.2.1 Ninho do Slime (Tutorial)

- **Função:** Ponto de partida e lar evolutivo
- **Atmosfera:** Caverna aconchegante com cristais brilhantes
- **Criaturas:** Slimes comuns, morcegos pacíficos
- **Puzzles:** Tutoriais básicos de movimento e interação

#### 4.2.2 Floresta Calma (Nature/Earth/Air)

- **Elemento:** Nature
- **Reis Monstros:** Rainha Melífera, Imperatriz Nictófila
- **Atmosfera:** Primavera eterna, flores cristalinas, colmeias suspensas
- **Criaturas:** Cervos-Broto, Esquilos Coletores, Abelhas Cristalinas
- **Puzzles:** Geometria, padrões naturais, crescimento de plantas

#### 4.2.3 Lago Espelhado (Water/Air)

- **Elemento:** Water
- **Rei Monstro:** Imperador Escavarrok (nas profundezas)
- **Atmosfera:** Águas cristalinas que refletem perfeitamente
- **Criaturas:** Enguias Cristalizadas, Lontras Cristalinas, Águas-vivas Espelhadas
- **Puzzles:** Reflexos, correntes d'água, mergulho

#### 4.2.4 Área Rochosa (Earth/Fire)

- **Elemento:** Earth
- **Rei Monstro:** Conde Castoro
- **Atmosfera:** Formações rochosas, cavernas, construções de castores
- **Criaturas:** Golems de Pedra, Borboletas Minerais, Castores Arquitetos
- **Puzzles:** Peso, estruturas, engenharia

#### 4.2.5 Pântano das Névoas (Shadow/Water/Nature)

- **Elemento:** Shadow
- **Rei Monstro:** Rainha Formicida
- **Atmosfera:** Névoas densas, bioluminescência, colônias subterrâneas
- **Criaturas:** Libélulas-Névoa, Jacarés-Musgo, Rãs-Eco, Formigas Telepáticas
- **Puzzles:** Stealth, ecossistema, navegação na névoa

#### 4.2.6 Câmaras de Lava (Fire/Earth)

- **Elemento:** Fire
- **Rei Monstro:** Sultan Escamífero
- **Atmosfera:** Calor intenso, rios de lava, desfiladeiros flamejantes
- **Criaturas:** Escaravelhos-Magma, Salamandras de Fogo, Lagartos Velozes
- **Puzzles:** Velocidade, plataformas móveis, timing

#### 4.2.7 Pico Nevado (Air/Water/Ice)

- **Elemento:** Air e Ice
- **Reis Monstros:** Príncipe Fulgorante (tempestades)
- **Atmosfera:** Neve perpétua, ventos fortes, céu estrelado
- **Criaturas:** Corujas-Cristal, Raposas-Vento, Borboletas Glaciais
- **Puzzles:** Vento, gelo, constelações

---

## 5. Sistemas Temporais e Climáticos

### 5.1 Ciclo Dia/Noite

**Duração:** 24 minutos reais = 1 dia completo

**Períodos:**

- **Madrugada (05:00-06:59):** Transição, poucas criaturas
- **Manhã (07:00-11:59):** Criaturas diurnas ativas, melhor visibilidade
- **Tarde (12:00-17:59):** Pico de atividade, calor máximo
- **Entardecer (18:00-19:59):** Criaturas crepusculares, luz dourada
- **Noite (20:00-04:59):** Criaturas noturnas, bioluminescência, estrelas visíveis

**Efeitos Gameplay:**

- Certos NPCs só aparecem em horários específicos
- Puzzles estelares só funcionam à noite
- Visibilidade reduzida à noite (exceto com item/habilidade)
- Alguns inimigos mais fortes à noite

### 5.2 Ciclo Sazonal

**Duração:** 7 dias reais = 1 estação

**Estações:**

- **Primavera:** Flores emergem, criaturas mais amigáveis, chuvas leves
- **Verão:** Vegetação máxima, dias longos, calor
- **Outono:** Folhagem dourada, névoas, colheita
- **Inverno:** Neve, noites longas, alguns caminhos bloqueados

---

## 6. Sistema de Construção e Expansão do Lar

### 6.1 Caverna Principal (Inicial)

**Função:** Estação de descanso e save point

### 6.2 Jardim de Cristais

**Desbloqueio:** Amizade nível 3 com Cervos-Broto  
**Função:** Gera 1 cristal elemental aleatório por dia

### 6.3 Lago Interno

**Desbloqueio:** Amizade nível 4 com Castores Arquitetos  
**Função:** Cura contínua (+5 HP/segundo) quando próximo

### 6.4 Sótão Panorâmico

**Desbloqueio:** Amizade nível 4 com Borboletas Minerais  
**Função:** Previsão climática (próximas 3 mudanças)

### 6.5 Câmara dos Pactos

**Desbloqueio:** Automático após primeiro Ritual de Reconhecimento  
**Função:** Exibir Cristais de Pacto e receber buffs  
**Layout:** 10 pedestais em círculo, plataforma central

---

## 7. Sistema de Inventário

**Capacidade:** 20 slots (expansível até 40)

**Categorias:**

- Cristais Elementais (não ocupam slots, contador separado)
- Cristais de Pacto (não ocupam slots, coleção separada)
- Itens Consumíveis (poções, comida)
- Materiais de Crafting
- Itens de Quest
- Equipamentos

### 7.1 Estrutura do Inventário

**Organização:**

- Grid 5x4 (20 slots iniciais)
- Expansível para 5x8 (40 slots) através de upgrades
- Drag and drop para reorganizar
- Stacking automático de itens idênticos
- Stack máximo: 99 unidades por slot

**UI do Inventário:**

- Atalho: Tab (teclado) / - (Switch) / Touchpad (PlayStation) / View (Xbox)
- Pausa o jogo quando aberto
- Exibe informações detalhadas ao passar mouse/cursor sobre item
- Filtros por categoria (All, Consumables, Materials, Quest, Equipment)
- Peso visual: itens raros têm borda colorida (comum=cinza, raro=azul, épico=roxo, lendário=dourado)

### 7.2 Tipos de Itens

**Consumíveis:**

- **Poções de Cura:** Restaura HP instantaneamente (Pequena=25 HP, Média=50 HP, Grande=100 HP)
- **Poções de Stamina:** Restaura Stamina instantaneamente (Pequena=25, Média=50, Grande=100)
- **Comida:** Regeneração gradual de HP ao longo de 30 segundos
- **Buffs Temporários:** Aumenta atributos por tempo limitado (Velocidade+20% por 60s, Dano+15% por 45s)

**Materiais de Crafting:**

- Flores Cristalinas (comum)
- Essência Elemental (raro)
- Fragmentos de Cristal (comum)
- Madeira Antiga (comum)
- Pedra Rúnica (raro)

**Itens de Quest:**

- Não podem ser descartados
- Marcados com ícone de exclamação
- Removidos automaticamente ao completar quest

**Equipamentos:**

- Amuletos (buffs passivos)
- Anéis (efeitos especiais)
- Capas (defesa adicional)
- Máximo 3 equipamentos simultâneos

### 7.3 Gerenciamento de Inventário

**Ações Disponíveis:**

- **Usar:** Consome item (apenas consumíveis)
- **Equipar:** Equipa item (apenas equipamentos)
- **Descartar:** Remove item do inventário (confirmação necessária)
- **Dividir Stack:** Divide pilha de itens em duas
- **Favoritar:** Marca item para não ser vendido acidentalmente

**Inventário Cheio:**

- Notificação visual quando tentar coletar item
- Opção de descartar item automaticamente ou abrir inventário
- Itens no chão permanecem por 5 minutos antes de desaparecer

---

## 8. Sistema de Diálogo

### 8.1 Filosofia do Sistema

O sistema de diálogo em The Slime King é **orgânico e contextual**. NPCs reagem ao estado do mundo, reputação do jogador, hora do dia, e ações anteriores. Não há diálogos genéricos - cada conversa revela personalidade, lore, ou informações úteis.

### 8.2 Tipos de Diálogo

#### 8.2.1 Diálogo Linear

**Uso:** Conversas simples, informações diretas, saudações

**Estrutura:**

- NPC fala → Jogador lê → Diálogo termina
- Sem escolhas
- Pode ter múltiplas páginas (máximo 3)
- Sempre pulável após 2 segundos

**Exemplo:**

```
Cervo-Broto: "As flores cristalinas brilham mais ao amanhecer. 
É o melhor momento para colhê-las!"
```

#### 8.2.2 Diálogo com Escolhas

**Uso:** Quests, decisões importantes, construção de relacionamento

**Estrutura:**

- NPC fala → Jogador escolhe resposta (2-4 opções) → NPC reage
- Escolhas podem afetar:
  - Reputação (+5 a +20 pontos)
  - Amizade com espécie
  - Desbloqueio de quests
  - Recompensas diferentes

**Exemplo:**

```
Esquilo Coletor: "Perdi minha coleção de nozes cristalinas! 
Você pode me ajudar a encontrá-las?"

[Aceitar Quest] → "Claro! Onde você as viu pela última vez?"
[Recusar] → "Desculpe, estou ocupado agora."
[Perguntar Recompensa] → "O que você me dará em troca?"
```

#### 8.2.3 Diálogo Condicional

**Uso:** Reações baseadas em contexto (reputação, hora, clima, progresso)

**Condições Possíveis:**

- **Reputação:** NPCs tratam jogador diferente baseado em nível de reputação
- **Amizade:** Diálogos exclusivos em níveis altos de amizade
- **Hora do Dia:** NPCs comentam sobre manhã/tarde/noite
- **Clima:** Reações a chuva, neve, tempestade
- **Progresso:** Mencionam Reis Monstros derrotados, quests completadas
- **Primeira Vez:** Diálogo especial no primeiro encontro

**Exemplo:**

```
// Primeira vez
Abelha Cristalina: "Um slime? Que estranho ver um por aqui..."

// Após amizade nível 3
Abelha Cristalina: "Olá, amigo! A colmeia está florescendo graças à sua ajuda!"

// Após derrotar Rainha Melífera
Abelha Cristalina: "Você recebeu a bênção da Rainha! Que honra!"
```

### 8.3 Sistema de Apresentação

**UI de Diálogo:**

- Caixa de diálogo na parte inferior da tela (20% da altura)
- Portrait do NPC à esquerda (64x64 pixels, animado)
- Nome do NPC acima do portrait
- Texto com efeito de digitação (30 caracteres/segundo)
- Indicador de "mais texto" (seta piscando)
- Botão de skip visível após 2 segundos

**Animações:**

- Portrait do NPC anima sutilmente (idle breathing)
- Expressões mudam baseado no tom (feliz, triste, surpreso, bravo)
- Partículas emocionais (corações, gotas de suor, pontos de exclamação)

**Áudio:**

- Som de "blip" durante digitação (pitch varia por NPC)
- SFX de emoção (riso, suspiro, grito)
- Música de fundo diminui 30% durante diálogo

### 8.4 Sistema de Memória de Diálogo

**Tracking:**

- Jogo rastreia quais diálogos já foram vistos
- NPCs não repetem informações já dadas
- Referências a conversas anteriores

**Exemplo:**

```
// Primeira conversa
Esquilo: "Você sabia que cristais verdes crescem perto de água?"

// Segunda conversa (não repete)
Esquilo: "Como vão suas aventuras? Encontrou muitos cristais?"
```

### 8.5 Integração com Outros Sistemas

**Quest System:**

- Diálogos podem iniciar quests
- Quest givers têm diálogos específicos para oferta/progresso/conclusão
- Diálogos de quest são marcados com ícone de exclamação

**Friendship System:**

- Cada conversa positiva aumenta amizade (+5 pontos)
- Níveis altos de amizade desbloqueiam diálogos exclusivos
- NPCs lembram de favores feitos pelo jogador

**Reputation System:**

- NPCs reagem diferente baseado em reputação
- Reputação alta = diálogos mais amigáveis e respeitosos
- Reputação baixa = diálogos frios ou hostis

### 8.6 Ferramentas de Desenvolvimento

**DialogueData ScriptableObject:**

```csharp
[CreateAssetMenu(fileName = "DialogueData", menuName = "Game/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public string dialogueID;
    public string npcName;
    public Sprite npcPortrait;
    public List<DialogueNode> nodes;
}

[System.Serializable]
public class DialogueNode
{
    public string nodeID;
    public string text;
    public EmotionType emotion; // Happy, Sad, Surprised, Angry, Neutral
    public List<DialogueChoice> choices;
    public List<DialogueCondition> conditions;
    public DialogueEffect effect; // Reputation change, quest trigger, etc.
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public string nextNodeID;
    public int reputationChange;
    public int friendshipChange;
}
```

**Editor Visual (Futuro):**

- Node-based dialogue editor
- Preview de diálogos
- Teste de condições
- Exportação para localização

---

## 9. Sistema de Árvore de Habilidades

### 9.1 Estrutura da Árvore

**Organização:**

- 7 árvores elementais independentes (Nature, Earth, Air, Water, Fire, Shadow, Ice)
- Cada árvore tem 3 tiers verticais
- Progressão linear dentro de cada tier (desbloquear Tier 1 antes de Tier 2)
- Total: 21 habilidades (7 elementos × 3 tiers)

**Desbloqueio:**

- Árvore de Habilidades desbloqueia ao evoluir para **Adulto**
- Inicialmente, apenas Tier 1 de todos elementos está disponível
- Tier 2 desbloqueia ao evoluir para **Grande Slime**
- Tier 3 desbloqueia ao evoluir para **Rei Slime**

### 9.2 Custo de Habilidades

**Cristais Elementais como Moeda:**

- Cada habilidade custa cristais do elemento correspondente
- Tier 1: 10-15 cristais
- Tier 2: 25-35 cristais
- Tier 3: 50-75 cristais

**Exemplo:**

- Nature Tier 1 "Crescimento Rápido": 10 Cristais Verdes
- Nature Tier 2 "Espinhos Defensivos": 30 Cristais Verdes
- Nature Tier 3 "Jardim Selvagem": 60 Cristais Verdes

### 9.3 Habilidades por Elemento

#### Nature (Verde)

**Tier 1 - Crescimento Rápido (10 cristais)**

- Cria vinhas que prendem inimigos por 3s
- Cooldown: 8s | Stamina: 15
- Alcance: 8 unidades

**Tier 2 - Espinhos Defensivos (30 cristais)**

- Cria barreira de espinhos que reflete 30% do dano
- Duração: 6s | Cooldown: 15s | Stamina: 20

**Tier 3 - Jardim Selvagem (60 cristais)**

- Transforma área (10u raio) em jardim que cura aliados (+5 HP/s) e danifica inimigos (10 DPS)
- Duração: 10s | Cooldown: 30s | Stamina: 30

#### Water (Azul)

**Tier 1 - Jato d'Água (10 cristais)**

- Dispara jato que empurra inimigos 5 unidades
- Cooldown: 5s | Stamina: 10
- Dano: 15

**Tier 2 - Escudo Aquático (30 cristais)**

- Cria bolha que absorve 50% do dano por 5s
- Cooldown: 12s | Stamina: 20

**Tier 3 - Tsunami (60 cristais)**

- Onda massiva que atravessa tela inteira
- Dano: 80 | Empurra inimigos
- Cooldown: 25s | Stamina: 30

#### Fire (Vermelho)

**Tier 1 - Bola de Fogo (12 cristais)**

- Projétil que causa dano em área (3u raio)
- Dano: 25 | Cooldown: 6s | Stamina: 15

**Tier 2 - Trilha Flamejante (32 cristais)**

- Deixa rastro de fogo que persiste por 8s
- Dano: 10 DPS | Cooldown: 14s | Stamina: 20

**Tier 3 - Explosão Solar (65 cristais)**

- Explosão massiva centrada no slime (12u raio)
- Dano: 120 | Cooldown: 35s | Stamina: 30

#### Shadow (Roxo)

**Tier 1 - Passo Sombrio (12 cristais)**

- Teleporte curto (5 unidades) na direção do movimento
- Cooldown: 7s | Stamina: 15
- Invulnerável durante teleporte (0.3s)

**Tier 2 - Camuflagem (35 cristais)**

- Torna-se invisível por 6s ou até atacar
- Cooldown: 20s | Stamina: 25
- Inimigos perdem aggro

**Tier 3 - Clone Sombrio (70 cristais)**

- Cria clone que atrai inimigos por 10s
- Clone tem 50% do HP do jogador
- Cooldown: 30s | Stamina: 30

#### Earth (Marrom)

**Tier 1 - Pilar de Pedra (10 cristais)**

- Ergue pilar de pedra que bloqueia projéteis
- Duração: 8s | Cooldown: 10s | Stamina: 15
- HP do pilar: 100

**Tier 2 - Tremor (30 cristais)**

- Causa tremor que atordoa inimigos em 8u raio por 2s
- Dano: 20 | Cooldown: 15s | Stamina: 20

**Tier 3 - Fortaleza de Pedra (60 cristais)**

- Cria cúpula de pedra ao redor do slime
- Duração: 8s | Imune a dano | Cooldown: 40s | Stamina: 30

#### Air (Branco)

**Tier 1 - Rajada de Vento (10 cristais)**

- Empurra inimigos e projéteis em cone (90°)
- Alcance: 10u | Cooldown: 6s | Stamina: 12

**Tier 2 - Levitação (28 cristais)**

- Flutua por 5s, ignora terreno e armadilhas
- Velocidade +20% | Cooldown: 18s | Stamina: 20

**Tier 3 - Tornado (55 cristais)**

- Cria tornado que suga inimigos e causa dano
- Dano: 15 DPS | Duração: 6s | Cooldown: 28s | Stamina: 28

#### Ice (Ciano)

**Tier 1 - Lança de Gelo (12 cristais)**

- Projétil que congela inimigo por 2s
- Dano: 20 | Cooldown: 7s | Stamina: 15

**Tier 2 - Caminho Gelado (32 cristais)**

- Cria trilha de gelo que aumenta velocidade de aliados (+30%) e diminui de inimigos (-50%)
- Duração: 10s | Cooldown: 16s | Stamina: 20

**Tier 3 - Nevasca (68 cristais)**

- Tempestade de gelo em área (15u raio)
- Dano: 12 DPS | Reduz velocidade 70% | Duração: 8s
- Cooldown: 32s | Stamina: 30

### 9.4 UI da Árvore de Habilidades

**Navegação:**

- Atalho: H (teclado) / Y (Xbox) / Triangle (PlayStation) / X (Switch)
- Tabs para cada elemento na parte superior
- Visualização vertical (Tier 1 → Tier 2 → Tier 3)
- Habilidades bloqueadas aparecem em cinza com cadeado

**Informações Exibidas:**

- Nome da habilidade
- Ícone visual
- Descrição detalhada
- Custo em cristais
- Cooldown e custo de Stamina
- Dano/Efeitos
- Vídeo preview (GIF animado) ao passar mouse

**Desbloqueio:**

- Clicar em habilidade bloqueada mostra requisitos
- Botão "Desbloquear" se tiver cristais suficientes
- Animação de desbloqueio (partículas, som)
- Habilidade automaticamente adicionada ao inventário de habilidades

### 9.5 Equipando Habilidades

**4 Slots de Habilidades:**

- Slot 1 (Q/ZL/L2/LT): Habilidade primária
- Slot 2 (E/L/L1/LB): Habilidade secundária
- Slot 3 (R/R/R1/RB): Habilidade terciária
- Slot 4 (F/ZR/R2/RT): Habilidade ultimate

**Customização:**

- Qualquer habilidade desbloqueada pode ser equipada em qualquer slot
- Drag and drop para reorganizar
- Presets salvos (até 3 loadouts)
- Troca de loadout fora de combate (3s de cast time)

**Sinergias:**

- Algumas combinações de habilidades têm bônus
- Exemplo: Crescimento Rápido + Jato d'Água = Vinhas molhadas prendem por +1s
- Sinergias são descobertas experimentando

### 9.6 Progressão e Balanceamento

**Economia de Cristais:**

- Jogador médio terá ~200 cristais de cada elemento ao final do jogo
- Custo total para desbloquear todas habilidades: ~700 cristais (100 por elemento)
- Jogador precisará escolher quais elementos priorizar
- Completistas podem desbloquear tudo coletando todos cristais

**Respec:**

- Não há respec de habilidades
- Decisões são permanentes
- Incentiva múltiplos playthroughs com builds diferentes

---

## 10. Sistema de Save/Load

### 10.1 Filosofia do Sistema

O sistema de save em The Slime King é **automático e manual**. O jogo salva automaticamente em momentos-chave, mas também permite saves manuais em pontos de descanso. Não há punição por morte - o jogador respawna no último ponto de save com todos itens e progresso intactos.

### 10.2 Pontos de Save

**Save Automático:**

- Ao entrar/sair de cenas
- Após completar quest
- Após derrotar Rei Monstro
- Ao evoluir
- A cada 5 minutos de gameplay (background save)
- Ao fechar o jogo

**Save Manual:**

- Em pontos de descanso (Caverna Principal, fogueiras)
- Ícone de save aparece no HUD
- Animação de save (cristal brilha, som suave)
- Confirmação visual: "Jogo Salvo"

### 10.3 Dados Salvos

**Progresso do Jogador:**

- Posição no mundo (cena, coordenadas X/Y)
- Estágio de evolução (Filhote, Adulto, Grande, Rei, Transcendente)
- HP e Stamina atuais
- Reputação (pontos invisíveis)
- Cristais Elementais (contador de cada tipo)
- Cristais de Pacto coletados

**Inventário:**

- Todos itens no inventário (tipo, quantidade, posição no grid)
- Equipamentos equipados
- Habilidades desbloqueadas
- Habilidades equipadas nos 4 slots
- Loadouts salvos

**Progresso de Mundo:**

- Quests completadas e em progresso
- Objetivos de quest atuais
- NPCs encontrados
- Diálogos vistos
- Amizade com cada espécie (nível 0-5)
- Reis Monstros derrotados (0-10)
- Rituais de Reconhecimento recebidos

**Expansões do Lar:**

- Quais expansões foram construídas
- Decorações colocadas
- NPCs visitantes

**Mundo Persistente:**

- Itens coletados (não respawnam)
- Baús abertos
- Puzzles resolvidos
- Áreas secretas descobertas
- Teleport points ativados

**Configurações:**

- Volume (master, music, sfx)
- Controles customizados
- Idioma
- Qualidade gráfica

### 10.4 Slots de Save

**Múltiplos Saves:**

- 3 slots de save independentes
- Cada slot mostra:
  - Screenshot do último save
  - Nome do jogador (editável)
  - Tempo de jogo total
  - Estágio de evolução
  - Número de Reis Monstros derrotados
  - Data/hora do último save

**Gerenciamento:**

- Copiar save para outro slot
- Deletar save (confirmação necessária)
- Renomear save

### 10.5 Sistema de Backup

**Auto-Backup:**

- Backup automático a cada 30 minutos
- Mantém últimos 3 backups
- Armazenado em pasta separada

**Recuperação:**

- Se save corrompido, jogo oferece carregar último backup
- Mensagem clara: "Save corrompido. Carregar backup de [data/hora]?"

### 10.6 Cloud Save (Steam)

**Sincronização:**

- Saves sincronizam automaticamente com Steam Cloud
- Permite jogar em múltiplos PCs
- Indicador de sincronização no menu

**Conflito de Saves:**

- Se detectar conflito (saves diferentes em PC e cloud)
- Pergunta qual versão manter:
  - "Local (mais recente: [data])"
  - "Cloud (mais recente: [data])"

### 10.7 Morte e Respawn

**Sem Punição:**

- Morte não resulta em perda de progresso
- Jogador respawna no último ponto de save
- Todos itens e cristais mantidos
- HP e Stamina restaurados

**Opções ao Morrer:**

- "Respawn" (volta ao último save)
- "Load Save" (carrega save manual anterior)
- "Main Menu" (volta ao menu principal)

### 10.8 Implementação Técnica

**Formato de Save:**

- JSON serializado
- Criptografia leve (anti-cheat básico)
- Compressão para reduzir tamanho

**Localização dos Saves:**

- **Windows:** `%APPDATA%/TheSlimeKing/Saves/`
- **Mac:** `~/Library/Application Support/TheSlimeKing/Saves/`
- **Linux:** `~/.config/TheSlimeKing/Saves/`
- **Switch:** Armazenamento interno do console

**Estrutura de Arquivo:**

```
Saves/
├── Slot1/
│   ├── save.json (save principal)
│   ├── backup_1.json
│   ├── backup_2.json
│   ├── backup_3.json
│   └── screenshot.png
├── Slot2/
└── Slot3/
```

**SaveData ScriptableObject:**

```csharp
[System.Serializable]
public class SaveData
{
    // Player
    public string playerName;
    public Vector2 playerPosition;
    public string currentScene;
    public EvolutionStage evolutionStage;
    public int currentHP;
    public int currentStamina;
    public int reputation;
    
    // Inventory
    public List<ItemData> inventoryItems;
    public List<AbilityData> unlockedAbilities;
    public int[] equippedAbilitySlots; // IDs das habilidades equipadas
    
    // Crystals
    public Dictionary<ElementType, int> elementalCrystals;
    public List<string> pactCrystalsCollected;
    
    // Progress
    public List<string> completedQuests;
    public List<QuestProgress> activeQuests;
    public Dictionary<string, int> npcFriendship;
    public List<string> defeatedMonsterKings;
    
    // World State
    public List<string> collectedItems;
    public List<string> openedChests;
    public List<string> solvedPuzzles;
    public List<string> discoveredAreas;
    
    // Home
    public List<string> homeExpansions;
    
    // Meta
    public float totalPlayTime;
    public System.DateTime lastSaveTime;
    public string gameVersion;
}
```

---

## 11. Sistema de IA e Comportamento

### 11.1 Arquitetura de IA

**Engine:** Unity 6.2 com URP  
**Sistema:** Máquina de Estados Finitos (FSM)  
**Implementação:** ScriptableObject-based AI para modularidade

### 8.2 Estados de IA - Inimigos (10 Estados)

#### 8.2.1 Idle (Ocioso)

- Permanece em posição ou patrulha pequena (raio 5 unidades)
- Percepção ativa (visão 10u, audição 5u)
- **Transições:** Alert (movimento suspeito), Chase (ver jogador), Patrol (após 5-10s)

#### 8.2.2 Patrol (Patrulha)

- Move entre 2-6 waypoints
- Velocidade: 70% da máxima
- **Transições:** Alert (suspeita), Chase (ver jogador), Idle (fim da rota)

#### 8.2.3 Alert (Alerta)

- Para e rotaciona em direção ao som
- Aumenta percepção em 50%
- Duração: 3-5 segundos
- **Transições:** Chase (confirmar jogador), Investigate (suspeitar), Idle/Patrol (nada encontrado)

#### 8.2.4 Investigate (Investigar)

- Move para última posição conhecida
- Velocidade: 80%
- Percepção dobrada
- **Transições:** Chase (encontrar), Alert (novo som), Return (não encontrar após 8s)

#### 8.2.5 Chase (Perseguir)

- Move diretamente ao jogador
- Velocidade: 100-110%
- Atualiza caminho a cada 0.2s
- **Transições:** Attack (alcance), Search (perder visão 3s), Return (30+ unidades)
- **Stealth:** Jogador agachado atrás de cobertura quebra perseguição

#### 8.2.6 Attack (Atacar)

- Para e executa ataque
- Cooldown: 1-2 segundos
- **Transições:** Chase (jogador sair), Victory (derrotar), Flee (HP < 20%)

#### 8.2.7 Search (Procurar)

- Move para última posição
- Padrão de busca (círculos/zigue-zague)
- Duração: 10-15 segundos
- **Transições:** Chase (encontrar), Alert (detectar), Return (não encontrar)

#### 8.2.8 Return (Retornar)

- Volta para spawn point
- Velocidade: 90%
- Regenera HP (2/segundo)
- **Transições:** Idle/Patrol (chegar), Alert (detectar novamente)

#### 8.2.9 Flee (Fugir)

- Move na direção oposta
- Velocidade: 120%
- **Transições:** Hide (encontrar esconderijo), Return (40+ unidades), Chase (HP > 50% + aliados)

#### 8.2.10 Stunned (Atordoado)

- Não pode mover ou atacar
- Duração: 2-4 segundos
- Vulnerável (+20% dano)
- **Transições:** Alert (após stun), Flee (HP baixo)

### 8.3 Sistema de Percepção

**Visão (Line of Sight):**

- Raycast 2D em cone (90-120 graus)
- Alcance: 10-15 unidades
- Bloqueado por obstáculos
- **Stealth:** Jogador agachado atrás de objeto = invisível

**Audição:**

- Raio: 5-8 unidades
- Sons têm intensidade (0.0-1.0)
- Atravessa obstáculos (reduz intensidade)

**Proximidade:**

- Trigger 2D (2-3 unidades)
- Detecta mesmo sem visão
- Usado por criaturas com sentidos aguçados

**Código Exemplo:**

```csharp
bool CanSeePlayer()
{
    Vector2 dir = player.position - transform.position;
    float angle = Vector2.Angle(transform.up, dir);
    
    if (angle < visionAngle / 2f && dir.magnitude < visionRange)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, visionRange);
        
        if (hit.collider != null && hit.collider.transform == player)
        {
            // Verifica stealth
            if (player.isCrouching && HasCoverBetween(transform.position, player.position))
                return false;
            return true;
        }
    }
    return false;
}
```

---

## 12. Sistema de Quests

### 12.1 Filosofia

Quests são **orgânicas e emergentes**. NPCs têm problemas reais que o jogador pode escolher resolver. Não há marcadores obrigatórios.

### 9.2 Tipos de Objetivos (6 Tipos)

#### 9.2.1 Collect (Coletar)

- Coletar X quantidade de item Y
- Exemplo: "Colete 5 Flores Cristalinas"

#### 9.2.2 Defeat (Derrotar)

- Derrotar X quantidade de inimigo Y
- Exemplo: "Elimine 3 Golems de Pedra"

#### 9.2.3 Deliver (Entregar)

- Entregar item X para NPC Y
- Exemplo: "Leve esta carta para o Conde Castoro"

#### 9.2.4 Explore (Explorar)

- Descobrir localização X
- Exemplo: "Encontre a Caverna Perdida"

#### 9.2.5 Interact (Interagir)

- Interagir com objeto/NPC específico
- Exemplo: "Ative os 3 cristais antigos"

#### 9.2.6 Escort (Escoltar)

- Proteger NPC até destino
- Exemplo: "Escolte o Esquilo até sua casa"

### 9.3 Estrutura de Quest

```csharp
[CreateAssetMenu(fileName = "Quest", menuName = "Quest System/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public string description;
    public string giverNPCID;
    public List<QuestObjective> objectives;
    public int reputationReward;
    public List<ItemReward> itemRewards;
    public bool repeatable;
}
```

---

## 13. Sistema de Cutscenes

### 13.1 Tipos de Cutscenes (4 Tipos)

#### 10.1.1 Dialogue (Diálogo)

- Conversa simples sem movimento de câmera
- Duração: 10-30 segundos
- Sempre pulável

#### 10.1.2 Cinematic (Cinemática)

- Com movimento de câmera
- Duração: 15-60 segundos
- Pulável após 3 segundos

#### 10.1.3 Ritual (Reconhecimento)

- Estrutura fixa para Rituais de Reconhecimento
- Duração: ~25-30 segundos
- Pulável após 5 segundos

**Fases do Ritual:**

1. Zoom para Rei Monstro (2s)
2. Diálogo de Reconhecimento (10-15s)
3. Marcação Elemental (5s)
4. Cristal de Pacto materializa (3s)
5. Título Adquirido (2s)
6. Fade Out (2s)

#### 10.1.4 Discovery (Descoberta)

- Câmera panorâmica ao descobrir área
- Duração: 3-5 segundos
- Jogador mantém controle parcial

---

## 14. Sistema de Puzzles

### 14.1 Filosofia

Puzzles são **integrados à lore e mecânicas**. Cada puzzle faz sentido no contexto do mundo e utiliza habilidades elementais, mecânica de agachar, e interação com ambiente.

**Princípios:**

- Solução lógica baseada em mecânicas estabelecidas
- Dificuldade progressiva (fácil → médio → difícil)
- Múltiplas soluções quando possível
- Feedback visual claro
- Sem time limits (exceto puzzles específicos de velocidade)

### 11.2 Categorias

1. **Elementais:** Usam habilidades elementais
2. **Stealth:** Usam agachar e detecção de IA
3. **Ambientais:** Interação com objetos
4. **Lógica:** Padrões e sequências
5. **Física:** Peso e momentum

### 11.3 Exemplos de Puzzles

#### Puzzle 1: Jardim Geométrico (Rainha Melífera)

**Tipo:** Elemental + Lógica  
**Dificuldade:** Médio

**Descrição:**
Sala hexagonal com 6 pilares de cristal. Cada pilar tem símbolo elemental. Ativar na ordem correta forma padrão geométrico.

**Mecânica:**

- Usar habilidade elemental correspondente em cada pilar
- Ordem: Nature → Earth → Water → Air → Fire → Shadow (ciclo natural)
- Pistas: Flores no chão formam padrão sutil

**Recompensa:** 15 Cristais Verdes, acesso à Câmara da Harmonia

---

#### Puzzle 2: Câmara do Eco (Imperador Escavarrok)

**Tipo:** Stealth + Audição  
**Dificuldade:** Médio

**Descrição:**
Túnel completamente escuro. Cristais emitem sons quando jogador se aproxima. Guardas cegos patrulham baseados em som.

**Mecânica:**

- Jogador deve **agachar (parado)** para reduzir ruído
- Cristais emitem "ping" que revela ambiente brevemente
- Guardas detectam por som
- Timing: esperar guardas passarem

**Estratégia:**

- Agachar e esperar guardas passarem
- Usar cristais para mapear ambiente
- Não se mover quando guarda está próximo

**Recompensa:** 20 Cristais Marrons, habilidade "Visão por Vibração"

---

#### Puzzle 3: Constelação Perdida (Imperatriz Nictófila)

**Tipo:** Lógica + Observação  
**Dificuldade:** Médio-Difícil

**Descrição:**
Jardim noturno com 12 cristais. Céu mostra constelação. Cristais devem ser iluminados na ordem que forma a mesma constelação.

**Mecânica:**

- Observar constelação no céu
- Usar habilidade Ice para "congelar" cristal na posição correta
- Ordem importa - seguir padrão de conexão das estrelas

**Pistas:** Borboletas noturnas voam entre cristais na ordem correta

**Recompensa:** 25 Cristais Ciano, título "Leitor de Estrelas"

---

#### Puzzle 4: Corrida Flamejante (Sultan Escamífero)

**Tipo:** Velocidade + Plataforma  
**Dificuldade:** Difícil

**Descrição:**
Desfiladeiro com plataformas móveis sobre lava. Alcançar cristal antes que timer expire (60 segundos).

**Mecânica:**

- Plataformas se movem em padrões previsíveis
- Algumas desmoronam após 1 segundo
- Jatos de lava surgem periodicamente
- Habilidades de movimento são essenciais

**Estratégia:**

- Memorizar padrão de plataformas
- Timing perfeito para evitar jatos
- Usar habilidades de movimento (Levitação, Passo Sombrio)

**Recompensa:** 30 Cristais Vermelhos, habilidade "Dash Flamejante"

---

#### Puzzle 5: Equilíbrio do Pântano (Rainha Formicida)

**Tipo:** Lógica + Diplomacia  
**Dificuldade:** Médio

**Descrição:**
Três espécies em conflito: Rãs-Eco, Libélulas-Névoa, Jacarés-Musgo. Restaurar equilíbrio sem eliminar nenhuma.

**Mecânica:**

- Conversar com representante de cada espécie
- Entender necessidades de cada um
- Encontrar solução que beneficie todos

**Soluções Possíveis:**

- **Diplomática:** Negociar territórios separados (+50 reputação)
- **Ecológica:** Introduzir planta que equilibra população (+40 reputação)
- **Agressiva:** Reduzir população (não recomendada, -20 reputação)

**Recompensa:** 35 Cristais Roxos, título "Mediador do Pântano"

---

#### Puzzle 6: Reflexos Espelhados (Lago Espelhado)

**Tipo:** Ambiental + Física  
**Dificuldade:** Médio

**Descrição:**
Cristais no teto só podem ser atingidos através dos reflexos na água.

**Mecânica:**

- Água reflete cristais perfeitamente
- Mirar no reflexo para atingir cristal real
- Projéteis ricocheteiam do reflexo

**Estratégia:**

- Posicionar corretamente para ter ângulo certo
- Usar habilidade de projétil (Bola de Fogo, Jato d'Água)
- 5 cristais devem ser ativados

**Recompensa:** 20 Cristais Azuis, item "Espelho Portátil"

---

#### Puzzle 7: Peso e Contrapeso (Área Rochosa)

**Tipo:** Física + Lógica  
**Dificuldade:** Médio-Difícil

**Descrição:**
4 plataformas de pressão. Porta só abre se todas tiverem peso correto simultaneamente.

**Mecânica:**

- Cada plataforma requer peso específico (10kg, 15kg, 20kg, 25kg)
- Objetos disponíveis: Pedras pequenas (5kg), médias (10kg), grandes (15kg)
- Slime tem peso próprio (5kg Filhote, aumenta com evolução)
- Seguidores têm peso próprio

**Exemplo de Solução:**

- Plataforma 1 (10kg): 2 pedras pequenas
- Plataforma 2 (15kg): 1 pedra média + 1 pequena
- Plataforma 3 (20kg): 1 pedra grande + 1 pequena
- Plataforma 4 (25kg): Slime (5kg) + Seguidor Golem (20kg)

**Recompensa:** 25 Cristais Marrons, habilidade "Pilar de Pedra Pesado"

---

#### Puzzle 8: Infiltração Silenciosa (Pântano das Névoas)

**Tipo:** Stealth Puro  
**Dificuldade:** Difícil

**Descrição:**
Fortaleza de formigas com 8 guardas. Alcançar cristal no centro sem ser detectado.

**Mecânica:**

- 8 guardas com patrulhas sincronizadas
- Áreas de cobertura: arbustos, rochas, pilares
- Detecção = reset do puzzle
- Sem combate permitido

**Estratégia:**

- Observar padrões de patrulha por 30 segundos
- **Agachar (parado)** atrás de cobertura quando guarda se aproxima
- Identificar "janelas" de tempo
- Movimento calculado entre coberturas

**Padrões:**

- Guardas 1-4: Patrulha externa (círculo, 20s)
- Guardas 5-6: Patrulha interna (linha, 15s)
- Guardas 7-8: Estáticos mas rotacionam 360° a cada 10s

**Recompensa:** 40 Cristais Roxos, título "Mestre da Infiltração"

---

## 15. Direção Visual e Sonora

### 15.1 Estilo Visual

**Pixel Art Moderno:**

- Resolução base: 320x180 (upscaled para 1920x1080)
- Sprites: 16x16px (Filhote) até 56x56px (Rei Transcendente)
- Paleta: 64 cores por bioma
- Animações: 4-8 frames por ação

**Iluminação:**

- Sistema de luz 2D dinâmico (URP)
- Sombras suaves
- Bioluminescência em criaturas e cristais
- Ciclo dia/noite afeta iluminação global

### 12.2 Direção Sonora

**Música Adaptativa:**

- Camadas que entram/saem baseado em contexto
- Transições suaves entre biomas
- Variações por hora do dia

**SFX:**

- Sons naturalísticos e orgânicos
- Feedback claro para ações
- Áudio posicional 2D

---

## 16. Controles e Interface

### 16.1 Mapeamento de Controles

| Função | Switch | PlayStation | Xbox | Keyboard |
|:--|:--|:--|:--|:--|
| Movimento | Analógico L | Analógico L | Analógico L | WASD |
| Habilidade 1 | ZL | L2 | LT | Q |
| Habilidade 2 | L | L1 | LB | E |
| Habilidade 3 | R | R1 | RB | R |
| Habilidade 4 | ZR | R2 | RT | F |
| Ataque | A | X | A | Space |
| Interagir | A | X | A | Space |
| **Agachar (parado)** | **B (segurar)** | **Circle (segurar)** | **B (segurar)** | **Ctrl (segurar)** |
| Menu | + | Options | Menu | Esc |
| Inventário | - | Touchpad | View | Tab |

### 13.2 HUD

**Elementos Mínimos:**

- HP Bar (canto superior esquerdo)
- Stamina Bar (abaixo do HP)
- Cristais Elementais (contador, canto superior direito)
- Habilidades Equipadas (canto inferior direito, com cooldowns)
- Quest Tracker (canto superior direito, opcional)
- Minimapa (canto inferior esquerdo, pode ser desativado)

---

## 17. Sistemas Técnicos

**Engine:** Unity 6.2  
**Render Pipeline:** Universal Render Pipeline (URP)  
**Linguagem:** C#  
**Controle de Versão:** Git

**Padrões:**

- Singleton para Managers
- ScriptableObjects para dados
- Event System para comunicação
- Object Pooling para projéteis e partículas

---

## 18. Performance e Otimização

**Targets:**

- **PC:** 60 FPS em 1920x1080
- **Switch:** 30 FPS estável (portátil), 60 FPS (docked)

**Técnicas:**

- Sprite Atlas para reduzir draw calls
- Occlusion Culling
- LOD para auras
- Object Pooling
- IA atualiza a cada 0.1-0.2 segundos

---

## 19. Métricas e Analytics

**Rastreadas:**

- Tempo para cada evolução
- Sequência de Reis Monstros conquistados
- Taxa de conclusão de quests
- Puzzles resolvidos vs abandonados
- Taxa de uso de stealth vs combate

---

## 20. Conclusão

**The Slime King v9.0** representa uma evolução significativa do conceito original, incorporando:

✅ **Narrativa Orgânica:** Progressão natural sem objetivos forçados  
✅ **Mecânica de Stealth:** Agachar (parado) adiciona profundidade ao gameplay  
✅ **IA Robusta:** Sistema de estados completo e comportamentos variados  
✅ **Quests Simplificadas:** Sistema modular e fácil de expandir  
✅ **Cutscenes Não-Intrusivas:** Curtas, significativas e puláveis  
✅ **Puzzles Criativos:** Integrados à lore e mecânicas do jogo  
✅ **Progressão Livre:** Jogador escolhe ordem e quais Reis Monstros enfrentar

O jogo mantém sua essência cozy e contemplativa enquanto oferece desafios significativos e sistemas profundos para jogadores que buscam mais complexidade.

---

**Versão:** 9.0  
**Data:** 2025  
**Última Atualização:** 29/10/2025  
**Changelog v9.0:**

- ✅ Adicionado Sistema de Diálogo (Seção 8)
- ✅ Adicionado Sistema de Árvore de Habilidades (Seção 9)
- ✅ Adicionado Sistema de Save/Load (Seção 10)
- ✅ Expandido Sistema de Inventário (Seção 7)
- ✅ Renumeradas seções 8-17 para 11-20

**Fim do Game Design Document v9.0**
