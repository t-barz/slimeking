using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlimeMec.Gameplay;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Utility class for batch configuration of multiple NPCs with performance optimizations.
    /// Supports parallel processing and resource sharing for efficient bulk operations.
    /// </summary>
    public static class NPCBatchConfigurator
    {
        /// <summary>
        /// Result of a batch configuration operation.
        /// </summary>
        public class BatchResult
        {
            public int TotalCount;
            public int SuccessCount;
            public int FailureCount;
            public List<string> Errors;
            public double ElapsedTime;

            public BatchResult()
            {
                Errors = new List<string>();
            }
        }

        /// <summary>
        /// Configures multiple NPCs in batch with performance optimizations.
        /// Uses resource sharing for common assets (same species uses same FriendshipData).
        /// </summary>
        /// <param name="targets">List of GameObjects to configure</param>
        /// <param name="config">Configuration to apply to all NPCs</param>
        /// <param name="useUniqueNames">If true, generates unique names for each NPC</param>
        /// <returns>BatchResult with statistics</returns>
        public static BatchResult ConfigureNPCsBatch(List<GameObject> targets, NPCConfigData config, bool useUniqueNames = true)
        {
            BatchResult result = new BatchResult
            {
                TotalCount = targets.Count
            };

            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // Performance Optimization: Group all undo operations
                Undo.SetCurrentGroupName($"Configure {targets.Count} NPCs");
                int undoGroup = Undo.GetCurrentGroup();

                // Performance Optimization: Start asset editing batch
                AssetDatabase.StartAssetEditing();

                // Resource Sharing: Create shared resources once
                FriendshipData sharedFriendshipData = null;
                if (config.friendshipEnabled && !string.IsNullOrEmpty(config.speciesName))
                {
                    sharedFriendshipData = NPCDataGenerator.CreateFriendshipData(config.speciesName);
                }

                AnimatorController sharedAnimatorController = null;
                if (!useUniqueNames)
                {
                    sharedAnimatorController = NPCAnimatorSetup.CreateOrLoadController(config.npcName);
                    if (sharedAnimatorController != null)
                    {
                        NPCAnimatorSetup.ConfigureStates(sharedAnimatorController, config.dialogueEnabled);
                        NPCAnimatorSetup.ConfigureParameters(sharedAnimatorController, config.dialogueEnabled);
                        NPCAnimatorSetup.ConfigureTransitions(sharedAnimatorController, config.dialogueEnabled);
                        NPCAnimatorSetup.ConfigurePlaceholderAnimations(sharedAnimatorController, config.npcName, config.dialogueEnabled);
                    }
                }

                // Configure each NPC
                for (int i = 0; i < targets.Count; i++)
                {
                    GameObject target = targets[i];

                    if (target == null)
                    {
                        result.FailureCount++;
                        result.Errors.Add($"Target {i} is null");
                        continue;
                    }

                    try
                    {
                        // Register undo for this object
                        Undo.RegisterCompleteObjectUndo(target, "Configure NPC");

                        // Create unique config for this NPC
                        NPCConfigData npcConfig = new NPCConfigData
                        {
                            npcName = useUniqueNames ? $"{config.npcName}_{i + 1}" : config.npcName,
                            speciesName = config.speciesName,
                            behaviorType = config.behaviorType,
                            aiType = config.aiType,
                            detectionRange = config.detectionRange,
                            aiSettings = config.aiSettings,
                            friendshipEnabled = config.friendshipEnabled,
                            friendshipSettings = config.friendshipSettings,
                            dialogueEnabled = config.dialogueEnabled,
                            dialogueSettings = config.dialogueSettings
                        };

                        // Create DialogueData (unique per NPC)
                        DialogueData dialogueData = null;
                        if (config.dialogueEnabled)
                        {
                            dialogueData = NPCDataGenerator.CreateDialogueData(npcConfig.npcName, config.behaviorType);
                        }

                        // Create NPCData (unique per NPC, but shares FriendshipData)
                        NPCData npcData = NPCDataGenerator.CreateNPCData(npcConfig, sharedFriendshipData, dialogueData);

                        // Get or create animator controller
                        AnimatorController animatorController = sharedAnimatorController;
                        if (useUniqueNames)
                        {
                            animatorController = NPCAnimatorSetup.CreateOrLoadController(npcConfig.npcName);
                            if (animatorController != null)
                            {
                                NPCAnimatorSetup.ConfigureStates(animatorController, config.dialogueEnabled);
                                NPCAnimatorSetup.ConfigureParameters(animatorController, config.dialogueEnabled);
                                NPCAnimatorSetup.ConfigureTransitions(animatorController, config.dialogueEnabled);
                                NPCAnimatorSetup.ConfigurePlaceholderAnimations(animatorController, npcConfig.npcName, config.dialogueEnabled);
                            }
                        }

                        // Configure components
                        NPCComponentConfigurator.ConfigureBasicComponents(target, npcConfig, npcData, animatorController);

                        // Mark as dirty
                        EditorUtility.SetDirty(target);

                        result.SuccessCount++;
                    }
                    catch (System.Exception e)
                    {
                        result.FailureCount++;
                        result.Errors.Add($"Failed to configure {target.name}: {e.Message}");
                        UnityEngine.Debug.LogError($"❌ Failed to configure {target.name}: {e.Message}");
                    }
                }

                // Performance Optimization: Collapse all undo operations
                Undo.CollapseUndoOperations(undoGroup);
            }
            catch (System.Exception e)
            {
                result.Errors.Add($"Batch operation failed: {e.Message}");
                UnityEngine.Debug.LogError($"❌ Batch configuration failed: {e.Message}");
            }
            finally
            {
                // Performance Optimization: Stop asset editing and refresh
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                stopwatch.Stop();
                result.ElapsedTime = stopwatch.Elapsed.TotalSeconds;
            }

            return result;
        }

        /// <summary>
        /// Validates multiple NPCs in parallel for faster validation.
        /// </summary>
        /// <param name="targets">List of GameObjects to validate</param>
        /// <param name="config">Configuration to validate</param>
        /// <returns>List of validation results</returns>
        public static List<ValidationResult> ValidateNPCsBatch(List<GameObject> targets, NPCConfigData config)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            // Performance Optimization: Parallel validation
            object lockObject = new object();

            Parallel.ForEach(targets, target =>
            {
                ValidationResult result = NPCValidator.ValidateConfiguration(config, target);

                lock (lockObject)
                {
                    results.Add(result);
                }
            });

            return results;
        }

        /// <summary>
        /// Displays a progress bar during batch operations.
        /// </summary>
        /// <param name="current">Current progress</param>
        /// <param name="total">Total items</param>
        /// <param name="title">Progress bar title</param>
        public static void ShowProgress(int current, int total, string title = "Configuring NPCs")
        {
            float progress = (float)current / total;
            EditorUtility.DisplayProgressBar(title, $"Processing {current}/{total}...", progress);
        }

        /// <summary>
        /// Clears the progress bar.
        /// </summary>
        public static void ClearProgress()
        {
            EditorUtility.ClearProgressBar();
        }
    }
}
