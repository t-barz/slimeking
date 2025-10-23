# Sistema de Fade para Botões de Interação

## Resumo das Mudanças

O sistema de exibição de botões de interação no `InteractivePointHandler.cs` foi atualizado para usar animações de fade suaves ao invés de aparecimento/desaparecimento instantâneo.

## Funcionalidades Implementadas

### 1. **Sistema de Fade Suave**

- **Fade In**: Transição suave de transparente (alpha 0) para opaco (alpha 1)
- **Fade Out**: Transição suave de opaco para transparente
- **Transição Direta**: Fade out de um botão seguido de fade in de outro

### 2. **Configuração Personalizável**

- **`fadeDuration`**: Duração da animação em segundos (padrão: 0.3s)
- **`fadeCurve`**: Curva de animação personalizada (padrão: EaseInOut)

### 3. **Métodos de Fade**

#### `FadeIn(Transform buttonTransform)`

- Faz fade in de um botão específico
- Ativa o `SpriteRenderer` e anima o alpha de 0 para 1
- Usa `AnimationCurve` para suavização

#### `FadeOut(Transform buttonTransform)`

- Faz fade out de um botão específico  
- Anima o alpha de valor atual para 0
- Desativa o `SpriteRenderer` no final

#### `FadeOutThenIn(Transform outButton, Transform inButton)`

- Transição sequencial entre botões
- Usado quando o tipo de input muda

### 4. **Gerenciamento de Estado**

- **`_currentFadeCoroutine`**: Controla apenas uma animação por vez
- **`_currentActiveButton`**: Rastreia qual botão está ativo
- **`_isVisible`**: Estado de visibilidade dos botões

## Melhorias de UX

### Antes

- Botões apareciam/desapareciam instantaneamente
- Transições bruscas entre tipos de input
- Experiência visual rígida

### Depois

- Transições suaves e naturais
- Animações configuráveis via Inspector
- Melhor feedback visual para o jogador
- Cancela animações em progresso para evitar conflitos

## Configuração no Unity

1. **Fade Duration**: Ajustar tempo de animação
2. **Fade Curve**: Personalizar suavização (Linear, EaseIn, EaseOut, etc.)
3. **Funciona automaticamente** com a detecção de input existente

## Compatibilidade

- ✅ Mantém toda funcionalidade existente
- ✅ Funciona com todos os tipos de input (Keyboard, PlayStation, Xbox, etc.)
- ✅ Compatível com sistema de outline existente
- ✅ Não afeta performance significativamente

## Estrutura de Arquivo

```
InteractivePointHandler.cs
├── Fade Animation Settings (Inspector)
├── Private Fields (estado do fade)
├── Fade Animation Methods
│   ├── FadeIn()
│   ├── FadeOut()
│   └── FadeOutThenIn()
├── ShowInteractionButtons() - MODIFICADO
└── HideAllButtons() - MODIFICADO
```

O sistema mantém a arquitetura original e adiciona as funcionalidades de fade de forma não-destrutiva.
