using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TheSlimeKing.Core;

namespace TheSlimeKing.Gameplay.Interactive
{
    /// <summary>
    /// Objeto que pode ser empurrado ou puxado pelo jogador
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovableObject : InteractableObject
    {
        [Header("Configurações de Movimento")]
        [SerializeField] private float _moveSpeed = 2f;
        [SerializeField] private float _maxPushDistance = 5f;
        [SerializeField] private float _pushDeceleration = 1f;
        [SerializeField] private bool _canBePulled = true;
        [SerializeField] private float _maxMass = 10f; // Massa base para cálculos de movimento
        [SerializeField] private float _pushForce = 100f;

        [Header("Restrições")]
        [SerializeField] private bool _lockXAxis = false;
        [SerializeField] private bool _lockYAxis = false;
        [SerializeField] private bool _requiresSpecificGrowthStage = false;
        [SerializeField] private int _minimumGrowthStage = 2; // 0=Baby, 1=Young, 2=Adult, 3=King

        [Header("Feedback")]
        [SerializeField] private AudioClip _pushStartSound;
        [SerializeField] private AudioClip _pushLoopSound;
        [SerializeField] private AudioClip _pushEndSound;
        [SerializeField] private GameObject _pushParticlePrefab;
        [SerializeField] private bool _enableTrail = false;
        [SerializeField] private GameObject _trailPrefab;
        [SerializeField] private float _trailSpacing = 0.5f;

        // Componentes
        private Rigidbody2D _rb;
        private AudioSource _audioSource;
        private List<GameObject> _trailInstances = new List<GameObject>();

        // Estado
        private bool _isBeingPushed = false;
        private GameObject _pusher;
        private Vector3 _startingPosition;
        private Vector3 _lastTrailPosition;
        private Coroutine _trailCoroutine;

        protected override void Awake()
        {
            base.Awake();

            _rb = GetComponent<Rigidbody2D>();
            _rb.freezeRotation = true;

            // Busca AudioSource ou cria um
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null && (_pushStartSound != null || _pushLoopSound != null || _pushEndSound != null))
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.loop = false;
                _audioSource.volume = 0.7f;
                _audioSource.spatialBlend = 1f;
            }

            _startingPosition = transform.position;
            _lastTrailPosition = _startingPosition;
        }

        public override void Interact(GameObject interactor)
        {
            base.Interact(interactor);

            // Verificar estágio de crescimento se necessário
            if (_requiresSpecificGrowthStage)
            {
                int playerGrowthStage = GetPlayerGrowthStage(interactor);
                if (playerGrowthStage < _minimumGrowthStage)
                {
                    // Feedback de que o jogador não pode mover este objeto ainda
                    Debug.Log("O jogador não está grande o suficiente para mover este objeto!");
                    return;
                }
            }

            // Lógica de empurrar/puxar
            StartPushing(interactor);
        }

        /// <summary>
        /// Inicia o processo de empurrar o objeto
        /// </summary>
        private void StartPushing(GameObject pusher)
        {
            if (_isBeingPushed)
                return;

            _isBeingPushed = true;
            _pusher = pusher;

            // Som de início de empurrar
            if (_pushStartSound != null && _audioSource != null)
            {
                _audioSource.clip = _pushStartSound;
                _audioSource.Play();

                // Configura loop de som se disponível
                if (_pushLoopSound != null)
                {
                    _audioSource.PlayDelayed(_pushStartSound.length);
                    _audioSource.loop = true;
                }
            }

            // Inicia sistema de rastros se habilitado
            if (_enableTrail && _trailPrefab != null)
            {
                _trailCoroutine = StartCoroutine(CreateTrail());
            }
        }

        /// <summary>
        /// Finaliza o movimento do objeto
        /// </summary>
        private void StopPushing()
        {
            if (!_isBeingPushed)
                return;

            _isBeingPushed = false;

            // Para som de loop se estiver tocando
            if (_audioSource != null && _audioSource.isPlaying)
            {
                _audioSource.loop = false;

                if (_pushEndSound != null)
                {
                    _audioSource.clip = _pushEndSound;
                    _audioSource.Play();
                }
                else
                {
                    _audioSource.Stop();
                }
            }

            // Para geração de rastro
            if (_trailCoroutine != null)
            {
                StopCoroutine(_trailCoroutine);
                _trailCoroutine = null;
            }
        }

        private void Update()
        {
            if (_isBeingPushed && _pusher != null)
            {
                // Verifica se o jogador ainda está próximo
                float distance = Vector3.Distance(transform.position, _pusher.transform.position);
                if (distance > _interactionRadius * 1.2f) // Um pouco mais que o raio original
                {
                    StopPushing();
                    return;
                }

                // Verifica se atingiu a distância máxima de empurrão
                float distanceFromStart = Vector3.Distance(transform.position, _startingPosition);
                if (_maxPushDistance > 0 && distanceFromStart >= _maxPushDistance)
                {
                    // Reduz ou para o movimento se atingiu o limite
                    _rb.linearVelocity = Vector2.Lerp(_rb.linearVelocity, Vector2.zero, Time.deltaTime * _pushDeceleration);

                    if (_rb.linearVelocity.magnitude < 0.1f)
                    {
                        StopPushing();
                    }

                    return;
                }

                // Aplica força de movimento baseado na posição do jogador
                MoveBasedOnPlayerInput();
            }
        }

        /// <summary>
        /// Movimenta o objeto baseado na posição e input do jogador
        /// </summary>
        private void MoveBasedOnPlayerInput()
        {
            if (_pusher == null)
                return;

            // Obtém direção do empurrão (do jogador para o objeto)
            Vector2 pushDirection = (transform.position - _pusher.transform.position).normalized;

            // Aplica restrições de eixo, se configurado
            if (_lockXAxis) pushDirection.x = 0;
            if (_lockYAxis) pushDirection.y = 0;

            if (pushDirection != Vector2.zero)
            {
                // Escala força com base na massa
                float scaledForce = _pushForce / Mathf.Max(1f, _rb.mass / _maxMass);

                // Aplica força ao objeto
                _rb.AddForce(pushDirection * scaledForce * Time.deltaTime);

                // Limita velocidade máxima
                if (_rb.linearVelocity.magnitude > _moveSpeed)
                {
                    _rb.linearVelocity = _rb.linearVelocity.normalized * _moveSpeed;
                }

                // Efeitos visuais enquanto se move
                if (_pushParticlePrefab != null && _rb.linearVelocity.magnitude > 0.5f)
                {
                    if (Random.Range(0f, 1f) < 0.05f) // Controla frequência de partículas
                    {
                        Vector3 spawnPos = transform.position - (Vector3)pushDirection * 0.5f;
                        GameObject particleInstance = Instantiate(_pushParticlePrefab, spawnPos, Quaternion.identity);
                        Destroy(particleInstance, 2f);
                    }
                }
            }
        }

        /// <summary>
        /// Gera rastros enquanto o objeto é movido
        /// </summary>
        private IEnumerator CreateTrail()
        {
            while (_isBeingPushed)
            {
                // Cria um rastro apenas se moveu o suficiente
                float distanceTraveled = Vector3.Distance(transform.position, _lastTrailPosition);

                if (distanceTraveled >= _trailSpacing && _rb.linearVelocity.magnitude > 0.5f)
                {
                    // Cria uma instância do rastro
                    GameObject trail = Instantiate(_trailPrefab, _lastTrailPosition, transform.rotation);
                    _trailInstances.Add(trail);

                    // Ajusta tamanho com base na velocidade
                    float sizeMultiplier = Mathf.Clamp(_rb.linearVelocity.magnitude / _moveSpeed, 0.5f, 1.5f);
                    trail.transform.localScale *= sizeMultiplier;

                    // Configura fade out do rastro após um tempo
                    StartCoroutine(FadeOutTrail(trail));

                    // Atualiza última posição
                    _lastTrailPosition = transform.position;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        /// <summary>
        /// Desaparece com o rastro gradualmente
        /// </summary>
        private IEnumerator FadeOutTrail(GameObject trail)
        {
            SpriteRenderer spriteRenderer = trail.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                Color originalColor = spriteRenderer.color;
                float duration = 3f; // Tempo até desaparecer
                float elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / duration);
                    spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }

            // Remove da lista e destrói
            if (_trailInstances.Contains(trail))
            {
                _trailInstances.Remove(trail);
            }

            Destroy(trail);
        }

        /// <summary>
        /// Obtém o estágio de crescimento do jogador
        /// </summary>
        private int GetPlayerGrowthStage(GameObject player)
        {
            // Aqui você deve implementar a lógica para detectar o estágio
            // de crescimento atual do jogador. Por enquanto, retorna um valor padrão.

            // Exemplo (substituir pela real implementação):
            var playerGrowth = player.GetComponent<PlayerGrowth>();
            if (playerGrowth != null)
            {
                return (int)playerGrowth.GetCurrentStage();
            }

            return 0; // Estágio base
        }

        /// <summary>
        /// Limpa rastros quando o objeto é destruído
        /// </summary>
        private void OnDestroy()
        {
            foreach (GameObject trail in _trailInstances)
            {
                if (trail != null)
                {
                    Destroy(trail);
                }
            }
        }
    }
}
