# Como Criar ItemData (ScriptableObjects)

## Guia Rápido

### Método 1: Via Menu do Unity (Recomendado)

1. **No Project Window**, navegue até a pasta onde quer criar o item
   - Sugestão: `Assets/Data/Items/`

2. **Clique com botão direito** na pasta

3. Selecione: **Create > Extra Tools > Items > Item**

4. **Renomeie o arquivo** (ex: `Item_RedFruit`, `Item_Apple`, `Item_Mushroom`)

5. **Configure os campos** no Inspector:
   - **Item Name**: Nome do item (ex: "Fruta Vermelha")
   - **Icon**: Arraste o sprite do item
   - **Type**: Selecione o tipo (Consumable, Equipment, QuestItem, Material)
   - **Is Stackable**: Deixe DESMARCADO (sistema não empilhável)
   - **Heal Amount**: Quantidade de cura (se for consumível)
   - Outros campos conforme necessário

---

## Estrutura do ItemData

```csharp
namespace TheSlimeKing.Inventory
{
    public class ItemData : ScriptableObject
    {
        // Informações Básicas
        public string itemName;        // Nome do item
        public Sprite icon;            // Ícone do item
        public ItemType type;          // Tipo do item
        public bool isStackable;       // Se empilha (deixar FALSE)

        // Propriedades de Consumível
        public int healAmount;         // Quantidade de cura

        // Propriedades de Equipamento
        public EquipmentType equipmentType;
        public int defenseBonus;
        public int speedBonus;

        // Propriedades de Quest
        public bool isQuestItem;
    }
}
```

---

## Tipos de Item (ItemType)

- **Consumable**: Itens consumíveis (frutas, poções)
- **Equipment**: Equipamentos (armas, armaduras)
- **QuestItem**: Itens de quest
- **Material**: Materiais (pedras, madeira)

---

## Tipos de Equipamento (EquipmentType)

- **None**: Não é equipamento
- **Weapon**: Arma
- **Armor**: Armadura
- **Accessory**: Acessório

---

## Exemplos de Configuração

### Exemplo 1: Fruta de Cura (Consumível)

```
Item Name: Fruta Vermelha
Icon: [sprite da fruta]
Type: Consumable
Is Stackable: FALSE
Heal Amount: 2
Equipment Type: None
Defense Bonus: 0
Speed Bonus: 0
Is Quest Item: FALSE
```

### Exemplo 2: Maçã (Consumível)

```
Item Name: Maçã
Icon: [sprite da maçã]
Type: Consumable
Is Stackable: FALSE
Heal Amount: 1
Equipment Type: None
Defense Bonus: 0
Speed Bonus: 0
Is Quest Item: FALSE
```

### Exemplo 3: Cogumelo (Consumível)

```
Item Name: Cogumelo
Icon: [sprite do cogumelo]
Type: Consumable
Is Stackable: FALSE
Heal Amount: 1
Equipment Type: None
Defense Bonus: 0
Speed Bonus: 0
Is Quest Item: FALSE
```

### Exemplo 4: Pedra (Material)

```
Item Name: Pedra Arredondada
Icon: [sprite da pedra]
Type: Material
Is Stackable: FALSE
Heal Amount: 0
Equipment Type: None
Defense Bonus: 0
Speed Bonus: 0
Is Quest Item: FALSE
```

---

## Passo a Passo Completo

### 1. Criar a Pasta de Items (se não existir)

1. No Project Window, navegue até `Assets/Data/`
2. Se não existir a pasta `Items`, crie: **Botão direito > Create > Folder**
3. Nomeie como `Items`

### 2. Criar ItemData para cada tipo de item

Para cada item na cena (frutas, cogumelos, pedras), crie um ItemData:

#### Fruta Vermelha
1. `Assets/Data/Items/` > **Botão direito > Create > Extra Tools > Items > Item**
2. Renomeie para `Item_RedFruit`
3. Configure:
   - Item Name: `Fruta Vermelha`
   - Type: `Consumable`
   - Is Stackable: `FALSE`
   - Heal Amount: `2`
   - Icon: Arraste o sprite da fruta vermelha

#### Maçã
1. `Assets/Data/Items/` > **Botão direito > Create > Extra Tools > Items > Item**
2. Renomeie para `Item_Apple`
3. Configure:
   - Item Name: `Maçã`
   - Type: `Consumable`
   - Is Stackable: `FALSE`
   - Heal Amount: `1`
   - Icon: Arraste o sprite da maçã

#### Cogumelo
1. `Assets/Data/Items/` > **Botão direito > Create > Extra Tools > Items > Item**
2. Renomeie para `Item_Mushroom`
3. Configure:
   - Item Name: `Cogumelo`
   - Type: `Consumable`
   - Is Stackable: `FALSE`
   - Heal Amount: `1`
   - Icon: Arraste o sprite do cogumelo

#### Pedra
1. `Assets/Data/Items/` > **Botão direito > Create > Extra Tools > Items > Item**
2. Renomeie para `Item_RoundedRock`
3. Configure:
   - Item Name: `Pedra Arredondada`
   - Type: `Material`
   - Is Stackable: `FALSE`
   - Heal Amount: `0`
   - Icon: Arraste o sprite da pedra

### 3. Atribuir ItemData aos PickupItems na Cena

1. Abra a cena `3_InitialForest`
2. Para cada item na cena (ex: `item_RedFruit (1)`):
   - Selecione o GameObject
   - No Inspector, encontre o componente `PickupItem`
   - No campo `Inventory Item Data`, arraste o ItemData correspondente
   - Exemplo: Para `item_RedFruit (1)`, arraste `Item_RedFruit`

3. Repita para todos os itens:
   - `item_RedFruit (1)` → `Item_RedFruit`
   - `item_appleA` → `Item_Apple`
   - `item_MushroomA` → `Item_Mushroom`
   - `item_MushroomA (1)` → `Item_Mushroom`
   - `item_RoundedRockA` (todos) → `Item_RoundedRock`

---

## Verificação

Após criar os ItemData e atribuir aos PickupItems:

1. ✅ Cada ItemData tem um ícone atribuído
2. ✅ `Is Stackable` está FALSE em todos
3. ✅ Cada PickupItem na cena tem um ItemData atribuído
4. ✅ Os ícones dos itens aparecem corretamente

---

## Testando

1. Entre em **Play Mode**
2. Pressione **Tab** para abrir o inventário
3. Colete um item (aproxime-se dele)
4. Verifique se o ícone aparece no slot do inventário
5. Colete mais itens até preencher os 12 slots

---

## Troubleshooting

### Problema: Menu "Extra Tools > Items > Item" não aparece
**Solução**: Aguarde a compilação do Unity terminar e tente novamente

### Problema: Ícone não aparece no inventário
**Solução**: 
- Verifique se o campo `Icon` do ItemData está preenchido
- Verifique se o PickupItem tem o ItemData atribuído

### Problema: Item não é coletado
**Solução**:
- Verifique se o PickupItem tem o campo `Inventory Item Data` preenchido
- Verifique se o InventoryManager existe na cena

---

## Estrutura de Pastas Recomendada

```
Assets/
└── Data/
    └── Items/
        ├── Item_RedFruit.asset
        ├── Item_Apple.asset
        ├── Item_Mushroom.asset
        └── Item_RoundedRock.asset
```

---

**Data**: Dezembro 4, 2025  
**Sistema**: Inventory System  
**Status**: ✅ Pronto para uso
