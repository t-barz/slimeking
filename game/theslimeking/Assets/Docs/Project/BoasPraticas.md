# 📋 Boas Práticas de Desenvolvimento - The Slime King

## 🎯 Diretrizes Gerais

- Lembre-se que estamos utilizando Unity 6.2+ e que todo o código precisa ser compatível com essa versão.
- Sempre busque as funcionalidades mais recentes do C# 10 e do Unity 6.2+, evitando práticas obsoletas.
- Sempre busque a simplicidade e clareza no código.
- Sempre avalie a utilização de um sistema utilizando Eventos.
- NUNCA utilize emojis em nomes de arquivos, pastas, classes ou variáveis.
- **SEMPRE consulte o Roadmap.md para verificar prioridades e tarefas pendentes antes de iniciar qualquer desenvolvimento.**
- **Todas as atividades de desenvolvimento devem estar registradas no Roadmap.md - nunca implemente algo que não esteja documentado lá.**
- Sempre verifique se existe alguma documentação relacionada na pasta Assets/Docs antes de implementar algo novo.
- Utilize o idioma inglês para nomes de arquivos, pastas, classes e variáveis.
- Nunca crie códigos de exemplo ou testes a não ser que seja solicitado.
- Use camelCase para nomes de variáveis e métodos, e PascalCase para nomes de classes.
- Utilize o idioma português para comentários e documentação, mantendo a clareza e a compreensão do código.
- Sempre utilize as funcionalidades mais recentes do C# e do Unity, evitando práticas obsoletas.
- Não faça over engineering; implemente apenas o necessário para a funcionalidade atual.
- Priorize a performance seguida da legibilidade do código, evitando complexidade desnecessária e mantendo uma estrutura clara.
- O Sorting dos objetos deve ser feito via eixo Y por se tratar de um jogo 2D Top Down.
- Nunca utilize detecção direta de input, sempre utilize o Input System.
- Sempre utilize as soluções com melhor performance, evitando soluções que possam impactar negativamente o desempenho do jogo.
- Utilize o recurso de regiões para organizar o código em seções lógicas, facilitando a navegação e compreensão.
- Sempre que possível, utilize o padrão de projeto Singleton para gerenciar instâncias únicas de classes.
- Todas as classes devem ter a opção de ligar e desligar os logs e debug, permitindo que o desenvolvedor controle a verbosidade do log.
- A utilização de gizmos deve ser feita sempre que possível, para facilitar a visualização de elementos no editor mas sempre com a opção de desativar.
- Sempre que tiver sons, deve-se utilizar uma lista de sons possíveis e um sistema de seleção aleatória para evitar repetição excessiva dos mesmos sons.
- Não gere classes ou métodos de teste a não ser que seja pedido.
- Utilize números inteiros para contagem e iteração, evitando o uso de floats ou doubles quando não necessário.
- Mantenha a documentação atualizada e clara, facilitando a compreensão do código por outros desenvolvedores.

## 🏗️ Padrões Arquiteturais e Nomenclatura

### 📝 **Manager** - Gerenciadores de Sistema

**Quando usar:** Para sistemas globais únicos que coordenam múltiplas funcionalidades.

**Características:**

- Implementar como Singleton
- Responsável por inicialização e configuração de sistemas
- Coordena comunicação entre diferentes componentes
- Gerencia estado global do sistema
- Deve sobreviver entre mudanças de cena (quando necessário)

**Exemplos de uso:**

- `GameManager` - Estado geral do jogo, progressão do slime, vidas, cristais
- `AudioManager` - Reprodução e controle de áudio global
- `SaveManager` - Sistema de salvamento e carregamento
- `SceneManager` - Transições e carregamento de cenas
- `InputManager` - Mapeamento e distribuição de inputs

**Estrutura recomendada:**

```csharp
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    #region Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
```

### 🎮 **Controller** - Controladores de Entidade

**Quando usar:** Para controlar comportamento específico de uma entidade ou GameObject.

**Características:**

- Anexado diretamente ao GameObject que controla
- Responsável pela lógica de movimento, ações e estados da entidade
- Foca em uma única responsabilidade (controle da entidade)
- Pode se comunicar com Managers para ações globais
- Geralmente não é Singleton

**Exemplos de uso:**

- `PlayerController` - Movimento, pulo, ataques do jogador
- `EnemyController` - IA, patrulha, ataques de inimigos
- `CameraController` - Seguimento, efeitos de câmera
- `UIController` - Controle de painéis específicos de UI
- `BushController` - Comportamento das moitas destrutíveis

**Estrutura recomendada:**

```csharp
public class PlayerController : MonoBehaviour
{
    #region Movement Variables
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    #endregion
    
    #region Components
    private Rigidbody2D rb;
    private Animator animator;
    #endregion
}
```

### 🔄 **Handler** - Manipuladores de Eventos

**Quando usar:** Para processar eventos específicos, transições ou manipulação de dados.

**Características:**

- Responsável por uma funcionalidade muito específica
- Geralmente estático ou com métodos estáticos
- Processa entrada e retorna saída processada
- Pode ser usado por Controllers e Managers
- Foca em transformação ou processamento de dados

**Exemplos de uso:**

- `InputHandler` - Processar e filtrar inputs do jogador
- `CollisionHandler` - Processar diferentes tipos de colisões
- `DropHandler` - Gerenciar drops de itens e probabilidades
- `AnimationHandler` - Controlar transições complexas de animação
- `EventHandler` - Processar eventos de gameplay específicos

**Estrutura recomendada:**

```csharp
public static class InputHandler
{
    public static Vector2 GetMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        return new Vector2(horizontal, vertical).normalized;
    }
    
    public static bool GetJumpInput()
    {
        return Input.GetButtonDown("Jump");
    }
}
```

### 🎯 **System** - Sistemas Especializados

**Quando usar:** Para funcionalidades complexas que não se encaixam nos padrões acima.

**Características:**

- Sistemas que podem ter múltiplas instâncias
- Funcionalidades modulares e reutilizáveis
- Podem ser compostos por múltiplos componentes
- Frequentemente usados em conjunto com outros padrões

**Exemplos de uso:**

- `HealthSystem` - Sistema de vida e dano
- `InventorySystem` - Sistema de inventário
- `DialogueSystem` - Sistema de diálogos
- `QuestSystem` - Sistema de missões

## 🔗 Comunicação Entre Padrões

### **Hierarquia de Comunicação:**

```text
Manager (Coordena) 
    ↓ 
Controller (Executa)
    ↓ 
Handler (Processa)
    ↓ 
System (Especializa)
```

### **Regras de Comunicação:**

1. **Controllers** podem acessar **Managers** via Singleton
2. **Controllers** podem usar **Handlers** para processamento
3. **Managers** podem coordenar múltiplos **Controllers**
4. **Handlers** devem ser independentes e reutilizáveis
5. **Systems** podem ser usados por qualquer camada

### **Comunicação via Eventos:**

**Priorize eventos** para comunicação entre diferentes camadas quando:

- Um **Manager** precisa notificar múltiplos **Controllers**
- **Systems** precisam comunicar mudanças de estado
- **Controllers** precisam informar **Managers** sobre ações do jogador
- Múltiplos **Handlers** precisam reagir ao mesmo evento

**Hierarquia com Eventos:**

```text
Manager (Dispara eventos globais)
    ↕ (eventos bidirecionais)
Controller (Escuta + dispara eventos)
    ↕ (eventos de processamento)
Handler (Processa + notifica via eventos)
    ↕ (eventos especializados)
System (Eventos internos do sistema)
```

**Exemplo de Comunicação Híbrida:**

```csharp
// Manager dispara eventos globais
public class GameManager : MonoBehaviour
{
    public static event Action<GameState> OnGameStateChanged;
    
    private void ChangeGameState(GameState newState)
    {
        currentGameState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
}

// Controller escuta eventos e acessa Manager diretamente
public class PlayerController : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChange;
    }
    
    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChange;
    }
    
    private void HandleGameStateChange(GameState newState)
    {
        if (newState == GameState.Paused)
        {
            // Parar movimento
        }
    }
    
    private void Die()
    {
        // Acesso direto ao Manager
        GameManager.Instance.PlayerDied();
        
        // Dispara evento para outros sistemas
        PlayerEvents.OnPlayerDeath?.Invoke();
    }
}
```

### **Exemplo Prático:**

```csharp
// No PlayerController
public class PlayerController : MonoBehaviour
{
    private void Update()
    {
        // Handler processa o input
        Vector2 movement = InputHandler.GetMovementInput();
        
        // Controller executa o movimento
        MovePlayer(movement);
        
        // Manager é notificado sobre ações importantes
        if (InputHandler.GetJumpInput())
        {
            Jump();
            AudioManager.Instance.PlaySFX("jump");
        }
    }
}
```

## � Sistema de Eventos - Comunicação Desacoplada

### **🎯 Quando Usar Eventos**

Os eventos são fundamentais para criar sistemas desacoplados e escaláveis. Use eventos quando:

- **Múltiplos objetos** precisam reagir à mesma ação
- Você quer **baixo acoplamento** entre sistemas
- Precisa de **comunicação assíncrona** entre componentes
- Quer implementar o padrão **Observer** de forma elegante

### **🔄 Tipos de Eventos Recomendados**

#### **UnityEvent** - Para Eventos de UI e Inspector

**Quando usar:** Eventos que precisam ser configurados no Inspector do Unity.

```csharp
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnPlayerDeath;
    public UnityEvent<float> OnHealthChanged;
    
    private void Die()
    {
        OnPlayerDeath?.Invoke();
    }
    
    private void TakeDamage(float damage)
    {
        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth);
    }
}
```

#### **System.Action** - Para Eventos de Código

**Quando usar:** Eventos rápidos e performáticos entre scripts.

```csharp
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Eventos simples
    public static event Action OnGameStart;
    public static event Action OnGameEnd;
    
    // Eventos com parâmetros
    public static event Action<int> OnScoreChanged;
    public static event Action<Vector3> OnPlayerPositionChanged;
    
    private void StartGame()
    {
        OnGameStart?.Invoke();
    }
    
    private void UpdateScore(int newScore)
    {
        OnScoreChanged?.Invoke(newScore);
    }
}
```

#### **Custom Events** - Para Sistemas Complexos

**Quando usar:** Para eventos que carregam dados complexos ou precisam de mais controle.

```csharp
using System;
using UnityEngine;

// EventArgs customizado para dados complexos
public class PlayerEventArgs : EventArgs
{
    public Vector3 Position { get; set; }
    public float Health { get; set; }
    public int Level { get; set; }
}

public class Player : MonoBehaviour
{
    // Evento customizado
    public static event EventHandler<PlayerEventArgs> OnPlayerStateChanged;
    
    private void UpdatePlayerState()
    {
        var eventArgs = new PlayerEventArgs
        {
            Position = transform.position,
            Health = currentHealth,
            Level = currentLevel
        };
        
        OnPlayerStateChanged?.Invoke(this, eventArgs);
    }
}
```

### **🏗️ Padrão EventManager - Centralização de Eventos**

Para projetos maiores, implemente um EventManager centralizado:

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    
    private Dictionary<string, Action<object[]>> eventDictionary;
    
    #region Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            eventDictionary = new Dictionary<string, Action<object[]>>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    
    #region Event Management
    public void Subscribe(string eventName, Action<object[]> listener)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] += listener;
        }
        else
        {
            eventDictionary[eventName] = listener;
        }
    }
    
    public void Unsubscribe(string eventName, Action<object[]> listener)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] -= listener;
        }
    }
    
    public void TriggerEvent(string eventName, params object[] parameters)
    {
        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName]?.Invoke(parameters);
        }
    }
    #endregion
}
```

### **📋 Eventos por Categoria de Sistema**

#### **🎮 Gameplay Events**

```csharp
public static class GameplayEvents
{
    // Player
    public static event Action<Vector3> OnPlayerMove;
    public static event Action OnPlayerJump;
    public static event Action<float> OnPlayerTakeDamage;
    public static event Action OnPlayerDeath;
    
    // Enemies
    public static event Action<GameObject> OnEnemySpawn;
    public static event Action<GameObject> OnEnemyDeath;
    
    // Items
    public static event Action<string> OnItemCollected;
    public static event Action<string, int> OnItemUsed;
}
```

#### **🔊 Audio Events**

```csharp
public static class AudioEvents
{
    public static event Action<string> OnPlaySFX;
    public static event Action<string> OnPlayMusic;
    public static event Action OnStopMusic;
    public static event Action<float> OnVolumeChanged;
}
```

#### **💾 Save/Load Events**

```csharp
public static class SaveEvents
{
    public static event Action OnGameSaved;
    public static event Action OnGameLoaded;
    public static event Action<string> OnSaveError;
}
```

### **🔄 Melhores Práticas para Eventos**

#### **✅ Faça:**

1. **Use null-conditional operator:** `OnEvent?.Invoke()`
2. **Desinscreva eventos:** Sempre unsubscribe no OnDestroy
3. **Nomeação consistente:** Use padrão `On[Ação][Objeto]`
4. **Documentação:** Comente quando e por que o evento é disparado
5. **Performance:** Prefira Action para eventos simples

```csharp
public class ExampleController : MonoBehaviour
{
    private void OnEnable()
    {
        // Sempre subscribe no OnEnable
        GameplayEvents.OnPlayerDeath += HandlePlayerDeath;
    }
    
    private void OnDisable()
    {
        // Sempre unsubscribe no OnDisable
        GameplayEvents.OnPlayerDeath -= HandlePlayerDeath;
    }
    
    private void HandlePlayerDeath()
    {
        // Lógica de resposta ao evento
        Debug.Log("Player morreu!");
    }
}
```

#### **❌ Evite:**

1. **Memory leaks:** Esquecer de unsubscribe
2. **Eventos em loops:** Performance ruim
3. **Muitos parâmetros:** Use custom EventArgs
4. **Eventos síncronos pesados:** Considere coroutines
5. **Dependências circulares:** Entre eventos

### **🎯 Exemplo Prático - Sistema de Moedas**

```csharp
// Evento global para sistema de moedas
public static class CurrencyEvents
{
    public static event Action<int> OnCoinsChanged;
    public static event Action<int> OnCoinsSpent;
    public static event Action<int> OnCoinsEarned;
}

// Manager que gerencia as moedas
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    
    [SerializeField] private int currentCoins = 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        CurrencyEvents.OnCoinsEarned?.Invoke(amount);
        CurrencyEvents.OnCoinsChanged?.Invoke(currentCoins);
    }
    
    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            CurrencyEvents.OnCoinsSpent?.Invoke(amount);
            CurrencyEvents.OnCoinsChanged?.Invoke(currentCoins);
            return true;
        }
        return false;
    }
}

// UI que escuta as mudanças
public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI coinsText;
    
    private void OnEnable()
    {
        CurrencyEvents.OnCoinsChanged += UpdateCoinsDisplay;
    }
    
    private void OnDisable()
    {
        CurrencyEvents.OnCoinsChanged -= UpdateCoinsDisplay;
    }
    
    private void UpdateCoinsDisplay(int newAmount)
    {
        coinsText.text = newAmount.ToString();
    }
}

// Item que adiciona moedas quando coletado
public class CoinPickup : MonoBehaviour
{
    [SerializeField] private int coinValue = 10;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CurrencyManager.Instance.AddCoins(coinValue);
            Destroy(gameObject);
        }
    }
}
```

### **⚡ Performance e Otimização de Eventos**

- **Use object pooling** para eventos que criam GameObjects
- **Implemente delays** para eventos muito frequentes
- **Cache delegates** quando possível
- **Considere UnityEngine.Pool** para EventArgs customizados
- **Monitore subscriber count** em eventos críticos

## �📁 Organização por Responsabilidade

### **Code/Systems/**

- Managers globais
- Sistemas base do jogo
- Singletons de arquitetura
- **EventManager** e classes de eventos estáticos

### **Code/Gameplay/**

- Controllers de entidades
- Handlers específicos do gameplay
- Lógica de mecânicas do jogo
- **Eventos específicos de gameplay** (PlayerEvents, EnemyEvents, etc.)

### **Code/Editor/**

- Ferramentas de desenvolvimento
- Scripts que executam apenas no editor
- Automatizações de workflow

## ⚡ Considerações de Performance

### **Geral:**

- **Managers:** Cache de referências, evite busca por nome/tag
- **Controllers:** Use FixedUpdate apenas para física, Update para inputs
- **Handlers:** Métodos estáticos quando possível para evitar alocação
- **Systems:** Implemente pooling quando necessário para objetos frequentes

### **Eventos:**

- **Subscribe/Unsubscribe:** Sempre balanceie no OnEnable/OnDisable
- **EventArgs customizados:** Use object pooling para eventos frequentes
- **Eventos frequentes:** Implemente throttling ou debouncing
- **Delegates vazios:** Sempre use null-conditional operator (?.)

## 💎 Configuração de Itens Coletáveis

### **Cristais Elementais - Configuração Correta**

**IMPORTANTE:** Para cristais funcionarem com atração magnética e coleta automática:

#### **✅ Configuração Recomendada (Apenas CrystalData)**

```csharp
// GameObject: "Nature_Crystal"
// Componente: ItemCollectable
Crystal Data: [NatureCrystalData] ✅ Preencher
Item Data: [VAZIO] ✅ Deixar vazio
Inventory Item Data: [VAZIO] ✅ Deixar vazio
Enable Attraction: true ✅
```

#### **🔧 Configuração Flexível (Híbrido)**

```csharp
// Para cristais com atração customizada
Crystal Data: [CrystalElementalData] ✅
Item Data: [CustomAttractConfig] ✅ Para configurações personalizadas
Inventory Item Data: [VAZIO] ✅
```

#### **❌ Configurações Incorretas**

```csharp
// NÃO FUNCIONA: Cristal sem dados
Crystal Data: [VAZIO] ❌
Item Data: [VAZIO] ❌

// FUNCIONA MAS VAI PARA INVENTÁRIO: Cristal como item
Crystal Data: [VAZIO] ❌
Item Data: [VAZIO]
Inventory Item Data: [SomeItemData] ⚠️ Vai para inventário, não para contador
```

### **Sistema de Prioridades de Coleta**

1. **🥇 Cristais:** `crystalData != null` → `GameManager.AddCrystal()`
2. **🥈 Inventário:** `inventoryItemData != null` → `InventoryManager.AddItem()`  
3. **🥉 Sistema Legado:** `itemData != null` → Aplica efeitos diretos

### **Valores Padrão para Cristais**

Quando apenas `crystalData` está configurado:

- **Attraction Radius:** 2.5f unidades
- **Attraction Speed:** 4.0f unidades/segundo
- **Visual Color:** Baseado em `crystalData.crystalTint`
- **Effects:** Baseado em `crystalData.collectVFX` e `collectSound`

### **Logs de Depuração**

Monitore estes logs para validar configuração:

```
[ItemCollectable] Cristal {name} inicializado com configurações padrão
[ItemCollectable] {name} ativou atração magnética após 0.5s
[ItemCollectable] Cristal {name} coletado (+{value} {type})
```

### **Troubleshooting Comum**

| Problema | Causa | Solução |
|----------|-------|---------|
| Cristal não é atraído | `crystalData` e `itemData` vazios | Preencher `crystalData` |
| Vai para inventário | `inventoryItemData` preenchido | Deixar `inventoryItemData` vazio |
| Sem efeitos visuais | `collectVFX`/`collectSound` vazios no `CrystalElementalData` | Configurar efeitos no ScriptableObject |

**📚 Documentação Completa:** Consulte `Assets/Docs/Crystal_Configuration_Guide.md`
