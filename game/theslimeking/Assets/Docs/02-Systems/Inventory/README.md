# Sistema de Inventário Persistente - KISS Version

**Filosofia:** Keep It Simple, Stupid  
**Data:** 15 de Novembro de 2025

---

## ✨ O Que Foi Feito

Modifiquei **apenas 1 arquivo** existente:
- `ItemPickup.cs` - Agora salva automaticamente quando coletado

Criei **apenas 1 arquivo** novo:
- `SimpleItemLogger.cs` - Mostra itens salvos (opcional)

**Total:** 2 arquivos. Simples assim! 🎯

---

## 🚀 Como Funciona

### Automático:
1. Item é coletado → **Salva automaticamente**
2. Cena recarrega → **Item não aparece mais**

### Manual (Opcional):
- Adicione `SimpleItemLogger` em um GameObject para ver logs

---

## 📊 Logs no Console

**Ao carregar:**
```
📦 Item disponível: item_RedFruit
📦 Item disponível: item_appleA
```

**Ao coletar:**
```
✅ Item coletado e salvo: item_RedFruit
```

**Ao recarregar:**
```
🚫 Item já coletado anteriormente: item_RedFruit
📦 Item disponível: item_appleA
```

---

## 🔧 Resetar Dados

**Menu:** `Extra Tools > Items > Clear ALL Item Save Data`

**Ou clique direito no ItemPickup:** `Clear Save Data (This Item)`

---

## 📖 Documentação

Veja: `SIMPLE_GUIDE.md` para mais detalhes

---

**Pronto!** Agora teste coletando itens e recarregando a cena. ✨
