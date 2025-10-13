using System.Collections;
using UnityEngine;

namespace SlimeKing.Core
{
    /// <summary>
    /// AudioManager simples com foco em fade in/out e volume master.
    /// </summary>
    public class AudioManager : ManagerBase<AudioManager>
    {
        [Header("Volume Settings")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 0.8f;
        [Range(0f, 1f)] public float sfxVolume = 1f;

        [Header("Fade Settings")]
        [SerializeField] private float defaultFadeTime = 1f;

        // AudioSource para música
        private AudioSource musicSource;

        // Controle de fade
        private Coroutine fadeCoroutine;
        private AudioClip currentMusicClip;

        protected override void Initialize()
        {
            SetupMusicSource();
            LoadVolumeSettings();
            Log("AudioManager initialized (Simple)");
        }

        #region Setup

        /// <summary>
        /// Configura a AudioSource de música
        /// </summary>
        private void SetupMusicSource()
        {
            GameObject musicGO = new GameObject("MusicSource");
            musicGO.transform.SetParent(transform);
            musicSource = musicGO.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            UpdateMusicVolume();
        }

        #endregion

        #region Music Control

        /// <summary>
        /// Toca uma música com fade in opcional
        /// </summary>
        public void PlayMusic(AudioClip clip, bool fadeIn = true, float fadeTime = -1f)
        {
            if (clip == null) return;

            if (fadeTime < 0) fadeTime = defaultFadeTime;

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            currentMusicClip = clip;

            if (fadeIn && musicSource.isPlaying)
            {
                fadeCoroutine = StartCoroutine(CrossfadeMusic(clip, fadeTime));
            }
            else
            {
                PlayMusicImmediate(clip, fadeIn, fadeTime);
            }
        }

        /// <summary>
        /// Para a música com fade out opcional
        /// </summary>
        public void StopMusic(bool fadeOut = true, float fadeTime = -1f)
        {
            if (!musicSource.isPlaying) return;

            if (fadeTime < 0) fadeTime = defaultFadeTime;

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            if (fadeOut)
            {
                fadeCoroutine = StartCoroutine(FadeOutMusic(fadeTime));
            }
            else
            {
                musicSource.Stop();
                currentMusicClip = null;
            }
        }

        /// <summary>
        /// Toca música imediatamente
        /// </summary>
        private void PlayMusicImmediate(AudioClip clip, bool fadeIn, float fadeTime)
        {
            musicSource.clip = clip;
            musicSource.volume = fadeIn ? 0f : musicVolume * masterVolume;
            musicSource.Play();

            if (fadeIn)
            {
                fadeCoroutine = StartCoroutine(FadeInMusic(fadeTime));
            }
        }

        #endregion

        #region Fade Effects

        /// <summary>
        /// Fade in da música atual
        /// </summary>
        private IEnumerator FadeInMusic(float fadeTime)
        {
            float startVolume = 0f;
            float targetVolume = musicVolume * masterVolume;
            float elapsed = 0f;

            musicSource.volume = startVolume;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / fadeTime;
                musicSource.volume = Mathf.Lerp(startVolume, targetVolume, progress);
                yield return null;
            }

            musicSource.volume = targetVolume;
            fadeCoroutine = null;
        }

        /// <summary>
        /// Fade out da música atual
        /// </summary>
        private IEnumerator FadeOutMusic(float fadeTime)
        {
            float startVolume = musicSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / fadeTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, progress);
                yield return null;
            }

            musicSource.volume = 0f;
            musicSource.Stop();
            currentMusicClip = null;
            fadeCoroutine = null;
        }

        /// <summary>
        /// Crossfade entre músicas
        /// </summary>
        private IEnumerator CrossfadeMusic(AudioClip newClip, float fadeTime)
        {
            // Fade out da música atual
            float startVolume = musicSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeTime * 0.5f)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / (fadeTime * 0.5f);
                musicSource.volume = Mathf.Lerp(startVolume, 0f, progress);
                yield return null;
            }

            // Troca música
            musicSource.clip = newClip;
            musicSource.Play();

            // Fade in da nova música
            elapsed = 0f;
            float targetVolume = musicVolume * masterVolume;

            while (elapsed < fadeTime * 0.5f)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / (fadeTime * 0.5f);
                musicSource.volume = Mathf.Lerp(0f, targetVolume, progress);
                yield return null;
            }

            musicSource.volume = targetVolume;
            fadeCoroutine = null;
        }

        #endregion

        #region Volume Control

        /// <summary>
        /// Atualiza volume da música
        /// </summary>
        private void UpdateMusicVolume()
        {
            if (musicSource != null && musicSource.isPlaying)
            {
                musicSource.volume = musicVolume * masterVolume;
            }
        }

        /// <summary>
        /// Define volume master
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateMusicVolume();
            SaveVolumeSettings();
        }

        /// <summary>
        /// Define volume da música
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateMusicVolume();
            SaveVolumeSettings();
        }

        /// <summary>
        /// Define volume dos SFX
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            SaveVolumeSettings();
        }

        #endregion

        #region Settings Persistence

        /// <summary>
        /// Carrega configurações de volume
        /// </summary>
        private void LoadVolumeSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("Audio_MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("Audio_MusicVolume", 0.8f);
            sfxVolume = PlayerPrefs.GetFloat("Audio_SFXVolume", 1f);
            UpdateMusicVolume();
        }

        /// <summary>
        /// Salva configurações de volume
        /// </summary>
        public void SaveVolumeSettings()
        {
            PlayerPrefs.SetFloat("Audio_MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("Audio_MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("Audio_SFXVolume", sfxVolume);
            PlayerPrefs.Save();
        }

        #endregion

        #region Utility

        /// <summary>
        /// Retorna se há música tocando
        /// </summary>
        public bool IsMusicPlaying => musicSource != null && musicSource.isPlaying;

        /// <summary>
        /// Retorna o clip de música atual
        /// </summary>
        public AudioClip CurrentMusic => currentMusicClip;

        /// <summary>
        /// Retorna volume SFX calculado (para outros scripts usarem)
        /// </summary>
        public float GetSFXVolume() => sfxVolume * masterVolume;

        #endregion
    }
}