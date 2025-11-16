# 🔧 Debug Guide: Sistema de Cristais - Contadores não Atualizam

## 🚨 Problema Identificado

Os cristais estão sendo coletados corretamente, mas os contadores no HUD não estão aumentando.

## ✅ Logs de Funcionamento Detectados

```
[ItemCollectable] Cristal 'Nature Crystal' coletado (+1 Nature)
```

## ❌ Logs Ausentes (Esperados)

```
[GameManager] Cristal Nature adicionado: +1 (Total: 1)
[CrystalCounterUI] Contador de Nature conectado: Crystal_Nature/Count_Text
[CrystalCounterUI] Contador de Nature atualizado: 1
```

## 🔍 Verificações Necessárias

### 1. Verificar se CrystalCounterUI está na cena

**Passos:**

1. Selecionar `CanvasHUD` na hierarquia da cena
2. No Inspector, verificar se há o componente `CrystalCounterUI`
3. Se NÃO estiver presente → **ADICIONAR o componente**

### 2. Verificar logs do GameManager

**Problema provável:** Logs desabilitados no GameManager

**Solução:** Adicionar propriedade de logs no GameManager

## 🛠️ Soluções Implementadas

### Solução 1: Logs do GameManager

```csharp
// Adicionar no GameManager.cs
[Header("Debug Settings")]
[SerializeField] private bool enableDebugLogs = true;

// Modificar método Log para usar a flag
private void Log(string message)
{
    if (enableDebugLogs)
    {
        Debug.Log($"[GameManager] {message}");
    }
}
```

### Solução 2: Verificação de Conexão no CrystalCounterUI

```csharp
// Menu de contexto para debug
[ContextMenu("Debug Counter Status")]
private void EditorDebugCounterStatus()
{
    // Mostra status de conexão de todos os contadores
}
```

## 📋 Checklist de Debug

- [ ] `CanvasHUD` tem componente `CrystalCounterUI`
- [ ] `GameManager` tem logs habilitados
- [ ] Executar "Debug Counter Status" no `CrystalCounterUI`
- [ ] Verificar console para logs ausentes
- [ ] Testar coleta de cristal e observar logs

## 🎯 Log Esperado Após Correção

```
[GameManager] Sistema de cristais inicializado com todos os contadores zerados
[CrystalCounterUI] Inicialização concluída: 6/6 contadores conectados
[CrystalCounterUI] Subscrito aos eventos do GameManager
[ItemCollectable] Cristal 'Nature Crystal' coletado (+1 Nature)
[GameManager] Cristal Nature adicionado: +1 (Total: 1)
[CrystalCounterUI] Contador de Nature atualizado: 1
```

## 🚀 Próximos Passos

1. Implementar logs habilitáveis no GameManager
2. Verificar conexão UI na cena
3. Testar sistema completo
