# Slime King - Sistema de Localização

O sistema de localização do Slime King foi projetado para ser modular, eficiente e fácil de usar. Este documento explica como utilizá-lo em seu projeto.

## Estrutura

O sistema é composto por:

- `ILocalizationService`: Interface que define o contrato básico do serviço de localização
- `LocalizationService`: Implementação principal do serviço de localização
- `LocalizationManager`: MonoBehaviour que gerencia o serviço na cena
- `LocalizationData`: ScriptableObject que contém os dados de tradução
- `LocalizeTexts`: Componente para automaticamente traduzir textos UI

## Como Usar

### 1. Configuração Inicial

1. Crie um `LocalizationData` (Asset Menu → SlimeKing → Localization → LocalizationData)
2. Adicione suas traduções no inspector
3. Adicione o prefab `LocalizationManager` à sua cena inicial
4. Configure o `LocalizationData` no inspector do `LocalizationManager`

### 2. Traduzindo Textos UI

1. Adicione o componente `LocalizeTexts` a qualquer objeto com `TextMeshProUGUI`
2. Defina a chave de tradução no inspector (ex: "menu.play")
3. O texto será automaticamente traduzido e atualizado quando o idioma mudar

### 3. Tradução Via Código

```csharp
// Obter o serviço
var manager = Object.FindFirstObjectByType<LocalizationManager>();
var localization = manager.LocalizationService;

// Obter texto traduzido
string translated = localization.GetLocalizedText("menu.play");

// Mudar idioma
manager.SetLanguage(SystemLanguage.Portuguese);
```

## Estrutura de Chaves

Recomendamos usar uma estrutura hierárquica para as chaves de tradução:

- `menu.play` - Botão jogar no menu
- `menu.settings` - Botão configurações
- `game.title` - Título do jogo
- `dialog.confirm` - Texto de confirmação

## Melhores Práticas

1. Mantenha as chaves organizadas e descritivas
2. Sempre forneça uma tradução em inglês como fallback
3. Use o componente `LocalizeTexts` para UI ao invés de traduzir manualmente
4. Teste suas chaves no editor antes de buildar
