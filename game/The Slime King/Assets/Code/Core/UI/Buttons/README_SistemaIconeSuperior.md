# Sistema de Ícone Superior

Este sistema gerencia a exibição de ícones de interação adaptados a diferentes dispositivos de entrada (teclado, Xbox, PlayStation, Switch e genérico), com efeitos de fade-in e fade-out automáticos.

## Funcionalidades

- **Estrutura simplificada**: Todos os ícones são subobjetos diretos do GameObject principal.
- **Detecção automática de plataforma**: Detecta automaticamente qual dispositivo de entrada está sendo usado e exibe o ícone correspondente.
- **Troca dinâmica de ícone**: Se o jogador trocar de dispositivo durante o jogo, o ícone é atualizado automaticamente.
- **Detecção de player por trigger**: Utiliza OnTriggerEnter2D/Exit2D para detectar quando o jogador está no alcance.
- **Efeito de fade**: Fade-in ao exibir o ícone e fade-out ao ocultá-lo, com duração configurável.
- **Curva de animação personalizável**: Controle total sobre a progressão da animação de fade.
- **Posicionamento flexível**: Suporte para offset de posição dos ícones em relação ao objeto pai.

## Como Configurar o Prefab

1. Crie um GameObject com um Collider2D (marque "Is Trigger" como true)
2. Adicione o script `TopIconSwitcher` ao GameObject
3. Crie sub-GameObjects para cada plataforma dentro do GameObject principal:
   - keyboard
   - xbox
   - playstation
   - switch
   - generic
4. Adicione um SpriteRenderer a cada sub-GameObject
5. Configure os campos no Inspector do script TopIconSwitcher:
   - Arraste cada sub-GameObject para o campo correspondente
   - Configure o offset de posição desejado (Vector2)
   - Ajuste a duração do fade e a curva de animação conforme necessário

## Configuração Recomendada

```
IconTrigger (GameObject com BoxCollider2D isTrigger=true)
├── TopIconSwitcher (Script)
├── keyboard (GameObject com SpriteRenderer)
├── xbox (GameObject com SpriteRenderer)
├── playstation (GameObject com SpriteRenderer)
├── switch (GameObject com SpriteRenderer)
└── generic (GameObject com SpriteRenderer)
```

## Funcionamento

1. No Awake, todos os ícones são desativados (SetActive(false))
2. Quando o jogador entra no alcance (OnTriggerEnter2D), o script:
   - Identifica qual plataforma está sendo utilizada
   - Ativa apenas o ícone correspondente à plataforma atual
3. O ícone é exibido com efeito de fade-in (alpha 0→1)
4. Se o dispositivo de entrada mudar enquanto o jogador estiver no alcance, o script troca automaticamente para o ícone da nova plataforma
5. Quando o jogador sai do alcance (OnTriggerExit2D), o ícone faz fade-out (alpha 1→0) e é desativado

## Exemplo de Uso Avançado

```csharp
// Obter o script em outro objeto
TopIconSwitcher iconSwitcher = GetComponent<TopIconSwitcher>();

// Forçar a exibição do ícone (com fade-in)
iconSwitcher.ShowIcon();

// Forçar a ocultação do ícone (com fade-out)
iconSwitcher.HideIcon();

// Ajustar a posição do ícone em tempo de execução
iconSwitcher.SetIconOffset(new Vector2(0, 1.5f)); // Desloca o ícone 1.5 unidades para cima
```
