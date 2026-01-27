<img src="https://r2cdn.perplexity.ai/pplx-full-logo-primary-dark%402x.png" style="height:64px;margin-right:32px"/>

# Estou com problemas de performance no meu jogo The Slime King.

Me ajude a criar um guia de boas práticas e melhorias de performance para jogos 2D top down pixel art na Unity. Lembre-se de que em certas scenes eu tenho um número acima de 500 arbustos e árvores animados.

Perfeito! Agora tenho todas as informações necessárias para gerar um guia completo e profissional. Vou criar o relatório final.

***

# Guia Completo de Otimização de Performance para Jogos 2D Top-Down Pixel Art em Unity

## Executive Summary

A otimização de performance em jogos 2D com grandes volumes de sprites animados é um desafio arquitetural específico. Com 500+ árvores e arbustos animados na mesma cena, o gargalo principal não é renderização — é o processamento de animações no CPU via Animator Component. Este guia fornece uma estratégia em camadas que progride de técnicas fundamentais para soluções avançadas, priorizando aquelas que entregam máximo impacto em tempo mínimo de implementação.

***

## 1. Diagnóstico: Encontrando Seu Gargalo Real

Antes de otimizar, você precisa medir. O erro mais comum é otimizar cegamente.

**Passo 1: Habilitar o Profiler**

Abra **Window → Analysis → Profiler** e foque em:

- **CPU Usage**: Qual função consome mais tempo? Se for `Animator.Update()`, você tem o problema que esperamos.
- **Memory**: Quantos GameObjects estão na cena? Quantas texturas carregadas?
- **GPU**: Quantas draw calls? Se > 1000, batching é crítico.

**Passo 2: Definir Seu Target**

- Desktop: 60 FPS = 16.67ms budget por frame
- Mobile: 30 FPS = 33.33ms budget (mais realista para 500+ sprites)

Se você está vendo **Animator.Update() > 5ms**, sua estratégia é evitar Animators completamente ou reduzir drasticamente quantos estão ativos.

***

## 2. Camada 1: Fundamentals (Ganhos Imediatos, Baixo Risco)

### 2.1 Sprite Atlasing[^1][^2]

**O Problema**: Cada sprite individual = draw call. 500 arbustos com sprites separados = potencialmente 500+ draw calls.

**A Solução**: Empacotar sprites em atlases.


| Métrica | Sem Atlas | Com Atlas |
| :-- | :-- | :-- |
| Draw Calls | ~500 | ~5-10 |
| Memória | Fragmentada | Consolidada |
| Tempo de Carregamento | Variável | Previsível |

**Como Implementar**:

1. Crie um `Sprite Atlas` para cada tipo de árvore/arbusto:
    - `TreeAtlas.spriteatlas` (pinheiros, carvalhos, etc.)
    - `BushAtlas.spriteatlas` (arbustos pequenos)
2. Em Project Settings → Player → Sprite Atlas, certifique-se de que "Include in Build" está ativado.
3. Para cada Atlas, ajuste o tamanho máximo de textura:
    - Clique no Atlas → Inspector
    - **Platform-specific overrides** → ajuste **Max Texture Size** para o menor que acomode seus sprites
    - Isso reduz footprint de memória

**Boas Práticas**:

- Não crie um atlas gigante com tudo; separe por padrão de uso (cenas distantes vs cenas próximas)[^2]
- Verifique o **Pack Preview** para evitar espaços vazios desnecessários


### 2.2 Caching de Componentes[^3][^4][^5]

**O Problema**: `GetComponent<>()` é caro. Se você chama em Update(), multiplicado por 500 sprites, é perda significativa.

**A Solução**: Cache em Start() ou Awake().

```csharp
public class TreeAnimator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    
    void Awake()
    {
        // Cache UMA VEZ
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        // Reutilize referência, não chame GetComponent
        if (_animator != null)
            _animator.SetBool("isSwaying", true);
    }
}
```

**Ganho**: ~20% mais rápido que GetComponent + null check[^3].

### 2.3 Camera Culling Mask \& Layer Setup[^6][^7][^8]

Garanta que sua câmera renderiza apenas o necessário.

```csharp
// No Inspector da Camera:
// Culling Mask → uncheck layers desnecessários

// Ou programaticamente:
camera.cullingMask = LayerMask.GetMask("Default") | LayerMask.GetMask("Trees");
```

Se você tem UI, efeitos, ou outros elementos desnecessários na cena principal, tire-os.

***

## 3. Camada 2: Batching \& Draw Call Optimization

### 3.1 Static vs Dynamic Batching[^9][^10][^11]

Para suas árvores estáticas (que não se movem):

**Static Batching** (para árvores imóveis):

1. Selecione cada árvore/arbusto GameObject
2. Inspector → ativar **Batching Static**
3. Unity combina automaticamente em build time

**Limitações**:

- Máximo 64000 vértices por batch
- Não funciona se você muda posição/rotação/escala em runtime
- Aumenta memória (copia para world space)

**Dynamic Batching** (para árvores que se movem):

- Apenas para meshes < 300 vértices (sua arte 2D provavelmente atende)
- `Edit > Project Settings > Player > Other Settings > Enable Dynamic Batching`
- Funciona automaticamente para objetos com mesmo material

**Recomendação para seu caso**: Se árvores são estáticas no mapa, use Static Batching. Se oscilam levemente (vento), considere Dynamic Batching com validação em Profiler.

### 3.2 Verificar Batching via Frame Debugger[^12][^13]

`Window → Analysis → Frame Debugger`

Procure por "draw calls" que mostram múltiplos objetos sendo renderizados junto (sinal de batching bem-sucedido).

***

## 4. Camada 3: Animation Performance - The Critical Decision

Este é o ponto de decisão arquitetural. Você tem três caminhos:

### 4.1 Opção A: Manter Animators com Otimizações[^1][^14]

Se suas árvores usam Animator simples:

**a) Reduce Animator Complexity**

- Remova BlendTrees desnecessários
- Use apenas 1-2 parâmetros por Animator
- Evite múltiplas camadas de animação

**b) Disable Animator para Off-Screen Objects**

```csharp
public class TreeAnimationManager : MonoBehaviour
{
    private Animator _animator;
    
    void Update()
    {
        // Frustum culling manual para Animators
        if (GeometryUtility.TestPlanesAABB(GetCameraFrustumPlanes(), GetComponent<Renderer>().bounds))
        {
            _animator.enabled = true;
        }
        else
        {
            _animator.enabled = false; // Salva ~1ms por 100 animators desativados
        }
    }
    
    Plane[] GetCameraFrustumPlanes() => 
        GeometryUtility.CalculateFrustumPlanes(Camera.main);
}
```

**Limite esperado**: ~5-20 Animators antes de hit significativo em performance (desktop).

### 4.2 Opção B: Flipbook Shader Animation (HIGH PERFORMANCE)[^15][^16][^17][^18]

**O Conceito**: Em vez de Animator controlar quadros, um shader faz isso no GPU.

**Vantagem**:

- Suporta 1000+ sprites animados simultaneamente
- Praticamente zero custo de CPU para animação
- Frame-skipping integrado (para efeito estilístico ou otimização)

**Desvantagem**:

- Requer sprite sheet (all frames em uma textura)
- Shader customizado ou Shader Graph
- Mais complexo de debugar

**Implementação Rápida (com Shader Graph)**:

1. Crie um sprite sheet: todas as frames de animação em uma grid (ex: 4x4 = 16 frames)
2. Crie um Shader Graph:
    - Adicione uma textura
    - Use o nó **Flipbook** (Shader Graph → Texture → Flipbook)
    - Configure **Width** (4) e **Height** (4)
    - Conecte a um **Sample Texture 2D**
3. Anime via inspector ou código:
```csharp
public class ShaderFlipbookAnimator : MonoBehaviour
{
    private Material _material;
    private float _frameIndex = 0;
    public int framesPerSecond = 10;
    
    void Start() => _material = GetComponent<SpriteRenderer>().material;
    
    void Update()
    {
        _frameIndex += framesPerSecond * Time.deltaTime;
        _material.SetFloat("_FrameIndex", _frameIndex % 16); // 16 frames no exemplo
    }
}
```

**Resultado**: Rode teste com 100 árvores usando flipbook vs Animator. Diferença é dramática (~5-10x mais rápido).

### 4.3 Opção C: GPU Instancing + Material Property Blocks[^19][^20]

Para quando você tem múltiplas cópias de EXATAMENTE o mesmo objeto (animado):

```csharp
// Material com GPU Instancing ativado
Material instancedMaterial = new Material(baseMaterial);
instancedMaterial.enableInstancing = true;

// Renderizar 500 árvores em uma única draw call
for (int i = 0; i < 500; i++)
{
    MaterialPropertyBlock props = new MaterialPropertyBlock();
    props.SetColor("_Color", Random.ColorHSV()); // Variação por instância
    Graphics.DrawMesh(mesh, positions[i], Quaternion.identity, instancedMaterial, 0, null, 0, props);
}
```

**Limitação**: Funciona melhor com objetos visualmente idênticos.

***

## 5. Camada 4: Level of Detail (LOD) System[^21][^22][^23]

Para árvores distantes da câmera, renderize versões simplificadas.

### 5.1 Implementação Manual para 2D

```csharp
public class TreeLOD : MonoBehaviour
{
    public Sprite highQuality;
    public Sprite lowQuality;
    public float lodDistance = 20f;
    private SpriteRenderer _spriteRenderer;
    private Transform _cameraTransform;
    
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _cameraTransform = Camera.main.transform;
    }
    
    void Update()
    {
        float distance = Vector3.Distance(transform.position, _cameraTransform.position);
        _spriteRenderer.sprite = distance > lodDistance ? lowQuality : highQuality;
    }
}
```


### 5.2 Ganhos Esperados

| Distância | Sprite | Frames/Sec | Vertices |
| :-- | :-- | :-- | :-- |
| < 10m | Full | 24 | 100% |
| 10-30m | Reduced | 12 | 50% |
| > 30m | Tiny/Hidden | 4 | 10% |

Com 500 sprites espalhados, ~70% estarão longe → economia significativa.

***

## 6. Camada 5: Advanced - Frame Skipping \& Animation Culling[^24]

Para otimização extrema:

### 6.1 Frame-Skip para Animações Off-Screen

```csharp
public class AdaptiveAnimationUpdater : MonoBehaviour
{
    private Animator _animator;
    private float _updateTimer = 0;
    public float updateInterval = 0.2f; // Update a cada 200ms (5 FPS visual)
    
    void Update()
    {
        _updateTimer += Time.deltaTime;
        if (_updateTimer >= updateInterval)
        {
            _animator.Update(updateInterval);
            _updateTimer = 0;
        }
    }
}
```

**Uso**: Aplique a árvores completamente off-screen ou muito distantes.

### 6.2 Occlusion Culling (Avançado)[^25][^26]

Se você tem estruturas (casas, montanhas) que ocluem árvores:

1. `Window → Rendering → Occlusion Culling`
2. Bake a cena (leva tempo, mas one-time)
3. Objetos atrás de obstáculos não são renderizados

**Ganho**: Potencial 30-50% redução de draw calls em cenas complexas.

***
