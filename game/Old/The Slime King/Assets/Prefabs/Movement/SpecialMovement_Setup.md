# Guia de Configuração - Sistema de Movimento Especial

Este guia explica como configurar e utilizar o Sistema de Movimento Especial do jogo "The Slime King". O sistema consiste em dois tipos principais de movimento especial:

1. **Encolher e Esgueirar** - Permite que o jogador encolha e atravesse passagens estreitas
2. **Sistema de Pulo** - Permite que o jogador realize saltos em arco para alcançar áreas elevadas ou atravessar obstáculos

## Requisitos de Sistema

- Unity 6+ com URP 2D
- Input System Package instalado
- Jogador (Slime) com componente `SlimeMovement` implementando a interface `IPlayerController`

## 1. Configuração do Encolher e Esgueirar

### Configuração do Prefab

1. Crie um GameObject vazio para representar o ponto de entrada
2. Adicione o script `EncolherEsgueirarController` ao GameObject
3. Crie outro GameObject vazio para representar o ponto de saída
4. Arraste o GameObject de saída para o campo `Exit Point` no inspector

### Parâmetros Principais

- **Movement Duration**: Duração da animação de esgueirar (segundos)
- **Shrink Scale**: Escala para o jogador durante o movimento (0.5 = metade do tamanho)
- **Movement Curve**: Curva de animação para o movimento (easing)
- **Exit Point**: Ponto onde o jogador sairá após esgueirar

### Configuração de Feedback

- **Shrink Effect**: Efeito de partículas quando o jogador encolhe
- **Expand Effect**: Efeito de partículas quando o jogador expande
- **Shrink Sound**: Som ao encolher
- **Expand Sound**: Som ao expandir
- **Interaction Icon**: Ícone visual para indicar possibilidade de interação

## 2. Configuração do Sistema de Pulo

### Configuração do Prefab

1. Crie um GameObject vazio para representar o ponto de decolagem
2. Adicione o script `JumpController` ao GameObject
3. Crie outro GameObject vazio para representar o ponto de pouso
4. Arraste o GameObject de pouso para o campo `Landing Point` no inspector

### Parâmetros Principais

- **Jump Height**: Altura máxima do arco de salto
- **Jump Duration**: Duração completa do salto (segundos)
- **Jump Curve**: Curva de animação para movimento horizontal
- **Height Curve**: Curva para o arco vertical do salto (parabólica)
- **Landing Point**: Ponto onde o jogador pousará após o salto
- **Obstacle Layer Mask**: Camadas que bloqueiam o salto

### Configuração de Feedback

- **Takeoff Effect**: Efeito de partículas na decolagem
- **Landing Effect**: Efeito de partículas no pouso
- **Takeoff Sound**: Som na decolagem
- **Landing Sound**: Som no pouso
- **Interaction Icon**: Ícone visual para indicar possibilidade de pulo

## Integração com Player Controller

Para que o sistema funcione corretamente, o script `PlayerController` deve implementar a interface `IPlayerController`.

```csharp
public class YourPlayerController : MonoBehaviour, TheSlimeKing.Gameplay.IPlayerController
{
    // Implementar os métodos da interface IPlayerController:
    public void DisableControl() { /* ... */ }
    public void EnableControl() { /* ... */ }
    public void MoveToPosition(Vector2 position, bool immediate = false) { /* ... */ }
    public void SetScale(float scale) { /* ... */ }
    public void SetDirection(Vector2 direction) { /* ... */ }
    public Vector2 GetPosition() { /* ... */ }
    public Vector2 GetDirection() { /* ... */ }
}
```

## Dicas para Level Designers

### Para Pontos de Esgueirar (EncolherEsgueirarController)

- Posicione o ponto de entrada no início da passagem estreita
- O ponto de saída deve estar no final da passagem
- Ajuste a duração do movimento para corresponder à distância (mais longe = mais tempo)
- Teste com diferentes valores de shrink scale para o efeito visual desejado

### Para Pontos de Pulo (JumpController)

- Posicione o ponto de decolagem onde o jogador deve iniciar o salto
- O ponto de pouso deve estar onde o jogador aterrissará
- Ajuste a altura e duração do pulo para criar o arco desejado
- Certifique-se de que a trajetória está livre de obstáculos
- Use a visualização de gizmos no editor para verificar o arco de pulo

## Resolução de Problemas Comuns

1. **O jogador não está interagindo com o ponto**
   - Verifique se o jogador implementa corretamente a interface `IPlayerController`
   - Certifique-se de que a distância de interação está adequada

2. **Animação de movimento não está fluida**
   - Ajuste as curvas de animação para suavizar o movimento
   - Aumente a duração do movimento para torná-lo mais gradual

3. **Jogador fica preso durante o movimento**
   - Verifique se não há colisores bloqueando o caminho
   - Ajuste a escala de encolhimento para ser menor se necessário

4. **Salto está sendo bloqueado**
   - Certifique-se de que a layer mask de obstáculos está configurada corretamente
   - Ajuste a altura do pulo para evitar colisões
