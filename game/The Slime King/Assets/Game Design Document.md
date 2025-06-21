# The Slime King - Game Design Document (v.5.0)

## Índice
1. [Visão Geral](#visão-geral)
2. [Pilares de Design e Público-Alvo](#pilares-de-design-e-público-alvo)
3. [Mecânicas Principais](#mecânicas-principais)
4. [Sistemas Centrais do Jogo](#sistemas-centrais-do-jogo)
5. [Progressão e Crescimento](#progressão-e-crescimento)
6. [Design de Mundo e Níveis](#design-de-mundo-e-níveis)
7. [Interface do Usuário](#interface-do-usuário)
8. [Áudio e Atmosfera](#áudio-e-atmosfera)
9. [Aspectos Técnicos](#aspectos-técnicos)
10. [Localização e Acessibilidade](#localização-e-acessibilidade)
11. [Plataformas e Performance](#plataformas-e-performance)

---

## Visão Geral

### Conceito Central
**The Slime King** é um jogo de aventura-RPG com visual 2D top-down em pixel art que combina elementos de exploração relaxante, crescimento orgânico do personagem e descoberta de mundo. O jogador controla um raro slime branco recém-nascido em sua jornada de crescimento, desbravando um mundo governado por monstros lendários e extremamente poderosos conhecidos como Reis.

O protagonista inicia sua aventura como um pequeno slime vulnerável, mas através da absorção de fragmentos de energia elemental dropados por inimigos neutralizados e fontes mágicas espalhadas pelo mundo, ele gradualmente cresce em tamanho, poder e sabedoria, até eventualmente se tornar o lendário Rei Slime.

### Premissa Narrativa
A história começa com o pequeno slime acordando em um canto protegido e escuro dentro de uma pequena caverna. Momentos antes, ele teve de se esconder ali após um encontro aterrorizante com uma criatura agressiva que o atacou. A jornada do jogador começa com um objetivo imediato e claro: encontrar uma saída segura e sobreviver, dando o primeiro passo em sua longa jornada de crescimento.

Durante sua aventura, o slime descobrirá que não precisa enfrentar o mundo sozinho. Através de atos de bondade, ajudando criaturas em necessidade e eventualmente desenvolvendo a habilidade única de criar novos slimes companheiros, ele formará uma pequena família que o acompanhará em sua jornada para se tornar o Rei Slime.

### Experiência de Jogo Única
O jogo oferece uma experiência **cozy** e **contemplativa**, onde o jogador pode seguir a aventura no próprio ritmo. Vários desafios e atividades podem ser repetidos com recompensas diferentes, garantindo rejogabilidade sem pressão. Conforme o slime cresce e evolui, novas passagens se abrem em áreas já exploradas, incentivando o retorno a locais familiares para descobrir segredos antes inacessíveis - uma característica fundamental dos jogos estilo Metroidvania.

A mecânica de seguidores adiciona uma camada emocional única, onde as conexões formadas durante a jornada se tornam ferramentas valiosas para resolver desafios cooperativos, reforçando os temas de comunidade e crescimento conjunto.

### Diferencial Competitivo
- **Crescimento Visual Progressivo**: O slime evolui fisicamente de forma visível, mudando não apenas estatísticas mas também sua aparência e tamanho.
- **Sistema de Absorção Elemental**: Mecânica única de coleta e uso de energias mágicas que servem tanto para crescimento quanto para habilidades.
- **Sistema de Seguidores Cooperativos**: Companheiros únicos que auxiliam em puzzles e criam vínculos emocionais.
- **Habilidade de Reprodução**: Capacidade única de criar novos slimes companheiros através de divisão controlada.
- **Exploração Não-Linear**: Mundo interconectado que se expande conforme o jogador progride.
- **Experiência Cozy**: Atmosfera relaxante sem elementos estressantes ou pressão de tempo.

### Inspirações Principais
- **Stardew Valley**: Ritmo relaxante e atividades repetíveis recompensadoras.
- **Hollow Knight**: Exploração metroidvania e progressão de habilidades.
- **Slime Rancher**: Protagonista slime carismático e mecânicas de coleta.
- **Ori and the Blind Forest**: Crescimento orgânico e conexão emocional com o mundo.
- **Pikmin**: Sistema de seguidores cooperativos e puzzles de equipe.

---

## Pilares de Design e Público-Alvo

### Pilares Fundamentais do Gameplay

**1. Exploração Interativa**
O mundo do jogo é rico em elementos interativos e segredos para descobrir. Cada área oferece múltiplas camadas de exploração, desde passagens óbvias até segredos bem escondidos que recompensam curiosidade e persistência. O ambiente responde às ações do jogador de forma consistente e satisfatória.

**2. Coleta e Absorção Elemental**
Diferentes tipos de plantas, inimigos e fontes mágicas depositam cristais elementais que são acumulados para liberar habilidades novas relacionadas a cada elemento. Esta mecânica central conecta combate, exploração e progressão de forma orgânica e intuitiva.

**3. Crescimento Progressivo**
Os cristais coletados são acumulados e permitem que o slime mude para formas maiores e mais fortes. Este crescimento não é apenas numérico - o jogador pode ver fisicamente o slime evoluindo, criando uma conexão emocional forte com a progressão.

**4. Construção de Comunidade**
Através de quests de ajuda e atos de bondade, o slime forma conexões significativas com outras criaturas, algumas das quais se tornam seguidores leais. Esta mecânica reforça os valores cozy de cooperação e cuidado mútuo.

**5. Cooperação Estratégica**
Os seguidores não são apenas companhia - eles participam ativamente na resolução de puzzles e desafios, criando uma camada adicional de estratégia e satisfação quando soluções cooperativas funcionam perfeitamente.

### Público-Alvo

**Público Primário** (70%)
- **Idade**: 16-35 anos
- **Perfil**: Jogadores que apreciam experiências relaxantes e visuais agradáveis.
- **Preferências**: Stardew Valley, Animal Crossing, Ori series, Hollow Knight, Pikmin.
- **Motivações**: Relaxamento após o trabalho, experiências nostálgicas, progressão satisfatória, conexões emocionais com personagens.

**Público Secundário** (30%)
- **Idade**: 13-17 anos e 35+ anos
- **Perfil**: Jogadores casuais e entusiastas de indie games.
- **Preferências**: Pixel art, aventuras 2D, jogos com narrativa envolvente, mecânicas de coleção.

### Categorização
**The Slime King** é classificado como um **jogo de exploração e aventura relaxante** com elementos de **roguelite**, **RPG** e **party management**. A combinação única desses gêneros cria uma experiência que oferece tanto a satisfação da progressão RPG quanto a tranquilidade dos cozy games, enriquecida pela satisfação de formar e liderar uma pequena comunidade de companheiros.

---

## Mecânicas Principais

### Sistema de Movimento Avançado

**Movimentação Base**
O sistema utiliza movimentação fluida em 8 direções usando o novo Input System da Unity. A velocidade de movimento é configurável para diferentes situações e adapta-se automaticamente conforme o slime cresce. O movimento possui física realista que faz o slime "balançar" suavemente, reforçando sua natureza gelatinosa.

**Habilidades de Movimento Especializadas**
- **Squeeze (Apertar)**: Permite passar por frestas pequenas.
- **Bounce (Salto)**: Capacidade de pular sobre obstáculos, com altura aumentando conforme o crescimento.

### Sistema de Combate Cozy

**Filosofia de Combate**
O combate segue uma abordagem cozy, focando em **neutralização** ao invés de destruição. O objetivo é transformar energia hostil em poder elemental, mantendo a atmosfera relaxante do jogo.

**Mecânicas de Combate Principais**
- **Ataques Básicos**: Espaço (teclado) / botão Leste (gamepad) para ataques simples.
- **Dash Attack**: Ataque carregado que serve tanto para movimento quanto combate.
- **Sistema de Parry**: Realizar um ataque sobre o ataque inimigo no momento correto permite contra-ataques.
- **Feedback Visual**: Criaturas atingidas piscam brevemente em vermelho e ficam invulneráveis temporariamente.
- **Knockback**: Todos os golpes deslocam a criatura atingida na direção oposta.

### Sistema de Absorção Elemental

**Tipos de Energia Elemental**
1. **Terra** (Marrom): Aumenta resistência física e permite quebrar obstáculos rochosos.
2. **Água** (Azul): Facilita movimento em áreas aquáticas e permite crescer plantas.
3. **Fogo** (Vermelho): Ilumina áreas escuras e derrete obstáculos de gelo.
4. **Ar** (Branco): Permite planar distâncias maiores e ativar mecanismos eólicos.

**Mecânica de Absorção**
- **Fragmentos Elementais**: Dropados por inimigos neutralizados e fontes mágicas.
- **Atração Automática**: Fragmentos se movem em direção ao slime quando próximos.
- **Efeitos Visuais**: Cada elemento possui animação e som únicos de absorção.
- **Armazenamento**: Energia acumulada é mostrada através de barras coloridas na UI.

### Sistema de Seguidores e Companheiros

**Filosofia dos Seguidores**
Os seguidores representam as conexões emocionais que o slime forma durante sua jornada, servindo tanto como companhia quanto como parceiros cooperativos em desafios específicos.

**Tipos de Seguidores**

**1. Criaturas Resgatadas**
- **Origem**: Obtidos através de quests de ajuda e atos de bondade.
- **Características**: Cada criatura possui habilidades únicas baseadas em sua espécie natural.
- **Exemplos**: Pequenos pássaros (podem ativar interruptores altos), coelhos (podem passar por túneis), peixes mágicos (auxiliam em puzzles aquáticos).

**2. Mini-Slimes Gerados**
- **Origem**: Criados através da habilidade especial "Divisão" do slime principal.
- **Características**: Versões menores do slime principal que mantêm algumas de suas habilidades.
- **Funcionalidade**: Ideais para ativar múltiplos mecanismos simultaneamente ou explorar áreas muito pequenas.

**Mecânicas de Seguidores**
- **Sistema de Seguimento**: Seguidores seguem o slime principal em formação organizada, similar ao sistema "snake" de jogos clássicos.
- **Comandos Básicos**: O jogador pode direcionar seguidores para locais específicos usando botão dedicado.
- **Gestão Automática**: Seguidores se posicionam automaticamente durante interações e combates.
- **Limite de Seguidores**: Máximo de 3-4 seguidores ativos simultaneamente para manter clareza visual e gameplay focado.

**Habilidade de Divisão (Shrink & Spawn)**
- **Ativação**: Disponível a partir do estágio Adult Slime.
- **Mecânica**: O slime principal temporariamente diminui de tamanho e cria 1-2 mini-slimes.
- **Duração**: Mini-slimes permanecem ativos por tempo limitado ou até completar tarefa específica.
- **Custo**: Consome energia elemental significativa para balanceamento.

### Sistema de Interação Avançado

**Mecânicas de Interação**
O personagem pode interagir de formas diferentes com objetos e NPCs. Todos os objetos e NPCs possuem uma área de interação claramente definida, com feedback visual apropriado.

**Feedback Visual de Interação**
- **Outline Effects**: Objetos interativos podem exibir contornos quando o slime se aproxima.
- **Ícones Contextuais**: Alguns objetos mostram ícones indicando o tipo de interação possível.
- **Prompts Dinâmicos**: Botões de interação aparecem contextualmente conforme necessário.
- **Indicadores de Seguidor**: Ícones especiais mostram quando uma interação requer ajuda de seguidores.

**Tipos de Interação**
- **Coleta Automática**: Objetos coletáveis se deslocam automaticamente para o slime.
- **Coleta Manual**: Alguns itens requerem pressionar o botão de interação.
- **Ativação de Mecanismos**: Interruptores, alavancas e dispositivos mágicos.
- **Diálogos**: Conversas com NPCs com sistema de escolhas quando apropriado.
- **Interações Cooperativas**: Puzzles que requerem coordenação entre slime principal e seguidores.

### Sistema de Quests e Ajuda

**Tipos de Quests**
- **Quests de Resgate**: Salvar criaturas em perigo ou situações difíceis.
- **Quests de Coleta**: Encontrar itens específicos para NPCs necessitados.
- **Quests de Transporte**: Escoltar criaturas para locais seguros.
- **Quests de Limpeza**: Remover obstáculos ou purificar áreas contaminadas.

**Sistema de Recompensas**
- **Seguidores Permanentes**: Algumas criaturas se juntam permanentemente ao grupo.
- **Seguidores Temporários**: Ajuda ocasional em áreas específicas.
- **Fragmentos Elementais**: Recompensas em energia para crescimento.
- **Itens Únicos**: Objetos especiais que não podem ser encontrados de outras formas.

### Sistema de Stealth e Crouch

**Mecânica de Crouch**
O slime possui habilidade de se esconder usando um sistema de crouch. Quando ativado (Q no teclado / botão Oeste no gamepad), o personagem se abaixa e uma vinheta visual indica o estado de escondido.

**Stealth Condicional**
Quando escondido próximo a objetos de cobertura como grama alta, o slime torna-se completamente indetectável por inimigos mas não pode se movimentar enquanto se esconde. Esta mecânica permite abordagens mais estratégicas e pacíficas para situações de conflito.

**Stealth Cooperativo**
Seguidores podem também se esconder com o slime principal, mas apenas se houver cobertura suficiente para todo o grupo, adicionando uma camada estratégica ao uso de stealth com companheiros.

### Ambiente Destrutível

**Objetos Destrutíveis**
Alguns objetos do ambiente podem ser atacados e destruídos com um ou mais ataques. Essa mecânica serve tanto para progressão (removendo obstáculos) quanto para coleta de recursos.

**Destruição Cooperativa**
Alguns obstáculos grandes requerem múltiplos ataques simultâneos, sendo necessário coordenar ataques entre o slime principal e seguidores para quebrar barreiras específicas.

**Recompensas de Destruição**
Objetos destruídos podem derrubar itens úteis e cristais elementais, incentivando a exploração ativa do ambiente e oferecendo recursos adicionais para jogadores mais investigativos.

---

## Sistemas Centrais do Jogo

### Sistema de Inventário Evolutivo

**Estrutura do Inventário**
O sistema de inventário cresce junto com o slime, começando com apenas um slot e expandindo até quatro slots conforme o crescimento. Cada slot pode armazenar somente um tipo de item, mas esse item é cumulativo até 10 unidades.

**Mecânica de Slots**
- **Slot Único por Tipo**: Um slot para maçãs, outro para flores, etc.
- **Acumulação**: Até 10 unidades do mesmo item por slot.
- **Seleção Visual**: Slots são exibidos no canto inferior da tela com ícone, quantidade e indicação do slot selecionado.
- **Uso Simples**: Botão dedicado (Left Alt no teclado / botão Norte no gamepad) usa uma unidade do item selecionado.

### Sistema de Gestão de Seguidores

**Interface de Seguidores**
- **Painel de Companheiros**: Área da UI mostrando seguidores ativos com ícones de status.
- **Indicadores de Habilidade**: Cada seguidor mostra ícones das habilidades especiais que possui.
- **Status de Saúde**: Indicadores visuais simples do bem-estar de cada seguidor.

**Mecânicas de Gestão**
- **Formação Automática**: Seguidores se organizam automaticamente atrás do slime principal.
- **Comando de Posição**: Jogador pode direcionar seguidores para pontos específicos.
- **Rotação de Seguidores**: Opção de alternar quais seguidores estão ativos se o limite for excedido.
- **Chamada de Retorno**: Botão para fazer todos os seguidores retornarem rapidamente ao slime principal.

**Sistema de Bem-Estar**
- **Felicidade dos Seguidores**: Sistema simples que afeta eficácia em puzzles.
- **Necessidades Básicas**: Compartilhar itens de comida ocasionalmente mantém seguidores felizes.
- **Descanso**: Seguidores se recuperam automaticamente em pontos de save/descanso.

### Sistema de Status Balanceado

**Atributos Principais**
Todas as criaturas possuem as seguintes características fundamentais:
- **Pontos de Vida**: Determinam a resistência da criatura. Quando zerados, a criatura é neutralizada.
- **Ataque**: Influencia a força dos ataques físicos básicos da criatura.
- **Especial**: Determina a eficácia das habilidades elementais e ataques especiais.
- **Defesa**: Reduz o dano recebido de ataques inimigos, absorvendo pontos de força.
- **Nível**: Valor base que é somado aos demais atributos, criando progressão linear clara.

**Sistema de Condições**
As criaturas podem sofrer condições positivas (buffs) ou negativas (debuffs) que modificam temporariamente suas capacidades, adicionando profundidade estratégica sem complicar excessivamente o sistema.

**Atributos dos Seguidores**
- **Lealdade**: Determina quão rapidamente seguidor responde a comandos.
- **Energia**: Capacidade de participar ativamente em puzzles antes de precisar descansar.
- **Habilidade Especial**: Poder único de cada tipo de seguidor.

### Sistema de Efeitos Visuais

**Efeitos de Pós-Processamento**
Sistema robusto para criar ambientação através de efeitos visuais:
- **Bloom**: Para elementos mágicos e energia elemental.
- **Color Grading**: Ajuste de atmosfera por região.
- **Vignette**: Efeitos contextuais como o estado de stealth.

**Sistema de Partículas**
Feedback visual através de partículas para diversas ações:
- **Absorção Elemental**: Partículas específicas para cada elemento.
- **Crescimento**: Efeitos especiais durante evolução do slime.
- **Interações Ambientais**: Partículas responsivas para elementos do mundo.
- **Atividades de Seguidores**: Efeitos visuais únicos quando seguidores usam habilidades especiais.

**Outlines Adaptativos**
Sistema de contornos que destacam objetos interativos, ajudando na navegação e descoberta sem poluir visualmente a tela.

### Sistema de Puzzles Cooperativos

**Tipos de Puzzles**
- **Ativação Múltipla**: Interruptores que precisam ser pressionados simultaneamente.
- **Peso Combinado**: Plataformas que requerem peso total específico.
- **Habilidades Específicas**: Desafios que exploram habilidades únicas de diferentes seguidores.
- **Sequência Coordenada**: Puzzles que requerem ações em sequência temporal específica.

**Mecânicas de Cooperação**
- **Comando Direto**: Jogador pode direcionar seguidores individualmente para posições.
- **Ação Simultânea**: Botão especial para executar ações coordenadas.
- **Feedback Visual**: Indicadores claros mostram quando cooperação é necessária.

### Sistema de Cutscenes Cinematográficas

**Eventos Dinâmicos**
Sistema robusto para criar momentos narrativos memoráveis:
- **Eventos de Movimento**: Cinematografia dinâmica para sequências de ação.
- **Eventos de Animação**: Momentos dramáticos com animações especializadas.
- **Controle de Câmera**: Direção cinematográfica para cenas importantes.
- **Sistema de Spawn**: Aparição contextual de elementos durante cutscenes.
- **Momentos de União**: Cutscenes especiais quando novos seguidores se juntam ao grupo.

---

## Progressão e Crescimento

### Sistema de Crescimento em 4 Estágios

**Baby Slime (Estágio 1)**
- **Tamanho**: Menor forma, aproximadamente 0.5x do tamanho final.
- **Características**: Pode passar por frestas muito pequenas, mas é vulnerável.
- **Slots de Inventário**: 1 slot disponível.
- **Habilidades Únicas**: Squeeze através de espaços minúsculos.
- **Seguidores**: Não pode ter seguidores permanentes, apenas ajuda temporária ocasional.

**Young Slime (Estágio 2)**
- **Tamanho**: Crescimento para aproximadamente 1x do tamanho final.
- **Características**: Equilibrio entre mobilidade e resistência.
- **Slots de Inventário**: 2 slots disponíveis.
- **Habilidades Novas**: Bounce básico, primeiras habilidades elementais.
- **Seguidores**: Pode ter 1 seguidor permanente, primeiras quests de ajuda disponíveis.

**Adult Slime (Estágio 3)**
- **Tamanho**: Forma robusta, aproximadamente 1.5x do tamanho final.
- **Características**: Maior resistência, acesso a mais áreas.
- **Slots de Inventário**: 3 slots disponíveis.
- **Habilidades Avançadas**: Climb, habilidades elementais intermediárias.
- **Seguidores**: Pode ter 2-3 seguidores permanentes.
- **Habilidade Especial**: Desbloqueia "Divisão" - capacidade de criar mini-slimes temporários.

**Elder Slime/Slime King (Estágio 4)**
- **Tamanho**: Forma majestosa final, 2x do tamanho inicial.
- **Características**: Máximo poder, acesso a todas as áreas.
- **Slots de Inventário**: 4 slots disponíveis.
- **Habilidades Mestras**: Poderes elementais avançados, habilidades únicas de rei.
- **Seguidores**: Pode ter 3-4 seguidores permanentes.
- **Habilidade Suprema**: "Grande Divisão" - pode criar múltiplos mini-slimes ou um slime médio temporário.

### Sistema de Progressão Dual

**Crescimento por Absorção Elemental**
A energia elemental total absorvida determina transições entre estágios de crescimento. Cada estágio requer uma quantidade específica de energia elemental acumulada, independente do tipo.

**Experiência Tradicional**
Paralelamente ao crescimento físico, o slime ganha experiência através de:
- **Exploração**: Descobrir novas áreas e segredos.
- **Neutralização**: Resolver conflitos de forma pacífica.
- **Quebra-cabeças**: Solucionar desafios ambientais.
- **Interações**: Ajudar NPCs e criaturas do mundo.
- **Quests de Ajuda**: Completar missões de resgate e auxílio.
- **Cooperação**: Resolver puzzles com seguidores eficientemente.

### Desbloqueio de Habilidades Elementais

**Sistema de Especialização**
Conforme absorve diferentes tipos de energia elemental, o slime desbloqueia habilidades específicas:

**Habilidades Ativas** (Teclas 1-4 no teclado / LB, LT, RB, RT no gamepad)
O jogador é quem define qual habilidade será utilizada em cada um dos botões.

**Habilidades de Seguidores**
- **Comando de Formação**: Organizar seguidores em diferentes formações.
- **Divisão Menor**: Criar mini-slimes para puzzles específicos.
- **Chamada de Ajuda**: Invocar temporariamente criaturas ajudadas anteriormente.
- **Divisão Maior**: Habilidade suprema de criar múltiplos companheiros simultaneamente.

**Habilidades Passivas**
- **Resistências Elementais**: Proteção contra tipos específicos de dano.
- **Movimentação Aprimorada**: Velocidade e agilidade aumentadas.
- **Regeneração**: Recuperação gradual de vida em determinados ambientes.
- **Liderança Natural**: Seguidores permanecem felizes e eficientes por mais tempo.
- **Comunicação Empática**: Maior chance de sucesso em quests de ajuda.

### Sistema de Relacionamentos

**Vínculos com Seguidores**
- **Nível de Confiança**: Aumenta com tempo juntos e sucessos cooperativos.
- **Afinidade Elemental**: Seguidores podem desenvolver resistências baseadas no slime principal.
- **Memória Compartilhada**: Seguidores lembram de locais visitados e podem sugerir retornos.

**Rede de Favores**
- **Reputação Regional**: Ajudar criaturas melhora reputação em cada região.
- **Favores Acumulados**: Criaturas ajudadas podem fornecer assistência especial mais tarde.
- **Referências**: Criaturas satisfeitas recomendam o slime para outras em necessidade.

---

## Design de Mundo e Níveis

### Estrutura do Mundo Metroidvania

**Filosofia de Design**
O mundo é projetado como um mapa interconectado onde o crescimento do slime literalmente abre novas possibilidades de exploração. Áreas anteriormente inacessíveis tornam-se navegáveis conforme o jogador desenvolve novas habilidades e aumenta de tamanho. A adição de seguidores cria uma terceira dimensão de progressão, onde puzzles cooperativos desbloqueiam áreas que nem o crescimento nem habilidades elementais sozinhos poderiam acessar.

**Princípios de Level Design**
- **Layouts Legíveis**: Caminhos claros e visuais intuitivos que guiam naturalmente o jogador.
- **Múltiplos Caminhos**: Diferentes rotas baseadas no estágio de crescimento e habilidades disponíveis.
- **Segredos Escondidos**: 2-3 segredos por área principal para recompensar exploração detalhada.
- **Pontos de Descanso**: Áreas seguras distribuídas estrategicamente para salvamento e recuperação.
- **Oportunidades Cooperativas**: Puzzles que se beneficiam ou requerem trabalho em equipe.
- **Áreas de Socialização**: Espaços onde NPCs em necessidade podem ser encontrados.

### Regiões Principais

**1. Caverna Inicial (Área Tutorial)**
- **Atmosfera**: Claustrofóbica e um pouco tensa, mas com refúgios seguros de luz.
- **Elementos Dominantes**: Terra.
- **Função**: Introdução às mecânicas de movimento, ataque, stealth e absorção elemental. O objetivo é escapar.
- **Características Especiais**: Passagens estreitas que ensinam a mecânica de *Squeeze*, inimigos básicos (Ratos de Caverna), e a primeira fonte de energia elemental.
- **Seguidores**: Não há oportunidades de seguidores nesta área.

**2. Floresta Sussurrante (Primeira Região Principal)**
- **Atmosfera**: Ambiente acolhedor e vibrante, representando a primeira sensação de liberdade e segurança.
- **Localização**: Aos pés de uma grande montanha, é para onde o slime emerge ao sair da caverna.
- **Elementos Dominantes**: Natureza e Ar.
- **Função**: Primeira área de exploração aberta, introdução a NPCs e ao primeiro crescimento (Baby → Young).
- **Características Especiais**: Desafios de baixa dificuldade, introdução ao sistema de coleta e crafting.
- **Seguidores**: Primeira oportunidade de conseguir seguidor - um pequeno pássaro ferido que pode ser curado.
- **Puzzles Cooperativos**: Interruptores altos que requerem o pássaro para serem ativados.

**3. Cavernas Cristalinas (Segunda Região Principal)**
- **Atmosfera**: Misteriosa mas não intimidante, rica em recursos e segredos.
- **Elementos Dominantes**: Terra e Cristal.
- **Função**: Primeira área de exploração não-linear, com múltiplos caminhos e backtracking.
- **Características Especiais**: Labirintos de cristais, inimigos mais variados.
- **Seguidores**: Oportunidade de conseguir um pequeno ser cristalino que pode iluminar áreas escuras.
- **Puzzles Cooperativos**: Cristais que precisam ser ativados simultaneamente, passagens que requerem iluminação específica.

**4. Lagos Serenos (Terceira Região Principal)**
- **Atmosfera**: Tranquila e contemplativa, com desafios aquáticos.
- **Elementos Dominantes**: Água e gelo natural.
- **Função**: Introdução a mecânicas aquáticas e crescimento intermediário.
- **Características Especiais**: Puzzles de corrente d'água, plataformas flutuantes.
- **Seguidores**: Peixe mágico que pode ativar mecanismos subaquáticos.
- **Puzzles Cooperativos**: Comportas que requerem ativação coordenada, correntes d'água que precisam ser direcionadas.

**5. Picos Ventosos (Quarta Região Principal)**
- **Atmosfera**: Majestosa e aberta, com vista panorâmica.
- **Elementos Dominantes**: Ar e energia eólica.
- **Função**: Desafios de movimentação vertical e habilidades aéreas.
- **Características Especiais**: Correntes de vento, plataformas suspensas.
- **Seguidores**: Pássaro maior que pode carregar mini-slimes para locais altos.
- **Puzzles Cooperativos**: Plataformas eólicas que requerem peso distribuído, mecanismos que precisam ser ativados em diferentes altitudes.

**6. Núcleo Elemental (Região Final)**
- **Atmosfera**: Epicentro de poder mágico, convergência de todos os elementos.
- **Elementos Dominantes**: Todos os elementos em harmonia.
- **Função**: Desafio final e transformação em Slime King.
- **Características Especiais**: Fusão de mecânicas de todas as regiões anteriores.
- **Seguidores**: Todos os seguidores são necessários para puzzles finais complexos.
- **Puzzles Cooperativos**: Desafios épicos que requerem coordenação total entre slime e todos os seguidores.

### Sistema de Quests por Região

**Tipos de NPCs em Necessidade**
- **Criaturas Feridas**: Precisam de cura ou resgate de situações perigosas.
- **Perdidos**: Necessitam escolta para retornar a locais seguros.
- **Famílias Separadas**: Quests para reunir grupos de criaturas dispersas.
- **Guardiões Corrompidos**: Criaturas grandes que precisam ser purificadas ao invés de derrotadas.

**Estrutura de Quests**
- **Identificação**: NPCs em necessidade têm indicadores visuais distintos.
- **Diálogo**: Conversa simples explica a situação e necessidade.
- **Resolução**: Ação específica baseada no problema apresentado.
- **Recompensa**: Seguidor, itens únicos, ou acesso a áreas especiais.

### Elementos Ambientais Interativos

**Sistemas Naturais Responsivos**
- **Plantas Elementais**: Reagem à presença do slime e oferecem recursos.
- **Cristais Ativados**: Respondem a elementos específicos e desbloqueiam passagens.
- **Correntes e Fluxos**: Água, ar e energia que influenciam movimento.
- **Plataformas Dinâmicas**: Superfícies que mudam baseadas no tamanho do slime.

**Mecanismos Cooperativos**
- **Interruptores Múltiplos**: Requerem ativação simultânea por slime e seguidores.
- **Plataformas de Peso**: Necessitam distribuição específica de peso do grupo.
- **Portões Coordenados**: Só abrem quando diferentes membros do grupo ativam mecanismos em sequência.
- **Barreiras Elementais**: Requerem diferentes elementos aplicados simultaneamente.

**Integração com Progressão**
Cada região possui elementos que se tornam totalmente acessíveis apenas quando o slime atinge determinado estágio de crescimento e possui os seguidores adequados, garantindo que o jogador sempre tenha novas descobertas ao revisitar áreas familiares com sua equipe expandida.

---

## Interface do Usuário

### HUD Minimalista e Contextual

**Elementos Permanentes da Interface**
- **Barra de Vida**: Localizada no canto superior esquerdo, com design orgânico que reflete a natureza do slime.
- **Inventário Rápido**: Parte inferior central da tela, mostrando de 1-4 slots conforme crescimento.
- **Barras de Energia Elemental**: Canto superior direito, com cores vibrantes representando cada elemento.
- **Indicador de Crescimento**: Pequeno medidor mostrando progresso até próximo estágio.
- **Painel de Seguidores**: Área compacta mostrando seguidores ativos com ícones de status.

**Elementos Contextuais**
- **Prompts de Interação**: Aparecem dinamicamente próximo a objetos interativos.
- **Controles Virtuais**: Para plataformas mobile, otimizados e não-intrusivos.
- **Indicadores de Progresso**: Mostram temporariamente conquistas e coletas importantes.
- **Comandos de Seguidor**: Aparecem quando ações cooperativas são possíveis.

### Sistema de Menus

**Menu Principal** (Enter no teclado / Start no gamepad)
- **Continuar Jogo**: Retorna ao ponto de save mais recente.
- **Configurações**: Áudio, controles, gráficos e acessibilidade.
- **Progresso**: Estatísticas de jogo e conquistas.
- **Companheiros**: Visualizar histórico de seguidores e relacionamentos.
- **Sair**: Opções de save e saída.

**Menu de Inventário** (Right Shift no teclado / Select no gamepad)
- **Visualização Expandida**: Todos os slots com informações detalhadas.
- **Descrições de Itens**: Textos explicativos para cada item coletado.
- **Estatísticas de Uso**: Histórico de utilização de cada item.

**Menu de Seguidores** (Novo)
- **Lista de Companheiros**: Todos os seguidores ativos e suas habilidades.
- **Histórico de Ajuda**: NPCs ajudados e favores disponíveis.
- **Configurações de Formação**: Opções para organizar seguidores.

### Interface de Seguidores

**Painel de Companheiros Ativos**
- **Ícones Miniaturizados**: Representação visual de cada seguidor ativo.
- **Indicadores de Status**: Saúde, energia e felicidade de cada seguidor.
- **Habilidades Disponíveis**: Ícones mostrando quais habilidades especiais cada seguidor pode usar.

**Comandos Visuais**
- **Mira de Comando**: Cursor especial para direcionar seguidores a posições específicas.
- **Indicadores de Cooperação**: Ícones que aparecem em puzzles sinalizando necessidade de trabalho em equipe.
- **Feedback de Ação**: Confirmação visual quando comandos são executados com sucesso.

### Design Visual da Interface

**Estética Cozy e Pixel Art**
- **Paleta de Cores**: Tons pastéis e cores suaves que complementam o visual do jogo.
- **Bordas Orgânicas**: Elementos de UI com formas arredondadas e naturais.
- **Animações Suaves**: Transições gentis que não quebram a imersão.
- **Consistência Visual**: Todos os elementos seguem o mesmo padrão artístico.
- **Elementos de Comunidade**: UI reflete o aspecto social com ícones calorosos e conectivos.

**Responsividade Multi-Plataforma**
- **Escala Adaptativa**: Interface se ajusta automaticamente a diferentes resoluções.
- **Touch Controls**: Controles virtuais otimizados especificamente para dispositivos móveis.
- **Navegação Universal**: Sistema funciona igualmente bem com teclado, mouse, gamepad e touch.
- **Gestão de Seguidores**: Controles simplificados para dispositivos móveis sem perder funcionalidade.

---

## Áudio e Atmosfera

### Design Sonoro Cozy

**Filosofia do Áudio**
O sistema de áudio foi projetado para criar uma experiência sonora imersiva e dinâmica que reforça a atmosfera cozy e relaxante do jogo. Sons trabalham em harmonia para criar um ambiente acolhedor e responsivo, com elementos adicionais que representam a presença e atividades dos seguidores.

**Trilha Sonora Ambiente**
- **Estilo Musical**: Ambient orchestral com instrumentos orgânicos.
- **Instrumentação**: Piano suave, violão acústico, flauta, cordas delicadas.
- **Dinâmica Emocional**: Música que responde ao estado da cena e ações do jogador.
- **Looping Inteligente**: Transições suaves entre diferentes estados musicais.
- **Harmonias de Grupo**: Camadas musicais adicionais quando seguidores estão presentes.

### Sistema de Áudio Dinâmico

**Gerenciamento Centralizado**
Sistema robusto de gerenciamento de áudio do jogador que coordena todos os elementos sonoros para criar uma experiência coesa e imersiva.

**Camadas Sonoras Integradas**
- **Música de Fundo**: Loops principais para cada região e situação.
- **Efeitos Ambientais**: Sons naturais que criam atmosfera (vento, água, cristais).
- **Feedback de Ações**: Sons responsivos para cada ação do jogador.
- **Efeitos Elementais**: Sons únicos para cada tipo de energia absorvida.
- **Sons de Seguidores**: Efeitos sonoros específicos para cada tipo de companheiro.
- **Harmonias Cooperativas**: Música especial durante puzzles em equipe bem-sucedidos.

**Temas Musicais por Região**
- **Floresta Sussurrante**: Instrumentos acústicos com sons de natureza.
- **Cavernas Cristalinas**: Tons ressonantes com ecos naturais.
- **Lagos Serenos**: Instrumentos aquáticos com sons de água fluindo.
- **Picos Ventosos**: Instrumentos de sopro com atmosfera de altitude.
- **Núcleo Elemental**: Fusão harmoniosa de todos os elementos musicais.

### Efeitos Sonoros Responsivos

**Sons do Slime**
- **Movimento**: Sons gelatinosos suaves que mudam conforme o tamanho.
- **Crescimento**: Sequências musicais ascendentes durante evolução.
- **Absorção**: Tons harmoniosos únicos para cada elemento.
- **Habilidades**: Efeitos sonoros distintos para cada poder elemental.
- **Divisão**: Som especial quando cria mini-slimes.

**Sons dos Seguidores**
- **Movimento em Grupo**: Som coletivo de diferentes tipos de companheiros se movendo juntos.
- **Habilidades Especiais**: Efeito sonoro único para cada habilidade de seguidor.
- **Comunicação**: Sons de interação entre slime e seguidores.
- **Felicidade**: Pequenos sons de contentamento quando seguidores estão felizes.

**Sons de Cooperação**
- **Sucesso em Puzzle**: Harmonia especial quando puzzles cooperativos são resolvidos.
- **Comandos**: Feedback sonoro quando seguidores recebem e executam comandos.
- **Reunião**: Som especial quando novos seguidores se juntam ao grupo.

**Mixagem Dinâmica**
Sistema de mixagem que ajusta automaticamente volumes e prioridades baseado no contexto, garantindo que informações importantes nunca sejam mascaradas por outros sons. O sistema também ajusta a complexidade sonora baseado no número de seguidores ativos.

---

## Aspectos Técnicos

### Arquitetura Unity 6

**Engine e Pipeline**
- **Unity 6**: Versão mais recente com melhorias específicas para jogos 2D.
- **Universal Render Pipeline (URP)**: Otimizado para performance em múltiplas plataformas.
- **2D Renderer**: Especializado para pixel art com suporte a iluminação dinâmica.
- **Input System**: Sistema moderno de entrada com suporte universal.

**Organização de Código**
A arquitetura utiliza organização clara em namespaces funcionais com componentes modulares e reutilizáveis. O sistema de eventos permite comunicação desacoplada entre diferentes sistemas do jogo.

### Sistema de Componentes Modulares

**Componentes Principais do Jogador**
- **PlayerMovement**: Núcleo do controle de movimento e física.
- **PlayerCombat**: Sistema de combate modular e expansível.
- **PlayerVisualManager**: Gerenciamento visual desacoplado para efeitos.
- **PlayerActionController**: Controle de ações especiais e habilidades.
- **PlayerAudioManager**: Sistema de áudio independente e configurável.
- **PlayerGrowth**: Sistema de evolução e transformação do slime.

**Componentes de Seguidores**
- **FollowerManager**: Gerenciamento central de todos os seguidores.
- **FollowerAI**: Comportamento individual de cada seguidor.
- **FollowerAbilities**: Sistema de habilidades especiais dos seguidores.
- **CooperativePuzzleManager**: Coordenação de puzzles que requerem múltiplos participantes.

**Arquitetura de Eventos**
- **Comunicação Desacoplada**: Sistemas se comunicam via eventos sem dependências diretas.
- **Sistema de Callbacks**: Callbacks robustos para diferentes situações de jogo.
- **Gerenciamento de Listeners**: Sistema eficiente para registrar e limpar event listeners.
- **Debug de Eventos**: Ferramentas claras para rastreamento de fluxo de eventos.
- **Eventos de Cooperação**: Sistema especializado para coordenar ações entre slime e seguidores.

### Performance e Otimização

**Target de Performance**
- **120 FPS**: Objetivo principal em PC high-end.
- **60 FPS**: Performance estável em consoles.
- **30 FPS**: Mínimo aceitável em dispositivos móveis.
- **Adaptabilidade**: Sistema se ajusta automaticamente ao hardware disponível.

**Técnicas de Otimização**
- **Object Pooling**: Sistema otimizado para inimigos, projéteis e efeitos.
- **Follower Culling**: Otimização específica para gerenciar múltiplos seguidores.
- **AI Batching**: Processamento otimizado de IA de seguidores.
- **Physics Optimization**: Gerenciamento eficiente de física 2D para grupo.
- **Render Optimization**: Técnicas avançadas de otimização de renderização.
- **Resource Caching**: Cache inteligente de recursos frequentemente usados.

### Sistema de IA dos Seguidores

**Comportamentos Base**
- **Follow Behavior**: Sistema de seguimento inteligente que evita sobreposição.
- **Pathfinding**: Algoritmos otimizados para navegação em grupo.
- **Formation Management**: Manutenção automática de formações organizadas.
- **Collision Avoidance**: Prevenção de colisões entre membros do grupo.

**IA Cooperativa**
- **Puzzle Recognition**: Identificação automática de oportunidades cooperativas.
- **Command Processing**: Interpretação e execução de comandos do jogador.
- **Initiative Taking**: Capacidade limitada de ação independente em situações óbvias.

### Pipeline de Renderização

**URP Customizado**
Pipeline otimizado especificamente para pixel art 2D:
- **Shaders Customizados**: Efeitos visuais otimizados para performance.
- **Pós-Processamento Flexível**: Sistema escalável de efeitos visuais.
- **Batching Inteligente**: Otimização automática de draw calls.
- **Lighting 2D**: Iluminação dinâmica otimizada para atmosfera cozy.
- **Multi-Character Rendering**: Otimizações específicas para renderizar grupos.

### Sistema de Persistência

**Salvamento e Carregamento**
- **Save System Robusto**: Sistema confiável de persistência de dados.
- **Auto-Save**: Salvamento automático em pontos estratégicos.
- **Configurações Personalizadas**: Saves individuais por jogador.
- **Backup System**: Sistema de backup automático para prevenir perda de dados.

**Dados de Seguidores**
- **Estado de Relacionamentos**: Salvamento de vínculos e histórico com NPCs.
- **Progresso de Quests**: Acompanhamento de missões de ajuda completadas.
- **Configurações de Grupo**: Preferências de formação e comandos.

---

## Localização e Acessibilidade

### Suporte Multilíngue

**Idiomas Suportados**
- **Textos Completos**: Português (BR), Inglês, Espanhol, Francês, Japonês, Chinês Simplificado, Russo.
- **Fonte Especializada**: LanaPixel para suporte completo a caracteres ocidentais e orientais.
- **Contexto Cultural**: Adaptações culturais quando necessário para diferentes regiões.

**Sistema de Localização**
- **TextMeshPro Integration**: Sistema completo de fontes com fallbacks.
- **Dynamic Text Loading**: Carregamento dinâmico de textos por idioma.
- **Character Set Optimization**: Otimização de atlas por idioma para melhor performance.

### Características de Acessibilidade

**Acessibilidade Visual**
- **Suporte a Daltônicos**: Padrões visuais distintivos além de diferenciação por cores.
- **Contraste Ajustável**: Opções de alto contraste para diferentes necessidades.
- **Tamanho de Interface**: Escala da UI ajustável entre 100% e 150%.
- **Redução de Efeitos**: Opções para reduzir ou eliminar efeitos visuais intensos.
- **Indicadores de Seguidor**: Opções para tornar seguidores mais visualmente distintos.

**Acessibilidade Auditiva**
- **Legendas Completas**: Para todos os diálogos e efeitos sonoros importantes.
- **Indicadores Visuais**: Substitutos visuais para elementos dependentes de áudio.
- **Controles de Volume**: Mixers independentes para música, efeitos e ambiente.
- **Feedback Visual de Som**: Indicadores visuais para sons de seguidores e cooperação.

**Acessibilidade Motora**
- **Remapeamento Completo**: Todos os controles podem ser personalizados.
- **Simplificação de Controles**: Opções para reduzir complexidade de input.
- **Timing Generoso**: Mecânicas não punitivas com janelas de tempo amplas.
- **Pause Universal**: Capacidade de pausar em qualquer momento.
- **Comandos Simplificados**: Opções para automatizar algumas funções de gestão de seguidores.

**Acessibilidade Cognitiva**
- **Gestão Automática**: Opções para automatizar aspectos de gerenciamento de seguidores.
- **Indicadores Claros**: Sinais visuais óbvios para oportunidades cooperativas.
- **Tutorial Expandido**: Explicações detalhadas das mecânicas de cooperação.

### Configurações de Acessibilidade Avançadas

**Opções Cognitivas**
- **Simplificação de UI**: Interfaces com menos elementos visuais simultâneos.
- **Indicadores de Progresso**: Lembretes claros sobre objetivos e progresso.
- **Tutorial Expandido**: Opções de revisitar tutoriais a qualquer momento.
- **Assistência de Cooperação**: Indicadores automáticos para puzzles que requerem seguidores.

---

## Plataformas e Performance

### Plataformas de Lançamento

**Primeira Onda** (Lançamento Principal)
- **PC**: Steam e Microsoft Store com suporte completo.
- **Nintendo Switch**: Otimizado para gameplay portátil e docked.
- **Xbox Series X|S**: Aproveitando recursos de next-gen.

**Segunda Onda** (Expansão)
- **PlayStation 4/5**: Versões otimizadas para diferentes gerações.
- **Steam Deck**: Otimização específica para dispositivo portátil.

**Terceira Onda** (Mobile)
- **iOS/Android**: Versão adaptada com controles touch otimizados.
- **WebGL**: Versão browser para acessibilidade máxima.

### Configurações Adaptativas

**Adaptação por Plataforma**
Cada plataforma possui configurações específicas otimizadas:
- **PC**: Configurações gráficas escaláveis, suporte para 120 FPS.
- **Console**: Performance estável com efeitos visuais maximizados.
- **Mobile**: Interface touch redesenhada, otimizações de bateria, controles simplificados para seguidores.
- **Web**: Carregamento otimizado e compatibilidade universal.

### Requisitos de Sistema

**PC Mínimo**
- **OS**: Windows 10 64-bit
- **Processor**: Intel i3-6100 / AMD FX-6300
- **Memory**: 4 GB RAM
- **Graphics**: DirectX 11 compatible
- **Storage**: 2 GB available space

**PC Recomendado**
- **OS**: Windows 11 64-bit
- **Processor**: Intel i5-8400 / AMD Ryzen 5 2600
- **Memory**: 8 GB RAM
- **Graphics**: GTX 1060 / RX 580
- **Storage**: 2 GB available space (SSD recommended)

### Otimizações Específicas

**Console Optimization**
- **Switch**: Dynamic resolution scaling, efficient asset streaming, otimização para múltiplos personagens.
- **Xbox**: Smart Delivery para diferentes gerações.
- **PlayStation**: Aproveitamento de recursos específicos como DualSense.

**Mobile Adaptation**
- **Touch Controls**: Interface completamente redesenhada com controles simplificados para seguidores.
- **Battery Management**: Otimizações específicas para prolongar duração da bateria.
- **Performance Scaling**: Ajuste automático baseado no hardware do dispositivo.
- **Follower Simplification**: Redução automática de complexidade de IA em dispositivos menos potentes.

---

## Considerações de Design e Desenvolvimento

### Princípios Fundamentais

**1. Feedback Imediato e Claro**
Todas as ações do jogador recebem feedback visual e sonoro imediato, criando uma sensação de responsividade e conexão com o mundo do jogo.

**2. Controles Responsivos Universais**
O sistema de controles funciona de forma consistente e responsiva em todas as plataformas, garantindo que a experiência seja igualmente satisfatória independente do dispositivo.

**3. Sistemas Integrados e Coerentes**
Todos os sistemas do jogo trabalham em harmonia, criando uma experiência unificada onde cada mecânica reforça e complementa as outras.

**4. Experiência Consistente e Polida**
Manutenção de alta qualidade visual, sonora e de gameplay em todos os aspectos do jogo, desde interações básicas até momentos cinematográficos.

**5. Cooperação Natural e Intuitiva**
O sistema de seguidores deve ser natural e não intrusivo, melhorando a experiência sem adicionar complexidade desnecessária.

**6. Conexões Emocionais Significativas**
Cada seguidor e quest de ajuda deve criar um momento memorável que reforce os temas de comunidade e crescimento.

### Pipeline de Desenvolvimento

**Metodologia Iterativa**
- **Protótipos Rápidos**: Desenvolvimento de protótipos para validar mecânicas.
- **Testes Constantes**: Playtests regulares para refinamento da experiência.
- **Feedback Integration**: Incorporação ativa de feedback da comunidade.
- **Polish Incremental**: Refinamento contínuo de todos os aspectos do jogo.

**Quality Assurance**
- **Multi-Platform Testing**: Testes extensivos em todas as plataformas alvo.
- **Accessibility Testing**: Verificação de recursos de acessibilidade.
- **Performance Profiling**: Monitoramento constante de performance.
- **Localization QA**: Testes específicos para cada idioma suportado.
- **Cooperative Gameplay Testing**: Testes específicos para mecânicas de seguidores e puzzles cooperativos.

### Balanceamento de Mecânicas

**Sistema de Seguidores**
- **Evitar Overwhelm**: Limite de seguidores para manter clareza visual e jogabilidade focada.
- **Progressão Gradual**: Introdução gradual de seguidores para permitir adaptação do jogador.
- **Utilidade Clara**: Cada seguidor deve ter propósito claro e momento para brilhar.
- **Autonomia Balanceada**: Seguidores devem ser úteis sem remover agência do jogador.

**Puzzles Cooperativos**
- **Soluções Óbvias**: Puzzles devem ter soluções intuitivas que celebram a cooperação.
- **Múltiplas Abordagens**: Quando possível, permitir diferentes estratégias de resolução.
- **Feedback Claro**: Indicadores visuais claros sobre quando cooperação é necessária.
- **Graceful Failure**: Puzzles mal-sucedidos devem permitir tentativas fáceis de repetição.

---
