# 💎 Configuração de Cristais Elementais

## Problema Identificado ✅ RESOLVIDO

**Situação**: Cristais não eram atraídos até o player e não eram absorvidos quando o campo `Item Data` não estava preenchido no componente `ItemCollectable`.

**Causa**: O sistema de atração magnética dependia exclusivamente de `itemData` estar configurado, ignorando cristais que só tinham `crystalData`.

## Solução Implementada

### Mudanças no ItemCollectable.cs

1. **Condição de Atração Corrigida**

   ```csharp
   // ANTES: Só funcionava com itemData
   if (!enableAttraction || itemData == null) return;
   
   // AGORA: Funciona com itemData OU crystalData
   if (!enableAttraction || (itemData == null && crystalData == null)) return;
   ```

2. **Inicialização Flexível**

   ```csharp
   private void InitializeItem()
   {
       if (itemData != null)
       {
           // Usa configurações do itemData
           attractionRadius = itemData.detectionRadius;
           attractionSpeed = itemData.attractSpeed;
       }
       else if (crystalData != null)
       {
           // Configurações padrão para cristais
           attractionRadius = 2.5f; // Alcance médio
           attractionSpeed = 4.0f;   // Velocidade média
       }
   }
   ```

3. **Configuração Visual Automática**

   ```csharp
   private void SetupVisuals()
   {
       if (itemData != null)
           _originalColor = itemData.itemTint;
       else if (crystalData != null)
           _originalColor = crystalData.crystalTint;
       else
           _originalColor = Color.white;
   }
   ```

4. **Efeitos Visuais para Cristais**

   ```csharp
   private void PlayCollectionEffects()
   {
       if (itemData != null)
       {
           // Usa efeitos do itemData
       }
       else if (crystalData != null)
       {
           // Usa efeitos do crystalData
           if (crystalData.collectVFX != null)
               Instantiate(crystalData.collectVFX, transform.position, Quaternion.identity);
           if (crystalData.collectSound != null)
               AudioSource.PlayClipAtPoint(crystalData.collectSound, transform.position, 1f);
       }
   }
   ```

## Como Configurar Cristais Corretamente

### Método 1: Apenas CrystalData (Recomendado para Cristais)

1. **No GameObject do Cristal:**
   - Adicione componente `ItemCollectable`
   - **NÃO** preencha `Item Data`
   - **NÃO** preencha `Inventory Item Data`
   - **PREENCHA** apenas `Crystal Data` com um `CrystalElementalData`

2. **Configuração Automática:**
   - Atração: 2.5 unidades de alcance
   - Velocidade: 4.0 unidades/segundo
   - Cor: Baseada no `crystalTint` do CrystalData
   - Efeitos: Baseados nos campos do CrystalData

### Método 2: Cristal Híbrido (Flexível)

1. **Preencher ambos os campos:**
   - `Crystal Data`: Para o sistema de cristais
   - `Item Data`: Para configurações de atração personalizadas

2. **Comportamento:**
   - Prioriza configurações do `Item Data`
   - Ainda vai para o sistema de cristais (não para inventário)

## Configurações Padrão para Cristais

| Propriedade | Valor Padrão | Descrição |
|------------|--------------|-----------|
| `attractionRadius` | 2.5f | Alcance de atração magnética |
| `attractionSpeed` | 4.0f | Velocidade de movimento até o player |
| `activationDelay` | 0.5f | Delay antes da ativação da atração |
| Cor visual | `crystalData.crystalTint` | Cor baseada no tipo elemental |

## Sistema de Prioridades

A coleta segue esta ordem de prioridade:

1. **🥇 CRISTAIS**: Se `crystalData != null` → Vai para `GameManager.AddCrystal()`
2. **🥈 INVENTÁRIO**: Se `inventoryItemData != null` → Vai para `InventoryManager.AddItem()`
3. **🥉 SISTEMA LEGADO**: Se `itemData != null` → Aplica efeitos diretos

## Exemplo Prático

### Cristal de Fogo Simples

```
GameObject: "Fire_Crystal"
├── SpriteRenderer (sprite do cristal)
├── Collider2D (trigger ativo)
└── ItemCollectable
    ├── Crystal Data: FireCrystalData (ScriptableObject)
    ├── Item Data: [VAZIO] ✅
    ├── Inventory Item Data: [VAZIO] ✅
    └── Enable Attraction: true
```

### Cristal com Configuração Personalizada

```
GameObject: "Special_Nature_Crystal"
├── SpriteRenderer
├── Collider2D (trigger)
└── ItemCollectable
    ├── Crystal Data: NatureCrystalData ✅
    ├── Item Data: CustomAttractConfig ✅ (para atração customizada)
    ├── Inventory Item Data: [VAZIO]
    ├── Attraction Radius: 5.0f (ignorado, usa itemData)
    └── Attraction Speed: 8.0f (ignorado, usa itemData)
```

## Validação Visual

### ✅ Cristal Configurado Corretamente

- Aparece com a cor do tipo elemental
- É atraído quando player se aproxima
- Desaparece ao ser coletado
- Aparece no contador de cristais da UI

### ❌ Cristal Mal Configurado

- Não é atraído pelo player
- Console mostra: "não tem ItemData nem CrystalData configurado!"
- Pode ser coletado por colisão, mas não vai para lugar nenhum

## Logs de Debug

Durante a configuração, observe estes logs:

```
[ItemCollectable] Cristal Nature Crystal inicializado com configurações padrão
[ItemCollectable] Nature Crystal ativou atração magnética após 0.5s
[ItemCollectable] Nature Crystal iniciou atração magnética
[ItemCollectable] Cristal Nature Crystal coletado (+1 Nature)
```

## Troubleshooting

### Problema: Cristal não é atraído

**Solução**: Verifique se `Crystal Data` está preenchido e `Enable Attraction` está marcado

### Problema: Cristal é atraído mas não é coletado

**Solução**: Verifique se `GameManager` existe na cena e `GameManager.HasInstance` retorna true

### Problema: Cristal vai para inventário ao invés do contador

**Solução**: Certifique-se que `Crystal Data` está preenchido E que `Inventory Item Data` está vazio

### Problema: Cristal não tem efeitos visuais

**Solução**: Configure `collectVFX` e `collectSound` no `CrystalElementalData`

---

**Status**: ✅ Problema resolvido  
**Versão**: ItemCollectable v2.1  
**Compatibilidade**: Unity 2022.3+ LTS
