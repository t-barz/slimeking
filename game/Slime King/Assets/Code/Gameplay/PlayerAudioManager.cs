using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Gerencia o sistema de áudio do jogador.
    /// Responsável por tocar e controlar todos os efeitos sonoros do personagem.
    /// </summary>
    public class PlayerAudioManager : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip walkingSound;
        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip slideSound;
        [SerializeField] private AudioClip attackSound;

        [Header("Settings")]
        [Range(0, 1)]
        [SerializeField] private float soundVolume = 1f;

        private AudioSource audioSource;
        private float walkingSoundInterval = 0.3f;
        private float lastWalkingSoundTime;

        private void Awake()
        {
            SetupAudioSource();
        }

        private void SetupAudioSource()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = soundVolume;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // Set to 2D sound
        }

        public void PlayWalkingSound()
        {
            if (walkingSound != null && !audioSource.isPlaying)
            {
                if (Time.time - lastWalkingSoundTime >= walkingSoundInterval)
                {
                    PlaySound(walkingSound, false);
                    lastWalkingSoundTime = Time.time;
                }
            }
        }

        public void PlayJumpSound()
        {
            if (jumpSound != null)
            {
                PlaySound(jumpSound, false);
            }
        }

        public void PlaySlideSound()
        {
            if (slideSound != null)
            {
                PlaySound(slideSound, false);
            }
        }

        public void PlayAttackSound()
        {
            if (attackSound != null)
            {
                PlaySound(attackSound, false);
            }
        }

        private void PlaySound(AudioClip clip, bool loop)
        {
            audioSource.Stop();
            audioSource.loop = loop;
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void StopAllSounds()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
