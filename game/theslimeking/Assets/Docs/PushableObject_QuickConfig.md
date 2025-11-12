# 🛠️ PushableObject Quick Config - Guia de Uso

Documentação da ferramenta de configuração automática para PushableObjects.

## 🚀 Acesso Rápido

### Via Menu Principal

```
Tools → SlimeKing → Configure PushableObject
```

### Via Botão Objeto Selecionado

1. Selecione o GameObject no Hierarchy
2. No Inspector, procure pelo botão **"Configure PushableObject"**
3. Clique para configuração automática

## ⚡ Configuração Automática

A ferramenta **PushableObject Quick Config** realiza configuração completa e inteligente:

### 🎯 Detecção de Hierarquia

**Objeto Simples**:

- PushableObject no objeto principal
- Rigidbody2D no mesmo objeto
- Movimento aplicado diretamente

**Objeto Composto** (novo):

- Detecta se objeto tem pai com Rigidbody2D
- PushableObject como detector/rotacionador
- Movimento aplicado ao pai automaticamente

### ⚙️ Componentes Configurados

#### 📦 PushableObject

```csharp
// Configurações padrão aplicadas:
pushDirection = PushDirection.North;
pushDistance = 2.0f;
pushDuration = 1.5f;
rotationSpeed = 180f;
maxUses = -1; // Infinito
enableDebugLogs = false;
```

#### ⚖️ Rigidbody2D (massa alta para estabilidade)

```csharp
// Configuração Unity 6.2+ compatível:
bodyType = RigidbodyType2D.Dynamic;
gravityScale = 0f;
linearDamping = 5f;    // Novo: substitui drag
angularDamping = 5f;   // Novo: substitui angularDrag
mass = 100000f;        // Massa alta para estabilidade
```

#### 🔍 CircleCollider2D (Detector de Player)

```csharp
// Configuração automática:
isTrigger = true;
radius = 1.5f; // Ajustável conforme necessário
```

## 🏗️ Suporte a Objetos Compostos

### 🎯 Detecção Inteligente

A ferramenta automaticamente detecta a estrutura do objeto:

**Cenário 1: Objeto Simples**

```
📦 SimpleBox
   └── PushableObject (adicionado aqui)
   └── Rigidbody2D (adicionado aqui)
```

**Cenário 2: Objeto Composto**

```
🏗️ ComplexMachine
   ├── Rigidbody2D (já existe ou adicionado aqui)
   └── 📦 DetectorChild (objeto selecionado)
       └── PushableObject (adicionado aqui)
```

### 🛡️ Validação Automática

- ✅ Verifica se já existem componentes necessários
- ✅ Detecta hierarquia pai-filho
- ✅ Configura física adequadamente
- ✅ Aplica configurações de massa específicas

## 🎛️ Opções de Configuração

### 🔧 Debug Tools

**Debug Completo**:

```csharp
// Ativa logs detalhados para desenvolvimento
enableDebugLogs = true;
```

**Verificação de Setup**:

- Lista todos os componentes criados/configurados
- Informa sobre detecção de hierarquia
- Mostra configurações aplicadas

### ⚖️ Configuração de Massa

**Massa Padrão**: `100000f`

- Valor alto para estabilidade em física
- Evita movimentos indesejados
- Compatível com sistema de empurrão

## 📋 Exemplo de Uso Completo

### 1. Configuração Simples

```csharp
// 1. Crie um GameObject
var simpleBox = new GameObject("SimpleBox");

// 2. Execute Quick Config
// Tools → SlimeKing → Configure PushableObject
// (com simpleBox selecionado)

// ✅ Resultado: Objeto pronto para uso
```

### 2. Configuração Composta

```csharp
// 1. Estrutura existente
ComplexMachine/
├── Rigidbody2D (já existe)
├── VisualParts/
└── InteractionPoint (vazio)

// 2. Selecione 'InteractionPoint'
// 3. Execute Quick Config

// ✅ Resultado: PushableObject em InteractionPoint
//             ComplexMachine será movido
```

## 🚨 Resolução de Problemas

### ❌ Problemas Comuns

**"Componente não adicionado"**:

- ✅ Verifique se GameObject está selecionado
- ✅ Certifique-se de que não é um Prefab locked

**"Objeto não se move corretamente"**:

- ✅ Confirme configuração de Rigidbody2D
- ✅ Verifique se massa está configurada (100000)

**"Detecção de Player não funciona"**:

- ✅ Verifique se CircleCollider2D é Trigger
- ✅ Confirme se Player tem as tags corretas

### 🔍 Validação Manual

```csharp
// Script para verificar configuração:
var pushable = GetComponent<PushableObject>();
var rb = GetComponent<Rigidbody2D>() ?? GetComponentInParent<Rigidbody2D>();
var detector = GetComponent<CircleCollider2D>();

Debug.Log($"PushableObject: {pushable != null}");
Debug.Log($"Rigidbody2D: {rb != null} (massa: {rb?.mass})");
Debug.Log($"Detector: {detector != null} (trigger: {detector?.isTrigger})");
Debug.Log($"Move Parent: {pushable?.IsMovingParent}");
```

## 🎯 Best Practices

### ✅ Recomendações

1. **Sempre use Quick Config**: Evita erros de configuração manual
2. **Teste hierarquia**: Verifique se está movendo o objeto correto
3. **Ajuste massa**: Modifique se necessário para seu caso específico
4. **Debug durante desenvolvimento**: Ative logs para entender comportamento

### ⚠️ Cuidados

1. **Não modifique manualmente**: Use Quick Config para consistência
2. **Verifique Parent**: Certifique-se de que hierarquia está correta
3. **Performance**: Desative debug logs em produção

---

## 📝 Changelog

- **v1.3** - Adicionado suporte a objetos compostos
- **v1.2** - Configuração de massa automática (100000)  
- **v1.1** - Compatibilidade Unity 6.2+ (linearDamping/angularDamping)
- **v1.0** - Versão inicial com configuração básica

---

💡 **Dica**: Use sempre o Quick Config para garantir configuração perfeita e compatível com as últimas versões do Unity!
