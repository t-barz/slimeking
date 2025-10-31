# Requirements Document

## Introduction

Esta feature adiciona uma nova ferramenta no menu "Extra Tools" do Unity Editor que permite configurar automaticamente uma cena com todos os componentes essenciais necessários para permitir a transição do slime entre diferentes cenas do jogo (por exemplo, entre Initial Cave e Initial Forest). A ferramenta deve detectar o que já existe na cena e adicionar apenas os componentes que estão faltando, garantindo que a cena esteja completa e funcional para suportar o sistema de teleporte e transição entre cenas.

## Requirements

### Requirement 1

**User Story:** Como desenvolvedor, eu quero uma ferramenta de menu no Unity Editor que configure automaticamente uma cena, para que eu não precise adicionar manualmente todos os componentes necessários para transição entre cenas.

#### Acceptance Criteria

1. WHEN o desenvolvedor acessa o menu "Extra Tools" no Unity Editor THEN o sistema SHALL exibir uma opção "Setup Scene for Transitions"
2. WHEN o desenvolvedor seleciona a opção "Setup Scene for Transitions" THEN o sistema SHALL executar a configuração automática da cena ativa
3. WHEN a ferramenta é executada THEN o sistema SHALL exibir uma mensagem de log indicando o início do processo de configuração
4. WHEN a configuração é concluída THEN o sistema SHALL exibir uma mensagem de log resumindo o que foi adicionado ou já existia na cena

### Requirement 2

**User Story:** Como desenvolvedor, eu quero que a ferramenta detecte componentes existentes na cena, para que não haja duplicação de GameObjects ou componentes essenciais.

#### Acceptance Criteria

1. WHEN a ferramenta é executada THEN o sistema SHALL verificar se cada componente essencial já existe na cena antes de criá-lo
2. IF um GameObject essencial já existe na cena THEN o sistema SHALL reutilizá-lo ao invés de criar um duplicado
3. IF um GameObject essencial não existe na cena THEN o sistema SHALL criá-lo com a configuração padrão apropriada
4. WHEN a verificação é realizada THEN o sistema SHALL registrar no log quais componentes foram encontrados e quais foram criados

### Requirement 3

**User Story:** Como desenvolvedor, eu quero que a ferramenta configure o GameManager na cena, para que o gerenciamento do jogo funcione corretamente.

#### Acceptance Criteria

1. WHEN a ferramenta é executada THEN o sistema SHALL verificar se existe um GameObject chamado "GameManager" na cena
2. IF o GameManager não existe THEN o sistema SHALL criar um GameObject "GameManager" na posição (0, 0, 0)
3. IF o GameManager existe mas não possui o componente GameManager THEN o sistema SHALL adicionar o componente GameManager
4. WHEN o GameManager é configurado THEN o sistema SHALL garantir que ele possui o componente Transform

### Requirement 4

**User Story:** Como desenvolvedor, eu quero que a ferramenta configure o SceneTransitioner na cena, para que as transições entre cenas funcionem corretamente.

#### Acceptance Criteria

1. WHEN a ferramenta é executada THEN o sistema SHALL verificar se existe um GameObject chamado "SceneTransitioner" na cena
2. IF o SceneTransitioner não existe THEN o sistema SHALL criar um GameObject "SceneTransitioner" na posição (0, 0, 0)
3. IF o SceneTransitioner existe mas não possui o componente SceneTransitioner THEN o sistema SHALL adicionar o componente SceneTransitioner
4. WHEN o SceneTransitioner é configurado THEN o sistema SHALL garantir que ele possui o componente Transform

### Requirement 5

**User Story:** Como desenvolvedor, eu quero que a ferramenta configure o TeleportManager na cena, para que o sistema de teleporte funcione corretamente.

#### Acceptance Criteria

1. WHEN a ferramenta é executada THEN o sistema SHALL verificar se existe um GameObject chamado "TeleportManager" na cena
2. IF o TeleportManager não existe THEN o sistema SHALL criar um GameObject "TeleportManager" na posição (0, 0, 0)
3. IF o TeleportManager existe mas não possui o componente TeleportManager THEN o sistema SHALL adicionar o componente TeleportManager
4. WHEN o TeleportManager é configurado THEN o sistema SHALL garantir que ele possui o componente Transform

### Requirement 6

**User Story:** Como desenvolvedor, eu quero que a ferramenta configure o EventSystem na cena, para que a UI e inputs funcionem corretamente.

#### Acceptance Criteria

1. WHEN a ferramenta é executada THEN o sistema SHALL verificar se existe um GameObject com o componente EventSystem na cena
2. IF o EventSystem não existe THEN o sistema SHALL criar um GameObject "EventSystem" na posição (0, 0, 0)
3. WHEN o EventSystem é criado THEN o sistema SHALL adicionar os componentes EventSystem e InputSystemUIInputModule
4. IF o EventSystem já existe THEN o sistema SHALL verificar se possui o InputSystemUIInputModule e adicioná-lo se necessário

### Requirement 7

**User Story:** Como desenvolvedor, eu quero que a ferramenta seja segura e não destrutiva, para que não perca configurações existentes ao executá-la.

#### Acceptance Criteria

1. WHEN a ferramenta é executada THEN o sistema SHALL preservar todos os GameObjects e componentes existentes na cena
2. WHEN a ferramenta adiciona um componente a um GameObject existente THEN o sistema SHALL preservar todas as configurações e componentes já presentes
3. WHEN a ferramenta cria um novo GameObject THEN o sistema SHALL usar valores padrão seguros que não conflitem com a configuração existente
4. WHEN a ferramenta é executada THEN o sistema SHALL marcar a cena como modificada (dirty) para que o desenvolvedor possa salvar as alterações

### Requirement 8

**User Story:** Como desenvolvedor, eu quero feedback claro sobre o que a ferramenta fez, para que eu possa entender as mudanças aplicadas à cena.

#### Acceptance Criteria

1. WHEN a ferramenta é executada THEN o sistema SHALL exibir uma mensagem de log para cada componente verificado
2. WHEN um componente é encontrado existente THEN o sistema SHALL registrar no log que o componente já existe
3. WHEN um componente é criado THEN o sistema SHALL registrar no log que o componente foi adicionado
4. WHEN a configuração é concluída THEN o sistema SHALL exibir um resumo final com o total de componentes adicionados e já existentes
