# Camera Setup para Pixel Art - The Slime King

## 🎯 **Global Light 2D - Otimização para Pixel Art**

### **📋 Por que Global Light 2D é importante:**

A **Global Light 2D** é essencial para jogos pixel art pois:

- ✅ **Iluminação uniforme**: Proporciona uma base de iluminação consistente
- ✅ **Performance otimizada**: Mais eficiente que múltiplas luzes pontuais
- ✅ **Compatibilidade com Post Processing**: Trabalha perfeitamente com efeitos URP
- ✅ **Facilita workflow**: Simplifica o setup de iluminação 2D

### **⚙️ Configurações Otimizadas:**

#### **Configuração Manual Recomendada:**

```
Light Type: Global
Intensity: 1.0
Color: RGB(255, 242, 230) - Branco ligeiramente quente
Volume Opacity: 0.0 (desabilitado para performance)
Blend Style: Normal
```

#### **Benefícios para Pixel Art:**

- **Sem sombras complexas**: Mantém a estética pixel art clean
- **Cor quente sutil**: Adiciona atmosfera sem comprometer cores
- **Performance**: Volume Light desabilitado reduz overhead
- **Consistência**: Evita variações de iluminação indesejadas

## 🎮 **Sistema de Câmera Pixel Perfect Completo**

### **🛠️ Ferramentas Extra Tools Implementadas:**

#### **1. Setup Pixel Perfect Camera**

`Extra Tools > Post Processing > Setup Pixel Perfect Camera`

**O que faz:**

- ✅ Configura Main Camera com componentes essenciais
- ✅ Adiciona **Pixel Perfect Camera** (16 PPU, 320x240 referência)
- ✅ Adiciona **Cinemachine Brain** com blends suaves
- ✅ Configura **URP Camera Data** (Post Processing ON, Anti-aliasing OFF)
- ✅ Settings otimizados para pixel art

**Configurações aplicadas:**

```
Pixel Perfect Camera:
- Assets PPU: 16 (padrão pixel art)
- Reference Resolution: 320x240 (estilo retro)
- Upscale Render Texture: OFF (melhor performance)
- Pixel Snapping: ON (evita pixels borrados)
- Crop Frame: OFF (permite letterboxing)
- Stretch Fill: OFF (mantém aspect ratio)

Cinemachine Brain:
- Default Blend Time: 1.0s (transições suaves)
- Blend Style: EaseInOut
- Update Method: LateUpdate (sincronização)

URP Camera Data:
- Render Post Processing: ON
- Anti-aliasing: NONE (preserva pixel art)
- Render Type: Base
```

#### **2. Setup Global Light 2D**

`Extra Tools > Post Processing > Setup Global Light 2D`

**O que faz:**

- ✅ Cria ou configura Global Light 2D existente
- ✅ Aplica configurações otimizadas para pixel art
- ✅ Define cor ligeiramente quente
- ✅ Desabilita Volume Light para performance

#### **3. Complete Camera Setup**

`Extra Tools > Post Processing > Complete Camera Setup`

**Setup completo que inclui:**

- ✅ Pixel Perfect Camera configurada
- ✅ Global Light 2D otimizada  
- ✅ Post Processing Volume aplicado
- ✅ Cinemachine Brain configurado

## 🔧 **Integração com Post Processing**

### **Pipeline Otimizado:**

```text
Main Camera (Pixel Perfect)
    ↓
Cinemachine Brain (Smooth Blends)
    ↓
URP Renderer (Post Processing ON)
    ↓
Volume Profiles (Global + Biome)
    ↓
Global Light 2D (Consistent Lighting)
    ↓
Final Pixel Art Output
```

### **Compatibilidade com Sistemas Existentes:**

- ✅ **Volume Profiles**: Funciona com todos os profiles de bioma
- ✅ **Cinemachine Virtual Cameras**: Suporte completo
- ✅ **Post Processing Effects**: Bloom, Color Grading, Vignette, etc.
- ✅ **URP Features**: Shadow Casters 2D, Light 2D, etc.

## 🎨 **Melhores Práticas Pixel Art + Post Processing**

### **✅ Configurações Recomendadas:**

#### **Pixel Perfect Camera:**

- **Assets PPU**: 16 (padrão) ou 32 (detalhado)
- **Reference Resolution**: 320x240, 480x270, ou 640x360
- **Pixel Snapping**: Sempre ON
- **Upscale RT**: OFF para performance

#### **Post Processing:**

- **Bloom**: Threshold alto (0.9+) para elementos específicos
- **Anti-aliasing**: NONE (preserve pixel art)
- **Color Grading**: Saturation moderada (+10 a +15)
- **Chromatic Aberration**: Muito sutil (0.1 ou menos)

#### **Global Light 2D:**

- **Intensity**: 0.8 a 1.2 (dependendo da atmosfera)
- **Color**: Ligeiramente quente para atmosfera
- **Volume Opacity**: 0.0 (melhor performance)

### **❌ Evitar:**

- Anti-aliasing em qualquer forma
- Volume Lights complexos
- Muitas luzes dinâmicas
- Post Processing muito agressivo
- Resoluções não múltiplas do PPU

## 🚀 **Workflow de Setup Rápido**

### **Para Nova Cena:**

1. **Execute Complete Camera Setup:**

   ```
   Extra Tools > Post Processing > Complete Camera Setup
   ```

2. **Ajuste Resolution Reference** (se necessário):
   - Pixel Perfect Camera > Reference Resolution
   - 320x240: Estilo NES/Game Boy
   - 480x270: Estilo SNES
   - 640x360: Estilo moderno 16-bit

3. **Configure Cinemachine Virtual Camera:**

   ```csharp
   // Exemplo de Virtual Camera para pixel art
   var virtualCam = GameObject.Find("CM vcam1")?.GetComponent<CinemachineCamera>();
   if (virtualCam != null)
   {
       // Lens settings
       virtualCam.Lens.OrthographicSize = 5f;
       virtualCam.Lens.NearClipPlane = 0.3f;
       virtualCam.Lens.FarClipPlane = 1000f;
       
       // Follow settings suaves
       // Configure Follow e LookAt conforme necessário
   }
   ```

4. **Teste diferentes Volume Profiles:**

   ```
   Extra Tools > Post Processing > Setup [Biome] Volume
   ```

### **Para Cena Existente:**

1. **Backup da cena atual**
2. **Execute Setup Pixel Perfect Camera**
3. **Execute Setup Global Light 2D**
4. **Aplique Volume Profile global**
5. **Ajuste configurações específicas**

## 📊 **Configurações por Resolução Target**

### **320x240 (Retro Classic):**

```
Assets PPU: 16
Orthographic Size: Automático (Pixel Perfect)
Zoom levels: 1x, 2x, 3x, 4x
Target Platforms: Mobile, Web, Desktop
```

### **480x270 (Retro Modern):**

```
Assets PPU: 16 ou 24  
Orthographic Size: Automático (Pixel Perfect)
Zoom levels: 1x, 2x, 3x
Target Platforms: Desktop, Console
```

### **640x360 (HD Pixel Art):**

```
Assets PPU: 32
Orthographic Size: Automático (Pixel Perfect)
Zoom levels: 1x, 2x
Target Platforms: Desktop, Console
```

## 🔍 **Troubleshooting**

### **Problema: Pixels "borrados" ou antialiasing indesejado**

**Solução:**

- Verificar Pixel Perfect Camera > Pixel Snapping = ON
- URP Camera Data > Anti-aliasing = NONE
- Importar sprites com Filter Mode = Point

### **Problema: Post Processing não aparece**

**Solução:**

- URP Camera Data > Render Post Processing = ON
- Verificar se Volume Profile está aplicado
- Verificar se URP está ativo

### **Problema: Performance baixa**

**Solução:**

- Global Light 2D > Volume Opacity = 0
- Pixel Perfect Camera > Upscale RT = OFF
- Usar poucos Volume Profiles ativos simultaneamente

### **Problema: Transições de câmera abruptas**

**Solução:**

- Cinemachine Brain > Default Blend Time = 1.0s+
- Blend Style = EaseInOut
- Update Method = LateUpdate

## 📝 **Scripts de Exemplo**

### **Controller de Zoom Pixel Perfect:**

```csharp
using UnityEngine;

public class PixelPerfectZoom : MonoBehaviour
{
    [SerializeField] private PixelPerfectCamera pixelCamera;
    [SerializeField] private int[] zoomLevels = {1, 2, 3, 4};
    [SerializeField] private int currentZoomIndex = 1;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            CycleZoom();
        }
    }
    
    private void CycleZoom()
    {
        currentZoomIndex = (currentZoomIndex + 1) % zoomLevels.Length;
        
        // Ajusta resolução baseada no zoom
        int baseWidth = 320;
        int baseHeight = 240;
        int zoom = zoomLevels[currentZoomIndex];
        
        pixelCamera.refResolutionX = baseWidth / zoom;
        pixelCamera.refResolutionY = baseHeight / zoom;
    }
}
```

### **Dynamic Light Intensity (Day/Night):**

```csharp
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayNightLighting : MonoBehaviour
{
    [SerializeField] private Light2D globalLight;
    [SerializeField] private AnimationCurve intensityCurve;
    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private Color nightColor = new Color(0.3f, 0.4f, 0.8f);
    
    private void Update()
    {
        float time = (Time.time % 60f) / 60f; // 60 second cycle
        
        globalLight.intensity = intensityCurve.Evaluate(time);
        globalLight.color = Color.Lerp(nightColor, dayColor, intensityCurve.Evaluate(time));
    }
}
```

---

**Status**: ✅ **Sistema completo implementado e documentado**
**Ferramentas**: ✅ **4 funções automáticas no Extra Tools**
**Compatibilidade**: ✅ **URP + Post Processing + Pixel Perfect + Cinemachine**
