# Slime King - Shader Documentation

Este documento fornece detalhes sobre como utilizar os shaders disponíveis no projeto Slime King. Cada shader possui configurações específicas que podem ser ajustadas para obter diferentes efeitos visuais.

## Índice
1. [DamageEffectShader](#damageeffectshader) - Efeito de flash e transparência para indicar dano
2. [Water2DEffectShader](#water2deffectshader) - Efeito de água com ondulações para jogos 2D top-down
3. [SpriteOutline](#spriteoutline) - Contorno para sprites com configuração de tamanho e cor

---

<a name="damageeffectshader"></a>
## 1. DamageEffectShader

**Descrição:** Cria um efeito visual de dano que consiste em um flash rápido seguido de um período de semitransparência. Ideal para indicar quando um personagem ou inimigo recebeu dano.

### Propriedades do Shader:

| Propriedade | Descrição | Valores recomendados |
|-------------|-----------|----------------------|
| _MainTex | Textura principal do sprite | Texture2D do sprite |
| _Color | Cor base do sprite | Branco (1,1,1,1) para preservar cores originais |
| _FlashColor | Cor do efeito de flash | Vermelho (1,0,0,1) para dano, branco para invencibilidade |
| _FlashIntensity | Intensidade do flash | 0.5 - 0.8 |
| _SemiTransparency | Nível de transparência após o flash | 0.3 - 0.5 |
| _HitTime | Momento do impacto (controlado via script) | Definido automaticamente ao receber dano |
| _FlashDuration | Duração do efeito de flash em segundos | 0.1 - 0.3 |
| _TransparencyDuration | Duração do efeito de transparência em segundos | 0.5 - 2.0 |

### Como usar:

1. **Configuração no Editor:**
   - Crie um novo material com o shader "SlimeKing/DamageEffectShader"
   - Ajuste as propriedades conforme necessário no Inspector
   - Aplique o material ao sprite do objeto que receberá o efeito de dano

2. **Script para ativar o efeito:**
```csharp
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [SerializeField] private Material damageMaterial;

    public void TakeDamage()
    {
        // Atualiza o tempo do hit para o tempo atual do jogo
        damageMaterial.SetFloat("_HitTime", Time.time);
        
        // Opcionalmente, você pode alterar outras propriedades dinamicamente:
        // damageMaterial.SetColor("_FlashColor", Color.red);
        // damageMaterial.SetFloat("_FlashDuration", 0.2f);
    }
}
```

3. **Dicas de uso:**
   - Para inimigos, use um flash vermelho mais intenso
   - Para o jogador, considere usar um flash branco ou amarelo para invencibilidade
   - Ajuste a duração do flash e da transparência conforme o feedback visual desejado
   - Use valores menores em plataformas móveis para melhor performance

---

<a name="water2deffectshader"></a>
## 2. Water2DEffectShader

**Descrição:** Cria um efeito de água animada com ondulações para jogos 2D top-down. Este shader transforma tiles ou sprites específicos em água realista detectando uma cor configurável.

### Propriedades do Shader:

| Propriedade | Descrição | Valores recomendados |
|-------------|-----------|----------------------|
| _MainTex | Textura do sprite/tile a ser transformado | Textura do tileset ou sprite |
| _WaterColor | Cor base da água | Azul semitransparente (0.2, 0.5, 0.8, 0.7) |
| _DetectionColor | Cor a ser substituída pelo efeito de água | Depende do tileset (geralmente azul) |
| _ColorTolerance | Tolerância para detecção de cor | 0.1 - 0.3 |
| _WaterTransparency | Nível de transparência da água | 0.7 - 0.9 |
| _WaveSpeed | Velocidade de animação das ondas | 1 - 3 |
| _WaveFrequency | Frequência das ondulações (mais alto = ondas menores) | 10 - 20 |
| _WaveAmplitude | Amplitude das ondulações | 0.005 - 0.02 |
| _ReflectionIntensity | Intensidade dos reflexos na água | 0.3 - 0.7 |
| _Glossiness | Brilho da superfície da água | 0.5 - 0.8 |
| _RimPower | Controla a nitidez das bordas dos reflexos | 2 - 5 |
| _RimColor | Cor dos reflexos na água | Branco semitransparente (1,1,1,0.5) |

### Como usar:

1. **Configuração no Editor:**
   - Crie um novo material com o shader "SlimeKing/2D Water Effect"
   - Configure a _DetectionColor para corresponder à cor da água no seu tileset
   - Ajuste os parâmetros de ondas e reflexos conforme desejado
   - Aplique o material ao objeto contendo seu tilemap ou sprite de água

2. **Controlador para ajustes em tempo real:**
   - Você pode utilizar o script WaterEffectController para controlar o comportamento da água durante o jogo:

```csharp
// Exemplo de uso:
WaterEffectController waterController = GetComponent<WaterEffectController>();
// Aumentar velocidade das ondas durante uma tempestade:
waterController.SetWaveSpeed(5.0f);
// Alterar a transparência da água:
waterController.SetTransparency(0.5f);
// Alterar a cor da água para um lago tóxico:
waterController.SetWaterColor(new Color(0.2f, 0.8f, 0.2f, 0.7f));
```

3. **Dicas de uso:**
   - Para lagos calmos: use menor _WaveSpeed e menor _WaveAmplitude
   - Para rios: aumente a _WaveSpeed e ajuste a direção das ondas
   - Para água rasa: aumente a _WaterTransparency
   - Para água profunda: diminua a _WaterTransparency e use uma cor mais escura

---

<a name="spriteoutline"></a>
## 3. SpriteOutline

**Descrição:** Adiciona um contorno ao redor de sprites 2D. Útil para destacar objetos interativos, personagens selecionados ou itens importantes.

### Propriedades do Shader:

| Propriedade | Descrição | Valores recomendados |
|-------------|-----------|----------------------|
| _MainTex | Textura do sprite | Texture2D do sprite |
| _OutlineColor | Cor do contorno | Varia conforme o uso (branco, amarelo, etc.) |
| _OutlineSize | Espessura do contorno | 0.01 - 0.05 |
| _AlphaThreshold | Limite para detecção de bordas | 0.5 (ajustar para sprites com transparência parcial) |

### Como usar:

1. **Configuração no Editor:**
   - Crie um novo material com o shader "Custom/Builtin_SpriteOutline"
   - Configure a cor e tamanho do contorno
   - Aplique o material ao sprite que receberá o contorno

2. **Script para destacar objetos:**
```csharp
using UnityEngine;

public class OutlineHighlighter : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color highlightColor = Color.yellow;
    
    private Material originalMaterial;
    
    private void Start()
    {
        originalMaterial = spriteRenderer.material;
    }
    
    public void Highlight(bool enable)
    {
        if (enable)
        {
            spriteRenderer.material = outlineMaterial;
            outlineMaterial.SetColor("_OutlineColor", highlightColor);
        }
        else
        {
            spriteRenderer.material = originalMaterial;
        }
    }
}
```

3. **Dicas de uso:**
   - Use contornos amarelos ou brancos para itens interativos
   - Use contornos vermelhos para indicar inimigos ou perigo
   - Use contornos verdes para aliados ou itens benéficos
   - Ajuste _AlphaThreshold para melhorar a detecção de bordas em sprites com transparência parcial

---

## Considerações de Performance

- Os shaders foram otimizados para funcionar em múltiplas plataformas, incluindo dispositivos móveis
- Para melhor performance em dispositivos de baixo desempenho:
  - Reduza o número de objetos usando o DamageEffectShader simultaneamente
  - Diminua a área total de Water2DEffectShader visível na tela
  - Considere desabilitar efeitos complexos como reflexos em plataformas móveis

## Solução de Problemas

- **Problema:** Shader não aparece no Editor
  **Solução:** Verifique se o pipeline de renderização URP está configurado corretamente.

- **Problema:** Efeito de água não detecta corretamente as áreas
  **Solução:** Ajuste _ColorTolerance ou use um sprite com cores mais consistentes.

- **Problema:** Contorno não aparece corretamente em sprites com transparência
  **Solução:** Ajuste o _AlphaThreshold para corresponder melhor à transparência do seu sprite.

- **Problema:** Queda de performance com muitos efeitos
  **Solução:** Reduza a complexidade dos shaders ou limite o número de objetos usando-os simultaneamente.
