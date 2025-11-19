using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;

/// <summary>
/// Sistema auxiliar para configuração automática de animações em NPCs.
/// Busca e associa clipes de animação automaticamente baseado em convenções de nome.
/// </summary>
namespace ExtraTools.Editor
{
    public static class NPCAnimationHelper
    {
        [UnityEditor.MenuItem("Extra Tools/NPC/Auto-Assign Animations")]
        public static void AutoAssignAnimations()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("[NPCAnimationHelper] Nenhum GameObject selecionado!");
                return;
            }

            Animator animator = selected.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning($"[NPCAnimationHelper] GameObject '{selected.name}' não possui componente Animator!");
                return;
            }

            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null)
            {
                Debug.LogWarning($"[NPCAnimationHelper] GameObject '{selected.name}' não possui AnimatorController configurado!");
                return;
            }

            // Busca animações baseadas no nome do NPC
            string npcName = ExtractNPCName(selected.name);
            AssignAnimationsToStates(controller, npcName);
        }

        [UnityEditor.MenuItem("Extra Tools/NPC/Create Animation Clips")]
        public static void CreateAnimationClipsForNPC()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("[NPCAnimationHelper] Nenhum GameObject selecionado!");
                return;
            }

            string npcName = ExtractNPCName(selected.name);
            CreateBasicAnimationClips(npcName);
        }

        /// <summary>
        /// Extrai o nome base do NPC removendo prefixos como "NPC_" e sufixos numéricos
        /// </summary>
        /// <param name="fullName">Nome completo do GameObject</param>
        /// <returns>Nome base do NPC</returns>
        private static string ExtractNPCName(string fullName)
        {
            // Remove prefixos comuns
            string name = fullName;
            if (name.StartsWith("NPC_"))
            {
                name = name.Substring(4);
            }

            // Remove sufixos numéricos (ex: "Bee1234" -> "Bee")
            while (name.Length > 0 && char.IsDigit(name[name.Length - 1]))
            {
                name = name.Substring(0, name.Length - 1);
            }

            return name.Trim();
        }

        /// <summary>
        /// Busca e associa animações aos estados do Animator Controller
        /// </summary>
        /// <param name="controller">Animator Controller alvo</param>
        /// <param name="npcName">Nome base do NPC para buscar animações</param>
        private static void AssignAnimationsToStates(AnimatorController controller, string npcName)
        {
            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            int assignedAnimations = 0;

            // Busca por animações em pastas comuns
            string[] searchPaths = {
                "Assets/Art/Animations/NPCs",
                "Assets/Art/Animations",
                "Assets/External/AssetStore/SlimeMec/Art/Animations"
            };

            // Padrões de nome para buscar animações
            string[] idlePatterns = {
                $"{npcName}_Idle",
                $"{npcName.ToLower()}_idle",
                $"art_{npcName.ToLower()}",
                $"idle_{npcName.ToLower()}",
                npcName
            };

            string[] walkingPatterns = {
                $"{npcName}_Walking",
                $"{npcName}_Walk",
                $"{npcName.ToLower()}_walking",
                $"{npcName.ToLower()}_walk",
                $"walk_{npcName.ToLower()}"
            };

            // Tenta associar animação ao estado Idle
            AnimationClip idleClip = FindAnimationClip(searchPaths, idlePatterns);
            if (idleClip != null)
            {
                var idleState = FindStateByName(stateMachine, "Idle");
                if (idleState != null)
                {
                    idleState.motion = idleClip;
                    assignedAnimations++;
                    Debug.Log($"[NPCAnimationHelper] Animação Idle associada: {idleClip.name}");
                }
            }

            // Tenta associar animação ao estado Walking
            AnimationClip walkingClip = FindAnimationClip(searchPaths, walkingPatterns);
            if (walkingClip != null)
            {
                var walkingState = FindStateByName(stateMachine, "Walking");
                if (walkingState != null)
                {
                    walkingState.motion = walkingClip;
                    assignedAnimations++;
                    Debug.Log($"[NPCAnimationHelper] Animação Walking associada: {walkingClip.name}");
                }
            }

            // Salva mudanças
            if (assignedAnimations > 0)
            {
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();
                Debug.Log($"[NPCAnimationHelper] {assignedAnimations} animações associadas com sucesso para '{npcName}'!");
            }
            else
            {
                Debug.LogWarning($"[NPCAnimationHelper] Nenhuma animação encontrada para '{npcName}'. Verifique se os arquivos de animação existem e seguem as convenções de nome.");
                Debug.Log($"[NPCAnimationHelper] Padrões procurados para Idle: {string.Join(", ", idlePatterns)}");
                Debug.Log($"[NPCAnimationHelper] Padrões procurados para Walking: {string.Join(", ", walkingPatterns)}");
            }
        }

        /// <summary>
        /// Encontra um estado por nome no state machine
        /// </summary>
        /// <param name="stateMachine">State machine para buscar</param>
        /// <param name="stateName">Nome do estado</param>
        /// <returns>Estado encontrado ou null</returns>
        private static AnimatorState FindStateByName(AnimatorStateMachine stateMachine, string stateName)
        {
            foreach (var stateInfo in stateMachine.states)
            {
                if (stateInfo.state.name == stateName)
                {
                    return stateInfo.state;
                }
            }
            return null;
        }

        /// <summary>
        /// Busca um clipe de animação em múltiplos caminhos usando múltiplos padrões de nome
        /// </summary>
        /// <param name="searchPaths">Caminhos para buscar</param>
        /// <param name="namePatterns">Padrões de nome para buscar</param>
        /// <returns>Clipe de animação encontrado ou null</returns>
        private static AnimationClip FindAnimationClip(string[] searchPaths, string[] namePatterns)
        {
            foreach (string pattern in namePatterns)
            {
                foreach (string path in searchPaths)
                {
                    // Busca arquivos .anim com o padrão especificado
                    string[] guids = AssetDatabase.FindAssets($"{pattern} t:AnimationClip", new[] { path });

                    foreach (string guid in guids)
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);

                        if (clip != null)
                        {
                            // Verifica se o nome corresponde exatamente ou se contém o padrão
                            if (clip.name.Equals(pattern, System.StringComparison.OrdinalIgnoreCase) ||
                                clip.name.Contains(pattern))
                            {
                                return clip;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Cria clipes de animação básicos para um NPC
        /// </summary>
        /// <param name="npcName">Nome do NPC</param>
        private static void CreateBasicAnimationClips(string npcName)
        {
            string folderPath = "Assets/Art/Animations/NPCs";

            // Cria a pasta se não existir
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string[] pathParts = folderPath.Split('/');
                string currentPath = pathParts[0];

                for (int i = 1; i < pathParts.Length; i++)
                {
                    string newPath = $"{currentPath}/{pathParts[i]}";
                    if (!AssetDatabase.IsValidFolder(newPath))
                    {
                        AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                    }
                    currentPath = newPath;
                }
            }

            // Cria clipe Idle
            AnimationClip idleClip = CreateEmptyAnimationClip($"{npcName}_Idle", 1f);
            if (idleClip != null)
            {
                string idlePath = $"{folderPath}/{npcName}_Idle.anim";
                AssetDatabase.CreateAsset(idleClip, idlePath);
                Debug.Log($"[NPCAnimationHelper] Clipe de animação criado: {idlePath}");
            }

            // Cria clipe Walking
            AnimationClip walkingClip = CreateEmptyAnimationClip($"{npcName}_Walking", 0.5f);
            if (walkingClip != null)
            {
                string walkingPath = $"{folderPath}/{npcName}_Walking.anim";
                AssetDatabase.CreateAsset(walkingClip, walkingPath);
                Debug.Log($"[NPCAnimationHelper] Clipe de animação criado: {walkingPath}");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[NPCAnimationHelper] Clipes de animação básicos criados para '{npcName}'!");
        }

        /// <summary>
        /// Cria um clipe de animação vazio com duração específica
        /// </summary>
        /// <param name="clipName">Nome do clipe</param>
        /// <param name="duration">Duração em segundos</param>
        /// <returns>Clipe de animação criado</returns>
        private static AnimationClip CreateEmptyAnimationClip(string clipName, float duration)
        {
            AnimationClip clip = new AnimationClip();
            clip.name = clipName;
            clip.wrapMode = WrapMode.Loop;

            // Cria uma curva vazia para definir a duração
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(0f, 0f);
            curve.AddKey(duration, 0f);

            // Adiciona a curva ao clipe (propriedade dummy que não afeta o GameObject)
            clip.SetCurve("", typeof(Transform), "localPosition.x", curve);

            return clip;
        }

        /// <summary>
        /// Lista todas as animações encontradas para debug
        /// </summary>
        [UnityEditor.MenuItem("Extra Tools/NPC/List Available Animations")]
        public static void ListAvailableAnimations()
        {
            string[] searchPaths = {
                "Assets/Art/Animations/NPCs",
                "Assets/Art/Animations",
                "Assets/External/AssetStore/SlimeMec/Art/Animations"
            };

            Debug.Log("[NPCAnimationHelper] === ANIMAÇÕES DISPONÍVEIS ===");

            foreach (string path in searchPaths)
            {
                if (AssetDatabase.IsValidFolder(path))
                {
                    string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { path });

                    if (guids.Length > 0)
                    {
                        Debug.Log($"[NPCAnimationHelper] === {path} ===");

                        foreach (string guid in guids)
                        {
                            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);

                            if (clip != null)
                            {
                                Debug.Log($"[NPCAnimationHelper] • {clip.name} ({assetPath})");
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log($"[NPCAnimationHelper] Pasta não encontrada: {path}");
                }
            }

            Debug.Log("[NPCAnimationHelper] === FIM DA LISTA ===");
        }
    }
}