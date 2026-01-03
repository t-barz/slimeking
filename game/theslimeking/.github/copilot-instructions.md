# 🤖 SlimeKing Copilot Instructions

Guia rápido e específico para agentes de IA atuarem produtivamente neste projeto Unity.

## Visão Geral da Arquitetura
- Projeto Unity 2D (URP) organizado em pastas semânticas dentro de `Assets/`. NUNCA deve usar ícones unicode ("💻 Code", "🎮 Game", etc.). Referencie pelo nome exato ao citar caminhos.
- Código de jogo principal vive em `Assets/Code/Systems` (infra, managers) e `Assets/External/` (código de terceiros / importado). Evite editar código externo sem necessidade explícita.
- Sempre siga os princípios KISS (Keep It Simple, Stupid) e YAGNI (You Aren't Gonna Need It) ao sugerir mudanças ou adicionar funcionalidades.
- Padrão central: Managers derivados de `ManagerSingleton<T>` (`GameManager`, `SceneTransitionManager`) para serviços globais persistentes entre cenas. NÃO use Singleton para Player (ver `PlayerController`).
- `GameManager`: controla preload + ativação de cenas Additive e faz limpeza de artefatos (EventSystem, Light2D).
- `SceneTransitionManager`: oferece transições visuais e ordena canvas de transição (`sortingOrder = 9999`).

## Convenções e Padrões
- Namespaces: managers em `SlimeKing.Core`; gameplay em `SlimeKing.Gameplay`; visual em `SlimeKing.Visual`; items em `SlimeKing.Items`. Mantenha namespace consistente ao criar novos managers.
- Script lifecycle: Inicialização específica em `Initialize()` dos managers; use `protected override void Initialize()` ao adicionar novo manager.
- Não adicione Logs a menos que seja explicitamente solicitado.
- Evite referências diretas estáticas fora do padrão Singleton; prefira `GameManager.Instance` somente após checar `GameManager.HasInstance` se houver chance de ausência.
- Player: possui próprio `Instance` + `DontDestroyOnLoad` mas NÃO herda de `ManagerSingleton`; não introduzir Manager dependências circulares (Player não deve inicializar Managers).

## Cena & Fluxo de Carregamento
- Pré-carregamento: chamar `GameManager.Instance.PreloadScene(name)` seguido de `ActivatePreloadedScene(() => {/* callback */})`. Checar `IsPreloadReady` antes de ativar para evitar espera.
- Transições: usar `SceneTransitionManager.Instance.LoadSceneWithTransition(sceneName)` para efeito visual. Não misture com preload sem necessidade: se combinar, ative preload e então transição com fade-in adaptado.
- Limpeza automática pós ativação: não recrie manualmente `EventSystem` ou `Global Light2D` duplicadas; o manager já faz varredura e destruição segura.

## Input System
- `PlayerController` instancia `InputSystem_Actions` em `InitializeInputSystem()` e registra handlers no `OnEnable`. Ao adicionar nova ação: gerar novo mapa de input (Unity) e criar método handler `private void On<Nome>Input(InputAction.CallbackContext ctx)` seguindo exemplo, subscrever em `SubscribeToInputEvents()` e retirar em `UnsubscribeFromInputEvents()`.

## Animações & Visual
- Usa `Animator.StringToHash` para caching de parâmetros (ex: `IsWalking`, `Attack01`). Novos parâmetros devem seguir mesma estratégia: definir `private static readonly int Param = Animator.StringToHash("ParamName");`.
- Direção visual controlada por enum interna `VisualDirection` com três estados (South/North/Side). Alterações visuais devem atualizar `_currentVisualDirection` e flip de sprite (`_facingRight`).

## Corrotinas & Timing
- Movimentos e transições usam corrotinas (`StartCoroutine`). Respeitar padrão de métodos privados `IEnumerator Nome()` e limpeza de referência (`pendingActivationCoroutine = null`) após conclusão.
- Para esperar carregamento de cena: laço `while (op.progress < 0.9f) yield return null;` depois `allowSceneActivation = true` e aguardar `isDone`.

## Extensões / Novos Managers
Ao criar um novo Manager global:
```csharp
namespace SlimeKing.Core {
  public class AudioManager : ManagerSingleton<AudioManager> {
    protected override void Initialize() { /* setup */ }
  }
}
```
- Defina flags internas para controle de logs.
- Use `DontDestroyOnLoad` via base se precisar persistir (default `persistBetweenScenes = true`).

## Boas Práticas Específicas
- Não inserir lógica pesada em `Awake()` de Managers; colocar em `Initialize()` para consistência.
- Antes de chamar eventos públicos (`OnPreloadedSceneActivated`), validar se não é `null` (já segue padrão com operador de coalescência segura `?.Invoke`).
- Ao manipular UI de transição, mantenha `sortingOrder` alto e use `SetActive` para visibilidade em vez de destruir/instanciar repetidamente.

## Evitar
- Criar segundos `EventSystem` em cenas additivas (já há cleanup).
- Usar `DestroyImmediate` em runtime (Managers usam `Destroy`).
- Introduzir dependências ao `PlayerController` dentro de novos Managers (manter Player independente).
- Fazer over-engineering de soluções simples; prefira clareza e manutenção futura.
- **NUNCA criar menus fora de "Extra Tools/"** - todos os `[MenuItem]` devem seguir estrutura unificada (ver seção Editor Tools).

## Editor Tools & Menus
- **POLÍTICA OBRIGATÓRIA**: TODOS os menus de editor devem estar sob `"Extra Tools/"`.
- **Estrutura de menus**:
  - `"Extra Tools/Tests/"` - Para todos os testes e validações
  - `"Extra Tools/Setup/"` - Para ferramentas de configuração e integração
  - `"Extra Tools/NPC/"`, `"Extra Tools/Camera/"`, etc. - Para categorias específicas
  - `"Assets/Create/Extra Tools/"` - Para criação de assets customizados
- **NUNCA usar**: `"SlimeKing/"`, `"The Slime King/"`, `"ProjectName/"` ou qualquer outro menu raiz.
- Namespace padrão para editor tools: `ExtraTools.Editor`.
- Ver `Assets/Code/Editor/ExtraTools/README.md` para documentação completa.

---
Feedback: Informe se falta alguma convenção de build, testes ou pipelines para incluirmos. Quais fluxos internos (ex: audio, inventário) você quer documentar na próxima versão?
