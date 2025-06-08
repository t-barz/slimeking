using UnityEngine;

namespace SlimeKing.Core.Cutscenes
{
    /// <summary>
    /// Inicia a cutscene de abertura assim que a cena for carregada.
    /// Assume que o personagem já começa com a animação Sleep e isSleeping = true.
    /// </summary>
    public class OpeningCutsceneStarter : MonoBehaviour
    {
        [SerializeField] private CutsceneDefinition openingCutscene;

        private void Start()
        {
            if (openingCutscene == null)
            {
                // Tenta carregar a cutscene dos Resources se não estiver atribuída
                openingCutscene = Resources.Load<CutsceneDefinition>("Cutscenes/OpeningCutscene");

                if (openingCutscene == null)
                {
                    Debug.LogError("OpeningCutsceneStarter: Não foi possível carregar a cutscene inicial!");
                    return;
                }
            }

            StartCutscene();
        }

        private async void StartCutscene()
        {
            await CutsceneManager.Instance.PlayCutscene(openingCutscene);
        }
    }
}
