# Configuração do Sistema de Stealth

Este documento descreve como configurar e utilizar o Sistema de Stealth no jogo The Slime King.

## Visão Geral

O Sistema de Stealth permite que o jogador se esconda utilizando objetos de cobertura no ambiente, tornando-se temporariamente indetectável por inimigos.

## Componentes do Sistema

O sistema é composto pelos seguintes scripts:

1. `StealthController.cs` - Gerencia o estado de stealth do jogador
2. `CoverObject.cs` - Define objetos que funcionam como cobertura
3. `StealthState.cs` - Enumera os possíveis estados de stealth
4. `CoverType.cs` - Define tipos de objetos de cobertura
5. `StealthIcon.cs` - Controla o ícone visual que indica quando o personagem está escondido
6. `StealthEffect.cs` - Aplica efeito visual de vinheta quando escondido

## Configuração

### 1. Configurando o Player

Adicione o componente `StealthController` ao GameObject do jogador. Este componente requer:

- Referência para o `SlimeMovement`
- Referência para o `SlimeVisualController`
- Referência para o `SlimeAnimationController`
- Opcionalmente, prefabs para os efeitos visuais de stealth e ícone

### 2. Criando Objetos de Cobertura

Para criar um objeto que funcione como cobertura:

1. Crie um GameObject com um Collider2D (como Trigger)
2. Adicione o componente `CoverObject`
3. Defina o tipo de cobertura (Grass, Bush, Rock, Tree)
4. Certifique-se que a tag do GameObject corresponda ao tipo

### 3. Layer para Detecção

Configure uma Layer específica para objetos de cobertura e defina-a no `StealthController`.

## Estados de Visibilidade

O sistema implementa quatro estados de visibilidade:

1. **Normal** - Estado padrão, jogador completamente visível
2. **Crouched** - Jogador agachado (pressionar Q), movimento bloqueado
3. **Hidden** - Jogador agachado em objeto de cobertura, indetectável
4. **Exposed** - Jogador agachado sem cobertura, ainda detectável

## Exemplo de Uso

```csharp
// Verificar se o jogador está escondido
StealthController stealthController = GetComponent<StealthController>();
if (stealthController.IsHidden())
{
    // Jogador está escondido, ignorar detecção
}

// Verificar se o jogador está detectável
if (stealthController.IsDetectable())
{
    // Jogador está visível para inimigos
}
```

## Feedback Visual

- Quando o jogador está escondido, uma vinheta escura aparece na tela
- Um ícone de "olho riscado" aparece acima do personagem
- O personagem fica ligeiramente transparente quando escondido

## Observações

- Durante qualquer estado que não seja Normal, o movimento do jogador é bloqueado
- Utilize o método `OnCrouch(bool)` do StealthController para integrar com o sistema de input
- Para testes, utilize o script `StealthTester`, que cria objetos de cobertura automáticos
