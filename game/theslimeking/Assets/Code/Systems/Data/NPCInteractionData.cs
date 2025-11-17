using UnityEngine;

namespace SlimeMec.Systems.Data
{
    /// <summary>
    /// Define os tipos de interação disponíveis para NPCs.
    /// </summary>
    public enum InteractionType
    {
        /// <summary>
        /// Inicia um diálogo com o NPC.
        /// </summary>
        Dialogue,

        /// <summary>
        /// Permite entregar itens ao NPC.
        /// </summary>
        ItemDelivery,

        /// <summary>
        /// Ativa uma nova quest.
        /// </summary>
        QuestActivation,

        /// <summary>
        /// Completa uma quest existente.
        /// </summary>
        QuestCompletion,

        /// <summary>
        /// Abre uma loja para compra/venda de itens.
        /// </summary>
        Shop
    }

    /// <summary>
    /// ScriptableObject que define uma interação disponível para um NPC.
    /// Contém informações sobre o tipo de interação, requisitos e dados associados.
    /// </summary>
    [CreateAssetMenu(fileName = "NPCInteractionData", menuName = "Game/NPC/Interaction Data")]
    public class NPCInteractionData : ScriptableObject
    {
        [Header("Interaction Configuration")]
        [Tooltip("Tipo de interação que este NPC oferece")]
        public InteractionType interactionType;

        [Tooltip("Nome descritivo da interação")]
        public string interactionName;

        [Tooltip("Descrição da interação exibida ao jogador")]
        [TextArea(2, 4)]
        public string interactionDescription;

        [Header("Requirements")]
        [Tooltip("Nível mínimo de relacionamento necessário para esta interação (-100 a 100)")]
        [Range(-100, 100)]
        public int requiredRelationshipPoints = 0;

        [Tooltip("Se true, esta interação só pode ser usada uma vez")]
        public bool oneTimeOnly = false;

        [Header("Interaction Data")]
        [Tooltip("ID do diálogo a ser iniciado (para InteractionType.Dialogue)")]
        public string dialogueID;

        [Tooltip("ID da quest a ser ativada/completada (para QuestActivation/QuestCompletion)")]
        public string questID;

        [Tooltip("IDs dos itens necessários para entrega (para ItemDelivery)")]
        public string[] requiredItemIDs;

        [Tooltip("ID da loja a ser aberta (para InteractionType.Shop)")]
        public string shopID;

        [Header("Feedback")]
        [Tooltip("Mensagem exibida quando a interação é bem-sucedida")]
        public string successMessage;

        [Tooltip("Mensagem exibida quando os requisitos não são atendidos")]
        public string failureMessage;

        [Header("State")]
        [Tooltip("Indica se esta interação já foi consumida (usado internamente)")]
        [SerializeField]
        private bool consumed = false;

        /// <summary>
        /// Valida os dados da interação no Inspector.
        /// </summary>
        private void OnValidate()
        {
            // Validar se há dados necessários baseado no tipo de interação
            switch (interactionType)
            {
                case InteractionType.Dialogue:
                    if (string.IsNullOrEmpty(dialogueID))
                    {
                        Debug.LogWarning($"[NPCInteractionData] '{name}' do tipo Dialogue não possui dialogueID atribuído!", this);
                    }
                    break;

                case InteractionType.QuestActivation:
                case InteractionType.QuestCompletion:
                    if (string.IsNullOrEmpty(questID))
                    {
                        Debug.LogWarning($"[NPCInteractionData] '{name}' do tipo {interactionType} não possui questID atribuído!", this);
                    }
                    break;

                case InteractionType.ItemDelivery:
                    if (requiredItemIDs == null || requiredItemIDs.Length == 0)
                    {
                        Debug.LogWarning($"[NPCInteractionData] '{name}' do tipo ItemDelivery não possui requiredItemIDs atribuídos!", this);
                    }
                    break;

                case InteractionType.Shop:
                    if (string.IsNullOrEmpty(shopID))
                    {
                        Debug.LogWarning($"[NPCInteractionData] '{name}' do tipo Shop não possui shopID atribuído!", this);
                    }
                    break;
            }

            // Atualizar interactionName se estiver vazio
            if (string.IsNullOrEmpty(interactionName))
            {
                interactionName = $"{interactionType} Interaction";
            }
        }

        /// <summary>
        /// Verifica se a interação pode ser usada baseado no estado consumed.
        /// </summary>
        public bool CanBeUsed()
        {
            if (oneTimeOnly && consumed)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Marca a interação como consumida.
        /// </summary>
        public void MarkAsConsumed()
        {
            if (oneTimeOnly)
            {
                consumed = true;
            }
        }

        /// <summary>
        /// Reseta o estado consumed (útil para testes ou reset de save).
        /// </summary>
        public void ResetConsumption()
        {
            consumed = false;
        }

        /// <summary>
        /// Verifica se o jogador atende aos requisitos de relacionamento.
        /// </summary>
        public bool MeetsRelationshipRequirement(int currentRelationshipPoints)
        {
            return currentRelationshipPoints >= requiredRelationshipPoints;
        }
    }
}
