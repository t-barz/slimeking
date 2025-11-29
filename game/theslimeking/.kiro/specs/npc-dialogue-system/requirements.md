# Requirements Document - NPC Dialogue System

## Introduction

Este documento define os requisitos para um sistema de diálogo para NPCs no jogo The Slime King. O sistema permitirá que NPCs exibam textos localizados em um Canvas com efeito de digitação (typewriter), aguardem interação do jogador para avançar, e suportem eventos customizados ao final dos diálogos.

## Glossary

- **NPC**: Non-Player Character - Personagem não jogável que pode interagir com o jogador através de diálogos
- **Dialogue System**: Sistema de Diálogo - Sistema responsável por gerenciar a exibição de textos de NPCs
- **Typewriter Effect**: Efeito de Digitação - Animação que exibe texto caractere por caractere
- **Localized Text**: Texto Localizado - Texto que pode ser traduzido para diferentes idiomas usando o sistema de localização do Unity
- **Dialogue Canvas**: Canvas de Diálogo - Interface UI que exibe o diálogo do NPC
- **Dialogue Event**: Evento de Diálogo - Ação customizada que pode ser disparada ao final de um diálogo (quest, cutscene, item)
- **Player Input**: Entrada do Jogador - Ação do jogador para avançar ou fechar o diálogo
- **Interaction Radius**: Raio de Interação - Distância máxima na qual o jogador pode interagir com um NPC

## Requirements

### Requirement 1

**User Story:** Como jogador, eu quero interagir com NPCs e ver seus diálogos em uma interface visual, para que eu possa entender a história e receber informações do jogo.

#### Acceptance Criteria

1. WHEN o jogador está dentro do raio de interação de um NPC THEN o Dialogue System SHALL exibir um indicador visual de que a interação está disponível
2. WHEN o jogador pressiona o botão de interação próximo a um NPC THEN o Dialogue System SHALL abrir o Dialogue Canvas e iniciar a exibição do primeiro texto
3. WHEN o Dialogue Canvas está aberto THEN o Dialogue System SHALL pausar ou limitar o movimento do jogador
4. WHEN o diálogo termina THEN o Dialogue System SHALL fechar o Dialogue Canvas e restaurar o controle total do jogador

### Requirement 2

**User Story:** Como jogador, eu quero ver o texto do diálogo sendo exibido letra por letra, para que a experiência seja mais imersiva e natural.

#### Acceptance Criteria

1. WHEN um novo texto de diálogo é exibido THEN o Dialogue System SHALL mostrar cada caractere sequencialmente com um intervalo de tempo configurável
2. WHEN o typewriter effect está em execução THEN o Dialogue System SHALL permitir que o jogador pressione um botão para completar instantaneamente a exibição do texto atual
3. WHEN o texto está sendo exibido THEN o Dialogue System SHALL reproduzir um som de digitação para cada caractere (opcional e configurável)
4. WHEN caracteres especiais como espaços ou pontuação são encontrados THEN o Dialogue System SHALL exibi-los sem delay adicional ou com delay reduzido

### Requirement 3

**User Story:** Como jogador, eu quero avançar pelos diálogos no meu próprio ritmo, para que eu possa ler e compreender cada mensagem.

#### Acceptance Criteria

1. WHEN o texto atual terminou de ser exibido THEN o Dialogue System SHALL mostrar um indicador visual de que o jogador pode avançar
2. WHEN o jogador pressiona o botão de avançar e há mais textos THEN o Dialogue System SHALL exibir o próximo texto da sequência
3. WHEN o jogador pressiona o botão de avançar e não há mais textos THEN o Dialogue System SHALL fechar o diálogo
4. WHEN o diálogo possui múltiplos textos THEN o Dialogue System SHALL manter o controle da posição atual na sequência

### Requirement 4

**User Story:** Como desenvolvedor, eu quero que todos os textos de diálogo sejam localizados, para que o jogo possa ser traduzido para diferentes idiomas.

#### Acceptance Criteria

1. WHEN um diálogo é configurado THEN o Dialogue System SHALL usar referências de LocalizedString do Unity Localization
2. WHEN o idioma do jogo é alterado THEN o Dialogue System SHALL exibir os textos no novo idioma automaticamente
3. WHEN um texto localizado não está disponível THEN o Dialogue System SHALL usar o fallback configurado no sistema de localização
4. WHEN múltiplos textos são configurados THEN o Dialogue System SHALL suportar uma lista de LocalizedString references

### Requirement 5

**User Story:** Como desenvolvedor, eu quero configurar NPCs com diálogos de forma rápida e automatizada, para que eu possa criar múltiplos NPCs eficientemente.

#### Acceptance Criteria

1. WHEN o desenvolvedor seleciona um GameObject e executa "Extra Tools >> Setup Dialogue NPC" THEN o Dialogue System SHALL adicionar todos os componentes necessários ao GameObject
2. WHEN o setup é executado THEN o Dialogue System SHALL configurar o BoxCollider2D como trigger com tamanho apropriado
3. WHEN o setup é executado THEN o Dialogue System SHALL criar ou referenciar o Dialogue Canvas na cena
4. WHEN o setup é executado em um GameObject que já possui componentes de diálogo THEN o Dialogue System SHALL atualizar os componentes existentes sem duplicação

### Requirement 6

**User Story:** Como desenvolvedor, eu quero que o sistema de diálogo suporte eventos customizados, para que eu possa disparar ações como quests, cutscenes ou entrega de itens ao final dos diálogos.

#### Acceptance Criteria

1. WHEN um diálogo é configurado THEN o Dialogue System SHALL permitir a adição de UnityEvents que são invocados ao final do diálogo
2. WHEN o último texto do diálogo é exibido e o jogador avança THEN o Dialogue System SHALL invocar todos os eventos configurados antes de fechar o diálogo
3. WHEN nenhum evento está configurado THEN o Dialogue System SHALL fechar o diálogo normalmente sem erros
4. WHEN eventos são configurados THEN o Dialogue System SHALL permitir múltiplos eventos em sequência

### Requirement 7

**User Story:** Como designer de UI, eu quero que o diálogo seja exibido em um Canvas com uma imagem de fundo, para que a interface seja visualmente atraente e legível.

#### Acceptance Criteria

1. WHEN o Dialogue Canvas é criado THEN o Dialogue System SHALL incluir uma imagem de fundo configurável
2. WHEN o diálogo é exibido THEN o Dialogue System SHALL posicionar o Canvas de forma consistente na tela
3. WHEN o Canvas é configurado THEN o Dialogue System SHALL usar TextMeshPro para renderização de texto de alta qualidade
4. WHEN o Canvas é criado THEN o Dialogue System SHALL configurar o sorting order apropriado para aparecer sobre outros elementos UI

### Requirement 8

**User Story:** Como desenvolvedor, eu quero um sistema simples e direto de usar, para que eu possa configurar e manter diálogos facilmente seguindo o princípio KISS (Keep It Simple, Stupid).

#### Acceptance Criteria

1. WHEN o sistema é implementado THEN o Dialogue System SHALL usar no máximo 3 componentes principais (NPC Controller, Dialogue UI, Dialogue Data)
2. WHEN um desenvolvedor configura um diálogo THEN o Dialogue System SHALL requerer apenas a configuração de textos localizados e eventos opcionais
3. WHEN o código é escrito THEN o Dialogue System SHALL evitar abstrações desnecessárias e manter classes com responsabilidades únicas e claras
4. WHEN a documentação é criada THEN o Dialogue System SHALL incluir exemplos práticos e diretos de uso
5. WHEN novos desenvolvedores usam o sistema THEN o Dialogue System SHALL ser compreensível sem necessidade de documentação extensa

### Requirement 9

**User Story:** Como desenvolvedor, eu quero remover completamente o sistema de diálogo antigo, para que não haja conflitos ou código legado no projeto.

#### Acceptance Criteria

1. WHEN o novo sistema é implementado THEN o Dialogue System SHALL remover todos os scripts do sistema antigo (NPCDialogueController e relacionados)
2. WHEN componentes antigos são encontrados em NPCs THEN o Dialogue System SHALL fornecer uma ferramenta de migração ou limpeza
3. WHEN o sistema antigo é removido THEN o Dialogue System SHALL garantir que nenhuma referência quebrada permaneça no projeto
4. WHEN a limpeza é executada THEN o Dialogue System SHALL criar um log de todas as alterações realizadas
