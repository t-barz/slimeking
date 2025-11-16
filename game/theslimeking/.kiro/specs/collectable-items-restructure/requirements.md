# Requirements Document - Reestruturação do Sistema de Itens Coletáveis

## Introduction

Este documento define os requisitos para reestruturar o sistema de itens coletáveis do jogo The Slime King. O objetivo é unificar o comportamento de coleta para que todos os itens (comuns, equipáveis e cristais elementais) sejam adicionados ao inventário do jogador, com tratamento especial para cristais elementais que devem atualizar contadores específicos no HUD.

## Glossary

- **Slime**: O personagem jogável principal
- **ItemCollectable**: Componente que gerencia itens coletáveis no mundo
- **InventoryManager**: Sistema que gerencia o inventário do jogador
- **GameManager**: Gerenciador central do jogo que mantém contadores de cristais
- **CrystalType**: Enum que define os tipos de cristais elementais (Water, Fire, Earth, Air, Shadow, Nature)
- **HUD**: Interface visual que exibe informações ao jogador
- **CrystalContainer**: Container no HUD que exibe contadores de cristais elementais
- **Count_Text**: Componente de texto que exibe a quantidade de cada cristal
- **Atração Magnética**: Comportamento onde itens se movem automaticamente em direção ao slime
- **Absorção**: Momento em que o item é coletado e seus efeitos são aplicados

## Requirements

### Requirement 1: Comportamento de Coleta Unificado

**User Story:** Como jogador, eu quero que todos os itens coletáveis sejam absorvidos pelo slime e adicionados ao meu inventário, para que eu possa gerenciar meus recursos de forma consistente.

#### Acceptance Criteria

1. WHEN o slime coleta um item comum, THE ItemCollectable SHALL adicionar o item ao inventário através do InventoryManager
2. WHEN o slime coleta um item equipável, THE ItemCollectable SHALL adicionar o item equipável ao inventário através do InventoryManager
3. WHEN o item é adicionado ao inventário com sucesso, THE ItemCollectable SHALL executar efeitos visuais de coleta
4. WHEN o item é adicionado ao inventário com sucesso, THE ItemCollectable SHALL executar efeitos sonoros de coleta
5. WHEN o item é adicionado ao inventário com sucesso, THE ItemCollectable SHALL remover o item da cena

### Requirement 2: Sistema de Cristais Elementais

**User Story:** Como jogador, eu quero que cristais elementais sejam tratados de forma especial ao serem coletados, para que eu possa ver meus contadores de cristais atualizados no HUD sem ocupar espaço no inventário.

#### Acceptance Criteria

1. WHEN o slime coleta um cristal elemental, THE ItemCollectable SHALL identificar o tipo do cristal (Water, Fire, Earth, Air, Shadow, Nature)
2. WHEN um cristal elemental é identificado, THE ItemCollectable SHALL chamar GameManager.AddCrystal com o tipo e quantidade corretos
3. WHEN GameManager.AddCrystal é chamado, THE GameManager SHALL incrementar o contador do tipo de cristal correspondente
4. WHEN o contador de cristal é incrementado, THE GameManager SHALL disparar o evento OnCrystalCountChanged
5. WHEN OnCrystalCountChanged é disparado, THE HUD SHALL atualizar o Count_Text do cristal correspondente
6. THE ItemCollectable SHALL NOT adicionar cristais elementais ao inventário
7. THE cristais elementais SHALL NOT ocupar slots do inventário do jogador

### Requirement 3: Atualização do HUD de Cristais

**User Story:** Como jogador, eu quero ver a quantidade de cada tipo de cristal elemental no HUD, para que eu saiba quantos recursos tenho disponíveis.

#### Acceptance Criteria

1. THE CrystalContainer SHALL conter um marcador visual para cada tipo de cristal elemental (Water, Fire, Earth, Air, Shadow, Nature)
2. WHEN um cristal é coletado, THE Count_Text do tipo correspondente SHALL exibir a quantidade atualizada
3. THE Count_Text SHALL formatar a quantidade com prefixo "x" (exemplo: "x10" para 10 cristais)
4. WHEN o contador é zero, THE Count_Text SHALL exibir "x0"
5. THE HUD SHALL se inscrever no evento OnCrystalCountChanged do GameManager para receber atualizações

### Requirement 4: Empilhamento de Itens no Inventário

**User Story:** Como jogador, eu quero que itens do mesmo tipo sejam empilhados no inventário, para que eu não desperdice espaços com itens duplicados.

#### Acceptance Criteria

1. WHEN um item é adicionado ao inventário, THE InventoryManager SHALL verificar se já existe um item do mesmo tipo
2. IF um item do mesmo tipo existe, THEN THE InventoryManager SHALL incrementar a quantidade do item existente
3. IF um item do mesmo tipo não existe, THEN THE InventoryManager SHALL criar um novo slot com o item
4. WHEN a quantidade de um item atinge o limite máximo de empilhamento (99), THE InventoryManager SHALL criar um novo slot para itens adicionais
5. THE InventoryManager SHALL retornar true se o item foi adicionado com sucesso

### Requirement 5: Tratamento de Inventário Cheio

**User Story:** Como jogador, eu quero ser notificado quando meu inventário está cheio, para que eu saiba que preciso liberar espaço antes de coletar mais itens.

#### Acceptance Criteria

1. WHEN o inventário está cheio, THE InventoryManager.AddItem SHALL retornar false
2. WHEN InventoryManager.AddItem retorna false, THE ItemCollectable SHALL reverter o estado de coleta do item
3. WHEN o estado de coleta é revertido, THE ItemCollectable SHALL reabilitar o collider do item
4. WHEN o inventário está cheio, THE ItemCollectable SHALL manter o item visível na cena
5. WHERE inventário está cheio, THE sistema SHALL permitir que o jogador tente coletar o item novamente após liberar espaço

### Requirement 6: Priorização de Tipos de Itens

**User Story:** Como desenvolvedor, eu quero que o sistema priorize corretamente os diferentes tipos de dados de itens, para que a coleta funcione de forma previsível.

#### Acceptance Criteria

1. THE ItemCollectable SHALL verificar crystalData primeiro (prioridade máxima)
2. IF crystalData está configurado, THEN THE ItemCollectable SHALL processar como cristal elemental e NOT adicionar ao inventário
3. IF crystalData não está configurado, THEN THE ItemCollectable SHALL verificar inventoryItemData
4. IF inventoryItemData está configurado, THEN THE ItemCollectable SHALL adicionar ao inventário
5. IF nenhum dado está configurado, THEN THE ItemCollectable SHALL usar CollectableItemData como fallback (sistema legado)
6. THE sistema SHALL garantir que cristais elementais nunca sejam adicionados ao inventário independentemente de outras configurações

### Requirement 7: Efeitos Visuais e Sonoros

**User Story:** Como jogador, eu quero feedback visual e sonoro ao coletar itens, para que a experiência seja satisfatória e clara.

#### Acceptance Criteria

1. WHEN um item é coletado, THE ItemCollectable SHALL executar animação de absorção (scale up e fade out)
2. WHEN um cristal elemental é coletado, THE ItemCollectable SHALL instanciar o VFX configurado em crystalData.collectVFX
3. WHEN um cristal elemental é coletado, THE ItemCollectable SHALL reproduzir o som configurado em crystalData.collectSound
4. WHEN um item comum é coletado, THE ItemCollectable SHALL instanciar o VFX configurado em itemData.vfxPrefab
5. WHEN um item comum é coletado, THE ItemCollectable SHALL reproduzir o som configurado em itemData.collectSound

### Requirement 8: Integração com Sistema de Atração Magnética

**User Story:** Como jogador, eu quero que itens sejam atraídos magneticamente para o slime antes de serem coletados, para que a coleta seja fluida e natural.

#### Acceptance Criteria

1. THE ItemCollectable SHALL manter o sistema de atração magnética existente
2. WHEN um item entra no raio de atração, THE ItemCollectable SHALL iniciar movimento em direção ao slime
3. WHEN o item alcança o slime (distância <= 0.2 unidades), THE ItemCollectable SHALL iniciar processo de coleta
4. THE sistema de atração SHALL respeitar o delay de ativação configurado
5. THE sistema de atração SHALL funcionar independentemente do tipo de item (comum, equipável ou cristal)

### Requirement 9: Compatibilidade com Sistema Legado

**User Story:** Como desenvolvedor, eu quero manter compatibilidade com o sistema legado de CollectableItemData, para que itens antigos continuem funcionando durante a transição.

#### Acceptance Criteria

1. THE ItemCollectable SHALL suportar CollectableItemData como fallback
2. WHEN CollectableItemData é usado, THE ItemCollectable SHALL aplicar efeitos de cura, XP e buffs
3. WHEN CollectableItemData é usado, THE ItemCollectable SHALL executar efeitos visuais e sonoros configurados
4. THE sistema legado SHALL ter prioridade mais baixa que crystalData e inventoryItemData
5. THE sistema SHALL permitir migração gradual de itens antigos para o novo sistema

### Requirement 10: Validação e Tratamento de Erros

**User Story:** Como desenvolvedor, eu quero que o sistema valide dados e trate erros graciosamente, para que bugs sejam fáceis de identificar e corrigir.

#### Acceptance Criteria

1. WHEN crystalData é null e GameManager.Instance é null, THE ItemCollectable SHALL registrar erro no log
2. WHEN crystalData é null e GameManager.Instance é null, THE ItemCollectable SHALL reverter estado de coleta
3. WHEN inventoryItemData é null e InventoryManager.Instance é null, THE ItemCollectable SHALL registrar warning no log
4. WHEN nenhum dado de item está configurado, THE ItemCollectable SHALL registrar warning e não processar coleta
5. THE sistema SHALL incluir logs detalhados para facilitar debugging durante desenvolvimento
