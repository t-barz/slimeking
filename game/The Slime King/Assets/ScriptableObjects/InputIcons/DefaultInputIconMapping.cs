using UnityEngine;
using TheSlimeKing.Core.UI.Icons;

[CreateAssetMenu(fileName = "DefaultInputIconMapping", menuName = "The Slime King/Input Icons/Default Mapping")]
public class DefaultInputIconMapping : InputIconData
{
    private void OnValidate()
    {
        // Este método é chamado ao editar o asset no editor
        // Verifica se todas as ações necessárias estão configuradas

        // Estas são as ações principais do jogo conforme a tabela de mapeamento
        EnsureActionExists("Move");
        EnsureActionExists("Attack");
        EnsureActionExists("Interact");
        EnsureActionExists("Crouch");
        EnsureActionExists("UseItem");
        EnsureActionExists("ChangeItem");
        EnsureActionExists("Ability1");
        EnsureActionExists("Ability2");
        EnsureActionExists("Ability3");
        EnsureActionExists("Ability4");
        EnsureActionExists("Menu");
        EnsureActionExists("Inventory");
    }

    /// <summary>
    /// Verifica se uma ação existe na lista, se não, cria uma vazia
    /// </summary>
    private void EnsureActionExists(string actionName)
    {
        if (!actionIcons.Exists(ai => ai.actionName == actionName))
        {
            ActionIcon newIcon = new ActionIcon
            {
                actionName = actionName
            };
            actionIcons.Add(newIcon);
        }
    }
}
