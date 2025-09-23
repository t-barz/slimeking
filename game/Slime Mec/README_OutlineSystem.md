# Sistema de Outline via Shader - SlimeMec

Sistema completo de outline para sprites 2D utilizando shader customizado para alta performance.

## 📋 Visão Geral

Este sistema substitui o método tradicional de duplicação de sprites por uma solução baseada em shader HLSL, oferecendo:

- **Performance Superior**: Usa shader em vez de múltiplos SpriteRenderers
- **Controle Dinâmico**: Cor, tamanho e ativação em tempo real
- **Fácil Integração**: Component plug-and-play
- **Compatibilidade**: Funciona com Sprite Atlas e texturas diversas

## 🔧 Componentes do Sistema

### 1. SpriteOutline.shader

Shader HLSL customizado que implementa o efeito de outline:

- **Localização**: `Assets/_Art/Shaders/SpriteOutline.shader`
- **Nome do Shader**: `SlimeMec/SpriteOutline`
- **Propriedades**:
  - `_MainTex`: Textura principal do sprite
  - `_Color`: Cor base do sprite
  - `_OutlineColor`: Cor do outline
  - `_OutlineSize`: Tamanho do outline (0-0.5)
  - `_EnableOutline`: Toggle on/off (0/1)

### 2. OutlineShaderController.cs

Script controlador que gerencia o shader:

- **Localização**: `Assets/_Scripts/Visual/OutlineShaderController.cs`
- **Namespace**: `SlimeMec.Visual`
- **Funcionalidades**:
  - Criação automática de material instance
  - Controle dinâmico de propriedades
  - Context Menu para testes
  - Validações e debug

### 3. Material de Exemplo

Material pré-configurado com o shader:

- **Localização**: `Assets/_Art/Materials/SpriteOutlineMaterial.mat`
- **Configuração**: Pronto para uso com propriedades padrão

## 🚀 Como Usar

### Configuração Básica

1. **Adicionar ao GameObject**:

   ```csharp
   // O GameObject deve ter SpriteRenderer
   GameObject obj = // seu objeto com sprite
   OutlineShaderController controller = obj.AddComponent<OutlineShaderController>();
   ```

2. **Configurar no Inspector**:
   - Arrastar o component para o objeto
   - Configurar cor e tamanho do outline
   - Marcar "Create Material Instance" para múltiplos objetos

3. **Usar via Script**:

   ```csharp
   // Ativar outline
   controller.EnableOutline();
   
   // Alterar cor
   controller.SetOutlineColor(Color.red);
   
   // Alterar tamanho
   controller.SetOutlineSize(0.02f);
   
   // Desativar
   controller.DisableOutline();
   ```

### Integração com Sistema Interativo

O sistema já está integrado com `InteractivePointHandler`:

```csharp
[Header("Outline Effect")]
[SerializeField] private OutlineShaderController outlineController;
[SerializeField] private bool enableOutlineOnInteraction = true;
[SerializeField] private Color interactionOutlineColor = Color.cyan;
```

## 📝 Exemplos de Uso

### Exemplo 1: Outline Simples

```csharp
using SlimeMec.Visual;

public class SimpleOutlineExample : MonoBehaviour
{
    private OutlineShaderController outline;
    
    void Start()
    {
        outline = GetComponent<OutlineShaderController>();
        outline.SetOutlineColor(Color.white);
        outline.EnableOutline();
    }
}
```

### Exemplo 2: Outline Pulsante

```csharp
using SlimeMec.Visual;

public class PulsingOutlineExample : MonoBehaviour
{
    private OutlineShaderController outline;
    
    void Start()
    {
        outline = GetComponent<OutlineShaderController>();
        outline.EnableOutline();
    }
    
    void Update()
    {
        float pulse = Mathf.Sin(Time.time * 2f) * 0.5f + 0.5f;
        float size = Mathf.Lerp(0.005f, 0.03f, pulse);
        outline.SetOutlineSize(size);
    }
}
```

### Exemplo 3: Outline por Proximidade

```csharp
void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        outline.SetOutlineColor(Color.yellow);
        outline.EnableOutline();
    }
}

void OnTriggerExit2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        outline.DisableOutline();
    }
}
```

## 🎮 Controles de Teste

O script `OutlineExampleController.cs` oferece testes interativos:

- **Tecla O**: Toggle outline on/off
- **Tecla P**: Toggle efeito de pulsação
- **Tecla C**: Cicla entre cores predefinidas
- **Tecla R**: Reset para configuração original

## ⚙️ Configurações Avançadas

### Performance

```csharp
// Para múltiplos objetos, sempre usar material instance
[SerializeField] private bool createMaterialInstance = true;

// Para objetos únicos, pode reusar material
[SerializeField] private bool createMaterialInstance = false;
```

### Debug

```csharp
// Ativar logs detalhados
[SerializeField] private bool enableDebugLogs = true;
```

### Shader Properties

O shader expõe as seguintes propriedades para controle manual:

- `_OutlineColor`: Color
- `_OutlineSize`: Float (0-0.5)
- `_EnableOutline`: Float (0 ou 1)

## 🔍 Context Menu (Testes no Editor)

Todos os componentes incluem Context Menu para testes rápidos:

- **Test Enable Outline**: Ativa outline
- **Test Disable Outline**: Desativa outline  
- **Test Toggle Outline**: Alterna estado
- **Debug Info**: Mostra informações detalhadas
- **Force Recreate Material**: Recria material

## ⚠️ Requisitos e Limitações

### Requisitos

- Unity 2021.3 LTS ou superior
- SpriteRenderer no GameObject
- Shader compatível com URP (se usando)

### Limitações

- Outline size limitado a 0.5 para performance
- Funciona apenas com sprites 2D
- Requer SpriteRenderer ativo

## 🐛 Troubleshooting

### Shader não encontrado

```
Erro: "Shader 'SlimeMec/SpriteOutline' not found"
Solução: Verificar se o arquivo .shader está no projeto
```

### Material não criado

```
Erro: Material instance é null
Solução: Verificar se o shader está válido e o objeto tem SpriteRenderer
```

### Performance baixa

```
Problema: Muitos materiais instanciados
Solução: Usar createMaterialInstance = false para objetos que compartilham material
```

## 📊 Comparação de Performance

| Método | Draw Calls | Memory | Performance |
|--------|------------|---------|-------------|
| Sprite Duplication | 8x mais | 8x mais | Baixa |
| Shader Outline | 1x | 1x | Alta |

## 🔄 Migração do Sistema Antigo

Para migrar do `OutlineEffect.cs` (sprite duplication):

1. Remover `OutlineEffect` component
2. Adicionar `OutlineShaderController` component
3. Configurar cor e tamanho equivalentes
4. Atualizar scripts que referenciam o sistema antigo

## 📄 Arquivos do Sistema

```
Assets/
├── _Art/
│   ├── Shaders/
│   │   └── SpriteOutline.shader          # Shader HLSL
│   └── Materials/
│       └── SpriteOutlineMaterial.mat     # Material exemplo
├── _Scripts/
│   ├── Visual/
│   │   └── OutlineShaderController.cs    # Controlador principal
│   ├── Gameplay/
│   │   └── InteractivePointHandler.cs    # Integração com sistema interativo
│   └── Examples/
│       └── OutlineExampleController.cs   # Script de exemplo e testes
└── README_OutlineSystem.md               # Esta documentação
```

---
**Desenvolvido para SlimeMec Game**  
*Sistema de Outline de Alta Performance*
