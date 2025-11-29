# 🚀 PushableObject Quick Config - Ferramenta de Editor

## 🎯 Visão Geral

A ferramenta **PushableObject Quick Config** automatiza completamente a configuração de objetos empurráveis no SlimeKing, seguindo todos os padrões e melhores práticas do projeto.

## ✨ Funcionalidades

### 🔧 Configuração Automática

- ✅ **CircleCollider2D**: Adiciona e configura como Trigger
- ✅ **Rigidbody2D**: Configurado para física 2D otimizada
- ✅ **PushableObject**: Componente principal com configurações padrão
- ✅ **Unity 6.2+ Compatible**: Usa APIs mais recentes (linearDamping, etc.)

### 🛠️ Ferramentas de Debug

- ✅ **Informações Detalhadas**: Debug completo da configuração
- ✅ **Validação**: Verifica configuração e possíveis problemas
- ✅ **Helper Methods**: Métodos públicos para verificação programática

## 🎮 Como Usar

### 1. Configuração Básica

```text
1. Selecione um GameObject na hierarquia
2. Clique com botão direito
3. GameObject > Quick Config > 📦 Configure as Pushable Object
4. Pronto! Objeto configurado automaticamente
```

### 2. Debug e Verificação

```text
1. Selecione um PushableObject
2. GameObject > Quick Config > 📦 Debug Pushable Object Info
3. Veja informações completas no Console
```

## 🔧 Configurações Aplicadas

### CircleCollider2D

```csharp
isTrigger = true                    // Para detecção do Player
radius = spriteSize * 0.7f          // 70% do sprite (automático)
// ou radius = 0.75f               // Valor padrão se sem sprite
```

### Rigidbody2D (Unity 6.2+)

```csharp
bodyType = RigidbodyType2D.Dynamic  // Movimento dinâmico
gravityScale = 0f                   // Remove gravidade (2D top-down)
mass = 100000f                      // Massa alta para objetos pesados
linearDamping = 5f                  // Resistência linear
angularDamping = 5f                 // Resistência angular
freezeRotation = false              // Permite rotação
collisionDetectionMode = Continuous // Detecção contínua
```

### SpriteRenderer (se presente)

```csharp
sortingLayerName = "Default"        // Layer padrão
sortingOrder = -positionY * 100     // Baseado na posição Y
```

## 🧰 Métodos Helper Públicos

### Verificação de Configuração

```csharp
using SlimeKing.Editor;

// Verifica se está configurado
bool isConfigured = PushableObjectQuickConfig.IsPushableObjectConfigured(gameObject);

// Validação de problemas
string error = PushableObjectQuickConfig.ValidatePushableObjectSetup(gameObject);
if (error != null)
{
    Debug.LogWarning($"Problema: {error}");
}
```

### Configuração Programática

```csharp
using SlimeKing.Editor;

// Configura via código (apenas no Editor)
#if UNITY_EDITOR
PushableObjectQuickConfig.ConfigurePushableObjectComponents(gameObject);
#endif
```

## 📋 Recursos Inteligentes

### 🔍 Auto-Detection

- **Sprite Size**: Raio do collider baseado no tamanho do sprite
- **Existing Components**: Remove componentes conflitantes
- **Sorting Order**: Calcula baseado na posição Y do objeto

### 🛡️ Safety Features

- **Undo Support**: Todas as modificações suportam Ctrl+Z
- **Error Handling**: Tratamento robusto de erros
- **Validation**: Verifica pré-requisitos antes da configuração

### 🔄 Smart Updates

- **Non-Destructive**: Preserva componentes compatíveis
- **Override Conflicts**: Remove apenas componentes conflitantes
- **Preserve Settings**: Mantém configurações do usuário quando possível

## 🎯 Menu Locations

### Quick Config

```text
GameObject (Right-click menu)
└── Quick Config
    ├── 🪨 Configure as Item
    ├── 🌿 Configure as Bush
    └── 📦 Configure as Pushable Object    ← NOVO!
```

### Debug Tools

```text
GameObject (Right-click menu)
└── Quick Config
    └── 📦 Debug Pushable Object Info      ← NOVO!
```

## 📊 Output de Debug

Exemplo de saída do Debug Tool:

```text
📦 === DEBUG INFO: Rock_Pushable ===
✅ Configurado como PushableObject: True
📦 PushableObject: ✅
🔴 CircleCollider2D: ✅
   - Trigger: True, Raio: 0.52
🎯 Rigidbody2D: ✅
   - BodyType: Dynamic, Gravity: 0, Mass: 100000
```

## 🚀 Vantagens

### ⚡ Produtividade

- **Zero Configuração Manual**: Tudo automático
- **Padrões Consistentes**: Sempre configurado corretamente
- **Rapid Prototyping**: Prototipagem super rápida

### 🛠️ Manutenibilidade

- **Código Centralizado**: Uma fonte de verdade
- **Fácil Atualização**: Mudanças aplicadas em todos os objetos
- **Debug Simplificado**: Ferramentas integradas

### 🔧 Flexibilidade

- **Configuração Base**: Sólida fundação para customização
- **Extensível**: Fácil de expandir com novos recursos
- **Compatível**: Funciona com objetos existentes

## 🎮 Fluxo de Trabalho Recomendado

```text
1. 🎨 Crie GameObject com Sprite
2. 📦 Quick Config > Configure as Pushable Object
3. ⚙️ Ajuste configurações no Inspector (opcional)
4. 🔍 Debug Pushable Object Info (se necessário)
5. ✅ Teste no jogo!
```

## 📚 Integração com Outros Sistemas

### Quest System

```csharp
// Conta objetos empurráveis configurados
var pushableObjects = FindObjectsOfType<PushableObject>()
    .Where(p => PushableObjectQuickConfig.IsPushableObjectConfigured(p.gameObject));
```

### Level Design

```csharp
// Configura múltiplos objetos
foreach (var obj in selectedObjects)
{
    if (PushableObjectQuickConfig.ValidatePushableObjectSetup(obj) == null)
    {
        PushableObjectQuickConfig.ConfigurePushableObjectComponents(obj);
    }
}
```

---

✅ **Ferramenta de produtividade completa!**  
🚀 **Integrada ao workflow do projeto**  
🎯 **Zero configuração manual necessária**
