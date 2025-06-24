# Prefab TopIconSwitcher - README

## Configuração do Sistema de Ícone Superior

Este é o prefab de exemplo para o novo sistema de ícones adaptável para diferentes plataformas de controle.

### Como usar este prefab

1. Crie um objeto vazio na cena para representar o ponto de interação
2. Adicione o componente `TopIconSwitcher`
3. Adicione um `Collider2D` com `isTrigger = true` para detectar a entrada do jogador
4. Crie 5 subobjetos filhos, um para cada tipo de plataforma:
   - KeyboardIcon
   - XboxIcon
   - PlayStationIcon
   - SwitchIcon
   - GenericIcon
5. Configure cada ícone com um `CanvasGroup` ou `SpriteRenderer`

### Hierarquia recomendada

```
InteractionPoint
├── KeyboardIcon (GameObject com SpriteRenderer/Image)
├── XboxIcon (GameObject com SpriteRenderer/Image)
├── PlayStationIcon (GameObject com SpriteRenderer/Image)
├── SwitchIcon (GameObject com SpriteRenderer/Image)
└── GenericIcon (GameObject com SpriteRenderer/Image)
```

### Funcionamento

- Quando o jogador entra no `Collider2D`, o ícone correspondente à plataforma atual é exibido com fade-in
- Quando o jogador sai do `Collider2D`, o ícone realiza fade-out
- Se o jogador trocar de dispositivo enquanto estiver no alcance, o ícone é atualizado automaticamente
- Os ícones não utilizados ficam desativados para economizar recursos

### Personalização

- Ajuste a duração do fade no campo `_fadeDuration`
- Configure a curva de animação com `_fadeCurve`
