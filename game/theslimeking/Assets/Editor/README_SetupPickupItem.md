# Configure as Pickup Item - Ferramenta de Editor

## 📋 Descrição

Esta ferramenta automatiza a configuração de GameObjects para funcionarem como itens coletáveis (Pickup Items) no jogo, seguindo o padrão do `item_MushroomA`.

## 🎯 Como Usar

### Método 1: Menu de Contexto (Recomendado)
1. Selecione o GameObject na Hierarchy
2. Clique com o botão direito
3. Selecione **Extra Tools > Configure as Pickup Item**

### Método 2: Menu Superior
1. Selecione o GameObject na Hierarchy
2. No menu superior, vá em **GameObject > Extra Tools > Configure as Pickup Item**

## ⚙️ O que a ferramenta faz

A ferramenta executa automaticamente as seguintes ações:

### 1. Remove Componentes Desnecessários
- ❌ **Rigidbody2D** - Não é necessário para itens pickup
- ❌ **BounceHandler** - Comportamento de bounce não é usado
- ❌ **ItemBuffHandler** - Buffs são gerenciados pelo ItemPickup

### 2. Adiciona/Configura Animator
- ✅ Adiciona componente **Animator** se não existir
- ✅ Remove o RuntimeAnimatorController (deixa null)
- ✅ Configura CullingMode para `CullUpdateTransforms`

### 3. Configura CircleCollider2D
- ✅ Adiciona **CircleCollider2D** se não existir
- ✅ Configura:
  - Radius: `0.22`
  - Offset: `(0, 0)`
  - IsTrigger: `true`

### 4. Adiciona ItemPickup
- ✅ Adiciona componente **ItemPickup** se não existir
- ⚠️ Você precisará configurar manualmente:
  - Item Data (CollectableItemData)
  - Inventory Item Data (opcional)
  - Outros parâmetros específicos

### 5. Cria Shadow Child
- ✅ Cria GameObject filho chamado `shadowA` se não existir
- ✅ Adiciona SpriteRenderer
- ✅ Tenta encontrar e atribuir automaticamente o sprite "shadowA"
- ⚠️ Se o sprite não for encontrado, você precisará atribuí-lo manualmente

## 📝 Configuração Manual Necessária

Após usar a ferramenta, você ainda precisa configurar:

1. **ItemPickup Component:**
   - Atribuir o `CollectableItemData` (ScriptableObject com dados do item)
   - Configurar `Inventory Item Data` se o item for adicionado ao inventário
   - Ajustar parâmetros como `interactionRadius`, `moveSpeed`, etc.

2. **Shadow Sprite:**
   - Se o sprite não foi encontrado automaticamente, arraste o sprite correto para o SpriteRenderer do child `shadowA`

## 🔍 Exemplo de Uso

### Antes:
```
item_RedFruit
├─ SpriteRenderer
├─ Rigidbody2D
├─ BounceHandler
├─ ItemBuffHandler
├─ CircleCollider2D (radius: 0.15625, offset: 0, 0.15625)
├─ ItemPickup
└─ shadowA
```

### Depois:
```
item_RedFruit
├─ SpriteRenderer
├─ Animator (sem controller)
├─ CircleCollider2D (radius: 0.22, offset: 0, 0, trigger: true)
├─ ItemPickup
└─ shadowA (com SpriteRenderer configurado)
```

## ⚠️ Avisos

- A ferramenta usa **Undo**, então você pode desfazer as mudanças com `Ctrl+Z`
- Certifique-se de salvar a cena antes de usar a ferramenta
- A ferramenta não modifica o sprite principal do item
- Componentes removidos não podem ser recuperados após salvar a cena

## 🐛 Troubleshooting

**Problema:** Shadow sprite não foi atribuído
- **Solução:** Procure manualmente por "shadowA" nos assets e arraste para o SpriteRenderer do child

**Problema:** ItemPickup não funciona
- **Solução:** Verifique se você atribuiu o `CollectableItemData` no Inspector

**Problema:** Item não é coletado
- **Solução:** Verifique se o layer do GameObject está correto (deve ser "Interactable" ou similar)

## 📚 Referência

Esta ferramenta foi criada baseada na análise do `item_MushroomA` que está configurado corretamente na cena `3_InitialForest`.

Para mais informações sobre o sistema de itens, consulte:
- `ItemPickup.cs` - Script principal de coleta
- `CollectableItemData.cs` - ScriptableObject com dados do item
