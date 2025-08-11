# Sistema de Inventário - Documentação

## Visão Geral
O sistema de inventário é um componente flexível e expansível que permite gerenciar itens em um jogo 2D. O sistema suporta slots dinâmicos, empilhamento de itens e uma interface de usuário personalizável.

## Características Principais
- Tamanho inicial do inventário: 2 slots
- Tamanho máximo: 6 slots
- Sistema de empilhamento de itens
- Interface de usuário dinâmica
- Sistema de eventos para atualizações da UI
- Tipos de itens pré-definidos (Consumíveis, Equipamentos, Chaves, Recursos)

## Configuração

### 1. Criando Itens
Para criar um novo item:
1. No Project Window, clique com o botão direito
2. Selecione Create > SlimeKing > Inventory > Item
3. Configure as propriedades do item:
   - Item Name: Nome do item
   - Icon: Sprite que representa o item
   - Description: Descrição do item
   - Is Stackable: Se o item pode ser empilhado
   - Max Stack Size: Quantidade máxima por pilha
   - Item Type: Tipo do item (Consumable, Equipment, Key, Resource)

### 2. Configurando o Inventário
1. Adicione o componente `Inventory` a um GameObject na cena
2. Configure o tamanho inicial do inventário (padrão: 2 slots)

### 3. Configurando a Interface do Usuário
1. Crie um Canvas na sua cena
2. Adicione um GameObject vazio como filho do Canvas
3. Adicione o componente `InventoryUI` ao GameObject
4. Configure a referência para o componente `Inventory`

## Uso do Sistema

### Manipulando Itens via Código

```csharp
// Obtendo referência ao inventário
private Inventory inventory;

// Adicionando um item
public bool AddItem(ItemData item, int amount = 1)
{
    return inventory.AddItem(item, amount);
}

// Removendo um item
public bool RemoveItem(ItemData item, int amount = 1)
{
    return inventory.RemoveItem(item, amount);
}

// Expandindo o inventário
public bool ExpandInventory()
{
    return inventory.ExpandSize();
}
```

### Sistema de Eventos
O sistema utiliza eventos para notificar mudanças:

```csharp
// Inscrevendo-se em eventos
inventory.OnInventoryChanged += HandleInventoryChanged;
inventory.OnSlotUpdated += HandleSlotUpdated;

// Manipulando eventos
private void HandleInventoryChanged()
{
    // Atualizar a UI ou realizar outras ações
}

private void HandleSlotUpdated(int slotIndex)
{
    // Atualizar um slot específico
}
```

## Melhores Práticas

1. **Gerenciamento de Referências**
   - Mantenha referências aos ItemData em um ScriptableObject separado para fácil acesso
   - Use eventos para manter a UI sincronizada com o estado do inventário

2. **Performance**
   - Evite verificar o inventário a cada frame
   - Use os eventos do sistema para atualizações

3. **Extensibilidade**
   - Para adicionar novos tipos de itens, expanda o enum ItemType
   - Para funcionalidades específicas, herde de ItemData

## Limitações

- Máximo de 6 slots no inventário
- Itens não podem ser divididos em pilhas menores automaticamente
- Um item só pode ocupar um slot por vez

## Solução de Problemas

### Problemas Comuns

1. **Itens não aparecem na UI**
   - Verifique se o InventoryUI está referenciando corretamente o Inventory
   - Confirme se os sprites dos itens estão configurados corretamente

2. **Não é possível adicionar itens**
   - Verifique se há espaço disponível no inventário
   - Confirme se as configurações de empilhamento estão corretas

3. **Eventos não estão funcionando**
   - Verifique se os eventos estão sendo inscritos corretamente
   - Confirme se os handlers não foram removidos acidentalmente
