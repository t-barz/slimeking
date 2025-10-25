# 🔲 Sistema de Outline - Guia Rápido

Sistema moderno de outline para criar efeitos visuais quando o player se aproxima de objetos.

## 🚀 Uso Rápido

### **Método 1: Usar o Utilitário (Recomendado)**

```csharp
using SlimeKing.Visual;

// Para outline automático (detecta player)
OutlineController outline = OutlineUtility.SetupAutoOutline(gameObject, Color.white, 2.0f);

// Para outline manual (controle por script)
OutlineController outline = OutlineUtility.SetupManualOutline(gameObject, Color.cyan);
```

### **Método 2: Adicionar Componente**

1. Selecione o GameObject com `SpriteRenderer`
2. Adicione o componente `OutlineController`
3. Configure as propriedades no Inspector

### **Método 3: Usar o Script de Exemplo**

1. Adicione o componente `OutlineExample` ao objeto
2. Configure as opções no Inspector
3. Escolha entre detecção automática ou controle manual

## ⚙️ Configurações Principais

### **Detecção Automática:**

- `Enable Auto Detection`: ✅ Ativado
- `Detection Radius`: 1.5 - 3.0 (distância do player)
- `Player Tag`: "Player" (certifique-se que o player tem essa tag)

### **Visual:**

- `Outline Color`: Cor do outline (branco, azul, etc.)
- `Outline Size`: 0.03 - 0.05 (espessura da borda)
- `Enable Fade`: ✅ Para efeito suave

## 📱 Controle por Script

```csharp
// Controle básico
outlineController.ShowOutline(true);  // Liga
outlineController.ShowOutline(false); // Desliga

// Métodos modernos
outlineController.ActivateOutline();
outlineController.DeactivateOutline();
outlineController.ToggleOutline();

// Configuração dinâmica
outlineController.UpdateOutlineColor(Color.red);
outlineController.UpdateOutlineSize(0.06f);
outlineController.SetDetectionRadius(2.5f);
```

## 🛠️ Troubleshooting

### **Outline não aparece:**

1. Verifique se o Player tem tag "Player"
2. Verifique se `Detection Radius` é grande o suficiente
3. Certifique-se que o shader `SlimeKing/SpriteOutline` está no projeto

### **Cores estranhas:**

1. Use `OutlineUtility.SetupOutlineMaterial()` para corrigir o material
2. Verifique se está usando o shader correto

## 📦 Arquivos do Sistema

- `Assets/Code/Shaders/SpriteOutline.shader` - Shader principal
- `Assets/Code/Materials/SpriteOutlineMaterial.mat` - Material padrão
- `Assets/Code/Visual/OutlineUtility.cs` - Utilitário para setup rápido
- `Assets/Code/Visual/OutlineExample.cs` - Script de exemplo
- `Assets/External/.../OutlineController.cs` - Componente principal

## ✅ Sistema Pronto

O sistema está configurado corretamente na pasta `Assets/Code/`. Use o `OutlineUtility` para setup rápido ou adicione o `OutlineController` diretamente aos objetos.
