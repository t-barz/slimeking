# Configuração do Prefab de Portal

Para configurar corretamente os portais no jogo, siga estas instruções:

## Estrutura básica do prefab

1. GameObject pai: Portal
   - PortalController.cs
   - CircleCollider2D (como trigger)
   - SpriteRenderer (visual do portal)
   - Animator (opcional, para animação do portal)

2. GameObject filho: VFX
   - ParticleSystem (efeitos de partículas ativos enquanto o portal está disponível)

3. GameObject filho: PortalActivationVFX (opcional)
   - Prefab de efeito visual para quando o portal é ativado

## Configuração do componente PortalController

### Parâmetros gerais
- **Portal ID**: Identificador único para este portal (use nomes descritivos como "dungeon_entrance" ou "cave_exit")
- **Activation Type**: Escolha entre Touch (ativa automaticamente) ou Interaction (requer pressionar botão)

### Parâmetros de destino
- **Is Scene Portal**: Marque se este portal leva para outra cena
- **Target Scene Name**: Nome da cena de destino (apenas para portais entre cenas)
- **Target Portal ID**: ID do portal de destino na mesma cena ou na cena alvo
- **Target Position/Rotation**: Posição e rotação específicas caso não use um portal alvo

### Parâmetros visuais
- **Exit Offset**: Distância do ponto onde o jogador aparecerá ao sair do portal (ajuste para evitar colisões)
- **Portal Idle Effect**: Referência ao objeto de efeito visual constantemente ativo
- **Portal Activation Effect**: Prefab que será instanciado quando o portal for ativado

### Condições de acesso
- **Required Growth Stage**: Estágio mínimo que o slime precisa ter para usar o portal (0 = qualquer estágio)
- **Start Active**: Define se o portal começa ativo ou desativado

## Feedback visual
Para melhorar a percepção do usuário, dois prefabs de efeito são necessários:

1. **TeleportOutEffect**: Efeito visual que ocorre no portal de origem
2. **TeleportInEffect**: Efeito visual que ocorre no portal de destino

Estes efeitos devem ser referenciados no PortalManager.

## Efeitos sonoros recomendados

- **Teleport Sound**: Som 3D espacializado que toca na origem e no destino quando ocorre um teleporte
- **Looping Ambient**: Som ambiente em loop para o portal quando está ativo
- **Failure Sound**: Som quando o portal não pode ser usado (por requisitos não atendidos)

## Integração com outros sistemas

O PortalController já está integrado com:

1. **Sistema Interativo**: Herda de InteractableObject e usa o mecanismo padrão de interação
2. **Sistema de Crescimento**: Verifica requisitos de estágio antes de permitir teleporte
3. **Sistema de Ícones**: Mostra ícone de interação para portais do tipo Interaction

## Exemplo de configuração

- Portal simples na mesma cena:
  * Activation Type: Interaction
  * Is Scene Portal: False
  * Target Portal ID: "library_exit"
  * Required Growth Stage: 0

- Portal entre cenas:
  * Activation Type: Touch
  * Is Scene Portal: True
  * Target Scene Name: "Dungeon_01"
  * Target Portal ID: "dungeon_entrance"
  * Required Growth Stage: 2
