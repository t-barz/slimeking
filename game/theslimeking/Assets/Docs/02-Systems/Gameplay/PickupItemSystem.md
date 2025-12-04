# Sistema de Pickup Item

## Visão Geral

O sistema de Pickup Item permite que o jogador colete itens no mundo do jogo pressionando o botão de interação (E). Quando um item é coletado, o movimento do player é pausado temporariamente para dar feedback visual da ação.

## Componentes

### PickupItem.cs
Script principal que implementa a interface `IInteractable` e gerencia a coleta de itens.

**Localização:** `Assets/Code/Gameplay/Props/PickupItem.cs`

## Como Usar

### 1. Criar um Item Coletável

1. Crie um GameObject na cena (ex: "Apple", "Coin", "Potion")
2. Adicione o componente `PickupItem`
3. Configure os parâmetros no Inspector:

#### Configurações do Item
- **Item Data**: Referência ao ScriptableObject ItemData do item
- **Quantity**: Quantidade do item a ser adicionada ao inventário (padrão: 1)

#### Configurações de Pausa
- **Pause Duration**: Duração da pausa do movimento em segundos (padrão: 0.5s)

#### Configurações Visuais
- **Interaction Prompt**: Texto exibido ao jogador (padrão: "Pressione E para coletar")
- **Interaction Priority**: Prioridade quando múltiplas interações estão disponíveis (padrão: 10)

#### Debug
- **Enable Debug Logs**: Ativa logs detalhados no Console

### 2. Configurar Collider

O item precisa de um `Collider2D` configurado como **Trigger** para ser detectado pelo sistema de interação.

**Opção 1 - Manual:**
1. Adicione um `CircleCollider2D` ou `BoxCollider2D`
2. Marque a opção "Is Trigger"
3. Ajuste o tamanho conforme necessário

**Opção 2 - Editor Customizado:**
1. Selecione o GameObject com PickupItem
2. No Inspector, use os botões de "Ações Rápidas":
   - "Configurar Collider como Trigger"
   - "Adicionar CircleCollider2D"

### 3. Criar ItemData

Se ainda não existe, crie um ScriptableObject ItemData:

1. No Project: `Create > Inventory > Item Data`
2. Configure os dados do item:
   - Nome
   - Descrição
   - Sprite
   - Tipo (Consumable, Equipment, Quest, etc.)
   - Propriedades específicas

## Fluxo de Funcionamento

1. **Detecção**: O `InteractionHandler` do player detecta o PickupItem quando entra no range
2. **Prompt**: O sistema exibe o prompt de interação na UI
3. **Interação**: Jogador pressiona E
4. **Coleta**: 
   - Item é adicionado ao inventário via `InventoryManager`
   - Movimento do player é pausado por `pauseDuration` segundos
   - GameObject do item é destruído
5. **Retorno**: Movimento do player é restaurado automaticamente

## Integração com Outros Sistemas

### PlayerController
- Usa o método `SetCanMove(bool)` para pausar/retomar movimento
- Detecta itens através do `InteractionHandler`

### InventoryManager
- Recebe o item através do método `AddItem(ItemData, int)`
- Gerencia empilhamento e slots disponíveis

### InteractionHandler
- Detecta objetos `IInteractable` próximos ao player
- Gerencia prioridades quando múltiplas interações estão disponíveis
- Processa o input de interação

## Exemplo de Uso

```csharp
// Exemplo de configuração via código (não recomendado, use o Inspector)
PickupItem pickup = gameObject.AddComponent<PickupItem>();
pickup.itemData = Resources.Load<ItemData>("Items/Apple");
pickup.quantity = 1;
pickup.pauseDuration = 0.5f;
pickup.interactionPrompt = "Pressione E para coletar Maçã";
```

## Troubleshooting

### Item não é detectado
- ✅ Verifique se o Collider2D está marcado como "Is Trigger"
- ✅ Verifique se o ItemData está configurado
- ✅ Verifique se o player tem o componente `InteractionHandler`
- ✅ Ative "Enable Debug Logs" para ver mensagens no Console

### Inventário cheio
- O sistema exibe um aviso no Console
- O item não é coletado e permanece na cena
- O evento `OnInventoryFull` é disparado

### Player não para ao coletar
- Verifique se `pauseDuration` está maior que 0
- Verifique se o `PlayerController` tem o método `SetCanMove` implementado

## Extensões Futuras

- [ ] Efeito visual/sonoro ao coletar
- [ ] Animação de coleta
- [ ] Partículas ao coletar
- [ ] Feedback de UI mais elaborado
- [ ] Suporte para itens que não são destruídos (respawn)
- [ ] Cooldown entre coletas
