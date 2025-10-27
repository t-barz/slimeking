# The Slime King – Game Design Document v7.3 Completo

## 1. Conceito e Visão Geral

### 1.1 Conceito Central

**The Slime King** é um RPG de aventura 2D top-down em pixel art de mundo aberto focado em exploração com diversos quebra-cabeças e chances de revisitar lugares conforme avança no jogo. Nesse jogo, o jogador controla um slime branco recém-nascido e vivencia seu cotidiano cheio de desafios e descobertas que aos poucos o levam a evoluir até se tornar o lendário Rei Slime — o primeiro de sua espécie a alcançar tal título — em um mundo de fantasia habitado por criaturas únicas das Montanhas Cristalinas.

### 1.2 Premissa Narrativa

O slime desperta em seu ninho numa caverna escura, em um mundo onde **cinco a dez poderosas monarquias** já estão estabelecidas nas Montanhas Cristalinas de Aethros. Cada monarca governa seu reino com sabedoria, força e valores únicos — mas nenhum deles jamais foi um slime. Os slimes sempre foram vistos como criaturas simples, sem propósito maior, gelatinosos habitantes de cavernas sem ambições ou capacidades especiais.

Porém, o protagonista é diferente: um **Slime Branco** raro, dotado da capacidade única de absorver e canalizar todos os tipos de essência elemental — um dom que nunca foi explorado por nenhum de sua espécie.

Ao longo de sua jornada épica de autodescoberta, o slime precisará provar sua dignidade aos **Espíritos Elementais** que governam a região. Aqueles que o reconhecerem em seu poder e valor conjurarão **Rituais de Reconhecimento** — cerimônias sagradas que marcam o slime permanentemente com uma **Aura Elemental** e entregam um **Cristal de Pacto** que simboliza o reconhecimento do monarca. O jogador escolherá qual caminho seguir: conquistar os 5 monarcas primários (via clássico) ou desafiar todos os 10 monarcas (via épico), cada um representando um teste de maestria, coragem e sabedoria sobre elementos específicos.

Conforme estabelece laços profundos com as criaturas fantásticas das Montanhas, desenvolve seu humilde lar cristalino em um refúgio digno de realeza, domina os elementos um por um e aprende com os próprios monarcas, o slime ganha influência e respeito — não apenas entre sua própria espécie, mas entre todas as criaturas da região. Sua aura cresce, cristalizando no ar ao seu redor em padrões cada vez mais majestosos.

Sua meta final: reunir todas as **Auras Elementais** (5 ou 10 conforme escolha), coletando seus **Cristais de Pacto**, provar que os slimes são dignos de governança, e ser formalmente reconhecido pelos monarcas estabelecidos como um igual — tornando-se o **primeiro Rei Slime da história**, quebrando séculos de preconceito e estabelecendo uma nova dinastia real.

### 1.3 Experiência de Jogo

Atmosfera cozy e contemplativa, sem pressão de tempo. O jogador enfrenta desafios significativos, ganha recompensas variadas e vivencia um ciclo de crescimento que incentiva exploração profunda, personalização do lar, interação pacífica com a fauna local, e uma jornada épica de ascensão real única. Cada monarca oferece perspectiva diferente sobre liderança, tornando a jornada tanto uma odisseia de poder quanto de autoconhecimento. **O jogador controla seu próprio destino, vendo seu slime transformar-se visualmente conforme conquista reconhecimento, sua aura crescendo até formar uma coroa flutuante de pura energia elemental.**

---

## 2. Gameplay e Mecânicas Principais

### 2.1 Loop Principal de Gameplay

O jogador explora diversas áreas das Montanhas Cristalinas em busca de **Cristais Elementais** e dos raros **Rituais de Reconhecimento**. Os Cristais Elementais funcionam como moeda para desbloquear habilidades na Árvore de Habilidades, podendo ser encontrados após resolver puzzles, recebidos como recompensa ao completar quests para habitantes locais, ou obtidos como drop de inimigos derrotados.

Os **Rituais de Reconhecimento** são eventos únicos onde um monarca reconhece o slime após provar-se digno. Cada ritual marca permanentemente o slime com uma **Aura Elemental** nova e entrega um **Cristal de Pacto** simbólico que o slime pode coletar e exibir em seu lar.

### 2.2 Sistema de Evolução Revisado

O Slime cresce em tamanho e poder em quatro estágios evolutivos baseados na conquista dos **Rituais de Reconhecimento**. O número de auras necessárias varia conforme o modo escolhido:

#### Modos de Jogo

**Modo Clássico (5 Auras - Caminho Curto)**
- Mais acessível para jogadores causais
- Foca nos 5 monarcas primários
- Cerimônia de Coroação aos 5 monarcas
- Duração estimada: 30-40 horas

**Modo Épico (10 Auras - Odisseia Completa)**
- Desafiador para jogadores experientes
- Inclui todos os 10 monarcas
- Cerimônia de Coroação aos 10 monarcas
- Recompensas especiais adicionais
- Duração estimada: 50-70 horas

#### Estágios Evolutivos

**Filhote (Estado Inicial)**
- Movimentação básica e ataque corpo-a-corpo simples
- Sem habilidades elementais
- Sem capacidade de ter seguidores
- Tamanho: Pequeno (sprite base)
- Aura: Nenhuma
- **Objetivo:** Conquistar o primeiro Ritual de Reconhecimento

**Adulto (1 Aura Elemental)**
- Desbloqueia sistema de habilidades elementais (4 slots: ZL/L2, L/L1, R/R1, ZR/R2)
- Acesso à Árvore de Habilidades para comprar habilidades com Cristais Elementais
- Pode ter até 1 seguidor aliado
- Tamanho: Médio (sprite 1.5x maior)
- Aura: Brilho monocromático subtle (~1.2x do slime)
- **Objetivo:** Conquistar mais auras

**Grande Slime (3 Auras Elementais)**
- Acesso a habilidades elementais avançadas e combinações
- Maior potência em todos os atributos
- Pode ter até 3 seguidores aliados
- Tamanho: Grande (sprite 2x maior)
- Aura: Padrão visual complexo com 2-3 cores (~1.5x do slime), partículas leves
- **Objetivo:** Conquistar mais auras

**Rei Slime (5+ Auras Elementais)**
- Maestria completa dos elementos
- Liderança absoluta sobre criaturas
- Pode ter até 5 seguidores aliados (Modo Clássico) ou até 10 (Modo Épico)
- Tamanho: Muito Grande (sprite 2.5x maior)
- Aura: Majestosa e multicolorida (~2x do slime), pulsante com múltiplas cores harmoniosas
- Efeito: Radiância constante, trilha de luz ao mover

**Rei Slime Ascendido (7 Auras - Modo Épico)**
- Poder extraordinário
- Tamanho: Extremo (sprite 3x maior)
- Aura: Épica com padrão complexo (~2.5x do slime), cristais flutuam ao redor
- Efeito: Som de cristal ao mover, luz intensa

**Rei Slime Imortal (10 Auras - Modo Épico Final)**
- Transcendência elemental
- Tamanho: Máximo (sprite 3.5x maior)
- Aura: Forma **coroa flutuante etérea** de cristal multicolorido (~3x do slime)
- Efeito: Luz transcendental, cristais dançam ao redor, presença majestosa

### 2.3 Sistema de Aura Elemental

Cada monarca conquistado contribui uma **camada de aura** visual que se sobrepõe às anteriores, criando um efeito progressivo e épico.

#### Progressão Visual de Aura

**1ª Aura (Filhote → Adulto):**
- Brilho monocromático subtle
- Cor baseada no primeiro elemento
- Tamanho: ~1.2x do slime
- Sem padrão especial, apenas luz

**3ª Aura (Adulto → Grande Slime):**
- Padrão visual simples começa a aparecer
- Duas cores começam a se sobrepor (blend)
- Tamanho: ~1.5x do slime
- Partículas leves começam a circular

**5ª Aura (Grande Slime → Rei Slime):**
- Padrão complexo (hexágonos, ondas, ou cristais flutuando)
- Múltiplas cores harmoniosas
- Tamanho: ~2x do slime
- Aura pulsante, som suave de cristal

**7ª Aura (Modo Épico - Rei Slime Ascendido):**
- Padrão épico e claramente visível
- Múltiplas cores dançando harmoniosamente
- Tamanho: ~2.5x do slime
- Radiância constante, trilha de luz ao mover, som de cristal mais proeminente

**10ª Aura (Modo Épico - Rei Slime Imortal):**
- Aura transcendental e impossível de ignorar
- Padrão forma **coroa flutuante** de cristal multicolorido (não sólida, etérea)
- Tamanho: ~3x do slime
- Luz constante e brilhante, cristais fluem ao redor como constelação
- Som constante de harmonia elemental

#### Tabela de Cores de Aura por Elemento

| Elemento | Cor Primária | Cor Secundária | Padrão Visual |
|:--|:--|:--|:--|
| Nature (Melífera) | Verde vibrante | Dourado | Hexágonos e padrão geométrico |
| Water (Escavarrok) | Azul cristalino | Prateado | Reflexos de espelho, ondas |
| Ice (Nictófila) | Púrpura-Azul | Branco gelo | Padrão de constelação |
| Fire (Escamífero) | Vermelho-Laranja | Dourado | Forma dinâmica (sempre mudando) |
| Shadow (Formicida) | Verde-Roxo | Preto | Pontos sincronizados |
| Dark (Solibrida) | Preto-Roxo | Roxo profundo | Absorve e reflete luz |
| Air (Fulgorante) | Amarelo-Branco | Azul claro | Arcos de energia |
| Earth (Castoro) | Marrom-Verde | Ouro | Textura orgânica |
| Nature Growth (Flores) | Rosa-Verde | Branco | Pétalas flutuantes |
| All Elements (Luminescente) | Multicolorido | Branco puro | Coroa flutuante |

### 2.4 Sistema de Cristais de Pacto

#### O que é um Cristal de Pacto?

Após completar um **Ritual de Reconhecimento** com um monarca, o slime recebe um **Cristal de Pacto** — um objeto simbólico único que não é consumido, mas colecionado. Este cristal é a materialização do pacto entre o slime e o monarca/Espírito Elemental.

#### Cristais de Pacto por Monarca

| Monarca | Cristal | Cor | Forma | Descrição |
|:--|:--|:--|:--|:--|
| Rainha Melífera | Cristal da Harmonia | Dourado | Hexágono perfeito | Brilha em padrão geométrico perfeito |
| Imperador Escavarrok | Cristal das Profundezas | Prateado | Espelho líquido | Reflete toda a luz circundante |
| Imperatriz Nictófila | Cristal Estelar | Roxo-Azul | Forma de constelação | Brilha com luz de estrela lejana |
| Sultan Escamífero | Cristal Acelerado | Vermelho-Laranja | Forma dinâmica | Constantemente em movimento, vibrante |
| Rainha Formicida | Cristal Coletivo | Verde-Roxo | Múltiplos pequenos cristais | Brilham em perfeita sincronia |
| Duquesa Solibrida | Cristal Sombrio | Preto-Roxo | Forma abstrata | Absorve luz mas emite brilho próprio |
| Príncipe Fulgorante | Cristal Elétrico | Amarelo-Branco | Forma zigzag | Arcos de eletricidade pulsam constantemente |
| Conde Castoro | Cristal Madeira | Marrom-Verde | Forma de árvore | Textura orgânica e viva, cresce visualmente |
| Matriarca Flores | Cristal Floral | Rosa-Verde | Forma de flor | Pétalas de cristal flutuam ao redor |
| Luminescente | Cristal Unificado | Multicolorido | Coroa miniatura | Combina harmoniosamente TODAS as cores |

#### Funções do Cristal de Pacto

**No Inventário:**
- Ocupa espaço simbólico especial (não conta para limite)
- Podem ser visualizados como coleção de pactos
- Descrição narra história com aquele monarca
- Não podem ser descartados (permanentes)

**Na Câmara dos Pactos (Lar do Slime):**
- Slime pode **instalar cristais em pedestais especiais**
- Cristais colocados brilham permanentemente
- Cada cristal instalado oferece **pequeno buff** enquanto o slime está próximo:
  - **Melífera:** +10% velocidade de coleta de recursos
  - **Escavarrok:** +5% Defesa permanente
  - **Nictófila:** Visão noturna ativada próximo ao lar
  - **Escamífero:** +8% velocidade de movimento
  - **Formicida:** Aliados ganham +5% de dano
  - **Solibrida:** +3% evasão
  - **Fulgorante:** +10% velocidade de ataque
  - **Castoro:** +15% velocidade de construção/reparo
  - **Flores:** +2 HP regeneração por segundo
  - **Luminescente:** +20% para TODOS os buffs anteriores

**Visual no Lar:**
- Ao instalar 5 cristais: lar fica notavelmente mais brilhante e majestoso
- Ao instalar 10 cristais: lar se transforma em santuário sagrado (cores mudam, aura especial no ambiente)
- Visitantes comentam sobre o lar
- Inimigos sentem-se intimidados ao chegar

**Uso em Narrativa:**
- NPCs comentam sobre cristais visíveis
- "Vejo que você conquistou o reconhecimento de [Monarca]!"
- Diálogos únicos e especiais se você tem certos cristais
- Monarcas reconhecem seus próprios cristais em conversa

### 2.5 Ritual de Reconhecimento (Estrutura de Cutscene)

Cada ritual segue uma estrutura similar, mas personalizada para o monarca:

#### Fases do Ritual

**Fase 1: Prova**
- Slime enfrenta desafio específico do monarca
- Pode ser combat, puzzle, desafio de velocidade, ou narrativo
- Duração: Varia por monarca (5-15 min de gameplay)

**Fase 2: Reconhecimento**
- Monarca emerge em cutscene épica
- Diálogo narrativo explicando POR QUE reconhece o slime
- Importância: Este diálogo estabelece o que o slime provou

**Fase 3: Marcação Elemental**
- Luz envolvendo o slime
- Aura específica do monarca começa a brilhar ao redor
- Som satisfatório de cristal formando
- Duração: 5 segundos de animação

**Fase 4: Entrega do Cristal**
- Cristal de Pacto materializa
- Monarca (via diálogo ou ação): "Que este cristal represente nosso pacto."
- Cristal flutua suavemente até o lar do slime (simbólico)
- Som de cristal depositando

**Fase 5: Título Adquirido**
- Interface exibe: **[TÍTULO ADQUIRIDO: "Nome do Título"]**
- Exemplos: "Protetor da Floresta Calma", "Guardião das Profundezas", etc.
- Som de sucesso, interface brilha
- Slime pode agora usar este título em diálogos

**Fase 6: Volta à Exploração**
- Fade out da cutscene
- HUD atualiza mostrando nova aura visível
- Cristal confirmado no inventário
- Slime sente-se mais poderoso (foco visual na aura)

#### Exemplo Completo: Ritual com Rainha Melífera

```
[Câmera zoom para Floresta Calma]
[Floresta inteira brilha em padrão geométrico dourado]

[Melífera aparece entre flores, envolvida em hexágonos de luz]

MELÍFERA (voz calma e majestosa):
"Você compreendeu a geometria oculta do poder. 
A harmonia não é caos — é ORDEM PERFEITA.
Cada pétala, cada folha, cada cristal... 
segue padrão que o universo sussurra."

[Câmera para o slime, que permanece em silêncio]

MELÍFERA:
"Poucos compreendem. Você compreendeu.
E mais: você respeitou."

[Luz dourada começa a envolver o slime]

[Primeira aura dourada aparece ao redor do slime, formando hexágonos]

[Som de cristal formando, vibração harmônica]

MELÍFERA:
"Que a Harmonia Natural reconheça sua maestria.
Que este reconhecimento brilhe em você para sempre."

[Cristal da Harmonia (hexagonal, dourado) materializa]
[Flutua graciosamente até o lar do slime]
[Som satisfatório de cristal depositando]

[Interface exibe: TÍTULO ADQUIRIDO]
[Interface exibe: "PROTETOR DA FLORESTA CALMA"]

[Slime vê aura dourada pulsante ao seu redor]
[Você pode sentir poder crescer dentro de você]

[Melífera desaparece em padrão de luz]
[Fade out para exploração]
```

### 2.6 Sistema de Seleção de Caminho

No início do jogo, após completar tutorial no Ninho do Slime e conquistar primeira aura, um Espírito Elemental manifesta-se e oferece ao slime uma escolha:

**Opção A: Caminho Clássico (5 Auras)**
- "Prove-se digno ante os Cinco Pilares de Poder. Esta é a jornada tradicional dos líderes."
- Pode conquistar qualquer 5 dos 10 monarcas disponíveis
- Mais acessível, menos tempo necessário
- Reconhecimento dos 5 monarcas conquistados

**Opção B: Caminho Épico (10 Auras)**
- "Ou tome o caminho poucos seguem: domine TODOS os reinos. Torne-se verdadeiramente inigualável."
- Precisa conquistar TODOS os 10 monarcas
- Altamente desafiador, requer dedicação genuína
- Reconhecimento de todos os 10 monarcas + Santuário Eterno desbloqueado
- Aura final forma coroa flutuante

**Opção C: Caminho Livre (Escolher conforme avança)**
- "Ou simplesmente explore. Decida qual caminho seguir enquanto viaja."
- Pode conquistar Auras quando encontra monarcas
- Flexibilidade total
- Decide entre os dois caminhos após coletar 5 Auras

Esta escolha é **reversível** — o jogador pode mudar de ideia após coletar 5 Auras no Caminho Livre.

### 2.7 Câmara dos Pactos (Novo Espaço no Lar)

Após primeira aura, um novo espaço se abre no lar do slime automaticamente:

#### Layout da Câmara dos Pactos

- Sala circular com **10 pedestais** dispostos em círculo
- Centro da sala tem uma plataforma elevada
- Cada pedestal é uma estação para um cristal específico
- Conforme coloca cristais, sala fica mais brilhante progressivamente

#### Visual Progressivo

| Cristais Instalados | Aparência | Efeito | Buffs |
|:--|:--|:--|:--|
| 0 | Sala escura, pedestais vazios, sem luz | Nenhum | Nenhum |
| 1-2 | Luz suave de uma cor, 1-2 cristais brilhando | Cristais emitem aura discreta | +5% buff específico |
| 3-5 | Múltiplas cores, aura harmônica no ar | Aura ativa combina cores | +10% buff, visitantes mais frequentes |
| 6-8 | Espetáculo de luz multicolorido | Padrão complexo de luz | +15% buff, inimigos fraco fogem |
| 9-10 | Santuário sagrado, coroa flutuante em suspensão | Todos os cristais pulsam | +20% buff, presença transcendental |

#### Mecânica de Pedestal

- Slime pode **instalar/remover cristais a qualquer momento**
- Interface permite rotar cristais entre os 10 pedestais
- Alguns layouts específicos desbloqueiam efeitos especiais:
  - **Padrão em Estrela (5 cristais):** +5% para todos os buffs
  - **Padrão Completo (10 cristais):** +20% para todos os buffs
  - **Padrão Harmônico Especial:** Certos arranjos liberam habilidade especial

#### Interação com NPCs

- Visitantes comentam sobre cristais
- Alguns NPCs só visitam se você tiver cristais específicos
- Diálogos únicos baseados em quais cristais você tem
- Inimigos fortes sentem respeito/medo da Câmara

### 2.8 Sistema de Cristais Elementais (Moeda de Habilidades)

Os **Cristais Elementais** não são necessários para evolução, funcionando exclusivamente como **moeda para comprar habilidades** na Árvore de Habilidades (separado dos Cristais de Pacto).

#### Tipos de Cristais Elementais

| Cristal | Elemento | Cor | Uso |
|:--|:--|:--|:--|
| Cristal Verde | Nature | Verde vibrante | Comprar habilidades de Nature |
| Cristal Marrom | Earth | Marrom terroso | Comprar habilidades de Earth |
| Cristal Branco | Air | Branco translúcido | Comprar habilidades de Air |
| Cristal Azul | Water | Azul cristalino | Comprar habilidades de Water |
| Cristal Vermelho | Fire | Vermelho incandescente | Comprar habilidades de Fire |
| Cristal Roxo | Shadow | Roxo etéreo | Comprar habilidades de Shadow |
| Cristal Ciano | Ice | Ciano gélido | Comprar habilidades de Ice |

**IMPORTANTE:** Esses são totalmente separados dos Cristais de Pacto. Um é moeda, outro é coleção.

### 2.9 Árvore de Habilidades

[Mantém conteúdo anterior de v7.2]

### 2.10 Meta Final - Tornar-se o Rei Slime

**Caminho Clássico (5 Auras):**
1. Conquistar 5 Auras Elementais (qualquer combinação dos 10 monarcas)
2. Desenvolver significativamente o lar do slime (pelo menos 3 das 4 expansões)
3. Possuir aliados influentes (nível 5+ de amizade com pelo menos 15 espécies diferentes)

Ao completar esses requisitos, ocorre a **Cerimônia de Coroação dos Cinco**, onde os cinco monarcas conquistados visitam o lar do slime e o reconhecem formalmente como **Rei Slime**, sua aura formando coroa de poder.

**Caminho Épico (10 Auras):**
1. Conquistar TODAS as 10 Auras Elementais
2. Desenvolver completamente o lar do slime (todas as 4 expansões)
3. Alcançar nível 5+ de amizade com TODAS as espécies dos 10 reinos

Ao completar esses requisitos, ocorre a **Grande Cerimônia de Coroação**, onde todos os 10 monarcas visitam o lar do slime simultaneamente. Os Espíritos Elementais manifestam-se directamente. O slime é reconhecido como o **Primeiro Rei Slime Imortal**, sua aura transcendental formando uma coroa flutuante etérea de cristal multicolorido.

---

## 3. Os Dez Monarcas das Montanhas Cristalinas

[Todos os 10 monarcas mantêm descrições anteriores de v7.2 + Título de Pacto adicionado]

### 3.1 Grupo Primário (5 Monarcas - Caminho Clássico)

#### 3.1.1 Rainha Melífera, a Arquiteta Dourada

**Espécie:** Abelha Rainha Cristalina  
**Reino:** As Colmeias Suspensas  
**Bioma:** Floresta Calma (Primavera)  
**Horário Ideal:** Manhã (07:00-11:59)  
**Clima:** Sol Claro  
**Elemento:** Nature + Earth + Air  
**Aura:** Dourado com padrão hexagonal  
**Cristal de Pacto:** Cristal da Harmonia (hexágono perfeito)  
**Título Concedido:** "Protetor da Floresta Calma" ou "Geômetra Sagrado"

**Habitat:**
Estruturas hexagonais gigantes penduradas entre as copas das árvores primaveris. Refletem luz solar em padrões geométricos perfeitos. O ar zumba constantemente com voo coordenado de milhões de abelhas.

**Personalidade:**
- **Perfeccionista Matemática:** Obcecada por eficiência, geometria perfeita e ordem. Fala em porcentagens
- **Workaholic Incansável:** Nunca dorme, sempre supervisionando construções
- **Comunicação Única:** Fala através de "dança das abelhas" — movimentos que subordinadas traduzem
- **Valor Central:** **Ordem através da cooperação perfeita**

**Teste para Reconhecer:**
Construir estrutura geometricamente perfeita que sobreviva a correntes de vento, demonstrando precisão e planejamento.

**Ritual de Reconhecimento:**
[Descrito em seção 2.5]

**Quote Memorável:** *"A primavera não é caos de flores — é MATEMÁTICA pura em cores. Mostrar que você compreende a ordem oculta é aceitar a geometria do poder."*

---

#### 3.1.2 Imperador Escavarrok, o Senhor das Profundezas

[Descrição completa similar - mantém formato]

**Título Concedido:** "Guardião das Profundezas" ou "Navegador do Abismo"

---

#### 3.1.3 Imperatriz Nictófila, a Rainha da Noite Profunda

[Descrição completa similar]

**Título Concedido:** "Conhecedor das Estrelas" ou "Mestre da Escuridão"

---

#### 3.1.4 Sultan Escamífero, o Vencedor de Corridas

[Descrição completa similar]

**Título Concedido:** "Arauto da Velocidade" ou "Portador do Fogo"

---

#### 3.1.5 Rainha Formicida, a Estrategista Coletiva

[Descrição completa similar]

**Título Concedido:** "Pensador Coletivo" ou "Restaurador do Equilíbrio"

---

### 3.2 Grupo Secundário (5 Monarcas Adicionais - Caminho Épico)

#### 3.2.1 Duquesa Solibrida, a Rainha do Escuro

[Descrição completa]

**Título Concedido:** "Sussurrador das Sombras" ou "Vidente do Oculto"

---

#### 3.2.2 Príncipe Fulgorante, o Regente Elétrico

[Descrição completa]

**Título Concedido:** "Veículo da Liberdade" ou "Arauto da Tempestade"

---

#### 3.2.3 Conde Castoro, o Construtor Comunitário

[Descrição completa]

**Título Concedido:** "Arquiteto da Comunidade" ou "Construtor de Ligações"

---

#### 3.2.4 Matriarca Flores, a Guardiã Gentil

[Descrição completa]

**Título Concedido:** "Guardião do Florescimento" ou "Cuidador da Vida"

---

#### 3.2.5 Grão-Sacerdote Luminescente, o Guardião Cristalino

[Descrição completa]

**Título Concedido:** "Primeiro Rei Slime Imortal" ou "Harmonizador dos Elementos"

---

## 4. Mundo do Jogo e Ambientação

[Mantém conteúdo anterior de v7.2 com atualizações menores]

### 4.1 Lore das Montanhas Cristalinas de Aethros

[Mantém lore anterior]

Cada Ritual de Reconhecimento deixa marca permanente no slime, uma Aura Elemental que cresce conforme conquista mais monarcas.

---

## 5. Sistemas Temporais e Climáticos

[Mantém conteúdo anterior - sem mudanças]

## 6. Sistema de Construção e Expansão do Lar

[Mantém conteúdo anterior com adição:]

### 6.5 Câmara dos Pactos (Nova Expansão)

**Desbloqueio:** Automático após primeira aura  
**Função:** Exibir e instalar Cristais de Pacto para buffs  
**Visual:** Sala circular com 10 pedestais cristalinos  
**Efeito:** Buffs progressivos conforme cristais instalados (5%-20%)

---

## 7. Sistema de Inventário Simplificado

[Mantém conteúdo anterior com adição:]

### 7.3 Separação de Cristais

- **Cristais Elementais:** Moeda para habilidades (quantidade exibida)
- **Cristais de Pacto:** Coleção de pactos (lista separada, visual especial)
- Interface claramente diferencia os dois tipos

---

## 8-15. [Seções mantêm conteúdo anterior com atualizações menores]

## 16. Sistema de Salvamento Técnico

### 16.1 Dados Persistentes

**Progressão de Auras:**
- Caminho escolhido (Clássico/Épico/Livre)
- Auras conquistadas (array de 10 bools)
- Títulos adquiridos (array de 10 strings)
- Cristais de Pacto obtidos (array de 10 bools)
- Cristais instalados na Câmara (array de 10 ints - posição de cada pedestal)

**Aura Visual:**
- Cores de aura ativa
- Tamanho de aura atual
- Intensidade de aura
- Padrão visual

[Restante mantém conteúdo anterior]

---

## 17. Performance e Otimização

### 17.3 Otimizações de Aura

- Shader de aura suporta até 10 layers simultâneos
- Blend de cores otimizado
- Partículas pooled para aura
- LOD reduz qualidade de aura em distância

---

## 18. Métricas e Analytics

[Mantém conteúdo anterior com adição:]

- Tempo para primeiro Ritual
- Sequência de monarcas conquistados
- Cristais mais frequentemente instalados
- Duração média de permanência na Câmara dos Pactos

---

## 19. Roadmap de Desenvolvimento Atualizado

### 19.2 Próximas Etapas

**Demo Alpha Completação (Q4 2025)**
- [ ] Implementar sistema de Aura (visual + shader)
- [ ] Implementar sistema de Cristais de Pacto (objetos + UI)
- [ ] Criar Câmara dos Pactos
- [ ] Desenvolver 2 rituais completos (Melífera + Escavarrok)
- [ ] Sistema de Títulos funcional
- [ ] Testes de progressão visual de aura

**Demo Beta (Q1 2026)**
- [ ] Todos os 5 rituais primários implementados
- [ ] Câmara dos Pactos com buffs funcionais
- [ ] Shader de aura otimizado
- [ ] Diálogos dinâmicos baseados em títulos
- [ ] UI de monitoramento de auras

**Demo Next (Q3 2026)**
- [ ] Todos os 10 rituais implementados
- [ ] Ambos os caminhos funcionais
- [ ] Efeitos especiais de layout de cristais
- [ ] Cerimônia de Coroação customizada por caminho
- [ ] Conteúdo pós-game testado

---

**Fim do Game Design Document v7.3**