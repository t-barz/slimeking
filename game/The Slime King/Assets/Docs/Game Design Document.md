Claro! Aqui está o GDD reorganizado, com a mecânica de nomeação de seguidores integrada de maneira clara e contextualizada. Cada seção mantém sua explicação introdutória para facilitar o entendimento de quem lê o documento.

# The Slime King – Game Design Document (GDD)

## 1. Conceito e Visão Geral
*Esta seção apresenta a ideia central do jogo, seu universo, narrativa e a experiência que o jogador deve vivenciar.*

### Conceito Central
**The Slime King** é um RPG de aventura 2D top-down em pixel art com elementos de metroidvania, exploração, quebra-cabeças e personalização em um mundo de fantasia habitado por criaturas lendárias chamadas Reis. O jogador controla um slime branco recém-nascido, evoluindo de sua forma mais frágil até se tornar o lendário Rei Slime.

### Premissa Narrativa
*Aqui é detalhado o ponto de partida da história e o contexto emocional do protagonista.*  
A jornada começa com o slime acordando em seu ninho dentro de uma caverna escura. Esse é o ponto de partida para suas aventuras de exploração, sobrevivência e autodescoberta. Durante o percurso, aprende a não enfrentar os perigos sozinho, formando laços e criando uma pequena família de slimes companheiros.

### Experiência de Jogo
*Esta subseção explica o clima e os objetivos do gameplay.*  
O jogo oferece uma atmosfera cozy e contemplativa, sem pressão de tempo, com desafios repetíveis, recompensas variadas e um ciclo de crescimento que incentiva a exploração, a personalização do lar e o retorno a regiões já visitadas. A mecânica de seguidores adiciona cooperação, emoção e vínculo ao longo da jornada.

## 2. Referências Visuais
*Esta seção apresenta exemplos visuais para orientar a equipe de arte sobre a paleta de cores e a interface do jogo.*

### Paleta de Cores – Ambiente de Floresta
*Explica a escolha dos tons para compor cenários de floresta, reforçando o estilo visual cozy e a harmonia do ambiente.*  
A paleta utiliza tons suaves e aconchegantes, servindo de base para vegetação, iluminação e detalhes de interface.

### Sugestão de Interface In-Game
*Apresenta uma referência visual para a interface dentro do jogo.*  
A interface é minimalista, com elementos em pixel art, bordas arredondadas e HUD discreto, exibindo apenas informações essenciais de forma clara e sem poluição visual.

## 3. Configurações Técnicas
*Esta seção define as ferramentas, tecnologias e metas de performance, além das técnicas de otimização necessárias para garantir um jogo fluido e responsivo.*

- **Engine:** Unity 6 com Universal Render Pipeline (URP)
- **Input System:** Moderno e multiplataforma
- **Performance Target:** 120 FPS (PC high-end), 60 FPS (consoles)
- **Otimizações:** Uso de object pooling e resource caching para eficiência

## 4. Personagem, Inimigos e Objetos: Sistema de Atributos
*Esta seção organiza e detalha como os atributos do slime, dos inimigos e dos objetos funcionam, incluindo regras de cálculo, modificadores e progressão.*

### Atributos Básicos

Todos os personagens jogáveis, inimigos e certos objetos do cenário possuem os seguintes atributos fundamentais:

| Atributo               | Descrição                                               | Aplicação                                      |
|:-----------------------|:-------------------------------------------------------|:-----------------------------------------------|
| Pontos de Vida (PV)    | Quantidade de dano que pode receber antes de ser derrotado/destruído | Slime, inimigos, objetos destrutíveis |
| Defesa                 | Reduz o dano recebido de ataques                        | Slime, inimigos, objetos destrutíveis          |
| Ataque Básico          | Valor usado para calcular o dano de ataques normais     | Slime, inimigos                                |
| Ataque Especial        | Valor usado para calcular o dano de ataques especiais   | Slime, inimigos                                |
| Nível                  | Indica progressão do personagem                         | Slime                                          |
| Modificadores          | Alteram atributos temporária ou permanentemente         | Todos                                          |

### Cálculo de Dano

*Explica como o dano é calculado após um ataque, levando em conta atributos e modificadores.*

- Ao receber um ataque, subtrai-se o valor da Defesa do alvo do valor do Ataque do atacante.
- Se o resultado for maior que 0, esse valor é subtraído dos Pontos de Vida do alvo.
- Se o resultado for igual ou menor que 0, o alvo não recebe dano.

> **Fórmula:**  
> Dano Recebido = Máx[(Ataque do Atacante – Defesa do Alvo), 0]

### Modificadores de Atributos

- Todos os atributos podem receber modificadores de diferentes origens, como buffs, debuffs, equipamentos, efeitos temporários e habilidades.
- Modificadores podem ser aditivos (somam ou subtraem valores) ou multiplicativos (aumentam ou reduzem percentualmente o atributo).

### Progressão de Nível (Slime)

- O slime possui um sistema de níveis, aumentando conforme coleta energia elemental.
- Todos os atributos do slime aumentam proporcionalmente ao nível atual.
- O valor de cada atributo no nível atual é calculado multiplicando o valor base do atributo pelo nível do slime.

> **Fórmula:**  
> Atributo no Nível Atual = Valor Inicial Base × Nível Atual

### Evoluções Visuais

| Nível | Alteração Visual                      |
|:-----:|:--------------------------------------|
| 5     | Sprites maiores e mais detalhados     |
| 15    | Sprites ainda maiores, efeitos extras |
| 30    | Máximo detalhamento e tamanho         |

### Resumo dos Atributos

| Entidade              | Pontos de Vida | Defesa | Ataque Básico | Ataque Especial | Nível | Modificadores |
|:----------------------|:--------------:|:------:|:-------------:|:---------------:|:-----:|:-------------:|
| Slime (Jogador)       |      Sim       |  Sim   |      Sim      |      Sim        |  Sim  |     Sim       |
| Inimigos              |      Sim       |  Sim   |      Sim      |      Sim        |  Não  |     Sim       |
| Objetos Destrutíveis  |      Sim       |  Sim   |      Não      |      Não        |  Não  |     Sim       |

## 5. Ninho do Slime e Personalização da Caverna
*Esta seção detalha a mecânica de personalização e crescimento do ninho do slime e da caverna, promovendo sensação de lar, progresso e criatividade.*

### Visão Geral

O ninho do slime é o ponto de partida e lar do personagem dentro da caverna. O jogador pode coletar objetos especiais durante a exploração e utilizá-los para personalizar e aprimorar tanto o ninho quanto a própria caverna, reforçando o aspecto cozy, incentivando a coleta e oferecendo recompensas visuais e funcionais.

### Como Funciona

- **Coleta de Objetos Especiais:**  
  Durante a exploração, o slime encontra itens colecionáveis e recursos (ex: pedras raras, plantas luminosas, artefatos mágicos, mobílias rústicas).
- **Personalização do Ninho:**  
  O jogador pode decorar o ninho com os objetos coletados e alterando visualmente o espaço.
- **Expansão da Caverna:**  
  Certos itens e marcos de evolução permitem ampliar ou desbloquear novas áreas da caverna, como salas adicionais, jardins subterrâneos ou passagens secretas.
- **Interface de Customização:**  
  Um menu dedicado permite ao jogador posicionar, remover ou reorganizar objetos, além de visualizar o progresso de expansão do ninho e da caverna.
- **Feedback Visual e Sonoro:**  
  Cada melhoria ou adição é acompanhada de animações suaves e efeitos sonoros aconchegantes, reforçando a sensação de conquista.

### Tipos de Objetos Colecionáveis

| Tipo de Objeto         | Efeito Visual/Funcional                    | Exemplo de Bônus                |
|:-----------------------|:-------------------------------------------|:--------------------------------|
| Plantas Luminosas      | Iluminação ambiente, decoração             | Recuperação de vida no ninho    |
| Pedras Raras           | Alteram o visual das paredes e do solo     | Defesa extra temporária         |
| Mobílias Rústicas      | Personalizam o espaço do ninho             | Slots extras de inventário      |
| Artefatos Mágicos      | Efeitos visuais especiais                  | Buffs temporários ao descansar  |
| Elementos Temáticos    | Temas de estação ou eventos                | Itens exclusivos e conquistas   |

### Progressão e Desbloqueios

- **Níveis de Ninho:**  
  O ninho pode evoluir visualmente conforme o jogador adiciona objetos e atinge marcos de customização.
- **Desbloqueio de Áreas:**  
  Certos upgrades permitem acesso a novas áreas da caverna, como espaços para seguidores, jardins ou oficinas.

### Integração com Outras Mecânicas

- **Seguidores:**  
  Companheiros slimes podem interagir com objetos do ninho, trazendo vida ao ambiente.
- **Eventos Aleatórios:**  
  Novos visitantes, eventos ou minigames podem ser ativados conforme o ninho evolui.
- **Narrativa:**  
  A expansão do ninho pode desbloquear memórias, histórias ou diálogos únicos.

### Considerações para Implementação

- **Interface Intuitiva:**  
  Garantir que o sistema de customização seja acessível e fácil de usar.
- **Performance:**  
  Otimizar para que múltiplos objetos decorativos não impactem o desempenho.
- **Expansibilidade:**  
  Permitir a adição de novos objetos e temas em futuras atualizações.

## 6. Sistema de Salvamento
*Esta seção detalha a mecânica de salvamento do jogo, garantindo persistência do progresso, flexibilidade ao jogador e integridade do mundo persistente.*

### Visão Geral

O sistema de salvamento de **The Slime King** foi projetado para oferecer segurança, conveniência e controle ao jogador. Ele utiliza múltiplos slots de salvamento manual e um slot de salvamento automático, armazenando não apenas o progresso principal, mas também o estado dinâmico do mundo e do personagem.

### Estrutura dos Slots de Salvamento

- **3 Slots de Salvamento Manual:**  
  O jogador pode salvar o progresso em qualquer um dos três slots disponíveis a partir do menu de pausa, permitindo múltiplas linhas de progresso ou tentativas.
- **1 Slot de Salvamento Automático:**  
  O jogo cria saves automáticos em momentos-chave, garantindo que o jogador não perca progresso importante.

### Eventos de Salvamento Automático

- **Dormir no Ninho:**  
  Sempre que o slime dorme no ninho, o jogo realiza um salvamento automático, registrando o estado completo do progresso.
- **Troca de Cena:**  
  Ao transitar entre áreas/cenas do jogo, o progresso é salvo automaticamente.

### Opções no Menu

- **Salvar Jogo:**  
  O menu principal ou de pausa permite ao jogador escolher um dos três slots manuais para salvar o progresso atual.
- **Carregar Jogo:**  
  O jogador pode selecionar qualquer um dos slots manuais ou o slot automático para retomar o progresso a partir daquele ponto.

### Dados Armazenados no Salvamento

O sistema de salvamento registra e restaura todas as informações essenciais para a continuidade fiel da experiência:

- **Estado do Slime:**  
  - Pontos de vida, defesa, ataque básico e especial, nível, modificadores ativos
  - Posição atual no mundo
  - Poderes e habilidades selecionados
- **Inventário:**  
  - Todos os itens coletados e seus slots
  - Quantidade de itens consumíveis e equipamentos
- **Estado do Mundo:**  
  - Objetos destrutíveis já destruídos
  - Inimigos derrotados
  - Progresso de personalização e expansão do ninho/caverna
  - Eventos importantes já concluídos
- **Seguidores:**  
  - Status, posição, nomes personalizados e atributos dos slimes companheiros
- **Configurações de Interface:**  
  - Preferências do jogador (opcional, para maior conforto)

### Complementos e Boas Práticas

- **Integridade dos Dados:**  
  O sistema deve garantir que saves nunca fiquem corrompidos, utilizando backups temporários e validação antes de sobrescrever dados.
- **Feedback Visual e Sonoro:**  
  Sempre que um salvamento for realizado (manual ou automático), exibir um ícone ou mensagem breve e um efeito sonoro suave para informar o jogador.
- **Confirmação de Sobrescrita:**  
  Ao salvar manualmente, solicitar confirmação caso o slot já esteja ocupado.
- **Prevenção de Exploits:**  
  Salvar o estado de inimigos e objetos destruídos, evitando exploits de reaparecimento ao recarregar saves.
- **Compatibilidade com Atualizações:**  
  Estruturar o sistema para permitir migração de saves em futuras versões do jogo, mantendo a experiência do jogador intacta.

### Fluxo Resumido do Salvamento

1. **Ação do Jogador ou Evento do Jogo** (dormir, trocar de cena, destruir objeto, derrotar inimigo, salvar manualmente)
2. **Coleta e Armazenamento dos Dados** (estado do personagem, mundo, inventário, seguidores)
3. **Confirmação Visual/Sonora** (ícone, mensagem, som)
4. **Recuperação** (ao carregar, restaurar fielmente todos os estados salvos)

## 7. Controles e Interface
*Esta seção detalha o esquema de controles e os princípios de design da interface, garantindo acessibilidade e clareza para o jogador.*

### Esquema de Controle (Gamepad)
*Apresenta o mapeamento dos botões e suas funções, facilitando a compreensão e implementação dos controles.*

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

### Interface Visual
*Explica os princípios visuais da interface, reforçando a identidade cozy e a clareza das informações.*

- **Estética Cozy:** Tons pastéis e suaves (ver referência de paleta)
- **Bordas Orgânicas:** Formas arredondadas e naturais
- **HUD Minimalista:** Exibe apenas informações essenciais
- **Navegação Universal:** Compatível com teclado, mouse, gamepad e touch
- **Animações Suaves:** Transições delicadas sem quebrar a imersão

## 8. Sistema de Combate
*Esta seção apresenta as regras para cálculo de dano, uso de tags, lógica de colisão entre ataques, inimigos e objetos destrutíveis, além dos sistemas de feedback visual e auditivo.*

### Sistema de Tags e Colisões
*Define como as tags são utilizadas para identificar alvos válidos e garantir o funcionamento correto das mecânicas de combate.*

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

### Instanciação de Ataques

- Sempre que slime ou inimigos realizam ataques, um objeto é instanciado com a tag correspondente.
- Ao colidir com um alvo válido, é realizado o cálculo de dano e o objeto de ataque é destruído.

### Sistema de Feedback de Combate

*Detalha as reações visuais e auditivas que ocorrem durante o combate, fornecendo feedback claro ao jogador sobre os resultados dos ataques.*

#### Quando um Ataque Causa Dano

- **Feedback Visual:**  
  - Exibir sprite animado de impacto sobre o alvo
  - Animação de impacto no alvo (sprite piscando, efeito de partículas)
- **Feedback Auditivo:**  
  - Som de impacto específico para cada tipo de ataque
  - Som de dano do alvo (diferente para slime, inimigos e objetos)
- **Knockback (Recuo):**  
  - O alvo é deslocado na direção oposta ao ataque
  - Distância de recuo = fração do dano recebido
  - Duração breve com animação de recuperação

#### Quando um Ataque NÃO Causa Dano

- **Feedback Visual:**  
  - Animação de ataque é interrompida
  - Efeito visual de "bloqueio" ou "resistência" no alvo
- **Feedback Auditivo:**  
  - Som específico de ataque repelido/bloqueado
  - Som metálico ou de resistência
- **Knockback do Atacante:**  
  - O atacante é deslocado para trás
  - Distância de recuo = fração do poder de ataque utilizado
  - Breve animação de recuperação antes de poder atacar novamente

#### Fórmulas de Knockback

- **Knockback do Alvo (quando recebe dano):**  
  Distância = (Dano Recebido × Multiplicador de Knockback) / Peso do Alvo
- **Knockback do Atacante (quando não causa dano):**  
  Distância = (Poder de Ataque × Multiplicador de Repulsão) / Peso do Atacante

### Considerações para Implementação

- **Direção do Knockback:**  
  Calcular direção baseada na posição relativa entre atacante e alvo.
- **Limites de Knockback:**  
  Definir distâncias mínimas e máximas para evitar deslocamentos excessivos.
- **Invencibilidade Temporária:**  
  Após receber dano, o alvo fica brevemente invencível para evitar múltiplos hits.
- **Feedback Diferenciado:**  
  Cada tipo de inimigo, objeto e ataque deve ter feedbacks visuais e sonoros únicos.

## 9. Inimigos
*Esta seção detalha o papel dos inimigos no jogo, suas características principais, comportamentos e como interagem com o protagonista e o ambiente.*

### Papel dos Inimigos
Os inimigos são criaturas e entidades hostis que habitam o mundo de **The Slime King**, representando obstáculos e desafios para o jogador durante a exploração e progressão. Eles contribuem para o dinamismo do gameplay, oferecendo combate, recompensas e oportunidades de evolução.

### Características Gerais

- **Tags:** Todos os inimigos recebem a tag `Enemy`, permitindo fácil identificação para sistemas de combate, IA e lógica de drops.
- **Atributos:** Possuem pontos de vida, defesa, ataque básico e ataque especial, além de poderem receber modificadores temporários ou permanentes.
- **Aparência:** Variedade de designs que refletem o ambiente em que vivem (ex: inimigos de floresta, caverna, ruínas).

### Comportamento

- **IA Básica:** Patrulham áreas, perseguem o slime ao detectá-lo e podem fugir se estiverem em desvantagem.
- **Ataques:** Podem realizar ataques básicos e especiais, instanciando objetos de ataque com as tags `EnemyAttack` ou `EnemySpecialAttack`.
- **Interações Ambientais:** Alguns inimigos podem ativar armadilhas, destruir objetos com a tag `Destructible` ou influenciar o ambiente de outras formas.
- **Drops:** Ao serem derrotados, podem deixar fragmentos de energia elemental, itens ou recursos especiais.

### Exemplos de Tipos de Inimigos

| Tipo de Inimigo      | Comportamento                  | Recompensa Principal              |
|:---------------------|:------------------------------|:----------------------------------|
| Slime Selvagem       | Persegue e ataca corpo a corpo| Energia elemental, itens comuns   |
| Fungo Saltitante     | Salta e tenta bloquear caminhos| Energia elemental, buffs temporários|
| Guardião de Pedra    | Defesa alta, ataque lento      | Itens raros, desbloqueio de áreas |
| Espírito da Floresta | Ataca à distância, foge se ferido| Energia elemental, itens mágicos  |

### Evolução e Desafios

- **Dificuldade Progressiva:** Inimigos tornam-se mais complexos e desafiadores conforme o slime evolui e novas áreas são desbloqueadas.
- **Mini-bosses e Reis:** Alguns inimigos especiais atuam como chefes de área, exigindo estratégias específicas e recompensando com grandes quantidades de energia ou itens únicos.

### Considerações para Implementação

- **Balanceamento:** Ajustar atributos e comportamentos para garantir desafio sem frustração.
- **Variedade:** Introduzir novos inimigos gradualmente para manter o interesse e a sensação de descoberta.
- **Feedback Visual e Sonoro:** Garantir que cada tipo de inimigo tenha animações e sons próprios, reforçando sua identidade e papel no mundo do jogo.

## 10. Sistema de Áudio
*Esta seção descreve o sistema de feedback sonoro, reforçando a imersão e a resposta às ações do jogador.*

| Ação                | Efeito Sonoro                           | Observação                                    |
|:--------------------|:----------------------------------------|:----------------------------------------------|
| Movimentar-se       | Passos, deslizar, variação por terreno  | Volume e tom mudam conforme o piso            |
| Atacar              | Som de ataque básico/especial           | Diferente para cada tipo de ataque            |
| Receber Dano        | Impacto, dano, quebra                   | Sons distintos para slime, inimigos e objetos |
| Coletar Objetos     | Coleta, brilho, pop                     | Feedback rápido e positivo                    |
| Ataque Bloqueado    | Som metálico, resistência               | Quando ataque não causa dano                 |
| Knockback           | Som de deslizamento, impacto            | Acompanha o deslocamento por knockback        |

## 11. Mecânicas de Jogo
*Esta seção apresenta as interações especiais, o sistema de inventário, a mecânica de seguidores e os estados do personagem, detalhando como cada aspecto contribui para a experiência de jogo.*

### 11.1 Interações Especiais (Botão B)
*Explica os diferentes tipos de pontos de interação e suas consequências para o personagem.*

- **Ponto de Esgueirar:** Animação de esgueiro + deslocamento automático até um ponto definido
- **Ponto de Pulo:** Animação de pulo + transporte automático até um ponto definido
- **Ponto de Empurra:** Animação de empurrar + deslocamento de objetos
- **Ponto de Diálogo:** Inicia diálogos com caixa de texto e emoticons

### 11.2 Sistema de Inventário
*Descreve como o jogador pode gerenciar e utilizar itens durante a aventura.*

- **Capacidade:** 4 itens carregáveis
- **Acesso Rápido:** Botões LB, LT, RB, RT para uso rápido
- **Gerenciamento:** Menu dedicado para organizar itens

### 11.3 Mecânica de Seguidores e Nomeação
*Esta subseção detalha como os seguidores funcionam, incluindo a personalização de nomes, reforçando o vínculo emocional e a individualidade dos companheiros.*

#### Visão Geral

O jogador pode recrutar slimes seguidores durante a aventura. Cada seguidor pode receber um nome personalizado, tornando a experiência mais íntima e reforçando laços com a família slime.

#### Como Funciona

- **Nomeação Inicial:**  
  Ao recrutar um novo seguidor, o jogador pode escolher um nome através de uma interface de texto.
- **Renomeação Posterior:**  
  O nome pode ser alterado a qualquer momento pelo menu de gerenciamento de seguidores no ninho ou pelo inventário.
- **Exibição e Integração:**  
  O nome do seguidor aparece em diálogos, interações, menus e ao visualizar detalhes do grupo.
- **Validação:**  
  O sistema impede nomes ofensivos ou inválidos e pode sugerir nomes temáticos.
- **Salvamento:**  
  Os nomes personalizados são armazenados junto ao progresso do jogo.

#### Motivos

- **Vínculo Emocional:**  
  Dar nome aos seguidores aumenta o apego e incentiva o cuidado com cada slime.
- **Identidade e Expressão:**  
  Permite que cada jogador crie uma família única, reforçando a experiência cozy e personalizada.

### 11.4 Estados do Personagem
*Explica os diferentes estados animados e comportamentais do slime.*

- **Idle:** Animação de repouso quando parado
- **Movimento:** Animação ativa durante deslocamento
- **Abaixar:** Esconde atrás de objetos, impede movimento
- **Ataque:** Animações específicas para ataques básicos e especiais
- **Knockback:** Estado temporário após receber dano ou ter ataque repelido

## 12. Diretrizes de Desenvolvimento
*Esta seção reúne recomendações técnicas e práticas para garantir a qualidade, consistência e acessibilidade do projeto.*

- **Modificadores de Atributos:** Suporte a modificadores aditivos e multiplicativos, vindos de buffs, debuffs, equipamentos, efeitos temporários e habilidades.
- **Instanciação de Ataques:** Explica o fluxo de criação, colisão, cálculo de dano e destruição dos objetos de ataque.
- **Sistema de Feedback:** Implementar feedbacks visuais e auditivos claros para todas as interações de combate.
- **Knockback e Física:** Desenvolver sistema de knockback responsivo que melhore a sensação de impacto.
- **Testes de Performance:** Recomenda testes contínuos para garantir estabilidade nos alvos de FPS.
- **Acessibilidade:** Sugere opções de ajuste de escala, contraste e feedback auditivo para ampliar o público.
- **Documentação:** Ressalta a importância de manter padrões visuais e técnicas sempre atualizados.
- **Consistência:** Destaca a necessidade de aplicar tags corretamente para o funcionamento dos sistemas.
