# AudioManager Simples - Guia de Uso

## Funcionalidades

✅ **Fade In/Out**: Transições suaves entre músicas  
✅ **Volume Master**: Controle centralizado de volume  
✅ **Persistência**: Configurações salvas automaticamente  
✅ **Crossfade**: Troca suave entre músicas  

## Como Usar

### 1. Tocar Música

```csharp
// Toca com fade in (padrão)
AudioManager.Instance.PlayMusic(musicClip);

// Toca sem fade
AudioManager.Instance.PlayMusic(musicClip, false);

// Toca com fade personalizado
AudioManager.Instance.PlayMusic(musicClip, true, 3f);
```

### 2. Parar Música

```csharp
// Para com fade out
AudioManager.Instance.StopMusic();

// Para imediatamente
AudioManager.Instance.StopMusic(false);

// Para com fade personalizado
AudioManager.Instance.StopMusic(true, 2f);
```

### 3. Controle de Volume

```csharp
// Volume master (0.0 a 1.0)
AudioManager.Instance.SetMasterVolume(0.8f);

// Volume da música
AudioManager.Instance.SetMusicVolume(0.6f);

// Volume para SFX (outros scripts)
float sfxVolume = AudioManager.Instance.GetSFXVolume();
```

### 4. Verificar Estado

```csharp
// Música está tocando?
bool isPlaying = AudioManager.Instance.IsMusicPlaying;

// Qual música atual?
AudioClip current = AudioManager.Instance.CurrentMusic;
```

## Configuração no Inspector

### AudioManager GameObject

- **Master Volume**: Volume geral (0-1)
- **Music Volume**: Volume específico da música (0-1)  
- **SFX Volume**: Volume para efeitos sonoros (0-1)
- **Default Fade Time**: Tempo padrão de fade (segundos)

## Exemplo no TitleScreenController

```csharp
public class TitleScreenController : MonoBehaviour
{
    [SerializeField] private AudioClip titleMusic;
    
    void Start()
    {
        if (titleMusic != null)
        {
            AudioManager.Instance.PlayMusic(titleMusic);
        }
    }
}
```

## Vantagens

🎯 **Simples**: Interface direta, sem complexidade  
⚡ **Eficiente**: Sem cache ou carregamento desnecessário  
🔧 **Flexível**: Fade configurável por chamada  
💾 **Persistente**: Configurações salvas automaticamente  
🎮 **Prático**: Resolve os casos de uso reais  

Para SFX, use AudioSources diretamente nos GameObjects ou crie um pool simples conforme necessário.
