namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Define as categorias de NPCs no jogo.
    /// Determina o comportamento geral e integração com Behavior Graph.
    /// </summary>
    public enum NPCCategory
    {
        /// <summary>
        /// NPC inimigo que ataca o jogador.
        /// Comportamento hostil e agressivo.
        /// </summary>
        Enemy,

        /// <summary>
        /// NPC amigável que não ataca o jogador.
        /// Pode oferecer diálogos, quests e comércio.
        /// </summary>
        Friendly,

        /// <summary>
        /// NPC neutro que não ataca a menos que provocado.
        /// Pode se tornar hostil ou amigável baseado em ações do jogador.
        /// </summary>
        Neutral,

        /// <summary>
        /// NPC chefe com comportamento especial.
        /// Usa Behavior Graph complexo e possui atributos elevados.
        /// </summary>
        Boss
    }

    /// <summary>
    /// Define os padrões de movimentação disponíveis para NPCs.
    /// Controla como o NPC se move pelo mundo.
    /// </summary>
    public enum MovementPattern
    {
        /// <summary>
        /// NPC permanece parado no local inicial.
        /// Sem movimento automático.
        /// </summary>
        Idle,

        /// <summary>
        /// NPC patrulha entre pontos fixos definidos.
        /// Move-se sequencialmente através de uma lista de waypoints.
        /// </summary>
        PatrolPoints,

        /// <summary>
        /// NPC patrulha em uma área circular.
        /// Move-se aleatoriamente dentro de um raio definido.
        /// </summary>
        CircularPatrol,

        /// <summary>
        /// NPC persegue um alvo específico.
        /// Usado durante combate ou quando detecta o jogador.
        /// </summary>
        ChaseTarget
    }

    /// <summary>
    /// Define os estados possíveis de um NPC.
    /// Usado para controlar comportamento e animações.
    /// </summary>
    public enum NPCState
    {
        /// <summary>
        /// NPC está parado e inativo.
        /// Estado inicial ou quando não há ações a executar.
        /// </summary>
        Idle,

        /// <summary>
        /// NPC está patrulhando sua área.
        /// Movendo-se entre pontos ou em círculo.
        /// </summary>
        Patrolling,

        /// <summary>
        /// NPC está perseguindo um alvo.
        /// Geralmente o jogador após detecção.
        /// </summary>
        Chasing,

        /// <summary>
        /// NPC está atacando um alvo.
        /// Executando animação e aplicando dano.
        /// </summary>
        Attacking,

        /// <summary>
        /// NPC está interagindo com o jogador.
        /// Diálogo, comércio ou entrega de quest.
        /// </summary>
        Interacting,

        /// <summary>
        /// NPC está morto.
        /// Não executa mais ações e aguarda destruição.
        /// </summary>
        Dead
    }
}
