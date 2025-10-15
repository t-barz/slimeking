using SlimeKing.Core;
using UnityEngine;

public class GameManager : ManagerSingleton<GameManager>
{
    // Inicialização mínima seguindo KISS: define estado inicial e aplica configurações básicas de runtime.
    protected override void Initialize()
    {
        // Configurações básicas do runtime (podem ser ajustadas depois por outros managers)
        Application.targetFrameRate = 60; // manter consistente
        Time.timeScale = 1f;              // garantir tempo normal

        // Estado inicial simples (usar GameState se definido em enums do projeto)
        // Como este GameManager está reduzido, apenas registra o bootstrap.
        Log("GameManager bootstrap concluído");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
