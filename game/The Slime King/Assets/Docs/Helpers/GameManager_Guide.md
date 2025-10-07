# GameManager_Guide

## Visão Geral

O `GameManager` é o orquestrador central do The Slime King. Ele concentra o controle de:

- Estado global do jogo (ciclo: Splash → Menu → Loading → Exploring → estados derivados)
- Progressão do Slime (fragmentos, evolução, vidas)
- Sistema de amizade e expansões do lar
- Controle de bioma atual
- Tempo total e tempo de sessão
- Aplicação de configurações (gráficas e futuras de áudio/gameplay)
- Emissão de eventos globais para desacoplamento de sistemas

Ele NÃO deve conter lógica pesada de combate, IA, inventário detalhado, save system, áudio, etc. Isso pertence a outros managers/systems especializados.

## Arquitetura

### Padrões Aplicados

- Singleton persistente (`DontDestroyOnLoad`)
- State Machine explícita (enum `GameState` + validação de transições)
- Event-Driven (via `GameManagerEvents` e outros grupos de eventos)
- Segregação de responsabilidades por regiões (organizacional)
- Encapsulamento: nenhuma escrita direta em campos críticos fora desta classe

### Principais Enums

| Enum | Propósito |
|------|-----------|
| `GameState` | Fluxo macro do jogo |
| `SlimeStage` | Estágios evolutivos (Filhote → ReiSlime) |
| `ElementType` | Tipos de cristais coletáveis |

### Estrutura de Eventos

Eventos são agrupados por domínio em `GameEvents.cs`:

- `GameManagerEvents` (estados, evolução, vidas, bioma, configurações, tempo)
- `PlayerEvents` (morte, respawn, movimento, dano, etc.)
- `SlimeEvents` (absorção de cristais, evolução solicitada/completada)
- `CreatureEvents` (amizade, visitas, lar)
- `BiomeEvents` (entrada, saída, descobertas)
- `UIEvents` (ações iniciadas pela interface)
- `GameFlowEvents` (objetivos, áreas, game complete)

Todos possuem métodos `Raise...` para disparo seguro (evita CS0070 e centraliza padrão).

## Ciclo de Estados

| Origem | Destinos Válidos |
|--------|------------------|
| Splash | MainMenu |
| MainMenu | Options, Credits, Loading |
| Options | MainMenu, Exploring |
| Credits | MainMenu |
| Loading | Exploring |
| Exploring | Paused, Inventory, SkillTree, Interacting, Death, Evolution, Victory, MainMenu |
| Paused | Exploring, Options, MainMenu |
| Inventory | Exploring |
| SkillTree | Exploring |
| Interacting | Exploring |
| Death | Exploring, MainMenu |
| Evolution | Exploring |
| Victory | Loading, MainMenu |

Transições inválidas são ignoradas com log de aviso.

## Fluxo de Inicialização

1. Cena inicial contém um `GameObject` com `GameManager` ou é instanciado dinamicamente.
2. `Awake()` configura Singleton e chama `InitializeGameManager()`.
3. Carrega splash (ou pula se `skipSplash = true`).
4. Aplica configurações e vai para `MainMenu`.
5. UI dispara `UIEvents.RaiseStartGameRequested()` → `GameManager` responde e muda para `Loading`.
6. Após sequência de loading → `Exploring`.

## Progressão do Slime

Fragmentos são armazenados por `ElementType` em um dicionário. Evolução acontece quando thresholds são atingidos:

- Filhote → Adulto: `fragmentsForAdulto`
- Adulto → GrandeSlime: `fragmentsForGrandeSlime`
- GrandeSlime → ReiSlime: `fragmentsForReiSlime` + aliados >= `aliadosRequiredForRei`

Eventos relevantes:

- `GameManagerEvents.OnCrystalFragmentsChanged(ElementType, int)`
- `GameManagerEvents.OnSlimeEvolved(SlimeStage)`
- `GameManagerEvents.OnNewElementUnlocked(ElementType)` (primeira coleta de um tipo)

## Sistema de Amizade e Lar

- Cada criatura tem um nome/chave (`string`).
- Amizade cresce via `IncreaseFriendship()` ou evento `CreatureEvents.RaiseFriendshipIncreased()`.
- Expansões exemplo:
        - JardimCristais (amizade >=3 com cervo/esquilo/ouriço)
        - LagoInterno (amizade >=5 com castor)
        - SotaoPanoramico (amizade >=10 com borboleta)
- Eventos: `GameManagerEvents.OnFriendshipChanged`, `GameManagerEvents.OnHomeExpansionUnlocked`.

## Configurações

Estrutura `GameSettings` com campos simples. Atualização via:

```csharp
GameManager.Instance.UpdateSetting<float>("mouseSensitivity", 1.25f);
```

Aplica imediatamente e dispara `GameManagerEvents.OnSettingsChanged`.
Persistência ainda é placeholder até implementação de `SaveManager`.

## Tempo

- `gameTime`: acumulado total durante execução.
- `sessionTime`: reiniciado quando jogo começa / carrega.
- Evento: `GameManagerEvents.OnTimeChanged(float)` emitido durante exploração.

## API Pública Essencial

| Método | Função |
|--------|--------|
| `StartNewGame()` | Novo jogo (reset progressão + loading) |
| `ContinueGame()` | Continua progresso salvo (futuro) |
| `PauseGame()` | Pausa se possível |
| `ResumeGame()` | Retoma do pause |
| `RestartGame()` | Reset total e volta para loading |
| `ReturnToMainMenu()` | Força transição para menu |
| `QuitGame()` | Fecha aplicação / para Play Mode |
| `AddCrystalFragments(elem, qtd)` | Soma fragmentos e avalia evolução |
| `IncreaseFriendship(nome, qtd)` | Aumenta amizade e avalia expansões |
| `CanEvolve()` | Verifica requisito atual |
| `GetNextEvolutionStage()` | Retorna estágio seguinte |
| `IsKingSlime()` | Já está no máximo? |
| `AddLife()` / `LoseLife()` | Gerencia vidas |

## Disparando Eventos Externos

Exemplos:

```csharp
// UI Botão "Start"
UIEvents.RaiseStartGameRequested();

// Coleta de cristal
SlimeEvents.RaiseCrystalAbsorbed(ElementType.Fire, 3);

// Altar de evolução
if (GameManager.Instance.CanEvolve())
    SlimeEvents.RaiseEvolutionTriggered(GameManager.Instance.GetNextEvolutionStage());

// Criatura interagida
CreatureEvents.RaiseFriendshipIncreased("Cervo Verde", 1);

// Entrar em bioma
BiomeEvents.RaiseBiomeEntered("Floresta Calma");
```

## Escutando Eventos

```csharp
private void OnEnable()
{
    GameManagerEvents.OnGameStateChanged += HandleState;
    GameManagerEvents.OnSlimeEvolved += HandleEvolution;
}

private void OnDisable()
{
    GameManagerEvents.OnGameStateChanged -= HandleState;
    GameManagerEvents.OnSlimeEvolved -= HandleEvolution;
}
```

## Painel de Debug

Ativar em Inspector:

- `enableDebugMode = true`
- `showDebugGUI = true`

Botões úteis: adicionar fragmentos, vidas, forçar evolução, morte e testar amizade.

## Extensões Planejadas

| Futuro | Observação |
|--------|-----------|
| SaveManager | Persistir progresso e configs |
| SceneManager dedicado | Carregamento assíncrono real |
| AudioManager | Aplicar volumes e mix |
| AchievementSystem | Reagir a eventos já emitidos |
| Quest/Objectives Manager | Usar `GameFlowEvents` |

## Boas Práticas

- Nunca chamar `ChangeGameState` de dentro de listeners que disparariam loops indiretos sem checagem.
- Evitar acoplamento circular (ex: UI chamando diretamente métodos e disparando eventos redundantes).
- Assine eventos somente enquanto objeto está ativo.
- Para testes: use proxies simples em cena para disparar eventos manualmente.

## FAQ Rápido

| Pergunta | Resposta |
|----------|----------|
| Posso instanciar outro GameManager? | Não. O segundo se autodestrói. Use `GameManager.Instance`. |
| Onde salvo progresso? | Implementar posteriormente no `SaveManager` (placeholders já existem). |
| Como adiciono novo estado? | Adicionar ao enum + ajustar `IsValidStateTransition` + `EnterState`/`ExitState`. |
| Por que eventos usam Raise? | Encapsula invocation e evita uso inadvertido fora da classe. |
| Por que reflection nas configs? | Flexibilidade sem getters/setters repetitivos; pode ser trocado por abordagem forte futuramente. |

## Exemplo Completo de Integração (UI Botões)

```csharp
using UnityEngine;
public class MainMenuButtons : MonoBehaviour
{
    public void OnStartClicked() => UIEvents.RaiseStartGameRequested();
    public void OnContinueClicked() => UIEvents.RaiseContinueGameRequested();
    public void OnOptionsClicked() => UIEvents.RaiseOptionsRequested();
    public void OnQuitClicked() => UIEvents.RaiseQuitRequested();
}
```

## Exemplo: Listener de Evolução

```csharp
using UnityEngine;
public class EvolutionFX : MonoBehaviour
{
    private void OnEnable() => GameManagerEvents.OnSlimeEvolved += PlayFX;
    private void OnDisable() => GameManagerEvents.OnSlimeEvolved -= PlayFX;
    private void PlayFX(SlimeStage stage) => Debug.Log($"Spawn FX para estágio {stage}");
}
```

---
Se precisar de um pacote de scripts utilitários (proxies + listeners de exemplo), é só pedir.
