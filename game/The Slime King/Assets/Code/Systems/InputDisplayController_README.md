# InputDisplayController - Guia de Configuração

## Visão Geral

O `InputDisplayController` é um script que detecta automaticamente o tipo de controle usado pelo jogador e exibe apenas o ícone correspondente na interface. Ele se integra perfeitamente com o `InputManager` existente do projeto.

## Configuração no Unity

### 1. Adicionar o Script ao GameObject

1. Selecione o objeto `inputButton` na hierarquia da cena
2. No Inspector, clique em "Add Component"
3. Procure por "Input Display Controller" e adicione

### 2. Configurar as Referências dos Ícones

No Inspector do `InputDisplayController`, configure os seguintes campos:

**Input Icons:**

- `Gamepad Icon`: Arraste o objeto filho "gamepad"
- `PlayStation Icon`: Arraste o objeto filho "playstation"
- `Switch Icon`: Arraste o objeto filho "switch"
- `Xbox Icon`: Arraste o objeto filho "xbox"
- `Keyboard Icon`: Arraste o objeto filho "keyboard"

**Settings:**

- `Update Interval`: 0.3 (padrão) - Frequência de verificação em segundos
- `Auto Detect On Start`: ✓ Marcado - Detecta automaticamente ao iniciar
- `Log Device Changes`: ✗ Desmarcado - Ativa logs de debug (opcional)

### 3. Configuração Opcional para Debug

Para visualizar informações de debug durante o desenvolvimento:

- Marque `Show Debug Info` na seção "Debug (Editor Only)"
- Isso mostrará uma janela com informações do dispositivo atual

## Funcionalidades

### Detecção Automática

- **Teclado**: Detectado quando qualquer tecla é pressionada
- **Xbox**: Controllers XInput e dispositivos com "xbox" no nome
- **PlayStation**: DualShock, DualSense e dispositivos com "playstation/ps4/ps5" no nome
- **Nintendo Switch**: Pro Controller e Joy-Cons
- **Gamepad Genérico**: Outros controles não identificados especificamente

### Integração com InputManager

O script se conecta automaticamente aos eventos do `InputManager` existente:

- Detecta quando o jogador usa navegação de UI
- Responde a inputs de gameplay
- Atualiza em tempo real durante mudanças de contexto

### API Pública

Métodos disponíveis para outros scripts:

```csharp
// Força uma atualização manual
inputDisplayController.ForceUpdateDisplay();

// Define manualmente um tipo de controle
inputDisplayController.SetControllerType(InputDisplayController.ControllerType.Xbox);

// Obtém o tipo atual
var currentType = inputDisplayController.GetCurrentControllerType();

// Verifica se um tipo específico está conectado
bool hasXbox = inputDisplayController.IsControllerTypeConnected(InputDisplayController.ControllerType.Xbox);

// Evento de mudança de controle
inputDisplayController.OnControllerTypeChanged += (type) => {
    Debug.Log($"Controle alterado para: {type}");
};
```

## Integração com Outros Scripts

### Exemplo de Uso em Menu

```csharp
public class MenuController : MonoBehaviour
{
    private InputDisplayController inputDisplay;
    
    void Start()
    {
        inputDisplay = FindObjectOfType<InputDisplayController>();
        if (inputDisplay != null)
        {
            inputDisplay.OnControllerTypeChanged += OnControllerChanged;
        }
    }
    
    private void OnControllerChanged(InputDisplayController.ControllerType type)
    {
        // Atualizar textos de botões baseado no tipo de controle
        UpdateButtonPrompts(type);
    }
}
```

## Troubleshooting

### Ícones não aparecem

- Verifique se as referências dos GameObjects estão configuradas
- Confirme que os objetos filhos existem na hierarquia
- Ative `Log Device Changes` para debug

### Detecção incorreta

- Alguns controles genéricos podem não ser detectados corretamente
- Use `SetControllerType()` para forçar um tipo específico se necessário
- Verifique se o Input System está configurado corretamente no projeto

### Performance

- O script é otimizado para verificações periódicas (0.3s por padrão)
- Ajuste `Update Interval` se necessário
- O script se integra aos eventos existentes para minimizar verificações desnecessárias

## Compatibilidade

- Unity 2022.3 LTS ou superior
- Input System Package
- Funciona em todas as plataformas suportadas pelo Unity Input System
