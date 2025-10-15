# Design - Sistema de Transição de Cenários

Este documento descreve o design técnico do Sistema de Transição de Cenários para o projeto SlimeKing, adaptado às convenções locais (Managers em `SlimeKing.Core`, uso de `GameManager` e `SceneTransitionManager`, princípio KISS) e alinhado com a documentação prévia de alto nível.

---

## 1. Objetivos

- Fornecer transições suaves entre mapas/cenas usando efeito visual consistente.
- Centralizar a orquestração da troca de cena mantendo o estado global limpo.
- Permitir reutilização de lógica para: troca entre cenas diferentes (additive ou single) e reposicionamento dentro da mesma cena.
- Garantir ausência de duplicações indesejadas (EventSystem, luzes globais) através da limpeza já existente no `GameManager`.
- Manter Player independente (não inicializa Managers nem contém lógica de transição).

### Fora de Escopo

- Sistema de save/load.
- Gestão de inventário / UI complexa durante a transição.
- Efeitos custom avançados além dos fornecidos pelo asset de transição.

---

## 2. Requisitos Funcionais

1. Detectar entrada do Player em um trigger de transição.
2. Disparar uma sequência de transição visual (fade in -> troca -> fade out ou variante).
3. Carregar cena alvo (async) quando requerido.
4. Reposicionar Player em coordenadas destino (mesma cena ou nova cena).
5. Recriar/reativar câmera seguindo novo Player.
6. Prover API simples para solicitar transição por script (sem trigger físico).
7. Bloquear input do Player durante transição.
8. Garantir idempotência: evitar chamadas concorrentes enquanto uma transição está em andamento.

### Requisitos Não Funcionais

- KISS: mínimo de classes novas (apenas `TransitionTrigger` + ajustes em `GameManager` / uso de já existente `SceneTransitionManager`).
- Baixo acoplamento: Trigger não depende internamente de `SceneTransitionManager`, apenas chama `GameManager`.
- Extensível: fácil adicionar variantes de efeito ou lógica (ex: pré-carregamento futuro).
- Logs controláveis via flags dos Managers.

---

## 3. Componentes

### 3.1 TransitionTrigger (MonoBehaviour)

Responsável por expor dados de destino e notificar `GameManager`.

Campos:

- `public string targetSceneName;` (vazio => mesma cena)
- `public Vector2 targetPosition;` (posição 2D lógica)
- `public bool isSceneChange;`
- (Opcional futuro) `public string transitionProfileId;` (identifica preset de efeito)

Eventos do ciclo:

- `OnTriggerEnter2D(Collider2D other)` verifica se é Player e chama `RequestTransition()`.

Método:

- `private void RequestTransition()` => chama `GameManager.Instance.RequestSceneTransition(new SceneTransitionRequest(...))`.

### 3.2 Estrutura de Dados: SceneTransitionRequest

```csharp
public struct SceneTransitionRequest {
  public readonly string TargetScene;
  public readonly Vector2 TargetPosition; // 2D apenas
  public readonly bool IsSceneChange;
  public readonly string EffectProfileId; // pode ser null/empty
  public SceneTransitionRequest(string targetScene, Vector2 targetPosition, bool isSceneChange, string effectProfileId = null) {
    TargetScene = targetScene;
    TargetPosition = targetPosition;
    IsSceneChange = isSceneChange;
    EffectProfileId = effectProfileId;
  }
}
```

### 3.3 GameManager (SlimeKing.Core)

Extensões necessárias:

Campos privados sugeridos:

- `private bool isTransitionInProgress;`
- `private Coroutine activeTransitionCoroutine;`
- Referência ao prefab de Player e câmera (se não existir já):
  - `public GameObject playerPrefab;`
  - `public GameObject cameraPrefab;`

API Pública nova:

- `public bool IsTransitionInProgress => isTransitionInProgress;`
- `public void RequestSceneTransition(SceneTransitionRequest request)`

Fluxo interno:

1. Guard clause: se `isTransitionInProgress` true => ignorar/log.
2. Seta flag + inicia coroutine: `activeTransitionCoroutine = StartCoroutine(HandleSceneTransition(request));`
3. Coroutine:
   - Chamar efeito inicial via `SceneTransitionManager.Instance.BeginTransition(effectProfileId)`.
   - Bloquear input (ex: `PlayerController.Instance.SetInputEnabled(false)` se existir API; caso contrário introduzir).
   - Se `request.IsSceneChange`:
     - Carregar cena async additive ou single (decidir: usar `LoadSceneAsync(request.TargetScene, LoadSceneMode.Single)` para simplicidade KISS).
     - Aguardar `progress >= 0.9f`, depois permitir ativação.
     - Limpeza já realizada por `GameManager` padrão após ativação (reusar lógica existente se houver). Caso não, invocar método interno `CleanupSceneArtifacts()`.
   - Reinstanciar Player:
     - Se já existe `PlayerController.Instance` destruir `PlayerController.Instance.gameObject`.
     - Instanciar `playerPrefab` e posicionar em `new Vector3(request.TargetPosition.x, request.TargetPosition.y, 0f)` (Z fixo = 0).
   - Reinstanciar câmera:
     - Destruir câmera anterior (buscar tag ou referência persistente).
     - Instanciar `cameraPrefab`.
     - Configurar follow (ex: `virtualCam.Follow = newPlayer.transform`).
   - Liberar input.
   - Finalizar efeito: `SceneTransitionManager.Instance.EndTransition()`.
   - Reset flag `isTransitionInProgress = false; activeTransitionCoroutine = null;`

### 3.4 SceneTransitionManager

Já existente. Necessário expor dois métodos simples (se não existirem):

- `public void BeginTransition(string profileId = null)` => aplica fade-out.
- `public void EndTransition()` => faz fade-in.

Implementar internamente mapeamento `profileId -> TransitionPreset` (asset do Easy Transition) futuramente; inicialmente pode ignorar `profileId`.

### 3.5 PlayerController

Necessário ter método para habilitar/desabilitar input.

- `public void SetInputEnabled(bool enabled)` que liga/desliga ações do `InputSystem_Actions`.

---

## 4. Contratos (Resumo de Assinaturas)

```csharp
// TransitionTrigger
public class TransitionTrigger : MonoBehaviour {
  public string targetSceneName;
  public Vector2 targetPosition;
  public bool isSceneChange;
  private void OnTriggerEnter2D(Collider2D other);
}

// Estrutura Request
public struct SceneTransitionRequest {
  public readonly string TargetScene;
  public readonly Vector2 TargetPosition;
  public readonly bool IsSceneChange;
  public readonly string EffectProfileId;
}

// GameManager (adições)
public bool IsTransitionInProgress { get; }
public void RequestSceneTransition(SceneTransitionRequest request);
private IEnumerator HandleSceneTransition(SceneTransitionRequest request);

// SceneTransitionManager (adições se faltarem)
public void BeginTransition(string profileId = null);
public void EndTransition();

// PlayerController (adição)
public void SetInputEnabled(bool enabled);
```

---

## 5. Fluxo de Sequência (Texto)

1. Player entra em `TransitionTrigger`.
2. Trigger cria `SceneTransitionRequest` e chama `GameManager.RequestSceneTransition(...)`.
3. GameManager valida estado e inicia coroutine.
4. Efeito visual Begin (fade-out).
5. Input desativado.
6. (Opcional) Carrega cena destino.
7. Destroi Player e câmera atuais.
8. Instancia novos Player e câmera na posição destino (conversão para Vector3 com Z=0).
9. Reconfigura follow da câmera.
10. Efeito visual End (fade-in).
11. Input reativado.
12. Flag de transição limpa.

---

## 6. Estados & Controle de Concorrência

Estados principais:

- Idle (sem transição) => aceita nova requisição.
- Transitioning => rejeita novas requisições (log informativo).

Edge Cases:

- Trigger múltiplo acionado quase simultaneamente: primeira requisição vence; demais ignoradas.
- Cena alvo inválida / não encontrada: log de erro, aborta transição e tenta restaurar input.
- Prefab faltando: aborta e loga erro.
- Player destruído antes da reinstanciação (ex: outro sistema) -> coroutine detecta ausência e continua instanciando.

---

## 7. Erros & Logs

Usar métodos de log do `GameManager` e `SceneTransitionManager`. Mensagens chave:

- "[Transition] Request received to scene X (changeScene=Y)".
- "[Transition] Already in progress, ignoring.".
- "[Transition] Scene load started.".
- "[Transition] Scene load completed.".
- "[Transition] Instantiating player & camera.".
- "[Transition] Completed.".
- Erros: cena inválida, prefabs nulos, falha efeito visual.

---

## 8. Implementação Incremental (Roadmap)

1. Adicionar struct `SceneTransitionRequest`.
2. Adicionar campos e métodos novos ao `GameManager` (flag + Request + coroutine stub).
3. Implementar `BeginTransition/EndTransition` simples no `SceneTransitionManager` (se ausentes; usar fade preto padrão).
4. Criar `TransitionTrigger` mínimo (sem profileId inicial).
5. Adicionar `SetInputEnabled` ao `PlayerController`.
6. Teste funcional básico: transição dentro da mesma cena (apenas reposicionamento).
7. Teste transição com troca de cena simples (LoadSceneMode.Single).
8. Adicionar logs e validar concorrência.
9. Opcional: adicionar suporte a `EffectProfileId` e mapeamento de presets.
10. Refinar limpeza pós-carregamento reutilizando já existente lógica de `GameManager`.

---

## 9. Extensões Futuras

- Pré-carregamento (usar já existente `PreloadScene` + ajustar fluxo para ativar após fade-out).
- Transições condicionais (ex: requer item / estado de missão).
- Múltiplos pontos de spawn por ID.
- Persistência de atributos do Player entre reinstanciações (inventário, status).
- Cache de Cinemachine Brain para evitar reinstanciação completa.

---

## 10. Princípio KISS Aplicado

- Uma única coroutine central controla toda a sequência.
- Trigger apenas encaminha dados; sem lógica adicional.
- Sem filas complexas; requisções concorrentes são ignoradas.
- Uso de tipos simples (`struct` imutável) para request.
- Posições armazenadas como `Vector2`; conversão para `Vector3` feita somente ao aplicar em `Transform.position` com Z fixo = 0. (Boa prática: manter Z consistente para evitar bugs de sorting e facilitar futura migração para parallax se necessário.)

---

## 11. Checklist de Aceitação

- Transição não trava input após completar.
- Nenhuma exceção durante troca de cena normal.
- Player aparece na posição destino correta.
- Câmera segue Player após transição.
- Logs aparecem quando debug habilitado.

---

## 12. Segurança & Performance

- Evitar leaks: destruir corretamente instâncias antigas.
- Carregamento Single substitui cena anterior para simplicidade (reduz memória).
- Pode evoluir para Additive se o design exigir (preload).

---

Fim do documento.
