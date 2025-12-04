# Exemplo de Configuração: Pickup Item

## Cenário: Coletar uma Maçã

Este exemplo mostra como configurar um item coletável (maçã) que cura o jogador.

### Passo 1: Criar o ItemData

1. No Project: `Assets/Data/Items/`
2. Botão direito > `Create > Inventory > Item Data`
3. Nomeie como "Apple"
4. Configure:
   ```
   Item Name: Maçã
   Description: Uma maçã suculenta que restaura 20 HP
   Type: Consumable
   Is Stackable: true
   Heal Amount: 20
   ```

### Passo 2: Criar o GameObject na Cena

1. Hierarchy > Botão direito > `Create Empty`
2. Nomeie como "Apple_Pickup"
3. Adicione um `SpriteRenderer`:
   - Sprite: (sprite da maçã)
   - Sorting Layer: Items
   - Order in Layer: 0

### Passo 3: Adicionar PickupItem

1. Selecione "Apple_Pickup"
2. `Add Component > Pickup Item`
3. Configure no Inspector:
   ```
   Item Data: Apple (arraste o ScriptableObject)
   Quantity: 1
   Pause Duration: 0.5
   Interaction Prompt: Pressione E para coletar
   Interaction Priority: 10
   Enable Debug Logs: true (para teste)
   ```

### Passo 4: Configurar Collider

**Opção A - Usando o Editor Customizado:**
1. No Inspector, clique em "Adicionar CircleCollider2D"
2. Ajuste o radius se necessário (padrão: 0.5)

**Opção B - Manual:**
1. `Add Component > Circle Collider 2D`
2. Marque "Is Trigger"
3. Radius: 0.5

### Passo 5: Testar

1. Execute o jogo (Play)
2. Aproxime o player da maçã
3. Observe o prompt de interação aparecer
4. Pressione E
5. Verifique:
   - ✅ Maçã desaparece da cena
   - ✅ Player para por 0.5 segundos
   - ✅ Item aparece no inventário
   - ✅ Console mostra logs de debug

### Resultado Esperado

```
[PickupItem-Apple_Pickup] Item Maçã coletado com sucesso
[PickupItem-Apple_Pickup] Movimento do player pausado por 0.5 segundos
[InventoryManager] Item 'Maçã' adicionado ao slot 0
[PickupItem-Apple_Pickup] Movimento do player retomado
```

## Variações

### Item Raro (Prioridade Alta)

```
Item Data: GoldenApple
Quantity: 1
Pause Duration: 1.0 (pausa mais longa para item raro)
Interaction Priority: 50 (prioridade alta)
```

### Moedas Empilháveis

```
Item Data: Coin
Quantity: 10
Pause Duration: 0.3 (pausa rápida)
Interaction Priority: 5 (prioridade baixa)
```

### Item de Quest

```
Item Data: MagicKey
Quantity: 1
Pause Duration: 1.5 (pausa dramática)
Interaction Priority: 100 (máxima prioridade)
```

## Dicas de Level Design

1. **Posicionamento**: Coloque itens em locais visíveis mas que exijam exploração
2. **Agrupamento**: Evite muitos itens próximos (conflito de prioridade)
3. **Feedback Visual**: Use partículas ou brilho para destacar itens importantes
4. **Respawn**: Para itens que devem reaparecer, não destrua o GameObject (implementação futura)

## Integração com Quest System

```csharp
// Exemplo: Item que completa uma quest ao ser coletado
public class QuestPickupItem : PickupItem
{
    [SerializeField] private string questID;
    
    protected override void OnItemCollected()
    {
        base.OnItemCollected();
        QuestManager.Instance.CompleteObjective(questID);
    }
}
```
