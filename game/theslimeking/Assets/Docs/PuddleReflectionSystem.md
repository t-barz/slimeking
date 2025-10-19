# 🌊 Sistema de Reflexo de Poça - Guia de Uso

## 📋 Visão Geral

O Sistema de Reflexo de Poça permite criar reflexos dinâmicos e realistas em superfícies de água em jogos 2D. O sistema detecta automaticamente objetos que entram na área da poça e os reflete na superfície da água com efeitos visuais como distorção, fade e escurecimento.

## 🏗️ Componentes do Sistema

### 1. **PuddleReflectionTrigger**

- Detecta objetos que entram e saem da área da poça
- Usa Collider2D como trigger
- Filtra objetos por tags configuráveis
- Dispara eventos quando a lista de objetos muda

### 2. **PuddleReflectionController**

- Controller principal que gerencia o sistema de reflexo
- Cria e gerencia câmera auxiliar e RenderTexture
- Aplica parâmetros do material dinamicamente
- Otimiza performance renderizando apenas quando necessário

### 3. **PuddleReflection.shader**

- Shader URP que cria o efeito visual do reflexo
- Suporte a distorção de ondas, fade vertical e escurecimento
- Combina textura base da poça com reflexo capturado

## 🚀 Como Usar

### Passo 1: Preparar o GameObject da Poça

1. Crie um GameObject com **SpriteRenderer**
2. Adicione um **Collider2D** (Box, Circle, etc.)
3. Configure o collider para cobrir a área da poça
4. Adicione os componentes do sistema:
   - `PuddleReflectionTrigger`
   - `PuddleReflectionController`

### Passo 2: Configurar o Material

1. Crie um Material usando o shader **SlimeKing/2D/PuddleReflection**
2. Atribua a textura da poça no parâmetro **Puddle Sprite**
3. Ajuste os parâmetros visuais conforme desejado
4. Aplique o material ao SpriteRenderer da poça

### Passo 3: Configurar Objetos Refletíveis

1. Certifique-se de que os objetos que devem aparecer no reflexo possuem as tags corretas
2. Tags padrão suportadas: `"Player"`, `"Enemy"`
3. Adicione novas tags através do Inspector ou via código

## ⚙️ Configurações Disponíveis

### PuddleReflectionTrigger

| Parâmetro | Descrição | Padrão |
|-----------|-----------|---------|
| **Reflectable Tags** | Tags dos objetos que aparecem no reflexo | ["Player", "Enemy"] |
| **Enable Logs** | Ativa logs de debug | false |
| **Enable Gizmos** | Desenha gizmos no Scene View | true |

### PuddleReflectionController

| Parâmetro | Descrição | Padrão |
|-----------|-----------|---------|
| **Texture Size** | Resolução da RenderTexture | 512 |
| **Update Interval** | Intervalo entre atualizações (segundos) | 0.033 (~30fps) |
| **Reflection Strength** | Força do efeito de reflexo (0-1) | 0.6 |
| **Distortion Amount** | Quantidade de distorção das ondas | 0.015 |
| **Fade Start** | Onde o fade vertical começa (0-1) | 0.3 |
| **Darken Factor** | Escurecimento do reflexo (0-1) | 0.25 |

### Material (Shader Properties)

| Parâmetro | Descrição | Padrão |
|-----------|-----------|---------|
| **Puddle Sprite** | Textura base da poça | - |
| **Tint Color** | Cor de matiz da poça | Branco |
| **Reflection Strength** | Força do reflexo | 0.6 |
| **Vertical Fade Start** | Início do fade vertical | 0.3 |
| **Distortion Amount** | Quantidade de distorção | 0.015 |
| **Distortion Speed** | Velocidade da animação | 1.2 |
| **Wave Frequency** | Frequência das ondas | 12 |
| **Darken Factor** | Escurecimento do reflexo | 0.25 |

## 🎮 API Pública

### PuddleReflectionController

```csharp
// Controle dos parâmetros visuais
SetReflectionStrength(float value);    // 0-1
SetDistortionAmount(float value);      // 0-0.1
SetFadeStart(float value);             // 0-1
SetDarkenFactor(float value);          // 0-1
SetUpdateInterval(float interval);     // Segundos

// Informações do estado
int GetReflectableObjectCount();       // Número de objetos sendo refletidos
void ForceUpdate();                    // Força atualização imediata
```

### PuddleReflectionTrigger

```csharp
// Gerenciamento de tags
AddReflectableTag(string tag);
RemoveReflectableTag(string tag);

// Consulta de estado
List<GameObject> GetObjectsInTrigger();
int GetObjectCount();
bool ContainsObject(GameObject obj);
```

## 🎯 Exemplos Práticos

### Exemplo 1: Poça Básica

```csharp
// Configura uma poça simples que reflete o player
var puddle = new GameObject("WaterPuddle");
var spriteRenderer = puddle.AddComponent<SpriteRenderer>();
var boxCollider = puddle.AddComponent<BoxCollider2D>();
var trigger = puddle.AddComponent<PuddleReflectionTrigger>();
var controller = puddle.AddComponent<PuddleReflectionController>();

// Configura o trigger
boxCollider.isTrigger = true;
boxCollider.size = new Vector2(3f, 2f);

// Aplica material de reflexo
spriteRenderer.material = puddleReflectionMaterial;
```

### Exemplo 2: Ajuste Dinâmico de Parâmetros

```csharp
public class PuddleEffectController : MonoBehaviour
{
    [SerializeField] private PuddleReflectionController puddleController;
    
    private void Start()
    {
        // Configura reflexo sutil durante o dia
        puddleController.SetReflectionStrength(0.4f);
        puddleController.SetDistortionAmount(0.01f);
    }
    
    public void SetNightMode()
    {
        // Reflexo mais forte à noite
        puddleController.SetReflectionStrength(0.8f);
        puddleController.SetDarkenFactor(0.1f);
    }
}
```

### Exemplo 3: Sistema de Eventos

```csharp
public class PuddleInteractionDetector : MonoBehaviour
{
    private PuddleReflectionTrigger trigger;
    
    private void Awake()
    {
        trigger = GetComponent<PuddleReflectionTrigger>();
        trigger.OnReflectableObjectsChanged += OnObjectsChanged;
    }
    
    private void OnObjectsChanged(List<GameObject> objects)
    {
        if (objects.Count > 0)
        {
            // Player pisou na poça - tocar som de splash
            AudioManager.Instance.PlaySFX("water_splash");
        }
    }
}
```

## ⚡ Otimização de Performance

### Dicas para Melhor Performance

1. **Resolução da Textura**: Use 256x256 ou 512x512 para a maioria dos casos
2. **Update Interval**: 30fps (0.033s) é suficiente para movimento fluido
3. **Culling**: O sistema automaticamente otimiza o culling da câmera
4. **Múltiplas Poças**: Para muitas poças, considere usar um pool de RenderTextures

### Configurações Recomendadas por Plataforma

| Plataforma | Texture Size | Update Interval | Notas |
|------------|--------------|-----------------|-------|
| **Desktop** | 512x512 | 0.033s | Performance completa |
| **Mobile** | 256x256 | 0.05s | Balanceado |
| **Low-end** | 128x128 | 0.1s | Performance otimizada |

## 🐛 Solução de Problemas

### Problema: Reflexo não aparece

- ✅ Verifique se o material usa o shader correto
- ✅ Confirme que objetos possuem tags configuradas
- ✅ Certifique-se de que o Collider2D é um trigger

### Problema: Performance baixa

- ✅ Reduza a resolução da RenderTexture
- ✅ Aumente o Update Interval
- ✅ Verifique se há muitos objetos sendo refletidos

### Problema: Reflexo distorcido

- ✅ Ajuste os parâmetros de distorção no material
- ✅ Verifique o tamanho da câmera ortográfica
- ✅ Confirme que a textura UV está correta

## 🔄 Versionamento

- **v1.0**: Implementação inicial com detecção por trigger
- Compatível com Unity 6.2+ e URP
- Testado em projetos 2D Top-Down

## 📚 Referências

- [Unity URP Shader Documentation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)
- [Boas Práticas SlimeKing](../Docs/BoasPraticas.md)
- [Unity 2D Reflection Techniques](https://docs.unity3d.com/Manual/2DRendering.html)
