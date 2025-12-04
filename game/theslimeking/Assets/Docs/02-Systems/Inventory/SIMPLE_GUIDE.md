# Guia SUPER Simples - Sistema de Inventário Persistente

**Versão:** 2.0 KISS (Keep It Simple, Stupid)  
**Data:** 15 de Novembro de 2025

---

## 🎯 O Que Foi Feito

Modifiquei o `ItemPickup` existente para:
1. **Salvar automaticamente** quando item é coletado
2. **Verificar ao carregar** se item já foi coletado
3. **Destruir automaticamente** se já foi coletado antes

**Simples assim!** ✨

---

## 🚀 Como Usar (2 Passos)

### Passo 1: Seus itens já funcionam!

Se você já tem `ItemPickup` configurado nos objetos `item_RedFruit` e `item_appleA`, **não precisa fazer NADA**!

O sistema já está funcionando automaticamente.

### Passo 2 (Opcional): Adicionar Logger

Para ver os itens salvos ao carregar a cena:

1. Crie um GameObject vazio chamado `ItemLogger`
2. Adicione o componente `SimpleItemLogger`
3. Pronto!

---

## 📊 O Que Você Vai Ver no Console

### Ao Carregar a Cena (Primeira Vez):
```
═══════════════════════════════════════
📦 ITENS SALVOS NA CENA
═══════════════════════════════════════
❌ item_RedFruit: Não coletado
❌ item_appleA: Não coletado
───────────────────────────────────────
📊 Total: 0/2 coletados
═══════════════════════════════════════

📦 Item disponível: item_RedFruit
📦 Item disponível: item_appleA
```

### Ao Coletar item_RedFruit:
```
✅ Item coletado e salvo: item_RedFruit
```

### Ao Recarregar a Cena:
```
═══════════════════════════════════════
📦 ITENS SALVOS NA CENA
═══════════════════════════════════════
✅ item_RedFruit: Coletado
❌ item_appleA: Não coletado
───────────────────────────────────────
📊 Total: 1/2 coletados
═══════════════════════════════════════

🚫 Item já coletado anteriormente: item_RedFruit
📦 Item disponível: item_appleA
```

**O item_RedFruit não aparece mais na cena!** 🎉

---

## 🔧 Comandos Úteis

### Limpar Dados de UM Item:
1. Selecione o item na hierarquia (ex: `item_RedFruit`)
2. Clique direito no componente `ItemPickup`
3. Selecione: `Clear Save Data (This Item)`

### Limpar TODOS os Dados:
1. Menu: `Extra Tools > Items > Clear ALL Item Save Data`
2. Confirme a ação

---

## ⚙️ Como Funciona (Técnico)

**Ao Coletar:**
```csharp
PlayerPrefs.SetInt($"Item_{gameObject.name}", 1);
```

**Ao Carregar:**
```csharp
int wasCollected = PlayerPrefs.GetInt($"Item_{gameObject.name}", 0);
if (wasCollected == 1) Destroy(gameObject);
```

**Simples assim!** Usa o nome do GameObject como ID único.

---

## ✅ Checklist Rápido

- [ ] Seus itens têm componente `ItemPickup`? → Já funciona!
- [ ] Quer ver logs? → Adicione `SimpleItemLogger` em um GameObject vazio
- [ ] Quer resetar? → `Extra Tools > Items > Clear ALL Item Save Data`

---

## 🎯 Importante

**Nome do GameObject = ID do Item**

Certifique-se que cada item tem um nome único:
- ✅ `item_RedFruit`
- ✅ `item_appleA`
- ✅ `item_appleB`
- ❌ `item_apple` (duplicado)
- ❌ `item_apple` (duplicado)

---

## 🎉 Pronto!

Agora teste:
1. **Play** na cena
2. **Colete** um item
3. **Stop** o play
4. **Play** novamente
5. Item não aparece mais! ✨

---

**Última Atualização:** 15 de Novembro de 2025  
**Filosofia:** KISS - Keep It Simple, Stupid
