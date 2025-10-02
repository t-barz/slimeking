using System.Collections;
using UnityEngine;

namespace ExtraTools
{
    /// <summary>
    /// AudioManager - Gerenciador de áudio persistente para The Slime King
    /// Responsável por música contínua entre cenas e efeitos sonoros
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        #region Singleton
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Audio Sources
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        #endregion

        #region Music Clips
        [Header("Music Library")]
        [SerializeField] private AudioClip splashMusic;
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameplayMusic;
        #endregion

        #region Volume Settings
        [Header("Volume Settings")]
        [Range(0f, 1f)][SerializeField] private float masterVolume = 1f;
        [Range(0f, 1f)][SerializeField] private float musicVolume = 0.7f;
        [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.8f;
        #endregion

        private AudioClip currentMusicClip;
        private bool isCrossfading = false;

        /// <summary>
        /// Indica se a música atual é a música de splash.
        /// </summary>
        public bool IsPlayingSplashMusic => currentMusicClip == splashMusic;
        /// <summary>
        /// Indica se a música atual é a música de menu.
        /// </summary>
        public bool IsPlayingMenuMusic => currentMusicClip == menuMusic;
        /// <summary>
        /// Indica se a música atual é a música de gameplay.
        /// </summary>
        public bool IsPlayingGameplayMusic => currentMusicClip == gameplayMusic;

        /// <summary>
        /// Inicializa AudioSources se não foram atribuídas no Inspector
        /// </summary>
        private void InitializeAudioManager()
        {
            if (musicSource == null)
            {
                GameObject musicGO = new GameObject("MusicSource");
                musicGO.transform.SetParent(transform);
                musicSource = musicGO.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.volume = musicVolume;
            }

            if (sfxSource == null)
            {
                GameObject sfxGO = new GameObject("SFXSource");
                sfxGO.transform.SetParent(transform);
                sfxSource = sfxGO.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.volume = sfxVolume;
            }

            ApplyVolumeSettings();
            Debug.Log($"[AudioManager] Inicializado - Master: {masterVolume}, Music: {musicVolume}, SFX: {sfxVolume}");
        }

        #region Music Control
        /// <summary>
        /// Inicia música de splash (continua entre cenas)
        /// </summary>
        public void PlaySplashMusic()
        {
            PlayMusic(splashMusic);
            Debug.Log("[AudioManager] Música de splash iniciada");
        }

        /// <summary>
        /// Troca para música do menu principal
        /// </summary>
        public void PlayMenuMusic(bool crossfade = true)
        {
            if (crossfade && musicSource.isPlaying && currentMusicClip != menuMusic)
            {
                StartCoroutine(CrossfadeToMusic(menuMusic, 1f));
            }
            else
            {
                PlayMusic(menuMusic);
            }
            Debug.Log("[AudioManager] Música do menu iniciada");
        }

        /// <summary>
        /// Troca para música de gameplay
        /// </summary>
        public void PlayGameplayMusic(bool crossfade = true)
        {
            if (crossfade && musicSource.isPlaying && currentMusicClip != gameplayMusic)
            {
                StartCoroutine(CrossfadeToMusic(gameplayMusic, 1.5f));
            }
            else
            {
                PlayMusic(gameplayMusic);
            }
            Debug.Log("[AudioManager] Música de gameplay iniciada");
        }

        /// <summary>
        /// Toca uma música específica
        /// </summary>
        private void PlayMusic(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("[AudioManager] AudioClip é null - não é possível tocar música");
                return;
            }

            if (currentMusicClip == clip && musicSource.isPlaying)
            {
                Debug.Log($"[AudioManager] Música '{clip.name}' já está tocando");
                return;
            }

            Debug.Log($"[AudioManager] Iniciando música: {clip.name}");
            musicSource.clip = clip;
            musicSource.Play();
            currentMusicClip = clip;
            Debug.Log($"[AudioManager] Música '{clip.name}' definida e Play() chamado");
        }

        /// <summary>
        /// Crossfade suave entre músicas
        /// </summary>
        private IEnumerator CrossfadeToMusic(AudioClip newClip, float fadeTime = 1f)
        {
            if (isCrossfading || newClip == null) yield break;
            isCrossfading = true;

            float originalVolume = musicSource.volume;

            // Fade out música atual
            while (musicSource.volume > 0.01f)
            {
                musicSource.volume -= originalVolume * Time.deltaTime / fadeTime;
                yield return null;
            }

            // Troca para nova música
            musicSource.clip = newClip;
            musicSource.Play();
            currentMusicClip = newClip;

            // Fade in nova música
            while (musicSource.volume < originalVolume)
            {
                musicSource.volume += originalVolume * Time.deltaTime / fadeTime;
                yield return null;
            }

            musicSource.volume = originalVolume;
            isCrossfading = false;
        }

        /// <summary>
        /// Para toda música
        /// </summary>
        public void StopMusic()
        {
            musicSource.Stop();
            currentMusicClip = null;
        }
        #endregion

        #region Volume Control
        /// <summary>
        /// Define volume master (afeta tudo)
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
        }

        /// <summary>
        /// Define volume da música
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
                musicSource.volume = musicVolume * masterVolume;
        }

        /// <summary>
        /// Define volume dos efeitos sonoros
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource != null)
                sfxSource.volume = sfxVolume * masterVolume;
        }

        /// <summary>
        /// Aplica configurações de volume
        /// </summary>
        private void ApplyVolumeSettings()
        {
            if (musicSource != null)
                musicSource.volume = musicVolume * masterVolume;
            if (sfxSource != null)
                sfxSource.volume = sfxVolume * masterVolume;
        }
        #endregion

        #region Sound Effects
        /// <summary>
        /// Toca um efeito sonoro
        /// </summary>
        public void PlaySFX(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// Toca efeito sonoro com volume customizado
        /// </summary>
        public void PlaySFX(AudioClip clip, float volumeScale)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip, volumeScale);
            }
        }
        #endregion

        #region Public Properties
        public bool IsMusicPlaying => musicSource != null && musicSource.isPlaying;
        public AudioClip CurrentMusic => currentMusicClip;
        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float SFXVolume => sfxVolume;
        #endregion
    }
}