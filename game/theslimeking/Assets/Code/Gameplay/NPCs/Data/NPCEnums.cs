namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Define os tipos de comportamento social dos NPCs.
    /// Determina como o NPC reage ao jogador e a situações de combate.
    /// </summary>
    public enum BehaviorType
    {
        /// <summary>
        /// NPC passivo que não ataca o jogador.
        /// Foge quando HP < 30%.
        /// </summary>
        Passivo,

        /// <summary>
        /// NPC neutro que ignora o jogador até ser atacado.
        /// Retalia quando atacado.
        /// </summary>
        Neutro,

        /// <summary>
        /// NPC agressivo que persegue e ataca o jogador.
        /// Ataca quando jogador entra no Detection Range.
        /// </summary>
        Agressivo,

        /// <summary>
        /// NPC que oferece quests ao jogador.
        /// Geralmente estático com diálogo habilitado.
        /// </summary>
        QuestGiver
    }

    /// <summary>
    /// Define os tipos de IA de movimento dos NPCs.
    /// Controla como o NPC se move pelo mundo.
    /// </summary>
    public enum AIType
    {
        /// <summary>
        /// NPC permanece parado no local inicial.
        /// Sem movimento automático.
        /// </summary>
        Static,

        /// <summary>
        /// NPC vaga aleatoriamente dentro de um raio.
        /// Move-se para pontos aleatórios, pausa, e repete.
        /// </summary>
        Wander,

        /// <summary>
        /// NPC patrulha entre pontos fixos.
        /// Move-se em sequência através de uma lista de pontos.
        /// </summary>
        Patrol
    }

    /// <summary>
    /// Define como o diálogo é acionado pelo jogador.
    /// Controla a interação entre jogador e NPC.
    /// </summary>
    public enum DialogueTriggerType
    {
        /// <summary>
        /// Diálogo é acionado automaticamente quando jogador se aproxima.
        /// Usa Trigger Range para detecção.
        /// </summary>
        Proximity,

        /// <summary>
        /// Diálogo é acionado apenas quando jogador pressiona botão de interação.
        /// Requer input explícito do jogador.
        /// </summary>
        Interaction
    }
}
