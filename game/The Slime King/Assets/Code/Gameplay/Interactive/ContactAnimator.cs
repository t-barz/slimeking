using UnityEngine;
using System.Collections.Generic;

namespace TheSlimeKing.Gameplay.Interactive
{
    /// <summary>
    /// Controla objetos que reagem com animações e sons quando outros objetos entram em contato
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(Animator))]
    public class ContactAnimator : MonoBehaviour
    {
        [Header("Configurações de Detecção")]
        [Tooltip("Tags que podem ativar este objeto (Player, Enemy, Nature)")]
        [SerializeField] private List<string> _reactToTags = new List<string>() { "Player", "Enemy", "Nature" };

        [Header("Configurações de Reação")]
        [Tooltip("Reagir quando objetos entram na área do trigger")]
        [SerializeField] private bool _reactOnEnter = true;

        [Tooltip("Reagir quando objetos saem da área do trigger")]
        [SerializeField] private bool _reactOnExit = true;

        [Header("Animação")]
        [Tooltip("Nome do parâmetro trigger no Animator")]
        [SerializeField] private string _animationTriggerName = "OnContact";

        [Header("Efeitos Sonoros")]
        [Tooltip("Lista de sons que podem ser reproduzidos aleatoriamente")]
        [SerializeField] private List<AudioClip> _contactSounds = new List<AudioClip>();

        [Tooltip("Volume dos efeitos sonoros")]
        [Range(0f, 1f)]
        [SerializeField] private float _soundVolume = 0.7f;

        [Header("Efeitos Visuais")]
        [Tooltip("Efeito de partículas (opcional)")]
        [SerializeField] private ParticleSystem _contactParticles;

        [Header("Depuração")]
        [Tooltip("Mostrar mensagens de debug no console")]
        [SerializeField] private bool _showDebugMessages = false;

        // Componentes
        private Animator _animator;
        private AudioSource _audioSource;

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            // Procura AudioSource ou cria um novo se necessário e houver sons configurados
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null && _contactSounds.Count > 0)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.spatialBlend = 1.0f; // Som 3D
            }
        }

        private void Start()
        {
            // Garante que o Collider2D está configurado como trigger
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && !col.isTrigger)
            {
                col.isTrigger = true;
                if (_showDebugMessages)
                    Debug.Log($"ContactAnimator: Collider em {gameObject.name} foi configurado como trigger.");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Verifica se devemos reagir na entrada e se o objeto tem uma tag válida
            if (_reactOnEnter && _reactToTags.Contains(other.tag))
            {
                HandleContact(other.gameObject, true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Verifica se devemos reagir na saída e se o objeto tem uma tag válida
            if (_reactOnExit && _reactToTags.Contains(other.tag))
            {
                HandleContact(other.gameObject, false);
            }
        }

        /// <summary>
        /// Processa o evento de contato (tanto entrada quanto saída)
        /// </summary>
        /// <param name="contactor">Objeto que fez contato</param>
        /// <param name="isEntering">True se está entrando, False se está saindo</param>
        private void HandleContact(GameObject contactor, bool isEntering)
        {
            // Dispara o trigger de animação
            if (_animator != null && !string.IsNullOrEmpty(_animationTriggerName))
            {
                _animator.SetTrigger(_animationTriggerName);
            }

            // Reproduz um som aleatório da lista
            PlayRandomSound();

            // Ativa efeito de partículas
            if (_contactParticles != null)
            {
                if (_contactParticles.isPlaying)
                    _contactParticles.Stop();

                _contactParticles.Play();
            }

            if (_showDebugMessages)
            {
                string action = isEntering ? "entrou em" : "saiu do";
                Debug.Log($"ContactAnimator: {contactor.name} ({contactor.tag}) {action} contato com {gameObject.name}");
            }
        }

        /// <summary>
        /// Reproduz um som aleatório da lista de sons configurados
        /// </summary>
        private void PlayRandomSound()
        {
            if (_audioSource == null || _contactSounds == null || _contactSounds.Count == 0)
                return;

            // Filtra clips nulos
            List<AudioClip> validClips = new List<AudioClip>();
            foreach (var clip in _contactSounds)
            {
                if (clip != null)
                    validClips.Add(clip);
            }

            if (validClips.Count == 0)
                return;

            // Escolhe um som aleatório da lista
            AudioClip randomClip = validClips[Random.Range(0, validClips.Count)];

            // Reproduz o som
            _audioSource.PlayOneShot(randomClip, _soundVolume);
        }

        /// <summary>
        /// Permite disparar o contato via código
        /// </summary>
        /// <param name="contactor">Objeto que está causando o contato (opcional)</param>
        /// <param name="isEntering">Se é um contato de entrada (true) ou saída (false)</param>
        public void TriggerContact(GameObject contactor = null, bool isEntering = true)
        {
            HandleContact(contactor != null ? contactor : gameObject, isEntering);
        }
    }
}