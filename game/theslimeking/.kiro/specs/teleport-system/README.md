# Sistema de Teletransporte - Guia de Uso

## Visão Geral

O Sistema de Teletransporte permite que o jogador se mova instantaneamente entre diferentes pontos do mapa com transições visuais suaves. O sistema utiliza o asset **Easy Transition** para criar uma experiência fluida e imersiva.

## Características Principais

- ✅ Teletransporte instantâneo com transição visual suave
- ✅ Detecção automática de colisão com o Player
- ✅ Reposicionamento automático da câmera
- ✅ Controle de movimento bloqueado durante transição
- ✅ Configuração fácil via Inspector
- ✅ Visualização com Gizmos no Editor
- ✅ Sistema de debug integrado
- ✅ Validações robustas com mensagens de erro claras

## Requisitos

### Dependências

1. **Unity 6.2+**
2. **Easy Transition** (já incluído no projeto)
3. **PlayerController** com métodos `DisableMovement()` e `EnableMovement()`
4. **SceneTransitioner** presente na cena
5. Tag "Player" configurada no GameObject do jogador

### Estrutura de Arquivos

```
Assets/
├── Code/
│   └── Gameplay/
│       ├── TeleportPoint.cs
│       └── TeleportTransitionHelper.cs
├── External/
│   └── AssetStore/
│       └── Easy Transition/
│           └── Transition Effects/
│               └── CircleEffect.asset
```

## Instalação e Configuração

### Passo 1: Adicionar SceneTransitioner à Cena

1. Localize o prefab **SceneTransitioner** em `Assets/External/AssetStore/Easy Transition/Prefabs/`
2. Arraste o prefab para a hierarquia da cena
3. Certifique-se de que há apenas uma instância por cena

### Passo 2: Criar um TeleportPoint

#### Opção A: Usar o Prefab (Recomendado)

1. Localize o prefab **TeleportPoint** em `Assets/Prefabs/Gameplay/`
2. Arraste o prefab para a cena
3. Posicione onde deseja que o ponto de teletransporte fique

#### Opção B: Criar Manualmente

1. Crie um GameObject vazio: `GameObject → Create Empty`
2. Renomeie para "TeleportPoint"
3. Adicione o componente **BoxCollider2D**:
   - Marque "Is Trigger" ✓
4. Adicione o script **TeleportPoint**
5. Configure os parâmetros no Inspector

### Passo 3: Configurar o TeleportPoint

No Inspector, configure os seguintes campos:

#### Teleport Configuration

- **Destination Position**: Posição (X, Y, Z) para onde o Player será teletransportado
  - Exemplo: `(10, 5, 0)` para teletransportar para X=10, Y=5
  
- **Transition Effect**: Arraste o **CircleEffect** de `Assets/External/AssetStore/Easy Transition/Transition Effects/CircleEffect.asset`
  
- **Delay Before Fade In**: Tempo de espera após reposicionamento (padrão: 1 segundo)
  - Valores menores = transição mais rápida
  - Valores maiores = mais tempo para processar reposicionamento

#### Trigger Configuration

- **Trigger Size**: Tamanho da área de detecção (padrão: 1x1)
  - Aumente para áreas maiores de ativação
  - Diminua para precisão maior
  
- **Trigger Offset**: Deslocamento do trigger em relação ao GameObject
  - Use para ajustar a posição da área de detecção

#### Debug

- **Enable Debug Logs**: Ativa logs detalhados no Console
  - Útil para troubleshooting
  - Desative em produção para melhor performance
  
- **Enable Gizmos**: Mostra visualização no Editor
  - Área do trigger (wireframe)
  - Linha conectando origem ao destino
  - Marcador no ponto de destino
  
- **Gizmo Color**: Cor da visualização (padrão: Cyan)

## Exemplos de Uso

### Exemplo 1: Teletransporte Simples

Criar um ponto de teletransporte que move o Player de uma sala para outra:

```
1. Crie um TeleportPoint na posição (0, 0, 0)
2. Configure Destination Position para (20, 0, 0)
3. Atribua CircleEffect ao campo Transition Effect
4. Teste entrando na área do trigger
```

### Exemplo 2: Teletransporte Bidirecional

Criar dois pontos que permitem ir e voltar:

```
TeleportPoint A:
- Position: (0, 0, 0)
- Destination: (20, 0, 0)

TeleportPoint B:
- Position: (20, 0, 0)
- Destination: (0, 0, 0)
```

### Exemplo 3: Múltiplos Destinos

Criar um hub central com vários destinos:

```
Hub Central (0, 0, 0):

TeleportPoint 1 → Área Norte (0, 20, 0)
TeleportPoint 2 → Área Sul (0, -20, 0)
TeleportPoint 3 → Área Leste (20, 0, 0)
TeleportPoint 4 → Área Oeste (-20, 0, 0)

Cada área tem um TeleportPoint de retorno ao hub.
```

### Exemplo 4: Trigger Customizado

Ajustar o tamanho e posição do trigger:

```
Trigger Size: (2, 3) - Área retangular maior
Trigger Offset: (0, 0.5) - Deslocado meio metro para cima
```

Útil para:

- Portas largas
- Plataformas elevadas
- Áreas de ativação específicas

## Configurações Avançadas

### Ajuste de Performance

Para melhor performance em cenas com muitos TeleportPoints:

1. **Desabilite Debug Logs** em produção
2. **Desabilite Gizmos** quando não estiver editando
3. **Use Trigger Size mínimo** necessário
4. **Reutilize o mesmo TransitionEffect** entre múltiplos pontos

### Customização de Transição

Você pode usar diferentes efeitos do Easy Transition:

- **CircleEffect**: Círculo que fecha/abre (recomendado)
- **FadeEffect**: Fade simples para preto
- **WipeEffect**: Transição de varredura
- Outros efeitos disponíveis no Easy Transition

Para trocar o efeito:

1. Localize o efeito desejado em `Assets/External/AssetStore/Easy Transition/Transition Effects/`
2. Arraste para o campo **Transition Effect** do TeleportPoint

### Delay Customizado

Ajuste o **Delay Before Fade In** para diferentes experiências:

- **0 segundos**: Transição instantânea (pode ser abrupto)
- **0.5 segundos**: Transição rápida
- **1 segundo**: Transição padrão (recomendado)
- **2+ segundos**: Transição lenta (para efeitos dramáticos)

## Troubleshooting

### Problema: Teletransporte não funciona

**Possíveis causas e soluções:**

1. **Destination Position não configurado**
   - Verifique se o campo não está em (0, 0, 0)
   - Configure uma posição válida no Inspector

2. **Transition Effect não atribuído**
   - Arraste o CircleEffect para o campo no Inspector
   - Verifique se o asset existe no projeto

3. **SceneTransitioner não está na cena**
   - Adicione o prefab SceneTransitioner à cena
   - Certifique-se de que há apenas uma instância

4. **Player não tem a tag "Player"**
   - Selecione o GameObject do Player
   - Configure a Tag para "Player" no Inspector

5. **BoxCollider2D não está configurado como Trigger**
   - Selecione o TeleportPoint
   - Marque "Is Trigger" no BoxCollider2D

### Problema: Player fica preso sem controle

**Solução:**

- Isso pode acontecer se o teletransporte for interrompido
- Verifique os logs do Console para erros
- Certifique-se de que `PlayerController.Instance.EnableMovement()` é chamado

### Problema: Câmera não segue o Player

**Solução:**

- Verifique se existe uma câmera com tag "MainCamera"
- O sistema busca automaticamente `Camera.main`
- Se usar sistema de câmera customizado, pode ser necessário adaptação

### Problema: Múltiplos teletransportes simultâneos

**Solução:**

- O sistema já previne isso com a flag `isTeleporting`
- Se ainda ocorrer, verifique se há múltiplos TeleportPoints sobrepostos
- Ajuste o Trigger Size para evitar sobreposição

### Problema: Transição visual não aparece

**Solução:**

1. Verifique se o CircleEffect está atribuído
2. Verifique se o SceneTransitioner está ativo na cena
3. Verifique se há erros no Console
4. Ative Debug Logs para mais informações

## Limitações Conhecidas

### 1. Teletransporte Apenas na Mesma Cena

**Limitação:** O sistema atual funciona apenas dentro da mesma cena.

**Workaround:** Para teletransporte entre cenas, use o método padrão do Easy Transition:

```csharp
SceneTransitioner.Instance.LoadScene("NomeDaCena", circleEffect);
```

**Expansão Futura:** Pode ser implementado suporte para mudança de cena mantendo a API atual.

### 2. Um Destino Por TeleportPoint

**Limitação:** Cada TeleportPoint pode ter apenas um destino configurado.

**Workaround:** Para múltiplos destinos, crie múltiplos TeleportPoints na mesma posição ou próximos.

**Exemplo:**

```
Posição (0, 0):
- TeleportPoint_Norte → (0, 20)
- TeleportPoint_Sul → (0, -20)
```

### 3. Sem Suporte para Condições de Ativação

**Limitação:** O teletransporte é ativado automaticamente ao colidir.

**Workaround:** Para adicionar condições (ex: ter uma chave), você precisará:

1. Herdar de TeleportPoint
2. Override do método OnTriggerEnter2D
3. Adicionar suas validações customizadas

**Exemplo:**

```csharp
public class ConditionalTeleportPoint : TeleportPoint
{
    [SerializeField] private bool requiresKey = false;
    
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (requiresKey && !PlayerInventory.HasKey())
        {
            Debug.Log("Você precisa de uma chave!");
            return;
        }
        
        base.OnTriggerEnter2D(other);
    }
}
```

### 4. Sem Interação Manual (Pressionar Botão)

**Limitação:** O teletransporte é automático ao entrar no trigger.

**Workaround:** Para requerer input do jogador:

1. Desabilite o teletransporte automático
2. Mostre um prompt de interação
3. Ative o teletransporte quando o jogador pressionar o botão

**Expansão Futura:** Pode ser adicionado um campo `requiresInteraction` e `interactionKey`.

### 5. Sem Cooldown Entre Teletransportes

**Limitação:** Após completar um teletransporte, o Player pode imediatamente usar outro.

**Workaround:** Adicione um sistema de cooldown global se necessário.

**Expansão Futura:** Pode ser adicionado um campo `cooldownTime` por TeleportPoint.

### 6. Sem Efeitos Sonoros Integrados

**Limitação:** O sistema não toca sons automaticamente.

**Workaround:** Adicione um AudioSource ao TeleportPoint e toque manualmente:

```csharp
// No método ExecuteTeleport, antes da transição:
AudioSource audioSource = GetComponent<AudioSource>();
if (audioSource != null)
{
    audioSource.Play();
}
```

**Expansão Futura:** Pode ser adicionado um campo `AudioClip teleportSound`.

### 7. Sem Direção de Saída

**Limitação:** O Player pode imediatamente reteletransportar se o destino estiver dentro de outro trigger.

**Workaround:**

- Posicione os destinos fora das áreas de trigger
- Use Trigger Offset para ajustar a área de detecção
- Adicione um pequeno delay antes de reativar o trigger

**Expansão Futura:** Pode ser adicionado um campo `exitDirection` que empurra o Player para fora do trigger.

### 8. Performance com Muitos TeleportPoints

**Limitação:** Gizmos podem impactar performance do Editor com 50+ TeleportPoints.

**Workaround:**

- Desabilite Gizmos quando não estiver editando
- Use layers para organizar TeleportPoints
- Considere usar um sistema de LOD para Gizmos

### 9. Sem Suporte para Multiplayer

**Limitação:** O sistema foi projetado para single-player.

**Workaround:** Para multiplayer, seria necessário:

- Sincronização de rede
- Validação server-side
- Replicação de posição para todos os clientes

### 10. Dependência do Easy Transition

**Limitação:** O sistema requer o Easy Transition para funcionar.

**Workaround:** Se quiser remover a dependência:

- Implemente seu próprio sistema de transição
- Substitua TeleportTransitionHelper por sua implementação
- Mantenha a mesma interface pública

## Boas Práticas

### Level Design

1. **Posicione destinos fora de triggers**
   - Evite loops infinitos de teletransporte
   - Deixe espaço para o Player se orientar

2. **Use Gizmos para visualização**
   - Facilita o planejamento do level
   - Evita erros de posicionamento

3. **Teste ambas as direções**
   - Se criar teletransporte bidirecional, teste ida e volta
   - Verifique se as posições fazem sentido

4. **Considere o contexto visual**
   - Coloque TeleportPoints em locais que façam sentido (portas, portais, etc.)
   - Use efeitos visuais adicionais (partículas, luz) para indicar pontos de teletransporte

### Performance

1. **Reutilize TransitionEffects**
   - Use a mesma instância de CircleEffect para todos os pontos
   - Evita duplicação de assets

2. **Desabilite Debug em produção**
   - Logs impactam performance
   - Mantenha apenas durante desenvolvimento

3. **Otimize Trigger Size**
   - Use o menor tamanho necessário
   - Evita detecções desnecessárias

### Organização

1. **Use nomes descritivos**
   - "TeleportPoint_CaveToForest"
   - "TeleportPoint_HubToLevel1"

2. **Agrupe em hierarquia**

   ```
   Teleports/
   ├── Hub/
   │   ├── TeleportPoint_ToLevel1
   │   ├── TeleportPoint_ToLevel2
   │   └── TeleportPoint_ToLevel3
   └── Levels/
       ├── Level1_ReturnToHub
       └── Level2_ReturnToHub
   ```

3. **Use cores diferentes para Gizmos**
   - Azul para teletransportes principais
   - Verde para retornos
   - Amarelo para teletransportes secretos

## Integração com Outros Sistemas

### Sistema de Eventos

Se seu projeto tem um sistema de eventos, você pode estender TeleportPoint:

```csharp
public class EventTeleportPoint : TeleportPoint
{
    [SerializeField] private GameEvent onTeleportStarted;
    [SerializeField] private GameEvent onTeleportCompleted;
    
    protected override IEnumerator ExecuteTeleport()
    {
        onTeleportStarted?.Raise();
        yield return base.ExecuteTeleport();
        onTeleportCompleted?.Raise();
    }
}
```

### Sistema de Áudio

Adicione sons ao teletransporte:

```csharp
[SerializeField] private AudioClip teleportSound;

private void PlayTeleportSound()
{
    if (teleportSound != null)
    {
        AudioSource.PlayClipAtPoint(teleportSound, transform.position);
    }
}
```

### Sistema de Partículas

Adicione efeitos visuais:

```csharp
[SerializeField] private ParticleSystem teleportEffect;

private void PlayTeleportEffect()
{
    if (teleportEffect != null)
    {
        teleportEffect.Play();
    }
}
```

## Referência de API

### TeleportPoint

**Campos Públicos (Inspector):**

- `Vector3 destinationPosition` - Posição de destino
- `TransitionEffect transitionEffect` - Efeito de transição
- `float delayBeforeFadeIn` - Delay antes do fade in
- `Vector2 triggerSize` - Tamanho do trigger
- `Vector2 triggerOffset` - Offset do trigger
- `bool enableDebugLogs` - Ativa logs de debug
- `bool enableGizmos` - Ativa visualização de Gizmos
- `Color gizmoColor` - Cor dos Gizmos

**Métodos Principais:**

- `void Awake()` - Inicialização
- `void OnValidate()` - Atualização em tempo real no Editor
- `void OnTriggerEnter2D(Collider2D)` - Detecção de colisão
- `IEnumerator ExecuteTeleport()` - Execução do teletransporte
- `void RepositionPlayerAndCamera()` - Reposicionamento
- `bool ValidateTeleport()` - Validações
- `void OnDrawGizmos()` - Visualização no Editor

### TeleportTransitionHelper

**Métodos Estáticos:**

- `IEnumerator ExecuteTransition(TransitionEffect, Action, float, bool)` - Executa transição completa

**Parâmetros:**

- `effect` - Efeito de transição a usar
- `onMidTransition` - Callback executado após fade out
- `delayBeforeFadeIn` - Tempo de espera antes do fade in
- `enableDebugLogs` - Ativa logs de debug

## Suporte e Contribuição

### Reportar Bugs

Se encontrar bugs, inclua as seguintes informações:

1. Versão do Unity
2. Passos para reproduzir
3. Logs do Console (com Debug Logs ativado)
4. Screenshots ou vídeo do problema
5. Configuração do TeleportPoint

### Solicitar Features

Para solicitar novas funcionalidades:

1. Descreva o caso de uso
2. Explique por que a funcionalidade é necessária
3. Sugira uma possível implementação
4. Considere se pode ser feito via herança/extensão

## Changelog

### Versão 1.0.0 (Atual)

**Funcionalidades:**

- ✅ Teletransporte básico com transição visual
- ✅ Detecção automática de colisão
- ✅ Reposicionamento de Player e câmera
- ✅ Controle de movimento durante transição
- ✅ Configuração via Inspector
- ✅ Visualização com Gizmos
- ✅ Sistema de debug
- ✅ Validações robustas

**Limitações:**

- ⚠️ Apenas mesma cena
- ⚠️ Um destino por ponto
- ⚠️ Sem condições de ativação
- ⚠️ Sem interação manual
- ⚠️ Sem cooldown
- ⚠️ Sem efeitos sonoros integrados

### Roadmap Futuro

**Versão 1.1.0 (Planejado):**

- Suporte para teletransporte entre cenas
- Campo `requiresInteraction` para ativação manual
- Campo `interactionKey` configurável
- Campo `AudioClip teleportSound`
- Sistema de cooldown opcional

**Versão 1.2.0 (Planejado):**

- Múltiplos destinos por ponto
- Condições de ativação customizáveis
- Direção de saída configurável
- Integração com sistema de eventos
- Efeitos de partículas integrados

**Versão 2.0.0 (Futuro):**

- Suporte para multiplayer
- Sistema de transição customizável
- Remoção de dependência do Easy Transition (opcional)
- Editor customizado no Inspector
- Ferramentas de level design avançadas

## Licença e Créditos

**Sistema de Teletransporte:**

- Desenvolvido para The Slime King
- Código em inglês, comentários em português
- Segue BoasPraticas.md do projeto

**Dependências:**

- Easy Transition (Asset Store)
- Unity Engine

## Conclusão

O Sistema de Teletransporte fornece uma solução simples e eficaz para movimentação rápida do jogador. Seguindo este guia, você poderá criar experiências de teletransporte fluidas e imersivas em seu jogo.

Para dúvidas ou suporte adicional, consulte a documentação do código (comentários XML) ou os arquivos de design e requisitos na pasta `.kiro/specs/teleport-system/`.

**Bom desenvolvimento! 🎮**
