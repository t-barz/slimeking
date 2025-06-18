# Slime King - Game Design Document

## Índice
1. [Visão Geral](#visão-geral)
2. [Mecânicas Principais](#mecânicas-principais)
3. [Sistemas do Jogo](#sistemas-do-jogo)
4. [Interface do Usuário](#interface-do-usuário)
5. [Áudio](#áudio)
6. [Aspectos Técnicos](#aspectos-técnicos)

## Visão Geral

Slime King é um jogo 2D que combina elementos de aventura, stealth e combate. O jogador controla um personagem que pode se movimentar, atacar, se esconder e interagir com o ambiente. A experiência de jogo é focada em proporcionar uma mistura equilibrada entre exploração, combate estratégico e resolução de puzzles ambientais.

### Pilares do Gameplay
- **Exploração interativa**: O mundo do jogo é rico em elementos interativos e segredos para descobrir
- **Combate estratégico**: Sistema de combate que recompensa timing e posicionamento
- **Mecânicas de stealth**: Capacidade de se esconder e evitar confrontos
- **Sistema de inventário**: Gerenciamento de recursos e itens coletados

## Mecânicas Principais

### Descrição Geral
As mecânicas principais do Slime King foram projetadas para criar uma experiência de jogo fluida e satisfatória. O sistema de movimento permite controle preciso do personagem, enquanto o combate oferece profundidade estratégica. A mecânica de stealth adiciona uma camada extra de escolhas táticas, permitindo que os jogadores alternem entre abordagens agressivas e furtivas.

### Sistema de Movimento
- Movimentação fluida em 8 direções usando o novo Input System da Unity
- Sistema de física baseado em Rigidbody2D para movimento realista
- Velocidade de movimento configurável para diferentes situações
- Sistema de animação responsivo que reflete a direção do movimento
- Suporte para controles mobile e desktop

### Sistema de Combate
- Ataques com animações específicas e áreas de efeito
- Sistema de dano com diferentes tipos de ataques
- Efeitos visuais de combate para feedback claro
- Estados de ataque que impedem movimento para decisões táticas
- Sistema de status effects para variedade estratégica

### Mecânica de Stealth
- Habilidade de se esconder com sistema de crouch
- Efeito visual de vinheta que indica o estado de stealth
- Desativação de colisores durante stealth para atravessar certas áreas
- Integração com objetos do ambiente para criar oportunidades de stealth

### Sistema de Interação
- Interação contextual com objetos do mundo
- Coletáveis que afetam o gameplay
- NPCs interativos com diálogos e missões
- Objetos do ambiente destrutíveis e interativos
- Sistema de outline para identificação clara de interativos

## Sistemas do Jogo

### Descrição Geral
Os sistemas do jogo trabalham em conjunto para criar uma experiência coesa e envolvente. Cada sistema foi projetado para complementar os outros, criando uma teia de mecânicas interconectadas que dão profundidade ao gameplay.

### Sistema de Inventário
- Gerenciamento intuitivo de itens coletáveis
- Sistema de energia para limitar certas ações
- Interface de inventário clara e funcional
- Categorização de itens por tipo e uso

### Sistema de Cutscenes
- Sistema robusto de eventos de cutscene
- Eventos de movimento para cinematografia dinâmica
- Eventos de animação para momentos dramáticos
- Controle de câmera para cenas cinematográficas
- Sistema de spawn para momentos memoráveis

### Sistema de Efeitos Visuais
- Efeitos de pós-processamento para ambientação
- Sistema de partículas para feedback visual
- Outlines adaptativos para objetos interativos
- Shaders customizados para efeitos únicos

## Interface do Usuário

### Descrição Geral
A interface do usuário foi projetada para ser intuitiva e não-intrusiva, fornecendo informações importantes sem sobrecarregar a tela. Os elementos de UI se adaptam ao contexto e à plataforma, garantindo uma experiência consistente em diferentes dispositivos.

### HUD
- Controles virtuais otimizados para mobile
- Indicadores contextuais de interação
- Interface de inventário acessível
- Prompts de botões claros e responsivos

### Controles
- Suporte completo para o novo Input System
- Controles mobile com joystick virtual e botões
- Sistema de mapeamento de teclas flexível
- Detecção automática de dispositivos de entrada

## Áudio

### Descrição Geral
O sistema de áudio foi projetado para criar uma experiência sonora imersiva e dinâmica. Os efeitos sonoros e a música trabalham em conjunto para reforçar as ações do jogador e criar atmosfera.

### Sistema de Áudio
- Gerenciamento centralizado de áudio do jogador
- Efeitos sonoros responsivos para cada ação
- Música ambiente adaptativa
- Sistema de mixagem dinâmica

## Aspectos Técnicos

### Descrição Geral
A arquitetura técnica do jogo foi desenvolvida pensando em performance, manutenibilidade e escalabilidade. O código é organizado em namespaces lógicos e utiliza padrões de design modernos.

### Arquitetura
- Organização clara em namespaces funcionais
- Componentes modulares e reutilizáveis
- Sistema de eventos para comunicação entre sistemas
- Ferramentas de debug e desenvolvimento

### Performance
- Sistema otimizado de object pooling
- Gerenciamento eficiente de física
- Técnicas de otimização de renderização
- Cache de recursos frequentemente usados

### Plataformas Suportadas
- PC (Windows) com suporte completo
- Mobile (Android/iOS) com controles otimizados
- WebGL para acesso via navegador
- Configurações adaptativas por plataforma

## Notas de Implementação

### Estrutura de Componentes
O jogo utiliza uma arquitetura baseada em componentes focada em manutenibilidade e extensibilidade. Cada componente tem uma responsabilidade clara e bem definida:

- PlayerMovement: Núcleo do controle do jogador
- PlayerCombat: Gerenciamento de combate modular
- PlayerVisualManager: Sistema visual desacoplado
- PlayerActionController: Controle de ações especiais
- PlayerAudioManager: Sistema de áudio independente

### Sistema de Eventos
- Comunicação desacoplada via eventos
- Sistema robusto de callbacks
- Gerenciamento eficiente de listeners
- Depuração clara de fluxo de eventos

### Pipeline de Renderização
- URP otimizado para 2D
- Sistema flexível de pós-processamento
- Shaders otimizados para performance
- Efeitos visuais escaláveis

### Salvamento e Carregamento
- Sistema robusto de persistência
- Salvamento automático de progresso
- Configurações por jogador
- Sistema de backup de saves

## Considerações de Design

### Princípios de Design
1. Feedback imediato e claro para o jogador
2. Controles responsivos em todas as plataformas
3. Sistemas integrados e coerentes
4. Experiência consistente e polida

### Acessibilidade
- Interface escalável para diferentes telas
- Opções de controle personalizáveis
- Configurações de dificuldade
- Suporte a diferentes idiomas

### Escalabilidade
- Arquitetura preparada para expansão
- Sistema modular de conteúdo
- Ferramentas de desenvolvimento robustas
- Documentação completa e atualizada
