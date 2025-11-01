# Requirements Document

## Introduction

Este documento descreve os requisitos para um sistema de diálogos com NPCs que permite configuração rápida via editor do Unity. O sistema deve exibir diálogos multilíngues com efeito de texto animado (typewriter) quando o jogador interage com NPCs. A solução deve ser simples de configurar, suportar paginação de texto e carregar conteúdo de arquivos JSON com suporte a múltiplos idiomas.

## Requirements

### Requirement 1: Quick Setup via Editor

**User Story:** Como desenvolvedor, eu quero configurar um NPC com diálogo simplesmente selecionando o GameObject no editor e usando um Quick Setup, para que eu possa adicionar diálogos rapidamente sem código manual.

#### Acceptance Criteria

1. WHEN o desenvolvedor seleciona um GameObject NPC no editor THEN o sistema SHALL fornecer um botão ou menu "Quick Setup" no Inspector
2. WHEN o desenvolvedor clica em "Quick Setup" THEN o sistema SHALL adicionar automaticamente todos os componentes necessários (collider de interação, scripts de diálogo, etc.)
3. WHEN o Quick Setup é executado THEN o sistema SHALL permitir configurar o ID do diálogo e parâmetros básicos diretamente no Inspector
4. IF o GameObject já possui componentes de diálogo THEN o sistema SHALL detectar e não duplicar componentes

### Requirement 2: Proximity Detection and Interaction Icon

**User Story:** Como jogador, eu quero ver um ícone visual acima do NPC quando estou próximo, para que eu saiba que posso interagir com ele.

#### Acceptance Criteria

1. WHEN o jogador entra na área de proximidade do NPC THEN o sistema SHALL exibir um ícone de interação acima da cabeça do NPC
2. WHEN o jogador sai da área de proximidade do NPC THEN o sistema SHALL ocultar o ícone de interação
3. WHEN o ícone está visível AND o jogador pressiona o botão de interação THEN o sistema SHALL iniciar o diálogo
4. IF múltiplos NPCs estão próximos THEN o sistema SHALL exibir ícones apenas para o NPC mais próximo ou todos dentro do alcance (configurável)
5. WHEN o ícone é exibido THEN o sistema SHALL posicionar o ícone acima do NPC seguindo sua posição em world space

### Requirement 3: Dialogue Box Display

**User Story:** Como jogador, eu quero ver uma caixa de diálogo na tela com o texto do NPC, para que eu possa ler o que ele está dizendo.

#### Acceptance Criteria

1. WHEN o jogador inicia uma interação com NPC THEN o sistema SHALL exibir uma caixa de diálogo na tela
2. WHEN a caixa de diálogo é exibida THEN o sistema SHALL mostrar o texto configurado para aquele NPC
3. WHEN o diálogo está ativo THEN o sistema SHALL pausar ou limitar o movimento do jogador (configurável)
4. WHEN o diálogo termina THEN o sistema SHALL ocultar a caixa de diálogo e restaurar o controle do jogador
5. IF o texto possui múltiplas páginas THEN o sistema SHALL exibir um indicador visual de continuação

### Requirement 4: Typewriter Text Effect

**User Story:** Como jogador, eu quero ver o texto aparecer letra por letra, para que a experiência de leitura seja mais dinâmica e envolvente.

#### Acceptance Criteria

1. WHEN a caixa de diálogo exibe texto THEN o sistema SHALL mostrar o texto caractere por caractere
2. WHEN o efeito typewriter está ativo THEN o sistema SHALL usar uma velocidade configurável (caracteres por segundo)
3. WHEN o jogador pressiona o botão de interação durante o efeito THEN o sistema SHALL completar instantaneamente o texto da página atual
4. WHEN o texto da página está completo THEN o sistema SHALL permitir avançar para a próxima página (se houver)
5. IF a velocidade é configurada como 0 THEN o sistema SHALL exibir o texto instantaneamente

### Requirement 5: Multilingual JSON Data Loading

**User Story:** Como desenvolvedor, eu quero carregar textos de diálogo de arquivos JSON com suporte a múltiplos idiomas, para que o jogo possa ser facilmente localizado.

#### Acceptance Criteria

1. WHEN o sistema carrega diálogos THEN o sistema SHALL ler dados de arquivos JSON
2. WHEN um arquivo JSON é lido THEN o sistema SHALL conter campos para: ID, descrição curta, e textos em múltiplos idiomas
3. WHEN os idiomas suportados são definidos THEN o sistema SHALL incluir: BR (Português Brasil), EN (Inglês), ES (Espanhol), CH (Chinês), RS (Russo), FR (Francês), IT (Italiano), DT (Alemão), JP (Japonês), KR (Coreano)
4. WHEN o sistema busca um texto THEN o sistema SHALL usar o idioma configurado no jogo
5. IF o idioma configurado não está disponível para um diálogo THEN o sistema SHALL usar EN (Inglês) como fallback
6. IF o idioma EN não está disponível THEN o sistema SHALL usar o primeiro idioma disponível no JSON
7. WHEN o JSON é inválido ou não encontrado THEN o sistema SHALL registrar um erro e exibir uma mensagem padrão

### Requirement 6: Paginated Dialogue Support

**User Story:** Como desenvolvedor, eu quero criar diálogos com múltiplas páginas, para que NPCs possam ter conversas mais longas divididas em partes gerenciáveis.

#### Acceptance Criteria

1. WHEN um diálogo possui múltiplas páginas THEN o sistema SHALL exibir uma página por vez
2. WHEN uma página está sendo exibida AND o texto está completo THEN o sistema SHALL aguardar o jogador pressionar o botão de interação
3. WHEN o jogador pressiona o botão de interação AND há mais páginas THEN o sistema SHALL avançar para a próxima página
4. WHEN o jogador pressiona o botão de interação AND é a última página THEN o sistema SHALL fechar o diálogo
5. WHEN uma nova página é exibida THEN o sistema SHALL limpar o texto anterior e iniciar o efeito typewriter novamente
6. IF o diálogo possui apenas uma página THEN o sistema SHALL fechar ao pressionar o botão após o texto completar

### Requirement 7: Configuration and Customization

**User Story:** Como desenvolvedor, eu quero configurar parâmetros do sistema de diálogo, para que eu possa ajustar a experiência para diferentes NPCs e situações.

#### Acceptance Criteria

1. WHEN o desenvolvedor configura um NPC THEN o sistema SHALL permitir definir: ID do diálogo, distância de interação, velocidade do typewriter
2. WHEN o desenvolvedor configura o sistema global THEN o sistema SHALL permitir definir: idioma padrão, caminho dos arquivos JSON, prefab da UI de diálogo
3. WHEN parâmetros são alterados no editor THEN o sistema SHALL refletir as mudanças em tempo de execução (play mode)
4. WHEN o desenvolvedor testa no editor THEN o sistema SHALL fornecer feedback visual de áreas de interação (gizmos)
