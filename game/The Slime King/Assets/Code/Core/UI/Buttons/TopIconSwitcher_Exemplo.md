# Exemplo de Configuração do Sistema de Ícone Superior

Para facilitar a implementação do sistema, aqui está um exemplo passo a passo:

## 1. Criando a estrutura básica no Unity Editor

1. Crie um GameObject vazio chamado "IconTrigger"
2. Adicione um BoxCollider2D (ou outro tipo de Collider2D) ao GameObject
3. Configure o Collider2D com `isTrigger = true`
4. Adicione o script `TopIconSwitcher` ao GameObject

## 2. Configurando os ícones por plataforma

1. Crie cinco sub-GameObjects dentro do GameObject principal "IconTrigger":
   - keyboard
   - xbox
   - playstation
   - switch
   - generic

2. Em cada sub-GameObject, adicione:
   - Um SpriteRenderer com o sprite desejado para o ícone
   - Configure o sprite, tamanho e outras propriedades visuais conforme necessário

## 3. Configurando o script TopIconSwitcher

No Inspector do componente TopIconSwitcher:

1. Arraste cada sub-GameObject para o campo correspondente:
   - _keyboardIcon: arraste o GameObject "keyboard"
   - _xboxIcon: arraste o GameObject "xbox"
   - _playstationIcon: arraste o GameObject "playstation"
   - _switchIcon: arraste o GameObject "switch"
   - _genericIcon: arraste o GameObject "generic"

2. Configure o offset de posição se necessário (ex: Vector2(0, 0.5f) para posicionar o ícone ligeiramente acima)
3. Defina a duração do fade e ajuste a curva de animação conforme necessário

## 4. Testando a configuração

1. Entre em Play Mode no Unity
2. Verifique se o ícone correto é exibido quando o jogador entra no alcance do trigger
3. Teste a troca de dispositivo de entrada para ver se o ícone muda automaticamente
4. Confirme que o efeito de fade-in e fade-out está funcionando corretamente

## 5. Exemplo de código para controlar o sistema

```csharp
// Exemplo de como usar o TopIconSwitcher em outro script

using TheSlimeKing.Core.UI.Icons;
using UnityEngine;

public class IconTriggerExample : MonoBehaviour
{
    [SerializeField] private TopIconSwitcher _iconSwitcher;
    
    // Exemplo de como mostrar o ícone manualmente
    public void ShowIconManually()
    {
        _iconSwitcher.ShowIcon();
    }
      // Exemplo de como esconder o ícone manualmente
    public void HideIconManually()
    {
        _iconSwitcher.HideIcon();
    }
    
    // Exemplo de como ajustar a posição do ícone durante o jogo
    public void AjustarPosicaoIcone()
    {
        // Desloca o ícone 1 unidade para cima em relação ao objeto pai
        _iconSwitcher.SetIconOffset(new Vector2(0, 1f));
    }
}
```

## 6. Dicas de Performance

- Os ícones são automaticamente desativados quando não estão em uso para economizar recursos
- A detecção de plataforma é otimizada para verificar primeiro os dispositivos mais prováveis
- O sistema é projetado para atualizar apenas o ícone necessário, mantendo os outros desativados
