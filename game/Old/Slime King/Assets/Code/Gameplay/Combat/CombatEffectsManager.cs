using UnityEngine;
using SlimeKing.Utils;

namespace SlimeKing.Gameplay.Combat
{
    /// <summary>
    /// Gerencia efeitos visuais e sonoros de combate usando um sistema de pooling
    /// </summary>
    public class CombatEffectsManager : MonoBehaviour
    {
        private static CombatEffectsManager instance;
        public static CombatEffectsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("CombatEffectsManager");
                    instance = go.AddComponent<CombatEffectsManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        [Header("Pool Settings")]
        [SerializeField] private int poolInitialSize = 10;
        [SerializeField] private int poolMaxSize = 20;

        [Header("Visual Effects")]
        [SerializeField] private GameObject hitEffectPrefab;

        [Header("Audio Effects")]
        [SerializeField] private AudioClip[] hitSounds;
        [SerializeField] private AudioClip[] criticalHitSounds;

        private AudioSource audioSource;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound
        }

        /// <summary>
        /// Spawns a visual hit effect at the given position
        /// </summary>
        public void SpawnHitEffect(Vector3 position, Quaternion rotation)
        {
            if (hitEffectPrefab != null)
            {
                var effect = EffectPool.Instance.GetEffect(hitEffectPrefab, position, rotation);

                // Effect will be returned to pool when the particle system completes
                var autoReturn = effect.gameObject.AddComponent<AutoReturnToPool>();
                autoReturn.Initialize(effect, hitEffectPrefab);
            }
        }

        /// <summary>
        /// Plays a hit sound at the given position
        /// </summary>
        public void PlayHitSound(Vector3 position, bool isCritical = false)
        {
            if (audioSource != null)
            {
                AudioClip[] soundArray = isCritical ? criticalHitSounds : hitSounds;
                if (soundArray != null && soundArray.Length > 0)
                {
                    audioSource.transform.position = position;
                    audioSource.clip = soundArray[Random.Range(0, soundArray.Length)];
                    audioSource.Play();
                }
            }
        }
    }

    /// <summary>
    /// Component to automatically return particle effects to the pool when they complete
    /// </summary>
    public class AutoReturnToPool : MonoBehaviour
    {
        private ParticleSystem psComponent;
        private GameObject prefab;
        private bool isReturning;

        public void Initialize(ParticleSystem ps, GameObject original)
        {
            psComponent = ps;
            prefab = original;
            isReturning = false;
        }
        private void Update()
        {
            if (!isReturning && psComponent != null && !psComponent.IsAlive())
            {
                isReturning = true;
                EffectPool.Instance.ReturnToPool(psComponent, prefab);
                Destroy(this); // Remove this component after returning to pool
            }
        }
    }
}