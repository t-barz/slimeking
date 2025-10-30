# Requirements Document - NPCQuickConfig

## Introduction

O **NPCQuickConfig** é uma ferramenta de editor Unity que acelera drasticamente a criação de NPCs para The Slime King. Inspirado nos já existentes `BushQuickConfig` e `ItemQuickConfig`, esta ferramenta permite configurar NPCs completos com comportamentos, IA, diálogos e sistemas de amizade em minutos ao invés de horas.

**Objetivo:** Reduzir o tempo de criação de NPCs de 2 horas para 30 minutos (75% mais rápido), mantendo consistência e qualidade.

**Contexto do Jogo:**

- The Slime King é um RPG 2D top-down em pixel art
- NPCs são criaturas que interagem com o jogador através de diálogos, quests e sistema de amizade
- Existem 3 tipos principais de NPCs: Passivos (wander), Neutros (patrol), e Quest Givers (static)
- NPCs precisam de componentes Unity (SpriteRenderer, Animator, Collider2D), IA básica, e ScriptableObjects de dados

**Alpha 1 Requirements:**

- 1 Cervo-Broto (passivo, wander behavior)
- 1 Esquilo Coletor (quest giver, static)
- 1 Abelha Cristalina (neutro, patrol behavior)

---

## Requirements

### Requirement 1: Editor Window Interface

**User Story:** Como desenvolvedor, quero abrir uma janela de editor Unity intuitiva para configurar NPCs rapidamente, para que eu possa criar personagens sem escrever código manualmente.

#### Acceptance Criteria

1. WHEN o desenvolvedor seleciona "QuickWinds/NPC Quick Config" no menu Unity THEN uma janela de editor deve abrir com interface clara e organizada
2. WHEN a janela está aberta THEN deve exibir seções claramente separadas: Template Selection, Basic Configuration, Behavior Settings, e Advanced Options
3. WHEN o desenvolvedor não tem GameObject selecionado THEN a ferramenta deve exibir mensagem "Select a GameObject in the scene to configure as NPC"
4. WHEN o desenvolvedor seleciona um GameObject na cena THEN a ferramenta deve detectar automaticamente e exibir o nome do objeto selecionado
5. IF o GameObject selecionado já possui componentes de NPC THEN a ferramenta deve carregar as configurações existentes nos campos da interface

---

### Requirement 2: Template System

**User Story:** Como desenvolvedor, quero selecionar templates pré-configurados de NPCs baseados no GDD, para que eu possa criar personagens consistentes com o design do jogo rapidamente.

#### Acceptance Criteria

1. WHEN a janela está aberta THEN deve exibir dropdown com templates disponíveis: "Custom", "Cervo-Broto", "Esquilo Coletor", "Abelha Cristalina"
2. WHEN o desenvolvedor seleciona template "Cervo-Broto" THEN deve preencher automaticamente: Behavior Type = Passivo, AI Type = Wander, Friendship Enabled = True, Dialogue Enabled = False
3. WHEN o desenvolvedor seleciona template "Esquilo Coletor" THEN deve preencher automaticamente: Behavior Type = Quest Giver, AI Type = Static, Friendship Enabled = True, Dialogue Enabled = True, Quest Giver = True
4. WHEN o desenvolvedor seleciona template "Abelha Cristalina" THEN deve preencher automaticamente: Behavior Type = Neutro, AI Type = Patrol, Friendship Enabled = True, Dialogue Enabled = False, Detection Range = 5.0f
5. WHEN o desenvolvedor seleciona template "Custom" THEN todos os campos devem ficar vazios permitindo configuração manual completa
6. IF template é selecionado THEN deve exibir descrição do template abaixo do dropdown (ex: "Passivo, wander behavior, amizade habilitada")

---

### Requirement 3: Basic Component Configuration

**User Story:** Como desenvolvedor, quero que a ferramenta configure automaticamente os componentes Unity básicos necessários para um NPC, para que eu não precise adicionar manualmente SpriteRenderer, Animator, Colliders, etc.

#### Acceptance Criteria

1. WHEN o desenvolvedor clica em "Apply Configuration" THEN a ferramenta deve adicionar SpriteRenderer ao GameObject se não existir
2. WHEN SpriteRenderer é adicionado THEN deve configurar Sorting Layer = "Characters" e Order in Layer = 10
3. WHEN o desenvolvedor clica em "Apply Configuration" THEN a ferramenta deve adicionar Animator ao GameObject se não existir
4. WHEN o desenvolvedor clica em "Apply Configuration" THEN a ferramenta deve adicionar CircleCollider2D ao GameObject se não existir
5. WHEN CircleCollider2D é adicionado THEN deve configurar Is Trigger = False e Radius baseado no tamanho do sprite (default 0.5f)
6. WHEN o desenvolvedor clica em "Apply Configuration" THEN a ferramenta deve adicionar Rigidbody2D ao GameObject se não existir
7. WHEN Rigidbody2D é adicionado THEN deve configurar Body Type = Dynamic, Gravity Scale = 0, Constraints = Freeze Rotation Z
8. IF GameObject já possui algum desses componentes THEN a ferramenta deve preservar configurações existentes e apenas ajustar campos necessários

---

### Requirement 4: AI Behavior Configuration

**User Story:** Como desenvolvedor, quero configurar o comportamento de IA do NPC através de presets simples, para que o personagem tenha movimento e reações apropriadas sem programação complexa.

#### Acceptance Criteria

1. WHEN a janela está aberta THEN deve exibir dropdown "AI Type" com opções: "Static", "Wander", "Patrol"
2. WHEN AI Type = "Static" THEN NPC deve permanecer parado no local inicial
3. WHEN AI Type = "Wander" THEN deve exibir campos: Wander Radius (float, default 5.0f), Wander Speed (float, default 2.0f), Pause Duration (float, default 2.0f)
4. WHEN AI Type = "Patrol" THEN deve exibir campos: Patrol Points (lista de Vector2), Patrol Speed (float, default 2.5f), Wait at Point (float, default 1.0f)
5. WHEN o desenvolvedor clica em "Apply Configuration" THEN deve adicionar script de IA apropriado ao GameObject (NPCStaticAI, NPCWanderAI, ou NPCPatrolAI)
6. WHEN script de IA é adicionado THEN deve configurar todos os parâmetros públicos com valores definidos na interface
7. IF AI Type = "Patrol" AND Patrol Points está vazio THEN deve criar automaticamente 4 pontos de patrulha em quadrado ao redor da posição inicial (raio 3 unidades)

---

### Requirement 5: Behavior Type Configuration

**User Story:** Como desenvolvedor, quero definir o tipo de comportamento social do NPC (Passivo, Neutro, Agressivo, Quest Giver), para que o personagem reaja apropriadamente ao jogador.

#### Acceptance Criteria

1. WHEN a janela está aberta THEN deve exibir dropdown "Behavior Type" com opções: "Passivo", "Neutro", "Agressivo", "Quest Giver"
2. WHEN Behavior Type = "Passivo" THEN NPC não deve atacar jogador mesmo se atacado, deve fugir quando HP < 30%
3. WHEN Behavior Type = "Neutro" THEN NPC deve ignorar jogador a menos que atacado, então retalia
4. WHEN Behavior Type = "Agressivo" THEN NPC deve perseguir e atacar jogador quando dentro de Detection Range
5. WHEN Behavior Type = "Quest Giver" THEN deve habilitar automaticamente Dialogue Enabled = True e adicionar componente QuestGiver
6. WHEN Behavior Type é selecionado THEN deve adicionar script NPCBehavior ao GameObject com tipo configurado
7. IF Behavior Type = "Neutro" OR "Agressivo" THEN deve exibir campo Detection Range (float, default 5.0f para Neutro, 7.0f para Agressivo)

---

### Requirement 6: Friendship System Configuration

**User Story:** Como desenvolvedor, quero habilitar e configurar o sistema de amizade para NPCs, para que jogadores possam construir relacionamentos com personagens ao longo do jogo.

#### Acceptance Criteria

1. WHEN a janela está aberta THEN deve exibir checkbox "Friendship Enabled"
2. WHEN Friendship Enabled = True THEN deve exibir campos adicionais: Species Name (string), Initial Friendship Level (int, default 0), Max Friendship Level (int, default 5)
3. WHEN o desenvolvedor clica em "Apply Configuration" AND Friendship Enabled = True THEN deve adicionar componente NPCFriendship ao GameObject
4. WHEN NPCFriendship é adicionado THEN deve configurar Species Name, Initial Level, e Max Level com valores da interface
5. WHEN Friendship Enabled = True THEN deve criar automaticamente ScriptableObject de FriendshipData em "Assets/Data/NPCs/Friendship/{SpeciesName}FriendshipData.asset"
6. WHEN FriendshipData é criado THEN deve preencher com valores padrão: Level 0 = "Desconhecido", Level 1 = "Conhecido", Level 2 = "Amigável", Level 3 = "Amigo", Level 4 = "Melhor Amigo", Level 5 = "Companheiro Leal"
7. IF Species Name já existe em FriendshipData THEN deve reutilizar ScriptableObject existente ao invés de criar novo

---

### Requirement 7: Dialogue System Configuration

**User Story:** Como desenvolvedor, quero configurar sistema de diálogo para NPCs, para que personagens possam conversar com o jogador através de texto.

#### Acceptance Criteria

1. WHEN a janela está aberta THEN deve exibir checkbox "Dialogue Enabled"
2. WHEN Dialogue Enabled = True THEN deve exibir campos: Dialogue Trigger Type (dropdown: "Proximity", "Interaction"), Trigger Range (float, default 2.0f se Proximity)
3. WHEN o desenvolvedor clica em "Apply Configuration" AND Dialogue Enabled = True THEN deve adicionar componente NPCDialogue ao GameObject
4. WHEN NPCDialogue é adicionado THEN deve configurar Trigger Type e Range com valores da interface
5. WHEN Dialogue Enabled = True THEN deve criar automaticamente ScriptableObject de DialogueData em "Assets/Data/NPCs/Dialogues/{NPCName}DialogueData.asset"
6. WHEN DialogueData é criado THEN deve preencher com diálogo placeholder: "Olá! Eu sou {NPCName}. [Diálogo placeholder - edite este ScriptableObject]"
7. WHEN Dialogue Enabled = True AND Behavior Type = "Quest Giver" THEN deve adicionar linha de diálogo adicional: "Preciso de sua ajuda com algo importante."

---

### Requirement 8: ScriptableObject Generation

**User Story:** Como desenvolvedor, quero que a ferramenta gere automaticamente ScriptableObjects de dados do NPC, para que eu tenha estrutura de dados organizada e reutilizável.

#### Acceptance Criteria

1. WHEN o desenvolvedor clica em "Apply Configuration" THEN deve criar ScriptableObject NPCData em "Assets/Data/NPCs/{NPCName}Data.asset"
2. WHEN NPCData é criado THEN deve preencher campos: NPC Name, Species, Behavior Type, AI Type, Max HP (default 100), Move Speed, Detection Range
3. WHEN NPCData é criado THEN deve referenciar FriendshipData e DialogueData se habilitados
4. WHEN o desenvolvedor clica em "Apply Configuration" THEN deve adicionar componente NPCController ao GameObject
5. WHEN NPCController é adicionado THEN deve referenciar automaticamente o NPCData criado
6. IF NPCData com mesmo nome já existe THEN deve perguntar ao desenvolvedor: "NPCData already exists. Overwrite? (Yes/No/Create New)"
7. WHEN desenvolvedor escolhe "Create New" THEN deve adicionar sufixo numérico ao nome (ex: "CervoBroto_1")

---

### Requirement 9: Animator Configuration

**User Story:** Como desenvolvedor, quero que a ferramenta configure automaticamente o Animator Controller com estados básicos, para que o NPC tenha animações funcionais imediatamente.

#### Acceptance Criteria

1. WHEN o desenvolvedor clica em "Apply Configuration" THEN deve verificar se existe Animator Controller em "Assets/Art/Animations/NPCs/{NPCName}Controller.controller"
2. IF Animator Controller não existe THEN deve criar novo com estados: Idle, Walk, Talk (se Dialogue Enabled), Death
3. WHEN Animator Controller é criado THEN deve configurar transições: Idle ↔ Walk (parâmetro: Speed > 0.1), Any State → Death (parâmetro: IsDead = true)
4. WHEN Animator Controller é criado THEN deve adicionar parâmetros: Speed (float), IsDead (bool), IsTalking (bool se Dialogue Enabled)
5. WHEN Animator Controller é criado THEN deve aplicar animações placeholder se existirem em "Assets/Art/Animations/Placeholders/"
6. WHEN Animator é configurado THEN deve referenciar o Animator Controller no componente Animator do GameObject
7. IF Animator Controller já existe THEN deve reutilizar existente sem modificar

---

### Requirement 10: Validation and Error Handling

**User Story:** Como desenvolvedor, quero que a ferramenta valide configurações e exiba erros claros, para que eu possa corrigir problemas antes de aplicar configurações.

#### Acceptance Criteria

1. WHEN o desenvolvedor clica em "Apply Configuration" AND nenhum GameObject está selecionado THEN deve exibir erro: "Please select a GameObject in the scene first"
2. WHEN o desenvolvedor clica em "Apply Configuration" AND Species Name está vazio AND Friendship Enabled = True THEN deve exibir erro: "Species Name is required when Friendship is enabled"
3. WHEN o desenvolvedor clica em "Apply Configuration" AND AI Type = "Patrol" AND Patrol Points tem menos de 2 pontos THEN deve exibir warning: "Patrol requires at least 2 points. Auto-generating 4 points."
4. WHEN configuração é aplicada com sucesso THEN deve exibir mensagem: "NPC '{NPCName}' configured successfully! NPCData created at: {path}"
5. WHEN erro ocorre durante configuração THEN deve exibir mensagem de erro detalhada e reverter mudanças parciais
6. WHEN o desenvolvedor clica em "Validate Configuration" THEN deve verificar todas as dependências e exibir lista de warnings/erros sem aplicar mudanças
7. IF componentes necessários não podem ser adicionados (ex: GameObject é prefab) THEN deve exibir erro específico: "Cannot modify prefab directly. Unpack prefab first."

---

### Requirement 11: Preview and Visualization

**User Story:** Como desenvolvedor, quero visualizar como o NPC ficará configurado antes de aplicar mudanças, para que eu possa ajustar configurações sem trial-and-error.

#### Acceptance Criteria

1. WHEN template é selecionado THEN deve exibir preview visual mostrando: ícone do NPC, comportamento, IA, e sistemas habilitados
2. WHEN AI Type = "Wander" THEN deve desenhar gizmo circular na Scene View mostrando Wander Radius
3. WHEN AI Type = "Patrol" THEN deve desenhar gizmo de linha conectando Patrol Points na Scene View
4. WHEN Detection Range é configurado THEN deve desenhar gizmo circular mostrando alcance de detecção
5. WHEN Dialogue Enabled = True AND Trigger Type = "Proximity" THEN deve desenhar gizmo mostrando Trigger Range
6. WHEN o desenvolvedor passa mouse sobre campo na interface THEN deve exibir tooltip explicativo
7. WHEN configuração é aplicada THEN gizmos devem permanecer visíveis no editor para debugging

---

### Requirement 12: Batch Configuration

**User Story:** Como desenvolvedor, quero aplicar configurações para múltiplos NPCs simultaneamente, para que eu possa criar vários personagens similares rapidamente.

#### Acceptance Criteria

1. WHEN múltiplos GameObjects estão selecionados THEN a ferramenta deve exibir: "Multiple objects selected ({count}). Apply configuration to all?"
2. WHEN o desenvolvedor clica em "Apply to All" THEN deve aplicar mesma configuração para todos os GameObjects selecionados
3. WHEN batch configuration é aplicada THEN deve gerar NPCData único para cada GameObject (usando nome do GameObject)
4. WHEN batch configuration é aplicada THEN deve exibir progress bar: "Configuring NPCs... {current}/{total}"
5. WHEN batch configuration completa THEN deve exibir resumo: "{success} NPCs configured successfully, {failed} failed"
6. IF algum GameObject falha durante batch THEN deve continuar processando os demais e listar erros no final
7. WHEN batch configuration é aplicada THEN deve permitir undo com Ctrl+Z para reverter todas as mudanças

---

### Requirement 13: Integration with Existing Systems

**User Story:** Como desenvolvedor, quero que NPCs criados com QuickConfig integrem perfeitamente com sistemas existentes do jogo, para que funcionem imediatamente sem configuração adicional.

#### Acceptance Criteria

1. WHEN NPC é configurado THEN deve ser compatível com sistema de combate existente (se Behavior Type = "Neutro" ou "Agressivo")
2. WHEN NPC é configurado THEN deve ser compatível com sistema de amizade existente (se Friendship Enabled = True)
3. WHEN NPC é configurado THEN deve ser compatível com sistema de diálogo existente (se Dialogue Enabled = True)
4. WHEN NPC é configurado THEN deve ser compatível com sistema de quests existente (se Behavior Type = "Quest Giver")
5. WHEN NPC é configurado THEN deve registrar automaticamente no NPCManager global se existir
6. WHEN NPC é configurado THEN deve adicionar tags apropriadas: "NPC", e tag específica do Behavior Type (ex: "PassiveNPC", "QuestGiver")
7. WHEN NPC é configurado THEN deve configurar Layer = "NPCs" para colisões apropriadas

---

### Requirement 14: Documentation and Help

**User Story:** Como desenvolvedor, quero acessar documentação e ajuda diretamente na ferramenta, para que eu possa aprender a usar sem consultar documentos externos.

#### Acceptance Criteria

1. WHEN a janela está aberta THEN deve exibir botão "Help" no canto superior direito
2. WHEN o desenvolvedor clica em "Help" THEN deve abrir janela com guia de uso passo-a-passo
3. WHEN o desenvolvedor passa mouse sobre qualquer campo THEN deve exibir tooltip explicativo detalhado
4. WHEN a janela está aberta THEN deve exibir link "View QuickWinds Documentation" que abre Assets/Docs/QuickWinds.md
5. WHEN erro ocorre THEN mensagem de erro deve incluir sugestão de solução quando possível
6. WHEN template é selecionado THEN deve exibir descrição completa do template incluindo uso recomendado
7. WHEN a janela está aberta THEN deve exibir seção "Quick Tips" com dicas contextuais baseadas na configuração atual

---

### Requirement 15: Performance and Optimization

**User Story:** Como desenvolvedor, quero que a ferramenta execute rapidamente mesmo ao configurar múltiplos NPCs, para que não haja delays perceptíveis durante o desenvolvimento.

#### Acceptance Criteria

1. WHEN o desenvolvedor clica em "Apply Configuration" THEN configuração deve completar em menos de 500ms para NPC único
2. WHEN batch configuration é aplicada para 10 NPCs THEN deve completar em menos de 5 segundos
3. WHEN ScriptableObjects são criados THEN deve usar AssetDatabase.StartAssetEditing() e StopAssetEditing() para otimizar I/O
4. WHEN gizmos são desenhados THEN não deve causar lag na Scene View (máximo 16ms por frame)
5. WHEN a janela está aberta THEN não deve causar recompilação de scripts desnecessária
6. WHEN configuração é aplicada THEN deve usar Undo.RecordObject() para permitir undo eficiente
7. WHEN múltiplos NPCs são configurados THEN deve reutilizar recursos compartilhados (ex: mesmo Animator Controller para mesma espécie)
