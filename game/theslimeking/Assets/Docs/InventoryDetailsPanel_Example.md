# 🍎 Exemplo: Adicionando Descrição ao Item Maçã

## Como Preencher o Campo `description` em um ItemData Existente

### Via Inspector (Método Manual)

1. **Localize o asset:**
   - Navegue até `Assets/Data/Items/item_appleA`
   - Ou use o search: digite `item_appleA` no Project window

2. **Abra no Inspector:**
   - Clique no asset para selecioná-lo
   - O Inspector mostrará todos os campos do `ItemData`

3. **Preencha a descrição:**
   - Você verá um campo **Description** com área de texto expandida
   - Digite a descrição desejada, por exemplo:

   ```
   Uma maçã vermelha e suculenta.
   
   Restaura 15 pontos de vida quando consumida.
   Encontrada frequentemente em florestas e pomares.
   ```

4. **Salve:**
   - Ctrl+S ou File > Save Project
   - A descrição será salva no ScriptableObject

### Exemplo de Descrições para Diferentes Tipos de Itens

#### Consumível (Maçã)

```
Uma maçã vermelha e suculenta.

Restaura 15 pontos de vida quando consumida.
Encontrada frequentemente em florestas e pomares.
```

#### Material (Pedra Arredondada)

```
Uma pedra lisa e arredondada.

Material comum usado em construções e artesanato.
Pode ser encontrada ao longo de rios e praias.
```

#### Consumível (Cogumelo)

```
Um cogumelo silvestre de aparência peculiar.

Propriedades desconhecidas. Use com cautela.
```

#### Quest Item (Cristal Elemental)

```
Um cristal brilhante imbuído com energia elemental.

Este item parece importante para sua jornada.
Não pode ser descartado.
```

### Script de Migração Automática (Opcional)

Se você tiver muitos itens para atualizar, pode criar um script de Editor para preencher automaticamente:

```csharp
[MenuItem("Extra Tools/Items/Fill Sample Descriptions")]
public static void FillSampleDescriptions()
{
    string[] guids = AssetDatabase.FindAssets("t:ItemData");
    
    foreach (string guid in guids)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);
        
        if (item != null && string.IsNullOrEmpty(item.description))
        {
            // Descrição padrão baseada no tipo
            switch (item.type)
            {
                case ItemType.Consumable:
                    item.description = $"Restaura {item.healAmount} pontos de vida.";
                    break;
                case ItemType.Material:
                    item.description = "Material usado em artesanato.";
                    break;
                case ItemType.QuestItem:
                    item.description = "Item importante para uma missão.";
                    break;
                case ItemType.Equipment:
                    item.description = $"Equipamento: {item.equipmentType}";
                    break;
            }
            
            EditorUtility.SetDirty(item);
        }
    }
    
    AssetDatabase.SaveAssets();
    Debug.Log("Descrições preenchidas com sucesso!");
}
```

### Verificação Visual

Após preencher as descrições:

1. Entre em **Play Mode**
2. Pressione **Tab** para abrir o inventário
3. Navegue com as **setas direcionais**
4. Observe o painel à direita atualizando com:
   - ✅ Ícone grande do item
   - ✅ Nome do item
   - ✅ Descrição completa (com quebra de linha automática)

---

**Dica:** Use quebras de linha (Enter) na TextArea para organizar parágrafos e melhorar a legibilidade!
