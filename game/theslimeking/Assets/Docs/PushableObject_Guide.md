# 📦 PushableObject - Guia de Implementação

## 🎯 Visão Geral

O `PushableObject` é um sistema completo para objetos que podem ser empurrados pelo jogador no SlimeKing. Ele segue todas as boas práticas do projeto e se integra perfeitamente com o sistema de interação do Player.

## ✨ Funcionalidades Implementadas

- ✅ **Detecção de Player**: Automaticamente detecta quando o Player está próximo
- ✅ **Movimento Direcional**: Suporte para 4 direções (Norte, Sul, Leste, Oeste)
- ✅ **Rotação Direcional**: Objeto rotaciona no sentido correto baseado na direção
  - 🔄 **Leste/Sul**: Rotação horária
  - 🔄 **Norte/Oeste**: Rotação anti-horária
- ✅ **Usos Limitados**: Sistema de controle de quantidade máxima de interações
  - 🔢 **Configurável**: Define quantas vezes pode ser empurrado
  - ♾️ **Ilimitado**: Valor -1 permite uso infinito
- ✅ **Sistema de Áudio**: Sons para início e durante o movimento
- ✅ **Interface IInteractable**: Integração com sistema genérico de interações
- ✅ **Logs Controláveis**: Debug opcional para desenvolvimento
- ✅ **Gizmos Visuais**: Indicação visual da direção no Editor
- ✅ **Unity 6.2+ Compatible**: Usa APIs mais recentes

## 🛠️ Como Usar

### 1. Configuração Rápida (Recomendado) 🚀

**Use a ferramenta Quick Config para configuração automática:**

```text
1. Selecione um GameObject na hierarquia
2. Clique com botão direito > GameObject > Quick Config > 📦 Configure as Pushable Object
3. Tudo será configurado automaticamente!
```

**O Quick Config adiciona e configura:**

- ✅ CircleCollider2D (como Trigger, raio automático)
- ✅ Rigidbody2D (configurado para jogo 2D)
- ✅ Componente PushableObject
- ✅ Configurações otimizadas para Unity 6.2+

### 2. Configuração Manual (Avançado)

Se preferir configurar manualmente:

```
1. Crie um GameObject na cena
2. Adicione um SpriteRenderer (opcional, para visual)
3. Adicione um Collider2D:
   - Configure como Trigger ✅
   - Ajuste o tamanho para área de detecção do Player
4. Adicione um Rigidbody2D:
   - O script configurará automaticamente
5. Adicione o script PushableObject
```

### 2. Configuração do Inspector

#### ⚙️ Configurações de Movimento

- **Push Direction**: Escolha entre North, South, East, West
- **Move Speed**: Velocidade do movimento (padrão: 3 unidades/seg)
- **Move Duration**: Duração do movimento (padrão: 2 segundos)
- **Rotation Speed**: Velocidade de rotação (padrão: 90 graus/seg)

#### 🔢 Configurações de Uso

- **Max Uses**: Quantidade máxima de interações (-1 = ilimitado)
  - 🔢 **Valores positivos**: Número exato de usos permitidos
  - ♾️ **Valor -1**: Uso infinito (padrão)
  - 🚫 **Valor 0**: Objeto desabilitado desde o início

#### 🎧 Configurações de Áudio

- **Push Sound**: Som quando iniciar o movimento
- **Moving Sound**: Som durante o movimento (loop)

#### 🔧 Debug

- **Enable Debug Logs**: Ativa logs detalhados
- **Show Interaction Gizmos**: Mostra direção no Editor

### 3. Configuração do Player

O sistema já está integrado automaticamente no `PlayerController`. Não é necessário configuração adicional.

## 🔧 Arquivos Criados/Modificados

### Novos Arquivos

1. **`PushableObject.cs`** - Componente principal
2. **`IInteractable.cs`** - Interface para objetos interativos
3. **`InteractionHandler.cs`** - Sistema de detecção de interações
4. **`PushableObjectQuickConfig.cs`** - Ferramenta de configuração rápida (Editor)

### Arquivos Modificados

1. **`PlayerController.cs`** - Integração com sistema de interação

## 🎮 Como Funciona no Jogo

1. **Detecção**: Player se aproxima do objeto (Trigger)
2. **Interação**: Player pressiona [E] para empurrar
3. **Movimento**: Objeto se move na direção configurada
4. **Rotação Direcional**:
   - 🔄 **Leste/Sul**: Gira no sentido horário
   - 🔄 **Norte/Oeste**: Gira no sentido anti-horário
5. **Áudio**: Sons são reproduzidos automaticamente
6. **Finalização**: Objeto para após o tempo configurado

### 🔄 Sistema de Rotação Inteligente

A rotação do objeto é automaticamente determinada pela direção do movimento:

| Direção | Sentido da Rotação | Descrição |
|---------|-------------------|-----------|
| **Norte** ⬆️ | Anti-horário ↺ | Rotação negativa |
| **Sul** ⬇️ | Horário ↻ | Rotação positiva |
| **Leste** ➡️ | Horário ↻ | Rotação positiva |
| **Oeste** ⬅️ | Anti-horário ↺ | Rotação negativa |

> **💡 Dica**: Esta lógica cria um movimento mais natural e visualmente agradável!

### 🔢 Sistema de Usos Limitados

O PushableObject agora suporta controle de quantidade de interações:

| Valor | Comportamento | Uso Recomendado |
|-------|---------------|-----------------|
| **-1** | ♾️ Infinito | Objetos reutilizáveis, training areas |
| **0** | 🚫 Desabilitado | Objetos temporariamente bloqueados |
| **1** | ⚠️ Uso único | Puzzles, elementos críticos do level |
| **2-5** | 🔢 Limitado | Objetos que "quebram" ou "desgastam" |
| **6+** | 🔄 Multi-uso | Objetos duráveis mas não infinitos |

#### 💭 **Casos de Uso:**

- **🧩 Puzzles**: Pedras que só podem ser movidas uma vez
- **⚖️ Recursos**: Caixas que "quebram" após alguns usos  
- **🎯 Challenges**: Objetos com usos limitados para dificuldade
- **🔄 Training**: Objetos infinitos para prática do Player

> **💡 Dica**: Use usos limitados para criar tensão e decisões estratégicas!

## 📋 Exemplo de Setup Completo

```csharp
// Configurações recomendadas para diferentes tipos de objetos:

// PEDRA PEQUENA (uso único para puzzles)
moveSpeed = 2f;
moveDuration = 1.5f;
rotationSpeed = 180f;  // Velocidade alta para efeito visual
pushDirection = North; // ↺ Anti-horário
maxUses = 1;          // ⚠️ Só pode ser empurrada uma vez

// CAIXA GRANDE (rotação horária, uso limitado)
moveSpeed = 1f;
moveDuration = 3f;
rotationSpeed = 45f;   // Velocidade baixa para objeto pesado
pushDirection = East;  // ↻ Horário  
maxUses = 3;          // 🔢 Pode ser empurrada 3 vezes

// BARRIL (rotação horária, uso ilimitado)
moveSpeed = 2.5f;
moveDuration = 2f;
rotationSpeed = 90f;   // Velocidade média
pushDirection = South; // ↻ Horário
maxUses = -1;         // ♾️ Uso infinito

// BLOCO DE GELO (rotação anti-horária, frágil)
moveSpeed = 1.5f;
moveDuration = 4f;
rotationSpeed = 60f;   // Rotação lenta e suave
pushDirection = West;  // ↺ Anti-horário
maxUses = 2;          // 🧊 "Derrete" após 2 usos
```

## 🔍 Debug e Troubleshooting

### 🛠️ Ferramentas de Debug

**Quick Config Debug Tool:**

- Selecione um GameObject na hierarquia
- Clique: GameObject > Quick Config > 📦 Debug Pushable Object Info
- Mostra estado completo da configuração no Console

**Verificação de Configuração:**

```csharp
// Verifica se está configurado corretamente
bool isConfigured = PushableObjectQuickConfig.IsPushableObjectConfigured(gameObject);

// Valida possíveis problemas
string validationError = PushableObjectQuickConfig.ValidatePushableObjectSetup(gameObject);
```

### Problemas Comuns

**🚫 "Player não detectado"**

- Verifique se o Player tem tag "Player"
- Certifique-se que o Collider2D está marcado como Trigger
- Verifique se o Player está dentro da área do Collider

**🚫 "Objeto não se move"**

- Confirme que o Rigidbody2D está presente
- Verifique se bodyType = Dynamic
- Certifique-se que gravityScale = 0
- Confirme que mass está configurada (Quick Config usa 100000)

**🚫 "Interação não funciona"**

- Verifique se enableDebugLogs está ativo para diagnóstico
- Confirme que o PlayerController tem o InteractionHandler
- Verifique se há conflitos com SpecialMovementPoints
- **NOVO**: Confirme se ainda há usos disponíveis (maxUses não foi atingido)

**🚫 "Objeto não responde mais"**

- Verifique se o limite de usos foi atingido
- Console mostrará "Limite de usos atingido" se enableDebugLogs = true
- Verifique se maxUses está configurado corretamente (-1 = infinito)

### Debug Avançado

```csharp
// No Inspector do PushableObject:
// ✅ Enable Debug Logs = true
// ✅ Show Interaction Gizmos = true

// No Inspector do PlayerController:
// ✅ Enable Logs = true

// Console mostrará:
// [PushableObject-NomeObjeto] Player entrou na área de interação
// [PlayerController] Interação bem-sucedida com objeto IInteractable
// [PushableObject-NomeObjeto] Iniciando movimento na direção North por 2 segundos
```

## 🚀 Extensões Futuras

O sistema foi projetado para ser facilmente extensível:

- **Múltiplos Empurrões**: Adicionar contador de usos
- **Obstáculos**: Detectar colisões e parar movimento
- **Triggers**: Ativar outros objetos ao chegar no destino
- **Efeitos Visuais**: Partículas durante movimento
- **Sons Direcionais**: Áudio espacial baseado na posição

## 🎯 Integração com Outros Sistemas

O `PushableObject` usa a interface `IInteractable`, facilitando:

- **Sistemas de Quest**: Objetivos "empurre X objetos"
- **Puzzles**: Combinação com switches e pressure plates
- **Inventário**: Items que afetam capacidade de empurrar
- **Multiplayer**: Sincronização de movimento entre clientes

---

✅ **Sistema completamente funcional e integrado!**  
🎮 **Pronto para uso em produção**  
📚 **Documentado seguindo padrões do projeto**
