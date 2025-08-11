using UnityEngine;
using System.Threading.Tasks;
using System;

namespace SlimeKing.Core.Cutscenes
{
    /// <summary>
    /// Gerencia as cutscenes ativas e garante que apenas uma esteja rodando por vez
    /// </summary>
    public class CutsceneManager : MonoBehaviour
    {
        private static CutsceneManager instance;
        public static CutsceneManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("CutsceneManager");
                    instance = go.AddComponent<CutsceneManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private CutsceneDefinition activeCutscene;
        private bool isPlaying;
        private CutsceneContext context;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            context = new CutsceneContext();
        }

        public async Task PlayCutscene(CutsceneDefinition cutscene)
        {
            if (isPlaying)
            {
                Debug.LogWarning("Tentando reproduzir cutscene enquanto outra já está em execução");
                return;
            }

            try
            {
                isPlaying = true;
                activeCutscene = cutscene;

                // Setup do contexto da cutscene
                SetupCutsceneContext();

                // Desabilita controle do jogador se necessário
                if (cutscene.DisablePlayerControl)
                {
                    DisablePlayerControl();
                }

                // Executa cada evento em sequência
                foreach (var cutsceneEvent in cutscene.Events)
                {
                    await cutsceneEvent.Execute(context);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro durante execução da cutscene: {e}");
            }
            finally
            {
                // Cleanup
                if (cutscene.DisablePlayerControl)
                {
                    EnablePlayerControl();
                }

                isPlaying = false;
                activeCutscene = null;
            }
        }

        public void SkipCurrentCutscene()
        {
            if (!isPlaying || activeCutscene == null || !activeCutscene.CanBeSkipped)
                return;

            // TODO: Implementar lógica de skip
        }

        private void SetupCutsceneContext()
        {
            if (context == null)
                context = new CutsceneContext();

            var cutsceneRoot = new GameObject("CutsceneRoot").transform;
            cutsceneRoot.SetParent(transform);

            context.Player = GameObject.FindGameObjectWithTag("Player");
            context.MainCamera = Camera.main;
            context.Vignette = FindObjectOfType<VignetteController>();
            context.CutsceneRoot = cutsceneRoot;
        }

        private void DisablePlayerControl()
        {
            // TODO: Implementar desativação dos controles do jogador
            // Isso dependerá do seu sistema de input
        }

        private void EnablePlayerControl()
        {
            // TODO: Implementar reativação dos controles do jogador
        }
    }
}
