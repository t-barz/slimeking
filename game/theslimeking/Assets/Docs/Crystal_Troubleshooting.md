# 🔧 Troubleshooting: Cristais Atraídos mas Não Absorvidos

## Problema Reportado

- ✅ Cristal é atraído magneticamente até o player
- ❌ Cristal não é absorvido/coletado automaticamente
- ❌ Cristal "fica grudado" no player sem desaparecer

## Possíveis Causas e Soluções

### 1. **GameManager Não Encontrado**

**Sintomas:**

- Log: `GameManager.HasInstance retornou false! Cristal [nome] não foi coletado.`

**Soluções:**

- ✅ Verifique se há um GameObject com `GameManager` na cena
- ✅ Certifique-se que o `GameManager` está ativo
- ✅ Verifique se o `GameManager` está inicializando corretamente

### 2. **Distância de Coleta Inadequada**

**Sintomas:**

- Log: `[ItemCollectable] [nome] - Distância do player: [valor maior que 0.2]`
- Cristal fica "orbitando" o player sem chegar próximo o suficiente

**Soluções:**

- 🔧 Aumentar a distância de coleta de 0.2f para 0.5f
- 🔧 Verificar se o player tem colliders que impedem aproximação
- 🔧 Ajustar a curva de atração (`attractionCurve`)

### 3. **CrystalData Não Configurado**

**Sintomas:**

- Log: `CollectItem chamado para [nome]`
- Mas não aparece: `CrystalData encontrado: [nome]`

**Soluções:**

- ✅ Verificar se o campo `Crystal Data` está preenchido no `ItemCollectable`
- ✅ Criar um `CrystalElementalData` ScriptableObject se necessário

### 4. **Collider Conflitos**

**Sintomas:**

- Cristal para de se mover antes de chegar no player
- Não aparecem logs de coleta automática

**Soluções:**

- 🔧 Verificar se o cristal tem `Collider2D` marcado como `isTrigger = true`
- 🔧 Verificar se o player não tem colliders físicos impedindo aproximação
- 🔧 Verificar layers de colisão

### 5. **Curva de Atração Problemática**

**Sintomas:**

- Cristal se move devagar ou para antes de chegar
- Movimento errático

**Soluções:**

- 🔧 Resetar `attractionCurve` para `AnimationCurve.EaseInOut(0, 0, 1, 1)`
- 🔧 Aumentar `attractionSpeed` para valores maiores (6f - 10f)

## 🧪 Como Testar

### 1. Ativar Logs de Debug

```csharp
// No ItemCollectable, verificar se os logs estão aparecendo:
"CollectItem chamado para [nome]"
"CrystalData encontrado: [nome]"
"GameManager encontrado, adicionando cristal..."
"Cristal [nome] coletado (+[valor] [tipo])"
```

### 2. Monitorar Distância

- Observar logs de distância: `[ItemCollectable] [nome] - Distância do player: [valor]`
- Se a distância nunca chegar abaixo de 0.2, há problema de aproximação

### 3. Verificar Configuração

```
ItemCollectable Component:
├── Crystal Data: ✅ Deve estar preenchido
├── Item Data: ⬜ Pode estar vazio
├── Inventory Item Data: ⬜ Deve estar vazio
├── Enable Attraction: ✅ true
├── Attraction Radius: ✅ 2.5f (ou maior)
└── Attraction Speed: ✅ 4.0f (ou maior)
```

## 🔨 Correções Rápidas

### Correção 1: Aumentar Distância de Coleta

Se o problema for distância, editar `UpdateAttraction()`:

```csharp
if (distanceToPlayer <= 0.5f) // Era 0.2f
```

### Correção 2: Garantir GameManager na Cena

- Criar GameObject vazio
- Adicionar componente `GameManager`
- Verificar se está ativo na hierarquia

### Correção 3: Verificar Player Tag

- Player deve ter tag "Player"
- Campo `PLAYER_TAG` deve ser "Player"

### Correção 4: Forçar Coleta por Trigger

Se a coleta automática não funcionar, garantir que `OnTriggerEnter2D` funcione:

```csharp
// O player deve ter Collider2D para ativar triggers
// O cristal deve ter isTrigger = true
```

## 📊 Checklist de Validação

- [ ] GameManager existe e está ativo na cena
- [ ] CrystalData está configurado no ItemCollectable
- [ ] Enable Attraction está marcado
- [ ] Player tem tag "Player"
- [ ] Cristal tem Collider2D com isTrigger = true
- [ ] Logs de debug aparecem no console
- [ ] Distância do player chega abaixo de 0.5 unidades

## 🚀 Se Nada Funcionar

**Solução de emergência - Coleta por timer:**

```csharp
// No UpdateAttraction(), adicionar timeout
if (_attractionProgress >= 2.0f) // 2 segundos tentando
{
    Debug.Log("Forçando coleta por timeout...");
    CollectItem();
}
```

---
**Status**: 🔍 Investigando  
**Logs adicionados**: ✅ Detalhados  
**Próximo passo**: Testar e analisar logs
