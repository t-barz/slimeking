using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization;
using UnityEngine.Events;
using TMPro;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Configura automaticamente a localização dos botões do PauseMenu.
    /// Menu: Extra Tools/Setup/Configure Pause Menu Localization
    /// </summary>
    public static class ConfigurePauseMenuLocalization
    {
        [MenuItem("Extra Tools/Setup/Configure Pause Menu Localization")]
        public static void Configure()
        {
            // Busca o PauseCanvasHUD na cena ativa
            GameObject pauseCanvas = GameObject.Find("PauseCanvasHUD");

            if (pauseCanvas == null)
            {
                Debug.LogError("[ConfigurePauseMenuLocalization] PauseCanvasHUD não encontrado na cena!");
                return;
            }

            // Busca os botões
            Transform buttonContainer = pauseCanvas.transform.Find("PauseMenuPanel/ButtonContainer");

            if (buttonContainer == null)
            {
                Debug.LogError("[ConfigurePauseMenuLocalization] ButtonContainer não encontrado!");
                return;
            }

            // Configurar cada botão
            ConfigureButton(buttonContainer, "InventoryButton", "UIMenus", "MENU002");
            ConfigureButton(buttonContainer, "SaveButton", "UIMenus", "MENU003");
            ConfigureButton(buttonContainer, "LoadButton", "UIMenus", "MENU004");
            ConfigureButton(buttonContainer, "QuitButton", "UIMenus", "MENU005");

            Debug.Log("[ConfigurePauseMenuLocalization] ✅ Configuração de localização concluída!");
        }

        private static void ConfigureButton(Transform container, string buttonName, string tableCollection, string entryKey)
        {
            Transform button = container.Find(buttonName);

            if (button == null)
            {
                Debug.LogWarning($"[ConfigurePauseMenuLocalization] Botão '{buttonName}' não encontrado!");
                return;
            }

            Transform textTransform = button.Find("Text");

            if (textTransform == null)
            {
                Debug.LogWarning($"[ConfigurePauseMenuLocalization] Text child não encontrado em '{buttonName}'!");
                return;
            }

            // Obtém ou adiciona LocalizeStringEvent
            LocalizeStringEvent localizeEvent = textTransform.GetComponent<LocalizeStringEvent>();

            if (localizeEvent == null)
            {
                localizeEvent = textTransform.gameObject.AddComponent<LocalizeStringEvent>();
                Debug.Log($"[ConfigurePauseMenuLocalization] LocalizeStringEvent adicionado a '{buttonName}/Text'");
            }

            // Configura a StringReference
            localizeEvent.StringReference = new LocalizedString(tableCollection, entryKey);

            // Obtém o componente TextMeshProUGUI
            TextMeshProUGUI textComponent = textTransform.GetComponent<TextMeshProUGUI>();

            if (textComponent == null)
            {
                Debug.LogWarning($"[ConfigurePauseMenuLocalization] TextMeshProUGUI não encontrado em '{buttonName}/Text'!");
                return;
            }

            // Remove todos os listeners existentes
            int listenerCount = localizeEvent.OnUpdateString.GetPersistentEventCount();
            for (int i = listenerCount - 1; i >= 0; i--)
            {
                UnityEventTools.RemovePersistentListener(localizeEvent.OnUpdateString, i);
            }

            // Adiciona o evento persistente usando UnityEventTools
            UnityAction<string> action = new UnityAction<string>(textComponent.SetText);
            UnityEventTools.AddPersistentListener(localizeEvent.OnUpdateString, action);

            // Força a atualização imediata do texto
            localizeEvent.RefreshString();

            // Marca como modificado
            EditorUtility.SetDirty(localizeEvent);
            EditorUtility.SetDirty(textComponent.gameObject);

            Debug.Log($"[ConfigurePauseMenuLocalization] ✅ '{buttonName}' configurado com chave '{entryKey}' da tabela '{tableCollection}'");
        }
    }
}
