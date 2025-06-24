using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheSlimeKing.Core.UI.Icons
{
    /// <summary>
    /// ScriptableObject que define o mapeamento entre ações de input e ícones por dispositivo
    /// </summary>
    [CreateAssetMenu(fileName = "InputIconMapping", menuName = "The Slime King/InputIconMapping")]
    public class InputIconData : ScriptableObject
    {
        /// <summary>
        /// Classe que representa o mapeamento de ícones para uma ação específica em diferentes dispositivos
        /// </summary>
        [Serializable]
        public class ActionIcon
        {
            public string actionName;
            public Sprite keyboardIcon;
            public Sprite xboxIcon;
            public Sprite playstationIcon;
            public Sprite switchIcon;
            public Sprite genericIcon;
        }

        /// <summary>
        /// Lista de mapeamentos de ações para ícones
        /// </summary>
        public List<ActionIcon> actionIcons = new List<ActionIcon>();

        /// <summary>
        /// Retorna o ícone apropriado para uma ação específica baseado no tipo de dispositivo atual
        /// </summary>
        /// <param name="actionName">Nome da ação no InputSystem</param>
        /// <param name="deviceType">Tipo de dispositivo atual</param>
        /// <returns>Sprite do ícone correspondente à ação e dispositivo</returns>
        public Sprite GetIconForAction(string actionName, DeviceType deviceType)
        {
            ActionIcon actionIcon = actionIcons.Find(ai => ai.actionName == actionName);
            if (actionIcon == null)
            {
                Debug.LogWarning($"Ícone não encontrado para a ação: {actionName}");
                return null;
            }

            switch (deviceType)
            {
                case DeviceType.Keyboard:
                    return actionIcon.keyboardIcon;
                case DeviceType.Xbox:
                    return actionIcon.xboxIcon;
                case DeviceType.PlayStation:
                    return actionIcon.playstationIcon;
                case DeviceType.Switch:
                    return actionIcon.switchIcon;
                case DeviceType.Generic:
                    return actionIcon.genericIcon;
                default:
                    Debug.LogWarning($"Tipo de dispositivo não suportado: {deviceType}");
                    return null;
            }
        }
    }
}
