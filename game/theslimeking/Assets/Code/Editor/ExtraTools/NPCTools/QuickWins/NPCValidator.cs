using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SlimeMec.Gameplay;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Resultado de validação de configuração de NPC.
    /// Contém status de validação, erros e avisos.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Indica se a configuração é válida (sem erros).
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Lista de erros que impedem a aplicação da configuração.
        /// </summary>
        public List<string> Errors { get; private set; }

        /// <summary>
        /// Lista de avisos que não impedem a aplicação mas devem ser revisados.
        /// </summary>
        public List<string> Warnings { get; private set; }

        /// <summary>
        /// Construtor padrão que inicializa listas vazias.
        /// </summary>
        public ValidationResult()
        {
            IsValid = true;
            Errors = new List<string>();
            Warnings = new List<string>();
        }

        /// <summary>
        /// Adiciona um erro à lista e marca a validação como inválida.
        /// </summary>
        /// <param name="error">Mensagem de erro</param>
        public void AddError(string error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                Errors.Add(error);
                IsValid = false;
            }
        }

        /// <summary>
        /// Adiciona um aviso à lista (não afeta IsValid).
        /// </summary>
        /// <param name="warning">Mensagem de aviso</param>
        public void AddWarning(string warning)
        {
            if (!string.IsNullOrEmpty(warning))
            {
                Warnings.Add(warning);
            }
        }

        /// <summary>
        /// Retorna true se há erros.
        /// </summary>
        public bool HasErrors()
        {
            return Errors.Count > 0;
        }

        /// <summary>
        /// Retorna true se há avisos.
        /// </summary>
        public bool HasWarnings()
        {
            return Warnings.Count > 0;
        }

        /// <summary>
        /// Retorna uma string formatada com todos os erros e avisos.
        /// </summary>
        public string GetFormattedMessage()
        {
            string message = "";

            if (HasErrors())
            {
                message += "❌ ERROS:\n";
                foreach (string error in Errors)
                {
                    message += $"  • {error}\n";
                }
            }

            if (HasWarnings())
            {
                if (message.Length > 0) message += "\n";
                message += "⚠️ AVISOS:\n";
                foreach (string warning in Warnings)
                {
                    message += $"  • {warning}\n";
                }
            }

            if (!HasErrors() && !HasWarnings())
            {
                message = "✅ Configuração válida!";
            }

            return message;
        }
    }

    /// <summary>
    /// Validador de configurações de NPC para NPCQuickConfig.
    /// Verifica se a configuração é válida antes de aplicar.
    /// </summary>
    public static class NPCValidator
    {
        /// <summary>
        /// Valida uma configuração completa de NPC.
        /// Ponto de entrada principal para validação.
        /// </summary>
        /// <param name="config">Configuração a ser validada</param>
        /// <param name="targetObject">GameObject alvo (opcional)</param>
        /// <returns>Resultado da validação com erros e avisos</returns>
        public static ValidationResult ValidateConfiguration(NPCConfigData config, GameObject targetObject = null)
        {
            ValidationResult result = new ValidationResult();

            if (config == null)
            {
                result.AddError("Configuração é nula");
                return result;
            }

            // Validar GameObject
            if (targetObject != null)
            {
                if (!ValidateGameObject(targetObject, result))
                {
                    // Erros já adicionados ao result
                }
            }
            else
            {
                result.AddError("Nenhum GameObject selecionado. Selecione um GameObject na cena primeiro.");
            }

            // Validar nome do NPC
            if (string.IsNullOrWhiteSpace(config.npcName))
            {
                result.AddError("Nome do NPC não pode estar vazio");
            }

            // Validar Species Name se Friendship habilitado
            if (config.friendshipEnabled)
            {
                if (!ValidateSpeciesName(config.speciesName, result))
                {
                    // Erro já adicionado ao result
                }
            }

            // Validar Patrol Points se AI Type é Patrol
            if (config.aiType == AIType.Patrol)
            {
                if (!ValidatePatrolPoints(config.aiSettings.patrolPoints, result))
                {
                    // Aviso já adicionado ao result
                }
            }

            // Validar Detection Range para comportamentos que precisam
            if (config.behaviorType == BehaviorType.Neutro || config.behaviorType == BehaviorType.Agressivo)
            {
                if (config.detectionRange <= 0)
                {
                    result.AddWarning($"Detection Range é 0 para comportamento {config.behaviorType}. NPCs não detectarão o jogador.");
                }
            }

            // Validar Dialogue Settings se habilitado
            if (config.dialogueEnabled)
            {
                if (config.dialogueSettings.triggerType == DialogueTriggerType.Proximity)
                {
                    if (config.dialogueSettings.triggerRange <= 0)
                    {
                        result.AddWarning("Trigger Range de diálogo é 0. Diálogo por proximidade não funcionará.");
                    }
                }
            }

            // Validar Friendship Settings se habilitado
            if (config.friendshipEnabled)
            {
                if (config.friendshipSettings.initialLevel > config.friendshipSettings.maxLevel)
                {
                    result.AddError($"Nível inicial de amizade ({config.friendshipSettings.initialLevel}) não pode ser maior que o nível máximo ({config.friendshipSettings.maxLevel})");
                }

                if (config.friendshipSettings.maxLevel < 1)
                {
                    result.AddError("Nível máximo de amizade deve ser pelo menos 1");
                }
            }

            // Validar AI Settings
            if (config.aiType == AIType.Wander)
            {
                if (config.aiSettings.wanderRadius <= 0)
                {
                    result.AddWarning("Wander Radius é 0 ou negativo. NPC não se moverá.");
                }

                if (config.aiSettings.wanderSpeed <= 0)
                {
                    result.AddWarning("Wander Speed é 0 ou negativo. NPC não se moverá.");
                }
            }

            if (config.aiType == AIType.Patrol)
            {
                if (config.aiSettings.patrolSpeed <= 0)
                {
                    result.AddWarning("Patrol Speed é 0 ou negativo. NPC não se moverá.");
                }
            }

            return result;
        }

        /// <summary>
        /// Valida se o GameObject é válido para configuração.
        /// </summary>
        /// <param name="target">GameObject a ser validado</param>
        /// <param name="result">Resultado onde erros serão adicionados</param>
        /// <returns>True se válido, false caso contrário</returns>
        public static bool ValidateGameObject(GameObject target, ValidationResult result)
        {
            if (target == null)
            {
                result.AddError("GameObject é nulo");
                return false;
            }

            // Verificar se é um prefab
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(target);
            PrefabInstanceStatus prefabStatus = PrefabUtility.GetPrefabInstanceStatus(target);

            if (prefabType != PrefabAssetType.NotAPrefab && prefabStatus == PrefabInstanceStatus.NotAPrefab)
            {
                result.AddError("Não é possível modificar prefab diretamente. Desempacote o prefab primeiro (GameObject > Unpack Prefab).");
                return false;
            }

            // Verificar se pode adicionar componentes
            if (!ValidateComponentDependencies(target, result))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida se o nome da espécie é válido quando friendship está habilitado.
        /// </summary>
        /// <param name="speciesName">Nome da espécie</param>
        /// <param name="result">Resultado onde erros serão adicionados</param>
        /// <returns>True se válido, false caso contrário</returns>
        public static bool ValidateSpeciesName(string speciesName, ValidationResult result)
        {
            if (string.IsNullOrWhiteSpace(speciesName))
            {
                result.AddError("Species Name é obrigatório quando Friendship System está habilitado");
                return false;
            }

            // Verificar se contém caracteres inválidos para nomes de arquivo
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                if (speciesName.Contains(c.ToString()))
                {
                    result.AddError($"Species Name contém caractere inválido: '{c}'");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Valida se os pontos de patrulha são válidos.
        /// </summary>
        /// <param name="patrolPoints">Lista de pontos de patrulha</param>
        /// <param name="result">Resultado onde avisos serão adicionados</param>
        /// <returns>True se válido, false se precisa auto-gerar</returns>
        public static bool ValidatePatrolPoints(List<Vector2> patrolPoints, ValidationResult result)
        {
            if (patrolPoints == null || patrolPoints.Count < 2)
            {
                result.AddWarning("Patrol requer pelo menos 2 pontos. 4 pontos serão gerados automaticamente em quadrado (raio 3 unidades).");
                return false;
            }

            // Verificar se há pontos duplicados
            for (int i = 0; i < patrolPoints.Count; i++)
            {
                for (int j = i + 1; j < patrolPoints.Count; j++)
                {
                    if (Vector2.Distance(patrolPoints[i], patrolPoints[j]) < 0.1f)
                    {
                        result.AddWarning($"Pontos de patrulha {i} e {j} estão muito próximos (< 0.1 unidades)");
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Valida se os componentes necessários podem ser adicionados ao GameObject.
        /// </summary>
        /// <param name="target">GameObject alvo</param>
        /// <param name="result">Resultado onde erros serão adicionados</param>
        /// <returns>True se componentes podem ser adicionados, false caso contrário</returns>
        public static bool ValidateComponentDependencies(GameObject target, ValidationResult result)
        {
            if (target == null)
            {
                result.AddError("GameObject é nulo");
                return false;
            }

            // Verificar se o GameObject está ativo na hierarquia
            // (componentes podem ser adicionados mesmo se inativo, mas é bom avisar)
            if (!target.activeInHierarchy)
            {
                result.AddWarning("GameObject está inativo na hierarquia. Componentes serão adicionados mas podem não funcionar até ser ativado.");
            }

            // Verificar se há conflitos de componentes
            // Por exemplo, múltiplos Rigidbody2D não são permitidos
            Rigidbody2D[] rigidbodies = target.GetComponents<Rigidbody2D>();
            if (rigidbodies.Length > 1)
            {
                result.AddWarning("GameObject possui múltiplos Rigidbody2D. Apenas um será mantido.");
            }

            // Verificar se há múltiplos colliders (permitido, mas pode ser não intencional)
            Collider2D[] colliders = target.GetComponents<Collider2D>();
            if (colliders.Length > 1)
            {
                result.AddWarning($"GameObject possui {colliders.Length} colliders. Colliders existentes serão removidos e substituídos por CircleCollider2D.");
            }

            return true;
        }

        /// <summary>
        /// Retorna lista de avisos para uma configuração.
        /// </summary>
        /// <param name="config">Configuração a ser analisada</param>
        /// <returns>Lista de avisos</returns>
        public static List<string> GetWarnings(NPCConfigData config)
        {
            ValidationResult result = ValidateConfiguration(config, null);
            return result.Warnings;
        }

        /// <summary>
        /// Retorna lista de erros para uma configuração.
        /// </summary>
        /// <param name="config">Configuração a ser analisada</param>
        /// <returns>Lista de erros</returns>
        public static List<string> GetErrors(NPCConfigData config)
        {
            ValidationResult result = ValidateConfiguration(config, null);
            return result.Errors;
        }
    }
}
