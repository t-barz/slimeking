# Setup da TitleScreen com Sequência de Animações

## 🎬 **Visão Geral da Sequência**

1. **Início**: Todos os elementos invisíveis (alpha = 0)
2. **1 segundo**: Música inicia
3. **centerLogo**: Aparece → fica visível → desaparece
4. **background**: Aparece (simultâneo ao centerLogo desaparecendo)
5. **gameTitle**: Aparece (quando background estiver totalmente visível)
6. **wsLogo**: Aparece (quando gameTitle estiver totalmente visível)

## 🎨 **Setup no Unity Editor**

### **1. Estrutura da Cena TitleScreen**

```
TitleScreen
├── GameManager (GameObject)
│   └── GameManager.cs
├── AudioManager (GameObject) 
│   └── AudioManager.cs
└── TitleCanvas (Canvas)
    ├── TitleScreenController.cs
    ├── Background (Image) 
    ├── CenterLogo (Image)
    ├── GameTitle (Image)
    └── WSLogo (Image)
```

### **2. Canvas Setup**

1. Crie **Canvas** (UI → Canvas)
2. Configure **Canvas Scaler** para `Scale With Screen Size`
3. **Reference Resolution**: 1920x1080
4. Adicione **TitleScreenController** script ao Canvas

### **3. Imagens (UI → Image)**

#### **Background**

- **RectTransform**: Anchors Stretch (0,0) to (1,1)
- **Offsets**: Left=0, Top=0, Right=0, Bottom=0
- **Source Image**: Sua imagem de background
- **Color**: RGB normal, **Alpha = 1** (será controlado por script)

#### **CenterLogo**

- **RectTransform**: Middle Center (0.5, 0.5)
- **Position**: (0, 0)
- **Source Image**: Seu logo central
- **Color**: RGB normal, **Alpha = 1** (será controlado por script)

#### **GameTitle**

- **RectTransform**: Posição conforme design
- **Source Image**: Título do jogo
- **Color**: RGB normal, **Alpha = 1** (será controlado por script)

#### **WSLogo**

- **RectTransform**: Posição conforme design (geralmente canto)
- **Source Image**: Logo do estúdio
- **Color**: RGB normal, **Alpha = 1** (será controlado por script)

### **4. TitleScreenController Setup**

No Inspector do Canvas com TitleScreenController:

```
TitleScreenController
├── UI Elements
│   ├── Center Logo: [arraste CenterLogo]
│   ├── Ws Logo: [arraste WSLogo] 
│   ├── Background: [arraste Background]
│   └── Game Title: [arraste GameTitle]
├── Animation Timings
│   ├── Music Delay: 1.0
│   ├── Center Logo Fade In Duration: 1.5
│   ├── Center Logo Visible Duration: 2.0
│   ├── Center Logo Fade Out Duration: 1.5
│   ├── Background Fade In Duration: 2.0
│   ├── Game Title Fade In Duration: 1.5
│   └── Ws Logo Fade In Duration: 1.0
├── Animation Curves
│   ├── Fade In Curve: EaseInOut
│   └── Fade Out Curve: EaseInOut
└── Control
    ├── Auto Start: ✓
    ├── Skip On Input: ✓
    └── Skip Key: Space
```

### **5. GameManager Setup**

No Inspector do GameManager:

```
GameManager
├── Game State
│   └── Current Game State: MainMenu (não Splash)
└── Scene Configuration
    └── Main Menu Scene Name: "TitleScreen"
```

## ⚙️ **Configurações Recomendadas**

### **Para Logo Corporativo Rápido:**

```
Music Delay: 0.5f
Center Logo Fade In: 1.0f
Center Logo Visible: 1.5f
Center Logo Fade Out: 1.0f
Background Fade In: 1.5f
Game Title Fade In: 1.0f
Ws Logo Fade In: 0.8f
```

### **Para Apresentação Cinematográfica:**

```
Music Delay: 1.5f
Center Logo Fade In: 2.0f
Center Logo Visible: 3.0f
Center Logo Fade Out: 2.0f
Background Fade In: 2.5f
Game Title Fade In: 2.0f
Ws Logo Fade In: 1.5f
```

### **Para Desenvolvimento/Teste:**

```
Music Delay: 0.1f
Center Logo Fade In: 0.3f
Center Logo Visible: 0.5f
Center Logo Fade Out: 0.3f
Background Fade In: 0.5f
Game Title Fade In: 0.3f
Ws Logo Fade In: 0.3f
Skip On Input: ✓ (Space ou qualquer tecla pula)
```

## 🎵 **Fluxo de Música**

1. **TitleScreen carrega** → elementos invisíveis
2. **1 segundo depois** → `AudioManager.PlayMenuMusic()`
3. **Animações começam** com a música
4. **Música continua** durante toda a sequência

## 🎮 **Controles de Debug**

### **Durante Runtime:**

- **Space** ou **qualquer tecla**: Pula para o final da sequência
- **Context Menu** (Right Click no componente):
  - "Test Title Sequence" - Reinicia sequência
  - "Skip to End" - Pula para final
  - "Reset Elements" - Reseta para invisível

### **Logs no Console:**

```
[TitleScreen] Elementos inicializados como invisíveis
[TitleScreen] Iniciando sequência da tela de título  
[TitleScreen] Música iniciada
[TitleScreen] centerLogo fade in concluído
[TitleScreen] centerLogo fade out concluído
[TitleScreen] background fade in concluído
[TitleScreen] gameTitle fade in concluído  
[TitleScreen] wsLogo fade in concluído
[TitleScreen] Sequência de animação concluída
```

## 🔧 **Troubleshooting**

| Problema | Solução |
|----------|---------|
| Elementos não aparecem | Verificar se referências foram arrastadas no TitleScreenController |
| Música não toca | Verificar se AudioManager está na cena |
| Animação muito rápida/lenta | Ajustar durações no Animation Timings |
| Não pula com tecla | Verificar Skip On Input ativado |
| Curva estranha | Usar AnimationCurve.EaseInOut ou Linear |

## 📋 **Checklist Final**

- [ ] Canvas com TitleScreenController
- [ ] 4 imagens (background, centerLogo, gameTitle, wsLogo) criadas
- [ ] Todas as referências arrastadas no TitleScreenController
- [ ] GameManager.currentGameState = MainMenu
- [ ] GameManager.mainMenuSceneName = "TitleScreen"
- [ ] AudioManager na cena com música de menu configurada
- [ ] Build Settings contém cena TitleScreen

## 🎯 **Resultado Final**

✅ **Jogo inicia direto na TitleScreen**  
✅ **Todos elementos começam invisíveis**  
✅ **Música inicia 1 segundo após carregar**  
✅ **Sequência suave: centerLogo → background → gameTitle → wsLogo**  
✅ **Pode pular com qualquer tecla**  
✅ **Logs detalhados para debug**

---

**Setup completo!** 🎮✨ A TitleScreen agora tem uma sequência profissional de apresentação com música e animações sincronizadas.
