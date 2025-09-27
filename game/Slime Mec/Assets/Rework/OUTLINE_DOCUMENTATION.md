# Documentação do Sistema de Outline - Slime Land A Puzzle Adventure

## Visão Geral

O sistema de Outline no projeto "Slime Land A Puzzle Adventure" é usado para criar efeitos visuais de contorno em sprites, principalmente para destacar objetos interativos quando o jogador se aproxima deles. O sistema é composto por três componentes principais: shader, material e scripts de controle.

## Arquivos Necessários

### 1. **Shader - SpriteOutline.shader**

- **Localização**: `Assets/VisualEffects/Environment/SpriteOutline.shader`
- **Nome do Shader**: `Custom/Builtin_SpriteOutline`
- **Função**: Define o algoritmo de renderização que cria o efeito de contorno ao redor dos sprites

### 2. **Material - VFXSpriteOutline.mat**

- **Localização**: `Assets/VisualEffects/Environment/VFXSpriteOutline.mat`
- **Função**: Material que utiliza o shader SpriteOutline e define as propriedades visuais do contorno

### 3. **Script Principal - VFXOutlineObject.cs**

- **Localização**: `Assets/VisualEffects/Environment/VFXOutlineObject.cs`
- **Função**: Controla a ativação/desativação do efeito de outline

### 4. **Script de Integração - WSInteractiveObject.cs**

- **Localização**: `Assets/_Old/Scripts/Environment/WSInteractiveObject.cs`
- **Função**: Integra o sistema de outline com objetos interativos

## Como Funciona o Sistema

### Algoritmo do Shader

O shader `SpriteOutline.shader` funciona através do seguinte processo:

1. **Amostragem em 8 Direções**: Para cada pixel, o shader verifica os 8 pixels vizinhos ao redor
2. **Detecção de Bordas**: Identifica pixels transparentes que têm vizinhos opacos
3. **Renderização do Contorno**: Pixels de borda recebem a cor do outline, pixels internos mantêm a cor original

```glsl
// Amostras em 8 direções
float2 offsets[8] = {
    float2(-1, 0), float2(1, 0),    // Horizontal
    float2(0, 1), float2(0, -1),    // Vertical
    float2(-1, 1), float2(1, 1),    // Diagonal
    float2(-1, -1), float2(1, -1)
};
```

### Propriedades do Shader

| Propriedade | Tipo | Descrição | Valor Padrão |
|-------------|------|-----------|--------------|
| `_MainTex` | Texture2D | Textura do sprite | white |
| `_OutlineColor` | Color | Cor do contorno | (1,1,1,1) |
| `_OutlineSize` | Range(0, 0.1) | Espessura do contorno | 0.03 |
| `_AlphaThreshold` | Range(0, 1) | Limiar de transparência | 0.5 |

## Como Implementar o Outline

### Método 1: Usando VFXOutlineObject (Recomendado)

#### Passo 1: Preparar o GameObject

```csharp
// Certifique-se que o objeto tem um SpriteRenderer
SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
```

#### Passo 2: Aplicar o Material

1. No Inspector do GameObject, localize o componente **SpriteRenderer**
2. No campo **Material**, arraste o material `VFXSpriteOutline.mat`

#### Passo 3: Adicionar o Script de Controle

```csharp
// Adicione o componente VFXOutlineObject ao GameObject
public class VFXOutlineObject : MonoBehaviour
{
    private Material material;

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        // Outline inicia desabilitado
        material.SetFloat("_ShowOutline", 0f);
    }

    public void ShowOutline(bool show)
    {
        material.SetFloat("_ShowOutline", show ? 1f : 0f);
    }
}
```

#### Passo 4: Controlar o Outline

```csharp
// Para ativar o outline
VFXOutlineObject outlineObj = GetComponent<VFXOutlineObject>();
outlineObj.ShowOutline(true);

// Para desativar o outline
outlineObj.ShowOutline(false);
```

### Método 2: Implementação Manual

#### Criando um Material em Runtime

```csharp
// Exemplo do StrokeFX.cs (arquivo legado)
void Start()
{
    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    Material material = new Material(Shader.Find("Custom/Builtin_SpriteOutline"));
    spriteRenderer.material = material;
    
    // Configurar propriedades
    material.SetColor("_OutlineColor", Color.white);
    material.SetFloat("_OutlineSize", 0.03f);
    material.SetFloat("_AlphaThreshold", 0.5f);
}
```

## Exemplo de Uso: Objetos Interativos

O arquivo `WSInteractiveObject.cs` demonstra como integrar o sistema de outline com objetos interativos:

```csharp
public class WSInteractiveObject : MonoBehaviour
{
    private VFXOutlineObject outlineObject;

    void Start()
    {
        outlineObject = GetComponent<VFXOutlineObject>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && outlineObject != null)
        {
            // Ativa o outline quando o player se aproxima
            outlineObject.ShowOutline(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && outlineObject != null)
        {
            // Desativa o outline quando o player se afasta
            outlineObject.ShowOutline(false);
        }
    }
}
```

## Configuração Visual

### Ajustando as Propriedades do Material

1. **Cor do Contorno** (`_OutlineColor`):
   - Branco (1,1,1,1) para contorno clássico
   - Cores vibrantes para destaque especial

2. **Espessura do Contorno** (`_OutlineSize`):
   - 0.01-0.03: Contorno fino e sutil
   - 0.04-0.07: Contorno médio, bem visível
   - 0.08-0.1: Contorno grosso, muito destacado

3. **Limiar de Transparência** (`_AlphaThreshold`):
   - 0.1: Contorno em pixels quase transparentes
   - 0.5: Padrão, bom equilíbrio
   - 0.9: Apenas pixels quase opacos geram contorno

## Considerações de Performance

### Otimizações Implementadas

- **Render Queue**: `"Queue"="Transparent"` para renderização eficiente
- **Blend Mode**: `SrcAlpha OneMinusSrcAlpha` para transparência adequada
- **Culling**: `Cull Off` permite contorno em ambos os lados

### Recomendações

- Use o material `VFXSpriteOutline.mat` ao invés de criar novos materiais em runtime
- Controle a visibilidade através do script `VFXOutlineObject` ao invés de trocar materiais
- Para muitos objetos, considere usar object pooling

## Solução de Problemas

### Problemas Comuns

1. **Outline não aparece**:
   - Verifique se o material está aplicado corretamente
   - Confirme que `_ShowOutline` está definido como 1.0
   - Verifique se a textura tem áreas transparentes

2. **Outline muito fino ou grosso**:
   - Ajuste a propriedade `_OutlineSize` no material
   - Considere a resolução da textura do sprite

3. **Outline com artifacts**:
   - Ajuste `_AlphaThreshold` para um valor mais apropriado
   - Verifique se a textura do sprite tem bordas limpas

4. **Performance baixa**:
   - Limite o número de objetos com outline ativo simultaneamente
   - Use o sistema de ativação/desativação ao invés de destruir/criar materiais

## Estrutura de Arquivos Recomendada

```
Assets/
├── VisualEffects/
│   └── Environment/
│       ├── SpriteOutline.shader          # Shader principal
│       ├── VFXSpriteOutline.mat          # Material configurado
│       └── VFXOutlineObject.cs           # Script de controle
├── Scripts/
│   └── Environment/
│       └── WSInteractiveObject.cs        # Exemplo de integração
└── Prefabs/
    └── InteractiveObjects/               # Prefabs com outline pré-configurado
```

Este sistema de outline oferece uma solução eficiente e flexível para destacar objetos interativos no jogo, mantendo boa performance e facilidade de uso.
