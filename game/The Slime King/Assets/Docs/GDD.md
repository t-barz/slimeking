# The Slime King – Game Design Document (GDD)

## 1. Conceito e Visão Geral

*Apresenta a ideia central do jogo, seu universo, narrativa e a experiência que o jogador deve vivenciar.*

### 1.1 Conceito Central

**The Slime King** é um RPG de aventura 2D top-down em pixel art, com elementos de metroidvania, exploração, quebra-cabeças e personalização. O jogador controla um slime branco recém-nascido, que evolui de sua forma mais frágil até se tornar o lendário Rei Slime em um mundo de fantasia habitado por criaturas lendárias chamadas Reis.

### 1.2 Premissa Narrativa

A jornada começa com o slime acordando em seu ninho dentro de uma caverna escura. Esse é o ponto de partida para suas aventuras de exploração, sobrevivência e autodescoberta. Durante o percurso, aprende a não enfrentar os perigos sozinho, formando laços e criando uma pequena família de slimes companheiros.

### 1.3 Experiência de Jogo

O jogo oferece uma atmosfera cozy e contemplativa, sem pressão de tempo, com desafios repetíveis, recompensas variadas e um ciclo de crescimento que incentiva a exploração, a personalização do lar e o retorno a regiões já visitadas. A mecânica de seguidores adiciona cooperação, emoção e vínculo ao longo da jornada.

## 2. Referências Visuais

*Exemplos visuais para orientar a equipe de arte sobre a paleta de cores e a interface do jogo.*

### 2.1 Paleta de Cores – Ambiente de Floresta

Tons suaves e aconchegantes, servindo de base para vegetação, iluminação e detalhes de interface, reforçando o estilo visual cozy e a harmonia do ambiente.

### 2.2 Sugestão de Interface In-Game

Interface minimalista, com elementos em pixel art, bordas arredondadas e HUD discreto, exibindo apenas informações essenciais de forma clara e sem poluição visual.

## 3. Configurações Técnicas

*Define as ferramentas, tecnologias e metas de performance, além das técnicas de otimização necessárias para garantir um jogo fluido e responsivo.*

- **Engine:** Unity 6 com Universal Render Pipeline (URP)
- **Input System:** Moderno e multiplataforma
- **Performance Target:** 120 FPS (PC high-end), 60 FPS (consoles)
- **Otimizações:** Uso de object pooling e resource caching para eficiência

## 4. Sistema de Atributos: Personagem, Inimigos e Objetos

*Organiza e detalha como os atributos do slime, dos inimigos e dos objetos funcionam, incluindo regras de cálculo, modificadores e progressão.*

### 4.1 Atributos Básicos

| Atributo               | Descrição                                               | Aplicação                                      |
|:-----------------------|:-------------------------------------------------------|:-----------------------------------------------|
| Pontos de Vida (PV)    | Quantidade de dano que pode receber antes de ser derrotado/destruído | Slime, inimigos, objetos destrutíveis |
| Defesa                 | Reduz o dano recebido de ataques                        | Slime, inimigos, objetos destrutíveis          |
| Ataque Básico          | Valor usado para calcular o dano de ataques normais     | Slime, inimigos                                |
| Ataque Especial        | Valor usado para calcular o dano de ataques especiais   | Slime, inimigos                                |
| Nível                  | Indica progressão do personagem                         | Slime                                          |
| Modificadores          | Alteram atributos temporária ou permanentemente         | Todos                                          |

### 4.2 Cálculo de Dano

- Subtrai-se a Defesa do alvo do Ataque do atacante.
- Se o resultado for maior que 0, subtrai-se esse valor dos Pontos de Vida do alvo.
- Se o resultado for igual ou menor que 0, o alvo não recebe dano.

**Fórmula:**  
Dano Recebido = Máx[(Ataque do Atacante – Defesa do Alvo), 0]

### 4.3 Modificadores de Atributos

- Podem ser aditivos (somam/subtraem valores) ou multiplicativos (aumentam/reduzem percentualmente).
- Origem: buffs, debuffs, equipamentos, efeitos temporários, habilidades.

### 4.4 Progressão de Nível (Slime)

- O slime sobe de nível ao coletar energia elemental.
- Todos os atributos aumentam proporcionalmente ao nível atual.
- **Fórmula:**  
  Atributo no Nível Atual = Valor Inicial Base × Nível Atual

### 4.5 Evoluções Visuais

| Nível | Alteração Visual                      |
|:-----:|:--------------------------------------|
| 5     | Sprites maiores e mais detalhados     |
| 15    | Sprites ainda maiores, efeitos extras |
| 30    | Máximo detalhamento e tamanho         |

### 4.6 Resumo dos Atributos

| Entidade              | Pontos de Vida | Defesa | Ataque Básico | Ataque Especial | Nível | Modificadores |
|:----------------------|:--------------:|:------:|:-------------:|:---------------:|:-----:|:-------------:|
| Slime (Jogador)       |      Sim       |  Sim   |      Sim      |      Sim        |  Sim  |     Sim       |
| Inimigos              |      Sim       |  Sim   |      Sim      |      Sim        |  Não  |     Sim       |
| Objetos Destrutíveis  |      Sim       |  Sim   |      Não      |      Não        |  Não  |     Sim       |

## 5. Ninho do Slime e Personalização da Caverna

*Detalha a mecânica de personalização e crescimento do ninho do slime e da caverna, promovendo sensação de lar, progresso e criatividade.*

### 5.1 Como Funciona

- **Coleta de Objetos Especiais:** Itens colecionáveis e recursos encontrados durante a exploração.
- **Personalização do Ninho:** Decoração visual e desbloqueio de bônus (ex: recuperação, slots extras, buffs temporários).
- **Expansão da Caverna:** Ampliação e desbloqueio de novas áreas (salas, jardins, passagens secretas).
- **Interface de Customização:** Menu para posicionar, remover, reorganizar objetos e visualizar progresso de expansão.
- **Feedback Visual/Sonoro:** Animações suaves e efeitos sonoros aconchegantes a cada melhoria.

### 5.2 Tipos de Objetos Colecionáveis

| Tipo de Objeto         | Efeito Visual/Funcional                    | Exemplo de Bônus                |
|:-----------------------|:-------------------------------------------|:--------------------------------|
| Plantas Luminosas      | Iluminação ambiente, decoração             | Recuperação de vida no ninho    |
| Pedras Raras           | Alteram o visual das paredes e do solo     | Defesa extra temporária         |
| Mobílias Rústicas      | Personalizam o espaço do ninho             | Slots extras de inventário      |
| Artefatos Mágicos      | Efeitos visuais especiais                  | Buffs temporários ao descansar  |
| Elementos Temáticos    | Temas de estação ou eventos                | Itens exclusivos e conquistas   |

### 5.3 Progressão e Integração

- **Níveis de Ninho:** Evolução visual conforme marcos de customização.
- **Desbloqueio de Áreas:** Novos espaços para seguidores, jardins ou oficinas.
- **Seguidores:** Interagem com objetos do ninho.
- **Eventos/Narrativa:** Expansão pode desbloquear memórias, histórias ou diálogos únicos.

## 6. Sistema de Salvamento

*Garante persistência do progresso, flexibilidade ao jogador e integridade do mundo persistente.*

### 6.1 Estrutura dos Slots

- **3 Slots de Salvamento Manual:** Salve em qualquer slot pelo menu de pausa.
- **1 Slot de Salvamento Automático:** Saves automáticos em eventos-chave.

### 6.2 Eventos de Salvamento Automático

- Dormir no ninho
- Troca de cena

### 6.3 Dados Armazenados

- Estado do slime (atributos, posição, poderes)
- Inventário (itens, slots, quantidades)
- Estado do mundo (objetos destruídos, inimigos derrotados, progresso do ninho/caverna)
- Seguidores (status, posição, nomes personalizados, atributos)
- Configurações de interface (opcional)

### 6.4 Boas Práticas

- Backups temporários, validação antes de sobrescrever saves
- Feedback visual/sonoro ao salvar
- Confirmação ao sobrescrever saves manuais
- Prevenção de exploits (objetos/inimigos não reaparecem)
- Compatibilidade com atualizações futuras

## 7. Controles e Interface

*Detalha o esquema de controles e os princípios de design da interface, garantindo acessibilidade e clareza para o jogador.*

### 7.1 Esquema de Controle (Gamepad)

| Botão | Função | Descrição |
|:------|:-------|:----------|
| **L Stick** | Movimentação | Move o slime em oito direções |
| **D-Pad** | Elementos/Ataques | Direita/Esquerda: muda elemento; Cima/Baixo: muda ataque especial |
| **A** | Atacar | Executa ataque básico |
| **B** | Interagir | Interage com pontos do cenário |
| **X** | Abaixar | Esconde atrás de objetos (não pode se mover) |
| **Y** | Ataque Especial | Executa ataque especial do elemento selecionado |
| **LB/LT/RB/RT** | Usar Itens | Usa itens dos slots 1, 2, 3 e 4 respectivamente |
| **Menu** | Opções | Abre menu de configurações e salvar |
| **Inventário** | Inventário | Gerencia itens e slots |

### 7.2 Interface Visual

- **Estética Cozy:** Tons pastéis e suaves
- **Bordas Orgânicas:** Formas arredondadas e naturais
- **HUD Minimalista:** Apenas informações essenciais
- **Navegação Universal:** Compatível com teclado, mouse, gamepad e touch
- **Animações Suaves:** Transições delicadas, sem quebrar imersão

## 8. Sistema de Combate

*Regras para cálculo de dano, uso de tags, lógica de colisão entre ataques, inimigos e objetos destrutíveis, além dos sistemas de feedback visual e auditivo.*

### 8.1 Sistema de Tags e Colisões

| Tag                  | Aplicação                        | Função                                          |
|:---------------------|:---------------------------------|:------------------------------------------------|
| **Player**           | O personagem jogador             | Identifica objetos que representam o slime      |
| **Destructible**     | Objetos destrutíveis             | Identifica objetos que podem ser destruídos      |
| **Enemy**            | Inimigos                         | Identifica entidades hostis ao jogador           |
| **Attack**           | Ataques do slime                 | Ataques básicos do jogador                      |
| **SpecialAttack**    | Ataques especiais do slime       | Ataques especiais do jogador                    |
| **EnemyAttack**      | Ataques básicos de inimigos      | Ataques básicos dos inimigos                    |
| **EnemySpecialAttack**| Ataques especiais de inimigos   | Ataques especiais dos inimigos                  |

- **Ataques do Slime:** Attack/SpecialAttack colidem com Enemy/Destructible
- **Ataques dos Inimigos:** EnemyAttack/EnemySpecialAttack colidem com o slime

### 8.2 Instanciação de Ataques

- Sempre que slime ou inimigos realizam ataques, um objeto é instanciado com a tag correspondente.
- Ao colidir com um alvo válido, é realizado o cálculo de dano e o objeto de ataque é destruído.

### 8.3 Feedback de Combate

#### Quando um Ataque Causa Dano

- **Visual:** Sprite animado de impacto, animação de impacto (piscando, partículas)
- **Auditivo:** Som de impacto específico para cada tipo de ataque/dano
- **Knockback:** O alvo é deslocado na direção oposta ao ataque (distância proporcional ao dano)

#### Quando um Ataque NÃO Causa Dano

- **Visual:** Animação interrompida, efeito de "bloqueio" ou "resistência"
- **Auditivo:** Som de ataque repelido/bloqueado, metálico ou de resistência
- **Knockback do Atacante:** O atacante é deslocado para trás (distância proporcional ao poder de ataque)

#### Fórmulas de Knockback

- **Alvo:** Distância = (Dano Recebido × Multiplicador de Knockback) / Peso do Alvo
- **Atacante:** Distância = (Poder de Ataque × Multiplicador de Repulsão) / Peso do Atacante

#### Considerações

- Direção baseada na posição relativa entre atacante e alvo
- Limites mínimos/máximos de knockback
- Invencibilidade temporária após dano
- Feedback diferenciado para cada tipo de inimigo, objeto e ataque

## 9. Inimigos

*Detalha o papel dos inimigos, suas características, comportamentos e como interagem com o protagonista e o ambiente.*

### 9.1 Características Gerais

- **Tags:** Todos os inimigos recebem a tag `Enemy`
- **Atributos:** Pontos de vida, defesa, ataque básico e especial, modificadores temporários ou permanentes
- **Aparência:** Designs variados conforme o ambiente

### 9.2 Comportamento

- **IA Básica:** Patrulham áreas, perseguem o slime, podem fugir se estiverem em desvantagem
- **Ataques:** Básicos e especiais, instanciando objetos de ataque com as tags `EnemyAttack` ou `EnemySpecialAttack`
- **Interações Ambientais:** Ativam armadilhas, destroem objetos, influenciam o ambiente
- **Drops:** Energia elemental, itens ou recursos especiais ao serem derrotados

### 9.3 Exemplos de Inimigos

| Tipo de Inimigo      | Comportamento                  | Recompensa Principal              |
|:---------------------|:------------------------------|:----------------------------------|
| Slime Selvagem       | Persegue e ataca corpo a corpo| Energia elemental, itens comuns   |
| Fungo Saltitante     | Salta e tenta bloquear caminhos| Energia elemental, buffs temporários|
| Guardião de Pedra    | Defesa alta, ataque lento      | Itens raros, desbloqueio de áreas |
| Espírito da Floresta | Ataca à distância, foge se ferido| Energia elemental, itens mágicos  |

### 9.4 Evolução e Desafios

- Dificuldade progressiva conforme o slime evolui e novas áreas são desbloqueadas
- Mini-bosses e Reis como chefes de área, exigindo estratégias específicas

## 10. Sistema de Áudio

*Descreve o sistema de feedback sonoro, reforçando a imersão e a resposta às ações do jogador.*

| Ação                | Efeito Sonoro                           | Observação                                    |
|:--------------------|:----------------------------------------|:----------------------------------------------|
| Movimentar-se       | Passos, deslizar, variação por terreno  | Volume e tom mudam conforme o piso            |
| Atacar              | Som de ataque básico/especial           | Diferente para cada tipo de ataque            |
| Receber Dano        | Impacto, dano, quebra                   | Sons distintos para slime, inimigos e objetos |
| Coletar Objetos     | Coleta, brilho, pop                     | Feedback rápido e positivo                    |
| Ataque Bloqueado    | Som metálico, resistência               | Quando ataque não causa dano                  |
| Knockback           | Som de deslizamento, impacto            | Acompanha o deslocamento por knockback        |

## 11. Mecânicas de Jogo

*Apresenta as interações especiais, o sistema de inventário, a mecânica de seguidores e os estados do personagem, detalhando como cada aspecto contribui para a experiência de jogo.*

### 11.1 Interações Especiais (Botão B)

- **Ponto de Esgueirar:** Animação de esgueiro + deslocamento automático até um ponto definido
- **Ponto de Pulo:** Animação de pulo + transporte automático até um ponto definido
- **Ponto de Empurra:** Animação de empurrar + deslocamento de objetos
- **Ponto de Diálogo:** Inicia diálogos com caixa de texto e emoticons

### 11.2 Sistema de Inventário

- **Capacidade:** 4 itens carregáveis
- **Acesso Rápido:** Botões LB, LT, RB, RT para uso rápido
- **Gerenciamento:** Menu dedicado para organizar itens

### 11.3 Mecânica de Seguidores e Nomeação

*Permite ao jogador recrutar slimes seguidores e dar nomes personalizados, reforçando vínculo emocional e identidade dos companheiros.*

- **Nomeação Inicial:** Ao recrutar um novo seguidor, escolha de nome via interface de texto
- **Renomeação Posterior:** Nome pode ser alterado a qualquer momento pelo menu de seguidores ou inventário
- **Exibição:** Nome aparece em diálogos, interações, menus e detalhes do grupo
- **Validação:** Sistema impede nomes ofensivos ou inválidos e pode sugerir nomes temáticos
- **Salvamento:** Nomes personalizados são armazenados junto ao progresso
- **Motivo:** Aumenta o apego, incentiva o cuidado e reforça a experiência cozy e personalizada

### 11.4 Estados do Personagem

- **Idle:** Animação de repouso quando parado
- **Movimento:** Animação ativa durante deslocamento
- **Abaixar:** Esconde atrás de objetos, impede movimento
- **Ataque:** Animações específicas para ataques básicos e especiais
- **Knockback:** Estado temporário após receber dano ou ter ataque repelido

## 12. Exibição de Sprites do Slime Conforme Direção

*Define as regras de exibição e ocultação dos diferentes sprites e efeitos visuais do objeto Slime, garantindo clareza visual e coerência na animação conforme a direção do movimento.*

### 12.1 Estrutura do Objeto Slime

- `back`
- `vfx_back`
- `vfx_front`
- `front`
- `side`
- `vfx_side`
- `shadow`

### 12.2 Regras de Exibição por Direção

- **Sul (Para Baixo) ou Parado (Padrão):**
  - Exibir: `front`, `vfx_front`
  - Ocultar: `back`, `vfx_back`, `side`, `vfx_side`
- **Norte (Para Cima):**
  - Exibir: `back`, `vfx_back`
  - Ocultar: `front`, `vfx_front`, `side`, `vfx_side`
- **Lateral (Esquerda/Direita):**
  - Exibir: `side`, `vfx_side`
  - Ocultar: `front`, `vfx_front`, `back`, `vfx_back`
  - **Flip Horizontal:** Ao mover para a esquerda, aplicar flip horizontal em `side` e `vfx_side`; para a direita, exibir normalmente.
- **Sombra:** `shadow` deve ser exibido em todas as direções.

### 12.3 Considerações para Implementação

- Troca de sprites deve ser instantânea e suave, acompanhando a direção do movimento.
- Apenas sub-objetos relevantes ativos em cada direção.
- Flip horizontal aplicado apenas nos sprites laterais ao mover para a esquerda.
- Efeitos visuais (`vfx_*`) seguem as mesmas regras dos sprites principais.

## 13. Diretrizes de Desenvolvimento

*Recomendações técnicas e práticas para garantir a qualidade, consistência e acessibilidade do projeto.*

- **Modificadores de Atributos:** Suporte a modificadores aditivos e multiplicativos, vindos de buffs, debuffs, equipamentos, efeitos temporários e habilidades.
- **Instanciação de Ataques:** Fluxo de criação, colisão, cálculo de dano e destruição dos objetos de ataque.
- **Sistema de Feedback:** Feedbacks visuais e auditivos claros para todas as interações de combate.
- **Knockback e Física:** Sistema responsivo de knockback para sensação de impacto.
- **Testes de Performance:** Testes contínuos para garantir estabilidade nos alvos de FPS.
- **Acessibilidade:** Opções de ajuste de escala, contraste e feedback auditivo.
- **Documentação:** Manter padrões visuais e técnicas sempre atualizados.
- **Consistência:** Aplicação correta de tags para funcionamento dos sistemas.

[1] <https://pplx-res.cloudinary.com/image/private/user_uploads/27861941/83356ea3-c58d-47a8-b90a-eec901a6fcab/image.jpg>
