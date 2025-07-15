# The Slime King – Game Design Document (GDD)

## 1. Conceito e Visão Geral

*Esta seção apresenta a ideia central do jogo, seu universo, narrativa e a experiência do jogador.*

### 1.1 Conceito Central

**The Slime King** é um RPG de aventura 2D top-down em pixel art, com metroidvania, exploração, quebra-cabeças e personalização. O jogador controla um slime branco recém-nascido, que evolui até se tornar o lendário Rei Slime em um mundo de fantasia habitado por criaturas chamadas Reis.

### 1.2 Premissa Narrativa

O slime desperta em seu ninho numa caverna escura — ponto de partida para exploração, sobrevivência e autodescoberta. Ao longo da jornada, aprende a formar laços e cria uma família de slimes companheiros.

### 1.3 Experiência de Jogo

Atmosfera cozy e contemplativa, sem pressão de tempo. O jogador encara desafios repetíveis, ganha recompensas variadas e vivencia um ciclo de crescimento que incentiva a exploração, personalização do lar e retorno a regiões antes visitadas. Seguidores fortalecem o vínculo emocional e ampliam o gameplay cooperativo.

## 2. Referências Visuais

*Guias visuais para a arte: paleta de cores e interface.*

### 2.1 Paleta de Cores – Ambiente de Floresta

Tons suaves e aconchegantes, base para vegetação, iluminação e interface, reforçando o estilo cozy e a harmonia visual.

### 2.2 Sugestão de Interface In-Game

Interface minimalista: pixel art, bordas arredondadas e HUD discreto. Exibe apenas informações essenciais de forma clara, sem poluição visual.

## 3. Configurações Técnicas

*Engloba ferramentas, tecnologias, metas de performance e otimizações.*

- **Engine:** Unity 6 + Universal Render Pipeline (URP)
- **Input System:** Moderno e multiplataforma
- **Performance:** 120 FPS (PC high-end), 60 FPS (consoles)
- **Otimizações:** Object pooling, resource caching

## 4. Sistema de Atributos (Personagem, Inimigos e Objetos)

*Organiza atributos, regras de cálculo, modificadores e progressão para slime, inimigos e objetos.*

### 4.1 Atributos Básicos

| Atributo | Descrição | Aplicação |
| :-- | :-- | :-- |
| Pontos de Vida (PV) | Dano suportado antes de derrota | Slime, inimigos, objetos destrutíveis |
| Defesa | Reduz dano recebido | Slime, inimigos, objetos destrutíveis |
| Ataque Básico | Dano de ataques normais | Slime, inimigos |
| Ataque Especial | Dano de ataques especiais | Slime, inimigos |
| Nível | Progressão do personagem principal | Slime |
| Modificadores | Aditivos/multiplicativos, buffs, etc | Todos |

### 4.2 Cálculo de Dano

- Dano Recebido = Máx[(Ataque Atacante – Defesa Alvo), 0]
- Se Dano > 0: subtrai dos PV do alvo. Se ≤ 0: nada acontece.

### 4.3 Modificadores de Atributos

- De origem diversa (buffs, debuffs, equipamentos)
- Podem ser aditivos ou multiplicativos

### 4.4 Progressão de Nível (Slime)

- Nível cresce ao coletar energia elemental
- Todos os atributos: valor base × nível atual

### 4.5 Evoluções Visuais

| Nível | Alteração Visual |
| :--: | :-- |
| 5 | Sprites maiores e mais detalhados |
| 15 | Ainda maiores, efeitos extras |
| 30 | Máximo detalhamento e tamanho |

### 4.6 Resumo dos Atributos

| Entidade | PV | Defesa | Atq. Básico | Atq. Especial | Nível | Mod. |
| :-- | :--: | :--: | :--: | :--: | :--: | :--: |
| Slime (Jogador) | Sim | Sim | Sim | Sim | Sim | Sim |
| Inimigos | Sim | Sim | Sim | Sim | Não | Sim |
| Objetos Destrutíveis | Sim | Sim | Não | Não | Não | Sim |

## 5. Ninho do Slime e Personalização da Caverna

*Detalha a personalização do ninho/caverna e sua integração com progressão, seguidores e recompensas.*

### 5.1 Como Funciona

- **Coleta de Objetos:** Plantas luminosas, pedras raras, artefatos mágicos, mobílias, temas de evento
- **Personalização Visual:** Decoração manual, desbloqueio de bônus (cura, slots, buffs temporários)
- **Expansão:** Novas áreas liberadas (salas, jardins, oficinas)
- **Interface:** Menu para organizar/visualizar objetos e progresso
- **Feedback:** Animações suaves plus sons aconchegantes

### 5.2 Tipos de Objetos Colecionáveis

| Tipo | Visual/Função | Bônus/Buff |
| :-- | :-- | :-- |
| Plantas Luminosas | Iluminação, decoração | Recuperação de vida |
| Pedras Raras | Visual das paredes e solo | Defesa temporária |
| Mobílias Rústicas | Personalização do espaço | Slots extras no inventário |
| Artefatos Mágicos | Efeitos únicos | Buffs temporários |
| Elementos Temáticos | Decoração temática | Itens/conquistas exclusivas |

### 5.3 Progressão e Integração

- Níveis de ninho: desbloqueiam novas áreas, missões, interações
- Seguidores: podem interagir com o ambiente decorado
- Eventos especiais ativados pela expansão

## 6. Sistema de Salvamento

*Define o funcionamento do save: slots, eventos, persistência do mundo e integridade dos dados.*

### 6.1 Estrutura dos Slots

- **3 Slots de Salvamento Manual** (pausa/menu)
- **1 Slot de Salvamento Automático** (eventos-chave)

### 6.2 Eventos de Salvamento Automático

- Dormir no ninho
- Troca de cena

### 6.3 Dados Armazenados

- Estado completo do slime (atributos, posição, poderes)
- Inventário e itens
- Estado do mundo (objetos destruídos, inimigos derrotados, ninho/caverna)
- Seguidores (status, posição, nomes, atributos)
- Config. de interface (opcional)

### 6.4 Boas Práticas

- Backup/validação antes de sobrescrever saves
- Feedback claro de sucesso no salvamento
- Prevenção de exploit: objetos e inimigos não reaparecem após save
- Compatibilidade com atualizações

## 7. Controles e Interface

*Esquema de controles e princípios visuais da UI/UX.*

### 7.1 Esquema de Controle (Gamepad)

| Botão | Função | Descrição |
| :-- | :-- | :-- |
| L Stick | Movimentação | Oito direções |
| D-Pad | Elementos | Dir/Esq: muda elemento, Cima/Baixo: muda ataque especial |
| A | Atacar | Ataque básico |
| B | Interagir | Pontos de ação: itens, diálogos, pontos especiais |
| X | Abaixar | Esconder atrás de objetos (parado enquanto pressionado) |
| Y | Atq Especial | Ataque especial do elemento/habilidade selecionada |
| LB/LT/RB/RT | Itens | Usa itens dos slots 1, 2, 3, 4 |
| Menu | Opções | Configurações, salvar |
| Inventário | Inventário | Gerenciamento e seleção de itens |

### 7.2 Interface Visual

- Cozy: tons suaves e pastéis
- Bordas orgânicas e arredondadas
- HUD minimalista
- Navegação universal (mouse, teclado, gamepad, touch)
- Animações suaves

## 8. Sistema de Combate

*Regras de combate, colisão, feedback do ataque e knockback.*

### 8.1 Sistema de Tags e Colisões

| Tag | Aplicação | Função |
| :-- | :-- | :-- |
| Player | Slime | Identificação do personagem |
| Destructible | Objetos destrutíveis | Para lógica de destruição |
| Enemy | Inimigos | Para lógica de combate e drops |
| Attack | Ataques do slime (básicos) | Combate |
| SpecialAttack | Ataques especiais do slime | Combate |
| EnemyAttack | Ataques básicos de inimigos | Combate |
| EnemySpecialAttack | Ataques especiais de inimigos | Combate |

- Ataques do slime: colidem com Enemy/Destructible
- Ataques dos inimigos: colidem com o slime

### 8.2 Instanciação de Ataques

- Ataques criam objetos temporários
- Após colisão/dano, objeto de ataque é removido

### 8.3 Feedback e Knockback

- **Quando causa dano:**
  - Visual: impacto, partículas, piscadas
  - Auditivo: som de impacto e dano
  - Alvo é deslocado para trás proporcional ao dano
- **Quando não causa dano:**
  - Visual: efeito “bloqueio”, animação interrompida
  - Auditivo: som de resistência, metálico
  - Atacante é repelido (knockback proporcional à força do ataque)
- **Fórmulas:**
  - Knockback alvo: (dano recebido × multiplicador) / peso
  - Knockback atacante: (poder de ataque × multiplicador de repulsão) / peso
- Direção sempre baseada na posição relativa
- Limites de deslocamento definidos para evitar abuso
- Invencibilidade breve após dano (evita múltiplos acertos)

## 9. Inimigos

*Detalha características, comportamentos, drops e papéis dos inimigos.*

### 9.1 Características Gerais

- Tags: sempre "Enemy"
- PV, defesa, ataques, modificadores diversos
- Designs variados conforme ambiente

### 9.2 Comportamento

- IA básica: patrulha, perseguição, fuga quando necessário
- Ataques básicos e especiais, triggers de armadilhas, interação com ambiente
- Drops: energia elemental, itens, buffs/debuffs

### 9.3 Exemplos de Inimigos

| Tipo | Comportamento | Recompensa |
| :-- | :-- | :-- |
| Slime Selvagem | Corpo a corpo, perseguição | Energia elemental, itens |
| Fungo Saltitante | Salta e bloqueia caminhos | Energia, buffs temporários |
| Guardião de Pedra | Defesa alta, ataque lento | Itens raros, desbloqueio |
| Espírito da Floresta | Ataque à distância, foge ferido | Energia, itens mágicos |

### 9.4 Evolução e Desafios

- Dificuldade progressiva conforme avanço do jogador

## 10. Sistema de Áudio

*Reforça a imersão com efeitos e feedback auditivo contextualizado.*

| Ação | Efeito Sonoro | Observação |
| :-- | :-- | :-- |
| Movimentar-se | Passos, deslizar, variação | Volume/tom mudam conforme o piso/piso |
| Atacar | Som de ataque básico/especial | Diferente para cada tipo/estilo de ataque |
| Receber Dano | Impacto, dano, quebra | Sons distintos slime/inimigos/objetos |
| Coletar Objetos | Coleta, brilho, pop | Feedback rápido/positivo |
| Ataque Bloqueado | Som metálico/resistência | Quando ataque não causa dano |
| Knockback | Deslizamento, impacto | Acompanha deslocamento |
| Absorção Fragmentos | Fusão energética, ressonância | Ao absorver fragmentos elementais |

## 11. Mecânicas de Jogo

*Organiza interações especiais, inventário, seguidores, nomeação, drops e estados do personagem.*

### 11.1 Interações Especiais (Botão B)

- Esgueirar: animação + deslocamento automático
- Pulo: animação + transporte automático
- Empurra: animação + deslocamento de objetos
- Diálogo: caixa de texto e emoticons

### 11.2 Sistema de Inventário

- 4 slots de acesso rápido, gerenciamento via menu

### 11.3 Mecânica de Seguidores \& Nomeação

- Jogador recruta slimes seguidores, cada qual pode receber nome personalizado
- Nome é definido ao recrutar ou via menu; impedidos nomes ofensivos
- Nome aparece em diálogos, UI, detalhes do seguidor
- Nomes são salvos e integrados ao progresso do jogador

### 11.4 Sistema de Drops: Itens \& Fragmentos Elementais

- **Ao derrotar inimigos ou destruir objetos:**
  - Pode dropar itens e/ou fragmentos elementais
- **Fragmentos Elementais:**
  - Possuem elemento associado e quantidade de pontos
  - Quando slime se aproxima, fragmentos se deslocam até ele automaticamente
  - Ao contato, são absorvidos (animação/efeito sonoro); aumentam pontos elementais daquele elemento e mesma quantia vale como XP do slime
- **Itens Dropados:**
  - Devem ser coletados manualmente (botão de interação próximo ao item)
  - Efeito visual/sonoro curto ao pegar, adicionando ao inventário

| Tipo de Drop | Coleta Automática? | Interação? | Efeito ao Coletar | O que aumenta |
| :-- | :-- | :-- | :-- | :-- |
| Fragmento Elemental | Sim (atração auto.) | Não | Absorção energética | XP global e pontos elementais |
| Item | Não | Sim | Pop + som de coleta | Inventário |

### 11.5 Estados do Personagem

- Idle, Movimento, Abaixar, Ataque, Knockback (especial)

## 12. Exibição de Sprites do Slime Conforme Direção

*Define as regras visuais para os sub-objetos e efeitos conforme a direção do movimento.*

### 12.1 Estrutura do Slime

- `back`, `vfx_back`, `vfx_front`, `front`, `side`, `vfx_side`, `shadow`

### 12.2 Regras de Exibição

- **Para baixo/parado:** Exibir `front`, `vfx_front` (esconder resto)
- **Para cima:** Exibir `back`, `vfx_back` (esconder resto)
- **Lateral:**
  - Exibir `side`, `vfx_side`
  - Flip horizontal se movendo para a esquerda
- **Sombra:** Sempre exibida

### 12.3 Implementação

- Troca de sprites deve ser suave, conforme direção do movimento
- Flip horizontal só nos sprites laterais à esquerda
- Efeitos visuais (`vfx_*`) seguem a lógica dos sprites principais

## 13. Diretrizes de Desenvolvimento

*Recomenda boas práticas técnicas, de design e acessibilidade.*

- Modificadores robustos em todos os atributos
- Sistema seguro e validado de save/load
- Feedbacks visuais/auditivos claros em todo combate e interação
- Knockback sempre responsivo
- Testes regulares de performance e UX
- Opções de acessibilidade nas interfaces
- Documentação sempre atualizada para novas features ou patches

**Resumo:**
Essas diretrizes e mecânicas centralizam o desenvolvimento de **The Slime King**, entregando uma experiência coerente, confortável, profunda e tecnicamente robusta, ponto de partida ideal para toda a equipe criativa e técnica do projeto.
