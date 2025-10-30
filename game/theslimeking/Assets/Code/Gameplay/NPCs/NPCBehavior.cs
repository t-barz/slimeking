using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Gerencia o comportamento social e de combate do NPC.
    /// Define como o NPC reage ao jogador e a situações de combate.
    /// </summary>
    public class NPCBehavior : MonoBehaviour
    {
        [Header("Behavior Configuration")]
        [Tooltip("Tipo de comportamento social do NPC")]
        public BehaviorType behaviorType = BehaviorType.Neutro;

        [Tooltip("Alcance de detecção do jogador (metros)")]
        public float detectionRange = 5.0f;

        [Header("State")]
        [Tooltip("Indica se o NPC está atualmente fugindo")]
        private bool isFleeing = false;

        [Tooltip("Indica se o NPC está atualmente atacando")]
        private bool isAttacking = false;

        [Tooltip("Nível de agressão atual (usado para comportamento Agressivo)")]
        private float aggressionLevel = 0f;

        private NPCController npcController;
        private Animator animator;

        private void Awake()
        {
            npcController = GetComponent<NPCController>();
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Chamado quando o NPC recebe dano.
        /// Implementa lógica de comportamento específica baseada no BehaviorType.
        /// </summary>
        /// <param name="damage">Quantidade de dano recebido</param>
        public void OnTakeDamage(int damage)
        {
            // TODO: Integrar com sistema de combate quando implementado
            // int currentHP = combatSystem.GetCurrentHP();
            // int maxHP = npcController.GetMaxHP();

            switch (behaviorType)
            {
                case BehaviorType.Passivo:
                    HandlePassiveBehavior();
                    break;

                case BehaviorType.Neutro:
                    HandleNeutralBehavior();
                    break;

                case BehaviorType.Agressivo:
                    HandleAggressiveBehavior();
                    break;

                case BehaviorType.QuestGiver:
                    // Quest Givers não reagem a combate
                    Debug.Log($"💬 {gameObject.name} é um Quest Giver e não participa de combate.");
                    break;
            }
        }

        /// <summary>
        /// Lógica de comportamento para NPCs Passivos.
        /// Foge quando HP < 30%.
        /// </summary>
        private void HandlePassiveBehavior()
        {
            // TODO: Obter HP atual do sistema de combate
            // float hpPercentage = (float)currentHP / maxHP;
            // if (hpPercentage < 0.3f)
            // {
            //     StartFleeing();
            // }

            // Placeholder: sempre foge quando atacado
            StartFleeing();
            Debug.Log($"🏃 {gameObject.name} (Passivo) está fugindo!");
        }

        /// <summary>
        /// Lógica de comportamento para NPCs Neutros.
        /// Retalia quando atacado.
        /// </summary>
        private void HandleNeutralBehavior()
        {
            // TODO: Obter referência ao atacante do sistema de combate
            // GameObject attacker = combatSystem.GetLastAttacker();
            // SetTarget(attacker);

            StartAttacking();
            Debug.Log($"⚔️ {gameObject.name} (Neutro) está retalindo!");
        }

        /// <summary>
        /// Lógica de comportamento para NPCs Agressivos.
        /// Aumenta agressão e continua atacando.
        /// </summary>
        private void HandleAggressiveBehavior()
        {
            IncreaseAggression();
            
            if (!isAttacking)
            {
                StartAttacking();
            }

            Debug.Log($"😡 {gameObject.name} (Agressivo) aumentou agressão! Nível: {aggressionLevel}");
        }

        /// <summary>
        /// Inicia comportamento de fuga.
        /// NPC tenta se afastar do jogador/atacante.
        /// </summary>
        public void StartFleeing()
        {
            isFleeing = true;
            isAttacking = false;

            // TODO: Integrar com sistema de movimento/IA
            // aiController.SetState(AIState.Fleeing);
            // aiController.FleeFrom(attacker.transform.position);

            // Atualizar animação se disponível
            if (animator != null)
            {
                animator.SetFloat("Speed", 1.0f);
            }
        }

        /// <summary>
        /// Inicia comportamento de ataque.
        /// NPC persegue e ataca o alvo.
        /// </summary>
        public void StartAttacking()
        {
            isAttacking = true;
            isFleeing = false;

            // TODO: Integrar com sistema de combate
            // combatSystem.SetTarget(target);
            // combatSystem.StartAttack();

            // Atualizar animação se disponível
            if (animator != null)
            {
                animator.SetFloat("Speed", 1.0f);
            }
        }

        /// <summary>
        /// Aumenta o nível de agressão do NPC.
        /// Usado para NPCs Agressivos que ficam mais perigosos quando atacados.
        /// </summary>
        public void IncreaseAggression()
        {
            aggressionLevel += 1.0f;
            aggressionLevel = Mathf.Min(aggressionLevel, 10.0f); // Cap em 10

            // TODO: Aplicar modificadores baseados no nível de agressão
            // float damageMultiplier = 1.0f + (aggressionLevel * 0.1f);
            // combatSystem.SetDamageMultiplier(damageMultiplier);
        }

        /// <summary>
        /// Para comportamento de fuga.
        /// </summary>
        public void StopFleeing()
        {
            isFleeing = false;

            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
        }

        /// <summary>
        /// Para comportamento de ataque.
        /// </summary>
        public void StopAttacking()
        {
            isAttacking = false;

            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
        }

        /// <summary>
        /// Reseta o nível de agressão para 0.
        /// </summary>
        public void ResetAggression()
        {
            aggressionLevel = 0f;
        }

        /// <summary>
        /// Retorna se o NPC está atualmente fugindo.
        /// </summary>
        public bool IsFleeing()
        {
            return isFleeing;
        }

        /// <summary>
        /// Retorna se o NPC está atualmente atacando.
        /// </summary>
        public bool IsAttacking()
        {
            return isAttacking;
        }

        /// <summary>
        /// Retorna o nível de agressão atual.
        /// </summary>
        public float GetAggressionLevel()
        {
            return aggressionLevel;
        }
    }
}
