# Configuração do Sistema de Ícones

Este arquivo apresenta as instruções para configurar o Sistema de Ícone Superior conforme o Documento de Regras Técnicas.

## Passo a Passo

1. **Adicione o prefab IconManager à cena:**
   - Encontre o prefab em `Assets/Prefabs/UI/Icons/IconManager.prefab`
   - O prefab já vem configurado como um singleton
   - Pode ser adicionado à cena inicial ou a uma cena de persistência

2. **Configure o mapeamento de ícones:**
   - Use o menu `Extras > The Slime King > Ícones > Editor de Mapeamento` ou
   - Edite diretamente o ScriptableObject em `Assets/ScriptableObjects/InputIcons/DefaultInputIconMapping.asset`
   - Para cada ação, configure os sprites correspondentes para cada dispositivo

3. **Para objetos que precisam exibir ícones:**
   - Adicione o componente `InteractionIconTrigger` ao objeto
   - Vincule a InputAction relevante no campo `_actionReference`
   - Ajuste o `_detectionRadius` conforme necessário

## Integração com Input System

O sistema está configurado para trabalhar com o Unity Input System:

1. As InputActions são definidas em `Assets/InputSystem_Actions.inputactions`
2. O sistema detecta automaticamente o dispositivo atual (teclado, controle Xbox, PlayStation, etc)
3. Os ícones são atualizados dinamicamente conforme o dispositivo usado

## Considerações Importantes

- Certifique-se de que todos os sprites de ícones estejam configurados corretamente no ScriptableObject
- Teste a funcionalidade com diferentes tipos de controles
- O sistema tem suporte para fade in/out automático
- Para casos especiais, você pode chamar diretamente os métodos do IconManager
